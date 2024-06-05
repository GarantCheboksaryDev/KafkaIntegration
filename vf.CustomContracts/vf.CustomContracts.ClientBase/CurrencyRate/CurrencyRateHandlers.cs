using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.CurrencyRate;

namespace vf.CustomContracts
{
  partial class CurrencyRateClientHandlers
  {

    public virtual void StringRateValueInput(Sungero.Presentation.StringValueInputEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        if (e.NewValue != null)
        {
          double rate = 0;
          if (double.TryParse(e.NewValue, out rate))
          {
            if (_obj.Rate != rate)
              _obj.Rate = rate;
          }
          else
            e.AddError(vf.CustomContracts.CurrencyRates.Resources.WrongRateFormatError);
        }
        else if (_obj.Rate != null)
          _obj.Rate = null;
      }
    }

  }
}