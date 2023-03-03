/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: view_ExtraControl.cs
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
using System.IO;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class view_ExtraControl : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(view_ExtraControl).Assembly);
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //Utility.LoadUltraCombo();
      //Utility.LoadUltraDropDown();
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

    private void LoadData()
    {
    }

    private bool CheckValid(out string message)
    {
      message = string.Empty;
      //if (ultCBWo.Text.Length == 0)
      //{
      //  errorMessage = "Work Order";      
      //  return false;
      //}

      ////check detail      
      //for (int i = 0; i < ugdInformation.Rows.Count; i++)
      //{
      //  UltraGridRow row = ugdInformation.Rows[i];
      //  row.Selected = false;
      //  if (DBConvert.ParseDouble(row.Cells["Qty"].Value) <= 0)
      //  {
      //    row.Cells["Qty"].Appearance.BackColor = Color.Yellow;
      //    message = string.Format("{0} không được để trống và phải lớn hơn 0", row.Cells["Qty"].Column.Header.Caption);
      //    row.Selected = true;
      //    ugdInformation.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }
      //  if (DBConvert.ParseDouble(row.Cells["Price"].Value) < 0)
      //  {
      //    row.Cells["Price"].Appearance.BackColor = Color.Yellow;
      //    message = string.Format("{0} không được để trống và phải lớn hơn hoặc bằng 0", row.Cells["Price"].Column.Header.Caption);
      //    row.Selected = true;
      //    ugdInformation.ActiveRowScrollRegion.FirstRow = row;
      //    return false;
      //  }
      //}
      //// Check total proposal <= debit amount of PO or Invoice
      //DataTable dtDetail = new DataTable();
      //dtDetail.Columns.Add("Pid", typeof(long));
      //dtDetail.Columns.Add("EnquiryConfirmDetailPid", typeof(long));
      //dtDetail.Columns.Add("Qty", typeof(double));

      //DataTable dtSource = (DataTable)ugdInformation.DataSource;
      //foreach (DataRow row in dtSource.Rows)
      //{
      //  if (row.RowState != DataRowState.Deleted)
      //  {
      //    DataRow rowDetail = dtDetail.NewRow();
      //    rowDetail["Pid"] = row["Pid"];
      //    rowDetail["EnquiryConfirmDetailPid"] = row["EnquiryConfirmDetailPid"];
      //    rowDetail["Qty"] = row["Qty"];
      //    dtDetail.Rows.Add(rowDetail);
      //  }
      //}
      //int paramNumber = 2;
      //string storeName = "spCSDSaleOrder_CheckInvalid";

      //SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      //inputParam[0] = new SqlDBParameter("@AddedEnquiryList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtDetail));
      //inputParam[1] = new SqlDBParameter("@SaleOrderPid", SqlDbType.BigInt, this.saleOrderPid);
      //SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@ErrorMessage", SqlDbType.NVarChar, string.Empty) };
      //SqlDataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      //if (outputParam != null && outputParam[0].Value.ToString().Length > 0)
      //{
      //  message = outputParam[0].Value.ToString();
      //  return false;
      //}
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("", deleteParam, outputParam);
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
            DBParameter[] inputParam = new DBParameter[7];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[6] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("", inputParam, outputParam);
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
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    private void FillDataForGrid(DataTable dtItemList)
    {
      //DataTable dtItemCheck = (DataTable)ultraDDItemList.DataSource;
      //DataTable dtItemSource = (DataTable)ultData.DataSource;
      //foreach (DataRow row in dtItemList.Rows)
      //{
      //  string itemCode = row[0].ToString().Trim();
      //  if (itemCode.Length > 0)
      //  {
      //    DataRow rowItemSource = dtItemSource.NewRow();
      //    rowItemSource["ItemCode"] = itemCode;
      //    DataRow[] rows = dtItemCheck.Select(string.Format("ItemCode = '{0}'", itemCode));
      //    if (rows.Length == 0)
      //    {
      //      rowItemSource["StatusText"] = "Invalid";
      //      rowItemSource["StatusValue"] = 0;
      //    }
      //    else
      //    {
      //      rowItemSource["ItemName"] = rows[0]["ItemName"];
      //      rowItemSource["ActiveRevision"] = rows[0]["ActiveRevision"];
      //      rowItemSource["StatusText"] = rows[0]["StatusText"];
      //      rowItemSource["StatusValue"] = rows[0]["StatusValue"];
      //      rowItemSource["RevisionPid"] = rows[0]["RevisionPid"];
      //    }
      //    dtItemSource.Rows.Add(rowItemSource);
      //  }
      //}
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
      //btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);      

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public view_ExtraControl()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void view_ExtraControl_Load(object sender, EventArgs e)
    {
      Utility.Format_UltraNumericEditor(tlpForm);
      //Init Data
      this.InitData();
      // Set Language
      this.SetLanguage();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Utility.InitLayout_UltraGrid(ugdInformation);      
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        ugdInformation.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }
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
      //Total
      
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

    private void ugdInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [Items (1)$E3:E4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [Items (1)$B5:B{0}]", itemCount));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.FillDataForGrid(dsItemList.Tables[0]);
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      //string templateName = "ItemListTemplate";
      //string sheetName = "Items";
      //string outFileName = "Items Template";
      //string startupPath = System.Windows.Forms.Application.StartupPath;
      //string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      //string pathTemplate = startupPath + @"\ExcelTemplate\Technical";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      //oXlsReport.Out.File(outFileName);
      //Process.Start(outFileName);
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
    // Auto select the same condition
    //    private void ultDataFOU_CellChange(object sender, CellEventArgs e)
    //    {
    //      if (string.Compare(e.Cell.Column.ToString(), "Approved", true) == 0)
    //      {
    //        int selected = DBConvert.ParseInt(e.Cell.Text);
    //        UltraGridRow row = e.Cell.Row;
    //        long wo = DBConvert.ParseLong(row.Cells["Wo"].Value);
    //        string itemCode = row.Cells["ItemCode"].Value.ToString();
    //        int revision = DBConvert.ParseInt(row.Cells["Revision"].Value);
    //        string compCode = row.Cells["CarcassCode"].Value.ToString();
    //        long changeDeadlineDetailPid = DBConvert.ParseLong(row.Cells["ChangeDeadlineDetailPid"].Value);

    //        DataTable dtFOU = ((DataSet)ultDataFOU.DataSource).Tables[0];
    //        DataRow[] arrRows = dtFOU.Select(string.Format(@"Wo = {0} And ChangeDeadlineDetailPid <> {1} And ItemCode = '{2}'
    //            And Revision = {3} And CarcassCode = '{4}'", wo, changeDeadlineDetailPid, itemCode, revision, compCode));
    //        if (arrRows.Length > 0)
    //        {
    //          foreach (DataRow r in arrRows)
    //          {
    //            r["Approved"] = selected;
    //          }
    //        }
    //      }
    //    }

    private void btnAddItem_Click(object sender, EventArgs e)
    {
      //DataTable dtSource = (DataTable)ugdInformation.DataSource;

      //DataTable dtAddedEnquiryList = new DataTable();
      //dtAddedEnquiryList.Columns.Add("EnquiryConfirmDetailPid", typeof(long));
      //DataRow[] rows = dtSource.Select("EnquiryConfirmDetailPid is not null");
      //foreach (DataRow row in dtSource.Rows)
      //{
      //  if (row.RowState != DataRowState.Deleted)
      //  {
      //    DataRow rowDetail = dtAddedEnquiryList.NewRow();
      //    rowDetail["EnquiryConfirmDetailPid"] = row["EnquiryConfirmDetailPid"];
      //    dtAddedEnquiryList.Rows.Add(rowDetail);
      //  }
      //}

      //viewCSD_05_007 view = new viewCSD_05_007();
      //view.customerPid = customerPid;
      //view.customerDirect = directCus;
      //view.type = type;
      //view.dtAddedEnquiryList = dtAddedEnquiryList;
      //view.dtExistingSource = dtSource;
      //DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "Thêm Sản Phẩm", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
    }
    #endregion event
  }
}
