/*
  Author      : Duong Minh
  Date        : 10/05/2012
  Description : Issuing Note (Return To Production)
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
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using Infragistics.Win;
using System.Diagnostics;
using VBReport;
using System.IO;
using System.Data.SqlClient;
using DaiCo.Shared.DataSetSource.FinishGoodWarehouse;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_03_005 : MainUserControl
  {
    #region Field
    public long outStorePid = long.MinValue;
    private string issCode = string.Empty;
    private int status = 0;
    #endregion Field

    #region Init
    public viewFGH_03_005()
    {
      InitializeComponent();
    }

    private void viewFGH_03_001_Load(object sender, EventArgs e)
    {
      // Check Summary MonthLy Duong Minh 10/10/2011 START
      bool result = this.CheckSummary();
      if (result == false)
      {
        this.CloseTab();
        return;
      }
      // Check Summary MonthLy Duong Minh 10/10/2011 END

      this.LoadDeparment();

      this.LoadData();
    }

    /// <summary>
    /// Check Summary PreMonth && PreYear
    /// </summary>
    /// <returns></returns>
    private bool CheckSummary()
    {
      DateTime firstDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      int result = DateTime.Compare(firstDate, DateTime.Today);

      if (result <= 0)
      {
        int preMonth = 0;
        int preYear = 0;
        if (DateTime.Today.Month == 1)
        {
          preMonth = 12;
          preYear = DateTime.Today.Year - 1;
        }
        else
        {
          preMonth = DateTime.Today.Month - 1;
          preYear = DateTime.Today.Year;
        }

        string commandText = "SELECT COUNT(*) FROM TblWHFBalance WHERE Month = " + preMonth + " AND Year = " + preYear;
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if ((dtCheck == null) || (dtCheck != null && DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0))
        {
          WindowUtinity.ShowMessageError("ERR0303", preMonth.ToString(), preYear.ToString());
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Load Department
    /// </summary>
    private void LoadDeparment()
    {
      string commandText = " SELECT Department, DeparmentName ";
      commandText += "       FROM VHRDDepartment  ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "DeparmentName";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["DeparmentName"].Width = 500;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      // Insert
      if (this.outStorePid == long.MinValue)
      {
        string commandText = string.Empty;
        commandText += "SELECT dbo.FWHFGetNewOutStoreCode('09ISS')";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          this.issCode = dt.Rows[0][0].ToString();
          this.txtIssuingCode.Text = this.issCode;
        }
      }
      // Update
      else
      {
        DBParameter[] arrInputParam = new DBParameter[1];
        arrInputParam[0] = new DBParameter("@IssPid", DbType.Int64, this.outStorePid);

        DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHFOutStoreIssuingToProduction_Select", arrInputParam);
        if (ds != null)
        {
          if (ds.Tables[0].Rows.Count > 0)
          {
            this.issCode = ds.Tables[0].Rows[0]["OutStoreCode"].ToString();
            this.ultDepartment.Value = ds.Tables[0].Rows[0]["Department"].ToString();
            this.status = DBConvert.ParseInt(ds.Tables[0].Rows[0]["Posting"].ToString());
            this.txtNote.Text = ds.Tables[0].Rows[0]["Note"].ToString();
            this.txtIssuingCode.Text = this.issCode;
          }

          this.ultSummary.DataSource = ds.Tables[1];
          this.ultOutStoreDetail.DataSource = ds.Tables[2];
        }
      }
      //Set Control
      this.SetStatusControl();
    }

    private void SetStatusControl()
    {
      if (this.status == 1)
      {
        this.txtNote.Enabled = false;
        this.btnLoad.Enabled = false;
        this.chkConfirm.Checked = true;
        this.chkConfirm.Enabled = false;
        this.btnSave.Enabled = false;
        this.ultDepartment.Enabled = false;
        this.btnPrint.Enabled = true;
        this.btnPrintDetail.Enabled = true;

        for (int i = 0; i < ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultOutStoreDetail.Rows[i];
          row.Activation = Activation.ActivateOnly;
        }
      }
      else
      {
        this.txtNote.Enabled = true;
        this.btnSave.Enabled = true;
        if (this.ultOutStoreDetail.Rows.Count == 0)
        {
          this.btnLoad.Enabled = true;
        }
        else
        {
          this.btnLoad.Enabled = false;
        }
        this.ultDepartment.Enabled = true;
        if (this.outStorePid == long.MinValue)
        {
          this.chkConfirm.Enabled = false;
          this.btnPrint.Enabled = false;
          this.btnPrintDetail.Enabled = false;
        }
        else
        {
          this.chkConfirm.Enabled = true;
          this.btnPrint.Enabled = true;
          this.btnPrintDetail.Enabled = true;
        }

        for (int i = 0; i < ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultOutStoreDetail.Rows[i];
          row.Cells["No"].Activation = Activation.ActivateOnly;
          row.Cells["ItemGroup"].Activation = Activation.ActivateOnly;
          row.Cells["BoxCode"].Activation = Activation.ActivateOnly;
          row.Cells["BoxName"].Activation = Activation.ActivateOnly;
          row.Cells["BoxId"].Activation = Activation.ActivateOnly;
          row.Cells["Length"].Activation = Activation.ActivateOnly;
          row.Cells["Width"].Activation = Activation.ActivateOnly;
          row.Cells["Height"].Activation = Activation.ActivateOnly;
          row.Cells["Set"].Activation = Activation.ActivateOnly;
          row.Cells["WorkOrder"].Activation = Activation.ActivateOnly;
          row.Cells["Location"].Activation = Activation.ActivateOnly;
          row.Cells["Check"].Activation = Activation.AllowEdit;
        }
      }

      for (int i = 0; i < ultSummary.Rows.Count; i++)
      {
        UltraGridRow row = ultSummary.Rows[i];
        row.Activation = Activation.ActivateOnly;
      }
    }

    private DataTable createTableBox()
    {
      DataTable dt = new DataTable();

      DataColumn no = new DataColumn("No");
      no.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(no);

      DataColumn itemGroup = new DataColumn("ItemGroup");
      itemGroup.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(itemGroup);

      DataColumn pid = new DataColumn("Pid");
      pid.DataType = System.Type.GetType("System.Int64");
      dt.Columns.Add(pid);

      DataColumn seriBoxNo = new DataColumn("BoxId");
      seriBoxNo.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(seriBoxNo);

      DataColumn boxType = new DataColumn("BoxCode");
      boxType.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(boxType);

      DataColumn boxTypeName = new DataColumn("BoxName");
      boxTypeName.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(boxTypeName);

      DataColumn workOrder = new DataColumn("WorkOrder");
      workOrder.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(workOrder);

      DataColumn length = new DataColumn("Length");
      length.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(length);

      DataColumn width = new DataColumn("Width");
      width.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(width);

      DataColumn height = new DataColumn("Height");
      height.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(height);

      DataColumn set = new DataColumn("Set");
      set.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(set);

      DataColumn location = new DataColumn("Location");
      location.DataType = System.Type.GetType("System.String");
      dt.Columns.Add(location);

      DataColumn locationPid = new DataColumn("LocationPid");
      locationPid.DataType = System.Type.GetType("System.Int32");
      dt.Columns.Add(locationPid);

      DataColumn osdPidDetail = new DataColumn("OSDPidDetail");
      osdPidDetail.DataType = System.Type.GetType("System.Int32");
      dt.Columns.Add(osdPidDetail);

      DataColumn check = new DataColumn("Check");
      check.DataType = System.Type.GetType("System.Int32");
      dt.Columns.Add(check);
      return dt;
    }
    #endregion Function

    #region Event 
    private void ultOutStoreDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["OSDPidDetail"].Hidden = true;

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WorkOrder"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Set"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["WorkOrder"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["BoxName"].Header.Caption = "Box Name";
      e.Layout.Bands[0].Columns["BoxCode"].Header.Caption = "Box Code";
      e.Layout.Bands[0].Columns["BoxId"].Header.Caption = "Box Id";

      e.Layout.Bands[0].Columns["Check"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["No"].MinWidth = 45;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 45;

      e.Layout.Bands[0].Columns["Location"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Location"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Check"].MinWidth = 45;
      e.Layout.Bands[0].Columns["Check"].MaxWidth = 45;

      e.Layout.Bands[0].Columns["Length"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Length"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Width"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Width"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Height"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Height"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["WorkOrder"].MinWidth = 50;
      e.Layout.Bands[0].Columns["WorkOrder"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Set"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Set"].MaxWidth = 50;
     
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
     
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultSummary_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      e.Layout.Bands[0].Columns["RemainStockBalance"].Hidden = true;
      e.Layout.Bands[0].Columns["LoadingQty"].Hidden = true;
     
      e.Layout.Bands[0].Columns["IssQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["IssQty"].Header.Caption = "Iss Qty";
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
      if (this.ultDepartment.Value == null || this.ultDepartment.Value.ToString().Length == 0)
      {
        string message = "Please fill Department";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      string[] a;
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
 
      //// Get path from Folder
      //string path = @"\PhanmemDENSOBHT8000";
      //path = Path.GetFullPath(path);
      string pathBarCode = path + @"\THONGTIN.txt";
      try
      {
        a = File.ReadAllLines(pathBarCode);
      }
      catch
      {
        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (a.Length == 0)

      {
        string message = "No box have been scanned.";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      DataTable dt = createTableBox();

      int index = int.MinValue;
      if (a[0].ToString().Length > 0)
      {
        index = a[0].IndexOf("*");
      }

      if (index != -1)
      {
        for (int i = 0; i < a.Length; i++)
        {
          if (a[i].Trim().ToString() != string.Empty)
          {
            index = a[i].IndexOf("*");
            a[i] = a[i].Substring(0, index).Trim().ToString();
          }
        }
      }

      int k = 0;
      for (int i = 0; i < a.Length; i++)
      {
        //check duplicate
        k = 0;
        for (int j = i + 1; j < a.Length; j++)
        {
          if (a[i].ToString() == a[j].ToString())
          {
            k++;
          }
        }
        if (k > 0)
        {
          continue;
        }
        //

        DBParameter[] arrInputParam = new DBParameter[1];
        arrInputParam[0] = new DBParameter("@SeriBoxNo", DbType.String, a[i].ToString());
        DataTable dtBox = DataBaseAccess.SearchStoreProcedureDataTable("spWHFBoxOutStoreBySeriBoxNo", arrInputParam);
        
        DataRow dr = dt.NewRow();
        if (dtBox.Rows.Count > 0)
        {
          dr["ItemGroup"] = dtBox.Rows[0]["ItemGroup"].ToString();
          dr["Pid"] = dtBox.Rows[0]["Pid"].ToString();
          dr["BoxId"] = dtBox.Rows[0]["SeriBoxNO"].ToString();
          dr["BoxCode"] = dtBox.Rows[0]["BoxTypeCode"].ToString();
          dr["BoxName"] = dtBox.Rows[0]["BoxTypeName"].ToString();
          dr["WorkOrder"] = dtBox.Rows[0]["WorkOrder"].ToString();
          dr["Length"] = dtBox.Rows[0]["Length"].ToString();
          dr["Width"] = dtBox.Rows[0]["Width"].ToString();
          dr["Height"] = dtBox.Rows[0]["Height"].ToString();
          dr["Set"] = dtBox.Rows[0]["Set"].ToString();
          dr["Location"] = dtBox.Rows[0]["Location"].ToString();
          dr["LocationPid"] = DBConvert.ParseInt(dtBox.Rows[0]["LocationPid"].ToString());
          
        }
        else
        {
          dr["BoxId"] = a[i].ToString();
        }
        dr["Check"] = 0;
        dt.Rows.Add(dr);
      }

      DataView dView = dt.DefaultView;
      dView.Sort = "BoxId";
      dt = dView.ToTable();
      int count = 1;
      for (int p = 0; p < dt.Rows.Count; p++)
      {
        DataRow row = dt.Rows[p];
        row["No"] = count.ToString();
        count++;
      }

      this.ultOutStoreDetail.DataSource = dt;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.ultDepartment.Value == null || this.ultDepartment.Value.ToString().Length == 0)
      {
        string message = "Please fill Department";
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (this.chkConfirm.Checked)
      {
        DBParameter[] arrInputParam = new DBParameter[1];

        arrInputParam[0] = new DBParameter("@OutStorePid", DbType.Int64, this.outStorePid);

        DBParameter[] arrOutputParam = new DBParameter[1];
        arrOutputParam[0] = new DBParameter("@Result", DbType.Int32, 0);

        DataTable dt = DataBaseAccess.SearchCommandTextDataTable("spWHFCheckFullSetIssuingProduction_Select", arrInputParam);
        if (dt != null && dt.Rows.Count > 0)
        {
          string message = "This issuing need to issue full set";
          WindowUtinity.ShowMessageErrorFromText(message);
          return;
        }
      }

      for (int i = 0; i < ultOutStoreDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultOutStoreDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 0)
        {
          if (row.Cells["BoxCode"].Value.ToString().Length == 0)
          {
            string message = "Box Id is invalid";
            WindowUtinity.ShowMessageErrorFromText(message);
            return;
          }
        }
      }

      // Insert
      if (this.outStorePid == long.MinValue)
      {
        int j = 0;
        if (this.chkConfirm.Checked)
        {
          j = 1;
        }
        else
        {
          j = 0;
        }

        string outStoreCode = string.Empty;
        string commandText = string.Empty;
        commandText += "SELECT dbo.FWHFGetNewOutStoreCode('09ISS')";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          outStoreCode = dt.Rows[0][0].ToString();
        }

        DBParameter[] arrInputParam = new DBParameter[5];

        arrInputParam[0] = new DBParameter("@OutStoreCode", DbType.String, outStoreCode);
        arrInputParam[1] = new DBParameter("@UserWH", DbType.Int32, SharedObject.UserInfo.UserPid);
        arrInputParam[2] = new DBParameter("@Posting", DbType.Int32, j);
        arrInputParam[3] = new DBParameter("@Note", DbType.String, this.txtNote.Text.ToString());
        arrInputParam[4] = new DBParameter("@Department", DbType.String, this.ultDepartment.Value.ToString());

        DBParameter[] arrOutputParam = new DBParameter[1];
        arrOutputParam[0] = new DBParameter("@Result", DbType.Int32, 0);

        DataBaseAccess.ExecuteStoreProcedure("spWHFOutStore_Insert", arrInputParam, arrOutputParam);

        outStorePid = DBConvert.ParseLong(arrOutputParam[0].Value.ToString());

        //Detail
        for (int i = 0; i < this.ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = this.ultOutStoreDetail.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 0)
          {
            commandText = " SELECT COUNT(*) FROM TblWHFBoxInStore WHERE BoxPID = " + DBConvert.ParseInt(row.Cells["Pid"].Value.ToString());
            dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count > 0)
            {
              if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 0)
              {
                continue;
              }
            }
            else
            {
              continue;
            }
            //TblWHFOutStoreDetail
            DBParameter[] arrInputParamInStore = new DBParameter[3];

            arrInputParamInStore[0] = new DBParameter("@OutStoreID", DbType.Int64, this.outStorePid);
            arrInputParamInStore[1] = new DBParameter("@BoxPID", DbType.Int32, DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()));
            arrInputParamInStore[2] = new DBParameter("@Location", DbType.Int32, DBConvert.ParseInt(row.Cells["LocationPid"].Value.ToString()));

            DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetail_Insert", arrInputParamInStore);

            //TBLWHFINBOX
            DBParameter[] arrInputParamBox = new DBParameter[2];
            arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, row.Cells["BoxId"].Value.ToString());
            arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 3);
            DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);

            //TBLWHFINBOXINSTORE
            DBParameter[] arrInputParamBoxInStore = new DBParameter[1];
            arrInputParamBoxInStore[0] = new DBParameter("@BoxPID", DbType.Int32, DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()));

            DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Delete", arrInputParamBoxInStore);
          }
        }
      }
      //update
      else
      {
        int j = 0;
        if (this.chkConfirm.Checked)
        {
          j = 1;
        }
        else
        {
          j = 0;
        }

        //update TblWHFOutStore
        DBParameter[] arrInputParam = new DBParameter[5];
        arrInputParam[0] = new DBParameter("@OutStoreCode", DbType.String, this.issCode);
        arrInputParam[1] = new DBParameter("@Note", DbType.AnsiString, 255, this.txtNote.Text);
        arrInputParam[2] = new DBParameter("@UserWh", DbType.Int32, SharedObject.UserInfo.UserPid);
        arrInputParam[3] = new DBParameter("@Posting", DbType.Int32, j);
        arrInputParam[4] = new DBParameter("@Department", DbType.String, this.ultDepartment.Value.ToString());

        DataBaseAccess.ExecuteStoreProcedure("spWHFOutStore_Update", arrInputParam);

        for (int i = 0; i < this.ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = this.ultOutStoreDetail.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 1
              && DBConvert.ParseLong(row.Cells["OSDPidDetail"].Value.ToString()) != long.MinValue)
          {
            //TBLWHFINBOX
            DBParameter[] arrInputParamBox = new DBParameter[2];
            arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, row.Cells["BoxId"].Value.ToString());
            arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 2);
            DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);

            //TBLWHFINBOXINSTORE
            DBParameter[] arrParamBoxStore = new DBParameter[2];
            arrParamBoxStore[0] = new DBParameter("@BoxPID", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            arrParamBoxStore[1] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(row.Cells["LocationPid"].Value.ToString()));
            DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Insert", arrParamBoxStore);

            //TblWHFOutStoreDetail
            DBParameter[] arrParamBoxDetail = new DBParameter[2];
            arrParamBoxDetail[0] = new DBParameter("@OutStoreID", DbType.Int64, this.outStorePid);
            arrParamBoxDetail[1] = new DBParameter("@BoxPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetail_Delete", arrParamBoxDetail);
          }

          if (DBConvert.ParseLong(row.Cells["OSDPidDetail"].Value.ToString()) == long.MinValue
            && DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 0)
          {
            //TblWHFOutStoreDetail
            DBParameter[] arrInputParamInStore = new DBParameter[3];

            arrInputParamInStore[0] = new DBParameter("@OutStoreID", DbType.Int64, this.outStorePid);
            arrInputParamInStore[1] = new DBParameter("@BoxPID", DbType.Int32, DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()));
            arrInputParamInStore[2] = new DBParameter("@Location", DbType.Int32, DBConvert.ParseInt(row.Cells["LocationPid"].Value.ToString()));

            DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetail_Insert", arrInputParamInStore);

            //TBLWHFINBOX
            DBParameter[] arrInputParamBox = new DBParameter[2];
            arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, row.Cells["BoxId"].Value.ToString());
            arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 3);
            DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);

            //TBLWHFINBOXINSTORE
            DBParameter[] arrInputParamBoxInStore = new DBParameter[1];
            arrInputParamBoxInStore[0] = new DBParameter("@BoxPID", DbType.Int32, DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()));

            DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Delete", arrInputParamBoxInStore);
          }
        }
      }

      if (this.chkConfirm.Checked)
      {
        DBParameter[] arrInputParamWHF = new DBParameter[3];
        arrInputParamWHF[0] = new DBParameter("@Createby", DbType.Int32, SharedObject.UserInfo.UserPid);
        arrInputParamWHF[1] = new DBParameter("@OutStorePid", DbType.Int64, this.outStorePid);
        long waStart = (rdRepair.Checked) ? 44 : 248;
        arrInputParamWHF[2] = new DBParameter("@WorkAreaPid", DbType.Int64, waStart);
        // Create Furniture Repair
        DataBaseAccess.ExecuteStoreProcedure("spPLNAutoCreateFurnitureWhenIssingFGW_Exec", 3000, arrInputParamWHF);
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      ViewFGH_99_001 view = new ViewFGH_99_001();
      view.outStoreCode = this.issCode;
      view.ncategory = 9;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void btnPrintDetail_Click(object sender, EventArgs e)
    {
      ViewFGH_99_001 view = new ViewFGH_99_001();
      view.outStoreCode = this.issCode;
      view.ncategory = 10;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (chkHide.Checked)
      {
        this.grpSummary.Visible = false;
      }
      else
      {
        this.grpSummary.Visible = true;
      }
    }
    #endregion Event
  }
}
