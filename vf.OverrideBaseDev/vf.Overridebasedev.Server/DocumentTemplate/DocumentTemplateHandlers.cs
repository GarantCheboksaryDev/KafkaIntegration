using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.DocumentTemplate;

namespace vf.OverrideBaseDev
{
  partial class DocumentTemplateServerHandlers
  {

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.IsStandardTemplate = false;
    }
  }

}