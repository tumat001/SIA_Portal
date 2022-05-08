using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AnnouncementManager.Configs;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using SIA_Portal.CustomAccessors.RequestableDocument.PathConfig;

namespace SIA_Portal.CustomAccessors.RequestableDocument.Actions
{
    public class DeleteAllRequestableDocumentAction : AbstractAction<ReqDocuPathConfig>
    {

        internal DeleteAllRequestableDocumentAction(ReqDocuPathConfig config) : base(config)
        {

        }


        /// <summary>
        /// Deletes all requestable documents.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>True if the delete operation was successful, even if no announcment was deleted.</returns>
        public bool DeleteAllRequestableDocuments()
        {

            bool isSuccessful = true;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM [{0}]",
                        databasePathConfig.ReqDocuTableName);

                    command.ExecuteNonQuery();

                }
            }

            return isSuccessful;

        }


        /// <summary>
        /// Deletes all announcements.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <returns>True if the delete operation was successful, even if no announcment was deleted.</returns>
        public bool TryDeleteAllRequestableDocuments()
        {
            try
            {
                return DeleteAllRequestableDocuments();
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}