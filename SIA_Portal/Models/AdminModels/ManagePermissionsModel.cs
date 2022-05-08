using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.PermissionsManager;
using CommonDatabaseActionReusables.AccountManager;
using System.Web.Mvc;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{
    public class ManagePermissionsModel : BaseWithTableIndexingLoggedInModel<Permission, PermissionRepresentation>
    {

        public string TitleFilter { set; get; }


        public ManagePermissionsModel() : base() { }

        public ManagePermissionsModel(object account) : base((Account)account) { }


    }
}