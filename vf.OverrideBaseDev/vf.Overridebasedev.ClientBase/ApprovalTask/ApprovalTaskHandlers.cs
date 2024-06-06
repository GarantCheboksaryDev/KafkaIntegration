using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ApprovalTask;

namespace vf.OverrideBaseDev
{
  partial class ApprovalTaskClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      // Отображение хинта "Доверенность подписывающего действительна с <Дата> по <Дата>" в задаче на согласование по регламенту договора/допсоглашения.
      if (document != null && OverrideBaseDev.ContractualDocuments.Is(document))
      {
        var hintPOA = string.Empty;
        if (!e.Params.TryGetValue(CustomContracts.PublicConstants.Module.Params.PowerOfAttoneyHintText, out hintPOA))
        {
          hintPOA = CustomContracts.PublicFunctions.Module.Remote.GetPowerOfAttoneyHint(document.OurSigningReason);
          e.Params.AddOrUpdate(CustomContracts.PublicConstants.Module.Params.PowerOfAttoneyHintText, hintPOA);
        }
        
        if (!string.IsNullOrEmpty(hintPOA))
          e.AddInformation(hintPOA);
      }
    }

  }
}