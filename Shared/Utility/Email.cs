/*
   Author  : 
   Email   : minh_it@daico-furniture.com
   Date    : 07/06/2011
   Company :  Dai Co
*/
using DaiCo.Application;
using System.Collections;
using System.Data;
namespace DaiCo.Shared
{
  public class Email
  {
    #region Fields
    // Email Key
    private string key;

    // List Key Send Email
    // Send Email From RDD-> TEC When transfer list item
    public string KEY_RDD_001 = "RDD001";

    // Send Email From CSD -> PLN When Enquiry is confirmed
    public string KEY_CSD_001 = "CSD001";

    // Send Email From CSD -> ACC When SO is confirmed
    public string KEY_CSD_002 = "CSD002";

    // Send Email From CSD -> PLN When EN is returned to PLN
    public string KEY_CSD_003 = "CSD003";

    // Send Email From ACC -> PLN When SO is confirmed
    public string KEY_ACC_001 = "ACC001";

    // Send Email From ACC -> CS When SO is confirmed
    public string KEY_ACC_002 = "ACC002";

    // Send Email From ACC -> CS When SO is confirmed
    public string KEY_ACC_003 = "ACC003";

    // Send Email From PLN -> CS When EN is confirmed
    public string KEY_PLN_001 = "PLN001";

    // Send Email From PLN -> CS When EN is confirmed
    public string KEY_PLN_002 = "PLN002";

    // Send Email From PLN -> CS When WO is openned
    public string KEY_PLN_003 = "PLN003";

    // Send Email From PLN -> ACC When SO is confirmed
    public string KEY_PLN_004 = "PLN004";

    // Send Email From PLN -> CS When SO is confirmed
    public string KEY_PLN_005 = "PLN005";

    // Send Email From PLN -> CS When SO is confirmed
    public string KEY_PLN_006 = "PLN006";

    // Send Email From WHD -> PUR When Receiving Note Veneer is confirmed
    public string KEY_WHD_001 = "WHD001";

    // Send Email From Other Department To IT When request to IT Software
    public string KEY_GNR_001 = "GNR001";

    // Send Email From IT To Order Department When IT Confirm
    public string KEY_GNR_002 = "GNR002";

    // Send Email From IT To Order Department When IT Finish
    public string KEY_GNR_003 = "GNR003";

    // Send Email From PLN -> WHD
    public string KEY_PLN_007 = "PLN007";

    // Send Email From Other Department To WH When request to WH
    public string KEY_MRN_001 = "MRN001";

    // Send Email From WH To Order Department When WH Finish
    public string KEY_MRN_002 = "MRN002";

    // Send Email From Other Department To WH When Department Delete
    public string KEY_MRN_003 = "MRN003";

    // Send Email From PLN -> CS, Production When PLN Update Load List
    public string KEY_PLN_008 = "PLN008";

    // Send Email From CS -> PLN, Production When CS Update Load List
    public string KEY_PLN_009 = "PLN009";

    // Send Email From Production -> PLN, CS When Production Update Load List
    public string KEY_PLN_010 = "PLN010";

    // Send Email for EXI if PLN change loadlist
    public string KEY_EXI_001 = "EXI001";

    // Insert Email
    private string STORE_INSERT_MAIL = "spGNRDataMailInfo_Insert";
    #endregion Fields

    #region Constructors
    public Email()
    {
      this.key = string.Empty;
    }
    #endregion Constructors

    #region Properties
    public string Key
    {
      get { return this.key; }
      set { this.key = value; }
    }
    #endregion Properties

    #region Process
    /// <summary>
    /// Get To From Sql
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetEmailToFromSql(string toFromSql)
    {
      string mailToFromSql = string.Empty;
      DataTable dt = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(toFromSql);
      if (dt == null || dt.Rows.Count == 0)
      {
        return string.Empty;
      }

      foreach (DataRow dr in dt.Rows)
      {
        if (dr[0].ToString().Length > 0)
        {
          mailToFromSql += dr[0].ToString() + ';';
        }
      }

      if (mailToFromSql.Length == 0)
      {
        return string.Empty;
      }

      mailToFromSql = mailToFromSql.Substring(0, mailToFromSql.Length - 1);
      return mailToFromSql;
    }

    /// <summary>
    /// Get Name User Login
    /// </summary>
    /// <returns></returns>
    public string GetNameUserLogin(int userLogin)
    {
      string name = string.Empty;
      string commadText = "SELECT HoNV + ' ' + TenNV Name FROM VHRNhanVien WHERE ID_NhanVien =" + userLogin;
      DataTable dt = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commadText);
      if (dt != null && dt.Rows.Count > 0)
      {
        name = dt.Rows[0][0].ToString();
      }
      return name;
    }

    /// <summary>
    /// Get Subject From Database
    /// </summary>
    /// <param name="emailParameter"></param>
    /// <param name="key"></param>
    public ArrayList GetDataMain(string key)
    {
      ArrayList arrList = new ArrayList();
      string subject = string.Empty;
      string commandText = "SELECT ISNULL([ToFromSql], '') ToFromSql, ISNULL([Subject], '') Subject, ISNULL([Body], '') Body FROM TblGNREmailTemplateInfo WHERE [Key] = '" + key + "'";
      DataTable dt = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null || dt.Rows.Count == 0)
      {
        return arrList;
      }

      // ToFromSql
      arrList.Add(dt.Rows[0][0].ToString());

      // Subject
      arrList.Add(dt.Rows[0][1].ToString());

      // Body
      arrList.Add(dt.Rows[0][2].ToString());

      return arrList;
    }

    /// <summary>
    /// Insert Table TblGNRDataMailInfo
    /// </summary>
    /// <param name="key"></param>
    public void InsertEmail(string key, string toFromSql, string subject, string body)
    {
      string commandText = "SELECT [Key] FROM TblGNREmailTemplateInfo WHERE [Key] = '" + key + "'";
      DataTable dt = DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null || dt.Rows.Count == 0)
      {
        return;
      }
      DBParameter[] input = new DBParameter[4];
      input[0] = new DBParameter("@Key", DbType.AnsiString, 8, key);
      input[1] = new DBParameter("@ToFromSql", DbType.AnsiString, 4000, toFromSql);
      input[2] = new DBParameter("@Subject", DbType.AnsiString, 4000, subject);
      input[3] = new DBParameter("@Body", DbType.AnsiString, 4000, body);
      DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(STORE_INSERT_MAIL, input);
    }
    #endregion Process
  }
}