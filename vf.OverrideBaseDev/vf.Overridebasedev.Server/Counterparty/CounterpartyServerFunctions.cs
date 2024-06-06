using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Counterparty;

namespace vf.OverrideBaseDev.Server
{
  partial class CounterpartyFunctions
  {
    /// <summary>
    /// Получить доступных контрагентов для текущего сотрудника.
    /// </summary>
    /// <param name="query">Базовый список контрагентов.</param>
    /// <returns>Список доступных контрагентов.</returns>
    [Public]
    public static IQueryable<Sungero.Parties.ICounterparty> GetAvailableCounterparty(IQueryable<Sungero.Parties.ICounterparty> query)
    {
      var currentEmployee = Sungero.Company.Employees.Current;
      
      if (currentEmployee != null && !currentEmployee.IncludedIn(Roles.Administrators))
      {
        var businessUnit = currentEmployee.Department?.BusinessUnit;
        if (businessUnit == null)
          return query.Where(x => x.Id == -1);
        
        query = query.Where(x => !OverrideBaseDev.Counterparties.As(x).BusinessUnits.Any() || OverrideBaseDev.Counterparties.As(x).BusinessUnits.Any(b => b.BusinessUnit != null && BusinessUnits.Equals(b.BusinessUnit, businessUnit)));
      }

      return query;
    }
    
    /// <summary>
    /// Получить дубли контрагентов.
    /// </summary>
    /// <param name="counterparties">Контрагенты, среди которых будет осуществляться поиск дублей.</param>
    /// <param name="tin">ИНН.</param>
    /// <param name="trrc">КПП.</param>
    /// <param name="name">Наименование контрагента.</param>
    /// <param name="excludeClosed">Признак необходимости исключить закрытые записи.</param>
    /// <returns>Список дублей контрагентов.</returns>
    public static List<ICounterparty> GetDuplicateCounterparties(IQueryable<ICounterparty> counterparties, string tin, string trrc, string name, bool excludeClosed)
    {
      var searchByName = !string.IsNullOrWhiteSpace(name);
      var searchByTin = !string.IsNullOrWhiteSpace(tin);
      var searchByTrrc = !string.IsNullOrWhiteSpace(trrc);
      
      if (!searchByName && !searchByTin)
        return new List<ICounterparty>();
      
      var duplicates = new List<ICounterparty>();
      
      // Отфильтровать закрытые сущности.
      if (excludeClosed)
        counterparties = counterparties.Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed);
      
      // Поиск по ИНН, если ИНН передан.
      if (searchByTin)
      {
        var counterpartiesByTin = counterparties.Where(x => x.TIN == tin);
        
        // Поиск по КПП, если КПП передан.
        if (searchByTrrc)
        {
          // Поиск по КПП или пустому КПП. Контрагент с пустым КПП также является потенциальным дублем.
          var companies = counterpartiesByTin.Where(c => CompanyBases.Is(c)).Select(c => CompanyBases.As(c));
          duplicates = companies.Where(x => x.TRRC == trrc || x.TRRC == null || x.TRRC.Trim() == string.Empty).ToList<ICounterparty>();
          
          // Поиск по Имени с пустыми ИНН и КПП, если ничего не найдено раньше.
          if (duplicates.Count == 0 && searchByName)
          {
            companies = counterparties.Where(x => CompanyBases.Is(x)).Select(x => CompanyBases.As(x));
            duplicates = companies.Where(x => (x.TIN == null || x.TIN.Trim() == string.Empty) &&
                                         (x.TRRC == null || x.TRRC.Trim() == string.Empty) &&
                                         string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase)).ToList<ICounterparty>();
          }
        }
        else
        {
          // Поиск по Имени с пустыми ИНН, если задано Имя, но по ИНН ничего не найдено.
          if (counterpartiesByTin.Count() == 0 && searchByName)
          {
            var counterpartiesByName = counterparties.Where(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            duplicates = counterpartiesByName.Where(x => x.TIN == null || x.TIN.Trim() == string.Empty).ToList();
          }
          else
            duplicates = counterpartiesByTin.ToList();
        }
      }
      
      // Поиск по Имени, если задано только Имя.
      if (searchByName && !searchByTin)
        duplicates = counterparties.Where(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase)).ToList();
      
      return duplicates;
    }
    
    /// <summary>
    /// Получить контрагента по гуиду 1С.
    /// </summary>
    /// <param name="guid1C">Гуид 1С.</param>
    /// <returns>Найденный контрагент.</returns>
    [Public]
    public static OverrideBaseDev.ICounterparty GetCounterpartyFromGuid1C(string guid1C)
    {
      return OverrideBaseDev.Counterparties.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
    }
    
  }
}