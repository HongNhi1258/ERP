/*
  Author      : Duong Minh 
  Date        : 14/06/2012
  Description : Report
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_22_001 : MainUserControl
  {
    #region Field
    int checkDepartment = int.MinValue;
    #endregion Field

    #region Init

    public viewWHD_22_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load ViewVEN_05_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewVEN_05_001_Load(object sender, EventArgs e)
    {
      if (this.btnViewPrice.Visible)
      {
        this.checkDepartment = 0;
      }
      else
      {
        this.checkDepartment = 1;
      }

      if (this.checkDepartment == 1)
      {
        // Check User Login
        string commandText = string.Format(@"SELECT Code FROM VHRDDepartmentInfo WHERE Code = '{0}'", SharedObject.UserInfo.Department);
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null)
        {
          this.checkDepartment = string.Compare(dt.Rows[0]["Code"].ToString(), "ACC", true);
        }
      }

      // Load All Data Report
      this.LoadData();
      this.btnViewPrice.Visible = false;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      //Load UltraCombo Type (Stock Balance Report / Stock Balance Detail / Monthly Stock Balance Report/ Receiving Detail/ Issuing Detail
      //Stock Movement Report/ Stock Movement Detail Report/ Ageing )
      this.LoadComboType();

      // Load UltraCombo MaterialCode
      this.LoadComboMaterialCode();

      // Load UltraCombo Location
      this.LoadComboLocation();

      // Load UltraCombo ReceivingType (Receiving Note / Return From Production / Adjustment In)
      this.LoadComboReceivingType();

      // Load UltraCombo IssuingType (Issue To Production / Return To Supplier / Adjustment / Issue To Subcon)
      this.LoadComboIssuingType();

      // Load UltraCombo Supplier
      this.LoadComboSupplier();

      // Load UltraCombo Department
      this.LoadComboDepartment();

      // Load UltraCombo Month
      this.LoadComboMonth();

      // Load UltraComBo Year
      this.LoadComboYear();
    }

    /// <summary>
    /// Load UltraCombo Type (Stock Balance Report / Stock Balance Detail Report/ Monthly Stock Balance Report/ Receiving Detail Report/ Issuing Detail Report 
    /// Stock Movement Report/ Stock Movement Detail Report/ Ageing )
    /// </summary>
    private void LoadComboType()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'Stock Balance Report' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Stock Balance Detail Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 3 ID, 'Monthly Stock Balance Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 4 ID, 'Receiving Detail Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 5 ID, 'Issuing Detail Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 6 ID, 'Stock Movement Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 7 ID, 'Stock Movement Detail Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 12 ID, 'Stock Movement Detail With Allocate Data Report' Name";
      commandText += " UNION ";
      commandText += " SELECT 8 ID, 'Ageing' Name";
      commandText += " UNION ";
      commandText += " SELECT 11 ID, 'Ageing Monthly' Name";
      commandText += " UNION ";
      commandText += " SELECT 9 ID, 'Stock Balance Base Choose Time' Name";
      //commandText += " UNION ";
      //commandText += " SELECT 10 ID, 'Consumption Wood' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultType.DataSource = dtSource;
      ultType.DisplayMember = "Name";
      ultType.ValueMember = "ID";
      ultType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultType.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultType.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Material Code
    /// </summary>
    private void LoadComboMaterialCode()
    {
      string commandText = string.Empty;
      commandText += "  SELECT SP.ID_SanPham, SP.ID_SanPham + ' - ' + SP.TenEnglish Name";
      commandText += "  FROM VWHDMaterialCodeCommon SP";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultMaterialCode.DataSource = dtSource;
      ultMaterialCode.DisplayMember = "Name";
      ultMaterialCode.ValueMember = "ID_SanPham";
      ultMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["ID_SanPham"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Location
    /// </summary>
    private void LoadComboLocation()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_Position, Name ";
      commandText += " FROM VWHDLocationWoods ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultLocation.DataSource = dtSource;
      ultLocation.DisplayMember = "Name";
      ultLocation.ValueMember = "ID_Position";
      ultLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLocation.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultLocation.DisplayLayout.Bands[0].Columns["ID_Position"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo ReceivingType (Receiving Note / Return From Production / Adjustment In)
    /// </summary>
    private void LoadComboReceivingType()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 1 ID, 'Receiving Note' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Return From Production' Name";
      commandText += " UNION ";
      commandText += " SELECT 3 ID, 'Adjustment In' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultRECType.DataSource = dtSource;
      ultRECType.DisplayMember = "Name";
      ultRECType.ValueMember = "ID";
      ultRECType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultRECType.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultRECType.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }
    /// <summary>
    /// Load UltraCombo IssuingType (Issue To Production / Return To Supplier / Adjustment / Issue To Subcon)
    /// </summary>
    private void LoadComboIssuingType()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 1 ID, 'Issue To Production' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Return To Supplier' Name";
      commandText += " UNION ";
      commandText += " SELECT 3 ID, 'Adjustment' Name";
      commandText += " UNION ";
      commandText += " SELECT 4 ID, 'Issue To Subcon' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultISSType.DataSource = dtSource;
      ultISSType.DisplayMember = "Name";
      ultISSType.ValueMember = "ID";
      ultISSType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultISSType.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultISSType.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = "SELECT ID_NhaCC, ID_NhaCC + ' - ' + TenNhaCCEN AS Name FROM VWHDSupplier";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultSupplier.DataSource = dtSource;
      ultSupplier.DisplayMember = "Name";
      ultSupplier.ValueMember = "ID_NhaCC";
      ultSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSupplier.DisplayLayout.Bands[0].Columns["Name"].Width = 450;
      ultSupplier.DisplayLayout.Bands[0].Columns["ID_NhaCC"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "Name";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load Month
    /// </summary>
    private void LoadComboMonth()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Month", typeof(Int32));
      for (int i = 1; i < 13; i++)
      {
        DataRow dr = dt.NewRow();
        dr["Month"] = i;
        dt.Rows.Add(dr);
      }
      ultMonth.DataSource = dt;
      ultMonth.DisplayMember = "Month";
      ultMonth.ValueMember = "Month";
      ultMonth.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Year
    /// </summary>
    private void LoadComboYear()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Year", typeof(Int32));
      for (int i = 2008; i < 2099; i++)
      {
        DataRow dr = dt.NewRow();
        dr["Year"] = i;
        dt.Rows.Add(dr);
      }
      ultYear.DataSource = dt;
      ultYear.DisplayMember = "Year";
      ultYear.ValueMember = "Year";
      ultYear.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    #endregion Init

    #region Event

    /// <summary>
    /// Export
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      int value = int.MinValue;
      if (this.ultType.Value != null)
      {
        value = DBConvert.ParseInt(this.ultType.Value.ToString());

        if (value == 1)
        {
          this.ExportStockBalance();
        }
        else if (value == 2)
        {
          this.ExportStockBalanceDetail();
        }
        else if (value == 3)
        {
          this.ExportMonthlyStockBalance();
        }
        else if (value == 4)
        {
          this.ExportReceivingDetail();
        }
        else if (value == 5)
        {
          this.ExportIssuingDetail();
        }
        else if (value == 6)
        {
          this.ExportStockMovement();
        }
        else if (value == 7)
        {
          this.ExportStockMovementDetail();
        }
        else if (value == 8)
        {
          this.ExportAgeing();
        }
        else if (value == 9)
        {
          this.ExportStockBalanceBaseChooseTime();
        }
        else if (value == 10)
        {
          this.ExportConsumptionVeneer();
        }
        else if (value == 11)
        {
          this.ExportAgeingMonthly();
        }
        else if (value == 12)
        {
          this.ExportStockMovementDetailAllocateData();
        }
      }
    }
    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click_1(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion Event

    #region Function

    /// <summary>
    /// Export Stock Balance
    /// </summary>
    private void ExportStockBalance()
    {
      string strTemplateName = "RPT_WOOD_03_001_02";
      string strSheetName = "Sheet1";
      string strOutFileName = "Stock Balance";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[2];

      long locationPid = long.MinValue;
      string materialCode = string.Empty;

      // Location
      if (this.ultLocation.Value != null)
      {
        locationPid = DBConvert.ParseLong(this.ultLocation.Value.ToString());
        if (locationPid != long.MinValue)
        {
          arrInput[0] = new DBParameter("@LocationPid", DbType.Int64, locationPid);
        }
      }
      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
        if (materialCode.Length > 0)
        {
          arrInput[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
        }
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTStockBalanceWoods_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      if (dtData != null && dtData.Rows.Count > 0)
      {
        Double totalCBM = 0;
        Double totalPcs = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:J8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:J8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
          oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["EOH(Balance)"].ToString());
          oXlsReport.Cell("**Pcs", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          oXlsReport.Cell("**Package", 0, i).Value = dtRow["Package"].ToString();
          oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();

          if (dtRow["EOH(Balance)"] != null && dtRow["EOH(Balance)"].ToString().Trim().Length > 0)
          {
            totalCBM = totalCBM + DBConvert.ParseDouble(dtRow["EOH(Balance)"].ToString());
          }
          if (dtRow["Qty"] != null && dtRow["Qty"].ToString().Trim().Length > 0)
          {
            totalPcs = totalPcs + DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
        }
        oXlsReport.Cell("**TotalPcs").Value = totalPcs;
        oXlsReport.Cell("**TotalQtyM2").Value = totalCBM;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Veneer Stock Balance Detail
    /// </summary>
    private void ExportStockBalanceDetail()
    {
      string strTemplateName = "RPT_WOOD_03_001_03";
      string strSheetName = "Sheet1";
      string strOutFileName = "Stock Balance Detail";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[2];

      long locationPid = long.MinValue;
      string materialCode = string.Empty;

      // Location
      if (this.ultLocation.Value != null)
      {
        locationPid = DBConvert.ParseLong(this.ultLocation.Value.ToString());
        if (locationPid != long.MinValue)
        {
          arrInput[0] = new DBParameter("@LocationPid", DbType.Int64, locationPid);
        }
      }
      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
        if (materialCode.Length > 0)
        {
          arrInput[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
        }
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTStockBalanceDetailWoods_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double totalCBM = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:P8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:P8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**RECDate", 0, i).Value = dtRow["RECDate"].ToString();
          oXlsReport.Cell("**LotNoId", 0, i).Value = dtRow["LotNoId"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
          oXlsReport.Cell("**Length", 0, i).Value = DBConvert.ParseDouble(dtRow["Length"].ToString());
          oXlsReport.Cell("**Width", 0, i).Value = DBConvert.ParseDouble(dtRow["Width"].ToString());
          oXlsReport.Cell("**Thickness", 0, i).Value = DBConvert.ParseDouble(dtRow["Thickness"].ToString());
          oXlsReport.Cell("**Qtym2", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
          oXlsReport.Cell("**Package", 0, i).Value = dtRow["Package"].ToString();
          oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();
          oXlsReport.Cell("**MC", 0, i).Value = dtRow["MC"].ToString();
          oXlsReport.Cell("**Grade", 0, i).Value = dtRow["Grade"].ToString();

          if (dtRow["QtyM2"] != null && dtRow["QtyM2"].ToString().Trim().Length > 0)
          {
            totalCBM = totalCBM + DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
          }
        }
        oXlsReport.Cell("**TotalQtyM2").Value = totalCBM;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Receiving Detail
    /// </summary>
    private void ExportReceivingDetail()
    {
      string strTemplateName = "RPT_WOOD_03_001_04";
      string strSheetName = "Sheet1";
      string strOutFileName = "Receiving Detail";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[8];

      long locationPid = long.MinValue;
      string materialCode = string.Empty;

      // RECFrom
      if (txtRECFrom.Text.Length > 0)
      {
        arrInput[0] = new DBParameter("@NoFrom", DbType.AnsiString, 24, txtRECFrom.Text.ToString());
      }

      // RECTo
      if (txtRECTo.Text.Length > 0)
      {
        arrInput[1] = new DBParameter("@NoTo", DbType.AnsiString, 24, txtRECTo.Text.ToString());
      }

      // RECSet
      if (txtRECSet.Text.Length > 0)
      {
        arrInput[2] = new DBParameter("@NoSet", DbType.AnsiString, 1024, txtRECSet.Text.ToString());
      }

      //Create Date
      DateTime prDateFrom = DateTime.MinValue;
      if (drpDateFrom.Value != null)
      {
        prDateFrom = (DateTime)drpDateFrom.Value;
      }
      DateTime prDateTo = DateTime.MinValue;
      if (drpDateFrom.Value != null)
      {
        prDateTo = (DateTime)drpDateTo.Value;
      }

      if (prDateFrom != DateTime.MinValue)
      {
        arrInput[3] = new DBParameter("@CreateDateFrom", DbType.DateTime, prDateFrom);
      }

      if (prDateTo != DateTime.MinValue)
      {
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        arrInput[4] = new DBParameter("@CreateDateTo", DbType.DateTime, prDateTo);
      }

      // ReceivingType
      int value = int.MinValue;

      if (this.ultRECType.Value != null)
      {
        value = DBConvert.ParseInt(this.ultRECType.Value.ToString());
        if (value != int.MinValue)
        {
          arrInput[5] = new DBParameter("@Type", DbType.Int32, value);
        }
      }

      // Location
      if (this.ultLocation.Value != null)
      {
        locationPid = DBConvert.ParseInt(this.ultLocation.Value.ToString());
        if (locationPid != long.MinValue)
        {
          arrInput[6] = new DBParameter("@LocationPid", DbType.Int64, locationPid);
        }
      }
      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
        if (materialCode.Length > 0)
        {
          arrInput[7] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
        }
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTReceivingDetailWoods_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double total = 0;
        double totalQtyM2 = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:Q8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:Q8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**RECDate", 0, i).Value = dtRow["RECDate"].ToString();
          oXlsReport.Cell("**RECNo", 0, i).Value = dtRow["ReceivingCode"].ToString();
          oXlsReport.Cell("**LotNoId", 0, i).Value = dtRow["LotNoId"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
          oXlsReport.Cell("**Length", 0, i).Value = DBConvert.ParseDouble(dtRow["Length"].ToString());
          oXlsReport.Cell("**Width", 0, i).Value = DBConvert.ParseDouble(dtRow["Width"].ToString());
          oXlsReport.Cell("**Thickness", 0, i).Value = DBConvert.ParseDouble(dtRow["Thickness"].ToString());
          oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          oXlsReport.Cell("**Qtym2", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
          oXlsReport.Cell("**Package", 0, i).Value = dtRow["Package"].ToString();
          oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();
          oXlsReport.Cell("**Invoice", 0, i).Value = dtRow["Invoice"].ToString();

          if (dtRow["Qty"] != null && dtRow["Qty"].ToString().Trim().Length > 0)
          {
            total = total + DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          if (dtRow["QtyM2"] != null && dtRow["QtyM2"].ToString().Trim().Length > 0)
          {
            totalQtyM2 = totalQtyM2 + DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
          }
        }
        oXlsReport.Cell("**TotalQty").Value = total;
        oXlsReport.Cell("**TotalQtyM2").Value = totalQtyM2;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Issuing Detail
    /// </summary>
    private void ExportIssuingDetail()
    {
      string strTemplateName = "RPT_WOOD_03_001_05";
      string strSheetName = "Sheet1";
      string strOutFileName = "Issuing Detail";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[8];

      long locationPid = long.MinValue;
      string materialCode = string.Empty;

      // RECFrom
      if (txtISSFrom.Text.Length > 0)
      {
        arrInput[0] = new DBParameter("@NoFrom", DbType.AnsiString, 24, txtISSFrom.Text.ToString());
      }

      // RECTo
      if (txtISSTo.Text.Length > 0)
      {
        arrInput[1] = new DBParameter("@NoTo", DbType.AnsiString, 24, txtISSTo.Text.ToString());
      }

      // RECSet
      if (txtISSSet.Text.Length > 0)
      {
        arrInput[2] = new DBParameter("@NoSet", DbType.AnsiString, 1024, txtISSSet.Text.ToString());
      }

      //Create Date
      DateTime prDateFrom = DateTime.MinValue;
      if (drpDateFrom.Value != null)
      {
        prDateFrom = (DateTime)drpDateFrom.Value;
      }

      DateTime prDateTo = DateTime.MinValue;
      if (drpDateTo.Value != null)
      {
        prDateTo = (DateTime)drpDateTo.Value;
      }

      if (prDateFrom != DateTime.MinValue)
      {
        arrInput[3] = new DBParameter("@CreateDateFrom", DbType.DateTime, prDateFrom);
      }

      if (prDateTo != DateTime.MinValue)
      {
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        arrInput[4] = new DBParameter("@CreateDateTo", DbType.DateTime, prDateTo);
      }

      // IssuingType
      int value = int.MinValue;

      if (this.ultISSType.Value != null)
      {
        value = DBConvert.ParseInt(this.ultISSType.Value.ToString());
        if (value != int.MinValue)
        {
          arrInput[5] = new DBParameter("@Type", DbType.Int32, value);
        }
      }

      // Location
      if (this.ultLocation.Value != null)
      {
        locationPid = DBConvert.ParseInt(this.ultLocation.Value.ToString());
        if (locationPid != long.MinValue)
        {
          arrInput[6] = new DBParameter("@LocationPid", DbType.Int64, locationPid);
        }
      }
      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
        if (materialCode.Length > 0)
        {
          arrInput[7] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
        }
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTIssuingDetailWoods_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      DataTable dtTrace = new DataTable();
      dtTrace.Columns.Add("LotNoId", typeof(System.String));

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double total = 0;
        double totalQtyM2 = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];

          if (i > 0)
          {
            oXlsReport.Cell("B8:S8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:S8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**RECDate", 0, i).Value = dtRow["RECDate"].ToString();
          oXlsReport.Cell("**RECNo", 0, i).Value = dtRow["IssuingCode"].ToString();
          oXlsReport.Cell("**LotNoId", 0, i).Value = dtRow["LotNoId"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
          oXlsReport.Cell("**Length", 0, i).Value = DBConvert.ParseDouble(dtRow["Length"].ToString());
          oXlsReport.Cell("**Width", 0, i).Value = DBConvert.ParseDouble(dtRow["Width"].ToString());
          oXlsReport.Cell("**Thickness", 0, i).Value = DBConvert.ParseDouble(dtRow["Thickness"].ToString());
          oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          oXlsReport.Cell("**Qtym2", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
          oXlsReport.Cell("**Package", 0, i).Value = dtRow["Package"].ToString();
          oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();

          if (dtRow["Qty"] != null && dtRow["Qty"].ToString().Trim().Length > 0)
          {
            total = total + DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          if (dtRow["QtyM2"] != null && dtRow["QtyM2"].ToString().Trim().Length > 0)
          {
            totalQtyM2 = totalQtyM2 + DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
          }
        }
        oXlsReport.Cell("**TotalQty").Value = total;
        oXlsReport.Cell("**TotalQtyM2").Value = totalQtyM2;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Monthly Stock Balance
    /// </summary>
    private void ExportMonthlyStockBalance()
    {
      string strTemplateName = "RPT_WOOD_03_001_06";
      string strSheetName = "Sheet1";
      string strOutFileName = "Monthly Stock Balance";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[3];

      if (ultMonth.Value != null && ultMonth.Text.ToString().Length > 0)
      {
        int month = DBConvert.ParseInt(ultMonth.Value.ToString());
        arrInput[0] = new DBParameter("@Month", DbType.Int32, month);
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Input Month");
        return;
      }

      if (ultYear.Value != null && ultYear.Text.ToString().Length > 0)
      {
        int year = DBConvert.ParseInt(ultYear.Value.ToString());
        arrInput[1] = new DBParameter("@Year", DbType.Int32, year);
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Input Year");
        return;
      }

      string materialCode = string.Empty;
      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
        if (materialCode.Length > 0)
        {
          arrInput[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
        }
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTMonthlyStockBalanceWoods_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
      oXlsReport.Cell("**Time").Value = this.ultMonth.Text.ToString();

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double total1 = 0;
        double total2 = 0;
        double total3 = 0;
        double total4 = 0;
        double total5 = 0;
        double total6 = 0;
        double total7 = 0;
        double total8 = 0;

        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B9:M9").Copy();
            oXlsReport.RowInsert(8 + i);
            oXlsReport.Cell("B9:M9", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
          if (DBConvert.ParseDouble(dtRow["BOH"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**BOH", 0, i).Value = DBConvert.ParseDouble(dtRow["BOH"].ToString());
          }
          else
          {
            oXlsReport.Cell("**BOH", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["BOHPieces"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**BOHPieces", 0, i).Value = DBConvert.ParseDouble(dtRow["BOHPieces"].ToString());
          }
          else
          {
            oXlsReport.Cell("**BOHPieces", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["QtyIn"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**In", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyIn"].ToString());
          }
          else
          {
            oXlsReport.Cell("**In", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["QtyInPieces"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**InPieces", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyInPieces"].ToString());
          }
          else
          {
            oXlsReport.Cell("**InPieces", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["QtyOut"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Out", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyOut"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Out", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["QtyOutPieces"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**OutPieces", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyOutPieces"].ToString());
          }
          else
          {
            oXlsReport.Cell("**OutPieces", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["EOH"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**EOH", 0, i).Value = DBConvert.ParseDouble(dtRow["EOH"].ToString());
          }
          else
          {
            oXlsReport.Cell("**EOH", 0, i).Value = 0;
          }

          if (DBConvert.ParseDouble(dtRow["EOHPieces"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**EOHPieces", 0, i).Value = DBConvert.ParseDouble(dtRow["EOHPieces"].ToString());
          }
          else
          {
            oXlsReport.Cell("**EOHPieces", 0, i).Value = 0;
          }

          if (dtRow["BOHPieces"] != null && dtRow["BOHPieces"].ToString().Trim().Length > 0)
          {
            total1 = total1 + DBConvert.ParseDouble(dtRow["BOHPieces"].ToString());
          }

          if (dtRow["BOH"] != null && dtRow["BOH"].ToString().Trim().Length > 0)
          {
            total2 = total2 + DBConvert.ParseDouble(dtRow["BOH"].ToString());
          }

          if (dtRow["QtyInPieces"] != null && dtRow["QtyInPieces"].ToString().Trim().Length > 0)
          {
            total3 = total3 + DBConvert.ParseDouble(dtRow["QtyInPieces"].ToString());
          }

          if (dtRow["QtyIn"] != null && dtRow["QtyIn"].ToString().Trim().Length > 0)
          {
            total4 = total4 + DBConvert.ParseDouble(dtRow["QtyIn"].ToString());
          }

          if (dtRow["QtyOutPieces"] != null && dtRow["QtyOutPieces"].ToString().Trim().Length > 0)
          {
            total5 = total5 + DBConvert.ParseDouble(dtRow["QtyOutPieces"].ToString());
          }

          if (dtRow["QtyOut"] != null && dtRow["QtyOut"].ToString().Trim().Length > 0)
          {
            total6 = total6 + DBConvert.ParseDouble(dtRow["QtyOut"].ToString());
          }

          if (dtRow["EOHPieces"] != null && dtRow["EOHPieces"].ToString().Trim().Length > 0)
          {
            total7 = total7 + DBConvert.ParseDouble(dtRow["EOHPieces"].ToString());
          }

          if (dtRow["EOH"] != null && dtRow["EOH"].ToString().Trim().Length > 0)
          {
            total8 = total8 + DBConvert.ParseDouble(dtRow["EOH"].ToString());
          }
        }
        oXlsReport.Cell("**TotalBOHPieces").Value = total1;
        oXlsReport.Cell("**TotalBOH").Value = total2;
        oXlsReport.Cell("**TotalInPieces").Value = total3;
        oXlsReport.Cell("**TotalIn").Value = total4;
        oXlsReport.Cell("**TotalOutPieces").Value = total5;
        oXlsReport.Cell("**TotalOut").Value = total6;
        oXlsReport.Cell("**TotalEOHPieces").Value = total7;
        oXlsReport.Cell("**TotalEOH").Value = total8;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Stock Balance Base Choose Time
    /// </summary>
    private void ExportStockBalanceBaseChooseTime()
    {
      string strTemplateName = "RPT_WOOD_03_001_06";
      string strSheetName = "Sheet1";
      string strOutFileName = "Stock Balance Base Choose Time";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[1];

      if (drpDateFrom.Value != null && drpDateFrom.Text.ToString().Length > 0)
      {
        arrInput[0] = new DBParameter("@Date", DbType.DateTime, (DateTime)drpDateFrom.Value);
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Input Date");
        return;
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTMonthlyStockBalanceVeneerBaseDateTimeWoods_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
      oXlsReport.Cell("**Time").Value = this.ultMonth.Text.ToString();

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double totalBOHPieces = 0;
        double totalBOH = 0;
        double totalQtyInPieces = 0;
        double totalQtyIn = 0;
        double totalQtyOutPieces = 0;
        double totalQtyOut = 0;
        double totalEOHPieces = 0;
        double totalEOH = 0;

        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B9:M9").Copy();
            oXlsReport.RowInsert(8 + i);
            oXlsReport.Cell("B9:M9", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["ID_SanPham"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
          oXlsReport.Cell("**BOHPieces", 0, i).Value = DBConvert.ParseDouble(dtRow["BOHPieces"].ToString());
          oXlsReport.Cell("**BOH", 0, i).Value = DBConvert.ParseDouble(dtRow["BOH"].ToString());
          oXlsReport.Cell("**InPieces", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyInPieces"].ToString());
          oXlsReport.Cell("**In", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyIn"].ToString());
          oXlsReport.Cell("**OutPieces", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyOutPieces"].ToString());
          oXlsReport.Cell("**Out", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyOut"].ToString());
          oXlsReport.Cell("**EOHPieces", 0, i).Value = DBConvert.ParseDouble(dtRow["EOHPieces"].ToString());
          oXlsReport.Cell("**EOH", 0, i).Value = DBConvert.ParseDouble(dtRow["EOH"].ToString());

          if (dtRow["BOHPieces"] != null && dtRow["BOHPieces"].ToString().Trim().Length > 0)
          {
            totalBOHPieces = totalBOHPieces + DBConvert.ParseDouble(dtRow["BOHPieces"].ToString());
          }

          if (dtRow["BOH"] != null && dtRow["BOH"].ToString().Trim().Length > 0)
          {
            totalBOH = totalBOH + DBConvert.ParseDouble(dtRow["BOH"].ToString());
          }

          if (dtRow["QtyInPieces"] != null && dtRow["QtyInPieces"].ToString().Trim().Length > 0)
          {
            totalQtyInPieces = totalQtyInPieces + DBConvert.ParseDouble(dtRow["QtyInPieces"].ToString());
          }

          if (dtRow["QtyIn"] != null && dtRow["QtyIn"].ToString().Trim().Length > 0)
          {
            totalQtyIn = totalQtyIn + DBConvert.ParseDouble(dtRow["QtyIn"].ToString());
          }

          if (dtRow["QtyOutPieces"] != null && dtRow["QtyOutPieces"].ToString().Trim().Length > 0)
          {
            totalQtyOutPieces = totalQtyOutPieces + DBConvert.ParseDouble(dtRow["QtyOutPieces"].ToString());
          }

          if (dtRow["QtyOut"] != null && dtRow["QtyOut"].ToString().Trim().Length > 0)
          {
            totalQtyOut = totalQtyOut + DBConvert.ParseDouble(dtRow["QtyOut"].ToString());
          }

          if (dtRow["EOHPieces"] != null && dtRow["EOHPieces"].ToString().Trim().Length > 0)
          {
            totalEOHPieces = totalEOHPieces + DBConvert.ParseDouble(dtRow["EOHPieces"].ToString());
          }

          if (dtRow["EOH"] != null && dtRow["EOH"].ToString().Trim().Length > 0)
          {
            totalEOH = totalEOH + DBConvert.ParseDouble(dtRow["EOH"].ToString());
          }
        }
        oXlsReport.Cell("**TotalBOHPieces").Value = totalBOHPieces;
        oXlsReport.Cell("**TotalBOH").Value = totalBOH;
        oXlsReport.Cell("**TotalInPieces").Value = totalQtyInPieces;
        oXlsReport.Cell("**TotalIn").Value = totalQtyIn;
        oXlsReport.Cell("**TotalOutPieces").Value = totalQtyOutPieces;
        oXlsReport.Cell("**TotalOut").Value = totalQtyOut;
        oXlsReport.Cell("**TotalEOHPieces").Value = totalEOHPieces;
        oXlsReport.Cell("**TotalEOH").Value = totalEOH;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export ConsumptionVeneer
    /// </summary>
    private void ExportConsumptionVeneer()
    {
      if (txtAverage.Text.Length == 0 || (txtAverage.Text.Length > 0 && DBConvert.ParseInt(txtAverage.Text) != int.MinValue && DBConvert.ParseInt(txtAverage.Text) > 0 && DBConvert.ParseInt(txtAverage.Text) <= 6))
      {
        string commandText = string.Empty;
        string column1 = string.Empty;
        string column2 = string.Empty;
        DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTGetConsumptionWoods");
        for (int i = 5; i > 0; i--)
        {
          DateTime LastMonthDate = DateTime.Now.AddMonths(-i);
          commandText = string.Empty;
          commandText += " SELECT MSD.MaterialCode, MSD.QtyIn, MSD.QtyOut";
          commandText += " FROM TblWHDMonthlySummary MS";
          commandText += " 	INNER JOIN TblWHDMonthlySummaryDetail MSD ON MS.PID = MSD.MonthlySummaryPid";
          commandText += " WHERE [Month] = " + LastMonthDate.Month + " AND [Year] = " + LastMonthDate.Year;
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

          column1 = LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString() + " QtyIn";
          column2 = LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString() + " QtyOut";
          dtData.Columns.Add(column1, typeof(System.Double));
          dtData.Columns.Add(column2, typeof(System.Double));
          foreach (DataRow row in dtData.Rows)
          {
            string materialCode = row["ID_SanPham"].ToString();
            DataRow[] foundRows = dt.Select("MaterialCode = '" + materialCode + "'");
            if (foundRows.Length == 0)
            {
              continue;
            }

            row[column1] = DBConvert.ParseDouble(foundRows[0]["QtyIn"].ToString());
            row[column2] = DBConvert.ParseDouble(foundRows[0]["QtyOut"].ToString());
          }
        }

        column1 = DateTime.Now.Month.ToString() + '/' + DateTime.Now.Year.ToString() + " QtyIn";
        column2 = DateTime.Now.Month.ToString() + '/' + DateTime.Now.Year.ToString() + " QtyOut";
        dtData.Columns.Add(column1, typeof(System.Double));
        dtData.Columns.Add(column2, typeof(System.Double));

        DBParameter[] arrInput = new DBParameter[1];
        arrInput[0] = new DBParameter("@Date", DbType.DateTime, (DateTime)DateTime.Now);

        DataTable dtCurrent = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTMonthlyStockBalanceVeneerBaseDateTimeWoods_Select", arrInput);
        foreach (DataRow row in dtData.Rows)
        {
          string materialCode = row["ID_SanPham"].ToString();
          DataRow[] foundRows = dtCurrent.Select("ID_SanPham = '" + materialCode + "'");
          if (foundRows.Length == 0)
          {
            continue;
          }

          row[column1] = DBConvert.ParseDouble(foundRows[0]["QtyIn"].ToString());
          row[column2] = DBConvert.ParseDouble(foundRows[0]["QtyOut"].ToString());
        }

        string strTemplateName = "RPT_WOOD_03_001_11";
        string strSheetName = "Sheet1";
        string strOutFileName = "Consumption Veneer Report";
        string strStartupPath = System.Windows.Forms.Application.StartupPath;
        string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
        string strPathTemplate = strStartupPath + @"\ExcelTemplate";
        XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

        if (dtData != null && dtData.Rows.Count > 0)
        {
          oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

          oXlsReport.Cell("**MonthQtyIn(7)").Value = dtData.Columns[5].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyOut(7)").Value = dtData.Columns[6].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyIn(8)").Value = dtData.Columns[7].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyOut(8)").Value = dtData.Columns[8].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyIn(9)").Value = dtData.Columns[9].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyOut(9)").Value = dtData.Columns[10].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyIn(10)").Value = dtData.Columns[11].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyOut(10)").Value = dtData.Columns[12].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyIn(11)").Value = dtData.Columns[13].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyOut(11)").Value = dtData.Columns[14].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyIn(12)").Value = dtData.Columns[15].ColumnName.ToString();
          oXlsReport.Cell("**MonthQtyOut(12)").Value = dtData.Columns[16].ColumnName.ToString();

          for (int i = 0; i < dtData.Rows.Count; i++)
          {
            DataRow dtRow = dtData.Rows[i];
            if (i > 0)
            {
              oXlsReport.Cell("B8:U8").Copy();
              oXlsReport.RowInsert(7 + i);
              oXlsReport.Cell("B8:U8", 0, i).Paste();
            }
            oXlsReport.Cell("**No", 0, i).Value = i + 1;
            oXlsReport.Cell("**Material", 0, i).Value = dtRow["ID_SanPham"].ToString();
            oXlsReport.Cell("**Name", 0, i).Value = dtRow["TenEnglish"].ToString();
            oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
            oXlsReport.Cell("**Price(PO)", 0, i).Value = DBConvert.ParseDouble(dtRow["GiaTien"].ToString());
            oXlsReport.Cell("**Balance(M2)", 0, i).Value = DBConvert.ParseDouble(dtRow["Balance(M2)"].ToString());
            if (DBConvert.ParseDouble(dtRow[5].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyIn(7)", 0, i).Value = DBConvert.ParseDouble(dtRow[5].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyIn(7)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[6].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyOut(7)", 0, i).Value = DBConvert.ParseDouble(dtRow[6].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyOut(7)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[7].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyIn(8)", 0, i).Value = DBConvert.ParseDouble(dtRow[7].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyIn(8)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[8].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyOut(8)", 0, i).Value = DBConvert.ParseDouble(dtRow[8].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyOut(8)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[9].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyIn(9)", 0, i).Value = DBConvert.ParseDouble(dtRow[9].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyIn(9)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[10].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyOut(9)", 0, i).Value = DBConvert.ParseDouble(dtRow[10].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyOut(9)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[11].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyIn(10)", 0, i).Value = DBConvert.ParseDouble(dtRow[11].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyIn(10)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[12].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyOut(10)", 0, i).Value = DBConvert.ParseDouble(dtRow[12].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyOut(10)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[13].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyIn(11)", 0, i).Value = DBConvert.ParseDouble(dtRow[13].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyIn(11)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[14].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyOut(11)", 0, i).Value = DBConvert.ParseDouble(dtRow[14].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyOut(11)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[15].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyIn(12)", 0, i).Value = DBConvert.ParseDouble(dtRow[15].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyIn(12)", 0, i).Value = 0;
            }
            if (DBConvert.ParseDouble(dtRow[16].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**QtyOut(12)", 0, i).Value = DBConvert.ParseDouble(dtRow[16].ToString());
            }
            else
            {
              oXlsReport.Cell("**QtyOut(12)", 0, i).Value = 0;
            }

            int k = dtData.Columns.Count;
            int average = int.MinValue;
            int coutAverage = 0;
            int coutAverage1 = 0;
            double sum = 0;
            double sum1 = 0;
            if (txtAverage.Text.Length > 0)
            {
              average = DBConvert.ParseInt(txtAverage.Text);
            }
            else
            {
              average = 6;
            }
            for (int m = k; m > (k - (average * 2)); m = m - 1)
            {
              if (m % 2 == 0)
              {
                if (DBConvert.ParseDouble(dtRow[m - 1].ToString()) != double.MinValue && DBConvert.ParseDouble(dtRow[m - 1].ToString()) > 0)
                {
                  sum1 = sum1 + DBConvert.ParseDouble(dtRow[m - 1].ToString());
                  coutAverage1 = coutAverage1 + 1;
                }
              }
              else
              {
                if (DBConvert.ParseDouble(dtRow[m - 1].ToString()) != double.MinValue && DBConvert.ParseDouble(dtRow[m - 1].ToString()) > 0)
                {
                  sum = sum + DBConvert.ParseDouble(dtRow[m - 1].ToString());
                  coutAverage = coutAverage + 1;
                }
              }
            }
            if (sum > 0 && sum / coutAverage != double.MinValue)
            {
              oXlsReport.Cell("**QtyOut(Average)", 0, i).Value = sum / coutAverage;
            }
            else
            {
              oXlsReport.Cell("**QtyOut(Average)", 0, i).Value = 0;
            }
            if (sum1 > 0 && sum1 / coutAverage1 != double.MinValue)
            {
              oXlsReport.Cell("**QtyIn(Average)", 0, i).Value = sum1 / coutAverage1;
            }
            else
            {
              oXlsReport.Cell("**QtyIn(Average)", 0, i).Value = 0;
            }
          }
        }
        oXlsReport.Out.File(strOutFileName);
        Process.Start(strOutFileName);
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Average > 0 && Average <= 6");
        return;
      }
    }

    /// <summary>
    /// Export Stock Movement
    /// </summary>
    private void ExportStockMovement()
    {
      string strTemplateName = "RPT_WOOD_03_001_07";
      string strSheetName = "Sheet1";
      string strOutFileName = "Stock Movement";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[5];

      long locationPid = long.MinValue;
      string materialCode = string.Empty;

      //Create Date
      DateTime prDateFrom = DateTime.MinValue;
      if (drpDateFrom.Value != null)
      {
        prDateFrom = (DateTime)drpDateFrom.Value;
      }
      DateTime prDateTo = DateTime.MinValue;
      if (drpDateTo.Value != null)
      {
        prDateTo = (DateTime)drpDateTo.Value;
      }

      if (prDateFrom != DateTime.MinValue)
      {
        arrInput[0] = new DBParameter("@CreateDateFrom", DbType.DateTime, prDateFrom);
      }

      if (prDateTo != DateTime.MinValue)
      {
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        arrInput[1] = new DBParameter("@CreateDateTo", DbType.DateTime, prDateTo);
      }

      // Location
      if (this.ultLocation.Value != null)
      {
        locationPid = DBConvert.ParseInt(this.ultLocation.Value.ToString());
        if (locationPid != long.MinValue)
        {
          arrInput[2] = new DBParameter("@LocationPid", DbType.Int64, locationPid);
        }
      }
      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
        if (materialCode.Length > 0)
        {
          arrInput[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
        }
      }

      if (this.ultSupplier.Value != null)
      {
        arrInput[4] = new DBParameter("Source", DbType.String, this.ultSupplier.Value.ToString());
      }
      else if (this.ultDepartment.Value != null)
      {
        arrInput[4] = new DBParameter("Source", DbType.String, this.ultDepartment.Value.ToString());
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTStockMovementWoods_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double total = 0;
        double totalQtyPcs = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:N8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:N8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**SubDate", 0, i).Value = dtRow["TrDate"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["TenEnglish"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["TenVietNam"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
          if (DBConvert.ParseDouble(dtRow["QtyPcs"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**QtyPcs", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyPcs"].ToString());
          }
          else
          {
            oXlsReport.Cell("**QtyPcs", 0, i).Value = "";
          }
          if (DBConvert.ParseDouble(dtRow["Qty"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = "";
          }
          oXlsReport.Cell("**Source", 0, i).Value = dtRow["ReceivingCode"].ToString();
          oXlsReport.Cell("**Invoice", 0, i).Value = dtRow["Invoice"].ToString();

          if (dtRow["Title"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Title", 0, i).Value = dtRow["Title"].ToString();
          }
          else
          {
            oXlsReport.Cell("**Title", 0, i).Value = "";
          }

          oXlsReport.Cell("**[Supplier/Department]", 0, i).Value = dtRow["Supplier/Department"].ToString();

          if (dtRow["Remark"].ToString().Length > 0)
          {
            oXlsReport.Cell("**WO", 0, i).Value = dtRow["Remark"].ToString();
          }
          else
          {
            oXlsReport.Cell("**WO", 0, i).Value = "";
          }

          if (dtRow["Qty"] != null && dtRow["Qty"].ToString().Trim().Length > 0)
          {
            total = total + DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }

          if (dtRow["QtyPcs"] != null && dtRow["QtyPcs"].ToString().Trim().Length > 0)
          {
            totalQtyPcs = totalQtyPcs + DBConvert.ParseDouble(dtRow["QtyPcs"].ToString());
          }
        }
        oXlsReport.Cell("**TotalQtyPcs").Value = totalQtyPcs;
        oXlsReport.Cell("**TotalQty").Value = total;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Stock Movement Detail Allocate Data
    /// </summary>
    private void ExportStockMovementDetailAllocateData()
    {
      string strTemplateName = "RPT_WOOD_03_001_17";
      string strSheetName = "Sheet1";
      string strOutFileName = "Stock Movement Detail With Allocate Data";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[6];

      long locationPid = long.MinValue;
      string materialCode = string.Empty;

      //Create Date
      DateTime prDateFrom = DateTime.MinValue;
      if (drpDateFrom.Value != null)
      {
        prDateFrom = (DateTime)drpDateFrom.Value;
      }
      DateTime prDateTo = DateTime.MinValue;
      if (drpDateTo.Value != null)
      {
        prDateTo = (DateTime)drpDateTo.Value;
      }

      if (prDateFrom != DateTime.MinValue)
      {
        arrInput[0] = new DBParameter("@CreateDateFrom", DbType.DateTime, prDateFrom);
      }

      if (prDateTo != DateTime.MinValue)
      {
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        arrInput[1] = new DBParameter("@CreateDateTo", DbType.DateTime, prDateTo);
      }

      // Location
      if (this.ultLocation.Value != null)
      {
        locationPid = DBConvert.ParseInt(this.ultLocation.Value.ToString());
        if (locationPid != long.MinValue)
        {
          arrInput[2] = new DBParameter("@LocationPid", DbType.Int64, locationPid);
        }
      }
      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
        if (materialCode.Length > 0)
        {
          arrInput[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
        }
      }

      // Supplier/ Department
      if (this.ultSupplier.Value != null)
      {
        arrInput[4] = new DBParameter("@Source", DbType.String, this.ultSupplier.Value.ToString());
      }
      else if (this.ultDepartment.Value != null)
      {
        arrInput[4] = new DBParameter("@Source", DbType.String, this.ultDepartment.Value.ToString());
      }

      if (txtLotNoId.Text.Trim().ToString().Length > 0)
      {
        arrInput[5] = new DBParameter("@LotNoId", DbType.AnsiString, 255, this.txtLotNoId.Text.ToString());
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTStockMovementDetailWoodsWithAllocate_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double total = 0;
        double totalQtyM2 = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:R8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:R8", 0, i).Paste();
          }

          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**TrDate", 0, i).Value = dtRow["TrDate"].ToString();
          oXlsReport.Cell("**LotNoId", 0, i).Value = dtRow["LotNoId"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();

          if (dtRow["WO"].ToString().Length > 0)
          {
            oXlsReport.Cell("**WO", 0, i).Value = dtRow["WO"].ToString();
          }

          if (dtRow["CarcassCode"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Carcass", 0, i).Value = dtRow["CarcassCode"].ToString();
          }

          if (dtRow["SupplementNo"].ToString().Length > 0)
          {
            oXlsReport.Cell("**Supplement", 0, i).Value = dtRow["SupplementNo"].ToString();
          }

          oXlsReport.Cell("**QtyM2", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
          oXlsReport.Cell("**Package", 0, i).Value = dtRow["Package"].ToString();
          oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();
          oXlsReport.Cell("**Source", 0, i).Value = dtRow["Source"].ToString();
          oXlsReport.Cell("**Title", 0, i).Value = dtRow["Title"].ToString();
          oXlsReport.Cell("**[Supplier/Department]", 0, i).Value = dtRow["Supplier/Department"].ToString();
          oXlsReport.Cell("**Invoice", 0, i).Value = dtRow["Invoice"].ToString();
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Stock Movement Detail
    /// </summary>
    private void ExportStockMovementDetail()
    {
      string strTemplateName = "RPT_WOOD_03_001_08";
      string strSheetName = "Sheet1";
      string strOutFileName = "Stock Movement Detail";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[6];

      long locationPid = long.MinValue;
      string materialCode = string.Empty;

      //Create Date
      DateTime prDateFrom = DateTime.MinValue;
      if (drpDateFrom.Value != null)
      {
        prDateFrom = (DateTime)drpDateFrom.Value;
      }
      DateTime prDateTo = DateTime.MinValue;
      if (drpDateTo.Value != null)
      {
        prDateTo = (DateTime)drpDateTo.Value;
      }

      if (prDateFrom != DateTime.MinValue)
      {
        arrInput[0] = new DBParameter("@CreateDateFrom", DbType.DateTime, prDateFrom);
      }

      if (prDateTo != DateTime.MinValue)
      {
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        arrInput[1] = new DBParameter("@CreateDateTo", DbType.DateTime, prDateTo);
      }

      // Location
      if (this.ultLocation.Value != null)
      {
        locationPid = DBConvert.ParseInt(this.ultLocation.Value.ToString());
        if (locationPid != long.MinValue)
        {
          arrInput[2] = new DBParameter("@LocationPid", DbType.Int64, locationPid);
        }
      }
      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
        if (materialCode.Length > 0)
        {
          arrInput[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
        }
      }

      // Supplier/ Department
      if (this.ultSupplier.Value != null)
      {
        arrInput[4] = new DBParameter("@Source", DbType.String, this.ultSupplier.Value.ToString());
      }
      else if (this.ultDepartment.Value != null)
      {
        arrInput[4] = new DBParameter("@Source", DbType.String, this.ultDepartment.Value.ToString());
      }

      if (txtLotNoId.Text.Trim().ToString().Length > 0)
      {
        arrInput[5] = new DBParameter("@LotNoId", DbType.AnsiString, 255, this.txtLotNoId.Text.ToString());
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTStockMovementDetailWoods_Select", arrInput);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double total = 0;
        double totalQtyM2 = 0;
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:V8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:V8", 0, i).Paste();
          }

          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**TrDate", 0, i).Value = dtRow["TrDate"].ToString();
          oXlsReport.Cell("**LotNoId", 0, i).Value = dtRow["LotNoId"].ToString();
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
          oXlsReport.Cell("**Length", 0, i).Value = DBConvert.ParseDouble(dtRow["Length"].ToString());
          oXlsReport.Cell("**Width", 0, i).Value = DBConvert.ParseDouble(dtRow["Width"].ToString());
          oXlsReport.Cell("**Thickness", 0, i).Value = DBConvert.ParseDouble(dtRow["Thickness"].ToString());
          oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseDouble(dtRow["Qty"].ToString());
          oXlsReport.Cell("**QtyM2", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
          oXlsReport.Cell("**Package", 0, i).Value = dtRow["Package"].ToString();
          oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();
          oXlsReport.Cell("**Source", 0, i).Value = dtRow["Source"].ToString();
          oXlsReport.Cell("**Title", 0, i).Value = dtRow["Title"].ToString();
          oXlsReport.Cell("**[Supplier/Department]", 0, i).Value = dtRow["Supplier/Department"].ToString();
          oXlsReport.Cell("**Invoice", 0, i).Value = dtRow["Invoice"].ToString();

          if (dtRow["Qty"] != null && dtRow["Qty"].ToString().Trim().Length > 0)
          {
            total = total + DBConvert.ParseDouble(dtRow["Qty"].ToString());
          }
          if (dtRow["QtyM2"] != null && dtRow["QtyM2"].ToString().Trim().Length > 0)
          {
            totalQtyM2 = totalQtyM2 + DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
          }
        }
        oXlsReport.Cell("**TotalQty").Value = total;
        oXlsReport.Cell("**TotalQtyM2").Value = totalQtyM2;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Ageing
    /// </summary>
    private void ExportAgeing()
    {
      string strTemplateName = string.Empty;
      if (checkDepartment == 1)
      {
        strTemplateName = "RPT_WOOD_03_001_14";
      }
      else
      {
        strTemplateName = "RPT_WOOD_03_001_09";
      }
      string strSheetName = "Sheet1";
      string strOutFileName = "Ageing";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //Search
      DBParameter[] arrInput = new DBParameter[1];

      string materialCode = string.Empty;

      // Material Code
      if (this.ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
        if (materialCode.Length > 0)
        {
          arrInput[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
        }
      }
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTAgeingWoods_Select", arrInput);
      DateTime LastMonthDate;
      for (int i = 11; i > 0; i--)
      {
        LastMonthDate = DateTime.Now.AddMonths(-i);
        string commandText = string.Empty;
        commandText += " SELECT MSD.MaterialCode, MSD.QtyOut";
        commandText += " FROM TblWHDMonthlySummary MS";
        commandText += " 	INNER JOIN TblWHDMonthlySummaryDetail MSD ON MS.PID = MSD.MonthlySummaryPid";
        commandText += " WHERE [Month] = " + LastMonthDate.Month + " AND [Year] = " + LastMonthDate.Year;
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

        string column1 = LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString();

        dtData.Columns.Add(column1, typeof(System.Double));
        foreach (DataRow row in dtData.Rows)
        {
          materialCode = row["MaterialCode"].ToString();
          DataRow[] foundRows = dt.Select("MaterialCode = '" + materialCode + "'");
          if (foundRows.Length == 0)
          {
            continue;
          }

          row[column1] = DBConvert.ParseDouble(foundRows[0]["QtyOut"].ToString());
        }
      }

      dtData.Columns.Add("MonthlyConsumption", typeof(System.Double));
      dtData.Columns.Add("MinOfStock", typeof(System.Double));

      for (int j = 0; j < dtData.Rows.Count; j++)
      {
        DataRow row = dtData.Rows[j];
        if (DBConvert.ParseInt(row["TrungBinhTon"].ToString()) != int.MinValue)
        {
          double sumOut = 0;
          double maxOut = 0;
          double minOut = 0;
          for (int k = 11; k > 11 - DBConvert.ParseInt(row["TrungBinhTon"].ToString()); k--)
          {
            if (DBConvert.ParseDouble(row[k + 22].ToString()) != double.MinValue)
            {
              double value = 0;
              if (DBConvert.ParseDouble(row[k + 22].ToString()) == double.MinValue)
              {
                value = 0;
              }
              else
              {
                value = DBConvert.ParseDouble(row[k + 22].ToString());
              }

              //Set init Default for MinOut
              if (minOut == 0)
              {
                minOut = value;
              }

              sumOut += value;
              if (maxOut < value)
              {
                maxOut = value;
              }

              if (minOut > value)
              {
                minOut = value;
              }
            }
          }

          // Formula Sum
          if (DBConvert.ParseInt(row["Average"].ToString()) == 0)
          {
            row["MonthlyConsumption"] = DBConvert.ParseDouble(sumOut / DBConvert.ParseInt(row["TrungBinhTon"].ToString()));
          }
          // Formula Max
          else if (DBConvert.ParseInt(row["Average"].ToString()) == 1)
          {
            row["MonthlyConsumption"] = maxOut;
          }
          // Formula Min
          else if (DBConvert.ParseInt(row["Average"].ToString()) == 2)
          {
            row["MonthlyConsumption"] = minOut;
          }
          // By Default Min Of Stock 
          else
          {
            row["MonthlyConsumption"] = 0;
          }

          if (DBConvert.ParseInt(row["Average"].ToString()) == -1)
          {
            row["MinOfStock"] = DBConvert.ParseDouble(row["MinDefault"].ToString());
          }
          else
          {
            row["MinOfStock"] = DBConvert.ParseDouble(row["MonthlyConsumption"].ToString())
                                    * DBConvert.ParseDouble(row["HeSoTon"].ToString())
                                    * DBConvert.ParseDouble(row["MinimumStock"].ToString());
          }
          row["MinOfStock"] = Math.Round(DBConvert.ParseDouble(row["MinOfStock"].ToString()), 4);
        }
      }

      oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
      oXlsReport.Cell("**Month").Value = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();

      if (dtData != null && dtData.Rows.Count > 0)
      {
        double total1 = 0;
        double total2 = 0;
        double total3 = 0;
        double total4 = 0;
        double total5 = 0;
        double total6 = 0;
        double total7 = 0;
        double total8 = 0;
        double total9 = 0;
        double total10 = 0;
        double total11 = 0;
        double total12 = 0;
        double total13 = 0;

        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            if (checkDepartment == 1)
            {
              oXlsReport.Cell("A7:U7").Copy();
              oXlsReport.RowInsert(6 + i);
              oXlsReport.Cell("A7:U7", 0, i).Paste();
            }
            else
            {
              oXlsReport.Cell("A7:AH7").Copy();
              oXlsReport.RowInsert(6 + i);
              oXlsReport.Cell("A7:AH7", 0, i).Paste();
            }
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**Code", 0, i).Value = dtRow["MaterialCode"].ToString();
          oXlsReport.Cell("**Name", 0, i).Value = dtRow["NameEN"].ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
          oXlsReport.Cell("**MinMax", 0, i).Value = dtRow["Min-Max"].ToString();
          if (dtRow["HeSoTon"] != null && DBConvert.ParseDouble(dtRow["HeSoTon"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["HeSoTon"].ToString()) > 0)
          {
            oXlsReport.Cell("**LeadTime", 0, i).Value = DBConvert.ParseDouble(dtRow["HeSoTon"].ToString());
          }
          else
          {
            oXlsReport.Cell("**LeadTime", 0, i).Value = "";
          }

          if (dtRow["MinOfStock"] != null && DBConvert.ParseDouble(dtRow["MinOfStock"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["MinOfStock"].ToString()) >= 0)
          {
            oXlsReport.Cell("**SafetyStock", 0, i).Value = DBConvert.ParseDouble(dtRow["MinOfStock"].ToString());
          }
          else
          {
            oXlsReport.Cell("**SafetyStock", 0, i).Value = "";
          }

          if (dtRow["0-1 Week"] != null && DBConvert.ParseDouble(dtRow["0-1 Week"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["0-1 Week"].ToString()) > 0)
          {
            oXlsReport.Cell("**1", 0, i).Value = DBConvert.ParseDouble(dtRow["0-1 Week"].ToString());
          }
          else
          {
            oXlsReport.Cell("**1", 0, i).Value = "";
          }
          // Amount 0-1 Week
          if (dtRow["Amount 0-1 Week"] != null && DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString()) != double.MinValue
                 && DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString()) > 0)
          {
            oXlsReport.Cell("**1a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString());
          }
          else
          {
            oXlsReport.Cell("**1a", 0, i).Value = "";
          }

          if (dtRow["1-2 Weeks"] != null && DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString()) != double.MinValue
                  && DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString()) > 0)
          {
            oXlsReport.Cell("**2", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString());
          }
          else
          {
            oXlsReport.Cell("**2", 0, i).Value = "";
          }

          // Amount 1-2 Weeks
          if (dtRow["Amount 1-2 Weeks"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Weeks"].ToString()) != double.MinValue
                 && DBConvert.ParseDouble(dtRow["Amount 1-2 Weeks"].ToString()) > 0)
          {
            oXlsReport.Cell("**2a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Weeks"].ToString());
          }
          else
          {
            oXlsReport.Cell("**2a", 0, i).Value = "";
          }

          if (dtRow["2-3 Weeks"] != null && DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString()) > 0)
          {
            oXlsReport.Cell("**3", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString());
          }
          else
          {
            oXlsReport.Cell("**3", 0, i).Value = "";
          }
          // Amount 2-3 Weeks
          if (dtRow["Amount 2-3 Weeks"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Weeks"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["Amount 2-3 Weeks"].ToString()) > 0)
          {
            oXlsReport.Cell("**3a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Weeks"].ToString());
          }
          else
          {
            oXlsReport.Cell("**3a", 0, i).Value = "";
          }

          if (dtRow["3-4 Weeks"] != null && DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString()) > 0)
          {
            oXlsReport.Cell("**4", 0, i).Value = DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString());
          }
          else
          {
            oXlsReport.Cell("**4", 0, i).Value = "";
          }
          // Amount 3-4 Weeks
          if (dtRow["Amount 3-4 Weeks"] != null && DBConvert.ParseDouble(dtRow["Amount 3-4 Weeks"].ToString()) != double.MinValue
               && DBConvert.ParseDouble(dtRow["Amount 3-4 Weeks"].ToString()) > 0)
          {
            oXlsReport.Cell("**4a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 3-4 Weeks"].ToString());
          }
          else
          {
            oXlsReport.Cell("**4a", 0, i).Value = "";
          }

          if (dtRow["1-2 Months"] != null && DBConvert.ParseDouble(dtRow["1-2 Months"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["1-2 Months"].ToString()) > 0)
          {
            oXlsReport.Cell("**5", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Months"].ToString());
          }
          else
          {
            oXlsReport.Cell("**5", 0, i).Value = "";
          }
          // Amount 1-2 Months
          if (dtRow["Amount 1-2 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString()) > 0)
          {
            oXlsReport.Cell("**5a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString());
          }
          else
          {
            oXlsReport.Cell("**5a", 0, i).Value = "";
          }


          if (dtRow["2-3 Months"] != null && DBConvert.ParseDouble(dtRow["2-3 Months"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["2-3 Months"].ToString()) > 0)
          {
            oXlsReport.Cell("**6", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Months"].ToString());
          }
          else
          {
            oXlsReport.Cell("**6", 0, i).Value = "";
          }
          // Amount 2-3 Months
          if (dtRow["Amount 2-3 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString()) > 0)
          {
            oXlsReport.Cell("**6a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString());
          }
          else
          {
            oXlsReport.Cell("**6a", 0, i).Value = "";
          }

          if (dtRow["3-6 Months"] != null && DBConvert.ParseDouble(dtRow["3-6 Months"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["3-6 Months"].ToString()) > 0)
          {
            oXlsReport.Cell("**7", 0, i).Value = DBConvert.ParseDouble(dtRow["3-6 Months"].ToString());
          }
          else
          {
            oXlsReport.Cell("**7", 0, i).Value = "";
          }
          // Amount 3-6 Months
          if (dtRow["Amount 3-6 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString()) != double.MinValue
              && DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString()) > 0)
          {
            oXlsReport.Cell("**7a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString());
          }
          else
          {
            oXlsReport.Cell("**7a", 0, i).Value = "";
          }

          if (dtRow["6-9 Mths"] != null && DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString()) > 0)
          {
            oXlsReport.Cell("**8", 0, i).Value = DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString());
          }
          else
          {
            oXlsReport.Cell("**8", 0, i).Value = "";
          }
          // Amount 6-9 Months
          if (dtRow["Amount 6-9 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString()) != double.MinValue
             && DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString()) > 0)
          {
            oXlsReport.Cell("**8a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString());
          }
          else
          {
            oXlsReport.Cell("**8a", 0, i).Value = "";
          }

          if (dtRow["9-12 Mths"] != null && DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString()) > 0)
          {
            oXlsReport.Cell("**9", 0, i).Value = DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString());
          }
          else
          {
            oXlsReport.Cell("**9", 0, i).Value = "";
          }
          // Amount 9-12 Months
          if (dtRow["Amount 9-12 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString()) > 0)
          {
            oXlsReport.Cell("**9a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString());
          }
          else
          {
            oXlsReport.Cell("**9a", 0, i).Value = "";
          }

          if (dtRow["1-2 Years"] != null && DBConvert.ParseDouble(dtRow["1-2 Years"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["1-2 Years"].ToString()) > 0)
          {
            oXlsReport.Cell("**10", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Years"].ToString());
          }
          else
          {
            oXlsReport.Cell("**10", 0, i).Value = "";
          }
          // Amount 1-2 Years
          if (dtRow["Amount 1-2 Years"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString()) > 0)
          {
            oXlsReport.Cell("**10a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString());
          }
          else
          {
            oXlsReport.Cell("**10a", 0, i).Value = "";
          }

          if (dtRow["2-3 Years"] != null && DBConvert.ParseDouble(dtRow["2-3 Years"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["2-3 Years"].ToString()) > 0)
          {
            oXlsReport.Cell("**11", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Years"].ToString());
          }
          else
          {
            oXlsReport.Cell("**11", 0, i).Value = "";
          }
          // Amount 2-3 Years
          if (dtRow["Amount 2-3 Years"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString()) > 0)
          {
            oXlsReport.Cell("**11a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString());
          }
          else
          {
            oXlsReport.Cell("**11a", 0, i).Value = "";
          }

          if (dtRow["Over 3 Years"] != null && DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString()) > 0)
          {
            oXlsReport.Cell("**12", 0, i).Value = DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString());
          }
          else
          {
            oXlsReport.Cell("**12", 0, i).Value = "";
          }
          // Amount Over 3 Years
          if (dtRow["Amount Over 3 Years"] != null && DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString()) > 0)
          {
            oXlsReport.Cell("**12a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString());
          }
          else
          {
            oXlsReport.Cell("**12a", 0, i).Value = "";
          }

          if (dtRow["Total"] != null && DBConvert.ParseDouble(dtRow["Total"].ToString()) != double.MinValue
                && DBConvert.ParseDouble(dtRow["Total"].ToString()) > 0)
          {
            oXlsReport.Cell("**13", 0, i).Value = DBConvert.ParseDouble(dtRow["Total"].ToString());
          }
          else
          {
            oXlsReport.Cell("**13", 0, i).Value = "";
          }
          // Total Amount
          if (dtRow["TotalAmount"] != null && DBConvert.ParseDouble(dtRow["TotalAmount"].ToString()) != double.MinValue
               && DBConvert.ParseDouble(dtRow["TotalAmount"].ToString()) > 0)
          {
            oXlsReport.Cell("**13a", 0, i).Value = DBConvert.ParseDouble(dtRow["TotalAmount"].ToString());
          }
          else
          {
            oXlsReport.Cell("**13a", 0, i).Value = "";
          }

          if (dtRow["0-1 Week"] != null && dtRow["0-1 Week"].ToString().Trim().Length > 0)
          {
            total1 = total1 + DBConvert.ParseDouble(dtRow["0-1 Week"].ToString());
          }
          if (dtRow["1-2 Weeks"] != null && dtRow["1-2 Weeks"].ToString().Trim().Length > 0)
          {
            total2 = total2 + DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString());
          }
          if (dtRow["2-3 Weeks"] != null && dtRow["2-3 Weeks"].ToString().Trim().Length > 0)
          {
            total3 = total3 + DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString());
          }
          if (dtRow["3-4 Weeks"] != null && dtRow["3-4 Weeks"].ToString().Trim().Length > 0)
          {
            total4 = total4 + DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString());
          }
          if (dtRow["1-2 Months"] != null && dtRow["1-2 Months"].ToString().Trim().Length > 0)
          {
            total5 = total5 + DBConvert.ParseDouble(dtRow["1-2 Months"].ToString());
          }
          if (dtRow["2-3 Months"] != null && dtRow["2-3 Months"].ToString().Trim().Length > 0)
          {
            total6 = total6 + DBConvert.ParseDouble(dtRow["2-3 Months"].ToString());
          }
          if (dtRow["3-6 Months"] != null && dtRow["3-6 Months"].ToString().Trim().Length > 0)
          {
            total7 = total7 + DBConvert.ParseDouble(dtRow["3-6 Months"].ToString());
          }
          if (dtRow["6-9 Mths"] != null && dtRow["6-9 Mths"].ToString().Trim().Length > 0)
          {
            total8 = total8 + DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString());
          }
          if (dtRow["9-12 Mths"] != null && dtRow["9-12 Mths"].ToString().Trim().Length > 0)
          {
            total9 = total9 + DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString());
          }
          if (dtRow["1-2 Years"] != null && dtRow["1-2 Years"].ToString().Trim().Length > 0)
          {
            total10 = total10 + DBConvert.ParseDouble(dtRow["1-2 Years"].ToString());
          }
          if (dtRow["2-3 Years"] != null && dtRow["2-3 Years"].ToString().Trim().Length > 0)
          {
            total11 = total11 + DBConvert.ParseDouble(dtRow["2-3 Years"].ToString());
          }
          if (dtRow["Over 3 Years"] != null && dtRow["Over 3 Years"].ToString().Trim().Length > 0)
          {
            total12 = total12 + DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString());
          }
          if (dtRow["Total"] != null && dtRow["Total"].ToString().Trim().Length > 0)
          {
            total13 = total13 + Math.Round(DBConvert.ParseDouble(dtRow["Total"].ToString()), 4);
          }
        }
        oXlsReport.Cell("**Total1").Value = total1;
        oXlsReport.Cell("**Total2").Value = total2;
        oXlsReport.Cell("**Total3").Value = total3;
        oXlsReport.Cell("**Total4").Value = total4;
        oXlsReport.Cell("**Total5").Value = total5;
        oXlsReport.Cell("**Total6").Value = total6;
        oXlsReport.Cell("**Total7").Value = total7;
        oXlsReport.Cell("**Total8").Value = total8;
        oXlsReport.Cell("**Total9").Value = total9;
        oXlsReport.Cell("**Total10").Value = total10;
        oXlsReport.Cell("**Total11").Value = total11;
        oXlsReport.Cell("**Total12").Value = total12;
        oXlsReport.Cell("**Total13").Value = total13;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// Export Ageing
    /// </summary>
    private void ExportAgeingMonthly()
    {// Month
      if (this.ultMonth.Value == null || this.ultMonth.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Month");
        return;
      }

      // Year
      if (this.ultYear.Value == null || this.ultYear.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Year");
        return;
      }

      string commandText = string.Empty;
      commandText += " SELECT COUNT(*)";
      commandText += " FROM TblWHDMonthlySummary MS";
      commandText += " 	INNER JOIN TblWHDMonthlySummaryAgeingStore_Wood STO ON MS.PID = STO.MonthlySummaryPid";
      commandText += " WHERE [Month] = " + DBConvert.ParseInt(this.ultMonth.Value.ToString());
      commandText += "  AND [Year] = " + DBConvert.ParseInt(this.ultYear.Value.ToString());
      DataTable dtCheckMonth = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheckMonth != null && dtCheckMonth.Rows.Count > 0)
      {
        string strStartupPath = System.Windows.Forms.Application.StartupPath;
        string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
        string strPathTemplate = strStartupPath + @"\ExcelTemplate";
        string strTemplateName = string.Empty;
        string strSheetName = "Sheet1";
        string strOutFileName = "Ageing";
        XlsReport oXlsReport;

        //Search
        DBParameter[] arrInput = new DBParameter[3];
        int month = DBConvert.ParseInt(ultMonth.Value.ToString());
        arrInput[0] = new DBParameter("@Month", DbType.Int32, month);
        int year = DBConvert.ParseInt(ultYear.Value.ToString());
        arrInput[1] = new DBParameter("@Year", DbType.Int32, year);
        string materialCode = string.Empty;
        // Material Code
        if (this.ultMaterialCode.Value != null)
        {
          materialCode = this.ultMaterialCode.Value.ToString();
          if (materialCode.Length > 0)
          {
            arrInput[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
          }
        }
        // Get Old Template
        if (DBConvert.ParseInt(dtCheckMonth.Rows[0][0].ToString()) == 0)
        {
          if (checkDepartment == 1)
          {
            strTemplateName = "RPT_WOOD_03_001_15";
          }
          else
          {
            strTemplateName = "RPT_WOOD_03_001_12";
          }
          oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

          DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTWoodsAgeingMonthly_Select", arrInput);

          oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
          oXlsReport.Cell("**Month").Value = "Month: " + ultMonth.Value.ToString();
          oXlsReport.Cell("**Year").Value = "Year: " + ultYear.Value.ToString();
          if (dtData != null && dtData.Rows.Count > 0)
          {
            double total1 = 0;
            double total2 = 0;
            double total3 = 0;
            double total4 = 0;
            double total5 = 0;
            double total6 = 0;
            double total7 = 0;
            double total8 = 0;

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
              DataRow dtRow = dtData.Rows[i];
              if (i > 0)
              {
                if (checkDepartment == 1)
                {
                  oXlsReport.Cell("B8:O8").Copy();
                  oXlsReport.RowInsert(7 + i);
                  oXlsReport.Cell("B8:O8", 0, i).Paste();
                }
                else
                {
                  oXlsReport.Cell("B8:W8").Copy();
                  oXlsReport.RowInsert(7 + i);
                  oXlsReport.Cell("B8:W8", 0, i).Paste();
                }
              }
              oXlsReport.Cell("**No", 0, i).Value = i + 1;
              oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
              oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
              oXlsReport.Cell("**NameVN", 0, i).Value = dtRow["NameVN"].ToString();
              oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
              oXlsReport.Cell("**SubDate", 0, i).Value = dtRow["LastRecDate"].ToString();

              if (dtRow["0-3 Mths"] != null && DBConvert.ParseDouble(dtRow["0-3 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty1", 0, i).Value = DBConvert.ParseDouble(dtRow["0-3 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty1", 0, i).Value = "";
              }
              // Amount 0-3 Months
              if (dtRow["Amount 0-3 Mths"] != null && DBConvert.ParseDouble(dtRow["Amount 0-3 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty1a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 0-3 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty1a", 0, i).Value = "";
              }

              if (dtRow["4-6 Mths"] != null && DBConvert.ParseDouble(dtRow["4-6 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty2", 0, i).Value = DBConvert.ParseDouble(dtRow["4-6 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty2", 0, i).Value = "";
              }
              // Amount 4-6 Months
              if (dtRow["Amount 4-6 Mths"] != null && DBConvert.ParseDouble(dtRow["Amount 4-6 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty2a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 4-6 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty2a", 0, i).Value = "";
              }

              if (dtRow["7-9 Mths"] != null && DBConvert.ParseDouble(dtRow["7-9 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty3", 0, i).Value = DBConvert.ParseDouble(dtRow["7-9 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty3", 0, i).Value = "";
              }
              // Amount 7-9 Months
              if (dtRow["Amount 7-9 Mths"] != null && DBConvert.ParseDouble(dtRow["Amount 7-9 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty3a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 7-9 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty3a", 0, i).Value = "";
              }

              if (dtRow["10-12 Mths"] != null && DBConvert.ParseDouble(dtRow["10-12 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty4", 0, i).Value = DBConvert.ParseDouble(dtRow["10-12 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty4", 0, i).Value = "";
              }
              // Amount 10-12 Months
              if (dtRow["Amount 10-12 Mths"] != null && DBConvert.ParseDouble(dtRow["Amount 10-12 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty4a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 10-12 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty4a", 0, i).Value = "";
              }

              if (dtRow["13-24 Mths"] != null && DBConvert.ParseDouble(dtRow["13-24 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty5", 0, i).Value = DBConvert.ParseDouble(dtRow["13-24 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty5", 0, i).Value = "";
              }
              // Amount 13-24 Months
              if (dtRow["Amount 13-24 Mths"] != null && DBConvert.ParseDouble(dtRow["Amount 13-24 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty5a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 13-24 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty5a", 0, i).Value = "";
              }

              if (dtRow["25-36 Mths"] != null && DBConvert.ParseDouble(dtRow["25-36 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty6", 0, i).Value = DBConvert.ParseDouble(dtRow["25-36 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty6", 0, i).Value = "";
              }
              // Amount 25-36 Months
              if (dtRow["Amount 25-36 Mths"] != null && DBConvert.ParseDouble(dtRow["Amount 25-36 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty6a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 25-36 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty6a", 0, i).Value = "";
              }

              if (dtRow["Over 36 Mths"] != null && DBConvert.ParseDouble(dtRow["Over 36 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty7", 0, i).Value = DBConvert.ParseDouble(dtRow["Over 36 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty7", 0, i).Value = "";
              }
              // Amount Over 36 Months
              if (dtRow["Amount Over 36 Mths"] != null && DBConvert.ParseDouble(dtRow["Amount Over 36 Mths"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty7a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount Over 36 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty7a", 0, i).Value = "";
              }

              if (dtRow["Total"] != null && DBConvert.ParseDouble(dtRow["Total"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty8", 0, i).Value = DBConvert.ParseDouble(dtRow["Total"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty8", 0, i).Value = "";
              }
              // Amount TotalAmount
              if (dtRow["TotalAmount"] != null && DBConvert.ParseDouble(dtRow["TotalAmount"].ToString()) != double.MinValue)
              {
                oXlsReport.Cell("**Qty8a", 0, i).Value = DBConvert.ParseDouble(dtRow["TotalAmount"].ToString());
              }
              else
              {
                oXlsReport.Cell("**Qty8a", 0, i).Value = "";
              }

              if (dtRow["0-3 Mths"] != null && dtRow["0-3 Mths"].ToString().Trim().Length > 0)
              {
                total1 = total1 + DBConvert.ParseDouble(dtRow["0-3 Mths"].ToString());
              }
              if (dtRow["4-6 Mths"] != null && dtRow["4-6 Mths"].ToString().Trim().Length > 0)
              {
                total2 = total2 + DBConvert.ParseDouble(dtRow["4-6 Mths"].ToString());
              }
              if (dtRow["7-9 Mths"] != null && dtRow["7-9 Mths"].ToString().Trim().Length > 0)
              {
                total3 = total3 + DBConvert.ParseDouble(dtRow["7-9 Mths"].ToString());
              }
              if (dtRow["10-12 Mths"] != null && dtRow["10-12 Mths"].ToString().Trim().Length > 0)
              {
                total4 = total4 + DBConvert.ParseDouble(dtRow["10-12 Mths"].ToString());
              }
              if (dtRow["13-24 Mths"] != null && dtRow["13-24 Mths"].ToString().Trim().Length > 0)
              {
                total5 = total5 + DBConvert.ParseDouble(dtRow["13-24 Mths"].ToString());
              }
              if (dtRow["25-36 Mths"] != null && dtRow["25-36 Mths"].ToString().Trim().Length > 0)
              {
                total6 = total6 + DBConvert.ParseDouble(dtRow["25-36 Mths"].ToString());
              }
              if (dtRow["Over 36 Mths"] != null && dtRow["Over 36 Mths"].ToString().Trim().Length > 0)
              {
                total7 = total7 + DBConvert.ParseDouble(dtRow["Over 36 Mths"].ToString());
              }
              if (dtRow["Total"] != null && dtRow["Total"].ToString().Trim().Length > 0)
              {
                total8 = total8 + DBConvert.ParseDouble(dtRow["Total"].ToString());
              }

            }
            oXlsReport.Cell("**TotalQty1").Value = total1;
            oXlsReport.Cell("**TotalQty2").Value = total2;
            oXlsReport.Cell("**TotalQty3").Value = total3;
            oXlsReport.Cell("**TotalQty4").Value = total4;
            oXlsReport.Cell("**TotalQty5").Value = total5;
            oXlsReport.Cell("**TotalQty6").Value = total6;
            oXlsReport.Cell("**TotalQty7").Value = total7;
            oXlsReport.Cell("**TotalQty8").Value = total8;
          }
        }
        // Get New Template
        else
        {
          if (checkDepartment == 1)
          {
            strTemplateName = "RPT_WOOD_03_001_16";
          }
          else
          {
            strTemplateName = "RPT_WOOD_03_001_13";
          }
          oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

          DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRPTWoodsAgeingMonthlyStore_Select", arrInput);

          oXlsReport.Cell("**Date").Value = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
          oXlsReport.Cell("**Month").Value = DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();

          if (dtData != null && dtData.Rows.Count > 0)
          {
            double total1 = 0;
            double total2 = 0;
            double total3 = 0;
            double total4 = 0;
            double total5 = 0;
            double total6 = 0;
            double total7 = 0;
            double total8 = 0;
            double total9 = 0;
            double total10 = 0;
            double total11 = 0;
            double total12 = 0;
            double total13 = 0;

            for (int i = 0; i < dtData.Rows.Count; i++)
            {
              DataRow dtRow = dtData.Rows[i];
              if (i > 0)
              {
                if (checkDepartment == 1)
                {
                  oXlsReport.Cell("A7:T7").Copy();
                  oXlsReport.RowInsert(6 + i);
                  oXlsReport.Cell("A7:T7", 0, i).Paste();
                }
                else
                {
                  oXlsReport.Cell("A7:AG7").Copy();
                  oXlsReport.RowInsert(6 + i);
                  oXlsReport.Cell("A7:AG7", 0, i).Paste();
                }
              }
              oXlsReport.Cell("**No", 0, i).Value = i + 1;
              oXlsReport.Cell("**Code", 0, i).Value = dtRow["MaterialCode"].ToString();
              oXlsReport.Cell("**NameEN", 0, i).Value = dtRow["NameEN"].ToString();
              oXlsReport.Cell("**Unit", 0, i).Value = dtRow["TenDonViEN"].ToString();
              if (dtRow["HeSoTon"] != null && DBConvert.ParseDouble(dtRow["HeSoTon"].ToString()) != double.MinValue
                      && DBConvert.ParseDouble(dtRow["HeSoTon"].ToString()) > 0)
              {
                oXlsReport.Cell("**LeadTime", 0, i).Value = DBConvert.ParseDouble(dtRow["HeSoTon"].ToString());
              }
              else
              {
                oXlsReport.Cell("**LeadTime", 0, i).Value = "";
              }

              if (dtRow["MinOfStock"] != null && DBConvert.ParseDouble(dtRow["MinOfStock"].ToString()) != double.MinValue
                      && DBConvert.ParseDouble(dtRow["MinOfStock"].ToString()) >= 0)
              {
                oXlsReport.Cell("**SafetyStock", 0, i).Value = DBConvert.ParseDouble(dtRow["MinOfStock"].ToString());
              }
              else
              {
                oXlsReport.Cell("**SafetyStock", 0, i).Value = "";
              }

              if (dtRow["0-1 Week"] != null && DBConvert.ParseDouble(dtRow["0-1 Week"].ToString()) != double.MinValue
                      && DBConvert.ParseDouble(dtRow["0-1 Week"].ToString()) > 0)
              {
                oXlsReport.Cell("**1", 0, i).Value = DBConvert.ParseDouble(dtRow["0-1 Week"].ToString());
              }
              else
              {
                oXlsReport.Cell("**1", 0, i).Value = "";
              }
              // Amount 0-1 Week
              if (dtRow["Amount 0-1 Week"] != null && DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString()) != double.MinValue
                     && DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString()) > 0)
              {
                oXlsReport.Cell("**1a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 0-1 Week"].ToString());
              }
              else
              {
                oXlsReport.Cell("**1a", 0, i).Value = "";
              }

              if (dtRow["1-2 Weeks"] != null && DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString()) != double.MinValue
                      && DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString()) > 0)
              {
                oXlsReport.Cell("**2", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString());
              }
              else
              {
                oXlsReport.Cell("**2", 0, i).Value = "";
              }
              // Amount 1-2 Weeks
              if (dtRow["Amount 1-2 Weeks"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Weeks"].ToString()) != double.MinValue
                     && DBConvert.ParseDouble(dtRow["Amount 1-2 Weeks"].ToString()) > 0)
              {
                oXlsReport.Cell("**2a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Weeks"].ToString());
              }
              else
              {
                oXlsReport.Cell("**2a", 0, i).Value = "";
              }

              if (dtRow["2-3 Weeks"] != null && DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString()) > 0)
              {
                oXlsReport.Cell("**3", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString());
              }
              else
              {
                oXlsReport.Cell("**3", 0, i).Value = "";
              }
              // Amount 2-3 Weeks
              if (dtRow["Amount 2-3 Weeks"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Weeks"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Amount 2-3 Weeks"].ToString()) > 0)
              {
                oXlsReport.Cell("**3a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Weeks"].ToString());
              }
              else
              {
                oXlsReport.Cell("**3a", 0, i).Value = "";
              }

              if (dtRow["3-4 Weeks"] != null && DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString()) > 0)
              {
                oXlsReport.Cell("**4", 0, i).Value = DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString());
              }
              else
              {
                oXlsReport.Cell("**4", 0, i).Value = "";
              }
              // Amount 3-4 Weeks
              if (dtRow["Amount 3-4 Weeks"] != null && DBConvert.ParseDouble(dtRow["Amount 3-4 Weeks"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Amount 3-4 Weeks"].ToString()) > 0)
              {
                oXlsReport.Cell("**4a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 3-4 Weeks"].ToString());
              }
              else
              {
                oXlsReport.Cell("**4a", 0, i).Value = "";
              }

              if (dtRow["1-2 Months"] != null && DBConvert.ParseDouble(dtRow["1-2 Months"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["1-2 Months"].ToString()) > 0)
              {
                oXlsReport.Cell("**5", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Months"].ToString());
              }
              else
              {
                oXlsReport.Cell("**5", 0, i).Value = "";
              }
              // Amount 1-2 Months
              if (dtRow["Amount 1-2 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString()) > 0)
              {
                oXlsReport.Cell("**5a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Months"].ToString());
              }
              else
              {
                oXlsReport.Cell("**5a", 0, i).Value = "";
              }

              if (dtRow["2-3 Months"] != null && DBConvert.ParseDouble(dtRow["2-3 Months"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["2-3 Months"].ToString()) > 0)
              {
                oXlsReport.Cell("**6", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Months"].ToString());
              }
              else
              {
                oXlsReport.Cell("**6", 0, i).Value = "";
              }
              // Amount 2-3 Months
              if (dtRow["Amount 2-3 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString()) != double.MinValue
                   && DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString()) > 0)
              {
                oXlsReport.Cell("**6a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Months"].ToString());
              }
              else
              {
                oXlsReport.Cell("**6a", 0, i).Value = "";
              }

              if (dtRow["3-6 Months"] != null && DBConvert.ParseDouble(dtRow["3-6 Months"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["3-6 Months"].ToString()) > 0)
              {
                oXlsReport.Cell("**7", 0, i).Value = DBConvert.ParseDouble(dtRow["3-6 Months"].ToString());
              }
              else
              {
                oXlsReport.Cell("**7", 0, i).Value = "";
              }
              // Amount 3-6 Months
              if (dtRow["Amount 3-6 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString()) != double.MinValue
                   && DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString()) > 0)
              {
                oXlsReport.Cell("**7a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 3-6 Months"].ToString());
              }
              else
              {
                oXlsReport.Cell("**7a", 0, i).Value = "";
              }

              if (dtRow["6-9 Mths"] != null && DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString()) > 0)
              {
                oXlsReport.Cell("**8", 0, i).Value = DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**8", 0, i).Value = "";
              }
              // Amount 6-9 Months
              if (dtRow["Amount 6-9 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString()) > 0)
              {
                oXlsReport.Cell("**8a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 6-9 Months"].ToString());
              }
              else
              {
                oXlsReport.Cell("**8a", 0, i).Value = "";
              }

              if (dtRow["9-12 Mths"] != null && DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString()) > 0)
              {
                oXlsReport.Cell("**9", 0, i).Value = DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString());
              }
              else
              {
                oXlsReport.Cell("**9", 0, i).Value = "";
              }
              // Amount 9-12 Months
              if (dtRow["Amount 9-12 Months"] != null && DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString()) > 0)
              {
                oXlsReport.Cell("**9a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 9-12 Months"].ToString());
              }
              else
              {
                oXlsReport.Cell("**9a", 0, i).Value = "";
              }

              if (dtRow["1-2 Years"] != null && DBConvert.ParseDouble(dtRow["1-2 Years"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["1-2 Years"].ToString()) > 0)
              {
                oXlsReport.Cell("**10", 0, i).Value = DBConvert.ParseDouble(dtRow["1-2 Years"].ToString());
              }
              else
              {
                oXlsReport.Cell("**10", 0, i).Value = "";
              }
              // Amount 1-2 Years
              if (dtRow["Amount 1-2 Years"] != null && DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString()) > 0)
              {
                oXlsReport.Cell("**10a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 1-2 Years"].ToString());
              }
              else
              {
                oXlsReport.Cell("**10a", 0, i).Value = "";
              }

              if (dtRow["2-3 Years"] != null && DBConvert.ParseDouble(dtRow["2-3 Years"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["2-3 Years"].ToString()) > 0)
              {
                oXlsReport.Cell("**11", 0, i).Value = DBConvert.ParseDouble(dtRow["2-3 Years"].ToString());
              }
              else
              {
                oXlsReport.Cell("**11", 0, i).Value = "";
              }
              // Amount 2-3 Years
              if (dtRow["Amount 2-3 Years"] != null && DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString()) > 0)
              {
                oXlsReport.Cell("**11a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount 2-3 Years"].ToString());
              }
              else
              {
                oXlsReport.Cell("**11a", 0, i).Value = "";
              }

              if (dtRow["Over 3 Years"] != null && DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString()) > 0)
              {
                oXlsReport.Cell("**12", 0, i).Value = DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString());
              }
              else
              {
                oXlsReport.Cell("**12", 0, i).Value = "";
              }
              // Amount Over 3 Years
              if (dtRow["Amount Over 3 Years"] != null && DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString()) > 0)
              {
                oXlsReport.Cell("**12a", 0, i).Value = DBConvert.ParseDouble(dtRow["Amount Over 3 Years"].ToString());
              }
              else
              {
                oXlsReport.Cell("**12a", 0, i).Value = "";
              }

              if (dtRow["Total"] != null && DBConvert.ParseDouble(dtRow["Total"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["Total"].ToString()) > 0)
              {
                oXlsReport.Cell("**13", 0, i).Value = DBConvert.ParseDouble(dtRow["Total"].ToString());
              }
              else
              {
                oXlsReport.Cell("**13", 0, i).Value = "";
              }
              // Amount TotalAmount 
              if (dtRow["TotalAmount"] != null && DBConvert.ParseDouble(dtRow["TotalAmount"].ToString()) != double.MinValue
                    && DBConvert.ParseDouble(dtRow["TotalAmount"].ToString()) > 0)
              {
                oXlsReport.Cell("**13a", 0, i).Value = DBConvert.ParseDouble(dtRow["TotalAmount"].ToString());
              }
              else
              {
                oXlsReport.Cell("**13a", 0, i).Value = "";
              }

              if (dtRow["0-1 Week"] != null && dtRow["0-1 Week"].ToString().Trim().Length > 0)
              {
                total1 = total1 + DBConvert.ParseDouble(dtRow["0-1 Week"].ToString());
              }
              if (dtRow["1-2 Weeks"] != null && dtRow["1-2 Weeks"].ToString().Trim().Length > 0)
              {
                total2 = total2 + DBConvert.ParseDouble(dtRow["1-2 Weeks"].ToString());
              }
              if (dtRow["2-3 Weeks"] != null && dtRow["2-3 Weeks"].ToString().Trim().Length > 0)
              {
                total3 = total3 + DBConvert.ParseDouble(dtRow["2-3 Weeks"].ToString());
              }
              if (dtRow["3-4 Weeks"] != null && dtRow["3-4 Weeks"].ToString().Trim().Length > 0)
              {
                total4 = total4 + DBConvert.ParseDouble(dtRow["3-4 Weeks"].ToString());
              }
              if (dtRow["1-2 Months"] != null && dtRow["1-2 Months"].ToString().Trim().Length > 0)
              {
                total5 = total5 + DBConvert.ParseDouble(dtRow["1-2 Months"].ToString());
              }
              if (dtRow["2-3 Months"] != null && dtRow["2-3 Months"].ToString().Trim().Length > 0)
              {
                total6 = total6 + DBConvert.ParseDouble(dtRow["2-3 Months"].ToString());
              }
              if (dtRow["3-6 Months"] != null && dtRow["3-6 Months"].ToString().Trim().Length > 0)
              {
                total7 = total7 + DBConvert.ParseDouble(dtRow["3-6 Months"].ToString());
              }
              if (dtRow["6-9 Mths"] != null && dtRow["6-9 Mths"].ToString().Trim().Length > 0)
              {
                total8 = total8 + DBConvert.ParseDouble(dtRow["6-9 Mths"].ToString());
              }
              if (dtRow["9-12 Mths"] != null && dtRow["9-12 Mths"].ToString().Trim().Length > 0)
              {
                total9 = total9 + DBConvert.ParseDouble(dtRow["9-12 Mths"].ToString());
              }
              if (dtRow["1-2 Years"] != null && dtRow["1-2 Years"].ToString().Trim().Length > 0)
              {
                total10 = total10 + DBConvert.ParseDouble(dtRow["1-2 Years"].ToString());
              }
              if (dtRow["2-3 Years"] != null && dtRow["2-3 Years"].ToString().Trim().Length > 0)
              {
                total11 = total11 + DBConvert.ParseDouble(dtRow["2-3 Years"].ToString());
              }
              if (dtRow["Over 3 Years"] != null && dtRow["Over 3 Years"].ToString().Trim().Length > 0)
              {
                total12 = total12 + DBConvert.ParseDouble(dtRow["Over 3 Years"].ToString());
              }
              if (dtRow["Total"] != null && dtRow["Total"].ToString().Trim().Length > 0)
              {
                total13 = total13 + Math.Round(DBConvert.ParseDouble(dtRow["Total"].ToString()), 4);
              }
            }
            oXlsReport.Cell("**Total1").Value = total1;
            oXlsReport.Cell("**Total2").Value = total2;
            oXlsReport.Cell("**Total3").Value = total3;
            oXlsReport.Cell("**Total4").Value = total4;
            oXlsReport.Cell("**Total5").Value = total5;
            oXlsReport.Cell("**Total6").Value = total6;
            oXlsReport.Cell("**Total7").Value = total7;
            oXlsReport.Cell("**Total8").Value = total8;
            oXlsReport.Cell("**Total9").Value = total9;
            oXlsReport.Cell("**Total10").Value = total10;
            oXlsReport.Cell("**Total11").Value = total11;
            oXlsReport.Cell("**Total12").Value = total12;
            oXlsReport.Cell("**Total13").Value = total13;
          }
        }
        oXlsReport.Out.File(strOutFileName);
        Process.Start(strOutFileName);
      }
    }
    #endregion Function
  }
}
