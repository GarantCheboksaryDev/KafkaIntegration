using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.OfficialDocument;

namespace vf.OverrideBaseDev.Server
{
  partial class OfficialDocumentFunctions
  {
    /// <summary>
    /// Получить все данные для отображения диалога регистрации.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <param name="operation">Операция.</param>
    /// <returns>Параметры диалога.</returns>
    [Public, Remote(IsPure = true)]
    public static Sungero.Docflow.Structures.OfficialDocument.IDialogParamsLite GetRegistrationDialogParamsCustom(Sungero.Docflow.IOfficialDocument document, Enumeration operation)
    {
      return GetRegistrationDialogParams(document, operation);
    }
  }
}