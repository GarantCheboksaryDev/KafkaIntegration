using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.AccountingDocumentBase;

namespace vf.OverrideBaseDev
{
  partial class AccountingDocumentBaseCounterpartyPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> CounterpartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.CounterpartyFiltering(query, e);
      
      query = OverrideBaseDev.PublicFunctions.Counterparty.GetAvailableCounterparty(query).Cast<T>();
      
      return query;
    }
  }


  partial class AccountingDocumentBaseServerHandlers
  {

    public override void BeforeSigning(Sungero.Domain.BeforeSigningEventArgs e)
    {
      base.BeforeSigning(e);
      
      if (_obj.IsOriginalContractAvailable != true)
      {
        var isOriginalContractAvailable = vf.CustomContracts.PublicFunctions.Module.IsOriginalContractAvailable(_obj, e.Signature);
        if (_obj.IsOriginalContractAvailable != isOriginalContractAvailable)
          _obj.IsOriginalContractAvailable = isOriginalContractAvailable;
      }
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      if (_obj.IsOriginalContractAvailable != true)
      {
        var isOriginalContractAvailable = vf.CustomContracts.PublicFunctions.Module.IsOriginalContractAvailable(_obj);
        if (_obj.IsOriginalContractAvailable != isOriginalContractAvailable)
          _obj.IsOriginalContractAvailable = isOriginalContractAvailable;
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsOriginalContractAvailable = vf.CustomContracts.PublicFunctions.Module.IsOriginalContractAvailable(_obj);
    }
  }

}