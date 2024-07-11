using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.KafkaIntegration.Structures.Module
{   
  /// <summary>
  /// Структура, содержащая информацию о банках из брокера Kafka.
  /// </summary>
  [Public(Isolated=true)]
  partial class BanksFromKafka
  {
    /// <summary>
    /// Страна банка.
    /// </summary>
    public string Bank_ctry { get; set; }
    /// <summary>
    /// БИК / SWIFT.
    /// </summary>
    public string Bank_key { get; set; }
    /// <summary>
    /// Наименование банка.
    /// </summary>
    public string Bank_name { get; set; }
    /// <summary>
    /// Улица банка.
    /// </summary>
    public string Street { get; set; }
    /// <summary>
    /// SWIFT.
    /// </summary>
    public string Swift_Code { get; set; }
    /// <summary>
    /// Корр. счет.
    /// </summary>
    public string Bank_Branch { get; set; }
    /// <summary>
    /// Внешняя ссылка системы.
    /// </summary>
    public string ExternalLink { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
  }

  /// <summary>
  /// /// Структура, содержащая информацию о контрагентах из брокера Kafka.
  /// /// </summary>
  [Public(Isolated=true)]
  partial class CounterpartiesFromKafka
  {
    /// <summary>
    /// Внешняя ссылка системы.
    /// </summary>
    public string ExternalLink { get; set; }
    /// <summary>
    /// Признак контрагента.
    /// </summary>
    public string Common { get; set; }
    /// <summary>
    /// Юридическое наименование.
    /// </summary>
    public string Longtextlnam { get; set; }
    /// <summary>
    /// Наименование.
    /// </summary>
    public string Longtext { get; set; }
    /// <summary>
    /// ИНН.
    /// </summary>
    public string INN { get; set; }
    /// <summary>
    /// КПП.
    /// </summary>
    public string KPP { get; set; }
    /// <summary>
    /// ОГРН.
    /// </summary>
    public string OGRN { get; set; }
    /// <summary>
    /// Регистрационный номер страны.
    /// </summary>
    public string CountryCode { get; set; }
    /// <summary>
    /// Юридический адрес.
    /// </summary>
    public string LegalAddress { get; set; }
    /// <summary>
    /// Почтовый адрес.
    /// </summary>
    public string PostalAddress { get; set; }
    /// <summary>
    /// Телефоны.
    /// </summary>
    public string Phones { get; set; }
    /// <summary>
    /// Электронная почта.
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
  }
}