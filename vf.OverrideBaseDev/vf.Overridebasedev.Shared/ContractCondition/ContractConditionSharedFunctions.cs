using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractCondition;

namespace vf.OverrideBaseDev.Shared
{
  partial class ContractConditionFunctions
  {

    public override void ChangePropertiesAccess()
    {
      base.ChangePropertiesAccess();
      
      var isSpecialList = _obj.ConditionType == ConditionType.IsInSpecialList;
      var isWithDisagreement = _obj.ConditionType == ConditionType.WithDisagrement;
      var isServiceSecurityCheckResult = _obj.ConditionType == ConditionType.SvcSecurityChkR;
      
      var properties = _obj.State.Properties;
      properties.SpecialListValue.IsVisible = isSpecialList;
      properties.SpecialListValue.IsRequired = isSpecialList;
      properties.DisagreementDocumentKinds.IsVisible = isWithDisagreement;
      properties.DisagreementDocumentKinds.IsRequired = isWithDisagreement;
      properties.ServiceSecurityCheckResult.IsRequired = isServiceSecurityCheckResult;
      properties.ServiceSecurityCheckResult.IsEnabled = isServiceSecurityCheckResult;
    }
    
    public override void ClearHiddenProperties()
    {
      base.ClearHiddenProperties();
      
      if (!_obj.State.Properties.SpecialListValue.IsVisible)
        _obj.SpecialListValue = null;
      if (!_obj.State.Properties.DisagreementDocumentKinds.IsVisible)
        _obj.DisagreementDocumentKinds.Clear();
      if (!_obj.State.Properties.ServiceSecurityCheckResult.IsVisible)
        _obj.ServiceSecurityCheckResult = null;
    }
    
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckCondition(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalTask task)
    {
      if (_obj.ConditionType == ConditionType.IsInSpecialList)
      {
        var contractualDocument = Sungero.Docflow.ContractualDocumentBases.As(document);
        if (contractualDocument != null)
        {
          var counterparty = OverrideBaseDev.Counterparties.As(contractualDocument.Counterparty);
          if (counterparty != null)
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(counterparty.SpecialList == _obj.SpecialListValue, string.Empty);
          else
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, vf.OverrideBaseDev.ContractConditions.Resources.CounterpartyNotFilled);
        }
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, vf.OverrideBaseDev.ContractConditions.Resources.DocumentHasWrongType);
      }
      
      if (_obj.ConditionType == ConditionType.WithDisagrement)
      {
        var documentKinds = _obj.DisagreementDocumentKinds.Select(x => x.DocumentKind).ToList();
        var hasDisagreementProtocol = OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.HasRelationsWithGivenDocumentKinds(document, documentKinds);
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(hasDisagreementProtocol, string.Empty);
      }
      
      if (_obj.ConditionType == ConditionType.SvcSecurityChkR)
      {
        var contractualDocument = Sungero.Docflow.ContractualDocumentBases.As(document);
        if (contractualDocument != null)
        {
          var counterparty = OverrideBaseDev.Counterparties.As(contractualDocument.Counterparty);
          if (counterparty != null)
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(vf.CustomParties.ServiceSecurityCheckResults.Equals(counterparty.ServiceSecurityCheckResult, _obj.ServiceSecurityCheckResult), string.Empty);
          else
            return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, vf.OverrideBaseDev.ContractConditions.Resources.CounterpartyNotFilled);
        }
        else
          return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(null, vf.OverrideBaseDev.ContractConditions.Resources.DocumentHasWrongType);
      }
      
      return base.CheckCondition(document, task);
    }
    
    public override System.Collections.Generic.Dictionary<string, List<Enumeration?>> GetSupportedConditions()
    {
      var baseSupport = base.GetSupportedConditions();
      
      baseSupport[Constants.Docflow.DocumentKind.ContractGuid].Add(ConditionType.IsInSpecialList);
      baseSupport[Constants.Docflow.DocumentKind.SupAgreementGuid].Add(ConditionType.IsInSpecialList);
      baseSupport[Constants.Docflow.DocumentKind.ContractStatementGuid].Add(ConditionType.IsInSpecialList);
      baseSupport[Constants.Docflow.DocumentKind.UTDGuid].Add(ConditionType.IsInSpecialList);
      baseSupport[Constants.Docflow.DocumentKind.WaybillGuid].Add(ConditionType.IsInSpecialList);      
      baseSupport[Constants.Docflow.DocumentKind.ContractGuid].Add(ConditionType.WithDisagrement);
      baseSupport[Constants.Docflow.DocumentKind.SupAgreementGuid].Add(ConditionType.WithDisagrement);
      
      var contractuals = Sungero.Docflow.PublicFunctions.DocumentKind.GetDocumentGuids(typeof(IContractualDocument));
      foreach (var typeGuid in contractuals)
        baseSupport[typeGuid].Add(ConditionType.SvcSecurityChkR);

      return baseSupport;
    }
  }
}