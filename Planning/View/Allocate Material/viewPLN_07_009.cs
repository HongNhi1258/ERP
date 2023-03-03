/*
  Author      : Vo Van Duy Qui
  Date        : 03/03/2011
  Description : Material Report
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using VBReport;
namespace DaiCo.Planning
{
  public partial class viewPLN_07_009 : MainUserControl
  {
    #region Field
    private string pathOutputFile;
    private string pathTemplate;
    #endregion Field

    #region Init

    /// <summary>
    /// Init Form
    /// </summary>
    public viewPLN_07_009()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_07_009_Load(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathOutputFile = string.Format(@"{0}\Report", startupPath);
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.LoadDataForListView();
    }

    #endregion Init

    #region Function

    /// <summary>
    /// Load Data For ListView
    /// </summary>
    private void LoadDataForListView()
    {
      listLeft.Clear();
      listRight.Clear();
      // Load ListView Material Group
      string commandText = "Select [Group], Description From VBOMControlMaterialGroup";
      DataTable dtMaterialGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      // Add columns to ListView
      listLeft.Columns.Add("Material Group", 100, HorizontalAlignment.Center);
      listLeft.Columns.Add("Description", 250, HorizontalAlignment.Left);
      listRight.Columns.Add("Material Group", 100, HorizontalAlignment.Center);
      listRight.Columns.Add("Description", 250, HorizontalAlignment.Left);

      // Add rows to ListView
      foreach (DataRow row in dtMaterialGroup.Rows)
      {
        ListViewItem item = new ListViewItem(row[0].ToString());
        item.SubItems.Add(row[1].ToString());
        listLeft.Items.Add(item);
      }
    }

    /// <summary>
    /// Get Value Of List
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
    /// Search Close Material Information
    /// </summary>
    private DataTable SearchCloseMaterialInfo()
    {
      string wo = txtWO.Text.Trim().Replace(" ", string.Empty);
      string materialGroup = txtMaterial.Text.Trim().Replace(" ", string.Empty);
      DBParameter[] inputParam = new DBParameter[2];
      if (wo.Length > 0)
      {
        wo = string.Format(";{0};", wo).Replace(" ", string.Empty);
        inputParam[0] = new DBParameter("@WO", DbType.AnsiString, 1000, wo);
      }
      if (materialGroup.Length > 0)
      {
        materialGroup = string.Format(";{0};", materialGroup);
        inputParam[1] = new DBParameter("@MaterialGroup", DbType.AnsiString, 1000, materialGroup);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNCloseWoMaterialInformation", inputParam);
      return dtSource;
    }

    /// <summary>
    /// Search Supplement Material Information
    /// </summary>
    private DataTable SearchSuppMaterialInfo()
    {
      string wo = txtWO.Text.Trim().Replace(" ", string.Empty);
      string materialGroup = txtMaterial.Text.Trim().Replace(" ", string.Empty);
      DBParameter[] inputParam = new DBParameter[2];
      if (wo.Length > 0)
      {
        wo = string.Format(";{0};", wo).Replace(" ", string.Empty);
        inputParam[0] = new DBParameter("@WO", DbType.AnsiString, 1000, wo);
      }
      if (materialGroup.Length > 0)
      {
        materialGroup = string.Format(";{0};", materialGroup);
        inputParam[1] = new DBParameter("@MaterialGroup", DbType.AnsiString, 1000, materialGroup);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNSupplementMaterialInformation", inputParam);
      return dtSource;
    }

    /// <summary>
    /// Search Allocate Material Information
    /// </summary>
    private DataTable SearchAllocateMaterialInfo()
    {
      string wo = txtWO.Text.Trim().Replace(" ", string.Empty);
      string materialGroup = txtMaterial.Text.Trim().Replace(" ", string.Empty);
      DBParameter[] inputParam = new DBParameter[2];
      if (wo.Length > 0)
      {
        wo = string.Format(";{0};", wo).Replace(" ", string.Empty);
        inputParam[0] = new DBParameter("@WO", DbType.AnsiString, 1000, wo);
      }
      if (materialGroup.Length > 0)
      {
        materialGroup = string.Format(";{0};", materialGroup);
        inputParam[1] = new DBParameter("@MaterialGroup", DbType.AnsiString, 1000, materialGroup);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNAllocateMaterialInformation", inputParam);
      return dtSource;
    }

    /// <summary>
    /// Check File Is Exist. If Exist Then Delete And Create File Else Create File
    /// </summary>
    private void InitializeOutputdirectory()
    {
      if (Directory.Exists(this.pathOutputFile))
      {
        string[] files = Directory.GetFiles(this.pathOutputFile);
        foreach (string file in files)
        {
          try
          {
            File.Delete(file);
          }
          catch { }
        }
      }
      else
      {
        Directory.CreateDirectory(this.pathOutputFile);
      }
    }

    /// <summary>
    /// Format Export File
    /// </summary>
    /// <param name="strTemplateName"></param>
    /// <param name="strSheetName"></param>
    /// <param name="strPreOutFileName"></param>
    /// <param name="strOutFileName"></param>
    /// <returns></returns>
    private XlsReport InitializeXlsReport(string strTemplateName, string strSheetName, string strPreOutFileName, out string strOutFileName)
    {
      IContainer components = new Container();
      XlsReport oXlsReport = new XlsReport(components);
      this.InitializeOutputdirectory();
      strTemplateName = string.Format(@"{0}\{1}.xls", this.pathTemplate, strTemplateName);
      oXlsReport.FileName = strTemplateName;
      oXlsReport.Start.File();
      oXlsReport.Page.Begin(strSheetName, "1");
      strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", this.pathOutputFile, strPreOutFileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      return oXlsReport;
    }

    /// <summary>
    /// Export Close WO Material To Excel File
    /// </summary>
    private void ExportExcelCloseWOMaterial()
    {
      //Init report
      string strTemplateName = "CloseWoMaterialTemplate";
      string strSheetName = "Sheet1";
      string strOutFileName = "CloseWoMaterial";
      XlsReport oXlsReport = this.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, out strOutFileName);

      //Load data
      DataTable dtData = this.SearchCloseMaterialInfo();
      if (dtData != null || dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B7:K7").Copy();
            oXlsReport.RowInsert(6 + i);
            oXlsReport.Cell("B7:K7", 0, i).Paste();
          }
          oXlsReport.Cell("**ActualConsume", 0, i).Attr.FontColor = xlColor.xcDefault;
          oXlsReport.Cell("**Wo", 0, i).Value = dtRow["WoPid"];
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"];
          oXlsReport.Cell("**MaterialName", 0, i).Value = dtRow["MaterialName"];
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"];
          oXlsReport.Cell("**TotalAllocated", 0, i).Value = dtRow["Total Allocate"];
          oXlsReport.Cell("**ActualConsume", 0, i).Value = dtRow["Actual Consumption"];
          oXlsReport.Cell("**Excess", 0, i).Value = dtRow["Excess (%)"];
          oXlsReport.Cell("**Supplement", 0, i).Value = dtRow["Supplement"];
          oXlsReport.Cell("**Remain", 0, i).Value = dtRow["Remain"];
          oXlsReport.Cell("**CloseDate", 0, i).Value = dtRow["CloseWorkDate"];
          double totalAllocate = DBConvert.ParseDouble(dtRow["Total Allocate"].ToString());
          double actualConsume = DBConvert.ParseDouble(dtRow["Actual Consumption"].ToString());
          if (actualConsume > totalAllocate)
          {
            oXlsReport.Cell("**ActualConsume", 0, i).Attr.FontColor = xlColor.xcRed;
          }
        }
      }
      //Export
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Supplement Material To Excel File
    /// </summary>
    private void ExportExcelSupplementMaterial()
    {
      //Init report
      string strTemplateName = "SupplementMaterialTemplate";
      string strSheetName = "Supplement";
      string strOutFileName = "SupplementMaterial";
      XlsReport oXlsReport = this.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, out strOutFileName);

      //Load data
      DataTable dtData = this.SearchSuppMaterialInfo();
      if (dtData != null || dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B7:L7").Copy();
            oXlsReport.RowInsert(6 + i);
            oXlsReport.Cell("B7:L7", 0, i).Paste();
          }
          oXlsReport.Cell("**Wo", 0, i).Value = dtRow["WoPid"];
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**Revision", 0, i).Value = dtRow["Revision"];
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"];
          oXlsReport.Cell("**MaterialName", 0, i).Value = dtRow["MaterialName"];
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"];
          oXlsReport.Cell("**Reason", 0, i).Value = dtRow["Reason"];
          oXlsReport.Cell("**TotalSupp", 0, i).Value = dtRow["TotalSupplement"];
          oXlsReport.Cell("**Issued", 0, i).Value = dtRow["Issued"];
          oXlsReport.Cell("**Remain", 0, i).Value = dtRow["Remain"];
          oXlsReport.Cell("**CloseDate", 0, i).Value = dtRow["CloseWorkDate"];
        }
      }
      //Export
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Allocate Material Information To Excel
    /// </summary>
    private void ExportExcelAllocateMaterial()
    {
      //Init report
      string strTemplateName = "AllocateMaterialInformationTemplate";
      string strSheetName = "Allocate";
      string strOutFileName = "AllocateMaterialInformation";
      XlsReport oXlsReport = this.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, out strOutFileName);

      //Load data
      DataTable dtData = this.SearchAllocateMaterialInfo();
      if (dtData != null || dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:P8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:P8", 0, i).Paste();
          }
          oXlsReport.Cell("**Wo", 0, i).Value = dtRow["WoPid"];
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**Revision", 0, i).Value = dtRow["Revision"];
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"];
          oXlsReport.Cell("**MaterialName", 0, i).Value = dtRow["MaterialName"];
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"];
          oXlsReport.Cell("**AllocateQty", 0, i).Value = dtRow["AllocatedQty"];
          oXlsReport.Cell("**AllocateIssue", 0, i).Value = dtRow["AllocatedIssued"];
          oXlsReport.Cell("**RegisterQty", 0, i).Value = dtRow["RegisteredQty"];
          oXlsReport.Cell("**RegisterIssue", 0, i).Value = dtRow["RegisteredIssued"];
          oXlsReport.Cell("**TotalRemain", 0, i).Value = dtRow["TotalRemain"];
          oXlsReport.Cell("**Balance", 0, i).Value = dtRow["Balance"];
          oXlsReport.Cell("**FreeQty", 0, i).Value = dtRow["FreeQty"];
          oXlsReport.Cell("**QtyCanIssue", 0, i).Value = dtRow["QtyCanIssue"];
          oXlsReport.Cell("**CloseDate", 0, i).Value = dtRow["CloseWorkDate"];
        }
      }
      //Export
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    #endregion Function

    #region Event

    /// <summary>
    /// btnSearch Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      int index = cmbReport.SelectedIndex;
      DataTable dtSource;
      if (index <= 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Report" });
        cmbReport.Focus();
        return;
      }
      else if (index == 1)
      {
        dtSource = this.SearchCloseMaterialInfo();
        ultData.DataSource = dtSource;
        ultData.DisplayLayout.Bands[0].Columns["Total Allocate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["Total Allocate"].MaxWidth = 120;
        ultData.DisplayLayout.Bands[0].Columns["Total Allocate"].MinWidth = 120;
        ultData.DisplayLayout.Bands[0].Columns["Total Allocate"].Format = "#,##0.0000";

        ultData.DisplayLayout.Bands[0].Columns["Actual Consumption"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["Actual Consumption"].MaxWidth = 120;
        ultData.DisplayLayout.Bands[0].Columns["Actual Consumption"].MinWidth = 120;
        ultData.DisplayLayout.Bands[0].Columns["Actual Consumption"].Format = "#,##0.0000";

        ultData.DisplayLayout.Bands[0].Columns["Excess (%)"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["Excess (%)"].MaxWidth = 70;
        ultData.DisplayLayout.Bands[0].Columns["Excess (%)"].MinWidth = 70;
        ultData.DisplayLayout.Bands[0].Columns["Excess (%)"].Format = "#,##0.00";

        ultData.DisplayLayout.Bands[0].Columns["Supplement"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["Supplement"].MaxWidth = 70;
        ultData.DisplayLayout.Bands[0].Columns["Supplement"].MinWidth = 70;
        ultData.DisplayLayout.Bands[0].Columns["Supplement"].Format = "#,##0.0000";

        ultData.DisplayLayout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["Remain"].MaxWidth = 70;
        ultData.DisplayLayout.Bands[0].Columns["Remain"].MinWidth = 70;
        ultData.DisplayLayout.Bands[0].Columns["Remain"].Format = "#,##0.0000";
      }
      if (index == 2)
      {
        dtSource = this.SearchSuppMaterialInfo();
        ultData.DataSource = dtSource;
        ultData.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
        ultData.DisplayLayout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["TotalSupplement"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["Issued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["TotalSupplement"].Header.Caption = "Total Supplement";

        ultData.DisplayLayout.Bands[0].Columns["MaterialName"].MaxWidth = 350;
        ultData.DisplayLayout.Bands[0].Columns["MaterialName"].MinWidth = 350;
      }
      if (index == 3)
      {
        dtSource = this.SearchAllocateMaterialInfo();
        ultData.DataSource = dtSource;
        ultData.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
        ultData.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
        ultData.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 80;
        ultData.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 50;
        ultData.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 50;
        ultData.DisplayLayout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["AllocatedQty"].Header.Caption = "Alc. Qty";
        ultData.DisplayLayout.Bands[0].Columns["AllocatedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["AllocatedIssued"].Header.Caption = "Alc. Issued";
        ultData.DisplayLayout.Bands[0].Columns["AllocatedIssued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["RegisteredQty"].Header.Caption = "Reg. Qty";
        ultData.DisplayLayout.Bands[0].Columns["RegisteredQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["RegisteredIssued"].Header.Caption = "Reg. Issued";
        ultData.DisplayLayout.Bands[0].Columns["RegisteredIssued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["TotalRemain"].MaxWidth = 80;
        ultData.DisplayLayout.Bands[0].Columns["TotalRemain"].MinWidth = 80;
        ultData.DisplayLayout.Bands[0].Columns["TotalRemain"].Header.Caption = "Total Remain";
        ultData.DisplayLayout.Bands[0].Columns["TotalRemain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["FreeQty"].MaxWidth = 80;
        ultData.DisplayLayout.Bands[0].Columns["FreeQty"].MinWidth = 80;
        ultData.DisplayLayout.Bands[0].Columns["FreeQty"].Header.Caption = "Non Allocate";
        ultData.DisplayLayout.Bands[0].Columns["FreeQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["QtyCanIssue"].Header.Caption = "Qty Can Issue";
        ultData.DisplayLayout.Bands[0].Columns["QtyCanIssue"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultData.DisplayLayout.Bands[0].Columns["QtyCanIssue"].MaxWidth = 80;
        ultData.DisplayLayout.Bands[0].Columns["QtyCanIssue"].MinWidth = 80;
        ultData.DisplayLayout.Bands[0].Columns["MaterialName"].MaxWidth = 200;
        ultData.DisplayLayout.Bands[0].Columns["MaterialName"].MinWidth = 200;
      }
    }

    /// <summary>
    /// btnExport Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      int index = cmbReport.SelectedIndex;
      if (index <= 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Report" });
        cmbReport.Focus();
        return;
      }
      else if (index == 1)
      {
        this.ExportExcelCloseWOMaterial();
      }
      if (index == 2)
      {
        this.ExportExcelSupplementMaterial();
      }
      if (index == 3)
      {
        this.ExportExcelAllocateMaterial();
      }
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// CheckBox Show Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShow_CheckedChanged(object sender, EventArgs e)
    {
      tableLayoutMaterialGroup.Visible = chkShow.Checked;
    }

    /// <summary>
    /// List Left Column Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void listLeft_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      if (listLeft.Sorting == SortOrder.Descending)
      {
        listLeft.Sorting = SortOrder.Ascending;
      }
      else
      {
        listLeft.Sorting = SortOrder.Descending;
      }
    }

    /// <summary>
    /// List Right Column Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void listRight_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      if (listRight.Sorting == SortOrder.Descending)
      {
        listRight.Sorting = SortOrder.Ascending;
      }
      else
      {
        listRight.Sorting = SortOrder.Descending;
      }
    }

    /// <summary>
    /// btnAdd Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem item in listLeft.SelectedItems)
      {
        listRight.Items.Add((ListViewItem)item.Clone());
        listLeft.Items.Remove(item);
      }
      txtMaterial.Text = GetSelectedListView(listRight);
    }

    /// <summary>
    /// btnAddAll Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddAll_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem item in listLeft.Items)
      {
        listRight.Items.Add((ListViewItem)item.Clone());
        listLeft.Items.Remove(item);
      }
      txtMaterial.Text = GetSelectedListView(listRight);
    }

    /// <summary>
    /// btnRemove Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRemove_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem item in listRight.SelectedItems)
      {
        listLeft.Items.Add((ListViewItem)item.Clone());
        listRight.Items.Remove(item);
      }
      txtMaterial.Text = GetSelectedListView(listRight);
    }

    /// <summary>
    /// btnRemoveAll Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRemoveAll_Click(object sender, EventArgs e)
    {
      foreach (ListViewItem item in listRight.Items)
      {
        listLeft.Items.Add((ListViewItem)item.Clone());
        listRight.Items.Remove(item);
      }
      txtMaterial.Text = string.Empty;
    }

    /// <summary>
    /// ComboBox Report Selected Index Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbReport_SelectedIndexChanged(object sender, EventArgs e)
    {
      txtWO.Text = string.Empty;
      txtMaterial.Text = string.Empty;
      this.LoadDataForListView();
      ultData.DataSource = null;
      this.chkShow.Checked = false;
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["WoPid"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["WoPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WoPid"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["WoPid"].MinWidth = 40;
      e.Layout.Bands[0].Columns["CloseWorkDate"].Header.Caption = "Close Date";
      e.Layout.Bands[0].Columns["CloseWorkDate"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["CloseWorkDate"].MinWidth = 80;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 45;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 45;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
    }
    #endregion Event
  }
}