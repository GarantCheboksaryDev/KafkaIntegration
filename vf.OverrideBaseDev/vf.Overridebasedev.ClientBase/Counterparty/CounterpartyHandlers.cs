using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Counterparty;

namespace vf.OverrideBaseDev
{
  partial class CounterpartyClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.Counterparty.SetStateEntityProperties(_obj, e.Params);
    }
  }
}