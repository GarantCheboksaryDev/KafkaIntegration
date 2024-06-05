using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.SAPIntegration.PaymentTerms;

namespace vf.SAPIntegration.Shared
{
  partial class PaymentTermsFunctions
  {
    /// <summary>
    /// Установить состояние свойств.
    /// </summary>
    public virtual void SetStateProperties()
    {
      var postpay = _obj.Postpay == true;
      var prepayment =  _obj.Prepayment == true;
      
      var properties = _obj.State.Properties;
      properties.Postpay.IsEnabled = !prepayment;
      properties.Prepayment.IsEnabled = !postpay;
    } 
  }
}