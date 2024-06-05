using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Counterparty;

namespace vf.OverrideBaseDev
{
  partial class CounterpartySharedHandlers
  {

    public virtual void SAPIDChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      // Если код MDG начинается с заданной константы, то устанавливается признак "Нерезидент".
      if (e.NewValue != e.OldValue)
      {
        var nonResident = Functions.Counterparty.IsNonResident(_obj);
        if (_obj.Nonresident != nonResident)
          _obj.Nonresident = nonResident;
      }
    }

    public override void TINChanged(Sungero.Domain.Shared.StringPropertyChangedEventArgs e)
    {
      base.TINChanged(e);
      // Если ИНН начинается с заданной константы, то устанавливается признак "Нерезидент".
      if (e.NewValue != e.OldValue)
      {
        var nonResident = Functions.Counterparty.IsNonResident(_obj);
        if (_obj.Nonresident != nonResident)
          _obj.Nonresident = nonResident;
      }
    }
    
    public virtual void CheckingResultSPARKChanged(Sungero.Domain.Shared.EnumerationPropertyChangedEventArgs e)
    {
      if (e.NewValue != e.OldValue)
      {
        if (e.NewValue == OverrideBaseDev.Counterparty.CheckingResultSPARK.Green)
          _obj.State.Properties.CheckingResultSPARK.HighlightColor = Colors.Common.Green;
        else if (e.NewValue == OverrideBaseDev.Counterparty.CheckingResultSPARK.Red)
          _obj.State.Properties.CheckingResultSPARK.HighlightColor = Colors.Common.Red;
        else if (e.NewValue == OverrideBaseDev.Counterparty.CheckingResultSPARK.Yellow)
          _obj.State.Properties.CheckingResultSPARK.HighlightColor = Colors.Common.Yellow;
        else
          _obj.State.Properties.CheckingResultSPARK.HighlightColor = Colors.Empty;
        
      }
    }
  }
}