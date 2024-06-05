using System;
using Sungero.Core;

namespace vf.OverrideBaseDev.Constants.Contracts
{
  public static class ContractualDocument
  {
    /// <summary>
    /// Наименование параметров.
    /// </summary>
    public static class ParamNames
    {
      // Наименование параметра, хранящего признак того, что поле "Действует по" в карточке договора доступно и обязательно для заполнения.
      public const string IsValidTillEnabled = "IsValidTillEnabled";
      
      // Наименование параметра, хранящего признак того, что поле "Событие" в карточке договора доступно и обязательно для заполнения.
      public const string IsEventNameEnabled = "IsEventNameEnabled";
      
      // Наименование параметра "Из последней номер версии".
      public const string FromLastVersionNumber = "FromLastVersionVersionNumber";
      
      // Наименование параметра, хранящего признак того, что поле "Инкотремс" в карточке договора доступно и обязательно для заполнения.
      public const string IsIncotermsEnabled = "IsIncotermsEnabled";
      
      // Наименование параметра, хранящего признак того, что поле "Налоговый агент" в карточке договора доступно и обязательно для заполнения.
      public const string IsTaxAgentEnabled = "IsTaxAgentEnabled";
      
      // Наименование параметра, хранящего признак того, что поле "Ответственный по договору" в карточке договора доступно и обязательно для заполнения.
      public const string IsProjectInitiatorEnabled = "IsProjectInitiatorEnabled";
      
       // Наименование параметра, хранящего признак того, что текущий сотрудник входит в роль "Администратор".
      public const string IsAdministrator = "IsAdministrator";
      
      // Наименование параметра, хранящего признак того, что поле "Рейс" в карточке договора доступно и обязательно для заполнения.
      public const string IsVoyageEnabled = "IsVoyageEnabled";
      
      // Наименование параметра, хранящего признак того, что поле "Установлен на валютный контроль" в карточке договора доступно для заполнения.
      public const string IsCurrencyControlEnabled = "IsCurrencyControlEnabled";
      
      // Наименование параметра, хранящего признак того, что поле "Дней для завершения" в карточке договора доступно для заполнения.
      public const string IsSetDaysToFinishWorksControlEnabled = "IsSetDaysToFinishWorksControlEnabled";
      
      // Наименование параметра, хранящего признак того, что поля "Дней для завершения" и "Действует по" в карточке договора обязательны для заполнения.
      public const string IsSetDaysToFinishWorksControlRequired = "IsSetDaysToFinishWorksControlRequired";
      
      // Наименование параметра, хранящего признак того, что поле текущий сотрудник входит в группу регистрации, зарегистрировавшую договор.
      public const string CurrentUserIsInRegistrationGroup = "CurrentUserIsInRegistrationGroup";
    }
    
    /// <summary>
    /// Операции с историей.
    /// </summary>
    public static class HistoryOperation
    {
      /// <summary>
      /// Наименование операции "Изменение свойства".
      /// </summary>
      [Public]
      public const string SDChange = "SDChange";
      
      /// <summary>
      /// Наименование операции "Создание".
      /// </summary>
      [Public]
      public const string Create = "Create";
      
      /// <summary>
      /// Префикс для записи ид шаблона в истории.
      /// </summary>
      [Public]
      public const string ID = "ИД:";
    }
    
    /// <summary>
    /// Коды состояния договора в SAP.
    /// </summary>
    public static class SAPState
    {
      /// <summary>
      /// Состояние "Проект".
      /// </summary>
      public const string Draft = "1";
      /// <summary>
      /// Состояние "Действует".
      /// </summary>
      public const string Active = "2";
      /// <summary>
      /// Состояние "Не действует".
      /// </summary>
      public const string NotActive = "3";
      /// <summary>
      /// Состояние "Расторгнут".
      /// </summary>
      public const string Terminated = "4";
    }
    
    /// <summary>
    /// Коды статуса договора в SAP.
    /// </summary>
    public static class SAPStatus
    {
      /// <summary>
      /// Статус "Подготовка проекта".
      /// </summary>
      public const string Draft = "10";
      /// <summary>
      /// Статус "Согласование".
      /// </summary>
      public const string OnApproval = "20";
      /// <summary>
      /// Статус "Согласован".
      /// </summary>
      public const string Approval = "30";
      /// <summary>
      /// Статус "Консолидация".
      /// </summary>
      public const string OnRework = "40";
      /// <summary>
      /// Статус "Подписание".
      /// </summary>
      public const string OnSigned = "50";
      /// <summary>
      /// Статус "Подписан".
      /// </summary>
      public const string Signed = "50";
      /// <summary>
      /// Статус "Отменен".
      /// </summary>
      public const string Canceled = "60";
      /// <summary>
      /// Статус "Регистрация".
      /// </summary>
      public const string OnRegistration = "70";
      /// <summary>
      /// Статус "Зарегистрирован".
      /// </summary>
      public const string Registered = "80";
      /// <summary>
      /// Статус "Отложенный номер".
      /// </summary>
      public const string Reserved = "81";
      /// <summary>
      /// Статус "Аннулирован".
      /// </summary>
      public const string Obsolete = "90";
    }
  }
}