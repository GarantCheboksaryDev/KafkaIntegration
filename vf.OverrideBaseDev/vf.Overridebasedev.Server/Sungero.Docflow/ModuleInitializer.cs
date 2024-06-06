using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace vf.OverrideBaseDev.Module.Docflow.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      base.Initializing(e);
      
      SetDefaultValueOnDocumentTemplate();
    }
    
    /// <summary>
    /// Заполнить значения по умолчанию в карточке "Шаблон".
    /// </summary>
    public void SetDefaultValueOnDocumentTemplate()
    {
      InitializationLogger.DebugFormat("Init. Шаблоны документов. Обработка стандартных шаблонов.");
      
      var notSetValues = OverrideBaseDev.DocumentTemplates.GetAll(x => x.IsStandardTemplate == null);
      var count = notSetValues.Count();
      
      if (count > 0)
      {
        InitializationLogger.DebugFormat("Init. Шаблоны документов. Записей к обработке: {0}.", count);
        
        foreach (var notSetValue in notSetValues)
        {
          notSetValue.IsStandardTemplate = false;
          notSetValue.Save();
        }
      }
    }
  }
}
