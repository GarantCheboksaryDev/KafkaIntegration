using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.KafkaIntegration.KafkaQueueItem;

namespace vf.KafkaIntegration.Server
{
  partial class KafkaQueueItemFunctions
  {
    /// <summary>
    /// Прочитать сообщения из Kafka и записать в очереди сообщений.
    /// </summary>
    /// <param name="objectName">Наименование объекта чтения.</param>
    public static void GetMessagesAndCreateQueueItem(Enumeration objectName)
    {
      var prefix = string.Format("GetMessagesAndCreateQueueItem. {0}. ", objectName);
      
      var settings = IntegrationSettings.PublicFunctions.ConnectSettings.GetSettingsKafkaConnector();
      if (settings != null)
      {
        // Получить дополнительные настройки для объекта.
        var topicName = settings.ObjectSettings.Where(x => x.MessageType == IntegrationSettings.ConnectSettingsObjectSettings.MessageType.Income
                                                      && x.ObjectName == objectName
                                                      && x.ActionType == IntegrationSettings.ConnectSettingsObjectSettings.ActionType.CreateUpdate).Select(x => x.TopicName).FirstOrDefault();
        
        // Создать экземлпяр коннектора к Kafka.
        KaffkaNet.Connector connector = Functions.Module.CreateKaffkaConnector(settings);
        try
        {
          // Создать экземпляр для чтения сообщений из Kafka.
          var messages = new List<KaffkaNet.ResponseMessages>();
          
          if (!string.IsNullOrEmpty(topicName))
          {
            var addedMessages = connector.ReadMessagesFromTopic(connector, topicName);
            
            if (addedMessages.Any())
              messages.AddRange(addedMessages);
          }
          else
            Logger.DebugFormat("{0}Не найдены настройки для топика добавления/создания.", prefix);
          
          if (messages.Any())
          {
            foreach (var message in messages)
            {
              try
              {
                // Записать сообщения в справочник "Очереди сообщения".
                if (objectName == vf.KafkaIntegration.KafkaQueueItem.ObjectName.CFOArticle || objectName == vf.KafkaIntegration.KafkaQueueItem.ObjectName.RegisterDeprt)
                {
                  var register = IsolatedFunctions.DeserializeObject.SplitRegister(message.Value);
                  foreach (var registerItem in register)
                    CreateKafkaQueueItem(message, objectName, topicName, registerItem);
                }
                else
                  CreateKafkaQueueItem(message, objectName, topicName, message.Value);
              }
              catch(Exception ex)
              {
                Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}.", prefix, ex);
                continue;
              }
            }
          }
        }
        catch(Exception ex)
        {
          Logger.ErrorFormat("{0}Во время обработки произошла ошибка: {1}.", prefix, ex);
          return;
        }
      }
      else
        Logger.ErrorFormat("{0}Не найдены настройки для подключения к Kafka.", prefix);
    }
    
    /// <summary>
    /// Создать запись справочника "Очереди сообщений kafka".
    /// </summary>
    /// <param name="message">Сообщение из kafka.</param>
    /// <param name="objectName">Тип объекта.</param>
    /// <param name="topicName">Наименование топика из kafka.</param>
    /// <param name="messageValue">Содержание сообщения.</param>
    public static void CreateKafkaQueueItem(KaffkaNet.ResponseMessages message, Enumeration objectName, string topicName, string messageValue)
    {
      var queueItem = KafkaQueueItems.Create();
      queueItem.ExternalId = !string.IsNullOrEmpty(message.Key) ? message.Key.Trim() : message.MessageId;
      queueItem.MessageId = message.MessageId;
      queueItem.ObjectName = objectName;
      queueItem.TopicName = topicName;
      queueItem.LastUpdate = message.Created;
      queueItem.JsonBodyValue = !string.IsNullOrEmpty(messageValue) ? messageValue : message.Value;
      queueItem.MessageType = KafkaIntegration.KafkaQueueItem.MessageType.Inner;
      queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.NotProcessed;
      queueItem.Save();
    }
    
    /// <summary>
    /// Получить записи сообщения для обработки.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <returns>Список записей справочника "Очереди сообщения".</returns>
    public static IQueryable<IKafkaQueueItem> GetKaffkaItemQueueForProccess(Enumeration objectName)
    {
      return KafkaQueueItems.GetAll(x => x.ObjectName == objectName && (x.ProcessingStatus == KafkaIntegration.KafkaQueueItem.ProcessingStatus.NotProcessed));
    }
    
    /// <summary>
    /// Получить записи сообщения с ошибками и необработанные.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <returns>Список записей справочника "Очереди сообщения".</returns>
    [Public]
    public static IQueryable<IKafkaQueueItem> GetKaffkaItemQueueForError(Enumeration objectName)
    {
      return KafkaQueueItems.GetAll(x => x.ObjectName == objectName && (x.ProcessingStatus == KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error || x.ProcessingStatus == KafkaIntegration.KafkaQueueItem.ProcessingStatus.NotProcessed));
    }
    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений с информацией о банках.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForBankMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateBankInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений с информацией о рейсах.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForVoyageMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateVoyageInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }

    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений с информацией о котрагентах.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForCounterpartiesMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateCounterpartiesInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений с информацией о расчетных счетах.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForPaymentAccountMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdatePaymentAccountsInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений с информацией о расчетных счетах наших организаций.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForOurPaymentAccountMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateOurPaymentAccountsInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений с информацией о статьях расхода.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForBudgetItemsMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateBudgetItemsInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Запустить асинхронные обработчики для обработки сообщений с информацией о сотрудниках.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForEmployeesMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateEmployeesInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений с информацией о персонах.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForPersonMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdatePeopleInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }

    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений с информацией о подразделениях.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForDepartmentsMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateDepartmentsInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений с информацией о ЦФО/МВЗ.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForCompanyStructureMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateCompanyStructureInfo.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений по регистру ЦФО-Статьи БК.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForCFOArticleItemMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateCFOArticleItem.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    /// <summary>
    /// Запустить асинхронный обработчик для обработки сообщений по регистру Подразделение-ЦФО-МВЗ-Менеджер.
    /// </summary>
    /// <param name="messagesQueue">Список сообщений.</param>
    [Public]
    public static void CreateAsynchForDepartmentRegisterItemMessage(IQueryable<IKafkaQueueItem> messagesQueue)
    {
      if (messagesQueue.Any())
      {
        foreach (var messageQueue in messagesQueue)
        {
          // Запустить АО для обработки.
          var asynch = AsyncHandlers.UpdateDepartmentRegisterItem.Create();
          asynch.QueueId = messageQueue.Id;
          asynch.ExecuteAsync();
        }
      }
    }
    
    
    /// <summary>
    /// Создать сообщение для отправки.
    /// </summary>
    /// <param name="jsonValue">Тело JSON.</param>
    /// <param name="topicName">Наименование топика.</param>
    /// <param name="objectName">Наименование объекта.</param>
    /// <returns>Созданное сообщение.</returns>
    public static IKafkaQueueItem CreateKafkaQueueItemForSend(string jsonValue, string topicName, Enumeration objectName)
    {
      var prefix = string.Format("CreateKafkaQueueItem. {0}. ", topicName);
      
      var queueItem = KafkaQueueItems.Create();
      try
      {
        queueItem.MessageType = KafkaIntegration.KafkaQueueItem.MessageType.Outgoing;
        queueItem.ObjectName = objectName;
        queueItem.TopicName = topicName;
        queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.NotProcessed;
        queueItem.JsonBodyValue = jsonValue;
        queueItem.Save();
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("{0}Во время сохранения записи возникла ошибка.", ex);
      }
      
      return queueItem;
    }
    
    /// <summary>
    /// Создать сообщение для отправки.
    /// </summary>
    /// <param name="jsonValue">Тело JSON.</param>
    /// <param name="topicName">Наименование топика.</param>
    /// <param name="keyValue">Ключ сообщения.</param>
    /// <param name="objectName">Наименование объекта.</param>
    /// <returns>Созданное сообщение.</returns>
    public static IKafkaQueueItem CreateKafkaQueueItemForSend(string jsonValue, string topicName, string keyValue, Enumeration objectName)
    {
      var prefix = string.Format("CreateKafkaQueueItem. {0}. ", topicName);
      
      var queueItem = KafkaQueueItems.Create();
      try
      {
        queueItem.MessageType = KafkaIntegration.KafkaQueueItem.MessageType.Outgoing;
        queueItem.ObjectName = objectName;
        queueItem.TopicName = topicName;
        queueItem.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.NotProcessed;
        queueItem.JsonBodyValue = jsonValue;
        queueItem.ExternalId = keyValue;
        queueItem.Save();
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("{0}Во время сохранения записи возникла ошибка.", ex);
      }
      
      return queueItem;
    }
    
    /// <summary>
    /// Отправить соощение в Кафку.
    /// </summary>
    /// <param name="connectSettings">Настройки интеграции.</param>
    public void SendMessageToKafka(IntegrationSettings.IConnectSettings connectSettings)
    {
      var bootstrapServers = connectSettings.WebServiceAddressee;
      var login = connectSettings.Login;
      var password = connectSettings.Password;
      var logPath = connectSettings.LogFilePath;
      
      var connector = new KaffkaNet.KafkaProducer(bootstrapServers, login, password, logPath);
      
      var sendResult = connector.ProduceMessage(connector, _obj.TopicName, _obj.JsonBodyValue, _obj.ExternalId);
      
      if (!string.IsNullOrEmpty(sendResult))
      {
        if (_obj.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error)
          _obj.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Error;
        
        if (_obj.ErrorText != sendResult)
          _obj.ErrorText = sendResult;
      }
      else
      {
        if (_obj.ProcessingStatus != KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed)
          _obj.ProcessingStatus = KafkaIntegration.KafkaQueueItem.ProcessingStatus.Completed;
        
        if (!string.IsNullOrEmpty(_obj.ErrorText))
          _obj.ErrorText = string.Empty;
      }
      
      if (_obj.State.IsChanged)
        _obj.Save();
    }
  }
}