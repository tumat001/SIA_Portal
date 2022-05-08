using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AnnouncementManager.Configs;
using CommonDatabaseActionReusables.AnnouncementManager;
using System.Data.SqlClient;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using CommonDatabaseActionReusables.GeneralUtilities.InputConstraints;
using SIA_Portal.CustomAccessors.RequestableDocument.PathConfig;
using SIA_Portal.CustomAccessors.RequestableDocument.Exceptions;

namespace SIA_Portal.CustomAccessors.RequestableDocument.Actions
{
    public class EditRequestableDocumentAction : AbstractAction<ReqDocuPathConfig>
    {

        internal EditRequestableDocumentAction(ReqDocuPathConfig config) : base(config)
        {

        }


        /// <summary>
        /// Edits the requestable document with the provided <paramref name="id"/> using the properties found in <paramref name="builder"/>.<br/>
        /// Setting <paramref name="builder"/> to null makes no edits to the document.<br/><br/>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="builder"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="AnnouncementDoesNotExistException"></exception>
        /// <exception cref="InputStringConstraintsViolatedException"></exception>
        /// <returns>True if the requestable document was edited successfully, even if <paramref name="builder"/> is set to null.</returns>
        public bool EditRequestableDocument(int id, RequestableDocument.Builder builder)
        {
            bool idExists = new RequestableDocumentExistsAction(databasePathConfig).TryIfRequestDocumentIdExsists(id);
            if (!idExists)
            {
                throw new RequestableDocumentDoesNotExistException(id);
            }

            if (builder == null)
            {
                return true;
            }


            var inputConstraintsChecker = new AntiSQLInjectionInputConstraint();
            inputConstraintsChecker.SatisfiesConstraint(builder.DocumentName);

            //


            var success = false;

            using (SqlConnection sqlConn = databasePathConfig.GetSQLConnection())
            {
                sqlConn.Open();

                using (SqlCommand command = sqlConn.CreateCommand())
                {
                    command.CommandText = String.Format("UPDATE [{0}] SET [{1}] = @NewNameVal, [{2}] = @NewNoteDescVal WHERE [{3}] = @IdVal",
                        databasePathConfig.ReqDocuTableName, 
                        databasePathConfig.NameColumnName, databasePathConfig.NoteDescriptionColumnName,
                        databasePathConfig.IdColumnName);
                    command.Parameters.Add(new SqlParameter("NewNameVal", builder.DocumentName));
                    command.Parameters.Add(new SqlParameter("NewNoteDescVal", GetParamOrDbNullIfParamIsNull(builder.NoteDescription)));
                    command.Parameters.Add(new SqlParameter("IdVal", id));

                    success = command.ExecuteNonQuery() > 0;
                }
            }

            return success;

        }


        /// <summary>
        /// Edits the requestable document with the provided <paramref name="id"/> using the properties found in <paramref name="builder"/>.<br/>
        /// Setting <paramref name="builder"/> to null makes no edits to the document.<br/><br/>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="builder"></param>
        /// <returns>True if the requestable document was edited successfully, even if <paramref name="builder"/> is set to null. False otherwise.</returns>
        public bool TryEditRequestableDocument(int id, RequestableDocument.Builder builder)
        {
            try
            {
                return EditRequestableDocument(id, builder);
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}