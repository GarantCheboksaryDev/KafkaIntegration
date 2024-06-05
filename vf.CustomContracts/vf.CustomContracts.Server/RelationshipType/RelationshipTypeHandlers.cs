using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.RelationshipType;

namespace vf.CustomContracts
{
  partial class RelationshipTypeServerHandlers
  {
    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.InventoryItem = false;
      _obj.Service = false;
    }
  }
}