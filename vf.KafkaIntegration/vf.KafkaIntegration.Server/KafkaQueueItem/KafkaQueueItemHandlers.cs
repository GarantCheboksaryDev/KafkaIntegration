using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.KafkaIntegration.KafkaQueueItem;

namespace vf.KafkaIntegration
{
  partial class KafkaQueueItemServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      _obj.Name = Sungero.Docflow.PublicFunctions.Module.CutText(vf.KafkaIntegration.KafkaQueueItems.Resources.KafkaQueueItemNameFormat(_obj.TopicName, _obj.MessageId, _obj.ExternalId), _obj.Info.Properties.Name.Length);
    }
  }

}