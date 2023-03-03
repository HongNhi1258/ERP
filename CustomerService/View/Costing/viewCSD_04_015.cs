/*
  Author        : Vo Van Duy Qui
  Create date   : 08/06/2012
  Decription    : Searching item cost price
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

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_015 : MainUserControl
  {
    #region Init
    /// <summary>
    /// Init view
    /// </summary>
    public viewCSD_04_015()
    {
      InitializeComponent();
    }

    /// <summary>
    /// View Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_04_014_Load(object sender, EventArgs e)
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
      //
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

        DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemCosting_Select", inputParam);
        lbCount.Text = string.Format("Count: {0}", dtSource.Rows.Count);
        ultData.DataSource = dtSource;
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
    #endregion Function

    #region Event

    /// <summary>
    /// Initialize layout of ultra grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
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
    }

    /// <summary>
    /// Show item cost price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Selected.Rows != null && ultData.Selected.Rows.Count > 0) 
      { 
        UltraGridRow row = ultData.Selected.Rows[0];
        string itemCode = row.Cells["ItemCode"].Value.ToString().Trim();
        if (itemCode.Length > 0)
        {
          this.ViewItemCosting(itemCode);
        }
      }
    }

    private void ViewItemCosting(string itemCode)
    {
      try
      {
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        DBParameter[] outParam = new DBParameter[3];
        outParam[0] = new DBParameter("@ExchangeRate", DbType.Double, double.MinValue);
        outParam[1] = new DBParameter("@FOH", DbType.Double, double.MinValue);
        outParam[2] = new DBParameter("@Profit", DbType.Double, double.MinValue);
        // Calculate BOM Price
        DataSet dsBOMPrice = DataBaseAccess.SearchStoreProcedure("spCSDItemCosting_Manufacture", inputParam, outParam);
        double totalVNDPrice = 0;
        double totalUSDPrice = 0;
        if (dsBOMPrice != null && dsBOMPrice.Tables.Count > 0)
        {
          foreach (DataRow priceRow in dsBOMPrice.Tables[1].Rows)
          {
            double vndPrice = DBConvert.ParseDouble(priceRow["Amount"].ToString());
            if (vndPrice > 0)
            {
              totalVNDPrice += vndPrice;
            }
            double usdPrice = DBConvert.ParseDouble(priceRow["USD"].ToString());
            if (usdPrice > 0)
            {
              totalUSDPrice += usdPrice;
            }
          }
        }
        // End Calculate BOM Price
        DBParameter[] outputParam = new DBParameter[4];
        outputParam[0] = new DBParameter("@ExchangeRate", DbType.Double, double.MinValue);
        outputParam[1] = new DBParameter("@FOH", DbType.Double, double.MinValue);
        outputParam[2] = new DBParameter("@Profit", DbType.Double, double.MinValue);
        outputParam[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, string.Empty);
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDItemCostingInformation", inputParam, outputParam);
        if (dsSource != null && dsSource.Tables.Count > 1)
        {
          dsSource.Tables[0].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dsSource.Tables[0].Columns.Add("CheckImage", typeof(String));
          for (int i = 0; i < dsSource.Tables[0].Rows.Count; i++)
          {
            dsSource.Tables[0].Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImageToByteArrayWithFormat(@dsSource.Tables[0].Rows[i]["img"].ToString(), 380, 1.74, "JPG");
            dsSource.Tables[0].Rows[i]["CheckImage"] = dsSource.Tables[0].Rows[i]["Image"].ToString();
          }
          dsSource.Tables[1].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
          dsSource.Tables[1].Columns.Add("CheckImage", typeof(String));
          for (int i = 0; i < dsSource.Tables[1].Rows.Count; i++)
          {
            dsSource.Tables[1].Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray_Always(@dsSource.Tables[1].Rows[i]["Picture"].ToString());
            dsSource.Tables[1].Rows[i]["CheckImage"] = dsSource.Tables[1].Rows[i]["Image"].ToString();
          }

          Shared.DataSetSource.CustomerService.dsCSDItemCostPrice ds = new Shared.DataSetSource.CustomerService.dsCSDItemCostPrice();
          ds.Tables["dtItemCostMaster"].Merge(dsSource.Tables[0]);
          ds.Tables["dtItemCostDetail"].Merge(dsSource.Tables[1]);

          Shared.ReportTemplate.CustomerService.cptCSDItemCostPrice1 cptItemCostPrice = new Shared.ReportTemplate.CustomerService.cptCSDItemCostPrice1();
          cptItemCostPrice.SetDataSource(ds);
          double dExchange = DBConvert.ParseDouble(outputParam[0].Value.ToString());
          double dFOH = DBConvert.ParseDouble(outputParam[1].Value.ToString());
          double dProfit = DBConvert.ParseDouble(outputParam[2].Value.ToString());
          string remark = outputParam[3].Value.ToString();
          cptItemCostPrice.SetParameterValue("ExchangeRate", dExchange);
          cptItemCostPrice.SetParameterValue("FOH", dFOH);
          cptItemCostPrice.SetParameterValue("Profit", dProfit);
          cptItemCostPrice.SetParameterValue("Remark", remark);
          totalVNDPrice = totalVNDPrice + ((totalVNDPrice * dFOH) / 100) + (((totalVNDPrice + ((totalVNDPrice * dFOH) / 100)) * dProfit) / 100);
          totalUSDPrice = totalUSDPrice + ((totalUSDPrice * dFOH) / 100) + (((totalUSDPrice + ((totalUSDPrice * dFOH) / 100)) * dProfit) / 100);
          cptItemCostPrice.SetParameterValue("TotalBOMPrice_VND", totalVNDPrice);
          cptItemCostPrice.SetParameterValue("TotalBOMPrice_USD", totalUSDPrice);

          // Contract out
          DataRow[] contractOutRow = dsSource.Tables[1].Select("Group = 6");
          if (contractOutRow.Length > 0)
          {
            cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = false;
            cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = true;
          }
          else
          {
            cptItemCostPrice.ContractOutFooter.SectionFormat.EnableSuppress = true;
            cptItemCostPrice.ManufactureFooter.SectionFormat.EnableSuppress = false;
          }

          View_Report frm = new View_Report(cptItemCostPrice);
          frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
        }
      }
      catch
      {
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
  }
}
