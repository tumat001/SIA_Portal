using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Constants;
using System.ComponentModel.DataAnnotations;
using CommonDatabaseActionReusables.AnnouncementManager;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using SIA_Portal.Models.BaseModels;


namespace SIA_Portal.Models.AdminModels
{
    public class CreateEditAnnouncementModel : BaseAccountLoggedInModel
    {

        public const int ANNOUNCEMENT_NOT_YET_CREATED = -1;

        //

        public int AnnouncementId { set; get; } = ANNOUNCEMENT_NOT_YET_CREATED;


        [Required(ErrorMessage = "Title is required.")]
        [DataType(DataType.Text)]
        public string InputTitle { set; get; }

        [DataType(DataType.MultilineText)]
        public string InputContent { set; get; }


        //public byte[] InputImage { set; get; }

        //

        public string AnnouncementImagePath { set; get; }

        public HttpPostedFileBase InputImage { set; get; }

        //
        public string InputChosenEmployeeCategoryName { set; get; }

        public IList<string> ListOfEmployeeCategoriesName { set; get; }

        //

        //
        public int ActionExecuteStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

        public string StatusMessage { set; get; } = "";


        //

        #region "View related decision"

        public string GetHeaderToDisplay()
        {
            string actionName;
            if (AnnouncementId == ANNOUNCEMENT_NOT_YET_CREATED)
            {
                actionName = "Create";
            }
            else
            {
                actionName = "Edit";
            }


            return string.Format("{0} Announcmeent", actionName);
        }

        public string GetButtonTextDisplay()
        {
            if (IsActionCreateAnnouncement())
            {
                return "Create Announcement";
            }
            else if (IsActionEditAnnouncement())
            {
                return "Edit Announcement";
            }
            else
            {
                return "Err: No Action";
            }
        }

        public string GetImageBytesAsStringForHtml()
        {
            //var imreBase64Data = Convert.ToBase64String(InputImage);
            //var imreBase64Data = StringUtilities.ConvertByteArrayToString(InputImage);
            //var imreBase64Data = InputImage;
            var imreBase64Data = AnnouncementImagePath;

            return string.Format("data:image/png;base64,{0}", imreBase64Data);
            //return imreBase64Data;
        }



        #endregion


        #region "Controller decisions related"

        public bool IsActionCreateAnnouncement()
        {
            return AnnouncementId == ANNOUNCEMENT_NOT_YET_CREATED;
        }

        public bool IsActionEditAnnouncement()
        {
            return AnnouncementId != ANNOUNCEMENT_NOT_YET_CREATED;
        }


        #endregion


    }
}