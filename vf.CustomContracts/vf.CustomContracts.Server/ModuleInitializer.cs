using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Docflow;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace vf.CustomContracts.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      GrantRightsOnDatabooks();
      CreateRoles();
      CreateApprovalRoles();
      CreateAutoNumberingOfMemoStage();
      GrantRightsOnSetCurrencyControl();
      CreateDocumentKinds();
      CreatePaymentTypes();
    }
    
    #region Создание и получение Ролей.
    
    /// <summary>
    /// Создание ролей согласования.
    /// </summary>
    public void CreateApprovalRoles()
    {
      CreateApprovalRole(CustomContracts.DirectionManagerApprovalRole.Type.DirectionManager, vf.CustomContracts.Resources.DirectionManagerApprovalRoleName);
      CreateApprovalRole(CustomContracts.DirectionManagerApprovalRole.Type.BudgetController, vf.CustomContracts.Resources.BudgetControllerRoleName);
    }
    
    /// <summary>
    /// Создание роли согласования.
    /// </summary>
    public static void CreateApprovalRole(Enumeration roleType, string description)
    {
      var role = DirectionManagerApprovalRoles.GetAll().Where(r => Equals(r.Type, roleType)).FirstOrDefault();
      // Проверяет наличие роли.
      if (role == null)
      {
        role = DirectionManagerApprovalRoles.Create();
        role.Type = roleType;
      }
      role.Description = description;
      role.Save();
      InitializationLogger.DebugFormat("Создана роль {0}.", description);
    }
    
    /// <summary>
    /// Создать предопределенные роли.
    /// </summary>
    public void CreateRoles()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(vf.CustomContracts.Resources.DocSupportDepartmentRoleName,
                                                                      vf.CustomContracts.Resources.DocSupportDepartmentRoleName,
                                                                      Constants.Module.Roles.DocSupportDepartment);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(vf.CustomContracts.Resources.CurrencyControlRoleName,
                                                                      vf.CustomContracts.Resources.CurrencyControlRoleName,
                                                                      Constants.Module.Roles.CurrencyControlRole);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(vf.CustomContracts.Resources.CurrencyControlPerformerRoleName,
                                                                      vf.CustomContracts.Resources.CurrencyControlPerformerRoleDescription,
                                                                      Constants.Module.Roles.CurrencyControlAuthorRole);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(vf.CustomContracts.Resources.UsersWithRightsToSeaVoyagesRoleName,
                                                                      vf.CustomContracts.Resources.UsersWithRightsToSeaVoyagesRoleName,
                                                                      Constants.Module.Roles.UsersWithRightsToSeaVoyagesRole);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(vf.CustomContracts.Resources.FinancialControlRoleName,
                                                                      vf.CustomContracts.Resources.FinancialControlRoleDescription,
                                                                      Constants.Module.Roles.FinancialControlRole);
      
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateRole(vf.CustomContracts.Resources.MethodologistCentralResearchCenterRoleName,
                                                                      vf.CustomContracts.Resources.MethodologistCentralResearchCenterRoleName,
                                                                      Constants.Module.Roles.MethodologistCentralResearchCenterRole);
    }
    
    /// <summary>
    /// Получить роль "Методолог ЦКР".
    /// </summary>
    /// <returns>Роль "Методолог ЦКР".</returns>
    [Public]
    public static IRole GetMethodologistCentralResearchCenterRole()
    {
      return Roles.GetAll(r => r.Sid ==  Constants.Module.Roles.MethodologistCentralResearchCenterRole).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить роль "Пользователи с правами на справочник Рейсы".
    /// </summary>
    /// <returns>Роль "Пользователи с правами на справочник Рейсы".</returns>
    [Public]
    public static IRole GetUsersWithRightsToSeaVoyagesRole()
    {
      return Roles.GetAll(r => r.Sid ==  Constants.Module.Roles.UsersWithRightsToSeaVoyagesRole).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить роль "ЦКР Валютный контроль".
    /// </summary>
    /// <returns>Роль "ЦКР Валютный контроль".</returns>
    [Public]
    public static IRole GetCurrencyControlRole()
    {
      return Roles.GetAll(r => r.Sid ==  Constants.Module.Roles.CurrencyControlRole).FirstOrDefault();
    }
    
    
    #endregion
    
    #region Выдача прав.

    /// <summary>
    /// Выдать права на справочники.
    /// </summary>
    public void GrantRightsOnDatabooks()
    {
      #region Выдать права роли "Ответственные за договоры".
      
      InitializationLogger.Debug("Init: ContractCustom. Grant rights on databooks to responsible for contracts.");
      
      var contractsResponsible = Roles.GetAll().Where(n => n.Sid == Sungero.Docflow.PublicConstants.Module.RoleGuid.ContractsResponsible).FirstOrDefault();
      if (contractsResponsible != null)
      {
        ContractConcludingBasises.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        PaymentTypes.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        Projects.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        RelationshipTypes.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        ContractKinds.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        ErpDatabookNames.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        IncotermsKinds.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        ContractSettings.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        CurrencyRates.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        CFOs.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        BKBDRs.AccessRights.Grant(contractsResponsible, DefaultAccessRightsTypes.Read);
        
        ContractConcludingBasises.AccessRights.Save();
        PaymentTypes.AccessRights.Save();
        Projects.AccessRights.Save();
        RelationshipTypes.AccessRights.Save();
        ContractKinds.AccessRights.Save();
        ErpDatabookNames.AccessRights.Save();
        IncotermsKinds.AccessRights.Save();
        ContractSettings.AccessRights.Save();
        CurrencyRates.AccessRights.Save();
        CFOs.AccessRights.Save();
        BKBDRs.AccessRights.Save();
      }
      
      #endregion
      
      #region Выдать права роли "Пользователи с правами на справочник Рейсы".
      
      var usersWithRightsToSeaVoyageRole = GetUsersWithRightsToSeaVoyagesRole();
      if (usersWithRightsToSeaVoyageRole != null)
      {
        CustomContracts.Voyages.AccessRights.Grant(usersWithRightsToSeaVoyageRole, DefaultAccessRightsTypes.Read);
        CustomContracts.Voyages.AccessRights.Save();
        
      }
      
      #endregion
      
      #region Выдать права всем пользователям.
      
      var allUsers = Roles.AllUsers;
      if (allUsers != null)
        CustomContracts.ControlNotificationSettings.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      
      #endregion
    }
    
    /// <summary>
    /// Выдача прав на задачи модуля.
    /// </summary>
    public void GrantRightsOnSetCurrencyControl()
    {
      var financialControlRole = Roles.GetAll().Where(n => n.Sid == CustomContracts.Constants.Module.Roles.FinancialControlRole).FirstOrDefault();
      if (financialControlRole != null)
      {
        SetOnCurrencyControls.AccessRights.Grant(financialControlRole, DefaultAccessRightsTypes.Create);
        SetOnCurrencyControls.AccessRights.Save();
      }
    }
    
    #endregion
    
    #region Создание типов оплаты.
    
    /// <summary>
    /// Создать типы оплат.
    /// </summary>
    public static void CreatePaymentTypes()
    {
      CreatePaymentType(vf.CustomContracts.Resources.PostpayTypeName, Constants.Module.PaymentType.Postpay);
      CreatePaymentType(vf.CustomContracts.Resources.Prepayment, Constants.Module.PaymentType.Prepayment);
    }
    
    /// <summary>
    /// Создать тип оплаты.
    /// </summary>
    /// <param name="name">Название.</param>
    /// <param name="sid">Уникальный ИД, регистрозависимый.</param>
    public static void CreatePaymentType(string name, string sid)
    {
      var result = string.IsNullOrEmpty(sid) ? vf.CustomContracts.PaymentTypes.GetAll(m => m.Name == name).FirstOrDefault() :
        vf.CustomContracts.PaymentTypes.GetAll(m => m.Sid == sid).FirstOrDefault();
      if (result == null)
      {
        result = vf.CustomContracts.PaymentTypes.Create();
        result.Sid = sid;
      }
      result.Name = name;
      result.Save();
    }
    
    #endregion
    
    /// <summary>
    /// Создать виды документов.
    /// </summary>
    public static void CreateDocumentKinds()
    {
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(vf.CustomContracts.Resources.ProtocolDisagreementKindName,
                                                                              vf.CustomContracts.Resources.ProtocolDisagreementKindShrotName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true, false, Constants.Module.DocumentTypes.Addendum,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.DocumentKinds.ProtocolDisagreement);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(vf.CustomContracts.Resources.ProtocolReconcilingDisagreementKindName,
                                                                              vf.CustomContracts.Resources.ProtocolReconcilingDisagreementKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true, false, Constants.Module.DocumentTypes.Addendum,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.DocumentKinds.ProtocolReconcilingDisagreement);
      Sungero.Docflow.PublicInitializationFunctions.Module.CreateDocumentKind(vf.CustomContracts.Resources.ProtocolDisputeResolutionKindName,
                                                                              vf.CustomContracts.Resources.ProtocolDisputeResolutionKindShortName,
                                                                              Sungero.Docflow.DocumentKind.NumberingType.NotNumerable,
                                                                              Sungero.Docflow.DocumentType.DocumentFlow.Inner,
                                                                              true, false, Constants.Module.DocumentTypes.Addendum,
                                                                              new Sungero.Domain.Shared.IActionInfo[] { Sungero.Docflow.OfficialDocuments.Info.Actions.SendForFreeApproval },
                                                                              Constants.Module.DocumentKinds.ProtocolDisputeResolution);
    }
    
    /// <summary>
    /// Создание записи нового типа сценария "Авторегистрация договорных документов".
    /// </summary>
    public static void CreateAutoNumberingOfMemoStage()
    {
      InitializationLogger.DebugFormat("Init: Create stage for automatic numbering of memos.");
      var autoRegistration = CustomContracts.AutoRegistrationScripts.GetAll().FirstOrDefault();
      if (autoRegistration == null)
        autoRegistration = CustomContracts.AutoRegistrationScripts.Create();
      
      autoRegistration.Name = "Автоматическая регистрация договоров";
      autoRegistration.TimeoutInHours = 4;
      autoRegistration.Save();
    }
  }
}
