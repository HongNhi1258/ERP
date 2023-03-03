/*
  Author      : Dang Xuan Truong
  Date        : 30/06/2012
  Description : List Receiving For Service
*/
using DaiCo.Application;
using DaiCo.ERPProject.Warehouse.Material.DataSetSource;
using DaiCo.ERPProject.Warehouse.Reports;
using DaiCo.ERPProject.Warehouse.Service.DataSetSource;
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
  public partial class viewWHD_26_001 : MainUserControl
  {
    #region Field
    // Store
    string SP_ListReceivingNoteService = "spWHDListReceivingNoteService_Select";
    #endregion Field

    #region Init
    public viewWHD_26_001()
    {
      InitializeComponent();
    }

    private void viewWHD_26_001_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function
    private void LoadData()
    {
      this.LoadMaterialCode();
      this.LoadPosting();
      this.LoadSupplier();
      ultDateFrom.Value = DateTime.Today.AddDays(-7);
    }

    private void LoadSupplier()
    {
      string commandText = string.Empty;
      commandText = "SELECT Pid, EnglishName AS Name  FROM TblPURSupplierInfo WHERE DeleteFlg = 0 AND Confirm = 2";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBSupplier.DataSource = dtSource;
      ultCBSupplier.DisplayMember = "Name";
      ultCBSupplier.ValueMember = "Pid";
      ultCBSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    private void LoadMaterialCode()
    {
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT MAT.MaterialCode, MAT.MaterialCode +' - '+ MAT.NameEN AS Name
                                    FROM TblGNRMaterialInformation MAT
	                                    INNER JOIN TblGNRMaterialGroup MG ON	MG.[Group] = MAT.[Group] 
										                                                    AND MG.Warehouse = 4");

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBMaterialCode.DataSource = dtSource;
      ultCBMaterialCode.DisplayMember = "Name";
      ultCBMaterialCode.ValueMember = "MaterialCode";
      ultCBMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBMaterialCode.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultCBMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = true;
    }

    private void LoadPosting()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Confirmed' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBPosting.DataSource = dtSource;
      ultCBPosting.DisplayMember = "Name";
      ultCBPosting.ValueMember = "ID";
      ultCBPosting.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBPosting.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBPosting.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }

    private void Search()
    {
      DBParameter[] param = new DBParameter[7];

      // Receiving No
      string text = txtRecNoFrom.Text.Trim().Replace("'", "''");
      int value = int.MinValue;
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

      // Posting
      if (this.ultCBPosting.Value != null)
      {
        value = DBConvert.ParseInt(this.ultCBPosting.Value.ToString());
        if (value != int.MinValue)
        {
          param[4] = new DBParameter("@Status", DbType.Int32, value);
        }
      }
      // Material Code
      if (this.ultCBMaterialCode.Value != null)
      {
        text = this.ultCBMaterialCode.Value.ToString();
        if (text.Length > 0)
        {
          param[5] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, text);
        }
      }

      // Supplier
      if (this.ultCBSupplier.Value != null)
      {
        text = this.ultCBSupplier.Value.ToString();
        if (text.Length > 0)
        {
          param[6] = new DBParameter("@Supplier", DbType.Int64, DBConvert.ParseLong(ultCBSupplier.Value.ToString()));
        }
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(SP_ListReceivingNoteService, param);
      if (dsSource != null)
      {
        dsWHDListReceivingNoteService dsData = new dsWHDListReceivingNoteService();
        dsData.Tables["dtReceivingInfo"].Merge(dsSource.Tables[0]);
        dsData.Tables["dtReceivingDetail"].Merge(dsSource.Tables[1]);
        ultData.DataSource = dsData;
      }
      else
      {
        ultData.DataSource = DBNull.Value;
      }
    }

    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ReceivingCodePid"].Hidden = true;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ReceivingCode"].Header.Caption = "Receiving Code";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";

      //e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      //e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtRecNoFrom.Text = string.Empty;
      txtRecNoTo.Text = string.Empty;
      ultCBMaterialCode.Text = string.Empty;
      ultCBPosting.Text = string.Empty;
      ultCBSupplier.Text = string.Empty;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

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

      long receivingPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

      viewWHD_26_002 uc = new viewWHD_26_002();
      uc.receivingPid = receivingPid;
      WindowUtinity.ShowView(uc, "UPDATE RECEIVING CONFIRMATION", false, ViewState.MainWindow);
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewWHD_26_002 view = new viewWHD_26_002();
      WindowUtinity.ShowView(view, "RECEIVING CONFIRMATION ", false, ViewState.MainWindow);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      int count = 0;
      // Check Valid
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          count = count + 1;
          if (count > 1)
          {
            WindowUtinity.ShowMessageError("ERR0115", "One Receiving");
            return;
          }
        }
      }
      // Printf
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          long receivingPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          // Report
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@ReceivingPid", DbType.Int64, receivingPid);

          DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHDRPTReceivingService_Select", inputParam);

          dsWHDReceivingServiceReport dsSource = new dsWHDReceivingServiceReport();
          if (dsSource != null)
          {
            dsSource.Tables["dtInfo"].Merge(ds.Tables[0]);
            dsSource.Tables["dtDetail"].Merge(ds.Tables[1]);
            DaiCo.Shared.View_Report report = null;
            cptReceivingService rpt = new cptReceivingService();
            rpt.SetDataSource(dsSource);
            double totalQty = 0;
            for (int j = 0; j < dsSource.Tables["dtDetail"].Rows.Count; j++)
            {
              if (DBConvert.ParseDouble(dsSource.Tables["dtDetail"].Rows[j]["Qty"].ToString()) != double.MinValue)
              {
                totalQty = totalQty + DBConvert.ParseDouble(dsSource.Tables["dtDetail"].Rows[j]["Qty"].ToString());
              }
            }
            string printDate = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
            string printBy = SharedObject.UserInfo.EmpName;
            rpt.SetParameterValue("TotalQty", totalQty);
            rpt.SetParameterValue("Printdate", printDate);
            rpt.SetParameterValue("Printby", printBy);
            report = new DaiCo.Shared.View_Report(rpt);

            report.IsShowGroupTree = false;
            report.ShowReport(Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
          }
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
