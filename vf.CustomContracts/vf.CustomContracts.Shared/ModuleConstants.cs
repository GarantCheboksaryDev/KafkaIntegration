using System;
using Sungero.Core;

namespace vf.CustomContracts.Constants
{
  public static class Module
  {
    /// <summary>
    /// Guid Типов документов.
    /// </summary>
    public static class DocumentTypes
    {
      /// <summary>
      /// Приложение к документу.
      /// </summary>
      [Sungero.Core.Public]
      public static readonly Guid Addendum = Guid.Parse("58b9ed35-9c84-46cd-aa79-9b5ef5a82f5d");
    }
    
    /// <summary>
    /// Guid видов документов.
    /// </summary>
    public static class DocumentKinds
    {
      /// <summary>
      /// Протокол разногласий.
      /// </summary>
      [Sungero.Core.Public]
      public static readonly Guid ProtocolDisagreement = Guid.Parse("6443088F-4E83-44D4-874F-66F3C5671BD1");
      
      /// <summary>
      /// Протокол согласования разногласий.
      /// </summary>
      [Sungero.Core.Public]
      public static readonly Guid ProtocolReconcilingDisagreement = Guid.Parse("005BCC11-6E77-432B-9367-7D8FBF498B2D");
      
      /// <summary>
      /// Протокол урегулирования разногласий.
      /// </summary>
      [Sungero.Core.Public]
      public static readonly Guid ProtocolDisputeResolution = Guid.Parse("FA568E09-056B-49BD-B246-44B76F890F75");
    }
    
    /// <summary>
    /// Sid типов оплаты.
    /// </summary>
    public static class PaymentType
    {
      /// <summary>
      /// Sid типа оплаты доставки "Предоплата".
      /// </summary>
      [Public]
      public const string Prepayment = "E8F8690D-44E3-41EA-BD17-B8FB7FD90491";
      
      /// <summary>
      /// Sid типа оплаты доставки "Постоплата".
      /// </summary>
      [Public]
      public const string Postpay = "900AB21E-B460-47FA-A839-CDADCEC52190";
    }
    
    /// <summary>
    /// Sid Методов доставки.
    /// </summary>
    public static class MailDeliveryMethod
    {
      /// <summary>
      /// Sid метода доставки "Сервис эл. обмена".
      /// </summary>
      [Public]
      public const string Exchange = "267c030c-a93a-44d8-ba60-17d8b56ad9c8";
    }
    
    /// <summary>
    /// Буквенный код валюты Рубль.
    /// </summary>
    [Public]
    public const string RubAlphaCode = "RUB";

    /// <summary>
    /// Параметры Xml-файла с сайта ЦБ РФ.
    /// </summary>
    public static class RatesXmlParams
    {
      /// <summary>
      /// Раздел Xml-файла, хранящий информацию о всех валютах.
      /// </summary>
      public const string ValCurs = "ValCurs";
      
      /// <summary>
      /// Параметр Xml-файла, хранящий информацию о конкретной валюте.
      /// </summary>
      public const string Valute = "Valute";
      
      /// <summary>
      /// Параметр Xml-файла, хранящий информацию о цифровом коде валюты.
      /// </summary>
      public const string NumCode = "NumCode";
      
      /// <summary>
      /// Параметр Xml-файла, хранящий информацию о курсе валюты.
      /// </summary>
      public const string Value = "Value";
    }
    
    /// <summary>
    /// Guid ролей.
    /// </summary>
    public static class Roles
    {
      /// <summary>
      /// Guid роли "Финансовый контроль".
      /// </summary>
      [Public]
      public static readonly Guid FinancialControlRole = Guid.Parse("C32E918A-164F-4BBB-842A-8DF998DD7F10");
      
      /// <summary>
      /// Guid роли "Сотрудники отдела документационного обеспечения".
      /// </summary>
      public static readonly Guid DocSupportDepartment = Guid.Parse("B8AC1E88-628C-4CEA-B50A-BC49E56F5AB4");
      
      /// <summary>
      /// Guid роли "ЦКР Валютный контроль".
      /// </summary>
      [Public]
      public static readonly Guid CurrencyControlRole = Guid.Parse("A03D81EE-B7A4-45D5-9727-E6538E035EB8");
      
      /// <summary>
      /// Guid роли "Сотрудник для отправки задания о постановке на валютный контроль".
      /// </summary>
      public static readonly Guid CurrencyControlAuthorRole = Guid.Parse("b53e2207-3500-4cd9-a56e-470242bd348a");
      
      /// <summary>
      /// Guid роли "Пользователи с правами на справочник рейсы".
      /// </summary>
      public static readonly Guid UsersWithRightsToSeaVoyagesRole = Guid.Parse("BAA1203D-8736-465B-A7A2-A5A7672EB74D");
      
      /// <summary>
      /// Guid роли "Методолог ЦКР".
      /// </summary>
      [Public]
      public static readonly Guid MethodologistCentralResearchCenterRole = Guid.Parse("3A91938C-FDCA-4817-BCDA-29EBB5EDEA62");
    }
    
    /// <summary>
    /// Параметры.
    /// </summary>
    public static class Params
    {
      /// <summary>
      /// Наименование параметра, хранящего текст хинта "Доверенность подписывающего действительна с <Дата> по <Дата>".
      /// </summary>
      [Public]
      public const string PowerOfAttoneyHintText = "PowerOfAttoneyHintText";
      
      /// <summary>
      /// Наименование параметра "Является сотрудником отдела документационного обеспечения".
      /// </summary>
      [Public]
      public const string IsDocSupportEmployee = "IsDocSupportEmployee";
      
      /// <summary>
      /// Наименование параметра "Сотрудник, входящий в роль Сотрудник для отправки задания о постановке на валютный контроль и его замещающие".
      /// </summary>
      [Public]
      public const string IncludeInCurrencyControlPerformerRole = "IncludeInCurrencyControlPerformerRole";
      
      /// <summary>
      /// Наименование параметра "Сотрудник, входящий в роль Методолог ЦКР и его замещающие".
      /// </summary>
      [Public]
      public const string IncludeInMethodologistCentralResearchCenterRole = "IncludeInMethodologistCentralResearchCenterRole";
    }

    /// <summary>
    /// Количество дней, проверяемое при выводе хинта "Доверенность подписывающего действительна с <Дата> по <Дата>" в карточке договора, доп. соглашения и задаче на согласование по регламенту.
    /// </summary>
    public const int AmountOfDaysForCheckingPowerOfAttoney = 30;
    
    /// <summary>
    /// Максимальное количество записей для обработки.
    /// </summary>
    /// HACK: Чтобы не получать ошибку Large Fetches.
    public const int MaxRecordCountProcessing = 1000;
  }
}