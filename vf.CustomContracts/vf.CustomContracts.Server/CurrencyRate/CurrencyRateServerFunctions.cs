using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.CurrencyRate;

namespace vf.CustomContracts.Server
{
  partial class CurrencyRateFunctions
  {
    /// <summary>
    /// Получить дублирующие записи справочника "Курсы валют".
    /// </summary>
    /// <returns>Дублирующие записи справочника "Курсы валют".</returns>
    [Remote]
    public IQueryable<ICurrencyRate> GetCurrencyRateDuplicates()
    {
      return CurrencyRates.GetAll(x => Sungero.Commons.Currencies.Equals(x.Currency, _obj.Currency) && x.Date == _obj.Date && !CustomContracts.CurrencyRates.Equals(x, _obj));
    }
  }
}