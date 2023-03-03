/*
  Author      : Xuan Truong
  Date        : 23/10/2012
  Description : Add Multi Materials
*/
using DaiCo.Application;
using DaiCo.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_07_003 : MainUserControl
  {
    #region Field
    public int whPid = 0;
    public DataTable dtDetail = new DataTable();
    #endregion Field

    #region Init
    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public viewWHD_07_003()
    {
      InitializeComponent();
      dtDetail = this.CreateDataTable();
    }

    /// <summary>
    /// Create data table
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Selected", typeof(System.Int32));
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("NameEN", typeof(System.String));
      dt.Columns.Add("NameVN", typeof(System.String));
      dt.Columns.Add("Unit", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("Location", typeof(System.String));
      dt.Columns.Add("LocationPid", typeof(System.Int64));
      return dt;
    }

    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_07_003_Load(object sender, EventArgs e)
    {
      // Load UltraCombo MaterialCode
      Utility.LoadUltraCBMaterialListByWHPid(ultMaterialCode, this.whPid);
      // Load UltraCombo Location
      Utility.LoadUltraCBLocationListByWHPid(ultLocation, this.whPid);
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] param = new DBParameter[3];
      if (ultMaterialCode.Value != null)
      {
        param[0] = new DBParameter("@MaterialCode", DbType.AnsiString, ultMaterialCode.Value.ToString());
      }
      if (ultLocation.Value != null)
      {
        param[1] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(ultLocation.Value.ToString()));
      }
      param[2] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);

      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spWHDAddMultiDetailMaterials_Select", param);
      if (dtSource == null)
      {
        return;
      }
      if (dtDetail.Rows.Count == 0)
      {
        ultData.DataSource = dtSource;
        return;
      }
      DataTable temp = this.CreateDataTable();
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtDetail.NewRow();
        DataRow[] foundRows = dtDetail.Select("MaterialCode = '" + dtSource.Rows[i]["MaterialCode"].ToString() + "'");
        if (foundRows.Length > 0)
        {
          continue;
        }
        DataRow rows = temp.NewRow();
        rows["MaterialCode"] = dtSource.Rows[i]["MaterialCode"].ToString();
        rows["NameEN"] = dtSource.Rows[i]["NameEN"].ToString();
        rows["NameVN"] = dtSource.Rows[i]["NameVN"].ToString();
        rows["Unit"] = dtSource.Rows[i]["Unit"].ToString();
        rows["Qty"] = DBConvert.ParseDouble(dtSource.Rows[i]["Qty"].ToString());
        rows["Location"] = dtSource.Rows[i]["Location"].ToString();
        rows["LocationPid"] = DBConvert.ParseLong(dtSource.Rows[i]["LocationPid"].ToString());
        temp.Rows.Add(rows);
      }
      ultData.DataSource = temp;
      for (int a = 0; a < ultData.Rows.Count; a++)
      {
        ultData.Rows[a].Cells["Selected"].Value = 0;
      }
      int count = dtSource.Columns.Count;
      for (int i = 1; i < count; i++)
      {
        ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
    }

    #endregion Init

    #region Event

    /// <summary>
    /// InitializeLayout ultGridview
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Name VN";
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 70;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 110;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 110;
      e.Layout.Bands[0].Columns["Location"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Location"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;

      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      //Sum Qty, CBM And IssueQty
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Show Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnShowData_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }

    /// <summary>
    /// CheckedChanged Check All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkAll_CheckedChanged(object sender, EventArgs e)
    {
      int check = chkAll.Checked ? 1 : 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Cells["Selected"].Value = check;
      }
    }

    /// <summary>
    /// Save and Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveClose_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (string.Compare(ultData.Rows[i].Cells["Selected"].Value.ToString(), "1", true) == 0)
        {
          DataRow row = dtDetail.NewRow();
          row["MaterialCode"] = ultData.Rows[i].Cells["MaterialCode"].Value.ToString();
          row["NameEN"] = ultData.Rows[i].Cells["NameEN"].Value.ToString();
          row["NameVN"] = ultData.Rows[i].Cells["NameVN"].Value.ToString();
          row["Unit"] = ultData.Rows[i].Cells["Unit"].Value.ToString();
          row["Qty"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Qty"].Value.ToString());
          row["Location"] = ultData.Rows[i].Cells["Location"].Value.ToString();
          row["LocationPid"] = DBConvert.ParseLong(ultData.Rows[i].Cells["LocationPid"].Value.ToString());
          dtDetail.Rows.Add(row);
        }
      }
      this.CloseTab();

    }

    /// <summary>
    /// Save and Continue
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveContinue_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (string.Compare(ultData.Rows[i].Cells["Selected"].Value.ToString(), "1", true) == 0)
        {
          DataRow row = dtDetail.NewRow();
          row["MaterialCode"] = ultData.Rows[i].Cells["MaterialCode"].Value.ToString();
          row["NameEN"] = ultData.Rows[i].Cells["NameEN"].Value.ToString();
          row["NameVN"] = ultData.Rows[i].Cells["NameVN"].Value.ToString();
          row["Unit"] = ultData.Rows[i].Cells["Unit"].Value.ToString();
          row["Qty"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Qty"].Value.ToString());
          row["Location"] = ultData.Rows[i].Cells["Location"].Value.ToString();
          row["LocationPid"] = DBConvert.ParseLong(ultData.Rows[i].Cells["LocationPid"].Value.ToString());
          dtDetail.Rows.Add(row);
        }
      }
      this.LoadData();

    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExit_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event

  }
}
