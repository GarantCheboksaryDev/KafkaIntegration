using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractBase;

namespace vf.OverrideBaseDev
{
  partial class ContractBaseSharedHandlers
  {

    public override void BKBDRChanged(vf.OverrideBaseDev.Shared.ContractualDocumentBKBDRChangedEventArgs e)
    {
      base.BKBDRChanged(e);
      Functions.ContractBase.SetRequiredProperties(_obj);
    }

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      
      if (Sungero.Docflow.DocumentKinds.Equals(e.NewValue, e.OriginalValue))
      {
        if (e.NewValue != null)
          _obj.ErpDatabookName = vf.CustomContracts.PublicFunctions.ErpDatabookName.Remote.GetErpDatabookNamesByDocumentKind(e.NewValue).FirstOrDefault();
        else
          _obj.ErpDatabookName = null;
      }
    }

    public override void DocumentRegisterChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentRegisterChangedEventArgs e)
    {
      base.DocumentRegisterChanged(e);
      
      if (!Sungero.Docflow.DocumentRegisters.Equals(e.OldValue, e.NewValue))
      {
        var isInRegistrationGroup = Functions.ContractBase.Remote.CheckCurrentUserIsInRegistrationGroup(_obj);
        e.Params
          .AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.CurrentUserIsInRegistrationGroup, isInRegistrationGroup);
      }
    }

    public override void ContractUniqueNumberChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.ContractUniqueNumberChanged(e);
      
      if (e.NewValue != e.OriginalValue && !string.IsNullOrEmpty(e.NewValue))
        _obj.CurrencyControl = true;
      else if (string.IsNullOrEmpty(e.NewValue))
        _obj.CurrencyControl = null;
    }

    public override void IsAutomaticRenewalChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      base.IsAutomaticRenewalChanged(e);
      
      Functions.ContractBase.SetRequiredProperties(_obj);
    }

    public override void ValidFromEndTypeChanged(vf.OverrideBaseDev.Shared.ContractualDocumentValidFromEndTypeChangedEventArgs e)
    {
      base.ValidFromEndTypeChanged(e);
      
      if (!vf.SAPIntegration.ContractEndTypes.Equals(e.NewValue, e.OldValue))
      {
        _obj.IsAutomaticRenewal = false;
        _obj.DaysToFinishWorks = null;
      }
    }

    public override void RelationshipTypeChanged(vf.OverrideBaseDev.Shared.ContractualDocumentRelationshipTypeChangedEventArgs e)
    {
      base.RelationshipTypeChanged(e);
      if (!CustomContracts.RelationshipTypes.Equals(e.OldValue, e.NewValue))
      {
        _obj.IncotermsKind = null;
        e.Params.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsIncotermsEnabled, Functions.ContractBase.IsIncotermsEnabled(_obj));
      }
    }

    public override void ContractKindChanged(vf.OverrideBaseDev.Shared.ContractualDocumentContractKindChangedEventArgs e)
    {
      base.ContractKindChanged(e);
      
      if (!CustomContracts.ContractKinds.Equals(e.NewValue, e.OldValue))
      {
        var needTransferToSudk = Functions.ContractBase.NeedsTransferToSudk(_obj);
        if (_obj.NeedTransferToSudk != needTransferToSudk)
          _obj.NeedTransferToSudk = needTransferToSudk;
      }
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      
      if (!CustomContracts.ContractKinds.Equals(e.NewValue, e.OldValue))
      {
        var needTransferToSudk = Functions.ContractBase.NeedsTransferToSudk(_obj);
        if (_obj.NeedTransferToSudk != needTransferToSudk)
          _obj.NeedTransferToSudk = needTransferToSudk;
      }
    }

    public override void TripartileContractChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      base.TripartileContractChanged(e);
      Functions.ContractualDocument.SetAdditionalCounterpartyVisibility(_obj);
    }
  }
}