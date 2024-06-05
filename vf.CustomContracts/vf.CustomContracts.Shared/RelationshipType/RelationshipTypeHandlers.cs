using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.RelationshipType;

namespace vf.CustomContracts
{
  partial class RelationshipTypeSharedHandlers
  {

    public virtual void ServiceChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.State.Properties.InventoryItem.IsEnabled = e.NewValue != true;
    }

    public virtual void InventoryItemChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
        _obj.State.Properties.Service.IsEnabled = e.NewValue != true;
    }

  }
}