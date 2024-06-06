using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractCondition;

namespace vf.OverrideBaseDev.Server
{
  partial class ContractConditionFunctions
  {
    public override string GetConditionName()
    {
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.ConditionType == ConditionType.IsInSpecialList)
          return vf.OverrideBaseDev.ContractConditions.Resources.SpecialListConditionDisplayNameFormat(vf.OverrideBaseDev.ContractConditions.Info.Properties.SpecialListValue.GetLocalizedValue(_obj.SpecialListValue));
        
        if (_obj.ConditionType == ConditionType.WithDisagrement)
          return vf.OverrideBaseDev.ContractConditions.Resources.DocumentWithDisagreementProtocol;
        
        if (_obj.ConditionType == ConditionType.SvcSecurityChkR)
          return vf.OverrideBaseDev.ContractConditions.Resources.ServiceSecurityCheckResultFormat(_obj.ServiceSecurityCheckResult != null ? _obj.ServiceSecurityCheckResult.Name : string.Empty);
      }
      
      return base.GetConditionName();
    }
  }
}