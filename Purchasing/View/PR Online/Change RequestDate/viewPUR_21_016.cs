/*
  Author      : Huynh Thi Bang
  Date        : 28/10/2016
  Description : Import change requestdate
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;


namespace DaiCo.Purchasing
{
  public partial class viewPUR_21_016 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    //public long transactionPid = long.MinValue;
    public long pid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init
    public viewPUR_21_016()
    {
      InitializeComponent();
    }

    private void viewPUR_21_016_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function

    private void LoadData()
    {
      if (this.pid == long.MinValue)
      {
        DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPURPRChangeDateNo('PRCD') NewReceivingNo");
        if ((dtCode != null) && (dtCode.Rows.Count == 1))
        {
          txtChangeNo.Text = dtCode.Rows[0]["NewReceivingNo"].ToString();
          txtRequestBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          txtDepartment.Text = SharedObject.UserInfo.Department;
        }
      }
      else
      {
        DBParameter[] param = new DBParameter[1];
        param[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
        string storeName = "spPURPRImportChangeRequestDate_Select";
        DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
        txtChangeNo.Text = dsSource.Tables[0].Rows[0]["ChangeNo"].ToString();
        txtRequestBy.Text = dsSource.Tables[0].Rows[0]["RequestBy"].ToString();
        txtRemark.Text = dsSource.Tables[0].Rows[0]["Remark"].ToString();
        txtDepartment.Text = dsSource.Tables[0].Rows[0]["Department"].ToString();
        txtCreateDate.Text = dsSource.Tables[0].Rows[0]["CreateDate"].ToString();
      }
    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "PRT_PUR_21_014";
      string sheetName = "Data";
      string outFileName = "Data";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [Data (1)$B5:E{0}]", 100));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      //input data
      DataTable dtSource = new DataTable();
      dtSource = dsItemList.Tables[0];

      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      DataTable dtInput = this.dtImport();
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtInput.NewRow();
        if (DBConvert.ParseString(dtSource.Rows[i][1].ToString()) != "")
        {
          rowadd["PRDetailPid"] = row["PRDetailPid"];
          rowadd["Qty"] = row["Qty"];
          rowadd["RequestDate"] = row["RequestDate"];
          rowadd["Remark"] = row["Remark"];
          dtInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@Data", SqlDbType.Structured, dtInput);
      DataTable dtResult = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPURChangeRequestDate_Import", sqlinput);

      ultData.DataSource = dtResult;
      for (int j = 0; j < ultData.Rows.Count; j++)
      {
        if (ultData.Rows[j].Cells["StatusText"].Value.ToString().Trim().Length > 0)
        {
          ultData.Rows[j].Appearance.BackColor = Color.Yellow;
        }
      }

    }
    private DataTable dtImport()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("PRDetailPid", typeof(System.Int64));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("RequestDate", typeof(System.DateTime));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Save data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      //Check error
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["StatusText"].Value.ToString().Length > 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          return;
        }
      }
      // Save Data
      bool success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      this.CloseTab();
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      // Save master info
      bool success = this.SaveInfo();
      if (success)
      {
        // Save detail
        success = this.SaveDetail();
      }
      else
      {
        success = false;
      }
      return success;
    }
    /// <summary>
    /// Save Master Information
    /// </summary>
    /// <returns></returns>
    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[7];
      if (this.pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }

      //Code
      inputParam[1] = new DBParameter("@Prefix", DbType.String, "PRCD");
      inputParam[2] = new DBParameter("@Department", DbType.String, SharedObject.UserInfo.Department);
      inputParam[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[4] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[5] = new DBParameter("@Status", DbType.Int32, 0);
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[6] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
      }
      //Code

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeDateInformation_Edit", inputParam, ouputParam);
      // Gan Lai Pid
      this.pid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if (this.pid == long.MinValue)
      {
        return false;
      }
      return true;
    }
    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      DataTable dt = new DataTable();
      dt.Columns.Add("PRDetailPid", typeof(System.Int64));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("RequestDate", typeof(System.DateTime));
      dt.Columns.Add("Remark", typeof(System.String));
      foreach (DataRow row in dtSource.Rows)
      {
        long pidDetail = DBConvert.ParseLong(row["PRDetailPid"]);
        double qty = DBConvert.ParseDouble(row["Qty"]);
        string remark = row["Remark"].ToString().Trim();

        DataRow row1 = dt.NewRow();
        row1["PRDetailPid"] = pidDetail;
        row1["Qty"] = qty;
        row1["RequestDate"] = DBConvert.ParseDateTime(row["RequestDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
        row1["Remark"] = remark;
        dt.Rows.Add(row1);

      }
      SqlDBParameter[] input = new SqlDBParameter[2];
      input[0] = new SqlDBParameter("@Data", SqlDbType.Structured, dt);
      input[1] = new SqlDBParameter("@ChangeNotePid", SqlDbType.Int, this.pid);
      SqlDBParameter[] output = new SqlDBParameter[1];
      output[0] = new SqlDBParameter("@Result", SqlDbType.Int, 0);
      SqlDataBaseAccess.ExecuteStoreProcedure("spPURChangeRequestDateDetail_Insert", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      if (resultSave == 0)
      {
        return false;
      }
      return true;
    }

    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Request Date";
      e.Layout.Bands[0].Columns["RequestDateOld"].Header.Caption = "Request Date Old";
      e.Layout.Bands[0].Columns["PRDetailPid"].Hidden = true;
      //e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["RequestDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["RequestDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      //e.Layout.Bands[1].Columns["RemarkChange"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[1].Columns["RequestDateNew"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[1].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      ////e.Layout.Bands[1].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[1].Columns["PURRemark"].CellActivation = Activation.ActivateOnly;

    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion Event
  }
}
