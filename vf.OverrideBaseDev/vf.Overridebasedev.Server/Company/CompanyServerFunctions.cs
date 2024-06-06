using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Company;

namespace vf.OverrideBaseDev.Server
{
  partial class CompanyFunctions
  {
    /// <summary>
    /// Создать запрос на проверку контрагента.
    /// </summary>
    /// <returns>Запрос на проверку контрагента.</returns>
    [Remote]
    public CustomParties.ICounterpartyCheckRequest CreateCounterpartyCheckRequest()
    {
      var request = CustomParties.CounterpartyCheckRequests.Create();
      request.Counterparty = _obj;
      return request;
    }
    
    /// <summary>
    /// Получить доступных организаций для текущего сотрудника.
    /// </summary>
    /// <param name="query">Базовый список организаций.</param>
    /// <returns>Список доступных организаций.</returns>
    [Public]
    public static IQueryable<OverrideBaseDev.ICompany> GetAvailableCompanies(IQueryable<OverrideBaseDev.ICompany> query)
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
    ///  Получить синхронизированную организацию.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <returns>Найденная организация, иначе новая.</returns>
    [Public]
    public static OverrideBaseDev.ICompany GetCompanySynch(string guid1C, string tin, string trrc)
    {
      var findCompany = OverrideBaseDev.Companies.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
      if (findCompany == null)
        findCompany = Sungero.Parties.PublicFunctions.Counterparty.GetDuplicateCounterparties(tin, trrc, string.Empty, null, true)
          .Where(x => OverrideBaseDev.Companies.Is(x)).Cast<OverrideBaseDev.ICompany>()
          .FirstOrDefault();
      
      return findCompany != null ? findCompany : OverrideBaseDev.Companies.Create();
      
    }
  }
}