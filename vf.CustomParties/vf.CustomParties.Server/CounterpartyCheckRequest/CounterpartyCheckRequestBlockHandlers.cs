using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using vf.CustomParties.CounterpartyCheckRequest;

namespace vf.CustomParties.Server.CounterpartyCheckRequestBlocks
{
  partial class InitiatorNoticeBlockHandlers
  {

    public virtual void InitiatorNoticeBlockStartNotice(Sungero.Workflow.INotice notice)
    {
      var subject = vf.CustomParties.CounterpartyCheckRequests.Resources.CounterpartyCheckedNoticeSubjectFormat(Functions.CounterpartyCheckRequest.GetCounterpartyName(_obj.Counterparty));
      notice.Subject = Sungero.Docflow.PublicFunctions.Module.CutText(subject, notice.Info.Properties.Subject.Length);
    }
  }

  partial class CounterpartyCheckAssignmentHandlers
  {

    public virtual void CounterpartyCheckAssignmentCompleteAssignment(vf.CustomParties.ICounterpartyCheckAssignment assignment)
    {
      _obj.CheckingResultSB = assignment.CheckingResultSB;
      _obj.CheckingResultSPARK = assignment.CheckingResultSPARK;
      _obj.CommentSB = assignment.ActiveText;
      _obj.CheckingDate = Calendar.Today;
      
      if (_obj.Counterparty != null)
      {
        var handler = CustomParties.AsyncHandlers.UpdateCounterpartyRequisitesAsync.Create();
        handler.CounterpartyId = _obj.Counterparty.Id;
        handler.AssignmentId = assignment.Id;
        handler.ExecuteAsync();
      }
    }

    public virtual void CounterpartyCheckAssignmentStartAssignment(vf.CustomParties.ICounterpartyCheckAssignment assignment)
    {
      var subject = vf.CustomParties.CounterpartyCheckRequests.Resources.CounterpartyCheckAssignmentSubjectFormat(Functions.CounterpartyCheckRequest.GetCounterpartyName(_obj.Counterparty));
      assignment.Subject = Sungero.Docflow.PublicFunctions.Module.CutText(subject, assignment.Info.Properties.Subject.Length);
      assignment.RequestType = _obj.RequestType;
      assignment.Counterparty = _obj.Counterparty;
      assignment.TIN = _obj.TIN;
      assignment.IsInBlackList = _obj.IsInBlackList;
    }
  }

}