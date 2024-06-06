using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace vf.OverrideBaseDev.Module.Docflow.Server
{
  partial class ModuleFunctions
  {
    /// <summary>
    /// Получить задание на Контроль возврата, сформированного в рамках задачи на согласование со статусом в работе.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Задание на контроль возврата.</returns>
    [Public, Remote(IsPure = true)]
    public Sungero.Docflow.IApprovalCheckReturnAssignment GetInProcessApprovalCheckReturnAssignmentByDocument(Sungero.Docflow.IOfficialDocument document)
    {
      return Sungero.Docflow.ApprovalCheckReturnAssignments.GetAll()
        .Where(x => x.Status == Sungero.Workflow.Assignment.Status.InProcess && 
               x.AttachmentDetails.Any(a => a.AttachmentId == document.Id)).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить задание на согласование по документу со статусом в работе.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Задание на контроль возврата.</returns>
    [Public, Remote(IsPure = true)]
    public Sungero.Docflow.IApprovalAssignment GetInProcessApprovalAssignmentByDocument(Sungero.Docflow.IOfficialDocument document)
    {
      return Sungero.Docflow.ApprovalAssignments.GetAll()
        .Where(x => x.Status == Sungero.Workflow.Assignment.Status.InProcess && 
               x.AttachmentDetails.Any(a => a.AttachmentId == document.Id)).FirstOrDefault();
    }
    
    /// <summary>
    /// Получить документы по рейсу.
    /// </summary>
    /// <param name="voyage">Рейс.</param>
    /// <returns>Список документов по рейсу.</returns>
    [Public, Remote(IsPure = true)]
    public IQueryable<vf.OverrideBaseDev.IContractualDocument> GetVoyageDocuments(vf.CustomContracts.IVoyage voyage)
    {
      return vf.OverrideBaseDev.ContractualDocuments.GetAll()
        .Where(doc => doc.Voyages.Any(v => vf.CustomContracts.Voyages.Equals(voyage, v.Voyage)));
    }
    
    /// <summary>
    /// Получить дату подписания контрагентом.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Дата подписания контрагентом.</returns>
    [Public, Remote(IsPure = true)]
    public static DateTime? GetDateOfCounterpartySigning(Sungero.Docflow.IOfficialDocument document)
    {
      if (!document.HasVersions)
        return null;

      var exchangeDocumentInfos = Sungero.Exchange.ExchangeDocumentInfos.GetAll(x => Sungero.Docflow.OfficialDocuments.Equals(x.Document, document));
      if (!exchangeDocumentInfos.Any())
        return null;
      
      var signatures = Signatures.Get(document);
      if (!signatures.Any())
        return null;
      
      var counterpartySignatures = signatures
        .Where(signature => !exchangeDocumentInfos.Any(exchangeDocumentInfo =>
                                                       (exchangeDocumentInfo.MessageType == Sungero.Exchange.ExchangeDocumentInfo.MessageType.Incoming &&
                                                        exchangeDocumentInfo.ReceiverSignId == signature.Id) ||
                                                       (exchangeDocumentInfo.MessageType == Sungero.Exchange.ExchangeDocumentInfo.MessageType.Outgoing &&
                                                        exchangeDocumentInfo.SenderSignId == signature.Id))).ToList();
      if (!counterpartySignatures.Any())
        return null;

      return counterpartySignatures.Max(x => x.SigningDate);
    }
    
    /// <summary>
    /// Проверка вхождения пользователя или замещаемых им пользователей в указанную роль.
    /// </summary>
    /// <param name="roleSid">Guid роли.</param>
    /// <returns>True, если польлзователь, или замещаемые им пользователи входят в заданную роль. Иначе - false.</returns>
    [Public, Remote(IsPure = true)]
    public static bool UserOrSubstitutionIncludedInRole(Guid roleSid)
    {
      if (Users.Current.IncludedIn(roleSid))
        return true;
      else
      {
        var directSubstitutionRecipientIds = Recipients.DirectSubstitutionRecipientIds;
        return Users.GetAll(x => directSubstitutionRecipientIds.Any() && directSubstitutionRecipientIds.Contains(x.Id)).ToList().Any(x => x.IncludedIn(roleSid));
      }
      return false;
    }
    
    /// <summary>
    /// Проверка наличия у заданного документа связей, Вид документа которых содержится в заданном списке.
    /// </summary>
    /// <param name="documentKinds">Список видов документов.</param>
    /// <returns>True, если есть хотя бы один связанный документ с Видом документа, который есть в заданном списке. Иначе - false.</returns>
    [Public, Remote(IsPure = true)]
    public bool HasRelationsWithGivenDocumentKinds(Sungero.Docflow.IOfficialDocument document, List<OverrideBaseDev.IDocumentKind> documentKinds)
    {
      if (document != null && document.HasRelations)
      {
        var relations = document.Relations.GetRelated();
        foreach (var relation in relations)
        {
          var relatedDocument = Sungero.Docflow.OfficialDocuments.As(relation);
          if (relatedDocument != null && relatedDocument.DocumentKind != null && documentKinds.Contains(relatedDocument.DocumentKind))
            return true;
        }
      }
      return false;
    }
    
    /// <summary>
    /// Проверить вхождение текущего пользователя в роль.
    /// </summary>
    /// <param name="roleSid">Sid роли.</param>
    /// <returns>True, если входит, иначе false.</returns>
    [Public, Remote(IsPure = true)]
    public static bool IncludedInRole(Guid roleSid)
    {
      return Users.Current.IncludedIn(roleSid);
    }
  }
}