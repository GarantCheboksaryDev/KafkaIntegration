using System;
using Sungero.Core;

namespace vf.SAPIntegration.Constants
{
  public static class Module
  {
    /// <summary>
    /// Гуиды ролей.
    /// </summary>
    public static class RoleGuid
    {
      // GUID роли "Служба безопасности".
      [Sungero.Core.Public]
      public static readonly Guid SecurityService = Guid.Parse("6579cbf7-b488-40d6-9b3d-eb706ee0764f");
    }
  }
}