using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ApprovalStage;

namespace vf.OverrideBaseDev
{
  partial class ApprovalStageServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.SkipApprovalOfSigner = false;
      _obj.SkipHeadManager = false;
    }
  }

}