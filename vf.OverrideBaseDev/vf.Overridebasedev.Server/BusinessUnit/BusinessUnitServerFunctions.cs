using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.BusinessUnit;

namespace vf.OverrideBaseDev.Server
{
  partial class BusinessUnitFunctions
  {
    /// <summary>
    /// Получить НОР по коду HCM.
    /// </summary>
    /// <param name="HCMCode">Код HCM.</param>
    /// <returns>Найденная НОР.</returns>
    [Public]
    public static IBusinessUnit GetBusinessUnitByHCMCode(string HCMCode)
    {
      return OverrideBaseDev.BusinessUnits.GetAll(x => x.SAPID == HCMCode).FirstOrDefault();
    }
  }
}