using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.Request;

namespace Study.HelpDesk
{
  partial class RequestClientHandlers
  {

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      if(_obj.LifeCycle == LifeCycle.Closed && _obj.Result != null){
        _obj.State.IsEnabled = false;
      }
      if(!Equals(_obj.Responsible, Sungero.Company.Employees.Current))
        _obj.State.Properties.RequestCollection.CanDelete = false;
    }

  }
}