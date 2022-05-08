using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.RequestableDocument.PathConfig;
using SIA_Portal.CustomAccessors.RequestableDocument;

namespace SIA_Portal.CustomAccessors
{
    public class PortalRequestDocuAccessor
    {

        //Change these values upon changing database
        const string DOCU_DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string DOCU_TABLE_NAME = "RequestableDocumentTable";

        const string DOCU_ID_COL_NAME = "Id";
        const string DOCU_NAME_COL_NAME = "Name";
        const string DOCU_NOTE_DESCRIPTION_COL_NAME = "NoteDescription";
        

        public ReqDocuPathConfig ReqDocuDatabasePathConfig { get; }

        public RequestDocumentManagerHelper ReqDocuManagerHelper { get; }


        public PortalRequestDocuAccessor()
        {
            ReqDocuDatabasePathConfig = new ReqDocuPathConfig(DOCU_DATABASE_CONN_STRING, DOCU_ID_COL_NAME, DOCU_NAME_COL_NAME,
                DOCU_NOTE_DESCRIPTION_COL_NAME, DOCU_TABLE_NAME);

            ReqDocuManagerHelper = new RequestDocumentManagerHelper(ReqDocuDatabasePathConfig);
        }

    }
}