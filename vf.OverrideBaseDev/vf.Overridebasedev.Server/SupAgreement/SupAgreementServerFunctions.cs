using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.SupAgreement;

namespace vf.OverrideBaseDev.Server
{
  partial class SupAgreementFunctions
  {
    public override StateView GetDocumentSummary()
    {
      var documentSummary = StateView.Create();
      var block = documentSummary.AddBlock();
      
      // Краткое имя документа.
      var documentName = _obj.DocumentKind.Name;
      if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
        documentName += OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
      
      if (_obj.RegistrationDate != null)
        documentName += OfficialDocuments.Resources.DateFrom + _obj.RegistrationDate.Value.ToString("d");
      
      block.AddLabel(documentName);
      
      // Типовое/Не типовое.
      var isStandardLabel = _obj.IsStandard.Value ? SupAgreements.Resources.IsStandartSupAgreement : SupAgreements.Resources.IsNotStandartSupAgreement;
      block.AddLabel(string.Format("({0})", isStandardLabel));
      block.AddLineBreak();
      block.AddEmptyLine();
      
      // НОР.
      block.AddLabel(string.Format("{0}: ", _obj.Info.Properties.BusinessUnit.LocalizedName));
      if (_obj.BusinessUnit != null)
        block.AddLabel(Hyperlinks.Get(_obj.BusinessUnit));
      else
        block.AddLabel("-");
      
      block.AddLineBreak();
      
      // Контрагент.
      block.AddLabel(string.Format("{0}: ", _obj.Info.Properties.Counterparty.LocalizedName));
      if (_obj.Counterparty != null)
      {
        block.AddLabel(Hyperlinks.Get(_obj.Counterparty));
        if (_obj.Counterparty.Nonresident == true)
          block.AddLabel(string.Format("({0})", _obj.Counterparty.Info.Properties.Nonresident.LocalizedName).ToLower());
      }
      else
      {
        block.AddLabel("-");
      }
      
      block.AddLineBreak();
      
      // Включен спец. список.
      AddSpeacialListLabel(block);
      
      // Содержание.
      var subject = !string.IsNullOrEmpty(_obj.Subject) ? _obj.Subject : "-";
      block.AddLabel(string.Format("{0}: {1}", _obj.Info.Properties.Subject.LocalizedName, subject));
      block.AddLineBreak();
      
      // Сумма.
      var amount = this.GetTotalAmountDocumentSummary(_obj.TotalAmount);
      var amountText = string.Format("{0}: {1}", _obj.Info.Properties.TotalAmount.LocalizedName, amount);
      block.AddLabel(amountText);
      block.AddLineBreak();

      // Валюта.
      var currencyText = string.Format("{0}: {1}", _obj.Info.Properties.Currency.LocalizedName, _obj.Currency);
      block.AddLabel(currencyText);
      block.AddLineBreak();
      
      // Срок действия.
      var validity = "-";
      var validFrom = _obj.ValidFrom.HasValue
        ? string.Format("{0} {1} ", ContractBases.Resources.From, _obj.ValidFrom.Value.ToShortDateString())
        : string.Empty;
      var validTill = _obj.ValidTill.HasValue
        ? string.Format("{0} {1}", ContractBases.Resources.Till, _obj.ValidTill.Value.ToShortDateString())
        : string.Empty;
      if (!string.IsNullOrEmpty(validFrom) || !string.IsNullOrEmpty(validTill))
        validity = string.Format("{0}{1}", validFrom, validTill);
      
      var validityText = string.Format("{0}: {1}", ContractBases.Resources.Validity, validity);
      block.AddLabel(validityText);
      block.AddEmptyLine();
      
      // Примечание.
      var note = !string.IsNullOrEmpty(_obj.Note) ? _obj.Note : "-";
      block.AddLabel(string.Format("{0}: {1}", _obj.Info.Properties.Note.LocalizedName, note));
      
      return documentSummary;
    }
    
    /// <summary>
    /// Получить итоговую сумму с ДДД по доп. соглашению.
    /// </summary>
    /// <param name="singingDate">Дата подписания.</param>
    /// <returns>Итоговая сумма с ДДД.</returns>
    [Public]
    public double GetTotalSumBySupAgreement(DateTime? singingDate)
    {
      double sum = 0;
      
      if (!singingDate.HasValue)
        return sum;
      
      if (_obj.TotalAmountAction == OverrideBaseDev.SupAgreement.TotalAmountAction.Union)
      {
        if (_obj.LeadingDocument != null && _obj.LeadingDocument.TotalSum.HasValue)
          sum += _obj.LeadingDocument.TotalSum.Value;
        sum += PublicFunctions.ContractualDocument.GetRubSum(_obj, singingDate);
      }
      else if (_obj.TotalAmountAction == OverrideBaseDev.SupAgreement.TotalAmountAction.Replace)
        sum = PublicFunctions.ContractualDocument.GetRubSum(_obj, singingDate);
      
      return sum;
    }
  }
}