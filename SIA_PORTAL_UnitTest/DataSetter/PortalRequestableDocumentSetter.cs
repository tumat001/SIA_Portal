using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using SIA_Portal.Accessors;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Constants;
using SIA_Portal.CustomAccessors;
using SIA_Portal.CustomAccessors.RequestableDocument;

namespace SIA_PORTAL_UnitTest.DataSetter
{
    [TestClass]
    public class PortalRequestableDocumentSetter
    {

        private static PortalRequestDocuAccessor portalReqDocuAccessor;


        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            portalReqDocuAccessor = new PortalRequestDocuAccessor();
        }


        //

        [TestCategory("Reset")]
        [TestMethod]
        public void Cleann_ResetOf_ReqDocus()
        {
            DeleteAll_ReqDocus();

            Create_ReqDocus();
        }



        [TestCategory("Create")]
        [TestMethod]
        public void Create_ReqDocus()
        {
            var builder = new RequestableDocument.Builder();
            builder.DocumentName = "Certificate of Employment";
            builder.SetNoteDescriptionWithString("Please indicate the reasons for requesting the document.");
            portalReqDocuAccessor.ReqDocuManagerHelper.CreateRequestableDocument(builder);

            var builder02 = new RequestableDocument.Builder();
            builder02.DocumentName = "Certificate of XYZ";
            builder02.SetNoteDescriptionWithString("Please indicate the reasons for requesting the document.");
            portalReqDocuAccessor.ReqDocuManagerHelper.CreateRequestableDocument(builder02);

        }


        [TestCategory("Delete")]
        [TestMethod]
        public void DeleteAll_ReqDocus()
        {
            portalReqDocuAccessor.ReqDocuManagerHelper.DeleteAllRequestableDocuments();

        }
    }
}