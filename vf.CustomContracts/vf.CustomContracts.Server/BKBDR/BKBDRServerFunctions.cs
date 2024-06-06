using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.BKBDR;

namespace vf.CustomContracts.Server
{
  partial class BKBDRFunctions
  {
    /// <summary>
    ///  Получить синхронизированную статью бюджета.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <returns>Найденная статья, иначе новая.</returns>
    [Public]
    public static CustomContracts.IBKBDR GetBudgetItemSynch(string guid1C)
    {
      var budgetItem = GetBudgetItemFrom1CGuid(guid1C);
      
      return budgetItem != null ? budgetItem : CustomContracts.BKBDRs.Create();
    }
    
    /// <summary>
    ///  Найти статью бюджета по Guid.
    /// </summary>
    /// <param name="guid1C">Guid 1C.</param>
    /// <returns>Найденная статья.</returns>
    [Public]
    public static CustomContracts.IBKBDR GetBudgetItemFrom1CGuid(string guid1C)
    {
      return CustomContracts.BKBDRs.GetAll(x => x.ExternalId == guid1C).FirstOrDefault();
    }
  }
}