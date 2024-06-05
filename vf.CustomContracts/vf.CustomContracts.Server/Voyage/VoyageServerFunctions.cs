using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.Voyage;

namespace vf.CustomContracts.Server
{
  partial class VoyageFunctions
  {
    /// <summary>
    /// Получить Рейсы по подразделению.
    /// </summary>
    /// <param name="department">Подразделение.</param>
    /// <returns>Список Рейсов.</returns>
    [Public, Remote(IsPure=true)]
    public static IQueryable<vf.CustomContracts.IVoyage> GetVoyageByDepartment(vf.OverrideBaseDev.IDepartment department)
    {
      return vf.CustomContracts.Voyages.GetAll().Where(x => department != null && CustomContracts.MVZs.Equals(x.Vessel, department.MVZ) && x.Status == vf.CustomContracts.Voyage.Status.Active);
    }
    
    /// <summary>
    /// Получить синхронизированный рейс.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <param name="voyageNumver">Номер рейса.</param>
    /// <returns>Найденный рейс, иначе создать новый.</returns>
    [Public]
    public static IVoyage GetSynchVoyage(string guid1C, string voyageNumver)
    {
      var findVoyage = GetVoyageBy1CGuid(guid1C);
      if (findVoyage == null)
        findVoyage = CustomContracts.Voyages.GetAll(x => !(x.ExternalId != null && x.ExternalId != string.Empty)
                                                    && x.Number != null && x.Number != string.Empty
                                                    && x.Number == voyageNumver).FirstOrDefault();
      
      return findVoyage != null ? findVoyage : CustomContracts.Voyages.Create();
    }
    
    /// <summary>
    /// Найти рейс по Guid.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <returns>Найденный рейс.</returns>
    public static IVoyage GetVoyageBy1CGuid(string guid1C)
    {
      return CustomContracts.Voyages.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
    }
  }
}