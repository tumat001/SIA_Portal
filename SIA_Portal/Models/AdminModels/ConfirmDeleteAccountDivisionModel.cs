using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.CategoryManager;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;

namespace SIA_Portal.Models.AdminModels
{
    public class ConfirmDeleteAccountDivisionModel : BaseWithTableIndexingLoggedInModel<Category, CategoryRepresentation>
    {

        public ConfirmDeleteAccountDivisionModel() : base() { }

        public ConfirmDeleteAccountDivisionModel(object account) : base((Account)account) { }


    }
}