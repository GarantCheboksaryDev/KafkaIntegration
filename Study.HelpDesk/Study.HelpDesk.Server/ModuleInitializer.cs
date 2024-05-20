using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace Study.HelpDesk.Server
{
  public partial class ModuleInitializer
  {

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      CreateDefaultPurchaseKind();
    }

    /// <summary>
    /// 
    /// </summary>
    private static void CreateDefaultPurchaseKind()
    {
      InitializationLogger.Debug("Выданы права на чтение справочника обращений всем пользователям.");
      var allUsers = Roles.AllUsers;
      Requests.AccessRights.Grant(allUsers, DefaultAccessRightsTypes.Read);
      Requests.AccessRights.Save();
      // Добавить тип документа SupportContract в справочник DocumentType,
      // как документ договорного документопотока.
    }
  }

}
