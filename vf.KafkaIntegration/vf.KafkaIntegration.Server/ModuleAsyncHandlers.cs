using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.KafkaIntegration.Server
{
  public class ModuleAsyncHandlers
  {
    
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
  }
}