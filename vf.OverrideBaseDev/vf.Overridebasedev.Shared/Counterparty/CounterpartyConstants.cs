using System;
using Sungero.Core;

namespace vf.OverrideBaseDev.Constants.Parties
{
  public static class Counterparty
  {
    /// <summary>
    /// Имена параметров.
    /// </summary>
    public static class ParamNames
    {
      public const string IncludeInSecurityServiceParamName = "IncludeInSecurityServiceParamName";
    }
    
    /// <summary>
    /// Параметры, при которых в карточке организации проставляется свойство "Нерезидент".
    /// </summary>
    public static class NonResidentParams
    {
      public const string TIN = "9909";
      
      public const string CompanyMDGCode = "2000";
      
      public const string BankMDGCode = "3000";
    }
  }
}