/*
  Author      : Nguyễn Thanh Thịnh
  Date        : 23/07/2014
  Description : Search And Update MOQ 
  Standard Form: viewCSD_03_016.cs
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
  public partial class viewCSD_03_016 : MainUserControl
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
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
      this.getCBCustomer();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      int paramNumber = 3;
      string storeName = "spCSDItemInfo_searchMOQ";

      DBParameter[] inputParam = new DBParameter[paramNumber];

      if(txtItemCode.Text.Length >0)
      {
        inputParam[0] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text);
      }
      if (txtItemName.Text.Length > 0)
      {
        inputParam[1] = new DBParameter("@ItemName", DbType.String, txtItemName.Text);
      }
      if (ultCBCustomer.Text.Length > 0 && ultCBCustomer.Value != null)
      {
        inputParam[2] = new DBParameter("@CustomerPid", DbType.Int64, ultCBCustomer.Value.ToString());
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      this.txtItemCode.Text = "";
      this.txtItemName.Text = "";
      this.ultCBCustomer.Focus();
    }
    private DataTable CheckDupDT(DataTable dtSource)
    {
      DataTable dtItemSource = new DataTable("datatable1");
      dtItemSource.Columns.Add("ItemCode", typeof(System.String));
      dtItemSource.Columns.Add("MOQ", typeof(System.String));
      foreach (DataRow row in dtSource.Rows)
      {
        string itemCode = row[0].ToString().Trim();
        string textMOQ =  row[1].ToString().Trim();
        DataRow rowItemSource = dtItemSource.NewRow();
        if (itemCode.Length > 0)
        {
          rowItemSource["ItemCode"] = itemCode;
          DataRow[] rows = dtSource.Select(string.Format("ItemCode = '{0}'", itemCode));
          if (rows.Length > 1)
          {
            DataRow[] rowsDup = dtItemSource.Select(string.Format("ItemCode = '{0}'", itemCode));
            int CheckMOQ;
            if (Int32.TryParse(textMOQ,out CheckMOQ))
            {
              if (rowsDup.Length == 0)
              {
                rowItemSource["ItemCode"] = rows[0]["ItemCode"];
                int MOQ = 0;
                for (int j = 0; j < rows.Length; j++)
                {
                  if (Int32.TryParse(rows[j]["MOQ"].ToString(), out CheckMOQ))
                  {
                    MOQ = MOQ + CheckMOQ;
                  }
                }
                rowItemSource["MOQ"] = MOQ;
              }
              else
              {
                continue;
              }
            }
            else
            {
              continue;
            }
          }
          else 
          {
            rowItemSource["ItemCode"] = itemCode;
            rowItemSource["MOQ"] = textMOQ;
          }
          dtItemSource.Rows.Add(rowItemSource);
        }
      }
      return dtItemSource;
    }
    
    private DataTable CreateTable()
    {
      DataTable dtSource = new DataTable("datatable1");
      dtSource.Columns.Add("ItemCode", typeof(System.String));
      dtSource.Columns.Add("ItemNameEN", typeof(System.String));
      dtSource.Columns.Add("ItemNameVN", typeof(System.String));
      dtSource.Columns.Add("OldMOQ", typeof(System.Int32));
      dtSource.Columns.Add("NewMOQ", typeof(System.String));
      dtSource.Columns.Add("Error", typeof(System.Int32));
      dtSource.Columns.Add("RowState", typeof(System.Int32));      
      return dtSource;
    }
    /// <summary>
    /// Fill data Excel To Gird
    /// </summary>
    private void FillDataForGrid(DataTable dtExcel)
    {
        this.NeedToSave = true;
        btnImportExcel.Enabled = false;
        DataTable dtSource = this.CheckDupDT(dtExcel);
        DataTable dtItemSource = this.CreateTable();
        string storeName = "spCSDItemInfo_searchMOQ";
        DBParameter[] inputParam = new DBParameter[3];
        DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
        foreach (DataRow row in dtSource.Rows)
        {
          string itemCode = row[0].ToString().Trim();
          DataRow rowItemSource = dtItemSource.NewRow();
          if (itemCode.Length > 0)
          {
            rowItemSource["ItemCode"] = itemCode;
            DataRow[] rows = dt.Select(string.Format("ItemCode = '{0}'", itemCode));

            if (rows.Length == 0)
            {
              rowItemSource["ItemNameEN"] = "";
              rowItemSource["ItemNameVN"] = "";
              rowItemSource["OldMOQ"] = DBNull.Value;
              rowItemSource["NewMOQ"] = row[1].ToString().Trim();
              rowItemSource["Error"] = 1;
              rowItemSource["RowState"] = 1;
            }
            else
            {
              if (DBConvert.ParseInt(row[1].ToString().Trim()) >= 0)
              {
                rowItemSource["ItemNameEN"] = rows[0]["ItemNameEN"];
                rowItemSource["ItemNameVN"] = rows[0]["ItemNameVN"];
                rowItemSource["OldMOQ"] = rows[0]["OldMOQ"];
                rowItemSource["NewMOQ"] = DBConvert.ParseInt(row[1].ToString().Trim());
                rowItemSource["Error"] = rows[0]["Error"];
                rowItemSource["RowState"] = 1;
              }
              else
              {
                rowItemSource["ItemNameEN"] = rows[0]["ItemNameEN"];
                rowItemSource["ItemNameVN"] = rows[0]["ItemNameVN"];
                rowItemSource["OldMOQ"] = rows[0]["OldMOQ"];
                rowItemSource["NewMOQ"] = row[1].ToString().Trim();
                rowItemSource["Error"] = 2;
                rowItemSource["RowState"] = 1;
              }
            }
            dtItemSource.Rows.Add(rowItemSource);
          }

        }
        ultraGridInformation.DataSource = dtItemSource;
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          UltraGridRow dr = ultraGridInformation.Rows[i];
          if (DBConvert.ParseInt(dr.Cells["Error"].Value.ToString()) > 0)
          {
            dr.CellAppearance.BackColor = Color.Yellow;
          }
          else
          {
            dr.CellAppearance.BackColor = Color.White;
          }
        }
        lbCount.Text = string.Format("Count: {0}", (dtItemSource != null ? dtItemSource.Rows.Count : 0));

        btnImportExcel.Enabled = true;
    }
    private void FillToGirdAfterSave(DataTable dtSource) 
    { 
      DataTable dtItemSource = this.CreateTable();
      string storeName = "spCSDItemInfo_searchMOQ";
      DBParameter[] inputParam = new DBParameter[3];
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      foreach (DataRow row in dtSource.Rows)
      {
        string itemCode = row[0].ToString().Trim();
        DataRow rowItemSource = dtItemSource.NewRow();
        if (itemCode.Length > 0)
        {
          rowItemSource["ItemCode"] = itemCode;
          DataRow[] rows = dt.Select(string.Format("ItemCode = '{0}'", itemCode));

          if (rows.Length == 0)
          {
            rowItemSource["ItemNameEN"] = "";
            rowItemSource["ItemNameVN"] = "";
            rowItemSource["OldMOQ"] = DBNull.Value;
            rowItemSource["NewMOQ"] = DBNull.Value;
            rowItemSource["Error"] = 1;
            rowItemSource["RowState"] = 1;
          }
          else 
          {
            rowItemSource["ItemNameEN"] = rows[0]["ItemNameEN"];
            rowItemSource["ItemNameVN"] = rows[0]["ItemNameVN"];
            rowItemSource["OldMOQ"] = rows[0]["OldMOQ"];
            rowItemSource["NewMOQ"] = DBNull.Value;
            rowItemSource["Error"] = rows[0]["Error"];
            rowItemSource["RowState"] = 0;
          }
          dtItemSource.Rows.Add(rowItemSource);
        }
      }
      ultraGridInformation.DataSource = dtItemSource;
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
    private void getCBCustomer()
    {
      string cm = @"SELECT Pid,CustomerCode,CustomerCode +' - '+ Name Value
                    FROM TblCSDCustomerInfo
                    ORDER BY CustomerCode";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultCBCustomer, dtSource, "Pid", "Value", false, "Pid");    }

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
      DataTable dtSource = (DataTable) ultraGridInformation.DataSource;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
          DataRow dr = dtSource.Rows[i];
        
        if(DBConvert.ParseInt(dr["RowState"].ToString()) == 1)
        { 
            if(DBConvert.ParseInt(dr["Error"].ToString()) > 0)
            {
                    errorMessage = "ItemCode";
                    return false;
            }
          if (DBConvert.ParseInt(dr["NewMOQ"].ToString()) < 0)
                 {
                    errorMessage = "NewMOQ";
                    return false;
                 }
        }
      }
     
      return true;
    }

    private bool saveDetail()
    {
      bool success = true;
      string storeName = "spCSDItemInfo_UpdateMOQ";

      DataTable dtDetail = (DataTable) ultraGridInformation.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (DBConvert.ParseInt(row["RowState"].ToString()) == 1)
        {          
            DBParameter[] inputParam = new DBParameter[3];
            if (row["NewMOQ"].ToString().Trim().Length == 0)
            {
              continue;
            }
            else
            {
              inputParam[0] = new DBParameter("@MOQ", DbType.Int32, row["NewMOQ"].ToString().Trim());
            }
            inputParam[1] = new DBParameter("@ItemCode", DbType.String, row["ItemCode"].ToString().Trim());
            inputParam[2] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }          
        }       
      }

      return success;
    }
    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;

        success = this.saveDetail();
        
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        DataTable dtSource = (DataTable) ultraGridInformation.DataSource;        
        this.FillToGirdAfterSave(dtSource);
        this.SaveSuccess = true;
        this.NeedToSave = false;
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
    public viewCSD_03_016()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_016_Load(object sender, EventArgs e)
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
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["NewMOQ"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["RowState"].Hidden = true;

      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemNameEN"].Header.Caption = "Item Name EN";
      e.Layout.Bands[0].Columns["ItemNameVN"].Header.Caption = "Item Name VN";
      e.Layout.Bands[0].Columns["OldMOQ"].Header.Caption = "Old MOQ";
      e.Layout.Bands[0].Columns["NewMOQ"].Header.Caption = "New MOQ";

  
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
        case "NewMOQ":
          if (DBConvert.ParseInt(value) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "NewMOQ");
            e.Cancel = true;
          }
          break;
        default:
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
        case "NewMOQ":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Error"].Value) == 2)
          {
            e.Cell.Row.Cells["RowState"].Value = 1;
            e.Cell.Row.CellAppearance.BackColor = Color.White;
            e.Cell.Row.Cells["Error"].Value = 0;
          }
          break;
        default:
          break;
      }

    }
    

    private void btnImportExcel_Click(object sender, EventArgs e)
    {
      // Check invalid file
      
      if (!System.IO.File.Exists(txtImportExcel.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Items (1)$E3:E4]");
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
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), string.Format("SELECT * FROM [Items (1)$B5:C{0}]", itemCount+5));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }     
      this.FillDataForGrid(dsItemList.Tables[0]);
    }

    

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "MOQItemList";
      string sheetName = "Items";
      string outFileName = "MOQ Item List ";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate\";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }
    
    private void btnBrowser_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }
    #endregion event
  }
}
