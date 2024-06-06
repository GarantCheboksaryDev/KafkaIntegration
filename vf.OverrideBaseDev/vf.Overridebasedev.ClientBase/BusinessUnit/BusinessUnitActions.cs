using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.BusinessUnit;

namespace vf.OverrideBaseDev.Client
{
  partial class BusinessUnitActions
  {

    public virtual void ShowPaymentAccounts(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var paymentAccounts = SAPIntegration.PublicFunctions.PaymentAccount.Remote.GetBusinessUnitPaymentAccounts(_obj, null, true);
      
      paymentAccounts.Show();
    }

    public virtual bool CanShowPaymentAccounts(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}