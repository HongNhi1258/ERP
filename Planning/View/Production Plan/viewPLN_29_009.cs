/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewPLN_29_009.cs
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_29_009 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    private bool flagImport = false;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
    }

    /// <summary>
    /// Import Data
    /// </summary>
    private void Brown()
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtPath.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    /// <summary>
    /// Import Data
    /// </summary>
    private void GetTemplate()
    {
      string templateName = "Template Import Move Container";
      string sheetName = "Sheet1";
      string outFileName = "List Suggest Move Container";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      string storeName = "spPLNSuggestMovingContainer";

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, 60000, null);
      ultraGridInformation.DataSource = dtSource;
      ultraGridInformation.DisplayLayout.Bands[0].Columns["CARLock"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ultraGridInformation.DisplayLayout.Bands[0].Columns["CARConfirmed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            
      ultraGridInformation.DisplayLayout.Bands[0].Columns["IsNotYetPutOnContainer"].Hidden = true;

      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(row.Cells["IsNotYetPutOnContainer"].Value.ToString()) == 1)
        {
          row.Appearance.BackColor = Color.Yellow;
        }
      }
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Deadline Result After Import
    /// </summary>
    /// <returns></returns>
    private DataTable dtMovingContainer()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("KeyInput", typeof(System.String));
      dt.Columns.Add("IsConfirmedPAKDeadline", typeof(System.Int32));
      dt.Columns.Add("SuggestPAKDeadline", typeof(System.DateTime));
      dt.Columns.Add("ReasonSuggestPAKDeadline", typeof(System.Int32));
      dt.Columns.Add("RemarkSuggestPAKDeadline", typeof(System.String));

      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("SO", typeof(System.String));

      dt.Columns.Add("LoadingListFrom", typeof(System.String));
      dt.Columns.Add("LoadingListTo", typeof(System.String));
      dt.Columns.Add("ReasonMovingLoadingList", typeof(System.Int32));
      dt.Columns.Add("UpdateLoadingDate", typeof(System.DateTime));
      dt.Columns.Add("UpdateOriginalLoadingDate", typeof(System.DateTime));

      dt.Columns.Add("OldESD", typeof(System.DateTime));
      dt.Columns.Add("NewESD", typeof(System.DateTime));
      dt.Columns.Add("ReasonChangeESD", typeof(System.Int32));

      dt.Columns.Add("Remark", typeof(System.String));
      dt.Columns.Add("ContainerRemark", typeof(System.String));
      dt.Columns.Add("IsConfirmedSUBDeadline", typeof(System.Int32));
      return dt;
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

    /// <summary>
    /// Import Data
    /// </summary>
    private void ImportData()
    {
      if (this.txtPath.Text.Trim().Length == 0)
      {
        return;
      }
      // Get Data Table From Excel
      DataTable dtSource = new DataTable();
      dtSource = FunctionUtility.GetExcelToDataSet(txtPath.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B5:U1000]").Tables[0];
      //dtSource = FunctionUtility.GetDataFromExcel(txtPath.Text.Trim(), "Sheet1 (1)", "B5:S500");
      if (dtSource == null)
      {
        return;
      }

      // Input ------- 
      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      DataTable dtMovingContainerInput = this.dtMovingContainer();

      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtMovingContainerInput.NewRow();
        if (row["ItemCode"].ToString().Trim().Length > 0 || row["Key Input"].ToString().Trim().Length > 0 || row["Move To _Loading List"].ToString().Trim().Length > 0)
        {
          // KeyInput
          if (row["Key Input"].ToString().Trim().Length > 0)
          {
            rowadd["KeyInput"] = row["Key Input"];
          }

          // is confirm pak deadline
          if (DBConvert.ParseInt(row["Is _Confirmed _PAK Deadline"].ToString()) != int.MinValue)
          {
            rowadd["IsConfirmedPAKDeadline"] = DBConvert.ParseInt(row["Is _Confirmed _PAK Deadline"]);
          }

          // is confirm SUB deadline
          if (DBConvert.ParseInt(row["Is _Confirmed _SUB Deadline"].ToString()) != int.MinValue)
          {
            rowadd["IsConfirmedSUBDeadline"] = DBConvert.ParseInt(row["Is _Confirmed _SUB Deadline"]);
          }

          // suggest PAK Deadline
          if (row["Suggest _PAK Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["SuggestPAKDeadline"] = DBConvert.ParseDateTime(row["Suggest _PAK Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // Reason suggest pak deadline
          if (DBConvert.ParseInt(row["Reason_Suggest _PAK Deadline"].ToString()) != int.MinValue)
          {
            rowadd["ReasonSuggestPAKDeadline"] = DBConvert.ParseInt(row["Reason_Suggest _PAK Deadline"]);
          }

          // CON remark
          if (row["Remark_Suggest _PAK Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["RemarkSuggestPAKDeadline"] = row["Remark_Suggest _PAK Deadline"];
          }

          // ItemCode
          if (row["ItemCode"].ToString().Trim().Length > 0)
          {
            rowadd["ItemCode"] = row["ItemCode"];
          }

          // Revision
          if (DBConvert.ParseInt(row["Revision"].ToString()) != int.MinValue)
          {
            rowadd["Revision"] = DBConvert.ParseInt(row["Revision"]);
          }

          // Qty
          if (DBConvert.ParseInt(row["Move Qty"].ToString()) != int.MinValue)
          {
            rowadd["Qty"] = DBConvert.ParseInt(row["Move Qty"]);
          }

          // SO
          if (row["SONo"].ToString().Trim().Length > 0)
          {
            rowadd["SO"] = row["SONo"];
          }

          // Loading list from
          if (row["Loading List_ From"].ToString().Trim().Length > 0)
          {
            rowadd["LoadingListFrom"] = row["Loading List_ From"];
          }

          // Loading list to
          if (row["Move To _Loading List"].ToString().Trim().Length > 0)
          {
            rowadd["LoadingListTo"] = row["Move To _Loading List"];
          }

          // Reason Moving Container
          if (DBConvert.ParseInt(row["Reason _Move Loading List"].ToString()) != int.MinValue)
          {
            rowadd["ReasonMovingLoadingList"] = DBConvert.ParseInt(row["Reason _Move Loading List"]);
          }

          // Update Loading date
          if (row["Update_LoadingDate"].ToString().Trim().Length > 0)
          {
            rowadd["UpdateLoadingDate"] = DBConvert.ParseDateTime(row["Update_LoadingDate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // Update Original Loading date
          if (row["Update_Original_LoadingDate"].ToString().Trim().Length > 0)
          {
            rowadd["UpdateOriginalLoadingDate"] = DBConvert.ParseDateTime(row["Update_Original_LoadingDate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // Old ESD
          if (row["Old _Confirmed Shipdate"].ToString().Trim().Length > 0)
          {
            rowadd["OldESD"] = DBConvert.ParseDateTime(row["Old _Confirmed Shipdate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // New ESD
          if (row["New _Confirm Shipdate"].ToString().Trim().Length > 0)
          {
            rowadd["NewESD"] = DBConvert.ParseDateTime(row["New _Confirm Shipdate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // Reason change ESD
          if (DBConvert.ParseInt(row["Reason _Change Confirm Shipdate"].ToString()) != int.MinValue)
          {
            rowadd["ReasonChangeESD"] = DBConvert.ParseInt(row["Reason _Change Confirm Shipdate"]);
          }

          // CON remark
          if (row["Container Remark"].ToString().Trim().Length > 0)
          {
            rowadd["ContainerRemark"] = row["Container Remark"];
          }

          // CON remark
          if (row["Remarks"].ToString().Trim().Length > 0)
          {
            rowadd["Remark"] = row["Remarks"];
          }

          //Add row datatable
          dtMovingContainerInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@MovingContainer", SqlDbType.Structured, dtMovingContainerInput);
      DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNSuggestMovingContainer_Import", 1000, sqlinput);
      ultraGridInformation.DataSource = dtResultDeadline;

      ultraGridInformation.DisplayLayout.Bands[0].Columns["IsConfirmedPAKDeadline"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ultraGridInformation.DisplayLayout.Bands[0].Columns["IsConfirmedSUBDeadline"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridInformation.Rows[i];
        if (row.Cells["ErrorDisplay"].Value.ToString().Length > 0)
        {
          row.Cells["ErrorDisplay"].Appearance.BackColor = Color.Red;
        }
      }
    }

    private bool CheckValid()
    {
      //for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      //{
      //  if (ultraGridInformation.Rows[i].Cells["ErrorDisplay"].Value.ToString().Trim().Length > 0)
      //  {
      //    WindowUtinity.ShowMessageError("ERR0050", "Error");
      //    return false;
      //  }
      //}
      return true;
    }

    private bool Save()
    {
      DataTable dtMovingContainerInput = this.dtMovingContainer();
      DataTable dtSource = (DataTable)ultraGridInformation.DataSource;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtMovingContainerInput.NewRow();
        if (row["ItemCode"].ToString().Trim().Length > 0 || row["KeyInput"].ToString().Trim().Length > 0 || row["LoadingListTo"].ToString().Trim().Length > 0)
        {
          // KeyInput
          if (row["KeyInput"].ToString().Trim().Length > 0)
          {
            rowadd["KeyInput"] = row["KeyInput"];
          }

          // is confirm pak deadline
          //if (DBConvert.ParseInt(row["IsConfirmedPAKDeadline"].ToString()) != int.MinValue)
          //{
          //  rowadd["IsConfirmedPAKDeadline"] = DBConvert.ParseInt(row["IsConfirmedPAKDeadline"]);
          //}

          // is confirm pak deadline
          //if (DBConvert.ParseInt(row["IsConfirmedSUBDeadline"].ToString()) != int.MinValue)
          //{
          //  rowadd["IsConfirmedSUBDeadline"] = DBConvert.ParseInt(row["IsConfirmedSUBDeadline"]);
          //}

          // suggest PAK Deadline
          //if (row["SuggestPAKDeadline"].ToString().Trim().Length > 0)
          //{
          //  rowadd["SuggestPAKDeadline"] = DBConvert.ParseDateTime(row["SuggestPAKDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          //}

          // Reason suggest pak deadline
          //if (DBConvert.ParseInt(row["ReasonSuggestPAKDeadline"].ToString()) != int.MinValue)
          //{
          //  rowadd["ReasonSuggestPAKDeadline"] = DBConvert.ParseInt(row["ReasonSuggestPAKDeadline"]);
          //}

          // Remark suggest pak deadline
          //if (row["RemarkSuggestPAKDeadline"].ToString().Trim().Length > 0)
          //{
          //  rowadd["RemarkSuggestPAKDeadline"] = row["RemarkSuggestPAKDeadline"];
          //}

          // ItemCode
          if (row["ItemCode"].ToString().Trim().Length > 0)
          {
            rowadd["ItemCode"] = row["ItemCode"];
          }

          // Revision
          if (DBConvert.ParseInt(row["Revision"].ToString()) != int.MinValue)
          {
            rowadd["Revision"] = DBConvert.ParseInt(row["Revision"]);
          }

          // Qty
          if (DBConvert.ParseInt(row["Qty"].ToString()) != int.MinValue)
          {
            rowadd["Qty"] = DBConvert.ParseInt(row["Qty"]);
          }

          // SO
          if (row["SO"].ToString().Trim().Length > 0)
          {
            rowadd["SO"] = row["SO"];
          }

          // Loading list from
          if (row["LoadingListFrom"].ToString().Trim().Length > 0)
          {
            rowadd["LoadingListFrom"] = row["LoadingListFrom"];
          }

          // Loading list to
          if (row["LoadingListTo"].ToString().Trim().Length > 0)
          {
            rowadd["LoadingListTo"] = row["LoadingListTo"];
          }

          // Reason Moving Container
          if (DBConvert.ParseInt(row["ReasonMovingLoadingList"].ToString()) != int.MinValue)
          {
            rowadd["ReasonMovingLoadingList"] = DBConvert.ParseInt(row["ReasonMovingLoadingList"]);
          }

          // Update Loading Date
          if (row["UpdateLoadingDate"].ToString().Trim().Length > 0)
          {
            rowadd["UpdateLoadingDate"] = DBConvert.ParseDateTime(row["UpdateLoadingDate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // Update Original Loading Date
          if (row["UpdateOriginalLoadingDate"].ToString().Trim().Length > 0)
          {
            rowadd["UpdateOriginalLoadingDate"] = DBConvert.ParseDateTime(row["UpdateOriginalLoadingDate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // Old ESD
          if (row["OldESD"].ToString().Trim().Length > 0)
          {
            rowadd["OldESD"] = DBConvert.ParseDateTime(row["OldESD"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // New ESD
          if (row["NewESD"].ToString().Trim().Length > 0)
          {
            rowadd["NewESD"] = DBConvert.ParseDateTime(row["NewESD"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // Reason change ESD
          if (DBConvert.ParseInt(row["ReasonChangeESD"].ToString()) != int.MinValue)
          {
            rowadd["ReasonChangeESD"] = DBConvert.ParseInt(row["ReasonChangeESD"]);
          }

          // CON remark
          if (row["ContainerRemark"].ToString().Trim().Length > 0)
          {
            rowadd["ContainerRemark"] = row["ContainerRemark"];
          }

          // CON remark
          if (row["Remark"].ToString().Trim().Length > 0)
          {
            rowadd["Remark"] = row["Remark"];
          }

          //Add row datatable
          dtMovingContainerInput.Rows.Add(rowadd);
        }
      }
      //Input
      SqlDBParameter[] sqlinput = new SqlDBParameter[2];
      sqlinput[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtMovingContainerInput);
      sqlinput[1] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      // Output ------
      SqlDBParameter[] sqloutput = new SqlDBParameter[1];
      sqloutput[0] = new SqlDBParameter("@Result", SqlDbType.Int, int.MinValue);

      // Result ------
      SqlDataBaseAccess.ExecuteStoreProcedure("spPLNSuggestMovingContainer_Insert", 1000, sqlinput, sqloutput);
      long result = DBConvert.ParseLong(sqloutput[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.Save();
    }

    private System.Data.DataTable CreateDatatableTransaction()
    {
      System.Data.DataTable dt = new System.Data.DataTable();
      dt.Columns.Add("HangTag", typeof(System.String));
      return dt;
    }
    #endregion function

    #region event
    public viewPLN_29_009()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_29_009_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.GetTemplate();
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

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.UseFixedHeaders = true;
      e.Layout.Bands[0].ColHeaderLines = 2;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      // Tạm thời chưa cần xem nên ẩn đi
      e.Layout.Bands[0].Columns["ENSOTransactionNo"].Hidden = true;
      e.Layout.Bands[0].Columns["KeyInput"].Hidden = true;
      e.Layout.Bands[0].Columns["USCollection"].Hidden = true;

      e.Layout.Bands[0].Columns["INTCollection"].Header.Caption = "Collection";

      if (this.flagImport == true)
      {
        e.Layout.Bands[0].ColHeaderLines = 3;

        e.Layout.Bands[0].Columns["ReasonSuggestPAKDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["ReasonMovingLoadingList"].Hidden = true;
        e.Layout.Bands[0].Columns["ReasonChangeESD"].Hidden = true;

        e.Layout.Bands[0].Columns["ErrorDisplay"].Header.Caption = "Errors";
        e.Layout.Bands[0].Columns["IsConfirmedPAKDeadline"].Header.Caption = "Is\nConfirmed\nPAKDeadline";
        e.Layout.Bands[0].Columns["SuggestPAKDeadline"].Header.Caption = "Suggest\nPAKDeadline";
        e.Layout.Bands[0].Columns["TextReasonSuggest"].Header.Caption = "Reason\nSuggest\nPAKDeadline";
        e.Layout.Bands[0].Columns["RemarkSuggestPAKDeadline"].Header.Caption = "Remark\nSuggest\nPAKDeadline";
        e.Layout.Bands[0].Columns["IsConfirmedSUBDeadline"].Header.Caption = "Is\nConfirmed\nSUBCONDeadline";
                
        e.Layout.Bands[0].Columns["LoadingListFrom"].Header.Caption = "LoadingList\nFrom";
        e.Layout.Bands[0].Columns["LoadingListTo"].Header.Caption = "LoadingList\nTo";
        e.Layout.Bands[0].Columns["TextReasonMove"].Header.Caption = "Reason\nMoving\nLoadingList";
        e.Layout.Bands[0].Columns["ContainerRemark"].Header.Caption = "LoadingList\nRemark";

        e.Layout.Bands[0].Columns["TextReasonChangeShipdate"].Header.Caption = "Reason\nChange\nEstimateShipdate";

        e.Layout.Bands[0].Columns["KeyInput"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
        e.Layout.Bands[0].Columns["IsConfirmedPAKDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
        e.Layout.Bands[0].Columns["SuggestPAKDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
        e.Layout.Bands[0].Columns["TextReasonSuggest"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
        e.Layout.Bands[0].Columns["RemarkSuggestPAKDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);

        e.Layout.Bands[0].Columns["IsConfirmedSUBDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);

        e.Layout.Bands[0].Columns["LoadingListFrom"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
        e.Layout.Bands[0].Columns["LoadingListTo"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
        e.Layout.Bands[0].Columns["TextReasonMove"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
        e.Layout.Bands[0].Columns["UpdateLoadingDate"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
        e.Layout.Bands[0].Columns["UpdateOriginalLoadingDate"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
        e.Layout.Bands[0].Columns["ContainerRemark"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);

        e.Layout.Bands[0].Columns["OldESD"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
        e.Layout.Bands[0].Columns["NewESD"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
        e.Layout.Bands[0].Columns["TextReasonChangeShipdate"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
        e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      }
      else
      {
        e.Layout.Bands[0].Columns["MakeTransaction"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        e.Layout.Bands[0].Columns["MakeTransaction"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      btnSave.Enabled = false;
      if (this.CheckValid())
      {
        bool success = this.Save();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          ultraGridInformation.DataSource = null;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");

        }
      }
      btnSave.Enabled = true;
      //this.SearchData();
    }

    private void btnAddRemove_Click(object sender, EventArgs e)
    {
      System.Data.DataTable dtInput = this.CreateDatatableTransaction();
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        DataRow rowadd = dtInput.NewRow();
        if (ultraGridInformation.Rows[i].IsFilteredOut == false && DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["MakeTransaction"].Value.ToString()) == 1)
        {
          //StringHangtag
          if (ultraGridInformation.Rows[i].Cells["KeyInput"].Value.ToString().Trim().Length > 0)
          {
            rowadd["HangTag"] = ultraGridInformation.Rows[i].Cells["KeyInput"].Value.ToString();
          }
          dtInput.Rows.Add(rowadd);
        }
      }
      viewPLN_29_008 view = new viewPLN_29_008();
      view.dtTable = dtInput;
      Shared.Utility.WindowUtinity.ShowView(view, "UPDATE CARCASS WORK", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, "Deadline Information");
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      btnRefresh.Enabled = false;
      DataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanHardWareStatus_Capture", 600, null, null);
      DataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanMaterialStatus_Capture", 600, null, null);
      btnRefresh.Enabled = true;
      this.SearchData();
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        ControlUtility.GetDataForClipboard(ultraGridInformation);
      }
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultraGridInformation.Selected.Rows.Count > 0 || ultraGridInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultraGridInformation, new Point(e.X, e.Y));
        }
      }
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      this.Brown();
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      this.flagImport = true;
      this.ImportData();
      this.flagImport = false;
    }

    private void btnRefreshDeadline_Click(object sender, EventArgs e)
    {
      btnRefreshDeadline.Enabled = false;
      DataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanRefreshDeadline", 1000, null, null);
      btnRefreshDeadline.Enabled = true;
      this.SearchData();
    }

    private void btnAddContainer_Click(object sender, EventArgs e)
    {
      viewPLN_98_010 view = new viewPLN_98_010();
      Shared.Utility.WindowUtinity.ShowView(view, "ADD CONTAINER", true, DaiCo.Shared.Utility.ViewState.ModalWindow);
    }    
    #endregion event
  }
}
