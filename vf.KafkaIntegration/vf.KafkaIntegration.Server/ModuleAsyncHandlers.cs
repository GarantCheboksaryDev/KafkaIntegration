using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.KafkaIntegration.Server
{
  public class ModuleAsyncHandlers
  {

    #region Контрагенты
    
    /// <summary>
    /// Обновить информацию о контрагенте.
    /// </summary>
    /// <param name="args">Аргументы АО.</param>
    public virtual void UpdateCounterpartiesInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateCounterpartiesInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      Functions.Module.WriteToLog("Старт процесса.", false);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Functions.Module.WriteToLog("Не найдена запись справочника \"Очереди сообщения\"", true);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Functions.Module.WriteToLog("Не заполнено тело Json запроса", true);
        return;
      }
      
      // Дессериализовать Json.
      var counterpartyInfo = IsolatedFunctions.DeserializeObject.DesirializeCounterpartiesInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (counterpartyInfo != null)
      {
        try
        {
          Functions.Module.SetCounterpartyInfo(counterpartyInfo, queueItem);
        }
        catch (Exception ex)
        {
          Functions.Module.WriteToLog(string.Format("Во время обработки произошла ошибка: {0}", ex), true);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Functions.Module.WriteToLog("Во время преобразования произошла ошибка.", true);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Functions.Module.WriteToLog("Конец процесса.", false);
    }
    
    #endregion
    
    #region Банки
    
    /// <summary>
    /// Обновить информацию о банке.
    /// </summary>
    /// <param name="args">Аргументы АО.</param>
    public virtual void UpdateBankInfo(vf.KafkaIntegration.Server.AsyncHandlerInvokeArgs.UpdateBankInfoInvokeArgs args)
    {
      var queueId = args.QueueId;
      
      Functions.Module.WriteToLog("Старт процесса.", false);
      
      var queueItem = KafkaQueueItems.GetAll(x => x.Id == queueId).FirstOrDefault();
      
      if (queueItem == null)
      {
        Functions.Module.WriteToLog("Не найдена запись справочника \"Очереди сообщения\"", true);
        return;
      }
      
      var jsonValue = queueItem.JsonBodyValue;
      
      if (string.IsNullOrEmpty(jsonValue))
      {
        Functions.Module.WriteToLog("Не заполнено тело Json запроса", true);
        return;
      }
      
      // Дессериализовать Json.
      var bankInfo = IsolatedFunctions.DeserializeObject.DesirializeBankInfo(jsonValue);
      
      #region Изменить данные в Directum Rx
      
      if (bankInfo != null)
      {
        try
        {
          Functions.Module.SetBankInfo(bankInfo, queueItem);
        }
        catch (Exception ex)
        {
          Functions.Module.WriteToLog(string.Format("Во время обработки произошла ошибка: {0}", ex), true);
          
          if (queueItem.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
            queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
          
          queueItem.ErrorText = ex.Message;
          
          args.Retry = true;
        }
      }
      else
        Functions.Module.WriteToLog("Во время преобразования произошла ошибка.", true);
      
      #endregion
      
      if (queueItem.State.IsChanged)
        queueItem.Save();
      
      Functions.Module.WriteToLog("Конец процесса.", false);
    }
    
    #endregion
  }
}