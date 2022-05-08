using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SIA_Portal.CustomAccessors.RequestableDocument.PathConfig;
using SIA_Portal.CustomAccessors.RequestableDocument.Actions;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;

namespace SIA_Portal.CustomAccessors.RequestableDocument
{
    public class RequestDocumentManagerHelper
    {

        public ReqDocuPathConfig PathConfig { get; }

        public RequestDocumentManagerHelper(ReqDocuPathConfig pathConfig)
        {
            this.PathConfig = pathConfig;
        }


        #region "If docu exists"

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
            return new RequestableDocumentExistsAction(PathConfig).IfRequestDocumentIdExsists(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the requestable document id exists in the given parameters of <see cref="DatabasePathConfig"/>, false otherwise</returns>
        public bool TryIfRequestDocumentIdExsists(int id)
        {
            return new RequestableDocumentExistsAction(PathConfig).TryIfRequestDocumentIdExsists(id);
        }

        #endregion

        //

        #region "Create Docu"

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
            return new CreateRequestableDocumentAction(PathConfig).CreateRequestableDocument(builder);
        }

        /// <summary>
        /// Creates a requestable document with the given parameters in the given <paramref name="builder"/>.<br></br>
        /// The document is created in the database and table specified in this object's <see cref="DatabasePathConfig"/><br/><br/>
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>The Id corresponding to the created requestable document, or -1 if an exception has occurred.</returns>
        public int TryCreateRequestableDocument(RequestableDocument.Builder builder)
        {
            return new CreateRequestableDocumentAction(PathConfig).TryCreateRequestableDocument(builder);
        }

        #endregion

        //

        #region "Delete all docu"

        /// <summary>
        /// Deletes all requestable documents.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <returns>True if the delete operation was successful, even if no announcment was deleted.</returns>
        public bool DeleteAllRequestableDocuments()
        {
            return new DeleteAllRequestableDocumentAction(PathConfig).DeleteAllRequestableDocuments();
        }

        /// <summary>
        /// Deletes all announcements.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <returns>True if the delete operation was successful, even if no announcment was deleted.</returns>
        public bool TryDeleteAllRequestableDocuments()
        {
            return new DeleteAllRequestableDocumentAction(PathConfig).TryDeleteAllRequestableDocuments();
        }

        #endregion

        //

        #region "Get Requestable document"

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
            return new GetRequestableDocumentAction(PathConfig).GetRequestableDocumentFromId(id);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An <see cref="RequestableDocument"/> object containing information about the document with the provided <paramref name="id"/>.</returns>
        public RequestableDocument TryGetRequestableDocumentFromId(int id)
        {
            return new GetRequestableDocumentAction(PathConfig).TryGetRequestableDocumentFromId(id);
        }

        #endregion

        //

        #region "Edit Docu"

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
            return new EditRequestableDocumentAction(PathConfig).EditRequestableDocument(id, builder);
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
            return new EditRequestableDocumentAction(PathConfig).TryEditRequestableDocument(id, builder);
        }

        #endregion

        //

        #region "Delete docu"

        /// <summary>
        /// Deletes the requestable document with the given <paramref name="id"/>.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="SqlException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="RequestableDocumentDoesNotExistException"></exception>
        /// <returns>True if the delete operation was successful. False otherwise.</returns>
        public bool DeleteRequestableDocumentWithId(int id)
        {
            return new DeleteRequestableDocumentAction(PathConfig).DeleteRequestableDocumentWithId(id);
        }

        /// <summary>
        /// Deletes the requestable document with the given <paramref name="id"/>.<br/>
        /// This object's <see cref="DatabasePathConfig"/> determines which database and table is affected.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if the delete operation was successful. False otherwise.</returns>
        public bool TryDeleteRequestableDocumentWithId(int id)
        {
            return new DeleteRequestableDocumentAction(PathConfig).TryDeleteRequestableDocumentWithId(id);
        }

        #endregion

        //

        #region "Advanced Get Docu"

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
            return new AdvancedGetRequestableDocumentsAction(PathConfig).AdvancedGetRequestableDocumentsAsList(adGetParameter);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="adGetParameter"></param>
        /// <returns>A list of requestable documents found in the database given in this object's <see cref="DatabasePathConfig"/>, taking into
        /// account the given <paramref name="adGetParameter"/>.</returns>
        public IReadOnlyList<RequestableDocument> TryAdvancedGetRequestableDocumentsAsList(AdvancedGetParameters adGetParameter)
        {
            return new AdvancedGetRequestableDocumentsAction(PathConfig).TryAdvancedGetRequestableDocumentsAsList(adGetParameter);
        }


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
            return new AdvancedGetRequestableDocumentsAction(PathConfig).AdvancedGetRequestableDocumentCount(adGetParameter);
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
            return new AdvancedGetRequestableDocumentsAction(PathConfig).TryAdvancedGetRequestableDocumentCount(adGetParameter);
        }

        #endregion

    }
}