using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.DocumentTemplate;

namespace vf.OverrideBaseDev
{
  partial class DocumentTemplateSharedHandlers
  {

    public override void DocumentTypeChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.DocumentTypeChanged(e);
      
      if (e.NewValue != e.OldValue && _obj.IsStandardTemplate == true)
        _obj.IsStandardTemplate = false;
    }

  }
}