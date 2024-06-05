using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractualDocument;

namespace vf.OverrideBaseDev
{
  partial class ContractualDocumentClientHandlers
  {
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var prop = _obj.State.Properties;
      
      var documentKind = vf.OverrideBaseDev.DocumentKinds.As(_obj.DocumentKind);
      var isDocumentKindConfidential = documentKind != null && documentKind.IsConfidential == true;
      
      // Доступность свойства Конфиденциально в зависмости от вида документа.
      // Также доступно сотрудникам с полными правами на документ при условии, что свойство Конфиденциально в документе = false.
      prop.IsConfidential.IsEnabled = !isDocumentKindConfidential
        && _obj.AccessRights.IsGranted(DefaultAccessRightsTypes.FullAccess, Users.Current);
      
      Functions.ContractualDocument.EventValidTillPropertiesEnable(_obj, e.Params);
      
      // Отображение хинта "Доверенность подписывающего действительна с <Дата> по <Дата>".
      if (_obj.OurSignatory != null)
      {
        var hintPOA = string.Empty;
        if (!e.Params.TryGetValue(CustomContracts.PublicConstants.Module.Params.PowerOfAttoneyHintText, out hintPOA))
        {
          hintPOA = CustomContracts.PublicFunctions.Module.Remote.GetPowerOfAttoneyHint(_obj.OurSigningReason);
          e.Params.AddOrUpdate(CustomContracts.PublicConstants.Module.Params.PowerOfAttoneyHintText, hintPOA);
        }
        
        if (!string.IsNullOrEmpty(hintPOA))
          e.AddInformation(hintPOA);
      }
      
      // Достуность свойства "Оригинал договора в наличии".
      prop.IsOriginalContractAvailable.IsEnabled = CustomContracts.PublicFunctions.Module.IsOriginalContractAvailableEnabled(e.Params);
      
    }
  }
}