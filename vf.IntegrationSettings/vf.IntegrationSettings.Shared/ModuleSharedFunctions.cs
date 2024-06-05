using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.IntegrationSettings.Shared
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Проверить вхождение текущего пользователя в роль Администраторы.
    /// </summary>
    /// <returns>True - пользователь является Администратором, иначе - false.</returns>
    /// <remarks>За счет применения "hack-a", функция работает быстрее чем другие IncludeIn.</remarks>
    [Public]
    public bool CheckCurrentUserIsAdmin()
    {
      // HACK Проверка вхождение сотрудника в роль реализована косвенно - через проверку прав на тип сущности. Это быстрее так как не выполняются запросы на сервер.
      // Выбран такой справочник, права на изменение которого будут только у админских ролей.
      return IntegrationSettings.ConnectSettingses.AccessRights.CanUpdate();
    }

  }
}