using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Constants;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager;
using SIA_Portal.Accessors;

namespace SIA_Portal.Models.GenericUserModel
{
    public class ViewEditEmployeeRecordModel : BaseAccountLoggedInModel
    {

        public ViewEditEmployeeRecordModel() { }

        //
        public int RecordId { set; get; } = -1;

        public int EmployeeId { set; get; }

        public string EmployeeUsername { set; get; }

        //

        [Required(ErrorMessage = "First Name is required")]
        [DataType(DataType.Text)]
        public string FirstName { set; get; }

        [Required(ErrorMessage = "Middle Name is required")]
        [DataType(DataType.Text)]
        public string MiddleName { set; get; }

        [Required(ErrorMessage = "Last Name is required")]
        [DataType(DataType.Text)]
        public string LastName { set; get; }


        [Required(ErrorMessage = "Birthdate is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Birthday { set; get; }

        [Required(ErrorMessage = "Address is required")]
        [DataType(DataType.MultilineText)]
        public string Address { set; get; }

        [Required(ErrorMessage = "Contact Number is required")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Input must be a phone number")]
        [StringLength(11, ErrorMessage = "Contact number should have a length of 7 to 11", MinimumLength = 7)]
        public string ContactNumber { set; get; }


        [Required(ErrorMessage = "Elementary is required")]
        [DataType(DataType.Text)]
        public string ElementarySchool { set; get; }

        [Required(ErrorMessage = "High School is required")]
        [DataType(DataType.Text)]
        public string HighSchool { set; get; }

        [Required(ErrorMessage = "College is required")]
        [DataType(DataType.Text)]
        public string College { set; get; }


        [DataType(DataType.Text)]
        public string PreviousCompanyName { set; get; }

        [DataType(DataType.Text)]
        public string PreviousCompanyPositionName { set; get; }


        [DataType(DataType.Text)]
        public string SeminarNameAttended { set; get; }

        //

        public string EmployeeDivCategoryNameSelected { set; get; }

        public IList<String> EmployeeDivCategoryList { set; get; }

        //

        [Required(ErrorMessage = "Emergency Contact Name is required")]
        [DataType(DataType.Text)]
        public string EmergencyContactName { set; get; }

        [Required(ErrorMessage = "Emergency Contact Number is required")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Input must be a phone number")]
        [StringLength(11, ErrorMessage = "Emergency Contact number should have a length of 7 to 11", MinimumLength = 7)]
        public string EmergencyContactNumber { set; get; }

        //


        public int ActionExecuteStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

        public string StatusMessage { set; get; } = "";


        //


        public EmployeeRecord.Builder PopulateBuilder(EmployeeRecord.Builder builder, 
            PortalAccountDivisionCategoryAccessor empDivCatAccessor)
        {
            builder.FirstName = FirstName;
            builder.MiddleName = MiddleName;
            builder.LastName = LastName;

            builder.BirthDay = Birthday.Value;
            builder.Address = Address;
            builder.ContactNumber = ContactNumber;

            builder.ElementarySchool = ElementarySchool;
            builder.HighSchool = HighSchool;
            builder.College = College;

            builder.PreviousCompanyName = PreviousCompanyName;
            builder.PreviousCompanyPositionName = PreviousCompanyPositionName;

            builder.SeminarNameAttended = SeminarNameAttended;

            //

            var cat = empDivCatAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromName(EmployeeDivCategoryNameSelected);
            if (cat != null)
            {
                builder.EmployeeDivCategoryId = cat.Id;
            }

            //

            builder.EmergencyContactName = EmergencyContactName;
            builder.EmergencyContact = EmergencyContactNumber;

            return builder;
        }


        public void PopulateSelfWithRecordData(EmployeeRecord record,
            PortalAccountDivisionCategoryAccessor empDivCatAccessor)
        {
            FirstName = record.FirstName;
            MiddleName = record.MiddleName;
            LastName = record.LastName;

            Birthday = record.BirthDay;
            Address = record.Address;
            ContactNumber = record.ContactNumber;

            ElementarySchool = record.ElementarySchool;
            HighSchool = record.HighSchool;
            College = record.College;

            PreviousCompanyName = record.PreviousCompanyName;
            PreviousCompanyPositionName = record.PreviousCompanyPositionName;

            SeminarNameAttended = record.SeminarNameAttended;

            var cat = empDivCatAccessor.CategoryDatabaseManagerHelper.TryGetCategoryInfoFromId(record.EmployeeDivCategoryId);
            if (cat != null)
            {
                EmployeeDivCategoryNameSelected = cat.Name;
            }

            EmergencyContactName = record.EmergencyContactName;
            EmergencyContactNumber = record.EmergencyContact;

        }

    }
}