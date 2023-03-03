/*
 * Author       : Vo Van Duy Qui
 * CreateDate   : 28-03-2011
 * Description  : Material Information
 */

using DaiCo.Application;
using DaiCo.Application.Web.Mail;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_01_003 : MainUserControl
  {
    #region Field
    private string imgPath = "J:\\Minh\\Image\\";
    public string materialGroup = string.Empty;
    public string materialCategory = string.Empty;
    public string materialCode = string.Empty;
    //private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private IList listDeletedParentPid = new ArrayList();
    private IList listDeletedChildPid = new ArrayList();
    private long pid = long.MinValue;
    private bool loadData = false;
    private bool canUpdate = true;
    private int status = int.MinValue;
    private long pidtemp = long.MinValue;
    public long viewPid = long.MinValue;
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    private string SP_GNR_MATERIALINFORMATION = "spGNRMaterialInformationDetail";
    private string SP_GNR_MATERIALINFO_INSERT = "spGNRMaterialInfo_Insert";
    private string SP_GNR_MATERIALINFO_UPDATE = "spGNRMaterialInfo_Update";
    private string SP_PUR_MATERIALSUPPLIER_EDIT = "spPURMaterialSupplier_Edit";
    private string SP_PUR_MATERIALSUPPLIER_DELETE = "spPURMaterialSupplier_Delete";
    #endregion Field

    #region Init
    /// <summary>
    /// Init Form
    /// </summary>
    public viewPUR_01_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_003_Load(object sender, EventArgs e)
    {
      btnCoefficient.Visible = false;
      this.LoadComboMaterialGroup();
      if (this.materialGroup.Length > 0 && this.materialCategory.Length > 0 && this.materialCode.Length == 0)
      {
        string prefix = this.materialGroup + "-" + this.materialCategory;
        ultComboMaterialGroup.Value = this.materialGroup;
        ultComboMaterialGroup.ReadOnly = true;
        this.LoadComboMaterialCategory(this.materialGroup);
        ultComboMaterialCategory.Value = this.materialCategory;
        ultComboMaterialCategory.ReadOnly = true;
        string material = DataBaseAccess.ExecuteScalarCommandText(string.Format("SELECT dbo.FPURGetNewMaterialCode ('{0}')", prefix)).ToString();
        txtCode.Text = material.Split('-')[2];
      }

      this.LoadComboCarcassCode();
      this.LoadComboDepartment();
      this.LoadComboGroupInCharge();
      this.LoadComboBoxUnit(ultComboUnit);
      this.LoadComboBoxUnit(ultComboFactoryUnit);
      this.LoadDropDownSupplier();

      // Load Source
      this.LoadComboSource();

      Utility.LoadComboboxCodeMst(cmbColor, 7003);
      Utility.LoadComboboxCodeMst(cmbMadeBy, 7004);
      this.LoadCBWarehouse();
      this.LoadData(this.materialCode);
      this.loadData = true;

      // Load Permission
      this.LoadPermission();

      

      //Phan quyen button save
      //string department = Shared.Utility.SharedObject.UserInfo.Department;
      //if (department != "PUR")
      //{
      //  this.btnSave.Enabled = false;
      //  this.btnCoefficient.Enabled = false;
      //}
      //else
      //{
      //  this.btnSave.Enabled = true;
      //  this.btnCoefficient.Enabled = true;
      //}
    }
    #endregion Init

    #region LoadData

   

    /// <summary>
    /// LoadPermission
    /// </summary>
    private void LoadPermission()
    {
      //int empPid = Shared.Utility.SharedObject.UserInfo.UserPid;
      //string commandText = "SELECT Manager FROM VHRDDepartmentInfo WHERE Code = 'PUR'";
      //object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      //int departmentManager = (int)obj;
      //if (empPid != departmentManager && (this.status == int.MinValue || this.status == 0))
      //{
      //  this.chkConfirm.Enabled = false;
      //}
    }

    /// <summary>
    /// Load UltraDropDown Supplier
    /// </summary>
    private void LoadDropDownSupplier()
    {
      string commandText = "SELECT Pid, SupplierCode, EnglishName FROM TblPURSupplierInfo WHERE DeleteFlg = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultddSupplier.DataSource = dtSource;
      ultddSupplier.DisplayMember = "SupplierCode";
      ultddSupplier.ValueMember = "Pid";
      ultddSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultddSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Material Group
    /// </summary>
    private void LoadComboMaterialGroup()
    {
      string commandText = "SELECT [Group], [Group] + ' - ' + Name AS Name FROM TblGNRMaterialGroup";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultComboMaterialGroup.DataSource = dtSource;
      ultComboMaterialGroup.DisplayMember = "Name";
      ultComboMaterialGroup.ValueMember = "Group";
      ultComboMaterialGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboMaterialGroup.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
    }

    //private void LoadCBWarehouse()
    //{
    //  string cmd = string.Format(@"SELECT Pid Value, DisplayText Display
    //          FROM VWHDWarehouseList
    //          ORDER BY DisplayText");
    //  DataTable dtMaterialGroup = DataBaseAccess.SearchCommandTextDataTable(cmd);
    //  Utility.LoadCheckBoxComboBox(chkCBOtherMaterial, dtOtherMaterial, "Code", "Value");
    //}

    public void LoadCBWarehouse()
    {
      string commandText = string.Format(@"SELECT Pid Value, DisplayText Display
              FROM VWHDWarehouseList
              ORDER BY DisplayText");
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbWarehouse, dtSource, "Value", "Display", "Value");
      ucbWarehouse.DisplayLayout.Bands[0].Columns["Display"].Header.Caption = "Warehouse";

      //Add an additional unbound column to WinCombo.
      //This will be used for the Selection of each Item
      UltraGridColumn checkedCol = ucbWarehouse.DisplayLayout.Bands[0].Columns.Add();
      checkedCol.Key = "Selected";
      checkedCol.Header.Caption = string.Empty;

      //This allows end users to select / unselect ALL items
      checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;
      checkedCol.DataType = typeof(bool);

      //Move the checkbox column to the first position.
      checkedCol.Header.VisiblePosition = 0;

      ucbWarehouse.CheckedListSettings.CheckStateMember = "Selected";
      ucbWarehouse.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems;
      // Set up the control to use a custom list delimiter
      ucbWarehouse.CheckedListSettings.ListSeparator = ", ";
      // Set ItemCheckArea to Item, so that clicking directly on an item also checks the item
      ucbWarehouse.CheckedListSettings.ItemCheckArea = Infragistics.Win.ItemCheckArea.Item;
    }

    /// <summary>
    /// Load UltraCombo Material Category
    /// </summary>
    /// <param name="group"></param>
    private void LoadComboMaterialCategory(string group)
    {
      string commandText = string.Format("SELECT Category, Category + ' - ' + Name AS Name FROM TblGNRMaterialCategory WHERE [Group] = '{0}'", group);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultComboMaterialCategory.DataSource = dtSource;
      ultComboMaterialCategory.DisplayMember = "Name";
      ultComboMaterialCategory.ValueMember = "Category";
      ultComboMaterialCategory.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboMaterialCategory.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = string.Format("SELECT Department, Department + ' - ' + DeparmentName AS DeparmentName FROM VHRDDepartment WHERE IsNew = 1 ORDER BY Department");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultComboDeptInCharge.DataSource = dtSource;
      ultComboDeptInCharge.ValueMember = "Department";
      ultComboDeptInCharge.DisplayMember = "DeparmentName";
      ultComboDeptInCharge.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboDeptInCharge.DisplayLayout.Bands[0].Columns["DeparmentName"].MinWidth = 200;
      ultComboDeptInCharge.DisplayLayout.Bands[0].Columns["DeparmentName"].MaxWidth = 200;
    }

    /// <summary>
    /// Load UltraCombo Group Incharge
    /// </summary>
    private void LoadComboGroupInCharge()
    {
      string commandText = "SELECT Pid, GroupName FROM TblPURStaffGroup WHERE DeleteFlg = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultComboGroupInCharge.DataSource = dtSource;
      ultComboGroupInCharge.ValueMember = "Pid";
      ultComboGroupInCharge.DisplayMember = "GroupName";
      ultComboGroupInCharge.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboGroupInCharge.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultComboGroupInCharge.DisplayLayout.Bands[0].Columns["GroupName"].Width = 250;
    }

    /// <summary>
    /// Load UltraCombo UltSource
    /// </summary>
    private void LoadComboSource()
    {
      string commandText = string.Empty;
      commandText += " SELECT Code, Value ";
      commandText += " FROM TblBOMCodeMaster ";
      commandText += " WHERE [Group] = 7013 ";
      commandText += "  	AND DeleteFlag = 0 ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultSource.DataSource = dtSource;
      ultSource.ValueMember = "Code";
      ultSource.DisplayMember = "Value";
      ultSource.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSource.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultSource.DisplayLayout.Bands[0].Columns["Value"].Width = 250;
    }

    /// <summary>
    /// Load UltraCombo Unit
    /// </summary>
    /// <param name="ultCombo"></param>
    private void LoadComboBoxUnit(UltraCombo ultCombo)
    {
      string commandText = "SELECT Pid, Symbol FROM TblGNRMaterialUnit";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCombo.DataSource = dtSource;
      ultCombo.DisplayMember = "Symbol";
      ultCombo.ValueMember = "Pid";
      ultCombo.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCombo.DisplayLayout.Bands[0].Columns["Symbol"].Width = 400;
      ultCombo.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load UltraCombo CarcassCode
    /// </summary>
    private void LoadComboCarcassCode()
    {
      string commandText = string.Empty;
      commandText = "   SELECT CarcassCode, ISNULL([Description], '') NameEN, ISNULL(DescriptionVN, '') NameVN";
      commandText += "  FROM TblBOMCarcass";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBCarcassCode.DataSource = dtSource;
      ultCBCarcassCode.ValueMember = "CarcassCode";
      ultCBCarcassCode.DisplayMember = "CarcassCode";
      ultCBCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBCarcassCode.DisplayLayout.Bands[0].Columns["CarcassCode"].Width = 100;
      ultCBCarcassCode.DisplayLayout.Bands[0].Columns["NameEN"].Width = 250;
      ultCBCarcassCode.DisplayLayout.Bands[0].Columns["NameVN"].Width = 250;
    }

    /// <summary>
    /// Load UltraCombo Useful
    /// 1: Local
    /// 2: Import
    /// </summary>
    private void LoadComboUseful(int source)
    {
      string commandText = string.Empty;
      commandText = string.Format(@"
                                    SELECT USF.[Source], USF.Useful
                                    FROM 
                                    (
	                                    SELECT 1 [Source], 90 Useful
	                                    UNION
	                                    SELECT 1, 95
	                                    UNION
	                                    SELECT 1, 100
	                                    UNION
	                                    SELECT 2, 90
	                                    UNION
	                                    SELECT 2, 95
	                                    UNION
	                                    SELECT 2, 100
                                    )USF
                                    WHERE USF.[Source] = {0}", source);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBUseful.DataSource = dtSource;
      ultCBUseful.ValueMember = "Useful";
      ultCBUseful.DisplayMember = "Useful";
      ultCBUseful.DisplayLayout.Bands[0].Columns["Source"].Hidden = true;
      ultCBUseful.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Format Number
    /// </summary>
    /// <param name="number"></param>
    /// <param name="phanLe"></param>
    /// <returns></returns>
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
    /// Create Data Set
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("SupplierPid", typeof(System.Int64));
      taParent.Columns.Add("EnglishName", typeof(System.String));
      taParent.Columns.Add("SupplierReferenceCode", typeof(System.String));
      taParent.Columns.Add("SupplierReferenceName", typeof(System.String));
      taParent.Columns.Add("IsDefault", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("ContractNo", typeof(System.String));
      taChild.Columns.Add("StartDate", typeof(System.DateTime));
      taChild.Columns.Add("ExpireDate", typeof(System.DateTime));
      taChild.Columns.Add("Price", typeof(System.Double));
      taChild.Columns.Add("MaterialSupplierPid", typeof(System.Int64));

      ds.Tables.Add(taChild);
      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["MaterialSupplierPid"], false));
      return ds;
    }
    /// <summary>
    /// Load Data With Material Code
    /// </summary>
    /// <param name="materialCode"></param>
    private void LoadData(string materialCode)
    {
      if (materialCode.Length > 0)
      {
        //picBoxMaterial.ImageLocation = this.imgPath + materialCode + ".wmf";

        picBoxMaterial.ImageLocation = @FunctionUtility.GNRGetMaterialImage(materialCode);
        ultComboMaterialGroup.ReadOnly = true;
        ultComboMaterialCategory.ReadOnly = true;
        txtCode.ReadOnly = true;
      }

      DBParameter[] input = new DBParameter[] { new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(SP_GNR_MATERIALINFORMATION, input);

      if (dsSource != null)
      {
        // Material Info
        DataTable dtMain = dsSource.Tables[0];
        if (dtMain.Rows.Count > 0)
        {
          txtNameEN.Text = dtMain.Rows[0]["NameEN"].ToString().Trim();
          txtNameVN.Text = dtMain.Rows[0]["NameVN"].ToString().Trim();
          txtDensity.Text = dtMain.Rows[0]["Density"].ToString().Trim();
          txtWaste.Text = dtMain.Rows[0]["Waste"].ToString().Trim();
          txtStandardCost.Text = dtMain.Rows[0]["StandardCost"].ToString().Trim();
          txtOthers.Text = dtMain.Rows[0]["Remark"].ToString().Trim();
          txtDimension.Text = dtMain.Rows[0]["Dimension"].ToString().Trim();
          txtSpecification.Text = dtMain.Rows[0]["Specification"].ToString().Trim();

          ultComboUnit.Value = dtMain.Rows[0]["Unit"];
          ultComboMaterialGroup.Value = dtMain.Rows[0]["Group"];
          ultComboMaterialCategory.Value = dtMain.Rows[0]["Category"];
          ultComboFactoryUnit.Value = dtMain.Rows[0]["FactoryUnit"];
          ultComboGroupInCharge.Value = dtMain.Rows[0]["GroupIncharge"];
          ultComboDeptInCharge.Value = dtMain.Rows[0]["DepartmentInCharge"];
          ultCBCarcassCode.Value = dtMain.Rows[0]["CarcassCode"];

          if (DBConvert.ParseInt(dtMain.Rows[0]["Source"].ToString()) != int.MinValue)
          {
            this.ultSource.Value = DBConvert.ParseInt(dtMain.Rows[0]["Source"].ToString());
          }
          // Load Purchase Price
          if (DBConvert.ParseDouble(dtMain.Rows[0]["PurchasePrice"]) > 0)
          {
            txtPurchasPrice.Text = dtMain.Rows[0]["PurchasePrice"].ToString();
          }
          // Load Useful
          if (DBConvert.ParseDouble(dtMain.Rows[0]["Useful"]) >= 0)
          {
            ultCBUseful.Value = DBConvert.ParseDouble(dtMain.Rows[0]["Useful"]);
          }

          string material = dtMain.Rows[0]["MaterialCode"].ToString().Trim();
          txtGroup.Text = material.Split('-')[0];
          txtCategory.Text = material.Split('-')[1];
          txtCode.Text = material.Split('-')[2];
          cmbColor.SelectedValue = dtMain.Rows[0]["Color"];
          cmbMadeBy.SelectedValue = dtMain.Rows[0]["MadeBy"];

          //CheckedValueListItemsCollection chekedItems;
          // checkedValue = string.Format("|{0}|", checkedValue);

          Utility.SetCheckedValueUltraCombobox(ucbWarehouse, dtMain.Rows[0]["WHPids"].ToString());
          //txtWarningPricePercent.Text = dtMain.Rows[0]["WarningPricePercent"].ToString();
          this.txtLeadTime.Text = dtMain.Rows[0]["LeadTime"].ToString();
          this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"].ToString());
          chkConfirm.Checked = (this.status > 0);
          if (chkConfirm.Checked)
          {
            this.SetStatusControl(Shared.Utility.SharedObject.UserInfo.UserPid, true, this.status);
          }
        }

        // Supplier Info
        //dsSource.Tables[1].PrimaryKey = new DataColumn[] { dsSource.Tables[1].Columns["SupplierReferenceCode"] };
        //ultData.DataSource = dsSource.Tables[1];
        DataSet dsMain = this.CreateDataSet();
        dsMain.Tables["dtParent"].Merge(dsSource.Tables[1]);
        dsMain.Tables["dtChild"].Merge(dsSource.Tables[2]);
        ultData.DataSource = dsMain;
      }
    }

    /// <summary>
    /// Set Status For Control
    /// </summary>
    /// <param name="empPid"></param>
    /// <param name="isEnable"></param>
    private void SetStatusControl(int empPid, bool isEnable, int status)
    {
      if (status == 1)
      {
        ultComboMaterialGroup.ReadOnly = isEnable;
        ultComboMaterialCategory.ReadOnly = isEnable;
        txtCode.ReadOnly = isEnable;
        txtNameEN.ReadOnly = isEnable;
        ultCBCarcassCode.ReadOnly = !isEnable;
        txtNameVN.ReadOnly = isEnable;
        ultComboUnit.ReadOnly = isEnable;
        ultComboFactoryUnit.ReadOnly = !isEnable;
        ultComboGroupInCharge.ReadOnly = !isEnable;
        chkConfirm.Enabled = isEnable;
        ultSource.ReadOnly = !isEnable;
        txtLeadTime.ReadOnly = !isEnable;
      }
      if (status == 2)
      {
        ultComboMaterialGroup.ReadOnly = isEnable;
        ultComboMaterialCategory.ReadOnly = isEnable;
        txtCode.ReadOnly = isEnable;
        txtNameEN.ReadOnly = isEnable;
        ultCBCarcassCode.ReadOnly = isEnable;
        txtNameVN.ReadOnly = isEnable;
        ultComboUnit.ReadOnly = isEnable;
        chkConfirm.Enabled = !isEnable;
        ultComboFactoryUnit.ReadOnly = !isEnable;
        ultComboGroupInCharge.ReadOnly = !isEnable;
        ultSource.ReadOnly = !isEnable;
        txtLeadTime.ReadOnly = !isEnable;
      }
    }
    #endregion LoadData

    #region Check And Save

    /// <summary>
    /// Check Input Data
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      // Material Group
      string group = Utility.GetSelectedValueUltraCombobox(ultComboMaterialGroup);
      if (group.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0011", "Material Group");
        this.SaveSuccess = false;
        return false;
      }

      string commandGroup = string.Format("SELECT COUNT(*) FROM TblGNRMaterialGroup WHERE [Group] = '{0}'", group);
      object objGroup = DataBaseAccess.ExecuteScalarCommandText(commandGroup);
      if ((objGroup == null) || (objGroup != null && (int)objGroup == 0))
      {
        WindowUtinity.ShowMessageError("ERR0011", "Material Group");
        this.SaveSuccess = false;
        return false;
      }
      // Material Category
      string category = Utility.GetSelectedValueUltraCombobox(ultComboMaterialCategory);
      if (category.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0011", "Material Category");
        this.SaveSuccess = false;
        return false;
      }

      string commandCategory = string.Format("SELECT COUNT(*) FROM TblGNRMaterialCategory WHERE [Group] = '{0}' AND Category = '{1}'", group, category);
      object objCategory = DataBaseAccess.ExecuteScalarCommandText(commandCategory);
      if ((objCategory == null) || (objCategory != null && (int)objCategory == 0))
      {
        WindowUtinity.ShowMessageError("ERR0011", "Material Category");
        this.SaveSuccess = false;
        return false;
      }
      // Material Code
      string code = txtCode.Text.Trim();
      if (code.Length < 5)
      {
        WindowUtinity.ShowMessageError("WRN0013", "MaterialCode");
        this.SaveSuccess = false;
        txtCode.Focus();
        return false;
      }

      if (this.materialCode.Length == 0)
      {
        string material = txtGroup.Text + "-" + txtCategory.Text.Trim() + "-" + txtCode.Text.Trim();
        string commandText = string.Format("SELECT COUNT (*) FROM TblGNRMaterialInformation WHERE MaterialCode = '{0}'", material);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (obj != null && (int)obj > 0)
        {
          WindowUtinity.ShowMessageError("ERR0028", "MaterialCode");
          this.SaveSuccess = false;
          return false;
        }
      }
      // Name EN
      string nameEN = txtNameEN.Text.Trim();
      if (nameEN.Length == 0)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Name EN");
        this.SaveSuccess = false;
        txtNameEN.Focus();
        return false;
      }
      // Name VN
      string nameVN = txtNameVN.Text.Trim();
      if (nameVN.Length == 0)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Name VN");
        this.SaveSuccess = false;
        txtNameVN.Focus();
        return false;
      }
      // Unit
      long unit = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultComboUnit));
      if (unit == long.MinValue)
      {
        WindowUtinity.ShowMessageError("MSG0011", "Unit");
        this.SaveSuccess = false;
        ultComboUnit.Focus();
        return false;
      }

      // Group In Charge
      long groupInCharge = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultComboGroupInCharge));
      if (groupInCharge == long.MinValue)
      {
        WindowUtinity.ShowMessageError("MSG0011", "Group In Charge");
        this.SaveSuccess = false;
        ultComboGroupInCharge.Focus();
        return false;
      }

      // Waste
      if (txtWaste.Text.Trim().Length > 0)
      {
        double waste = DBConvert.ParseDouble(txtWaste.Text);
        if (waste == double.MinValue)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Waste");
          txtWaste.Focus();
          return false;
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0013", "Waste");
        this.SaveSuccess = false;
        txtWaste.Focus();
        return false;
      }

      // Check Source
      if (this.ultSource.Value == null || DBConvert.ParseInt(this.ultSource.Value) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Source");
        this.SaveSuccess = false;
        this.ultSource.Focus();
        return false;
      }

      // Check Lead Time
      if (txtLeadTime.Text.Trim().Length > 0)
      {
        double leadTime = DBConvert.ParseDouble(txtLeadTime.Text);
        if (leadTime == double.MinValue)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Lead Time");
          txtLeadTime.Focus();
          return false;
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0013", "Lead Time");
        this.SaveSuccess = false;
        txtLeadTime.Focus();
        return false;
      }

      // Chek CarcassCode
      if (ultCBCarcassCode.Text.Trim().Length > 0 && ultCBCarcassCode.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "CarcassCode");
        this.SaveSuccess = false;
        return false;
      }
      else if (ultCBCarcassCode.Value != null)
      {
        string commandCarcassCode = string.Empty;
        commandCarcassCode = "SELECT FactoryComponentCode FROM TblGNRMaterialInformation";
        commandCarcassCode += " WHERE FactoryComponentCode = '" + ultCBCarcassCode.Value.ToString() + "' AND MaterialCode != '" + this.materialCode + "'";
        DataTable dtCarcassCode = DataBaseAccess.SearchCommandTextDataTable(commandCarcassCode);
        if (dtCarcassCode != null && dtCarcassCode.Rows.Count > 0)
        {
          WindowUtinity.ShowMessageError("ERR0028", "Factory ComponentCode");
          this.SaveSuccess = false;
          return false;
        }
      }

      // Check Purchase Price
      if (txtPurchasPrice.Text.Trim().Length > 0)
      {
        if (DBConvert.ParseDouble(txtPurchasPrice.Text) < 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Purchase Price");
          this.SaveSuccess = false;
          return false;
        }
      }

      //check warehouse
      if(ucbWarehouse.Text.Trim().Length <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Warehouse");
        this.SaveSuccess = false;
        return false;
      }  

      // Supplier Material
      DataTable dtSource = (DataTable)((DataSet)ultData.DataSource).Tables["dtParent"];
      if (dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            // Supllier
            long supplierPid = DBConvert.ParseLong(row["SupplierPid"].ToString());
            DataRow[] rowsDup = dtSource.Select(string.Format("SupplierPid = {0}", supplierPid));
            if (rowsDup.Length > 1)
            {
              WindowUtinity.ShowMessageError("ERR0006", "Supplier");
              this.SaveSuccess = false;
              return false;
            }

            // Reference Supplier Code
            string referenceSupplierCode = row["SupplierReferenceCode"].ToString().Trim();
            if (referenceSupplierCode.Length == 0)
            {
              WindowUtinity.ShowMessageError("WRN0013", "Reference Supplier Code");
              this.SaveSuccess = false;
              return false;
            }
          }
        }
      }
      //Contract No
      DataTable dt = (DataTable)((DataSet)ultData.DataSource).Tables["dtChild"];
      if (dt.Rows.Count > 0)
      {
        foreach (DataRow row in dt.Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            string contract = DBConvert.ParseString(row["ContractNo"].ToString());
            if (pid < 0 && contract != "")
            {
              // Price
              double price = DBConvert.ParseDouble(row["Price"].ToString());
              if (price != double.MinValue)
              {
                if (price < 0)
                {
                  WindowUtinity.ShowMessageError("ERR0001", "Price");
                  this.SaveSuccess = false;
                  return false;
                }
              }

              // StartDate 

              if (DBConvert.ParseDateTime(row["StartDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) >= DBConvert.ParseDateTime(row["ExpireDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME))
              {
                MessageBox.Show("StartDate <= ExpireDate");
                this.SaveSuccess = false;
                return false;
              }
            }
          }
        }
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string reference = ultData.Rows[i].Cells["SupplierReferenceCode"].Value.ToString().Trim();
        if (reference.Length == 0)
        {
          WindowUtinity.ShowMessageError("WRN0013", "Reference Supplier Code");
          this.SaveSuccess = false;
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    private string SaveData()
    {
      string storeName = string.Empty;
      string group = Utility.GetSelectedValueUltraCombobox(ultComboMaterialGroup);
      string category = Utility.GetSelectedValueUltraCombobox(ultComboMaterialCategory);
      string code = txtGroup.Text + "-" + txtCategory.Text + "-" + txtCode.Text.Trim();
      int color = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbColor));
      string nameEN = txtNameEN.Text.Trim();
      string nameVN = txtNameVN.Text.Trim();
      long unit = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultComboUnit));
      long factoryUnit = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultComboFactoryUnit));
      string deptIncharge = Utility.GetSelectedValueUltraCombobox(ultComboDeptInCharge);
      long groupIncharge = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultComboGroupInCharge));
      double waste = DBConvert.ParseDouble(txtWaste.Text);
      string density = txtDensity.Text;
      string remark = txtOthers.Text.Trim();
      
      string carcassCode = (ultCBCarcassCode.Value != null) ? ultCBCarcassCode.Value.ToString() : string.Empty;
      double purchasePrice = DBConvert.ParseDouble(txtPurchasPrice.Text);
      double useful = DBConvert.ParseDouble(ultCBUseful.Value);
      string specification = txtSpecification.Text.Trim();
      if (this.status == int.MinValue || this.status == 0)
      {
        this.status = (chkConfirm.Checked) ? 1 : 0;
      }
      else
      {
        this.status = (!chkConfirm.Checked) ? 0 : this.status;
      }
      string dimension = txtDimension.Text.Trim();
      int madeby = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbMadeBy));

      DBParameter[] input = new DBParameter[26];
      input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, code);
      input[1] = new DBParameter("@NameEN", DbType.String, 256, nameEN);
      input[2] = new DBParameter("@NameVN", DbType.String, 256, nameVN);
      input[3] = new DBParameter("@Unit", DbType.Int64, unit);
      if (factoryUnit != long.MinValue)
      {
        input[4] = new DBParameter("@FactoryUnit", DbType.Int64, factoryUnit);
      }

      input[7] = new DBParameter("@GroupInCharge", DbType.Int64, groupIncharge);
      if (color != int.MinValue)
      {
        input[8] = new DBParameter("@Color", DbType.Int32, color);
      }
      if (dimension.Length > 0)
      {
        input[9] = new DBParameter("@Dimension", DbType.AnsiString, 100, dimension);
      }
      if (madeby != int.MinValue)
      {
        input[10] = new DBParameter("@MadeBy", DbType.Int32, madeby);
      }
      if (waste != double.MinValue)
      {
        input[11] = new DBParameter("@Waste", DbType.Double, waste);
      }
      if (density.Length > 0)
      {
        input[12] = new DBParameter("@Density", DbType.String, density);
      }

      if (deptIncharge.Length > 0)
      {
        input[13] = new DBParameter("@DepartmentInCharge", DbType.AnsiString, 50, deptIncharge);
      }
      if (remark.Length > 0)
      {
        input[14] = new DBParameter("@Remark", DbType.String, 512, remark);
      }
      input[15] = new DBParameter("@Status", DbType.Int32, this.status);

      
      input[16] = new DBParameter("@WhPids", DbType.String, Utility.GetCheckedValueUltraCombobox(ucbWarehouse));
      
      if (this.materialCode.Length > 0)
      {
        input[17] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        input[18] = new DBParameter("@Source", DbType.Int32, DBConvert.ParseInt(this.ultSource.Value.ToString()));
        input[19] = new DBParameter("@LeadTime", DbType.Double, DBConvert.ParseDouble(this.txtLeadTime.Text.ToString()));
        storeName = SP_GNR_MATERIALINFO_UPDATE;
      }
      else
      {
        input[5] = new DBParameter("@Group", DbType.AnsiString, 3, group);
        input[6] = new DBParameter("@Category", DbType.AnsiString, 2, category);
        input[17] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        input[18] = new DBParameter("@Source", DbType.Int32, DBConvert.ParseInt(this.ultSource.Value.ToString()));
        input[19] = new DBParameter("@LeadTime", DbType.Double, DBConvert.ParseDouble(this.txtLeadTime.Text.ToString()));
        storeName = SP_GNR_MATERIALINFO_INSERT;
      }
      if (carcassCode.Length > 0)
      {
        input[20] = new DBParameter("@CarcassCode", DbType.String, carcassCode);
      }
      if (purchasePrice != double.MinValue)
      {
        input[21] = new DBParameter("@PurchasePrice", DbType.Double, purchasePrice);
      }
      if (useful != double.MinValue)
      {
        input[22] = new DBParameter("@Useful", DbType.Double, useful);
      }
      if (specification.Length > 0)
      {
        input[23] = new DBParameter("@Specification", DbType.String, specification);
      }
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, 3000, input, output);
      string result = output[0].Value.ToString().Trim();
      return result;
    }


    private void SendEmail()
    {
      string commadText = string.Format(@"SELECT CONVERT(VARCHAR, MAT.CreateDate, 103) CreateDate, EM.EmpName +'  MSNV: '+ CONVERT(VARCHAR, EM.Pid) CreateBy ,MAT.MaterialCode, MAT.NameEN, MAT.NameVN, UNIT.Symbol Unit, MAT.GroupIncharge, Status
                                          FROM TblGNRMaterialInformation MAT
                                            LEFT JOIN TblGNRMaterialUnit UNIT ON UNIT.Pid = MAT.Unit
	                                          LEFT JOIN VHRMEmployee EM ON EM.Pid = MAT.CreateBy
                                          WHERE MAT.MaterialCode = '{0}'", this.materialCode);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commadText);
      if (dt != null && dt.Rows.Count == 1)
      {
        if (DBConvert.ParseInt(dt.Rows[0]["Status"].ToString()) == 0)
        {
          // Mail
          MailMessage mailMessage = new MailMessage();
          string body = string.Empty;
          body += "<p><i><font color='6699CC'>";
          body += "Create By: " + dt.Rows[0]["CreateBy"].ToString() + "<br>";
          body += "Create Date: " + dt.Rows[0]["CreateDate"].ToString() + "<br>";
          // table
          body += "<table border = '1'>";

          body += "<tr>";
          body += "<th>";
          body += "Material Code";
          body += "</th>";
          body += "<th>";
          body += "NameVN";
          body += "</th>";
          body += "<th>";
          body += "NameEN";
          body += "</th>";
          body += "<th>";
          body += "Unit";
          body += "</th>";
          body += "</tr>";

          body += "<tr>";
          body += "<td>";
          body += dt.Rows[0]["MaterialCode"].ToString();
          body += "</td>";
          body += "<td>";
          body += dt.Rows[0]["NameEN"].ToString();
          body += "</td>";
          body += "<td>";
          body += dt.Rows[0]["NameVN"].ToString();
          body += "</td>";
          body += "<td>";
          body += dt.Rows[0]["Unit"].ToString();
          body += "</td>";
          body += "</tr>";
          body += "</table>";

          body += "<p><i><font color='red'>";
          body += "Automatic Email From ERP System.";
          body += "</font></i></p> ";

          string mailTo = string.Empty;
          string commandTextMail = string.Format(@" SELECT NV.email
                                              FROM
                                              (
	                                              SELECT LeaderGroup Employee FROM TblPURStaffGroup WHERE Pid = {0}
	                                              UNION
	                                              SELECT Employee FROM TblPURStaffGroupDetail WHERE [Group] = {1}
	                                              UNION
	                                              SELECT Manager FROM VHRDDepartmentInfo WHERE Code = 'PUR'
                                              )EM
	                                              LEFT JOIN VHRNhanVien NV ON NV.ID_NhanVien = EM.Employee
                                                WHERE NV.Resigned = 0", DBConvert.ParseInt(dt.Rows[0]["GroupIncharge"].ToString()), DBConvert.ParseInt(dt.Rows[0]["GroupIncharge"].ToString()));
          DataTable dtMailTo = DataBaseAccess.SearchCommandTextDataTable(commandTextMail);
          if (dtMailTo != null && dtMailTo.Rows.Count > 0)
          {
            for (int i = 0; i < dtMailTo.Rows.Count; i++)
            {
              mailTo += "," + dtMailTo.Rows[i]["email"].ToString();
            }
            mailTo = mailTo.Substring(1, mailTo.Length - 1);
          }
          //mailMessage.ServerName = "10.0.0.5";
          //mailMessage.Username = "dc@daico-furniture.com";
          //mailMessage.Password = "dc123456";
          //mailMessage.From = "dc@daico-furniture.com";

          mailMessage.ServerName = "10.0.0.6";
          mailMessage.Username = "dc@vfr.net.vn";
          mailMessage.Password = "dc123456";
          mailMessage.From = "dc@vfr.net.vn";

          mailMessage.To = mailTo;
          mailMessage.Subject = "NEW MATERIAL CODE";
          mailMessage.Body = body;
          //mailMessage.Bcc = "truong_it@daico-furniture.com";
          IList attachments = new ArrayList();
          //attachments.Add(pathImage);
          mailMessage.Attachfile = attachments;
          //mailMessage.SendMail(true);
        }
      }
    }
    /// <summary>
    /// Save Material Supplier
    /// </summary>
    /// <param name="material"></param>
    private void SaveMaterialSupplier(string material)
    {
      //1: Delete Parent
      foreach (long pidDelete in this.listDeletedParentPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure(SP_PUR_MATERIALSUPPLIER_DELETE, inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete == 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          this.SaveSuccess = false;
          return;
        }
      }
      // 2: Delete Detail
      foreach (long pidDeleteDetail in this.listDeletedChildPid)
      {
        DBParameter[] inputDelete1 = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDeleteDetail) };
        DBParameter[] outputDelete1 = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure("spPURContactNolSupplier_Delete", inputDelete1, outputDelete1);
        long resultDelete = DBConvert.ParseLong(outputDelete1[0].Value.ToString());
        if (resultDelete == 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          this.SaveSuccess = false;
          return;
        }
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        long pid = DBConvert.ParseLong(rowParent.Cells["Pid"].Value.ToString());
        long supplierPid = DBConvert.ParseLong(rowParent.Cells["SupplierPid"].Value.ToString());
        string supplierReferenceName = rowParent.Cells["SupplierReferenceName"].Value.ToString().Trim();
        string supplierReferenceCode = rowParent.Cells["SupplierReferenceCode"].Value.ToString().Trim();

        DBParameter[] input = new DBParameter[8];
        if (pid > 0)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, pid);
          input[6] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        }
        input[1] = new DBParameter("@SupplierPid", DbType.Int64, supplierPid);
        input[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, material);
        input[3] = new DBParameter("@SupplierReferenceName", DbType.String, 256, supplierReferenceName);
        input[4] = new DBParameter("@SupplierReferenceCode", DbType.String, 256, supplierReferenceCode);
        input[5] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        if (DBConvert.ParseInt(rowParent.Cells["IsDefault"].Value.ToString()) == 1)
        {
          input[7] = new DBParameter("@IsDefault", DbType.Int32, DBConvert.ParseInt(rowParent.Cells["IsDefault"].Value.ToString()));
        }
        else
        {
          input[7] = new DBParameter("@IsDefault", DbType.Int32, 0);
        }
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure(SP_PUR_MATERIALSUPPLIER_EDIT, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 0)
        {
          WindowUtinity.ShowMessageError("ERR0005");
          this.SaveSuccess = false;
          return;
        }
        else
        {
          for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            long detailPid = DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Pid"].Value.ToString());
            string start = DBConvert.ParseString(DBConvert.ParseDateTime(ultData.Rows[i].ChildBands[0].Rows[j].Cells["StartDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
            string expire = DBConvert.ParseString(DBConvert.ParseDateTime(ultData.Rows[i].ChildBands[0].Rows[j].Cells["ExpireDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
            DBParameter[] input1 = new DBParameter[6];

            if (detailPid > 0)
            {
              input1[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
            }

            input1[1] = new DBParameter("@ContractNo", DbType.String, ultData.Rows[i].ChildBands[0].Rows[j].Cells["ContractNo"].Value.ToString());
            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Price"].Value.ToString()) > 0)
            {
              input1[2] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Price"].Value.ToString()));
            }
            input1[3] = new DBParameter("@MaterialSupplierPid", DbType.Int64, result);
            if (start.Length > 0)
            {
              input1[4] = new DBParameter("@StartDate", DbType.DateTime, DBConvert.ParseDateTime(ultData.Rows[i].ChildBands[0].Rows[j].Cells["StartDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            }
            if (expire.Length > 0)
            {
              input1[5] = new DBParameter("@ExpireDate", DbType.DateTime, DBConvert.ParseDateTime(ultData.Rows[i].ChildBands[0].Rows[j].Cells["ExpireDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            }
            DBParameter[] output1 = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spPURContractSupplier_Edit", input1, output1);
            long resultSave = DBConvert.ParseLong(output1[0].Value.ToString());
            if (resultSave == 0)
            {
              WindowUtinity.ShowMessageError("ERR0005");
              this.SaveSuccess = false;
              return;
            }
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.SaveSuccess = true;
    }
    #endregion Check And Save

    #region Event
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// btnSave Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckValid())
      {
        this.materialCode = this.SaveData();
        if (this.materialCode.Length > 0)
        {
          this.SaveMaterialSupplier(this.materialCode);
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
          this.SaveSuccess = false;
          return;
        }
        //Send Mail
        this.SendEmail();
        this.LoadData(this.materialCode);

      }
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      //e.Layout.Bands[2].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["MaterialSupplierPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Open"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["Open"].MinWidth = 100;

      e.Layout.Bands[0].Columns["SupplierPid"].Header.Caption = "Supplier";
      e.Layout.Bands[0].Columns["SupplierPid"].ValueList = ultddSupplier;
      e.Layout.Bands[0].Columns["SupplierPid"].Header.Caption = "Supplier Name";
      e.Layout.Bands[0].Columns["SupplierReferenceName"].Header.Caption = "Supplier Reference Name";
      e.Layout.Bands[0].Columns["SupplierReferenceCode"].Header.Caption = "Supplier Reference Code";
      e.Layout.Bands[1].Columns["ContractNo"].Header.Caption = "Contract No";
      e.Layout.Bands[1].Columns["StartDate"].Header.Caption = "Start Date";
      e.Layout.Bands[1].Columns["ExpireDate"].Header.Caption = "Expire Date";
      e.Layout.Bands[0].Columns["IsDefault"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsDefault"].DefaultCellValue = 0;
      e.Layout.Bands[1].Columns["Open"].Header.Caption = "View File";
      e.Layout.Bands[1].Columns["Open"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[1].Columns["Open"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
    }

    /// <summary>
    /// ultComboMaterialGroup Value Changed 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboMaterialGroup_ValueChanged(object sender, EventArgs e)
    {
      if (ultComboMaterialGroup.SelectedRow != null)
      {
        string group = ultComboMaterialGroup.SelectedRow.Cells["Group"].Value.ToString().Trim();
        this.LoadComboMaterialCategory(group);
        ultComboMaterialCategory.Value = string.Empty;
        txtGroup.Text = string.Empty;
        txtCategory.Text = string.Empty;
        txtCode.Text = string.Empty;
      }
    }

    /// <summary>
    /// ultComboMaterialCategory Value Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboMaterialCategory_ValueChanged(object sender, EventArgs e)
    {
      if (ultComboMaterialGroup.SelectedRow != null && ultComboMaterialCategory.SelectedRow != null && this.materialCode.Length == 0)
      {
        string group = ultComboMaterialGroup.SelectedRow.Cells["Group"].Value.ToString().Trim();
        string category = ultComboMaterialCategory.SelectedRow.Cells["Category"].Value.ToString().Trim();
        if (category.Length > 0)
        {
          string prefix = group + "-" + category;
          string material = DataBaseAccess.ExecuteScalarCommandText(string.Format("SELECT dbo.FPURGetNewMaterialCode ('{0}')", prefix)).ToString();
          txtGroup.Text = material.Split('-')[0];
          txtCategory.Text = material.Split('-')[1];
          txtCode.Text = material.Split('-')[2];
        }
        else
        {
          txtGroup.Text = string.Empty;
          txtCategory.Text = string.Empty;
          txtCode.Text = string.Empty;
        }
      }
    }

    /// <summary>
    /// ultData Before Rows Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      //this.listDeletingPid = new ArrayList();
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      foreach (UltraGridRow row in e.Rows)
      {
        //long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        //if (pid != long.MinValue)
        //{
        //  this.listDeletingPid.Add(pid);
        //}
        if (row.ParentRow == null)
        {
          long pidParent = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (pidParent != long.MinValue)
          {
            this.listDeletedParentPid.Add(pidParent);
          }
        }
        else
        {
          long pidChild = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (pidChild != long.MinValue)
          {
            this.listDeletedChildPid.Add(pidChild);
          }
        }
      }
    }

    /// <summary>
    /// ultData After Rows Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletedParentPid)
      {
        this.listDeletedPid.Add(pid);
      }
      foreach (long pid in this.listDeletedChildPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    /// <summary>
    /// ultData After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      if (colName.CompareTo("supplierpid") == 0)
      {
        e.Cell.Row.Cells["EnglishName"].Value = ultddSupplier.SelectedRow.Cells["EnglishName"].Value.ToString();
      }
      switch (colName)
      {
        case "contractno":
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) == long.MinValue)
            {
              e.Cell.Row.Cells["Pid"].Value = pidtemp;
              pidtemp += 1;
            }
          }
          break;
        case "supplierpid":
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) == long.MinValue)
            {
              pidtemp += 1;
              e.Cell.Row.Cells["Pid"].Value = pidtemp;
            }
          }
          break;
        case "isdefault":
          {
            if (e.Cell.Value.ToString() == "1")
            {
              int rowIndex = e.Cell.Row.Index;
              for (int i = 0; i < ultData.Rows.Count; i++)
              {
                if (i != rowIndex)
                {
                  ultData.Rows[i].Cells["IsDefault"].Value = 0;
                }
              }
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// ultComboMaterialGroup Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboMaterialGroup_Leave(object sender, EventArgs e)
    {
      string value = string.Empty;
      try
      {
        value = ultComboMaterialGroup.Value.ToString().Trim();
      }
      catch { }
      if (value.Length == 0)
      {
        string text = ultComboMaterialGroup.Text.Trim();
        if (text.Length > 0 && text.Length != 3)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Material Group");
          ultComboMaterialGroup.Focus();
          return;
        }
        else if (text.Length > 0 && text.Length == 3)
        {
          string commandGroup = string.Format("SELECT COUNT(*) FROM TblGNRMaterialGroup WHERE [Group] = '{0}'", text);
          object objGroup = DataBaseAccess.ExecuteScalarCommandText(commandGroup);
          if ((objGroup == null) || (objGroup != null && (int)objGroup == 0))
          {
            WindowUtinity.ShowMessageError("ERR0011", "Material Group");
            ultComboMaterialGroup.Focus();
            return;
          }
        }
      }
    }

    /// <summary>
    /// ultComboMaterialCategory Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboMaterialCategory_Leave(object sender, EventArgs e)
    {
      string value = string.Empty;
      try
      {
        value = ultComboMaterialCategory.Value.ToString().Trim();
      }
      catch { }
      if (value.Length == 0)
      {
        string text = ultComboMaterialCategory.Text.Trim();
        string group = ultComboMaterialGroup.Text.ToString().Trim();
        if (text.Length > 0 && text.Length != 2)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Material Category");
          ultComboMaterialCategory.Focus();
          return;
        }
        else if (text.Length > 0 && text.Length == 2)
        {
          string commandCategory = string.Format("SELECT COUNT(*) FROM TblGNRMaterialCategory WHERE [Group] = '{0}' AND Category = '{1}'", group, text);
          object objCategory = DataBaseAccess.ExecuteScalarCommandText(commandCategory);
          if ((objCategory == null) || (objCategory != null && (int)objCategory == 0))
          {
            WindowUtinity.ShowMessageError("ERR0011", "Material Category");
            ultComboMaterialCategory.Focus();
            return;
          }
        }
      }
    }

    /// <summary>
    /// ultComboUnit Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboUnit_Leave(object sender, EventArgs e)
    {
      string text = ultComboUnit.Text.Trim();
      if (text.Length > 0)
      {
        string commandUnit = string.Format("SELECT COUNT(*) FROM TblGNRMaterialUnit WHERE Symbol = '{0}'", text);
        object objUnit = DataBaseAccess.ExecuteScalarCommandText(commandUnit);
        if ((objUnit == null) || (objUnit != null && (int)objUnit == 0))
        {
          WindowUtinity.ShowMessageError("ERR0011", "Material Unit");
          ultComboUnit.Focus();
          return;
        }
      }
    }

    /// <summary>
    /// ultComboFactoryUnit Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboFactoryUnit_Leave(object sender, EventArgs e)
    {
      string text = ultComboFactoryUnit.Text.Trim();
      if (text.Length > 0)
      {
        string commandFacUnit = string.Format("SELECT COUNT(*) FROM TblGNRMaterialUnit WHERE Symbol = '{0}'", text);
        object objFacUnit = DataBaseAccess.ExecuteScalarCommandText(commandFacUnit);
        if ((objFacUnit == null) || (objFacUnit != null && (int)objFacUnit == 0))
        {
          WindowUtinity.ShowMessageError("ERR0011", "Factory Unit");
          ultComboFactoryUnit.Focus();
          return;
        }
      }
    }

    /// <summary>
    /// ultComboDeptInCharge Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboDeptInCharge_Leave(object sender, EventArgs e)
    {
      string value = string.Empty;
      try
      {
        value = ultComboDeptInCharge.Value.ToString().Trim();
      }
      catch { }
      if (value.Length == 0)
      {
        string text = ultComboDeptInCharge.Text.Trim();
        if (text.Length > 0)
        {
          string commandDept = string.Format("SELECT COUNT(*) FROM VHRDDepartment WHERE Department = '{0}'", text);
          object objDept = DataBaseAccess.ExecuteScalarCommandText(commandDept);
          if ((objDept == null) || (objDept != null && (int)objDept == 0))
          {
            WindowUtinity.ShowMessageError("ERR0011", "Department");
            ultComboDeptInCharge.Focus();
            return;
          }
        }
      }
    }

    /// <summary>
    /// ultComboGroupInCharge Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboGroupInCharge_Leave(object sender, EventArgs e)
    {
      string text = ultComboGroupInCharge.Text.Trim();
      if (text.Length > 0)
      {
        string commandGroupStaff = string.Format("SELECT COUNT(*) FROM TblPURStaffGroup WHERE DeleteFlg = 0 AND GroupName = '{0}'", text);
        object objGroupStaff = DataBaseAccess.ExecuteScalarCommandText(commandGroupStaff);
        if ((objGroupStaff == null) || (objGroupStaff != null && (int)objGroupStaff == 0))
        {
          WindowUtinity.ShowMessageError("ERR0011", "Group Staff");
          ultComboGroupInCharge.Focus();
          return;
        }
      }
    }

    /// <summary>
    /// ultData Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      if (colName.CompareTo("supplierpid") == 0)
      {
        string supplierCode = e.Cell.Row.Cells["SupplierPid"].Text.Trim();
        string commandText = string.Format("SELECT COUNT(*) FROM TblPURSupplierInfo WHERE SupplierCode = '{0}'", supplierCode);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if ((obj == null) || (obj != null && (int)obj == 0))
        {
          WindowUtinity.ShowMessageError("ERR0011", "Supplier Code");
          e.Cancel = true;
        }
      }
    }

    /// <summary>
    /// click button Coefficient
    /// </summary>
    private void btnCoefficient_Click(object sender, EventArgs e)
    {
      viewPUR_01_010 view = new viewPUR_01_010();
      view.materialCode = txtGroup.Text.ToString() + "-" + txtCategory.Text.ToString() + "-" + txtCode.Text.ToString();
      Shared.Utility.WindowUtinity.ShowView(view, "Setup Material Formula", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Normal);
    }

    /// <summary>
    /// Change Local, Import
    /// Local = 1, Import = 2
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSource_ValueChanged(object sender, EventArgs e)
    {
      ultCBUseful.Value = null;

      if (ultSource.Value != null)
      {
        int source = (string.Compare(ultSource.Text, "Local", true) == 0) ? 1 : 2;
        this.LoadComboUseful(source);
      }
      else
      {
        this.LoadComboUseful(-1);
      }
    }

    /// <summary>
    /// Format Purchas Price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtPurchasPrice_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtPurchasPrice.Text);
      string numberRead = this.NumericFormat(number, 0);
      txtPurchasPrice.Text = numberRead;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_ClickCellButton(object sender, CellEventArgs e)
    {
      string value = e.Cell.Value.ToString();
      if (value == "Open File")
      {
        long pid = DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString());
        if (pid > 0)
        {
          viewPUR_01_013 uc = new viewPUR_01_013();
          uc.viewPid = pid;
          DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "Upload File", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
        }
        else
        {
          MessageBox.Show("Please save data before upload file", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }
      }
      //this.LoadData(this.materialCode);
    }

    private void ultData_InitializeRow(object sender, InitializeRowEventArgs e)
    {
      try
      {
        if (e.ReInitialize == false)
        {
          e.Row.Cells["Open"].Value = "Open File";
          e.Row.Cells["Open"].ButtonAppearance.ForeColor = Color.Blue;
          e.Row.Cells["Open"].Appearance.ForeColor = Color.Blue;
        }
      }
      catch
      {
      }
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      //bool selected = false;
      //try
      //{
      //  selected = ultData.Selected.Rows[0].Selected;
      //}
      //catch
      //{
      //  selected = false;
      //}
      //if (!selected)
      //{
      //  return;
      //}
      //UltraGridRow row = ultData.Selected.Rows[0];
      //Process prc = new Process();

      //if (DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) == int.MinValue)
      //{
      //  prc.StartInfo = new ProcessStartInfo(row.Cells["LocationFileLocal"].Value.ToString());
      //}
      //else
      //{
      //  string startupPath = System.Windows.Forms.Application.StartupPath;
      //  string folder = string.Format(@"{0}\Temporary", startupPath);
      //  if (!Directory.Exists(folder))
      //  {
      //    Directory.CreateDirectory(folder);
      //  }
      //  string locationFile = row.Cells["LocationFile"].Value.ToString();
      //  if (File.Exists(locationFile))
      //  {
      //    string newLocationFile = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(row.Cells["LocationFile"].Value.ToString()));
      //    if (File.Exists(newLocationFile))
      //    {
      //      try
      //      {
      //        File.Delete(newLocationFile);
      //      }
      //      catch
      //      {
      //        WindowUtinity.ShowMessageWarningFromText("File Is Opening!");
      //        return;
      //      }
      //    }
      //    File.Copy(locationFile, newLocationFile);
      //    prc.StartInfo = new ProcessStartInfo(newLocationFile);
      //  }
      //}
      //try
      //{
      //  prc.Start();
      //}
      //catch
      //{
      //  Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
      //}
    }
    #endregion Event

  }
}
