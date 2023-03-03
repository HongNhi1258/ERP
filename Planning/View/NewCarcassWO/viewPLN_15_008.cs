/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: ViewPLN_15_008.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class ViewPLN_15_008 : MainUserControl
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      int paramNumber = 1;
      string storeName = "spPLNCarcassWOFollow_Select";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("", DbType.String, "");

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.FilteredInRowCount);
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {

    }
    private DataTable CreateData()
    {

      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("OldCarCassWOPid", typeof(System.Int64));
      taParent.Columns.Add("OldFurnitureCodePid", typeof(System.Int64));
      taParent.Columns.Add("NewCarCassWOPid", typeof(System.Int64));
      taParent.Columns.Add("NewFurnitureCodePid", typeof(System.Int64));

      return taParent;
    }
    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow ur = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(ur.Cells["Error"].Value.ToString()) == 1)
        {
          errorMessage = "OldCarcassWO,OldFurnitureCode";
          return false;
        }
        if (DBConvert.ParseInt(ur.Cells["Error"].Value.ToString()) == 2)
        {
          errorMessage = "New FurnitureCode";
          return false;
        }
        if (DBConvert.ParseInt(ur.Cells["Error"].Value.ToString()) == 3)
        {
          errorMessage = "Furniture Type";
          return false;
        }

      }

      return true;
    }

    private void SaveData()
    {

      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ultraGridInformation.DataSource;
        DataTable dtSource = this.CreateData();
        foreach (DataRow row in dtDetail.Rows)
        {
          DataRow dr = dtSource.NewRow();
          if (DBConvert.ParseLong(row["OldCarcassWOPid"].ToString()) > 0)
          {
            dr["OldCarCassWOPid"] = DBConvert.ParseLong(row["OldCarcassWOPid"].ToString());
            dr["OldFurnitureCodePid"] = DBConvert.ParseLong(row["OldFurniturePid"].ToString());
            dr["NewCarCassWOPid"] = DBConvert.ParseLong(row["NewCarCassWOPid"].ToString());
            dr["NewFurnitureCodePid"] = DBConvert.ParseLong(row["NewFurniturePid"].ToString());
            dtSource.Rows.Add(dr);
          }
        }
        SqlDBParameter[] inputParam = new SqlDBParameter[1];
        inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dtSource);
        SqlDBParameter[] outputParam = new SqlDBParameter[1];
        outputParam[0] = new SqlDBParameter("@Result", SqlDbType.Int, int.MinValue);
        SqlDataBaseAccess.ExecuteStoreProcedure("spPLNCarcassWOSwapFurniture_Insert", inputParam, outputParam);
        if (outputParam[0] == null || DBConvert.ParseInt(outputParam[0].Value.ToString()) <= 0)
        {
          success = false;
        }

        if (success)
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
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }

    }

    private void FillDataForGrid(DataTable dt)
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
      DataTable dtSource = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNSwapCarcassWo_Import", inputParam);
      ultraGridInformation.DataSource = dtSource;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow ur = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(ur.Cells["Error"].Value.ToString()) == 1)
        {
          ur.CellAppearance.BackColor = Color.SkyBlue;
        }
        else if (DBConvert.ParseInt(ur.Cells["Error"].Value.ToString()) == 2)
        {
          ur.CellAppearance.BackColor = Color.Yellow;
        }
        else if (DBConvert.ParseInt(ur.Cells["Error"].Value.ToString()) == 3)
        {
          ur.CellAppearance.BackColor = Color.Pink;
        }
      }
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
    public ViewPLN_15_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewPLN_15_008_Load(object sender, EventArgs e)
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
      e.Layout.Bands[0].Columns["OldCarcassWOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["OldFurniturePid"].Hidden = true;
      e.Layout.Bands[0].Columns["NewCarcassWOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["NewFurniturePid"].Hidden = true;
      e.Layout.Bands[0].Columns["Error"].Hidden = true;

      e.Layout.Bands[0].Columns["OldCarcassWONo"].Header.Caption = "Old Carcass WO";
      e.Layout.Bands[0].Columns["OldFurnitureCode"].Header.Caption = "Old Furniture";
      e.Layout.Bands[0].Columns["NewCarcassWONo"].Header.Caption = "New Carcass WO";
      e.Layout.Bands[0].Columns["NewFurnitureCode"].Header.Caption = "New Furniture";

    }
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "SwapCarcassWOTemp";
      string sheetName = "Allocation";
      string outFileName = "SwapCarcassWOTemp";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }
    private void btnImportExcel_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportPatch.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportPatch.Text.Trim(), "SELECT * FROM [Allocation (1)$E3:E4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0].ToString());
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportPatch.Text.Trim(), string.Format("SELECT * FROM [Allocation (1)$B5:E{0}]", itemCount));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.FillDataForGrid(dsItemList.Tables[0]);
    }



    private void btnGetFile_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportPatch.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    #endregion event





  }
}
