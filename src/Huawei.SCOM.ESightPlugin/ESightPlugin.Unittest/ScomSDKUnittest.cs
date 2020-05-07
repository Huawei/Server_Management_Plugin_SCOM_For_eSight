using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Huawei.SCOM.ESightPlugin.ViewLib.OM12R2;
using Huawei.SCOM.ESightPlugin.Core.Const;
using System.Linq;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Monitoring;
using Huawei.SCOM.ESightPlugin.ViewLib.Repo;
using Huawei.SCOM.ESightPlugin.Models;
using CommonUtil;

namespace ESightPlugin.Unittest
{
    /// <summary>
    /// ScomSDKUnittest 的摘要说明
    /// </summary>
    [TestClass]
    public class ScomSDKUnittest
    {
        public ScomSDKUnittest()
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
        public void TestMethod1()
        {
            // ISAvailable && AvailableLastModified && LastModified
            var objects = OM12Connection.All<PartialMonitoringObject>(EntityTypeConst.ESight.HuaweiServer).ToList();
            objects.ForEach(obj =>
            {

                var stateLastModified = obj.StateLastModified.HasValue ? obj.StateLastModified.Value : obj.TimeAdded;
                TimeSpan addedTimeLong = DateTime.UtcNow - stateLastModified.ToUniversalTime();
                TimeSpan expectTimeLong = TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(20);
                // the interval of monitor for our object is 5 minutes. So we will wait (5m + 20s).
                if (addedTimeLong <= expectTimeLong)
                {
                    Console.WriteLine($"{expectTimeLong - addedTimeLong}");
                }

                //Console.WriteLine(obj.TimeAdded);
                //PartialMonitoringObject po = (PartialMonitoringObject)obj;
                //Console.WriteLine($"{po.FullName}, {po.HealthState}");
                //po.RecalculateMonitoringState();
                //// obj.ResetMonitoringState();
                //Console.WriteLine($"{po.FullName}, {po.HealthState}");
            });

            //Console.WriteLine(objects.Count);
            //PartialMonitoringObject value = (PartialMonitoringObject) objects.First();
            //var result = value.ResetMonitoringState();

            //var monitors = OM12Connection.HuaweiESightMG.Monitoring.GetMonitorHierarchy(value);
            //ManagementPackMonitor monitor = monitors.Item;

            // var monitor = monitors.ChildNodes.ElementAt(3);


            //var filters = OM12Connection.HuaweiESightMG.Monitoring.GetMonitors(new ManagementPackMonitorCriteria("id = 'ESight.BladeServer.MainMonitor'"));


            //var health = value.HealthState;
            //Console.WriteLine(value.GetMostDerivedClasses());
            //Console.WriteLine(value.GetLeastDerivedNonAbstractClass());
            //Console.WriteLine(value.GetType());
            //Console.WriteLine(value.GetProperties());

            //            4
            //System.Collections.ObjectModel.ReadOnlyCollection`1[Microsoft.EnterpriseManagement.Configuration.ManagementPackClass]
            //ESight.HighdensityServer
            //Microsoft.EnterpriseManagement.Common.EnterpriseManagementObject
            //System.Collections.ObjectModel.ReadOnlyCollection`1[Microsoft.EnterpriseManagement.Configuration.ManagementPackProperty]
            //Rackserver

            //string deviceId = "192.168.0.2-Rack123456c";
            //IObjectReader<EnterpriseManagementObject> objectReader = OM12Connection.Query(EntityTypeConst.ESight.HuaweiServer, $"DN = '{deviceId}'",
            //    new ObjectQueryOptions(ObjectPropertyRetrievalBehavior.None));

            //EnterpriseManagementObject enterpriseManagementObject = objectReader.First();
            //Console.WriteLine(enterpriseManagementObject);
        }

        public void GetAllAlert()
        {
            OM12Connection.HuaweiESightConnector.GetMonitoringAlerts();
            // OM12Connection.HuaweiESightMG.GetMonitoringAlert();
            var objects = OM12Connection.All<EnterpriseManagementObject>(EntityTypeConst.ESight.HuaweiServer).ToList();
            MonitoringClass mpClass = (MonitoringClass)OM12Connection.GetManagementPackClass(EntityTypeConst.ESight.HuaweiServer);

            OM12Connection.HuaweiESightMG.GetMonitoringAlertReader(mpClass);
            Console.WriteLine(objects.Count);

        }


        [TestMethod]
        public void TestGetAllESightApplicanceAsync()
        {
            string clazzName = EntityTypeConst.BladeServer.Fan;
            ManagementPackClass clazz = OM12Connection.GetManagementPackClass(clazzName);
            var clazzProps = clazz.GetProperties();
            var list = OM12Connection.All<PartialMonitoringObject>(clazzName);
            var first = list.FirstOrDefault();

            var props = OM12Connection.GetManagementPackProperties(first);
            var baseProps = OM12Connection.GetManagementPackProperties(EntityTypeConst.ESight.HuaweiServer);

            var Status = first[props["Status"]].Value;
            var DN = first[baseProps["DN"]].Value;

            Console.WriteLine(clazz);
        }

        [TestMethod]
        public void TestGetPluginConfig()
        {
            var config = XmlHelper.Load(typeof(PluginConfig), @"C:\Users\qianbiao.ng\Desktop\PluginConfig.xml") as PluginConfig;
            Console.WriteLine(config.InternetIp);
        }
    }
}
