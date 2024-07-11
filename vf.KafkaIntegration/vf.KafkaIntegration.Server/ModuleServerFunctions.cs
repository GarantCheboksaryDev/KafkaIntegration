using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.KafkaIntegration.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Создать экземпляр подключения Kafka.
    /// </summary>
    /// <returns>Экземпляр подключения Kafka.</returns>
    public static KafkaNet.Connector CreateKafkaConnector()
    {
      var bootstrapServers = string.Empty;
      var consumerGroupId = string.Empty;
      var password = string.Empty;
      var userName = string.Empty;
      
      var settings = IntegrationSettings.PublicFunctions.ConnectSettings.GetSettingsKafkaConnector();
      
      if (settings != null)
      {
        bootstrapServers = settings.WebServiceAddressee;
        consumerGroupId = settings.ConsumerGroupId;
        password = settings.Password;
        userName = settings.Login;
      }
      
      return new KafkaNet.Connector(bootstrapServers, userName, password, consumerGroupId);
    }
    
    /// <summary>
    /// Создать экземпляр подключения Kafka.
    /// </summary>
    /// <param name="settings">Запись справочника "Настройки подключения".</param>
    /// <returns>Экземпляр подключения Kafka</returns>
    public static KafkaNet.Connector CreateKafkaConnector(IntegrationSettings.IConnectSettings settings)
    {
      var bootstrapServers = string.Empty;
      var consumerGroupId = string.Empty;
      var password = string.Empty;
      var userName = string.Empty;

      if (settings != null)
      {
        bootstrapServers = settings.WebServiceAddressee;
        consumerGroupId = settings.ConsumerGroupId;
        password = settings.Password;
        userName = settings.Login;
      }
      
      return new KafkaNet.Connector(bootstrapServers, userName, password, consumerGroupId);
    }
    
    /// <summary>
    /// Проверить подключение к Kafka сервису.
    /// </summary>
    /// <param name="connector">Экземпляр подключения.</param>
    /// <returns>True - если подключение успешно установлено, иначе false.</returns>
    [Public]
    public static bool CheckConnectToService(string topicName)
    {
      KafkaNet.Connector connector = CreateKafkaConnector();
      
      return connector.CheckConnection(connector, topicName);
    }
    
    /// <summary>
    /// Получить страну по альфа коду 3.
    /// </summary>
    /// <param name="alphaCode3">Альфа код 3.</param>
    /// <returns>Страна.</returns>
    public static Sungero.Commons.ICountry GetCountryByAlphaCode3(string alphaCode3)
    {
      return Sungero.Commons.Countries.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.Code == alphaCode3).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить синхронизированный банк.
    /// </summary>
    /// <param name="externalLink">Внешняя ссылка системы.</param>
    /// <param name="bic">БИК.</param>
    /// <param name="swift">SWIFT.</param>
    /// <param name="nonResident">Нерезидент.</param>
    /// <returns>Найденный банк, иначе новый.</returns>
    public static Sungero.Parties.IBank GetSynchBank(string externalLink, string bic, string swift, bool nonResident)
    {
      var findBank = Sungero.Parties.Banks.GetAll(x => x.ExternalId == externalLink
                                                  || (!nonResident && bic != string.Empty && bic != null && x.BIC == bic
                                                      || (nonResident && swift != string.Empty && swift != null && x.SWIFT == swift
                                                         )
                                                     )
                                                 ).FirstOrDefault();
      
      return findBank != null ? findBank : Sungero.Parties.Banks.Create();
    }
    
    /// <summary>
    ///  Получить синхронизированную организацию.
    /// </summary>
    /// <param name="externalLink">Внешняя ссылка системы.</param>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>Найденная организация, иначе новая.</returns>
    public static Sungero.Parties.ICompany GetCompanySynch(string externalLink, string tin, string trrc)
    {
      var findCompany = Sungero.Parties.Companies.GetAll(x => x.ExternalId == externalLink).FirstOrDefault();
      if (findCompany == null)
        findCompany = Sungero.Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, trrc, string.Empty, null, true)
          .Where(x => Sungero.Parties.Companies.Is(x)).Cast<Sungero.Parties.ICompany>()
          .FirstOrDefault();
      
      return findCompany != null ? findCompany : Sungero.Parties.Companies.Create();
    }
    
    /// <summary>
    /// Получить синхронизированного физического лица.
    /// </summary>
    /// <param name="externalLink">Внешняя ссылка системы.</param>
    /// <param name="tin">ИНН.</param>
    /// <returns>Найденное физическое лицо, иначе новое.</returns>
    public static Sungero.Parties.IPerson GetPersonSynch(string externalLink, string tin)
    {
      var findPerson = GetPersonByGuid(externalLink);
      if (findPerson == null)
        findPerson = Sungero.Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, string.Empty, string.Empty, null, true)
          .Where(x => Sungero.Parties.People.Is(x)).Cast<Sungero.Parties.IPerson>()
          .FirstOrDefault();
      
      return findPerson != null ? findPerson : Sungero.Parties.People.Create();
      
    }
    
    /// <summary>
    /// Получить персону по Guid в 1С.
    /// </summary>
    /// <param name="externalLink">Внешняя ссылка системы.</param>
    /// <returns>Найденная персона.</returns>
    public static Sungero.Parties.IPerson GetPersonByGuid(string externalLink)
    {
      return Sungero.Parties.People.GetAll(x => x.ExternalId != null && x.ExternalId != string.Empty && x.ExternalId == externalLink).FirstOrDefault();
    }
    
    /// <summary>
    /// Записать сообщение в лог-файл.
    /// </summary>
    /// <param name="message">Текст сообщения.</param>
    /// <param name="isError">Признак ошибки.</param>
    public static void WriteToLog(string message, bool isError)
    {
      var prefix = "UpdateCounterpartiesInfo. ";
      
      var formatMessage = string.Format("{0}{1}", prefix, message);
      
      if (!isError)
        Logger.DebugFormat(formatMessage);
      else
        Logger.ErrorFormat(formatMessage);
    }
    
    /// <summary>
    /// Заполнить данные контрагента.
    /// </summary>
    /// <param name="counterpartyInfo">Структура с данными о контрагенте.</param>
    /// <param name="queueItem">Запись справочника "Очереди сообщений".</param>
    public static void SetCounterpartyInfo(KafkaIntegration.Structures.Module.ICounterpartiesFromKafka counterpartyInfo, IKafkaQueueItem queueItem)
    {
      var externalLink = !string.IsNullOrEmpty(counterpartyInfo.ExternalLink) ? counterpartyInfo.ExternalLink.Trim() : string.Empty;
      
      var common = !string.IsNullOrEmpty(counterpartyInfo.Common) ? counterpartyInfo.Common.Trim() : string.Empty;
      
      var longTextlnam = !string.IsNullOrEmpty(counterpartyInfo.Longtextlnam) ? counterpartyInfo.Longtextlnam.Trim() : string.Empty;
      
      var longText = !string.IsNullOrEmpty(counterpartyInfo.Longtext) ? counterpartyInfo.Longtext.Trim() : string.Empty;
      
      var tin = !string.IsNullOrEmpty(counterpartyInfo.INN) ? counterpartyInfo.INN.Trim() : string.Empty;
      
      var trrc = !string.IsNullOrEmpty(counterpartyInfo.KPP) ? counterpartyInfo.KPP.Trim() : string.Empty;
      
      var psrn = !string.IsNullOrEmpty(counterpartyInfo.OGRN) ? counterpartyInfo.OGRN.Trim() : string.Empty;
      
      var codeAlpha3 = !string.IsNullOrEmpty(counterpartyInfo.CountryCode) ? counterpartyInfo.CountryCode.Trim() : string.Empty;
      
      var legalAddressee = !string.IsNullOrEmpty(counterpartyInfo.LegalAddress) ? counterpartyInfo.LegalAddress.Trim() : string.Empty;
      
      var postalAddresee = !string.IsNullOrEmpty(counterpartyInfo.PostalAddress) ? counterpartyInfo.PostalAddress.Trim() : string.Empty;
      
      var phones = !string.IsNullOrEmpty(counterpartyInfo.Phones) ? counterpartyInfo.Phones.Trim() : string.Empty;
      
      var mail = !string.IsNullOrEmpty(counterpartyInfo.Email) ? counterpartyInfo.Email.Trim() : string.Empty;
      
      var isNonResident = codeAlpha3 != Constants.Module.SystemCodes.RuAlphaCode3;
      
      var errorText = string.Empty;
      
      var debugText = string.Empty;
      
      if (string.IsNullOrEmpty(common))
      {
        errorText = vf.KafkaIntegration.Resources.CounterpartiesMarkIsNull;
        
        Functions.Module.WriteToLog(errorText, true);
        
        if (queueItem.ErrorText != errorText)
          queueItem.ErrorText = errorText;
        if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
          queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
        
        if (queueItem.State.IsChanged)
          queueItem.Save();
        
        return;
      }
      
      var isCompany = common != Constants.Module.SystemCodes.PersonReferenceName;
      
      Functions.Module.WriteToLog(string.Format("Обработка объекта с гуидом: {0}.", externalLink), false);
      
      if (isCompany)
      {
        #region Заполнить организацию
        
        // Выполнить поиск организаций.
        var company = Functions.Module.GetCompanySynch(externalLink, tin, trrc);
        
        if (company.State.IsInserted)
          Functions.Module.WriteToLog(string.Format("Организация с параметрами не найдена. ИД: {0}, ИНН: {1}, КПП: {2}.", externalLink, tin, trrc), false);
        
        if (company.Nonresident != isNonResident)
          company.Nonresident = isNonResident;
        if (company.ExternalId != externalLink)
          company.ExternalId = externalLink;
        
        if (company.LegalName != longTextlnam)
          company.LegalName = longTextlnam;

        if (company.Name != longText)
          company.Name = longText;
        
        if (company.TIN != tin)
          company.TIN = tin;
        
        if (company.TRRC != trrc)
          company.TRRC = trrc;
        
        if (company.PSRN != psrn)
          company.PSRN = psrn;
        
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
            debugText += vf.KafkaIntegration.Resources.EmailIsLengthOutWeigthFormat(mail);
          
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
          Functions.Module.WriteToLog(errorText, true);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = errorText;
        }
        
        if (company.State.IsChanged && string.IsNullOrEmpty(errorText))
        {
          company.Note = vf.KafkaIntegration.Resources.CounterpartiesAutoUploadFrom1C;
          
          company.Save();
          
          Functions.Module.WriteToLog(string.Format("Организация с параметрами успешно занесена. ИД: {0}, ИНН: {1}, КПП: {2}.", externalLink, tin, trrc), false);
          
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
        
        // Выполнить поиск физ лица.
        var person = Functions.Module.GetPersonSynch(externalLink, tin);
        
        if (person.State.IsInserted)
          Functions.Module.WriteToLog(string.Format("Физ лицо с параметрами не найдено. ИД: {0}, ИНН: {1}.", externalLink, tin), false);
        
        if (person.Nonresident != isNonResident)
          person.Nonresident = isNonResident;
        
        if (person.ExternalId != externalLink)
          person.ExternalId = externalLink;
        
        var lastName = string.Empty;
        var firstName = string.Empty;
        var middleName = string.Empty;
        
        var fullNameRegex = System.Text.RegularExpressions.Regex.Match(longTextlnam, Constants.Module.FIOPattern);
        if (fullNameRegex.Success)
        {
          lastName = fullNameRegex.Groups[1].Value;
          firstName = fullNameRegex.Groups[2].Value;
          
          middleName = fullNameRegex.Groups[3].Value;
          
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
        
        if (person.TIN != tin)
          person.TIN = tin;
        
        if (person.PSRN != psrn)
          person.PSRN = psrn;
        
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
            debugText += vf.KafkaIntegration.Resources.EmailIsLengthOutWeigthFormat(mail);
          
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
          Functions.Module.WriteToLog(errorText, true);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = errorText;
        }
        
        if (person.State.IsChanged && string.IsNullOrEmpty(errorText))
        {
          person.Note = vf.KafkaIntegration.Resources.CounterpartiesAutoUploadFrom1C;
          
          person.Save();
          
          Functions.Module.WriteToLog(string.Format("Физ лицо с параметрами успешно занесено. ИД: {0}, ИНН: {1}.", externalLink, tin), false);
          
          if (!string.IsNullOrEmpty(queueItem.ErrorText))
            queueItem.ErrorText = string.Empty;
          
          if (!string.IsNullOrEmpty(debugText))
            queueItem.ErrorText = debugText;
          
          queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
        }
      }
      
      #endregion
    }
    
    /// <summary>
    /// Заполнить данные банка.
    /// </summary>
    /// <param name="counterpartyInfo">Структура с данными о банке.</param>
    /// <param name="queueItem">Запись справочника "Очереди сообщений".</param>
    public static void SetBankInfo(KafkaIntegration.Structures.Module.IBanksFromKafka bankInfo, IKafkaQueueItem queueItem)
    {
      // Поиск банка.
      var externalLink = !string.IsNullOrEmpty(bankInfo.ExternalLink) ? bankInfo.ExternalLink.Trim() : string.Empty;
      
      var swift = !string.IsNullOrEmpty(bankInfo.Swift_Code) ? bankInfo.Swift_Code.Trim() : string.Empty;
      
      var bic = !string.IsNullOrEmpty(bankInfo.Bank_key) ? bankInfo.Bank_key.Trim() : string.Empty;
      
      var bankBranch = !string.IsNullOrEmpty(bankInfo.Bank_Branch) ? bankInfo.Bank_Branch.Trim() : string.Empty;
      
      var codeAlpha3 = !string.IsNullOrEmpty(bankInfo.Bank_ctry) ? bankInfo.Bank_ctry.Trim() : string.Empty;
      
      var isNonResident = codeAlpha3 != Constants.Module.SystemCodes.RuAlphaCode3;
      
      if (!string.IsNullOrEmpty(swift) || !string.IsNullOrEmpty(bic))
      {
        Functions.Module.WriteToLog(string.Format("Обработка объекта с гуидом: {0}.", externalLink), false);
        
        var bank = Functions.Module.GetSynchBank(externalLink, bic, swift, isNonResident);

        if (bank.State.IsInserted)
          Functions.Module.WriteToLog(string.Format("Банк с параметрами не найден. ИД: {0}, БИК: {1}, SWIFT: {2}.", externalLink, bic, swift), false);
        
        if (bank.Nonresident != isNonResident)
          bank.Nonresident = isNonResident;
        if (bankInfo.Bank_name != bank.Name)
          bank.Name = bankInfo.Bank_name;
        
        // Добавить проверку, что требуется заполнить БИК, так как в 1С для зарубежных банков заполняется и SWIFT и БИК.
        if (bic != bank.BIC && !isNonResident)
          bank.BIC = bankInfo.Bank_key;
        
        if (bank.SWIFT != swift)
          bank.SWIFT = bankInfo.Swift_Code;
        
        if (bank.ExternalId != externalLink)
          bank.ExternalId = externalLink;
        
        if (bank.LegalName != bankInfo.Bank_name)
          bank.LegalName = bankInfo.Bank_name;
        
        if (bank.PostalAddress != bankInfo.Street)
          bank.PostalAddress = bankInfo.Street;
        
        if (bank.LegalAddress != bankInfo.Street)
          bank.LegalAddress = bankInfo.Street;
        
        // Для нерезидента заполняется "Филиал банка", для российского банка "Корр. счет".
        if (!isNonResident && bank.CorrespondentAccount != bankInfo.Bank_Branch)
          bank.CorrespondentAccount = bankInfo.Bank_Branch;
        
        if (!bankInfo.Deleted_flag && bank.Status != Sungero.CoreEntities.DatabookEntry.Status.Active)
          bank.Status = Sungero.CoreEntities.DatabookEntry.Status.Active;
        else if (bankInfo.Deleted_flag && bank.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed)
          bank.Status = Sungero.CoreEntities.DatabookEntry.Status.Closed;
        
        if (bank.State.IsChanged)
        {
          bank.Note = vf.KafkaIntegration.Resources.BankAutoUploadFrom1C;
          
          bank.State.Properties.BIC.IsRequired = !isNonResident;
          
          // Отключить проверку перед сохранением.
          using (EntityEvents.Disable(bank.Info.Events.BeforeSave, bank.Info.Events.AfterSave))
          {
            bank.Save();
          }
          
          Functions.Module.WriteToLog(string.Format("Банк с параметрами успешно занесен. ИД: {0}, БИК: {1}, SWIFT: {2}.", externalLink, bic, swift), false);
          
          if (!string.IsNullOrEmpty(queueItem.ErrorText))
            queueItem.ErrorText = string.Empty;
          
          queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
        }
      }
      else
      {
        var error = vf.KafkaIntegration.Resources.NotSetMainPropertiesBank;
        
        Functions.Module.WriteToLog(error, true);
        
        queueItem.ErrorText = error;
        
        queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
      }
    }
  }
}