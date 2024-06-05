using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractBase;

namespace vf.OverrideBaseDev.Shared
{
  partial class ContractBaseFunctions
  {
    /// <summary>
    /// Признак доступности свойства "Установлен на валютный контроль".
    /// </summary>
    public bool IsCurrencyControlRemoveDateEnable()
    {
      var сurrencyControlRole = vf.CustomContracts.PublicInitializationFunctions.Module.GetCurrencyControlRole();
      if (сurrencyControlRole == null)
        return false;
      
      var activeSubstitutedUsers = Substitutions.ActiveSubstitutedUsers;
      if (!activeSubstitutedUsers.Any())
        return false;
      
      if (Users.Current.IncludedIn(сurrencyControlRole) ||
          сurrencyControlRole.RecipientLinks.Any(x => activeSubstitutedUsers.Contains(x.Member)))
        return true;
      
      return false;
    }
    
    public override void SetRequiredProperties()
    {
      base.SetRequiredProperties();
      
      var isAdmin = IntegrationSettings.PublicFunctions.Module.CheckCurrentUserIsAdmin();
      var isAutomaticRenewal = _obj.IsAutomaticRenewal == true;
      var properties = _obj.State.Properties;
      var nonResident = _obj.CounterpartyNonresident == true;
      var isCurrencyControlRemoveDateEnable = IsCurrencyControlRemoveDateEnable();
      var isActive = _obj.LifeCycleState == vf.OverrideBaseDev.ContractBase.LifeCycleState.Active;
      var isExternalApprovalSigned = _obj.ExternalApprovalState == ExternalApprovalState.Signed;
      var isRegistered = _obj.RegistrationState == RegistrationState.Registered;
      var isInternalApprovalSigned = _obj.InternalApprovalState == InternalApprovalState.Signed;
      var isClosed = _obj.LifeCycleState == LifeCycleState.Closed;
      var isTerminated = _obj.LifeCycleState == LifeCycleState.Terminated;
      var isVisbleAllUsers = isRegistered && isInternalApprovalSigned && isExternalApprovalSigned && (isActive || isClosed || isTerminated);
      var isVisbileMVZ = _obj.BKBDR != null ? _obj.BKBDR.ArticleSegment == vf.CustomContracts.BKBDR.ArticleSegment.Flot || _obj.BKBDR.ArticleSegment == vf.CustomContracts.BKBDR.ArticleSegment.Both : false;
      
      properties.CounterpartyNonresident.IsEnabled = false;
      properties.ValidTill.IsRequired = IsValidTillRequired() || isAutomaticRenewal;
      properties.DaysToFinishWorks.IsRequired = isAutomaticRenewal;
      properties.CurrencyControl.IsVisible = nonResident && isVisbleAllUsers;
      properties.ContractUniqueNumber.IsEnabled = isCurrencyControlRemoveDateEnable && isVisbleAllUsers;
      properties.CurrencyControlSetDate.IsEnabled = isCurrencyControlRemoveDateEnable && isVisbleAllUsers;
      properties.CurrencyControlRemoveDate.IsEnabled = isCurrencyControlRemoveDateEnable && isVisbleAllUsers;
      properties.CurrencyControl.IsVisible = nonResident && isActive;
      properties.CurrencyControlRemoveDate.IsVisible = nonResident && isActive;
      properties.CurrencyControlSetDate.IsVisible = nonResident && isActive;
      properties.ContractUniqueNumber.IsVisible = nonResident && isActive;
      properties.CurrencyControlSetDate.IsVisible = nonResident && isActive;
      properties.TotalSum.IsVisible = isVisbleAllUsers;
      properties.MVZ.IsVisible = isVisbileMVZ;
      properties.BKBDR.IsVisible = _obj.TotalAmount.HasValue;
      // Сделать недоступными поля  группы полей "Условия", если состояние документа = "Действующий".
      properties.RelationshipType.IsEnabled = !isActive;
      properties.ContractKind.IsEnabled = !isActive;
      properties.Offer.IsEnabled = !isActive;
      properties.MovementGoodsAcrossBorder.IsEnabled = !isActive;
      properties.ContractConcludingBasis.IsEnabled = !isActive;
      properties.ValidFrom.IsEnabled = !isActive;
      properties.ValidFromEndType.IsEnabled = !isActive;
      properties.EventName.IsEnabled = !isActive;
      properties.ValidTill.IsEnabled = !isActive;
      properties.IsAutomaticRenewal.IsEnabled = !isActive;
      properties.StartDate.IsEnabled = !isActive;
      properties.EndDate.IsEnabled = !isActive;
      properties.TotalAmount.IsEnabled = !isActive;
      properties.Currency.IsEnabled = !isActive;
      properties.OwnPaymentAccount.IsEnabled = !isActive;
      properties.VatRate.IsEnabled = !isActive;
      properties.VatAmount.IsEnabled = !isActive;
      properties.PurchasingProcedyreType.IsEnabled = !isActive;
      properties.VatComment.IsEnabled = !isActive;
      properties.Provision.IsEnabled = !isActive;
      properties.TotalSum.IsEnabled = !isActive;
      properties.IncotermsKind.IsEnabled = !isActive;
      properties.PaymentTerms.IsEnabled = !isActive;
      properties.PaymentCurrencies.IsEnabled = !isActive;
      properties.BankChargeType.IsEnabled = !isActive;
      properties.AdvanceClosedDate.IsEnabled = !isActive;
      properties.ConditionBasePayment.IsEnabled = !isActive;
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
      var isActive = _obj.LifeCycleState == vf.OverrideBaseDev.ContractBase.LifeCycleState.Active;
      var isExternalApprovalSigned = _obj.ExternalApprovalState == ExternalApprovalState.Signed;
      var isRegistered = _obj.RegistrationState == RegistrationState.Registered;
      var isInternalApprovalSigned = _obj.InternalApprovalState == InternalApprovalState.Signed;
      var isClosed = _obj.LifeCycleState == LifeCycleState.Closed;
      var isTerminated = _obj.LifeCycleState == LifeCycleState.Terminated;
      var isVisbleAllUsers = isRegistered && isInternalApprovalSigned && isExternalApprovalSigned && (isActive || isClosed || isTerminated);
      
      var properties = _obj.State.Properties;
      // Доступно для просмотра всем пользователям.
      properties.LifeCycleState.IsVisible = false;
      properties.InternalApprovalState.IsVisible = needShow && isVisbleAllUsers;
      properties.InternalApprovalState.IsVisible = needShow && isVisbleAllUsers;
      properties.RegistrationState.IsVisible = needShow && isVisbleAllUsers;
      properties.PaperCount.IsVisible = needShow && isVisbleAllUsers;
      properties.StoredIn.IsVisible = needShow && isVisbleAllUsers;
      properties.PlacedToCaseFileDate.IsVisible = needShow && isVisbleAllUsers;
      properties.SignDate.IsVisible = needShow && (isVisbleAllUsers || IsVisibleForApprovalCheckReturnAssignmentRequiredPerformer());
      properties.DocumentRegister.IsVisible = needShow && isVisbleAllUsers;
    }
    
    /// <summary>
    /// Проверка, видимости поля для исполнителя задания Контроль возврат.
    /// </summary>
    /// <returns>True, если поле доступно, иначе - False.</returns>
    public bool IsVisibleForApprovalCheckReturnAssignmentRequiredPerformer()
    {
      var approvalCheckReturnAssignment = vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.GetInProcessApprovalCheckReturnAssignmentByDocument(_obj);
      if (approvalCheckReturnAssignment == null)
        return false;
      
      return Users.Equals(approvalCheckReturnAssignment.Performer, Users.Current);
    }
    
    /// <summary>
    /// Проверка, видимости поля для исполнителя задания на согласование.
    /// </summary>
    /// <returns>True, если поле доступно, иначе - False.</returns>
    public bool IsVisibleForApprovalAssignmentPerformer()
    {
      var approvalAssignment = vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.GetInProcessApprovalAssignmentByDocument(_obj);
      if (approvalAssignment == null)
        return false;
      
      return Users.Equals(approvalAssignment.Performer, Users.Current) && vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.IncludedInRole(vf.CustomContracts.PublicConstants.Module.Roles.MethodologistCentralResearchCenterRole);
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
    ///  Установить доступность поля "№ контрагента".
    /// </summary>
    public virtual void SetCounterpartyRegistrationNumberEnabled(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isInRegistrationGroup = false;
      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.CurrentUserIsInRegistrationGroup, out isInRegistrationGroup))
      {
        isInRegistrationGroup = Functions.ContractBase.Remote.CheckCurrentUserIsInRegistrationGroup(_obj);
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.CurrentUserIsInRegistrationGroup, isInRegistrationGroup);
      }
      
      _obj.State.Properties.CounterpartyRegistrationNumber.IsEnabled = (_obj.IsProgramCounterpartyNumber == true
                                                                        && (isInRegistrationGroup)) || _obj.IsProgramCounterpartyNumber != true;
    }
    
    /// <summary>
    ///  Установить доступность полей "Дней для завершения" и "С автопролонгацией".
    /// </summary>
    public virtual void SetDaysToFinishWorksControlEnabled(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isEnabled = false;

      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsSetDaysToFinishWorksControlEnabled, out isEnabled))
      {
        isEnabled = CompareContractEndTypes();
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsCurrencyControlEnabled, isEnabled);
      }
      
      var properties = _obj.State.Properties;
      properties.DaysToFinishWorks.IsEnabled = isEnabled;
      properties.IsAutomaticRenewal.IsEnabled = isEnabled;
    }
    
    /// <summary>
    ///  Установить обязательность полей "Дней для завершения" и "Действует по".
    /// </summary>
    public virtual void SetDaysToFinishWorksControlRequired(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isRequired = false;

      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsSetDaysToFinishWorksControlRequired, out isRequired))
      {
        isRequired = _obj.IsAutomaticRenewal == true || IsContractEndingEvent();
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsSetDaysToFinishWorksControlRequired, isRequired);
      }
      
      var properties = _obj.State.Properties;
      properties.DaysToFinishWorks.IsRequired = isRequired;
      properties.ValidTill.IsRequired = isRequired;
    }
    
    /// <summary>
    /// Сравнить значение поля "Окончание договора в рамках даты" из настроек договора с значением "Срок действия".
    /// </summary>
    /// <returns>True, если значения совпадают, иначе - false.</returns>
    public virtual bool CompareContractEndTypes()
    {
      var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(_obj.BusinessUnit);
      if (contractSettings == null)
        return false;

      return vf.SAPIntegration.ContractEndTypes.Equals(contractSettings.ContractEndingDate, _obj.ValidFromEndType);
    }
    
    /// <summary>
    /// Установить доступность полей "Установлен на валютный контроль" и "УНК".
    /// </summary>
    public void SetCurrecyControlEnabled(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isEnabled = false;
      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsCurrencyControlEnabled, out isEnabled))
      {
        isEnabled = OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.UserOrSubstitutionIncludedInRole(CustomContracts.PublicConstants.Module.Roles.CurrencyControlRole);
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsCurrencyControlEnabled, isEnabled);
      }
      
      _obj.State.Properties.CurrencyControl.IsEnabled = isEnabled;
      _obj.State.Properties.ContractUniqueNumber.IsEnabled = isEnabled;
    }
    
    /// <summary>
    /// Установить доступность и обязательность поля Рейс.
    /// </summary>
    public void SetVoyageEnabled(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isEnabled = false;
      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsVoyageEnabled, out isEnabled))
      {
        isEnabled = IsVoyageEnabled();
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsVoyageEnabled, isEnabled);
      }
      
      _obj.State.Properties.Voyages.IsEnabled = isEnabled;
      _obj.State.Properties.Voyages.IsRequired = isEnabled;
    }
    
    /// <summary>
    /// Проверка доступности и обязательности поля "Рейс" в карточке договора.
    /// </summary>
    /// <returns>True, если поле можно редактировать, иначе - false.</returns>
    public bool IsVoyageEnabled()
    {
      if (_obj.BusinessUnit == null || _obj.DocumentKind == null)
        return false;
      
      var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(_obj.BusinessUnit);
      return contractSettings.VoyageDocumentKinds.Any(x => OverrideBaseDev.DocumentKinds.Equals(x.VoyageDocumentKind, _obj.DocumentKind));
    }
    
    /// <summary>
    /// Установить доступность поля Исполнитель по договору.
    /// </summary>
    public void SetResponsibleEmployeerEnable(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isEnabled = false;
      
      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsAdministrator, out isEnabled))
      {
        isEnabled = Sungero.Docflow.PublicFunctions.Module.Remote.IsAdministrator();
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsAdministrator, isEnabled);
      }
      
      _obj.State.Properties.ResponsibleEmployee.IsEnabled = isEnabled;
    }
    
    /// <summary>
    /// Установить доступность поля Ответственный по договору.
    /// </summary>
    public void SetProjectInitiatorEnable(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isEnabled = false;
      
      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsProjectInitiatorEnabled, out isEnabled))
      {
        isEnabled = IsIsProjectInitiatorEnabled();
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsProjectInitiatorEnabled, isEnabled);
      }
      
      _obj.State.Properties.ProjectInitiator.IsEnabled = isEnabled;
    }
    
    /// <summary>
    /// Установить доступность поля Налоговый агент.
    /// </summary>
    public void SetTaxAgentVisible(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isVisible = false;
      
      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsTaxAgentEnabled, out isVisible))
      {
        isVisible = IsTaxAgentVisible();
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsTaxAgentEnabled, isVisible);
      }
      
      _obj.State.Properties.TaxAgent.IsVisible = isVisible;
    }
    
    /// <summary>
    /// Установить доступность поля Инкотермс.
    /// </summary>
    public void SetIncotermsEnabled(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isEnabled = false;
      
      if (!paramsDictionary.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.IsIncotermsEnabled, out isEnabled))
      {
        isEnabled = IsIncotermsEnabled();
        paramsDictionary.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.IsIncotermsEnabled, isEnabled);
      }
      
      _obj.State.Properties.IncotermsKind.IsEnabled = isEnabled;
    }
    
    /// <summary>
    /// Установить или очистить регистрационный номер контрагента.
    /// </summary>
    public void SetCounterpartyRegistrationNumber()
    {
      // Присвоить или изменить номер при регистрации или изменении реквизитов.
      if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Active
          && (
            string.IsNullOrEmpty(_obj.CounterpartyRegistrationNumber)
            || !string.IsNullOrEmpty(_obj.CounterpartyRegistrationNumber) && _obj.IsProgramCounterpartyNumber == true
           )
          && _obj.CounterpartyRegistrationNumber != _obj.RegistrationNumber
          && !string.IsNullOrEmpty(_obj.RegistrationNumber)
          && _obj.InternalApprovalState == OverrideBaseDev.ContractBase.InternalApprovalState.Signed
          && _obj.ExternalApprovalState == OverrideBaseDev.ContractBase.ExternalApprovalState.Signed)
      {
        _obj.CounterpartyRegistrationNumber = _obj.RegistrationNumber;
        _obj.IsProgramCounterpartyNumber = true;
      }
      else if (string.IsNullOrEmpty(_obj.RegistrationNumber) && _obj.IsProgramCounterpartyNumber == true && !string.IsNullOrEmpty(_obj.CounterpartyRegistrationNumber))
      {
        // Удалить номер контрагента при отмене регистрации.
        _obj.CounterpartyRegistrationNumber = string.Empty;
        _obj.IsProgramCounterpartyNumber = false;
      }
    }
    
    /// <summary>
    /// Проверка досутпности поля "Налоговый агент" в карточке договора.
    /// </summary>
    /// <returns>True, если доступно, иначе - false.</returns>
    public bool IsTaxAgentVisible()
    {
      var isActive = _obj.LifeCycleState == vf.OverrideBaseDev.ContractBase.LifeCycleState.Active;
      var isExternalApprovalSigned = _obj.ExternalApprovalState == ExternalApprovalState.Signed;
      var isRegistered = _obj.RegistrationState == RegistrationState.Registered;
      var isInternalApprovalSigned = _obj.InternalApprovalState == InternalApprovalState.Signed;
      var isClosed = _obj.LifeCycleState == LifeCycleState.Closed;
      var isTerminated = _obj.LifeCycleState == LifeCycleState.Terminated;
      var isVisbleAllUsers = isRegistered && isInternalApprovalSigned && isExternalApprovalSigned && (isActive || isClosed || isTerminated);
      
      return isVisbleAllUsers && _obj.CounterpartyNonresident == true || IsVisibleForApprovalAssignmentPerformer();
    }
    
    /// <summary>
    /// Проверка досутпности поля "Ответственный по договору" в карточке договора.
    /// </summary>
    /// <returns>True, если доступно, иначе - false.</returns>
    public bool IsIsProjectInitiatorEnabled()
    {
      return (_obj.InternalApprovalState == InternalApprovalState.OnRework || _obj.InternalApprovalState == InternalApprovalState.Aborted) &&
        (_obj.AccessRights.IsGranted(DefaultAccessRightsTypes.Change, Users.Current) || _obj.AccessRights.IsGranted(DefaultAccessRightsTypes.FullAccess, Users.Current)) ||
        Sungero.Docflow.PublicFunctions.Module.Remote.IsAdministrator();
    }
    
    /// <summary>
    /// Проверка возможности редактирования поля "Инкотермс" в карточке договора.
    /// </summary>
    /// <returns>True, если поле можно редактировать, иначе - false.</returns>
    public bool IsIncotermsEnabled()
    {
      if (_obj.BusinessUnit == null || _obj.RelationshipType == null)
        return false;
      
      var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(_obj.BusinessUnit);
      return CustomContracts.RelationshipTypes.Equals(contractSettings.SupplierRelationshipType, _obj.RelationshipType);
    }
    
    /// <summary>
    /// Проверка необходимости заполнения свойства "Передавать в СУДК".
    /// </summary>
    /// <returns>True, если найдена запись справочника СУДК с совпадающим значением поля "Вид договора", и "Наша организация", иначе - False.</returns>
    public virtual bool NeedsTransferToSudk()
    {
      if (_obj.ContractKind == null || _obj.BusinessUnit == null)
        return false;
      
      var contractSettings = CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(_obj.BusinessUnit);
      return contractSettings.SudkContractKinds.Any(x => CustomContracts.ContractKinds.Equals(x.ContractKind, _obj.ContractKind));
    }
    
    /// <summary>
    /// Получить состояние для отправки в SAP.
    /// </summary>
    /// <returns>Код состояния.</returns>
    [Public]
    public override string GetSAPState()
    {
      if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Draft)
        return Constants.Contracts.ContractualDocument.SAPState.Draft;
      else if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Active)
        return Constants.Contracts.ContractualDocument.SAPState.Active;
      else if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Obsolete || _obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Closed)
        return Constants.Contracts.ContractualDocument.SAPState.NotActive;
      else if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Terminated)
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
      if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Obsolete)
        return Constants.Contracts.ContractualDocument.SAPStatus.Obsolete;
      else if (_obj.RegistrationState == OverrideBaseDev.ContractBase.RegistrationState.Registered)
        return Constants.Contracts.ContractualDocument.SAPStatus.Registered;
      else if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Draft && !_obj.InternalApprovalState.HasValue)
        return Constants.Contracts.ContractualDocument.SAPStatus.Draft;
      else if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.ContractBase.InternalApprovalState.OnApproval)
        return Constants.Contracts.ContractualDocument.SAPStatus.OnApproval;
      else if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.ContractBase.InternalApprovalState.OnRework)
        return Constants.Contracts.ContractualDocument.SAPStatus.OnRework;
      else if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.ContractBase.InternalApprovalState.PendingSign)
        return Constants.Contracts.ContractualDocument.SAPStatus.OnSigned;
      else if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.ContractBase.InternalApprovalState.Signed)
        return Constants.Contracts.ContractualDocument.SAPStatus.Signed;
      else if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Draft && _obj.InternalApprovalState == OverrideBaseDev.ContractBase.InternalApprovalState.Aborted)
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