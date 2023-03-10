using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Technical;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using PresentationControls;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_01_003 : MainUserControl
  {
    public int iIndex = 0;
    private bool loaded;
    private string itemCode = string.Empty;
    private int revision = int.MinValue;
    public string currItemCode;
    public int currRevision;
    private string tempDelete;
    private string deletePid;
    private DataTable dtSourceMaterials = new DataTable();
    private bool isUpdateData = true;
    private bool isDuplicateCompo = false;
    public viewBOM_01_003()
    {
      InitializeComponent();
    }

    private void viewBOM_01_003_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + Shared.Utility.SharedObject.UserInfo.UserName + " | " + Shared.Utility.SharedObject.UserInfo.LoginDate;
      loaded = false;
      this.InitTabData();
    }

    private void InitTabData()
    {
      loaded = false;
      Utility.LoadUltraDropdownCodeMst(ultraDDWorkArea, ConstantClass.GROUP_FOUNDY_WORK_AREA);
      switch (iIndex)
      {
        case 9:
          this.InitDataCarcass();
          break;
        case 1:
          this.InitDataHardware();
          break;
        case 2:
          this.InitDataGlass();
          break;
        case 3:
          this.InitDataSupport();
          break;
        case 4:
          this.InitDataAccessory();
          break;
        case 5:
          this.InitDataUpholstery();
          break;
        case 6:
          this.InitDataFinish();
          break;
        case 7:
          this.InitDataPackage();
          break;
        case 8:
          this.InitDataLabour();
          break;
        default:
          break;
      }
    }

    private void ShowTextConfirmed()
    {
      lbNotConfirmed.Visible = false;
      string commandText = string.Format("Select Confirm From TblBOMItemInfo Where ItemCode = '{0}' And Revision = {1}", this.itemCode, this.revision);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        int confirm = DBConvert.ParseInt(dt.Rows[0]["Confirm"].ToString());
        if (confirm == 0)
        {
          lbNotConfirmed.Visible = true;
        }
      }
    }

    #region Load DropDownList
    private void LoadDropDownHardware(Infragistics.Win.UltraWinGrid.UltraDropDown ultraDDComp, string orderBy)
    {
      //Load data Dropdown Comp theo Group
      string commandText = string.Format("Select (Code + '|' + ISNULL(Cast(Revision as varchar), '')) as DisplayCode, Code, Name, Revision From VBOMComponent Where CompGroup = {0} Order By '{1}' Asc", iIndex, orderBy);
      DataTable dtComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDComp.DataSource = dtComp;
      ultraDDComp.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDComp.ValueMember = "DisplayCode";
      ultraDDComp.DisplayMember = "Code";
      ultraDDComp.DisplayLayout.Bands[0].Columns["DisplayCode"].Hidden = true;
      ultraDDComp.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
      ultraDDComp.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 90;
      ultraDDComp.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 90;
    }

    private void LoadDropDownComp(Infragistics.Win.UltraWinGrid.UltraDropDown ultraDDComp, string orderBy)
    {
      //Load data Dropdown Comp theo Group
      string commandText = string.Format("Select Code, Name From VBOMComponent Where CompGroup = {0} Order By '{1}' Asc", iIndex, orderBy);
      DataTable dtComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDComp.DataSource = dtComp;
      ultraDDComp.ValueMember = "Code";
      ultraDDComp.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDComp.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 90;
      ultraDDComp.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 90;
    }

    private void LoadDropdownCodeMst(UltraDropDown udrpDropDown, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value, Description FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpDropDown.DataSource = dtSource;
      udrpDropDown.ValueMember = "Code";
      udrpDropDown.DisplayMember = "Value";
      udrpDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;

    }

    private void MakeMaterialsData()
    {
      string commandText = "SELECT MaterialCode, MaterialName, MaterialNameVn, IDFactoryUnit, FactoryUnit FROM VBOMMaterialsForCarcassComponent ORDER BY MaterialCode";
      this.dtSourceMaterials = DataBaseAccess.SearchCommandTextDataTable(commandText);
    }

    private void LoadDropdownMaterial(UltraDropDown udrpMaterials)
    {
      udrpMaterials.DataSource = this.dtSourceMaterials;
      udrpMaterials.ValueMember = "MaterialCode";
      udrpMaterials.DisplayMember = "MaterialCode";
      udrpMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpMaterials.DisplayLayout.Bands[0].Columns["IDFactoryUnit"].Hidden = true;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialName"].Width = 200;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialNameVN"].Width = 200;

    }
    #endregion Load DropDownList

    private void btnClose_Click(object sender, EventArgs e)
    {
      if (this.NeedToSave && (this.SaveBeforeClosing(this.iIndex) == DialogResult.No))
      {
        this.NeedToSave = false;
      }
      this.ConfirmToCloseTab();
    }

    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (tabControl1.SelectedIndex)
      {
        case 0:
          iIndex = 9;
          break;
        default:
          iIndex = tabControl1.SelectedIndex;
          break;
      }
      InitTabData();
    }

    #region Load Grid Data
    private void LoadGridHardwareInfo()
    {
      try
      {
        deletePid = string.Empty;
        string commandText = string.Format("Select INFO.Confirm, INFO.CarcassCode, BS.Name as Description From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
        DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtInfo.Rows.Count > 0)
        {
          txtCarcassHarware.Text = dtInfo.Rows[0]["CarcassCode"].ToString();
          txtDescriptionHarware.Text = dtInfo.Rows[0]["Description"].ToString();
          if (dtInfo.Rows[0]["Confirm"].ToString() == "1")
          {
            btnSaveHardware.Enabled = false;
            btnPrintHardware.Enabled = true;
          }
          else
          {
            btnSaveHardware.Enabled = true;
            btnPrintHardware.Enabled = false;
          }
        }

        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabHardwareInfo", inputParam);

        ultraGridHardwareInfo.DataSource = dtSource;
        lbCount.Text = string.Format("Count: {0}", dtSource.Rows.Count);
        //Gan Dropdown vao grid
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompHardware;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["ComponentName"].ValueList = ultraDDCompHardwareName;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

        //Edit lai Grid        
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].Header.Caption = "Component Code";
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["ComponentName"].Header.Caption = "Component Name";
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Revision"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Revision"].CellAppearance.BackColor = Color.LightGray;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Length"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.BackColor = Color.LightGray;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Width"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.BackColor = Color.LightGray;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.BackColor = Color.LightGray;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.BackColor = Color.LightGray;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.LightGray;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["Comp"].Hidden = true;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Hidden = true;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["ContractOut"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridHardwareInfo.DisplayLayout.Bands[0].Columns["ContractOut"].CellAppearance.BackColor = Color.LightGray;
      }
      catch
      {
        txtCarcassHarware.Text = string.Empty;
        txtDescriptionHarware.Text = string.Empty;
        ultraGridHardwareInfo.DataSource = null;
      }
      this.NeedToSave = false;
      //btnSaveHardware.Enabled = false;
    }
    private void LoadGridAccessoryInfo()
    {
      try
      {
        deletePid = string.Empty;
        string commandText = string.Format("Select INFO.Confirm, INFO.CarcassCode, BS.Name as Description From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
        DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtInfo.Rows.Count > 0)
        {
          txtCarcassAccessory.Text = dtInfo.Rows[0]["CarcassCode"].ToString();
          txtDescriptionAccessory.Text = dtInfo.Rows[0]["Description"].ToString();
          if (dtInfo.Rows[0]["Confirm"].ToString() == "1")
          {
            btnSaveAccessory.Enabled = false;
            btnPrintAccessory.Enabled = true;
          }
          else
          {
            btnSaveAccessory.Enabled = true;
            btnPrintAccessory.Enabled = false;
          }
        }

        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
        inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
        DataTable dtComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfo", inputParam);
        DataTable dtCompDetail = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfoDetail", inputParam);
        DataSet dsSource = CreateDataSet.TabComponent();
        //dsSource.Tables.Add(dtComp.Clone());
        dsSource.Tables["TblBOMComponentInfo"].Merge(dtComp);
        //dsSource.Tables.Add(dtCompDetail.Clone());
        dsSource.Tables["TblBOMComponentInfoDetail"].Merge(dtCompDetail);
        dsSource.Tables["TblBOMComponentInfo"].Columns["Qty"].AllowDBNull = false;
        ultraGridAccessoryInfo.DataSource = dsSource;
        lbCount.Text = string.Format("Count: {0}", dsSource.Tables["TblBOMComponentInfo"].Rows.Count);
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

        //Gan Dropdown vao grid
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompAccessory;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ComponentName"].ValueList = ultraDDCompAccessoryName;

        //Edit lai Grid        
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].Header.Caption = "Component Code";
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ComponentName"].Header.Caption = "Component Name";
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Length"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.BackColor = Color.LightGray;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Width"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.BackColor = Color.LightGray;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.BackColor = Color.LightGray;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].CellAppearance.BackColor = Color.LightGray;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.BackColor = Color.LightGray;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.LightGray;
        ultraGridAccessoryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Hidden = true;

        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["ComponentCode"].Hidden = true;
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["MaterialName"].Header.Caption = "Material Name";
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["TotalQty"].Header.Caption = "Total Qty";
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridAccessoryInfo.DisplayLayout.Bands[1].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      }
      catch
      {
        txtCarcassAccessory.Text = string.Empty;
        txtDescriptionAccessory.Text = string.Empty;
        ultraGridAccessoryInfo.DataSource = null;
        btnSaveAccessory.Enabled = false;
      }
      this.NeedToSave = false;
      //btnSaveAccessory.Enabled = false;
    }
    private void LoadGridUpholsteryInfo()
    {
      try
      {
        deletePid = string.Empty;
        string commandText = string.Format("Select INFO.Confirm, INFO.CarcassCode, BS.Name as Description From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
        DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtInfo.Rows.Count > 0)
        {
          txtCarcassUphol.Text = dtInfo.Rows[0]["CarcassCode"].ToString();
          txtDescriptionUphol.Text = dtInfo.Rows[0]["Description"].ToString();
          if (dtInfo.Rows[0]["Confirm"].ToString() == "1")
          {
            btnSaveUphol.Enabled = false;
            btnPrintUpholstery.Enabled = true;
          }
          else
          {
            btnSaveUphol.Enabled = true;
            btnPrintUpholstery.Enabled = false;
          }
        }

        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
        inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
        DataTable dtComp = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfo", inputParam);
        DataTable dtCompDetail = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabCompInfoDetail", inputParam);
        DataSet dsSource = CreateDataSet.TabComponent();
        dsSource.Tables["TblBOMComponentInfo"].Merge(dtComp);
        dsSource.Tables["TblBOMComponentInfoDetail"].Merge(dtCompDetail);
        dsSource.Tables["TblBOMComponentInfo"].Columns["Qty"].AllowDBNull = false;
        ultraGridUpholsteryInfo.DataSource = dsSource;
        lbCount.Text = string.Format("Count: {0}", dsSource.Tables["TblBOMComponentInfo"].Rows.Count);
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

        //Gan Dropdown vao grid
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompUphol;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ComponentName"].ValueList = ultraDDCompUpholName;

        //Edit lai Grid        
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].Header.Caption = "Component Code";
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ComponentName"].Header.Caption = "Component Name";
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Length"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.BackColor = Color.LightGray;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Width"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.BackColor = Color.LightGray;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.BackColor = Color.LightGray;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].CellAppearance.BackColor = Color.LightGray;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.BackColor = Color.LightGray;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.LightGray;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[0].Columns["ContractOut"].Hidden = true;

        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["ComponentCode"].Hidden = true;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["MaterialName"].Header.Caption = "Material Name";
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["TotalQty"].Header.Caption = "Total Qty";
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridUpholsteryInfo.DisplayLayout.Bands[1].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      }
      catch
      {
        txtCarcassUphol.Text = string.Empty;
        txtDescriptionUphol.Text = string.Empty;
        ultraGridUpholsteryInfo.DataSource = null;
        btnSaveUphol.Enabled = false;
      }
      this.NeedToSave = false;
      //btnSaveUphol.Enabled = false;
    }
    private void LoadGridFinishingInfo()
    {
      try
      {
        deletePid = string.Empty;
        string commandText = string.Format("Select INFO.Confirm, INFO.CarcassCode, BS.Name as Description From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
        DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
        string carcassCode = string.Empty;
        if (dtInfo.Rows.Count > 0)
        {
          carcassCode = dtInfo.Rows[0]["CarcassCode"].ToString();
          txtCarcassFinish.Text = carcassCode;          
          if (dtInfo.Rows[0]["Confirm"].ToString() == "1")
          {
            btnSaveFinish.Enabled = false;
            btnPrintFinishing.Enabled = true;
          }
          else
          {
            btnSaveFinish.Enabled = true;
            btnPrintFinishing.Enabled = false;
          }
        }

        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
        inputParam[2] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
        DataSet dsFinishingInfo = DataBaseAccess.SearchStoreProcedure("spBOMListTabFinishingInfo", inputParam);
        if (dsFinishingInfo.Tables.Count >= 3)
        {
          dsFinishingInfo.Relations.Add(new DataRelation("Relation1", dsFinishingInfo.Tables[0].Columns["FinishProcessMasterPid"], dsFinishingInfo.Tables[1].Columns["FinishProcessMasterPid"], false));
          dsFinishingInfo.Relations.Add(new DataRelation("Relation2", dsFinishingInfo.Tables[1].Columns["ChemicalPid"], dsFinishingInfo.Tables[2].Columns["ChemicalPid"], false));
          ultraGridFinishingInfo.DataSource = dsFinishingInfo;
        }
        lbCount.Text = string.Format("Count: {0}", dsFinishingInfo.Tables[0].Rows.Count);

        // Load Carcass finishing information
        DBParameter[] inputParamCarcass = new DBParameter[] { new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode) };
        DataTable dtCarcassFinish = DataBaseAccess.SearchStoreProcedureDataTable("spBOMCarcassCompFinishInfo", inputParamCarcass);
        ugdCarcassFinishing.DataSource = dtCarcassFinish;
      }
      catch (Exception ex)
      {
        string a = ex.Message.ToString();
        txtCarcassFinish.Text = string.Empty;        
        ultraGridFinishingInfo.DataSource = null;
        btnSaveFinish.Enabled = false;
      }
      this.NeedToSave = false;
      //btnSaveFinish.Enabled = false;
    }
    private void LoadGridSupportInfo()
    {
      try
      {
        deletePid = string.Empty;
        string commandText = string.Format("SELECT BS.Name, INFO.Confirm, INFO.CarcassCode, INFO.SupCode	FROM TblBOMItemInfo INFO INNER JOIN dbo.TblBOMItemBasic BS ON INFO.ItemCode = BS.ItemCode AND INFO.ItemCode = '{0}' AND INFO.Revision = {1} ", itemCode, revision);
        DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtInfo.Rows.Count > 0)
        {
          txtCarcassSupport.Text = dtInfo.Rows[0]["CarcassCode"].ToString();
          txtDescriptionSupport.Text = dtInfo.Rows[0]["Name"].ToString();
          try
          {
            multiCBSupport.SelectedValue = dtInfo.Rows[0]["SupCode"].ToString();
          }
          catch
          {
          }
          if (dtInfo.Rows[0]["Confirm"].ToString() == "1")
          {
            btnSaveSupport.Enabled = false;
            btnPrintSupport.Enabled = true;
          }
          else
          {
            btnSaveSupport.Enabled = true;
            btnPrintSupport.Enabled = false;
          }
        }
      }
      catch
      {
        txtCarcassSupport.Text = string.Empty;
        txtDescriptionSupport.Text = string.Empty;
        ultraGridSupportInfo.DataSource = null;
        btnSaveSupport.Enabled = false;
      }
      ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Depth"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      this.NeedToSave = false;
      //btnSaveSupport.Enabled = false;
    }
    private void LoadGridGlassInfo()
    {
      try
      {
        deletePid = string.Empty;
        string commandText = string.Format("Select INFO.Confirm, INFO.CarcassCode, BS.Name as Description From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
        DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtInfo.Rows.Count > 0)
        {
          txtCarcassGlass.Text = dtInfo.Rows[0]["CarcassCode"].ToString();
          txtDescriptionGlass.Text = dtInfo.Rows[0]["Description"].ToString();
          if (dtInfo.Rows[0]["Confirm"].ToString() == "1")
          {
            btnSaveGlass.Enabled = false;
            btnPrintGlass.Enabled = true;
          }
          else
          {
            btnSaveGlass.Enabled = true;
            btnPrintGlass.Enabled = false;
          }
        }

        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
        DataTable dtGlassInfo = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListItemGlassInfo", inputParam);

        string commandTextAdj = "Select Code, Description From TblBOMCodeMaster Where [Group] = 11";
        DataTable dtAdj = DataBaseAccess.SearchCommandTextDataTable(commandTextAdj);

        DataColumn column = new DataColumn();
        column.DataType = System.Type.GetType("System.String");
        column.ColumnName = "GlassType";
        dtGlassInfo.Columns.Add(column);

        // Load Du lieu vao column Glass, Mirror Specification
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
              dr["GlassType"] += objCM.Description + ", ";
            }
          }
          string strTemp = dr["GlassType"].ToString().Trim();
          if (strTemp.Length > 0)
          {
            strTemp = dr["Bevel"].ToString().Trim() + ", " + strTemp.TrimEnd(',');
            strTemp = strTemp.TrimStart(',');
            strTemp = strTemp.Trim();
          }

          dr["GlassType"] = strTemp;
        }
        dtGlassInfo.Columns["Qty"].AllowDBNull = false;
        ultraGridGlassInfo.DataSource = dtGlassInfo;
        lbCount.Text = string.Format("Count: {0}", dtGlassInfo.Rows.Count);

        //Gan Dropdown vao grid
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompGlass;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["ComponentName"].ValueList = ultraDDCompGlassName;

        //Edit lai Grid        
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["GlassAdj"].Hidden = true;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["ComponentCode"].Header.Caption = "Component Code";
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["ComponentName"].Header.Caption = "Component Name";
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = false;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.LightGray;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.BackColor = Color.LightGray;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Length"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.BackColor = Color.LightGray;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Width"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.BackColor = Color.LightGray;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.BackColor = Color.LightGray;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Remark"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.LightGray;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Type"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Type"].CellAppearance.BackColor = Color.LightGray;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["Bevel"].Hidden = true;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["GlassAdj"].Hidden = true;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["GlassType"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["GlassType"].CellAppearance.BackColor = Color.LightGray;
        ultraGridGlassInfo.DisplayLayout.Bands[0].Columns["GlassType"].Header.Caption = "Adjective";
      }
      catch
      {
        txtCarcassGlass.Text = string.Empty;
        txtDescriptionGlass.Text = string.Empty;
        ultraGridGlassInfo.DataSource = null;
        btnSaveGlass.Enabled = false;
      }
      this.NeedToSave = false;
      //btnSaveGlass.Enabled = false;
    }
    private void LoadTreeViewCarcassInfo()
    {
      try
      {
        deletePid = string.Empty;
        string commandText = string.Format("Select INFO.Confirm, INFO.CarcassCode, BS.Name as Description From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
        DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtInfo.Rows.Count > 0)
        {
          try
          {
            multiCBCarcass.SelectedValue = dtInfo.Rows[0]["CarcassCode"].ToString();
          }
          catch
          {
            multiCBCarcass.SelectedIndex = -1;
          }
          txtDescriptionCarcass.Text = dtInfo.Rows[0]["Description"].ToString();
          if (dtInfo.Rows[0]["Confirm"].ToString() == "1")
          {
            btnSaveCarcass.Enabled = false;
          }
          else
          {
            btnSaveCarcass.Enabled = true;
          }
        }
      }
      catch
      {
        multiCBCarcass.SelectedIndex = -1;
        txtDescriptionCarcass.Text = string.Empty;
        treeViewComponentStruct.Nodes.Clear();
        btnSaveCarcass.Enabled = false;
      }
      this.NeedToSave = false;
      //btnSaveCarcass.Enabled = false;
    }
    private void LoadGridPackageInfo()
    {
      try
      {
        deletePid = string.Empty;
        string commandText = string.Format("Select INFO.Confirm, INFO.PackageCode, BS.Name as Description, INFO.CarcassCode From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
        DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtInfo.Rows.Count > 0)
        {
          try
          {
            multiCBPackage.SelectedValue = dtInfo.Rows[0]["PackageCode"].ToString();
          }
          catch
          {
            multiCBPackage.SelectedIndex = 0;
          }
          txtCarcassPackage.Text = dtInfo.Rows[0]["CarcassCode"].ToString();
          txtDescriptionPackage.Text = dtInfo.Rows[0]["Description"].ToString();
          if (dtInfo.Rows[0]["Confirm"].ToString() == "1")
          {
            btnSavePackage.Enabled = false;
            btnPrintPacking.Enabled = true;
            this.multiCBPackage.Enabled = false;
            this.btnNewPackage.Enabled = false;
          }
          else
          {
            btnSavePackage.Enabled = true;
            btnPrintPacking.Enabled = false;
            this.multiCBPackage.Enabled = true;
            this.btnNewPackage.Enabled = true;
          }
        }
      }
      catch
      {
        multiCBPackage.SelectedIndex = -1;
        txtDescriptionPackage.Text = string.Empty;
        ultraGridPackageInfo.DataSource = null;
        btnSavePackage.Enabled = false;
      }
      lbCount.Text = string.Empty;
      this.NeedToSave = false;
      //btnSavePackage.Enabled = false;

    }
    private void LoadGridLabourInfo()
    {
      try
      {
        deletePid = string.Empty;
        string commandText = string.Format("Select INFO.Confirm, INFO.CarcassCode, BS.Name as Description From TblBOMItemInfo INFO Inner Join TblBOMItemBasic BS ON (BS.ItemCode = INFO.ItemCode) And INFO.ItemCode = '{0}' And INFO.Revision = {1}", itemCode, revision);
        DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtInfo.Rows.Count > 0)
        {
          txtCarcassLabour.Text = dtInfo.Rows[0]["CarcassCode"].ToString();
          txtDescriptionLabour.Text = dtInfo.Rows[0]["Description"].ToString();
          if (dtInfo.Rows[0]["Confirm"].ToString() == "1")
          {
            btnSaveLabour.Enabled = false;
            btnPrintDirectLabour.Enabled = true;
          }
          else
          {
            btnSaveLabour.Enabled = true;
            btnPrintDirectLabour.Enabled = false;
          }
        }

        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
        DataTable dtDirectLabour = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabDirectLabourInfo", inputParam);
        ultraGridLabourInfo.DataSource = dtDirectLabour;
        lbCount.Text = string.Format("Count: {0}", dtDirectLabour.Rows.Count);
      }
      catch
      {
        txtCarcassLabour.Text = string.Empty;
        txtDescriptionLabour.Text = string.Empty;
        ultraGridLabourInfo.DataSource = null;
        btnSaveLabour.Enabled = false;
      }
      this.NeedToSave = false;
      //btnSaveLabour.Enabled = false;
    }

    //TIEN ADD
    private DataTable GetDataRefLevel2nd(string itemcode, int revision, int compgroup)
    {
      string storeName = "spBOMGetDataRefLevel2nd";
      DBParameter[] input = new DBParameter[3];
      input[0] = new DBParameter("@ItemCode", DbType.String, itemcode);
      input[1] = new DBParameter("@Revision", DbType.Int32, revision);
      input[2] = new DBParameter("@CompGroup", DbType.Int32, compgroup);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, input);
      return dtSource;
    }

    #endregion Load Grid Data

    #region Init Data
    private void InitDataHardware()
    {
      Utility.LoadItemComboBox(ultraCBItemHardware);

      //Tien Add Item Ref
      // Load CB ref item
      string cmdText = @"SELECT INF.ItemCode, INF.Revision, BS.Name, INF.ItemCode + '|' + CAST(INF.Revision AS varchar) Value,
                         INF.ItemCode + ' - ' + CAST(INF.Revision AS varchar) + ' - ' + BS.Name Display
                         FROM TblBOMItemInfo INF INNER JOIN TblBOMItemBasic BS ON INF.ItemCode = BS.ItemCode  AND INF.Revision = BS.RevisionActive";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cmdText);
      ultCBItemRefHW.DataSource = dtSource;
      ultCBItemRefHW.ValueMember = "Value";
      ultCBItemRefHW.DisplayMember = "Display";
      ultCBItemRefHW.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultCBItemRefHW.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
      ultCBItemRefHW.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      ultCBItemRefHW.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      ultCBItemRefHW.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      ultCBItemRefHW.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 50;
      ultCBItemRefHW.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 50;

      //Load data Dropdown Comp Hardware
      this.LoadDropDownHardware(ultraDDCompHardware, "Code");
      this.LoadDropDownHardware(ultraDDCompHardwareName, "Name");
      ultraDDCompHardwareName.DisplayMember = "Name";

      loaded = true;
      ultraCBItemHardware.Value = currItemCode;
      ultraCBRevisionHardware.Value = currRevision;
      this.ShowTextConfirmed();
    }
    private void InitDataAccessory()
    {
      Utility.LoadItemComboBox(multiCBItemAccessory);
      multiCBItemAccessory.ColumnWidths = "100, 400, 0";

      //Tien Add Item Ref
      // Load CB ref item
      string cmdText = @"SELECT INF.ItemCode, INF.Revision, BS.Name, INF.ItemCode + '|' + CAST(INF.Revision AS varchar) Value,
                         INF.ItemCode + ' - ' + CAST(INF.Revision AS varchar) + ' - ' + BS.Name Display
                         FROM TblBOMItemInfo INF INNER JOIN TblBOMItemBasic BS ON INF.ItemCode = BS.ItemCode  AND INF.Revision = BS.RevisionActive";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cmdText);
      ultCBItemRefACC.DataSource = dtSource;
      ultCBItemRefACC.ValueMember = "Value";
      ultCBItemRefACC.DisplayMember = "Display";
      ultCBItemRefACC.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultCBItemRefACC.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
      ultCBItemRefACC.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      ultCBItemRefACC.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      ultCBItemRefACC.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      ultCBItemRefACC.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 50;
      ultCBItemRefACC.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 50;

      //Load data Dropdown Comp Hardware
      this.LoadDropDownComp(ultraDDCompAccessory, "Code");
      this.LoadDropDownComp(ultraDDCompAccessoryName, "Name");
      ultraDDCompAccessoryName.DisplayMember = "Name";

      loaded = true;
      try
      {
        multiCBItemAccessory.SelectedValue = currItemCode;
      }
      catch
      {
      }
      try
      {
        cmbRevisionAccessory.SelectedValue = currRevision;
      }
      catch
      {
      }
      this.ShowTextConfirmed();
    }
    private void InitDataUpholstery()
    {
      Utility.LoadItemComboBox(multiCBItemUphol);
      multiCBItemUphol.ColumnWidths = "100, 400, 0";
      //Tien Add Item Ref
      // Load CB ref item
      string cmdText = @"SELECT INF.ItemCode, INF.Revision, BS.Name, INF.ItemCode + '|' + CAST(INF.Revision AS varchar) Value,
                         INF.ItemCode + ' - ' + CAST(INF.Revision AS varchar) + ' - ' + BS.Name Display
                         FROM TblBOMItemInfo INF INNER JOIN TblBOMItemBasic BS ON INF.ItemCode = BS.ItemCode  AND INF.Revision = BS.RevisionActive";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cmdText);
      ultCBItemRefUph.DataSource = dtSource;
      ultCBItemRefUph.ValueMember = "Value";
      ultCBItemRefUph.DisplayMember = "Display";
      ultCBItemRefUph.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultCBItemRefUph.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
      ultCBItemRefUph.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      ultCBItemRefUph.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      ultCBItemRefUph.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      ultCBItemRefUph.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 50;
      ultCBItemRefUph.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 50;

      //Load data Dropdown Comp Upholstery
      this.LoadDropDownComp(ultraDDCompUphol, "Code");
      this.LoadDropDownComp(ultraDDCompUpholName, "Name");
      ultraDDCompUpholName.DisplayMember = "Name";

      loaded = true;
      try
      {
        multiCBItemUphol.SelectedValue = currItemCode;
      }
      catch
      {
      }
      try
      {
        cmbRevisionUphol.SelectedValue = currRevision;
      }
      catch
      {
      }
      this.ShowTextConfirmed();
    }

    private UltraCombo LoadComboFinishProcess(string finishCode, UltraCombo ucb)
    {
      if (ucb == null)
      {
        ucb = new UltraCombo();
        this.Controls.Add(ucb);
      }
      string cm = string.Format(@"SELECT PRO.Pid, PRO.ProcessCode, PRO.[Description], SUP.VietnameseName SupplierName, IsDefault, (PRO.ProcessCode + ISNULL(' | ' + PRO.[Description], ''))  DisplayText
                                  FROM TblBOMFinishingProcessMaster PRO
	                                  LEFT JOIN TblPURSupplierInfo SUP ON PRO.SupplierPid = SUP.Pid
                                  WHERE FinishingCode = '{0}'", finishCode);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      if (dt.Rows.Count > 0)
      {
        Utility.LoadUltraCombo(ucb, dt, "Pid", "DisplayText", true, new string[] { "Pid", "IsDefault", "DisplayText" });
        ucb.Width = 500;
        ucb.DisplayLayout.Bands[0].Columns["ProcessCode"].Header.Caption = "Process Code";
        ucb.DisplayLayout.Bands[0].Columns["SupplierName"].Header.Caption = "Supplier Name";
        ucb.DisplayLayout.Bands[0].Columns["ProcessCode"].Width = 80;
      }
      return ucb;
    }
    private void InitDataFinish()
    {
      Utility.LoadItemComboBox(multiCBItemFinish);
      multiCBItemFinish.ColumnWidths = "100, 400, 0";

      //Load data Dropdown Comp Finish            
      string commandText = string.Format("Select Code, Name, (Code + ' | ' + Name) DisplayText From VBOMComponent Where CompGroup = {0}", iIndex);
      DataTable dtComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraDDCompFinish, dtComp, "Code", "DisplayText", false, "DisplayText");
      ultraDDCompFinish.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;

      loaded = true;
      try
      {
        multiCBItemFinish.SelectedValue = currItemCode;
      }
      catch
      {
      }
      try
      {
        cmbRevisionFinish.SelectedValue = currRevision;
      }
      catch
      {
      }
      this.ShowTextConfirmed();
    }
    private void InitDataSupport()
    {
      Utility.LoadItemComboBox(multiCBItemSupport);

      //Load support code
      string commandText = string.Format("Select Code SupCode, Name Description, (Code + ' | ' + Name) as SupDesc From VBOMComponent Where CompGroup = {0}", iIndex);
      DataTable dtSupport = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadMultiCombobox(multiCBSupport, dtSupport, "SupCode", "SupDesc");
      multiCBSupport.ColumnWidths = "120, 400, 0";
      loaded = true;
      try
      {
        multiCBItemSupport.SelectedValue = currItemCode;
      }
      catch
      {
      }
      try
      {
        cmbRevisionSupport.SelectedValue = currRevision;
      }
      catch
      {
      }
      this.ShowTextConfirmed();
    }
    private void InitDataGlass()
    {
      Utility.LoadItemComboBox(multiCBItemGlass);
      multiCBItemGlass.ColumnWidths = "100, 400, 0";

      //Load data Dropdown Comp Glass
      string commandText = string.Format("Select Code, Name From VBOMComponent Where CompGroup = {0}", iIndex);
      DataTable dtComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDCompGlass.DataSource = dtComp;
      ultraDDCompGlass.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDCompGlass.DisplayLayout.Bands[0].Columns["Name"].Width = 300;

      Utility.LoadUltraDropDown(ultraDDCompGlassName, dtComp, "Code", "Name");
      ultraDDCompGlassName.DisplayLayout.Bands[0].ColHeadersVisible = false;
      loaded = true;

      // Load Combobox ref item
      string cmdText = @"SELECT INF.ItemCode, INF.Revision, BS.Name, INF.ItemCode + '|' + CAST(INF.Revision AS varchar) Value,
                         INF.ItemCode + ' - ' + CAST(INF.Revision AS varchar) + ' - ' + BS.Name Display
                         FROM TblBOMItemInfo INF INNER JOIN TblBOMItemBasic BS ON INF.ItemCode = BS.ItemCode  AND INF.Revision = BS.RevisionActive";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cmdText);
      ultCBItemRefGL.DataSource = dtSource;
      ultCBItemRefGL.ValueMember = "Value";
      ultCBItemRefGL.DisplayMember = "Display";
      ultCBItemRefGL.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultCBItemRefGL.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
      ultCBItemRefGL.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      ultCBItemRefGL.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      ultCBItemRefGL.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      ultCBItemRefGL.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 50;
      ultCBItemRefGL.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 50;

      try
      {
        multiCBItemGlass.SelectedValue = currItemCode;
      }
      catch
      {
      }
      try
      {
        cmbRevisionGlass.SelectedValue = currRevision;
      }
      catch
      {
      }
      this.ShowTextConfirmed();
    }
    private void InitDataCarcass()
    {
      this.MakeMaterialsData();

      Utility.LoadItemComboBox(ultraCBItemCarcass);

      //Load data ComboBox Carcass            
      string commandText = "Select CarcassCode, Description, (CarcassCode + ' | ' + Description) DescCarcass From TblBOMCarcass Where DeleteFlag = 0";
      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadMultiCombobox(multiCBCarcass, dtCarcass, "CarcassCode", "DescCarcass");
      multiCBCarcass.ColumnWidths = "120, 400, 0";

      string commandTextCar = string.Format(@"SELECT A.ComponentCode, I.DescriptionVN ComponentName, CASE WHEN A.[Status] = 0 THEN 'Lost'
							 WHEN A.[Status] = 1 THEN 'Valid' 
							 WHEN A.[Status] = 2 THEN 'Invalid' END Status, CurrentLocation
FROM TblQADMasterInfo A
	LEFT JOIN TblBOMCarcassComponent I ON A.ComponentCode = I.ComponentCode AND A.CarcassCode = I.CarcassCode
WHERE Kind = 2 AND ItemCode = '{0}'", currItemCode);
      DataTable dtCar = DataBaseAccess.SearchCommandTextDataTable(commandTextCar);
      ultCarving.DataSource = dtCar;

      string commandTextChair = string.Format(@"SELECT A.ItemCode, D.Name ItemName, CASE WHEN A.[Status] = 0 THEN 'Lost'
							                                                 WHEN A.[Status] = 1 THEN 'Valid' 
							                                                 WHEN A.[Status] = 2 THEN 'Invalid' END StatusRemark, CurrentLocation
                                                FROM TblQADMasterInfo A
	                                                LEFT JOIN TblBOMItemBasic D ON A.ItemCode = D.ItemCode AND A.Revision = D.RevisionActive
                                                WHERE Kind = 1 AND A.ItemCode = '{0}' AND A.Revision = {1}", currItemCode, currRevision);
      DataTable dtChair = DataBaseAccess.SearchCommandTextDataTable(commandTextChair);
      ultChairMaster.DataSource = dtChair;

      loaded = true;
      ultraCBItemCarcass.Value = currItemCode;
      try
      {
        cmbRevisionCarcass.SelectedValue = currRevision;
      }
      catch
      {
      }
      this.ShowTextConfirmed();
    }
    private void InitDataPackage()
    {
      Utility.LoadItemComboBox(multiCBItemPackage);
      multiCBItemPackage.ColumnWidths = "100, 400, 0";

      //Load data ComboBox Package
      string commandText = string.Format("Select PackageCode, PackageName, (PackageCode + ' | ' + PackageName) DescPackage From TblBOMPackage Where ItemCode = '{0}' And Revision = {1}", currItemCode, currRevision);
      DataTable dtPackage = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadMultiCombobox(multiCBPackage, dtPackage, "PackageCode", "DescPackage");
      multiCBPackage.ColumnWidths = "120, 400, 0";

      loaded = true;
      try
      {
        multiCBItemPackage.SelectedValue = currItemCode;
      }
      catch
      {
      }
      try
      {
        cmbRevisionPackage.SelectedValue = currRevision;
      }
      catch
      {
      }
      this.ShowTextConfirmed();
    }
    private void InitDataLabour()
    {
      Utility.LoadItemComboBox(multiCBItemLabour);
      multiCBItemLabour.ColumnWidths = "100, 400, 0";

      // Load CB ref item
      string cmdText = @"SELECT INF.ItemCode, INF.Revision, BS.Name, INF.ItemCode + '|' + CAST(INF.Revision AS varchar) Value,
                         INF.ItemCode + ' - ' + CAST(INF.Revision AS varchar) + ' - ' + BS.Name Display
                         FROM TblBOMItemInfo INF INNER JOIN TblBOMItemBasic BS ON INF.ItemCode = BS.ItemCode  AND INF.Revision = BS.RevisionActive";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cmdText);
      ultCBItemRef.DataSource = dtSource;
      ultCBItemRef.ValueMember = "Value";
      ultCBItemRef.DisplayMember = "Display";
      ultCBItemRef.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultCBItemRef.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
      ultCBItemRef.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      ultCBItemRef.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      ultCBItemRef.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      ultCBItemRef.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 50;
      ultCBItemRef.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 50;

      //Load data Dropdown Labour            
      string commandText = "Select Code, NameEN From VBOMSectionForItem ORDER BY Code";
      DataTable dtLabour = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDLabour.DataSource = dtLabour;
      ultraDDLabour.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDLabour.DisplayLayout.Bands[0].Columns[0].Width = 150;
      ultraDDLabour.DisplayLayout.Bands[0].Columns[1].Width = 400;

      loaded = true;
      try
      {
        multiCBItemLabour.SelectedValue = currItemCode;
      }
      catch
      {
      }
      try
      {
        cmbRevisionLabour.SelectedValue = currRevision;
      }
      catch
      {
      }
      this.ShowTextConfirmed();
    }
    #endregion Init Data

    #region After Cell Update
    private void ultraGridHardwareInfo_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      int index = e.Cell.Row.Index;
      int revisionComp = int.MinValue;
      string compCode = string.Empty;
      DataTable dtCompInfo;
      string commandText;

      switch (columnName)
      {
        case "ComponentCode":
          if (this.isUpdateData)
          {
            this.isUpdateData = false;
            if (ultraDDCompHardware.SelectedRow != null)
            {
              revisionComp = DBConvert.ParseInt(ultraDDCompHardware.SelectedRow.Cells["Revision"].Value.ToString());
              compCode = ultraDDCompHardware.SelectedRow.Cells["Code"].Value.ToString();
            }
            commandText = string.Format(
              @"SELECT DT.CompNameEN Name, COMP.Revision, COMP.[Length], COMP.Width, COMP.Thickness, WorkArea, ISNULL(DT.IsContractOut, 0) ContractOut, DT.MaterialCode
              FROM TblFOUComponentRevisionInfo COMP
	              INNER JOIN TblFOUComponentInfo DT ON COMP.CompCode = DT.CompCode
              WHERE COMP.CompCode = '{0}'", compCode);
            if (revisionComp != int.MinValue)
            {
              commandText += string.Format(" AND Revision = {0}", revisionComp);
            }

            dtCompInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCompInfo != null && dtCompInfo.Rows.Count > 0)
            {
              ultraGridHardwareInfo.Rows[index].Cells["ComponentName"].Value = dtCompInfo.Rows[0]["Name"];
              ultraGridHardwareInfo.Rows[index].Cells["Revision"].Value = dtCompInfo.Rows[0]["Revision"];
              ultraGridHardwareInfo.Rows[index].Cells["Length"].Value = dtCompInfo.Rows[0]["Length"];
              ultraGridHardwareInfo.Rows[index].Cells["Width"].Value = dtCompInfo.Rows[0]["Width"];
              ultraGridHardwareInfo.Rows[index].Cells["Thickness"].Value = dtCompInfo.Rows[0]["Thickness"];
              ultraGridHardwareInfo.Rows[index].Cells["WorkAreaPid"].Value = dtCompInfo.Rows[0]["WorkArea"];
              ultraGridHardwareInfo.Rows[index].Cells["ContractOut"].Value = dtCompInfo.Rows[0]["ContractOut"];
              ultraGridHardwareInfo.Rows[index].Cells["MaterialCode"].Value = dtCompInfo.Rows[0]["MaterialCode"];
            }
            else
            {
              ultraGridHardwareInfo.Rows[index].Cells["ComponentName"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["Revision"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["Length"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["Width"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["Thickness"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["WorkAreaPid"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["ContractOut"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["MaterialCode"].Value = DBNull.Value;
            }
            this.isUpdateData = true;
          }
          //this.CheckComponentDuplicate(ultraGridHardwareInfo);
          break;
        case "ComponentName":
          if (this.isUpdateData)
          {
            this.isUpdateData = false;
            if (ultraDDCompHardwareName.SelectedRow != null)
            {
              revisionComp = DBConvert.ParseInt(ultraDDCompHardwareName.SelectedRow.Cells["Revision"].Value.ToString());
              compCode = ultraDDCompHardwareName.SelectedRow.Cells["Code"].Value.ToString();
            }
            commandText = string.Format(
              @"SELECT DT.CompCode Code, COMP.Revision, COMP.[Length], COMP.Width, COMP.Thickness, WorkArea, ISNULL(DT.IsContractOut, 0) ContractOut, DT.MaterialCode
              FROM TblFOUComponentRevisionInfo COMP
	              INNER JOIN TblFOUComponentInfo DT ON COMP.CompCode = DT.CompCode
              WHERE DT.CompCode = '{0}'", compCode);
            if (revisionComp != int.MinValue)
            {
              commandText += string.Format(" AND Revision = {0}", revisionComp);
            }

            dtCompInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCompInfo != null && dtCompInfo.Rows.Count > 0)
            {
              ultraGridHardwareInfo.Rows[index].Cells["ComponentCode"].Value = dtCompInfo.Rows[0]["Code"];
              ultraGridHardwareInfo.Rows[index].Cells["Revision"].Value = dtCompInfo.Rows[0]["Revision"];
              ultraGridHardwareInfo.Rows[index].Cells["Length"].Value = dtCompInfo.Rows[0]["Length"];
              ultraGridHardwareInfo.Rows[index].Cells["Width"].Value = dtCompInfo.Rows[0]["Width"];
              ultraGridHardwareInfo.Rows[index].Cells["Thickness"].Value = dtCompInfo.Rows[0]["Thickness"];
              ultraGridHardwareInfo.Rows[index].Cells["WorkAreaPid"].Value = dtCompInfo.Rows[0]["WorkArea"];
              ultraGridHardwareInfo.Rows[index].Cells["ContractOut"].Value = dtCompInfo.Rows[0]["ContractOut"];
              ultraGridHardwareInfo.Rows[index].Cells["MaterialCode"].Value = dtCompInfo.Rows[0]["MaterialCode"];
            }
            else
            {
              ultraGridHardwareInfo.Rows[index].Cells["ComponentCode"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["Revision"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["Length"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["Width"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["Thickness"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["WorkAreaPid"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["ContractOut"].Value = DBNull.Value;
              ultraGridHardwareInfo.Rows[index].Cells["MaterialCode"].Value = DBNull.Value;
            }
            this.isUpdateData = true;
          }
          break;
        case "Qty":
        case "Waste":
          double qty, waste;
          try
          {
            qty = double.Parse(ultraGridHardwareInfo.Rows[index].Cells["Qty"].Value.ToString());
          }
          catch
          {
            qty = 0;
          }
          try
          {
            waste = double.Parse(ultraGridHardwareInfo.Rows[index].Cells["Waste"].Value.ToString());
          }
          catch
          {
            waste = 0;
          }
          ultraGridHardwareInfo.Rows[index].Cells["TotalQty"].Value = (qty * (1 + waste));
          break;
        default:
          break;
      }
      this.NeedToSave = true;
    }
    private void ultraGridAccessoryInfo_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      int index = e.Cell.Row.Index;
      string compCode = string.Empty;
      string commandText;
      switch (columnName)
      {
        case "ComponentCode":
          if (this.isUpdateData)
          {
            this.isUpdateData = false;
            if (ultraDDCompAccessory.SelectedRow != null)
            {
              compCode = ultraDDCompAccessory.SelectedRow.Cells["Code"].Value.ToString();
            }
            commandText = string.Format(@"Select ComponentName, COM.Length, COM.Width, COM.Thickness, ContractOut, COM.Alternative MaterialCode
                          From TblBOMComponentInfo COM LEFT JOIN TblBOMComponentInfoDetail COMD ON COM.Pid = COMD.ComponentInfoPID
                          Where ComponentCode = '{0}' And CompGroup = {1}", compCode, ConstantClass.COMP_ACCESSORY);
            DataTable dtCompInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCompInfo != null && dtCompInfo.Rows.Count > 0)
            {
              ultraGridAccessoryInfo.Rows[index].Cells["ComponentName"].Value = dtCompInfo.Rows[0]["ComponentName"];
              ultraGridAccessoryInfo.Rows[index].Cells["MaterialCode"].Value = dtCompInfo.Rows[0]["MaterialCode"];
              ultraGridAccessoryInfo.Rows[index].Cells["Length"].Value = dtCompInfo.Rows[0]["Length"];
              ultraGridAccessoryInfo.Rows[index].Cells["Width"].Value = dtCompInfo.Rows[0]["Width"];
              ultraGridAccessoryInfo.Rows[index].Cells["Thickness"].Value = dtCompInfo.Rows[0]["Thickness"];
              ultraGridAccessoryInfo.Rows[index].Cells["ContractOut"].Value = dtCompInfo.Rows[0]["ContractOut"];
            }
            else
            {
              ultraGridAccessoryInfo.Rows[index].Cells["ComponentName"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["MaterialCode"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["Length"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["Width"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["Thickness"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["ContractOut"].Value = DBNull.Value;
            }
            //this.CheckComponentDuplicate(ultraGridAccessoryInfo);
            this.isUpdateData = true;
          }
          break;
        case "ComponentName":
          if (this.isUpdateData)
          {
            this.isUpdateData = false;
            if (ultraDDCompAccessoryName.SelectedRow != null)
            {
              compCode = ultraDDCompAccessoryName.SelectedRow.Cells["Code"].Value.ToString();
            }
            commandText = string.Format(@"Select ComponentCode, COM.Length, COM.Width, COM.Thickness, ContractOut, COM.Alternative MaterialCode
                          From TblBOMComponentInfo COM LEFT JOIN TblBOMComponentInfoDetail COMD ON COM.Pid = COMD.ComponentInfoPID
                          Where ComponentCode = '{0}' And CompGroup = {1}", compCode, ConstantClass.COMP_ACCESSORY);
            DataTable dtCompInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCompInfo != null && dtCompInfo.Rows.Count > 0)
            {
              ultraGridAccessoryInfo.Rows[index].Cells["ComponentCode"].Value = dtCompInfo.Rows[0]["ComponentCode"];
              ultraGridAccessoryInfo.Rows[index].Cells["MaterialCode"].Value = dtCompInfo.Rows[0]["MaterialCode"];
              ultraGridAccessoryInfo.Rows[index].Cells["Length"].Value = dtCompInfo.Rows[0]["Length"];
              ultraGridAccessoryInfo.Rows[index].Cells["Width"].Value = dtCompInfo.Rows[0]["Width"];
              ultraGridAccessoryInfo.Rows[index].Cells["Thickness"].Value = dtCompInfo.Rows[0]["Thickness"];
              ultraGridAccessoryInfo.Rows[index].Cells["ContractOut"].Value = dtCompInfo.Rows[0]["ContractOut"];
            }
            else
            {
              ultraGridAccessoryInfo.Rows[index].Cells["ComponentCode"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["MaterialCode"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["Length"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["Width"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["Thickness"].Value = DBNull.Value;
              ultraGridAccessoryInfo.Rows[index].Cells["ContractOut"].Value = DBNull.Value;
            }
            this.isUpdateData = true;
          }
          break;
        case "Qty":
        case "Waste":
          double qty, waste;
          try
          {
            qty = double.Parse(ultraGridAccessoryInfo.Rows[index].Cells["Qty"].Value.ToString());
          }
          catch
          {
            qty = 0;
          }
          try
          {
            waste = double.Parse(ultraGridAccessoryInfo.Rows[index].Cells["Waste"].Value.ToString());
          }
          catch
          {
            waste = 0;
          }
          ultraGridAccessoryInfo.Rows[index].Cells["TotalQty"].Value = (qty * (1 + waste));
          break;
        default:
          break;
      }
      this.NeedToSave = true;
    }
    private void ultraGridUpholsteryInfo_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      int index = e.Cell.Row.Index;
      string compCode = string.Empty;
      string commandText;
      switch (columnName)
      {
        case "ComponentCode":
          if (this.isUpdateData)
          {
            this.isUpdateData = false;
            if (ultraDDCompUphol.SelectedRow != null)
            {
              compCode = ultraDDCompUphol.SelectedRow.Cells["Code"].Value.ToString();
            }
            commandText = string.Format(@"Select ComponentName, COM.Length, COM.Width, COM.Thickness, ContractOut, COM.Alternative MaterialCode
          From TblBOMComponentInfo COM
	          LEFT JOIN TblBOMComponentInfoDetail COMD ON COM.Pid = COMD.ComponentInfoPID
          Where ComponentCode = '{0}' And CompGroup = {1}", compCode, ConstantClass.COMP_UPHOLSTERY);
            DataTable dtCompInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCompInfo != null && dtCompInfo.Rows.Count > 0)
            {
              ultraGridUpholsteryInfo.Rows[index].Cells["ComponentName"].Value = dtCompInfo.Rows[0]["ComponentName"];
              ultraGridUpholsteryInfo.Rows[index].Cells["MaterialCode"].Value = dtCompInfo.Rows[0]["MaterialCode"];
              ultraGridUpholsteryInfo.Rows[index].Cells["Length"].Value = dtCompInfo.Rows[0]["Length"];
              ultraGridUpholsteryInfo.Rows[index].Cells["Width"].Value = dtCompInfo.Rows[0]["Width"];
              ultraGridUpholsteryInfo.Rows[index].Cells["Thickness"].Value = dtCompInfo.Rows[0]["Thickness"];
              ultraGridUpholsteryInfo.Rows[index].Cells["ContractOut"].Value = dtCompInfo.Rows[0]["ContractOut"];
            }
            else
            {
              ultraGridUpholsteryInfo.Rows[index].Cells["ComponentName"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["Length"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["Width"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["Thickness"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["ContractOut"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["MaterialCode"].Value = DBNull.Value;
            }
            this.CheckComponentDuplicate(ultraGridUpholsteryInfo);
            this.isUpdateData = true;
          }
          break;
        case "ComponentName":
          if (this.isUpdateData)
          {
            this.isUpdateData = false;
            if (ultraDDCompUpholName.SelectedRow != null)
            {
              compCode = ultraDDCompUpholName.SelectedRow.Cells["Code"].Value.ToString();
            }
            commandText = string.Format(@"Select ComponentCode, COM.Length, COM.Width, COM.Thickness, ContractOut, COM.Alternative MaterialCode
              From TblBOMComponentInfo COM
                LEFT JOIN TblBOMComponentInfoDetail COMD ON COM.Pid = COMD.ComponentInfoPID
              Where ComponentCode = '{0}' And CompGroup = {1}", compCode, ConstantClass.COMP_UPHOLSTERY);
            DataTable dtCompInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCompInfo != null && dtCompInfo.Rows.Count > 0)
            {
              ultraGridUpholsteryInfo.Rows[index].Cells["ComponentCode"].Value = dtCompInfo.Rows[0]["ComponentCode"];
              ultraGridUpholsteryInfo.Rows[index].Cells["MaterialCode"].Value = dtCompInfo.Rows[0]["MaterialCode"];
              ultraGridUpholsteryInfo.Rows[index].Cells["Length"].Value = dtCompInfo.Rows[0]["Length"];
              ultraGridUpholsteryInfo.Rows[index].Cells["Width"].Value = dtCompInfo.Rows[0]["Width"];
              ultraGridUpholsteryInfo.Rows[index].Cells["Thickness"].Value = dtCompInfo.Rows[0]["Thickness"];
              ultraGridUpholsteryInfo.Rows[index].Cells["ContractOut"].Value = dtCompInfo.Rows[0]["ContractOut"];
            }
            else
            {
              ultraGridUpholsteryInfo.Rows[index].Cells["ComponentCode"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["Length"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["Width"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["Thickness"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["ContractOut"].Value = DBNull.Value;
              ultraGridUpholsteryInfo.Rows[index].Cells["MaterialCode"].Value = DBNull.Value;
            }
            this.CheckComponentDuplicate(ultraGridUpholsteryInfo);
            this.isUpdateData = true;
          }
          break;
        case "Qty":
        case "Waste":
          {
            double qty, waste;
            try
            {
              qty = double.Parse(ultraGridUpholsteryInfo.Rows[index].Cells["Qty"].Value.ToString());
            }
            catch
            {
              qty = 0;
            }
            try
            {
              waste = double.Parse(ultraGridUpholsteryInfo.Rows[index].Cells["Waste"].Value.ToString());
            }
            catch
            {
              waste = 0;
            }
            ultraGridUpholsteryInfo.Rows[index].Cells["TotalQty"].Value = (qty * (1 + waste));
          }
          break;
        default:
          break;
      }
      this.NeedToSave = true;
    }
    private void ultraGridFinishingInfo_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      int index = e.Cell.Row.Index;
      if (columnName == "ComponentCode")
      {        
        this.CheckComponentDuplicate(ultraGridFinishingInfo);

        string finishCode = e.Cell.Row.Cells["ComponentCode"].Value.ToString();        
        UltraCombo ultc = (UltraCombo)e.Cell.Row.Cells["FinishProcessMasterPid"].ValueList;
        ultc = LoadComboFinishProcess(finishCode, ultc);
        e.Cell.Row.Cells["FinishProcessMasterPid"].ValueList = ultc;
        DataTable dt = (DataTable)ultc.DataSource;
        if (dt != null)
        {
          DataRow[] rows = dt.Select("IsDefault = 1");
          if (rows.Length > 0)
          {
            e.Cell.Row.Cells["FinishProcessMasterPid"].Value = DBConvert.ParseLong(rows[0]["Pid"]);
          }
          else
          {
            e.Cell.Row.Cells["FinishProcessMasterPid"].Value = DBNull.Value;
          }
        }
        else
        {
          e.Cell.Row.Cells["FinishProcessMasterPid"].Value = DBNull.Value;
        }  
      }
      else if ((columnName == "Qty") || (columnName == "Waste"))
      {
        double qty, waste;
        try
        {
          qty = double.Parse(ultraGridFinishingInfo.Rows[index].Cells["Qty"].Value.ToString());
        }
        catch
        {
          qty = 0;
        }
        try
        {
          waste = double.Parse(ultraGridFinishingInfo.Rows[index].Cells["Waste"].Value.ToString());
        }
        catch
        {
          waste = 0;
        }
        ultraGridFinishingInfo.Rows[index].Cells["TotalQty"].Value = (qty * (1 + waste));
      }
      this.NeedToSave = true;
      //btnSaveFinish.Enabled = true;
    }
    private void ultraGridGlassInfo_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      int index = e.Cell.Row.Index;
      string compCode = string.Empty;
      DBParameter[] inputParam;
      DataTable dtCompInfo;

      switch (columnName)
      {
        case "ComponentCode":
          if (this.isUpdateData)
          {
            this.isUpdateData = false;
            if (ultraDDCompGlass.SelectedRow != null)
            {
              compCode = ultraDDCompGlass.SelectedRow.Cells["Code"].Value.ToString();
            }
            inputParam = new DBParameter[] { new DBParameter("@ComponentCode", DbType.AnsiString, 16, compCode) };

            dtCompInfo = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListGlassInfo", inputParam);
            if (dtCompInfo != null && dtCompInfo.Rows.Count > 0)
            {
              ultraGridGlassInfo.Rows[index].Cells["ComponentName"].Value = dtCompInfo.Rows[0]["ComponentName"];
              ultraGridGlassInfo.Rows[index].Cells["MaterialCode"].Value = dtCompInfo.Rows[0]["MaterialCode"];
              ultraGridGlassInfo.Rows[index].Cells["Length"].Value = dtCompInfo.Rows[0]["Length"];
              ultraGridGlassInfo.Rows[index].Cells["Width"].Value = dtCompInfo.Rows[0]["Width"];
              ultraGridGlassInfo.Rows[index].Cells["Thickness"].Value = dtCompInfo.Rows[0]["Thickness"];
              ultraGridGlassInfo.Rows[index].Cells["Remark"].Value = dtCompInfo.Rows[0]["Remark"];
              ultraGridGlassInfo.Rows[index].Cells["Type"].Value = dtCompInfo.Rows[0]["Type"];
              ultraGridGlassInfo.Rows[index].Cells["Bevel"].Value = dtCompInfo.Rows[0]["Bevel"];

              //Adjective
              string commandTextAdj = "Select Code, Description From TblBOMCodeMaster Where [Group] = 11";
              DataTable dtAdj = DataBaseAccess.SearchCommandTextDataTable(commandTextAdj);

              DataColumn column = new DataColumn();
              column.DataType = System.Type.GetType("System.String");
              column.ColumnName = "GlassType";
              dtCompInfo.Columns.Add(column);

              foreach (DataRow dr in dtCompInfo.Rows)
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
                    dr["GlassType"] += objCM.Description + ", ";
                  }
                }
                string strTemp = dr["GlassType"].ToString().Trim();
                if (strTemp.Length > 0)
                {
                  strTemp = dr["Bevel"].ToString().Trim() + ", " + strTemp.TrimEnd(',');
                  strTemp = strTemp.TrimStart(',');
                  strTemp = strTemp.Trim();
                }
                ultraGridGlassInfo.Rows[index].Cells["GlassType"].Value = strTemp;
              }
            }
            else
            {
              ultraGridGlassInfo.Rows[index].Cells["ComponentName"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["MaterialCode"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Length"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Width"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Thickness"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Remark"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Type"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Bevel"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["GlassType"].Value = DBNull.Value;
            }
            this.CheckComponentDuplicate(ultraGridGlassInfo);
            this.isUpdateData = true;
          }
          break;
        case "ComponentName":
          if (this.isUpdateData)
          {
            this.isUpdateData = false;
            compCode = string.Empty;
            if (ultraDDCompGlassName.SelectedRow != null)
            {
              compCode = ultraDDCompGlassName.SelectedRow.Cells["Code"].Value.ToString();
            }
            inputParam = new DBParameter[] { new DBParameter("@ComponentCode", DbType.AnsiString, 16, compCode) };

            dtCompInfo = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListGlassInfo", inputParam);
            if (dtCompInfo != null && dtCompInfo.Rows.Count > 0)
            {
              ultraGridGlassInfo.Rows[index].Cells["ComponentCode"].Value = dtCompInfo.Rows[0]["ComponentCode"];
              ultraGridGlassInfo.Rows[index].Cells["MaterialCode"].Value = dtCompInfo.Rows[0]["MaterialCode"];
              ultraGridGlassInfo.Rows[index].Cells["Length"].Value = dtCompInfo.Rows[0]["Length"];
              ultraGridGlassInfo.Rows[index].Cells["Width"].Value = dtCompInfo.Rows[0]["Width"];
              ultraGridGlassInfo.Rows[index].Cells["Thickness"].Value = dtCompInfo.Rows[0]["Thickness"];
              ultraGridGlassInfo.Rows[index].Cells["Remark"].Value = dtCompInfo.Rows[0]["Remark"];
              ultraGridGlassInfo.Rows[index].Cells["Type"].Value = dtCompInfo.Rows[0]["Type"];
              ultraGridGlassInfo.Rows[index].Cells["Bevel"].Value = dtCompInfo.Rows[0]["Bevel"];

              //Adjective
              string commandTextAdj = "Select Code, Description From TblBOMCodeMaster Where [Group] = 11";
              DataTable dtAdj = DataBaseAccess.SearchCommandTextDataTable(commandTextAdj);

              DataColumn column = new DataColumn();
              column.DataType = System.Type.GetType("System.String");
              column.ColumnName = "GlassType";
              dtCompInfo.Columns.Add(column);

              foreach (DataRow dr in dtCompInfo.Rows)
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
                    dr["GlassType"] += objCM.Description + ", ";
                  }
                }
                string strTemp = dr["GlassType"].ToString().Trim();
                if (strTemp.Length > 0)
                {
                  strTemp = dr["Bevel"].ToString().Trim() + ", " + strTemp.TrimEnd(',');
                  strTemp = strTemp.TrimStart(',');
                  strTemp = strTemp.Trim();
                }
                ultraGridGlassInfo.Rows[index].Cells["GlassType"].Value = strTemp;
              }
            }
            else
            {
              ultraGridGlassInfo.Rows[index].Cells["ComponentCode"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["MaterialCode"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Length"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Width"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Thickness"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Remark"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Type"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["Bevel"].Value = DBNull.Value;
              ultraGridGlassInfo.Rows[index].Cells["GlassType"].Value = DBNull.Value;
            }
            this.CheckComponentDuplicate(ultraGridGlassInfo);
            this.isUpdateData = true;
          }
          break;
        case "Qty":
        case "Waste":
          double qty, waste;
          try
          {
            qty = double.Parse(ultraGridGlassInfo.Rows[index].Cells["Qty"].Value.ToString());
          }
          catch
          {
            qty = 0;
          }
          try
          {
            waste = double.Parse(ultraGridGlassInfo.Rows[index].Cells["Waste"].Value.ToString());
          }
          catch
          {
            waste = 0;
          }
          ultraGridGlassInfo.Rows[index].Cells["TotalQty"].Value = (qty * (1 + waste));
          break;
        default:
          break;
      }
      this.NeedToSave = true;
      //btnSaveGlass.Enabled = true;
    }
    private void ultraGridLabourInfo_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      int index = e.Cell.Row.Index;
      if (columnName == "SectionCode")
      {
        try
        {
          ultraGridLabourInfo.Rows[index].Cells["NameEN"].Value = ultraDDLabour.SelectedRow.Cells["NameEN"].Value.ToString();
        }
        catch
        {
        }
      }
      this.NeedToSave = true;
      //btnSaveLabour.Enabled = true;
    }
    #endregion After Cell Update

    #region Before Cell Update
    private void ultraGridHardwareInfo_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      if (e.Cell.Text.ToString().Trim().Length > 0)
      {
        string strColName = e.Cell.Column.ToString().ToLower();
        switch (strColName)
        {
          case "componentcode":
          case "comp_alter":
            //Kiem tra xem ComponentCode nhap dung chua
            string commandText = string.Format("Select Code From VBOMComponent Where Code = '{0}' And CompGroup = {1}", e.NewValue.ToString(), iIndex);
            DataTable dtTest = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtTest.Rows.Count <= 0)
            {
              if (string.Compare(strColName, "Comp_Alter", true) == 0)
              {
                strColName = "Comp Alternative";
              }
              else
              {
                strColName = "Component Code";
              }
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", strColName);
              e.Cancel = true;
            }
            break;
          case "componentname":
            //Kiem tra xem ComponentName nhap dung chua
            string cmt = string.Format("Select Name From VBOMComponent Where Name = N'{0}' And CompGroup = {1}", e.Cell.Text, iIndex);
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmt);
            if (dt.Rows.Count <= 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Component Name");
              e.Cancel = true;
            }
            break;
          case "qty":
            double qty = DBConvert.ParseDouble(e.Cell.Text);
            if (qty <= 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Qty");
              e.Cancel = true;
            }
            break;
          case "waste":
            double waste = DBConvert.ParseDouble(e.Cell.Text);
            if (waste < 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Waste");
              e.Cancel = true;
            }
            break;
          default:
            break;
        }
      }
    }
    private void ultraGridLabourInfo_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      if (e.Cell.Text.ToString().Trim().Length > 0)
      {
        string strColName = e.Cell.Column.ToString();
        DataTable dtTest;
        if (string.Compare(strColName, "SectionCode", true) == 0)
        {
          //Kiem tra xem Section nhap dung chua
          string commandText = string.Format("Select Code From VBOMSection Where Code = '{0}'", e.Cell.Text.ToString().Trim());
          dtTest = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtTest.Rows.Count <= 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", strColName);
            e.Cancel = true;
          }
        }
      }
    }
    #endregion Before Cell Update

    #region Before Rows Deleted
    private void ultraGridHardwareInfo_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      this.tempDelete = string.Empty;
      foreach (Infragistics.Win.UltraWinGrid.UltraGridRow ultRow in e.Rows)
      {
        if (tempDelete != string.Empty)
        {
          tempDelete += '|';
        }
        try
        {
          tempDelete = tempDelete + ultRow.Cells["PID"].Value.ToString();
        }
        catch
        {
        }
      }
    }
    #endregion Before Rows Deleted

    #region After Rows Deleted
    private void ultraGridHardwareInfo_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (deletePid != string.Empty && deletePid != null)
      {
        deletePid += "|";
      }
      deletePid += tempDelete;
      deletePid = deletePid.TrimEnd('|');
      this.CheckComponentDuplicate(ultraGridHardwareInfo);
      this.CheckComponentDuplicate(ultraGridAccessoryInfo);
      this.CheckComponentDuplicate(ultraGridGlassInfo);
      this.CheckComponentDuplicate(ultraGridUpholsteryInfo);
      this.CheckComponentDuplicate(ultraGridFinishingInfo);
      this.NeedToSave = true;
    }
    #endregion After Rows Deleted

    #region InitializeLayout
    private void ultraGridHardwareInfo_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["WorkAreaPid"].ValueList = ultraDDWorkArea;
      e.Layout.Bands[0].Columns["WorkAreaPid"].Header.Caption = "Work Area";
      e.Layout.Bands[0].Columns["ComponentCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ComponentCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Waste"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Waste"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Length"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Length"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Width"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Width"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Thickness"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Thickness"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["TotalQty"].MinWidth = 80;
      e.Layout.Bands[0].Columns["TotalQty"].MaxWidth = 80;
    }

    //private void ultraGridGlassInfo_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    //{
    //  e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
    //}

    private void ultraGridPackageInfo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands["dtBoxType"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["dtBoxType"].Columns["Child"].Hidden = true;
      e.Layout.Bands["dtBoxType"].Columns["BoxTypePid"].Hidden = true;
      e.Layout.Bands["dtBoxType"].Columns["CheckWeight"].Hidden = true;
      e.Layout.Bands["dtBoxType"].Columns["BoxCode"].Header.Caption = "Box Code";
      e.Layout.Bands["dtBoxType"].Columns["BoxName"].Header.Caption = "Box Name";
      e.Layout.Bands["dtBoxType"].Columns["Length"].Header.Caption = "Depth";
      e.Layout.Bands["dtBoxType"].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["GWeight"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType"].Columns["NWeight"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Pid"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Child"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["BoxCode"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["BoxNo"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["BoxTypePid"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["IDFactoryUnit"].Hidden = true;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["FactoryUnit"].Header.Caption = "Factory Unit";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["TotalQty"].Header.Caption = "Total Qty";
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["RAW_Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["RAW_Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["RAW_Thickness"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands["dtBoxType_dtBoxTypeDetail"].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }
    #endregion InitializeLayout

    #region UltraCBRevision Value Change
    private void ultraCBRevisionHardware_ValueChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          revision = DBConvert.ParseInt(ultraCBRevisionHardware.Value);
          pictureItemHardware.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
          this.LoadGridHardwareInfo();
        }
        catch
        {
          revision = int.MinValue;
          txtCarcassHarware.Text = string.Empty;
          txtDescriptionHarware.Text = string.Empty;
          ultraGridHardwareInfo.DataSource = null;
        }
      }
    }

    private void cmbRevisionAccessory_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          revision = int.Parse(cmbRevisionAccessory.SelectedValue.ToString());
          pictureItemAccessory.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
          this.LoadGridAccessoryInfo();
        }
        catch
        {
          revision = int.MinValue;
          txtCarcassAccessory.Text = string.Empty;
          txtDescriptionAccessory.Text = string.Empty;
          ultraGridAccessoryInfo.DataSource = null;
        }
      }
    }
    private void cmbRevisionUphol_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          revision = int.Parse(cmbRevisionUphol.SelectedValue.ToString());
          pictureItemUpholstery.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
          this.LoadGridUpholsteryInfo();
        }
        catch
        {
          revision = int.MinValue;
          txtCarcassUphol.Text = string.Empty;
          txtDescriptionUphol.Text = string.Empty;
          ultraGridUpholsteryInfo.DataSource = null;
        }
      }
    }
    private void cmbRevisionFinish_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          revision = int.Parse(cmbRevisionFinish.SelectedValue.ToString());
          pictureItemFinishing.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
          this.LoadGridFinishingInfo();
        }
        catch
        {
          revision = int.MinValue;
          txtCarcassFinish.Text = string.Empty;          
          ultraGridFinishingInfo.DataSource = null;
        }
      }
    }
    private void cmbRevisionSupport_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          revision = int.Parse(cmbRevisionSupport.SelectedValue.ToString());
          pictureItemSupport.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
          this.LoadGridSupportInfo();
        }
        catch
        {
          revision = int.MinValue;
          txtCarcassSupport.Text = string.Empty;
          txtDescriptionSupport.Text = string.Empty;
          ultraGridSupportInfo.DataSource = null;
        }
      }
    }
    private void cmbRevisionGlass_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          revision = int.Parse(cmbRevisionGlass.SelectedValue.ToString());
          pictureItemGlass.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
          this.LoadGridGlassInfo();
        }
        catch
        {
          revision = int.MinValue;
          txtCarcassGlass.Text = string.Empty;
          txtDescriptionGlass.Text = string.Empty;
          ultraGridGlassInfo.DataSource = null;
        }
      }
    }
    private void cmbRevisionCarcass_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          revision = int.Parse(cmbRevisionCarcass.SelectedValue.ToString());
          pictureItemCarcass.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
          this.LoadTreeViewCarcassInfo();
        }
        catch
        {
          revision = int.MinValue;
          multiCBCarcass.SelectedIndex = -1;
          txtDescriptionCarcass.Text = string.Empty;
          treeViewComponentStruct.Nodes.Clear();
        }
      }
    }
    private void cmbRevisionPackage_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          revision = int.Parse(cmbRevisionPackage.SelectedValue.ToString());
          pictureItemPackage.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);

          this.LoadGridPackageInfo();
        }
        catch
        {
          revision = int.MinValue;
          multiCBPackage.SelectedIndex = -1;
          txtDescriptionPackage.Text = string.Empty;
          ultraGridPackageInfo.DataSource = null;
        }
      }
    }
    private void cmbRevisionLabour_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          revision = int.Parse(cmbRevisionLabour.SelectedValue.ToString());
          pictureItemDirectLabour.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
          this.LoadGridLabourInfo();
        }
        catch
        {
          revision = int.MinValue;
          txtCarcassLabour.Text = string.Empty;
          txtDescriptionLabour.Text = string.Empty;
          ultraGridLabourInfo.DataSource = null;
        }
      }
    }
    #endregion UltraCBRevision Value Change

    #region multiCBItem_SelectedIndexChanged
    private void multiCBItemAccessory_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          itemCode = multiCBItemAccessory.SelectedValue.ToString();
        }
        catch
        {
          itemCode = string.Empty;
        }
        Utility.LoadRevisionByItemCode(cmbRevisionAccessory, itemCode);
      }
    }

    private void ultraCBItemHardware_ValueChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          itemCode = ultraCBItemHardware.Value.ToString();
        }
        catch
        {
          itemCode = string.Empty;
        }
        Utility.LoadRevisionByItemCode(ultraCBRevisionHardware, itemCode);
      }
    }

    private void multiCBItemUphol_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          itemCode = multiCBItemUphol.SelectedValue.ToString();
        }
        catch
        {
          itemCode = string.Empty;
        }
        Utility.LoadRevisionByItemCode(cmbRevisionUphol, itemCode);
      }
    }
    private void multiCBItemFinish_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          itemCode = multiCBItemFinish.SelectedValue.ToString();
        }
        catch
        {
          itemCode = string.Empty;
        }
        Utility.LoadRevisionByItemCode(cmbRevisionFinish, itemCode);
      }
    }
    private void multiCBItemSupport_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          itemCode = multiCBItemSupport.SelectedValue.ToString();
        }
        catch
        {
          itemCode = string.Empty;
        }
        Utility.LoadRevisionByItemCode(cmbRevisionSupport, itemCode);
      }
    }
    private void multiCBItemGlass_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          itemCode = multiCBItemGlass.SelectedValue.ToString();
        }
        catch
        {
          itemCode = string.Empty;
        }
        Utility.LoadRevisionByItemCode(cmbRevisionGlass, itemCode);
      }
    }

    private void ultraCBItemCarcass_ValueChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        itemCode = ultraCBItemCarcass.Value.ToString();
        Utility.LoadRevisionByItemCode(cmbRevisionCarcass, itemCode);
      }
    }

    private void multiCBItemPackage_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          itemCode = multiCBItemPackage.SelectedValue.ToString();
        }
        catch
        {
          itemCode = string.Empty;
        }
        Utility.LoadRevisionByItemCode(cmbRevisionPackage, itemCode);
      }
    }
    private void multiCBItemLabour_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          itemCode = multiCBItemLabour.SelectedValue.ToString();
        }
        catch
        {
          itemCode = string.Empty;
        }
        Utility.LoadRevisionByItemCode(cmbRevisionLabour, itemCode);
      }
    }
    #endregion multiCBItem_SelectedIndexChanged

    #region Check And SaveData
    private bool CheckInvalid()
    {
      UltraGrid ultraGridCurrentTab = new UltraGrid();
      //Tien add
      DataSet dtGridComponentToCompare = null;
      //end
      switch (this.iIndex)
      {
        case 9:
          break;
        case 1:
          ultraGridCurrentTab = ultraGridHardwareInfo;
          break;
        case 2:
          ultraGridCurrentTab = ultraGridGlassInfo;
          break;
        case 3:
          break;
        case 4:
          ultraGridCurrentTab = ultraGridAccessoryInfo;
          break;
        case 5:
          ultraGridCurrentTab = ultraGridUpholsteryInfo;
          break;
        case 6:
          ultraGridCurrentTab = ultraGridFinishingInfo;
          break;
        case 7:
          break;
        case 8:
          break;
        default:
          break;
      }
      // Check invalid Component Code
      string code = string.Empty;

      for (int i = 0; i < ultraGridCurrentTab.Rows.Count; i++)
      {
        code = ultraGridCurrentTab.Rows[i].Cells["ComponentCode"].Value.ToString();
        if (code.Length == 0)
        {
          WindowUtinity.ShowMessageError("MSG0005", string.Format("Component Code at row {0}", i + 1));
          return false;
        }
        //Tien update check trùng component hardware
        //Update date: 15/12/2014
        if (this.isDuplicateCompo == true)
        {
          WindowUtinity.ShowMessageError("ERR0013", "Component Code");
          return false;
        }
        //End     
      }
      return true;
    }
    private void SaveHardware()
    {
      if (this.CheckInvalid())
      {
        // 1. Delete
        if (deletePid != null && deletePid != string.Empty)
        {
          string[] arrDelete = deletePid.Split('|');
          deletePid = string.Empty;
          for (int i = 0; i < arrDelete.Length; i++)
          {
            DBParameter[] inputParamDelete = new DBParameter[1];
            inputParamDelete[0] = new DBParameter("@Pid", DbType.Int32, arrDelete[i]);
            DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spBOMItemComponent_Delete", inputParamDelete, outputParamDelete);
            if (outputParamDelete[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
              this.LoadGridHardwareInfo();
              return;
            }
          }
        }
        // 2. Insert, update
        DataTable dtSource = (DataTable)ultraGridHardwareInfo.DataSource;
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          if ((dtSource.Rows[i].RowState == DataRowState.Modified) || (dtSource.Rows[i].RowState == DataRowState.Added))
          {
            string storeName = string.Empty;
            string compCode = string.Empty;
            string alternative = string.Empty;
            try
            {
              compCode = dtSource.Rows[i]["ComponentCode"].ToString().Split('|')[0];
            }
            catch
            {
            }
            try
            {
              alternative = dtSource.Rows[i]["Comp_Alter"].ToString().Split('|')[0];
            }
            catch
            {
            }
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DBParameter[] inputParam = new DBParameter[13];
            inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
            inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
            inputParam[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 16, compCode);
            inputParam[3] = new DBParameter("@CompRevision", DbType.Int32, dtSource.Rows[i]["Revision"]);
            inputParam[4] = new DBParameter("@Qty", DbType.Double, dtSource.Rows[i]["Qty"]);
            inputParam[5] = new DBParameter("@Waste", DbType.Double, dtSource.Rows[i]["Waste"]);
            if (alternative.Length > 0)
            {
              inputParam[6] = new DBParameter("@Alternative", DbType.AnsiString, 16, alternative);
              inputParam[7] = new DBParameter("@AlterRevision", DbType.Int32, dtSource.Rows[i]["AlterRevision"]);
            }

            if (dtSource.Rows[i].RowState == DataRowState.Added) //Insert
            {
              inputParam[8] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              storeName = "spBOMItemComponent_Insert";
            }
            else //Update
            {
              inputParam[8] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              inputParam[9] = new DBParameter("@PID", DbType.Int32, dtSource.Rows[i]["PID"]);
              storeName = "spBOMItemComponent_Update";
            }
            inputParam[10] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
            long workAreaPid = DBConvert.ParseLong(dtSource.Rows[i]["WorkAreaPid"]);
            if (workAreaPid > 0)
            {
              inputParam[11] = new DBParameter("@WorkAreaPid", DbType.Int64, workAreaPid);
            }
            string remark = dtSource.Rows[i]["Remark"].ToString().Trim();
            if (remark.Length > 0)
            {
              inputParam[12] = new DBParameter("@Remark", DbType.String, 256, remark);
            }
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if (outputParam[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
              this.LoadGridHardwareInfo();
              return;
            }
          }
        }
        this.LoadGridHardwareInfo();
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }
    private void SaveAccessory()
    {
      if (this.CheckInvalid())
      {
        // 1. Delete
        if (deletePid != null && deletePid != string.Empty)
        {
          string[] arrDelete = deletePid.Split('|');
          deletePid = string.Empty;
          for (int i = 0; i < arrDelete.Length; i++)
          {
            DBParameter[] inputParamDelete = new DBParameter[1];
            inputParamDelete[0] = new DBParameter("@Pid", DbType.Int32, arrDelete[i]);
            DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spBOMItemComponent_Delete", inputParamDelete, outputParamDelete);
            if (outputParamDelete[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
              this.LoadGridAccessoryInfo();
              return;
            }
          }
        }
        // 2. Insert, update      
        DataTable dtSource = ((DataSet)ultraGridAccessoryInfo.DataSource).Tables["TblBOMComponentInfo"];
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          if ((dtSource.Rows[i].RowState == DataRowState.Modified) || (dtSource.Rows[i].RowState == DataRowState.Added))
          {
            string storeName = string.Empty;
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DBParameter[] inputParam = new DBParameter[11];
            inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
            inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
            inputParam[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, dtSource.Rows[i]["ComponentCode"].ToString().Trim());
            inputParam[3] = new DBParameter("@CompRevision", DbType.Int32, dtSource.Rows[i]["Revision"]);
            inputParam[4] = new DBParameter("@Qty", DbType.Double, dtSource.Rows[i]["Qty"]);
            inputParam[5] = new DBParameter("@Waste", DbType.Double, dtSource.Rows[i]["Waste"]);
            inputParam[6] = new DBParameter("@WorkAreaPid", DbType.Int64, dtSource.Rows[i]["WorkAreaPid"]);

            if (dtSource.Rows[i].RowState == DataRowState.Added) //Insert
            {
              inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              storeName = "spBOMItemComponent_Insert";
            }
            else //Update
            {
              inputParam[7] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              inputParam[8] = new DBParameter("@PID", DbType.Int32, dtSource.Rows[i]["PID"]);
              storeName = "spBOMItemComponent_Update";
            }
            inputParam[9] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
            string remark = dtSource.Rows[i]["Remark"].ToString().Trim();
            if (remark.Length > 0)
            {
              inputParam[10] = new DBParameter("@Remark", DbType.String, 256, remark);
            }

            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if (outputParam[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
              this.LoadGridAccessoryInfo();
              return;
            }
          }
        }
        this.LoadGridAccessoryInfo();
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }
    private void SaveUphol()
    {
      if (this.CheckInvalid())
      {
        // 1. Delete
        if (deletePid != null && deletePid != string.Empty)
        {
          string[] arrDelete = deletePid.Split('|');
          deletePid = string.Empty;
          for (int i = 0; i < arrDelete.Length; i++)
          {
            DBParameter[] inputParamDelete = new DBParameter[1];
            inputParamDelete[0] = new DBParameter("@Pid", DbType.Int32, arrDelete[i]);
            DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spBOMItemComponent_Delete", inputParamDelete, outputParamDelete);
            if (outputParamDelete[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
              this.LoadGridUpholsteryInfo();
              return;
            }
          }
        }
        // 2. Insert, update      
        DataTable dtSource = ((DataSet)ultraGridUpholsteryInfo.DataSource).Tables["TblBOMComponentInfo"];
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          if ((dtSource.Rows[i].RowState == DataRowState.Modified) || (dtSource.Rows[i].RowState == DataRowState.Added))
          {
            string storeName = string.Empty;
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DBParameter[] inputParam = new DBParameter[11];
            inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
            inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
            inputParam[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, dtSource.Rows[i]["ComponentCode"]);
            inputParam[3] = new DBParameter("@CompRevision", DbType.Int32, dtSource.Rows[i]["Revision"]);
            inputParam[4] = new DBParameter("@Qty", DbType.Double, dtSource.Rows[i]["Qty"]);
            inputParam[5] = new DBParameter("@Waste", DbType.Double, dtSource.Rows[i]["Waste"]);
            inputParam[6] = new DBParameter("@WorkAreaPid", DbType.AnsiString, 16, dtSource.Rows[i]["WorkAreaPid"]);

            if (dtSource.Rows[i].RowState == DataRowState.Added) //Insert
            {
              inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              storeName = "spBOMItemComponent_Insert";
            }
            else //Update
            {
              inputParam[7] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              inputParam[8] = new DBParameter("@PID", DbType.Int32, dtSource.Rows[i]["PID"]);
              storeName = "spBOMItemComponent_Update";
            }
            inputParam[9] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
            string remark = dtSource.Rows[i]["Remark"].ToString().Trim();
            if (remark.Length > 0)
            {
              inputParam[10] = new DBParameter("@Remark", DbType.String, 256, remark);
            }

            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if (outputParam[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
              this.LoadGridUpholsteryInfo();
              return;
            }
          }
        }
        this.LoadGridUpholsteryInfo();
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }
    private void SaveFinish()
    {
      if (this.CheckInvalid())
      {
        // 1. Delete
        if (deletePid != null && deletePid != string.Empty)
        {
          string[] arrDelete = deletePid.Split('|');
          deletePid = string.Empty;
          for (int i = 0; i < arrDelete.Length; i++)
          {
            DBParameter[] inputParamDelete = new DBParameter[1];
            inputParamDelete[0] = new DBParameter("@Pid", DbType.Int32, arrDelete[i]);
            DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spBOMItemComponent_Delete", inputParamDelete, outputParamDelete);
            if (outputParamDelete[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
              this.LoadGridFinishingInfo();
              return;
            }
          }
        }
        // 2. Insert, update      
        DataTable dtSource = ((DataSet)ultraGridFinishingInfo.DataSource).Tables[0];
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          if ((dtSource.Rows[i].RowState == DataRowState.Modified) || (dtSource.Rows[i].RowState == DataRowState.Added))
          {
            string storeName = string.Empty;
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DBParameter[] inputParam = new DBParameter[10];
            inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
            inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
            inputParam[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 16, dtSource.Rows[i]["ComponentCode"]);            
            inputParam[4] = new DBParameter("@Qty", DbType.Double, dtSource.Rows[i]["Qty"]);
            inputParam[5] = new DBParameter("@Waste", DbType.Double, dtSource.Rows[i]["Waste"]);

            if (dtSource.Rows[i].RowState == DataRowState.Added) //Insert
            {
              inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              storeName = "spBOMItemComponent_Insert";
            }
            else //Update
            {
              inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              inputParam[7] = new DBParameter("@PID", DbType.Int32, dtSource.Rows[i]["PID"]);
              storeName = "spBOMItemComponent_Update";
            }
            inputParam[8] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
            long finishProcessMasterPid = DBConvert.ParseLong(dtSource.Rows[i]["FinishProcessMasterPid"]);
            if (finishProcessMasterPid > 0)
            {
              inputParam[9] = new DBParameter("@FinishProcessMasterPid", DbType.Int64, finishProcessMasterPid);
            }

            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if (outputParam[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
              this.LoadGridFinishingInfo();
              return;
            }
          }
        }
        this.LoadGridFinishingInfo();
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }
    private void SaveSupport()
    {
      string supCode = multiCBSupport.SelectedValue.ToString();
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      inputParam[2] = new DBParameter("@SupCode", DbType.AnsiString, 16, supCode);
      DataBaseAccess.ExecuteStoreProcedure("spBOMItemInfo_UpdateSupCode", inputParam, outputParam);
      if (outputParam[0].Value.ToString() == "0")
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }
    private void SaveGlass()
    {
      if (this.CheckInvalid())
      {
        // 1. Delete
        if (deletePid != null && deletePid != string.Empty)
        {
          string[] arrDelete = deletePid.Split('|');
          deletePid = string.Empty;
          for (int i = 0; i < arrDelete.Length; i++)
          {
            DBParameter[] inputParamDelete = new DBParameter[1];
            inputParamDelete[0] = new DBParameter("@Pid", DbType.Int32, arrDelete[i]);
            DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spBOMItemComponent_Delete", inputParamDelete, outputParamDelete);
            if (outputParamDelete[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
              this.LoadGridGlassInfo();
              return;
            }
          }
        }
        // 2. Insert, update      
        DataTable dtSource = (DataTable)ultraGridGlassInfo.DataSource;
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          if ((dtSource.Rows[i].RowState == DataRowState.Modified) || (dtSource.Rows[i].RowState == DataRowState.Added))
          {
            string storeName = string.Empty;
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DBParameter[] inputParam = new DBParameter[9];
            inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
            inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
            inputParam[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 16, dtSource.Rows[i]["ComponentCode"]);
            inputParam[3] = new DBParameter("@CompGroup", DbType.Int32, iIndex);
            inputParam[4] = new DBParameter("@Qty", DbType.Double, dtSource.Rows[i]["Qty"]);
            inputParam[5] = new DBParameter("@Waste", DbType.Double, dtSource.Rows[i]["Waste"]);

            if (dtSource.Rows[i].RowState == DataRowState.Added) //Insert
            {
              inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              storeName = "spBOMItemComponent_Insert";
            }
            else //Update
            {
              inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              inputParam[7] = new DBParameter("@PID", DbType.Int64, dtSource.Rows[i]["PID"]);
              storeName = "spBOMItemComponent_Update";
            }
            long workAreaPid = DBConvert.ParseLong(dtSource.Rows[i]["WorkAreaPid"]);
            if (workAreaPid > 0)
            {
              inputParam[8] = new DBParameter("@WorkAreaPid", DbType.Int64, workAreaPid);
            }
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if (outputParam[0].Value.ToString() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
              this.LoadGridGlassInfo();
              return;
            }
          }
        }
        this.LoadGridGlassInfo();
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }
    private void SaveCarcass()
    {
      string carcassCode = string.Empty;
      try
      {
        carcassCode = multiCBCarcass.SelectedValue.ToString();
      }
      catch
      {
      }
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
      DataBaseAccess.ExecuteStoreProcedure("spBOMItemInfo_UpdateCarcassCode", inputParam, outputParam);
      if (outputParam[0].Value.ToString() == "0")
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }
    private void SaveLabour()
    {
      DataTable dtSource = (DataTable)ultraGridLabourInfo.DataSource;
      // Check Invalid Labour
      for (int i = 0; i < ultraGridLabourInfo.Rows.Count; i++)
      {
        string section = ultraGridLabourInfo.Rows[i].Cells["SectionCode"].Value.ToString();
        DataRow[] count = dtSource.Select(string.Format("SectionCode = '{0}'", section));
        if (count.Length > 1)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0006", "Section Code");
          return;
        }
        if (section.Length == 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", string.Format("Section Code at row {0}", i + 1));
          return;
        }
        double qty = DBConvert.ParseDouble(ultraGridLabourInfo.Rows[i].Cells["Qty"].Value.ToString());
        if (qty <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", string.Format("Qty at row {0}", i + 1));
          return;
        }
      }

      // 1. Delete
      if (deletePid != null && deletePid != string.Empty)
      {
        string[] arrDelete = deletePid.Split('|');
        deletePid = string.Empty;
        for (int i = 0; i < arrDelete.Length; i++)
        {
          DBParameter[] inputParamDelete = new DBParameter[1];
          inputParamDelete[0] = new DBParameter("@Pid", DbType.Int32, arrDelete[i]);
          DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMDirectLabour_Delete", inputParamDelete, outputParamDelete);
          if (outputParamDelete[0].Value.ToString() == "0")
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
            this.LoadGridLabourInfo();
            return;
          }
        }
      }
      // 2. Insert, update
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        if ((dtSource.Rows[i].RowState == DataRowState.Modified) || (dtSource.Rows[i].RowState == DataRowState.Added))
        {
          string storeName = string.Empty;
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DBParameter[] inputParam = new DBParameter[8];
          inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
          inputParam[2] = new DBParameter("@SectionCode", DbType.AnsiString, 50, dtSource.Rows[i]["SectionCode"]);
          inputParam[3] = new DBParameter("@Qty", DbType.Double, dtSource.Rows[i]["Qty"]);
          inputParam[4] = new DBParameter("@Description", DbType.String, 512, dtSource.Rows[i]["Description"]);
          inputParam[5] = new DBParameter("@Remark", DbType.String, 512, dtSource.Rows[i]["Remark"]);

          if (dtSource.Rows[i].RowState == DataRowState.Added) //Insert
          {
            inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            storeName = "spBOMDirectLabour_Insert";
          }
          else //Update
          {
            inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            inputParam[7] = new DBParameter("@PID", DbType.Int32, dtSource.Rows[i]["PID"]);
            storeName = "spBOMDirectLabour_Update";
          }
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          if (outputParam[0].Value.ToString() == "0")
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            this.LoadGridLabourInfo();
            return;
          }
        }
      }
      this.LoadGridLabourInfo();
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
    }
    private void SavePackage()
    {
      string packageCode = string.Empty;
      try
      {
        packageCode = multiCBPackage.SelectedValue.ToString();
      }
      catch
      {
      }
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      if (packageCode.Length > 0)
      {
        inputParam[2] = new DBParameter("@PackageCode", DbType.AnsiString, 50, packageCode);
      }
      DataBaseAccess.ExecuteStoreProcedure("spBOMItemInfo_UpdatePackageCode", inputParam, outputParam);
      if (outputParam[0].Value.ToString() == "0")
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }
    #endregion Check And SaveData

    #region btnSave_Click
    private void btnSaveHardware_Click(object sender, EventArgs e)
    {
      this.SaveHardware();
    }
    private void btnSaveAccessory_Click(object sender, EventArgs e)
    {
      this.SaveAccessory();
    }
    private void btnSaveUphol_Click(object sender, EventArgs e)
    {
      this.SaveUphol();
    }
    private void btnSaveFinish_Click(object sender, EventArgs e)
    {
      this.SaveFinish();
    }
    private void btnSaveSupport_Click(object sender, EventArgs e)
    {
      this.SaveSupport();
      this.LoadGridSupportInfo();
    }
    private void btnSaveGlass_Click(object sender, EventArgs e)
    {
      this.SaveGlass();
    }
    private void btnSaveCarcass_Click(object sender, EventArgs e)
    {
      this.SaveCarcass();
      this.LoadTreeViewCarcassInfo();
    }
    private void btnSaveLabour_Click(object sender, EventArgs e)
    {
      this.SaveLabour();
    }
    private void btnSavePackage_Click(object sender, EventArgs e)
    {
      this.SavePackage();
      this.LoadGridPackageInfo();
    }
    #endregion btnSave_Click

    #region btnNew_Click
    private void btnNewHardware_Click(object sender, EventArgs e)
    {
      viewBOM_03_014 view = new viewBOM_03_014();
      view.componentGroup = iIndex;
      WindowUtinity.ShowView(view, "COMPONENT INFORMATION", false, Shared.Utility.ViewState.ModalWindow);
      this.InitDataHardware();
    }

    private void btnNewGlass_Click(object sender, EventArgs e)
    {
      viewBOM_03_016 view = new viewBOM_03_016();
      Shared.Utility.WindowUtinity.ShowView(view, "Glass Information", false, Shared.Utility.ViewState.ModalWindow);
      this.InitDataGlass();
    }
    private void btnNewSupport_Click(object sender, EventArgs e)
    {
      viewBOM_03_006 view = new viewBOM_03_006();
      view.supCode = string.Empty;
      Shared.Utility.WindowUtinity.ShowView(view, "Support Infomation", false, Shared.Utility.ViewState.ModalWindow);
      this.InitDataSupport();
    }
    private void btnNewFinish_Click(object sender, EventArgs e)
    {
      viewBOM_03_004 view = new viewBOM_03_004();
      view.finishingCode = string.Empty;
      Shared.Utility.WindowUtinity.ShowView(view, "Finishing Infomation", false, Shared.Utility.ViewState.ModalWindow);
      this.InitDataFinish();
    }
    private void btnNewPackage_Click(object sender, EventArgs e)
    {
      viewBOM_03_021 view = new viewBOM_03_021();
      Shared.Utility.WindowUtinity.ShowView(view, "Package Infomation", false, Shared.Utility.ViewState.ModalWindow);
      this.InitDataPackage();
    }
    #endregion btnNew_Click

    #region More Function Support Tab
    private void multiCBSupport_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        string supCode = string.Empty;
        if (multiCBSupport.SelectedIndex > 0)
        {
          supCode = multiCBSupport.SelectedValue.ToString();
        }
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SupCode", DbType.AnsiString, 16, supCode) };
        DataTable dtSup = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListSupportDetailBySupCode", inputParam);
        ultraGridSupportInfo.DataSource = dtSup;
        lbCount.Text = string.Format("Count: {0}", dtSup.Rows.Count);
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
        ultraGridSupportInfo.DisplayLayout.Bands[0].Columns["UnitCode"].Hidden = true;
        this.NeedToSave = true;
        //btnSaveSupport.Enabled = true;
      }
    }
    #endregion More Function Support Tab

    #region More Function Carcass Tab
    private void multiCBCarcass_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        try
        {
          string carcass = multiCBCarcass.SelectedValue.ToString();
          Utility.LoadDataCarcassComponentStruct(treeViewComponentStruct, carcass);
        }
        catch
        {
        }
        //btnSaveCarcass.Enabled = true;
        this.NeedToSave = true;
      }
      lbCount.Text = string.Empty;
    }
    #endregion More Function Carcass Tab

    #region More Function Package Tab
    private void multiCBPackage_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (loaded)
      {
        string packageCode = string.Empty;
        try
        {
          packageCode = multiCBPackage.SelectedValue.ToString();
        }
        catch
        {
        }
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PackageCode", DbType.AnsiString, 16, packageCode) };
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMPackageInfomationByPackageCode", inputParam);

        //Load Package mater info
        if (dsSource.Tables[0].Rows.Count > 0)
        {
          DataTable dtMaterInfo = dsSource.Tables[0];
          txtPackageItemQty.Text = dtMaterInfo.Rows[0]["QuantityItem"].ToString();
          txtPackageBoxQty.Text = dtMaterInfo.Rows[0]["QuantityBox"].ToString();
          txtPackageTotalCBM.Text = dtMaterInfo.Rows[0]["TotalCBM"].ToString();
        }
        else
        {
          txtPackageItemQty.Text = string.Empty;
          txtPackageBoxQty.Text = string.Empty;
          txtPackageTotalCBM.Text = string.Empty;
        }
        //Load grid info
        DataSet dsData = CreateDataSet.BoxTypeInfo();
        //dsData.Tables.Add(dsSource.Tables[1].Clone());
        //dsData.Tables.Add(dsSource.Tables[2].Clone());
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
        ultraGridPackageInfo.DataSource = dsData;
        this.NeedToSave = true;
        //btnSavePackage.Enabled = true;
      }
    }

    #endregion More Function Package Tab

    private DialogResult SaveBeforeClosing(int tabIndex)
    {
      DialogResult result = DialogResult.No;
      switch (tabIndex)
      {
        case 0:
          if (btnSaveCarcass.Visible && btnSaveCarcass.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SaveCarcass();
            }
          }
          break;
        case 9:
          if (btnSaveCarcass.Visible && btnSaveCarcass.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SaveCarcass();
            }
          }
          break;
        case 1:
          if (btnSaveHardware.Visible && btnSaveHardware.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SaveHardware();
            }
          }
          break;
        case 2:
          if (btnSaveGlass.Visible && btnSaveGlass.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SaveGlass();
            }
          }
          break;
        case 3:
          if (btnSaveSupport.Visible && btnSaveSupport.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SaveSupport();
            }
          }
          break;
        case 4:
          if (btnSaveAccessory.Visible && btnSaveAccessory.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SaveAccessory();
            }
          }
          break;
        case 5:
          if (btnSaveUphol.Visible && btnSaveUphol.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SaveUphol();
            }
          }
          break;
        case 6:
          if (btnSaveFinish.Visible && btnSaveFinish.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SaveFinish();
            }
          }
          break;
        case 7:
          if (btnSavePackage.Visible && btnSavePackage.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SavePackage();
            }
          }
          break;
        case 8:
          if (btnSaveLabour.Visible && btnSaveLabour.Enabled)
          {
            result = FunctionUtility.SaveBeforeClosing();
            if (result == DialogResult.Yes)
            {
              this.SaveLabour();
            }
          }
          break;
        default:
          break;
      }
      return result;
    }

    private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
    {
      if (this.NeedToSave && (this.SaveBeforeClosing(e.TabPageIndex) == DialogResult.Cancel))
      {
        e.Cancel = true;
      }
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      switch (iIndex)
      {
        case 9:
          if (btnSaveCarcass.Visible && btnSaveCarcass.Enabled)
          {
            this.SaveCarcass();
            this.LoadTreeViewCarcassInfo();
          }
          break;
        case 1:
          if (btnSaveHardware.Visible && btnSaveHardware.Enabled)
          {
            this.SaveHardware();
            this.LoadGridHardwareInfo();
          }
          break;
        case 2:
          if (btnSaveGlass.Visible && btnSaveGlass.Enabled)
          {
            this.SaveGlass();
            this.LoadGridGlassInfo();
          }
          break;
        case 3:
          if (btnSaveSupport.Visible && btnSaveSupport.Enabled)
          {
            this.SaveSupport();
            this.LoadGridSupportInfo();
          }
          break;
        case 4:
          if (btnSaveAccessory.Visible && btnSaveAccessory.Enabled)
          {
            this.SaveAccessory();
            this.LoadGridAccessoryInfo();
          }
          break;
        case 5:
          if (btnSaveUphol.Visible && btnSaveUphol.Enabled)
          {
            this.SaveUphol();
            this.LoadGridUpholsteryInfo();
          }
          break;
        case 6:
          if (btnSaveFinish.Visible && btnSaveFinish.Enabled)
          {
            this.SaveFinish();
            this.LoadGridFinishingInfo();
          }
          break;
        case 7:
          break;
        case 8:
          if (btnSaveLabour.Visible && btnSaveLabour.Enabled)
          {
            this.SaveLabour();
            this.LoadGridLabourInfo();
          }
          break;
        default:
          break;
      }
    }

    #region Print Report
    private void btnPrintHardware_Click(object sender, EventArgs e)
    {
      viewBOM_ItemReport view = new viewBOM_ItemReport();
      view.iIndex = iIndex;
      view.itemCode = itemCode;
      view.revision = revision;
      view.ncategory = 2;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void btnPrintUpholstery_Click(object sender, EventArgs e)
    {
      viewBOM_ItemReport view = new viewBOM_ItemReport();
      view.iIndex = iIndex;
      view.itemCode = itemCode;
      view.revision = revision;
      view.ncategory = 6;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void btnPrintAccessory_Click(object sender, EventArgs e)
    {
      viewBOM_ItemReport view = new viewBOM_ItemReport();
      view.iIndex = iIndex;
      view.itemCode = itemCode;
      view.revision = revision;
      view.ncategory = 5;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void btnPrintFinishing_Click(object sender, EventArgs e)
    {
      viewBOM_ItemReport view = new viewBOM_ItemReport();
      view.iIndex = iIndex;
      view.itemCode = itemCode;
      view.revision = revision;
      view.ncategory = 7;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void btnPrintSupport_Click(object sender, EventArgs e)
    {
      viewBOM_ItemReport view = new viewBOM_ItemReport();
      view.iIndex = iIndex;
      string commandText = string.Format("SELECT SupCode FROM TblBOMItemInfo WHERE ItemCode = '{0}' AND Revision = {1}", itemCode, revision);
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      if (obj != null)
      {
        view.code = obj.ToString();
      }
      view.itemCode = itemCode;
      view.revision = revision;
      view.ncategory = 13;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void btnPrintGlass_Click(object sender, EventArgs e)
    {
      viewBOM_ItemReport view = new viewBOM_ItemReport();
      view.iIndex = iIndex;
      view.itemCode = itemCode;
      view.revision = revision;
      view.ncategory = 3;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void btnDirectLabour_Click(object sender, EventArgs e)
    {
      viewBOM_ItemReport view = new viewBOM_ItemReport();
      view.iIndex = iIndex;
      view.itemCode = itemCode;
      view.revision = revision;
      view.ncategory = 9;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void btnPrintPacking_Click(object sender, EventArgs e)
    {
      viewBOM_ItemReport view = new viewBOM_ItemReport();
      string packageCode = Utility.GetSelectedValueMultiCombobox(multiCBPackage);
      view.packageCode = packageCode;
      view.ncategory = 18;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow);
    }
    #endregion Print Report

    #region Mouse Click Grid
    private void ultraGridHardwareInfo_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      string code = string.Empty;
      switch (iIndex)
      {
        case 1:
          try
          {
            code = ultraGridHardwareInfo.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
            string[] path = code.Split('|');
            pictureHardware.ImageLocation = FunctionUtility.BOMGetItemComponentImage(path[0]);
            groupHardwareImg.Text = path[0];
          }
          catch
          {
            pictureHardware.ImageLocation = string.Empty;
            groupHardwareImg.Text = "Hardware Image";
          }
          break;
        case 2:
          try
          {
            code = ultraGridGlassInfo.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
            pictureGlass.ImageLocation = FunctionUtility.BOMGetItemComponentImage(code);
            groupGlassImg.Text = code;
          }
          catch
          {
            pictureGlass.ImageLocation = string.Empty;
            groupGlassImg.Text = "Glass Image";
          }
          break;
        case 4:
          try
          {
            code = ultraGridAccessoryInfo.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
            pictureAccessory.ImageLocation = FunctionUtility.BOMGetItemComponentImage(code);
            groupAccessoryImg.Text = code;
          }
          catch
          {
            pictureAccessory.ImageLocation = string.Empty;
            groupAccessoryImg.Text = "Accessory Image";
          }
          break;
        case 5:
          try
          {
            code = ultraGridUpholsteryInfo.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
            pictureUpholstery.ImageLocation = FunctionUtility.BOMGetItemComponentImage(code);
            groupUpholsteryImg.Text = code;
          }
          catch
          {
            pictureUpholstery.ImageLocation = string.Empty;
            groupUpholsteryImg.Text = "Upholstery Image";
          }
          break;
        case 6:
          try
          {
            code = ultraGridFinishingInfo.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
            //string code = ultraGridFinishingInfo.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
            pictureFinishing.ImageLocation = FunctionUtility.BOMGetItemComponentImage(code);
            groupFinishingImg.Text = code;
          }
          catch
          {
            pictureFinishing.ImageLocation = string.Empty;
            groupFinishingImg.Text = "Upholstery Image";
          }
          break;
      }
    }

    #endregion Mouse Click Grid

    private void treeViewComponentStruct_AfterSelect(object sender, TreeViewEventArgs e)
    {
      if (treeViewComponentStruct.SelectedNode != null && chkShowImage.Checked)
      {
        TreeNode node = treeViewComponentStruct.SelectedNode;
        long pidComp = DBConvert.ParseLong(node.Name);

        string commandText = string.Format("Select ComponentCode From TblBOMCarcassComponent Where Pid = {0}", pidComp);
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          string componentCode = dt.Rows[0]["ComponentCode"].ToString();
          string carcassCode = Utility.GetSelectedValueMultiCombobox(multiCBCarcass);
          gbComponent.Text = componentCode;
          picComponent.ImageLocation = FunctionUtility.BOMGetCarcassComponentImage(carcassCode, componentCode);
          Popup ppComponent = new Popup(gbComponent);
          ppComponent.Show(MousePosition.X + 20, MousePosition.Y + 10);
        }
      }
    }

    private void ultraGridGlassInfo_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraGridGlassInfo.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultraGridGlassInfo.Selected.Rows[0];

      string commandText = string.Format("SELECT Pid FROM TblBOMGlassInfo WHERE ComponentCode = '{0}'", row.Cells["ComponentCode"].Value.ToString());
      long glassPid = DBConvert.ParseLong(DataBaseAccess.ExecuteScalarCommandText(commandText).ToString());

      viewBOM_03_016 view = new viewBOM_03_016();
      view.pid = glassPid;
      view.ViewParam = this.ViewParam;
      Shared.Utility.WindowUtinity.ShowView(view, "Glass Information", false, Shared.Utility.ViewState.ModalWindow);
      this.LoadGridGlassInfo();
    }

    private void ultraGridHardwareInfo_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraGridHardwareInfo.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultraGridHardwareInfo.Selected.Rows[0];
      if (rdFitting.Checked)
      {
        if (DBConvert.ParseLong(row.Cells["PID"].Value.ToString()) != long.MinValue && DBConvert.ParseLong(row.Cells["PID"].Value.ToString()) > 0)
        {
          viewBOM_06_002 view = new viewBOM_06_002();
          view.itemCode = itemCode;
          view.revision = revision;
          view.PID = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
          view.compGroup = 1;
          Shared.Utility.WindowUtinity.ShowView(view, "FITTING INSTRUCTION MATERIAL PROCESS", false, ViewState.ModalWindow);
        }
      }
      else
      {
        viewBOM_07_001 view = new viewBOM_07_001();
        view.currItemCode = itemCode;
        view.currRevision = revision;
        view.type = 1;
        view.currComp = row.Cells["Comp"].Value.ToString();
        //view.compGroup = 1;
        Shared.Utility.WindowUtinity.ShowView(view, "Master Data", false, ViewState.ModalWindow);
      }
    }

    private void ultraGridAccessoryInfo_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraGridAccessoryInfo.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultraGridAccessoryInfo.Selected.Rows[0];

      if (DBConvert.ParseLong(row.Cells["PID"].Value.ToString()) != long.MinValue && DBConvert.ParseLong(row.Cells["PID"].Value.ToString()) > 0)
      {
        if (rdFilter.Checked)
        {
          viewBOM_06_002 view = new viewBOM_06_002();
          view.itemCode = itemCode;
          view.revision = revision;
          view.PID = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
          view.compGroup = 4;
          Shared.Utility.WindowUtinity.ShowView(view, "FITTING INSTRUCTION MATERIAL PROCESS", false, ViewState.ModalWindow);
        }
        else if (rdInfo.Checked)
        {
          string compCode = row.Cells["ComponentCode"].Value.ToString();
          string commandText = string.Format("SELECT Pid FROM TblBOMComponentInfo WHERE ComponentCode = '{0}' AND CompGroup = {1}", compCode, this.iIndex);
          object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
          long compPid = obj != null ? DBConvert.ParseLong(obj.ToString()) : long.MinValue;
          viewBOM_03_014 view = new viewBOM_03_014();
          view.componentPid = compPid;
          view.componentGroup = this.iIndex;
          Shared.Utility.WindowUtinity.ShowView(view, "ACCESSORY INFORMATION", false, ViewState.ModalWindow);
        }
      }
    }

    private void ultCBItemRef_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBItemRef.SelectedRow != null)
      {
        btnCopyDirect.Enabled = true;
      }
      else
      {
        btnCopyDirect.Enabled = false;
      }
    }

    private void btnCopyDirect_Click(object sender, EventArgs e)
    {
      if (ultCBItemRef.SelectedRow != null)
      {
        string item = ultCBItemRef.Value.ToString().Trim();
        string itemCode = item.Split('|')[0];
        int revision = DBConvert.ParseInt(item.Split('|')[1].ToString());

        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
        DataTable dtDirectLabour = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabDirectLabourInfo", inputParam);
        DataTable dtSource = (DataTable)ultraGridLabourInfo.DataSource;
        if (dtSource == null)
        {
          if (dtDirectLabour.Columns.Contains("Pid"))
          {
            dtDirectLabour.Columns.Remove("Pid");
            dtDirectLabour.Columns.Add("Pid", typeof(System.Int64));
          }
          dtSource = dtDirectLabour.Copy();
        }
        else
        {
          foreach (DataRow row in dtSource.Rows)
          {
            if (row.RowState != DataRowState.Deleted)
            {
              if (this.deletePid != null && this.deletePid.Length > 0)
              {
                this.deletePid += "|";
              }
              string pidLocal = row["Pid"].ToString().Trim();
              this.deletePid += pidLocal;
            }
          }
          this.deletePid = this.deletePid.TrimEnd('|');
          dtSource.Rows.Clear();
          foreach (DataRow directRow in dtDirectLabour.Rows)
          {
            directRow["Pid"] = DBNull.Value;
            DataRow newRow = dtSource.NewRow();
            newRow.ItemArray = directRow.ItemArray;
            dtSource.Rows.Add(newRow);
          }
        }
        ultraGridLabourInfo.DataSource = dtSource;
        lbCount.Text = string.Format("Count: {0}", dtDirectLabour.Rows.Count);
      }
    }

    private void ultraGridLabourInfo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["SectionCode"].ValueList = ultraDDLabour;
      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["SectionCode"].Header.Caption = "Section Code";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Qty (Man*hour)";
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }

    //Tien Add
    private void ultCBItemRefHW_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBItemRefHW.SelectedRow != null)
      {
        btnRefHW.Enabled = true;
      }
      else
      {
        btnRefHW.Enabled = false;
      }
    }

    private void ultCBItemRefGL_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBItemRefGL.SelectedRow != null)
      {
        btnRefGL.Enabled = true;
      }
      else
      {
        btnRefGL.Enabled = false;
      }
    }

    private void ultCBItemRefACC_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBItemRefACC.SelectedRow != null)
      {
        btnRefACC.Enabled = true;
      }
      else
      {
        btnRefACC.Enabled = false;
      }
    }

    private void ultCBItemRefUph_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBItemRefUph.SelectedRow != null)
      {
        btnRefUph.Enabled = true;
      }
      else
      {
        btnRefUph.Enabled = false;
      }
    }

    //Tien Add
    private void CheckComponentDuplicate(UltraGrid ultG)
    {
      isDuplicateCompo = false;
      for (int k = 0; k < ultG.Rows.Count; k++)
      {
        ultG.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultG.Rows.Count; i++)
      {
        string comcode = ultG.Rows[i].Cells["ComponentCode"].Value.ToString();
        for (int j = i + 1; j < ultG.Rows.Count; j++)
        {
          string comcodeCompare = ultG.Rows[j].Cells["ComponentCode"].Value.ToString();
          if (string.Compare(comcode, comcodeCompare, true) == 0)
          {
            ultG.Rows[i].CellAppearance.BackColor = Color.Yellow;
            ultG.Rows[j].CellAppearance.BackColor = Color.Yellow;
            this.isDuplicateCompo = true;
          }
        }
      }
    }

    private void btnRefHW_Click(object sender, EventArgs e)
    {
      if (ultCBItemRefHW.SelectedRow != null)
      {
        string item = ultCBItemRefHW.Value.ToString().Trim();
        string itemCode = item.Split('|')[0];
        int revision = DBConvert.ParseInt(item.Split('|')[1].ToString());
        DataTable dt = this.GetDataRefLevel2nd(itemCode, revision, iIndex);
        if (dt != null)
        {
          DataTable dtGird = (DataTable)ultraGridHardwareInfo.DataSource;
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            DataRow row = dtGird.NewRow();
            row["ComponentCode"] = dt.Rows[i]["Code"];
            row["Qty"] = dt.Rows[i]["Qty"];
            row["ComponentName"] = dt.Rows[i]["Name"];
            row["Revision"] = dt.Rows[i]["Revision"];
            row["Length"] = dt.Rows[i]["Length"];
            row["Width"] = dt.Rows[i]["Width"];
            row["Thickness"] = dt.Rows[i]["Thickness"];
            row["Waste"] = dt.Rows[i]["Waste"];
            row["TotalQty"] = dt.Rows[i]["TotalQty"];
            row["MaterialCode"] = dt.Rows[i]["MaterialCode"];
            row["ContractOut"] = dt.Rows[i]["ContractOut"];
            row["WorkAreaPid"] = dt.Rows[i]["WorkAreaPid"];
            dtGird.Rows.Add(row);
          }
        }
        //this.CheckComponentDuplicate(ultraGridHardwareInfo);
        this.NeedToSave = true;
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0125", "Item Reference");
      }
    }

    private void btnRefGL_Click(object sender, EventArgs e)
    {
      if (ultCBItemRefGL.SelectedRow != null)
      {
        string item = ultCBItemRefGL.Value.ToString().Trim();
        string itemCode = item.Split('|')[0];
        int revision = DBConvert.ParseInt(item.Split('|')[1].ToString());
        DataTable dt = this.GetDataRefLevel2nd(itemCode, revision, iIndex);
        if (dt != null)
        {
          DataTable dtGrid = (DataTable)ultraGridGlassInfo.DataSource;
          DataColumn column = new DataColumn();
          column.DataType = System.Type.GetType("System.String");
          column.ColumnName = "GlassType";
          dt.Columns.Add(column);
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            DataRow row = dtGrid.NewRow();
            row["ComponentCode"] = dt.Rows[i]["ComponentCode"];
            row["ComponentName"] = dt.Rows[i]["ComponentName"];
            row["MaterialCode"] = dt.Rows[i]["MaterialCode"];
            row["Qty"] = dt.Rows[i]["Qty"];
            row["Waste"] = dt.Rows[i]["Waste"];
            row["TotalQty"] = dt.Rows[i]["TotalQty"];
            row["Length"] = dt.Rows[i]["Length"];
            row["Width"] = dt.Rows[i]["Width"];
            row["Thickness"] = dt.Rows[i]["Thickness"];
            row["WorkAreaPid"] = dt.Rows[i]["WorkAreaPid"];

            string commandTextAdj = "Select Code, Description From TblBOMCodeMaster Where [Group] = 11";
            DataTable dtAdj = DataBaseAccess.SearchCommandTextDataTable(commandTextAdj);
            string[] glassAdj = dt.Rows[i]["GlassAdj"].ToString().Split('|');
            for (int j = 0; j < glassAdj.Length; j++)
            {
              if (glassAdj[j].ToString().Length > 0)
              {
                BOMCodeMaster objCM = new BOMCodeMaster();
                objCM.Code = DBConvert.ParseInt(glassAdj[j].ToString());
                objCM.Group = ConstantClass.GROUP_GLASS;
                objCM = (BOMCodeMaster)DataBaseAccess.LoadObject(objCM, new string[] { "Group", "Code" });
                dt.Rows[i]["GlassType"] += objCM.Description + ", ";
              }
            }
            string strTemp = dt.Rows[i]["GlassType"].ToString().Trim();
            if (strTemp.Length > 0)
            {
              strTemp = dt.Rows[i]["Description"].ToString().Trim() + ", " + strTemp.TrimEnd(',');
              strTemp = strTemp.TrimStart(',');
              strTemp = strTemp.Trim();
            }
            row["GlassType"] = strTemp;
            row["Remark"] = dt.Rows[i]["Remark"];
            row["Type"] = dt.Rows[i]["Type"];

            dtGrid.Rows.Add(row);
          }
          this.CheckComponentDuplicate(ultraGridGlassInfo);
          this.NeedToSave = true;
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0125", "Item Reference");
        }
      }
    }

    private void btnRefACC_Click(object sender, EventArgs e)
    {
      if (ultCBItemRefACC.SelectedRow != null)
      {
        string item = ultCBItemRefACC.Value.ToString().Trim();
        string itemCode = item.Split('|')[0];
        int revision = DBConvert.ParseInt(item.Split('|')[1].ToString());
        DataTable dt = this.GetDataRefLevel2nd(itemCode, revision, iIndex);
        if (dt != null)
        {
          DataSet dsGird = (DataSet)ultraGridAccessoryInfo.DataSource;
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            DataRow row = dsGird.Tables[0].NewRow();
            row["ComponentCode"] = dt.Rows[i]["Code"];
            row["Qty"] = dt.Rows[i]["Qty"];
            row["ComponentName"] = dt.Rows[i]["Name"];
            row["MaterialCode"] = dt.Rows[i]["MaterialCode"];
            row["Length"] = dt.Rows[i]["Length"];
            row["Width"] = dt.Rows[i]["Width"];
            row["Thickness"] = dt.Rows[i]["Thickness"];
            row["ContractOut"] = dt.Rows[i]["ContractOut"];
            row["Waste"] = dt.Rows[i]["Waste"];
            row["TotalQty"] = dt.Rows[i]["TotalQty"];
            row["WorkAreaPid"] = dt.Rows[i]["WorkAreaPid"];
            dsGird.Tables[0].Rows.Add(row);
          }
        }
        this.CheckComponentDuplicate(ultraGridAccessoryInfo);
        this.NeedToSave = true;
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0125", "Item Reference");
      }
    }

    private void btnRefUph_Click(object sender, EventArgs e)
    {
      if (ultCBItemRefUph.SelectedRow != null)
      {
        string item = ultCBItemRefUph.Value.ToString().Trim();
        string itemCode = item.Split('|')[0];
        int revision = DBConvert.ParseInt(item.Split('|')[1].ToString());
        DataTable dt = this.GetDataRefLevel2nd(itemCode, revision, iIndex);
        if (dt != null)
        {
          DataSet dsGird = (DataSet)ultraGridUpholsteryInfo.DataSource;
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            DataRow row = dsGird.Tables[0].NewRow();
            row["ComponentCode"] = dt.Rows[i]["Code"];
            row["Qty"] = dt.Rows[i]["Qty"];
            row["ComponentName"] = dt.Rows[i]["Name"];
            row["MaterialCode"] = dt.Rows[i]["MaterialCode"];
            row["ContractOut"] = dt.Rows[i]["ContractOut"];
            row["Length"] = dt.Rows[i]["Length"];
            row["Width"] = dt.Rows[i]["Width"];
            row["Thickness"] = dt.Rows[i]["Thickness"];
            row["ContractOut"] = dt.Rows[i]["ContractOut"];
            row["Waste"] = dt.Rows[i]["Waste"];
            row["TotalQty"] = dt.Rows[i]["TotalQty"];
            row["WorkAreaPid"] = dt.Rows[i]["WorkAreaPid"];
            dsGird.Tables[0].Rows.Add(row);
          }
        }
        this.CheckComponentDuplicate(ultraGridUpholsteryInfo);
        this.NeedToSave = true;
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0125", "Item Reference");
      }
    }   

    private void ultraGridPackageInfo_MouseClick(object sender, MouseEventArgs e)
    {
      if (ultraGridPackageInfo.Selected != null)
      {
        if (ultraGridPackageInfo.Selected.Rows.All.Length > 0)
        {
          string boxType = ((UltraGridRow)ultraGridPackageInfo.Selected.Rows.All[0]).Cells["BoxCode"].Value.ToString().Replace("/", "-").Replace("_", "-").Replace("\\", "-").Replace("|", "-");
          picturePackage.ImageLocation = FunctionUtility.BOMGetBoxType(boxType);
        }
      }
    }

    private void ultCarving_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      for (int j = 0; j < e.Layout.Bands[0].Columns.Count; j++)
      {
        e.Layout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
      }
    }

    private void ultResin_InitializeLayout_1(object sender, InitializeLayoutEventArgs e)
    {
      for (int j = 0; j < e.Layout.Bands[0].Columns.Count; j++)
      {
        e.Layout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
      }
    }

    private void ultraGridFinishingInfo_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultraGridFinishingInfo.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      viewBOM_07_001 view = new viewBOM_07_001();
      view.currItemCode = itemCode;
      view.currRevision = revision;
      view.type = 2;
      //view.compGroup = 1;
      Shared.Utility.WindowUtinity.ShowView(view, "Master Data", false, ViewState.ModalWindow);
    }

    private void ultraGridFinishingInfo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ultraGridFinishingInfo);
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Bands[2].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      //Gan Dropdown vao grid
      e.Layout.Bands[0].Columns["ComponentCode"].ValueList = ultraDDCompFinish;      
      e.Layout.Bands[0].Columns["ComponentCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ComponentCode"].AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;            
      e.Layout.Bands[0].Columns["FinishProcessMasterPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["FinishProcessMasterPid"].AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["FinishProcessMasterPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;

      //Edit lai Grid        
      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalQty"].CellAppearance.BackColor = Color.LightGray;      
      
      e.Layout.Bands[1].Columns["FinishProcessMasterPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ChemicalPid"].Hidden = true;
      e.Layout.Bands[2].Columns["ChemicalPid"].Hidden = true;

      e.Layout.Bands[1].ColHeaderLines = 2;
      e.Layout.Bands[2].ColHeaderLines = 2;

      // Set Header
      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Màu sơn";
      e.Layout.Bands[0].Columns["FinishProcessMasterPid"].Header.Caption = "Qui trình";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Diện tích";
      e.Layout.Bands[0].Columns["Waste"].Header.Caption = "Hao phí";
      e.Layout.Bands[0].Columns["TotalQty"].Header.Caption = "Tổng diện tích";

      e.Layout.Bands[1].Columns["Step"].Header.Caption = "Bước";
      e.Layout.Bands[1].Columns["ProcessDescription"].Header.Caption = "Qui trình";
      e.Layout.Bands[1].Columns["Chemical"].Header.Caption = "Hợp chất";
      e.Layout.Bands[1].Columns["HoldTime"].Header.Caption = "Thời gian chờ\nkhô (phút)";
      e.Layout.Bands[1].Columns["Consumption"].Header.Caption = "Số lượng";

      e.Layout.Bands[2].Columns["MaterialCode"].Header.Caption = "Mã hóa chất";
      e.Layout.Bands[2].Columns["MaterialName"].Header.Caption = "Tên hóa chất";
      e.Layout.Bands[2].Columns["Consumption"].Header.Caption = "Số lượng";
      e.Layout.Bands[2].Columns["Qty"].Header.Caption = "Tỷ lệ pha";
      e.Layout.Bands[2].Columns["Unit"].Header.Caption = "Đơn vị";

      e.Layout.Bands[1].Columns["A"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["B"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["C"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["A"].DefaultCellValue = 0;
      e.Layout.Bands[1].Columns["B"].DefaultCellValue = 0;
      e.Layout.Bands[1].Columns["C"].DefaultCellValue = 0;

      // Set Width
      e.Layout.Bands[0].Columns["ComponentCode"].Width = 250;
      e.Layout.Bands[0].Columns["Qty"].Width = 60;
      e.Layout.Bands[0].Columns["Waste"].Width = 60;
      e.Layout.Bands[0].Columns["TotalQty"].Width = 80;
      e.Layout.Bands[1].Columns["A"].MinWidth = 40;
      e.Layout.Bands[1].Columns["A"].MaxWidth = 40;
      e.Layout.Bands[1].Columns["B"].MinWidth = 40;
      e.Layout.Bands[1].Columns["B"].MaxWidth = 40;
      e.Layout.Bands[1].Columns["C"].MinWidth = 40;
      e.Layout.Bands[1].Columns["C"].MaxWidth = 40;
      e.Layout.Bands[1].Columns["Step"].MinWidth = 50;
      e.Layout.Bands[1].Columns["Step"].MaxWidth = 50; 
      e.Layout.Bands[2].Columns["Qty"].Width = 80;
      e.Layout.Bands[2].Columns["Unit"].Width = 60;

      foreach (UltraGridRow row in ultraGridFinishingInfo.Rows)
      {
        string finishCode = row.Cells["ComponentCode"].Value.ToString();
        UltraCombo ultc = (UltraCombo)row.Cells["FinishProcessMasterPid"].ValueList;
        row.Cells["FinishProcessMasterPid"].ValueList = LoadComboFinishProcess(finishCode, ultc);
      }
    }

    private void ugdCarcassFinishing_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdCarcassFinishing);
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["IsMainComp"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Rename caption header
      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Comp Code";      
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "VN Description";
      e.Layout.Bands[0].Columns["IsMainComp"].Header.Caption = "Root";     
      e.Layout.Bands[0].Columns["FIN_Length"].Header.Caption = "Length";
      e.Layout.Bands[0].Columns["FIN_Width"].Header.Caption = "Width";
      e.Layout.Bands[0].Columns["FIN_Thickness"].Header.Caption = "Thick";
      e.Layout.Bands[0].Columns["APaintFormula"].Header.Caption = "Paint A";
      e.Layout.Bands[0].Columns["BPaintFormula"].Header.Caption = "Paint B";
      e.Layout.Bands[0].Columns["CPaintFormula"].Header.Caption = "Paint C";
      e.Layout.Bands[0].Columns["APaintValue"].Header.Caption = "Sqm A\n(m2)";
      e.Layout.Bands[0].Columns["BPaintValue"].Header.Caption = "Sqm B\n(m2)";
      e.Layout.Bands[0].Columns["CPaintValue"].Header.Caption = "Sqm C\n(m2)";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.#####";
        }
      }

      // Set Width
      e.Layout.Bands[0].Columns["No"].Width = 30;
      e.Layout.Bands[0].Columns["ComponentCode"].Width = 90;
      e.Layout.Bands[0].Columns["IsMainComp"].Width = 50;
      e.Layout.Bands[0].Columns["DescriptionVN"].Width = 100;
      e.Layout.Bands[0].Columns["Qty"].Width = 40;
      e.Layout.Bands[0].Columns["FIN_Length"].Width = 60;
      e.Layout.Bands[0].Columns["FIN_Width"].Width = 60;
      e.Layout.Bands[0].Columns["FIN_Thickness"].Width = 60;
      e.Layout.Bands[0].Columns["APaintFormula"].Width = 80;
      e.Layout.Bands[0].Columns["APaintFormula"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[0].Columns["BPaintFormula"].Width = 80;
      e.Layout.Bands[0].Columns["BPaintFormula"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[0].Columns["CPaintFormula"].Width = 80;
      e.Layout.Bands[0].Columns["CPaintFormula"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[0].Columns["APaintValue"].Width = 60;
      e.Layout.Bands[0].Columns["BPaintValue"].Width = 60;
      e.Layout.Bands[0].Columns["CPaintValue"].Width = 60;

      for (int k = 0; k < ugdCarcassFinishing.Rows.Count; k++)
      {
        int isMainComp = DBConvert.ParseInt(ugdCarcassFinishing.Rows[k].Cells["IsMainComp"].Value.ToString());
        if (isMainComp == 1)
        {
          ugdCarcassFinishing.Rows[k].CellAppearance.BackColor = Color.LightGray;
        }        
      }
      // set number line header
      e.Layout.Bands[0].ColHeaderLines = 2;

      //Summary
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["APaintValue"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["BPaintValue"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["CPaintValue"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:#,##0.#####}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:#,##0.#####}";
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:#,##0.#####}";
      e.Layout.Bands[0].SummaryFooterCaption = "Total:";      
    }

    private void ultraGridFinishingInfo_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      if (columnName == "FinishProcessMasterPid")
      {
        string finishCode = e.Cell.Row.Cells["ComponentCode"].Value.ToString();
        UltraCombo ultc = (UltraCombo)e.Cell.Row.Cells["FinishProcessMasterPid"].ValueList;
        e.Cell.Row.Cells["FinishProcessMasterPid"].ValueList = LoadComboFinishProcess(finishCode, ultc);        
      }
    }
  }
}