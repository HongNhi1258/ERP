/*
  Author      : Xuan Truong
  Date        : 23/10/2012
  Description : Transfer Location(Materials)
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
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_07_001 : MainUserControl
  {
    #region Field
    private int whPid = 0;
    // Pid Location
    public long locationPid = long.MinValue;

    // Status
    private int status = 0;

    // Flag Update
    private bool canUpdate = false;

    #endregion Field

    #region Init

    public viewWHD_07_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewWHD_07_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_07_001_Load(object sender, EventArgs e)
    {
      Utility.LoadUltraCBMaterialWHListByUser(ucbWarehouse);
      //Load Data
      this.LoadData();
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@TranLocationPid", DbType.Int64, this.locationPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDTransferLocationMaterials_Select", inputParam);
      DataTable dtLocationInfo = dsSource.Tables[0];

      if (dtLocationInfo.Rows.Count > 0)
      {
        DataRow row = dtLocationInfo.Rows[0];
        this.txtTrNo.Text = row["TrNo"].ToString();
        this.status = DBConvert.ParseInt(row["Status"].ToString());
        this.txtRemark.Text = row["Remark"].ToString();
        this.txtCreateBy.Text = row["CreateBy"].ToString();
        this.txtCreateDate.Text = row["CreateDate"].ToString();
        this.whPid = DBConvert.ParseInt(row["WarehousePid"]);
        ucbWarehouse.Value = this.whPid;
        if (this.status == 1)
        {
          this.chkComfirm.Checked = true;
        }
      }
      else
      {
        DataTable dtLocation = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewTraLocationNoMaterials('03MTR') NewLocationNo");
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
        ultLocation.ReadOnly = true;
        txtRemark.ReadOnly = true;
        ucbWarehouse.ReadOnly = true;
      }
      chkComfirm.Enabled = this.canUpdate;
      btnSave.Enabled = this.canUpdate;
      btnAddMultiDetail.Enabled = this.canUpdate;
    }

    /// <summary>
    /// Load Data Detail Location
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataLocationDetail(DataSet dsSource)
    {
      ultData.DataSource = dsSource.Tables[1];
      if (dsSource.Tables[0].Rows.Count > 0)
      {
        int status = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["Status"].ToString());
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        for (int j = 0; j < row.Cells.Count - 1; j++)
        {
          row.Cells[j].Activation = Activation.ActivateOnly;
        }
        if (status == 1)
        {
          row.Activation = Activation.ActivateOnly;
        }
      }
      if (ultData.Rows.Count > 0)
      {
        ucbWarehouse.ReadOnly = true;
      }
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
        if (message.Length > 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", message);
        }
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

      e.Layout.Bands[0].Columns["Errors"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["TranNoPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFromPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationToPid"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["LocationTo"].Header.Caption = "Location To";
      e.Layout.Bands[0].Columns["LocationFrom"].Header.Caption = "Location From";
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //Sum Qty
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;

      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      //e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
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

      if (this.locationPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
          {
            continue;
          }

          DBParameter[] inputParams = new DBParameter[1];
          inputParams[0] = new DBParameter("@TranLocationPid", DbType.Int64, this.locationPid);

          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          string storeName = string.Empty;
          storeName = "spWHDTranLocationDetailMaterials_Delete";

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

    /// <summary>
    /// Add Multi Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddMultiDetail_Click(object sender, EventArgs e)
    {
      // Check Valid
      string messageLocation = string.Empty;
      bool success = this.CheckValidLocation(out messageLocation);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", messageLocation);
        return;
      }

      viewWHD_07_003 uc = new viewWHD_07_003();
      uc.whPid = this.whPid;
      Shared.Utility.WindowUtinity.ShowView(uc, "ADD MULTI DETAIL", true, Shared.Utility.ViewState.ModalWindow, FormWindowState.Normal);

      DataTable dtSource = this.CreateDataTable();

      if (uc.dtDetail != null && uc.dtDetail.Rows.Count > 0)
      {
        for (int i = 0; i < uc.dtDetail.Rows.Count; i++)
        {
          DataRow row = dtSource.NewRow();
          row["MaterialCode"] = uc.dtDetail.Rows[i]["MaterialCode"].ToString();
          row["LocationFrom"] = DBConvert.ParseLong(uc.dtDetail.Rows[i]["LocationPid"].ToString());
          dtSource.Rows.Add(row);
        }
      }

      //Get Data
      DataTable result = this.GetDataLoad(dtSource);
      if (result == null)
      {
        return;
      }
      DataTable dtMain = (DataTable)this.ultData.DataSource;
      // Loop for datatable get from SQL
      foreach (DataRow dr in result.Rows)
      {
        DataRow row = dtMain.NewRow();

        DataRow[] foundRows = dtMain.Select("MaterialCode = '" + dr["MaterialCode"].ToString() + "'" + "AND LocationFromPid = " + DBConvert.ParseInt(dr["LocationFromPid"].ToString()));
        if (foundRows.Length > 0)
        {
          row["Errors"] = 1;
        }
        else
        {
          row["Errors"] = DBConvert.ParseInt(dr["Errors"].ToString());
        }
        row["MaterialCode"] = dr["MaterialCode"].ToString();
        row["NameEN"] = dr["NameEN"].ToString();
        row["NameVN"] = dr["NameVN"].ToString();
        row["Unit"] = dr["Unit"].ToString();
        row["Qty"] = DBConvert.ParseDouble(dr["Qty"].ToString());
        row["LocationTo"] = dr["LocationTo"].ToString();
        row["LocationFrom"] = dr["LocationFrom"].ToString();
        row["LocationToPid"] = DBConvert.ParseLong(dr["LocationToPid"].ToString());
        row["LocationFromPid"] = DBConvert.ParseLong(dr["LocationFromPid"].ToString());
        dtMain.Rows.Add(row);
      }

      this.ultData.DataSource = dtMain;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        for (int j = 0; j < row.Cells.Count - 1; j++)
        {
          row.Cells[j].Activation = Activation.ActivateOnly;
        }
        //row.Activation = Activation.ActivateOnly;

        if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
        {
          row.CellAppearance.BackColor = Color.Yellow;
        }
      }
      if (ultData.Rows.Count > 0)
      {
        ucbWarehouse.ReadOnly = true;
      }
    }
    #endregion Event

    #region Process    
    /// <summary>
    /// Check valid before Load
    /// </summary>
    /// <returns></returns>
    private bool CheckValidLocation(out string message)
    {
      message = string.Empty;
      // Master Information
      if (this.ultLocation.Value != null && this.ultLocation.Value.ToString().Length > 0)
      {
        string commandText = string.Format("SELECT COUNT(*) FROM TblWHDPosition WHERE Pid = {0} AND WarehousePid = {1}", ultLocation.Value, this.whPid);
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Location";
            return false;
          }
        }
        else
        {
          message = "Location";
          return false;
        }
      }
      else
      {
        message = "Location";
        return false;
      }
      return true;
    }

    /// <summary>
    /// Check valid before Load
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      if (this.locationPid == long.MinValue)
      {
        // Check WH Summary of preMonth
        if (!Utility.CheckWHPreMonthSummary(this.whPid))
        {
          return false;
        }
      }
      if (ucbWarehouse.SelectedRow == null)
      {
        message = "Warehouse";
        return false;
      }
      // Detail
      DataTable dtMain = (DataTable)this.ultData.DataSource;
      foreach (DataRow drMain in dtMain.Rows)
      {
        if (drMain.RowState != DataRowState.Deleted)
        {
          //Errors
          if (DBConvert.ParseInt(drMain["Errors"].ToString()) != 0)
          {
            message = "Data Input";
            return false;
          }
          double qtyTransfer = DBConvert.ParseDouble(drMain["Qty"].ToString());
          int location = DBConvert.ParseInt(drMain["LocationFromPid"].ToString());
          string mamterialCode = drMain["MaterialCode"].ToString();
          string commandText = string.Format(@"SELECT Qty
                                                FROM TblWHDMaterialStockBalanceLocation
                                                WHERE StockCode = '{0}' AND IDPosition = {1} AND IDKho = {2}", mamterialCode, location, this.whPid);
          DataTable dtCheckQty = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheckQty != null && dtCheckQty.Rows.Count > 0)
          {
            double qty = DBConvert.ParseDouble(dtCheckQty.Rows[0]["Qty"].ToString());
            if (qtyTransfer > qty || qtyTransfer < 0)
            {
              message = "Material Code : " + mamterialCode + ":  0 < Qty <= " + qty;
              return false;
            }
          }
          else
          {
            message = "Qty";
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Get Data When Load Data
    /// </summary>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private DataTable GetDataLoad(DataTable dtSource)
    {
      DataTable dt = new DataTable();
      SqlDBParameter[] input = new SqlDBParameter[3];
      input[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtSource);
      input[1] = new SqlDBParameter("@LocationPid", SqlDbType.BigInt, DBConvert.ParseLong(this.ultLocation.Value.ToString()));
      input[2] = new SqlDBParameter("@WarehousePid", SqlDbType.Int, this.whPid);
      dt = SqlDataBaseAccess.SearchStoreProcedureDataTable("spWHDGetDataTranLocationMaterials_Select", input);
      return dt;
    }

    /// <summary>
    /// Create DataTable
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Pid", typeof(System.Int64));
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("LocationFrom", typeof(System.Int64));
      dt.Columns.Add("LocationTo", typeof(System.Int64));
      return dt;
    }

    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;

      //Master Information Location Veneer
      result = this.SaveTranLocation();

      if (!result)
      {
        return false;
      }

      // Save Location Veneer Detail 
      result = this.SaveTranLocationDetail();

      return result;
    }

    /// <summary>
    /// Save Master Information Location Veneer
    /// </summary>
    /// <returns></returns>
    private bool SaveTranLocation()
    {
      string storeName = string.Empty;

      // Update
      if (this.locationPid != long.MinValue)
      {
        storeName = "spWHDTranLocationMaterials_Update";
        DBParameter[] inputParamUpdate = new DBParameter[4];

        // Pid
        inputParamUpdate[0] = new DBParameter("@PID", DbType.Int64, this.locationPid);

        // Status
        if (this.chkComfirm.Checked)
        {
          inputParamUpdate[1] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamUpdate[1] = new DBParameter("@Status", DbType.Int32, 0);
        }
        // Remark
        inputParamUpdate[2] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);
        inputParamUpdate[3] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);

        DBParameter[] outputParamUpdate = new DBParameter[1];
        outputParamUpdate[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamUpdate, outputParamUpdate);

        long resultPid = DBConvert.ParseLong(outputParamUpdate[0].Value.ToString());
        this.locationPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
      }
      // Insert
      else
      {
        storeName = "spWHDTranLocationMaterials_Insert";
        DBParameter[] inputParamInsert = new DBParameter[5];

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
        inputParamInsert[3] = new DBParameter("@TraLocationNo", DbType.String, "03MTR");
        inputParamInsert[4] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);

        DBParameter[] outputParamInsert = new DBParameter[1];
        outputParamInsert[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);

        long resultPid = DBConvert.ParseLong(outputParamInsert[0].Value.ToString());
        this.locationPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Location Veneer Detail 
    /// </summary>
    /// <returns></returns>
    private bool SaveTranLocationDetail()
    {
      // Create dt Before Save
      DataTable dtSource = this.CreateDataTableBeforeSave();

      // Get Main Data Location Detail
      DataTable dtMain = (DataTable)this.ultData.DataSource;

      foreach (DataRow drMain in dtMain.Rows)
      {
        // Get Row is Added and no Errors
        // 
        if ((drMain.RowState == DataRowState.Added || drMain.RowState == DataRowState.Modified) && DBConvert.ParseDouble(drMain["Errors"].ToString()) == 0)
        {
          DataRow row = dtSource.NewRow();
          if (DBConvert.ParseLong(drMain["Pid"].ToString()) != long.MinValue)
          {
            row["Pid"] = DBConvert.ParseLong(drMain["Pid"].ToString());
          }
          row["MaterialCode"] = drMain["MaterialCode"].ToString();
          row["Qty"] = DBConvert.ParseDouble(drMain["Qty"].ToString());
          row["LocationFrom"] = DBConvert.ParseLong(drMain["LocationFromPid"].ToString());
          row["LocationTo"] = DBConvert.ParseLong(drMain["LocationToPid"].ToString());
          dtSource.Rows.Add(row);
        }
      }
      SqlDBParameter[] input = new SqlDBParameter[2];
      input[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtSource);
      input[1] = new SqlDBParameter("@TranLocationPid", SqlDbType.BigInt, this.locationPid);
      return SqlDataBaseAccess.ExecuteStoreProcedure("spWHDTranLocationDetailMaterials_Insert", input);

    }

    /// <summary>
    /// Create DataTable Before Save
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTableBeforeSave()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Pid", typeof(System.Int64));
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("LocationFrom", typeof(System.Int64));
      dt.Columns.Add("LocationTo", typeof(System.Int64));
      return dt;
    }

    private void ucbWarehouse_ValueChanged(object sender, EventArgs e)
    {
      if (ucbWarehouse.SelectedRow != null)
      {
        this.whPid = DBConvert.ParseInt(ucbWarehouse.Value);
      }
      else
      {
        this.whPid = 0;
      }
      Utility.LoadUltraCBLocationListByWHPid(ultLocation, this.whPid);
      ultLocation.Value = null;
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      bool isReadOnly = (ultData.Rows.Count > 0 ? true : false);
      ucbWarehouse.ReadOnly = isReadOnly;
    }
    #endregion Process
  }
}
