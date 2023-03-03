/*
  Author      : 
  Date        : 
  Description : 
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
  public partial class viewGNR_03_001 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(view_SearchSave).Assembly);
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {

      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spLoadUCBColumnAlias");
      LoadUltraCombo_Item(ucbColumnName, dsInit.Tables[0], "ColumnName");
      LoadUltraCombo_Item(ucbDocCode, dsInit.Tables[1], "DocCode");


      LoadUltraCombo_Item(ucbddDocCode, dsInit.Tables[1], "DocCode");



      // Set Language
      this.SetLanguage();
    }
    private void LoadUltraCombo_Item(UltraCombo ucbData, DataTable dtSource, string columnText)
    {
      ucbData.DataSource = dtSource;
      if (dtSource != null)
      {

        ucbData.DisplayMember = columnText;

      }
      ucbData.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      ucbData.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      ucbData.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      ucbData.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
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
    private void SearchData()
    {
      btnSearch.Enabled = false;
      DBParameter[] inputParam = new DBParameter[4];
      if (ucbDocCode.Text != null)
      {

        inputParam[0] = new DBParameter("@DocCode", DbType.AnsiString, 20, "%" + ucbDocCode.Text.Replace("'", "''") + "%");
      }

      if (ucbColumnName.Text != null)
      {

        inputParam[1] = new DBParameter("@ColumnName", DbType.AnsiString, 20, "%" + ucbColumnName.Text.Replace("'", "''") + "%");
      }
      if (txtVNCaption.Text != "")
      {

        inputParam[2] = new DBParameter("@VNCaption", DbType.AnsiString, 20, "%" + txtVNCaption.Text.Replace("'", "''") + "%");
      }
      if (txtENCaption.Text != "")
      {

        inputParam[3] = new DBParameter("@ENCaption", DbType.AnsiString, 20, "%" + txtENCaption.Text.Replace("'", "''") + "%");
      }

      //   inputParam[4] = new DBParameter("@Activation", DbType.Int32, chkActivation.Checked ? 1 : 0);
      //  inputParam[5] = new DBParameter("@Hidden", DbType.Int32, chkHidden.Checked ? 1 : 0);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spGetColumnAlias_Search", inputParam);
      ugdInformation.DataSource = dtSource;


      lblCount.Text = string.Format("Count: {0}", ugdInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      ucbColumnName.Value = null;
      ucbDocCode.Value = null;
      txtENCaption.Text = string.Empty;
      txtVNCaption.Text = string.Empty;

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

    private bool CheckValid()
    {

      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        UltraGridRow row = ugdInformation.Rows[i];
        row.Selected = false;

        if (row.Cells["DocCode"].Value.ToString() == "")
        {
          row.Cells["DocCode"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText(" Cột Mã DOC không được bỏ trống !!!");
          row.Selected = true;
          ugdInformation.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (row.Cells["ColumnName"].Value.ToString() == "")
        {
          row.Cells["ColumnName"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText(" Tên Cột không được bỏ trống !!!");
          row.Selected = true;
          ugdInformation.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }

        if (row.Cells["VNCaption"].Value.ToString() == "")
        {
          row.Cells["VNCaption"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText(" Cột Item Định Nghĩa VN không được bỏ trống !!!");
          row.Selected = true;
          ugdInformation.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (row.Cells["ENCaption"].Value.ToString() == "")
        {
          row.Cells["ENCaption"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText(" Cột Item Định Nghĩa EN không được bỏ trống !!!");
          row.Selected = true;
          ugdInformation.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (DBConvert.ParseDouble(row.Cells["Index"].Value) < 0)
        {
          row.Cells["Index"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageErrorFromText("Cột Index không được bỏ trống !!!");
          row.Selected = true;
          ugdInformation.ActiveRowScrollRegion.FirstRow = row;
          return false;
        }
        if (row.Cells["Style"].Value.ToString() != "")
        {
          if (DBConvert.ParseDouble(row.Cells["Style"].Value) < 0)
          {
            row.Cells["Style"].Appearance.BackColor = Color.Yellow;
            WindowUtinity.ShowMessageErrorFromText("Cột Style không đúng định dạng");
            row.Selected = true;
            ugdInformation.ActiveRowScrollRegion.FirstRow = row;
            return false;
          }
        }
      }
      return true;
    }

    bool SaveFromGridView()
    {

      if (this.CheckValid())
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("spColumnAlias_Delete", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ugdInformation.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[10];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@ColumnName", DbType.String, row["ColumnName"].ToString());
            inputParam[2] = new DBParameter("@VNCaption", DbType.String, row["VNCaption"].ToString());
            inputParam[3] = new DBParameter("@ENCaption", DbType.String, row["ENCaption"].ToString());
            inputParam[4] = new DBParameter("@DocCode", DbType.String, row["DocCode"].ToString());
            inputParam[5] = new DBParameter("@Index", DbType.Int32, DBConvert.ParseInt(row["Index"].ToString()));
            inputParam[6] = new DBParameter("@Hidden", DbType.Int32, DBConvert.ParseInt(row["Hidden"].ToString() == "True" ? "1" : "0"));
            inputParam[7] = new DBParameter("@Activation", DbType.Int32, DBConvert.ParseInt(row["Activation"].ToString() == "True" ? "1" : "0"));
            inputParam[8] = new DBParameter("@Format", DbType.String, row["Format"].ToString());
            string a = row["Style"].ToString();
            inputParam[9] = new DBParameter("@Style", DbType.String, row["Style"].ToString());

            DataBaseAccess.ExecuteStoreProcedure("spTblGNRColumnAlias_Save", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.SearchData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001");
        this.SaveSuccess = false;
      }
      return true;
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveFromGridView();
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
    public viewGNR_03_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void view_SearchSave_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(gpbSearch);

      //Init Data
      this.InitData();
      SearchData();
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
        this.SearchData();
      }
    }

    private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {

      Utility.InitLayout_UltraGrid(ugdInformation);
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        ugdInformation.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      //show combobox on gridview    
      e.Layout.Bands[0].Columns["DocCode"].ValueList = ucbddDocCode;


      //allow add new
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      //hide
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      // Set caption column
      e.Layout.Bands[0].Columns["ColumnName"].Header.Caption = "Column Name";
      e.Layout.Bands[0].Columns["VNCaption"].Header.Caption = "VN Caption";
      e.Layout.Bands[0].Columns["ENCaption"].Header.Caption = "EN Caption";
      e.Layout.Bands[0].Columns["DocCode"].Header.Caption = "Doc Code";
      e.Layout.Bands[0].Columns["Index"].Header.Caption = "Index";
      e.Layout.Bands[0].Columns["Hidden"].Header.Caption = "Hidden";
      e.Layout.Bands[0].Columns["Activation"].Header.Caption = "Activation";
      e.Layout.Bands[0].Columns["Format"].Header.Caption = "Format";
      e.Layout.Bands[0].Columns["Style"].Header.Caption = "Style";



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
      //TOtal

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "Tổng = {0:#,##0.##}";
      e.Layout.Bands[0].SummaryFooterCaption = "Tổng:";
      */
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = false;

      if (this.CheckValid())
      {


        success = this.SaveFromGridView();
        this.SearchData();
      }


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
      //switch (colName)
      //{
      //    case "CompCode":
      //        WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //        e.Cancel = true;
      //        break;
      //    default:
      //        break;
      //}
    }

    private void ugdInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
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

    #endregion event

  }
}
