using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CommonDatabaseActionReusables.NotificationManager;
using SIA_Portal.Accessors;

namespace SIA_PORTAL_UnitTest.DataSetter
{
    [TestClass]
    public class PortalNotificationSetter
    {

        private PortalNotificationAccessor notifAccessor = new PortalNotificationAccessor();


        //


        [TestCategory("Delete")]
        [TestMethod]
        public void DeleteAll_Notifications()
        {
            notifAccessor.NotificationManagerHelper.DeleteAllNotifications();

        }
    }
}
