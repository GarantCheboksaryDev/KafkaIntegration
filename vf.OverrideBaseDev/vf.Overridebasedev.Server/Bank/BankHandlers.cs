using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Bank;

namespace vf.OverrideBaseDev
{
  partial class BankServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
    }
  }


  partial class BankFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      query = base.Filtering(query, e);
      
      query = OverrideBaseDev.Functions.Bank.GetAvailableBanks(query).Cast<T>();
      
      return query;
    }
  }

}