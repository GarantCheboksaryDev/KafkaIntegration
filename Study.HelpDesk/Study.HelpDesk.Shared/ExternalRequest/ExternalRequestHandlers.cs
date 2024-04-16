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

    public virtual void ContactChanged(Study.HelpDesk.Shared.ExternalRequestContactChangedEventArgs e)
    {
      if(e.NewValue != null){
        _obj.Company = e.NewValue.Company;
      }
    }

    public virtual void CompanyChanged(Study.HelpDesk.Shared.ExternalRequestCompanyChangedEventArgs e)
    {
      if(!Equals(e.NewValue, e.OldValue) && _obj.Contact != null && !_obj.Contact.Company.Equals(e.NewValue)){
        _obj.Contact = null;
      }
      
    }

  }
}