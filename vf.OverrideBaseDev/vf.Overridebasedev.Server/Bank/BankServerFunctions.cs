using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Bank;

namespace vf.OverrideBaseDev.Server
{
  partial class BankFunctions
  {
    /// <summary>
    /// Получить доступных банков для текущего сотрудника.
    /// </summary>
    /// <param name="query">Базовый список банков.</param>
    /// <returns>Список доступных банков.</returns>
    public static IQueryable<OverrideBaseDev.IBank> GetAvailableBanks(IQueryable<OverrideBaseDev.IBank> query)
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
    /// Получить синхронизированный банк.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <param name="bic">БИК.</param>
    /// <param name="swift">SWIFT.</param>
    /// <param name="bankBranch">Филиал банка.</param>
    /// <param name="nonResident">Нерезидент.</param>
    /// <returns>Найденный банк, иначе новый.</returns>
    [Public]
    public static IBank GetSynchBank(string guid1C, string bic, string swift, string bankBranch, bool nonResident)
    {
      var findBank = Banks.GetAll(x => x.ExternalId == guid1C
                                  || (!nonResident && bic != string.Empty && bic != null && x.BIC == bic
                                      || (nonResident && swift != string.Empty && swift != null && x.SWIFT == swift
                                          && (bankBranch != string.Empty && bankBranch != null && x.BankBranch == bankBranch
                                              || bankBranch == string.Empty
                                              || bankBranch == null
                                              || x.BankBranch == string.Empty
                                              || x.BankBranch == null
                                             )
                                         )
                                     )
                                 ).FirstOrDefault();
      
      return findBank != null ? findBank : Banks.Create();
    }
    
    /// <summary>
    /// Получить банки с одинаковым SWIFT.
    /// </summary>
    /// <param name="excludeClosed">Исключить закрытые.</param>
    /// <returns>Банки с одинаковым SWIFT.</returns>
    [Public, Remote]
    public List<OverrideBaseDev.IBank> GetBanksWithSameSwiftAnBankBranch(bool excludeClosed)
    {
      var banks = Banks.GetAll();
      
      // Отфильтровать закрытые сущности.
      if (excludeClosed)
        banks = banks.Where(x => x.Status != Sungero.CoreEntities.DatabookEntry.Status.Closed);
      
      return banks.Where(b => b.SWIFT == _obj.SWIFT && b.Id != _obj.Id
                         && (_obj.BankBranch != null && _obj.BankBranch != string.Empty && b.BankBranch == _obj.BankBranch
                             || _obj.BankBranch == null
                             || _obj.BankBranch == string.Empty)).ToList();
    }
    
    /// <summary>
    /// Получить банк по гуиду 1С.
    /// </summary>
    /// <param name="guid1C">Гуид 1С.</param>
    /// <returns>Найденный банк.</returns>
    [Public]
    public static OverrideBaseDev.IBank GetBankFromGuid1C(string guid1C)
    {
      return OverrideBaseDev.Banks.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
    }
  }
}