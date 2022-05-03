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
    public class ManageEmployeeAccountsModel : BaseWithTableIndexingLoggedInModel<Account, EmployeeAccountRepresentation>
    {
        public string AccountUsernameFilter { set; get; }


        public ManageEmployeeAccountsModel() : base() { }

        public ManageEmployeeAccountsModel(object account) : base((Account)account) { }

    }
}