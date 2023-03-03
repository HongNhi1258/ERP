/*
  Author      : Xuan Truong
  Date        : 24/09/2012
  Description : Insert/Update ItemGroup
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.FinishGoodWarehouse;
using DaiCo.FinishGoodWarehouse.Reports;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using Infragistics.Win;
using System.Diagnostics;
using VBReport;
using System.IO;
using System.Data.SqlClient;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_02_003 : MainUserControl
  {
    #region Field
    // Pid Location
    public long locationPid = long.MinValue;
    // Status
    private int status = 0;

    private int count = 1;
    // Flag Update
    private bool canUpdate = false;
    #endregion Field

    #region Init
    public viewFGH_02_003()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load viewVEN_04_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewFGH_02_003_Load(object sender, EventArgs e)
    {
      //Load UltraCombo Item Group
      this.LoadComboItemGroup();
      //Load Data
      this.LoadData();
    }
    /// <summary>
    /// Load UltraCombo Location
    /// </summary>
    private void LoadComboItemGroup()
    {
      string commandText = string.Empty;
      commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 4001 AND DeleteFlag = 0";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultItemGroup.DataSource = dtSource;
      ultItemGroup.DisplayMember = "Value";
      ultItemGroup.ValueMember = "Code";
      ultItemGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultItemGroup.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@TranNoPid", DbType.Int64, this.locationPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHFTransferItemGroupByTranNoPid_Select", inputParam);
      DataTable dtLocationInfo = dsSource.Tables[0];
      if (dtLocationInfo.Rows.Count > 0)
      {
        DataRow row = dtLocationInfo.Rows[0];
        this.txtTrNo.Text = row["TranNo"].ToString();
        this.status = DBConvert.ParseInt(row["Status"].ToString());
        this.txtRemark.Text = row["Remark"].ToString();
        this.txtCreateBy.Text = row["CreateBy"].ToString();
        this.txtCreateDate.Text = row["CreateDate"].ToString();
        if (this.status == 1)
        {
          this.chkComfirm.Checked = true;
        }
      }
      else
      {
        DataTable dtLocation = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHFGetNewTranferItemGroupNo('09TRI') NewLocationNo");
        if ((dtLocation != null) && (dtLocation.Rows.Count > 0))
        {
          this.txtTrNo.Text = dtLocation.Rows[0]["NewLocationNo"].ToString();
          this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
        }
      }
      this.SetStatusControl();
      this.ultData.DataSource = dsSource.Tables[1];
    }
    /// <summary>
    /// SetStatusControl
    /// </summary>
    private void SetStatusControl()
    {
      this.canUpdate = (btnSave.Visible && this.status == 0);
      if (canUpdate)
      {
        this.ultItemGroup.ReadOnly = false;
        this.txtRemark.ReadOnly = false;
      }
      else
      {
        this.ultItemGroup.ReadOnly = true;
        this.txtRemark.ReadOnly = true;
        this.btnPrint.Enabled = true;
      }
      this.btnLoad.Enabled = this.canUpdate;
      this.chkComfirm.Enabled = this.canUpdate;
      this.btnSave.Enabled = this.canUpdate;
      this.btnDelete.Enabled = this.canUpdate;
    }
    #endregion Init

    #region Event

    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLoad_Click(object sender, EventArgs e)
    {
      //// Get path from Folder
      //string path = @"\PhanmemDENSOBHT8000";
      //path = Path.GetFullPath(path);
      //string[] a;
      DriveInfo[] allDrives = DriveInfo.GetDrives();
      string stringName = string.Empty;
      string path = "PhanmemDENSOBHT8000";
      int flagDrive = 0;
      foreach (DriveInfo d in allDrives)
      {
        stringName = d.Name;
        path = stringName + path;
        if (Directory.Exists(path))
        {
          flagDrive = 1;
          break;
        }
        path = "PhanmemDENSOBHT8000";
        stringName = string.Empty;
      }

      if (flagDrive == 0)
      {
        return;
      }

      string messageLocation = string.Empty;
      bool success = this.CheckValidLocation(out messageLocation);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", messageLocation);
        return;
      }
      string curFile = path + @"\THONGTIN.txt";

      if (!File.Exists(curFile))
      {
        string message = string.Format(DaiCo.Shared.Utility.FunctionUtility.GetMessage("ERR0011"), "THONGTIN.txt");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      string[] a = File.ReadAllLines(curFile);
     
      if (a.Length == 0)
      {
        return;
      }
      List<string> list = new List<string> (a);
      for (int i = 0; i < list.Count; i++)
      {
        for (int j = i + 1; j < list.Count; j++ )
        {
          if(list[i] == list[j])
          {
            list.RemoveAt(j);
            j--;
          }
        }
      }
      a = list.ToArray();
      DataTable dtSource = (DataTable)ultData.DataSource;
      for (int i = 0; i < a.Length; i++)
      {
        if (a[i].Trim().ToString().Length > 0)
        {
          string seriBoxNo = string.Empty;
          seriBoxNo = a[i].ToString();
          Boolean result = this.CheckInputData(seriBoxNo);
          if (result)
          {
            DBParameter[] input = new DBParameter[1];
            input[0] = new DBParameter("@SeriBox", DbType.AnsiString, 16, seriBoxNo);
            DataSet dsMain = DataBaseAccess.SearchStoreProcedure("spWHFTransferItemGroupByGetDataBySeriBoxNo_Select", input);
            DataTable dtMain = dsMain.Tables[0];
            if (dtMain.Rows.Count > 0)
            {
              DataRow row = dtSource.NewRow();
              row["Errors"] = 0;
              row["BoxNo"] = dtMain.Rows[0]["BoxNo"].ToString();
              row["BoxId"] = DBConvert.ParseInt(dtMain.Rows[0]["BoxId"].ToString());
              row["BoxCode"] = dtMain.Rows[0]["BoxCode"].ToString();
              row["ItemGroupFrom"] = dtMain.Rows[0]["ItemGroupFrom"].ToString();
              row["ItemGroupFromId"] = DBConvert.ParseInt(dtMain.Rows[0]["ItemGroupFromId"].ToString());
              row["ItemGroupTo"] = this.ultItemGroup.Text;
              row["ItemGroupToId"] = DBConvert.ParseInt(ultItemGroup.Value.ToString());
              dtSource.Rows.Add(row);
            }
            else
            {
              DataRow row = dtSource.NewRow();
              row["Errors"] = 1;
              row["BoxNo"] = seriBoxNo;
              row["BoxCode"] = "";
              row["ItemGroupFrom"] = "";
              row["ItemGroupTo"] = "";
              dtSource.Rows.Add(row);
            }
          }    
        }
      }
      this.ultData.DataSource = dtSource;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Errors"].Value.ToString()) == 1)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
      }
    }
    /// <summary>
    /// Save Location
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      DataTable dtMain = (DataTable)this.ultData.DataSource;
      if (dtMain == null || dtMain.Rows.Count == 0)
      {
        return;
      }
      bool success = this.CheckValid(out message);
      if (!success)
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
      // Load Data
      this.LoadData();
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
    /// <summary>
    /// Format Gird
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Columns["BoxNo"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["BoxCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemGroupFrom"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemGroupTo"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["TranNoPid"].Hidden = true;
      e.Layout.Bands[0].Columns["BoxId"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemGroupFromId"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemGroupToId"].Hidden = true;
      e.Layout.Bands[0].Columns["Errors"].Hidden = true;

      e.Layout.Bands[0].Columns["ItemGroupFrom"].Header.Caption = "Item Group From";
      e.Layout.Bands[0].Columns["ItemGroupTo"].Header.Caption = "Item Group To";
      e.Layout.Bands[0].Columns["BoxNo"].Header.Caption = "Box No";
      e.Layout.Bands[0].Columns["BoxCode"].Header.Caption = "Box Code";
      if (canUpdate == false)
      {
        e.Layout.Bands[0].Columns["BoxNo"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      }
      else
      {
        e.Layout.Bands[0].Columns["BoxNo"].CellActivation = Activation.AllowEdit;
      }
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    /// <summary>
    /// Delete Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      if (this.locationPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["PID"].Value.ToString()) > 0)
          {
            DBParameter[] inputParams = new DBParameter[1];
            long locationPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
            inputParams[0] = new DBParameter("@Pid", DbType.Int64, locationPid);
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            string storeName = string.Empty;
            storeName = "spWHFTransferItemGroupDetail_Delete";
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
            if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
            {
              WindowUtinity.ShowMessageError("ERR0004");
              this.LoadData();
              return;
            }
          }
        }
      }
    }

    /// <summary>
    /// Aftercell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (this.ultItemGroup.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Item Group");
        return;
      }
      if (count == 0)
      {
        return;
      }
      DataTable dtSource = (DataTable)ultData.DataSource;
      if (e.Cell.Text.Trim().Length > 0)
      {
        string strColName = e.Cell.Column.ToString().ToLower();
        int index = e.Cell.Row.Index;
        switch (strColName)
        {
          case "boxno":
            {
              string seriBoxNo = e.Cell.Value.ToString();
              bool success = this.CheckDouplicate(seriBoxNo);
              if (success)
              {
                DBParameter[] input = new DBParameter[1];
                input[0] = new DBParameter("@SeriBox", DbType.AnsiString, 16, seriBoxNo);
                DataSet dsMain = DataBaseAccess.SearchStoreProcedure("spWHFTransferItemGroupByGetDataBySeriBoxNo_Select", input);
                DataTable dtMain = dsMain.Tables[0];
                if (dtMain.Rows.Count > 0)
                {
                  count = 0;
                  e.Cell.Row.Cells["Errors"].Value = 0;
                  e.Cell.Row.Cells["BoxId"].Value = DBConvert.ParseInt(dtMain.Rows[0]["BoxId"].ToString());
                  e.Cell.Row.Cells["BoxCode"].Value = dtMain.Rows[0]["BoxCode"].ToString();
                  e.Cell.Row.Cells["ItemGroupFrom"].Value = dtMain.Rows[0]["ItemGroupFrom"].ToString();
                  e.Cell.Row.Cells["ItemGroupFromId"].Value = DBConvert.ParseInt(dtMain.Rows[0]["ItemGroupFromId"].ToString());
                  e.Cell.Row.Cells["ItemGroupTo"].Value = this.ultItemGroup.Text;
                  e.Cell.Row.Cells["ItemGroupToId"].Value = DBConvert.ParseInt(ultItemGroup.Value.ToString());
                }
                else
                {
                  e.Cell.Row.Cells["ItemGroupFrom"].Value = "";
                  e.Cell.Row.Cells["ItemGroupTo"].Value = "";
                  e.Cell.Row.Cells["BoxCode"].Value = "";
                  e.Cell.Row.Cells["Errors"].Value = 1;
                }

              }
              else
              {
                string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Duplicate");
                WindowUtinity.ShowMessageErrorFromText(message);
                e.Cell.Row.Cells["Errors"].Value = 1;
              }
              break;
            }

        }
      }
      else
      {
        e.Cell.Row.Cells["Errors"].Value = 1;
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Errors"].Value.ToString()) == 1)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.White;
        }
      }
      count = 1;
    }
    /// <summary>
    /// BeforeCell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string strColName = e.Cell.Column.ToString();

      if (string.Compare(strColName, "BoxNo") == 0)
      {
        if (this.ultItemGroup.Value == null)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Item Group");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
        }
      }
    }
    /// <summary>
    /// Delete phieu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.locationPid == long.MinValue)
      {
        return;
      }
      if (WindowUtinity.ShowMessageConfirm("MSG0007", "Transfer Item Group").ToString() == "No")
      {
        return;
      }
      string storeName = "spWHFTransferItemGroup_Delete";
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@Pid", DbType.Int64, locationPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure(storeName, input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result == 1)
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0093", "Transfer Item Group");
        return;
      }     

      this.btnDelete.Visible = false;
      this.CloseTab();
    }
    #endregion Event

    #region Process
    /// <summary>
    /// Check valid before Load
    /// </summary>
    /// <returns></returns>
    private bool CheckValidLocation(out string message)
    {
      message = string.Empty;
      // Master Information
      if (this.ultItemGroup.Value != null && this.ultItemGroup.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE [Group] = 4001 AND DeleteFlag = 0 AND Code =  '" + this.ultItemGroup.Value.ToString() + "'";
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Item Group";
            return false;
          }
        }
        else
        {
          message = "Item Group";
          return false;
        }
      }
      else
      {
        message = "Item Group";
        return false;
      }
      return true;
    }
    /// <summary>
    /// Check valid before Load
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      // Detail
      DataTable dtMain = (DataTable)this.ultData.DataSource;

      foreach (DataRow drMain in dtMain.Rows)
      {
        if (drMain.RowState != DataRowState.Deleted)
        {
          //Errors
          if (DBConvert.ParseInt(drMain["Errors"].ToString()) == 1)
          {
            message = "Data Input";
            return false;
          }
          else
          {
            if(drMain["BoxNo"].ToString().Length == 0)
            {
              message = "Data Input";
              return false;
            }
          }
        }
      }
      return true;
    }
    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;
      //Master Information Location Veneer
      result = this.SaveTransferLocation();
      if (!result)
      {
        return false;
      }
      // Save Location Veneer Detail 
      result = this.SaveTransferLocationDetail();
      if (!result)
      {
        return false;
      }

      if (this.chkComfirm.Checked)
      {
        result = this.UpdateItemGroup();
        if (!result)
        {
          return false;
        }
      }
      return result;
    }

    /// <summary>
    /// Update ItemGroup
    /// </summary>
    /// <returns></returns>
    private bool UpdateItemGroup()
    {
      long boxPid = long.MinValue;
      int afterItemGroup = int.MinValue;

      DataTable dtMain = (DataTable)ultData.DataSource;
      foreach (DataRow drMain in dtMain.Rows)
      {
        if (drMain.RowState != DataRowState.Deleted)
        {
          boxPid = DBConvert.ParseLong(drMain["BoxId"].ToString());
          afterItemGroup = DBConvert.ParseInt(drMain["ItemGroupToId"].ToString());
          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@Pid", DbType.Int64, boxPid);
          input[1] = new DBParameter("@AfterItemGroup", DbType.Int32, afterItemGroup);
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spWHFItemGroupBox_Update", input, output);
          int succsess = int.MinValue;
          succsess = DBConvert.ParseInt(output[0].Value.ToString());
          if (succsess == 0)
          {
            return false;
          }
        }   
      }
      return true;
    }
    /// <summary>
    /// Save Master Information Location Veneer
    /// </summary>
    /// <returns></returns>
    private bool SaveTransferLocation()
    {
      string storeName = string.Empty;

      // Update
      if (this.locationPid != long.MinValue)
      {
        storeName = "spWHFTransferItemGroup_Update";
        DBParameter[] inputParamUpdate = new DBParameter[4];

        // Pid
        inputParamUpdate[0] = new DBParameter("@PID", DbType.Int64, this.locationPid);

        // Status
        if (this.chkComfirm.Checked)
        {
          inputParamUpdate[1] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamUpdate[2] = new DBParameter("@Status", DbType.Int32, 0);
        }

        // Remark
        inputParamUpdate[3] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);

        DBParameter[] outputParamUpdate = new DBParameter[1];
        outputParamUpdate[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamUpdate, outputParamUpdate);

        long resultPid = DBConvert.ParseLong(outputParamUpdate[0].Value.ToString());
        this.locationPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
      }
      // Insert
      else
      {
        storeName = "spWHFTransferItemGroup_Insert";
        DBParameter[] inputParamInsert = new DBParameter[4];

        // Status
        if (this.chkComfirm.Checked)
        {
          inputParamInsert[0] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamInsert[0] = new DBParameter("@Status", DbType.Int32, 0);
        }

        // Remark
        inputParamInsert[1] = new DBParameter("@Remark", DbType.AnsiString, 4000, this.txtRemark.Text);

        // CreateBy
        inputParamInsert[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        // TrNo
        inputParamInsert[3] = new DBParameter("@TraLocationNo", DbType.String, "09TRI");

        DBParameter[] outputParamInsert = new DBParameter[1];
        outputParamInsert[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);

        long resultPid = DBConvert.ParseLong(outputParamInsert[0].Value.ToString());
        this.locationPid = resultPid;
        if (resultPid == long.MinValue)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Location Veneer Detail 
    /// </summary>
    /// <returns></returns>
    private bool SaveTransferLocationDetail()
    {
      long boxPid = long.MinValue;
      int locationFrom = int.MinValue;
      int locationTo = int.MinValue;

      DataTable dtMain = (DataTable)ultData.DataSource;
      foreach (DataRow drMain in dtMain.Rows)
      {
        if (drMain.RowState != DataRowState.Deleted)
        {
          if (drMain.RowState == DataRowState.Added || drMain.RowState == DataRowState.Modified)
          {
            
            boxPid = DBConvert.ParseLong(drMain["BoxId"].ToString());
            locationFrom = DBConvert.ParseInt(drMain["ItemGroupFromId"].ToString());
            locationTo = DBConvert.ParseInt(drMain["ItemGroupToId"].ToString());
            DBParameter[] input = new DBParameter[5];
            //Pid
            if (DBConvert.ParseLong(drMain["PID"].ToString()) >= 0)
            {
              input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(drMain["PID"].ToString()));
            }
            input[1] = new DBParameter("@TranNoPid", DbType.Int64, this.locationPid);
            input[2] = new DBParameter("@BoxPid", DbType.Int64, boxPid);
            input[3] = new DBParameter("@LocationFrom", DbType.Int32, locationFrom);
            input[4] = new DBParameter("@LocationTo", DbType.Int32, locationTo);
            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spWHFTransferItemGroupDetail_Edit", input, output);
            int succsess = int.MinValue;
            succsess = DBConvert.ParseInt(output[0].Value.ToString());
            if (succsess == 0)
            {
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check Douplicate
    /// </summary>
    /// <param name="seriBoxNo"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool CheckDouplicate(string seriBoxNo)
    {
      int k = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (String.Compare(seriBoxNo, ultData.Rows[i].Cells["BoxNo"].Value.ToString()) == 0)
        {
          k = k + 1;
          if (k > 1)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check Douplicate
    /// </summary>
    /// <param name="seriBoxNo"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool CheckInputData(string seriBoxNo)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (String.Compare(seriBoxNo, ultData.Rows[i].Cells["BoxNo"].Value.ToString()) == 0)
        {
          return false;
        }
      }
      return true;
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@TranNoPid", DbType.Int64, this.locationPid);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHFRPTFinishGoodsTransferItemGroupBoxCode", inputParam);
      Shared.DataSetSource.FinishGoodWarehouse.dsFGHTransferItemGroupBoxCode dsSource = new DaiCo.Shared.DataSetSource.FinishGoodWarehouse.dsFGHTransferItemGroupBoxCode();
      dsSource.Tables["TableInfo"].Merge(ds.Tables[0]);
      dsSource.Tables["TableDetail"].Merge(ds.Tables[1]);
      double totalQtyItems = 0;
      for (int i = 0; i < dsSource.Tables["TableDetail"].Rows.Count; i++)
      {
        if(DBConvert.ParseDouble(dsSource.Tables["TableDetail"].Rows[i]["QtyItems"].ToString()) != double.MinValue)
        {
          totalQtyItems = totalQtyItems + DBConvert.ParseDouble(dsSource.Tables["TableDetail"].Rows[i]["QtyItems"].ToString());
        }
      }
      DaiCo.Shared.View_Report report = null;
      cptWHFTransferItemGroupBoxCode rpt = new cptWHFTransferItemGroupBoxCode();
      rpt.SetDataSource(dsSource);
      rpt.SetParameterValue("TotalQtyItems", totalQtyItems);
      report = new DaiCo.Shared.View_Report(rpt);
      report.IsShowGroupTree = false;
      report.ShowReport(Shared.Utility.ViewState.MainWindow);
    }
    #endregion Process
  }
}
