using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Actions;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager;

namespace SIA_Portal.Accessors
{
    public class PortalEmployeeRecordAccessor
    {

        //Change these values upon changing database
        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string TABLE_NAME = "EmployeeRecordsTable";

        const string ID_COL_NAME = "Id";
        const string FIRST_NAME_COL_NAME = "FirstName";
        const string MIDDLE_NAME_COL_NAME = "MiddleName";
        const string LAST_NAME_COL_NAME = "LastName";

        const string BIRTHDAY_COL_NAME = "Birthday";
        const string ADDRESS_COL_NAME = "Address";
        const string CONTACT_NO_COL_NAME = "ContactNumber";

        const string ELEMENTARY_SCHOOL_COL_NAME = "ElementarySchool";
        const string HIGH_SCHOOL_COL_NAME = "HighSchool";
        const string COLLEGE_COL_NAME = "College";

        const string PREVIOUS_COMPANY_NAME_COL_NAME = "PreviousCompanyName";
        const string PREVIOUS_COMPANY_POSITION_COL_NAME = "PreviousCompanyPositionName";

        const string SEMINAR_NAME_ATTENDED_COL_NAME = "SeminarNameAttended";

        const string EMPLOYEE_DIV_CAT_ID_COL_NAME = "EmployeeDivCategoryId";

        const string EMERGENCY_CONTACT_NAME_COL_NAME = "EmergencyContactName";
        const string EMERGENCY_CONTACT_COL_NAME = "EmergencyContact";




        public EmployeeRecordsDatabasePathConfig EmployeeRecordsDatabasePathConfig { get; }

        public EmployeeRecordsDatabaseManagerHelper EmployeeRecordDatabaseManagerHelper { get; }


        public PortalEmployeeRecordAccessor()
        {
            EmployeeRecordsDatabasePathConfig = new EmployeeRecordsDatabasePathConfig(DATABASE_CONN_STRING, TABLE_NAME);

            EmployeeRecordsDatabasePathConfig.IdColumnName = ID_COL_NAME;

            EmployeeRecordsDatabasePathConfig.FirstNameColumnName = FIRST_NAME_COL_NAME;
            EmployeeRecordsDatabasePathConfig.MiddleNameColumnName = MIDDLE_NAME_COL_NAME;
            EmployeeRecordsDatabasePathConfig.LastNameColumnName = LAST_NAME_COL_NAME;

            EmployeeRecordsDatabasePathConfig.BirthDayColumnName = BIRTHDAY_COL_NAME;
            EmployeeRecordsDatabasePathConfig.AddressColumnName = ADDRESS_COL_NAME;
            EmployeeRecordsDatabasePathConfig.ContactNumberColumnName = CONTACT_NO_COL_NAME;

            EmployeeRecordsDatabasePathConfig.ElementarySchoolColumnName = ELEMENTARY_SCHOOL_COL_NAME;
            EmployeeRecordsDatabasePathConfig.HighSchoolColumnName = HIGH_SCHOOL_COL_NAME;
            EmployeeRecordsDatabasePathConfig.CollegeColumnName = COLLEGE_COL_NAME;

            EmployeeRecordsDatabasePathConfig.PreviousCompanyNameColumnName = PREVIOUS_COMPANY_NAME_COL_NAME;
            EmployeeRecordsDatabasePathConfig.PreviousCompanyPositionColumnName = PREVIOUS_COMPANY_POSITION_COL_NAME;

            EmployeeRecordsDatabasePathConfig.SeminarTitleAttendedColumnName = SEMINAR_NAME_ATTENDED_COL_NAME;

            EmployeeRecordsDatabasePathConfig.EmployeeCategoryIdColumnName = EMPLOYEE_DIV_CAT_ID_COL_NAME;

            EmployeeRecordsDatabasePathConfig.EmergencyContactNameColumnName = EMERGENCY_CONTACT_NAME_COL_NAME;
            EmployeeRecordsDatabasePathConfig.EmergencyContactColumnName = EMERGENCY_CONTACT_COL_NAME;


            EmployeeRecordDatabaseManagerHelper = new EmployeeRecordsDatabaseManagerHelper(EmployeeRecordsDatabasePathConfig);
        }

    }
}