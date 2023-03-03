/*
  Author      : Nguyễn Bình An
  Date        : 11/04/2021
  Description : search paymnet voucher
  Standard Form: view_SearchInfo.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_10_005 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    private int docTypePid = ConstantClass.Payment_Voucher;
    private int status = 0;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCPaymentVoucherList_Init", inputParam);
      Utility.LoadUltraCombo(ucbCreateBy, dsInit.Tables[0], "EmployeePid", "Employee", false, "EmployeePid");
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "Pid", "Object", false, new string[] { "Pid", "ObjectType" });
      Utility.LoadUltraCombo(ucbStatus, dsInit.Tables[2], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbActionCode, dsInit.Tables[3], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbNewTypeVoucher, dsInit.Tables[3], "Value", "Display", false, "Value");
      udtFromDate.Value = null;
      udtToDate.Value = null;
      //Utility.LoadUltraCombo();
      //Utility.LoadUltraDropDown();

      // Set Language
      this.SetLanguage();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 8;
      string storeName = "spACCPaymentVoucher_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];

      if (txtPaymentCode.Text.ToString().Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@PaymentCode", DbType.String, txtPaymentCode.Text.ToString());
      }
      if (DBConvert.ParseInt(ucbCreateBy.Value) > 0)
      {
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(ucbCreateBy.Value));
      }
      if (udtFromDate.Value != null)
      {
        inputParam[2] = new DBParameter("@FromDate", DbType.DateTime, DBConvert.ParseDateTime(udtFromDate.Value));
      }
      if (udtToDate.Value != null)
      {
        inputParam[3] = new DBParameter("@ToDate", DbType.DateTime, DBConvert.ParseDateTime(udtToDate.Value));
      }
      if (DBConvert.ParseInt(ucbObject.Value) > 0)
      {
        inputParam[4] = new DBParameter("@Object", DbType.Int32, DBConvert.ParseInt(ucbObject.Value));
        inputParam[5] = new DBParameter("@ObjectType", DbType.Int32, DBConvert.ParseInt(ucbStatus.SelectedRow.Cells["ObjectType"].Value));

      }
      if (DBConvert.ParseInt(ucbStatus.Value) >= 0)
      {
        inputParam[6] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ucbStatus.Value));
      }
      if (DBConvert.ParseInt(ucbActionCode.Value) >= 0)
      {
        inputParam[7] = new DBParameter("@ActionCode", DbType.Int32, DBConvert.ParseInt(ucbActionCode.Value));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", ugdInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtPaymentCode.Text = string.Empty;
      ucbActionCode.Value = null;
      ucbCreateBy.Value = null;
      udtFromDate.Value = null;
      udtToDate.Value = null;
      ucbObject.Value = null;
      ucbStatus.Value = null;
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
      btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewACC_10_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_10_005_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(uegSearch);

      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
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
        this.SearchData();
      }
    }

    private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdInformation);
      e.Layout.AutoFitColumns = false;
      e.Layout.UseFixedHeaders = true;
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Allow update, delete, add new

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["ActionCode"].Hidden = true;

      e.Layout.Bands[0].Columns["PaymentCode"].Header.Caption = "Phiếu chi";
      e.Layout.Bands[0].Columns["PaymentDate"].Header.Caption = "Ngày phiếu chi";
      e.Layout.Bands[0].Columns["Object"].Header.Caption = "Đối tượng";
      e.Layout.Bands[0].Columns["Currency"].Header.Caption = "Tiền tệ";
      e.Layout.Bands[0].Columns["ExchangeRate"].Header.Caption = "Tỉ giá";
      e.Layout.Bands[0].Columns["PaymentDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Người tạo";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Ngày tạo";
      e.Layout.Bands[0].Columns["StatusRemark"].Header.Caption = "Tình trạng";
      e.Layout.Bands[0].Columns["ReceiverName"].Header.Caption = "Người nhận tiền";
      e.Layout.Bands[0].Columns["ActionName"].Header.Caption = "Loại phiếu chi";
      e.Layout.Bands[0].Columns["LoanReceiptPid"].Header.Caption = "Khế ước vay";
      e.Layout.Bands[0].Columns["LoanReceiptPlanPid"].Header.Caption = "Kế hoạch trả tiền vay và nợ gốc";
      e.Layout.Bands[0].Columns["CashFund"].Header.Caption = "Quỹ tiền mặt";



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
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
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
      */
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Danh sách phiếu chi");
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


    private void ugdInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      viewACC_10_006 view = new viewACC_10_006();
      view.viewPid = DBConvert.ParseLong(ugdInformation.ActiveRow.Cells["Pid"].Value);
      view.actionCode = DBConvert.ParseInt(ugdInformation.ActiveRow.Cells["ActionCode"].Value);
      Shared.Utility.WindowUtinity.ShowView(view, "Chi tiết phiếu chi", false, ViewState.MainWindow);
    }


    private void btnNew_Click(object sender, EventArgs e)
    {
      if (ucbNewTypeVoucher.SelectedRow != null)
      {
        if (DBConvert.ParseInt(ucbNewTypeVoucher.SelectedRow.Cells["Value"].Value) == 2 || DBConvert.ParseInt(ucbNewTypeVoucher.SelectedRow.Cells["Value"].Value) == 5 || DBConvert.ParseInt(ucbNewTypeVoucher.SelectedRow.Cells["Value"].Value) == 1)
        {
          viewACC_10_006 view = new viewACC_10_006();
          view.actionCode = DBConvert.ParseInt(ucbNewTypeVoucher.SelectedRow.Cells["Value"].Value);
          Shared.Utility.WindowUtinity.ShowView(view, "Tạo mới phiếu chi", false, ViewState.MainWindow);
        }
        else if (DBConvert.ParseInt(ucbNewTypeVoucher.SelectedRow.Cells["Value"].Value) == 3)
        {
          //viewACC_10_009 view = new viewACC_10_009();
          //view.actionCode = DBConvert.ParseInt(ucbNewTypeVoucher.SelectedRow.Cells["Value"].Value);
          //Shared.Utility.WindowUtinity.ShowView(view, "Tạo mới đề nghị hoàn ứng", false, ViewState.MainWindow);
        }
      }
      else
      {
        WindowUtinity.ShowMessageWarningFromText("Vui lòng chọn loại phiếu chi.");
      }
    }

    #endregion event
  }
}
