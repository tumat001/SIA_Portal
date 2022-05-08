using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.QueueManager;
using CommonDatabaseActionReusables.AccountManager;
using System.Web.Mvc;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{
    public class ManageAcknowledgedReqDocuQueueModel : BaseWithTableIndexingLoggedInModel<Queue, QueueRepresentation>
    {

        public ManageAcknowledgedReqDocuQueueModel() : base() { }

        public ManageAcknowledgedReqDocuQueueModel(object account) : base((Account)account) { }


    }
}