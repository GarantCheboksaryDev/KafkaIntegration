using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.SAPIntegration.PaymentTerms;

namespace vf.SAPIntegration
{
  partial class PaymentTermsClientHandlers
  {
    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      vf.SAPIntegration.Functions.PaymentTerms.SetStateProperties(_obj);
    }
  }
}