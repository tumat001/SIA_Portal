using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CommonDatabaseActionReusables.DocumentManager;
using SIA_Portal.Accessors;

namespace SIA_PORTAL_UnitTest.DataSetter
{
    [TestClass]
    public class PortalDocumentSetter
    {

        private PortalDocumentFileAccessor documentFileAccessor = new PortalDocumentFileAccessor();


        //


        [TestCategory("Delete")]
        [TestMethod]
        public void DeleteAll_Documents()
        {
            documentFileAccessor.DocumentDatabaseManagerHelper.DeleteAllDocuments();

        }

    }
}
