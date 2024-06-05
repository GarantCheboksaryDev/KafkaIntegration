using System;
using Sungero.Core;

namespace vf.CustomParties.Constants
{
  public static class Module
  {
    /// <summary>
    /// Sid результататов проверок СБ.
    /// </summary>
    public static class ServiceSecurityCheckResult
    {
      /// <summary>
      /// Согласован.
      /// </summary>
      public const string Approved = "36960E33-2BA1-4BBF-9F56-F665F5F3AC3B";
      
      /// <summary>
      /// Не рекомендован.
      /// </summary>
      public const string NotRecommended = "92D793C6-1A19-49DC-A538-1242975544D7";
      
      /// <summary>
      /// Не согласован.
      /// </summary>
      public const string NotApproved = "026ED180-2AD9-4A61-B905-A7ADE48AAFB4";
    }
  }
}