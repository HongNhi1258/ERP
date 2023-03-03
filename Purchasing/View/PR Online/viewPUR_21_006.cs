/*
  Author      : 
  Date        : 02/07/2013
  Description : PR Online
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_21_006 : MainUserControl
  {
    #region Field
    public long PRPid = long.MinValue;
    private int status = 0;
    #endregion Field

    #region Init
    public viewPUR_21_006()
    {
      InitializeComponent();
    }

    private void viewPUR_21_006_Load(object sender, EventArgs e)
    {
      this.LoadInit();
      this.LoadData();
    }

    private void LoadInit()
    {
      this.LoadTypeOfRequest();
      this.LoadDropDownProjectCode();
      this.LoadDropDownUrgentLevel();
      this.LoadDrawing();
      this.LoadSample();
    }

    private void LoadDropDownProjectCode()
    {
      string commandText = string.Format(@"SELECT  Pid Code, ProjectCode Name 
                                            FROM    TblPURPRProjectCode 
                                            WHERE   ISNULL(Finished, 0) = 0 AND ISNULL([Status], 0) = 1");

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDProjectCode.DataSource = dtSource;
      ultDDProjectCode.DisplayMember = "Name";
      ultDDProjectCode.ValueMember = "Code";
      ultDDProjectCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDProjectCode.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDDProjectCode.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void LoadDropDownUrgentLevel()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7007 AND Code IN(1,3,5) AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDUrgentLevel.DataSource = dtSource;
      ultDDUrgentLevel.DisplayMember = "Name";
      ultDDUrgentLevel.ValueMember = "Code";
      ultDDUrgentLevel.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDUrgentLevel.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDDUrgentLevel.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void LoadUrgent()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7007 AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDDUrgentLevel.DataSource = dtSource;
      ultDDUrgentLevel.DisplayMember = "Name";
      ultDDUrgentLevel.ValueMember = "Code";
      ultDDUrgentLevel.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDUrgentLevel.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultDDUrgentLevel.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadTypeOfRequest()
    {
      string commandText = "SELECT Code, Value Name FROM TblBOMCodeMaster WHERE [Group] = 7005 AND DeleteFlag = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultcbTypeOfRequest.DataSource = dtSource;
      ultcbTypeOfRequest.DisplayMember = "Name";
      ultcbTypeOfRequest.ValueMember = "Code";
      ultcbTypeOfRequest.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbTypeOfRequest.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultcbTypeOfRequest.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadDrawing()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'No' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Yes' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultddDrawing.DataSource = dtSource;
      ultddDrawing.DisplayMember = "Name";
      ultddDrawing.ValueMember = "ID";
      ultddDrawing.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultddDrawing.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultddDrawing.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadSample()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'No' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Yes' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultddSample.DataSource = dtSource;
      ultddSample.DisplayMember = "Name";
      ultddSample.ValueMember = "ID";
      ultddSample.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultddSample.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultddSample.DisplayLayout.AutoFitColumns = true;
    }

    #endregion Init

    #region Function
    private DataTable ArrageGrid()
    {
      DataTable dt = new DataTable();

      dt.Columns.Add("PurCancel", typeof(System.Int32));
      dt.Columns.Add("PurRemark", typeof(System.String));
      dt.Columns.Add("PID", typeof(System.Int64));
      dt.Columns.Add("WO", typeof(System.Int32));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("LastPOSupplier", typeof(System.String));
      dt.Columns.Add("LastPrice", typeof(System.Double));
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("MaterialNameEn", typeof(System.String));
      dt.Columns.Add("MaterialNameVn", typeof(System.String));
      dt.Columns.Add("Unit", typeof(System.String));
      dt.Columns.Add("Status", typeof(System.String));
      dt.Columns.Add("QtyRequest", typeof(System.Double));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("RequestDate", typeof(System.DateTime));
      dt.Columns.Add("Urgent", typeof(System.Int32));
      dt.Columns.Add("ProjectCode", typeof(System.Int32));
      dt.Columns.Add("ExpectedBrand", typeof(System.String));
      dt.Columns.Add("Drawing", typeof(System.Int32));
      dt.Columns.Add("Sample", typeof(System.Int32));
      dt.Columns.Add("Remark", typeof(System.String));

      return dt;
    }

    public void Search()
    {
      this.LoadInit();
      this.LoadData();
    }

    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PRPid", DbType.Int64, this.PRPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURPROnlineInfomation_Select", inputParam);
      if (dsSource != null)
      {
        // 1: Load Master
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          txtPRNo.Text = row["PROnlineNo"].ToString();
          txtCreateDate.Text = row["CreateDate"].ToString();
          txtHeadSignDate.Text = row["HeadDate"].ToString();
          txtDepartment.Text = row["Department"].ToString();
          txtRequester.Text = row["Requester"].ToString();
          txtRemark.Text = row["Remark"].ToString();
          txtPurposeOfPR.Text = row["PurposeOfRequisition"].ToString();
          ultcbTypeOfRequest.Value = DBConvert.ParseInt(row["TypeOfRequest"].ToString());
          labStatus.Text = row["StatusName"].ToString();
          this.status = DBConvert.ParseInt(row["Status"].ToString());
        }
        else
        {
          DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPURPROnlineGetNewPRNo('PROL') NewPRNo");
          if ((dtCode != null) && (dtCode.Rows.Count == 1))
          {
            txtPRNo.Text = dtCode.Rows[0]["NewPRNo"].ToString();
            txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
            txtRequester.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
            txtDepartment.Text = SharedObject.UserInfo.Department;
            labStatus.Text = "New";
          }
        }
        DataTable dt = this.ArrageGrid();
        dt.Merge(dsSource.Tables[1]);

        // 2: Load Detail
        ultData.DataSource = dt;

        // 2: Load File UpLoad
        ultUploadFile.DataSource = dsSource.Tables[2];

        // Set Control
        btnSave.Enabled = true;
        btnReturn.Enabled = true;
        if (this.status != 2)
        {
          btnSave.Enabled = false;
          btnReturn.Enabled = false;
        }
        if (ultUploadFile.Rows.Count == 0)
        {
          grbUploadFile.Visible = false;
        }
      }
    }

    private bool SaveData()
    {
      bool success = this.SaveMaster();
      if (success)
      {
        success = this.SaveDetail();
        return success;
      }
      else
      {
        return false;
      }
    }

    private bool SaveMaster()
    {
      DBParameter[] input = new DBParameter[3];
      input[0] = new DBParameter("@PID", DbType.Int64, this.PRPid);
      input[1] = new DBParameter("@Status", DbType.Int32, 3); // Status = 3 Waiting For Receiving
      input[2] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPURPROnlinePurchaseApproved_Update", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Detail
    /// Update Status PROnline
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      bool success = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        // Input
        DBParameter[] input = new DBParameter[3];
        input[0] = new DBParameter("@PID", DbType.Int64, DBConvert.ParseLong(row.Cells["PID"].Value.ToString()));
        if (DBConvert.ParseInt(row.Cells["PurCancel"].Value.ToString()) == 1)
        {
          input[1] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          input[1] = new DBParameter("@Status", DbType.Int32, 2);
        }
        if (row.Cells["PurRemark"].Value.ToString().Length > 0)
        {
          input[2] = new DBParameter("@PurRemark", DbType.String, row.Cells["PurRemark"].Value.ToString());
        }
        // Output
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        // Exec
        DataBaseAccess.ExecuteStoreProcedure("spPURPROnlineDetailPurchaseApproved_Update", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result <= 0)
        {
          success = false;
          return success;
        }
      }

      // Update Status PROnline Info When Cancel All
      // Input
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@PRPid", DbType.Int64, this.PRPid);
      // Output
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, 0);
      // Exec
      DataBaseAccess.ExecuteStoreProcedure("spPURPROnlineInfomationStatus_Update", inputParam, outputParam);
      long result1 = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result1 <= 0)
      {
        success = false;
        return success;
      }

      return success;
    }

    /// <summary>
    /// Save Mapping PR
    /// </summary>
    /// <returns></returns>
    private bool SaveMappingPR()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PRPid", DbType.Int64, this.PRPid);
      input[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] ouput = new DBParameter[1];
      ouput[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPURPROnlineMappingPRInfomation_Insert", input, ouput);
      long result = DBConvert.ParseLong(ouput[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    private void SendEmail(int type)
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PROnlinePid", DbType.Int64, this.PRPid);
      input[1] = new DBParameter("@Type", DbType.Int32, type);
      DataBaseAccess.ExecuteStoreProcedure("spPURSendEmailWhenConfirmedPROnline", input);
    }
    #endregion Function

    #region Event
    private void btnSave_Click(object sender, EventArgs e)
    {
      // Save Data
      bool success = this.SaveData();
      if (!success)
      {
        return;
      }

      string prOnlineNo = string.Empty;
      string commandText = "SELECT PROnlineNo, [Status] FROM TblPURPROnlineInformation WHERE PID = " + this.PRPid;
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      prOnlineNo = dt.Rows[0]["PROnlineNo"].ToString();

      if (DBConvert.ParseInt(dt.Rows[0]["Status"].ToString()) == 3)  // Wating For Receiving
      {
        // Copy From PR Online To PR
        success = this.SaveMappingPR();
        if (!success)
        {
          return;
        }
        // Send Email
        this.SendEmail(3);

        this.CloseTab();
        viewPUR_03_001 view = new viewPUR_03_001();
        view.prNo = prOnlineNo;
        Shared.Utility.WindowUtinity.ShowView(view, "UPDATE PR", false, DaiCo.Shared.Utility.ViewState.MainWindow);
      }
      else
      {
        this.LoadData();
        this.CloseTab();
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Purpose Of Requisition";
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Required Delivery Date";
      e.Layout.Bands[0].Columns["LastPOSupplier"].Header.Caption = "Last PO Supplier";
      e.Layout.Bands[0].Columns["LastPrice"].Header.Caption = "Last Price";
      e.Layout.Bands[0].Columns["LastPrice"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["PurCancel"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["PurCancel"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PurCancel"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["Urgent"].ValueList = ultDDUrgentLevel;
      e.Layout.Bands[0].Columns["ProjectCode"].ValueList = ultDDProjectCode;
      e.Layout.Bands[0].Columns["Drawing"].ValueList = ultddDrawing;
      e.Layout.Bands[0].Columns["Sample"].ValueList = ultddSample;
      e.Layout.Bands[0].Columns["RequestDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyRequest"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Purchase Qty";
      e.Layout.Bands[0].Columns["QtyRequest"].Header.Caption = "Qty From Requester";
      e.Layout.Bands[0].Columns["WO"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["WO"].MinWidth = 50;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 50;
      e.Layout.Bands[0].Columns["QtyRequest"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["QtyRequest"].MinWidth = 50;
      e.Layout.Bands[0].Columns["RequestDate"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["RequestDate"].MinWidth = 80;
      e.Layout.Bands[0].Columns["PURCancel"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["PURCancel"].MinWidth = 60;
      e.Layout.Bands[0].Columns["PURRemark"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["PURCancel"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      if (this.status != 2)
      {
        e.Layout.Bands[0].Columns["PURRemark"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["PURCancel"].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultUploadFile_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Location"].Hidden = true;

      e.Layout.Bands[0].Columns["FileName"].CellActivation = Activation.ActivateOnly;

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultUploadFile_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultUploadFile.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }

      UltraGridRow row = ultUploadFile.Selected.Rows[0];
      Process prc = new Process();

      if (DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) <= 0)
      {
        prc.StartInfo = new ProcessStartInfo(row.Cells["Location"].Value.ToString());
      }
      else
      {
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string folder = string.Format(@"{0}\Temporary", startupPath);
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }
        string location = row.Cells["Location"].Value.ToString();
        if (File.Exists(location))
        {
          string newLocation = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(row.Cells["Location"].Value.ToString()));
          if (File.Exists(newLocation))
          {
            try
            {
              File.Delete(newLocation);
            }
            catch
            {
              WindowUtinity.ShowMessageWarningFromText("File Is Opening!");
              return;
            }
          }
          File.Copy(location, newLocation);
          prc.StartInfo = new ProcessStartInfo(newLocation);
        }
      }
      try
      {
        prc.Start();
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
      }
    }

    /// <summary>
    /// Return From Purchasing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReturn_Click(object sender, EventArgs e)
    {
      // input
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PRPid", DbType.Int64, this.PRPid);
      input[1] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      // output
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      //Execute
      DataBaseAccess.ExecuteStoreProcedure("spPURPROnlineReturnFromHeadDepartment_Update", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        // Error
        WindowUtinity.ShowMessageError("ERR0037", "Return");
      }
      else
      {
        this.SendEmail(6);
        // Success
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      this.CloseTab();
    }
    #endregion Event
  }
}
