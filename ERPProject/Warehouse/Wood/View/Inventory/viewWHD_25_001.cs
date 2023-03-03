/*
  Author      : Xuan Truong
  Date        : 15/06/2012
  Description : Stock Count Location
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_25_001 : MainUserControl
  {
    #region Init
    public viewWHD_25_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewWHD_25_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_25_001_Load(object sender, EventArgs e)
    {
      //Load ultracombo Location
      this.LoadComboLocation();
    }

    /// <summary>
    /// Load UltraCombo Location
    /// </summary>
    private void LoadComboLocation()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_Position, Name";
      commandText += " FROM VWHDLocationWoods ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultLocation.DataSource = dtSource;
      ultLocation.DisplayMember = "Name";
      ultLocation.ValueMember = "ID_Position";
      ultLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLocation.DisplayLayout.Bands[0].Columns["Name"].Width = 450;
      ultLocation.DisplayLayout.Bands[0].Columns["ID_Position"].Hidden = true;
    }
    #endregion Init

    #region Event
    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLoad_Click(object sender, EventArgs e)
    {
      // Get path from Folder
      string path = @"\PhanmemDENSOBHT8000";
      path = Path.GetFullPath(path);

      string messageLocation = string.Empty;
      bool success = this.CheckValid(out messageLocation);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", messageLocation);
        return;
      }

      DataTable dtSource = this.CreateDataTable();

      string curFile = path + @"\THONGTIN.txt";

      if (!File.Exists(curFile))
      {
        string message = string.Format(DaiCo.Shared.Utility.FunctionUtility.GetMessage("ERR0011"), "THONGTIN.txt");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string[] a = File.ReadAllLines(curFile);

      if (a.Length == 0)
      {
        return;
      }

      int index = int.MinValue;
      if (a[0].ToString().Length > 0)
      {
        index = a[0].IndexOf("*");
      }

      for (int i = 0; i < a.Length; i++)
      {
        if (a[i].Trim().ToString().Length > 0 && index != -1)
        {
          DataRow row = dtSource.NewRow();
          index = a[i].IndexOf("*");
          a[i] = a[i].Substring(0, index).Trim().ToString();
          row["IDVeneer"] = a[i].ToString().Trim();
          row["Code"] = "0";
          row["Pcs"] = 0;
          row["Width"] = 0;
          row["Length"] = 0;
          row["Thickness"] = 0;
          row["Location"] = "0";
          dtSource.Rows.Add(row);
        }
        else if (a[i].Trim().ToString().Length > 0)
        {
          DataRow row = dtSource.NewRow();
          row["IDVeneer"] = a[i].ToString().Trim();
          row["Code"] = "0";
          row["Pcs"] = 0;
          row["Width"] = 0;
          row["Length"] = 0;
          row["Thickness"] = 0;
          row["Location"] = "0";
          dtSource.Rows.Add(row);
        }
      }

      //Get Data
      DataTable result = this.GetDataLoad(dtSource);

      this.ultData.DataSource = result;

      for (int j = 0; j < ultData.Rows.Count; j++)
      {
        UltraGridRow rowGrid = ultData.Rows[j];
        rowGrid.Activation = Activation.ActivateOnly;

        if (DBConvert.ParseInt(rowGrid.Cells["Errors"].Value.ToString()) == 1)
        {
          rowGrid.CellAppearance.BackColor = Color.Yellow;
        }
        else if (DBConvert.ParseInt(rowGrid.Cells["Errors"].Value.ToString()) == 2)
        {
          rowGrid.CellAppearance.BackColor = Color.Lime;
        }
      }
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Errors"].Hidden = true;

      e.Layout.Bands[0].Columns["IDVeneer"].Header.Caption = "Scan Barcode";
      e.Layout.Bands[0].Columns["LotNoId"].Header.Caption = "LotNo Id";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["TenEnglish"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["TenVietNam"].Header.Caption = "Name VN";
      e.Layout.Bands[0].Columns["TenDonViEN"].Header.Caption = "Unit";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[0].Columns["DateInStore"].Header.Caption = "Date In Store";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Location";

      e.Layout.Bands[0].Columns["IDVeneer"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["LotNoId"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["MaterialCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["TenEnglish"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["TenVietNam"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["TenDonViEN"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DateInStore"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.0000}";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
    }

    /// <summary>
    /// Export data to Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      string strTemplateName = "RPT_WOOD_25_001_01";
      string strSheetName = "StockCount";
      string strOutFileName = "Stock Count Location";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtSource = (DataTable)ultData.DataSource;

      oXlsReport.Cell("**Location").Value = this.ultLocation.Text.ToString();
      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];

          if (i > 0)
          {
            oXlsReport.Cell("B9:O9").Copy();
            oXlsReport.RowInsert(8 + i);
            oXlsReport.Cell("B9:O9", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**ScanBarcode", 0, i).Value = row.Cells["IDVeneer"].Value.ToString();
          oXlsReport.Cell("**LotNoId", 0, i).Value = row.Cells["LotNoId"].Value.ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = row.Cells["MaterialCode"].Value.ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = row.Cells["TenEnglish"].Value.ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = row.Cells["TenVietNam"].Value.ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = row.Cells["TenDonViEN"].Value.ToString();
          oXlsReport.Cell("**Qty", 0, i).Value = row.Cells["Qty"].Value.ToString();
          oXlsReport.Cell("**Length", 0, i).Value = row.Cells["Length"].Value.ToString();
          oXlsReport.Cell("**Width", 0, i).Value = row.Cells["Width"].Value.ToString();
          oXlsReport.Cell("**Thickness", 0, i).Value = row.Cells["Thickness"].Value.ToString();
          oXlsReport.Cell("**TotalCBM", 0, i).Value = row.Cells["TotalCBM"].Value.ToString();
          oXlsReport.Cell("**DateInStore", 0, i).Value = row.Cells["DateInStore"].Value.ToString();


          if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 1)
          {
            oXlsReport.Cell("B9:O9", 0, i).Attr.FontColor = xlColor.xcBlue;
          }
          else if (DBConvert.ParseInt(row.Cells["Errors"].Value.ToString()) == 2)
          {
            oXlsReport.Cell("B9:O9", 0, i).Attr.FontColor = xlColor.xcRed;
          }
        }
      }
      else
      {
        return;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }


    /// <summary>
    /// Export Excel FGW
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExportExcelFGW_Click(object sender, EventArgs e)
    {
      // Get path from Folder
      string path = @"\PhanmemDENSOBHT8000";
      path = Path.GetFullPath(path);

      DataTable dtSource = this.CreateDataTable();

      string curFile = path + @"\THONGTIN.txt";

      if (!File.Exists(curFile))
      {
        string message = string.Format(DaiCo.Shared.Utility.FunctionUtility.GetMessage("ERR0011"), "THONGTIN.txt");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string[] a = File.ReadAllLines(curFile);

      if (a.Length == 0)
      {
        return;
      }

      int index = int.MinValue;
      if (a[0].ToString().Length > 0)
      {
        index = a[0].IndexOf("*");
      }

      for (int i = 0; i < a.Length; i++)
      {
        if (a[i].Trim().ToString().Length > 0 && index != -1)
        {
          DataRow row = dtSource.NewRow();
          index = a[i].IndexOf("*");
          a[i] = a[i].Substring(0, index).Trim().ToString();

          DataRow[] foundRow = dtSource.Select("IDVeneer ='" + a[i].ToString().Trim() + "'");
          if (foundRow.Length > 0)
          {
            continue;
          }

          row["IDVeneer"] = a[i].ToString().Trim();
          row["Code"] = "0";
          row["Pcs"] = 0;
          row["Width"] = 0;
          row["Length"] = 0;
          row["Thickness"] = 0;
          row["Location"] = "0";
          dtSource.Rows.Add(row);
        }
        else if (a[i].Trim().ToString().Length > 0)
        {
          DataRow[] foundRow = dtSource.Select("IDVeneer ='" + a[i].ToString().Trim() + "'");
          if (foundRow.Length > 0)
          {
            continue;
          }

          DataRow row = dtSource.NewRow();
          row["IDVeneer"] = a[i].ToString().Trim();
          row["Code"] = "0";
          row["Pcs"] = 0;
          row["Width"] = 0;
          row["Length"] = 0;
          row["Thickness"] = 0;
          row["Location"] = "0";
          dtSource.Rows.Add(row);
        }
      }

      string strTemplateName = "RPT_WOOD_25_001_02";
      string strSheetName = "Sheet1";
      string strOutFileName = "Export Barcode FGW";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      if (dtSource != null && dtSource.Rows.Count > 0)
      {

        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          if (i > 0)
          {
            oXlsReport.Cell("A4:A4").Copy();
            oXlsReport.RowInsert(3 + i);
            oXlsReport.Cell("A4:A4", 0, i).Paste();
          }

          oXlsReport.Cell("**Barcode", 0, i).Value = dtSource.Rows[i]["IDVeneer"].ToString();
        }
      }
      else
      {
        return;
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
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

    #endregion Event

    #region Process
    /// <summary>
    /// Check valid before Load
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      // Master Information
      if (this.ultLocation.Value != null && this.ultLocation.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM VWHDLocationWoods WHERE ID_Position = '" + this.ultLocation.Value.ToString() + "'";
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Location";
            return false;
          }
        }
        else
        {
          message = "Location";
          return false;
        }
      }
      else
      {
        message = "Location";
        return false;
      }
      return true;
    }

    /// <summary>
    /// Get Data When Load Data
    /// </summary>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private DataTable GetDataLoad(DataTable dtSource)
    {
      DataTable dt = new DataTable();
      SqlCommand cm = new SqlCommand("spWHDGetDataStockCountLocationWoods_Select");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      cm.Parameters.Add(para);

      // Lication Pid
      para = cm.CreateParameter();
      para.ParameterName = "@LocationPid";
      para.DbType = DbType.Int64;
      para.Value = DBConvert.ParseLong(this.ultLocation.Value.ToString());

      cm.Parameters.Add(para);

      SqlDataAdapter adp = new SqlDataAdapter();
      adp.SelectCommand = cm;
      DataSet result = new DataSet();
      try
      {
        if (cm.Connection.State != ConnectionState.Open)
        {
          cm.Connection.Open();
        }
        adp.Fill(result);
      }
      catch (Exception ex)
      {
        result = null;
        return null;
      }

      dt = result.Tables[0];
      return dt;
    }

    /// <summary>
    /// Create DataTable Before Load
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();

      dt.Columns.Add("IDVeneer", typeof(System.String));
      dt.Columns.Add("Code", typeof(System.String));
      dt.Columns.Add("Pcs", typeof(System.Double));
      dt.Columns.Add("Width", typeof(System.Double));
      dt.Columns.Add("Length", typeof(System.Double));
      dt.Columns.Add("Thickness", typeof(System.Double));
      dt.Columns.Add("Location", typeof(System.String));

      return dt;
    }
    #endregion Process
  }
}
