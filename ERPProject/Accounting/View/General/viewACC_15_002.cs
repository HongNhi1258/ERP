/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : payment voucher
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
  public partial class viewACC_15_002 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    public int status = 0;
    private DataTable dtObject = new DataTable();
    private int docTypePid = 122;
    public int actionCode = int.MinValue;
    private int currency = int.MinValue;
    #endregion Field

    #region Init
    public viewACC_15_002()
    {
      InitializeComponent();
    }

    private void viewACC_15_002_Load(object sender, EventArgs e)
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCAccountDocument_Init");
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[0], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });

      //Init dropdown
      Utility.LoadUltraCBAccountList(ucbddDebit);
      Utility.LoadUltraCBAccountList(ucbddCredit);
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "KeyValue", "Object", false, new string[] { "ObjectPid", "Object", "ObjectType", "KeyValue" });
      Utility.LoadUltraCombo(ucbddCreditObject, dsInit.Tables[1], "KeyValue", "Object", false, new string[] { "ObjectPid", "Object", "ObjectType", "KeyValue" });
      Utility.LoadUltraCombo(ucbddDebitObject, dsInit.Tables[1], "KeyValue", "Object", false, new string[] { "ObjectPid", "Object", "ObjectType", "KeyValue" });
      Utility.LoadUltraCombo(ucbddInputVATDocType, dsInit.Tables[2], "Value", "Display", false, "Value");
    

        // Set Language
        //this.SetLanguage();
      }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtDocCode.ReadOnly = true;
        udtDocDate.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        ucbObject.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        txtDocDesc.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
        chkPosstDebt.Enabled = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtDocCode.Text = dtMain.Rows[0]["DocCode"].ToString();
        udtDocDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["DocDate"]);
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        uneExchangeRate.Value = dtMain.Rows[0]["ExchangeRate"];
        ucbObject.Value = dtMain.Rows[0]["ObjectPid"];
        txtDocDesc.Text = dtMain.Rows[0]["DocDesc"].ToString();
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
          txtDocCode.Text = outputParam[0].Value.ToString();
          udtDocDate.Value = DateTime.Now;
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
        case "utpcListDocument":
          //this.LoadData();
          break;
        case "utpcPostDocument":
          if (chkConfirm.Checked)
            this.LoadTransationData();
          break;
        default:
          break;
      }
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCAccountDocument_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        this.LoadMainData(dsSource.Tables[0]);
        ugdData.DataSource = dsSource.Tables[1];
        lbCount.Text = string.Format(@"Đếm {0}", ugdData.Rows.FilteredInRowCount > 0 ? ugdData.Rows.FilteredInRowCount : 0);
      }

      this.SetStatusControl();
      this.NeedToSave = false;
    }
    /// <summary>
    /// Load transaction
    /// </summary>
    private void LoadTransationData()
    {
      grdPostTran.SetDataSource(this.docTypePid, this.viewPid);
    }
    private bool CheckValid()
    {
      //check master
      if (udtDocDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtDocDate.Focus();
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
      if (ucbObject.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Đối tượng không được để trống!!!");
        ucbObject.Focus();
        return false;
      }

      //check detail

      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        UltraGridRow row = ugdData.Rows[i];
        row.Selected = false;
        if (DBConvert.ParseDouble(row.Cells["DebitAccountPid"].Value) <= 0)
        {
          row.Cells["DebitAccountPid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tài khoản nợ không được để trống.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseDouble(row.Cells["CreditAccountPid"].Value) <= 0)
        {
          row.Cells["CreditAccountPid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tài khoản có không được để trống.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseDouble(row.Cells["Amount"].Value) <= 0)
        {
          row.Cells["Amount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tiền hàng không được để trống và phải lớn hơn 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (row.Cells["TaxPercent"].Text.Length > 0 && DBConvert.ParseDouble(row.Cells["TaxPercent"].Value) < 0)
        {
          row.Cells["TaxPercent"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("% Thuế phải lớn hơn 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
      }
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[11];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      if (txtDocDesc.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@DocDesc", SqlDbType.NVarChar, txtDocDesc.Text.Trim().ToString());
      }
      inputParam[2] = new SqlDBParameter("@DocDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtDocDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[3] = new SqlDBParameter("@ObjectPid", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectPid"].Value);
      inputParam[4] = new SqlDBParameter("@ObjectType", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectType"].Value);
      inputParam[5] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[6] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, uneExchangeRate.Value);
      inputParam[7] = new SqlDBParameter("@IsPostDebt", SqlDbType.Bit, chkPosstDebt.Checked ? 1 :0);
      inputParam[8] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[9] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[10] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCAccountDocumentMaster_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCAccountDocumentDetail_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2. Insert/Update      
      DataTable dtDetail = (DataTable)ugdData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          SqlDBParameter[] inputParam = new SqlDBParameter[18];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@AccountDocPid", SqlDbType.BigInt, this.viewPid);

          if (DBConvert.ParseInt(row["DebitAccountPid"].ToString()) > 0)
          {
            inputParam[2] = new SqlDBParameter("@DebitAccountPid", SqlDbType.Int, DBConvert.ParseInt(row["DebitAccountPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DebitObjectPid"].ToString()) > 0)
          {
            inputParam[3] = new SqlDBParameter("@DebitObjectPid", SqlDbType.Int, DBConvert.ParseInt(row["DebitObjectPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["DebitObjectType"].ToString()) > 0)
          {
            inputParam[4] = new SqlDBParameter("@DebitObjectType", SqlDbType.Int, DBConvert.ParseInt(row["DebitObjectType"].ToString()));
          }
          if (DBConvert.ParseInt(row["CreditAccountPid"].ToString()) > 0)
          {
            inputParam[5] = new SqlDBParameter("@CreditAccountPid", SqlDbType.Int, DBConvert.ParseInt(row["CreditAccountPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CreditObjectPid"].ToString()) > 0)
          {
            inputParam[6] = new SqlDBParameter("@CreditObjectPid", SqlDbType.Int, DBConvert.ParseInt(row["CreditObjectPid"].ToString()));
          }
          if (DBConvert.ParseInt(row["CreditObjectType"].ToString()) > 0)
          {
            inputParam[7] = new SqlDBParameter("@CreditObjectType", SqlDbType.Int, DBConvert.ParseInt(row["CreditObjectType"].ToString()));
          }
          if (row["VATInvoiceNo"].ToString().Trim().Length > 0)
          {
            inputParam[8] = new SqlDBParameter("@VATInvoiceNo", SqlDbType.VarChar, row["VATInvoiceNo"].ToString());
          }
          if (DBConvert.ParseDateTime(row["VATDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            inputParam[9] = new SqlDBParameter("@VATDate", SqlDbType.Date, DBConvert.ParseDateTime(row["VATDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }
          if (row["VATFormNo"].ToString().Trim().Length > 0)
          {
            inputParam[10] = new SqlDBParameter("@VATFormNo", SqlDbType.VarChar, row["VATFormNo"].ToString());
          }
          if (row["VATSymbol"].ToString().Trim().Length > 0)
          {
            inputParam[11] = new SqlDBParameter("@VATSymbol", SqlDbType.VarChar, row["VATSymbol"].ToString());
          }
          if (DBConvert.ParseInt(row["InputVATDocType"].ToString()) > 0)
          {
            inputParam[12] = new SqlDBParameter("@InputVATDocType", SqlDbType.Int, DBConvert.ParseInt(row["InputVATDocType"].ToString()));
          }
          inputParam[13] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row["Amount"].ToString()));
          if (DBConvert.ParseDouble(row["TaxPercent"].ToString()) > 0)
          {
            inputParam[14] = new SqlDBParameter("@TaxPercent", SqlDbType.Float, DBConvert.ParseDouble(row["TaxPercent"].ToString()));
          }
          if (DBConvert.ParseDouble(row["TaxAmount"].ToString()) > 0)
          {
            inputParam[15] = new SqlDBParameter("@TaxAmount", SqlDbType.Float, DBConvert.ParseDouble(row["TaxAmount"].ToString()));
          }
          inputParam[16] = new SqlDBParameter("@TotalAmount", SqlDbType.Float, DBConvert.ParseDouble(row["TotalAmount"].ToString()));            
          inputParam[17] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCAccountDocumentDetail_Save", inputParam, outputParam);
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
          if (this.SaveDetail())
          {
            if (chkConfirm.Checked)
            {
              bool isPosted = chkConfirm.Checked;
              success = Utility.ACCPostTransaction(this.docTypePid, this.viewPid, isPosted, SharedObject.UserInfo.UserPid);
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
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Allow update, delete, add new
      if (this.status == 0)
      {        
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      }
      else
      {        
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
        for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
        e.Layout.Bands[0].Columns["DebitAccountPid"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["CreditAccountPid"].CellActivation = Activation.AllowEdit;
      }       

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["DebitObjectPid"].Hidden = true;
      e.Layout.Bands[0].Columns["DebitObjectType"].Hidden = true;
      e.Layout.Bands[0].Columns["CreditObjectPid"].Hidden = true;
      e.Layout.Bands[0].Columns["CreditObjectType"].Hidden = true;

      e.Layout.Bands[0].Columns["DebitObjectKey"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CreditObjectKey"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["DebitAccountPid"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CreditAccountPid"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Amount"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TaxPercent"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TaxAmount"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Fixed = true;


      // Set caption column
      e.Layout.Bands[0].Columns["DebitAccountPid"].Header.Caption = "TK nợ";
      e.Layout.Bands[0].Columns["CreditAccountPid"].Header.Caption = "TK có";
      e.Layout.Bands[0].Columns["DebitObjectKey"].Header.Caption = "Đối tượng nợ";
      e.Layout.Bands[0].Columns["CreditObjectKey"].Header.Caption = "Đối tượng có";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Số tiền";
      e.Layout.Bands[0].Columns["TaxPercent"].Header.Caption = "% thuế";
      e.Layout.Bands[0].Columns["TaxAmount"].Header.Caption = "Tiền thuế";
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng\ntiền";
      e.Layout.Bands[0].Columns["VATDate"].Header.Caption = "Ngày\nhóa đơn";
      e.Layout.Bands[0].Columns["InputVATDocType"].Header.Caption = "Loại\nhóa đơn";
      e.Layout.Bands[0].Columns["VATFormNo"].Header.Caption = "Mẫu số";
      e.Layout.Bands[0].Columns["VATSymbol"].Header.Caption = "Ký hiệu";
      e.Layout.Bands[0].Columns["VATInvoiceNo"].Header.Caption = "Số\nhóa đơn";

      // Read Only
      e.Layout.Bands[0].Columns["TaxAmount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalAmount"].CellActivation = Activation.ActivateOnly;      

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["DebitAccountPid"].ValueList = ucbddDebit;
      e.Layout.Bands[0].Columns["CreditAccountPid"].ValueList = ucbddCredit;
      e.Layout.Bands[0].Columns["InputVATDocType"].ValueList = ucbddInputVATDocType;
      e.Layout.Bands[0].Columns["DebitObjectKey"].ValueList = ucbddDebitObject;
      e.Layout.Bands[0].Columns["CreditObjectKey"].ValueList = ucbddCreditObject;

      //set align
      e.Layout.Bands[0].Columns["DebitAccountPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CreditAccountPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["DebitObjectKey"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["CreditObjectKey"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["InputVATDocType"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;


      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["DebitAccountPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DebitAccountPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CreditAccountPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CreditAccountPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["InputVATDocType"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["InputVATDocType"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["DebitObjectKey"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["DebitObjectKey"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["CreditObjectKey"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["CreditObjectKey"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
     
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";

      e.Layout.Bands[0].Columns["CreditObjectKey"].Width = 200;
      e.Layout.Bands[0].Columns["DebitObjectKey"].Width = 200;

    }

    private void ugdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      double vat = DBConvert.ParseDouble(e.Cell.Row.Cells["TaxPercent"].Value);
      switch (columnName)
      {
        case "debitobjectkey":
          if (ucbddDebitObject.SelectedRow == null)
          {
            ucbddDebitObject.Value = e.Cell.Value;
          }  
          e.Cell.Row.Cells["DebitObjectPid"].Value = DBConvert.ParseInt(ucbddDebitObject.SelectedRow.Cells["ObjectPid"].Value);
          e.Cell.Row.Cells["DebitObjectType"].Value = DBConvert.ParseInt(ucbddDebitObject.SelectedRow.Cells["ObjectType"].Value);
          break;
        case "creditobjectkey":
          e.Cell.Row.Cells["CreditObjectPid"].Value = DBConvert.ParseInt(ucbddCreditObject.SelectedRow.Cells["ObjectPid"].Value);
          e.Cell.Row.Cells["CreditObjectType"].Value = DBConvert.ParseInt(ucbddCreditObject.SelectedRow.Cells["ObjectType"].Value);
          break;
        case "amount":
          try
          {
            if (DBConvert.ParseDouble(value) >= 0)
            {
              if (vat >= 0)
              {
                if (this.currency == 1)
                {
                  e.Cell.Row.Cells["TaxAmount"].Value = Math.Round(DBConvert.ParseDouble(value) * (vat / 100), 0, MidpointRounding.AwayFromZero);

                }
                else
                {
                  e.Cell.Row.Cells["TaxAmount"].Value = Math.Round(DBConvert.ParseDouble(value) * (vat / 100), 2, MidpointRounding.AwayFromZero);
                }
                e.Cell.Row.Cells["TotalAmount"].Value = DBConvert.ParseDouble(value) + DBConvert.ParseDouble(e.Cell.Row.Cells["TaxAmount"].Value);
              }
            }
          }
          catch
          {
            e.Cell.Row.Cells["TaxAmount"].Value = DBNull.Value;
            e.Cell.Row.Cells["TotalAmount"].Value = DBNull.Value;
          }
          break;
        case "taxpercent":
          try
          {
            if (DBConvert.ParseDouble(value) >= 0)
            {
              if (this.currency == 1)
              {
                e.Cell.Row.Cells["TaxAmount"].Value = Math.Round(DBConvert.ParseDouble(e.Cell.Row.Cells["Amount"].Value) * (DBConvert.ParseDouble(value) / 100), 0, MidpointRounding.AwayFromZero);
              }
              else
              {
                e.Cell.Row.Cells["TaxAmount"].Value = Math.Round(DBConvert.ParseDouble(e.Cell.Row.Cells["Amount"].Value) * (DBConvert.ParseDouble(value) / 100), 2, MidpointRounding.AwayFromZero);
              }
              e.Cell.Row.Cells["TotalAmount"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Amount"].Value) + DBConvert.ParseDouble(e.Cell.Row.Cells["TaxAmount"].Value);
            }
            else
            {
              e.Cell.Row.Cells["TaxAmount"].Value = DBNull.Value;
              e.Cell.Row.Cells["TotalAmount"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Amount"].Value);
            }  
          }
          catch
          {
            e.Cell.Row.Cells["TotalAmount"].Value = DBNull.Value;
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
        case "DebitAccountPid":
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
        case "CreditAccountPid":
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
      Utility.ExportToExcelWithDefaultPath(ugdData, "Chi tiết phiếu kế toán");
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


   


    private void ugdData_AfterRowInsert(object sender, RowEventArgs e)
    {    
      e.Row.Cells["Amount"].Value = 0;
      e.Row.Cells["TaxPercent"].Value = 0;
      e.Row.Cells["TaxAmount"].Value = 0;
      e.Row.Cells["TotalAmount"].Value = 0;

      if (ucbObject.SelectedRow != null)
      {
        e.Row.Cells["DebitObjectKey"].Value = ucbObject.Value;
      }
      else
      {
        e.Row.Cells["DebitObjectKey"].Value = DBNull.Value;
      }
    }  
    private void utcDetail_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      this.LoadTabData();
    }
    #endregion Event
  }
}