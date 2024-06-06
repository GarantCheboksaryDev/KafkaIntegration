using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Docflow;
using vf.CustomContracts.AutoRegistrationScript;

namespace vf.CustomContracts.Server
{
  partial class AutoRegistrationScriptFunctions
  {
    /// <summary>
    /// Выполнить сценарий.
    /// </summary>
    /// <param name="approvalTask">Задача на согласование по регламенту.</param>
    /// <returns>Результат выполнения сценария.</returns>
    public override Sungero.Docflow.Structures.ApprovalFunctionStageBase.ExecutionResult Execute(Sungero.Docflow.IApprovalTask approvalTask)
    {
      var result = base.Execute(approvalTask);
      var document = approvalTask.DocumentGroup.OfficialDocuments.FirstOrDefault();
      if (document == null)
        return this.GetErrorResult(vf.CustomContracts.AutoRegistrationScripts.Resources.DocumentNotFind);
      
      var contract = OverrideBaseDev.ContractBases.As(document);
      if (contract != null && contract.RegistrationState != OverrideBaseDev.ContractBase.RegistrationState.Registered)
      {
        if (contract.DocumentKind == null)
          return this.GetErrorResult(vf.CustomContracts.AutoRegistrationScripts.Resources.DocumentKindNotFind);

        var documentRegisters = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetDocumentRegistersIdsByParams(contract.DocumentKind, contract.BusinessUnit, contract.Department, contract.DocumentKind.NumberingType, false);
        if (documentRegisters.Any())
        {
          try
          {
            // Регистрация документа.
            var documentRegister = Sungero.Docflow.PublicFunctions.DocumentRegister.Remote.GetDocumentRegister(documentRegisters.First());
            
            var registrationDate = Calendar.Today;
            var registrationNumber = string.Empty;
            
            if (contract.RegistrationState == OverrideBaseDev.ContractBase.RegistrationState.Reserved)
            {
              documentRegister = contract.DocumentRegister;
              registrationDate = contract.RegistrationDate.HasValue ? contract.RegistrationDate.Value : Calendar.Today;
              registrationNumber = contract.RegistrationNumber;
            }
            else
              registrationNumber = this.GetFirstSkippedNumber(documentRegister, contract, registrationDate);
            
            Sungero.Docflow.PublicFunctions.OfficialDocument.RegisterDocument(contract, documentRegister, registrationDate, registrationNumber, false, true);
          }
          catch (Exception ex)
          {
            result = this.GetRetryResult(string.Empty);
            Logger.ErrorFormat("AutoRegistrationScript. Во время регистрации документа с Id: {0} произошла ошбика: {1}.", document.Id, ex);
          }
        }
        else
          Logger.ErrorFormat("AutoRegistrationScript. Во время регистрации документа с Id: {0} произошла ошбика: не найден журнал регистрации.", document.Id);
      }
      
      return result;
    }

    /// <summary>
    /// Получить первый в порядке увеличения пропущенный номер в журнале регистрации.
    /// </summary>
    /// <param name="documentRegister">Журнал регистрации.</param>
    /// <param name="document">Документ.</param>
    /// <param name="registrationDate">Дата регистрации.</param>
    /// <returns>Первый в порядке увеличения пропущенный номер в журнале регистрации. Если пропущенных номеров нет, то возвращается пустая строка.</returns>
    public string GetFirstSkippedNumber(Sungero.Docflow.IDocumentRegister documentRegister, Sungero.Docflow.IOfficialDocument document, DateTime? registrationDate)
    {
      var args = this.GetBeforeExecuteArguments(documentRegister, document, registrationDate);
      var documents = Enumerable.Empty<IOfficialDocument>().AsQueryable();
      AccessRights.AllowRead(() => { documents = this.FilterDocumentsByDocumentRegister(Sungero.Docflow.OfficialDocuments.GetAll(), args); });
      
      documents = this.FilterDocumentsByNumberingSection(documents, args);
      documents = this.FilterDocumentsByDocumentRegisterPeriod(documents, args);
      var firstIndex = this.GetFirstDocumentIndexInPeriod(documents, args);
      var lastIndex = this.GetLastDocumentIndexInPeriod(documents, args);
      // Для случая когда нет документов в периоде.
      if (lastIndex < firstIndex)
        lastIndex = firstIndex - 1;
      
      var firstSkippedIndex = this.GetFirstSkippedIndex(firstIndex, lastIndex, args);
      var registrationNumber = string.Empty;
      if (firstSkippedIndex.HasValue)
      {
        var registrationParams = OverrideBaseDev.PublicFunctions.OfficialDocument.Remote.GetRegistrationDialogParamsCustom(document, document.DocumentKind.NumberingType.Value);
        var currentRegistrationDate = registrationDate ?? Calendar.UserToday;
        registrationNumber = Sungero.Docflow.PublicFunctions.DocumentRegister.GenerateRegistrationNumber(documentRegister, currentRegistrationDate, firstSkippedIndex.ToString(),
                                                                                                         registrationParams.DepartmentCode, registrationParams.BusinessUnitCode, registrationParams.CaseFileIndex, registrationParams.DocKindCode,
                                                                                                         registrationParams.CounterpartyCode, registrationParams.LeadNumber);
      }
      return registrationNumber;
    }
    
    /// <summary>
    /// Получить аргументы для поиска пропусков в журнале регистрации.
    /// </summary>
    /// <returns>Аргументы для поиска пропусков в журнале регистрации.</returns>
    public virtual Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments GetBeforeExecuteArguments(Sungero.Docflow.IDocumentRegister documentRegister,
                                                                                                                     Sungero.Docflow.IOfficialDocument document,
                                                                                                                     DateTime? registrationDate)
    {
      var args = Sungero.Docflow.Structures.SkippedNumbersReport.BeforeExecuteArguments.Create();
      args.ReportSessionId = Guid.NewGuid().ToString();
      args.CurrentDate = Calendar.Now;
      args.DocumentRegister = documentRegister;
      args.LeadingDocument = document.LeadingDocument;
      args.HasLeadingDocument = document.LeadingDocument != null;
      
      args.LaunchedFromDialog = registrationDate.HasValue;
      args.RegistrationDate = registrationDate;
      
      args.Period = this.GetPeriod(args);
      args.BaseDate = this.GetBaseDate(args);
      args.DocumentRegisterPeriodBegin = Sungero.Docflow.PublicFunctions.DocumentRegister.GetBeginPeriod(args.DocumentRegister, args.BaseDate);
      args.DocumentRegisterPeriodEnd = Sungero.Docflow.PublicFunctions.DocumentRegister.GetEndPeriod(args.DocumentRegister, args.BaseDate);
      args.PeriodBegin = this.GetPeriodBegin(args);
      args.PeriodEnd = this.GetPeriodEnd(args);
      
      return args;
    }
    
    /// <summary>
    /// Получить первый пропущенный индекс в журнале.
    /// </summary>
    /// <param name="firstIndex">Индекс "с".</param>
    /// <param name="lastIndex">Индекс "по".</param>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Первый пропущенный индекс в журнале.</returns>
    public virtual int? GetFirstSkippedIndex(int firstIndex,
                                             int lastIndex,
                                             Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      var periodBeginDate = args.DocumentRegisterPeriodBegin.HasValue ? args.DocumentRegisterPeriodBegin.Value : Calendar.SqlMinValue;
      var month = periodBeginDate.ToString("MM");
      var day = periodBeginDate.ToString("dd");
      var startDate = string.Format("{0}{1}{2}", periodBeginDate.Year, month, day);
      
      var periodEndDate = args.DocumentRegisterPeriodEnd.HasValue ? args.DocumentRegisterPeriodEnd.Value : args.PeriodEnd;
      month = periodEndDate.ToString("MM");
      day = periodEndDate.ToString("dd");
      var endDate = string.Format("{0}{1}{2}", periodEndDate.Year, month, day);
      
      var queryText = string.Format(Sungero.Docflow.Queries.SkippedNumbersReport.GetSkippedIndexes,
                                    args.DocumentRegister.Id.ToString(),
                                    (firstIndex - 1).ToString(),
                                    (lastIndex + 1).ToString(),
                                    args.HasBusinessUnit.ToString(),
                                    args.HasBusinessUnit ? args.BusinessUnit.Id.ToString() : "0",
                                    args.HasDepartment.ToString(),
                                    args.HasDepartment ? args.Department.Id.ToString() : "0",
                                    args.HasLeadingDocument.ToString(),
                                    args.HasLeadingDocument ? args.LeadingDocument.Id.ToString() : "0",
                                    args.DocumentRegisterPeriodBegin.HasValue.ToString(),
                                    startDate,
                                    endDate);
      
      // Получить первый пропущенный индекс журнала в периоде.
      int? firstSkippedIndex = null;
      
      using (var command = SQL.GetCurrentConnection().CreateCommand())
      {
        command.CommandText = queryText;
        using (var result = command.ExecuteReader())
        {
          if (result.Read())
            firstSkippedIndex = (int)result[1];
          while (result.Read())
          {
            if ((int)result[1] < firstSkippedIndex)
              firstSkippedIndex = (int)result[1];
          }
        }
      }
      
      return firstSkippedIndex;
    }
    
    /// <summary>
    /// Получить период поиска пропущенных номеров в журнале.
    /// </summary>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Период поиска пропущенных номеров в журнале.</returns>
    public string GetPeriod(Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      if (args.DocumentRegister.NumberingPeriod == Sungero.Docflow.DocumentRegister.NumberingPeriod.Day)
        return Constants.AutoRegistrationScript.TimePeriods.Day;
      else if (args.DocumentRegister.NumberingPeriod == Sungero.Docflow.DocumentRegister.NumberingPeriod.Month)
        return Constants.AutoRegistrationScript.TimePeriods.Month;
      else if (args.DocumentRegister.NumberingPeriod == Sungero.Docflow.DocumentRegister.NumberingPeriod.Quarter)
        return Constants.AutoRegistrationScript.TimePeriods.Quarter;
      else if (args.DocumentRegister.NumberingPeriod == Sungero.Docflow.DocumentRegister.NumberingPeriod.Year)
        return Constants.AutoRegistrationScript.TimePeriods.Year;
      return Constants.AutoRegistrationScript.TimePeriods.Year;
    }
    
    /// <summary>
    /// Отфильтровать документы по индексам.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="firstIndex">Индекс "с".</param>
    /// <param name="lastIndex">Индекс "по".</param>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Документы, отфильтрованные по индекам.</returns>
    /// <remarks>Допускать документы с номером, не соответствующим формату (Index = 0).</remarks>
    public virtual IQueryable<IOfficialDocument> FilterDocumentsByIndicies(IQueryable<IOfficialDocument> documents,
                                                                           int firstIndex,
                                                                           int lastIndex,
                                                                           Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      return documents.Where(l => l.Index >= firstIndex && l.Index <= lastIndex ||
                             l.Index == 0 && l.RegistrationDate <= args.PeriodEnd && l.RegistrationDate >= args.PeriodBegin);
    }
    
    /// <summary>
    /// Получить первый индекс по периоду поиска пропущенных номеров в журнале.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Первый индекс по периоду поиска пропущенных номеров в журнале.</returns>
    public virtual int GetFirstDocumentIndexInPeriod(IQueryable<IOfficialDocument> documents,
                                                     Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      var periodBegin = args.PeriodBegin;
      var periodEnd = args.PeriodEnd;
      
      // Получить минимальный индекс по документам в периоде (при ручной регистрации мб нарушение следования индексов).
      var firstDocumentIndex = OverrideBaseDev.PublicFunctions.DocumentRegister.GetIndexCustom(documents, periodBegin, periodEnd, false);
      
      // Получить индекс документа из предыдущего периода.
      var previousIndex = 0;
      if (periodBegin != args.DocumentRegisterPeriodBegin)
        previousIndex = OverrideBaseDev.PublicFunctions.DocumentRegister.FilterDocumentsByPeriodCustom(documents, args.DocumentRegisterPeriodBegin,
                                                                                                       periodBegin.AddDays(-1).EndOfDay())
          .Where(d => !firstDocumentIndex.HasValue || d.Index < firstDocumentIndex).Select(d => d.Index).OrderByDescending(a => a).FirstOrDefault() ?? 0;
      
      if (!firstDocumentIndex.HasValue)
        firstDocumentIndex = previousIndex + 1;
      
      return firstDocumentIndex < previousIndex ? firstDocumentIndex.Value : previousIndex + 1;
    }
    
    /// <summary>
    /// Получить последний индекс по периоду поиска пропущенных номеров в журнале.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Последний индекс по периоду поиска пропущенных номеров в журнале.</returns>
    public virtual int GetLastDocumentIndexInPeriod(IQueryable<IOfficialDocument> documents,
                                                    Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      var periodBegin = args.PeriodBegin;
      var periodEnd = args.PeriodEnd;
      
      // Получить первый индекс документа следующего периода.
      var nextIndex = periodEnd != args.DocumentRegisterPeriodEnd ?
        OverrideBaseDev.PublicFunctions.DocumentRegister.GetIndexCustom(documents, periodEnd.AddDays(1).BeginningOfDay(), args.DocumentRegisterPeriodEnd, false) : null;
      
      var leadingDocumentId = args.HasLeadingDocument ? args.LeadingDocument.Id : 0;
      var departmentId = args.HasDepartment ? args.Department.Id : 0;
      var businessUnitId = args.HasBusinessUnit ? args.BusinessUnit.Id : 0;
      
      // Если в следующем периоде ещё нет документов, то взять текущий индекс журнала.
      if (!nextIndex.HasValue)
        nextIndex = OverrideBaseDev.PublicFunctions.DocumentRegister.GetCurrentNumber(OverrideBaseDev.DocumentRegisters.As(args.DocumentRegister), args.BaseDate, leadingDocumentId, departmentId, businessUnitId) + 1;
      
      // Получить индекс по зарегистрированным документам (при ручной регистрации мб нарушение следования индексов).
      var lastDocumentIndex = OverrideBaseDev.PublicFunctions.DocumentRegister.GetIndexCustom(documents, periodBegin, periodEnd, true) ?? nextIndex.Value - 1;
      return lastDocumentIndex >= nextIndex ? lastDocumentIndex : nextIndex.Value - 1;
    }
    
    /// <summary>
    /// Отфильтровать документы по разрезу журнала.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Документы, отфильтрованные по разрезу журнала.</returns>
    public virtual IQueryable<IOfficialDocument> FilterDocumentsByNumberingSection(IQueryable<IOfficialDocument> documents,
                                                                                   Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      if (args.HasLeadingDocument)
        documents = documents.Where(d => Equals(d.LeadingDocument, args.LeadingDocument));
      
      if (args.HasDepartment)
        documents = documents.Where(d => Equals(d.Department, args.Department));
      
      if (args.HasBusinessUnit)
        documents = documents.Where(d => Equals(d.BusinessUnit, args.BusinessUnit));
      
      return documents;
    }
    
    /// <summary>
    /// Получить дату начала периода поиска пропущенных номеров в журнале.
    /// </summary>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Дата начала периода поиска пропущенных номеров в журнале.</returns>
    public virtual DateTime GetPeriodBegin(Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      var baseDate = args.BaseDate;
      var periodBegin = baseDate;
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Year))
        periodBegin = Calendar.BeginningOfYear(baseDate);
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Quarter))
        periodBegin = Sungero.Docflow.PublicFunctions.AccountingDocumentBase.BeginningOfQuarter(baseDate);
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Month))
        periodBegin = Calendar.BeginningOfMonth(baseDate);
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Week))
        periodBegin = Calendar.BeginningOfWeek(baseDate);
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Day))
        periodBegin = Calendar.BeginningOfDay(baseDate);
      
      if (args.DocumentRegisterPeriodBegin.HasValue && args.DocumentRegisterPeriodBegin.Value > periodBegin)
        return args.DocumentRegisterPeriodBegin.Value;
      
      return periodBegin;
    }
    
    /// <summary>
    /// Получить дату конца периода поиска пропущенных номеров в журнале.
    /// </summary>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Дата конца периода поиска пропущенных номеров в журнале.</returns>
    public virtual DateTime GetPeriodEnd(Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      return Calendar.EndOfDay(args.BaseDate);
    }

    /// <summary>
    /// Получить базовую дату для поиска пропущенных номеров в журнале.
    /// </summary>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Базовая дата для поиска пропущенных номеров в журнале.</returns>
    public DateTime GetBaseDate(Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      var currentDate = args.CurrentDate;
      var periodOffset = args.PeriodOffset;
      
      if (args.LaunchedFromDialog)
        return args.RegistrationDate.Value;
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Year))
        return Calendar.EndOfYear(currentDate.AddYears(periodOffset));
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Quarter))
        return Calendar.EndOfDay(Sungero.Docflow.PublicFunctions.AccountingDocumentBase.EndOfQuarter(currentDate.AddMonths(3 * periodOffset)));
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Month))
        return Calendar.EndOfMonth(currentDate.AddMonths(periodOffset));
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Week))
        return Calendar.EndOfWeek(currentDate.AddDays(7 * periodOffset));
      
      if (args.Period.Equals(Constants.AutoRegistrationScript.TimePeriods.Day))
        return Calendar.EndOfDay(currentDate.AddDays(periodOffset));
      
      return currentDate;
    }
    
    /// <summary>
    /// Отфильтровать документы по журналу регистрации.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Документы, отфильтрованные по журналу регистрации.</returns>
    /// <remarks>Документ должен быть зарегистрирован или зарезервирован в журнале.</remarks>
    public virtual IQueryable<IOfficialDocument> FilterDocumentsByDocumentRegister(IQueryable<IOfficialDocument> documents,
                                                                                   Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      return documents
        .Where(d => d.DocumentRegister == args.DocumentRegister)
        .Where(d => d.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Registered || d.RegistrationState == Sungero.Docflow.OfficialDocument.RegistrationState.Reserved);
    }
    
    /// <summary>
    /// Отфильтровать документы по периоду журнала регистрации.
    /// </summary>
    /// <param name="documents">Документы.</param>
    /// <param name="args">Аргументы построения отчета.</param>
    /// <returns>Документы, отфильтрованые по периоду журнала регистрации.</returns>
    public virtual IQueryable<IOfficialDocument> FilterDocumentsByDocumentRegisterPeriod(IQueryable<IOfficialDocument> documents,
                                                                                         Sungero.Docflow.Structures.SkippedNumbersReport.IBeforeExecuteArguments args)
    {
      DateTime? documentRegisterPeriodBegin = args.DocumentRegisterPeriodBegin;
      DateTime? documentRegisterPeriodEnd = args.DocumentRegisterPeriodEnd;
      
      return documents
        .Where(d => !documentRegisterPeriodBegin.HasValue || d.RegistrationDate >= documentRegisterPeriodBegin)
        .Where(d => !documentRegisterPeriodEnd.HasValue || d.RegistrationDate <= documentRegisterPeriodEnd);
    }
  }
}