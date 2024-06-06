using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.PersonalSetting;

namespace vf.OverrideBaseDev
{
  partial class PersonalSettingServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      _obj.OverdueApprovalAssignmentsNotification = false;
    }
  }

}