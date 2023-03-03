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
using DaiCo.Shared.ReportTemplate.CustomerService;
using DaiCo.Shared.DataSetSource.CustomerService;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_017 : MainUserControl
  {
    #region Init
    public viewCSD_04_017()
    {
      InitializeComponent();
    }

    private void viewCSD_04_017_Load(object sender, EventArgs e)
    {
      ControlUtility.LoadUltraComboboxComponentGroup(ultCBGroup, 1);
    }
    #endregion Init

    #region Function
    private bool CheckValid()
    {
      if (ultCBGroup.Text.Trim().Length > 0 && ultCBGroup.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Component Group");
        return false;
      }
      return true;
    }

    private void Search()
    {
      if (this.CheckValid())
      {
        string code = txtCode.Text.Trim();
        long compGroup = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultCBGroup));
        DBParameter[] input = new DBParameter[2];
        if (code.Length > 0)
        {
          input[0] = new DBParameter("@CompCode", DbType.AnsiString, 16, "%" + code + "%");
        }
        if (compGroup != long.MinValue)
        {
          input[1] = new DBParameter("@CompGroupPid", DbType.Int64, compGroup);
        }

        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDHardwareCostPrice_Select", input);
        ultData.DataSource = dtSource;
      }
    }

    private void ViewHardwareCosting(string code)
    {
      try
      {
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@HardwareCode", DbType.AnsiString, 16, code);
        inputParam[1] = new DBParameter("@ViewType", DbType.Int32, rdMaterial.Checked ? 0 : 1);
        DBParameter[] outParam = new DBParameter[3];
        outParam[0] = new DBParameter("@ExchangeRate", DbType.Double, double.MinValue);
        outParam[1] = new DBParameter("@FOH", DbType.Double, double.MinValue);
        outParam[2] = new DBParameter("@Profit", DbType.Double, double.MinValue);
        // BOM Price
        DataSet dsCSDPrice = DataBaseAccess.SearchStoreProcedure("spCSDHardwareCostingInfo", inputParam, outParam);
        double totalVNDPrice = 0;
        double totalUSDPrice = 0;
        double factoryFOH = 1;
        double fatoryProfit = 1;
        if (dsCSDPrice != null && dsCSDPrice.Tables.Count > 0)
        {
          if (dsCSDPrice.Tables[0].Rows.Count > 0)
          {
            if (!dsCSDPrice.Tables[0].Columns.Contains("Image"))
            {
              dsCSDPrice.Tables[0].Columns.Add("Image", System.Type.GetType("System.Byte[]"));
            }
            if (!dsCSDPrice.Tables[0].Columns.Contains("CheckImage"))
            {
              dsCSDPrice.Tables[0].Columns.Add("CheckImage", System.Type.GetType("System.String"));
            }
            dsCSDPrice.Tables[0].Rows[0]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray(@dsCSDPrice.Tables[0].Rows[0]["Picture"].ToString());
            dsCSDPrice.Tables[0].Rows[0]["CheckImage"] = dsCSDPrice.Tables[0].Rows[0]["Picture"].ToString();
          }
          
          factoryFOH = DBConvert.ParseDouble(outParam[1].Value.ToString());
          fatoryProfit = DBConvert.ParseDouble(outParam[2].Value.ToString());
          if (dsCSDPrice.Tables[1].Rows.Count > 0)
          {
            foreach (DataRow priceRow in dsCSDPrice.Tables[1].Rows)
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
          else
          {
            DataRow newRow = dsCSDPrice.Tables[1].NewRow();
            dsCSDPrice.Tables[1].Rows.Add(newRow);
          }
        }
        totalVNDPrice = totalVNDPrice + ((totalVNDPrice * factoryFOH) / 100) + (((totalVNDPrice + ((totalVNDPrice * factoryFOH) / 100)) * fatoryProfit) / 100);
        totalUSDPrice = totalUSDPrice + ((totalUSDPrice * factoryFOH) / 100) + (((totalUSDPrice + ((totalUSDPrice * factoryFOH) / 100)) * fatoryProfit) / 100);
        // End BOM Price


        dsCSDHardwareCostPrice ds = new dsCSDHardwareCostPrice();
        ds.Tables["dtHardwareCostMaster"].Merge(dsCSDPrice.Tables[0]);
        ds.Tables["dtHardwareCostDetail"].Merge(dsCSDPrice.Tables[1]);

        cptCSDHardwareCostPrice cptCostPrice = new cptCSDHardwareCostPrice();
        cptCostPrice.SetDataSource(ds);
        double dExchange = DBConvert.ParseDouble(outParam[0].Value.ToString());
        double dFOH = DBConvert.ParseDouble(outParam[1].Value.ToString());
        double dProfit = DBConvert.ParseDouble(outParam[2].Value.ToString());
        cptCostPrice.SetParameterValue("ExchangeRate", dExchange);
        cptCostPrice.SetParameterValue("FOH", dFOH);
        cptCostPrice.SetParameterValue("Profit", dProfit);
        cptCostPrice.SetParameterValue("TotalBOMPrice_VND", totalVNDPrice);
        cptCostPrice.SetParameterValue("TotalBOMPrice_USD", totalUSDPrice);
        cptCostPrice.SetParameterValue("User", SharedObject.UserInfo.EmpName);

        View_Report frm = new View_Report(cptCostPrice);
        frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
      }
      catch { }
    }
    #endregion Function

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Selected.Rows != null && ultData.Selected.Rows.Count > 0)
      {
        UltraGridRow row = ultData.Selected.Rows[0];
        string code = row.Cells["Code"].Value.ToString().Trim();
        if (code.Length > 0)
        {
          this.ViewHardwareCosting(code);
        }
      }
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      e.Layout.Bands[0].Columns["Code"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Code"].MinWidth = 70;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    private void Object_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }
  }
}
