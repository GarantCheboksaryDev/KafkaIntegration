using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.SAPIntegration.PaymentAccount;

namespace vf.SAPIntegration
{
  partial class PaymentAccountServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Если установлен признак "Собственный счет", то можно указать только одну Нашу организацию.
      if (_obj.OwnAccount == true && _obj.BusinessUnits.Count > 1)
      {
        e.AddError(_obj.Info.Properties.BusinessUnits, vf.SAPIntegration.PaymentAccounts.Resources.ManyBusinessUnitsError, _obj.Info.Properties.OwnAccount);
        e.AddError(_obj.Info.Properties.OwnAccount, vf.SAPIntegration.PaymentAccounts.Resources.ManyBusinessUnitsError, _obj.Info.Properties.BusinessUnits);
      }
      
      // Если в качестве Контрагента указана копия Нашей организации, то она не может быть указана в поле Наша орг.
      if (Functions.PaymentAccount.IsBusinessUnitCopy(_obj.Counterparty) && _obj.BusinessUnits.Any(x => OverrideBaseDev.Counterparties.Equals(x.BusinessUnit.Company, _obj.Counterparty)))
      {
        e.AddError(_obj.Info.Properties.BusinessUnits, vf.SAPIntegration.PaymentAccounts.Resources.BusinessUnitCopyError, _obj.Info.Properties.Counterparty);
        e.AddError(_obj.Info.Properties.Counterparty, vf.SAPIntegration.PaymentAccounts.Resources.BusinessUnitCopyError, _obj.Info.Properties.BusinessUnits);
      }
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.OwnAccount = false;
    }
  }

  partial class PaymentAccountFilteringServerHandler<T>
  {

    public override IQueryable<T> Filtering(IQueryable<T> query, Sungero.Domain.FilteringEventArgs e)
    {
      // Фильтрация доступных записей справочника в зависимости от нашей организации сотрудника.
      var currentEmployee = Sungero.Company.Employees.Current;
      if (currentEmployee != null && !IntegrationSettings.PublicFunctions.Module.CheckCurrentUserIsAdmin())
      {
        var businessUnit = currentEmployee.Department?.BusinessUnit;
        var businessUnitCopy = businessUnit?.Company;
        query = query.Where(x => businessUnit != null && (x.BusinessUnits.Any(b => b.BusinessUnit != null && Sungero.Company.BusinessUnits.Equals(b.BusinessUnit, businessUnit)))
                            || businessUnitCopy != null && x.Counterparty != null && OverrideBaseDev.Counterparties.Equals(x.Counterparty, businessUnitCopy));
      }
      
      // Панель фильтрации.
      if (_filter != null)
      {
        if (_filter.BusinessUnit != null)
          query = query.Where(x => x.BusinessUnits.Any(b => OverrideBaseDev.BusinessUnits.Equals(b.BusinessUnit, _filter.BusinessUnit)));
        if (_filter.Counterparty != null)
          query = query.Where(x => OverrideBaseDev.Counterparties.Equals(x.Counterparty, _filter.Counterparty));
        if (_filter.Bank != null)
          query = query.Where(x => OverrideBaseDev.Banks.Equals(x.Bank, _filter.Bank));
        if (_filter.Currency != null)
          query = query.Where(x => Sungero.Commons.Currencies.Equals(x.Currency, _filter.Currency));
        
        if (_filter.OwnAccount)
          query = query.Where(x => x.OwnAccount == true);
        else
          query = query.Where(x => x.OwnAccount != true);
      }
      
      return query;
    }
  }

}