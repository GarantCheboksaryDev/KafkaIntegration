using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Counterparty;

namespace vf.OverrideBaseDev.Client
{
  partial class CounterpartyActions
  {
    public virtual void ShowPaymentAccounts(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var paymentAccounts = SAPIntegration.PublicFunctions.PaymentAccount.Remote.GetCounterpartyPaymentAccounts(_obj);
      
      paymentAccounts.Show();
    }

    public virtual bool CanShowPaymentAccounts(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}