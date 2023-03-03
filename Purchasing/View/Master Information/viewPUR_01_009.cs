/*
 * Author       : Vo Van Duy Qui
 * CreateDate   : 29-03-2011
 * Description  : Currency Exchange Rate Information
 */

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_01_009 : MainUserControl
  {
    #region Field
    public long currencyPid = long.MinValue;
    public bool approvedStandard = false;
    private bool loadData = false;

    private string SP_PUR_CURRENCYEXCHANGEINFO = "spPURCurrencyExchangeInfo";
    private string SP_PUR_CURRENCYEXCHANGE_UPDATE = "spPURCurrencyExchange_Update";
    #endregion Field

    #region Init

    /// <summary>
    /// Init Form
    /// </summary>
    public viewPUR_01_009()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_009_Load(object sender, EventArgs e)
    {
      if (approvedStandard == false)
      {
        txtActualExchangeRate.ReadOnly = true;
      }

      this.LoadData();
      this.loadData = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      if (this.currencyPid != long.MinValue)
      {
        DBParameter[] input = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.currencyPid) };
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure(SP_PUR_CURRENCYEXCHANGEINFO, input);

        if (dsSource != null)
        {
          DataTable dtMain = dsSource.Tables[0];
          if (dtMain.Rows.Count > 0)
          {
            string code = dtMain.Rows[0]["Code"].ToString().Trim();
            double exchangeRate = DBConvert.ParseDouble(dtMain.Rows[0]["CurrentExchangeRate"].ToString());
            double actualExchangeRate = DBConvert.ParseDouble(dtMain.Rows[0]["ActualCurrentExchangeRate"].ToString());

            txtCurrency.Text = code;
            txtExchangeRate.Text = this.NumericFormat(exchangeRate, 2);
            txtActualExchangeRate.Text = this.NumericFormat(actualExchangeRate, 2);
          }
          ultData.DataSource = dsSource.Tables[1];
        }
      }
    }

    /// <summary>
    /// Check Valid Input Data
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      // Standard ExchangeRete
      double exchangeRate = DBConvert.ParseDouble(txtExchangeRate.Text.Trim());
      if (exchangeRate == double.MinValue || exchangeRate < 0)
      {
        WindowUtinity.ShowMessageError("ERR0061");
        return false;
      }

      // Actual ExchangeRete
      double actualExchangeRate = DBConvert.ParseDouble(txtActualExchangeRate.Text.Trim());
      if (actualExchangeRate == double.MinValue || actualExchangeRate < 0)
      {
        WindowUtinity.ShowMessageError("ERR0061");
        return false;
      }

      string commandText = (string.Format("SELECT TOP 1 ExchangeRate, ISNULL(ActualExchangeRate, 0) ActualExchangeRate FROM TblPURExchangeRateHistory WHERE CurrencyPid = {0} ORDER BY CreateDate DESC", this.currencyPid));
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      double exchange = 0;
      double actualExchange = 0;
      if (dt != null && dt.Rows.Count == 1)
      {
        exchange = DBConvert.ParseDouble(dt.Rows[0]["ExchangeRate"].ToString());
        actualExchange = DBConvert.ParseDouble(dt.Rows[0]["ActualExchangeRate"].ToString());
      }
      if (exchange == exchangeRate && actualExchange == actualExchangeRate)
      {
        WindowUtinity.ShowMessageError("ERR0032", "Exchange Rate");
        this.NeedToSave = false;
        return false;
      }
      return true;
    }
    #endregion Init

    #region Event

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["CurrencyCode"].Header.Caption = "Currency Code";
      e.Layout.Bands[0].Columns["ExchangeRate"].Header.Caption = "Standard Exchange Rate";
      e.Layout.Bands[0].Columns["ActualExchangeRate"].Header.Caption = "Actual Exchange Rate";
      e.Layout.Bands[0].Columns["EmpName"].Header.Caption = "Employee Name";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["ExchangeRate"].Format = "###,###.## VND";
      e.Layout.Bands[0].Columns["ActualExchangeRate"].Format = "###,###.## VND";
    }

    /// <summary>
    /// btn Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// btnSave Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckValid())
      {
        double exchangeRate = DBConvert.ParseDouble(txtExchangeRate.Text);
        double actualExchangeRate = DBConvert.ParseDouble(txtActualExchangeRate.Text);

        DBParameter[] input = new DBParameter[4];
        input[0] = new DBParameter("@CurrentcyPid", DbType.Int64, this.currencyPid);
        input[1] = new DBParameter("@ExchangeRate", DbType.Double, exchangeRate);
        input[2] = new DBParameter("@ActualExchangeRate", DbType.Double, actualExchangeRate);
        input[3] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

        DBParameter[] output = new DBParameter[] { new DBParameter("Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure(SP_PUR_CURRENCYEXCHANGE_UPDATE, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          WindowUtinity.ShowMessageError("ERR0005");
          this.SaveSuccess = false;
          return;
        }
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.NeedToSave = false;
        this.LoadData();
      }
    }

    /// <summary>
    /// txtExchangeRate Text Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtExchangeRate_TextChanged(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        this.NeedToSave = true;
      }
    }

    private string NumericFormat(double number, int phanLe)
    {
      if (number == double.MinValue)
      {
        return string.Empty;
      }
      if (phanLe < 0)
      {
        return number.ToString();
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      double t = Math.Truncate(number);
      formatInfo.NumberDecimalDigits = phanLe;
      return number.ToString("N", formatInfo);
    }

    /// <summary>
    /// txtExchangeRate Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtExchangeRate_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtExchangeRate.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtExchangeRate.Text = numberRead;
      this.NeedToSave = false;
    }

    private void txtActualExchangeRate_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtActualExchangeRate.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtActualExchangeRate.Text = numberRead;
      this.NeedToSave = false;
    }

    private void txtActualExchangeRate_TextChanged(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        this.NeedToSave = true;
      }
    }
    #endregion Event
  }
}

