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

namespace DaiCo.Purchasing
{
  public partial class viewPUR_21_013 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private int status = int.MinValue;
    private IList listDeletedPODetailSchedulePid = new ArrayList();
    public bool isPurchaser = false;
    public bool isPlanner = false;
    private string prDepartment = string.Empty;
    private string depChangeRequestDate = string.Empty;
    #endregion Field

    #region Init

    public viewPUR_21_013()
    {
      InitializeComponent();
    }

    private void viewPUR_21_013_Load(object sender, EventArgs e)
    {
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
      // prDepartment && depChangeRequestDate
      string commandText = string.Empty;
      commandText = string.Format(@" SELECT DISTINCT INF.Department DepChangeDate, PROL.Department DepPROnline
                                    FROM TblPURPRChangeDateInformation INF
	                                    INNER JOIN TblPURPRChangeDateDetail DT ON INF.Pid = DT.ChangeNotePid
	                                    INNER JOIN TblPURPROnlineDetailInformation PROLDT ON DT.PRDetailPid = PROLDT.PID
	                                    INNER JOIN TblPURPROnlineInformation PROL ON PROL.PID = PROLDT.PROnlinePid
                                    WHERE INF.Pid = {0}", this.pid);
      DataTable dtDeppartment = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtDeppartment != null && dtDeppartment.Rows.Count == 1)
      {
        this.prDepartment = dtDeppartment.Rows[0]["DepPROnline"].ToString();
        this.depChangeRequestDate = dtDeppartment.Rows[0]["DepChangeDate"].ToString();
      }
      //
      this.LoadUltraDropPRDetail();
    }

    /// <summary>
    /// Load DropDown PRDetail
    /// </summary>
    private void LoadUltraDropPRDetail()
    {
      string commandText = string.Empty;
      commandText = "  SELECT PRD.PID PRDetailPid, PR.PROnlineNo";
      commandText += " FROM TblPURPROnlineInformation PR";
      commandText += "    	INNER JOIN TblPURPROnlineDetailInformation PRD ON PR.PID  = PRD.PROnlinePid";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultdrpPRDetail.DataSource = dtSource;
        ultdrpPRDetail.ValueMember = "PRDetailPid";
        ultdrpPRDetail.DisplayMember = "PROnlineNo";
      }
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      string storeName = "spPURPRChangeRequestDateAffectPODetail_Select";
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
        dsMain.Tables["dtChildPODetail"].Merge(dsSource.Tables[3]);
        dsMain.Tables["dtChildPODetailSchedule"].Merge(dsSource.Tables[4]);
        ultData.DataSource = dsMain;

        ultData.Rows[0].ChildBands[0].Rows.Band.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

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
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("NameEN", typeof(System.String));
      taParent.Columns.Add("NameVN", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("QtyPROnline", typeof(System.Double));
      taParent.Columns.Add("QtyCancelReceived", typeof(System.Double));
      taParent.Columns.Add("RequestDate", typeof(System.String));
      taParent.Columns.Add("Reject", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("PRDetailPid", typeof(System.Int64));
      taChild.Columns.Add("QtySeparate", typeof(System.Double));
      taChild.Columns.Add("QtyCancelReceived", typeof(System.Double));
      taChild.Columns.Add("RequestDateSeparate", typeof(System.DateTime));
      taChild.Columns.Add("Remark", typeof(System.String));
      taChild.Columns.Add("PURRemark", typeof(System.String));
      taChild.Columns.Add("IsPODetail", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["PRDetailPid"], taChild.Columns["PRDetailPid"], false));

      DataTable taChildPODetail = new DataTable("dtChildPODetail");
      taChildPODetail.Columns.Add("PRDetailPid", typeof(System.Int64));
      taChildPODetail.Columns.Add("PODetailPid", typeof(System.Int64));
      taChildPODetail.Columns.Add("PONo", typeof(System.String));
      taChildPODetail.Columns.Add("Qty", typeof(System.Double));
      taChildPODetail.Columns.Add("QtyCancel", typeof(System.Double));
      taChildPODetail.Columns.Add("RequestDate", typeof(System.String));
      taChildPODetail.Columns.Add("IsPODetail", typeof(System.Int32));
      taChildPODetail.Columns.Add("IsPODetailUpdate", typeof(System.Int32));
      ds.Tables.Add(taChildPODetail);

      ds.Relations.Add(new DataRelation("dtParent_dtChildPODetail", taParent.Columns["PRDetailPid"], taChildPODetail.Columns["PRDetailPid"], false));

      DataTable taChildPODetailSchedule = new DataTable("dtChildPODetailSchedule");
      taChildPODetailSchedule.Columns.Add("PODetailPid", typeof(System.Int64));
      taChildPODetailSchedule.Columns.Add("PODetailSchedulePid", typeof(System.Int64));
      taChildPODetailSchedule.Columns.Add("Qty", typeof(System.Double));
      taChildPODetailSchedule.Columns.Add("QtyCancel", typeof(System.Double));
      taChildPODetailSchedule.Columns.Add("ReceiptedQty", typeof(System.Double));
      taChildPODetailSchedule.Columns.Add("ExpectDate", typeof(System.DateTime));
      taChildPODetailSchedule.Columns.Add("IsUpdate", typeof(System.Int32));
      ds.Tables.Add(taChildPODetailSchedule);

      ds.Relations.Add(new DataRelation("dtChildPODetail_dtChildPODetailSchedule", taChildPODetail.Columns["PODetailPid"], taChildPODetailSchedule.Columns["PODetailPid"], false));

      return ds;
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      chkConfirm.Enabled = false;
      btnSave.Enabled = false;
      btnReturn.Enabled = false;

      if (this.isPurchaser == true)
      {
        if ((this.prDepartment == Shared.Utility.SharedObject.UserInfo.Department && this.depChangeRequestDate == Shared.Utility.SharedObject.UserInfo.Department)
        || (Shared.Utility.SharedObject.UserInfo.Department == "PUR" && this.depChangeRequestDate != Shared.Utility.SharedObject.UserInfo.Department)
        || (this.prDepartment != Shared.Utility.SharedObject.UserInfo.Department && this.depChangeRequestDate == "PLA")
        || (Shared.Utility.SharedObject.UserInfo.Department == "PLA" && this.depChangeRequestDate != Shared.Utility.SharedObject.UserInfo.Department))
        {
          //chkConfirm.Enabled = false;
          //btnSave.Enabled = false;
          //btnReturn.Enabled = false;
          chkConfirm.Enabled = true;
          btnSave.Enabled = true;
          btnReturn.Enabled = true;
        }
        else
        {
          //chkConfirm.Enabled = true;
          //btnSave.Enabled = true;
          //btnReturn.Enabled = true;
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          btnReturn.Enabled = false;
        }
      }
      else if (this.isPlanner == true)
      {
        if (this.prDepartment == Shared.Utility.SharedObject.UserInfo.Department && this.depChangeRequestDate == "PUR")
        {
          chkConfirm.Enabled = true;
          btnSave.Enabled = true;
          btnReturn.Enabled = true;
        }
      }

      //if (this.isPurchaser == true)
      //{
      //  if (this.prDepartment == "PLA" && this.depChangeRequestDate == "PUR")
      //  {
      //    chkConfirm.Enabled = false;
      //    btnSave.Enabled = false;
      //    btnReturn.Enabled = false;
      //  }
      //  else
      //  {
      //    chkConfirm.Enabled = true;
      //    btnSave.Enabled = true;
      //    btnReturn.Enabled = true;
      //  }
      //}
      //else if(this.isPlanner == true)
      //{
      //  if (this.prDepartment == "PLA" && this.depChangeRequestDate == "PUR")
      //  {
      //    chkConfirm.Enabled = true;
      //    btnSave.Enabled = true;
      //    btnReturn.Enabled = true;
      //  }
      //}

      if (this.status > 1)
      {
        txtRemark.ReadOnly = true;
        chkConfirm.Checked = true;
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnReturn.Enabled = false;
      }
    }

    /// <summary>
    ///  Check Data
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        for (int j = 0; j < row.ChildBands[1].Rows.Count; j++)
        {
          UltraGridRow rowChild = row.ChildBands[1].Rows[j];
          double totalQty = 0;
          for (int k = 0; k < rowChild.ChildBands[0].Rows.Count; k++)
          {
            UltraGridRow rowChildDetail = rowChild.ChildBands[0].Rows[k];
            // Check Qty
            if (DBConvert.ParseDouble(rowChildDetail.Cells["Qty"].Value.ToString()) <= 0)
            {
              message = "Qty";
              return false;
            }
            totalQty = totalQty + DBConvert.ParseDouble(DBConvert.ParseDouble(rowChildDetail.Cells["Qty"].Value.ToString()));

            // Check Expect Date
            if (rowChildDetail.Cells["ExpectDate"].Value.ToString().Length == 0)
            {
              message = "ExpectDate";
              return false;
            }
          }
          // Check Total Qty
          if (totalQty != DBConvert.ParseDouble(rowChild.Cells["Qty"].Value.ToString()))
          {
            message = "TotalQty";
            return false;
          }
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
        if (success)
        {
          // Send Email
          this.SendEmailWhenConfirmed();

          success = this.FinishedPRChangeDate();
          if (success && this.isPurchaser == true)
          {
            success = UpdatePODetailSchedule();
          }
        }
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
        inputParam[5] = new DBParameter("@Status", DbType.Int32, 2);
      }
      else
      {
        inputParam[5] = new DBParameter("@Status", DbType.Int32, 1);
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
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(rowChild.Cells["IsPODetail"].Value.ToString()) == 0)
          {
            DBParameter[] inputParam = new DBParameter[8];
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

            inputParam[7] = new DBParameter("@Reject", DbType.Int32, DBConvert.ParseInt(rowParent.Cells["Reject"].Value.ToString()));

            DBParameter[] outPutParam = new DBParameter[1];
            outPutParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeDateDetail_Edit", inputParam, outPutParam);
            if (DBConvert.ParseLong(outPutParam[0].Value.ToString()) == long.MinValue)
            {
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool UpdatePODetailSchedule()
    {
      if (chkConfirm.Checked & this.pid != long.MinValue)
      {
        // 1: Delete Parent
        foreach (long poDetailSchedulePid in this.listDeletedPODetailSchedulePid)
        {
          DBParameter[] inputParams = new DBParameter[1];
          inputParams[0] = new DBParameter("@PODetailSchedulePid", DbType.Int64, poDetailSchedulePid);
          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeRequestDatePODetailSchedule_Delete", inputParams, outputParams);
          if (DBConvert.ParseLong(outputParams[0].Value.ToString()) <= 0)
          {
            return false;
          }
        }

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow rowParent = ultData.Rows[i];
          if (DBConvert.ParseInt(rowParent.Cells["Reject"].Value.ToString()) == 0)
          {
            for (int j = 0; j < rowParent.ChildBands[1].Rows.Count; j++)
            {
              UltraGridRow rowChild = rowParent.ChildBands[1].Rows[j];
              if (DBConvert.ParseInt(rowChild.Cells["IsPODetail"].Value.ToString()) == 1 &&
                  DBConvert.ParseInt(rowChild.Cells["IsPODetailUpdate"].Value.ToString()) == 1)
              {
                string arrayPODetailSchedulePid = string.Empty;
                long poDetailPid = DBConvert.ParseLong(rowChild.Cells["PODetailPid"].Value.ToString());
                for (int k = 0; k < rowChild.ChildBands[0].Rows.Count; k++)
                {
                  UltraGridRow rowChildDetail = rowChild.ChildBands[0].Rows[k];
                  //if (DBConvert.ParseInt(rowChildDetail.Cells["IsUpdate"].Value.ToString()) == 1)
                  //{
                  DBParameter[] inputParam = new DBParameter[4];
                  if (DBConvert.ParseLong(rowChildDetail.Cells["PODetailSchedulePid"].Value.ToString()) != long.MinValue)
                  {
                    inputParam[0] = new DBParameter("@PODetailSchedulePid", DbType.Int64, DBConvert.ParseLong(rowChildDetail.Cells["PODetailSchedulePid"].Value.ToString()));
                  }
                  inputParam[1] = new DBParameter("@PODetailPid", DbType.Int64, DBConvert.ParseLong(rowChildDetail.Cells["PODetailPid"].Value.ToString()));
                  inputParam[2] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(rowChildDetail.Cells["Qty"].Value.ToString()));
                  inputParam[3] = new DBParameter("@ExpectDate", DbType.DateTime, DBConvert.ParseDateTime(rowChildDetail.Cells["ExpectDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));

                  DBParameter[] outPutParam = new DBParameter[1];
                  outPutParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
                  DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeRequestDatePODetailSchedule_Update", inputParam, outPutParam);
                  if (DBConvert.ParseLong(outPutParam[0].Value.ToString()) == long.MinValue)
                  {
                    return false;
                  }

                  // Input Array PODetailSchedulePid
                  arrayPODetailSchedulePid = arrayPODetailSchedulePid + "~" + outPutParam[0].Value.ToString();
                  // End
                  //}
                }
                // Delete PODetailSchedulePid
                bool result = this.DeletePODetailSchedulePidOver(poDetailPid, arrayPODetailSchedulePid);
                if (result == false)
                {
                  return false;
                }
              }
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Delete PODetailSchedulePid Over
    /// </summary>
    /// <param name="poDetailPid"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    private bool DeletePODetailSchedulePidOver(long poDetailPid, string stringData)
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PODetailPid", DbType.Int64, poDetailPid);
      input[1] = new DBParameter("@StringData", DbType.String, stringData);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int32, 0);
      DataBaseAccess.ExecuteStoreProcedure("spPURPRChangeRequestDatePODetailScheduleOver_Delete", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Update Fisnihed PR Change RequestDate(Purchaser)
    /// </summary>
    /// <returns></returns>
    private bool FinishedPRChangeDate()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@ChangeNotePid", DbType.Int64, this.pid);

      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPURPRDetailStockBalance_Update", inputParam, outputParam);
      if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Update Data When Return Change Request Date
    /// </summary>
    /// <returns></returns>
    private bool ReturnChangeRequestDate()
    {
      // input
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@ChangeNotePid", DbType.Int64, this.pid);
      input[1] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      // output
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      //Execute
      DataBaseAccess.ExecuteStoreProcedure("spPURPRReturnChangeRequestDate_Update", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Send Email When Confirmed
    /// </summary>
    /// <returns></returns>
    private void SendEmailWhenConfirmed()
    {
      if (chkConfirm.Checked)
      {
        DBParameter[] input = new DBParameter[3];
        input[0] = new DBParameter("@ChangeNotePid", DbType.Int64, this.pid);
        input[1] = new DBParameter("@Type", DbType.Int32, 2);
        input[2] = new DBParameter("@Purchaser", DbType.String, SharedObject.UserInfo.EmpName);

        DataBaseAccess.ExecuteStoreProcedure("spPURSendEmailWhenConfirmedChangeRequestDatePR", input);
      }
    }

    /// <summary>
    /// Send Email When Return
    /// </summary>
    /// <returns></returns>
    private void SendEmailWhenReturn()
    {
      DBParameter[] input = new DBParameter[3];
      input[0] = new DBParameter("@ChangeNotePid", DbType.Int64, this.pid);
      input[1] = new DBParameter("@Type", DbType.Int32, 3);
      input[2] = new DBParameter("@Purchaser", DbType.String, SharedObject.UserInfo.EmpName);

      DataBaseAccess.ExecuteStoreProcedure("spPURSendEmailWhenConfirmedChangeRequestDatePR", input);
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["QtyCancelReceived"].Header.Caption = "QtyCancel + Received";
      e.Layout.Bands[0].Columns["PRDetailPid"].Header.Caption = "PROnlineNo";
      e.Layout.Bands[0].Columns["QtyPROnline"].Header.Caption = "Qty";
      e.Layout.Bands[0].Columns["PRDetailPid"].ValueList = ultdrpPRDetail;
      e.Layout.Bands[0].Columns["QtyPROnline"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyCancelReceived"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Reject"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["PRDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["QtySeparate"].Header.Caption = "Qty Separate";
      e.Layout.Bands[1].Columns["RequestDateSeparate"].Header.Caption = "RequestDate Separate";
      e.Layout.Bands[1].Columns["QtyCancelReceived"].Header.Caption = "QtyCancel + Received";
      e.Layout.Bands[1].Columns["QtySeparate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyCancelReceived"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["PURRemark"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[1].Columns["QtySeparate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["QtyCancelReceived"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["RequestDateSeparate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["IsPODetail"].Hidden = true;
      e.Layout.Bands[2].Columns["PRDetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["PODetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["IsPODetail"].Hidden = true;
      e.Layout.Bands[2].Columns["IsPODetailUpdate"].Hidden = true;
      e.Layout.Bands[2].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["QtyCancel"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["RequestDate"].Header.Caption = "PR RequestDate";

      e.Layout.Bands[2].Columns["PONo"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[2].Columns["PONo"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[2].Columns["Qty"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[2].Columns["Qty"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[2].Columns["QtyCancel"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[2].Columns["QtyCancel"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[2].Columns["RequestDate"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[2].Columns["RequestDate"].CellAppearance.FontData.Bold = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.No;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 3; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[2].Override.AllowUpdate = DefaultableBoolean.True;

      if (this.status == 2)
      {
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        for (int j = 0; j < row.ChildBands[1].Rows.Count; j++)
        {
          UltraGridRow rowChild = row.ChildBands[1].Rows[j];
          for (int k = 0; k < rowChild.ChildBands[0].Rows.Count; k++)
          {
            UltraGridRow rowChildDetail = rowChild.ChildBands[0].Rows[k];
            rowChildDetail.Cells["PODetailPid"].Column.Hidden = true;
            rowChildDetail.Cells["PODetailSchedulePid"].Column.Hidden = true;
            rowChildDetail.Cells["IsUpdate"].Column.Hidden = true;
            rowChildDetail.Cells["Qty"].Appearance.BackColor = Color.Aqua;
            rowChildDetail.Cells["ExpectDate"].Appearance.BackColor = Color.Aqua;
            rowChildDetail.Cells["Qty"].Appearance.TextHAlign = HAlign.Right;
            rowChildDetail.Cells["QtyCancel"].Appearance.TextHAlign = HAlign.Right;
            rowChildDetail.Cells["ReceiptedQty"].Appearance.TextHAlign = HAlign.Right;
            rowChildDetail.Cells["QtyCancel"].Activation = Activation.ActivateOnly;
            rowChildDetail.Cells["ReceiptedQty"].Activation = Activation.ActivateOnly;
            rowChildDetail.Cells["ExpectDate"].Column.Format = ConstantClass.FORMAT_DATETIME;
            rowChildDetail.Cells["ExpectDate"].Column.FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
            if (this.status == 2)
            {
              rowChildDetail.Band.Override.AllowAddNew = AllowAddNew.No;
              rowChildDetail.Band.Override.AllowDelete = DefaultableBoolean.False;
              rowChildDetail.Cells["Qty"].Activation = Activation.ActivateOnly;
              rowChildDetail.Cells["ExpectDate"].Activation = Activation.ActivateOnly;
            }

            // Những PO Nào có nhận về một phần thì không cho delete
            if (DBConvert.ParseDouble(rowChildDetail.Cells["ReceiptedQty"].Value.ToString()) > 0)
            {
              rowChildDetail.Band.Override.AllowDelete = DefaultableBoolean.False;
            }
          }
        }
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
        case "Qty":
          e.Cell.Row.ParentRow.Cells["IsPODetailUpdate"].Value = 1;
          //e.Cell.Row.Cells["IsUpdate"].Value = 1;
          break;
        case "ExpectDate":
          e.Cell.Row.ParentRow.Cells["IsPODetailUpdate"].Value = 1;
          //e.Cell.Row.Cells["IsUpdate"].Value = 1;
          break;
        default:
          break;
      }
    }

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "Qty":
          row.Cells["Qty"].Appearance.TextHAlign = HAlign.Right;
          row.Cells["Qty"].Appearance.BackColor = Color.Aqua;
          row.Cells["ExpectDate"].Appearance.BackColor = Color.Aqua;
          row.Cells["QtyCancel"].Appearance.TextHAlign = HAlign.Right;
          row.Cells["ReceiptedQty"].Appearance.TextHAlign = HAlign.Right;
          row.Cells["QtyCancel"].Activation = Activation.ActivateOnly;
          row.Cells["ReceiptedQty"].Activation = Activation.ActivateOnly;

          break;
        case "ExpectDate":
          row.Cells["ExpectDate"].Appearance.BackColor = Color.Aqua;
          row.Cells["Qty"].Appearance.BackColor = Color.Aqua;
          row.Cells["QtyCancel"].Appearance.TextHAlign = HAlign.Right;
          row.Cells["ReceiptedQty"].Appearance.TextHAlign = HAlign.Right;
          row.Cells["QtyCancel"].Activation = Activation.ActivateOnly;
          row.Cells["ReceiptedQty"].Activation = Activation.ActivateOnly;
          break;
        default:
          break;
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "Qty":
          if (row.Cells["Qty"].Text.Length > 0)
          {
            double qtyCancel = (DBConvert.ParseDouble(row.Cells["QtyCancel"].Value.ToString()) > 0) ? DBConvert.ParseDouble(row.Cells["QtyCancel"].Value.ToString()) : 0;
            double qtyReceipted = (DBConvert.ParseDouble(row.Cells["ReceiptedQty"].Value.ToString()) > 0) ? DBConvert.ParseDouble(row.Cells["ReceiptedQty"].Value.ToString()) : 0;
            if (DBConvert.ParseDouble(row.Cells["Qty"].Text) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Qty");
              e.Cancel = true;
              return;
            }
            else if (DBConvert.ParseDouble(row.Cells["Qty"].Text) < (qtyCancel + qtyReceipted))
            {
              WindowUtinity.ShowMessageError("ERR0001", "Qty < QtyCancel + ReceiptedQty");
              e.Cancel = true;
              return;
            }
          }
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
        if (row.Cells["PODetailSchedulePid"].Value != null)
        {
          long poDetailPid = DBConvert.ParseLong(row.Cells["PODetailSchedulePid"].Value.ToString());
          if (poDetailPid != long.MinValue && this.pid != long.MinValue)
          {
            this.listDeletedPODetailSchedulePid.Add(poDetailPid);
          }
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
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

      //Load Data
      this.LoadData();
    }

    private void btnReturn_Click(object sender, EventArgs e)
    {
      // Save Data
      bool success = this.SaveDetail();
      if (success)
      {
        success = this.ReturnChangeRequestDate();
        if (success)
        {
          this.SendEmailWhenReturn();
        }
      }
      // Show Message
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Return");
      }

      // Close Tab
      this.CloseTab();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
