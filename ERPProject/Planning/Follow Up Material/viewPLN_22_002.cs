/*
 * Created By   : 
 * Created Date : 18/7/2013
 * Description  : Follow Carcass Division By Material 
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_22_002 : MainUserControl
  {
    #region field
    private DataTable dtSource = new DataTable();
    #endregion field

    #region init

    public viewPLN_22_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_22_002_Load(object sender, EventArgs e)
    {
      // Load Material Group
      this.LoadMaterialGroup();

      // Load Note
      this.LoadNote();

      // CreateTable
      this.dtSource = this.CreateTable();
    }
    #endregion init

    #region function

    /// <summary>
    /// Create Table
    /// </summary>
    private DataTable CreateTable()
    {
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("MaterialGroup", typeof(string));
      dtSource.Columns.Add("GroupName", typeof(string));
      dtSource.Columns.Add("MaterialCode", typeof(string));
      dtSource.Columns.Add("NameVN", typeof(string));
      dtSource.Columns.Add("Unit", typeof(string));
      dtSource.Columns.Add("ReOrder", typeof(double));
      dtSource.Columns.Add("PRQty", typeof(double));
      dtSource.Columns.Add("RequestDate", typeof(DateTime));
      dtSource.Columns.Add("Urgent", typeof(int));
      dtSource.Columns.Add("ProjectCode", typeof(int));

      return dtSource;
    }
    /// <summary>
    /// Load Note
    /// </summary>
    private void LoadNote()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'Materials In BOM' Name";
      commandText += " UNION ";
      commandText += " SELECT 2 ID, 'ALL Of Materials' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultNote.DataSource = dtSource;
      ultNote.DisplayMember = "Name";
      ultNote.ValueMember = "ID";
      ultNote.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultNote.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultNote.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;

      this.ultNote.Value = 1;
    }

    /// <summary>
    /// Load Material Group
    /// </summary>
    private void LoadMaterialGroup()
    {
      string commandText = string.Empty;
      commandText += " SELECT [Group], Name ";
      commandText += " FROM TblGNRMaterialGroup ";
      commandText += " WHERE Warehouse = 1 ";
      commandText += "    	AND [Group] != '010' ";

      DataTable dtGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucMaterialGroup.DataSource = dtGroup;
      ucMaterialGroup.ColumnWidths = "100;400";
      ucMaterialGroup.DataBind();
      ucMaterialGroup.ValueMember = "[Group]";
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      if (this.ultNote.Value == null
          || DBConvert.ParseInt(this.ultNote.Value.ToString()) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Note");
        return;
      }

      DateTime LastMonthDate = DateTime.Now.AddMonths(-1);

      DBParameter[] input = new DBParameter[7];
      input[0] = new DBParameter("@PreMonth", DbType.Int32, LastMonthDate.Month);
      input[1] = new DBParameter("@PreYear", DbType.Int32, LastMonthDate.Year);
      input[2] = new DBParameter("@Month", DbType.Int32, DateTime.Now.Month);
      input[3] = new DBParameter("@Year", DbType.Int32, DateTime.Now.Year);
      if (txtMaterialCode.Text.Length > 0)
      {
        input[4] = new DBParameter("@MaterialCode", DbType.String, txtMaterialCode.Text.Trim());
      }

      if (txtMatGroup.Text.Length > 0)
      {
        input[5] = new DBParameter("@GroupMaterialCode", DbType.String, txtMatGroup.Text.Trim());
      }
      input[6] = new DBParameter("@Note", DbType.Int32, DBConvert.ParseInt(this.ultNote.Value.ToString()));

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanFollowUpMaterialCarcassDivision_Select", input);

      this.ultData.DataSource = dtSource;

      // To Mau ReOrder & Stock PR
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseDouble(row.Cells["ReOrder"].Value.ToString()) > 0)
        {
          row.Cells["ReOrder"].Appearance.BackColor = Color.Orange;
        }

        if (DBConvert.ParseDouble(row.Cells["StockPR"].Value.ToString()) > 0)
        {
          row.Cells["StockPR"].Appearance.BackColor = Color.GreenYellow;
        }
      }
      // End To Mau ReOrder & Stock PR

      // Load Column Hide/ Unhide
      if (ultShowColumns.Rows.Count == 0)
      {
        this.LoadColumnName();
      }
      else
      {
        this.SetStatusColumn();
      }
      // End Load Column Hide/ Unhide
    }

    /// <summary>
    /// Load MaterialCode
    /// </summary>
    private void LoadMaterialCode()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, this.txtMatGroup.Text);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNGetListControlMaterial", inputParam);
      dtSource.Columns.Remove("Description");
      ucMaterialCode.DataSource = dtSource;
      ucMaterialCode.ColumnWidths = "100; 200";
      ucMaterialCode.DataBind();
      ucMaterialCode.ValueMember = "MaterialCode";
      txtMaterialCode.Text = string.Empty;
    }

    /// <summary>
    /// Set Status Column When Search
    /// </summary>
    private void SetStatusColumn()
    {
      for (int colIndex = 1; colIndex < ultShowColumns.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumns.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// Load Column Name
    /// </summary>
    private void LoadColumnName()
    {
      DataTable dtNew = new DataTable();
      DataTable dtColumn = (DataTable)ultData.DataSource;
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        dtNew.Columns[column.ColumnName].DefaultValue = 0;

        if (string.Compare(column.ColumnName, "NameVN", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "Unit", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "Ending", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "MonthlyConsumption", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "ReOrder", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "StockPR", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      ultShowColumns.DataSource = dtNew;
      ultShowColumns.Rows[0].Appearance.BackColor = Color.LightBlue;
    }

    /// <summary>
    /// Make PROnline
    /// </summary>
    private void MakePROnline()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          DataRow[] foundRow = dtSource.Select("MaterialCode = '" + row.Cells["MaterialCode"].Value.ToString() + "'");
          if (foundRow.Length == 0)
          {
            DataRow rowAdd = dtSource.NewRow();
            rowAdd["MaterialGroup"] = row.Cells["MaterialGroup"].Value.ToString();
            rowAdd["GroupName"] = row.Cells["GroupName"].Value.ToString();
            rowAdd["MaterialCode"] = row.Cells["MaterialCode"].Value.ToString();
            rowAdd["NameVN"] = row.Cells["NameVN"].Value.ToString();
            rowAdd["Unit"] = row.Cells["Unit"].Value.ToString();
            if (DBConvert.ParseDouble(row.Cells["ReOrder"].Value.ToString()) > 0)
            {
              rowAdd["ReOrder"] = DBConvert.ParseDouble(row.Cells["ReOrder"].Value.ToString());
              rowAdd["PRQty"] = DBConvert.ParseDouble(row.Cells["ReOrder"].Value.ToString());
            }
            else
            {
              rowAdd["ReOrder"] = 0;
              rowAdd["PRQty"] = 0;
            }
            dtSource.Rows.Add(rowAdd);
          }
          row.Cells["Select"].Value = 0;
        }
      }
    }
    #endregion function

    #region event

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;
      // Search
      this.Search();
      this.btnSearch.Enabled = true;
    }

    private void btnMakePROnline_Click(object sender, EventArgs e)
    {
      this.MakePROnline();
    }

    private void btnTransfer_Click(object sender, EventArgs e)
    {
      this.MakePROnline();

      viewPLN_22_003 uc = new viewPLN_22_003();
      uc.dtSource = this.dtSource;
      this.dtSource = this.CreateTable();
      WindowUtinity.ShowView(uc, "MAKE PR ONLINE", false, ViewState.MainWindow);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      btnPrint.Enabled = false;
      string strTemplateName = "RPT_PLN_22_002";
      string strSheetName = "Sheet1";
      string strOutFileName = "Materials Carcass Division";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtData = (DataTable)ultData.DataSource;
      if (dtData != null)
      {
        DateTime today = DateTime.Now;
        for (int i = 1; i < 7; i++)
        {
          string month = today.Month.ToString();
          string year = today.Year.ToString();
          oXlsReport.Cell(string.Format("**ColName{0}", i)).Value = month + "/" + year;
          today = today.AddMonths(1);
        }
      }

      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:Y8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:Y8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**MaterialGroup", 0, i).Value = dtRow["MaterialGroup"].ToString();
          oXlsReport.Cell("**GroupName", 0, i).Value = dtRow["GroupName"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          if (DBConvert.ParseDouble(dtRow["MonthlyConsumption"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**MonthlyConsumption", 0, i).Value = DBConvert.ParseDouble(dtRow["MonthlyConsumption"].ToString());
          }
          else
          {
            oXlsReport.Cell("**MonthlyConsumption", 0, i).Value = 0;
          }

          oXlsReport.Cell("**Min/Max", 0, i).Value = dtRow["Min/Max"].ToString();
          if (DBConvert.ParseDouble(dtRow["LeadTime"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**LeadTime", 0, i).Value = DBConvert.ParseDouble(dtRow["LeadTime"].ToString());
          }
          else
          {
            oXlsReport.Cell("**LeadTime", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["MinOfStock"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**MinOfStock", 0, i).Value = DBConvert.ParseDouble(dtRow["MinOfStock"].ToString());
          }
          else
          {
            oXlsReport.Cell("**MinOfStock", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["Beginning"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Beginning", 0, i).Value = DBConvert.ParseDouble(dtRow["Beginning"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Beginning", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["InStock"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**InStock", 0, i).Value = DBConvert.ParseDouble(dtRow["InStock"].ToString());
          }
          else
          {
            oXlsReport.Cell("**InStock", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["OutStock"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**OutStock", 0, i).Value = DBConvert.ParseDouble(dtRow["OutStock"].ToString());
          }
          else
          {
            oXlsReport.Cell("**OutStock", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["Ending"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Ending", 0, i).Value = DBConvert.ParseDouble(dtRow["Ending"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Ending", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["ReOrder"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**ReOrder", 0, i).Value = DBConvert.ParseDouble(dtRow["ReOrder"].ToString());
          }
          else
          {
            oXlsReport.Cell("**ReOrder", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["StockPR"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = DBConvert.ParseDouble(dtRow["StockPR"].ToString());
          }
          else
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["Column1"].ToString()) != double.MinValue)
          {

            oXlsReport.Cell("**ColValue1", 0, i).Value = DBConvert.ParseDouble(dtRow["Column1"].ToString());
          }
          else
          {
            oXlsReport.Cell("**ColValue1", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Column2"].ToString()) != double.MinValue)
          {

            oXlsReport.Cell("**ColValue2", 0, i).Value = DBConvert.ParseDouble(dtRow["Column2"].ToString());
          }
          else
          {
            oXlsReport.Cell("**ColValue2", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Column3"].ToString()) != double.MinValue)
          {

            oXlsReport.Cell("**ColValue3", 0, i).Value = DBConvert.ParseDouble(dtRow["Column3"].ToString());
          }
          else
          {
            oXlsReport.Cell("**ColValue3", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Column4"].ToString()) != double.MinValue)
          {

            oXlsReport.Cell("**ColValue4", 0, i).Value = DBConvert.ParseDouble(dtRow["Column4"].ToString());
          }
          else
          {
            oXlsReport.Cell("**ColValue4", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Column5"].ToString()) != double.MinValue)
          {

            oXlsReport.Cell("**ColValue5", 0, i).Value = DBConvert.ParseDouble(dtRow["Column5"].ToString());
          }
          else
          {
            oXlsReport.Cell("**ColValue5", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Column6"].ToString()) != double.MinValue)
          {

            oXlsReport.Cell("**ColValue6", 0, i).Value = DBConvert.ParseDouble(dtRow["Column6"].ToString());
          }
          else
          {
            oXlsReport.Cell("**ColValue6", 0, i).Value = DBNull.Value;
          }

          if (DBConvert.ParseDouble(dtRow["Others"].ToString()) != double.MinValue)
          {

            oXlsReport.Cell("**Others", 0, i).Value = DBConvert.ParseDouble(dtRow["Others"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Others", 0, i).Value = DBNull.Value;
          }
        }
      }

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
      btnPrint.Enabled = true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ucMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMatGroup.Text = ucMaterialGroup.SelectedValue;
      this.LoadMaterialCode();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["Select"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["No"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["MaterialGroup"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["GroupName"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["NameEN"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["Min/Max"].Hidden = true;
      e.Layout.Bands[0].Columns["LeadTime"].Hidden = true;
      e.Layout.Bands[0].Columns["MinOfStock"].Hidden = true;
      e.Layout.Bands[0].Columns["Beginning"].Hidden = true;
      e.Layout.Bands[0].Columns["InStock"].Hidden = true;
      e.Layout.Bands[0].Columns["OutStock"].Hidden = true;
      e.Layout.Bands[0].Columns["Column1"].Hidden = true;
      e.Layout.Bands[0].Columns["Column2"].Hidden = true;
      e.Layout.Bands[0].Columns["Column3"].Hidden = true;
      e.Layout.Bands[0].Columns["Column4"].Hidden = true;
      e.Layout.Bands[0].Columns["Column5"].Hidden = true;
      e.Layout.Bands[0].Columns["Column6"].Hidden = true;
      e.Layout.Bands[0].Columns["Others"].Hidden = true;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      for (int i = 1; i < ultData.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        if (i >= 18) // Column 1 -> Column Others
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGreen;
        }
      }

      DateTime today = DateTime.Now;
      for (int i = 1; i < 7; i++)
      {
        string month = today.Month.ToString();
        string year = today.Year.ToString();
        e.Layout.Bands[0].Columns[string.Format(@"Column{0}", i)].Header.Caption = month + "/" + year;
        today = today.AddMonths(1);
      }

      e.Layout.Bands[0].Columns["MaterialGroup"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["GroupName"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["NameEN"].CellAppearance.BackColor = Color.Yellow;

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MaterialGroup"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MonthlyConsumption"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LeadTime"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MinOfStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Beginning"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["InStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["OutStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Ending"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ReOrder"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["StockPR"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Column1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Column2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Column3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Column4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Column5"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Column6"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Others"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["No"].MinWidth = 10;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 10;
      e.Layout.Bands[0].Columns["Select"].MinWidth = 30;
      e.Layout.Bands[0].Columns["Select"].MaxWidth = 30;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultShowColumns_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
        }
      }
      else
      {
        for (int i = 1; i < ultShowColumns.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          row.Cells[i].Value = e.Cell.Text;
        }
        this.ChkAll_CheckedChange();
      }
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange()
    {
      for (int colIndex = 1; colIndex < ultShowColumns.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumns.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    private void ultShowColumns_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      DataTable dtColumn = (DataTable)ultShowColumns.DataSource;
      int count = dtColumn.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        e.Layout.Bands[0].Columns[i].Width = 60;
      }

      e.Layout.Bands[0].Columns["Select"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialGroup"].Hidden = true;
      e.Layout.Bands[0].Columns["GroupName"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialCode"].Hidden = true;
      e.Layout.Bands[0].Columns["NameEN"].Hidden = true;

      DateTime today = DateTime.Now;
      for (int i = 1; i < 7; i++)
      {
        string month = today.Month.ToString();
        string year = today.Year.ToString();
        e.Layout.Bands[0].Columns[string.Format(@"Column{0}", i)].Header.Caption = month + "/" + year;
        today = today.AddMonths(1);
      }

      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }


    private void chkShowItemListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucMaterialGroup.Visible = chkShowItemListBox.Checked;
    }

    private void chkShowMaterialListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucMaterialCode.Visible = chkShowMaterialListBox.Checked;
    }

    private void ucMaterialCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialCode.Text = ucMaterialCode.SelectedValue;
    }

    #endregion event
  }
}
