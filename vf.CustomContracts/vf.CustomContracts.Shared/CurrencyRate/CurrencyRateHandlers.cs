using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.CurrencyRate;

namespace vf.CustomContracts
{
  partial class CurrencyRateSharedHandlers
  {

    public virtual void RateChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        if (e.NewValue.HasValue)
        {
          var stringRate = e.NewValue.Value.ToString();
          if (_obj.StringRate != stringRate)
            _obj.StringRate = e.NewValue.Value.ToString();
        }
        else if (!string.IsNullOrEmpty(_obj.StringRate))
          _obj.StringRate = string.Empty;
      }
    }
  }
}