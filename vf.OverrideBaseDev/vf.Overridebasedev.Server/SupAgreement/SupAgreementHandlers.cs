using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.SupAgreement;

namespace vf.OverrideBaseDev
{
  partial class SupAgreementBKBDRPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> BKBDRFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      Enumeration? сontractKind = null;
      
      if (_obj.LeadingDocument != null)
      {
        var contract = vf.OverrideBaseDev.ContractualDocuments.As(_obj.LeadingDocument);
        if (contract != null)
        {
          if (contract.ContractType == vf.OverrideBaseDev.ContractualDocument.ContractType.Income)
            сontractKind = vf.CustomContracts.BKBDR.ContractKind.Income;
          else if (contract.ContractType == vf.OverrideBaseDev.ContractualDocument.ContractType.Outcome)
            сontractKind = vf.CustomContracts.BKBDR.ContractKind.Outcome;
        }
      }
      
      return query.Where(x => x.ContractKind == сontractKind
                         && !query.Any(b => CustomContracts.BKBDRs.Equals(b.Level1Article, x))
                         && !query.Any(b => CustomContracts.BKBDRs.Equals(b.Level2Article, x))
                         && !query.Any(b => CustomContracts.BKBDRs.Equals(b.Level3Article, x)));
    }
  }

  partial class SupAgreementServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);

      if (_obj.LeadingDocument != null
          && _obj.LeadingDocument.LifeCycleState == ContractBase.LifeCycleState.Active
          && _obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Active
          && (_obj.TotalAmount.HasValue && _obj.TotalAmount != _obj.State.Properties.TotalAmount.OriginalValue
              || !Sungero.Commons.Currencies.Equals(_obj.Currency, _obj.State.Properties.Currency.OriginalValue)
              || _obj.SignDate != _obj.State.Properties.SignDate.OriginalValue
              || _obj.LifeCycleState != _obj.State.Properties.LifeCycleState.OriginalValue))
      {
        var handler = CustomContracts.AsyncHandlers.FillInContractTotalSum.Create();
        handler.SupAgreementId = _obj.Id;
        handler.ExecuteAsync();
        
        if (_obj.UpdatedTotalSum != true)
          _obj.UpdatedTotalSum = true;
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.SupAgreementType = SupAgreementType.ChangesText;
      _obj.NeedTransferToSudk = false;
      _obj.IsAutoRenewal = false;
    }
  }

}