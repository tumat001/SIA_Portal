using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDatabaseActionReusables.AnnouncementManager.Configs;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.AnnouncementManager.Exceptions;
using SIA_Portal.CustomAccessors.RequestableDocument.Exceptions;
using SIA_Portal.CustomAccessors.RequestableDocument.PathConfig;

namespace SIA_Portal.CustomAccessors.RequestableDocument.Actions
{
    class DeleteRequestableDocumentAction : AbstractAction<ReqDocuPathConfig>
    {

        internal DeleteRequestableDocumentAction(ReqDocuPathConfig config) : base(config)
        {

        }


        /// <summary>
        /// Deletes the requestable document with the given <paramref name="id"/>.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="RequestableDocumentDoesNotExistException"></exception>
        /// <returns>True if the delete operation was successful. False otherwise.</returns>
        public bool DeleteRequestableDocumentWithId(int id)
        {
            bool idExists = new RequestableDocumentExistsAction(databasePathConfig).TryIfRequestDocumentIdExsists(id);
            if (!idExists)
            {
                throw new RequestableDocumentDoesNotExistException(id);
            }

            //


            bool isSuccessful = false;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM [{0}] WHERE [{1}] = @Id",
                        databasePathConfig.ReqDocuTableName, databasePathConfig.IdColumnName);
                    command.Parameters.Add(new SqlParameter("Id", id));

                    isSuccessful = command.ExecuteNonQuery() > 0;

                }
            }

            return isSuccessful;
        }


        /// <summary>
        /// Deletes the requestable document with the given <paramref name="id"/>.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the delete operation was successful. False otherwise.</returns>
        public bool TryDeleteRequestableDocumentWithId(int id)
        {
            try
            {
                return DeleteRequestableDocumentWithId(id);
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
