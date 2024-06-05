using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.KafkaIntegration.KafkaQueueItem;

namespace vf.KafkaIntegration.Shared
{
  partial class KafkaQueueItemFunctions
  {
    /// <summary>
    /// Установить видимость и обязательность свойств.
    /// </summary>
    public void SetRequiredAndEnabledProperties()
    {
      var isError = !string.IsNullOrEmpty(_obj.ErrorText);
      
      _obj.State.Properties.ErrorText.IsVisible = isError;
      
      foreach (var prop in _obj.State.Properties)
        prop.IsEnabled = false;
    }
  }
}