using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.IntegrationSettings.ConnectSettings;

namespace vf.IntegrationSettings
{
  partial class ConnectSettingsClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.ConnectSettings.SetRequiredAndEnabledProperties(_obj);
    }

  }
}