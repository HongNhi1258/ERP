/*
 * Author       : 
 * CreateDate   : 31/08/2012
 * Description  : Register Material Group For Allocate (Veneer And Wood)
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

namespace DaiCo.Purchasing
{
  public partial class viewPUR_01_011 : MainUserControl
  {
    #region Field
    private string imgPath = "J:\\Minh\\Image\\";
    public string materialGroup = string.Empty;
    public string materialCategory = string.Empty;
    public string materialCode = string.Empty;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool loadData = false;
    private bool canUpdate = true;
    private int status = int.MinValue;

    private string SP_GNR_MATERIALINFORMATION = "spGNRMaterialInformationDetailGroupAllocate_Select";
    private string SP_GNR_MATERIALINFO_INSERT = "spGNRMaterialInfo_Insert";
    private string SP_GNR_MATERIALINFO_UPDATE = "spGNRMaterialInfo_Update";
    private string SP_PUR_MATERIALSUPPLIER_EDIT = "spPURMaterialGroupAllocate_Insert";
    private string SP_PUR_MATERIALSUPPLIER_DELETE = "spPURMaterialGroupAllocate_Delete";
    #endregion Field

    #region Init
    /// <summary>
    /// Init Form
    /// </summary>
    public viewPUR_01_011()
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
      this.LoadComboMaterialGroup();
      if (this.materialGroup.Length > 0 && this.materialCategory.Length > 0 && this.materialCode.Length == 0)
      {
        string prefix = this.materialGroup + "-" + this.materialCategory;
        ultComboMaterialGroup.Value = this.materialGroup;
        ultComboMaterialGroup.Enabled = false;
        this.LoadComboMaterialCategory(this.materialGroup);
        ultComboMaterialCategory.Value = this.materialCategory;
        ultComboMaterialCategory.Enabled = false;
        string material = DataBaseAccess.ExecuteScalarCommandText(string.Format("SELECT dbo.FPURGetNewMaterialCode ('{0}')", prefix)).ToString();
        txtCode.Text = material.Split('-')[2];
      }
      else
      {
        ultComboMaterialGroup.Enabled = true;
        ultComboMaterialCategory.Enabled = true;
      }
      this.LoadComboBoxUnit(ultComboUnit);
      this.LoadComboBoxUnit(ultComboFactoryUnit);
      this.LoadDropDownMaterialCode();
      ControlUtility.LoadComboboxCodeMst(cmbColor, 7003);
      ControlUtility.LoadComboboxCodeMst(cmbMadeBy, 7004);

      this.LoadData(this.materialCode);
      this.loadData = true;
    }
    #endregion Init

    #region LoadData
    /// <summary>
    /// Load UltraDropDown MaterialCode
    /// </summary>
    private void LoadDropDownMaterialCode()
    {
      string commandText = string.Empty;
      commandText = " SELECT MAT.MaterialCode, MAT.NameEN, UNI.Symbol ";
      commandText = " FROM TblGNRMaterialInformation MAT ";
      commandText = "   INNER JOIN TblGNRMaterialUnit UNI ON MAT.Unit = UNI.Pid ";
      commandText = " WHERE MAT.[GroupIncharge] <> 100 ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultMaterialCode.DataSource = dtSource;
      ultMaterialCode.DisplayMember = "MaterialCode";
      ultMaterialCode.ValueMember = "MaterialCode";
      ultMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
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
    /// Load Data With Material Code
    /// </summary>
    /// <param name="materialCode"></param>
    private void LoadData(string materialCode)
    {
      if (materialCode.Length > 0)
      {
        picBoxMaterial.ImageLocation = this.imgPath + materialCode + ".wmf";
        ultComboMaterialGroup.Enabled = false;
        ultComboMaterialCategory.Enabled = false;
        txtCode.Enabled = false;
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

          ultComboUnit.Value = dtMain.Rows[0]["Unit"];
          ultComboMaterialGroup.Value = dtMain.Rows[0]["Group"];
          ultComboMaterialCategory.Value = dtMain.Rows[0]["Category"];
          ultComboFactoryUnit.Value = dtMain.Rows[0]["FactoryUnit"];

          string material = dtMain.Rows[0]["MaterialCode"].ToString().Trim();
          txtGroup.Text = material.Split('-')[0];
          txtCategory.Text = material.Split('-')[1];
          txtCode.Text = material.Split('-')[2];
          cmbColor.SelectedValue = dtMain.Rows[0]["Color"];
          cmbMadeBy.SelectedValue = dtMain.Rows[0]["MadeBy"];

          this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"].ToString());
          chkConfirm.Checked = (this.status > 0);
          if (chkConfirm.Checked)
          {
            this.SetStatusControl(Shared.Utility.SharedObject.UserInfo.UserPid, true, this.status);
          }
        }

        ultData.DataSource = dsSource.Tables[1];
      }
    }

    /// <summary>
    /// Set Status For Control
    /// </summary>
    /// <param name="empPid"></param>
    /// <param name="isEnable"></param>
    private void SetStatusControl(int empPid, bool isEnable, int status)
    {
      string commandText = "SELECT Manager FROM VHRDDepartmentInfo WHERE Code = 'PUR'";
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      int departmentManager = (int)obj;
      if (empPid != departmentManager)
      {
        ultComboMaterialGroup.Enabled = !isEnable;
        ultComboMaterialCategory.Enabled = !isEnable;
        txtCode.Enabled = !isEnable;
        txtNameEN.Enabled = !isEnable;
        txtNameVN.Enabled = !isEnable;
        ultComboUnit.Enabled = !isEnable;
        ultComboFactoryUnit.Enabled = !isEnable;
        chkConfirm.Enabled = !isEnable;
      }
      else
      {
        if (status == 1)
        {
          ultComboMaterialGroup.Enabled = !isEnable;
          ultComboMaterialCategory.Enabled = !isEnable;
          txtCode.Enabled = !isEnable;
          txtNameEN.Enabled = isEnable;
          txtNameVN.Enabled = isEnable;
          ultComboUnit.Enabled = isEnable;
          ultComboFactoryUnit.Enabled = isEnable;
          chkConfirm.Enabled = isEnable;
        }
        if (status == 2)
        {
          ultComboMaterialGroup.Enabled = !isEnable;
          ultComboMaterialCategory.Enabled = !isEnable;
          txtCode.Enabled = !isEnable;
          txtNameEN.Enabled = !isEnable;
          txtNameVN.Enabled = !isEnable;
          ultComboUnit.Enabled = !isEnable;
          chkConfirm.Enabled = !isEnable;
          ultComboFactoryUnit.Enabled = isEnable;
        }
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
      string group = ControlUtility.GetSelectedValueUltraCombobox(ultComboMaterialGroup);
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
      string category = ControlUtility.GetSelectedValueUltraCombobox(ultComboMaterialCategory);
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
      long unit = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultComboUnit));
      if (unit == long.MinValue)
      {
        WindowUtinity.ShowMessageError("MSG0011", "Unit");
        this.SaveSuccess = false;
        ultComboUnit.Focus();
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

      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    private string SaveData()
    {
      string storeName = string.Empty;
      string group = ControlUtility.GetSelectedValueUltraCombobox(ultComboMaterialGroup);
      string category = ControlUtility.GetSelectedValueUltraCombobox(ultComboMaterialCategory);
      string code = txtGroup.Text + "-" + txtCategory.Text + "-" + txtCode.Text.Trim();
      int color = DBConvert.ParseInt(ControlUtility.GetSelectedValueCombobox(cmbColor));
      string nameEN = txtNameEN.Text.Trim();
      string nameVN = txtNameVN.Text.Trim();
      long unit = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultComboUnit));
      long factoryUnit = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultComboFactoryUnit));
      double waste = DBConvert.ParseDouble(txtWaste.Text);
      string density = txtDensity.Text;
      string remark = txtOthers.Text.Trim();
      if (this.status == int.MinValue || this.status == 0)
      {
        this.status = (chkConfirm.Checked) ? 1 : 0;
      }
      else
      {
        this.status = (!chkConfirm.Checked) ? 0 : this.status;
      }
      string dimension = txtDimension.Text.Trim();
      int madeby = DBConvert.ParseInt(ControlUtility.GetSelectedValueCombobox(cmbMadeBy));

      DBParameter[] input = new DBParameter[17];
      input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, code);
      input[1] = new DBParameter("@NameEN", DbType.String, 256, nameEN);
      input[2] = new DBParameter("@NameVN", DbType.String, 256, nameVN);
      input[3] = new DBParameter("@Unit", DbType.Int64, unit);
      if (factoryUnit != long.MinValue)
      {
        input[4] = new DBParameter("@FactoryUnit", DbType.Int64, factoryUnit);
      }

      input[7] = new DBParameter("@GroupInCharge", DbType.Int64, 100);
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

      if (remark.Length > 0)
      {
        input[14] = new DBParameter("@Remark", DbType.String, 512, remark);
      }
      input[15] = new DBParameter("@Status", DbType.Int32, this.status);
      if (this.materialCode.Length > 0)
      {
        input[16] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = SP_GNR_MATERIALINFO_UPDATE;
      }
      else
      {
        input[5] = new DBParameter("@Group", DbType.AnsiString, 3, group);
        input[6] = new DBParameter("@Category", DbType.AnsiString, 2, category);
        input[16] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = SP_GNR_MATERIALINFO_INSERT;
      }

      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, input, output);
      string result = output[0].Value.ToString().Trim();
      return result;
    }

    /// <summary>
    /// Save Material
    /// </summary>
    /// <param name="material"></param>
    private void SaveMaterial(string material)
    {
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[2];
        inputDelete[0] = new DBParameter("@Pid", DbType.Int64, pidDelete);
        inputDelete[1] = new DBParameter("@DeleteBy", DbType.Int32, SharedObject.UserInfo.UserPid);

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

      DataTable dtSource = (DataTable)ultData.DataSource;
      if (dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (row.RowState == DataRowState.Added)
          {
            DBParameter[] input = new DBParameter[3];
            input[0] = new DBParameter("@MainMaterialCode", DbType.String, row["MainMaterialCode"].ToString());
            input[1] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
            input[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure(SP_PUR_MATERIALSUPPLIER_EDIT, input, output);
            long result = DBConvert.ParseLong(output[0].Value.ToString());
            if (result == 0)
            {
              WindowUtinity.ShowMessageError("ERR0004");
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
          this.SaveMaterial(this.materialCode);
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
          this.SaveSuccess = false;
          return;
        }
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
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ultMaterialCode;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Symbol"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.LightBlue;
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
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
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
      foreach (long pid in this.listDeletingPid)
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
      if (colName.CompareTo("supplierpid") == 0)
      {
        e.Cell.Row.Cells["EnglishName"].Value = ultMaterialCode.SelectedRow.Cells["EnglishName"].Value.ToString();
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
    /// ultData Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      if (colName.CompareTo("materialcode") == 0)
      {
        string materialCode = e.Cell.Row.Cells["MaterialCode"].Text.Trim();
        string commandText = string.Format("SELECT COUNT(*) FROM TblGNRMaterialInformation WHERE MaterialCode = '{0}'", materialCode);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if ((obj == null) || (obj != null && (int)obj == 0))
        {
          WindowUtinity.ShowMessageError("ERR0011", "Material Code");
          e.Cancel = true;
        }
      }
    }
    #endregion Event
  }
}
