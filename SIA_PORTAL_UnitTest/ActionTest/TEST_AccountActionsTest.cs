using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CommonDatabaseActionReusables.AccountManager;
using CommonDatabaseActionReusables.GeneralUtilities.DatabaseActions;
using SIA_Portal.Accessors;
using SIA_Portal.CustomAccessors.EmployeeRecordsManager;

namespace SIA_PORTAL_UnitTest.ActionTest
{
    [TestClass]
    public class TEST_ActionsTest
    {


        private static PortalEmployeeRecordAccessor portalEmployeeRecordAccessor;

        private static EmployeeRecord.Builder builder01;
        private static EmployeeRecord.Builder builder02;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            portalEmployeeRecordAccessor = new PortalEmployeeRecordAccessor();

            builder01 = new EmployeeRecord.Builder();
            builder01.FirstName = "FirstName01";
            builder01.MiddleName = "MiddleName01";
            builder01.LastName = "LastName01";

            builder01.BirthDay = DateTime.Now.Date;
            builder01.Address = "Address01";
            builder01.ContactNumber = "01111111111";

            builder01.ElementarySchool = "Elementary01";
            builder01.HighSchool = "HighSchool01";
            builder01.College = "College01";

            builder01.PreviousCompanyName = "PreviousCompanyName01";
            builder01.PreviousCompanyPositionName = "PreviousCompanyPosition01";

            builder01.SeminarNameAttended = "SeminarName01";

            builder01.EmployeeDivCategoryId = -1;

            builder01.EmergencyContactName = "EmergencyContactName01";
            builder01.EmergencyContact = "01111111111";

            //

            builder02 = new EmployeeRecord.Builder();
            builder02.FirstName = "FirstName02";
            builder02.MiddleName = "MiddleName02";
            builder02.LastName = "LastName02";

            builder02.BirthDay = DateTime.Now.Date;
            builder02.Address = "Address02";
            builder02.ContactNumber = "02222222222";

            builder02.ElementarySchool = "Elementary02";
            builder02.HighSchool = "HighSchool02";
            builder02.College = "College02";

            builder02.PreviousCompanyName = "PreviousCompanyName02";
            builder02.PreviousCompanyPositionName = "PreviousCompanyPosition02";

            builder02.SeminarNameAttended = "SeminarName02";

            builder02.EmployeeDivCategoryId = -1;

            builder02.EmergencyContactName = "EmergencyContactName02";
            builder02.EmergencyContact = "02222222222";


        }

        [TestCleanup]
        public void TestMethodCleanup()
        {
            portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.DeleteAllEmployeeRecords();
        }

        //

        #region "Creation"

        [TestCategory("Creation")]
        [TestMethod]
        public void CreateEmployeeRecord_WithValidInputs_ShouldSucceed()
        {
            
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);
            Assert.AreNotEqual(-1, accId);

            //var acc = portalAccountAccessor.EmployeeAccountManagerHelper.GetAccountInfoFromId(accId);
            
        }

        [TestCategory("Creation")]
        [TestMethod]
        public void CreateEmployeeRecord_WithInvalidInputs_DETAILS_EmployeeDivDNE_ShouldFail()
        {
            var invalidBuilder = builder01.GetCopy();
            invalidBuilder.EmployeeDivCategoryId = 99999999;

            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryCreateEmployeeRecord(invalidBuilder);
            Assert.AreEqual(-1, accId);

        }

        [TestCategory("Creation")]
        [TestMethod]
        public void CreateEmployeeRecord_WithInvalidInputs_DETAILS_InputsAntiSQLInj_ShouldFail()
        {
            var invalidBuilder = builder01.GetCopy();
            invalidBuilder.FirstName = "Name<>;";

            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryCreateEmployeeRecord(invalidBuilder);
            Assert.AreEqual(-1, accId);
        }
        #endregion

        //

        #region Is related queries

        [TestCategory("Is query")]
        [TestMethod]
        public void IsRecordExists_InDatabaseWithId_WithRecord_ShouldReturnTrue()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            var isExists = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.IfEmployeeRecordIdExsists(accId);
            Assert.IsTrue(isExists);
        }

        [TestCategory("Is query")]
        [TestMethod]
        public void IsRecordExists_InDatabaseWithId_WithNoRecord_ShouldReturnFalse()
        {
            var isExists = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.IfEmployeeRecordIdExsists(-1);
            Assert.IsFalse(isExists);
        }

        #endregion

        //

        #region "Delete"

        [TestCategory("Delete")]
        [TestMethod]
        public void DeleteExistingRecord_ShouldSucceed()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            var delSucceed = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.DeleteEmployeeRecordWithId(accId);
            
            Assert.IsTrue(delSucceed);
            Assert.IsFalse(portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.IfEmployeeRecordIdExsists(accId));
        }

        [TestCategory("Delete")]
        [TestMethod]
        public void DeleteNonExistingRecord_ShouldFail()
        {
            var delSucceed = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryDeleteEmployeeRecordWithId(-1);
            Assert.IsFalse(delSucceed);
        }

        //

        [TestCategory("Delete All")]
        [TestMethod]
        public void DeletingAllRecords_With2Records_ShouldLeaveNoRecordAndReturnTrue()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var success = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.DeleteAllEmployeeRecords();
            Assert.IsTrue(success);

            Assert.IsFalse(portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.IfEmployeeRecordIdExsists(accId));
            Assert.IsFalse(portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.IfEmployeeRecordIdExsists(accId2));
        }


        [TestCategory("Delete All Account")]
        [TestMethod]
        public void DeletingAllRecords_With0Records_ShouldLeaveNoRecordAndReturnTrue()
        {
            var success = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.DeleteAllEmployeeRecords();
            Assert.IsTrue(success);
        }

        #endregion

        //

        #region "Get"

        [TestCategory("Get")]
        [TestMethod]
        public void GettingExistingRecord_ShouldReturnCorrectAccountInfo()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var rec01 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.GetEmployeeRecordInfoFromId(accId);
            
            Assert.AreEqual(accId, rec01.Id);
            Assert.AreEqual(builder01.FirstName, rec01.FirstName);
            Assert.AreEqual(builder01.MiddleName, rec01.MiddleName);
            Assert.AreEqual(builder01.LastName, rec01.LastName);

            Assert.AreEqual(builder01.BirthDay, rec01.BirthDay);
            Assert.AreEqual(builder01.Address, rec01.Address);
            Assert.AreEqual(builder01.ContactNumber, rec01.ContactNumber);

            Assert.AreEqual(builder01.ElementarySchool, rec01.ElementarySchool);
            Assert.AreEqual(builder01.HighSchool, rec01.HighSchool);
            Assert.AreEqual(builder01.College, rec01.College);

            Assert.AreEqual(builder01.PreviousCompanyName, rec01.PreviousCompanyName);
            Assert.AreEqual(builder01.PreviousCompanyPositionName, rec01.PreviousCompanyPositionName);

            Assert.AreEqual(builder01.SeminarNameAttended, rec01.SeminarNameAttended);

            Assert.AreEqual(builder01.EmployeeDivCategoryId, rec01.EmployeeDivCategoryId);

            Assert.AreEqual(builder01.EmergencyContact, rec01.EmergencyContact);
            Assert.AreEqual(builder01.EmergencyContactName, rec01.EmergencyContactName);
        }

        [TestCategory("Get")]
        [TestMethod]
        public void GettingNonExistingRecord_ShouldReturnNull()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var nonExistingAcc = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryGetEmployeeRecordInfoFromId(-1);
            Assert.AreEqual(null, nonExistingAcc);

        }

        #endregion

        //

        #region "Editing"

        [TestCategory("Editing")]
        [TestMethod]
        public void Editing_ExistingRecord_WithBuilder_WithValidInputs_ShouldReturnTrue()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            var success = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.EditEmployeeRecord(accId, builder02);
            var editedRec = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.GetEmployeeRecordInfoFromId(accId);


            Assert.IsTrue(success);

            Assert.AreEqual(accId, editedRec.Id);
            Assert.AreEqual(builder02.FirstName, editedRec.FirstName);
            Assert.AreEqual(builder02.MiddleName, editedRec.MiddleName);
            Assert.AreEqual(builder02.LastName, editedRec.LastName);

            Assert.AreEqual(builder02.BirthDay, editedRec.BirthDay);
            Assert.AreEqual(builder02.Address, editedRec.Address);
            Assert.AreEqual(builder02.ContactNumber, editedRec.ContactNumber);

            Assert.AreEqual(builder02.ElementarySchool, editedRec.ElementarySchool);
            Assert.AreEqual(builder02.HighSchool, editedRec.HighSchool);
            Assert.AreEqual(builder02.College, editedRec.College);

            Assert.AreEqual(builder02.PreviousCompanyName, editedRec.PreviousCompanyName);
            Assert.AreEqual(builder02.PreviousCompanyPositionName, editedRec.PreviousCompanyPositionName);

            Assert.AreEqual(builder02.SeminarNameAttended, editedRec.SeminarNameAttended);

            Assert.AreEqual(builder02.EmployeeDivCategoryId, editedRec.EmployeeDivCategoryId);

            Assert.AreEqual(builder02.EmergencyContact, editedRec.EmergencyContact);
            Assert.AreEqual(builder02.EmergencyContactName, editedRec.EmergencyContactName);

        }

        [TestCategory("Editing")]
        [TestMethod]
        public void Editing_ExistingRecord_WithBuilder_WithInvalidInputs_DETAILS_EmployeeDivDNE_ShouldReturnFalse()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            var invalidBuilder = builder02.GetCopy();
            invalidBuilder.EmployeeDivCategoryId = 99999999;

            var success = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryEditEmployeeRecord(accId, invalidBuilder);
            var rec01 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.GetEmployeeRecordInfoFromId(accId);

            Assert.IsFalse(success);

            Assert.AreEqual(accId, rec01.Id);
            Assert.AreEqual(builder01.FirstName, rec01.FirstName);
            Assert.AreEqual(builder01.MiddleName, rec01.MiddleName);
            Assert.AreEqual(builder01.LastName, rec01.LastName);

            Assert.AreEqual(builder01.BirthDay, rec01.BirthDay);
            Assert.AreEqual(builder01.Address, rec01.Address);
            Assert.AreEqual(builder01.ContactNumber, rec01.ContactNumber);

            Assert.AreEqual(builder01.ElementarySchool, rec01.ElementarySchool);
            Assert.AreEqual(builder01.HighSchool, rec01.HighSchool);
            Assert.AreEqual(builder01.College, rec01.College);

            Assert.AreEqual(builder01.PreviousCompanyName, rec01.PreviousCompanyName);
            Assert.AreEqual(builder01.PreviousCompanyPositionName, rec01.PreviousCompanyPositionName);

            Assert.AreEqual(builder01.SeminarNameAttended, rec01.SeminarNameAttended);

            Assert.AreEqual(builder01.EmployeeDivCategoryId, rec01.EmployeeDivCategoryId);

            Assert.AreEqual(builder01.EmergencyContact, rec01.EmergencyContact);
            Assert.AreEqual(builder01.EmergencyContactName, rec01.EmergencyContactName);
        }

        [TestCategory("Editing")]
        [TestMethod]
        public void Editing_ExistingRecord_WithBuilder_WithInvalidInputs_DETAILS_InputsAntiSQLInj_ShouldReturnFalse()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            var invalidBuilder = builder02.GetCopy();
            invalidBuilder.FirstName = "Name<>;";

            var success = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryEditEmployeeRecord(accId, invalidBuilder);
            var rec01 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.GetEmployeeRecordInfoFromId(accId);

            Assert.IsFalse(success);

            Assert.AreEqual(accId, rec01.Id);
            Assert.AreEqual(builder01.FirstName, rec01.FirstName);
            Assert.AreEqual(builder01.MiddleName, rec01.MiddleName);
            Assert.AreEqual(builder01.LastName, rec01.LastName);

            Assert.AreEqual(builder01.BirthDay, rec01.BirthDay);
            Assert.AreEqual(builder01.Address, rec01.Address);
            Assert.AreEqual(builder01.ContactNumber, rec01.ContactNumber);

            Assert.AreEqual(builder01.ElementarySchool, rec01.ElementarySchool);
            Assert.AreEqual(builder01.HighSchool, rec01.HighSchool);
            Assert.AreEqual(builder01.College, rec01.College);

            Assert.AreEqual(builder01.PreviousCompanyName, rec01.PreviousCompanyName);
            Assert.AreEqual(builder01.PreviousCompanyPositionName, rec01.PreviousCompanyPositionName);

            Assert.AreEqual(builder01.SeminarNameAttended, rec01.SeminarNameAttended);

            Assert.AreEqual(builder01.EmployeeDivCategoryId, rec01.EmployeeDivCategoryId);

            Assert.AreEqual(builder01.EmergencyContact, rec01.EmergencyContact);
            Assert.AreEqual(builder01.EmergencyContactName, rec01.EmergencyContactName);
        }


        [TestCategory("Editing Account")]
        [TestMethod]
        public void Editing_ExistingRecord_WithNullBuilder_ShouldReturnTrue()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            var success = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.TryEditEmployeeRecord(accId, null);
            var rec01 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.GetEmployeeRecordInfoFromId(accId);

            Assert.IsTrue(success);

            Assert.AreEqual(accId, rec01.Id);
            Assert.AreEqual(builder01.FirstName, rec01.FirstName);
            Assert.AreEqual(builder01.MiddleName, rec01.MiddleName);
            Assert.AreEqual(builder01.LastName, rec01.LastName);

            Assert.AreEqual(builder01.BirthDay, rec01.BirthDay);
            Assert.AreEqual(builder01.Address, rec01.Address);
            Assert.AreEqual(builder01.ContactNumber, rec01.ContactNumber);

            Assert.AreEqual(builder01.ElementarySchool, rec01.ElementarySchool);
            Assert.AreEqual(builder01.HighSchool, rec01.HighSchool);
            Assert.AreEqual(builder01.College, rec01.College);

            Assert.AreEqual(builder01.PreviousCompanyName, rec01.PreviousCompanyName);
            Assert.AreEqual(builder01.PreviousCompanyPositionName, rec01.PreviousCompanyPositionName);

            Assert.AreEqual(builder01.SeminarNameAttended, rec01.SeminarNameAttended);

            Assert.AreEqual(builder01.EmployeeDivCategoryId, rec01.EmployeeDivCategoryId);

            Assert.AreEqual(builder01.EmergencyContact, rec01.EmergencyContact);
            Assert.AreEqual(builder01.EmergencyContactName, rec01.EmergencyContactName);
        }

        #endregion

        //

        #region "Get All"

        [TestCategory("Get All")]
        [TestMethod]
        public void GettingAllRecords_InDatabaseWith2Records_Returns2Records()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            //

            var accList = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.GetAllEmployeeRecordsAsList();
            Assert.AreEqual(2, accList.Count);
        }

        [TestCategory("Get All")]
        [TestMethod]
        public void GettingAllAcounts_AsList_InDatabaseWith_0Accounts_Returns0Accounts()
        {
            var accList = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.GetAllEmployeeRecordsAsList();
            Assert.AreEqual(0, accList.Count);
        }



        #endregion

        //

        #region "Advanced Get"

        [TestCategory("Advanced Get")]
        [TestMethod]
        public void AdvancedGettingAllRecords_InDatabaseWith_2Records_WithAdvancedParams_WithFetch_WithOffset_ReturnsAppropriateAmount()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var advancedGetParam = new AdvancedGetParameters();
            advancedGetParam.Offset = 1;
            advancedGetParam.Fetch = 1;
            //advancedGetParam.TextToContain = "2";

            //

            var accList = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.AdvancedGetEmployeeRecordsAsList(advancedGetParam);

            var rec01 = accList[0];


            Assert.AreEqual(accId, rec01.Id);
            Assert.AreEqual(builder01.FirstName, rec01.FirstName);
            Assert.AreEqual(builder01.MiddleName, rec01.MiddleName);
            Assert.AreEqual(builder01.LastName, rec01.LastName);

            Assert.AreEqual(builder01.BirthDay, rec01.BirthDay);
            Assert.AreEqual(builder01.Address, rec01.Address);
            Assert.AreEqual(builder01.ContactNumber, rec01.ContactNumber);

            Assert.AreEqual(builder01.ElementarySchool, rec01.ElementarySchool);
            Assert.AreEqual(builder01.HighSchool, rec01.HighSchool);
            Assert.AreEqual(builder01.College, rec01.College);

            Assert.AreEqual(builder01.PreviousCompanyName, rec01.PreviousCompanyName);
            Assert.AreEqual(builder01.PreviousCompanyPositionName, rec01.PreviousCompanyPositionName);

            Assert.AreEqual(builder01.SeminarNameAttended, rec01.SeminarNameAttended);

            Assert.AreEqual(builder01.EmployeeDivCategoryId, rec01.EmployeeDivCategoryId);

            Assert.AreEqual(builder01.EmergencyContact, rec01.EmergencyContact);
            Assert.AreEqual(builder01.EmergencyContactName, rec01.EmergencyContactName);
        }


        [TestCategory("Advanced Get")]
        [TestMethod]
        public void AdvancedGettingAllRecords_InDatabaseWith_2Records_WithAdvancedParams_WithFetch_WithNoOffset_ReturnsAppropriateAmount()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var advancedGetParam = new AdvancedGetParameters();
            //advancedGetParam.Offset = 1;
            advancedGetParam.Fetch = 1;
            //advancedGetParam.TextToContain = "U";

            //

            var accList = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.AdvancedGetEmployeeRecordsAsList(advancedGetParam);

            var rec02 = accList[0];

            Assert.AreEqual(1, accList.Count);


            Assert.AreEqual(accId2, rec02.Id);
            Assert.AreEqual(builder02.FirstName, rec02.FirstName);
            Assert.AreEqual(builder02.MiddleName, rec02.MiddleName);
            Assert.AreEqual(builder02.LastName, rec02.LastName);

            Assert.AreEqual(builder02.BirthDay, rec02.BirthDay);
            Assert.AreEqual(builder02.Address, rec02.Address);
            Assert.AreEqual(builder02.ContactNumber, rec02.ContactNumber);

            Assert.AreEqual(builder02.ElementarySchool, rec02.ElementarySchool);
            Assert.AreEqual(builder02.HighSchool, rec02.HighSchool);
            Assert.AreEqual(builder02.College, rec02.College);

            Assert.AreEqual(builder02.PreviousCompanyName, rec02.PreviousCompanyName);
            Assert.AreEqual(builder02.PreviousCompanyPositionName, rec02.PreviousCompanyPositionName);

            Assert.AreEqual(builder02.SeminarNameAttended, rec02.SeminarNameAttended);

            Assert.AreEqual(builder02.EmployeeDivCategoryId, rec02.EmployeeDivCategoryId);

            Assert.AreEqual(builder02.EmergencyContact, rec02.EmergencyContact);
            Assert.AreEqual(builder02.EmergencyContactName, rec02.EmergencyContactName);
        }


        [TestCategory("Advanced Get")]
        [TestMethod]
        public void AdvancedGettingAllRecords_InDatabaseWith_2Records_WithAdvancedParams_WithNoFetch_WithOffset_ReturnsAppropriateAmount()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var advancedGetParam = new AdvancedGetParameters();
            advancedGetParam.Offset = 1;
            //advancedGetParam.Fetch = 1;
            //advancedGetParam.TextToContain = "2";

            //

            var accList = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.AdvancedGetEmployeeRecordsAsList(advancedGetParam);

            var rec01 = accList[0];


            Assert.AreEqual(1, accList.Count);

            Assert.AreEqual(accId, rec01.Id);
            Assert.AreEqual(builder01.FirstName, rec01.FirstName);
            Assert.AreEqual(builder01.MiddleName, rec01.MiddleName);
            Assert.AreEqual(builder01.LastName, rec01.LastName);

            Assert.AreEqual(builder01.BirthDay, rec01.BirthDay);
            Assert.AreEqual(builder01.Address, rec01.Address);
            Assert.AreEqual(builder01.ContactNumber, rec01.ContactNumber);

            Assert.AreEqual(builder01.ElementarySchool, rec01.ElementarySchool);
            Assert.AreEqual(builder01.HighSchool, rec01.HighSchool);
            Assert.AreEqual(builder01.College, rec01.College);

            Assert.AreEqual(builder01.PreviousCompanyName, rec01.PreviousCompanyName);
            Assert.AreEqual(builder01.PreviousCompanyPositionName, rec01.PreviousCompanyPositionName);

            Assert.AreEqual(builder01.SeminarNameAttended, rec01.SeminarNameAttended);

            Assert.AreEqual(builder01.EmployeeDivCategoryId, rec01.EmployeeDivCategoryId);

            Assert.AreEqual(builder01.EmergencyContact, rec01.EmergencyContact);
            Assert.AreEqual(builder01.EmergencyContactName, rec01.EmergencyContactName);
        }


        [TestCategory("Advanced Get")]
        [TestMethod]
        public void AdvancedGettingAllRecords_InDatabaseWith_2Records_WithAdvancedParams_WithNoFetch_WithNoOffset_ReturnsAppropriateAmount()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var advancedGetParam = new AdvancedGetParameters();
            //advancedGetParam.Offset = 1;
            //advancedGetParam.Fetch = 1;
            //advancedGetParam.TextToContain = "2";

            //

            var accList = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.AdvancedGetEmployeeRecordsAsList(advancedGetParam);

            Assert.AreEqual(2, accList.Count);
        }


        #endregion

        //

        #region "Advanced Get Account count"


        [TestCategory("Advanced Get Account Count")]
        [TestMethod]
        public void AdvancedGettingAllRecordCount_InDatabaseWith_2Records_WithAdvancedParams_WithFetch_WithOffset_ReturnsAppropriateAmount()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var advancedGetParam = new AdvancedGetParameters();
            advancedGetParam.Offset = 1;
            advancedGetParam.Fetch = 1;
            //advancedGetParam.TextToContain = "2";

            //

            var count = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.AdvancedGetEmployeeRecordsCount(advancedGetParam);

            Assert.AreEqual(1, count);

        }



        [TestCategory("Advanced Get Account Count")]
        [TestMethod]
        public void AdvancedGettingAllRecordCount_InDatabaseWith_2Records_WithAdvancedParams_WithFetch_WithNoOffset_ReturnsAppropriateAmount()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var advancedGetParam = new AdvancedGetParameters();
            //advancedGetParam.Offset = 1;
            advancedGetParam.Fetch = 3;
            //advancedGetParam.TextToContain = "2";

            //

            var count = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.AdvancedGetEmployeeRecordsCount(advancedGetParam);

            Assert.AreEqual(2, count);

        }



        [TestCategory("Advanced Get Account Count")]
        [TestMethod]
        public void AdvancedGettingAllRecordCount_InDatabaseWith_2Records_WithAdvancedParams_WithNoFetch_WithOffset_ReturnsAppropriateAmount()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var advancedGetParam = new AdvancedGetParameters();
            advancedGetParam.Offset = 1;
            //advancedGetParam.Fetch = ;
            //advancedGetParam.TextToContain = "2";

            //

            var count = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.AdvancedGetEmployeeRecordsCount(advancedGetParam);

            Assert.AreEqual(1, count);

        }


        [TestCategory("Advanced Get Account Count")]
        [TestMethod]
        public void AdvancedGettingAllRecordCount_InDatabaseWith_2Records_WithAdvancedParams_WithNoFetch_WithNoOffset_ReturnsAppropriateAmount()
        {
            int accId = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder01);

            int accId2 = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.CreateEmployeeRecord(builder02);


            var advancedGetParam = new AdvancedGetParameters();
            //advancedGetParam.Offset = 1;
            //advancedGetParam.Fetch = ;
            //advancedGetParam.TextToContain = "2";

            //

            var count = portalEmployeeRecordAccessor.EmployeeRecordDatabaseManagerHelper.AdvancedGetEmployeeRecordsCount(advancedGetParam);

            Assert.AreEqual(2, count);
        }

        #endregion

    }
}
