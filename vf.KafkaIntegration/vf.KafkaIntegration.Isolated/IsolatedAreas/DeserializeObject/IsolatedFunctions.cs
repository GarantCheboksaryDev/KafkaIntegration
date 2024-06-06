using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sungero.Core;
using vf.KafkaIntegration.Structures.Module;

namespace vf.KafkaIntegration.Isolated.DeserializeObject
{
  public class IsolatedFunctions
  {
    /// <summary>
    /// Десериализовать Json с информацией о банке.
    /// </summary>
    /// <returns>Экземпляр банка.</returns>
    [Public]
    public virtual Structures.Module.IBanksFromKafka DesirializeBankInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.BanksFromKafka>(data.ToString());
      }
      
      return Structures.Module.BanksFromKafka.Create();
    }
    
    /// <summary>
    /// Десериализовать Json с информацией о контрагенте.
    /// </summary>
    /// <returns>Экземпляр контрагента.</returns>
    [Public]
    public virtual Structures.Module.ICounterpartiesFromKafka DesirializeCounterpartiesInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.CounterpartiesFromKafka>(data.ToString());
      }
      
      return Structures.Module.CounterpartiesFromKafka.Create();
    }
  }
}