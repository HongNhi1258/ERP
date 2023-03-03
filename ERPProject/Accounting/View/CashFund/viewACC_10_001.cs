/*
  Author      : Nguyen Thanh Binh
  Date        : 11/04/2021
  Description : Advance detail
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
  public partial class viewACC_10_001 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private int docTypePid = ConstantClass.Payment_Advance;
    private int status = 0;
    #endregion Field

    #region Init
    public viewACC_10_001()
    {
      InitializeComponent();
    }

    private void viewACC_10_001_Load(object sender, EventArgs e)
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
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCPaymentAdvance_Init", inputParam);
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[0], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });
      Utility.LoadUltraCombo(ucbSegment, dsInit.Tables[1], "Pid", "Display", false, "Pid");
      Utility.LoadUltraCombo(ucbProject, dsInit.Tables[2], "Pid", "Display", false, "Pid");
      Utility.LoadUltraCombo(ucbActionCode, dsInit.Tables[3], "ActionCode", "ActionName", false, "ActionCode");
      Utility.LoadUltraCombo(ucbEmployee, dsInit.Tables[4], "EmployeePid", "Employee", false, "EmployeePid");
      Utility.LoadUltraCombo(ucbPaymentType, dsInit.Tables[5], "Code", "Value", false, "Code");
      // Set Language
      //this.SetLanguage();
    }

    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtAdvanceCode.ReadOnly = true;
        udtAdvanceDate.ReadOnly = true;
        ucbCurrency.ReadOnly = true;
        uneExchangeRate.ReadOnly = true;
        ucbSegment.ReadOnly = true;
        ucbProject.ReadOnly = true;
        txtAdvanceDesc.ReadOnly = true;
        ucbActionCode.ReadOnly = true;
        btnSave.Enabled = false;
        chkConfirm.Enabled = false;
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtAdvanceCode.Text = dtMain.Rows[0]["AdvanceCode"].ToString();
        udtAdvanceDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["AdvanceDate"]);
        ucbCurrency.Value = DBConvert.ParseInt(dtMain.Rows[0]["CurrencyPid"]);
        uneExchangeRate.Value = dtMain.Rows[0]["ExchangeRate"];
        if (DBConvert.ParseInt(dtMain.Rows[0]["SegmentPid"]) > 0)
        {
          ucbSegment.Value = DBConvert.ParseInt(dtMain.Rows[0]["SegmentPid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["ProjectPid"]) > 0)
        {
          ucbProject.Value = DBConvert.ParseInt(dtMain.Rows[0]["ProjectPid"]);
        }
        ucbActionCode.Value = DBConvert.ParseInt(dtMain.Rows[0]["ActionCode"]);
        txtAdvanceDesc.Text = dtMain.Rows[0]["AdvanceDesc"].ToString();
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
          txtAdvanceCode.Text = outputParam[0].Value.ToString();
          udtAdvanceDate.Value = DateTime.Now;
        }
      }
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCPaymentAdvance_Load", inputParam);
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
      if (udtAdvanceDate.Value == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày chứng từ không được để trống!!!");
        udtAdvanceDate.Focus();
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
        WindowUtinity.ShowMessageErrorFromText("Tỉ giá không được để trống.");
        uneExchangeRate.Focus();
        return false;
      }
      if (ucbActionCode.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Loại tạm ứng không được để trống.");
        ucbActionCode.Focus();
        return false;
      }
      //check detail
      if (ugdData.Rows.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Chi tiết không được để trống");
        return false;
      }

      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        UltraGridRow row = ugdData.Rows[i];
        row.Selected = false;
        if (row.Cells["EmployeePid"].Value.ToString().Trim().Length == 0)
        {
          row.Cells["EmployeePid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Nhân viên không được để trống.");
          row.Selected = true;
          ugdData.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseDouble(row.Cells["Amount"].Value.ToString()) <= 0)
        {
          row.Cells["Amount"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Tiền tạm ứng không được để trống hoặc phải lớn hơn bằng 0.");
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
      inputParam[1] = new SqlDBParameter("@AdvanceDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtAdvanceDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      inputParam[2] = new SqlDBParameter("@Status", SqlDbType.Bit, chkConfirm.Checked ? 1 : 0);
      inputParam[3] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, DBConvert.ParseInt(ucbCurrency.Value));
      inputParam[4] = new SqlDBParameter("@ExchangeRate", SqlDbType.Float, uneExchangeRate.Value);
      if (ucbSegment.SelectedRow != null)
      {
        inputParam[5] = new SqlDBParameter("@SegmentPid", SqlDbType.Int, DBConvert.ParseInt(ucbSegment.Value));
      }
      inputParam[6] = new SqlDBParameter("@ActionCode", SqlDbType.Int, DBConvert.ParseInt(ucbActionCode.Value));
      if (ucbProject.SelectedRow != null)
      {
        inputParam[7] = new SqlDBParameter("@ProjectPid", SqlDbType.Int, DBConvert.ParseInt(ucbProject.Value));
      }
      if (txtAdvanceDesc.Text.Trim().Length > 0)
      {
        inputParam[8] = new SqlDBParameter("@AdvanceDesc", SqlDbType.NVarChar, txtAdvanceDesc.Text.Trim().ToString());
      }
      inputParam[9] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParam[10] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };

      SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentAdvanceMaster_Save", inputParam, outputParam);
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
        SqlDataBaseAccess.ExecuteStoreProcedure("", deleteParam, outputParam);
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
          SqlDBParameter[] inputParam = new SqlDBParameter[11];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, pid);
          }
          inputParam[1] = new SqlDBParameter("@AdvancePid", SqlDbType.BigInt, this.viewPid);
          inputParam[2] = new SqlDBParameter("@EmployeePid", SqlDbType.Int, DBConvert.ParseInt(row["EmployeePid"].ToString()));
          inputParam[3] = new SqlDBParameter("@Amount", SqlDbType.Float, DBConvert.ParseDouble(row["Amount"].ToString()));
          if (DBConvert.ParseDateTime(row["StartDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            inputParam[4] = new SqlDBParameter("@StartDate", SqlDbType.Date, DBConvert.ParseDateTime(row["StartDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }
          if (row["DetailDesc"].ToString().Trim().Length > 0)
          {
            inputParam[5] = new SqlDBParameter("@DetailDesc", SqlDbType.NVarChar, row["DetailDesc"].ToString());
          }
          if (DBConvert.ParseInt(row["PaymentType"].ToString()) > 0)
          {
            inputParam[6] = new SqlDBParameter("@PaymentType", SqlDbType.Int, DBConvert.ParseInt(row["PaymentType"].ToString()));
          }
          if (DBConvert.ParseInt(row["MonthQty"].ToString()) >= 0)
          {
            inputParam[7] = new SqlDBParameter("@MonthQty", SqlDbType.Int, DBConvert.ParseInt(row["MonthQty"].ToString()));
          }
          if (DBConvert.ParseDouble(row["MonthlyAmount"].ToString()) >= 0)
          {
            inputParam[8] = new SqlDBParameter("@MonthlyAmount", SqlDbType.Float, DBConvert.ParseDouble(row["MonthlyAmount"].ToString()));
          }
          inputParam[9] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          inputParam[10] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, ConstantClass.Payment_Advance);
          SqlDataBaseAccess.ExecuteStoreProcedure("spACCPaymentAdvanceDetail_Save", inputParam, outputParam);
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
        if (DBConvert.ParseDouble(ugdData.Rows[i].Cells["Amount"].Value) != Double.MinValue)
        {
          total += DBConvert.ParseDouble(ugdData.Rows[i].Cells["Amount"].Value);
        }
      }
      uneTotalAmount.Value = total;
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdData);
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["EmployeePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
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
      e.Layout.Bands[0].Columns["EmployeePid"].Header.Caption = "Nhân viên";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Tiền tạm ứng";
      e.Layout.Bands[0].Columns["StartDate"].Header.Caption = "Ngày bắt đầu trả";
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Ghi chú";
      e.Layout.Bands[0].Columns["PaymentType"].Header.Caption = "Hình thức trả";
      e.Layout.Bands[0].Columns["MonthQty"].Header.Caption = "Số tháng trả";
      e.Layout.Bands[0].Columns["MonthlyAmount"].Header.Caption = "Sô tiền trả hàng tháng";

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["EmployeePid"].ValueList = ucbEmployee;
      e.Layout.Bands[0].Columns["PaymentType"].ValueList = ucbPaymentType;

      e.Layout.Bands[0].Columns["EmployeePid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
      e.Layout.Bands[0].Columns["PaymentType"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["EmployeePid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["EmployeePid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["PaymentType"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["PaymentType"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        ugdData.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";


      // Set Width
      e.Layout.Bands[0].Columns["EmployeePid"].Width = 200;
      e.Layout.Bands[0].Columns["PaymentType"].Width = 120;
      /*
    
      
      // Enable support for displaying errors through IDataErrorInfo interface. By default
			// the functionality is disabled.
			e.Layout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;

			// Set data error related appearances.
			e.Layout.Override.DataErrorCellAppearance.ForeColor = Color.Red;
			e.Layout.Override.DataErrorRowAppearance.BackColor = Color.LightYellow;
			e.Layout.Override.DataErrorRowSelectorAppearance.BackColor = Color.Green;

			// Make the row selectors bigger so they can accomodate the data error icon as 
			// well active row indicator.
			e.Layout.Override.RowSelectorWidth = 32;

     
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Add Column Selected
      if (!e.Layout.Bands[0].Columns.Exists("Selected"))
      {
        UltraGridColumn checkedCol = e.Layout.Bands[0].Columns.Add();
        checkedCol.Key = "Selected";
        checkedCol.Header.Caption = string.Empty;
        checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;        
        checkedCol.DataType = typeof(bool);
        checkedCol.Header.VisiblePosition = 0;
      } 
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      // Set language
      e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
      */
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
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

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
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
      Utility.ExportToExcelWithDefaultPath(ugdData, "Chi tiết phiếu tạm ứng");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdData);
      }
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
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
    #endregion Event
  }
}
