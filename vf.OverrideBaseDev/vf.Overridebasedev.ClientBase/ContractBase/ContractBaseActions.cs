using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractBase;

namespace vf.OverrideBaseDev.Client
{

  partial class ContractBaseActions
  {
    public override void SendForApproval(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if (string.IsNullOrEmpty(_obj.MailForAct))
      {
        e.AddError(vf.OverrideBaseDev.ApprovalTasks.Resources.NeedMailForActError);
        return;
      }
      
      base.SendForApproval(e);
    }

    public override bool CanSendForApproval(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return base.CanSendForApproval(e);
    }

    public override void CreateVersionFromLastVersion(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      e.Params.AddOrUpdate(Constants.Contracts.ContractualDocument.ParamNames.FromLastVersionNumber, _obj.LastVersion.Number);
      
      base.CreateVersionFromLastVersion(e);
      
      if (e.Params.Contains(Constants.Contracts.ContractualDocument.ParamNames.FromLastVersionNumber))
        e.Params.Remove(Constants.Contracts.ContractualDocument.ParamNames.FromLastVersionNumber);
    }

    public override void Register(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      base.Register(e);
      
      // HACK. При автоматической регистрации RegistrationNumber пустой. Вызвать проверку для CounterpartyRegistrationNumber и сохранить с отключением событий.
      Functions.ContractBase.SetCounterpartyRegistrationNumber(_obj);
      
      using (EntityEvents.Disable(OverrideBaseDev.ContractBases.Info.Events.BeforeSave))
      {
        _obj.Save();
      }
    }
  }
}