using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using SIA_Portal.Accessors;
using CommonDatabaseActionReusables.PermissionsManager;
using CommonDatabaseActionReusables.RelationManager;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;

namespace SIA_Portal.Models.BaseModels
{
    public class BaseAccountLoggedInModel
    {

        public Account LoggedInAccount { set; get; }

        public ISet<int> PermissionIds { set; get; }

        public IReadOnlyList<Permission> Permissions { set; get; }


        public BaseAccountLoggedInModel(Account loggedInAccount)
        {
            LoggedInAccount = loggedInAccount;
            var permList = new List<Permission>();

            if (LoggedInAccount != null) {

                PermissionIds = new PortalAccountToPermissionsAccessor().EntityToCategoryDatabaseManagerHelper.AdvancedGetRelationsOfPrimaryAsSet(loggedInAccount.Id, new AdvancedGetParameters());
                
                var permAccessor = new PortalPermissionAccessor();

                foreach (int id in PermissionIds)
                {
                    var perm = permAccessor.PermissionDatabaseManagerHelper.TryGetPermissionInfoFromId(id);

                    if (perm != null)
                    {
                        permList.Add(perm);
                    }
                } 

            }

            Permissions = permList;
        }

        public BaseAccountLoggedInModel(object loggedInAccount) : this((Account) loggedInAccount) { }

        public BaseAccountLoggedInModel() { }


        //

        public bool IfAccountHasPermission(string permName)
        {
            foreach (Permission perm in Permissions)
            {
                if (perm.Name.Equals(permName))
                {
                    return true;
                }
            }

            return false;
        }

    }
}