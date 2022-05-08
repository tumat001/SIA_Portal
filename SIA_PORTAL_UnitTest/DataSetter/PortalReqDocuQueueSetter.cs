using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CommonDatabaseActionReusables.RelationManager;
using CommonDatabaseActionReusables.QueueManager;
using CommonDatabaseActionReusables.NotificationManager;
using SIA_Portal.Accessors;
using SIA_Portal.Constants;

namespace SIA_PORTAL_UnitTest.DataSetter
{
    [TestClass]
    public class PortalReqDocuQueueSetter
    {

        private PortalReqDocuQueueAccessor reqDocuQueueAccessor = new PortalReqDocuQueueAccessor();
        private PortalReqDocuQueueToReqDocuAccessor reqDocuQueueToReqDocuAccessor = new PortalReqDocuQueueToReqDocuAccessor();
        private PortalReqDocuQueueToAccountAccessor reqDocuQueueToAccAccessor = new PortalReqDocuQueueToAccountAccessor();

        private Queue.Builder builder01;

        /*
        [TestCategory("Reset")]
        [TestMethod]
        public void Cleann_ResetOf_ReqDocuQueue()
        {
            DeleteAll_ReqDocuQueues();

            Create_ReqDocuQueues();
        }

        //

        [TestCategory("Create")]
        [TestMethod]
        public void Create_ReqDocuQueues()
        {
            var dateTimeNow = DateTime.Now;


            builder01 = new Queue.Builder();
            builder01.SetDescriptionUsingString("Queue Desc 01");
            builder01.PriorityLevel = 0;
            builder01.Status = QueueStatusConstants.STATUS_PENDING;
            builder01.SetStatusDescriptionUsingString("");
            builder01.DateTimeOfQueue = dateTimeNow;




        }


        [TestCategory("Delete")]
        [TestMethod]
        public void DeleteAll_ReqDocuQueues()
        {
            
        }
        */
    }
}
