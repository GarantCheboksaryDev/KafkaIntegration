using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.SupAgreement;

namespace vf.OverrideBaseDev
{
  partial class SupAgreementAdditionalCounterpartySharedHandlers
  {

    public override void AdditionalCounterpartyCounterpartyChanged(vf.OverrideBaseDev.Shared.ContractualDocumentAdditionalCounterpartyCounterpartyChangedEventArgs e)
    {
      base.AdditionalCounterpartyCounterpartyChanged(e);
    }
  }

  partial class SupAgreementSharedHandlers
  {

    public override void PaymentTermsChanged(vf.OverrideBaseDev.Shared.ContractualDocumentPaymentTermsChangedEventArgs e)
    {
      base.PaymentTermsChanged(e);
      
         if (!SAPIntegration.PaymentTermses.Equals(e.NewValue, e.OldValue))
      {
        _obj.State.Properties.PaymentTermAction.IsEnabled = e.NewValue != null;
        _obj.State.Properties.PaymentTermAction.IsRequired = e.NewValue != null;
      }
    }

    public override void ConditionBasePaymentChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.ConditionBasePaymentChanged(e);
      
      if (e.NewValue != e.OldValue)
      {
        _obj.State.Properties.ConditionBasePaymentAction.IsEnabled = !string.IsNullOrEmpty(e.NewValue);
        _obj.State.Properties.ConditionBasePaymentAction.IsRequired = !string.IsNullOrEmpty(e.NewValue);
      }
    }

    public override void ErpDatabookNameChanged(vf.OverrideBaseDev.Shared.ContractualDocumentErpDatabookNameChangedEventArgs e)
    {
      base.ErpDatabookNameChanged(e);
      
      if (!CustomContracts.ErpDatabookNames.Equals(e.NewValue, e.OldValue))
      {
        _obj.State.Properties.ErpDatabookNameAction.IsEnabled = e.NewValue != null;
        _obj.State.Properties.ErpDatabookNameAction.IsRequired = e.NewValue != null;
      }
    }

    public override void BankChargeTypeChanged(vf.OverrideBaseDev.Shared.ContractualDocumentBankChargeTypeChangedEventArgs e)
    {
      base.BankChargeTypeChanged(e);
      
      if (!SAPIntegration.BankChargeTypes.Equals(e.NewValue, e.OldValue))
      {
        _obj.State.Properties.BankChargeTypeAction.IsEnabled = e.NewValue != null;
        _obj.State.Properties.BankChargeTypeAction.IsRequired = e.NewValue != null;
      }
    }

    public override void CounterpartyPaymentAccountChanged(vf.OverrideBaseDev.Shared.ContractualDocumentCounterpartyPaymentAccountChangedEventArgs e)
    {
      base.CounterpartyPaymentAccountChanged(e);
      
      if (!SAPIntegration.PaymentAccounts.Equals(e.NewValue, e.OldValue))
      {
        _obj.State.Properties.CounterpartyPaymentAccountAction.IsEnabled = e.NewValue != null;
        _obj.State.Properties.CounterpartyPaymentAccountAction.IsRequired = e.NewValue != null;
      }
    }

    public override void OwnPaymentAccountChanged(vf.OverrideBaseDev.Shared.ContractualDocumentOwnPaymentAccountChangedEventArgs e)
    {
      base.OwnPaymentAccountChanged(e);
      
      if (!SAPIntegration.PaymentAccounts.Equals(e.NewValue, e.OldValue))
      {
        _obj.State.Properties.OwnPaymentAccountAction.IsEnabled = e.NewValue != null;
        _obj.State.Properties.OwnPaymentAccountAction.IsRequired = e.NewValue != null;
      }
    }

    public override void ProvisionChanged(vf.OverrideBaseDev.Shared.ContractualDocumentProvisionChangedEventArgs e)
    {
      base.ProvisionChanged(e);
      
      if (!SAPIntegration.Provisions.Equals(e.NewValue, e.OldValue))
      {
        _obj.State.Properties.ProvisionAction.IsEnabled = e.NewValue != null;
        _obj.State.Properties.ProvisionAction.IsRequired = e.NewValue != null;
      }
    }

    public override void PaymentCurrenciesChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      base.PaymentCurrenciesChanged(e);
      
      _obj.State.Properties.PaymentCurrenciesAction.IsEnabled = _obj.PaymentCurrencies.Any();
      _obj.State.Properties.PaymentCurrenciesAction.IsRequired = _obj.PaymentCurrencies.Any();
    }

    public override void CurrencyChanged(Sungero.Docflow.Shared.ContractualDocumentBaseCurrencyChangedEventArgs e)
    {
      base.CurrencyChanged(e);
      
      if (!Sungero.Commons.Currencies.Equals(e.NewValue, e.OldValue))
      {
        _obj.State.Properties.TotalAmountAction.IsEnabled = e.NewValue != null;
        _obj.State.Properties.TotalAmountAction.IsRequired = e.NewValue != null;
      }
    }

    public virtual void SupAgreementTypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      // Видимость вкладки "Новые сведения о договоре".
      _obj.State.Pages.NewContractProperties.IsVisible = _obj.SupAgreementType == SupAgreementType.ChangesDetails;
    }

    public override void LeadingDocumentChanged(Sungero.Docflow.Shared.OfficialDocumentLeadingDocumentChangedEventArgs e)
    {
      base.LeadingDocumentChanged(e);
      
      if (!OverrideBaseDev.ContractBases.Equals(e.OldValue, e.NewValue))
      {
        var customContract = OverrideBaseDev.ContractBases.As(e.NewValue);
        if (customContract != null)
        {
          // Свойство "Передавать в СУДК" заполняется автоматически значением из ведущего документа.
          var needTransferToSudk = customContract != null ? customContract.NeedTransferToSudk == true : false;
          if (_obj.NeedTransferToSudk != needTransferToSudk)
            _obj.NeedTransferToSudk = needTransferToSudk;
          
          // Свойство "Доп. контрагент" заполняется автоматически значением из ведущего документа.
          var additionalCounterparty = customContract.AdditionalCounterparty;
          
          _obj.AdditionalCounterparty.Clear();
          foreach (var addCounterparty in additionalCounterparty)
          {
            var newRecord = _obj.AdditionalCounterparty.AddNew();
            newRecord.Counterparty = addCounterparty.Counterparty;
          }
        }
      }
    }

  }
}