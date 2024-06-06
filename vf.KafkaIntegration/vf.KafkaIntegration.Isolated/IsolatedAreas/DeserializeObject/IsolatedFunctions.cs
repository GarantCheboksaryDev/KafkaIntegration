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
    /// Десериализовать Json с информацией о персоне.
    /// </summary>
    /// <returns>Экземпляр персоны.</returns>
    [Public]
    public virtual Structures.Module.IPersonFromKafka DesirializePersonInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.PersonFromKafka>(data.ToString());
      }
      
      return Structures.Module.PersonFromKafka.Create();
    }
    
    /// <summary>
    /// Десериализовать Json с информацией о сотруднике.
    /// </summary>
    /// <returns>Экземпляр сотруника.</returns>
    [Public]
    public virtual Structures.Module.IEmployeesFromKafka DesirializeEmployeeInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.EmployeesFromKafka>(data.ToString());
      }
      
      return Structures.Module.EmployeesFromKafka.Create();
    }
    
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
    /// Десериализовать Json с информацией о рейсе.
    /// </summary>
    /// <returns>Экземпляр рейса.</returns>
    [Public]
    public virtual Structures.Module.IVoyagesFromKafka DesirializeVoyageInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.VoyagesFromKafka>(data.ToString());
      }
      
      return Structures.Module.VoyagesFromKafka.Create();
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
    
    /// <summary>
    /// Десериализовать Json с информацией о расчетном счете.
    /// </summary>
    /// <returns>Экземпляр контрагента.</returns>
    [Public]
    public virtual Structures.Module.IPaymentAccountFromKafka DesirializePaymentAccountsInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.PaymentAccountFromKafka>(data.ToString());
      }
      
      return Structures.Module.PaymentAccountFromKafka.Create();
    }
    
    /// <summary>
    /// Десериализовать Json с информацией о расчетном счете наших организаций.
    /// </summary>
    /// <returns>Экземпляр контрагента.</returns>
    [Public]
    public virtual Structures.Module.IOurPaymentAccountFromKafka DesirializeOurPaymentAccountsInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.OurPaymentAccountFromKafka>(data.ToString());
      }
      
      return Structures.Module.OurPaymentAccountFromKafka.Create();
    }
    
    /// <summary>
    /// Десериализовать Json с информацией о статьях бюджета.
    /// </summary>
    /// <returns>Экземпляр контрагента.</returns>
    [Public]
    public virtual Structures.Module.IBudgetItemFromKafka DesirializeBudgetItemsInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.BudgetItemFromKafka>(data.ToString());
      }
      
      return Structures.Module.BudgetItemFromKafka.Create();
    }
    
    /// <summary>
    /// Десериализовать Json с информацией о подразделении.
    /// </summary>
    /// <returns>Экземпляр подразделения.</returns>
    [Public]
    public virtual Structures.Module.IDepartmentsFromKafka DesirializeDepartmentInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.DepartmentsFromKafka>(data.ToString());
      }
      
      return Structures.Module.DepartmentsFromKafka.Create();
    }
    
    /// <summary>
    /// Десериализовать Json с информацией о ЦФО/МВЗ.
    /// </summary>
    /// <returns>Экземпляр ЦФО/МВЗ.</returns>
    [Public]
    public virtual Structures.Module.ICompanyStructuresFromKafka DesirializeCompanyStructureInfo(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.CompanyStructuresFromKafka>(data.ToString());
      }
      
      return Structures.Module.CompanyStructuresFromKafka.Create();
    }
    
    /// <summary>
    /// Разделить Json с информацией о регистре связи на отдельные сообщения.
    /// </summary>
    /// <returns>Список сообщений регистра связи.</returns>
    [Public]
    public virtual List<string> SplitRegister(string jsonValue)
    {
      var register = new List<string>();
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        var data = jObject["data"];
        if (data.HasValues)
          foreach (var item in data)
            register.Add(item.ToString());
      }
      
      return register;
    }
    
    /// <summary>
    /// Десериализовать Json с информацией о связи ЦФО-Статьи БК.
    /// </summary>
    /// <returns>Экземпляр связи ЦФО-Статьи БК.</returns>
    [Public]
    public virtual Structures.Module.ICFOArticleRegisterFromKafka DesirializeCFOArticleItem(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        if (jObject.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.CFOArticleRegisterFromKafka>(jObject.ToString());
      }
      
      return Structures.Module.CFOArticleRegisterFromKafka.Create();
    }
    
    /// <summary>
    /// Десериализовать Json с информацией о связи Подразделение-ЦФО-МВЗ-Менеджер.
    /// </summary>
    /// <returns>Экземпляр связи Подразделение-ЦФО-МВЗ-Менеджер.</returns>
    [Public]
    public virtual Structures.Module.IDepartmentRegisterFromKafka DesirializeDepartmentRegisterItem(string jsonValue)
    {
      if (!string.IsNullOrEmpty(jsonValue))
      {
        JObject jObject = JObject.Parse(jsonValue);
        if (jObject.HasValues)
          return JsonConvert.DeserializeObject<Structures.Module.DepartmentRegisterFromKafka>(jObject.ToString());
      }
      
      return Structures.Module.DepartmentRegisterFromKafka.Create();
    }
    
    /// <summary>
    /// Сериализовать Json с информацией о договоре.
    /// </summary>
    /// <param name="contractInfo">Структура с информацией о договоре.</param>
    /// <returns>Тело Json.</returns>
    [Public]
    public virtual string SerializeContractInfo(Structures.Module.IContractToKafka contractInfo)
    {
      return JsonConvert.SerializeObject(contractInfo);
    }
  }
}