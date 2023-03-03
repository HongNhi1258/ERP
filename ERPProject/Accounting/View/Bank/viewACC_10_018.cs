/*
  Author      : Nguyen Thanh Binh
  Date        : 16/04/2021
  Description : Select loan doc
  Standard Form: view_SearchInfo.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_10_018 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public int loanAccountPid = int.MinValue;
    public DataTable dtDetail = new DataTable();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      udtFromDate.Value = null;
      udtToDate.Value = null;

      // Set Language
      //this.SetLanguage();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 4;
      string storeName = "spACCLoanDocListMakeLoanReceipt";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (txtReferenceNo.Text.ToString().Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@ReferenceNo", DbType.String, txtReferenceNo.Text.ToString());
      }
      if (udtFromDate.Value != null)
      {
        inputParam[1] = new DBParameter("@FromDate", DbType.DateTime, DBConvert.ParseDateTime(udtFromDate.Value));
      }
      if (udtToDate.Value != null)
      {
        inputParam[2] = new DBParameter("@ToDate", DbType.DateTime, DBConvert.ParseDateTime(udtToDate.Value));
      }
      inputParam[3] = new DBParameter("@LoanDocPid", DbType.Int32, this.loanAccountPid);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;
      lbCount.Text = string.Format(String.Format("Đếm: {0}", ugdInformation.Rows.FilteredInRowCount > 0 ? ugdInformation.Rows.FilteredInRowCount : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtReferenceNo.Text = string.Empty;
      udtFromDate.Value = null;
      udtToDate.Value = null;
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

    /// <summary>
    /// Create Data Table
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("ReferenceNo", typeof(System.String));
      dt.Columns.Add("ReferenceDate", typeof(System.DateTime));
      dt.Columns.Add("Currency", typeof(System.String));
      dt.Columns.Add("ExchangeRate", typeof(System.Double));
      dt.Columns.Add("LoanDesc", typeof(System.String));
      dt.Columns.Add("TotalAmount", typeof(System.Double));
      return dt;
    }

    #endregion function

    #region event
    public viewACC_10_018()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_10_018_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(uegSearch);
      this.SetBlankForTextOfButton(this);
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
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Override.RowSelectorWidth = 32;
      // Add Column Selected
      if (!e.Layout.Bands[0].Columns.Exists("Selected"))
      {
        UltraGridColumn checkedCol = e.Layout.Bands[0].Columns.Add();
        checkedCol.Key = "Selected";
        checkedCol.Header.Caption = "Chọn";
        checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;
        checkedCol.DataType = typeof(bool);
        checkedCol.Header.VisiblePosition = 0;

        e.Layout.Bands[0].Columns["ReferenceNo"].Header.Caption = "Mã chứng từ";
        e.Layout.Bands[0].Columns["ReferenceDate"].Header.Caption = "Ngày chứng từ";
        e.Layout.Bands[0].Columns["Currency"].Header.Caption = "Tiền tệ";
        e.Layout.Bands[0].Columns["ExchangeRate"].Header.Caption = "Tỉ giá";
        e.Layout.Bands[0].Columns["LoanDesc"].Header.Caption = "Diễn giải";
        e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Số tiền";
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Danh sách phiếu hoàn ứng");
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

    private void btnSelect_Click(object sender, EventArgs e)
    {
      dtDetail = this.CreateDataTable();
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        UltraGridRow row = ugdInformation.Rows[i];
        if ((bool)row.Cells["Selected"].Value == true)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["ReferenceNo"] = row.Cells["ReferenceNo"].Value.ToString();
          rowDetail["ReferenceDate"] = DBConvert.ParseDateTime(row.Cells["ReferenceDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          rowDetail["Currency"] = row.Cells["Currency"].Value.ToString();
          rowDetail["ExchangeRate"] = DBConvert.ParseDouble(row.Cells["ExchangeRate"].Value.ToString());
          rowDetail["LoanDesc"] = row.Cells["LoanDesc"].Value.ToString();
          rowDetail["TotalAmount"] = DBConvert.ParseDouble(row.Cells["TotalAmount"].Value.ToString());
          dtDetail.Rows.Add(rowDetail);
        }
      }
      this.CloseTab();
    }
    #endregion event
  }
}
