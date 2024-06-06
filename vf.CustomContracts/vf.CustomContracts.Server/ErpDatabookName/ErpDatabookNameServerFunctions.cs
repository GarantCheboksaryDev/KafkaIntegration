using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.ErpDatabookName;

namespace vf.CustomContracts.Server
{
  partial class ErpDatabookNameFunctions
  {
    /// <summary>
    /// Получить наименования справочника в 1С:ERP по виду документа.
    /// </summary>
    /// <returns>Наименования справочника в 1С:ERP.</returns>
    [Remote(IsPure = true), Public]
    public static IQueryable<IErpDatabookName> GetErpDatabookNamesByDocumentKind(Sungero.Docflow.IDocumentKind documentKind)
    {
      return vf.CustomContracts.ErpDatabookNames.GetAll().Where(x => x.DocumentKinds.Any(dk => Sungero.Docflow.DocumentKinds.Equals(dk, documentKind)));
    }
  }
}