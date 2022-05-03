using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using SIA_Portal.Constants;

namespace SIA_Portal.Models.RegisNotRequired
{
    public class RegisNotRequired_RegisterModel
    {

        [Required]
        [DataType(DataType.Text)]
        public string InputFirstName { set; get; }

        [Required]
        [DataType(DataType.Text)]
        public string InputLastName { set; get; }


        public string SelectedDivision { set; get; }

        [Required]
        [DataType(DataType.Text)]
        public string InputUsername { set; get; }

        [Required]
        [DataType(DataType.Password)]
        public string InputPassword { set; get; }

        [Required]
        [DataType(DataType.Password)]
        public string InputConfirmPassword { set; get; }


        //

        public string StatusMessage { set; get; }

        public int StatusResult { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

    }
}