using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomParties.ServiceSecurityCheckResult;

namespace vf.CustomParties.Server
{
  partial class ServiceSecurityCheckResultFunctions
  {
    /// <summary>
    /// Получить "Наименования справочника в 1С:ERP" по Sid.
    /// </summary>
    /// <returns>Результат проверки.</returns>
    [Remote(IsPure = true), Public]
    public static IServiceSecurityCheckResult GetServiceSecurityCheckResultBySid(string sid)
    {
      return ServiceSecurityCheckResults.GetAll(x => x.Sid == sid).FirstOrDefault();
    }
  }
}