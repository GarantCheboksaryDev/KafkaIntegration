using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Company;

namespace vf.OverrideBaseDev
{
  partial class CompanyFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = base.Filtering(query, e);
      
      query = OverrideBaseDev.Functions.Company.GetAvailableCompanies(query).Cast<T>();
      
      return query;
    }
  }


}