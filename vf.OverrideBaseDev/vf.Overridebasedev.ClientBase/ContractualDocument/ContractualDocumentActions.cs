using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.OverrideBaseDev.ContractualDocument;

namespace vf.OverrideBaseDev.Client
{
  partial class ContractualDocumentActions
  {
    public virtual void SendContractToKafka(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      Functions.ContractualDocument.Remote.CreateSendContractAsycnh(_obj);
      e.AddInformation("Договор успешно отправлен в Kafka");
    }

    public virtual bool CanSendContractToKafka(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return IntegrationSettings.PublicFunctions.Module.CheckCurrentUserIsAdmin();
    }


    public virtual void SendToCurrencyControl(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      // Проверить, что запись сохранена.
      if (_obj.State.IsChanged || _obj.State.IsInserted)
      {
        _obj.Save();
      }
      
      var sended = CustomContracts.PublicFunctions.SetOnCurrencyControl.Remote.CreateTaskSetOnCurrencyControl(_obj);
      
      if (sended)
        e.AddInformation(vf.OverrideBaseDev.ContractualDocuments.Resources.TaskSend);
      else
        e.AddError(vf.OverrideBaseDev.ContractualDocuments.Resources.TaskNotSendError);
    }

    public virtual bool CanSendToCurrencyControl(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      // Проверить права доступа на создание задачи.
      return _obj.SendToCurrencyControl != true && _obj.CounterpartyNonresident == true && CustomContracts.SetOnCurrencyControls.AccessRights.CanCreate()
        && _obj.LifeCycleState == OverrideBaseDev.ContractualDocument.LifeCycleState.Active;
    }

  }

}