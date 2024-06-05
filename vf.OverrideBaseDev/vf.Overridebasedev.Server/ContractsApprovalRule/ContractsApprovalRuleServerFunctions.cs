using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using vf.OverrideBaseDev.ContractsApprovalRule;

namespace vf.OverrideBaseDev.Server
{
  partial class ContractsApprovalRuleFunctions
  {
    /// <summary>
    /// Проверка возможности существования маршрута правила.
    /// </summary>
    /// <param name="route">Маршрут.</param>
    /// <param name="ruleConditions">Условие.</param>
    /// <param name="conditionStep">Этап.</param>
    /// <returns>Возможность существования.</returns>
    public override bool CheckRoutePossibility(List<Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep> route, List<Sungero.Docflow.Structures.ApprovalRuleBase.ConditionRouteStep> ruleConditions, Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep conditionStep)
    {
      var possibleStage = base.CheckRoutePossibility(route, ruleConditions, conditionStep);
      var conditionType = _obj.Conditions.First(c => c.Number == conditionStep.StepNumber).Condition.ConditionType;
      
      if (conditionType == vf.OverrideBaseDev.ContractCondition.ConditionType.SvcSecurityChkR)
      {
        var serviceSecurityCheckConditions = this.GetServiceSecurityCheckResultConditionsInRoute(route).Where(c => c.StepNumber != conditionStep.StepNumber).ToList();
        possibleStage = this.CheckServiceSecurityCheckResultConditions(serviceSecurityCheckConditions, conditionStep);
      }
      
      return possibleStage;
    }
    
    /// <summary>
    /// Проверить возможность существования данного маршрута с условиями по результату проверки контрагентов.
    /// </summary>
    /// <param name="allConditions">Все условия в данном маршруте.</param>
    /// <param name="condition">Текущее условие.</param>
    /// <returns>Возможность существования данного маршрута.</returns>
    public List<Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep> GetServiceSecurityCheckResultConditionsInRoute(List<Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep> route)
    {
      return route.Where(e => _obj.Conditions.Any(c => Equals(c.Number, e.StepNumber) && c.Condition.ConditionType ==
                                                  vf.OverrideBaseDev.ContractCondition.ConditionType.SvcSecurityChkR)).ToList();
    }
    
    /// <summary>
    /// Проверить возможность существования данного маршрута с условиями по результату проверки контрагентов.
    /// </summary>
    /// <param name="allConditions">Все условия в данном маршруте.</param>
    /// <param name="condition">Текущее условие.</param>
    /// <returns>Возможность существования данного маршрута.</returns>
    public bool CheckServiceSecurityCheckResultConditions(List<Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep> allConditions, Sungero.Docflow.Structures.ApprovalRuleBase.RouteStep condition)
    {
      var conditionItem = _obj.Conditions.Where(x => x.Number == condition.StepNumber).FirstOrDefault();
      var contractCondition = vf.OverrideBaseDev.ContractConditions.As(conditionItem.Condition);
      var serviceSecurityCheckResult = contractCondition.ServiceSecurityCheckResult;

      foreach (var previousCondition in allConditions.TakeWhile(x => !Equals(x, condition)))
      {
        var previousConditionItem = _obj.Conditions.Where(x => x.Number == previousCondition.StepNumber).FirstOrDefault();
        var previousContractCondition = vf.OverrideBaseDev.ContractConditions.As(previousConditionItem.Condition);
        var previousServiceSecurityCheckResult = previousContractCondition.ServiceSecurityCheckResult;
        
        if (vf.CustomParties.ServiceSecurityCheckResults.Equals(serviceSecurityCheckResult, previousServiceSecurityCheckResult) && previousCondition.Branch != condition.Branch)
          return false;
      }
      
      return true;
    }
  }
}