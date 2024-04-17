using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.Request;

namespace Study.HelpDesk.Server
{
  partial class RequestFunctions
  {

    /// <summary>
    /// 
    /// </summary>       
    [Remote]
    public IAddendumRequest CreateAddendumRequest()
    {
      var document = AddendumRequests.Create();
      document.Request = _obj;
      document.Name = string.Format("Приложение к обращению №{0}", _obj.Number);
      return document;
    }

  }
}