/*
  Author      : Nguyen Thanh Binh
  Date        : 16/04/2021
  Description : Asset list
  Standard Form: view_SearchInfo.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_12_002 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public string listPaymentRefundPid = string.Empty;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCAssetList_Init");
      Utility.LoadUltraCombo(ucbCreateBy, dsInit.Tables[0], "EmployeePid", "Employee", false, "EmployeePid");
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
      string storeName = "spACCAssetList_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (txtAssetCode.Text.ToString().Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@AssetCode", DbType.String, txtAssetCode.Text.ToString());
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
      txtAssetCode.Text = string.Empty;
      ucbCreateBy.Value = null;
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
    #endregion function

    #region event
    public viewACC_12_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_12_002_Load(object sender, EventArgs e)
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

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["AssetCode"].Header.Caption = "Mã tài sản";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Người tạo";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Ngày tạo";
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
      this.listPaymentRefundPid = string.Empty;
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        UltraGridRow row = ugdInformation.Rows[i];
        if ((bool)row.Cells["Selected"].Value == true)
        {
          if (this.listPaymentRefundPid != string.Empty)
          {
            this.listPaymentRefundPid += ",";
          }
          this.listPaymentRefundPid += row.Cells["Pid"].Value.ToString();
        }
      }
      this.CloseTab();
    }


    private void btnNew_Click(object sender, EventArgs e)
    {
      viewACC_12_001 view = new viewACC_12_001();
      Shared.Utility.WindowUtinity.ShowView(view, "Tạo tài sản mới", false, ViewState.MainWindow);
    }


    private void ugdInformation_DoubleClick(object sender, EventArgs e)
    {
      viewACC_12_001 view = new viewACC_12_001();
      view.viewPid = DBConvert.ParseLong(ugdInformation.ActiveRow.Cells["Pid"].Value);
      Shared.Utility.WindowUtinity.ShowView(view, "Chi tiết tài sản", false, ViewState.MainWindow);
    }
    #endregion event
  }
}
