using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AnnouncementManager;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Models.ObjectRepresentations;
using SIA_Portal.Models.BaseModels;

namespace SIA_Portal.Models.EmployeeModel
{
    public class EmployeeHomePageModel : BaseWithTableIndexingLoggedInModel<Announcement, AnnouncementRepresentation>
    {

        public string AnnouncementTitleFilter { set; get; }

        public const int ANNOUNCEMENT_COUNT = 1;//5;
        public const int ANNOUNCEMENT_PREVIEW_LENGTH = 100;


        public static string GetAnnouncementPreview(Announcement announcement)
        {
            var content = announcement.GetContentAsString();

            if (content == null)
            {
                return "";
            }
            else
            {
                var finalLength = ANNOUNCEMENT_PREVIEW_LENGTH;
                if (finalLength > content.Length)
                {
                    finalLength = content.Length;
                }

                return content.Substring(0, finalLength);
            }
        }



        public EmployeeHomePageModel() : base() { }

        public EmployeeHomePageModel(object account) : base((Account)account) { }

    }
}