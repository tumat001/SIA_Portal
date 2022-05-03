using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions
{
    public class EmployeeRecordExistsAction : AbstractAction<EmployeeRecordsDatabasePathConfig>
    {

        internal EmployeeRecordExistsAction(EmployeeRecordsDatabasePathConfig config) : base(config)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>True if the employee record id exists in the given parameters of <see cref="DatabasePathConfig"/></returns>
        public bool IfEmployeeRecordIdExsists(int id)
        {
            bool result = false;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT [{0}] FROM [{1}] WHERE [{0}] = @TargetId",
                        databasePathConfig.IdColumnName, databasePathConfig.EmployeeRecordsTableName);
                    command.Parameters.Add(new SqlParameter("TargetId", id));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        result = reader.HasRows;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the employee record id exists in the given parameters of <see cref="DatabasePathConfig"/></returns>
        public bool TryIfEmployeeRecordIdExsists(int id)
        {
            try
            {
                return IfEmployeeRecordIdExsists(id);
            }
            catch (Exception)
            {
                return false;
            }

        }

    }
}