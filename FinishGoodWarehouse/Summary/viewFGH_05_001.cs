/*
  Author      : Duong Minh
  Date        : 14/5/2012
  Description : Summary Monthly
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.IO;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_05_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewFGH_05_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewVEN_01_005_Load(object sender, EventArgs e)
    {
      // Load UltraCombo Month
      this.LoadComboMonth();

      // Load UltraCombo Year
      this.LoadComboYear();
    }

    /// <summary>
    /// Load UltraCombo Month
    /// </summary>
    private void LoadComboMonth()
    {
      DataTable dtMonth = new DataTable();
      dtMonth.Columns.Add("Month", typeof(System.Int16));
      for(int i = 1; i< 13; i++)
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
      for (int i = DateTime.Now.Year - 1; i < DateTime.Now.Year + 1; i++)
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
      // Information

      // Month
      if (this.ultMonth.Value != null && this.ultMonth.Value.ToString().Length > 0)
      {
        //
      }
      else
      {
        message = "Month";
        return false;
      }

      // Year
      if (this.ultMonth.Value != null && this.ultMonth.Value.ToString().Length > 0)
      {
        //
      }
      else
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

      DBParameter[] arrInputParam = new DBParameter[3];
      arrInputParam[0] = new DBParameter("@Month", DbType.Int32, DBConvert.ParseInt(this.ultMonth.Value.ToString()));
      arrInputParam[1] = new DBParameter("@Year", DbType.Int32, DBConvert.ParseInt(this.ultYear.Value.ToString()));
      arrInputParam[2] = new DBParameter("@UserID", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] arrOutputParam = new DBParameter[1];
      arrOutputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spWHFMonthlyClosingFinishGood", arrInputParam, arrOutputParam);

      int result = DBConvert.ParseInt(arrOutputParam[0].Value.ToString());
      if (result == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Exist Some Receving Note Or Some Issuing Note UnLock!");
      }
      else if (result == 1)
      {
        WindowUtinity.ShowMessageErrorFromText("Don't Do Summarize Previous Month");
      }
      else if (result == 2)
      {
        WindowUtinity.ShowMessageErrorFromText("Exist Summarize Of This Month");
      }
      else if (result == 3)
      {
        WindowUtinity.ShowMessageSuccessFromText("Summarize Sucessfully");
      }
    }
    #endregion Event
  }
}
