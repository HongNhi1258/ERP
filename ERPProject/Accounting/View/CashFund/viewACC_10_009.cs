/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : payment voucher clear
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
  public partial class viewACC_10_009 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listAdvanceDeletedPid = new ArrayList();
    private IList listNoInvoiceDeletedPid = new ArrayList();
    private IList listInvoiceDeletedPid = new ArrayList();
    private int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = ConstantClass.Payment_Refund;
    public int actionCode = int.MinValue;
    private int currency = int.MinValue;
    private int objectType = int.MinValue;
    #endregion Field

    #region Init
    public viewACC_10_009()
    {
      InitializeComponent();
    }

    private void viewACC_10_009_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(ugbInformation);
      this.SetBlankForTextOfButton(this);
      if (this.actionCode == 1)
      {
        btnAdd.Visible = false;
      }
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCPaymentRefund_Init", inputParam);
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[0], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "ObjectPid", "Object", false, new string[] { "ObjectPid", "ObjectType" });
      Utility.LoadUltraCombo(ucbKind, dsInit.Tables[2], "Value", "Display", false, "Value");

      //Init dropdown
      Utility.LoadUltraCombo(ucbddDebit, dsInit.Tables[3], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbddObject, dsInit.Tables[4], "ObjectPid", "Object", false, new string[] { "ObjectPid", "ObjectType" });
      //Utility.LoadUltraCombo(ucbddInputVATDocType, dsInit.Tables[7], "Value", "Display", false, "Value");
      //Utility.LoadUltraCombo(ucbddProjectPid, dsInit.Tables[8], "Value", "Display", false, "Value");
      //Utility.LoadUltraCombo(ucbddSegmentPid, dsInit.Tables[9], "Value", "Display", false, "Value");
      //Utility.LoadUltraCombo(ucbddUnCostConstructionPid, dsInit.Tables[10], "Value", "Display", false, "Value");
      //Utility.LoadUltraCombo(ucbddCostObjectPid, dsInit.Tables[11], "Value", "Display", false, "Value");
      //Utility.LoadUltraCombo(ucbddEInvoiceTypePid, dsInit.Tables[12], "Value", "Display", false, "Value");
      //Utility.LoadUltraCombo(ucbddCostCenterPid, dsInit.Tables[13], "Value", "Display", false, "Value");

      //this.dtObject = dsInit.Tables[5];
      // Set Language
      //this.SetLanguage();
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtPaymentCode.ReadOnly = true;
        udtPaymentDate.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        txtPaymentDesc.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
        ucbAdvanceCode.ReadOnly = true;
      }
    }

    private void LoadMainData()
    {
      DBParameter[] inputParamMain = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spACCPaymentRefund_Load", inputParamMain);
      if (dtMain.Rows.Count > 0)
      {
        txtPaymentCode.Text = dtMain.Rows[0]["RequestCode"].ToString();
        udtPaymentDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["RequestDate"]);
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        uneExchangeRate.Value = dtMain.Rows[0]["ExchangeRate"];
        ucbKind.Value = DBConvert.ParseInt(dtMain.Rows[0]["PaymentType"]);
        ucbObject.Value = DBConvert.ParseInt(dtMain.Rows[0]["ObjectPid"]);
        txtPaymentDesc.Text = dtMain.Rows[0]["RequestDesc"].ToString();
        uneTotalAmount.Value = dtMain.Rows[0]["TotalAmount"];
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
          txtPaymentCode.Text = outputParam[0].Value.ToString();
          udtPaymentDate.Value = DateTime.Now;
        }
      }
    }


    private void LoadPaymentAdvanceList()
    {
      DBParameter[] inputParamMain = new DBParameter[] { new DBParameter("@RequestPid", DbType.Int64, this.viewPid) };
      DataTable dtAdvanceList = DataBaseAccess.SearchStoreProcedureDataTable("spACCPaymentRefundAdvanceList_Load", inputParamMain);
      ugdPaymentAdvance.DataSource = dtAdvanceList;
    }

    private void LoadPaymentNoInputInvoiceList()
    {
      DBParameter[] inputParamMain = new DBParameter[] { new DBParameter("@RequestPid", DbType.Int64, this.viewPid) };
      DataTable dtNoInvoice = DataBaseAccess.SearchStoreProcedureDataTable("spACCPaymentRefundNoInputInvoice_Load", inputParamMain);
      ugdDataNoInputInvoice.DataSource = dtNoInvoice;
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
        case "utpcPaymentAdvance":
          //this.LoadData();
          break;
        case "utpcPostPaymentVoucher":

          break;
        default:
          break;
      }
    }


    private void LoadData()
    {
      this.listAdvanceDeletedPid = new ArrayList();
      this.listInvoiceDeletedPid = new ArrayList();
      this.listNoInvoiceDeletedPid = new ArrayList();
      this.LoadMainData();
      this.LoadPaymentAdvanceList();
      this.LoadPaymentNoInputInvoiceList();
      this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      //check master
      if (udtPaymentDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtPaymentDate.Focus();
        return false;
      }
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Loại tiền tệ không được để trống!!!");
        ucbCurrency.Focus();
        return false;
      }

      if (uneExchangeRate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Tỉ giá không được để trống!!!");
        uneExchangeRate.Focus();
        return false;
      }

      //check detail

      for (int i = 0; i < ugdDataNoInputInvoice.Rows.Count; i++)
      {
        UltraGridRow row = ugdDataNoInputInvoice.Rows[i];
        row.Selected = false;
        if (row.Cells["Amount"].Value.ToString().Trim().Length == 0)
        {
          row.Cells["Amount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tiền hàng không được để trống");
          row.Selected = true;
          ugdDataNoInputInvoice.ActiveRowScrollRegion.FirstRow = row;
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
      inputParam[1] = new SqlDBParameter("@RequestDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtPaymentDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[2] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[3] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[4] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, uneExchangeRate.Value);
      if (ucbKind.SelectedRow != null)
      {
        inputParam[5] = new SqlDBParameter("@PaymentType", SqlDbType.Int, DBConvert.ParseInt(ucbKind.Value));
      }
      if (ucbObject.SelectedRow != null)
      {
        inputParam[6] = new SqlDBParameter("@Object", SqlDbType.Int, DBConvert.ParseInt(ucbObject.Value));
      }
      if (txtPaymentDesc.Text.Trim().Length > 0)
      {
        inputParam[7] = new SqlDBParameter("@RequestDesc", SqlDbType.NVarChar, txtPaymentDesc.Text.Trim().ToString());
      }
      inputParam[8] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, uneTotalAmount.Value);
      inputParam[9] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[10] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      inputParam[11] = new SqlDBParameter("@ObjectType", SqlDbType.Int, this.objectType);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentRefundMaster_Save", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }
      return false;
    }

    private bool SaveDetailAdvance()
    {
      bool success = true;
      // 1. Delete      
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      for (int i = 0; i < listAdvanceDeletedPid.Count; i++)
      {
        SqlDBParameter[] deleteParam = new SqlDBParameter[] { new SqlDBParameter("@Pid", SqlDbType.BigInt, listAdvanceDeletedPid[i]) };
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentRefundAdvance_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugdPaymentAdvance.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          SqlDBParameter[] inputParam = new SqlDBParameter[7];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@RequestPid", SqlDbType.BigInt, this.viewPid);
          inputParam[2] = new SqlDBParameter("@AdvanceDetailPid", SqlDbType.Int, DBConvert.ParseInt(row["AdvanceDetailPid"].ToString()));
          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[3] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString());
          }
          inputParam[4] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row["Amount"].ToString()));
          if (row["ReferenceNo"].ToString().Trim().Length > 0)
          {
            inputParam[5] = new SqlDBParameter("@ReferenceNo", SqlDbType.NVarChar, row["ReferenceNo"].ToString());
          }
          inputParam[6] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentRefundAdvance_Save", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveDetailNoInputInvoice()
    {
      bool success = true;
      // 1. Delete      
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      for (int i = 0; i < listNoInvoiceDeletedPid.Count; i++)
      {
        SqlDBParameter[] deleteParam = new SqlDBParameter[] { new SqlDBParameter("@Pid", SqlDbType.BigInt, listNoInvoiceDeletedPid[i]) };
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentRefundNoInvoice_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugdDataNoInputInvoice.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          SqlDBParameter[] inputParam = new SqlDBParameter[14];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@RequestPid", SqlDbType.BigInt, this.viewPid);

          if (row["VATInvoiceNo"].ToString().Trim().Length > 0)
          {
            inputParam[2] = new SqlDBParameter("@VATInvoiceNo", SqlDbType.VarChar, row["VATInvoiceNo"].ToString());
          }
          if (DBConvert.ParseDateTime(row["VATDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            inputParam[3] = new SqlDBParameter("@VATDate", SqlDbType.Date, DBConvert.ParseDateTime(row["VATDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }
          inputParam[4] = new SqlDBParameter("@DebitPid", SqlDbType.Int, DBConvert.ParseInt(row["DebitPid"].ToString()));
          inputParam[5] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row["Amount"].ToString()));
          inputParam[6] = new SqlDBParameter("@VATPercent", SqlDbType.Float, DBConvert.ParseDouble(row["VATPercent"].ToString()));
          inputParam[7] = new SqlDBParameter("@VATAmount", SqlDbType.Float, DBConvert.ParseDouble(row["VATAmount"].ToString()));
          inputParam[8] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, DBConvert.ParseDouble(row["TotalAmount"].ToString()));
          if (DBConvert.ParseInt(row["ObjectPid"].ToString()) >= 0)
          {
            inputParam[9] = new SqlDBParameter("@ObjectPid", SqlDbType.Int, DBConvert.ParseInt(row["ObjectPid"].ToString()));
          }
          if (row["Address"].ToString().Trim().Length > 0)
          {
            inputParam[10] = new SqlDBParameter("@Address", SqlDbType.VarChar, row["Address"].ToString());
          }
          if (row["TaxNumber"].ToString().Trim().Length > 0)
          {
            inputParam[11] = new SqlDBParameter("@TaxNumber", SqlDbType.VarChar, row["TaxNumber"].ToString());
          }
          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[12] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString());
          }
          inputParam[13] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentRefundNoInvoiceDetail_Save", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
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
          if (this.SaveDetailAdvance())
          {
            if (this.SaveDetailNoInputInvoice())
            {
              success = true;
            }
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
      double totalAdvance = 0;
      double totalAmountNoInvoice = 0;
      for (int i = 0; i < ugdPaymentAdvance.Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(ugdPaymentAdvance.Rows[i].Cells["Amount"].Value) != Double.MinValue)
        {
          totalAdvance += DBConvert.ParseDouble(ugdPaymentAdvance.Rows[i].Cells["Amount"].Value);
        }
      }

      for (int i = 0; i < ugdDataNoInputInvoice.Rows.Count; i++)
      {
        if (DBConvert.ParseDouble(ugdDataNoInputInvoice.Rows[i].Cells["TotalAmount"].Value) != Double.MinValue)
        {
          totalAmountNoInvoice += DBConvert.ParseDouble(ugdDataNoInputInvoice.Rows[i].Cells["TotalAmount"].Value);
        }
      }
      if (totalAdvance - totalAmountNoInvoice >= 0)
      {
        ucbKind.Value = 1;
        uneTotalAmount.Value = totalAdvance - totalAmountNoInvoice;
      }
      else
      {
        ucbKind.Value = 2;
        uneTotalAmount.Value = totalAmountNoInvoice - totalAdvance;
      }
    }

    private void LoadDataAdvanceForGrid()
    {
      string cmd = string.Format(@"SELECT AD.AdvanceDetailPid, AD.DetailDesc, AD.AdvanceCode, AD.ObjectPid, VO.SubAmount, VO.PaymentCode
                                          FROM
                                          (
	                                          SELECT DT.Pid AdvanceDetailPid, AD.AdvanceCode, DT.EmployeePid ObjectPid, DT.DetailDesc
	                                          FROM TblACCPaymentAdvance AD
	                                          INNER JOIN TblACCPaymentAdvanceDetail DT ON AD.Pid = DT.AdvancePid
                                          )AD
                                          INNER JOIN
                                          (
	                                          SELECT MS.PaymentCode, DT.PaymentAdvanceDetailPid, DT.SubAmount
	                                          FROM TblACCPaymentVoucher MS
	                                          INNER JOIN TblACCPaymentVoucherDetail DT ON MS.Pid = DT.PaymentVoucherPid
                                          )VO ON AD.AdvanceDetailPid = VO.PaymentAdvanceDetailPid
                                    WHERE AD.AdvanceDetailPid = {0}", DBConvert.ParseInt(ucbAdvanceCode.SelectedRow.Cells["AdvanceDetailPid"].Value));
      DataTable dtAdvance = SqlDataBaseAccess.SearchCommandTextDataTable(cmd);

      DataTable dtAdvanceList = (DataTable)ugdPaymentAdvance.DataSource;
      for (int i = 0; i < dtAdvance.Rows.Count; i++)
      {
        DataRow row = dtAdvance.Rows[i];
        DataRow rowAdd = dtAdvanceList.NewRow();
        rowAdd["AdvanceDetailPid"] = DBConvert.ParseInt(row["AdvanceDetailPid"].ToString());
        rowAdd["DetailDesc"] = row["DetailDesc"].ToString();
        rowAdd["AdvanceCode"] = row["AdvanceCode"].ToString();
        rowAdd["Amount"] = DBConvert.ParseDouble(row["SubAmount"].ToString());
        rowAdd["ReferenceNo"] = row["PaymentCode"].ToString();

        dtAdvanceList.Rows.Add(rowAdd);
      }
      ugdPaymentAdvance.DataSource = dtAdvanceList;
    }
    #endregion Function

    #region Event
    private void ugdDataNoInputInvoice_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdDataNoInputInvoice);
      e.Layout.AutoFitColumns = false;
      e.Layout.UseFixedHeaders = true;
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
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
      e.Layout.Bands[0].Columns["VATInvoiceNo"].Header.Caption = "Số hóa đơn";
      e.Layout.Bands[0].Columns["VATDate"].Header.Caption = "Ngày hóa đơn";
      e.Layout.Bands[0].Columns["DebitPid"].Header.Caption = "Tài khoản nợ";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Tiền hàng";
      e.Layout.Bands[0].Columns["VATPercent"].Header.Caption = "% thuế";
      e.Layout.Bands[0].Columns["VATAmount"].Header.Caption = "Tiền thuế";
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng thanh toán";
      e.Layout.Bands[0].Columns["ObjectPid"].Header.Caption = "Nhà cung cấp";
      e.Layout.Bands[0].Columns["ObjectName"].Header.Caption = "Tên đối tượng";
      e.Layout.Bands[0].Columns["Address"].Header.Caption = "Địa chỉ";
      e.Layout.Bands[0].Columns["TaxNumber"].Header.Caption = "MST";
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Diễn giải";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["DebitPid"].ValueList = ucbddDebit;
      e.Layout.Bands[0].Columns["ObjectPid"].ValueList = ucbddObject;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["DebitPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DebitPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["ObjectPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ObjectPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;


      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
    }

    private void ugdDataNoInputInvoice_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      double vat = DBConvert.ParseDouble(e.Cell.Row.Cells["VATPercent"].Value);
      switch (columnName)
      {
        case "amount":
          try
          {
            if (DBConvert.ParseDouble(value) >= 0)
            {
              if (vat >= 0)
              {
                if (this.currency == 1)
                {
                  e.Cell.Row.Cells["VATAmount"].Value = Math.Round(DBConvert.ParseDouble(value) * (vat / 100), 0, MidpointRounding.AwayFromZero);

                }
                else
                {
                  e.Cell.Row.Cells["VATAmount"].Value = Math.Round(DBConvert.ParseDouble(value) * (vat / 100), 2, MidpointRounding.AwayFromZero);
                }
                e.Cell.Row.Cells["TotalAmount"].Value = DBConvert.ParseDouble(value) + DBConvert.ParseDouble(e.Cell.Row.Cells["VATAmount"].Value);
              }
            }
          }
          catch
          {
            e.Cell.Row.Cells["VATAmount"].Value = DBNull.Value;
            e.Cell.Row.Cells["TotalAmount"].Value = DBNull.Value;
          }
          break;
        case "vatpercent":
          try
          {
            if (DBConvert.ParseDouble(value) >= 0)
            {
              if (this.currency == 1)
              {
                e.Cell.Row.Cells["VATAmount"].Value = Math.Round(DBConvert.ParseDouble(e.Cell.Row.Cells["Amount"].Value) * (DBConvert.ParseDouble(value) / 100), 0, MidpointRounding.AwayFromZero);
              }
              else
              {
                e.Cell.Row.Cells["VATAmount"].Value = Math.Round(DBConvert.ParseDouble(e.Cell.Row.Cells["Amount"].Value) * (DBConvert.ParseDouble(value) / 100), 2, MidpointRounding.AwayFromZero);
              }
              e.Cell.Row.Cells["TotalAmount"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Amount"].Value) + DBConvert.ParseDouble(e.Cell.Row.Cells["VATAmount"].Value);
            }
          }
          catch
          {
            e.Cell.Row.Cells["TotalAmount"].Value = DBNull.Value;
          }
          break;
        case "totalamount":
          try
          {
            this.TotalAmount();
          }
          catch
          {

          }
          break;
        default:
          break;
      }
    }

    private void ugdDataNoInputInvoice_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ugdDataNoInputInvoice_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listNoInvoiceDeletedPid.Add(pid);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdDataNoInputInvoice, "Chi tiết phiếu ĐNTTTT");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdData);
      }
    }

    private void ugdDataNoInputInvoice_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdDataNoInputInvoice.Selected.Rows.Count > 0 || ugdDataNoInputInvoice.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdDataNoInputInvoice, new Point(e.X, e.Y));
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
        DBParameter[] inputParamMain = new DBParameter[] { new DBParameter("@ObjectPid", DbType.Int32, DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectPid"].Value)) };
        DataTable dtAdvance = DataBaseAccess.SearchStoreProcedureDataTable("spACCPaymentRefundLoadPaymentAdvance_List", inputParamMain);
        Utility.LoadUltraCombo(ucbAdvanceCode, dtAdvance, "AdvanceDetailPid", "AdvanceCode", true, new string[] { "AdvanceDetailPid", "ObjectPid"});
        ucbAdvanceCode.DisplayLayout.Bands[0].Columns["AdvanceCode"].Header.Caption = "Mã tạm ứng";
        ucbAdvanceCode.DisplayLayout.Bands[0].Columns["SubAmount"].Header.Caption = "Tiền tạm ứng";
        ucbAdvanceCode.DisplayLayout.Bands[0].Columns["PaymentCode"].Header.Caption = "Mã tham chiếu";
        ucbAdvanceCode.DisplayLayout.Bands[0].Columns["SubAmount"].Format = "#,##0.##";

        this.objectType = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectType"].Value);
      }
    }


    private void ugdDataNoInputInvoice_AfterRowInsert(object sender, RowEventArgs e)
    {
      e.Row.Cells["Amount"].Value = 0;
      e.Row.Cells["VATPercent"].Value = 0;
      e.Row.Cells["VATAmount"].Value = 0;
      e.Row.Cells["TotalAmount"].Value = 0;
    }


    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (ucbAdvanceCode.SelectedRow != null)
      {
        this.LoadDataAdvanceForGrid();
        this.TotalAmount();
        ucbAdvanceCode.Value = null;
      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText("Vui lòng chọn phiếu tạm ứng.");
        ucbAdvanceCode.Focus();
      }
    }


    private void utcDetail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      //this.LoadTabData();
    }


    private void ugdPaymentAdvance_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdPaymentAdvance);
      e.Layout.AutoFitColumns = false;
      e.Layout.UseFixedHeaders = true;
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["AdvanceDetailPid"].Hidden = true;

      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["AdvanceCode"].Header.Caption = "Mã tạm ứng";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Tiền tạm ứng";
      e.Layout.Bands[0].Columns["ReferenceNo"].Header.Caption = "Mã tham chiếu";
    }

    private void ugdPaymentAdvance_AfterCellUpdate(object sender, CellEventArgs e)
    {

    }

    private void ugdPaymentAdvance_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdPaymentAdvance.Selected.Rows.Count > 0 || ugdPaymentAdvance.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdPaymentAdvance, new Point(e.X, e.Y));
        }
      }
    }

    private void ugdPaymentAdvance_AfterRowInsert(object sender, RowEventArgs e)
    {

    }


    private void ugdPaymentAdvance_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listAdvanceDeletedPid.Add(pid);
        }
      }
    }

    private void ugdPaymentAdvance_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.TotalAmount();
    }


    private void ugdDataNoInputInvoice_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.TotalAmount();
    }
    #endregion Event
  }
}