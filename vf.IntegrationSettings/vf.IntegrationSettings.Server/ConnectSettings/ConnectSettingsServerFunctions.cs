using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.IntegrationSettings.ConnectSettings;
using vf.Integration;

namespace vf.IntegrationSettings.Server
{
  partial class ConnectSettingsFunctions
  {
    /// <summary>
    /// Получить настройки подключения к Kafka.
    /// </summary>
    /// <returns>Настройки подключения к Kafka.</returns>
    [Public]
    public static IntegrationSettings.IConnectSettings GetSettingsKafkaConnector()
    {
      return IntegrationSettings.ConnectSettingses.GetAll(x => x.SystemName == IntegrationSettings.ConnectSettings.SystemName.Kafka).FirstOrDefault();
    }
    
    /// <summary>
    /// Проверить подключение к Kafka.
    /// </summary>
    /// <returns>True - если подключение успешно установлено, иначе false.</returns>
    [Remote(IsPure=true)]
    public static bool CheckConnectionToKafka()
    {
      return KafkaIntegration.PublicFunctions.Module.CheckConnectToService(Constants.ConnectSettings.CheckConnectionTopicName);
    }
    
  }
}