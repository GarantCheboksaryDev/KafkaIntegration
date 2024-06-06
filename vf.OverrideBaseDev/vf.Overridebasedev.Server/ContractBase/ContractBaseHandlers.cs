using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractBase;

namespace vf.OverrideBaseDev
{
  partial class ContractBaseDepartmentPropertyFilteringServerHandler<T>
  {

    public override IQueryable<T> DepartmentFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      query = base.DepartmentFiltering(query, e);
      var allCfoDepartment = vf.CustomContracts.PublicFunctions.CFO.GetAllDepartment();
      return query.Where(x => allCfoDepartment.Any(y => vf.OverrideBaseDev.Departments.Equals(x, y)));
    }
  }


  partial class ContractBaseServerHandlers
  {

    public override void BeforeSaveHistory(Sungero.Content.DocumentHistoryEventArgs e)
    {
      base.BeforeSaveHistory(e);
    }

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      
      #region Присвоить или удалить значение № контрагента
      
      Functions.ContractBase.SetCounterpartyRegistrationNumber(_obj);
      
      #endregion
      
      #region Обновление итоговой суммы с ДДД
      
      if (_obj.TotalAmount != _obj.State.Properties.TotalAmount.OriginalValue
          || !Sungero.Commons.Currencies.Equals(_obj.Currency, _obj.State.Properties.Currency.OriginalValue)
          || _obj.LifeCycleState != _obj.State.Properties.LifeCycleState.OriginalValue
          || _obj.SignDate.HasValue && _obj.SignDate != _obj.State.Properties.SignDate.OriginalValue)
      {
        if (_obj.LifeCycleState == OverrideBaseDev.ContractBase.LifeCycleState.Active)
        {
          var sum = PublicFunctions.ContractBase.GetTotalSum(_obj);
          if (sum != 0)
            _obj.TotalSum = sum;
          else
            _obj.TotalSum = null;
        }
        else
          _obj.TotalSum = null;
      }


      
      #endregion
      
      #region Установить значение поля "С протоколом разногласий".
      
      var addendumRelationDocuments = _obj.Relations.GetRelatedDocuments(Sungero.Docflow.PublicConstants.Module.AddendumRelationName).ToList();

      var protocolDisagreementKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(vf.CustomContracts.PublicConstants.Module.DocumentKinds.ProtocolDisagreement);
      var protocolDisputeResolutionKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(vf.CustomContracts.PublicConstants.Module.DocumentKinds.ProtocolDisputeResolution);
      var protocolReconcilingDisagreementKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(vf.CustomContracts.PublicConstants.Module.DocumentKinds.ProtocolReconcilingDisagreement);

      var hasProtocolDisagreement = addendumRelationDocuments.Any(x => vf.OverrideBaseDev.OfficialDocuments.Is(x) &&
                                                                  (Sungero.Docflow.DocumentKinds.Equals(vf.OverrideBaseDev.OfficialDocuments.As(x).DocumentKind, protocolDisagreementKind) ||
                                                                   Sungero.Docflow.DocumentKinds.Equals(vf.OverrideBaseDev.OfficialDocuments.As(x).DocumentKind, protocolDisputeResolutionKind) ||
                                                                   Sungero.Docflow.DocumentKinds.Equals(vf.OverrideBaseDev.OfficialDocuments.As(x).DocumentKind, protocolReconcilingDisagreementKind)));

      if (!_obj.WithDisagreementsProtocols.HasValue || !_obj.WithDisagreementsProtocols != hasProtocolDisagreement)
        _obj.WithDisagreementsProtocols = hasProtocolDisagreement;
      
      #endregion
      
      #region Продление срока на один год.
      
      if (_obj.LifeCycleState == LifeCycleState.Active && _obj.IsAutomaticRenewal == true &&
          _obj.IsExtendedOneYear != true && _obj.ValidTill.HasValue)
      {
        _obj.ValidTill = _obj.ValidTill.Value.AddYears(1);
        _obj.IsExtendedOneYear = true;
      }
      
      #endregion
      
      if (!Sungero.Company.Employees.Equals(_obj.State.Properties.ProjectInitiator.OriginalValue, _obj.ProjectInitiator))
      {
        var operation = new Enumeration(vf.OverrideBaseDev.PublicConstants.Contracts.ContractualDocument.HistoryOperation.SDChange);
        var comment = vf.OverrideBaseDev.ContractBases.Resources.ProjectInitiatorChangeFormat(_obj.State.Properties.ProjectInitiator.OriginalValue, _obj.ProjectInitiator);
        _obj.History.Write(operation, operation, comment);
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      base.Created(e);
      
      _obj.TripartileContract = false;
      _obj.IsProgramCounterpartyNumber = false;
      var employeeCurrent = Sungero.Company.Employees.Current;
      
      if (_obj.BusinessUnit != null)
      {
        var contractSettings = vf.CustomContracts.PublicFunctions.ContractSetting.GetContractSettings(_obj.BusinessUnit);
        if (contractSettings != null)
          _obj.DaysToFinishWorks = contractSettings.DefaultDaysToFinishWork;
      }
    }
  }
}