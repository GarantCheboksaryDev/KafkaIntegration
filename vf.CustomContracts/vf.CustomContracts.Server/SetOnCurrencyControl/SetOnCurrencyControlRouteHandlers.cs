using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using vf.CustomContracts.SetOnCurrencyControl;

namespace vf.CustomContracts.Server
{
  partial class SetOnCurrencyControlRouteHandlers
  {
    #region Блок №6. Задание на произведение оплаты по договору с предоплатой

    public virtual void StartBlock6(vf.CustomContracts.Server.SimpleAssignmentArguments e)
    {
      var document = _obj.ContractGroup.ContractualDocuments.FirstOrDefault();
      if (document != null)
      {
        e.Block.Subject = vf.CustomContracts.SetOnCurrencyControls.Resources.PaymentAssignmentSubjectFormat(document);
        
        var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(document.BusinessUnit);
        if (contractSettings != null)
        {
          e.Block.RelativeDeadlineDays = contractSettings.ContractPaymentDeadline;
          if (document.RelationshipType != null)
          {
            if (document.RelationshipType.Service == true)
              e.Block.Performers.Add(contractSettings.ServicePaymentEmployee);
            else if (document.RelationshipType.InventoryItem == true)
              e.Block.Performers.Add(contractSettings.InventoryItemsPaymentEmployee);
          }
        }
      }
      
      e.Block.Author = _obj.Author;
    }
    
    #endregion

    #region Блок №7. Условие "Договор с предоплатой?"
    
    public virtual bool Decision7Result()
    {
      var document = _obj.ContractGroup.ContractualDocuments.FirstOrDefault();
      if (document != null)
      {
        var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(document.BusinessUnit);
        if (contractSettings != null)
          return CustomContracts.PaymentTypes.Equals(contractSettings.PrepaymentCheckingCondition, document.PaymentType);
      }
      
      return false;
    }
    
    #endregion
    
    #region Блок №5. Уведомление о выполнении задания постановки на валютный контроль
    
    public virtual void StartBlock5(vf.CustomContracts.Server.FinancialContolNoticeArguments e)
    {
      var financialControlRole = Roles.GetAll(x => Guid.Equals(x.Sid, PublicConstants.Module.Roles.FinancialControlRole)).FirstOrDefault();
      var document = _obj.ContractGroup.ContractualDocuments.FirstOrDefault();
      if (document != null && financialControlRole != null)
      {
        e.Block.Subject = vf.CustomContracts.SetOnCurrencyControls.Resources.Block5SubjectFormat(document);
        e.Block.Performers.Add(financialControlRole);
      }
      
      e.Block.Author = _obj.Author;
    }
    
    #endregion
    
    #region Блок №3. Установка договора на валютный контроль
    
    public virtual void StartBlock3(vf.CustomContracts.Server.SimpleAssignmentArguments e)
    {
      var document = _obj.ContractGroup.ContractualDocuments.FirstOrDefault();

      if (document != null)
      {
        var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(document.BusinessUnit);
        
        e.Block.Subject = Sungero.Docflow.PublicFunctions.Module.CutText(vf.CustomContracts.SetOnCurrencyControls.Resources.Block3SubjectFormat(document), _obj.Info.Properties.Subject.Length);
        
        if (contractSettings != null)
        {
          e.Block.Performers.Add(contractSettings.CurrencyControlVirtualPerformer);
          
          if (contractSettings.HoursToSetOnControl.HasValue)
            e.Block.RelativeDeadlineHours = contractSettings.HoursToSetOnControl.Value;
        }
      }
      
      e.Block.Author = _obj.Author;
    }
    
    #endregion

  }
}