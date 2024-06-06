using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.IntegrationSettings.ConnectSettings;

namespace vf.IntegrationSettings.Client
{
  partial class ConnectSettingsActions
  {
    public virtual void TestConnect(Sungero.Domain.Client.ExecuteActionArgs e)
    {      
      var checkConnection = Functions.ConnectSettings.Remote.CheckConnectionToKafka();
      if (checkConnection)
        e.AddInformation(vf.IntegrationSettings.ConnectSettingses.Resources.ConnectionIsSuccessfull);
      else
        e.AddError(vf.IntegrationSettings.ConnectSettingses.Resources.ServerIsUnawailable);
    }

    public virtual bool CanTestConnect(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted && !_obj.State.IsChanged;
    }

  }

}