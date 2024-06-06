using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Bank;

namespace vf.OverrideBaseDev.Client
{
  partial class BankActions
  {
    public override void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var duplicates = new List<OverrideBaseDev.IBank>();
      if (!string.IsNullOrWhiteSpace(_obj.BIC))
        duplicates.AddRange(Sungero.Parties.PublicFunctions.Bank.Remote.GetBanksWithSameBic(_obj, true).Cast<OverrideBaseDev.IBank>());
      if (!string.IsNullOrWhiteSpace(_obj.SWIFT))
        duplicates.AddRange(Functions.Bank.Remote.GetBanksWithSameSwiftAnBankBranch(_obj, true));
      if (duplicates.Any())
      {
        duplicates.Show();
        return;
      }
    }

    public override bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowDuplicates(e);
    }

    public override void ShowPaymentAccounts(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var paymentAccounts = SAPIntegration.PublicFunctions.PaymentAccount.Remote.GetBankPaymentAccounts(_obj);
      
      paymentAccounts.Show();
    }

    public override bool CanShowPaymentAccounts(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanShowPaymentAccounts(e);
    }

  }

}