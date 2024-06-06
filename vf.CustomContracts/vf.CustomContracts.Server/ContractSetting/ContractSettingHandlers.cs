using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.ContractSetting;

namespace vf.CustomContracts
{
  partial class ContractSettingServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      // Если найдена запись справочника с таким же значением свойства "Наша организация", то выводим ошибку, и не даем сохранить.
      if (ContractSettings.GetAll(x => OverrideBaseDev.BusinessUnits.Equals(_obj.BusinessUnit, x.BusinessUnit) && !ContractSettings.Equals(_obj, x)).Any())
        e.AddError(_obj.Info.Properties.BusinessUnit, vf.CustomContracts.ContractSettings.Resources.CantSaveDatabook);
    }
  }

}