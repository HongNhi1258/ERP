/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: ViewPLN_15_007.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class ViewPLN_15_007 : MainUserControl
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadCBCarcassWo();
    }

    private void LoadCBCarcassWo()
    {
      string cm = @"Select Pid, CarcassWONo from TblPLNCarcassWOBalaneMaster";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraCombo(ultCBCarcassWO, dt, "Pid", "CarcassWONo", false, "Pid");
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      if (ultCBCarcassWO.Value == null || ultCBCarcassWO.Value.ToString().Trim().Length <= 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarningFromText("Carcass Wo must be selected");
        return;
      }
      btnSearch.Enabled = false;
      int paramNumber = 1;
      string storeName = "spPLNCarcassWOFollow_Select";

      DBParameter[] inputParam = new DBParameter[paramNumber];

      inputParam[0] = new DBParameter("@CarcassWoPid", DbType.Int64, ultCBCarcassWO.Value.ToString());

      DataSet dtSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      dtSource.Relations.Add(new DataRelation("dtParent_dtChildDiff", new DataColumn[] { dtSource.Tables[0].Columns["ItemCode"], dtSource.Tables[0].Columns["Revision"], dtSource.Tables[0].Columns["WO"] }, new DataColumn[] { dtSource.Tables[1].Columns["ItemCode"], dtSource.Tables[1].Columns["Revision"], dtSource.Tables[1].Columns["WO"] }, false));

      ultraGridInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
      ultraGridInformation.Rows.ExpandAll(true);

    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      this.ultCBCarcassWO.Text = string.Empty;
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }
    #endregion function

    #region event
    public ViewPLN_15_007()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewPLN_15_007_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      e.Layout.Bands[1].Columns["StandByEn"].Header.Caption = "Work Area";
      e.Layout.Bands[0].Columns["Carcass Remark"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["PLN Remark"].CellAppearance.BackColor = Color.SkyBlue;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.SkyBlue;
      e.Layout.Bands[1].Columns["WO"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;

    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (ultraGridInformation.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, out xlBook, "Follow CarcassWo List", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 2] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("B1", "B1");

        xlSheet.Cells[2, 2] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 2] = "Follow CarcassWo List";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 4] = "Row: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 5] = DBConvert.ParseInt(ultraGridInformation.Rows.Count.ToString());
        xlSheet.Cells[4, 6] = "(You can change rows count if you want)";

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }

    private void button2_Click(object sender, EventArgs e)
    {
      ViewPLN_15_008 view = new ViewPLN_15_008();
      WindowUtinity.ShowView(view, "Swap Carcass WO", true, ViewState.MainWindow);
    }
    #endregion event


  }
}
