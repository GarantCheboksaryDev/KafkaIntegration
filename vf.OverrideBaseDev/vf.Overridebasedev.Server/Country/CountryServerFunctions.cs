using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Country;

namespace vf.OverrideBaseDev.Server
{
  partial class CountryFunctions
  {
    /// <summary>
    /// Получить страну по альфа коду 3.
    /// </summary>
    /// <param name="alphaCode3">Альфа код 3.</param>
    /// <returns>Страна.</returns>
    [Public]
    public static ICountry GetCountryByAlphaCode3(string alphaCode3)
    {
      return Countries.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.CodeAlpha3 == alphaCode3).FirstOrDefault();
    }
  }
}