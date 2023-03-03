/*
 * Author       : Duong Minh
 * CreateDate   : 14/11/2013
 * Description  : Reports
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_99_003 : MainUserControl
  {
    #region Field
    int checkDepartment = int.MinValue;
    #endregion Field

    #region Init

    public viewWHD_99_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewWHD_99_003
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_99_003_Load(object sender, EventArgs e)
    {
      if (this.btnViewPrice.Visible)
      {
        this.checkDepartment = 0;
      }
      else
      {
        this.checkDepartment = 1;
      }

      if (this.checkDepartment == 1)
      {
        // Check User Login
        string commandText = string.Format(@"SELECT Code FROM VHRDDepartmentInfo WHERE Code = '{0}'", SharedObject.UserInfo.Department);
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null)
        {
          this.checkDepartment = string.Compare(dt.Rows[0]["Code"].ToString(), "ACC", true);
        }
      }

      // 
      ultDTDate.Value = DBNull.Value;
      this.LoadReport();
      this.LoadComboMonth();
      this.LoadComboYear();
      this.btnViewPrice.Visible = false;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load Type Report
    /// </summary>
    private void LoadReport()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'Ageing Material Report' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Ageing Monthly Material Report' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBReport.DataSource = dtSource;
      ultCBReport.DisplayMember = "Name";
      ultCBReport.ValueMember = "ID";
      ultCBReport.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBReport.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultCBReport.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;

      // Load Default
      ultCBReport.Value = 1;
    }

    /// <summary>
    /// Load Month
    /// </summary>
    private void LoadComboMonth()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Month", typeof(Int32));
      for (int i = 1; i < 13; i++)
      {
        DataRow dr = dt.NewRow();
        dr["Month"] = i;
        dt.Rows.Add(dr);
      }
      ultMonth.DataSource = dt;
      ultMonth.DisplayMember = "Month";
      ultMonth.ValueMember = "Month";
      ultMonth.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Year
    /// </summary>
    private void LoadComboYear()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Year", typeof(Int32));
      for (int i = 2008; i < 2099; i++)
      {
        DataRow dr = dt.NewRow();
        dr["Year"] = i;
        dt.Rows.Add(dr);
      }
      ultYear.DataSource = dt;
      ultYear.DisplayMember = "Year";
      ultYear.ValueMember = "Year";
      ultYear.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    private bool CheckValid(int value, out string message)
    {
      message = string.Empty;
      return true;
    }

    /// <summary>
    /// Ageing Material Report
    /// </summary>
    private void AgeingMaterial()
    {
      string strTemplateName = string.Empty;
      if (checkDepartment == 1)
      {
        strTemplateName = "RPT_WHD_AgeingMaterialNotAcc";
      }
      else
      {
        strTemplateName = "RPT_WHD_AgeingMaterial";
      }

      string strSheetName = "Sheet1";
      string strOutFileName = "Ageing Material";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[2];

      string materialCode = string.Empty;

      // Material Code
      if (this.txtMaterial.Text.Length > 0)
      {
        arrInput[0] = new DBParameter("@Material", DbType.String, txtMaterial.Text);
      }
      if (ultDTDate.Value != null)
      {
        arrInput[1] = new DBParameter("@Date", DbType.DateTime, DBConvert.ParseDateTime(ultDTDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTAgeingMaterials_Select", arrInput);
      if (dtData != null)
      {
        DateTime LastMonthDate;
        for (int i = 11; i > 0; i--)
        {
          LastMonthDate = DateTime.Now.AddMonths(-i);
          string commandText = string.Empty;
          commandText = string.Format(@"  SELECT IDSanPham MaterialCode, QtyOut
                                        FROM TblWHDMaterialSummary
                                        WHERE IDKho = 1 AND MONTH(NgayKet) = {0} AND YEAR(NgayKet) = {1}", LastMonthDate.Month, LastMonthDate.Year);
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

          string column1 = LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString();

          dtData.Columns.Add(column1, typeof(System.Double));
          foreach (DataRow row in dtData.Rows)
          {
            materialCode = row["MaterialCode"].ToString();
            DataRow[] foundRows = dt.Select("MaterialCode = '" + materialCode + "'");
            if (foundRows.Length == 0)
            {
              continue;
            }

            row[column1] = DBConvert.ParseDouble(foundRows[0]["QtyOut"].ToString());
          }
        }

        dtData.Columns.Add("MonthlyConsumption", typeof(System.Double));
        dtData.Columns.Add("MinOfStock", typeof(System.Double));

        for (int j = 0; j < dtData.Rows.Count; j++)
        {
          DataRow row = dtData.Rows[j];
          if (DBConvert.ParseInt(row["TrungBinhTon"].ToString()) != int.MinValue)
          {
            double sumOut = 0;
            double maxOut = 0;
            double minOut = 0;
            for (int k = 11; k > 11 - DBConvert.ParseInt(row["TrungBinhTon"].ToString()); k--)
            {
              if (DBConvert.ParseDouble(row[k + 23].ToString()) != double.MinValue)
              {
                double value = 0;
                if (DBConvert.ParseDouble(row[k + 23].ToString()) == double.MinValue)
                {
                  value = 0;
                }
                else
                {
                  value = DBConvert.ParseDouble(row[k + 23].ToString());
                }

                //Set init Default for MinOut
                if (minOut == 0)
                {
                  minOut = value;
                }

                sumOut += value;
                if (maxOut < value)
                {
                  maxOut = value;
                }

                if (minOut > value)
                {
                  minOut = value;
                }
              }
            }

            // Formula Sum
            if (DBConvert.ParseInt(row["Average"].ToString()) == 0)
            {
              row["MonthlyConsumption"] = DBConvert.ParseDouble(sumOut / DBConvert.ParseInt(row["TrungBinhTon"].ToString()));
            }
            // Formula Max
            else if (DBConvert.ParseInt(row["Average"].ToString()) == 1)
            {
              row["MonthlyConsumption"] = maxOut;
            }
            // Formula Min
            else if (DBConvert.ParseInt(row["Average"].ToString()) == 2)
            {
              row["MonthlyConsumption"] = minOut;
            }
            // By Default Min Of Stock 
            else
            {
              row["MonthlyConsumption"] = 0;
            }

            if (DBConvert.ParseInt(row["Average"].ToString()) == -1)
            {
              row["MinOfStock"] = DBConvert.ParseDouble(row["MinDefault"].ToString());
            }
            else
            {
              row["MinOfStock"] = DBConvert.ParseDouble(row["MonthlyConsumption"].ToString())
                                      * DBConvert.ParseDouble(row["HeSoTon"].ToString())
                                      * DBConvert.ParseDouble(row["MinimumStock"].ToString());
            }
            row["MinOfStock"] = Math.Round(DBConvert.ParseDouble(row["MinOfStock"].ToString()), 2);
          }
        }

        if (ultDTDate.Value == null)
        {
          oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        }
        else
        {
          oXlsReport.Cell("**Date").Value = DBConvert.ParseDateTime(ultDTDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        }
        oXlsReport.Cell("**Month").Value = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();

        if (dtData != null && dtData.Rows.Count > 0)
        {
          double total1 = 0;
          double total2 = 0;
          double total3 = 0;
          double total4 = 0;
          double total5 = 0;
          double total6 = 0;
          double total7 = 0;
          double total8 = 0;
          double total9 = 0;
          double total10 = 0;
          double total11 = 0;
          double total12 = 0;
          double total13 = 0;

          for (int i = 0; i < dtData.Rows.Count; i++)
          {
            DataRow dtRow = dtData.Rows[i];
            if (i > 0)
            {
              if (checkDepartment == 1)
              {
                oXlsReport.Cell("A7:V7").Copy();
                oXlsReport.RowInsert(6 + i);
                oXlsReport.Cell("A7:V7", 0, i).Paste();
              }
              else
              {
                oXlsReport.Cell("A7:AI7").Copy();
                oXlsReport.RowInsert(6 + i);
                oXlsReport.Cell("A7:AI7", 0, i).Paste();
              }
            }
            oXlsReport.Cell("**No", 0, i).Value = i + 1;
            oXlsReport.Cell("**Code", 0, i).Value = dtRow["MaterialCode"].ToString();
            oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
            oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
            oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
            oXlsReport.Cell("**MinMax", 0, i).Value = dtRow["Min-Max"].ToString();
            if (dtRow["HeSoTon"] != null && DBConvert.ParseDouble(dtRow["HeSoTon"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["HeSoTon"].ToString()) > 0)
            {
              oXlsReport.Cell("**LeadTime", 0, i).Value = DBConvert.ParseDouble(dtRow["HeSoTon"].ToString());
            }
            else
            {
              oXlsReport.Cell("**LeadTime", 0, i).Value = "";
            }

            if (dtRow["MinOfStock"] != null && DBConvert.ParseDouble(dtRow["MinOfStock"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["MinOfStock"].ToString()) >= 0)
            {
              oXlsReport.Cell("**SafetyStock", 0, i).Value = DBConvert.ParseDouble(dtRow["MinOfStock"].ToString());
            }
            else
            {
              oXlsReport.Cell("**SafetyStock", 0, i).Value = "";
            }

            if (dtRow["0-1 Week"] != null && DBConvert.ParseDouble(dtRow["0-1 Week"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["0-1 Week"].ToString()) > 0)
            {
              oXlsReport.Cell("**1", 0, i).Value = DBConvert.ParseDouble(dtRow["0-1 Week"].ToString());
            }
            else
            {
              oXlsReport.Cell("**1", 0, i).Value = "";
            }

            // Amount 0-1 Week
            if (dtRow["Amount 0-1 Week"] != null && DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString()) > 0)
            {
              oXlsReport.Cell("**1a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString());
            }
            else
            {
              oXlsReport.Cell("**1a", 0, i).Value = "";
            }


            if (dtRow["1-2 Weeks"] != null && DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString()) > 0)
            {
              oXlsReport.Cell("**2", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString());
            }
            else
            {
              oXlsReport.Cell("**2", 0, i).Value = "";
            }

            // Amount 1-2 Week
            if (dtRow["Amount 1-2 Week"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Week"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 1-2 Week"].ToString()) > 0)
            {
              oXlsReport.Cell("**2a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Week"].ToString());
            }
            else
            {
              oXlsReport.Cell("**2a", 0, i).Value = "";
            }

            if (dtRow["2-3 Weeks"] != null && DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString()) > 0)
            {
              oXlsReport.Cell("**3", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString());
            }
            else
            {
              oXlsReport.Cell("**3", 0, i).Value = "";
            }

            // Amount 2-3 Week
            if (dtRow["Amount 2-3 Week"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Week"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 2-3 Week"].ToString()) > 0)
            {
              oXlsReport.Cell("**3a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Week"].ToString());
            }
            else
            {
              oXlsReport.Cell("**3a", 0, i).Value = "";
            }

            if (dtRow["3-4 Weeks"] != null && DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString()) > 0)
            {
              oXlsReport.Cell("**4", 0, i).Value = DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString());
            }
            else
            {
              oXlsReport.Cell("**4", 0, i).Value = "";
            }

            // Amount 3-4 Week
            if (dtRow["Amount 3-4 Week"] != null && DBConvert.ParseDouble(dtRow["Amount 3-4 Week"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 3-4 Week"].ToString()) > 0)
            {
              oXlsReport.Cell("**4a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 3-4 Week"].ToString());
            }
            else
            {
              oXlsReport.Cell("**4a", 0, i).Value = "";
            }

            if (dtRow["1-2 Months"] != null && DBConvert.ParseDouble(dtRow["1-2 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["1-2 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**5", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**5", 0, i).Value = "";
            }

            // Amount 1-2 Months
            if (dtRow["Amount 1-2 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**5a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**5a", 0, i).Value = "";
            }

            if (dtRow["2-3 Months"] != null && DBConvert.ParseDouble(dtRow["2-3 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["2-3 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**6", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**6", 0, i).Value = "";
            }

            // Amount 2-3 Months
            if (dtRow["Amount 2-3 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**6a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**6a", 0, i).Value = "";
            }

            if (dtRow["3-6 Months"] != null && DBConvert.ParseDouble(dtRow["3-6 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["3-6 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**7", 0, i).Value = DBConvert.ParseDouble(dtRow["3-6 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**7", 0, i).Value = "";
            }

            // Amount 3-6 Months
            if (dtRow["Amount 3-6 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**7a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**7a", 0, i).Value = "";
            }

            if (dtRow["6-9 Mths"] != null && DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString()) > 0)
            {
              oXlsReport.Cell("**8", 0, i).Value = DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString());
            }
            else
            {
              oXlsReport.Cell("**8", 0, i).Value = "";
            }

            // Amount 6-9 Months
            if (dtRow["Amount 6-9 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**8a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**8a", 0, i).Value = "";
            }

            if (dtRow["9-12 Mths"] != null && DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString()) > 0)
            {
              oXlsReport.Cell("**9", 0, i).Value = DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString());
            }
            else
            {
              oXlsReport.Cell("**9", 0, i).Value = "";
            }

            // Amount 9-12 Months
            if (dtRow["Amount 9-12 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**9a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**9a", 0, i).Value = "";
            }

            if (dtRow["1-2 Years"] != null && DBConvert.ParseDouble(dtRow["1-2 Years"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["1-2 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**10", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**10", 0, i).Value = "";
            }

            // Amount 1-2 Years
            if (dtRow["Amount 1-2 Years"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**10a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**10a", 0, i).Value = "";
            }

            if (dtRow["2-3 Years"] != null && DBConvert.ParseDouble(dtRow["2-3 Years"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["2-3 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**11", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**11", 0, i).Value = "";
            }

            // Amount 2-3 Years
            if (dtRow["Amount 2-3 Years"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**11a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**11a", 0, i).Value = "";
            }

            if (dtRow["Over 3 Years"] != null && DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**12", 0, i).Value = DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**12", 0, i).Value = "";
            }

            // Amount Over 3 Years
            if (dtRow["Amount Over 3 Years"] != null && DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**12a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**12a", 0, i).Value = "";
            }

            if (dtRow["Total"] != null && DBConvert.ParseDouble(dtRow["Total"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Total"].ToString()) > 0)
            {
              oXlsReport.Cell("**13", 0, i).Value = DBConvert.ParseDouble(dtRow["Total"].ToString());
            }
            else
            {
              oXlsReport.Cell("**13", 0, i).Value = "";
            }

            // Total Amount
            if (dtRow["TotalAmount"] != null && DBConvert.ParseDouble(dtRow["TotalAmount"].ToString()) != double.MinValue
             && DBConvert.ParseDouble(dtRow["TotalAmount"].ToString()) > 0)
            {
              oXlsReport.Cell("**13a", 0, i).Value = DBConvert.ParseDouble(dtRow["TotalAmount"].ToString());
            }
            else
            {
              oXlsReport.Cell("**13a", 0, i).Value = "";
            }

            if (dtRow["0-1 Week"] != null && dtRow["0-1 Week"].ToString().Trim().Length > 0)
            {
              total1 = total1 + DBConvert.ParseDouble(dtRow["0-1 Week"].ToString());
            }
            if (dtRow["1-2 Weeks"] != null && dtRow["1-2 Weeks"].ToString().Trim().Length > 0)
            {
              total2 = total2 + DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString());
            }
            if (dtRow["2-3 Weeks"] != null && dtRow["2-3 Weeks"].ToString().Trim().Length > 0)
            {
              total3 = total3 + DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString());
            }
            if (dtRow["3-4 Weeks"] != null && dtRow["3-4 Weeks"].ToString().Trim().Length > 0)
            {
              total4 = total4 + DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString());
            }
            if (dtRow["1-2 Months"] != null && dtRow["1-2 Months"].ToString().Trim().Length > 0)
            {
              total5 = total5 + DBConvert.ParseDouble(dtRow["1-2 Months"].ToString());
            }
            if (dtRow["2-3 Months"] != null && dtRow["2-3 Months"].ToString().Trim().Length > 0)
            {
              total6 = total6 + DBConvert.ParseDouble(dtRow["2-3 Months"].ToString());
            }
            if (dtRow["3-6 Months"] != null && dtRow["3-6 Months"].ToString().Trim().Length > 0)
            {
              total7 = total7 + DBConvert.ParseDouble(dtRow["3-6 Months"].ToString());
            }
            if (dtRow["6-9 Mths"] != null && dtRow["6-9 Mths"].ToString().Trim().Length > 0)
            {
              total8 = total8 + DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString());
            }
            if (dtRow["9-12 Mths"] != null && dtRow["9-12 Mths"].ToString().Trim().Length > 0)
            {
              total9 = total9 + DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString());
            }
            if (dtRow["1-2 Years"] != null && dtRow["1-2 Years"].ToString().Trim().Length > 0)
            {
              total10 = total10 + DBConvert.ParseDouble(dtRow["1-2 Years"].ToString());
            }
            if (dtRow["2-3 Years"] != null && dtRow["2-3 Years"].ToString().Trim().Length > 0)
            {
              total11 = total11 + DBConvert.ParseDouble(dtRow["2-3 Years"].ToString());
            }
            if (dtRow["Over 3 Years"] != null && dtRow["Over 3 Years"].ToString().Trim().Length > 0)
            {
              total12 = total12 + DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString());
            }
            if (dtRow["Total"] != null && dtRow["Total"].ToString().Trim().Length > 0)
            {
              total13 = total13 + Math.Round(DBConvert.ParseDouble(dtRow["Total"].ToString()), 2);
            }
          }
          oXlsReport.Cell("**Total1").Value = total1;
          oXlsReport.Cell("**Total2").Value = total2;
          oXlsReport.Cell("**Total3").Value = total3;
          oXlsReport.Cell("**Total4").Value = total4;
          oXlsReport.Cell("**Total5").Value = total5;
          oXlsReport.Cell("**Total6").Value = total6;
          oXlsReport.Cell("**Total7").Value = total7;
          oXlsReport.Cell("**Total8").Value = total8;
          oXlsReport.Cell("**Total9").Value = total9;
          oXlsReport.Cell("**Total10").Value = total10;
          oXlsReport.Cell("**Total11").Value = total11;
          oXlsReport.Cell("**Total12").Value = total12;
          oXlsReport.Cell("**Total13").Value = total13;
        }
        oXlsReport.Out.File(strOutFileName);
        Process.Start(strOutFileName);
      }
    }

    /// <summary>
    /// Export Ageing
    /// </summary>
    private void ExportAgeingMonthly()
    {
      // Month
      if (this.ultMonth.Value == null || this.ultMonth.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Month");
        return;
      }

      // Year
      if (this.ultYear.Value == null || this.ultYear.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Year");
        return;
      }

      string commandText = string.Empty;
      commandText += " SELECT COUNT(*)";
      commandText += " FROM TblWHDMonthlySummary_Materials MS";
      commandText += " 	INNER JOIN TblWHDMonthlySummaryAgeingStore_Materials STO ON MS.PID = STO.MonthlySummaryPid";
      commandText += " WHERE [Month] = " + DBConvert.ParseInt(this.ultMonth.Value.ToString());
      commandText += "  AND [Year] = " + DBConvert.ParseInt(this.ultYear.Value.ToString());
      DataTable dtCheckMonth = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheckMonth != null && dtCheckMonth.Rows.Count > 0)
      {
        string strStartupPath = System.Windows.Forms.Application.StartupPath;
        string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
        string strPathTemplate = strStartupPath + @"\ExcelTemplate";
        string strSheetName = "Sheet1";
        string strOutFileName = "Ageing";
        XlsReport oXlsReport;

        string strTemplateName = string.Empty;

        if (checkDepartment == 1)
        {
          strTemplateName = "RPT_WHD_AgeingMonthlyMaterialNotAcc";
        }
        else
        {
          strTemplateName = "RPT_WHD_AgeingMonthlyMaterial";
        }

        oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

        //Search
        DBParameter[] arrInput = new DBParameter[3];
        int month = DBConvert.ParseInt(ultMonth.Value.ToString());
        arrInput[0] = new DBParameter("@Month", DbType.Int32, month);
        int year = DBConvert.ParseInt(ultYear.Value.ToString());
        arrInput[1] = new DBParameter("@Year", DbType.Int32, year);
        string materialCode = string.Empty;
        // Material Code
        if (this.txtMaterial.Text.Length > 0)
        {
          arrInput[2] = new DBParameter("@Material", DbType.String, txtMaterial.Text);
        }

        DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTMaterialsAgeingMonthlyStore_Select", arrInput);

        oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        oXlsReport.Cell("**Month").Value = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();

        if (dtData != null && dtData.Rows.Count > 0)
        {
          double total1 = 0;
          double total2 = 0;
          double total3 = 0;
          double total4 = 0;
          double total5 = 0;
          double total6 = 0;
          double total7 = 0;
          double total8 = 0;
          double total9 = 0;
          double total10 = 0;
          double total11 = 0;
          double total12 = 0;
          double total13 = 0;

          for (int i = 0; i < dtData.Rows.Count; i++)
          {
            DataRow dtRow = dtData.Rows[i];
            if (i > 0)
            {
              if (checkDepartment == 1)
              {
                oXlsReport.Cell("A7:U").Copy();
                oXlsReport.RowInsert(6 + i);
                oXlsReport.Cell("A7:U7", 0, i).Paste();
              }
              else
              {
                oXlsReport.Cell("A7:AH7").Copy();
                oXlsReport.RowInsert(6 + i);
                oXlsReport.Cell("A7:AH7", 0, i).Paste();
              }
            }
            oXlsReport.Cell("**No", 0, i).Value = i + 1;
            oXlsReport.Cell("**Code", 0, i).Value = dtRow["MaterialCode"].ToString();
            oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
            oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
            oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
            if (dtRow["HeSoTon"] != null && DBConvert.ParseDouble(dtRow["HeSoTon"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["HeSoTon"].ToString()) > 0)
            {
              oXlsReport.Cell("**LeadTime", 0, i).Value = DBConvert.ParseDouble(dtRow["HeSoTon"].ToString());
            }
            else
            {
              oXlsReport.Cell("**LeadTime", 0, i).Value = "";
            }

            if (dtRow["MinOfStock"] != null && DBConvert.ParseDouble(dtRow["MinOfStock"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["MinOfStock"].ToString()) >= 0)
            {
              oXlsReport.Cell("**SafetyStock", 0, i).Value = DBConvert.ParseDouble(dtRow["MinOfStock"].ToString());
            }
            else
            {
              oXlsReport.Cell("**SafetyStock", 0, i).Value = "";
            }

            if (dtRow["0-1 Week"] != null && DBConvert.ParseDouble(dtRow["0-1 Week"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["0-1 Week"].ToString()) > 0)
            {
              oXlsReport.Cell("**1", 0, i).Value = DBConvert.ParseDouble(dtRow["0-1 Week"].ToString());
            }
            else
            {
              oXlsReport.Cell("**1", 0, i).Value = "";
            }
            // Amount 0-1 Week
            if (dtRow["Amount 0-1 Week"] != null && DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString()) != double.MinValue
                 && DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString()) > 0)
            {
              oXlsReport.Cell("**1a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString());
            }
            else
            {
              oXlsReport.Cell("**1a", 0, i).Value = "";
            }

            if (dtRow["1-2 Weeks"] != null && DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString()) > 0)
            {
              oXlsReport.Cell("**2", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString());
            }
            else
            {
              oXlsReport.Cell("**2", 0, i).Value = "";
            }
            // Amount 1-2 Weeks
            if (dtRow["Amount 1-2 Weeks"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Weeks"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Amount 1-2 Weeks"].ToString()) > 0)
            {
              oXlsReport.Cell("**2a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Weeks"].ToString());
            }
            else
            {
              oXlsReport.Cell("**2a", 0, i).Value = "";
            }

            if (dtRow["2-3 Weeks"] != null && DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString()) > 0)
            {
              oXlsReport.Cell("**3", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString());
            }
            else
            {
              oXlsReport.Cell("**3", 0, i).Value = "";
            }
            // Amount 2-3 Weeks
            if (dtRow["Amount 2-3 Weeks"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Weeks"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Amount 2-3 Weeks"].ToString()) > 0)
            {
              oXlsReport.Cell("**3a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Weeks"].ToString());
            }
            else
            {
              oXlsReport.Cell("**3a", 0, i).Value = "";
            }

            if (dtRow["3-4 Weeks"] != null && DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString()) > 0)
            {
              oXlsReport.Cell("**4", 0, i).Value = DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString());
            }
            else
            {
              oXlsReport.Cell("**4", 0, i).Value = "";
            }
            // Amount 3-4 Weeks
            if (dtRow["Amount 3-4 Weeks"] != null && DBConvert.ParseDouble(dtRow["Amount 3-4 Weeks"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Amount 3-4 Weeks"].ToString()) > 0)
            {
              oXlsReport.Cell("**4a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 3-4 Weeks"].ToString());
            }
            else
            {
              oXlsReport.Cell("**4a", 0, i).Value = "";
            }

            if (dtRow["1-2 Months"] != null && DBConvert.ParseDouble(dtRow["1-2 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["1-2 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**5", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**5", 0, i).Value = "";
            }
            // Amount 1-2 Months
            if (dtRow["Amount 1-2 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString()) != double.MinValue
                 && DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**5a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**5a", 0, i).Value = "";
            }

            if (dtRow["2-3 Months"] != null && DBConvert.ParseDouble(dtRow["2-3 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["2-3 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**6", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**6", 0, i).Value = "";
            }
            // Amount 2-3 Months
            if (dtRow["Amount 2-3 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**6a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**6a", 0, i).Value = "";
            }

            if (dtRow["3-6 Months"] != null && DBConvert.ParseDouble(dtRow["3-6 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["3-6 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**7", 0, i).Value = DBConvert.ParseDouble(dtRow["3-6 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**7", 0, i).Value = "";
            }
            // Amount 3-6 Months
            if (dtRow["Amount 3-6 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**7a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**7a", 0, i).Value = "";
            }

            if (dtRow["6-9 Mths"] != null && DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString()) > 0)
            {
              oXlsReport.Cell("**8", 0, i).Value = DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString());
            }
            else
            {
              oXlsReport.Cell("**8", 0, i).Value = "";
            }
            // Amount 6-9 Months
            if (dtRow["Amount 6-9 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**8a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**8a", 0, i).Value = "";
            }

            if (dtRow["9-12 Mths"] != null && DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString()) > 0)
            {
              oXlsReport.Cell("**9", 0, i).Value = DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString());
            }
            else
            {
              oXlsReport.Cell("**9", 0, i).Value = "";
            }
            // Amount 9-12 Months
            if (dtRow["Amount 9-12 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString()) > 0)
            {
              oXlsReport.Cell("**9a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString());
            }
            else
            {
              oXlsReport.Cell("**9a", 0, i).Value = "";
            }

            if (dtRow["1-2 Years"] != null && DBConvert.ParseDouble(dtRow["1-2 Years"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["1-2 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**10", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**10", 0, i).Value = "";
            }
            // Amount 1-2 Years
            if (dtRow["Amount 1-2 Years"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**10a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**10a", 0, i).Value = "";
            }

            if (dtRow["2-3 Years"] != null && DBConvert.ParseDouble(dtRow["2-3 Years"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["2-3 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**11", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**11", 0, i).Value = "";
            }
            // Amount 2-3 Years
            if (dtRow["Amount 2-3 Years"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString()) != double.MinValue
                 && DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**11a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**11a", 0, i).Value = "";
            }


            if (dtRow["Over 3 Years"] != null && DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**12", 0, i).Value = DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**12", 0, i).Value = "";
            }
            // Amount Over 3 Years
            if (dtRow["Amount Over 3 Years"] != null && DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString()) > 0)
            {
              oXlsReport.Cell("**12a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString());
            }
            else
            {
              oXlsReport.Cell("**12a", 0, i).Value = "";
            }

            if (dtRow["Total"] != null && DBConvert.ParseDouble(dtRow["Total"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["Total"].ToString()) > 0)
            {
              oXlsReport.Cell("**13", 0, i).Value = DBConvert.ParseDouble(dtRow["Total"].ToString());
            }
            else
            {
              oXlsReport.Cell("**13", 0, i).Value = "";
            }
            // Total Amount 
            if (dtRow["TotalAmount"] != null && DBConvert.ParseDouble(dtRow["TotalAmount"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["TotalAmount"].ToString()) > 0)
            {
              oXlsReport.Cell("**13a", 0, i).Value = DBConvert.ParseDouble(dtRow["TotalAmount"].ToString());
            }
            else
            {
              oXlsReport.Cell("**13a", 0, i).Value = "";
            }


            if (dtRow["0-1 Week"] != null && dtRow["0-1 Week"].ToString().Trim().Length > 0)
            {
              total1 = total1 + DBConvert.ParseDouble(dtRow["0-1 Week"].ToString());
            }
            if (dtRow["1-2 Weeks"] != null && dtRow["1-2 Weeks"].ToString().Trim().Length > 0)
            {
              total2 = total2 + DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString());
            }
            if (dtRow["2-3 Weeks"] != null && dtRow["2-3 Weeks"].ToString().Trim().Length > 0)
            {
              total3 = total3 + DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString());
            }
            if (dtRow["3-4 Weeks"] != null && dtRow["3-4 Weeks"].ToString().Trim().Length > 0)
            {
              total4 = total4 + DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString());
            }
            if (dtRow["1-2 Months"] != null && dtRow["1-2 Months"].ToString().Trim().Length > 0)
            {
              total5 = total5 + DBConvert.ParseDouble(dtRow["1-2 Months"].ToString());
            }
            if (dtRow["2-3 Months"] != null && dtRow["2-3 Months"].ToString().Trim().Length > 0)
            {
              total6 = total6 + DBConvert.ParseDouble(dtRow["2-3 Months"].ToString());
            }
            if (dtRow["3-6 Months"] != null && dtRow["3-6 Months"].ToString().Trim().Length > 0)
            {
              total7 = total7 + DBConvert.ParseDouble(dtRow["3-6 Months"].ToString());
            }
            if (dtRow["6-9 Mths"] != null && dtRow["6-9 Mths"].ToString().Trim().Length > 0)
            {
              total8 = total8 + DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString());
            }
            if (dtRow["9-12 Mths"] != null && dtRow["9-12 Mths"].ToString().Trim().Length > 0)
            {
              total9 = total9 + DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString());
            }
            if (dtRow["1-2 Years"] != null && dtRow["1-2 Years"].ToString().Trim().Length > 0)
            {
              total10 = total10 + DBConvert.ParseDouble(dtRow["1-2 Years"].ToString());
            }
            if (dtRow["2-3 Years"] != null && dtRow["2-3 Years"].ToString().Trim().Length > 0)
            {
              total11 = total11 + DBConvert.ParseDouble(dtRow["2-3 Years"].ToString());
            }
            if (dtRow["Over 3 Years"] != null && dtRow["Over 3 Years"].ToString().Trim().Length > 0)
            {
              total12 = total12 + DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString());
            }
            if (dtRow["Total"] != null && dtRow["Total"].ToString().Trim().Length > 0)
            {
              total13 = total13 + Math.Round(DBConvert.ParseDouble(dtRow["Total"].ToString()), 2);
            }
          }
          oXlsReport.Cell("**Total1").Value = total1;
          oXlsReport.Cell("**Total2").Value = total2;
          oXlsReport.Cell("**Total3").Value = total3;
          oXlsReport.Cell("**Total4").Value = total4;
          oXlsReport.Cell("**Total5").Value = total5;
          oXlsReport.Cell("**Total6").Value = total6;
          oXlsReport.Cell("**Total7").Value = total7;
          oXlsReport.Cell("**Total8").Value = total8;
          oXlsReport.Cell("**Total9").Value = total9;
          oXlsReport.Cell("**Total10").Value = total10;
          oXlsReport.Cell("**Total11").Value = total11;
          oXlsReport.Cell("**Total12").Value = total12;
          oXlsReport.Cell("**Total13").Value = total13;
        }
        oXlsReport.Out.File(strOutFileName);
        Process.Start(strOutFileName);
      }
    }

    #endregion Function

    #region Event
    /// <summary>
    /// Export Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      int value = int.MinValue;
      if (ultCBReport.Value != null)
      {
        value = DBConvert.ParseInt(ultCBReport.Value.ToString());
        string message = string.Empty;

        // Check Valid
        bool success = this.CheckValid(value, out message);
        if (success == false)
        {
          WindowUtinity.ShowMessageError("ERR0001", message);
          return;
        }

        // Expenditure Department
        if (value == 1)
        {
          this.AgeingMaterial();
        }
        else if (value == 2)
        {
          this.ExportAgeingMonthly();
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return;
      }
    }

    private void ultCBReport_ValueChanged(object sender, EventArgs e)
    {
      int value = int.MinValue;
      if (ultCBReport.Value != null)
      {
        // To Mau Control
        labMaterial.ForeColor = System.Drawing.SystemColors.ControlText;
        labDate.ForeColor = System.Drawing.SystemColors.ControlText;
        labMonth.ForeColor = System.Drawing.SystemColors.ControlText;
        labYear.ForeColor = System.Drawing.SystemColors.ControlText;
        // End
        value = DBConvert.ParseInt(ultCBReport.Value.ToString());
        if (value == 1)
        {
          // To Mau Control
          labMaterial.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labDate.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          // End
        }
        else if (value == 2)
        {
          labMaterial.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labMonth.ForeColor = System.Drawing.SystemColors.ActiveCaption;
          labYear.ForeColor = System.Drawing.SystemColors.ActiveCaption;
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Report");
        return;
      }
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
  }
}
