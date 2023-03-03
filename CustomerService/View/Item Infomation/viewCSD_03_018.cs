/*
  Author      : Nguyen Chi Cuong
  Date        : 27/07/2015
  Description : Save Transaction  SaleCode
  Standard Form: viewCSD_03_018
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms; 
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using VBReport;
using System.Diagnostics;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_018 : MainUserControl
  {
    #region Field
    public long viewTransactionPid = long.MinValue;
    private bool canUpdate = false;
    public bool isperApproved = false;
    private IList listDeletedPid = new ArrayList();
    private bool isDuplicateProcess = false;
    public int Status = 0;
    private long create = long.MinValue;
    //private bool  rslt=true;
    #endregion Field

    #region Init
    public viewCSD_03_018()
    {
      InitializeComponent();
    }

    private void viewCSD_03_018_Load(object sender, EventArgs e)
    {     
      this.InitData();
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //Load data Item SaleCode
      LoadDDItemCode();
      this.LoadDDReason();
    }

    private void LoadDDItemCode()
    {
      string commandtext = string.Format(@" SELECT ItemCode, Name, SaleCode FROM TblBOMItemBasic  ");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandtext);
      ControlUtility.LoadUltraDropDown(ultraDDItem, dt, "ItemCode", "ItemCode");
      ultraDDItem.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadDDReason()
    {
      string commandtext = string.Format(@" SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 251116  ");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandtext);
      ControlUtility.LoadUltraDropDown(ultddReason, dt, "Code", "Value", "Code");
      ultddReason.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadDataComponent(DataSet dsSource)
    {
      ultData.DataSource = dsSource.Tables[1];
      int count = ultData.Rows.Count;

      if (!this.canUpdate)
      {
        for (int j = 0; j < ultData.DisplayLayout.Bands[0].Columns.Count; j++)
        {
          ultData.DisplayLayout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
          ultData.DisplayLayout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.LightGray;
        }
      }
      if (isperApproved && this.Status < 2)
      {
        ultData.DisplayLayout.Bands[0].Columns["Approved"].CellActivation = Activation.AllowEdit;
      }
      else
      {
        ultData.DisplayLayout.Bands[0].Columns["Approved"].CellActivation = Activation.ActivateOnly;
      }
    }

    private void SetStatusControl()
    { 
      //Not Confirm
      if (isperApproved == true)
      {
        if (this.Status == 0 && this.create != SharedObject.UserInfo.UserPid)
        {
          this.canUpdate = false;
          btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
        else
        {
          // Isn't CreateBy
          if (this.create != SharedObject.UserInfo.UserPid)
          {
            this.canUpdate = false;
          }
          else
          {
            if (this.Status == 2)
            {
              this.canUpdate = false;
            }
            else
            {
              this.canUpdate = true;
            }
          }
        }
      }
      else
      {
        if (this.Status > 0)
        {
          this.canUpdate = false;
        }
        else
        {
          this.canUpdate = true;
        }
      }
    }

    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewTransactionPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDTransactionCodeSaleCode_Select", inputParam);
      if (dsSource.Tables[0].Rows.Count > 0)
      {
        DataRow row = dsSource.Tables[0].Rows[0];
        txtTransCode.Text = row["TransactionCode"].ToString();
        txtCreateBy.Text = row["CreateBy"].ToString();
        txtApprovedBy.Text = row["ApprovedBy"].ToString();
        txtCreateDate.Text = row["CreateDate"].ToString();
        txtApprovedDate.Text = row["ApprovedDate"].ToString();
        txtRemark.Text = row["Remark"].ToString();
        this.create = DBConvert.ParseLong(row["CreateBy1"].ToString());
        this.Status = DBConvert.ParseInt(row["Status"].ToString());
        if (this.Status == 0)
        {
          lbSatus.Text = "New";
        }
        else if (Status == 1)
        {
          if (isperApproved == true)
          {
            lbSatus.Text = "Waiting For Approved";
            txtRemark.ReadOnly = true;
            chkConfirm.Checked = true;
            chkConfirm.Enabled = false;
          }
          else
          {
            chkConfirm.Enabled = false;
            btnSave.Enabled = false;
            lbSatus.Text = "Waiting For Approved";
            chkConfirm.Checked = true;
            txtRemark.ReadOnly = true;
          }
        }
        else
        {
          lbSatus.Text = "Approved";
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          chkConfirm.Checked = true;
          txtRemark.ReadOnly = true;
        }
        if (isperApproved == true)
        {
          chkAutoAll.Enabled = true;
        }
      }
      else
      {
        DataTable dtPR = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FGNRGetNewTransactionCodeSaleCode('CSC') NewTrans");
        if ((dtPR != null) && (dtPR.Rows.Count > 0))
        {
          this.txtTransCode.Text = dtPR.Rows[0]["NewTrans"].ToString();
          this.txtCreateBy.Text = SharedObject.UserInfo.EmpName;
          this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          lbSatus.Text = "New";
          this.create = SharedObject.UserInfo.UserPid;
        }
      }
      if (isperApproved == true)
      {
        chkAutoAll.Enabled = true;
        btnSave.Text = "Approved";
      }
      SetStatusControl();
      LoadDataComponent(dsSource);
    }

    private bool CheckVaild(out string errorMessage)
    {
      errorMessage = string.Empty;
      if (txtTransCode.Text.Length  ==  0)
      {
        errorMessage = "TransactionCode";
        return false;
      }
      //Duplicate
      if (this.ProcessData())
      {
        errorMessage = "Duplicate Data";
        return false;
      }


      // Check ItemCode
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        string ItemCode = row.Cells["ItemCode"].Value.ToString();
        string NewSaleCode = row.Cells["NewSaleCode"].Value.ToString();   

        if (row.Cells["ItemCode"].Value.ToString().Trim().Length == 0)
        {
          row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
          errorMessage = "Item Code";
          return false;
        }       
        else
        {
          string commandText = string.Empty;
          commandText += " SELECT ItemCode";
          commandText += " FROM TblBOMItemBasic ";
          commandText += " WHERE ItemCode   ='" + ItemCode + "'";
          DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtItem.Rows.Count == 0 || dtItem == null)
          {
            row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
            errorMessage = "Item Code";
            return false;
          }
        }

        if (row.Cells["Reason"].Value.ToString().Trim().Length == 0)
        {
          row.Cells["Reason"].Appearance.BackColor = Color.Yellow;
          errorMessage = "Reason";
          return false;
        }
        else
        {
          string commandText = string.Empty;
          commandText += " SELECT Code";
          commandText += " FROM TblBOMCodeMaster ";
          commandText += " WHERE [Group] = 251116 AND Code   = " + DBConvert.ParseInt(row.Cells["Reason"].Value.ToString());
          DataTable dtReason = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtReason.Rows.Count ==0 || dtReason == null)
          {
            row.Cells["Reason"].Appearance.BackColor = Color.Yellow;
            errorMessage = "Reason";
            return false;
          }
        }

        // Lengh NewSaleCode <= 16
        if (row.Cells["NewSaleCode"].Value.ToString().Trim().Length > 16 || row.Cells["NewSaleCode"].Value.ToString().Trim().Length == 0)
        {
          row.Cells["NewSaleCode"].Appearance.BackColor = Color.Yellow;
          errorMessage = "New Sale Code";
          return false;
        }
        else
        {
          string commandText = string.Empty;
          commandText += string.Format(@" SELECT SaleCode
                            FROM TblBOMItemBasic
                            WHERE SaleCode = '{0}'",NewSaleCode);
          DataTable dtItemSaleCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtItemSaleCode.Rows.Count > 0)
          {
            row.Cells["NewSaleCode"].Appearance.BackColor = Color.Yellow;
            errorMessage = "New Sale Code";
            return false;
          }
        }
      }
        return true;
    }

    //Check tren luoi
    private bool CheckItem(string salecode)
    {
      bool result = true;
      int countComponent = ultData.Rows.Count;
      int count = 1;
      DataTable dt = new DataTable();
      DataTable detail = new DataTable();
      detail = (DataTable)ultData.DataSource;
      dt.Columns.Add("ItemCode");
      dt.Columns.Add("Name");
      dt.Columns.Add("OldSaleCode");
      dt.Columns.Add("NewSaleCode");
      DataRow[] foundRows = detail.Select(string.Format("NewSaleCode = '{0}'",  salecode.ToUpper()));
      if (foundRows.Length > 0)
      {
        for (int i = 0; i < foundRows.Length; i++)
        {


          DataRow row = dt.NewRow();
          row[0] = foundRows[i].ItemArray[1];
          row[1] = foundRows[i].ItemArray[2];
          row[2] = foundRows[i].ItemArray[3];
          row[3] = foundRows[i].ItemArray[4];

          dt.Rows.Add(row);
          count += count;
        }
      }
      if (count > 2)
      {
        result = false;
      }
      else
      {
        result = true;
      }
      return result;
    }


    private bool SaveMain()
    {
      DBParameter[] inputParam = new DBParameter[7];
      if (this.viewTransactionPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.viewTransactionPid);
      }

      if (txtTransCode.Text.Length  > 0)
      {
        inputParam[1] = new DBParameter("@TransactionCode", DbType.String, "CSC");
      }

      if (chkConfirm.Checked && isperApproved == true)
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 2);
        inputParam[6] = new DBParameter("@ApprovedBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        //inputParam[7] = new DBParameter("@ApprovedDate", DbType.DateTime, DateTime.Now);
      }
      else if (chkConfirm.Checked && isperApproved == false)
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);
        inputParam[6] = new DBParameter("@ApprovedBy", DbType.Int32, null);
        //inputParam[7] = new DBParameter("@ApprovedDate", DbType.DateTime, null);
      }
      else
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 0);
        inputParam[6] = new DBParameter("@ApprovedBy", DbType.Int32, null);
        //inputParam[7] = new DBParameter("@ApprovedDate", DbType.DateTime, null);
      }

      inputParam[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      
      inputParam[4] = new DBParameter("@Remark", DbType.String, txtRemark.Text.ToString());
      inputParam[5] = new DBParameter("@EditBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNTransactionCodeSaleCode_Edit", inputParam, ouputParam);

      this.viewTransactionPid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if (this.viewTransactionPid <= 0)
      {
        this.SaveSuccess = false;
        return false;
      }
      return true;
    }

    private bool SaveDetail()
    {
      bool success = true;

      // Delete Row In grid
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spCSCTransactionSaleCodeDetail_Delete", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      DataTable dt = (DataTable)ultData.DataSource;
      foreach (DataRow dr in dt.Rows)
      {
        if (dr.RowState != DataRowState.Deleted)
        {
          DBParameter[] input = new DBParameter[8];
          if (DBConvert.ParseLong(dr["Pid"].ToString()) != long.MinValue)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dr["Pid"].ToString()));
          }
          input[1] = new DBParameter("@TransactionPid", DbType.Int64, viewTransactionPid);
          input[2] = new DBParameter("@ItemCode", DbType.String, dr["ItemCode"].ToString());
          input[3] = new DBParameter("@OldSaleCode", DbType.String, dr["OldSaleCode"].ToString());
          input[4] = new DBParameter("@NewSaleCode", DbType.String, dr["NewSaleCode"].ToString());
          input[5] = new DBParameter("@Remark", DbType.String, dr["Remark"].ToString());
          input[6] = new DBParameter("@Approved", DbType.Int32, dr["Approved"].ToString());
          input[7] = new DBParameter("@Reason", DbType.Int32, dr["Reason"].ToString());

          DBParameter[] ouputParam = new DBParameter[1];
          ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spTransactionCodeDetailSaleCode_Edit", input, ouputParam);
          if (DBConvert.ParseLong(ouputParam[0].Value.ToString()) <= 0)
          {
            this.SaveSuccess = false;
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveData()
    {
      bool success = this.SaveMain();
      if (success)
      {
        success = this.SaveDetail();
        this.SaveSuccess = true;
      }
      else
      {
        success = false;
      }
      NeedToSave = false;
      return success;
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      string message = string.Empty;
      bool success = true;
      DataTable dt = (DataTable)ultData.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState == DataRowState.Modified || dt.Rows[i].RowState == DataRowState.Added)
        {
          if (dt.Rows[i]["NewSaleCode"].ToString().Length > 0)
          {
            success = this.CheckItem(dt.Rows[i]["NewSaleCode"].ToString());
          }
        }
      }
      // Check Valid
      if (success)
      {
        success = this.CheckVaild(out message);
        if (!success)
        {
          WindowUtinity.ShowMessageError("ERR0001", message);
          return;
        }
        // Save Data
        success = this.SaveData();

        //Update Sale Code
        if (isperApproved == true)
        {
          if (chkConfirm.Checked)
          {
            DBParameter[] inputParam = new DBParameter[1];
            if (this.viewTransactionPid != long.MinValue)
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.viewTransactionPid);
            }
            DBParameter[] ouputParam = new DBParameter[1];

            ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spCSDTransactionCodeUpdateSaleCode", inputParam, ouputParam);
            if (DBConvert.ParseLong(ouputParam[0].Value.ToString()) <= 0)
            {
              this.SaveSuccess = false;
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0037", "Data");
        }
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0002");
      }
    }

    private DataTable dtDeadlineResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("NewSaleCode", typeof(System.String));
      dt.Columns.Add("Remark", typeof(System.String));
      dt.Columns.Add("Reason", typeof(System.Int32));
      return dt;
    }

    private void ImportData()
    {
      if (this.txtLocation.Text.Trim().Length == 0)
      {
        return;
      }

      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), "SELECT * FROM [Sheet1 (1)$H2:H3]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), string.Format("SELECT * FROM [Sheet1 (1)$B3:E{0}]",itemCount + 3)).Tables[0];
      if (dtSource == null)
      {
        return;
      }

      DataTable dtInput = this.dtDeadlineResult();
      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtInput.NewRow();
        if (row["ItemCode"].ToString().Length > 0)
        {
          if (row["ItemCode"].ToString().Trim().Length > 0)
          {
            rowadd["ItemCode"] = row["ItemCode"];
          }

          if (row["NewSaleCode"].ToString().Trim().Length > 0)
          {
            rowadd["NewSaleCode"] = row["NewSaleCode"];
          }

          if (row["Remark"].ToString().Trim().Length > 0)
          {
            rowadd["Remark"] = row["Remark"];
          }

          if (DBConvert.ParseInt(row["Reason"].ToString()) > 0)
          {
            rowadd["Reason"] = row["Reason"];
          }

          dtInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@Data", SqlDbType.Structured, dtInput);
      DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spCSDImportUpdateSaleCode", sqlinput);
      ultData.DataSource = dtResultDeadline;
      this.CheckItemDuplicate();
    }
    

    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      if (this.Status > 0)
      {
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      }
      else
      {
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
        e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      }
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = this.ultraDDItem;
      e.Layout.Bands[0].Columns["Reason"].ValueList = this.ultddReason;

      e.Layout.Bands[0].Columns["OldSaleCode"].Header.Caption = "Old SaleCode";
      e.Layout.Bands[0].Columns["NewSaleCode"].Header.Caption = "New SaleCode";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      if (isperApproved == false)
      {
        e.Layout.Bands[0].Columns["Approved"].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;     
      e.Layout.Bands[0].Columns["OldSaleCode"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["Approved"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["NewSaleCode"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Approved"].CellAppearance.BackColor = Color.LightGray;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = true;
      if (this.isDuplicateProcess == true)
      {
        WindowUtinity.ShowMessageError("ERR0013");
        return;
      }
      DataTable dt = (DataTable)ultData.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState == DataRowState.Modified || dt.Rows[i].RowState == DataRowState.Added)
        {
          if (dt.Rows[i]["NewSaleCode"].ToString().Length > 0)
          {
            success = this.CheckItem(dt.Rows[i]["NewSaleCode"].ToString());
          }
        }
      }
      // Check Valid
      if (success)
      {
        success = this.CheckVaild(out message);
        if (!success)
        {
          WindowUtinity.ShowMessageError("ERR0001", message);
          return;
        }
        // Save Data
        success = this.SaveData();

        //Update Sale Code
        if (isperApproved == true)
        {
          if (chkConfirm.Checked)
          {
            DBParameter[] inputParam = new DBParameter[1];
            if (this.viewTransactionPid != long.MinValue)
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.viewTransactionPid);
            }
            DBParameter[] ouputParam = new DBParameter[1];

            ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spCSDTransactionCodeUpdateSaleCode", inputParam, ouputParam);
            if (DBConvert.ParseLong(ouputParam[0].Value.ToString()) <= 0)
            {
              this.SaveSuccess = false;
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0037", "Data");
        }
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0002");
      }
    }
    private void chkAutoAll_CheckedChanged(object sender, EventArgs e)
    {
      if (Status != 2)
      {
        int selected = (chkAutoAll.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].Cells["Approved"].Value = selected;
        }
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
    /// HÀM CHECK PROCESS DUPLICATE
    /// </summary>
    private void CheckItemDuplicate()
    {
      isDuplicateProcess = false;
      for (int k = 0; k < ultData.Rows.Count; k++)
      {
        ultData.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string comcode = ultData.Rows[i].Cells["ItemCode"].Value.ToString();
        for (int j = i + 1; j < ultData.Rows.Count; j++)
        {
          string comcodeCompare = ultData.Rows[j].Cells["ItemCode"].Value.ToString();
          if (string.Compare(comcode, comcodeCompare, true) == 0)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
            ultData.Rows[j].CellAppearance.BackColor = Color.Yellow;
            this.isDuplicateProcess = true;
          }
        }
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      string colName = e.Cell.Column.ToString().Trim();
      switch (colName)
      {
        case "ItemCode":
          if (ultraDDItem.SelectedRow != null)
          {
            e.Cell.Row.Cells["Name"].Value = ultraDDItem.SelectedRow.Cells["Name"].Value;
            e.Cell.Row.Cells["OldSaleCode"].Value = ultraDDItem.SelectedRow.Cells["SaleCode"].Value;
            e.Cell.Row.Cells["Approved"].Value = 0;
          }
          else
          {
            e.Cell.Row.Cells["Name"].Value = DBNull.Value;
            e.Cell.Row.Cells["OldSaleCode"].Value = DBNull.Value;
            e.Cell.Row.Cells["Approved"].Value = 0;
          }
          this.CheckItemDuplicate();
          break;
        default:
          break;
      }
    }
    #endregion Event

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      string text = e.Cell.Text.Trim();
      switch (colName)
      {
        case "NewSaleCode":
          if (text.Trim().Length > 16 || text.Trim().Length == 0)
          {
            WindowUtinity.ShowMessageErrorFromText("NewSaleCode length must be less than 16 characters");
            e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    private bool ProcessData()
    {
      DataTable dtOriginal = (DataTable)ultData.DataSource;
      DataTable dtFinal = dtOriginal.Clone();
      for (int i = 0; i < dtOriginal.Rows.Count; i++)
      {
        bool isDupe = false;
        for (int j = 0; j < dtFinal.Rows.Count; j++)
        {
          if (dtOriginal.Rows[i] == null)
          {
            if (dtOriginal.Rows[i][1].ToString() == dtFinal.Rows[j][1].ToString())
            {
              return true;
              isDupe = true;
              break;
            }
          }
        }

        if (!isDupe)
        {
          dtFinal.ImportRow(dtOriginal.Rows[i]);
        }
      }
      return false;
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
    this.CheckItemDuplicate();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "Data");
    }

    private void btBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGet_Click(object sender, EventArgs e)
    {
      string templateName = "UpdateSaleCode";
      string sheetName = "Sheet1";
      string outFileName = "Update SaleCode";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      this.ImportData();
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.CheckItemDuplicate();
    }
  }
}
