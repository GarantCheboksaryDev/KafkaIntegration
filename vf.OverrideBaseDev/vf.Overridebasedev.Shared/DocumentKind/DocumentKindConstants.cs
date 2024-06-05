using System;
using Sungero.Core;

namespace vf.OverrideBaseDev.Constants.Docflow
{
  public static class DocumentKind
  {
    /// <summary>
    /// Guid вида документа "Анкета контрагента".
    /// </summary>
    public const string QuestionnaireKindGuid = "CBABD410-1220-4CFB-825E-5261D835B967";
    
    /// <summary>
    /// Guid ТД Сведения о контрагенте.
    /// </summary>
    public const string CounterpartyDocumentGuid = "49d0c5e7-7069-44d2-8eb6-6e3098fc8b10";

    /// <summary>
    /// Guid ТД Контракт.
    /// </summary>
    public const string ContractGuid = "f37c7e63-b134-4446-9b5b-f8811f6c9666";
    
    /// <summary>
    /// Guid ТД Дополнительное соглашение.
    /// </summary>
    public const string SupAgreementGuid = "265f2c57-6a8a-4a15-833b-ca00e8047fa5";
    
    /// <summary>
    /// Guid ТД Товарная накладная.
    /// </summary>
    public const string WaybillGuid = "4e81f9ca-b95a-4fd4-bf76-ea7176c215a7";
    
    /// <summary>
    /// Guid ТД Товарная накладная (цифровая бухгалтерия).
    /// </summary>
    public const string ComparisonWaybillGuid = "cafbccc1-a94d-442f-af86-b8d44dbe0204";
    
    /// <summary>
    /// Guid ТД Акт выполненных работ.
    /// </summary>
    public const string ContractStatementGuid = "f2f5774d-5ca3-4725-b31d-ac618f6b8850";
    
    /// <summary>
    /// Guid ТД Универсальный передаточный документ.
    /// </summary>
    public const string UTDGuid = "58986e23-2b0a-4082-af37-bd1991bc6f7e";
  }
}