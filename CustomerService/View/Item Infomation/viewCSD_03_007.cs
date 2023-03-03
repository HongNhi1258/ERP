/*
  Author        : Võ Hoa Lư
  Create date   : 16/06/2011
  Decription    : Import item price from excel file  
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
namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_007 : MainUserControl
  {

    #region Fields
    private string importFileName = string.Empty;
    private long customerPid = long.MinValue;
    #endregion Fields

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_03_007()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    private void viewCSD_03_007_Load(object sender, EventArgs e)
    {
      ControlUtility.LoadUltraComboCustomer(ucmbCustomer, "WHERE Pid > 3 AND ParentPid IS NULL AND DeletedFlg = 0", true);      
    }
    #endregion Init Form

    #region Process Data
    /// <summary>
    /// Delete data in table TblCSDItemSalePrice_ImportTemp by ImportKey
    /// </summary>
    /// <param name="importKey"></param>
    private void DeteteTempData(string importKey) {
      string commandText = "DELETE FROM TblCSDItemActive_ImportTemp WHERE ImportKey = @ImportKey";
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
        commandText = string.Format(@"SELECT * FROM [Data$B5:E2005]");
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
      dataImport.Columns.Add("Active", typeof(float));
      dataImport.Columns.Add("QuickShip", typeof(float));
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
            newRow["Active"] = (string.Compare(row["Active"].ToString().Trim(), "X", true) == 0) ? 1 : 0;
            newRow["QuickShip"] = (string.Compare(row["QuickShip"].ToString().Trim(), "X", true) == 0) ? 1 : 0;
            dataImport.Rows.Add(newRow);
          }
        }
      }
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ImportKey", DbType.AnsiString, 32, importKey) };
      commandText = string.Format(@"SELECT No, ItemCode, Active, QuickShip, ImportKey FROM TblCSDItemActive_ImportTemp WHERE ImportKey = @ImportKey");
      DataBaseAccess.UpdateDataTable(dataImport, commandText, inputParam);
    }

    /// <summary>
    /// Load infomation on grid
    /// </summary>
    private void LoadData(string importKey) {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ImportKey", DbType.AnsiString, 32, importKey);
      inputParam[1] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);
      DataTable dataSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemActiveInfomation_ImportSelect", inputParam);
      ultData.DataSource = dataSource;
      int count = ultData.DisplayLayout.Bands[0].Columns.Count;
      for (int i = 0; i < count; i++)
      {
        string name = ultData.DisplayLayout.Bands[0].Columns[i].ToString();
        if (string.Compare(name, "Active", true) == 0 || string.Compare(name, "QuickShip", true) == 0)
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
        }
        else
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      count = ultData.Rows.Count;
      for (int i = 0; i < count; i++) { 
        UltraGridRow row = ultData.Rows[i];
        int isValid = DBConvert.ParseInt(row.Cells["IsValidItem"].Value.ToString());
        if (isValid < 1) {
          row.Appearance.BackColor = Color.LightGray;
        }
      }      
    }

    private bool CheckValid() { 
      int count = ultData.Rows.Count;
      for (int i = 0; i < count; i++) {
        UltraGridRow row = ultData.Rows[i];
        int isValid = DBConvert.ParseInt(row.Cells["IsValidItem"].Value.ToString());
        if (isValid < 1)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0097");
          row.Selected = true;
          return false;
        }
        int active = DBConvert.ParseInt(row.Cells["Active"].Value.ToString());
        int quickShip = DBConvert.ParseInt(row.Cells["QuickShip"].Value.ToString());
        if (active <= 0 && quickShip == 1) {
          Shared.Utility.WindowUtinity.ShowMessageError("MSG0041");
          row.Selected = true;
          return false;
        }
      }
      return true;
    }
    #endregion Process Data

    #region Events

    /// <summary>
    /// If Choose customer then disable control ucmbCustomer and uncheck checkbox chkEnable
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ucmbCustomer_TextChanged(object sender, EventArgs e)
    {
      long selectedPid = long.MinValue;
      if (ucmbCustomer.SelectedRow != null)
      {
        selectedPid = DBConvert.ParseLong(ucmbCustomer.SelectedRow.Cells["Pid"].Value.ToString());
      }
      if (selectedPid != long.MinValue)
      {
        ucmbCustomer.Enabled = false;
        chkEnable.Checked = false;        
      }
      else {
        ucmbCustomer.Enabled = true;
        chkEnable.Checked = true;
      }
      this.customerPid = selectedPid;
      DataTable dataSource = (DataTable)ultData.DataSource;
      if (dataSource != null && dataSource.Rows.Count > 0)
      {
        dataSource.Rows.Clear();
      }
    }

    /// <summary>
    /// Enable or disable ucmbCustomer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkEnable_CheckedChanged(object sender, EventArgs e)
    {
      bool isChecked = chkEnable.Checked;
      ucmbCustomer.Enabled = isChecked;
      btnSave.Enabled = !isChecked;
      btnAdd.Enabled = ((this.importFileName.Length > 0) && (!isChecked));
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
        txtFileName.Text = this.importFileName.Substring(index + 1, this.importFileName.Length - index - 1);
      }
      btnAdd.Enabled = ((this.importFileName.Length > 0) && (!chkEnable.Checked));
    }
    
    /// <summary>
    /// Get file template
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      string templateName = string.Format(@"{0}\ExcelTemplate\ItemActiveInformation.xls", startupPath);
      if(File.Exists(templateName)){
        string newFileName = string.Format(@"{0}\ItemActiveInformation.xls", folder);
        if (File.Exists(newFileName))
        {
          newFileName = string.Format(@"{0}\ItemActiveInformation_{1}.xls", folder, DateTime.Now.Ticks);
        }
        File.Copy(templateName, newFileName);
        Process.Start(newFileName);
      }
      // Delete all file in folder Report
      foreach (string file in Directory.GetFiles(folder))
      {
        try
        {
          File.Delete(file);
        }
        catch { }
      }
    }

    /// <summary>
    /// Display data from excel file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      btnAdd.Enabled = false;
      if (this.customerPid == long.MinValue)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Customer");
        return;
      }
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
          int isValid = DBConvert.ParseInt(row["IsValidItem"].ToString());
          if (isValid < 1) {
            dataSource.Rows.RemoveAt(i);
            i--;
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0039");
    }

    /// <summary>
    /// Save Data : 1. Insert/Update TblCSDItemSalePrice
    ///             2. Insert TblCSDItemSalePriceHistory
    /// </summary>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (!this.CheckValid())
      {
        return;
      }
      bool success = true;
      int editBy = Shared.Utility.SharedObject.UserInfo.UserPid;
      int count = ultData.Rows.Count;
      long outputValue = 0;
      for (int i = 0; i < count; i++)
      {
        DBParameter[] input = new DBParameter[5];
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        
        UltraGridRow row = ultData.Rows[i];
        string text = row.Cells["ItemCode"].Value.ToString();
        input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, text);

        input[1] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);

        int value = DBConvert.ParseInt(row.Cells["Active"].Value.ToString());
        value = (value <= 0) ? 0 : value;
        input[2] = new DBParameter("@IsActive", DbType.Int32, value);

        value = DBConvert.ParseInt(row.Cells["QuickShip"].Value.ToString());
        value = (value <= 0) ? 0 : value;
        input[3] = new DBParameter("@IsQuickShip", DbType.Int32, value);

        input[4] = new DBParameter("@EditBy", DbType.Int32, editBy);
        DataBaseAccess.ExecuteStoreProcedure("spCSDItemActiveInfomation_Edit", input, output);
        outputValue = DBConvert.ParseInt(output[0].Value.ToString());
        if (outputValue != 1)
        {
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
    /// format and resize column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["IsValidItem"].Hidden = true;      

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

      e.Layout.Bands[0].Columns["Active"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Active"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Active"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["QuickShip"].MinWidth = 70;
      e.Layout.Bands[0].Columns["QuickShip"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["QuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Status"].MinWidth = 120;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 120;
    }

    /// <summary>
    /// Unchecked active --> Unchecked QuickShip
    /// Checked Quickship --> Checked Active
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      int value = DBConvert.ParseInt(e.Cell.Text.ToString());
      if (string.Compare(columnName, "Active", true) == 0)
      {
        if (value <= 0)
        {
          e.Cell.Row.Cells["QuickShip"].Value = 0;
        }
      }
      else if (string.Compare(columnName, "QuickShip", true) == 0)
      {
        if (value == 1)
        {
          e.Cell.Row.Cells["Active"].Value = 1;
        }
      }
    }    
    #endregion Events
  }
}