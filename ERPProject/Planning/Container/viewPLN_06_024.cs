/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewPLN_06_024.cs
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
  public partial class viewPLN_06_024 : MainUserControl
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
      // Load Customer List
      string commandText = @"Select Pid, CustomerCode Code, Name, (CustomerCode + ' - ' + Name) Display 
                            From TblCSDCustomerInfo 
                            Where ParentPid Is Null And Kind NOT IN (1, 2)
                            Order By CustomerCode";
      DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCustomer, dtCustomer, "Pid", "Display", false, new string[] { "Pid", "Display" });

      // Set Language
      this.SetLanguage();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 2;
      string storeName = "spPLNShipment_List";
      string shipmentCode = txtShipmentCode.Text.Trim();
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (shipmentCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@ShipmentCode", DbType.AnsiString, 16, shipmentCode);
      }
      if (customerPid > 0)
      {
        inputParam[1] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
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
      txtShipmentCode.Text = string.Empty;
      ucbCustomer.Value = null;
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
      uegSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      uegData.Text = rm.GetString("Information", ConstantClass.CULTURE);
      lblShipmentCode.Text = rm.GetString("ShipmentCode", ConstantClass.CULTURE);
      lblCustomer.Text = rm.GetString("Customer", ConstantClass.CULTURE);

      btnClear.Text = rm.GetString("Clear", ConstantClass.CULTURE);
      btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);
      btnNew.Text = rm.GetString("New", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewPLN_06_024()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_06_024_Load(object sender, EventArgs e)
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
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

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
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        ugdInformation.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      // Hide column
      e.Layout.Bands[0].Columns["ShipmentPid"].Hidden = true;

      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["PlanShipDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["PlanShipDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ActualShipDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ActualShipDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ETD"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ETD"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      // Set Column Style
      e.Layout.Bands[0].Columns["Status"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Set Width
      e.Layout.Bands[0].Columns["Customer"].Width = 250;

      // Set language
      e.Layout.Bands[0].Columns["ShipmentCode"].Header.Caption = rm.GetString("ShipmentCode", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["ContainerNumber"].Header.Caption = rm.GetString("ContainerNumber", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["Customer"].Header.Caption = rm.GetString("Customer", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = rm.GetString("CreateDate", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["ContainerType"].Header.Caption = rm.GetString("ContainerType", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["PlanShipDate"].Header.Caption = rm.GetString("PlanShipDate", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["ActualShipDate"].Header.Caption = rm.GetString("ActualShipDate", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = rm.GetString("Remark", ConstantClass.CULTURE);
      e.Layout.Bands[0].Columns["Status"].Header.Caption = rm.GetString("Confirm", ConstantClass.CULTURE);

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
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
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
      */
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Data");
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
      viewPLN_06_023 view = new viewPLN_06_023();
      string viewHeader = rm.GetString("ShipmentInfo", ConstantClass.CULTURE);
      WindowUtinity.ShowView(view, viewHeader, true, ViewState.MainWindow);
    }
    #endregion event

    private void ugdInformation_DoubleClick(object sender, EventArgs e)
    {
      if (ugdInformation.Selected.Rows.Count > 0)
      {
        viewPLN_06_023 view = new viewPLN_06_023();
        view.shipmentPid = DBConvert.ParseLong(ugdInformation.Selected.Rows[0].Cells["ShipmentPid"].Value);
        string viewHeader = rm.GetString("ShipmentInfo", ConstantClass.CULTURE);
        WindowUtinity.ShowView(view, viewHeader, true, ViewState.MainWindow);
      }
    }
  }
}
