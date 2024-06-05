using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractStatement;

namespace vf.OverrideBaseDev
{
  partial class ContractStatementServerHandlers
  {
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      base.BeforeSave(e);
      if (_obj.State.IsInserted && _obj.AccessRights.CanSetStrictMode(Sungero.Core.AccessRightsStrictMode.Strict))
        _obj.AccessRights.SetStrictMode(Sungero.Core.AccessRightsStrictMode.Strict);
    }
  }
}