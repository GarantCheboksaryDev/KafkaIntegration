using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.KafkaIntegration.KafkaQueueItem;

namespace vf.KafkaIntegration
{
  partial class KafkaQueueItemClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      Functions.KafkaQueueItem.SetRequiredAndEnabledProperties(_obj);
    }

  }
}