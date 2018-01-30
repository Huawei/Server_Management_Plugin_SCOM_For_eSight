using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace CommonUtil
{
    /// <summary>
    /// 加密解密工具类。
    /// </summary>
    public class EncryptUtil
    {
        static string key = "668DAFB758034A97";
        /// <summary>
        /// RtlGenRandom 只有C++能够调用。
        /// .NET(C#)中的密码安全随机
        /// 普遍采用的方案是使用System.Security.Cryptography.RNGCryptoServiceProvider
        /// 详见： http://blog.csdn.net/asxinyu_usst/article/details/50703544
        /// </summary>
        /// <returns>随机的iv</returns>
        private static byte[] GetSafeRandomIV()
        {
            using (var random = new RNGCryptoServiceProvider())
            {
                var iv = new byte[16];
                random.GetBytes(iv);
                return iv;
            }
        }
        /// <summary>
        /// 获得一个x*2的长度的随机16进制字符串
        /// </summary>
        /// <param name="size">字符串的长度，默认x=8, 是8*2个字符</param>
        /// <returns>随机16进制字符串</returns>
        private static string GetRandomBitKey(int size = 8)
        {
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
                return BitConverter.ToString(data).Replace("-", String.Empty);
            }
        }
        private static void setAccesssToCurrentUserOnly(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            AuthorizationRuleCollection accessRules = file.GetAccessControl().GetAccessRules(true, true,
                                                   typeof(System.Security.Principal.SecurityIdentifier));

            System.Security.AccessControl.FileSecurity fileSecurity = file.GetAccessControl();
            IList<FileSystemAccessRule> existsList = new List<FileSystemAccessRule>();
            foreach (FileSystemAccessRule rule in accessRules)
            {
                //all rule.
                existsList.Add(rule);
            }
            //Add full control to curent user.
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            IdentityReference ir = wi.User.Translate(typeof(NTAccount));
            fileSecurity.AddAccessRule(new FileSystemAccessRule(ir, FileSystemRights.FullControl, AccessControlType.Allow));
            //administrators
            IdentityReference BuiltinAdministrators = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            fileSecurity.AddAccessRule(new FileSystemAccessRule(BuiltinAdministrators, FileSystemRights.FullControl, AccessControlType.Allow));

            //Clear all rules.
            foreach (FileSystemAccessRule rule in existsList)
            {
                if (!rule.IdentityReference.Equals(ir) && !rule.Equals(BuiltinAdministrators))
                    fileSecurity.RemoveAccessRuleAll(rule);
            }
            file.SetAccessControl(fileSecurity);
        }
        /// <summary>
        /// 初始化根密钥，并存储到目标文件夹。
        /// </summary>
        public static void InitRootKey()
        {
            SaveCurVersion();
            LogUtil.HWLogger.DEFAULT.Info("rootFile...");
            FileInfo fileInfo = new FileInfo(rootFile);//如果目录不存在重新创建。
            if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();
            using (FileStream fs = new FileStream(rootFile, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    //开始写入
                    string randKey = GetRandomBitKey(8);
                    sw.WriteLine(randKey);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                }
            }
            setAccesssToCurrentUserOnly(rootFile);
        }


        public static DateTime GetLatestKeyChangeDate()
        {
            FileInfo fileInfo = new FileInfo(mainFile);
            if (fileInfo.Exists)
            {
                return fileInfo.LastWriteTime;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// 重新初始化主密钥(工作密钥）
        /// </summary>
        public static void InitMainKey()
        {
            SaveCurVersion();
            LogUtil.HWLogger.DEFAULT.Info("InitMainKey...");
            FileInfo fileInfo = new FileInfo(mainFile);//如果目录不存在重新创建。
            if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();

            using (FileStream fs = new FileStream(mainFile, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string rootkey = GetRootKeyFromPath();
                    string trueKey = GetRandomBitKey(16);

                    string trueEnKey = EncryptWithKey(rootkey, trueKey);
                    sw.WriteLine(trueEnKey);
                    //清空缓冲区
                    sw.Flush();
                }
            }
            setAccesssToCurrentUserOnly(mainFile);
        }
        /// <summary>
        /// 从指定文件中读取一行。
        /// </summary>
        /// <param name="filepath">指定的文件</param>
        /// <returns>第一行数据</returns>
        private static string ReadOneLineFromFile(string filepath)
        {
            string line = "";
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long byteLen = fs.Length;
                if (byteLen < 1024)
                {
                    byte[] buffer = new Byte[byteLen];
                    fs.Read(buffer, 0, Convert.ToInt32(byteLen));
                    line = System.Text.Encoding.UTF8.GetString(buffer);
                }
            }
            return line.Trim();
        }
#if !DEBUG
        //static string rootPath = System.Environment.GetEnvironmentVariable("userprofile");
        //static string rootPath = @"C:\Program Files (x86)\WebServer";
        static string rootPath = System.Environment.GetEnvironmentVariable("ESIGHTSCOMPLUGIN");

#else
        static string rootPath = System.Environment.GetEnvironmentVariable("ESIGHTSCOMPLUGIN");
#endif
        /// <summary>
        /// 密钥存储路径，放在user目录下。
        /// </summary>
        static string rootFolderPath = rootPath + "\\KN";
        //密钥组件分散保存，当密钥组件存储于文件中时，不能所有的组件文件放在同一目录下存储；
        //因此加多一层目录.
        static string rootFile = rootPath + "\\KN\\base.kn";
        static string mainFile = rootPath + "\\KN\\main\\main.kn";
        /// <summary>
        /// 获得根密钥
        /// </summary>
        /// <returns>根密钥</returns>
        public static string GetRootKey()
        {
            LogUtil.HWLogger.DEFAULT.Info("GetRootKey...");
            return GetRootKeyFromPath();
        }


        #region AES加解密
        /// <summary>
        /// Aes 加密，随机生成16位IV。
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string EncryptWithKey(string enKey, string val)
        {
            // LogUtil.HWLogger.DEFAULT.InfoFormat("EncryptWithKey->enKey={0},val={1}", enKey, val);
            if (string.IsNullOrEmpty(val)) return val;
            try
            {
                using (AesManaged aes = new AesManaged())
                {
                    var toEncryptBytes = Encoding.UTF8.GetBytes(val);

                    byte[] enKeyByte = Convert.FromBase64String(enKey);
                    aes.Key = enKeyByte;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.Zeros;
                    aes.IV = GetSafeRandomIV();

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        using (CustomeStream ms = new CustomeStream())
                        {
                            ms.Write(aes.IV, 0, 16);
                            using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(toEncryptBytes, 0, toEncryptBytes.Length);
                                cs.FlushFinalBlock();
                                byte[] bytes = (byte[])ms.ToArray();
                                return Convert.ToBase64String(bytes);
                            }
                        }
                    }
                }
            }
            catch (CryptographicException e)
            {
                LogUtil.HWLogger.DEFAULT.Error("A Cryptographic error occurred:", e);
                if (string.IsNullOrWhiteSpace(val))
                    return "";
                else
                    return val;
            }
            catch (FormatException fe)
            {
                LogUtil.HWLogger.DEFAULT.Error("A FormatException error occurred:", fe);
                if (string.IsNullOrWhiteSpace(val))
                    return "";
                else
                    return val;
            }
        }
        /// <summary>
        /// Aes解密。
        /// </summary>
        /// <param name="val">需要解密的字符串</param>
        /// <returns></returns>
        public static string DecryptWithKey(string enKey, string val)
        {
            try
            {
                String retVal = "";
                if (string.IsNullOrEmpty(val)) return "";

                using (AesManaged aes = new AesManaged())
                {
                    byte[] inputByteArray = Convert.FromBase64String(val.Trim());
                    byte[] enKeyByte = Convert.FromBase64String(enKey);

                    aes.Key = enKeyByte;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.Zeros;
                    using (CustomeStream ms = new CustomeStream(inputByteArray))
                    {
                        byte[] iv = new byte[16];
                        ms.Read(iv, 0, iv.Length);
                        // Array.Copy(inputByteArray, 0, iv, 0, iv.Length);
                        aes.IV = iv;
                        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                        {
                            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                            {
                                using (var sr = new StreamReader(cs))
                                {
                                    retVal = sr.ReadToEnd().Trim('\0');
                                }
                            }
                        }
                    }
                }

                //LogUtil.HWLogger.DEFAULT.InfoFormat("DecryptWithKey->enKey={0},val={1},retVal={2}", enKey, val, retVal);
                return retVal;
            }
            catch (CryptographicException e)
            {
                LogUtil.HWLogger.DEFAULT.Error("A Cryptographic error occurred:", e);
                if (string.IsNullOrWhiteSpace(val))
                    return "";
                else
                    return val;
            }
            catch (FormatException fe)
            {
                LogUtil.HWLogger.DEFAULT.Error("A FormatException error occurred:", fe);
                if (string.IsNullOrWhiteSpace(val))
                    return "";
                else
                    return val;
            }

        }
        #endregion

        private static string XOrTwoString(string str1, string str2)
        {
            byte[] aBytes = System.Text.Encoding.UTF8.GetBytes(str1);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str2);

            byte[] rBytes = new byte[aBytes.Length];
            for (int i = 0; i < aBytes.Length; i++)
            {
                rBytes[i] = (byte)(aBytes[i] ^ bytes[i]);
            }
            string result = Convert.ToBase64String(rBytes);
            return result;
        }

        #region gen rootkey
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            byte[] returnBytes = new byte[hexString.Length];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = (byte)hexString[i];
            return returnBytes;
        }
        /// <summary>
        /// GenRootKey specified plaintext 
        /// </summary>
        /// <param name="plainText">
        /// Plaintext value to be GenRootKey.
        /// </param>
        /// <param name="saltValue">
        /// Salt value used along with passphrase to generate pd. Salt can
        /// be any string. In this example we assume that salt is an ASCII string.
        /// </param>
        /// <param name="pdIterations">
        /// Number of iterations used to generate pd. One or two iterations
        /// should be enough.
        /// </param>
        /// <param name="keySize">
        /// Size of encryption key in bits. Allowed values are: 128, 192, and 256. 
        /// Longer keys are more secure than shorter keys.
        /// </param>
        /// <returns>
        /// key string.
        /// </returns>
        public static string GenRootKey(
            string passPhrase,
            string saltValue,
            int pdIterations,
            int keySize
        )
        {

            byte[] saltValueBytes = strToToHexByte(saltValue);

            /*
             * 2017-10-09 Using ssl dll to gen the pd.
           */
            /*
             参数说明：
             1. temp_component： 16字节硬编码密钥组件 与 16字节文件中密钥组件 异或后的值；
             2. component_size: 密钥组件长度 16字节；
             3. compoent3：16字节盐值，可以是硬编码的值；
             4. COMPONENT_SIZE：盐值长度，16字节
             5. ITERATION_NUM：迭代次数，10000次
             6. EVP_sha256：HASH算法，SHA256
             7. ROOTKEY_SIZE： 导出的根密钥长度，16字节
             8. rootkey： 导出的根密钥
             */
            byte[] keyBytes = new byte[32];
            PKCS5_PBKDF2_HMAC(passPhrase, 16, saltValueBytes, 16, pdIterations, EVP_sha256(), 32, keyBytes);
            string retVal = Convert.ToBase64String(keyBytes); ;
            string byteStr = BitConverter.ToString(keyBytes).Replace("-", String.Empty);
            //LogUtil.HWLogger.DEFAULT.InfoFormat("32 passPhrase={0},saltValue={1},retVal={2},byteStr={3}", passPhrase, saltValue, retVal, byteStr);
            return retVal;
        }
        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static int PKCS5_PBKDF2_HMAC(string pass, int passlen, byte[] salt, int saltlen, int iter,
            IntPtr digest, int keylen, byte[] outBytes);
        [DllImport("libeay32.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr EVP_sha256();
        #endregion gen rootkey
        public static string GetRootKeyFromPath()
        {
            return GetRootKeyFromPath(true);
        }

        /// <summary>
        /// 从指定路径获得根密钥。
        /// </summary>
        /// <param name="folderPath">指定路径</param>
        /// <returns>根密钥</returns>
        public static string GetRootKeyFromPath(bool isInit)
        {
            if (!File.Exists(rootFile))
            {
                if (isInit)
                    InitRootKey();//如果没有存在，则重新生成。
                else
                    return "";
            }

            string randKey = ReadOneLineFromFile(rootFile);//根密钥

            string plainText = XOrTwoString(key, randKey);  // original plaintext

            //  LogUtil.HWLogger.DEFAULT.Info("plainText..." +Encoding.Unicode.GetBytes(plainText)+":"
            //+BitConverter.ToString(Encoding.Unicode.GetBytes(plainText)).Replace("-", String.Empty));


            string saltValue = key;        // can be any string
                                           // string hashAlgorithm = "SHA256";             // can be "MD5"
            int pdIterations = 10000;                // can be any number
            int keySize = 128;                // can be 192 or 128

            /*
             string trueEnKey = GenRootKey(plainText, saltValue,
                     hashAlgorithm, pdIterations, keySize);*/
            string trueEnKey = GenRootKey(plainText, saltValue, pdIterations, keySize);

            //LogUtil.HWLogger.DEFAULT.Info("trueEnKey..." + trueEnKey);
            return trueEnKey;//根密钥
        }

        /// <summary>
        /// 从指定路径获得主密钥(工作密钥)。
        /// </summary>
        /// <param name="rootKey">根秘钥</param>
        /// <returns>主密钥(工作密钥)</returns>
        public static string GetMainKeyByRootKeyAndPath(bool isInit, string rootKey)
        {
            if (!File.Exists(mainFile))
            {
                if (isInit)
                    InitMainKey();//如果没有存在，则重新生成。
                else
                    return "";
            }
            string mainKey = ReadOneLineFromFile(mainFile);//原始密钥

            string rootkey = GetRootKeyFromPath();

            string trueKey = DecryptWithKey(rootkey, mainKey);
            //2. 解密获得原始密钥
            return trueKey;
        }
        /// <summary>
        /// 从指定路径获得主密钥(工作密钥)。
        /// </summary>
        /// <param name="folderPath">指定路径</param>
        /// <returns>主密钥(工作密钥)</returns>
        public static string GetMainKeyFromPath()
        {
            LogUtil.HWLogger.DEFAULT.Info("GetMainKeyFromPath...");
            string rootKey = GetRootKeyFromPath();//1.获得根秘钥
            return GetMainKeyByRootKeyAndPath(true, rootKey);
        }
        public static string GetMainKeyWithoutInit()
        {
            LogUtil.HWLogger.DEFAULT.Info("GetMainKeyFromPath...");
            string rootKey = GetRootKeyFromPath(false);//1.获得根秘钥
            return GetMainKeyByRootKeyAndPath(false, rootKey);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="pwdStr">需要加密的密码</param>
        /// <returns>加密后的密码</returns>
        public static string EncryptPwd(string pwdStr)
        {
            return EncryptWithKey(GetMainKeyFromPath(), pwdStr);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="pwdStr">需要解密的密码</param>
        /// <returns>解密后的密码</returns>
        public static string DecryptPwd(string pwdStr)
        {
            return DecryptWithKey(GetMainKeyFromPath(), pwdStr);
        }

        #region 兼容旧的密钥
        static string versionFile = rootPath + "\\KN\\vrn.kn";
        static string curVersion = "1.0.7";
        /// <summary>
        /// 是否兼容的加密版本。
        /// </summary>
        /// <returns>是 或者 否</returns>
        public static bool IsCompatibleVersion()
        {
            if (!File.Exists(rootFile)) return true;
            if (!File.Exists(versionFile))
            {
                LogUtil.HWLogger.DEFAULT.Info("IsCompatibleVersion=!File.Exists...");
                return false;
            }
            string fVersion = ReadOneLineFromFile(versionFile);
            if (!string.Equals(CoreUtil.GetObjTranNull<string>(fVersion).Trim(), curVersion))
            {
                LogUtil.HWLogger.DEFAULT.InfoFormat("IsCompatibleVersion=!string.Equals({0},curVersion)...", fVersion);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 2017-10-11 清除并升级密钥。
        /// </summary>
        public static void ClearAndUpgradeKey()
        {
            LogUtil.HWLogger.DEFAULT.Info("ClearAndUpgradeKey...");
            if (File.Exists(rootFile)) File.Delete(rootFile);
            if (File.Exists(mainFile)) File.Delete(mainFile);
            SaveCurVersion();
            InitRootKey();
            InitMainKey();

        }
        public static void SaveCurVersion()
        {
            if (File.Exists(versionFile)) return;
            FileInfo fileInfo = new FileInfo(versionFile);//如果目录不存在重新创建。
            if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();

            using (FileStream fs = new FileStream(versionFile, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(curVersion);
                    //清空缓冲区
                    sw.Flush();
                }
            }
            setAccesssToCurrentUserOnly(versionFile);
        }

        /// <summary>
        /// 获得105的密钥
        /// </summary>
        public static string GetRootKey1060()
        {
            if (!File.Exists(rootFile)) return "";//如果没有存在，则返回为空。

            string randKey = ReadOneLineFromFile(rootFile);//根密钥
            string plainText = XOrTwoString1060(key, randKey);  // original plaintext

            string saltValue = key;        // can be any string
            string hashAlgorithm = "SHA256";             // can be "MD5"
            int pdIterations = 10000;                // can be any number
            int keySize = 128;                // can be 192 or 128

            string trueEnKey = GenRootKey1060(plainText, saltValue,
                     hashAlgorithm, pdIterations, keySize);

            return trueEnKey;
        }
        private static string XOrTwoString1060(string str1, string str2)
        {
            byte[] aBytes = System.Text.Encoding.UTF8.GetBytes(str1);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str2);

            byte[] rBytes = new byte[aBytes.Length];
            for (int i = 0; i < aBytes.Length; i++)
            {
                rBytes[i] = (byte)(aBytes[i] ^ bytes[i]);
            }
            string result = System.Text.Encoding.Unicode.GetString(rBytes);
            return result;
        }

        private static string GenRootKey1060(
            string passPhrase,
            string saltValue,
            string hashAlgorithm,
            int pdIterations,
            int keySize
        )
        {
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // First, we must create a pd, from which the key will be derived.
            // This pd will be generated from the specified passphrase and 
            // salt value. The pd will be created using the specified hash 
            // algorithm. pd creation can be done in several iterations.
            Rfc2898DeriveBytes pd = new Rfc2898DeriveBytes
            (
                passPhrase,
                saltValueBytes,
                // hashAlgorithm,
                pdIterations
            );

            // Use the pd to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = pd.GetBytes(keySize / 8);
            return BitConverter.ToString(keyBytes).Replace("-", String.Empty);
        }
        /// <summary>
        /// 从指定路径获得主密钥(工作密钥)。
        /// </summary>
        /// <returns>主密钥(工作密钥)</returns>
        public static string GetMainKey1060()
        {
            string rootKey = GetRootKey1060();//根秘钥
            if (!File.Exists(mainFile)) return "";//如果没有存在，则重新生成。

            string mainKey = ReadOneLineFromFile(mainFile);//原始密钥

            string trueKey = DecryptWithKey(rootKey, mainKey);//2. 解密获得原始密钥
            return trueKey;
        }
        #endregion 兼容旧的密钥
    }
}
