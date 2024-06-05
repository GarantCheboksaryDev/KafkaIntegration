using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.Bank;

namespace vf.OverrideBaseDev.Shared
{
  partial class BankFunctions
  {
    /// <summary>
    /// Получить текст ошибки о наличии дублей контрагента.
    /// </summary>
    /// <returns>Текст ошибки.</returns>
    public override string GetCounterpartyDuplicatesErrorText()
    {
      var searchByBic = !string.IsNullOrWhiteSpace(_obj.BIC);
      var searchBySwift = !string.IsNullOrWhiteSpace(_obj.SWIFT);
      var foundByField = string.Empty;
      
      if (_obj.Status == Sungero.CoreEntities.DatabookEntry.Status.Closed)
        return base.GetCounterpartyDuplicatesErrorText();
      
      var duplicates = new List<OverrideBaseDev.IBank>();
      if (searchByBic)
      {
        duplicates.AddRange(Sungero.Parties.PublicFunctions.Bank.Remote.GetBanksWithSameBic(_obj, true).Cast<OverrideBaseDev.IBank>());
        foundByField += _obj.Info.Properties.BIC.LocalizedName;
      }
      if (searchBySwift)
      {
        duplicates.AddRange(Functions.Bank.Remote.GetBanksWithSameSwiftAnBankBranch(_obj, true));
        foundByField += searchByBic ? "/" + _obj.Info.Properties.SWIFT.LocalizedName : _obj.Info.Properties.SWIFT.LocalizedName;
      }
      if (duplicates.Any())
      {
        var firstDuplicate = duplicates.OrderByDescending(x => x.Id).First();
        var duplicateTypeInNominative = Sungero.Commons.PublicFunctions.Module.GetTypeDisplayValue(firstDuplicate, CommonLibrary.DeclensionCase.Nominative);
        return Banks.Resources.SameBicAndOrSwiftFormat(foundByField, duplicateTypeInNominative.ToLower(), firstDuplicate);
      }
      
      return string.Empty;
    }
  }
}