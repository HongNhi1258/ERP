/*
  Author      : 
  Date        : 2/7/2013
  Description : Insert PR, Update PR Cancel
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_21_007 : MainUserControl
  {
    #region Field
    // Pid
    public long prCancelPid = long.MinValue;
    // Status
    private int status = 1;
    #endregion Field

    #region Init Data
    /// <summary>
    /// 
    /// </summary>
    public viewPUR_21_007()
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

      // Load Requester
      this.LoadComboRequester();

      // Load Data
      this.LoadData();
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
    private void LoadComboRequester()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, TenNV + ' ' + HoNV + ' - ' + CAST(ID_NhanVien AS VARCHAR) Name ";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Resigned = 0 ";
      commandText += " ORDER BY Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultRequester.DataSource = dtSource;
      ultRequester.DisplayMember = "Name";
      ultRequester.ValueMember = "ID_NhanVien";
      ultRequester.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultRequester.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultRequester.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@PRCancelPid", DbType.Int64, this.prCancelPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURPRCancelInformationByPidApproved_Select", inputParam);
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

        if (this.status == 2)
        {
          this.chkConfirm.Checked = true;
        }
      }

      this.SetStatusControl();

      this.ultData.DataSource = dsSource.Tables[1];
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      bool flag = true;
      if (this.status == 2)
      {
        flag = false;
      }
      this.txtPRCancelNo.Enabled = false;
      this.ultCreateDate.Enabled = false;
      this.ultDepartment.Enabled = false;
      this.ultRequester.Enabled = false;

      this.txtRemark.Enabled = false;
      this.btnSave.Enabled = flag;
      this.chkConfirm.Enabled = flag;
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

      e.Layout.Bands[0].Columns["No"].MaxWidth = 20;
      e.Layout.Bands[0].Columns["No"].MinWidth = 20;

      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Material Name";

      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["PurQtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["PurQtyCancel"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["PurRemark"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Reject"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Columns["Reject"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      if (this.status == 1)
      {
        for (int i = 0; i < this.ultData.DisplayLayout.Bands[0].Columns.Count - 3; i++)
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      else
      {
        for (int i = 0; i < this.ultData.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      string commandText = string.Empty;
      switch (columnName.ToLower())
      {
        case "purqtycancel":
          if (DBConvert.ParseDouble(text)
              > DBConvert.ParseDouble(e.Cell.Row.Cells["QtyCancel"].Value.ToString()))
          {
            WindowUtinity.ShowMessageErrorFromText("Pur Qty Cancel must <= Qty Cancel Of User");
            e.Cancel = true;
          }

          if (DBConvert.ParseDouble(text) <= 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Pur Qty Cancel must > 0");
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
        DBParameter[] inputParam = new DBParameter[4];
        UltraGridRow row = this.ultData.Rows[i];
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));

        inputParam[1] = new DBParameter("@QtyCancelPur", DbType.Double, DBConvert.ParseDouble(row.Cells["PurQtyCancel"].Value.ToString()));
        inputParam[2] = new DBParameter("@Remark", DbType.String, row.Cells["PurRemark"].Value.ToString());
        if (DBConvert.ParseInt(row.Cells["Reject"].Value.ToString()) == 0)
        {
          inputParam[3] = new DBParameter("@Reject", DbType.Int32, 2);
        }
        else
        {
          inputParam[3] = new DBParameter("@Reject", DbType.Int32, 1);
        }

        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure("spPURPRCancelDetailPurApproved_Update", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      // Cast Data To Purchase (Qty Cancel)
      if (this.chkConfirm.Checked)
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

      if (this.chkConfirm.Checked)
      {
        this.status = 2;
      }
      else
      {
        this.status = 1;
      }

      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.prCancelPid);
      inputParam[3] = new DBParameter("@Status", DbType.Int32, this.status);
      inputParam[4] = new DBParameter("@Remark", DbType.String, this.txtRemark.Text);
      inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return result;
    }

    private void SendEmail()
    {
      if (chkConfirm.Checked)
      {
        DBParameter[] input = new DBParameter[2];
        input[0] = new DBParameter("@PRCancelPid", DbType.Int64, this.prCancelPid);
        input[1] = new DBParameter("@Type", DbType.Int32, 5);
        DataBaseAccess.ExecuteStoreProcedure("spPURSendEmailWhenConfirmedPROnline", input);
      }
    }
    #endregion Function
  }
}
