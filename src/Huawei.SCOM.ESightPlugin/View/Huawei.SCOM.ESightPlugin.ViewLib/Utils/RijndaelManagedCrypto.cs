//**************************************************************************  
//Copyright (C) 2019 Huawei Technologies Co., Ltd. All rights reserved.
//This program is free software; you can redistribute it and/or modify
//it under the terms of the MIT license.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//MIT license for more detail.
//*************************************************************************  
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Huawei.SCOM.ESightPlugin.ViewLib.Utils
{
    public class RijndaelManagedCrypto
    {

        private static readonly string key = "668DAFB758034A97";
        private static readonly byte[] csKey = Encoding.ASCII.GetBytes(key);
        private static readonly byte[] csInitVector = new byte[16];

        private ICryptoTransform _encryptor;
        private ICryptoTransform _decryptor;
        private UTF8Encoding _UTF8Encoder;

        public RijndaelManagedCrypto(byte[] key, byte[] iv)
        {
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            this._encryptor = rijndaelManaged.CreateEncryptor(key, iv);
            this._decryptor = rijndaelManaged.CreateDecryptor(key, iv);
            this._UTF8Encoder = new UTF8Encoding();
        }

        public RijndaelManagedCrypto() : this(RijndaelManagedCrypto.csKey, RijndaelManagedCrypto.csInitVector)
        {
        }

        public string Encrypt(string unencrypted)
        {
            return Convert.ToBase64String(this.Encrypt(this._UTF8Encoder.GetBytes(unencrypted)));
        }

        public byte[] Encrypt(byte[] buffer)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, this._encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(buffer, 0, buffer.Length);
            }
            return memoryStream.ToArray();
        }

        public string Decrypt(string encrypted)
        {
            return this._UTF8Encoder.GetString(this.Decrypt(Convert.FromBase64String(encrypted)));
        }

        public byte[] Decrypt(byte[] buffer)
        {
            MemoryStream memoryStream = new MemoryStream();
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, this._decryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(buffer, 0, buffer.Length);
            }
            return memoryStream.ToArray();
        }

        public string EncryptForCS(string unencrypted)
        {
            byte[] array = new byte[8];
            RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider();
            rngcryptoServiceProvider.GetBytes(array);
            string str = string.Format("{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}{6:x2}{7:x2}", new object[]
            {
                array[0],
                array[1],
                array[2],
                array[3],
                array[4],
                array[5],
                array[6],
                array[7]
            });
            return this.Encrypt(str + unencrypted);
        }

        public string DecryptFromCS(string encrypted)
        {
            return this.Decrypt(encrypted).Substring(16);
        }


        #region Singleton Provider
        private static readonly object SyncObject = new object();
        private static RijndaelManagedCrypto _instance = null;

        public static RijndaelManagedCrypto Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new RijndaelManagedCrypto();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion //Singleton Provider
    }
}
