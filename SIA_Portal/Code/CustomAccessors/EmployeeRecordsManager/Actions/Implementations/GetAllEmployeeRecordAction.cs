using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.GeneralUtilities.InputConstraints;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions
{
    public class GetAllEmployeeRecordAction : AbstractAction<EmployeeRecordsDatabasePathConfig>
    {

        internal GetAllEmployeeRecordAction(EmployeeRecordsDatabasePathConfig config) : base(config)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>A list of employee records found in the database given in this object's <see cref="DatabasePathConfig"/></returns>
        public IReadOnlyList<EmployeeRecord> GetAllEmployeeRecordsAsList()
        {
            return new AdvancedGetEmployeeRecordsAction(databasePathConfig).AdvancedGetEmployeeRecordsAsList(new AdvancedGetParameters());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>A list of employee records found in the database given in this object's <see cref="DatabasePathConfig"/>. Returns an empty list if an exception occurs.</returns>
        public IReadOnlyList<EmployeeRecord> TryGetEmployeeRecordsAsList()
        {
            try
            {
                return GetAllEmployeeRecordsAsList();
            }
            catch (Exception)
            {
                return new List<EmployeeRecord>();
            }
        }


    }
}