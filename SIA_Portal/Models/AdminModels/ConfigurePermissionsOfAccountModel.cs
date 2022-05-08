using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Constants;
using System.ComponentModel.DataAnnotations;
using SIA_Portal.Models.BaseModels;
using CommonDatabaseActionReusables.PermissionsManager;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{
    public class ConfigurePermissionsOfAccountModel : BaseAccountLoggedInModel
    {

        public IList<PermissionRepresentation> AllPermissionRepresentation { set; get; }

        public int AccPermissionToEditId { set; get; }

        public string AccPermissionToEditUsername { set; get; }



        //

        public int ActionExecuteStatus { set; get; } = ActionStatusConstants.STATUS_NO_ACTION;

        public string StatusMessage { set; get; } = "";

        //

        public ConfigurePermissionsOfAccountModel() : base() { }

        public ConfigurePermissionsOfAccountModel(object account) : base((Account)account) { }



    }
}