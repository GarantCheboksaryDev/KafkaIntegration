using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.Request;

namespace Study.HelpDesk
{
  partial class RequestSharedHandlers
  {

    public virtual void RequestCollectionChanged(Sungero.Domain.Shared.CollectionPropertyChangedEventArgs e)
    {
      _obj.SumHours = _obj.RequestCollection.Select(element => element.CountTime).Sum();
    }
  }

  partial class RequestRequestCollectionSharedCollectionHandlers
  {

    public virtual void RequestCollectionAdded(Sungero.Domain.Shared.CollectionPropertyAddedEventArgs e)
    {
      _added.Employee = Sungero.Company.Employees.Current;
    }
  }
}