using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Constants;
using System.ComponentModel.DataAnnotations;
using SIA_Portal.Models.BaseModels;

namespace SIA_Portal.Models.AdminModels
{
    public class CreateEditAdminAccountModel : BaseAccountLoggedInModel
    {

        public const int ACCOUNT_NOT_YET_CREATED = -1;

        public const string NO_EMPLOYEE_DIV_CATEGORY_NAME = "(None)";


        //
        public string AccountType { set; get; }

        public int AccountId { set; get; } = ACCOUNT_NOT_YET_CREATED;

        //

        [Required(ErrorMessage = "Username is required."), MaxLength(InputConstraintsConstants.USERNAME_CHARACTER_LIMIT)]
        [DataType(DataType.Text)]
        public string InputUsername { set; get; }

        
        public bool InputDisabledFromLogIn { set; get; }

        [MaxLength(InputConstraintsConstants.PASSWORD_CHARACTER_LIMIT)]
        [DataType(DataType.Password)]
        public string InputPassword { set; get; }

        [MaxLength(InputConstraintsConstants.PASSWORD_CHARACTER_LIMIT)]
        [DataType(DataType.Password)]
        public string InputConfirmPassword { set; get; }

        public bool InputNoChangeOnPassword { set; get; }

        public bool InputGenerateRandomPasswordInstead { set; get; }

        //

        public IList<string> DivisionNameList { set; get; }

        public IList<string> InputAccountDivisionNameList { set; get; }


        public IList<string> ToDivisionResponsibilityNameList { set; get; }

        public IList<string> InputAccountToDivisionResponibilityNameList { set; get; }



        //

        public int ActionExecuteStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

        public string StatusMessage { set; get; } = "";

        //

        public CreateEditAdminAccountModel() : base() { }

        public CreateEditAdminAccountModel(object account) : base((Account) account) { }


        //

        #region "View decisions related"

        public bool CanEditUsername()
        {
            return AccountId == ACCOUNT_NOT_YET_CREATED;
        }

        public string GetHeaderToDisplay()
        {
            string actionName;
            if (AccountId == ACCOUNT_NOT_YET_CREATED)
            {
                actionName = "Create";
            }
            else
            {
                actionName = "Edit";
            }

            var accountName = TypeConstants.GetAccountNameFromType(AccountType);


            return String.Format("{0} {1} account", actionName, accountName);
        }


        public string GetButtonTextDisplay()
        {
            if (IsActionCreateAccount())
            {
                return "Create Account";
            }
            else if (IsActionEditAccount()) {
                return "Edit Account";
            }
            else
            {
                return "Err: No Action";
            }
        }


        #endregion

        //

        #region "Controller decisions related"

        public bool IsActionCreateAccount()
        {
            return AccountId == ACCOUNT_NOT_YET_CREATED;
        }

        public bool IsActionEditAccount()
        {
            return AccountId != ACCOUNT_NOT_YET_CREATED;
        }

        public bool ArePasswordAndConfirmEqualAndNotEmptyAndNotNull_OrNoChangeInPassword_OrGeneratePassword()
        {
            return (!String.IsNullOrEmpty(InputPassword) && !String.IsNullOrEmpty(InputConfirmPassword) && InputPassword.Equals(InputConfirmPassword)) || InputNoChangeOnPassword || InputGenerateRandomPasswordInstead;
        }

        #endregion


    }
}