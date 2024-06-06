using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.DocumentTemplate;

namespace vf.OverrideBaseDev.Shared
{
  partial class DocumentTemplateFunctions
  {
    /// <summary>
    /// Установить доступность свойств.
    /// </summary>
    public void SetEnableProperties()
    {
      var properties = _obj.State.Properties;
      
      properties.IsStandardTemplate.IsEnabled = _obj.DocumentType.HasValue &&
        (_obj.DocumentType.Value == Guid.Parse(Sungero.Contracts.PublicConstants.Module.ContractGuid)
         || _obj.DocumentType.Value == Guid.Parse(Sungero.Contracts.PublicConstants.Module.SupAgreementGuid));
    }
  }
}