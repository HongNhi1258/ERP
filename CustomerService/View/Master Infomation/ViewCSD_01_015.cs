/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: ViewCSD_01_015.cs
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using System.Diagnostics;
using VBReport;
using System.IO;

namespace DaiCo.CustomerService
{
  public partial class ViewCSD_01_015 : MainUserControl
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

    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    private void LoadData()
    {
    }
    private void Search()
    {
      string storeName = "spCSDItemCodeRemark";
      DBParameter[] inputParam = new DBParameter[1];
      if (txtItemCode.Text.ToString().Length > 0)
      {
        inputParam[0] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text.ToString());
      }
      this.ultraGridInformation.DataSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      lbCount.Text = string.Format("Count: {0}", (ultraGridInformation.Rows.FilteredInRowCount));
    }
    private DataTable LoadGird()
    {
      DataTable dt = (DataTable) ultraGridInformation.DataSource;
      DataTable dtSource = this.Createdata();
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dr = dtSource.NewRow();

        if (dt.Rows[i].RowState == DataRowState.Modified)
        {          
          if (dt.Rows[i]["ItemCode"].ToString().Length > 0)
          {
            dr["ItemCode"] = dt.Rows[i]["ItemCode"].ToString();
          }
          if (dt.Rows[i]["Remark"].ToString().Length > 0)
          {
            dr["Remark"] = dt.Rows[i]["Remark"].ToString();
          }
          dtSource.Rows.Add(dr);
        }
      }
      return dtSource;
    }

    private bool SaveHistory(DataTable dt)
    {
      bool result = true;
      string storeName = "spCSDItemCodeRemark_Insert";
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
      SqlDBParameter[] outputParam = new SqlDBParameter[1];
      outputParam[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);
      SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if(outputParam[0].Value == null || DBConvert.ParseInt(outputParam[0].Value.ToString()) < 0)
      {
        result = false;
      }
      return result;
    }

    private void SaveData()
    {
      DataTable dt = this.LoadGird();
      if (this.CheckValid(dt))
      {
        bool success = true;
        success = this.SaveHistory(dt);
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        //this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001","Data");
        this.SaveSuccess = false;
      }
    }

    private DataTable Createdata()
    {      
      DataTable taParent = new DataTable("TblMaster");
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      return taParent;
    }

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

    private void FillDataForGrid(DataTable dtItemList)
    {
      DataTable dt = this.Createdata();
      for (int i = 0; i < dtItemList.Rows.Count; i++)
        {
          if (dtItemList.Rows[i]["ItemCode"].ToString().Length > 0)
          {
            DataRow dr = dt.NewRow();
            dr["ItemCode"] = dtItemList.Rows[i][0].ToString();
            dr["Remark"] = dtItemList.Rows[i][1].ToString();
            dt.Rows.Add(dr);
          }
        }
        if (CheckValid(dt))
        {
          if (this.SaveHistory(dt))
          {
            WindowUtinity.ShowMessageSuccess("MSG0004");
          }
          else
          {
            WindowUtinity.ShowMessageError("WRN0004");
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", "Data");
          this.SaveSuccess = false;
        }
         
    }
    private bool CheckValid(DataTable dt)
    {
      bool result = true;
      string storeName = "spCSDItemCodeRemark_Check";
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
      DataTable dtSource = SqlDataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridInformation.DataSource = dtSource;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Error"].Value.ToString()) > 0)
        {
          ultraGridInformation.Rows[i].Appearance.BackColor = Color.Yellow;
          result = false;
        }
      }
      return result;
    }

    #endregion function

    #region event
    public ViewCSD_01_015()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewCSD_01_015_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(gbSearch);

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
      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;


      e.Layout.Bands[0].Columns["Status"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Name EN";
     
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      
    }

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty; 
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [List (1)$D3:D4]");
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
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [List (1)$B5:C{0}]", itemCount));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.FillDataForGrid(dsItemList.Tables[0]);
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "ItemCodeRemark_Temp";
      string sheetName = "List";
      string outFileName = "ItemCodeRemark_Temp";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      
      this.Search();
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      if (ultraGridInformation.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        UltraGrid AA = ultraGridInformation;

        AA.Rows.Band.Columns["ItemCode"].Header.Caption = "Item Code";
        AA.Rows.Band.Columns["Name"].Header.Caption = "Name EN";  

        AA.Rows.Band.Columns["Status"].Hidden = true;

        ControlUtility.ExportToExcelWithDefaultPath(AA, out xlBook, "ItemCode Remark", 7);
        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Vietnam Furniture Resources Company limited (VFR Co.,ltd)";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan ward, Thuan An town, Binh Duong province";

        xlSheet.Cells[3, 1] = "ItemCode Remark";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        xlSheet.Cells[5, 1] = "Note: ";
        r.Font.Bold = true;
        xlBook.Application.DisplayAlerts = false;
        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }
    #endregion event
  }
}
