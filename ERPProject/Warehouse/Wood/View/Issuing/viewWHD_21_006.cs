/*
  Author      : TRAN HUNG
  Date        : 15/07/2012
  Description : Show Woods Warehouse
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_21_006 : MainUserControl
  {
    #region Field
    public DataTable dtDetail = new DataTable();
    #endregion Field

    #region Init
    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public viewWHD_21_006()
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
      dt.Columns.Add("IDWood", typeof(System.String));
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("NameEN", typeof(System.String));
      dt.Columns.Add("NameVN", typeof(System.String));
      dt.Columns.Add("Unit", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("Width", typeof(System.Double));
      dt.Columns.Add("Length", typeof(System.Double));
      dt.Columns.Add("Thickness", typeof(System.Double));
      dt.Columns.Add("CBM", typeof(System.Double));
      dt.Columns.Add("PackageName", typeof(System.String));
      dt.Columns.Add("Location", typeof(System.String));
      dt.Columns.Add("IssueQty", typeof(System.Double));
      dt.Columns.Add("Type", typeof(System.Int32));
      return dt;
    }

    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_21_006_Load(object sender, EventArgs e)
    {
      // Load UltraCombo MaterialCode
      this.LoadComboMaterialCode();
      // Load UltraCombo Location
      this.LoadComboLocation();
    }

    /// <summary>
    /// LoadComboMaterialCode
    /// </summary>
    private void LoadComboMaterialCode()
    {
      string commandText = string.Empty;
      //commandText += "  SELECT SP.ID_SanPham MaterialCode, SP.ID_SanPham + ' - ' + SP.TenEnglish Name";
      //commandText += "  FROM VWHDMaterialCodeCommon SP";
      //commandText += "  WHERE IDTinhTrang = 9 AND [Status] = 2 AND SP.ID_SanPham LIKE '012%'";

      commandText += " SELECT MaterialCode, MaterialCode + ' - '+MaterialNameEn AS Name";
      commandText += " FROM VBOMMaterials";
      commandText += " WHERE MaterialCode LIKE '012%'";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultMaterialCode.DataSource = dtSource;
      ultMaterialCode.DisplayMember = "Name";
      ultMaterialCode.ValueMember = "MaterialCode";
      ultMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["Name"].Width = 369;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = true;
    }

    /// <summary>
    /// LoadComboLocation
    /// </summary>
    private void LoadComboLocation()
    {
      string commandText = "SELECT Pid , Name FROM VWHDListLocation WHERE Kho = 3";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultLocation.DataSource = dtSource;
      ultLocation.DisplayMember = "Name";
      ultLocation.ValueMember = "Pid";
      ultLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLocation.DisplayLayout.Bands[0].Columns["Name"].Width = 369;
      ultLocation.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// LoadComboPackage
    /// </summary>
    private void LoadComboPackage()
    {
      if (ultLocation.Value != null && ultLocation.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText += " SELECT IDPackage,PackageName FROM VWHDPackageWoods";
        commandText += " WHERE ID_Position = " + ultLocation.Value.ToString();
        System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        ultPackage.Text = string.Empty;
        ultPackage.DataSource = dtSource;
        ultPackage.DisplayMember = "PackageName";
        ultPackage.ValueMember = "IDPackage";
        ultPackage.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultPackage.DisplayLayout.Bands[0].Columns["PackageName"].Width = 369;
        ultPackage.DisplayLayout.Bands[0].Columns["IDPackage"].Hidden = true;
      }
      else
      {
        ultPackage.Text = string.Empty;
      }
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] param = new DBParameter[10];
      if (ultMaterialCode.Value != null)
      {
        param[0] = new DBParameter("@MaterialCode", DbType.AnsiString, ultMaterialCode.Value.ToString());
      }
      if (ultPackage.Value != null)
      {
        param[1] = new DBParameter("@Package", DbType.AnsiString, ultPackage.Value.ToString());
      }
      double LengthFrom = DBConvert.ParseDouble(txtLengthFrom.Text.Trim());
      if (LengthFrom != double.MinValue)
      {
        param[2] = new DBParameter("@LengthFrom", DbType.Double, LengthFrom);
      }
      double LengthTo = DBConvert.ParseDouble(txtLengthTo.Text.Trim());
      if (LengthTo != double.MinValue)
      {
        param[3] = new DBParameter("@LengthTo", DbType.Double, LengthTo);
      }
      double WidthFrom = DBConvert.ParseDouble(txtWidthFrom.Text.Trim());
      if (WidthFrom != double.MinValue)
      {
        param[4] = new DBParameter("@WidthFrom", DbType.Double, WidthFrom);
      }
      double WidthTo = DBConvert.ParseDouble(txtWidthTo.Text.Trim());
      if (WidthTo != double.MinValue)
      {
        param[5] = new DBParameter("@WidthTo", DbType.Double, WidthTo);
      }
      double ThicknessFrom = DBConvert.ParseDouble(txtThicknessFrom.Text.Trim());
      if (ThicknessFrom != double.MinValue)
      {
        param[6] = new DBParameter("@ThicknessFrom", DbType.Double, ThicknessFrom);
      }
      double ThicknessTo = DBConvert.ParseDouble(txtThicknessTo.Text.Trim());
      if (ThicknessTo != double.MinValue)
      {
        param[7] = new DBParameter("@ThicknessTo", DbType.Double, ThicknessTo);
      }

      if (this.txtWO.Text.Length > 0 && DBConvert.ParseInt(this.txtWO.Text) != int.MinValue)
      {
        param[8] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(this.txtWO.Text));
      }

      if (this.txtCarcassCode.Text.Length > 0)
      {
        param[9] = new DBParameter("@CarcassCode", DbType.String, this.txtCarcassCode.Text);
      }
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spWHDStockBalanceWoods_Select", param);
      if (dtSource == null)
      {
        return;
      }
      if (dtDetail.Rows.Count == 0)
      {
        ultData.DataSource = dtSource;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["Type"].Value.ToString()) == 1)
          {
            ultData.Rows[i].Cells["IDWood"].Appearance.BackColor = Color.Yellow;
          }
        }
        return;
      }
      DataTable temp = this.CreateDataTable();
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtDetail.NewRow();
        DataRow[] foundRows = dtDetail.Select("IDWood = '" + dtSource.Rows[i]["IDWood"].ToString() + "'");
        if (foundRows.Length > 0)
        {
          continue;
        }
        DataRow rows = temp.NewRow();
        rows["IDWood"] = dtSource.Rows[i]["IDWood"].ToString();
        rows["MaterialCode"] = dtSource.Rows[i]["MaterialCode"].ToString();
        rows["NameEN"] = dtSource.Rows[i]["NameEN"].ToString();
        rows["NameVN"] = dtSource.Rows[i]["NameVN"].ToString();
        rows["Unit"] = dtSource.Rows[i]["Unit"].ToString();
        rows["Qty"] = DBConvert.ParseDouble(dtSource.Rows[i]["Qty"].ToString());
        rows["Width"] = DBConvert.ParseDouble(dtSource.Rows[i]["Width"].ToString());
        rows["Length"] = DBConvert.ParseDouble(dtSource.Rows[i]["Length"].ToString());
        rows["Thickness"] = DBConvert.ParseDouble(dtSource.Rows[i]["Thickness"].ToString());
        rows["CBM"] = DBConvert.ParseDouble(dtSource.Rows[i]["CBM"].ToString());
        rows["PackageName"] = dtSource.Rows[i]["PackageName"].ToString();
        rows["Location"] = dtSource.Rows[i]["Location"].ToString();
        rows["IssueQty"] = DBConvert.ParseDouble(dtSource.Rows[i]["IssueQty"].ToString());
        rows["Type"] = DBConvert.ParseInt(dtSource.Rows[i]["Type"].ToString());
        temp.Rows.Add(rows);
      }
      ultData.DataSource = temp;
      for (int a = 0; a < ultData.Rows.Count; a++)
      {
        ultData.Rows[a].Cells["Selected"].Value = 0;
        if (DBConvert.ParseInt(ultData.Rows[a].Cells["Type"].Value.ToString()) == 1)  // Allocation WO & Carcass
        {
          ultData.Rows[a].Cells["IDWood"].Appearance.BackColor = Color.Yellow;
        }
      }
      int count = temp.Columns.Count;
      for (int i = 1; i < count; i++)
      {
        ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
    }

    #endregion Init

    #region Event
    /// <summary>
    /// TextChanged ultLocation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultLocation_TextChanged(object sender, EventArgs e)
    {
      this.LoadComboPackage();
    }

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

      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Bands[0].Columns["IDWood"].Header.Caption = "ID Wood";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["CBM"].Header.Caption = "CBM";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Name VN";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Unit";
      e.Layout.Bands[0].Columns["PackageName"].Header.Caption = "Package";
      e.Layout.Bands[0].Columns["IssueQty"].Header.Caption = "Issue Qty";

      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 70;

      e.Layout.Bands[0].Columns["IDWood"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["IDWood"].MinWidth = 70;

      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;

      e.Layout.Bands[0].Columns["PackageName"].MaxWidth = 110;
      e.Layout.Bands[0].Columns["PackageName"].MinWidth = 110;

      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;

      e.Layout.Bands[0].Columns["Length"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Length"].MinWidth = 70;

      e.Layout.Bands[0].Columns["Width"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Width"].MinWidth = 70;

      e.Layout.Bands[0].Columns["Thickness"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Thickness"].MinWidth = 70;

      e.Layout.Bands[0].Columns["CBM"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["CBM"].MinWidth = 80;

      e.Layout.Bands[0].Columns["IssueQty"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["IssueQty"].MinWidth = 120;

      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["IssueQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["IssueQty"].CellAppearance.ForeColor = Color.Blue;

      //Sum Qty, CBM And IssueQty
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["CBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["IssueQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.0000}";
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,##0.0000}";
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
      this.btnShowData.Enabled = false;

      this.LoadData();

      this.btnShowData.Enabled = true;
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
          row["IDWood"] = ultData.Rows[i].Cells["IDWood"].Value.ToString();
          row["MaterialCode"] = ultData.Rows[i].Cells["MaterialCode"].Value.ToString();
          row["NameEN"] = ultData.Rows[i].Cells["NameEN"].Value.ToString();
          row["NameVN"] = ultData.Rows[i].Cells["NameVN"].Value.ToString();
          row["Unit"] = ultData.Rows[i].Cells["Unit"].Value.ToString();
          row["Qty"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Qty"].Value.ToString());
          row["Width"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Width"].Value.ToString());
          row["Length"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Length"].Value.ToString());
          row["Thickness"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Thickness"].Value.ToString());
          row["CBM"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["CBM"].Value.ToString());
          row["PackageName"] = ultData.Rows[i].Cells["PackageName"].Value.ToString();
          row["Location"] = ultData.Rows[i].Cells["Location"].Value.ToString();
          row["IssueQty"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["IssueQty"].Value.ToString());
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
          row["IDWood"] = ultData.Rows[i].Cells["IDWood"].Value.ToString();
          row["MaterialCode"] = ultData.Rows[i].Cells["MaterialCode"].Value.ToString();
          row["NameEN"] = ultData.Rows[i].Cells["NameEN"].Value.ToString();
          row["NameVN"] = ultData.Rows[i].Cells["NameVN"].Value.ToString();
          row["Unit"] = ultData.Rows[i].Cells["Unit"].Value.ToString();
          row["Qty"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Qty"].Value.ToString());
          row["Width"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Width"].Value.ToString());
          row["Length"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Length"].Value.ToString());
          row["Thickness"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["Thickness"].Value.ToString());
          row["CBM"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["CBM"].Value.ToString());
          row["PackageName"] = ultData.Rows[i].Cells["PackageName"].Value.ToString();
          row["IssueQty"] = DBConvert.ParseDouble(ultData.Rows[i].Cells["IssueQty"].Value.ToString());
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
