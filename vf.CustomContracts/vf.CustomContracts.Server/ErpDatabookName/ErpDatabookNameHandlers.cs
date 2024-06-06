using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.ErpDatabookName;

namespace vf.CustomContracts
{
  partial class ErpDatabookNameServerHandlers
  {
    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Если найдена запись справочника с таким же значением свойства "Справочник". то выводим ошибку, и не даем сохранить.
      if (ErpDatabookNames.GetAll(x => x.Databook == _obj.Databook && !ErpDatabookNames.Equals(x, _obj)).Any())
        e.AddError(_obj.Info.Properties.Databook, vf.CustomContracts.ErpDatabookNames.Resources.ErpDatabookHasDuplicates);
    }
  }
}