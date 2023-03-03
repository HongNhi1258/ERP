/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : Receipt voucher
  Standard Form: view_SaveMasterDetail
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
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_11_001 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = ConstantClass.Receipt_Voucher;
    public int actionCode = int.MinValue;
    private int currency = int.MinValue;
    private int objectType = int.MinValue;
    public string listPaymentRefundPid = string.Empty;
    private bool isLoadedPostTransaction = false;
    #endregion Field

    #region Init
    public viewACC_11_001()
    {
      InitializeComponent();
    }

    private void viewACC_11_001_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(ugbInformation);
      this.SetBlankForTextOfButton(this);
      this.InitData();
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
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCReceiptVoucher_Init", inputParam);
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[0], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });
      Utility.LoadUltraCombo(ucbCashFund, dsInit.Tables[1], "Value", "Display", false, new string[] { "Value", "AccountPid" });
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[2], "ObjectPid", "Object", false, new string[] { "ObjectPid", "ObjectType" });
      Utility.LoadUltraCombo(ucbProject, dsInit.Tables[3], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbSegment, dsInit.Tables[4], "Value", "Display", false, "Value");

      //Init dropdown
      Utility.LoadUltraCBAccountList(ucbddDebit);      
      Utility.LoadUltraCBAccountList(ucbddCredit);      

      this.dtObject = dsInit.Tables[5];
      udtReceiptDate.Value = DateTime.Now;
      // Set Language
      //this.SetLanguage();
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtReceiptCode.ReadOnly = true;
        udtReceiptDate.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        txtReceiptDesc.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtReceiptCode.Text = dtMain.Rows[0]["ReceiptCode"].ToString();
        udtReceiptDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["ReceiptDate"]);
        this.actionCode = DBConvert.ParseInt(dtMain.Rows[0]["ActionCode"]);
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        uneExchangeRate.Value = dtMain.Rows[0]["ExchangeRate"];
        if (DBConvert.ParseInt(dtMain.Rows[0]["CashFundPid"]) != int.MinValue)
        {
          ucbCashFund.Value = DBConvert.ParseInt(dtMain.Rows[0]["CashFundPid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["ProjectPid"]) != int.MinValue)
        {
          ucbSegment.Value = DBConvert.ParseInt(dtMain.Rows[0]["ProjectPid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["SegmentPid"]) != int.MinValue)
        {
          ucbSegment.Value = DBConvert.ParseInt(dtMain.Rows[0]["SegmentPid"]);
        }
        this.objectType = DBConvert.ParseInt(dtMain.Rows[0]["ObjectType"]);
        ucbObject.Value = DBConvert.ParseInt(dtMain.Rows[0]["ObjectPid"]);
        txtSenderName.Text = dtMain.Rows[0]["SenderName"].ToString();
        txtReceiptDesc.Text = dtMain.Rows[0]["ReceiptDesc"].ToString();
        lbStatusDes.Text = (bool)dtMain.Rows[0]["Status"] ? "Đã xác nhận" : "Tạo mới";
        chkConfirm.Checked = (bool)dtMain.Rows[0]["Status"];
        this.status = (bool)dtMain.Rows[0]["Status"] ? 1 : 0;
      }
      else
      {
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@NewDocCode", DbType.String, 32, string.Empty) };
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@DocTypePid", DbType.Int32, this.docTypePid);
        inputParam[1] = new DBParameter("@DocDate", DbType.Date, DateTime.Now.Date);

        DataBaseAccess.SearchStoreProcedure("spACCGetNewDocCode", inputParam, outputParam);
        if (outputParam[0].Value.ToString().Length > 0)
        {
          txtReceiptCode.Text = outputParam[0].Value.ToString();
          udtReceiptDate.Value = DateTime.Now;
        }
      }
    }

    /// <summary>
    /// Load tab data
    /// </summary>
    private void LoadTabData()
    {
      // Load Tab Data Component
      string tabPageName = utcDetail.SelectedTab.TabPage.Name;
      switch (tabPageName)
      {
        case "utpcListReceiptVoucher":
          //this.LoadData();
          break;
        case "utpcPostReceiptVoucher":
          if (!isLoadedPostTransaction)
          {
            if (chkConfirm.Checked)
            {
              this.LoadTransationData();
              this.isLoadedPostTransaction = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Load transaction
    /// </summary>
    private void LoadTransationData()
    {
      ugdTransaction.SetDataSource(this.docTypePid, this.viewPid);
    }

    private void LoadPaymentReceiptList()
    {
      DBParameter[] inputParamMain = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataTable dtNoInvoice = DataBaseAccess.SearchStoreProcedureDataTable("spACCPaymentReceiptDetail_Load", inputParamMain);
      ugdData.DataSource = dtNoInvoice;
    }
    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      if (this.viewPid != long.MinValue)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCPaymentReceipt_Load", inputParam);
        if (dsSource != null && dsSource.Tables.Count > 0)
        {
          this.LoadMainData(dsSource.Tables[0]);
          this.LoadPaymentReceiptList();
          lbCount.Text = string.Format(@"Đếm {0}", ugdData.Rows.FilteredInRowCount > 0 ? ugdData.Rows.FilteredInRowCount : 0);
        }
      }
      else
      {
        if(this.actionCode == 1)
        {
          this.LoadPaymentReceiptList();
        } 
        else if (this.actionCode == 2)
        {
          viewACC_11_003 view = new viewACC_11_003();
          Shared.Utility.WindowUtinity.ShowView(view, "Chọn phiếu đề nghị hoàn ứng", false, ViewState.ModalWindow, FormWindowState.Normal);
          this.listPaymentRefundPid = view.listPaymentRefundPid;
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@ListPaymentRefundPid", DbType.String, this.listPaymentRefundPid);
          inputParam[1] = new DBParameter("@DocTypePid", DbType.Int32, this.docTypePid);
          inputParam[2] = new DBParameter("@ActionCode", DbType.Int32, this.actionCode);
          DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCPaymentReceiptForPaymentRefundList_Load", inputParam);
          if (dsSource != null && dsSource.Tables.Count > 0)
          {
            this.LoadMainData(dsSource.Tables[0]);
            ugdData.DataSource = dsSource.Tables[1];           
            lbCount.Text = string.Format(@"Đếm {0}", ugdData.Rows.FilteredInRowCount > 0 ? ugdData.Rows.FilteredInRowCount : 0);
          }
        }
      }
      this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      //check master
      if (udtReceiptDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtReceiptDate.Focus();
        return false;
      }
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Loại tiền tệ không được để trống!!!");
        ucbCurrency.Focus();
        return false;
      }
      if (ucbCashFund.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Quỹ tiền mặt không được để trống!!!");
        ucbCashFund.Focus();
        return false;
      }
      if (uneExchangeRate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Tỉ giá không được để trống!!!");
        uneExchangeRate.Focus();
        return false;
      }

      //check detail

      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        UltraGridRow row = ugdData.Rows[i];
        row.Selected = false;
        if (DBConvert.ParseDouble(row.Cells["Amount"].Value) <= 0)
        {
          row.Cells["Amount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tiền hàng không được để trống và phải lớn hơn 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseInt(row.Cells["DebitPid"].Value) < 0)
        {
          row.Cells["DebitPid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("TK nợ không được để trống!!!");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseInt(row.Cells["CreditPid"].Value) < 0)
        {
          row.Cells["CreditPid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("TK có không được để trống!!!");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
      }
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[15];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtReceiptDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@ReceiptDesc", SqlDbType.NVarChar, txtReceiptDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@ReceiptDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtReceiptDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      if (txtSenderName.Text.Trim().Length > 0)
      {
        inputParam[3] = new SqlDBParameter("@SenderName", SqlDbType.NVarChar, txtSenderName.Text.Trim().ToString());
      }
      if (ucbSegment.SelectedRow != null)
      {
        inputParam[4] = new SqlDBParameter("@SegmentPid", SqlDbType.Int, DBConvert.ParseInt(ucbSegment.Value));
      }
      if (ucbCashFund.SelectedRow != null)
      {
        inputParam[5] = new SqlDBParameter("@CashFundPid", SqlDbType.Int, DBConvert.ParseInt(ucbCashFund.Value));
      }
      if (ucbProject.SelectedRow != null)
      {
        inputParam[6] = new SqlDBParameter("@ProjectPid", SqlDbType.Int, DBConvert.ParseInt(ucbProject.Value));
      }
      inputParam[7] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[8] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, uneExchangeRate.Value);
      inputParam[9] = new SqlDBParameter("@ObjectType", SqlDbType.Int, DBConvert.ParseInt(this.objectType));
      if (ucbObject.SelectedRow != null)
      {
        inputParam[10] = new SqlDBParameter("@Object", SqlDbType.Int, DBConvert.ParseInt(ucbObject.Value));
      }
      inputParam[11] = new SqlDBParameter("@ActionCode", SqlDbType.Int, this.actionCode);
      inputParam[12] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[13] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[14] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCReceiptVoucherMaster_Save", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }
      return false;
    }

    private bool SaveDetail()
    {
      bool success = true;
      // 1. Delete      
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        SqlDBParameter[] deleteParam = new SqlDBParameter[] { new SqlDBParameter("@Pid", SqlDbType.BigInt, listDeletedPid[i]) };
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCReceiptVoucherDetail_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugdData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {

        SqlDBParameter[] inputParam = new SqlDBParameter[8];
        long pid = DBConvert.ParseLong(row["Pid"].ToString());
        if (pid > 0) // Update
        {
          inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
        }
        inputParam[1] = new SqlDBParameter("@ReceiptVoucherPid", SqlDbType.BigInt, this.viewPid);

        if (row["DetailDesc"].ToString().Trim().Length > 0)
        {
          inputParam[2] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString());
        }
        inputParam[3] = new SqlDBParameter("@DebitPid", SqlDbType.Int, DBConvert.ParseInt(row["DebitPid"].ToString()));
        inputParam[4] = new SqlDBParameter("@CreditPid", SqlDbType.Int, DBConvert.ParseInt(row["CreditPid"].ToString()));
        inputParam[5] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row["Amount"].ToString()));
        if (row["ReferenceNo"].ToString().Trim().Length > 0)
        {
          inputParam[6] = new SqlDBParameter("@ReferenceNo", SqlDbType.VarChar, row["ReferenceNo"].ToString());
        }
        inputParam[7] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCReceiptVoucherDetail_Save", inputParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }

      return success;
    }

    private void SaveData()
    {
      if (this.CheckValid())
      {
        bool success = true;
        if (this.SaveMain())
        {
          if (this.SaveDetail())
          {
            if (chkConfirm.Checked)
            {
              bool isPosted = chkConfirm.Checked;
              success = Utility.ACCPostTransaction(this.docTypePid, viewPid, isPosted, SharedObject.UserInfo.UserPid);
            }
            else
            {
              success = true;
            }

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
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    /// <summary>
    /// Set Auto Add 4 blank before text of button
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetBlankForTextOfButton(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count > 0)
        {
          this.SetBlankForTextOfButton(ctr);
        }
        else if (ctr.GetType().Name == "Button")
        {
          ctr.Text = string.Format("{0}{1}", "    ", ctr.Text);
        }
      }
    }

    private void SetLanguage()
    {
      btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }


    private void TotalAmount()
    {
      double total = 0;
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(ugdData.Rows[i].Cells["TotalAmount"].Value) != Double.MinValue)
        {
          total += DBConvert.ParseDouble(ugdData.Rows[i].Cells["TotalAmount"].Value);
        }
      }
    }
    #endregion Function

    #region Event
    private void ugdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdData);                  
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Allow update, delete, add new
      if (this.status == 0)
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnTop;
      }
      else
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["DebitPid"].Header.Caption = "TK nợ";
      e.Layout.Bands[0].Columns["CreditPid"].Header.Caption = "TK có";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Tiền hàng";
      e.Layout.Bands[0].Columns["ExchangeAmount"].Header.Caption = "Tiền quy đổi";
      e.Layout.Bands[0].Columns["ReferenceNo"].Header.Caption = "Số tham chiếu";

      e.Layout.Bands[0].Columns["DetailDesc"].Width = 250;
      e.Layout.Bands[0].Columns["Amount"].Width = 150;
      e.Layout.Bands[0].Columns["ExchangeAmount"].Width = 150;

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["DebitPid"].ValueList = ucbddDebit;
      e.Layout.Bands[0].Columns["CreditPid"].ValueList = ucbddCredit;

      //set align
      e.Layout.Bands[0].Columns["DebitPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CreditPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["DebitPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DebitPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CreditPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CreditPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["ExchangeAmount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";      
    }

    private void ugdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      switch (columnName)
      {
        case "amount":
          try
          {
            if (DBConvert.ParseDouble(value) >= 0)
            {
              if (DBConvert.ParseInt(ucbCurrency.Value) == 1)
              {
                e.Cell.Row.Cells["ExchangeAmount"].Value = Math.Round(DBConvert.ParseDouble(value) * DBConvert.ParseDouble(uneExchangeRate.Value), 0, MidpointRounding.AwayFromZero);
              }
              else
              {
                e.Cell.Row.Cells["ExchangeAmount"].Value = Math.Round(DBConvert.ParseDouble(value) * DBConvert.ParseDouble(uneExchangeRate.Value), 2, MidpointRounding.AwayFromZero);
              }

            }
          }
          catch
          {
            e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
            e.Cell.Row.Cells["ExchangeAmount"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    private void ugdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "Amount":
          if (DBConvert.ParseDouble(value) < 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Số tiền không được nhỏ hơn 0.");
            e.Cancel = true;
          }
          break;
        case "DebitPid":
          if (ucbddDebit.SelectedRow != null)
          {
            int isLeaf = DBConvert.ParseInt(ucbddDebit.SelectedRow.Cells["IsLeaf"].Value);
            if (isLeaf == 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Bạn không thể hạch toán vào tk cha.");
              e.Cancel = true;
            }
          }
          break;
        case "CreditPid":
          if (ucbddCredit.SelectedRow != null)
          {
            int isLeaf = DBConvert.ParseInt(ucbddCredit.SelectedRow.Cells["IsLeaf"].Value);
            if (isLeaf == 0)
            {
              WindowUtinity.ShowMessageErrorFromText("Bạn không thể hạch toán vào tk cha.");
              e.Cancel = true;
            }
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

    private void ugdData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdData, "Chi tiết phiếu ĐNTTTT");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdData);
      }
    }

    private void ugdData_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdData.Selected.Rows.Count > 0 || ugdData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdData, new Point(e.X, e.Y));
        }
      }
    }


    private void ucbCurrency_ValueChanged(object sender, EventArgs e)
    {
      if (ucbCurrency.SelectedRow != null)
      {
        this.currency = DBConvert.ParseInt(ucbCurrency.Value);
        uneExchangeRate.Value = ucbCurrency.SelectedRow.Cells["ExchangeRate"].Value;
        if (DBConvert.ParseInt(ucbCurrency.SelectedRow.Cells["Pid"].Value) == 1)
        {
          uneExchangeRate.ReadOnly = true;
        }
        else
        {
          uneExchangeRate.ReadOnly = false;
        }
      }
    }




    private void ucbObject_ValueChanged(object sender, EventArgs e)
    {
      if (ucbObject.SelectedRow != null)
      {
        this.objectType = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectType"].Value);
        txtSenderName.Text = ucbObject.SelectedRow.Cells["ObjectName"].Value.ToString();
      }
    }


    private void ugdData_AfterRowInsert(object sender, RowEventArgs e)
    {
      if (ucbCashFund.SelectedRow != null)
      {
        e.Row.Cells["DebitPid"].Value = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
      }
      e.Row.Cells["Amount"].Value = 0;
      e.Row.Cells["ExchangeAmount"].Value = 0; 
    }

    private void ucbCashFund_ValueChanged(object sender, EventArgs e)
    {
      if (ucbCashFund.SelectedRow != null && ugdData.Rows.Count > 0)
      {
        for (int i = 0; i < ugdData.Rows.Count; i++)
        {
          ugdData.Rows[i].Cells["DebitPid"].Value = DBConvert.ParseInt(ucbCashFund.SelectedRow.Cells["AccountPid"].Value);
        }
      }
    }

    private void utcDetail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      this.LoadTabData();
    }

    private void ugdData_AfterRowsDeleted(object sender, EventArgs e)
    {
      
    }

    #endregion Event
  }
}