/*
  Author      : Dang Xuan Truong
  Date        : 01/06/2012
  Description : List All Receiving Note For Material
*/
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.ERPProject.Warehouse.Material.DataSetSource;
using DaiCo.ERPProject.Warehouse.Material.Reports;
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
  public partial class viewWHD_05_002 : MainUserControl
  {
    #region Field    
    #endregion Field

    #region Init
    public viewWHD_05_002()
    {
      InitializeComponent();
    }

    private void viewWHD_05_002_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      this.InitData();
      ultDateFrom.Value = DateTime.Today.AddDays(-7);
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadWarehouse();

      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spWHDMaterialInStore_InitList");
      Utility.LoadUltraCombo(ultCBPosting, dsInit.Tables[0], "StatusCode", "StatusName", false, "StatusCode");
      Utility.LoadUltraCombo(ultCBRecType, dsInit.Tables[1], "TypeCode", "TypeName", false, "TypeCode");
      Utility.LoadUltraCombo(ultCBMaterialCode, dsInit.Tables[2], "MaterialCode", "DisplayText", false, "DisplayText");
      ultCBMaterialCode.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
      Utility.LoadUltraCombo(ultCBSupplier, dsInit.Tables[3], "ObjectCode", "DisplayText", false, "DisplayText");
      ultCBSupplier.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
      Utility.LoadUltraCombo(ucbActionCode, dsInit.Tables[4], "ActionCode", "ActionName", false, "ActionCode");
    }

    /// <summary>
    /// Load Type Warehouse
    /// </summary>
    private void LoadWarehouse()
    {
      Utility.LoadUltraCBMaterialWHListByUser(ultCBWarehouse);
    }

    private bool CheckValid(out string message)
    {
      message = string.Empty;
      //if (ultCBWarehouse.Value == null)
      //{
      //  message = "Warehouse";
      //  return false;
      //}
      return true;
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[10];

      // Receiving No
      string text = txtRecNoFrom.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@RECNoFrom", DbType.AnsiString, 32, text);
      }
      text = txtRecNoTo.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@RECNoTo", DbType.AnsiString, 32, text);
      }
      //Create Date
      DateTime prDateFrom = DateTime.MinValue;
      if (ultDateFrom.Value != null)
      {
        prDateFrom = (DateTime)ultDateFrom.Value;
      }
      DateTime prDateTo = DateTime.MinValue;
      if (ultDateTo.Value != null)
      {
        prDateTo = (DateTime)ultDateTo.Value;
      }
      if (prDateFrom != DateTime.MinValue)
      {
        param[2] = new DBParameter("@DateFrom", DbType.DateTime, prDateFrom);
      }
      if (prDateTo != DateTime.MinValue)
      {
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        param[3] = new DBParameter("@DateTo", DbType.DateTime, prDateTo);
      }
      // Type
      int value = int.MinValue;
      if (this.ultCBRecType.Value != null)
      {
        value = DBConvert.ParseInt(this.ultCBRecType.Value.ToString());
        if (value != int.MinValue)
        {
          param[4] = new DBParameter("@RECType", DbType.Int32, value);
        }
      }
      // Posting
      if (this.ultCBPosting.Value != null)
      {
        value = DBConvert.ParseInt(this.ultCBPosting.Value.ToString());
        if (value != int.MinValue)
        {
          param[5] = new DBParameter("@Posting", DbType.Int32, value);
        }
      }
      // Warehouse
      DataTable dtSelectedWH = new DataTable();
      dtSelectedWH.Columns.Add("WHPid", typeof(int));
      if (ultCBWarehouse.SelectedRow != null)
      {
        value = DBConvert.ParseInt(ultCBWarehouse.Value.ToString());
        DataRow rowWH = dtSelectedWH.NewRow();
        rowWH["WHPid"] = value;
        dtSelectedWH.Rows.Add(rowWH);             
      }
      else
      {
        DataTable dtWHs = (DataTable)ultCBWarehouse.DataSource;        
        foreach (DataRow row in dtWHs.Rows)
        {
          DataRow rowWH = dtSelectedWH.NewRow();
          rowWH["WHPid"] = row["Pid"];
          dtSelectedWH.Rows.Add(rowWH);
        }        
      }
      param[6] = new DBParameter("@WarehousePidList", DbType.String, 1024, DBConvert.ParseXMLString(dtSelectedWH));
      // Location
      
      // Material Code
      if (this.ultCBMaterialCode.Value != null)
      {
        text = this.ultCBMaterialCode.Value.ToString();
        if (text.Length > 0)
        {
          param[8] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, text);
        }
      }
      // Supplier
      if (this.ultCBSupplier.Value != null)
      {
        text = this.ultCBSupplier.Value.ToString();
        if (text.Length > 0)
        {
          param[9] = new DBParameter("@Supplier", DbType.AnsiString, 50, text);
        }
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDMaterialInStore_SearchList", 300, param);
      if (dsSource != null)
      {
        dsWHDListReceivingForMaterial dsData = new dsWHDListReceivingForMaterial();
        dsData.Tables["dtReceivingInfo"].Merge(dsSource.Tables[0]);
        dsData.Tables["dtReceivingDetail"].Merge(dsSource.Tables[1]);

        ultData.DataSource = dsData;

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].Appearance.BackColor = (DBConvert.ParseInt(ultData.Rows[i].Cells["Posting"].Value) == 1 ? Color.LightGray : Color.White);
        }
      }
      else
      {
        ultData.DataSource = DBNull.Value;
      }
      lbCount.Text = string.Format("Đếm: {0}", (ultData.Rows.FilteredInRowCount > 0 ? ultData.Rows.FilteredInRowCount : 0));
    }

    #endregion Function

    #region Event
    /// <summary>
    /// Init Layout 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ReceivingCode"].Hidden = true;
      e.Layout.Bands[1].Columns["IDPhieuDatMua"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["NgayNhap"].Hidden = true;
      e.Layout.Bands[0].Columns["Posting"].Hidden = true;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      // Set caption column
      e.Layout.Bands[0].Columns["Selected"].Header.Caption = "Chọn";
      e.Layout.Bands[0].Columns["ReceivingCode"].Header.Caption = "Mã phiếu nhập";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Người tạo";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Ngày nhập";
      e.Layout.Bands[0].Columns["NguonNhap"].Header.Caption = "Nhà cung cấp";      
      e.Layout.Bands[0].Columns["Status"].Header.Caption = "Trạng thái";

      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm";
      e.Layout.Bands[1].Columns["MaterialName"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands[1].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[1].Columns["Location"].Header.Caption = "Vị trí";
      e.Layout.Bands[1].Columns["Qty"].Header.Caption = "Số lượng";

      e.Layout.Bands[0].Columns["Status"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      //e.Layout.Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Clear Infomation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      txtRecNoFrom.Text = string.Empty;
      txtRecNoTo.Text = string.Empty;      
      ultCBMaterialCode.Text = string.Empty;
      ultCBPosting.Text = string.Empty;
      ultCBRecType.Text = string.Empty;
      ultCBWarehouse.Text = string.Empty;
      ultCBSupplier.Text = string.Empty;
      ultDateFrom.Value = DBNull.Value;
      ultDateTo.Value = DBNull.Value;
    }

    /// <summary>
    /// Search 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      string message;
      bool success = CheckValid(out message);
      if (success)
      {
        this.Search();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
      }
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Double Click Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      string receivingCode = row.Cells["ReceivingCode"].Value.ToString();
      int type = DBConvert.ParseInt(row.Cells["Type"].Value.ToString());
      if (type == 1)
      {
        //UltraGridRow rowChild = (ultData.Selected.Rows[0].ParentRow != null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ChildBands[0].Rows[0];
        //if (rowChild.Cells["IDPhieuDatMua"].Value.ToString().Length > 0)
        //{
        //  viewWHD_05_001 uc = new viewWHD_05_001();
        //  uc.receivingNo = receivingCode;
        //  WindowUtinity.ShowView(uc, "UPDATE RECEIVING NOTE", false, ViewState.MainWindow);
        //}
        //else
        //{
        //  viewWHD_05_007 uc = new viewWHD_05_007();
        //  uc.receivingNo = receivingCode;
        //  WindowUtinity.ShowView(uc, "UPDATE RECEIVING NOTE", false, ViewState.MainWindow);
        //}
        if (row.ChildBands[0].Rows.Count == 0)
        {
          viewWHD_05_001 uc = new viewWHD_05_001();
          uc.receivingNo = receivingCode;
          uc.receivingPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          WindowUtinity.ShowView(uc, "Chi tiết NK", false, ViewState.MainWindow);
        }
        else
        {
          UltraGridRow rowChild = (ultData.Selected.Rows[0].ParentRow != null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ChildBands[0].Rows[0];
          if (rowChild.Cells["IDPhieuDatMua"].Value.ToString().Length > 0)
          {
            viewWHD_05_001 uc = new viewWHD_05_001();
            uc.receivingNo = receivingCode;
            uc.receivingPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
            WindowUtinity.ShowView(uc, "Chi tiết NK", false, ViewState.MainWindow);
          }
          else
          {
            viewWHD_05_007 uc = new viewWHD_05_007();
            uc.receivingNo = receivingCode;
            WindowUtinity.ShowView(uc, "Chi tiết NK", false, ViewState.MainWindow);
          }
        }
      }
      else if (type == 2)
      {
        viewWHD_05_003 uc = new viewWHD_05_003();
        uc.RTWNo = receivingCode;
        WindowUtinity.ShowView(uc, "Chi tiết NK", false, ViewState.MainWindow);
      }
      else if (type == 3)
      {
        viewWHD_05_004 uc = new viewWHD_05_004();
        uc.ADINo = receivingCode;
        WindowUtinity.ShowView(uc, "Chi tiết NK", false, ViewState.MainWindow);
      }
      else if (type == 5)
      {
        viewWHD_05_006 uc = new viewWHD_05_006();
        uc.receivingCode = receivingCode;
        WindowUtinity.ShowView(uc, "UPDATE RECEIVING RETURN FROM SUPPLIER", false, ViewState.MainWindow);
      }
    }

    /// <summary>
    /// Print
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          DBParameter[] input = new DBParameter[] { new DBParameter("@ReceivingNote", DbType.AnsiString, 48, row.Cells["ReceivingCode"].Value.ToString()) };
          DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDRPTReceivingNotePrint_Materials", input);
          dsWHDRPTMaterialsReceivingNote dsReceiving = new dsWHDRPTMaterialsReceivingNote();
          dsReceiving.Tables["dtReceivingInfo"].Merge(dsSource.Tables[0]);
          dsReceiving.Tables["dtReceivingDetail"].Merge(dsSource.Tables[1]);
          double totalQty = 0;
          for (int j = 0; j < dsReceiving.Tables["dtReceivingDetail"].Rows.Count; j++)
          {
            if (DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[j]["Qty"].ToString()) != double.MinValue)
            {
              totalQty = totalQty + DBConvert.ParseDouble(dsReceiving.Tables["dtReceivingDetail"].Rows[j]["Qty"].ToString());
            }
          }

          ReportClass cpt = null;
          DaiCo.Shared.View_Report report = null;

          cpt = new cptMaterialsReceivingNote();
          cpt.SetDataSource(dsReceiving);
          cpt.SetParameterValue("TotalQty", totalQty);
          if (DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 1 ||
              DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 2 ||
              DBConvert.ParseInt(row.Cells["Type"].Value.ToString()) == 5)
          {
            cpt.SetParameterValue("Title", "MATERIALS RECEIVING NOTE");
            cpt.SetParameterValue("Receivedby", "Received by: ");
          }
          else
          {
            cpt.SetParameterValue("Title", "MATERIALS ADJUSTMENT IN");
            cpt.SetParameterValue("Receivedby", "Checked by: ");
          }
          report = new DaiCo.Shared.View_Report(cpt);
          report.IsShowGroupTree = false;
          report.ShowReport(Shared.Utility.ViewState.ModalWindow);

          return;
        }
      }
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
   

    private void btnNew_Click(object sender, EventArgs e)
    {
      if (ucbActionCode.SelectedRow != null)
      {
        viewWHD_05_001 view = new viewWHD_05_001();
        view.actionCode = DBConvert.ParseInt(ucbActionCode.Value);
        Shared.Utility.WindowUtinity.ShowView(view, "Phiếu NK Mới", false, ViewState.MainWindow);
      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText("Vui lòng chọn loại phiếu nhập kho");
      }
    }
    #endregion Event
  }
}
