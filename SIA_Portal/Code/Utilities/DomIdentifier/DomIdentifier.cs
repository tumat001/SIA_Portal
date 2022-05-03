using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIA_Portal.Utilities.DomIdentifier
{
    public class DomIdentifier
    {

        public const char DOM_NAME_TO_ID_SEPARATOR = '_';


        public static string GenerateDomIdentifierAsString(string typeName, string id)
        {
            return string.Format("{0}{2}{1}", typeName, id, DOM_NAME_TO_ID_SEPARATOR);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="domIdentifier"></param>
        /// <returns>A string array that contains the type name and id respectively.</returns>
        public static string[] GetElementsOfDomIdentifier(string domIdentifier)
        {
            return domIdentifier.Split(DOM_NAME_TO_ID_SEPARATOR);
        }

        public static bool IsDomIdentifier(string candidate)
        {
            if (candidate != null)
            {
                return candidate.Contains(DOM_NAME_TO_ID_SEPARATOR);
            }
            else
            {
                return false;
            }
        }



    }
}