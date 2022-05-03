using CommonDatabaseActionReusables.CategoryManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIA_Portal.Accessors;

namespace SIA_PORTAL_UnitTest.DataSetter
{

    [TestClass]
    public class PortalAccountDivisionCategorySetter
    {

        private static PortalAccountDivisionCategoryAccessor portalAccountDivisionAccessor;

        private const string DIVISION_FACULTY = "Faculty";
        private const string DIVISION_HR = "Human Resource";
        private const string DIVISION_FRONT_DESK = "Front Desk";
        

        [ClassInitialize]
        public static void TestInit(TestContext context)
        {
            portalAccountDivisionAccessor = new PortalAccountDivisionCategoryAccessor();
        }

        [TestMethod]
        public void Clean_ResetOf_PositionCategory()
        {
            Clear_PositionCategory();
            Add_PositionCategory();
        }


        [TestMethod]
        public void Add_PositionCategory()
        {

            var cat01 = new Category.Builder();
            cat01.Name = DIVISION_FACULTY;
            portalAccountDivisionAccessor.CategoryDatabaseManagerHelper.CreateCategory(cat01);


            var cat02 = new Category.Builder();
            cat02.Name = DIVISION_HR;
            portalAccountDivisionAccessor.CategoryDatabaseManagerHelper.CreateCategory(cat02);


            var cat03 = new Category.Builder();
            cat03.Name = DIVISION_FRONT_DESK;
            portalAccountDivisionAccessor.CategoryDatabaseManagerHelper.CreateCategory(cat03);


        }

        [TestMethod]
        public void Clear_PositionCategory()
        {

            portalAccountDivisionAccessor.CategoryDatabaseManagerHelper.DeleteAllCategories();

        }


    }

}
