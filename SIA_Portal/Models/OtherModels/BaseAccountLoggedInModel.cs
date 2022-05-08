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

        public Account _loggedInAccount;
        public Account LoggedInAccount 
        { 
            set
            {
                _loggedInAccount = value;

                var permList = new List<Permission>();

                if (LoggedInAccount != null)
                {

                    AccountPermissionIds = new PortalAccountToPermissionsAccessor().EntityToCategoryDatabaseManagerHelper.TryAdvancedGetRelationsOfPrimaryAsSet(_loggedInAccount.Id, new AdvancedGetParameters());

                    var permAccessor = new PortalPermissionAccessor();

                    foreach (int id in AccountPermissionIds)
                    {
                        var perm = permAccessor.PermissionDatabaseManagerHelper.TryGetPermissionInfoFromId(id);

                        if (perm != null)
                        {
                            permList.Add(perm);
                        }
                    }

                }

                AccountPermissions = permList;
            }
            get
            {
                return _loggedInAccount;
            } 
        }


        public ISet<int> AccountPermissionIds { set; get; }

        public IReadOnlyList<Permission> AccountPermissions { set; get; }


        //

        public BaseAccountLoggedInModel(Account loggedInAccount)
        {
            LoggedInAccount = loggedInAccount;
        }

        public BaseAccountLoggedInModel(object loggedInAccount) : this((Account) loggedInAccount) { }

        public BaseAccountLoggedInModel() { }


        //

        public bool IfAccountHasPermission(string permName)
        {
            foreach (Permission perm in AccountPermissions)
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