using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager.Actions;
using CommonDatabaseActionReusables.AccountManager.Actions.Configs;
using CommonDatabaseActionReusables.AccountManager;

namespace SIA_Portal.Accessors
{

    public class PortalAccountAccessor
    {

        //Change these values upon changing database
        const string DATABASE_CONN_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mat\source\repos\SIA_Portal\SIA_Portal\App_Data\PortalDatabase.mdf;Integrated Security=True";
        const string TABLE_NAME = "AccountTable";

        const string ACC_ID_COL_NAME = "Id";
        const string ACC_USERNAME_COL_NAME = "Username";
        const string ACC_PASSWORD_COL_NAME = "Password";
        const string ACC_DISABLED_FROM_LOGIN_COL_NAME = "DisabledFromLogIn";
        const string ACC_EMAIL_COL_NAME = "Email";
        const string ACC_TYPE_COL_NAME = "AccountType";


        public AccountRelatedDatabasePathConfig AccountDatabasePathConfig { get; }

        public AccountDatabaseManagerHelper AccountDatabaseManagerHelper { get; }


        public PortalAccountAccessor()
        {
            AccountDatabasePathConfig = new AccountRelatedDatabasePathConfig(DATABASE_CONN_STRING, ACC_ID_COL_NAME, ACC_USERNAME_COL_NAME,
                ACC_PASSWORD_COL_NAME, ACC_DISABLED_FROM_LOGIN_COL_NAME, ACC_EMAIL_COL_NAME, ACC_TYPE_COL_NAME, TABLE_NAME);

            AccountDatabaseManagerHelper = new AccountDatabaseManagerHelper(AccountDatabasePathConfig);
        }

    }
}