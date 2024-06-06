using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.CounterpartyDocument;

namespace vf.OverrideBaseDev
{
  partial class CounterpartyDocumentClientHandlers
  {
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      // Настройка видимости свойств "Дата подписания анкеты" и "Действует по" в зависимости от вида документа.
      var isQuestionnaire = Functions.CounterpartyDocument.IsQuestionnaireKind(_obj, e);
      
      _obj.State.Properties.ValidTo.IsVisible = isQuestionnaire;
      _obj.State.Properties.QuestionnaireSigningDate.IsVisible = isQuestionnaire;
      _obj.State.Properties.QuestionnaireSigningDate.IsRequired = isQuestionnaire;
    }
  }
}