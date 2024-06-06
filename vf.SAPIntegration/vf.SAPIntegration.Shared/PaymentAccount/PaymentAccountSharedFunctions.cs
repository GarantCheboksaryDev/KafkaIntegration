using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.SAPIntegration.PaymentAccount;

namespace vf.SAPIntegration.Shared
{
  partial class PaymentAccountFunctions
  {
    /// <summary>
    /// Изменить доступность свойств.
    /// </summary>
    public void SetRequiredProperties(Sungero.Domain.Shared.ParamsDictionary paramsDictionary)
    {
      var properties = _obj.State.Properties;
      var isOwnAccount = _obj.OwnAccount == true;
      var isBUCopy = false;
      var isAdmin = IntegrationSettings.PublicFunctions.Module.CheckCurrentUserIsAdmin();
      
      if (!paramsDictionary.TryGetValue(Constants.PaymentAccount.CounterpartyIsBUCopy, out isBUCopy))
      {
        isBUCopy = Functions.PaymentAccount.Remote.IsBusinessUnitCopy(_obj.Counterparty);
        paramsDictionary.AddOrUpdate(Constants.PaymentAccount.CounterpartyIsBUCopy, isBUCopy);
      }
      
      // Свойство "Контрагент" - доступно для редактирование, если "Собственный счет" не заполнен.
      properties.Counterparty.IsEnabled = !isOwnAccount;
      
      // Свойства "Собственный счет" и "Наш орг." доступны, если в поле "Контрагент" указана не наша организация.
      properties.OwnAccount.IsEnabled = !isBUCopy;
      
      properties.ExternalId.IsEnabled = isAdmin;
      properties.SAPMDG.IsEnabled = isAdmin;
    }
  }
}