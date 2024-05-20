using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.InternalRequest;

namespace Study.HelpDesk.Client
{
  partial class InternalRequestActions
  {
    public virtual void ShowEmployeeRequest(Sungero.Domain.Client.ExecuteActionArgs e)
    {
    }

    public virtual bool CanShowEmployeeRequest(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

  }

}