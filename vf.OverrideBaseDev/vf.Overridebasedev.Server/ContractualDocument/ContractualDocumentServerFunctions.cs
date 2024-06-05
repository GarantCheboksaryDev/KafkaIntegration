using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractualDocument;

namespace vf.OverrideBaseDev.Server
{
  partial class ContractualDocumentFunctions
  {
    /// <summary>
    /// Добавить в блок информацию по полю "Включен в спец список".
    /// </summary>
    /// <param name="documentBlock">Блок с информацией по документу.</param>
    public virtual void AddSpeacialListLabel(Sungero.Core.StateBlock documentBlock)
    {
      var counterparty = vf.OverrideBaseDev.Counterparties.As(_obj.Counterparty);
      if (counterparty != null && counterparty.SpecialList == vf.OverrideBaseDev.Counterparty.SpecialList.AddMonitoring)
      {
        var specialList = counterparty.SpecialList.HasValue ? counterparty.Info.Properties.SpecialList.GetLocalizedValue(counterparty.SpecialList.Value) : "-";
        documentBlock.AddLabel(string.Format("{0}: {1}", vf.OverrideBaseDev.ContractBases.Resources.SpecialList, specialList), CreateRedStyleForLabel());

        documentBlock.AddLineBreak();
      }
    }
    
    /// <summary>
    /// Добавить в блок информацию по полю "С протоколом разногласий".
    /// </summary>
    /// <param name="documentBlock">Блок с информацией по документу.</param>
    public virtual void AddWithDisagreementsProtocolsLabel(Sungero.Core.StateBlock documentBlock)
    {
      if (_obj.WithDisagreementsProtocols == true)
      {
        documentBlock.AddLabel(vf.OverrideBaseDev.ContractualDocuments.Resources.WithDisagreementsProtocolsLabel , CreateRedStyleForLabel());
        documentBlock.AddLineBreak();
      }
    }
    
    /// <summary>
    /// Создать красный стиль для текстового элеменов.
    /// </summary>
    /// <returns>Стиль.</returns>
    public Sungero.Core.StateBlockLabelStyle CreateRedStyleForLabel()
    {
      var style = StateBlockLabelStyle.Create();
      style.Color = Sungero.Core.Colors.Common.Red;
      return style;
    }
    
    /// <summary>
    /// Получить общую сумму договора / доп. соглашения в рублях.
    /// </summary>
    /// <param name="signingDate">Дата подписания.</param>
    /// <returns>Общая сумма договора / доп. соглашения в рублях.</returns>
    [Public]
    public double GetRubSum(DateTime? signingDate)
    {
      double sum = 0;
      
      if (_obj.TotalAmount.HasValue)
      {
        if (_obj.Currency.AlphaCode == CustomContracts.PublicConstants.Module.RubAlphaCode)
          sum = _obj.TotalAmount.Value;
        else
        {
          var currencyRate = CustomContracts.CurrencyRates.GetAll(x => Sungero.Commons.Currencies.Equals(x.Currency, _obj.Currency)
                                                                  && x.Date == signingDate && x.Status == CustomContracts.CurrencyRate.Status.Active).FirstOrDefault();
          if (currencyRate != null && currencyRate.Rate.HasValue)
            sum = currencyRate.Rate.Value * _obj.TotalAmount.Value;
        }
      }
      
      return sum;
    }
    
    /// <summary>
    /// Отправить договор на установку валютного контроля.
    /// </summary>
    [Public, Remote]
    public virtual void SendContractOnCurrencyControl()
    {
      var sendAsync = CustomContracts.AsyncHandlers.SendContractOnCurrencyControl.Create();
      sendAsync.ContractId = _obj.Id;
      sendAsync.ExecuteAsync(vf.OverrideBaseDev.ContractualDocuments.Resources.ContractSendToCKRFormat(_obj.Id));
    }
    
    /// <summary>
    /// Получить ид шаблона из истории.
    /// </summary>
    /// <param name="lastVersionNumber">Номер последней версии.</param>
    /// <returns>Ид шаблона.</returns>
    [Remote(IsPure=true)]
    public long GetTemplateIdFromHistory(int lastVersionNumber)
    {
      var created = new Enumeration(Constants.Contracts.ContractualDocument.HistoryOperation.Create);
      
      long templateId = 0;
      
      string historyRecordComment = _obj.History.GetAll().Where(x => x.VersionNumber == lastVersionNumber
                                                                && (x.Operation == Sungero.Content.DocumentHistory.Operation.CreateVersion
                                                                    || x.Operation == Sungero.Content.DocumentHistory.Operation.UpdateVersion
                                                                    || x.Operation == Sungero.Content.DocumentHistory.Operation.UpdateVerBody
                                                                    || x.Operation == Sungero.Content.DocumentHistory.Operation.Import
                                                                    || x.Operation == created
                                                                    || x.Action == Sungero.Content.DocumentHistory.Operation.CreateVersion
                                                                    || x.Action == Sungero.Content.DocumentHistory.Operation.UpdateVersion
                                                                    || x.Action == Sungero.Content.DocumentHistory.Operation.UpdateVerBody
                                                                    || x.Action == Sungero.Content.DocumentHistory.Operation.Import
                                                                    || x.Action == created))
        .OrderByDescending(x => x.HistoryDate)
        .Select(x => x.Comment)
        .FirstOrDefault();
      
      if (!string.IsNullOrEmpty(historyRecordComment))
      {
        string[] separator = { Constants.Contracts.ContractualDocument.HistoryOperation.ID };
        
        var createFromTemplatePattern = historyRecordComment.Split(separator, StringSplitOptions.None);
        if (createFromTemplatePattern.Count() > 1)
        {
          var createFromTemplateId = createFromTemplatePattern[1].Trim('"').Trim();
          long.TryParse(createFromTemplateId, out templateId);
        }
      }
      
      return templateId;
    }
    
    /// <summary>
    /// Создать асинхронный обработчик для отправки договора.
    /// </summary>
    [Public, Remote]
    public void CreateSendContractAsycnh()
    {
      var asynch = KafkaIntegration.AsyncHandlers.SendContractInfo.Create();
      asynch.ContractId = _obj.Id;
      asynch.ExecuteAsync();
    }
  }
}