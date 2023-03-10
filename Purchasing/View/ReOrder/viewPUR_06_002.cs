/*
  Author      : 
  Date        : 04/06/2012
  Description : Make PR From MinMax Module
*/
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
using System.Threading;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_06_002 : MainUserControl
  {
    #region Field
    public DataTable dtData = new DataTable();
    private long prPid = long.MinValue;
    private bool isDuplicateProcess = false;
    #endregion Field

    #region Init Data
    /// <summary>
    /// 
    /// </summary>
    public viewPUR_06_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_03_001_Load(object sender, EventArgs e)
    {
      this.LoadDropDownMaterial();
      // Load Department
      this.LoadComboDepartment();

      // Load Type Of Request
      this.LoadComboTypeRequest();

      // Load UrgentLevel
      this.LoadComboUrgentLevel();

      // Load ProjectCode
      this.LoadComboProjectCode();

      // Load Drawing
      this.LoadDrawing();

      // Load Sample
      this.LoadSample();

      // Load Curency
      this.LoadComboCurrency();

      // Load Locatl Import
      this.LoadDropDownLocalImport();

      // Load PRNo
      this.LoadPRNo();

      // Load Grid
      this.LoadGrid();
    }

    /// <summary>
    /// Load PR No
    /// </summary>
    private void LoadPRNo()
    {
      DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPURPROnlineGetNewPRNo('PROL') NewPRNo");
      if (dtCode != null && dtCode.Rows.Count == 1)
      {
        this.txtPRNo.Text = dtCode.Rows[0][0].ToString();
      }
    }

    /// <summary>
    /// Load UltraCombo ProjectCode
    /// </summary>
    private void LoadComboProjectCode()
    {
      //      string commandText = string.Format(@" SELECT  Pid Code, ProjectCode Name 
      //                                            FROM    TblPURPRProjectCode 
      //                                            WHERE   ISNULL(Finished, 0) = 0 AND ISNULL([Status], 0) = 1 AND Department = 'PUR'");
      string commandText = string.Format(@"SELECT  PRO.Pid Code, PRO.ProjectCode Name,
                                            ISNULL(PRO.BudgetAmount, 0) - ISNULL(US.BudgetUsed, 0) AS PJOutStanding
                                          FROM TblPURPRProjectCode PRO
                                          LEFT JOIN 
                                            (
	                                            SELECT	Department, ProjectPid, SUM(BudgetRequested) BudgetUsed 
	                                            FROM	VPURTotalPricePRFollowProjectCode
	                                            GROUP	BY ProjectPid, Department
                                            )US ON US.ProjectPid = PRO.Pid
                                          WHERE   ISNULL(PRO.Finished, 0) = 0 
                                            AND ISNULL(PRO.[Status], 0) = 1 
                                            AND PRO.Department = 'PUR'
                                          ORDER BY PRO.ProjectCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText, 6000);

      udrpProjectCode.DataSource = dtSource;
      udrpProjectCode.DisplayMember = "Name";
      udrpProjectCode.ValueMember = "Code";
      udrpProjectCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpProjectCode.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      udrpProjectCode.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Urgent Level
    /// </summary>
    private void LoadComboUrgentLevel()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7007 AND [Code] IN (1, 3, 5) AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      udrpUrgent.DataSource = dtSource;
      udrpUrgent.DisplayMember = "Name";
      udrpUrgent.ValueMember = "Code";
      udrpUrgent.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpUrgent.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      udrpUrgent.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load Currency
    /// </summary>
    private void LoadComboCurrency()
    {
      string commandText = "SELECT Pid, CurrentExchangeRate,[Code] Name FROM TblPURCurrencyInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      udrpCurrency.DataSource = dtSource;
      udrpCurrency.DisplayMember = "Name";
      udrpCurrency.ValueMember = "Pid";
      udrpCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpCurrency.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      udrpCurrency.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      udrpCurrency.DisplayLayout.Bands[0].Columns["CurrentExchangeRate"].Hidden = true;
    }

    /// <summary>
    /// Load Drawing
    /// </summary>
    private void LoadDrawing()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'No' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Yes' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultddDrawing.DataSource = dtSource;
      ultddDrawing.DisplayMember = "Name";
      ultddDrawing.ValueMember = "ID";
      ultddDrawing.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultddDrawing.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultddDrawing.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load Sample
    /// </summary>
    private void LoadSample()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'No' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Yes' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultddSample.DataSource = dtSource;
      ultddSample.DisplayMember = "Name";
      ultddSample.ValueMember = "ID";
      ultddSample.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultddSample.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultddSample.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load Data From Data MAKE PR
    /// </summary>
    private void LoadGrid()
    {
      ultCreateDate.Value = DBConvert.ParseDateTime(DateTime.Today.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME); ;
      this.lblStatus.Text = "New";

      DataTable dt = new DataTable();
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("NameEN", typeof(System.String));
      dt.Columns.Add("NameVN", typeof(System.String));
      dt.Columns.Add("Unit", typeof(System.String));
      dt.Columns.Add("Quantity", typeof(System.Double));
      dt.Columns.Add("LastPrice", typeof(System.Double));
      dt.Columns.Add("Price", typeof(System.Double));
      dt.Columns.Add("Currency", typeof(System.Int32));
      dt.Columns.Add("CurrentExchangeRate", typeof(System.Double));
      dt.Columns.Add("LeadTime", typeof(System.Int32));
      dt.Columns.Add("VAT", typeof(System.Double));
      dt.Columns.Add("Urgent", typeof(System.Int32));
      dt.Columns.Add("RequiredDeliveryDate", typeof(System.DateTime));
      dt.Columns.Add("ExpectedBrand", typeof(System.String));
      dt.Columns.Add("Drawing", typeof(System.Int32));
      dt.Columns.Add("Sample", typeof(System.Int32));
      dt.Columns.Add("Imported", typeof(System.Int32));
      dt.Columns.Add("ProjectCode", typeof(System.Int32));
      dt.Columns.Add("PJOutStanding", typeof(System.Double));
      dt.Columns.Add("PurposeOfRequisition", typeof(System.String));
      dt.Columns.Add("GroupInCharge", typeof(System.Int32));

      foreach (DataRow row in dtData.Rows)
      {
        DataRow rowGrid = dt.NewRow();
        rowGrid["MaterialCode"] = row["MaterialCode"].ToString();
        rowGrid["NameEN"] = row["NameEN"].ToString();
        rowGrid["Unit"] = row["Unit"].ToString();
        if (DBConvert.ParseDouble(row["Quantity"].ToString()) > 0)
        {
          rowGrid["Quantity"] = DBConvert.ParseDouble(row["Quantity"].ToString());
        }
        // Get Lasted Price
        // PODetail = 4 is Cancel
        double lastedPrice = double.MinValue;
        string commandText = string.Empty;
        commandText += " SELECT PRI.MaterialCode, PRI.Price";
        commandText += " FROM ";
        commandText += " (";
        commandText += " 	  SELECT ROW_NUMBER() OVER(PARTITION BY PO.MaterialCode ORDER BY PO.CreateDate DESC) Priority,";
        commandText += " 		  PO.MaterialCode, PO.Price";
        commandText += " 	  FROM ";
        commandText += " 	  (";
        commandText += " 		  SELECT PO.CreateDate, PRDT.MaterialCode, PODT.Price * PODT.ExchangeRate AS Price";
        commandText += " 		  FROM TblPURPOInformation PO";
        commandText += " 			  INNER JOIN TblPURPODetail PODT ON PO.PONo = PODT.PONo ";
        commandText += " 			  INNER JOIN TblPURPRDetail PRDT ON PRDT.Pid = PODT.PRDetailPid";
        commandText += "      WHERE PODT.[Status] <> 4 AND PRDT.MaterialCode = '" + row["MaterialCode"].ToString() + "'";
        commandText += " 	  )PO";
        commandText += " )PRI";
        commandText += " WHERE PRI.Priority = 1";

        DataTable dtLastedPrice = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtLastedPrice != null && dtLastedPrice.Rows.Count > 0)
        {
          lastedPrice = DBConvert.ParseDouble(dtLastedPrice.Rows[0]["Price"].ToString());
          if (lastedPrice > 0)
          {
            rowGrid["LastPrice"] = lastedPrice;
            rowGrid["Price"] = lastedPrice;
          }
        }

        //LeadTime
        int leadTime = int.MinValue;
        commandText = @"SELECT MaterialCode, LeadTime
                        FROM VBOMMaterials
                        WHERE Warehouse = 1 AND MaterialCode = '" + row["MaterialCode"].ToString() + "'";
        DataTable dtLeadTime = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtLeadTime != null)
        {
          //rowGrid["LeadTime"] = dtLeadTime.Rows[0]["LeadTime"].ToString();
          leadTime = DBConvert.ParseInt(dtLeadTime.Rows[0]["LeadTime"].ToString());
          if (leadTime > 0)
          {
            rowGrid["LeadTime"] = leadTime;
          }
        }

        // GroupInCharge
        int groupInCharge = int.MinValue;
        commandText = "SELECT NameVN, GroupIncharge FROM TblGNRMaterialInformation WHERE MaterialCode = '" + row["MaterialCode"].ToString() + "'";
        DataTable dtGroupInCharge = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtGroupInCharge != null)
        {
          rowGrid["NameVN"] = dtGroupInCharge.Rows[0]["NameVN"].ToString();
          groupInCharge = DBConvert.ParseInt(dtGroupInCharge.Rows[0]["GroupIncharge"].ToString());
          if (groupInCharge > 0)
          {
            rowGrid["GroupInCharge"] = groupInCharge;
          }
        }
        // Add Row
        dt.Rows.Add(rowGrid);
      }
      this.ultData.DataSource = dt;
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
      ultDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
      this.ultDepartment.Value = "PUR";
    }

    /// <summary>
    /// Load UltraCombo Type Of Request
    /// </summary>
    private void LoadComboTypeRequest()
    {
      string commandText = "SELECT Code, Value Name FROM TblBOMCodeMaster WHERE [Group] = 7005 AND DeleteFlag = 0";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultTypeRequest.DataSource = dtSource;
      ultTypeRequest.DisplayMember = "Name";
      ultTypeRequest.ValueMember = "Code";
      ultTypeRequest.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultTypeRequest.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultTypeRequest.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultTypeRequest.Value = 1;
    }

    /// <summary>
    /// Load UltraCombo Requester
    /// </summary>
    /// <param name="group"></param>
    private void LoadComboRequester(string department)
    {
      string commandText = string.Format("SELECT ID_NhanVien, CAST(ID_NhanVien AS VARCHAR) + ' - ' + HoNV + ' ' + TenNV Name FROM VHRNhanVien WHERE Resigned = 0 AND Department = '{0}'", department);
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);
      ultRequester.DataSource = dtSource;
      ultRequester.DisplayMember = "Name";
      ultRequester.ValueMember = "ID_NhanVien";
      ultRequester.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultRequester.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultRequester.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }

    // Load Local Imported
    private void LoadDropDownLocalImport()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7006 AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultdrpImported.DataSource = dtSource;
      ultdrpImported.DisplayMember = "Name";
      ultdrpImported.ValueMember = "Code";
      ultdrpImported.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultdrpImported.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultdrpImported.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }


    // Load Material
    private void LoadDropDownMaterial()
    {
      string commandText = @"SELECT MaterialCode, MaterialNameEn, MaterialNameVn, Unit, LeadTime
                              FROM VBOMMaterials
                              WHERE Warehouse = 1";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDMaterial.DataSource = dtSource;
      ultDDMaterial.DisplayMember = "MaterialCode";
      ultDDMaterial.ValueMember = "MaterialCode";
      ultDDMaterial.DisplayLayout.Bands[0].ColHeadersVisible = false;
      //ultdrpImported.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      //ultdrpImported.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    #endregion Init Data

    #region Event
    /// <summary>
    /// Close Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultDepartment_ValueChanged(object sender, EventArgs e)
    {
      if (ultDepartment.SelectedRow != null)
      {
        string department = ultDepartment.SelectedRow.Cells["Department"].Value.ToString().Trim();
        this.LoadComboRequester(department);

        this.ultRequester.Value = SharedObject.UserInfo.UserPid;
      }
    }

    private void ultDepartment_Leave(object sender, EventArgs e)
    {
      string value = string.Empty;
      try
      {
        value = ultDepartment.Value.ToString().Trim();
      }
      catch
      {
      }
      if (value.Length == 0)
      {
        string text = ultDepartment.Text.Trim();

        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM VHRDDepartment WHERE Department = '" + text + "'";
        object objDep = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if ((objDep == null) || (objDep != null && (int)objDep == 0))
        {
          WindowUtinity.ShowMessageError("ERR0011", "Department");
          ultDepartment.Focus();
          return;
        }
      }
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["GroupInCharge"].Hidden = true;
      e.Layout.Bands[0].Columns["PJOutStanding"].Hidden = true;
      e.Layout.Bands[0].Columns["CurrentExchangeRate"].Hidden = true;

      e.Layout.Bands[0].Columns["Price"].Header.Caption = "(*)Price";
      e.Layout.Bands[0].Columns["Currency"].Header.Caption = "(*)Currency";
      e.Layout.Bands[0].Columns["Urgent"].Header.Caption = "(*)Urgent";
      e.Layout.Bands[0].Columns["RequiredDeliveryDate"].Header.Caption = "(*)RequiredDeliveryDate";
      e.Layout.Bands[0].Columns["Imported"].Header.Caption = "(*)Imported";
      e.Layout.Bands[0].Columns["Quantity"].Header.Caption = "(*)Quantity";
      e.Layout.Bands[0].Columns["PurposeOfRequisition"].Header.Caption = "(*)PurposeOfRequisiton";
      e.Layout.Bands[0].Columns["ProjectCode"].Header.Caption = "(*)ProjectCode";
      //e.Layout.Bands[0].Columns["LeadTime"].Header.Caption = "(*)LeadTime";

      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ultDDMaterial;
      e.Layout.Bands[0].Columns["Urgent"].ValueList = udrpUrgent;
      e.Layout.Bands[0].Columns["ProjectCode"].ValueList = udrpProjectCode;
      e.Layout.Bands[0].Columns["Currency"].ValueList = udrpCurrency;
      e.Layout.Bands[0].Columns["Drawing"].ValueList = ultddDrawing;
      e.Layout.Bands[0].Columns["Sample"].ValueList = ultddSample;
      e.Layout.Bands[0].Columns["Imported"].ValueList = ultdrpImported;

      //e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LastPrice"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LeadTime"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Price"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Quantity"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Urgent"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ProjectCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Currency"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Sample"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Imported"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["PurposeOfRequisition"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["VAT"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Drawing"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ExpectedBrand"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["RequiredDeliveryDate"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Columns["Quantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LastPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["VAT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 60;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 100;

      e.Layout.Bands[0].Columns["RequiredDeliveryDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["LastPrice"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["Price"].Format = "###,###.##";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success;
      string message = string.Empty;
      // Check Info
      success = this.CheckValidPRInformationInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      // Check Detail
      success = this.CheckValidPRDetailInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      //Save Master
      success = this.SaveAddNew();
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return;
      }
      // Save Detail
      success = this.SaveDetail();
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return;
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      // Close Tab
      this.CloseTab();

      // Get PRNo
      string prNo = string.Empty;
      string commandText = "SELECT PROnlineNo FROM TblPURPROnlineInformation WHERE PID = " + this.prPid + "";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        prNo = dt.Rows[0]["PROnlineNo"].ToString();
      }
      viewPUR_03_001 view = new viewPUR_03_001();
      view.prNo = prNo;
      Shared.Utility.WindowUtinity.ShowView(view, "UPDATE PR", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }
    #endregion Event

    #region Process
    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      bool result = this.SavePrDetailInfo();
      if (!result)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save PR Detail Information
    /// </summary>
    /// <returns></returns>
    private bool SavePrDetailInfo()
    {
      long result = long.MinValue;
      string storeName = string.Empty;
      string commandText = string.Empty;

      storeName = "spPURMappingPRDetailFromReOrder_Insert";
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];
        // Input
        DBParameter[] inputParamInsert = new DBParameter[17];
        inputParamInsert[0] = new DBParameter("@PROnlinePid", DbType.Int64, this.prPid);
        inputParamInsert[1] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MaterialCode"].Value.ToString());
        inputParamInsert[2] = new DBParameter("@Status", DbType.Int32, 2);
        inputParamInsert[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Quantity"].Value.ToString()));
        inputParamInsert[4] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()));
        inputParamInsert[5] = new DBParameter("@Currency", DbType.Int32, DBConvert.ParseInt(row.Cells["Currency"].Value.ToString()));
        inputParamInsert[6] = new DBParameter("@RequestDate", DbType.DateTime, (DateTime)row.Cells["RequiredDeliveryDate"].Value);
        inputParamInsert[7] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseInt(row.Cells["Urgent"].Value.ToString()));
        if (DBConvert.ParseInt(row.Cells["ProjectCode"].Value.ToString()) != int.MinValue)
        {
          inputParamInsert[8] = new DBParameter("@ProjectCode", DbType.Int32, DBConvert.ParseInt(row.Cells["ProjectCode"].Value.ToString()));
        }
        if (row.Cells["PurposeOfRequisition"].Value.ToString().Trim().Length > 0)
        {
          inputParamInsert[9] = new DBParameter("@PurposeOfRequisition", DbType.String, row.Cells["PurposeOfRequisition"].Value.ToString());
        }
        if (row.Cells["ExpectedBrand"].Value.ToString().Trim().Length > 0)
        {
          inputParamInsert[10] = new DBParameter("@ExpectedBrand", DbType.String, row.Cells["ExpectedBrand"].Value.ToString());
        }
        if (DBConvert.ParseInt(row.Cells["Drawing"].Value.ToString()) != int.MinValue)
        {
          inputParamInsert[11] = new DBParameter("@Drawing", DbType.Int32, DBConvert.ParseInt(row.Cells["Drawing"].Value.ToString()));
        }
        if (DBConvert.ParseInt(row.Cells["Sample"].Value.ToString()) != int.MinValue)
        {
          inputParamInsert[12] = new DBParameter("@Sample", DbType.Int32, DBConvert.ParseInt(row.Cells["Sample"].Value.ToString()));
        }
        if (DBConvert.ParseInt(row.Cells["Imported"].Value.ToString()) != int.MinValue)
        {
          inputParamInsert[13] = new DBParameter("@Imported", DbType.Int32, DBConvert.ParseInt(row.Cells["Imported"].Value.ToString()));
        }
        if (DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString()) != double.MinValue)
        {
          inputParamInsert[14] = new DBParameter("@VAT", DbType.Double, DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString()));
        }
        if (DBConvert.ParseInt(row.Cells["GroupInCharge"].Value.ToString()) != int.MinValue)
        {
          inputParamInsert[15] = new DBParameter("@GroupInCharge", DbType.Int32, DBConvert.ParseInt(row.Cells["GroupInCharge"].Value.ToString()));
        }
        inputParamInsert[16] = new DBParameter("@CurrentExchangeRate", DbType.Double, DBConvert.ParseDouble(row.Cells["CurrentExchangeRate"].Value.ToString()));
        // OutPut
        DBParameter[] outputParamInsert = new DBParameter[1];
        outputParamInsert[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        // Excecute
        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);
        result = DBConvert.ParseLong(outputParamInsert[0].Value.ToString());
        if (result == long.MinValue)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Add New PR Information
    /// </summary>
    /// <returns></returns>
    private bool SaveAddNew()
    {
      bool result = true;
      this.prPid = this.SaveInsertNewPrInformation();
      if (this.prPid <= 0)
      {
        return false;
      }
      return result;
    }

    /// <summary>
    /// Save Insert New Pr Information
    /// </summary>
    /// <returns></returns>
    private long SaveInsertNewPrInformation()
    {
      long result = long.MinValue;
      string commandText = string.Empty;
      string storeName = string.Empty;

      storeName = "spPURMappingPRInfoFromReOrder_Insert";
      // Input
      DBParameter[] inputParam = new DBParameter[9];
      inputParam[0] = new DBParameter("@RequestBy", DbType.Int32, DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultRequester)));
      inputParam[1] = new DBParameter("@Department", DbType.String, ControlUtility.GetSelectedValueUltraCombobox(ultDepartment));
      inputParam[2] = new DBParameter("@Status", DbType.Int32, 3);
      commandText += " SELECT Manager";
      commandText += " FROM VHRDDepartmentInfo";
      commandText += " WHERE Code ='" + ControlUtility.GetSelectedValueUltraCombobox(ultDepartment) + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        inputParam[3] = new DBParameter("@HeadDepartmentApproved", DbType.Int32, DBConvert.ParseInt(dt.Rows[0][0].ToString()));
      }
      inputParam[4] = new DBParameter("@TypeOfRequest", DbType.Int32, ControlUtility.GetSelectedValueUltraCombobox(ultTypeRequest));
      if (txtPurposePR.Text.Trim().Length > 0)
      {
        inputParam[5] = new DBParameter("@PurposeOfRequisition", DbType.String, txtPurposePR.Text);
      }
      inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[7] = new DBParameter("@PROnlineNo", DbType.String, "PROL");
      if (txtRemark.Text.Trim().Length > 0)
      {
        inputParam[8] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
      }
      // Output
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      // Excecute
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      return result;
    }

    private void CheckProcessDuplicate()
    {
      this.isDuplicateProcess = false;
      for (int k = 0; k < ultData.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultData.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }


      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowcurentA = ultData.Rows[i];
        string materialCodeA = rowcurentA.Cells["MaterialCode"].Value.ToString();
        string quantityA = rowcurentA.Cells["Quantity"].Value.ToString();
        string lastPriceA = rowcurentA.Cells["LastPrice"].Value.ToString();
        string priceA = rowcurentA.Cells["Price"].Value.ToString();
        string currencyA = rowcurentA.Cells["Currency"].Value.ToString();
        string vatA = rowcurentA.Cells["VAT"].Value.ToString();
        string urgentA = rowcurentA.Cells["Urgent"].Value.ToString();
        string requiredDeliveryDateA = rowcurentA.Cells["RequiredDeliveryDate"].Value.ToString();
        string expectedBrandA = rowcurentA.Cells["ExpectedBrand"].Value.ToString();
        string drawingA = rowcurentA.Cells["Drawing"].Value.ToString();
        string sampleA = rowcurentA.Cells["Sample"].Value.ToString();
        string importedA = rowcurentA.Cells["Imported"].Value.ToString();
        string projectCodeA = rowcurentA.Cells["ProjectCode"].Value.ToString();
        string pJOutStandingA = rowcurentA.Cells["PJOutStanding"].Value.ToString();
        string purposeOfRequisitionA = rowcurentA.Cells["PurposeOfRequisition"].Value.ToString();
        string groupInChargeA = rowcurentA.Cells["GroupInCharge"].Value.ToString();

        for (int x = i + 1; x < ultData.Rows.Count; x++)
        {
          UltraGridRow rowcurentB = ultData.Rows[x];
          string materialCodeB = rowcurentB.Cells["MaterialCode"].Value.ToString();
          string quantityB = rowcurentB.Cells["Quantity"].Value.ToString();
          string lastPriceB = rowcurentB.Cells["LastPrice"].Value.ToString();
          string priceB = rowcurentB.Cells["Price"].Value.ToString();
          string currencyB = rowcurentB.Cells["Currency"].Value.ToString();
          string vatB = rowcurentB.Cells["VAT"].Value.ToString();
          string urgentB = rowcurentB.Cells["Urgent"].Value.ToString();
          string requiredDeliveryDateB = rowcurentB.Cells["RequiredDeliveryDate"].Value.ToString();
          string expectedBrandB = rowcurentB.Cells["ExpectedBrand"].Value.ToString();
          string drawingB = rowcurentB.Cells["Drawing"].Value.ToString();
          string sampleB = rowcurentB.Cells["Sample"].Value.ToString();
          string importedB = rowcurentB.Cells["Imported"].Value.ToString();
          string projectCodeB = rowcurentB.Cells["ProjectCode"].Value.ToString();
          string pJOutStandingB = rowcurentB.Cells["PJOutStanding"].Value.ToString();
          string purposeOfRequisitionB = rowcurentB.Cells["PurposeOfRequisition"].Value.ToString();
          string groupInChargeB = rowcurentB.Cells["GroupInCharge"].Value.ToString();

          if (string.Compare(materialCodeA, materialCodeB) == 0 &&
              string.Compare(quantityA, quantityB) == 0 &&
              string.Compare(lastPriceA, lastPriceB) == 0 &&
              string.Compare(priceA, priceB) == 0 &&
              string.Compare(currencyA, currencyB) == 0 &&
              string.Compare(vatA, vatB) == 0 &&
              string.Compare(urgentA, urgentB) == 0 &&
              string.Compare(requiredDeliveryDateA, requiredDeliveryDateB) == 0 &&
              string.Compare(expectedBrandA, expectedBrandB) == 0 &&
              string.Compare(drawingA, drawingB) == 0 &&
              string.Compare(sampleA, sampleB) == 0 &&
              string.Compare(importedA, importedB) == 0 &&
              string.Compare(projectCodeA, projectCodeB) == 0 &&
              string.Compare(pJOutStandingA, pJOutStandingB) == 0 &&
              string.Compare(purposeOfRequisitionA, purposeOfRequisitionB) == 0 &&
              string.Compare(groupInChargeA, groupInChargeB) == 0)
          {
            rowcurentA.CellAppearance.BackColor = Color.Yellow;
            rowcurentB.CellAppearance.BackColor = Color.Yellow;
            this.isDuplicateProcess = true;
          }
          //else
          //{
          //  rowcurentA.CellAppearance.BackColor = Color.White;
          //  rowcurentB.CellAppearance.BackColor = Color.White;
          //  this.isDuplicateProcess = false;
          //}
        }
      }

    }



    /// <summary>
    /// Check Valid PR Detail Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPRDetailInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;

      long projectPid = long.MinValue;
      double totalAmount = 0;
      double PJOutStanding = 0;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        double totalBudget = 0;
        double total = 0;
        UltraGridRow row = ultData.Rows[i];
        projectPid = DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString());
        // Qty
        if (DBConvert.ParseDouble(row.Cells["Quantity"].Value.ToString()) <= 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Quantity");
          return false;
        }

        //MaterialCode
        string material = row.Cells["MaterialCode"].Value.ToString();
        string cm = string.Format(@"SELECT MaterialCode
                              FROM VBOMMaterials
                              WHERE Warehouse = 1 AND MaterialCode='{0}'", material);
        DataTable dtMaterial = DataBaseAccess.SearchCommandTextDataTable(cm);
        if (dtMaterial.Rows.Count == 0 || row.Cells["MaterialCode"].Value.ToString().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "MaterialCode");
          return false;
        }

        // Price
        if (DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()) <= 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Price");
          return false;
        }
        // Currency
        if (DBConvert.ParseInt(row.Cells["Currency"].Value.ToString()) <= 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Currency");
          return false;
        }
        // Urgent
        if (DBConvert.ParseInt(row.Cells["Urgent"].Value.ToString()) <= 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Urgent");
          return false;
        }
        // Required Delivery Date
        if (row.Cells["RequiredDeliveryDate"].Value.ToString().Length > 0)
        {
          DateTime requestDate = (DateTime)row.Cells["RequiredDeliveryDate"].Value;
          if (requestDate == DateTime.MinValue)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Required Delivery Date");
            return false;
          }
        }
        else
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Required Delivery Date");
          return false;
        }
        // Imported
        if (DBConvert.ParseInt(row.Cells["Imported"].Value.ToString()) <= 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Imported");
          return false;
        }
        // ProjectCode
        if (row.Cells["ProjectCode"].Value.ToString().Length > 0)
        {
          if (DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString()) <= 0)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "ProjectCode");
            return false;
          }
        }

        if (row.Cells["ProjectCode"].Value.ToString().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "ProjectCode");
          return false;
        }

        ////Ha Anh add check project code
        //if (i == 0)
        //{
        //  projectPid = DBConvert.ParseLong(ultData.Rows[i].Cells["ProjectCode"].Value.ToString());
        //}
        //if (i > 0 && projectPid != DBConvert.ParseLong(ultData.Rows[i].Cells["ProjectCode"].Value.ToString()))
        //{
        //  message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Project Code");
        //  return false;
        //}

        commandText = string.Format(@"SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Pid = {0}", DBConvert.ParseLong(row.Cells["Currency"].Value.ToString()));
        double dtExchangeRate = DBConvert.ParseDouble(DataBaseAccess.ExecuteScalarCommandText(commandText));

        commandText = string.Format(@"SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Pid = 2"); //USD
        double usExchangeRate = DBConvert.ParseDouble(DataBaseAccess.ExecuteScalarCommandText(commandText));


        totalAmount += (DBConvert.ParseDouble(ultData.Rows[i].Cells["Price"].Value.ToString()))
                        * DBConvert.ParseDouble(ultData.Rows[i].Cells["Quantity"].Value.ToString()) * dtExchangeRate / usExchangeRate;


        PJOutStanding = DBConvert.ParseDouble(ultData.Rows[i].Cells["PJOutStanding"].Value.ToString());
        //end

        // Purpose Of Requisition
        if (row.Cells["PurposeOfRequisition"].Value.ToString().Trim().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Purpose Of Requisition");
          return false;
        }

        //Check Duplicate
        if (this.isDuplicateProcess)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Data is Duplicate");
          return false;
        }

        //Sum Qty follow ProjectPid
        DataTable dt = (DataTable)ultData.DataSource;
        DataRow[] dr1 = dt.Select(string.Format("ProjectCode ={0}", projectPid));
        if (dr1.Length > 0)
        {
          foreach (DataRow r in dr1)
          {
            double l = DBConvert.ParseDouble(r["Quantity"].ToString());
            double z = DBConvert.ParseDouble(r["Price"].ToString());
            total += (l * z);
          }
          totalBudget = total * dtExchangeRate;
        }

        //Check budget > total request
        DBParameter[] inputBudget = new DBParameter[1];
        inputBudget[0] = new DBParameter("@ProjectPid", DbType.Int64, projectPid);
        DBParameter[] output = new DBParameter[3];
        output[0] = new DBParameter("@BudgetRequested", DbType.Double, double.MinValue);
        output[1] = new DBParameter("@BudgetAmount", DbType.Double, double.MinValue);
        output[2] = new DBParameter("@ProjectCode", DbType.String, 32, string.Empty);
        DataBaseAccess.ExecuteStoreProcedure("spPURPRCheckingBudgetOverForReOrderPurchasing", inputBudget, output);
        double budgetRequested = DBConvert.ParseDouble(output[0].Value.ToString());
        double budgetAmount = DBConvert.ParseDouble(output[1].Value.ToString());
        string projectCode = output[2].Value.ToString();
        if (totalBudget + budgetRequested > budgetAmount)
        {
          if (WindowUtinity.ShowMessageConfirmFromText("Total Budget Requested > Budget Amount, Do You want to show detail ?") == DialogResult.Yes)
          {
            bool a = true;
            a = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = '" + projectCode + "' WHERE [Group] = 16077 AND Value = 1", null);
            Process.Start(DataBaseAccess.ExecuteScalarCommandText("SELECT [Description] FROM TblBOMCodeMaster WHERE [Group] = 16077 AND Value = 2", null).ToString());
            Thread.Sleep(2000);
            a = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = NULL WHERE [Group] = 16077 AND Value = 1", null);
            message = string.Format("Project Code '{0}' is over Budget outstanding", projectCode);
            return false;
          }
          else
          {
            message = string.Format("Project Code '{0}' is over Budget outstanding", projectCode);
            return false;
          }
        }
      }

      //DataTable dt = (DataTable)ultData.DataSource;
      //if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["ProjectCode"].ToString().Length > 0)
      //{
      //  if (totalAmount > PJOutStanding)
      //  {
      //    message = string.Format("Project Code is over Budget outstanding");
      //    return false;
      //  }
      //}

      return true;
    }

    /// <summary>
    /// Check Valid PR Information Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPRInformationInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();

      // Department
      string department = ControlUtility.GetSelectedValueUltraCombobox(ultDepartment);
      if (department.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Department");
        return false;
      }
      // Requester
      int requester = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultRequester));
      if (requester == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Requester");
        return false;
      }
      commandText = "SELECT COUNT(*) FROM VHRNhanVien WHERE Department = '" + department + "' AND ID_NhanVien = " + requester;
      dtCheck = DataBaseAccess.SearchCommandText(commandText).Tables[0];
      if (dtCheck != null && DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Requester");
        return false;
      }
      // Type Of Request
      int typeOfRequest = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultTypeRequest));
      if (typeOfRequest == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Type Of Request");
        return false;
      }
      return true;
    }
    #endregion Process

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "ProjectCode":
          if (DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString()) != long.MinValue)
          {
            row.Cells["PJOutStanding"].Value = DBConvert.ParseDouble(udrpProjectCode.SelectedRow.Cells["PJOutStanding"].Value.ToString());
          }
          else
          {
            row.Cells["PJOutStanding"].Value = DBNull.Value;
          }
          break;
        case "Currency":
          if (udrpCurrency.SelectedRow != null)
          {
            row.Cells["CurrentExchangeRate"].Value = DBConvert.ParseDouble(udrpCurrency.SelectedRow.Cells["CurrentExchangeRate"].Value.ToString());
          }
          else
          {
            row.Cells["CurrentExchangeRate"].Value = DBNull.Value;
          }
          break;
        case "MaterialCode":
          if (ultDDMaterial.SelectedRow != null)
          {
            row.Cells["NameEN"].Value = ultDDMaterial.SelectedRow.Cells["MaterialNameEn"].Value.ToString();
            row.Cells["NameVN"].Value = ultDDMaterial.SelectedRow.Cells["MaterialNameVn"].Value.ToString();
            row.Cells["Unit"].Value = ultDDMaterial.SelectedRow.Cells["Unit"].Value.ToString();
            row.Cells["LeadTime"].Value = DBConvert.ParseInt(ultDDMaterial.SelectedRow.Cells["LeadTime"].Value.ToString());

          }
          else
          {
            row.Cells["NameEN"].Value = DBNull.Value;
            row.Cells["NameVN"].Value = DBNull.Value;
            row.Cells["Unit"].Value = DBNull.Value;
            row.Cells["LeadTime"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
      this.CheckProcessDuplicate();
    }
  }
}
