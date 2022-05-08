using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AnnouncementManager.Configs;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.GeneralUtilities.TypeUtilities;
using SIA_Portal.CustomAccessors.RequestableDocument.PathConfig;
using SIA_Portal.CustomAccessors.RequestableDocument;


namespace SIA_Portal.CustomAccessors.RequestableDocument.Actions
{
    public class AdvancedGetRequestableDocumentsAction : AbstractAction<ReqDocuPathConfig>
    {

        internal AdvancedGetRequestableDocumentsAction(ReqDocuPathConfig config) : base(config)
        {

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>A list of requestable documents found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>.</returns>
        public IReadOnlyList<RequestableDocument> AdvancedGetRequestableDocumentsAsList(AdvancedGetParameters adGetParameter)
        {

            var list = new List<RequestableDocument>();
            var builder = new RequestableDocument.Builder();

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT [{0}], [{1}], [{2}] FROM [{3}] {6} {7} {4} {5}",
                        databasePathConfig.IdColumnName, databasePathConfig.NameColumnName, databasePathConfig.NoteDescriptionColumnName,
                        databasePathConfig.ReqDocuTableName,
                        adGetParameter.GetSQLStatementFromOffset(),
                        adGetParameter.GetSQLStatementFromFetch(),
                        adGetParameter.GetSQLStatementFromTextToContain(databasePathConfig.NameColumnName),
                        adGetParameter.GetSQLStatementFromOrderBy(databasePathConfig.IdColumnName));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            builder.DocumentName = reader.GetSqlString(1).ToString();
                            builder.NoteDescription = StringUtilities.ConvertSqlStringToByteArray(reader.GetSqlString(2));

                            var id = reader.GetSqlInt32(0).Value;

                            list.Add(builder.Build(id));
                        }
                    }
                }
            }

            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <returns>A list of requestable documents found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>.</returns>
        public IReadOnlyList<RequestableDocument> TryAdvancedGetRequestableDocumentsAsList(AdvancedGetParameters adGetParameter)
        {
            try
            {
                return AdvancedGetRequestableDocumentsAsList(adGetParameter);
            }
            catch (Exception)
            {
                return new List<RequestableDocument>();
            }
        }


        //


        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <returns>The number of requestable documents found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>. Ignores <see cref="AdvancedGetParameters.OrderByParameters"/>.</returns>
        public int AdvancedGetRequestableDocumentCount(AdvancedGetParameters adGetParameter)
        {
            var count = -1;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT COUNT(*) FROM (SELECT [{0}] FROM [{1}] {4} {5} {2} {3}) T",
                        databasePathConfig.IdColumnName,
                        databasePathConfig.ReqDocuTableName,
                        adGetParameter.GetSQLStatementFromOffset(),
                        adGetParameter.GetSQLStatementFromFetch(),
                        adGetParameter.GetSQLStatementFromTextToContain(databasePathConfig.NameColumnName),
                        adGetParameter.GetSQLStatementFromOrderBy(databasePathConfig.IdColumnName));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            count = reader.GetSqlInt32(0).Value;
                        }
                    }
                }
            }

            return count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <returns>The number of requestable documents found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>. Ignores <see cref="AdvancedGetParameters.OrderByParameters"/>.<br/><br/>
        /// Returns -1 if an error has occured.</returns>
        public int TryAdvancedGetRequestableDocumentCount(AdvancedGetParameters adGetParameter)
        {
            try
            {
                return AdvancedGetRequestableDocumentCount(adGetParameter);
            }
            catch (Exception)
            {
                return -1;
            }
        }

    }
}