using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Currency;

namespace vf.OverrideBaseDev
{
  partial class CurrencyServerHandlers
  {

    public override void BeforeSave(Sungero.Domain.BeforeSaveEventArgs e)
    {
      #region Копипаст из базы.
      var result = Functions.Currency.ValidateNumericCode(_obj.NumericCode);
      if (!string.IsNullOrEmpty(result))
        e.AddError(result);
      
      // Убрана обязательность по цифровому коду.
      
      // Нельзя закрывать валюту по умолчанию.
      if (_obj.Status == Status.Closed && _obj.IsDefault == true)
        e.AddError(Sungero.Commons.Currencies.Resources.ClosedCurrencyCannotBeDefault);
      
      // Если установить для текущей валюты флаг валюты по умолчанию, то с другой валюты этот флаг снимается.
      if (_obj.IsDefault == true)
      {
        var defaultCurrency = Functions.Currency.GetDefaultCurrency();
        if (defaultCurrency != null && defaultCurrency != _obj)
        {
          var lockInfo = Locks.GetLockInfo(defaultCurrency);
          if (lockInfo != null && lockInfo.IsLocked)
          {
            var error = Sungero.Commons.Resources.LinkedEntityLockedFormat(
              defaultCurrency.Name,
              defaultCurrency.Id,
              lockInfo.OwnerName);
            e.AddError(error);
          }
          
          defaultCurrency.IsDefault = false;
        }
      }
      
      #endregion
    }
  }
}