using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractualDocument;

namespace vf.OverrideBaseDev
{
  partial class ContractualDocumentSharedHandlers
  {

    public virtual void PaymentTermsChanged(vf.OverrideBaseDev.Shared.ContractualDocumentPaymentTermsChangedEventArgs e)
    {
      if (vf.SAPIntegration.PaymentTermses.Equals(e.NewValue, e.OriginalValue))
      {
        if (e.NewValue != null)
        {
          if (e.NewValue.Postpay == true)
            _obj.PaymentType =  vf.CustomContracts.PublicFunctions.PaymentType.Remote.GetPostOrPrePaymentType(true);
          else if (e.NewValue.Prepayment == true)
            _obj.PaymentType =  vf.CustomContracts.PublicFunctions.PaymentType.Remote.GetPostOrPrePaymentType(false);
          else
            _obj.PaymentType = null;
        }
      }
    }

    public virtual void SignDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (e.NewValue.HasValue)
        _obj.SignDateTime = Calendar.Now;
      else
        _obj.SignDateTime = null;
    }

    public virtual void ContractTypeChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OriginalValue)
        _obj.BKBDR = null;
    }

    public virtual void CounterpartyNonresidentChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OriginalValue)
      {
        if (e.NewValue == true)
        {
          var settings = vf.CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(_obj.BusinessUnit);
          if (settings != null && settings.ContractEndingDate != null && !vf.SAPIntegration.ContractEndTypes.Equals(settings.ContractEndingDate, _obj.ValidFromEndType))
          {
            _obj.ValidFromEndType = settings.ContractEndingDate;
            _obj.IsAutomaticValidFromEndType = true;
          }
        }
      }
      else
        _obj.ValidFromEndType = null;
      
      Functions.ContractualDocument.SetRequiredProperties(_obj);
    }

    public override void CurrencyChanged(Sungero.Docflow.Shared.ContractualDocumentBaseCurrencyChangedEventArgs e)
    {
      base.CurrencyChanged(e);
      
      if (!OverrideBaseDev.Currencies.Equals(e.OldValue, e.NewValue))
      {
        var availableOwnPaymentAccount = SAPIntegration.PublicFunctions.PaymentAccount.Remote.GetAvailablePaymentAccounts(_obj, false);
        var availableCounterpartyPaymentAccount = SAPIntegration.PublicFunctions.PaymentAccount.Remote.GetAvailablePaymentAccounts(_obj, true);
        
        if (e.NewValue == null || !availableOwnPaymentAccount.Any())
          _obj.OwnPaymentAccount = null;
        else if (availableOwnPaymentAccount.Count() == 1)
          _obj.OwnPaymentAccount = availableOwnPaymentAccount.FirstOrDefault();
        
        if (e.NewValue == null || !availableCounterpartyPaymentAccount.Any())
          _obj.CounterpartyPaymentAccount = null;
        else if (availableCounterpartyPaymentAccount.Count() == 1)
          _obj.CounterpartyPaymentAccount = availableCounterpartyPaymentAccount.FirstOrDefault();
      }
    }

    public override void DeliveryMethodChanged(Sungero.Docflow.Shared.OfficialDocumentDeliveryMethodChangedEventArgs e)
    {
      base.DeliveryMethodChanged(e);
      Functions.ContractualDocument.SetRequiredProperties(_obj);
    }

    public virtual void ContractDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
    }

    public virtual void RelationshipTypeChanged(vf.OverrideBaseDev.Shared.ContractualDocumentRelationshipTypeChangedEventArgs e)
    {
      if (!CustomContracts.RelationshipTypes.Equals(e.NewValue, e.OldValue))
      {
        if (e.NewValue != null)
        {
          if (e.NewValue.ContractType == vf.CustomContracts.RelationshipType.ContractType.Income)
            _obj.ContractType = OverrideBaseDev.ContractualDocument.ContractType.Income;
          else if (e.NewValue.ContractType == vf.CustomContracts.RelationshipType.ContractType.Outcome)
            _obj.ContractType = OverrideBaseDev.ContractualDocument.ContractType.Outcome;
          else
            _obj.ContractType = null;
        }
        else
          _obj.ContractType = null;
      }
    }

    public virtual void MVZChanged(vf.OverrideBaseDev.Shared.ContractualDocumentMVZChangedEventArgs e)
    {
      if (!OverrideBaseDev.Departments.Equals(e.NewValue, e.OldValue))
        _obj.MVZCode = e.NewValue != null ? e.NewValue.MVZCode : string.Empty;
    }
        
    public override void DepartmentChanged(Sungero.Docflow.Shared.OfficialDocumentDepartmentChangedEventArgs e)
    {
      base.DepartmentChanged(e);
      
      if (!OverrideBaseDev.Departments.Equals(e.NewValue, e.OldValue))
      {
        _obj.BKBDR = null;
        if (e.NewValue != null)
        {
          var department = vf.OverrideBaseDev.Departments.As(e.NewValue);
          _obj.CFO = department?.CFO;
        }
        else
          _obj.CFO = null;
      }
    }

    public virtual void CounterpartyPaymentAccountChanged(vf.OverrideBaseDev.Shared.ContractualDocumentCounterpartyPaymentAccountChangedEventArgs e)
    {
      if (!OverrideBaseDev.Banks.Equals(e.OldValue, e.NewValue))
      {
        if (e.NewValue != null)
          _obj.CounterpartyBank = e.NewValue.Bank;
        else
          _obj.CounterpartyBank = null;
      }
    }

    public override void OurSigningReasonChanged(Sungero.Docflow.Shared.OfficialDocumentOurSigningReasonChangedEventArgs e)
    {
      base.OurSigningReasonChanged(e);
      
      if (!Sungero.Docflow.SignatureSettings.Equals(e.OldValue, e.NewValue))
      {
        // Обновление параметра "Доверенность подписывающего действительна с <Дата> по <Дата>".
        var hintPOA = CustomContracts.PublicFunctions.Module.Remote.GetPowerOfAttoneyHint(e.NewValue);
        e.Params.AddOrUpdate(CustomContracts.PublicConstants.Module.Params.PowerOfAttoneyHintText, hintPOA);
      }
    }

    public virtual void NeedTransferToSudkChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      Functions.ContractualDocument.SetTransferToSudkVisibility(_obj);
    }

    public override void BusinessUnitChanged(Sungero.Docflow.Shared.OfficialDocumentBusinessUnitChangedEventArgs e)
    {
      base.BusinessUnitChanged(e);
      
      if (!OverrideBaseDev.BusinessUnits.Equals(e.NewValue, e.OldValue))
        // Если поле "Наша организация" очистили, или собственный расчетный счет в карточке договора не соответствует новой нашей организации, то очистить поле "Собственный расчетный счет".
        if (_obj.OwnPaymentAccount != null
            && (e.NewValue == null
                || (_obj.OwnPaymentAccount.BusinessUnits.Any() && !_obj.OwnPaymentAccount.BusinessUnits.Any(b => OverrideBaseDev.BusinessUnits.Equals(b.BusinessUnit, _obj.BusinessUnit)))))
      {
        _obj.OwnPaymentAccount = null;
      }
      else
      {
        var availableOwnPaymentAccount = SAPIntegration.PublicFunctions.PaymentAccount.Remote.GetAvailablePaymentAccounts(_obj, false);
        var availableCounterpartyPaymentAccount = SAPIntegration.PublicFunctions.PaymentAccount.Remote.GetAvailablePaymentAccounts(_obj, true);
        
        if (availableOwnPaymentAccount.Count() == 1)
          _obj.OwnPaymentAccount = availableOwnPaymentAccount.FirstOrDefault();
        
        if (availableCounterpartyPaymentAccount.Count() == 1)
          _obj.CounterpartyPaymentAccount = availableCounterpartyPaymentAccount.FirstOrDefault();
      }
    }

    public override void CounterpartyChanged(Sungero.Docflow.Shared.ContractualDocumentBaseCounterpartyChangedEventArgs e)
    {
      base.CounterpartyChanged(e);
      
      if (!OverrideBaseDev.Counterparties.Equals(e.NewValue, e.OldValue))
      {
        // Если поле "Контрагент" очистили, или расчетный счет контрагента в карточке договора не соответствует новому контрагенту, то очистить поле "Расчетный счет контрагента".
        if (_obj.CounterpartyPaymentAccount != null
            && (e.NewValue == null
                || (_obj.CounterpartyPaymentAccount.Counterparty != null && !OverrideBaseDev.Counterparties.Equals(_obj.CounterpartyPaymentAccount.Counterparty, e.NewValue))))
          _obj.CounterpartyPaymentAccount = null;
        
        // Если поле "Контрагент" очистили, или собственный счет в карточке договора не соответствует новому контрагенту, то очистить поле "Собственный счет".
        if (_obj.OwnPaymentAccount != null
            && (e.NewValue == null
                || (_obj.OwnPaymentAccount.Counterparty != null && !OverrideBaseDev.Counterparties.Equals(_obj.OwnPaymentAccount.Counterparty, e.NewValue))))
          _obj.OwnPaymentAccount = null;
        
        // Заполнение свойств "Результат пров. ОБ", "Результат пров. СПАРК", "Включен в спец список", "Нерезидент" из карточки контрагента.
        if (e.NewValue == null)
        {
          _obj.CheckingResultSB = null;
          _obj.CheckingResultSpark = null;
          _obj.SpecialList = null;
          _obj.CounterpartyNonresident = false;
          _obj.MailForAct = string.Empty;
        }
        else
        {
          var counterparty = OverrideBaseDev.Counterparties.As(e.NewValue);
          if (counterparty != null)
          {
            _obj.MailForAct = counterparty.Email;
            _obj.CheckingResultSpark = counterparty.CheckingResultSPARK;
            _obj.SpecialList = counterparty.SpecialList;
            _obj.CounterpartyNonresident = counterparty.Nonresident;
          }
        }
      }
    }

    public virtual void ValidFromEndTypeChanged(vf.OverrideBaseDev.Shared.ContractualDocumentValidFromEndTypeChangedEventArgs e)
    {
      // Обновление связанных со свойством параметров.
      e.Params.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsEventNameEnabled, Functions.ContractualDocument.IsContractEndingEvent(_obj));
      e.Params.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsValidTillEnabled, Functions.ContractualDocument.IsContractEndingDate(_obj));
      
      // В свойстве «Срок действия» указано значение «Событие».
      var isEvent = Functions.ContractualDocument.IsContractEndingEvent(_obj);
      // В свойстве «Срок действия» указано значение «Дата».
      var isDate = Functions.ContractualDocument.IsContractEndingDate(_obj);
      
      // Очистить соответвующие поля при изменении срока действия.
      if (!isEvent)
        _obj.EventName = null;
      if (!isDate)
        _obj.ValidTill = null;
    }

    public virtual void OwnPaymentAccountChanged(vf.OverrideBaseDev.Shared.ContractualDocumentOwnPaymentAccountChangedEventArgs e)
    {
      if (!OverrideBaseDev.Banks.Equals(e.OldValue, e.NewValue))
      {
        // Собственный банк автоматически заполняется банком, который указан в выбранном значении собственного счета.
        if (e.NewValue != null && !OverrideBaseDev.Banks.Equals(e.NewValue.Bank, _obj.OwnBank))
          _obj.OwnBank = e.NewValue.Bank;
        else if (e.NewValue == null)
          _obj.OwnBank = null;
        
        // Если реквизит «Расчетный счет» заполнен счетом, Валюта которой отличная от «Российский рубль»
        // или в выбранной валюте признак «Условная валюта» заполнен значением «Нет», реквизит «Валюта договора» заполняется автоматически валютой расчетного счета
        var rubCurrency = OverrideBaseDev.PublicFunctions.Currency.Remote.GetDefaultCurrency();
        if (e.NewValue != null && e.NewValue.Currency != null && (OverrideBaseDev.Currencies.Equals(e.NewValue.Currency, rubCurrency)
                                                                  || e.NewValue.Currency.IsConditionalCurrency != true))
          _obj.Currency = e.NewValue.Currency;
      }
    }

    public override void AuthorChanged(Sungero.Content.Shared.ElectronicDocumentAuthorChangedEventArgs e)
    {
      base.AuthorChanged(e);
      
      // Инициатор проекта по умолчанию заполняется автором документа.
      if (!Sungero.Company.Employees.Equals(e.NewValue, e.OldValue))
        _obj.ProjectInitiator = Sungero.Company.Employees.As(e.NewValue);
    }

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      
      // Берем признак Конфиденциально из вида документа.
      var documentKind = vf.OverrideBaseDev.DocumentKinds.As(e.NewValue);
      _obj.IsConfidential = documentKind != null && documentKind.IsConfidential == true;
    }
  }
}