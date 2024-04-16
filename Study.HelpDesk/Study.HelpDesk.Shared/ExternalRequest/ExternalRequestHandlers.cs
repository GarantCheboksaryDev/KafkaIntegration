using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Study.HelpDesk.ExternalRequest;

namespace Study.HelpDesk
{
  partial class ExternalRequestSharedHandlers
  {

    public override void SumHoursChanged(Sungero.Domain.Shared.DoublePropertyChangedEventArgs e)
    {
      base.SumHoursChanged(e);
    }

  }
}