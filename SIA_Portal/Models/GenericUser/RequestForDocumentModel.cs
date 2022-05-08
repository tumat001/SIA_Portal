using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.Models.ObjectRepresentations;
using CommonDatabaseActionReusables.AccountManager;
using System.Web.Mvc;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Constants;


namespace SIA_Portal.Models.GenericUserModel
{
    public class RequestForDocumentModel : BaseAccountLoggedInModel
    {

        public int SelectedDocumentRepId { set; get; }

        public RequestableDocumentRepresentation SelectedDocumentRepresentation { set; get; }

        public string InputReasonForRequest { set; get; }

        public int InputPriorityLevel { set; get; } = 0;

        

        public IList<RequestableDocumentRepresentation> AllRequestableDocumentRepresentations { set; get; }



        public RequestForDocumentModel(object loggedInAccount) : base((Account)loggedInAccount)
        {
            
        }

        public RequestForDocumentModel() : base()
        {

        }

        //

        public int ActionExecuteStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

        public string StatusMessage { set; get; } = "";

    }
}