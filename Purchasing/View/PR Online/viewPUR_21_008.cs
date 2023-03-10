/*
  Author      : 
  Date        : 16/07/2013
  Description : List PO Cancel
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Purchasing.DataSetSource;
using Purchasing.Reports;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_21_008 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init
    public viewPUR_21_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_21_008_Load(object sender, EventArgs e)
    {
      drpDateFrom.Value = DateTime.Today.AddDays(-7);
      // Load Init
      this.LoadInit();
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      this.LoadComboCreateBy();
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Resigned = 0 AND Department = 'PUR'";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBRequestBy.DataSource = dtSource;
      ultCBRequestBy.DisplayMember = "Name";
      ultCBRequestBy.ValueMember = "ID_NhanVien";
      ultCBRequestBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBRequestBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
      ultCBRequestBy.DisplayLayout.AutoFitColumns = true;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[6];
      // PONo From
      if (txtPROnlineFrom.Text.Length > 0)
      {
        input[0] = new DBParameter("@PONoFrom", DbType.String, txtPROnlineFrom.Text.Trim());
      }
      // PONo To
      if (txtPROnlineTo.Text.Length > 0)
      {
        input[1] = new DBParameter("@PONoTo", DbType.String, txtPROnlineTo.Text.Trim());
      }
      // DateFrom
      if (drpDateFrom.Value != null)
      {
        DateTime prDateFrom = DateTime.MinValue;
        prDateFrom = (DateTime)drpDateFrom.Value;
        input[2] = new DBParameter("@DateFrom", DbType.DateTime, prDateFrom);
      }
      // DateTo
      if (drpDateTo.Value != null)
      {
        DateTime prDateTo = DateTime.MinValue;
        prDateTo = (DateTime)drpDateTo.Value;
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        input[3] = new DBParameter("@DateTo", DbType.DateTime, prDateTo);
      }
      // CreateBy
      if (ultCBRequestBy.Value != null)
      {
        input[4] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(ultCBRequestBy.Value.ToString()));
      }

      // Material
      if (txtMaterial.Text.Trim().Length > 0)
      {
        input[5] = new DBParameter("@Material", DbType.String, txtMaterial.Text.Trim());
      }

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPURListPOCancelInformation_Select", input);
      if (dt != null)
      {
        ultData.DataSource = dt;
      }

      // Enable button search
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Enter Search
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      txtPROnlineFrom.Text = string.Empty;
      txtPROnlineTo.Text = string.Empty;
      ultCBRequestBy.Text = string.Empty;
      txtMaterial.Text = string.Empty;
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].DefaultCellValue = 0;
      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event

    private void btnPrint_Click(object sender, EventArgs e)
    {
      int count = 0;
      // Check Valid
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          count = count + 1;
          if (count > 1)
          {
            WindowUtinity.ShowMessageError("ERR0115", "One PO");
            return;
          }
        }
      }
      // Printf
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          string poNo = row.Cells["PONo"].Value.ToString();
          this.PrintReport(poNo);
        }
      }
    }

    /// <summary>
    /// Print Report
    /// </summary>
    /// <param name="poNo"></param>
    private void PrintReport(string poNo)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@PONo", DbType.AnsiString, 16, poNo);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPOInfomation_Select", input);
      if (ds != null)
      {
        dsPURPOInfomation dsSource = new dsPURPOInfomation();
        dsSource.Tables["dtPOInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtPODetail"].Merge(ds.Tables[1]);
        dsSource.Tables["dtPOAdditionPrice"].Merge(ds.Tables[2]);
        dsSource.Tables["dtListPR"].Merge(ds.Tables[3]);

        DaiCo.Shared.View_Report report = null;
        cptPURPOInformation cpt = new cptPURPOInformation();
        double totalAmount = 0;
        double totalAdditionPrice = 0;
        double totalVAT = 0;
        double total = 0;
        string orderRemark = string.Empty;
        // Total Amount
        for (int i = 0; i < dsSource.Tables["dtPODetail"].Rows.Count; i++)
        {
          DataRow row = dsSource.Tables["dtPODetail"].Rows[i];
          totalAmount = totalAmount + DBConvert.ParseDouble(row["Amount"].ToString());
          if (row["VAT"].ToString().Length > 0)
          {
            totalVAT = totalVAT + ((DBConvert.ParseDouble(row["Amount"].ToString()) * DBConvert.ParseDouble(row["VAT"].ToString())) / 100);
          }
        }
        // Total Addition Price
        for (int j = 0; j < dsSource.Tables["dtPOAdditionPrice"].Rows.Count; j++)
        {
          DataRow row = dsSource.Tables["dtPOAdditionPrice"].Rows[j];
          totalAdditionPrice = totalAdditionPrice + DBConvert.ParseDouble(row["Amount"].ToString());
        }
        // Total
        total = totalAmount + totalAdditionPrice + totalVAT;
        // Number To English
        string numberToEnglish = Shared.Utility.NumberToEnglish.ChangeNumericToWords(total) + "(" + dsSource.Tables["dtPOInfo"].Rows[0]["Currency"].ToString() + ")";
        // Order Remark
        if (dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString().Length > 0)
        {
          DateTime date = DateTime.MinValue;
          DateTime dateHtml = DateTime.MinValue;
          date = DBConvert.ParseDateTime(dsSource.Tables["dtPOInfo"].Rows[0]["CreateDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
          string commandText1 = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 9009";
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText1);
          if (dt != null)
          {
            dateHtml = DBConvert.ParseDateTime(dt.Rows[0]["Value"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
          }
          if (date >= dateHtml)
          {
            orderRemark = dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString();
          }
          else
          {
            orderRemark = this.StripHTML(dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString());
          }
          cpt.ReportFooterSection3.SectionFormat.EnableSuppress = false;
        }
        else
        {
          cpt.ReportFooterSection3.SectionFormat.EnableSuppress = true;
        }

        // Remark Detail
        if (dsSource.Tables["dtPOInfo"].Rows[0]["RemarkDetailEN"].ToString().Length > 0 ||
            dsSource.Tables["dtPOInfo"].Rows[0]["RemarkDetailVN"].ToString().Length > 0)
        {
          cpt.ReportFooterSection6.SectionFormat.EnableSuppress = false;
        }
        else
        {
          cpt.ReportFooterSection6.SectionFormat.EnableSuppress = true;
        }

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

        string commandText = "SELECT Code, ISNULL([Description], '') [Description] FROM TblBOMCodeMaster WHERE [Group] = 9008";
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtSource != null)
        {
          address = dtSource.Rows[0]["Description"].ToString();
          telephone = dtSource.Rows[1]["Description"].ToString();
          fax = dtSource.Rows[2]["Description"].ToString();
          email = dtSource.Rows[3]["Description"].ToString();
          website = dtSource.Rows[4]["Description"].ToString();
          taxCode = dtSource.Rows[5]["Description"].ToString();
          accountNo = dtSource.Rows[6]["Description"].ToString();
          companyName = dtSource.Rows[7]["Description"].ToString();
        }
        // Purchas Manager
        string commandTex = "SELECT ManagerName FROM VHRDDepartmentInfo WHERE CODE = 'PUR'";
        DataTable dtPurchaseManagerName = DataBaseAccess.SearchCommandTextDataTable(commandTex);
        if (dtPurchaseManagerName != null)
        {
          purchaseManager = dtPurchaseManagerName.Rows[0]["ManagerName"].ToString();
        }
        // PrintDate
        PrintDate = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME) + " By: " + SharedObject.UserInfo.EmpName;
        // Hide, Unhide Header, Detail(When Check RefCode)
        if (chkRefCode.Checked)
        {
          cpt.PageHeaderSection1.SectionFormat.EnableSuppress = true;
          cpt.DetailSection1.SectionFormat.EnableSuppress = true;
          cpt.PageHeaderSection2.SectionFormat.EnableSuppress = false;
          cpt.DetailSection2.SectionFormat.EnableSuppress = false;
          if (chkNameVn.Checked)
          {
            cpt.DetailSection3.SectionFormat.EnableSuppress = true;
            cpt.DetailSection4.SectionFormat.EnableSuppress = false;
          }
          else
          {
            cpt.DetailSection3.SectionFormat.EnableSuppress = true;
            cpt.DetailSection4.SectionFormat.EnableSuppress = true;
          }
        }
        else
        {
          cpt.PageHeaderSection1.SectionFormat.EnableSuppress = false;
          cpt.DetailSection1.SectionFormat.EnableSuppress = false;
          cpt.PageHeaderSection2.SectionFormat.EnableSuppress = true;
          cpt.DetailSection2.SectionFormat.EnableSuppress = true;
          if (chkNameVn.Checked)
          {
            cpt.DetailSection3.SectionFormat.EnableSuppress = false;
            cpt.DetailSection4.SectionFormat.EnableSuppress = true;
          }
          else
          {
            cpt.DetailSection3.SectionFormat.EnableSuppress = true;
            cpt.DetailSection4.SectionFormat.EnableSuppress = true;
          }
        }
        // Hide, unhide AdditionPrice
        if (dsSource.Tables["dtPOAdditionPrice"].Rows.Count > 0)
        {
          cpt.ReportFooterSection1.SectionFormat.EnableSuppress = false;
        }
        else
        {
          cpt.ReportFooterSection1.SectionFormat.EnableSuppress = true;
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
        cpt.SetParameterValue("totalAmount", totalAmount);
        cpt.SetParameterValue("totalVAT", totalVAT);
        cpt.SetParameterValue("total", total);
        // Order Remark
        cpt.SetParameterValue("orderRemark", orderRemark);
        // Number To English
        cpt.SetParameterValue("numberToEnglish", numberToEnglish);
        //PrintDate
        cpt.SetParameterValue("PrintDate", PrintDate);
        if (chkRefCode.Checked)
        {
          cpt.SetParameterValue("checkRefCode", 1);
        }
        else
        {
          cpt.SetParameterValue("checkRefCode", 0);
        }
        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = false;
        report.ShowReport(Shared.Utility.ViewState.MainWindow);
      }
    }

    private string StripHTML(string source)
    {
      try
      {
        string result;

        // Remove HTML Development formatting
        // Replace line breaks with space
        // because browsers inserts space
        result = source.Replace("\r", " ");
        // Replace line breaks with space
        // because browsers inserts space
        result = result.Replace("\n", " ");
        // Remove step-formatting
        result = result.Replace("\t", string.Empty);
        // Remove repeating spaces because browsers ignore them
        result = System.Text.RegularExpressions.Regex.Replace(result,
                                                              @"( )+", " ");

        // Remove the header (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*head([^>])*>", "<head>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<( )*(/)( )*head( )*>)", "</head>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(<head>).*(</head>)", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // remove all scripts (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*script([^>])*>", "<script>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<( )*(/)( )*script( )*>)", "</script>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //result = System.Text.RegularExpressions.Regex.Replace(result,
        //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
        //         string.Empty,
        //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<script>).*(</script>)", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // remove all styles (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*style([^>])*>", "<style>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<( )*(/)( )*style( )*>)", "</style>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(<style>).*(</style>)", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert tabs in spaces of <td> tags
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*td([^>])*>", "\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert line breaks in places of <BR> and <LI> tags
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*br( )*>", "\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*li( )*>", "\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert line paragraphs (double line breaks) in place
        // if <P>, <DIV> and <TR> tags
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*div([^>])*>", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*tr([^>])*>", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*p([^>])*>", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Remove remaining tags like <a>, links, images,
        // comments etc - anything that's enclosed inside < >
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<[^>]*>", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // replace special characters:
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @" ", " ",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&bull;", " * ",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&lsaquo;", "<",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&rsaquo;", ">",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&trade;", "(tm)",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&frasl;", "/",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&lt;", "<",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&gt;", ">",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&copy;", "(c)",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&reg;", "(r)",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove all others. More can be added, see
        // http://hotwired.lycos.com/webmonkey/reference/special_characters/
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&(.{2,6});", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // for testing
        //System.Text.RegularExpressions.Regex.Replace(result,
        //       this.txtRegex.Text,string.Empty,
        //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // make line breaking consistent
        result = result.Replace("\n", "\r");

        // Remove extra line breaks and tabs:
        // replace over 2 breaks with 2 and over 4 tabs with 4.
        // Prepare first to remove any whitespaces in between
        // the escaped characters and remove redundant tabs in between line breaks
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)( )+(\r)", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\t)( )+(\t)", "\t\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\t)( )+(\r)", "\t\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)( )+(\t)", "\r\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove redundant tabs
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)(\t)+(\r)", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove multiple tabs following a line break with just one tab
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)(\t)+", "\r\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Initial replacement target string for line breaks
        string breaks = "\r\r\r";
        // Initial replacement target string for tabs
        string tabs = "\t\t\t\t\t";
        for (int index = 0; index < result.Length; index++)
        {
          result = result.Replace(breaks, "\r\r");
          result = result.Replace(tabs, "\t\t\t\t");
          breaks = breaks + "\r";
          tabs = tabs + "\t";
        }

        // That's it.
        return result;
      }
      catch
      {
        MessageBox.Show("Error");
        return source;
      }
    }
  }
}
