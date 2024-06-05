using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomParties.CounterpartyCheckRequest;

namespace vf.CustomParties.Shared
{
  partial class CounterpartyCheckRequestFunctions
  {
    /// <summary>
    /// Получить наименование контрагента.
    /// </summary>
    /// <param name="counterparty">Контрагент.</param>
    /// <returns>Для банков и организаций возвращается юридическое наименование, для персон - полное имя.</returns>
    public static string GetCounterpartyName(OverrideBaseDev.ICounterparty counterparty)
    {
      if (counterparty != null)
      {
        var bank = OverrideBaseDev.Banks.As(counterparty);
        var company = OverrideBaseDev.Companies.As(counterparty);
        var person = OverrideBaseDev.People.As(counterparty);
        
        if (bank != null)
          return bank.LegalName;
        else if (company != null)
          return company.LegalName;
        else if (person != null)
          return person.Name;
      }
      return string.Empty;
    }
    
    /// <summary>
    /// Заполнить тему задачи.
    /// </summary>
    public virtual void FillSubject()
    {
      var subject = (_obj.Counterparty != null
                     ? vf.CustomParties.CounterpartyCheckRequests.Resources.CounterpartyCheckRequestSubjectFormat(GetCounterpartyName(_obj.Counterparty))
                     : vf.CustomParties.CounterpartyCheckRequests.Resources.CounterpartyCheckRequestEmptySubject)
        .ToString(Sungero.Core.TenantInfo.Culture);
      
      _obj.Subject = Sungero.Docflow.PublicFunctions.Module.CutText(subject, _obj.Info.Properties.Subject.Length);
    }
  }
}