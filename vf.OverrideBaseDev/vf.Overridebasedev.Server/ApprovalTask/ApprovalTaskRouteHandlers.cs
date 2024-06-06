using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Workflow;
using vf.OverrideBaseDev.ApprovalTask;

namespace vf.OverrideBaseDev.Server
{
  partial class ApprovalTaskRouteHandlers
  {

    public override void StartBlock6(Sungero.Docflow.Server.ApprovalAssignmentArguments e)
    {
      base.StartBlock6(e);
      
      var stage = e.Block.Stage;
      if (stage == null)
        return;
      
      var overrideStage = OverrideBaseDev.ApprovalStages.As(stage);
      var signer = _obj.Signatory;
      
      // Исключить подписывающего из согласующих, если установлена настройка "Пропускать согласование с подписывающим".
      if (overrideStage.SkipApprovalOfSigner == true && e.Block.Performers.Contains(signer))
        e.Block.Performers.Remove(signer);
      
      // Исключить руководителя подписывающего из согласующих, если установлена настройка "Пропускать согласование с вышестоящими руководителями подписывающего".
      if (overrideStage.SkipHeadManager == true && e.Block.Performers.Any())
      {
        var removedPerformers = new List<IRecipient>();
        
        foreach (var performer in e.Block.Performers)
        {
          if (Functions.Department.CheckEmployeeIsManager(OverrideBaseDev.Departments.As(signer.Department), OverrideBaseDev.Employees.As(performer)))
            removedPerformers.Add(performer);
        }
        
        if (removedPerformers.Any())
        {
          foreach (var removedPerformer in removedPerformers)
            e.Block.Performers.Remove(removedPerformer);
        }
      }
    }

    public override void StartBlock3(Sungero.Docflow.Server.ApprovalManagerAssignmentArguments e)
    {
      base.StartBlock3(e);
      
      var stage = e.Block.Stage;
      if (stage == null)
        return;
      
      var overrideStage = OverrideBaseDev.ApprovalStages.As(stage);
      var signer = _obj.Signatory;
      
      // Исключить подписывающего из согласующих, если установлена настройка "Пропускать согласование с подписывающим".
      if (overrideStage.SkipApprovalOfSigner == true && e.Block.Performers.Contains(signer))
        e.Block.Performers.Remove(signer);
      
      // Исключить руководителя подписывающего из согласующих, если установлена настройка "Пропускать согласование с вышестоящими руководителями подписывающего".
      if (overrideStage.SkipHeadManager == true && e.Block.Performers.Any())
      {
        var removedPerformers = new List<IRecipient>();
        
        foreach (var performer in e.Block.Performers)
        {
          if (Functions.Department.CheckEmployeeIsManager(OverrideBaseDev.Departments.As(signer.Department), OverrideBaseDev.Employees.As(performer)))
            removedPerformers.Add(performer);
        }
        
        if (removedPerformers.Any())
        {
          foreach (var removedPerformer in removedPerformers)
            e.Block.Performers.Remove(removedPerformer);
        }
      }
    }

    public override void CompleteAssignment9(Sungero.Docflow.IApprovalSigningAssignment assignment, Sungero.Docflow.Server.ApprovalSigningAssignmentArguments e)
    {
      base.CompleteAssignment9(assignment, e);
    }
  }
}