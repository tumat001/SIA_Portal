using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{
    public class ConfirmDeleteAdminAccountModel : BaseWithTableIndexingLoggedInModel<Account, AdminAccountRepresentation>
    {

        public ConfirmDeleteAdminAccountModel() : base() { }

        public ConfirmDeleteAdminAccountModel(object account) : base((Account)account) { }


    }
}