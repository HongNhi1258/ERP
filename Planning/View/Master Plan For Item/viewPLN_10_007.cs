/*  TRAN HUNG
 *  Date: 04/08/2012
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Planning.View;
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
  public partial class viewPLN_10_007 : MainUserControl
  {
    #region Field
    public string Itemcode = string.Empty;
    public int Revision = int.MinValue;
    public long Saleorderpid = long.MinValue;
    public bool value;
    private IList listDeleteDetailPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    #endregion Field

    #region Load
    public viewPLN_10_007()
    {
      InitializeComponent();
    }
    private void Search()
    {
      DBParameter[] input = new DBParameter[5];
      string Itemcode = txtItemCode.Text.Trim();
      if (Itemcode.Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 255, Itemcode);
      }
      int Revision = DBConvert.ParseInt(txtRevision.Text.Trim());
      if (Revision != int.MinValue)
      {
        input[1] = new DBParameter("@Revision", DbType.Int32, Revision);
      }
      if (ultcbmSaleorderdetail.Text.Trim().Length > 0)
      {
        long Saleorder = DBConvert.ParseLong(ultcbmSaleorderdetail.Value.ToString());

        if (Saleorder != long.MinValue)
        {
          input[2] = new DBParameter("@SaleorderPid", DbType.Int64, Saleorder);
        }
      }
      long Wo = DBConvert.ParseLong(ultcbmWo.Text.Trim());
      if (Wo != long.MinValue)
      {
        input[3] = new DBParameter("@Wo", DbType.Int64, Wo);
      }
      if (chkIsSubcon.Checked)
      {
        input[4] = new DBParameter("@Isubcon", DbType.Int32, 1);
      }
      DataSet dsSearch = DataBaseAccess.SearchStoreProcedure("spWoLinkSoInformation_Select", input);
      this.ultSaleorderdetail.DataSource = null;
      ultSaleorderdetail.DataSource = dsSearch.Tables[0];
      // Check Lock
      for (int i = 0; i < ultSaleorderdetail.Rows.Count; i++)
      {
        UltraGridRow row = ultSaleorderdetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Lock"].Value) == 1)
        {
          row.Activation = Activation.ActivateOnly;
          row.Cells["Lock"].Appearance.BackColor = Color.Red;
        }
      }
      // End
      ultWO.DataSource = dsSearch.Tables[1];
    }

    /// <summary>
    /// Load combobox ItemCode
    /// </summary>
    private void LoadComboboxItemCode()
    {
      string commandText = string.Format(@"SELECT distinct BOMI.ItemCode , BOMB.Name  FROM TblBOMItemInfo BOMI
                                            INNER JOIN TblBOMItemBasic BOMB ON BOMI.ItemCode = BOMB.ItemCode   
                                            ORDER BY ItemCode DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucUltraListItem.DataSource = dt;
      ucUltraListItem.ColumnWidths = "100; 200";
      ucUltraListItem.DataBind();
      ucUltraListItem.ValueMember = "ItemCode";
    }
    /// <summary>
    /// Load combobox Wo
    /// </summary>
    private void LoadComboboxWo()
    {
      string commandText = string.Format(@"SELECT Pid   FROM TblPLNWorkOrder ORDER BY Pid DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultcbmWo.DataSource = dt;
      ultcbmWo.DisplayMember = "Pid";
      ultcbmWo.ValueMember = "Pid";
      ultcbmWo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbmWo.DisplayLayout.Bands[0].Columns["Pid"].Width = 200;
    }
    /// <summary>
    /// Load combobox Saleorderdetail
    /// </summary>
    private void LoadComboboxSaleorderdetail()
    {
      string commandText = string.Format(@"SELECT Pid ,SaleNo FROM TblPLNSaleOrder ORDER BY SaleNo DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultcbmSaleorderdetail.DataSource = dt;
      ultcbmSaleorderdetail.DisplayMember = "SaleNo";
      ultcbmSaleorderdetail.ValueMember = "Pid";
      ultcbmSaleorderdetail.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbmSaleorderdetail.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultcbmSaleorderdetail.DisplayLayout.Bands[0].Columns["SaleNo"].Width = 200;
    }

    /// <summary>
    /// Load DropDown ItemCode
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// 
    private void LoadDropDownItemCode()
    {
      string commandText = "SELECT ItemCode, Revision FROM TblBOMItemInfo ORDER BY ItemCode DESC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDrItemCode.DataSource = dt;
      ultDrItemCode.DisplayMember = "ItemCode";
      ultDrItemCode.ValueMember = "ItemCode";
    }

    /// <summary>
    /// Load DropDown Wo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// 
    private void LoadDropDownWo()
    {
      string commandText = string.Format(@"SELECT Pid   FROM TblPLNWorkOrder ORDER BY Pid DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDrWo.DataSource = dt;
      ultDrWo.DisplayMember = "Pid";
      ultDrWo.ValueMember = "Pid";
      ultDrWo.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load DropDown SaleorderDetail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// 
    private void LoadDropDownSaleorderdetail()
    {
      string commandText = string.Empty;
      commandText = " SELECT SaleOrderDetailPid Pid , SaleNo, CONVERT(VARCHAR,ScheduleDelivery,103) ScheduleDelivery, ItemCode, Revision, Remain, Balance ";
      commandText += " FROM VPLNMasterPlan";
      commandText += " ORDER BY SaleNo, ItemCode, CONVERT(VARCHAR,ScheduleDelivery,103) DESC";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDrSaleorder.DataSource = dt;
      ultDrSaleorder.DisplayMember = "SaleNo";
      ultDrSaleorder.ValueMember = "Pid";
      ultDrSaleorder.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDrSaleorder.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }
    #endregion Load

    #region Event
    private void viewPLN_10_007_Load(object sender, EventArgs e)
    {
      this.LoadComboboxItemCode();
      this.LoadComboboxWo();
      this.LoadComboboxSaleorderdetail();
      DataTable dt = new DataTable();

      dt.Columns.Add("Pid", typeof(Int64));
      dt.Columns.Add("ItemCode", typeof(string));
      dt.Columns.Add("SaleNo", typeof(string));
      dt.Columns.Add("PONo", typeof(string));
      dt.Columns.Add("Revision", typeof(Int32));
      dt.Columns.Add("Wo", typeof(Int64));
      dt.Columns.Add("SOQty", typeof(Int64));
      dt.Columns.Add("Qty", typeof(Int32));
      dt.Columns.Add("Shipped", typeof(Int32));
      dt.Columns.Add("Note", typeof(string));
      dt.Columns.Add("IsSubCon", typeof(Int32));
      dt.Columns.Add("Selected", typeof(Int32));
      dt.Columns.Add("Priority", typeof(Int32));
      dt.Columns.Add("PrioritySO", typeof(Int32));
      dt.Columns.Add("WOType", typeof(Int32));
      dt.Columns.Add("Lock", typeof(Int32));

      ultSaleorderdetail.DataSource = dt;
      this.LoadDropDownItemCode();
      this.LoadDropDownWo();
      this.LoadDropDownSaleorderdetail();
      if (value == true)
      {
        this.txtRevision.Text = Revision.ToString();
        //this.ultcbmItemcode.Text = Itemcode.ToString();
        this.ultcbmSaleorderdetail.Value = Saleorderpid;
        this.btnSearch_Click(sender, e);
      }
    }
    /// <summary>
    /// Search
    /// </summary>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultSaleorderdetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultSaleorderdetail);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Hidden = true;
      e.Layout.Bands[0].Columns["WOType"].Hidden = true;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultDrItemCode;
      e.Layout.Bands[0].Columns["Wo"].ValueList = ultDrWo;
      e.Layout.Bands[0].Columns["SaleNo"].ValueList = ultDrSaleorder;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SOQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Shipped"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Lock"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Lock"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Priority"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["PrioritySO"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["SaleNo"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Wo"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Note"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["SOQty"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Shipped"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["PrioritySO"].Header.Caption = "Priority SO For Container";

      //Sum Qty
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      for (int i = 0; i < this.ultSaleorderdetail.Rows.Count; i++)
      {
        UltraGridRow row = this.ultSaleorderdetail.Rows[i];
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
        {
          row.Cells["SaleNo"].Activation = Activation.ActivateOnly;
          row.Cells["ItemCode"].Activation = Activation.ActivateOnly;
          row.Cells["Wo"].Activation = Activation.ActivateOnly;
        }
        else
        {
          row.Cells["SaleNo"].Activation = Activation.AllowEdit;
          row.Cells["ItemCode"].Activation = Activation.AllowEdit;
          row.Cells["Wo"].Activation = Activation.AllowEdit;
        }
        if (DBConvert.ParseInt(row.Cells["WOType"].Value.ToString()) > 0)
        {
          row.Cells["IsSubCon"].Activation = Activation.ActivateOnly;
        }
        else
        {
          row.Cells["IsSubCon"].Activation = Activation.AllowEdit;
        }
      }

      // Check btnSave Cho Phep Delete     
      //if(btnSave.Visible == false)
      //{
      //  e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      //}     
      //e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      //Thinh Update (Reques Wo link SO) khong cho delete tren luoi
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
    }

    private DataTable CreateDataCheck()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("SODetailPid", typeof(Int64));
      dt.Columns.Add("ItemCode", typeof(string));
      dt.Columns.Add("Revision", typeof(Int32));
      dt.Columns.Add("WO", typeof(Int32));
      dt.Columns.Add("Qty", typeof(Int32));
      dt.Columns.Add("IsSubCon", typeof(Int32));
      return dt;
    }
    private void FillDataForGrid(DataTable dt)
    {
      SqlDBParameter[] inputparam = new SqlDBParameter[1];
      string storeName = "spPLNListOfSwapWOSO_Import";
      inputparam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
      DataTable dtSource = SqlDataBaseAccess.SearchStoreProcedureDataTable(storeName, 900, inputparam);
      viewPLN_10_022 view = new viewPLN_10_022();
      view.dtSource = dtSource;
      WindowUtinity.ShowView(view, "LIST OF SWAP WO", true, ViewState.MainWindow);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string Out = string.Empty;
      string listPid = string.Empty;
      DataTable dtDataCheck = this.CreateDataCheck();
      DataTable dt = (DataTable)ultSaleorderdetail.DataSource;
      if (dt.Rows.Count > 0)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i].RowState == DataRowState.Added || dt.Rows[i].RowState == DataRowState.Modified)
          {
            if (DBConvert.ParseLong(dt.Rows[i]["Wo"].ToString()) < 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("WRN0013", "Workorder");
              return;
            }

            if (DBConvert.ParseLong(dt.Rows[i]["SaleNo"].ToString()) < 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "SaleOrder");
              return;
            }

            if (DBConvert.ParseDouble(dt.Rows[i]["Qty"].ToString()) < 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0036", "");
              return;
            }

            if (dt.Rows[i]["PrioritySO"].ToString().Length > 0 && DBConvert.ParseInt(dt.Rows[i]["PrioritySO"].ToString()) <= 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Priority SO For Container");
              return;
            }

            listPid += ";" + dt.Rows[i]["Pid"].ToString();
            if (DBConvert.ParseInt(dt.Rows[i]["Wo"].ToString()) != 1)
            {
              DataRow rowCheck = dtDataCheck.NewRow();
              rowCheck["SODetailPid"] = DBConvert.ParseLong(dt.Rows[i]["SaleNo"].ToString());
              rowCheck["ItemCode"] = dt.Rows[i]["ItemCode"].ToString();
              rowCheck["Revision"] = DBConvert.ParseInt(dt.Rows[i]["Revision"].ToString());
              rowCheck["WO"] = DBConvert.ParseInt(dt.Rows[i]["Wo"].ToString());
              rowCheck["Qty"] = DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString());
              rowCheck["IsSubCon"] = DBConvert.ParseInt(dt.Rows[i]["IsSubCon"].ToString());
              dtDataCheck.Rows.Add(rowCheck);
            }
          }
        }

        // Check Valid
        DataSet dsResult = GetDataTableImport(dtDataCheck, listPid);
        if (dsResult == null)
        {
          return;
        }

        if (dsResult.Tables[0].Rows.Count > 0)
        {
          WindowUtinity.ShowMessageErrorFromText("Tồn tại WO, Item, Revision vừa Check SubCon và không Subcon");
          return;
        }

        if (dsResult.Tables[1].Rows.Count > 0)
        {
          WindowUtinity.ShowMessageErrorFromText("Số luợng WO-SO khác số luợng Confirm với sản xuất");
          return;
        }
        // Check Valid

        for (int i = 0; i < dt.Rows.Count; i++)
        {
          string storeName = string.Empty;
          DBParameter[] inputParam = new DBParameter[12];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          if (dt.Rows[i].RowState != DataRowState.Deleted)
          {
            if (dt.Rows[i].RowState == DataRowState.Added)
            {
              storeName = "spWoLinkSoInformation_Insert";
              inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            }
            else if (dt.Rows[i].RowState == DataRowState.Modified)
            {
              storeName = "spWoLinkSoInformation_Update";
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()));
              inputParam[8] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            }
            if (storeName.Length > 0)
            {
              string itemCode = dt.Rows[i]["ItemCode"].ToString();
              inputParam[1] = new DBParameter("@ItemCode", DbType.String, itemCode);
              int revision = DBConvert.ParseInt(dt.Rows[i]["Revision"].ToString());
              inputParam[2] = new DBParameter("@Revision", DbType.Int32, revision);
              long SaleOrderDetailPid = DBConvert.ParseLong(dt.Rows[i]["SaleNo"].ToString());
              inputParam[3] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, SaleOrderDetailPid);
              int qty = DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString());
              inputParam[4] = new DBParameter("@Qty", DbType.Int32, qty);
              long Wo = DBConvert.ParseLong(dt.Rows[i]["Wo"].ToString());
              inputParam[5] = new DBParameter("@Wo", DbType.Int64, Wo);
              int Issubcon = DBConvert.ParseInt(dt.Rows[i]["IsSubCon"].ToString());
              if (Issubcon > int.MinValue)
              {
                inputParam[6] = new DBParameter("@Isubcon", DbType.Int32, Issubcon);
              }

              string note = dt.Rows[i]["Note"].ToString();
              if (note.Length > 0)
              {
                inputParam[9] = new DBParameter("@Note", DbType.String, note);
              }
              int Priority = DBConvert.ParseInt(dt.Rows[i]["Priority"].ToString());
              if (Priority > int.MinValue)
              {
                inputParam[10] = new DBParameter("@Priority", DbType.Int32, Priority);
              }
              // Priority SO
              int prioritySO = DBConvert.ParseInt(dt.Rows[i]["PrioritySO"].ToString());
              if (prioritySO > 0)
              {
                inputParam[11] = new DBParameter("@PrioritySO", DbType.Int32, prioritySO);
              }
              Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
              long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
              Out += "," + outValue.ToString();
              if (outValue == -1)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0050", "Unrelease < Qty");
                ultSaleorderdetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
                return;
              }
              if (outValue == -2)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0029", "ItemCode  " + itemCode + " Revision  " + revision.ToString());
                ultSaleorderdetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
                return;
              }
              else
              {
                dt.Rows[i]["Pid"] = outValue.ToString();
              }
            }
          }
        }
        DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerPriorityRefeshData_Insert");
      }

      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.Search();
    }

    /// <summary>
    /// Get Data Import Excel
    /// </summary>
    /// <param name="dtImport"></param>
    /// <returns></returns>
    private DataSet GetDataTableImport(DataTable dtImport, string listPid)
    {
      SqlDBParameter[] input = new SqlDBParameter[2];
      input[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtImport);
      input[1] = new SqlDBParameter("@listPid", SqlDbType.Text, listPid);
      DataSet dsMain = SqlDataBaseAccess.SearchStoreProcedure("spPLNAdjustWOSOCheck_Select", input);
      return dsMain;
    }

    private void ultSaleorderdetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      // Check Locked
      foreach (UltraGridRow row in e.Rows)
      {
        if (DBConvert.ParseInt(row.Cells["Lock"].Value) == 1)
        {
          WindowUtinity.ShowMessageError("ERR0093", row.Cells["SaleNo"].Text + " - " + row.Cells["ItemCode"].Value + " - " + row.Cells["Revision"].Value);
          e.Cancel = true;
          return;
        }
      }
      // End Check Locked

      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          string storeNamedelete = "spWoLinkSoInformation_Delete";
          DBParameter[] inputParamdelete = new DBParameter[1];
          inputParamdelete[0] = new DBParameter("@Pid", DbType.Int64, pid);
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeNamedelete, inputParamdelete);
        }
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.txtRevision.Text = string.Empty;
      //this.ultcbmItemcode.Text = string.Empty;
      this.ultcbmSaleorderdetail.Text = string.Empty;
      this.ultcbmWo.Text = string.Empty;
      this.chkIsSubcon.Checked = false;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
      this.ConfirmToCloseTab();
    }

    private void ultSaleorderdetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();

      switch (columnName)
      {
        case "itemcode":
          e.Cell.Row.Cells["SaleNo"].Value = DBNull.Value;
          if (ultDrItemCode.SelectedRow != null)
          {
            ultSaleorderdetail.Rows[index].Cells["Revision"].Value = DBConvert.ParseInt(ultDrItemCode.SelectedRow.Cells["Revision"].Value.ToString());
          }
          break;
      }
    }

    private void ultSaleorderdetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.Cell.Text;
      if (string.Compare(colName, "Qty", true) == 0)
      {
        int values = DBConvert.ParseInt(e.Cell.Text.Trim());
        if (values < 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0110", new string[] { "Qty" });
          e.Cancel = true;
        }
      }

      if (string.Compare(colName, "Qty", true) == 0)
      {
        int qty = DBConvert.ParseInt(e.Cell.Text.Trim());
        int soQty = DBConvert.ParseInt(e.Cell.Row.Cells["SOQty"].Value.ToString());
        int shippedQty = DBConvert.ParseInt(e.Cell.Row.Cells["Shipped"].Value.ToString());

        if (DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) != long.MinValue)
        {
          if (qty > soQty)
          {
            Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Open WO <= Total Balance on SO");
            e.Cancel = true;
          }

          if (qty < shippedQty)
          {
            Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Open WO >= Shipped Qty");
            e.Cancel = true;
          }
        }
      }

      if (string.Compare(colName, "Wo", true) == 0)
      {
        if (value.Trim().Length > 0)
        {
          string commandText = "SELECT Pid  FROM TblPLNWorkOrder WHERE Pid = " + DBConvert.ParseInt(value) + "";
          DataTable dtCheckWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheckWO == null || dtCheckWO.Rows.Count == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "WO");
            e.Cancel = true;
          }
        }
      }
    }

    private void ultSaleorderdetail_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      string itemCode = row.Cells["ItemCode"].Value.ToString();
      int revision = DBConvert.ParseInt(row.Cells["revision"].Value.ToString());

      switch (columnName)
      {
        case "SaleNo":
          if (DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) <= 0)
          {
            string commandText = string.Empty;
            commandText = " SELECT SaleOrderDetailPid Pid , SaleNo, PONo, CONVERT(VARCHAR,ScheduleDelivery,103) ScheduleDelivery, ItemCode, Revision, Remain, Balance ";
            commandText += " FROM VPLNMasterPlan";
            commandText += " WHERE ItemCode = '" + itemCode + "' AND Revision = " + revision + " And Balance > 0";
            commandText += " ORDER BY SaleNo, ItemCode, CONVERT(VARCHAR,ScheduleDelivery,103) DESC";
            DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtSource != null)
            {
              ultraDrSaleOrderActive.DataSource = dtSource;
              ultraDrSaleOrderActive.DisplayMember = "SaleNo";
              ultraDrSaleOrderActive.ValueMember = "Pid";
              // Format Grid
              ultraDrSaleOrderActive.DisplayLayout.Bands[0].ColHeadersVisible = true;
              ultraDrSaleOrderActive.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
              ultSaleorderdetail.Rows[e.Cell.Row.Index].Cells["SaleNo"].ValueList = ultraDrSaleOrderActive;
            }
          }
          break;
        default:
          break;
      }
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

    private void ultWO_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultSaleorderdetail_DoubleClick(object sender, EventArgs e)
    {
      try
      {
        UltraGridRow row = (ultSaleorderdetail.Selected.Rows[0].ParentRow == null) ? ultSaleorderdetail.Selected.Rows[0] : ultSaleorderdetail.Selected.Rows[0].ParentRow;
        if (DBConvert.ParseInt(row.Cells["Lock"].Value) == 0)
        {
          viewPLN_02_011 view = new viewPLN_02_011();
          view.WoDetail = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          view.Priority = DBConvert.ParseInt(row.Cells["Priority"].Value.ToString());
          view.WorkOrderPID = DBConvert.ParseLong(row.Cells["Wo"].Value.ToString());
          if (DBConvert.ParseInt(row.Cells["IsSubCon"].Value.ToString()) == 1)
          {
            view.isSubCon = 1;
          }
          else
          {
            view.isSubCon = 0;
          }
          Shared.Utility.WindowUtinity.ShowView(view, "SWAP WORK ORDER", false, Shared.Utility.ViewState.ModalWindow);
          this.Search();
        }
      }
      catch
      { }
    }

    private void btnBrownser_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImportExcel_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcel.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Items (1)$E3:E4]");
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
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), string.Format("SELECT * FROM [Items (1)$B5:H{0}]", itemCount));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.FillDataForGrid(dsItemList.Tables[0]);
    }

    private void btnGetTemp_Click(object sender, EventArgs e)
    {
      string templateName = "ListOfSwap";
      string sheetName = "Items";
      string outFileName = "List Of Swap";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void ucUltraListItem_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtItemCode.Text = ucUltraListItem.SelectedValue;
    }

    private void chkShowItemListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListItem.Visible = chkShowItemListBox.Checked;
    }
  }
  #endregion Event
}
