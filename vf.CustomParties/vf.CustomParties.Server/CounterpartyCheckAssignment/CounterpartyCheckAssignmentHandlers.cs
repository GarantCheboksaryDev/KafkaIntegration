using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomParties.CounterpartyCheckAssignment;

namespace vf.CustomParties
{
  partial class CounterpartyCheckAssignmentServerHandlers
  {

    public override void BeforeComplete(Sungero.Workflow.Server.BeforeCompleteEventArgs e)
    {
      if (!_obj.Texts.Any() || _obj.Texts.FirstOrDefault()?.Body == vf.CustomParties.CounterpartyCheckAssignments.Resources.CpmpleteAtciveText)
        e.AddError(vf.CustomParties.CounterpartyCheckAssignments.Resources.ActiveTextNotFilledError);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsInBlackList = false;
    }
  }

}