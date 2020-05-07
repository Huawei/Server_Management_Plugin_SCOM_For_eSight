using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Huawei.SCOM.ESightPlugin.ViewLib.OM12R2;
using Huawei.SCOM.ESightPlugin.ViewLib.Model;
using Microsoft.EnterpriseManagement.Configuration;
using Huawei.SCOM.ESightPlugin.ViewLib.Repo;
using Microsoft.EnterpriseManagement.Common;
using System.Linq;
using Microsoft.EnterpriseManagement.Monitoring;
using Huawei.SCOM.ESightPlugin.Core.Const;
using System.Collections.ObjectModel;

namespace ESightPlugin.Unittest
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class ScomMonitorAlert
    {
        public ScomMonitorAlert()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void InsertAlert()
        {
            var eSightHost = "192.168.0.2";
            var objects = OM12Connection.Query<PartialMonitoringObject>(ESightAppliance.EntityClassName, $"Host='{eSightHost}'");
            PartialMonitoringObject p = (PartialMonitoringObject) objects.First();

            //OM12Connection.HuaweiESightMG.GetMonitoringAlert();
        }

        [TestMethod]
        public void TestQueryAlert()
        {
            var objects = OM12Connection.All<PartialMonitoringObject>(EntityTypeConst.BladeServer.MainName).ToList();
            objects.ForEach(monitoringObject =>
            {
                var criteria = $"ResolutionState != '255' and CustomField5 = '16777219' " +
                           $"and CustomField8 = 'Event Subject=CPUMemoryCPU1Memory'";
                ReadOnlyCollection<MonitoringAlert> alerts = monitoringObject.GetMonitoringAlerts(new MonitoringAlertCriteria(criteria));
                Console.WriteLine(alerts.Count);
            });
           

            //OM12Connection.HuaweiESightMG.GetMonitoringAlert();

            //p.InsertCustomMonitoringEvent
            // We will identify the alert using suppression rule.
           
        }

        [TestMethod]
        public void TestQueryAlert2()
        {
            var criteria = "ResolutionState != '255' and CustomField10 = 'e990fa24-82c4-415a-9fe3-bb6d6c8cb369'";
            var lastModifiedTime = new DateTime(2019, 12, 27, 21, 49, 21).ToUniversalTime();
            var objects = OM12Connection.HuaweiESightMG.OperationalData.GetMonitoringAlerts(new MonitoringAlertCriteria(criteria), null);

            //var objects = OM12Connection.HuaweiESightMG.OperationalData.GetMonitoringAlerts(OM12Connection.HuaweiESightGuid, new MonitoringAlertCriteria(criteria), TraversalDepth.OneLevel, new DateTime());
            List<string> snList = new List<string>();
            objects.ToList().ForEach(alert =>
            {
                snList.Add(alert.CustomField6);
                // Console.WriteLine(alert.CustomField6);
            });

            snList.Sort();
            Console.WriteLine(string.Join("\n", snList));
        }
    }
}
