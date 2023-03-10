/*
  Author      : 
  Date        : 14/04/2014
  Description : Connect WO Carcass Contract Out
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Technical;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_02_031 : MainUserControl
  {
    #region Field
    public long wo = long.MinValue;

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;

    private IList listDeletedPid = new ArrayList();
    private IList listUpdateCarcassContractOutPid = new ArrayList();
    #endregion Field

    #region Init
    public viewPLN_02_031()
    {
      InitializeComponent();
    }

    private void viewPLN_02_031_Load(object sender, EventArgs e)
    {
      // Load Supplier
      this.LoadDropDownSupplier();

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@WO", DbType.Int64, this.wo);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNWOCarcassContractOutRelation_Select", input);
      if (ds != null)
      {
        DataSet dsMain = this.CreateDataSet();
        dsMain.Tables["dtParent"].Merge(ds.Tables[0]);
        dsMain.Tables["dtChild"].Merge(ds.Tables[1]);
        // Gan DataSource
        ultData.DataSource = dsMain;

        this.SetStatus();
      }

      //chkExpandAll.Checked = true;
      //ultData.Rows.ExpandAll(true);
    }

    /// <summary>
    /// Set Status
    /// </summary>
    private void SetStatus()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        if (DBConvert.ParseInt(rowParent.Cells["IsTransfer"].Value.ToString()) == 1)
        {
          rowParent.Cells["Transfer"].Activation = Activation.ActivateOnly;
          rowParent.Cells["Transfer"].Appearance.BackColor = Color.Yellow;
        }
        else
        {
          rowParent.Cells["Transfer"].Activation = Activation.AllowEdit;
        }

        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(rowChild.Cells["PlanningConfirm"].Value.ToString()) == 1)
          {
            rowChild.Cells["PlanningConfirm"].Appearance.BackColor = Color.LightSkyBlue;
          }
          else
          {
            rowChild.Cells["PlanningConfirm"].Appearance.BackColor = Color.LightGray;
          }
          int flagDouble = 0;
          // Check Trung
          for (int k = 0; k < rowParent.ChildBands[0].Rows.Count; k++)
          {
            UltraGridRow rowChildCheck = rowParent.ChildBands[0].Rows[k];
            if (DBConvert.ParseLong(rowChild.Cells["WO"].Value.ToString()) == DBConvert.ParseLong(rowChildCheck.Cells["WO"].Value.ToString()) &&
              rowChild.Cells["CarcassCode"].Value.ToString() == rowChildCheck.Cells["CarcassCode"].Value.ToString() &&
              DBConvert.ParseLong(rowChild.Cells["SupplierPid"].Value.ToString()) == DBConvert.ParseLong(rowChildCheck.Cells["SupplierPid"].Value.ToString()))
            {
              flagDouble = flagDouble + 1;
            }
          }
          if (flagDouble == 2)
          {
            rowParent.Appearance.BackColor = Color.Aqua;
          }
          // End
        }
      }
    }

    /// <summary>
    /// Load Supplier
    /// </summary>
    private void LoadDropDownSupplier()
    {
      string commandText = string.Empty;
      commandText = "SELECT Pid, SupplierCode + ' - ' + EnglishName AS Name";
      commandText += " FROM TblPURSupplierInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultddSupplier.DataSource = dtSource;
      ultddSupplier.DisplayMember = "Name";
      ultddSupplier.ValueMember = "Pid";
      ultddSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultddSupplier.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultddSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    #endregion Init

    #region Event
    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtLocation.Text.Trim().Length > 0);
    }

    private void Import_Click(object sender, EventArgs e)
    {
      if (this.txtLocation.Text.Trim().Length == 0)
      {
        return;
      }

      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), "SELECT * FROM [Data (1)$B5:D570]").Tables[0];
        if (dtSource == null)
        {
          return;
        }
        // Enabled = false
        btnImport.Enabled = false;

        // Create Data Table Import
        DataTable dtMain = this.CreateDataTable();
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          if (dtSource.Rows[i]["CarcassCode"].ToString().Length > 0)
          {
            DataRow row = dtMain.NewRow();
            row["CarcassCode"] = dtSource.Rows[i]["CarcassCode"].ToString();
            row["SupplierCode"] = dtSource.Rows[i]["SupplierCode"].ToString();
            row["NetPrice"] = DBConvert.ParseDouble(dtSource.Rows[i]["NetPrice"].ToString());
            //row["ItemQty"] = DBConvert.ParseInt(dtSource.Rows[i]["ItemQty"].ToString());
            dtMain.Rows.Add(row);
          }
        }

        // Import Data
        SqlDBParameter[] input = new SqlDBParameter[2];
        input[0] = new SqlDBParameter("@WO", SqlDbType.BigInt, this.wo);
        input[1] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtMain);

        DataSet dsSource = SqlDataBaseAccess.SearchStoreProcedure("spPLNGetDataWOCarcassContractOutRelation_Select", input);
        if (dtSource != null)
        {
          // Data Result
          DataTable dtResult = dsSource.Tables[0];
          DataTable dtErrors = dsSource.Tables[1];

          // Data Grid
          DataSet dsGrird = (DataSet)ultData.DataSource;
          DataTable dtGrirdDetail = dsGrird.Tables["dtChild"];

          // Data
          for (int i = 0; i < dtResult.Rows.Count; i++)
          {
            // Check Exist In Gird Detail
            DataRow[] foundRow = dtGrirdDetail.Select(string.Format(@"WO = {0} AND CarcassCode = '{1}' AND SupplierPid = {2}", DBConvert.ParseLong(dtResult.Rows[i]["WO"].ToString()), dtResult.Rows[i]["CarcassCode"].ToString(), DBConvert.ParseLong(dtResult.Rows[i]["SupplierPid"].ToString())));
            if (foundRow.Length == 0)
            {
              DataRow rowGird = dtGrirdDetail.NewRow();
              rowGird["WO"] = DBConvert.ParseLong(dtResult.Rows[i]["WO"].ToString());
              rowGird["CarcassCode"] = dtResult.Rows[i]["CarcassCode"].ToString();
              rowGird["SupplierPid"] = DBConvert.ParseLong(dtResult.Rows[i]["SupplierPid"].ToString());
              rowGird["NetPrice"] = DBConvert.ParseLong(dtResult.Rows[i]["NetPrice"].ToString());
              rowGird["ItemQty"] = DBConvert.ParseInt(dtResult.Rows[i]["ItemQty"].ToString());
              rowGird["IsUpdate"] = 1;
              dtGrirdDetail.Rows.Add(rowGird);
            }
          }

          // Errors
          string errors = string.Empty;
          for (int j = 0; j < dtErrors.Rows.Count; j++)
          {
            errors = errors + "(" + dtErrors.Rows[j]["Name"].ToString() + "), ";
          }
          if (errors.Trim().Length > 0)
          {
            labErrors.Text = errors;
          }
        }

        // Enable = true
        btnImport.Enabled = true;
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_PLN_02_031";
      string sheetName = "Data";
      string outFileName = "IMPORT DATA CARCASS CONTRACT OUT";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        success = this.SaveMappingCarcassContractOut();
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Load Data
      this.listDeletedPid = new ArrayList();
      this.listUpdateCarcassContractOutPid = new ArrayList();
      this.LoadData();
    }

    private void btnTransfer_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      bool success = this.CheckValidTransfer(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save
      success = this.TransferWOCarcassContractOut();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // Load Data
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["WO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ItemQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["IsTransfer"].Hidden = true;
      e.Layout.Bands[0].Columns["Transfer"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Transfer"].Header.Caption = "Lock";

      // Set Align
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["ItemQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["NetPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["GrossPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["OtherPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["MaterialSupply"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["WOCarcassContractOutPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ContractOutCode"].Hidden = true;
      e.Layout.Bands[1].Columns["WO"].Hidden = true;
      e.Layout.Bands[1].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[1].Columns["IsUpdate"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemQty"].Header.Caption = "WO Qty General";
      e.Layout.Bands[1].Columns["SupplierPid"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[1].Columns["NetPrice"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[1].Columns["SupplierPid"].Header.Caption = "Supplier";
      e.Layout.Bands[1].Columns["SupplierPid"].ValueList = ultddSupplier;
      e.Layout.Bands[1].Columns["OtherPrice"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["MaterialSupply"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["GrossPrice"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ContractOut"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["PlanningConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["NetPrice"].Format = "###,###";
      e.Layout.Bands[1].Columns["OtherPrice"].Format = "###,###";
      e.Layout.Bands[1].Columns["GrossPrice"].Format = "###,###";
      e.Layout.Bands[1].Columns["MaterialSupply"].Format = "###,###";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string commandText = string.Empty;

      switch (columnName)
      {
        case "SupplierPid":
          int count = 0;
          long supplierPid = DBConvert.ParseLong(ultddSupplier.SelectedRow.Cells["Pid"].Value.ToString());

          UltraGridRow row = e.Cell.Row.ParentRow;
          for (int i = 0; i < row.ChildBands[0].Rows.Count; i++)
          {
            UltraGridRow RowData = row.ChildBands[0].Rows[i];
            if (supplierPid == DBConvert.ParseLong(row.ChildBands[0].Rows[i].Cells["SupplierPid"].Value.ToString()))
            {
              count = count + 1;
            }
          }

          if (count >= 1)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0013"), "Supplier");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
            return;
          }

          // Check Transfer
          if (DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["IsTransfer"].Value.ToString()) == 1)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), e.Cell.Row.ParentRow.Cells["CarcassCode"].Value.ToString() + " Was Transfer");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;

            return;
          }
          // End
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Expand All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// Show Image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      if (chkShowImage.Checked == true)
      {
        try
        {
          grpBoxCarcassCode.Visible = true;
          string carcassCode = ultData.Selected.Rows[0].Cells["CarcassCode"].Value.ToString().Trim();
          if (carcassCode.Length > 0)
          {
            picCarcassCode.ImageLocation = FunctionUtility.BOMGetCarcassImage(carcassCode);
            grpBoxCarcassCode.Text = string.Format("{0}", carcassCode);
          }
        }
        catch { }
      }
    }

    /// <summary>
    /// Check Show Image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowImage.Checked)
      {
        grpBoxCarcassCode.Visible = true;
      }
      else
      {
        grpBoxCarcassCode.Visible = false;
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      if (string.Compare("SupplierPid", colName, true) == 0 ||
        string.Compare("ItemQty", colName, true) == 0 ||
        string.Compare("PlanningConfirm", colName, true) == 0 ||
        string.Compare("NetPrice", colName, true) == 0)
      {
        row.Cells["IsUpdate"].Value = 1;
      }
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }

      UltraGridRow row = ultData.Selected.Rows[0];
      if (row.ParentRow != null)
      {
        long carcassContractOutPid = DBConvert.ParseLong(row.Cells["WOCarcassContractOutPid"].Value.ToString());
        string carcassCode = row.Cells["CarcassCode"].Value.ToString();
        if (carcassContractOutPid != long.MinValue)
        {
          viewBOM_03_031 uc = new viewBOM_03_031();
          uc.carcassContractOutPid = carcassContractOutPid;
          uc.carcassCode = carcassCode;
          WindowUtinity.ShowView(uc, "CARCASS CONTRACTOUT DETAIL", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
          this.LoadData();
        }
      }
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      // Check Transfer
      foreach (UltraGridRow row in e.Rows)
      {
        if (DBConvert.ParseInt(row.ParentRow.Cells["IsTransfer"].Value.ToString()) == 1)
        {
          e.Cancel = true;
          return;
        }
      }
      // End

      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        long carcassContractOutPid = DBConvert.ParseLong(row.Cells["WOCarcassContractOutPid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }

        if (carcassContractOutPid != long.MinValue)
        {
          this.listUpdateCarcassContractOutPid.Add(carcassContractOutPid);
        }
      }
    }
    #endregion Event

    #region Function
    /// <summary>
    /// Create Data Table
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("SupplierCode", typeof(System.String));
      dt.Columns.Add("NetPrice", typeof(System.Double));

      return dt;
    }

    /// <summary>
    /// Create Data Set
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("WO", typeof(System.Int64));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("NameEN", typeof(System.String));
      taParent.Columns.Add("NameVN", typeof(System.String));
      taParent.Columns.Add("Transfer", typeof(System.Int32));
      taParent.Columns.Add("IsTransfer", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("WOCarcassContractOutPid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("SupplierPid", typeof(System.Int64));
      taChild.Columns.Add("ItemQty", typeof(System.Int32));
      taChild.Columns.Add("NetPrice", typeof(System.Double));
      taChild.Columns.Add("ContractOut", typeof(System.String));
      taChild.Columns.Add("MaterialSupply", typeof(System.Double));
      taChild.Columns.Add("OtherPrice", typeof(System.Double));
      taChild.Columns.Add("GrossPrice", typeof(System.Double));
      taChild.Columns.Add("IsUpdate", typeof(System.Int32));
      //taChild.Columns.Add("Transfer", typeof(System.Int32));
      //taChild.Columns.Add("IsTransfer", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["CarcassCode"] }, new DataColumn[] { taChild.Columns["WO"], taChild.Columns["CarcassCode"] }, false));
      return ds;
    }

    /// <summary>
    /// Check Valid Transfer
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidTransfer(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        if (DBConvert.ParseInt(rowParent.Cells["Transfer"].Value.ToString()) == 1 &&
          DBConvert.ParseInt(rowParent.Cells["IsTransfer"].Value.ToString()) == 0)
        {
          for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow row = rowParent.ChildBands[0].Rows[j];
            if (DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) <= 0)
            {
              message = "Transfer";
              row.Appearance.BackColor = Color.Yellow;
              return false;
            }
            if (DBConvert.ParseInt(row.Cells["PlanningConfirm"].Value.ToString()) == 0 || DBConvert.ParseInt(row.Cells["IsUpdate"].Value.ToString()) == 1)
            {
              message = "PlanningConfirm";
              row.Appearance.BackColor = Color.Yellow;
              return false;
            }
            if (row.Cells["ContractOut"].Value.ToString().Length == 0)
            {
              message = "ContractOut";
              row.Appearance.BackColor = Color.Yellow;
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = rowParent.ChildBands[0].Rows[j];
          if (DBConvert.ParseLong(row.Cells["SupplierPid"].Value.ToString()) <= 0)
          {
            message = "Supplier";
            return false;
          }
          int confirm = DBConvert.ParseInt(row.Cells["PlanningConfirm"].Value.ToString());
          int isUpdate = DBConvert.ParseInt(row.Cells["IsUpdate"].Value.ToString());
          if (confirm == 1 && isUpdate == 1)
          {
            if (row.Cells["ContractOut"].Value.ToString().Length == 0)
            {
              message = "ContractOut";
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      // Delete Row In grid
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spPLNWOCarcassContractOutRelation_Delete", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        if (DBConvert.ParseInt(rowParent.Cells["IsTransfer"].Value.ToString()) == 0)
        {
          for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow row = rowParent.ChildBands[0].Rows[j];
            if (DBConvert.ParseInt(row.Cells["IsUpdate"].Value.ToString()) == 1)
            {
              DBParameter[] input = new DBParameter[8];
              if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
              {
                input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
              }
              input[1] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row.Cells["WO"].Value.ToString()));
              input[2] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
              input[3] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(row.Cells["SupplierPid"].Value.ToString()));
              input[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              input[5] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              if (row.Cells["ContractOutCode"].Value.ToString().Trim().Length > 0)
              {
                input[6] = new DBParameter("@ContractOut", DbType.AnsiString, 32, row.Cells["ContractOutCode"].Value.ToString());
              }
              if (DBConvert.ParseInt(row.Cells["PlanningConfirm"].Value.ToString()) != int.MinValue)
              {
                input[7] = new DBParameter("@PlanningConfirm", DbType.Int32, DBConvert.ParseInt(row.Cells["PlanningConfirm"].Value.ToString()));
              }

              DBParameter[] output = new DBParameter[1];
              output[0] = new DBParameter("@Result", DbType.Int32, 0);

              DataBaseAccess.ExecuteStoreProcedure("spPLNWOCarcassContractOutRelation_Edit", input, output);
              long result = DBConvert.ParseInt(output[0].Value.ToString());
              if (result <= 0)
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Mapping Carcass ContractOut Info
    /// </summary>
    /// <returns></returns>
    private bool SaveMappingCarcassContractOut()
    {
      // Update WO = NULL In CarcassContractOut
      foreach (long pidUpdate in this.listUpdateCarcassContractOutPid)
      {
        DBParameter[] inputUpdate = new DBParameter[] { new DBParameter("@CarcassContractOutPid", DbType.Int64, pidUpdate) };
        DBParameter[] outputUpdate = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spPLNCarcassContractOutConnectWO_Update", inputUpdate, outputUpdate);
        long resultUpdate = DBConvert.ParseLong(outputUpdate[0].Value.ToString());
        if (resultUpdate <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        if (DBConvert.ParseInt(rowParent.Cells["IsTransfer"].Value.ToString()) == 0)
        {
          for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow row = rowParent.ChildBands[0].Rows[j];
            if (DBConvert.ParseInt(row.Cells["IsUpdate"].Value.ToString()) == 1)
            {
              DBParameter[] input = new DBParameter[7];
              input[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row.Cells["WO"].Value.ToString()));
              input[1] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
              input[2] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(row.Cells["SupplierPid"].Value.ToString()));
              if (DBConvert.ParseDouble(row.Cells["NetPrice"].Value.ToString()) > 0)
              {
                input[3] = new DBParameter("@NetPrice", DbType.Double, DBConvert.ParseDouble(row.Cells["NetPrice"].Value.ToString()));
              }
              //input[4] = new DBParameter("@ItemQty", DbType.Int32, DBConvert.ParseInt(row.Cells["ItemQty"].Value.ToString()));
              input[4] = new DBParameter("@ItemQty", DbType.Int32, 0);
              input[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              input[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

              DBParameter[] output = new DBParameter[1];
              output[0] = new DBParameter("@Result", DbType.Int64, 0);

              DataBaseAccess.ExecuteStoreProcedure("spPLNMappingWOCarcassContractOut_Edit", input, output);
              long result = DBConvert.ParseLong(output[0].Value.ToString());
              if (result <= 0)
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Transfer WO Carcass Contract Out
    /// </summary>
    /// <returns></returns>
    private bool TransferWOCarcassContractOut()
    {
      string stringData = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        if (DBConvert.ParseInt(rowParent.Cells["Transfer"].Value.ToString()) == 1 &&
            DBConvert.ParseInt(rowParent.Cells["IsTransfer"].Value.ToString()) == 0)
        {
          for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow row = rowParent.ChildBands[0].Rows[j];
            string ContractOutCode = "";
            if (row.Cells["ContractOutCode"].Value.ToString().Trim().Length > 0)
            {
              ContractOutCode = row.Cells["ContractOutCode"].Value.ToString().Trim();
            }
            else
            {
              ContractOutCode = "NULL";
            }
            stringData = stringData + row.Cells["CarcassCode"].Value.ToString() + "^" + DBConvert.ParseLong(row.Cells["SupplierPid"].Value.ToString()) + "^" + ContractOutCode + "~";
          }
        }
      }
      if (stringData.Length > 0)
      {
        DBParameter[] input = new DBParameter[3];
        input[0] = new DBParameter("@WO", DbType.Int64, this.wo);
        input[1] = new DBParameter("@StringData", DbType.String, stringData);
        input[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, 0);

        DataBaseAccess.ExecuteStoreProcedure("spPLNTransferWOCarcassContractOut_Edit", input, output);
        long result = DBConvert.ParseInt(output[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }

      return true;
    }

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();
      //if(colName == "PlanningConfirm" && e.Cell.Row.Cells["CarcassCode"].Text.Length == 0)
      //{
      //  e.Cancel = true;
      //}
    }
    #endregion Function
  }
}
