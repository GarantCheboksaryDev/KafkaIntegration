using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.Voyage;

namespace vf.CustomContracts
{
  partial class VoyageServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var handler = CustomContracts.AsyncHandlers.FillInDocumentVoyage.Create();
      handler.VoyageId = _obj.Id;
      handler.ExecuteAsync();
    }
  }

  partial class VoyageContractsContractPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContractsContractFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(x => vf.OverrideBaseDev.ContractBases.Is(x) || vf.OverrideBaseDev.SupAgreements.Is(x));
    }
  }


}