/*
  Author      : Nguyen Thanh Thinh
  Date        : 
  Description : 
  Standard Form: ViewPLN_15_002
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class ViewPLN_15_002 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    public long TransactionPid;
    private int depart;
    private int status;
    private int importing;
    private IList listItemCode;
    private IList listRevision;
    private IList listCarcassCode;
    private IList listWo;
    #endregion Field

    #region Init
    public ViewPLN_15_002()
    {
      InitializeComponent();
    }

    private void ViewPLN_15_002_Load(object sender, EventArgs e)
    {
      if (btnPLA.Visible)
      {
        depart = 1;
      }
      if (btnCAR.Visible)
      {
        depart = 2;
      }
      pnlAccessRight.Visible = false;
      // Add ask before closing form even if user change data

      this.SetAutoAskSaveWhenCloseForm(groupBoxMaster);
      this.SetDate();
      this.InitData();
      this.LoadData();
      gbImportExel.Height = 56;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoAskSaveWhenCloseForm(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.TextChanged += new System.EventHandler(this.Object_Changed);
        }
        else
        {
          this.SetAutoAskSaveWhenCloseForm(ctr);
        }
      }
    }
    private void SetDate()
    {
      ultDTDateTo.Value = DBConvert.ParseString(DateTime.Now);
      ultDTFromDate.Value = DBConvert.ParseString(DateTime.Now);
      string week = WeekofYear(DateTime.Now) + " - " + WeekofYear(DateTime.Now);
      txtWeekNo.Text = week;
    }
    private void FillDataForGrid(DataTable dt)
    {

      DataSet dsItem = (DataSet)ultData.DataSource;
      DataTable dt_tmp = this.EraseDeleteData(dsItem.Tables[1]);
      foreach (DataRow row in dt.Rows)
      {
        string itemCode = row[0].ToString().Trim();
        if (itemCode.Length > 0)
        {
          DataRow rowItemSource = dt_tmp.NewRow();
          if (row.RowState != DataRowState.Deleted)
          {

            rowItemSource["ItemCode"] = itemCode;
            if (row["Revision"].ToString().Length > 0 && DBConvert.ParseInt(row["Revision"].ToString()) >= 0)
            {
              rowItemSource["Revision"] = row["Revision"].ToString();
            }
            if (row["CarcassCode"].ToString().Length > 0)
            {
              rowItemSource["CarcassCode"] = row["CarcassCode"].ToString();
            }
            if (DBConvert.ParseLong(row["WO"].ToString().Trim()) > 0)
            {
              rowItemSource["WO"] = DBConvert.ParseLong(row["WO"].ToString().Trim());
            }
            if (row["Container"].ToString().Length > 0)
            {
              rowItemSource["Container"] = row["Container"].ToString();
            }
            if (row["Qty"].ToString().Length > 0 && DBConvert.ParseInt(row["Qty"].ToString()) > 0)
            {
              rowItemSource["Qty"] = row["Qty"].ToString();
            }
            if (row["LoadingDate"].ToString().Length > 0 && DBConvert.ParseDateTime(row["LoadingDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
            {
              rowItemSource["LoadingDate"] = row["LoadingDate"].ToString();
            }
            if (row["WorkStation"].ToString().Length > 0)
            {
              rowItemSource["StandByEN"] = row["WorkStation"].ToString();
            }
            if (row["DeadlineProcess"].ToString().Length > 0 && DBConvert.ParseDateTime(row["DeadlineProcess"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
            {
              rowItemSource["DeadlineProcess"] = row["DeadlineProcess"].ToString();
            }
            if (row["PackingDeadline"].ToString().Length > 0 && DBConvert.ParseDateTime(row["PackingDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
            {
              rowItemSource["DeadlinePacking"] = row["PackingDeadline"].ToString();
            }
            if (row["PackingDeadline"].ToString().Length > 0 && DBConvert.ParseDateTime(row["PackingDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
            {
              rowItemSource["ConfirmDeadline"] = row["PackingDeadline"].ToString();
            }
            if (row["PLNRemark"].ToString().Length > 0)
            {
              rowItemSource["PLNRemark"] = row["PLNRemark"].ToString();
            }

          }
          //rowItemSource["PLNRemark"] = row["PLNRemark"].ToString();
          dt_tmp.Rows.Add(rowItemSource);
        }
      }

      string storeName = "spPLNImportWoCarcassdetailFromExcel";
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt_tmp);
      DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      DataSet ds = this.CreateDataSet();
      ds.Tables["dtParent"].Merge(dsSource.Tables[0]);
      ds.Tables["dtChild"].Merge(dsSource.Tables[1]);
      ultData.DataSource = ds;
      SetStatusControl(this.status);
      this.SetColor();
    }
    private void SetColor()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow ur = ultData.Rows[i];
        if (this.status < 3)
        {
          if (DBConvert.ParseInt(ur.Cells["Error"].Value.ToString()) == 1)
          {
            ur.CellAppearance.BackColor = Color.SkyBlue;
          }
          if (DBConvert.ParseInt(ur.Cells["Error"].Value.ToString()) == 2)
          {
            ur.CellAppearance.BackColor = Color.LightGreen;
          }
          for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow urc = ur.ChildBands[0].Rows[j];
            if (DBConvert.ParseInt(urc.Cells["Qty"].Value.ToString()) != DBConvert.ParseInt(urc.Cells["ConfirmQty"].Value.ToString()))
            {
              urc.Cells["WO"].Appearance.BackColor = Color.Pink;
            }
            if (DBConvert.ParseInt(urc.Cells["Error"].Value.ToString()) == 1)
            {
              urc.CellAppearance.BackColor = Color.Yellow;
            }
          }
          for (int j = 0; j < ultData.Rows[i].ChildBands[1].Rows.Count; j++)
          {
            UltraGridRow urcs = ur.ChildBands[1].Rows[j];
            if (DBConvert.ParseInt(urcs.Cells["FunitureExists"].Value.ToString()) == 1)
            {
              urcs.CellAppearance.BackColor = Color.LightGreen;
            }
          }
        }
        ur.ChildBands[0].Band.Columns["Qty"].CellAppearance.BackColor = Color.LightGreen;
        ur.ChildBands[0].Band.Columns["DeadlinePacking"].CellAppearance.BackColor = Color.LightGreen;
        ur.ChildBands[0].Band.Columns["ConfirmQty"].CellAppearance.BackColor = Color.Yellow;
        ur.ChildBands[0].Band.Columns["ConfirmDeadline"].CellAppearance.BackColor = Color.Yellow;

      }
    }
    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
    }

    private void SetStatusControl(int status)
    {

      switch (status)
      {
        case 0:
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            UltraGridRow ur = ultData.Rows[i];
            ur.Activation = Activation.ActivateOnly;
            for (int j = 0; j < ur.ChildBands[0].Band.Columns.Count; j++)
            {
              ur.ChildBands[0].Band.Columns[j].CellActivation = Activation.ActivateOnly;
            }
          }
          if (this.depart == 1)
          {
            txtRemark.ReadOnly = false;
            ultDTDateTo.ReadOnly = false;
            ultDTFromDate.ReadOnly = false;

            this.gbImportExel.Visible = true;
            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              UltraGridRow ur = ultData.Rows[i];
              ur.Activation = Activation.ActivateOnly;
              for (int k = 0; k < ur.ChildBands[0].Rows.Count; k++)
              {
                if (DBConvert.ParseInt(ur.ChildBands[0].Rows[k].Cells["Qty"].Value) != DBConvert.ParseInt(ur.ChildBands[0].Rows[k].Cells["ConfirmQty"].Value))
                {
                  ur.ChildBands[0].Band.Columns["Qty"].CellActivation = Activation.AllowEdit;
                }
              }
              ur.ChildBands[0].Band.Columns["PLNRemark"].CellActivation = Activation.AllowEdit;
            }
            ckbComfirm.Enabled = true;
            btnSave.Enabled = true;
          }
          else if (this.depart == 2)
          {
            ckbComfirm.Enabled = false;
            btnSave.Enabled = false;
          }
          btnFinish.Visible = false;
          btnReturn.Visible = false;
          break;
        case 1:
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            UltraGridRow ur = ultData.Rows[i];
            ur.Activation = Activation.ActivateOnly;
            for (int j = 0; j < ur.ChildBands[0].Band.Columns.Count; j++)
            {
              ur.ChildBands[0].Band.Columns[j].CellActivation = Activation.ActivateOnly;
            }
          }
          if (this.depart == 1)
          {

            btnReturn.Visible = false;
            ckbComfirm.Enabled = false;
            ckbComfirm.Checked = true;
            btnSave.Enabled = false;
          }
          else if (this.depart == 2)
          {
            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              UltraGridRow ur = ultData.Rows[i];
              ur.ChildBands[0].Band.Columns["CarcassRemark"].CellActivation = Activation.AllowEdit;
              ur.ChildBands[0].Band.Columns["ConfirmQty"].CellActivation = Activation.AllowEdit;
              ur.ChildBands[0].Band.Columns["ConfirmDeadline"].CellActivation = Activation.AllowEdit;
            }

            btnReturn.Visible = true;
            ckbComfirm.Enabled = true;
            ckbComfirm.Checked = false;
            btnSave.Enabled = true;
          }
          this.gbImportExel.Visible = false;
          btnFinish.Visible = false;
          break;
        case 2:
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            UltraGridRow ur = ultData.Rows[i];
            ur.Activation = Activation.ActivateOnly;
            for (int j = 0; j < ur.ChildBands[0].Band.Columns.Count; j++)
            {
              ur.ChildBands[0].Band.Columns[j].CellActivation = Activation.ActivateOnly;
            }
          }
          if (this.depart == 1)
          {
            btnFinish.Visible = true;
            ckbComfirm.Checked = false;

          }
          else if (this.depart == 2)
          {
            btnFinish.Visible = false;
          }
          this.gbImportExel.Visible = false;
          btnReturn.Visible = false;
          ckbComfirm.Enabled = false;
          ckbComfirm.Checked = true;
          btnSave.Enabled = false;
          break;
        default:
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            UltraGridRow ur = ultData.Rows[i];
            ur.Activation = Activation.ActivateOnly;
            for (int j = 0; j < ur.ChildBands[0].Band.Columns.Count; j++)
            {
              ur.ChildBands[0].Band.Columns[j].CellActivation = Activation.ActivateOnly;
            }
          }
          btnFinish.Visible = false;
          btnReturn.Visible = false;
          ckbComfirm.Enabled = false;
          ckbComfirm.Checked = false;
          btnSave.Enabled = false;
          break;
      }
      if (this.depart == 3)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow ur = ultData.Rows[i];
          ur.Activation = Activation.ActivateOnly;
          for (int j = 0; j < ur.ChildBands[0].Band.Columns.Count; j++)
          {
            ur.ChildBands[0].Band.Columns[j].CellActivation = Activation.ActivateOnly;
          }
        }
        btnFinish.Visible = true;
        btnReturn.Visible = true;
        ckbComfirm.Enabled = true;
        btnSave.Enabled = false;
      }

    }
    private int WeekofYear(DateTime date)
    {
      int[] moveByDays = { 6, 7, 8, 9, 10, 4, 5 };
      DateTime startOfYear = new DateTime(date.Year, 1, 1);
      DateTime endOfYear = new DateTime(date.Year, 12, 31);
      int numberDays = date.Subtract(startOfYear).Days +
              moveByDays[(int)startOfYear.DayOfWeek];
      int weekNumber = numberDays / 7;
      switch (weekNumber)
      {
        case 0:
          // Before start of first week of this year - in last week of previous year
          weekNumber = WeekofYear(startOfYear.AddDays(-1));
          break;
        case 53:
          // In first week of next year.
          if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
          {
            weekNumber = 1;
          }
          break;
      }
      return weekNumber;
    }
    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain != null && dtMain.Rows.Count > 0)
      {
        foreach (DataRow dr in dtMain.Rows)
        {
          txtCarcassWONo.Text = dr["CarcassWoNo"].ToString();
          ultDTFromDate.Value = dr["FromDate"].ToString();
          ultDTDateTo.Value = dr["ToDate"].ToString();
          txtCreateby.Text = dr["CreateBy"].ToString();
          txtDateCreate.Text = dr["CreateDate"].ToString();
          txtRemark.Text = dr["Remark"].ToString();

          string weekOfYear = this.WeekofYear(Convert.ToDateTime(dr["FromDate"].ToString())) + " - " + this.WeekofYear(Convert.ToDateTime(dr["ToDate"].ToString()));
          txtWeekNo.Text = weekOfYear;
          this.status = DBConvert.ParseInt(dr["Status"].ToString());
        }
      }
      else
      {
        txtCarcassWONo.Text = DataBaseAccess.ExecuteScalarCommandText(string.Format("select dbo.FPLNGetNewCarcassWONo('{0}')", Shared.Utility.ConstantClass.PLN_PREFIX_CARCASSWODETAIL)).ToString();
        txtCreateby.Text = Shared.Utility.SharedObject.UserInfo.UserPid + " - " + Shared.Utility.SharedObject.UserInfo.EmpName;
        txtDateCreate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
      }


    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@TransactionPid", DbType.Int64, this.TransactionPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNSelectWoCarcassDetail", inputParam);
      listItemCode = new ArrayList();
      listRevision = new ArrayList();
      listCarcassCode = new ArrayList();
      listWo = new ArrayList();
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        this.LoadMainData(dsSource.Tables[0]);
        DataSet ds = this.CreateDataSet();
        ds.Tables["dtParent"].Merge(dsSource.Tables[1]);
        ds.Tables["dtChild"].Merge(dsSource.Tables[2]);
        ds.Tables["dtChildSec"].Merge(dsSource.Tables[3]);
        ultData.DataSource = ds;
      }
      this.SetStatusControl(this.status);
      this.SetColor();
      this.NeedToSave = false;
    }

    private bool CheckValid(out string errorMessage)
    {

      errorMessage = string.Empty;
      DataTable dt = ((DataSet)ultData.DataSource).Tables[1];
      DataTable dtPA = ((DataSet)ultData.DataSource).Tables[0];
      if (ultDTFromDate.Value == null)
      {
        errorMessage = "Date From";
        return false;
      }
      if (ultDTDateTo.Value == null)
      {
        errorMessage = "Date To";
        return false;
      }
      if (this.importing == 1)
      {
        for (int k = 0; k < dt.Rows.Count; k++)
        {
          DataRow drc = dt.Rows[k];
          if (DBConvert.ParseInt(drc["Error"].ToString()) == 1)
          {
            this.importing = 2;
            errorMessage = "WO,ItemCode,Revision,CarcassCode";
          }
        }
      }
      if (ckbComfirm.Checked == true)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          DataRow dr = dt.Rows[i];
          if (dr.RowState != DataRowState.Deleted)
          {
            if (DBConvert.ParseInt(dr["Error"].ToString()) == 2)
            {
              errorMessage = "Qty ";
              return false;
            }
            if (DBConvert.ParseInt(dr["Qty"].ToString()) != DBConvert.ParseInt(dr["ConfirmQty"].ToString()))
            {
              errorMessage = "Qty,ConfirmQty";
              return false;
            }
            else if (DBConvert.ParseInt(dr["Error"].ToString()) == 1)
            {
              errorMessage = "WO,ItemCode,Revision,CarcassCode";
              return false;
            }
          }
        }
        for (int i = 0; i < dtPA.Rows.Count; i++)
        {
          DataRow dr = dtPA.Rows[i];
          if (dr.RowState != DataRowState.Deleted)
          {
            if (DBConvert.ParseInt(dr["Error"].ToString()) == 1)
            {
              errorMessage = "Qty";
              return false;
            }
          }
        }

      }
      return true;
    }


    private bool SaveMain()
    {

      return false;
    }

    private bool SaveParent()
    {
      bool success = true;
      int transPid = 0;
      // 1. Insert/Update      

      DBParameter[] inputParam = new DBParameter[7];
      string cm = string.Format("SELECT [dbo].[FPLNGetNewTransactionNo]('{0}')", Shared.Utility.ConstantClass.PLN_PREFIX_CARCASSWODETAIL);
      inputParam[1] = new DBParameter("@CarcassWONo", DbType.String, DataBaseAccess.ExecuteScalarCommandText(cm).ToString());
      if (this.TransactionPid > 0 && this.TransactionPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.TransactionPid);
        inputParam[1] = new DBParameter("@CarcassWONo", DbType.String, txtCarcassWONo.Text.ToString());
      }
      inputParam[2] = new DBParameter("@FromDate", DbType.DateTime, ultDTFromDate.Value.ToString());
      inputParam[3] = new DBParameter("@ToDate", DbType.DateTime, ultDTDateTo.Value.ToString());
      if (this.ckbComfirm.Checked == true)
      {
        switch (status)
        {
          case 0:
            inputParam[4] = new DBParameter("@Status", DbType.Int32, 1);
            break;
          case 1:
            inputParam[4] = new DBParameter("@Status", DbType.Int32, 2);
            break;
          case 2:
            inputParam[4] = new DBParameter("@Status", DbType.Int32, 3);
            break;
        }
      }
      else
      {
        inputParam[4] = new DBParameter("@Status", DbType.Int32, this.status);
      }
      inputParam[5] = new DBParameter("@Remark", DbType.String, txtRemark.Text.ToString());
      inputParam[6] = new DBParameter("@CreateBy", DbType.Int64, SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNCarcassWOMaster_Edit", inputParam, outputParam);
      if ((outputParam == null) || DBConvert.ParseInt(outputParam[0].Value.ToString()) <= 0)
      {
        success = false;
      }
      else
      {
        transPid = DBConvert.ParseInt(outputParam[0].Value.ToString());
      }
      this.TransactionPid = transPid;
      success = this.SaveChild(transPid);
      return success;
    }
    private DataTable EraseDeleteData(DataTable dt)
    {
      DataTable dtItem = new DataTable();
      dtItem.Merge(dt);
      dtItem.Clear();
      foreach (DataRow row in dt.Rows)
      {
        DataRow dr = dtItem.NewRow();
        if (row.RowState != DataRowState.Deleted)
        {

          if (DBConvert.ParseLong(row["Pid"].ToString()) > 0)
          {
            dr["Pid"] = DBConvert.ParseLong(row["Pid"].ToString());
          }
          if (DBConvert.ParseLong(row["TransactionPid"].ToString()) > 0)
          {
            dr["TransactionPid"] = DBConvert.ParseLong(row["TransactionPid"].ToString());
          }
          if (DBConvert.ParseLong(row["WO"].ToString().Trim()) > 0)
          {
            dr["WO"] = row["WO"].ToString();
          }
          dr["Itemcode"] = row["Itemcode"].ToString();

          if (row["Revision"].ToString().Length > 0 && DBConvert.ParseInt(row["Revision"].ToString()) >= 0)
          {
            dr["Revision"] = row["Revision"].ToString();
          }

          if (row["CarcassCode"].ToString().Length > 0)
          {
            dr["CarcassCode"] = row["CarcassCode"].ToString();
          }
          if (row["LoadingDate"].ToString().Length > 0 && DBConvert.ParseDateTime(row["LoadingDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            dr["LoadingDate"] = row["LoadingDate"].ToString();
          }
          if (row["WO"].ToString().Length > 0)
          {
            dr["WO"] = row["WO"].ToString();
          }
          if (row["Container"].ToString().Length > 0)
          {
            dr["Container"] = row["Container"].ToString();
          }
          if (row["Qty"].ToString().Length > 0 && DBConvert.ParseInt(row["Qty"].ToString()) > 0)
          {
            dr["Qty"] = row["Qty"].ToString();
          }
          if (row["ConfirmQty"].ToString().Length > 0)
          {
            dr["ConfirmQty"] = row["ConfirmQty"].ToString();
          }
          if (DBConvert.ParseDouble(row["CBM"].ToString()) > 0)
          {
            dr["CBM"] = DBConvert.ParseDouble(row["CBM"].ToString());
          }
          dr["WorkStation"] = DBConvert.ParseInt(row["WorkStation"].ToString());
          dr["StandByEN"] = row["StandByEN"].ToString();
          if (row["DeadlineProcess"].ToString().Length > 0 && DBConvert.ParseDateTime(row["DeadlineProcess"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            dr["DeadlineProcess"] = row["DeadlineProcess"].ToString();
          }
          if (row["DeadlinePacking"].ToString().Length > 0 && DBConvert.ParseDateTime(row["DeadlinePacking"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            dr["DeadlinePacking"] = row["DeadlinePacking"].ToString();
          }
          if (row["ConfirmDeadline"].ToString().Length > 0 && DBConvert.ParseDateTime(row["ConfirmDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            dr["ConfirmDeadline"] = DBConvert.ParseDateTime(row["ConfirmDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }
          dr["PLNRemark"] = row["PLNRemark"].ToString();
          if (DBConvert.ParseInt(row["CarcassConfirm"].ToString()) > 0)
          {
            dr["CarcassConfirm"] = DBConvert.ParseInt(row["CarcassConfirm"].ToString());
          }
          else
          {
            dr["CarcassConfirm"] = 0;
          }
          dr["CarcassRemark"] = row["CarcassRemark"].ToString();
          dr["Error"] = row["Error"].ToString();
          dtItem.Rows.Add(dr);
        }

      }
      return dtItem;
    }
    private bool SaveChild(long transactionPid)
    {
      bool success = true;

      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("sp_DeleteTblPLNCarcassWODetail", deleteParam, outputParam);
        if (DBConvert.ParseLong(outputParam[0].Value) <= 0)
        {
          success = false;
        }
      }

      // 2. delete parent detail

      for (int i = 0; i < listCarcassCode.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[5];
        deleteParam[0] = new DBParameter("@CarcassCode", DbType.String, this.listCarcassCode[i]);
        if (DBConvert.ParseInt(this.listWo[i]) > 0)
        {
          deleteParam[1] = new DBParameter("@WO", DbType.Int32, this.listWo[i]);
        }
        if (DBConvert.ParseInt(this.listRevision[i]) > 0)
        {
          deleteParam[2] = new DBParameter("@Revision", DbType.Int32, this.listRevision[i]);
        }
        deleteParam[3] = new DBParameter("@ItemCode", DbType.String, this.listItemCode[i]);
        deleteParam[4] = new DBParameter("@TransactionPid", DbType.Int64, this.TransactionPid);
        DataBaseAccess.ExecuteStoreProcedure("sp_DeleteTblPLNParentCarcassWODetail", deleteParam, outputParam);
        if (DBConvert.ParseLong(outputParam[0].Value) <= 0)
        {
          success = false;
        }
      }
      // 3. Insert or update Detail
      DataTable dtDetail = ((DataSet)ultData.DataSource).Tables[1];
      DataTable dtItem = this.EraseDeleteData(dtDetail);
      if (dtItem.Rows.Count > 0)
      {
        SqlDBParameter[] inputParam = new SqlDBParameter[2];
        inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dtItem);
        inputParam[1] = new SqlDBParameter("@TransactionPid", SqlDbType.Int, transactionPid);
        SqlDBParameter[] outputParam_save = new SqlDBParameter[1];
        outputParam_save[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);
        SqlDataBaseAccess.ExecuteStoreProcedure("spPLNCarcassWODetail_Edit", inputParam, outputParam_save);
        if (outputParam_save == null || DBConvert.ParseInt(outputParam_save[0].Value.ToString()) <= 0)
        {
          success = false;
        }
      }

      // 4. Refresh
      DBParameter[] inputParam_refresh = new DBParameter[1];
      inputParam_refresh[0] = new DBParameter("@TransactionPid", DbType.Int64, this.TransactionPid);
      DBParameter[] outputParam_refresh = new DBParameter[1];
      outputParam_refresh[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNCarcassWORefreshFuniture", inputParam_refresh, outputParam_refresh);
      if (outputParam_refresh == null || DBConvert.ParseInt(outputParam_refresh[0].Value.ToString()) <= 0)
      {
        success = false;
      }
      return success;
    }



    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("WO", typeof(System.Int64));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("FurSelected", typeof(System.Int32));
      taParent.Columns.Add("TotalQtyCorfirm", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("TransactionPid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("LoadingDate", typeof(System.DateTime));
      taChild.Columns.Add("Container", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Int32));
      taChild.Columns.Add("ConfirmQty", typeof(System.Int32));
      taChild.Columns.Add("CBM", typeof(System.Double));
      taChild.Columns.Add("WorkStation", typeof(System.Int32));
      taChild.Columns.Add("StandByEN", typeof(System.String));
      taChild.Columns.Add("DeadlineProcess", typeof(System.DateTime));
      taChild.Columns.Add("DeadlinePacking", typeof(System.DateTime));
      taChild.Columns.Add("ConfirmDeadline", typeof(System.DateTime));
      taChild.Columns.Add("PLNRemark", typeof(System.String));
      taChild.Columns.Add("CarcassConfirm", typeof(System.Int32));
      taChild.Columns.Add("CarcassRemark", typeof(System.String));
      ds.Tables.Add(taChild);


      DataTable taChildsec = new DataTable("dtChildSec");
      taChildsec.Columns.Add("WorkOrder", typeof(System.Int64));
      taChildsec.Columns.Add("ItemCode", typeof(System.String));
      taChildsec.Columns.Add("Revision", typeof(System.Int32));
      taChildsec.Columns.Add("CarcassCode", typeof(System.String));
      taChildsec.Columns.Add("FurnitureCode", typeof(System.String));
      taChildsec.Columns.Add("FunitureExists", typeof(System.Int32));
      ds.Tables.Add(taChildsec);

      ds.Relations.Add(new DataRelation("dtParent_dtChildDiff", new DataColumn[] { taParent.Columns["ItemCode"], taParent.Columns["Revision"], taParent.Columns["CarcassCode"], taParent.Columns["WO"] }, new DataColumn[] { taChild.Columns["ItemCode"], taChild.Columns["Revision"], taChild.Columns["CarcassCode"], taChild.Columns["WO"] }, false));

      ds.Relations.Add(new DataRelation("dtParent_dtChildSec", new DataColumn[] { taParent.Columns["ItemCode"], taParent.Columns["Revision"], taParent.Columns["CarcassCode"], taParent.Columns["WO"] }, new DataColumn[] { taChildsec.Columns["ItemCode"], taChildsec.Columns["Revision"], taChildsec.Columns["CarcassCode"], taChildsec.Columns["WorkOrder"] }, false));

      return ds;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        if (this.SaveParent())
        {
          success = true;
        }
        else
        {
          success = false;
        }
        if (success == true && this.importing == 2)
        {
          WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        }
        else if (success && this.importing != 2)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {

      base.SaveAndClose();
      this.SaveData();
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {

        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }

      for (int j = 0; j < e.Layout.Bands[1].Columns.Count; j++)
      {
        // Set Align column
        Type colType = e.Layout.Bands[1].Columns[j].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[j].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[1].Columns[j].Format = "#,##0.##";
        }
      }
      for (int k = 0; k < e.Layout.Bands[2].Columns.Count; k++)
      {
        e.Layout.Bands[2].Columns[k].CellActivation = Activation.ActivateOnly;
        // Set Align column
        Type colType = e.Layout.Bands[2].Columns[k].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[2].Columns[k].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[2].Columns[k].Format = "#,##0.##";
        }
      }

      e.Layout.Bands[0].Columns["Error"].Hidden = true;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["TransactionPid"].Hidden = true;
      e.Layout.Bands[1].Columns["WorkStation"].Hidden = true;
      e.Layout.Bands[1].Columns["Error"].Hidden = true;

      e.Layout.Bands[2].Columns["FunitureExists"].Hidden = true;

      e.Layout.Bands[0].Columns["FurSelected"].Header.Caption = "Total Furniture";

      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[1].Columns["LoadingDate"].Header.Caption = "Loading Date";
      e.Layout.Bands[1].Columns["WO"].Header.Caption = "Work Order";
      e.Layout.Bands[1].Columns["ConfirmQty"].Header.Caption = "Confirm Qty";
      e.Layout.Bands[1].Columns["StandByEN"].Header.Caption = "Stand By EN";
      e.Layout.Bands[1].Columns["DeadlineProcess"].Header.Caption = "Deadline Process";
      e.Layout.Bands[1].Columns["DeadlinePacking"].Header.Caption = "Deadline Packing";
      e.Layout.Bands[1].Columns["PLNRemark"].Header.Caption = "PLN Remark";
      e.Layout.Bands[1].Columns["CarcassRemark"].Header.Caption = "Carcass Remark";
      e.Layout.Bands[1].Columns["CarcassConfirm"].Header.Caption = "Carcass Confirm";

      e.Layout.Bands[1].Columns["CarcassConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = (this.status == 0 && this.depart == 1) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "ConfirmQty":
          string cm = string.Format(@"SELECT WD.Qty FROM TblPLNCarcassWODetail WD WHERE WD.Pid = {0}", e.Cell.Row.Cells["Pid"].Value.ToString());
          string qty = DataBaseAccess.ExecuteScalarCommandText(cm).ToString();
          if (DBConvert.ParseInt(value) > DBConvert.ParseInt(qty))
          {
            WindowUtinity.ShowMessageError("ERR0001", "QtyConfirm");
            e.Cancel = true;
          }
          break;
        case "Qty":

          string QtyConfirm = e.Cell.Row.Cells["ConfirmQty"].Value.ToString();
          if (DBConvert.ParseInt(value) != DBConvert.ParseInt(QtyConfirm))
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.importing = 0;
      this.SaveData();
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (e.Rows[0].HasParent())
      {
        if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
        {
          e.Cancel = true;
          return;
        }

        foreach (UltraGridRow row in e.Rows)
        {
          this.SetNeedToSave();
          long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (pid != long.MinValue)
          {
            this.listDeletedPid.Add(pid);
          }
        }
      }
      else
      {
        if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
        {
          e.Cancel = true;
          return;
        }
        this.SetNeedToSave();
        this.listItemCode.Add(e.Rows[0].Cells["ItemCode"].Value.ToString());
        this.listWo.Add(DBConvert.ParseInt(e.Rows[0].Cells["WO"].Value.ToString()));
        this.listCarcassCode.Add(e.Rows[0].Cells["CarcassCode"].Value.ToString());
        this.listRevision.Add(DBConvert.ParseInt(e.Rows[0].Cells["Revision"].Value.ToString()));
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }
    private void btnGetTemp_Click(object sender, EventArgs e)
    {
      string templateName = "WoCarcassDetailTemplate";
      string sheetName = "Allocation";
      string outFileName = "WoCarcassDetailTemplate";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }
    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcel.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Allocation (1)$E3:E4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0].ToString());
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), string.Format("SELECT * FROM [Allocation (1)$B5:L{0}]", itemCount + 5));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.importing = 1;
      this.FillDataForGrid(dsItemList.Tables[0]);
      this.SaveData();
    }
    private void btnBrowser_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (this.depart == 1)
      {
        bool selected = false;
        try
        {
          selected = ultData.Selected.Rows[0].Selected;
        }
        catch
        {
          selected = false;
        }
        if (!selected)
        {
          return;
        }
        UltraGridRow row = (ultData.Selected.Rows[0].HasParent()) ? ultData.Selected.Rows[0].ParentRow.ChildBands[0].Rows[0] : ultData.Selected.Rows[0].ChildBands[0].Rows[0];
        long Pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        ViewPLN_15_001 view = new ViewPLN_15_001();
        view.CarcassWOPid = Pid;
        WindowUtinity.ShowView(view, "Carcass WO Furniture", true, ViewState.ModalWindow);
        this.LoadData();
      }
    }
    private void btnReturn_Click(object sender, EventArgs e)
    {
      string storeName = "spPLNCarcassWoMaster_Status";

      DataTable dtDetail = ((DataSet)ultData.DataSource).Tables[1];
      DataTable dtItem = this.EraseDeleteData(dtDetail);
      if (dtItem.Rows.Count > 0)
      {
        SqlDBParameter[] inputParam = new SqlDBParameter[2];
        inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dtItem);
        inputParam[1] = new SqlDBParameter("@Status", SqlDbType.Int, 0);
        SqlDBParameter[] outputParam = new SqlDBParameter[1];
        outputParam[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);
        SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
        if (outputParam[0] == null || DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
        {
          WindowUtinity.ShowMessageErrorFromText("Can't Return");
          return;
        }
        this.LoadData();
      }
    }
    private void btnFinish_Click(object sender, EventArgs e)
    {
      string storeName = "spPLNCarcassWOMaster_Confirm";
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.TransactionPid);
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam[0] == null || DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't Confirm");
        return;
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      this.LoadData();
    }
    private void ultDTDateTo_ValueChanged(object sender, EventArgs e)
    {
      if (ultDTFromDate.Value.ToString().Length > 0 && ultDTDateTo.Value.ToString().Length > 0)
      {
        string weekOfYear = this.WeekofYear(Convert.ToDateTime(ultDTFromDate.Value.ToString())) + " - " + this.WeekofYear(Convert.ToDateTime(ultDTDateTo.Value.ToString()));
        txtWeekNo.Text = weekOfYear;
      }
    }
    private void ultDTFromDate_ValueChanged(object sender, EventArgs e)
    {
      if (ultDTFromDate.Value.ToString().Length > 0 && ultDTDateTo.Value.ToString().Length > 0)
      {
        string weekOfYear = this.WeekofYear(Convert.ToDateTime(ultDTFromDate.Value.ToString())) + " - " + this.WeekofYear(Convert.ToDateTime(ultDTDateTo.Value.ToString()));
        txtWeekNo.Text = weekOfYear;
      }
    }
    #endregion Event

    private DataTable CreateData()
    {
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("Container", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      taParent.Columns.Add("ConfirmQty", typeof(System.Int32));
      taParent.Columns.Add("LoadingDate", typeof(System.DateTime));
      taParent.Columns.Add("CBM", typeof(System.Double));
      taParent.Columns.Add("WorkStation", typeof(System.String));
      taParent.Columns.Add("DeadlineProcess", typeof(System.DateTime));
      taParent.Columns.Add("DeadlinePacking", typeof(System.DateTime));
      taParent.Columns.Add("ConfirmDeadline", typeof(System.DateTime));
      return taParent;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        DataTable dtExport = this.CreateData();
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow ur = ultData.Rows[i];
          for (int j = 0; j < ur.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow urc = ur.ChildBands[0].Rows[j];
            DataRow dr = dtExport.NewRow();
            dr["ItemCode"] = urc.Cells["ItemCode"].Value.ToString();
            dr["Revision"] = urc.Cells["Revision"].Value.ToString();
            dr["WO"] = urc.Cells["WO"].Value.ToString();
            dr["Container"] = urc.Cells["Container"].Value.ToString();
            dr["Qty"] = urc.Cells["Qty"].Value.ToString();
            dr["ConfirmQty"] = urc.Cells["QtyConfirm"].Value.ToString();
            dr["LoadingDate"] = urc.Cells["LoadingDate"].Value.ToString();
            dr["CBM"] = 0;
            dr["WorkStation"] = urc.Cells["StandByEN"].Value.ToString();
            dr["DeadlineProcess"] = urc.Cells["DeadlineProcess"].Value.ToString();
            dr["DeadlinePacking"] = urc.Cells["DeadlinePacking"].Value.ToString();
            dr["ConfirmDeadline"] = urc.Cells["ConfirmDeadline"].Value.ToString();
            dtExport.Rows.Add(dr);
          }
        }
        UltraGrid UG = new UltraGrid();
        UG.DataSource = dtExport;
        Controls.Add(UG);
        ControlUtility.ExportToExcelWithDefaultPath(UG, out xlBook, "Adjust List", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 2] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("B1", "B1");

        xlSheet.Cells[2, 2] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 2] = "Carcass WO List";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 4] = "Row: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 5] = DBConvert.ParseInt(UG.Rows.Count.ToString());
        xlSheet.Cells[4, 6] = "(You can change rows count if you want)";

        //xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }



  }
}
