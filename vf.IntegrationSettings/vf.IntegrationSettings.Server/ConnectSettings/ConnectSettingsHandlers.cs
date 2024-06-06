using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.IntegrationSettings.ConnectSettings;

namespace vf.IntegrationSettings
{
  partial class ConnectSettingsServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (_obj.SystemName.Value != _obj.State.Properties.SystemName.OriginalValue)
        _obj.Name = _obj.Info.Properties.SystemName.GetLocalizedValue(_obj.SystemName.Value);
    }
  }

}