using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ApprovalTask;

namespace vf.OverrideBaseDev
{
  partial class ApprovalTaskSharedHandlers
  {

    public override void AuthorChanged(Sungero.Workflow.Shared.TaskAuthorChangedEventArgs e)
    {
      base.AuthorChanged(e);
      
      if (_obj.ApprovalRule != null)
      {
        var stages = Functions.ApprovalTask.Remote.GetApprovalStages(_obj).Stages;
        Functions.ApprovalTask.RemoveDirectionManagerFromRequiredApprovers(_obj, stages);
      }
    }

    public override void DocumentGroupAdded(Sungero.Workflow.Interfaces.AttachmentAddedEventArgs e)
    {
      base.DocumentGroupAdded(e);
      
      var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)_obj)?.Params;

      // Обновление параметра "Доверенность подписывающего действительна с <Дата> по <Дата>" в задаче на согласование договора/допсоглашения.
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && OverrideBaseDev.ContractualDocuments.Is(document))
      {
        var hintPOA = CustomContracts.PublicFunctions.Module.Remote.GetPowerOfAttoneyHint(document.OurSigningReason);
        paramsDictionary[CustomContracts.PublicConstants.Module.Params.PowerOfAttoneyHintText] = hintPOA;
      }
    }

    public override void DocumentGroupDeleted(Sungero.Workflow.Interfaces.AttachmentDeletedEventArgs e)
    {
      base.DocumentGroupDeleted(e);
      
      var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)_obj)?.Params;
      paramsDictionary[CustomContracts.PublicConstants.Module.Params.PowerOfAttoneyHintText] = string.Empty;
    }

    public override void DocumentGroupCreated(Sungero.Workflow.Interfaces.AttachmentCreatedEventArgs e)
    {
      base.DocumentGroupCreated(e);
      
      var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)_obj)?.Params;
      
      // Обновление параметра "Доверенность подписывающего действительна с <Дата> по <Дата>" в задаче на согласование договора/допсоглашения.
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document != null && OverrideBaseDev.ContractualDocuments.Is(document))
      {
        var hintPOA = CustomContracts.PublicFunctions.Module.Remote.GetPowerOfAttoneyHint(document.OurSigningReason);
        paramsDictionary[CustomContracts.PublicConstants.Module.Params.PowerOfAttoneyHintText] = hintPOA;
      }
    }
  }


}