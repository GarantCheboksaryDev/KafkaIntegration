using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.Request;

namespace Study.HelpDesk.Client
{
  partial class RequestActions
  {
    public virtual void ShowAddendumRequests(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      AddendumRequests.GetAll().Where( document => Equals(document.Request, _obj) ).Show();
    }

    public virtual bool CanShowAddendumRequests(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

    public virtual void CreateAddendumDocument(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.Request.Remote.CreateAddendumRequest(_obj).Show();
    }

    public virtual bool CanCreateAddendumDocument(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return !_obj.State.IsInserted;
    }

    public virtual void OpenRequest(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      _obj.State.IsEnabled = true;
      _obj.LifeCycle = LifeCycle.InWork;
      _obj.ClosedDate = null;
    }

    public virtual bool CanOpenRequest(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return Equals(_obj.LifeCycle, LifeCycle.Closed) && !_obj.State.IsChanged;
    }

  }



}