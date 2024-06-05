using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.DirectionManagerApprovalRole;

namespace vf.CustomContracts.Server
{
  partial class DirectionManagerApprovalRoleFunctions
  {
    public override Sungero.Company.IEmployee GetRolePerformer(Sungero.Docflow.IApprovalTask task)
    {
      var document = task.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (_obj.Type == CustomContracts.DirectionManagerApprovalRole.Type.DirectionManager)
      {
        if (document != null)
          return CustomContracts.PublicFunctions.Module.GetDirectionManager(document.Department);
        return Sungero.Company.Employees.Null;
      }
      else if (_obj.Type == CustomContracts.DirectionManagerApprovalRole.Type.BudgetController)
      {
        var contractualDocument = vf.OverrideBaseDev.ContractualDocuments.As(document);
        if (contractualDocument != null)
          return CustomContracts.PublicFunctions.Module.GetBudgetController(contractualDocument);
        
        return Sungero.Company.Employees.Null;
      }
      
      return base.GetRolePerformer(task);
    }
  }
}