/*
 * Author       :  
 * CreateDate   : 1/6/2013
 * Description  : Supplier Price Info
 */
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_20_002 : DaiCo.Shared.MainUserControl
  {
    #region Init
    public long pidSupplierPrice = 0;
    public viewPUR_20_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_07_101_Load(object sender, EventArgs e)
    {
      // Unit
      this.LoadUnit();

      // Currency
      this.LoadCurrency();

      // Load Valid Invoice
      this.LoadValidInvoice();

      // Load Changeable After Delivery
      this.LoadChangeableAfterDelivery();

      // Load Source
      this.LoadSource();

      // Load Quality
      this.LoadQuality();

      // Load MaterialCode
      this.LoadMaterialCode();

      // Load Supplier
      this.LoadSupplier();

      // Load Data
      this.LoadData();
    }
    #endregion Init

    #region Process
    /// <summary>
    /// Load Material Code
    /// </summary>
    private void LoadMaterialCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT MaterialCode, MaterialCode + ' ' + NameEN Name";
      commandText += " FROM TblGNRMaterialInformation ";
      commandText += " ORDER BY MaterialCode ";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultMaterialCode.DataSource = dt;
      ultMaterialCode.DisplayMember = "Name";
      ultMaterialCode.ValueMember = "MaterialCode";
      ultMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = true;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
    }

    /// <summary>
    /// Load Supplier
    /// </summary>
    private void LoadSupplier()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, EnglishName";
      commandText += " FROM TblPURSupplierInfo ";
      commandText += " WHERE LEN(EnglishName) > 0 ";
      commandText += " ORDER BY EnglishName ";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultSupplier.DataSource = dt;
      ultSupplier.DisplayMember = "EnglishName";
      ultSupplier.ValueMember = "Pid";
      ultSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultSupplier.DisplayLayout.Bands[0].Columns["EnglishName"].Width = 350;
    }

    /// <summary>
    /// Load Quality
    /// </summary>
    private void LoadQuality()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 Pid, 'Over Meet' Code";
      commandText += " UNION ALL ";
      commandText += " SELECT 2 Pid, 'Meet' ";
      commandText += " UNION ALL ";
      commandText += " SELECT 3 Pid, 'Min Acceptable' ";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultQuality.DataSource = dt;
      ultQuality.DisplayMember = "Code";
      ultQuality.ValueMember = "Pid";
      ultQuality.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultQuality.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultQuality.DisplayLayout.Bands[0].Columns["Code"].Width = 150;
    }

    /// <summary>
    /// Load Unit
    /// </summary>
    private void LoadUnit()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, Symbol, [Description] ";
      commandText += " FROM TblGNRMaterialUnit ";
      commandText += " ORDER BY Symbol ";

      DataTable dtUnit = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultUnit.DataSource = dtUnit;
      ultUnit.DisplayMember = "Symbol";
      ultUnit.ValueMember = "Pid";
      ultUnit.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultUnit.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultUnit.DisplayLayout.Bands[0].Columns["Symbol"].Width = 150;
      ultUnit.DisplayLayout.Bands[0].Columns["Description"].Width = 250;
    }

    /// <summary>
    /// Load Currency
    /// </summary>
    private void LoadCurrency()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, Code ";
      commandText += " FROM TblPURCurrencyInfo ";

      DataTable dtCurrency = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCurrency.DataSource = dtCurrency;
      ultCurrency.DisplayMember = "Code";
      ultCurrency.ValueMember = "Pid";
      ultCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCurrency.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCurrency.DisplayLayout.Bands[0].Columns["Code"].Width = 150;
    }

    /// <summary>
    /// Load Valid Invoice
    /// </summary>
    private void LoadValidInvoice()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 Pid, 'Direct Invoice' Code";
      commandText += " UNION ALL ";
      commandText += " SELECT 2 Pid, 'VAT Invoice' ";
      commandText += " UNION ALL ";
      commandText += " SELECT 3 Pid, 'No Invoice' ";

      DataTable dtValid = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultValidInvoice.DataSource = dtValid;
      ultValidInvoice.DisplayMember = "Code";
      ultValidInvoice.ValueMember = "Pid";
      ultValidInvoice.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultValidInvoice.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultValidInvoice.DisplayLayout.Bands[0].Columns["Code"].Width = 150;
    }

    /// <summary>
    /// Load Changeable After Delivery
    /// </summary>
    private void LoadChangeableAfterDelivery()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 Pid, 'Yes' Code";
      commandText += " UNION ALL ";
      commandText += " SELECT 2 Pid, 'No' ";

      DataTable dtChangeable = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultChangeable.DataSource = dtChangeable;
      ultChangeable.DisplayMember = "Code";
      ultChangeable.ValueMember = "Pid";
      ultChangeable.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultChangeable.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultChangeable.DisplayLayout.Bands[0].Columns["Code"].Width = 150;

      ultTraining.DataSource = dtChangeable;
      ultTraining.DisplayMember = "Code";
      ultTraining.ValueMember = "Pid";
      ultTraining.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultTraining.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultTraining.DisplayLayout.Bands[0].Columns["Code"].Width = 150;

      ultTailor.DataSource = dtChangeable;
      ultTailor.DisplayMember = "Code";
      ultTailor.ValueMember = "Pid";
      ultTailor.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultTailor.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultTailor.DisplayLayout.Bands[0].Columns["Code"].Width = 150;
    }

    /// <summary>
    /// Load Source
    /// </summary>
    private void LoadSource()
    {
      string commandText = string.Empty;
      commandText += " SELECT Code, Value ";
      commandText += " FROM TblBOMCodeMaster ";
      commandText += " WHERE [Group] = 7013 ";
      commandText += " 	AND DeleteFlag = 0 ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultSource.DataSource = dtSource;
      ultSource.DisplayMember = "Value";
      ultSource.ValueMember = "Code";
      ultSource.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSource.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultSource.DisplayLayout.Bands[0].Columns["Value"].Width = 150;
    }


    /// <summary>
    /// Close Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Save Allocate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckError(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      int status = rdFix.Checked ? 1 : 0;
      DBParameter[] input = new DBParameter[29];
      // Update
      if (this.pidSupplierPrice > 0)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, this.pidSupplierPrice);
        input[27] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }
      // insert
      else
      {
        input[1] = new DBParameter("@MaterialCode", DbType.String, this.ultMaterialCode.Value.ToString());
        input[2] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(this.ultSupplier.Value.ToString()));
        input[26] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }

      input[3] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(this.txtPrice.Text));
      input[4] = new DBParameter("@Unit", DbType.Int32, DBConvert.ParseInt(this.ultUnit.Value.ToString()));
      input[5] = new DBParameter("@Currency", DbType.Int32, DBConvert.ParseInt(this.ultCurrency.Value.ToString()));
      input[6] = new DBParameter("@PaymentTime", DbType.String, this.txtPayment.Text);
      input[7] = new DBParameter("@ValidInvoice", DbType.Int32, DBConvert.ParseInt(this.ultValidInvoice.Value.ToString()));
      input[8] = new DBParameter("@BrandName", DbType.String, this.txtBrandName.Text);
      input[9] = new DBParameter("@Model", DbType.String, this.txtModel.Text);
      input[10] = new DBParameter("@Origin", DbType.String, this.txtOrigin.Text);
      input[11] = new DBParameter("@LeadTime", DbType.Double, DBConvert.ParseDouble(this.txtLeadTime.Text));
      input[12] = new DBParameter("@DeliveryTerm", DbType.String, this.txtDelivery.Text);
      input[13] = new DBParameter("@Warranty", DbType.Double, DBConvert.ParseDouble(this.txtWarrantyTime.Text));
      input[14] = new DBParameter("@Changeable", DbType.Int32, DBConvert.ParseInt(this.ultChangeable.Value.ToString()));
      input[15] = new DBParameter("@MinimumOrder", DbType.Double, DBConvert.ParseDouble(this.txtMinimumOrder.Text));
      if (this.txtPackingSpe.Text.Length > 0)
      {
        input[16] = new DBParameter("@PackingSpe", DbType.String, this.txtPackingSpe.Text);
      }
      if (this.ultSource.Value != null && DBConvert.ParseInt(this.ultSource.Value.ToString()) != int.MinValue)
      {
        input[17] = new DBParameter("@Source", DbType.Int32, DBConvert.ParseInt(this.ultSource.Value.ToString()));
      }
      if (this.txtSetUp.Text.Length > 0)
      {
        input[18] = new DBParameter("@SetUp", DbType.Double, DBConvert.ParseDouble(this.txtSetUp.Text));
      }
      if (this.ultQuality.Value != null && DBConvert.ParseInt(this.ultQuality.Value.ToString()) != int.MinValue)
      {
        input[19] = new DBParameter("@Quality", DbType.Int32, DBConvert.ParseInt(this.ultQuality.Value.ToString()));
      }
      if (this.ultTailor.Value != null && DBConvert.ParseInt(this.ultTailor.Value.ToString()) != int.MinValue)
      {
        input[20] = new DBParameter("@Tailor", DbType.Int32, DBConvert.ParseInt(this.ultTailor.Value.ToString()));
      }
      if (this.txtResponse.Text.Length > 0)
      {
        input[21] = new DBParameter("@Response", DbType.String, this.txtResponse.Text);
      }
      if (this.ultTraining.Value != null && DBConvert.ParseInt(this.ultTraining.Value.ToString()) != int.MinValue)
      {
        input[22] = new DBParameter("@Training", DbType.Int32, DBConvert.ParseInt(this.ultTraining.Value.ToString()));
      }
      if (this.txtPromotion.Text.Length > 0)
      {
        input[23] = new DBParameter("@Promotion", DbType.String, this.txtPromotion.Text);
      }
      if (this.txtSpecialDes.Text.Length > 0)
      {
        input[24] = new DBParameter("@SpecialDes", DbType.String, this.txtSpecialDes.Text);
      }
      if (this.txtRemark.Text.Length > 0)
      {
        input[25] = new DBParameter("@Remark", DbType.String, this.txtRemark.Text);
      }
      if (status != int.MinValue)
      {
        input[27] = new DBParameter("@Status", DbType.Int32, status);
      }
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spPURMaterialQuote_Edit", input, output);
      if (DBConvert.ParseInt(output[0].Value.ToString()) == 0)
      {
        return false;
      }
      this.pidSupplierPrice = DBConvert.ParseLong(output[0].Value.ToString());

      return true;
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      string commandText = string.Empty;
      commandText += " SELECT [MaterialCode], [SupplierPid], [Price], [Unit], [Currency],  ";
      commandText += " 	[PaymentTime], [ValidInvoice], [BrandName], [Model], [Origin], [LeadTime],   ";
      commandText += " 	[DeliveryTerm], [Warranty], [Changeable], [MinimumOrder], [PackingSpe],   ";
      commandText += " 	[Source], [SetUp], [Quality], [Tailor], [Response], [Training],   ";
      commandText += " 	[Promotion], [SpecialDes], [Remark], [Status]   ";
      commandText += " FROM TblPURMaterialQuote   ";
      commandText += " WHERE Pid = " + this.pidSupplierPrice;

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count == 1)
      {
        this.ultMaterialCode.Value = dt.Rows[0]["MaterialCode"].ToString();
        this.ultSupplier.Value = DBConvert.ParseLong(dt.Rows[0]["SupplierPid"].ToString());
        this.txtPrice.Text = dt.Rows[0]["Price"].ToString();
        double number = DBConvert.ParseDouble(txtPrice.Text);
        string numberRead = this.NumericFormat(number, 2);
        this.txtPrice.Text = numberRead;
        this.ultUnit.Value = DBConvert.ParseInt(dt.Rows[0]["Unit"].ToString());
        this.ultCurrency.Value = DBConvert.ParseInt(dt.Rows[0]["Currency"].ToString());
        this.txtPayment.Text = dt.Rows[0]["PaymentTime"].ToString();
        this.ultValidInvoice.Value = DBConvert.ParseInt(dt.Rows[0]["ValidInvoice"].ToString());
        this.txtBrandName.Text = dt.Rows[0]["BrandName"].ToString();
        this.txtModel.Text = dt.Rows[0]["Model"].ToString();
        this.txtOrigin.Text = dt.Rows[0]["Origin"].ToString();
        this.txtLeadTime.Text = dt.Rows[0]["LeadTime"].ToString();
        this.txtDelivery.Text = dt.Rows[0]["DeliveryTerm"].ToString();
        this.txtWarrantyTime.Text = dt.Rows[0]["Warranty"].ToString();
        this.ultChangeable.Value = DBConvert.ParseInt(dt.Rows[0]["Changeable"].ToString());
        this.txtMinimumOrder.Text = dt.Rows[0]["MinimumOrder"].ToString();
        this.txtPackingSpe.Text = dt.Rows[0]["PackingSpe"].ToString();
        if (DBConvert.ParseInt(dt.Rows[0]["Source"].ToString()) != int.MinValue)
        {
          this.ultSource.Value = DBConvert.ParseInt(dt.Rows[0]["Source"].ToString());
        }
        this.txtSetUp.Text = dt.Rows[0]["SetUp"].ToString();

        if (DBConvert.ParseInt(dt.Rows[0]["Quality"].ToString()) != int.MinValue)
        {
          this.ultQuality.Value = DBConvert.ParseInt(dt.Rows[0]["Quality"].ToString());
        }

        if (DBConvert.ParseInt(dt.Rows[0]["Tailor"].ToString()) != int.MinValue)
        {
          this.ultTailor.Value = DBConvert.ParseInt(dt.Rows[0]["Tailor"].ToString());
        }

        this.txtResponse.Text = dt.Rows[0]["Response"].ToString();

        if (DBConvert.ParseInt(dt.Rows[0]["Training"].ToString()) != int.MinValue)
        {
          this.ultTraining.Value = DBConvert.ParseInt(dt.Rows[0]["Training"].ToString());
        }
        this.txtPromotion.Text = dt.Rows[0]["Promotion"].ToString();
        this.txtSpecialDes.Text = dt.Rows[0]["SpecialDes"].ToString();
        this.txtRemark.Text = dt.Rows[0]["Remark"].ToString();
        int status = DBConvert.ParseInt(dt.Rows[0]["Status"].ToString());
        if (status == 1)
        {
          rdFix.Checked = true;
        }
        else if (status == 0)
        {
          rdNonFix.Checked = true;
        }
        this.ultMaterialCode.Enabled = false;
        this.ultSupplier.Enabled = false;
        this.btnBrown.Enabled = false;
        this.btnImport.Enabled = false;
      }
    }

    private string NumericFormat(double number, int phanLe)
    {
      if (number == double.MinValue)
      {
        return string.Empty;
      }
      if (phanLe < 0)
      {
        return number.ToString();
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      double t = Math.Truncate(number);
      formatInfo.NumberDecimalDigits = phanLe;
      return number.ToString("N", formatInfo);
    }

    /// <summary>
    /// Check Error
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckError(out string message)
    {
      message = string.Empty;

      // Material Code
      if (this.ultMaterialCode.Value == null
          || this.ultMaterialCode.Value.ToString().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Material Code");
        return false;
      }

      // Supplier
      if (this.ultSupplier.Value == null
          || DBConvert.ParseLong(this.ultSupplier.Value.ToString()) == long.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Supplier");
        return false;
      }

      // Unit
      if (this.ultUnit.Value == null
          || DBConvert.ParseInt(this.ultUnit.Value.ToString()) == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Unit");
        return false;
      }

      // Price
      if (this.txtPrice.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Price");
        return false;
      }
      else
      {
        double qty = DBConvert.ParseDouble(this.txtPrice.Text);
        if (qty < 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Price");
          return false;
        }
      }

      // Currency
      if (this.ultCurrency.Value == null
          || DBConvert.ParseInt(this.ultCurrency.Value.ToString()) == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Currency");
        return false;
      }

      // Payment Time
      if (this.txtPayment.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Payment Time");
        return false;
      }

      // Valid Invoice
      if (this.ultValidInvoice.Value == null
          || DBConvert.ParseInt(this.ultValidInvoice.Value.ToString()) == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Valid Invoice");
        return false;
      }

      // Brand Name / Trade Name
      if (this.txtBrandName.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Brand Name / Trade Name");
        return false;
      }

      // Model
      if (this.txtModel.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Model");
        return false;
      }

      // Origin
      if (this.txtOrigin.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Origin");
        return false;
      }

      // Lead Time
      if (this.txtLeadTime.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Lead Time");
        return false;
      }
      else
      {
        double qty = DBConvert.ParseDouble(this.txtLeadTime.Text);
        if (qty < 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Lead Time");
          return false;
        }
      }

      // Delivery Term
      if (this.txtDelivery.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Delivery Term");
        return false;
      }

      // Warranty Time
      if (this.txtWarrantyTime.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Warranty Time");
        return false;
      }
      else
      {
        double qty = DBConvert.ParseDouble(this.txtWarrantyTime.Text);
        if (qty < 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Warranty Time");
          return false;
        }
      }

      // Changeable After Delivery
      if (this.ultChangeable.Value == null
          || DBConvert.ParseInt(this.ultChangeable.Value.ToString()) == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Changeable After Delivery");
        return false;
      }

      // Minimum Order
      if (this.txtMinimumOrder.Text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Minimum Order");
        return false;
      }
      else
      {
        double qty = DBConvert.ParseDouble(this.txtMinimumOrder.Text);
        if (qty < 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Minimum Order");
          return false;
        }
      }

      // Setup year
      if (this.txtSetUp.Text.Length > 0)
      {
        double qty = DBConvert.ParseDouble(this.txtSetUp.Text);
        if (qty < 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Set-Up Year");
          return false;
        }
      }
      return true;
    }

    private void txtPrice_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtPrice.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtPrice.Text = numberRead;
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGetTemPlate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_PUR_08_001_09";
      string sheetName = "Sheet1";
      string outFileName = "TemplateImport";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Import Data
      if (this.txtImportExcel.Text.Trim().Length == 0)
      {
        return;
      }
      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Sheet1 (1)$D2:D27]").Tables[0];
        if (dtSource == null)
        {
          return;
        }

        if (dtSource.Rows.Count > 0)
        {
          string commandText = string.Empty;
          DataTable dt = new DataTable();
          if (dtSource.Rows[0][0].ToString().Length > 0)
          {
            this.ultMaterialCode.Value = dtSource.Rows[0][0].ToString();
          }

          if (dtSource.Rows[1][0].ToString().Length > 0)
          {
            commandText = string.Empty;
            commandText += " SELECT Pid ";
            commandText += " FROM TblPURSupplierInfo ";
            commandText += " WHERE SupplierCode = '" + dtSource.Rows[1][0].ToString() + "' ";

            dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count == 1)
            {
              this.ultSupplier.Value = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
            }
          }

          if (dtSource.Rows[2][0].ToString().Length > 0
              && DBConvert.ParseDouble(dtSource.Rows[2][0].ToString()) != double.MinValue)
          {
            this.txtPrice.Text = dtSource.Rows[2][0].ToString();
          }

          if (dtSource.Rows[3][0].ToString().Length > 0)
          {
            commandText = string.Empty;
            commandText += " SELECT Pid ";
            commandText += " FROM TblGNRMaterialUnit ";
            commandText += " WHERE Symbol = '" + dtSource.Rows[3][0].ToString() + "' ";

            dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count == 1)
            {
              this.ultUnit.Value = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
            }
          }

          if (dtSource.Rows[4][0].ToString().Length > 0)
          {
            commandText = string.Empty;
            commandText += " SELECT Pid ";
            commandText += " FROM TblPURCurrencyInfo ";
            commandText += " WHERE [Code] = '" + dtSource.Rows[4][0].ToString() + "' ";

            dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count == 1)
            {
              this.ultCurrency.Value = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
            }
          }

          if (dtSource.Rows[5][0].ToString().Length > 0)
          {
            this.txtPayment.Text = dtSource.Rows[5][0].ToString();
          }

          if (dtSource.Rows[6][0].ToString().Length > 0)
          {
            if (string.Compare("Direct Invoice", dtSource.Rows[6][0].ToString()) == 0)
            {
              this.ultValidInvoice.Value = 1;
            }
            else if (string.Compare("VAT Invoice", dtSource.Rows[6][0].ToString()) == 0)
            {
              this.ultValidInvoice.Value = 2;
            }
            else if (string.Compare("No Invoice", dtSource.Rows[6][0].ToString()) == 0)
            {
              this.ultValidInvoice.Value = 3;
            }
          }

          if (dtSource.Rows[7][0].ToString().Length > 0)
          {
            this.txtBrandName.Text = dtSource.Rows[7][0].ToString();
          }

          if (dtSource.Rows[8][0].ToString().Length > 0)
          {
            this.txtModel.Text = dtSource.Rows[8][0].ToString();
          }

          if (dtSource.Rows[9][0].ToString().Length > 0)
          {
            this.txtOrigin.Text = dtSource.Rows[9][0].ToString();
          }

          if (dtSource.Rows[10][0].ToString().Length > 0)
          {
            this.txtLeadTime.Text = dtSource.Rows[10][0].ToString();
          }

          if (dtSource.Rows[11][0].ToString().Length > 0)
          {
            this.txtDelivery.Text = dtSource.Rows[11][0].ToString();
          }

          if (dtSource.Rows[12][0].ToString().Length > 0)
          {
            this.txtWarrantyTime.Text = dtSource.Rows[12][0].ToString();
          }

          if (dtSource.Rows[13][0].ToString().Length > 0)
          {
            if (string.Compare("Yes", dtSource.Rows[13][0].ToString()) == 0)
            {
              this.ultChangeable.Value = 1;
            }
            else if (string.Compare("No", dtSource.Rows[13][0].ToString()) == 0)
            {
              this.ultChangeable.Value = 2;
            }
          }

          if (dtSource.Rows[14][0].ToString().Length > 0)
          {
            this.txtMinimumOrder.Text = dtSource.Rows[14][0].ToString();
          }

          if (dtSource.Rows[15][0].ToString().Length > 0)
          {
            if (string.Compare("Local", dtSource.Rows[15][0].ToString()) == 0)
            {
              this.ultSource.Value = 1;
            }
            else if (string.Compare("Import", dtSource.Rows[15][0].ToString()) == 0)
            {
              this.ultSource.Value = 2;
            }
          }

          if (dtSource.Rows[16][0].ToString().Length > 0)
          {
            this.txtPackingSpe.Text = dtSource.Rows[16][0].ToString();
          }

          if (dtSource.Rows[17][0].ToString().Length > 0)
          {
            this.txtSetUp.Text = dtSource.Rows[17][0].ToString();
          }

          if (dtSource.Rows[18][0].ToString().Length > 0)
          {
            if (string.Compare("Over Meet", dtSource.Rows[18][0].ToString()) == 0)
            {
              this.ultQuality.Value = 1;
            }
            else if (string.Compare("Meet", dtSource.Rows[18][0].ToString()) == 0)
            {
              this.ultQuality.Value = 2;
            }
            else if (string.Compare("Min Acceptable", dtSource.Rows[18][0].ToString()) == 0)
            {
              this.ultQuality.Value = 3;
            }
          }

          if (dtSource.Rows[19][0].ToString().Length > 0)
          {
            if (string.Compare("Yes", dtSource.Rows[19][0].ToString()) == 0)
            {
              this.ultTailor.Value = 1;
            }
            else if (string.Compare("No", dtSource.Rows[19][0].ToString()) == 0)
            {
              this.ultTailor.Value = 2;
            }
          }

          if (dtSource.Rows[20][0].ToString().Length > 0)
          {
            this.txtResponse.Text = dtSource.Rows[20][0].ToString();
          }

          if (dtSource.Rows[21][0].ToString().Length > 0)
          {
            if (string.Compare("Yes", dtSource.Rows[21][0].ToString()) == 0)
            {
              this.ultTraining.Value = 1;
            }
            else if (string.Compare("No", dtSource.Rows[21][0].ToString()) == 0)
            {
              this.ultTraining.Value = 2;
            }
          }

          if (dtSource.Rows[22][0].ToString().Length > 0)
          {
            this.txtPromotion.Text = dtSource.Rows[22][0].ToString();
          }

          if (dtSource.Rows[23][0].ToString().Length > 0)
          {
            this.txtSpecialDes.Text = dtSource.Rows[23][0].ToString();
          }

          if (dtSource.Rows[24][0].ToString().Length > 0)
          {
            this.txtRemark.Text = dtSource.Rows[24][0].ToString();
          }
        }
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }
    #endregion Process
  }
}

