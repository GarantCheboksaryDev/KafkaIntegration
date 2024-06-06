using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ConditionBase;

namespace vf.OverrideBaseDev.Shared
{
  partial class ConditionBaseFunctions
  {
    
    public override Sungero.Docflow.Structures.ConditionBase.ConditionResult CheckCondition(Sungero.Docflow.IOfficialDocument document, Sungero.Docflow.IApprovalTask task)
    {
      if (_obj.ConditionType == ConditionType.Registred)
        return Sungero.Docflow.Structures.ConditionBase.ConditionResult.Create(document.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered, string.Empty);

      
      return base.CheckCondition(document, task);
    }
    
    public override System.Collections.Generic.Dictionary<string, List<Enumeration?>> GetSupportedConditions()
    {
      // Все типы системы поддерживают:
      // документ по проекту
      // вид документа
      // условия по ролям согласования
      // способ доставки документа
      var types = Sungero.Docflow.PublicFunctions.DocumentKind.GetDocumentGuids(typeof(Sungero.Docflow.IOfficialDocument));
      return types.ToDictionary(t => t,
                                t => new List<Enumeration?>
                                {
                                  ConditionType.ProjectDocument,
                                  ConditionType.DocumentKind,
                                  ConditionType.RolesComparer,
                                  ConditionType.RoleEmpComparer,
                                  ConditionType.DeliveryMethod,
                                  ConditionType.SignedByCParty,
                                  ConditionType.HasAddenda,
                                  ConditionType.EmployeeInRole,
                                  ConditionType.Registred
                                });
    }
  }
}