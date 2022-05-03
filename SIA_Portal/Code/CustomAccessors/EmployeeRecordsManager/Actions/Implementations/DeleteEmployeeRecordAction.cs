using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Exceptions;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions
{
    class DeleteEmployeeRecordAction : AbstractAction<EmployeeRecordsDatabasePathConfig>
    {

        internal DeleteEmployeeRecordAction(EmployeeRecordsDatabasePathConfig config) : base(config)
        {

        }


        /// <summary>
        /// Deletes the employee record with the given <paramref name="id"/>.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="EmployeeRecordDoesNotExistException"></exception>
        /// <returns>True if the delete operation was successful. False otherwise.</returns>
        public bool DeleteEmployeeRecordWithId(int id)
        {
            bool idExists = new EmployeeRecordExistsAction(databasePathConfig).IfEmployeeRecordIdExsists(id);
            if (!idExists)
            {
                throw new EmployeeRecordDoesNotExistException(id);
            }

            //


            bool isSuccessful = false;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = string.Format("DELETE FROM [{0}] WHERE [{1}] = @Id",
                        databasePathConfig.EmployeeRecordsTableName, databasePathConfig.IdColumnName);
                    command.Parameters.Add(new SqlParameter("Id", id));

                    isSuccessful = command.ExecuteNonQuery() > 0;

                }
            }

            return isSuccessful;
        }


        /// <summary>
        /// Deletes the employee record with the given <paramref name="id"/>.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the delete operation was successful. False otherwise.</returns>
        public bool TryDeleteEmployeeRecordWithId(int id)
        {
            try
            {
                return DeleteEmployeeRecordWithId(id);
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
