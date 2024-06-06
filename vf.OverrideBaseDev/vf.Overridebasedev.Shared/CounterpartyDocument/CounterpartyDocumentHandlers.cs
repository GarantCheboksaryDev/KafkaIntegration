using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.CounterpartyDocument;

namespace vf.OverrideBaseDev
{
  partial class CounterpartyDocumentSharedHandlers
  {

    public override void DocumentKindChanged(Sungero.Docflow.Shared.OfficialDocumentDocumentKindChangedEventArgs e)
    {
      base.DocumentKindChanged(e);
      
      // Обновить параметр, хранящий признак того, что документ является анкетой на контрагента.
      if (!OverrideBaseDev.DocumentKinds.Equals(e.NewValue, e.OldValue))
      {
        var isQuestionnaireKind = false;
        var paramName = Constants.Docflow.CounterpartyDocument.IsQuestionnaireKindParamName;

        var questionnaireDocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.GetNativeDocumentKind(Guid.Parse(OverrideBaseDev.Constants.Docflow.DocumentKind.QuestionnaireKindGuid));
        isQuestionnaireKind = e.NewValue != null && DocumentKinds.Equals(e.NewValue, questionnaireDocumentKind);
        
        e.Params.AddOrUpdate(paramName, isQuestionnaireKind);
      }
    }

    public virtual void QuestionnaireSigningDateChanged(Sungero.Domain.Shared.DateTimePropertyChangedEventArgs e)
    {
      if (!Equals(e.NewValue, e.OldValue))
      {
        if (e.NewValue.HasValue)
          _obj.ValidTo = e.NewValue.Value.AddDays(180);
        else
          _obj.ValidTo = null;
      }
    }

  }
}