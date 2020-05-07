using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Timers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Timer = System.Timers.Timer;

namespace ESightPlugin.Unittest
{
    /// <summary>
    /// GrammerUnittest 的摘要说明
    /// </summary>
    [TestClass]
    public class GrammerUnittest
    {
        public GrammerUnittest()
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
            TimeSpan addedTimeLong = DateTime.Now - new DateTime(2019, 12, 25, 19, 30, 0);
            TimeSpan expectTimeLong = TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(20);
            if (addedTimeLong < expectTimeLong)
            {
                Console.WriteLine(expectTimeLong - addedTimeLong);
            }
        }


        [TestMethod]
        public void TestTimer()
        {
            // 开启一个timer来监听本次轮询任务是否执行完
            bool isCompleted = false;
            Timer timer = new Timer(1000)
            {
                AutoReset = false,
            };
            timer.Elapsed += (sender, e) =>
            {
                try
                {
                    if (isCompleted)
                    {
                        Console.WriteLine("Not Completed");
                    } else
                    {
                        Console.WriteLine("Completed");
                    }
                } finally
                {
                    if (!isCompleted)
                    {
                        timer.Start();
                    }
                }
            };
            timer.Start();

            Thread.Sleep(10000);
            isCompleted = true;
            Thread.Sleep(5000);
        }


        [TestMethod]
        public void TestMethod3()
        {
          
                Console.WriteLine($"{(int)EventLogEntryType.Information}15");
        }
    }
}
