/*
  Author      : 
  Date        : 29/07/2013
  Description : Save Remark Detail
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;

namespace Purchasing
{
  public partial class viewPUR_04_006 : MainUserControl
  {
    #region Field
    public string poNo = string.Empty;
    public int status = int.MinValue;
    #endregion Field

    #region Init
    public viewPUR_04_006()
    {
      InitializeComponent();
    }

    private void viewPUR_04_006_Load(object sender, EventArgs e)
    {
      this.LoadData();
      // Set Status Control
      //if (this.status >= 5)
      //{
      //  btnSave.Enabled = false;
      //}
    }
    #endregion Init

    #region Function
    private void LoadData()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@PONo", DbType.String, this.poNo);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPURPOInfomationRemarkDetail_Select", input);
      if (dtSource != null)
      {
        txtRemarkDetailEn.Text = dtSource.Rows[0]["RemarkDetailEN"].ToString();
        txtRemarkDetailVn.Text = dtSource.Rows[0]["RemarkDetailVN"].ToString();
      }
    }

    private bool SaveData()
    {
      DBParameter[] input = new DBParameter[4];
      input[0] = new DBParameter("@PONo", DbType.String, this.poNo);
      input[1] = new DBParameter("@RemarkDetailEN", DbType.String, txtRemarkDetailEn.Text);
      input[2] = new DBParameter("@RemarkDetailVN", DbType.String, txtRemarkDetailVn.Text);
      input[3] = new DBParameter("UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPURPOInformationRemarkDetail_Update", input, output);
      int result = DBConvert.ParseInt(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }
    #endregion Function

    #region Event
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0001");
      }
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
