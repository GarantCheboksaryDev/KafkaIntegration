using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.CustomParties.Server
{
  public class ModuleAsyncHandlers
  {

    public virtual void UpdateCounterpartyRequisitesAsync(vf.CustomParties.Server.AsyncHandlerInvokeArgs.UpdateCounterpartyRequisitesAsyncInvokeArgs args)
    {
      var prefix = "UpdateCounterpartyRequisitesAsync. ";
      Logger.DebugFormat("{0}Старт процесса.", prefix);
      var counterparty = OverrideBaseDev.Counterparties.GetAll(x => x.Id == args.CounterpartyId).FirstOrDefault();
      var assignment = CustomParties.CounterpartyCheckAssignments.GetAll(x => x.Id == args.AssignmentId).FirstOrDefault();
      try
      {
        if (counterparty == null)
        {
          Logger.ErrorFormat("{0}Не найден контрагент с Id {1}.", prefix, args.CounterpartyId);
          return;
        }
        if (assignment == null)
        {
          Logger.ErrorFormat("{0}Не найдено задание на проверку контрагента с Id {1}.", prefix, args.AssignmentId);
          return;
        }
        
        if (Locks.TryLock(counterparty))
        {
          if (assignment.CheckingResultSB != null)
          {
            if (!vf.CustomParties.ServiceSecurityCheckResults.Equals(counterparty.ServiceSecurityCheckResult, assignment.CheckingResultSB))
              counterparty.ServiceSecurityCheckResult = assignment.CheckingResultSB;
            
            if (vf.CustomParties.ServiceSecurityCheckResults.Equals(counterparty.ServiceSecurityCheckResult ,vf.CustomParties.PublicFunctions.ServiceSecurityCheckResult.Remote.GetServiceSecurityCheckResultBySid(Constants.Module.ServiceSecurityCheckResult.NotRecommended)) ||
                vf.CustomParties.ServiceSecurityCheckResults.Equals(counterparty.ServiceSecurityCheckResult ,vf.CustomParties.PublicFunctions.ServiceSecurityCheckResult.Remote.GetServiceSecurityCheckResultBySid(Constants.Module.ServiceSecurityCheckResult.NotApproved)))
              counterparty.SpecialList = OverrideBaseDev.Counterparty.SpecialList.AddMonitoring;
          }
          
          if (assignment.CheckingResultSPARK == CustomParties.CounterpartyCheckAssignment.CheckingResultSPARK.Green)
            counterparty.CheckingResultSPARK = OverrideBaseDev.Counterparty.CheckingResultSPARK.Green;
          else if (assignment.CheckingResultSPARK == CustomParties.CounterpartyCheckAssignment.CheckingResultSPARK.Yellow)
            counterparty.CheckingResultSPARK = OverrideBaseDev.Counterparty.CheckingResultSPARK.Yellow;
          else if (assignment.CheckingResultSPARK == CustomParties.CounterpartyCheckAssignment.CheckingResultSPARK.Red)
            counterparty.CheckingResultSPARK = OverrideBaseDev.Counterparty.CheckingResultSPARK.Red;
          
          var counterpartyCheckRequests = vf.CustomParties.CounterpartyCheckRequests.As(assignment.Task);
          if (counterpartyCheckRequests != null && counterpartyCheckRequests.CheckingDate.HasValue && counterparty.CheckingDate != counterpartyCheckRequests.CheckingDate)
            counterparty.CheckingDate = counterpartyCheckRequests.CheckingDate;
          
          if (counterpartyCheckRequests != null && counterpartyCheckRequests.CheckValidTill.HasValue && counterparty.CheckValidTill != counterpartyCheckRequests.CheckValidTill)
            counterparty.CheckValidTill = counterpartyCheckRequests.CheckValidTill;

          counterparty.Save();
          Locks.Unlock(counterparty);
          
          Logger.DebugFormat("{0}Результат проверки СБ и результат проверки СПАРК успешно занесены в карточку контрагента с Id {1}.", prefix, args.CounterpartyId);
        }
        else
        {
          Logger.ErrorFormat("{0}Не удалось установить блокировку на запись справочника Контрагенты с Id {1}.", prefix, args.CounterpartyId);
          args.Retry = true;
        }
      }
      catch (Exception ex)
      {
        Logger.ErrorFormat("{0}Произошла ошибка при занесении результата проверки СБ и результата проверки СПАРК в карточку контрагента с Id {1}. Id задания на проверку контрагента: {2}. {3}", prefix, args.CounterpartyId, args.AssignmentId, ex);
        args.Retry = true;
      }
    }

  }
}