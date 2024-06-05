using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.MVZ;

namespace vf.CustomContracts.Server
{
  partial class MVZFunctions
  {
    /// <summary>
    /// Получить синхронизированное МВЗ.
    /// </summary>
    /// <param name="guid1C">Guid в 1С.</param>
    /// <param name="code">Код МВЗ.</param>
    /// <returns>Найденное МВЗ. Иначе - новое.</returns>
    [Public]
    public static CustomContracts.IMVZ GetMVZSynch(string guid1C, string code)
    {
      var MVZ = GetMVZByGuid(guid1C);
      if (MVZ == null)
        MVZ = CustomContracts.MVZs.GetAll(x => (x.ExternalId == null || x.ExternalId == string.Empty) && x.MVZCode == code).FirstOrDefault();
      
      return MVZ != null ? MVZ : CustomContracts.MVZs.Create();
    }
    
    /// <summary>
    /// Получить МВЗ по Guid в 1С.
    /// </summary>
    /// <param name="guid1C">Guid в 1С.</param>
    /// <returns>Найденное МВЗ</returns>
    [Public]
    public static CustomContracts.IMVZ GetMVZByGuid(string guid1C)
    {
      return CustomContracts.MVZs.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
    }
  }
}