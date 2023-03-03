/*
  Author      : Xuan Truong
  Date        : 15/06/2012
  Description : Transfer Location(Wood)
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_24_001 : MainUserControl
  {
    #region Field
    // Pid Location
    public long locationPid = long.MinValue;

    // Status
    private int status = 0;

    // Flag Update
    private bool canUpdate = false;

    #endregion Field

    #region Init

    public viewWHD_24_001()
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

      //Load Data
      this.LoadData();
    }

    /// <summary>
    /// Load UltraCombo Location
    /// </summary>
    private void LoadComboLocation()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_Position, Name";
      commandText += " FROM VWHDLocationWoods ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultLocation.DataSource = dtSource;
      ultLocation.DisplayMember = "Name";
      ultLocation.ValueMember = "ID_Position";
      ultLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLocation.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultLocation.DisplayLayout.Bands[0].Columns["ID_Position"].Hidden = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@TranLocationPid", DbType.Int64, this.locationPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDTranLocationInfoByTranLocationPidWoods_Select", inputParam);
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
      }
      else
      {
        DataTable dtLocation = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewTraLocationNoWoods('WTR') NewLocationNo");
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
      this.btnLoad.Enabled = this.canUpdate;
      this.chkComfirm.Enabled = this.canUpdate;
      this.btnSave.Enabled = this.canUpdate;
      this.btnAddMultiDetail.Enabled = this.canUpdate;
    }

    /// <summary>
    /// Load Data Detail Location
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataLocationDetail(DataSet dsSource)
    {
      this.ultData.DataSource = dsSource.Tables[1];

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        row.Activation = Activation.ActivateOnly;
      }
    }

    #endregion Init

    #region Event

    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLoad_Click(object sender, EventArgs e)
    {
      // Get path from Folder
      string path = @"\PhanmemDENSOBHT8000";
      path = Path.GetFullPath(path);

      string messageLocation = string.Empty;
      bool success = this.CheckValidLocation(out messageLocation);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", messageLocation);
        return;
      }

      DataTable dtSource = this.CreateDataTable();

      string curFile = path + @"\THONGTIN.txt";

      if (!File.Exists(curFile))
      {
        string message = string.Format(DaiCo.Shared.Utility.FunctionUtility.GetMessage("ERR0011"), "THONGTIN.txt");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string[] a = File.ReadAllLines(curFile);

      if (a.Length == 0)
      {
        return;
      }

      int index = int.MinValue;
      if (a[0].ToString().Length > 0)
      {
        index = a[0].IndexOf("*");
      }

      for (int i = 0; i < a.Length; i++)
      {
        if (a[i].Trim().ToString().Length > 0 && index != -1)
        {
          DataRow row = dtSource.NewRow();
          index = a[i].IndexOf("*");
          a[i] = a[i].Substring(0, index).Trim().ToString();
          row["IDVeneer"] = a[i].ToString().Trim();
          row["Code"] = "0";
          row["Pcs"] = 0;
          row["Width"] = 0;
          row["Length"] = 0;
          row["Thickness"] = 0;
          row["Location"] = "0";
          dtSource.Rows.Add(row);
        }
        else if (a[i].Trim().ToString().Length > 0)
        {
          DataRow row = dtSource.NewRow();
          row["IDVeneer"] = a[i].ToString().Trim();
          row["Code"] = "0";
          row["Pcs"] = 0;
          row["Width"] = 0;
          row["Length"] = 0;
          row["Thickness"] = 0;
          row["Location"] = "0";
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

        DataRow[] foundRows = dtMain.Select("IDVeneer = '" + dr["IDVeneer"].ToString() + "'");
        if (foundRows.Length > 0)
        {
          row["Errors"] = 1;
        }
        else
        {
          row["Errors"] = DBConvert.ParseInt(dr["Errors"].ToString());
        }
        row["IDVeneer"] = dr["IDVeneer"].ToString();
        row["MaterialCode"] = dr["MaterialCode"].ToString();
        row["NameEN"] = dr["NameEN"].ToString();
        row["NameVN"] = dr["NameVN"].ToString();
        row["TenDonViEN"] = dr["TenDonViEN"].ToString();
        row["Qty"] = DBConvert.ParseDouble(dr["Qty"].ToString());
        row["Width"] = DBConvert.ParseDouble(dr["Width"].ToString());
        row["Length"] = DBConvert.ParseDouble(dr["Length"].ToString());
        row["Thickness"] = DBConvert.ParseDouble(dr["Thickness"].ToString());
        row["TotalCBM"] = DBConvert.ParseDouble(dr["TotalCBM"].ToString());
        row["LocationTo"] = dr["LocationTo"].ToString();
        row["LocationFrom"] = dr["LocationFrom"].ToString();
        dtMain.Rows.Add(row);
      }

      this.ultData.DataSource = dtMain;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        row.Activation = Activation.ActivateOnly;

        if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
        {
          row.CellAppearance.BackColor = Color.Yellow;
        }
      }
    }

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
        WindowUtinity.ShowMessageError("ERR0001", message);
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

      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["LocationTo"].Header.Caption = "Package To";
      e.Layout.Bands[0].Columns["LocationFrom"].Header.Caption = "Package From";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "CBM";
      e.Layout.Bands[0].Columns["TenDonViEN"].Header.Caption = "Unit";
      e.Layout.Bands[0].Columns["IDVeneer"].Header.Caption = "ID Wood";

      e.Layout.Bands[0].Columns["IDVeneer"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["IDVeneer"].MinWidth = 70;

      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;

      e.Layout.Bands[0].Columns["LocationTo"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["LocationTo"].MinWidth = 90;

      e.Layout.Bands[0].Columns["LocationFrom"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["LocationFrom"].MinWidth = 90;

      e.Layout.Bands[0].Columns["TenDonViEN"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["TenDonViEN"].MinWidth = 80;

      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;

      e.Layout.Bands[0].Columns["Length"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Length"].MinWidth = 70;

      e.Layout.Bands[0].Columns["Width"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Width"].MinWidth = 70;

      e.Layout.Bands[0].Columns["Thickness"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Thickness"].MinWidth = 70;

      e.Layout.Bands[0].Columns["TotalCBM"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["TotalCBM"].MinWidth = 80;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.0000}";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
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

      if (this.locationPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
          {
            continue;
          }

          string lotNoId = row.Cells["IDVeneer"].Value.ToString();

          DBParameter[] inputParams = new DBParameter[2];
          inputParams[0] = new DBParameter("@TranLocationPid", DbType.Int64, this.locationPid);
          inputParams[1] = new DBParameter("@LotNoId", DbType.String, lotNoId);

          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          string storeName = string.Empty;
          storeName = "spWHDTranLocationDetailWoods_Delete";

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

      viewWHD_21_006 uc = new viewWHD_21_006();
      Shared.Utility.WindowUtinity.ShowView(uc, "ADD MULTI DETAIL", true, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);

      DataTable dtSource = this.CreateDataTable();

      if (uc.dtDetail != null && uc.dtDetail.Rows.Count > 0)
      {
        for (int i = 0; i < uc.dtDetail.Rows.Count; i++)
        {
          DataRow row = dtSource.NewRow();
          row["IDVeneer"] = uc.dtDetail.Rows[i]["IDWood"].ToString();
          row["Code"] = "0";
          row["Pcs"] = 0;
          row["Width"] = 0;
          row["Length"] = 0;
          row["Thickness"] = 0;
          row["Location"] = "0";
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

        DataRow[] foundRows = dtMain.Select("IDVeneer = '" + dr["IDVeneer"].ToString() + "'");
        if (foundRows.Length > 0)
        {
          row["Errors"] = 1;
        }
        else
        {
          row["Errors"] = DBConvert.ParseInt(dr["Errors"].ToString());
        }
        row["IDVeneer"] = dr["IDVeneer"].ToString();
        row["MaterialCode"] = dr["MaterialCode"].ToString();
        row["NameEN"] = dr["NameEN"].ToString();
        row["NameVN"] = dr["NameVN"].ToString();
        row["TenDonViEN"] = dr["TenDonViEN"].ToString();
        row["Qty"] = DBConvert.ParseDouble(dr["Qty"].ToString());
        row["Width"] = DBConvert.ParseDouble(dr["Width"].ToString());
        row["Length"] = DBConvert.ParseDouble(dr["Length"].ToString());
        row["Thickness"] = DBConvert.ParseDouble(dr["Thickness"].ToString());
        row["TotalCBM"] = DBConvert.ParseDouble(dr["TotalCBM"].ToString());
        row["LocationTo"] = dr["LocationTo"].ToString();
        row["LocationFrom"] = dr["LocationFrom"].ToString();
        dtMain.Rows.Add(row);
      }

      this.ultData.DataSource = dtMain;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        row.Activation = Activation.ActivateOnly;

        if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) != 0)
        {
          row.CellAppearance.BackColor = Color.Yellow;
        }
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
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM VWHDLocationWoods WHERE ID_Position = '" + this.ultLocation.Value.ToString() + "'";
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
      SqlCommand cm = new SqlCommand("spWHDGetDataTranLocationWoods_Select");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      cm.Parameters.Add(para);

      // Lication Pid
      para = cm.CreateParameter();
      para.ParameterName = "@LocationPid";
      para.DbType = DbType.Int64;
      para.Value = DBConvert.ParseLong(this.ultLocation.Value.ToString());

      cm.Parameters.Add(para);

      SqlDataAdapter adp = new SqlDataAdapter();
      adp.SelectCommand = cm;
      DataSet result = new DataSet();
      try
      {
        if (cm.Connection.State != ConnectionState.Open)
        {
          cm.Connection.Open();
        }
        adp.Fill(result);
      }
      catch (Exception ex)
      {
        result = null;
        return null;
      }

      dt = result.Tables[0];
      return dt;
    }

    /// <summary>
    /// Create DataTable
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();

      dt.Columns.Add("IDVeneer", typeof(System.String));
      dt.Columns.Add("Code", typeof(System.String));
      dt.Columns.Add("Pcs", typeof(System.Double));
      dt.Columns.Add("Width", typeof(System.Double));
      dt.Columns.Add("Length", typeof(System.Double));
      dt.Columns.Add("Thickness", typeof(System.Double));
      dt.Columns.Add("Location", typeof(System.String));

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
      result = this.SaveTranLocationVeneer();

      if (!result)
      {
        return false;
      }

      // Save Location Veneer Detail 
      result = this.SaveTranLocationDetailVeneer();

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
      if (this.locationPid != long.MinValue)
      {
        storeName = "spWHDTranLocationWoods_Update";
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
          inputParamUpdate[2] = new DBParameter("@Status", DbType.Int32, 0);
        }

        // Remark
        inputParamUpdate[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);

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
        inputParamInsert[3] = new DBParameter("@TraLocationNo", DbType.String, "WTR");

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
    private bool SaveTranLocationDetailVeneer()
    {
      // Create dt Before Save
      DataTable dtSource = this.CreateDataTableBeforeSave();

      // Get Main Data Location Detail
      DataTable dtMain = (DataTable)this.ultData.DataSource;

      foreach (DataRow drMain in dtMain.Rows)
      {
        // Get Row is Added and no Errors
        // 
        if (drMain.RowState == DataRowState.Added && DBConvert.ParseDouble(drMain["Errors"].ToString()) == 0)
        {
          DataRow row = dtSource.NewRow();
          row["LotNoId"] = drMain["IDVeneer"].ToString();
          row["Qty"] = DBConvert.ParseDouble(drMain["Qty"].ToString());
          row["Width"] = DBConvert.ParseDouble(drMain["Width"].ToString());
          row["Length"] = DBConvert.ParseDouble(drMain["Length"].ToString());
          row["Thickness"] = DBConvert.ParseDouble(drMain["Thickness"].ToString());
          row["LocationFrom"] = drMain["LocationFrom"].ToString();
          row["LocationTo"] = drMain["LocationTo"].ToString();
          dtSource.Rows.Add(row);
        }
      }

      SqlCommand cm = new SqlCommand("spWHDTranLocationDetailWoods_Insert");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      cm.Parameters.Add(para);

      // Location Pid
      para = cm.CreateParameter();
      para.ParameterName = "@TranLocationPid";
      para.DbType = DbType.Int64;
      para.Value = this.locationPid;

      cm.Parameters.Add(para);

      try
      {
        if (cm.Connection.State != ConnectionState.Open)
        {
          cm.Connection.Open();
        }
        cm.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        string a = ex.Message;
        return false;
      }
      finally
      {
        if (cm.Connection.State != ConnectionState.Closed)
        {
          cm.Connection.Close();
        }
        cm.Connection.Dispose();
        cm.Dispose();
      }

      return true;
    }

    /// <summary>
    /// Create DataTable Before Save
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTableBeforeSave()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("LotNoId", typeof(System.String));
      dt.Columns.Add("Width", typeof(System.Double));
      dt.Columns.Add("Length", typeof(System.Double));
      dt.Columns.Add("Thickness", typeof(System.Double));
      dt.Columns.Add("LocationFrom", typeof(System.String));
      dt.Columns.Add("LocationTo", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      return dt;
    }
    #endregion Process
  }
}
