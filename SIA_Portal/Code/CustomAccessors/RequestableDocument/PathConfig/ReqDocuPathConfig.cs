using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;

namespace SIA_Portal.CustomAccessors.RequestableDocument.PathConfig
{
    public class ReqDocuPathConfig : DatabasePathConfig
    {

        public string ReqDocuTableName { get; }

        
        public string IdColumnName { get; }

        public string NameColumnName { get; }

        public string NoteDescriptionColumnName { get; }


        public ReqDocuPathConfig(string connString, string idColName, string nameColName, string noteDescColumnName,
            string reqDocuTableName) : base(connString)
        {
            IdColumnName = idColName;
            NameColumnName = nameColName;
            NoteDescriptionColumnName = noteDescColumnName;

            ReqDocuTableName = reqDocuTableName;
        }

    }
}