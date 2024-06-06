using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;
using Sungero.Docflow.DocumentKind;

namespace vf.OverrideBaseDev.Module.Parties.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      base.Initializing(e);
      CreateDocumentKinds();
    }
    
    // Создание видов документов
    public void CreateDocumentKinds()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(vf.OverrideBaseDev.Module.Parties.Resources.CounterpartyQuestionnaireKindName,
                                                                              vf.OverrideBaseDev.Module.Parties.Resources.CounterpartyQuestionnaireKindName,
                                                                              NumberingType.Numerable,
                                                                              DocumentKind.DocumentFlow.Inner,
                                                                              true,
                                                                              true,
                                                                              Guid.Parse(OverrideBaseDev.Constants.Docflow.DocumentKind.CounterpartyDocumentGuid),
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForApproval,
                                                                                Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval},
                                                                              Guid.Parse(OverrideBaseDev.Constants.Docflow.DocumentKind.QuestionnaireKindGuid));
    }
  }
}
