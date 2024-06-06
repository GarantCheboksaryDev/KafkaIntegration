using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.OverrideBaseDev.Module.ContractsUI.Client
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Создать документ с диалогом выбора типа документа.
    /// </summary>
    [LocalizeFunction("CreateDocumentWithoutContractStatementFunctionName", "CreateDocumentFunctionDescription")]
    public virtual void CreateWithoutContractStatementDocument()
    {
      Sungero.Contracts.ContractualDocuments.CreateDocumentWithCreationDialog(Sungero.Contracts.ContractualDocuments.Info,
                                                                              Sungero.Docflow.SimpleDocuments.Info,
                                                                              Sungero.Docflow.Addendums.Info,
                                                                              Sungero.Contracts.IncomingInvoices.Info,
                                                                              Sungero.Contracts.OutgoingInvoices.Info,
                                                                              Sungero.Docflow.CounterpartyDocuments.Info);
    }
  }
}