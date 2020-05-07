using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtil
{
    public class MessageQueueHelper
    {
        public static MessageQueue CreatePrivateQueue(string name, bool transactional)
        {
            string path = string.Format(@".\private$\{0}", name);
            if (!MessageQueue.Exists(path))
            {
                return MessageQueue.Create(path, transactional);
            }

            return new MessageQueue(path);
        }

        public static MessageQueue CreatePublicQueue(string hostname, string queuename, bool transactional)
        {
            string path = $"${hostname}\\${queuename}";
            if (!MessageQueue.Exists(path))
            {
                return MessageQueue.Create(path, transactional);
            }

            return new MessageQueue(path);
        }
    }
}
