/*
  Author      : 
  Date        : 06/08/2013
  Description : Print PR
*/

using DaiCo.Application;
using DaiCo.ERPProject.Purchasing.DataSetSource;
using DaiCo.ERPProject.Purchasing.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_04_007 : MainUserControl
  {
    #region Field
    public string poNO = string.Empty;
    public bool chkWithAddition = false;
    public bool chkRefCode = false;
    public bool chkNameVn = false;
    public int currency = int.MinValue;
    public int printType = int.MinValue;

    #endregion Field

    #region Init
    public viewPUR_04_007()
    {
      InitializeComponent();
    }

    private void viewPUR_04_007_Load(object sender, EventArgs e)
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

        if(currency!=int.MinValue)
        {
          ultCBCurrency.Value = currency;
        }  
      }
    }

    #endregion Init

    #region Function
    private void PrintReport(string poNo)
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PONo", DbType.AnsiString, 16, poNo);
      input[1] = new DBParameter("@Current", DbType.Int32, DBConvert.ParseInt(ultCBCurrency.Value.ToString()));
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPOInfomation_Select", input);
      if (ds != null)
      {
        dsPURPOInfomation dsSource = new dsPURPOInfomation();
        dsSource.Tables["dtPOInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtPODetail"].Merge(ds.Tables[1]);
        if (chkWithAddition)
        {
          dsSource.Tables["dtPOAdditionPrice"].Merge(ds.Tables[2]);
        }
        dsSource.Tables["dtListPR"].Merge(ds.Tables[3]);

        DaiCo.Shared.View_Report report = null;
        cptPURPOInformationNew cpt = new cptPURPOInformationNew();
        double totalAmount = 0;
        double totalAmount1 = 0;
        double totalAdditionPrice = 0;
        double totalVAT = 0;
        double total = 0;
        string orderRemark = string.Empty;
        // Total Amount
        for (int i = 0; i < dsSource.Tables["dtPODetail"].Rows.Count; i++)
        {
          DataRow row = dsSource.Tables["dtPODetail"].Rows[i];
          totalAmount = totalAmount + DBConvert.ParseDouble(row["Amount"].ToString());
          if (rdAfter.Checked)
          {
            if (row["VAT"].ToString().Length > 0)
            {
              totalVAT = totalVAT + ((DBConvert.ParseDouble(row["Amount"].ToString()) * DBConvert.ParseDouble(row["VAT"].ToString())) / 100);
            }
          }
          else
          {
            totalVAT = 0;
          }
        }
        // Total Addition Price
        for (int j = 0; j < dsSource.Tables["dtPOAdditionPrice"].Rows.Count; j++)
        {
          DataRow row = dsSource.Tables["dtPOAdditionPrice"].Rows[j];
          totalAdditionPrice = totalAdditionPrice + DBConvert.ParseDouble(row["Amount"].ToString());
        }
        // Total
        totalAmount1 = totalAmount + totalAdditionPrice;
        total = totalAmount + totalAdditionPrice + totalVAT;
        // Number To English
        string numberToEnglish = DaiCo.Shared.Utility.NumberToEnglish.ChangeNumericToWords(total) + "(" + dsSource.Tables["dtPOInfo"].Rows[0]["Currency"].ToString() + ")";
        // Order Remark
        if (dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString().Length > 0)
        {
          orderRemark = dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString();
          //DateTime date = DateTime.MinValue;
          //DateTime dateHtml = DateTime.MinValue;
          //date = DBConvert.ParseDateTime(dsSource.Tables["dtPOInfo"].Rows[0]["CreateDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
          //string commandText1 = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 9009";
          //DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText1);
          //if (dt != null)
          //{
          //  dateHtml = DBConvert.ParseDateTime(dt.Rows[0]["Value"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
          //}
          //if (date >= dateHtml)
          //{
          //  orderRemark = dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString();
          //}
          //else
          //{
          //  orderRemark = this.StripHTML(dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString());
          //}
          //cpt.ReportFooterSection3.SectionFormat.EnableSuppress = false;
        }
        //else
        //{
        //  cpt.ReportFooterSection3.SectionFormat.EnableSuppress = true;
        //}

        //// Remark Detail
        //if (dsSource.Tables["dtPOInfo"].Rows[0]["RemarkDetailEN"].ToString().Length > 0 ||
        //    dsSource.Tables["dtPOInfo"].Rows[0]["RemarkDetailVN"].ToString().Length > 0)
        //{
        //  cpt.ReportFooterSection6.SectionFormat.EnableSuppress = false;
        //}
        //else
        //{
        //  cpt.ReportFooterSection6.SectionFormat.EnableSuppress = true;
        //}

        string companyName = string.Empty;
        string email = string.Empty;
        string website = string.Empty;
        string telephone = string.Empty;
        string taxCode = string.Empty;
        string accountNo = string.Empty;
        string fax = string.Empty;
        string address = string.Empty;
        string purchaseManager = string.Empty;
        string PrintDate = string.Empty;

        string commandText = "SELECT TOP 1 FullName, ShortName, [Address], Tel, Fax, Email, TaxCode, Website, AccountNo FROM TblGNRCompanyInfo";
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtSource != null)
        {
          address = dtSource.Rows[0]["Address"].ToString();
          telephone = dtSource.Rows[0]["Tel"].ToString();
          fax = dtSource.Rows[0]["Fax"].ToString();
          email = dtSource.Rows[0]["Email"].ToString();
          website = dtSource.Rows[0]["Website"].ToString();
          taxCode = dtSource.Rows[0]["TaxCode"].ToString();
          accountNo = dtSource.Rows[0]["AccountNo"].ToString();
          companyName = dtSource.Rows[0]["FullName"].ToString();
        }
        // Purchas Manager
        string commandTex = "SELECT ManagerName FROM VHRDDepartmentInfo WHERE CODE = 'ACC'";
        DataTable dtPurchaseManagerName = DataBaseAccess.SearchCommandTextDataTable(commandTex);
        if (dtPurchaseManagerName != null)
        {
          purchaseManager = dtPurchaseManagerName.Rows[0]["ManagerName"].ToString();
        }
        // PrintDate
        PrintDate = DBConvert.ParseString(DateTime.Now, DaiCo.Shared.Utility.ConstantClass.FORMAT_DATETIME) + " By: " + SharedObject.UserInfo.EmpName;

        // Hide, Unhide Header, Detail(When Check RefCode)
        #region NOT USE 
        //if (chkRefCode)
        //{
        //  cpt.PageHeaderSection1.SectionFormat.EnableSuppress = true;
        //  cpt.DetailSection1.SectionFormat.EnableSuppress = true;
        //  cpt.PageHeaderSection2.SectionFormat.EnableSuppress = false;
        //  cpt.DetailSection2.SectionFormat.EnableSuppress = false;
        //  if (chkNameVn)
        //  {
        //    cpt.DetailSection3.SectionFormat.EnableSuppress = true;
        //    cpt.DetailSection4.SectionFormat.EnableSuppress = false;
        //  }
        //  else
        //  {
        //    cpt.DetailSection3.SectionFormat.EnableSuppress = true;
        //    cpt.DetailSection4.SectionFormat.EnableSuppress = true;
        //  }
        //}
        //else
        //{
        //  cpt.PageHeaderSection1.SectionFormat.EnableSuppress = false;
        //  cpt.DetailSection1.SectionFormat.EnableSuppress = false;
        //  cpt.PageHeaderSection2.SectionFormat.EnableSuppress = true;
        //  cpt.DetailSection2.SectionFormat.EnableSuppress = true;
        //  if (chkNameVn)
        //  {
        //    cpt.DetailSection3.SectionFormat.EnableSuppress = false;
        //    cpt.DetailSection4.SectionFormat.EnableSuppress = true;
        //  }
        //  else
        //  {
        //    cpt.DetailSection3.SectionFormat.EnableSuppress = true;
        //    cpt.DetailSection4.SectionFormat.EnableSuppress = true;
        //  }
        //}
        // Hide, unhide AdditionPrice
        //if (dsSource.Tables["dtPOAdditionPrice"].Rows.Count > 0)
        //{
        //  cpt.ReportFooterSection1.SectionFormat.EnableSuppress = false;
        //}
        //else
        //{
        //  cpt.ReportFooterSection1.SectionFormat.EnableSuppress = true;
        //}
        #endregion

        // Show/Hidden Table Detail by group material
        if (printType == 1) // 1 - Đơn mua hàng vật tư
        {
          cpt.PageHeaderSection1.SectionFormat.EnableSuppress = false;
          cpt.DetailSection1.SectionFormat.EnableSuppress = false;
          cpt.PageHeaderSection3.SectionFormat.EnableSuppress = true;
          cpt.DetailSection3.SectionFormat.EnableSuppress = true;
        }
        else // 2 - Đơn mua hàng gỗ
        {
          cpt.PageHeaderSection1.SectionFormat.EnableSuppress = true;
          cpt.DetailSection1.SectionFormat.EnableSuppress = true;
          cpt.PageHeaderSection3.SectionFormat.EnableSuppress = false;
          cpt.DetailSection3.SectionFormat.EnableSuppress = false;
        }
        cpt.SetDataSource(dsSource);

        cpt.SetParameterValue("address", address);
        cpt.SetParameterValue("telephone", telephone);
        cpt.SetParameterValue("email", email);
        cpt.SetParameterValue("website", website);
        cpt.SetParameterValue("taxCode", taxCode);
        cpt.SetParameterValue("accountNo", accountNo);
        cpt.SetParameterValue("fax", fax);
        cpt.SetParameterValue("companyName", companyName);
        // PurchaseManager Name
        cpt.SetParameterValue("purchaseManager", purchaseManager);
        // Total
        cpt.SetParameterValue("totalAmount", totalAmount1);
        cpt.SetParameterValue("totalVAT", totalVAT);
        cpt.SetParameterValue("total", total);
        // Order Remark
        cpt.SetParameterValue("orderRemark", orderRemark);
        // Number To English
        cpt.SetParameterValue("numberToEnglish", numberToEnglish);
        //PrintDate
        cpt.SetParameterValue("PrintDate", PrintDate);
        if (chkRefCode)
        {
          cpt.SetParameterValue("checkRefCode", 1);
        }
        else
        {
          cpt.SetParameterValue("checkRefCode", 0);
        }
        //Utility.ViewCrystalReport(cpt);

        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = false;
        report.ShowReport(DaiCo.Shared.Utility.ViewState.MainWindow);
      }
    }


    #endregion Function

    #region Event
    private void btnPrint_Click(object sender, EventArgs e)
    {
      if (ultCBCurrency.Text.ToString().Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Currrency");
        return;
      }
      this.PrintReport(this.poNO);
      //this.CloseTab();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion Event
  }
}
