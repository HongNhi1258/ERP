/*
  Author      : Nguyen Van Tron
  Date        : 14/02/2012
  Description : Carcass Contract Out Detail
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
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Technical
{
  public partial class viewBOM_03_031 : MainUserControl
  {
    #region field
    public long carcassContractOutPid = long.MinValue;
    public string carcassCode = string.Empty;
    private IList listMaterialsDeletedPid;
    private IList listOtherDeletedPid;
    private bool bPur = false;
    private bool bPla = false;
    private bool bSelectMaterial = false;
    #endregion field

    #region function
    private void LoadData()
    {
      if (this.carcassCode.ToString().Trim().Length == 0)
      {
        chkSetDefault.Enabled = true;
        chkSetDefault.Checked = false;
      }
      this.listMaterialsDeletedPid = new ArrayList();
      this.listOtherDeletedPid = new ArrayList();
      ultraCBCarcass.ReadOnly = (this.carcassContractOutPid == long.MinValue ? false : true);
      ultraCBCarcass.Value = this.carcassCode;

      DBParameter[] inputParam = new DBParameter[2];
      if (this.carcassContractOutPid > 0)
      {
        inputParam[0] = new DBParameter("@CarcassContractOutPid", DbType.Int64, this.carcassContractOutPid);
      }
      inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMCarcassContractOut_Select", inputParam);
      if (dsSource != null)
      {
        if (dsSource.Tables[0].Rows.Count > 0)
        {
          string carcassCode = dsSource.Tables[0].Rows[0]["CarcassCode"].ToString();
          picCarcass.ImageLocation = FunctionUtility.BOMGetCarcassImage(carcassCode);
          string contractOutCode = dsSource.Tables[0].Rows[0]["ContractOutCode"].ToString();
          string supplierPid = dsSource.Tables[0].Rows[0]["SupplierPid"].ToString();
          int provideMaterials = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["ProvideMaterialsFlg"].ToString());
          int setDefault = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["Default"].ToString());
          ultraCBCarcass.Value = carcassCode;
          ultraCBContractOutCode.Value = contractOutCode;
          ultraCBSupplier.Value = supplierPid;
          chkProvideMaterials.Checked = (provideMaterials == 1 ? true : false);
          chkSetDefault.Checked = (setDefault == 1 ? true : false);
          txtWo.Text = dsSource.Tables[0].Rows[0]["WoPid"].ToString();
          txtItemQty.Text = dsSource.Tables[0].Rows[0]["ItemQty"].ToString();
          txtNetPrice.Text = dsSource.Tables[0].Rows[0]["NetPrice"].ToString();
          double number = DBConvert.ParseDouble(txtNetPrice.Text);
          string numberRead = this.NumericFormat(number, 0);
          this.txtNetPrice.Text = numberRead;

          if (DBConvert.ParseDateTime(dsSource.Tables[0].Rows[0]["DeliveryDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) > DateTime.MinValue)
          {
            ultdDelivery.Value = DBConvert.ParseDateTime(dsSource.Tables[0].Rows[0]["DeliveryDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }
          if (DBConvert.ParseDateTime(dsSource.Tables[0].Rows[0]["DCConfirmedDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) > DateTime.MinValue)
          {
            ultdDCConfirm.Value = DBConvert.ParseDateTime(dsSource.Tables[0].Rows[0]["DCConfirmedDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }
        }
        ultraGridMaterialsList.DataSource = dsSource.Tables[1];
        if (dsSource.Tables[2] == null || dsSource.Tables[2].Rows.Count == 0)
        {
          grpNotInBOMList.Visible = false;
          tableLayoutPanel6.ColumnStyles[0].SizeType = SizeType.Percent;
          tableLayoutPanel6.ColumnStyles[0].Width = 100;

        }
        else
        {
          grpNotInBOMList.Visible = true;
          tableLayoutPanel6.ColumnStyles[0].SizeType = SizeType.Percent;
          tableLayoutPanel6.ColumnStyles[0].Width = 50;
          tableLayoutPanel6.ColumnStyles[1].SizeType = SizeType.Percent;
          tableLayoutPanel6.ColumnStyles[1].Width = 50;
        }
        ultNotinBOM.DataSource = dsSource.Tables[2];
        ultOther.DataSource = dsSource.Tables[3];
        if (chkSetDefault.Enabled)
        {
          chkSetDefault.Enabled = !chkSetDefault.Checked;
        }
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
    private void LoadCarcassList()
    {
      string commandText = "Select CarcassCode, [Description], (CarcassCode + ' - ' + [Description]) DisplayText From TblBOMCarcass Where DeleteFlag = 0";
      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Shared.Utility.ControlUtility.LoadUltraCombo(ultraCBCarcass, dtCarcass, "CarcassCode", "DisplayText", "DisplayText");
    }

    private void LoadContractOutCode()
    {
      string commandText = string.Format(@"SELECT MaterialCode, MaterialNameEn, [StandardCost] Price, [TenTienTe] Unit, MaterialCode + ' - ' + MaterialNameEn DisplayText FROM VBOMMaterials");
      DataTable dtMaterials = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Shared.Utility.ControlUtility.LoadUltraCombo(ultraCBContractOutCode, dtMaterials, "MaterialCode", "DisplayText", "DisplayText");
    }

    private void LoadSupplier()
    {
      string commandText = string.Format(@"SELECT Pid, ID_NhaCC, TenNhaCCVN, TenNhaCCEN, (ID_NhaCC + ' - ' + ISNULL(TenNhaCCVN, '')) DisplayText FROM VWHFSupplier Order By ID_NhaCC");
      DataTable dtSupplier = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Shared.Utility.ControlUtility.LoadUltraCombo(ultraCBSupplier, dtSupplier, "Pid", "DisplayText", "DisplayText");
    }

    private void LoadMaterials()
    {
      string commandText = string.Format(@"SELECT MaterialCode, MaterialNameEn, StandardCost Price, TenTienTe Unit FROM VBOMMaterials");
      DataTable dtMaterials = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Shared.Utility.ControlUtility.LoadUltraDropDown(ultraDDMaterials, dtMaterials, "MaterialCode", "MaterialCode");
    }

    private bool CheckInvalid()
    {
      if (chkSetDefault.Checked && this.carcassContractOutPid < 0)
      {
        DBParameter[] inputParam = new DBParameter[3];
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        inputParam[0] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, ultraCBCarcass.Value);
        inputParam[1] = new DBParameter("@SupplierPid", DbType.Int64, ultraCBSupplier.Value);
        string strSupplierName = DataBaseAccess.SearchStoreProcedureDataTable("spBOMCheckingDefaultCarcassContractOut", inputParam, outputParam).Rows[0][0].ToString();
        long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result > 0)
        {
          if (WindowUtinity.ShowMessageConfirm("ERR0137", "Default " + strSupplierName + ", do you want to update?") == DialogResult.Yes)
          {
            inputParam[2] = new DBParameter("@OldSupplierPid", DbType.Int64, result);
            DataBaseAccess.ExecuteStoreProcedure("spBOMCheckingDefaultCarcassContractOut", inputParam, outputParam);
          }
          else
          {
            chkSetDefault.Checked = false;
          }

        }
      }
      if (ultraCBCarcass.Value == null || ultraCBCarcass.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Carcass");
        return false;
      }
      if (bPur)
      {
        //if (txtWo.Text.Trim().Length == 0)
        //{
        //  WindowUtinity.ShowMessageError("ERR0001", "Wo");
        //  return false;
        //}
        if (this.chkSetDefault.Checked)
        {
          if (ultraCBContractOutCode.Value == null || ultraCBContractOutCode.Value.ToString().Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Contract Out Code");
            return false;
          }
        }

        if (ultraCBSupplier.Value == null)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Supplier");
          return false;
        }
      }
      if (ultNotinBOM.Rows.Count > 0)
      {
        WindowUtinity.ShowMessageError("ERR0137", "Materials Not in BOM, Please Delete");
        return false;
      }
      //Check supplier existed
      DBParameter[] inputParamSub = new DBParameter[5];
      DBParameter[] outputParamSub = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      inputParamSub[0] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(txtWo.Text));
      inputParamSub[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, ultraCBCarcass.Value);
      inputParamSub[2] = new DBParameter("@SupplierPid", DbType.Int64, ultraCBSupplier.Value);
      inputParamSub[3] = new DBParameter("@CarcassContractOutPid", DbType.Int64, this.carcassContractOutPid);
      inputParamSub[4] = new DBParameter("@ContractOut", DbType.String, 16, ultraCBContractOutCode.Value.ToString());

      DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassContractOutCheckingSupplier_Select", inputParamSub, outputParamSub);
      long resultSupplier = DBConvert.ParseLong(outputParamSub[0].Value.ToString());
      if (resultSupplier <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0137", "Supplier");
        return false;
      }

      // Check Detail
      if (bPla)
      {
        if (!chkProvideMaterials.Checked && ultraGridMaterialsList.Rows.Count > 0
            && WindowUtinity.ShowMessageConfirmFromText("You unchecked \"Provide Materials\". All materials in Grid will be not registerred. Are you sure?") == DialogResult.No
          )
        {
          return false;
        }
        if (chkProvideMaterials.Checked)
        {
          bool result = true;
          bool isSelected = false;
          for (int i = 0; i < ultraGridMaterialsList.Rows.Count; i++)
          {
            if (ultraGridMaterialsList.Rows[i].Cells["Selected"].Value.ToString() == "1")
            {
              if (!isSelected)
              {
                isSelected = true;
              }
              // Check Material Code
              string materialCode = ultraGridMaterialsList.Rows[i].Cells["MaterialCode"].Value.ToString().Trim();
              if (materialCode.Length == 0)
              {
                WindowUtinity.ShowMessageError("ERR0001", string.Format("Material at row {0}", i + 1));
                return false;
              }
              // Check qty
              double qty = DBConvert.ParseDouble(ultraGridMaterialsList.Rows[i].Cells["Qty"].Value.ToString());
              double BOMqty = DBConvert.ParseDouble(ultraGridMaterialsList.Rows[i].Cells["BOM Qty"].Value.ToString());
              ultraGridMaterialsList.Rows[i].Cells["Qty"].Appearance.BackColor = Color.White;
              if (qty == double.MinValue)
              {
                ultraGridMaterialsList.Rows[i].Cells["Qty"].Appearance.BackColor = Color.Yellow;
                result = false;
              }
              if (qty > BOMqty)
              {
                ultraGridMaterialsList.Rows[i].Cells["Qty"].Appearance.BackColor = Color.Yellow;
                result = false;
              }
            }
          }
          if (!isSelected)
          {
            WindowUtinity.ShowMessageError("ERR0115", "Materials Supply");
            return false;
          }
          if (!result)
          {
            WindowUtinity.ShowMessageError("ERR0010", "Qty", "BOM Qty");
            return false;
          }
        }
      }
      return true;
    }

    private void SaveData()
    {
      if (this.CheckInvalid())
      {
        // 1. Save main data
        DBParameter[] inputParam = new DBParameter[12];
        DBParameter[] outputParam = new DBParameter[2];
        outputParam[0] = new DBParameter("@Result", DbType.Int32, 0);
        outputParam[1] = new DBParameter("@CarcassCodeNew", DbType.String, 16, string.Empty);
        if (carcassContractOutPid != long.MinValue)
        {
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, carcassContractOutPid);
        }
        inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, ultraCBCarcass.Value);
        inputParam[2] = new DBParameter("@SupplierPid", DbType.Int64, ultraCBSupplier.Value);
        inputParam[3] = new DBParameter("@ContractOut", DbType.AnsiString, 16, ultraCBContractOutCode.Value);
        inputParam[4] = new DBParameter("@ProvideMaterialsFlg", DbType.Int32, chkProvideMaterials.Checked ? 1 : 0);
        inputParam[5] = new DBParameter("@Default", DbType.Int32, chkSetDefault.Checked ? 1 : 0);
        inputParam[6] = new DBParameter("@AdjustBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        if (DBConvert.ParseLong(txtWo.Text) > 0)
        {
          inputParam[7] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(txtWo.Text));
        }
        if (DBConvert.ParseDouble(txtItemQty.Text) > 0)
        {
          inputParam[8] = new DBParameter("@ItemQty", DbType.Double, DBConvert.ParseDouble(txtItemQty.Text));
        }
        inputParam[9] = new DBParameter("@DeliveryDate", DbType.DateTime, ultdDelivery.Value);
        inputParam[10] = new DBParameter("@DCConfirmedDate", DbType.DateTime, ultdDCConfirm.Value);
        if (DBConvert.ParseDouble(txtNetPrice.Text) > 0)
        {
          inputParam[11] = new DBParameter("@NetPrice", DbType.Double, DBConvert.ParseDouble(txtNetPrice.Text));
        }
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassContractOutInfo_Edit", inputParam, outputParam);
        int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
        string carcassCodeNew = outputParam[1].Value.ToString();
        if (result > 0)
        {
          // 2. Save Data in Grid (Materials Provide)
          this.carcassContractOutPid = result;
          this.carcassCode = carcassCodeNew;
          bool success = true;
          if (chkProvideMaterials.Checked)
          {
            // 2.1. Delete Materials
            foreach (long pid in this.listMaterialsDeletedPid)
            {
              DBParameter[] inputDeleted = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
              DBParameter[] outputDeleted = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
              DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassContractOutDetail_Delete", inputDeleted, outputDeleted);
              result = DBConvert.ParseInt(outputDeleted[0].Value.ToString());
              if (result == 0)
              {
                success = false;
              }
            }
            // 2.2. Insert/Update Materials
            success = this.UpdateGrid(ultraGridMaterialsList);
            success = this.UpdateGrid(ultNotinBOM);

          }
          else
          {
            string commandDeleteAllMaterials = string.Format("Delete From TblBOMCarcassContractOutDetail Where CarcassContractOutPid = {0}", this.carcassContractOutPid);
            success = DataBaseAccess.ExecuteCommandText(commandDeleteAllMaterials);
          }
          // 2.1. Delete Other
          if (this.listOtherDeletedPid != null)
          {
            foreach (long pid in this.listOtherDeletedPid)
            {
              DBParameter[] inputDeleted = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
              DBParameter[] outputDeleted = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
              DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassContractOutDetailOther_Delete", inputDeleted, outputDeleted);
              result = DBConvert.ParseInt(outputDeleted[0].Value.ToString());
              if (result == 0)
              {
                success = false;
              }
            }
          }
          for (int i = 0; i < ultOther.Rows.Count; i++)
          {
            string EnDescription = ultOther.Rows[i].Cells["ENG Description"].Value.ToString();
            string VnDescription = ultOther.Rows[i].Cells["VN Description"].Value.ToString();
            double Price = DBConvert.ParseDouble(ultOther.Rows[i].Cells["Price"].Value.ToString());
            DBParameter[] outputDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DBParameter[] inputDetail = new DBParameter[5];
            long pid = DBConvert.ParseLong(ultOther.Rows[i].Cells["Pid"].Value.ToString());
            if (pid != long.MinValue)
            {
              inputDetail[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputDetail[1] = new DBParameter("@CarcassContractOutPid", DbType.Int64, this.carcassContractOutPid);
            inputDetail[2] = new DBParameter("@EnDescription", DbType.String, 256, EnDescription);
            inputDetail[3] = new DBParameter("@VnDescription", DbType.String, 256, VnDescription);
            inputDetail[4] = new DBParameter("@Price", DbType.Double, Price);
            DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassContractOutDetailOther_Edit", inputDetail, outputDetail);
            result = DBConvert.ParseInt(outputDetail[0].Value.ToString());
            if (result == 0)
            {
              success = false;
            }
          }
          if (success)
          {
            WindowUtinity.ShowMessageSuccess("MSG0004");
          }
          else
          {
            WindowUtinity.ShowMessageWarning("WRN0004");
          }
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
      }
    }

    private bool UpdateGrid(UltraGrid ultData)
    {
      bool returnState = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["Selected"].Value.ToString() == "1")
        {
          string materialCode = ultData.Rows[i].Cells["MaterialCode"].Value.ToString();
          double qty = DBConvert.ParseDouble(ultData.Rows[i].Cells["Qty"].Value.ToString());
          DBParameter[] outputDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DBParameter[] inputDetail = new DBParameter[4];
          long pid = DBConvert.ParseLong(ultData.Rows[i].Cells["Pid"].Value.ToString());
          if (pid != long.MinValue)
          {
            inputDetail[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          inputDetail[1] = new DBParameter("@CarcassContractOutPid", DbType.Int64, this.carcassContractOutPid);
          inputDetail[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
          inputDetail[3] = new DBParameter("@Qty", DbType.Double, qty);
          DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassContractOutDetail_Edit", inputDetail, outputDetail);
          long result = DBConvert.ParseLong(outputDetail[0].Value.ToString());
          if (result == 0)
          {
            returnState = false;
          }
        }
      }
      return returnState;
    }

    private void DefaultQty(UltraGrid ultData, bool selected)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (selected)
        {
          ultData.Rows[i].Cells["Qty"].Value = ultData.Rows[i].Cells["BOM Qty"].Value;
          ultData.Rows[i].Cells["Selected"].Value = 1;
          chkProvideMaterials.Checked = true;
          ultData.Rows[i].Cells["Qty"].Appearance.BackColor = Color.Yellow;
          ultData.Rows[i].Cells["Qty Price"].Appearance.BackColor = Color.Yellow;
        }
        else
        {
          ultData.Rows[i].Cells["Qty"].Value = DBNull.Value;
          ultData.Rows[i].Cells["Selected"].Value = 0;
          chkProvideMaterials.Checked = false;
          ultData.Rows[i].Cells["Qty"].Appearance.BackColor = Color.White;
          ultData.Rows[i].Cells["Qty Price"].Appearance.BackColor = Color.White;
        }
      }
    }

    #endregion function

    #region event
    public viewBOM_03_031()
    {
      InitializeComponent();
    }

    private void ultraCBCarcass_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns[0].MinWidth = 120;
      e.Layout.Bands[0].Columns[0].MaxWidth = 120;
    }

    private void ultraGridMaterialsList_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridMaterialsList);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = false; e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassContractOutPid"].Hidden = true;
      e.Layout.Bands[0].Columns["BOMQty"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["Price"].Header.Caption = "Standard Price";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ultraDDMaterials;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Price"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Price"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 80;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      if (bPla)
      {
        e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.LightBlue;
        e.Layout.Bands[0].Columns["Selected"].CellAppearance.BackColor = Color.LightBlue;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
        if (((UltraGrid)sender).Name == "ultraGridMaterialsList")
        {
          e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

        }
      }
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      for (int i = 0; i < ultraGridMaterialsList.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        Type colType = ultraGridMaterialsList.DisplayLayout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ultraGridMaterialsList.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      e.Layout.Bands[0].Columns["BOM Qty Price"].Header.Caption = "BOM\n Amount(VND)";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "SubCon Qty";
      e.Layout.Bands[0].Columns["Qty Price"].Header.Caption = "SubCon\n Amount(VND)";

      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["BOM Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Price"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Price"].Format = "###,###";
      e.Layout.Bands[0].Columns["BOM Qty Price"].Format = "###,###";
      e.Layout.Bands[0].Columns["Qty Price"].Format = "###,###";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["BOM Qty Price"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty Price"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = HAlign.Right;
    }

    private void ultraCBSupplier_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ID_NhaCC"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ID_NhaCC"].MaxWidth = 100;
    }

    private void viewBOM_03_031_Load(object sender, EventArgs e)
    {
      bPur = btnPUR.Visible;
      bPla = btnPLA.Visible;
      panel1.Visible = false;
      ultdDelivery.Value = DBNull.Value;
      ultdDCConfirm.Value = DBNull.Value;
      this.LoadCarcassList();
      this.LoadContractOutCode();
      this.LoadSupplier();
      this.LoadMaterials();
      this.LoadData();
      try
      {
        if (this.carcassCode.ToString().Trim().Length > 0)
        {
          ultraCBCarcass.Enabled = false;
          ultraCBSupplier.Enabled = false;
        }
        else
        {
          ultraCBCarcass.Enabled = true;

        }
      }
      catch
      {
        ultraCBCarcass.Enabled = false;
      }
      //if (bPla)
      //{
      //  this.HidePlanning(true);
      //}
      //else
      //{
      //  this.HidePlanning(false);
      //}
    }

    private void HidePlanning(bool hide)
    {
      ultraCBCarcass.ReadOnly = hide;
      ultraCBContractOutCode.ReadOnly = hide;
      //txtPrice.Enabled = !hide;
      txtNetPrice.Enabled = !hide;
      ultraCBSupplier.ReadOnly = hide;
      txtWo.ReadOnly = hide;
      txtItemQty.ReadOnly = hide;
      ultdDelivery.ReadOnly = hide;
      ultdDCConfirm.ReadOnly = hide;
      chkProvideMaterials.Enabled = hide;
      chkSetDefault.Enabled = !hide;
      chkDefaultBOM.Visible = hide;
      if (!hide)
      {
        chkSetDefault.Enabled = !chkSetDefault.Checked;
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultraGridMaterialsList_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listMaterialsDeletedPid.Add(pid);
        }
      }
    }

    private void chkProvideMaterials_CheckedChanged(object sender, EventArgs e)
    {
      //tableLayoutPanel6.Visible = chkProvideMaterials.Checked;
      //if (chkProvideMaterials.Checked)
      //{
      //  tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Percent;
      //  tableLayoutPanel1.RowStyles[1].Height = 100;
      //  tableLayoutPanel6.Visible = true;
      //  tableLayoutPanel1.RowStyles[2].SizeType = SizeType.Absolute;
      //  tableLayoutPanel1.RowStyles[2].Height = 150;
      //  grpOtherList.Visible = true;
      //}
      //else
      //{
      //  tableLayoutPanel1.RowStyles[1].SizeType = SizeType.AutoSize;
      //  tableLayoutPanel6.Visible = false;
      //  tableLayoutPanel1.RowStyles[2].SizeType = SizeType.Percent;
      //  tableLayoutPanel1.RowStyles[2].Height = 100;
      //  grpOtherList.Visible = true;

      //}
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraDDMaterials_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
    }

    private void ultraGridMaterialsList_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (string.Compare(colName, "MaterialCode", true) == 0)
      {
        string commandTestMaterialCode = string.Format("Select Count(MaterialCode) From VBOMMaterials Where MaterialCode = '{0}'", e.Cell.Text);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandTestMaterialCode);
        if (obj == null || DBConvert.ParseInt(obj.ToString()) == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Material Code");
          e.Cancel = true;
        }
      }
    }

    private void ultraGridMaterialsList_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (string.Compare(colName, "MaterialCode", true) == 0)
      {
        if (ultraDDMaterials.SelectedRow != null)
        {
          e.Cell.Row.Cells["MaterialName"].Value = ultraDDMaterials.SelectedRow.Cells["MaterialNameEn"].Value;
          e.Cell.Row.Cells["Price"].Value = ultraDDMaterials.SelectedRow.Cells["Price"].Value;
          e.Cell.Row.Cells["Unit"].Value = ultraDDMaterials.SelectedRow.Cells["Unit"].Value;
        }
      }
      else if (string.Compare(colName, "Qty", true) == 0)
      {
        if (bSelectMaterial)
        {
          return;
        }
        bSelectMaterial = true;
        e.Cell.Row.Cells["Selected"].Value = "1";
        if (DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString()) > 0)
        {
          e.Cell.Row.Cells["Qty Price"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
        }
        else
        {
          e.Cell.Row.Cells["Qty Price"].Value = DBNull.Value;
        }
        bSelectMaterial = false;

      }
      //else if (string.Compare(colName, "Selected", true) == 0)
      //{
      //  if (bSelectMaterial)
      //  {
      //    return;
      //  }
      //  bSelectMaterial = true;
      //  if (e.Cell.Row.Cells["Selected"].Value.ToString() == "1")
      //  {
      //    e.Cell.Row.Cells["Qty"].Value = e.Cell.Row.Cells["BOMQty"].Value;
      //    e.Cell.Row.Cells["Qty Price"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
      //    e.Cell.Row.Cells["Qty"].Appearance.BackColor = Color.Yellow;
      //    e.Cell.Row.Cells["Qty Price"].Appearance.BackColor = Color.Yellow;
      //  }
      //  else
      //  {
      //    e.Cell.Row.Cells["Qty"].Value = DBNull.Value;
      //    e.Cell.Row.Cells["Qty Price"].Value = DBNull.Value;
      //    e.Cell.Row.Cells["Qty"].Appearance.BackColor = Color.White;
      //    e.Cell.Row.Cells["Qty Price"].Appearance.BackColor = Color.White;
      //  }
      //  bSelectMaterial = false;
      //}
    }

    private void ultraCBContractOutCode_ValueChanged(object sender, EventArgs e)
    {
      if (ultraCBContractOutCode.SelectedRow != null)
      {
        txtPrice.Text = ultraCBContractOutCode.SelectedRow.Cells["Price"].Value.ToString();
        double number = DBConvert.ParseDouble(txtPrice.Text);
        string numberRead = this.NumericFormat(number, 0);
        this.txtPrice.Text = numberRead;
        txtUnit.Text = ultraCBContractOutCode.SelectedRow.Cells["Unit"].Value.ToString();
      }
      else
      {
        txtPrice.Text = string.Empty;
        txtUnit.Text = string.Empty;
      }
    }

    private void ultOther_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultOther);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = false;
      e.Layout.Bands[0].Columns["ENG Description"].Width = 300;
      e.Layout.Bands[0].Columns["VN Description"].Width = 300;
      e.Layout.Bands[0].Columns["Price"].Width = 100;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassContractOutPid"].Hidden = true;
      for (int i = 0; i < ultOther.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        Type colType = ultOther.DisplayLayout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ultOther.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      if (this.bPur)
      {
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      }
      else
      {
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }
      e.Layout.Bands[0].Columns["Price"].Format = "###,###";
    }

    private void ultOther_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listOtherDeletedPid.Add(pid);
        }
      }
    }

    private void chkDefaultBOM_CheckedChanged(object sender, EventArgs e)
    {
      this.DefaultQty(ultraGridMaterialsList, chkDefaultBOM.Checked);
    }

    private void txtNetPrice_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtNetPrice.Text);
      string numberRead = this.NumericFormat(number, 0);
      txtNetPrice.Text = numberRead;
    }

    private void chkSetDefault_CheckedChanged(object sender, EventArgs e)
    {
      if (bSelectMaterial)
      {
        return;
      }
      string strContractOut = "";
      try
      {
        strContractOut = ultraCBContractOutCode.Value.ToString().Trim();
      }
      catch
      {
        strContractOut = "";
      }
      if (strContractOut.Length <= 0 && chkSetDefault.Checked)
      {
        bSelectMaterial = true;
        WindowUtinity.ShowMessageError("ERR0011", "Contract Out");
        chkSetDefault.Checked = false;
        bSelectMaterial = false;
      }
    }

    private void ultraGridMaterialsList_CellChange(object sender, CellEventArgs e)
    {
      UltraGridRow row = e.Cell.Row;
      string colName = e.Cell.Column.ToString();
      if (string.Compare(colName, "Selected", true) == 0)
      {
        if (bSelectMaterial)
        {
          return;
        }
        bSelectMaterial = true;
        if (e.Cell.Row.Cells["Selected"].Text.ToString() == "1")
        {
          e.Cell.Row.Cells["Qty"].Value = e.Cell.Row.Cells["BOMQty"].Value;
          e.Cell.Row.Cells["Qty Price"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
          e.Cell.Row.Cells["Qty"].Appearance.BackColor = Color.Yellow;
          e.Cell.Row.Cells["Qty Price"].Appearance.BackColor = Color.Yellow;
        }
        else
        {
          e.Cell.Row.Cells["Qty"].Value = DBNull.Value;
          e.Cell.Row.Cells["Qty Price"].Value = DBNull.Value;
          e.Cell.Row.Cells["Qty"].Appearance.BackColor = Color.White;
          e.Cell.Row.Cells["Qty Price"].Appearance.BackColor = Color.White;
        }
        bSelectMaterial = false;

        // Check Choose Provice
        for (int i = 0; i < ultraGridMaterialsList.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultraGridMaterialsList.Rows[i].Cells["Selected"].Text) == 1)
          {
            chkProvideMaterials.Checked = true;
            return;
          }
          else
          {
            chkProvideMaterials.Checked = false;
          }
        }
      }
    }
    #endregion event
  }
}
