/*
  Author      : Ha Anh
  Date        : 10/05/2011
  Description : Material Formula
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
  public partial class viewPUR_01_010 : MainUserControl
  {

    public string materialCode = string.Empty;
    private int radCheck;

    /// <summary>
    /// 
    /// </summary>
    public viewPUR_01_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// 
    /// </summary>
    private void viewPUR_01_010_Load(object sender, EventArgs e)
    {
      txtMaterialCode.Text = this.materialCode.ToString();
      this.LoadMaterialFormula();

      string commandText = string.Format(@"SELECT * FROM TblGNRMaterialInformation WHERE MaterialCode = '{0}'", this.materialCode);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count == 1)
      {
        if (ultGrid.Rows.Count == 0)
        {
          // Load Default Formula
          if (this.materialCode.Trim().Length > 0)
          {
            string materialCodeSub = this.materialCode.Substring(0, 6);
            // Parameter Input
            DBParameter[] inputParam = new DBParameter[1];
            inputParam[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCodeSub);
            DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPURLoadMaterialFormulaDefault", inputParam);
            int minOfStockOption = int.MinValue;
            if (dtSource != null && dtSource.Rows.Count == 1)
            {
              txtLeadTime.Text = dtSource.Rows[0]["LeadTime"].ToString();
              txtCoefficientReorder.Text = dtSource.Rows[0]["CoefficientReorder"].ToString();
              txtCoefficientMinOfStock.Text = dtSource.Rows[0]["CoefficientMinimumStock"].ToString();
              minOfStockOption = DBConvert.ParseInt(dtSource.Rows[0]["MinOfStockOption"].ToString());
              if (minOfStockOption != int.MinValue)
              {
                if (minOfStockOption == 1)
                {
                  radDefault.Checked = true;
                  txtDefaultMinOfStock.Text = dtSource.Rows[0]["DefaultMinOfStock"].ToString();
                }
                else
                {
                  radFormula.Checked = true;
                  if (DBConvert.ParseInt(dtSource.Rows[0]["FormulaMinOfStockOption"].ToString()) == 1)
                  {
                    radAverage.Checked = true;
                  }
                  else if (DBConvert.ParseInt(dtSource.Rows[0]["FormulaMinOfStockOption"].ToString()) == 2)
                  {
                    radMax.Checked = true;
                  }
                  else
                  {
                    radMin.Checked = true;
                  }
                  txtMonthOfFormula.Text = dtSource.Rows[0]["MonthOfFormula"].ToString();
                }
              }
            }
          }
        }
      }
    }

    private void LoadMaterialFormula()
    {
      String storeName = "spPURMaterialFormula_Select";
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, this.materialCode.ToString());
      DataSet dataset = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      ultGrid.DataSource = dataset;
      ultGrid.DisplayLayout.Bands[0].Columns["LeadTime"].Header.Caption = "Lead Time";
      ultGrid.DisplayLayout.Bands[0].Columns["LeadTime"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["CoefficientReorder"].Header.Caption = "Coefficient Reorder";
      ultGrid.DisplayLayout.Bands[0].Columns["CoefficientReorder"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["CoefficientMinimumStock"].Header.Caption = "Coefficient Minimum Stock";
      ultGrid.DisplayLayout.Bands[0].Columns["CoefficientMinimumStock"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["CoefficientMinimumStock"].Header.Caption = "Coefficient Minimum Stock";
      ultGrid.DisplayLayout.Bands[0].Columns["CoefficientMinimumStock"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["MinOfStockOption"].Header.Caption = "Min Of Stock Option";
      ultGrid.DisplayLayout.Bands[0].Columns["MinOfStockOption"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["DefaultMinOfStock"].Header.Caption = "Default Min Of Stock";
      ultGrid.DisplayLayout.Bands[0].Columns["DefaultMinOfStock"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["FormulaMinOfStockOption"].Header.Caption = "Formula Option";
      ultGrid.DisplayLayout.Bands[0].Columns["FormulaMinOfStockOption"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["MonthOfFormula"].Header.Caption = "Month Of Formula";
      ultGrid.DisplayLayout.Bands[0].Columns["MonthOfFormula"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      ultGrid.DisplayLayout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      ultGrid.DisplayLayout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;

      ultGrid.DisplayLayout.Bands[0].Columns["Active"].Header.Caption = "Actived";
      ultGrid.DisplayLayout.Bands[0].Columns["Active"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ultGrid.DisplayLayout.Bands[0].Columns["Active"].CellActivation = Activation.ActivateOnly;
    }

    /// <summary>
    /// 
    /// </summary>
    private void ultGrid_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool check = ValidationInput(out message);
      if (!check)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      check = this.SaveData();
      if (check)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadMaterialFormula();
    }

    /// <summary>
    /// 
    /// </summary>
    private bool SaveData()
    {
      String storeName = "spPURMaterialFormula_Insert";
      DBParameter[] inputParam = new DBParameter[11];
      inputParam[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode.ToString());
      double leadtime = DBConvert.ParseDouble(txtLeadTime.Text.ToString());
      inputParam[2] = new DBParameter("LeadTime", DbType.Double, leadtime);
      double reorder = DBConvert.ParseDouble(txtCoefficientReorder.Text.ToString());
      inputParam[3] = new DBParameter("@CoefficientReorder", DbType.Double, reorder);
      double coeMinOfStock = DBConvert.ParseDouble(txtCoefficientMinOfStock.Text.ToString());
      inputParam[4] = new DBParameter("@CoefficientMinimumStock", DbType.Double, coeMinOfStock);
      if (radDefault.Checked)
      {
        inputParam[5] = new DBParameter("@MinOfStockOption", DbType.Int16, 1);
      }
      else
      {
        inputParam[5] = new DBParameter("@MinOfStockOption", DbType.Int16, 2);
      }

      if (txtDefaultMinOfStock.Text.ToString().Length > 0)
      {
        double defaultMinOfStock = DBConvert.ParseDouble(txtDefaultMinOfStock.Text.ToString());
        inputParam[6] = new DBParameter("@DefaultMinOfStock", DbType.Double, defaultMinOfStock);
      }
      if (radFormula.Checked)
      {
        if (radAverage.Checked)
        {
          radCheck = 1;
        }
        if (radMax.Checked)
        {
          radCheck = 2;
        }
        if (radMin.Checked)
        {
          radCheck = 3;
        }
        inputParam[7] = new DBParameter("@FormulaMinOfStockOption", DbType.Int16, radCheck);
        int month = DBConvert.ParseInt(txtMonthOfFormula.Text.ToString());
        inputParam[8] = new DBParameter("MonthOfFormula", DbType.Int16, month);
      }
      inputParam[9] = new DBParameter("@CreateDate", DbType.DateTime, DateTime.Today);
      inputParam[10] = new DBParameter("@CreateBy", DbType.Int16, SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      long outputvalue = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (outputvalue == 0)
      {
        return false;
      }
      else
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    private bool ValidationInput(out string message)
    {
      message = string.Empty;
      if (DBConvert.ParseDouble(txtLeadTime.Text.ToString()) == double.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Lead Day");
        return false;
      }
      if (DBConvert.ParseDouble(txtCoefficientReorder.Text.ToString()) == double.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Coefficient Reorder");
        return false;
      }
      if (DBConvert.ParseDouble(txtCoefficientMinOfStock.Text.ToString()) == double.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Coefficient Minimum Of Stock");
        return false;
      }
      if (radDefault.Checked)
      {
        if (DBConvert.ParseDouble(txtDefaultMinOfStock.Text.ToString()) == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Default Of Stock");
          return false;
        }
      }
      if (radFormula.Checked)
      {
        if (DBConvert.ParseInt(txtMonthOfFormula.Text.ToString()) == int.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Month Of Formula");
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    private void btClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// 
    /// </summary>
    private void radFormula_CheckedChanged(object sender, EventArgs e)
    {
      if (radFormula.Checked)
      {
        txtMonthOfFormula.Enabled = true;
        txtDefaultMinOfStock.Enabled = false;
        radAverage.Enabled = true;
        radMax.Enabled = true;
        radMin.Enabled = true;
        lbDefault.Visible = false;
        lbFormula.Visible = true;
        lbFormulaOption.Visible = true;
      }
      if (!radFormula.Checked)
      {
        txtMonthOfFormula.Enabled = false;
        txtDefaultMinOfStock.Enabled = true;
        radAverage.Enabled = false;
        radMax.Enabled = false;
        radMin.Enabled = false;
        lbDefault.Visible = true;
        lbFormula.Visible = false;
        lbFormulaOption.Visible = false;
      }
    }
  }
}
