using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomParties.CounterpartyCheckRequest;

namespace vf.CustomParties
{
  partial class CounterpartyCheckRequestSharedHandlers
  {

    public virtual void CounterpartyChanged(vf.CustomParties.Shared.CounterpartyCheckRequestCounterpartyChangedEventArgs e)
    {
      if (!OverrideBaseDev.Counterparties.Equals(e.NewValue, e.OldValue))
      {
        _obj.CounterpartyGroup.Counterparties.Clear();
        if (e.NewValue != null)
          _obj.CounterpartyGroup.Counterparties.Add(e.NewValue);
        
        _obj.TIN = e.NewValue?.TIN;
        _obj.IsInBlackList = e.NewValue?.SpecialList == OverrideBaseDev.Counterparty.SpecialList.Black;
        
        if (!vf.CustomParties.ServiceSecurityCheckResults.Equals(e.NewValue?.ServiceSecurityCheckResult, _obj.CheckingResultSB))
          _obj.CheckingResultSB = e.NewValue.ServiceSecurityCheckResult;
        
        Functions.CounterpartyCheckRequest.FillSubject(_obj);
      }
    }

    public virtual void CheckingDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue != e.OldValue)
      {
        var checkValidTill = e.NewValue.Value.AddYears(Constants.CounterpartyCheckRequest.CounterpartyCheckingAvailableYears);
        if (!Calendar.IsWorkingDay(checkValidTill))
          checkValidTill = Calendar.NextWorkingDay(checkValidTill);
        
        _obj.CheckValidTill = checkValidTill;
        _obj.NextCheckDate = Calendar.NextWorkingDay(checkValidTill);
      }
    }

  }
}