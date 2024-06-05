using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomParties.ServiceSecurityCheckResult;

namespace vf.CustomParties
{
  partial class ServiceSecurityCheckResultClientHandlers
  {
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      _obj.State.IsEnabled = string.IsNullOrWhiteSpace(_obj.Sid);
    }
  }
}