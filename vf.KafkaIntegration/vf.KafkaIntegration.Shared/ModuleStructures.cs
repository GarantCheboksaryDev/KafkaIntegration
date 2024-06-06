using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.KafkaIntegration.Structures.Module
{
  /// <summary>
  /// Структура, содержащая информацию о персонах из брокера Kafka.
  /// </summary>
  [Public(Isolated=true)]
  partial class PersonFromKafka
  {
    /// <summary>
    /// Guid 1C Банка.
    /// </summary>
    public string Guid1C { get; set; }
    /// <summary>
    /// Код HCM физ. лица.
    /// </summary>
    public string HCMCode { get; set; }
    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; set; }
    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; set; }
    /// <summary>
    /// Отчество.
    /// </summary>
    public string MiddleName { get; set; }
    /// <summary>
    /// Дата рождения.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }
    /// <summary>
    /// Пол.
    /// </summary>
    public string Gender { get; set; }
    /// <summary>
    /// ИНН.
    /// </summary>
    public string INN { get; set; }
    /// <summary>
    /// СНИЛС.
    /// </summary>
    public string SNILS { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
  }
  
  /// <summary>
  /// Структура, содержащая информацию о сотрудниках из брокера Kafka.
  /// </summary>
  [Public(Isolated=true)]
  partial class EmployeesFromKafka
  {
    /// <summary>
    /// Guid 1C Сотрудника.
    /// </summary>
    public string Guid1C { get; set; }
    /// <summary>
    /// Табельный номер.
    /// </summary>
    public string PersonnelNumber { get; set; }
    /// <summary>
    /// Guid 1C Персоны.
    /// </summary>
    public string PersonGuid { get; set; }
    /// <summary>
    /// Guid 1C Подразделения.
    /// </summary>
    public string DepartmentGuid { get; set; }
    /// <summary>
    /// Guid 1C Должности.
    /// </summary>
    public string JobTitleGuid { get; set; }
    /// <summary>
    /// Наименование должности.
    /// </summary>
    public string JobTitleName { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Fired_flag { get; set; }
    /// <summary>
    /// Код организации.
    /// </summary>
    public string Organization { get; set; }
  }
  
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
    /// GUID 1C.
    /// </summary>
    public string GUID1C { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
  }
  
  /// <summary>
  /// Структура, содержащая информацию о рейсах из брокера Kafka.
  /// </summary>
  [Public(Isolated=true)]
  partial class VoyagesFromKafka
  {
    /// <summary>
    /// Номер рейса.
    /// </summary>
    public string Number { get; set; }
    /// <summary>
    /// Guid 1C рейса.
    /// </summary>
    public string Guid1C { get; set; }
    /// <summary>
    /// Начало рейса.
    /// </summary>
    public DateTime? Start_date { get; set; }
    /// <summary>
    /// Завершение рейса.
    /// </summary>
    public DateTime? End_date { get; set; }
    /// <summary>
    /// Код МВЗ судна.
    /// </summary>
    public string Vessel_MVZ_code { get; set; }
    /// <summary>
    /// Id договоров.
    /// </summary>
    public List<string> ContractIds { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
    /// <summary>
    /// Код организации.
    /// </summary>
    public string Organization { get; set; }
  }
  
  /// <summary>
  /// /// Структура, содержащая информацию о контрагентах из брокера Kafka.
  /// /// </summary>
  [Public(Isolated=true)]
  partial class CounterpartiesFromKafka
  {
    /// <summary>
    /// Код системы MDG.
    /// </summary>
    public string InternalId { get; set; }
    /// <summary>
    /// Уникальный идентификатор банка в 1С.
    /// </summary>
    public string Guid1C { get; set; }
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
    /// Международное наименование.
    /// </summary>
    public string InternationalName { get; set; }
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
    /// Налоговый номер.
    /// </summary>
    public string TaxIdentificationNumberTypeCode { get; set; }
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
  
  /// <summary>
  /// /// Структура, содержащая информацию о расчетных счетах из брокера Kafka.
  /// /// </summary>
  [Public(Isolated=true)]
  partial class PaymentAccountFromKafka
  {
    /// <summary>
    /// Код системы MDG.
    /// </summary>
    public string InternalId { get; set; }
    /// <summary>
    /// GUID 1C.
    /// </summary>
    public string Guid1C { get; set; }
    /// <summary>
    /// GUID 1C банка.
    /// </summary>
    public string GuidBank1C { get; set; }
    /// <summary>
    /// Расчетный счет.
    /// </summary>
    public string BankControlKey { get; set; }
    /// <summary>
    /// GUID 1C контрагента.
    /// </summary>
    public string GuidCounterparty1C { get; set; }
    /// <summary>
    /// Валюта.
    /// </summary>
    public string Currency { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
  }
  
  /// <summary>
  /// /// Структура, содержащая информацию о статьях бюджета из брокера Kafka.
  /// /// </summary>
  [Public(Isolated=true)]
  partial class BudgetItemFromKafka
  {
    /// <summary>
    /// GUID 1C.
    /// </summary>
    public string Guid1C { get; set; }
    /// <summary>
    /// Наименование статьи.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Тип договора.
    /// </summary>
    public string ContractType { get; set; }
    /// <summary>
    /// Тип статьи.
    /// </summary>
    public string ArticleType { get; set; }
    /// <summary>
    /// Первый уровень.
    /// </summary>
    public string FirstLevel { get; set; }
    /// <summary>
    /// Второй уровень.
    /// </summary>
    public string SecondLevel { get; set; }
    /// <summary>
    /// Третий уровень.
    /// </summary>
    public string ThreeLevel { get; set; }
    /// <summary>
    /// Четвертый уровень.
    /// </summary>
    public string FourLevel { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
  }
  
  /// <summary>
  /// Структура, содержащая информацию о подразделениях из брокера Kafka.
  /// </summary>
  [Public(Isolated=true)]
  partial class DepartmentsFromKafka
  {
    /// <summary>
    /// Guid 1C.
    /// </summary>
    public string Guid1C { get; set; }
    /// <summary>
    /// Наименование gjlhfpltktybz.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Guid головного подразделения.
    /// </summary>
    public string HeadDepartment { get; set; }
    /// <summary>
    /// Код HCM нашей организации.
    /// </summary>
    public string Organization { get; set; }
    /// <summary>
    /// Код HCM подразделения.
    /// </summary>
    public string HCMCode { get; set; }
    /// <summary>
    /// Признак, что подразделение является судном.
    /// </summary>
    public bool Ship_flag { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
  }
  
  /// <summary>
  /// Структура, содержащая информацию о ЦФО/МВЗ из брокера Kafka.
  /// </summary>
  [Public(Isolated=true)]
  partial class CompanyStructuresFromKafka
  {
    /// <summary>
    /// Guid 1C.
    /// </summary>
    public string Guid1C { get; set; }
    /// <summary>
    /// Наименование ЦФО/МВЗ.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Код HCM нашей организации.
    /// </summary>
    public string Organization { get; set; }
    /// <summary>
    /// Код МВЗ.
    /// </summary>
    public string MVZCode { get; set; }
    /// <summary>
    /// Код ЦФО.
    /// </summary>
    public string CFOCode { get; set; }
    /// <summary>
    /// Признак, что передан ЦФО.
    /// </summary>
    public bool CFO_flag { get; set; }
    /// <summary>
    /// Признак судна.
    /// </summary>
    public bool Ship_flag { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
  }
  
  /// <summary>
  /// Структура, содержащая информацию о регистре связи ЦФО-Статьи БК.
  /// </summary>
  [Public(Isolated=true)]
  partial class CFOArticleRegisterFromKafka
  {
    /// <summary>
    /// Guid 1C ЦФО.
    /// </summary>
    public string CFO { get; set; }
    /// <summary>
    /// Guid 1C Статьи БК.
    /// </summary>
    public List<string> Article { get; set; }
  }
  
  /// <summary>
  /// Структура, содержащая информацию о регистре связи Подразделение-ЦФО-МВЗ-Менеджер.
  /// </summary>
  [Public(Isolated=true)]
  partial class DepartmentRegisterFromKafka
  {
    /// <summary>
    /// Guid 1C подразделения.
    /// </summary>
    public string Department { get; set; }
    /// <summary>
    /// Guid 1C руководителя подразделения.
    /// </summary>
    public string Manager { get; set; }
    /// <summary>
    /// Guid 1C МВЗ.
    /// </summary>
    public string MVZ { get; set; }
    /// <summary>
    /// Guid 1C ЦФО.
    /// </summary>
    public string CFO { get; set; }

  }
  
  /// <summary>
  /// Структура для передачи информации о договорах в броке Kafka.
  /// </summary>
  [Public(Isolated=true)]
  partial class ContractToKafka
  {
    /// <summary>
    /// Ид договора.
    /// </summary>
    public string SedId { get; set; }
    /// <summary>
    /// Гиперссылка на договор.
    /// </summary>
    public string Hyperlink { get; set; }
    /// <summary>
    /// Номер регистрации.
    /// </summary>
    public string RegistrationNumber { get; set; }
    /// <summary>
    /// Дата регистрации.
    /// </summary>
    public DateTime? RegistrationDate { get; set; }
    /// <summary>
    /// Дата создания.
    /// </summary>
    public DateTime? Created { get; set; }
    /// <summary>
    /// Комментарий к договору.
    /// </summary>
    public string Content { get; set; }
    /// <summary>
    /// Дата начала.
    /// </summary>
    public DateTime? Start_date { get; set; }
    /// <summary>
    /// Дата окончания.
    /// </summary>
    public DateTime? End_date { get; set; }
    /// <summary>
    /// Автопролонгация.
    /// </summary>
    public bool IsAutoRenewable { get; set; }
    /// <summary>
    /// Менеджер.
    /// </summary>
    public string ResponsibleEmployee { get; set; }
    /// <summary>
    /// Подразделение.
    /// </summary>
    public string Department { get; set; }
    /// <summary>
    /// Ставка НДС.
    /// </summary>
    public double VatRate { get; set; }
    /// <summary>
    /// Контрагент.
    /// </summary>
    public string Counterparty { get; set; }
    /// <summary>
    /// Состояние.
    /// </summary>
    public string GetSAPState { get; set; }
    /// <summary>
    /// Статус.
    /// </summary>
    public string GetSAPStatus { get; set; }
    /// <summary>
    /// Наименование условия оплаты.
    /// </summary>
    public string PaymentTermsName { get; set; }
    /// <summary>
    /// Код условия оплаты.
    /// </summary>
    public string PaymentTermsCode { get; set; }
    /// <summary>
    /// Тип взаимоотношений.
    /// </summary>
    public string ContractType { get; set; }
    /// <summary>
    /// Инкотермс.
    /// </summary>
    public string IncotermsKind { get; set; }
    /// <summary>
    /// Валюта.
    /// </summary>
    public string Currency { get; set; }
    /// <summary>
    /// Валюта платежа.
    /// </summary>
    public System.Collections.Generic.List<string> PaymentCurrencies { get; set; }
    /// <summary>
    /// Сумма договора.
    /// </summary>
    public double? TotalAmount { get; set; }
    /// <summary>
    /// Сумма договора без НДС.
    /// </summary>
    public double? NetAmount { get; set; }
    /// <summary>
    /// Сумма НДС.
    /// </summary>
    public double? VatAmount { get; set; }
    /// <summary>
    /// УНК.
    /// </summary>
    public string ContractUniqueNumber { get; set; }
    /// <summary>
    /// Дополнительное соглашение.
    /// </summary>
    public bool IsSupAgreement { get; set; }
    /// <summary>
    /// Тип взаимоотношений.
    /// </summary>
    public string MainContract { get; set; }
    /// <summary>
    /// Номер договора контрагента.
    /// </summary>
    public string CounterpartyRegistrationNumber { get; set; }
    /// <summary>
    /// Дата договора.
    /// </summary>
    public DateTime? ContractDate { get; set; }
    /// <summary>
    /// Вид договора.
    /// </summary>
    public string DocumentKind { get; set; }
    /// <summary>
    /// Договоры в 1С.
    /// </summary>
    public string ErpDatabookName { get; set; }
    /// <summary>
    /// Код общества.
    /// </summary>
    public string BusinessUnit { get; set; }
    /// <summary>
    /// Банк.
    /// </summary>
    public string OwnBank { get; set; }
    /// <summary>
    /// Наш Р/С.
    /// </summary>
    public string OwnPaymentAccount { get; set; }
    /// <summary>
    /// Р/С контрагента.
    /// </summary>
    public string CounterpartyPaymentAccount { get; set; }
    /// <summary>
    /// Р/С контрагента.
    /// </summary>
    public List<string> AdditionalCounterParty { get; set; }
    /// <summary>
    /// Событие окончания договора.
    /// </summary>
    public string ValidFromDeadLine { get; set; }
    /// <summary>
    /// Событие окончания договора.
    /// </summary>
    public string ValidFromAction { get; set; }
    /// <summary>
    /// Автор документа.
    /// </summary>
    public string Author { get; set; }
    /// <summary>
    /// Обеспечение.
    /// </summary>
    public string Provision { get; set; }
    /// <summary>
    /// Налоговые агенты.
    /// </summary>
    public string TaxAgent { get; set; }
    /// <summary>
    /// Тип банковской комиссии.
    /// </summary>
    public string BankChargeType { get; set; }
    /// <summary>
    /// Условие определния базовой даты платежа.
    /// </summary>
    public string ConditionBasePayment { get; set; }
    /// <summary>
    /// Типы применяемых закупочных процедур.
    /// </summary>
    public string PurchasingType { get; set; }
    /// <summary>
    /// Обеспечение.
    /// </summary>
    public DateTime? AdvanceClosedDate { get; set; }
    /// <summary>
    /// Отметка о наличии сданного оригинала подписанного договора.
    /// </summary>
    public bool OriginalStock { get; set; }
    /// <summary>
    /// Права доступа к документу.
    /// </summary>
    public List<string> AccessRights { get; set; }
    /// <summary>
    /// Регистрационный номер равен договору.
    /// </summary>
    public bool RegNumEqualContractNumber { get; set; }
    /// <summary>
    /// Номер проекта.
    /// </summary>
    public string ProjectNumber { get; set; }
    /// <summary>
    /// Признак конфиденциальности.
    /// </summary>
    public bool Confidential { get; set; }
  }
  
  /// <summary>
  /// /// Структура, содержащая информацию о расчетных счетах наших организаций из брокера Kafka.
  /// /// </summary>
  [Public(Isolated=true)]
  partial class OurPaymentAccountFromKafka
  {
    /// <summary>
    /// Код системы MDG.
    /// </summary>
    public string InternalId { get; set; }
    /// <summary>
    /// GUID 1C.
    /// </summary>
    public string Guid1C { get; set; }
    /// <summary>
    /// GUID 1C банка.
    /// </summary>
    public string GuidBank1C { get; set; }
    /// <summary>
    /// Расчетный счет.
    /// </summary>
    public string BankControlKey { get; set; }
    /// <summary>
    /// Код нашей организации.
    /// </summary>
    public string Organization { get; set; }
    /// <summary>
    /// Валюта.
    /// </summary>
    public string Currency { get; set; }
    /// <summary>
    /// Действующий / Закрытый.
    /// </summary>
    public bool Deleted_flag { get; set; }
  }
}