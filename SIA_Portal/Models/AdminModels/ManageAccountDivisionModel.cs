using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using System.Web.Mvc;
using SIA_Portal.Models.BaseModels;
using SIA_Portal.Models.ObjectRepresentations;
using CommonDatabaseActionReusables.CategoryManager;

namespace SIA_Portal.Models.AdminModels
{
    public class ManageAccountDivisionModel : BaseWithTableIndexingLoggedInModel<Category, CategoryRepresentation>
    {

        public string NameFilter { set; get; }


        public ManageAccountDivisionModel() : base() { }

        public ManageAccountDivisionModel(object account) : base((Account)account) { }



    }
}