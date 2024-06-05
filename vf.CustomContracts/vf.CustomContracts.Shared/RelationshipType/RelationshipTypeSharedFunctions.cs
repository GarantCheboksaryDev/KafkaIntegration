using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.RelationshipType;

namespace vf.CustomContracts.Shared
{
  partial class RelationshipTypeFunctions
  {
    /// <summary>
    /// Установить видимость и доступность свойств.
    /// </summary>
    public void SetEnabledAndVisibleProperty()
    {
      var properties = _obj.State.Properties;
      
      properties.InventoryItem.IsEnabled = _obj.Service != true;
      properties.Service.IsEnabled = _obj.InventoryItem != true;
    }
  }
}