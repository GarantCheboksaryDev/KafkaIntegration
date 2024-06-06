using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractBase;

namespace vf.OverrideBaseDev.Client
{
  partial class ContractBaseFunctions
  {
    #region Всплывающие подсказки.
    
    /// <summary>
    /// Получить всплывающую подсказку для контролла "Тип банковской комиссии".
    /// </summary>
    public void GetBankChargeTypeNotification()
    {
      var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params;
      PublicFunctions.ContractualDocument.FillNotificationTexts(_obj, paramsDictionary);
      if (paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.BankChargeType.Value))
        Dialogs.NotifyMessage(paramsDictionary[CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.BankChargeType.Value].ToString());
    }
    
    /// <summary>
    /// Получить всплывающую подсказку для контролла "Налоговый агент".
    /// </summary>
    public void GetTaxAgentNotification()
    {
      var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params;
      PublicFunctions.ContractualDocument.FillNotificationTexts(_obj, paramsDictionary);
      if (paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TaxAgent.Value))
        Dialogs.NotifyMessage(paramsDictionary[CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TaxAgent.Value].ToString());
    }
    
    /// <summary>
    /// Получить всплывающую подсказку для контролла "Итоговая сумма с ДДД (в рублях)".
    /// </summary>
    public void GetTotalSumNotification()
    {
      var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params;
      PublicFunctions.ContractualDocument.FillNotificationTexts(_obj, paramsDictionary);
      if (paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TotalSum.Value))
        Dialogs.NotifyMessage(paramsDictionary[CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.TotalSum.Value].ToString());
    }

    /// <summary>
    /// Получить всплывающую подсказку для контролла "Включен в спец. список".
    /// </summary>
    public void GetPurchasingProcedyreTypeNotification()
    {
      var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params;
      PublicFunctions.ContractualDocument.FillNotificationTexts(_obj, paramsDictionary);
      if (paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.PurchasingProce.Value))
        Dialogs.NotifyMessage(paramsDictionary[CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.PurchasingProce.Value].ToString());
    }
    
    /// <summary>
    /// Получить всплывающую подсказку для контролла "Включен в спец. список".
    /// </summary>
    public void GetSpecialListNotification()
    {
      var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params;
      PublicFunctions.ContractualDocument.FillNotificationTexts(_obj, paramsDictionary);
      if (paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.SpecialList.Value))
        Dialogs.NotifyMessage(paramsDictionary[CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.SpecialList.Value].ToString());
    }

    /// <summary>
    /// Получить всплывающую подсказку для контролла "Основание заключения договора".
    /// </summary>
    public void GetContractConcludingBasisNotification()
    {
      var paramsDictionary = ((Sungero.Domain.Shared.IExtendedEntity)_obj).Params;
      PublicFunctions.ContractualDocument.FillNotificationTexts(_obj, paramsDictionary);
      if (paramsDictionary.ContainsKey(CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.ContractConclud.Value))
        Dialogs.NotifyMessage(paramsDictionary[CustomContracts.ControlNotificationSettingDocumentProperties.PropertyName.ContractConclud.Value].ToString());
    }
    
    #endregion
  }
}