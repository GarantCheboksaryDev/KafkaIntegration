using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.ControlNotificationSetting;

namespace vf.CustomContracts
{
  partial class ControlNotificationSettingDocumentPropertiesClientHandlers
  {

    public virtual void DocumentPropertiesPropertyNameValueInput(Sungero.Presentation.EnumerationValueInputEventArgs e)
    {
      if (ControlNotificationSettings.As(_obj.RootEntity).DocumentProperties.Any(x => x.PropertyName == _obj.PropertyName && x.Id != _obj.Id))
        e.AddError(vf.CustomContracts.ControlNotificationSettings.Resources.RowAlreadyAdded);
    }
  }

  partial class ControlNotificationSettingClientHandlers
  {

  }
}