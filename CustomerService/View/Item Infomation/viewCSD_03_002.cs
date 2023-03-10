/*
  Author        : Nguyễn Văn Trọn
  Create date   : 19/10/2010
  Decription    : Item Information
  Checked by    : 
  Checked date  : 
  Update By     : Huynh Thi Bang (25/09/2015)
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using System.Text;
using DaiCo.CustomerService.DataSetSource;
using Infragistics.Win;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_002 : MainUserControl
  {
    #region Fields
    public long pid = long.MinValue;
    public string itemCode = string.Empty;
    public int revision = int.MinValue;
    private bool isDataLoading = false;
    private string currentTabName = string.Empty;
    private IList listDeletedPid = new ArrayList();
    private IList listQuickShipDeletedPid = new ArrayList();
    private IList listItemRoomDeletedPid;
    private long test = long.MinValue;
    private int test1 = int.MinValue;
    private int test2 = int.MinValue;
    #endregion Fields

    public viewCSD_03_002()
    {
      InitializeComponent();
    }

    #region function
    /// <summary>
    /// Close view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseView(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    #region Load Data
    /// <summary>
    /// Load category combobox
    /// </summary>
    private void LoadComboboxCategory()
    {
      string commandText = "Select Pid, Category From TblCSDCategory ORDER BY Category";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadCombobox(cmbCategory, dtSource, "Pid", "Category");
    }

    /// <summary>
    /// Load Combobox Collection
    /// </summary>
    private void LoadComboboxCollection()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 2 AND DeleteFlag = 0 ORDER BY Value";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadCombobox(cmbCollection, dtSource, "Code", "Value");
    }

    /// <summary>
    /// Load sample combobox
    /// </summary>
    private void LoadComboboxSample()
    {
      string commandText = "Select Pid, Name From TblCSDSampleInfo Order By OrderBy";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadCombobox(cmbSample, dtSource, "Pid", "Name");
    }

    /// <summary>
    /// Load dropdown hardware
    /// </summary>
    /// <param name="ultraDDComp"></param>
    private void LoadDropDownHardware(Infragistics.Win.UltraWinGrid.UltraDropDown ultraDDComp)
    {
      //Load data Dropdown Comp theo Group
      string commandText = string.Format("Select (Code + '|' + ISNULL(Cast(Revision as varchar), '')) as DisplayCode, Code, Name, Revision From VBOMComponent Where CompGroup = {0}", ConstantClass.COMP_HARDWARE);
      DataTable dtComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDComp.DataSource = dtComp;
      ultraDDComp.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDComp.ValueMember = "DisplayCode";
      ultraDDComp.DisplayMember = "Code";
      ultraDDComp.DisplayLayout.Bands[0].Columns["DisplayCode"].Hidden = true;
      ultraDDComp.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
    }

    /// <summary>
    /// Load dropdown Component
    /// </summary>
    /// <param name="ultraDDComp"></param>
    private void LoadDropDownComp(Infragistics.Win.UltraWinGrid.UltraDropDown ultraDDComp, int componentGroup)
    {
      //Load data Dropdown Comp theo Group
      string commandText = string.Format("Select Code, Name, Revision From VBOMComponent Where CompGroup = {0}", componentGroup);
      DataTable dtComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDComp.DataSource = dtComp;
      ultraDDComp.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDComp.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
    }

    /// <summary>
    /// Load dropdown finishing material alternative
    /// </summary>
    /// <param name="ultraDDAlter"></param>
    private void LoadDropDownFinishAlternative(Infragistics.Win.UltraWinGrid.UltraDropDown ultraDDAlter)
    {
      //Load data Dropdown Alternative
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListCompInfoMaterial", null);
      ultraDDAlter.DataSource = dtSource;
      ultraDDAlter.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load data for combobox Item Type
    /// </summary>
    private void LoadComboboxType()
    {
      string commandText = string.Format("Select Code, Value From TblBOMCodeMaster Where [Group] = {0} And Code <> 4", ConstantClass.GROUP_SALEORDERTYPE);
      DataTable dtType = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadCombobox(cmbType, dtType, "Code", "Value");
    }
    private void LoadCBBaseItemCode()
    {
      string cmText = string.Format("SELECT Pid, BaseItemCode FROM TblCSDECatBaseItemCodeInfo");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      ControlUtility.LoadUltraCombo(ultCBBaseItem, dt, "Pid", "BaseItemCode", false, "Pid");
    }
    /// <summary>
    /// Load Main data
    /// </summary>
    private void LoadDataMain()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("ItemCode", DbType.AnsiString, 16, this.itemCode) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDGeneralItemInfo", inputParam);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        this.pid = DBConvert.ParseLong(dtSource.Rows[0]["Pid"].ToString());
        txtItemCode.Text = this.itemCode;
        txtItemName.Text = dtSource.Rows[0]["ItemName"].ToString();
        txtSaleCode.Text = dtSource.Rows[0]["SaleCode"].ToString();
        txtBaseItemCode.Text = dtSource.Rows[0]["BaseItemCode"].ToString();
        if (DBConvert.ParseLong(dtSource.Rows[0]["BaseItemPid"].ToString()) != long.MinValue)
        {
          ultCBBaseItem.Value = DBConvert.ParseLong(dtSource.Rows[0]["BaseItemPid"].ToString());
        }
        if (txtSaleCode.Text.Trim().Length == 0)
        {
          txtSaleCode.ReadOnly = false;
        }
        else
        {
          txtSaleCode.ReadOnly = true;
        }
        txtShortName.Text = dtSource.Rows[0]["ShortName"].ToString();
        txtUSShortName.Text = dtSource.Rows[0]["USShortName"].ToString();
        txtDimension.Text = dtSource.Rows[0]["Dimension"].ToString();
        ultraCBCatalogue.Value = dtSource.Rows[0]["CatalogueID"];
        try
        {
          cmbType.SelectedValue = dtSource.Rows[0]["ItemType"];
        }
        catch { }
        ultraCBCustomer.Value = dtSource.Rows[0]["CustomerPid"];
        ultraCBExhibition.Value = dtSource.Rows[0]["Exhibition"];
        pictureItem.ImageLocation = FunctionUtility.BOMGetItemImage(this.itemCode, this.revision);
        chkReshooting.Checked = (DBConvert.ParseInt(dtSource.Rows[0]["Reshooting"]) == 1 ? true : false);
      }
    }

    /// <summary>
    /// Load data for General Tab
    /// </summary>
    private void LoadDataGeneral()
    {
      this.listItemRoomDeletedPid = new ArrayList();
      // Load ultraDDRoom
      string commandText = "Select Pid, Room From TblCSDRoom";
      DataTable dtRoom = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraDropDown(ultraDDRoom, dtRoom, "Pid", "Room", "Pid");
      ultraDDRoom.DisplayLayout.Bands[0].ColHeadersVisible = false;

      // Load data from DB
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("ItemCode", DbType.AnsiString, 16, this.itemCode) };
      DataSet dsGeneralItemInfo = DataBaseAccess.SearchStoreProcedure("spCSDGeneralItemInfo", inputParam);      
      if (dsGeneralItemInfo.Tables.Count > 2)
      {
        DataTable dtSource = dsGeneralItemInfo.Tables[0];
        if (dtSource != null && dtSource.Rows.Count > 0)
        {
          try
          {
            cmbCollection.SelectedValue = dtSource.Rows[0]["Collection"];
          }
          catch { }
          try
          {
            cmbCategory.SelectedValue = dtSource.Rows[0]["Category"];
          }
          catch { }
          try
          {
            cmbSample.SelectedValue = dtSource.Rows[0]["SamplePid"];
          }
          catch { }
          if (DBConvert.ParseLong(dtSource.Rows[0]["DesignerPid"].ToString()) > 0)
          {
            ultCBDesigner.Value = DBConvert.ParseLong(dtSource.Rows[0]["DesignerPid"].ToString());
          }
          txtPageNo.Text = dtSource.Rows[0]["PageNo"].ToString();
          chkDiscontinue.Checked = (dtSource.Rows[0]["DiscontinueFlag"].ToString().Equals("1") ? true : false);
          txtCustomNote.Text = dtSource.Rows[0]["CustomNote"].ToString();
          txtKeyWords.Text = dtSource.Rows[0]["KeyWords"].ToString();
          txtDescription.Text = dtSource.Rows[0]["Description"].ToString();
          txtMakertingDescription.Text = dtSource.Rows[0]["MakertingDescription"].ToString();
          txtRDNote.Text = dtSource.Rows[0]["RDNote"].ToString();
          txtFactoryDesciption.Text = dtSource.Rows[0]["FactoryDescription"].ToString();
          txtPriceListDescription.Text = dtSource.Rows[0]["PriceListDescription"].ToString();
          txtRemark.Text = dtSource.Rows[0]["Remark"].ToString();
          txtMainFinishing.Text = dtSource.Rows[0]["FinishingName"].ToString();
          txtMainMaterial.Text = dtSource.Rows[0]["MaterialName"].ToString();
          this.txtSpecialDescriptionEN.Text = dtSource.Rows[0]["SpecialDescriptionEN"].ToString();
          this.txtSpecialDescriptionVN.Text = dtSource.Rows[0]["SpecialDescriptionVN"].ToString();
        }
        //Room
        DataTable dtItemRoom = dsGeneralItemInfo.Tables[1];
        dtItemRoom.PrimaryKey = new DataColumn[] { dtItemRoom.Columns["RoomPid"] };
        ultraGridItemRoom.DataSource = dtItemRoom;

        //Option Set
        ultraGridOptionSet.DataSource = dsGeneralItemInfo.Tables[2];

        //Option Set Detail
        ultraGridOptionSetDetail.DataSource = dsGeneralItemInfo.Tables[3];
      }
    }

    private void LoadDataQuickShip()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemActiveInfomation_SelectByItemCode", inputParam);
      ultraGridQuickShip.DataSource = dtSource;

      // Neu quickship thi khong cho sua active
      for (int i = 0; i < ultraGridQuickShip.Rows.Count; i++)
      {
        int quickShip = DBConvert.ParseInt(ultraGridQuickShip.Rows[i].Cells["QuickShip"].Value.ToString());
        if (quickShip == 1)
        {
          ultraGridQuickShip.Rows[i].Cells["Active"].Activation = Activation.ActivateOnly;
        }
      }
    }

    /// <summary>
    /// Load data for carcass tab
    /// </summary>
    private void LoadDataCarcass()
    {

      string commandText = "Select CAR.CarcassCode, CAR.[Description] CarcassName From TblBOMItemInfo ITEM ";
      commandText += "LEFT JOIN TblBOMCarcass CAR ON ITEM.CarcassCode = CAR.CarcassCode ";
      commandText += string.Format("Where ITEM.ItemCode = '{0}' AND ITEM.Revision = {1}", this.itemCode, this.revision);
      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCarcass != null && dtCarcass.Rows.Count > 0)
      {
        string carcass = dtCarcass.Rows[0]["CarcassCode"].ToString();
        txtCarcassCode.Text = carcass;
        txtCarcassName.Text = dtCarcass.Rows[0]["CarcassName"].ToString();
        pictureCarcass.ImageLocation = FunctionUtility.BOMGetCarcassImage(carcass);

        // Load dropdown Sup Comp
        commandText = "Select Pid, ComponentCode From TblBOMCarcassComponent ";
        commandText += string.Format(" Where CarcassCode = '{0}' And IsMainComp = 0", carcass);
        DataTable dtSubComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
        ultraDDSubComp.DataSource = dtSubComp;
        ultraDDSubComp.ValueMember = "Pid";
        ultraDDSubComp.DisplayMember = "ComponentCode";

        // Load grid Carcass
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcass) };
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMCarcassInfomationByCarcassCode", inputParam);

        Technical.DataSetSource.dsBOMCarcassComponentInfo dsData = new Technical.DataSetSource.dsBOMCarcassComponentInfo();
        dsData.Tables["CarcassComponent"].Merge(dsSource.Tables[1]);
        dsData.Tables["SubComponent"].Merge(dsSource.Tables[2]);
        dsData.Tables["SubComponentDetail"].Merge(dsSource.Tables[3]);
        ultraGridCarcassInfo.DataSource = dsData;
      }
    }

    /// <summary>
    /// Load data for hardware tab
    /// </summary>
    private void LoadDataHardware()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataTable dtComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabHardwareInfo", inputParam);
      DataTable dtCompDetail = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabHardwareInfoDetail", inputParam);
      DataSet dsSource = CreateDataSet.TabComponent();
      dsSource.Tables["TblBOMComponentInfo"].Merge(dtComp);
      dsSource.Tables["TblBOMComponentInfoDetail"].Merge(dtCompDetail);
      ultraGridHardwareInfo.DataSource = dsSource;
    }

    /// <summary>
    /// Load data for glass tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadDataGlass()
    {
      //Load Glass Grid
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataTable dtGlassInfo = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListItemGlassInfo", inputParam);

      string commandTextAdj = string.Format("Select Code, Description From TblBOMCodeMaster Where [Group] = {0}", ConstantClass.GROUP_GLASS);
      DataTable dtAdj = DataBaseAccess.SearchCommandTextDataTable(commandTextAdj);

      DataColumn column = new DataColumn();
      column.DataType = System.Type.GetType("System.String");
      column.ColumnName = "Glass, Mirror Specification";
      dtGlassInfo.Columns.Add(column);

      // Load Du lieu vao column Glass, Mirror Spaecification
      foreach (DataRow dr in dtGlassInfo.Rows)
      {
        string[] glassAdj = dr["GlassAdj"].ToString().Split('|');

        for (int i = 0; i < glassAdj.Length; i++)
        {
          if (glassAdj[i].ToString().Length > 0)
          {
            BOMCodeMaster objCM = new BOMCodeMaster();
            objCM.Code = DBConvert.ParseInt(glassAdj[i].ToString());
            objCM.Group = ConstantClass.GROUP_GLASS;
            objCM = (BOMCodeMaster)DataBaseAccess.LoadObject(objCM, new string[] { "Group", "Code" });
            dr["Glass, Mirror Specification"] += objCM.Description + ", ";
          }
        }
        string strTemp = dr["Glass, Mirror Specification"].ToString().Trim();
        if (strTemp.Length > 0)
        {
          strTemp = dr["Bevel"].ToString().Trim() + ", " + strTemp.TrimEnd(',');
          strTemp = strTemp.TrimStart(',');
          strTemp = strTemp.Trim();
        }

        dr["Glass, mirror Specification"] = strTemp;
      }

      ultraGridGlassInfo.DataSource = dtGlassInfo;

      //Gan Dropdown vao grid
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompGlass;

      //Edit lai Grid        
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["GlassAdj"].Hidden = true;
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Bevel"].Hidden = true;
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["GlassAdj"].Hidden = true;
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].Header.Caption = "Component Code";
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["ComponentName"].Header.Caption = "Component Name";
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }

    /// <summary>
    /// Load data for Support tab
    /// </summary>
    private void LoadDataSupport()
    {
      string commandText = string.Format("SELECT INFO.SupCode Code, SUP.Description Name	FROM TblBOMItemInfo INFO LEFT JOIN TblBOMSupportInfo SUP ON INFO.SupCode = SUP.SupCode WHERE INFO.ItemCode = '{0}' AND INFO.Revision = {1} ", this.itemCode, this.revision);
      DataTable dtSupport = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSupport != null && dtSupport.Rows.Count > 0)
      {
        string supCode = dtSupport.Rows[0]["Code"].ToString();
        string supName = dtSupport.Rows[0]["Name"].ToString();
        txtSupportCode.Text = supCode;
        txtSupportName.Text = supName;
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SupCode", DbType.AnsiString, 16, supCode) };
        DataTable dtSup = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListSupportDetailBySupCode", inputParam);
        ultraGridSupportInfo.DataSource = dtSup;
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Depth"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      }
    }

    /// <summary>
    /// Load data for Accessory tab
    /// </summary>
    private void LoadDataAccessory()
    {
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, ConstantClass.COMP_ACCESSORY);
      DataTable dtComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfo", inputParam);
      DataTable dtCompDetail = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfoDetail", inputParam);
      DataSet dsSource = CreateDataSet.TabComponent();
      //dsSource.Tables.Add(dtComp.Clone());
      dsSource.Tables["TblBOMComponentInfo"].Merge(dtComp);
      //dsSource.Tables.Add(dtCompDetail.Clone());
      dsSource.Tables["TblBOMComponentInfoDetail"].Merge(dtCompDetail);
      ultraGridAccessoryInfo.DataSource = dsSource;

      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      //Gan Dropdown vao grid
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompAccessory;
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Comp_Alter"].ValueList = ultraDDAlternativeAccessory;

      //Edit lai Grid        
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Material_Alter"].Header.Caption = "Material Alternative";
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Comp_Alter"].Header.Caption = "Comp Alternative";
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["AlterRevision"].Hidden = true;
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].Header.Caption = "Comp Code";
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ComponentName"].Header.Caption = "Comp Name";
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["ComponentCode"].Hidden = true;
      ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["TotalQty"].Header.Caption = "Total Qty";
      ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }

    /// <summary>
    /// Load data for Upholstery Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadDataUpholstery()
    {
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, ConstantClass.COMP_UPHOLSTERY);
      DataTable dtComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfo", inputParam);
      DataTable dtCompDetail = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfoDetail", inputParam);
      DataSet dsSource = CreateDataSet.TabComponent();
      dsSource.Tables["TblBOMComponentInfo"].Merge(dtComp);
      dsSource.Tables["TblBOMComponentInfoDetail"].Merge(dtCompDetail);
      ultraGridUpholsteryInfo.DataSource = dsSource;

      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      //Gan Dropdown vao grid
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompUphol;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Comp_Alter"].ValueList = ultraDDAlternativeUphol;

      //Edit lai Grid        
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Material_Alter"].Header.Caption = "Material Alternative";
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Comp_Alter"].Header.Caption = "Comp Alternative";
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["AlterRevision"].Hidden = true;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].Header.Caption = "Comp Code";
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ComponentName"].Header.Caption = "Comp Name";
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["ComponentCode"].Hidden = true;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["TotalQty"].Header.Caption = "Total Qty";
      ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;


    }

    /// <summary>
    /// Load data for Finishing Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadDataFinishing()
    {
      ////Load data finishing grid
      //DBParameter[] inputParam = new DBParameter[3];
      //inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      //inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      //inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, ConstantClass.COMP_FINISHING);
      //DataSet dsFinishingInfo = DataBaseAccess.SearchStoreProcedure("spBOMListTabFinishingInfo", inputParam);
      //dsFinishingInfo.Relations.Add("Relation1", dsFinishingInfo.Tables[0]);
      //DataSet dsSource = CreateDataSet.TabFinishing();
      //dsSource.Tables["TblBOMComponentInfo"].Merge(dsFinishingInfo.Tables[0]);
      //dsSource.Tables["TblBOMComponentInfoDetail"].Merge(dsFinishingInfo.Tables[1]);
      //ultraGridFinishingInfo.DataSource = dsSource;

      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      ////Gan Dropdown vao grid
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompFinish;

      ////Edit lai Grid        
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["Length"].Hidden = true;
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["Width"].Hidden = true;
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["Thickness"].Hidden = true;
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Hidden = true;
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].Header.Caption = "Comp Code";
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["ComponentName"].Header.Caption = "Comp Name";
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //ultraGridFinishingInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //ultraGridFinishingInfo.DisplayLayout.Bands[1].Columns["ComponentCode"].Hidden = true;
      //ultraGridFinishingInfo.DisplayLayout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      //ultraGridFinishingInfo.DisplayLayout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }

    /// <summary>
    /// Load data for Packing tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadDataPacking()
    {
      string commandText = "Select ITEM.PackageCode, PAK.PackageName, PAK.QuantityItem, PAK.QuantityBox, PAK.TotalCBM ";
      commandText += "From TblBOMItemInfo ITEM LEFT JOIN TblBOMPackage PAK ON ITEM.PackageCode = PAK.PackageCode ";
      commandText += string.Format("Where ITEM.ItemCode = '{0}' And ITEM.Revision = {1}", this.itemCode, this.revision);
      DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtInfo != null && dtInfo.Rows.Count > 0)
      {
        string packageCode = dtInfo.Rows[0]["PackageCode"].ToString();
        txtPackageCode.Text = packageCode;
        txtPackageName.Text = dtInfo.Rows[0]["PackageName"].ToString();
        txtItemPerBox.Text = dtInfo.Rows[0]["QuantityItem"].ToString();
        txtBoxPerItem.Text = dtInfo.Rows[0]["QuantityBox"].ToString();
        txtTotalCBM.Text = dtInfo.Rows[0]["TotalCBM"].ToString();

        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PackageCode", DbType.AnsiString, 16, packageCode) };
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMPackageInfomationByPackageCode", inputParam);

        //Load grid info
        DataSet dsData = CreateDataSet.BoxTypeInfo();
        dsData.Tables["dtBoxType"].Merge(dsSource.Tables[1]);
        dsData.Tables["dtBoxTypeDetail"].Merge(dsSource.Tables[2]);
        ultraGridPackageInfo.DataSource = dsData;
        ultraGridPackageInfo.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
        ultraGridPackageInfo.DisplayLayout.Bands[0].Columns["Child"].Hidden = true;
        ultraGridPackageInfo.DisplayLayout.Bands[0].Columns["BoxCode"].Header.Caption = "Box Code";
        ultraGridPackageInfo.DisplayLayout.Bands[0].Columns["BoxName"].Header.Caption = "Box Name";
        ultraGridPackageInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridPackageInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridPackageInfo.DisplayLayout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridPackageInfo.DisplayLayout.Bands[0].Columns["GWeight"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridPackageInfo.DisplayLayout.Bands[0].Columns["NWeight"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["Pid"].Hidden = true;
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["Child"].Hidden = true;
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["BoxCode"].Hidden = true;
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["MaterialName"].Header.Caption = "Material Name";
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["FactoryUnit"].Header.Caption = "Factory Unit";
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["TotalQty"].Header.Caption = "Total Qty";
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["Raw_Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["Raw_Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["Raw_thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridPackageInfo.DisplayLayout.Bands[1].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      }
    }

    /// <summary>
    /// Load data for Direct Labour Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadDataDirectLabour()
    {
      //Load data Dropdown Labour            
      string commandText = "Select Code, NameEN From VBOMSection";
      DataTable dtLabour = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDLabour.DataSource = dtLabour;
      ultraDDLabour.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDLabour.DisplayLayout.Bands[0].Columns[0].Width = 150;
      ultraDDLabour.DisplayLayout.Bands[0].Columns[1].Width = 400;

      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataTable dtDirectLabour = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabDirectLabourInfo", inputParam);
      ultraGridLabourInfo.DataSource = dtDirectLabour;

      //Gan Dropdown vao grid        
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["SectionCode"].ValueList = ultraDDLabour;

      //Edit lai Grid        
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["Qty"].Header.Caption = "Qty (Man*hour)";
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["Qty"].MinWidth = 90;
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["Qty"].MaxWidth = 90;
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["SectionCode"].Header.Caption = "Section Code";
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["SectionCode"].MinWidth = 80;
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["SectionCode"].MaxWidth = 80;
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["NameEN"].Header.Caption = "Name";
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["NameEN"].MinWidth = 250;
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["NameEN"].MaxWidth = 250;
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["Qty"].Format = "#,###";
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["Description"].MinWidth = 300;
      ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["Description"].MaxWidth = 300;
    }

    /// <summary>
    /// Load data for Relative Tab
    /// </summary>
    private void LoadDataItemRelative()
    {
      // Load data for dropdown Item Relative
      //string commandText = string.Format("Select ItemCode, SaleCode, Name From TblBOMItemBasic Where ItemCode <> '{0}'", this.itemCode);
      // Truong Add
      string commandText = string.Format(@" SELECT  Bas.ItemCode, Bas.SaleCode, Bas.Name 
                                            FROM TblBOMItemBasic Bas
                                                INNER JOIN TblCSDItemInfo Info ON Bas.ItemCode = Info.ItemCode
                                            WHERE ISNULL(Info.DiscontinueFlag, 0) = 0 AND Bas.ItemCode <> '{0}'", this.itemCode);
      // End
      DataTable dtItemRelative = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDItemRelative.DataSource = dtItemRelative;
      ultraDDItemRelative.DisplayMember = "ItemCode";
      ultraDDItemRelative.ValueMember = "ItemCode";
      ultraDDItemRelative.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      ultraDDItemRelative.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      ultraDDItemRelative.DisplayLayout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      ultraDDItemRelative.DisplayLayout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
     // ultraDDItemRelative.DisplayLayout.Bands[0].Columns["Name"].MinWidth = 250;
      ultraDDItemRelative.DisplayLayout.Bands[0].ColHeadersVisible = false;

      // Truong Add
      string commandTextSaleCode = string.Format(@" SELECT  Bas.SaleCode, Bas.ItemCode, Bas.Name 
                                            FROM TblBOMItemBasic Bas
                                                INNER JOIN TblCSDItemInfo Info ON Bas.ItemCode = Info.ItemCode
                                            WHERE ISNULL(Info.DiscontinueFlag, 0) = 0 AND BAS.SaleCode IS NOT NULL AND Bas.ItemCode <> '{0}'", this.itemCode);
      DataTable dtSaleCodeRelative = DataBaseAccess.SearchCommandTextDataTable(commandTextSaleCode);
      ultraDDSaleCodeRelative.DataSource = dtSaleCodeRelative;
      ultraDDSaleCodeRelative.DisplayMember = "SaleCode";
      ultraDDSaleCodeRelative.ValueMember = "SaleCode";
      ultraDDSaleCodeRelative.DisplayLayout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      ultraDDSaleCodeRelative.DisplayLayout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
      ultraDDSaleCodeRelative.DisplayLayout.Bands[0].ColHeadersVisible = false;
      // End

      // Load data for Grid Item Relative
      //string commandItemRelative = "Select REL.Pid, INFO.ItemCode, INFO.SaleCode, INFO.Name, REL.[Description] From TblCSDItemRelative REL Inner Join TblBOMItemBasic INFO";
      //commandItemRelative += string.Format(" ON REL.RelativeItem = INFO.ItemCode And REL.ItemCode = '{0}'", this.itemCode);

      string commandItemRelative = string.Format(@"Select REL.Pid, REL.[No], INFO.ItemCode, INFO.SaleCode, INFO.Name, REL.[Description] 
                                                    From TblCSDItemRelative REL 
                                                    Inner Join TblBOMItemBasic INFO ON REL.RelativeItem = INFO.ItemCode And REL.ItemCode = '{0}'
                                                    Inner Join	TblCSDItemInfo ITEM ON INFO.ItemCode = ITEM.ItemCode AND ITEM.DiscontinueFlag = 0
                                                    ORDER BY REL.[No] ASC", this.itemCode);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandItemRelative);
      ultraGridItemRelative.DataSource = dtSource;
      dtSource.PrimaryKey = new DataColumn[] { dtSource.Columns["ItemCode"] };
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["ItemCode"].ValueList = ultraDDItemRelative;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["No"].MinWidth = 50;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["No"].MaxWidth = 50;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["No"].CellAppearance.TextHAlign = HAlign.Right;
      // Truong Add
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["SaleCode"].ValueList = ultraDDSaleCodeRelative;
      // End
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
      //ultraGridItemRelative.DisplayLayout.Bands[0].Columns["SaleCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["Name"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["Name"].MinWidth = 350;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["Name"].MaxWidth = 350;
      ultraGridItemRelative.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultraGridItemRelative.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
    }

    /// <summary>
    /// Load data for Alternative Tab
    /// </summary>
    private void LoadDataItemAlternative()
    {
      // Load data for dropdown Item Alternative
      //string commandText = string.Format("Select ItemCode, SaleCode, Name From TblBOMItemBasic Where ItemCode <> '{0}'", this.itemCode);
      // Truong Add
      string commandText = string.Format(@" SELECT  Bas.ItemCode, Bas.SaleCode, Bas.Name 
                                            FROM TblBOMItemBasic Bas
                                                INNER JOIN TblCSDItemInfo Info ON Bas.ItemCode = Info.ItemCode
                                            WHERE ISNULL(Info.DiscontinueFlag, 0) = 0 AND Bas.ItemCode <> '{0}'", this.itemCode);
      // End
      DataTable dtItemRelative = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDItemAlternative.DataSource = dtItemRelative;
      ultraDDItemAlternative.DisplayMember = "ItemCode";
      ultraDDItemAlternative.ValueMember = "ItemCode";
      ultraDDItemAlternative.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      ultraDDItemAlternative.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      ultraDDItemAlternative.DisplayLayout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      ultraDDItemAlternative.DisplayLayout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
      ultraDDItemAlternative.DisplayLayout.Bands[0].ColHeadersVisible = false;
     // ultraDDItemAlternative.DisplayLayout.Bands[0].Columns["Name"].MinWidth = 350;

      // Truong Add
      string commandTextSaleCode = string.Format(@" SELECT  Bas.SaleCode, Bas.ItemCode, Bas.Name 
                                            FROM TblBOMItemBasic Bas
                                                INNER JOIN TblCSDItemInfo Info ON Bas.ItemCode = Info.ItemCode
                                            WHERE ISNULL(Info.DiscontinueFlag, 0) = 0 AND BAS.SaleCode IS NOT NULL AND Bas.ItemCode <> '{0}'", this.itemCode);
      DataTable dtSaleCodeAlternative = DataBaseAccess.SearchCommandTextDataTable(commandTextSaleCode);
      ultraDDSaleCodeAlternative.DataSource = dtSaleCodeAlternative;
      ultraDDSaleCodeAlternative.DisplayMember = "SaleCode";
      ultraDDSaleCodeAlternative.ValueMember = "SaleCode";
      ultraDDSaleCodeAlternative.DisplayLayout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      ultraDDSaleCodeAlternative.DisplayLayout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
      ultraDDSaleCodeAlternative.DisplayLayout.Bands[0].ColHeadersVisible = false;
      // End

      // Load data for Grid Item Relative

      //StringBuilder commandItemRelative = new StringBuilder();
      //commandItemRelative.Append("Select REL.Pid, INFO.ItemCode, INFO.SaleCode, INFO.Name, REL.[Description] From TblCSDItemAlternative REL Inner Join TblBOMItemBasic INFO");
      //commandItemRelative.Append(string.Format(" ON REL.AlternativeItem = INFO.ItemCode And REL.ItemCode = '{0}'", this.itemCode));
      //DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandItemRelative.ToString());

      string commandItemAlternative = string.Format(@"Select REL.Pid, REL.[No], INFO.ItemCode, INFO.SaleCode, INFO.Name, REL.[Description] 
                                                      From TblCSDItemAlternative REL 
                                                      Inner Join TblBOMItemBasic INFO ON REL.AlternativeItem = INFO.ItemCode And REL.ItemCode = '{0}'
                                                      Inner Join TblCSDItemInfo ITEM ON INFO.ItemCode = ITEM.ItemCode AND ITEM.DiscontinueFlag = 0
                                                      ORDER BY REL.[No] ASC", this.itemCode);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandItemAlternative);
      ultraGridItemAlternative.DataSource = dtSource;
      dtSource.PrimaryKey = new DataColumn[] { dtSource.Columns["ItemCode"] };
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["ItemCode"].ValueList = ultraDDItemAlternative;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["No"].MinWidth = 50;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["No"].MaxWidth = 50;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["No"].CellAppearance.TextHAlign = HAlign.Right;
      // Truong Add
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["SaleCode"].ValueList = ultraDDSaleCodeAlternative;
      // End
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      //ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["SaleCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["Name"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["Name"].MinWidth = 350;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["Name"].MaxWidth = 350;
      ultraGridItemAlternative.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultraGridItemAlternative.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
    }

    /// <summary>
    /// Load data for Sale History Tab
    /// </summary>
    private void LoadDataSaleHistory()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode) };
      DataTable dtSaleHistory = DataBaseAccess.SearchStoreProcedureDataTable("spCSDSaleHistoryItem", inputParam);
      ultraGridSaleHistory.DataSource = dtSaleHistory;

      ultraGridSaleHistory.DisplayLayout.Bands[0].Columns["Month"].MaxWidth = 80;
      ultraGridSaleHistory.DisplayLayout.Bands[0].Columns["Month"].MinWidth = 80;

      ultraGridSaleHistory.DisplayLayout.Bands[0].Columns["Qty"].MaxWidth = 80;
      ultraGridSaleHistory.DisplayLayout.Bands[0].Columns["Qty"].MinValue = 80;
      ultraGridSaleHistory.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridSaleHistory.DisplayLayout.Bands[0].Columns["Qty"].Format = "#,###";
    }

    /// <summary>
    /// Load date for Item Language
    /// </summary>
    private void LoadDataTabItemLanguage()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode) };
      DataTable dtItemLanguage = DataBaseAccess.SearchStoreProcedureDataTable("spCSDItemInfoLanguage_Select", inputParam);
      ultraGridItemLanguage.DataSource = dtItemLanguage;
    }
    /// <summary>
    /// Load data for tab ECAT
    /// </summary>
    private void LoadDataTabECAT()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@ItemCode", DbType.String, this.itemCode);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spCSDECatItemOptionSet_Select", input);
      ultItemOption.DataSource = dt;

    }
    private UltraDropDown LoadOptionCode(int kindPid, UltraDropDown ultDDOp)
    {
      if (ultDDOp == null)
      {
        ultDDOp = new UltraDropDown();
        this.Controls.Add(ultDDOp);
      }
      string cmText = string.Format(@"SELECT DISTINCT OPC.Pid OptionPid, OPC.Code + ' - ' + OPC.Name OptionName
		                                  FROM TblCSDOptionSetForEcat OPG
				                                  INNER JOIN TblCSDOptionCodeForEcat OPC ON '|' + OPG.[Option] + '|' LIKE '%|' + CAST(OPC.Pid AS VARCHAR) + '|%'
		                                  WHERE OPG.KindPid = {0}", kindPid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      if (dt != null)
      {
        ultDDOp.DataSource = dt;
        ultDDOp.ValueMember = "OptionPid";
        ultDDOp.DisplayMember = "OptionName";
        ultDDOp.DisplayLayout.Bands[0].Columns["OptionPid"].Hidden = true;
        ultDDOp.DisplayLayout.AutoFitColumns = true;
        ultDDOp.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      }
      return ultDDOp;
    }
    /// <summary>
    /// Load data for costing component (Hardware, Glass) tab
    /// </summary>
    private DataSet LoadDataCostingComponent(string storeName)
    {
      dsCSDCostingComponentInfo dsCosting = new dsCSDCostingComponentInfo();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);

      DataSet dsComponent = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      if (dsComponent != null)
      {
        dsCosting.Tables["tblCostingComponent"].Merge(dsComponent.Tables[0]);
        dsCosting.Tables["tblCostingComponentDetail"].Merge(dsComponent.Tables[1]);
      }
      return dsCosting;
    }

    /// <summary>
    /// Load data for costing support tab
    /// </summary>
    private DataSet LoadDataCostingSupport()
    {
      dsCSDCostingComponentInfo dsCosting = new dsCSDCostingComponentInfo();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataSet dsComponent = DataBaseAccess.SearchStoreProcedure("spBOMListCostingSupportInfo", inputParam);
      if (dsComponent != null)
      {
        dsCosting.Tables["tblCostingComponent"].Merge(dsComponent.Tables[0]);
        dsCosting.Tables["tblCostingComponentDetail"].Merge(dsComponent.Tables[1]);
      }
      return dsCosting;
    }

    /// <summary>
    /// Load data for costing carcass tab
    /// </summary>
    private void LoadDataCostingCarcass()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataSet dsCarcass = DataBaseAccess.SearchStoreProcedure("spBOMListCostingCarcassInfo", inputParam);
      int count = dsCarcass.Tables.Count;
      if (dsCarcass != null && count > 0)
      {
        DataTable dtCarcassInfo = dsCarcass.Tables[0];
        if (dtCarcassInfo.Rows.Count > 0)
        {
          txtCostingCarcassCode.Text = dtCarcassInfo.Rows[0]["CarcassCode"].ToString();
          txtCostingCarcassDescription.Text = dtCarcassInfo.Rows[0]["Description"].ToString();
        }
        //Make in local
        if (count > 1)
        {
          dsCSDCostingCarcassInfo dsCostingMakeInLocal = new dsCSDCostingCarcassInfo();
          dsCostingMakeInLocal.Tables["tblCostingCarcassDetail"].Merge(dsCarcass.Tables[1]);
          ultraGridCostingLocalCarcass.DataSource = dsCostingMakeInLocal;
        }
        // Contract Out
        if (count > 2)
        {
          dsCSDCostingCarcassInfo dsCostingContractOut = new dsCSDCostingCarcassInfo();
          dsCostingContractOut.Tables["tblCostingCarcassDetail"].Merge(dsCarcass.Tables[2]);
          ultraGridCostingContractOutCarcass.DataSource = dsCostingContractOut;
          tableLayoutCostingCarcass.RowStyles[2].SizeType = SizeType.Percent;
          groupContractOut.Visible = true;
        }
        else
        {
          tableLayoutCostingCarcass.RowStyles[2].SizeType = SizeType.AutoSize;
          groupContractOut.Visible = false;
        }
      }
    }

    /// <summary>
    /// Load data for costing support tab
    /// </summary>
    private DataSet LoadDataCostingPackage()
    {
      dsCSDCostingComponentInfo dsCosting = new dsCSDCostingComponentInfo();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataSet dsComponent = DataBaseAccess.SearchStoreProcedure("spBOMListCostingPackageInfo", inputParam);
      if (dsComponent != null)
      {
        dsCosting.Tables["tblCostingComponent"].Merge(dsComponent.Tables[0]);
        dsCosting.Tables["tblCostingComponentDetail"].Merge(dsComponent.Tables[1]);
      }
      return dsCosting;
    }

    /// <summary>
    /// Load data for costing direct labour tab
    /// </summary>
    private void LoadDataCostingDirectLabour()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      DataTable dtDirectLabour = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListCostingDirectLabourInfo", inputParam);
      if (dtDirectLabour != null)
      {
        ultraGridCostingDirectLabour.DataSource = dtDirectLabour;
      }
    }

    /// <summary>
    /// Load data for Tabs
    /// </summary>
    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      this.listQuickShipDeletedPid = new ArrayList();
      this.isDataLoading = true;
      this.currentTabName = tabControlItemInfo.SelectedTab.Name;
      this.LoadDataCombobox();
      this.LoadDataMain();
      switch (this.currentTabName)
      {
        case "tabPageGeneral":
          this.LoadDataGeneral();
          break;
        case "tabPageQuickShip":
          this.LoadDataQuickShip();
          break;
        case "tabPageCarcass":
          this.LoadDataCarcass();
          break;
        case "tabPageHardware":
          this.LoadDataHardware();
          break;
        case "tabPageGlass":
          this.LoadDataGlass();
          break;
        case "tabPageSupport":
          this.LoadDataSupport();
          break;
        case "tabPageAccessory":
          this.LoadDataAccessory();
          break;
        case "tabPageUpholstery":
          this.LoadDataUpholstery();
          break;
        case "tabPageFinishing":
          this.LoadDataFinishing();
          break;
        case "tabPagePacking":
          this.LoadDataPacking();
          break;
        case "tabPageDirectLabour":
          this.LoadDataDirectLabour();
          break;
        case "tabPageRelative":
          this.LoadDataItemRelative();
          break;
        case "tabPageAlternative":
          this.LoadDataItemAlternative();
          break;
        case "tabPageSaleHistory":
          this.LoadDataSaleHistory();
          break;
        case "tabPageCosting":
          this.LoadDataTabCosting();
          break;
        case "tabPageItemLanguage":
          this.LoadDataTabItemLanguage();
          break;
        case"tabPageECat":
          this.LoadDataTabECAT();
          break;
        default:
          break;
      }
      this.isDataLoading = false;
      this.NeedToSave = false;
    }

    private void LoadDropdownMaterial(UltraDropDown udrpMaterials)
    {
      string commandText = "SELECT MaterialCode, MaterialName, MaterialNameVn, IDFactoryUnit, FactoryUnit FROM VBOMMaterialsForCarcassComponent ORDER BY MaterialCode";
      DataTable dtSourceMaterials = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpMaterials.DataSource = dtSourceMaterials;
      udrpMaterials.ValueMember = "MaterialCode";
      udrpMaterials.DisplayMember = "MaterialCode";
      udrpMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpMaterials.DisplayLayout.Bands[0].Columns["IDFactoryUnit"].Hidden = true;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialName"].Width = 200;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialNameVN"].Width = 200;
    }

    /// <summary>
    /// Load data for tab costing
    /// </summary>
    private void LoadDataTabCosting()
    {
      string tabCostingName = tabControlCostingInfo.SelectedTab.Name;
      string storeName = string.Empty;
      switch (tabCostingName)
      {
        case "tabCostingCarcass":
          this.LoadDataCostingCarcass();
          break;
        case "tabCostingHardware":
          storeName = "spBOMListCostingHardwareInfo";
          ultraGridCostingHardware.DataSource = this.LoadDataCostingComponent(storeName);
          break;
        case "tabCostingGlass":
          storeName = "spBOMListCostingGlassInfo";
          ultraGridCostingGlass.DataSource = this.LoadDataCostingComponent(storeName);
          break;
        case "tabCostingSupport":
          ultraGridCostingSupport.DataSource = this.LoadDataCostingSupport();
          break;
        case "tabCostingAccessory":
          storeName = "spBOMListCostingAccessoryInfo";
          ultraGridCostingAccessory.DataSource = this.LoadDataCostingComponent(storeName);
          break;
        case "tabCostingUpholstery":
          storeName = "spBOMListCostingUpholsteryInfo";
          ultraGridCostingUpholstery.DataSource = this.LoadDataCostingComponent(storeName);
          break;
        case "tabCostingFinishing":
          storeName = "spBOMListCostingFinishingInfo";
          ultraGridCostingFinishing.DataSource = this.LoadDataCostingComponent(storeName);
          ultraGridCostingFinishing.DisplayLayout.Bands[0].Columns["Unit"].Hidden = true;
          break;
        case "tabCostingPacking":
          ultraGridCostingPackage.DataSource = this.LoadDataCostingPackage();
          break;
        case "tabCostingDirectLabour":
          this.LoadDataCostingDirectLabour();
          break;
      }
    }
    #endregion Load Data

    /// <summary>
    /// Load data for combobox belongs to current tab
    /// </summary>
    private void LoadDataCombobox()
    {
      this.currentTabName = tabControlItemInfo.SelectedTab.Name;
      switch (this.currentTabName)
      {
        case "tabPageGeneral":
          //Load collection combobox
          //DaiCo.Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbCollection, DaiCo.Shared.Utility.ConstantClass.GROUP_COLLECTION);
          this.LoadComboboxCollection();

          //Load category combobox
          this.LoadComboboxCategory();
          //Load sample combobox
          this.LoadComboboxSample();
          break;
        case "tabPageQuickShip":
          // load distribute customer
          string commandTextCustomer = "Select Pid, CustomerCode, Name From TblCSDCustomerInfo Where Pid > 3 And ParentPid Is Null";
          DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandTextCustomer);
          ultraDDCustomerInfo.DataSource = dtCustomer;
          ultraDDCustomerInfo.ValueMember = "Pid";
          ultraDDCustomerInfo.DisplayMember = "CustomerCode";
          break;
        case "tabPageCarcass":
          //Specify          
          DaiCo.Shared.Utility.ControlUtility.LoadUltraDropdownCodeMst(udrpSpecify, ConstantClass.GROUP_COMPONENTSPECIFY);
          //Status
          DaiCo.Shared.Utility.ControlUtility.LoadUltraDropdownCodeMst(udrpStatus, ConstantClass.GROUP_COMPONENTSTATUS);
          // MaterialsCode
          this.LoadDropdownMaterial(udrpMaterialsCode);
          // Alternative
          this.LoadDropdownMaterial(udrpAlternative);
          break;
        case "tabPageHardware":
          //Load data Dropdown Comp Hardware
          this.LoadDropDownHardware(ultraDDCompHardware);
          //Load data Dropdown Alternative
          this.LoadDropDownHardware(ultraDDAlternative);
          break;
        case "tabPageGlass":
          //Load data Dropdown Comp Glass            
          string commandText = string.Format("Select Code, Name From VBOMComponent Where CompGroup = {0}", ConstantClass.COMP_GLASS);
          DataTable dtComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
          ultraDDCompGlass.DataSource = dtComp;
          ultraDDCompGlass.DisplayLayout.Bands[0].ColHeadersVisible = false;
          ultraDDCompGlass.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
          break;
        case "tabPageSupport":

          break;
        case "tabPageAccessory":
          //Load data Dropdown Comp Hardware
          this.LoadDropDownComp(ultraDDCompAccessory, ConstantClass.COMP_ACCESSORY);
          ultraDDCompAccessory.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
          //Load data Dropdown Alternative
          this.LoadDropDownComp(ultraDDAlternativeAccessory, ConstantClass.COMP_ACCESSORY);
          ultraDDAlternativeAccessory.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
          break;
        case "tabPageUpholstery":
          //Load data Dropdown Comp Hardware
          this.LoadDropDownComp(ultraDDCompUphol, ConstantClass.COMP_UPHOLSTERY);
          ultraDDCompUphol.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
          //Load data Dropdown Alternative
          this.LoadDropDownComp(ultraDDAlternativeUphol, ConstantClass.COMP_UPHOLSTERY);
          ultraDDAlternativeUphol.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
          break;
        case "tabPageFinishing":
          //Load data Dropdown Comp Finish            
          this.LoadDropDownComp(ultraDDCompFinish, ConstantClass.COMP_FINISHING);
          ultraDDCompFinish.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
          //Load data Dropdown Alternative
          this.LoadDropDownFinishAlternative(ultraDDAlternativeFinish);
          break;
        case "tabPagePacking":

          break;
        case "tabPageDirectLabour":

          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Check data in item language tab before saving
    /// </summary>
    /// <returns></returns>
    private bool CheckItemLanguageInvalid()
    {
      for (int i = 0; i < ultraGridItemLanguage.Rows.Count; i++)
      {
        string itemName = ultraGridItemLanguage.Rows[i].Cells["ItemName"].Value.ToString().Trim();
        string itemShortName = ultraGridItemLanguage.Rows[i].Cells["ItemShortName"].Value.ToString().Trim();
        string pageNo = ultraGridItemLanguage.Rows[i].Cells["PageNo"].Value.ToString().Trim();
        string makertingDescription = ultraGridItemLanguage.Rows[i].Cells["MakertingDescription"].Value.ToString().Trim();
        string description = ultraGridItemLanguage.Rows[i].Cells["Description"].Value.ToString().Trim();
        string remark = ultraGridItemLanguage.Rows[i].Cells["Remark"].Value.ToString().Trim();
        if ((itemName.Length == 0) && (itemShortName.Length > 0 || pageNo.Length > 0 || makertingDescription.Length > 0 || description.Length > 0 || remark.Length > 0))
        {
          WindowUtinity.ShowMessageError("MSG0005", string.Format("Item name at row {0}", DBConvert.ParseString(i + 1)));
          return false;
        }
      }
      return true;
    }

    private UltraDropDown LoadUltraDDOptionSet(long optionSetKind, UltraDropDown ultraDDOptionSet)
    {
      if (ultraDDOptionSet == null)
      {
        ultraDDOptionSet = new UltraDropDown();
        this.Controls.Add(ultraDDOptionSet);
      }

      string commandText = string.Format("SELECT Pid, Code FROM TblCSDOptionSet WHERE KindPid = {0}", optionSetKind);
      DataTable dtOptionSet = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraDropDown(ultraDDOptionSet, dtOptionSet, "Pid", "Code", "Pid");
      ultraDDOptionSet.DisplayLayout.Bands[0].ColHeadersVisible = false;
      return ultraDDOptionSet;
    }

    private UltraDropDown LoadUltraDDOptionSetEcat(long optionSetKindEcat, UltraDropDown ultraDDOptionSetEcat)
    {
      if (ultraDDOptionSetEcat == null)
      {
        ultraDDOptionSetEcat = new UltraDropDown();
        this.Controls.Add(ultraDDOptionSetEcat);
      }

      string commandText = string.Format("SELECT Pid, Code FROM TblCSDOptionSetForEcat WHERE KindPid = {0}", optionSetKindEcat);
      DataTable dtOptionSet = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraDropDown(ultraDDOptionSetEcat, dtOptionSet, "Pid", "Code", "Pid");
      ultraDDOptionSetEcat.DisplayLayout.Bands[0].ColHeadersVisible = false;
      return ultraDDOptionSetEcat;
    }

    /// <summary>
    /// Check invalid in all tab before saving
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      // Check Sale Code
      string saleCode = txtSaleCode.Text.Trim();
      if (saleCode.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Sale Code");
        return false;
      }
      else
      {
        string itemCode = txtItemCode.Text;
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@SaleCode", DbType.AnsiString, 16, saleCode);
        string commandTextSaleCode = "Select dbo.FBOMCheckSaleCode(@ItemCode, @SaleCode)";
        int countSaleCode = (int)DataBaseAccess.ExecuteScalarCommandText(commandTextSaleCode, inputParam);
        if (countSaleCode > 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("MSG0006", "Sale Code");
          txtSaleCode.Focus();
          return false;
        }
      }
      //Customer
      if (ultraCBCustomer.SelectedRow == null)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Customer");
        ultraCBCustomer.Focus();
        return false;
      }
      // Check Item Name
      if (txtItemName.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Item Name");
        return false;
      }
      return true;

      switch (this.currentTabName)
      {
        case "tabPageItemLanguage":
          return this.CheckItemLanguageInvalid();
        default:
          break;
      }
      return true;
    }

    #region Save Data
    /// <summary>
    /// Save before Closing
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      if (this.CheckInvalid())
      {
        if (this.SaveData())
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SaveSuccess = true;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
          this.SaveSuccess = false;
        }
      }
      else
      {
        this.SaveSuccess = false;
      }
    }

    /// <summary>
    /// Save data belongs to current tab
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool success = true;
      switch (this.currentTabName)
      {
        case "tabPageGeneral":
          success = this.SaveGeneralTab();
          break;
        case "tabPageRelative":
          success = this.SaveRelativeTab();
          break;
        case "tabPageAlternative":
          success = this.SaveAlternativeTab();
          break;
        case "tabPageItemLanguage":
          success = this.SaveItemLanguageTab();
          break;
        default:
          success = this.SaveMainInfo();
          break;
      }
      return success;
    }

    /// <summary>
    /// Save customer item default code
    /// </summary>
    /// <returns></returns>
    private bool SaveMainInfo()
    {
      string saleCode = txtSaleCode.Text.Trim();
      string shortName = txtShortName.Text.Trim();
      string itemName = txtItemName.Text.Trim();
      string usshortName = txtUSShortName.Text.Trim();

      DBParameter[] inputParam = new DBParameter[15];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      if (saleCode.Length > 0)
      {
        inputParam[2] = new DBParameter("@SaleCode", DbType.AnsiString, 16, saleCode);
      }
      if (shortName.Length > 0)
      {
        inputParam[3] = new DBParameter("@ShortName", DbType.AnsiString, 512, shortName);
      }
      if (cmbType.SelectedIndex > 0)
      {
        inputParam[4] = new DBParameter("@ItemType", DbType.Int32, cmbType.SelectedValue);
      }
      inputParam[5] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[6] = new DBParameter("@ItemName", DbType.AnsiString, 512, itemName);
      if (ultraCBCustomer.SelectedRow != null)
      {
        inputParam[7] = new DBParameter("@CustomerPid", DbType.Int64, ultraCBCustomer.Value);
      }
      if (ultraCBExhibition.SelectedRow != null)
      {
        inputParam[8] = new DBParameter("@Exhibition", DbType.Int32, ultraCBExhibition.Value);
      }
      if (ultraCBCatalogue.SelectedRow != null)
      {
        inputParam[9] = new DBParameter("@CatalogueID", DbType.Int32, ultraCBCatalogue.Value);
      }
      if(ultCBDesigner.Value != null)
      {
        inputParam[10] = new DBParameter("@DesignerPid", DbType.Int64, DBConvert.ParseLong(ultCBDesigner.Value.ToString()));
      }
      inputParam[11] = new DBParameter("@Reshooting", DbType.Int32, chkReshooting.Checked ? 1 : 0);
      if (txtBaseItemCode.Text.Trim().Length > 0)
      {
        inputParam[12] = new DBParameter("@BaseItemCode", DbType.AnsiString, 32, txtBaseItemCode.Text.Trim());
      }
      if (ultCBBaseItem.SelectedRow != null)
      {
        inputParam[13] = new DBParameter("@BaseItemPid", DbType.Int64, DBConvert.ParseLong(ultCBBaseItem.Value.ToString()));
      }
      if (usshortName.Length > 0)
      {
        inputParam[14] = new DBParameter("@USShortName", DbType.AnsiString, 29, usshortName);
      }
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spCSDItemInfo_UpdateMainInfo", inputParam, outputParam);
      int success = DBConvert.ParseInt(outputParam[0].Value.ToString());
      return (success == 0 ? false : true);
    }

    /// <summary>
    /// Save tab General
    /// </summary>
    private bool SaveGeneralTab()
    {
      if (this.SaveMainInfo())
      {
        int collection = int.MinValue;
        int discontinueFlag = chkDiscontinue.Checked ? 1 : 0;
        if (cmbCollection.SelectedIndex > 0)
        {
          collection = DBConvert.ParseInt(cmbCollection.SelectedValue.ToString());
        }
        int category = int.MinValue;
        if (cmbCategory.SelectedIndex > 0)
        {
          category = DBConvert.ParseInt(cmbCategory.SelectedValue.ToString());
        }
        int samplePid = int.MinValue;
        if (cmbSample.SelectedIndex > 0)
        {
          samplePid = DBConvert.ParseInt(cmbSample.SelectedValue.ToString());
        }

        DBParameter[] inputParam = new DBParameter[17];
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
        inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
        if (collection != int.MinValue)
        {
          inputParam[2] = new DBParameter("@Collection", DbType.Int32, collection);
        }
        if (category != int.MinValue)
        {
          inputParam[3] = new DBParameter("@Category", DbType.Int32, category);
        }
        inputParam[4] = new DBParameter("@Description", DbType.AnsiString, 1024, txtDescription.Text.Trim());
        inputParam[5] = new DBParameter("@DiscontinueFlag", DbType.Int32, discontinueFlag);
        if (samplePid != int.MinValue)
        {
          inputParam[6] = new DBParameter("@SamplePid", DbType.Int32, samplePid);
        }
        inputParam[7] = new DBParameter("@PageNo", DbType.AnsiString, 8, txtPageNo.Text.Trim());
        inputParam[8] = new DBParameter("@MakertingDescription", DbType.AnsiString, 4000, txtMakertingDescription.Text.Trim());
        inputParam[9] = new DBParameter("@Remark", DbType.AnsiString, 4000, txtRemark.Text.Trim());
        inputParam[10] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (txtCustomNote.Text.Trim().Length > 0)
        {
          inputParam[11] = new DBParameter("@CustomNote", DbType.String, 512, txtCustomNote.Text.Trim());
        }
        if (txtFactoryDesciption.Text.Trim().Length > 0)
        {
          inputParam[12] = new DBParameter("@FactoryDescription", DbType.String, 4000, txtFactoryDesciption.Text.Trim());
        }
        if (txtPriceListDescription.Text.Trim().Length > 0)
        {
          inputParam[13] = new DBParameter("@PriceListDescription", DbType.String, 4000, txtPriceListDescription.Text.Trim());
        }

        if (this.txtSpecialDescriptionEN.Text.Trim().Length > 0)
        {
          inputParam[14] = new DBParameter("@SpecialDescriptionEN", DbType.String, 4000, txtSpecialDescriptionEN.Text.Trim());
        }

        if (this.txtSpecialDescriptionVN.Text.Trim().Length > 0)
        {
          inputParam[15] = new DBParameter("@SpecialDescriptionVN", DbType.String, 4000, txtSpecialDescriptionVN.Text.Trim());
        }
        if (txtKeyWords.Text.Trim().Length > 0)
        {
          inputParam[16] = new DBParameter("@KeyWords", DbType.String, 1024, txtKeyWords.Text.Trim());
        }
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spCSDItemInfo_Update", inputParam, outputParam);
        int success = DBConvert.ParseInt(outputParam[0].Value.ToString());
        if (success > 0)
        {
          //1. Save Room of Item
          //1.1. Delete
          foreach (long pid in listItemRoomDeletedPid)
          {
            DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
            DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spCSDItemRoom_Delete", inputDelete, outputDelete);
            if (DBConvert.ParseInt(outputDelete[0].Value.ToString()) == 0)
            {
              success = 0;
            }
          }
          //1.2. Insert/ Update
          DataTable dtItemRoom = (DataTable)ultraGridItemRoom.DataSource;
          if (dtItemRoom != null && dtItemRoom.Rows.Count > 0)
          {
            foreach (DataRow row in dtItemRoom.Rows)
            {
              if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
              {
                DBParameter[] inputRoom = new DBParameter[4];
                DBParameter[] outputRoom = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
                string storeName = string.Empty;
                if (row.RowState == DataRowState.Added)
                {
                  storeName = "spCSDItemRoom_Insert";
                }
                else
                {
                  storeName = "spCSDItemRoom_Update";
                  inputRoom[0] = new DBParameter("Pid", DbType.Int64, row["Pid"]);
                }
                inputRoom[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
                inputRoom[2] = new DBParameter("@RoomPid", DbType.Int64, row["RoomPid"]);
                inputRoom[3] = new DBParameter("@Description", DbType.String, 256, row["Description"]);
                DataBaseAccess.ExecuteStoreProcedure(storeName, inputRoom, outputRoom);
                if (DBConvert.ParseInt(outputRoom[0].Value.ToString()) == 0)
                {
                  success = 0;
                }
              }
            }
          }

          //2. Option Set
          DataTable dtOptionSet = new DataTable();
          dtOptionSet.Columns.Add("OptionSetPid", typeof(System.Int64));
          dtOptionSet.Columns.Add("OptionSetRequired", typeof(System.Int32));
          dtOptionSet.Columns.Add("KindPid", typeof(System.Int64));
          long oldKind = long.MinValue;
          for (int i = 0; i < ultraGridOptionSet.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            long value = DBConvert.ParseLong(ultraGridOptionSet.Rows[0].Cells[i].Value);
            string colName = ultraGridOptionSet.DisplayLayout.Bands[0].Columns[i].ToString();
            string[] arrColName = colName.Split('|');
            long kind = DBConvert.ParseLong(arrColName[0]);
            if (kind != oldKind)
            {
              dtOptionSet.Rows.Add(dtOptionSet.NewRow());
              oldKind = kind;
            }
            DataRow row = dtOptionSet.Rows[dtOptionSet.Rows.Count - 1];
            row["KindPid"] = kind;
            if (arrColName.Length > 1)
            {
              row["OptionSetRequired"] = value;
            }
            else
            {
              row["OptionSetPid"] = value;
            }
          }
          foreach (DataRow row in dtOptionSet.Rows)
          {
            long optionSetPid = DBConvert.ParseLong(row["OptionSetPid"]);
            int optionSetRequired = DBConvert.ParseInt(row["OptionSetRequired"]);
            long kindPid = DBConvert.ParseLong(row["KindPid"]);
            DBParameter[] inputOptionSet = new DBParameter[5];
            DBParameter[] outputOptionSet = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            inputOptionSet[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
            if (optionSetPid > 0)
            {
              inputOptionSet[1] = new DBParameter("@OptionSetPid", DbType.Int64, optionSetPid);
            }
            inputOptionSet[2] = new DBParameter("@OptionSetRequired", DbType.Int32, optionSetRequired);
            inputOptionSet[3] = new DBParameter("@KindPid", DbType.Int64, kindPid);
            inputOptionSet[4] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spCSDItemOptionSet_Edit", inputOptionSet, outputOptionSet);
            if (DBConvert.ParseInt(outputOptionSet[0].Value) == 0)
            {
              success = 0;
            }
          }
         //3. Option Set For Ecat

          DataTable dtOptionSetEcat = new DataTable();
          dtOptionSetEcat.Columns.Add("OptionSetPid", typeof(System.Int64));
          dtOptionSetEcat.Columns.Add("OptionSetRequired", typeof(System.Int32));
          dtOptionSetEcat.Columns.Add("OptionSetMatrixed", typeof(System.Int32));
          dtOptionSetEcat.Columns.Add("KindPid", typeof(System.Int64));
          long oldKindEcat = long.MinValue;
          for (int i = 0; i < ultraGridOptionSetDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            long value = DBConvert.ParseLong(ultraGridOptionSetDetail.Rows[0].Cells[i].Value);
            string colName = ultraGridOptionSetDetail.DisplayLayout.Bands[0].Columns[i].ToString();
            string[] arrColName = colName.Split('|');
            long kind = DBConvert.ParseLong(arrColName[0]);
            if (kind != oldKindEcat)
            {
              dtOptionSetEcat.Rows.Add(dtOptionSetEcat.NewRow());
              oldKindEcat = kind;
            }
            DataRow row = dtOptionSetEcat.Rows[dtOptionSetEcat.Rows.Count - 1];
            row["KindPid"] = kind;
            if (arrColName.Length > 1)
            {
              if (arrColName[1] == "Required")
              {
                row["OptionSetRequired"] = value;
              }
              else //Matrixed
              {
                row["OptionSetMatrixed"] = value;
              }
            }
            else
            {
              row["OptionSetPid"] = value;
            }
          }
          foreach (DataRow row in dtOptionSetEcat.Rows)
          {
            long optionSetPid = DBConvert.ParseLong(row["OptionSetPid"]);
            int optionSetRequired = DBConvert.ParseInt(row["OptionSetRequired"]);
            int optionSetMatrixed = DBConvert.ParseInt(row["OptionSetMatrixed"]);
            long kindPid = DBConvert.ParseLong(row["KindPid"]);
            DBParameter[] inputOptionSet = new DBParameter[6];
            DBParameter[] outputOptionSet = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            inputOptionSet[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
            if (optionSetPid > 0)
            {
              inputOptionSet[1] = new DBParameter("@OptionSetPid", DbType.Int64, optionSetPid);
            }
            inputOptionSet[2] = new DBParameter("@OptionSetRequired", DbType.Int32, optionSetRequired);
            inputOptionSet[3] = new DBParameter("@OptionSetMatrixed", DbType.Int32, optionSetMatrixed);
            inputOptionSet[4] = new DBParameter("@KindPid", DbType.Int64, kindPid);
            inputOptionSet[5] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spCSDItemOptionSetForEcat_Edit", inputOptionSet, outputOptionSet);
            if (DBConvert.ParseInt(outputOptionSet[0].Value) == 0)
            {
              success = 0;
            }
          }
        }
        return (success == 0 ? false : true);
      }
      return false;
    }

    /// <summary>
    /// Save data for Relative Tab
    /// </summary>
    /// <returns></returns>
    private bool SaveRelativeTab()
    {
      bool success = true;
      success = this.SaveMainInfo();
      if (success)
      {
        foreach (long pid in this.listDeletedPid)
        {
          DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
          DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemRelative_Delete", inputDelete, outputDelete);
          if (outputDelete[0].Value.Equals("0"))
          {
            success = false;
          }
        }
        DataTable dtItemRelative = (DataTable)ultraGridItemRelative.DataSource;
        foreach (DataRow row in dtItemRelative.Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            string storeName = string.Empty;
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            DBParameter[] inputParam = new DBParameter[6];
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            if (pid == long.MinValue)
            {
              storeName = "spCSDItemRelative_Insert";
              inputParam[0] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            }
            else
            {
              storeName = "spCSDItemRelative_Update";
              inputParam[0] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              inputParam[4] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            int no = DBConvert.ParseInt(row["No"].ToString());
            string relativeItem = row["ItemCode"].ToString();
            string description = row["Description"].ToString().Trim();
            inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
            inputParam[2] = new DBParameter("@RelativeItem", DbType.AnsiString, 16, relativeItem);
            inputParam[3] = new DBParameter("@Description", DbType.AnsiString, 256, description);
            if (no != int.MinValue)
            {
              inputParam[5] = new DBParameter("@No", DbType.Int32, no);
            }
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if (outputParam[0].Value.Equals("0"))
            {
              success = false;
            }
          }
        }
      }
      return success;
    }

    /// <summary>
    /// Save data for Alternative Tab
    /// </summary>
    /// <returns></returns>
    private bool SaveAlternativeTab()
    {
      bool success = true;
      success = this.SaveMainInfo();
      if (success)
      {
        foreach (long pid in this.listDeletedPid)
        {
          DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
          DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemAlternative_Delete", inputDelete, outputDelete);
          if (outputDelete[0].Value.Equals("0"))
          {
            success = false;
          }
        }
        DataTable dtItemAlternative = (DataTable)ultraGridItemAlternative.DataSource;
        foreach (DataRow row in dtItemAlternative.Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            string storeName = string.Empty;
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            DBParameter[] inputParam = new DBParameter[6];
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            if (pid == long.MinValue)
            {
              storeName = "spCSDItemAlternative_Insert";
              inputParam[0] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            }
            else
            {
              storeName = "spCSDItemAlternative_Update";
              inputParam[0] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              inputParam[4] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            int no = DBConvert.ParseInt(row["No"].ToString());
            string AlternativeItem = row["ItemCode"].ToString();
            string description = row["Description"].ToString().Trim();
            inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
            inputParam[2] = new DBParameter("@AlternativeItem", DbType.AnsiString, 16, AlternativeItem);
            inputParam[3] = new DBParameter("@Description", DbType.AnsiString, 256, description);
            if (no != int.MinValue)
            {
              inputParam[5] = new DBParameter("@No", DbType.Int32, no);
            }
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if (outputParam[0].Value.Equals("0"))
            {
              success = false;
            }
          }
        }
      }
      return success;
    }

    /// <summary>
    /// Save data for Item Language Tab
    /// </summary>
    /// <returns></returns>
    private bool SaveItemLanguageTab()
    {
      bool success = true;
      success = this.SaveMainInfo();
      if (success)
      {
        DataTable dtSource = (DataTable)ultraGridItemLanguage.DataSource;
        foreach (DataRow row in dtSource.Rows)
        {
          if (row.RowState == DataRowState.Modified)
          {
            string itemName = row["ItemName"].ToString().Trim();
            string itemShortName = row["ItemShortName"].ToString().Trim();
            string pageNo = row["PageNo"].ToString().Trim();
            string makertingDescription = row["MakertingDescription"].ToString().Trim();
            string description = row["Description"].ToString().Trim();
            string remark = row["Remark"].ToString().Trim();
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            long languagePid = DBConvert.ParseLong(row["LanguagePid"].ToString());
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            if ((itemName.Length == 0) && (pid != long.MinValue))
            {
              DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
              DataBaseAccess.ExecuteStoreProcedure("spCSDItemInfoLanguage_Delete", inputParam, outputParam);
              if (outputParam[0].Value.Equals(0))
              {
                success = false;
              }
            }
            else if (itemName.Length > 0)
            {
              string storeName = string.Empty;
              DBParameter[] inputParam = new DBParameter[10];
              if (pid > 0) //Update
              {
                inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
                inputParam[1] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                storeName = "spCSDItemInfoLanguage_Update";
              }
              else //Insert
              {
                inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                storeName = "spCSDItemInfoLanguage_Insert";
              }
              inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
              inputParam[3] = new DBParameter("@LanguagePid", DbType.Int64, languagePid);
              inputParam[4] = new DBParameter("@ItemName", DbType.String, 256, itemName);
              inputParam[5] = new DBParameter("@ItemShortName", DbType.String, 128, itemShortName);
              inputParam[6] = new DBParameter("@PageNo", DbType.AnsiString, 8, pageNo);
              inputParam[7] = new DBParameter("@MakertingDescription", DbType.String, 4000, makertingDescription);
              inputParam[8] = new DBParameter("@Description", DbType.String, 4000, description);
              inputParam[9] = new DBParameter("@Remark", DbType.String, 4000, remark);
              DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
              if (outputParam[0].Value.Equals(0))
              {
                success = false;
              }
            }
          }
        }
      }
      return success;
    }

    private bool SaveQuickShipTab()
    {
      bool success = true;
      success = this.SaveMainInfo();
      if (success)
      {
        // 1. Delete
        foreach (long pid in this.listQuickShipDeletedPid)
        {
          DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
          DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemActiveInfomation_Delete", inputDelete, outputDelete);
          if (outputDelete[0].Value.Equals("0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update
        DataTable dtSource = (DataTable)ultraGridQuickShip.DataSource;
        foreach (DataRow row in dtSource.Rows)
        {
          if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
          {
            long customerPid = DBConvert.ParseLong(row["CustomerPid"].ToString());
            int active = DBConvert.ParseInt(row["Active"].ToString());
            int quickShip = DBConvert.ParseInt(row["QuickShip"].ToString());
            DBParameter[] inputParam = new DBParameter[5];
            inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
            inputParam[1] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
            inputParam[2] = new DBParameter("@IsActive", DbType.Int32, active);
            inputParam[3] = new DBParameter("@IsQuickShip", DbType.Int32, quickShip);
            inputParam[4] = new DBParameter("@EditBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spCSDItemActiveInfomation_Edit", inputParam, outputParam);
            if ((int)outputParam[0].Value == 0)
            {
              success = false;
            }
          }
        }
      }
      return success;
    }
    private bool SaveECatTab()
    {
      bool success = true;
      success = this.SaveMainInfo();
      if (success)
      {
        long pid = DBConvert.ParseLong(ultItemOption.Rows[0].Cells[0].Value.ToString());
        string item = this.itemCode;
        string optionCode = string.Empty;
        string kindOption = string.Empty;
        for (int i = 1; i < ultItemOption.Rows[0].Band.Columns.Count; i++)
        {
          string value = ultItemOption.Rows[0].Cells[i].Value.ToString();
          string col = ultItemOption.Rows[0].Cells[i].Column.Key;
          if (value.Length > 0)
          {
            optionCode += "|" + value;
            kindOption += "|" + col;
          }
        }
        DBParameter[] input = new DBParameter[5];
        if (pid != long.MinValue)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, pid);
        }
        input[1] = new DBParameter("@ItemCode", DbType.String, item);
        if (optionCode.Length > 0)
        {
          input[2] = new DBParameter("@OptionCode", DbType.String, optionCode);
        }
        if (kindOption.Length > 0)
        {
          input[3] = new DBParameter("@KindOption", DbType.String, kindOption);
        }
        input[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spCSDECatItemOptionSet_Edit", input, output);
        if (output == null || DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
        {
          success = false;
        }
      }
      return success;
    }
    #endregion Save Data

    /// <summary>
    /// When data change then the changed flag is enabled
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Object_ValueChanged(object sender, EventArgs e)
    {
      if (!this.isDataLoading)
      {
        switch (this.currentTabName)
        {
          case "tabPageGeneral":
            if (!btnSaveGeneral.Visible || !btnSaveGeneral.Enabled)
            {
              this.NeedToSave = false;
              return;
            }
            break;
          case "tabPageQuickShip":
            if (!btnSaveQuickShip.Visible || !btnSaveQuickShip.Enabled)
            {
              this.NeedToSave = false;
              return;
            }
            break;
          case "tabPageRelative":
            if (!btnSaveRelative.Visible || !btnSaveRelative.Enabled)
            {
              this.NeedToSave = false;
              return;
            }
            break;
          case "tabPageAlternative":
            if (!btnSaveAlternative.Visible || !btnSaveAlternative.Enabled)
            {
              this.NeedToSave = false;
              return;
            }
            break;
          default:
            break;
        }
        this.NeedToSave = true;
      }
    }
    #endregion function

    #region event
    /// <summary>
    /// Load current active revision and data in selected tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_002_Load(object sender, EventArgs e)
    {
      this.LoadComboboxType();
      //Tien add
      this.LoadCBBaseItemCode();
      //End
      ControlUtility.LoadUltraCBJC_OEM_Customer(ultraCBCustomer);
      ControlUtility.LoadUltraCBExhibition(ultraCBExhibition);
      ControlUtility.LoadUltraComboCodeMst(ultraCBCatalogue, ConstantClass.GROUP_CATALOGUE);
      DataTable dtActiveRevision = DataBaseAccess.SearchCommandTextDataTable(string.Format("SELECT dbo.FBOMGetActiveRevisionByItemCode('{0}') ActiveRevision", this.itemCode));
      this.LoadDesigner();
      if (dtActiveRevision != null && dtActiveRevision.Rows.Count > 0)
      {
        this.revision = DBConvert.ParseInt(dtActiveRevision.Rows[0]["ActiveRevision"].ToString());
      }
      this.LoadData();
    }

    /// <summary>
    /// Load designer
    /// </summary>
    private void LoadDesigner()
    {
      string commandText = "SELECT Pid, Code + ' - ' + Name Display FROM TblRDDDesigner";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBDesigner.DataSource = dtSource;
      ultCBDesigner.DisplayMember = "Display";
      ultCBDesigner.ValueMember = "Pid";
      ultCBDesigner.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBDesigner.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBDesigner.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load data at selected tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControlItemInfo_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.LoadData();
    }

    /// <summary>
    /// Save data in general tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveGeneral_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        if (this.SaveGeneralTab())
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
      }
    }

    /// <summary>
    /// Save data in item language tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveItemLanguage_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        if (this.SaveItemLanguageTab())
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
      }
    }

    /// <summary>
    /// Save data in quick ship grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveQuickShip_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        if (this.SaveQuickShipTab())
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
      }
    }

    /// <summary>
    /// Save data question before change tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControlItemInfo_Deselecting(object sender, TabControlCancelEventArgs e)
    {
      if (this.NeedToSave)
      {
        DialogResult dlg = MessageBox.Show(FunctionUtility.GetMessage("MSG0008"), "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (dlg == DialogResult.Yes)
        {
          if (!this.CheckInvalid())
          {
            e.Cancel = true;
            return;
          }
          bool success = this.SaveData();
          if (!success)
          {
            WindowUtinity.ShowMessageError("ERR0005");
            this.LoadData();
            e.Cancel = true;
            return;
          }
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else if (dlg == DialogResult.Cancel)
        {
          e.Cancel = true;
        }
      }
    }

    #region Item Relative
    /// <summary>
    /// Check item code
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridItemRelative_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (colName.Equals("ItemCode"))
      {
        string itemRelative = e.Cell.Text.Trim();
        if (itemRelative.Length > 0)
        {
          string commandText = string.Format("Select Count(ItemCode) From TblBOMItemBasic Where ItemCode = '{0}'", itemRelative);
          int result = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText).ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Item Code");
            e.Cancel = true;
          }
        }
      }
      // Truong Add
      else if (colName.Equals("SaleCode"))
      {
        string saleCodeRelative = e.Cell.Text.Trim();
        if (saleCodeRelative.Length > 0)
        {
          string commandText = string.Format("Select Count(SaleCode) From TblBOMItemBasic Where SaleCode = '{0}'", saleCodeRelative);
          int result = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText).ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Sale Code");
            e.Cancel = true;
          }
        }
      }
      // End
    }

    /// <summary>
    /// Show item name
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    bool flagItem = true;
    bool flagSaleCode = true;
    private void ultraGridItemRelative_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      if (btnSaveRelative.Visible && btnSaveRelative.Enabled)
      {
        this.NeedToSave = true;
      }    
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "itemcode":
          {
            if (flagItem == true)
            {
              // SaleCode
              try
              {
                flagSaleCode = false;
                e.Cell.Row.Cells["SaleCode"].Value = ultraDDItemRelative.SelectedRow.Cells["SaleCode"].Value;
                flagSaleCode = true;
              }
              catch
              {
                flagSaleCode = false;
                e.Cell.Row.Cells["SaleCode"].Value = DBNull.Value;
                flagSaleCode = true;
              }

              // Name
              try
              {
                e.Cell.Row.Cells["Name"].Value = ultraDDItemRelative.SelectedRow.Cells["Name"].Value;
              }
              catch
              {
                e.Cell.Row.Cells["Name"].Value = DBNull.Value;
              }
            }
            break;
          }
        //default:
        //  break;
          // Truong Add
        case "salecode":
          {
            if (flagSaleCode == true)
            {
              // Item Code
              try
              {
                flagItem = false;
                e.Cell.Row.Cells["ItemCode"].Value = ultraDDSaleCodeRelative.SelectedRow.Cells["ItemCode"].Value;
                flagItem = true;
              }
              catch
              {
                flagItem = false;
                e.Cell.Row.Cells["ItemCode"].Value = DBNull.Value;
                flagItem = true;
              }

              // Name
              try
              {
                e.Cell.Row.Cells["Name"].Value = ultraDDSaleCodeRelative.SelectedRow.Cells["Name"].Value;
              }
              catch
              {
                e.Cell.Row.Cells["Name"].Value = DBNull.Value;
              }
            }

            break;
          }
        default:
          break;
          // End
      }
    }
    /// <summary>
    /// Set needToSave = true
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridItemLanguage_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (btnSaveItemLanguage.Visible && btnSaveItemLanguage.Enabled)
      {
        this.NeedToSave = true;
      }
    }

    /// <summary>
    /// Set the deleted pid into array list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridItemRelative_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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
          this.listDeletedPid.Add(pid);
          if (btnSaveRelative.Visible && btnSaveRelative.Enabled)
          {
            this.NeedToSave = true;
          }
        }
      }
    }

    /// <summary>
    /// Save data for Item Relative
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveRelative_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        if (this.SaveRelativeTab())
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
    }
    #endregion Item Relative

    /// <summary>
    /// Save data for Item Alternative
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveAlternative_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        if (this.SaveAlternativeTab())
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
    }

    private void btnCloseGeneral_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Tab control costing info of item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControlCostingInfo_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.LoadDataTabCosting();
    }

    #region InitializeLayout
    /// <summary>
    /// InitializeLayout Carcass Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridCarcassInfo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands["CarcassComponent"].Columns["No"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent"].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent"].Columns["FIN_Length"].Width = 100;
      e.Layout.Bands["CarcassComponent"].Columns["FIN_Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent"].Columns["FIN_Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent"].Columns["FIN_Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent"].Columns["FIN_Thickness"].Width = 115;
      e.Layout.Bands["CarcassComponent"].Columns["FingerJoin"].Width = 90;
      e.Layout.Bands["CarcassComponent"].Columns["ContractOut"].Width = 100;
      e.Layout.Bands["CarcassComponent"].Columns["Waste"].Width = 75;
      e.Layout.Bands["CarcassComponent"].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent"].Columns["Lamination"].Width = 85;
      e.Layout.Bands["CarcassComponent"].Columns["Qty"].Width = 60;
      e.Layout.Bands["CarcassComponent"].Columns["ComponentCode"].Header.Caption = "Comp Code";
      e.Layout.Bands["CarcassComponent"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["CarcassComponent"].Columns["RowState"].Hidden = true;
      e.Layout.Bands["CarcassComponent"].Columns["Child"].Hidden = true;
      e.Layout.Bands["CarcassComponent"].Columns["Lamination"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent"].Columns["FingerJoin"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent"].Columns["FingerJoin"].Header.Caption = "Finger Join";
      e.Layout.Bands["CarcassComponent"].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent"].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands["CarcassComponent"].Columns["Primary"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent"].Columns["Specify"].ValueList = udrpSpecify;
      e.Layout.Bands["CarcassComponent"].Columns["Status"].ValueList = udrpStatus;
      e.Layout.Bands["CarcassComponent"].Override.HeaderClickAction = HeaderClickAction.SortMulti;
      e.Layout.Bands["CarcassComponent"].Columns["No"].Width = 30;
      e.Layout.Bands["CarcassComponent"].Columns["DescriptionVN"].Width = 250;
      e.Layout.Bands["CarcassComponent"].Columns["DescriptionVN"].Header.Caption = "VN Description";
      e.Layout.Bands["CarcassComponent"].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent"].Columns["Select"].DefaultCellValue = 0;
      e.Layout.Bands["CarcassComponent"].Columns["Select"].Header.Caption = "Selected";
      e.Layout.Bands["CarcassComponent"].Columns["Select"].Hidden = true;

      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["No"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FIN_Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FIN_Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FIN_Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FIN_Length"].Width = 100;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FIN_Thickness"].Width = 115;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FingerJoin"].Width = 90;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["ContractOut"].Width = 100;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Waste"].Width = 75;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Lamination"].Width = 85;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Qty"].Width = 60;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["SubCompPid"].Header.Caption = "Sub Comp Code";
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["SubCompPid"].ValueList = ultraDDSubComp;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["MainCompPid"].Hidden = true;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["MainCompCode"].Hidden = true;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["RowState"].Hidden = true;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Child"].Hidden = true;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Lamination"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FingerJoin"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FingerJoin"].Header.Caption = "Finger Join";
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Primary"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Specify"].ValueList = udrpSpecify;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Status"].ValueList = udrpStatus;
      e.Layout.Bands["CarcassComponent_SubComponent"].Override.HeaderClickAction = HeaderClickAction.SortMulti;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["No"].Width = 30;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["DescriptionVN"].Width = 250;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["DescriptionVN"].Header.Caption = "VN Description";
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Select"].DefaultCellValue = 0;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Select"].Hidden = true;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["DescriptionVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FIN_Length"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FIN_Width"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FIN_Thickness"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Lamination"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["FingerJoin"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Specify"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["ContractOut"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Primary"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["CarcassComponent_SubComponent"].Columns["Waste"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["QtyCombine"].Header.Caption = "Qty Combine";
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["RAW_Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["RAW_Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["RAW_Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["QtyCombine"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["ComponentPid"].Hidden = true;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["RowState"].Hidden = true;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["Child"].Hidden = true;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["IDFactoryUnit"].Hidden = true;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["MaterialCode"].ValueList = udrpMaterialsCode;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["FactoryUnit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["FactoryUnit"].Header.Caption = "Factory Unit";
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["Alternative"].ValueList = udrpAlternative;
      e.Layout.Bands["SubComponent_SubComponentDetail"].Columns["MaterialName"].Width = 250;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// InitializeLayout Hardware Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridHardwareInfo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      //Gan Dropdown vao grid
      e.Layout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompHardware;
      e.Layout.Bands[0].Columns["Comp_Alter"].ValueList = ultraDDAlternative;

      //Edit lai Grid        
      e.Layout.Bands[0].Columns["PID"].Hidden = true;

      e.Layout.Bands[0].Columns["Material_Alter"].Header.Caption = "Material Alternative";
      e.Layout.Bands[0].Columns["Comp_Alter"].Header.Caption = "Comp Alternative";

      e.Layout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Columns["ComponentCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Comp Code";
      e.Layout.Bands[0].Columns["ComponentName"].Header.Caption = "Comp Name";
      e.Layout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands[0].Columns["ComponentName"].Header.Caption = "Total Qty";
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AlterRevision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }

    /// <summary>
    /// InitializeLayout Costing Hardware, Glass Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridCostingHardware_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Bands[0].Columns["Code"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Code"].MaxValue = 80;
      e.Layout.Bands[0].Columns["Name"].MinWidth = 350;
      e.Layout.Bands[0].Columns["Name"].MaxValue = 350;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Total"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Total"].Header.Caption = "Amount";
      e.Layout.Bands[0].Columns["Total"].Format = "#,###.##";

      e.Layout.Bands[1].Columns["Code"].Hidden = true;

      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[1].Columns["MaterialCode"].MaxWidth = 110;
      e.Layout.Bands[1].Columns["MaterialCode"].MinWidth = 110;

      e.Layout.Bands[1].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[1].Columns["MaterialName"].MaxWidth = 350;
      e.Layout.Bands[1].Columns["MaterialName"].MinWidth = 350;

      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Price"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Price"].Format = "#,###.##";
      e.Layout.Bands[1].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Total"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Total"].Header.Caption = "Amount";
      e.Layout.Bands[1].Columns["Total"].Format = "#,###.##";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Total"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
    }

    /// <summary>
    /// InitializeLayout Costing Support Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridCostingSupport_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Unit"].Hidden = true;
      e.Layout.Bands[0].Columns["Qty"].Hidden = true;
      e.Layout.Bands[0].Columns["Waste"].Hidden = true;
      e.Layout.Bands[0].Columns["Total"].Hidden = true;
      e.Layout.Bands[0].Columns["Total"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].Format = "#,###.##";
      e.Layout.Bands[1].Columns["Code"].Hidden = true;
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[1].Columns["MaterialCode"].MaxWidth = 110;
      e.Layout.Bands[1].Columns["MaterialCode"].MinWidth = 110;
      e.Layout.Bands[1].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[1].Columns["MaterialName"].MaxWidth = 350;
      e.Layout.Bands[1].Columns["MaterialName"].MinWidth = 350;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Price"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Price"].Format = "#,###.##";
      e.Layout.Bands[1].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Total"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Total"].Header.Caption = "Amount";
      e.Layout.Bands[1].Columns["Total"].Format = "#,###.##";
    }

    /// <summary>
    /// InitializeLayout Costing Carcass Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridCostingLocalCarcass_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["MaterialName"].MaxWidth = 300;
      e.Layout.Bands[0].Columns["MaterialName"].MinWidth = 300;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Total"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Total"].Header.Caption = "Amount";
      e.Layout.Bands[0].Columns["Total"].Format = "#,###.##";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Total"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
    }

    /// <summary>
    /// InitializeLayout Costing Direct Labour
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridCostingDirectLabour_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Bands[0].Columns["SectionCode"].Header.Caption = "Section Code";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].Format = "#,###.##";
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Total"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Total"].Header.Caption = "Amount";
      e.Layout.Bands[0].Columns["Total"].Format = "#,###.##";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Total"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
    }

    /// <summary>
    /// InitializeLayout Item Language Tab
    /// </summary>
    /// <param name="sender"></param> 
    /// <param name="e"></param>
    private void ultraGridItemLanguage_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LanguagePid"].Hidden = true;
      e.Layout.Bands[0].Columns["Language"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Language"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Language"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Name";
      e.Layout.Bands[0].Columns["ItemShortName"].Header.Caption = "Short Name";
      e.Layout.Bands[0].Columns["PageNo"].Header.Caption = "Page No";
      e.Layout.Bands[0].Columns["PageNo"].MinWidth = 60;
      e.Layout.Bands[0].Columns["PageNo"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["MakertingDescription"].Header.Caption = "Makerting Description";
    }

    private void ultraDDCustomerInfo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["CustomerCode"].MaxWidth = 100;
    }

    private void ultraGridQuickShip_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerPid"].ValueList = ultraDDCustomerInfo;
      e.Layout.Bands[0].Columns["CustomerPid"].Header.Caption = "Customer Code";
      e.Layout.Bands[0].Columns["CustomerName"].Header.Caption = "Customer Name";
      e.Layout.Bands[0].Columns["CustomerName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Active"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["QuickShip"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["QuickShip"].Header.Caption = "Quick Ship";
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
    }
    #endregion InitializeLayout

    private void btnCostingPrint_Click(object sender, EventArgs e)
    {
      FunctionUtility.ViewItemCosting(itemCode, 0); 
    }

    private void ultraGridQuickShip_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare(column, "CustomerPid", true) == 0)
      {
        string customerCode = e.Cell.Text.Trim();
        if (customerCode.Length > 0)
        {
          string command = string.Format("Select Count(Pid) From TblCSDCustomerInfo Where CustomerCode = '{0}'", customerCode);
          object obj = DataBaseAccess.ExecuteScalarCommandText(command);
          if (obj != null)
          {
            if ((int)obj == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Customer Code");
              e.Cancel = true;
            }
          }
        }
      }
    }

    private void ultraGridQuickShip_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare(column, "CustomerPid", true) == 0)
      {
        if (ultraDDCustomerInfo.SelectedRow != null)
        {
          e.Cell.Row.Cells["CustomerName"].Value = ultraDDCustomerInfo.SelectedRow.Cells["Name"].Value.ToString();
        }
        else
        {
          e.Cell.Row.Cells["CustomerName"].Value = DBNull.Value;
        }
      }
      if (btnSaveQuickShip.Visible && btnSaveQuickShip.Enabled)
      {
        this.NeedToSave = true;
      }
    }

    private void ultraGridQuickShip_CellChange(object sender, CellEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare(column, "QuickShip", true) == 0)
      {
        int quickShip = DBConvert.ParseInt(e.Cell.Row.Cells["QuickShip"].Text);
        if (quickShip == 1)
        {
          e.Cell.Row.Cells["Active"].Value = 1;
          e.Cell.Row.Cells["Active"].Activation = Activation.ActivateOnly;
        }
        else
        {
          e.Cell.Row.Cells["Active"].Activation = Activation.AllowEdit;
        }
      }
    }

    bool flagItemAlternative = true;
    bool flagSaleCodeAlternative = true;
    private void ultraGridItemAlternative_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (btnSaveAlternative.Visible && btnSaveAlternative.Enabled)
      {
        this.NeedToSave = true;
      }
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "itemcode":
          if (flagItemAlternative == true)
          {
            // SaleCode
            try
            {
              flagSaleCodeAlternative = false;
              e.Cell.Row.Cells["SaleCode"].Value = ultraDDItemAlternative.SelectedRow.Cells["SaleCode"].Value;
              flagSaleCodeAlternative = true;
            }
            catch
            {
              flagSaleCodeAlternative = false;
              e.Cell.Row.Cells["SaleCode"].Value = DBNull.Value;
              flagSaleCodeAlternative = true;
            }

            // Name
            try
            {
              e.Cell.Row.Cells["Name"].Value = ultraDDItemAlternative.SelectedRow.Cells["Name"].Value;
            }
            catch
            {
              e.Cell.Row.Cells["Name"].Value = DBNull.Value;
            }
          }
          break;
        // Truong Add
        case "salecode":
          {
            if (flagSaleCodeAlternative == true)
            {
              // Item Code
              try
              {
                flagItemAlternative = false;
                e.Cell.Row.Cells["ItemCode"].Value = ultraDDSaleCodeAlternative.SelectedRow.Cells["ItemCode"].Value;
                flagItemAlternative = true;
              }
              catch
              {
                flagItemAlternative = false;
                e.Cell.Row.Cells["ItemCode"].Value = DBNull.Value;
                flagItemAlternative = true;
              }

              // Name
              try
              {
                e.Cell.Row.Cells["Name"].Value = ultraDDSaleCodeAlternative.SelectedRow.Cells["Name"].Value;
              }
              catch
              {
                e.Cell.Row.Cells["Name"].Value = DBNull.Value;
              }
            }
            break;
          }
        default:
          break;
          //End
      }
    }

    private void ultraCBExhibition_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void ultraGridQuickShip_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listQuickShipDeletedPid.Add(pid);
          if (btnSaveQuickShip.Visible && btnSaveQuickShip.Enabled)
          {
            this.NeedToSave = true;
          }
        }
      }
    }

    private void ultraGridItemRoom_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RoomPid"].Header.Caption = "Room";
      e.Layout.Bands[0].Columns["RoomPid"].ValueList = ultraDDRoom;
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
    }

    private void ultraGridItemRoom_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (btnSaveGeneral.Visible && btnSaveGeneral.Enabled)
      {
        this.NeedToSave = true;
      }
    }

    private void ultraGridItemRoom_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listItemRoomDeletedPid.Add(pid);
          if (btnSaveGeneral.Visible && btnSaveGeneral.Enabled)
          {
            this.NeedToSave = true;
          }
        }
      }
    }

    private void ultraGridOptionSet_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      DataTable dtOptionKind = DataBaseAccess.SearchCommandTextDataTable("Select Pid, Name From TblCSDOptionSetKind");
      if (dtOptionKind != null && dtOptionKind.Rows.Count > 0)
      {
        for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
        {
          string colName = e.Layout.Bands[0].Columns[i].ToString();
          string[] arrColName = colName.Split('|');
          string optionKind = (arrColName.Length > 1 ? arrColName[0] : colName);
          DataRow[] rows = dtOptionKind.Select(string.Format("Pid = {0}", optionKind));
          string header = (rows.Length > 0 ? rows[0]["Name"].ToString() : string.Empty);
          //header = (arrColName.Length > 1 ? string.Format("{0} {1}", header, arrColName[1]) : header);
          if (arrColName.Length > 1)
          {
            header = "Required";
            e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            e.Layout.Bands[0].Columns[i].MinWidth = 60;
            e.Layout.Bands[0].Columns[i].MaxWidth = 60;
          }
          e.Layout.Bands[0].Columns[i].Header.Caption = header;

          // Load Ultra Dropdown for Option Set
          long optionSetKind = DBConvert.ParseLong(colName);
          if (optionSetKind > 0)
          {
            UltraDropDown ultraDDOptionSet = (UltraDropDown)e.Layout.Bands[0].Columns[i].ValueList;
            e.Layout.Bands[0].Columns[i].ValueList = this.LoadUltraDDOptionSet(optionSetKind, ultraDDOptionSet);
          }
        }
      }
    }

    private void ultraGridOptionSet_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (btnSaveGeneral.Visible && btnSaveGeneral.Enabled)
      {
        this.NeedToSave = true;
      }
    }

    private void ultraGridOptionSet_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();      
      long optionSetKind = DBConvert.ParseLong(colName);
      if (optionSetKind > 0 && e.Cell.Text.Trim().Length > 0)
      {
        DataTable dtOptionSet = (DataTable)((UltraDropDown)e.Cell.Column.ValueList).DataSource;
        if (dtOptionSet.Select(string.Format("Pid = {0}", DBConvert.ParseLong(e.NewValue))).Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0029", "Option Set");
          e.Cancel = true;
        }
      }
    }
    #endregion event

    private void ultraDDRoom_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {

    }

    private void ultraGridOptionSetDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      DataTable dtOptionKind = DataBaseAccess.SearchCommandTextDataTable("Select Pid, Name From TblCSDOptionSetKindEcat");
      if (dtOptionKind != null && dtOptionKind.Rows.Count > 0)
      {
        for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
        {
          string colName = e.Layout.Bands[0].Columns[i].ToString();
          string[] arrColName = colName.Split('|');
          string optionKind = (arrColName.Length > 1 ? arrColName[0] : colName);
          DataRow[] rows = dtOptionKind.Select(string.Format("Pid = {0}", optionKind));
          string header = (rows.Length > 0 ? rows[0]["Name"].ToString() : string.Empty);
          string header1 = (rows.Length > 0 ? rows[0]["Name"].ToString() : string.Empty);

          int a = arrColName.Length;
          //header = (arrColName.Length > 1 ? string.Format("{0} {1} ", header, arrColName[1]) : header);

          if (arrColName.Length > 1 && arrColName[1].ToString() == "Required")
          {
            header = "Required";
            e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            e.Layout.Bands[0].Columns[i].MinWidth = 60;
            e.Layout.Bands[0].Columns[i].MaxWidth = 60;
          }
          else if(arrColName.Length > 1 && arrColName[1].ToString() == "Matrixed")
          {
            header = "Matrixed";
            e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            e.Layout.Bands[0].Columns[i].MinWidth = 60;
            e.Layout.Bands[0].Columns[i].MaxWidth = 60;
          }
           e.Layout.Bands[0].Columns[i].Header.Caption = header;
        
          // Load Ultra Dropdown for Option Set
          long optionSetKindEcat = DBConvert.ParseLong(colName);
          if (optionSetKindEcat > 0)
          {
            UltraDropDown ultraDDOptionSetEcat = (UltraDropDown)e.Layout.Bands[0].Columns[i].ValueList;
            e.Layout.Bands[0].Columns[i].ValueList = this.LoadUltraDDOptionSetEcat(optionSetKindEcat, ultraDDOptionSetEcat);
          }
        }
      }
    }

    private void ultraGridOptionSetDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (btnSaveGeneral.Visible && btnSaveGeneral.Enabled)
      {
        this.NeedToSave = true;
      }
      //string colName = e.Cell.Column.ToString();
      //DataTable dt = (DataTable)ultraGridOptionSetDetail.DataSource;
      //long value = DBConvert.ParseLong(ultraGridOptionSetDetail.Rows[0].Cells[colName].Value);            
      //string columnUpdate;
      //if ((colName.Contains("Required") || colName.Contains("Matrixed")) && value == 1)
      //{
      //  if (colName.Contains("Required"))
      //  {
      //    columnUpdate = colName.Replace("Required", "Matrixed");          
      //  }
      //  else
      //  {
      //    columnUpdate = colName.Replace("Matrixed", "Required");          
      //  }
      //  e.Cell.Row.Cells[columnUpdate].Value = 0;
      //}
    }

    private void txtSaleCode_Leave(object sender, EventArgs e)
    {
      if (txtSaleCode.Text.Trim().Length > 16)
      {
        WindowUtinity.ShowMessageErrorFromText("NewSaleCode length must be less than 16 characters");
        txtSaleCode.Text = "";
        txtSaleCode.Focus();
      }
    }

    private void ultItemOption_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      DataTable dtOptionKind = DataBaseAccess.SearchCommandTextDataTable("Select Pid, Name From TblCSDOptionSetKindEcat");
      if (dtOptionKind != null && dtOptionKind.Rows.Count > 0)
      {
        for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
        {
          string colName = e.Layout.Bands[0].Columns[i].ToString();
          DataRow[] rows = dtOptionKind.Select(string.Format("Pid = {0}", colName));
          string header = (rows.Length > 0 ? rows[0]["Name"].ToString() : string.Empty);
          e.Layout.Bands[0].Columns[i].Header.Caption = header;

          // Load Ultra Dropdown for Option Set
          int optionSetKindEcat = DBConvert.ParseInt(colName);
          if (optionSetKindEcat > 0)
          {
            UltraDropDown ultraDDOptionSetEcat = (UltraDropDown)e.Layout.Bands[0].Columns[i].ValueList;
            e.Layout.Bands[0].Columns[i].ValueList = this.LoadOptionCode(optionSetKindEcat, ultraDDOptionSetEcat);
          }
        }
      }
    }

    private void btnSaveTabECAT_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        if (this.SaveECatTab())
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
      }
    }
  }
}
