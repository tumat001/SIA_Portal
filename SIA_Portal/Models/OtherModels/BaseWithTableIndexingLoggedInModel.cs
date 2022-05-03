using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonDatabaseActionReusables.AccountManager;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;

namespace SIA_Portal.Models.BaseModels
{

    /// <summary>
    /// A model that manages table indexing and manages selection of items.
    /// </summary>
    /// <typeparam name="T">The datatype that is to be displayed and used for basis.</typeparam>
    /// <typeparam name="U">A representation of the datatype <typeparamref name="T"/> that has to be mutable, and have a public paramerterless constructor. May be the same type as <typeparamref name="T"/> if it fills the conditions.</typeparam>
    public class BaseWithTableIndexingLoggedInModel<T, U> : BaseAccountLoggedInModel
    {

        public const int ENTITIES_PER_PAGE = 2;
        public const int PAGE_INDEX_COUNT_IN_NAVIGATION = 9;
        public const int CURRENT_INDEX_MAX_OFFSET_FROM_LEFT = 4;
        public const int CURRENT_INDEX_MAX_OFFSET_FROM_RIGHT = PAGE_INDEX_COUNT_IN_NAVIGATION - CURRENT_INDEX_MAX_OFFSET_FROM_LEFT - 1;

        public const string DISABLED_FLI_CHECKBOX_DOM_TYPE_NAME = "Disabled"; //unused
        public const string SELECTED_CHECKBOX_DOM_TYPE_NAME = "Selected"; //unused


        public int CurrentPageIndex { set; get; } = 1;


        public IReadOnlyList<T> EntitiesInPage { set; get; }


        public IList<U> EntityRepresentationsInPage { set; get; }

        public IList<int> SelectedEntityIds { set; get; }

        //

        public AdvancedGetParameters AdGetParam { set; get; }

        //

        public int TotalEntityCount { set; get; }


        public BaseWithTableIndexingLoggedInModel() : base() { }

        public BaseWithTableIndexingLoggedInModel(object account) : base((Account)account) { }

        //


        #region "Page index display nav"

        public IList<int> GetPageIndicesToDisplayBasedOnCurrentPageIndex()
        {
            var totalPages = GetTotalPagesBasedOnStudentCount();


            int lowerLimit;
            int upperLimit;


            lowerLimit = (CurrentPageIndex > CURRENT_INDEX_MAX_OFFSET_FROM_LEFT) ? CurrentPageIndex - CURRENT_INDEX_MAX_OFFSET_FROM_LEFT : CurrentPageIndex;
            if (lowerLimit + PAGE_INDEX_COUNT_IN_NAVIGATION > totalPages)
            {
                lowerLimit = totalPages - PAGE_INDEX_COUNT_IN_NAVIGATION;

                if (lowerLimit < 0)
                {
                    lowerLimit = 1;
                }
            }

            upperLimit = lowerLimit + PAGE_INDEX_COUNT_IN_NAVIGATION;
            if (upperLimit > totalPages)
            {
                upperLimit = totalPages;
            }


            var bucket = new List<int>();


            Console.WriteLine(String.Format("Lowerlimit: {0}, Upperlimit: {1}", lowerLimit, upperLimit));
            foreach (int i in Enumerable.Range(lowerLimit, (upperLimit - lowerLimit) + 1))
            {
                bucket.Add(i);
            }

            return bucket;
        }


        #endregion

        public int GetTotalPagesBasedOnStudentCount()
        {
            var pages = (int)Math.Ceiling((decimal)TotalEntityCount / (decimal)ENTITIES_PER_PAGE);
            if (pages <= 0)
            {
                pages = 1;
            }

            return pages;
        }


        public bool IsPageIndexValid(int pageIndex)
        {
            return GetTotalPagesBasedOnStudentCount() > pageIndex;
        }

        public static int GetOffsetToUseBasedOnPageIndex(int pageIndex)
        {
            return (pageIndex - 1) * ENTITIES_PER_PAGE;
        }

        //

        /*
        #region "DomIdentifier related"
        //NOTE: UNUSED

        public static string GenerateDOMIndentifierForSelectedForAccountId(string id)
        {
            return DomIdentifier.GenerateDomIdentifierAsString(ManageStudentAccountsModel.SELECTED_CHECKBOX_DOM_TYPE_NAME, id);
        }

        public static string[] GetDOMTypeAndNameFromSelectedFromAccount(string domIdentifier)
        {
            return DomIdentifier.GetElementsOfDomIdentifier(domIdentifier);
        }



        public static string GenerateDOMIndentifierForDisabledFromLogInForAccount(Account account)
        {
            return DomIdentifier.GenerateDomIdentifierAsString(ManageStudentAccountsModel.DISABLED_FLI_CHECKBOX_DOM_TYPE_NAME, account.Id.ToString());
        }

        public static string[] GetDOMTypeAndNameFromDisabledFromLogInFromAccount(string domIdentifier)
        {
            return DomIdentifier.GetElementsOfDomIdentifier(domIdentifier);
        }

        #endregion
        */

    }
}