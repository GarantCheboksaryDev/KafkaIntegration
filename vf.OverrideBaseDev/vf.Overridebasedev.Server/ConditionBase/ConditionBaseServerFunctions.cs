using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ConditionBase;

namespace vf.OverrideBaseDev.Server
{
  partial class ConditionBaseFunctions
  {
    public override string GetConditionName()
    {
      using (TenantInfo.Culture.SwitchTo())
      {
        if (_obj.ConditionType == ConditionType.Registred)
          return vf.OverrideBaseDev.ContractConditions.Resources.DocumentIsRegistred;
      }
      
      return base.GetConditionName();
    }
  }
}