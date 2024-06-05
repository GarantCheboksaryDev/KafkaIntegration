using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.CurrencyRate;

namespace vf.CustomContracts
{
  partial class CurrencyRateServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if (CurrencyRates.GetAll(x => Sungero.Commons.Currencies.Equals(x.Currency, _obj.Currency) && x.Date == _obj.Date && !CustomContracts.CurrencyRates.Equals(x, _obj)).Any())
        e.AddError(vf.CustomContracts.CurrencyRates.Resources.FoundDuplicates, _obj.Info.Actions.ShowDuplicates);
      _obj.Name = vf.CustomContracts.CurrencyRates.Resources.CurrencyRateNameFormat(_obj.Currency.DisplayValue, _obj.Date.HasValue ? _obj.Date.Value.ToShortDateString() : string.Empty);
    }
  }

}