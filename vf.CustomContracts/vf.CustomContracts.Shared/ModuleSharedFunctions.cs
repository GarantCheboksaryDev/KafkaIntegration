using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.CustomContracts.Shared
{
  public class ModuleFunctions
  {    
    /// <summary>
    /// Проверка доступности свойства "Оригинал в наличии" в карточках документов.
    /// </summary>
    /// <param name="paramsDictionary">Параметры.</param>
    /// <returns>True, если сотрудник включен в роль "Сотрудники отдела документационного обеспечения". Иначе - false.</returns>
    [Public]
    public bool IsOriginalContractAvailableEnabled(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var isEnabled = false;
      if (!paramsDictionary.TryGetValue(CustomContracts.PublicConstants.Module.Params.IsDocSupportEmployee, out isEnabled))
      {
        isEnabled = vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.IncludedInRole(Constants.Module.Roles.DocSupportDepartment);
        paramsDictionary.AddOrUpdate(CustomContracts.PublicConstants.Module.Params.IsDocSupportEmployee, isEnabled);
      }
      return isEnabled;
    }
  }
}