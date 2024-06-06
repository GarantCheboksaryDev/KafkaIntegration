using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.DocumentTemplate;

namespace vf.OverrideBaseDev
{
  partial class DocumentTemplateClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.DocumentTemplate.SetEnableProperties(_obj);
    }

  }
}