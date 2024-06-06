using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.CurrencyRate;

namespace vf.CustomContracts.Client
{
  partial class CurrencyRateActions
  {
    public virtual void ShowDuplicates(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.CurrencyRate.Remote.GetCurrencyRateDuplicates(_obj).ShowModal();
    }

    public virtual bool CanShowDuplicates(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}