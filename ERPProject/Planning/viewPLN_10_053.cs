/*
  Author      : Huynh Thi Bang
  Date        : 28/12/2018
  Description : 
  Standard Form: view_SearchSave.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_10_053 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //Load data for combobox WO
      string commandText = "Select Pid From TblPLNWorkOrder Where Confirm = 1 Order By Pid Desc";
      DataTable dtWO = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtWO != null)
      {
        Utility.LoadUltraCombo(ucbWO, dtWO, "Pid", "Pid");
      }

      // Load data for combobox Customer
      Utility.LoadUltraCBCustomer(ucbCustomer);

      udtNgayXuatHangFrom.Value = DBNull.Value;
      udtNgayXuatHangTo.Value = DBNull.Value;

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
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      int paramNumber = 4;
      string storeName = "spPLNListWorkOrderDeadline";

      DBParameter[] inputParam = new DBParameter[paramNumber];

      if (udtNgayXuatHangFrom.Value != null)
      {
        inputParam[0] = new DBParameter("@DateFrom", DbType.DateTime, udtNgayXuatHangFrom.Value);
      }

      if (udtNgayXuatHangTo.Value != null)
      {
        inputParam[1] = new DBParameter("@DateTo", DbType.DateTime, udtNgayXuatHangTo.Value);
      }
      if (ucbWO.SelectedRow != null)
      {
        inputParam[2] = new DBParameter("@Wo", DbType.Int64, DBConvert.ParseLong(ucbWO.Value.ToString()));
      }
      if (ucbCustomer.SelectedRow != null)
      {
        inputParam[3] = new DBParameter("@Customer", DbType.Int64, DBConvert.ParseLong(ucbCustomer.Value.ToString()));
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;

      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        string item = ugdInformation.Rows[i].Cells["ItemCode"].Value.ToString();
        if (item == "")
        {
          ugdInformation.Rows[i].CellAppearance.BackColor = Color.PapayaWhip;
        }
      }

      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      udtNgayXuatHangFrom.Value = DBNull.Value;
      udtNgayXuatHangTo.Value = DBNull.Value;
      ucbWO.Value = DBNull.Value;
      ucbCustomer.Value = DBNull.Value;
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

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      DataTable dtDetail = (DataTable)ugdInformation.DataSource;
      foreach (DataRow row1 in dtDetail.Rows)
      {
        if (row1.RowState == DataRowState.Modified)
        {
          string deadline2 = DBConvert.ParseString(DBConvert.ParseDateTime(row1["FinishMachinery"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
          if (deadline2.Length <= 0)
          {
            errorMessage = "Hoàn Thành Phôi";
            return false;
          }
        }
      }
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        // . Insert/Update      
        DataTable dtDetail = (DataTable)ugdInformation.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[8];

            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            string deadline = DBConvert.ParseString(DBConvert.ParseDateTime(row["StartMachinery"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            if (deadline.Length > 0)
            {
              inputParam[1] = new DBParameter("@StartMachinery", DbType.DateTime, DBConvert.ParseDateTime(row["StartMachinery"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            }
            inputParam[2] = new DBParameter("@FinishMachinery", DbType.DateTime, DBConvert.ParseDateTime(row["FinishMachinery"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            inputParam[3] = new DBParameter("@FinishTrialAssembly", DbType.DateTime, DBConvert.ParseDateTime(row["FinishTrialAssembly"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            inputParam[4] = new DBParameter("@Sanding", DbType.DateTime, DBConvert.ParseDateTime(row["Sanding"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            inputParam[5] = new DBParameter("@Assembly", DbType.DateTime, DBConvert.ParseDateTime(row["Assembly"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            inputParam[6] = new DBParameter("@BenchworkSpraying", DbType.DateTime, DBConvert.ParseDateTime(row["BenchworkSpraying"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            inputParam[7] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

            DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrderDeadline_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SearchData();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }

      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
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
      btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);
      btnClear.Text = rm.GetString("Clear", ConstantClass.CULTURE);
      lbWork.Text = rm.GetString("WorkOrder", ConstantClass.CULTURE);
      lbCustomer.Text = rm.GetString("Customer", ConstantClass.CULTURE);
      lbDate.Text = rm.GetString("DateOrder", ConstantClass.CULTURE);
      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewPLN_10_053()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_10_053_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(gpbSearch);

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
      e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          //e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }

      e.Layout.Bands[0].Columns["CustomerName"].MergedCellEvaluationType = MergedCellEvaluationType.MergeSameText;
      e.Layout.Bands[0].Columns["WorkOrderPid"].MergedCellEvaluationType = MergedCellEvaluationType.MergeSameText;

      for (int j = 0; j < e.Layout.Bands[0].Columns.Count; j++)
      {
        e.Layout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["STT"].Hidden = true;

      e.Layout.Bands[0].Columns["StartMachinery"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["FinishMachinery"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["FinishTrialAssembly"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Sanding"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Assembly"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["BenchworkSpraying"].CellActivation = Activation.AllowEdit;

      // Set color
      e.Layout.Bands[0].Columns["StartMachinery"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["FinishMachinery"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["FinishTrialAssembly"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Sanding"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Assembly"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["BenchworkSpraying"].CellAppearance.BackColor = Color.LightGray;

      // Set caption column
      e.Layout.Bands[0].Columns["CustomerName"].Header.Caption = "Khách Hàng";
      e.Layout.Bands[0].Columns["WorkOrderPid"].Header.Caption = "LSX";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Mã Sản Phẩm";
      e.Layout.Bands[0].Columns["Description"].Header.Caption = "Tên Sản Phẩm";
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Mã Cũ";
      e.Layout.Bands[0].Columns["MaterialWood"].Header.Caption = "Loại Gỗ";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số Lượng";
      e.Layout.Bands[0].Columns["CBMPerItem"].Header.Caption = "KL Tinh 1 SP";
      e.Layout.Bands[0].Columns["CBM"].Header.Caption = "KL Tinh KH";
      e.Layout.Bands[0].Columns["OrderDate"].Header.Caption = "Ngày Xuất Hàng";
      e.Layout.Bands[0].Columns["StartMachinery"].Header.Caption = "BĐ chạy Phôi";
      e.Layout.Bands[0].Columns["FinishMachinery"].Header.Caption = "HT Phôi";
      e.Layout.Bands[0].Columns["FinishTrialAssembly"].Header.Caption = "HT Định Hình";
      e.Layout.Bands[0].Columns["Sanding"].Header.Caption = "Chà Nhám";
      e.Layout.Bands[0].Columns["Assembly"].Header.Caption = "HT Lắp Ráp";
      e.Layout.Bands[0].Columns["BenchworkSpraying"].Header.Caption = "Nguội + Giao Sơn";
      e.Layout.Bands[0].Columns["Color"].Header.Caption = "Màu Sơn";
      e.Layout.Bands[0].Columns["RRW"].Header.Caption = "STT";
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Ghi Chú";

      // Set Width
      e.Layout.Bands[0].Columns["WorkOrderPid"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["WorkOrderPid"].MinWidth = 60;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 80;

      //e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.0000}";
      //e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["CBMPerItem"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.0000}";
      //e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["CBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.0000}";


      /*

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;

      */
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
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
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "FinishMachinery":
          string deadline = DBConvert.ParseString(DBConvert.ParseDateTime(row.Cells["FinishMachinery"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
          if (deadline.Length > 0)
          {
            DateTime dt = DBConvert.ParseDateTime(row.Cells["FinishMachinery"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["FinishTrialAssembly"].Value = dt.AddDays(7);

            DateTime dt1 = DBConvert.ParseDateTime(row.Cells["FinishTrialAssembly"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["Sanding"].Value = dt1.AddDays(3);

            DateTime dt2 = DBConvert.ParseDateTime(row.Cells["Sanding"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["Assembly"].Value = dt2.AddDays(10);

            DateTime dt3 = DBConvert.ParseDateTime(row.Cells["Assembly"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["BenchworkSpraying"].Value = dt3.AddDays(3);
          }
          break;

        case "FinishTrialAssembly":
          string deadline1 = DBConvert.ParseString(DBConvert.ParseDateTime(row.Cells["FinishTrialAssembly"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
          if (deadline1.Length > 0)
          {
            DateTime dt4 = DBConvert.ParseDateTime(row.Cells["FinishTrialAssembly"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["Sanding"].Value = dt4.AddDays(3);

            DateTime dt5 = DBConvert.ParseDateTime(row.Cells["Sanding"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["Assembly"].Value = dt5.AddDays(10);

            DateTime dt6 = DBConvert.ParseDateTime(row.Cells["Assembly"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["BenchworkSpraying"].Value = dt6.AddDays(3);
          }
          break;

        case "Sanding":
          string deadline2 = DBConvert.ParseString(DBConvert.ParseDateTime(row.Cells["Sanding"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
          if (deadline2.Length > 0)
          {
            DateTime dt7 = DBConvert.ParseDateTime(row.Cells["Sanding"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["Assembly"].Value = dt7.AddDays(10);

            DateTime dt8 = DBConvert.ParseDateTime(row.Cells["Assembly"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["BenchworkSpraying"].Value = dt8.AddDays(3);
          }
          break;

        case "Assembly":
          string deadline3 = DBConvert.ParseString(DBConvert.ParseDateTime(row.Cells["Assembly"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
          if (deadline3.Length > 0)
          {
            DateTime dt9 = DBConvert.ParseDateTime(row.Cells["Assembly"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            row.Cells["BenchworkSpraying"].Value = dt9.AddDays(3);
          }
          break;
        default:
          break;
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Data");
    }

    #endregion event
  }
}
