using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.SAPIntegration.PaymentAccount;

namespace vf.SAPIntegration.Server
{
  partial class PaymentAccountFunctions
  {
    /// <summary>
    /// Проверить, является ли контрагент копией НОР.
    /// </summary>
    /// <param name="counterparty">Контрагент.</param>
    /// <returns>True, если контрагент является копией НОР, иначе - False.</returns>
    [Remote(IsPure=true)]
    public static bool IsBusinessUnitCopy(Sungero.Parties.ICounterparty counterparty)
    {
      return counterparty == null ? false : OverrideBaseDev.BusinessUnits.GetAll(x => OverrideBaseDev.Counterparties.Equals(x.Company, counterparty)).Any();
    }
    
    /// <summary>
    /// Получить расчетные счета контаргента без фильтрации по НОР в карточке договора.
    /// </summary>
    /// <param name="query">Расчетные счета.</param>
    /// <param name="counterparty">Контрагент.</param>
    /// <returns>Расчетные счета контрагента.</returns>
    [Public, Remote(IsPure=true)]
    public static IQueryable<SAPIntegration.IPaymentAccount> GetCounterpartyPaymentAccounts(Sungero.Parties.ICounterparty counterparty)
    {
      var query = SAPIntegration.PaymentAccounts.GetAll();
      
      // Если "Контрагент" не заполнен, то расчтеные счета контрагента недоступны.
      if (counterparty == null)
        return query.Where(x => x.Id == -1);
      
      // Свойство «Контрагент» из карточки договора соответствует значению из записи справочника расчетного счета.
      query = query.Where(x => OverrideBaseDev.Counterparties.Equals(x.Counterparty, counterparty));
      
      return query;
    }
    
    /// <summary>
    /// Получить расчетные счета банка.
    /// </summary>
    /// <param name="bank">Банк.</param>
    /// <returns>Расчетные счета банка.</returns>
    [Public, Remote(IsPure=true)]
    public static IQueryable<SAPIntegration.IPaymentAccount> GetBankPaymentAccounts(OverrideBaseDev.IBank bank)
    {
      return SAPIntegration.PaymentAccounts.GetAll(x => x.OwnAccount != true && x.Bank != null && OverrideBaseDev.Banks.Equals(bank, x.Bank));
    }
    
    /// <summary>
    /// Получить расчетные счета нашей организации.
    /// </summary>
    /// <param name="businessUnit">Наша организация.</param>
    /// <param name="document">Документ.</param>
    /// <param name="needPaymentAccountsFromBUCopy">True, если нужно также получить расчетные счета, где в поле "Контрагент" указана копия Нашей организации, иначе - false.</param>
    /// <returns>Список расчетных счетов нашей организации.</returns>
    [Public, Remote(IsPure=true)]
    public static IQueryable<SAPIntegration.IPaymentAccount> GetBusinessUnitPaymentAccounts(Sungero.Company.IBusinessUnit businessUnit, OverrideBaseDev.IContractualDocument document, bool needPaymentAccountsFromBUCopy)
    {
      var query = SAPIntegration.PaymentAccounts.GetAll();
      
      // Фильтрация по НОР.
      query = query.Where(x => x.OwnAccount == true && x.BusinessUnits.Any(b => OverrideBaseDev.BusinessUnits.Equals(b.BusinessUnit, businessUnit))
                          || needPaymentAccountsFromBUCopy && x.Counterparty != null && OverrideBaseDev.Counterparties.Equals(businessUnit.Company, x.Counterparty));
      
      // Фильтрация по валюте договора.
      if (document != null && document.Currency != null)
      {
        var rubCurrency = OverrideBaseDev.PublicFunctions.Currency.Remote.GetDefaultCurrency();
        var documentCurrency = OverrideBaseDev.Currencies.As(document.Currency);
        query = query.Where(x => documentCurrency.IsConditionalCurrency == true && OverrideBaseDev.Currencies.Equals(x.Currency, rubCurrency)
                            || documentCurrency.IsConditionalCurrency != true && Sungero.Commons.Currencies.Equals(x.Currency, documentCurrency));
      }
      
      return query;
    }
    
    /// <summary>
    /// Получить расчетные счета по документу.
    /// </summary>
    /// <param name="query">Расчетные счета.</param>
    /// <param name="document">Документ.</param>
    /// <param name="forCounterparty">True, если нужно получить расчетные для контрагента.Иначе - false.</param>
    /// <returns>Список расчетных счетов нашей организации.</returns>
    [Public, Remote(IsPure=true)]
    public static IQueryable<SAPIntegration.IPaymentAccount> GetAvailablePaymentAccounts(vf.OverrideBaseDev.IContractualDocument document, bool forCounterparty)
    {
      var query = SAPIntegration.PaymentAccounts.GetAll();
      
      // Если в карточке договора не заполнено свойство "Наша организация", то расчетные счета недоступны.
      if (document.BusinessUnit == null)
        return query.Where(x => x.Id == -1);
      
      query = forCounterparty ? query.Where(x => x.OwnAccount != true
                                            && x.BusinessUnits.Any(b => b.BusinessUnit != null && OverrideBaseDev.BusinessUnits.Equals(b.BusinessUnit, document.BusinessUnit))
                                            && (document.Counterparty != null && OverrideBaseDev.Counterparties.Equals(x.Counterparty, document.Counterparty)
                                                || document.Counterparty == null)) :
        GetBusinessUnitPaymentAccounts(document.BusinessUnit, document, false);
      
      return query;
    }
    
    /// <summary>
    ///  Получить синхронизированный расчетный счет.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <returns>Найденный расчетный счет, иначе новый.</returns>
    [Public]
    public static SAPIntegration.IPaymentAccount GetPaymentAccountSynch(string guid1C)
    {
      var paymentAccount = SAPIntegration.PaymentAccounts.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
      
      return paymentAccount != null ? paymentAccount : SAPIntegration.PaymentAccounts.Create();
      
    }
  }
}