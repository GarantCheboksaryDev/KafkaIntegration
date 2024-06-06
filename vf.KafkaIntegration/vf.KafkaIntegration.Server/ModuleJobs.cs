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
    /// Получить информацию о расчетных счетах нашей организации.
    /// </summary>
    public virtual void GetOurPaymentAccountInfoFromKafka()
    {
      var prefix = "GetOurPaymentAccountInfoFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.OurPaymentAccou);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.OurPaymentAccou);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForOurPaymentAccountMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    /// <summary>
    /// Получить информацию о рейсах.
    /// </summary>
    public virtual void GetVoyageInfoFromKafka()
    {
      var prefix = "GetVoyageInfoFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Voyage);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Voyage);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForVoyageMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    /// <summary>
    /// Получить информацию о персонах.
    /// </summary>
    public virtual void GetPeopleInfoFromKafka()
    {
      var prefix = "GetPeopleInfoFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Person);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Person);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForPersonMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    /// <summary>
    /// Получить информацию о сотрудниках.
    /// </summary>
    public virtual void GetEmployeesInfoFromKafka()
    {
      var prefix = "GetEmployeesInfoFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Employee);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Employee);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForEmployeesMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }

    /// <summary>
    /// Получить информацию о статьях бюджета.
    /// </summary>
    public virtual void GetBudgetItemsInfoFromKafka()
    {
      var prefix = "GetBudgetItemsInfoFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.BudgetItem);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.BudgetItem);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForBudgetItemsMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }

    /// <summary>
    /// Получить информацию о расчетных счетах.
    /// </summary>
    public virtual void GetPaymentAccountInfoFromKafka()
    {
      var prefix = "GetPaymentAccountInfoFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.PaymentAccount);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.PaymentAccount);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForPaymentAccountMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }

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
    
    /// <summary>
    /// Получить информацию о подразделениях.
    /// </summary>
    public virtual void GetDepartmentsFromkafka()
    {
      var prefix = "GetDepartmentsFromkafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Department);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Department);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForDepartmentsMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }

    /// <summary>
    /// Получить информацию о ЦФО/МВЗ.
    /// </summary>
    public virtual void GetCompanyStructureFromKafka()
    {
      var prefix = "GetCompanyStructureFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.CompanyStruct);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.CompanyStruct);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForCompanyStructureMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    /// <summary>
    /// Получить регистр связи ЦФО-Статьи БК.
    /// </summary>
    public virtual void GetCFOArticleRegisterFromKafka()
    {
      var prefix = "GetCFOArticleRegisterFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.CFOArticle);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.CFOArticle);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForCFOArticleItemMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
    
    /// <summary>
    /// Получить регистр связи Подразделение-ЦФО-МВЗ-Менеджер.
    /// </summary>
    public virtual void GetDepartmentRegisterFromKafka()
    {
      var prefix = "GetDepartmentRegisterFromKafka. ";
      
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      
      // Прочитать сообщения и занести в очередь сообщений.
      Functions.KafkaQueueItem.GetMessagesAndCreateQueueItem(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.RegisterDeprt);
      
      // Обработать сообщения и занести данные в DirectumRx.
      var messagesQueue = Functions.KafkaQueueItem.GetKaffkaItemQueueForProccess(IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.RegisterDeprt);
      
      Logger.DebugFormat("{0}Количество сообщений к обработке: {1}.", prefix, messagesQueue.Count());
      
      Functions.KafkaQueueItem.CreateAsynchForDepartmentRegisterItemMessage(messagesQueue);
      
      Logger.DebugFormat("{0}Конец процесса.", prefix);
    }
  }
}