using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Messaging;

namespace ESightPlugin.Unittest
{
    /// <summary>
    /// MessageQueueUnittest 的摘要说明
    /// </summary>
    [TestClass]
    public class MessageQueueUnittest
    {
        public MessageQueueUnittest()
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

        public static MessageQueue CreatePrivate(string name)
        {
            string path = string.Format(@".\private$\{0}", name);
            if (!MessageQueue.Exists(path))
            {
                return MessageQueue.Create(path, true);
            }
            return new MessageQueue(path);
        }

        public static MessageQueue CreatePublic(string hostname, string queuename)
        {
            string path = string.Format(@"{0}\{1}", hostname, queuename);
            if (!MessageQueue.Exists(path))
            {
                return MessageQueue.Create(path, true);
            }
            return new MessageQueue(path);
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
        public void SendTextMessage()
        {
            var queuePath = "huawei-esight-alarms";

            // Open the queue.
            using (var queue = CreatePrivate(queuePath))
            {
                // Since we're using a transactional queue, make a transaction.
                using (MessageQueueTransaction mqt = new MessageQueueTransaction())
                {
                    mqt.Begin();
                    // Create a simple text message.
                    Message myMessage = new Message("Hello World", new BinaryMessageFormatter())
                    {
                        Label = "First Message"
                    };
                    // Send the message.
                    queue.Send(myMessage, mqt);
                    mqt.Commit();
                }
            }
        }

        [TestMethod]
        public void ReceiveMesage()
        {
            var queuePath = "huawei-esight-alarms";

            // Open the queue.
            using (var queue = CreatePrivate(queuePath))
            {
                queue.ReceiveCompleted += new ReceiveCompletedEventHandler(MyReceiveCompleted);
                queue.BeginReceive();
            }
        }

        private static void MyReceiveCompleted(Object source, ReceiveCompletedEventArgs asyncResult)
        {
            try
            {
                // Connect to the queue.
                MessageQueue mq = (MessageQueue) source;
                // End the asynchronous receive operation.
                Message m = mq.EndReceive(asyncResult.AsyncResult);
                m.Formatter = new BinaryMessageFormatter();

                Console.WriteLine("Message body: {0}", (string)m.Body);
                // Restart the asynchronous receive operation.
                mq.BeginReceive();
            }
            catch (MessageQueueException)
            {
                // Handle sources of MessageQueueException.
            }
        }
    }
}
