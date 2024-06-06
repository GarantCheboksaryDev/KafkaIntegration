using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.PaymentType;

namespace vf.CustomContracts.Server
{
  partial class PaymentTypeFunctions
  {
    /// <summary>
    /// Получить тип оплаты Постоплата/Предоплата.
    /// </summary>
    /// <param name="paymentType">True - Постоплата.False - Предоплата.</param>
    /// <returns>Тип оплаты.</returns>
    [Remote(IsPure = true), Public]
    public static IPaymentType GetPostOrPrePaymentType(bool paymentType)
    {
      return paymentType ? GetPaymentTypeBySid(Constants.Module.PaymentType.Postpay) : GetPaymentTypeBySid(Constants.Module.PaymentType.Prepayment);
    }
    
    /// <summary>
    /// Получить тип оплаты по Sid.
    /// </summary>
    /// <returns>Тип оплаты.</returns>
    [Remote(IsPure = true), Public]
    public static IPaymentType GetPaymentTypeBySid(string sid)
    {
      return PaymentTypes.GetAll(x => x.Sid == sid).FirstOrDefault();
    }    
  }
}