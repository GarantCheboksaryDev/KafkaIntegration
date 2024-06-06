using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.IntegrationSettings.ConnectSettings;

namespace vf.IntegrationSettings.Shared
{
  partial class ConnectSettingsFunctions
  {
    /// <summary>
    /// Установить видимость и обязательность свойств.
    /// </summary>
    public void SetRequiredAndEnabledProperties()
    {
      var isKafka = _obj.SystemName == IntegrationSettings.ConnectSettings.SystemName.Kafka;
      
      _obj.State.Properties.ConsumerGroupId.IsVisible = isKafka;
      _obj.State.Properties.ConsumerGroupId.IsRequired = isKafka;
      
      _obj.State.Properties.ObjectSettings.IsVisible = isKafka;
    }
    
    /// <summary>
    /// Получить наименование объекта по ключу.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <returns>Объект.</returns>
    public static Enumeration? GetKeyValueFromObjectNames(string key)
    {
      var objectNames = new Dictionary<string, Enumeration>
      {
        { vf.IntegrationSettings.Resources.Bank, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Bank },
        { vf.IntegrationSettings.Resources.Counterparties, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Counterparty }
      };
      
      return objectNames.Where(x => x.Key == key).Select(x => x.Value).FirstOrDefault();
    }
  }
}