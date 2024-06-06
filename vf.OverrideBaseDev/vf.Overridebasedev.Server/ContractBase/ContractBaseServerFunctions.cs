using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractBase;

namespace vf.OverrideBaseDev.Server
{
  partial class ContractBaseFunctions
  {
    /// <summary>
    /// Построить сводку по документу.
    /// </summary>
    /// <returns>Сводка по документу.</returns>
    [Remote(IsPure = true)]
    public override StateView GetDocumentSummary()
    {
      var documentSummary = StateView.Create();
      var documentBlock = documentSummary.AddBlock();
      
      // Краткое имя документа.
      var documentName = _obj.DocumentKind.Name;
      if (!string.IsNullOrWhiteSpace(_obj.RegistrationNumber))
        documentName += Sungero.Docflow.OfficialDocuments.Resources.Number + _obj.RegistrationNumber;
      
      if (_obj.RegistrationDate != null)
        documentName += Sungero.Docflow.OfficialDocuments.Resources.DateFrom + _obj.RegistrationDate.Value.ToString("d");
      
      documentBlock.AddLabel(documentName);
      
      // Типовой/Не типовой, Рамочный.
      var isStandardLabel = _obj.IsStandard.Value ? ContractBases.Resources.isStandartContract : ContractBases.Resources.isNotStandartContract;
      var isframeworkContractLabel = _obj.IsFrameworkContract.Value ? ContractBases.Resources.IsFrameworkContract : string.Empty;
      
      if (string.IsNullOrEmpty(isframeworkContractLabel))
        documentBlock.AddLabel(string.Format("({0})", isStandardLabel));
      else
        documentBlock.AddLabel(string.Format("({0}, {1})", isStandardLabel, isframeworkContractLabel));
      documentBlock.AddLineBreak();
      documentBlock.AddLineBreak();
      
      // НОР.
      documentBlock.AddLabel(string.Format("{0}: ", _obj.Info.Properties.BusinessUnit.LocalizedName));
      if (_obj.BusinessUnit != null)
        documentBlock.AddLabel(Hyperlinks.Get(_obj.BusinessUnit));
      else
        documentBlock.AddLabel("-");
      
      documentBlock.AddLineBreak();
      
      // Контрагент.
      documentBlock.AddLabel(string.Format("{0}:", ContractBases.Resources.Counterparty));
      if (_obj.Counterparty != null)
      {
        documentBlock.AddLabel(Hyperlinks.Get(_obj.Counterparty));
        if (_obj.Counterparty.Nonresident == true)
          documentBlock.AddLabel(string.Format("({0})", _obj.Counterparty.Info.Properties.Nonresident.LocalizedName).ToLower());
      }
      else
      {
        documentBlock.AddLabel("-");
      }
      
      documentBlock.AddLineBreak();
      
      // Включен спец. список.
      AddSpeacialListLabel(documentBlock);
      
      // С протоколом разногласий.
      AddWithDisagreementsProtocolsLabel(documentBlock);
      
      // Содержание.
      var subject = !string.IsNullOrEmpty(_obj.Subject) ? _obj.Subject : "-";
      documentBlock.AddLabel(string.Format("{0}: {1}", ContractBases.Resources.Subject, subject));
      documentBlock.AddLineBreak();
      
      // Сумма договора.
      var amount = this.GetTotalAmountDocumentSummary(_obj.TotalAmount);
      var amountText = string.Format("{0}: {1}", _obj.Info.Properties.TotalAmount.LocalizedName, amount);
      documentBlock.AddLabel(amountText);
      documentBlock.AddLineBreak();

      // Валюта.
      var currencyText = string.Format("{0}: {1}", _obj.Info.Properties.Currency.LocalizedName, _obj.Currency);
      documentBlock.AddLabel(currencyText);
      documentBlock.AddLineBreak();
      
      // Срок действия договора.
      var validity = "-";
      var validFrom = _obj.ValidFrom.HasValue ?
        string.Format("{0} {1} ", ContractBases.Resources.From, _obj.ValidFrom.Value.Date.ToShortDateString()) :
        string.Empty;
      
      var validTill = _obj.ValidTill.HasValue ?
        string.Format("{0} {1}", ContractBases.Resources.Till, _obj.ValidTill.Value.Date.ToShortDateString()) :
        string.Empty;
      
      var isAutomaticRenewal = _obj.IsAutomaticRenewal.Value &&  !string.IsNullOrEmpty(validTill) ?
        string.Format(", {0}", ContractBases.Resources.Renewal) :
        string.Empty;
      
      if (!string.IsNullOrEmpty(validFrom) || !string.IsNullOrEmpty(validTill))
        validity = string.Format("{0}{1}{2}", validFrom, validTill, isAutomaticRenewal);
      
      var validityText = string.Format("{0}:", ContractBases.Resources.Validity);
      documentBlock.AddLabel(validityText);
      documentBlock.AddLabel(validity);
      documentBlock.AddLineBreak();
      documentBlock.AddEmptyLine();
      
      // Примечание.
      var note = string.IsNullOrEmpty(_obj.Note) ? "-" : _obj.Note;
      var noteText = string.Format("{0}:", ContractBases.Resources.Note);
      documentBlock.AddLabel(noteText);
      documentBlock.AddLabel(note);
      
      return documentSummary;
    }
    
    /// <summary>
    /// Проверить, входит ли текущий пользователь в группу регистрации, которая зарегистрировала договор.
    /// </summary>
    /// <returns>True, если текущий пользователь входит в группу регистрации, которая зарегистрировала договор. Иначе - false.</returns>
    [Remote]
    public bool CheckCurrentUserIsInRegistrationGroup()
    {
      var registrationGroup = _obj.DocumentRegister?.RegistrationGroup;
      if (registrationGroup == null)
        return false;
      
      return Users.Current.IncludedIn(registrationGroup);
    }
    
    /// <summary>
    /// Получить итоговую сумму с ДДД.
    /// </summary>
    /// <returns>Итоговая сумма с ДДД.</returns>
    [Public]
    public double GetTotalSum()
    {
      return PublicFunctions.ContractualDocument.GetRubSum(_obj, _obj.SignDate);
    }    
  }
}