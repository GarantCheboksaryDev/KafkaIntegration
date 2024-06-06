using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Person;

namespace vf.OverrideBaseDev.Server
{
  partial class PersonFunctions
  {
    /// <summary>
    /// Получить доступных персон для текущего сотрудника.
    /// </summary>
    /// <param name="query">Базовый список персон.</param>
    /// <returns>Список доступных персон.</returns>
    public static IQueryable<OverrideBaseDev.IPerson> GetAvailablePeople(IQueryable<OverrideBaseDev.IPerson> query)
    {
      var currentEmployee = Sungero.Company.Employees.Current;
      
      if (currentEmployee != null && !currentEmployee.IncludedIn(Roles.Administrators))
      {
        var businessUnit = currentEmployee.Department?.BusinessUnit;
        if (businessUnit == null)
          return query.Where(x => x.Id == -1);
        
        query = query.Where(x => !x.BusinessUnits.Any() || x.BusinessUnits.Any(b => b.BusinessUnit != null && BusinessUnits.Equals(b.BusinessUnit, businessUnit)));
      }

      return query;
    }
    
    /// <summary>
    /// Получить синхронизированного физического лица.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <param name="tin">ИНН.</param>
    /// <returns>Найденное физическое лицо, иначе новое.</returns>
    [Public]
    public static OverrideBaseDev.IPerson GetPersonSynch(string guid1C, string tin)
    {
      var findPerson = GetPersonByGuid(guid1C);
      if (findPerson == null)
        findPerson = Sungero.Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, string.Empty, string.Empty, null, true)
          .Where(x => OverrideBaseDev.People.Is(x)).Cast<OverrideBaseDev.IPerson>()
          .FirstOrDefault();
      
      return findPerson != null ? findPerson : OverrideBaseDev.People.Create();
      
    }
    
    /// <summary>
    /// Получить персону по Guid в 1С.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <returns>Найденная персона.</returns>
    [Public]
    public static OverrideBaseDev.IPerson GetPersonByGuid(string guid1C)
    {
      return OverrideBaseDev.People.GetAll(x => x.ExternalId != null && x.ExternalId != string.Empty && x.ExternalId == guid1C).FirstOrDefault();
    }
  }
}