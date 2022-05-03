using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{
    public class ConfirmDeleteEmployeeAccountModel : BaseWithTableIndexingLoggedInModel<Account, EmployeeAccountRepresentation>
    {

        public ConfirmDeleteEmployeeAccountModel() : base() { }

        public ConfirmDeleteEmployeeAccountModel(object account) : base((Account)account) { }


    }
}