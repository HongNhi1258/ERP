/*
  Author      : Nguyen Thanh Binh
  Date        : 16/04/2021
  Description : choose REC to make invoice in
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
  public partial class viewACC_13_004 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public string listRec = string.Empty;
    private bool isDuplicateObject = false;
    public string listPO = string.Empty;
    private bool isDuplicateCurrency = false;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[1];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, ConstantClass.Payment_Advance);
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCInvoiceChooseReceiving_Init", inputParam);
      Utility.LoadUltraCombo(ucbCreateBy, dsInit.Tables[0], "EmployeePid", "Employee", false, "EmployeePid");
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "Pid", "Object", false, new string[] { "Pid", "ObjectType" });
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
      int paramNumber = 5;
      string storeName = "spACCInvoiceChooseWarehoureReceiving_List";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (txtPONo.Text.ToString().Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@ReceivingNo", DbType.String, txtPONo.Text.ToString());
      }
      if (ucbCreateBy.SelectedRow != null)
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
      if (ucbObject.SelectedRow != null)
      {
        inputParam[4] = new DBParameter("@Object", DbType.Int32, DBConvert.ParseInt(ucbObject.Value));
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
      txtPONo.Text = string.Empty;
      ucbCreateBy.Value = null;
      udtFromDate.Value = null;
      udtToDate.Value = null;
      ucbObject.Value = null;
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

    private bool CheckValid()
    {
      this.CheckObjectDuplicate(ugdInformation);
      if (isDuplicateObject)
      {
        WindowUtinity.ShowMessageErrorFromText("Chỉ được chọn đơn hàng cùng đối tượng (NCC).");
        return false;
      }
      this.CheckCurrency(ugdInformation);
      if (isDuplicateCurrency)
      {
        WindowUtinity.ShowMessageErrorFromText("Chỉ được chọn đơn hàng cùng tiền tệ.");
        return false;
      }

      return true;
    }
    /// <summary>
    /// check duplicate object
    /// </summary>
    /// <param name="ultG"></param>
    private void CheckCurrency(UltraGrid ultG)
    {
      isDuplicateCurrency = false;
      for (int k = 0; k < ultG.Rows.Count; k++)
      {
        ultG.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultG.Rows.Count; i++)
      {
        if ((bool)ultG.Rows[i].Cells["Selected"].Value == true)
        {
          int currencyID = DBConvert.ParseInt(ultG.Rows[i].Cells["Currency"].Value.ToString());
          for (int j = i + 1; j < ultG.Rows.Count; j++)
          {
            if ((bool)ultG.Rows[j].Cells["Selected"].Value == true)
            {
              int currencyCompareID = DBConvert.ParseInt(ultG.Rows[j].Cells["Currency"].Value.ToString());
              if (currencyID != currencyCompareID)
              {
                ultG.Rows[i].CellAppearance.BackColor = Color.Yellow;
                ultG.Rows[j].CellAppearance.BackColor = Color.Yellow;
                this.isDuplicateCurrency = true;
              }
            }
          }
        }
      }
    }


    /// <summary>
    /// check duplicate object
    /// </summary>
    /// <param name="ultG"></param>
    private void CheckObjectDuplicate(UltraGrid ultG)
    {
      isDuplicateObject = false;
      for (int k = 0; k < ultG.Rows.Count; k++)
      {
        ultG.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      DataTable dtSelected = (DataTable)ultG.DataSource;
      for (int i = 0; i < ultG.Rows.Count; i++)
      {
        if ((bool)ultG.Rows[i].Cells["Selected"].Value == true)
        {
          int objectID = DBConvert.ParseInt(ultG.Rows[i].Cells["SupplierPid"].Value.ToString());
          for (int j = i + 1; j < ultG.Rows.Count; j++)
          {
            if ((bool)ultG.Rows[j].Cells["Selected"].Value == true)
            {
              int objectCompareID = DBConvert.ParseInt(ultG.Rows[j].Cells["SupplierPid"].Value.ToString());
              if (objectID != objectCompareID)
              {
                ultG.Rows[i].CellAppearance.BackColor = Color.Yellow;
                ultG.Rows[j].CellAppearance.BackColor = Color.Yellow;
                this.isDuplicateObject = true;
              }
            }
          }
        }
      }
    }
    #endregion function

    #region event
    public viewACC_13_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_13_004_Load(object sender, EventArgs e)
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
      e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
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
      }      

      //Caption
      e.Layout.Bands[0].Columns["ReceivingCode"].Header.Caption = "Mã chứng từ";
      e.Layout.Bands[0].Columns["ReceivingDate"].Header.Caption = "Ngày nhập";
      e.Layout.Bands[0].Columns["PONo"].Header.Caption = "Mã đơn\nmua hàng";
      e.Layout.Bands[0].Columns["Object"].Header.Caption = "Đối tượng";
      e.Layout.Bands[0].Columns["Currency"].Header.Caption = "Tiền tệ";
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng tiền";
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Ghi chú";

      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns["ReceivingDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ReceivingDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      // Hidden column
      e.Layout.Bands[0].Columns["SupplierPid"].Hidden = true;

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
      this.listRec = string.Empty;
      bool invalid = this.CheckValid();
      if (invalid)
      {
        for (int i = 0; i < ugdInformation.Rows.Count; i++)
        {
          UltraGridRow row = ugdInformation.Rows[i];
          if ((bool)row.Cells["Selected"].Value == true)
          {
            if (this.listRec != string.Empty)
            {
              this.listRec += ",";
            }
            this.listRec += row.Cells["ReceivingCode"].Value.ToString();
          }
        }
        this.CloseTab();
      }
    }
    #endregion event
  }
}
