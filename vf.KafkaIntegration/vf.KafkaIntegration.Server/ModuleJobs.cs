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
      var prefix = "GetCounterpartiesInfoFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Counterparty);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Counterparty);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForCounterpartiesMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
      
    }
    
    /// <summary>
    /// Получить информацию о банках.
    /// </summary>
    public virtual void GetBankInfoFromKafka()
    {
      var prefix = "GetBankInfoFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Bank);

      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Bank);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      // Обработать сообщения и занести данные в DirectumRx.
      Functions.KafkaQueueItem.CreateAsynchForBankMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
  }
}