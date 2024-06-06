using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.IntegrationSettings.Server
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Отправить сообщения с ошибками на обработку.
    /// </summary>
    /// <param name="objectName">Наименование объекта</param>
    /// <returns>True - если успешно отправлено, иначе false.</returns>
    [Remote(IsPure=true)]
    public static bool HandleErrorRecordsServer(string objectName)
    {
      var objectEnumeration = Functions.ConnectSettings.GetKeyValueFromObjectNames(objectName);
      if (objectEnumeration.HasValue)
      {
        var queueItemsWithError = KafkaIntegration.PublicFunctions.KafkaQueueItem.GetKaffkaItemQueueForError(objectEnumeration.Value);
        if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Bank)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForBankMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Counterparty)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForCounterpartiesMessage(queueItemsWithError);
        
        return true;
      }
      
      return false;
    }
  }
}