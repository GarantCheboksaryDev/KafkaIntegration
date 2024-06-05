using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.DocumentTemplate;

namespace vf.OverrideBaseDev.Server
{
  partial class DocumentTemplateFunctions
  {
    /// <summary>
    /// Получить шаблон по ИД.
    /// </summary>
    /// <param name="templateId">ИД шаблона.</param>
    /// <returns>Шаблон.</returns>
    [Public, Remote(IsPure=true)]
    public static OverrideBaseDev.IDocumentTemplate GetTemplateById(string templateId)
    {
      return OverrideBaseDev.DocumentTemplates.GetAll().Where(t => t.Id.ToString() == templateId).FirstOrDefault();
    }
  }
}