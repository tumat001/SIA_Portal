using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.QueueManager;
using CommonDatabaseActionReusables.AccountManager;
using System.Web.Mvc;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;
using SIA_Portal.Constants;


namespace SIA_Portal.Models.AdminModels
{
    public class FulfillAcknowledgedReqDocQueueModel : BaseAccountLoggedInModel
    {

        public QueueRepresentation QueueRep { set; get; }


        public string InputFulfillerNotificationRemarks { set; get; }

        public HttpPostedFileBase InputDocument { set; get; }


        //

        public bool IsCompleted { set; get; }

        //

        public int ActionExecuteStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

        public string StatusMessage { set; get; } = "";

        //

        public FulfillAcknowledgedReqDocQueueModel() : base() { }

        public FulfillAcknowledgedReqDocQueueModel(object account) : base((Account)account) { }


    }
}