using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.AccountingDocumentBase;

namespace vf.OverrideBaseDev
{
  partial class AccountingDocumentBaseClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Достуность свойства "Оригинал договора в наличии".
      _obj.State.Properties.IsOriginalContractAvailable.IsEnabled = CustomContracts.PublicFunctions.Module.IsOriginalContractAvailableEnabled(e.Params);
    }

  }
}