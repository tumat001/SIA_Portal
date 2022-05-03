using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Exceptions;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.GeneralUtilities.InputConstraints;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions
{
    public class GetEmployeeRecordAction : AbstractAction<EmployeeRecordsDatabasePathConfig>
    {

        internal GetEmployeeRecordAction(EmployeeRecordsDatabasePathConfig config) : base(config)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="EmployeeRecordDoesNotExistException"></exception>
        /// <returns>An <see cref="EmployeeRecord"/> object containing information about the employee record with the provided <paramref name="id"/>.</returns>
        public EmployeeRecord GetEmployeeRecordInfoFromId(int id)
        {

            //

            bool idExists = new EmployeeRecordExistsAction(databasePathConfig).IfEmployeeRecordIdExsists(id);
            if (!idExists)
            {
                throw new EmployeeRecordDoesNotExistException(id);
            }

            //

            var builder = new EmployeeRecord.Builder();
            EmployeeRecord announcement = null;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT [{1}], [{2}], [{3}], [{4}], [{5}], [{6}]," +
                        "[{7}], [{8}], [{9}], [{10}], [{11}], [{12}], [{13}], [{14}], [{15}] " +
                        "FROM [{16}] WHERE [{0}] = @IdVal",
                        databasePathConfig.IdColumnName,

                        databasePathConfig.FirstNameColumnName, databasePathConfig.MiddleNameColumnName, databasePathConfig.LastNameColumnName,
                        databasePathConfig.BirthDayColumnName, databasePathConfig.AddressColumnName, databasePathConfig.ContactNumberColumnName,
                        databasePathConfig.ElementarySchoolColumnName, databasePathConfig.HighSchoolColumnName, databasePathConfig.CollegeColumnName,
                        databasePathConfig.PreviousCompanyNameColumnName, databasePathConfig.PreviousCompanyPositionColumnName,
                        databasePathConfig.SeminarTitleAttendedColumnName, databasePathConfig.EmployeeCategoryIdColumnName,
                        databasePathConfig.EmergencyContactNameColumnName, databasePathConfig.EmergencyContactColumnName,

                        databasePathConfig.EmployeeRecordsTableName);

                    command.Parameters.Add(new SqlParameter("IdVal", id));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            builder.FirstName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(0));
                            builder.MiddleName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(1));
                            builder.LastName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(2));

                            builder.BirthDay = (DateTime)GetParamOrNullIfParamIsDbNull(reader.GetSqlDateTime(3).Value);
                            builder.Address = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(4));
                            builder.ContactNumber = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(5));

                            builder.ElementarySchool = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(6));
                            builder.HighSchool = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(7));
                            builder.College = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(8));

                            builder.PreviousCompanyName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(9));
                            builder.PreviousCompanyPositionName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(10));

                            builder.SeminarNameAttended = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(11));
                            builder.EmployeeDivCategoryId = IntegerUtilities.ConvertSqlIntToInt(reader.GetSqlInt32(12));

                            builder.EmergencyContactName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(13));
                            builder.EmergencyContact = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(14));

                            announcement = builder.Build(id);
                        }
                    }
                }
            }


            return announcement;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="EmployeeRecordDoesNotExistException"></exception>
        /// <returns>An <see cref="EmployeeRecord"/> object containing information about the employee record with the provided <paramref name="id"/>.</returns>
        public EmployeeRecord TryGetEmployeeRecordInfoFromId(int id)
        {
            try
            {
                return GetEmployeeRecordInfoFromId(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}