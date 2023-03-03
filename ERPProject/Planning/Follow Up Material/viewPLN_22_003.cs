/*
  Author      : 
  Date        : 19/07/2013
  Description : Make PR Online
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
  public partial class viewPLN_22_003 : MainUserControl
  {
    #region Field
    public DataTable dtSource = new DataTable();
    private long prOnlinePid = long.MinValue;
    #endregion Field

    #region Init
    public viewPLN_22_003()
    {
      InitializeComponent();
    }

    private void viewPLN_22_003_Load(object sender, EventArgs e)
    {
      this.LoadDropDownProjectCode();
      this.LoadDropDownUrgentLevel();

      ultData.DataSource = dtSource;
    }
    #endregion Init

    #region Function

    private void LoadDropDownProjectCode()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7008 AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultddProjectCode.DataSource = dtSource;
      ultddProjectCode.DisplayMember = "Name";
      ultddProjectCode.ValueMember = "Code";
      ultddProjectCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultddProjectCode.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultddProjectCode.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void LoadDropDownUrgentLevel()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7007 AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultddUrgent.DataSource = dtSource;
      ultddUrgent.DisplayMember = "Name";
      ultddUrgent.ValueMember = "Code";
      ultddUrgent.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultddUrgent.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultddUrgent.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private bool CheckValid(out String message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseDouble(row.Cells["PRQty"].Value.ToString()) <= 0)
        {
          message = "PRQty";
          return false;
        }

        if (row.Cells["RequestDate"].Value.ToString().Length == 0)
        {
          message = "RequestDate";
          return false;
        }

        if (row.Cells["Urgent"].Text.Length == 0 ||
            DBConvert.ParseInt(row.Cells["Urgent"].Value.ToString()) <= 0)
        {
          message = "Urgent";
          return false;
        }

        if (row.Cells["ProjectCode"].Text.Length > 0 &&
             DBConvert.ParseInt(row.Cells["ProjectCode"].Value.ToString()) <= 0)
        {
          message = "ProjectCode";
          return false;
        }
      }
      return true;
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
      DBParameter[] input = new DBParameter[5];
      input[0] = new DBParameter("@RequestBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      input[1] = new DBParameter("@Department", DbType.String, SharedObject.UserInfo.Department);
      input[2] = new DBParameter("@Status", DbType.Int32, 0);
      input[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      input[4] = new DBParameter("@PROnlineNo", DbType.String, "PROL");

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNMakePROnlineInformation_Insert", input, output);
      this.prOnlinePid = DBConvert.ParseLong(output[0].Value.ToString());
      if (this.prOnlinePid <= 0)
      {
        return false;
      }
      return true;
    }

    private bool SaveDetail()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] input = new DBParameter[7];
        input[0] = new DBParameter("@PROnlinePid", DbType.Int64, this.prOnlinePid);
        input[1] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MaterialCode"].Value.ToString());
        input[2] = new DBParameter("@Status", DbType.Int32, 0);
        input[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["PRQty"].Value.ToString()));
        input[4] = new DBParameter("@RequestDate", DbType.DateTime, (DateTime)row.Cells["RequestDate"].Value);
        if (DBConvert.ParseInt(row.Cells["Urgent"].Value.ToString()) != int.MinValue)
        {
          input[5] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseInt(row.Cells["Urgent"].Value.ToString()));
        }
        if (DBConvert.ParseInt(row.Cells["ProjectCode"].Value.ToString()) != int.MinValue)
        {
          input[6] = new DBParameter("@ProjectCode", DbType.Int32, DBConvert.ParseInt(row.Cells["ProjectCode"].Value.ToString()));
        }

        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNMakePROnlineDetail_Insert", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      Utility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Columns["MaterialGroup"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["GroupName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ReOrder"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ReOrder"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["PRQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Urgent"].ValueList = ultddUrgent;
      e.Layout.Bands[0].Columns["ProjectCode"].ValueList = ultddProjectCode;
      e.Layout.Bands[0].Columns["RequestDate"].Format = ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[0].Columns["PRQty"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Urgent"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ProjectCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["RequestDate"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      success = this.SaveData();
      if (success)
      {
        this.CloseTab();
        viewPUR_21_002 uc = new viewPUR_21_002();
        uc.PRPid = this.prOnlinePid;
        WindowUtinity.ShowView(uc, "UPDATE PR ONLINE", true, ViewState.MainWindow);
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
