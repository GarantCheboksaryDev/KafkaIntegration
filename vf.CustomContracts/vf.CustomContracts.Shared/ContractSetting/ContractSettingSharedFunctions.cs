using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.ContractSetting;

namespace vf.CustomContracts.Shared
{
  partial class ContractSettingFunctions
  {
    /// <summary>
    /// Получить настройки модуля "Договоры".
    /// </summary>
    /// <param name="businessUnit">Наша организация.</param>
    /// <returns>Настройки модуля.</returns>
    [Public]
    public static CustomContracts.IContractSetting GetContractSettings(Sungero.Company.IBusinessUnit businessUnit)
    {
      return CustomContracts.ContractSettings.GetAllCached(x => businessUnit != null && OverrideBaseDev.BusinessUnits.Equals(x.BusinessUnit, businessUnit)).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить настройки моудля "Договоры" по всем НОР.
    /// </summary>
    /// <returns>Настройки модуля.</returns>
    [Public]
    public static IQueryable<CustomContracts.IContractSetting> GetAllBusinessUnitsContractSettings()
    {
      return CustomContracts.ContractSettings.GetAllCached();
    }
  }
}