using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Company;

namespace vf.OverrideBaseDev.Client
{
  partial class CompanyActions
  {
    public virtual void CreateCounterpartyCheckRequest(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.IncludedInRole(Sungero.ExchangeCore.PublicConstants.Module.RoleGuid.CounterpartiesResponsibleRole))
        Functions.Company.Remote.CreateCounterpartyCheckRequest(_obj).Show();
      else
        e.AddError(vf.OverrideBaseDev.Companies.Resources.CreateCounterpartyRequestError);
    }

    public virtual bool CanCreateCounterpartyCheckRequest(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !string.IsNullOrEmpty(_obj.TIN) || _obj.Nonresident == true;
    }

  }

}