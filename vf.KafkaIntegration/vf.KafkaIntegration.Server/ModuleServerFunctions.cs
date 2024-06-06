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
    public static KaffkaNet.Connector CreateKaffkaConnector()
    {
      var bootstrapServers = string.Empty;
      var consumerGroupId = string.Empty;
      var logPath = string.Empty;
      var password = string.Empty;
      var userName = string.Empty;
      
      var settings = IntegrationSettings.PublicFunctions.ConnectSettings.GetSettingsKafkaConnector();
      
      if (settings != null)
      {
        bootstrapServers = settings.WebServiceAddressee;
        consumerGroupId = settings.ConsumerGroupId;
        logPath = settings.LogFilePath;
        password = settings.Password;
        userName = settings.Login;
      }
      
      return new KaffkaNet.Connector(bootstrapServers, userName, password, consumerGroupId, logPath);
    }
    
    /// <summary>
    /// Создать экземпляр подключения Kafka.
    /// </summary>
    /// <param name="settings">Запись справочника "Настройки подключения".</param>
    /// <returns>Экземпляр подключения Kafka</returns>
    public static KaffkaNet.Connector CreateKaffkaConnector(IntegrationSettings.IConnectSettings settings)
    {
      var bootstrapServers = string.Empty;
      var consumerGroupId = string.Empty;
      var logPath = string.Empty;
      var password = string.Empty;
      var userName = string.Empty;

      if (settings != null)
      {
        bootstrapServers = settings.WebServiceAddressee;
        consumerGroupId = settings.ConsumerGroupId;
        logPath = settings.LogFilePath;
        password = settings.Password;
        userName = settings.Login;
      }
      
      return new KaffkaNet.Connector(bootstrapServers, userName, password, consumerGroupId, logPath);
    }
    
    /// <summary>
    /// Проверить подключение к Kafka сервису.
    /// </summary>
    /// <param name="connector">Экземпляр подключения.</param>
    /// <returns>True - если подключение успешно установлено, иначе false.</returns>
    [Public]
    public static bool CheckConnectToService(string topicName)
    {
      KaffkaNet.Connector connector = CreateKaffkaConnector();
      
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
    /// <param name="guid1C">Guid 1C.</param>
    /// <param name="bic">БИК.</param>
    /// <param name="swift">SWIFT.</param>
    /// <param name="nonResident">Нерезидент.</param>
    /// <returns>Найденный банк, иначе новый.</returns>
    public static Sungero.Parties.IBank GetSynchBank(string guid1C, string bic, string swift, bool nonResident)
    {
      var findBank = Sungero.Parties.Banks.GetAll(x => x.ExternalId == guid1C
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
    /// <param name="guid1C">Guid 1C.</param>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>Найденная организация, иначе новая.</returns>
    public static Sungero.Parties.ICompany GetCompanySynch(string guid1C, string tin, string trrc)
    {
      var findCompany = Sungero.Parties.Companies.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
      if (findCompany == null)
        findCompany = Sungero.Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, trrc, string.Empty, null, true)
          .Where(x => Sungero.Parties.Companies.Is(x)).Cast<Sungero.Parties.ICompany>()
          .FirstOrDefault();
      
      return findCompany != null ? findCompany : Sungero.Parties.Companies.Create();
    }
    
    /// <summary>
    /// Получить синхронизированного физического лица.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <param name="tin">ИНН.</param>
    /// <returns>Найденное физическое лицо, иначе новое.</returns>
    [Public]
    public static Sungero.Parties.IPerson GetPersonSynch(string guid1C, string tin)
    {
      var findPerson = GetPersonByGuid(guid1C);
      if (findPerson == null)
        findPerson = Sungero.Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, string.Empty, string.Empty, null, true)
          .Where(x => Sungero.Parties.People.Is(x)).Cast<Sungero.Parties.IPerson>()
          .FirstOrDefault();
      
      return findPerson != null ? findPerson : Sungero.Parties.People.Create();
      
    }
    
    /// <summary>
    /// Получить персону по Guid в 1С.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <returns>Найденная персона.</returns>
    [Public]
    public static Sungero.Parties.IPerson GetPersonByGuid(string guid1C)
    {
      return Sungero.Parties.People.GetAll(x => x.ExternalId != null && x.ExternalId != string.Empty && x.ExternalId == guid1C).FirstOrDefault();
    }
  }
}