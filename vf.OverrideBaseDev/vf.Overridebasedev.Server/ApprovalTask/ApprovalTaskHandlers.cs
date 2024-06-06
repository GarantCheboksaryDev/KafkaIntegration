using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ApprovalTask;

namespace vf.OverrideBaseDev
{
  partial class ApprovalTaskServerHandlers
  {

    public override void BeforeAbort(Sungero.Workflow.Server.BeforeAbortEventArgs e)
    {
      base.BeforeAbort(e);
      
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && OverrideBaseDev.ContractBases.Is(document) && document.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Reserved)
      {
        // Отмена резервирования.
        var handler = CustomContracts.AsyncHandlers.CancelReservation.Create();
        handler.DocumentId = document.Id;
        handler.ExecuteAsync();
      }
    }
  }

}