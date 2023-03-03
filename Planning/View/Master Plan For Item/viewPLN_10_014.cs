/*
  Author      : TRAN HUNG
  Date        : 15/01/2013
  Description : Import Data 
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_10_014 : MainUserControl
  {
    #region Init
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public DataTable dt = new DataTable();
    public viewPLN_10_014()
    {
      InitializeComponent();
    }
    #endregion Init

    #region Event
    private string department(int eid)
    {
      string commandText = string.Format(@"SELECT Department FROM VHRMEmployee  WHERE Pid = {0}", eid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      return dt.Rows[0]["Department"].ToString();
    }

    private bool CheckIsValid(out string message)
    {
      message = string.Empty;
      bool result = true;
      if (ultData.Rows.Count > 0)
      {
        string strTeam = string.Empty;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.White;
          // Check Colum Isvaild 
          if (department(SharedObject.UserInfo.UserPid) == "COM1")
          {
            if (DBConvert.ParseLong(ultData.Rows[i].Cells["WorkOrder"].Value.ToString()) <= 0 || DBConvert.ParseInt(ultData.Rows[i].Cells["Qty"].Value.ToString()) <= 0 || DBConvert.ParseDouble(ultData.Rows[i].Cells["Manhour"].Value.ToString()) <= 0 || DBConvert.ParseDouble(ultData.Rows[i].Cells["Minutes"].Value.ToString()) <= 0 || DBConvert.ParseDouble(ultData.Rows[i].Cells["Processing"].Value.ToString()) <= 0)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0009"));
              result = false;
            }
          }
          else
          {
            if (DBConvert.ParseLong(ultData.Rows[i].Cells["WorkOrder"].Value.ToString()) <= 0 || DBConvert.ParseInt(ultData.Rows[i].Cells["Qty"].Value.ToString()) <= 0 || DBConvert.ParseDouble(ultData.Rows[i].Cells["Manhour"].Value.ToString()) <= 0)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0009"));
              result = false;
            }
          }
          // Check Team 
          strTeam += "," + ultData.Rows[i].Cells["Team"].Value.ToString().Trim();
        }
        strTeam = strTeam + ",";
        DBParameter[] inputParam = new DBParameter[2];
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.String, 4000, "") };
        inputParam[0] = new DBParameter("@strTeam", DbType.String, 4000, strTeam);
        inputParam[1] = new DBParameter("Department", DbType.AnsiString, 32, department(SharedObject.UserInfo.UserPid));
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNManhourAllocation_CheckIsValid", inputParam, outputParam);
        string outValue = outputParam[0].Value.ToString();
        if (outValue.Length > 1)
        {
          for (int j = 0; j < ultData.Rows.Count; j++)
          {
            if (outValue.IndexOf(ultData.Rows[j].Cells["Team"].Value.ToString()) != -1)
            {
              ultData.Rows[j].CellAppearance.BackColor = Color.Yellow;
            }
          }
          message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0011"), "Team");
          result = false;
        }
      }
      else
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0007"));
        result = false;
      }
      return result;
    }

    private void viewPLN_10_014_Load(object sender, EventArgs e)
    {
      ultData.DataSource = dt;
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Bands[0].Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WorkOrder"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Manhour"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = department(SharedObject.UserInfo.UserPid) == "COM2" ? "Product Code" : "Carcass code";
      e.Layout.Bands[0].Columns["WCP"].Hidden = department(SharedObject.UserInfo.UserPid) == "COM1" ? false : true;
      e.Layout.Bands[0].Columns["Minutes"].Hidden = department(SharedObject.UserInfo.UserPid) == "COM1" ? false : true;
      e.Layout.Bands[0].Columns["Processing"].Hidden = department(SharedObject.UserInfo.UserPid) == "COM1" ? false : true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }
      string Out = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        DBParameter[] inputParam = new DBParameter[12];
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        inputParam[0] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        inputParam[1] = new DBParameter("@TeamCode", DbType.AnsiString, 32, ultData.Rows[i].Cells["Team"].Value.ToString().Trim());
        inputParam[2] = new DBParameter("@WDate", DbType.DateTime, DBConvert.ParseDateTime(ultData.Rows[i].Cells["Date"].Value.ToString(), formatConvert));
        inputParam[3] = new DBParameter("@Wo", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["WorkOrder"].Value.ToString()));
        inputParam[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 32, ultData.Rows[i].Cells["CarcassCode"].Value.ToString());
        inputParam[5] = new DBParameter("@Component", DbType.AnsiString, 32, ultData.Rows[i].Cells["Component"].Value.ToString());
        if (ultData.Rows[i].Cells["WCP"].Value.ToString().Length > 0)
        {
          inputParam[6] = new DBParameter("@WCP", DbType.String, 500, ultData.Rows[i].Cells["WCP"].Value.ToString());
        }
        inputParam[7] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["Qty"].Value.ToString()));
        if (DBConvert.ParseDouble(ultData.Rows[i].Cells["Minutes"].Value.ToString()) > 0)
        {
          inputParam[8] = new DBParameter("@Minutes", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].Cells["Minutes"].Value.ToString()));
        }
        if (DBConvert.ParseDouble(ultData.Rows[i].Cells["Processing"].Value.ToString()) > 0)
        {
          inputParam[9] = new DBParameter("@Processing", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].Cells["Processing"].Value.ToString()));
        }
        inputParam[10] = new DBParameter("@Manhour", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].Cells["Manhour"].Value.ToString()));
        inputParam[11] = new DBParameter("@Remark", DbType.String, 500, ultData.Rows[i].Cells["Remark"].Value.ToString());
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNManhourAllocation_Insert", inputParam, outputParam);
        long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
        Out += "," + outValue.ToString();
        if (outValue == -1 || outValue == 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
      }
      if (Out.IndexOf("-1") == -1 && Out.IndexOf("-2") == -1)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        for (int a = 0; a < ultData.Rows.Count; a++)
        {
          ultData.Rows[a].CellAppearance.BackColor = Color.White;
          this.btnSave.Visible = false;
        }
      }
    }
    #endregion Event
  }
}
