using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.SetOnCurrencyControl;

namespace vf.CustomContracts.Server
{
  partial class SetOnCurrencyControlFunctions
  {
    /// <summary>
    /// Создать задачу на постановку договора на валютный контроль.
    /// </summary>
    /// <param name="contract">Договор.</param>
    /// <returns>True - если задача создана, иначе false.</returns>
    [Public, Remote]
    public static bool CreateTaskSetOnCurrencyControl(OverrideBaseDev.IContractualDocument contract)
    {
      try
      {
        // Создать задачу.
        var task = CustomContracts.SetOnCurrencyControls.Create();
        task.Subject = CustomContracts.SetOnCurrencyControls.Resources.Block3SubjectFormat(contract);
        
        task.Author = Users.Current;
        
        // Заполнить вложения.
        task.ContractGroup.ContractualDocuments.Add(contract);
        
        // Получить приложения.
        var documentAddenda = Sungero.Docflow.PublicFunctions.Module.GetAddenda(contract)
          .Where(x => Sungero.Docflow.OfficialDocuments.Is(x))
          .Cast<Sungero.Docflow.IOfficialDocument>();
        
        if (documentAddenda.Any())
        {
          foreach (var documentAddend in documentAddenda)
          {
            if (!task.AddendumGroup.OfficialDocuments.Contains(documentAddend))
              task.AddendumGroup.OfficialDocuments.Add(documentAddend);
          }
        }
        
        task.Save();
        task.Start();
        
        contract.SendToCurrencyControl = true;
        contract.Save();
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("Произошла ошбика: {0}. StackTrace: {1}.", ex.Message, ex.StackTrace);
        return false;
      }
      
      return true;
    }
  }
}