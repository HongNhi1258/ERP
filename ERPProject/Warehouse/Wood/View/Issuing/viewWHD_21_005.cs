/*
  Author      : Duong Minh
  Date        : 14/06/2012
  Description : Insert/Update Issue For Subcon
*/

using DaiCo.Application;
using DaiCo.ERPProject.DataSetSource.Woods;
using DaiCo.ERPProject.Reports.Woods;
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
  public partial class viewWHD_21_005 : MainUserControl
  {
    #region Field
    // Pid Issuing
    public long issuingPid = long.MinValue;

    // Status
    private int status = 0;

    // Flag Update
    private bool canUpdate = false;
    private string printerName;
    private int whPid = 3;
    #endregion Field

    #region Init
    public viewWHD_21_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewVEN_02_002_Load(object sender, EventArgs e)
    {
      if (this.issuingPid == long.MinValue)
      {
        // Check WH Summary of preMonth
        bool result = Utility.CheckWHPreMonthSummary(this.whPid);
        if (result == false)
        {
          this.CloseTab();
          return;
        }
      }

      // Load UltraCombo Supplier
      this.LoadComboSupplier();

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = "SELECT ID_NhaCC, ISNULL(TenNhaCCEN, '') AS Name FROM VWHDSupplier";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultSupplier.DataSource = dtSource;
      ultSupplier.DisplayMember = "Name";
      ultSupplier.ValueMember = "ID_NhaCC";
      ultSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSupplier.DisplayLayout.Bands[0].Columns["Name"].Width = 450;
      ultSupplier.DisplayLayout.Bands[0].Columns["ID_NhaCC"].Hidden = true;
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@IssuingPid", DbType.Int64, this.issuingPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDIssuingInformationByIssuingPidWoods_Select", inputParam);
      DataTable dtReceivingInfo = dsSource.Tables[0];

      if (dtReceivingInfo.Rows.Count > 0)
      {
        DataRow row = dtReceivingInfo.Rows[0];

        this.txtIssuingNote.Text = row["IssuingCode"].ToString();
        this.txtTitle.Text = row["Title"].ToString();

        this.status = DBConvert.ParseInt(row["Status"].ToString());

        this.ultSupplier.Value = row["IssuingSource"].ToString();

        this.txtRemark.Text = row["Remark"].ToString();
        this.txtCreateBy.Text = row["CreateBy"].ToString();
        this.txtDate.Text = row["CreateDate"].ToString();

        if (this.status == 1)
        {
          this.chkComfirm.Checked = true;
        }
      }
      else
      {
        DataTable dtReceiving = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewIssuingNoWoods('05ITS') NewIssuingNo");
        if ((dtReceiving != null) && (dtReceiving.Rows.Count > 0))
        {
          this.txtIssuingNote.Text = dtReceiving.Rows[0]["NewIssuingNo"].ToString();
          this.txtDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
        }
      }
      this.SetStatusControl();

      // Load Data Detail Issuing Detail
      this.LoadDataIssuingDetail(dsSource);

      // Set Focus
      //this.txtTitle.Focus();
    }

    /// <summary>
    /// SetStatusControl
    /// </summary>
    private void SetStatusControl()
    {
      this.canUpdate = (btnSave.Visible && this.status == 0);

      this.ultSupplier.Enabled = this.canUpdate;
      this.txtTitle.Enabled = this.canUpdate;
      this.txtRemark.Enabled = this.canUpdate;

      this.chkComfirm.Enabled = this.canUpdate;
      this.btnSave.Enabled = this.canUpdate;
      this.btnPrintPreview.Enabled = !this.canUpdate;
      this.btnPrint.Enabled = !this.canUpdate;
      this.btnLoad.Enabled = this.canUpdate;
      this.btnAddMultiDetail.Enabled = this.canUpdate;
    }

    /// <summary>
    /// Load Data Detail Issuing
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataIssuingDetail(DataSet dsSource)
    {
      this.ultReceipt.DataSource = dsSource.Tables[1];
      this.ultSummary.DataSource = dsSource.Tables[2];
      for (int i = 0; i < ultReceipt.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultReceipt.Rows[i];

        rowGrid.Cells["LotNoId"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["MaterialCode"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["NameEN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["NameVN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["TenDonViEN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Qty"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Length"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Width"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Thickness"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["TotalCBM"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Location"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["IssueQty"].Activation = Activation.ActivateOnly;
      }
    }
    #endregion Init

    #region Event
    /// <summary>
    /// Key Up && Down==> For input data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultReceipt_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
      {
        return;
      }

      int rowIndex = (e.KeyCode == Keys.Down) ? ultReceipt.ActiveCell.Row.Index + 1 : ultReceipt.ActiveCell.Row.Index - 1;
      int cellIndex = ultReceipt.ActiveCell.Column.Index;

      try
      {
        ultReceipt.Rows[rowIndex].Cells[cellIndex].Activate();
        ultReceipt.PerformAction(UltraGridAction.EnterEditMode, false, false);
      }
      catch { }
    }

    /// <summary>
    /// Check Valid Issue Qty
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultReceipt_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      switch (columnName)
      {
        case "LotNoId":
          string lotnoId = e.Cell.Row.Cells["LotNoId"].Value.ToString();
          string commandText = string.Empty;
          commandText += @"SELECT DISTINCT CASE WHEN SB.LotNoId IS NULL THEN 1 ELSE 0 END Errors,
                                0 PID, SB.LotNoId LotNoId, SB.MaterialCode, MAT.TenEnglish NameEN,
                                 MAT.TenVietNam NameVN, UNI.TenDonViEN, ISNULL(SB.Qty, 0) Qty, ISNULL(DIM.[Length], 0) [Length],
                                 ISNULL(DIM.Width, 0) Width, ISNULL(DIM.Thickness, 0) Thickness, 
                                 (ISNULL(SB.Qty, 0) * ISNULL(DIM.[Length], 0) * ISNULL(DIM.Width, 0) * ISNULL(DIM.Thickness, 0)/1000000000) TotalCBM,
                                 ISNULL(DIMEXI.[Length], 0) [LengthEXI],
                                 ISNULL(DIMEXI.Width, 0) WidthEXI, ISNULL(DIMEXI.Thickness, 0) ThicknessEXI, 
                                 (ISNULL(SB.Qty, 0) * ISNULL(DIMEXI.[Length], 0) * ISNULL(DIMEXI.Width, 0) * ISNULL(DIMEXI.Thickness, 0)/1000000000) TotalCBMEXI,
                                  LOC.Name Location, ISNULL(SB.Qty, 0) IssueQty
                            FROM TblWHDStockBalance SB 
                                LEFT JOIN VWHDMaterialCodeCommon MAT ON MAT.ID_SanPham = SB.MaterialCode
                                LEFT JOIN VWHDUnit UNI ON MAT.IDDonViTinh = UNI.ID_DonViTinh
                                LEFT JOIN VWHDDimensionWoods DIM ON DIM.Pid = SB.DimensionPid
                                LEFT JOIN VWHDDimensionWoods DIMEXI ON DIMEXI.Pid = SB.DimensionPidEXI
                                LEFT JOIN VWHDLocationWoods LOC ON SB.LocationPid = LOC.ID_Position
                            WHERE SB.LotNoId ='" + lotnoId + "'";

          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt != null && dt.Rows.Count > 0)
          {
            e.Cell.Row.Cells["Errors"].Value = DBConvert.ParseInt(dt.Rows[0]["Errors"].ToString());
            e.Cell.Row.Cells["PID"].Value = 0;
            e.Cell.Row.Cells["MaterialCode"].Value = dt.Rows[0]["MaterialCode"].ToString();
            e.Cell.Row.Cells["NameEN"].Value = dt.Rows[0]["NameEN"].ToString();
            e.Cell.Row.Cells["NameVN"].Value = dt.Rows[0]["NameVN"].ToString();
            e.Cell.Row.Cells["TenDonViEN"].Value = dt.Rows[0]["TenDonViEN"].ToString();
            e.Cell.Row.Cells["Qty"].Value = DBConvert.ParseDouble(dt.Rows[0]["Qty"].ToString());
            e.Cell.Row.Cells["Length"].Value = DBConvert.ParseDouble(dt.Rows[0]["Length"].ToString());
            e.Cell.Row.Cells["Width"].Value = DBConvert.ParseDouble(dt.Rows[0]["Width"].ToString());
            e.Cell.Row.Cells["Thickness"].Value = DBConvert.ParseDouble(dt.Rows[0]["Thickness"].ToString());
            e.Cell.Row.Cells["TotalCBM"].Value = DBConvert.ParseDouble(dt.Rows[0]["TotalCBM"].ToString());
            e.Cell.Row.Cells["LengthEXI"].Value = DBConvert.ParseDouble(dt.Rows[0]["LengthEXI"].ToString());
            e.Cell.Row.Cells["WidthEXI"].Value = DBConvert.ParseDouble(dt.Rows[0]["WidthEXI"].ToString());
            e.Cell.Row.Cells["ThicknessEXI"].Value = DBConvert.ParseDouble(dt.Rows[0]["ThicknessEXI"].ToString());
            e.Cell.Row.Cells["TotalCBMEXI"].Value = DBConvert.ParseDouble(dt.Rows[0]["TotalCBMEXI"].ToString());
            e.Cell.Row.Cells["Location"].Value = dt.Rows[0]["Location"].ToString();
            e.Cell.Row.Cells["IssueQty"].Value = DBConvert.ParseDouble(dt.Rows[0]["TotalCBM"].ToString());

            e.Cell.Row.CellAppearance.BackColor = Color.White;
          }
          else
          {
            e.Cell.Row.Cells["Errors"].Value = 1;
            e.Cell.Row.Cells["MaterialCode"].Value = "";
            e.Cell.Row.Cells["NameEN"].Value = "";
            e.Cell.Row.Cells["NameVN"].Value = "";
            e.Cell.Row.Cells["TenDonViEN"].Value = "";
            e.Cell.Row.Cells["Qty"].Value = 0;
            e.Cell.Row.Cells["Length"].Value = 0;
            e.Cell.Row.Cells["Width"].Value = 0;
            e.Cell.Row.Cells["Thickness"].Value = 0;
            e.Cell.Row.Cells["TotalCBM"].Value = 0;
            e.Cell.Row.Cells["LengthEXI"].Value = 0;
            e.Cell.Row.Cells["WidthEXI"].Value = 0;
            e.Cell.Row.Cells["ThicknessEXI"].Value = 0;
            e.Cell.Row.Cells["TotalCBMEXI"].Value = 0;
            e.Cell.Row.Cells["Location"].Value = "";
            e.Cell.Row.Cells["IssueQty"].Value = 0;
            e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
          }
          // truong add
          DataTable dtMain = (DataTable)this.ultReceipt.DataSource;
          DataRow[] foundRows = dtMain.Select("LotNoId = '" + lotnoId + "'");
          if (foundRows.Length > 0)
          {
            e.Cell.Row.Cells["Errors"].Value = 1;
            e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
          }
          // end

          e.Cell.Row.Cells["MaterialCode"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["NameEN"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["NameVN"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["TenDonViEN"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["Qty"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["Length"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["Width"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["Thickness"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["TotalCBM"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["LengthEXI"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["WidthEXI"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["ThicknessEXI"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["TotalCBMEXI"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["Location"].Activation = Activation.ActivateOnly;
          break;
        default:
          break;
      }
    }

    private void btnAddMultiDetail_Click(object sender, EventArgs e)
    {
      viewWHD_21_006 uc = new viewWHD_21_006();
      Shared.Utility.WindowUtinity.ShowView(uc, "WAREHOUSE ISSUING NOTE DETAIL", true, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);

      // Create DataTable
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
          row["WidthEXI"] = 0;
          row["LengthEXI"] = 0;
          row["ThicknessEXI"] = 0;
          row["Package"] = "0";
          row["Location"] = "0";
          row["Package"] = "0";
          dtSource.Rows.Add(row);
        }
      }
      //Get Data
      DataTable result = this.GetDataLoad(dtSource);

      // Get DataSource Grid
      DataTable dtMain = (DataTable)this.ultReceipt.DataSource;

      // Loop for datatable get from SQL
      foreach (DataRow dr in result.Rows)
      {
        DataRow row = dtMain.NewRow();
        DataRow[] foundRows = dtMain.Select("LotNoId = '" + dr["LotNoId"].ToString() + "'");
        if (foundRows.Length > 0)
        {
          row["Errors"] = 1;
        }
        else
        {
          row["Errors"] = DBConvert.ParseInt(dr["Errors"].ToString());
        }

        row["LotNoId"] = dr["LotNoId"].ToString();
        row["MaterialCode"] = dr["MaterialCode"].ToString();
        row["NameEN"] = dr["NameEN"].ToString();
        row["NameVN"] = dr["NameVN"].ToString();
        row["TenDonViEN"] = dr["TenDonViEN"].ToString();
        row["Qty"] = DBConvert.ParseDouble(dr["Qty"].ToString());
        row["Width"] = DBConvert.ParseDouble(dr["Width"].ToString());
        row["Length"] = DBConvert.ParseDouble(dr["Length"].ToString());
        row["Thickness"] = DBConvert.ParseDouble(dr["Thickness"].ToString());
        row["TotalCBM"] = DBConvert.ParseDouble(dr["TotalCBM"].ToString());
        row["WidthEXI"] = DBConvert.ParseDouble(dr["WidthEXI"].ToString());
        row["LengthEXI"] = DBConvert.ParseDouble(dr["LengthEXI"].ToString());
        row["ThicknessEXI"] = DBConvert.ParseDouble(dr["ThicknessEXI"].ToString());
        row["TotalCBMEXI"] = DBConvert.ParseDouble(dr["TotalCBMEXI"].ToString());
        row["Package"] = dr["Package"].ToString();
        row["Location"] = dr["Location"].ToString();
        row["IssueQty"] = DBConvert.ParseDouble(dr["TotalCBM"].ToString());
        dtMain.Rows.Add(row);
      }

      this.ultReceipt.DataSource = dtMain;

      for (int j = 0; j < ultReceipt.Rows.Count; j++)
      {
        UltraGridRow rowGrid = ultReceipt.Rows[j];

        rowGrid.Cells["LotNoId"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["MaterialCode"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["NameEN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["NameVN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["TenDonViEN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Qty"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Length"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Width"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Thickness"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["TotalCBM"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["LengthEXI"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["WidthEXI"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["ThicknessEXI"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["TotalCBMEXI"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Package"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Location"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["IssueQty"].Activation = Activation.ActivateOnly;

        if (DBConvert.ParseInt(rowGrid.Cells["Errors"].Value.ToString()) == 1)
        {
          rowGrid.CellAppearance.BackColor = Color.Yellow;
        }
      }
      // End
    }

    /// <summary>
    /// Load Data Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLoad_Click(object sender, EventArgs e)
    {
      // Get path from Folder
      string path = @"\PhanmemDENSOBHT8000";
      path = Path.GetFullPath(path);

      // Create DataTable
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
          row["WidthEXI"] = 0;
          row["LengthEXI"] = 0;
          row["ThicknessEXI"] = 0;
          row["Package"] = "0";
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
          row["WidthEXI"] = 0;
          row["LengthEXI"] = 0;
          row["ThicknessEXI"] = 0;
          row["Package"] = "0";
          row["Location"] = "0";
          dtSource.Rows.Add(row);
        }
      }

      //Get Data
      DataTable result = this.GetDataLoad(dtSource);

      // Get DataSource Grid
      DataTable dtMain = (DataTable)this.ultReceipt.DataSource;

      // Loop for datatable get from SQL
      foreach (DataRow dr in result.Rows)
      {
        DataRow row = dtMain.NewRow();
        DataRow[] foundRows = dtMain.Select("LotNoId = '" + dr["LotNoId"].ToString() + "'");
        if (foundRows.Length > 0)
        {
          row["Errors"] = 1;
        }
        else
        {
          row["Errors"] = DBConvert.ParseInt(dr["Errors"].ToString());
        }

        row["LotNoId"] = dr["LotNoId"].ToString();
        row["MaterialCode"] = dr["MaterialCode"].ToString();
        row["NameEN"] = dr["NameEN"].ToString();
        row["NameVN"] = dr["NameVN"].ToString();
        row["TenDonViEN"] = dr["TenDonViEN"].ToString();
        row["Qty"] = DBConvert.ParseDouble(dr["Qty"].ToString());
        row["Width"] = DBConvert.ParseDouble(dr["Width"].ToString());
        row["Length"] = DBConvert.ParseDouble(dr["Length"].ToString());
        row["Thickness"] = DBConvert.ParseDouble(dr["Thickness"].ToString());
        row["TotalCBM"] = DBConvert.ParseDouble(dr["TotalCBM"].ToString());
        row["WidthEXI"] = DBConvert.ParseDouble(dr["WidthEXI"].ToString());
        row["LengthEXI"] = DBConvert.ParseDouble(dr["LengthEXI"].ToString());
        row["ThicknessEXI"] = DBConvert.ParseDouble(dr["ThicknessEXI"].ToString());
        row["TotalCBMEXI"] = DBConvert.ParseDouble(dr["TotalCBMEXI"].ToString());
        row["Location"] = dr["Location"].ToString();
        row["IssueQty"] = DBConvert.ParseDouble(dr["TotalCBM"].ToString());

        dtMain.Rows.Add(row);
      }

      this.ultReceipt.DataSource = dtMain;

      for (int j = 0; j < ultReceipt.Rows.Count; j++)
      {
        UltraGridRow rowGrid = ultReceipt.Rows[j];

        rowGrid.Cells["LotNoId"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["MaterialCode"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["NameEN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["NameVN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["TenDonViEN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Qty"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Length"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Width"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Thickness"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["TotalCBM"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["LengthEXI"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["WidthEXI"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["ThicknessEXI"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["TotalCBMEXI"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["Location"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["IssueQty"].Activation = Activation.ActivateOnly;

        if (DBConvert.ParseInt(rowGrid.Cells["Errors"].Value.ToString()) == 1)
        {
          rowGrid.CellAppearance.BackColor = Color.Yellow;
        }
      }
    }

    private void ultSummary_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["QtyM2"].Header.Caption = "CBM";
      e.Layout.Bands[0].Columns["TenDonViEN"].Header.Caption = "Unit";

      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 100;

      e.Layout.Bands[0].Columns["TenDonViEN"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["TenDonViEN"].MinWidth = 100;

      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 100;

      e.Layout.Bands[0].Columns["QtyM2"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["QtyM2"].MinWidth = 100;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyM2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //Sum Qty, Total CBM And IssueQty
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["QtyM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.0000}";

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultReceipt_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Errors"].Hidden = true;
      e.Layout.Bands[0].Columns["PID"].Hidden = true;

      e.Layout.Bands[0].Columns["LotNoId"].Header.Caption = "ID Wood";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "CBM \nUsed";
      e.Layout.Bands[0].Columns["TotalCBMEXI"].Header.Caption = "CBM \nPhysical";

      e.Layout.Bands[0].Columns["Length"].Header.Caption = "Length \nUsed";
      e.Layout.Bands[0].Columns["Width"].Header.Caption = "Width \nUsed";
      e.Layout.Bands[0].Columns["Thickness"].Header.Caption = "Thickness \nUsed";

      e.Layout.Bands[0].Columns["LengthEXI"].Header.Caption = "Length \nPhysical";
      e.Layout.Bands[0].Columns["WidthEXI"].Header.Caption = "Width \nPhysical";
      e.Layout.Bands[0].Columns["ThicknessEXI"].Header.Caption = "Thickness \nPhysical";

      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Name VN";
      e.Layout.Bands[0].Columns["TenDonViEN"].Header.Caption = "Unit";
      //e.Layout.Bands[0].Columns["Location"].Header.Caption = "Package";
      e.Layout.Bands[0].Columns["IssueQty"].Header.Caption = "Issue Qty";

      e.Layout.Bands[0].Columns["TenDonViEN"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["TenDonViEN"].MinWidth = 70;

      e.Layout.Bands[0].Columns["LotNoId"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["LotNoId"].MinWidth = 70;

      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;

      e.Layout.Bands[0].Columns["Location"].MaxWidth = 110;
      e.Layout.Bands[0].Columns["Location"].MinWidth = 110;

      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;

      e.Layout.Bands[0].Columns["Length"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Length"].MinWidth = 80;

      e.Layout.Bands[0].Columns["Width"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Width"].MinWidth = 80;

      e.Layout.Bands[0].Columns["Thickness"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Thickness"].MinWidth = 90;

      e.Layout.Bands[0].Columns["TotalCBM"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["TotalCBM"].MinWidth = 120;

      e.Layout.Bands[0].Columns["IssueQty"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["IssueQty"].MinWidth = 120;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["IssueQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["IssueQty"].CellAppearance.ForeColor = Color.Blue;

      e.Layout.Bands[0].Columns["IssueQty"].CellActivation = (this.canUpdate) ? Activation.AllowEdit : Activation.ActivateOnly;

      //Sum Qty, Total CBM And IssueQty
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["IssueQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBMEXI"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.0000}";
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,##0.0000}";
      e.Layout.Bands[0].Summaries[3].DisplayFormat = "{0:###,##0.0000}";

      e.Layout.Bands[0].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

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
    /// Print Issuing Note
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrintPreview_Click(object sender, EventArgs e)
    {
      viewWHD_99_001 view = new viewWHD_99_001();
      view.ncategory = 1;
      view.issuingNotePid = issuingPid;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      PrintDialog prt = new PrintDialog();
      DialogResult result = prt.ShowDialog();

      if (result == DialogResult.OK)
      {
        int nCopy = prt.PrinterSettings.Copies;
        int sPage = prt.PrinterSettings.FromPage;
        int ePage = prt.PrinterSettings.ToPage;

        // Report
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@IssuingNotePid", DbType.Int64, this.issuingPid);

        DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDRPTIssuingNoteWoods_Select", inputParam);

        dsWHDWoodsIssuingNote dsSource = new dsWHDWoodsIssuingNote();
        dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Merge(ds.Tables[1]);

        double qty = 0;
        double pcs = 0;
        foreach (DataRow row in dsSource.Tables["TblWHDWoodsIssuingNoteDetail"].Rows)
        {
          qty += DBConvert.ParseDouble(row["Qty"].ToString());
          pcs += DBConvert.ParseDouble(row["Pcs"].ToString());
        }
        qty = Math.Round(qty, 4);
        cptWHDIssuingNoteWoods rpt = new cptWHDIssuingNoteWoods();
        rpt.SetDataSource(dsSource);

        string remark = "WOD: " + ds.Tables[0].Rows[0]["Remark"].ToString();
        string source = string.Empty;
        int type = DBConvert.ParseInt(ds.Tables[0].Rows[0]["Type"].ToString());
        if (type == 1)
        {
          source = "Department Requested :" + ds.Tables[0].Rows[0]["Source"].ToString();
        }
        else if (type == 2)
        {
          source = "Supplier :" + ds.Tables[0].Rows[0]["Source"].ToString();
        }
        else if (type == 3)
        {
          source = "";
        }
        else if (type == 4)
        {
          source = "Supplier :" + ds.Tables[0].Rows[0]["Source"].ToString();
        }

        string title = ds.Tables[0].Rows[0]["Title"].ToString();
        string trNo = "Tr#no: " + ds.Tables[0].Rows[0]["IssuingCode"].ToString();
        string date = "Date: " + ds.Tables[0].Rows[0]["CreateDate"].ToString();

        rpt.SetParameterValue("WOD", remark);
        rpt.SetParameterValue("Title", title);
        rpt.SetParameterValue("TrNo", trNo);
        rpt.SetParameterValue("Date", date);
        rpt.SetParameterValue("Source", source);
        rpt.SetParameterValue("Total", qty);
        rpt.SetParameterValue("TotalPcs", pcs);

        // End Report

        printerName = DaiCo.Shared.Utility.FunctionUtility.GetDefaultPrinter();
        try
        {
          rpt.PrintOptions.PrinterName = printerName;
          rpt.PrintToPrinter(nCopy, false, sPage, ePage);
          WindowUtinity.ShowMessageSuccessFromText("Print Successfully");
        }
        catch (Exception err)
        {
          MessageBox.Show(err.ToString());
        }
      }
    }
    #endregion Event

    #region Load Data
    /// <summary>
    /// Get Data When Load Data
    /// </summary>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private DataTable GetDataLoad(DataTable dtSource)
    {
      DataTable dt = new DataTable();
      SqlCommand cm = new SqlCommand("spWHDGetDataIssuingFromBarCodeWoods_Select");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

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
    /// Create DataTable Before Load
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
      dt.Columns.Add("WidthEXI", typeof(System.Double));
      dt.Columns.Add("LengthEXI", typeof(System.Double));
      dt.Columns.Add("ThicknessEXI", typeof(System.Double));
      dt.Columns.Add("Package", typeof(System.String));
      dt.Columns.Add("Location", typeof(System.String));

      return dt;
    }

    /// <summary>
    /// Check valid before save
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      // Master Information
      if (this.ultSupplier.Value != null && this.ultSupplier.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM VWHDSupplier WHERE ID_NhaCC = '" + this.ultSupplier.Value.ToString() + "'";
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Supplier";
            return false;
          }
        }
        else
        {
          message = "Supplier";
          return false;
        }
      }
      else
      {
        message = "Supplier";
        return false;
      }

      // Detail
      DataTable dtMain = (DataTable)this.ultReceipt.DataSource;

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
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;

      //Master Information Issuing Veneer
      result = this.SaveIssuingVeneer();

      if (!result)
      {
        return false;
      }

      // Save Issuing Veneer Detail 
      result = this.SaveIssuingDetailVeneer();

      return result;
    }

    /// <summary>
    /// Save Master Information Issuing Veneer
    /// </summary>
    /// <returns></returns>
    private bool SaveIssuingVeneer()
    {
      string storeName = string.Empty;

      // Update
      if (this.issuingPid != long.MinValue)
      {
        storeName = "spWHDIssuingWoods_Update";
        DBParameter[] inputParamUpdate = new DBParameter[7];

        // Pid
        inputParamUpdate[0] = new DBParameter("@PID", DbType.Int64, this.issuingPid);

        // Title
        if (this.txtTitle.Text.Length > 0)
        {
          inputParamUpdate[1] = new DBParameter("@Title", DbType.AnsiString, 4000, this.txtTitle.Text);
        }

        // Status
        if (this.chkComfirm.Checked)
        {
          inputParamUpdate[2] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamUpdate[2] = new DBParameter("@Status", DbType.Int32, 0);
        }

        // Issuing Source
        inputParamUpdate[3] = new DBParameter("@IssuingSource", DbType.String, this.ultSupplier.Value.ToString());

        // Type
        inputParamUpdate[4] = new DBParameter("@Type", DbType.Int32, 4);

        // Remark
        inputParamUpdate[5] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);

        // UpdateBy
        inputParamUpdate[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        DBParameter[] outputParamUpdate = new DBParameter[1];
        outputParamUpdate[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamUpdate, outputParamUpdate);

        long resultPid = DBConvert.ParseLong(outputParamUpdate[0].Value.ToString());
        this.issuingPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
      }
      // Insert
      else
      {
        storeName = "spWHDIssuingWoods_Insert";
        DBParameter[] inputParamInsert = new DBParameter[7];

        // Title
        if (this.txtTitle.Text.Length > 0)
        {
          inputParamInsert[0] = new DBParameter("@Title", DbType.AnsiString, 4000, this.txtTitle.Text);
        }

        // Status
        if (this.chkComfirm.Checked)
        {
          inputParamInsert[1] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamInsert[1] = new DBParameter("@Status", DbType.Int32, 0);
        }

        // Import Source
        inputParamInsert[2] = new DBParameter("@IssuingSource", DbType.String, this.ultSupplier.Value.ToString());

        // Type
        inputParamInsert[3] = new DBParameter("@Type", DbType.Int32, 4);

        // Remark
        inputParamInsert[4] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);

        // CreateBy
        inputParamInsert[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        // ReceivingNo
        inputParamInsert[6] = new DBParameter("@IssuingNo", DbType.String, "05ITS");

        DBParameter[] outputParamInsert = new DBParameter[1];
        outputParamInsert[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);

        long resultPid = DBConvert.ParseLong(outputParamInsert[0].Value.ToString());
        this.issuingPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
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
      dt.Columns.Add("IDVeneer", typeof(System.String));
      dt.Columns.Add("Code", typeof(System.String));
      dt.Columns.Add("Pcs", typeof(System.Double));
      dt.Columns.Add("Width", typeof(System.Double));
      dt.Columns.Add("Length", typeof(System.Double));
      dt.Columns.Add("Thickness", typeof(System.Double));
      dt.Columns.Add("WidthEXI", typeof(System.Double));
      dt.Columns.Add("LengthEXI", typeof(System.Double));
      dt.Columns.Add("ThicknessEXI", typeof(System.Double));
      dt.Columns.Add("Package", typeof(System.String));
      dt.Columns.Add("Location", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Save Issuing Veneer Detail 
    /// </summary>
    /// <returns></returns>
    private bool SaveIssuingDetailVeneer()
    {
      // Create dt Before Save
      DataTable dtSource = this.CreateDataTableBeforeSave();

      // Get Main Data Issuing Detail
      DataTable dtMain = (DataTable)this.ultReceipt.DataSource;

      foreach (DataRow drMain in dtMain.Rows)
      {
        // Get Row is not Delete and no Errors
        if (drMain.RowState != DataRowState.Deleted && DBConvert.ParseDouble(drMain["Errors"].ToString()) == 0)
        {
          DataRow row = dtSource.NewRow();
          row["IDVeneer"] = drMain["LotNoId"].ToString();
          row["Code"] = drMain["MaterialCode"].ToString();
          row["Pcs"] = 1;
          row["Width"] = 0;
          row["Length"] = 0;
          row["Thickness"] = 0;
          row["WidthEXI"] = 0;
          row["LengthEXI"] = 0;
          row["ThicknessEXI"] = 0;
          row["Location"] = drMain["Location"].ToString();
          row["Package"] = drMain["Package"].ToString();
          dtSource.Rows.Add(row);
        }
      }

      SqlCommand cm = new SqlCommand("spWHDIssuingDetailWoods_Insert");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;
      cm.CommandTimeout = 300;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      cm.Parameters.Add(para);

      // Issuing Pid
      para = cm.CreateParameter();
      para.ParameterName = "@IssuingPid";
      para.DbType = DbType.Int64;
      para.Value = this.issuingPid;

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
    /// Delete Row/ Update Stock Balance
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultReceipt_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.issuingPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["PID"].Value.ToString()) > 0
                        && DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 0)
          {
            DBParameter[] inputParams = new DBParameter[2];
            inputParams[0] = new DBParameter("@IssuingPid", DbType.Int64, this.issuingPid);
            inputParams[1] = new DBParameter("@LotNoId", DbType.String, row.Cells["LotNoId"].Value.ToString());

            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            string storeName = string.Empty;
            storeName = "spWHDIssuingDetailWoods_Delete";

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
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Load Data
  }
}
