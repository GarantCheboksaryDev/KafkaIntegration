using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.Docflow;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.DocumentRegister;

namespace vf.OverrideBaseDev.Server
{
  partial class DocumentRegisterFunctions
  {
    /// <summary>
    /// Получить текущий порядковый номер для журнала.
    /// </summary>
    /// <param name="date">Дата.</param>
    /// <param name="leadDocumentId">ID ведущего документа.</param>
    /// <param name="departmentId">ID подразделения.</param>
    /// <param name="businessUnitId">ID НОР.</param>
    /// <returns>Порядковый номер.</returns>
    [Public]
    public override int GetCurrentNumber(DateTime date, long leadDocumentId, long departmentId, long businessUnitId)
    {
      return base.GetCurrentNumber(date, leadDocumentId, departmentId, businessUnitId);
    }
    
    /// <summary>
    /// Получить последний или первый индекс среди документов за указанный период.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="periodBegin">Начало периода.</param>
    /// <param name="periodEnd">Конец периода.</param>
    /// <param name="orderByDescending">True - последний индекс, false - первый индекс.</param>
    /// <returns>Индекс.</returns>
    [Public]
    public static int? GetIndexCustom(IQueryable<Sungero.Docflow.IOfficialDocument> documents, DateTime? periodBegin, DateTime? periodEnd, bool orderByDescending)
    {
      return GetIndex(documents, periodBegin, periodEnd, orderByDescending);
    }
    
    /// <summary>
    /// Получить список документов, зарегистрированных в указанном периоде.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="periodBegin">Начало периода.</param>
    /// <param name="periodEnd">Конец периода.</param>
    /// <returns>Документы, зарегистрированные в промежутке между periodBegin и periodEnd.</returns>
    [Public]
    public static IQueryable<Sungero.Docflow.IOfficialDocument> FilterDocumentsByPeriodCustom(IQueryable<Sungero.Docflow.IOfficialDocument> documents,
                                                                              DateTime? periodBegin, DateTime? periodEnd)
    {
      return FilterDocumentsByPeriod(documents, periodBegin, periodEnd);
    }
  }
}