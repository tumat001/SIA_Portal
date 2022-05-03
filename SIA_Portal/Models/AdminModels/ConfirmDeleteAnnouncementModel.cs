using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AnnouncementManager;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{
    public class ConfirmDeleteAnnouncementModel : BaseWithTableIndexingLoggedInModel<Announcement, AnnouncementRepresentation>
    {

        public ConfirmDeleteAnnouncementModel() : base() { }

        public ConfirmDeleteAnnouncementModel(object account) : base((Account)account) { }


    }
}