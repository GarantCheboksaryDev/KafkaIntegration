using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.CustomContracts.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Получить директора по направлению.
    /// </summary>
    /// <param name="departmentFromDoc">Подразделение.</param>
    /// <returns>Директор по направлению. Если директор по направлению не найден или совпадает с руководителем НОР, то функция возвращает null.</returns>
    [Public]
    public vf.OverrideBaseDev.IEmployee GetDirectionManager(Sungero.Company.IDepartment departmentFromDoc)
    {
      var department = departmentFromDoc;
      var directionManager = OverrideBaseDev.Employees.Null;
      while (directionManager == null && department != null)
      {
        var manager = OverrideBaseDev.Employees.As(department.Manager);
        if (manager?.IsDirectionManager == true)
          directionManager = manager;
        else
          department = department?.HeadOffice;
      }
      if (directionManager != null && department != null && !OverrideBaseDev.Employees.Equals(directionManager, department.BusinessUnit?.CEO))
        return directionManager;
      
      return null;
    }
    
    /// <summary>
    /// Получить бюджетного контролера.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Бюджетный контролер.</returns>
    [Public]
    public vf.OverrideBaseDev.IEmployee GetBudgetController(vf.OverrideBaseDev.IContractualDocument document)
    {
      if (document == null)
        return vf.OverrideBaseDev.Employees.Null;
      
      if (document.CFO != null && document.CFO.BudgetController != null)
        return document.CFO.BudgetController;
      
      return vf.OverrideBaseDev.Employees.Null;
    }
       
    /// <summary>
    /// Получить текст хинта "Доверенность подписывающего действительна с <Дата> по <Дата>" в карточке договора, доп. соглашения и задаче на согласование по регламенту.
    /// </summary>
    /// <param name="signatureSetting">Право подписи.</param>
    /// <returns>Если в переданном праве подписи в типе основания указано "Доверенность" или "Эл. доверенность"
    /// и дата окончания действия доверенности в праве подписи истекает в течении 30 дней от текущей даты, то возвращается текст хинта с проставленными датами действия.
    /// Иначе, возвращается пустая строка.</returns>
    [Public, Remote(IsPure = true)]
    public string GetPowerOfAttoneyHint(Sungero.Docflow.ISignatureSetting signatureSetting)
    {
      if (signatureSetting != null
          && (signatureSetting.Reason == Sungero.Docflow.SignatureSetting.Reason.PowerOfAttorney || signatureSetting.Reason == Sungero.Docflow.SignatureSetting.Reason.FormalizedPoA)
          && signatureSetting.ValidTill.HasValue
          && signatureSetting.ValidTill.Value <= Calendar.Today.AddDays(Constants.Module.AmountOfDaysForCheckingPowerOfAttoney))
      {
        var validFrom = signatureSetting.ValidFrom.HasValue ? signatureSetting.ValidFrom.Value.Date.ToShortDateString() : string.Empty;
        var validTill = signatureSetting.ValidTill.Value.Date.ToShortDateString();
        return vf.CustomContracts.Resources.PowerOfAttoneyHintFormat(validFrom, validTill);
      }
      
      return string.Empty;
    }
    
    /// <summary>
    /// Отправка инициатору уведомления о просроченном задании в рамках задачи на согласование по регламенту.
    /// </summary>
    /// <param name="assignment">Задание, оправленное в рамках задачи на согласование по регламенту.</param>
    public void SendOverdueApprovalAssignmentsNotification(Sungero.Workflow.IAssignmentBase assignment)
    {
      var initiator = assignment.Author;
      var personalSettings = OverrideBaseDev.PersonalSettings.GetAll(x => Sungero.Company.Employees.Equals(x.Employee, initiator)).FirstOrDefault();
      if (personalSettings == null || personalSettings.OverdueApprovalAssignmentsNotification != true)
        return;
      
      var approvalTask = OverrideBaseDev.ApprovalTasks.As(assignment.MainTask);
      if (approvalTask == null)
        return;
      
      var document = approvalTask.DocumentGroup.OfficialDocuments.FirstOrDefault();
      var subject = document != null ? vf.CustomContracts.Resources.OverdueApprovalAssignmentNotificationSubjectFormat(document.Name) : vf.CustomContracts.Resources.OverdueApprovalAssignmentNotificationSubject;
      var notification = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, initiator);
      notification.Attachments.Add(assignment);
      notification.Start();
      
      var sentNotification = CustomContracts.SentNotifications.Create();
      sentNotification.AssignmentId = assignment.Id;
      sentNotification.Save();
    }
    
    /// <summary>
    /// Проверить необходимость заполнения свойства "Оригинал договора в наличии".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>True, если необходимо заполнить свойство "Оригинал договора в наличии", иначе - False.</returns>
    [Public]
    public bool IsOriginalContractAvailable(Sungero.Docflow.IOfficialDocument document)
    {
      return IsOriginalContractAvailable(document, null);
    }
    
    /// <summary>
    /// Проверить необходимость заполнения свойства "Оригинал договора в наличии".
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="signature">Подпись.</param>
    /// <returns>True, если необходимо заполнить свойство "Оригинал договора в наличии", иначе - False.</returns>
    [Public]
    public bool IsOriginalContractAvailable(Sungero.Docflow.IOfficialDocument document, Sungero.Domain.Shared.ISignature signature)
    {
      if (document == null || document.LastVersion == null)
        return false;
      
      // Состояние - действующий.
      var isActive = document.LifeCycleState == Sungero.Docflow.OfficialDocument.LifeCycleState.Active;
      
      // Проверка для СФ полученных, СФ выставленных и исходящих счетов.
      if (Sungero.FinancialArchive.IncomingTaxInvoices.Is(document) || Sungero.FinancialArchive.OutgoingTaxInvoices.Is(document) || Sungero.Contracts.OutgoingInvoices.Is(document))
      {
        var isExchange = document.ExchangeState != null;
        return isActive && isExchange;
      }
      
      // Документ зарегистрирован.
      var isRegistered = document.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered;
      
      var signatures = Signatures.Get(document.LastVersion).Where(x => x.SignatureType == SignatureType.Approval && x.SignCertificate != null);
      // Подписан УКЭП контрагентом.
      var hasExternalSignature = signatures.Any(x => x.IsExternal == true);
      // Подписан УКЭП с нашей стороны.
      var hasOwnSignature = signatures.Any(x => x.IsExternal != true)
        || signature != null && signature.IsExternal != true;
      
      return isRegistered && isActive && hasExternalSignature && hasOwnSignature;
    }
    
    /// <summary>
    /// Проверка вхождения текущего пользователя в роль "Сотрудник для отправки задания о постановке на валютный контроль" или его замещение, и сохранение результата в параметры сущности.
    /// </summary>
    /// <param name="entityParams">Хранилище параметров типа сущности.</param>
    /// <returns>True, если пользователь входит в роль или является замеющающим пользователя, входящего в роль, иначе - false.</returns>
    [Public]
    public static bool IncludeInCurrencyControlPerformerRole(Sungero.Domain.Shared.ParamsDictionary entityParams)
    {
      var includeInCurrencyControlPerformerRole = false;
      var paramName = Constants.Module.Params.IncludeInCurrencyControlPerformerRole;
      
      if (!entityParams.TryGetValue(paramName, out includeInCurrencyControlPerformerRole))
      {
        includeInCurrencyControlPerformerRole = vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.IncludedInRole(Constants.Module.Roles.CurrencyControlAuthorRole);
        
        if (!includeInCurrencyControlPerformerRole)
          includeInCurrencyControlPerformerRole = vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.UserOrSubstitutionIncludedInRole(Constants.Module.Roles.CurrencyControlAuthorRole);
        
        entityParams.AddOrUpdate(paramName, includeInCurrencyControlPerformerRole);
      }
      
      return includeInCurrencyControlPerformerRole;
    }
    
    /// <summary>
    /// Преобразовать список строк в коллекцию ид.
    /// </summary>
    /// <param name="source">Исходная строка.</param>
    /// <param name="logPrefix">Префикс лог файла.</param>
    /// <returns>Коллекция ид.</returns>
    public List<long> GetIdListByString(string source, string logPrefix)
    {
      var ids = new List<long>();
      if (string.IsNullOrWhiteSpace(source))
        return ids;
      Logger.Debug($"{logPrefix} Преобразование ids: {source}.");
      try
      {
        ids = source.Split(',').ToList().ConvertAll(long.Parse);
      }
      catch (Exception ex)
      {
        Logger.Error($"{logPrefix} Ошибка во время преобразования: {ex.Message}", ex);
      }
      return ids;
    }
  }
}