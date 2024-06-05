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
        { vf.IntegrationSettings.Resources.Counterparties, IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Counterparty }
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