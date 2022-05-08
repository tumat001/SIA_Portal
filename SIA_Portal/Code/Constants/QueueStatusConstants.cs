using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Constants
{
    public class QueueStatusConstants
    {

        public const int STATUS_PENDING = 0;
        public const int STATUS_ACKNOWLEDGED = 1;
        public const int STATUS_COMPLETED = 2;
        public const int STATUS_TERMINATED = 3;


        public const string STATUS_PENDING_TEXT = "Pending";
        public const string STATUS_ACKNOWLEDGED_TEXT = "Acknowledged";
        public const string STATUS_COMPLETED_TEXT = "Completed";
        public const string STATUS_TERMINATED_TEXT = "Terminated";


        public static string GetStatusAsText(int status)
        {
            if (status == STATUS_PENDING)
            {
                return STATUS_PENDING_TEXT;
            }
            else if (status == STATUS_ACKNOWLEDGED)
            {
                return STATUS_ACKNOWLEDGED_TEXT;
            }
            else if (status == STATUS_COMPLETED)
            {
                return STATUS_COMPLETED_TEXT;
            }
            else if (status == STATUS_TERMINATED)
            {
                return STATUS_TERMINATED_TEXT;
            }

            return "";
        }

        public static int GetStatusAsInt(string status)
        {
            if (status.Equals(STATUS_PENDING_TEXT))
            {
                return STATUS_PENDING;
            }
            else if (status.Equals(STATUS_ACKNOWLEDGED_TEXT))
            {
                return STATUS_ACKNOWLEDGED;
            }
            else if (status.Equals(STATUS_COMPLETED_TEXT))
            {
                return STATUS_COMPLETED;
            }
            else if (status.Equals(STATUS_TERMINATED_TEXT))
            {
                return STATUS_TERMINATED;
            }

            return -1;
        }

    }
}