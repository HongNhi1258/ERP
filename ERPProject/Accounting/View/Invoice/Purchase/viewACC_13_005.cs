/*
  Author      : Nguyễn Văn Trọn
  Date        : 10/5/2022
  Description : Clear deposit payment for PO in Inovice
  Standard Form: view_SearchSave.cs
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
  public partial class viewACC_13_005 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewACC_13_005).Assembly);
    private IList listDeletedPid = new ArrayList();
    public long invoiceInPid;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ucbSupplierPayment
      //Utility.LoadUltraCombo();
      //Utility.LoadUltraDropDown();

      // Set Language
      this.SetLanguage();
    }

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

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void LoadData()
    {
      this.NeedToSave = false;      
      int paramNumber = 1;
      string storeName = "spACCAPDepositPaymentClearing_Load";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@InvoiceInPid", DbType.Int32, this.invoiceInPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);

      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        //1. Load main data
        if (dsSource.Tables[0].Rows.Count > 0)
        {
          uneTotalAmount.Value = dsSource.Tables[0].Rows[0]["TotalAmount"];
          unePaymentAmount.Value = dsSource.Tables[0].Rows[0]["PaymentAmount"];
          uneWaitForPayment.Value = dsSource.Tables[0].Rows[0]["WaitForPayment"];
          uneRemainAmount.Value = dsSource.Tables[0].Rows[0]["RemainAmount"];
        }

        //2. Load Detail data (list of clearing deposit payment)
        ugdInformation.DataSource = dsSource.Tables[1];

        lblCount.Text = string.Format("Đếm: {0}", (dsSource.Tables[1] != null ? dsSource.Tables[1].Rows.Count : 0));
      }
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
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

    private bool CheckValid(out string clearingXMLString)
    {
      string errorMessage = string.Empty;
      // Check payment & clearing Amount
      DataTable dtDetail = new DataTable();
      dtDetail.Columns.Add("Pid", typeof(long));
      dtDetail.Columns.Add("PaymentPid", typeof(long));
      dtDetail.Columns.Add("PONo", typeof(string));
      dtDetail.Columns.Add("ClearAmount", typeof(double));

      DataTable dtSource = (DataTable)ugdInformation.DataSource;
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["Pid"] = row["Pid"];
          rowDetail["PaymentPid"] = row["PaymentPid"];
          rowDetail["PONo"] = row["PONo"];
          rowDetail["ClearAmount"] = row["ClearAmount"];
          dtDetail.Rows.Add(rowDetail);
        }
      }
      int paramNumber = 2;
      string storeName = "spACCAPDepositPaymentClearing_CheckValid";

      clearingXMLString = DBConvert.ParseXMLString(dtDetail);

      SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      inputParam[0] = new SqlDBParameter("@DepositClearing", SqlDbType.NVarChar, clearingXMLString);
      inputParam[1] = new SqlDBParameter("@InvoiceInPid", SqlDbType.BigInt, this.invoiceInPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@ErrorMessage", SqlDbType.NVarChar, string.Empty) };
      SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && outputParam[0].Value.ToString().Length > 0)
      {
        errorMessage = outputParam[0].Value.ToString();
        WindowUtinity.ShowMessageErrorFromText(errorMessage);
        return false;
      }      
      return true;
    }

    private void SaveData()
    {
      string clearingXMLString;
      if (this.CheckValid(out clearingXMLString))
      {
        bool success = true;
        // 1. Delete/Insert/Update      
        SqlDBParameter[] inputParam = new SqlDBParameter[3];
        inputParam[0] = new SqlDBParameter("@DepositClearing", SqlDbType.NVarChar, clearingXMLString);
        inputParam[1] = new SqlDBParameter("@InvoiceInPid", SqlDbType.BigInt, this.invoiceInPid);
        inputParam[2] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
        SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.Int, 0) };
        SqlDataBaseAccess.ExecuteStoreProcedure("spACCAPDepositPaymentClearing_Edit", inputParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
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
      //lblCount.Text = rm.GetString("Count", ConstantClass.CULTURE) + ":";
      //btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);      
      //btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);      

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewACC_13_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_13_005_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(gpbSearch);

      //Init Data
      this.InitData();

      //Load data
      this.LoadData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.LoadData();
      }
    }

    private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Utility.InitLayout_UltraGrid(ugdInformation);
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        ugdInformation.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      // Set caption column
      e.Layout.Bands[0].Columns["PaymentKind"].Header.Caption = "Loại chứng từ";
      e.Layout.Bands[0].Columns["PaymentDate"].Header.Caption = "Ngày chứng từ";
      e.Layout.Bands[0].Columns["PaymentCode"].Header.Caption = "Mã chứng từ";
      e.Layout.Bands[0].Columns["PODepositAmount"].Header.Caption = "Tổng tiền cọc";
      e.Layout.Bands[0].Columns["PONo"].Header.Caption = "Đơn mua hàng";
      e.Layout.Bands[0].Columns["ClearAmount"].Header.Caption = "Số tiền cấn trừ";
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["POInInvoiceAmount"].Header.Caption = "Tiền ĐMH trong HĐ";
      e.Layout.Bands[0].Columns["POTotalAmount"].Header.Caption = "Tổng tiền ĐMH";
      e.Layout.Bands[0].Columns["POInInvoicePercent"].Header.Caption = "%";


      // Format
      e.Layout.Bands[0].Columns["PODepositAmount"].Format = "#,##0.##";
      e.Layout.Bands[0].Columns["ClearAmount"].Format = "#,##0.##";
      e.Layout.Bands[0].Columns["POInInvoiceAmount"].Format = "#,##0.##";
      e.Layout.Bands[0].Columns["POTotalAmount"].Format = "#,##0.##";

      // Read only
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["ClearAmount"].CellActivation = Activation.AllowEdit;      

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PaymentPid"].Hidden = true;

      // Summaries
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["ClearAmount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";

      /*
      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns[""].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns[""].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

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

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
            
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
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
      ugdInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ugdInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ugdInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ugdInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      // Set language
      e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
      //TOtal
      
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
      */
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ugdInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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

    private void ugdInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "ClearAmount":
          double poDepositAmount = DBConvert.ParseDouble(e.Cell.Row.Cells["PODepositAmount"].Value);
          double clearAmount = DBConvert.ParseDouble(value);
          double poInInvoiceAmount = DBConvert.ParseDouble(e.Cell.Row.Cells["POInInvoiceAmount"].Value);
          if(clearAmount < 0 || (clearAmount > poDepositAmount) || (clearAmount > poInInvoiceAmount))
          {
            WindowUtinity.ShowMessageError("ERR0029", "Số tiền cấn trừ");
            e.Cancel = true;
          }            
         
          break;
        default:
          break;
      }
    }

    private void ugdInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      //Utility.ExportToExcelWithDefaultPath(ugdInformation, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdInformation);
      }
    }

    private void ugdInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdInformation.Selected.Rows.Count > 0 || ugdInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdInformation, new Point(e.X, e.Y));
        }
      }
    }
    #endregion event
  }
}
