using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.PathConfig;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.GeneralUtilities.InputConstraints;
using SIA_Portal.CustomAccessors.RequestableDocument.PathConfig;
using SIA_Portal.CustomAccessors.RequestableDocument;

namespace SIA_Portal.CustomAccessors.RequestableDocument.Actions
{
    public class CreateRequestableDocumentAction : AbstractAction<ReqDocuPathConfig>
    {
        internal CreateRequestableDocumentAction(ReqDocuPathConfig config) : base(config)
        {

        }


        /// <summary>
        /// Creates a requestable document with the given parameters in the given <paramref name="builder"/>.<br></br>
        /// The document is created in the database and table specified in this object's <see cref="DatabasePathConfig"/><br/><br/>
        /// </summary>
        /// <param name="builder"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="InputStringConstraintsViolatedException"></exception>
        /// <returns>The Id corresponding to the created requestable document.</returns>
        public int CreateRequestableDocument(RequestableDocument.Builder builder)
        {
            var inputConstraintsChecker = new AntiSQLInjectionInputConstraint();
            inputConstraintsChecker.SatisfiesConstraint(builder.DocumentName);

            //

            int docuId = -1;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("INSERT INTO [{0}] ({1}, {2}) VALUES (@DocuName, @NoteDescription); SELECT SCOPE_IDENTITY()",
                        databasePathConfig.ReqDocuTableName,
                        databasePathConfig.NameColumnName, databasePathConfig.NoteDescriptionColumnName);
                    
                    command.Parameters.Add(new SqlParameter("DocuName", builder.DocumentName));

                    command.Parameters.Add(new SqlParameter("NoteDescription", GetParamOrDbNullIfParamIsNull(builder.NoteDescription)));
                    

                    docuId = int.Parse(command.ExecuteScalar().ToString());
                }
            }

            return docuId;
        }


        /// <summary>
        /// Creates a requestable document with the given parameters in the given <paramref name="builder"/>.<br></br>
        /// The document is created in the database and table specified in this object's <see cref="DatabasePathConfig"/><br/><br/>
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>The Id corresponding to the created requestable document, or -1 if an exception has occurred.</returns>
        public int TryCreateRequestableDocument(RequestableDocument.Builder builder)
        {
            try
            {
                return CreateRequestableDocument(builder);
            }
            catch (Exception)
            {
                return -1;
            }
        }


    }
}