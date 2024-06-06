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
        { vf.IntegrationSettings.Resources.Contract, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Contract },
        { vf.IntegrationSettings.Resources.Counterparties, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Counterparty },
        { vf.IntegrationSettings.Resources.Departments, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Department },
        { vf.IntegrationSettings.Resources.Employee, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Employee },
        { vf.IntegrationSettings.Resources.PaymentAccounts, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.PaymentAccount },
        { vf.IntegrationSettings.Resources.Voyages, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Voyage },
        { vf.IntegrationSettings.Resources.BudgetItems, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.BudgetItem },
        { vf.IntegrationSettings.Resources.People, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Person },
        { vf.IntegrationSettings.Resources.CompanyStructure, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.CompanyStruct },
        { vf.IntegrationSettings.Resources.CFOArticleRegister, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.CFOArticle },
        { vf.IntegrationSettings.Resources.DepartmentRegister, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.RegisterDeprt },
        { vf.IntegrationSettings.Resources.OurPaymentAccount, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.OurPaymentAccou }
      };
      
      return objectNames.Where(x => x.Key == key).Select(x => x.Value).FirstOrDefault();
    }
  }
}