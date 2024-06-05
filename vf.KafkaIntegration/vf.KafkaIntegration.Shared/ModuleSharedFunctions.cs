using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.KafkaIntegration.Shared
{
  public class ModuleFunctions
  {
    /// <summary>
    /// Проверить, является ли Id - Id директума.
    /// </summary>
    /// <param name="Id">Id.</param>
    /// <returns>True, если Id является Id директума. Иначе - False.</returns>
    [Public]
    public bool IsDirectumId(string Id)
    {
      return Id.Contains(Constants.Module.SystemCodes.DirectumIdPrefix);
    }
    
    /// <summary>
    /// Очистить от префикса Id, переданный из 1С.
    /// </summary>
    /// <param name="Id">Id с префиксом.</param>
    /// <returns>Id без префикса.</returns>
    [Public]
    public string RemovePrefixFromId(string Id)
    {
      return Id.Replace(Constants.Module.SystemCodes.DirectumIdPrefix, string.Empty);
    }

  }
}