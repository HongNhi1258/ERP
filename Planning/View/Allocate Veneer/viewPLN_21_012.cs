/*
 * Created By   : 
 * Created Date : 20/03/2013
 * Description  : Planning Allocate WO Veneer
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_012 : MainUserControl
  {
    #region field
    public viewPLN_21_012()
    {
      InitializeComponent();
    }
    #endregion field

    #region init
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_07_003_Load(object sender, EventArgs e)
    {
      // Load ItemCode
      this.LoadItemCode();

      // Load Material Code
      this.LoadMaterialCode();
    }
    #endregion init

    #region function
    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadItemCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT ItemCode, Name ItemName";
      commandText += " FROM TblBOMItemBasic ";
      commandText += " ORDER BY ItemCode ";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucItemCode.DataSource = dtItem;
      ucItemCode.ColumnWidths = "200;400";
      ucItemCode.DataBind();
      ucItemCode.ValueMember = "ItemCode";
    }

    /// <summary>
    /// Load Material Code
    /// </summary>
    private void LoadMaterialCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT MAT.MaterialCode Code, MAT.NameEN GroupName ";
      commandText += " FROM TblGNRMaterialInformation MAT ";
      commandText += " 	INNER JOIN TblGNRMaterialGroup GRP ON MAT.[Group] = GRP.[Group] ";
      commandText += "          									AND GRP.Warehouse = 2";
      commandText += " WHERE MAT.IsControl = 1 ";
      commandText += "      ORDER BY MAT.MaterialCode ";

      DataTable dtGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucMaterialCode.DataSource = dtGroup;
      ucMaterialCode.ColumnWidths = "200;400";
      ucMaterialCode.DataBind();
      ucMaterialCode.ValueMember = "Code";
      ucMaterialCode.DisplayMember = "Code";
    }

    /// <summary>
    /// Load Item Base On WO
    /// </summary>
    private void LoadItemBaseOnWO()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ITEM.ItemCode, ITEM.Name ItemName  ";
      commandText += " FROM TblBOMItemBasic ITEM  ";
      commandText += " 	  INNER JOIN TblPLNWOInfoDetailGeneral WO ON ITEM.ItemCode = WO.ItemCode ";
      commandText += " WHERE 1 = 1 ";
      if (txtWOFrom.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOFrom.Text.Trim()) != int.MinValue)
      {
        commandText += " 	AND WO.WoInfoPID >= " + DBConvert.ParseInt(txtWOFrom.Text.Trim());
      }

      if (txtWOTo.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOTo.Text.Trim()) != int.MinValue)
      {
        commandText += " 	AND WO.WoInfoPID <= " + DBConvert.ParseInt(txtWOTo.Text.Trim());
      }

      DataTable dtGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucItemCode.DataSource = dtGroup;
      ucItemCode.ColumnWidths = "200;400";
      ucItemCode.DataBind();
      ucItemCode.ValueMember = "ItemCode";
      ucItemCode.DisplayMember = "ItemCode";
    }

    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[5];

      // WO From
      if (txtWOFrom.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOFrom.Text.Trim()) != int.MinValue)
      {
        inputParam[0] = new DBParameter("@WOFrom", DbType.Int32, DBConvert.ParseInt(txtWOFrom.Text.Trim()));
      }

      // WO To
      if (txtWOTo.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOTo.Text.Trim()) != int.MinValue)
      {
        inputParam[1] = new DBParameter("@WOTo", DbType.Int32, DBConvert.ParseInt(txtWOTo.Text.Trim()));
      }

      // Carcass Code
      if (this.txtCarcassCode.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 256, this.txtCarcassCode.Text.Trim());
      }

      // ItemCode
      if (txtItemCode.Text.Trim().Length > 0)
      {
        inputParam[3] = new DBParameter("@ItemCode", DbType.AnsiString, 256, txtItemCode.Text.Trim());
      }

      // MaterialCode
      if (txtMaterialCode.Text.Trim().Length > 0)
      {
        inputParam[4] = new DBParameter("@MaterialCode", DbType.AnsiString, 128, txtMaterialCode.Text.Trim());
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNVeneerPlanningAllocateWO_Select", inputParam);
      ultraGridWOMaterialDetail.DataSource = dtSource;
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        int closeWork = DBConvert.ParseInt(ultraGridWOMaterialDetail.Rows[i].Cells["CloseWork"].Value.ToString());
        if (closeWork == 1)
        {
          ultraGridWOMaterialDetail.Rows[i].Appearance.BackColor = Color.Pink;
        }
      }
    }

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
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultraGridWOMaterialDetail.Rows.Count > 0)
      {
        ControlUtility.ExportToExcelWithDefaultPath(ultraGridWOMaterialDetail, "Veneer Allocation Information");
        //Excel.Workbook xlBook;
        //ControlUtility.ExportToExcelWithDefaultPath(ultraGridWOMaterialDetail, out xlBook, "Veneer Allocation Information", 6);

        //string filePath = xlBook.FullName;
        //Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        //xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        //Excel.Range r = xlSheet.get_Range("A1", "A1");

        //xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        //xlSheet.Cells[3, 1] = "Veneer Allocation Information";
        //r = xlSheet.get_Range("A3", "A3");
        //r.Font.Bold = true;
        //r.Font.Size = 14;
        //r.EntireRow.RowHeight = 20;

        //xlSheet.Cells[4, 1] = "Date: ";
        //r.Font.Bold = true;
        //xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        ////xlBook.Application.DisplayAlerts = false;

        //xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        //Process.Start(filePath);
      }
    }
    #endregion function

    #region event
    private void chkShowItemListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucItemCode.Visible = chkShowItemListBox.Checked;
    }

    private void chkShowMaterialListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucMaterialCode.Visible = chkShowMaterialListBox.Checked;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;

      // Search
      this.Search();

      this.btnSearch.Enabled = true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultraGridWOMaterialDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["CloseWork"].Hidden = true;
      // Set Align
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CarQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyUnit"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Allocated"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AllocatedSpecial"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Supplement"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Required"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Issued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["NonIssue"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["NonAllocate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Stock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyReallocate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalQty"].Header.Caption = "Current BOM";
      e.Layout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }


    private void btnPrint_Click(object sender, EventArgs e)
    {
      //string strTemplateName = "RPT_PLN_21_012";
      //string strSheetName = "Sheet1";
      //string strOutFileName = "Veneer Allocation Infomation";
      //string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //DataTable dtData = (DataTable)ultraGridWOMaterialDetail.DataSource;

      //if (dtData != null && dtData.Rows.Count > 0)
      //{
      //  for (int i = 0; i < dtData.Rows.Count; i++)
      //  {
      //    DataRow dtRow = dtData.Rows[i];
      //    if (i > 0)
      //    {
      //      oXlsReport.Cell("B8:Q8").Copy();
      //      oXlsReport.RowInsert(7 + i);
      //      oXlsReport.Cell("B8:Q8", 0, i).Paste();
      //    }
      //    oXlsReport.Cell("**No", 0, i).Value = i + 1;
      //    oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseLong(dtRow["WO"].ToString());
      //    oXlsReport.Cell("**CarcassCode", 0, i).Value = dtRow["CarcassCode"].ToString();
      //    oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
      //    if (DBConvert.ParseInt(dtRow["CarQty"].ToString()) != int.MinValue)
      //    {
      //      oXlsReport.Cell("**ItemQty", 0, i).Value = DBConvert.ParseInt(dtRow["CarQty"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**ItemQty", 0, i).Value = DBNull.Value;
      //    }

      //    oXlsReport.Cell("**MainCode", 0, i).Value = dtRow["MainCode"].ToString();
      //    oXlsReport.Cell("**Name", 0, i).Value = dtRow["MainName"].ToString();
      //    if (dtRow["AltCode"].ToString().Length > 0)
      //    {
      //      oXlsReport.Cell("**AltCode", 0, i).Value = dtRow["AltCode"].ToString();
      //    }
      //    if (dtRow["AltName"].ToString().Length > 0)
      //    {
      //      oXlsReport.Cell("**AltName", 0, i).Value = dtRow["AltName"].ToString();
      //    }

      //    if (DBConvert.ParseDouble(dtRow["QtyUnit"].ToString()) != double.MinValue)
      //    {
      //      oXlsReport.Cell("**MaterialItem", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyUnit"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**MaterialItem", 0, i).Value = DBNull.Value;
      //    }

      //    if (DBConvert.ParseDouble(dtRow["TotalQty"].ToString()) != double.MinValue)
      //    {
      //      oXlsReport.Cell("**TotalQty", 0, i).Value = DBConvert.ParseDouble(dtRow["TotalQty"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**TotalQty", 0, i).Value = DBNull.Value;
      //    }

      //    if (DBConvert.ParseDouble(dtRow["Allocated"].ToString()) != double.MinValue)
      //    {
      //      oXlsReport.Cell("**Allocated", 0, i).Value = DBConvert.ParseDouble(dtRow["Allocated"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**Allocated", 0, i).Value = DBNull.Value;
      //    }

      //    if (DBConvert.ParseDouble(dtRow["Supplement"].ToString()) != double.MinValue)
      //    {
      //      oXlsReport.Cell("**Supplement", 0, i).Value = DBConvert.ParseDouble(dtRow["Supplement"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**Supplement", 0, i).Value = DBNull.Value;
      //    }

      //    if (DBConvert.ParseDouble(dtRow["Issued"].ToString()) != double.MinValue)
      //    {
      //      oXlsReport.Cell("**Issued", 0, i).Value = DBConvert.ParseDouble(dtRow["Issued"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**Issued", 0, i).Value = DBNull.Value;
      //    }

      //    if (DBConvert.ParseDouble(dtRow["NonIssue"].ToString()) != double.MinValue)
      //    {
      //      oXlsReport.Cell("**NonIssue", 0, i).Value = DBConvert.ParseDouble(dtRow["NonIssue"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**NonIssue", 0, i).Value = DBNull.Value;
      //    }

      //    if (DBConvert.ParseDouble(dtRow["Stock"].ToString()) != double.MinValue)
      //    {
      //      oXlsReport.Cell("**Stock", 0, i).Value = DBConvert.ParseDouble(dtRow["Stock"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**Stock", 0, i).Value = DBNull.Value;
      //    }
      //  }
      //}

      //oXlsReport.Out.File(strOutFileName);
      //Process.Start(strOutFileName);

      this.ExportExcel();
    }

    private void ucUltraListItem_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtItemCode.Text = ucItemCode.SelectedValue;
    }

    private void ucUltraListMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialCode.Text = ucMaterialCode.SelectedValue;
    }

    private void txtWOFrom_Leave(object sender, EventArgs e)
    {
      // Load Item Base On WO
      this.LoadItemBaseOnWO();
    }

    private void txtWOTo_Leave(object sender, EventArgs e)
    {
      // Load Item Base On WO
      this.LoadItemBaseOnWO();
    }

    private void ultraGridWOMaterialDetail_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        if (ultraGridWOMaterialDetail.Selected.Rows.Count > 0)
        {
          viewPLN_21_005 view = new viewPLN_21_005();
          long wo = DBConvert.ParseLong(ultraGridWOMaterialDetail.Selected.Rows[0].Cells["Wo"].Value.ToString());
          string carcassCode = ultraGridWOMaterialDetail.Selected.Rows[0].Cells["CarcassCode"].Value.ToString();
          string mainCode = ultraGridWOMaterialDetail.Selected.Rows[0].Cells["MainCode"].Value.ToString();
          string altCode = ultraGridWOMaterialDetail.Selected.Rows[0].Cells["AltCode"].Value.ToString();

          view.wo = wo;
          view.carcassCode = carcassCode;
          view.mainCode = mainCode;
          view.altCode = altCode;
          WindowUtinity.ShowView(view, "HISTORICAL ALLOCATION INFORMATION", true, ViewState.Window);
        }
      }
      catch
      {
        WindowUtinity.ShowMessageError("MSG0011", "the row you want to allocate or register");
      }
    }

    private void btnDecrease_Click(object sender, EventArgs e)
    {
      viewPLN_21_003 view = new viewPLN_21_003();
      WindowUtinity.ShowView(view, "DECREASE", true, ViewState.ModalWindow);
    }

    private void chkAll_CheckedChanged(object sender, EventArgs e)
    {
      int check = (chkAll.Checked ? 1 : 0);
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        ultraGridWOMaterialDetail.Rows[i].Cells["Auto"].Value = check;
        if (check == 0)
        {
          ultraGridWOMaterialDetail.Rows[i].Cells["QtyReallocate"].Value = DBNull.Value;
        }
        else
        {
          ultraGridWOMaterialDetail.Rows[i].Cells["QtyReallocate"].Value = DBConvert.
                          ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["NonIssue"].Value.ToString());
        }
      }
    }

    private void ultraGridWOMaterialDetail_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      int value = DBConvert.ParseInt(e.Cell.Text.ToString());
      if (string.Compare(columnName, "Auto", true) == 0)
      {
        if (value == 1)
        {
          e.Cell.Row.Cells["QtyReallocate"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["NonIssue"].Value.ToString());
        }
        else
        {
          e.Cell.Row.Cells["QtyReallocate"].Value = DBNull.Value;
        }
      }
    }

    private void ultraGridWOMaterialDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      string commandText = string.Empty;
      long wo = 0;
      switch (columnName)
      {
        case "qtyreallocate":
          double remain = DBConvert.ParseDouble(e.Cell.Row.Cells["NonIssue"].Value.ToString());
          double qtyReallocate = DBConvert.ParseDouble(e.Cell.Row.Cells["QtyReallocate"].Value.ToString());
          if (qtyReallocate > remain)
          {
            WindowUtinity.ShowMessageErrorFromText("Qty Reallocate must less Non Issue Qty !, Please fill again");
            e.Cell.Row.Cells["QtyReallocate"].Value = 0;
          }
          break;
        default:
          break;
      }
    }

    private void btnReallocateQty_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < this.ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        UltraGridRow row = this.ultraGridWOMaterialDetail.Rows[i];
        if (DBConvert.ParseDouble(row.Cells["QtyReallocate"].Value.ToString()) != double.MinValue
            && DBConvert.ParseDouble(row.Cells["QtyReallocate"].Value.ToString()) > 0)
        {
          string storeName = "spPLNVeneerAllocateForWODecrease_Insert";
          DBParameter[] inputParam = new DBParameter[7];
          inputParam[0] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
          inputParam[1] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
          inputParam[2] = new DBParameter("@MainCode", DbType.String, row.Cells["MainCode"].Value.ToString());
          inputParam[3] = new DBParameter("@AltCode", DbType.String, row.Cells["AltCode"].Value.ToString());
          inputParam[4] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["QtyReallocate"].Value.ToString()));
          inputParam[5] = new DBParameter("@Remark", DbType.AnsiString, 4000, "");
          inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
        }
      }

      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.Search();
    }
    #endregion event
  }
}
