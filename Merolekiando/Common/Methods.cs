﻿using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using System;
using System.IO;

namespace Merolekando.Common
{
    public class Methods
    {
        //public static string baseurl = "https://localhost:44303/";
        //public static string baseurl = "http://64.20.48.40/";
        public static string baseurl = "http://173.225.99.89/";
        //public static string baseurl = "https://smallyellowbook30.conveyor.cloud/";
        public static string RemoveWhitespace(string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        public static string Encrypt(string originalString)
        {
            //pass = originalString.ToString();
            return Encrypt(originalString, getKey);
        }
        public static string Encrypt(string originalString, byte[] bytes)
        {
            try
            {
                if (String.IsNullOrEmpty(originalString))
                {
                    throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
                }

                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);

                StreamWriter writer = new StreamWriter(cryptoStream);
                writer.Write(originalString);
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                writer.Flush();

                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length).Replace("+", "-").Replace("/", "_");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Decrypt(string originalString)
        {
            return Decrypt(originalString.Replace("-", "+").Replace("_", "/"), getKey);
        }
        public static string Decrypt(string cryptedString, byte[] bytes)
        {
            try
            {
                if (String.IsNullOrEmpty(cryptedString))
                {
                    throw new ArgumentNullException("The string which needs to be decrypted can not be null.");
                }

                cryptedString = Regex.Replace(cryptedString, "[ ]", "+");

                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cryptoStream);

                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static byte[] getKey
        {
            get
            {
                ConfigurationManager.AppSettings["EncryptKey"] = "vtCRMkey";
                return ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings["EncryptKey"]);
            }
        }
    }
}
