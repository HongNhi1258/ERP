/*
   Author  : Võ Hoa Lu
   Email   : luvh_it@daico-furniture.com
   Date    : 25/06/2010
   Company : Dai Co
   Update  : Hà Anh
*/
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_02_002 : MainUserControl
  {
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public long woPid = long.MinValue;
    public int isCloseWo = 0;
    public DataTable dtNewDetail;
    public DataRow[] dtRowDetail;
    private DataTable dtSource;
    private IList listDeleteDetailPid = new ArrayList();
    private int loopFlag = 0;
    private bool workConfirmed;
    private int focusRow = int.MinValue;
    private IList listDeletingPid = new ArrayList();
    private int boolLoadData = 0;
    private DataTable dtUpdateQty = new DataTable();
    private int checkSubCon = 0;
    private bool loadedGrid = false;
    /// <summary>
    /// init view
    /// </summary>
    public viewPLN_02_002()
    {
      InitializeComponent();
      this.workConfirmed = false;
    }

    /// <summary>
    /// load view 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_002_Load(object sender, EventArgs e)
    {
      this.Search();
    }
    public void Search()
    {
      //string strf = this.GetType().ToString();
      Utility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      if (this.woPid != long.MinValue)
      {
        txtWo.Enabled = false;
      }
      else
      {
        btnConfirmDetail.Enabled = false;
        txtWo.Enabled = true;
      }
      txtCreateBy.Enabled = false;
      txtCreateDate.Enabled = false;
      this.LoadData();
      this.DefaultValue();
      this.NeedToSave = false;
      this.generate_ColumnName();
      this.LoadTotalOfLabel();
      if (this.isCloseWo == 1)
      {
        btnAddDetail.Enabled = false;
      }
    }
    /// <summary>
    /// load label total qty and cbm
    /// </summary>
    private void LoadTotalOfLabel()
    {
      DataTable dt = (DataTable)dgvData.DataSource;
      double totalCbm = 0;
      int totalQty = 0;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState != DataRowState.Deleted)
        {
          if (dt.Rows[i]["CBM"].ToString().Length > 0)
          {
            totalCbm += DBConvert.ParseDouble(dt.Rows[i]["CBM"].ToString()) * DBConvert.ParseDouble(dt.Rows[i]["Qty"].ToString());
          }
          if (dt.Rows[i]["Qty"].ToString().Length > 0)
          {
            totalQty += DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString());
          }
        }
      }
      if (totalCbm > 0)
      {
        lbTotalCBM.Text = totalCbm.ToString();
      }
      else
      {
        lbTotalCBM.Text = string.Empty;
      }
      if (totalQty > 0)
      {
        lbTotalOpenQty.Text = totalQty.ToString();
      }
      else
      {
        lbTotalOpenQty.Text = string.Empty;
      }
    }

    #region Load Data
    /// <summary>
    /// default value
    /// </summary>
    private void DefaultValue()
    {
      if (this.woPid == long.MinValue)
      {
        txtWo.Text = "";
        txtCreateBy.Text = Shared.Utility.SharedObject.UserInfo.EmpName;
        txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
      }
    }

    /// <summary>
    /// load data
    /// </summary>
    private void LoadData()
    {
      if (woPid != long.MinValue)
      {
        this.tableLayoutPanel5.Visible = true;
      }
      this.loadedGrid = false;
      this.listDeleteDetailPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@WoInfoPID", DbType.Int64, this.woPid);
      inputParam[1] = new DBParameter("@NeedToSwap", DbType.Int32, 0);
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNWorkOrderInfomationByPid", inputParam);
      DataTable dtWoInfo = dsSource.Tables[0];
      if (dtWoInfo.Rows.Count > 0)
      {
        DataRow row = dtWoInfo.Rows[0];
        txtWo.Text = row["Pid"].ToString();
        txtCreateDate.Text = row["CreateDate"].ToString();
        try
        {
          cmbType.SelectedValue = row["Type"];
        }
        catch { }
        txtCreateBy.Text = row["CreateBy"].ToString();
        txtRemark.Text = row["Remark"].ToString();
        workConfirmed = (DBConvert.ParseInt(row["Confirm"].ToString()) == 1);
      }
      this.SetStatusControl();
      dgvData.DataSource = dsSource.Tables[1];

      if (this.workConfirmed)
      {
        for (int i = 0; i < dgvData.Rows.Count; i++)
        {
          string itemCode = dgvData.Rows[i].Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(dgvData.Rows[i].Cells["Revision"].Value.ToString());
          string commandText = "SELECT  COUNT(DISTINCT WCD.Pid) ";
          commandText += " FROM TblPLNWOInfoDetailGeneral INFO ";
          commandText += " INNER JOIN TblPLNWorkOrderConfirmedDetails WCD ON WCD.WorkOrderPid = INFO.WoInfoPID AND WCD.ItemCode = INFO.ItemCode AND WCD.Revision = INFO.Revision ";
          commandText += " WHERE WCD.WorkOrderPid = " + woPid + " AND WCD.ItemCode = '" + itemCode + "' AND WCD.Revision =" + revision;
          int test = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText));
          if (test == 1)
          {
            dgvData.Rows[i].Cells["IsConfirm"].Value = 1;
          }
          else
          {
            dgvData.Rows[i].Cells["IsConfirm"].Value = 0;
          }
        }

      }
      else
      {

        dgvData.DisplayLayout.Bands[0].Columns["IsConfirm"].Hidden = true;
        dgvData.DisplayLayout.Bands[0].Columns["WIP Run"].Hidden = true;
        dgvData.DisplayLayout.Bands[0].Columns["Allocated"].Hidden = true;
      }

      this.LoadTotalOfLabel();

      for (int i = 0; i < dgvData.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        string columnName = dgvData.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if ((string.Compare(columnName, "Qty", true) == 0) || (string.Compare(columnName, "Priority", true) == 0) || (string.Compare(columnName, "SubCon", true) == 0) || (string.Compare(columnName, "Note", true) == 0) || (string.Compare(columnName, "ShipDate(Release Wo)", true) == 0) || (string.Compare(columnName, "ShipDate", true) == 0))
        {
          dgvData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
        {
          dgvData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }
      //lock confirm khi bom chua confirm
      for (int i = 0; i < dgvData.Rows.Count; i++)
      {
        string itemCode = dgvData.Rows[i].Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(dgvData.Rows[i].Cells["Revision"].Value.ToString());
        string commandText = "SELECT Confirm FROM TblBOMItemInfo WHERE ItemCode = '" + itemCode + "' AND Revision =" + revision;
        int confirmBOM = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText));
        if (confirmBOM == 0)
        {
          dgvData.Rows[i].Cells["IsConfirm"].Activation = Activation.ActivateOnly;
        }
        else
        {
          dgvData.Rows[i].Cells["IsConfirm"].Activation = Activation.AllowEdit;
        }
      }
      this.loadedGrid = true;
      for (int a = 0; a < dgvData.Rows.Count; a++)
      {
        if (dgvData.Rows[a].Cells["Flag"].Value.ToString() == "1")
        {
          dgvData.Rows[a].CellAppearance.BackColor = Color.Yellow;
        }
      }
      // Hien Wo chua Confirm be nhat nam trong khoang ma Wo da chon
      if (woPid != long.MinValue)
      {
        long woMin = long.MinValue;
        if (woPid > 90000)
        {
          string commandText = "SELECT MIN(Pid ) FROM TblPLNWorkOrder WHERE ISNULL(Confirm,0) = 0 AND Pid > 90000";
          woMin = DBConvert.ParseLong(DataBaseAccess.ExecuteScalarCommandText(commandText));
        }
        else
        {
          string commandText = "SELECT MIN(Pid ) FROM TblPLNWorkOrder WHERE ISNULL(Confirm,0) = 0 AND Pid < 90000";
          woMin = DBConvert.ParseLong(DataBaseAccess.ExecuteScalarCommandText(commandText));
        }
        txtWOMin.Text = woMin.ToString();
      }
    }

    /// <summary>
    /// set Status
    /// </summary>
    private void SetStatusControl()
    {
      cmbType.Enabled = !this.workConfirmed;
      chkConfirm.Enabled = !this.workConfirmed;
      chkConfirm.Checked = this.workConfirmed;
      btnSetDeadline.Enabled = this.workConfirmed;
    }

    #endregion

    #region Save And Check

    /// <summary>
    /// load workoder for update
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    private PLNWorkOrder LoadWorkOrderByPid(long pid)
    {
      PLNWorkOrder workOrder = new PLNWorkOrder();
      if (pid == long.MinValue)
      {
        return new PLNWorkOrder();
      }
      workOrder.Pid = this.woPid;
      workOrder = (PLNWorkOrder)Shared.DataBaseUtility.DataBaseAccess.LoadObject(workOrder, new string[] { "Pid" });
      return workOrder;
    }

    /// <summary>
    /// save work order
    /// </summary>
    /// <returns></returns>
    private long SaveWorkOrder()
    {
      PLNWorkOrder workOrder = this.LoadWorkOrderByPid(this.woPid);
      if (workOrder == null)
      {
        return long.MinValue;
      }
      if (this.woPid != long.MinValue)
      {
        workOrder.UpdateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
        workOrder.UpdateDate = DateTime.Now;
      }
      else
      {
        workOrder.CreateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
        workOrder.CreateDate = DateTime.Now;
        workOrder.Pid = DBConvert.ParseLong(txtWo.Text.ToString());
      }
      workOrder.Type = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbType));
      workOrder.Status = 0;
      workOrder.Confirm = (chkConfirm.Checked) ? 1 : 0;
      if (chkConfirm.Checked && dgvData.Rows.Count == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0001");
        return long.MinValue;
      }
      string strBKRemark = "";
      if (workOrder.Remark.Trim() != txtRemark.Text.Trim())
      {
        strBKRemark = workOrder.Remark.Trim();
      }
      workOrder.Remark = txtRemark.Text.Trim();

      if (this.woPid == long.MinValue)
      {
        Shared.DataBaseUtility.DataBaseAccess.InsertObject(workOrder);
        return workOrder.Pid;
      }
      else
      {
        if (strBKRemark != "")
        {
          //backup remark
          string strStoreName = "spPLNBackupRemarkForWorkOrder";
          DBParameter[] inputParam = new DBParameter[3];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          inputParam[0] = new DBParameter("@WoPid", DbType.Int64, this.woPid);
          inputParam[1] = new DBParameter("@Remark", DbType.AnsiString, 4000, strBKRemark);
          inputParam[2] = new DBParameter("@UserPid", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(strStoreName, inputParam, outputParam);
          long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (outValue == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          }
        }

        bool success = Shared.DataBaseUtility.DataBaseAccess.UpdateObject(workOrder, new string[] { "Pid" });

        // Tron update (06/08/2013)
        string commandCount = string.Format(@"SELECT COUNT(*)
                                              FROM TblPLNWorkOrderConfirmedDetails 
                                              WHERE WorkOrderPid = {0}", this.woPid);
        int countConfirm = (int)DataBaseAccess.ExecuteScalarCommandText(commandCount);
        if (countConfirm > 0)
        {
          string commandUpdateConfirm = string.Format("Update TblPLNWorkOrder Set Confirm = 1 Where Pid = {0}", this.woPid);
          DataBaseAccess.ExecuteScalarCommandText(commandUpdateConfirm);
        }
        // End Tron update (06/08/2013)

        return (success) ? workOrder.Pid : long.MinValue;
      }
    }

    /// <summary>
    /// save detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      DataTable dt = (DataTable)dgvData.DataSource;
      if (!dtUpdateQty.Columns.Contains("WorkOrder"))
      {
        dtUpdateQty.Columns.Add("WorkOrder", typeof(Int64));
        dtUpdateQty.Columns.Add("ItemCode", typeof(string));
        dtUpdateQty.Columns.Add("Revision", typeof(Int32));
      }
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState == DataRowState.Modified)
        {
          dt.Rows[i]["RowState"] = 1;
        }
      }
      bool result = true;
      PLNWOInfoDetail workDetai = new PLNWOInfoDetail();
      // 1. Delete
      if (this.listDeleteDetailPid.Count > 0)
      {
        foreach (long pid in this.listDeleteDetailPid)
        {
          workDetai.Pid = pid;
          DBParameter[] inputDelete = new DBParameter[1];
          DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          inputDelete[0] = new DBParameter("@Pid", DbType.Int64, pid);
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNWOInfoDetail_Delete", inputDelete, outputDelete);
          long Value = DBConvert.ParseLong(outputDelete[0].Value.ToString());
          if (Value == 0)
          {
            result = false;
          }
        }
      }

      // 2. Insert/ Update
      int count = dgvData.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        string storeName = string.Empty;
        DBParameter[] inputParam = new DBParameter[12];
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        UltraGridRow row = dgvData.Rows[i];
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid == long.MinValue)
        {
          storeName = "spPLNWOInfoDetail_Insert";
          inputParam[8] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        }
        else
        {
          int rowState = DBConvert.ParseInt(row.Cells["RowState"].Value.ToString());
          if (rowState == 1)
          {
            storeName = "spPLNWOInfoDetail_Update";
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            inputParam[8] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          }
        }
        if (storeName.Length > 0)
        {
          inputParam[1] = new DBParameter("@WoInfoPID", DbType.Int64, this.woPid);
          string itemCode = row.Cells["ItemCode"].Value.ToString().Trim().Replace("'", "''");
          inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          int value = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          inputParam[3] = new DBParameter("@Revision", DbType.Int32, value);
          value = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
          inputParam[4] = new DBParameter("@Qty", DbType.Int32, value);
          value = DBConvert.ParseInt(row.Cells["Priority"].Value.ToString());
          if (value != int.MinValue)
          {
            inputParam[5] = new DBParameter("@Priority", DbType.Int32, value);
          }
          long saleOrderDetailPid = DBConvert.ParseLong(row.Cells["SaleOrderDetailPid"].Value.ToString());
          inputParam[6] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleOrderDetailPid);

          int temp = int.MinValue;
          try
          {
            temp = DBConvert.ParseInt(row.Cells["IsSubCon"].Value.ToString());
          }
          catch { }
          int subCon = (temp == 0 || temp == int.MinValue) ? 0 : 1;
          inputParam[7] = new DBParameter("@IsSubCon", DbType.Int32, subCon);
          DateTime DeadLineDate = new DateTime();
          if (row.Cells["ShipDate"].Value.ToString().Trim().Length > 0)
          {
            DeadLineDate = DBConvert.ParseDateTime(row.Cells["ShipDate"].Value.ToString(), formatConvert);
            inputParam[11] = new DBParameter("@DeadLineDate", DbType.DateTime, DeadLineDate);
          }

          if (row.Cells["Note"].Value.ToString().Length > 0)
          {
            inputParam[9] = new DBParameter("@Note", DbType.AnsiString, 256, row.Cells["Note"].Value.ToString());
          }
          if (row.Cells.Exists("IsConfirm"))
          {
            inputParam[10] = new DBParameter("@IsConfirm", DbType.Int32, DBConvert.ParseInt(row.Cells["IsConfirm"].Value.ToString()));
          }
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (outValue == -1)
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0002", DBConvert.ParseString(i + 1));
            result = false;
          }
          else if (outValue == -2)
          {
            boolLoadData = 1;
            dgvData.Rows[i].Appearance.BackColor = Color.Yellow;
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0102", "This Item Code");
            result = false;
          }
          else if (outValue == 0)
          {
            result = false;
          }

          //check ton tai trong workconfirmdetail ko và update QTY
          if (woPid > 0)
          {
            string search_text = "SELECT COUNT (Pid) FROM TblPLNWorkOrderConfirmedDetails where WorkOrderPid = " + woPid + " AND ItemCode= '" + itemCode + "' AND Revision =" + DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
            int check = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(search_text));
            if (check == 1)
            {
              if (dtUpdateQty != null)
              {
                check = 0;
                for (int k = 0; k < dtUpdateQty.Rows.Count; k++)
                {
                  if (dtUpdateQty.Rows[k]["WorkOrder"].ToString() == woPid.ToString() && dtUpdateQty.Rows[k]["ItemCode"].ToString() == itemCode && dtUpdateQty.Rows[k]["Revision"].ToString() == row.Cells["Revision"].Value.ToString())
                  {
                    check = 1;
                    break;
                  }
                }
              }
              if (check == 0)
              {
                DataRow newrow = dtUpdateQty.NewRow();
                newrow["WorkOrder"] = woPid;
                newrow["ItemCode"] = itemCode;
                newrow["Revision"] = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
                dtUpdateQty.Rows.Add(newrow);
              }
            }
          }
        }
      }
      return result;
    }

    /// <summary>
    /// save data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      string message = string.Empty;
      if (!this.CheckValid())
      {
        return false;
        this.dtRowDetail = null;
      }
      bool result = true;
      long pid = this.SaveWorkOrder();
      result = pid > 0;
      if (result)
      {
        this.woPid = pid;
        result = this.SaveDetail();
        if (result)
        {
          for (int k = 0; k < dtUpdateQty.Rows.Count; k++)
          {
            DBParameter[] inputParam = new DBParameter[5];
            inputParam[1] = new DBParameter("@WorkOrder", DbType.Int64, DBConvert.ParseLong(dtUpdateQty.Rows[k]["WorkOrder"].ToString()));
            inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, dtUpdateQty.Rows[k]["ItemCode"].ToString());
            inputParam[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dtUpdateQty.Rows[k]["Revision"].ToString()));
            inputParam[4] = new DBParameter("@ConfirmBy", DbType.Int32, SharedObject.UserInfo.UserPid);

            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrderConfirmDetail_Update", inputParam, output);
            if (DBConvert.ParseInt(output[0].Value.ToString()) == 1)
            {
              result = true;
            }
            else
            {
              result = false;
            }
          }
        }
        if (result && chkConfirm.Checked)
        {
          //Expose cong thuc cua carcass component doi voi wo
          result = this.CastDataWhenWorkConfirmed();
        }
      }
      if (result)
      {
        //this.NeedToSave = true;
        this.SaveSuccess = true;
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.btnConfirmDetail.Enabled = true;
      }
      else
      {
      }

      this.LoadData();

      return result;
    }

    /// <summary>
    /// Get CarcassCode of ItemCode
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    private string GetCarcassCode(string itemCode)
    {
      string commandText = string.Format("Select CarcassCode From TblBOMItemInfo Where ItemCode LIKE '{0}'", itemCode);
      object objCarcass = Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandText);
      if (objCarcass != null)
        return objCarcass.ToString();
      else
        return string.Empty;
    }

    /// <summary>
    /// check valid
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      // Check WO NO 
      if (this.woPid == long.MinValue)
      {
        if (txtWo.Text.Length <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0013", "WorkOrder No");
          return false;
        }
        else
        {
          string strExistWo = "Select COUNT(*) from TblPLNWorkOrder where Pid = " + txtWo.Text;
          if (Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(strExistWo) == null)
          {
            return false;
          }
          strExistWo = Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(strExistWo).ToString();
          if (strExistWo != "0")
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0028", "WorkOrder No " + txtWo.Text);
            return false;

          }
        }
      }
      // Check requird Type: 
      if (cmbType.SelectedIndex <= 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Type");
        return false;
      }
      // Check remain PO :
      DataTable dtCheck = (DataTable)dgvData.DataSource;
      DataRow[] rows = dtCheck.Select("Qty < 0 OR Qty > Remain OR Priority > 1000");


      if (rows.Length > 0)
      {
        string message = string.Format("Qty is invalid (qty > 0 and qty <= remain), please check at ItemCode = {0}", rows[0]["ItemCode"]);
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText(message);
        return false;
      }
      DataRow[] rowPrioritys = dtCheck.Select("Priority > 1000");
      if (rowPrioritys.Length > 0)
      {
        string message = string.Format("Priority is invalid , please check at ItemCode = {0}", rows[0]["ItemCode"]);
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText(message);
        return false;
      }

      //Check Qty Item must = Qty at Confirm time
      if (this.woPid > 0 && this.workConfirmed)
      {
        string cmdWorkConfirmDetail = "SELECT * FROM TblPLNWorkOrderConfirmedDetails WHERE WorkOrderPid = " + this.woPid;
        DataTable dtWorkDetailsConfirm = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cmdWorkConfirmDetail);
        int countRowChecked = 0;
        int rowDeleted = 0;
        foreach (DataRow rowConfirm in dtWorkDetailsConfirm.Rows)
        {
          string itemCodeCheck = DBConvert.ParseString(rowConfirm["ItemCode"]);
          int revisionCheck = DBConvert.ParseInt(rowConfirm["Revision"]);
          int qtyCheck = DBConvert.ParseInt(rowConfirm["Qty"]);
          DataRow[] rowsDetails = dtCheck.Select(string.Format("ItemCode = '{0}' AND Revision = {1}", itemCodeCheck, revisionCheck));
          int qtySource = 0;
          foreach (DataRow row in rowsDetails)
          {
            int qty = DBConvert.ParseInt(row["Qty"]);
            qtySource += qty > 0 ? qty : 0;
            countRowChecked++;
          }
          //if (qtyCheck != qtySource) {
          //  string message = string.Format("Quantity of item {0} not equal at confirm time, please check again", itemCodeCheck);
          //  Shared.Utility.WindowUtinity.ShowMessageErrorFromText(message);
          //  return false;
          //}
        }

        foreach (DataRow rowDelete in dtCheck.Rows)
        {
          if (rowDelete.RowState == DataRowState.Deleted)
            rowDeleted++;
        }
      }
      return true;
    }

    /// <summary>
    /// copy data when work confirm
    /// </summary>
    /// <returns></returns>
    private bool CastDataWhenWorkConfirmed()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Wo", DbType.Int64, this.woPid) };
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNCastWorkDetailsWhenConfirm", inputParam, outputParam);
      int success = DBConvert.ParseInt(outputParam[0].Value.ToString());
      return (success == 1 ? true : false);
    }

    #endregion

    #region Event
    /// <summary>
    /// save and close
    /// </summary>
    //public override void SaveAndClose()
    //{
    //  base.SaveAndClose();
    //  SaveData();
    //}

    /// <summary>
    /// add detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddDetail_Click(object sender, EventArgs e)
    {
      this.dtRowDetail = null;
      viewPLN_02_003 uc = new viewPLN_02_003();
      //uc.parentUC = this;
      DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "Work Order Information", false, ViewState.ModalWindow, FormWindowState.Maximized);
      this.dtSource = (DataTable)dgvData.DataSource;
      if (this.dtRowDetail == null)
      {
        return;
      }
      //for (int i = 0; i < this.dtNewDetail.Rows.Count; i++)
      //{
      for (int i = 0; i < this.dtRowDetail.Length; i++)
      {
        //DataRow newRow = this.dtNewDetail.Rows[i];
        int openQty = DBConvert.ParseInt(dtRowDetail[i]["OpenQty"].ToString());
        if (openQty > 0)
        {
          long saleOrderDetailPid = DBConvert.ParseLong(dtRowDetail[i]["SaleOrderDetailPid"].ToString());
          bool existDetailPid = false;
          for (int j = 0; j < dgvData.Rows.Count; j++)
          {
            long value = DBConvert.ParseLong(dgvData.Rows[j].Cells["SaleOrderDetailPid"].Value.ToString());
            if (value == saleOrderDetailPid)
            {
              int qty = DBConvert.ParseInt(dgvData.Rows[j].Cells["Qty"].Value.ToString());
              qty = (qty == int.MinValue) ? openQty : qty + openQty;
              dgvData.Rows[j].Cells["Qty"].Value = qty;
              dgvData.Rows[j].Cells["RowState"].Value = 1;
              if (qty > DBConvert.ParseInt(dgvData.Rows[j].Cells["Remain"].Value.ToString()))
              {
                dgvData.Rows[j].Appearance.BackColor = Color.Yellow;
              }
              existDetailPid = true;
              break;
            }
          }
          if (!existDetailPid)
          {
            DataRow row = dtSource.NewRow();
            row["ItemCode"] = dtRowDetail[i]["ItemCode"];
            row["Revision"] = dtRowDetail[i]["Revision"];
            row["CarcassCode"] = dtRowDetail[i]["CarcassCode"];
            row["SaleNo"] = dtRowDetail[i]["SaleNo"];
            row["CustomerPONo"] = dtRowDetail[i]["Customer's PO No"];
            row["SaleOrderDetailPid"] = dtRowDetail[i]["SaleOrderDetailPid"];
            row["ScheduleDelivery"] = dtRowDetail[i]["ScheduleDelivery"];
            row["Remain"] = dtRowDetail[i]["Remain"];
            row["Qty"] = dtRowDetail[i]["OpenQty"];
            row["CBM"] = dtRowDetail[i]["CBM Remain"];
            row["RowState"] = 1;
            row["IsSubCon"] = dtRowDetail[i]["ContractOut"];

            //techConfirm
            string commandText = "SELECT Confirm FROM TblBOMItemInfo WHERE ItemCode = '" + dtRowDetail[i]["ItemCode"] + "' AND Revision =" + dtRowDetail[i]["Revision"];
            int confirmBOM = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText));
            row["IsConfirm"] = 0;
            row["WIP Run"] = 0;
            row["Allocated"] = 0;
            dtSource.Rows.Add(row);
          }
        }
      }
      this.generate_ColumnName();
      this.LoadTotalOfLabel();
    }

    /// <summary>
    /// generate column name Ha Anh add them
    /// </summary>
    private void generate_ColumnName()
    {
      DataTable dtcolum = new DataTable();
      DataTable dt_grid = (DataTable)dgvData.DataSource;
      dtcolum.Columns.Add("ColName");
      dtcolum.Columns.Add("DisplayName");
      if (dt_grid != null)
      {
        for (int c = 0; c < dt_grid.Columns.Count; c++)
        {
          if (dgvData.DisplayLayout.Bands[0].Columns[dt_grid.Columns[c].ColumnName].Hidden == false)
          {
            DataRow dr = dtcolum.NewRow();
            dr["ColName"] = dt_grid.Columns[c].ColumnName;
            dr["DisplayName"] = dgvData.DisplayLayout.Bands[0].Columns[dt_grid.Columns[c].ColumnName].Header.Caption;
            dtcolum.Rows.Add(dr);
          }
        }
      }
    }

    /// <summary>
    /// save click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.SaveData())
      {
        if (boolLoadData != 1)
        {
          this.LoadData();
        }
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// set deadline
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSetDeadline_Click(object sender, EventArgs e)
    {
      viewPLN_02_004 uc = new viewPLN_02_004();
      uc.woPid = this.woPid;
      Shared.Utility.WindowUtinity.ShowView(uc, "WORK ORDER DEADLINE", true, Shared.Utility.ViewState.MainWindow);
    }

    /// <summary>
    /// close click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {

      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// grid cell change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void dgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
      {
        if (dgvData.DisplayLayout.Bands[0].Columns["IsSubCon"].Index == e.ColumnIndex && e.RowIndex >= 0)
        {
          int value = DBConvert.ParseInt(dgvData.Rows[e.RowIndex].Cells["IsSubCon"].Value);
          string carcassCode = dgvData.Rows[e.RowIndex].Cells["CarcassCode"].Value.ToString();
          DataTable dtCheck = (DataTable)dgvData.DataSource;
          DataRow[] rows = dtCheck.Select(string.Format("CarcassCode ='{0}'", carcassCode));
          foreach (DataRow row in rows)
          {
            row["IsSubCon"] = value;
          }
        }
        dgvData.Rows[e.RowIndex].Cells["RowState"].Value = 1;
      }
      this.LoadTotalOfLabel();
    }

    /// <summary>
    /// check show image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
    }

    #endregion Event
    /// <summary>
    /// remark text change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtRemark_TextChanged(object sender, EventArgs e)
    {
      //this.NeedToSave = true;
    }

    /// <summary>
    /// type selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
    {
      //this.NeedToSave = true;
    }

    private void dgvData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      Utility.SetPropertiesUltraGrid(dgvData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["WoInfoPID"].Hidden = true;
      //e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["RowState"].Hidden = true;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Schedule Date";
      e.Layout.Bands[0].Columns["ShipDate"].Header.Caption = "ShipDate(Release Wo)";
      e.Layout.Bands[0].Columns["ShipDate"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Header.Caption = "Sale No";

      e.Layout.Bands[0].Columns["CustomerPONo"].Header.Caption = "Customer's PO No";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["IsSubCon"].Header.Caption = "SubCon";
      e.Layout.Bands[0].Columns["IsConfirm"].Header.Caption = "PLN Confirm";

      e.Layout.Bands[0].Columns["IsConfirm"].MinWidth = 80;
      e.Layout.Bands[0].Columns["IsConfirm"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["WIP Run"].MinWidth = 80;
      e.Layout.Bands[0].Columns["WIP Run"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["Allocated"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Allocated"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["IsSubCon"].MinWidth = 60;
      e.Layout.Bands[0].Columns["IsSubCon"].MaxWidth = 60;

      e.Layout.Bands[0].Columns["Qty"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Priority"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Priority"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Revision"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Remain"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Remain"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["CBM"].MinWidth = 50;
      e.Layout.Bands[0].Columns["CBM"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Revision"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Priority"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Priority"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["IsConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["WIP Run"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Allocated"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
    }

    private void dgvData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeleteDetailPid.Add(pid);
      }
    }

    private void dgvData_Click(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = dgvData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = dgvData.Selected.Rows[0];
      string itemCode = row.Cells["ItemCode"].Value.ToString();
      int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
      if (itemCode.Length > 0)
      {
        if (chkShowImage.Checked)
        {
          try
          {
            string local = FunctionUtility.BOMGetItemImage(itemCode, revision);
            FunctionUtility.ShowImagePopup(local);
          }
          catch
          {
          }
        }
      }
    }

    /// <summary>
    /// add Pid để chuẩn bị delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void dgvData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {

      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
          //remove row trong workorderconfirmdetail
          if (!dtUpdateQty.Columns.Contains("WorkOrder"))
          {
            dtUpdateQty.Columns.Add("WorkOrder", typeof(Int64));
            dtUpdateQty.Columns.Add("ItemCode", typeof(string));
            dtUpdateQty.Columns.Add("Revision", typeof(Int32));
          }
          string itemcode = row.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          string search_text = "SELECT COUNT (Pid) FROM TblPLNWorkOrderConfirmedDetails where WorkOrderPid = " + woPid + " AND ItemCode= '" + itemcode + "' AND Revision =" + revision;
          int check = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(search_text));
          if (check == 1)
          {
            if (dtUpdateQty != null)
            {
              check = 0;
              for (int k = 0; k < dtUpdateQty.Rows.Count; k++)
              {
                if (dtUpdateQty.Rows[k]["WorkOrder"].ToString() == woPid.ToString() && dtUpdateQty.Rows[k]["ItemCode"].ToString() == itemcode && dtUpdateQty.Rows[k]["Revision"].ToString() == revision.ToString())
                {
                  check = 1;
                  break;
                }
              }
            }
            if (check == 0)
            {
              DataRow newrow = dtUpdateQty.NewRow();
              newrow["WorkOrder"] = woPid;
              newrow["ItemCode"] = itemcode;
              newrow["Revision"] = revision;
              dtUpdateQty.Rows.Add(newrow);
            }
          }
          if (DBConvert.ParseLong(row.Cells["Pid"].Text.ToString()) != long.MinValue && woPid != long.MinValue)
          {
            long pid_input = DBConvert.ParseLong(row.Cells["Pid"].Text.ToString());
            string itemcode_input = row.Cells["ItemCode"].Text.ToString();
            int revision_input = DBConvert.ParseInt(row.Cells["Revision"].Text.ToString());

            DBParameter[] input = new DBParameter[5];
            input[0] = new DBParameter("@Pid", DbType.Int64, pid_input);
            input[1] = new DBParameter("@WorkOrder", DbType.Int64, woPid);
            input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemcode_input);
            input[3] = new DBParameter("@Revision", DbType.Int32, revision_input);
            input[4] = new DBParameter("@IsDelete", DbType.Int32, 1);

            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrder_CheckWIP_AndAllocate", input, output);
            long outvalue = DBConvert.ParseLong(output[0].Value.ToString());
            if (outvalue == 1)
            {
              e.Cancel = true;
            }
          }
        }
      }
    }

    private void dgvData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (loopFlag == 1)
      {
        return;
      }
      int index = e.Cell.Row.Index;
      string colName = e.Cell.Column.ToString().ToLower();
      if (colName == "shipdate" && chkAllShipdate.Checked)
      {
        loopFlag = 1;
        for (int i = 0; i < dgvData.Rows.Count; i++)
        {
          dgvData.Rows[i].Cells["RowState"].Value = 1;
          dgvData.Rows[i].Cells["ShipDate"].Value = dgvData.Rows[index].Cells["ShipDate"].Value;
        }
        loopFlag = 0;
      }
      this.LoadTotalOfLabel();
      if (colName == "issubcon")
      {
        if (checkSubCon == 1)
        {
          return;
        }
        for (int i = 0; i < dgvData.Rows.Count; i++)
        {
          if (e.Cell.Row.Cells["ItemCode"].Value.ToString() == dgvData.Rows[i].Cells["ItemCode"].Value.ToString())
          {
            checkSubCon = 1;
            dgvData.Rows[i].Cells["RowState"].Value = 1;
            dgvData.Rows[i].Cells["IsSubCon"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["IsSubCon"].Value.ToString());
          }
        }
        checkSubCon = 0;
      }
    }

    private void dgvData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      for (int i = 0; i < dgvData.Rows.Count; i++)
      {
        dgvData.Rows[i].Appearance.BackColor = Color.White;
      }
      string columnName = e.Cell.Column.ToString().ToLower();
      if (columnName == "qty")
      {
        int newQty = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Text.ToString());
        if (newQty > DBConvert.ParseInt(e.Cell.Row.Cells["Remain"].Value.ToString()))
        {
          //e.Cell.Row.Appearance.BackColor = Color.Yellow;
          Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", "Qty");
          e.Cancel = true;
        }
        if (newQty <= 0)
        {
          //e.Cell.Row.Appearance.BackColor = Color.Yellow;
          Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", "Qty");
          e.Cancel = true;
        }
        else
        {
          if (e.Cell.Row.Cells["IsConfirm"].Text == "1")
          {
            int check_wip_run = DBConvert.ParseInt(e.Cell.Row.Cells["WIP Run"].Text.ToString());
            string query = "SELECT Qty FROM TblPLNWOInfoDetailGeneral WHERE Pid =" + DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Text.ToString());
            int oldqty = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(query));
            if (oldqty > newQty)
            {
              if (e.Cell.Row.Cells["WIP Run"].Text.ToString() == "1")
              {
                e.Cell.Row.Appearance.BackColor = Color.Yellow;
                Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0102", "decrease Qty of this Item Code");
                e.Cancel = true;
              }
            }
            else
            {
              e.Cell.Row.Appearance.BackColor = Color.White;
            }
          }
        }
      }
    }

    private void btnConfirmDetail_Click(object sender, EventArgs e)
    {
      viewPLN_02_010 view_010 = new viewPLN_02_010();
      view_010.workOrder = woPid;
      DaiCo.Shared.Utility.WindowUtinity.ShowView(view_010, "Work Order Confirm Detail", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
      this.LoadData();
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      DataTable dtDetail = (DataTable)dgvData.DataSource;
      if (dtDetail.Columns.Contains("Pid"))
      {
        dtDetail.Columns.Remove("Pid");
      }
      if (dtDetail.Columns.Contains("SaleOrderDetailPid"))
      {
        dtDetail.Columns.Remove("SaleOrderDetailPid");
      }
      if (dtDetail.Rows.Count > 0)
      {
        //Lay dtMain ra sài
        string strTemplateName = "WorkOrderDetailInformationTemplate";
        string strSheetName = "Sheet1";
        string strOutFileName = "Work Order Detail Informaiton";
        string strStartupPath = System.Windows.Forms.Application.StartupPath;
        string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
        string strPathTemplate = strStartupPath + @"\ExcelTemplate";
        XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

        if (this.workConfirmed)
        {
          oXlsReport.Cell("**19").Value = "Official";
        }
        else
        {
          oXlsReport.Cell("**19").Value = "Draff";
        }
        for (int i = 0; i < dtDetail.Rows.Count; i++)
        {
          DataRow dtRow = dtDetail.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("A4:R4").Copy();
            oXlsReport.RowInsert(3 + i);
            oXlsReport.Cell("A4:R4", 0, i).Paste();
          }
          oXlsReport.Cell("**1", 0, i).Value = i + 1;
          oXlsReport.Cell("**2", 0, i).Value = dtRow["WoInfoPID"].ToString();
          oXlsReport.Cell("**3", 0, i).Value = dtRow["SaleNo"].ToString();
          oXlsReport.Cell("**4", 0, i).Value = dtRow["CustomerPONo"].ToString();
          oXlsReport.Cell("**5", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**6", 0, i).Value = dtRow["Revision"].ToString();
          oXlsReport.Cell("**7", 0, i).Value = dtRow["CarcassCode"].ToString();
          oXlsReport.Cell("**8", 0, i).Value = dtRow["ScheduleDelivery"].ToString();
          oXlsReport.Cell("**9", 0, i).Value = dtRow["Remain"].ToString();
          oXlsReport.Cell("**10", 0, i).Value = dtRow["CBM"].ToString();
          oXlsReport.Cell("**11", 0, i).Value = dtRow["Qty"].ToString();
          oXlsReport.Cell("**12", 0, i).Value = dtRow["Priority"].ToString();
          oXlsReport.Cell("**13", 0, i).Value = dtRow["IsSubCon"].ToString();
          oXlsReport.Cell("**14", 0, i).Value = dtRow["Note"].ToString();
          oXlsReport.Cell("**15", 0, i).Value = dtRow["RowState"].ToString();
          oXlsReport.Cell("**16", 0, i).Value = dtRow["WIP Run"].ToString();
          oXlsReport.Cell("**17", 0, i).Value = dtRow["Allocated"].ToString();
          oXlsReport.Cell("**18", 0, i).Value = dtRow["IsConfirm"].ToString();
        }
        oXlsReport.Out.File(strOutFileName);
        Process.Start(strOutFileName);
      }
    }

    static public void releaseObject(object obj)
    {
      try
      {
        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        obj = null;
      }
      catch (Exception ex)
      {
        obj = null;
        throw new Exception("Exception Occured while releasing object " + ex.ToString());
      }
      finally
      {
        GC.Collect();
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      viewPLN_02_006 uc = new viewPLN_02_006();
      Shared.Utility.WindowUtinity.ShowView(uc, "ReLoad Carcass", true, Shared.Utility.ViewState.MainWindow);
    }

    private void dgvData_DoubleClick(object sender, EventArgs e)
    {
      try
      {
        if (chkNeedToSwap.Checked == true)
        {
          viewPLN_02_011 view = new viewPLN_02_011();
          view.WoDetail = DBConvert.ParseLong(dgvData.Selected.Rows[0].Cells["Pid"].Value.ToString());
          view.Priority = DBConvert.ParseInt(dgvData.Selected.Rows[0].Cells["Priority"].Value.ToString());
          view.WorkOrderPID = DBConvert.ParseLong(txtWo.Text.Trim());
          Shared.Utility.WindowUtinity.ShowView(view, "SWAP WORK ORDER", false, Shared.Utility.ViewState.ModalWindow);
          this.chkNeedToSwap.Checked = true;
          this.LoadData();
          chkNeedToSwap_CheckedChanged(sender, e);
        }
      }
      catch
      { }
    }

    private void chkNeedToSwap_CheckedChanged(object sender, EventArgs e)
    {
      this.loadedGrid = false;
      this.listDeleteDetailPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@WoInfoPID", DbType.Int64, this.woPid);
      if (chkNeedToSwap.Checked)
      {
        inputParam[1] = new DBParameter("@NeedToSwap", DbType.Int32, 1);
      }
      else
      {
        inputParam[1] = new DBParameter("@NeedToSwap", DbType.Int32, 0);
      }
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNWorkOrderInfomationByPid", inputParam);
      DataTable dtWoInfo = dsSource.Tables[0];
      if (dtWoInfo.Rows.Count > 0)
      {
        DataRow row = dtWoInfo.Rows[0];
        txtWo.Text = row["Pid"].ToString();
        txtCreateDate.Text = row["CreateDate"].ToString();
        try
        {
          cmbType.SelectedValue = row["Type"];
        }
        catch { }
        txtCreateBy.Text = row["CreateBy"].ToString();
        txtRemark.Text = row["Remark"].ToString();
        workConfirmed = (DBConvert.ParseInt(row["Confirm"].ToString()) == 1);
      }

      this.SetStatusControl();
      dgvData.DataSource = dsSource.Tables[1];

      if (this.workConfirmed)
      {
        for (int i = 0; i < dgvData.Rows.Count; i++)
        {
          string itemCode = dgvData.Rows[i].Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(dgvData.Rows[i].Cells["Revision"].Value.ToString());
          string commandText = "SELECT  COUNT(DISTINCT WCD.Pid) ";
          commandText += " FROM TblPLNWOInfoDetailGeneral INFO ";
          commandText += " INNER JOIN TblPLNWorkOrderConfirmedDetails WCD ON WCD.WorkOrderPid = INFO.WoInfoPID AND WCD.ItemCode = INFO.ItemCode AND WCD.Revision = INFO.Revision ";
          commandText += " WHERE WCD.WorkOrderPid = " + woPid + " AND WCD.ItemCode = '" + itemCode + "' AND WCD.Revision =" + revision;
          int test = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText));
          if (test == 1)
          {
            dgvData.Rows[i].Cells["IsConfirm"].Value = 1;
          }
          else
          {
            dgvData.Rows[i].Cells["IsConfirm"].Value = 0;
          }
        }
      }
      else
      {
        dgvData.DisplayLayout.Bands[0].Columns["IsConfirm"].Hidden = true;
        dgvData.DisplayLayout.Bands[0].Columns["WIP Run"].Hidden = true;
        dgvData.DisplayLayout.Bands[0].Columns["Allocated"].Hidden = true;
      }

      this.LoadTotalOfLabel();

      for (int i = 0; i < dgvData.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        string columnName = dgvData.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if ((string.Compare(columnName, "Qty", true) == 0) || (string.Compare(columnName, "Priority", true) == 0) || (string.Compare(columnName, "SubCon", true) == 0) || (string.Compare(columnName, "Note", true) == 0) || (string.Compare(columnName, "ShipDate(Release Wo)", true) == 0) || (string.Compare(columnName, "ShipDate", true) == 0))
        {
          dgvData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
        {
          dgvData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }
      //lock confirm khi bom chua confirm
      for (int i = 0; i < dgvData.Rows.Count; i++)
      {
        string itemCode = dgvData.Rows[i].Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(dgvData.Rows[i].Cells["Revision"].Value.ToString());
        string commandText = "SELECT Confirm FROM TblBOMItemInfo WHERE ItemCode = '" + itemCode + "' AND Revision =" + revision;
        int confirmBOM = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText));
        if (confirmBOM == 0)
        {
          dgvData.Rows[i].Cells["IsConfirm"].Activation = Activation.ActivateOnly;
        }
        else
        {
          dgvData.Rows[i].Cells["IsConfirm"].Activation = Activation.AllowEdit;
        }
      }
      this.loadedGrid = true;
      for (int a = 0; a < dgvData.Rows.Count; a++)
      {
        if (dgvData.Rows[a].Cells["Flag"].Value.ToString() == "1")
        {
          dgvData.Rows[a].CellAppearance.BackColor = Color.Yellow;
        }
      }
      this.btnConfirmDetail.Enabled = !chkNeedToSwap.Checked;
      this.btnAddDetail.Enabled = !chkNeedToSwap.Checked;
    }

    private void btnSwap_Click(object sender, EventArgs e)
    {
      if (WindowUtinity.ShowMessageConfirm("MSG0056").ToString() == "No")
      {
        return;
      }
      else
      {
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@WoOld", DbType.Int64, woPid);
        inputParam[1] = new DBParameter("@WoNew", DbType.Int64, DBConvert.ParseLong(txtWOMin.Text.Trim()));
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrder_Swap", inputParam, outputParam);
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result == 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0132", woPid.ToString());
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0055");
        }
      }
    }

    private void dgvData_BeforeRowFilterChanged(object sender, BeforeRowFilterChangedEventArgs e)
    {

    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < dgvData.Rows.Count; i++)
      {
        dgvData.Rows[i].Cells["IsSubCon"].Value = chkSelectAll.Checked;
      }
    }

    private void btnCapture_Click(object sender, EventArgs e)
    {
      FunctionUtility.CaptureForm(this);
    }
  }
}
