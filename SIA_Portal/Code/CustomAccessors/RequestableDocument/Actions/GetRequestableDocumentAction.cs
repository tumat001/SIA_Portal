using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AnnouncementManager.Configs;
using CommonDatabaseActionReusables.AnnouncementManager.Exceptions;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.GeneralUtilities.InputConstraints;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using SIA_Portal.CustomAccessors.RequestableDocument.PathConfig;
using SIA_Portal.CustomAccessors.RequestableDocument.Exceptions;

namespace SIA_Portal.CustomAccessors.RequestableDocument.Actions
{
    public class GetRequestableDocumentAction : AbstractAction<ReqDocuPathConfig>
    {

        internal GetRequestableDocumentAction(ReqDocuPathConfig config) : base(config)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>An <see cref="RequestableDocument"/> object containing information about the document with the provided <paramref name="id"/>.</returns>
        public RequestableDocument GetRequestableDocumentFromId(int id)
        {

            //

            bool idExists = new RequestableDocumentExistsAction(databasePathConfig).TryIfRequestDocumentIdExsists(id);
            if (!idExists)
            {
                throw new RequestableDocumentDoesNotExistException(id);
            }

            //

            var builder = new RequestableDocument.Builder();
            RequestableDocument reqDocu = null;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT [{0}], [{1}] FROM [{2}] WHERE [{3}] = @IdVal",
                        databasePathConfig.NameColumnName, databasePathConfig.NoteDescriptionColumnName,
                        databasePathConfig.ReqDocuTableName,
                        databasePathConfig.IdColumnName);
                    command.Parameters.Add(new SqlParameter("IdVal", id));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            builder.DocumentName = reader.GetSqlString(0).ToString();
                            builder.NoteDescription = StringUtilities.ConvertSqlStringToByteArray(reader.GetSqlString(1));

                            reqDocu = builder.Build(id);
                        }
                    }
                }
            }


            return reqDocu;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An <see cref="RequestableDocument"/> object containing information about the document with the provided <paramref name="id"/>.</returns>
        public RequestableDocument TryGetRequestableDocumentFromId(int id)
        {
            try
            {
                return GetRequestableDocumentFromId(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}