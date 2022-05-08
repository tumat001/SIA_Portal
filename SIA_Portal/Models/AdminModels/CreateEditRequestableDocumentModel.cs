using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.Constants;
using System.ComponentModel.DataAnnotations;
using SIA_Portal.CustomAccessors.RequestableDocument;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using SIA_Portal.Models.BaseModels;

namespace SIA_Portal.Models.AdminModels
{
    public class CreateEditRequestableDocumentModel : BaseAccountLoggedInModel
    {


        public const int REQUESTABLE_DOCU_NOT_YET_CREATED = -1;

        //

        public int RequestableDocumentId { set; get; } = REQUESTABLE_DOCU_NOT_YET_CREATED;


        [Required(ErrorMessage = "Name is required.")]
        [DataType(DataType.Text)]
        public string InputName { set; get; }

        [DataType(DataType.MultilineText)]
        public string InputContent { set; get; }


        //

        public int ActionExecuteStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

        public string StatusMessage { set; get; } = "";


        //

        #region "View related decision"

        public string GetHeaderToDisplay()
        {
            string actionName;
            if (RequestableDocumentId == REQUESTABLE_DOCU_NOT_YET_CREATED)
            {
                actionName = "Create";
            }
            else
            {
                actionName = "Edit";
            }


            return string.Format("{0} Requestable Document", actionName);
        }

        public string GetButtonTextDisplay()
        {
            if (IsActionCreateRequestableDocument())
            {
                return "Create Requestable Document";
            }
            else if (IsActionEditRequestableDocument())
            {
                return "Edit Requestable Document";
            }
            else
            {
                return "Err: No Action";
            }
        }


        #endregion


        #region "Controller decisions related"

        public bool IsActionCreateRequestableDocument()
        {
            return RequestableDocumentId == REQUESTABLE_DOCU_NOT_YET_CREATED;
        }

        public bool IsActionEditRequestableDocument()
        {
            return RequestableDocumentId != REQUESTABLE_DOCU_NOT_YET_CREATED;
        }


        #endregion



    }
}