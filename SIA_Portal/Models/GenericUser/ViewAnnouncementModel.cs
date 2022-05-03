using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.Models.BaseModels;
using CommonDatabaseActionReusables.AccountManager;

namespace SIA_Portal.Models.GenericUserModel
{
    public class ViewAnnouncementModel : BaseAccountLoggedInModel
    {
        public int AnnouncementId { set; get; }

        public string AnnouncementTitle { set; get; }

        public string AnnouncementContent { set; get; }

        public string AnnouncementImagePathOrContent { set; get; }


        //

        public ViewAnnouncementModel() : base() { }

        public ViewAnnouncementModel(object loggedInAccount) : base((Account)loggedInAccount) { }
    }
}