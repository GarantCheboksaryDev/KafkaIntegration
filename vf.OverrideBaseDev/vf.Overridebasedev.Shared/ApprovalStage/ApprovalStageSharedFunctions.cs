using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ApprovalStage;

namespace vf.OverrideBaseDev.Shared
{
  partial class ApprovalStageFunctions
  {
    public override List<Enumeration?> GetPossibleRoles()
    {
      var baseRoles = base.GetPossibleRoles();
      
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers
          || _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Manager)
        baseRoles.Add(CustomContracts.DirectionManagerApprovalRole.Type.DirectionManager);
      if (_obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers ||
          _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.SimpleAgr ||
          _obj.StageType == Sungero.Docflow.ApprovalStage.StageType.Notice)
        baseRoles.Add(CustomContracts.DirectionManagerApprovalRole.Type.BudgetController);
      
      return baseRoles;
    }
    
    /// <summary>
    /// Установить видимость и доступность свойств.
    /// </summary>
    public void SetEnabledAndVisibleProperties()
    {
      var isApprovalState = _obj.StageType == OverrideBaseDev.ApprovalStage.StageType.Approvers || _obj.StageType == OverrideBaseDev.ApprovalStage.StageType.Manager;

      _obj.State.Properties.SkipApprovalOfSigner.IsVisible = isApprovalState;
      _obj.State.Properties.SkipApprovalOfSigner.IsEnabled = isApprovalState;
      
      _obj.State.Properties.SkipHeadManager.IsVisible = isApprovalState;
      _obj.State.Properties.SkipHeadManager.IsEnabled = isApprovalState;
    }
  }
}