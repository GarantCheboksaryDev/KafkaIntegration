using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ApprovalTask;

namespace vf.OverrideBaseDev.Shared
{
  partial class ApprovalTaskFunctions
  {
    /// <summary>
    /// Проверка наличия версий документов, в группе вложений "Документ".
    /// </summary>
    /// <returns>False, если с группой вложений можно работать.</returns>
    public virtual bool HasDocumentBodyInDcoumentGroup()
    {
      return _obj.DocumentGroup.OfficialDocuments.Any(x => !x.HasVersions);
    }
    
    /// <summary>
    /// Проверка наличия версий документов, в группе вложений "Приложения".
    /// </summary>
    /// <returns>False, если с группой вложений можно работать.</returns>
    public virtual bool HasDocumentBodyInAddendaGroup()
    {
      return _obj.AddendaGroup.OfficialDocuments.Any(x => !x.HasVersions);
    }
    
    /// <summary>
    /// Проверка наличия версий документов, в группе вложений "Дополнительно".
    /// </summary>
    /// <returns>False, если с группой вложений можно работать.</returns>
    public virtual bool HasDocumentBodyInOtherGroup()
    {
      return _obj.OtherGroup.All.Any(x => Sungero.Docflow.OfficialDocuments.Is(x) && !Sungero.Docflow.OfficialDocuments.As(x).HasVersions);
    }
    
    /// <summary>
    /// Убрать из обязательных согласующих руководителя направления, если он совпадает с автором задачи.
    /// </summary>
    /// <param name="stages">Этапы согласования.</param>
    public void RemoveDirectionManagerFromRequiredApprovers(List<Sungero.Docflow.Structures.Module.DefinedApprovalStageLite> stages)
    {
      var directionManagerStages = stages.Where(s => s.Stage != null && (s.Stage.ApprovalRole?.Type == CustomContracts.DirectionManagerApprovalRole.Type.DirectionManager
                                                                         || s.Stage.ApprovalRoles.Any(x => x.ApprovalRole?.Type == CustomContracts.DirectionManagerApprovalRole.Type.DirectionManager)));
      foreach (var directionManagerStage in directionManagerStages)
      {
        var directionManagers = new List<Sungero.Company.IEmployee>();
        
        if (directionManagerStage.Stage.StageType == OverrideBaseDev.ApprovalStage.StageType.Approvers)
          directionManagers = Sungero.Docflow.PublicFunctions.ApprovalStage.Remote.GetStagePerformers(_obj, directionManagerStage.Stage);
        else if (directionManagerStage.Stage.StageType == OverrideBaseDev.ApprovalStage.StageType.Manager)
          directionManagers.Add(Sungero.Docflow.PublicFunctions.ApprovalStage.Remote.GetStagePerformer(_obj, directionManagerStage.Stage));
        
        if (directionManagers.Any())
        {
          var directionManager = directionManagers.Where(x => OverrideBaseDev.Employees.Equals(x, _obj.Author)).FirstOrDefault();
          if (directionManager != null)
            _obj.ReqApprovers.Remove(_obj.ReqApprovers.Where(x => Employees.Equals(x.Approver, directionManager)).FirstOrDefault());
        }
      }
    }
    
    /// <summary>
    /// Преобразовать базовый этап в этап согласования.
    /// </summary>
    /// <param name="stage">Базовый этап согласования.</param>
    /// <returns>Этап согласования. Null, если базовый этап не является этапом согласования.</returns>
    public static List<Sungero.Docflow.Structures.Module.DefinedApprovalStageLite> CastToDefinedApprovalStagesLite(List<Sungero.Docflow.Structures.Module.DefinedApprovalBaseStageLite> stages)
    {
      var approvalSatges = new List<Sungero.Docflow.Structures.Module.DefinedApprovalStageLite>();
      foreach (var stage in stages.Where(x => x != null))
      {
        var approvalStage = ApprovalStages.As(stage.StageBase);
        if (approvalStage != null)
          approvalSatges.Add(Sungero.Docflow.Structures.Module.DefinedApprovalStageLite.Create(approvalStage, stage.Number, stage.StageType));
      }
      
      return approvalSatges;
    }
    
    /// <summary>
    /// Валидация старта задачи на согласование по регламенту.
    /// </summary>
    /// <param name="e">Аргументы действия.</param>
    /// <returns>True, если валидация прошла успешно, и False, если были ошибки.</returns>
    public override bool ValidateApprovalTaskStart(Sungero.Core.IValidationArgs e)
    {
      var document = _obj.DocumentGroup.OfficialDocuments.FirstOrDefault();
      
      var haveError = base.ValidateApprovalTaskStart(e);
      
      // Проверить вложения Анкеты для Договора и Дополнительного соглашения.
      if (OverrideBaseDev.ContractualDocuments.Is(document))
      {
        var counterpartyEntity = _obj.AddendaGroup.OfficialDocuments.Any(x => OverrideBaseDev.CounterpartyDocuments.Is(x))
          ? _obj.AddendaGroup.OfficialDocuments.Where(x => OverrideBaseDev.CounterpartyDocuments.Is(x)).FirstOrDefault()
          :  _obj.OtherGroup.All.Where(x => OverrideBaseDev.CounterpartyDocuments.Is(x)).FirstOrDefault();
        
        var counterpartyDocument = OverrideBaseDev.CounterpartyDocuments.As(counterpartyEntity);
        
        var questionnaireDocumentKind = Sungero.Docflow.PublicFunctions.DocumentKind.Remote.GetNativeDocumentKindRemote(Guid.Parse(OverrideBaseDev.Constants.Docflow.DocumentKind.QuestionnaireKindGuid));
        
        if (counterpartyDocument != null && counterpartyDocument.DocumentKind != null && OverrideBaseDev.DocumentKinds.Equals(counterpartyDocument.DocumentKind, questionnaireDocumentKind))
        {
          if (OverrideBaseDev.Counterparties.Equals(OverrideBaseDev.ContractualDocuments.As(document).Counterparty, counterpartyDocument.Counterparty))
          {
            if (counterpartyDocument.ValidTo.HasValue && counterpartyDocument.ValidTo.Value < Calendar.Today)
            {
              e.AddError(vf.OverrideBaseDev.ApprovalTasks.Resources.NotActualCounterpartyDocument);
              haveError = true;
            }
          }
          else
          {
            e.AddError(vf.OverrideBaseDev.ApprovalTasks.Resources.CounterpartyNotEqualsInCounterPartyDocument);
            haveError = true;
          }
        }
        else
        {
          e.AddError(vf.OverrideBaseDev.ApprovalTasks.Resources.NotFindCounterpartyDocument);
          haveError = true;
        }
        
        // Если на согласование отправляется договор или допсоглашение, и контрагент находится в черном списке, то выводится ошибка о невозможности отправки задачи.
        var contractualDocument = OverrideBaseDev.ContractualDocuments.As(document);
        var customCounterparty = OverrideBaseDev.Counterparties.As(contractualDocument.Counterparty);
        if (customCounterparty.SpecialList == OverrideBaseDev.Counterparty.SpecialList.Black)
        {
          e.AddError(vf.OverrideBaseDev.ApprovalTasks.Resources.CounterpartyIsInBlackList);
          haveError = true;
        }
      }

      var contract = OverrideBaseDev.ContractBases.As(document);
      if (contract != null)
      {
        if (string.IsNullOrEmpty(contract.MailForAct))
        {
          e.AddError(vf.OverrideBaseDev.ApprovalTasks.Resources.NeedMailForActError);
          haveError = true;
        }
      }
      
      if (Functions.ApprovalTask.HasDocumentBodyInDcoumentGroup(_obj))
      {
        e.AddError(ApprovalTasks.Resources.HasntDocumentBodyErrorFormat("Документ"));
        haveError = true;
      }
      
      if (Functions.ApprovalTask.HasDocumentBodyInAddendaGroup(_obj))
      {
        e.AddError(ApprovalTasks.Resources.HasntDocumentBodyErrorFormat("Приложения"));
        haveError = true;
      }
      
      if (Functions.ApprovalTask.HasDocumentBodyInOtherGroup(_obj))
      {
        e.AddError(ApprovalTasks.Resources.HasntDocumentBodyErrorFormat("Дополнительно"));
        haveError = true;
      }
      
      return haveError;
    }
  }
}