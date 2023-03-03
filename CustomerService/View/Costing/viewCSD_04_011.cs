/*
  Author        : Võ Hoa Lư
  Create date   : 16/06/2011
  Decription    : Show/update the item's price by customer  
  Truong Update : Export To Excel
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
using VBReport;
using DaiCo.Technical;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_011 : MainUserControl
  {
    #region Fields
    public long customerPid = long.MinValue;
    public string customerCode = string.Empty;
    private double factoryOverHead = double.MinValue;
    private double contractOutFOH = double.MinValue;
    private double profit = double.MinValue;
    private double makup = double.MinValue;
    private double discount = double.MinValue;
    private double margin = double.MinValue;
    private double ex_USD_VND = double.MinValue;
    private const long FACTORY_PID = 1;
    private bool flg = false;
    #endregion Fields

    #region Init Form
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_04_011()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    private void viewCSD_04_011_Load(object sender, EventArgs e)
    {      
      viewCSD_04_013 view = new viewCSD_04_013();
      view.parent = this;
      WindowUtinity.ShowView(view, "CHOOSE DISTRIBUTE", false, ViewState.ModalWindow);
      if (this.customerPid == long.MinValue) {
        this.CloseTab();
        return;
      }
      this.LoadTypeCompare();

      this.flg = false;
      btnSearch.Enabled = false;
      this.LoadUltraData();
      btnSearch.Enabled = true;
    }
    #endregion Init Form

    #region Method Data
    /// <summary>
    /// Load Choose Compate
    /// </summary>
    private void LoadTypeCompare()
    {
      DataTable dtTypeCompate = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemCostPriceFrozenType");
      ultTypeCompare.DataSource = dtTypeCompate;
    }

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
    /// Load grid checkbox show column
    /// </summary>
    private void LoadGridShowColumn()
    {
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

      dataSource.Columns.Add("ActualDate", typeof(int));
      dataSource.Columns["ActualDate"].DefaultValue = 1;

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
      row["Customer"] = this.customerCode;
      dataSource.Rows.Add(row);

      row = dataSource.NewRow();
      row["Customer"] = "Factory";
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

      this.ChkAll_CheckedChange(0);
      this.ChkAll_CheckedChange(1);      
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange(int rowIndex)
    {
      string prefix = (rowIndex == 0) ? "DIS" : "DC";
      for (int colIndex = 2; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
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
          //ultData.DisplayLayout.Bands[0].Columns[prefix + cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// Load information item price from screen's condition
    /// </summary>
    private void LoadUltraData()
    {
      groupBoxInformation.Text = string.Format("Information of {0}", this.customerCode);
      string code = txtItemCode.Text.Trim();
      DBParameter[] input = new DBParameter[2];
      if (code.Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, string.Format("%{0}%", code));
      }
      input[1] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);
      DBParameter[] output = new DBParameter[6];
      output[0] = new DBParameter("@FOH", DbType.Double, 0);
      output[1] = new DBParameter("@Profit", DbType.Double, 0);
      output[2] = new DBParameter("@Makup", DbType.Double, 0);
      output[3] = new DBParameter("@Discount", DbType.Double, 0);
      output[4] = new DBParameter("@Ex_USD_VND", DbType.Double, 0);
      output[5] = new DBParameter("@ContractOutFOH", DbType.Double, 0);

      DataTable dataSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemSalePrice_SelectByDistribute", 1800, input, output);
      ultData.DataSource = dataSource;
      ultData.DisplayLayout.Bands[0].ColHeaderLines = 3;
      int columnCount = ultData.DisplayLayout.Bands[0].Columns.Count;
      for (int i = 0; i < columnCount; i++)
      {
        ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      ultData.DisplayLayout.Bands[0].Columns["DISSchedulePrice"].CellActivation = Activation.AllowEdit;
      ultData.DisplayLayout.Bands[0].Columns["MOQ"].CellActivation = Activation.AllowEdit;
      ultData.DisplayLayout.Bands[0].Columns["DISActive"].CellActivation = Activation.AllowEdit;
      ultData.DisplayLayout.Bands[0].Columns["DCSchedulePrice"].CellActivation = Activation.AllowEdit;
      
      // FOH
      this.factoryOverHead = DBConvert.ParseDouble(output[0].Value.ToString());
      txtFOH.Text = string.Format("{0}%", FunctionUtility.NumericFormat(this.factoryOverHead * 100, 2));

      // Contract Out FOH
      this.contractOutFOH = DBConvert.ParseDouble(output[5].Value.ToString());
      txtContractOutFOH.Text = string.Format("{0}%", FunctionUtility.NumericFormat(this.contractOutFOH * 100, 2));

      // Profit
      this.profit = DBConvert.ParseDouble(output[1].Value.ToString());
      txtProfit.Text = string.Format("{0}%", FunctionUtility.NumericFormat(this.profit * 100, 2));

      // Makup
      this.makup = DBConvert.ParseDouble(output[2].Value.ToString());
      txtMakup.Text = string.Format("{0}", FunctionUtility.NumericFormat(this.makup, 2));

      // Discount
      this.discount = DBConvert.ParseDouble(output[3].Value.ToString());
      txtDiscount.Text = string.Format("{0}", FunctionUtility.NumericFormat(this.discount, 2));

      // USD/GBP
      this.margin = this.makup * (1 - this.discount);
      txtMargin.Text = string.Format("{0}", FunctionUtility.NumericFormat(this.margin, 2));

      // USD/VND
      this.ex_USD_VND = DBConvert.ParseDouble(output[4].Value.ToString());
      txtUSD_VND.Text = string.Format("{0}", FunctionUtility.NumericFormat(this.ex_USD_VND, 2));
      if (!this.flg) {
        this.flg = true;
        this.LoadGridShowColumn();
      }
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
          DBParameter[] input = new DBParameter[7];

          string itemCode = row.Cells["ItemCode"].Value.ToString();
          input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          input[1] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);

          double newPrice = DBConvert.ParseDouble(row.Cells["DISSchedulePrice"].Value.ToString());
          if (newPrice != double.MinValue)
          {
            input[2] = new DBParameter("@DISSchedulePrice", DbType.Double, newPrice);
          }

          newPrice = DBConvert.ParseDouble(row.Cells["DCSchedulePrice"].Value.ToString());
          if (newPrice != double.MinValue)
          {
            input[3] = new DBParameter("@FactorySchedulePrice", DbType.Double, newPrice);
          }

          int active = DBConvert.ParseInt(row.Cells["DISActive"].Value.ToString());
          input[4] = new DBParameter("@DISActive", DbType.Int32, active);
          input[5] = new DBParameter("@AdjustBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          int moq = DBConvert.ParseInt(row.Cells["MOQ"].Value.ToString());
          if (moq > 0)
          {
            input[6] = new DBParameter("@MOQ", DbType.Int32, moq);
          }

          DataBaseAccess.ExecuteStoreProcedure("spCSDItemSalePrice_EditSchedulePrice_ByDistibute", input, output);
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
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
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

      // DISTRIBUTE
      e.Layout.Bands[0].Columns["DISPrice5"].Header.Caption = "Last 5th\nPrice";
      e.Layout.Bands[0].Columns["DISPrice5"].Width = 55;
      e.Layout.Bands[0].Columns["DISPrice5"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice5"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice5"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISDate5"].Header.Caption = "Last 5th\nDate";
      e.Layout.Bands[0].Columns["DISDate5"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate5"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISPrice4"].Header.Caption = "Last 4th\nPrice";
      e.Layout.Bands[0].Columns["DISPrice4"].Width = 55;
      e.Layout.Bands[0].Columns["DISPrice4"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice4"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISDate4"].Header.Caption = "Last 4th\nDate";
      e.Layout.Bands[0].Columns["DISDate4"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate4"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISPrice3"].Header.Caption = "Last 3rd\nPrice";
      e.Layout.Bands[0].Columns["DISPrice3"].Width = 55;
      e.Layout.Bands[0].Columns["DISPrice3"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice3"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISDate3"].Header.Caption = "Last 3rd\nDate";
      e.Layout.Bands[0].Columns["DISDate3"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate3"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISPrice2"].Header.Caption = "Last 2nd\nPrice";
      e.Layout.Bands[0].Columns["DISPrice2"].Width = 55;
      e.Layout.Bands[0].Columns["DISPrice2"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice2"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISDate2"].Header.Caption = "Last 2nd\nDate";
      e.Layout.Bands[0].Columns["DISDate2"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate2"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISPrice1"].Header.Caption = "Latest\nPrice";
      e.Layout.Bands[0].Columns["DISPrice1"].Width = 55;
      e.Layout.Bands[0].Columns["DISPrice1"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISPrice1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISPrice1"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISDate1"].Header.Caption = "Latest\nDate";
      e.Layout.Bands[0].Columns["DISDate1"].Width = 70;
      e.Layout.Bands[0].Columns["DISDate1"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISDefault"].Header.Caption = "Def.\nPrice";
      e.Layout.Bands[0].Columns["DISDefault"].Width = 55;
      e.Layout.Bands[0].Columns["DISDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISDefault"].CellAppearance.BackColor = Color.LightBlue;

      //e.Layout.Bands[0].Columns["DISActualPrice"].Header.Caption = "Actual\nPrice";
      e.Layout.Bands[0].Columns["DISActualPrice"].Header.Caption = "Sale\nPrice";
      e.Layout.Bands[0].Columns["DISActualPrice"].Width = 55;
      e.Layout.Bands[0].Columns["DISActualPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISActualPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISActualPrice"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["MOQ"].Width = 55;
      e.Layout.Bands[0].Columns["MOQ"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      
      //e.Layout.Bands[0].Columns["DISActualDate"].Header.Caption = "Actual\nDate";
      e.Layout.Bands[0].Columns["DISActualDate"].Header.Caption = "Sale\nDate";
      e.Layout.Bands[0].Columns["DISActualDate"].Width = 70;
      e.Layout.Bands[0].Columns["DISActualDate"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISChange"].Header.Caption = "+/-%";
      e.Layout.Bands[0].Columns["DISChange"].Width = 55;
      e.Layout.Bands[0].Columns["DISChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DISChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISChange"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISMakup"].Header.Caption = "Makup";
      e.Layout.Bands[0].Columns["DISMakup"].Width = 55;
      e.Layout.Bands[0].Columns["DISMakup"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DISMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISMakup"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISSchedulePrice"].Header.Caption = "New\nPrice";
      e.Layout.Bands[0].Columns["DISSchedulePrice"].Width = 55;
      e.Layout.Bands[0].Columns["DISSchedulePrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DISSchedulePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      
      e.Layout.Bands[0].Columns["DISNewChange"].Header.Caption = "New\n+/-%";
      e.Layout.Bands[0].Columns["DISNewChange"].Width = 55;
      e.Layout.Bands[0].Columns["DISNewChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DISNewChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISNewChange"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISNewMakup"].Header.Caption = "New\nMakup";
      e.Layout.Bands[0].Columns["DISNewMakup"].Width = 55;
      e.Layout.Bands[0].Columns["DISNewMakup"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DISNewMakup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DISNewMakup"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["DISActive"].Header.Caption = "Active";
      e.Layout.Bands[0].Columns["DISActive"].Width = 55;
      e.Layout.Bands[0].Columns["DISActive"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["DISActive"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

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

      e.Layout.Bands[0].Columns["DCInhouseDefault"].Header.Caption = "Standard\nInhouse\nPrice";
      e.Layout.Bands[0].Columns["DCInhouseDefault"].Width = 55;
      e.Layout.Bands[0].Columns["DCInhouseDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCInhouseDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCInhouseDefault"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCSubconDefault"].Header.Caption = "Standard\nSubCon\nPrice";
      e.Layout.Bands[0].Columns["DCSubconDefault"].Width = 55;
      e.Layout.Bands[0].Columns["DCSubconDefault"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCSubconDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCSubconDefault"].CellAppearance.BackColor = Color.LightCyan;

      //e.Layout.Bands[0].Columns["DCDefault"].Header.Caption = "Fac.\nDefault\nPrice";
      //e.Layout.Bands[0].Columns["DCDefault"].Width = 55;
      //e.Layout.Bands[0].Columns["DCDefault"].Format = "#,##0.00";
      //e.Layout.Bands[0].Columns["DCDefault"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Columns["DCDefault"].CellAppearance.BackColor = Color.LightCyan;

      //e.Layout.Bands[0].Columns["DCActualPrice"].Header.Caption = "Fac.\nActual\nPrice";
      e.Layout.Bands[0].Columns["DCActualPrice"].Header.Caption = "Fac.\nSale\nPrice";
      e.Layout.Bands[0].Columns["DCActualPrice"].Width = 55;
      e.Layout.Bands[0].Columns["DCActualPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCActualPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCActualPrice"].CellAppearance.BackColor = Color.LightCyan;

      //e.Layout.Bands[0].Columns["DCActualDate"].Header.Caption = "Fac.\nActual\nDate";
      e.Layout.Bands[0].Columns["DCActualDate"].Header.Caption = "Fac.\nSale\nDate";
      e.Layout.Bands[0].Columns["DCActualDate"].Width = 70;
      e.Layout.Bands[0].Columns["DCActualDate"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCChange"].Header.Caption = "+/-%";
      e.Layout.Bands[0].Columns["DCChange"].Width = 55;
      e.Layout.Bands[0].Columns["DCChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DCChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCChange"].CellAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["DCSchedulePrice"].Header.Caption = "Fac.\nNew\nPrice";
      e.Layout.Bands[0].Columns["DCSchedulePrice"].Width = 55;
      e.Layout.Bands[0].Columns["DCSchedulePrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["DCSchedulePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      
      e.Layout.Bands[0].Columns["DCNewChange"].Header.Caption = "New\n+/-%";
      e.Layout.Bands[0].Columns["DCNewChange"].Width = 55;
      e.Layout.Bands[0].Columns["DCNewChange"].Format = "#,##0.00%";
      e.Layout.Bands[0].Columns["DCNewChange"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["DCNewChange"].CellAppearance.BackColor = Color.LightCyan;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      // Change color if BOM not confirm
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        int bomConfirm = DBConvert.ParseInt(row.Cells["BOMConfirm"].Value.ToString());
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
      bool isChangeDistribute = (string.Compare(columnName, "DISSchedulePrice", true) == 0);
      bool isChangeFactory = (string.Compare(columnName, "DCSchedulePrice", true) == 0);
      if (isChangeDistribute || isChangeFactory)
      {
        string text = e.Cell.Text.ToString().Trim();
        if (text.Length > 0)
        {
          double value = DBConvert.ParseDouble(text);
          string title = (isChangeDistribute) ? "Distribute New Price" : "Factory New Price";
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
      if (string.Compare(columnName, "Rowstate [hidden]", true) != 0)
      {
        row.Cells["Rowstate"].Value = 1;
      }

      double dcSchedulePrice = DBConvert.ParseDouble(row.Cells["DCSchedulePrice"].Value.ToString());
      double disSchedulePrice = DBConvert.ParseDouble(row.Cells["DISSchedulePrice"].Value.ToString());

      if (string.Compare(columnName, "DISSchedulePrice", true) == 0)
      {
        double disActivePrice = DBConvert.ParseDouble(row.Cells["DISActualPrice"].Value.ToString());

        if (disActivePrice != double.MinValue && disActivePrice != 0 && disSchedulePrice != double.MinValue)
        {
          row.Cells["DISNewChange"].Value = (disSchedulePrice - disActivePrice) / disActivePrice;
        }
        else
        {
          row.Cells["DISNewChange"].Value = DBNull.Value;
        }

        if (dcSchedulePrice != double.MinValue && dcSchedulePrice != 0 && disSchedulePrice != double.MinValue)
        {
          row.Cells["DISNewMakup"].Value = disSchedulePrice / dcSchedulePrice;
        }
        else
        {
          row.Cells["DISNewMakup"].Value = DBNull.Value;
        }
      }
      else if (string.Compare(columnName, "DCSchedulePrice", true) == 0)
      {
        double dcActivePrice = DBConvert.ParseDouble(row.Cells["DCActualPrice"].Value.ToString());
        if (dcActivePrice != double.MinValue && dcActivePrice != 0 && dcSchedulePrice != double.MinValue)
        {
          row.Cells["DCNewChange"].Value = (dcSchedulePrice - dcActivePrice) / dcActivePrice;
        }
        else
        {
          row.Cells["DCNewChange"].Value = DBNull.Value;
        }

        if (dcSchedulePrice != double.MinValue && dcSchedulePrice != 0 && disSchedulePrice != double.MinValue)
        {
          row.Cells["DISNewMakup"].Value = disSchedulePrice / dcSchedulePrice;
        }
        else
        {
          row.Cells["DISNewMakup"].Value = DBNull.Value;
        }        
      }      
    }

    /// <summary>
    /// Out report factory cost
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //try
      //{
      //  string itemCode = ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString();
      //  if (rdViewCosting.Checked)
      //  {
      //    FunctionUtility.ViewItemCosting(itemCode);
      //  }
      //  else
      //  {
      //    viewBOM_01_002 objItemMaster = new viewBOM_01_002();
      //    objItemMaster.itemCode = itemCode;
      //    DataTable dtRevision = DataBaseAccess.SearchCommandTextDataTable(string.Format("Select RevisionActive From TblBOMItemBasic Where ItemCode = '{0}'", itemCode));
      //    if (dtRevision != null && dtRevision.Rows.Count > 0)
      //    {
      //      objItemMaster.revision = DBConvert.ParseInt(dtRevision.Rows[0][0].ToString());
      //      Shared.Utility.WindowUtinity.ShowView(objItemMaster, "ITEM MASTER LEVEL 1ST", false, ViewState.Window, FormWindowState.Maximized);
      //    }
      //  }
      //}
      //catch { }
    }

    /// <summary>
    /// Innit layout ultra grid Show Column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultShowColumn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Customer"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Customer"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Customer"].MinWidth = 90;
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

      //e.Layout.Bands[0].Columns["ActualPrice"].Header.Caption = "Actual\nPrice";
      e.Layout.Bands[0].Columns["ActualPrice"].Header.Caption = "Sale\nPrice";
      e.Layout.Bands[0].Columns["ActualPrice"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      //e.Layout.Bands[0].Columns["ActualDate"].Header.Caption = "Actual\nDate";
      e.Layout.Bands[0].Columns["ActualDate"].Header.Caption = "Sale\nDate";
      e.Layout.Bands[0].Columns["ActualDate"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

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
      string prefix = (row.Index == 0) ? "DIS" : "DC";
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly)
        {
          ultData.DisplayLayout.Bands[0].Columns[string.Format("{0}{1}", prefix, columnName)].Hidden = e.Cell.Text.ToString().Equals("0");
        }
      }
      else
      {
        for (int i = 2; i < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; i++)
        {
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
      this.LoadUltraData();
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
      view.isFactory = false;
      WindowUtinity.ShowView(view, "IMPORT DISTRIBUTE SALE PRICE", false, ViewState.MainWindow);
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
      this.LoadUltraData();
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
        if (schedule != double.MinValue && actual != schedule)
        {
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
      this.LoadUltraData();
      btnConfirmPriceFactory.Enabled = true;
    }

    /// <summary>
    /// Save data and confirm distibute price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnConfirmPriceDistrbute_Click(object sender, EventArgs e)
    {
      btnConfirmPriceFactory.Enabled = false;
      bool saveSuccess = this.SaveData();
      bool confirmSuccess = true;
      int count = ultData.Rows.Count;
      int outputValue = 0;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        double actual = DBConvert.ParseDouble(row.Cells["DISActualPrice"].Value.ToString());
        double schedule = DBConvert.ParseDouble(row.Cells["DISSchedulePrice"].Value.ToString());
        if (schedule != double.MinValue && actual != schedule)
        {
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          DBParameter[] input = new DBParameter[3];
          input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          input[1] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);
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
      this.LoadUltraData();
      btnConfirmPriceFactory.Enabled = true;
    }
    /// <summary>
    /// Export To Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExportToExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "OEMPriceList");

      //string strTemplateName = "CSDItemsPriceByCustomer";
      //string strSheetName = "ItemsPrice";
      //string strOutFileName = "Distribute Sale Price";
      //string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //DataTable dtData = (DataTable)ultData.DataSource;
      //if (dtData != null && dtData.Rows.Count > 0)
      //{
      //  for (int i = 0; i < dtData.Rows.Count; i++)
      //  {
      //    DataRow drRow = dtData.Rows[i];
      //    if (i > 0)
      //    {
      //      oXlsReport.Cell("B7:M7").Copy();
      //      oXlsReport.RowInsert(6 + i);
      //      oXlsReport.Cell("B7:M7", 0, i).Paste();
      //    }
      //    oXlsReport.Cell("**No", 0, i).Value = i + 1;
      //    oXlsReport.Cell("**Customer", 0, i).Value = this.customerCode;
      //    oXlsReport.Cell("**ItemCode", 0, i).Value = drRow["ItemCode"];
      //    oXlsReport.Cell("**ItemName", 0, i).Value = drRow["Name"];
      //    oXlsReport.Cell("**SaleCode", 0, i).Value = drRow["SaleCode"];
      //    oXlsReport.Cell("**FacDefaultPrice", 0, i).Value = drRow["DCDefault"];
      //    oXlsReport.Cell("**FacActualDate", 0, i).Value = drRow["DCActualDate"];
      //    oXlsReport.Cell("**FacActualPrice", 0, i).Value = drRow["DCActualPrice"];
      //    oXlsReport.Cell("**ActualDate", 0, i).Value = drRow["DISActualDate"];
      //    oXlsReport.Cell("**ActualPrice", 0, i).Value = drRow["DISActualPrice"];
      //    oXlsReport.Cell("**MOQ", 0, i).Value = drRow["MOQ"];
      //  }
      //}
      //oXlsReport.Out.File(strOutFileName);
      //Process.Start(strOutFileName);
    }

    private void btnImportMOQ_Click(object sender, EventArgs e)
    {
      viewCSD_04_018 view = new viewCSD_04_018();      
      WindowUtinity.ShowView(view, "Import MOQ For Item", false, ViewState.MainWindow);
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
        this.LoadUltraData();
      }
    }

    private void chkShowColumns_CheckedChanged(object sender, EventArgs e)
    {
      ultShowColumn.Visible = chkShowColumns.Checked;
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

    private void ultData_DoubleClick(object sender, EventArgs e)
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

            if (ultTypeCompare.DisplayLayout.Bands[0].Columns.Count >= 5)
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

            if (ultTypeCompare.DisplayLayout.Bands[0].Columns.Count >= 4)
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
          //DataTable dtRevision = DataBaseAccess.SearchCommandTextDataTable(string.Format("Select RevisionActive From TblBOMItemBasic Where ItemCode = '{0}'", itemCode));
          //if (dtRevision != null && dtRevision.Rows.Count > 0)
          //{
          //  objItemMaster.revision = DBConvert.ParseInt(dtRevision.Rows[0][0].ToString());
          //  Shared.Utility.WindowUtinity.ShowView(objItemMaster, "ITEM MASTER LEVEL 1ST", false, ViewState.Window, FormWindowState.Maximized);
          //}
        }
      }
      catch { }
    }
    #endregion Textbox KeyDown Events  
  }
}
