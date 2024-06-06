using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.SAPIntegration.PaymentTerms;

namespace vf.SAPIntegration
{
  partial class PaymentTermsSharedHandlers
  {
    public virtual void PrepaymentChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      vf.SAPIntegration.Functions.PaymentTerms.SetStateProperties(_obj);
    }
    
    public virtual void PostpayChanged(Sungero.Domain.Shared.BooleanPropertyChangedEventArgs e)
    {
      vf.SAPIntegration.Functions.PaymentTerms.SetStateProperties(_obj);
    }
  }
}