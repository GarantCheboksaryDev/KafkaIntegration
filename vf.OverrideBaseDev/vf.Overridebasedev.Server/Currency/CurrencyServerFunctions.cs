using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Currency;

namespace vf.OverrideBaseDev.Server
{
  partial class CurrencyFunctions
  {
    #region Копипаст из базы.
    
    /// <summary>
    /// Получить валюту по умолчанию.
    /// </summary>
    /// <returns>Валюта, используемая по умолчанию.</returns>
    [Remote(IsPure = true), Public]
    public static ICurrency GetDefaultCurrency()
    {
      return Currencies.GetAll().FirstOrDefault(r => r.IsDefault.Value == true);
    }
    
    /// <summary>
    /// Проверить код валюты на валидность.
    /// </summary>
    /// <param name="numericCode">Код валюты.</param>
    /// <returns>Пустая строка, если код валидный, иначе сообщение с ошибкой о невалидности.</returns>
    public static string ValidateNumericCode(string numericCode)
    {
      if (string.IsNullOrEmpty(numericCode) || !System.Text.RegularExpressions.Regex.IsMatch(numericCode,  @"(^\d{3}$)"))
        return Sungero.Commons.Currencies.Resources.InvalidNumericCode;
      
      return string.Empty;
    }
    
    #endregion
    
    /// <summary>
    /// Получить валюту по буквенному коду.
    /// </summary>
    /// <param name="alphaCode">Буквенный код.</param>
    /// <returns>Валюта</returns>
    [Public]
    public static OverrideBaseDev.ICurrency GetCurrencyByAlphaCode(string alphaCode)
    {
      return OverrideBaseDev.Currencies.GetAll(x => x.AlphaCode == alphaCode).FirstOrDefault();
    }
  }
}