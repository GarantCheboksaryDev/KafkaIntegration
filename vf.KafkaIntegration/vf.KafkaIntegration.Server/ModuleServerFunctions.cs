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
  }
}