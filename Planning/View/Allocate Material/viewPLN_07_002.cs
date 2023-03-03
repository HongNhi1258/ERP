/*
 * Create By:   Nguyen Van Tron
 * Create Date: 01/02/2011
 * Description: Forecast Material of Planning
 * Truong Update: Change control report(26/4/2012)
 * Qui Update : Add Export Compponent To Excel (25/7/2012)
 * */

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_002 : MainUserControl
  {
    #region fields
    public long pid = long.MinValue;
    public bool isLoadingData = false;
    private IList listDeletedItem = new ArrayList();
    private bool isResetRevision = false;
    private string pathTemplate = string.Empty;
    private string pathExport = string.Empty;
    #endregion fields

    #region Function

    private XlsReport InitializeXlsReport(string strTemplateName, string strSheetName, string strOutFileName, out string strOutFileName_4)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    private void LoadData()
    {
      this.isLoadingData = true;
      this.NeedToSave = false;
      this.listDeletedItem = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNForecastMaterialDetail", inputParam);
      if (dsSource != null)
      {
        DataTable dtMain = dsSource.Tables[0];
        if (dtMain != null && dtMain.Rows.Count > 0)
        {
          // Main Info
          txtTittle.Text = dtMain.Rows[0]["Tittle"].ToString();
          txtRemark.Text = dtMain.Rows[0]["Description"].ToString();
          txtCreateBy.Text = dtMain.Rows[0]["CreateBy"].ToString();
          txtCreateDate.Text = dtMain.Rows[0]["CreateDate"].ToString();
        }
        else
        {
          txtCreateBy.Text = SharedObject.UserInfo.EmpName;
          txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        }
        // Detail Info
        DataTable dtDetail = dsSource.Tables[1];
        if (dtDetail != null)
        {
          ultraGridItemList.DataSource = dtDetail;
        }
      }
      lbTotalRecords.Text = string.Format("Total Records: {0}", ultraGridItemList.Rows.Count);
      this.isLoadingData = false;
    }

    private bool checkInvalid()
    {
      if (txtTittle.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Tittle");
        txtTittle.Focus();
        return false;
      }
      // Check invalid detail
      for (int i = 0; i < ultraGridItemList.Rows.Count; i++)
      {
        // Item Code
        string itemCode = ultraGridItemList.Rows[i].Cells["ItemCode"].Value.ToString().Trim();
        if (itemCode.Length == 0)
        {
          WindowUtinity.ShowMessageError("MSG0005", string.Format("Item Code at row {0}", i + 1));
          return false;
        }
        // Revision
        int revision = DBConvert.ParseInt(ultraGridItemList.Rows[i].Cells["Revision"].Value.ToString());
        if (revision == int.MinValue)
        {
          WindowUtinity.ShowMessageError("MSG0005", string.Format("Revision at row {0}", i + 1));
          return false;
        }
        // Qty
        int qty = DBConvert.ParseInt(ultraGridItemList.Rows[i].Cells["Qty"].Value.ToString());
        if (qty <= 0)
        {
          WindowUtinity.ShowMessageError("MSG0005", string.Format("Qty at row {0}", i + 1));
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Forecast Material data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private long SaveMain()
    {
      DBParameter[] inputParam = new DBParameter[4];
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      string storeName = string.Empty;
      if (this.pid != long.MinValue)
      {
        storeName = "spPLNForecastMaterial_Update";
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
        inputParam[3] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }
      else
      {
        storeName = "spPLNForecastMaterial_Insert";
        inputParam[3] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }
      inputParam[1] = new DBParameter("@Tittle", DbType.String, 256, txtTittle.Text.Trim());
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@Description", DbType.String, 1024, txtRemark.Text.Trim());
      }
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      return DBConvert.ParseLong(outputParam[0].Value.ToString());
    }

    /// <summary>
    /// Load WO
    /// </summary>
    private void LoadWO()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid WO";
      commandText += " FROM TblPLNWorkOrder ";
      commandText += " ORDER BY Pid ";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucListWOs.DataSource = dtItem;
      ucListWOs.ColumnWidths = "200";
      ucListWOs.DataBind();
      ucListWOs.ValueMember = "WO";
      ucListWOs.DisplayMember = "WO";
      ucListWOs.AutoSearchBy = "WO";
    }

    /// <summary>
    /// Save Detail Material ForeCast Material
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private bool SaveDetail()
    {
      bool result = true;
      // Delete
      foreach (long pid in this.listDeletedItem)
      {
        DBParameter[] inputDeleted = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputDeleted = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNForecastMaterialDetail_Delete", inputDeleted, outputDeleted);
        if (DBConvert.ParseInt(outputDeleted[0].Value.ToString()) != 1)
        {
          result = false;
        }
      }

      // Insert, update
      DataTable dtSource = (DataTable)ultraGridItemList.DataSource;
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[4];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          string storeName = string.Empty;
          if (row.RowState == DataRowState.Added)
          {
            storeName = "spPLNForecastMaterialDetail_Insert";
            inputParam[0] = new DBParameter("@ForcastMaterialPid", DbType.Int64, this.pid);
          }
          else
          {
            storeName = "spPLNForecastMaterialDetail_Update";
            long pidRow = DBConvert.ParseLong(row["Pid"].ToString());
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pidRow);
          }
          string itemCode = row["ItemCode"].ToString();
          int revision = DBConvert.ParseInt(row["Revision"].ToString());
          int qty = DBConvert.ParseInt(row["Qty"].ToString());
          inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          inputParam[2] = new DBParameter("@Revision", DbType.Int32, revision);
          inputParam[3] = new DBParameter("@Qty", DbType.Int32, qty);
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          if (DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
          {
            result = false;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Save all data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private bool SaveData()
    {
      bool success = true;
      if (this.checkInvalid())
      {
        this.pid = this.SaveMain();
        if (this.pid > 0)
        {
          success = this.SaveDetail();
          if (!success)
          {
            WindowUtinity.ShowMessageError("WRN0004");
            return success;
          }
        }
        else
        {
          success = false;
          WindowUtinity.ShowMessageError("ERR0005");
          return success;
        }
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        success = false;
      }
      return success;
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveSuccess = this.SaveData();
    }

    private UltraDropDown LoadRevision(string itemCode, UltraDropDown ultRevision)
    {
      if (ultRevision == null)
      {
        ultRevision = new UltraDropDown();
        this.Controls.Add(ultRevision);
      }
      string commandText = string.Format(@"SELECT DISTINCT Revision FROM TblBOMItemInfo WHERE ItemCode = '{0}' ORDER BY Revision ASC", itemCode);
      DataTable dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultRevision.DataSource = dt;
      ultRevision.ValueMember = "Revision";
      ultRevision.DisplayMember = "Revision";
      ultRevision.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultRevision.Visible = false;
      return ultRevision;
    }
    #endregion Function

    #region event
    public viewPLN_07_002()
    {
      InitializeComponent();
    }

    private void viewPLN_07_002_Load(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";

      // Load Dropdown Item
      ControlUtility.LoadDropdownItem(ultraDDItem);
      this.LoadWO();
      this.LoadData();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.SaveData())
      {
        this.LoadData();
      }
    }

    private void ultraGridItemList_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridItemList);
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultraDDItem;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 150;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemName"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Item Name";
      e.Layout.Bands[0].Columns["Revision"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 100;
    }

    private void ultraGridItemList_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.NeedToSave = true;
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedItem.Add(pid);
        }
      }
      this.NeedToSave = true;
    }

    private void ultraGridItemList_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      if (string.Compare(columnName, "itemcode", true) == 0)
      {
        string itemCode = e.Cell.Text.Trim();
        if (itemCode.Length > 0)
        {
          string commandTextItem = string.Format("Select ItemCode From tblBOMItemBasic Where ItemCode = '{0}'", itemCode);
          DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandTextItem);
          if (dtItem.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Item Code");
            e.Cancel = true;
          }
        }
      }
      else if (string.Compare(columnName, "revision", true) == 0)
      {
        if (!isResetRevision)
        {
          string itemCode = e.Cell.Row.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(e.Cell.Text);
          if (revision != int.MinValue)
          {
            string commandTextRevision = string.Format("Select ItemCode From TblBOMItemInfo Where ItemCode = '{0}' And Revision = {1}", itemCode, revision);
            DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandTextRevision);
            if (dtItem.Rows.Count == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Revision");
              e.Cancel = true;
            }
          }
        }
      }

    }

    private void ultraGridItemList_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      switch (columnName)
      {
        case "itemcode":
          UltraDropDown ultRevision = (UltraDropDown)e.Cell.Row.Cells["Revision"].ValueList;
          ultRevision = this.LoadRevision(value, ultRevision);
          e.Cell.Row.Cells["Revision"].ValueList = ultRevision;
          if (ultraDDItem.SelectedRow != null)
          {
            e.Cell.Row.Cells["ItemName"].Value = ultraDDItem.SelectedRow.Cells["ItemName"].Value;
          }
          else
          {
            e.Cell.Row.Cells["ItemName"].Value = DBNull.Value;
          }
          this.isResetRevision = true;
          if (ultRevision.Rows.Count > 0)
          {
            e.Cell.Row.Cells["Revision"].Value = ultRevision.Rows[ultRevision.Rows.Count - 1].Cells[0].Value;
          }
          else
          {
            e.Cell.Row.Cells["Revision"].Value = DBNull.Value;
          }
          this.isResetRevision = false;
          break;
        default:
          break;
      }
      if (!this.isLoadingData)
      {
        this.NeedToSave = true;
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridItemList_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string itemCode;
      try
      {
        itemCode = e.Cell.Row.Cells["ItemCode"].Value.ToString();
      }
      catch
      {
        itemCode = "";
      }
      switch (columnName)
      {
        case "revision":
          UltraDropDown ultRevision = (UltraDropDown)e.Cell.Row.Cells["Revision"].ValueList;
          e.Cell.Row.Cells["Revision"].ValueList = this.LoadRevision(itemCode, ultRevision);
          break;
        default:
          break;
      }
    }

    private void txtTittle_TextChanged(object sender, EventArgs e)
    {
      if (!this.isLoadingData)
      {
        this.NeedToSave = true;
      }
    }

    private void chkExportToExcel_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExportToExcel.Checked)
      {
        chkExportComp.Checked = false;
        tableLayoutCompGroup.Visible = chkExportComp.Checked;
        tblLayoutExportCompGroup.Visible = chkExportComp.Checked;
        ControlUtility.LoaducUltraListMaterialGroup(ucUltraListMaterialGroup);
        tblLayoutGroup.Visible = chkExportToExcel.Checked;
        tableLayoutExportExcel.Visible = chkExportToExcel.Checked;
      }
      else
      {
        tblLayoutGroup.Visible = chkExportToExcel.Checked;
        tableLayoutExportExcel.Visible = chkExportToExcel.Checked;
      }
    }

    private void chkExportComp_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExportComp.Checked)
      {
        chkExportToExcel.Checked = false;
        tblLayoutGroup.Visible = chkExportToExcel.Checked;
        tableLayoutExportExcel.Visible = chkExportToExcel.Checked;
        string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 9 AND Code <> 8";
        DataTable dtCompGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
        ControlUtility.LoaducUltraList(ucUltraListCompGroup, dtCompGroup, "0; 300", "Code", "Value");
        ucUltraListCompGroup.AutoSearchBy = "Value";
        tableLayoutCompGroup.Visible = chkExportComp.Checked;
        tblLayoutExportCompGroup.Visible = chkExportComp.Checked;
      }
      else
      {
        tableLayoutCompGroup.Visible = chkExportComp.Checked;
        tblLayoutExportCompGroup.Visible = chkExportComp.Checked;
      }
    }

    /// <summary>
    /// Export data to Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      btnExport.Enabled = false;
      string strTemplateName = "PlanningReport";
      string strSheetName = "ForecastMaterial";
      string strOutFileName = "Forecast Materials Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Search
      DBParameter[] arrInput = new DBParameter[2];
      arrInput[0] = new DBParameter("@ForcastPid", DbType.Int64, this.pid);
      // Truong Add
      if (txtMaterialGroup.Text.Trim().Length > 0)
      {
        arrInput[1] = new DBParameter("@GroupMaterials", DbType.String, 4000, txtMaterialGroup.Text.Trim());
      }
      // End
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNForecastMaterialDimention_Select", 1200, arrInput);
      string commandTextTittle = string.Format("Select Tittle From TblPLNForecastMaterial Where Pid = {0}", this.pid);
      DataTable dtTittle = DataBaseAccess.SearchCommandTextDataTable(commandTextTittle);
      if (dtTittle != null && dtTittle.Rows.Count > 0)
      {
        oXlsReport.Cell("**Tittle").Value = dtTittle.Rows[0][0].ToString();
      }
      oXlsReport.Cell("**PrinDate").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B7:P7").Copy();
            oXlsReport.RowInsert(6 + i);
            oXlsReport.Cell("B7:P7", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**Revision", 0, i).Value = dtRow["Revision"];
          oXlsReport.Cell("**ItemName", 0, i).Value = dtRow["ItemName"];
          oXlsReport.Cell("**QtyItem", 0, i).Value = dtRow["QtyItem"];
          oXlsReport.Cell("**CompCode", 0, i).Value = dtRow["ComponentCode"];
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"];
          oXlsReport.Cell("**MaterialName", 0, i).Value = dtRow["MaterialName"];
          oXlsReport.Cell("**Length", 0, i).Value = dtRow["Length"];
          oXlsReport.Cell("**Width", 0, i).Value = dtRow["Width"];
          oXlsReport.Cell("**Thickness", 0, i).Value = dtRow["Thickness"];
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"];
          oXlsReport.Cell("**Waste", 0, i).Value = dtRow["Waste"];
          oXlsReport.Cell("**QtyPerItem", 0, i).Value = dtRow["QtyPerItem"];
          double qtyPerItem = DBConvert.ParseDouble(dtRow["QtyPerItem"].ToString());
          qtyPerItem = (qtyPerItem == double.MinValue ? 0 : qtyPerItem);
          oXlsReport.Cell("**TotalQty", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyItem"].ToString()) * qtyPerItem;
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
      btnExport.Enabled = true;
    }

    /// <summary>
    /// Export Comp to Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExportComp_Click(object sender, EventArgs e)
    {
      btnExportComp.Enabled = false;
      string strTemplateName = "PlanningReport";
      string strSheetName = "ForecastCompTemplate";
      string strOutFileName = "Forecast Component Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DBParameter[] arrInput = new DBParameter[3];
      arrInput[0] = new DBParameter("@ForcastPid", DbType.Int64, this.pid);
      if (txtCompGroup.Text.Trim().Length > 0)
      {
        arrInput[1] = new DBParameter("@CompGroups", DbType.String, 4000, ucUltraListCompGroup.SelectedValue.Trim());
      }
      arrInput[2] = new DBParameter("@NotMaterial", DbType.Int32, ultChkHasMaterial.Checked ? 1 : 0);
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNForecastCompGroup_Select", 1200, arrInput);
      string commandTextTittle = string.Format("Select Tittle From TblPLNForecastMaterial Where Pid = {0}", this.pid);
      DataTable dtTittle = DataBaseAccess.SearchCommandTextDataTable(commandTextTittle);
      if (dtTittle != null && dtTittle.Rows.Count > 0)
      {
        oXlsReport.Cell("**Tittle").Value = dtTittle.Rows[0][0].ToString();
      }
      oXlsReport.Cell("**PrinDate").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B7:N7").Copy();
            oXlsReport.RowInsert(6 + i);
            oXlsReport.Cell("B7:N7", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = dtRow["No"];
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**Revision", 0, i).Value = dtRow["Revision"];
          oXlsReport.Cell("**ItemName", 0, i).Value = dtRow["ItemName"];
          oXlsReport.Cell("**QtyItem", 0, i).Value = dtRow["QtyItem"];
          oXlsReport.Cell("**CompCode", 0, i).Value = dtRow["ComponentCode"];
          oXlsReport.Cell("**CompName", 0, i).Value = dtRow["Name"];
          oXlsReport.Cell("**Length", 0, i).Value = dtRow["Length"];
          oXlsReport.Cell("**Width", 0, i).Value = dtRow["Width"];
          oXlsReport.Cell("**Thickness", 0, i).Value = dtRow["Thickness"];
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"];
          oXlsReport.Cell("**QtyPerItem", 0, i).Value = dtRow["QtyPerItem"];
          oXlsReport.Cell("**TotalQty", 0, i).Value = dtRow["TotalQty"];
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
      btnExportComp.Enabled = true;
    }
    /// <summary>
    /// Value Change Material Group
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucUltraListMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialGroup.Text = ucUltraListMaterialGroup.SelectedValue;
    }

    private void ucUltraListCompGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtCompGroup.Text = ucUltraListCompGroup.SelectedText;
    }
    /// <summary>
    /// btn Import Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      bool success = true;
      DataTable dtCountRecord = Shared.Utility.FunctionUtility.GetDataFromExcel(txtFilePath.Text.Trim(), "ForecastMaterialTemplate (1)", "G3:G4");
      if (dtCountRecord == null)
      {
        return;
      }
      if (dtCountRecord.Rows.Count == 0 || DBConvert.ParseLong(dtCountRecord.Rows[0][0].ToString()) == long.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Please input max records.");
        return;
      }
      long maxRecords = DBConvert.ParseLong(dtCountRecord.Rows[0][0].ToString());
      DataTable dt = Shared.Utility.FunctionUtility.GetDataFromExcel(txtFilePath.Text.Trim(), "ForecastMaterialTemplate (1)", string.Format("B5:C{0}", maxRecords + 6));
      if (dt == null)
      {
        return;
      }
      DataTable dtSource = (DataTable)ultraGridItemList.DataSource;
      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable("SELECT ITEM.ItemCode, ITEM.Name, MAX(INFO.Revision) Revision FROM TblBOMItemBasic ITEM LEFT JOIN TblBOMItemInfo INFO ON ITEM.ItemCode = INFO.ItemCode GROUP BY ITEM.ItemCode, ITEM.Name");
      if (dtItem == null)
      {
        return;
      }
      foreach (DataRow drRow in dt.Rows)
      {
        string itemCode = drRow[0].ToString().Trim();
        if (itemCode.Length > 0)
        {
          DataRow row = dtSource.NewRow();
          DataRow[] arrRowItem = dtItem.Select(string.Format("ItemCode = '{0}'", itemCode));
          if (arrRowItem.Length > 0)
          {
            // Item Name, Revision
            row["ItemCode"] = arrRowItem[0]["ItemCode"];
            row["ItemName"] = arrRowItem[0]["Name"];
            row["Revision"] = arrRowItem[0]["Revision"];

            // Qty
            int qty = DBConvert.ParseInt(drRow[1].ToString());
            if (qty != int.MinValue)
            {
              row["Qty"] = qty;
            }
            dtSource.Rows.Add(row);
          }
          else
          {
            success = false;
          }
        }
      }
      if (success)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0024");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0027");
      }
      lbTotalRecords.Text = string.Format("Total Records: {0}", ultraGridItemList.Rows.Count);
    }

    /// <summary>
    /// Open File Dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBrowser_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtFilePath.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtFilePath.Text.Trim().Length > 0);
    }

    /// <summary>
    /// btnGetTemplate Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "PlanningReport";
      string sheetName = "ForecastMaterialTemplate";
      string outFileName = "Forecast Materials Template";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void chkWO_CheckedChanged(object sender, EventArgs e)
    {
      ucListWOs.Visible = chkWO.Checked;
    }

    private void ucListWOs_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtListWOs.Text = this.ucListWOs.SelectedValue;
    }

    private void btnAddWO_Click(object sender, EventArgs e)
    {
      DBParameter[] param = new DBParameter[1];
      param[0] = new DBParameter("@ListWOs", DbType.String, this.txtListWOs.Text);

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNForecastMaterialAddListWOs_Insert", param);
      if (dt != null && dt.Rows.Count > 0)
      {
        DataTable dtData = (DataTable)this.ultraGridItemList.DataSource;
        foreach (DataRow row in dt.Rows)
        {
          DataRow rowAdd = dtData.NewRow();
          rowAdd["ItemCode"] = row["ItemCode"].ToString();
          rowAdd["ItemName"] = row["ItemName"].ToString();
          rowAdd["Revision"] = row["Revision"].ToString();
          rowAdd["Qty"] = DBConvert.ParseInt(row["Qty"].ToString());
          dtData.Rows.Add(rowAdd);
        }

        this.ultraGridItemList.DataSource = dtData;
      }
    }
    #endregion event
  }
}
