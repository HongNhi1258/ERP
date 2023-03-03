/*
  Author      : 
  Date        : 
  Description : 
  Standard Code: view_MasterDetail.cs
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;

namespace DaiCo.General
{
  public partial class viewGNR_05_001 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private int status = int.MinValue;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init

    public viewGNR_05_001()
    {
      InitializeComponent();
    }

    private void viewGNR_05_001_Load(object sender, EventArgs e)
    {
      this.LoadInit();
      this.LoadData();
      this.SetStatusControl();
    }

    #endregion Init

    #region Function

    private void LoadInit()
    {
      //this.LoadUltraComboDepartment(ultraCBDepartment);
    }

    //private void LoadUltraComboDepartment(UltraCombo ultraCBDepartment)
    //{
    //  string commandText = "SELECT Department, DeparmentName, Department + ' | ' + DeparmentName Display FROM VHRDDepartment ORDER BY Department";
    //  DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
    //  ControlUtility.LoadUltraCombo(ultraCBDepartment, dtSource, "Department", "Display", "Display");
    //}

    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("", inputParam);
      if (dsSource != null)
      {
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          this.status = DBConvert.ParseInt(row["Status"].ToString());
          //Code
          //Code
          if (this.status > 0)
          {
            chkConfirm.Checked = true;
          }
        }
        else
        {
          //DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewReceivingNo('01ADI') NewReceivingNo");
          //if ((dtCode != null) && (dtCode.Rows.Count == 1))
          //{
          //  this.txtReceivingNote.Text = dtCode.Rows[0]["NewReceivingNo"].ToString();
          //  this.txtDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          //  this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          //}
        }

        // Load Detail
        ultData.DataSource = dsSource.Tables[1];
        // Set Status Control
        this.SetStatusControl();
      }
    }

    private void SetStatusControl()
    {
      
    }

    private bool CheckVaild(out string message)
    {
      message = string.Empty;

      // Check Info

      // Check Detail

      return true;
    }

    private bool SaveData()
    {
      // Save master info
      bool success = this.SaveInfo();
      if (success)
      {
        // Save detail
        success = this.SaveDetail();
      }
      else
      {
        success = false;
      }
      return success;
    }

    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[1];
      if (this.pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }

      //Code
      //Code

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("", inputParam, ouputParam);
      // Gan Lai Pid
      this.pid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if(this.pid == long.MinValue)
      {
        return false;
      }
      return true;
    }

    private bool SaveDetail()
    {
      // Delete Row In grid
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      // Save Detail
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if(row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[1];
          if(DBConvert.ParseLong(row["DetailPid"].ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@DetailPid", DbType.Int64, DBConvert.ParseLong(row["DetailPid"].ToString()));
          }

          // Code
          // Code

          DBParameter[] outPutParam = new DBParameter[1];
          outPutParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("", inputParam, outPutParam);
          if(DBConvert.ParseLong(outPutParam[0].Value.ToString()) == long.MinValue)
          {
            return false;
          }
        }
      }
      // End
      return true;
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      /*
      // Hidden column
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
      // Edit caption column
      e.Layout.Bands[0].Columns["Value"].Header.Caption = "Function";
      // Set dropdown in grid
      e.Layout.Bands[0].Columns["Kind"].ValueList = this.ultKind;
      // Set text align value
      e.Layout.Bands[0].Columns["TimeDoIt"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      // Set max width column
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      // Set column style checkbox
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      // Set defaulf value
      e.Layout.Bands[0].Columns["Selected"].DefaultCellValue = 0;
      */

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString().ToLower();
      //switch (columnName)
      //{
      //  case "location":
      //    if (DBConvert.ParseLong(e.Cell.Row.Cells["Location"].Value.ToString()) <= 0)
      //    {
      //      WindowUtinity.ShowMessageError("ERR0001", "Location");
      //      return;
      //    }
      //    break;
      //  default:
      //    break;
      //}
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString();
      //string text = e.Cell.Text.Trim();
      //switch (columnName.ToLower())
      //{
      //  case "qty":
      //    if (text.Length > 0)
      //    {
      //      if (DBConvert.ParseDouble(text) <= 0)
      //      {
      //        WindowUtinity.ShowMessageError("ERR0001", "Qty");
      //        e.Cancel = true;
      //      }
      //    }
      //    break;
      //  default:
      //    break;
      //}
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      bool success = this.CheckVaild(out message);
      if(!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      //Load Data
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion Event
  }
}
