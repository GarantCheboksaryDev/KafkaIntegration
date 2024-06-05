using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Counterparty;

namespace vf.OverrideBaseDev.Shared
{
  partial class CounterpartyFunctions
  {
    
    /// <summary>
    /// Установить состояние свойств сущности.
    /// </summary>
    /// <param name="entityParams">Хранилище параметров типа сущности.</param>
    public virtual void SetStateEntityProperties(Sungero.Domain.Shared.ParamsDictionary entityParams)
    {
      var currentUser = Users.Current;
      var isAdmin = IntegrationSettings.PublicFunctions.Module.CheckCurrentUserIsAdmin();
      var includeInServiceSecurityRole = IsUserIncludedInServiceSecurityRole(entityParams);
      
      var properties = _obj.State.Properties;
      properties.ServiceSecurityCheckResult.IsEnabled = isAdmin || includeInServiceSecurityRole;
      properties.SAPID.IsEnabled = isAdmin;
      properties.ExternalId.IsEnabled = isAdmin;
      properties.SpecialList.IsEnabled = includeInServiceSecurityRole;
      properties.SpecialList.IsVisible = includeInServiceSecurityRole;
      properties.Note.IsEnabled = includeInServiceSecurityRole;
      properties.Note.IsVisible = includeInServiceSecurityRole;
      properties.Nonresident.IsEnabled = !IsNonResident();
    }
    
    /// <summary>
    /// Проверка, является ли контрагент нерезидентом.
    /// </summary>
    /// <returns>True, если ИНН контрагента начинается с "9909" или код MDG начинается с "2000" (для организаций) или с "3000" (для банков), иначе False.</returns>
    public bool IsNonResident()
    {
      var isNonResidentSAPID = !string.IsNullOrEmpty(_obj.SAPID)
        && (OverrideBaseDev.Companies.Is(_obj) && _obj.SAPID.StartsWith(Constants.Parties.Counterparty.NonResidentParams.CompanyMDGCode)
            || OverrideBaseDev.Banks.Is(_obj) && _obj.SAPID.StartsWith(Constants.Parties.Counterparty.NonResidentParams.BankMDGCode));
      
      return !string.IsNullOrEmpty(_obj.TIN) && _obj.TIN.StartsWith(Constants.Parties.Counterparty.NonResidentParams.TIN)
        && (string.IsNullOrEmpty(_obj.SAPID) || isNonResidentSAPID)
        || isNonResidentSAPID;
    }
    
    /// <summary>
    /// Проверка вхождения текущего пользователя в роль "Служба безопаности" и сохранение результата в параметры сущности.
    /// </summary>
    /// <param name="entityParams">Хранилище параметров типа сущности.</param>
    /// <returns>True, если пользователь входит в одну из ролей, иначе - false.</returns>
    public static bool IsUserIncludedInServiceSecurityRole(Sungero.Domain.Shared.ParamsDictionary entityParams)
    {
      var includeInServiceSecurityRole = false;
      var paramName = Constants.Parties.Counterparty.ParamNames.IncludeInSecurityServiceParamName;
      if (!entityParams.TryGetValue(paramName, out includeInServiceSecurityRole))
      {
        includeInServiceSecurityRole = vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.IncludedInRole(SAPIntegration.PublicConstants.Module.RoleGuid.SecurityService);
        
        entityParams.AddOrUpdate(paramName, includeInServiceSecurityRole);
      }
      
      return includeInServiceSecurityRole;
    }
  }
}