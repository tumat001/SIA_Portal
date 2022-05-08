using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using System.Web.Mvc;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;
using SIA_Portal.CustomAccessors.RequestableDocument;

namespace SIA_Portal.Models.AdminModels
{
    public class ManageRequestableDocumentsModel : BaseWithTableIndexingLoggedInModel<RequestableDocument, RequestableDocumentRepresentation>
    {

        public string TitleFilter { set; get; }


        public ManageRequestableDocumentsModel() : base() { }

        public ManageRequestableDocumentsModel(object account) : base((Account)account) { }


    }
}