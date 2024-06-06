using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.ContractSetting;

namespace vf.CustomContracts
{
  partial class ContractSettingClientHandlers
  {

    public virtual void ContractPaymentDeadlineValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue < 0)
        e.AddError(vf.CustomContracts.ContractSettings.Resources.ValueShouldBePositive);
    }

    public virtual void SettingPostpaymentContractOnCurrControlDeadlineValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue < 0)
        e.AddError(vf.CustomContracts.ContractSettings.Resources.ValueShouldBePositive);
    }

    public virtual void SettingPrepaymentContractOnCurrControlDeadlineValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      if (e.NewValue.HasValue && e.NewValue < 0)
        e.AddError(vf.CustomContracts.ContractSettings.Resources.ValueShouldBePositive);
    }

  }
}