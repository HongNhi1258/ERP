/*
   Author  : Vo Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 13-08-2008
   Company :  Dai Co   
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DaiCo.Application
{
  /// <summary>
  /// Summary description for EnDeCryptionString.
  /// </summary>
  public class Encryption64
  {
    public static string Decrypt(string stringToDecrypt,
      string sEncryptionKey)
    {

      byte[] key = { };
      byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
      byte[] inputByteArray = new byte[stringToDecrypt.Length];

      try
      {
        key = Encoding.GetEncoding("Shift-JIS").GetBytes(sEncryptionKey.Substring(0, 8));
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //inputByteArray = Convert.FromBase64String(stringToDecrypt);
        inputByteArray = ConvertHexStringToBytes(stringToDecrypt);

        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();

        Encoding encoding = Encoding.GetEncoding("Shift-JIS");
        return encoding.GetString(ms.ToArray());
      }
      catch
      {
        return "";
        //throw ex;
      }
    }

    public static string Encrypt(string stringToEncrypt,
      string sEncryptionKey)
    {

      byte[] key = { };
      byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
      byte[] inputByteArray; //Convert.ToByte(stringToEncrypt.Length)

      try
      {
        key = Encoding.GetEncoding("Shift-JIS").GetBytes(sEncryptionKey.Substring(0, 8));
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        inputByteArray = Encoding.GetEncoding("Shift-JIS").GetBytes(stringToEncrypt);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();

        //return Convert.ToBase64String(ms.ToArray());
        return ConvertBytesToHexString(ms.ToArray());
      }
      catch
      {
        return "";
        //throw ex;
      }
    }
    private static string ConvertBytesToHexString(byte[] data)
    {
      string hexString = "";
      foreach (byte b in data)
        hexString += String.Format("{0:X2}", b);
      return hexString;
    }
    private static byte[] ConvertHexStringToBytes(string hexString)
    {
      int numBytes = hexString.Length / 2;
      byte[] arrByte = new byte[numBytes];
      for (int i = 0; i < numBytes; ++i)
        arrByte[i] = byte.Parse(hexString.Substring(i * 2, 2),
          System.Globalization.NumberStyles.HexNumber);
      return arrByte;
    }
  }
}

