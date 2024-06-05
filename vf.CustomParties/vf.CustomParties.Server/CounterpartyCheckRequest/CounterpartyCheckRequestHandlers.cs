using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomParties.CounterpartyCheckRequest;

namespace vf.CustomParties
{
  partial class CounterpartyCheckRequestCounterpartyPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CounterpartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(x => x.TIN != null || x.Nonresident == true);
    }
  }

  partial class CounterpartyCheckRequestFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      if (_filter == null || query == null)
        return query;
      
      // Фильтр по контрагенту.
      if (_filter.Counterparty != null)
        query = query.Where(q => OverrideBaseDev.Counterparties.Equals(q.Counterparty, _filter.Counterparty));
      
      // Фильтр "Результат проверки СПАРК".
      var sparkCheckingResults = new List<Enumeration>();
      if (_filter.Green)
        sparkCheckingResults.Add(CounterpartyCheckRequest.CheckingResultSPARK.Green);
      if (_filter.Yellow)
        sparkCheckingResults.Add(CounterpartyCheckRequest.CheckingResultSPARK.Yellow);
      if (_filter.Red)
        sparkCheckingResults.Add(CounterpartyCheckRequest.CheckingResultSPARK.Red);
      
      if (sparkCheckingResults.Any())
        query = query.Where(q => q.CheckingResultSPARK.HasValue && sparkCheckingResults.Contains(q.CheckingResultSPARK.Value));
      
      // Фильтр "Результат проверки СБ".
      if (_filter.CheckingResultSB != null)
        query = query.Where(q => vf.CustomParties.ServiceSecurityCheckResults.Equals(q.CheckingResultSB, _filter.CheckingResultSB));
      
      // Фильтр "Тип заявки".
      var requestTypes = new List<Enumeration>();
      if (_filter.Initiative)
        requestTypes.Add(CounterpartyCheckRequest.RequestType.Initiative);
      if (_filter.NewCounterparty)
        requestTypes.Add(CounterpartyCheckRequest.RequestType.NewCounterparty);
      
      if (requestTypes.Any())
        query = query.Where(q => q.RequestType.HasValue && requestTypes.Contains(q.RequestType.Value));
      
      // Фильтр "Включен в черный список".
      if (_filter.IsInBlackList)
        query = query.Where(q => q.IsInBlackList == true);
      
      // Фильтрация по дате проверки
      var beginDate = Calendar.UserToday.AddDays(-30);
      var endDate = Calendar.UserToday;
      
      if (_filter.Last365days)
        beginDate = Calendar.UserToday.AddDays(-365);
      
      if (_filter.ManualPeriod)
      {
        beginDate = _filter.DateRangeFrom ?? Calendar.SqlMinValue;
        endDate = _filter.DateRangeTo ?? Calendar.SqlMaxValue;
      }

      var serverPeriodBegin = Equals(Calendar.SqlMinValue, beginDate) ? beginDate : Sungero.Docflow.PublicFunctions.Module.Remote.GetTenantDateTimeFromUserDay(beginDate);
      var serverPeriodEnd = Equals(Calendar.SqlMaxValue, endDate) ? endDate : endDate.EndOfDay().FromUserTime();
      var clientPeriodEnd = !Equals(Calendar.SqlMaxValue, endDate) ? endDate.AddDays(1) : Calendar.SqlMaxValue;
      query = query.Where(q => (q.CheckingDate.Between(serverPeriodBegin, serverPeriodEnd) ||
                                q.CheckingDate == beginDate) && q.CheckingDate != clientPeriodEnd);
      
      return query;
    }
  }

  partial class CounterpartyCheckRequestServerHandlers
  {

    public override void BeforeStart(Sungero.Workflow.Server.BeforeStartEventArgs e)
    {
      if (!OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.IncludedInRole(Sungero.ExchangeCore.PublicConstants.Module.RoleGuid.CounterpartiesResponsibleRole))
        e.AddError(vf.CustomParties.CounterpartyCheckRequests.Resources.NotCounterpartyResponsibleError);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsInBlackList = false;
      _obj.ActiveText = vf.CustomParties.CounterpartyCheckRequests.Resources.CounterpartyCheckRequestActiveText;
      Functions.CounterpartyCheckRequest.FillSubject(_obj);
    }
  }

}