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
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.PaymentAccount)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForPaymentAccountMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Person)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForPersonMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Voyage)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForVoyageMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.BudgetItem)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForBudgetItemsMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Employee)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForEmployeesMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.Department)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForDepartmentsMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.CompanyStruct)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForCompanyStructureMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.CFOArticle)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForCFOArticleItemMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.RegisterDeprt)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForDepartmentRegisterItemMessage(queueItemsWithError);
        else if (objectEnumeration.Value == IntegrationSettings.ConnectSettingsObjectSettings.ObjectName.OurPaymentAccou)
          KafkaIntegration.PublicFunctions.KafkaQueueItem.CreateAsynchForOurPaymentAccountMessage(queueItemsWithError);
        
        return true;
      }
      
      return false;
    }
  }
}