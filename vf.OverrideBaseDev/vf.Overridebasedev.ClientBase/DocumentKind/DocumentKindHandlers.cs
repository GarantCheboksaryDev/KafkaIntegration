using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.DocumentKind;

namespace vf.OverrideBaseDev
{
  partial class DocumentKindClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      var isAdmin = IntegrationSettings.PublicFunctions.Module.CheckCurrentUserIsAdmin();
      
      // Свойство Конфиденциально доступно только для типов документов Договор и Дополнительное соглашение сотрудникам, которые входят в роль "Администраторы".
      _obj.State.Properties.IsConfidential.IsEnabled = isAdmin
        && _obj.DocumentType != null
        && (_obj.DocumentType.DocumentTypeGuid == Constants.Docflow.DocumentKind.ContractGuid
            || _obj.DocumentType.DocumentTypeGuid == Constants.Docflow.DocumentKind.SupAgreementGuid);
    }

  }
}