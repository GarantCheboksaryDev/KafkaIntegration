using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.ExternalRequest;

namespace Study.HelpDesk
{
  partial class ExternalRequestContactPropertyFilteringServerHandler<T>
  {

    public virtual IQueryable<T> ContactFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
    {
      if(_obj.Company != null)
        query = query.Where(contract => Equals(contract.Company, _obj.Company));
      return query;
    }
  }


}