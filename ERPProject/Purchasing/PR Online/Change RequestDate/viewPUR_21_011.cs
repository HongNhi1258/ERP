/*
  Author      : 
  Date        : 23/12/2013
  Description : Change Request Date Of PROnline
  Standard Code: view_GNR_90_005
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_21_011 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private int status = int.MinValue;
    private IList listDeletedParentPid = new ArrayList();
    private IList listDeletedChildPid = new ArrayList();
    private bool flagPur = false;
    private DataSet dsMain1 = new DataSet();
    private int a = int.MinValue;
    private long prDT = long.MinValue;
    private string prDepartment = string.Empty;

    #endregion Field

    #region Init

    public viewPUR_21_011()
    {
      InitializeComponent();
    }

    private void viewPUR_21_011_Load(object sender, EventArgs e)
    {
      if (this.btnPerPurchaser.Visible == true)
      {
        this.flagPur = true;
        this.btnPerPurchaser.Visible = false;
      }
      this.dsMain1 = this.CreateDataSet();
      this.LoadInit();
      this.LoadData();
      this.SetStatusControl();
    }

    #endregion Init

    #region Function

    /// <summary>
    /// Load Init Data
    /// </summary>
    private void LoadInit()
    {
      this.LoadUltraDropPRDetailCurrent();
      this.LoadUltraCBPR();
    }

    private void LoadUltraDropPRDetailCurrent()
    {
      string commandText = string.Empty;
      commandText = "  SELECT PRD.PID PRDetailPid, PR.PROnlineNo";
      commandText += " FROM TblPURPROnlineInformation PR";
      commandText += "    	INNER JOIN TblPURPROnlineDetailInformation PRD ON PR.PID  = PRD.PROnlinePid";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultdrpPRDetailTemp.DataSource = dtSource;
        ultdrpPRDetailTemp.ValueMember = "PRDetailPid";
        ultdrpPRDetailTemp.DisplayMember = "PROnlineNo";
      }
    }

    //Tuan Add
    private void LoadUltraCBPR()
    {
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT PR.Pid Value, PR.PROnlineNo Display
                                    FROM TblPURPROnlineInformation PR
                                    WHERE PR.Status <> 5 AND (PR.Department = '{0}' 
	                                      OR CASE WHEN '{1}' = 'PUR' THEN 1 ELSE 2 END = 1)
                                    ORDER BY PR.CreateDate DESC", SharedObject.UserInfo.Department, SharedObject.UserInfo.Department);

      //      commandText = string.Format(@"SELECT PR.Pid Value, PR.PROnlineNo Display
      //                                    FROM TblPURPROnlineInformation PR
      //                                    WHERE Department = '{0}'
      //                                    ORDER BY PR.CreateDate DESC", SharedObject.UserInfo.Department);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultcbPR, dtSource, "Value", "Display", "Value");
      ultcbPR.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    //End Tuan Add

    /// <summary>
    /// Load DropDown PRDetail
    /// </summary>
    private void LoadUltraDropPRDetail(string department)
    {
      string storeName = "spPURPRLoadListPRChangeRequestDate_Select";
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Department", DbType.String, department);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      if (dtSource != null)
      {
        ultdrpPRDetail.DataSource = dtSource;
        ultdrpPRDetail.ValueMember = "PRDetailPid";
        ultdrpPRDetail.DisplayMember = "PROnlineNo";

        // Format Grid
        ultdrpPRDetail.DisplayLayout.Bands[0].ColHeadersVisible = true;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["PROnlineNo"].MaxWidth = 120;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["PROnlineNo"].MinWidth = 120;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["WO"].MaxWidth = 50;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["WO"].MinWidth = 50;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["CarcassCode"].MaxWidth = 100;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["CarcassCode"].MinWidth = 100;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 100;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["MaterialCode"].MinWidth = 100;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["NameEN"].MaxWidth = 200;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["NameEN"].MinWidth = 200;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["NameVN"].MaxWidth = 200;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["NameVN"].MinWidth = 200;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["Unit"].MaxWidth = 50;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["Unit"].MinWidth = 50;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["Qty"].MaxWidth = 50;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["Qty"].MinWidth = 50;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["RequestDate"].MaxWidth = 100;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["RequestDate"].MinWidth = 100;

        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["IsChangeDate"].Hidden = true;
        ultdrpPRDetail.DisplayLayout.Bands[0].Columns["PRDetailPid"].Hidden = true;
      }
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      string storeName = "spPURPRChangeRequestDate_Select";
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      if (dsSource != null)
      {
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          this.status = DBConvert.ParseInt(row["Status"].ToString());
          txtChangeNo.Text = row["ChangeNo"].ToString();
          txtCreateDate.Text = row["CreateDate"].ToString();
          txtRequestBy.Text = row["RequestBy"].ToString();
          txtDepartment.Text = row["Department"].ToString();
          txtRemark.Text = row["Remark"].ToString();
        }
        else
        {
          DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPURPRChangeDateNo('PRCD') NewReceivingNo");
          if ((dtCode != null) && (dtCode.Rows.Count == 1))
          {
            txtChangeNo.Text = dtCode.Rows[0]["NewReceivingNo"].ToString();
            txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
            txtRequestBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
            txtDepartment.Text = SharedObject.UserInfo.Department;
          }
        }

        // Load Detail
        DataSet dsMain = this.CreateDataSet();
        dsMain.Tables["dtParent"].Merge(dsSource.Tables[1]);
        dsMain.Tables["dtChild"].Merge(dsSource.Tables[2]);
        ultData.DataSource = dsMain;

        // Set Status Control
        this.SetStatusControl();
      }
    }

    /// <summary>
    /// Create Data Set
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("PRDetailPid", typeof(System.Int64));
      taParent.Columns.Add("Department", typeof(System.String));
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("NameEN", typeof(System.String));
      taParent.Columns.Add("NameVN", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("QtyCancelReceived", typeof(System.Double));
      taParent.Columns.Add("RequestDate", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("PRDetailPid", typeof(System.Int64));
      taChild.Columns.Add("QtySeparate", typeof(System.Double));
      taChild.Columns.Add("QtyCancelReceived", typeof(System.Double));
      taChild.Columns.Add("RequestDateSeparate", typeof(System.DateTime));
      taChild.Columns.Add("Remark", typeof(System.String));
      taChild.Columns.Add("PURRemark", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["PRDetailPid"], taChild.Columns["PRDetailPid"], false));
      return ds;
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtRemark.ReadOnly = true;
        chkConfirm.Checked = true;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnDelete.Enabled = false;
      }

      //if(this.flagPur == true)
      //{
      //  chkConfirm.Checked = true;
      //  chkConfirm.Visible = false;
      //}
    }

    /// <summary>
    /// Check Data
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      string listPRDetail = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        listPRDetail += rowParent.Cells["PRDetailPid"].Value.ToString() + ";";

        double qtySeparate = 0;
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          // Check Qty Separate
          if (rowChild.Cells["QtySeparate"].Text.Length == 0 ||
             DBConvert.ParseDouble(rowChild.Cells["QtySeparate"].Value.ToString()) <= 0)
          {
            message = "Qty";
            return false;
          }

          // Total Qty 
          qtySeparate = qtySeparate + DBConvert.ParseDouble(rowChild.Cells["QtySeparate"].Value.ToString());

          // Check Date Change
          if (rowChild.Cells["RequestDateSeparate"].Text.Length == 0 ||
            DBConvert.ParseDateTime(rowChild.Cells["RequestDateSeparate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
          {
            message = "Request Date";
            return false;
          }
        }

        // Check Total Qty
        if (qtySeparate != DBConvert.ParseDouble(rowParent.Cells["Qty"].Value.ToString()))
        {
          message = "Total Qty";
          return false;
        }

        // Check Change Request For PR have only one Department
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@ListPRDetai", DbType.String, listPRDetail);
        DataTable dtDepartment = DataBaseAccess.SearchStoreProcedureDataTable("spPURCheckDepartmentForChangeDate_Select", input);
        if (dtDepartment.Rows.Count > 1)
        {
          message = "Don't change request date for two department";
          return false;
        }
        else
        {
          this.prDepartment = dtDepartment.Rows[0][0].ToString();
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Save Master Information
    /// </summary>
    /// <returns></returns>
    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[7];
      if (this.pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }

      //Code
      inputParam[1] = new DBParameter("@Prefix", DbType.String, "PRCD");
      inputParam[2] = new DBParameter("@Department", DbType.String, SharedObject.UserInfo.Department);
      inputParam[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[4] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (chkConfirm.Checked)
      {
        inputParam[5] = new DBParameter("@Status", DbType.Int32, 1);
      }
      else
      {
        inputParam[5] = new DBParameter("@Status", DbType.Int32, 0);
      }

      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[6] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
      }
      //Code

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeDateInformation_Edit", inputParam, ouputParam);
      // Gan Lai Pid
      this.pid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if (this.pid == long.MinValue)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      // 1: Delete Parent
      foreach (long pidParent in this.listDeletedParentPid)
      {
        DBParameter[] inputParams1 = new DBParameter[2];
        inputParams1[0] = new DBParameter("@ChangeNotePid", DbType.Int64, this.pid);
        inputParams1[1] = new DBParameter("@PRDetailPid", DbType.Int64, pidParent);
        DBParameter[] outputParams1 = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeRequestDatePRDetail_Delete", inputParams1, outputParams1);
        if (DBConvert.ParseLong(outputParams1[0].Value.ToString()) <= 0)
        {
          return false;
        }
      }

      // 2: Delete Detail
      foreach (long pidChild in this.listDeletedChildPid)
      {
        DBParameter[] inputParams2 = new DBParameter[2];
        inputParams2[0] = new DBParameter("@ChangeNotePid", DbType.Int64, this.pid);
        inputParams2[1] = new DBParameter("@Pid", DbType.Int64, pidChild);
        DBParameter[] outputParams2 = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeRequestDateDetail_Delete", inputParams2, outputParams2);
        if (DBConvert.ParseLong(outputParams2[0].Value.ToString()) <= 0)
        {
          return false;
        }
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          DBParameter[] inputParam = new DBParameter[7];
          if (DBConvert.ParseLong(rowChild.Cells["Pid"].Value.ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["Pid"].Value.ToString()));
          }
          inputParam[1] = new DBParameter("@ChangeNotePid", DbType.Int64, this.pid);
          inputParam[2] = new DBParameter("@PRDetailPid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["PRDetailPid"].Value.ToString()));
          inputParam[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(rowChild.Cells["QtySeparate"].Value.ToString()));
          inputParam[4] = new DBParameter("@RequestDate", DbType.DateTime, DBConvert.ParseDateTime(rowChild.Cells["RequestDateSeparate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          if (rowChild.Cells["Remark"].Value.ToString().Length > 0)
          {
            inputParam[5] = new DBParameter("@Remark", DbType.String, rowChild.Cells["Remark"].Value.ToString());
          }
          if (rowChild.Cells["PURRemark"].Value.ToString().Length > 0)
          {
            inputParam[6] = new DBParameter("@PURRemark", DbType.String, rowChild.Cells["PURRemark"].Value.ToString());
          }

          DBParameter[] outPutParam = new DBParameter[1];
          outPutParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeDateDetail_Edit", inputParam, outPutParam);
          if (DBConvert.ParseLong(outPutParam[0].Value.ToString()) == long.MinValue)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Send Email
    /// </summary>
    /// <returns></returns>
    private void SendEmail()
    {
      if (chkConfirm.Checked)
      {
        DBParameter[] input = new DBParameter[2];
        input[0] = new DBParameter("@ChangeNotePid", DbType.Int64, this.pid);
        input[1] = new DBParameter("@Type", DbType.Int32, 1);
        DataBaseAccess.ExecuteStoreProcedure("spPURSendEmailWhenConfirmedChangeRequestDatePR", input);
      }
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["PRDetailPid"].Header.Caption = "PROnlineNo";
      e.Layout.Bands[0].Columns["QtyCancelReceived"].Header.Caption = "QtyCancel + Received";
      e.Layout.Bands[1].Columns["QtyCancelReceived"].Header.Caption = "QtyCancel + Received";
      e.Layout.Bands[1].Columns["QtySeparate"].Header.Caption = "Qty Separate";
      e.Layout.Bands[1].Columns["RequestDateSeparate"].Header.Caption = "RequestDate Separate";
      e.Layout.Bands[1].Columns["PRDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PRDetailPid"].ValueList = ultdrpPRDetailTemp;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyCancelReceived"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtySeparate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyCancelReceived"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["PRDetailPid"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[1].Columns["QtySeparate"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[1].Columns["RequestDateSeparate"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[1].Columns["Remark"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[1].Columns["RequestDateSeparate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["RequestDateSeparate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      e.Layout.Bands[0].Columns["WO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyCancelReceived"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["QtyCancelReceived"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["PURRemark"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.True;

      if (this.status > 0)
      {
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

        e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "PRDetailPid":
          if (DBConvert.ParseLong(row.Cells["PRDetailPid"].Value.ToString()) != long.MinValue)
          {
            if (DBConvert.ParseInt(ultdrpPRDetail.SelectedRow.Cells["IsChangeDate"].Value.ToString()) == 0)
            {
              row.Cells["Department"].Value = ultdrpPRDetail.SelectedRow.Cells["Department"].Value.ToString();
              if (DBConvert.ParseInt(ultdrpPRDetail.SelectedRow.Cells["WO"].Value.ToString()) != int.MinValue)
              {
                row.Cells["WO"].Value = DBConvert.ParseInt(ultdrpPRDetail.SelectedRow.Cells["WO"].Value.ToString());
              }
              row.Cells["CarcassCode"].Value = ultdrpPRDetail.SelectedRow.Cells["CarcassCode"].Value.ToString();
              row.Cells["ItemCode"].Value = ultdrpPRDetail.SelectedRow.Cells["ItemCode"].Value.ToString();
              row.Cells["MaterialCode"].Value = ultdrpPRDetail.SelectedRow.Cells["MaterialCode"].Value.ToString();
              row.Cells["NameEN"].Value = ultdrpPRDetail.SelectedRow.Cells["NameEN"].Value.ToString();
              row.Cells["NameVN"].Value = ultdrpPRDetail.SelectedRow.Cells["NameVN"].Value.ToString();
              row.Cells["Unit"].Value = ultdrpPRDetail.SelectedRow.Cells["Unit"].Value.ToString();
              row.Cells["Qty"].Value = DBConvert.ParseDouble(ultdrpPRDetail.SelectedRow.Cells["Qty"].Value.ToString());
              row.Cells["QtyCancelReceived"].Value = DBConvert.ParseDouble(ultdrpPRDetail.SelectedRow.Cells["QtyCancelReceived"].Value.ToString());
              row.Cells["RequestDate"].Value = ultdrpPRDetail.SelectedRow.Cells["RequestDate"].Value.ToString();
            }
            else
            {
              DBParameter[] inputParam = new DBParameter[1];
              inputParam[0] = new DBParameter("@PRDetailPid", DbType.Int64, DBConvert.ParseLong(ultdrpPRDetail.SelectedRow.Cells["PRDetailPid"].Value.ToString()));
              DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURGetInformationPRDetailChangedDate_Select", inputParam);
              if (dsSource != null)
              {
                DataTable dt1 = dsSource.Tables[0];
                DataTable dt2 = dsSource.Tables[1];

                row.Cells["Department"].Value = dt1.Rows[0]["Department"].ToString();
                if (DBConvert.ParseInt(dt1.Rows[0]["WO"].ToString()) != int.MinValue)
                {
                  row.Cells["WO"].Value = DBConvert.ParseInt(dt1.Rows[0]["WO"].ToString());
                }
                row.Cells["CarcassCode"].Value = dt1.Rows[0]["CarcassCode"].ToString();
                row.Cells["ItemCode"].Value = dt1.Rows[0]["ItemCode"].ToString();
                row.Cells["MaterialCode"].Value = dt1.Rows[0]["MaterialCode"].ToString();
                row.Cells["NameEN"].Value = dt1.Rows[0]["NameEN"].ToString();
                row.Cells["NameVN"].Value = dt1.Rows[0]["NameVN"].ToString();
                row.Cells["Unit"].Value = dt1.Rows[0]["Unit"].ToString();
                row.Cells["Qty"].Value = DBConvert.ParseDouble(dt1.Rows[0]["Qty"].ToString());
                row.Cells["QtyCancelReceived"].Value = DBConvert.ParseDouble(dt1.Rows[0]["QtyCancelReceived"].ToString());
                row.Cells["RequestDate"].Value = dt1.Rows[0]["RequestDate"].ToString();

                DataSet dsMain = (DataSet)ultData.DataSource;
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                  DataRow rowAdd = dsMain.Tables[1].NewRow();
                  rowAdd["PRDetailPid"] = DBConvert.ParseLong(dt2.Rows[i]["PRDetailPid"].ToString());
                  rowAdd["QtySeparate"] = DBConvert.ParseDouble(dt2.Rows[i]["QtySeparate"].ToString());
                  rowAdd["QtyCancelReceived"] = DBConvert.ParseDouble(dt2.Rows[i]["QtyCancelReceived"].ToString());
                  rowAdd["RequestDateSeparate"] = (DateTime)dt2.Rows[i]["RequestDateSeparate"];

                  DataRow[] foundRow = dsMain.Tables[1].Select("PRDetailPid = " + DBConvert.ParseLong(dt2.Rows[i]["PRDetailPid"].ToString()) + "");
                  if (foundRow.Length <= dt2.Rows.Count - 1)
                  {
                    dsMain.Tables[1].Rows.Add(rowAdd);
                  }
                }
                ultData.DataSource = dsMain;
              }
            }
          }
          else
          {
            row.Cells["WO"].Value = DBNull.Value;
            row.Cells["CarcassCode"].Value = DBNull.Value;
            row.Cells["ItemCode"].Value = DBNull.Value;
            row.Cells["MaterialCode"].Value = DBNull.Value;
            row.Cells["NameEN"].Value = DBNull.Value;
            row.Cells["NameVN"].Value = DBNull.Value;
            row.Cells["Unit"].Value = DBNull.Value;
            row.Cells["QtyCancelReceived"].Value = DBNull.Value;
            row.Cells["Qty"].Value = DBNull.Value;
            row.Cells["RequestDate"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      string text = e.Cell.Text.Trim();
      switch (columnName)
      {
        case "PRDetailPid":
          if (text.Length > 0)
          {
            if (ultdrpPRDetail.SelectedRow == null)
            {
              WindowUtinity.ShowMessageError("ERR0001", "PROnlineNo");
              e.Cancel = true;
              return;
            }

            DataTable dt = (DataTable)ultdrpPRDetail.DataSource;
            DataRow[] foundRow = dt.Select(string.Format(@"PRDetailPid = {0}", DBConvert.ParseLong(ultdrpPRDetail.SelectedRow.Cells["PRDetailPid"].Value.ToString())));
            if (foundRow.Length == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "PROnlineNo");
              e.Cancel = true;
              return;
            }

            // Check Trung
            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              UltraGridRow rowGird = ultData.Rows[i];
              if (rowGird != row &&
                DBConvert.ParseLong(rowGird.Cells["PRDetailPid"].Value.ToString()) == DBConvert.ParseLong(ultdrpPRDetail.SelectedRow.Cells["PRDetailPid"].Value.ToString()))
              {
                WindowUtinity.ShowMessageError("ERR0013", "PROnlineNo");
                e.Cancel = true;
                return;
              }
            }
            // End
          }
          break;
        case "QtySeparate":
          if (text.Length > 0)
          {
            if (DBConvert.ParseDouble(row.Cells["QtySeparate"].Text) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Qty");
              e.Cancel = true;
              return;
            }
          }

          if (DBConvert.ParseDouble(row.Cells["QtyCancelReceived"].Value.ToString()) > 0)
          {
            if (DBConvert.ParseDouble(row.Cells["QtySeparate"].Text) < DBConvert.ParseDouble(row.Cells["QtyCancelReceived"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0001", "QtySeparate < Qty(Cancel + Received)");
              e.Cancel = true;
              return;
            }
          }
          break;
        case "RequestDateSeparate":
          DateTime requestDate = DBConvert.ParseDateTime(text, ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          // Check RequestDate Is Holiday
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@Date", DbType.DateTime, requestDate);
          DataTable dtCheckHoliday = DataBaseAccess.SearchStoreProcedureDataTable("spGNRCheckDateHoliday", input);
          if (dtCheckHoliday != null && dtCheckHoliday.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "RequestDate Separate Is Holiday ");
            e.Cancel = true;
          }
          // Check RequestDate Is Holiday
          break;
        default:
          break;
      }
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
        if (row.ParentRow == null)
        {
          long pidParent = DBConvert.ParseLong(row.Cells["PRDetailPid"].Value.ToString());
          if (pidParent != long.MinValue && this.pid != long.MinValue)
          {
            this.listDeletedParentPid.Add(pidParent);
          }
        }
        else
        {
          long pidChild = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (pidChild != long.MinValue && this.pid != long.MinValue)
          {
            this.listDeletedChildPid.Add(pidChild);
          }
        }
      }
    }

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      string text = e.Cell.Text.Trim();
      switch (columnName)
      {
        case "PRDetailPid":
          if (DBConvert.ParseLong(row.Cells["PRDetailPid"].Value.ToString()) == long.MinValue ||
              this.status == 0)
          {
            this.LoadUltraDropPRDetail(SharedObject.UserInfo.Department);
            e.Cell.Row.Cells["PRDetailPid"].ValueList = ultdrpPRDetail;
          }
          break;
        default:
          break;
      }
    }

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
        this.SendEmail();
      }

      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      if (this.flagPur == true && this.prDepartment == Shared.Utility.SharedObject.UserInfo.Department)
      {
        viewPUR_21_013 uc = new viewPUR_21_013();
        uc.pid = this.pid;
        uc.isPurchaser = true;
        WindowUtinity.ShowView(uc, "UPDATE PR CHANGE REQUEST DATE", false, ViewState.MainWindow);
      }
      else
      {
        //Load Data
        this.LoadData();
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (WindowUtinity.ShowMessageConfirm("MSG0007", txtChangeNo.Text).ToString() == "No")
      {
        return;
      }
      if (this.pid != long.MinValue)
      {
        string storeName = "spPURPRChangeDateInformation_Delete";
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@Pid", DbType.Int64, this.pid);

        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result > 0)
        {
          WindowUtinity.ShowMessageSuccess("MSG0002");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0093", txtChangeNo.Text);
          return;
        }
      }
      this.btnDelete.Visible = false;
      this.CloseTab();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }


    private void btnImport_Click(object sender, EventArgs e)
    {
      viewPUR_21_016 view = new viewPUR_21_016();
      view.pid = this.pid;
      Shared.Utility.WindowUtinity.ShowView(view, "Change Request Date", false, Shared.Utility.ViewState.ModalWindow);
      this.pid = view.pid;
      this.LoadData();
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (ultcbPR.Value != null)
      {
        string storeName = "spPURPRChangeRequestDateAuto_Select";
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@PROnlinePid", DbType.Int64, DBConvert.ParseLong(ultcbPR.Value.ToString()));
        inputParam[1] = new DBParameter("@Department", DbType.String, SharedObject.UserInfo.Department);

        DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
        dsSource.Relations.Add(new DataRelation("Parent_Child", dsSource.Tables[0].Columns["PRDetailPid"], dsSource.Tables[1].Columns["PRDetailPid"], false));
        if (dsSource != null)
        {
          this.prDT = DBConvert.ParseLong(dsSource.Tables[0].Rows[0]["PRDetailPid"].ToString());
          DataTable dtMain = (DataTable)this.dsMain1.Tables[0];
          for (int i = 0; i < dsSource.Tables[0].Rows.Count; i++)
          {
            DataRow rowData = dsSource.Tables[0].Rows[i];
            // Check Exist PRDetail
            DataRow[] foundRow = dtMain.Select("PRDetailPid = " + DBConvert.ParseLong(rowData["PRDetailPid"].ToString()) + "");
            if (foundRow.Length == 0)
            {
              // End
              this.a = 1;
            }
            else
            {
              this.a = int.MinValue;
            }
            if (this.a > 0)
            {
              DataRow row = dtMain.NewRow();
              row["PRDetailPid"] = DBConvert.ParseLong(rowData["PRDetailPid"].ToString());
              row["Department"] = rowData["Department"].ToString();
              if (DBConvert.ParseLong(rowData["WO"].ToString()) != long.MinValue)
              {
                row["WO"] = DBConvert.ParseLong(rowData["WO"].ToString());
              }
              if (rowData["CarcassCode"].ToString().Length > 0)
              {
                row["CarcassCode"] = rowData["CarcassCode"].ToString();
              }
              if (rowData["ItemCode"].ToString().Length > 0)
              {
                row["ItemCode"] = rowData["ItemCode"].ToString();
              }
              row["MaterialCode"] = rowData["MaterialCode"].ToString();
              row["NameEN"] = rowData["NameEN"].ToString();
              row["NameVN"] = rowData["NameVN"].ToString();
              row["Unit"] = rowData["Unit"].ToString();
              row["Qty"] = DBConvert.ParseDouble(rowData["Qty"].ToString());
              row["QtyCancelReceived"] = DBConvert.ParseDouble(rowData["QtyCancelReceived"].ToString());
              row["RequestDate"] = rowData["RequestDate"].ToString();

              dtMain.Rows.Add(row);
            }
          }
          if (this.a > 0)
          {
            DataTable process = (DataTable)((DataSet)dsSource).Tables[0];
            DataTable process1 = (DataTable)((DataSet)dsSource).Tables[1];
            DataTable data2 = (DataTable)this.dsMain1.Tables[1];

            DataRow[] data1 = this.dsMain1.Tables[1].Select(string.Format("PRDetailPid = {0}", this.prDT));
            foreach (DataRow dr1 in data1)
            {
              data2.Rows.Remove(dr1);
            }

            foreach (DataRow drRow1 in process1.Rows)
            {
              DataRow row1 = data2.NewRow();
              //row1["Pid"] = drRow1["Pid"];
              row1["PRDetailPid"] = drRow1["PRDetailPid"];
              row1["QtySeparate"] = drRow1["QtySeparate"];
              row1["RequestDateSeparate"] = drRow1["RequestDateSeparate"];
              row1["QtyCancelReceived"] = drRow1["QtyCancelReceived"];
              //row1["Remark"] = drRow1["Remark"];
              //row1["PURRemark"] = drRow1["PURRemark"];
              data2.Rows.Add(row1);
            }
          }
          ultData.DataSource = this.dsMain1;
        }
      }
    }
    #endregion Event

  }
}
