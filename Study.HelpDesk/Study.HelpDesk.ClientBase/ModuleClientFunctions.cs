using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace Study.HelpDesk.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// 
    /// </summary>
    [LocalizeFunction("Создание внутреннего обращения","Быстрое создание нового обращения")]
    public virtual void CreateInternalRequest()
    {
      Functions.Module.Remote.CreateInternalRequest().Show();
    }

  }
}