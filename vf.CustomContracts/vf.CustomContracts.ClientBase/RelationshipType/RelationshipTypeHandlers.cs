using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.RelationshipType;

namespace vf.CustomContracts
{
  partial class RelationshipTypeClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      Functions.RelationshipType.SetEnabledAndVisibleProperty(_obj);
    }

    public override void Showing(Sungero.Presentation.FormShowingEventArgs e)
    {
      _obj.State.Properties.ContractType.IsRequired = true;
    }

  }
}