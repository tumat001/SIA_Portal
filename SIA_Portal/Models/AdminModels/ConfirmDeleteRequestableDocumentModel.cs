using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.RequestableDocument;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{
    public class ConfirmDeleteRequestableDocumentModel : BaseWithTableIndexingLoggedInModel<RequestableDocument, RequestableDocumentRepresentation>
    {
        public ConfirmDeleteRequestableDocumentModel() : base() { }

        public ConfirmDeleteRequestableDocumentModel(object account) : base((Account)account) { }


    }
}