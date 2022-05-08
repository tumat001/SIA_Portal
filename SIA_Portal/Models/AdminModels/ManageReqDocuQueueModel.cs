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
    public class ManageReqDocuQueueModel : BaseWithTableIndexingLoggedInModel<Queue, QueueRepresentation>
    {

        //public string TitleFilter { set; get; }

        public string SelectedQueueStatus { set; get; }

        public IList<string> QueueStatusList { set; get; }



        public ManageReqDocuQueueModel() : base() { }

        public ManageReqDocuQueueModel(object account) : base((Account)account) { }


    }
}