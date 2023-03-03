/*
  Author      : 
  Date        : 06/08/2013
  Description : Print PR
*/

using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Purchasing.DataSetSource;
using Purchasing.Reports;
using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Purchasing
{
  public partial class viewPUR_03_007 : MainUserControl
  {
    #region Field
    public string prNO = string.Empty;
    public string prDetailPid = string.Empty;
    #endregion Field

    #region Init
    public viewPUR_03_007()
    {
      InitializeComponent();
    }

    private void viewPUR_03_007_Load(object sender, EventArgs e)
    {
      this.LoadCurrencyExchange();
    }

    private void LoadCurrencyExchange()
    {
      string commandText = string.Empty;
      commandText = "SELECT Pid, Code FROM TblPURCurrencyInfo";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultCBCurrency.DataSource = dtSource;
        ultCBCurrency.DisplayMember = "Code";
        ultCBCurrency.ValueMember = "Pid";
        ultCBCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultCBCurrency.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      }
    }

    #endregion Init

    #region Function
    private void PrintPRNormal()
    {
      DBParameter[] inputParam = new DBParameter[4];
      inputParam[0] = new DBParameter("@PRNo", DbType.String, this.prNO);
      inputParam[1] = new DBParameter("@Currency", DbType.Int32, DBConvert.ParseInt(ultCBCurrency.Value.ToString()));
      if (this.prDetailPid.Length > 0)
      {
        inputParam[2] = new DBParameter("@PRDetailPid", DbType.String, this.prDetailPid);
      }
      if (DBConvert.ParseInt(txtMaterialPage.Text) != int.MinValue)
      {
        inputParam[3] = new DBParameter("@MaterialPage", DbType.Int32, DBConvert.ParseInt(txtMaterialPage.Text));
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPRInformation_Select", 300, inputParam);
      if (ds != null && ds.Tables.Count > 0)
      {
        dsPURPRInfomation dsSource = new dsPURPRInfomation();
        dsSource.Tables["dtPRInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtPRDetail"].Merge(ds.Tables[1]);
        string printer = SharedObject.UserInfo.EmpName;
        string printDate = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        ReportClass cpt = null;
        DaiCo.Shared.View_Report report = null;
        cpt = new cptPURPRInformation();
        cpt.SetDataSource(dsSource);
        cpt.SetParameterValue("printer", printer);
        cpt.SetParameterValue("printDate", printDate);
        cpt.SetParameterValue("ChkMoreDetail", 1);
        //cpt.Subreports["cptPURSubBudget.rpt"].PrintOptions.PageContentHeight = this.chkMoreDetail.Checked ? 80 : 0;



        if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
        {

          cpt.SetParameterValue("ProjectCode", ds.Tables[2].Rows[0]["ProjectCode"].ToString());
          cpt.SetParameterValue("ProjectName", ds.Tables[2].Rows[0]["ProjectName"].ToString());
          cpt.SetParameterValue("BudgetAmount", ds.Tables[2].Rows[0]["BudgetAmount"].ToString());
          cpt.SetParameterValue("BudgetRequest", ds.Tables[2].Rows[0]["BudgetRequested"].ToString());
          cpt.SetParameterValue("BudgetReceive", ds.Tables[2].Rows[0]["BudgetReceived"].ToString());
          cpt.SetParameterValue("OutstandingRequest", ds.Tables[2].Rows[0]["OutstandingRequest"].ToString());
          cpt.SetParameterValue("OutstandingReceive", ds.Tables[2].Rows[0]["OutstandingReceive"].ToString());
        }
        else
        {
          cpt.SetParameterValue("ProjectCode", "");
          cpt.SetParameterValue("ProjectName", "");
          cpt.SetParameterValue("BudgetAmount", "");
          cpt.SetParameterValue("BudgetRequest", "");
          cpt.SetParameterValue("BudgetReceive", "");
          cpt.SetParameterValue("OutstandingRequest", "");
          cpt.SetParameterValue("OutstandingReceive", "");
        }

        //ControlUtility.ViewCrystalReport(cpt);

        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = false;
        report.ShowReport(DaiCo.Shared.Utility.ViewState.MainWindow);
      }
    }

    private void PrintPRMonthly()
    {
      DBParameter[] inputParam = new DBParameter[4];
      inputParam[0] = new DBParameter("@PRNo", DbType.String, this.prNO);
      inputParam[1] = new DBParameter("@Currency", DbType.Int32, DBConvert.ParseInt(ultCBCurrency.Value.ToString()));
      if (this.prDetailPid.Length > 0)
      {
        inputParam[2] = new DBParameter("@PRDetailPid", DbType.String, this.prDetailPid);
      }
      if (DBConvert.ParseInt(txtMaterialPage.Text) != int.MinValue)
      {
        inputParam[3] = new DBParameter("@MaterialPage", DbType.Int32, DBConvert.ParseInt(txtMaterialPage.Text));
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPRInformationVersion1_Select", 300, inputParam);
      if (ds.Tables.Count > 0)
      {
        dsPURPRInfomation dsSource = new dsPURPRInfomation();
        dsSource.Tables["dtPRInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtPRDetail"].Merge(ds.Tables[1]);
        string printer = SharedObject.UserInfo.EmpName;
        string printDate = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        ReportClass cpt = null;
        DaiCo.Shared.View_Report report = null;
        System.Globalization.DateTimeFormatInfo d = new System.Globalization.DateTimeFormatInfo();

        //int a = DateTime.Now.AddMonths(-5).Month - 1;
        //string b = d.AbbreviatedMonthNames[3];
        string columnName1 = d.AbbreviatedMonthNames[DateTime.Now.AddMonths(-5).Month - 1];
        string columnName2 = d.AbbreviatedMonthNames[DateTime.Now.AddMonths(-4).Month - 1];
        string columnName3 = d.AbbreviatedMonthNames[DateTime.Now.AddMonths(-3).Month - 1];
        string columnName4 = d.AbbreviatedMonthNames[DateTime.Now.AddMonths(-2).Month - 1];
        string columnName5 = d.AbbreviatedMonthNames[DateTime.Now.AddMonths(-1).Month - 1];
        string monthCurrent = d.AbbreviatedMonthNames[DateTime.Now.Month - 1];

        string acolumnName4 = d.AbbreviatedMonthNames[DateTime.Now.AddMonths(4).Month - 1];
        string acolumnName3 = d.AbbreviatedMonthNames[DateTime.Now.AddMonths(3).Month - 1];
        string acolumnName2 = d.AbbreviatedMonthNames[DateTime.Now.AddMonths(2).Month - 1];
        string acolumnName1 = d.AbbreviatedMonthNames[DateTime.Now.AddMonths(1).Month - 1];
        string amonthCurrent = d.AbbreviatedMonthNames[DateTime.Now.Month - 1];

        cpt = new cptPURPRInformationVersion1();
        cpt.SetDataSource(dsSource);
        cpt.SetParameterValue("printer", printer);
        cpt.SetParameterValue("printDate", printDate);
        cpt.SetParameterValue("ColumnName1", columnName1);
        cpt.SetParameterValue("ColumnName2", columnName2);
        cpt.SetParameterValue("ColumnName3", columnName3);
        cpt.SetParameterValue("ColumnName4", columnName4);
        cpt.SetParameterValue("ColumnName5", columnName5);

        cpt.SetParameterValue("MonthCurName", amonthCurrent);
        cpt.SetParameterValue("MonthCurAdd1Name", acolumnName1);
        cpt.SetParameterValue("MonthCurAdd2Name", acolumnName2);
        cpt.SetParameterValue("MonthCurAdd3Name", acolumnName3);
        cpt.SetParameterValue("MonthCurAdd4Name", acolumnName4);

        cpt.SetParameterValue("MonthCurrent", monthCurrent);
        cpt.SetParameterValue("ChkMoreDetail", 1);
        //cpt.Subreports["cptPURSubBudget.rpt"].PrintOptions.PageContentHeight = this.chkMoreDetail.Checked ? 80 : 0;



        if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
        {
          cpt.SetParameterValue("ProjectCode", ds.Tables[2].Rows[0]["ProjectCode"].ToString());
          cpt.SetParameterValue("ProjectName", ds.Tables[2].Rows[0]["ProjectName"].ToString());
          cpt.SetParameterValue("BudgetAmount", ds.Tables[2].Rows[0]["BudgetAmount"].ToString());
          cpt.SetParameterValue("BudgetRequest", ds.Tables[2].Rows[0]["BudgetRequested"].ToString());
          cpt.SetParameterValue("BudgetReceive", ds.Tables[2].Rows[0]["BudgetReceived"].ToString());
          cpt.SetParameterValue("OutstandingRequest", ds.Tables[2].Rows[0]["OutstandingRequest"].ToString());
          cpt.SetParameterValue("OutstandingReceive", ds.Tables[2].Rows[0]["OutstandingReceive"].ToString());
        }
        else
        {
          cpt.SetParameterValue("ProjectCode", "");
          cpt.SetParameterValue("ProjectName", "");
          cpt.SetParameterValue("BudgetAmount", "");
          cpt.SetParameterValue("BudgetRequest", "");
          cpt.SetParameterValue("BudgetReceive", "");
          cpt.SetParameterValue("OutstandingRequest", "");
          cpt.SetParameterValue("OutstandingReceive", "");
        }

        //ControlUtility.ViewCrystalReport(cpt);

        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = false;
        report.ShowReport(DaiCo.Shared.Utility.ViewState.MainWindow);
      }
    }

    private void PrintPRDetail()
    {
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@PRNo", DbType.String, this.prNO);
      inputParam[1] = new DBParameter("@Currency", DbType.Int32, DBConvert.ParseInt(ultCBCurrency.Value.ToString()));
      if (this.prDetailPid.Length > 0)
      {
        inputParam[2] = new DBParameter("@PRDetailPid", DbType.String, this.prDetailPid);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPRInformationVersion2_Select", 6000, inputParam);
      if (ds.Tables.Count > 0)
      {
        dsPURPRInformationVersion2 dsSource = new dsPURPRInformationVersion2();
        dsSource.Tables["dtPRInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtPRDetail"].Merge(ds.Tables[1]);
        string printer = SharedObject.UserInfo.EmpName;
        string printDate = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        ReportClass cpt = null;
        DaiCo.Shared.View_Report report = null;
        cpt = new cptPURPRInformationVersion2();
        cpt.SetDataSource(dsSource);
        cpt.SetParameterValue("printer", printer);
        cpt.SetParameterValue("printDate", printDate);


        if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
        {
          cpt.SetParameterValue("ProjectCode", ds.Tables[2].Rows[0]["ProjectCode"].ToString());
          cpt.SetParameterValue("BudgetAmount", ds.Tables[2].Rows[0]["BudgetAmount"].ToString());
          cpt.SetParameterValue("BudgetRequest", ds.Tables[2].Rows[0]["BudgetRequested"].ToString());
          cpt.SetParameterValue("BudgetReceive", ds.Tables[2].Rows[0]["BudgetReceived"].ToString());
          cpt.SetParameterValue("OutstandingRequest", ds.Tables[2].Rows[0]["OutstandingRequest"].ToString());
          cpt.SetParameterValue("OutstandingReceive", ds.Tables[2].Rows[0]["OutstandingReceive"].ToString());
        }
        else
        {
          cpt.SetParameterValue("ProjectCode", "");
          cpt.SetParameterValue("BudgetAmount", "");
          cpt.SetParameterValue("BudgetRequest", "");
          cpt.SetParameterValue("BudgetReceive", "");
          cpt.SetParameterValue("OutstandingRequest", "");
          cpt.SetParameterValue("OutstandingReceive", "");

        }

        //ControlUtility.ViewCrystalReport(cpt);

        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = false;
        report.ShowReport(DaiCo.Shared.Utility.ViewState.MainWindow);
      }
    }

    #endregion Function

    #region Event
    private void btnPrint_Click(object sender, EventArgs e)
    {
      // Check Budget
      DBParameter[] inputBudget = new DBParameter[1];
      inputBudget[0] = new DBParameter("@PRNo", DbType.String, this.prNO);
      DBParameter[] output = new DBParameter[2];
      output[0] = new DBParameter("@Result", DbType.Int64, 0);
      output[1] = new DBParameter("@BudgetCode", DbType.String, 128, string.Empty);
      DataBaseAccess.ExecuteStoreProcedure("spPURPRCheckingBudgetOverForPurchasing", inputBudget, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      string budgetCode = output[1].Value.ToString();
      if (result == 0)
      {
        if (WindowUtinity.ShowMessageConfirmFromText("Total Budget Requested > Budget Amount, Do You want to show detail ?") == DialogResult.Yes)
        {
          bool a = true;
          a = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = '" + budgetCode + "' WHERE [Group] = 16077 AND Value = 1", null);
          Process.Start(DataBaseAccess.ExecuteScalarCommandText("SELECT [Description] FROM TblBOMCodeMaster WHERE [Group] = 16077 AND Value = 2", null).ToString());
          Thread.Sleep(2000);
          a = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = NULL WHERE [Group] = 16077 AND Value = 1", null);
          return;
        }
        else
        {
          return;
        }
      }
      // End

      // Check Currency Exchange
      if (ultCBCurrency.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Currency Exchange");
        return;
      }

      // Warning Input Labor Price
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@PRNo", DbType.String, this.prNO);
      if (this.prDetailPid.Length > 0)
      {
        inputParam[1] = new DBParameter("@PRDetailPid", DbType.String, this.prDetailPid);
      }
      DataTable dtWarning = DataBaseAccess.SearchStoreProcedureDataTable("spPURPRDetailWarningLaborPrice_Select", 300, inputParam);
      if (dtWarning != null && dtWarning.Rows.Count > 0)
      {
        DialogResult resultWarning = WindowUtinity.ShowMessageConfirmFromText(string.Format(string.Format(@"Materials {0} aren't inputted the labor price. Do you want to continue?", dtWarning.Rows[0]["MaterialCode"])));
        if (resultWarning == DialogResult.No)
        {
          return;
        }
      }
      // End Warning Input Labor Price

      // Update PrintNo
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@PRNo", DbType.String, this.prNO);
      DataBaseAccess.ExecuteStoreProcedure("spPURPRInformationPrintNo_Update", input);

      if (radPurpose.Checked)
      {
        this.PrintPRNormal();
      }
      else if (radMonthlyConsumption.Checked)
      {
        this.PrintPRMonthly();
      }
      else if (radGroup.Checked)
      {
        this.PrintPRDetail();
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion Event
  }
}
