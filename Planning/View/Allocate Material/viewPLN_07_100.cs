/*
 * Created By   : 
 * Created Date : 08/02/2011
 * Description  : Allocate , Re-Allocate Department
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_100 : MainUserControl
  {
    #region Init
    public viewPLN_07_100()
    {
      InitializeComponent();
    }

    private void viewPLN_07_100_Load(object sender, EventArgs e)
    {
      // Load All Combo Init
      this.LoadCombo();
      ControlUtility.LoaducUltraListMaterialGroup(ucUltraListMaterialGroup);
    }
    #endregion Init

    #region function
    /// <summary>
    /// Load List Material
    /// </summary>
    private void LoadData_ucUltraListMaterial()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, txtMaterialGroup.Text);
      //inputParam[1] = new DBParameter("@ControlType", DbType.Int32, materialControlType); // = 1: Theo Wo, Item
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNGetListControlMaterial", inputParam);
      dtSource.Columns.Remove("Description");
      ucUltraListMaterial.DataSource = dtSource;
      ucUltraListMaterial.ColumnWidths = "100; 200";
      ucUltraListMaterial.DataBind();
      ucUltraListMaterial.ValueMember = "MaterialCode";
      txtMaterialCode.Text = string.Empty;
    }

    /// <summary>
    /// Get Text From List View
    /// </summary>
    /// <param name="lst"></param>
    /// <returns></returns>
    private string GetSelectedListView(ListView lst)
    {
      string result = string.Empty;
      foreach (ListViewItem item in lst.Items)
      {
        if (result.Length > 0)
        {
          result += "; ";
        }
        result += item.Text;
      }
      return result;
    }

    /// <summary>
    /// Search From Condition Above
    /// </summary>
    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[5];
      if (multiCBDepartment.SelectedValue != null && multiCBDepartment.SelectedValue.ToString().Length > 0)
      {
        inputParam[0] = new DBParameter("@Depaterment", DbType.AnsiString, 50, multiCBDepartment.SelectedValue.ToString());
      }

      if (txtMaterialGroup.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, txtMaterialGroup.Text.Trim());
      }

      if (txtMaterialCode.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@Materials", DbType.AnsiString, 4000, txtMaterialCode.Text.Trim());
      }

      if (multiCBMonth.SelectedValue != null && multiCBMonth.SelectedValue.ToString().Length > 0)
      {
        inputParam[3] = new DBParameter("@Month", DbType.Int32, DBConvert.ParseInt(multiCBMonth.SelectedValue.ToString()));
      }

      if (multiCBYear.SelectedValue != null && multiCBYear.SelectedValue.ToString().Length > 0)
      {
        inputParam[4] = new DBParameter("@Year", DbType.Int32, DBConvert.ParseInt(multiCBYear.SelectedValue.ToString()));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNAllocateForDepartmentInfomation", inputParam);
      ultraGridWOMaterialDetail.DataSource = dtSource;
    }

    /// <summary>
    /// Load Combo
    /// </summary>
    private void LoadCombo()
    {
      // Load Data For Department
      string commandText = "SELECT Department, DeparmentName FROM VHRDDepartment";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadMultiCombobox(multiCBDepartment, dtSource, "Department", "DepartmentName");
      multiCBDepartment.ColumnWidths = "100, 500";

      // Load Data For Month
      DataTable dtMonth = new DataTable();

      DataColumn Month = new DataColumn("Month");
      Month.DataType = System.Type.GetType("System.String");
      dtMonth.Columns.Add(Month);

      for (int i = 1; i < 13; i++)
      {
        DataRow drMonth = dtMonth.NewRow();
        drMonth["Month"] = i.ToString(); ;
        dtMonth.Rows.Add(drMonth);
      }

      ControlUtility.LoadMultiCombobox(multiCBMonth, dtMonth, "Month", "Month");
      // Load Data For Year
      DataTable dtYear = new DataTable();

      DataColumn Year = new DataColumn("Year");
      Year.DataType = System.Type.GetType("System.String");
      dtYear.Columns.Add(Year);

      for (int i = DateTime.Now.Year - 3; i < DateTime.Now.Year + 3; i++)
      {
        DataRow drYear = dtYear.NewRow();
        drYear["Year"] = i.ToString();
        dtYear.Rows.Add(drYear);
      }
      ControlUtility.LoadMultiCombobox(multiCBYear, dtYear, "Year", "Year");
    }
    #endregion function

    #region Event
    /// <summary>
    /// Check visible Material Group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowMaterialListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListMaterialGroup.Visible = chkShowMaterialListBox.Checked;
    }
    /// <summary>
    /// Check visible Material Code
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowMaterialCodeListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListMaterial.Visible = chkShowMaterialCodeListBox.Checked;
    }

    /// <summary>
    /// Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Layout Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridWOMaterialDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridWOMaterialDetail);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["Month"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Year"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Department"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalAllocate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Issued"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["StockQty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Month"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Year"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["TotalAllocate"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Issued"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["StockQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["TotalAllocate"].Header.Caption = "Total Allocate";
      e.Layout.Bands[0].Columns["StockQty"].Header.Caption = "Stock Qty";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
    }

    /// <summary>
    /// Open Allocate Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAllocate_Click(object sender, EventArgs e)
    {
      viewPLN_07_101 view = new viewPLN_07_101();
      WindowUtinity.ShowView(view, "Increase", true, ViewState.ModalWindow);
      this.Search();
    }

    /// <summary>
    /// Open ReAllocate Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReAllocate_Click(object sender, EventArgs e)
    {
      viewPLN_07_102 view = new viewPLN_07_102();
      WindowUtinity.ShowView(view, "Decrease", true, ViewState.ModalWindow);
      this.Search();
    }

    /// <summary>
    /// Show History Department Allocation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridWOMaterialDetail_DoubleClick(object sender, EventArgs e)
    {
      if (ultraGridWOMaterialDetail.Selected.Rows.Count > 0 && ultraGridWOMaterialDetail.Selected.Rows[0].Band.ParentBand == null)
      {
        long pid = DBConvert.ParseLong(ultraGridWOMaterialDetail.Selected.Rows[0].Cells["Pid"].Value.ToString());

        viewPLN_07_103 view = new viewPLN_07_103();
        view.departmentAllocatePid = pid;
        WindowUtinity.ShowView(view, "Historical Department Allocation", true, ViewState.ModalWindow);
      }
    }
    /// <summary>
    /// Value Change Material Group
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucUltraListMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialGroup.Text = ucUltraListMaterialGroup.SelectedValue;
      this.LoadData_ucUltraListMaterial();
    }
    /// <summary>
    /// Value Change Material
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucUltraListMaterial_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialCode.Text = ucUltraListMaterial.SelectedValue;
    }

    private void btnAdjustByExcel_Click(object sender, EventArgs e)
    {
      viewPLN_07_105 view = new viewPLN_07_105();
      WindowUtinity.ShowView(view, "Adjust Qty Material", true, ViewState.ModalWindow);
      this.Search();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridWOMaterialDetail, "Department Allocation");
    }
    #endregion Event
  }
}
