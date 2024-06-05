using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.IntegrationSettings.ConnectSettings;

namespace vf.IntegrationSettings
{
  partial class ConnectSettingsSharedHandlers
  {

    public virtual void SystemNameChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        Functions.ConnectSettings.SetRequiredAndEnabledProperties(_obj);
    }

  }
}