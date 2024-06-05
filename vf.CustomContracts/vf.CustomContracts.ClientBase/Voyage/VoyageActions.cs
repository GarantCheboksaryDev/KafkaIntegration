using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using vf.CustomContracts.Voyage;

namespace vf.CustomContracts.Client
{
  partial class VoyageActions
  {

    public virtual bool CanShowVoyageDocuments(Sungero.Domain.Client.CanExecuteActionArgs e)
    {
      return true;
    }

    public virtual void ShowVoyageDocuments(Sungero.Domain.Client.ExecuteActionArgs e)
    {
      var documents = vf.OverrideBaseDev.Module.Docflow.PublicFunctions.Module.Remote.GetVoyageDocuments(_obj);
      documents.Show(vf.CustomContracts.Voyages.Resources.RecordIsAttachInDocuments);
    }
  }

}