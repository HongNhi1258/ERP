/*
  Author      : Dang 
  Date        : 30/06/2012
  Description : List Receiving Note Special For Veneer
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Purchasing.DataSetSource;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_07_005 : MainUserControl
  {
    #region Field
    // Store
    string SP_ListReceivingForMaterial = "spPURListReceivingSpecialForVeneer_Select";
    #endregion Field

    #region Init
    public viewPUR_07_005()
    {
      InitializeComponent();
    }

    private void viewPUR_07_005_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function
    private void LoadData()
    {
      this.LoadMaterialCode();
      this.LoadPosting();
      this.LoadReceivingType();
      this.LoadSupplier();
      this.LoadLocation();
      ultDateFrom.Value = DateTime.Today.AddDays(-7);
    }

    private void LoadLocation()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_Position, Name";
      commandText += " FROM VWHDLocation ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBLocation.DataSource = dtSource;
      ultCBLocation.DisplayMember = "Name";
      ultCBLocation.ValueMember = "ID_Position";
      ultCBLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBLocation.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBLocation.DisplayLayout.Bands[0].Columns["ID_Position"].Hidden = true;
    }

    private void LoadSupplier()
    {
      string commandText = string.Empty;
      commandText = "SELECT SupplierCode, EnglishName AS Name  FROM TblPURSupplierInfo WHERE DeleteFlg = 0 AND Confirm = 2";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBSupplier.DataSource = dtSource;
      ultCBSupplier.DisplayMember = "Name";
      ultCBSupplier.ValueMember = "SupplierCode";
      ultCBSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["SupplierCode"].Hidden = true;
    }

    private void LoadMaterialCode()
    {
      string commandText = string.Empty;
      commandText = "SELECT ID_SanPham MaterialCode, ID_SanPham +' - '+ TenEnglish AS Name FROM VWHDMaterialCodeCommon";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBMaterialCode.DataSource = dtSource;
      ultCBMaterialCode.DisplayMember = "Name";
      ultCBMaterialCode.ValueMember = "MaterialCode";
      ultCBMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBMaterialCode.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultCBMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = true;
    }

    private void LoadReceivingType()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 1 ID, 'Receiving Note' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Return From Production' Name";
      commandText += " UNION ";
      commandText += " SELECT 3 ID, 'Adjustment In' Name";
      commandText += " UNION ";
      commandText += " SELECT 4 ID, 'Special' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBRecType.DataSource = dtSource;
      ultCBRecType.DisplayMember = "Name";
      ultCBRecType.ValueMember = "ID";
      ultCBRecType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBRecType.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultCBRecType.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      // Set default Special Receiving
      ultCBRecType.Value = 1;
    }
    private void LoadPosting()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 1 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Confirmed' Name";

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
      DBParameter[] param = new DBParameter[9];

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

      // Location
      if (this.ultCBLocation.Value != null)
      {
        value = DBConvert.ParseInt(this.ultCBLocation.Value.ToString());
        if (value != int.MinValue)
        {
          param[6] = new DBParameter("@LocationPid", DbType.Int32, value);
        }
      }

      // Material Code
      if (this.ultCBMaterialCode.Value != null)
      {
        text = this.ultCBMaterialCode.Value.ToString();
        if (text.Length > 0)
        {
          param[7] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, text);
        }
      }

      // Supplier
      if (this.ultCBSupplier.Value != null)
      {
        text = this.ultCBSupplier.Value.ToString();
        if (text.Length > 0)
        {
          param[8] = new DBParameter("@Supplier", DbType.AnsiString, 32, text);
        }
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(SP_ListReceivingForMaterial, param);
      if (dsSource != null)
      {
        dsPURListReceivingForWoods dsData = new dsPURListReceivingForWoods();
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

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[1].Columns["ReceivingNotePid"].Hidden = true;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["NguonNhap"].Header.Caption = "Source";
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 100;

      e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtRecNoFrom.Text = string.Empty;
      txtRecNoTo.Text = string.Empty;
      ultCBLocation.Text = string.Empty;
      ultCBMaterialCode.Text = string.Empty;
      ultCBPosting.Text = string.Empty;
      ultCBRecType.Text = string.Empty;
      ultCBSupplier.Text = string.Empty;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
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

      long receivingPid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
      int type = DBConvert.ParseInt(row.Cells["Type"].Value.ToString());
      if (type == 1)
      {
        viewPUR_07_006 uc = new viewPUR_07_006();
        uc.receivingPid = receivingPid;
        WindowUtinity.ShowView(uc, "MAPPING PRPO FOR VENEER", false, ViewState.MainWindow);
      }
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
