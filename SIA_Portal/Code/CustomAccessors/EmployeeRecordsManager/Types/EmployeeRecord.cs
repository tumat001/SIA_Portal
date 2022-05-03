using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using System.Drawing;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager
{
    public class EmployeeRecord
    {

        private EmployeeRecord(int id,
            string firstName, string middleName, string lastName,
            DateTime birthday, string address, string contactNumber,
            string elementarySchool, string highSchool, string college,
            string previousCompanyName, string previousCompanyPositionName,
            string seminarNameAttended,
            int employeeDivCategoryId,
            string emergencyContactName, string emergencyContact)
        {
            Id = id;

            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;

            BirthDay = birthday;
            Address = address;
            ContactNumber = contactNumber;

            ElementarySchool = elementarySchool;
            HighSchool = highSchool;
            College = college;

            PreviousCompanyName = previousCompanyName;
            PreviousCompanyPositionName = previousCompanyPositionName;

            SeminarNameAttended = seminarNameAttended;

            EmployeeDivCategoryId = employeeDivCategoryId;

            EmergencyContactName = emergencyContactName;
            EmergencyContact = emergencyContact;
        }

        public int Id { get; }

        public string FirstName { set; get; }

        public string MiddleName { set; get; }

        public string LastName { set; get; }

        public DateTime BirthDay { set; get; }

        public string Address { set; get; }

        public string ContactNumber { set; get; }

        public string ElementarySchool { set; get; }

        public string HighSchool { set; get; }

        public string College { set; get; }

        public string PreviousCompanyName { set; get; }

        public string PreviousCompanyPositionName { set; get; }

        public string SeminarNameAttended { set; get; }

        public int EmployeeDivCategoryId { set; get; }

        public string EmergencyContactName { set; get; }

        public string EmergencyContact { set; get; }




        public override bool Equals(object obj)
        {
            if (obj.GetType() == this.GetType())
            {
                return ((EmployeeRecord)(obj)).Id == Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }



        public class Builder
        {

            public string FirstName { set; get; }

            public string MiddleName { set; get; }

            public string LastName { set; get; }

            public DateTime BirthDay { set; get; }

            public string Address { set; get; }

            public string ContactNumber { set; get; }

            public string ElementarySchool { set; get; }

            public string HighSchool { set; get; }

            public string College { set; get; }

            public string PreviousCompanyName { set; get; }
           
            public string PreviousCompanyPositionName { set; get; }

            public string SeminarNameAttended { set; get; }

            public int EmployeeDivCategoryId { set; get; }

            public string EmergencyContactName { set; get; }

            public string EmergencyContact { set; get; }


            internal EmployeeRecord Build(int id)
            {
                return new EmployeeRecord(id, FirstName, MiddleName, LastName,
                    BirthDay, Address, ContactNumber,
                    ElementarySchool, HighSchool, College,
                    PreviousCompanyName, PreviousCompanyPositionName,
                    SeminarNameAttended,
                    EmployeeDivCategoryId,
                    EmergencyContactName, EmergencyContact);
            }


            public EmployeeRecord.Builder GetCopy()
            {
                var builder01 = new Builder();
                builder01.FirstName = FirstName;
                builder01.MiddleName = MiddleName;
                builder01.LastName = LastName;

                builder01.BirthDay = BirthDay;
                builder01.Address = Address;
                builder01.ContactNumber = ContactNumber;

                builder01.ElementarySchool = ElementarySchool;
                builder01.HighSchool = HighSchool;
                builder01.College = College;

                builder01.PreviousCompanyName = PreviousCompanyName;
                builder01.PreviousCompanyPositionName = PreviousCompanyPositionName;

                builder01.SeminarNameAttended = SeminarNameAttended;

                builder01.EmployeeDivCategoryId = EmployeeDivCategoryId;

                builder01.EmergencyContactName = EmergencyContactName;
                builder01.EmergencyContact = EmergencyContact;

                return builder01;
            }

        }


    }
}