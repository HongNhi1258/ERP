/*
  Author      : Nguyen Van Tron
  Date        : 1/04/2022
  Description : Payment proposal
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
  public partial class viewACC_14_002 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int docTypePid = 246;
    private int status = 0;
    private DataTable dtObject = new DataTable();    
    #endregion Field

    #region Init
    public viewACC_14_002()
    {
      InitializeComponent();
    }

    private void viewACC_14_002_Load(object sender, EventArgs e)
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCPaymentProposal_Init");
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[0], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });           
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "KeyValue", "Object", false, new string[] { "ObjectPid", "Object", "ObjectType", "KeyValue" });
      ucbObject.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
      Utility.LoadUltraCombo(ucbPaymentType, dsInit.Tables[2], "TypeCode", "TypeName", false, "TypeCode");

      this.dtObject = dsInit.Tables[1];
      // Set Language
      //this.SetLanguage();
    }

    /// <summary>
    /// Read Only for key main data
    /// </summary>
    private void ReadOnlyKeyMainData()
    {
      bool isReadOnly = false;
      if(ugdData.Rows.Count > 0)
      {
        isReadOnly = true;
      }  
      ucbCurrency.ReadOnly = isReadOnly;
      ucbObject.ReadOnly = isReadOnly;
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtProposalCode.ReadOnly = true;
        udtProposalDate.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        ucbPaymentType.ReadOnly = true;
        txtProposalDesc.ReadOnly = true;        
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
      }
      if(this.status != 1)
      {
        btnApprove.Visible = false;
      }  
      else
      {
        btnApprove.Visible = true;
      }
      this.ReadOnlyKeyMainData();
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtProposalCode.Text = dtMain.Rows[0]["ProposalCode"].ToString();
        udtProposalDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["ProposalDate"]);
        lbStatusDes.Text = dtMain.Rows[0]["StatusText"].ToString();
        uneTotalAmount.Value = dtMain.Rows[0]["TotalAmount"];
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        txtCreateBy.Text = dtMain.Rows[0]["CreateBy"].ToString();
        txtApproveBy.Text = dtMain.Rows[0]["ApproveBy"].ToString();
        ucbObject.Value = dtMain.Rows[0]["ObjectValue"];
        if (DBConvert.ParseInt(dtMain.Rows[0]["PaymentType"]) > 0)
        {
          ucbPaymentType.Value = DBConvert.ParseInt(dtMain.Rows[0]["PaymentType"]);
        }
        txtProposalDesc.Text = dtMain.Rows[0]["ProposalDesc"].ToString();
        
        this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"]);
        chkConfirm.Checked = (this.status >= 1 ? true : false);
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
          txtProposalCode.Text = outputParam[0].Value.ToString();
          udtProposalDate.Value = DateTime.Now;
          txtCreateBy.Text = SharedObject.UserInfo.EmpName;
        }
      }
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCPaymentProposal_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        this.LoadMainData(dsSource.Tables[0]);
        ugdData.DataSource = dsSource.Tables[1];
        lbCount.Text = string.Format(@"Đếm {0}", ugdData.Rows.FilteredInRowCount > 0 ? ugdData.Rows.FilteredInRowCount : 0);
      }

      this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      //check master
      if (udtProposalDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày đề nghị không được để trống!!!");
        udtProposalDate.Focus();
        return false;
      }
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Loại tiền tệ không được để trống.");
        ucbCurrency.Focus();
        return false;
      }          
      if (ucbObject.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Đối tượng không được để trống.");
        ucbObject.Focus();
        return false;
      }

      //check detail      
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        UltraGridRow row = ugdData.Rows[i];
        row.Selected = false;
        if (DBConvert.ParseDouble(row.Cells["Amount"].Value.ToString()) <= 0)
        {
          row.Cells["Amount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Số tiền đề nghị không được để trống và phải lớn hơn 0.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
      }

      // Check total proposal <= debit amount of PO or Invoice
      DataTable dtDetail = new DataTable();
      dtDetail.Columns.Add("Pid", typeof(long));
      dtDetail.Columns.Add("APTranPid", typeof(long));      
      dtDetail.Columns.Add("Amount", typeof(double));

      DataTable dtSource = (DataTable)ugdData.DataSource;      
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["Pid"] = row["Pid"];
          rowDetail["APTranPid"] = row["APTranPid"];          
          rowDetail["Amount"] = row["Amount"];
          dtDetail.Rows.Add(rowDetail);
        }
      }
      int paramNumber = 2;
      string storeName = "spACCPaymentProposal_CheckValid";

      SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      inputParam[0] = new SqlDBParameter("@AddedDocList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtDetail));
      inputParam[1] = new SqlDBParameter("@ProposalPid", SqlDbType.BigInt, this.viewPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@ErrorMessage", SqlDbType.NVarChar, string.Empty) };
      SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && outputParam[0].Value.ToString().Length > 0)
      {
        WindowUtinity.ShowMessageErrorFromText(outputParam[0].Value.ToString());
        return false;
      }  
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[10];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }
      inputParam[1] = new SqlDBParameter("@ProposalDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtProposalDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[2] = new SqlDBParameter("@Status", SqlDbType.Int, chkConfirm.Checked ? 1 : 0);
      inputParam[3] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      if (ucbPaymentType.SelectedRow != null)
      {
        inputParam[4] = new SqlDBParameter("@PaymentType", SqlDbType.Int, ucbPaymentType.Value);
      }
      inputParam[5] = new SqlDBParameter("@ObjectType", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectType"].Value);
      inputParam[6] = new SqlDBParameter("@ObjectPid", SqlDbType.Int, ucbObject.SelectedRow.Cells["ObjectPid"].Value);

      if (txtProposalDesc.Text.Trim().Length > 0)
      {
        inputParam[7] = new SqlDBParameter("@ProposalDesc", SqlDbType.NVarChar, txtProposalDesc.Text.Trim().ToString());
      }      
      inputParam[8] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[9] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentProposal_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentProposalDetail_Delete", deleteParam, outputParam);
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
          SqlDBParameter[] inputParam = new SqlDBParameter[7];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@ProposalPid", SqlDbType.BigInt, this.viewPid);
          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[2] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString().Trim());
          }
          inputParam[3] = new SqlDBParameter("@APTranPid", SqlDbType.BigInt, DBConvert.ParseLong(row["APTranPid"].ToString()));          
          inputParam[4] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row["Amount"].ToString()));
          DateTime dueDate = DBConvert.ParseDateTime(row["DueDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          if (dueDate != DateTime.MinValue)
          {
            inputParam[5] = new SqlDBParameter("@DueDate", SqlDbType.Date, dueDate);
          }
          inputParam[6] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentProposalDetail_Save", inputParam, outputParam);
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
          success = this.SaveDetail();
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
      uneTotalAmount.Value = total;
    }
    #endregion Function

    #region Event
    private void ugdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdData);      
      e.Layout.UseFixedHeaders = true;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Allow update, delete, add new
      if (this.status == 0)
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;        
      }
      else
      {
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;        
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["APTranPid"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["DocCode"].Header.Caption = "Mã chứng từ";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Số tiền đề nghị";
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng cộng";
      e.Layout.Bands[0].Columns["PaymentAmount"].Header.Caption = "Đã trả";
      e.Layout.Bands[0].Columns["WaitForPayment"].Header.Caption = "Chờ thanh toán";
      e.Layout.Bands[0].Columns["NotYetProposal"].Header.Caption = "Còn lại";
      e.Layout.Bands[0].Columns["DueDate"].Header.Caption = "Ngày thanh toán";      
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Diễn giải";

      // Read Only
      e.Layout.Bands[0].Columns["DocCode"].CellActivation = Activation.ActivateOnly;      
      e.Layout.Bands[0].Columns["TotalAmount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PaymentAmount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WaitForPayment"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NotYetProposal"].CellActivation = Activation.ActivateOnly;

      // Column color
      e.Layout.Bands[0].Columns["Amount"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["DueDate"].CellAppearance.BackColor = Color.LightGray;

      e.Layout.Bands[0].Columns["DocCode"].Header.Fixed = true;      
      e.Layout.Bands[0].Columns["Amount"].Header.Fixed = true;      

      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        ugdData.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
    }

    private void ugdData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();      
    }

    private void ugdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      if (columnName == "Amount")
      {
        double oldAmount = DBConvert.ParseDouble(e.Cell.Row.Cells["NotYetProposal"].Value);
        double newAmount = DBConvert.ParseDouble(e.NewValue);
        if (newAmount > oldAmount || newAmount <= 0)
        {
          e.Cancel = true;
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageErrorFromText(string.Format("Số tiền đề nghị phải lớn hơn 0 và nhỏ hơn hoặc bằng {0}", oldAmount));
        }
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

    private void ugdData_AfterRowInsert(object sender, RowEventArgs e)
    {
          
    }

    private void btnPaymentSchedule_Click(object sender, EventArgs e)
    {
      // Check valid
      if (ucbCurrency.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Bạn chưa chọn loại tiền tệ.");
        ucbCurrency.Focus();
        return;
      }
      if (ucbObject.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Bạn chưa chọn đối tượng.");
        ucbObject.Focus();
        return;
      }

      // Transfer data to child form
      int currencyPid = DBConvert.ParseInt(ucbCurrency.Value);
      int objectPid = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectPid"].Value);
      int objectType = DBConvert.ParseInt(ucbObject.SelectedRow.Cells["ObjectType"].Value);

      DataTable dtDetail = new DataTable();      
      dtDetail.Columns.Add("APTranPid", typeof(long));      

      DataTable dtSource = (DataTable)ugdData.DataSource;
      DataRow[] rows = dtSource.Select("Pid is null");
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["APTranPid"] = row["APTranPid"];          
          dtDetail.Rows.Add(rowDetail);
        }
      }

      viewACC_14_003 view = new viewACC_14_003();
      view.currencyPid = currencyPid;
      view.objectPid = objectPid;
      view.objectType = objectType;
      view.dtAddedDocList = dtDetail;

      Shared.Utility.WindowUtinity.ShowView(view, "Chọn đợt thanh toán", false, ViewState.ModalWindow);

      // Add selected documents into grid
      if (view.rowSelectedDoc != null && view.rowSelectedDoc.Length > 0)
      {
        for (int i = 0; i < view.rowSelectedDoc.Length; i++)
        {
          DataRow rowDoc = view.rowSelectedDoc[i];
          DataRow row = dtSource.NewRow();
          row["APTranPid"] = rowDoc["APTranPid"];          
          row["DocCode"] = rowDoc["DocCode"];
          row["Amount"] = rowDoc["Amount"];
          row["TotalAmount"] = rowDoc["TotalAmount"];
          row["PaymentAmount"] = rowDoc["PaymentAmount"];
          row["WaitForPayment"] = rowDoc["WaitForPayment"];
          row["NotYetProposal"] = rowDoc["NotYetProposal"];
          row["DueDate"] = rowDoc["DueDate"];
          row["DetailDesc"] = rowDoc["DetailDesc"];
          dtSource.Rows.Add(row);
        }
      }
      this.ReadOnlyKeyMainData();
    }

    private void btnApprove_Click(object sender, EventArgs e)
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      inputParam[1] = new SqlDBParameter("@ApproveBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };
      SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentProposal_Approve", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {      
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.LoadData();
    }

    private void ugdData_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.ReadOnlyKeyMainData();
    }
    #endregion Event
  }
}