/*
  Author        : Võ Hoa Lư
  Create date   : 20/06/2011
  Decription    : Show/update the item's price by customer  
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
namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_006 : MainUserControl
  {
    #region Fields
    private long customerPid = long.MinValue;    
    #endregion Fields

    #region Init Form
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_03_006()
    {
      InitializeComponent();
    }
    
    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    private void viewCSD_03_006_Load(object sender, EventArgs e)
    {
      ControlUtility.LoadUltraComboCustomer(ucmbCustomer, "WHERE Pid > 3 AND ParentPid IS NULL AND DeletedFlg = 0", false);
    }
    #endregion Init Form

    #region Load Data
    /// <summary>
    /// Enabled, disabled btnFocus
    /// </summary>
    private void SetStatusControl()
    {
      long customerPid = long.MinValue;
      if (ucmbCustomer.SelectedRow != null)
      {
        customerPid = DBConvert.ParseLong(ucmbCustomer.SelectedRow.Cells["Pid"].Value.ToString());
      }
      string findCode = txtItemCode.Text.Trim();
      btnFocus.Enabled = (customerPid != long.MinValue) && (findCode.Length > 0);
    }

    private void LoadData(long customerPid) { 
      btnSave.Enabled = false;
      DBParameter[] input = new DBParameter[] { new DBParameter("@CustomerPid", DbType.Int64, customerPid) };
      DataTable dataSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemActiveInfomation_SelectByCustomer", input);
      ultData.DataSource = dataSource;
      int count = ultData.DisplayLayout.Bands[0].Columns.Count;
      for (int i = 0; i < count; i++)
      {
        string name = ultData.DisplayLayout.Bands[0].Columns[i].ToString();
        if (string.Compare(name, "Active", true) == 0 || string.Compare(name, "QuickShip", true) == 0)
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
        }
        else {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      btnSave.Enabled = true;
    }
    #endregion Load Data

    #region Process Data
    /// <summary>
    /// Focus on itemcode (sale code) like txtItemCode.Text
    /// </summary>
    /// <param name="findKey"></param>
    private void FocusRow()
    {
      string findCode = txtItemCode.Text.Trim();
      if (findCode.Length == 0)
      {
        return;
      }
      int count = ultData.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        ultData.Rows[i].Selected = false;
      }
      for (int i = 0; i < count; i++)
      {
        string itemCode = ultData.Rows[i].Cells["ItemCode"].Value.ToString().ToUpper();
        string saleCode = ultData.Rows[i].Cells["SaleCode"].Value.ToString().ToUpper();
        if (itemCode.IndexOf(findCode.ToUpper()) >= 0 || saleCode.IndexOf(findCode.ToUpper()) >= 0)
        {
          ultData.Rows[i].Selected = true;
          return;
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageInfomation("MSG0038", new string[] { findCode });
    }
    #endregion ProcessData

    #region Event
    /// <summary>
    /// Load data when change customer
    /// Enabled, disabled btnFocus
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ucmbCustomer_ValueChanged(object sender, EventArgs e)
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
      else
      {
        ucmbCustomer.Enabled = true;
        chkEnable.Checked = true;
      }
      this.customerPid = selectedPid;

      this.LoadData(this.customerPid);
      this.SetStatusControl();
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
    }

    /// <summary>
    /// Reload data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRefresh_Click(object sender, EventArgs e)
    {
      this.LoadData(this.customerPid);
    }

    /// <summary>
    /// Enabled, disabled btnFocus
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtItemCode_TextChanged(object sender, EventArgs e)
    {
      this.SetStatusControl();
    }

    /// <summary>
    /// Focus on itemcode (sale code) like txtItemCode.Text
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.FocusRow();
      }
    }

    /// <summary>
    /// Focus on itemcode (sale code) like txtItemCode.Text
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFocus_Click(object sender, EventArgs e)
    {
      this.FocusRow();
    }

    /// <summary>
    /// Resize, hiden, readonly, enable colum in grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Rowstate"].Hidden = true;

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

    /// <summary>
    /// Update cell --> set rowstate = 1
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      e.Cell.Row.Cells["Rowstate"].Value = 1;
    }

    /// <summary>
    /// Open from import item active information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      viewCSD_03_007 view = new viewCSD_03_007();
      WindowUtinity.ShowView(view, "IMPORT ITEM'S PRICE FROM EXCEL FILE", false, ViewState.MainWindow);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = true;
      int outputValue = 0;
      int count = ultData.Rows.Count;
      int editBy = Shared.Utility.SharedObject.UserInfo.UserPid;
      for(int i = 0; i < count; i++){
        UltraGridRow row = ultData.Rows[i];
        int rowState = DBConvert.ParseInt(row.Cells["Rowstate"].Value.ToString());
        if(rowState == 1){
          DBParameter[] input = new DBParameter[5];
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

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
          if (outputValue != 1) {
            success = false;
          }
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
      this.LoadData(this.customerPid);
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
    #endregion Event
  }
}
