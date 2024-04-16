using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.Request;

namespace Study.HelpDesk
{
  partial class RequestServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      if(_obj.ClosedDate == null && _obj.LifeCycle == LifeCycle.Closed)
        _obj.ClosedDate = Calendar.Today;
      if(_obj.LifeCycle == LifeCycle.Closed && string.IsNullOrEmpty(_obj.Result))
        e.AddError("Перед закрытием обращения заполните результат.");
    }

    public override void Created(Sungero.Domain.CreatedEventArgs e)
    {
      _obj.Number = _obj.Id;
      _obj.Responsible = Sungero.Company.Employees.Current;
      _obj.LifeCycle = LifeCycle.InWork;
      _obj.CreatedDate = Calendar.Today;
    }

  }
}