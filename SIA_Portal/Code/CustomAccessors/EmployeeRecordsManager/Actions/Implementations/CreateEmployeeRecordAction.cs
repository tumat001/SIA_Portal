using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.GeneralUtilities.InputConstraints;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions
{
    public class CreateEmployeeRecordAction : AbstractAction<EmployeeRecordsDatabasePathConfig>
    {
        internal CreateEmployeeRecordAction(EmployeeRecordsDatabasePathConfig config) : base(config)
        {

        }


        /// <summary>
        /// Creates an employee record with the given parameters in the given <paramref name="builder"/>.<br></br>
        /// The employee record is created in the database and table specified in this object's <see cref="DatabasePathConfig"/><br/><br/>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="InputStringConstraintsViolatedException"></exception>
        /// <returns>The Id corresponding to the created announcement.</returns>
        public int CreateEmployeeRecord(EmployeeRecord.Builder builder)
        {
            var inputConstraintsChecker = new AntiSQLInjectionInputConstraint();
            inputConstraintsChecker.SatisfiesConstraint(builder.FirstName);
            inputConstraintsChecker.SatisfiesConstraint(builder.MiddleName);
            inputConstraintsChecker.SatisfiesConstraint(builder.LastName);

            inputConstraintsChecker.SatisfiesConstraint(builder.Address);

            inputConstraintsChecker.SatisfiesConstraint(builder.ElementarySchool);
            inputConstraintsChecker.SatisfiesConstraint(builder.HighSchool);
            inputConstraintsChecker.SatisfiesConstraint(builder.College);

            inputConstraintsChecker.SatisfiesConstraint(builder.PreviousCompanyName);
            inputConstraintsChecker.SatisfiesConstraint(builder.PreviousCompanyPositionName);

            inputConstraintsChecker.SatisfiesConstraint(builder.SeminarNameAttended);

            inputConstraintsChecker.SatisfiesConstraint(builder.EmergencyContactName);

            //

            int recordId = -1;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("INSERT INTO [{0}] ({1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}) VALUES (@FirstName, @MiddleName, @LastName, @Birthday, @Address," +
                        " @ContactNumber, @ElementarySchool, @HighSchool, @College, @PreviousCompanyName, @PreviousCompanyPosition, @SeminarAttended, @EmployeeCatId," +
                        " @EmergencyContactName, @EmergencyContact); SELECT SCOPE_IDENTITY()",
                        databasePathConfig.EmployeeRecordsTableName,

                        databasePathConfig.FirstNameColumnName, databasePathConfig.MiddleNameColumnName, databasePathConfig.LastNameColumnName,
                        databasePathConfig.BirthDayColumnName, databasePathConfig.AddressColumnName, databasePathConfig.ContactNumberColumnName,
                        databasePathConfig.ElementarySchoolColumnName, databasePathConfig.HighSchoolColumnName, databasePathConfig.CollegeColumnName,
                        databasePathConfig.PreviousCompanyNameColumnName, databasePathConfig.PreviousCompanyPositionColumnName,
                        databasePathConfig.SeminarTitleAttendedColumnName,
                        databasePathConfig.EmployeeCategoryIdColumnName,
                        databasePathConfig.EmergencyContactNameColumnName, databasePathConfig.EmergencyContactColumnName);
                    
                    command.Parameters.Add(new SqlParameter("FirstName", GetParamOrDbNullIfParamIsNull(builder.FirstName)));
                    command.Parameters.Add(new SqlParameter("MiddleName", GetParamOrDbNullIfParamIsNull(builder.MiddleName)));
                    command.Parameters.Add(new SqlParameter("LastName", GetParamOrDbNullIfParamIsNull(builder.LastName)));

                    command.Parameters.Add(new SqlParameter("Birthday", GetParamOrDbNullIfParamIsNull(builder.BirthDay)));
                    command.Parameters.Add(new SqlParameter("Address", GetParamOrDbNullIfParamIsNull(builder.Address)));
                    command.Parameters.Add(new SqlParameter("ContactNumber", GetParamOrDbNullIfParamIsNull(builder.ContactNumber)));

                    command.Parameters.Add(new SqlParameter("ElementarySchool", GetParamOrDbNullIfParamIsNull(builder.ElementarySchool)));
                    command.Parameters.Add(new SqlParameter("HighSchool", GetParamOrDbNullIfParamIsNull(builder.HighSchool)));
                    command.Parameters.Add(new SqlParameter("College", GetParamOrDbNullIfParamIsNull(builder.College)));

                    command.Parameters.Add(new SqlParameter("PreviousCompanyName", GetParamOrDbNullIfParamIsNull(builder.PreviousCompanyName)));
                    command.Parameters.Add(new SqlParameter("PreviousCompanyPosition", GetParamOrDbNullIfParamIsNull(builder.PreviousCompanyPositionName)));

                    command.Parameters.Add(new SqlParameter("SeminarAttended", GetParamOrDbNullIfParamIsNull(builder.SeminarNameAttended)));
                    command.Parameters.Add(new SqlParameter("EmployeeCatId", GetParamOrDbNullIfParamIsNegativeInt(builder.EmployeeDivCategoryId)));

                    command.Parameters.Add(new SqlParameter("EmergencyContactName", GetParamOrDbNullIfParamIsNull(builder.EmergencyContactName)));
                    command.Parameters.Add(new SqlParameter("EmergencyContact", GetParamOrDbNullIfParamIsNull(builder.EmergencyContact)));

                    recordId = int.Parse(command.ExecuteScalar().ToString());
                }
            }

            return recordId;
        }


        /// <summary>
        /// Creates an employee record with the given parameters in the given <paramref name="builder"/>.<br></br>
        /// The employee record is created in the database and table specified in this object's <see cref="DatabasePathConfig"/><br/><br/>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>The Id corresponding to the created announcement.</returns>
        public int TryCreateEmployeeRecord(EmployeeRecord.Builder builder)
        {
            try
            {
                return CreateEmployeeRecord(builder);
            }
            catch (Exception)
            {
                return -1;
            }
        }


    }
}