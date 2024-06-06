using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomParties.CounterpartyCheckAssignment;

namespace vf.CustomParties
{
  partial class CounterpartyCheckAssignmentClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      _obj.State.Properties.CheckingResultSB.IsRequired = true;
      
      var isNonResident = _obj.Counterparty?.Nonresident == true;
      
      _obj.State.Properties.CheckingResultSPARK.IsEnabled = !isNonResident;
      _obj.State.Properties.CheckingResultSPARK.IsRequired = !isNonResident;
    }

  }
}