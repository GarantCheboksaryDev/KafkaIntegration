using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractualDocument;

namespace vf.OverrideBaseDev.Shared
{
  partial class ContractualDocumentFunctions
  {
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      SetAdditionalCounterpartyVisibility();
      
      var nonResident = _obj.CounterpartyNonresident == true;
      var properties = _obj.State.Properties;
      var isGrantedFullAccessOrChange = _obj.AccessRights.IsGranted(DefaultAccessRightsTypes.FullAccess, Users.Current) || _obj.AccessRights.IsGranted(DefaultAccessRightsTypes.Change, Users.Current);
      
      properties.ContractUniqueNumber.IsEnabled = nonResident;
      properties.CurrencyControlSetDate.IsEnabled = nonResident;
      properties.CurrencyControlRemoveDate.IsEnabled = nonResident;
      properties.ContractDate.IsEnabled = isGrantedFullAccessOrChange;
      properties.StartDate.IsEnabled = isGrantedFullAccessOrChange;
      properties.EndDate.IsEnabled = isGrantedFullAccessOrChange;
      properties.SignDate.IsEnabled = IsSignDateEnabled();
      properties.Currency.IsRequired = _obj.TotalAmount.HasValue;
      properties.IsStandard.IsEnabled = false;
      properties.ValidFromEndType.IsEnabled = !nonResident || _obj.IsAutomaticValidFromEndType != true;
    }
    
    /// <summary>
    /// Записать в параметры тексты всплывающих подсказок.
    /// </summary>
    /// <param name="paramsDictionary">Словарь параметров.</param>
    [Public]
    public void FillNotificationTexts(System.Collections.Generic.Dictionary<string, object> paramsDictionary)
    {
      if (_obj.DocumentKind != null)
      {
        var controlNotificationSetting = CustomContracts.ControlNotificationSettings.GetAllCached(x => Sungero.Docflow.DocumentTypes.Equals(x.DocumentType, _obj.DocumentKind.DocumentType)).FirstOrDefault();
        if (controlNotificationSetting != null)
        {
          var specialListSetting = controlNotificationSetting.DocumentProperties.Where(x => x.PropertyName == CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.SpecialList).FirstOrDefault();
          if (specialListSetting != null && !string.IsNullOrEmpty(specialListSetting.Notification) && !paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.SpecialList.Value))
            paramsDictionary.Add(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.SpecialList.Value, specialListSetting.Notification);
          
          var contractConclud = controlNotificationSetting.DocumentProperties.Where(x => x.PropertyName == CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.ContractConclud).FirstOrDefault();
          if (contractConclud != null && !string.IsNullOrEmpty(contractConclud.Notification) && !paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.ContractConclud.Value))
            paramsDictionary.Add(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.ContractConclud.Value, contractConclud.Notification);
          
          var bankChargeType = controlNotificationSetting.DocumentProperties.Where(x => x.PropertyName == CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.BankChargeType).FirstOrDefault();
          if (bankChargeType != null && !string.IsNullOrEmpty(bankChargeType.Notification) && !paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.BankChargeType.Value))
            paramsDictionary.Add(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.BankChargeType.Value, bankChargeType.Notification);
          
          var purchasingProce = controlNotificationSetting.DocumentProperties.Where(x => x.PropertyName == CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.PurchasingProce).FirstOrDefault();
          if (purchasingProce != null && !string.IsNullOrEmpty(purchasingProce.Notification) && !paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.PurchasingProce.Value))
            paramsDictionary.Add(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.BankChargeType.Value, purchasingProce.Notification);
          
          var totalSum = controlNotificationSetting.DocumentProperties.Where(x => x.PropertyName == CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TotalSum).FirstOrDefault();
          if (totalSum != null && !string.IsNullOrEmpty(purchasingProce.Notification) && !paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TotalSum.Value))
            paramsDictionary.Add(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TotalSum.Value, totalSum.Notification);
          
          var taxAgent = controlNotificationSetting.DocumentProperties.Where(x => x.PropertyName == CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TaxAgent).FirstOrDefault();
          if (taxAgent != null && !string.IsNullOrEmpty(taxAgent.Notification) && !paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TaxAgent.Value))
            paramsDictionary.Add(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TaxAgent.Value, totalSum.Notification);
        }
      }
    }
    
    /// <summary>
    /// Установить видимость свойства "Передано в СУДК".
    /// </summary>
    public void SetTransferToSudkVisibility()
    {
      _obj.State.Properties.TransferedToSudk.IsVisible = _obj.NeedTransferToSudk == true;
    }
    
    /// <summary>
    /// Доступность и обязательность свойств "Событие" и "Дейтвует по" в зависимости от срока действия.
    /// </summary>
    public void EventValidTillPropertiesEnable(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var prop = _obj.State.Properties;
      var isSupAgreement = vf.OverrideBaseDev.SupAgreements.Is(_obj);
      
      // В свойстве «Срок действия» указано значение, которое указано в справочнике "Настройки модуля" в поле "Окончания договора в рамках события".
      var isEvent = false;
      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsEventNameEnabled, out isEvent))
      {
        isEvent = IsContractEndingEvent();
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsEventNameEnabled, isEvent);
      }
      
      // В свойстве «Срок действия» указано значение, которое указано в справочнике "Настройки модуля" в поле "Окончания договора в рамках даты".
      var isDate = false;
      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsValidTillEnabled, out isDate))
      {
        isDate = IsContractEndingDate();
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsValidTillEnabled, isDate);
      }
      
      // Свойство "Событие" доступно и обязательно для заполнения, если в свойстве «Срок действия» указано значение «Событие».
      prop.EventName.IsEnabled = isEvent;
      prop.EventName.IsRequired = isEvent;
      
      // Свойство "Действует по" доступно и обязательно для заполнения, если в свойстве «Срок действия» указано значение «Дата».
      prop.ValidTill.IsEnabled = isDate;
      prop.ValidTill.IsRequired = !isSupAgreement ? isDate :
        isDate && vf.OverrideBaseDev.SupAgreements.As(_obj).SupAgreementType == vf.OverrideBaseDev.SupAgreement.SupAgreementType.ChangesDetails;
    }
    
    /// <summary>
    /// Установить видимость и обязательность свойства "Доп. контрагент".
    /// </summary>
    public void SetAdditionalCounterpartyVisibility()
    {
      // Видимость, доступность и обязательность свойства "Дополнительный контрагент" в зависимости от свойства "Трехсторонний договор".
      var supAgreement = OverrideBaseDev.SupAgreements.As(_obj);
      var isTripartile = false;
      
      if (supAgreement == null)
      {
        isTripartile = _obj.TripartileContract == true;
        _obj.State.Properties.AdditionalCounterparty.IsVisible = isTripartile;
        _obj.State.Properties.AdditionalCounterparty.IsRequired = isTripartile;
      }
      else
      {
        isTripartile = supAgreement?.LeadingDocument?.TripartileContract == true;
        _obj.State.Properties.AdditionalCounterparty.IsEnabled = isTripartile;
        _obj.State.Properties.AdditionalCounterparty.IsRequired = isTripartile;
      }
    }
    
    /// <summary>
    /// Проверка, является ли "Срок действия" в карточке договора событием окончания договора в рамках даты.
    /// </summary>
    /// <returns>True, если "Срок действия" в карточке договора является событием окончания договора в рамках даты, иначе - false.</returns>
    public bool IsContractEndingDate()
    {
      if (_obj.BusinessUnit == null || _obj.ValidFromEndType == null)
        return false;
      
      var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(_obj.BusinessUnit);
      return SAPIntegration.ContractEndTypes.Equals(contractSettings.ContractEndingDate, _obj.ValidFromEndType);
    }
    
    /// <summary>
    /// Проверка, доступности поля "Дата подписания" в карточке договора.
    /// </summary>
    /// <returns>True, если "Дата подписания" доступно для редактирования, иначе - false</returns>
    public bool IsSignDateEnabled()
    {
      var employeesCurrent = Sungero.Company.Employees.Current;
      
      if (_obj.RegistrationState != vf.OverrideBaseDev.ContractualDocument.RegistrationState.Registered &&
          _obj.InternalApprovalState != vf.OverrideBaseDev.ContractualDocument.InternalApprovalState.Signed &&
          _obj.ExternalApprovalState != vf.OverrideBaseDev.ContractualDocument.ExternalApprovalState.Signed &&
          _obj.LifeCycleState != vf.OverrideBaseDev.ContractualDocument.LifeCycleState.Active)
        return true;
      else if (_obj.RegistrationState == vf.OverrideBaseDev.ContractualDocument.RegistrationState.Registered &&
               _obj.InternalApprovalState == vf.OverrideBaseDev.ContractualDocument.InternalApprovalState.Signed &&
               _obj.ExternalApprovalState == vf.OverrideBaseDev.ContractualDocument.ExternalApprovalState.Signed &&
               _obj.LifeCycleState == vf.OverrideBaseDev.ContractualDocument.LifeCycleState.Active &&
               _obj.DocumentRegister != null && _obj.DocumentRegister.RegistrationGroup != null &&
               _obj.DocumentRegister.RegistrationGroup.RecipientLinks.Any(x => Sungero.CoreEntities.Recipients.Equals(x.Member, employeesCurrent)))
        return true;
      
      return false;
    }
    
    
    /// <summary>
    /// Проверка, является ли "Срок действия" в карточке договора событием окончания договора в рамках события.
    /// </summary>
    /// <returns>True, если "Срок действия" в карточке договора является событием окончания договора в рамках события, иначе - false</returns>
    public bool IsContractEndingEvent()
    {
      if (_obj.BusinessUnit == null || _obj.ValidFromEndType == null)
        return false;
      
      var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(_obj.BusinessUnit);
      return SAPIntegration.ContractEndTypes.Equals(contractSettings.ContractEndingEvent, _obj.ValidFromEndType);
    }
    
    /// <summary>
    /// Изменить отображение панели регистрации.
    /// </summary>
    /// <param name="needShow">Признак отображения.</param>
    /// <param name="repeatRegister">Признак повторной регистрации\изменения реквизитов.</param>
    public override void ChangeRegistrationPaneVisibility(bool needShow, bool repeatRegister)
    {
      base.ChangeRegistrationPaneVisibility(needShow, repeatRegister);
      
      // Есть права на изменение.
      var canUpdate = _obj.AccessRights.CanUpdate();
      
      var properties = _obj.State.Properties;
      properties.ContractDate.IsVisible = needShow;
      properties.SignDate.IsVisible = needShow;
      properties.Created.IsVisible = needShow;
      properties.DeliveryMethod.IsVisible = needShow;
      properties.DeliveryMethod.IsEnabled = false;
    }
    
    /// <summary>
    /// Получить состояние для отправки в SAP.
    /// </summary>
    /// <returns>Код состояния.</returns>
    [Public]
    public virtual string GetSAPState()
    {
      return string.Empty;
    }
    
    /// <summary>
    /// Получить статус для отправки в SAP.
    /// </summary>
    /// <returns>Код статуса.</returns>
    [Public]
    public virtual string GetSAPStatus()
    {
      return string.Empty;
    }
    
    /// <summary>
    /// Проверить необходимость отправки договорного документа в брокер сообщений Kafka.
    /// </summary>
    /// <returns>True - если необходимо отправить, иначе false.</returns>
    /// Функции перекрыты в наследниках.
    public virtual bool NeedSendContractToKafka()
    {
      return false;
    }
  }
}