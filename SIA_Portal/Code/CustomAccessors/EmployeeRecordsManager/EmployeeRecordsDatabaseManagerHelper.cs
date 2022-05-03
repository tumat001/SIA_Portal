using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager
{
    public class EmployeeRecordsDatabaseManagerHelper
    {

        public EmployeeRecordsDatabasePathConfig PathConfig { set; get; }


        public EmployeeRecordsDatabaseManagerHelper(EmployeeRecordsDatabasePathConfig argPathConfig)
        {
            PathConfig = argPathConfig;
        }

        //

        #region "IfExists"

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
            return new EmployeeRecordExistsAction(PathConfig).IfEmployeeRecordIdExsists(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the employee record id exists in the given parameters of <see cref="DatabasePathConfig"/></returns>
        public bool TryIfEmployeeRecordIdExsists(int id)
        {
            return new EmployeeRecordExistsAction(PathConfig).TryIfEmployeeRecordIdExsists(id);
        }

        #endregion

        //

        #region "Create"

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
        /// <returns>The Id corresponding to the created employee record.</returns>
        public int CreateEmployeeRecord(EmployeeRecord.Builder builder)
        {
            return new CreateEmployeeRecordAction(PathConfig).CreateEmployeeRecord(builder);
        }

        /// <summary>
        /// Creates an employee record with the given parameters in the given <paramref name="builder"/>.<br></br>
        /// The employee record is created in the database and table specified in this object's <see cref="DatabasePathConfig"/><br/><br/>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>The Id corresponding to the created employee record.</returns>
        public int TryCreateEmployeeRecord(EmployeeRecord.Builder builder)
        {
            return new CreateEmployeeRecordAction(PathConfig).TryCreateEmployeeRecord(builder);
        }

        #endregion

        //

        #region "Delete All"

        /// <summary>
        /// Deletes all employee records.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>True if the delete operation was successful, even if no announcment was deleted.</returns>
        public bool DeleteAllEmployeeRecords()
        {
            return new DeleteAllEmployeeRecordAction(PathConfig).DeleteAllEmployeeRecords();
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
            return new DeleteAllEmployeeRecordAction(PathConfig).TryDeleteAllEmployeeRecords();
        }

        #endregion

        //

        #region "Get"

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
            return new GetEmployeeRecordAction(PathConfig).GetEmployeeRecordInfoFromId(id);
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
            return new GetEmployeeRecordAction(PathConfig).TryGetEmployeeRecordInfoFromId(id);
        }

        #endregion

        //

        #region "Advanced Get "

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
            return new AdvancedGetEmployeeRecordsAction(PathConfig).AdvancedGetEmployeeRecordsAsList(adGetParameter);
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
            return new AdvancedGetEmployeeRecordsAction(PathConfig).TryAdvancedGetRecordsAsList(adGetParameter);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>The number of employee records found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>. <see cref="AdvancedGetParameters.OrderByParameters"/> has no effect.
        /// <br/><br/>Ignores <paramref name="adGetParameter"/>'s <see cref="AdvancedGetParameters.TextToContain"/><br/><br/>
        /// </returns>
        public int AdvancedGetEmployeeRecordsCount(AdvancedGetParameters adGetParameter)
        {
            return new AdvancedGetEmployeeRecordsAction(PathConfig).AdvancedGetEmployeeRecordsCount(adGetParameter);
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
            return new AdvancedGetEmployeeRecordsAction(PathConfig).TryAdvancedGetEmployeeRecordsCount(adGetParameter);
        }

        #endregion

        //

        #region "Get All"

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>A list of employee records found in the database given in this object's <see cref="DatabasePathConfig"/></returns>
        public IReadOnlyList<EmployeeRecord> GetAllEmployeeRecordsAsList()
        {
            return new GetAllEmployeeRecordAction(PathConfig).GetAllEmployeeRecordsAsList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A list of employee records found in the database given in this object's <see cref="DatabasePathConfig"/>. Returns an empty list if an exception occurs.</returns>
        public IReadOnlyList<EmployeeRecord> TryGetEmployeeRecordsAsList()
        {
            return new GetAllEmployeeRecordAction(PathConfig).TryGetEmployeeRecordsAsList();
        }

        #endregion

        //

        #region "Edit"

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
            return new EditEmployeeRecordAction(PathConfig).EditEmployeeRecord(id, builder);
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
            return new EditEmployeeRecordAction(PathConfig).TryEditEmployeeRecord(id, builder);
        }

        #endregion

        //

        #region "Delete"

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
            return new DeleteEmployeeRecordAction(PathConfig).DeleteEmployeeRecordWithId(id);
        }


        /// <summary>
        /// Deletes the employee record with the given <paramref name="id"/>.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the delete operation was successful. False otherwise.</returns>
        public bool TryDeleteEmployeeRecordWithId(int id)
        {
            return new DeleteEmployeeRecordAction(PathConfig).TryDeleteEmployeeRecordWithId(id);
        }

        #endregion


    }
}