using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SIA_Portal.Accessors;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.AnnouncementManager;
using SIA_Portal.Constants;

namespace SIA_PORTAL_UnitTest.DataSetter
{
    [TestClass]
    public class PortalAnnouncementSetter
    {

        private static PortalAnnouncementAccessor portalAnnAccessor;


        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            portalAnnAccessor = new PortalAnnouncementAccessor();
        }

        //

        [TestCategory("Creating Announcements")]
        [TestMethod]
        public void Creating_Announcements()
        {
            var builder = new Announcement.Builder();
            builder.Title = "Announcement Title 01";
            builder.SetContentUsingString("Lorem ipsum content of announcement 01 Filler fill fill here.");
            portalAnnAccessor.AnnouncementManagerHelper.CreateAnnouncement(builder);


            var builder02 = new Announcement.Builder();
            builder02.Title = "Announcement Title 02";
            builder02.SetContentUsingString("Lorem ipsum content of announcement 02 Filler fill fill here fill fill there.");
            portalAnnAccessor.AnnouncementManagerHelper.CreateAnnouncement(builder02);

            var builder03 = new Announcement.Builder();
            builder03.Title = "Announcement Title 03";
            builder03.SetContentUsingString("Lorem ipsum content of announcement 03 Filler fill fill here fill fill there.");
            portalAnnAccessor.AnnouncementManagerHelper.CreateAnnouncement(builder03);
        }


        [TestCategory("Deleting Announcements")]
        [TestMethod]
        public void Deleting_All_Announcements()
        {
            portalAnnAccessor.AnnouncementManagerHelper.DeleteAllAnnouncements();
        }


        [TestCategory("Reset")]
        [TestMethod]
        public void Clean_ResetOfAnnouncements()
        {
            Deleting_All_Announcements();
            Creating_Announcements();
        }

    }
}
