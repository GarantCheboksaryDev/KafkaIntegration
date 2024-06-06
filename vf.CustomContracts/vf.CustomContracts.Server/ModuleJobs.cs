using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.Net;
using System.Xml.Linq;

namespace vf.CustomContracts.Server
{
  public class ModuleJobs
  {
    /// <summary>
    /// Уведомление о постановке на валютный контроль.
    /// </summary>
    public virtual void SendContractOnCurrencyControl()
    {
      var prefix = "SendContractOnCurrencyControl. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Получить настройки модуля "Договоры" для всех НОР.
      var settings = Functions.ContractSetting.GetAllBusinessUnitsContractSettings();
      
      if (settings.Any())
      {
        var contractsToSend = new List<OverrideBaseDev.IContractBase>();
        
        foreach (var setting in settings)
        {
          var businessUnit = setting.BusinessUnit;
          var importLimit = setting.OutcomeContractTotalAmount;
          var exportLimit = setting.IncomeContractTotalAmount;
          var prepaymentType = setting.PrepaymentCheckingCondition;

          Logger.DebugFormat("{0}Обработка настроек для {1}.", prefix, businessUnit);
          
          if (!importLimit.HasValue)
            Logger.DebugFormat("{0}Не заполнен лимит для импортных договоров", prefix);
          
          if (!exportLimit.HasValue)
            Logger.DebugFormat("{0}Не заполнен лимит для экспортных договоров", prefix);
          
          
          var contracts = OverrideBaseDev.ContractBases.GetAll(x => x.SendToCurrencyControl != true
                                                               && x.TotalSum.HasValue
                                                               && x.RelationshipType != null
                                                               && x.CounterpartyNonresident == true
                                                               && x.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Active
                                                               && x.BusinessUnit != null && OverrideBaseDev.BusinessUnits.Equals(x.BusinessUnit, businessUnit)
                                                               && x.PaymentType != null && CustomContracts.PaymentTypes.Equals(x.PaymentType, prepaymentType)
                                                               && (importLimit.HasValue && x.RelationshipType.ContractType == CustomContracts.RelationshipType.ContractType.Outcome
                                                                   && x.TotalSum >= importLimit
                                                                   || exportLimit.HasValue && x.RelationshipType.ContractType == CustomContracts.RelationshipType.ContractType.Income
                                                                   && x.TotalSum >= exportLimit));
          
          Logger.DebugFormat("{0}Наша организация: {1}. Договоров для отправки: {2}", prefix, businessUnit, contracts.Count());
          
          if (contracts.Any())
            contractsToSend.AddRange(contracts);
        }
        
        if (contractsToSend.Any())
        {
          foreach (var contractToSend in contractsToSend)
            OverrideBaseDev.PublicFunctions.ContractualDocument.Remote.SendContractOnCurrencyControl(contractToSend);
        }
      }
      else
        Logger.DebugFormat("{0}Не найдены настройки модуля \"Договоры\"", prefix);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }

    /// <summary>
    /// Для каждой записи справочника Валюты с Состоянием = «Действующая» ФП находит курс на текущую дату на сайте ЦБ РФ и создает для нее запись в справочнике Курсы валют.
    /// </summary>
    public virtual void GetCurrenciesRate()
    {
      var prefix = "GetCurrenciesRate. ";
      try
      {
        Logger.DebugFormat("{0}Начало процесса.", prefix);
        using (var client = new WebClient())
        {
          var setting = IntegrationSettings.ConnectSettingses.GetAll(x => x.SystemName == IntegrationSettings.ConnectSettings.SystemName.CBRF).FirstOrDefault();
          if (setting == null)
          {
            Logger.ErrorFormat("{0}Не задана ссылка на сайт ЦБ в Настройках модуля ", prefix);
            return;
          }
          
          var xml = client.DownloadString(setting.WebServiceAddressee);
          Logger.DebugFormat("{0}XML с ЦБ РФ загружен", prefix);
          var xdoc = XDocument.Parse(xml);
          Logger.Debug("{0}XML с ЦБ РФ считан", prefix);
          
          var dateString = xdoc.Element(Constants.Module.RatesXmlParams.ValCurs)?.FirstAttribute?.Value;
          var date = DateTime.Parse(dateString);
          var elements = xdoc.Element(Constants.Module.RatesXmlParams.ValCurs)?.Elements(Constants.Module.RatesXmlParams.Valute);
          
          foreach (var element in elements)
          {
            try
            {
              // Поиск записи спрввочника валюты по цифровому коду.
              var numCode = element.Element(Constants.Module.RatesXmlParams.NumCode)?.Value;
              var currencies = Sungero.Commons.Currencies.GetAll(x => x.NumericCode == numCode && x.Status == Sungero.Commons.Currency.Status.Active);
              if (!currencies.Any())
              {
                Logger.DebugFormat("{0}Не найдены валюты с кодом {1}", prefix, numCode);
                continue;
              }
              
              // Получение курса валюты из файла.
              foreach (var currency in currencies)
              {
                var rateString = element.Element(Constants.Module.RatesXmlParams.Value).Value;
                double rate = 0;
                if (!double.TryParse(rateString, out rate))
                {
                  Logger.ErrorFormat("{0}Не удалось получить курс для валюты с кодом {1}", prefix, numCode);
                  continue;
                }
                
                // Создание/обновление записи справочника "Курсы валют".
                if (rate != 0)
                {
                  var currencyRate = CurrencyRates.GetAll(x => Sungero.Commons.Currencies.Equals(x.Currency, currency) && x.Date.HasValue && x.Date == date).FirstOrDefault();
                  if (currencyRate == null)
                    currencyRate = CurrencyRates.Create();
                  
                  if (currencyRate.Currency != currency)
                    currencyRate.Currency = currency;
                  if (currencyRate.Date != date)
                    currencyRate.Date = date;
                  if (currencyRate.Rate != rate)
                    currencyRate.Rate = rate;
                  
                  if (currencyRate.State.IsChanged)
                    currencyRate.Save();
                  
                  Logger.DebugFormat("{0}Создана запись справочника Курсы валют {1}", prefix, currencyRate.DisplayValue);
                }
              }
            }
            catch (Exception ex)
            {
              Logger.ErrorFormat("{0}Во время создания записи справочника курсы валют возникла ошибка {1}", prefix, ex.ToString());
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("{0}Во время выполнения процесса возникла ошибка {1}", prefix, ex.ToString());
      }
    }
    
    /// <summary>
    /// Отправка инициаторам уведомлений о просроченных заданиях в рамках задачи на согласование по регламенту.
    /// </summary>
    public virtual void SendOverdueApprovalAssignmentsNotifications()
    {
      var prefix = "SendOverdueApprovalAssignmentsNotifications. ";
      Logger.DebugFormat("{0}Старт фонового процесса.", prefix);
      
      // Получить список Id заданий, по которым уже отправлены уведомления.
      var sentNotifications = CustomContracts.SentNotifications.GetAll().Select(x => x.AssignmentId);
      
      var assignments = Sungero.Workflow.AssignmentBases.GetAll(x => x.Deadline < Calendar.Now
                                                                && x.Status == Sungero.Workflow.AssignmentBase.Status.InProcess
                                                                && !sentNotifications.Contains(x.Id)
                                                                && (Sungero.Docflow.ApprovalAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalCheckingAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalCheckReturnAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalExecutionAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalManagerAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalPrintingAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalRegistrationAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalReviewAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalReworkAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalSendingAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalSigningAssignments.Is(x)
                                                                    || Sungero.Docflow.ApprovalSimpleAssignments.Is(x)));
      foreach (var assignment in assignments)
      {
        try
        {
          Functions.Module.SendOverdueApprovalAssignmentsNotification(assignment);
          Logger.DebugFormat("{0}Уведомление по заданию с Id {1} успешно отправлено.", prefix, assignment?.Id);
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}При отправке уведомления по заданию с Id {1} произошла ошибка {2}.", prefix, assignment?.Id, ex);
        }
      }
      Logger.DebugFormat("{0}Завершение фонового процесса.", prefix);
    }

  }
}