/*
  Author      : 
  Date        : 1/6/2013
  Description : Material Quote List
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

namespace DaiCo.Purchasing
{
  public partial class viewPUR_20_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPUR_20_001()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_03_001_Load(object sender, EventArgs e)
    {
      drpDateFrom.Value = DateTime.Today.AddDays(-7);
      this.LoadInit();
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      // Load Material Code
      this.LoadMaterialCode();

      // Load Create By
      this.LoadCreateBy();

      // Load Supplier
      this.LoadSupplier();

      this.ultCreateBy.Value = SharedObject.UserInfo.UserPid;
    }

    /// <summary>
    /// Load Supplier
    /// </summary>
    private void LoadSupplier()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, EnglishName";
      commandText += " FROM TblPURSupplierInfo ";
      commandText += " WHERE Confirm = 2 ";
      commandText += " 	AND LEN(EnglishName) > 0 ";
      commandText += " ORDER BY EnglishName ";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultSupplier.DataSource = dt;
      ultSupplier.DisplayMember = "EnglishName";
      ultSupplier.ValueMember = "Pid";
      ultSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultSupplier.DisplayLayout.Bands[0].Columns["EnglishName"].Width = 350;
    }

    /// <summary>
    /// Load Material Code
    /// </summary>
    private void LoadMaterialCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT MaterialCode, MaterialCode + ' ' + NameEN Name";
      commandText += " FROM TblGNRMaterialInformation ";
      commandText += " ORDER BY MaterialCode ";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultMaterialCode.DataSource = dt;
      ultMaterialCode.DisplayMember = "Name";
      ultMaterialCode.ValueMember = "MaterialCode";
      ultMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = true;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
    }

    /// <summary>
    /// Load Create By
    /// </summary>
    private void LoadCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Resigned = 0";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCreateBy.DataSource = dtSource;
      ultCreateBy.DisplayMember = "Name";
      ultCreateBy.ValueMember = "ID_NhanVien";
      ultCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCreateBy.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCreateBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[10];

      if (this.ultMaterialCode.Value != null && this.ultMaterialCode.Value.ToString().Length > 0)
      {
        input[0] = new DBParameter("@MaterialCode", DbType.String, this.ultMaterialCode.Value.ToString());
      }

      if (this.ultSupplier.Value != null && DBConvert.ParseLong(this.ultSupplier.Value.ToString()) != long.MinValue)
      {
        input[1] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(this.ultSupplier.Value.ToString()));
      }

      if (this.ultCreateBy.Value != null && DBConvert.ParseInt(this.ultCreateBy.Value.ToString()) != int.MinValue)
      {
        input[2] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(this.ultCreateBy.Value.ToString()));
      }

      // DateFrom
      if (drpDateFrom.Value != null)
      {
        DateTime prDateFrom = DateTime.MinValue;
        prDateFrom = (DateTime)drpDateFrom.Value;
        input[3] = new DBParameter("@DateFrom", DbType.DateTime, prDateFrom);
      }

      // DateTo
      if (drpDateTo.Value != null)
      {
        DateTime prDateTo = DateTime.MinValue;
        prDateTo = (DateTime)drpDateTo.Value;
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        input[4] = new DBParameter("@DateTo", DbType.DateTime, prDateTo);
      }

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPURMaterialQuoteList_Select", input);

      this.ultData.DataSource = dt;

      // Enable button search
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Enter Search
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Price"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      //e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      for (int i = 0; i < ultData.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        string columnName = ultData.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if ((string.Compare(columnName, "Select", true) == 0))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }


      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Double Click
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
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

      viewPUR_20_002 uc = new viewPUR_20_002();
      uc.pidSupplierPrice = pid;
      WindowUtinity.ShowView(uc, "MATERIAL QUOTE", false, ViewState.MainWindow);
    }

    /// <summary>
    /// New Requisition Special
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPUR_20_002 view = new viewPUR_20_002();
      WindowUtinity.ShowView(view, "MATERIAL QUOTE", true, DaiCo.Shared.Utility.ViewState.MainWindow);
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
    /// Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Select"].Value.ToString()) == 1)
        {
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@DeletePid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["Pid"].Value.ToString()));

          DBParameter[] ouputParam = new DBParameter[1];
          ouputParam[0] = new DBParameter("@Result", DbType.Int64, Int64.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spPURMaterialQuote_Edit", inputParam, ouputParam);
        }
      }
      this.Search();
    }

    #endregion Event
  }
}
