/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: ViewPLN_15_009
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

namespace DaiCo.Planning
{
  public partial class ViewPLN_15_009 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int Status = int.MinValue;
    #endregion Field

    #region Init
    public ViewPLN_15_009()
    {
      InitializeComponent();
    }

    private void ViewPLN_15_009_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(groupBoxMaster);
      this.InitData();
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoAskSaveWhenCloseForm(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.TextChanged += new System.EventHandler(this.Object_Changed);
        }
        else
        {
          this.SetAutoAskSaveWhenCloseForm(ctr);
        }
      }
    }
    private void LoadDDWo()
    {
      string cm = "SELECT distinct WoInfoPID FROM TblPLNWOInfoDetailGeneral ORDER BY WoInfoPID";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraDropDown(ultDDWo, dt, "WoInfoPID", "WoInfoPID");
    }
    private void LoadDDReason()
    {
      string cm = "SELECT Code,Value  FROM TblBOMCodeMaster where [Group] =9223 and DeleteFlag = 0";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraDropDown(ultDDReason, dt, "Code", "Value", "Code");
    }

    private UltraDropDown LoadDDItemCode(int WO, UltraDropDown DDItemcode)
    {
      if (DDItemcode == null)
      {
        DDItemcode = new UltraDropDown();
        this.Controls.Add(DDItemcode);
      }
      string cm = string.Format(@"SELECT DISTINCT CD.ItemCode,IB.Name [ItemName],IB.RevisionActive FROM TblPLNWorkOrderConfirmedDetails CD INNER JOIN TblBOMItemBasic IB ON CD.ItemCode = IB.ItemCode WHERE CD.WorkOrderPid = {0}", WO);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraDropDown(DDItemcode, dt, "ItemCode", "ItemCode", "RevisionActive");
      return DDItemcode;
    }

    private UltraDropDown LoadDDRevision(string Itemcode, UltraDropDown DDRevision)
    {
      if (DDRevision == null)
      {
        DDRevision = new UltraDropDown();
        this.Controls.Add(DDRevision);
      }
      string cm = string.Format(@"SELECT Revision FROM TblBOMItemInfo WHERE ItemCode = '{0}'", Itemcode);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraDropDown(DDRevision, dt, "Revision", "Revision");
      return DDRevision;
    }

    private DataTable CreateTableImport()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("Pid", typeof(System.String));
      taParent.Columns.Add("WO", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.String));
      taParent.Columns.Add("PackingDeadline", typeof(System.DateTime));
      taParent.Columns.Add("PLNRemark", typeof(System.String));
      taParent.Columns.Add("CARRemark", typeof(System.String));
      taParent.Columns.Add("Reason", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      return taParent;
    }

    private void MerceDatatoGrid(DataTable dt)
    {
      DataTable dtSource = this.CreateTableImport();

      foreach (DataRow row in dt.Rows)
      {
        if (dt.Rows[0].ToString().Length > 0)
        {
          DataRow dr = dtSource.NewRow();
          if (row[0].ToString().Length > 0)
          {
            dr["WO"] = row[0].ToString();
            dr["ItemCode"] = row[1].ToString();
            dr["Revision"] = row[2].ToString();
            dr["Qty"] = row[3].ToString();
            if (DBConvert.ParseDateTime(row[4].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
            {
              dr["PackingDeadline"] = DBConvert.ParseDateTime(row[4].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            dr["PLNRemark"] = row[5].ToString();
            dr["CARRemark"] = row[6].ToString();
            dr["Reason"] = row[7].ToString();
            dr["Status"] = row[8].ToString();
            dtSource.Rows.Add(dr);
          }
        }
      }
      DataTable dtGrid = (DataTable)ultData.DataSource;
      foreach (DataRow row in dtGrid.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow dr = dtSource.NewRow();
          if (row[0].ToString().Trim().Length > 0)
          {
            dr["Pid"] = row[0].ToString();
          }
          if (row[2].ToString().Trim().Length > 0)
          {
            dr["WO"] = row[2].ToString();
          }
          dr["ItemCode"] = row[3].ToString();
          dr["Revision"] = row[4].ToString();
          dr["Qty"] = row[5].ToString();
          dr["PackingDeadline"] = DBConvert.ParseDateTime(row[6].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          dr["PLNRemark"] = row[8].ToString();
          dr["CARRemark"] = row[9].ToString();
          if (row[6].ToString().Trim().Length > 0)
          {
            dr["Reason"] = row[7].ToString();
          }
          dtSource.Rows.Add(dr);
        }
      }
      if (!CheckValid(dtSource))
      {
        WindowUtinity.ShowMessageError("ERR0001", "Data");
      }
    }
    private bool CheckValid(DataTable dt)
    {
      bool result = true;
      string storeName = "spPLNCarcassWoDetail_Import";
      SqlDBParameter[] inputParam = new SqlDBParameter[3];
      inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);

      if (ultDateFrom.Value != null && DBConvert.ParseDateTime(ultDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        inputParam[1] = new SqlDBParameter("@FromDate", SqlDbType.DateTime, DBConvert.ParseDateTime(ultDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (ultToDate.Value != null && DBConvert.ParseDateTime(ultToDate.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        inputParam[2] = new SqlDBParameter("@ToDate", SqlDbType.DateTime, DBConvert.ParseDateTime(ultToDate.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      DataTable tableMain = SqlDataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultData.DataSource = tableMain;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int error = DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString());
        if (error > 0)
        {
          ultData.Rows[i].Cells["ItemCode"].ValueList = this.LoadDDItemCode(DBConvert.ParseInt(ultData.Rows[i].Cells["WO"].Value.ToString()), this.ultDDItemcode);
          ultData.Rows[i].Cells["Revision"].ValueList = this.LoadDDRevision(ultData.Rows[i].Cells["ItemCode"].Value.ToString(), this.ultDDRevision);
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
          result = false;
        }
        if (DBConvert.ParseDateTime(ultData.Rows[i].Cells["PackingDeadline"].Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) < DBConvert.ParseDateTime(ultDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) || DBConvert.ParseDateTime(ultData.Rows[i].Cells["PackingDeadline"].Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) > DBConvert.ParseDateTime(ultToDate.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME))
        {
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
        }
      }
      lbCount.Text = string.Format("Count: {0}", (ultData.Rows.FilteredInRowCount));
      return result;
    }

    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //Load DropDown Reason
      this.LoadDDReason();
      //Load DropDown Wo
      this.LoadDDWo();
    }




    private void SetStatusControl(int Status)
    {
      int value = (this.Status == int.MinValue) ? 0 : Status;
      switch (value)
      {
        case 0:
          this.chkConfirm.Enabled = true;
          this.ultDateFrom.ReadOnly = false;
          this.ultToDate.ReadOnly = false;
          break;

        case 1:
          for (int i = 0; i < ultData.Rows.Count; i++)
          {

            ultData.Rows[i].Cells["ItemCode"].Activation = Activation.ActivateOnly;
            ultData.Rows[i].Cells["Revision"].Activation = Activation.ActivateOnly;
            ultData.Rows[i].Cells["WO"].Activation = Activation.ActivateOnly;
          }
          this.ultDateFrom.ReadOnly = true;
          this.ultToDate.ReadOnly = true;
          this.chkConfirm.Enabled = false;
          break;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtCarcassWoNo.Text = dtMain.Rows[0]["CarcassWONo"].ToString();
        ultDateFrom.Value = dtMain.Rows[0]["FromDate"].ToString();
        ultToDate.Value = dtMain.Rows[0]["ToDate"].ToString();
        txtRemark.Text = dtMain.Rows[0]["Remark"].ToString();
        this.Status = DBConvert.ParseInt(dtMain.Rows[0]["StatusID"].ToString());
        this.chkConfirm.Checked = (this.Status == 1 ? true : false);
        txtCreateBy.Text = dtMain.Rows[0]["EmpName"].ToString();
        txtCreateDate.Text = dtMain.Rows[0]["CreateDate"].ToString();
        string weekOfYear = this.WeekofYear(Convert.ToDateTime(dtMain.Rows[0]["FromDate"].ToString())) + " - " + this.WeekofYear(Convert.ToDateTime(dtMain.Rows[0]["ToDate"].ToString()));
        txtWeekNo.Text = weekOfYear;
      }
      else
      {
        txtCarcassWoNo.Text = DataBaseAccess.ExecuteScalarCommandText(string.Format("select dbo.FPLNCarcassWONote ('{0}')", Shared.Utility.ConstantClass.PLN_PREFIX_CARCASSWO)).ToString();
        txtCreateBy.Text = Shared.Utility.SharedObject.UserInfo.UserPid + " - " + Shared.Utility.SharedObject.UserInfo.EmpName;
        txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);

        string weekOfYear = this.WeekofYear(Convert.ToDateTime(this.ultDateFrom.Value.ToString())) + " - " + this.WeekofYear(Convert.ToDateTime(this.ultToDate.Value.ToString()));
        txtWeekNo.Text = weekOfYear;
      }
    }

    private void ImportExcel()
    {
      // Check invalid file
      if (!File.Exists(txtImportText.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportText.Text.Trim(), "SELECT * FROM [List (1)$E3:E4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0].ToString());
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportText.Text.Trim(), string.Format("SELECT * FROM [List (1)$B5:J{0}]", itemCount + 5));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.MerceDatatoGrid(dsItemList.Tables[0]);
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CarcassWoPid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNCarcassWO_Select", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        this.LoadMainData(dsSource.Tables[0]);
        ultData.DataSource = dsSource.Tables[1];
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].Cells["ItemCode"].ValueList = this.LoadDDItemCode(DBConvert.ParseInt(ultData.Rows[i].Cells["WO"].Value.ToString()), this.ultDDItemcode);
          ultData.Rows[i].Cells["Revision"].ValueList = this.LoadDDRevision(ultData.Rows[i].Cells["ItemCode"].Value.ToString(), this.ultDDRevision);
          if (DBConvert.ParseDateTime(ultData.Rows[i].Cells["PackingDeadline"].Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) < DBConvert.ParseDateTime(ultDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) || DBConvert.ParseDateTime(ultData.Rows[i].Cells["PackingDeadline"].Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) > DBConvert.ParseDateTime(ultToDate.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME))
          {
            ultData.Rows[i].Appearance.BackColor = Color.Yellow;
          }
        }
      }
      this.SetStatusControl(this.Status);
      this.NeedToSave = false;
    }

    private bool SaveMain()
    {
      string storeName = "spPLNCarcassWOMaster_Insert";
      int paramNumber = 6;
      DBParameter[] inputParam = new DBParameter[paramNumber];

      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, viewPid);
      }
      if (ultDateFrom.Value != null && DBConvert.ParseDateTime(ultDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        inputParam[1] = new DBParameter("@FromDate", DbType.DateTime, DBConvert.ParseDateTime(ultDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (ultToDate.Value != null && DBConvert.ParseDateTime(ultToDate.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        inputParam[2] = new DBParameter("@ToDate", DbType.DateTime, DBConvert.ParseDateTime(ultToDate.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (txtRemark.Text.ToString().Length > 0)
      {
        inputParam[3] = new DBParameter("@Remark", DbType.String, txtRemark.Text.ToString());
      }

      inputParam[4] = new DBParameter("@Status", DbType.Int32, (this.chkConfirm.Checked ? 1 : 0));
      inputParam[5] = new DBParameter("@CreateBy", DbType.Int64, SharedObject.UserInfo.UserPid);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }

      return false;
    }

    private bool SaveDetail(DataTable dt)
    {
      bool success = true;


      // 2. Insert/Update      

      string storeName = "spPLNCarcassWo_Update";
      int paramNumber = 3;
      SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
      inputParam[1] = new SqlDBParameter("@TransactionPid", SqlDbType.BigInt, this.viewPid);
      inputParam[2] = new SqlDBParameter("@CurrentPid", SqlDbType.BigInt, SharedObject.UserInfo.UserPid);
      SqlDBParameter[] outputParam1 = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue) };
      SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam1);
      if (outputParam1 == null || DBConvert.ParseLong(outputParam1[0].Value.ToString()) < 0)
      {
        success = false;
        if (this.chkConfirm.Checked)
        {
          string cm = string.Format(@"UPDATE TblPLNCarcassWorkOrder 
                      SET [Status] = 0
                      WHERE Pid = {0}", this.viewPid);
          DataBaseAccess.ExecuteCommandText(cm);
        }
      }

      return success;

    }
    private bool CheckValidMaster()
    {
      if (ultDateFrom.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "From Date");
        return false;
      }
      else
      {
        if (DBConvert.ParseDateTime(ultDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
        {
          WindowUtinity.ShowMessageError("ERR0001", "From Date");
          return false;
        }
      }
      if (ultToDate.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "To Date");
        return false;
      }
      else
      {
        if (DBConvert.ParseDateTime(ultToDate.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
        {
          WindowUtinity.ShowMessageError("ERR0001", "To Date");
          return false;
        }
      }

      DBParameter[] inputParam = new DBParameter[3];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.viewPid);
      }
      if (this.ultDateFrom.Value != null && DBConvert.ParseDateTime(this.ultDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        inputParam[1] = new DBParameter("@FromDate", DbType.DateTime, DBConvert.ParseDateTime(this.ultDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (this.ultToDate.Value != null && DBConvert.ParseDateTime(this.ultToDate.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        inputParam[2] = new DBParameter("@ToDate", DbType.DateTime, DBConvert.ParseDateTime(this.ultToDate.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNCheckDatetime", inputParam);
      if (dt.Rows.Count > 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "ToDate or FromDate");
        return false;
      }

      return true;
    }
    private DataTable LoadFromGrid()
    {
      DataTable dt = (DataTable)ultData.DataSource;
      DataTable dtSource = this.CreateTableImport();
      foreach (DataRow row in dt.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow dr = dtSource.NewRow();
          if (row[0].ToString().Length > 0)
          {
            dr["Pid"] = row[0].ToString();
          }
          if (row[2].ToString().Length > 0)
          {
            dr["WO"] = row[2].ToString();
          }
          if (row[3].ToString().Length > 0)
          {
            dr["ItemCode"] = row[3].ToString();
          }
          if (row[4].ToString().Length > 0)
          {
            dr["Revision"] = row[4].ToString();
          }
          if (row[5].ToString().Length > 0)
          {
            dr["Qty"] = row[5].ToString();
          }
          if (row[6].ToString().Length > 0 && DBConvert.ParseDateTime(row[6].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            dr["PackingDeadline"] = DBConvert.ParseDateTime(row[6].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }
          if (row[8].ToString().Length > 0)
          {
            dr["PLNRemark"] = row[8].ToString();
          }
          if (row[9].ToString().Length > 0)
          {
            dr["CARRemark"] = row[9].ToString();
          }
          if (row[7].ToString().Length > 0)
          {
            dr["Reason"] = row[7].ToString();
          }
          dtSource.Rows.Add(dr);
        }
      }
      return dtSource;
    }

    private int WeekofYear(DateTime date)
    {
      int[] moveByDays = { 6, 7, 8, 9, 10, 4, 5 };
      DateTime startOfYear = new DateTime(date.Year, 1, 1);
      DateTime endOfYear = new DateTime(date.Year, 12, 31);
      int numberDays = date.Subtract(startOfYear).Days +
              moveByDays[(int)startOfYear.DayOfWeek];
      int weekNumber = numberDays / 7;
      switch (weekNumber)
      {
        case 0:
          // Before start of first week of this year - in last week of previous year
          weekNumber = WeekofYear(startOfYear.AddDays(-1));
          break;
        case 53:
          // In first week of next year.
          if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
          {
            weekNumber = 1;
          }
          break;
      }
      return weekNumber;
    }
    private void SaveData()
    {


      // 1. Delete      

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]), new DBParameter("@CurrentPid", DbType.Int64, SharedObject.UserInfo.UserPid) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNCarcassWorkOrder_Delete", deleteParam, outputParam);
        if (outputParam == null || DBConvert.ParseInt(outputParam[0].Value.ToString()) < 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Data");
          this.LoadData();
          return;
        }
      }

      if (this.CheckValid(this.LoadFromGrid()) && this.CheckValidMaster())
      {
        bool success = true;
        if (this.SaveMain())
        {
          success = this.SaveDetail(this.LoadFromGrid());
        }
        else
        {
          success = false;
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }

      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Data");
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["TransactionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Error"].Hidden = true;


      // Set caption column
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["PackingDeadline"].Header.Caption = "Packing Deadline";
      e.Layout.Bands[0].Columns["PLNRemark"].Header.Caption = "PLN Remark";
      e.Layout.Bands[0].Columns["CARRemark"].Header.Caption = "CAR Remark";
      e.Layout.Bands[0].Columns["WO"].Header.Caption = "Work Order";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["Reason"].ValueList = this.ultDDReason;
      e.Layout.Bands[0].Columns["WO"].ValueList = this.ultDDWo;
      e.Layout.Bands[0].Columns["Itemcode"].ValueList = this.ultDDItemcode;
      e.Layout.Bands[0].Columns["Revision"].ValueList = this.ultDDRevision;

      // Set Align
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Reason"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["Status"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();

      string colName = e.Cell.Column.ToString();
      string value = e.Cell.Value.ToString();
      switch (colName)
      {
        case "WO":

          int wo = DBConvert.ParseInt(value);
          UltraDropDown ultDDItemCode = (UltraDropDown)e.Cell.Row.Cells["ItemCode"].ValueList;
          e.Cell.Row.Cells["ItemCode"].ValueList = this.LoadDDItemCode(wo, ultDDItemCode);
          e.Cell.Row.Cells["Revision"].Value = DBNull.Value;
          break;
        case "ItemCode":

          string Itemcode = value;
          UltraDropDown ultDDRevision = (UltraDropDown)e.Cell.Row.Cells["Revision"].ValueList;
          e.Cell.Row.Cells["Revision"].Value = DBNull.Value;
          e.Cell.Row.Cells["Revision"].ValueList = this.LoadDDRevision(Itemcode, ultDDRevision);

          break;
        default:
          break;
      }

    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.btnSave.Enabled = false;
      this.SaveData();
      this.btnSave.Enabled = true;
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }
    private void btnImportExcel_Click(object sender, EventArgs e)
    {
      btnImportExcel.Enabled = false;
      this.ImportExcel();
      btnImportExcel.Enabled = true;
    }
    private void btnGetTemp_Click(object sender, EventArgs e)
    {
      string templateName = string.Empty;
      templateName = "PLN_15_009";
      string sheetName = "List";
      string outFileName = "Template Import Container";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNReasonCarcassWO");

      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dtRow = dt.Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("L7:M7").Copy();
          oXlsReport.RowInsert(10 + i);
          oXlsReport.Cell("L7:M7", 0, i).Paste();
        }

        oXlsReport.Cell("**1", 0, i).Value = dtRow["CodeReason"];
        oXlsReport.Cell("**2", 0, i).Value = dtRow["TextReason"];
      }
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);


    }
    private void btnBrowser_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportText.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }
    #endregion Event

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        UltraGrid AA = ultData;

        AA.Rows.Band.Columns["ItemCode"].Header.Caption = "Item Code";
        AA.Rows.Band.Columns["PackingDeadline"].Header.Caption = "Packing Deadline";
        AA.Rows.Band.Columns["PLNRemark"].Header.Caption = "PLN Remark";
        AA.Rows.Band.Columns["CARRemark"].Header.Caption = "CAR Remark";
        AA.Rows.Band.Columns["WO"].Header.Caption = "Work Order";



        AA.Rows.Band.Columns["Pid"].Hidden = true;
        AA.Rows.Band.Columns["TransactionPid"].Hidden = true;
        AA.Rows.Band.Columns["Error"].Hidden = true;


        ControlUtility.ExportToExcelWithDefaultPath(AA, out xlBook, "Carcass WO Detail", 7);
        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Vietnam Furniture Resources Company limited (VFR Co.,ltd)";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan ward, Thuan An town, Binh Duong province";

        xlSheet.Cells[3, 1] = "Carcass Wo Detail";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        xlSheet.Cells[5, 1] = "Note: ";
        r.Font.Bold = true;
        xlBook.Application.DisplayAlerts = false;
        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }
  }
}
