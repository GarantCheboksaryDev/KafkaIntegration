using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace vf.CustomParties.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      GrantRights();
      CreateServiceSecurityCheckResults();
    }
    
    #region Выдача прав.
    
    /// <summary>
    /// Выдача прав всем пользователям.
    /// </summary>
    public static void GrantRightsToAllUsers()
    {
      var allUsers = Roles.AllUsers;
      if (allUsers != null)
        GrantRightsOnDatabooks(allUsers);
    }
    
    /// <summary>
    /// Выдать права всем пользователям на справочники.
    /// </summary>
    /// <param name="allUsers">Группа "Все пользователи".</param>
    public static void GrantRightsOnDatabooks(IRole allUsers)
    {
      InitializationLogger.Debug("Init: Grant rights on databooks to all users.");
      
      // Результаты проверок службы безопасности.
      vf.CustomParties.ServiceSecurityCheckResults.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      vf.CustomParties.ServiceSecurityCheckResults.AccessRights.Save();
    }
    
    /// <summary>
    /// Назначить права роли "Ответственные за контрагентов".
    /// </summary>
    public static void GrantRightsToCounterpartiesResponsibleRole()
    {
      var counterpartiesResponsibleRole = Roles.GetAll().Where(n => n.Sid == Sungero.ExchangeCore.PublicConstants.Module.RoleGuid.CounterpartiesResponsibleRole).FirstOrDefault();
      if (counterpartiesResponsibleRole == null)
        return;
      
      CustomParties.CounterpartyCheckRequests.AccessRights.Grant(counterpartiesResponsibleRole, DefaultAccessRightsTypes.Create);
      CustomParties.CounterpartyCheckRequests.AccessRights.Save();
    }
    
    /// <summary>
    /// Выдача прав на сущности модуля.
    /// </summary>
    public void GrantRights()
    {
      GrantRightsToAllUsers();
      GrantRightsToCounterpartiesResponsibleRole();
    }
    
    #endregion

    #region Создание результатов проверок службы безопасности.
    
    /// <summary>
    /// Создать результаты проверки службы безопасности.
    /// </summary>
    public static void CreateServiceSecurityCheckResults()
    {
      CreateServiceSecurityCheckResults(vf.CustomParties.ServiceSecurityCheckResults.Resources.Approved, Constants.Module.ServiceSecurityCheckResult.Approved);
      CreateServiceSecurityCheckResults(vf.CustomParties.ServiceSecurityCheckResults.Resources.NotApproved, Constants.Module.ServiceSecurityCheckResult.NotApproved);
      CreateServiceSecurityCheckResults(vf.CustomParties.ServiceSecurityCheckResults.Resources.NotRecommended, Constants.Module.ServiceSecurityCheckResult.NotRecommended);
    }
    
    /// <summary>
    /// Создать результат проверки службы безопасности.
    /// </summary>
    /// <param name="name">Название.</param>
    /// <param name="sid">Уникальный ИД, регистрозависимый.</param>
    public static void CreateServiceSecurityCheckResults(string name, string sid)
    {
      var result = string.IsNullOrEmpty(sid) ? vf.CustomParties.ServiceSecurityCheckResults.GetAll(m => m.Name == name).FirstOrDefault() :
        vf.CustomParties.ServiceSecurityCheckResults.GetAll(m => m.Sid == sid).FirstOrDefault();
      if (result == null)
      {
        result = vf.CustomParties.ServiceSecurityCheckResults.Create();
        result.Sid = sid;
      }
      result.Name = name;
      result.Save();
    }
    
    #endregion
  }
}
