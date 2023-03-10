/*
  Author      : Do Tam
  Date        : 15/11/2013
  Description : Change Note Confirm ShipDate Info
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

namespace DaiCo.Planning
{
  public partial class viewPLN_02_020 : MainUserControl
  {
    #region Field
    public long transactionPid = long.MinValue;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool _1st = true;
    private bool _2nd = true;
    private int Status = 0;
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init
    public viewPLN_02_020()
    {
      InitializeComponent();
    }

    private void LoadItemCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ItemCode ";
      commandText += " FROM TblBOMItemBasic ";
      commandText += " ORDER BY ItemCode ";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucItemCode.DataSource = dtItem;
      ucItemCode.ColumnWidths = "200";
      ucItemCode.DataBind();
      ucItemCode.ValueMember = "ItemCode";
    }

    private void LoadCustomer()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, CustomerCode + '-' + Name Name ";
      commandText += " FROM TblCSDCustomerInfo CSD ";
      commandText += " ORDER BY CustomerCode ";
      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCustomer.DataSource = dtItem;
      ucCustomer.ColumnWidths = "0;200";
      ucCustomer.DataBind();
      ucCustomer.ValueMember = "Pid";
      ucCustomer.DisplayMember = "Name";
      ucCustomer.AutoSearchBy = "Name";
    }

    private void LoadData()
    {
      pnlRight.Visible = true;
      _1st = btn1st.Visible;
      _2nd = btn2sd.Visible;
      if (_1st)
      {
        lblStatus.Text = "PLN Account";
        btnReturn.Visible = false;
      }
      else
      {
        lblStatus.Text = "PRODUCTION Account";
      }
      pnlRight.Visible = false;

      if (this.transactionPid == long.MinValue)
      {
        txtTransaction.Text = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNGetNewOutputCodeForChangeNoteWoQuantity('WCQ')", null).Rows[0][0].ToString();
        txtCreateBy.Text = SharedObject.UserInfo.EmpName.ToString();
        txtCreateDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
        Status = 0;
      }
      else
      {
        DBParameter[] param = new DBParameter[1];
        param[0] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
        string storeName = "spPLNChangeWoQuantityList_Select";
        DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
        Status = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["ST"].ToString());
        txtTransaction.Text = dsSource.Tables[0].Rows[0]["TransactionCode"].ToString();
        txtCreateBy.Text = dsSource.Tables[0].Rows[0]["CreateBy"].ToString();
        txtRemark.Text = dsSource.Tables[0].Rows[0]["Remark"].ToString();
        txtReason.Text = dsSource.Tables[0].Rows[0]["Reason"].ToString();
        txtCreateDate.Text = dsSource.Tables[0].Rows[0]["CreateDate"].ToString();
        ultData.DataSource = dsSource.Tables[1];
      }
      if (Status > 0)
      {
        grpSearch.Visible = false;
      }
      else
      {
        grpSearch.Visible = true;
      }

      if (!_1st)
      {
        chkHide.Checked = true;
        chkHide.Visible = false;
      }
      else
      {
        chkHide.Visible = true;
      }

      if (_1st)
      {
        if (Status == 0)
        {
          chkConfirm.Checked = false;
          if (this.transactionPid != long.MinValue)
          {
            chkConfirm.Enabled = true;
          }
          btnSave.Enabled = true;
        }
        else if (Status == 1)
        {
          chkConfirm.Checked = true;
          chkHide.Checked = true;
          chkHide.Visible = false;
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          this.grpSearch.Visible = false;
        }
        else
        {
          chkHide.Checked = true;
          chkHide.Visible = false;
          chkConfirm.Checked = true;
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          this.grpSearch.Visible = false;
        }
      }
      else
      {
        if (Status == 2)
        {
          chkConfirm.Checked = true;
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          btnReturn.Enabled = false;

        }
        else if (Status == 1)
        {
          chkConfirm.Checked = false;
          chkConfirm.Enabled = true;
          btnSave.Enabled = true;
          btnReturn.Enabled = true;
        }
        else
        {
          chkConfirm.Checked = false;
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
          btnReturn.Enabled = false;

        }
        this.grpSearch.Visible = false;
      }

      if (Status > 0)
      {
        btnAdd.Visible = false;
        btnSearch.Visible = false;
        btnClear.Visible = false;
      }
      ultInformation.DataSource = null;
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_020_Load(object sender, EventArgs e)
    {
      LoadItemCode();
      LoadCustomer();
      this.LoadData();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    /// <summary>
    /// Load Column Name
    /// </summary>
    //private void LoadColumnName()
    //{
    //    DataTable dtNew = new DataTable();
    //    DataTable dtColumn = ((DataSet)ultInformation.DataSource).Tables[0];
    //    dtNew.Columns.Add("All", typeof(Int32));
    //    dtNew.Columns["All"].DefaultValue = 0;
    //    foreach (DataColumn column in dtColumn.Columns)
    //    {
    //      dtNew.Columns.Add(column.ColumnName, typeof(Int32));
    //      dtNew.Columns[column.ColumnName].DefaultValue = 0;

    //      if (string.Compare(column.ColumnName, "Revision", true) == 0)
    //      {
    //        dtNew.Columns[column.ColumnName].DefaultValue = 1;
    //      }

    //      if (string.Compare(column.ColumnName, "SONo", true) == 0)
    //      {
    //        dtNew.Columns[column.ColumnName].DefaultValue = 1;
    //      }

    //      if (string.Compare(column.ColumnName, "PONo", true) == 0)
    //      {
    //        dtNew.Columns[column.ColumnName].DefaultValue = 1;
    //      }

    //      if (string.Compare(column.ColumnName, "[Cust.]", true) == 0)
    //      {
    //        dtNew.Columns[column.ColumnName].DefaultValue = 1;
    //      }

    //      if (string.Compare(column.ColumnName, "ContainerNo", true) == 0)
    //      {
    //        dtNew.Columns[column.ColumnName].DefaultValue = 1;
    //      }

    //      if (string.Compare(column.ColumnName, "ConfirmShipDate", true) == 0)
    //      {
    //        dtNew.Columns[column.ColumnName].DefaultValue = 1;
    //      }

    //      if (string.Compare(column.ColumnName, "OldConfirmShipDate", true) == 0)
    //      {
    //        dtNew.Columns[column.ColumnName].DefaultValue = 1;
    //      }

    //      if (string.Compare(column.ColumnName, "ShipDate", true) == 0)
    //      {
    //        dtNew.Columns[column.ColumnName].DefaultValue = 1;
    //      }
    //    }
    //    DataRow row = dtNew.NewRow();
    //    dtNew.Rows.Add(row);
    //    ultShowColumn.DataSource = dtNew;
    //    ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
    //}

    /// <summary>
    /// Set Status Column When Search
    /// </summary>
    //private void SetStatusColumn()
    //{
    //    for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
    //    {
    //        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
    //        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
    //        {
    //            ultInformation.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
    //        }
    //    }
    //}

    /// <summary>
    /// Save Transaction Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster()
    {
      DBParameter[] input = new DBParameter[6];
      if (this.transactionPid != long.MinValue)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, this.transactionPid);
      }
      input[1] = new DBParameter("@TransactionCode", DbType.AnsiString, 16, txtTransaction.Text);
      int confirm = 0;
      if (chkConfirm.Checked)
      {
        confirm = 1;
      }
      else
      {
        confirm = 0;
      }
      input[2] = new DBParameter("@Status", DbType.Int32, confirm);
      input[3] = new DBParameter("@CurrencyPid", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (txtRemark.Text.Trim().Length > 0)
      {
        input[4] = new DBParameter("@Remark", DbType.String, 500, txtRemark.Text.Trim());
      }
      if (txtReason.Text.Trim().Length > 0)
      {
        input[5] = new DBParameter("@Reason", DbType.String, 500, txtReason.Text.Trim());
      }

      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantity_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      this.transactionPid = resultSave;
      if (resultSave == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Transaction detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      bool flag = true;
      //Delete only one time
      string strDelete = "";
      foreach (long pidDelete in this.listDeletedPid)
      {
        if (pidDelete > 0)
        {
          strDelete += pidDelete.ToString() + ",";
        }
      }
      if (strDelete.Length > 0)
      {
        strDelete = "," + strDelete;
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@DeleteList", DbType.String, 4000, strDelete);
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantityDetail_Delete", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        DBParameter[] input = new DBParameter[8];
        if (DBConvert.ParseLong(ultData.Rows[i].Cells["TransactionDetailPid"].Value.ToString()) != long.MinValue)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["TransactionDetailPid"].Value.ToString()));
        }
        input[1] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
        input[2] = new DBParameter("@WorkOrderPid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["WorkOrderPid"].Value.ToString()));
        input[3] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, ultData.Rows[i].Cells["CarcassCode"].Value.ToString());
        input[4] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["NewQty"].Value.ToString()));
        input[5] = new DBParameter("@OldQty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["OldQty"].Value.ToString()));
        if (ultData.Rows[i].Cells["Remark"].Value.ToString().Length > 0)
        {
          input[6] = new DBParameter("@Remark", DbType.AnsiString, 256, ultData.Rows[i].Cells["Remark"].Value.ToString());
        }
        input[7] = new DBParameter("@FlagAccept", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["FlagAccept"].Value.ToString()));
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantityDetail_Edit", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }

      }
      return flag;
    }

    /// <summary>
    /// Add Detail
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private bool AddDetail(DBParameter[] input, int index)
    {
      bool flag = true;
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantityDetail_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      if (resultSave == 0)
      {
        flag = false;
      }
      else
      {
        this.AddWoLinkSo(resultSave, index);
      }
      return flag;
    }

    /// <summary>
    /// Add Data
    /// </summary>
    private void AddData()
    {
      try
      {
        for (int i = 0; i < ultInformation.Rows.Count; i++)
        {
          if (ultInformation.Rows[i].Cells["Select"].Value.ToString() == "1")
          {
            DBParameter[] input = new DBParameter[7];
            input[0] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
            input[1] = new DBParameter("@WorkOrderPid", DbType.Int64, DBConvert.ParseLong(ultInformation.Rows[i].Cells["WorkOrderPid"].Value.ToString()));
            input[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, ultInformation.Rows[i].Cells["CarcassCode"].Value.ToString());
            input[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultInformation.Rows[i].Cells["NewQty"].Value.ToString()));
            input[4] = new DBParameter("@OldQty", DbType.Int32, DBConvert.ParseInt(ultInformation.Rows[i].Cells["OldQty"].Value.ToString()));
            if (ultInformation.Rows[i].Cells["Remark"].Value.ToString().Length > 0)
            {
              input[5] = new DBParameter("@Remark", DbType.AnsiString, 256, ultInformation.Rows[i].Cells["Remark"].Value.ToString());
            }
            input[6] = new DBParameter("@FlagAccept", DbType.Int32, 0);
            this.AddDetail(input, i);
          }
        }
      }
      catch
      { }
    }

    /// <summary>
    /// Add WorkOrder Link SaleOrder
    /// </summary>
    private void AddWoLinkSo(long TransactionDetailPid, int RowIndex)
    {
      int i = RowIndex;
      if (ultInformation.Rows[i].Cells["Select"].Value.ToString() == "1")
      {
        for (int j = 0; j < ultInformation.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          int OpenQty = DBConvert.ParseInt(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["Open Qty"].Value.ToString());
          int OldQty = DBConvert.ParseInt(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["OldQty"].Value.ToString());

          if (OpenQty > 0 && OpenQty > OldQty)
          {
            DBParameter[] input = new DBParameter[4];
            input[0] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, DBConvert.ParseLong(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["SaleOrderDetailPid"].Value.ToString()));
            input[1] = new DBParameter("@WorkOrderPid", DbType.Int64, DBConvert.ParseLong(ultInformation.Rows[i].Cells["WorkOrderPid"].Value.ToString()));
            input[2] = new DBParameter("@ReviseQty", DbType.Int32, OpenQty - OldQty);
            input[3] = new DBParameter("@TransactionDetailPid", DbType.Int64, TransactionDetailPid);

            bool flag = true;
            DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantityWoLinkSo_Edit", input, output);
            long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
            if (resultSave == 0)
            {
              flag = false;
            }
          }
        }
      }
    }

    /// <summary>
    /// Confirm Transaction 
    /// </summary>
    /// <returns></returns>
    private bool ConfirmTransaction()
    {
      bool flag = true;
      if (this.transactionPid > 0 && (chkConfirm.Checked && (this.Status == 0 || this.Status == 1)))
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@TransactionPid", DbType.Int64, this.transactionPid);
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantity_Confirm", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }
      }
      return flag;
    }
    /// <summary>
    /// Return Transaction 
    /// </summary>
    /// <returns></returns>
    private bool ReturnTransaction()
    {
      bool flag = true;
      if (this.transactionPid > 0 && this.Status == 1)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@TransactionPid", DbType.Int64, this.transactionPid);
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantity_Return", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }
      }
      return flag;
    }
    /// <summary>
    /// Search Condition to adding detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Check valid
    /// </summary>
    /// <returns></returns>
    private int CheckValid()
    {
      int errorNumber = 0;
      bool checkReason = true;
      bool checkWo = true;
      int NewQty, TotalOpenQty;
      DataTable dt = new DataTable();
      for (int i = 0; i < ultInformation.Rows.Count; i++)
      {
        if (ultInformation.Rows[i].Cells["Select"].Value.ToString() == "1")
        {
          ultInformation.Rows[i].CellAppearance.BackColor = Color.White;
          NewQty = DBConvert.ParseInt(ultInformation.Rows[i].Cells["NewQty"].Value.ToString());
          TotalOpenQty = 0;
          for (int j = 0; j < ultInformation.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            TotalOpenQty += DBConvert.ParseInt(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["Open Qty"].Value.ToString());
          }
          if (NewQty != TotalOpenQty)
          {
            checkReason = false;
            errorNumber = 1;
            ultInformation.Rows[i].CellAppearance.BackColor = Color.Yellow;
          }
          long wo = DBConvert.ParseLong(ultInformation.Rows[i].Cells["WorkOrderPid"].Value);
          string carcass = ultInformation.Rows[i].Cells["CarcassCode"].Value.ToString();
          string cmtext = string.Format(@"SELECT COUNT(*)
                                              FROM TblPLNWoChangeNoteQuantity A
                                              INNER JOIN TblPLNWoChangeNoteQuantityDetail B ON A.Pid = B.TransactionPid
                                              WHERE A.[Status] <> 2 AND B.WorkOrderPid = {0} AND B.CarcassCode = '{1}'", wo, carcass);
          int rs = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(cmtext).ToString());
          if (rs > 0)
          {
            ultInformation.Rows[i].Appearance.BackColor = Color.Yellow;
            errorNumber = 3;
            checkReason = false;
          }
        }
      }
      if (errorNumber == 0)
      {
        if (this.Status == 1 && chkConfirm.Checked)
        {
          bool isAccepted = false;
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            if (ultData.Rows[i].Cells["FlagAccept"].Value.ToString() == "1")
            {
              isAccepted = true;
              break;
            }
          }
          if (!isAccepted)
          {
            if (WindowUtinity.ShowMessageConfirmFromText("Are you sure to confirm when nothing be accepted") != DialogResult.Yes)
            {
              errorNumber = 2;
            }
          }
        }
      }
      return errorNumber;
    }
    /// <summary>
    /// Check Close Wo
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckCloseWo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      for (int i = 0; i < ultInformation.Rows.Count; i++)
      {
        UltraGridRow rowIncrease = ultInformation.Rows[i];
        long wo = DBConvert.ParseLong(rowIncrease.Cells["WorkOrderPid"].Value.ToString());
        string cacass = rowIncrease.Cells["CarcassCode"].Value.ToString();
        commandText = string.Format(@"SELECT DISTINCT WorkOrderPid, CarcassCode
                                          FROM TblPLNWorkOrderConfirmedDetails
                                          WHERE WorkOrderPid = {0} AND CarcassCode = '{1}' 
                                                              AND (ISNULL(CloseCOM1, 0) = 1 OR ISNULL(CloseCAR, 0) = 1 
                                                              OR ISNULL(CloseAll, 0) = 1)", wo, cacass);

        dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          ultInformation.Rows[i].Appearance.BackColor = Color.Yellow;
          message = " WO is Close";
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Check Close Wo
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckWo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowIncrease = ultData.Rows[i];
        long wo = DBConvert.ParseLong(rowIncrease.Cells["WorkOrderPid"].Value.ToString());
        string cacass = rowIncrease.Cells["CarcassCode"].Value.ToString();
        commandText = string.Format(@"SELECT DISTINCT WorkOrderPid, CarcassCode
                                          FROM TblPLNWorkOrderConfirmedDetails
                                          WHERE WorkOrderPid = {0} AND CarcassCode = '{1}' 
                                                              AND (ISNULL(CloseCOM1, 0) = 1 OR ISNULL(CloseCAR, 0) = 1 
                                                              OR ISNULL(CloseAll, 0) = 1)", wo, cacass);

        dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
          message = " WO is Close";
          return false;
        }
      }
      return true;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Load Transaction Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["FlagAccept"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["FlagAccept"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["WorkOrderPid"].Header.Caption = "Work Order";
      e.Layout.Bands[0].Columns["NewQty"].Header.Caption = "New Qty";
      e.Layout.Bands[0].Columns["OldQty"].Header.Caption = "Old Qty";

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseLong(ultData.Rows[i].Cells["ErrorPid"].Value.ToString()) > 0)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
      }


      DataTable dtNew = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtNew.Columns.Count; i++)
      {
        if (dtNew.Columns[i].DataType == typeof(Int32) || dtNew.Columns[i].DataType == typeof(float) || dtNew.Columns[i].DataType == typeof(Double))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dtNew.Columns[i].DataType == typeof(DateTime))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (!(e.Layout.Bands[0].Columns[i].Key == "Remark"))
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }

      if (Status == 2)
      {
        e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      }
      if (!_1st)
      {
        if (Status == 1)
        {
          e.Layout.Bands[0].Columns["FlagAccept"].CellActivation = Activation.AllowEdit;
        }
        e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["TransactionDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ErrorPid"].Hidden = true;

      e.Layout.Bands[0].Columns["FlagAccept"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["FlagAccept"].Header.Caption = "Accepted";

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Search Condition
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[10];

      // ItemCode
      string text = string.Empty;
      text = this.txtItemCode.Text;
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@ItemCode", DbType.String, text);
      }

      // Customer
      text = string.Empty;
      text = this.txtCustomer.Text;
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@Customer", DbType.String, text);
      }


      // SaleCode
      text = string.Empty;
      text = this.txtSaleCode.Text;
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@SaleCode", DbType.String, text);
      }

      // Carcass Code
      text = this.txtCarcassCode.Text;
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@CarcassCode", DbType.String, text);
      }

      // Old Code
      text = this.txtOldCode.Text;
      if (text.Length > 0)
      {
        param[5] = new DBParameter("@OldCode", DbType.String, text);
      }
      // Wo From
      text = this.txtWoFrom.Text;
      if (DBConvert.ParseLong(text) > 0)
      {
        param[6] = new DBParameter("@WoFrom", DbType.String, DBConvert.ParseLong(text));
      }
      // Wo To
      text = this.txtWoTo.Text;
      if (DBConvert.ParseLong(text) > 0)
      {
        param[7] = new DBParameter("@WoTo", DbType.String, DBConvert.ParseLong(text));
      }
      DateTime shipdate;
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNChangeWoQuantity_Select", 600, param);
      dsSource.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { dsSource.Tables[0].Columns["WorkOrderPid"],
                                                                                 dsSource.Tables[0].Columns["CarcassCode"]},
                                              new DataColumn[] { dsSource.Tables[1].Columns["WorkOrderPid"],
                                                                                dsSource.Tables[1].Columns["CarcassCode"]}, false));

      ultInformation.DataSource = dsSource;
      for (int i = 0; i < ultInformation.Rows.Count; i++)
      {
        int totalOpen = 0;
        for (int j = 0; j < ultInformation.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          totalOpen += DBConvert.ParseInt(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["Open Qty"].Value.ToString());
        }
        ultInformation.Rows[i].Cells["Total Openned"].Value = totalOpen;
      }
      //// Load Column Hide/ Unhide
      //if (ultShowColumn.Rows.Count == 0)
      //{
      //    this.LoadColumnName();
      //}
      //else
      //{
      //    this.SetStatusColumn();
      //}
    }

    /// <summary>
    /// Get multi Item to searching
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkItemCode_CheckedChanged(object sender, EventArgs e)
    {
      ucItemCode.Visible = chkItemCode.Checked;
    }

    /// <summary>
    /// Get multi customer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkCustomer_CheckedChanged(object sender, EventArgs e)
    {
      ucCustomer.Visible = chkCustomer.Checked;
    }

    /// <summary>
    /// Item list
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucItemCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtItemCode.Text = this.ucItemCode.SelectedValue;
    }

    /// <summary>
    /// Customer list
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucCustomer_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtCustomer.Text = this.ucCustomer.SelectedValue;
    }

    /// <summary>
    /// Hide areas
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (chkHide.Checked)
      {
        grpInformation.Height = 140;
        grpInformation.Visible = false;
        grpData.Visible = true;
      }
      else if (chkHide.Checked == false)
      {
        grpData.Visible = false;
        grpInformation.Visible = true;
        grpInformation.Height = 325;
      }
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    //private void ChkAll_CheckedChange()
    //{
    //    for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
    //    {
    //        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
    //        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
    //        {
    //            try
    //            {
    //                ultInformation.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
    //            }
    //            catch
    //            { }
    //        }
    //    }
    //}

    /// <summary>
    /// Show columns
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>




    /// <summary>
    /// Add Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      bool chkValid = true;
      bool chkIsSelected = false;
      // Check close WO
      string message = string.Empty;
      bool success = this.CheckCloseWo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      int errorNumber = this.CheckValid();
      if (errorNumber == 1)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Qty");
        return;
      }
      if (errorNumber == 3)
      {
        WindowUtinity.ShowMessageError("ERR0028", "Transaction for this wo and carcass");
        return;
      }
      for (int i = 0; i < ultInformation.Rows.Count; i++)
      {
        if (String.Compare(ultInformation.Rows[i].Cells["Select"].Value.ToString(), "1", true) == 0)
        {
          chkIsSelected = true;
        }
      }
      if (!chkIsSelected)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0115", "Data");
      }
      else if (chkValid && success)
      {
        // Save Master
        this.SaveMaster();

        // Add Data
        this.AddData();

        // LoadData
        this.LoadData();
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0003");

        // Search 
        //this.Search();
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Ship Date");
      }
    }

    /// <summary>
    /// Save click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckWo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      int intCheckValid = this.CheckValid();
      if (intCheckValid == -1)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Reason");
      }
      else if (intCheckValid == 2)
      {
        this.LoadData();
        return;
      }
      else
      {
        if (this.SaveMaster())
        {

          if (this.SaveDetail())
          {
            if (this.ConfirmTransaction())
            {
              Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
            }
            else
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0026", "Quantity");
            }
          }
          else
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          }
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        }

        this.listDeletingPid = new ArrayList();
        this.listDeletedPid = new ArrayList();
        try
        {
          bool needToAdd = false;
          for (int i = 0; i < ultInformation.Rows.Count; i++)
          {
            if (ultInformation.Rows[i].Cells["Select"].Value.ToString() == "1")
            {
              needToAdd = true;
              break;
            }
          }
          if (needToAdd)
          {
            btnAdd_Click(sender, e);
          }
        }
        catch
        { }
        // Load Data
        this.LoadData();
      }
    }

    /// <summary>
    /// Init Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInformation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["OldQty"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Total Openned"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["NewQty"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[1].Columns["Item Confirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["OldQty"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[1].Columns["Remain"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[1].Columns["Open Qty"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[1].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["WoDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["WorkorderPid"].Hidden = true;
      e.Layout.Bands[1].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[1].Columns["OldQty"].Hidden = true;
      e.Layout.Bands[0].Columns["WorkOrderPid"].Header.Caption = "WO";
      DataSet dsNew = (DataSet)ultInformation.DataSource;
      for (int i = 0; i < dsNew.Tables[0].Columns.Count; i++)
      {
        if (dsNew.Tables[0].Columns[i].DataType == typeof(Int32) || dsNew.Tables[0].Columns[i].DataType == typeof(float) || dsNew.Tables[0].Columns[i].DataType == typeof(Double) || dsNew.Tables[0].Columns[i].DataType == typeof(long))
        {
          ultInformation.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dsNew.Tables[0].Columns[i].DataType == typeof(DateTime))
        {
          ultInformation.DisplayLayout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }
      for (int i = 0; i < dsNew.Tables[1].Columns.Count; i++)
      {
        if (dsNew.Tables[1].Columns[i].DataType == typeof(Int32) || dsNew.Tables[1].Columns[i].DataType == typeof(float) || dsNew.Tables[1].Columns[i].DataType == typeof(Double) || dsNew.Tables[1].Columns[i].DataType == typeof(long))
        {
          ultInformation.DisplayLayout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dsNew.Tables[1].Columns[i].DataType == typeof(DateTime))
        {
          ultInformation.DisplayLayout.Bands[1].Columns[i].Format = "dd-MMM-yyyy";
        }
      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (!(e.Layout.Bands[0].Columns[i].Key == "NewQty" || e.Layout.Bands[0].Columns[i].Key == "Select"))
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        if (!(e.Layout.Bands[1].Columns[i].Key == "Open Qty"))
        {
          e.Layout.Bands[1].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      e.Layout.Bands[1].Layout.AutoFitColumns = false;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Select All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultInformation.Rows.Count; i++)
      {
        if (ultInformation.Rows[i].Cells["Selected"].Value.ToString() != "1")
        {
          ultInformation.Rows[i].Cells["Selected"].Value = true;
        }
        else
        {
          ultInformation.Rows[i].Cells["Selected"].Value = false;

        }
      }
    }

    /// <summary>
    /// Mouse Double Click Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {

    }

    /// <summary>
    /// After cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      try
      {
        int Parentindex = e.Cell.Row.ParentRow.Index;
        int ParentQty = DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["NewQty"].Value.ToString());
        int ChildQty = 0;
        for (int i = 0; i < ultInformation.Rows[Parentindex].ChildBands[0].Rows.Count; i++)
        {
          try
          {
            if (DBConvert.ParseInt(ultInformation.Rows[Parentindex].ChildBands[0].Rows[i].Cells["Open Qty"].Value.ToString()) > int.MinValue)
            {
              ChildQty += DBConvert.ParseInt(ultInformation.Rows[Parentindex].ChildBands[0].Rows[i].Cells["Open Qty"].Value.ToString());
            }
            else
            {
              ChildQty += 0;
            }
          }
          catch
          {
            ChildQty += 0;
          }
        }
        if (ParentQty < ChildQty)
        {
          e.Cell.Row.Cells["Open Qty"].Value = e.Cell.Row.Cells["OldQty"].Value;
        }
        if (ultInformation.Rows[Parentindex].ChildBands[0].Rows.Count > 0)
        {
          e.Cell.Row.ParentRow.Cells["Select"].Value = true;
        }
        else
        {
          e.Cell.Row.ParentRow.Cells["Select"].Value = false;

        }
        if (String.Compare(e.Cell.Column.Key, "Open Qty", true) == 0)
        {
          int totalOpen = 0;
          for (int i = 0; i < e.Cell.Row.ParentRow.ChildBands[0].Rows.Count; i++)
          {
            totalOpen += DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Open Qty"].Text.ToString());
          }
          e.Cell.Row.ParentRow.Cells["Total Openned"].Value = totalOpen;
        }
      }
      catch
      { }
    }

    /// <summary>
    /// Before cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (String.Compare(e.Cell.Column.Key, "NewQty", true) == 0)
      {
        if (DBConvert.ParseInt(e.Cell.Row.Cells["NewQty"].Text) < 0)
        {
          e.Cancel = true;
        }
        else if (DBConvert.ParseInt(e.Cell.Row.Cells["NewQty"].Text) < DBConvert.ParseInt(e.Cell.Row.Cells["OldQty"].Text))
        {
          e.Cancel = true;
        }
      }
      if (String.Compare(e.Cell.Column.Key, "Open Qty", true) == 0)
      {
        if (DBConvert.ParseInt(e.Cell.Row.Cells["Open Qty"].Text) < 0)
        {
          e.Cancel = true;
        }
        else if (DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["NewQty"].Text) < DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["OldQty"].Text))
        {
          e.Cancel = true;
        }
        else if (DBConvert.ParseInt(e.Cell.Row.Cells["Open Qty"].Text) > DBConvert.ParseInt(e.Cell.Row.Cells["Remain"].Text))
        {
          e.Cancel = true;
        }
        else if (DBConvert.ParseInt(e.Cell.Row.Cells["Open Qty"].Text) < DBConvert.ParseInt(e.Cell.Row.Cells["OldQty"].Text))
        {
          e.Cancel = true;
        }

      }
    }

    /// <summary>
    /// Hide change state
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkHide_CheckStateChanged(object sender, EventArgs e)
    {
      if (chkHide.CheckState == CheckState.Indeterminate)
      {
        grpInformation.Height = 140;
        grpInformation.Visible = true;
        grpInformation.Visible = true;
      }
    }

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      foreach (Control o in tableLayoutPanel4.Controls)
      {
        if (o.GetType() == typeof(TextBox))
        {
          o.Text = "";
        }
      }
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
    /// Before Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long detailPid = DBConvert.ParseLong(row.Cells["TransactionDetailPid"].Value.ToString());
        if (detailPid != long.MinValue)
        {
          listDeletingPid.Add(detailPid);
        }
      }
    }

    /// <summary>
    /// After Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        listDeletedPid.Add(pid);
      }
    }

    /// <summary>
    /// BeforeCellActivate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInformation_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      if (String.Compare(e.Cell.Column.Key, "Open Qty", true) == 0)
      {
        if (DBConvert.ParseInt(e.Cell.Row.Cells["Item Confirm"].Value.ToString()) != 1)
        {
          e.Cancel = true;
        }
      }
    }

    /// <summary>
    /// btnReturn_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReturn_Click(object sender, EventArgs e)
    {
      chkConfirm.Checked = false;
      if (this.ReturnTransaction())
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0051", "Return");
        this.LoadData();
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERRO122", "Return");
      }
    }

    #endregion Event
    private void btnImport_Click(object sender, EventArgs e)
    {
      viewPLN_02_033 view = new viewPLN_02_033();
      view.transactionPid = this.transactionPid;
      Shared.Utility.WindowUtinity.ShowView(view, " WO Increase Information", false, Shared.Utility.ViewState.ModalWindow);
      this.transactionPid = view.transactionPid;
      this.LoadData();
    }

    private void chkConfirm_CheckedChanged(object sender, EventArgs e)
    {

    }
  }
}
