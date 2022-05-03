using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.CustomAccessors.EmployeeRecordsManager.Exceptions
{
    public class EmployeeRecordDoesNotExistException : ApplicationException
    {

        /// <summary>
        /// The id that does not exist.
        /// </summary>
        public int NonExistingId { get; }

        internal EmployeeRecordDoesNotExistException(int nonExistingId)
        {
            NonExistingId = nonExistingId;
        }

    }
}