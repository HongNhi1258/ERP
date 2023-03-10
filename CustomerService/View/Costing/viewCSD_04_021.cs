/*
  Author        : 
  Create date   : 12/08/2014
  Decription    : Searching Compare item cost price
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
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using Infragistics.Win.UltraWinGrid;
using PresentationControls;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_021 : MainUserControl
  {
    #region Init
    /// <summary>
    /// Init view
    /// </summary>
    public viewCSD_04_021()
    {
      InitializeComponent();
    }

    /// <summary>
    /// View Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_04_021_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load data for ultracombo Customer and Status
    /// </summary>
    private void LoadData()
    {
      // Load data for ultra combo Customer
      ControlUtility.LoadUltraCBCustomer(ultCBCustomer);
      ultCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBCustomer.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCBCustomer.DisplayLayout.Bands[0].Columns["Name"].Hidden = true;
      // Create data for ultra combo Status
      DataTable dt = new DataTable();
      dt.Columns.Add("Value", typeof(System.Int32));
      dt.Columns.Add("Text", typeof(System.String));

      DataRow row1 = dt.NewRow();
      row1["Value"] = 0;
      row1["Text"] = "Not Confirmed";
      dt.Rows.Add(row1);

      DataRow row2 = dt.NewRow();
      row2["Value"] = 1;
      row2["Text"] = "Confirmed";
      dt.Rows.Add(row2);
      ultCBStatus.DataSource = dt;
      ultCBStatus.ValueMember = "Value";
      ultCBStatus.DisplayMember = "Text";
      ultCBStatus.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Check valid input data
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      if (ultCBCustomer.Text.Trim().Length > 0 && ultCBCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Customer");
        return false;
      }      
      if (ultCBStatus.Value == null && ultCBStatus.Text.Trim().Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Status");
        return false;
      }
      return true;
    }

    /// <summary>
    /// Search information with conditionals 
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;
      if (this.CheckInvalid())
      {
        string itemCode = txtItemCode.Text.Trim();
        string saleCode = txtSaleCode.Text.Trim();
        string oldCode = txtOldCode.Text.Trim();
        string carcassCode = txtCarCode.Text.Trim();
        long cusPid = long.MinValue;
        try 
        { 
          cusPid = DBConvert.ParseLong(ultCBCustomer.Value.ToString()); 
        }
        catch { }
        int status = int.MinValue;
        try
        {
          status = DBConvert.ParseInt(ultCBStatus.Value.ToString());
        }
        catch { }

        DBParameter[] inputParam = new DBParameter[6];
        if (itemCode.Length > 0)
        {
          inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + itemCode.Replace("'", "''") + "%");
        }
        if (saleCode.Length > 0)
        {
          inputParam[1] = new DBParameter("@SaleCode", DbType.AnsiString, 16, "%" + saleCode.Replace("'", "''") + "%");
        }
        if (carcassCode.Length > 0)
        {
          inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, "%" + carcassCode.Replace("'", "''") + "%");
        }
        if (cusPid != long.MinValue)
        {
          inputParam[3] = new DBParameter("@CustomerPid", DbType.Int64, cusPid);
        }
        if (oldCode.Length > 0)
        {
          inputParam[4] = new DBParameter("@OldCode", DbType.AnsiString, 16, "%" + oldCode.Replace("'", "''") + "%");
        }
        if (status != int.MinValue)
        {
          inputParam[5] = new DBParameter("@Status", DbType.Int32, status);
        }

        DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCSDItemCostingCompare_Select", inputParam);
        // Info choose compare
        if (ultTypeCompare.Rows.Count == 0)
        {
          ultTypeCompare.DataSource = dsSource.Tables[0];
        }

        // Detail
        lbCount.Text = string.Format("Count: {0}", dsSource.Tables[1].Rows.Count);
        ultData.DataSource = dsSource.Tables[1];
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          int confirm = DBConvert.ParseInt(ultData.Rows[i].Cells["Confirm"].Value.ToString());
          if (confirm == 1)
          {
            ultData.Rows[i].Appearance.BackColor = Color.LightGray;
          }
        }
      }
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Check Only 2 Option
    /// </summary>
    private bool CheckWhenDoubleClick()
    {
      int count = 0;
      for (int i = 0; i < ultTypeCompare.Rows[0].Cells.Count; i++)
      {
        if(DBConvert.ParseInt(ultTypeCompare.Rows[0].Cells[i].Value.ToString()) == 1)
        {
          count = count + 1;
        }
      }
      if(count != 2)
      {
        return false;
      }
      return true;
    }

    #endregion Function

    #region Event

    /// <summary>
    /// Initialize layout of ultra grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["CustomerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Confirm"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["NameVN"].Header.Caption = "Vietnamese Name";
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultTypeCompare_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Original"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Last"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Current"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Standard"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Actual"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Show item cost price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      // Check
      if(!this.CheckWhenDoubleClick())
      {
        return;
      }

      int compare1 = 0;
      int compare2 = 0;
      if(DBConvert.ParseInt(ultTypeCompare.Rows[0].Cells["Original"].Value.ToString()) == 1)
      {
        compare1 = 1;
      }
      if (DBConvert.ParseInt(ultTypeCompare.Rows[0].Cells["Last"].Value.ToString()) == 1)
      {
        if (compare1 == 0)
        {
          compare1 = 2;
        }
        else
        {
          compare2 = 2;
        }
      }

      if (DBConvert.ParseInt(ultTypeCompare.Rows[0].Cells["Current"].Value.ToString()) == 1)
      {
        if (compare1 == 0)
        {
          compare1 = 3;
        }
        else
        {
          compare2 = 3;
        }
      }

      if (DBConvert.ParseInt(ultTypeCompare.Rows[0].Cells["Standard"].Value.ToString()) == 1)
      {
        if (compare1 == 0)
        {
          compare1 = 4;
        }
        else
        {
          compare2 = 4;
        }
      }

      if (DBConvert.ParseInt(ultTypeCompare.Rows[0].Cells["Actual"].Value.ToString()) == 1)
      {
        if (compare1 == 0)
        {
          compare1 = 5;
        }
        else
        {
          compare2 = 5;
        }
      }

      if (ultData.Selected.Rows != null && ultData.Selected.Rows.Count > 0) 
      { 
        UltraGridRow row = ultData.Selected.Rows[0];
        string itemCode = row.Cells["ItemCode"].Value.ToString().Trim();
        if (itemCode.Length > 0)
        {
          int viewType = -1;
          if (rdMakeLocal.Checked)
          {
            viewType = 0;
          }
          else if (rdContractOut.Checked)
          {
            viewType = 1;
          }
          FunctionUtility.ViewItemCostingCompare(itemCode, 0, compare1, compare2, viewType);
        }
      }
    }

    /// <summary>
    /// Search information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Close view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    
    /// <summary>
    /// Event when key Enter down
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Object_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }
    #endregion Event

    private void ultData_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
    {
      string typeName = e.Type.Name;
      if ((string.Compare("ColumnHeader", typeName, true) == 0) && (ultData.Selected != null) && (ultData.Selected.Columns.Count > 0))
      {
        string colName = ultData.Selected.Columns[0].Caption;
        if (string.Compare(colName, "Actual\nInhouse\nPrice 1", true) == 0)
        {
          Popup description = new Popup(labDescription);
          description.AutoSize = true;
          labDescription.Text = "Standard Inhouse Price";
          description.Show(new Point(MousePosition.X, MousePosition.Y - 30));
        }
        else if (string.Compare(colName, "Actual\nSubCon\nPrice 1", true) == 0)
        {
          Popup description = new Popup(labDescription);
          description.AutoSize = true;
          labDescription.Text = "Standard SubCon Price";
          description.Show(new Point(MousePosition.X, MousePosition.Y - 30));
        }
      }
    }

    private void ultData_AfterCellActivate(object sender, EventArgs e)
    {
      UltraGridCell cell = ultData.ActiveCell;
      if(string.Compare(cell.Column.Header.Caption, "Item Code", true) == 0)
      {
        string saleCode = ultData.ActiveCell.Row.Cells["SaleCode"].Value.ToString();
        Popup description = new Popup(labDescription);
        description.AutoSize = true;
        labDescription.Text = saleCode;
        description.Show(new Point(MousePosition.X, MousePosition.Y - 30));
      }
    }

    private void ultData_CellListSelect(object sender, CellEventArgs e)
    {

    }

    private void ultData_MouseLeaveElement(object sender, Infragistics.Win.UIElementEventArgs e)
    {

    }

    private void ultData_MouseLeave(object sender, EventArgs e)
    {
      
    }

    private void ultData_MouseHover(object sender, EventArgs e)
    {
      
    }

    private void ultData_CellChange(object sender, CellEventArgs e)
    {
     
    }
  }
}
