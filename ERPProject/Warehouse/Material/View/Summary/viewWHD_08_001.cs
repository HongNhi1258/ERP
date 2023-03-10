/*
  Author      : Xuan Truong
  Date        : 07/11/2012
  Description : Summary Monthly Materials
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_08_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewWHD_08_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_08_001_Load(object sender, EventArgs e)
    {
      // Load UltraCombo Month
      this.LoadComboMonth();

      // Load UltraCombo Year
      this.LoadComboYear();

      //Warehouse List
      Utility.LoadUltraCBMaterialWHListByUser(ucbWarehouse);
    }

    /// <summary>
    /// Load UltraCombo Month
    /// </summary>
    private void LoadComboMonth()
    {
      DataTable dtMonth = new DataTable();
      dtMonth.Columns.Add("Month", typeof(System.Int16));
      for (int i = 1; i < 13; i++)
      {
        DataRow row = dtMonth.NewRow();
        row["Month"] = i;
        dtMonth.Rows.Add(row);
      }

      ultMonth.DataSource = dtMonth;
      ultMonth.DisplayMember = "Month";
      ultMonth.ValueMember = "Month";
      ultMonth.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultMonth.DisplayLayout.Bands[0].Columns["Month"].Width = 120;
    }

    /// <summary>
    /// Load UltraCombo Year
    /// </summary>
    private void LoadComboYear()
    {
      DataTable dtYear = new DataTable();
      dtYear.Columns.Add("Year", typeof(System.Int16));
      for (int i = DateTime.Now.Year - 1; i < DateTime.Now.Year + 2; i++)
      {
        DataRow row = dtYear.NewRow();
        row["Year"] = i;
        dtYear.Rows.Add(row);
      }

      ultYear.DataSource = dtYear;
      ultYear.DisplayMember = "Year";
      ultYear.ValueMember = "Year";
      ultYear.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultYear.DisplayLayout.Bands[0].Columns["Year"].Width = 120;
    }
    #endregion Init

    #region LoadData
    /// <summary>
    /// Check valid before save
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      // Warehouse
      if (ucbWarehouse.SelectedRow == null)
      {
        message = "Warehouse";
        return false;
      }

      // Month
      if (this.ultMonth.Value == null || this.ultMonth.Value.ToString().Length == 0)
      {
        message = "Month";
        return false;
      }

      // Year
      if (this.ultYear.Value == null || this.ultYear.Value.ToString().Length == 0)
      {
        message = "Year";
        return false;
      }

      return true;
    }
    #endregion LoadData

    #region Event
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Summary Monthly
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSummary_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      int whPid = DBConvert.ParseInt(ucbWarehouse.Value);

      string storeName = string.Empty;
      storeName = "spWHDSummaryMonthlyMaterials_Edit";
      DBParameter[] arrInput = new DBParameter[5];
      arrInput[0] = new DBParameter("@IDKho", DbType.Int32, whPid);
      arrInput[1] = new DBParameter("@SubDealer", DbType.Int32, 0);
      DateTime datePreSummarize = DBConvert.ParseDateTime(string.Format("01/{0}/{1}", DBConvert.ParseInt(this.ultMonth.Value.ToString()), DBConvert.ParseInt(this.ultYear.Value.ToString())), ConstantClass.FORMAT_DATETIME);
      DateTime dateSummarize = datePreSummarize.AddMonths(1);
      datePreSummarize = datePreSummarize.AddSeconds(-1);
      dateSummarize = dateSummarize.AddSeconds(-1);
      arrInput[2] = new DBParameter("@SummarizeDateTime", DbType.DateTime, dateSummarize);
      arrInput[3] = new DBParameter("@PreSummarizeDateTime", DbType.DateTime, datePreSummarize);
      arrInput[4] = new DBParameter("@IDNhanVien", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] arrOutput = new DBParameter[2];
      arrOutput[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);

      DataBaseAccess.ExecuteStoreProcedure(storeName, 1000, arrInput, arrOutput);

      // @Result = 0 : Chua Clock du lieu nen khong cho tong ket
      if (DBConvert.ParseInt(arrOutput[0].Value.ToString()) == 0)
      {
        return;
      }
      // @Result = 1 : Thang truoc chua tong ket nen khong cho tong ket
      else if (DBConvert.ParseInt(arrOutput[0].Value.ToString()) == 1)
      {
        int preMonth = 0;
        int preYear = 0;
        if (DBConvert.ParseInt(ultMonth.Value.ToString()) == 1)
        {
          preMonth = 12;
          preYear = DBConvert.ParseInt(ultYear.Value.ToString()) - 1;
        }
        else
        {
          preMonth = DBConvert.ParseInt(ultMonth.Value.ToString()) - 1;
          preYear = DBConvert.ParseInt(ultYear.Value.ToString());
        }

        WindowUtinity.ShowMessageError("ERR0303", preMonth.ToString(), preYear.ToString());
        return;
      }
      // @Result = 2 : Thang nay da tong ket nen khong cho tong ket
      else if (DBConvert.ParseInt(arrOutput[0].Value.ToString()) == 2)
      {
        WindowUtinity.ShowMessageError("ERR0300", ultMonth.Value.ToString(), ultYear.Value.ToString());
        return;
      }
      // @Result = 4 : Tong ket that bai
      else if (DBConvert.ParseInt(arrOutput[0].Value.ToString()) == 4)
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
        return;
      }

      // Insert Vao TblWHDMonthlySummary_Materials, TblWHDAgeingMaterials
      storeName = "spWHDMonthlySummaryMaterials_Insert";
      int month = DBConvert.ParseInt(ultMonth.Value.ToString());
      int year = DBConvert.ParseInt(ultYear.Value.ToString());
      int createBy = Shared.Utility.SharedObject.UserInfo.UserPid;
      DBParameter[] inputParam = new DBParameter[5];
      inputParam[0] = new DBParameter("@Month", DbType.Int32, month);
      inputParam[1] = new DBParameter("@Year", DbType.Int32, year);
      inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, createBy);

      DateTime dateTimePre = DBConvert.ParseDateTime(string.Format("01/{0}/{1}", DBConvert.ParseInt(this.ultMonth.Value.ToString()), DBConvert.ParseInt(this.ultYear.Value.ToString())), ConstantClass.FORMAT_DATETIME);
      dateTimePre = dateTimePre.AddSeconds(-1);
      inputParam[3] = new DBParameter("@DateTimePre", DbType.DateTime, dateTimePre);
      inputParam[4] = new DBParameter("@WarehousePid", DbType.Int32, whPid);

      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);

      DataBaseAccess.ExecuteStoreProcedure(storeName, 1000, inputParam, outputParam);
      if (DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
        return;
      }

      // Insert TblWHDMonthlySummaryAgeingStore_Materials
      DBParameter[] ageingParam = new DBParameter[1];
      ageingParam[0] = new DBParameter("@WarehousePid", DbType.Int32, whPid);
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTAgeingMaterials_Select", 600, ageingParam);
      DateTime LastMonthDate;
      string commandText = string.Empty;
      for (int i = 11; i > 0; i--)
      {
        LastMonthDate = DateTime.Now.AddMonths(-i);
        commandText = string.Format(@"  SELECT IDSanPham MaterialCode, QtyOut
                                        FROM TblWHDMaterialSummary
                                        WHERE IDKho = {0} AND MONTH(NgayKet) = {1} AND YEAR(NgayKet) = {2}", whPid, LastMonthDate.Month, LastMonthDate.Year);
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

        string column1 = LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString();

        dtData.Columns.Add(column1, typeof(System.Double));
        foreach (DataRow row in dtData.Rows)
        {
          string materialCode = row["MaterialCode"].ToString();
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

      // Insert into TblWHDMonthlySummaryAgeingStore_Wood
      commandText = string.Format(@"SELECT PID
                                    FROM TblWHDMonthlySummary_Materials
                                    WHERE WarehousePid = {0} AND [Month] = {1} AND [Year] = {2}", whPid, ultMonth.Value, ultYear.Value);
      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      long monthlySummaryPid = 0;
      bool flag = false;
      if (dtCheck != null && dtCheck.Rows.Count == 1)
      {
        monthlySummaryPid = DBConvert.ParseLong(dtCheck.Rows[0][0].ToString());
      }
      else
      {
        flag = true;
      }

      foreach (DataRow rowInsert in dtData.Rows)
      {
        DBParameter[] inputParamInsert = new DBParameter[20];
        inputParamInsert[0] = new DBParameter("@MonthlySummaryPid", DbType.Int64, monthlySummaryPid);
        inputParamInsert[1] = new DBParameter("@MaterialCode", DbType.String, rowInsert["MaterialCode"].ToString());
        inputParamInsert[2] = new DBParameter("@NameEN", DbType.String, rowInsert["NameEN"].ToString());
        inputParamInsert[3] = new DBParameter("@NameVN", DbType.String, rowInsert["NameVN"].ToString());
        inputParamInsert[4] = new DBParameter("@Unit", DbType.String, rowInsert["Unit"].ToString());
        if (rowInsert["HeSoTon"] != null && DBConvert.ParseDouble(rowInsert["HeSoTon"].ToString()) != double.MinValue)
        {
          inputParamInsert[5] = new DBParameter("@LeadTime", DbType.Double, DBConvert.ParseDouble(rowInsert["HeSoTon"].ToString()));
        }
        if (rowInsert["MinOfStock"] != null && DBConvert.ParseDouble(rowInsert["MinOfStock"].ToString()) != double.MinValue)
        {
          inputParamInsert[6] = new DBParameter("@SafetyStock", DbType.Double, DBConvert.ParseDouble(rowInsert["MinOfStock"].ToString()));
        }

        if (rowInsert["0-1 Week"] != null && DBConvert.ParseDouble(rowInsert["0-1 Week"].ToString()) != double.MinValue)
        {
          inputParamInsert[7] = new DBParameter("@1Week", DbType.Double, DBConvert.ParseDouble(rowInsert["0-1 Week"].ToString()));
        }

        if (rowInsert["1-2 Weeks"] != null && DBConvert.ParseDouble(rowInsert["1-2 Weeks"].ToString()) != double.MinValue)
        {
          inputParamInsert[8] = new DBParameter("@2Week", DbType.Double, DBConvert.ParseDouble(rowInsert["1-2 Weeks"].ToString()));
        }

        if (rowInsert["2-3 Weeks"] != null && DBConvert.ParseDouble(rowInsert["2-3 Weeks"].ToString()) != double.MinValue)
        {
          inputParamInsert[9] = new DBParameter("@3Week", DbType.Double, DBConvert.ParseDouble(rowInsert["2-3 Weeks"].ToString()));
        }

        if (rowInsert["3-4 Weeks"] != null && DBConvert.ParseDouble(rowInsert["3-4 Weeks"].ToString()) != double.MinValue)
        {
          inputParamInsert[10] = new DBParameter("@4Week", DbType.Double, DBConvert.ParseDouble(rowInsert["3-4 Weeks"].ToString()));
        }

        if (rowInsert["1-2 Months"] != null && DBConvert.ParseDouble(rowInsert["1-2 Months"].ToString()) != double.MinValue)
        {
          inputParamInsert[11] = new DBParameter("@2Month", DbType.Double, DBConvert.ParseDouble(rowInsert["1-2 Months"].ToString()));
        }

        if (rowInsert["2-3 Months"] != null && DBConvert.ParseDouble(rowInsert["2-3 Months"].ToString()) != double.MinValue)
        {
          inputParamInsert[12] = new DBParameter("@3Month", DbType.Double, DBConvert.ParseDouble(rowInsert["2-3 Months"].ToString()));
        }

        if (rowInsert["3-6 Months"] != null && DBConvert.ParseDouble(rowInsert["3-6 Months"].ToString()) != double.MinValue)
        {
          inputParamInsert[13] = new DBParameter("@6Month", DbType.Double, DBConvert.ParseDouble(rowInsert["3-6 Months"].ToString()));
        }

        if (rowInsert["6-9 Mths"] != null && DBConvert.ParseDouble(rowInsert["6-9 Mths"].ToString()) != double.MinValue)
        {
          inputParamInsert[14] = new DBParameter("@9Month", DbType.Double, DBConvert.ParseDouble(rowInsert["6-9 Mths"].ToString()));
        }

        if (rowInsert["9-12 Mths"] != null && DBConvert.ParseDouble(rowInsert["9-12 Mths"].ToString()) != double.MinValue)
        {
          inputParamInsert[15] = new DBParameter("@12Month", DbType.Double, DBConvert.ParseDouble(rowInsert["9-12 Mths"].ToString()));
        }

        if (rowInsert["1-2 Years"] != null && DBConvert.ParseDouble(rowInsert["1-2 Years"].ToString()) != double.MinValue)
        {
          inputParamInsert[16] = new DBParameter("@2Year", DbType.Double, DBConvert.ParseDouble(rowInsert["1-2 Years"].ToString()));
        }

        if (rowInsert["2-3 Years"] != null && DBConvert.ParseDouble(rowInsert["2-3 Years"].ToString()) != double.MinValue)
        {
          inputParamInsert[17] = new DBParameter("@3Year", DbType.Double, DBConvert.ParseDouble(rowInsert["2-3 Years"].ToString()));
        }

        if (rowInsert["Over 3 Years"] != null && DBConvert.ParseDouble(rowInsert["Over 3 Years"].ToString()) != double.MinValue)
        {
          inputParamInsert[18] = new DBParameter("@Over3Year", DbType.Double, DBConvert.ParseDouble(rowInsert["Over 3 Years"].ToString()));
        }

        if (rowInsert["Total"] != null && DBConvert.ParseDouble(rowInsert["Total"].ToString()) != double.MinValue)
        {
          inputParamInsert[19] = new DBParameter("@Total", DbType.Double, DBConvert.ParseDouble(rowInsert["Total"].ToString()));
        }

        DBParameter[] outputParamInsert = new DBParameter[1];
        outputParamInsert[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spWHDMonthlySummaryAgeingStoreMaterials_Insert", 1000, inputParamInsert, outputParamInsert);
        long resultPid = DBConvert.ParseLong(outputParamInsert[0].Value.ToString());
        if (resultPid == 0)
        {
          flag = true;
          break;
        }
      }
      if (flag == true)
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
        return;
      }
      // @Result = 3 : Thanh Cong
      else if (DBConvert.ParseInt(arrOutput[0].Value.ToString()) == 3)
      {
        WindowUtinity.ShowMessageSuccess("MSG0047");
        return;
      }
    }
    #endregion Event
  }
}
