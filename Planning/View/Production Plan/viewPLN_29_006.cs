/*
  Author      : Nguyen Chi Cuong
  Date        : 17/07/2015
  Description : Save Transaction And Update FurniturePid
  Standard Code: viewPLN_29_006
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_29_006 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    public DataTable dtTable = new DataTable();
    public bool flagSearch = false;

    #endregion Field

    #region Init

    public viewPLN_29_006()
    {
      InitializeComponent();
    }

    private void viewPLN_29_006_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DataTable dtCarWO = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNGetNewCarcassWO() NewCarWO");
      if ((dtCarWO != null) && (dtCarWO.Rows.Count > 0))
      {
        this.txtCarcassWo.Text = dtCarWO.Rows[0]["NewCarWO"].ToString();
        this.txtCreateBy.Text = SharedObject.UserInfo.EmpName;
        this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
      }
    }

    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      if (txtWeekFrom.Text.Length == 0 || DBConvert.ParseInt(txtWeekFrom.Text) <= 0)
      {
        message = "Week From";
        return false;
      }
      if (txtWeekTo.Text.Length == 0 || DBConvert.ParseInt(txtWeekTo.Text) <= 0)
      {
        message = "Week To";
        return false;
      }
      if (txtYear.Text.Length == 0 || DBConvert.ParseInt(txtYear.Text) <= 0)
      {
        message = "Year";
        return false;
      }
      return true;
    }

    private bool SaveData()
    {
      // Save info
      bool success = this.SaveInfo();
      if (success)
      {
        // update furniture
        success = this.SaveDetail();
      }
      else
      {
        success = false;
      }
      btnSave.Enabled = false;
      return success;
    }

    private bool SaveDetail()
    {
      bool success = true;
      SqlDBParameter[] input = new SqlDBParameter[2];
      if (this.viewPid != long.MinValue)
      {
        input[0] = new SqlDBParameter("@CarcassWOPid", SqlDbType.BigInt, this.viewPid);
      }
      input[1] = new SqlDBParameter("@DataHangtag", SqlDbType.Structured, dtTable);
      SqlDBParameter[] ouput = new SqlDBParameter[1];
      ouput[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);

      SqlDataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanTransactionFurniture_Edit", 6000, input, ouput);
      if (DBConvert.ParseLong(ouput[0].Value.ToString()) <= 0)
      {
        this.SaveSuccess = false;
        success = false;
      }
      return success;
    }

    private bool SaveInfo()
    {

      DBParameter[] inputParam = new DBParameter[7];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(this.viewPid.ToString()));
      }

      if (txtCarcassWo.Text.Length > 0)
      {
        string CarWO = txtCarcassWo.Text.Trim();
        inputParam[1] = new DBParameter("@CarWO", DbType.String, CarWO);
      }

      if (txtWeekFrom.Text.Length > 0)
      {
        string WeekFrom = txtWeekFrom.Text.Trim();
        inputParam[2] = new DBParameter("@WeekFrom", DbType.Int16, DBConvert.ParseInt(WeekFrom));
      }

      if (txtWeekTo.Text.Length > 0)
      {
        string WeekTo = txtWeekTo.Text.Trim();
        inputParam[3] = new DBParameter("@WeekTo", DbType.Int16, DBConvert.ParseInt(WeekTo));
      }

      if (txtYear.Text.Length > 0)
      {
        string Year = txtYear.Text.Trim();
        inputParam[4] = new DBParameter("@Year", DbType.Int16, DBConvert.ParseInt(Year));
      }

      inputParam[5] = new DBParameter("@CreateBy", DbType.Int64, SharedObject.UserInfo.UserPid);
      inputParam[6] = new DBParameter("@Remark", DbType.String, txtRemark.Text.ToString());

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanTransaction", inputParam, ouputParam);
      // Gan Lai Pid
      this.viewPid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if (this.viewPid == long.MinValue)
      {
        return false;
      }
      return true;
    }

    #endregion Function

    #region Event
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      bool success = this.CheckVaild(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        this.flagSearch = true;
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.CloseTab();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void txtYear_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
      {
        e.Handled = true;
      }
    }

    // 
    private void txtWeekFrom_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
      {
        e.Handled = true;
      }
    }

    private void txtWeekTo_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
      {
        e.Handled = true;
      }
    }
    #endregion Event

  }
}
