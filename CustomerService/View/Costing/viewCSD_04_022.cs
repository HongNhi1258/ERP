/*
  Author        : Võ Hoa Lư
  Create date   : 16/06/2011
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
using DaiCo.Technical;
using PresentationControls;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_022 : MainUserControl
  {
    #region Fields
    private double factoryOverHead = double.MinValue;
    private double contractOutFOH = double.MinValue;
    private double profit = double.MinValue;
    private double us_Margin = double.MinValue;
    private double uk_Margin = double.MinValue;
    private double ex_USD_GBP = double.MinValue;
    private double ex_USD_VND = double.MinValue;
    private const long US_PID = 12;
    private const long UK_PID = 11;
    private const long FACTORY_PID = 1;
    private const long DCI_PID = 146;
    private bool flg = false;
    #endregion Fields

    #region Init Form
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_04_022()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    private void viewCSD_04_022_Load(object sender, EventArgs e)
    {
      this.LoadTypeCompare();
      this.flg = false;
      //Load Customer
      ControlUtility.LoadUltraCBCustomer(ultraCBCustomer);
    }
    #endregion Init Form

    #region Method Data

    /// <summary>
    /// Check Only 2 Option
    /// </summary>
    private bool CheckWhenDoubleClick()
    {
      int count = 0;
      for (int i = 0; i < ultTypeCompare.Rows[0].Cells.Count; i++)
      {
        if (DBConvert.ParseInt(ultTypeCompare.Rows[0].Cells[i].Value.ToString()) == 1)
        {
          count = count + 1;
        }
      }
      if (count != 2)
      {
        return false;
      }
      return true;
    }
    /// <summary>
    /// Load Customer
    /// </summary>
//    public static void LoadUltraCBCustomer(UltraCombo ultraCBCustomer)
//    {
//      string commandText = @"Select Pid, CustomerCode Code, Name, (CustomerCode + ' - ' + Name) Display 
//                            From TblCSDCustomerInfo 
//                            Where ParentPid Is Null And Pid > 3
//                            Order By CustomerCode";
//      DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandText);
//      ultraCBCustomer.DataSource = dtCustomer;
//      ultraCBCustomer.ValueMember = "Pid";
//      ultraCBCustomer.DisplayMember = "Display";
//      ultraCBCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
//      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
//      ultraCBCustomer.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
//    }
    /// <summary>
    /// Load grid checkbox show column
    /// </summary>
    private void LoadGridShowColumn() {
      DataTable dataSource = new DataTable();
      dataSource.Columns.Add("Customer", typeof(string));
      dataSource.Columns.Add("ALL", typeof(int));
      dataSource.Columns["ALL"].DefaultValue = 0;

      dataSource.Columns.Add("Price5", typeof(int));
      dataSource.Columns["Price5"].DefaultValue = 0;

      dataSource.Columns.Add("Date5", typeof(int));
      dataSource.Columns["Date5"].DefaultValue = 0;

      dataSource.Columns.Add("Price4", typeof(int));
      dataSource.Columns["Price4"].DefaultValue = 0;

      dataSource.Columns.Add("Date4", typeof(int));
      dataSource.Columns["Date4"].DefaultValue = 0;

      dataSource.Columns.Add("Price3", typeof(int));
      dataSource.Columns["Price3"].DefaultValue = 0;

      dataSource.Columns.Add("Date3", typeof(int));
      dataSource.Columns["Date3"].DefaultValue = 0;

      dataSource.Columns.Add("Price2", typeof(int));
      dataSource.Columns["Price2"].DefaultValue = 0;

      dataSource.Columns.Add("Date2", typeof(int));
      dataSource.Columns["Date2"].DefaultValue = 0;

      dataSource.Columns.Add("Price1", typeof(int));
      dataSource.Columns["Price1"].DefaultValue = 1;

      dataSource.Columns.Add("Date1", typeof(int));
      dataSource.Columns["Date1"].DefaultValue = 1;

      dataSource.Columns.Add("Default", typeof(int));
      dataSource.Columns["Default"].DefaultValue = 1;

      dataSource.Columns.Add("ActualPrice", typeof(int));
      dataSource.Columns["ActualPrice"].DefaultValue = 1;

      dataSource.Columns.Add("Change", typeof(int));
      dataSource.Columns["Change"].DefaultValue = 1;

      dataSource.Columns.Add("Makup", typeof(int));
      dataSource.Columns["Makup"].DefaultValue = 1;

      dataSource.Columns.Add("SchedulePrice", typeof(int));
      dataSource.Columns["SchedulePrice"].DefaultValue = 1;

      dataSource.Columns.Add("NewChange", typeof(int));
      dataSource.Columns["NewChange"].DefaultValue = 1;

      dataSource.Columns.Add("NewMakup", typeof(int));
      dataSource.Columns["NewMakup"].DefaultValue = 1;

      dataSource.Columns.Add("Active", typeof(int));
      dataSource.Columns["Active"].DefaultValue = 1;
      
      DataRow row = dataSource.NewRow();
      row["Customer"] = "US";
      dataSource.Rows.Add(row);            

      row = dataSource.NewRow();
      row["Customer"] = "Factory";
      dataSource.Rows.Add(row);

      row = dataSource.NewRow();
      row["Customer"] = "UK";
      dataSource.Rows.Add(row);

      row = dataSource.NewRow();
      row["Customer"] = "FacIn.";
      dataSource.Rows.Add(row);

      row = dataSource.NewRow();
      row["Customer"] = "OEM";
      dataSource.Rows.Add(row);

      ultShowColumn.DataSource = dataSource;
      ultShowColumn.DisplayLayout.Bands[0].ColHeaderLines = 2;
      ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
      ultShowColumn.Rows[1].Appearance.BackColor = Color.LightCyan;
      ultShowColumn.Rows[1].Cells["Makup"].Value = 0;
      ultShowColumn.Rows[1].Cells["Makup"].Activation = Activation.ActivateOnly;
      ultShowColumn.Rows[1].Cells["NewMakup"].Value = 0;
      ultShowColumn.Rows[1].Cells["NewMakup"].Activation = Activation.ActivateOnly;
      ultShowColumn.Rows[1].Cells["Active"].Value = 0;
      ultShowColumn.Rows[1].Cells["Active"].Activation = Activation.ActivateOnly;
      ultShowColumn.Rows[2].Appearance.BackColor = Color.LightSkyBlue;
      ultShowColumn.Rows[3].Appearance.BackColor = Color.LightGreen;
      ultShowColumn.Rows[4].Appearance.BackColor = Color.Wheat;

      this.ChkAll_CheckedChange(0);
      this.ChkAll_CheckedChange(1);
      this.ChkAll_CheckedChange(2);
      this.ChkAll_CheckedChange(3);
      this.ChkAll_CheckedChange(4);
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange(int rowIndex) {
      string prefix = (rowIndex == 0) ? "US" : (rowIndex == 1) ? "DC" : (rowIndex == 2) ? "UK" : (rowIndex == 3) ? "DCI" : "DIS";
      for (int colIndex = 2; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++) {
        UltraGridCell cell = ultShowColumn.Rows[rowIndex].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly) 
        { 
          string columnName = cell.Column.ToString().Trim();
          if ((string.Compare(columnName, "Default", true) == 0) && (rowIndex == 1))
          {
            ultData.DisplayLayout.Bands[0].Columns[prefix + "Inhouse" + columnName].Hidden = !cell.Value.ToString().Equals("1");
            ultData.DisplayLayout.Bands[0].Columns[prefix + "Subcon" + columnName].Hidden = !cell.Value.ToString().Equals("1");
          }
          else
          {
            ultData.DisplayLayout.Bands[0].Columns[prefix + columnName].Hidden = !cell.Value.ToString().Equals("1");
          }
        }
      }      
    }

    /// <summary>
    /// Load Choose Compate
    /// </summary>
    private void LoadTypeCompare()
    {
      DataTable dtTypeCompate = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemCostPriceFrozenType");
      ultTypeCompare.DataSource = dtTypeCompate;
    }

    /// <summary>
    /// Load information item price from screen's condition
    /// </summary>
    private void LoadData()
    {
      groupBoxInformation.Text = string.Format("Information of US && UK");
      string code = txtItemCode.Text.Trim();
      long customerPid = DBConvert.ParseLong(ultraCBCustomer.Value);
      DBParameter[] input = new DBParameter[2];
      if (code.Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, string.Format("%{0}%", code));
      }
      if (customerPid > 0)
      {
        input[1] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }
      DBParameter[] output = new DBParameter[7];
      output[0] = new DBParameter("@FOH", DbType.Double, 0);
      output[1] = new DBParameter("@Profit", DbType.Double, 0);
      output[2] = new DBParameter("@US_Margin", DbType.Double, 0);
      output[3] = new DBParameter("@UK_Margin", DbType.Double, 0);
      output[4] = new DBParameter("@Ex_USD_GBP", DbType.Double, 0);
      output[5] = new DBParameter("@Ex_USD_VND", DbType.Double, 0);
      output[6] = new DBParameter("@ContractOutFOH", DbType.Double, 0);

      DataTable dataSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemSalePrice_SelectByAll", 36000, input, output);
      ultData.DataSource = dataSource;
      ultData.DisplayLayout.Bands[0].ColHeaderLines = 3;
      int columnCount = ultData.DisplayLayout.Bands[0].Columns.Count;
      for (int i = 0; i < columnCount; i++)
      {
        ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      ultData.DisplayLayout.Bands[0].Columns["USSchedulePrice"].CellActivation = Activation.AllowEdit;
      ultData.DisplayLayout.Bands[0].Columns["USActive"].CellActivation = Activation.AllowEdit;
      ultData.DisplayLayout.Bands[0].Columns["DCISchedulePrice"].CellActivation = Activation.AllowEdit;
      ultData.DisplayLayout.Bands[0].Columns["DCIActive"].CellActivation = Activation.AllowEdit;
      ultData.DisplayLayout.Bands[0].Columns["DCSchedulePrice"].CellActivation = Activation.AllowEdit;
      ultData.DisplayLayout.Bands[0].Columns["UKSchedulePrice"].CellActivation = Activation.AllowEdit;
      ultData.DisplayLayout.Bands[0].Columns["UKActive"].CellActivation = Activation.AllowEdit;
      
      //// FOH
      //this.factoryOverHead = DBConvert.ParseDouble(output[0].Value.ToString());
      //txtFOH.Text = string.Format("{0}%", FunctionUtility.NumericFormat(this.factoryOverHead * 100, 2));

      //// Contract Out FOH
      //this.contractOutFOH = DBConvert.ParseDouble(output[6].Value.ToString());
      //txtContractOutFOH.Text = string.Format("{0}%", FunctionUtility.NumericFormat(this.contractOutFOH * 100, 2));

      //// Profit
      //this.profit = DBConvert.ParseDouble(output[1].Value.ToString());
      //txtProfit.Text = string.Format("{0}%", FunctionUtility.NumericFormat(this.profit * 100, 2));

      //// US Margin
      //this.us_Margin = DBConvert.ParseDouble(output[2].Value.ToString());
      //txtUSMargin.Text = string.Format("{0}", FunctionUtility.NumericFormat(this.us_Margin, 2));

      //// UK Margin
      //this.uk_Margin = DBConvert.ParseDouble(output[3].Value.ToString());
      //txtUKMargin.Text = string.Format("{0}", FunctionUtility.NumericFormat(this.uk_Margin, 2));

      //// USD/GBP
      //this.ex_USD_GBP = DBConvert.ParseDouble(output[4].Value.ToString());
      //txtUSD_GBP.Text = string.Format("{0}", FunctionUtility.NumericFormat(this.ex_USD_GBP, 5));

      //// USD/VND
      //this.ex_USD_VND = DBConvert.ParseDouble(output[5].Value.ToString());
      //txtUSD_VND.Text = string.Format("{0}", FunctionUtility.NumericFormat(this.ex_USD_VND, 2));

      if (!this.flg) {
        this.flg = true;
        this.LoadGridShowColumn();        
      }      
      lbCount.Text = string.Format("Count: {0}", ultData.Rows.FilteredInRowCount);
    }

    /// <summary>
    /// Save Data : 1. Insert/Update TblCSDItemSalePrice
    ///             2. Insert/ Update TblCSDItemActiveInfomation
    /// </summary>
    private bool SaveData()
    {
      int outputValue = 0;
      bool success = true;
      int count = ultData.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        int rowstate = DBConvert.ParseInt(row.Cells["Rowstate"].Value.ToString());
        if (rowstate == 1)
        {
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DBParameter[] input = new DBParameter[9];
          
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);

          double newPrice = DBConvert.ParseDouble(row.Cells["USSchedulePrice"].Value.ToString());
          if (newPrice != double.MinValue)
          {
            input[1] = new DBParameter("@USSchedulePrice", DbType.Double, newPrice);
          }

          newPrice = DBConvert.ParseDouble(row.Cells["DCSchedulePrice"].Value.ToString());
          if (newPrice != double.MinValue)
          {
            input[2] = new DBParameter("@FactorySchedulePrice", DbType.Double, newPrice);
          }          

          newPrice = DBConvert.ParseDouble(row.Cells["UKSchedulePrice"].Value.ToString());
          if (newPrice != double.MinValue)
          {
            input[3] = new DBParameter("@UKSchedulePrice", DbType.Double, newPrice);
          }

          newPrice = DBConvert.ParseDouble(row.Cells["DCISchedulePrice"].Value.ToString());
          if (newPrice != double.MinValue)
          {
            input[4] = new DBParameter("@DCISchedulePrice", DbType.Double, newPrice);
          }

          int active = DBConvert.ParseInt(row.Cells["USActive"].Value.ToString());
          input[5] = new DBParameter("@USActive", DbType.Int32, active);
          
          active = DBConvert.ParseInt(row.Cells["UKActive"].Value.ToString());
          input[6] = new DBParameter("@UKActive", DbType.Int32, active);

          active = DBConvert.ParseInt(row.Cells["DCIActive"].Value.ToString());
          input[7] = new DBParameter("@DCIActive", DbType.Int32, active);

          input[8] = new DBParameter("@AdjustBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);           

          DataBaseAccess.ExecuteStoreProcedure("spCSDItemSalePrice_EditSchedulePrice_ByFactory", input, output);
          outputValue = DBConvert.ParseInt(output[0].Value.ToString());
          if (outputValue <= 0)
          {
            success = false;
          }
        }
      }
      return success;
    }
    #endregion Method Data

    #region Grid Events

    /// <summary>
    /// Resize, hiden, readonly, enable colum in grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.UseFixedHeaders = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Rowstate"].Hidden = true;
      e.Layout.Bands[0].Columns["BOMConfirm"].Hidden = true;
      e.Layout.Bands[0].Columns["ContractOut"].Hidden = true;
      e.Layout.Bands[0].Columns["OriginalContractOut"].Hidden = true;
      e.Layout.Bands[0].Columns["LastContractOut"].Hidden = true;
      e.Layout.Bands[0].Columns["CurrentContractOut"].Hidden = true;

      // Base Data
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemCode"].Width = 70;

      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["SaleCode"].Width = 70;

      e.Layout.Bands[0].Columns["Name"].Width = 250;
      e.Layout.Bands[0].Columns["Unit"].Width = 45;
      e.Layout.Bands[0].Columns["Unit"].Header.Fixed = true;

      // US
      e.Layout.Bands[0].Columns["USPrice5"].Header.Caption = "US\nLast 5th\nPrice";
      e.Layout.Bands[0].Columns["USPrice5"].Width = 55;
      e.Layout.Bands[0].Columns["USPrice5"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["USPrice5"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USPrice5"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USDate5"].Header.Caption = "US\nLast 5th\nDate";
      e.Layout.Bands[0].Columns["USDate5"].Width = 70;
      e.Layout.Bands[0].Columns["USDate5"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USPrice4"].Header.Caption = "US\nLast 4th\nPrice";
      e.Layout.Bands[0].Columns["USPrice4"].Width = 55;
      e.Layout.Bands[0].Columns["USPrice4"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["USPrice4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USPrice4"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USDate4"].Header.Caption = "US\nLast 4th\nDate";
      e.Layout.Bands[0].Columns["USDate4"].Width = 70;
      e.Layout.Bands[0].Columns["USDate4"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USPrice3"].Header.Caption = "US\nLast 3rd\nPrice";
      e.Layout.Bands[0].Columns["USPrice3"].Width = 55;
      e.Layout.Bands[0].Columns["USPrice3"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["USPrice3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USPrice3"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USDate3"].Header.Caption = "US\nLast 3rd\nDate";
      e.Layout.Bands[0].Columns["USDate3"].Width = 70;
      e.Layout.Bands[0].Columns["USDate3"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USPrice2"].Header.Caption = "US\nLast 2nd\nPrice";
      e.Layout.Bands[0].Columns["USPrice2"].Width = 55;
      e.Layout.Bands[0].Columns["USPrice2"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["USPrice2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USPrice2"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USDate2"].Header.Caption = "US\nLast 2nd\nDate";
      e.Layout.Bands[0].Columns["USDate2"].Width = 70;
      e.Layout.Bands[0].Columns["USDate2"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USPrice1"].Header.Caption = "US\nLatest\nPrice";
      e.Layout.Bands[0].Columns["USPrice1"].Width = 55;
      e.Layout.Bands[0].Columns["USPrice1"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["USPrice1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USPrice1"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USDate1"].Header.Caption = "US\nLatest\nDate";
      e.Layout.Bands[0].Columns["USDate1"].Width = 70;
      e.Layout.Bands[0].Columns["USDate1"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USDefault"].Header.Caption = "US\nDef.\nPrice";
      e.Layout.Bands[0].Columns["USDefault"].Width = 55;
      e.Layout.Bands[0].Columns["USDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["USDefault"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USActualPrice"].Header.Caption = "US\nActual\nPrice";
      e.Layout.Bands[0].Columns["USActualPrice"].Width = 55;
      e.Layout.Bands[0].Columns["USActualPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["USActualPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USActualPrice"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USChange"].Header.Caption = "+/-%";
      e.Layout.Bands[0].Columns["USChange"].Width = 55;
      e.Layout.Bands[0].Columns["USChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["USChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USChange"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USMakup"].Header.Caption = "US\nMakup";
      e.Layout.Bands[0].Columns["USMakup"].Width = 55;
      e.Layout.Bands[0].Columns["USMakup"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["USMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USMakup"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USSchedulePrice"].Header.Caption = "US\nNew\nPrice";
      e.Layout.Bands[0].Columns["USSchedulePrice"].Width = 55;
      e.Layout.Bands[0].Columns["USSchedulePrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["USSchedulePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns["UKSchedulePrice"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USNewChange"].Header.Caption = "New\n+/-%";
      e.Layout.Bands[0].Columns["USNewChange"].Width = 55;
      e.Layout.Bands[0].Columns["USNewChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["USNewChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USNewChange"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USNewMakup"].Header.Caption = "US\nNew\nMakup";
      e.Layout.Bands[0].Columns["USNewMakup"].Width = 55;
      e.Layout.Bands[0].Columns["USNewMakup"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["USNewMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USNewMakup"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["USActive"].Header.Caption = "US\nActive";
      e.Layout.Bands[0].Columns["USActive"].Width = 55;
      e.Layout.Bands[0].Columns["USActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["USActive"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns["USActive"].CellAppearance.BackColor = Color.LightBlue;

      // DCI (Update 8-Jan-2014)
      e.Layout.Bands[0].Columns["DCIPrice5"].Header.Caption = "FacIn.\nLast 5th\nPrice";
      e.Layout.Bands[0].Columns["DCIPrice5"].Width = 55;
      e.Layout.Bands[0].Columns["DCIPrice5"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCIPrice5"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCIPrice5"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIDate5"].Header.Caption = "FacIn.\nLast 5th\nDate";
      e.Layout.Bands[0].Columns["DCIDate5"].Width = 70;
      e.Layout.Bands[0].Columns["DCIDate5"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIPrice4"].Header.Caption = "FacIn.\nLast 4th\nPrice";
      e.Layout.Bands[0].Columns["DCIPrice4"].Width = 55;
      e.Layout.Bands[0].Columns["DCIPrice4"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCIPrice4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCIPrice4"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIDate4"].Header.Caption = "FacIn.\nLast 4th\nDate";
      e.Layout.Bands[0].Columns["DCIDate4"].Width = 70;
      e.Layout.Bands[0].Columns["DCIDate4"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIPrice3"].Header.Caption = "FacIn.\nLast 3rd\nPrice";
      e.Layout.Bands[0].Columns["DCIPrice3"].Width = 55;
      e.Layout.Bands[0].Columns["DCIPrice3"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCIPrice3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCIPrice3"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIDate3"].Header.Caption = "FacIn.\nLast 3rd\nDate";
      e.Layout.Bands[0].Columns["DCIDate3"].Width = 70;
      e.Layout.Bands[0].Columns["DCIDate3"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIPrice2"].Header.Caption = "FacIn.\nLast 2nd\nPrice";
      e.Layout.Bands[0].Columns["DCIPrice2"].Width = 55;
      e.Layout.Bands[0].Columns["DCIPrice2"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCIPrice2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCIPrice2"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIDate2"].Header.Caption = "FacIn.\nLast 2nd\nDate";
      e.Layout.Bands[0].Columns["DCIDate2"].Width = 70;
      e.Layout.Bands[0].Columns["DCIDate2"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIPrice1"].Header.Caption = "FacIn.\nLatest\nPrice";
      e.Layout.Bands[0].Columns["DCIPrice1"].Width = 55;
      e.Layout.Bands[0].Columns["DCIPrice1"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCIPrice1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCIPrice1"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIDate1"].Header.Caption = "FacIn.\nLatest\nDate";
      e.Layout.Bands[0].Columns["DCIDate1"].Width = 70;
      e.Layout.Bands[0].Columns["DCIDate1"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIDefault"].Header.Caption = "FacIn.\nDef.\nPrice";
      e.Layout.Bands[0].Columns["DCIDefault"].Width = 55;
      e.Layout.Bands[0].Columns["DCIDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCIDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCIDefault"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIActualPrice"].Header.Caption = "FacIn.\nActual\nPrice";
      e.Layout.Bands[0].Columns["DCIActualPrice"].Width = 55;
      e.Layout.Bands[0].Columns["DCIActualPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCIActualPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCIActualPrice"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIChange"].Header.Caption = "+/-%";
      e.Layout.Bands[0].Columns["DCIChange"].Width = 55;
      e.Layout.Bands[0].Columns["DCIChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DCIChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCIChange"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIMakup"].Header.Caption = "FacIn.\nMakup";
      e.Layout.Bands[0].Columns["DCIMakup"].Width = 55;
      e.Layout.Bands[0].Columns["DCIMakup"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DCIMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCIMakup"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCISchedulePrice"].Header.Caption = "FacIn.\nNew\nPrice";
      e.Layout.Bands[0].Columns["DCISchedulePrice"].Width = 55;
      e.Layout.Bands[0].Columns["DCISchedulePrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCISchedulePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;      

      e.Layout.Bands[0].Columns["DCINewChange"].Header.Caption = "New\n+/-%";
      e.Layout.Bands[0].Columns["DCINewChange"].Width = 55;
      e.Layout.Bands[0].Columns["DCINewChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DCINewChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCINewChange"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCINewMakup"].Header.Caption = "FacIn.\nNew\nMakup";
      e.Layout.Bands[0].Columns["DCINewMakup"].Width = 55;
      e.Layout.Bands[0].Columns["DCINewMakup"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DCINewMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCINewMakup"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[0].Columns["DCIActive"].Header.Caption = "FacIn.\nActive";
      e.Layout.Bands[0].Columns["DCIActive"].Width = 55;
      e.Layout.Bands[0].Columns["DCIActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["DCIActive"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      // End DCI (Update 8-Jan-2014)

      // Factory
      e.Layout.Bands[0].Columns["DCPrice5"].Header.Caption = "Fac.\nLast 5th\nPrice";
      e.Layout.Bands[0].Columns["DCPrice5"].Width = 55;
      e.Layout.Bands[0].Columns["DCPrice5"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCPrice5"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCPrice5"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCDate5"].Header.Caption = "Fac.\nLast 5th\nDate";
      e.Layout.Bands[0].Columns["DCDate5"].Width = 70;
      e.Layout.Bands[0].Columns["DCDate5"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCPrice4"].Header.Caption = "Fac.\nLast 4th\nPrice";
      e.Layout.Bands[0].Columns["DCPrice4"].Width = 55;
      e.Layout.Bands[0].Columns["DCPrice4"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCPrice4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCPrice4"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCDate4"].Header.Caption = "Fac.\nLast 4th\nDate";
      e.Layout.Bands[0].Columns["DCDate4"].Width = 70;
      e.Layout.Bands[0].Columns["DCDate4"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCPrice3"].Header.Caption = "Fac.\nLast 3rd\nPrice";
      e.Layout.Bands[0].Columns["DCPrice3"].Width = 55;
      e.Layout.Bands[0].Columns["DCPrice3"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCPrice3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCPrice3"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCDate3"].Header.Caption = "Fac.\nLast 3rd\nDate";
      e.Layout.Bands[0].Columns["DCDate3"].Width = 70;
      e.Layout.Bands[0].Columns["DCDate3"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCPrice2"].Header.Caption = "Fac.\nLast 2nd\nPrice";
      e.Layout.Bands[0].Columns["DCPrice2"].Width = 55;
      e.Layout.Bands[0].Columns["DCPrice2"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCPrice2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCPrice2"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCDate2"].Header.Caption = "Fac.\nLast 2nd\nDate";
      e.Layout.Bands[0].Columns["DCDate2"].Width = 70;
      e.Layout.Bands[0].Columns["DCDate2"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCPrice1"].Header.Caption = "Fac.\nLatest\nPrice";
      e.Layout.Bands[0].Columns["DCPrice1"].Width = 55;
      e.Layout.Bands[0].Columns["DCPrice1"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCPrice1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCPrice1"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCDate1"].Header.Caption = "Fac.\nLatest\nDate";
      e.Layout.Bands[0].Columns["DCDate1"].Width = 70;
      e.Layout.Bands[0].Columns["DCDate1"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["CustomerName"].Header.Caption = "Customer\nName";
      e.Layout.Bands[0].Columns["CustomerName"].Width = 120;
      e.Layout.Bands[0].Columns["CustomerName"].CellAppearance.BackColor = Color.LightCyan;

      //OEM
      e.Layout.Bands[0].Columns["DISPrice5"].Header.Caption = "OEM\nLast 5th\nPrice";
      e.Layout.Bands[0].Columns["DISPrice5"].Width = 70;
      e.Layout.Bands[0].Columns["DISPrice5"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice5"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice5"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISDate5"].Header.Caption = "OEM\nLast 5th\nDate";
      e.Layout.Bands[0].Columns["DISDate5"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate5"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISDate5"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISPrice4"].Header.Caption = "OEM\nLast 4th\nPrice";
      e.Layout.Bands[0].Columns["DISPrice4"].Width = 70;
      e.Layout.Bands[0].Columns["DISPrice4"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice4"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISDate4"].Header.Caption = "OEM\nLast 4th\nDate";
      e.Layout.Bands[0].Columns["DISDate4"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate4"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISDate4"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISPrice3"].Header.Caption = "OEM\nLast 3rd\nPrice";
      e.Layout.Bands[0].Columns["DISPrice3"].Width = 70;
      e.Layout.Bands[0].Columns["DISPrice3"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice3"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISDate3"].Header.Caption = "OEM\nLast 3rd\nDate";
      e.Layout.Bands[0].Columns["DISDate3"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate3"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISDate3"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISPrice2"].Header.Caption = "OEM\nLast 2nd\nPrice";
      e.Layout.Bands[0].Columns["DISPrice2"].Width = 70;
      e.Layout.Bands[0].Columns["DISPrice2"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice2"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISDate2"].Header.Caption = "OEM\nLast 2nd\nDate";
      e.Layout.Bands[0].Columns["DISDate2"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate2"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISDate2"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISPrice1"].Header.Caption = "OEM\nLatest\nPrice";
      e.Layout.Bands[0].Columns["DISPrice1"].Width = 70;
      e.Layout.Bands[0].Columns["DISPrice1"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice1"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISDate1"].Header.Caption = "OEM\nLatest\nDate";
      e.Layout.Bands[0].Columns["DISDate1"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate1"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISDate1"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISDefault"].Header.Caption = "OEM\nDefault";
      e.Layout.Bands[0].Columns["DISDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISDefault"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISActualPrice"].Header.Caption = "OEM\nActual\nPrice";
      e.Layout.Bands[0].Columns["DISActualPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISActualPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISActualPrice"].CellAppearance.BackColor = Color.Wheat;

      //e.Layout.Bands[0].Columns["DISActualDate"].Header.Caption = "OEM\nActual\nDate";
      //e.Layout.Bands[0].Columns["DISActualDate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns["DISActualDate"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISChange"].Header.Caption = "OEM\nChange";
      e.Layout.Bands[0].Columns["DISChange"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISChange"].CellAppearance.BackColor = Color.Wheat;


      e.Layout.Bands[0].Columns["DISMakup"].Header.Caption = "OEM\nMakup";
      e.Layout.Bands[0].Columns["DISMakup"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISMakup"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISSchedulePrice"].Header.Caption = "OEM\nSchedule\nPrice";
      e.Layout.Bands[0].Columns["DISSchedulePrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISSchedulePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISSchedulePrice"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISNewChange"].Header.Caption = "OEM\nNew\nChange";
      e.Layout.Bands[0].Columns["DISNewChange"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISNewChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISNewChange"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISNewMakup"].Header.Caption = "OEM\nNew\nMakup";
      e.Layout.Bands[0].Columns["DISNewMakup"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISNewMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISNewMakup"].CellAppearance.BackColor = Color.Wheat;

      e.Layout.Bands[0].Columns["DISActive"].Header.Caption = "OEM\nActive";
      e.Layout.Bands[0].Columns["DISActive"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["DISActive"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISActive"].CellAppearance.BackColor = Color.Wheat;
      //Item Costing Capture      
      e.Layout.Bands[0].Columns["FrozenOriginalDCInhouseDefault"].Header.Caption = "Original\nStandard\nInhouse\nPrice";
      e.Layout.Bands[0].Columns["FrozenOriginalDCInhouseDefault"].Width = 60;
      e.Layout.Bands[0].Columns["FrozenOriginalDCInhouseDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["FrozenOriginalDCInhouseDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["FrozenOriginalDCInhouseDefault"].CellAppearance.BackColor = Color.LightCyan;
            
      e.Layout.Bands[0].Columns["FrozenOriginalDCSubconDefault"].Header.Caption = "Original\nStandard\nSubcon\nPrice";
      e.Layout.Bands[0].Columns["FrozenOriginalDCSubconDefault"].Width = 60;
      e.Layout.Bands[0].Columns["FrozenOriginalDCSubconDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["FrozenOriginalDCSubconDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["FrozenOriginalDCSubconDefault"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["FrozenLastDCInhouseDefault"].Header.Caption = "Last\nStandard\nInhouse\nPrice";
      e.Layout.Bands[0].Columns["FrozenLastDCInhouseDefault"].Width = 60;
      e.Layout.Bands[0].Columns["FrozenLastDCInhouseDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["FrozenLastDCInhouseDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["FrozenLastDCInhouseDefault"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["FrozenLastDCSubconDefault"].Header.Caption = "Last\nStandard\nSubcon\nPrice";
      e.Layout.Bands[0].Columns["FrozenLastDCSubconDefault"].Width = 60;
      e.Layout.Bands[0].Columns["FrozenLastDCSubconDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["FrozenLastDCSubconDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["FrozenLastDCSubconDefault"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["FrozenCurrentDCInhouseDefault"].Header.Caption = "Current\nStandard\nInhouse\nPrice";
      e.Layout.Bands[0].Columns["FrozenCurrentDCInhouseDefault"].Width = 60;
      e.Layout.Bands[0].Columns["FrozenCurrentDCInhouseDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["FrozenCurrentDCInhouseDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["FrozenCurrentDCInhouseDefault"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["FrozenCurrentDCSubconDefault"].Header.Caption = "Current\nStandard\nSubcon\nPrice";
      e.Layout.Bands[0].Columns["FrozenCurrentDCSubconDefault"].Width = 60;
      e.Layout.Bands[0].Columns["FrozenCurrentDCSubconDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["FrozenCurrentDCSubconDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["FrozenCurrentDCSubconDefault"].CellAppearance.BackColor = Color.LightCyan;

      int columnCount = ultTypeCompare.DisplayLayout.Bands[0].Columns.Count;
      if (columnCount < 3)
      {
        e.Layout.Bands[0].Columns["FrozenOriginalDCInhouseDefault"].Hidden = true;
        e.Layout.Bands[0].Columns["FrozenOriginalDCSubconDefault"].Hidden = true;
      }
      if (columnCount < 4)
      {
        e.Layout.Bands[0].Columns["FrozenCurrentDCInhouseDefault"].Hidden = true;
        e.Layout.Bands[0].Columns["FrozenCurrentDCSubconDefault"].Hidden = true;
      }
      if (columnCount < 5)
      {
        e.Layout.Bands[0].Columns["FrozenLastDCInhouseDefault"].Hidden = true;
        e.Layout.Bands[0].Columns["FrozenLastDCSubconDefault"].Hidden = true;
      }    
      //End Item Costing Capture

      //e.Layout.Bands[0].Columns["DCInhouseDefault"].Header.Caption = "Fac.\nInhouse\nPrice";
      e.Layout.Bands[0].Columns["DCInhouseDefault"].Header.Caption = "Actual\nInhouse\nPrice 1";
      e.Layout.Bands[0].Columns["DCInhouseDefault"].Width = 60;
      e.Layout.Bands[0].Columns["DCInhouseDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCInhouseDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCInhouseDefault"].CellAppearance.BackColor = Color.LightCyan;

      //e.Layout.Bands[0].Columns["DCSubconDefault"].Header.Caption = "Fac.\nSubcon\nPrice";
      e.Layout.Bands[0].Columns["DCSubconDefault"].Header.Caption = "Actual\nSubcon\nPrice 1";
      e.Layout.Bands[0].Columns["DCSubconDefault"].Width = 60;
      e.Layout.Bands[0].Columns["DCSubconDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCSubconDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCSubconDefault"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["ActualDCInhouseDefault"].Header.Caption = "Actual\nInhouse\nPrice";
      e.Layout.Bands[0].Columns["ActualDCInhouseDefault"].Width = 60;
      e.Layout.Bands[0].Columns["ActualDCInhouseDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["ActualDCInhouseDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ActualDCInhouseDefault"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["ActualDCSubconDefault"].Header.Caption = "Actual\nSubcon\nPrice";
      e.Layout.Bands[0].Columns["ActualDCSubconDefault"].Width = 60;
      e.Layout.Bands[0].Columns["ActualDCSubconDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["ActualDCSubconDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ActualDCSubconDefault"].CellAppearance.BackColor = Color.LightCyan;

      //e.Layout.Bands[0].Columns["DCActualPrice"].Header.Caption = "Fac.\nStandard\nCost";
      e.Layout.Bands[0].Columns["DCActualPrice"].Header.Caption = "Fac.\nSale\nPrice";
      e.Layout.Bands[0].Columns["DCActualPrice"].Width = 65;
      e.Layout.Bands[0].Columns["DCActualPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCActualPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCActualPrice"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCChange"].Header.Caption = "+/-%";
      e.Layout.Bands[0].Columns["DCChange"].Width = 55;
      e.Layout.Bands[0].Columns["DCChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DCChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCChange"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCSchedulePrice"].Header.Caption = "Fac.\nNew\nPrice";
      e.Layout.Bands[0].Columns["DCSchedulePrice"].Width = 55;
      e.Layout.Bands[0].Columns["DCSchedulePrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCSchedulePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns["DCSchedulePrice"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCNewChange"].Header.Caption = "New\n+/-%";
      e.Layout.Bands[0].Columns["DCNewChange"].Width = 55;
      e.Layout.Bands[0].Columns["DCNewChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DCNewChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCNewChange"].CellAppearance.BackColor = Color.LightCyan;

      // UK
      e.Layout.Bands[0].Columns["UKPrice5"].Header.Caption = "UK\nLast 5th\nPrice";
      e.Layout.Bands[0].Columns["UKPrice5"].Width = 55;
      e.Layout.Bands[0].Columns["UKPrice5"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["UKPrice5"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKPrice5"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKDate5"].Header.Caption = "UK\nLast 5th\nDate";
      e.Layout.Bands[0].Columns["UKDate5"].Width = 70;
      e.Layout.Bands[0].Columns["UKDate5"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKPrice4"].Header.Caption = "UK\nLast 4th\nPrice";
      e.Layout.Bands[0].Columns["UKPrice4"].Width = 55;
      e.Layout.Bands[0].Columns["UKPrice4"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["UKPrice4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKPrice4"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKDate4"].Header.Caption = "UK\nLast 4th\nDate";
      e.Layout.Bands[0].Columns["UKDate4"].Width = 70;
      e.Layout.Bands[0].Columns["UKDate4"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKPrice3"].Header.Caption = "UK\nLast 3rd\nPrice";
      e.Layout.Bands[0].Columns["UKPrice3"].Width = 55;
      e.Layout.Bands[0].Columns["UKPrice3"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["UKPrice3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKPrice3"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKDate3"].Header.Caption = "UK\nLast 3rd\nDate";
      e.Layout.Bands[0].Columns["UKDate3"].Width = 70;
      e.Layout.Bands[0].Columns["UKDate3"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKPrice2"].Header.Caption = "UK\nLast 2nd\nPrice";
      e.Layout.Bands[0].Columns["UKPrice2"].Width = 55;
      e.Layout.Bands[0].Columns["UKPrice2"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["UKPrice2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKPrice2"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKDate2"].Header.Caption = "UK\nLast 2nd\nDate";
      e.Layout.Bands[0].Columns["UKDate2"].Width = 70;
      e.Layout.Bands[0].Columns["UKDate2"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKPrice1"].Header.Caption = "UK\nLatest\nPrice";
      e.Layout.Bands[0].Columns["UKPrice1"].Width = 55;
      e.Layout.Bands[0].Columns["UKPrice1"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["UKPrice1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKPrice1"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKDate1"].Header.Caption = "UK\nLatest\nDate";
      e.Layout.Bands[0].Columns["UKDate1"].Width = 70;
      e.Layout.Bands[0].Columns["UKDate1"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKDefault"].Header.Caption = "UK\nDef.\nPrice";
      e.Layout.Bands[0].Columns["UKDefault"].Width = 55;
      e.Layout.Bands[0].Columns["UKDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["UKDefault"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKActualPrice"].Header.Caption = "UK\nActual\nPrice";
      e.Layout.Bands[0].Columns["UKActualPrice"].Width = 55;
      e.Layout.Bands[0].Columns["UKActualPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["UKActualPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKActualPrice"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKChange"].Header.Caption = "+/-%";
      e.Layout.Bands[0].Columns["UKChange"].Width = 55;
      e.Layout.Bands[0].Columns["UKChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["UKChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKChange"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKMakup"].Header.Caption = "UK\nMakup";
      e.Layout.Bands[0].Columns["UKMakup"].Width = 55;
      e.Layout.Bands[0].Columns["UKMakup"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["UKMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKMakup"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKSchedulePrice"].Header.Caption = "UK\nNew\nPrice";
      e.Layout.Bands[0].Columns["UKSchedulePrice"].Width = 55;
      e.Layout.Bands[0].Columns["UKSchedulePrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["UKSchedulePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns["UKSchedulePrice"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKNewChange"].Header.Caption = "New\n+/-%";
      e.Layout.Bands[0].Columns["UKNewChange"].Width = 55;
      e.Layout.Bands[0].Columns["UKNewChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["UKNewChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKNewChange"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKNewMakup"].Header.Caption = "UK\nNew\nMakup";
      e.Layout.Bands[0].Columns["UKNewMakup"].Width = 55;
      e.Layout.Bands[0].Columns["UKNewMakup"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["UKNewMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UKNewMakup"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[0].Columns["UKActive"].Header.Caption = "UK\nActive";
      e.Layout.Bands[0].Columns["UKActive"].Width = 55;
      e.Layout.Bands[0].Columns["UKActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["UKActive"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns["UKActive"].CellAppearance.BackColor = Color.LightSkyBlue;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      // Change color if BOM not confirm
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        int bomConfirm = DBConvert.ParseInt(row.Cells["BOMConfirm"].Value);
        int defaultPrice = DBConvert.ParseInt(row.Cells["ContractOut"].Value);
        if (bomConfirm == 0)
        {
          row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
          row.Cells["SaleCode"].Appearance.BackColor = Color.Yellow;
          row.Cells["Unit"].Appearance.BackColor = Color.Yellow;
          row.Cells["Name"].Appearance.BackColor = Color.Yellow;
        }
        string columnDefault = (defaultPrice == 0 ? "DCInhouseDefault" : "DCSubconDefault");        
        row.Cells[columnDefault].Appearance.ForeColor = Color.Blue;

        columnDefault = (defaultPrice == 0 ? "ActualDCInhouseDefault" : "ActualDCSubconDefault");
        row.Cells[columnDefault].Appearance.ForeColor = Color.Blue;

        //Capture History
        defaultPrice = DBConvert.ParseInt(row.Cells["OriginalContractOut"].Value);
        columnDefault = (defaultPrice == 0 ? "FrozenOriginalDCInhouseDefault" : "FrozenOriginalDCSubconDefault");
        row.Cells[columnDefault].Appearance.ForeColor = Color.Blue;

        defaultPrice = DBConvert.ParseInt(row.Cells["LastContractOut"].Value);
        columnDefault = (defaultPrice == 0 ? "FrozenLastDCInhouseDefault" : "FrozenLastDCSubconDefault");
        row.Cells[columnDefault].Appearance.ForeColor = Color.Blue;

        defaultPrice = DBConvert.ParseInt(row.Cells["CurrentContractOut"].Value);
        columnDefault = (defaultPrice == 0 ? "FrozenCurrentDCInhouseDefault" : "FrozenCurrentDCSubconDefault");
        row.Cells[columnDefault].Appearance.ForeColor = Color.Blue;
        //Capture History
      }
    }

    /// <summary>
    /// Check valid price before update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      bool isChangeUS = (string.Compare(columnName, "USSchedulePrice", true) == 0);      
      bool isChangeFactory = (string.Compare(columnName, "DCSchedulePrice", true) == 0);
      bool isChangeUK = (string.Compare(columnName, "UKSchedulePrice", true) == 0);
      bool isChangeDCI = (string.Compare(columnName, "DCISchedulePrice", true) == 0);
      if (isChangeUS || isChangeUK || isChangeFactory || isChangeDCI)
      {
        string text = e.Cell.Text.ToString().Trim();
        if (text.Length > 0)
        {
          double value = DBConvert.ParseDouble(text);
          string title = (isChangeUS) ? "US New Price" : (isChangeFactory) ? "Factory New Price" : (isChangeUK) ? "UK New Price" : "FacIn. New Price";
          if (value <= 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", title);
            e.Cancel = true;
          }
        }        
      }
    }

    /// <summary>
    /// Calculate makup after change price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      string value = e.Cell.Value.ToString();
      UltraGridRow row = e.Cell.Row;
      if (string.Compare(columnName, "Rowstate [hidden]", true) != 0) {
        row.Cells["Rowstate"].Value = 1;
      }
      
      double dcSchedulePrice = DBConvert.ParseDouble(row.Cells["DCSchedulePrice"].Value.ToString());
      double usSchedulePrice = DBConvert.ParseDouble(row.Cells["USSchedulePrice"].Value.ToString());
      double ukSchedulePrice = DBConvert.ParseDouble(row.Cells["UKSchedulePrice"].Value.ToString());
      double dciSchedulePrice = DBConvert.ParseDouble(row.Cells["DCISchedulePrice"].Value.ToString());
      
      if (string.Compare(columnName, "USSchedulePrice", true) == 0) {
        double usActivePrice = DBConvert.ParseDouble(row.Cells["USActualPrice"].Value.ToString());

        if (usActivePrice != double.MinValue && usActivePrice != 0 && usSchedulePrice != double.MinValue)
        {
          row.Cells["USNewChange"].Value = (usSchedulePrice - usActivePrice) / usActivePrice;
        }
        else {
          row.Cells["USNewChange"].Value = DBNull.Value;
        }

        if (dcSchedulePrice != double.MinValue && dcSchedulePrice != 0 && usSchedulePrice != double.MinValue) {
          row.Cells["USNewMakup"].Value = usSchedulePrice / dcSchedulePrice;
        }
        else
        {
          row.Cells["USNewMakup"].Value = DBNull.Value;
        }
      }
      else if (string.Compare(columnName, "DCSchedulePrice", true) == 0) {
        double dcActivePrice = DBConvert.ParseDouble(row.Cells["DCActualPrice"].Value.ToString());
        if (dcActivePrice != double.MinValue && dcActivePrice != 0 && dcSchedulePrice != double.MinValue)
        {
          row.Cells["DCNewChange"].Value = (dcSchedulePrice - dcActivePrice) / dcActivePrice;
        }
        else {
          row.Cells["DCNewChange"].Value = DBNull.Value;
        }

        if (dcSchedulePrice != double.MinValue && dcSchedulePrice != 0 && usSchedulePrice != double.MinValue)
        {
          row.Cells["USNewMakup"].Value = usSchedulePrice / dcSchedulePrice;
        }
        else
        {
          row.Cells["USNewMakup"].Value = DBNull.Value;
        }
        if (dcSchedulePrice != double.MinValue && dcSchedulePrice != 0 && ukSchedulePrice != double.MinValue && this.ex_USD_GBP != double.MinValue && this.ex_USD_GBP != 0)
        {
          row.Cells["UKNewMakup"].Value = ukSchedulePrice / (dcSchedulePrice * this.ex_USD_GBP);
        }
        else
        {
          row.Cells["UKNewMakup"].Value = DBNull.Value;
        }
      }
      else if (string.Compare(columnName, "UKSchedulePrice", true) == 0) {
        double ukActivePrice = DBConvert.ParseDouble(row.Cells["UKActualPrice"].Value.ToString());
        if (ukActivePrice != double.MinValue && ukActivePrice != 0 && ukSchedulePrice != double.MinValue)
        {
          row.Cells["UKNewChange"].Value = (ukSchedulePrice - ukActivePrice) / ukActivePrice;
        }
        else
        {
          row.Cells["UKNewChange"].Value = DBNull.Value;
        }

        if (dcSchedulePrice != double.MinValue && dcSchedulePrice != 0 && ukSchedulePrice != double.MinValue && this.ex_USD_GBP != double.MinValue && this.ex_USD_GBP != 0)
        {
          row.Cells["UKNewMakup"].Value = ukSchedulePrice / (dcSchedulePrice * this.ex_USD_GBP);
        }
        else
        {
          row.Cells["UKNewMakup"].Value = DBNull.Value;
        }
      }
    }

    /// <summary>
    /// Out report factory cost
    /// 1 : Original, 2: Last, 3: Current, 4: Standard, 5: Actual
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        string itemCode = ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString();
        if (rdViewCosting.Checked)
        {
          // Check
          if (!this.CheckWhenDoubleClick())
          {
            WindowUtinity.ShowMessageErrorFromText("Please choose 2 colunms only for comparing");
            return;
          }

          int viewType = -1;
          if (rdMakeLocal.Checked)
          {
            viewType = 0;
          }
          else if (rdContractOut.Checked)
          {
            viewType = 1;
          }
          int isDifference = (chkDifference.Checked ? 1 : 0);
          // View standard , Actual
          if (ultTypeCompare.Rows[0].Cells["Standard"].Value.ToString() == "1" && ultTypeCompare.Rows[0].Cells["Actual"].Value.ToString() == "1")
          {
            if (viewType >= 0)
            {
              FunctionUtility.ViewItemCostingByStandardAndActual(itemCode, isDifference, viewType);
            }
            else
            {
              FunctionUtility.ViewItemCostingByStandardAndActual(itemCode, isDifference);
            }
          }
          else // View Capture Price
          {
            int compare1 = 0;
            int compare2 = 0;
            if (ultTypeCompare.DisplayLayout.Bands[0].Columns.Count >= 3)
            {
              if (DBConvert.ParseInt(ultTypeCompare.Rows[0].Cells["Original"].Value.ToString()) == 1)
              {
                compare1 = 1;
              }
            }

            if (ultTypeCompare.DisplayLayout.Bands[0].Columns.Count >= 5 )
            {
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
            }

            if (ultTypeCompare.DisplayLayout.Bands[0].Columns.Count >= 4 )
            {
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
            FunctionUtility.ViewItemCostingCompare(itemCode, isDifference, compare1, compare2, viewType);
          }
        }
        else
        {
          //viewBOM_01_002 objItemMaster = new viewBOM_01_002();
          //objItemMaster.itemCode = itemCode;          
          //DataTable dtRevision =  DataBaseAccess.SearchCommandTextDataTable(string.Format("Select RevisionActive From TblBOMItemBasic Where ItemCode = '{0}'", itemCode));
          //if (dtRevision != null && dtRevision.Rows.Count > 0)
          //{
          //  objItemMaster.revision = DBConvert.ParseInt(dtRevision.Rows[0][0].ToString());
          //  Shared.Utility.WindowUtinity.ShowView(objItemMaster, "ITEM MASTER LEVEL 1ST", false, ViewState.Window, FormWindowState.Maximized);
          //}          
        }
      }
      catch { }
    }

    /// <summary>
    /// Innit layout ultra grid Show Column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultShowColumn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultShowColumn);
      e.Layout.Bands[0].Columns["Customer"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Customer"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Customer"].MinWidth = 70;
      e.Layout.Bands[0].Columns["ALL"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Price5"].Header.Caption = "Last 5th\nPrice";
      e.Layout.Bands[0].Columns["Price5"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Date5"].Header.Caption = "Last 5th\nDate";
      e.Layout.Bands[0].Columns["Date5"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Price4"].Header.Caption = "Last 4th\nPrice";
      e.Layout.Bands[0].Columns["Price4"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Date4"].Header.Caption = "Last 4th\nDate";
      e.Layout.Bands[0].Columns["Date4"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Price3"].Header.Caption = "Last 3rd\nPrice";
      e.Layout.Bands[0].Columns["Price3"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Date3"].Header.Caption = "Last 3rd\nDate";
      e.Layout.Bands[0].Columns["Date3"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Price2"].Header.Caption = "Last 2nd\nPrice";
      e.Layout.Bands[0].Columns["Price2"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Date2"].Header.Caption = "Last 2nd\nDate";
      e.Layout.Bands[0].Columns["Date2"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Price1"].Header.Caption = "Latest\nPrice";
      e.Layout.Bands[0].Columns["Price1"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Date1"].Header.Caption = "Latest\nDate";
      e.Layout.Bands[0].Columns["Date1"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Default"].Header.Caption = "Def.\nPrice";
      e.Layout.Bands[0].Columns["Default"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["ActualPrice"].Header.Caption = "Actual\nPrice";
      e.Layout.Bands[0].Columns["ActualPrice"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Change"].Header.Caption = "+/-%";
      e.Layout.Bands[0].Columns["Change"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Makup"].Header.Caption = "Makup";
      e.Layout.Bands[0].Columns["Makup"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["SchedulePrice"].Header.Caption = "New\nPrice";
      e.Layout.Bands[0].Columns["SchedulePrice"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["NewChange"].Header.Caption = "New\n+/-%";
      e.Layout.Bands[0].Columns["NewChange"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["NewMakup"].Header.Caption = "New\nMakup";
      e.Layout.Bands[0].Columns["NewMakup"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Active"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Show or hiden the column of ultragird data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultShowColumn_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      string prefix = (row.Index == 0) ? "US" : (row.Index == 1) ? "DC" : (row.Index == 2) ? "UK" : (row.Index == 3) ? "DCI" : "DIS";
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly)
        {
          if ((string.Compare(columnName, "Default", true) == 0) && (row.Index == 1))
          {
            ultData.DisplayLayout.Bands[0].Columns[string.Format("{0}Inhouse{1}", prefix, columnName)].Hidden = e.Cell.Text.ToString().Equals("0");
            ultData.DisplayLayout.Bands[0].Columns[string.Format("{0}Subcon{1}", prefix, columnName)].Hidden = e.Cell.Text.ToString().Equals("0");
          }
          else
          {
            ultData.DisplayLayout.Bands[0].Columns[string.Format("{0}{1}", prefix, columnName)].Hidden = e.Cell.Text.ToString().Equals("0");
          }
        }
      }
      else {
        for (int i = 2; i < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; i++) {
          row.Cells[i].Value = e.Cell.Text;
          if (row.Index == 1)
          {
            row.Cells["Makup"].Value = 0;
            row.Cells["NewMakup"].Value = 0;
            row.Cells["Active"].Value = 0;            
          }
        }
        this.ChkAll_CheckedChange(row.Index);
      }
    }

    private void ultTypeCompare_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Standard"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Actual"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      int columnCount = e.Layout.Bands[0].Columns.Count;
      if (columnCount >= 3)
      {
        e.Layout.Bands[0].Columns["Original"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      if (columnCount >= 4)
      {
        e.Layout.Bands[0].Columns["Current"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      if (columnCount >= 5)
      {
        e.Layout.Bands[0].Columns["Last"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }    

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void chkShowComparePrice_CheckedChanged(object sender, EventArgs e)
    {
      tableLayoutPriceCompare.Visible = chkShowComparePrice.Checked;
    }

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
    #endregion Grid Events

    #region Button Click Events

    /// <summary>
    /// Load information item price from screen's condition
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.LoadData();
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Focus on itemcode (sale code) like txtItemCode.Text
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFocus_Click(object sender, EventArgs e)
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
          ultData.ActiveRowScrollRegion.FirstRow = ultData.Rows[i];
          return;
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageInfomation("MSG0038", new string[] { findCode });
    }

    /// <summary>
    /// Open from import item price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      viewCSD_04_010 view = new viewCSD_04_010();
      view.isFactory = true;
      WindowUtinity.ShowView(view, "IMPORT FACTORY SALE PRICE", false, ViewState.MainWindow);
    }

    /// <summary>
    /// Save Data : 1. Insert/Update TblCSDItemSalePrice
    ///             2. Insert TblCSDItemSalePriceHistory
    /// </summary>
    private void btnSave_Click(object sender, EventArgs e)
    {
      btnSave.Enabled = false;
      bool success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
      btnSave.Enabled = true;
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
    /// Save data and confirm factory price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnConfirmPriceFactory_Click(object sender, EventArgs e)
    {
      btnConfirmPriceFactory.Enabled = false;
      bool saveSuccess = this.SaveData();
      bool confirmSuccess = true;
      int count = ultData.Rows.Count;
      int outputValue = 0;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        double actual = DBConvert.ParseDouble(row.Cells["DCActualPrice"].Value.ToString());
        double schedule = DBConvert.ParseDouble(row.Cells["DCSchedulePrice"].Value.ToString());
        if (schedule != double.MinValue && actual != schedule) {
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          DBParameter[] input = new DBParameter[3];
          input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          input[1] = new DBParameter("@CustomerPid", DbType.Int64, FACTORY_PID);
          input[2] = new DBParameter("@ConfirmBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemSalePrice_ConfirmPrice", input, output);
          outputValue = DBConvert.ParseInt(output[0].Value.ToString());
          if (outputValue <= 0)
          {
            confirmSuccess = false;
          }
        }
      }
      if (saveSuccess && confirmSuccess)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
      btnConfirmPriceFactory.Enabled = true;
    }

    /// <summary>
    /// Save data and confirm US price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnConfirmPriceUS_Click(object sender, EventArgs e)
    {
      btnConfirmPriceFactory.Enabled = false;
      bool saveSuccess = this.SaveData();
      bool confirmSuccess = true;
      int count = ultData.Rows.Count;
      int outputValue = 0;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        double actual = DBConvert.ParseDouble(row.Cells["USActualPrice"].Value.ToString());
        double schedule = DBConvert.ParseDouble(row.Cells["USSchedulePrice"].Value.ToString());
        if (schedule != double.MinValue && actual != schedule)
        {
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          DBParameter[] input = new DBParameter[3];
          input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          input[1] = new DBParameter("@CustomerPid", DbType.Int64, US_PID);
          input[2] = new DBParameter("@ConfirmBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemSalePrice_ConfirmPrice", input, output);
          outputValue = DBConvert.ParseInt(output[0].Value.ToString());
          if (outputValue <= 0)
          {
            confirmSuccess = false;
          }
        }
      }
      if (saveSuccess && confirmSuccess)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
      btnConfirmPriceFactory.Enabled = true;
    }

    /// <summary>
    /// Save data and confirm UK price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnConfirmPriceUK_Click(object sender, EventArgs e)
    {
      btnConfirmPriceFactory.Enabled = false;
      bool saveSuccess = this.SaveData();
      bool confirmSuccess = true;
      int count = ultData.Rows.Count;
      int outputValue = 0;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        double actual = DBConvert.ParseDouble(row.Cells["UKActualPrice"].Value.ToString());
        double schedule = DBConvert.ParseDouble(row.Cells["UKSchedulePrice"].Value.ToString());
        if (schedule != double.MinValue && actual != schedule)
        {
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          DBParameter[] input = new DBParameter[3];
          input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          input[1] = new DBParameter("@CustomerPid", DbType.Int64, UK_PID);
          input[2] = new DBParameter("@ConfirmBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemSalePrice_ConfirmPrice", input, output);
          outputValue = DBConvert.ParseInt(output[0].Value.ToString());
          if (outputValue <= 0)
          {
            confirmSuccess = false;
          }
        }
      }
      if (saveSuccess && confirmSuccess)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
      btnConfirmPriceFactory.Enabled = true;
    }

    /// <summary>
    /// Save data and confirm Factory International price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnConfirmPriceFacIn_Click(object sender, EventArgs e)
    {
      btnConfirmPriceFacIn.Enabled = false;
      bool saveSuccess = this.SaveData();
      bool confirmSuccess = true;
      int count = ultData.Rows.Count;
      int outputValue = 0;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        double actual = DBConvert.ParseDouble(row.Cells["DCIActualPrice"].Value.ToString());
        double schedule = DBConvert.ParseDouble(row.Cells["DCISchedulePrice"].Value.ToString());
        if (schedule != double.MinValue && actual != schedule)
        {
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          DBParameter[] input = new DBParameter[3];
          input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          input[1] = new DBParameter("@CustomerPid", DbType.Int64, DCI_PID);
          input[2] = new DBParameter("@ConfirmBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemSalePrice_ConfirmPrice", input, output);
          outputValue = DBConvert.ParseInt(output[0].Value.ToString());
          if (outputValue <= 0)
          {
            confirmSuccess = false;
          }
        }
      }
      if (saveSuccess && confirmSuccess)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
      btnConfirmPriceFacIn.Enabled = true;
    }

    private void btnExportToExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "JCPriceList");
    }
    #endregion Button Click Events

    #region Textbox KeyDown Events

    /// <summary>
    /// Load information item price from screen's condition
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.LoadData();
      }
    }

    private void chkShowColumns_CheckedChanged(object sender, EventArgs e)
    {
      ultShowColumn.Visible = chkShowColumns.Checked;
    }  
    #endregion Textbox KeyDown Events 

    private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
    {

    }

    private void ultraCBCustomer_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      
    }

    private void ultraCBCustomer_InitializeLayout_1(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Display"].Hidden = true;
      e.Layout.Bands[0].ColHeadersVisible = false;
      //e.Layout.Bands[0].Columns["Code"].MinWidth = 200;
      //e.Layout.Bands[0].Columns["Code"].MaxWidth = 200;
    }
  }
}
