using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.Planing.Plan;

namespace Study.Planing.Client
{
  partial class PlanActions
  {
    public virtual void ShowManager(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      if(_obj.Manager != null)
        _obj.Manager.Show();
      else
        e.AddWarning("Руководитель плана не указан!!!");
    }

    public virtual bool CanShowManager(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}