/*
  Author      : 
  Date        : 2/7/2013
  Description : Insert PR, Update PR Cancel
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using FormSerialisation;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_21_004 : MainUserControl
  {
    #region Field
    // Pid
    public long prCancelPid = long.MinValue;
    private bool flagCheck = true;
    // Status
    private int status = 0;
    private DataTable dtDataPRNo;
    //Flag Purchaser
    public bool flagPur = false;
    #endregion Field

    #region Init Data
    /// <summary>
    /// 
    /// </summary>
    public viewPUR_21_004()
    {
      InitializeComponent();
      ultCreateDate.FormatString = ConstantClass.FORMAT_DATETIME;
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_21_004_Load(object sender, EventArgs e)
    {
      // Load Department
      this.LoadComboDepartment();

      // Load List PRCancel
      this.LoadComboPRCancel();

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Search
    /// </summary>
    public void Search()
    {
      // Load Department
      this.LoadComboDepartment();

      // Load Data
      this.LoadData();
    }

    private void LoadDropDownPRNo()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid) };
      dtDataPRNo = DataBaseAccess.SearchStoreProcedureDataTable("spPURPRCancelGetQtyCancel_Select", inputParam);

      udrpPRNo.DataSource = dtDataPRNo;
      udrpPRNo.DisplayMember = "PRNo";
      udrpPRNo.ValueMember = "PRDetailPid";
      udrpPRNo.DisplayLayout.Bands[0].ColHeadersVisible = true;
      udrpPRNo.DisplayLayout.Bands[0].Columns["PRNo"].Width = 250;
      udrpPRNo.DisplayLayout.Bands[0].Columns["PRDetailPid"].Hidden = true;
    }

    private void LoadDropDownReason()
    {
      string commandText = string.Empty;
      commandText += " SELECT Code, Value ";
      commandText += " FROM TblBOMCodeMaster ";
      commandText += " WHERE [Group] = 7050 ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      udrpReason.DataSource = dtSource;
      udrpReason.DisplayMember = "Value";
      udrpReason.ValueMember = "Code";
      udrpReason.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpReason.DisplayLayout.Bands[0].Columns["Value"].Width = 250;
      udrpReason.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment ORDER BY Department";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "Name";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Requester
    /// </summary>
    /// <param name="group"></param>
    private void LoadComboRequester(string department)
    {
      string commandText = string.Format("SELECT ID_NhanVien, TenNV + ' ' + HoNV + ' - ' + CAST(ID_NhanVien AS VARCHAR) Name FROM VHRNhanVien WHERE Resigned = 0 AND Department = '{0}' ORDER BY Name", department);
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultRequester.DataSource = dtSource;
      ultRequester.DisplayMember = "Name";
      ultRequester.ValueMember = "ID_NhanVien";
      ultRequester.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultRequester.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultRequester.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo List PRCancel
    /// </summary>
    /// <param name="group"></param>
    private void LoadComboPRCancel()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPURListPROnlineNeedToCancel_Select", input);
      if (dtSource == null)
      {
        return;
      }
      ultCBPRCancel.DataSource = dtSource;
      ultCBPRCancel.DisplayMember = "PROnlineNo";
      ultCBPRCancel.ValueMember = "PROnlineNo";
      ultCBPRCancel.DisplayLayout.Bands[0].Columns["PROnlineNo"].Width = 200;
      ultCBPRCancel.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@PRCancelPid", DbType.Int64, this.prCancelPid);
      inputParam[1] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURPRCancelInformationByPid_Select", inputParam);
      DataTable dtPRCancelInfo = dsSource.Tables[0];

      if (dtPRCancelInfo != null && dtPRCancelInfo.Rows.Count > 0)
      {
        DataRow row = dtPRCancelInfo.Rows[0];

        this.txtPRCancelNo.Text = row["PRCancelNo"].ToString();
        this.ultCreateDate.Value = row["CreateDate"].ToString();
        this.ultDepartment.Value = row["Department"].ToString();
        this.ultRequester.Value = DBConvert.ParseInt(row["RequestBy"].ToString());
        this.txtRemark.Text = row["Remark"].ToString();
        this.status = DBConvert.ParseInt(row["Status"].ToString());

        if (this.status == 1)
        {
          this.chkConfirm.Checked = true;
        }
      }
      else
      {
        DataTable dtPRCancel = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPURGetNewPRCancelNo('PRC') NewPRCancelNo");
        if ((dtPRCancel != null) && (dtPRCancel.Rows.Count > 0))
        {
          this.txtPRCancelNo.Text = dtPRCancel.Rows[0]["NewPRCancelNo"].ToString();
          this.ultCreateDate.Value = DateTime.Now;
          this.ultDepartment.Value = SharedObject.UserInfo.Department;
          this.ultRequester.Value = SharedObject.UserInfo.UserPid;
        }
      }
      this.SetStatusControl();

      // Load Drop Down PRNo
      this.LoadDropDownPRNo();

      // Load Drop Down Reason
      this.LoadDropDownReason();

      this.ultData.DataSource = dsSource.Tables[1];
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      bool flag = true;
      if (this.status == 1 || this.status == 2)
      {
        flag = false;
      }
      this.txtPRCancelNo.Enabled = false;
      this.ultCreateDate.Enabled = false;
      this.ultDepartment.Enabled = false;
      this.ultRequester.Enabled = false;

      this.txtRemark.Enabled = flag;
      this.btnSave.Enabled = flag;
      this.chkConfirm.Enabled = flag;
      this.btnAddPRCancel.Enabled = flag;
      this.ultCBPRCancel.Enabled = flag;
    }
    #endregion Init Data

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();

      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PRDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["PRNo"].ValueList = udrpPRNo;
      e.Layout.Bands[0].Columns["Reason"].ValueList = udrpReason;

      e.Layout.Bands[0].Columns["No"].MaxWidth = 20;
      e.Layout.Bands[0].Columns["No"].MinWidth = 20;

      e.Layout.Bands[0].Columns["PRNo"].Header.Caption = "(*)PRNo";
      e.Layout.Bands[0].Columns["QtyCancel"].Header.Caption = "(*)QtyCancel";
      e.Layout.Bands[0].Columns["Reason"].Header.Caption = "(*)Reason";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Material Name";

      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["PurQtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["PRNo"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Reason"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.Aqua;

      for (int i = 0; i < this.ultData.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      if (this.status != 1)
      {
        ultData.DisplayLayout.Bands[0].Columns["PRNo"].CellActivation = Activation.AllowEdit;
        ultData.DisplayLayout.Bands[0].Columns["QtyCancel"].CellActivation = Activation.AllowEdit;
        ultData.DisplayLayout.Bands[0].Columns["Reason"].CellActivation = Activation.AllowEdit;
        ultData.DisplayLayout.Bands[0].Columns["Remark"].CellActivation = Activation.AllowEdit;
      }
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultDepartment_ValueChanged(object sender, EventArgs e)
    {
      if (ultDepartment.SelectedRow != null)
      {
        string department = ultDepartment.SelectedRow.Cells["Department"].Value.ToString().Trim();
        this.LoadComboRequester(department);
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      string commandText = string.Empty;
      switch (columnName)
      {
        case "prno":
          try
          {
            e.Cell.Row.Cells["PRDetailPid"].Value = udrpPRNo.SelectedRow.Cells["PRDetailPid"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["PRDetailPid"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["WO"].Value = udrpPRNo.SelectedRow.Cells["WO"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["WO"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["CarcassCode"].Value = udrpPRNo.SelectedRow.Cells["CarcassCode"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["CarcassCode"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["ItemCode"].Value = udrpPRNo.SelectedRow.Cells["ItemCode"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["ItemCode"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["MaterialCode"].Value = udrpPRNo.SelectedRow.Cells["MaterialCode"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["MaterialCode"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["NameEN"].Value = udrpPRNo.SelectedRow.Cells["NameEN"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["NameEN"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["NameVN"].Value = udrpPRNo.SelectedRow.Cells["NameVN"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["NameVN"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["Unit"].Value = udrpPRNo.SelectedRow.Cells["Unit"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["Unit"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["RequestDate"].Value = udrpPRNo.SelectedRow.Cells["RequestDate"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["RequestDate"].Value = DBNull.Value;
          }

          this.flagCheck = false;
          e.Cell.Row.Cells["QtyCancel"].Value = 0;
          this.flagCheck = true;

          try
          {
            e.Cell.Row.Cells["Remain"].Value = udrpPRNo.SelectedRow.Cells["QtyCancel"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["Remain"].Value = DBNull.Value;
          }

          this.flagCheck = false;
          try
          {
            e.Cell.Row.Cells["QtyCancel"].Value = udrpPRNo.SelectedRow.Cells["QtyCancel"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["QtyCancel"].Value = DBNull.Value;
          }
          this.flagCheck = true;

          break;
        default:
          break;
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      string commandText = string.Empty;
      switch (columnName.ToLower())
      {
        case "qtycancel":
          if (DBConvert.ParseDouble(text)
              > DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString()))
          {
            WindowUtinity.ShowMessageErrorFromText("Qty Cancel must <= Remain Qty");
            e.Cancel = true;
          }

          if (flagCheck == true)
          {
            if (DBConvert.ParseDouble(text) <= 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Qty Cancel must > 0");
              e.Cancel = true;
            }
          }
          break;
        case "reason":
          commandText += " SELECT COUNT(*) SL ";
          commandText += " FROM TblBOMCodeMaster ";
          commandText += " WHERE [Group] = 7050 ";
          commandText += " 	AND Value = '" + text + "'";

          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null)
          {
            if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Reason");
              e.Cancel = true;
            }
          }

          break;
        case "prno":
          DataRow[] foundRow = dtDataPRNo.Select("PRNo ='" + text + "'");
          if (foundRow.Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "PRNo");
            e.Cancel = true;
          }

          break;
        default:
          break;
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success;
      string message = string.Empty;

      success = this.CheckValidPRInformationInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      success = this.CheckValidPRDetailInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.SendEmail();
      this.LoadData();
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.prCancelPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
          {
            DBParameter[] inputParams = new DBParameter[1];
            inputParams[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));

            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            string storeName = string.Empty;
            storeName = "spPURPRCancelDetailInformation_Delete";

            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
            if (DBConvert.ParseLong(outputParams[0].Value.ToString()) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0004");
              this.LoadData();
              return;
            }
          }
        }
      }
    }

    /// <summary>
    /// Add PR Cancel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddPRCancel_Click(object sender, EventArgs e)
    {
      if (ultCBPRCancel.Value != null)
      {
        DBParameter[] input = new DBParameter[2];
        input[0] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);
        input[1] = new DBParameter("@PRNo", DbType.String, ultCBPRCancel.Value.ToString());

        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPURPRCancelGetQtyCancel_Select", input);
        if (dtSource != null)
        {
          DataTable dtMain = (DataTable)ultData.DataSource;
          for (int i = 0; i < dtSource.Rows.Count; i++)
          {
            DataRow rowData = dtSource.Rows[i];
            // Check Exist PRDetail
            DataRow[] foundRow = dtMain.Select("PRDetailPid = " + DBConvert.ParseLong(rowData["PRDetailPid"].ToString()) + "");
            if (foundRow.Length == 0)
            {
              // End
              DataRow row = dtMain.NewRow();
              row["PRDetailPid"] = DBConvert.ParseLong(rowData["PRDetailPid"].ToString());
              row["PRNo"] = rowData["PRNo"].ToString();
              if (DBConvert.ParseLong(rowData["WO"].ToString()) != long.MinValue)
              {
                row["WO"] = DBConvert.ParseLong(rowData["WO"].ToString());
              }
              if (rowData["CarcassCode"].ToString().Length > 0)
              {
                row["CarcassCode"] = rowData["CarcassCode"].ToString();
              }
              if (rowData["ItemCode"].ToString().Length > 0)
              {
                row["ItemCode"] = rowData["ItemCode"].ToString();
              }
              row["MaterialCode"] = rowData["MaterialCode"].ToString();
              row["NameEN"] = rowData["NameEN"].ToString();
              row["NameVN"] = rowData["NameVN"].ToString();
              row["Unit"] = rowData["Unit"].ToString();
              row["RequestDate"] = rowData["RequestDate"].ToString();
              row["Remain"] = DBConvert.ParseDouble(rowData["QtyCancel"].ToString());
              row["QtyCancel"] = DBConvert.ParseDouble(rowData["QtyCancel"].ToString());

              dtMain.Rows.Add(row);
            }
          }
          ultData.DataSource = dtMain;
        }
      }
    }
    #endregion Event

    #region Function
    /// <summary>
    /// Save Insert New Pr Information
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        DBParameter[] inputParam = new DBParameter[8];
        UltraGridRow row = this.ultData.Rows[i];
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
        {
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
        }

        inputParam[1] = new DBParameter("@PRCPid", DbType.Int64, this.prCancelPid);
        inputParam[2] = new DBParameter("@PRDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["PRDetailPid"].Value.ToString()));
        inputParam[3] = new DBParameter("@QtyCancel", DbType.Double, DBConvert.ParseDouble(row.Cells["QtyCancel"].Value.ToString()));
        inputParam[4] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(row.Cells["Reason"].Value.ToString()));
        inputParam[5] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());

        // Truong Add
        if (this.flagPur && chkConfirm.Checked)
        {
          inputParam[6] = new DBParameter("@QtyCancelPur", DbType.Double, DBConvert.ParseDouble(row.Cells["QtyCancel"].Value.ToString()));
          inputParam[7] = new DBParameter("@Reject", DbType.Int32, 2);
        }
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure("spPURPRCancelDetailInformation_Edit", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result == 0)
        {
          return false;
        }
      }

      // Cast Data To Purchase (Qty Cancel)
      if (this.flagPur && this.chkConfirm.Checked)
      {
        // Input
        DBParameter[] inputParamCast = new DBParameter[2];
        inputParamCast[0] = new DBParameter("@PRCancelPid", DbType.Int64, this.prCancelPid);
        inputParamCast[1] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);
        // Output
        DBParameter[] outputParamCast = new DBParameter[1];
        outputParamCast[0] = new DBParameter("@Result", DbType.Int64, 0);
        // Exec
        DataBaseAccess.ExecuteStoreProcedure("spPURPRCancelUpdateToPurchasingSystem_Update", inputParamCast, outputParamCast);
        long result1 = DBConvert.ParseLong(outputParamCast[0].Value.ToString());
        if (result1 <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Insert New Pr Information
    /// </summary>
    /// <returns></returns>
    private long SaveInformation()
    {
      long result = 0;

      DBParameter[] inputParam = new DBParameter[7];
      string commandText = string.Empty;
      DataTable dt = new DataTable();

      string storeName = string.Empty;
      storeName = "spPURPRCancelInformation_Edit";

      if (this.flagPur == true && this.chkConfirm.Checked)
      {
        this.status = 2;
      }
      else if (this.chkConfirm.Checked)
      {
        this.status = 1;
      }
      else
      {
        this.status = 0;
      }

      // Insert
      if (this.prCancelPid == long.MinValue)
      {
        inputParam[1] = new DBParameter("@RequestBy", DbType.Int32, DBConvert.ParseInt(this.ultRequester.Value.ToString()));
        inputParam[2] = new DBParameter("@Department", DbType.String, this.ultDepartment.Value.ToString());
        inputParam[3] = new DBParameter("@Status", DbType.Int32, this.status);
        inputParam[4] = new DBParameter("@Remark", DbType.String, this.txtRemark.Text);
        inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }
      // Update
      else
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.prCancelPid);
        inputParam[3] = new DBParameter("@Status", DbType.Int32, this.status);
        inputParam[4] = new DBParameter("@Remark", DbType.String, this.txtRemark.Text);
        inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return result;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      // Save Master
      this.prCancelPid = this.SaveInformation();
      if (prCancelPid <= 0)
      {
        return false;
      }
      else
      {
        bool result = this.SaveDetail();
        if (!result)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Check Valid PR Information Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPRInformationInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;

      // Department
      if (this.ultDepartment.Value == null || this.ultDepartment.Value.ToString().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Department");
        return false;
      }

      // Requester
      if (this.ultRequester.Value == null
          || DBConvert.ParseInt(this.ultRequester.Value.ToString()) == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Requester");
        return false;
      }

      return true;
    }

    /// <summary>
    /// Check Valid PR Detail Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPRDetailInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;

      int count = ultData.Rows.Count;
      if (count == 0 && this.chkConfirm.Checked)
      {
        message = FunctionUtility.GetMessage("ERR0087");
        return false;
      }

      DataTable dtSource = (DataTable)this.ultData.DataSource;

      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];

        // Check Exist PR Detail Pid
        DataRow[] foundRow = dtSource.Select("PRDetailPid =" + DBConvert.ParseLong(row.Cells["PRDetailPid"].Value.ToString()));
        if (foundRow.Length > 1)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Data On Grid");
          return false;
        }

        // Material Code
        if (row.Cells["MaterialCode"].Value.ToString().Trim().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Material Code");
          return false;
        }

        // Reason
        if (row.Cells["Reason"].Value.ToString().Trim().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Reason");
          return false;
        }
      }

      return true;
    }

    private void SendEmail()
    {
      if (chkConfirm.Checked)
      {
        int type = int.MinValue;
        if (this.flagPur) // Purchaser
        {
          type = 5;
        }
        else // Requester
        {
          type = 4;
        }
        DBParameter[] input = new DBParameter[6];
        input[0] = new DBParameter("@PRCancelPid", DbType.Int64, this.prCancelPid);
        input[1] = new DBParameter("@Type", DbType.Int32, type);

        MainUserControl c = (MainUserControl)this;
        string strTypeObject = c.GetType().FullName + "," + c.GetType().Namespace.Split('.')[1];
        string strTitle = SharedObject.tabContent.TabPages[SharedObject.tabContent.SelectedIndex].Text;
        string strFileName = c.Name + ".xml";
        MemoryStream stream = new MemoryStream();
        stream = FormSerialisor.Serialise(c);
        byte[] file;
        file = stream.ToArray();
        stream.Close();

        input[2] = new DBParameter("@File", DbType.Binary, file.Length, file);
        input[3] = new DBParameter("@TypeObject", DbType.AnsiString, 500, strTypeObject);
        input[4] = new DBParameter("@Title", DbType.AnsiString, 300, strTitle);
        input[5] = new DBParameter("@FileName", DbType.String, 300, strFileName);

        DataBaseAccess.ExecuteStoreProcedure("spPURSendEmailWhenConfirmedPROnline", input);
      }
    }
    #endregion Function
  }
}
