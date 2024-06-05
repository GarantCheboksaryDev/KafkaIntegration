using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.KafkaIntegration.Server
{
  public class ModuleAsyncHandlers
  {

    #region Расчетные счета наших организаций
    
    /// <summary>
    /// Обновить информацию о расчетных счетах наших организаций.
    /// </summary>
    /// <param name="args">Аргументы АО.</param>
    public virtual void UpdateOurPaymentAccountsInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateOurPaymentAccountsInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateOurPaymentAccountsInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      // Дессериализовать Json.
      var paymentAccountInfo = IsolatedFunctions.DeserializeObject.DesirializeOurPaymentAccountsInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (paymentAccountInfo != null)
      {
        try
        {
          var guid1C = !string.IsNullOrEmpty(paymentAccountInfo.Guid1C) ? paymentAccountInfo.Guid1C.Trim() : string.Empty;
          
          var sapMDG = !string.IsNullOrEmpty(paymentAccountInfo.InternalId) ? paymentAccountInfo.InternalId.Trim() : string.Empty;
          
          var bankGuid1C = !string.IsNullOrEmpty(paymentAccountInfo.GuidBank1C) ? paymentAccountInfo.GuidBank1C.Trim() : string.Empty;
          
          var organization = !string.IsNullOrEmpty(paymentAccountInfo.Organization) ? paymentAccountInfo.Organization.Trim() : string.Empty;
          
          var bankControlKey = !string.IsNullOrEmpty(paymentAccountInfo.BankControlKey) ? paymentAccountInfo.BankControlKey.Trim() : string.Empty;
          
          var currencyCode = !string.IsNullOrEmpty(paymentAccountInfo.Currency) ? paymentAccountInfo.Currency.Trim() : string.Empty;
          
          var errorText = string.Empty;
          
          // Проверить банк.
          var bank = OverrideBaseDev.PublicFunctions.Bank.GetBankFromGuid1C(bankGuid1C);
          if (bank  == null)
          {
            errorText += string.Format("Банк с гуидом: {0} не найден.", bankGuid1C);
            
            Logger.ErrorFormat("{0}Банк с гуидом: {1} не найден.", prefix, bankGuid1C);
          }
          
          // Проверить НОР.
          var businessUnit = OverrideBaseDev.PublicFunctions.BusinessUnit.GetBusinessUnitByHCMCode(organization);
          if (businessUnit  == null)
          {
            errorText += string.Format("НОР с кодом: {0} не найдена.", organization);
            
            Logger.ErrorFormat("{0}НОР с кодом: {1} не найдена.", prefix, organization);
          }
          
          if (businessUnit != null && bank != null)
          {
            // Найти синхронизированный расчетный счет.
            var paymentAccount = SAPIntegration.PublicFunctions.PaymentAccount.GetPaymentAccountSynch(guid1C);
            
            if (paymentAccount.State.IsInserted)
              Logger.DebugFormat("{0}Расчетный счет с параметрами не найден. Гуид 1С: {1}.", prefix, guid1C);
            else
              prefix = string.Format("{0}Обработка расчетного счета с ИД: {1}. ", prefix, paymentAccount.Id);
            
            if (!string.IsNullOrEmpty(currencyCode))
            {
              var currency = vf.OverrideBaseDev.PublicFunctions.Currency.GetCurrencyByAlphaCode(currencyCode);
              
              if (currency != null && !OverrideBaseDev.Currencies.Equals(currency, paymentAccount.Currency))
              {
                Logger.DebugFormat("{0}Найдена валюта: {1}.", prefix, currency.Name);
                
                paymentAccount.Currency = currency;
              }
            }
            
            if (paymentAccount.OwnAccount != true)
              paymentAccount.OwnAccount = true;
            
            if (paymentAccount.ExternalId != guid1C)
              paymentAccount.ExternalId = guid1C;
            
            if (paymentAccount.SAPMDG != sapMDG)
              paymentAccount.SAPMDG = sapMDG;
            
            if (!OverrideBaseDev.Banks.Equals(paymentAccount.Bank, bank))
              paymentAccount.Bank = bank;
            
            if (!paymentAccount.BusinessUnits.Any(x => OverrideBaseDev.BusinessUnits.Equals(x.BusinessUnit, businessUnit)))
            {
              var newRecord = paymentAccount.BusinessUnits.AddNew();
              newRecord.BusinessUnit = businessUnit;
            }
            
            if (paymentAccount.Number != bankControlKey)
              paymentAccount.Number = bankControlKey;

            if (!paymentAccountInfo.Deleted_flag && paymentAccount.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
              paymentAccount.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
            else if (paymentAccountInfo.Deleted_flag && paymentAccount.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
              paymentAccount.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
            
            if (paymentAccount.State.IsChanged && string.IsNullOrEmpty(errorText))
            {
              paymentAccount.Note = "Расчетный счет загружен автоматически из 1С";
              
              paymentAccount.Save();
              
              Logger.DebugFormat("{0}Расчетный счет с параметрами успешно занесен. Гуид 1С: {1}.", prefix, guid1C);
              
              if (!string.IsNullOrEmpty(queueItem.ErrorText))
                queueItem.ErrorText = string.Empty;
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
            }
          }
          else
          {

            if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            
            queueItem.ErrorText = errorText;
          }
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region Договоры
    
    public virtual void SendContractInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.SendContractInfoInvokeArgs args)
    {
      var contractId = args.ContractId;
      
      var prefix = string.Format("SendContractInfo. Обработка договора с ИД: {0}. ", contractId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var contract = OverrideBaseDev.ContractualDocuments.GetAll(x => x.Id == contractId).FirstOrDefault();
      
      if (contract == null)
      {
        Logger.ErrorFormat("{0}Не найден договор", prefix);
        return;
      }
      
      var isSupAgreement = OverrideBaseDev.SupAgreements.Is(contract);
      var isContract = OverrideBaseDev.ContractBases.Is(contract);
      
      var contractInfo = Structures.Module.ContractToKafka.Create();
      
      var keyValue = !string.IsNullOrEmpty(contract.OldSedId) ? contract.OldSedId.Trim() : contract.Id.ToString() + KafkaIntegration.PublicConstants.Module.SystemCodes.DirectumIdPrefix;
      contractInfo.SedId = keyValue;
      
      contractInfo.ProjectNumber = keyValue;
      
      contractInfo.Hyperlink = Hyperlinks.Get(contract);
      contractInfo.RegistrationNumber = !string.IsNullOrEmpty(contract.RegistrationNumber) ? contract.RegistrationNumber.Trim() : null;
      
      contractInfo.RegistrationDate = contract.RegistrationDate.HasValue ? (DateTime?)contract.RegistrationDate.Value.Date : (DateTime?)null;

      contractInfo.Created = contract.Created.HasValue ? contract.Created.Value.Date : (DateTime?)null;
      
      contractInfo.Content = !string.IsNullOrEmpty(contract.Subject) ? contract.Subject.Trim() : null;
      contractInfo.Start_date = contract.ValidFrom.HasValue ? contract.ValidFrom.Value.Date : (DateTime?)null;
      contractInfo.End_date = contract.ValidTill.HasValue ? contract.ValidTill.Value.Date : (DateTime?)null;
      
      contractInfo.IsAutoRenewable = isContract ? OverrideBaseDev.ContractBases.As(contract).IsAutomaticRenewal == true : false;
      contractInfo.ResponsibleEmployee = contract.ResponsibleEmployee != null && !string.IsNullOrEmpty(contract.ResponsibleEmployee.Person.ExternalId) ? contract.ResponsibleEmployee.Person.ExternalId.Trim() : null;
      
      string departmentGuid = null;
      if (contract.Department != null)
      {
        var cfo = OverrideBaseDev.Departments.As(contract.Department).CFO;
        if (cfo != null && !string.IsNullOrEmpty(cfo.ExternalId))
          departmentGuid = cfo.ExternalId.Trim();
      }
      
      contractInfo.Department = departmentGuid;
      
      contractInfo.Counterparty = contract.Counterparty != null && !string.IsNullOrEmpty(contract.Counterparty.ExternalId) ? contract.Counterparty.ExternalId.Trim() : null;
      contractInfo.VatRate = contract.VatRate != null && contract.VatRate.Rate.HasValue ? contract.VatRate.Rate.Value : 0;
      contractInfo.GetSAPState = isContract ? OverrideBaseDev.PublicFunctions.ContractualDocument.GetSAPState(OverrideBaseDev.ContractBases.As(contract)) : OverrideBaseDev.PublicFunctions.ContractualDocument.GetSAPState(OverrideBaseDev.SupAgreements.As(contract));
      contractInfo.GetSAPStatus = isContract ? OverrideBaseDev.PublicFunctions.ContractualDocument.GetSAPStatus(OverrideBaseDev.ContractBases.As(contract)) : OverrideBaseDev.PublicFunctions.ContractualDocument.GetSAPStatus(OverrideBaseDev.SupAgreements.As(contract));
      contractInfo.PaymentTermsName = contract.PaymentTerms != null && !string.IsNullOrEmpty(contract.PaymentTerms.Name) ? contract.PaymentTerms.Name.Trim() : null;
      contractInfo.PaymentTermsCode = contract.PaymentTerms != null && !string.IsNullOrEmpty(contract.PaymentTerms.SAPCode) ? contract.PaymentTerms.SAPCode.Trim() : null;
      contractInfo.ContractType = OverrideBaseDev.ContractualDocuments.Info.Properties.ContractType.GetLocalizedValue(contract.ContractType);
      contractInfo.IncotermsKind = contract.IncotermsKind != null && !string.IsNullOrEmpty(contract.IncotermsKind.Code) ? contract.IncotermsKind.Code.Trim() : null;
      
      contractInfo.Currency = contract.Currency != null && !string.IsNullOrEmpty(contract.Currency.AlphaCode) ? contract.Currency.AlphaCode.Trim() : null;
      
      var paymentCurrencies = new List<string>();
      
      if (contract.PaymentCurrencies.Any())
        paymentCurrencies.AddRange(contract.PaymentCurrencies.Where(x => x.PaymentCurrency != null && !string.IsNullOrEmpty(x.PaymentCurrency.AlphaCode)).Select(x => x.PaymentCurrency.AlphaCode.Trim()));
      
      contractInfo.PaymentCurrencies = paymentCurrencies;
      contractInfo.TotalAmount = contract.TotalAmount.HasValue ? contract.TotalAmount.Value : 0;
      contractInfo.NetAmount = contract.NetAmount.HasValue ? contract.NetAmount.Value : 0;
      contractInfo.VatAmount = contract.VatAmount.HasValue ? contract.VatAmount.Value : 0;
      
      contractInfo.ContractUniqueNumber = !string.IsNullOrEmpty(contract.ContractUniqueNumber) ? contract.ContractUniqueNumber.Trim() : null;
      contractInfo.IsSupAgreement = isSupAgreement;
      
      string mainContractId = null;
      
      if (isSupAgreement)
      {
        var mainContract = OverrideBaseDev.ContractBases.As(contract.LeadingDocument);
        if (mainContract != null)
          mainContractId = !string.IsNullOrEmpty(mainContract.OldSedId) ? mainContract.OldSedId.Trim() : mainContract.Id.ToString() + KafkaIntegration.PublicConstants.Module.SystemCodes.DirectumIdPrefix;
      }
      
      contractInfo.MainContract = mainContractId;
      
      contractInfo.CounterpartyRegistrationNumber = !string.IsNullOrEmpty(contract.CounterpartyRegistrationNumber) ? contract.CounterpartyRegistrationNumber.Trim() : null;
      contractInfo.ContractDate = contract.ContractDate.HasValue ? contract.ContractDate.Value.Date : (DateTime?)null;
      contractInfo.DocumentKind = contract.ContractKind != null && !string.IsNullOrEmpty(contract.ContractKind.Code) ? contract.ContractKind.Code.Trim() : null;
      contractInfo.ErpDatabookName = contract.ErpDatabookName != null && contract.ErpDatabookName.Databook.HasValue ? CustomContracts.ErpDatabookNames.Info.Properties.Databook.GetLocalizedValue(contract.ErpDatabookName.Databook.Value) : null;
      contractInfo.BusinessUnit = contract.BusinessUnit != null && !string.IsNullOrEmpty(OverrideBaseDev.BusinessUnits.As(contract.BusinessUnit).SAPID) ? OverrideBaseDev.BusinessUnits.As(contract.BusinessUnit).SAPID.Trim() : null;
      contractInfo.OwnBank = contract.OwnBank != null && !string.IsNullOrEmpty(contract.OwnBank.ExternalId) ? contract.OwnBank.ExternalId.Trim() : null;
      contractInfo.OwnPaymentAccount = contract.OwnPaymentAccount != null && !string.IsNullOrEmpty(contract.OwnPaymentAccount.ExternalId) ? contract.OwnPaymentAccount.ExternalId.Trim() : null;
      contractInfo.CounterpartyPaymentAccount = contract.CounterpartyPaymentAccount != null && !string.IsNullOrEmpty(contract.CounterpartyPaymentAccount.ExternalId) ? contract.CounterpartyPaymentAccount.ExternalId.Trim() : null;

      var additionaIds = new List<string>();
      
      if (contract.AdditionalCounterparty.Any())
        paymentCurrencies.AddRange(contract.AdditionalCounterparty.Where(x => x.Counterparty != null && !string.IsNullOrEmpty(x.Counterparty.ExternalId)).Select(x => x.Counterparty.ExternalId.Trim()));

      contractInfo.AdditionalCounterParty = additionaIds;
      contractInfo.ValidFromDeadLine = contract.ValidFromEndType != null && !string.IsNullOrEmpty(contract.ValidFromEndType.SAPCode) ? contract.ValidFromEndType.SAPCode.Trim() : null;
      contractInfo.ValidFromAction = !string.IsNullOrEmpty(contract.EventName) ? contract.EventName.Trim() : null;
      contractInfo.Author = contract.Author != null && OverrideBaseDev.Employees.Is(contract.Author) && !string.IsNullOrEmpty(OverrideBaseDev.Employees.As(contract.Author).Person.ExternalId)
        ? OverrideBaseDev.Employees.As(contract.Author).Person.ExternalId
        : null;
      
      contractInfo.Provision = contract.Provision != null && !string.IsNullOrEmpty(contract.Provision.SAPCode) ? contract.Provision.SAPCode.Trim() : null;
      contractInfo.BankChargeType = contract.BankChargeType != null && !string.IsNullOrEmpty(contract.BankChargeType.SAPCode) ? contract.BankChargeType.SAPCode.Trim() : null;
      contractInfo.ConditionBasePayment = !string.IsNullOrEmpty(contract.ConditionBasePayment) ? contract.ConditionBasePayment.Trim() : null;
      contractInfo.TaxAgent = contract.TaxAgent != null && !string.IsNullOrEmpty(contract.TaxAgent.SAPCode) ? contract.TaxAgent.SAPCode.Trim() : null;
      contractInfo.PurchasingType = contract.PurchasingProcedyreType != null && !string.IsNullOrEmpty(contract.PurchasingProcedyreType.SAPCode) ? contract.PurchasingProcedyreType.SAPCode.Trim() : null;
      contractInfo.AdvanceClosedDate = contract.AdvanceClosedDate.HasValue ? contract.AdvanceClosedDate.Value.Date : (DateTime?)null;
      contractInfo.OriginalStock = contract.IsOriginalContractAvailable == true;
      contractInfo.RegNumEqualContractNumber = contract.RegistrationNumber == contract.CounterpartyRegistrationNumber;

      var accessRights = new List<string>();
      var accessRightsDepartments = contract.AccessRights.Current.Where(x => OverrideBaseDev.Departments.Is(x.Recipient)).Select(x => OverrideBaseDev.Departments.As(x.Recipient)).Cast<OverrideBaseDev.IDepartment>();
      if (accessRightsDepartments.Any())
      {
        foreach (var accessRightsDepartment in accessRightsDepartments)
        {
          var cfoAccessRights = accessRightsDepartment.CFO;
          if (cfoAccessRights != null && !string.IsNullOrEmpty(cfoAccessRights.ExternalId))
            accessRights.Add(cfoAccessRights.ExternalId.Trim());
        }
      }

      contractInfo.AccessRights = accessRights;
      contractInfo.Confidential = contract.IsConfidential == true;
      
      // Сериализовать Json.
      var jsonValue = IsolatedFunctions.DeserializeObject.SerializeContractInfo(contractInfo);
      
      if (!string.IsNullOrEmpty(jsonValue))
      {
        var settings = IntegrationSettings.PublicFunctions.ConnectSettings.GetSettingsKafkaConnector();
        var contractRecord = settings.ObjectSettings.Where(x => x.MessageType == IntegrationSettings.ConnectSettingsObjectSettings.MessageType.Outgoing && x.ObjectName == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Contract).FirstOrDefault();
        
        if (contractRecord != null)
        {
          // Создать запись.
          var queueItem = Functions.KafkaQueueItem.CreateKafkaQueueItemForSend(jsonValue, contractRecord.TopicName, keyValue, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Contract);
          
          // Отправить информацию в Кафку.
          Functions.KafkaQueueItem.SendMessageToKafka(queueItem, settings);
        }
        else
          Logger.ErrorFormat("{0}Не найдены настройки для отправки договора", prefix);
      }
      else
      {
        Logger.ErrorFormat("{0}Во время сериализации произошла ошибка.", prefix);
      }
    }
    
    #endregion
    
    #region Персоны
    
    public virtual void UpdatePeopleInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdatePeopleInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdatePeopleInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      // Дессериализовать Json.
      var personInfo = IsolatedFunctions.DeserializeObject.DesirializePersonInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (personInfo != null)
      {
        try
        {
          var firstName = !string.IsNullOrEmpty(personInfo.FirstName) ? personInfo.FirstName.Trim() : string.Empty;
          var gender = !string.IsNullOrEmpty(personInfo.Gender) ? personInfo.Gender.Trim() : string.Empty;
          var guid1C = !string.IsNullOrEmpty(personInfo.Guid1C) ? personInfo.Guid1C.Trim() : string.Empty;
          var HCMCode = !string.IsNullOrEmpty(personInfo.HCMCode) ? personInfo.HCMCode.Trim() : string.Empty;
          var SNILS = !string.IsNullOrEmpty(personInfo.SNILS) ? personInfo.SNILS.Trim() : string.Empty;
          var lastName = !string.IsNullOrEmpty(personInfo.LastName) ? personInfo.LastName.Trim() : string.Empty;
          var middleName = !string.IsNullOrEmpty(personInfo.MiddleName) ? personInfo.MiddleName.Trim() : string.Empty;
          var INN = !string.IsNullOrEmpty(personInfo.INN) ? personInfo.INN.Trim() : string.Empty;
          
          #region Заполнить персону.
          
          if (!string.IsNullOrEmpty(guid1C))
          {
            prefix = string.Format("{0}Обработка персоны с Guid в 1С: {1}. ", prefix, personInfo.Guid1C);
            
            var person = OverrideBaseDev.PublicFunctions.Person.GetPersonSynch(guid1C, INN);
            
            if (person.State.IsInserted)
              Logger.DebugFormat("{0}Персона не найдена. Гуид 1С: {1}.", prefix, guid1C);
            else
              prefix = string.Format("{0}Обработка персоны с ИД: {1}. ", prefix, person.Id);
            
            if (person.ExternalId != guid1C)
              person.ExternalId = guid1C;
            
            if (person.FirstName != firstName)
              person.FirstName = firstName;
            
            if (person.LastName != lastName)
              person.LastName = lastName;

            if (person.MiddleName != middleName)
              person.MiddleName = middleName;

            if (person.HCMCode != HCMCode)
              person.HCMCode = HCMCode;

            if (person.TIN != INN)
              person.TIN = INN;
            
            if (person.INILA != SNILS)
              person.INILA = SNILS;
            
            if (person.DateOfBirth != personInfo.DateOfBirth)
              person.DateOfBirth = personInfo.DateOfBirth;

            // Пол.
            if (!string.IsNullOrEmpty(gender))
            {
              if (gender.ToUpper() == OverrideBaseDev.PublicConstants.Parties.Person.Genders.Male && person.Sex != OverrideBaseDev.Person.Sex.Male)
                person.Sex = OverrideBaseDev.Person.Sex.Male;
              else if (gender.ToUpper() == OverrideBaseDev.PublicConstants.Parties.Person.Genders.Male && person.Sex != OverrideBaseDev.Person.Sex.Male)
                person.Sex = OverrideBaseDev.Person.Sex.Male;
            }
            else if (person.Sex != null)
              person.Sex = null;
            
            if (!personInfo.Deleted_flag && person.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
              person.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
            else if (personInfo.Deleted_flag && person.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
              person.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
            
            if (person.State.IsChanged)
            {
              person.Save();
              Logger.DebugFormat("{0}Персона успешно занесена.", prefix);
            }
            
            if (!string.IsNullOrEmpty(queueItem.ErrorText))
              queueItem.ErrorText = string.Empty;
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
          }
          else
          {
            var errorText = "Не заполнен Guid в 1С";
            
            Logger.ErrorFormat("{0}{1}", prefix, errorText);
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            queueItem.ErrorText = errorText;
          }
          
          #endregion
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region Статьи бюджета
    
    /// <summary>
    /// Обновить информацию о статьях бюджета.
    /// </summary>
    /// <param name="args">Аргументы АО.</param>>
    public virtual void UpdateBudgetItemsInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateBudgetItemsInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateBudgetItemsInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      // Дессериализовать Json.
      var budgetItemInfo = IsolatedFunctions.DeserializeObject.DesirializeBudgetItemsInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (budgetItemInfo != null)
      {
        try
        {
          var guid1C = !string.IsNullOrEmpty(budgetItemInfo.Guid1C) ? budgetItemInfo.Guid1C.Trim() : string.Empty;
          
          var name = !string.IsNullOrEmpty(budgetItemInfo.Name) ? budgetItemInfo.Name.Trim() : string.Empty;
          
          var contractType = !string.IsNullOrEmpty(budgetItemInfo.ContractType) ? budgetItemInfo.ContractType.Trim() : string.Empty;
          
          var articleType = !string.IsNullOrEmpty(budgetItemInfo.ArticleType) ? budgetItemInfo.ArticleType.Trim() : string.Empty;
          
          var firstLevelGuid = !string.IsNullOrEmpty(budgetItemInfo.FirstLevel) && budgetItemInfo.FirstLevel != Constants.Module.SystemCodes.PseudoGuid  ? budgetItemInfo.FirstLevel.Trim() : string.Empty;
          
          var secondLevelGuid = !string.IsNullOrEmpty(budgetItemInfo.SecondLevel) && budgetItemInfo.SecondLevel != Constants.Module.SystemCodes.PseudoGuid ? budgetItemInfo.SecondLevel.Trim() : string.Empty;
          
          var threeLevelGuid = !string.IsNullOrEmpty(budgetItemInfo.ThreeLevel) && budgetItemInfo.ThreeLevel != Constants.Module.SystemCodes.PseudoGuid ? budgetItemInfo.ThreeLevel.Trim() : string.Empty;
          
          var fourLevelGuid = !string.IsNullOrEmpty(budgetItemInfo.FourLevel) && budgetItemInfo.FourLevel != Constants.Module.SystemCodes.PseudoGuid ? budgetItemInfo.FourLevel.Trim() : string.Empty;
          
          var errorText = string.Empty;
          
          // Выполнить поиск статьи бюджета.
          var budgetItem = CustomContracts.PublicFunctions.BKBDR.GetBudgetItemSynch(guid1C);
          
          if (budgetItem.State.IsInserted)
            Logger.DebugFormat("{0}Статья бюджета с параметрами не найдена. Гуид 1С: {1}.", prefix, guid1C);
          else
            prefix = string.Format("{0}Обработка статьи бюджета с ИД: {1}. ", prefix, budgetItem.Id);
          
          Enumeration? contractTypeCode = null;
          
          if (!string.IsNullOrEmpty(contractType))
            contractTypeCode = contractType == Constants.Module.SystemCodes.ContractTypeOutgoing ? CustomContracts.BKBDR.ContractKind.Outcome : CustomContracts.BKBDR.ContractKind.Income;
          else
          {
            errorText += "Не заполнен тип договора.";
            
            Logger.ErrorFormat("{0}{1}.", prefix, errorText);
          }
          
          if (budgetItem != null && contractTypeCode.HasValue)
          {
            
            if (budgetItem.ExternalId != guid1C)
              budgetItem.ExternalId = guid1C;
            
            if (budgetItem.Name != name)
              budgetItem.Name = name;
            
            if (budgetItem.ContractKind != contractTypeCode)
              budgetItem.ContractKind = contractTypeCode;
            
            if (!string.IsNullOrEmpty(firstLevelGuid))
            {
              var firstLevel = CustomContracts.PublicFunctions.BKBDR.GetBudgetItemFrom1CGuid(firstLevelGuid);
              if (CustomContracts.BKBDRs.Equals(firstLevel, budgetItem.Level1Article))
                budgetItem.Level1Article = firstLevel;
            }
            if (!string.IsNullOrEmpty(secondLevelGuid))
            {
              var secondLevel = CustomContracts.PublicFunctions.BKBDR.GetBudgetItemFrom1CGuid(secondLevelGuid);
              if (CustomContracts.BKBDRs.Equals(secondLevel, budgetItem.Level2Article))
                budgetItem.Level2Article = secondLevel;
            }
            if (!string.IsNullOrEmpty(threeLevelGuid))
            {
              var threeLevel = CustomContracts.PublicFunctions.BKBDR.GetBudgetItemFrom1CGuid(threeLevelGuid);
              if (CustomContracts.BKBDRs.Equals(threeLevel, budgetItem.Level3Article))
                budgetItem.Level3Article = threeLevel;
            }
            if (!string.IsNullOrEmpty(fourLevelGuid))
            {
              var fourLevel = CustomContracts.PublicFunctions.BKBDR.GetBudgetItemFrom1CGuid(fourLevelGuid);
              if (CustomContracts.BKBDRs.Equals(fourLevel, budgetItem.Level4Article))
                budgetItem.Level4Article = fourLevel;
            }

            if (!budgetItemInfo.Deleted_flag && budgetItem.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
              budgetItem.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
            else if (budgetItemInfo.Deleted_flag && budgetItem.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
              budgetItem.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
            
            if (budgetItem.State.IsChanged && string.IsNullOrEmpty(errorText))
            {
              budgetItem.Note = "Статья расхода загружена автоматически из 1С";
              
              budgetItem.Save();
              
              Logger.DebugFormat("{0}Статья расхода с параметрами успешно занесена. Гуид 1С: {1}.", prefix, guid1C);
              
              if (!string.IsNullOrEmpty(queueItem.ErrorText))
                queueItem.ErrorText = string.Empty;
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
            }
          }
          else
          {

            if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            
            queueItem.ErrorText = errorText;
          }
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region Рейсы
    
    /// <summary>
    /// Обновить информацию о рейсе.
    /// </summary>
    /// <param name="args">Параметры АО.</param>
    public virtual void UpdateVoyageInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateVoyageInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateVoyageInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      List<string> errors = new List<string>();
      
      // Дессериализовать Json.
      var voyageInfo = IsolatedFunctions.DeserializeObject.DesirializeVoyageInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (voyageInfo != null)
      {
        try
        {
          var guid1C = !string.IsNullOrEmpty(voyageInfo.Guid1C) ? voyageInfo.Guid1C.Trim() : string.Empty;
          var number = !string.IsNullOrEmpty(voyageInfo.Number) ? voyageInfo.Number.Trim() : string.Empty;
          var vessel_MVZ_code = !string.IsNullOrEmpty(voyageInfo.Vessel_MVZ_code) ? voyageInfo.Vessel_MVZ_code.Trim() : string.Empty;
          var businessUnitCode = !string.IsNullOrEmpty(voyageInfo.Organization) ? voyageInfo.Organization.Trim() : string.Empty;
          
          if (!string.IsNullOrEmpty(guid1C))
          {
            if (!string.IsNullOrEmpty(businessUnitCode))
            {
              var businessUnit = OverrideBaseDev.PublicFunctions.BusinessUnit.GetBusinessUnitByHCMCode(businessUnitCode);
              
              if (businessUnit != null)
              {
                Logger.DebugFormat("{0} Обработка объекта с гуидом: {1}.", prefix, guid1C);
                
                var voyage = CustomContracts.PublicFunctions.Voyage.GetSynchVoyage(guid1C, number);

                if (voyage.State.IsInserted)
                  Logger.DebugFormat("{0}Рейс с параметрами не найден. Гуид 1С: {1}.", prefix, guid1C);
                else
                  prefix = string.Format("{0}Обработка рейса с ИД: {1}. ", prefix, voyage.Id);
                
                if (voyage.ExternalId != guid1C)
                  voyage.ExternalId = guid1C;
                
                if (voyage.Number != number)
                  voyage.Number = number;
                
                if (voyage.StartDate != voyageInfo.Start_date)
                  voyage.StartDate = voyageInfo.Start_date;
                
                if (voyage.EndDate != voyageInfo.End_date)
                  voyage.EndDate = voyageInfo.End_date;
                
                if (voyage.BusinessInit != businessUnit)
                  voyage.BusinessInit = businessUnit;
                
                if (!voyageInfo.Deleted_flag && voyage.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
                  voyage.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
                else if (voyageInfo.Deleted_flag && voyage.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
                  voyage.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
                
                if (!string.IsNullOrEmpty(vessel_MVZ_code))
                {
                  var vessel = CustomContracts.MVZs.GetAll(x => x.MVZCode == vessel_MVZ_code).FirstOrDefault();
                  if (vessel != null)
                  {
                    if (!CustomContracts.MVZs.Equals(voyage.Vessel, vessel))
                      voyage.Vessel = vessel;
                  }
                  else
                  {
                    var error = string.Format("Не найдено судно с кодом МВЗ {0}", vessel_MVZ_code);
                    Logger.ErrorFormat("{0}{1}", prefix, error);
                    errors.Add(error);
                  }
                }
                else if (voyage.Vessel != null)
                  voyage.Vessel = null;
                
                #region Обновление списка договоров.
                
                // Переданные Id договоров.
                var newContractIds = voyageInfo.ContractIds.Where(x => x != null && x != string.Empty);
                var oldContracts = voyage.Contracts.Where(x => x.Contract != null);
                
                if (!(newContractIds.Count() == oldContracts.Count()
                      && newContractIds.All(x => oldContracts.Any(c => Functions.Module.IsDirectumId(x) && Functions.Module.RemovePrefixFromId(x) == c.Contract.Id.ToString()
                                                                  || !Functions.Module.IsDirectumId(x) && x == c.Contract.OldSedId))))
                {
                  voyage.Contracts.Clear();
                  
                  foreach (var id in newContractIds)
                  {
                    var contract = OverrideBaseDev.ContractBases.GetAll(x => Functions.Module.IsDirectumId(id) && Functions.Module.RemovePrefixFromId(id) == x.Id.ToString()
                                                                        || !Functions.Module.IsDirectumId(id) && id == x.OldSedId).FirstOrDefault();
                    if (contract != null)
                      voyage.Contracts.AddNew().Contract = contract;
                    else
                    {
                      var error = string.Format("Не найден договор с Id {0}", id);
                      Logger.ErrorFormat("{0}{1}", prefix, error);
                      errors.Add(error);
                    }
                  }
                }
                
                #endregion
                
                if (voyage.State.IsChanged)
                {
                  voyage.Save();
                  
                  Logger.DebugFormat("{0}Рейс с параметрами успешно занесен. Гуид 1С: {1}.", prefix, guid1C);
                  
                  if (errors.Any())
                  {
                    var errorText = string.Join(";" + Environment.NewLine, errors.ToArray());
                    if (queueItem.ErrorText != errorText)
                      queueItem.ErrorText = errorText;
                    
                    if (queueItem.Retries < PublicConstants.Module.MaxRetriesAmount)
                    {
                      queueItem.Retries++;
                      queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.NotProcessed;
                    }
                    else
                      queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
                  }
                  else
                  {
                    if (!string.IsNullOrEmpty(queueItem.ErrorText))
                      queueItem.ErrorText = string.Empty;
                    
                    queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
                  }
                }
              }
              else
              {
                var errorText = string.Format("В DirectumRX не найдена наша организация по коду HCM {0}.", businessUnitCode);
                Logger.ErrorFormat("{0}{1}", prefix, errorText);
                
                queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
                queueItem.ErrorText = errorText;
              }
            }
            else
            {
              var errorText = "Не заполнен код HCM нашей организации";
              Logger.ErrorFormat("{0}{1}", prefix, errorText);
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
              queueItem.ErrorText = errorText;
            }
          }
          else
          {
            Logger.ErrorFormat("{0}Не заполнен Гуид 1С.", prefix);
            
            queueItem.ErrorText = "Не заполнен Гуид 1С.";
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          }
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region Сотрудники
    
    public virtual void UpdateEmployeesInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateEmployeesInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateEmployeesInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      var errors = new List<string>();
      
      // Дессериализовать Json.
      var employeeInfo = IsolatedFunctions.DeserializeObject.DesirializeEmployeeInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (employeeInfo != null)
      {
        try
        {
          // Поиск сотрудника.
          var guid1C = !string.IsNullOrEmpty(employeeInfo.Guid1C) ? employeeInfo.Guid1C.Trim() : string.Empty;
          var personnelNumber = !string.IsNullOrEmpty(employeeInfo.PersonnelNumber) ? employeeInfo.PersonnelNumber.Trim() : string.Empty;
          var personGuid = !string.IsNullOrEmpty(employeeInfo.PersonGuid) ? employeeInfo.PersonGuid.Trim() : string.Empty;
          var departmentGuid = !string.IsNullOrEmpty(employeeInfo.DepartmentGuid) ? employeeInfo.DepartmentGuid.Trim() : string.Empty;
          var jobTitleGuid = !string.IsNullOrEmpty(employeeInfo.JobTitleGuid) ? employeeInfo.JobTitleGuid.Trim() : string.Empty;
          var jobTitleName = !string.IsNullOrEmpty(employeeInfo.JobTitleName) ? employeeInfo.JobTitleName.Trim() : string.Empty;
          var businessUnitCode = !string.IsNullOrEmpty(employeeInfo.Organization) ? employeeInfo.Organization.Trim() : string.Empty;
          
          #region Заполнить сотрудника.
          
          if (!string.IsNullOrEmpty(guid1C))
          {
            if (!string.IsNullOrEmpty(businessUnitCode))
            {
              var businessUnit = OverrideBaseDev.PublicFunctions.BusinessUnit.GetBusinessUnitByHCMCode(businessUnitCode);
              
              if (businessUnit != null)
              {
                prefix = string.Format("{0}Обработка сотрудника с Guid в 1С: {1}. ", prefix, employeeInfo.Guid1C);
                
                var employee = OverrideBaseDev.PublicFunctions.Employee.GetEmployeeSynch(guid1C, personnelNumber);
                
                if (employee.State.IsInserted)
                  Logger.DebugFormat("{0}Сотрудник не найден. Гуид 1С: {1}.", prefix, guid1C);
                else
                  prefix = string.Format("{0}Обработка сотрудника с ИД: {1}. ", prefix, employee.Id);
                
                if (employee.ExternalId != guid1C)
                  employee.ExternalId = guid1C;
                
                if (employee.PersonnelNumber != personnelNumber)
                  employee.PersonnelNumber = personnelNumber;
                
                if (employee.JobTitleGuid != jobTitleGuid)
                  employee.JobTitleGuid = jobTitleGuid;

                // Персона.
                if (!string.IsNullOrEmpty(personGuid))
                {
                  var person = OverrideBaseDev.PublicFunctions.Person.GetPersonByGuid(personGuid);
                  if (person == null)
                  {
                    var error = string.Format("Не найдена персона с Guid в 1С {0}.", personGuid);
                    Logger.ErrorFormat("{0}{1}", prefix, error);
                    errors.Add(error);
                  }
                  else if (!OverrideBaseDev.People.Equals(employee.Person, person))
                    employee.Person = person;
                }
                else if (employee.Person != null)
                  employee.Person = null;
                
                // Подразделение.
                if (!string.IsNullOrEmpty(departmentGuid))
                {
                  var department = OverrideBaseDev.PublicFunctions.Department.GetDepartmentByGuid1C(departmentGuid);
                  if (department == null)
                  {
                    var error = string.Format("Не найдено подразделение с Guid в 1С {0}.", departmentGuid);
                    Logger.ErrorFormat("{0}{1}", prefix, error);
                    errors.Add(error);
                  }
                  else if (!OverrideBaseDev.Departments.Equals(employee.Department, department))
                    employee.Department = department;
                }
                else if (employee.Department != null)
                  employee.Department = null;
                
                // Должность.
                if (!string.IsNullOrEmpty(jobTitleName))
                {
                  var jobTitle = Sungero.Company.JobTitles.GetAll(x => x.Name == jobTitleName).FirstOrDefault();
                  if (jobTitle == null)
                  {
                    jobTitle = Sungero.Company.JobTitles.Create();
                    jobTitle.Name = jobTitleName;
                    jobTitle.Save();
                  }
                  
                  if (!Sungero.Company.JobTitles.Equals(employee.JobTitle, jobTitle))
                    employee.JobTitle = jobTitle;
                }
                else if (employee.JobTitle != null)
                  employee.JobTitle = null;
                
                if (string.IsNullOrEmpty(employee.Email))
                {
                  if (employee.NeedNotifyAssignmentsSummary != false)
                    employee.NeedNotifyAssignmentsSummary = false;
                  
                  if (employee.NeedNotifyExpiredAssignments != false)
                    employee.NeedNotifyExpiredAssignments = false;
                  
                  if (employee.NeedNotifyNewAssignments != false)
                    employee.NeedNotifyNewAssignments = false;
                }
                
                if (!employeeInfo.Fired_flag && employee.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
                  employee.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
                else if (employeeInfo.Fired_flag && employee.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
                  employee.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
                
                if (employee.State.IsChanged)
                {
                  employee.Save();
                  Logger.DebugFormat("{0}Сотрудник успешно занесен.", prefix);
                }
                
                if (!errors.Any())
                {
                  if (!string.IsNullOrEmpty(queueItem.ErrorText))
                    queueItem.ErrorText = string.Empty;
                  
                  queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
                }
                else
                {
                  var errorText = string.Join(";" + Environment.NewLine, errors.ToArray());
                  if (queueItem.ErrorText != errorText)
                    queueItem.ErrorText = errorText;
                  
                  if (queueItem.Retries < PublicConstants.Module.MaxRetriesAmount)
                  {
                    queueItem.Retries++;
                    queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.NotProcessed;
                  }
                  else
                    queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
                }
              }
              else
              {
                var errorText = string.Format("В DirectumRX не найдена наша организация по коду HCM {0}.", businessUnitCode);
                Logger.ErrorFormat("{0}{1}", prefix, errorText);
                
                queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
                queueItem.ErrorText = errorText;
              }
            }
            else
            {
              var errorText = "Не заполнен код HCM нашей организации";
              Logger.ErrorFormat("{0}{1}", prefix, errorText);
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
              queueItem.ErrorText = errorText;
            }
          }
          else
          {
            var errorText = "Не заполнен Guid в 1С";
            Logger.ErrorFormat("{0}{1}", prefix, errorText);
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            queueItem.ErrorText = errorText;
          }
          
          #endregion
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region Расчетные счета
    
    /// <summary>
    /// Обновить информацию о расчетных счетах.
    /// </summary>
    /// <param name="args">Аргументы АО.</param>
    public virtual void UpdatePaymentAccountsInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdatePaymentAccountsInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdatePaymentAccountsInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      // Дессериализовать Json.
      var paymentAccountInfo = IsolatedFunctions.DeserializeObject.DesirializePaymentAccountsInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (paymentAccountInfo != null)
      {
        try
        {
          var guid1C = !string.IsNullOrEmpty(paymentAccountInfo.Guid1C) ? paymentAccountInfo.Guid1C.Trim() : string.Empty;
          
          var sapMDG = !string.IsNullOrEmpty(paymentAccountInfo.InternalId) ? paymentAccountInfo.InternalId.Trim() : string.Empty;
          
          var bankGuid1C = !string.IsNullOrEmpty(paymentAccountInfo.GuidBank1C) ? paymentAccountInfo.GuidBank1C.Trim() : string.Empty;
          
          var counterpartyGuid1C = !string.IsNullOrEmpty(paymentAccountInfo.GuidCounterparty1C) ? paymentAccountInfo.GuidCounterparty1C.Trim() : string.Empty;
          
          var bankControlKey = !string.IsNullOrEmpty(paymentAccountInfo.BankControlKey) ? paymentAccountInfo.BankControlKey.Trim() : string.Empty;
          
          var currencyCode = !string.IsNullOrEmpty(paymentAccountInfo.Currency) ? paymentAccountInfo.Currency.Trim() : string.Empty;
          
          var errorText = string.Empty;
          
          // Проверить банк.
          var bank = OverrideBaseDev.PublicFunctions.Bank.GetBankFromGuid1C(bankGuid1C);
          if (bank  == null)
          {
            errorText += string.Format("Банк с гуидом: {0} не найден.", bankGuid1C);
            
            Logger.ErrorFormat("{0}Банк с гуидом: {1} не найден.", prefix, bankGuid1C);
          }
          
          // Проверить контрагентов.
          var counterparty = OverrideBaseDev.PublicFunctions.Counterparty.GetCounterpartyFromGuid1C(counterpartyGuid1C);
          if (counterparty  == null)
          {
            errorText += string.Format("Контрагент с гуидом: {0} не найден.", counterpartyGuid1C);
            
            Logger.ErrorFormat("{0}Контрагент с гуидом: {1} не найден.", prefix, counterpartyGuid1C);
          }
          
          if (counterparty != null && bank != null)
          {
            // Найти синхронизированный счет.
            var paymentAccount = SAPIntegration.PublicFunctions.PaymentAccount.GetPaymentAccountSynch(guid1C);
            
            if (paymentAccount.State.IsInserted)
              Logger.DebugFormat("{0}Расчетный счет с параметрами не найден. Гуид 1С: {1}.", prefix, guid1C);
            else
              prefix = string.Format("{0}Обработка расчетного счета с ИД: {1}. ", prefix, paymentAccount.Id);
            
            if (!string.IsNullOrEmpty(currencyCode))
            {
              var currency = vf.OverrideBaseDev.PublicFunctions.Currency.GetCurrencyByAlphaCode(currencyCode);
              
              if (currency != null && !OverrideBaseDev.Currencies.Equals(currency, paymentAccount.Currency))
              {
                Logger.DebugFormat("{0}Найдена валюта: {1}.", prefix, currency.Name);
                
                paymentAccount.Currency = currency;
              }
            }
            
            var businessUnit = OverrideBaseDev.PublicFunctions.BusinessUnit.GetBusinessUnitByHCMCode(OverrideBaseDev.PublicConstants.Company.BusinessUnit.VFHCMCode);
            if (businessUnit != null && !paymentAccount.BusinessUnits.Any(x => OverrideBaseDev.BusinessUnits.Equals(x.BusinessUnit, businessUnit)))
            {
              var newRecord = paymentAccount.BusinessUnits.AddNew();
              newRecord.BusinessUnit = businessUnit;
            }
            
            if (paymentAccount.OwnAccount == true)
              paymentAccount.OwnAccount = false;
            
            if (paymentAccount.ExternalId != guid1C)
              paymentAccount.ExternalId = guid1C;
            
            if (paymentAccount.SAPMDG != sapMDG)
              paymentAccount.SAPMDG = sapMDG;
            
            if (!OverrideBaseDev.Banks.Equals(paymentAccount.Bank, bank))
              paymentAccount.Bank = bank;
            
            if (!OverrideBaseDev.Counterparties.Equals(paymentAccount.Counterparty, counterparty))
              paymentAccount.Counterparty = counterparty;
            
            if (paymentAccount.Number != bankControlKey)
              paymentAccount.Number = bankControlKey;

            if (!paymentAccountInfo.Deleted_flag && paymentAccount.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
              paymentAccount.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
            else if (paymentAccountInfo.Deleted_flag && paymentAccount.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
              paymentAccount.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
            
            if (paymentAccount.State.IsChanged && string.IsNullOrEmpty(errorText))
            {
              paymentAccount.Note = "Расчетный счет загружен автоматически из 1С";
              
              paymentAccount.Save();
              
              Logger.DebugFormat("{0}Расчетный счет с параметрами успешно занесен. Гуид 1С: {1}.", prefix, guid1C);
              
              if (!string.IsNullOrEmpty(queueItem.ErrorText))
                queueItem.ErrorText = string.Empty;
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
            }
          }
          else
          {

            if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            
            queueItem.ErrorText = errorText;
          }
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region Контрагенты
    
    /// <summary>
    /// Обновить информацию о контрагенте.
    /// </summary>
    /// <param name="args">Аргументы АО.</param>
    public virtual void UpdateCounterpartiesInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateCounterpartiesInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateCounterpartiesInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      // Дессериализовать Json.
      var counterpartyInfo = IsolatedFunctions.DeserializeObject.DesirializeCounterpartiesInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (counterpartyInfo != null)
      {
        try
        {
          // Поиск контрагента.
          var guid1C = !string.IsNullOrEmpty(counterpartyInfo.Guid1C) ? counterpartyInfo.Guid1C.Trim() : string.Empty;
          
          var sapMDG = !string.IsNullOrEmpty(counterpartyInfo.InternalId) ? counterpartyInfo.InternalId.Trim() : string.Empty;
          
          var common = !string.IsNullOrEmpty(counterpartyInfo.Common) ? counterpartyInfo.Common.Trim() : string.Empty;
          
          var longTextlnam = !string.IsNullOrEmpty(counterpartyInfo.Longtextlnam) ? counterpartyInfo.Longtextlnam.Trim() : string.Empty;
          
          var longText = !string.IsNullOrEmpty(counterpartyInfo.Longtext) ? counterpartyInfo.Longtext.Trim() : string.Empty;
          
          var internationalName = !string.IsNullOrEmpty(counterpartyInfo.InternationalName) ? counterpartyInfo.InternationalName.Trim() : string.Empty;
          
          var tin = !string.IsNullOrEmpty(counterpartyInfo.INN) ? counterpartyInfo.INN.Trim() : string.Empty;
          
          var trrc = !string.IsNullOrEmpty(counterpartyInfo.KPP) ? counterpartyInfo.KPP.Trim() : string.Empty;
          
          var psrn = !string.IsNullOrEmpty(counterpartyInfo.OGRN) ? counterpartyInfo.OGRN.Trim() : string.Empty;
          
          var codeAlpha3 = !string.IsNullOrEmpty(counterpartyInfo.CountryCode) ? counterpartyInfo.CountryCode.Trim() : string.Empty;
          
          var taxIdentificationNumberTypeCode = !string.IsNullOrEmpty(counterpartyInfo.TaxIdentificationNumberTypeCode) ? counterpartyInfo.TaxIdentificationNumberTypeCode.Trim() : string.Empty;
          
          var legalAddressee = !string.IsNullOrEmpty(counterpartyInfo.LegalAddress) ? counterpartyInfo.LegalAddress.Trim() : string.Empty;
          
          var postalAddresee = !string.IsNullOrEmpty(counterpartyInfo.PostalAddress) ? counterpartyInfo.PostalAddress.Trim() : string.Empty;
          
          var phones = !string.IsNullOrEmpty(counterpartyInfo.Phones) ? counterpartyInfo.Phones.Trim() : string.Empty;
          
          var mail = !string.IsNullOrEmpty(counterpartyInfo.Email) ? counterpartyInfo.Email.Trim() : string.Empty;
          
          var isNonResident = codeAlpha3 != OverrideBaseDev.PublicConstants.Commons.Country.RuAlphaCode3;
          
          var errorText = string.Empty;
          
          var debugText = string.Empty;
          
          if (string.IsNullOrEmpty(common))
          {
            errorText = string.Format("{0}. Не заполнен признак контрагента.", prefix);
            
            Logger.Error(errorText);
            
            if (queueItem.ErrorText != errorText)
              queueItem.ErrorText = errorText;
            if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            
            if (queueItem.State.IsChanged)
              queueItem.Save();
            
            return;
          }
          
          var isCompany = common != OverrideBaseDev.PublicConstants.Parties.Person.PersonReferenceName;
          
          Logger.DebugFormat("{0}Обработка объекта с гуидом: {1}.", prefix, guid1C);
          
          if (isCompany)
          {
            #region Заполнить организацию
            
            // Выполнить поиск организаций.
            var company = OverrideBaseDev.PublicFunctions.Company.GetCompanySynch(guid1C, tin, trrc);
            
            if (company.State.IsInserted)
              Logger.DebugFormat("{0}Организация с параметрами не найдена. Гуид 1С: {1}, ИНН: {2}, КПП: {3}.", prefix, guid1C, tin, trrc);
            else
              prefix = string.Format("{0}Обработка организации с ИД: {1}. ", prefix, company.Id);
            
            if (!string.IsNullOrEmpty(codeAlpha3))
            {
              var country = vf.OverrideBaseDev.PublicFunctions.Country.GetCountryByAlphaCode3(codeAlpha3);
              
              if (country != null && !OverrideBaseDev.Countries.Equals(country, company.Country))
              {
                Logger.DebugFormat("{0}Найдена страна: {1}.", prefix, country.Name);
                
                company.Country = country;
                company.Nonresident = isNonResident;
              }
            }
            
            if (company.ExternalId != guid1C)
              company.ExternalId = guid1C;
            
            if (company.SAPID != sapMDG)
              company.SAPID = sapMDG;
            
            if (company.LegalName != longTextlnam)
              company.LegalName = longTextlnam;

            if (company.Name != longText)
              company.Name = longText;
            
            if (company.InternationalName != internationalName)
              company.InternationalName = internationalName;
            
            if (company.TIN != tin)
              company.TIN = tin;
            
            if (company.TRRC != trrc)
              company.TRRC = trrc;
            
            if (company.PSRN != psrn)
              company.PSRN = psrn;
            
            if (company.TaxIdentificationNumberTypeCode != taxIdentificationNumberTypeCode)
              company.TaxIdentificationNumberTypeCode = taxIdentificationNumberTypeCode;
            
            if (company.LegalAddress != legalAddressee)
              company.LegalAddress = legalAddressee;
            
            if (company.PostalAddress != postalAddresee)
              company.PostalAddress = postalAddresee;
            
            if (company.Phones != phones)
              company.Phones = phones;
            
            if (company.Email != mail)
            {
              var mailCut = Sungero.Docflow.PublicFunctions.Module.CutText(mail, company.Info.Properties.Email.Length);
              
              if (mail != mailCut)
                debugText += string.Format("Электронная почта превышает длину строки. {0}", mail);
              
              company.Email = mailCut;
            }
            
            if (!counterpartyInfo.Deleted_flag && company.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
              company.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
            else if (counterpartyInfo.Deleted_flag && company.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
              company.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
            
            // Проверить перед сохранением дубль.
            errorText = Sungero.Parties.PublicFunctions.Counterparty.GetCounterpartyDuplicatesErrorText(company);
            if (!string.IsNullOrEmpty(errorText))
            {
              Logger.ErrorFormat("{0}{1}", prefix, errorText);
              
              if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
                queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
              
              queueItem.ErrorText = errorText;
            }
            
            if (company.State.IsChanged && string.IsNullOrEmpty(errorText))
            {
              company.Note = "Контрагент загружен автоматически из 1С";
              
              company.Save();
              
              Logger.DebugFormat("{0}Организация с параметрами успешно занесена. Гуид 1С: {1}, ИНН: {2}, КПП: {3}.", prefix, guid1C, tin, trrc);
              
              if (!string.IsNullOrEmpty(queueItem.ErrorText))
                queueItem.ErrorText = string.Empty;
              
              if (!string.IsNullOrEmpty(debugText))
                queueItem.ErrorText = debugText;
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
            }
            #endregion
            
          }
          else
          {
            #region Заполнить персоны
            
            // Выполнить поиск организаций.
            var person = OverrideBaseDev.PublicFunctions.Person.GetPersonSynch(guid1C, tin);
            
            if (person.State.IsInserted)
              Logger.DebugFormat("{0}Физ лицо с параметрами не найдено. Гуид 1С: {1}, ИНН: {2}.", prefix, guid1C, tin);
            else
              prefix = string.Format("{0}Обработка физ лица с ИД: {1}. ", prefix, person.Id);
            
            if (!string.IsNullOrEmpty(codeAlpha3))
            {
              var country = vf.OverrideBaseDev.PublicFunctions.Country.GetCountryByAlphaCode3(codeAlpha3);
              
              if (country != null && !OverrideBaseDev.Countries.Equals(country, person.Country))
              {
                Logger.DebugFormat("{0}Найдена страна: {1}.", prefix, country.Name);
                
                person.Country = country;
                person.Nonresident = isNonResident;
              }
            }
            
            if (person.ExternalId != guid1C)
              person.ExternalId = guid1C;
            
            if (person.SAPID != sapMDG)
              person.SAPID = sapMDG;
            
            var lastName = string.Empty;
            var firstName = string.Empty;
            var middleName = string.Empty;
            
            var fullNameRegex = System.Text.RegularExpressions.Regex.Match(longTextlnam, @"^(\S+)(?<!\.)\s*(\S+)(?<!\.)\s*(\S*)(?<!\.)$");
            if (fullNameRegex.Success)
            {
              lastName = fullNameRegex.Groups[1].Value;
              firstName = fullNameRegex.Groups[2].Value;
              
              middleName = fullNameRegex.Groups[3].Value;
              firstName = fullNameRegex.Groups[1].Value;
              lastName = fullNameRegex.Groups[3].Value;
              middleName = string.Empty;
              
              if (lastName == string.Empty)
                lastName = fullNameRegex.Groups[2].Value;
              else
                middleName = fullNameRegex.Groups[2].Value;
            }
            
            if (person.LastName != lastName)
              person.LastName = lastName;
            
            if (person.FirstName != firstName)
              person.FirstName = firstName;
            
            if (person.MiddleName != middleName)
              person.MiddleName = middleName;
            
            if (person.Name != longText)
              person.Name = longText;
            
            if (person.InternationalName != internationalName)
              person.InternationalName = internationalName;
            
            if (person.TIN != tin)
              person.TIN = tin;
            
            if (person.PSRN != psrn)
              person.PSRN = psrn;
            
            if (person.TaxIdentificationNumberTypeCode != taxIdentificationNumberTypeCode)
              person.TaxIdentificationNumberTypeCode = taxIdentificationNumberTypeCode;
            
            if (person.LegalAddress != legalAddressee)
              person.LegalAddress = legalAddressee;
            
            if (person.PostalAddress != postalAddresee)
              person.PostalAddress = postalAddresee;
            
            if (person.Phones != phones)
              person.Phones = phones;
            
            if (person.Email != mail)
            {
              var mailCut = Sungero.Docflow.PublicFunctions.Module.CutText(mail, person.Info.Properties.Email.Length);
              
              if (mail != mailCut)
                debugText += string.Format("Р­Р»РµРєС‚СЂРѕРЅРЅР°СЏ РїРѕС‡С‚Р° РїСЂРµРІС‹С€Р°РµС‚ РґРѕРїСѓСЃС‚РёРјСѓСЋ РґР»РёРЅСѓ СЂРµРєРІРёР·РёС‚Р°. {0}", mail);
              
              person.Email = mailCut;
            }
            
            if (!counterpartyInfo.Deleted_flag && person.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
              person.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
            else if (counterpartyInfo.Deleted_flag && person.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
              person.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
            
            // Проверить перед сохранением дубль.
            errorText = Sungero.Parties.PublicFunctions.Counterparty.GetCounterpartyDuplicatesErrorText(person);
            
            if (!string.IsNullOrEmpty(errorText))
            {
              Logger.ErrorFormat("{0}{1}", prefix, errorText);
              
              if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
                queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
              
              queueItem.ErrorText = errorText;
            }
            
            if (person.State.IsChanged && string.IsNullOrEmpty(errorText))
            {
              person.Note = "Контрагент загружен автоматически из 1С";
              
              person.Save();
              
              Logger.DebugFormat("{0}Физ лицо с параметрами успешно занесено. Гуид 1С: {1}, ИНН: {2}.", prefix, guid1C, tin);
              
              if (!string.IsNullOrEmpty(queueItem.ErrorText))
                queueItem.ErrorText = string.Empty;
              
              if (!string.IsNullOrEmpty(debugText))
                queueItem.ErrorText = debugText;
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
            }
          }
          
          #endregion
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region Банки
    
    /// <summary>
    /// Обновить информацию о банке.
    /// </summary>
    /// <param name="args">Аргументы АО.</param>
    public virtual void UpdateBankInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateBankInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateBankInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      // Дессериализовать Json.
      var bankInfo = IsolatedFunctions.DeserializeObject.DesirializeBankInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (bankInfo != null)
      {
        try
        {
          // Поиск банка.
          var guid1C = !string.IsNullOrEmpty(bankInfo.GUID1C) ? bankInfo.GUID1C.Trim() : string.Empty;
          
          var swift = !string.IsNullOrEmpty(bankInfo.Swift_Code) ? bankInfo.Swift_Code.Trim() : string.Empty;
          
          var bic = !string.IsNullOrEmpty(bankInfo.Bank_key) ? bankInfo.Bank_key.Trim() : string.Empty;
          
          var bankBranch = !string.IsNullOrEmpty(bankInfo.Bank_Branch) ? bankInfo.Bank_Branch.Trim() : string.Empty;
          
          var codeAlpha3 = !string.IsNullOrEmpty(bankInfo.Bank_ctry) ? bankInfo.Bank_ctry.Trim() : string.Empty;
          
          var isNonResident = codeAlpha3 != OverrideBaseDev.PublicConstants.Commons.Country.RuAlphaCode3;
          
          if (!string.IsNullOrEmpty(swift) || !string.IsNullOrEmpty(bic))
          {
            Logger.DebugFormat("{0} Обработка объекта с гуидом: {1}.", prefix, guid1C);
            
            var bank = OverrideBaseDev.PublicFunctions.Bank.GetSynchBank(guid1C, bic, swift, bankBranch, isNonResident);

            if (bank.State.IsInserted)
              Logger.DebugFormat("{0}Банк с параметрами не найден. Гуид 1С: {1}, БИК: {2}, SWIFT: {3}.", prefix, guid1C, bic, swift);
            else
              prefix = string.Format("{0}Обработка банка с ИД: {1}. ", prefix, bank.Id);
            
            if (!string.IsNullOrEmpty(codeAlpha3))
            {
              var country = vf.OverrideBaseDev.PublicFunctions.Country.GetCountryByAlphaCode3(codeAlpha3);
              
              if (country != null && !OverrideBaseDev.Countries.Equals(country, bank.Country))
              {
                Logger.DebugFormat("{0}Найдена страна: {1}.", prefix, country.Name);
                
                bank.Country = country;
                bank.Nonresident = isNonResident;
              }
            }
            
            if (bankInfo.Bank_name != bank.Name)
              bank.Name = bankInfo.Bank_name;
            
            // Добавить проверку, что требуется заполнить БИК, так как в 1С для зарубежных банков заполняется и SWIFT и БИК.
            if (bic != bank.BIC && !isNonResident)
              bank.BIC = bankInfo.Bank_key;
            
            if (bank.SWIFT != swift)
              bank.SWIFT = bankInfo.Swift_Code;
            
            if (bank.ExternalId != guid1C)
              bank.ExternalId = guid1C;
            
            if (bank.LegalName != bankInfo.Bank_name)
              bank.LegalName = bankInfo.Bank_name;
            
            if (bank.PostalAddress != bankInfo.Street)
              bank.PostalAddress = bankInfo.Street;
            
            if (bank.LegalAddress != bankInfo.Street)
              bank.LegalAddress = bankInfo.Street;
            
            // Для нерезидента заполняется "Филиал банка", для российского банка "Корр. счет".
            if (!isNonResident && bank.CorrespondentAccount != bankInfo.Bank_Branch)
              bank.CorrespondentAccount = bankInfo.Bank_Branch;
            else if (isNonResident && bank.BankBranch != bankInfo.Bank_Branch)
              bank.BankBranch = bankInfo.Bank_Branch;
            
            if (!bankInfo.Deleted_flag && bank.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
              bank.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
            else if (bankInfo.Deleted_flag && bank.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
              bank.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
            
            if (bank.State.IsChanged)
            {
              bank.Note = "Банк загружен автоматически из 1С";
              
              Logger.DebugFormat("{0}Значение нерезидент: {1}.", prefix, isNonResident);
              
              bank.State.Properties.BIC.IsRequired = !isNonResident;
              
              // Отключить проверку перед сохранением.
              using (EntityEvents.Disable(bank.Info.Events.BeforeSave, bank.Info.Events.AfterSave))
              {
                bank.Save();
              }
              
              Logger.DebugFormat("{0}Банк с параметрами успешно занесен. Гуид 1С: {1}, БИК: {2}, SWIFT: {3}.", prefix, guid1C, bic, swift);
              
              if (!string.IsNullOrEmpty(queueItem.ErrorText))
                queueItem.ErrorText = string.Empty;
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
            }
          }
          else
          {
            Logger.ErrorFormat("{0}Не заполнены данные Гуид 1С, БИК, SWIFT.", prefix);
            
            queueItem.ErrorText = "Не заполнены данные Гуид 1С, БИК, SWIFT.";
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          }
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }

    #endregion
    
    #region Подразделения
    
    public virtual void UpdateDepartmentsInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateDepartmentsInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateDepartmentsInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      var errors = new List<string>();
      
      // Дессериализовать Json.
      var departmentInfo = IsolatedFunctions.DeserializeObject.DesirializeDepartmentInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (departmentInfo != null)
      {
        try
        {
          var guid1C = !string.IsNullOrEmpty(departmentInfo.Guid1C) ? departmentInfo.Guid1C.Trim() : string.Empty;
          var name = !string.IsNullOrEmpty(departmentInfo.Name) ? departmentInfo.Name.Trim() : string.Empty;
          var headDepartmentGuid = !string.IsNullOrEmpty(departmentInfo.HeadDepartment) ? departmentInfo.HeadDepartment.Trim() : string.Empty;
          var businessUnitCode = !string.IsNullOrEmpty(departmentInfo.Organization) ? departmentInfo.Organization.Trim() : string.Empty;
          var HCMCode = !string.IsNullOrEmpty(departmentInfo.HCMCode) ? departmentInfo.HCMCode.Trim() : string.Empty;
          
          #region Заполнить подразделение.
          
          if (!string.IsNullOrEmpty(guid1C))
          {
            prefix = string.Format("{0}Обработка подразделения с Guid в 1С: {1}. ", prefix, departmentInfo.Guid1C);
            
            if (!string.IsNullOrEmpty(businessUnitCode))
            {
              var businessUnit = OverrideBaseDev.PublicFunctions.BusinessUnit.GetBusinessUnitByHCMCode(businessUnitCode);
              
              if (businessUnit != null)
              {
                var department = OverrideBaseDev.PublicFunctions.Department.GetDepartmentSynch(guid1C, HCMCode);
                
                if (department.State.IsInserted)
                  Logger.DebugFormat("{0}Подразделение не найдено. Гуид 1С: {1}.", prefix, guid1C);
                else
                  prefix = string.Format("{0}Обработка подразделения с ИД: {1}. ", prefix, department.Id);
                
                if (department.ExternalId != guid1C)
                  department.ExternalId = guid1C;
                
                if (department.HCMCode != HCMCode)
                  department.HCMCode = HCMCode;
                
                if (department.Name != name)
                  department.Name = name;
                
                if (department.IsShip != departmentInfo.Ship_flag)
                  department.IsShip = departmentInfo.Ship_flag;
                
                if (department.BusinessUnit != businessUnit)
                  department.BusinessUnit = businessUnit;
                
                // Головное подразделение.
                if (!string.IsNullOrEmpty(headDepartmentGuid))
                {
                  var headDepartment = OverrideBaseDev.PublicFunctions.Department.GetDepartmentByGuid1C(headDepartmentGuid);
                  if (headDepartment == null)
                  {
                    var error = string.Format("Не найдено головное подразделение с Guid в 1С {0}.", headDepartmentGuid);
                    Logger.ErrorFormat("{0}{1}", prefix, error);
                    errors.Add(error);
                  }
                  else if (!OverrideBaseDev.Departments.Equals(department.HeadOffice, headDepartment))
                    department.HeadOffice = headDepartment;
                }
                else if (department.HeadOffice != null)
                  department.HeadOffice = null;
                
                if (!departmentInfo.Deleted_flag && department.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
                  department.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
                else if (departmentInfo.Deleted_flag && department.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
                  department.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
                
                if (department.State.IsChanged)
                {
                  department.Save();
                  Logger.DebugFormat("{0}Подразделение успешно занесено.", prefix);
                }
                
                if (!errors.Any())
                {
                  if (!string.IsNullOrEmpty(queueItem.ErrorText))
                    queueItem.ErrorText = string.Empty;
                  
                  queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
                }
                else
                {
                  var errorText = string.Join(";" + Environment.NewLine, errors.ToArray());
                  if (queueItem.ErrorText != errorText)
                    queueItem.ErrorText = errorText;
                  
                  if (queueItem.Retries < PublicConstants.Module.MaxRetriesAmount)
                  {
                    queueItem.Retries++;
                    queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.NotProcessed;
                  }
                  else
                    queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
                }
              }
              else
              {
                var errorText = string.Format("В DirectumRX не найдена наша организация по коду HCM {0}.", businessUnitCode);
                Logger.ErrorFormat("{0}{1}", prefix, errorText);
                
                queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
                queueItem.ErrorText = errorText;
              }
            }
            else
            {
              var errorText = "Не заполнен код HCM нашей организации";
              Logger.ErrorFormat("{0}{1}", prefix, errorText);
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
              queueItem.ErrorText = errorText;
            }
          }
          else
          {
            var errorText = "Не заполнен Guid в 1С";
            Logger.ErrorFormat("{0}{1}", prefix, errorText);
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            queueItem.ErrorText = errorText;
          }
          
          #endregion
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region ЦФО/МВЗ
    
    public virtual void UpdateCompanyStructureInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateCompanyStructureInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateCompanyStructureInfo. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      // Дессериализовать Json.
      var companyStructureInfo = IsolatedFunctions.DeserializeObject.DesirializeCompanyStructureInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (companyStructureInfo != null)
      {
        try
        {
          var guid1C = !string.IsNullOrEmpty(companyStructureInfo.Guid1C) ? companyStructureInfo.Guid1C.Trim() : string.Empty;
          var name = !string.IsNullOrEmpty(companyStructureInfo.Name) ? companyStructureInfo.Name.Trim() : string.Empty;
          var MVZCode = !string.IsNullOrEmpty(companyStructureInfo.MVZCode) ? companyStructureInfo.MVZCode.Trim() : string.Empty;
          var CFOCode = !string.IsNullOrEmpty(companyStructureInfo.CFOCode) ? companyStructureInfo.CFOCode.Trim() : string.Empty;
          var businessUnitCode = !string.IsNullOrEmpty(companyStructureInfo.Organization) ? companyStructureInfo.Organization.Trim() : string.Empty;
          
          #region Заполнить ЦФО/МВЗ.
          
          if (!string.IsNullOrEmpty(guid1C))
          {
            if (!string.IsNullOrEmpty(businessUnitCode))
            {
              var businessUnit = OverrideBaseDev.PublicFunctions.BusinessUnit.GetBusinessUnitByHCMCode(businessUnitCode);
              
              if (businessUnit != null)
              {
                var companyStructure = CustomContracts.CompanyStructures.Null;
                
                if (companyStructureInfo.CFO_flag)
                {
                  prefix = string.Format("{0}Обработка ЦФО с Guid в 1С: {1}. ", prefix, companyStructureInfo.Guid1C);
                  companyStructure = CustomContracts.PublicFunctions.CFO.GetCFOSynch(guid1C, CFOCode);
                }
                else
                {
                  prefix = string.Format("{0}Обработка МВЗ с Guid в 1С: {1}. ", prefix, companyStructureInfo.Guid1C);
                  companyStructure = CustomContracts.PublicFunctions.MVZ.GetMVZSynch(guid1C, MVZCode);
                }
                
                if (companyStructure.State.IsInserted)
                  Logger.DebugFormat("{0}Запись справочника не найдена. Гуид 1С: {1}.", prefix, guid1C);
                else
                  prefix = string.Format("{0}Обработка записи справочника с ИД: {1}. ", prefix, companyStructure.Id);
                
                if (companyStructure.ExternalId != guid1C)
                  companyStructure.ExternalId = guid1C;
                
                if (companyStructure.Name != name)
                  companyStructure.Name = name;
                
                if (companyStructure.MVZCode != MVZCode)
                  companyStructure.MVZCode = MVZCode;
                
                if (companyStructure.CFOCode != CFOCode)
                  companyStructure.CFOCode = CFOCode;
                
                if (companyStructure.BusinessUnit != businessUnit)
                  companyStructure.BusinessUnit = businessUnit;
                
                if (companyStructure.IsShip != companyStructureInfo.Ship_flag)
                  companyStructure.IsShip = companyStructureInfo.Ship_flag;
                
                if (!companyStructureInfo.Deleted_flag && companyStructure.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
                  companyStructure.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
                else if (companyStructureInfo.Deleted_flag && companyStructure.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
                  companyStructure.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
                
                if (companyStructure.State.IsChanged)
                {
                  companyStructure.Save();
                  Logger.DebugFormat("{0}Запись справочника успешно создана/обновлена.", prefix);
                  

                  if (!string.IsNullOrEmpty(queueItem.ErrorText))
                    queueItem.ErrorText = string.Empty;
                  
                  queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
                }
              }
              else
              {
                var errorText = string.Format("В DirectumRX не найдена наша организация по коду HCM {0}.", businessUnitCode);
                Logger.ErrorFormat("{0}{1}", prefix, errorText);
                
                queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
                queueItem.ErrorText = errorText;
              }
            }
            else
            {
              var errorText = "Не заполнен код HCM нашей организации";
              Logger.ErrorFormat("{0}{1}", prefix, errorText);
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
              queueItem.ErrorText = errorText;
            }
          }
          else
          {
            var errorText = "Не заполнен Guid в 1С";
            Logger.ErrorFormat("{0}{1}", prefix, errorText);
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            queueItem.ErrorText = errorText;
          }
          
          #endregion
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region Регистр ЦФО-Статьи БК
    
    public virtual void UpdateCFOArticleItem(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateCFOArticleItemInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateCFOArticleItem. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      // Дессериализовать Json.
      var CFOArticleItem = IsolatedFunctions.DeserializeObject.DesirializeCFOArticleItem(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (CFOArticleItem != null)
      {
        try
        {
          var CFOGuid = !string.IsNullOrEmpty(CFOArticleItem.CFO) ? CFOArticleItem.CFO.Trim() : string.Empty;
          var newArticleGuids = CFOArticleItem.Article.Where(x => x != null && x != string.Empty).Select(x => x.Trim()).ToList();
          
          #region Заполнить ЦФО.
          
          if (!string.IsNullOrEmpty(CFOGuid))
          {
            if (newArticleGuids.Any())
            {
              Logger.DebugFormat("{0}Обработка ЦФО с Guid {1}", prefix, CFOGuid);
              
              var CFO = CustomContracts.PublicFunctions.CFO.GetCFOByGuid(CFOGuid);
              
              if (CFO != null)
              {
                var oldArticles = CFO.Articles.Where(x => x.Article != null);
                
                if (!(newArticleGuids.Count() == oldArticles.Count()
                      && oldArticles.All(x => newArticleGuids.Contains(x.Article.ExternalId))))
                {
                  CFO.Articles.Clear();
                  foreach (var articleGuid in newArticleGuids)
                  {
                    var article = CustomContracts.PublicFunctions.BKBDR.GetBudgetItemFrom1CGuid(articleGuid);
                    if (article != null)
                      CFO.Articles.AddNew().Article = article;
                    else
                    {
                      var errorText = string.Format("Не найдена статья БК с Guid в 1С {0}; ", articleGuid);
                      Logger.ErrorFormat("{0}{1}", prefix, errorText);
                      
                      queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
                      queueItem.ErrorText += errorText;
                    }
                  }
                }
                
                if (CFO.State.IsChanged)
                {
                  CFO.Save();
                  Logger.DebugFormat("{0}Статьи в карточке ЦФО успешно обновлены.", prefix);
                }
                
                if (!string.IsNullOrEmpty(queueItem.ErrorText))
                  queueItem.ErrorText = string.Empty;
                
                queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
              }
              else
              {
                var errorText = string.Format("Не найден ЦФО с Guid в 1С {0}", CFOGuid);
                Logger.ErrorFormat("{0}{1}", prefix, errorText);
                
                queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
                queueItem.ErrorText = errorText;
              }
            }
            else
            {
              var errorText = "Не заполнены Guid статей";
              Logger.ErrorFormat("{0}{1}", prefix, errorText);
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
              queueItem.ErrorText = errorText;
            }
          }
          else
          {
            var errorText = "Не заполнен Guid в 1С ЦФО";
            Logger.ErrorFormat("{0}{1}", prefix, errorText);
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            queueItem.ErrorText = errorText;
          }
          
          #endregion
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
    
    #region Регистр связи Подразделение-ЦФО-МВЗ-Менеджер
    
    public virtual void UpdateDepartmentRegisterItem(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateDepartmentRegisterItemInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      var prefix = string.Format("UpdateDepartmentRegisterItem. Обработка очереди с ИД: {0}. ", queueId);
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Logger.ErrorFormat("{0}Не найдена запись справочника \"Очереди сообщения\"", prefix);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Logger.ErrorFormat("{0}Не заполнено тело Json запроса", prefix);
        return;
      }
      
      // Дессериализовать Json.
      var departmentRegisterItem = IsolatedFunctions.DeserializeObject.DesirializeDepartmentRegisterItem(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (departmentRegisterItem != null)
      {
        try
        {
          var departmentGuid = !string.IsNullOrEmpty(departmentRegisterItem.Department) ? departmentRegisterItem.Department.Trim() : string.Empty;
          var CFOGuid = !string.IsNullOrEmpty(departmentRegisterItem.CFO) ? departmentRegisterItem.CFO.Trim() : string.Empty;
          var MVZGuid = !string.IsNullOrEmpty(departmentRegisterItem.MVZ) ? departmentRegisterItem.MVZ.Trim() : string.Empty;
          var managerGuid = !string.IsNullOrEmpty(departmentRegisterItem.Manager) ? departmentRegisterItem.Manager.Trim() : string.Empty;

          #region Обновить Подразделение.
          
          if (!string.IsNullOrEmpty(departmentGuid))
          {
            Logger.DebugFormat("{0}Обработка подразделения с Guid {1}", prefix, departmentGuid);
            
            var department = OverrideBaseDev.PublicFunctions.Department.GetDepartmentByGuid1C(departmentGuid);
            
            if (department != null)
            {
              // Руководитель подразделения.
              if (!string.IsNullOrEmpty(managerGuid))
              {
                var manager = OverrideBaseDev.PublicFunctions.Employee.GetEmployeeByGuid(managerGuid);
                if (manager == null)
                  Logger.ErrorFormat("{0}Не найден руководитель подразделения с Guid в 1С {1}", prefix, managerGuid);
                else if (!OverrideBaseDev.Employees.Equals(department.Manager, manager))
                  department.Manager = manager;
              }
              else if (department.Manager != null)
                department.Manager = null;
              
              // ЦФО.
              if (!string.IsNullOrEmpty(CFOGuid))
              {
                var CFO = CustomContracts.PublicFunctions.CFO.GetCFOByGuid(CFOGuid);
                if (CFO == null)
                  Logger.ErrorFormat("{0}Не найден ЦФО с Guid в 1С {1}", prefix, CFOGuid);
                else if (!CustomContracts.CFOs.Equals(department.CFO, CFO))
                  department.CFO = CFO;
              }
              else if (department.CFO != null)
                department.CFO = null;

              // МВЗ.
              if (!string.IsNullOrEmpty(MVZGuid))
              {
                var MVZ = CustomContracts.PublicFunctions.MVZ.GetMVZByGuid(MVZGuid);
                if (MVZ == null)
                  Logger.ErrorFormat("{0}Не найдено МВЗ с Guid в 1С {1}", prefix, MVZGuid);
                else if (!CustomContracts.MVZs.Equals(department.MVZ, MVZ))
                  department.MVZ = MVZ;
              }
              else if (department.MVZ != null)
                department.MVZ = null;
              
              if (department.State.IsChanged)
              {
                department.Save();
                Logger.DebugFormat("{0}Подразделение успешно обновлено.", prefix);
              }
              
              if (!string.IsNullOrEmpty(queueItem.ErrorText))
                queueItem.ErrorText = string.Empty;
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
            }
            else
            {
              var errorText = string.Format("Не найдено подразделение с Guid в 1С {0}", departmentGuid);
              Logger.ErrorFormat("{0}{1}", prefix, errorText);
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
              queueItem.ErrorText = errorText;
            }
          }
          else
          {
            var errorText = "Не заполнен Guid в 1С подразделения";
            Logger.ErrorFormat("{0}{1}", prefix, errorText);
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            queueItem.ErrorText = errorText;
          }
          
          #endregion
          
          #region Обновить МВЗ
          
          if (!string.IsNullOrEmpty(MVZGuid))
          {
            Logger.DebugFormat("{0}Обработка МВЗ с Guid {1}", prefix, MVZGuid);
            
            var MVZ = CustomContracts.PublicFunctions.MVZ.GetMVZByGuid(MVZGuid);
            
            if (MVZ != null)
            {
              // ЦФО.
              if (!string.IsNullOrEmpty(CFOGuid))
              {
                var CFO = CustomContracts.PublicFunctions.CFO.GetCFOByGuid(CFOGuid);
                if (CFO == null)
                  Logger.ErrorFormat("{0}Не найден ЦФО с Guid в 1С {1}", prefix, CFOGuid);
                else if (!CustomContracts.CFOs.Equals(MVZ.CFO, CFO))
                  MVZ.CFO = CFO;
              }
              else if (MVZ.CFO != null)
                MVZ.CFO = null;

              if (MVZ.State.IsChanged)
              {
                MVZ.Save();
                Logger.DebugFormat("{0}МВЗ успешно обновлено.", prefix);
              }
              
              if (!string.IsNullOrEmpty(queueItem.ErrorText))
                queueItem.ErrorText = string.Empty;
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
            }
            else
            {
              var errorText = string.Format("Не найдено МВЗ с Guid в 1С {0}", departmentGuid);
              Logger.ErrorFormat("{0}{1}", prefix, errorText);
              
              queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
              queueItem.ErrorText = errorText;
            }
          }
          else
          {
            var errorText = "Не заполнен Guid в 1С МВЗ";
            Logger.ErrorFormat("{0}{1}", prefix, errorText);
            
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
            queueItem.ErrorText = errorText;
          }
          
          #endregion
        }
        catch (Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}", prefix, ex);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Logger.ErrorFormat("{0}Во время преобразования произошла ошибка.", prefix);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    #endregion
  }
}