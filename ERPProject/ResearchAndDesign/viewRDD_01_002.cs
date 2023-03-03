/*
  Update By   : 
  Description : Copy Level 2 Of Item Reference
  Date        : 
*/
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewRDD_01_002 : MainUserControl
  {
    public string itemCode = string.Empty;
    private DataTable moreDimensionSource;
    private DataTable otherFinishingSource;
    private DataTable dtRoomList;
    private bool loadingReference = false;
    private bool loadingData = false;
    private bool canUpdate = false;
    private bool isDuplicate = false;
    private IList listDeletedMoreDimension = new ArrayList();
    private IList listDeletedProductionRelative = new ArrayList();
    private IList listDeletingMoreDimension = new ArrayList();
    private IList listDeletedOtherFinishingCode = new ArrayList();
    private IList listDeletingOtherFinishingCode = new ArrayList();
    private IList listDeletedRoom = new ArrayList();
    private bool addReferenceItem = false;
    private String itemReference = string.Empty;
    private int itemKindGlobal = 0;
    UltraCombo ultraCBOtherFinishing = new UltraCombo();
    public viewRDD_01_002()
    {
      InitializeComponent();
    }

    private void View_RDD_0006_ItemInfo_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + SharedObject.UserInfo.UserPid + " | " + SharedObject.UserInfo.LoginDate;
      this.LoadCombobox();
      this.LoadData();
    }

    #region InnitData

    private void LoadItemReference()
    {
      this.loadingReference = true;
      DataTable dtIte1 = DataBaseAccess.SearchCommandTextDataTable(string.Format(@" SELECT INF.ItemCode, INF.Revision, BS.Name,
                                                                                    (INF.ItemCode + ' | ' + CAST(INF.Revision as varchar) + ' | ' + BS.Name) DisplayText
                                                                                    FROM	TblBOMItemInfo INF
                                                                                          INNER JOIN TblBOMItemBasic BS ON 
                                                                                                    (INF.ItemCode = BS.ItemCode)
                                                                                    WHERE INF.ItemCode <> '{0}' ORDER BY INF.ItemCode DESC", this.itemCode));
      Utility.LoadUltraCombo(ultraCbItemReference, dtIte1, "ItemCode", "DisplayText", "DisplayText");
      this.loadingReference = false;
    }
    private void LoadCombobox()
    {
      this.loadingData = true;
      this.LoadPrefixItemCode();
      this.LoadItemKind();
      Utility.LoadUltraCBCategory(cmbCategory);
      Utility.LoadUltraCBCollection(cmbCollection);
      Utility.LoadUltraComboCodeMst(cmbMainMaterials, ConstantClass.GROUP_MATERIALSTYPE);
      Utility.LoadComboboxCodeMst(cmbKD, ConstantClass.GROUP_KNOCKDOWN);
      Utility.LoadComboboxUnit(cmbUnit);
      Utility.LoadUltraCBCarcass(cmbCarcassCode);
      string finishingCondition = "DeleteFlag = 0"; // Qui, 30/10/2010 drop condition 'Confirm <> 0'
      Utility.LoadUltraComboboxFinishingStyle(cmbFinishingCode, false);
      Utility.LoadUltraComboFinishsingCode(ultraCBOtherFinishing, finishingCondition);
      Utility.LoadOtherMaterials(ultraCBOtherMaterials);
      Utility.LoadUltraDropdownCodeMst(udrpDimensionKind, ConstantClass.GROUP_MOREDIMENSION);
      // Load data Room
      string commandText = "SELECT Pid, Room FROM TblCSDRoom";
      DataTable dtRoom = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraDropDown(ultraDDRoom, dtRoom, "Pid", "Room", "Pid");

      // Reference
      //Tien edit load item and revision
      this.LoadItemReference();

      this.LoadUltraCBJC_OEM_Customer(long.MinValue);
      Utility.LoadUltraCBExhibition(ultraCBExhibition);

      this.LoadDesigner();

      // Load ultra dropdown Production Item
      DataTable dtProduction = DataBaseAccess.SearchCommandTextDataTable("SELECT ItemCode, SaleCode, Name FROM TblBOMItemBasic ORDER BY ItemCode");
      Utility.LoadUltraDropDown(ultraDDProductionItem, dtProduction, "ItemCode", "ItemCode");
      this.loadingData = false;
      this.LoadLastCarcassProcess();
    }

    /// <summary>
    /// Load last carcass process
    /// </summary>
    private void LoadLastCarcassProcess()
    {
      string commandText = string.Format(@" SELECT Code [Value],  [Value] Display
                                        FROM TblBOMCodeMaster 
                                        WHERE [Group] = 240221 ");
      DataTable dtLastCarcassProcess = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbLastCarcassProcess, dtLastCarcassProcess, "Value", "Display", false, "Value");
    }

    private void LoadDesigner()
    {
      string commandText = "SELECT Pid, Code + ' - ' + Name Display FROM TblRDDDesigner";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCBDesigner, dtSource, "Pid", "Display", false, "Pid");
    }

    private void LoadUltraCBJC_OEM_Customer(long cusPid)
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' | ' + Name Display
                             FROM	TblCSDCustomerInfo
                             WHERE Pid = {0}
                             UNION
                             SELECT	Pid, CustomerCode + ' | ' + Name Display
                             FROM	TblCSDCustomerInfo
                             WHERE	ParentPid IS NULL AND DeletedFlg = 0 AND (Pid = 27 OR Kind = 5) ORDER BY Display", cusPid);
      DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCBCustomer, dtCustomer, "Pid", "Display", false, "Pid");
    }

    /// <summary>
    /// Load combobox prefix item code
    /// </summary>
    private void LoadPrefixItemCode()
    {
      string commandText = string.Format("Select Distinct Left(ItemCode, 6) PrefixCode From TblRDDItemInfo Order By Left(ItemCode, 6) Desc");
      DataTable dtPrefixCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadMultiCombobox(multiCBPrefixCode, dtPrefixCode, "PrefixCode", "PrefixCode");
    }

    private void LoadItemKind()
    {
      string commandText = string.Format("Select Code, Value From TblBOMCodeMaster Where [Group] = {0} And DeleteFlag = 0 Order By Sort", ConstantClass.GROUP_ITEMKIND);
      DataTable dtItemKind = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtItemKind != null)
      {
        DataRow row = dtItemKind.NewRow();
        row["Code"] = 0;
        row["Value"] = "";
        dtItemKind.Rows.InsertAt(row, 0);
        Utility.LoadUltraCombo(ultraCBItemKind, dtItemKind, "Code", "Value", false, "Code");
      }
    }

    private void LoadReferenceGird(string code)
    {
      DBParameter[] param = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, code) };
      // LoadMoreDimension
      this.moreDimensionSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spBOMListMoreDimention", param);
      if (this.addReferenceItem)
      {
        foreach (DataRow row in this.moreDimensionSource.Rows)
        {
          row["Pid"] = DBNull.Value;
        }
      }
      ultMoreDimension.DataSource = this.moreDimensionSource;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["DimensionKind"].ValueList = udrpDimensionKind;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["DimensionKind"].Width = 100;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["DimensionKind"].Header.Caption = "Dimension Kind";
      ultMoreDimension.DisplayLayout.Bands[0].Columns["mm"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["Description"].CellActivation = Activation.ActivateOnly;

      ultMoreDimension.DisplayLayout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      if (!this.canUpdate)
      {
        int count = ultMoreDimension.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          ultMoreDimension.Rows[i].Activation = Activation.ActivateOnly;
        }
      }

      // OtherFinishing
      this.otherFinishingSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spBOMListOtherFinishingByItemCode", param);
      if (this.addReferenceItem)
      {
        foreach (DataRow row in this.otherFinishingSource.Rows)
        {
          row["Pid"] = DBNull.Value;
        }
      }
      ultOtherFinishCode.DataSource = this.otherFinishingSource;
      ultOtherFinishCode.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultOtherFinishCode.DisplayLayout.Bands[0].Columns["OtherFinishingCode"].ValueList = ultraCBOtherFinishing;
      ultOtherFinishCode.DisplayLayout.Bands[0].Columns["OtherFinishingCode"].Header.Caption = "Other Finishing";
      ultOtherFinishCode.DisplayLayout.Bands[0].Columns["SheenLevel"].Header.Caption = "Sheen Level";
      ultOtherFinishCode.DisplayLayout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      if (!this.canUpdate)
      {
        int count = ultOtherFinishCode.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          ultOtherFinishCode.Rows[i].Activation = Activation.ActivateOnly;
        }
      }

      //Sale Relationship
      DBParameter[] input = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, code) };
      DataTable dtItemSaleRelative = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spBOMListItemSaleRelativeByItemCode", input);
      ultraGridSaleRelative.DataSource = dtItemSaleRelative;

      //Production Relationship      
      //DataTable dtProductionRelative = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spBOMProductionRelative_Select", input);
      //ultraGridProductionRelative.DataSource = dtProductionRelative;

      //Room
      this.dtRoomList = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemRoom_Select", param);
      ultraGridRoom.DataSource = dtRoomList;
      if (this.addReferenceItem)
      {
        foreach (DataRow row in this.dtRoomList.Rows)
        {
          row["Pid"] = DBNull.Value;
        }
      }
    }

    private void LoadGird(string code)
    {
      DBParameter[] param = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, code) };
      // LoadMoreDimension
      this.moreDimensionSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spRDDListMoreDimensionByItemCode", param);
      if (this.addReferenceItem)
      {
        foreach (DataRow row in this.moreDimensionSource.Rows)
        {
          row["Pid"] = DBNull.Value;
        }
      }
      ultMoreDimension.DataSource = this.moreDimensionSource;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["DimensionKind"].ValueList = udrpDimensionKind;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["DimensionKind"].Width = 100;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["DimensionKind"].Header.Caption = "Dimension Kind";
      ultMoreDimension.DisplayLayout.Bands[0].Columns["mm"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultMoreDimension.DisplayLayout.Bands[0].Columns["Description"].CellActivation = Activation.ActivateOnly;

      ultMoreDimension.DisplayLayout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      if (!this.canUpdate)
      {
        int count = ultMoreDimension.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          ultMoreDimension.Rows[i].Activation = Activation.ActivateOnly;
        }
      }

      // OtherFinishing
      this.otherFinishingSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spRDDListOtherFinishingByItemCode", param);
      if (this.addReferenceItem)
      {
        foreach (DataRow row in this.otherFinishingSource.Rows)
        {
          row["Pid"] = DBNull.Value;
        }
      }
      ultOtherFinishCode.DataSource = this.otherFinishingSource;
      ultOtherFinishCode.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultOtherFinishCode.DisplayLayout.Bands[0].Columns["OtherFinishingCode"].ValueList = ultraCBOtherFinishing;
      ultOtherFinishCode.DisplayLayout.Bands[0].Columns["OtherFinishingCode"].Header.Caption = "Other Finishing";
      ultOtherFinishCode.DisplayLayout.Bands[0].Columns["SheenLevel"].Header.Caption = "Sheen Level";
      ultOtherFinishCode.DisplayLayout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      if (!this.canUpdate)
      {
        int count = ultOtherFinishCode.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          ultOtherFinishCode.Rows[i].Activation = Activation.ActivateOnly;
        }
      }

      //Sale Relationship
      DBParameter[] input = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, code) };
      DataTable dtItemSaleRelative = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spBOMListItemSaleRelativeByItemCode", input);
      ultraGridSaleRelative.DataSource = dtItemSaleRelative;

      //Production Relationship
      DataTable dtProductionRelative = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spBOMProductionRelative_Select", input);
      dtProductionRelative.PrimaryKey = new DataColumn[] { dtProductionRelative.Columns["Relative"] };
      ultraGridProductionRelative.DataSource = dtProductionRelative;

      //Room
      this.dtRoomList = DataBaseAccess.SearchStoreProcedureDataTable("spRDDListRoomByItemCode", param);
      ultraGridRoom.DataSource = dtRoomList;
      if (this.addReferenceItem)
      {
        foreach (DataRow row in this.dtRoomList.Rows)
        {
          row["Pid"] = DBNull.Value;
        }
      }
    }

    private void LoadData()
    {
      this.addReferenceItem = false;
      this.loadingData = true;
      bool confirm = false;
      this.listDeletedMoreDimension = new ArrayList();
      this.listDeletedOtherFinishingCode = new ArrayList();
      this.listDeletedRoom = new ArrayList();
      txtSuffixCode.ReadOnly = true;
      if (this.itemCode.Length == 0)
      {
        lblComponent.Visible = false;
      }
      else
      {
        //Check No Level 2
        string commandText = string.Format("Select Count(ComponentCode) From TblRDDItemComponent Where ItemCode = '{0}'", this.itemCode);
        int count = (int)DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (count > 0)
        {
          chkNoLevel2.Enabled = false;
        }

        multiCBPrefixCode.Enabled = false;
        btnGetCode.Enabled = false;
        lblComponent.Visible = true;
        RDDItemInfo item = new RDDItemInfo();
        item.ItemCode = this.itemCode;
        item = (RDDItemInfo)Shared.DataBaseUtility.DataBaseAccess.LoadObject(item, new string[] { "ItemCode" });
        if (item == null)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          return;
        }
        if (item != null)
        {
          btnPrint.Visible = true;
          btnExportexel.Visible = true;
        }

        image.ImageLocation = FunctionUtility.RDDGetItemImage(item.ItemCode);
        txtSaleCode.Text = item.SaleCode;
        txtOldCode.Text = item.OldCode;
        string[] listKey = item.ItemCode.Split('-');
        try
        {
          txtPrefixCode.Text = listKey[0];
        }
        catch { }
        try
        {
          txtSuffixCode.Text = listKey[1];
        }
        catch { }
        txtShortName.Text = item.ShortName;
        txtRDNote.Text = item.RDNote;
        if (item.CustomerPid != long.MinValue)
        {
          this.LoadUltraCBJC_OEM_Customer(item.CustomerPid);
          ultraCBCustomer.Value = item.CustomerPid;
        }
        if (item.Exhibition != int.MinValue)
        {
          ultraCBExhibition.Value = item.Exhibition;
        }
        txtItemName.Text = item.Name;
        txtDescription.Text = item.Description;
        if (item.Category != int.MinValue)
        {
          cmbCategory.Value = item.Category;
        }
        if (item.Collection != int.MinValue)
        {
          cmbCollection.Value = item.Collection;
        }
        if (item.CarcassCode.Length > 0)
        {
          cmbCarcassCode.Value = item.CarcassCode;
        }
        if (item.MainFinish.Length > 0)
        {
          cmbFinishingCode.Value = item.MainFinish;
        }
        if (item.MainMaterial.Length > 0)
        {
          cmbMainMaterials.Value = item.MainMaterial;
        }
        Utility.SetCheckedValueUltraCombobox(ultraCBOtherMaterials, item.OtherMaterial);
        txtWidth.Text = DBConvert.ParseString(item.WidthDefault);
        txtDepth.Text = DBConvert.ParseString(item.DepthDefault);
        txtHeight.Text = DBConvert.ParseString(item.HighDefault);
        txtCOM.Text = DBConvert.ParseString(item.CustomerOwnMaterial);
        //Carcass Process
        string cmd = string.Format(@"SELECT CarcassProcessID FROM TblBOMItemBasic WHERE ItemCode = '{0}'", item.ItemCode);
        DataTable dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cmd);
        if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) > 0)
        {
          ucbLastCarcassProcess.Value = DBConvert.ParseInt(dt.Rows[0][0].ToString());
        }
        else
        {
          ucbLastCarcassProcess.Value = null;
        }
        //Tien edit formula
        double width = (double)item.WidthDefault + 100;
        double depth = (double)item.DepthDefault + 100;
        double high = (double)item.HighDefault + 120;
        double itemCBM = (width * depth * high) / 1000000000;
        txtItemCBM.Text = itemCBM.ToString("#,##0.0000");
        //Tien Add
        if (DBConvert.ParseDouble(item.EstCBM.ToString()) != double.MinValue)
        {
          txtItemCBM.Text = item.EstCBM.ToString("#,##0.0000");
        }

        //
        txtInchWidth.Text = Utility.ConverMilimetToInch(item.WidthDefault);
        txtInchDepth.Text = Utility.ConverMilimetToInch(item.DepthDefault);
        txtInchHeight.Text = Utility.ConverMilimetToInch(item.HighDefault);
        try
        {
          cmbKD.SelectedValue = item.KD;
        }
        catch { }
        try
        {
          cmbUnit.SelectedValue = item.Unit;
        }
        catch { }

        try
        {
          if (item.DesignerPid != long.MinValue)
          {
            ultCBDesigner.Value = item.DesignerPid;
          }
        }
        catch { }

        txtCBM.Text = DBConvert.ParseString(item.CBM);
        chkNoLevel2.Checked = (item.IsNoLevel2 == 1 ? true : false);
        this.itemKindGlobal = (item.ItemKind == int.MinValue ? 0 : item.ItemKind);
        ultraCBItemKind.Value = this.itemKindGlobal;
        ultraCBItemKind.ReadOnly = true;
        confirm = (item.Confirm != 0);
        if (confirm)
        {
          btnSave.Enabled = false;
          btnClear.Visible = false;
          chkConfirm.Enabled = false;
          chkConfirm.Checked = true;
          ultraCbItemReference.Enabled = false;
          btnReference.Enabled = false;
        }
      }
      this.canUpdate = ((!confirm) && (btnSave.Visible));
      this.LoadGird(this.itemCode);
      this.loadingData = false;
      this.NeedToSave = false;

      // Truong Add
      this.itemReference = string.Empty;
      chkCopyLevel2OfItemReference.Checked = false;
      chkCopyLevel2OfItemReference.Visible = false;
      // End
    }

    #endregion InnitData

    #region Save Data

    private DBParameter[] GetRDDItemInfoParams()
    {
      DBParameter[] inputParam = new DBParameter[30];
      string newItemCode = string.Format(@"{0}-{1}", txtPrefixCode.Text.Trim(), txtSuffixCode.Text.Trim());
      if (this.itemCode.Length == 0)
      {
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, newItemCode);
        inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }
      else
      {
        inputParam[0] = new DBParameter("@OldItemCode", DbType.AnsiString, 16, this.itemCode);
        inputParam[1] = new DBParameter("@NewItemCode", DbType.AnsiString, 16, newItemCode);
        inputParam[2] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }
      inputParam[3] = new DBParameter("@SaleCode", DbType.AnsiString, 64, txtSaleCode.Text.Trim());
      string text = Utility.GetSelectedValueUltraCombobox(cmbCarcassCode);
      if (text.Length > 0)
      {
        inputParam[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, text);
      }
      text = txtItemName.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[5] = new DBParameter("@Name", DbType.String, 512, text);
      }
      text = txtDescription.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[6] = new DBParameter("@Description", DbType.String, 1024, text);
      }
      int number = int.MinValue;
      if (cmbCategory.Value != null)
      {
        number = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(cmbCategory));
        inputParam[7] = new DBParameter("@Category", DbType.Int32, number);
      }
      if (cmbCollection.Value != null)
      {
        number = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(cmbCollection));
        if (number != int.MinValue)
        {
          inputParam[8] = new DBParameter("@Collection", DbType.Int32, number);
        }
      }
      double dimension = DBConvert.ParseDouble(txtWidth.Text);
      if (dimension != double.MinValue)
      {
        inputParam[9] = new DBParameter("@WidthDefault", DbType.Double, dimension);
      }
      dimension = DBConvert.ParseDouble(txtDepth.Text);
      if (dimension != double.MinValue)
      {
        inputParam[10] = new DBParameter("@DepthDefault", DbType.Double, dimension);
      }
      dimension = DBConvert.ParseDouble(txtHeight.Text);
      if (dimension != double.MinValue)
      {
        inputParam[11] = new DBParameter("@HighDefault", DbType.Double, dimension);
      }

      if (cmbFinishingCode.Value != null)
      {
        text = Utility.GetSelectedValueUltraCombobox(cmbFinishingCode);
        inputParam[12] = new DBParameter("@MainFinish", DbType.AnsiString, 16, text);
      }

      if (cmbMainMaterials.Value != null)
      {
        text = Utility.GetSelectedValueUltraCombobox(cmbMainMaterials);
        inputParam[13] = new DBParameter("@MainMaterial", DbType.AnsiString, 16, text);
      }

      if (ultraCBOtherMaterials.Value != null)
      {
        text = Utility.GetCheckedValueUltraCombobox(ultraCBOtherMaterials);
        inputParam[14] = new DBParameter("@OtherMaterial", DbType.AnsiString, 128, text);
      }
      inputParam[15] = new DBParameter("@Confirm", DbType.Int32, (chkConfirm.Checked) ? 1 : 0);

      if (cmbKD.SelectedValue != null)
      {
        number = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbKD));
        inputParam[16] = new DBParameter("@KD", DbType.Int32, number);
      }

      if (cmbUnit.SelectedValue != null)
      {
        text = Utility.GetSelectedValueCombobox(cmbUnit);
        inputParam[17] = new DBParameter("@Unit", DbType.AnsiString, 8, text);
      }
      text = txtShortName.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[18] = new DBParameter("@ShortName", DbType.AnsiString, 512, text);
      }
      int isNoLevel2 = (chkNoLevel2.Checked ? 1 : 0);
      inputParam[19] = new DBParameter("@IsNoLevel2", DbType.Int32, isNoLevel2);
      if (ultraCBCustomer.Value != null)
      {
        long customerPid = DBConvert.ParseLong(ultraCBCustomer.Value.ToString());
        inputParam[20] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }
      if (ultraCBExhibition.Value != null)
      {
        long exhibition = DBConvert.ParseInt(ultraCBExhibition.Value);
        inputParam[21] = new DBParameter("@Exhibition", DbType.Int32, exhibition);
      }
      inputParam[22] = new DBParameter("@ItemKind", DbType.Int32, this.itemKindGlobal);
      if (txtCOM.Text.Trim().Length > 0)
      {
        inputParam[23] = new DBParameter("@COM", DbType.Double, txtCOM.Text.Trim());
      }
      if (ultCBDesigner.Value != null)
      {
        inputParam[24] = new DBParameter("@DesignerPid", DbType.Int64, DBConvert.ParseLong(ultCBDesigner.Value.ToString()));
      }
      if (DBConvert.ParseDouble(txtItemCBM.Text.Trim()) != double.MinValue)
      {
        inputParam[25] = new DBParameter("@EstCBM", DbType.Double, DBConvert.ParseDouble(txtItemCBM.Text.Trim()));
      }
      if (txtRDNote.Text.Trim().Length > 0)
      {
        inputParam[26] = new DBParameter("@RDNote", DbType.AnsiString, 512, txtRDNote.Text.Trim());
      }
      if (txtOldCode.Text.Trim().Length > 0)
      {
        inputParam[27] = new DBParameter("@OldCode", DbType.AnsiString, 32, txtOldCode.Text.Trim());
      }
      if (ucbLastCarcassProcess.Value != null)
      {
        inputParam[28] = new DBParameter("@CarcassProcessID", DbType.Int32, DBConvert.ParseInt(ucbLastCarcassProcess.Value.ToString()));
      }
      // Other Finishing
      string otherfinishings = string.Empty;
      for (int i = 0; i < ultOtherFinishCode.Rows.Count; i++)
      {
        string finishName = ultOtherFinishCode.Rows[i].Cells["Name"].Value.ToString();
        if (otherfinishings.Length > 0)
        {
          otherfinishings = string.Format("{0}, ", otherfinishings);
        }
        otherfinishings = string.Format("{0}{1}", otherfinishings, finishName);
      }
      if (otherfinishings.Length > 0)
      {
        inputParam[29] = new DBParameter("@OtherFinish", DbType.AnsiString, 128, otherfinishings);
      }

      return inputParam;
    }

    private DBParameter[] GetRDDMoreDimensionParams(DataRow dtRow)
    {
      DBParameter[] inputParam = new DBParameter[4];
      int pid = DBConvert.ParseInt(dtRow["Pid"].ToString());
      int dimensionKind = DBConvert.ParseInt(dtRow["DimensionKind"].ToString());
      int value = DBConvert.ParseInt(dtRow["mm"].ToString());
      if (pid != int.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
      }
      inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      if (dimensionKind == int.MinValue)
      {
        return null;
      }
      inputParam[2] = new DBParameter("@DimensionKind", DbType.Int32, dimensionKind);
      if (value != int.MinValue)
      {
        inputParam[3] = new DBParameter("@Values", DbType.Int32, value);
      }
      return inputParam;
    }

    private DBParameter[] GetRDDOtherFinishingDeleteParams(DataRow dtRow)
    {
      DBParameter[] inputParam = new DBParameter[4];
      long pid = DBConvert.ParseLong(dtRow["Pid"].ToString());
      if (pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
      }
      inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      string otherFinishingCode = dtRow["OtherFinishingCode"].ToString();
      if (otherFinishingCode.Length == 0)
      {
        return null;
      }
      inputParam[2] = new DBParameter("@OtherFinishingCode", DbType.AnsiString, 16, otherFinishingCode);
      string description = dtRow["Description"].ToString();
      inputParam[3] = new DBParameter("@Description", DbType.String, 256, description);
      return inputParam;
    }

    private bool SaveProductionRelative()
    {
      bool success = true;
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, -1) };
      //1. Delete
      foreach (long pid in this.listDeletedProductionRelative)
      {
        DBParameter[] input = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spBOMProductionRelative_Delete", input, output);
        if ((output == null) || (output[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      //2. Insert/Update
      DataTable dtDetail = (DataTable)ultraGridProductionRelative.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[5];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          string itemRelative = row["Relative"].ToString();
          string description = row["Description"].ToString().Trim();
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
          inputParam[2] = new DBParameter("@ItemRelative", DbType.AnsiString, 16, itemRelative);
          if (description.Length > 0)
          {
            inputParam[3] = new DBParameter("@Description", DbType.String, 256, description);
          }
          inputParam[4] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spBOMProductionRelative_Edit", inputParam, output);
          if ((output == null) || (output[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveData()
    {
      // Save RDDItemInfo
      bool result = true;
      DBParameter[] inputParam = this.GetRDDItemInfoParams();
      string storeName = string.Empty;
      if (this.itemCode.Length == 0)
      {
        storeName = "spRDDItemInfo_Insert";
      }
      else
      {
        storeName = "spRDDItemInfo_Update";
      }
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      long outputResult = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
      if (outputResult != 1)
      {
        return false;
      }
      this.itemCode = string.Format(@"{0}-{1}", txtPrefixCode.Text.Trim(), txtSuffixCode.Text.Trim());

      // 1. Save RDDMoreDimension
      //Truong hop reference item
      if (this.addReferenceItem)
      {
        //Delete Old MoreDimension
        string deleteCommandText = string.Format("Delete From TblRDDMoreDimension Where ItemCode = '{0}'", this.itemCode);
        result = DataBaseAccess.ExecuteCommandText(deleteCommandText);
        deleteCommandText = string.Format("Delete From TblBOMMoreDimension Where ItemCode = '{0}' AND Revision = 0", this.itemCode);
        result = DataBaseAccess.ExecuteCommandText(deleteCommandText);
      }
      // 1.1 Delete
      foreach (long pid in this.listDeletedMoreDimension)
      {
        DBParameter[] inputParamDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spRDDMoreDimension_Delete", inputParamDelete);
      }
      // 1.2 Insert/Update      
      foreach (DataRow dtRow in this.moreDimensionSource.Rows)
      {
        storeName = string.Empty;
        //Truong hop reference item
        if (this.addReferenceItem && dtRow.RowState != DataRowState.Deleted)
        {
          storeName = "spRDDMoreDimension_Insert";
        }
        else
        {
          if (dtRow.RowState == DataRowState.Added)
          {
            storeName = "spRDDMoreDimension_Insert";
          }
          else if (dtRow.RowState == DataRowState.Modified)
          {
            // Update
            storeName = "spRDDMoreDimension_Update";
          }
        }
        if (storeName.Length > 0)
        {
          DBParameter[] inputParamDetail = this.GetRDDMoreDimensionParams(dtRow);
          if (inputParamDetail != null)
          {
            DBParameter[] outputParamDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, -1) };
            Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamDetail, outputParamDetail);
            outputResult = DBConvert.ParseInt(outputParamDetail[0].Value.ToString());
            if (outputResult == 0)
            {
              result = false;
              break;
            }
          }
        }
      }


      // 2. Save OtherFinishing
      //Truong hop reference item
      if (this.addReferenceItem)
      {
        //Delete Old OtherFinishing        
        string deleteCommandText = string.Format("Delete From TblBOMOtherFinishing Where ItemCode = '{0}' AND Revision = 0", this.itemCode);
        result = DataBaseAccess.ExecuteCommandText(deleteCommandText);
      }
      // 2.1 Delete
      foreach (long pid in this.listDeletedOtherFinishingCode)
      {
        DBParameter[] inputParamParamOtherFinishingDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spRDDOtherFinishing_Delete", inputParamParamOtherFinishingDelete);
      }
      // 2.2 Insert/Update
      foreach (DataRow dtRow in this.otherFinishingSource.Rows)
      {
        storeName = string.Empty;
        //Truong hop reference item
        if (this.addReferenceItem && dtRow.RowState != DataRowState.Deleted)
        {
          storeName = "spRDDOtherFinishing_Insert";
        }
        else
        {
          if (dtRow.RowState == DataRowState.Added)
          {
            storeName = "spRDDOtherFinishing_Insert";
          }
          else if (dtRow.RowState == DataRowState.Modified)
          {
            // Update
            storeName = "spRDDOtherFinishing_Update";
          }
        }
        if (storeName.Length > 0)
        {
          DBParameter[] inputParamOtherFinishing = this.GetRDDOtherFinishingDeleteParams(dtRow);
          if (inputParamOtherFinishing != null)
          {
            DBParameter[] outputParamOtherFinishing = new DBParameter[] { new DBParameter("@Result", DbType.Int64, -1) };
            Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamOtherFinishing, outputParamOtherFinishing);
            outputResult = DBConvert.ParseInt(outputParamOtherFinishing[0].Value.ToString());
            if (outputResult == 0)
            {
              result = false;
              break;
            }
          }
        }
      }

      //3. Save Room
      //3.1. Truong hop reference item
      if (this.addReferenceItem)
      {
        //Delete Old Item Room
        string deleteCommandText = string.Format("Delete From TblRDDItemRoom Where ItemCode = '{0}'", this.itemCode);
        result = DataBaseAccess.ExecuteCommandText(deleteCommandText);
        deleteCommandText = string.Format("Delete From TblCSDItemRoom Where ItemCode = '{0}'", this.itemCode);
        result = DataBaseAccess.ExecuteCommandText(deleteCommandText);
      }
      //3.2. Delete room
      foreach (long pid in this.listDeletedRoom)
      {
        DBParameter[] inputParamDeletedRoom = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParamDeletedRoom = new DBParameter[] { new DBParameter("@Result", DbType.Int64, -1) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spRDDItemRoom_Delete", inputParamDeletedRoom, outputParamDeletedRoom);
      }
      //3.2. Insert/Update room
      foreach (DataRow dtRow in this.dtRoomList.Rows)
      {
        DBParameter[] inputParamRoom = new DBParameter[4];
        storeName = string.Empty;
        //Truong hop reference item
        if (this.addReferenceItem && dtRow.RowState != DataRowState.Deleted)
        {
          storeName = "spRDDItemRoom_Insert";
        }
        else
        {
          if (dtRow.RowState == DataRowState.Added)
          {
            storeName = "spRDDItemRoom_Insert";
          }
          else if (dtRow.RowState == DataRowState.Modified)
          {
            // Update
            inputParamRoom[0] = new DBParameter("@Pid", DbType.Int64, dtRow["Pid"]);
            storeName = "spRDDItemRoom_Update";
          }
        }
        if (storeName.Length > 0)
        {
          inputParamRoom[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
          inputParamRoom[2] = new DBParameter("@RoomPid", DbType.Int64, dtRow["RoomPid"]);
          if (dtRow["Description"].ToString().Trim().Length > 0)
          {
            inputParamRoom[3] = new DBParameter("@Description", DbType.String, 256, dtRow["Description"]);
          }
          DBParameter[] outputParamRoom = new DBParameter[] { new DBParameter("@Result", DbType.Int64, -1) };
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamRoom, outputParamRoom);
          outputResult = DBConvert.ParseLong(outputParamRoom[0].Value.ToString());
          if (outputResult == 0)
          {
            result = false;
            break;
          }
        }
      }

      //4. Save production relationship
      if (!this.SaveProductionRelative())
      {
        result = false;
      }

      // Truong Add
      if (result == true && chkCopyLevel2OfItemReference.Checked && ultraCbItemReference.SelectedRow != null)
      {
        result = this.CopyLevel2OfItemRefence();
      }
      // End
      Utility.SynchronizeFinishing(this.itemCode, 0);
      return result;
    }
    /// <summary>
    /// Copy Level 2 Of Reference Item
    /// </summary>
    /// <returns></returns>
    private bool CopyLevel2OfItemRefence()
    {
      //Old
      /*
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@ItemReference", DbType.AnsiString, 16, this.itemReference);
      inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spRDDCopyLevel2OfReferenceItem", inputParam, outputParam);
      if (DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
      {
        return false;
      }
      return true;
      */
      //Tien edit
      string referenceItem = ultraCbItemReference.SelectedRow.Cells["ItemCode"].Value.ToString();
      int referenceRevision = DBConvert.ParseInt(ultraCbItemReference.SelectedRow.Cells["Revision"].Value.ToString());
      DBParameter[] inputReference = new DBParameter[5];
      DBParameter[] outputReference = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      inputReference[0] = new DBParameter("@FromItemCode", DbType.AnsiString, 16, referenceItem);
      inputReference[1] = new DBParameter("@FromRevision", DbType.Int32, referenceRevision);
      inputReference[2] = new DBParameter("@ToItemCode", DbType.AnsiString, 16, this.itemCode);
      inputReference[3] = new DBParameter("@ToRevision", DbType.Int32, 0);
      inputReference[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DataBaseAccess.ExecuteStoreProcedure("spBOMCopyLevel2OfItem", inputReference, outputReference);
      if (outputReference[0].Value.ToString() == "0")
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Clear Input Information
    /// </summary>
    private void ClearInformation()
    {
      this.itemCode = string.Empty;
      ultraCBItemKind.Value = null;
      txtOldCode.Text = string.Empty;
      txtRDNote.Text = string.Empty;
      multiCBPrefixCode.Enabled = true;
      ultraCBItemKind.ReadOnly = false;
      ultraCbItemReference.Enabled = true;
      cmbCategory.ReadOnly = false;
      cmbCollection.ReadOnly = false;
      cmbCarcassCode.ReadOnly = false;
      cmbFinishingCode.ReadOnly = false;
      cmbMainMaterials.ReadOnly = false;
      ultraCBOtherMaterials.ReadOnly = false;
      btnGetCode.Enabled = true;
      cmbKD.Enabled = true;
      cmbUnit.Enabled = true;
      chkNoLevel2.Enabled = true;
      multiCBPrefixCode.SelectedIndex = -1;

      //Tien add

      ultraCbItemReference.Value = null;
      chkConfirm.Checked = false;
      chkConfirm.Enabled = true;
      btnSave.Enabled = true;

      txtPrefixCode.Text = string.Empty;
      txtSuffixCode.Text = string.Empty;
      txtSaleCode.Text = string.Empty;
      txtItemName.Text = string.Empty;
      txtShortName.Text = string.Empty;
      ultraCBCustomer.Value = null;
      ultraCBExhibition.Value = null;
      txtDescription.Text = null;
      cmbCategory.Value = null;
      cmbCollection.Value = null;
      cmbCarcassCode.Value = null;
      cmbFinishingCode.Value = null;
      ucbLastCarcassProcess.Value = null;
      cmbMainMaterials.Value = null;
      ucbLastCarcassProcess.Value = null;
      Utility.SetCheckedValueUltraCombobox(ultraCBOtherMaterials, string.Empty);
      txtWidth.Text = string.Empty;
      txtDepth.Text = string.Empty;
      txtHeight.Text = string.Empty;
      txtCOM.Text = string.Empty;
      txtInchDepth.Text = string.Empty;
      txtInchWidth.Text = string.Empty;
      txtInchHeight.Text = string.Empty;
      txtItemCBM.Text = string.Empty;
      cmbKD.SelectedIndex = -1;
      cmbUnit.SelectedIndex = -1;
      txtCBM.Text = string.Empty;
      chkNoLevel2.Checked = false;
      image.ImageLocation = string.Empty;
      DataTable dtRoom = (DataTable)ultraGridRoom.DataSource;
      dtRoom.Clear();

      DBParameter[] param = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, string.Empty) };
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spRDDListMoreDimensionByItemCode", param);
      ultMoreDimension.DataSource = dtSource;
      dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spRDDListOtherFinishingByItemCode", param);
      ultOtherFinishCode.DataSource = dtSource;
      dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spBOMProductionRelative_Select", param);
      ultraGridProductionRelative.DataSource = dtSource;
      ultraGridSaleRelative.DataSource = null;
      this.NeedToSave = false;
    }

    #endregion Save Data

    #region Check Data

    private bool CheckRelationCode(string code)
    {
      string strCommandText = string.Format(@"SELECT Count(*) FROM  TblRDDItemInfo Where ItemCode <> '{0}' AND ItemCode = @Code", this.itemCode);
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@Code", DbType.AnsiString, 16, code) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) >= 1);
    }

    private bool CheckFinishingCode(string code)
    {
      string strCommandText = @"SELECT Count(FinCode) FROM  TblBOMFinishingInfo WHERE FinCode = @Code AND DeleteFlag = 0"; // Qui, 30/10/2010 drop condition 'Confirm <> 0'
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@Code", DbType.AnsiString, 64, code) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) >= 1);
    }

    private bool CheckDimensionKind(string code)
    {
      string strCommandText = string.Format(@"SELECT Count(*) FROM  TblBOMCodeMaster WHERE [Group] = {0} AND Value = @Code", ConstantClass.GROUP_MOREDIMENSION);
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@Code", DbType.AnsiString, 64, code) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) >= 1);
    }

    private bool checkBeforeSave()
    {
      //item code
      string prefix = txtPrefixCode.Text.Trim();
      if (prefix.Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Item Code");
        txtPrefixCode.Focus();
        return false;
      }
      string suffix = txtSuffixCode.Text.Trim();
      if (suffix.Length != 2)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Item Code");
        txtSuffixCode.Focus();
        return false;
      }
      else if (this.itemKindGlobal == 3 && suffix.Substring(0, 1) != "W")
      {
        WindowUtinity.ShowMessageError("ERR0001", "Item Code");
        txtSuffixCode.Focus();
        return false;
      }
      // Check Sale Code
      string itemCode = string.Format("{0}-{1}", prefix, suffix);
      string saleCode = txtSaleCode.Text.Trim();
      if (saleCode.Length > 0)
      {
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@SaleCode", DbType.AnsiString, 64, saleCode);
        string commandTextSaleCode = "Select dbo.FBOMCheckSaleCode(@ItemCode, @SaleCode)";
        int countSaleCode = (int)DataBaseAccess.ExecuteScalarCommandText(commandTextSaleCode, inputParam);

        if (countSaleCode > 0)
        {
          WindowUtinity.ShowMessageError("MSG0006", "Sale Code");
          txtSaleCode.Focus();
          return false;
        }
      }
      //item name      
      if (txtItemName.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Item Name");
        txtItemName.Focus();
        return false;
      }
      //Category
      if (cmbCategory.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Category");
        cmbCategory.Focus();
        return false;
      }
      //Customer
      if (ultraCBCustomer.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Customer");
        ultraCBCustomer.Focus();
        return false;
      }


      //HP Exhibition
      //if (ultraCBExhibition.SelectedRow == null)
      //{
      //  WindowUtinity.ShowMessageError("ERR0001", "HP Exhibition");
      //  ultraCBExhibition.Focus();
      //  return false;
      //}
      //Collection
      //if (cmbCollection.SelectedRow == null)
      //{
      //  WindowUtinity.ShowMessageError("ERR0001", "Collection");
      //  cmbCollection.Focus();
      //  return false;
      //}
      //Carcass
      if (cmbCarcassCode.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Carcass");
        cmbCarcassCode.Focus();
        return false;
      }
      //Main Finishing
      if (cmbFinishingCode.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Main Finishing");
        cmbFinishingCode.Focus();
        return false;
      }
      //item size
      double dimension = DBConvert.ParseDouble(txtWidth.Text.Trim());
      if (dimension < 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Width");
        txtWidth.Focus();
        return false;
      }
      dimension = DBConvert.ParseDouble(txtDepth.Text.Trim());
      if (dimension < 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Depth");
        txtDepth.Focus();
        return false;
      }
      dimension = DBConvert.ParseDouble(txtHeight.Text.Trim());
      if (dimension < 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Height");
        txtHeight.Focus();
        return false;
      }
      // Check COM
      if (txtCOM.Text.Trim().Length > 0)
      {
        double com = DBConvert.ParseDouble(txtCOM.Text);
        if (com < 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "COM");
          txtCOM.Focus();
          return false;
        }
      }

      // check more dimension
      DataTable dtMoreDimension = (DataTable)this.ultMoreDimension.DataSource;
      foreach (DataRow drMoreDimension in dtMoreDimension.Rows)
      {
        if (drMoreDimension.RowState != DataRowState.Deleted)
        {
          if (drMoreDimension["DimensionKind"].ToString().Trim().Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Dimension Kind");
            this.ultMoreDimension.Focus();
            return false;
          }

          if (drMoreDimension["mm"].ToString().Trim().Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "mm");
            this.ultMoreDimension.Focus();
            return false;
          }
        }
      }
      // Kiem tra save data truoc khi confirm confirm
      string code = string.Empty;
      if (this.itemCode.Length == 0)
      {
        code = string.Format(@"{0}-{1}", txtPrefixCode.Text.Trim(), txtSuffixCode.Text.Trim());
      }
      else
      {
        code = this.itemCode;
      }
      string commandText = string.Format("Select Count(ComponentCode) From TblRDDItemComponent Where ItemCode = '{0}'", code);
      int count = (int)DataBaseAccess.ExecuteScalarCommandText(commandText);
      if ((chkConfirm.Checked) && (!chkNoLevel2.Checked) && (count == 0))
      {
        WindowUtinity.ShowMessageError("ERR0060");
        return false;
      }

      //kiểm tra carcass code có được sử dụng chưa, nếu đã có báo lỗi
      string cmd = string.Format(@"SELECT COUNT(ItemCode)
                              FROM TblRDDItemInfo A
                              WHERE CarcassCode = '{0}' AND ItemCode <> '{1}'", cmbCarcassCode.Value.ToString(), code);
      int countCarcassExist = (int)DataBaseAccess.ExecuteScalarCommandText(cmd);
      if (countCarcassExist > 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Carcass Code has been used.");
        return false;
      }

      return true;
    }

    private void checkChangeDataBeforeClose()
    {
      if (this.loadingData)
      {
        return;
      }
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }
    /// <summary>
    /// Check Double
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckFinishCodeDuplicate()
    {
      isDuplicate = false;
      for (int k = 0; k < ultOtherFinishCode.Rows.Count; k++)
      {
        ultOtherFinishCode.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultOtherFinishCode.Rows.Count; i++)
      {
        string finishcode = ultOtherFinishCode.Rows[i].Cells["OtherFinishingCode"].Value.ToString();
        for (int j = i + 1; j < ultOtherFinishCode.Rows.Count; j++)
        {
          string finishcodeCompare = ultOtherFinishCode.Rows[j].Cells["OtherFinishingCode"].Value.ToString();
          if (string.Compare(finishcode, finishcodeCompare, true) == 0)
          {
            ultOtherFinishCode.Rows[i].CellAppearance.BackColor = Color.Yellow;
            ultOtherFinishCode.Rows[j].CellAppearance.BackColor = Color.Yellow;
            this.isDuplicate = true;
          }
        }
      }
    }
    #endregion Check Data

    #region UltraGrid Events

    private void ultMoreDimension_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "dimensionkind":
          string code = e.Cell.Text.Trim();
          if (!this.CheckDimensionKind(code))
          {
            WindowUtinity.ShowMessageError("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultMoreDimension_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.checkChangeDataBeforeClose();
      string message = string.Empty;
      string columnName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "dimensionkind":
          try
          {
            ultMoreDimension.Rows[index].Cells["Description"].Value = udrpDimensionKind.SelectedRow.Cells["Description"].Value;
          }
          catch
          {
            ultMoreDimension.Rows[index].Cells["Description"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    private void ultMoreDimension_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingMoreDimension = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingMoreDimension.Add(pid);
        }
      }
    }

    private void ultMoreDimension_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.checkChangeDataBeforeClose();
      foreach (long pid in this.listDeletingMoreDimension)
      {
        this.listDeletedMoreDimension.Add(pid);
      }
    }

    private void ultOtherFinishCode_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.checkChangeDataBeforeClose();
      foreach (long pid in this.listDeletingOtherFinishingCode)
      {
        this.listDeletedOtherFinishingCode.Add(pid);
      }
    }

    private void ultOtherFinishCode_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      switch (columnName)
      {
        case "otherfinishingcode":
          e.Cell.Row.Cells["Name"].Value = ultraCBOtherFinishing.SelectedRow.Cells["FinName"].Value;
          break;
        default:
          break;
      }
      this.checkChangeDataBeforeClose();
      this.CheckFinishCodeDuplicate();
    }

    private void ultOtherFinishCode_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "otherfinishingcode":
          string code = e.NewValue.ToString();
          if (!this.CheckFinishingCode(code))
          {
            WindowUtinity.ShowMessageError("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }

    }

    private void ultOtherFinishCode_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingOtherFinishingCode = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingOtherFinishingCode.Add(pid);
        }
      }
    }

    private void ultraGridSaleRelative_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RelativeItem"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
    }

    private void ultraGridProductionRelative_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemRelative"].Hidden = true;
      e.Layout.Bands[0].Columns["Relative"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["SaleCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Relative"].ValueList = ultraDDProductionItem;
      e.Layout.Bands[0].Columns["Relative"].MinWidth = 85;
      e.Layout.Bands[0].Columns["Relative"].MaxWidth = 85;
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 80;
    }

    private void ultraGridProductionRelative_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "relative":
          string code = e.Cell.Text.Trim();
          if (!this.CheckRelationCode(code))
          {
            WindowUtinity.ShowMessageError("ERR0001", "Relative Item");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultraGridProductionRelative_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "relative":
          if (ultraDDProductionItem.SelectedRow != null)
          {
            e.Cell.Row.Cells["SaleCode"].Value = ultraDDProductionItem.SelectedRow.Cells["SaleCode"].Value;
            e.Cell.Row.Cells["Name"].Value = ultraDDProductionItem.SelectedRow.Cells["Name"].Value;
          }
          break;
        default:
          break;
      }
      this.checkChangeDataBeforeClose();
    }

    private void ultraGridProductionRelative_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listDeletedProductionRelative.Add(pid);
        }
      }
      this.checkChangeDataBeforeClose();
    }
    #endregion UltraGrid Events

    #region Other Events
    //Tien Add
    private void ultraCbItemReference_ValueChanged(object sender, EventArgs e)
    {
      if (this.loadingReference)
      {
        return;
      }
      if (ultraCbItemReference.SelectedRow != null)
      {
        string referenceCode = Utility.GetSelectedValueUltraCombobox(ultraCbItemReference);
        btnReference.Enabled = (referenceCode.Length > 0);
        chkCopyLevel2OfItemReference.Visible = (referenceCode.Length > 0);
      }
    }

    private DataTable GetItemReferenceInfo(string itemCode, int revision)
    {
      string commandText = string.Format(@"SELECT	BS.ShortName, BS.Name, BS.[Description], BS.CustomerPid, BS.Exhibition, BS.Category, BS.[Collection], 
		                                              INF.CarcassCode, INF.MainFinish, INF.MainMaterial, INF.OtherMaterial, INF.Width, INF.Depth, INF.High,
		                                              INF.KD, INF.Unit, INF.CBM, INF.IsNoLevel2, BS.CustomerOwnMaterial COM
                                           FROM	  TblBOMItemInfo INF
	                                                INNER JOIN TblBOMItemBasic BS ON INF.ItemCode = BS.ItemCode
                                           WHERE	INF.ItemCode = '{0}' AND INF.Revision = {1}", itemCode, revision);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      return dtSource;
    }

    private void btnReference_Click(object sender, EventArgs e)
    {
      this.addReferenceItem = true;
      string referenceCode = Utility.GetSelectedValueUltraCombobox(ultraCbItemReference);
      // Truong Add // Tien Edit load theo item and revision
      this.itemReference = ultraCbItemReference.SelectedRow.Cells["ItemCode"].Value.ToString();
      int revisionReference = DBConvert.ParseInt(ultraCbItemReference.SelectedRow.Cells["Revision"].Value.ToString());
      // End
      DialogResult confirm = WindowUtinity.ShowMessageConfirm("MSG0010", referenceCode);
      if (confirm == DialogResult.Yes)
      {
        DataTable dtItemRef = this.GetItemReferenceInfo(itemReference, revisionReference);
        if (dtItemRef == null || dtItemRef != null && dtItemRef.Rows.Count == 0)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          return;
        }
        txtShortName.Text = dtItemRef.Rows[0]["ShortName"].ToString();
        txtItemName.Text = dtItemRef.Rows[0]["Name"].ToString();
        txtDescription.Text = dtItemRef.Rows[0]["Description"].ToString();
        long cusPid = DBConvert.ParseLong(dtItemRef.Rows[0]["CustomerPid"].ToString());
        if (cusPid != long.MinValue)
        {
          ultraCBCustomer.Value = cusPid;
        }
        int exhibition = DBConvert.ParseInt(dtItemRef.Rows[0]["Exhibition"].ToString());
        if (exhibition != int.MinValue)
        {
          ultraCBExhibition.Value = exhibition;
        }
        long category = DBConvert.ParseLong(dtItemRef.Rows[0]["Category"].ToString());
        if (category != long.MinValue)
        {
          cmbCategory.Value = category;
        }
        int collection = DBConvert.ParseInt(dtItemRef.Rows[0]["Collection"].ToString());
        if (collection != int.MinValue)
        {
          cmbCollection.Value = collection;
        }
        if (dtItemRef.Rows[0]["CarcassCode"].ToString().Trim().Length > 0)
        {
          cmbCarcassCode.Value = dtItemRef.Rows[0]["CarcassCode"];
        }
        if (dtItemRef.Rows[0]["MainFinish"].ToString().Trim().Length > 0)
        {
          cmbFinishingCode.Value = dtItemRef.Rows[0]["MainFinish"];
        }
        int mainMaterial = DBConvert.ParseInt(dtItemRef.Rows[0]["MainMaterial"].ToString());
        if (mainMaterial != int.MinValue)
        {
          cmbMainMaterials.Value = mainMaterial;
        }
        string otherMaterial = dtItemRef.Rows[0]["OtherMaterial"].ToString().Trim();
        if (otherMaterial.Length > 0)
        {
          Utility.SetCheckedValueUltraCombobox(ultraCBOtherMaterials, otherMaterial);
        }
        int width = DBConvert.ParseInt(dtItemRef.Rows[0]["Width"].ToString());
        int depth = DBConvert.ParseInt(dtItemRef.Rows[0]["Depth"].ToString());
        int high = DBConvert.ParseInt(dtItemRef.Rows[0]["High"].ToString());
        txtWidth.Text = dtItemRef.Rows[0]["Width"].ToString();
        txtDepth.Text = dtItemRef.Rows[0]["Depth"].ToString();
        txtHeight.Text = dtItemRef.Rows[0]["High"].ToString();
        txtInchWidth.Text = FunctionUtility.ConverMilimetToInch(width);
        txtInchDepth.Text = FunctionUtility.ConverMilimetToInch(depth);
        txtInchHeight.Text = FunctionUtility.ConverMilimetToInch(high);
        txtCOM.Text = dtItemRef.Rows[0]["COM"].ToString();
        int kd = DBConvert.ParseInt(dtItemRef.Rows[0]["KD"].ToString());
        if (kd != int.MinValue)
        {
          cmbKD.SelectedValue = DBConvert.ParseInt(dtItemRef.Rows[0]["KD"].ToString());
        }
        if (dtItemRef.Rows[0]["Unit"].ToString().Trim().Length > 0)
        {
          cmbUnit.SelectedValue = dtItemRef.Rows[0]["Unit"];
        }
        txtCBM.Text = dtItemRef.Rows[0]["CBM"].ToString();
        int isNoLv2 = DBConvert.ParseInt(dtItemRef.Rows[0]["IsNoLevel2"].ToString());
        chkNoLevel2.Checked = (isNoLv2 == 1 ? true : false);
        this.LoadReferenceGird(referenceCode);
        // Truong Add
        if (this.itemReference.Trim().Length > 0)
        {
          chkCopyLevel2OfItemReference.Visible = true;
          chkCopyLevel2OfItemReference.Checked = true;
        }
        // End
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.btnSave_Click(null, null);
    }

    //private void btnCarcass_Click(object sender, EventArgs e)
    //{
    //  viewBOM_01_010 objCarcass = new viewBOM_01_010();
    //  WindowUtinity.ShowView(objCarcass, "New Carcass", false, ViewState.ModalWindow);
    //  Utility.LoadUltraCBCarcass(cmbCarcassCode);
    //  this.Show();
    //}

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.checkBeforeSave())
      {
        string message = string.Empty;
        bool success = this.SaveData();
        if (success)
        {
          message = "MSG0004";
          this.itemCode = string.Format(@"{0}-{1}", txtPrefixCode.Text.Trim(), txtSuffixCode.Text.Trim());
          WindowUtinity.ShowMessageSuccess(message);
          this.LoadData();
          this.SaveSuccess = true;
        }
        else
        {
          message = "ERR0005";
          FunctionUtility.UnlockRDDItemInfo(this.itemCode);
          WindowUtinity.ShowMessageError(message);
          this.SaveSuccess = false;
        }
      }
      else
      {
        this.SaveSuccess = false;
      }
    }

    private void lblComponent_Click(object sender, EventArgs e)
    {
      viewRDD_01_003 view = new viewRDD_01_003();
      view.itemCode = this.itemCode;
      WindowUtinity.ShowView(view, "Item Component List", false, ViewState.ModalWindow);
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.checkChangeDataBeforeClose();
    }

    private void ConvertToInch(object sender, EventArgs e)
    {
      if (this.loadingData)
      {
        return;
      }
      this.checkChangeDataBeforeClose();
      int value = DBConvert.ParseInt(((TextBox)sender).Text);
      string name = ((TextBox)sender).Name;
      switch (name)
      {
        case "txtWidth":
          txtInchWidth.Text = FunctionUtility.ConverMilimetToInch(value);
          break;
        case "txtDepth":
          txtInchDepth.Text = FunctionUtility.ConverMilimetToInch(value);
          break;
        case "txtHeight":
          txtInchHeight.Text = FunctionUtility.ConverMilimetToInch(value);
          break;
        default:
          break;
      }

      if (txtWidth.Text.Trim().Length > 0 && txtDepth.Text.Trim().Length > 0 && txtHeight.Text.Trim().Length > 0)
      {
        double width = DBConvert.ParseDouble(txtWidth.Text);
        double depth = DBConvert.ParseDouble(txtDepth.Text);
        double height = DBConvert.ParseDouble(txtHeight.Text);
        //Tien edit formula
        double itemCBM = ((width + 100) * (depth + 100) * (height + 120)) / 1000000000;
        txtItemCBM.Text = itemCBM.ToString("#,##0.0000");
      }
    }

    /// <summary>
    /// Get auto code or continue code from prefix code
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetCode_Click(object sender, EventArgs e)
    {
      string prefixCode = string.Empty;
      if (ultraCBItemKind.Value != null)
      {
        this.itemKindGlobal = DBConvert.ParseInt(ultraCBItemKind.Value.ToString());
        if (this.itemKindGlobal == 3)
        {
          txtSuffixCode.ReadOnly = false;
        }
        else
        {
          txtSuffixCode.ReadOnly = true;
        }
      }
      if (multiCBPrefixCode.SelectedIndex > 0)
      {
        prefixCode = multiCBPrefixCode.SelectedValue.ToString();
      }
      string commandExec = "Select dbo.FRDDGetAutoItemCode(@PrefixCode, @ItemKind)";
      DBParameter[] inputParam = new DBParameter[2];
      if (prefixCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@PrefixCode", DbType.AnsiString, 8, prefixCode);
      }
      else
      {
        inputParam[0] = new DBParameter("@PrefixCode", DbType.AnsiString, 8, DBNull.Value);
      }
      inputParam[1] = new DBParameter("@ItemKind", DbType.Int32, this.itemKindGlobal);
      string newItemCode = string.Empty;
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandExec, inputParam);
      if (obj != null)
      {
        newItemCode = obj.ToString();
      }
      if (newItemCode.Length > 0)
      {
        string[] arrCode = newItemCode.Split('-');
        if (arrCode.Length > 1)
        {
          txtPrefixCode.Text = arrCode[0];
          txtSuffixCode.Text = arrCode[1];
          return;
        }
      }
      txtPrefixCode.Text = string.Empty;
      txtSuffixCode.Text = string.Empty;
    }

    /// <summary>
    /// Print Sample Data Sheet
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      DataSet dsDataSheet = new Shared.DataSetSource.CustomerService.dsCSDSampleDataSheet();
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spRDDSampleDataSheetReport", inputParam);
      dsSource.Tables[0].Columns.Add("Picture", typeof(System.Byte[]));
      dsSource.Tables[1].Columns.Add("ComponentPicture", typeof(System.Byte[]));
      dsSource.Tables[2].Columns.Add("ComponentPicture", typeof(System.Byte[]));

      if (dsSource.Tables[0].Rows.Count > 0)
      {
        string imgPath = FunctionUtility.RDDGetItemImage(this.itemCode);
        dsSource.Tables[0].Rows[0]["Picture"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 1.02, "JPG");
      }

      foreach (DataRow rowGlass in dsSource.Tables[1].Rows)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemComponentImage(rowGlass["ComponentCode"].ToString().Trim());
          FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
          BinaryReader br = new BinaryReader(fs);
          byte[] imgbyte = new byte[fs.Length + 1];
          imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
          rowGlass["ComponentPicture"] = imgbyte;
          br.Close();
          fs.Close();
        }
        catch { }
      }

      foreach (DataRow row in dsSource.Tables[2].Rows)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemComponentImage(row["ComponentCode"].ToString().Trim());
          FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
          BinaryReader br = new BinaryReader(fs);
          byte[] imgbyte = new byte[fs.Length + 1];
          imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
          row["ComponentPicture"] = imgbyte;
          br.Close();
          fs.Close();
        }
        catch { }
      }

      if (dsSource.Tables.Count >= 3)
      {
        dsDataSheet.Tables["dtItemInfo"].Merge(dsSource.Tables[0]);
        dsDataSheet.Tables["dtItemGlass"].Merge(dsSource.Tables[1]);
        dsDataSheet.Tables["dtItemDetail"].Merge(dsSource.Tables[2]);
      }
      string commandText = string.Format(@"SELECT DMS.ItemCode, DimensionKind, MST.[Description], DMS.[Values] mm, dbo.FCSDConvert_mm_to_inches(DMS.[Values]) Inches
	                                            FROM TblRDDMoreDimension DMS
		                                            LEFT JOIN TblBOMCodeMaster MST ON (DMS.DimensionKind = MST.Code AND MST.[Group] = 5)
	                                            WHERE ItemCode = @ItemCode");
      DataTable dtMoreDimention = DataBaseAccess.SearchCommandTextDataTable(commandText, inputParam);
      if (dtMoreDimention != null && dtMoreDimention.Rows.Count > 0)
      {
        dsDataSheet.Tables["dtMoreDimention"].Merge(dtMoreDimention);
      }

      //ReportClass cpt = null;
      DaiCo.Shared.View_Report report = null;

      Shared.ReportTemplate.CustomerService.cptCSDSampleDataSheet cpt = new Shared.ReportTemplate.CustomerService.cptCSDSampleDataSheet();
      cpt.SetDataSource(dsDataSheet);
      cpt.Subreports["cptCSDSubGlass.rpt"].SetDataSource(dsDataSheet.Tables["dtItemGlass"]);
      cpt.Subreports["cptCSDSubComponent.rpt"].SetDataSource(dsDataSheet.Tables["dtItemDetail"]);
      cpt.Subreports["cptSubOtherDimention.rpt"].SetDataSource(dsDataSheet.Tables["dtMoreDimention"]);

      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(ViewState.Window, FormWindowState.Maximized);
    }

    /// <summary>
    /// btnClear Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearInformation();
      this.itemReference = string.Empty;
      chkCopyLevel2OfItemReference.Checked = false;
      chkCopyLevel2OfItemReference.Visible = false;
    }

    /// <summary>
    /// btnSaveAndContinue Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveAndContinue_Click(object sender, EventArgs e)
    {
      //if (this.checkBeforeSave())
      //{
      //string message = string.Empty;
      //bool success = this.SaveData();
      //if (success)
      //{
      //  message = "MSG0004";
      //  this.itemCode = string.Format(@"{0}-{1}", txtPrefixCode.Text.Trim(), txtSuffixCode.Text.Trim());
      //  WindowUtinity.ShowMessageSuccess(message);
      if (this.NeedToSave)
      {
        string messageConfirm = FunctionUtility.GetMessage("MSG0008");
        DialogResult dlgr = MessageBox.Show(messageConfirm, "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (dlgr == DialogResult.Yes)
        {
          if (this.checkBeforeSave())
          {
            bool success = this.SaveData();
            if (success)
            {
              this.ClearInformation();
              //this.itemCode = string.Empty;
              //this.LoadCombobox();
              //this.LoadData();
            }
          }
        }
        if (dlgr == DialogResult.No)
        {
          this.ClearInformation();
        }
      }
      else
      {
        this.ClearInformation();

      }
      this.LoadItemReference();
      //  this.LoadCombobox();
      //}
      //else
      //{
      //  message = "ERR0005";
      //  FunctionUtility.UnlockRDDItemInfo(this.itemCode);
      //  WindowUtinity.ShowMessageError(message);
      //  this.SaveSuccess = false;
      //}
      //}
      //else
      //{
      //  this.SaveSuccess = false;
      //}
    }

    private void ultraCBExhibition_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void chkSparePart_CheckedChanged(object sender, EventArgs e)
    {

    }

    private void ultraGridRoom_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.checkChangeDataBeforeClose();
    }

    private void ultraGridRoom_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (string.Compare(e.Cell.Column.ToString(), "RoomPid", true) == 0)
      {
        DataTable dtCheckRoom = (DataTable)ultraDDRoom.DataSource;
        if (dtCheckRoom.Select(string.Format("Room = '{0}'", e.Cell.Text)).Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Room");
          e.Cancel = true;
        }
      }
    }
    private void ultraGridRoom_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RoomPid"].ValueList = ultraDDRoom;
      e.Layout.Bands[0].Columns["RoomPid"].Header.Caption = "Room";
      e.Layout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
    }

    private void ultraGridRoom_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listDeletedRoom.Add(pid);
        }
      }
      this.checkChangeDataBeforeClose();
    }

    private void ultraDDRoom_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
    }

    private void btnExportexel_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode) };
      DataSet dtset = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spRDDSampleDataSheetExportExel", inputParam);
      string strTemplateName = "RDcostingdatasheet";
      string strSheetName = "RD2";
      string strOutFileName = "RDcostingdatasheet";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\ResearchAndDesign";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);
      oXlsReport.Cell("**CusName").Value = dtset.Tables[0].Rows[0]["CusName"].ToString();
      oXlsReport.Cell("**ItemCode").Value = dtset.Tables[0].Rows[0]["ItemCode"].ToString();
      oXlsReport.Cell("**CustCode").Value = dtset.Tables[0].Rows[0]["SaleCode"].ToString();
      oXlsReport.Cell("**Item").Value = dtset.Tables[0].Rows[0]["ItemName"].ToString();
      oXlsReport.Cell("**Description").Value = dtset.Tables[0].Rows[0]["Description"].ToString();
      oXlsReport.Cell("**Width").Value = dtset.Tables[0].Rows[0]["WidthDefault"].ToString();
      oXlsReport.Cell("**Depth").Value = dtset.Tables[0].Rows[0]["DepthDefault"].ToString();
      oXlsReport.Cell("**Height").Value = dtset.Tables[0].Rows[0]["HighDefault"].ToString();
      oXlsReport.Cell("**MainMaterial").Value = dtset.Tables[0].Rows[0]["MainMaterial"].ToString();
      oXlsReport.Cell("**OtherMaterial").Value = dtset.Tables[0].Rows[0]["OtherMaterial"].ToString();
      oXlsReport.Cell("**MainFinish").Value = dtset.Tables[0].Rows[0]["MainFinish"].ToString();
      oXlsReport.Cell("**OtherFinish").Value = dtset.Tables[0].Rows[0]["OtherFinishing"].ToString();
      oXlsReport.Cell("**Date").Value = "Date : " + DateTime.Now.ToString();
      for (int i = 0; i < dtset.Tables[1].Rows.Count; i++)
      {
        DataRow dtRow = dtset.Tables[1].Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("A33:C33").Copy();
          oXlsReport.RowInsert(32 + i);
          oXlsReport.Cell("A33:C33", 0, i).Paste();
        }
        for (int c = 0; c < dtset.Tables[1].Columns.Count; c++)
        {
          oXlsReport.Cell("**Name", 0, i).Value = dtRow["Name"];
          oXlsReport.Cell("**Code", 0, i).Value = dtRow["ComponentCode"];
          oXlsReport.Cell("**Qty", 0, i).Value = dtRow["Qty"];
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void ultOtherFinishCode_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      // Set auto complete combo in grid
      e.Layout.Bands[0].Columns["OtherFinishingCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["OtherFinishingCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      // Hidden Column
      e.Layout.Bands[0].Columns["Name"].Hidden = true;
    }

    private void cmbFinishingCode_ValueChanged(object sender, EventArgs e)
    {
      if (cmbFinishingCode.SelectedRow != null)
      {
        string cmd = string.Format(@"SELECT  LastCarcassProcessID
                    FROM TblBOMFinishingInfo 
                    WHERE FinCode = '{0}'", cmbFinishingCode.Value.ToString());
        DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cmd);
        if (dataSource.Rows.Count > 0 && DBConvert.ParseInt(dataSource.Rows[0][0].ToString()) > 0)
        {
          ucbLastCarcassProcess.Value = DBConvert.ParseInt(dataSource.Rows[0][0].ToString());
        }
        else
        {
          ucbLastCarcassProcess.Value = null;
        }
      }

    }

    private void btnNewCarcass_Click(object sender, EventArgs e)
    {
      viewBOM_01_010 objCarcass = new viewBOM_01_010();
      Shared.Utility.WindowUtinity.ShowView(objCarcass, "New Carcass", false, Shared.Utility.ViewState.ModalWindow);
      Utility.LoadUltraCBCarcass(cmbCarcassCode);
      this.Show();
    }
    #endregion Other Events

    //private void txtSaleCode_Leave(object sender, EventArgs e)
    //{
    //  if (txtSaleCode.Text.Trim().Length > 16)
    //  {
    //    WindowUtinity.ShowMessageErrorFromText("NewSaleCode length must be less than 16 characters");
    //    txtSaleCode.Text = "";
    //    txtSaleCode.Focus();
    //  }
    //}
  }
}