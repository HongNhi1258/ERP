/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewCSD_03_015.cs
*/
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Text;
//using System.Windows.Forms;
//using DaiCo.Shared;
//using DaiCo.Shared.DataBaseUtility;
//using DaiCo.Application;
//using DaiCo.Shared.Utility;
//using DaiCo.Objects;
//using System.Diagnostics;
//using CrystalDecisions.CrystalReports.Engine;
//using VBReport;
//using Infragistics.Win.UltraWinGrid;
//using System.Collections;

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using System.Diagnostics;
using CrystalDecisions.CrystalReports.Engine;
using VBReport;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_015 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    public string SaleCode = "";
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
      this.LoadDDReason();
      this.SearchData();      
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      int paramNumber = 1;
      string storeName = "spCSDSaleCodeCDDEMO_Search";
      DBParameter[] inputParam = new DBParameter[paramNumber];
      if(this.SaleCode.Length >0 && txtSearchCode.Text.Length == 0)
      {
        txtSearchCode.Text = this.SaleCode;
      }
      if (txtSearchCode.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@SaleCode", DbType.String, 32, txtSearchCode.Text);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridInformation.DataSource = dtSource;

      this.LoadultSection();
      //this.CheckSection();

      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
    }
    private void LoadDDReason()
    {
      string cm = string.Format("SELECT Code,Value FROM TblBOMCodeMaster WHERE [Group]= 16030");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraDropDown(ultDDReason, dt, "Code", "Value","Code");
      ultDDReason.DisplayLayout.AutoFitColumns = true;
      ultDDReason.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    private void CheckAll(string comlum,int value) {


      for (int i = 0; i < ultraGridInformation.Rows.Count;i++ )
      {
        UltraGridRow dr = ultraGridInformation.Rows[i];
        dr.Cells[comlum].Value = (value == 1) ? true : false;
      }

    
    }
    private void CheckSection() 
    {

      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow dr = ultraGridInformation.Rows[i];
        dr.Cells["1"].Value = SearchCheckAll("1");
        dr.Cells["2"].Value = SearchCheckAll("2");
        dr.Cells["3"].Value = SearchCheckAll("3");
        dr.Cells["4"].Value = SearchCheckAll("4");        
      }
    
    }
    private bool SearchCheckAll(string collum) 
    {
      DataTable dt = (DataTable) ultraGridInformation.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++) {
        DataRow dr = dt.Rows[i];
        if (DBConvert.ParseInt(dr[collum].ToString()) == 0) {
          return false;
        }        
      }
      return true;
    
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      this.SaleCode = "";
      txtSearchCode.Text = "";
      txtSearchCode.Focus();
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

    private void ExportExcel()
    {
      if (ultraGridInformation.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ultraGridInformation.Rows.Band.Columns["RowState"].Hidden = true;
        ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, out xlBook, "APPROVED STATUS OVERVIEW", 6);
        ultraGridInformation.Rows.Band.Columns["RowState"].Hidden = true;

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "APPROVED STATUS OVERVIEW";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
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

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //if (ultCBWo.Text.Length == 0)
      //{
      //  errorMessage = "Work Order";      
      //  return false;
      //}
      return true;
    }
    private void LoadultSection()
    {
      string cm = "select 0 [1],0 [2],0 [3],0 [4]";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ultCheckSections.DataSource = dt;

    }
    private bool SaveDetail() {      
      SqlDBParameter[] input = new SqlDBParameter[2];
      DataTable dt = (DataTable) ultraGridInformation.DataSource;    
      if (dt.Rows.Count > 0 && dt != null)
      {
        input[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
      }
      input[1] = new SqlDBParameter("@CreateBy", SqlDbType.Int, Shared.Utility.SharedObject.UserInfo.UserPid);
      SqlDBParameter[] output = new SqlDBParameter[1];
      output[0] = new SqlDBParameter("@result", SqlDbType.Int, 0);
      Shared.DataBaseUtility.SqlDataBaseAccess.ExecuteStoreProcedure("spCSDSaleCodeCD_Edit", input, output);
      if (DBConvert.ParseLong(output[0].Value.ToString()) == 0 || output[0] == null)
      {
        return false;
      }
      this.SearchData();
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        if (this.SaveDetail())
        {  
            WindowUtinity.ShowMessageSuccess("MSG0004");
          
        }else
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

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region event
    public viewCSD_03_015()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_015_Load(object sender, EventArgs e)
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
      this.ConfirmToCloseTab();
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
      //e.Layout.AutoFitColumns = true;
      //e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      //e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      //e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      //e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      //e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;


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
      e.Layout.Bands[0].Columns["ReasonPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

     

      e.Layout.Bands[0].Columns["1"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["2"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["3"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["4"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["1"].Header.Caption = "Approved Drawing Received";
      e.Layout.Bands[0].Columns["2"].Header.Caption = "Sample Requested";
      e.Layout.Bands[0].Columns["3"].Header.Caption = "Sample Sent To Customer";
      e.Layout.Bands[0].Columns["4"].Header.Caption = "Sample Approved by Customer";

  
      e.Layout.Bands[0].Columns["11"].Header.Caption = "Comment Approved Drawing Received";      
      e.Layout.Bands[0].Columns["12"].Header.Caption = "Comment Sample Requested";     
      e.Layout.Bands[0].Columns["13"].Header.Caption = "Comment Sample Sent To Customer";     
      e.Layout.Bands[0].Columns["14"].Header.Caption = "Comment Sample Approved by Customer";     

      e.Layout.Bands[0].Columns["21"].Header.Caption = "Date Update";
      e.Layout.Bands[0].Columns["22"].Header.Caption = "Date Update";
      e.Layout.Bands[0].Columns["23"].Header.Caption = "Date Update";
      e.Layout.Bands[0].Columns["24"].Header.Caption = "Date Update";

      e.Layout.Bands[0].Columns["1"].MinWidth = 100;
      e.Layout.Bands[0].Columns["2"].MinWidth = 100;
      e.Layout.Bands[0].Columns["3"].MinWidth = 100;
      e.Layout.Bands[0].Columns["4"].MinWidth = 100;

      e.Layout.Bands[0].Columns["11"].MinWidth = 150;
      e.Layout.Bands[0].Columns["12"].MinWidth = 150;
      e.Layout.Bands[0].Columns["13"].MinWidth = 150;
      e.Layout.Bands[0].Columns["14"].MinWidth = 150;

      e.Layout.Bands[0].Columns["21"].MinWidth = 150;
      e.Layout.Bands[0].Columns["22"].MinWidth = 150;
      e.Layout.Bands[0].Columns["23"].MinWidth = 150;
      e.Layout.Bands[0].Columns["24"].MinWidth = 150;



      e.Layout.Bands[0].Columns["ReasonPid"].Header.Caption = "Reason";
      e.Layout.Bands[0].Columns["ReasonPid"].ValueList = ultDDReason;
      e.Layout.Bands[0].Columns["RowState"].Hidden = true;

      e.Layout.Bands[0].Columns["SaleCode"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["1"].CellAppearance.BackColor = Color.SkyBlue;
      e.Layout.Bands[0].Columns["2"].CellAppearance.BackColor = Color.SkyBlue;
      e.Layout.Bands[0].Columns["3"].CellAppearance.BackColor = Color.SkyBlue;
      e.Layout.Bands[0].Columns["4"].CellAppearance.BackColor = Color.SkyBlue;

      e.Layout.Bands[0].Columns["11"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["12"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["13"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["14"].CellAppearance.BackColor = Color.Yellow;

      e.Layout.Bands[0].Columns["21"].CellAppearance.BackColor = Color.Pink;
      e.Layout.Bands[0].Columns["22"].CellAppearance.BackColor = Color.Pink;
      e.Layout.Bands[0].Columns["23"].CellAppearance.BackColor = Color.Pink;
      e.Layout.Bands[0].Columns["24"].CellAppearance.BackColor = Color.Pink;

      e.Layout.Bands[0].Columns["ReasonPid"].CellAppearance.BackColor = Color.SkyBlue; 

      e.Layout.Bands[0].Columns["SaleCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      /*
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      */
    }
    private void ultCheckSections_InitializeLayout(object sender, InitializeLayoutEventArgs e)
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
      e.Layout.Bands[0].Columns["1"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["2"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["3"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["4"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["1"].Header.Caption = "Check All Approved Drawing Received";
      e.Layout.Bands[0].Columns["2"].Header.Caption = "Check All Sample Requested";
      e.Layout.Bands[0].Columns["3"].Header.Caption = "Check All Sample Sent To Customer";
      e.Layout.Bands[0].Columns["4"].Header.Caption = "Check All Sample Approved by Customer";

    }
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
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
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {

      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "ReasonPid":
          if (value.Length > 0)
          {
            string cm = string.Format("select COUNT(*) FROM TblBOMCodeMaster where [Group]=16030 And Code ={0}", value);
            DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(cm);
            if (dtCheck != null && dtCheck.Rows.Count > 0)
            {
              if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
              {
                WindowUtinity.ShowMessageError("ERR0001", colName);
                e.Cancel = true;
              }
            }
            else
            {
              WindowUtinity.ShowMessageError("ERR0001", colName);
              e.Cancel = true;
            }
          }
          break;
      }
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      string colName = e.Cell.Column.ToString();
      string value = e.Cell.Value.ToString();
      switch (colName)
      {
        case "ReasonPid":
          if (value.Length > 0)
          {
            if (DBConvert.ParseInt(value) == 3)
              {
                e.Cell.Row.Cells["4"].Value = true;
              }            
          }
          break;
      }
      e.Cell.Row.Cells["RowState"].Value = 1;
    }
    private void ultCheckSections_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "1":
          this.CheckAll(colName, DBConvert.ParseInt(value));
          break;
        case "2":
          this.CheckAll(colName, DBConvert.ParseInt(value));
          break;
        case "3":
          this.CheckAll(colName, DBConvert.ParseInt(value));
          break;
        case "4":
          this.CheckAll(colName, DBConvert.ParseInt(value));
          break;
      }
    }
    private void btnExpExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }
    #endregion event

    

    

   

    

    
  }
}
