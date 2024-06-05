using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.CompanyBase;

namespace vf.OverrideBaseDev
{
  partial class CompanyBaseClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.Counterparty.SetStateEntityProperties(_obj, e.Params);
    }

  }
}