/*
  Author      : Do Tam
  Date        : 20/01/2013
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_10_013 : MainUserControl
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
    public viewPLN_10_013()
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
      grpShowCol.Height = 80;
      pnlRight.Visible = true;
      _1st = btn1st.Visible;
      _2nd = btn2sd.Visible;
      if (_1st)
      {
        lblStatus.Text = "PLN Account";
      }
      else
      {
        lblStatus.Text = "CSD Account";
      }
      pnlRight.Visible = false;

      if (this.transactionPid == long.MinValue)
      {
        txtTransaction.Text = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNGetNewChangeNoteNo('')", null).Rows[0][0].ToString();
        txtCreateBy.Text = SharedObject.UserInfo.EmpName.ToString();
        txtCreateDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
        Status = 0;
      }
      else
      {
        DBParameter[] param = new DBParameter[1];
        param[0] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
        string storeName = "spPLNSaleOrderChangeNoteShipDate_Select";
        DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
        Status = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["ST"].ToString());
        txtTransaction.Text = dsSource.Tables[0].Rows[0]["TransactionCode"].ToString();
        txtCreateBy.Text = dsSource.Tables[0].Rows[0]["CreateBy"].ToString();
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
          chkConfirm.Enabled = true;
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
        }
        else if (Status == 1)
        {
          chkConfirm.Checked = false;
          chkConfirm.Enabled = true;
          btnSave.Enabled = true;
        }
        else
        {
          chkConfirm.Checked = false;
          chkConfirm.Enabled = false;
          btnSave.Enabled = false;
        }
        this.grpSearch.Visible = false;
      }

      if (Status > 0)
      {
        btnAdd.Visible = false;
        btnSearch.Visible = false;
        btnClear.Visible = false;
      }
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
      
        Utility.ExportToExcelWithDefaultPath(ultData, "Change ShipDate");
      }
    }
    /// <summary>
    /// Load Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_10_013_Load(object sender, EventArgs e)
    {
      dt_POFrom.Value = DateTime.Today;
      dt_POTo.Value = DateTime.Today.AddDays(7);
      LoadItemCode();
      LoadCustomer();
      ultdtChangeShipDate.Value = DBNull.Value;
      this.LoadDDReason();
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
    private void LoadColumnName()
    {
      DataTable dtNew = new DataTable();
      DataTable dtColumn = ((DataSet)ultInformation.DataSource).Tables[0];
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        dtNew.Columns[column.ColumnName].DefaultValue = 0;

        if (string.Compare(column.ColumnName, "Revision", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "SONo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "PONo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
        if (string.Compare(column.ColumnName, "[Cust.]", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
        if (string.Compare(column.ColumnName, "ContainerNo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
        if (string.Compare(column.ColumnName, "ConfirmShipDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
        if (string.Compare(column.ColumnName, "OldConfirmShipDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
        if (string.Compare(column.ColumnName, "ShipDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      ultShowColumn.DataSource = dtNew;
      ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
    }

    /// <summary>
    /// Set Status Column When Search
    /// </summary>
    private void SetStatusColumn()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultInformation.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// Save Transaction Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster()
    {
      DBParameter[] input = new DBParameter[4];
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
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderChangeNoteShipDate_Edit", input, output);
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
        DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderChangeNoteShipDateDetail_Delete", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        DBParameter[] input = new DBParameter[12];
        if (DBConvert.ParseLong(ultData.Rows[i].Cells["ChangeShipDateDetailPid"].Value.ToString()) != long.MinValue)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["ChangeShipDateDetailPid"].Value.ToString()));
        }
        input[1] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
        input[2] = new DBParameter("@SaleOrderPid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["SOPid"].Value.ToString()));
        input[3] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultData.Rows[i].Cells["ItemCode"].Value.ToString());
        input[4] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["Revision"].Value.ToString()));
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Reason"].Value.ToString()) > 0)
        {
          input[5] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["Reason"].Value.ToString()));
        }
        if (DBConvert.ParseDateTime(ultData.Rows[i].Cells["NewShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
        {
          input[6] = new DBParameter("@NewShipDate", DbType.DateTime, DBConvert.ParseDateTime(ultData.Rows[i].Cells["NewShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
        }
        if (DBConvert.ParseDateTime(ultData.Rows[i].Cells["OriginalShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
        {
          input[7] = new DBParameter("@OldShipdate", DbType.DateTime, DBConvert.ParseDateTime(ultData.Rows[i].Cells["OriginalShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
        }
        if (ultData.Rows[i].Cells["CSRemark"].Value.ToString().Length > 0)
        {
          input[8] = new DBParameter("@CSRemark", DbType.AnsiString, 256, ultData.Rows[i].Cells["CSRemark"].Value.ToString());
        }
        if (ultData.Rows[i].Cells["PLARemark"].Value.ToString().Length > 0)
        {
          input[9] = new DBParameter("@PLARemark", DbType.AnsiString, 256, ultData.Rows[i].Cells["PLARemark"].Value.ToString());
        }
        input[10] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["Qty"].Value.ToString()));
        input[11] = new DBParameter("@FlagAccept", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["FlagAccept"].Value.ToString()));
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderChangeNoteShipDateDetail_Edit", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }
      }
      return flag;
    }

    /// <summary>
    /// Insert SO Detail Confirmed Shipdate
    /// </summary>
    /// <returns></returns>
    private bool SaveSODetailConfirmedShipdate()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@ChangeNotePid", DbType.Int64, this.transactionPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, 0);
      DataBaseAccess.ExecuteStoreProcedure("spPLNSODetailConfirmedShipdate_Insert", input, output);
      if (DBConvert.ParseLong(output[0].Value.ToString()) <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Add Detail
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private bool AddDetail(DBParameter[] input)
    {
      bool flag = true;
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderChangeNoteShipDateDetail_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      if (resultSave <= 0)
      {
        flag = false;
      }
      return flag;
    }

    /// <summary>
    /// Add Data
    /// </summary>
    private void AddData()
    {
      for (int i = 0; i < ultInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseDateTime(ultInformation.Rows[i].Cells["ShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != null)
        {
          //DataTable dt = ((DataSet)ultInformation.DataSource).Tables[0];

          if (ultInformation.Rows[i].Cells["Selected"].Value.ToString() == "1")
          {

            if (ultInformation.Rows[i].ChildBands[0].Rows.Count == 0)
            {

              DBParameter[] input = new DBParameter[12];
              input[1] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
              input[2] = new DBParameter("@SaleOrderPid", DbType.Int64, DBConvert.ParseLong(ultInformation.Rows[i].Cells["SOPid"].Value.ToString()));
              input[3] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultInformation.Rows[i].Cells["ItemCode"].Value.ToString());
              input[4] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(ultInformation.Rows[i].Cells["Revision"].Value.ToString()));
              if (DBConvert.ParseDateTime(ultInformation.Rows[i].Cells["ShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
              {
                input[6] = new DBParameter("@NewShipDate", DbType.DateTime, DBConvert.ParseDateTime(ultInformation.Rows[i].Cells["ShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
              }
              if (DBConvert.ParseDateTime(ultInformation.Rows[i].Cells["OldConfirmShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
              {
                input[7] = new DBParameter("@OldShipdate", DbType.DateTime, DBConvert.ParseDateTime(ultInformation.Rows[i].Cells["OldConfirmShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
              }

              input[10] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultInformation.Rows[i].Cells["Qty"].Value.ToString()));
              this.AddDetail(input);
            }
            else
            {
              for (int j = 0; j < ultInformation.Rows[i].ChildBands[0].Rows.Count; j++)
              {
                if (DBConvert.ParseInt(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString()) > 0 && DBConvert.ParseInt(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["Reason"].Value.ToString()) != int.MinValue)
                //&& DBConvert.ParseDateTime(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["ConfirmShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != null)
                {
                  DBParameter[] input = new DBParameter[12];
                  input[1] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
                  input[2] = new DBParameter("@SaleOrderPid", DbType.Int64, DBConvert.ParseLong(ultInformation.Rows[i].Cells["SOPid"].Value.ToString()));
                  input[3] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultInformation.Rows[i].Cells["ItemCode"].Value.ToString());
                  input[4] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(ultInformation.Rows[i].Cells["Revision"].Value.ToString()));
                  if (DBConvert.ParseDateTime(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["ConfirmShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                  {
                    input[6] = new DBParameter("@NewShipDate", DbType.DateTime, DBConvert.ParseDateTime(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["ConfirmShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
                  }
                  if (DBConvert.ParseDateTime(ultInformation.Rows[i].Cells["OldConfirmShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                  {
                    input[7] = new DBParameter("@OldShipdate", DbType.DateTime, DBConvert.ParseDateTime(ultInformation.Rows[i].Cells["OldConfirmShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
                  }
                  input[8] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["Reason"].Value.ToString()));
                  input[10] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString()));
                  this.AddDetail(input);
                }
              }
            }
          }
        }
      }
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
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        // Check Reason
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Reason"].Value.ToString()) < 0)
        {
          ultData.Rows[i].Cells["Reason"].Appearance.BackColor = Color.Yellow;
          return -1;
        }
        // Check New Confirmed Shipdate
        if (_1st)
        {
          if (ultData.Rows[i].Cells["NewShipDate"].Value.ToString().Length > 0 && DBConvert.ParseDateTime(ultData.Rows[i].Cells["NewShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) <= DateTime.Now)
          {
            ultData.Rows[i].Cells["NewShipDate"].Appearance.BackColor = Color.Yellow;
            return -2;
          }
        }
        else if (_2nd)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["FlagAccept"].Value.ToString()) == 1 &&
              ultData.Rows[i].Cells["NewShipDate"].Value.ToString().Length > 0 && DBConvert.ParseDateTime(ultData.Rows[i].Cells["NewShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) <= DateTime.Now)
          {
            ultData.Rows[i].Cells["NewShipDate"].Appearance.BackColor = Color.Yellow;
            return -2;
          }
        }
      }
      return 0;
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

      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["OldCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["FlagAccept"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["FlagAccept"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["CSRemark"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["NewShipDate"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["OriginalShipDate"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["PLARemark"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Reason"].CellAppearance.BackColor = Color.LightBlue;

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
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
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
          e.Layout.Bands[0].Columns["CSRemark"].CellActivation = Activation.AllowEdit;
        }
        e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      }
      else
      {
        if (Status == 0)
        {
          e.Layout.Bands[0].Columns["NewShipDate"].CellActivation = Activation.AllowEdit;
          e.Layout.Bands[0].Columns["Reason"].CellActivation = Activation.AllowEdit;
          e.Layout.Bands[0].Columns["PLARemark"].CellActivation = Activation.AllowEdit;
        }
      }
      ultdrResion.DataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable("Select Code,CAST(Code AS VARCHAR) +' - ' + Value [Value] from TblBOMCodeMaster where [Group] = 16005 ORDER BY Kind, Code", null);
      ultdrResion.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultdrResion.DisplayLayout.Bands[0].Columns["Value"].Width = 200;
      ultdrResion.ValueMember = "Code";
      ultdrResion.DisplayMember = "Value";
      e.Layout.Bands[0].Columns["Reason"].ValueList = ultdrResion;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ChangeShipDateDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["FlagAccept"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["FlagAccept"].Header.Caption = "Accepted";
      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void LoadDDReason()
    {
      string cm = "Select Code,CAST(Code AS VARCHAR) +' - ' + Value [Value] from TblBOMCodeMaster where [Group] = 16005 ORDER BY Kind, Code";
      DataTable dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cm);
      Utility.LoadUltraDropDown(ultDDReasonInfor, dt, "Code", "Value", "Code");
    }
    /// <summary>
    /// Search Condition
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[9];

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
      DateTime shipdate;
      if (this.dt_POFrom.Value != null)
      {
        DateTime orderDate = DBConvert.ParseDateTime(dt_POFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        param[6] = new DBParameter("@ShipDateFrom", DbType.DateTime, orderDate);
      }
      if (this.dt_POTo.Value != null)
      {
        DateTime orderDate = DBConvert.ParseDateTime(dt_POTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        orderDate = orderDate.AddDays(1);
        param[7] = new DBParameter("@ShipDateTo", DbType.DateTime, orderDate);
      }
      // Carcass Code
      text = this.txtContainerNo.Text;
      if (text.Length > 0)
      {
        param[8] = new DBParameter("@ContainerNo", DbType.String, text);
      }
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNChangeSOShipDate_Select", 60, param);
      DataSet dsNew = new DaiCo.Shared.DataSetSource.Planning.dsPLNChangeSOShipDate();
      dsNew.Tables["Parent"].Merge(dsSource.Tables[0]);
      //dsNew.Tables["Child"].Merge(dsSource.Tables[1]);
      ultInformation.DataSource = dsNew;
      // Load Column Hide/ Unhide
      if (ultShowColumn.Rows.Count == 0)
      {
        this.LoadColumnName();
      }
      else

      {
        this.SetStatusColumn();
      }
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
        tblData.Visible = false;
        grpData.Visible = true;
        grpShowCol.Visible = false;
      }
      else if (chkHide.Checked == false)
      {
        grpData.Visible = false;
        grpInformation.Visible = true;
        tblData.Visible = true;
        grpInformation.Height = 325;
        grpShowCol.Visible = true;
      }
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          try
          {
            ultInformation.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
          }
          catch
          { }
        }
      }
    }

    /// <summary>
    /// Show columns
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultShowColumn_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          ultInformation.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          ultInformation.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
        }
      }
      else
      {
        for (int i = 1; i < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          row.Cells[i].Value = e.Cell.Text;
        }
        this.ChkAll_CheckedChange();
      }
    }

    /// <summary>
    /// Init 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultShowColumn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      DataTable dtColumn = (DataTable)ultShowColumn.DataSource;
      int count = dtColumn.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["SaleCode"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["OldCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["SoPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Hidden = true;
    }

    /// <summary>
    /// Add Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      bool chkValid = true;
      bool chkIsSelected = false;
      for (int i = 0; i < ultInformation.Rows.Count; i++)
      {
        if (String.Compare(ultInformation.Rows[i].Cells["Selected"].Value.ToString(), "1", true) == 0)
        {
          chkIsSelected = true;
        }

        // Check ShipDate
        if (ultInformation.Rows[i].ChildBands[0].Rows.Count == 0)
        {
          // Parent
          if (String.Compare(ultInformation.Rows[i].Cells["Selected"].Value.ToString(), "1", true) == 0
              && DBConvert.ParseDateTime(ultInformation.Rows[i].Cells["ShipDate"].Value.ToString(),
                  USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
          {
            ultInformation.Rows[i].CellAppearance.BackColor = Color.Yellow;
            if (chkValid)
              chkValid = false;
          }
          else
          {
            ultInformation.Rows[i].CellAppearance.BackColor = Color.White;
          }
        }
        else
        {
          // Child
          for (int j = 0; j < ultInformation.Rows[i].ChildBands[0].Rows.Count; j++)
          {

            if (ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["ConfirmShipDate"].Value.ToString().Length > 0 && DBConvert.ParseDateTime(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["ConfirmShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) <= DateTime.Now)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "New ShipDate");
              return;
            }
            if (String.Compare(ultInformation.Rows[i].Cells["Selected"].Value.ToString(), "1", true) == 0 && DBConvert.ParseInt(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["Reason"].Value.ToString()) == int.MinValue)

            //&& DBConvert.ParseDateTime(ultInformation.Rows[i].ChildBands[0].Rows[j].Cells["ConfirmShipDate"].Value.ToString(),
            //    USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
            {


              ultInformation.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Yellow;


              if (chkValid)
                chkValid = false;
            }
            else
            {
              ultInformation.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.White;
            }
          }
        }
      }
      if (!chkIsSelected)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0115", "Data");
      }
      else if (chkValid)
      {
        // Save Master
        this.SaveMaster();

        // Add Data
        this.AddData();

        // LoadData
        this.LoadData();
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0003");

        // Search 
        this.Search();
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Reason");
      }
    }

    /// <summary>
    /// Save click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      int valid = this.CheckValid();
      if (valid == -1)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Reason");
      }
      else
      if (valid == -2)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "New ShipDate");
      }
      else
      {
        if (this.SaveMaster())
        {
          if (this.SaveDetail())
          {
            if (this.SaveSODetailConfirmedShipdate())
            {
              Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
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

          //if (this.SaveDetail())
          //{
          //  Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
          //}
          //else
          //{
          //  Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          //}
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        }

        this.listDeletingPid = new ArrayList();
        this.listDeletedPid = new ArrayList();

        // Load Data
        this.LoadData();

        if (grpSearch.Visible == true)
        {
          // Search 
          this.Search();
        }
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
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["OldCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Selected"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["PODate"].Hidden = true;
      e.Layout.Bands[0].Columns["OrderQty"].Hidden = true;
      e.Layout.Bands[0].Columns["CancelledQty"].Hidden = true;
      e.Layout.Bands[0].Columns["ShippedQty"].Hidden = true;
      e.Layout.Bands[0].Columns["BalanceQty"].Hidden = true;
      e.Layout.Bands[0].Columns["SpecialDescription"].Hidden = true;
      e.Layout.Bands[0].Columns["CBM"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ShipDate"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[1].Columns["ConfirmShipDate"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["ContainerPid"].Hidden = true;

      DataSet dsNew = (DataSet)ultInformation.DataSource;
      for (int i = 0; i < dsNew.Tables[0].Columns.Count; i++)
      {
        if (dsNew.Tables[0].Columns[i].DataType == typeof(Int32) || dsNew.Tables[0].Columns[i].DataType == typeof(float) || dsNew.Tables[0].Columns[i].DataType == typeof(Double))
        {
          ultInformation.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dsNew.Tables[0].Columns[i].DataType == typeof(DateTime))
        {
          ultInformation.DisplayLayout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }

      e.Layout.Bands[1].Columns["ConfirmShipDate"].Format = "dd-MMM-yyyy";
      e.Layout.Bands[0].Columns["OldConfirmShipDate"].Hidden = true;
      e.Layout.Bands[0].Columns["ConfirmShipDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Reason"].ValueList = ultDDReasonInfor;

      e.Layout.Bands[1].Layout.AutoFitColumns = false;
      e.Layout.Bands[1].Columns["ConfirmShipDate"].Width = 150;
      e.Layout.Bands[1].Columns["Qty"].Width = 100;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[1].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

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
    /// Change Ship Date
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnChangeShipDate_Click(object sender, EventArgs e)
    {
      bool IsChanged = false;
      if (ultdtChangeShipDate.Value == null)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0125", "Change Ship Date");
      }
      else
      {
        for (int i = 0; i < ultInformation.Rows.Count; i++)
        {
          if (ultInformation.Rows[i].Cells["Selected"].Value.ToString() == "1")
          {
            ultInformation.Rows[i].Cells["ShipDate"].Value = DBConvert.ParseDateTime(ultdtChangeShipDate.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
            IsChanged = true;
          }
        }
        if (!IsChanged)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0096", "Information", "Change Ship Date");
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0001");
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
      try
      {
        ultInformation.Selected.Rows[0].Cells["ConfirmShipDate"].Value = DBConvert.ParseDateTime(ultdtChangeShipDate.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
      }
      catch
      { }
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
        int ParentQty = DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["Qty"].Value.ToString());
        int ChildQty = 0;
        for (int i = 0; i < ultInformation.Rows[Parentindex].ChildBands[0].Rows.Count; i++)
        {
          try
          {
            if (DBConvert.ParseInt(ultInformation.Rows[Parentindex].ChildBands[0].Rows[i].Cells["Qty"].Value.ToString()) > int.MinValue)
            {
              ChildQty += DBConvert.ParseInt(ultInformation.Rows[Parentindex].ChildBands[0].Rows[i].Cells["Qty"].Value.ToString());
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
          e.Cell.Row.Cells["Qty"].Value = 0;
        }
        else
        {
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) == int.MinValue)
          {
            e.Cell.Row.Cells["Qty"].Value = ParentQty - ChildQty;
          }
        }
        if (ultInformation.Rows[Parentindex].ChildBands[0].Rows.Count > 0)
        {
          e.Cell.Row.ParentRow.Cells["Selected"].Value = true;
        }
        else
        {
          e.Cell.Row.ParentRow.Cells["Selected"].Value = false;

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
      if (String.Compare(e.Cell.Column.Key, "ConfirmShipDate", true) == 0 && e.Cell.Row.ParentRow != null)
      {
        if (DBConvert.ParseDateTime(e.Cell.Row.Cells["ConfirmShipDate"].Text, USER_COMPUTER_FORMAT_DATETIME) == DBConvert.ParseDateTime(e.Cell.Row.ParentRow.Cells["ConfirmShipDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME))
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0006", "Confirm Ship Date");
          e.Cancel = true;
        }
      }
      if (String.Compare(e.Cell.Column.Key, "Qty", true) == 0 && e.Cell.Row.ParentRow != null)
      {
        if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Text) < 0 && e.Cell.Row.Cells["Qty"].Text != "")
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Qty");
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
        tblData.Visible = true;
        grpInformation.Visible = true;
        grpShowCol.Visible = true;
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
      dt_POFrom.Value = null;
      dt_POTo.Value = null;
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
        long detailPid = DBConvert.ParseLong(row.Cells["ChangeShipDateDetailPid"].Value.ToString());
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

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }
    private void button1_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {

        Excel.Workbook xlBook;
        Utility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Register So Change ShipDate Note Report", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "Register So Change ShipDate Note Report";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }

    private void chkApprovedAll_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (chkApprovedAll.Checked)
        {
          row.Cells["FlagAccept"].Value = 1;
        }
        else
        {
          row.Cells["FlagAccept"].Value = 0;
        }
      }
    }
    #endregion Event   
  }
}
