using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.Models.BaseModels;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Models.ObjectRepresentations;


namespace SIA_Portal.Models.GenericUserModel
{
    public class ViewNotificationModel : BaseAccountLoggedInModel
    {

        public NotificationRepresentation NotifRep { set; get; }

        //

        //public DocumentFileRepresentation DocuFileRep { set; get; }

        public bool HasDocu { set; get; }

        public int DocuId { set; get; }

        public string DocuOriginalName { set; get; }

        //

        public ViewNotificationModel() : base() { }

        public ViewNotificationModel(object loggedInAccount) : base((Account)loggedInAccount) { }

    }
}