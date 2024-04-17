using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.Request;

namespace Study.HelpDesk.Shared
{
  partial class RequestFunctions
  {

    /// <summary>
    /// 
    /// </summary>  
    
    public void RequestThemeEditor()
    {
      if((_obj.RequestKind != null) && (_obj.Number != null) && (_obj.CreatedDate != null) && (_obj.Deskription != null)){
        _obj.Theme = string.Format("{0} № {1} от {2:dd/MM/yyyy} : {3}", _obj.RequestKind, _obj.Number, _obj.CreatedDate, _obj.Deskription.Length > 50 ? _obj.Deskription.Substring(0, 50) : _obj.Deskription);
      }
      else _obj.Theme = "Тема обращения будет сформирована АВТОматически!";
    }

  }
}