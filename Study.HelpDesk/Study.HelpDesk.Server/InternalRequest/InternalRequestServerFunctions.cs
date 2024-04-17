using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.InternalRequest;

namespace Study.HelpDesk.Server
{
  partial class InternalRequestFunctions
  {

    /// <summary>
    /// 
    /// </summary>    
    [Remote(IsPure=true)]
    public IQueryable<IInternalRequest> GetEmployeeRequests()
    {
      return InternalRequests.GetAll( r => Equals(r.Author, _obj.Author) );
    }

  }
}