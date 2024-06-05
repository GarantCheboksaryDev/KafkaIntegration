using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.BusinessUnit;

namespace vf.OverrideBaseDev
{
  partial class BusinessUnitClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var isAdmin = IntegrationSettings.PublicFunctions.Module.CheckCurrentUserIsAdmin();
      
      _obj.State.Properties.SAPID.IsVisible = isAdmin;
      _obj.State.Properties.SAPID.IsEnabled = isAdmin;            
    }

  }
}