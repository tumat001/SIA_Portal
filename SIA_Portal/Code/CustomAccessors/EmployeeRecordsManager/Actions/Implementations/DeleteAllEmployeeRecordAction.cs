using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions
{
    public class DeleteAllEmployeeRecordAction : AbstractAction<EmployeeRecordsDatabasePathConfig>
    {

        internal DeleteAllEmployeeRecordAction(EmployeeRecordsDatabasePathConfig config) : base(config)
        {

        }


        /// <summary>
        /// Deletes all employee records.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>True if the delete operation was successful, even if no announcment was deleted.</returns>
        public bool DeleteAllEmployeeRecords()
        {

            bool isSuccessful = true;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM [{0}]",
                        databasePathConfig.EmployeeRecordsTableName);

                    command.ExecuteNonQuery();

                }
            }

            return isSuccessful;

        }


        /// <summary>
        /// Deletes all employee records.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>True if the delete operation was successful, even if no announcment was deleted.</returns>
        public bool TryDeleteAllEmployeeRecords()
        {
            try
            {
                return DeleteAllEmployeeRecords();
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}