using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ApprovalStage;

namespace vf.OverrideBaseDev
{
  partial class ApprovalStageSharedHandlers
  {

    public override void StageTypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      base.StageTypeChanged(e);
      
      if (e.NewValue != e.OldValue)
      {
        if (_obj.SkipApprovalOfSigner == true)
          _obj.SkipApprovalOfSigner = false;
        
        if (_obj.SkipHeadManager == true)
          _obj.SkipHeadManager = false;
        
        Functions.ApprovalStage.SetEnabledAndVisibleProperties(_obj);
      }
    }
  }

}