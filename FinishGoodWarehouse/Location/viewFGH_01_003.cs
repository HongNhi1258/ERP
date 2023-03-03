using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using System.Collections;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Diagnostics;
using System.IO;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_01_003 : MainUserControl
  {
    #region Field
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool checkDelete = false;
    private bool checkDuplicate = false;
    #endregion Field

    #region Init
    public viewFGH_01_003()
    {
      InitializeComponent();
    }

    private void viewFGH_01_003_Load(object sender, EventArgs e)
    {
      this.LoadLocation();
    }

    private void LoadLocation()
    {
      string commandText = "SELECT ROW_NUMBER() OVER (ORDER BY Pid DESC) [No], Pid, Location, Remark FROM TblWHFLocation";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultGrid.DataSource = dt;
    }
    #endregion Init

    #region Event
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      ultGrid.DisplayLayout.AutoFitColumns = true;
      ultGrid.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultGrid.DisplayLayout.Bands[0].Columns["No"].CellActivation = Activation.ActivateOnly;
      ultGrid.DisplayLayout.Bands[0].Columns["No"].MaxWidth = 50;
      ultGrid.DisplayLayout.Bands[0].Columns["No"].MinWidth = 50;
      ultGrid.DisplayLayout.Bands[0].Columns["Location"].MaxWidth = 250;
      ultGrid.DisplayLayout.Bands[0].Columns["Location"].MinWidth = 250;
      ultGrid.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool check = this.Save();
      message = string.Empty;
      if (check)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        if (this.checkDelete)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0054"), "Location");
          WindowUtinity.ShowMessageErrorFromText(message);
        }
        if (this.checkDuplicate)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Location");
          WindowUtinity.ShowMessageErrorFromText(message);
        }
      }
      this.LoadLocation();
    }

    private void ultGrid_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    private void ultGrid_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    private void ultGrid_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      int count = 0;
      if (colName == "location")
      {
        for (int i = 0; i < ultGrid.Rows.Count; i++)
        {
          if (e.Cell.Row.Cells["Location"].Text == ultGrid.Rows[i].Cells["Location"].Text)
          {
            count++;
            if (count == 2)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Location");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
              break;
            }
          }
        }
      }
    }

    private void ultGrid_BeforeRowUpdate(object sender, CancelableRowEventArgs e)
    {
      string message = string.Empty;
      if (e.Row.Cells["Location"].Text.Trim().Length == 0)
      {
        e.Cancel = true;
      }
    }
    #endregion Event

    #region Process
    private bool Save()
    {
      //delete Location
      if (this.listDeletedPid != null)
      {
        foreach (long pid in this.listDeletedPid)
        {
          DBParameter[] inputParamDelete = new DBParameter[1];
          inputParamDelete[0] = new DBParameter("@PID", DbType.Int64, pid);

          DataBaseAccess.ExecuteStoreProcedure("spWHFLocation_Delete", inputParamDelete);
        }
      }

      // insert update
      DataTable dt = (DataTable)ultGrid.DataSource;
      if (dt != null)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i].RowState == DataRowState.Modified || dt.Rows[i].RowState == DataRowState.Added)
          {
            if (dt.Rows[i]["Location"].ToString().Trim().Length > 0)
            {
              string storename = string.Empty;
              long locationPid = DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString());

              DBParameter[] inputParam = new DBParameter[4];
              if (locationPid != long.MinValue)
              {
                storename = "spWHFLocation_Update";
                inputParam[0] = new DBParameter("@Pid", DbType.Int64, locationPid);
                inputParam[1] = new DBParameter("@UserWH", DbType.Int32, SharedObject.UserInfo.UserPid);
              }
              else 
              {
                storename = "spWHFLocation_Insert";
                inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              }
              inputParam[2] = new DBParameter("@Location", DbType.AnsiString, 256, dt.Rows[i]["Location"].ToString());
              if (locationPid != long.MinValue)
              {
                inputParam[3] = new DBParameter("@Note", DbType.String, 256, dt.Rows[i]["Remark"].ToString());
              }
              else 
              {
                inputParam[3] = new DBParameter("@Remark", DbType.String, 256, dt.Rows[i]["Remark"].ToString());
              }

              DBParameter[] outParam = new DBParameter[1];
              if (locationPid != long.MinValue)
              {
                outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              }
              else 
              {
                outParam[0] = new DBParameter("@Pid", DbType.Int64, long.MinValue);
              }
              DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outParam);
              long outResult = DBConvert.ParseLong(outParam[0].Value.ToString());
              if (outResult == 0 && outResult == long.MinValue)
              {
                return false;
              }
              if (outResult == 2)
              {
                this.checkDuplicate = true;
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    private bool ValidationInput(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultGrid.Rows.Count; i++)
      {
        if (ultGrid.Rows[i].Cells["Location"].Text.Trim().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Location");
          return false;
        }
      }
      return true;
    }
    #endregion Process
  }
}
