using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.CFO;

namespace vf.CustomContracts.Server
{
  partial class CFOFunctions
  {
    /// <summary>
    /// Получить синхронизированный ЦФО.
    /// </summary>
    /// <param name="guid1C">Guid в 1С.</param>
    /// <param name="code">Код ЦФО.</param>
    /// <returns>Найденный ЦФО. Иначе - новый.</returns>
    [Public]
    public static CustomContracts.ICFO GetCFOSynch(string guid1C, string code)
    {
      var CFO = GetCFOByGuid(guid1C);
      if (CFO == null)
        CFO = CustomContracts.CFOs.GetAll(x => (x.ExternalId == null || x.ExternalId == string.Empty) && x.CFOCode == code).FirstOrDefault();
      
      return CFO != null ? CFO : CustomContracts.CFOs.Create();
    }
    
    /// <summary>
    /// Получить ЦФО по Guid в 1С.
    /// </summary>
    /// <param name="guid1C">Guid в 1С.</param>
    /// <returns>Найденный ЦФО</returns>
    [Public]
    public static CustomContracts.ICFO GetCFOByGuid(string guid1C)
    {
      return CustomContracts.CFOs.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить все подразделения ЦФО.
    /// </summary>
    /// <returns>Список подразделений.</returns>
    [Public]
    public static IQueryable<OverrideBaseDev.IDepartment> GetAllDepartment()
    {
      return OverrideBaseDev.Departments.GetAll(x => x.Status == Sungero.CoreEntities.DatabookEntry.Status.Active && x.CFO != null);
    }
  }
}