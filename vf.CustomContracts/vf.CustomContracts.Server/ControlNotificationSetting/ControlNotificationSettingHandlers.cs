using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.ControlNotificationSetting;

namespace vf.CustomContracts
{
  partial class ControlNotificationSettingServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (ControlNotificationSettings.GetAll(x => Sungero.Docflow.DocumentTypes.Equals(x.DocumentType, _obj.DocumentType) && x.Id != _obj.Id).Any())
        e.AddError(vf.CustomContracts.ControlNotificationSettings.Resources.DatabookAlreadyExists);
    }
  }

}