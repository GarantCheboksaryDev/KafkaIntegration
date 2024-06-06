using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace vf.SAPIntegration.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      GrantRightOnReference();
      CreateRoles();
    }
    
    /// <summary>
    /// Выдать права на справочники.
    /// </summary>
    public void GrantRightOnReference()
    {
      #region Выдать права роли "Ответственные за договоры"
      
      InitializationLogger.Debug("Init: SAPIntegration. Grant rights on linked reference to responsible managers.");
      
      var contractsResponsible = Roles.GetAll().Where(n => n.Sid == Sungero.Docflow.PublicConstants.Module.RoleGuid.ContractsResponsible).FirstOrDefault();
      if (contractsResponsible != null)
      {
        BankChargeTypes.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        ContractEndTypes.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        PaymentTermses.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        Provisions.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        PurchaseProcedureTypes.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        TaxAgents.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        
        BankChargeTypes.AccessRights.Save();
        ContractEndTypes.AccessRights.Save();
        PaymentTermses.AccessRights.Save();
        Provisions.AccessRights.Save();
        PurchaseProcedureTypes.AccessRights.Save();
        TaxAgents.AccessRights.Save();
      }
      
      #endregion
      
      #region Выдать права всем пользователям на просмотр справочника "Расчетные счета".
      
      var allUsers = Roles.AllUsers;
      
      if (!PaymentAccounts.AccessRights.IsGranted(DefaultAccessRightsTypes.Read, allUsers))
      {
        PaymentAccounts.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
        PaymentAccounts.AccessRights.Save();
      }
      
      #endregion
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public void CreateRoles()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(vf.SAPIntegration.Resources.ServiceSecurityRoleName,
                                                                      vf.SAPIntegration.Resources.ServiceSecurityRoleDescription,
                                                                      SAPIntegration.Constants.Module.RoleGuid.SecurityService);
    }
  }
}
