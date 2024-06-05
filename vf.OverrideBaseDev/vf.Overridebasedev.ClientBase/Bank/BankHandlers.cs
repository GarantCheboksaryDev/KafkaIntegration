using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Bank;

namespace vf.OverrideBaseDev
{
  partial class BankClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var nonResident = _obj.Nonresident == true;
      
      _obj.State.Properties.BankBranch.IsVisible = nonResident;
      _obj.State.Properties.CorrespondentAccount.IsVisible = !nonResident;
    }
  }


}