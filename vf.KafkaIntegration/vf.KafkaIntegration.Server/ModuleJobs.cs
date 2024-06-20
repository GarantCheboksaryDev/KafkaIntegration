using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.KafkaIntegration.Server
{
  public class ModuleJobs
  {
    /// <summary>
    /// Получить информацию о контрагентах.
    /// </summary>
    public virtual void GetCounterpartiesInfoFromKafka()
    {      
      Functions.Module.WriteToLog("Старт процесса.", false);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Counterparty);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKafkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Counterparty);
      
      Functions.Module.WriteToLog(string.Format("Количество сообщений к обработке: {0}.", messagesQueue.Count()), false);
      
      Functions.KafkaQueueItem.CreateAsynchForCounterpartiesMessage(messagesQueue);
      
      Functions.Module.WriteToLog("Конец процесса.", false);
      
    }
    
    /// <summary>
    /// Получить информацию о банках.
    /// </summary>
    public virtual void GetBankInfoFromKafka()
    {      
      Functions.Module.WriteToLog("Старт процесса.", false);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Bank);

      var messagesQueue = Functions.KafkaQueueItem.GetKafkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Bank);
      
      Functions.Module.WriteToLog(string.Format("Количество сообщений к обработке: {0}.", messagesQueue.Count()), false);
      
      // Обработать сообщения и занести данные в DirectumRx.
      Functions.KafkaQueueItem.CreateAsynchForBankMessage(messagesQueue);
      
      Functions.Module.WriteToLog("Конец процесса.", false);
    }
  }
}