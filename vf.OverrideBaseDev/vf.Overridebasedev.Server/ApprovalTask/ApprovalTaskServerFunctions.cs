using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ApprovalTask;

namespace vf.OverrideBaseDev.Server
{
  partial class ApprovalTaskFunctions
  {
    [Remote(IsPure = true)]
    public override Sungero.Docflow.Structures.Module.DefinedApprovalStages GetApprovalStages()
    {
      return base.GetApprovalStages();
    }
    
    public override void UpdateReglamentApprovers(Sungero.Docflow.IApprovalRuleBase rule, List<Sungero.Docflow.Structures.Module.DefinedApprovalBaseStageLite> stages)
    {
      base.UpdateReglamentApprovers(rule, stages);
      
      if (rule == null)
        return;
      
      var approvalStages = Functions.ApprovalTask.CastToDefinedApprovalStagesLite(stages);
      Functions.ApprovalTask.RemoveDirectionManagerFromRequiredApprovers(_obj, approvalStages);
    }
  }
}