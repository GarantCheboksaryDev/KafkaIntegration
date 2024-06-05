using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.SAPIntegration.PaymentAccount;

namespace vf.SAPIntegration
{
  partial class PaymentAccountClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.PaymentAccount.SetRequiredProperties(_obj, e.Params);
    }

  }
}