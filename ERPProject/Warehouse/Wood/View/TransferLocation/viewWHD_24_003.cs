/*
  Author      : Duong Minh
  Date        : 27/03/2013
  Description : Transfer Location(Wood) (Choose Package)
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_24_003 : MainUserControl
  {
    #region Field
    // Pid Location
    public long tranLocationPid = long.MinValue;

    // Status
    private int status = 0;

    // Flag Update
    private bool canUpdate = false;

    #endregion Field

    #region Init

    public viewWHD_24_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewWHD_24_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_24_001_Load(object sender, EventArgs e)
    {
      //Load UltraCombo Location
      this.LoadComboLocation();

      //Load UltraCombo Package
      this.LoadComboPackage();

      //Load Data
      this.LoadData();
    }

    /// <summary>
    /// Load UltraCombo Package
    /// </summary>
    private void LoadComboPackage()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, Name ";
      commandText += " FROM TblWHDPackage ";
      commandText += " WHERE PositionPid != -1 ";
      commandText += " ORDER BY Name ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultPackage.DataSource = dtSource;
      ultPackage.DisplayMember = "Name";
      ultPackage.ValueMember = "Pid";
      ultPackage.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultPackage.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultPackage.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }
    /// <summary>
    /// Load UltraCombo Location
    /// </summary>
    private void LoadComboLocation()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, Name ";
      commandText += " FROM TblWHDPosition ";
      commandText += " WHERE WarehousePid = 3 ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultLocation.DataSource = dtSource;
      ultLocation.DisplayMember = "Name";
      ultLocation.ValueMember = "Pid";
      ultLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLocation.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultLocation.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@TranLocationPid", DbType.Int64, this.tranLocationPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDTranLocationInfoByTranLocationPidPackageWoods_Select", inputParam);
      DataTable dtLocationInfo = dsSource.Tables[0];

      if (dtLocationInfo.Rows.Count > 0)
      {
        DataRow row = dtLocationInfo.Rows[0];

        this.txtTrNo.Text = row["TrNo"].ToString();

        this.status = DBConvert.ParseInt(row["Status"].ToString());

        this.txtRemark.Text = row["Remark"].ToString();
        this.txtCreateBy.Text = row["CreateBy"].ToString();
        this.txtCreateDate.Text = row["CreateDate"].ToString();

        if (this.status == 1)
        {
          this.chkComfirm.Checked = true;
        }

        if (dsSource.Tables[1].Rows.Count > 0)
        {
          this.ultLocation.Value = DBConvert.ParseLong(dsSource.Tables[1].Rows[0][2].ToString());
        }
      }
      else
      {
        DataTable dtLocation = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewTraLocationNoWoods('WPR') NewLocationNo");
        if ((dtLocation != null) && (dtLocation.Rows.Count > 0))
        {
          this.txtTrNo.Text = dtLocation.Rows[0]["NewLocationNo"].ToString();
          this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
        }
      }
      this.SetStatusControl();

      // Load Data Detail Location
      this.LoadDataLocationDetail(dsSource);
    }

    /// <summary>
    /// SetStatusControl
    /// </summary>
    private void SetStatusControl()
    {
      this.canUpdate = (btnSave.Visible && this.status == 0);

      if (!this.canUpdate)
      {
        this.ultLocation.ReadOnly = true;
        this.txtRemark.ReadOnly = true;
      }
      this.chkComfirm.Enabled = this.canUpdate;
      this.btnSave.Enabled = this.canUpdate;
    }

    /// <summary>
    /// Load Data Detail Location
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataLocationDetail(DataSet dsSource)
    {
      this.ultData.DataSource = dsSource.Tables[1];
    }

    #endregion Init

    #region Event
    /// <summary>
    /// Save Location
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      DataTable dtMain = (DataTable)this.ultData.DataSource;
      if (dtMain == null || dtMain.Rows.Count == 0)
      {
        return;
      }

      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Format Gird
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPidTo"].Hidden = true;
      e.Layout.Bands[0].Columns["Name"].ValueList = this.ultPackage;

      e.Layout.Bands[0].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
    }

    /// <summary>
    /// Delete Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.tranLocationPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          DBParameter[] inputParams = new DBParameter[1];
          inputParams[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));

          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          string storeName = string.Empty;
          storeName = "spWHDTranPackageDetailWoods_Delete";

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadData();
            return;
          }
        }
      }
    }
    #endregion Event

    #region Process
    /// <summary>
    /// Check valid before Load
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dt = new DataTable();
      // Detail
      DataTable dtMain = (DataTable)this.ultData.DataSource;

      if (this.ultLocation.Value == null ||
          DBConvert.ParseLong(this.ultLocation.Value.ToString()) == long.MinValue)
      {
        message = "Location is invalid";
        return false;
      }

      foreach (DataRow drMain in dtMain.Rows)
      {
        if (drMain.RowState != DataRowState.Deleted)
        {
          // Check exist
          commandText = string.Empty;
          commandText += " SELECT COUNT(*) FROM TblWHDPackage WHERE PositionPid <> -1 AND Pid =" + DBConvert.ParseInt(drMain["Name"].ToString());
          dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt == null || DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 0)
          {
            message = "Package is invalid";
            return false;
          }

          // Check Duplicate
          DataRow[] foundRow = dtMain.Select("Name =" + DBConvert.ParseInt(drMain["Name"].ToString()));
          if (foundRow.Length > 1)
          {
            message = "Duplicate Package is invalid";
            return false;
          }

        }
      }
      return true;
    }

    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;

      //Master Information Location Veneer
      result = this.SaveTranLocationVeneer();

      if (!result)
      {
        return false;
      }

      // Save Location Veneer Detail 
      result = this.SaveTranLocationDetailVeneer();
      if (!result)
      {
        return false;
      }

      if (this.chkComfirm.Checked)
      {
        // Save Update Package
        result = this.SaveUpdatePackage();
      }

      return result;
    }

    /// <summary>
    /// Save Master Information Location Veneer
    /// </summary>
    /// <returns></returns>
    private bool SaveTranLocationVeneer()
    {
      string storeName = string.Empty;

      // Update
      if (this.tranLocationPid != long.MinValue)
      {
        storeName = "spWHDTranLocationWoods_Update";
        DBParameter[] inputParamUpdate = new DBParameter[4];

        // Pid
        inputParamUpdate[0] = new DBParameter("@PID", DbType.Int64, this.tranLocationPid);

        // Status
        if (this.chkComfirm.Checked)
        {
          inputParamUpdate[1] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamUpdate[2] = new DBParameter("@Status", DbType.Int32, 0);
        }

        // Remark
        inputParamUpdate[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);

        DBParameter[] outputParamUpdate = new DBParameter[1];
        outputParamUpdate[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamUpdate, outputParamUpdate);

        long resultPid = DBConvert.ParseLong(outputParamUpdate[0].Value.ToString());
        this.tranLocationPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
      }
      // Insert
      else
      {
        storeName = "spWHDTranLocationWoods_Insert";
        DBParameter[] inputParamInsert = new DBParameter[4];

        // Status
        if (this.chkComfirm.Checked)
        {
          inputParamInsert[0] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamInsert[0] = new DBParameter("@Status", DbType.Int32, 0);
        }

        // Remark
        inputParamInsert[1] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);

        // CreateBy
        inputParamInsert[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        // TrNo
        inputParamInsert[3] = new DBParameter("@TraLocationNo", DbType.String, "WPR");

        DBParameter[] outputParamInsert = new DBParameter[1];
        outputParamInsert[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);

        long resultPid = DBConvert.ParseLong(outputParamInsert[0].Value.ToString());
        this.tranLocationPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Update Package
    /// </summary>
    /// <returns></returns>
    private bool SaveUpdatePackage()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@TrNoPid", DbType.Int64, this.tranLocationPid);

      DataBaseAccess.ExecuteStoreProcedure("spWHDUpdatePackage_Update", inputParam);
      return true;
    }

    /// <summary>
    /// Save Location Veneer Detail 
    /// </summary>
    /// <returns></returns>
    private bool SaveTranLocationDetailVeneer()
    {
      string storeName = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] inputParamInsert = new DBParameter[4];
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
        {
          // Insert
          inputParamInsert[0] = new DBParameter("@Pid", DbType.Int64,
                                DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
        }

        inputParamInsert[1] = new DBParameter("@TrNoPid", DbType.Int64, this.tranLocationPid);
        inputParamInsert[2] = new DBParameter("@PackagePid", DbType.Int64,
                                DBConvert.ParseLong(row.Cells["Name"].Value.ToString()));
        inputParamInsert[3] = new DBParameter("@LocationPidTo", DbType.Int64,
                                DBConvert.ParseLong(this.ultLocation.Value.ToString()));

        DBParameter[] outputParamInsert = new DBParameter[1];
        outputParamInsert[0] = new DBParameter("@Result", DbType.Int32, long.MinValue);

        storeName = "spWHDTranPackageDetail_Edit";
        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);
        if (DBConvert.ParseLong(outputParamInsert[0].Value.ToString()) == long.MinValue)
        {
          return false;
        }
      }
      return true;
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      switch (columnName)
      {
        case "Name":
          string commandText = string.Empty;
          commandText += " SELECT COUNT(*) ";
          commandText += " FROM TblWHDPackage  ";
          commandText += " WHERE PositionPid <> -1 ";
          commandText += "    AND Name ='" + e.Cell.Row.Cells["Name"].Text + "'";

          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);

          if (dtCheck == null || DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Package");
            e.Cancel = true;
            break;
          }

          break;
        default:
          break;
      }

    }
    #endregion Process
  }
}
