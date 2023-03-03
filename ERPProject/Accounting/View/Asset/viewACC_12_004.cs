/*
  Author      : Nguyen Thanh Binh
  Date        : 16/04/2021
  Description : Asset Receipt list
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
  public partial class viewACC_12_004 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, ConstantClass.Asset_Receipt);
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCAssetReceiptList_Init", inputParam);
      Utility.LoadUltraCombo(ucbCreateBy, dsInit.Tables[0], "EmployeePid", "Employee", false, "EmployeePid");
      Utility.LoadUltraCombo(ucbStatus, dsInit.Tables[1], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbKind, dsInit.Tables[2], "Value", "Display", false, "Value");
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
      int paramNumber = 6;
      string storeName = "spACCAssetReceipt_List";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (txtBankDebitCode.Text.ToString().Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@ReceiptCode", DbType.String, txtBankDebitCode.Text.ToString());
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
      if (DBConvert.ParseInt(ucbStatus.Value) >= 0)
      {
        inputParam[4] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ucbStatus.Value));
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        ugdInformation.Rows[i].Appearance.BackColor = ((bool)(ugdInformation.Rows[i].Cells["Status"].Value) ? Color.LightGray : Color.White);
      }
      lbCount.Text = string.Format(String.Format("Đếm: {0}", ugdInformation.Rows.FilteredInRowCount > 0 ? ugdInformation.Rows.FilteredInRowCount : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtBankDebitCode.Text = string.Empty;
      ucbCreateBy.Value = null;
      udtFromDate.Value = null;
      udtToDate.Value = null;
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
    public viewACC_12_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_12_004_Load(object sender, EventArgs e)
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
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;

      e.Layout.Bands[0].Columns["ReceiptCode"].Header.Caption = "Mã chứng từ";
      e.Layout.Bands[0].Columns["ReceiptDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["Currency"].Header.Caption = "Tiền tệ";
      e.Layout.Bands[0].Columns["ExchangeRate"].Header.Caption = "Tỉ giá";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Người tạo";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Người tạo";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Ngày tạo";
      e.Layout.Bands[0].Columns["StatusRemark"].Header.Caption = "Tình trạng";

    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Danh sách báo nợ ngân hàng");
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


    private void btnNew_Click(object sender, EventArgs e)
    {
      if (ucbKind.SelectedRow != null)
      {
        viewACC_12_003 view = new viewACC_12_003();
        view.actionCode = DBConvert.ParseInt(ucbKind.Value);
        Shared.Utility.WindowUtinity.ShowView(view, "Tạo mới phiếu tăng tài sản", false, ViewState.MainWindow);

      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText("Vui lòng chọn loại tăng tài sản.");
      }

    }


    private void ugdInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      viewACC_12_003 view = new viewACC_12_003();
      view.viewPid = DBConvert.ParseLong(ugdInformation.ActiveRow.Cells["Pid"].Value);
      Shared.Utility.WindowUtinity.ShowView(view, "Chi tiết phiếu tăng tài sản", false, ViewState.MainWindow);
    }
    #endregion event
  }
}
