/*
  Author      : Xuan Truong
  Date        : 23/10/2012
  Description : Transfer Location List(Materials)
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
  public partial class viewWHD_07_002 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewWHD_07_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_07_002_Load(object sender, EventArgs e)
    {
      // Load All Data For Search Information
      this.LoadData();

      // Set Focus
      this.txtNoFrom.Focus();
    }
    #endregion Init

    #region LoadData
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[7];

      // Tran No
      string text = txtNoFrom.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@NoFrom", DbType.AnsiString, 24, text);
      }

      text = txtNoTo.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@NoTo", DbType.AnsiString, 24, text);
      }

      text = txtNoSet.Text.Trim();
      string[] listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[2] = new DBParameter("@NoSet", DbType.AnsiString, 1024, text);
      }

      //Create Date
      DateTime prDateFrom = DateTime.MinValue;
      if (drpDateFrom.Value != null)
      {
        prDateFrom = (DateTime)drpDateFrom.Value;
      }
      DateTime prDateTo = DateTime.MinValue;
      if (drpDateTo.Value != null)
      {
        prDateTo = (DateTime)drpDateTo.Value;
      }

      if (prDateFrom != DateTime.MinValue)
      {
        param[3] = new DBParameter("@CreateDateFrom", DbType.DateTime, prDateFrom);
      }

      if (prDateTo != DateTime.MinValue)
      {
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        param[4] = new DBParameter("@CreateDateTo", DbType.DateTime, prDateTo);
      }

      // Create By
      int value = int.MinValue;
      if (this.ultCreateBy.Value != null)
      {
        value = DBConvert.ParseInt(this.ultCreateBy.Value.ToString());
        if (value != int.MinValue)
        {
          param[5] = new DBParameter("@CreateBy", DbType.Int32, value);
        }
      }

      // ID Veneer
      text = txtMaterialCode.Text.Trim();
      //listNo = text.Split(',');
      //text = string.Empty;
      //foreach (string no in listNo)
      //{
      //  if (no.Trim().Length > 0)
      //  {
      //    text += string.Format(",'{0}'", no.Replace("'", "").Trim());
      //  }
      //}
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[6] = new DBParameter("@MaterialCode", DbType.String, text);
      }

      string storeName = "spWHDListTranLocationMaterials_Select";
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, param);
      DataSet dsData = this.ListTranLocationWoods();
      dsData.Tables["TblTranLocationWoods"].Merge(dsSource.Tables[0]);
      dsData.Tables["TblTranLocationDetailWoods"].Merge(dsSource.Tables[1]);

      ultData.DataSource = dsData;
    }

    /// <summary>
    /// List Tran Location Veneer
    /// </summary>
    /// <returns></returns>
    private DataSet ListTranLocationWoods()
    {
      DataSet ds = new DataSet();

      // Parent
      DataTable taParent = new DataTable("TblTranLocationWoods");
      taParent.Columns.Add("PID", typeof(System.Int64));
      taParent.Columns.Add("TrNo", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);

      // Child
      DataTable taChild = new DataTable("TblTranLocationDetailWoods");
      taChild.Columns.Add("TranNoPid", typeof(System.Int64));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("NameEN", typeof(System.String));
      taChild.Columns.Add("NameVN", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("LocationFrom", typeof(System.String));
      taChild.Columns.Add("LocationTo", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblTranLocationWoods_TblTranLocationDetailWoods", taParent.Columns["PID"], taChild.Columns["TranNoPid"], false));
      return ds;
    }
    /// <summary>
    /// Load All Data For Search Information
    /// </summary>
    private void LoadData()
    {
      // Load UltraCombo Create By
      this.LoadComboCreateBy();

      // Set Focus
      this.txtNoFrom.Focus();
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Department = 'WHD' AND Resigned = 0";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCreateBy.DataSource = dtSource;
      ultCreateBy.DisplayMember = "Name";
      ultCreateBy.ValueMember = "ID_NhanVien";
      ultCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCreateBy.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCreateBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }
    #endregion LoadData

    #region Event
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Clear Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      //Format
      this.txtNoFrom.Text = string.Empty;
      this.txtNoTo.Text = string.Empty;
      this.txtNoSet.Text = string.Empty;
      this.drpDateFrom.Value = DateTime.Today;
      this.drpDateTo.Value = DateTime.Today;
      this.ultCreateBy.Text = string.Empty;
      this.txtMaterialCode.Text = string.Empty;

      // Load All Data For Search Information
      this.LoadData();
    }

    /// <summary>
    /// Search Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      // Search Information
      this.Search();
    }

    /// <summary>
    /// Format Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["TranNoPid"].Hidden = true;

      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[1].Columns["LocationFrom"].Header.Caption = "Location From";
      e.Layout.Bands[1].Columns["LocationTo"].Header.Caption = "Location To";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Open Sceen when Double
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

      long pid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());

      viewWHD_07_001 uc = new viewWHD_07_001();
      uc.locationPid = pid;
      WindowUtinity.ShowView(uc, "UPDATE TRAN LOCATION NOTE", false, ViewState.MainWindow);

      // Search Grid Again 
      this.Search();
    }
    #endregion Event
  }
}
