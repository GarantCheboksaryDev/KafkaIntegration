using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractualDocument;

namespace vf.OverrideBaseDev
{
  partial class ContractualDocumentCurrencyPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> CurrencyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.CurrencyFiltering(query, e);
      
      // Если реквизит «Расчетный счет» заполнен счетом, в валюте которой указана валюта – «Российский рубль»,
      // то в реквизите «Валюта договора» для выбора доступна валюта «Российский рубль» и валюты, в которых признак «Условная валюта» проставлен значением «Да».
      var rubCurrency = OverrideBaseDev.PublicFunctions.Currency.Remote.GetDefaultCurrency();
      if (_obj.OwnPaymentAccount != null && _obj.OwnPaymentAccount.Currency != null && OverrideBaseDev.Currencies.Equals(_obj.OwnPaymentAccount.Currency, rubCurrency))
        query = query.Where(x => x.AlphaCode == CustomContracts.PublicConstants.Module.RubAlphaCode || OverrideBaseDev.Currencies.As(x).IsConditionalCurrency == true);
      
      return query;
    }
  }

  partial class ContractualDocumentVoyagesVoyagePropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> VoyagesVoyageFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(x => x.Status == vf.CustomContracts.Voyage.Status.Active);
    }
  }




  partial class ContractualDocumentDepartmentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> DepartmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return CustomContracts.PublicFunctions.CFO.GetAllDepartment().Cast<T>();
    }
  }

  partial class ContractualDocumentBKBDRPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> BKBDRFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(x => _obj.ContractType != null && x.ContractKind == _obj.ContractType
                         && !query.Any(b => CustomContracts.BKBDRs.Equals(b.Level1Article, x))
                         && !query.Any(b => CustomContracts.BKBDRs.Equals(b.Level2Article, x))
                         && !query.Any(b => CustomContracts.BKBDRs.Equals(b.Level3Article, x)));
    }
  }

  partial class ContractualDocumentMVZPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> MVZFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      return query.Where(x => x.MVZCode != null && x.MVZCode != string.Empty) ;
    }
  }

  partial class ContractualDocumentOwnPaymentAccountPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> OwnPaymentAccountFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Если в карточке договора не заполнено свойство "Наша организация", то собственные расчтеные счета недоступны.
      if (_obj.BusinessUnit == null)
        return query.Where(x => x.Id == -1);
      
      query = SAPIntegration.PublicFunctions.PaymentAccount.Remote.GetAvailablePaymentAccounts(_obj, false).Cast<T>();
      
      return query;
    }
  }

  partial class ContractualDocumentCounterpartyPaymentAccountPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> CounterpartyPaymentAccountFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      // Если "Наша организация" не заполнена, то расчетные счета недоступны.
      if (_obj.BusinessUnit == null)
        return query.Where(x => x.Id == -1);
      
      // Свойство «Наша организация» из карточки договора соответствует значению из записи справочника расчетного счета.
      query = SAPIntegration.PublicFunctions.PaymentAccount.Remote.GetAvailablePaymentAccounts(_obj, true).Cast<T>();
      
      return query;
    }
  }

  partial class ContractualDocumentCounterpartyPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> CounterpartyFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.CounterpartyFiltering(query, e);
      
      query = OverrideBaseDev.PublicFunctions.Counterparty.GetAvailableCounterparty(query).Cast<T>();
      
      return query;
    }
  }


  partial class ContractualDocumentServerHandlers
  {

    public override void AfterSave(Sungero.Domain.AfterSaveEventArgs e)
    {
      base.AfterSave(e);
      
      #region Отправить данные в брокер сообщений Kafka.
      if (Functions.ContractualDocument.NeedSendContractToKafka(_obj))
        Functions.ContractualDocument.CreateSendContractAsycnh(_obj);
      #endregion
    }

    public override void BeforeSaveHistory(Sungero.Content.DocumentHistoryEventArgs e)
    {
      base.BeforeSaveHistory(e);
      
      var convertBoolInString = new Dictionary<bool?, string>()
      {
        { true, vf.OverrideBaseDev.ContractualDocuments.Resources.YesValue},
        { false, vf.OverrideBaseDev.ContractualDocuments.Resources.NoValue}
      };
      
      object templateId = 0;
      
      object fromLastVersionNumber = 0;
      
      var exEntity = (Sungero.Domain.Shared.IExtendedEntity)_obj;

      var created = new Enumeration(Constants.Contracts.ContractualDocument.HistoryOperation.Create);
      
      if ((e.Operation == Sungero.Content.DocumentHistory.Operation.CreateVersion
           || e.Operation == Sungero.Content.DocumentHistory.Operation.UpdateVersion
           || e.Operation == Sungero.Content.DocumentHistory.Operation.UpdateVerBody
           || e.Operation == Sungero.Content.DocumentHistory.Operation.Import
           || e.Operation == Sungero.Content.DocumentHistory.Operation.DeleteVersion
           || e.Action == created)
          && e.VersionNumber.HasValue)
      {
        if (exEntity.Params.TryGetValue(Sungero.Content.Shared.ElectronicDocumentUtils.FromTemplateIdKey, out templateId))
          e.Comment = vf.OverrideBaseDev.ContractualDocuments.Resources.FromTemplateIdKeyFormat(e.Comment, Constants.Contracts.ContractualDocument.HistoryOperation.ID, templateId);
        
        if (exEntity.Params.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.FromLastVersionNumber, out fromLastVersionNumber))
        {
          var lastVersionTemplateId = Functions.ContractualDocument.GetTemplateIdFromHistory(_obj, int.Parse(fromLastVersionNumber.ToString()));
          if (lastVersionTemplateId > 0)
            e.Comment = vf.OverrideBaseDev.ContractualDocuments.Resources.CreateFromTemplateFromLastVersionFormat(Constants.Contracts.ContractualDocument.HistoryOperation.ID, lastVersionTemplateId);
        }

        
        if (!_obj.State.IsInserted && _obj.State.Properties.IsStandard.OriginalValue != _obj.IsStandard)
        {
          var isStandardString = string.Empty;
          var oldIsStandardString = string.Empty;
          
          convertBoolInString.TryGetValue(_obj.State.Properties.IsStandard.OriginalValue, out oldIsStandardString);
          convertBoolInString.TryGetValue(_obj.IsStandard, out isStandardString);
          
          e.Write(new Enumeration(Constants.Contracts.ContractualDocument.HistoryOperation.SDChange),
                  new Enumeration(Constants.Contracts.ContractualDocument.HistoryOperation.SDChange),
                  vf.OverrideBaseDev.Counterparties.Resources.ChangePropertyValueFormat(_obj.Info.Properties.IsStandard.LocalizedName,
                                                                                        oldIsStandardString,
                                                                                        isStandardString
                                                                                       ));
        }
      }
    }

    public override void BeforeSigning(Sungero.Domain.BeforeSigningEventArgs e)
    {
      base.BeforeSigning(e);
      
      if (_obj.IsOriginalContractAvailable != true)
      {
        var isOriginalContractAvailable = vf.CustomContracts.PublicFunctions.Module.IsOriginalContractAvailable(_obj, e.Signature);
        if (_obj.IsOriginalContractAvailable != isOriginalContractAvailable)
          _obj.IsOriginalContractAvailable = isOriginalContractAvailable;
      }
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      var properties = _obj.State.Properties;
      
      #region Заполнить историю создания версии из шаблона, если проводились операции с версиями.
      
      if (properties.Versions.IsChanged)
      {
        long templateId = 0;
        
        var isStandard = false;
        
        var lastVersion = _obj.LastVersion != null && _obj.LastVersion.Number.HasValue ? _obj.LastVersion.Number.Value : 0;
        
        if (e.Params.Contains(Sungero.Content.Shared.ElectronicDocumentUtils.FromTemplateIdKey))
          e.Params.TryGetValue(Sungero.Content.Shared.ElectronicDocumentUtils.FromTemplateIdKey, out templateId);
        else if (lastVersion > 0)
        {
          if (e.Params.Contains(Constants.Contracts.ContractualDocument.ParamNames.FromLastVersionNumber))
            e.Params.TryGetValue(Constants.Contracts.ContractualDocument.ParamNames.FromLastVersionNumber, out lastVersion);
          
          templateId = Functions.ContractualDocument.GetTemplateIdFromHistory(_obj, lastVersion);
        }
        
        if (templateId > 0)
        {
          var documentTemplate = OverrideBaseDev.PublicFunctions.DocumentTemplate.Remote.GetTemplateById(templateId.ToString());
          
          if (documentTemplate != null)
            isStandard = documentTemplate.IsStandardTemplate == true;
        }
        
        if (_obj.IsStandard != isStandard)
          _obj.IsStandard = isStandard;
      }
      
      #endregion
      
      base.BeforeSave(e);
      
      if (_obj.IsOriginalContractAvailable != true)
      {
        var isOriginalContractAvailable = vf.CustomContracts.PublicFunctions.Module.IsOriginalContractAvailable(_obj);
        if (_obj.IsOriginalContractAvailable != isOriginalContractAvailable)
          _obj.IsOriginalContractAvailable = isOriginalContractAvailable;
      }
      
      if (_obj.RegistrationState == RegistrationState.Registered &&
          _obj.ExternalApprovalState == ExternalApprovalState.Signed &&
          _obj.InternalApprovalState == vf.OverrideBaseDev.ContractualDocument.InternalApprovalState.Signed &&
          _obj.LifeCycleState == vf.OverrideBaseDev.ContractualDocument.LifeCycleState.Active)
      {
        if (_obj.DeliveryMethod == null || _obj.DeliveryMethod != null &&  _obj.DeliveryMethod.Sid != vf.CustomContracts.PublicConstants.Module.MailDeliveryMethod.Exchange &&
            !_obj.SignDate.HasValue)
          _obj.SignDate = _obj.ContractDate;
        
        if (_obj.TripartileContract == true)
          _obj.IsForceMajeure = true;
        else
          _obj.IsForceMajeure = false;
      }
      else if (_obj.RegistrationState == vf.OverrideBaseDev.ContractualDocument.RegistrationState.Registered &&
               _obj.ExternalApprovalState == vf.OverrideBaseDev.ContractualDocument.ExternalApprovalState.Signed &&
               _obj.InternalApprovalState == vf.OverrideBaseDev.ContractualDocument.InternalApprovalState.Signed &&
               _obj.LifeCycleState == vf.OverrideBaseDev.ContractualDocument.LifeCycleState.Active &&
               _obj.DeliveryMethod != null && _obj.DeliveryMethod.Sid == vf.CustomContracts.PublicConstants.Module.MailDeliveryMethod.Exchange)
        _obj.SignDate =  vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.GetDateOfCounterpartySigning(_obj);
      
      // Проверка, что "Начало вып. работ/оказания услуг" не раньше, чем "Окончание вып. работ/оказания услуг".
      if (_obj.StartDate > _obj.EndDate)
      {
        e.AddError(_obj.Info.Properties.StartDate, vf.OverrideBaseDev.ContractualDocuments.Resources.IncorrectStartDates, _obj.Info.Properties.EndDate);
        e.AddError(_obj.Info.Properties.EndDate, vf.OverrideBaseDev.ContractualDocuments.Resources.IncorrectStartDates, _obj.Info.Properties.StartDate);
      }
      
      var deleteVoyage = _obj.Voyages.Where(x => x.Voyage != null && x.Voyage.Vessel != null && !CustomContracts.MVZs.Equals(OverrideBaseDev.Departments.As(_obj.Department)?.MVZ, x.Voyage.Vessel)).FirstOrDefault();
      if (deleteVoyage != null)
        _obj.Voyages.Remove(deleteVoyage);
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.IsStandard = false;
      _obj.IsConfidential = false;
      _obj.NeedTransferToSudk = false;
      _obj.TransferedToSudk = false;
      _obj.IsForceMajeure = false;
      _obj.CurrencyControl = false;
      _obj.SendToCurrencyControl = false;
      
      // Автоматическое заполнение поля "Контрагент нерезидент" в зависимости от контрагента.
      _obj.CounterpartyNonresident = _obj.Counterparty != null ? _obj.Counterparty.Nonresident == true : false;
      base.Created(e);
      
      _obj.IsOriginalContractAvailable = vf.CustomContracts.PublicFunctions.Module.IsOriginalContractAvailable(_obj);
      
      // Свойство "Наименование справочника 1С:ERP" по умолчанию заполняется записью, в котором свойство «Справочник» заполнено значением «1».
      var erpDatabookName = CustomContracts.ErpDatabookNames.GetAll(x => x.Databook == CustomContracts.ErpDatabookName.Databook.One).FirstOrDefault();
      _obj.ErpDatabookName = erpDatabookName;
    }
  }
}