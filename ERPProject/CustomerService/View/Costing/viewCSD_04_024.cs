/*
  Author      : Nguyen Thanh Binh
  Date        : 03/12/2018
  Description : Save Transaction  Item Price
  Standard Form: viewCSD_03_018
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_04_024 : MainUserControl
  {
    #region Field
    public long viewTransactionPid = long.MinValue;
    private bool canUpdate = false;
    public bool isCSApproved = false;
    public bool isACCAproved = false;
    private IList listDeletedPid = new ArrayList();
    private IList listDeletedPidUploadFile = new ArrayList();
    private bool isDuplicateProcess = false;
    public int Status = 0;
    private int showImage = 0;
    private long create = long.MinValue;
    private string importFileName = string.Empty;
    private IList listFile = new ArrayList();
    private string sourseFile = string.Empty;
    private string destFile = string.Empty;

    //public bool isFactory;
    //private bool  rslt=true;
    #endregion Field

    #region Init
    public viewCSD_04_024()
    {
      InitializeComponent();
    }

    private void viewCSD_04_024_Load(object sender, EventArgs e)
    {
      //set cs right
      this.isCSApproved = btnIsCSDApprove.Visible ? true : false;
      btnIsCSDApprove.Visible = false;
      //set acc right
      this.isACCAproved = btnIsACCApprove.Visible ? true : false;
      btnIsACCApprove.Visible = false;
      this.LoadDataCustomer();
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
      //LoadDDItemCode();
      this.LoadDDReason();
    }

    private void LoadDataCustomer()
    {
      DataTable dtSource;
      string commandText = @"SELECT Pid, CASE WHEN Pid = 1 THEN 'Factory Sale Price'
				                            ELSE (CustomerCode + ' - ' + [Name]) END Code
                            FROM TblCSDCustomerInfo 
                            WHERE (DeletedFlg = 0 ) OR (Pid IN (1))
                            ORDER BY Kind, CustomerCode";
      dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucbCustomer.DataSource = dtSource;
      ucbCustomer.ValueMember = "Pid";
      ucbCustomer.DisplayMember = "Code";
      ucbCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ucbCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ucbCustomer.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      ucbCustomer.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      ucbCustomer.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
    }
    /// <summary>
    /// Load DropDown Sale Code
    /// </summary>
    /// <param name = "ultra" ></ param >
    private void LoadComBoItemCode(UltraCombo ultra)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CustomerPid", DbType.Int32, ucbCustomer.Value) };
      DataSet dt = DataBaseAccess.SearchStoreProcedure("spCSDUpdateItemPrice_LoadItem", inputParam);

      Utility.LoadUltraCombo(ucboItemCode, dt.Tables[0], "ItemCode", "ItemCode", true);
      ucboItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      ucboItemCode.DisplayLayout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      ucboItemCode.DisplayLayout.Bands[0].Columns["CurrentPrice"].Header.Caption = "Current Price";
      ucboItemCode.DisplayLayout.Bands[0].Columns["BOMPrice"].Header.Caption = "BOM Price";
    }

    private void LoadComBoSale(UltraCombo ultra)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CustomerPid", DbType.Int32, ucbCustomer.Value) };
      DataSet dt = DataBaseAccess.SearchStoreProcedure("spCSDUpdateItemPrice_LoadItem", inputParam);

      Utility.LoadUltraCombo(ucboLoadSaleCode, dt.Tables[0], "SaleCode", "SaleCode", true);
      ucboLoadSaleCode.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      ucboLoadSaleCode.DisplayLayout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      ucboLoadSaleCode.DisplayLayout.Bands[0].Columns["CurrentPrice"].Header.Caption = "Current Price";
    }
    //private void LoadDDItemCode()
    //{
    //  string commandtext = string.Format(@" SELECT ItemCode, Name, SaleCode FROM TblBOMItemBasic  ");
    //  DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandtext);
    //  Utility.LoadUltraDropDown(ultraDDItem, dt, "ItemCode", "ItemCode");
    //  ultraDDItem.DisplayLayout.Bands[0].ColHeadersVisible = false;
    //}

    private void LoadDDReason()
    {
      string commandtext = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 271118");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandtext);
      Utility.LoadUltraCombo(ucbReason, dt, "Code", "Value", false, "Code");
    }

    private void LoadDataComponent(DataSet dsSource)
    {
      if (dsSource.Tables[1].Rows.Count > 0)
      {
        ucbCustomer.Enabled = false;
        ugrdData.DataSource = dsSource.Tables[1];
        int count = ugrdData.Rows.Count;

        if (!this.canUpdate)
        {
          for (int j = 0; j < ugrdData.DisplayLayout.Bands[0].Columns.Count; j++)
          {
            ugrdData.DisplayLayout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
            ugrdData.DisplayLayout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.LightGray;
          }
        }
        if ((isCSApproved || isACCAproved) && this.Status < 2)
        {
          ugrdData.DisplayLayout.Bands[0].Columns["Approved"].CellActivation = Activation.AllowEdit;
          ugrdData.DisplayLayout.Bands[0].Columns["Remark"].CellActivation = Activation.AllowEdit;
        }
        else
        {
          ugrdData.DisplayLayout.Bands[0].Columns["Approved"].CellActivation = Activation.ActivateOnly;
        }
      }
      else
      {
        ugrdData.DataSource = dsSource.Tables[1];
        //btnSave.Enabled = false;
      }
    }

    private void SetStatusControl()
    {
      //Not Confirm
      if ((isCSApproved || isACCAproved) == true)
      {
        if (this.Status == 0 && this.create != SharedObject.UserInfo.UserPid)
        {
          this.canUpdate = false;
          btnSave.Enabled = false;
          chkConfirm.Enabled = false;
          btnRemove.Enabled = false;
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
              ucbCustomer.Enabled = false;
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
          ucbCustomer.Enabled = false;
        }
        else
        {
          this.canUpdate = true;
        }
      }

      if (this.Status > 0)
      {
        btnPathUploadFile.Enabled = false;
        btnGet.Enabled = false;
      }
    }

    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewTransactionPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDTransactionCodeItemPrice_Select", inputParam);
      if (dsSource.Tables[0].Rows.Count > 0)
      {
        DataRow row = dsSource.Tables[0].Rows[0];
        txtTransCode.Text = row["TransactionCode"].ToString();
        ucbCustomer.Value = DBConvert.ParseInt(row["CustomerPid"].ToString());
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
          chkConfirm.Text = "Confirm";
          btnSave.Enabled = true;
          chkAutoAll.Visible = false;
        }
        else if (Status == 1)
        {
          if ((isCSApproved || isACCAproved) == true)
          {
            lbSatus.Text = "Waiting For Approved";
            chkConfirm.Checked = true;
            chkConfirm.Visible = false;
            chkConfirm.Text = "Confirmed";
            chkAutoAll.Visible = true;
            chkAutoAll.Enabled = true;
          }
          else
          {
            chkConfirm.Enabled = false;
            btnSave.Enabled = false;
            lbSatus.Text = "Waiting For Approval";
            chkConfirm.Checked = true;
            chkConfirm.Text = "Confirmed";
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
          chkAutoAll.Visible = true;
          chkAutoAll.Enabled = false;
          chkAutoAll.Checked = false;
          btBrown.Enabled = false;
          btnImport.Enabled = false;
          btnReturn.Enabled = false;
          btnRemove.Enabled = false;
        }
      }
      else
      {
        DataTable dtPR = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FGNRGetNewTransactionCodeItemPrice('CIP') NewTrans");
        if ((dtPR != null) && (dtPR.Rows.Count > 0))
        {
          this.txtTransCode.Text = dtPR.Rows[0]["NewTrans"].ToString();
          this.txtCreateBy.Text = SharedObject.UserInfo.EmpName;
          this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          lbSatus.Text = "New";
          this.create = SharedObject.UserInfo.UserPid;
          chkAutoAll.Visible = false;
        }
      }
      if ((isCSApproved || isACCAproved) == true & this.Status >= 1)
      {
        btnSave.Text = "Approved";
        chkConfirm.Text = "Confirmed";
      }
      SetStatusControl();
      LoadDataComponent(dsSource);
      this.ugrdUploadData.DataSource = dsSource.Tables[2];
      if (this.ugrdUploadData.Rows.Count > 0)
      {
        this.chkUpload.Checked = true;
      }
      else
      {
        this.chkUpload.Checked = false;
      }

      for (int i = 0; i < ugrdUploadData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ugrdUploadData.Rows[i];
        if (File.Exists(rowGrid.Cells["File"].Value.ToString()))
        {
          rowGrid.Cells["Type"].Appearance.Image = Image.FromFile(rowGrid.Cells["File"].Value.ToString());
        }
      }
      txtUploadFile.Text = string.Empty;
      txtLocation.Text = string.Empty;
      if (this.chkUpload.Checked)
      {
        this.ugrdUploadData.Visible = true;
      }
      else
      {
        this.ugrdUploadData.Visible = false;
      }
      lbCount.Text = string.Format("Count: {0}", ugrdData.Rows.FilteredInRowCount);
    }

    private bool CheckVaild(out string errorMessage)
    {
      errorMessage = string.Empty;
      if (txtTransCode.Text.Length == 0)
      {
        errorMessage = "TransactionCode";
        return false;
      }
      //Check Customer
      if (ucbCustomer.Value == null)
      {
        WindowUtinity.ShowMessageError("MSG0011", "Customer");
        ucbCustomer.Focus();
        return false;
      }
      //Duplicate
      if (this.ProcessData())
      {
        WindowUtinity.ShowMessageErrorFromText("Duplicate Data");
        return false;
      }

      int customer = DBConvert.ParseInt(ucbCustomer.Value.ToString());
      // Check ItemCode
      for (int i = 0; i < ugrdData.Rows.Count; i++)
      {
        UltraGridRow row = ugrdData.Rows[i];
        row.Selected = false;
        string ItemCode = row.Cells["ItemCode"].Value.ToString();
        if (this.Status == 0 || DBConvert.ParseInt(row.Cells["Approved"].Value.ToString()) == 1)
        {
          if (row.Cells["ItemCode"].Value.ToString().Trim().Length == 0)
          {
            row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
            errorMessage = "Item Code";
            row.Selected = true;
            ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
            return false;
          }
          else
          {
            if (customer != 1)
            {
              string commandText = string.Format(@"SELECT ItemCode
                                  FROM TblBOMItemBasic A
                                  WHERE ItemCode = '{0}'
	                                  AND (CustomerPid = {1})", ItemCode, customer);
              DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtItem.Rows.Count == 0 || dtItem == null)
              {
                row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
                errorMessage = "Item Code not exist or customer of item not match";
                row.Selected = true;
                ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
                return false;
              }
            }
          }

          if (row.Cells["Reason"].Value.ToString().Trim().Length == 0)
          {
            row.Cells["Reason"].Appearance.BackColor = Color.Yellow;
            errorMessage = "Reason";
            row.Selected = true;
            ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
            return false;
          }
          else
          {
            string commandText = string.Empty;
            commandText += " SELECT Code";
            commandText += " FROM TblBOMCodeMaster ";
            commandText += " WHERE [Group] = 271118 AND Code   = " + DBConvert.ParseInt(row.Cells["Reason"].Value.ToString());
            DataTable dtReason = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtReason.Rows.Count == 0 || dtReason == null)
            {
              row.Cells["Reason"].Appearance.BackColor = Color.Yellow;
              errorMessage = "Reason";
              row.Selected = true;
              ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
              return false;
            }
          }

          // Check price
          //1 Check price invalid
          double price = DBConvert.ParseDouble(row.Cells["NewPrice"].Value.ToString());
          if (price == double.MinValue)
          {
            row.Cells["NewPrice"].Selected = true;
            Shared.Utility.WindowUtinity.ShowMessageError("WRN0013", "Price");
            return false;
          }
          else if (price <= 0)
          {
            row.Cells["NewPrice"].Selected = true;
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Price");
            return false;
          }
          //2 Check new price with current price
          string cmt = string.Format(@"SELECT ISNULL(dbo.FCSDGetCurrentPriceOfItem('{0}','{1}'), 0) CurrentPrice", ItemCode, customer);
          DataTable dtCurrentPrice = DataBaseAccess.SearchCommandTextDataTable(cmt);
          if ((dtCurrentPrice != null) && (dtCurrentPrice.Rows.Count > 0))
          {
            double currentPrice = DBConvert.ParseDouble(dtCurrentPrice.Rows[0]["CurrentPrice"].ToString());
            //2.1 check new price with current price
            if (price == currentPrice)
            {
              WindowUtinity.ShowMessageErrorFromText("New price equal current price in the system. Please check!");
              row.Cells["Error"].Value = "New price equals current price";
              row.Selected = true;
              ugrdData.ActiveRowScrollRegion.FirstRow = ugrdData.Rows[i];
              //errorMessage = "New Price";
              return false;
            }
          }
        }
      }
      return true;
    }

    private bool SaveMain()
    {
      DBParameter[] inputParam = new DBParameter[9];
      if (this.viewTransactionPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.viewTransactionPid);
      }

      if (txtTransCode.Text.Length > 0)
      {
        inputParam[1] = new DBParameter("@TransactionCode", DbType.String, "CIP");
      }


      if (chkConfirm.Checked && (isCSApproved || isACCAproved) == true)
      {
        if (this.Status >= 1)
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, 2);
          inputParam[6] = new DBParameter("@ApprovedBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        else
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);
          inputParam[6] = new DBParameter("@ApprovedBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        //inputParam[7] = new DBParameter("@ApprovedDate", DbType.DateTime, DateTime.Now);
      }
      else if (chkConfirm.Checked && (isCSApproved || isACCAproved) == false)
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

      if (DBConvert.ParseInt(ucbCustomer.Value.ToString()) != int.MinValue)
      {
        inputParam[7] = new DBParameter("@CustomerPid", DbType.Int32, DBConvert.ParseInt(ucbCustomer.Value.ToString()));
      }
      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spCSDTransactionCodeItemPrice_Edit", inputParam, ouputParam);

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

        DataBaseAccess.ExecuteStoreProcedure("spCSDTransactionItemPriceDetail_Delete", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      DataTable dt = (DataTable)ugrdData.DataSource;
      foreach (DataRow dr in dt.Rows)
      {
        if (dr.RowState != DataRowState.Deleted)
        {
          DBParameter[] input = new DBParameter[7];
          if (DBConvert.ParseLong(dr["Pid"].ToString()) != long.MinValue)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dr["Pid"].ToString()));
          }
          input[1] = new DBParameter("@TransactionPid", DbType.Int64, viewTransactionPid);
          input[2] = new DBParameter("@ItemCode", DbType.String, dr["ItemCode"].ToString());
          input[3] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(dr["NewPrice"].ToString()));
          input[4] = new DBParameter("@Remark", DbType.String, dr["Remark"].ToString());
          if (DBConvert.ParseInt(dr["Approved"].ToString()) != int.MinValue)
          {
            input[5] = new DBParameter("@Approved", DbType.Int32, DBConvert.ParseInt(dr["Approved"].ToString()));
          }
          input[6] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(dr["Reason"].ToString()));

          DBParameter[] ouputParam = new DBParameter[1];
          ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spCSDTransactionCodeDetailItemPrice_Edit", input, ouputParam);
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
        success = this.SaveUploadFile();
        if (success)
        {
          success = this.SaveDetail();
          this.SaveSuccess = true;
        }
        else
        {
          success = false;
        }
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
      DataTable dt = (DataTable)ugrdData.DataSource;
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

        //Update transaction
        if ((isCSApproved || isACCAproved) == true)
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
            DataBaseAccess.ExecuteStoreProcedure("spCSDTransactionCodeUpdateItemPrice", inputParam, ouputParam);
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
      dt.Columns.Add("Price", typeof(System.Double));
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

      DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), string.Format("SELECT * FROM [Sheet1 (1)$B3:E{0}]", itemCount + 3)).Tables[0];
      if (dtSource == null)
      {
        return;
      }

      DataTable dtInput = this.dtDeadlineResult();
      SqlDBParameter[] sqlinput = new SqlDBParameter[3];
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

          if (DBConvert.ParseDouble(row["Price"].ToString().Trim()) > 0)
          {
            rowadd["Price"] = DBConvert.ParseDouble(row["Price"]);
          }

          if (row["Remark"].ToString().Trim().Length > 0)
          {
            rowadd["Remark"] = row["Remark"];
          }

          if (DBConvert.ParseInt(row["ReasonCode"].ToString()) > 0)
          {
            rowadd["Reason"] = row["ReasonCode"];
          }

          dtInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@Data", SqlDbType.Structured, dtInput);
      sqlinput[1] = new SqlDBParameter("@CustomerPid", SqlDbType.Int, DBConvert.ParseInt(ucbCustomer.Value.ToString()));
      DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spCSDImportUpdateItemPrice", sqlinput);
      ugrdData.DataSource = dtResultDeadline;
      ucbCustomer.Enabled = false;
      if (ugrdData.Rows.Count > 0)
      {
        btnSave.Enabled = true;
      }
    }

    /// <summary>
    /// Save Upload File
    /// </summary>
    /// <returns></returns>
    private bool SaveUploadFile()
    {
      //Copy File 
      //System.IO.File.Copy(sourseFile, destFile, true);

      // Delete Row In grid
      foreach (long pidDelete in this.listDeletedPidUploadFile)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spCSDUploadFileItemPriceDetail_Delete", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }

      string storeName = string.Empty;
      DataTable dtMain = (DataTable)this.ugrdUploadData.DataSource;
      foreach (DataRow row in dtMain.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (row.RowState == DataRowState.Added)
          {
            //Copy File
            System.IO.File.Copy(row["LocationFileLocal"].ToString(), row["LocationFile"].ToString(), true);
          }
          storeName = "spCSDUpdatePriceUploadFile_Edit";
          DBParameter[] inputParam = new DBParameter[6];

          //Pid
          if (DBConvert.ParseLong(row["Pid"].ToString()) >= 0)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
          //Transaction PId

          inputParam[1] = new DBParameter("@TransactionPid", DbType.Int64, this.viewTransactionPid);

          inputParam[2] = new DBParameter("@FileName", DbType.String, 1024, row["FileName"].ToString());

          inputParam[3] = new DBParameter("@LocationFile", DbType.String, 1024, row["LocationFile"].ToString());

          inputParam[4] = new DBParameter("@Remark", DbType.String, 4000, row["Remark"].ToString());

          inputParam[5] = new DBParameter("@File", DbType.String, 1024, row["File"].ToString());

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    #endregion Function

    #region Event
    private void ugrdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      ugrdData.SyncWithCurrencyManager = false;
      ugrdData.StyleSetName = "Excel2013";
      //e.Layout.UseFixedHeaders = true;
      //Utility.SetPropertiesUltraGrid(ugrdData);
      e.Layout.Bands[0].Columns.Insert(4, "Picture").DataType = typeof(Image);
      if (this.Status > 0)
      {
        if (this.Status == 1)
        {
          e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
          e.Layout.Bands[0].Columns["Error"].Hidden = true;
          e.Layout.Bands[0].Columns["NewPrice"].CellActivation = Activation.ActivateOnly;
          e.Layout.Bands[0].Columns["Reason"].CellActivation = Activation.ActivateOnly;
          e.Layout.Bands[0].Columns["NewPrice"].CellAppearance.BackColor = Color.LightGray;
          e.Layout.Bands[0].Columns["Reason"].CellAppearance.BackColor = Color.LightGray;
        }
        else
        {
          e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
          e.Layout.Bands[0].Columns["Error"].Hidden = true;
          e.Layout.Bands[0].Columns["NewPrice"].CellActivation = Activation.ActivateOnly;
          e.Layout.Bands[0].Columns["Reason"].CellActivation = Activation.ActivateOnly;
          e.Layout.Bands[0].Columns["NewPrice"].CellAppearance.BackColor = Color.LightGray;
          e.Layout.Bands[0].Columns["Reason"].CellAppearance.BackColor = Color.LightGray;
          e.Layout.Bands[0].Columns["Approved"].CellAppearance.BackColor = Color.LightGray;
          e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.LightGray;
        }
      }
      else
      {
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
        e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
        //e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
        //e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        //e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      }
      //e.Layout.Bands[0].Columns["ItemCode"].ValueList = this.ultraDDItem;
      e.Layout.Bands[0].Columns["Reason"].ValueList = this.ucbReason;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = this.ucboItemCode;
      //e.Layout.Bands[0].Columns["SaleCode"].ValueList = this.ucboLoadSaleCode;
      e.Layout.Bands[0].Columns["Reason"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["Reason"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ItemCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ItemCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      //e.Layout.Bands[0].Columns["SaleCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      //e.Layout.Bands[0].Columns["Salecode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[0].Columns[i].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      }
      ugrdData.DisplayLayout.Override.MinRowHeight = 70;
      for (int i = 0; i < ugrdData.Rows.Count; i++)
      {
        ugrdData.DisplayLayout.Rows[i].Height = 70;
      }
      e.Layout.Bands[0].Columns["Picture"].CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
      e.Layout.Bands[0].Columns["Reason"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      if (this.Status < 1)
      {
        e.Layout.Bands[0].Columns["Approved"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Approved"].Hidden = true;
      }

      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SaleCode"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Error"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CurrentPrice"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["BOMPrice"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["Picture"].Hidden = true;

      e.Layout.Bands[0].Columns["Approved"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      //e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["SaleCode"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Name"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Error"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["CurrentPrice"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["BOMPrice"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Reason"].MinWidth = 220;
      e.Layout.Bands[0].Columns["Reason"].MaxWidth = 220;

      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 140;
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 140;

      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["BOMPrice"].Header.Caption = "BOM Price";

      e.Layout.Bands[0].Columns["NewPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["NewPrice"].MinWidth = 90;
      e.Layout.Bands[0].Columns["NewPrice"].MaxWidth = 90;

      e.Layout.Bands[0].Columns["CurrentPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["CurrentPrice"].MinWidth = 90;
      e.Layout.Bands[0].Columns["CurrentPrice"].MaxWidth = 90;

      e.Layout.Bands[0].Columns["BOMPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["BOMPrice"].MinWidth = 90;
      e.Layout.Bands[0].Columns["BOMPrice"].MaxWidth = 90;

      e.Layout.Bands[0].Columns["Approved"].MinWidth = 75;
      e.Layout.Bands[0].Columns["Approved"].MaxWidth = 75;

      e.Layout.Bands[0].Columns["Picture"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Picture"].MinWidth = 90;

      e.Layout.Bands[0].Columns["CurrentPrice"].Header.Caption = "Old Price";
      e.Layout.Bands[0].Columns["NewPrice"].Header.Caption = "New Price";
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
        WindowUtinity.ShowMessageErrorFromText("Duplicate item code. Please check yellow rows!");
        return;
      }
      // Check Valid
      if (success)
      {
        success = this.CheckVaild(out message);
        if (!success)
        {
          if (message.Length > 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", message);
          }
          return;
        }
        //Check error
        for (int i = 0; i < ugrdData.Rows.Count; i++)
        {
          if (ugrdData.Rows[i].Cells["Error"].Value.ToString().Length > 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Please remove invalid rows");
            return;
          }
        }
        // Save Data
        success = this.SaveData();

        // Update item price list
        if ((isCSApproved || isACCAproved) & this.Status >= 1)
        {
          int temp = 0;
          if (chkConfirm.Checked)
          {
            for (int i = 0; i < ugrdData.Rows.Count; i++)
            {
              if (DBConvert.ParseInt(ugrdData.Rows[i].Cells["Approved"].Value.ToString()) == 1)
              {
                temp += 1;
              }
            }

            DBParameter[] inputParam = new DBParameter[1];
            if (this.viewTransactionPid != long.MinValue)
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.viewTransactionPid);
            }
            DBParameter[] ouputParam = new DBParameter[1];

            ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            if (temp == 0)
            {
              DialogResult dlgr = MessageBox.Show("There are not any items which are approved. Do you want to approve?", "ERP SYSTEM", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
              if (dlgr == DialogResult.Yes)
              {
                DataBaseAccess.ExecuteStoreProcedure("spCSDTransactionCodeUpdateItemPrice", inputParam, ouputParam);
              }
              else
              {
                return;
              }
            }
            else
            {
              DataBaseAccess.ExecuteStoreProcedure("spCSDTransactionCodeUpdateItemPrice", inputParam, ouputParam);
            }
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
        for (int i = 0; i < ugrdData.Rows.Count; i++)
        {
          ugrdData.Rows[i].Cells["Approved"].Value = selected;
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
      for (int k = 0; k < ugrdData.Rows.Count; k++)
      {
        ugrdData.Rows[k].CellAppearance.BackColor = Color.Empty;
        ugrdData.Rows[k].Cells["Error"].Value = "";
      }
      for (int i = 0; i < ugrdData.Rows.Count; i++)
      {
        string comcode = ugrdData.Rows[i].Cells["ItemCode"].Value.ToString();
        for (int j = i + 1; j < ugrdData.Rows.Count; j++)
        {
          string comcodeCompare = ugrdData.Rows[j].Cells["ItemCode"].Value.ToString();
          if (string.Compare(comcode, comcodeCompare, true) == 0)
          {
            ugrdData.Rows[i].CellAppearance.BackColor = Color.Yellow;
            ugrdData.Rows[j].CellAppearance.BackColor = Color.Yellow;
            ugrdData.Rows[i].Cells["Error"].Value = "Double item code";
            ugrdData.Rows[j].Cells["Error"].Value = "Double item code";
            this.isDuplicateProcess = true;
          }
        }
      }
    }

    private void ugrdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      string colName = e.Cell.Column.ToString().Trim();
      string value = e.Cell.Value.ToString();
      switch (colName)
      {
        case "ItemCode":
          e.Cell.Row.Cells["Revision"].Value = DBConvert.ParseInt(ucboItemCode.SelectedRow.Cells["Revision"].Value);
          e.Cell.Row.Cells["SaleCode"].Value = ucboItemCode.SelectedRow.Cells["SaleCode"].Value;
          e.Cell.Row.Cells["Name"].Value = ucboItemCode.SelectedRow.Cells["Name"].Value;
          e.Cell.Row.Cells["CurrentPrice"].Value = ucboItemCode.SelectedRow.Cells["CurrentPrice"].Value;
          e.Cell.Row.Cells["BOMPrice"].Value = ucboItemCode.SelectedRow.Cells["BOMPrice"].Value;
          this.CheckItemDuplicate();
          break;
        //case "SaleCode":
        //  e.Cell.Row.Cells["ItemCode"].Value = ucboItemCode.SelectedRow.Cells["ItemCode"].Value;
        //  e.Cell.Row.Cells["Name"].Value = ucboItemCode.SelectedRow.Cells["Name"].Value;
        //  e.Cell.Row.Cells["CurrentPrice"].Value = ucboItemCode.SelectedRow.Cells["CurrentPrice"].Value;
        //  this.CheckItemDuplicate();
        //  break;
        case "NewPrice":
          if (DBConvert.ParseDouble(value) > 0 && DBConvert.ParseDouble(value) != double.MinValue)
          {
            int customer = DBConvert.ParseInt(ucbCustomer.Value.ToString());
            string itemCode = e.Cell.Row.Cells["ItemCode"].Value.ToString();
            double newPrice = DBConvert.ParseDouble(value);
            string cmt = string.Format(@"SELECT ISNULL(dbo.FCSDGetCurrentPriceOfItem('{0}','{1}'), 0) CurrentPrice", itemCode, customer);
            DataTable dtCurrentPrice = DataBaseAccess.SearchCommandTextDataTable(cmt);
            if ((dtCurrentPrice != null) && (dtCurrentPrice.Rows.Count > 0))
            {
              double currentPrice = DBConvert.ParseDouble(dtCurrentPrice.Rows[0]["CurrentPrice"].ToString());
              if (newPrice == currentPrice)
              {
                WindowUtinity.ShowMessageErrorFromText("New price equal current price in the system. Please check!");
                e.Cell.Row.Cells["Error"].Value = "New price equals current price";
              }
              else
              {
                e.Cell.Row.Cells["Error"].Value = "";
              }
            }
          }
          else
          {
            WindowUtinity.ShowMessageErrorFromText("New price must be larger than 0");
            e.Cell.Row.Cells["Error"].Value = "New price = 0";
          }
          break;
        default:
          break;
      }
    }


    private void ugrdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      string text = e.Cell.Text.Trim();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "Price":
          {
            if (text.Trim().Length == 0 || DBConvert.ParseDouble(value) != double.MinValue || DBConvert.ParseDouble(value) < 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Price Invalid");
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
              e.Cancel = true;
            }
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
      DataTable dtOriginal = (DataTable)ugrdData.DataSource;
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

    private void ugrdData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
      Utility.ExportToExcelWithDefaultPath(ugrdData, "Data");
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
      string templateName = "ItemPriceList";
      string sheetName = "Sheet1";
      string outFileName = "Update Item Price";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\CustomerService\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spCSDListReasonOfUpdateItemPrice");

      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dtRow = dt.Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("J5:K5").Copy();
          oXlsReport.RowInsert(6 + i);
          oXlsReport.Cell("J5:K5", 0, i).Paste();
        }

        oXlsReport.Cell("**1", 0, i).Value = dtRow["Code"];
        oXlsReport.Cell("**2", 0, i).Value = dtRow["Value"];
      }
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      if (ucbCustomer.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Please select customer!");
        ucbCustomer.Focus();
        return;
      }
      this.ImportData();
      lbCount.Text = string.Format("Count: {0}", ugrdData.Rows.FilteredInRowCount);
    }

    private void ugrdData_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.CheckItemDuplicate();
      if (ugrdData.Rows.Count > 0)
      {
        ucbCustomer.Enabled = false;
        btnSave.Enabled = true;
      }
      else
      {
        ucbCustomer.Enabled = true;
        btnSave.Enabled = false;
      }
    }

    private void txtLocation_TextChanged(object sender, EventArgs e)
    {
      if (txtLocation.Text.Length > 0)
      {
        btnImport.Enabled = true;
      }
    }

    private void chkUpload_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkUpload.Checked)
      {
        this.ugrdUploadData.Visible = true;
      }
      else
      {
        this.ugrdUploadData.Visible = false;
      }
    }

    private void btnUploadFile_Click(object sender, EventArgs e)
    {
      if (listFile.Count > 0)
      {
        for (int j = 0; j < listFile.Count; j++)
        {
          string file = listFile[j].ToString();
          FileInfo f = new FileInfo(file);
          long fLength = f.Length;
          //if (fLength < 5120000)
          //{
          string extension = System.IO.Path.GetExtension(file).ToLower();
          string typeFile = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE Value = '" + extension + "' AND [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_TYPEFILEUPLOAD;
          DataTable dtTypeFile = DataBaseAccess.SearchCommandTextDataTable(typeFile);
          if (dtTypeFile != null && dtTypeFile.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dtTypeFile.Rows[0][0].ToString()) > 0)
            {
              string fileName1 = System.IO.Path.GetFileName(file).ToString();
              string fileName = System.IO.Path.GetFileNameWithoutExtension(file).ToString()
                          + DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd"))
                          + DBConvert.ParseString(DateTime.Now.Ticks)
                          + System.IO.Path.GetExtension(file);

              string sourcePath = System.IO.Path.GetDirectoryName(file);
              string commandText = string.Empty;
              commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 11);
              DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              string targetPath = string.Empty;
              if (dt != null && dt.Rows.Count > 0)
              {
                targetPath = dt.Rows[0][0].ToString();
              }

              sourseFile = System.IO.Path.Combine(sourcePath, fileName1);
              destFile = System.IO.Path.Combine(targetPath, fileName);
              if (!System.IO.Directory.Exists(targetPath))
              {
                System.IO.Directory.CreateDirectory(targetPath);
              }
              DataTable dtSource = (DataTable)ugrdUploadData.DataSource;
              int i = dtSource.Rows.Count;
              foreach (DataRow row1 in dtSource.Rows)
              {
                if (row1.RowState == DataRowState.Deleted)
                {
                  i = i - 1;
                }
              }
              DataRow row = dtSource.NewRow();
              row["FileName"] = fileName1;
              row["LocationFile"] = destFile;
              row["LocationFileLocal"] = sourseFile;
              dtSource.Rows.Add(row);
              if (String.Compare(extension, ".docx") == 0 || String.Compare(extension, ".doc") == 0)
              {
                this.ugrdUploadData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "word.bmp");
                row["File"] = targetPath + "word.bmp";
              }
              else if (string.Compare(extension, ".xls") == 0 || string.Compare(extension, ".xlsx") == 0)
              {
                this.ugrdUploadData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "xls.bmp");
                row["File"] = targetPath + "xls.bmp";
              }
              else if (string.Compare(extension, ".pdf") == 0)
              {
                this.ugrdUploadData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "pdf.bmp");
                row["File"] = targetPath + "pdf.bmp";
              }
              else if (string.Compare(extension, ".txt") == 0)
              {
                this.ugrdUploadData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "notepad.bmp");
                row["File"] = targetPath + "notepad.bmp";
              }
              else if (string.Compare(extension, ".gif") == 0
                    || string.Compare(extension, ".jpg") == 0
                    || string.Compare(extension, ".bmp") == 0
                    || string.Compare(extension, ".png") == 0)
              {
                this.ugrdUploadData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "image.bmp");
                row["File"] = targetPath + "image.bmp";
              }
              else if (string.Compare(extension, ".mov") == 0
                      || string.Compare(extension, ".mp4") == 0
                      || string.Compare(extension, ".mpeg") == 0
                      || string.Compare(extension, ".wmv") == 0)
              {
                this.ugrdUploadData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "movie.bmp");
                row["File"] = targetPath + "movie.bmp";
              }
              else if (string.Compare(extension, ".msg") == 0)
              {
                this.ugrdUploadData.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "mail.bmp");
                row["File"] = targetPath + "mail.bmp";
              }
              //this.btnUpload.Enabled = false;
              this.chkUpload.Checked = true;
            }
            else
            {
              WindowUtinity.ShowMessageError("ERR0001", "Type File Not UPload");
            }
          }
        }

      }
      txtUploadFile.Text = string.Empty;
    }

    private void btnPathUploadFile_Click(object sender, EventArgs e)
    {
      listFile.Clear();
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Multiselect = true;
      //txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        foreach (string file in dialog.FileNames)
        {
          if (txtUploadFile.Text == "")
          {
            txtUploadFile.Text = file;
            listFile.Add(file);
          }
          else
          {
            txtUploadFile.Text += " ; " + file;
            listFile.Add(file);
          }
        }
      }
      btnUploadFile.Enabled = (txtUploadFile.Text.Trim().Length > 0);
    }


    private void ugrdUploadData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      Utility.SetPropertiesUltraGrid(ugrdUploadData);
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;

      if (this.Status > 0)
      {
        e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      }
      else
      {
        e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      }


      e.Layout.Bands[0].Columns["FileName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Type"].CellActivation = Activation.ActivateOnly;


      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["TransactionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFile"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFileLocal"].Hidden = true;
      e.Layout.Bands[0].Columns["File"].Hidden = true;

      e.Layout.Bands[0].Columns["FileName"].Header.Caption = "File Name";

      e.Layout.Bands[0].Columns["Type"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Type"].MinWidth = 150;
      e.Layout.Bands[0].Columns["FileName"].MaxWidth = 400;
      e.Layout.Bands[0].Columns["FileName"].MinWidth = 400;
      //e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
    }



    private void ugrdUploadData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ugrdUploadData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ugrdUploadData.Selected.Rows[0];
      Process prc = new Process();

      if (DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) == int.MinValue)
      {
        prc.StartInfo = new ProcessStartInfo(row.Cells["LocationFileLocal"].Value.ToString());
      }
      else
      {
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string folder = string.Format(@"{0}\Report", startupPath);
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }
        string locationFile = row.Cells["LocationFile"].Value.ToString();
        if (File.Exists(locationFile))
        {
          string newLocationFile = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(row.Cells["LocationFile"].Value.ToString()));
          if (File.Exists(newLocationFile))
          {
            try
            {
              File.Delete(newLocationFile);
            }
            catch
            {
              WindowUtinity.ShowMessageWarningFromText("File Is Opening!");
              return;
            }
          }
          File.Copy(locationFile, newLocationFile);
          prc.StartInfo = new ProcessStartInfo(newLocationFile);
        }
      }
      try
      {
        prc.Start();
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
      }
    }

    private void ugrdUploadData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pidUploadFile = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pidUploadFile != long.MinValue)
        {
          this.listDeletedPidUploadFile.Add(pidUploadFile);
        }
      }
    }

    private void txtUploadFile_TextChanged(object sender, EventArgs e)
    {
      if (txtUploadFile.Text.Length > 0)
      {
        btnUploadFile.Enabled = true;
      }
    }

    /// <summary>
    /// Remove invalid rows in the grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRemove_Click(object sender, EventArgs e)
    {
      DataTable dataSource = (DataTable)ugrdData.DataSource;
      if (dataSource != null && dataSource.Rows.Count > 0)
      {
        for (int i = 0; i < dataSource.Rows.Count; i++)
        {
          DataRow row = dataSource.Rows[i];
          string isValid = row["Error"].ToString();
          if (isValid.Trim().Length > 0)
          {
            dataSource.Rows.RemoveAt(i);
            i--;
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0039");
      btnSave.Enabled = (ugrdData.Rows.Count > 0);
    }


    private void ugrdData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (colName == "ItemCode")
      {
        if (ucbCustomer.SelectedRow == null)
        {
          WindowUtinity.ShowMessageErrorFromText("Please select Customer");
          e.Cancel = true;
        }
      }
    }

    private void ucbCustomer_ValueChanged(object sender, EventArgs e)
    {
      this.LoadComBoItemCode(ucboItemCode);
      //this.LoadComBoSale(ucboLoadSaleCode);
    }

    private void btnReturn_Click(object sender, EventArgs e)
    {
      bool flag = true;
      if (this.Status != 2)
      {
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DBParameter[] returnParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewTransactionPid) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDItemChangePrice_Return", returnParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          flag = false;
        }
        if (flag == true)
        {
          MessageBox.Show("Return success!", "Success");
        }
      }
      else
      {
        MessageBox.Show("Can't return because this transaction has been finished!", "Error");
      }
    }


    private void ugrdData_InitializeRow(object sender, InitializeRowEventArgs e)
    {
      if (this.showImage == 1)
      {
        // Check if this is data row -  if you have summaries, groups...
        if (e.Row.IsDataRow)
        {
          // Create an image from the path string in the "Path" cell
          try
          {
            Image image = Bitmap.FromFile(FunctionUtility.BOMGetItemImage(e.Row.Cells["ItemCode"].Value.ToString(), DBConvert.ParseInt(e.Row.Cells["Revision"].Value.ToString())));
            e.Row.Cells["Picture"].Value = image;
          }
          catch { }
          // Put the image in the "Image" cell
        }
      }
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowImage.Checked)
      {
        if (this.showImage == 0)
        {
          this.showImage = 1;
          if (this.viewTransactionPid > 0)
          {
            this.LoadData();
          }
        }
        ugrdData.DisplayLayout.Bands[0].Columns["Picture"].Hidden = false;
      }
      else
      {
        ugrdData.DisplayLayout.Bands[0].Columns["Picture"].Hidden = true;
      }

    }
    #endregion Event
  }
}
