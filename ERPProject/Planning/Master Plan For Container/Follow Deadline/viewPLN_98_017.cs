/*
  Author      : 
  Date        : 05/01/2014
  Description : Pcking and ASS Output
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_017 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPLN_98_017()
    {
      InitializeComponent();
    }

    private void viewPLN_98_017_Load(object sender, EventArgs e)
    {
      ultLoadingDateFrom.Value = DBNull.Value;
      ultLoadingDateTo.Value = DBNull.Value;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      // Check Data
      if (ultLoadingDateFrom.Value.ToString().Length == 0 ||
        DBConvert.ParseDateTime(ultLoadingDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Date From");
        return;
      }

      if (ultLoadingDateTo.Value.ToString().Length == 0 ||
        DBConvert.ParseDateTime(ultLoadingDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Date To");
        return;
      }
      // End Check Data
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@ShipDateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultLoadingDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      input[1] = new DBParameter("@ShipDateTo", DbType.DateTime, DBConvert.ParseDateTime(ultLoadingDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNMasterPlanPackingAndASSOutput_Exec", 300, input);
      if (dsSource != null)
      {
        ultPacking.DataSource = dsSource.Tables[0];
        ultASS.DataSource = dsSource.Tables[1];
      }
      btnSearch.Enabled = true;
    }

    ///// <summary>
    ///// ProcessCmdKey
    ///// </summary>
    ///// <param name="sender"></param>
    ///// <param name="e"></param>
    //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    //{
    //  if (keyData == Keys.Enter)
    //  {
    //    this.Search(); 
    //    return true;
    //  }
    //  return base.ProcessCmdKey(ref msg, keyData);
    //}

    #endregion Function

    #region Event
    private void ultPacking_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      //e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["YearNo"].Hidden = true;
      e.Layout.Bands[0].Columns["WeekNo"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassWO"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["CarcassWO"].CellAppearance.FontData.Bold = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["RequiredCBM"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["PackingActualCBM"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["RemainCBM"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["RequiredQty"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["PackingActualQty"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["RemainQty"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Columns["YearNo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WeekNo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RequiredQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RequiredCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["PackingActualQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["PackingActualCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RemainQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RemainCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RequiredQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RequiredCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.000}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["PackingActualQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,##0}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["PackingActualCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[3].DisplayFormat = "{0:###,##0.000}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RemainQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[4].DisplayFormat = "{0:###,##0}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RemainCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[5].DisplayFormat = "{0:###,##0.000}";

      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultASS_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      //e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["YearNo"].Hidden = true;
      e.Layout.Bands[0].Columns["WeekNo"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassWO"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["CarcassWO"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["RequiredCBM"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["ASSActual"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["ASSActualCBM"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["RemainCBM"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["RequiredQty"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ASSActual"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["RemainQty"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Columns["YearNo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WeekNo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RequiredQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RequiredCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ASSActual"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ASSActualCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RemainQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RemainCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RequiredQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RequiredCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.000}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["ASSActual"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,##0}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["ASSActualCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[3].DisplayFormat = "{0:###,##0.000}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RemainQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[4].DisplayFormat = "{0:###,##0}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RemainCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[5].DisplayFormat = "{0:###,##0.000}";

      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      btnExportExcel.Enabled = false;

      string strTemplateName = "RPT_PLN_098_017";
      string strSheetName = "Sheet1";
      string strOutFileName = "Output Packing And ASS";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // (1): Output Packing
      int count = 0;
      for (int i = 0; i < ultPacking.Rows.Count; i++)
      {
        UltraGridRow row = ultPacking.Rows[i];
        int ro = 10 + count;
        for (int j = 2; j < ultPacking.Rows[i].Cells.Count; j++) // j = 2 (Loai bo YearNo, MonthNo)
        {
          string col = System.Convert.ToChar(66 + j - 2).ToString(); // J - 2: (Colum dau tien khi gan excel)
          if (DBConvert.ParseDouble(row.Cells[j].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Value = DBConvert.ParseDouble(row.Cells[j].Value.ToString());
          }
          else
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Value = row.Cells[j].Value.ToString();
          }
          oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.LineLeft = xlLineStyle.lsNormal;
          oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.LineRight = xlLineStyle.lsNormal;
          oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.LineBottom = xlLineStyle.lsNormal;
          oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.LineTop = xlLineStyle.lsNormal;
          if (String.Compare(row.Cells[j].Column.ToString(), "CarcassWO", true) == 0)
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.FontColor = xlColor.xcBlue;
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.FontStyle = xlFontStyle.xsBold;
          }
          else if (String.Compare(row.Cells[j].Column.ToString(), "RequiredCBM", true) == 0 ||
            String.Compare(row.Cells[j].Column.ToString(), "PackingActualCBM", true) == 0 ||
            String.Compare(row.Cells[j].Column.ToString(), "RemainCBM", true) == 0)
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.BackColor = xlColor.xcYellow;
          }
          else if (String.Compare(row.Cells[j].Column.ToString(), "RequiredQty", true) == 0 ||
            String.Compare(row.Cells[j].Column.ToString(), "PackingActualQty", true) == 0 ||
            String.Compare(row.Cells[j].Column.ToString(), "RemainQty", true) == 0)
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.BackColor = xlColor.xcCyan;
          }
        }
        count = count + 1;
      }

      // (2): Output ASS
      int countASS = 0;
      for (int n = 0; n < ultASS.Rows.Count; n++)
      {
        UltraGridRow rowASS = ultASS.Rows[n];
        int ro = 10 + countASS;
        for (int m = 2; m < ultASS.Rows[n].Cells.Count; m++) //m = 2 (Loai bo YearNo, MonthNo)
        {
          string col = System.Convert.ToChar(74 + m - 2).ToString(); // m - 2: (Colum dau tien khi gan excel)
          if (DBConvert.ParseDouble(rowASS.Cells[m].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Value = DBConvert.ParseDouble(rowASS.Cells[m].Value.ToString());
          }
          else
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Value = rowASS.Cells[m].Value.ToString();
          }
          oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.LineLeft = xlLineStyle.lsNormal;
          oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.LineRight = xlLineStyle.lsNormal;
          oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.LineBottom = xlLineStyle.lsNormal;
          oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.LineTop = xlLineStyle.lsNormal;
          if (String.Compare(rowASS.Cells[m].Column.ToString(), "CarcassWO", true) == 0)
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.FontColor = xlColor.xcBlue;
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.FontStyle = xlFontStyle.xsBold;
          }
          else if (String.Compare(rowASS.Cells[m].Column.ToString(), "RequiredCBM", true) == 0 ||
            String.Compare(rowASS.Cells[m].Column.ToString(), "ASSActualCBM", true) == 0 ||
            String.Compare(rowASS.Cells[m].Column.ToString(), "RemainCBM", true) == 0)
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.BackColor = xlColor.xcYellow;
          }
          else if (String.Compare(rowASS.Cells[m].Column.ToString(), "RequiredQty", true) == 0 ||
           String.Compare(rowASS.Cells[m].Column.ToString(), "ASSActual", true) == 0 ||
           String.Compare(rowASS.Cells[m].Column.ToString(), "RemainQty", true) == 0)
          {
            oXlsReport.Cell(string.Format("{0}{1}:{2}{3}", col, ro, col, ro)).Attr.BackColor = xlColor.xcCyan;
          }
        }
        countASS = countASS + 1;
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);

      btnExportExcel.Enabled = true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event

    private void Clear_Click(object sender, EventArgs e)
    {
      ultLoadingDateFrom.Value = DBNull.Value;
      ultLoadingDateTo.Value = DBNull.Value;
    }
  }
}
