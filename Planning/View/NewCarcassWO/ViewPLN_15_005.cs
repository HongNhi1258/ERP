/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: ViewPLN_15_005
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
  public partial class ViewPLN_15_005 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    public long AdjustPid;
    private int depart;
    private int status;
    #endregion Field

    #region Init
    public ViewPLN_15_005()
    {
      InitializeComponent();
    }

    private void ViewPLN_15_005_Load(object sender, EventArgs e)
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
      this.InitData();
      this.LoadCBCarcassWo();
      this.LoadData();
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
    private void LoadCBCarcassWo()
    {
      string cm = @"select Pid,CarcassWONo from TblPLNCarcassWOMaster where [Status] = 3";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraCombo(ultCarcassWO, dt, "Pid", "CarcassWONo", false, "Pid");
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
            if (row["WO"].ToString().Length > 0 && DBConvert.ParseLong(row["WO"].ToString()) > 0)
            {
              rowItemSource["WO"] = row["WO"].ToString();
            }
            if (row["Container"].ToString().Length > 0)
            {
              rowItemSource["Container"] = row["Container"].ToString();
            }
            //rowItemSource["Qty"] = 0;
            //rowItemSource["ConfirmQty"] = 0;
            if (row["AdjustQty"].ToString().Length > 0 && DBConvert.ParseInt(row["AdjustQty"].ToString()) > 0)
            {
              rowItemSource["QtyAdjust"] = row["AdjustQty"].ToString();
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
            string status = "";
            if (row["Status"].ToString().Length > 0)
            {
              status = row["Status"].ToString();
            }
            int result = int.MinValue;
            if (status.ToLower().Trim().Equals("add"))
            {
              result = 0;
            }
            else if (status.ToLower().Trim().Equals("delete"))
            {
              result = 1;
            }
            if (result >= 0)
            {
              rowItemSource["Status"] = result;
            }
          }
          //rowItemSource["PLNRemark"] = row["PLNRemark"].ToString();
          dt_tmp.Rows.Add(rowItemSource);
        }
      }
      string storeName = "spPLNImportAdjustWoCarcassdetailFromExcel";
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
          if (DBConvert.ParseInt(ur.Cells["Error"].Value.ToString()) == 3)
          {
            ur.CellAppearance.BackColor = Color.Yellow;
          }
          for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow urc = ultData.Rows[i].ChildBands[0].Rows[j];
            if (DBConvert.ParseInt(urc.Cells["Qty"].Value.ToString()) != DBConvert.ParseInt(urc.Cells["ConfirmQty"].Value.ToString()))
            {
              urc.Cells["WO"].Appearance.BackColor = Color.Pink;
            }

            if (DBConvert.ParseInt(urc.Cells["Error"].Value.ToString()) == 1)
            {
              urc.CellAppearance.BackColor = Color.Yellow;
            }
            if (DBConvert.ParseInt(urc.Cells["Error"].Value.ToString()) == 2)
            {
              urc.CellAppearance.BackColor = Color.SkyBlue;
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
    private DataTable CreateData()
    {
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("Container", typeof(System.String));
      taParent.Columns.Add("AdjustQty", typeof(System.Int32));
      taParent.Columns.Add("LoadingDate", typeof(System.DateTime));
      taParent.Columns.Add("CBM", typeof(System.Double));
      taParent.Columns.Add("WorkStation", typeof(System.String));
      taParent.Columns.Add("DeadlineProcess", typeof(System.DateTime));
      taParent.Columns.Add("DeadlinePacking", typeof(System.DateTime));
      taParent.Columns.Add("ConfirmDeadline", typeof(System.DateTime));
      taParent.Columns.Add("Status", typeof(System.String));

      return taParent;
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

            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              UltraGridRow ur = ultData.Rows[i];
              ur.Activation = Activation.ActivateOnly;
              for (int k = 0; k < ur.ChildBands[0].Rows.Count; k++)
              {
                if (DBConvert.ParseInt(ur.ChildBands[0].Rows[k].Cells["Qty"].Value) != DBConvert.ParseInt(ur.ChildBands[0].Rows[k].Cells["ConfirmQty"].Value))
                {
                  ur.ChildBands[0].Band.Columns["Qty"].CellActivation = Activation.AllowEdit;
                  //ur.ChildBands[0].Rows[k].Cells["Qty"].Activation = Activation.AllowEdit;
                }
              }
              ur.ChildBands[0].Band.Columns["PLNRemark"].CellActivation = Activation.AllowEdit;
            }
            this.ultCarcassWO.ReadOnly = false;
            this.gbImportExcel.Visible = true;
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
            this.ultCarcassWO.ReadOnly = true;
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
              //ur.ChildBands[0].Band.Columns["CarcassConfirm"].CellActivation = Activation.AllowEdit;
              ur.ChildBands[0].Band.Columns["CarcassRemark"].CellActivation = Activation.AllowEdit;
              if (DBConvert.ParseInt(ur.Cells["Kind"].Value.ToString()) <= 0)
              {
                ur.ChildBands[0].Band.Columns["ConfirmQty"].CellActivation = Activation.AllowEdit;
                ur.ChildBands[0].Band.Columns["ConfirmDeadline"].CellActivation = Activation.AllowEdit;
              }
            }

            btnReturn.Visible = true;
            ckbComfirm.Enabled = true;
            ckbComfirm.Checked = false;
            btnSave.Enabled = true;
          }
          this.gbImportExcel.Visible = false;
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
          //for (int i = 0; i < ultData.Rows.Count; i++)
          //{
          //  UltraGridRow ur = ultData.Rows[i];
          //  ur.Activation = Activation.ActivateOnly;
          //  for (int j = 0; j < ur.ChildBands[0].Band.Columns.Count; j++)
          //  {
          //    ur.ChildBands[0].Band.Columns[j].CellActivation = Activation.ActivateOnly;
          //  }           
          //}

          this.gbImportExcel.Visible = false;
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

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain != null && dtMain.Rows.Count > 0)
      {
        foreach (DataRow dr in dtMain.Rows)
        {
          txtCarcassWONo.Text = dr["AdjCarcassWoNo"].ToString();
          ultCarcassWO.Value = dr["CarcassWo"].ToString();
          txtCreateby.Text = dr["CreateBy"].ToString();
          txtDateCreate.Text = dr["CreateDate"].ToString();

          this.status = DBConvert.ParseInt(dr["Status"].ToString());
        }
      }
      else
      {
        txtCarcassWONo.Text = DataBaseAccess.ExecuteScalarCommandText(string.Format("select dbo.FPLNGetNewAdjustCarcassWONo('{0}')", Shared.Utility.ConstantClass.PLN_PREFIX_ADJUST)).ToString();
        txtCreateby.Text = Shared.Utility.SharedObject.UserInfo.UserPid + " - " + Shared.Utility.SharedObject.UserInfo.EmpName;
        txtDateCreate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
      }


    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@AdjustPid", DbType.Int64, this.AdjustPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNAdjustCarcassWoDetail", inputParam);
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
      if (ultCarcassWO.Text.Trim().Length <= 0)
      {
        errorMessage = "Carcass Wo";
        return false;
      }
      errorMessage = string.Empty;
      DataTable dt = ((DataSet)ultData.DataSource).Tables[1];
      DataTable dtPA = ((DataSet)ultData.DataSource).Tables[0];
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
              errorMessage = "WO,ItemCode,Revision";
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
            else if (DBConvert.ParseInt(dr["Error"].ToString()) == 3)
            {
              errorMessage = "Data ";
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
      long transPid = 0;
      // 1. Insert/Update     
      DBParameter[] inputParam = new DBParameter[5];
      string cm = string.Format("SELECT [dbo].[FPLNGetNewAdjustCarcassWONo]('{0}')", Shared.Utility.ConstantClass.PLN_PREFIX_ADJUST);

      inputParam[1] = new DBParameter("@AjustNo", DbType.String, DataBaseAccess.ExecuteScalarCommandText(cm).ToString());
      if (this.AdjustPid > 0 && this.AdjustPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.AdjustPid);
        inputParam[1] = new DBParameter("@AjustNo", DbType.String, txtCarcassWONo.Text.ToString());
      }

      if (this.ckbComfirm.Checked == true)
      {
        switch (status)
        {
          case 0:
            inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);
            break;
          case 1:
            inputParam[2] = new DBParameter("@Status", DbType.Int32, 2);
            break;
          case 2:
            inputParam[2] = new DBParameter("@Status", DbType.Int32, 3);
            break;
        }
      }
      else
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, this.status);
      }
      if (this.ultCarcassWO.Value != null && this.ultCarcassWO.Value.ToString().Length > 0)
      {
        inputParam[3] = new DBParameter("@CarcassWOPid", DbType.Int64, ultCarcassWO.Value.ToString());
      }
      inputParam[4] = new DBParameter("@CreateBy", DbType.Int64, SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNAdjustCarcassWOMaster_Edit", inputParam, outputParam);
      if ((outputParam == null) || DBConvert.ParseInt(outputParam[0].Value.ToString()) <= 0)
      {
        success = false;
      }
      else
      {
        this.AdjustPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
      }
      if (success)
      {
        success = this.SaveChild(this.AdjustPid);
      }
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
          if (DBConvert.ParseLong(row["AdjustPid"].ToString()) > 0)
          {
            dr["AdjustPid"] = DBConvert.ParseLong(row["AdjustPid"].ToString());
          }

          dr["Itemcode"] = row["Itemcode"].ToString();
          if (row["Revision"].ToString().Length > 0 && DBConvert.ParseInt(row["Revision"].ToString()) >= 0)
          {
            dr["Revision"] = row["Revision"].ToString();
          }
          if (row["LoadingDate"].ToString().Length > 0 && DBConvert.ParseDateTime(row["LoadingDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            dr["LoadingDate"] = row["LoadingDate"].ToString();
          }
          if (row["WO"].ToString().Length > 0 && DBConvert.ParseLong(row["WO"].ToString()) > 0)
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
          if (row["QtyAdjust"].ToString().Length > 0 && DBConvert.ParseInt(row["QtyAdjust"].ToString()) > 0)
          {
            dr["QtyAdjust"] = row["QtyAdjust"].ToString();
          }
          if (row["Status"].ToString().ToLower().Equals("add"))
          {
            dr["Status"] = 0;
          }
          else if (row["Status"].ToString().ToLower().Equals("delete"))
          {
            dr["Status"] = 1;
          }
          if (DBConvert.ParseDouble(row["CBM"].ToString()) > 0)
          {
            dr["CBM"] = DBConvert.ParseDouble(row["CBM"].ToString());
          }
          if (DBConvert.ParseInt(row["WorkStation"].ToString()) > 0)
          {
            dr["WorkStation"] = DBConvert.ParseInt(row["WorkStation"].ToString());
          }
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
            dr["ConfirmDeadline"] = row["ConfirmDeadline"].ToString();
          }
          dr["PLNRemark"] = row["PLNRemark"].ToString();
          //dr["CarcassConfirm"] = DBConvert.ParseInt(row["CarcassConfirm"].ToString());
          dr["CarcassRemark"] = row["CarcassRemark"].ToString();
          dr["Error"] = DBConvert.ParseInt(row["Error"].ToString());
          dtItem.Rows.Add(dr);
        }

      }
      return dtItem;
    }
    private bool SaveChild(long AdjustPid)
    {
      bool success = true;

      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("sp_DeleteTblPLNAdjustCarcassWODetail", deleteParam, outputParam);
        if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
        {
          success = false;
        }
      }
      // 2. Insert or update Detail
      DataTable dtDetail = ((DataSet)ultData.DataSource).Tables[1];
      DataTable dtItem = this.EraseDeleteData(dtDetail);
      if (dtItem.Rows.Count > 0)
      {
        SqlDBParameter[] inputParam = new SqlDBParameter[2];
        inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dtItem);
        inputParam[1] = new SqlDBParameter("@AdjustPid", SqlDbType.Int, this.AdjustPid);
        SqlDBParameter[] outputParam_save = new SqlDBParameter[1];
        outputParam_save[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);
        SqlDataBaseAccess.ExecuteStoreProcedure("spPLNAdjustCarcassWODetail_Edit", inputParam, outputParam_save);
        if (outputParam_save == null || DBConvert.ParseInt(outputParam_save[0].Value.ToString()) <= 0)
        {
          success = false;
        }
      }
      // 3. Refresh
      DBParameter[] inputParam_refresh = new DBParameter[1];
      inputParam_refresh[0] = new DBParameter("@AdjustPid", DbType.Int64, this.AdjustPid);
      DBParameter[] outputParam_refresh = new DBParameter[1];
      outputParam_refresh[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNAdjustCarcassWORefreshFuniture", inputParam_refresh, outputParam_refresh);
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
      taParent.Columns.Add("Total", typeof(System.Int32));
      taParent.Columns.Add("QtyAdjust", typeof(System.Int32));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("Kind", typeof(System.Int32));
      taParent.Columns.Add("Error", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("AdjustPid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("Container", typeof(System.String));
      taChild.Columns.Add("LoadingDate", typeof(System.DateTime));
      taChild.Columns.Add("Qty", typeof(System.Int32));
      taChild.Columns.Add("ConfirmQty", typeof(System.Int32));
      taChild.Columns.Add("QtyAdjust", typeof(System.Int32));
      taChild.Columns.Add("Status", typeof(System.String));
      taChild.Columns.Add("CBM", typeof(System.Double));
      taChild.Columns.Add("WorkStation", typeof(System.Int32));
      taChild.Columns.Add("StandByEN", typeof(System.String));
      taChild.Columns.Add("DeadlineProcess", typeof(System.DateTime));
      taChild.Columns.Add("DeadlinePacking", typeof(System.DateTime));
      taChild.Columns.Add("ConfirmDeadline", typeof(System.DateTime));
      taChild.Columns.Add("PLNRemark", typeof(System.String));
      taChild.Columns.Add("CarcassConfirm", typeof(System.Int32));
      taChild.Columns.Add("CarcassRemark", typeof(System.String));
      taChild.Columns.Add("Error", typeof(System.Int32));
      ds.Tables.Add(taChild);

      DataTable taChildSec = new DataTable("dtChildSec");
      taChildSec.Columns.Add("WorkOrder", typeof(System.Int64));
      taChildSec.Columns.Add("ItemCode", typeof(System.String));
      taChildSec.Columns.Add("Revision", typeof(System.Int32));
      taChildSec.Columns.Add("FurnitureCode", typeof(System.String));
      taChildSec.Columns.Add("Kind", typeof(System.Int32));
      ds.Tables.Add(taChildSec);

      ds.Relations.Add(new DataRelation("dtParent_dtChildDiff", new DataColumn[] { taParent.Columns["ItemCode"], taParent.Columns["Revision"], taParent.Columns["WO"], taParent.Columns["Status"] }, new DataColumn[] { taChild.Columns["ItemCode"], taChild.Columns["Revision"], taChild.Columns["WO"], taChild.Columns["Status"] }, false));

      ds.Relations.Add(new DataRelation("dtParent_dtChildSec", new DataColumn[] { taParent.Columns["ItemCode"], taParent.Columns["Revision"], taParent.Columns["WO"], taParent.Columns["Kind"] }, new DataColumn[] { taChildSec.Columns["ItemCode"], taChildSec.Columns["Revision"], taChildSec.Columns["WorkOrder"], taChildSec.Columns["Kind"] }, false));
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
        if (success)
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
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
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

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["AdjustPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Error"].Hidden = true;
      e.Layout.Bands[1].Columns["QtyAdjust"].Hidden = true;
      e.Layout.Bands[1].Columns["WorkStation"].Hidden = true;

      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["Kind"].Hidden = true;

      e.Layout.Bands[2].Columns["Kind"].Hidden = true;


      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["LoadingDate"].Header.Caption = "Loading Date";
      e.Layout.Bands[1].Columns["WO"].Header.Caption = "Work Order";
      e.Layout.Bands[1].Columns["ConfirmQty"].Header.Caption = "Confirm Qty";
      e.Layout.Bands[1].Columns["StandByEN"].Header.Caption = "WorkArea";
      e.Layout.Bands[1].Columns["DeadlineProcess"].Header.Caption = "Deadline Process";
      e.Layout.Bands[1].Columns["DeadlinePacking"].Header.Caption = "Deadline Packing";
      e.Layout.Bands[1].Columns["PLNRemark"].Header.Caption = "PLN Remark";
      e.Layout.Bands[1].Columns["CarcassRemark"].Header.Caption = "Carcass Remark";

      //e.Layout.Bands[1].Columns["CarcassConfirm"].Header.Caption = "Carcass Confirm";
      //e.Layout.Bands[1].Columns["CarcassConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

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
          string cm = string.Format(@"SELECT WD.Qty FROM TblPLNAdjustCarcassWODetail WD WHERE WD.Pid = {0}", e.Cell.Row.Cells["Pid"].Value.ToString());
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
      this.SaveData();
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
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }
    private void btnGetTemp_Click(object sender, EventArgs e)
    {
      string templateName = "AdjustCarcassWoTemplate";
      string sheetName = "Allocation";
      string outFileName = "AdjustCarcassWoTemplate";
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
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), string.Format("SELECT * FROM [Allocation (1)$A5:K{0}]", itemCount + 5));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
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
        ViewPLN_15_006 view = new ViewPLN_15_006();
        view.AdjustDetailPid = Pid;
        view.Status = this.status;
        WindowUtinity.ShowView(view, "Adjust Furniture Code", true, ViewState.ModalWindow);
        this.LoadData();

      }
    }
    private void btnReturn_Click(object sender, EventArgs e)
    {
      string storeName = "spPLNAdjustCarcassWoMaster_Status";
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
    private void ultCarcassWO_ValueChanged(object sender, EventArgs e)
    {
      //string storeName ="spTBLPLNSelectCarcassWO";
      //long pid = DBConvert.ParseInt(ultCarcassWO.Value.ToString());
      //DBParameter[] inputParam = new DBParameter[1];
      //inputParam[0] = new DBParameter("@AdjustPid", DbType.Int64, pid);
      //DataSet ds = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      //DataSet dsSource = this.CreateDataSet();
      //dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
      //dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
      //ultData.DataSource = dsSource;
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
            dr["AdjustQty"] = urc.Cells["QtyAdjust"].Value.ToString();
            dr["LoadingDate"] = urc.Cells["LoadingDate"].Value.ToString();
            dr["CBM"] = 0;
            dr["WorkStation"] = urc.Cells["StandByEN"].Value.ToString();
            dr["DeadlineProcess"] = urc.Cells["DeadlineProcess"].Value.ToString();
            dr["DeadlinePacking"] = urc.Cells["DeadlinePacking"].Value.ToString();
            dr["ConfirmDeadline"] = urc.Cells["ConfirmDeadline"].Value.ToString();
            dr["Status"] = urc.Cells["Status"].Value.ToString();
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

        xlSheet.Cells[3, 2] = "Adjust List";
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
    private void btnFinish_Click(object sender, EventArgs e)
    {
      string storeName = "spPLNAdjustCarcassWOMaster_Confirm";
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.AdjustPid);
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
  }
  #endregion Event



}

