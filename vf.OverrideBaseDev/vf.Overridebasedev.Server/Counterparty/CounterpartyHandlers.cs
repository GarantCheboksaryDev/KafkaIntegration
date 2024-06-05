using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Counterparty;

namespace vf.OverrideBaseDev
{
  partial class CounterpartyFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = base.Filtering(query, e);
      
      if (_filter == null)
        return query;
      
      #region Наша организация
      
      if (_filter.BusinessUnit != null)
        query = query.Where(x => x.BusinessUnits.Any(b => OverrideBaseDev.BusinessUnits.Equals(b.BusinessUnit, _filter.BusinessUnit)));
      
      #endregion
      
      #region Проверка СБ
      
      if (_filter.ServiceSecurityCheckResult != null)
        query = query.Where(x => vf.CustomParties.ServiceSecurityCheckResults.Equals(x, _filter.ServiceSecurityCheckResult));
      
      #endregion
      
      #region Проверка СПАРК
      
      if (_filter.Green || _filter.Yellow || _filter.Red)
        query = query.Where(x => (_filter.Green && x.CheckingResultSPARK == OverrideBaseDev.Counterparty.CheckingResultSPARK.Green) ||
                            (_filter.Yellow && x.CheckingResultSPARK == OverrideBaseDev.Counterparty.CheckingResultSPARK.Yellow) ||
                            (_filter.Red && x.CheckingResultSPARK == OverrideBaseDev.Counterparty.CheckingResultSPARK.Red));
      
      #endregion
      
      #region Включен в спецсписок
      
      if (_filter.Black || _filter.AddMonitoring)
        query = query.Where(x => (_filter.Black && x.SpecialList == OverrideBaseDev.Counterparty.SpecialList.Black) ||
                            (_filter.AddMonitoring && x.SpecialList == OverrideBaseDev.Counterparty.SpecialList.AddMonitoring));
      
      #endregion
      
      #region Неризидент
      
      if (_filter.Nonresident)
        query = query.Where(x => x.Nonresident == true);
      
      #endregion
      
      return query;
    }
  }

  partial class CounterpartyBusinessUnitsBusinessUnitPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> BusinessUnitsBusinessUnitFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      var currentEmployee = Sungero.Company.Employees.Current;
      
      if (currentEmployee != null && !currentEmployee.IncludedIn(Roles.Administrators))
      {
        var businessUnit = currentEmployee.Department?.BusinessUnit;
        if (businessUnit == null)
          return query.Where(x => x.Id == -1);
        
        query = query.Where(x => BusinessUnits.Equals(x, businessUnit));
      }
      
      return query;
    }
  }


  partial class CounterpartyServerHandlers
  {

    public override void BeforeSaveHistory(Sungero.Domain.HistoryEventArgs e)
    {
      base.BeforeSaveHistory(e);
      
      // Отразить в истории изменения свойства "Результат ОБ".
      if (e.Action == Sungero.CoreEntities.History.Action.Update && !_obj.State.IsInserted &&
          vf.CustomParties.ServiceSecurityCheckResults.Equals(_obj.State.Properties.ServiceSecurityCheckResult.OriginalValue, _obj.ServiceSecurityCheckResult))
        e.Comment = vf.OverrideBaseDev.Counterparties.Resources.ChangePropertyValueFormat(_obj.Info.Properties.ServiceSecurityCheckResult.LocalizedName,
                                                                                          _obj.State.Properties.ServiceSecurityCheckResult.OriginalValue != null ?
                                                                                          _obj.State.Properties.ServiceSecurityCheckResult.OriginalValue.Name : string.Empty,
                                                                                          _obj.ServiceSecurityCheckResult != null ? _obj.ServiceSecurityCheckResult.Name : string.Empty);
    }
  }
}