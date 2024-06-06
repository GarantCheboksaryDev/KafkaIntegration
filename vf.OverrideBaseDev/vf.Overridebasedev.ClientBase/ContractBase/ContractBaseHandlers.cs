using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractBase;

namespace vf.OverrideBaseDev
{
  partial class ContractBaseClientHandlers
  {

    public override void CounterpartyValueInput(Sungero.Docflow.Client.ContractualDocumentBaseCounterpartyValueInputEventArgs e)
    {
      base.CounterpartyValueInput(e);
    }
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);

      Functions.ContractBase.SetProjectInitiatorEnable(_obj, e.Params);
      Functions.ContractBase.SetTaxAgentVisible(_obj, e.Params);
      Functions.ContractBase.SetIncotermsEnabled(_obj, e.Params);
      Functions.ContractBase.SetCurrecyControlEnabled(_obj, e.Params);
      Functions.ContractBase.SetDaysToFinishWorksControlEnabled(_obj, e.Params);
      Functions.ContractBase.SetDaysToFinishWorksControlRequired(_obj, e.Params);
      Functions.ContractBase.SetCounterpartyRegistrationNumberEnabled(_obj, e.Params);
      Functions.ContractBase.SetRequiredProperties(_obj);
      Functions.ContractBase.SetResponsibleEmployeerEnable(_obj, e.Params);
    }
  }
}