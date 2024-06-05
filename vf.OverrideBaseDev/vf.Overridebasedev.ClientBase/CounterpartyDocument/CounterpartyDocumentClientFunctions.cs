using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.CounterpartyDocument;

namespace vf.OverrideBaseDev.Client
{
  partial class CounterpartyDocumentFunctions
  {
    /// <summary>
    /// Проверка, является ли документ Анкетой на контрагента.
    /// </summary>
    /// <param name="entityParams">Параметры.</param>
    /// <returns></returns>
    public bool IsQuestionnaireKind(Sungero.Presentation.FormRefreshEventArgs entityParams)
    {
      var isQuestionnaireKind = false;
      var paramName = Constants.Docflow.CounterpartyDocument.IsQuestionnaireKindParamName;
      if (!entityParams.Params.TryGetValue(paramName, out isQuestionnaireKind))
      {
        var questionnaireDocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.Remote.GetNativeDocumentKindRemote(Guid.Parse(OverrideBaseDev.Constants.Docflow.DocumentKind.QuestionnaireKindGuid));
        isQuestionnaireKind = _obj.DocumentKind != null && DocumentKinds.Equals(_obj.DocumentKind, questionnaireDocumentKind);
        
        entityParams.Params.AddOrUpdate(paramName, isQuestionnaireKind);
      }
      return isQuestionnaireKind;
    }
  }
}