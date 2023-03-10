using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Accounting;
using DaiCo.Shared.DataSetSource.CustomerService;
using DaiCo.Shared.ReportTemplate.Accounting;
using DaiCo.Shared.ReportTemplate.CustomerService;
using FormSerialisation;
using Infragistics.Win.UltraWinGrid;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Shared.Utility
{
  public class FunctionUtility
  {
    public static string EncodePassword(string originalPassword)
    {
      //Declarations
      Byte[] originalBytes;
      Byte[] encodedBytes;
      MD5 md5;

      //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
      md5 = new MD5CryptoServiceProvider();
      originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
      encodedBytes = md5.ComputeHash(originalBytes);

      //Convert encoded bytes back to a 'readable' string
      return BitConverter.ToString(encodedBytes);
    }

    /// <summary>
    /// Get title of code master, with correlative group and code.
    /// </summary>
    /// <param name="group"></param>
    /// <param name="code"></param>
    /// <returns>Title of code master</returns>
    public static string GetCodeMasterTitle(int group, int code)
    {
      string sql = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = @Group AND Code = @Code AND DeleteFlag = 0";
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@Group", DbType.Int32, group);
      inputParam[1] = new DBParameter("@Code", DbType.Int32, code);
      object obj = DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(sql, inputParam);
      string result = string.Empty;
      if (obj != null)
        result = (string)obj;
      return result;
    }
    /// <summary>
    /// Store xml file
    /// </summary>
    /// <param name="strMessageId">varFilePath</param>
    /// <param name="strMessageId">Pid</param>
    public static long databaseFilePut(string varFilePath, string strTypeObject, string strTitle, string strFileName)
    {
      byte[] file;
      using (FileStream stream = new FileStream(varFilePath, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader reader = new BinaryReader(stream))
        {
          file = reader.ReadBytes((int)stream.Length);
        }
      }
      DBParameter[] param = new DBParameter[5];
      param[0] = new DBParameter("@File", DbType.Binary, file.Length, file);
      param[1] = new DBParameter("@TypeObject", DbType.AnsiString, 500, strTypeObject);
      param[2] = new DBParameter("@Title", DbType.AnsiString, 300, strTitle);
      param[3] = new DBParameter("@FileName", DbType.String, 300, strFileName);
      param[4] = new DBParameter("@CreatedBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spGNRTaskTransfer_Insert", param, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return result;
    }
    public static long databaseFilePut(MemoryStream stream, string strTypeObject, string strTitle, string strFileName)
    {
      byte[] file;
      file = stream.ToArray();
      stream.Close();
      DBParameter[] param = new DBParameter[5];
      param[0] = new DBParameter("@File", DbType.Binary, file.Length, file);
      param[1] = new DBParameter("@TypeObject", DbType.AnsiString, 500, strTypeObject);
      param[2] = new DBParameter("@Title", DbType.AnsiString, 300, strTitle);
      param[3] = new DBParameter("@FileName", DbType.String, 300, strFileName);
      param[4] = new DBParameter("@CreatedBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spGNRTaskTransfer_Insert", param, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return result;
    }

    public static void CaptureForm(MainUserControl c)
    {
      string strTypeObject = c.GetType().FullName + "," + c.GetType().Namespace.Split('.')[1];
      string strTitle = SharedObject.tabContent.TabPages[SharedObject.tabContent.SelectedIndex].Text;
      string strFileName = c.Name + ".xml";
      long TaskPid = FunctionUtility.databaseFilePut(FormSerialisor.Serialise(c), strTypeObject, strTitle, strFileName);
      viewMAI_01_002 o = new viewMAI_01_002();
      o.pid = TaskPid;
      DaiCo.Shared.Utility.WindowUtinity.ShowView(o, "TASK TRANSFER", true, ViewState.MainWindow);
    }
    /// <summary>
    /// Get contend of message base on message id, them defined in file Messages.txt at startup folder
    /// </summary>
    /// <param name="strMessageId">Message id</param>
    /// <returns>Contend of message</returns>
    public static string GetMessage(string strMessageId)
    {
      //Đường dẫn tới message
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPath = string.Empty;
      if (ConstantClass.CULTURE.Name == "vi-VN")
      {
        strPath = string.Format(@"{0}\Utility\VNMessages.txt", strStartupPath);
      }
      else
      {
        strPath = string.Format(@"{0}\Utility\Messages.txt", strStartupPath);
      }

      if (File.Exists(strPath))
      {
        StreamReader srReadLine = new StreamReader(strPath, System.Text.Encoding.GetEncoding("Shift-JIS"));
        srReadLine.BaseStream.Seek(0, SeekOrigin.Begin);
        while (true)
        {
          string str = srReadLine.ReadLine();
          if (str == null)
          {
            break;
          }
          try
          {
            string[] temp = str.Split('=');
            string id = temp[0].Replace("var", "").Trim();
            if (id == strMessageId)
            {
              string strMessage = str.Substring(str.IndexOf("=") + 1);
              srReadLine.Close();
              return strMessage.Trim();
            }
          }
          catch
          {
          }
        }
        srReadLine.Close();
      }
      return string.Empty;
    }

    /// <summary>
    /// Get new EnquiryNo with format: prefix + YY + MM + '- ' + xxxx
    /// </summary>
    /// <param name="prefix">Prefix</param>
    /// <returns>New Enquiry No</returns>
    public static string GetNewEnquiryNo(string prefix)
    {
      string commandText = string.Format(@"SELECT dbo.FPLNGetNewEnquiryNo(@Prefix)");
      DBParameter[] param = new DBParameter[] { new DBParameter("@Prefix", DbType.AnsiString, 16, prefix) };
      return DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandText, param).ToString();
    }
    public static DataSet GetExcelToDataSet(string strPathFile, string strSql)
    {
      OleDbConnection oConnection = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + strPathFile + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;MAXSCANROWS=0;\"");
      string strCommandText = strSql;
      OleDbDataAdapter adp = new OleDbDataAdapter(strCommandText, oConnection);
      DataSet dsXLS = new DataSet();
      try
      {
        adp.Fill(dsXLS);
        return dsXLS;
      }
      catch
      {
        return dsXLS;
      }
    }
    public static DataSet GetExcelToDataSetVersion2(string strPathFile, string strSql)
    {
      OleDbConnection oConnection = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + strPathFile + ";Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1;MAXSCANROWS=0;ImportMixedTypes=Text\"");
      string strCommandText = strSql;
      OleDbDataAdapter adp = new OleDbDataAdapter(strCommandText, oConnection);
      DataSet dsXLS = new DataSet();
      DataSet dsReturn = new DataSet();
      try
      {
        adp.Fill(dsXLS);

        if (dsXLS.Tables[0].Rows.Count > 0)
        {
          DataTable dtReturn = new DataTable();
          for (int c = 0; c < dsXLS.Tables[0].Columns.Count; c++)
          {
            dtReturn.Columns.Add(dsXLS.Tables[0].Rows[0][c].ToString());
          }
          for (int r = 1; r < dsXLS.Tables[0].Rows.Count; r++)
          {
            DataRow dr = dtReturn.NewRow();
            for (int i = 0; i < dsXLS.Tables[0].Columns.Count; i++)
            {
              dr[i] = dsXLS.Tables[0].Rows[r][i];
            }
            dtReturn.Rows.Add(dr);
          }

          dsReturn.Tables.Add(dtReturn);
        }
        return dsReturn;
      }
      catch
      {
        return dsXLS;
      }
    }

    /// <summary>
    /// Get Data From Excel File
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="nSheet"></param>
    /// <param name="areaData"></param>
    /// <returns></returns>
    public static DataTable GetDataFromExcel(string fileName, string sheetName, string areaData)
    {
      try
      {
        //object missing = System.Reflection.Missing.Value;
        //Excel.ApplicationClass xl = new Excel.ApplicationClass();
        //Excel.Workbook xlBook;
        //Excel.Sheets xlSheets;
        //Excel.Worksheet xlSheet;

        //xlBook = (Excel.Workbook)xl.Workbooks.Open(fileName, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
        //xlSheets = xlBook.Worksheets;
        //xlSheet = (Excel.Worksheet)xlSheets.get_Item(1);
        //xlBook.Close(null, null, null);

        OleDbConnection connection = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + fileName + ";Extended Properties=Excel 8.0;");
        string commandText = string.Format(@"SELECT * FROM [{0}${1}]", sheetName, areaData);
        OleDbDataAdapter adp = new OleDbDataAdapter(commandText, connection);
        System.Data.DataTable dtXLS = new System.Data.DataTable();
        adp.Fill(dtXLS);
        connection.Close();
        if (dtXLS == null)
        {
          return null;
        }
        return dtXLS;
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0069");
        return null;
      }
    }
    /// <summary>
    /// Get new SaleOrder No with format: prefix + YY + MM + '- ' + xxxx
    /// </summary>
    /// <param name="prefix">Prefix</param>
    /// <returns>New SaleOrderNo</returns>
    public static string GetNewSaleOrderNo(string prefix)
    {
      string commandText = string.Format(@"SELECT dbo.FPLNGetNewSaleNo(@Prefix)");
      DBParameter[] param = new DBParameter[] { new DBParameter("@Prefix", DbType.AnsiString, 16, prefix) };
      return DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandText, param).ToString();
    }

    public static string GetNewRequestCode(string prefix)
    {
      string commandText = string.Format(@"SELECT dbo.FWIPNewRequestCode(@Prefix)");
      DBParameter[] param = new DBParameter[] { new DBParameter("@Prefix", DbType.AnsiString, 16, prefix) };
      return DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandText, param).ToString();
    }

    public static string GetNewISSCode(string prefix)
    {
      string commandText = string.Format(@"SELECT dbo.FWIPNewISSCode(@Prefix)");
      DBParameter[] param = new DBParameter[] { new DBParameter("@Prefix", DbType.AnsiString, 16, prefix) };
      return DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandText, param).ToString();
    }

    public static string GetNewRECCode(string prefix)
    {
      string commandText = string.Format(@"SELECT dbo.FWIPReceivingNew(@Prefix)");
      DBParameter[] param = new DBParameter[] { new DBParameter("@Prefix", DbType.AnsiString, 16, prefix) };
      return DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandText, param).ToString();
    }


    /// <summary>
    /// Convert a number in Milimet to Inch
    /// </summary>
    /// <param name="value">Number in milimet</param>
    /// <returns>Number in Inch</returns>
    public static string ConverMilimetToInch(object value)
    {
      if (DBConvert.ParseDouble(value) == double.MinValue)
      {
        return string.Empty;
      }
      double result = DBConvert.ParseDouble(value) * 0.0393700787;
      int integerPart = (int)result;
      double decimalPart = result - integerPart;
      if (decimalPart < 0.125)
      {
        return DBConvert.ParseString(integerPart) + "''";
      }
      else if (decimalPart < 0.375)
      {
        return DBConvert.ParseString(integerPart) + " " + "1/4''";
      }
      else if (decimalPart < 0.635)
      {
        return DBConvert.ParseString(integerPart) + " " + "1/2''";
      }
      else if (decimalPart < 0.875)
      {
        return DBConvert.ParseString(integerPart) + " " + "3/4''";
      }
      return DBConvert.ParseString(integerPart + 1) + "''";
    }

    public static string BoDau(string accented)
    {
      Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
      string strFormD = accented.Normalize(System.Text.NormalizationForm.FormD);
      return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
    }

    public static string ToUpperFirstLetterEachWord(string source)
    {
      string result = string.Empty;
      if (string.IsNullOrEmpty(source))
      {
        return result;
      }
      // convert to char array of the string
      string[] arrString = source.ToLower().Trim().Split(' ');
      // upper case the first char
      for (int i = 0; i < arrString.Length; i++)
      {
        if (arrString[i].Length > 0)
        {
          if (result.Length > 0)
          {
            result += " ";
          }
          result += string.Format("{0}{1}", arrString[i].Substring(0, 1).ToUpper(), arrString[i].Substring(1, arrString[i].Length - 1));
        }
      }
      return result;
    }


    public static DialogResult SaveBeforeClosing()
    {
      return MessageBox.Show(GetMessage("MSG0008"), "Save data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
    }

    #region GetImagePath
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    public static string GetImagePathByPid(int pid)
    {
      string commandText = string.Format("SELECT Path FROM TBLBOMImagePath WHERE Pid = '{0}'", pid);
      return DataBaseAccess.ExecuteScalarCommandText(commandText).ToString();
    }
    public static string GetBassComponentImage(string componentCode)
    {
      return string.Format(@"{0}\{1}.wmf", GetImagePathByPid(2), componentCode);
    }
    public static string BOMGetItemImage(string itemCode, int revision)
    {
      string imagePath;
      if (revision == 0)
      {
        imagePath = string.Format(@"{0}\{1}.jpg", GetImagePathByPid(4), itemCode);
      }
      else
      {
        imagePath = string.Format(@"{0}\{1}-{2}.jpg", GetImagePathByPid(1), itemCode, revision.ToString().PadLeft(2, '0'));
      }
      return imagePath;
    }

    public static string BOMGetBoxType(string Boxtype)
    {
      string imagePath;
      imagePath = string.Format(@"{0}\{1}.jpg", GetImagePathByPid(19), Boxtype);
      return imagePath;
    }

    public static string BOMGetItemImageSubCon(string itemCode, int revision)
    {
      string imagePath;
      if (revision == 0)
      {
        imagePath = string.Format(@"{0}\{1}.jpg", GetImagePathByPid(14), itemCode);
      }
      else
      {
        imagePath = string.Format(@"{0}\{1}-{2}.jpg", GetImagePathByPid(15), itemCode, revision.ToString().PadLeft(2, '0'));
      }
      return imagePath;
    }

    /// <summary>
    /// Get Item Instruction Image Path
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="revision"></param>
    /// <returns></returns>
    public static string BOMGetItemInstructionImage(string itemCode, int revision)
    {
      string path = GetImagePathByPid(9);
      return string.Format(@"{0}\{1}-{2}.wmf", path, itemCode, revision.ToString().PadLeft(2, '0'));
    }
    public static string BOMGetItemComponentImage(string componentCode)
    {
      string fileName = string.Format(@"{0}\{1}.wmf", GetImagePathByPid(2), componentCode);
      if (File.Exists(fileName))
      {
        return fileName;
      }
      return string.Format(@"{0}\{1}.jpg", GetImagePathByPid(2), componentCode);
    }
    public static string GetNewItemImage(string itemCode)
    {
      string fileName = string.Format(@"{0}\{1}.wmf", GetImagePathByPid(24), itemCode);
      if (File.Exists(fileName))
      {
        return fileName;
      }
      return string.Format(@"{0}\{1}.jpg", GetImagePathByPid(24), itemCode);
    }
    public static string BOMGetCarcassImage(string carcassCode)
    {
      string fileName = string.Format(@"{0}\{1}.wmf", GetImagePathByPid(3), carcassCode);
      if (File.Exists(fileName))
      {
        return fileName;
      }
      return string.Format(@"{0}\{1}.jpg", GetImagePathByPid(3), carcassCode);
    }

    public static string BOMGetCarcassSubConImage(string carcassCode)
    {
      string fileName = string.Format(@"{0}\{1}.wmf", GetImagePathByPid(16), carcassCode);
      if (File.Exists(fileName))
      {
        return fileName;
      }
      return string.Format(@"{0}\{1}.jpg", GetImagePathByPid(3), carcassCode);
    }

    public static string BOMGetPackageImage(string packageCode)
    {
      return string.Format(@"{0}\{1}.jpg", GetImagePathByPid(1), packageCode);
    }
    public static string BOMGetCarcassComponentImage(string carcassCode, string componentCode)
    {
      return string.Format(@"{0}\Components\{1}\{1}-{2}.wmf", GetImagePathByPid(3), carcassCode, componentCode);
    }
    public static string RDDGetItemImage(string itemCode)
    {
      return string.Format(@"{0}\{1}.jpg", GetImagePathByPid(4), itemCode);
    }
    public static string RDDGetItemComponentImage(string itemCode, string componentCode)
    {
      return string.Format(@"{0}\{1}.wmf", GetImagePathByPid(5), componentCode);
    }
    public static string RDDGetCarcassImage(string carcassCode)
    {
      return string.Format(@"{0}\{1}.wmf", GetImagePathByPid(6), carcassCode);
    }
    public static string BOMGetProfileImage(string profileCode)
    {
      return string.Format(@"{0}\{1}.wmf", GetImagePathByPid(7), profileCode);
    }
    public static string GNRGetMaterialImage(string materialCode)
    {
      return string.Format(@"{0}\{1}.jpg", GetImagePathByPid(10), materialCode);
    }
    public static string PLNGetIDVeneerImage(string idVeneer)
    {
      return string.Format(@"{0}\{1}.jpg", GetImagePathByPid(11), idVeneer);
    }

    public static string FOUGetImagePath(string componentCode, int componentType, int revision)
    {
      if (componentCode.Length == 0)
      {
        return string.Empty;
      }
      string commandText = "SELECT Path FROM TBLBOMImagePath WHERE Pid = 0";
      object result = DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandText);
      string path = (result != null) ? result.ToString() : string.Empty;
      string fileName = componentCode;
      if (componentType == 2)
      {
        fileName = string.Format(@"{0}-{1}.wmf", componentCode, revision.ToString().PadLeft(2, '0'));
      }
      else
      {
        fileName = string.Format(@"{0}.wmf", componentCode);
      }
      return string.Format(@"{0}{1}", path, fileName);
    }

    /// <summary>
    /// Get Icon for Item Kind
    /// </summary>
    /// <param name="itemKind">0: Discontinue, 1: Special Order, 2: US_UK Quick Ship, 3: USQS, 4: UKQS, 5: Standard</param>
    /// <returns></returns>
    public static byte[] GetItemKindIcon(int itemKind)
    {
      switch (itemKind)
      {
        case 0:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.D_outline);
        case 1:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.SP_outline);
        case 2:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.QS_outline);
        case 3:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.USQS_outline);
        case 4:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.UKQS_outline);
        case 5:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_outline);
        default:
          break;
      }
      return null;
    }

    /// <summary>
    /// Get Icon for Item Kind
    /// </summary>
    /// <param name="itemKind">
    /// 0. Custom (just for Other)
    /// 1. QuickShip
    /// 2. Special
    /// 3. Discontinue
    /// 4. OnHold
    /// 5. Special Discontinue
    /// 6. Standard Discontinue
    /// 7. Standard Special
    /// 8. Standard
    /// 9. Standard Retail
    /// 10.Standard Retail Discontinue
    /// 11.Standard Retail Special
    /// </param>
    /// <returns></returns>
    public static byte[] GetNewItemKindIcon(int itemKind)
    {
      switch (itemKind)
      {
        case 0:
          return null;
        case 1:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.QS_outline);
        case 2:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.SP_outline);
        case 3:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.D_outline);
        case 4:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.OnHold_outline);
        case 5:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.SP_D_outline);
        case 6:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_D_outline);
        case 7:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_SP_outline);
        case 8:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_outline);
        case 9:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_Retail);
        case 10:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_Retail_D);
        case 11:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_Retail_SP);
        default:
          break;
      }
      return null;
    }
    public static Image GetEmailIcon(int Kind)
    {
      switch (Kind)
      {
        case 0:
          return null;
        case 1:
          return (Image)DaiCo.Shared.IconResource.openmail;
        case 2:
          return (Image)DaiCo.Shared.IconResource.small_mail_unread;
        case 3:
          return (Image)DaiCo.Shared.IconResource.mail_attachment_md;
        case 4:
          return (Image)DaiCo.Shared.IconResource.Juliewiens_Christmas_Gift.ToBitmap();
        default:
          break;
      }
      return null;
    }

    /// <summary>
    /// Get Item Kind Icon for local
    /// </summary>
    /// <param name="itemKind">
    /// 0. Custom (just for Other)
    /// 1. QuickShip
    /// 2. Special
    /// 3. Discontinue
    /// 4. OnHold
    /// 5. Special Discontinue
    /// 6. Standard Discontinue
    /// 7. Standard Special
    /// 8. Standard
    /// </param>
    /// <returns></returns>
    public static byte[] GetLocalItemKindIcon(int itemKind, long customerPid)
    {
      switch (itemKind)
      {
        case 0:
          return null;
        case 1:
          if (customerPid == 12)
          {
            return ImageToByteArray((Image)DaiCo.Shared.IconResource.USQS_outline);
          }
          else if (customerPid == 11)
          {
            return ImageToByteArray((Image)DaiCo.Shared.IconResource.UKQS_outline);
          }
          else
          {
            return ImageToByteArray((Image)DaiCo.Shared.IconResource.QS_outline);
          }
        case 2:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.SP_outline);
        case 3:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.D_outline);
        case 4:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.OnHold_outline);
        case 5:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.SP_D_outline);
        case 6:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_D_outline);
        case 7:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_SP_outline);
        case 8:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_outline);
        case 9:
          return ImageToByteArray((Image)DaiCo.Shared.IconResource.ST_Retail);
        default:
          break;
      }
      return null;
    }
    #endregion GetImagePath

    #region Check
    public static DataTable CloneTable(DataTable dtSource)
    {
      DataTable dtCopy = new DataTable();
      dtCopy = dtSource.Clone();
      dtCopy.Merge(dtSource);
      return dtCopy;
    }

    public static bool CheckBOMComponentCode(string code)
    {
      if (code.Trim().Trim().Length == 0)
      {
        return true;
      }
      string strCommandText = " SELECT dbo.FBOMCheckComponent(@Code)";
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@Code", DbType.AnsiString, 50, code) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) == 1);
    }

    public static bool CheckBOMMaterialCode(string code, int group)
    {
      if (code.Trim().Trim().Length == 0)
      {
        return true;
      }
      string strCommandText = " SELECT dbo.FBOMCheckMaterialCode(@Code, @Group)";
      DBParameter[] arrInput = new DBParameter[2];
      arrInput[0] = new DBParameter("@Code", DbType.AnsiString, 50, code);
      arrInput[1] = new DBParameter("@Group", DbType.Int32, group);
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) == 1);
    }

    public static bool CheckBOMBoxName(string code)
    {
      if (code.Trim().Trim().Length == 0)
      {
        return true;
      }
      string strCommandText = " SELECT dbo.FBOMCheckMBoxTypeName(@Code)";
      DBParameter[] arrInput = new DBParameter[1];
      arrInput[0] = new DBParameter("@Code", DbType.AnsiString, 50, code);
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) == 1);
    }

    public static bool CheckProfile(string profileCode)
    {
      if (profileCode.Trim().Length == 0)
      {
        return true;
      }
      string strCommandText = " SELECT dbo.FBOMCheckProfile(@ProfileCode)";
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@ProfileCode", DbType.AnsiString, 16, profileCode.Replace("'", "''")) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) == 1);
    }
    public static bool CheckWorkStation(string station)
    {
      if (station.Trim().Length == 0)
      {
        return true;
      }
      string strCommandText = " SELECT dbo.FBOMCheckWorkStation(@Station)";
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@Station", DbType.String, 256, station.Replace("'", "''")) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) == 1);
    }

    public static bool CheckWorkArea(string workAreaCode)
    {
      if (workAreaCode.Trim().Length == 0)
      {
        return true;
      }
      string strCommandText = " SELECT dbo.FBOMCheckWorkArea(@WorkAreaCode)";
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@WorkAreaCode", DbType.String, 256, workAreaCode.Replace("'", "''")) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) == 1);
    }

    public static bool CheckMachineGroup(string machineGroupENName)
    {
      if (machineGroupENName.Trim().Length == 0)
      {
        return true;
      }
      string strCommandText = " SELECT dbo.FBOMCheckMachineGroupENName(@MachineGroupENName)";
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@MachineGroupENName", DbType.String, 256, machineGroupENName.Trim().Replace("'", "''")) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) == 1);
    }

    public static bool CheckTeam(string workAreaCode, string teamCode)
    {
      if (teamCode.Trim().Length == 0)
      {
        return true;
      }
      string strCommandText = " SELECT dbo.FBOMCheckTeam(@WorkAreaCode, @TeamCode)";
      DBParameter[] arrInput = new DBParameter[2];
      arrInput[0] = new DBParameter("@WorkAreaCode", DbType.AnsiString, 50, workAreaCode);
      arrInput[1] = new DBParameter("@TeamCode", DbType.AnsiString, 50, teamCode);
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) == 1);
    }

    public static bool CheckSummary(int whKind)
    {
      DateTime dateNow = DBConvert.ParseDateTime(DataBaseAccess.ExecuteScalarCommandText("SELECT CONVERT(varchar, GETDATE(), 103)").ToString(), ConstantClass.FORMAT_DATETIME);
      DateTime firstDate = new DateTime(dateNow.Year, dateNow.Month, 1);
      int result = DateTime.Compare(firstDate, dateNow);

      if (result <= 0)
      {
        int preMonth = 0;
        int preYear = 0;
        if (dateNow.Month == 1)
        {
          preMonth = 12;
          preYear = dateNow.Year - 1;
        }
        else
        {
          preMonth = dateNow.Month - 1;
          preYear = dateNow.Year;
        }

        string commandText = "SELECT COUNT(*) FROM TblFOUMonthlyClosingHistory WHERE Month = " + preMonth + " AND Year = " + preYear + " AND WHKind = " + whKind;
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if ((dtCheck == null) || (dtCheck != null && DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0))
        {
          WindowUtinity.ShowMessageError("ERR0303", preMonth.ToString(), preYear.ToString());
          return false;
        }
      }
      return true;
    }
    #endregion Check

    #region Unlock
    public static bool UnlockRDDItemInfo(string itemCode)
    {
      bool success = true;
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DataBaseAccess.SearchStoreProcedure("spRDDItemInfo_Unlock", inputParam, outputParam);
      if (DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
      {
        success = false;
      }
      return success;
    }
    public static void UnlockRDDCarcassInfo(string carcassCode)
    {
      string commandText = string.Format(@"UPDATE TblRDDCarcassInfo SET Confirm = 0 WHERE CarcassCode = '{0}'", carcassCode);
      DataBaseAccess.ExecuteCommandText(commandText);
    }
    public static void UnlockBOMFinishingInfo(string finishingCode)
    {
      string commandText = string.Format(@"UPDATE TblBOMFinishingInfo SET Confirm = 0 WHERE FinCode = '{0}'", finishingCode);
      DataBaseAccess.ExecuteCommandText(commandText);
    }
    public static void UnlockBOMProfileInfo(long profilePid)
    {
      string commandText = string.Format(@"UPDATE TblBOMProfileDetail SET Confirm = 0 WHERE Pid = '{0}'", profilePid);
      DataBaseAccess.ExecuteCommandText(commandText);
    }
    public static void UnlockBOMComponentInfo(long pid)
    {
      string commandText = string.Format(@"UPDATE TblBOMComponentInfo SET Confirm = 0 WHERE Pid = {0}", pid);
      DataBaseAccess.ExecuteCommandText(commandText);
    }
    public static void UnlockBOMCarcass(string carcassCode)
    {
      string commandText = string.Format(@"UPDATE TblBOMCarcass SET Confirm = 0 WHERE CarcassCode = '{0}'", carcassCode);
      DataBaseAccess.ExecuteCommandText(commandText);
    }
    public static void UnlockBOMSupportInfo(string supCode)
    {
      string commandText = string.Format(@"UPDATE TblBOMSupportInfo SET Confirm = 0 WHERE SupCode = '{0}'", supCode);
      DataBaseAccess.ExecuteCommandText(commandText);
    }
    public static void UnlockACCSaleOrder(long pid)
    {
      string commandText = string.Format(@"UPDATE TblPLNSaleOrder SET Status = 1 WHERE Pid = '{0}'", pid);
      DataBaseAccess.ExecuteCommandText(commandText);
    }
    public static void UnlockPLNSaleOrder(long pid)
    {
      string commandText = string.Format(@"UPDATE TblPLNSaleOrder SET Status = 2 WHERE Pid = '{0}'", pid);
      DataBaseAccess.ExecuteCommandText(commandText);
    }
    public static void UnlockPLNEnquiry(long pid)
    {
      string commandText = string.Format(@"UPDATE TblPLNEnquiry SET Confirm = 0 WHERE Pid = '{0}'", pid);
      DataBaseAccess.ExecuteCommandText(commandText);
    }
    public static void UnlockPLNWorkOrder(long pid)
    {
      //haanh add them
      string query = "select count(WorkOrderPid) from TblWIPWorkAreaStore where WorkOrderPid = " + pid;
      int count = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(query));
      if (count == 0)
      {
        string commandText = string.Format(@"UPDATE TblPLNWorkOrder SET Confirm = 0 WHERE Pid = '{0}'", pid);
        DataBaseAccess.ExecuteCommandText(commandText);
        WindowUtinity.ShowMessageSuccess("MSG0023");
      }
      else
      {
        string message = string.Format(FunctionUtility.GetMessage("ERR0085"), "Work Order");
        WindowUtinity.ShowMessageErrorFromText(message);
      }
    }
    #endregion Unlock

    public static DialogResult PromptDeleteMessage(int countRows)
    {
      string message = (countRows > 1) ? "rows" : "row";
      message = string.Format("You have selected {0} {1} for deletion.\n Choose Yes to delete the {1} or No to exit.", countRows, message);
      return MessageBox.Show(message, "Delete Rows", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    }

    public static string GetNewToolCode(string prefix)
    {
      string commandText = string.Format(@"SELECT dbo.FPLNGetNewToolCode(@Prefix)");
      DBParameter[] param = new DBParameter[] { new DBParameter("@Prefix", DbType.AnsiString, 16, prefix) };
      return DataBaseAccess.ExecuteScalarCommandText(commandText, param).ToString();
    }

    /// <summary>
    /// view costing detail of item
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="viewType">0: Default, 1: Make local, 2: Contract out</param>
    public static void ViewItemCosting_Purchase(string itemCode, int priceType, params int[] viewType)
    {
      try
      {
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        DBParameter[] outParam = new DBParameter[3];
        outParam[0] = new DBParameter("@ExchangeRate", DbType.Double, double.MinValue);
        outParam[1] = new DBParameter("@FOH", DbType.Double, double.MinValue);
        outParam[2] = new DBParameter("@Profit", DbType.Double, double.MinValue);
        // Calculate BOM Price
        double totalVNDPrice = 0;
        double totalUSDPrice = 0;
        // End Calculate BOM Price
        DBParameter[] inputParamCosting = new DBParameter[2];
        inputParamCosting[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        if (viewType.Length > 0 && viewType[0] != int.MinValue)
        {
          inputParamCosting[1] = new DBParameter("@ViewType", DbType.Int32, viewType[0]);
        }

        DBParameter[] outputParamCosting = new DBParameter[11];
        outputParamCosting[0] = new DBParameter("@ExchangeRate", DbType.Double, double.MinValue);
        outputParamCosting[1] = new DBParameter("@FOH", DbType.Double, double.MinValue);
        outputParamCosting[2] = new DBParameter("@Profit", DbType.Double, double.MinValue);
        outputParamCosting[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, string.Empty);
        outputParamCosting[4] = new DBParameter("@ContractOut", DbType.Int32, int.MinValue);
        outputParamCosting[5] = new DBParameter("@CountPur", DbType.Int32, int.MinValue);
        outputParamCosting[6] = new DBParameter("@ContractOutHW", DbType.Int32, int.MinValue);
        outputParamCosting[7] = new DBParameter("@ExFactoryContractOutVND", DbType.Double, double.MinValue);
        outputParamCosting[8] = new DBParameter("@ExFactoryContractOutUSD", DbType.Double, double.MinValue);
        outputParamCosting[9] = new DBParameter("@ExFactoryInhouseVND", DbType.Double, double.MinValue);
        outputParamCosting[10] = new DBParameter("@ExFactoryInhouseUSD", DbType.Double, double.MinValue);


        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDItemCostingInformation_New", 3600, inputParamCosting, outputParamCosting);
        if (dsSource != null && dsSource.Tables.Count > 1)
        {
          dsSource.Tables[0].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dsSource.Tables[0].Columns.Add("CheckImage", typeof(String));
          for (int i = 0; i < dsSource.Tables[0].Rows.Count; i++)
          {
            dsSource.Tables[0].Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImageToByteArrayWithFormat(@dsSource.Tables[0].Rows[i]["img"].ToString(), 380, 1.74, "JPG");
            dsSource.Tables[0].Rows[i]["CheckImage"] = dsSource.Tables[0].Rows[i]["Image"].ToString();
          }
          dsSource.Tables[1].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dsSource.Tables[1].Columns.Add("CheckImage", typeof(String));
          for (int i = 0; i < dsSource.Tables[1].Rows.Count; i++)
          {
            dsSource.Tables[1].Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray_Always(@dsSource.Tables[1].Rows[i]["Picture"].ToString());
            dsSource.Tables[1].Rows[i]["CheckImage"] = dsSource.Tables[1].Rows[i]["Image"].ToString();
          }

          dsCSDItemCostPriceSummary ds = new dsCSDItemCostPriceSummary();
          ds.Tables["dtItemCostMaster"].Merge(dsSource.Tables[0]);
          ds.Tables["dtItemCostDetail"].Merge(dsSource.Tables[1]);
          ds.Tables["dtACTSalePrice"].Merge(dsSource.Tables[3]);
          ds.Tables["dtTotal"].Merge(dsSource.Tables[7]);
          ds.Tables["dtCostSummary"].Merge(dsSource.Tables[4]);
          ds.Tables["dtPurchaseCost"].Merge(dsSource.Tables[5]);
          ds.Tables["dtMaterialGroupSummary"].Merge(dsSource.Tables[6]);
          cptCSDItemCosrPriceSummary cptItemCostPrice = new cptCSDItemCosrPriceSummary();

          cptItemCostPrice.SetDataSource(ds);
          double dExchange = DBConvert.ParseDouble(outputParamCosting[0].Value.ToString());
          double dFOH = DBConvert.ParseDouble(outputParamCosting[1].Value.ToString());
          double dProfit = DBConvert.ParseDouble(outputParamCosting[2].Value.ToString());
          string remark = outputParamCosting[3].Value.ToString();
          int contractOut = DBConvert.ParseInt(outputParamCosting[4].Value);
          int countPur = DBConvert.ParseInt(outputParamCosting[5].Value);
          double exFactoryContractOutVND = DBConvert.ParseDouble(outputParamCosting[7].Value);
          double exFactoryContractOutUSD = DBConvert.ParseDouble(outputParamCosting[8].Value);
          double exFactoryInhouseVND = DBConvert.ParseDouble(outputParamCosting[9].Value);
          double exFactoryInhouseUSD = DBConvert.ParseDouble(outputParamCosting[10].Value);
          double differentVND = exFactoryContractOutVND - exFactoryInhouseVND;
          double differentUSD = exFactoryContractOutUSD - exFactoryInhouseUSD;
          double percent1 = double.MinValue;
          string percent = string.Empty;
          if (exFactoryInhouseVND > 0)
          {
            percent1 = Math.Round(differentVND / exFactoryInhouseVND * 100, 2);
            percent = percent1.ToString() + " %";
          }
          else
          {
            percent = "";
          }
          cptItemCostPrice.SetParameterValue("ExchangeRate", dExchange);
          cptItemCostPrice.SetParameterValue("FOH", dFOH);
          cptItemCostPrice.SetParameterValue("Profit", dProfit);
          cptItemCostPrice.SetParameterValue("TotalBOMPrice_VND", totalVNDPrice);
          cptItemCostPrice.SetParameterValue("TotalBOMPrice_USD", totalUSDPrice);
          cptItemCostPrice.SetParameterValue("User", SharedObject.UserInfo.EmpName);
          if (exFactoryContractOutVND > 0)
          {
            cptItemCostPrice.SetParameterValue("ExFactoryContractOutVND", exFactoryContractOutVND);
          }
          else
          {
            cptItemCostPrice.SetParameterValue("ExFactoryContractOutVND", 0);
          }

          if (exFactoryContractOutUSD > 0)
          {
            cptItemCostPrice.SetParameterValue("ExFactoryContractOutUSD", exFactoryContractOutUSD);
          }
          else
          {
            cptItemCostPrice.SetParameterValue("ExFactoryContractOutUSD", 0);
          }

          if (exFactoryInhouseVND > 0)
          {
            cptItemCostPrice.SetParameterValue("ExFactoryInhouseVND", exFactoryInhouseVND);
          }
          else
          {
            cptItemCostPrice.SetParameterValue("ExFactoryInhouseVND", 0);
          }

          if (exFactoryInhouseUSD > 0)
          {
            cptItemCostPrice.SetParameterValue("ExFactoryInhouseUSD", exFactoryInhouseUSD);
          }
          else
          {
            cptItemCostPrice.SetParameterValue("ExFactoryInhouseUSD", 0);
          }

          if (exFactoryContractOutVND != double.MinValue || exFactoryInhouseVND != double.MinValue)
          {
            cptItemCostPrice.SetParameterValue("DifferentVND", differentVND);
          }
          else
          {
            cptItemCostPrice.SetParameterValue("DifferentVND", 0);
          }

          if (exFactoryContractOutUSD != double.MinValue || exFactoryInhouseUSD != double.MinValue)
          {
            cptItemCostPrice.SetParameterValue("DifferentUSD", differentUSD);
          }
          else
          {
            cptItemCostPrice.SetParameterValue("DifferentUSD", 0);
          }

          if (percent.ToString().Trim().Length > 0)
          {
            cptItemCostPrice.SetParameterValue("Percent", percent);
          }
          else
          {
            cptItemCostPrice.SetParameterValue("Percent", percent);
          }


          //Carcass Summary
          DataTable dtCarcassSummary = dsSource.Tables[2];
          if (dtCarcassSummary.Rows.Count > 0)
          {
            cptItemCostPrice.SetParameterValue("TotalLabor", dtCarcassSummary.Rows[0]["TotalLabor"]);
            cptItemCostPrice.SetParameterValue("TotalMainMaterial", dtCarcassSummary.Rows[0]["TotalMainMaterial"]);
            cptItemCostPrice.SetParameterValue("OtherMaterial", dtCarcassSummary.Rows[0]["OtherMaterial"]);
            cptItemCostPrice.SetParameterValue("SubconNetPrice", dtCarcassSummary.Rows[0]["SubconNetPrice"]);
            cptItemCostPrice.SetParameterValue("MaterialSupplied", dtCarcassSummary.Rows[0]["MaterialSupplied"]);
          }

          // New price format (26/8/2016)
          cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = true;
          cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = true;

          if (exFactoryContractOutVND > 0)
          {
            cptItemCostPrice.ReportFooterSection1.SectionFormat.EnableSuppress = false;
            cptItemCostPrice.ReportFooterSection7.SectionFormat.EnableSuppress = true;
          }
          else
          {
            cptItemCostPrice.ReportFooterSection1.SectionFormat.EnableSuppress = true;
            cptItemCostPrice.ReportFooterSection7.SectionFormat.EnableSuppress = false;
          }

          //hide/show purchase cost
          if (countPur > 0)
          {
            cptItemCostPrice.PurchaseCost.SectionFormat.EnableSuppress = false;
          }
          else
          {
            cptItemCostPrice.PurchaseCost.SectionFormat.EnableSuppress = true;
          }

          //Hide/Show Sale Price
          if (priceType == 0)
          {
            cptItemCostPrice.SalePrice.SectionFormat.EnableSuppress = true;
          }
          else
          {
            cptItemCostPrice.SalePrice.SectionFormat.EnableSuppress = false;
          }

          View_Report frm = new View_Report(cptItemCostPrice);
          frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
        }
      }
      catch { }
    }

    /// <summary>
    /// view costing detail of item
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="viewType">0: Default, 1: Make local, 2: Contract out</param>
    public static void ViewItemCosting(string itemCode, int priceType, params int[] viewType)
    {
      try
      {
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        DBParameter[] outParam = new DBParameter[3];
        outParam[0] = new DBParameter("@ExchangeRate", DbType.Double, double.MinValue);
        outParam[1] = new DBParameter("@FOH", DbType.Double, double.MinValue);
        outParam[2] = new DBParameter("@Profit", DbType.Double, double.MinValue);
        // Calculate BOM Price
        //DataSet dsBOMPrice = DataBaseAccess.SearchStoreProcedure("spCSDItemCosting_Manufacture", 3600, inputParam, outParam);
        double totalVNDPrice = 0;
        double totalUSDPrice = 0;
        //double factoryFOH = 1;
        //double fatoryProfit = 1;
        //if (dsBOMPrice != null && dsBOMPrice.Tables.Count > 0)
        //{
        //  factoryFOH = DBConvert.ParseDouble(outParam[1].Value.ToString());
        //  fatoryProfit = DBConvert.ParseDouble(outParam[2].Value.ToString());

        //  foreach (DataRow priceRow in dsBOMPrice.Tables[1].Rows)
        //  {
        //    double vndPrice = DBConvert.ParseDouble(priceRow["Amount"].ToString());
        //    if (vndPrice > 0)
        //    {
        //      totalVNDPrice += vndPrice;
        //    }
        //    double usdPrice = DBConvert.ParseDouble(priceRow["USD"].ToString());
        //    if (usdPrice > 0)
        //    {
        //      totalUSDPrice += usdPrice;
        //    }
        //  }
        //}
        //totalVNDPrice = totalVNDPrice + ((totalVNDPrice * factoryFOH) / 100) + (((totalVNDPrice + ((totalVNDPrice * factoryFOH) / 100)) * fatoryProfit) / 100);
        //totalUSDPrice = totalUSDPrice + ((totalUSDPrice * factoryFOH) / 100) + (((totalUSDPrice + ((totalUSDPrice * factoryFOH) / 100)) * fatoryProfit) / 100);
        // End Calculate BOM Price
        DBParameter[] inputParamCosting = new DBParameter[2];
        inputParamCosting[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        if (viewType.Length > 0 && viewType[0] != int.MinValue)
        {
          inputParamCosting[1] = new DBParameter("@ViewType", DbType.Int32, viewType[0]);
        }

        DBParameter[] outputParamCosting = new DBParameter[5];
        outputParamCosting[0] = new DBParameter("@ExchangeRate", DbType.Double, double.MinValue);
        outputParamCosting[1] = new DBParameter("@FOH", DbType.Double, double.MinValue);
        outputParamCosting[2] = new DBParameter("@Profit", DbType.Double, double.MinValue);
        outputParamCosting[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, string.Empty);
        outputParamCosting[4] = new DBParameter("@ContractOut", DbType.Int32, int.MinValue);

        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDItemCostingInformation", 3600, inputParamCosting, outputParamCosting);
        if (dsSource != null && dsSource.Tables.Count > 1)
        {
          dsSource.Tables[0].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dsSource.Tables[0].Columns.Add("CheckImage", typeof(String));
          for (int i = 0; i < dsSource.Tables[0].Rows.Count; i++)
          {
            dsSource.Tables[0].Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImageToByteArrayWithFormat(@dsSource.Tables[0].Rows[i]["img"].ToString(), 380, 1.74, "JPG");
            dsSource.Tables[0].Rows[i]["CheckImage"] = dsSource.Tables[0].Rows[i]["Image"].ToString();
          }
          dsSource.Tables[1].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dsSource.Tables[1].Columns.Add("CheckImage", typeof(String));
          for (int i = 0; i < dsSource.Tables[1].Rows.Count; i++)
          {
            dsSource.Tables[1].Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray_Always(@dsSource.Tables[1].Rows[i]["Picture"].ToString());
            dsSource.Tables[1].Rows[i]["CheckImage"] = dsSource.Tables[1].Rows[i]["Image"].ToString();
          }

          dsCSDItemCostPrice ds = new dsCSDItemCostPrice();
          ds.Tables["dtItemCostMaster"].Merge(dsSource.Tables[0]);
          ds.Tables["dtItemCostDetail"].Merge(dsSource.Tables[1]);
          ds.Tables["dtACTSalePrice"].Merge(dsSource.Tables[3]);
          ds.Tables["dtCostSummary"].Merge(dsSource.Tables[4]);
          cptCSDItemCostPrice cptItemCostPrice = new cptCSDItemCostPrice();

          cptItemCostPrice.SetDataSource(ds);
          double dExchange = DBConvert.ParseDouble(outputParamCosting[0].Value.ToString());
          double dFOH = DBConvert.ParseDouble(outputParamCosting[1].Value.ToString());
          double dProfit = DBConvert.ParseDouble(outputParamCosting[2].Value.ToString());
          string remark = outputParamCosting[3].Value.ToString();
          int contractOut = DBConvert.ParseInt(outputParamCosting[4].Value);
          cptItemCostPrice.SetParameterValue("ExchangeRate", dExchange);
          cptItemCostPrice.SetParameterValue("FOH", dFOH);
          cptItemCostPrice.SetParameterValue("Profit", dProfit);
          cptItemCostPrice.SetParameterValue("Remark", remark);
          cptItemCostPrice.SetParameterValue("TotalBOMPrice_VND", totalVNDPrice);
          cptItemCostPrice.SetParameterValue("TotalBOMPrice_USD", totalUSDPrice);
          cptItemCostPrice.SetParameterValue("ViewType", (contractOut == 1 ? "Subcon" : "Inhouse"));
          cptItemCostPrice.SetParameterValue("User", SharedObject.UserInfo.EmpName);

          //Carcass Summary
          DataTable dtCarcassSummary = dsSource.Tables[2];
          if (dtCarcassSummary.Rows.Count > 0)
          {
            cptItemCostPrice.SetParameterValue("TotalLabor", dtCarcassSummary.Rows[0]["TotalLabor"]);
            cptItemCostPrice.SetParameterValue("TotalMainMaterial", dtCarcassSummary.Rows[0]["TotalMainMaterial"]);
            cptItemCostPrice.SetParameterValue("OtherMaterial", dtCarcassSummary.Rows[0]["OtherMaterial"]);
            cptItemCostPrice.SetParameterValue("SubconNetPrice", dtCarcassSummary.Rows[0]["SubconNetPrice"]);
            cptItemCostPrice.SetParameterValue("MaterialSupplied", dtCarcassSummary.Rows[0]["MaterialSupplied"]);
          }

          //// Contract out
          //if (contractOut == 1)
          //{
          //  cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = false;
          //  cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = true;
          //}
          //else
          //{
          //  cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = true;
          //  cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = false;
          //}          

          // New price format (26/8/2016)
          cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = true;
          cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = true;

          //Hide/Show Sale Price
          if (priceType == 0)
          {
            cptItemCostPrice.SalePrice.SectionFormat.EnableSuppress = true;
          }
          else
          {
            cptItemCostPrice.SalePrice.SectionFormat.EnableSuppress = false;
          }

          //ControlUtility.ViewCrystalReport(cptItemCostPrice);
          View_Report frm = new View_Report(cptItemCostPrice);
          frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
        }
      }
      catch { }
    }

    public static void ViewReportCheckListForQC(string itemCode, int revision)
    {
      DataTable dtb = new DataTable();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.String, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int16, revision);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spBOMItemRunCheckListReport", inputParam);

      dtb.Columns.Add("ItemCode", System.Type.GetType("System.String"));
      dtb.Columns.Add("QTy", System.Type.GetType("System.Double"));
      dtb.Columns.Add("ToDate", System.Type.GetType("System.DateTime"));
      dtb.Columns.Add("Images", System.Type.GetType("System.Byte[]"));
      dtb.Columns.Add("WO", System.Type.GetType("System.String"));
      dtb.Columns.Add("FinCode", System.Type.GetType("System.String"));
      dtb.Columns.Add("Name", System.Type.GetType("System.String"));
      if (dt != null)
      {
        foreach (DataRow dr in dt.Rows)
        {
          DataRow drow = dtb.NewRow();
          drow["Images"] = FunctionUtility.ImageToByteArrayWithFormat(FunctionUtility.BOMGetItemImage(itemCode, revision), 380, 1.44, "jpg");
          drow["ItemCode"] = dr["ItemCode"].ToString();
          drow["QTy"] = dr["QTy"].ToString();
          drow["ToDate"] = dr["ToDate"].ToString();
          drow["WO"] = dr["WO"].ToString();
          drow["FinCode"] = dr["FinCode"].ToString();
          drow["Name"] = dr["Name"].ToString();
          dtb.Rows.Add(drow);
        }
      }
      ReportTemplate.QualityControl.cptBOMItemRunCheckList ctpBOM = new DaiCo.Shared.ReportTemplate.QualityControl.cptBOMItemRunCheckList();
      ctpBOM.SetDataSource(dtb);
      View_Report frm = new View_Report(ctpBOM);
      frm.ShowReport(ViewState.Window, FormWindowState.Maximized);

    }

    public static void ViewReportCheckPriceListForMarketing()
    {
      DataTable dtb = new DataTable();
      DBParameter[] inputParam = new DBParameter[2];
      string MonthNo = "4/2015";
      inputParam[0] = new DBParameter("@MonthNo", DbType.String, MonthNo);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spUKRetailPriceListSaleCode", inputParam);

      dtb.Columns.Add("ItemCode", System.Type.GetType("System.String"));
      dtb.Columns.Add("SaleCode", System.Type.GetType("System.String"));
      dtb.Columns.Add("CreateDate", System.Type.GetType("System.DateTime"));
      dtb.Columns.Add("Imags", System.Type.GetType("System.Byte[]"));
      dtb.Columns.Add("Description", System.Type.GetType("System.String"));
      dtb.Columns.Add("MainFinish", System.Type.GetType("System.String"));
      dtb.Columns.Add("CBM", System.Type.GetType("System.Double"));

      foreach (DataRow dr in dt.Rows)
      {
        DataRow drow = dtb.NewRow();
        string Path = dr["ItemCode"].ToString();
        //if (Path.ToString()=="")
        //{
        //  MessageBox.Show(Path.ToString()); 
        //}
        int rev = DBConvert.ParseInt(dr["RevisionActive"].ToString());
        //drow["Imags"] = FunctionUtility.ImageToByteArrayWithFormat(FunctionUtility.BOMGetItemImage(Path, rev), 380, 0.8340, "jpg");
        drow["Imags"] = FunctionUtility.ImageToByteArrayWithFormat(dr["IMG"].ToString(), 380, 1.1794, "JPG");
        drow["ItemCode"] = dr["ItemCode"].ToString();
        drow["CBM"] = dr["CBM"].ToString();
        drow["CreateDate"] = dr["CreateDate"].ToString();
        drow["SaleCode"] = dr["SaleCode"].ToString();
        drow["Description"] = dr["Description"].ToString();
        drow["MainFinish"] = dr["MainFinish"].ToString();
        dtb.Rows.Add(drow);
      }
      ReportTemplate.Marketing.cptUKRetailPriceListSaleCode ctpBOM = new DaiCo.Shared.ReportTemplate.Marketing.cptUKRetailPriceListSaleCode();
      ctpBOM.SetDataSource(dtb);
      View_Report frm = new View_Report(ctpBOM);
      frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
    }
    /// <summary>
    /// view Compare costing detail of item
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="viewType">0: Default, 1: Make local, 2: Contract out</param>
    public static void ViewItemCostingCompare(string itemCode, int isDifference, int compare1, int compare2, params int[] viewType)
    {
      try
      {
        DBParameter[] inputParamCosting = new DBParameter[4];
        inputParamCosting[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        if (viewType.Length > 0 && viewType[0] != int.MinValue)
        {
          inputParamCosting[1] = new DBParameter("@ViewType", DbType.Int32, viewType[0]);
        }
        inputParamCosting[2] = new DBParameter("@Compare1", DbType.Int32, compare1);
        inputParamCosting[3] = new DBParameter("@Compare2", DbType.Int32, compare2);

        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDItemCostPriceCompare", 3600, inputParamCosting);
        if (dsSource != null && dsSource.Tables.Count > 2)
        {
          dsSource.Tables[0].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dsSource.Tables[0].Columns.Add("CheckImage", typeof(String));
          for (int i = 0; i < dsSource.Tables[0].Rows.Count; i++)
          {
            dsSource.Tables[0].Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImageToByteArrayWithFormat(@dsSource.Tables[0].Rows[i]["img"].ToString(), 380, 1.74, "JPG");
            dsSource.Tables[0].Rows[i]["CheckImage"] = dsSource.Tables[0].Rows[i]["Image"].ToString();
          }

          dsCSDItemCostPriceCompare ds = new dsCSDItemCostPriceCompare();
          ds.Tables["dtItemCostMaster"].Merge(dsSource.Tables[0]);
          ds.Tables["dtItemCostDetail"].Merge(dsSource.Tables[1]);

          cptCSDItemCostPriceCompare cptItemCostPrice = new cptCSDItemCostPriceCompare();
          cptItemCostPrice.SetDataSource(ds);

          // Master Information & Carcass Summary
          if (dsSource.Tables[2].Rows.Count > 0)
          {
            DataRow rowSummary = dsSource.Tables[2].Rows[0];
            int contractOut = DBConvert.ParseInt(rowSummary["ContractOut"]);
            cptItemCostPrice.SetParameterValue("StandardTotalLabor", rowSummary["StandardTotalLabor"]);
            cptItemCostPrice.SetParameterValue("ActualTotalLabor", rowSummary["ActualTotalLabor"]);
            cptItemCostPrice.SetParameterValue("StandardTotalMainMaterial", rowSummary["StandardTotalMainMaterial"]);
            cptItemCostPrice.SetParameterValue("ActualTotalMainMaterial", rowSummary["ActualTotalMainMaterial"]);
            cptItemCostPrice.SetParameterValue("StandardOtherMaterial", rowSummary["StandardOtherMaterial"]);
            cptItemCostPrice.SetParameterValue("ActualOtherMaterial", rowSummary["ActualOtherMaterial"]);
            cptItemCostPrice.SetParameterValue("StandardSubconNetPrice", rowSummary["StandardSubconNetPrice"]);
            cptItemCostPrice.SetParameterValue("ActualSubconNetPrice", rowSummary["ActualSubconNetPrice"]);
            cptItemCostPrice.SetParameterValue("StandardMaterialSupplied", rowSummary["StandardMaterialSupplied"]);
            cptItemCostPrice.SetParameterValue("ActualMaterialSupplied", rowSummary["ActualMaterialSupplied"]);
            cptItemCostPrice.SetParameterValue("StandardExchangeRate", rowSummary["StandardExchangeRate"]);
            cptItemCostPrice.SetParameterValue("ActualExchangeRate", rowSummary["ActualExchangeRate"]);
            cptItemCostPrice.SetParameterValue("StandardFOH", rowSummary["StandardFOH"]);
            cptItemCostPrice.SetParameterValue("ActualFOH", rowSummary["ActualFOH"]);
            cptItemCostPrice.SetParameterValue("StandardProfit", rowSummary["StandardProfit"]);
            cptItemCostPrice.SetParameterValue("ActualProfit", rowSummary["ActualProfit"]);
            cptItemCostPrice.SetParameterValue("Remark", rowSummary["Remark"].ToString());
            cptItemCostPrice.SetParameterValue("ViewType", (contractOut == 1 ? "Subcon" : (contractOut == 0 ? "Inhouse" : "Default")));
            cptItemCostPrice.SetParameterValue("TotalStandardBOMPrice", rowSummary["TotalStandardBOMPrice"]);
            cptItemCostPrice.SetParameterValue("TotalActualBOMPrice", rowSummary["TotalActualBOMPrice"]);
            cptItemCostPrice.SetParameterValue("IsDifference", isDifference);
            // Name
            string nameCompare1 = string.Empty;
            string nameCompare2 = string.Empty;
            if (compare1 == 1)
            {
              nameCompare1 = "Original";
            }
            else if (compare1 == 2)
            {
              nameCompare1 = "Last";
            }
            else if (compare1 == 3)
            {
              nameCompare1 = "Current";
            }
            else if (compare1 == 4)
            {
              nameCompare1 = "Standard";
            }
            else if (compare1 == 5)
            {
              nameCompare1 = "Actual";
            }
            // Compare2
            if (compare2 == 1)
            {
              nameCompare2 = "Original";
            }
            else if (compare2 == 2)
            {
              nameCompare2 = "Last";
            }
            else if (compare2 == 3)
            {
              nameCompare2 = "Current";
            }
            else if (compare2 == 4)
            {
              nameCompare2 = "Standard";
            }
            else if (compare2 == 5)
            {
              nameCompare2 = "Actual";
            }
            cptItemCostPrice.SetParameterValue("NameCompare1", nameCompare1);
            cptItemCostPrice.SetParameterValue("NameCompare2", nameCompare2);
            // Name
            // Contract out
            if (contractOut == 1)
            {
              cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = false;
              cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = true;
            }
            else
            {
              cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = true;
              cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = false;
            }
            View_Report frm = new View_Report(cptItemCostPrice);
            frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
          }
        }
      }
      catch { }
    }

    /// <summary>
    /// view costing detail of item
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="viewType">0: Default, 1: Make local, 2: Contract out</param>
    public static void ViewItemCostingByStandardAndActual(string itemCode, int isDifference, params int[] viewType)
    {
      try
      {
        DBParameter[] inputParamCosting = new DBParameter[2];
        inputParamCosting[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        if (viewType.Length > 0 && viewType[0] != int.MinValue)
        {
          inputParamCosting[1] = new DBParameter("@ViewType", DbType.Int32, viewType[0]);
        }

        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDItemCostPrice", 3600, inputParamCosting);
        if (dsSource != null && dsSource.Tables.Count > 2)
        {
          dsSource.Tables[0].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dsSource.Tables[0].Columns.Add("CheckImage", typeof(String));
          for (int i = 0; i < dsSource.Tables[0].Rows.Count; i++)
          {
            dsSource.Tables[0].Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImageToByteArrayWithFormat(@dsSource.Tables[0].Rows[i]["img"].ToString(), 380, 1.74, "JPG");
            dsSource.Tables[0].Rows[i]["CheckImage"] = dsSource.Tables[0].Rows[i]["Image"].ToString();
          }

          dsCSDItemStandardActualCost ds = new dsCSDItemStandardActualCost();
          ds.Tables["dtItemCostMaster"].Merge(dsSource.Tables[0]);
          ds.Tables["dtItemCostDetail"].Merge(dsSource.Tables[1]);

          cptCSDItemStandardActualCost cptItemCostPrice = new cptCSDItemStandardActualCost();
          cptItemCostPrice.SetDataSource(ds);

          // Master Information & Carcass Summary
          if (dsSource.Tables[2].Rows.Count > 0)
          {
            DataRow rowSummary = dsSource.Tables[2].Rows[0];
            int contractOut = DBConvert.ParseInt(rowSummary["ContractOut"]);
            cptItemCostPrice.SetParameterValue("StandardTotalLabor", rowSummary["StandardTotalLabor"]);
            cptItemCostPrice.SetParameterValue("ActualTotalLabor", rowSummary["ActualTotalLabor"]);
            cptItemCostPrice.SetParameterValue("StandardTotalMainMaterial", rowSummary["StandardTotalMainMaterial"]);
            cptItemCostPrice.SetParameterValue("ActualTotalMainMaterial", rowSummary["ActualTotalMainMaterial"]);
            cptItemCostPrice.SetParameterValue("StandardOtherMaterial", rowSummary["StandardOtherMaterial"]);
            cptItemCostPrice.SetParameterValue("ActualOtherMaterial", rowSummary["ActualOtherMaterial"]);
            cptItemCostPrice.SetParameterValue("StandardSubconNetPrice", rowSummary["StandardSubconNetPrice"]);
            cptItemCostPrice.SetParameterValue("ActualSubconNetPrice", rowSummary["ActualSubconNetPrice"]);
            cptItemCostPrice.SetParameterValue("StandardMaterialSupplied", rowSummary["StandardMaterialSupplied"]);
            cptItemCostPrice.SetParameterValue("ActualMaterialSupplied", rowSummary["ActualMaterialSupplied"]);
            cptItemCostPrice.SetParameterValue("StandardExchangeRate", rowSummary["StandardExchangeRate"]);
            cptItemCostPrice.SetParameterValue("ActualExchangeRate", rowSummary["ActualExchangeRate"]);
            cptItemCostPrice.SetParameterValue("StandardFOH", rowSummary["StandardFOH"]);
            cptItemCostPrice.SetParameterValue("ActualFOH", rowSummary["ActualFOH"]);
            cptItemCostPrice.SetParameterValue("StandardProfit", rowSummary["StandardProfit"]);
            cptItemCostPrice.SetParameterValue("ActualProfit", rowSummary["ActualProfit"]);
            cptItemCostPrice.SetParameterValue("Remark", rowSummary["Remark"].ToString());
            cptItemCostPrice.SetParameterValue("ViewType", (contractOut == 1 ? "Subcon" : "Inhouse"));
            cptItemCostPrice.SetParameterValue("TotalStandardBOMPrice", rowSummary["TotalStandardBOMPrice"]);
            cptItemCostPrice.SetParameterValue("TotalActualBOMPrice", rowSummary["TotalActualBOMPrice"]);
            cptItemCostPrice.SetParameterValue("IsDifference", isDifference);

            // Contract out
            if (contractOut == 1)
            {
              cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = false;
              cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = true;
            }
            else
            {
              cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = true;
              cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = false;
            }
            View_Report frm = new View_Report(cptItemCostPrice);
            frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
          }
        }
      }
      catch { }
    }

    #region VB Report, Excel
    public static void InitializeOutputdirectory(string strPathOutputFile)
    {
      if (Directory.Exists(strPathOutputFile))
      {
        string[] files = Directory.GetFiles(strPathOutputFile);
        foreach (string file in files)
        {
          try
          {
            File.Delete(file);
          }
          catch { }
        }
      }
      else
      {
        Directory.CreateDirectory(strPathOutputFile);
      }
    }

    public static XlsReport InitializeXlsReport(string strTemplateName, string strSheetName, string strPreOutFileName, string strPathTemplate, string strPathOutputFile, out string strOutFileName)
    {
      IContainer components = new Container();
      XlsReport oXlsReport = new XlsReport(components);
      InitializeOutputdirectory(strPathOutputFile);
      strTemplateName = string.Format(@"{0}\{1}.xls", strPathTemplate, strTemplateName);
      oXlsReport.FileName = strTemplateName;
      oXlsReport.Start.File();
      oXlsReport.Page.Begin(strSheetName, "1");
      strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", strPathOutputFile, strPreOutFileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      return oXlsReport;
    }

    /// <summary>
    /// Get Source from excel file
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="sheetName"></param>
    /// <param name="cellFromTo"></param>
    /// <returns>DataTable</returns>
    public static DataTable GetSourceFromExcelFile(string filePath, string sheetName, string cellFromTo)
    {
      DataTable dt = new DataTable();
      return dt;
    }
    #endregion VB Report, Excel

    #region Image Proccess
    public static Image GetThumbnailImage(string fileName, string fileFrame)
    {
      Image image = Image.FromFile(fileName);
      Image frame = Image.FromFile(fileFrame);

      Image dest = null;

      int size = frame.Height;
      if (image != null)
      {
        if (image.Width > image.Height)
          dest = image.GetThumbnailImage(size, image.Height * size / image.Width, null, new IntPtr());
        else
          dest = image.GetThumbnailImage(image.Width * size / image.Height, size, null, new IntPtr());
      }

      using (Graphics grfx = Graphics.FromImage(frame))
      {
        grfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        if (image.Width > image.Height)
          grfx.DrawImage(dest, 0, frame.Height / 2 - dest.Height / 2, dest.Width, dest.Height);
        else
          grfx.DrawImage(dest, frame.Width / 2 - dest.Width / 2, 0, dest.Width, dest.Height);
      }

      return frame;
    }

    /// <summary>
    /// thumbnail a image how to fix with frame that it's width and height in CM
    /// </summary>
    /// <param name="fileName">Image file source</param>
    /// <param name="width">Width of frame with cm</param>
    /// <param name="height">height of frame with cm</param>
    /// <returns>Destination image</returns>
    public static Image GetThumbnailImage(string fileName, double width, double height)
    {
      if (!File.Exists(fileName))
        return null;
      Image image = Image.FromFile(fileName);
      string frameFileName = System.Windows.Forms.Application.StartupPath + @"\\frame.JPG";
      Image frame = Image.FromFile(frameFileName);

      int dpix = 600;
      int dpiy = 600;
      System.Drawing.Printing.PrinterSettings oPS = new System.Drawing.Printing.PrinterSettings();
      foreach (System.Drawing.Printing.PrinterResolution resolution in oPS.PrinterResolutions)
      {
        if (resolution.X > 0)
        {
          dpix = resolution.X;
          dpiy = resolution.Y;
        }
      }

      int frameWidth = (int)((width / 2.54) * dpix);
      int frameHeight = (int)((height / 2.54) * dpiy);

      //Bitmap newFrame = new Bitmap(frame, new Size(frameWidth, frameHeight));
      Image newFrame = frame.GetThumbnailImage(frameWidth, frameHeight, null, new IntPtr());

      Image dest = null;

      if (image != null)
      {
        if (image.Width > frameWidth || image.Height > frameHeight)
        {
          if (image.Width > frameWidth && image.Height <= frameHeight)
          {
            dest = image.GetThumbnailImage(frameWidth, image.Height * frameWidth / image.Width, null, new IntPtr());
          }
          else if (image.Width > frameWidth && image.Height > frameHeight)
          {
            if ((float)image.Width / frameWidth < (float)image.Height / frameHeight)
            {
              dest = image.GetThumbnailImage(image.Width * frameHeight / image.Height, frameHeight, null, new IntPtr());
            }
            else
            {
              dest = image.GetThumbnailImage(frameWidth, image.Height * frameWidth / image.Width, null, new IntPtr());
            }
          }
          else if (image.Width < frameWidth && image.Height > frameHeight)
          {
            dest = image.GetThumbnailImage(image.Width * frameHeight / image.Height, frameHeight, null, new IntPtr());
          }
        }
        else
        {
          dest = image;
        }
      }
      try
      {
        using (Graphics grfx = Graphics.FromImage(newFrame))
        {
          grfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
          grfx.DrawImage(dest, (frameWidth - dest.Width) / 2, (frameHeight - dest.Height) / 2, dest.Width, dest.Height);
        }
      }
      catch (Exception ex)
      {
        string logFileName = "c:\\BOMLog.txt";
        if (!File.Exists(logFileName))
          File.Create(logFileName);

        StreamWriter SW;
        SW = File.AppendText(logFileName);
        SW.WriteLine(ex.Message);
        SW.Close();
      }
      finally
      {
        image.Dispose();
        frame.Dispose();
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }

      return newFrame;
    }

    public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
    {
      if (imageIn != null)
      {
        MemoryStream ms = new MemoryStream();
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        ms.Close();
        return ms.ToArray();
      }
      else
      {
        return null;
      }
    }

    /// <summary>
    /// If image <> null => return array byte of image other wise return white image
    /// </summary>
    /// <param name="imageIn"></param>
    /// <returns></returns>
    public static byte[] ImageToByteArray_Always(System.Drawing.Image imageIn)
    {
      if (imageIn != null)
      {
        MemoryStream ms = new MemoryStream();
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        ms.Close();
        return ms.ToArray();
      }
      else
      {
        return ImagePathToByteArray(System.Windows.Forms.Application.ExecutablePath + "\frame.JPG");
      }
    }

    public static Byte[] ImagePathToByteArray(string imagePath)
    {
      try
      {
        FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        br.Close();
        fs.Close();
        return imgbyte;
      }
      catch { }
      return null;
    }

    /// <summary>
    /// If image path is exist => return array byte of image other wise return white image
    /// </summary>
    /// <param name="imagePath"></param>
    /// <returns></returns>
    public static Byte[] ImagePathToByteArray_Always(string imagePath)
    {
      try
      {
        if (!File.Exists(imagePath))
          imagePath = System.Windows.Forms.Application.StartupPath + "\\frame.JPG";
        FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        br.Close();
        fs.Close();
        return imgbyte;
      }
      catch { }
      return null;
    }

    /// <summary>
    /// Copy image from BOM to Subcon folder
    /// </summary>
    public static void CopyImageForSubcon(string carcassCode)
    {
      try
      {
        //check image and copy to subcon folder
        //copy carcass
        string pathFrom = BOMGetCarcassImage(carcassCode);
        string ext = string.Empty;
        if (File.Exists(pathFrom))
        {
          ext = Path.GetExtension(pathFrom);
        }

        string commandText = string.Format("SELECT path FROM TBLBOMImagePath WHERE Pid = 16"); //Thu muc hinh carcass cho subcon
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

        string pathTo = dt.Rows[0]["path"].ToString() + "\\" + carcassCode + ext;
        if (File.Exists(pathFrom) && !File.Exists(pathTo))
        {
          File.Copy(pathFrom, pathTo);
        }

        //copy Item
        commandText = string.Format(@"  SELECT	ItemCode, Revision
                                        FROM	  TblBOMItemInfo 
                                        WHERE	  CarcassCode = '{0}'", carcassCode);
        DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtItem != null && dtItem.Rows.Count > 0)
        {
          for (int i = 0; i < dtItem.Rows.Count; i++)
          {
            pathFrom = BOMGetItemImage(dtItem.Rows[i]["ItemCode"].ToString(), DBConvert.ParseInt(dtItem.Rows[i]["Revision"].ToString()));
            if (File.Exists(pathFrom))
            {
              ext = Path.GetExtension(pathFrom);
            }

            if (DBConvert.ParseInt(dtItem.Rows[i]["Revision"].ToString()) == 0)
            {
              commandText = string.Format("SELECT path FROM TBLBOMImagePath WHERE Pid = 14"); //Thu muc hinh sample Item cho subcon
              dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              pathTo = dt.Rows[0]["path"].ToString() + "\\" + dtItem.Rows[i]["ItemCode"].ToString() + ext;
            }
            else
            {
              commandText = string.Format("SELECT path FROM TBLBOMImagePath WHERE Pid = 15"); //Thu muc hinh Item cho subcon
              dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              pathTo = dt.Rows[0]["path"].ToString() + "\\" + dtItem.Rows[i]["ItemCode"].ToString() + "-" + dtItem.Rows[i]["Revision"].ToString().PadLeft(2, '0') + ext;
            }
            if (File.Exists(pathFrom) && !File.Exists(pathTo))
            {
              File.Copy(pathFrom, pathTo);
            }
          }
        }
      }
      catch { }
    }

    /// <summary>
    /// Show popup image (can move)
    /// </summary>
    /// <param name="imagePath"></param>
    /// <param name="picture"></param>
    public static void ShowImagePopup(string imagePath)
    {
      string str = "\"" + imagePath + "\"";
      Process.Start(@"C:\Windows\Explorer.exe", str);

      //Process p = new Process();
      //p.StartInfo.FileName = "rundll32.exe";
      //if (File.Exists(imagePath))
      //{
      //  p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen " + imagePath;
      //}
      //else
      //{
      //  p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen ";
      //}
      //p.Start();
    }

    #region Draw Image with another size (03/04/2012)
    public static Image cropImage(Image img, Rectangle cropArea)
    {
      Bitmap bmpImage = new Bitmap(img);
      Bitmap bmpCrop = bmpImage.Clone(cropArea,
      bmpImage.PixelFormat);
      return (Image)(bmpCrop);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="imgExtension">BMP, DIB, RLE, JPG, JPEG, JPE, JFIF, GIF, TIF, TIFF, PNG</param>
    /// <returns></returns>
    public static ImageCodecInfo getEncoderInfo(string imgExtension)
    {
      string mimeType = string.Empty;
      switch (imgExtension)
      {
        case "BMP":
        case "DIB":
        case "RLE":
          mimeType = "image/bmp";
          break;
        case "JPG":
        case "JPEG":
        case "JPE":
        case "JFIF":
          mimeType = "image/jpeg";
          break;
        case "GIF":
          mimeType = "image/gif";
          break;
        case "TIF":
        case "TIFF":
          mimeType = "image/tiff";
          break;
        case "PNG":
          mimeType = "image/png";
          break;
        default:
          break;
      }

      // Get image codecs for all image formats
      ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
      // Find the correct image codec
      for (int i = 0; i < codecs.Length; i++)
        if (codecs[i].MimeType == mimeType)
          return codecs[i];
      return null;
    }

    public static Image resizeImage(Image imgToResize, Size size)
    {
      int sourceWidth = imgToResize.Width;
      int sourceHeight = imgToResize.Height;
      float nPercent = 0;
      float nPercentW = 0;
      float nPercentH = 0;
      nPercentW = ((float)size.Width / (float)sourceWidth);
      nPercentH = ((float)size.Height / (float)sourceHeight);
      if (nPercentH < nPercentW)
        nPercent = nPercentH;
      else
        nPercent = nPercentW;
      int destWidth = (int)(sourceWidth * nPercent);
      int destHeight = (int)(sourceHeight * nPercent);
      Bitmap b = new Bitmap(destWidth, destHeight);
      Graphics g = Graphics.FromImage((Image)b);
      g.InterpolationMode = InterpolationMode.HighQualityBicubic;
      g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
      g.Dispose();
      return (Image)b;
    }

    /// <summary>
    /// Support file extension: *.BMP;*.DIB;*.RLE;*.JPG;*.JPEG;*.JPE;*.JFIF;*.GIF;*.TIF;*.TIFF;*.PNG
    /// </summary>
    /// <param name="path"></param>
    /// <param name="img"></param>
    /// <param name="quality"></param>
    /// <param name="imgExtension">BMP, DIB, RLE, JPG, JPEG, JPE, JFIF, GIF, TIF, TIFF, PNG</param>
    public static byte[] SaveImage(Image img, long quality, string imgExtension)
    {
      try
      {
        // Encoder parameter for image quality
        EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
        // Jpeg image codec
        ImageCodecInfo imgCodec = getEncoderInfo(imgExtension.ToUpper());
        if (imgCodec == null)
          return null;
        EncoderParameters encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = qualityParam;

        MemoryStream ms = new MemoryStream();
        img.Save(ms, imgCodec, encoderParams);
        byte[] imgbyte = ms.ToArray();
        ms.Close();
        return imgbyte;
      }
      catch { }
      return null;
    }

    /// <summary>
    /// This function is used for Image with standard Height (EX: Height of Item = 380 pixcel)
    /// </summary>
    /// <param name="imagePath"></param>
    /// <param name="height"></param>
    /// <param name="percentBetweenWidthAndHeight"></param>
    /// <param name="imgExtension">Support file extension: BMP, DIB, RLE, JPG, JPEG, JPE, JFIF, GIF, TIF, TIFF, PNG</param>
    /// <returns></returns>
    public static byte[] ImageToByteArrayWithFormat(string imagePath, int standardHeight, double percentBetweenWidthAndHeight, string imgExtension)
    {
      int standardWidth = (int)((double)standardHeight * percentBetweenWidthAndHeight);
      if (File.Exists(imagePath))
      {
        Image standardImage = Image.FromFile(imagePath);
        if (standardImage != null)
        {
          byte[] imgbyte;
          if (standardImage.Width > standardWidth)
          {
            // Resize Image
            Size standardSize = new Size(standardWidth, standardHeight);
            standardImage = resizeImage(standardImage, standardSize);
            imgbyte = SaveImage(standardImage, 100, imgExtension);
            standardImage.Dispose();
          }
          else
          {
            imgbyte = ImagePathToByteArray(imagePath);
          }
          return imgbyte;
        }
      }
      return null;
    }
    #endregion Draw Image with another size (03/04/2012)

    #endregion Image Proccess

    #region Number Methods

    /// <summary>
    /// Doi so double sang string theo format "999,999.99" (Sau dau '.' là _phanLe chu so thap phan)
    /// Neu _phanLe < 0 thì không định dạng theo format nào cả mà chỉ tra về giá trị _number.ToString().
    /// </summary>
    /// <param name="_number">So double can doi sang string</param>
    /// <param name="_phanLe">So cac so le</param>
    /// <returns></returns>
    public static string NumericFormat(double _number, int _phanLe)
    {
      if (_number == double.MinValue)
      {
        return string.Empty;
      }
      if (_phanLe < 0)
      {
        return _number.ToString();
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      double t = Math.Truncate(_number);
      formatInfo.NumberDecimalDigits = _phanLe;
      return _number.ToString("N", formatInfo);
    }

    /// <summary>
    /// Doi so long sang string theo format "999,999"
    /// </summary>
    /// <param name="_number">So long can doi sang string</param>
    /// <returns></returns>
    public static string NumericFormat(long _number)
    {
      if (_number == long.MinValue)
      {
        return string.Empty;
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      formatInfo.NumberDecimalDigits = 0;
      return _number.ToString("N", formatInfo);
    }
    /// <summary>
    /// Doi so int sang string theo format "999,999"
    /// </summary>
    /// <param name="_number">So int can doi sang string</param>
    /// <returns></returns>

    public static string NumericFormat(int _number)
    {
      if (_number == int.MinValue)
      {
        return string.Empty;
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      formatInfo.NumberDecimalDigits = 0;
      return _number.ToString("N", formatInfo);
    }
    /// <summary>
    /// Doi so Int16 sang string theo format "9,999"
    /// </summary>
    /// <param name="_number">So Int16 can doi sang string</param>
    /// <returns></returns>

    public static string NumericFormat(Int16 _number)
    {
      if (_number == Int16.MinValue)
      {
        return string.Empty;
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      formatInfo.NumberDecimalDigits = 0;
      return _number.ToString("N", formatInfo);
    }

    /// <summary>
    /// Đổi số double sang kiểu string.
    /// Trong phần nguyên cứ 3 số thì thêm dấu phẩy vào
    /// </summary>
    /// <param name="_number"></param>
    /// <returns></returns>
    public static string AddSeparator(double _number)
    {
      if (_number < 0)
      {
        return string.Empty;
      }
      string parseString = _number.ToString();
      int index = parseString.IndexOf('.');
      string partFloat = string.Empty;
      string partInt = parseString;
      if (index > 0)
      {
        partFloat = parseString.Substring(index, parseString.Length - index);
        partInt = parseString.Substring(0, index);
      }
      partInt = NumericFormat(DBConvert.ParseDouble(partInt), 0);
      return string.Format("{0}{1}", partInt, partFloat);
    }

    /// <summary>
    /// Get Default Printer
    /// </summary>
    /// <returns></returns>
    public static string GetDefaultPrinter()
    {
      PrinterSettings settings = new PrinterSettings();
      foreach (string printer in PrinterSettings.InstalledPrinters)
      {
        settings.PrinterName = printer;
        if (settings.IsDefaultPrinter)
          return printer;
      }
      return string.Empty;
    }
    #endregion Number Methods

    #region Accounting AR
    public static void ViewProFormaInvoice(long invoicePid)
    {
      dsACCProFormaInvoice dsProFormaInvoice = new dsACCProFormaInvoice();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ProFormaInvoicePid", DbType.Int64, invoicePid) };
      DataSet dsReportSource = DataBaseAccess.SearchStoreProcedure("spACCProFormaInvoice_Report", inputParam);
      if (dsReportSource != null && dsReportSource.Tables.Count > 1)
      {
        DataColumn col = new DataColumn("LogoPic", typeof(System.Byte[]));
        dsReportSource.Tables[0].Columns.Add(col);
        dsReportSource.Tables[0].Rows[0]["LogoPic"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.Logo);
        dsReportSource.Tables[0].Rows[0]["InWords"] = NumberToEnglish.ChangeUSDollarToWords(dsReportSource.Tables[0].Rows[0]["Total"].ToString());
        dsReportSource.Tables[0].Rows[0]["AmountInWords"] = NumberToEnglish.ChangeUSDollarToWords(dsReportSource.Tables[0].Rows[0]["AmountPayable"].ToString());

        dsProFormaInvoice.Tables["dtCustomer"].Merge(dsReportSource.Tables[0]);
        dsProFormaInvoice.Tables["dtProForma"].Merge(dsReportSource.Tables[1]);
      }

      cptACCProFormaInvoice cpt = new cptACCProFormaInvoice();
      cpt.SetDataSource(dsProFormaInvoice);

      View_Report report = new View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(ViewState.Window, true, FormWindowState.Maximized);
    }

    public static void ViewCommercialInvoice(long invoicePid)
    {
      dsACCCommercialInvoice dsCommercialInvoice = new dsACCCommercialInvoice();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CommercialInvoicePid", DbType.Int64, invoicePid) };
      DataSet dsReportSource = DataBaseAccess.SearchStoreProcedure("spACCCommercialInvoice_Report", inputParam);
      if (dsReportSource != null && dsReportSource.Tables.Count > 1)
      {
        DataColumn col = new DataColumn("LogoPic", typeof(System.Byte[]));
        dsReportSource.Tables[0].Columns.Add(col);
        dsReportSource.Tables[0].Rows[0]["LogoPic"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.VFRLogo);
        dsReportSource.Tables[0].Rows[0]["InWords"] = NumberToEnglish.ChangeUSDollarToWords(dsReportSource.Tables[0].Rows[0]["GrandTotal"].ToString());

        dsCommercialInvoice.Tables["dtCustomer"].Merge(dsReportSource.Tables[0]);
        dsCommercialInvoice.Tables["dtProForma"].Merge(dsReportSource.Tables[1]);
      }

      cptACCCommercialInvoice cpt = new cptACCCommercialInvoice();
      cpt.SetDataSource(dsCommercialInvoice);

      View_Report report = new View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(ViewState.Window, true, FormWindowState.Maximized);
    }

    public static void ViewCreditNote(long creditPid)
    {
      dsACCCreditNote dsCreditNote = new dsACCCreditNote();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CreditPid", DbType.Int64, creditPid) };
      DataSet dsReportSource = DataBaseAccess.SearchStoreProcedure("spACCCreditNote_Report", inputParam);
      if (dsReportSource != null && dsReportSource.Tables.Count > 1)
      {
        dsReportSource.Tables[0].Columns.Add(new DataColumn("LogoPic", typeof(System.Byte[])));
        dsReportSource.Tables[0].Rows[0]["LogoPic"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.VFRLogo);
        dsReportSource.Tables[0].Rows[0]["InWords"] = NumberToEnglish.ChangeUSDollarToWords(dsReportSource.Tables[0].Rows[0]["TotalAmount"].ToString());

        dsCreditNote.Tables["CreditNote"].Merge(dsReportSource.Tables[0]);
        dsCreditNote.Tables["CreditDetail"].Merge(dsReportSource.Tables[1]);
      }

      cptACCCreditNote cpt = new cptACCCreditNote();
      cpt.SetDataSource(dsCreditNote);

      View_Report report = new View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(ViewState.Window, true, FormWindowState.Maximized);
    }

    public static void ViewDebitNote(long debitPid)
    {
      dsACCDebitNote dsDebitNote = new dsACCDebitNote();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@DebitPid", DbType.Int64, debitPid) };
      DataSet dsReportSource = DataBaseAccess.SearchStoreProcedure("spACCDebitNote_Report", inputParam);
      if (dsReportSource != null && dsReportSource.Tables.Count > 1)
      {
        dsReportSource.Tables[0].Columns.Add(new DataColumn("LogoPic", typeof(System.Byte[])));
        dsReportSource.Tables[0].Rows[0]["LogoPic"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.VFRLogo);
        dsReportSource.Tables[0].Rows[0]["InWords"] = NumberToEnglish.ChangeUSDollarToWords(dsReportSource.Tables[0].Rows[0]["TotalAmount"].ToString());

        dsDebitNote.Tables["DebitNote"].Merge(dsReportSource.Tables[0]);
        dsDebitNote.Tables["DebitDetail"].Merge(dsReportSource.Tables[1]);
      }

      cptACCDebitNote cpt = new cptACCDebitNote();
      cpt.SetDataSource(dsDebitNote);

      View_Report report = new View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(ViewState.Window, true, FormWindowState.Maximized);
    }
    #endregion Accounting AR

    #region OpenOfficeTool
    /// <summary>
    /// Get Data From Excel File
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="nSheet"></param>
    /// <param name="areaData"></param>
    /// <returns></returns>
    public static DataSet GetOpenOfficeCalcToDataSet(string strPathFile, string strSql)
    {
      OleDbConnection oConnection = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + strPathFile + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1;MAXSCANROWS=0;\"");
      string strCommandText = strSql;
      OleDbDataAdapter adp = new OleDbDataAdapter(strCommandText, oConnection);
      DataSet dsXLS = new DataSet();
      try
      {
        adp.Fill(dsXLS);
        return dsXLS;
      }
      catch
      {
        return dsXLS;
      }
    }

    public static void ExportToOpenOfficeCalc(UltraGrid ultraGridSource, string fileName)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report\", startupPath);
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, fileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter excelExport = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
      excelExport.Export(ultraGridSource, strOutFileName);
      Process.Start(strOutFileName);
    }
    #endregion OpenOfficeTool
  }
}
