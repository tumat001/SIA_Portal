using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Constants;
using System.ComponentModel.DataAnnotations;
using CommonDatabaseActionReusables.PermissionsManager;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using SIA_Portal.Models.BaseModels;

namespace SIA_Portal.Models.AdminModels
{
    public class CreateEditPermissionModel : BaseAccountLoggedInModel
    {


        public const int PERMISSION_NOT_YET_CREATED = -1;

        //

        public int PermissionId { set; get; } = PERMISSION_NOT_YET_CREATED;


        [Required(ErrorMessage = "Name is required.")]
        [DataType(DataType.Text)]
        public string InputName { set; get; }

        [DataType(DataType.MultilineText)]
        public string InputDescription { set; get; }



        //
        public int ActionExecuteStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

        public string StatusMessage { set; get; } = "";


        //

        #region "View related decision"

        public string GetHeaderToDisplay()
        {
            string actionName;
            if (PermissionId == PERMISSION_NOT_YET_CREATED)
            {
                actionName = "Create";
            }
            else
            {
                actionName = "Edit";
            }


            return string.Format("{0} Permission", actionName);
        }

        public string GetButtonTextDisplay()
        {
            if (IsActionCreatePermission())
            {
                return "Create Permission";
            }
            else if (IsActionEditPermission())
            {
                return "Edit Permission";
            }
            else
            {
                return "Err: No Action";
            }
        }



        #endregion


        #region "Controller decisions related"

        public bool IsActionCreatePermission()
        {
            return PermissionId == PERMISSION_NOT_YET_CREATED;
        }

        public bool IsActionEditPermission()
        {
            return PermissionId != PERMISSION_NOT_YET_CREATED;
        }


        #endregion


    }
}