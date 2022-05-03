using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Exceptions;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.GeneralUtilities.InputConstraints;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions
{
    public class EditEmployeeRecordAction : AbstractAction<EmployeeRecordsDatabasePathConfig>
    {

        internal EditEmployeeRecordAction(EmployeeRecordsDatabasePathConfig config) : base(config)
        {

        }


        /// <summary>
        /// Edits the employee record with the provided <paramref name="id"/> using the properties found in <paramref name="builder"/>.<br/>
        /// Setting <paramref name="builder"/> to null makes no edits to the employee record.<br/><br/>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="builder"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="EmployeeRecordDoesNotExistException"></exception>
        /// <exception cref="InputStringConstraintsViolatedException"></exception>
        /// <returns>True if the employee record was edited successfully, even if <paramref name="builder"/> is set to null.</returns>
        public bool EditEmployeeRecord(int id, EmployeeRecord.Builder builder)
        {
            bool idExists = new EmployeeRecordExistsAction(databasePathConfig).IfEmployeeRecordIdExsists(id);
            if (!idExists)
            {
                throw new EmployeeRecordDoesNotExistException(id);
            }

            if (builder == null)
            {
                return true;
            }


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


            var success = false;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("UPDATE [{0}] SET [{1}] = @FirstName, [{2}] = @MiddleName, [{3}] = @LastName," +
                        "[{4}] = @Birthday, [{5}] = @Address, [{6}] = @ContactNumber," +
                        "[{7}] = @ElementarySchool, [{8}] = @HighSchool, [{9}] = @College," +
                        "[{10}] = @PreviousCompanyName, [{11}] = @PreviousCompanyPositionName," +
                        "[{12}] = @SeminarNameAttended," +
                        "[{13}] = @EmployeeDivCategoryId," +
                        "[{14}] = @EmergencyContactName, [{15}] = @EmergencyContact " +
                        "WHERE [{16}] = @IdVal",
                        databasePathConfig.EmployeeRecordsTableName,

                        databasePathConfig.FirstNameColumnName, databasePathConfig.MiddleNameColumnName, databasePathConfig.LastNameColumnName,
                        databasePathConfig.BirthDayColumnName, databasePathConfig.AddressColumnName, databasePathConfig.ContactNumberColumnName,
                        databasePathConfig.ElementarySchoolColumnName, databasePathConfig.HighSchoolColumnName, databasePathConfig.CollegeColumnName,
                        databasePathConfig.PreviousCompanyNameColumnName, databasePathConfig.PreviousCompanyPositionColumnName,
                        databasePathConfig.SeminarTitleAttendedColumnName,
                        databasePathConfig.EmployeeCategoryIdColumnName,
                        databasePathConfig.EmergencyContactNameColumnName, databasePathConfig.EmergencyContactColumnName,

                        databasePathConfig.IdColumnName);
                    
                    command.Parameters.Add(new SqlParameter("FirstName", GetParamOrDbNullIfParamIsNull(builder.FirstName)));
                    command.Parameters.Add(new SqlParameter("MiddleName", GetParamOrDbNullIfParamIsNull(builder.MiddleName)));
                    command.Parameters.Add(new SqlParameter("LastName", GetParamOrDbNullIfParamIsNull(builder.LastName)));

                    command.Parameters.Add(new SqlParameter("BirthDay", GetParamOrDbNullIfParamIsNull(builder.BirthDay)));
                    command.Parameters.Add(new SqlParameter("Address", GetParamOrDbNullIfParamIsNull(builder.Address)));
                    command.Parameters.Add(new SqlParameter("ContactNumber", GetParamOrDbNullIfParamIsNull(builder.ContactNumber)));

                    command.Parameters.Add(new SqlParameter("ElementarySchool", GetParamOrDbNullIfParamIsNull(builder.ElementarySchool)));
                    command.Parameters.Add(new SqlParameter("HighSchool", GetParamOrDbNullIfParamIsNull(builder.HighSchool)));
                    command.Parameters.Add(new SqlParameter("College", GetParamOrDbNullIfParamIsNull(builder.College)));

                    command.Parameters.Add(new SqlParameter("PreviousCompanyName", GetParamOrDbNullIfParamIsNull(builder.PreviousCompanyName)));
                    command.Parameters.Add(new SqlParameter("PreviousCompanyPositionName", GetParamOrDbNullIfParamIsNull(builder.PreviousCompanyPositionName)));

                    command.Parameters.Add(new SqlParameter("SeminarNameAttended", GetParamOrDbNullIfParamIsNull(builder.SeminarNameAttended)));

                    command.Parameters.Add(new SqlParameter("EmployeeDivCategoryId", GetParamOrDbNullIfParamIsNegativeInt(builder.EmployeeDivCategoryId)));

                    command.Parameters.Add(new SqlParameter("EmergencyContactName", GetParamOrDbNullIfParamIsNull(builder.EmergencyContactName)));
                    command.Parameters.Add(new SqlParameter("EmergencyContact", GetParamOrDbNullIfParamIsNull(builder.EmergencyContact)));


                    command.Parameters.Add(new SqlParameter("IdVal", id));

                    success = command.ExecuteNonQuery() > 0;
                }
            }

            return success;

        }


        /// <summary>
        /// Edits the employee record with the provided <paramref name="id"/> using the properties found in <paramref name="builder"/>.<br/>
        /// Setting <paramref name="builder"/> to null makes no edits to the employee record.<br/><br/>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="builder"></param>
        /// <returns>True if the employee record was edited successfully, even if <paramref name="builder"/> is set to null.</returns>
        public bool TryEditEmployeeRecord(int id, EmployeeRecord.Builder builder)
        {
            try
            {
                return EditEmployeeRecord(id, builder);
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}