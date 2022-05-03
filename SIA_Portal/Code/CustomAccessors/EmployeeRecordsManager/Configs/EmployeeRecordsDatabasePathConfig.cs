using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Configs
{
    public class EmployeeRecordsDatabasePathConfig : DatabasePathConfig
    {

        public string IdColumnName { set; get; }

        public string FirstNameColumnName { set; get; }

        public string MiddleNameColumnName { set; get; }

        public string LastNameColumnName { set; get; }

        public string BirthDayColumnName { set; get; }

        public string AddressColumnName { set; get; }

        public string ContactNumberColumnName { set; get; }


        public string ElementarySchoolColumnName { set; get; }

        public string HighSchoolColumnName { set; get; }

        public string CollegeColumnName { set; get; }


        public string PreviousCompanyNameColumnName { set; get; }

        public string PreviousCompanyPositionColumnName { set; get; }


        public string SeminarTitleAttendedColumnName { set; get; }

        public string EmployeeCategoryIdColumnName { set; get; } //employee div category


        public string EmergencyContactNameColumnName { set; get; }

        public string EmergencyContactColumnName { set; get; }



        public string EmployeeRecordsTableName { get; }


        public EmployeeRecordsDatabasePathConfig(string connString, string employeeRecordsTableName) : base(connString)
        {
            EmployeeRecordsTableName = employeeRecordsTableName;
        }


    }
}