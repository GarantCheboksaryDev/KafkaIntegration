using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.CustomContracts.Server
{
  public class ModuleAsyncHandlers
  {
    /// <summary>
    /// Заполннение рейса в карточке документа.
    /// </summary>
    /// <param name="args">Аргументы асинхронного обработчика.</param>
    public virtual void FillInDocumentVoyage(vf.CustomContracts.Server.AsyncHandlerInvokeArgs.FillInDocumentVoyageInvokeArgs args)
    {
      var voyageId = args.VoyageId;
      var prefix = string.Format("FillInDocumentVoyage. Ид Рейса: {0}. ", voyageId);

      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var voyage = vf.CustomContracts.Voyages.GetAll(x => x.Id == voyageId).FirstOrDefault();
      if (voyage == null)
      {
        Logger.ErrorFormat("{0}Не найден рейс.", prefix);
        return;
      }
      
      try
      {
        var skipContractualDocumentsIds = Functions.Module.GetIdListByString(args.SkipContractualDocumentIds, prefix);
        
        var contractualDocuments = voyage.Contracts.Select(x => x.Contract).Where(x => vf.OverrideBaseDev.ContractualDocuments.Is(x)
                                                                                  && (skipContractualDocumentsIds.Any() && skipContractualDocumentsIds.Contains(x.Id)
                                                                                      || !skipContractualDocumentsIds.Any())).Cast<vf.OverrideBaseDev.IContractualDocument>();
        
        var actualContractualDocument = contractualDocuments.Take(Constants.Module.MaxRecordCountProcessing);
        
        var skipVoyagesDocumentsIds = Functions.Module.GetIdListByString(args.SkipDeletedVoyagesIds, prefix);
        
        var voyageDocuments = vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.GetVoyageDocuments(voyage).Where(x => skipVoyagesDocumentsIds.Any() && !skipVoyagesDocumentsIds.Contains(x.Id)
                                                                                                                               || !skipVoyagesDocumentsIds.Any());
        var actualVoyageDocuments = voyageDocuments.Take(Constants.Module.MaxRecordCountProcessing);
        
        if (actualContractualDocument.Any())
        {
          foreach (var document in actualContractualDocument)
          {
            if (CustomContracts.MVZs.Equals(OverrideBaseDev.Departments.As(document.Department)?.MVZ, voyage.Vessel) && !document.Voyages.Any(x => vf.CustomContracts.Voyages.Equals(x.Voyage, voyage)))
              document.Voyages.AddNew().Voyage = voyage;
            else if (!CustomContracts.MVZs.Equals(OverrideBaseDev.Departments.As(document.Department)?.MVZ, voyage.Vessel) && document.Voyages.Any(x => vf.CustomContracts.Voyages.Equals(x.Voyage, voyage)))
            {
              var deletedVoyage = document.Voyages.Where(x => vf.CustomContracts.Voyages.Equals(voyage, x.Voyage)).FirstOrDefault();
              if (deletedVoyage != null)
                document.Voyages.Remove(deletedVoyage);
            }
            document.Save();
          }
        }
        
        if (actualVoyageDocuments.Any())
        {
          foreach (var document in actualVoyageDocuments)
          {
            if (!contractualDocuments.Contains(document))
            {
              var deletedVoyage = document.Voyages.Where(x => vf.CustomContracts.Voyages.Equals(voyage, x.Voyage)).FirstOrDefault();
              if (deletedVoyage != null)
              {
                document.Voyages.Remove(deletedVoyage);
                document.Save();
              }
            }
          }
        }
        
        if (actualContractualDocument.Skip(Constants.Module.MaxRecordCountProcessing).Any() || actualVoyageDocuments.Skip(Constants.Module.MaxRecordCountProcessing).Any())
        {
          skipContractualDocumentsIds.AddRange(actualContractualDocument.Select(x => x.Id));
          skipVoyagesDocumentsIds.AddRange(actualVoyageDocuments.Select(x => x.Id));
          
          args.SkipContractualDocumentIds = string.Join(",", skipVoyagesDocumentsIds);
          
          args.SkipDeletedVoyagesIds = string.Join(",", skipVoyagesDocumentsIds);
          
          args.Retry = true;
        }
        
        Logger.DebugFormat("{0}Обработка рейса выполнена.", prefix);
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("{0}Произошла ошибка при заполнении/удалении рейса в документах. {1}", prefix, ex);
        args.Retry = true;
      }
    }
    
    /// <summary>
    /// Отмена резервирования документа.
    /// </summary>
    /// <param name="args">Аргументы асинхронного обработчика.</param>
    public virtual void CancelReservation(vf.CustomContracts.Server.AsyncHandlerInvokeArgs.CancelReservationInvokeArgs args)
    {
      var documentId = args.DocumentId;
      var prefix = string.Format("CancelReservation. Ид документа: {0}. ", documentId);

      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var document = Sungero.Docflow.OfficialDocuments.GetAll(x => x.Id == documentId).FirstOrDefault();
      if (document == null)
      {
        Logger.ErrorFormat("{0}Не найден документ.", prefix);
        return;
      }

      try
      {
        document.State.Properties.RegistrationNumber.IsRequired = false;
        document.State.Properties.RegistrationDate.IsRequired = false;
        document.State.Properties.DocumentRegister.IsRequired = false;
        
        // Не показывать хинт о перерегистрации.
        var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)document)?.Params;
        if (paramsDictionary != null)
        {
          object repeatRegister;
          if (paramsDictionary.TryGetValue(Sungero.Docflow.Constants.OfficialDocument.RepeatRegister, out repeatRegister) && (bool)repeatRegister)
            paramsDictionary.Remove(Sungero.Docflow.Constants.OfficialDocument.RepeatRegister);
        }
        
        Sungero.Docflow.PublicFunctions.OfficialDocument.RegisterDocument(document, Sungero.Docflow.DocumentRegisters.Null, null, null, false, true);
        Logger.DebugFormat("{0}Резервирование документа успешно отменено.", prefix);
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("{0}Произошла ошибка при отмене резервирования документа. {1}", prefix, ex);
        args.Retry = true;
      }
    }
    
    /// <summary>
    /// Создание задачи на отправку договора на валютный контроль.
    /// </summary>
    /// <param name="args">Аргументы асинхронного обработчика.</param>
    public virtual void SendContractOnCurrencyControl(vf.CustomContracts.Server.AsyncHandlerInvokeArgs.SendContractOnCurrencyControlInvokeArgs args)
    {
      var contractId = args.ContractId;
      
      var prefix = string.Format("SendContractOnCurrencyControl. Ид договора: {0}. ", contractId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var contract = OverrideBaseDev.ContractBases.GetAll(x => x.Id == args.ContractId).FirstOrDefault();
      if (contract == null)
      {
        Logger.ErrorFormat("{0}Не найден договор.", prefix);
        return;
      }
      
      Functions.SetOnCurrencyControl.CreateTaskSetOnCurrencyControl(contract);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }

    /// <summary>
    /// Заполнить итоговую сумму с ДДД в карточке договора.
    /// </summary>
    /// <param name="NeedDeletePreviousSum">Необходимость вычитания суммы ДДД из итоговой суммы договора.</param>
    /// <param name="PreviousAmount">Предыдущая сумма ДДД.</param>
    /// <param name="SupAgreementId">Id доп. соглашения.</param>
    public virtual void FillInContractTotalSum(vf.CustomContracts.Server.AsyncHandlerInvokeArgs.FillInContractTotalSumInvokeArgs args)
    {
      var supAgreementId = args.SupAgreementId;
      var prefix = string.Format("FillInContractTotalSum. Id доп. соглашения: {0}. ", supAgreementId);
      
      try
      {
        Logger.DebugFormat("{0}Старт процесса.", prefix);
        var supAgreement = OverrideBaseDev.SupAgreements.GetAll(x => x.Id == supAgreementId).FirstOrDefault();
        if (supAgreement == null)
        {
          Logger.ErrorFormat("{0}Не найдено доп. соглашение.", prefix);
          return;
        }
        
        var contract = supAgreement.LeadingDocument;
        var originalTotalSum = contract.TotalSum.HasValue ? contract.TotalSum.Value : 0;
        var totalSum = originalTotalSum;
        
        var supAgreementRelationDocumetns = contract.Relations.GetRelatedDocuments(Sungero.Contracts.PublicConstants.Module.SupAgreementRelationName)
          .Where(x => vf.OverrideBaseDev.SupAgreements.Is(x) && vf.OverrideBaseDev.SupAgreements.As(x).LifeCycleState == vf.OverrideBaseDev.SupAgreement.LifeCycleState.Active)
          .Cast<vf.OverrideBaseDev.ISupAgreement>().ToList();

        supAgreementRelationDocumetns = supAgreementRelationDocumetns.OrderBy(x => x.SignDate).ThenBy(x => x.SignDateTime).ToList();
        
        DateTime? maxSingingDate = supAgreementRelationDocumetns.Select(x => x.SignDate).Max();

        if (!maxSingingDate.HasValue)
        {
          Logger.ErrorFormat("{0}Не указана дата подписания.", prefix);
          return;
        }
        
        foreach (var supAgreementRelationDocument in supAgreementRelationDocumetns)
        {
          if (supAgreementRelationDocument.TotalAmountAction == OverrideBaseDev.SupAgreement.TotalAmountAction.Union)
            totalSum += vf.OverrideBaseDev.PublicFunctions.ContractualDocument.GetRubSum(supAgreementRelationDocument, maxSingingDate);
          else if (supAgreementRelationDocument.TotalAmountAction == OverrideBaseDev.SupAgreement.TotalAmountAction.Replace)
            totalSum = vf.OverrideBaseDev.PublicFunctions.ContractualDocument.GetRubSum(supAgreementRelationDocument, maxSingingDate);
        }
        
        if (Locks.GetLockInfo(contract).IsLockedByOther)
        {
          Logger.DebugFormat("{0}Не удалось занести итоговую сумму. Договор с ИД: {1} заблокирован.", prefix, contract.Id);
          args.Retry = true;
          return;
        }
        
        if (originalTotalSum != totalSum)
        {
          contract.TotalSum = totalSum;
          contract.Save();
          
          var operation = new Enumeration(vf.OverrideBaseDev.PublicConstants.Contracts.ContractualDocument.HistoryOperation.SDChange);
          var comment = string.Format("Итоговая сумма с ДДД (в руб.) изменена {0} с {1} на {2}", supAgreement.Name, Math.Round(originalTotalSum, 2), Math.Round(totalSum, 2));
          contract.History.Write(operation, operation, comment);
          
          Logger.DebugFormat("{0}Итоговая сумма с ДДД успешно занесена в карточку договора.", prefix);
        }
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("{0}Произошла ошибка при занесении итоговой суммы ДДД в карточку договора. {1}", prefix, ex);
        args.Retry = true;
      }
    }
  }

}