using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions
{
    public class AdvancedGetEmployeeRecordsAction : AbstractAction<EmployeeRecordsDatabasePathConfig>
    {

        internal AdvancedGetEmployeeRecordsAction(EmployeeRecordsDatabasePathConfig config) : base(config)
        {

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A list of records found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>.<br/><br/>
        /// If no order by params are supplied to <paramref name="adGetParameter"/>, then the items will be sorted by the date of insertion into the database, descending.
        /// <br/><br/>Ignores <paramref name="adGetParameter"/>'s <see cref="AdvancedGetParameters.TextToContain"/></returns>
        public IReadOnlyList<EmployeeRecord> AdvancedGetEmployeeRecordsAsList(AdvancedGetParameters adGetParameter)
        {

            var list = new List<EmployeeRecord>();
            var builder = new EmployeeRecord.Builder();

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT [{0}], [{5}], [{6}], [{7}], [{8}], [{9}], [{10}], [{11}], [{12}], [{13}], [{14}], [{15}], [{16}], [{17}], [{18}], [{19}] FROM [{1}] {2} {3} {4}",
                        databasePathConfig.IdColumnName,
                        databasePathConfig.EmployeeRecordsTableName,
                        adGetParameter.GetSQLStatementFromOrderBy(databasePathConfig.IdColumnName, OrderType.DESCENDING),
                        adGetParameter.GetSQLStatementFromOffset(),
                        adGetParameter.GetSQLStatementFromFetch(),
                        
                        databasePathConfig.FirstNameColumnName, databasePathConfig.MiddleNameColumnName, databasePathConfig.LastNameColumnName,
                        databasePathConfig.BirthDayColumnName, databasePathConfig.AddressColumnName, databasePathConfig.ContactNumberColumnName,
                        databasePathConfig.ElementarySchoolColumnName, databasePathConfig.HighSchoolColumnName, databasePathConfig.CollegeColumnName,
                        databasePathConfig.PreviousCompanyNameColumnName, databasePathConfig.PreviousCompanyPositionColumnName,
                        databasePathConfig.SeminarTitleAttendedColumnName, databasePathConfig.EmployeeCategoryIdColumnName,
                        databasePathConfig.EmergencyContactNameColumnName, databasePathConfig.EmergencyContactColumnName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            builder.FirstName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(1));
                            builder.MiddleName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(2));
                            builder.LastName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(3));

                            builder.BirthDay = Convert.ToDateTime(GetParamOrNullIfParamIsDbNull(reader.GetSqlDateTime(4).Value));
                            builder.Address = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(5));
                            builder.ContactNumber = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(6));

                            builder.ElementarySchool = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(7));
                            builder.HighSchool = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(8));
                            builder.College = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(9));

                            builder.PreviousCompanyName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(10));
                            builder.PreviousCompanyPositionName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(11));

                            builder.SeminarNameAttended = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(12));
                            builder.EmployeeDivCategoryId = IntegerUtilities.ConvertSqlIntToInt(reader.GetSqlInt32(13));

                            builder.EmergencyContactName = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(14));
                            builder.EmergencyContact = StringUtilities.ConvertSqlStringToString(reader.GetSqlString(15));


                            var id = reader.GetSqlInt32(0).Value;

                            list.Add(builder.Build(id));
                        }
                    }
                }
            }

            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <returns>A list of records found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>.<br/><br/>
        /// If no order by params are supplied to <paramref name="adGetParameter"/>, then the items will be sorted by the date of insertion into the database, descending.
        /// <br/><br/>Ignores <paramref name="adGetParameter"/>'s <see cref="AdvancedGetParameters.TextToContain"/></returns>
        public IReadOnlyList<EmployeeRecord> TryAdvancedGetRecordsAsList(AdvancedGetParameters adGetParameter)
        {
            try
            {
                return AdvancedGetEmployeeRecordsAsList(adGetParameter);
            }
            catch (Exception)
            {
                return new List<EmployeeRecord>();
            }
        }


        //



        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>The number of employee records found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>. <see cref="AdvancedGetParameters.OrderByParameters"/> has no effect.<br/><br/>
        /// <br/><br/>Ignores <paramref name="adGetParameter"/>'s <see cref="AdvancedGetParameters.TextToContain"/></returns>
        public int AdvancedGetEmployeeRecordsCount(AdvancedGetParameters adGetParameter)
        {
            var count = -1;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    
                    command.CommandText = String.Format("SELECT COUNT(*) FROM (SELECT [{0}] FROM [{1}] {2} {3} {4}) T",
                        databasePathConfig.IdColumnName,
                        databasePathConfig.EmployeeRecordsTableName,
                        adGetParameter.GetSQLStatementFromOrderBy(databasePathConfig.IdColumnName, OrderType.DESCENDING),
                        adGetParameter.GetSQLStatementFromOffset(),
                        adGetParameter.GetSQLStatementFromFetch());


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            count = reader.GetSqlInt32(0).Value;
                        }
                    }
                }
            }

            return count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>The number of employee records found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>. <see cref="AdvancedGetParameters.OrderByParameters"/> has no effect.<br/><br/>
        /// <br/><br/>Ignores <paramref name="adGetParameter"/>'s <see cref="AdvancedGetParameters.TextToContain"/></returns>
        public int TryAdvancedGetEmployeeRecordsCount(AdvancedGetParameters adGetParameter)
        {
            try
            {
                return AdvancedGetEmployeeRecordsCount(adGetParameter);
            }
            catch (Exception)
            {
                return -1;
            }
        }

    }
}