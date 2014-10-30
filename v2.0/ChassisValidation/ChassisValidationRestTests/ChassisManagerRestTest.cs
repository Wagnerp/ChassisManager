// Copyright © Microsoft Open Technologies, Inc.
// All Rights Reserved
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at 
// http://www.apache.org/licenses/LICENSE-2.0 

// THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR
// CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 
// See the Apache 2 License for the specific language governing permissions and limitations under the License. 

using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChassisValidation
{
    [TestClass]
    public class ChassisManagerRestTest
    {
        private static readonly string cmUrl = ConfigurationManager.AppSettings["CM_URL"];
        private static readonly string adminUserName = ConfigurationManager.AppSettings["AdminUserName"];
        private static readonly string password = ConfigurationManager.AppSettings["Password"];

        private static readonly string testDomainTestUser = ConfigurationManager.AppSettings["LabDomainTestUser"];
        private static readonly string testDomainName = ConfigurationManager.AppSettings["LabDomainName"];
       
        private static CmRestTest adminUserCmTest;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) 
        {
            adminUserCmTest = new CmRestTest(cmUrl, adminUserName, password, testDomainTestUser, testDomainName);
        }

        [TestMethod]
        public void GetChassisInfoTest()
        {
            Assert.IsTrue(adminUserCmTest.GetChassisInfoTest());
        }

        [TestMethod]
        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Get Info Actions\Blade")]
        public void GetAllBladesInfoTest()
        {
            Assert.IsTrue(adminUserCmTest.GetAllBladesInfoTest());
        }

        [TestMethod]
        public void GetBladeInfoTest()
        {
            Assert.IsTrue(adminUserCmTest.GetBladeInfoTest());
        }
       
        [TestMethod]
        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Get Health Actions\CM")]
        [TestCategory("BVT")]
        public void GetChassisHealthTest() 
        {
            Assert.IsTrue(adminUserCmTest.GetChassisHealthTest());
        }

        [TestMethod]
        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Get Health Actions\Blade")]
        [TestCategory("BVT")]
        public void GetBladeHealthTest() 
        {
            Assert.IsTrue(adminUserCmTest.GetBladeHealthTest());        
        }
        
        [TestMethod]
        public void ReadClearBladeLogTest()
        {
            Assert.IsTrue(adminUserCmTest.ReadClearBladeLogTest());
        }

        [TestMethod]
        public void ReadBladeLogWithTimestampTest() 
        {
            Assert.IsTrue(adminUserCmTest.ReadBladeLogWithTimestampTest());
        }

        [TestMethod]
        public void ReadChassisLogTest()
        {
            Assert.IsTrue(adminUserCmTest.ReadChassisLogTest());
        }

        [TestMethod]
        public void ClearChassisLogTest()
        {
            Assert.IsTrue(adminUserCmTest.ClearChassisLogTest());
        }

        [TestMethod]
        public void ReadChassisLogWithTimestampTest()
        {
            Assert.IsTrue(adminUserCmTest.ReadChassisLogWithTimestampTest());
        }


        [TestMethod]
        public void UserLogsTest()
        {
            Assert.IsTrue(adminUserCmTest.UserLogsTest());
        }

        [TestMethod]
        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power\Hard Power")]
        public void SetGetAllPowerStateTest() 
        {
            Assert.IsTrue(adminUserCmTest.SetGetAllPowerStateTest());
        }

        [TestMethod]
        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power\Hard Power")]
        public void SetGetPowerStateTest()
        {
            Assert.IsTrue(adminUserCmTest.SetGetPowerStateTest());
        }

        [TestMethod]
        public void SetGetNextBootTest()
        {
            Assert.IsTrue(adminUserCmTest.SetGetNextBootTest());
        }

        [TestMethod]
        public void StartStopBladeSerialSessionTest()
        {
            Assert.IsTrue(adminUserCmTest.StartStopBladeSerialSessionTest());
        }

        [TestMethod]
        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power\Power Cycle")]        
        public void SetBladeActivePowerCycleTest() 
        {
            Assert.IsTrue(adminUserCmTest.SetBladeActivePowerCycleTest());
        }

        [TestMethod]
        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power\Power Cycle")]        
        public void SetAllBladesActivePowerCycleTest()
        {
            Assert.IsTrue(adminUserCmTest.SetAllBladesActivePowerCycleTest());
        }

        [TestMethod]
        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power\Soft Power")]        
        public void SetGetAllBladesStateTest() 
        {
            Assert.IsTrue(adminUserCmTest.SetGetAllBladesStateTest());
        }

        [TestMethod]
        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power\Soft Power")]        
        public void SetGetBladeStateTest()
        {
            Assert.IsTrue(adminUserCmTest.SetGetBladeStateTest());
        }

        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power\Default Power State")]
        [TestMethod]
        public void SetGetAllBladesDefaultPowerStateTest() 
        {
            Assert.IsTrue(adminUserCmTest.SetGetAllBladesDefaultPowerStateTest());
        }

        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power\Default Power State")]
        [TestMethod]
        public void SetGetBladeDefaultPowerStateTest()
        {
            Assert.IsTrue(adminUserCmTest.SetGetBladeDefaultPowerStateTest());
        }

        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power")]
        [TestMethod]
        public void SetPowerActionsByAllUsersTest()
        {
            Assert.IsTrue(adminUserCmTest.SetPowerActionsByAllUsersTest());
        }

        [Description(@"TFS Area Path: Mt Rainier\Microsoft\Test\Chassis Manager\Functional Testing\Rest API Commands\Power Actions\Blade Power")]
        [TestMethod]
        public void GetPowerActionsByAllUsersTest()
        {
            Assert.IsTrue(adminUserCmTest.GetPowerActionsByAllUsersTest());
        }

        [TestMethod]
        public void GetChassisManagerAssetInfoTest()
        {
            Assert.IsTrue(adminUserCmTest.GetChassisManagerAssetInfoTest());
        }

        [TestMethod]
        public void GetPdbAssetInfoTest()
        {
            Assert.IsTrue(adminUserCmTest.GetPdbAssetInfoTest());
        }

        [TestMethod]
        public void GetBladeAssetInfo()
        {
            Assert.IsTrue(adminUserCmTest.GetBladeAssetInfoTest());
        }

        [TestMethod]
        public void SetChassisManagerAssetInfoTest()
        {
            Assert.IsTrue(adminUserCmTest.SetChassisManagerAssetInfoTest());
        }

        [TestMethod]
        public void SetPdbAssetInfoTest()
        {
            Assert.IsTrue(adminUserCmTest.SetPdbAssetInfoTest());
        }

        [TestMethod]
        public void SetBladeAssetInfoTest()
        {
            Assert.IsTrue(adminUserCmTest.SetBladeAssetInfoTest());
        }
    }
}
