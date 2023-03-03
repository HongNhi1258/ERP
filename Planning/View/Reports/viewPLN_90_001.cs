/*
  Author      : Nguyen Thanh Thinh
  Date        : 12/08/2014
  Description : Print 
  Standard Form: viewPLN_90_001.cs
*/
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.Planning.DataSetFile;
using DaiCo.Planning.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_90_001 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
    }

    private void ImportExcel()
    {
      // Check invalid file
      if (!File.Exists(txtFilePath.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), "SELECT * FROM [List (1)$C3:C4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      // Get data for items list

      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), string.Format("SELECT * FROM [List (1)$B5:C{0}]", 5 + itemCount));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.FillDataForGrid(dsItemList.Tables[0]);
    }

    private void LoadData()
    {
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      return true;
    }


    private DataTable CreateTempo()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      return taParent;
    }
    private dsPLNMinimumCutting GetSelectedData(DataTable dt)
    {
      dsPLNMinimumCutting dsResult = new dsPLNMinimumCutting();
      try
      {
        SqlDBParameter[] inputParam = new SqlDBParameter[1];
        inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
        DataTable dataSource = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNMinimumCuttingCalculator_Select", inputParam);
        if (dataSource != null)
        {

          foreach (DataRow rowParent in dataSource.Rows)
          {
            DataRow rowResultParent = dsResult.Tables["tblMinimumCutting"].NewRow();
            rowResultParent["FirstFGWDate"] = rowParent["DateInStore"];
            rowResultParent["SalesCode"] = rowParent["SaleCode"];
            rowResultParent["ItemCode"] = rowParent["ItemCode"];
            rowResultParent["Revision"] = rowParent["Revision"];
            rowResultParent["StandardCost"] = rowParent["DCDefault"];
            rowResultParent["DetailCategory"] = rowParent["DetailCategory"];
            rowResultParent["Qty6m"] = rowParent["Qty6m"];
            rowResultParent["Qty12m"] = rowParent["Qty7-12m"];
            rowResultParent["Qty18m"] = rowParent["Qty13-18m"];
            //Image Item
            string fileItemCodePath = FunctionUtility.BOMGetItemImage(rowParent["ItemCode"].ToString(), DBConvert.ParseInt(rowParent["Revision"].ToString()));
            rowResultParent["Photo"] = FunctionUtility.ImageToByteArrayWithFormat(fileItemCodePath, 969, 0.94, "JPG");
            rowResultParent["ItemKinds"] = FunctionUtility.GetLocalItemKindIcon(DBConvert.ParseInt(rowParent["USKind"].ToString()), 12);
            dsResult.Tables["tblMinimumCutting"].Rows.Add(rowResultParent);
          }
        }
      }
      catch { }
      return dsResult;
    }
    private void FillDataForGrid(DataTable dtItemList)
    {
      DataTable dt = this.CreateTempo();
      for (int i = 0; i < dtItemList.Rows.Count; i++)
      {
        if (dtItemList.Rows[i]["Itemcode"].ToString().Length > 0)
        {
          DataRow dr = dt.NewRow();
          dr["ItemCode"] = dtItemList.Rows[i]["Itemcode"].ToString();
          if (DBConvert.ParseInt(dtItemList.Rows[i]["Revision"].ToString()) != int.MinValue)
          {
            dr["Revision"] = DBConvert.ParseInt(dtItemList.Rows[i]["Revision"].ToString());
          }
          dt.Rows.Add(dr);
        }
      }
      DaiCo.Shared.View_Report report = null;
      ReportClass cpt = null;
      dsPLNMinimumCutting dsMinimumCutting = this.GetSelectedData(dt);
      if (dsMinimumCutting != null)
      {
        cpt = new cptPLNMinimumCutting();
        cpt.SetDataSource(dsMinimumCutting);
        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = true;
        report.ShowReport(Shared.Utility.ViewState.Window, true, FormWindowState.Maximized);
      }
      else
      {
        WindowUtinity.ShowMessageWarning("WRN0024", "Carcass");
      }

    }
    #endregion function

    #region event
    public viewPLN_90_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_90_001_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

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
    }

    private void btnSave_Click(object sender, EventArgs e)
    {

    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {

        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }


    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {

    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtFilePath.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "MinimumCutting_Temp";
      string sheetName = "List";
      string outFileName = "MinimumCutting_Temp";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.ImportExcel();
    }

    #endregion event
  }
}
