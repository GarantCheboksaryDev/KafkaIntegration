using System;
using Sungero.Core;

namespace vf.KafkaIntegration.Constants
{
  public static class Module
  {
    /// <summary>
    /// Системные коды.
    /// </summary>
    public static class SystemCodes
    {
      public const string ContractTypeOutgoing = "Outgoing";
      
      /// <summary>
      /// Псевдо Гуид для 1С.
      /// </summary>
      public const string PseudoGuid = "00000000-0000-0000-0000-000000000000";
      
      /// <summary>
      /// Префикс Id директума, хранящийся в 1C.
      /// </summary>
      [Public]
      public const string DirectumIdPrefix = "_DRX";
      
      /// <summary>
      /// Код Альфа 3 для России.
      /// </summary>
      public const string RuAlphaCode3 = "RUS";
      
      /// <summary>
      /// Признак, что КА - Физ.лицо.
      /// </summary>
      public const string PersonReferenceName = "Person";
    }

    /// <summary>
    /// Максимальное количество попыток обработки сообщения из Kafka.
    /// </summary>
    [Public]
    public const int MaxRetriesAmount = 5;
    
    /// <summary>
    /// Регулярное выражение для ФИО.
    /// </summary>
    public const string FIOPattern = @"^(\S+)(?<!\.)\s*(\S+)(?<!\.)\s*(\S*)(?<!\.)$";
  }
}