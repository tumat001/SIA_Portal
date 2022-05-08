using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.Constants;
using System.ComponentModel.DataAnnotations;
using CommonDatabaseActionReusables.CategoryManager;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using SIA_Portal.Models.BaseModels;

namespace SIA_Portal.Models.AdminModels
{
    public class CreateEditAccountDivisionModel : BaseAccountLoggedInModel
    {


        public const int ACC_DIV_NOT_YET_CREATED = -1;

        //

        public int AccountDivisionId { set; get; } = ACC_DIV_NOT_YET_CREATED;


        [Required(ErrorMessage = "Name is required.")]
        [DataType(DataType.Text)]
        public string InputName { set; get; }

        //

        public int ActionExecuteStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

        public string StatusMessage { set; get; } = "";


        //

        #region "View related decision"

        public string GetHeaderToDisplay()
        {
            string actionName;
            if (AccountDivisionId == ACC_DIV_NOT_YET_CREATED)
            {
                actionName = "Create";
            }
            else
            {
                actionName = "Edit";
            }


            return string.Format("{0} Account Division", actionName);
        }

        public string GetButtonTextDisplay()
        {
            if (IsActionCreateAccountDivision())
            {
                return "Create Account Division";
            }
            else if (IsActionEditAccountDivision())
            {
                return "Edit Account Division";
            }
            else
            {
                return "Err: No Action";
            }
        }


        #endregion


        #region "Controller decisions related"

        public bool IsActionCreateAccountDivision()
        {
            return AccountDivisionId == ACC_DIV_NOT_YET_CREATED;
        }

        public bool IsActionEditAccountDivision()
        {
            return AccountDivisionId != ACC_DIV_NOT_YET_CREATED;
        }


        #endregion


    }
}