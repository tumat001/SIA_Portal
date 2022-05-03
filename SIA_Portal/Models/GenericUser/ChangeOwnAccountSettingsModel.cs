using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using System.ComponentModel.DataAnnotations;
using SIA_Portal.Constants;
using SIA_Portal.Models.BaseModels;

namespace SIA_Portal.Models.GenericUserModel
{
    public class ChangeOwnAccountSettingsModel : BaseAccountLoggedInModel
    {

        [Required(ErrorMessage = "Current password is required."), MaxLength(InputConstraintsConstants.PASSWORD_CHARACTER_LIMIT)]
        [DataType(DataType.Password)]
        public string InputCurrentPassword { set; get; }

        [Required(ErrorMessage = "New password is required."), MaxLength(InputConstraintsConstants.PASSWORD_CHARACTER_LIMIT)]
        [DataType(DataType.Password)]
        public string InputNewPassword { set; get; }

        [Required(ErrorMessage = "Confirm password is required."), MaxLength(InputConstraintsConstants.PASSWORD_CHARACTER_LIMIT)]
        [DataType(DataType.Password)]
        public string InputConfirmNewPassword { set; get; }


        public int ChangeOwnSettingsStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION; //ChangeStatus.NO_ACTION;

        public string StatusMessage { set; get; }


        public bool IfNewAndConfirmPasswordsAreEqual()
        {
            return InputNewPassword.Equals(InputConfirmNewPassword);
        }


        //

        public ChangeOwnAccountSettingsModel() { }

        public ChangeOwnAccountSettingsModel(object acc) : base((Account)acc) { }

    }
}