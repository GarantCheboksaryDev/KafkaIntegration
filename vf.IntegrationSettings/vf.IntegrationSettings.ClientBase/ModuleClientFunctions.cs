using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.IntegrationSettings.Client
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Обработать сообщения с ошибками.
    /// </summary>
    public virtual void HandleErrorRecords()
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
      
      var dialog = Dialogs.CreateInputDialog(vf.IntegrationSettings.Resources.ProcessingErrorRecord);
      
      var objectName = dialog.AddSelect(vf.IntegrationSettings.Resources.ObjectName, true, string.Empty).From(objectNames.Select(x => x.Key).ToArray());
      if (dialog.Show() == DialogButtons.Ok)
      {
        if (Functions.Module.Remote.HandleErrorRecordsServer(objectName.Value))
          Dialogs.ShowMessage(vf.IntegrationSettings.Resources.MessagesSendToProcessing);
        else
          Dialogs.ShowMessage(vf.IntegrationSettings.Resources.MessagesSendError, MessageType.Error);
      }
      
    }

  }
}