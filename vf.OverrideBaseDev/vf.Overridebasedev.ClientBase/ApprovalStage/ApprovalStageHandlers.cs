using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ApprovalStage;

namespace vf.OverrideBaseDev
{
  partial class ApprovalStageClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.ApprovalStage.SetEnabledAndVisibleProperties(_obj);
    }

  }
}