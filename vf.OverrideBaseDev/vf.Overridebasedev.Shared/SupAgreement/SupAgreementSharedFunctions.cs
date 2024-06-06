using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.SupAgreement;

namespace vf.OverrideBaseDev.Shared
{
  partial class SupAgreementFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      // Видимость вкладки "Новые сведения о договоре".
      var isNewContractPropertiesVisible = _obj.SupAgreementType == SupAgreementType.ChangesDetails;
      _obj.State.Pages.NewContractProperties.IsVisible = isNewContractPropertiesVisible;
      
      var prop = _obj.State.Properties;
      
      // Доступность и обязательность свойств "Действие" в зависимости от заполненности соответвтующих им свойств с данными.
      prop.TotalAmountAction.IsEnabled = _obj.TotalAmount != null && isNewContractPropertiesVisible;
      prop.TotalAmountAction.IsRequired = _obj.TotalAmount != null && isNewContractPropertiesVisible;
      
      prop.CurrencyAction.IsEnabled = _obj.Currency != null && isNewContractPropertiesVisible;
      prop.CurrencyAction.IsRequired = _obj.Currency != null && isNewContractPropertiesVisible;
      
      prop.PaymentCurrenciesAction.IsEnabled = _obj.PaymentCurrencies.Any() && isNewContractPropertiesVisible;
      prop.PaymentCurrenciesAction.IsRequired = _obj.PaymentCurrencies.Any() && isNewContractPropertiesVisible;
      
      prop.PaymentTermAction.IsEnabled = _obj.PaymentTerms != null && isNewContractPropertiesVisible;
      prop.PaymentTermAction.IsRequired = _obj.PaymentTerms != null && isNewContractPropertiesVisible;

      prop.ProvisionAction.IsEnabled = _obj.Provision != null && isNewContractPropertiesVisible;
      prop.ProvisionAction.IsRequired = _obj.Provision != null && isNewContractPropertiesVisible;
      
      prop.OwnPaymentAccountAction.IsEnabled = _obj.OwnPaymentAccount != null && isNewContractPropertiesVisible;
      prop.OwnPaymentAccountAction.IsRequired = _obj.OwnPaymentAccount != null && isNewContractPropertiesVisible;
      
      prop.CounterpartyPaymentAccountAction.IsEnabled = _obj.CounterpartyPaymentAccount != null && isNewContractPropertiesVisible;
      prop.CounterpartyPaymentAccountAction.IsRequired = _obj.CounterpartyPaymentAccount != null && isNewContractPropertiesVisible;
      
      prop.BankChargeTypeAction.IsEnabled = _obj.BankChargeType != null && isNewContractPropertiesVisible;
      prop.BankChargeTypeAction.IsRequired = _obj.BankChargeType != null && isNewContractPropertiesVisible;
      
      prop.AdditionalCounterpartyAction.IsEnabled = _obj.AdditionalCounterparty.Any() && isNewContractPropertiesVisible;
      prop.AdditionalCounterpartyAction.IsRequired = _obj.AdditionalCounterparty.Any() && isNewContractPropertiesVisible;
      
      prop.ValidTill.IsRequired = isNewContractPropertiesVisible && IsValidTillRequired();
      prop.ValidTill.IsEnabled = isNewContractPropertiesVisible && IsValidTillRequired();
      
      prop.ErpDatabookNameAction.IsEnabled = _obj.ErpDatabookName != null && isNewContractPropertiesVisible;
      prop.ErpDatabookNameAction.IsRequired = _obj.ErpDatabookName != null && isNewContractPropertiesVisible;
      
      prop.ConditionBasePaymentAction.IsEnabled = !string.IsNullOrEmpty(_obj.ConditionBasePayment) && isNewContractPropertiesVisible;
      prop.ConditionBasePaymentAction.IsRequired = !string.IsNullOrEmpty(_obj.ConditionBasePayment) && isNewContractPropertiesVisible;
    }
    
    
    /// <summary>
    /// Проверка, обязательность полей "Дней для завершения" и "С автопролонгацией".
    /// </summary>
    /// <returns>True, если "Срок действия" в карточке договора является событием окончания договора в рамках даты, иначе - false.</returns>
    public bool IsValidTillRequired()
    {
      if (_obj.BusinessUnit == null || _obj.ValidFromEndType == null)
        return false;
      
      var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(_obj.BusinessUnit);
      if (contractSettings == null)
        return true;
      
      return SAPIntegration.ContractEndTypes.Equals(contractSettings.ContractEndingDate, _obj.ValidFromEndType);
    }
    
    /// <summary>
    /// Получить состояние для отправки в SAP.
    /// </summary>
    /// <returns>Код состояния.</returns>
    [Public]
    public override string GetSAPState()
    {
      if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Draft)
        return Constants.Contracts.ContractualDocument.SAPState.Draft;
      else if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Active)
        return Constants.Contracts.ContractualDocument.SAPState.Active;
      else if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Obsolete || _obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Closed)
        return Constants.Contracts.ContractualDocument.SAPState.NotActive;
      else if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Terminated)
        return Constants.Contracts.ContractualDocument.SAPState.Terminated;
      
      return null;
    }
    
    /// <summary>
    /// Получить статус для отправки в SAP.
    /// </summary>
    /// <returns>Код статуса.</returns>
    [Public]
    public override string GetSAPStatus()
    {
      if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Obsolete)
        return Constants.Contracts.ContractualDocument.SAPStatus.Obsolete;
      else if (_obj.RegistrationState == OverrideBaseDev.ContractBase.RegistrationState.Registered)
        return Constants.Contracts.ContractualDocument.SAPStatus.Registered;
      else if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Draft && !_obj.InternalApprovalState.HasValue)
        return Constants.Contracts.ContractualDocument.SAPStatus.Draft;
      else if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.SupAgreement.InternalApprovalState.OnApproval)
        return Constants.Contracts.ContractualDocument.SAPStatus.OnApproval;
      else if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.SupAgreement.InternalApprovalState.OnRework)
        return Constants.Contracts.ContractualDocument.SAPStatus.OnRework;
      else if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.SupAgreement.InternalApprovalState.PendingSign)
        return Constants.Contracts.ContractualDocument.SAPStatus.OnSigned;
      else if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.SupAgreement.InternalApprovalState.Signed)
        return Constants.Contracts.ContractualDocument.SAPStatus.Signed;
      else if (_obj.LifeCycleState == OverrideBaseDev.SupAgreement.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.SupAgreement.InternalApprovalState.Aborted)
        return Constants.Contracts.ContractualDocument.SAPStatus.Canceled;
      
      return null;
    }
    
    /// <summary>
    /// Проверить необходимость отправки договорного документа в брокер сообщений Kafka.
    /// </summary>
    /// <returns>True - если необходимо отправить, иначе false.</returns>
    public override bool NeedSendContractToKafka()
    {
      return _obj.State.Properties.LifeCycleState.IsChanged && _obj.State.Properties.LifeCycleState.OriginalValue != _obj.LifeCycleState
        && (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Active
            || _obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Closed
            || _obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Terminated
            || _obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Obsolete)
        || _obj.State.Properties.ResponsibleEmployee.IsChanged && !OverrideBaseDev.Employees.Equals(_obj.State.Properties.ResponsibleEmployee.OriginalValue, _obj.ResponsibleEmployee)
        || _obj.State.Properties.ContractDate.IsChanged &&_obj.State.Properties.ContractDate.OriginalValue != _obj.ContractDate
        || _obj.State.Properties.RegistrationNumber.IsChanged &&_obj.State.Properties.RegistrationNumber.OriginalValue != _obj.RegistrationNumber
        || _obj.State.Properties.RegistrationDate.IsChanged &&_obj.State.Properties.RegistrationDate.OriginalValue != _obj.RegistrationDate
        || _obj.State.Properties.CounterpartyRegistrationNumber.IsChanged &&_obj.State.Properties.CounterpartyRegistrationNumber.OriginalValue != _obj.CounterpartyRegistrationNumber;
    }
  }
}