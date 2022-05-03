using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using System.Web.Mvc;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{

    public class ManageAdminAccountsModel : BaseWithTableIndexingLoggedInModel<Account, AdminAccountRepresentation>
    {

        public string AccountUsernameFilter { set; get; }

        
        public ManageAdminAccountsModel() : base() { }

        public ManageAdminAccountsModel(object account) : base((Account)account) { }

 
    }

}