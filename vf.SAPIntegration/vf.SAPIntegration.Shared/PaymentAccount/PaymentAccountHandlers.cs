using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.SAPIntegration.PaymentAccount;

namespace vf.SAPIntegration
{
  partial class PaymentAccountSharedHandlers
  {
    
    public virtual void CounterpartyChanged(vf.SAPIntegration.Shared.PaymentAccountCounterpartyChangedEventArgs e)
    {
      if (!OverrideBaseDev.Counterparties.Equals(e.NewValue, e.OldValue))
      {
        var isBUCopy = Functions.PaymentAccount.Remote.IsBusinessUnitCopy(_obj.Counterparty);
        e.Params.AddOrUpdate(Constants.PaymentAccount.CounterpartyIsBUCopy, isBUCopy);

        if (isBUCopy)
          _obj.OwnAccount = false;
      }
    }

    public virtual void OwnAccountChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue && e.NewValue == true)
        _obj.Counterparty = null;
    }
  }
}