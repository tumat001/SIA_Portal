using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AnnouncementManager.Configs;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using SIA_Portal.CustomAccessors.RequestableDocument.PathConfig;

namespace SIA_Portal.CustomAccessors.RequestableDocument.Actions
{
    public class RequestableDocumentExistsAction : AbstractAction<ReqDocuPathConfig>
    {

        public RequestableDocumentExistsAction(ReqDocuPathConfig config) : base(config)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>True if the requestable document id exists in the given parameters of <see cref="DatabasePathConfig"/></returns>
        public bool IfRequestDocumentIdExsists(int id)
        {

            bool result = false;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT [{0}] FROM [{1}] WHERE [{0}] = @TargetId",
                        databasePathConfig.IdColumnName, databasePathConfig.ReqDocuTableName);
                    command.Parameters.Add(new SqlParameter("TargetId", id));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        result = reader.HasRows;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the requestable document id exists in the given parameters of <see cref="DatabasePathConfig"/>, false otherwise</returns>
        public bool TryIfRequestDocumentIdExsists(int id)
        {
            try
            {
                return IfRequestDocumentIdExsists(id);
            }
            catch (Exception)
            {
                return false;
            }

        }


    }
}