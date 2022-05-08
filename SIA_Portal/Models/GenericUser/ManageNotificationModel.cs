using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.NotificationManager;
using CommonDatabaseActionReusables.AccountManager;
using System.Web.Mvc;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.GenericUserModel
{
    public class ManageNotificationModel : BaseWithTableIndexingLoggedInModel<Notification, NotificationRepresentation>
    {

        public string TitleFilter { set; get; }


        public ManageNotificationModel() : base() { }

        public ManageNotificationModel(object account) : base((Account)account) { }


    }
}