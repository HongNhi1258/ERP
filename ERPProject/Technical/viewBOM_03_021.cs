/*
  Author      : 
  Date        : 12/08/2011
  Description : Create/Update Package
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;
namespace DaiCo.ERPProject
{
  public partial class viewBOM_03_021 : MainUserControl
  {
    #region Field
    private bool nonConfirm = true;
    private bool setDefault = false;
    private bool nonPacking = false;
    public string packageCode = string.Empty;
    private long pid = long.MinValue;
    private double totalCBM = long.MinValue;
    // Status Package
    private int status = int.MinValue;

    private DataTable dtSourceMaterials = new DataTable();

    //SectionCode
    private DataTable dtSectionCode = new DataTable();

    private bool canUpdate = false;
    private string itemCode = string.Empty;
    private int revision = int.MinValue;
    #endregion Field

    #region Init Data
    /// <summary>
    /// frmBOMPackageInfo
    /// </summary>
    public viewBOM_03_021()
    {
      InitializeComponent();
    }

    /// <summary>
    /// frmBOMPackageInfo_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewBOM_03_021_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + SharedObject.UserInfo.UserName + " | " + SharedObject.UserInfo.LoginDate;
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPAKPackageInitData");
      Utility.LoadUltraCombo(ucbddMaterial, ds.Tables[0], "MaterialCode", "DisplayText", true, new string[] { "DisplayText", "IDFactoryUnit", "Unit" });
      ucbddMaterial.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm";
      ucbddMaterial.DisplayLayout.Bands[0].Columns["MaterialName"].Header.Caption = "Tên tiếng anh";
      ucbddMaterial.DisplayLayout.Bands[0].Columns["MaterialNameVn"].Header.Caption = "Tên tiếng việt";
      ucbddMaterial.DisplayLayout.Bands[0].Columns["MaterialCode"].MinWidth = 100;
      ucbddMaterial.DisplayLayout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      Utility.LoadUltraCombo(ucbddAlternative, ds.Tables[0], "MaterialCode", "DisplayText", true, new string[] { "DisplayText", "IDFactoryUnit", "Unit" });
      Utility.LoadUltraDropDown(udrpSectionCode, ds.Tables[1], "Code", "Code", false);
      Utility.LoadUltraCombo(ucboItemCode, ds.Tables[2], "ItemCode", "Combo", false, new string[] { "ItemCode", "OldCode", "Name" });
      Utility.LoadUltraCombo(ucboRemark, ds.Tables[3], "Code", "Value", false, "Code");
      Utility.LoadUltraCombo(ucboReference, ds.Tables[4], "PackageCode", "Name", false, "PackageCode");
      lblDimension.Text = string.Empty;
      // Material From
      this.LoadComBoCopyMaterialFrom();
      // Material To
      this.LoadComBoCopyMaterialTo();
      this.LoadData();
    }

    private void LoadComBoCopyMaterialFrom()
    {
      string commandText = "SELECT Pid, BoxTypeCode FROM TblBOMBoxType ORDER BY Pid DESC";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ucboFrom.DataSource = dtSource;
      ucboFrom.DisplayMember = "BoxTypeCode";
      ucboFrom.ValueMember = "Pid";
      ucboFrom.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ucboFrom.DisplayLayout.Bands[0].Columns["BoxTypeCode"].Width = 350;
      ucboFrom.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    private void LoadComBoCopyMaterialTo()
    {
      if (this.packageCode.Length > 0)
      {
        string commandText = string.Empty;
        commandText += " SELECT BT.Pid, BT.BoxTypeCode ";
        commandText += " FROM TblBOMPackage PAK ";
        commandText += " 	INNER JOIN TblBOMBoxType BT ON PAK.Pid = BT.PackagePid ";
        commandText += " WHERE PAK.PackageCode = '" + this.packageCode + "'";
        commandText += " ORDER BY BT.Pid DESC ";
        System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

        ucboTo.DataSource = dtSource;
        ucboTo.DisplayMember = "BoxTypeCode";
        ucboTo.ValueMember = "Pid";
        ucboTo.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ucboTo.DisplayLayout.Bands[0].Columns["BoxTypeCode"].Width = 350;
        ucboTo.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      }
    }

    ///// <summary>
    ///// Load Combo Reference
    ///// </summary>
    //private void LoadReference(string carCassCode, string packageCode)
    //{
    //	string commandText = string.Empty;
    //	//commandText += " SELECT PK.PackageCode + '_' + PK.ItemCode + '-' + CONVERT(varchar(2), PK.Revision) + '_' ";
    //	commandText += " SELECT PK.ItemCode + '-' + CONVERT(varchar(2), PK.Revision) + '_' + PK.PackageCode + '_' ";
    //	commandText += " 			+ CONVERT(varchar(2), PK.QuantityItem) + '/' + CONVERT(varchar(2), PK.QuantityBox) Name, ";
    //	commandText += " 			PK.PackageCode PackageCode ";
    //	commandText += " FROM TblBOMPackage PK ";
    //	commandText += " INNER JOIN TblBOMItemInfo BII ON (PK.ItemCode = BII.ItemCode) AND (PK.Revision = BII.Revision) ";
    //	//commandText += " WHERE BII.CarcassCode = '" + carCassCode + "'";
    //	if (packageCode.Length > 0)
    //	{
    //		commandText += " AND PK.PackageCode != '" + packageCode + "'";
    //	}
    //	//commandText += " AND PK.Confirm = 1 ";

    //	DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
    //	Utility.LoadUltraCombo(ucboReference, dtSource, "PackageCode", "Name", false, "PackageCode");
    //}

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PackageCode", DbType.AnsiString, 16, this.packageCode) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMPackageInfomationByPackageCode", inputParam);
      DataTable dtPackageInfo = dsSource.Tables[0];

      if (dtPackageInfo.Rows.Count > 0)
      {
        DataRow row = dtPackageInfo.Rows[0];
        this.pid = DBConvert.ParseLong(row["Pid"].ToString());
        txtPackageCode.Text = this.packageCode;
        string itemImage = row["ItemCode"].ToString() + "-" + row["Revision"].ToString().PadLeft(2, '0');
        picPackage.ImageLocation = FunctionUtility.BOMGetPackageImage(itemImage);
        txtPackageName.Text = row["PackageName"].ToString();
        this.ucboItemCode.Value = row["ItemCode"].ToString() + " | " + row["Revision"].ToString();

        this.itemCode = row["ItemCode"].ToString();
        this.revision = DBConvert.ParseInt(row["Revision"].ToString());

        this.txtItem.Text = row["QuantityItem"].ToString();
        this.txtBox.Text = row["QuantityBox"].ToString();
        this.nonConfirm = (DBConvert.ParseInt(row["Confirm"].ToString()) == 0);

        // Status
        this.status = DBConvert.ParseInt(row["Confirm"].ToString());
        chkDefault.Checked = DBConvert.ParseInt(row["SetDefault"].ToString()) == 1 ? true : false;

        if (DBConvert.ParseInt(row["SetDefault"].ToString()) == 1)
        {
          this.setDefault = true;
        }

        if (DBConvert.ParseInt(row["Confirm"].ToString()) == 1)
        {
          this.chkLock.Checked = true;
        }
      }
      else
      {
        DataTable dtPackage = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FBOMGetNewPackageCode('PK') NewPackageCode");
        if ((dtPackage != null) && (dtPackage.Rows.Count > 0))
        {
          txtPackageCode.Text = dtPackage.Rows[0]["NewPackageCode"].ToString();
          txtPackageName.Text = dtPackage.Rows[0]["NewPackageCode"].ToString();
          this.packageCode = txtPackageCode.Text;
        }
      }
      this.NeedToSave = false;
      this.SetStatusControl();
      this.LoadDataPackageComponent(dsSource);
      //this.setDefault = false;
    }

    /// <summary>
    /// SetStatusControl
    /// </summary>
    private void SetStatusControl()
    {
      this.canUpdate = this.status >= 1 ? false : true;
      this.txtPackageCode.ReadOnly = true;
      this.txtPackageName.ReadOnly = this.canUpdate;
      this.ucboItemCode.Enabled = this.canUpdate;
      this.ucboReference.Enabled = this.canUpdate;
      this.btnCopy.Enabled = this.canUpdate;
      if (this.pid > 0)
      {
        chkDefault.Enabled = false;
        this.txtBox.ReadOnly = true;
        this.txtItem.ReadOnly = true;
      }
      if (chkLock.Checked)
      {
        chkLock.Enabled = false;
        btnCopyMaterial.Enabled = false;
        ucboFrom.Enabled = false;
        ucboTo.Enabled = false;
        this.btnPrint.Enabled = true;
      }
      else
      {
        this.btnPrint.Enabled = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      if (!this.CheckAndSaveData())
      {
        this.SaveSuccess = false;
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

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    /// <summary>
    /// ChangePackageInfo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ChangePackageInfo(object sender, EventArgs e)
    {
      this.SetNeedToSave();
      int num = 0;
      if (int.TryParse(this.txtItem.Text, out num))
      {
        int countComponent = ultComponent.Rows.Count;

        //check number box
        for (int i = 0; i < countComponent; i++)
        {
          UltraGridRow rowCheck = ultComponent.Rows[i];

          int j = this.ucboItemCode.Text.IndexOf('|');
          string text = this.ucboItemCode.Text.Substring(j + 1).Trim().ToString();

          int k = this.ucboItemCode.Value.ToString().IndexOf('|');

          if (DBConvert.ParseInt(this.txtBox.Text) > 1)
          {
            rowCheck.Cells["BoxCode"].Value = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + text.PadLeft(2, '0') + "_" + rowCheck.Cells["No"].Value.ToString().Trim() + "/" + this.txtBox.Text.Trim();
          }
          else if (DBConvert.ParseInt(this.txtBox.Text) == 1)
          {
            rowCheck.Cells["BoxCode"].Value = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + text.PadLeft(2, '0') + "_" + this.txtItem.Text.Trim() + "/" + this.txtBox.Text.Trim();
          }
        }
      }
    }
    #endregion Init Data

    #region ProcessData
    /// <summary>
    /// SaveBeforeClosing
    /// </summary>
    /// <returns></returns>
    private DialogResult SaveBeforeClosing()
    {
      DialogResult result = FunctionUtility.SaveBeforeClosing();
      if (result == DialogResult.Yes)
      {
        string message;
        bool success = this.CheckValidPackageInfo(out message);
        if (!success)
        {
          WindowUtinity.ShowMessageErrorFromText(message);
          return DialogResult.Cancel;
        }

        int countComponent = this.ultComponent.Rows.Count;
        if (countComponent < Convert.ToInt32(this.txtBox.Text))
        {
          if (this.chkLock.Checked == true)
          {
            WindowUtinity.ShowMessageError("WRN0006");
            return DialogResult.Cancel;
          }
          message = "Only Have " + countComponent.ToString() + " register ";
          if (WindowUtinity.ShowMessageConfirmFromText(message) == DialogResult.No)
          {
            return DialogResult.Cancel;
          }
        }

        success = this.CheckValidBoxTypeInfo(out message);
        {
          if (!success)
          {
            WindowUtinity.ShowMessageErrorFromText(message);
            return DialogResult.Cancel;
          }
        }

        success = this.SaveData();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          FunctionUtility.UnlockBOMCarcass(this.packageCode);
          WindowUtinity.ShowMessageError("ERR0005");
          return DialogResult.Cancel;
        }
      }
      return result;
    }

    /// <summary>
    /// LoadDataPackageComponent
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataPackageComponent(DataSet dsSource)
    {
      DataSet dsData = CreateDataSet.BoxTypeInfo();
      DataSet dsDirectLabour = CreateDataSet.DirectLabourInfo();
      try
      {
        dsData.Tables["dtBoxType"].Merge(dsSource.Tables[1]);
      }
      catch
      {
      }
      try
      {
        dsData.Tables["dtBoxTypeDetail"].Merge(dsSource.Tables[2]);
      }
      catch
      {
      }
      try
      {
        dsDirectLabour.Tables["dtDirectLabour"].Merge(dsSource.Tables[3]);
      }
      catch
      {
      }
      ultComponent.DataSource = dsData;
      ultDirectLabour.DataSource = dsDirectLabour;

      int count = ultComponent.Rows.Count;

      if (!this.canUpdate)
      {
        for (int j = 0; j < ultDirectLabour.Rows.Count; j++)
        {
          UltraGridRow row = ultDirectLabour.Rows[j];
          row.Activation = Activation.ActivateOnly;
        }

        for (int i = 0; i < count; i++)
        {
          UltraGridRow row = ultComponent.Rows[i];
          row.Cells["BoxCode"].Activation = Activation.ActivateOnly;
          row.Cells["No"].Activation = Activation.ActivateOnly;
          row.Cells["BoxName"].Activation = Activation.ActivateOnly;
          row.Cells["Length"].Activation = Activation.AllowEdit;
          row.Cells["Width"].Activation = Activation.AllowEdit;
          row.Cells["Height"].Activation = Activation.AllowEdit;
          row.Cells["GWeight"].Activation = Activation.AllowEdit;
          row.Cells["NWeight"].Activation = Activation.AllowEdit;
          row.Cells["CheckWeight"].Activation = Activation.AllowEdit;
          int countDetail = row.ChildBands[0].Rows.Count;
          for (int j = 0; j < countDetail; j++)
          {
            if (DBConvert.ParseDouble(row.ChildBands[0].Rows[j].Cells["RAW_Length"].Value.ToString()) == Double.MinValue)
            {
              row.ChildBands[0].Rows[j].Cells["RAW_Length"].Value = DBNull.Value;
            }
            if (DBConvert.ParseDouble(row.ChildBands[0].Rows[j].Cells["RAW_Width"].Value.ToString()) == Double.MinValue)
            {
              row.ChildBands[0].Rows[j].Cells["RAW_Width"].Value = DBNull.Value;
            }
            if (DBConvert.ParseDouble(row.ChildBands[0].Rows[j].Cells["RAW_Thickness"].Value.ToString()) == Double.MinValue)
            {
              row.ChildBands[0].Rows[j].Cells["RAW_Thickness"].Value = DBNull.Value;
            }
            if (DBConvert.ParseDouble(row.ChildBands[0].Rows[j].Cells["QTy"].Value.ToString()) == Double.MinValue)
            {
              row.ChildBands[0].Rows[j].Cells["QTy"].Value = DBNull.Value;
            }
            if (DBConvert.ParseDouble(row.ChildBands[0].Rows[j].Cells["TotalQty"].Value.ToString()) == Double.MinValue)
            {
              row.ChildBands[0].Rows[j].Cells["TotalQty"].Value = DBNull.Value;
            }
            if (DBConvert.ParseDouble(row.ChildBands[0].Rows[j].Cells["Waste"].Value.ToString()) == Double.MinValue)
            {
              row.ChildBands[0].Rows[j].Cells["Waste"].Value = DBNull.Value;
            }
          }
        }
      }
      else
      {
        for (int i = 0; i < count; i++)
        {
          UltraGridRow row = ultComponent.Rows[i];
          int countDetail = row.ChildBands[0].Rows.Count;
          for (int j = 0; j < countDetail; j++)
          {
            int factoryUnit = DBConvert.ParseInt(row.ChildBands[0].Rows[j].Cells["IDFactoryUnit"].Value.ToString());
            if (factoryUnit == 4)
            {
              row.ChildBands[0].Rows[j].Cells["RAW_Length"].Activation = Activation.AllowEdit;
              row.ChildBands[0].Rows[j].Cells["RAW_Width"].Activation = Activation.ActivateOnly;
              row.ChildBands[0].Rows[j].Cells["RAW_Width"].Value = DBNull.Value;
              row.ChildBands[0].Rows[j].Cells["RAW_Thickness"].Activation = Activation.ActivateOnly;
              row.ChildBands[0].Rows[j].Cells["RAW_Thickness"].Value = DBNull.Value;
            }
            else if (factoryUnit == 6)
            {
              row.ChildBands[0].Rows[j].Cells["RAW_Length"].Activation = Activation.AllowEdit;
              row.ChildBands[0].Rows[j].Cells["RAW_Width"].Activation = Activation.AllowEdit;
              row.ChildBands[0].Rows[j].Cells["RAW_Thickness"].Activation = Activation.ActivateOnly;
              row.ChildBands[0].Rows[j].Cells["RAW_Thickness"].Value = DBNull.Value;
            }
            else if (factoryUnit == 7)
            {
              row.ChildBands[0].Rows[j].Cells["RAW_Length"].Activation = Activation.AllowEdit;
              row.ChildBands[0].Rows[j].Cells["RAW_Width"].Activation = Activation.AllowEdit;
              row.ChildBands[0].Rows[j].Cells["RAW_Thickness"].Activation = Activation.AllowEdit;
            }
            else
            {
              row.ChildBands[0].Rows[j].Cells["RAW_Length"].Activation = Activation.ActivateOnly;
              row.ChildBands[0].Rows[j].Cells["RAW_Length"].Value = DBNull.Value;
              row.ChildBands[0].Rows[j].Cells["RAW_Width"].Activation = Activation.ActivateOnly;
              row.ChildBands[0].Rows[j].Cells["RAW_Width"].Value = DBNull.Value;
              row.ChildBands[0].Rows[j].Cells["RAW_Thickness"].Activation = Activation.ActivateOnly;
              row.ChildBands[0].Rows[j].Cells["RAW_Thickness"].Value = DBNull.Value;
            }

            // Set Total Qty
            //this.SetTotalQty(row.ChildBands[0].Rows[j], factoryUnit);
          }
        }
      }
    }

    /// <summary>
    /// CheckValidBoxTypeInfo
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidBoxTypeInfo(out string message)
    {
      message = string.Empty;
      int countComponent = ultComponent.Rows.Count;

      //check number box
      for (int j = 0; j < countComponent; j++)
      {
        UltraGridRow rowCheck = ultComponent.Rows[j];
        Boolean flagCheck = false;

        for (int k = 1; k <= Convert.ToInt32(txtBox.Text); k++)
        {
          if (rowCheck.Cells["No"].Value.ToString().Trim().Length > 0)
          {
            if (Convert.ToInt32(rowCheck.Cells["No"].Value.ToString().Trim()) == k)
            {
              flagCheck = true;
            }
          }
          else
          {
            break;
          }
        }

        if (flagCheck == false)
        {
          message = "No " + rowCheck.Cells["No"].Value.ToString().Trim() + " is not correct format!!";
          return false;
        }

        int count = 0;

        for (int m = 0; m < countComponent; m++)
        {
          UltraGridRow rowDupplicate = ultComponent.Rows[m];

          if (Convert.ToInt32(rowCheck.Cells["No"].Value.ToString().Trim()) == Convert.ToInt32(rowDupplicate.Cells["No"].Value.ToString().Trim()))
          {
            count++;
          }
        }
        if (count > 1)
        {
          message = "No " + rowCheck.Cells["No"].Value.ToString().Trim() + " is dupplicated!!";
          return false;
        }
        // Check barcode
        string barcode = rowCheck.Cells["Barcode"].Value.ToString().Trim();
        if (barcode.Length > 0)
        {
          long pid = DBConvert.ParseLong(rowCheck.Cells["Pid"].Value);
          DataTable dtBarcode = DataBaseAccess.SearchCommandTextDataTable(string.Format("SELECT * FROM TblBOMBoxType WHERE Barcode = '{0}' AND Pid <> {1}", barcode, pid));
          if (dtBarcode.Rows.Count > 0)
          {
            message = string.Format("Barcode {0} is already in use. Please input another barcode", barcode);
          }
        }
      }

      DataSet dsCheckUnit = (DataSet)ultComponent.DataSource;
      DataTable dtCheckUnit = dsCheckUnit.Tables["dtBoxTypeDetail"];
      foreach (DataRow row in dtCheckUnit.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          int unitCode = DBConvert.ParseInt(row["IDFactoryUnit"].ToString());
          string componentCode = row["MaterialCode"].ToString();

          if (row["Qty"].ToString().Length == 0)
          {
            message = string.Format(FunctionUtility.GetMessage("MSG0005"), string.Format("Qty of {0}", componentCode));
            return false;
          }

          if (DBConvert.ParseDouble(row["Qty"].ToString()) <= 0)
          {
            message = string.Format(FunctionUtility.GetMessage("MSG0005"), string.Format("Qty of {0}", componentCode));
            return false;
          }

          if (unitCode == 4)
          { // m
            double length = DBConvert.ParseDouble(row["RAW_Length"].ToString());
            if (length == double.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("MSG0005"), string.Format("Length of {0}", componentCode));
              return false;
            }
          }
          else if (unitCode == 6)
          { // sqm
            double length = DBConvert.ParseDouble(row["RAW_Length"].ToString());
            if (length == double.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("MSG0005"), string.Format("Length of {0}", componentCode));
              return false;
            }
            double width = DBConvert.ParseDouble(row["RAW_Width"].ToString());
            if (width == double.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("MSG0005"), string.Format("Width of {0}", componentCode));
              return false;
            }
          }
          else if (unitCode == 7)
          { // cbm
            double length = DBConvert.ParseDouble(row["RAW_Length"].ToString());
            if (length == double.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("MSG0005"), string.Format("Length of {0}", componentCode));
              return false;
            }
            double width = DBConvert.ParseDouble(row["RAW_Width"].ToString());
            if (width == double.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("MSG0005"), string.Format("Width of {0}", componentCode));
              return false;
            }
            double thickness = DBConvert.ParseDouble(row["RAW_Thickness"].ToString());
            if (thickness == double.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("MSG0005"), string.Format("Thickness of {0}", componentCode));
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// CheckValidDirectLabourInfo
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidDirectLabourInfo()
    {
      bool result = true;
      int countDirectLabour = ultDirectLabour.Rows.Count;

      //check number box
      for (int j = 0; j < countDirectLabour; j++)
      {
        UltraGridRow rowCheck = ultDirectLabour.Rows[j];
        if (rowCheck.Cells["SectionCode"].Value == DBNull.Value || rowCheck.Cells["Qty"].Value == DBNull.Value)
        {
          result = false;
          break;
        }
      }
      return result;
    }

    /// <summary>
    /// CheckValidPackageInfo
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPackageInfo(out string message)
    {
      message = string.Empty;
      //packageName
      string packageName = txtPackageName.Text.Trim();
      if (packageName.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The Package Name");
        return false;
      }

      //itemCode
      int k = this.ucboItemCode.Value.ToString().IndexOf('|');
      string itemCode = this.ucboItemCode.Value.ToString().Substring(0, k).Trim();
      if (itemCode.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The Item Code");
        return false;
      }

      if (this.ucboItemCode.Text.Trim().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The Item Code");
        return false;
      }

      //quantity item
      string quantityItem = this.txtItem.Text.Trim();
      if (quantityItem.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The Quantity Item");
        return false;
      }
      int num = 0;
      if (!int.TryParse(this.txtItem.Text, out num))
      {
        message = "The Quantity Item  must be number";
        this.txtItem.SelectAll();
        this.txtItem.Focus();
      }

      //quantity box
      string quantityBox = this.txtBox.Text.Trim();
      if (quantityBox.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The Quantity Box");
        return false;
      }

      if (!int.TryParse(this.txtBox.Text, out num))
      {
        message = "The Quantity Box  must be number";
        this.txtBox.SelectAll();
        this.txtBox.Focus();
        return false;
      }

      int item = Convert.ToInt32(this.txtItem.Text);
      int box = Convert.ToInt32(this.txtBox.Text);

      if (box != 1 && item != 1)
      {
        message = "Data Input Quantity Box And Quantity Item Error!!";
        this.txtBox.SelectAll();
        this.txtBox.Focus();
        return false;
      }

      string commandText = string.Empty;
      DataTable dt = new DataTable();
      int indexOf = int.MinValue;
      int revision = int.MinValue;

      indexOf = this.ucboItemCode.Text.ToString().Trim().IndexOf('|');
      revision = Convert.ToInt32(this.ucboItemCode.Text.ToString().Trim().Substring(indexOf + 2, 1).Trim());

      //commandText = "Select count(*) From TblBOMPackage Where ISNULL(IsDeleted, 0) = 0 AND ItemCode = '" + this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "'"
      //						  + " AND Revision = " + revision + " AND QuantityItem = " + Convert.ToInt32(this.txtItem.Text) + " AND QuantityBox = " + Convert.ToInt32(this.txtBox.Text);

      commandText = string.Format(@"SELECT COUNT(*) 
                                    FROM TblBOMPackage 
                                    WHERE ISNULL(IsDeleted, 0) = 0 
                                      AND ItemCode = '{0}'
                                      AND Revision = {1}
                                      AND QuantityItem = {2}
                                      AND QuantityBox = {3}
                                  ", this.ucboItemCode.Value.ToString().Substring(0, k).Trim(),
                                      revision,
                                      Convert.ToInt32(this.txtItem.Text),
                                      Convert.ToInt32(this.txtBox.Text));

      dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (this.packageCode != string.Empty)
      {
        commandText = string.Format(@"SELECT PackageCode 
                                    FROM TblBOMPackage 
                                    WHERE ISNULL(IsDeleted, 0) = 0 
                                      AND ItemCode = '{0}'
                                      AND Revision = {1}
                                      AND QuantityItem = {2}
                                      AND QuantityBox = {3}
                                  ", this.ucboItemCode.Value.ToString().Substring(0, k).Trim(),
                                      revision,
                                      Convert.ToInt32(this.txtItem.Text),
                                      Convert.ToInt32(this.txtBox.Text));

        DataTable dtTmp = DataBaseAccess.SearchCommandTextDataTable(commandText, null);
        if (dtTmp.Rows.Count > 0)
        {
          if (this.packageCode == dtTmp.Rows[0]["PackageCode"].ToString())
          {
            if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) > 1)
            {
              message = "This package has existed in database!!!";
              this.txtPackageName.SelectAll();
              this.txtPackageName.Focus();
              return false;
            }
          }
          else
          {
            if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) > 0)
            {
              message = "This package has existed in database!!!";
              this.txtPackageName.SelectAll();
              this.txtPackageName.Focus();
              return false;
            }
          }
        }
      }
      else
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The Package code");
        return false;
      }
      //Check standard
      if (chkDefault.Checked && this.setDefault == false)
      {
        commandText = string.Format(@"SELECT tbi.PackageCode
                                FROM TblBOMItemInfo AS tbi
                                LEFT JOIN TblBOMPackage AS tbp ON tbi.PackageCode = tbp.PackageCode
                                WHERE tbi.ItemCode = '{0}'
                                  AND tbi.Revision = {1}
                                  AND tbp.SetDefault = 1
                               ", this.ucboItemCode.Value.ToString().Substring(0, k).Trim(),
                                                              revision);
        DataTable dtStandard = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtStandard.Rows.Count > 0)
        {
          message = string.Format(@"Item {0} has existed standard package {1}!!!", this.ucboItemCode.Value.ToString().Substring(0, k).Trim(),
                                                                                  dtStandard.Rows[0]["PackageCode"].ToString());
          return false;
        }
      }
      return true;
    }

    /// <summary>
		/// Check and save data
		/// </summary>
		/// <returns></returns>
		private bool CheckAndSaveData()
    {
      string message = string.Empty;

      bool success = this.CheckValidPackageInfo(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return false;
      }

      success = this.CheckValidBoxTypeInfo(out message);
      {
        if (!success)
        {
          WindowUtinity.ShowMessageErrorFromText(message);
          return false;
        }
      }

      if (!CheckValidDirectLabourInfo())
      {
        WindowUtinity.ShowMessageError("ERR0001", new string[] { "Qty DirectLabour" });
        return false;
      }

      int countComponent = this.ultComponent.Rows.Count;
      if (countComponent < Convert.ToInt32(this.txtBox.Text))
      {
        if (this.chkLock.Checked == true)
        {
          message = "Register have not enough box !!!";
          WindowUtinity.ShowMessageErrorFromText(message);
          return false;
        }

        message = "Only Have " + countComponent.ToString() + " register ";
        if (WindowUtinity.ShowMessageConfirmFromText(message) == DialogResult.No)
        {
          return false;
        }
      }

      int indexOf = this.ucboItemCode.Text.ToString().Trim().IndexOf('|');
      int revision = Convert.ToInt32(this.ucboItemCode.Text.ToString().Trim().Substring(indexOf + 2, 1).Trim());
      int k = this.ucboItemCode.Value.ToString().IndexOf('|');
      string commandText = string.Format(@"SELECT tbi.PackageCode
                                FROM TblBOMItemInfo AS tbi
                                LEFT JOIN TblBOMPackage AS tbp ON tbi.PackageCode = tbp.PackageCode
                                WHERE tbi.ItemCode = '{0}'
                                  AND tbi.Revision = {1}
                                  AND tbp.SetDefault = 1
                               ", this.ucboItemCode.Value.ToString().Substring(0, k).Trim(),
                                                            revision);
      DataTable dtStandard = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (!chkDefault.Checked)
      {
        if (dtStandard.Rows.Count <= 0 || dtStandard.Rows[0][0].ToString().Trim().Length <= 0)
        {
          string temp = string.Format(@" have't standard package. Do you want to set package {0} for item {1}?",
                                  this.packageCode, this.ucboItemCode.Value.ToString().Substring(0, k).Trim());
          DialogResult result = MessageBox.Show(this.ucboItemCode.Text.ToString() + temp, "Save Package", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
          if (result == DialogResult.Yes)
          {
            chkDefault.Checked = true;
            success = this.SaveData();
            if (success)
            {
              this.SetPackageDefault();
            }
          }
          else
          {
            success = this.SaveData();
          }
        }
        else
        {
          success = this.SaveData();
        }
      }
      else
      {
        success = this.SaveData();
        if (dtStandard.Rows.Count <= 0 && success)
        {
          this.SetPackageDefault();
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        FunctionUtility.UnlockBOMCarcass(this.packageCode);
        WindowUtinity.ShowMessageError("ERR0005");
      }
      return true;
    }

    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;

      int countComponent = ultComponent.Rows.Count;

      string code = this.SavePackageInfo();
      if (code.Trim().Length == 0)
      {
        return false;
      }
      this.packageCode = code;
      result = this.SaveBoxTypeList();

      result = this.SaveDirectLabourList();
      return result;
    }

    /// <summary>
    /// SaveCuttingList
    /// </summary>
    /// <returns></returns>
    private bool SaveBoxTypeList()
    {

      string storeName = string.Empty;
      bool result = true;

      int countComponent = ultComponent.Rows.Count;
      int countComponentDetail = int.MinValue;

      //save data
      for (int i = 0; i < countComponent; i++)
      {

        UltraGridRow row = ultComponent.Rows[i];
        // 1. Save BoxTypeInfo
        storeName = string.Empty;
        DBParameter[] inputParam = new DBParameter[14];
        long componentPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

        //insert BoxType
        if (componentPid == long.MinValue)
        {
          storeName = "spBOMBoxType_Insert";
          inputParam[11] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        //update BoxType
        else
        {
          storeName = "spBOMBoxType_Update";
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, componentPid);
          inputParam[11] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }

        if (storeName.Length > 0)
        {
          inputParam = this.SetBoxTypeComponentParam(inputParam, row);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          componentPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (componentPid <= 0)
          {
            result = false;
            continue;
          }
        }
        // 2. Save BoxTypeDetail Info
        countComponentDetail = row.ChildBands[0].Rows.Count;
        for (int j = 0; j < countComponentDetail; j++)
        {
          UltraGridRow rowDetail = row.ChildBands[0].Rows[j];
          storeName = string.Empty;
          DBParameter[] inputParamDetail = new DBParameter[10];
          long detailPid = DBConvert.ParseLong(rowDetail.Cells["Pid"].Value.ToString());
          if (detailPid == long.MinValue) // Insert
          {
            storeName = "spBOMBoxTypeDetail_Insert";
            inputParamDetail[0] = new DBParameter("@BoxTypePid", DbType.Int64, componentPid);
            inputParamDetail[9] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          else
          {
            storeName = "spBOMBoxTypeDetail_Update";
            inputParamDetail[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
            inputParamDetail[9] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          if (storeName.Length > 0)
          {
            inputParamDetail = this.SetBoxTypeDetailParam(inputParamDetail, rowDetail);
            DBParameter[] outputParamDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamDetail, outputParamDetail);
            detailPid = DBConvert.ParseLong(outputParamDetail[0].Value.ToString());
            if (detailPid <= 0)
            {
              result = false;
            }
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Save Direct Labour
    /// </summary>
    /// <returns></returns>
    private bool SaveDirectLabourList()
    {
      string storeName = string.Empty;
      bool result = true;

      int countComponent = ultDirectLabour.Rows.Count;

      //save data
      for (int i = 0; i < countComponent; i++)
      {

        UltraGridRow row = ultDirectLabour.Rows[i];
        // 1. Save Direct Labour
        storeName = "spBOMPackageDirectLabour_Edit";

        DBParameter[] inputParam = new DBParameter[8];
        long componentPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

        //insert Direct Labour
        if (componentPid == long.MinValue)
        {
          inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        //update Direct Labour
        else
        {
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, componentPid);
          inputParam[7] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }

        //PackagePid
        string text = string.Empty;
        string commandText = "SELECT Pid  FROM TblBOMPackage  WHERE PackageCode = '" + this.packageCode + "'";
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (obj != null)
          text = obj.ToString();

        inputParam[1] = new DBParameter("@PackagePid", DbType.Int64, Convert.ToInt64(text));

        inputParam[2] = new DBParameter("@SectionCode", DbType.String, row.Cells["SectionCode"].Value.ToString());
        if (row.Cells["Qty"].Value.ToString().Length > 0)
        {
          inputParam[3] = new DBParameter("@Qty", DbType.Double, row.Cells["Qty"].Value.ToString());
        }

        if (row.Cells["Description"].Value.ToString().Length > 0)
        {
          inputParam[4] = new DBParameter("@Description", DbType.String, row.Cells["Description"].Value.ToString());
        }

        if (row.Cells["Remark"].Value.ToString().Length > 0)
        {
          inputParam[5] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
        }

        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
        componentPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (componentPid <= 0)
        {
          result = false;
          continue;
        }

      }
      return result;
    }

    /// <summary>
    /// SetBoxTypeDetailParam
    /// </summary>
    /// <param name="param"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private DBParameter[] SetBoxTypeDetailParam(DBParameter[] param, UltraGridRow row)
    {
      //MaterialCode
      string text = row.Cells["MaterialCode"].Value.ToString().Trim();
      param[1] = new DBParameter("@MaterialCode", DbType.String, text);

      //Alternative
      text = row.Cells["Alternative"].Value.ToString().Trim();
      param[2] = new DBParameter("@Alternative", DbType.String, text);

      //Waste
      text = row.Cells["Waste"].Value.ToString().Trim();
      if (DBConvert.ParseDouble(text) != double.MinValue)
      {
        param[3] = new DBParameter("@Waste", DbType.Double, DBConvert.ParseDouble(text));
      }

      //Length
      text = row.Cells["RAW_Length"].Value.ToString().Trim();
      if (DBConvert.ParseDouble(text) != double.MinValue)
      {
        param[4] = new DBParameter("@Length", DbType.Double, DBConvert.ParseDouble(text));
      }

      //Width
      text = row.Cells["RAW_Width"].Value.ToString().Trim();
      if (DBConvert.ParseDouble(text) != double.MinValue)
      {
        param[5] = new DBParameter("@Width", DbType.Double, DBConvert.ParseDouble(text));
      }

      //Thickness
      text = row.Cells["RAW_Thickness"].Value.ToString().Trim();
      if (DBConvert.ParseDouble(text) != double.MinValue)
      {
        param[6] = new DBParameter("@Thickness", DbType.Double, DBConvert.ParseDouble(text));
      }

      //Qty
      text = row.Cells["Qty"].Value.ToString().Trim();
      if (DBConvert.ParseDouble(text) != double.MinValue)
      {
        param[7] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(text));
      }

      //TotalQty
      text = row.Cells["TotalQty"].Value.ToString().Trim();
      if (DBConvert.ParseDouble(text) != double.MinValue)
      {
        param[8] = new DBParameter("@TotalQty", DbType.Double, DBConvert.ParseDouble(text));
      }

      return param;
    }

    /// <summary>
    /// SetBoxTypeComponentParam
    /// </summary>
    /// <param name="param"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    private DBParameter[] SetBoxTypeComponentParam(DBParameter[] param, UltraGridRow row)
    {
      string commandText = string.Empty;
      //BoxTypeCode
      string text = row.Cells["BoxCode"].Value.ToString().Trim().Replace("'", "''");
      param[1] = new DBParameter("@BoxTypeCode", DbType.String, text);

      //PackagePid
      commandText = "SELECT Pid  FROM TblBOMPackage  WHERE PackageCode = '" + this.packageCode + "'";
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      if (obj != null)
      {
        text = obj.ToString();
      }
      this.pid = DBConvert.ParseLong(text);
      if (this.pid > 0)
      {
        param[2] = new DBParameter("@PackagePid", DbType.Int64, this.pid);
      }
      //DimensionPID
      long pidDimension = 0;
      double length = DBConvert.ParseDouble(row.Cells["Length"].Value.ToString().Trim());
      double width = DBConvert.ParseDouble(row.Cells["Width"].Value.ToString().Trim());
      double height = DBConvert.ParseDouble(row.Cells["Height"].Value.ToString().Trim());
      DataTable dtData = new DataTable();

      commandText = "SELECT * FROM TblWHFDimension WHERE Length = " + length + " AND Width = " + width + " AND Height = " + height;
      dtData = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtData.Rows.Count == 0)
      {
        //dont have dimension,insert
        DBParameter[] inputParamDimension = new DBParameter[3];
        inputParamDimension[0] = new DBParameter("@Length", DbType.Int32, length);
        inputParamDimension[1] = new DBParameter("@Width", DbType.Int32, width);
        inputParamDimension[2] = new DBParameter("@Height", DbType.Int32, height);
        DBParameter[] outputParamDimension = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMDimension_Insert", inputParamDimension, outputParamDimension);
        pidDimension = DBConvert.ParseLong(outputParamDimension[0].Value.ToString());
      }
      else
      {
        //have dimension
        pidDimension = DBConvert.ParseLong(dtData.Rows[0]["PID"].ToString());
      }
      param[3] = new DBParameter("@DimensionPid", DbType.Int64, pidDimension);


      //BoxTypeName
      text = row.Cells["BoxName"].Value.ToString().Trim().Replace("'", "''");
      param[4] = new DBParameter("@BoxTypeName", DbType.String, text);

      //CBM
      double cbm = DBConvert.ParseDouble((length * width * height) / 1000000000.0);
      param[5] = new DBParameter("@CBM", DbType.Double, cbm);

      //BoxPerItem
      int box = DBConvert.ParseInt(this.txtBox.Text);
      if (box != int.MinValue)
      {
        param[6] = new DBParameter("@BoxPerItem", DbType.Int16, box);
      }

      //ItemPerBox
      int item = DBConvert.ParseInt(this.txtItem.Text);
      if (item != int.MinValue)
      {
        param[7] = new DBParameter("@ItemPerBox", DbType.Int16, item);
      }

      //NumberPerBox
      int no = DBConvert.ParseInt(row.Cells["No"].Value.ToString().Trim());
      if (no != int.MinValue)
      {
        param[8] = new DBParameter("@No", DbType.Int16, no);
      }

      //GWeight
      double gWeight = DBConvert.ParseDouble(row.Cells["GWeight"].Value.ToString().Trim());
      if (gWeight != double.MinValue)
      {
        param[9] = new DBParameter("@GWeight", DbType.Double, gWeight);
      }

      //NWeight
      double nWeight = DBConvert.ParseDouble(row.Cells["NWeight"].Value.ToString().Trim());
      if (nWeight != double.MinValue)
      {
        param[10] = new DBParameter("@NWeight", DbType.Double, nWeight);
      }

      //Check Weight
      int chkWeight = DBConvert.ParseInt(row.Cells["CheckWeight"].Value.ToString().Trim());
      if (chkWeight != int.MinValue)
      {
        param[12] = new DBParameter("@CheckWeight", DbType.Int32, chkWeight);
      }
      string barcode = row.Cells["Barcode"].Value.ToString().Trim();
      if (barcode.Length > 0)
      {
        param[13] = new DBParameter("@Barcode", DbType.AnsiString, 50, barcode);
      }
      return param;
    }

    /// <summary>
    /// SavePackageInfo
    /// </summary>
    /// <returns></returns>
    private string SavePackageInfo()
    {
      string result = string.Empty;

      DBParameter[] inputParam = new DBParameter[11];
      string storeName = string.Empty;

      if (this.pid > 0)
      {
        inputParam[0] = new DBParameter("@PackageCode", DbType.AnsiString, 16, this.packageCode);
        inputParam[7] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spBOMPackage_Update";

      }
      else
      {
        inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spBOMPackage_Insert";

      }

      string text = this.txtPackageName.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[1] = new DBParameter("@PackageName", DbType.String, text);
      }

      int k = this.ucboItemCode.Value.ToString().IndexOf('|');

      text = this.ucboItemCode.Value.ToString().Substring(0, k).Trim();
      if (text.Length > 0)
      {
        inputParam[2] = new DBParameter("@ItemCode", DbType.String, text);
      }

      int j = this.ucboItemCode.Text.IndexOf('|');
      text = ucboItemCode.Text.Substring(j + 2, 1).Trim().ToString();

      inputParam[3] = new DBParameter("@Revision", DbType.Int32, Convert.ToInt32(text));

      text = this.txtItem.Text;
      inputParam[4] = new DBParameter("@QuantityItem", DbType.Int32, Convert.ToInt32(text));

      text = this.txtBox.Text;
      inputParam[5] = new DBParameter("@QuantityBox", DbType.Int32, Convert.ToInt32(text));

      //totalCBM
      text = "";
      totalCBM = 0;
      int count = ultComponent.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultComponent.Rows[i];
        double length = DBConvert.ParseDouble(row.Cells["Length"].Value.ToString());
        double width = DBConvert.ParseDouble(row.Cells["Width"].Value.ToString());
        double height = DBConvert.ParseDouble(row.Cells["Height"].Value.ToString());
        totalCBM += ((double)(length * width * height)) / 1000000000.0;
      }

      text = totalCBM.ToString();

      inputParam[6] = new DBParameter("@TotalCBM", DbType.String, text);

      int value = (chkLock.Checked) ? 1 : 0;
      inputParam[8] = new DBParameter("@Confirm", DbType.Int32, value);

      value = (this.chkDefault.Checked) ? 1 : 0;
      inputParam[9] = new DBParameter("@SetDefault", DbType.Int32, value);
      if (ucboRemark.Value != null && DBConvert.ParseLong(ucboRemark.Value.ToString()) != long.MinValue)
      {
        inputParam[10] = new DBParameter("@Remark", DbType.Int32, DBConvert.ParseInt(ucboRemark.Value.ToString()));
      }
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      result = outputParam[0].Value.ToString();
      this.packageCode = outputParam[0].Value.ToString();
      return result;
    }


    /// <summary>
    /// set default and package standard for item
    /// </summary>
    private void SetPackageDefault()
    {
      //default
      if (chkDefault.Checked)
      {
        int k = this.ucboItemCode.Value.ToString().IndexOf('|');
        int j = this.ucboItemCode.Text.IndexOf('|');
        DBParameter[] inputPkCode = new DBParameter[6];
        inputPkCode[0] = new DBParameter("@ItemCode", DbType.String, this.ucboItemCode.Value.ToString().Substring(0, k).Trim());
        inputPkCode[1] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(ucboItemCode.Text.Substring(j + 2, 1).Trim().ToString()));
        inputPkCode[2] = new DBParameter("@PackageCode", DbType.String, this.packageCode);

        totalCBM = 0;
        int count = ultComponent.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          UltraGridRow row = ultComponent.Rows[i];
          double length = DBConvert.ParseDouble(row.Cells["Length"].Value.ToString());
          double width = DBConvert.ParseDouble(row.Cells["Width"].Value.ToString());
          double height = DBConvert.ParseDouble(row.Cells["Height"].Value.ToString());
          totalCBM += ((double)(length * width * height)) / 1000000000.0;
        }

        totalCBM = totalCBM / (DBConvert.ParseInt(this.txtItem.Text));
        inputPkCode[3] = new DBParameter("@CBM", DbType.Double, totalCBM);

        int countComponent = ultComponent.Rows.Count;
        double gWeight = 0;
        double nWeight = 0;

        for (int i = 0; i < countComponent; i++)
        {
          UltraGridRow row = ultComponent.Rows[i];

          if (row.Cells["GWeight"].Value.ToString().Length > 0)
          {
            //GWeight
            gWeight += DBConvert.ParseDouble(row.Cells["GWeight"].Value.ToString().Trim());
          }

          if (row.Cells["NWeight"].Value.ToString().Length > 0)
          {
            //GWeight
            nWeight += DBConvert.ParseDouble(row.Cells["NWeight"].Value.ToString().Trim());
          }
        }

        if (nWeight > 0)
        {
          inputPkCode[4] = new DBParameter("@NWeightDefault", DbType.Double, nWeight / (DBConvert.ParseInt(this.txtItem.Text)));
        }

        if (gWeight > 0)
        {
          inputPkCode[5] = new DBParameter("@GWeightDefault", DbType.Double, gWeight / (DBConvert.ParseInt(this.txtItem.Text)));
        }

        DataBaseAccess.ExecuteStoreProcedure("spBOMItemInfoPackageCode_Update", inputPkCode, null);
      }
    }

    /// <summary>
    /// SetTotalQty
    /// </summary>
    /// <param name="row"></param>
    /// <param name="unitCode"></param>
    private void SetTotalQty(UltraGridRow row, int unitCode)
    {
      double length = DBConvert.ParseDouble(row.Cells["RAW_Length"].Value.ToString());
      double width = DBConvert.ParseDouble(row.Cells["RAW_Width"].Value.ToString());
      double height = DBConvert.ParseDouble(row.Cells["RAW_Thickness"].Value.ToString());
      double qty = DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString());
      double totalQty = qty;
      if (qty != double.MinValue)
      {
        if (unitCode == 4)
        {
          if (length != double.MinValue)
          {
            totalQty = qty * length / 1000.00;
          }
          else
          {
            totalQty = qty / 1000.00;
          }
        }
        else if (unitCode == 6)
        {
          if (length != double.MinValue && width != double.MinValue)
          {
            totalQty = qty * length * width / 1000000.00;
          }
          else
          {
            totalQty = qty / 1000000.00;
          }
        }
        else if (unitCode == 7)
        {
          if (length != double.MinValue && width != double.MinValue && height != double.MinValue)
          {
            totalQty = qty * length * width * height / 1000000000.00;
          }
          else
          {
            totalQty = qty / 1000000000.00;
          }
        }
        if (totalQty != double.MinValue)
        {
          row.Cells["TotalQty"].Value = Math.Round(totalQty, 4);
        }
        else
        {
          row.Cells["TotalQty"].Value = DBNull.Value;
        }
      }
    }

    /// <summary>
    /// SetStatusRow
    /// </summary>
    /// <param name="row"></param>
    /// <param name="unitCode"></param>
    private void SetStatusRow(UltraGridRow row, int unitCode)
    {
      if (unitCode == 4)
      { // m
        row.Cells["RAW_Length"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Width"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Thickness"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Width"].Value = DBNull.Value;
        row.Cells["RAW_Thickness"].Value = DBNull.Value;
      }
      else if (unitCode == 6)
      { // sqm
        row.Cells["RAW_Length"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Width"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Thickness"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Thickness"].Value = DBNull.Value;
      }
      else if (unitCode == 7)
      { // cbm
        row.Cells["RAW_Length"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Width"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Thickness"].Activation = Activation.AllowEdit;
      }
      else
      {
        row.Cells["RAW_Length"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Width"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Thickness"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Length"].Value = DBNull.Value;
        row.Cells["RAW_Width"].Value = DBNull.Value;
        row.Cells["RAW_Thickness"].Value = DBNull.Value;
      }
    }
    #endregion ProcessData

    #region Button Click

    /// <summary>
    /// multiCBItemCode_SelectedIndexChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ucboItemCode_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.SetNeedToSave();
      string commandText = string.Empty;
      DataTable dt = new DataTable();
      if (ucboItemCode.SelectedRow != null)
      {
        int countComponent = ultComponent.Rows.Count;
        int k = 0;

        if (ucboItemCode.Value != null)
        {
          int i = this.ucboItemCode.Text.IndexOf('|');
          k = this.ucboItemCode.Value.ToString().IndexOf('|');

          string itemCode = this.ucboItemCode.Value.ToString().Substring(0, k).Trim();

          int revision = DBConvert.ParseInt(this.ucboItemCode.Value.ToString().Substring(k + 1, this.ucboItemCode.Value.ToString().Length - k - 1).Trim());


          commandText += " SELECT [Width], [Depth], [High], [KD] FROM TblBOMItemInfo WHERE ItemCode = '" + itemCode + "' AND Revision =" + revision;
          dt = DataBaseAccess.SearchCommandText(commandText).Tables[0];
          if (dt != null && dt.Rows.Count > 0)
          {
            this.lblDimension.Text = "Item Dimension (Width x Depth x High) : " + dt.Rows[0]["Width"].ToString() + " x " + dt.Rows[0]["Depth"].ToString() + " x " + dt.Rows[0]["High"].ToString();
            if (DBConvert.ParseInt(dt.Rows[0]["KD"].ToString()) == 1)
            {
              this.lblDimension.Text += " KD";
            }
          }
        }

        //  //check number box
        //  for (int i = 0; i < countComponent; i++)
        //  {
        //    UltraGridRow rowCheck = ultComponent.Rows[i];

        //    int j = this.ucboItemCode.Text.IndexOf('|');
        //    string text = this.ucboItemCode.Text.Substring(j + 1).Trim().ToString();
        //    k = this.ucboItemCode.Value.ToString().IndexOf('|');

        //    if (DBConvert.ParseInt(this.txtBox.Text) > 1)
        //    {
        //      rowCheck.Cells["BoxCode"].Value = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + text.PadLeft(2, '0') + "_" + rowCheck.Cells["No"].Value.ToString().Trim() + "/" + this.txtBox.Text.Trim();
        //    }
        //    else if (DBConvert.ParseInt(this.txtBox.Text) == 1)
        //    {
        //      rowCheck.Cells["BoxCode"].Value = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + text.PadLeft(2, '0') + "_" + this.txtItem.Text + "/" + this.txtBox.Text.Trim();
        //    }
        //  }
        //  k = this.ucboItemCode.Value.ToString().IndexOf('|');
        //  commandText = "SELECT CarcassCode FROM TblBOMItemInfo WHERE ItemCode = '" + this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "' AND Revision =" + this.ucboItemCode.Value.ToString().Substring(k + 1, this.ucboItemCode.Value.ToString().Length - k - 1).Trim();
        //  dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        //  if (dt.Rows.Count > 0)
        //  {
        //    this.LoadReference(dt.Rows[0][0].ToString(), this.packageCode);
        //  }

        //  int mm = this.ucboItemCode.Value.ToString().IndexOf('|');
        //  commandText = string.Empty;
        //  commandText += " SELECT TOP 1 PackageCode ";
        //  commandText += " FROM TblBOMPackage ";
        //  commandText += " WHERE ItemCode =  '" + this.ucboItemCode.Value.ToString().Substring(0, mm).Trim() + "'";
        //  commandText += " 	AND Revision != " + this.ucboItemCode.Value.ToString().Substring(mm + 1, this.ucboItemCode.Value.ToString().Length - mm - 1).Trim();
        //  commandText += " ORDER BY Revision DESC";

        //  DataTable dtRef = DataBaseAccess.SearchCommandTextDataTable(commandText);
        //  if (dtRef.Rows.Count > 0)
        //  {
        //    this.ucboReference.Value = dtRef.Rows[0][0].ToString();
        //  }
        //}
        //else
        //{
        //  this.LoadReference("", "");
      }
    }

    /// <summary>
    /// btnSave_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckAndSaveData())
      {
        this.LoadData();
      }
    }

    /// <summary>
    /// btnClose_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseForm();
    }
    #endregion Button Click

    #region ultComponent Handle Event
    private void ultComponent_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands["dtBoxType"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["dtBoxType"].Columns["BoxTypePid"].Hidden = true;
      e.Layout.Bands["dtBoxType"].Columns["CheckWeight"].Hidden = true;
      e.Layout.Bands["dtBoxType"].Columns["BoxCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtBoxType"].Columns["BoxCode"].Header.Caption = "Mã thùng";
      e.Layout.Bands["dtBoxType"].Columns["BoxName"].Header.Caption = "Tên thùng";
      e.Layout.Bands["dtBoxType"].Columns["Child"].Hidden = true;
      e.Layout.Bands["dtBoxType"].Columns["CheckWeight"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["dtBoxType"].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands["dtBoxType"].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;
      e.Layout.Bands["dtBoxType"].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["GWeight"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["NWeight"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["Length"].Header.Caption = "Dài";
      e.Layout.Bands["dtBoxType"].Columns["Height"].Header.Caption = "Cao";
      e.Layout.Bands["dtBoxType"].Columns["Width"].Header.Caption = "Rộng";


      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["BoxTypePid"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["BoxNo"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Child"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["BoxCode"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["IDFactoryUnit"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialCode"].ValueList = ucbddMaterial;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["FactoryUnit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["TotalQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialCode"].Header.Caption = "Sản phẩm";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialName"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["FactoryUnit"].Header.Caption = "Đơn vị";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["TotalQty"].Header.Caption = "Tổng số lượng";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["RAW_Length"].Header.Caption = "Dài";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["RAW_Width"].Header.Caption = "Rộng";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["RAW_Thickness"].Header.Caption = "Dày";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["RAW_Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["RAW_Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["RAW_Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Alternative"].ValueList = ucbddAlternative;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;
      //e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      //e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Override.AllowDelete = DefaultableBoolean.True;			
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialName"].Width = 250;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialName"].Hidden = true;
      // Set auto complete combo in grid
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialCode"].MinWidth = 400;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialCode"].MaxWidth = 400;
      for (int k = 0; k < ultComponent.Rows.Count; k++)
      {
        UltraGridRow row = ultComponent.Rows[k];
        for (int i = 0; i < row.ChildBands[0].Rows.Count; i++)
        {
          UltraGridRow childRow = row.ChildBands[0].Rows[i];
          int unitCode = int.MinValue;
          try
          {
            unitCode = DBConvert.ParseInt(childRow.Cells["IDFactoryUnit"].Value.ToString());
          }
          catch
          {
          }
          this.SetStatusRow(childRow, unitCode);
        }
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// ultComponent_BeforeCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComponent_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();

      switch (columnName.ToLower())
      {
        case "materialcode":
        case "alternative":
          //bool validMaterials = FunctionUtility.CheckBOMMaterialCode(text, 7);
          //if (!validMaterials)
          //{
          //  WindowUtinity.ShowMessageWarning("ERR0001", columnName);
          //  e.Cancel = true;
          //}
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// ultComponent_AfterCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComponent_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
      string columnName = e.Cell.Column.ToString().ToLower();
      int unitCode = int.MinValue;

      switch (columnName)
      {
        case "materialcode":
          try
          {
            e.Cell.Row.Cells["MaterialName"].Value = ucbddMaterial.SelectedRow.Cells["MaterialName"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["MaterialName"].Value = DBNull.Value;
          }
          try
          {
            e.Cell.Row.Cells["FactoryUnit"].Value = ucbddMaterial.SelectedRow.Cells["Unit"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["FactoryUnit"].Value = DBNull.Value;
          }
          try
          {
            e.Cell.Row.Cells["IDFactoryUnit"].Value = ucbddMaterial.SelectedRow.Cells["IDFactoryUnit"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["IDFactoryUnit"].Value = DBNull.Value;
          }

          try
          {
            unitCode = DBConvert.ParseInt(ucbddMaterial.SelectedRow.Cells["IDFactoryUnit"].Value.ToString());
          }
          catch
          {
          }
          this.SetStatusRow(e.Cell.Row, unitCode);
          this.SetTotalQty(e.Cell.Row, DBConvert.ParseInt(e.Cell.Row.Cells["IDFactoryUnit"].Value.ToString()));
          break;
        case "no":

          if (this.ucboItemCode.Value.ToString().Length > 0)
          {
            string message = string.Empty;

            //itemCode
            int k = this.ucboItemCode.Value.ToString().IndexOf('|');

            string itemCode = this.ucboItemCode.Value.ToString().Substring(0, k).Trim();
            if (itemCode.Length == 0)
            {
              WindowUtinity.ShowMessageWarning("MSG0005", "ItemCode");
              break;
            }
            int j = this.ucboItemCode.Text.IndexOf('|');
            string text = this.ucboItemCode.Text.Substring(j + 1).Trim().ToString();
            int pos_Revision = text.IndexOf('|');
            string revision = text.Substring(0, pos_Revision).ToString().Trim();

            if (DBConvert.ParseInt(this.txtBox.Text) > 1)
            {
              e.Cell.Row.Cells["BoxCode"].Value = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + revision.PadLeft(2, '0') + "_" + e.Cell.Row.Cells["No"].Value.ToString().Trim() + "/" + this.txtBox.Text.Trim();
            }
            else if (DBConvert.ParseInt(this.txtBox.Text) == 1)
            {
              e.Cell.Row.Cells["BoxCode"].Value = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + revision.PadLeft(2, '0') + "_" + this.txtItem.Text + "/" + this.txtBox.Text.Trim();
            }
          }

          break;
        case "qty":
        case "raw_length":
        case "raw_width":
        case "raw_thickness":
          this.SetTotalQty(e.Cell.Row, DBConvert.ParseInt(e.Cell.Row.Cells["IDFactoryUnit"].Value.ToString()));
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// ultComponent_BeforeRowsDeleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComponent_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      this.SetNeedToSave();
      foreach (UltraGridRow row in e.Rows)
      {
        int childRow = DBConvert.ParseInt(row.Cells["Child"].Value.ToString());
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          DBParameter[] inputParams = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          string storeName = string.Empty;
          if (childRow == 0) // Row Parents
          {
            storeName = "spBOMBoxType_Delete";
          }
          else
          {
            storeName = "spBOMBoxTypeDetail_Delete";
          }
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadData();
            return;
          }
        }
      }
    }

    /// <summary>
    /// Diect Labour After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDirectLabour_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      this.SetNeedToSave();
      switch (columnName)
      {
        case "sectioncode":
          try
          {
            e.Cell.Row.Cells["NameEN"].Value = udrpSectionCode.SelectedRow.Cells["NameEN"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["NameEN"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Direct Labour Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDirectLabour_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName.ToLower())
      {
        case "sectioncode":
          string commandText = "SELECT COUNT(Code) FROM VBOMSection WHERE Code ='" + text + "'";
          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
            {
              WindowUtinity.ShowMessageWarning("ERR0001", columnName);
              e.Cancel = true;
            }
          }
          else
          {
            WindowUtinity.ShowMessageWarning("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Print Package
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      View_BOM_0018_ItemMaterialReport view = new View_BOM_0018_ItemMaterialReport();

      string commandText = string.Format("Select PackageCode From TblBOMItemInfo Where ItemCode = '{0}' And Revision = '{1}'", this.itemCode, this.revision);
      DataTable dtPackage = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if ((dtPackage != null) && (dtPackage.Rows.Count > 0))
      {
        string packageCode = dtPackage.Rows[0]["PackageCode"].ToString();
        view.packageCode = packageCode;
        view.ncategory = 18;
        Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow);
      }
    }

    /// <summary>
    /// Direct Labour Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDirectLabour_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands["dtDirectLabour"].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands["dtDirectLabour"].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;

      if (this.nonPacking)
      {
        e.Layout.Bands["dtDirectLabour"].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        e.Layout.Bands["dtDirectLabour"].Override.AllowDelete = DefaultableBoolean.True;
      }

      e.Layout.Bands["dtDirectLabour"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["dtDirectLabour"].Columns["PackagePid"].Hidden = true;
      e.Layout.Bands["dtDirectLabour"].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtDirectLabour"].Columns["SectionCode"].ValueList = udrpSectionCode;
      e.Layout.Bands["dtDirectLabour"].Columns["SectionCode"].Header.Caption = "Section Code";
      e.Layout.Bands["dtDirectLabour"].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Delete Direct Labour
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDirectLabour_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          DBParameter[] inputParams = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.ExecuteStoreProcedure("spBOMPackageDirectLabour_Delete", inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) == 1)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadData();
            return;
          }
        }
      }
    }

    /// <summary>
    /// New package
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      //this.packageCode = string.Empty;
      //ultComponent.DataSource = null;
      //ultDirectLabour.DataSource = null;
      //ucboRemark.Value = null;
      //ucboItemCode.Value = null;
      //ucboReference.Value = null;
      //txtPackageName.Text = string.Empty;
      //txtItem.Text = string.Empty;
      //txtBox.Text = string.Empty;
      //this.status = 0;
      //this.nonConfirm = true;
      //btnSave.Visible = true;
      //btnCopy.Enabled = true;
      //this.LoadData();
      if (this.ConfirmToCloseForm())
      {
        viewBOM_03_021 view = new viewBOM_03_021();
        Shared.Utility.WindowUtinity.ShowView(view, "Package Information", false, Shared.Utility.ViewState.Window, FormWindowState.Normal);
      }
    }

    /// <summary>
    /// Copy Package Code
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCopy_Click(object sender, EventArgs e)
    {
      if (this.ucboReference.Value == null)
      {
        return;
      }

      if (this.ucboReference.Value != null && this.ucboReference.Value.ToString().Length == 0)
      {
        string message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Reference");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (this.ucboItemCode.Value != null && this.ucboItemCode.Value.ToString().Length == 0)
      {
        string message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Item Code");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (this.txtBox.Text.Length == 0)
      {
        string message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Quantity Box");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (this.txtItem.Text.Length == 0)
      {
        string message = string.Format(FunctionUtility.GetMessage("MSG0005"), "Quantity Item");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PackageCode", DbType.AnsiString, 16, this.ucboReference.Value.ToString()) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMPackageInfomationByPackageCode", inputParam);

      DataSet dsData = CreateDataSet.BoxTypeInfo();

      try
      {
        dsData.Tables["dtBoxType"].Merge(dsSource.Tables[1]);
      }
      catch
      {
      }
      try
      {
        dsData.Tables["dtBoxTypeDetail"].Merge(dsSource.Tables[2]);
      }
      catch
      {
      }

      DataSet dsUltComponent = (DataSet)this.ultComponent.DataSource;

      foreach (DataRow drBoxType in dsData.Tables[0].Rows)
      {
        DataRow drUltBoxType = dsUltComponent.Tables[0].NewRow();

        drUltBoxType["BoxTypePid"] = drBoxType["BoxTypePid"].ToString();

        //itemCode
        int k = this.ucboItemCode.Value.ToString().IndexOf('|');
        int j = this.ucboItemCode.Text.IndexOf('|');
        string text = this.ucboItemCode.Text.Substring(j + 1).Trim().ToString();

        if (DBConvert.ParseInt(this.txtBox.Text) > 1)
        {
          drUltBoxType["BoxCode"] = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + text.PadLeft(2, '0') + "_" + drBoxType["No"].ToString() + "/" + this.txtBox.Text.Trim();
        }
        else if (DBConvert.ParseInt(this.txtBox.Text) == 1)
        {
          drUltBoxType["BoxCode"] = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + text.PadLeft(2, '0') + "_" + this.txtItem.Text + "/" + this.txtBox.Text.Trim();
        }

        //drUltBoxType["BoxCode"] = drBoxType["BoxCode"].ToString();

        drUltBoxType["BoxName"] = drBoxType["BoxName"].ToString();
        drUltBoxType["Width"] = DBConvert.ParseDouble(drBoxType["Width"].ToString());
        drUltBoxType["Length"] = DBConvert.ParseDouble(drBoxType["Length"].ToString());
        drUltBoxType["Height"] = DBConvert.ParseDouble(drBoxType["Height"].ToString());
        drUltBoxType["No"] = DBConvert.ParseInt(drBoxType["No"].ToString());
        if (drBoxType["GWeight"].ToString().Length > 0)
        {
          drUltBoxType["GWeight"] = DBConvert.ParseDouble(drBoxType["GWeight"].ToString());
        }

        if (drBoxType["NWeight"].ToString().Length > 0)
        {
          drUltBoxType["NWeight"] = DBConvert.ParseDouble(drBoxType["NWeight"].ToString());
        }

        drUltBoxType["Child"] = 0;
        //drUltBoxType["CheckWeight"] = 0;


        dsUltComponent.Tables[0].Rows.Add(drUltBoxType);
      }

      foreach (DataRow drBoxTypeDetail in dsData.Tables[1].Rows)
      {
        DataRow drUltBoxTypeDetail = dsUltComponent.Tables[1].NewRow();

        //drUltBoxTypeDetail["BoxTypePid"] = drBoxTypeDetail["BoxTypePid"].ToString();

        int k = this.ucboItemCode.Value.ToString().IndexOf('|');
        int j = this.ucboItemCode.Text.IndexOf('|');
        string text = this.ucboItemCode.Text.Substring(j + 1).Trim().ToString();

        if (DBConvert.ParseInt(this.txtBox.Text) > 1)
        {
          drUltBoxTypeDetail["BoxCode"] = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + text.PadLeft(2, '0') + "_" + drBoxTypeDetail["BoxNo"].ToString() + "/" + this.txtBox.Text.Trim();
        }
        else if (DBConvert.ParseInt(this.txtBox.Text) == 1)
        {
          drUltBoxTypeDetail["BoxCode"] = this.ucboItemCode.Value.ToString().Substring(0, k).Trim() + "\\" + text.PadLeft(2, '0') + "_" + this.txtItem.Text.Trim() + "/" + this.txtBox.Text.Trim();
        }

        //drUltBoxTypeDetail["BoxCode"] = drBoxTypeDetail["BoxCode"].ToString();
        drUltBoxTypeDetail["MaterialCode"] = drBoxTypeDetail["MaterialCode"].ToString();
        drUltBoxTypeDetail["MaterialName"] = drBoxTypeDetail["MaterialName"].ToString();
        drUltBoxTypeDetail["FactoryUnit"] = drBoxTypeDetail["FactoryUnit"].ToString();
        drUltBoxTypeDetail["IDFactoryUnit"] = DBConvert.ParseInt(drBoxTypeDetail["IDFactoryUnit"].ToString());
        drUltBoxTypeDetail["Qty"] = DBConvert.ParseDouble(drBoxTypeDetail["Qty"].ToString());

        if (drBoxTypeDetail["RAW_Length"].ToString().Length > 0)
        {
          drUltBoxTypeDetail["RAW_Length"] = DBConvert.ParseDouble(drBoxTypeDetail["RAW_Length"].ToString());
        }

        if (drBoxTypeDetail["RAW_Width"].ToString().Length > 0)
        {
          drUltBoxTypeDetail["RAW_Width"] = DBConvert.ParseDouble(drBoxTypeDetail["RAW_Width"].ToString());
        }

        if (drBoxTypeDetail["RAW_Thickness"].ToString().Length > 0)
        {
          drUltBoxTypeDetail["RAW_Thickness"] = DBConvert.ParseDouble(drBoxTypeDetail["RAW_Thickness"].ToString());
        }

        drUltBoxTypeDetail["TotalQty"] = DBConvert.ParseDouble(drBoxTypeDetail["TotalQty"].ToString());
        drUltBoxTypeDetail["Alternative"] = drBoxTypeDetail["Alternative"].ToString();

        if (drBoxTypeDetail["Waste"].ToString().Length > 0)
        {
          drUltBoxTypeDetail["Waste"] = DBConvert.ParseDouble(drBoxTypeDetail["Waste"].ToString());
        }
        drUltBoxTypeDetail["Child"] = 1;

        dsUltComponent.Tables[1].Rows.Add(drUltBoxTypeDetail);
      }

      this.ultComponent.DataSource = dsUltComponent;
    }

    private void btnCopyMaterial_Click(object sender, EventArgs e)
    {
      // Material From
      if (this.ucboFrom.Value == null || this.ucboFrom.Value.ToString().Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Material From");
        return;
      }

      if (this.ucboTo.Value == null || this.ucboTo.Value.ToString().Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Material From");
        return;
      }

      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@BoxTypePidFrom", DbType.Int64, DBConvert.ParseLong(this.ucboFrom.Value.ToString()));
      inputParam[1] = new DBParameter("@BoxTypePidTo", DbType.Int64, DBConvert.ParseLong(this.ucboTo.Value.ToString()));
      inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      DataBaseAccess.ExecuteStoreProcedure("spPAKCopyMaterialsFromBoxCode_Insert", inputParam);
      WindowUtinity.ShowMessageSuccess("MSG0022");
      this.LoadData();
    }

    #endregion ultComponent Handle Event

  }
}
