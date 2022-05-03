using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AnnouncementManager;
using CommonDatabaseActionReusables.AccountManager;
using System.Web.Mvc;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{
    public class ManageAnnouncementsModel : BaseWithTableIndexingLoggedInModel<Announcement, AnnouncementRepresentation>
    {

        public string TitleFilter { set; get; }


        public ManageAnnouncementsModel() : base() { }

        public ManageAnnouncementsModel(object account) : base((Account)account) { }


    }
}