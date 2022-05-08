using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.CustomAccessors.RequestableDocument.Exceptions
{
    public class RequestableDocumentDoesNotExistException : ApplicationException
    {

        /// <summary>
        /// The id that does not exist.
        /// </summary>
        public int NonExistingDocumentId { get; }

        public RequestableDocumentDoesNotExistException(int nonExistingId)
        {
            NonExistingDocumentId = nonExistingId;
        }

    }
}