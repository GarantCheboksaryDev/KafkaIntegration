using System;
using Sungero.Core;

namespace vf.IntegrationSettings.Constants
{
  public static class ConnectSettings
  {
    /// <summary>
    /// Наименование топика для проверки подключения.
    /// </summary>
    [Public]
    public const string CheckConnectionTopicName = "logs_broker";
  }
}