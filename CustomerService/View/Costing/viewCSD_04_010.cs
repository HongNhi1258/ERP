/*
  Author        : Võ Hoa Lư
  Create date   : 16/06/2011
  Decription    : Import SPIL/VFR price from excel file
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System.Diagnostics;
using System.IO;
using System.Data.OleDb;
using VBReport;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_010 : MainUserControl
  {
    #region Fields
    private string importFileName = string.Empty;
    public bool isFactory = false;
    #endregion Fields

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_04_010()
    {
      InitializeComponent();
    }
    #endregion Init Form

    #region Process Data
    /// <summary>
    /// Delete data in table TblCSDItemSalePrice_ImportTemp by ImportKey
    /// </summary>
    /// <param name="importKey"></param>
    private void DeteteTempData(string importKey) {
      string commandText = "DELETE FROM TblCSDItemSalePrice_ImportTemp WHERE ImportKey = @ImportKey";
      DBParameter[] deleteParams = new DBParameter[] { new DBParameter("@ImportKey", DbType.AnsiString, 32, importKey) };
      DataBaseAccess.ExecuteCommandText(commandText, deleteParams);
    }

    /// <summary>
    ///  Save data from excel in to table TblCSDItemSalePrice_ImportTemp
    /// </summary>
    private void SaveTempData(string importKey)
    {
      string commandText = string.Empty;
      DataTable dataXLS = new DataTable();
      try
      {
        OleDbConnection connection = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + this.importFileName + ";Extended Properties=Excel 8.0;");
        commandText = string.Format(@"SELECT * FROM [Data (1)$B5:D2005]");
        OleDbDataAdapter adp = new OleDbDataAdapter(commandText, connection);
        adp.Fill(dataXLS);
        connection.Close();
        connection.Dispose();
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0069");
        return;
      }

      DataTable dataImport = new DataTable();
      dataImport.Columns.Add("No", typeof(string));
      dataImport.Columns.Add("ItemCode", typeof(string));
      dataImport.Columns.Add("Price", typeof(double));
      dataImport.Columns.Add("ImportKey", typeof(string));
      dataImport.Columns["ImportKey"].DefaultValue = importKey;
      if (dataXLS != null && dataXLS.Rows.Count > 0)
      {
        foreach (DataRow row in dataXLS.Rows)
        {
          DataRow newRow = dataImport.NewRow();
          if (row["ItemCode"].ToString().Trim().Length > 0)
          {
            newRow["No"] = row["No"];
            newRow["ItemCode"] = row["ItemCode"];
            double price = DBConvert.ParseDouble(row["Price"]);
            if (price > 0)
            {
              newRow["Price"] = price;
            }
            dataImport.Rows.Add(newRow);
          }
        }
      }
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ImportKey", DbType.AnsiString, 32, importKey) };
      commandText = string.Format(@"SELECT No, ItemCode, Price, ImportKey FROM TblCSDItemSalePrice_ImportTemp WHERE ImportKey = @ImportKey");
      DataBaseAccess.UpdateDataTable(dataImport, commandText, inputParam);
    }

    /// <summary>
    /// Load infomation on grid
    /// </summary>
    private void LoadData(string importKey) {
      long customerPid = DBConvert.ParseLong(ultraCBCustomer.Value.ToString());
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ImportKey", DbType.AnsiString, 32, importKey);
      inputParam[1] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      DataTable dataSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemSalePrice_ImportSelect", inputParam);
      ultData.DataSource = dataSource;
      int count = ultData.DisplayLayout.Bands[0].Columns.Count;
      for (int i = 0; i < count; i++)
      {
        string name = ultData.DisplayLayout.Bands[0].Columns[i].ToString();
        if (string.Compare(name, "Price", true) != 0)
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
        else
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightCyan;
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
        }
      }
      count = ultData.Rows.Count;
      for (int i = 0; i < count; i++) { 
        UltraGridRow row = ultData.Rows[i];
        int isValid = DBConvert.ParseInt(row.Cells["IsValidItem"].Value.ToString()) + DBConvert.ParseInt(row.Cells["IsActive"].Value.ToString());
        if (isValid < 2) {
          row.Appearance.BackColor = Color.LightGray;
        }
      }
      btnSave.Enabled = (count > 0);
    }

    private bool CheckValid() {
      if (ultraCBCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("MSG0011", "Customer");
        return false;
      }
      int count = ultData.Rows.Count;
      for (int i = 0; i < count; i++) {
        UltraGridRow row = ultData.Rows[i];
        int isValid = DBConvert.ParseInt(row.Cells["IsValidItem"].Value.ToString()) + DBConvert.ParseInt(row.Cells["IsActive"].Value.ToString());
        if (isValid < 2)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0097");
          row.Selected = true;
          return false;
        }
        double price = DBConvert.ParseDouble(row.Cells["Price"].Value.ToString());
        if (price == double.MinValue) {
          row.Cells["Price"].Selected = true;
          Shared.Utility.WindowUtinity.ShowMessageError("WRN0013", "Price");
          return false;
        }
        else if (price <= 0) {
          row.Cells["Price"].Selected = true;
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Price");
          return false;
        }
      }
      return true;
    }

    private void LoadDataCustomer()
    {
      DataTable dtSource;
      if (this.isFactory)
      {
        dtSource = new DataTable();
        dtSource.Columns.Add("Pid", typeof(System.Int64));
        dtSource.Columns.Add("Code", typeof(System.String));
        DataRow row1 = dtSource.NewRow();
        row1["Pid"] = 1;
        row1["Code"] = "Factory";
        dtSource.Rows.Add(row1);
        DataRow row2 = dtSource.NewRow();
        row2["Pid"] = 12;
        row2["Code"] = "US";
        dtSource.Rows.Add(row2);
        DataRow row3 = dtSource.NewRow();
        row3["Pid"] = 11;
        row3["Code"] = "UK";
        dtSource.Rows.Add(row3);
        DataRow row4 = dtSource.NewRow();
        row4["Pid"] = 146;
        row4["Code"] = "Factory International";
        dtSource.Rows.Add(row4);
        ultraCBCustomer.DataSource = dtSource;

        ultraCBCustomer.DisplayMember = "Code";
        ultraCBCustomer.ValueMember = "Pid";        
      }
      else
      {
        string commandText = @"Select Pid, CustomerCode Code, Name, (CustomerCode + ' - ' + Name) Display 
                            From TblCSDCustomerInfo 
                            Where (ParentPid Is Null And Pid > 3 and Kind = 5 AND DeletedFlg = 0) Or (Pid = 1)
                            Order By CustomerCode";
        dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        ultraCBCustomer.DataSource = dtSource;        
        ultraCBCustomer.DisplayMember = "Display";
        ultraCBCustomer.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
        ultraCBCustomer.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 80;
        ultraCBCustomer.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 80;        
      }
      ultraCBCustomer.ValueMember = "Pid";
      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultraCBCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    #endregion Process Data

    #region Events
    /// <summary>
    /// Load UltraCBCustomer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_04_010_Load(object sender, EventArgs e)
    {
      this.LoadDataCustomer();
    }

    /// <summary>
    /// Selecte excel file which we want to import data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOpenDialog_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      this.importFileName = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      int index = this.importFileName.LastIndexOf('\\');
      if (index > 0)
      {
        txtFileName.Text = this.importFileName;//.Substring(index + 1, this.importFileName.Length - index - 1);
      }
      btnAdd.Enabled = (this.importFileName.Length > 0);
    }
    
    /// <summary>
    /// Get file template
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      //string startupPath = System.Windows.Forms.Application.StartupPath;
      //string folder = string.Format(@"{0}\Report", startupPath);
      //string templateName = string.Format(@"{0}\ExcelTemplate\ItemSalePrice.xls", startupPath);
      //if(File.Exists(templateName)){
      //  string newFileName = string.Format(@"{0}\ItemSalePrice.xls", folder);
      //  if (File.Exists(newFileName))
      //  {
      //    newFileName = string.Format(@"{0}\ItemSalePrice_{1}.xls", folder, DateTime.Now.Ticks);
      //  }
      //  File.Copy(templateName, newFileName);
      //  Process.Start(newFileName);
      //}

      string templateName = "ItemSalePrice";
      string sheetName = "Data";
      string outFileName = "ItemSalePrice";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    /// <summary>
    /// Display data from excel file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (ultraCBCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("MSG0011", "Customer");
        return;
      }
      btnAdd.Enabled = false;
      string importKey = string.Format(@"{0}_{1}_{2}", Environment.MachineName.ToString(), DateTime.Now.Ticks.ToString(), Shared.Utility.SharedObject.UserInfo.UserPid);
      this.SaveTempData(importKey);
      this.LoadData(importKey);
      this.DeteteTempData(importKey);
      btnAdd.Enabled = true;
    }

    /// <summary>
    /// Remove invalid rows in the grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRemove_Click(object sender, EventArgs e)
    {
      DataTable dataSource = (DataTable)ultData.DataSource;
      if(dataSource != null && dataSource.Rows.Count > 0){
        for (int i = 0; i < dataSource.Rows.Count; i++) {
          DataRow row = dataSource.Rows[i];
          int isValid = DBConvert.ParseInt(row["IsValidItem"].ToString()) + DBConvert.ParseInt(row["IsActive"].ToString());
          if (isValid < 2) {
            dataSource.Rows.RemoveAt(i);
            i--;
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0039");
      btnSave.Enabled = (ultData.Rows.Count > 0);
    }

    /// <summary>
    /// Save Data : 1. Insert/Update TblCSDItemSalePrice
    ///             2. Insert TblCSDItemSalePriceHistory
    /// </summary>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (!this.CheckValid()) {
        return;
      }
      long customerPid = DBConvert.ParseLong(ultraCBCustomer.Value.ToString());
      int count = ultData.Rows.Count;
      long outputValue = 0;
      bool success = true;
      for (int i = 0; i < count; i++) 
      {
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] input = new DBParameter[4];

        string itemCode = row.Cells["ItemCode"].Value.ToString();
        input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        input[1] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
        double price = DBConvert.ParseDouble(row.Cells["Price"].Value.ToString());
        input[2] = new DBParameter("@SchedulePrice", DbType.Double, price);
        input[3] = new DBParameter("@AdjustBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0)};
        DataBaseAccess.ExecuteStoreProcedure("spCSDItemSalePrice_EditSchedulePrice", input, output);
        outputValue = DBConvert.ParseLong(output[0].Value.ToString());
        if (outputValue == 0) {
          success = false;
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");        
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
    }

    /// <summary>
    /// Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Hiden/show, format and resize column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["IsValidItem"].Hidden = true;
      e.Layout.Bands[0].Columns["IsActive"].Hidden = true;

      e.Layout.Bands[0].Columns["No"].MinWidth = 40;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["No"].Format = "#,###";

      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 110;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 110;

      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 90;

      e.Layout.Bands[0].Columns["Price"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Price"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].Format = "#,##0.00";

      e.Layout.Bands[0].Columns["Status"].MinWidth = 120;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 120;
    }

    /// <summary>
    /// Check valid price before update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      if (string.Compare(columnName, "Price", true) == 0)
      {
        string text = e.Cell.Text.ToString().Trim();
        if (text.Length > 0)
        {
          double value = DBConvert.ParseDouble(text);
          if (value <= 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Price");
            e.Cancel = true;
          }
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("WRN0013", "Price");
          e.Cancel = true;
        }
      }  
    }

    /// <summary>
    /// Delete row --> update status btnSave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      btnSave.Enabled = (ultData.Rows.Count > 0);
    }

    private void chkEnableCustomer_CheckedChanged(object sender, EventArgs e)
    {
      if (!chkEnableCustomer.Checked && ultraCBCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("MSG0011", "Customer");
        chkEnableCustomer.Checked = true;
        return;
      }
      ultraCBCustomer.ReadOnly = !chkEnableCustomer.Checked;
    }

    private void ultraCBCustomer_ValueChanged(object sender, EventArgs e)
    {
      chkEnableCustomer.Checked = false;      
    }
    #endregion Events
  }
}