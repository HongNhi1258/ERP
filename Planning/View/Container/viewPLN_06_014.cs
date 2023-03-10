/*
  Author      : 
  Date        : 09/10/2010
  Description : Shipment Request(Planning & Warehouse)
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Planning;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_06_014 : MainUserControl
  {
    #region Field

    private string pathExport = string.Empty;
    public string viewParam = string.Empty;
    //Shipment Request Pid
    public long rRequestPid = long.MinValue;

    //Status Shipment Request
    public int status = int.MinValue;

    //Department UserLogin
    private bool flagDelete = false;
    private bool IsMustShip = false;
    private bool IsRisk = false;

    //Status
    private string stNew = "NEW";
    private string stPlanTe = "PLANNING TEMPORARY CONFIRM";
    private string stWh = "WAREHOUSE CONFIRM";
    private string stPn = "PLANNING CONFIRM";
    private string stIss = "ISSUED";

    //Store
    private string SP_PLNSHIPMENTREQUEST_EDIT = "spPLNShipmentRequest_Edit";
    private string SP_PLNCONTAINERLIST_EDIT = "spPLNContainerList_Edit";
    private string SP_PLNLISTBOXTYPECONT_SELECT = "spPLNListBoxTypeCont_Select";
    private string SP_PLNCONTAINERLISTDETAIL_EDIT = "spPLNContainerListDetail_Edit";
    private string SP_PLNSHIPMENTREQUESTDETAIL_DELETE = "spPLNShipmentRequestDetail_Delete";
    private string SP_PLNSHIPMENTREQUEST_SELECT = "spPLNShipmentRequest_Select";

    //ItemGroup
    private DataTable dtItemGroup = new DataTable();

    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();

    // Qui (18/10/2010 15:30): VB Report
    private string strPathOutputFile;
    private string strPathTemplate;
    // End

    // Ha Anh luoi item - package
    private IList ListdeletingItemPackage = new ArrayList();
    private IList ListdeletedItemPackage = new ArrayList();
    private int flagSendMainEXI = 0;
    //
    #endregion Field

    #region Init
    public viewPLN_06_014()
    {
      InitializeComponent();
      dt_ShipDate.Value = DateTime.MinValue;
      this.LoadItemGroup();
    }

    private void UC_PLNShipmentRequest_Load(object sender, EventArgs e)
    {
      // Qui (18/10/2010 15:30): VB Report
      dt_ShipDate.Value = DateTime.Now;
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      this.strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      this.strPathTemplate = strStartupPath + @"\Template";
      // End

      //Load Data Init
      this.LoadInit();

      if (btnSave.Visible == true || btnWH.Visible == true || (btnMustShip.Visible == false && btnRisk.Visible == false))
      {
        btnCompileMustShipRisk.Visible = false;
      }
      else
      {
        btnCompileMustShipRisk.Visible = true;
        if (this.status == 3)
        {
          btnCompileMustShipRisk.Enabled = false;
        }
      }
      if (this.btnMustShip.Visible == true)
      {
        this.IsMustShip = true;
      }
      else
      {
        this.IsMustShip = false;
      }
      this.btnMustShip.Visible = false;
      if (this.btnRisk.Visible == true)
      {
        this.IsRisk = true;
      }
      else
      {
        this.IsRisk = false;
      }
      this.btnRisk.Visible = false;
      if (this.rRequestPid != long.MinValue)
      {
        this.LoadData();
      }
      this.btnAdd.Visible = this.btnSave.Visible;
      //Load Control Status Depend Shipment Request Status
      this.LoadControlStatus();
      this.chkHide.Checked = true;

    }
    #endregion Init

    #region LoadData

    /// <summary>
    /// Load combobox Itemgroup
    /// Create by: TRAN HUNG
    /// </summary>
    /// 
    private void LoadItemGroup()
    {
      string commandText = string.Format(@" SELECT Value, Code FROM TblBOMCodeMaster WHERE [Group] = 4001 ");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow dr = dtSource.NewRow();
      dtSource.Rows.InsertAt(dr, 0);
      ultcmbItemkind.DataSource = dtSource;
      ultcmbItemkind.DisplayLayout.AutoFitColumns = true;
      ultcmbItemkind.DisplayMember = "Value";
      ultcmbItemkind.ValueMember = "Code";
      ultcmbItemkind.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultcmbItemkind.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitializeOutputDirectory()
    {
      if (Directory.Exists(this.strPathOutputFile))
      {
        string[] files = Directory.GetFiles(this.strPathOutputFile);
        foreach (string file in files)
        {
          try
          {
            File.Delete(file);
          }
          catch { }
        }
      }
      else
      {
        Directory.CreateDirectory(this.strPathOutputFile);
      }
    }

    /// <summary>
    /// Format Export File
    /// </summary>
    /// <param name="strTemplateName"></param>
    /// <param name="strSheetName"></param>
    /// <param name="strPreOutFileName"></param>
    /// <param name="strOutFileName"></param>
    /// <returns></returns>
    private XlsReport InitializeXlsReport(string strTemplateName, string strSheetName, string strPreOutFileName, out string strOutFileName)
    {
      IContainer components = new Container();
      XlsReport oXlsReport = new XlsReport(components);
      this.InitializeOutputDirectory();
      strTemplateName = string.Format(@"{0}\{1}.xls", this.strPathTemplate, strTemplateName);
      oXlsReport.FileName = strTemplateName;
      oXlsReport.Start.File();
      oXlsReport.Page.Begin(strSheetName, "1");
      string time = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second;
      strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", this.strPathOutputFile, strPreOutFileName, DateTime.Now.ToString("yyyyMMdd"), time);
      return oXlsReport;
    }

    /// <summary>
    /// Load Control Status Depend Shipment Request Status
    /// </summary>
    private void LoadControlStatus()
    {
      this.btnReturnWH.Enabled = false;

      if (this.status == 0)
      {
        this.lblStatus.Text = this.stNew;
        this.SetStatus(true);
        if (this.rRequestPid != long.MinValue)
        {
          int count = ultAfter.Rows.Count;
          for (int i = 0; i < count; i++)
          {
            UltraGridRow row = ultAfter.Rows[i];
            row.Cells["Qty"].Activation = Activation.NoEdit;
            row.Cells["Remark"].Activation = Activation.NoEdit;
            if (this.btnSave.Visible == true)
            {
              row.Cells["Qty"].Activation = Activation.AllowEdit;
              row.Cells["Remark"].Activation = Activation.AllowEdit;
            }
            row.Cells["WH Qty"].Activation = Activation.NoEdit;
            row.Cells["Item Group"].Activation = Activation.NoEdit;
            row.Cells["Must Ship Qty"].Activation = Activation.NoEdit;
            row.Cells["CSRemark"].Activation = Activation.NoEdit;
            if (this.IsMustShip)
            {
              row.Cells["Must Ship Qty"].Activation = Activation.AllowEdit;
              row.Cells["CSRemark"].Activation = Activation.AllowEdit;

            }
            row.Cells["Risk"].Activation = Activation.NoEdit;
            row.Cells["ProRemark"].Activation = Activation.AllowEdit;
            if (this.IsRisk)
            {
              row.Cells["Risk"].Activation = Activation.AllowEdit;
              row.Cells["ProRemark"].Activation = Activation.AllowEdit;
            }

            row.Cells["Item Code"].Activation = Activation.NoEdit;
            row.Cells["Revision"].Activation = Activation.NoEdit;
          }
        }
      }
      else if (this.status == 1)
      {
        this.lblStatus.Text = this.stPlanTe;

        if (this.btnSave.Visible == true)
        {
          this.SetStatus(true);
          this.cmbCus.Enabled = false;

          // Load Status After (Depend Status)
          this.chkLock.Enabled = false;
        }
        else
        {
          this.SetStatus(false);
          this.ultAfter.Enabled = true;

          this.chkLock.Enabled = true;
          this.btnSave.Enabled = true;
          this.chkWH.Enabled = true;
          this.btnWH.Enabled = true;
          // Change Colour Condition (After)
          this.ChangeColor(1);
        }
        this.SetStatusAfter(true);
      }
      else if (this.status == 2)
      {
        this.lblStatus.Text = this.stWh;
        if (this.btnSave.Visible == true)
        {
          this.SetStatus(true);
          this.cmbCus.Enabled = false;
          // Change Colour Condition (After)
          this.ChangeColor(2);

          this.btnReturnWH.Enabled = true;
        }
        else
        {
          this.SetStatus(true);
          this.chkLock.Enabled = false;
          this.btnSave.Enabled = false;
          this.chkWH.Enabled = false;
          this.btnWH.Enabled = false;
        }
        this.SetStatusAfter(true);
      }
      else if (this.status == 3)
      {
        this.lblStatus.Text = this.stPn;

        this.SetStatus(true);
        this.chkLock.Enabled = false;
        this.btnSave.Enabled = false;
        this.chkWH.Enabled = false;
        this.btnWH.Enabled = false;

        int count = ultAfter.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          UltraGridRow row = ultAfter.Rows[i];
          row.Activation = Activation.NoEdit;
        }
      }
      else if (this.status == 4)
      {
        this.lblStatus.Text = this.stIss;
        this.SetStatus(false);
        this.chkLock.Enabled = false;
        this.btnSave.Enabled = false;
        this.chkWH.Enabled = false;
        this.btnWH.Enabled = false;

        int count = ultAfter.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          UltraGridRow row = ultAfter.Rows[i];
          row.Activation = Activation.ActivateOnly;
        }
      }

      if (this.rRequestPid != long.MinValue)
      {
        this.cmbCus.Enabled = false;
      }

      if (this.status != 1)
      {
        this.btnWH.Enabled = false;
      }
    }

    /// <summary>
    /// Change Colour Condition (After)
    /// </summary>
    private void ChangeColor(int stt)
    {
      int count = ultAfter.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultAfter.Rows[i];
        if (stt == 2)
        {
          if (DBConvert.ParseInt(row.Cells["WH Qty"].Value.ToString()) < DBConvert.ParseInt(row.Cells["Must Ship Qty"].Value.ToString()))
          {
            row.Appearance.BackColor = Color.Yellow;
          }
          else
          {
            row.Appearance.BackColor = Color.White;
          }
        }
        else if (stt == 1)
        {
          if (DBConvert.ParseInt(row.Cells["Check Box"].Value.ToString()) == 1)
          {
            row.Appearance.BackColor = Color.Yellow;
          }
          else
          {
            row.Appearance.BackColor = Color.White;
          }
        }
      }
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    /// <param name="flag"></param>
    private void SetStatus(Boolean flag)
    {
      this.txtConName.Enabled = flag;
      if (flag == true)
      {
        this.txtConName.ReadOnly = false;
      }
      else
      {
        this.txtConName.ReadOnly = true;
      }

      this.dt_ShipDate.Enabled = flag;
      this.cmbCus.Enabled = flag;

      this.txtFrom.Enabled = flag;
      if (flag == true)
      {
        this.txtFrom.ReadOnly = false;
      }
      else
      {
        this.txtFrom.ReadOnly = true;
      }

      this.txtTo.Enabled = flag;
      if (flag == true)
      {
        this.txtTo.ReadOnly = false;
      }
      else
      {
        this.txtTo.ReadOnly = true;
      }

      this.btnClear.Enabled = flag;
      this.btnSearch.Enabled = flag;
      this.ultBefore.Enabled = flag;
      this.btnAdd.Enabled = flag;
    }

    /// <summary>
    /// Load Status After (Depend Status)
    /// </summary>
    /// <param name="flag"></param>
    private void SetStatusAfter(bool flag)
    {
      int count = ultAfter.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultAfter.Rows[i];
        if (flag == true)
        {
          row.Cells["Item Group"].Activation = Activation.NoEdit;
          row.Cells["Item Code"].Activation = Activation.NoEdit;
          row.Cells["Revision"].Activation = Activation.NoEdit;
          row.Cells["WH Qty"].Activation = Activation.NoEdit;
          if (this.btnWH.Visible == true)
          {
            row.Cells["WH Qty"].Activation = Activation.AllowEdit;
          }
          row.Cells["Qty"].Activation = Activation.NoEdit;
          row.Cells["Remark"].Activation = Activation.NoEdit;
          if (this.btnSave.Visible == true)
          {
            row.Cells["Qty"].Activation = Activation.AllowEdit;
            row.Cells["Remark"].Activation = Activation.AllowEdit;
          }
          row.Cells["Must Ship Qty"].Activation = Activation.NoEdit;
          row.Cells["CSRemark"].Activation = Activation.NoEdit;
          if (this.IsMustShip)
          {
            row.Cells["Must Ship Qty"].Activation = Activation.AllowEdit;
            row.Cells["CSRemark"].Activation = Activation.AllowEdit;

          }
          row.Cells["Risk"].Activation = Activation.NoEdit;
          row.Cells["ProRemark"].Activation = Activation.NoEdit;

          if (this.IsRisk)
          {
            row.Cells["Risk"].Activation = Activation.AllowEdit;
            row.Cells["ProRemark"].Activation = Activation.AllowEdit;
          }
          row.Cells["Check Box"].Activation = Activation.AllowEdit;
        }
        else
        {
          row.Cells["Item Group"].Activation = Activation.NoEdit;
          row.Cells["Item Code"].Activation = Activation.NoEdit;
          row.Cells["Revision"].Activation = Activation.NoEdit;
          row.Cells["Qty"].Activation = Activation.NoEdit;
          row.Cells["Must Ship Qty"].Activation = Activation.NoEdit;
          row.Cells["Risk"].Activation = Activation.NoEdit;
          row.Cells["Remark"].Activation = Activation.NoEdit;
          row.Cells["Check Box"].Activation = Activation.NoEdit;
          if (DBConvert.ParseInt(row.Cells["Check Box"].Value.ToString()) == 1)
          {
            row.Cells["WH Qty"].Activation = Activation.AllowEdit;
          }
          else
          {
            row.Cells["WH Qty"].Activation = Activation.NoEdit;
          }
        }
      }
    }

    /// <summary>
    /// Load data init(Customer)
    /// </summary>
    private void LoadInit()
    {
      // Customer
      Shared.Utility.ControlUtility.LoadCustomerVersion2(cmbCus);
    }

    /// <summary>
    /// Load Data Control (After,Control Input)
    /// </summary>
    private void LoadData()
    {
      DBParameter[] param = new DBParameter[1];
      param[0] = new DBParameter("@ShipmentRequest", DbType.Int64, this.rRequestPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(SP_PLNSHIPMENTREQUEST_SELECT, param);

      if (dsSource.Tables[0].Rows.Count > 0)
      {
        this.txtConName.Text = dsSource.Tables[0].Rows[0]["ContainerNo"].ToString();

        this.dt_ShipDate.Value = (DateTime)dsSource.Tables[0].Rows[0]["ShipDate"];

        this.cmbCus.SelectedValue = DBConvert.ParseLong(dsSource.Tables[0].Rows[0]["CustomerPid"].ToString());

        //Get DataSet utlAfter
        DataSet dsData = GetDataSetAfter();

        dsData.Tables["taAfter"].Merge(dsSource.Tables[1]);
        this.ultAfter.DataSource = dsData;

        for (int i = 0; i < ultAfter.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultAfter.Rows[i].Cells["Risk"].Value.ToString()) > 0)
          {
            ultAfter.Rows[i].Cells["Risk"].Appearance.BackColor = Color.YellowGreen;
          }
          if (DBConvert.ParseInt(ultAfter.Rows[i].Cells["Must Ship Qty"].Value.ToString()) > 0)
          {
            ultAfter.Rows[i].Cells["Must Ship Qty"].Appearance.BackColor = Color.YellowGreen;
          }
        }

        //Ha Anh add allocate package
        dsPLNContainerListItemCodePackage dsShare = new dsPLNContainerListItemCodePackage();
        dsShare.Tables["Item"].Merge(dsSource.Tables[2]);
        dsShare.Tables["Package"].Merge(dsSource.Tables[3]);
        ultGridAllocatePackage.DataSource = dsShare;

        //to mau 
        for (int i = 0; i < ultGridAllocatePackage.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          string columnName = ultGridAllocatePackage.DisplayLayout.Bands[0].Columns[i].Header.Caption;
          if (dsSource.Tables[2] != null && dsSource.Tables[2].Rows.Count > 0 && DBConvert.ParseInt(dsSource.Tables[2].Rows[0]["FlagCont"].ToString()) == 1)
          {
            if ((string.Compare(columnName, "WHItem", true) == 0) || (string.Compare(columnName, "WHBox", true) == 0) || (string.Compare(columnName, "StrPackage", true) == 0))
            {
              ultGridAllocatePackage.DisplayLayout.Bands[0].Columns[i].CellAppearance.ForeColor = Color.Red;
            }
          }
          if (dsSource.Tables[2] != null && dsSource.Tables[2].Rows.Count > 0 && DBConvert.ParseInt(dsSource.Tables[2].Rows[0]["FlagCont"].ToString()) == 0)
          {
            if ((string.Compare(columnName, "WHItem", true) == 0) || (string.Compare(columnName, "WHBox", true) == 0) || (string.Compare(columnName, "StrPackage", true) == 0))
            {
              ultGridAllocatePackage.DisplayLayout.Bands[0].Columns[i].CellAppearance.ForeColor = Color.DarkGreen;
            }
          }
        }

        for (int i = 0; i < ultGridAllocatePackage.Rows.Count; i++)
        {
          string itemcode = ultGridAllocatePackage.Rows[i].Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(ultGridAllocatePackage.Rows[i].Cells["Rev"].Value.ToString());
          int sumqty = 0;
          for (int j = 0; j < ultGridAllocatePackage.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            UltraDropDown UltraDDPackage_Init = (UltraDropDown)ultGridAllocatePackage.Rows[i].ChildBands[0].Rows[j].Cells["PackagePid"].ValueList;
            ultGridAllocatePackage.Rows[i].ChildBands[0].Rows[j].Cells["PackagePid"].ValueList = this.DDPackage(itemcode, revision, UltraDDPackage_Init);
            sumqty += DBConvert.ParseInt(ultGridAllocatePackage.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString());

            //check box le
            if (DBConvert.ParseInt(ultGridAllocatePackage.Rows[i].ChildBands[0].Rows[j].Cells["CheckBox"].Value.ToString()) > 0)
            {
              ultGridAllocatePackage.Rows[i].Appearance.BackColor = Color.SkyBlue;
            }

            if (DBConvert.ParseLong(ultGridAllocatePackage.Rows[i].ChildBands[0].Rows[j].Cells["PackagePid"].Value.ToString()) != DBConvert.ParseLong(ultGridAllocatePackage.Rows[i].Cells["Package"].Value.ToString()))
            {
              ultGridAllocatePackage.Rows[i].Appearance.BackColor = Color.BurlyWood;
            }
          }
          if (sumqty != DBConvert.ParseInt(ultGridAllocatePackage.Rows[i].Cells["Qty"].Value.ToString()))
          {
            ultGridAllocatePackage.Rows[i].Appearance.BackColor = Color.SkyBlue;
          }

          //check qty item and qty box
          if (DBConvert.ParseInt(ultGridAllocatePackage.Rows[i].Cells["CheckBox"].Value.ToString()) == 0)
          {
            ultGridAllocatePackage.Rows[i].Appearance.BackColor = Color.SkyBlue;
          }
        }
      }
      else
      {
        this.ultAfter.DataSource = null;
      }
      btnImport.Visible = (this.rRequestPid != long.MinValue);
    }
    #endregion LoadData

    #region Process
    /// <summary>
    /// Make ItemGroup Data
    /// </summary>
    private void MakeItemGroupData()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 4001 ORDER BY Sort";
      this.dtItemGroup = DataBaseAccess.SearchCommandText(commandText).Tables[0];
    }

    /// <summary>
    /// Load DropDown ItemGroup
    /// </summary>
    /// <param name="udrpMaterials"></param>
    private void LoadDropdownItemGroup(UltraDropDown udrpItemGroup)
    {
      udrpItemGroup.DataSource = this.dtItemGroup;
      udrpItemGroup.ValueMember = "Code";
      udrpItemGroup.DisplayMember = "Value";
      udrpItemGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpItemGroup.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      udrpItemGroup.DisplayLayout.Bands[0].Columns["Value"].Width = 150;
    }

    /// <summary>
    /// Get DataSet utlBefore
    /// </summary>
    /// <returns></returns>
    private DataSet GetDataSetBefore()
    {
      DataSet ds = new DataSet();
      DataTable taBefore = new DataTable("taBefore");
      taBefore.Columns.Add("Item Code", typeof(System.String));
      taBefore.Columns.Add("Revision", typeof(System.Int32));
      taBefore.Columns.Add("Item Group", typeof(System.String));
      taBefore.Columns.Add("Qty", typeof(System.Int32));
      taBefore.Columns.Add("Must Ship Qty", typeof(System.Int32));
      taBefore.Columns.Add("Remark", typeof(System.String));
      taBefore.Columns.Add("Check Box", typeof(System.Int32));

      ds.Tables.Add(taBefore);

      return ds;
    }

    /// <summary>
    /// Get DataSet utlAfter
    /// </summary>
    /// <returns></returns>
    private DataSet GetDataSetAfter()
    {
      DataSet ds = new DataSet();
      DataTable taAfter = new DataTable("taAfter");
      taAfter.Columns.Add("PONo", typeof(System.String));
      taAfter.Columns.Add("SRDPid", typeof(System.Int64));
      taAfter.Columns.Add("Item Group", typeof(System.String));
      taAfter.Columns.Add("Sale Code", typeof(System.String));
      taAfter.Columns.Add("Item Code", typeof(System.String));
      taAfter.Columns.Add("Revision", typeof(System.Int32));
      taAfter.Columns.Add("Balance", typeof(System.Int64));
      taAfter.Columns.Add("Allocated", typeof(System.Int32));
      taAfter.Columns.Add("BalanceQty", typeof(System.Int32));
      taAfter.Columns.Add("ShipQty", typeof(System.Int32));
      taAfter.Columns.Add("RemainQty", typeof(System.Int32));
      taAfter.Columns.Add("Qty", typeof(System.Int32));
      taAfter.Columns.Add("PLNCBM", typeof(System.Double));
      taAfter.Columns.Add("Must Ship Qty", typeof(System.Int32));
      taAfter.Columns.Add("Risk", typeof(System.Double));
      taAfter.Columns.Add("WH Qty", typeof(System.Int32));
      taAfter.Columns.Add("IQCBM", typeof(System.Double));
      taAfter.Columns.Add("Remark", typeof(System.String));
      taAfter.Columns.Add("CSRemark", typeof(System.String));
      taAfter.Columns.Add("ProRemark", typeof(System.String));
      taAfter.Columns.Add("Check Box", typeof(System.Int32));
      taAfter.Columns.Add("ItemGroupPid", typeof(System.Int32));
      taAfter.Columns.Add("SOPid", typeof(System.Int64));

      ds.Tables.Add(taAfter);

      return ds;
    }

    /// <summary>
    /// Check Save Must Ship Qty && WH Qty
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckSaveBeforeInfo(out string message)
    {
      message = string.Empty;
      DataSet dsAfter = (DataSet)this.ultAfter.DataSource;

      if (dsAfter != null && dsAfter.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in dsAfter.Tables[0].Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            if (DBConvert.ParseInt(row["Must Ship Qty"].ToString()) > DBConvert.ParseInt(row["WH Qty"].ToString()))
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0010"), "WH Qty", "Must Ship Qty");
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check Duplicate Data Before
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidBeforeInfo(out string message)
    {
      message = string.Empty;
      DataSet dsBefore = (DataSet)this.ultBefore.DataSource;
      DataSet dsAfter = (DataSet)this.ultAfter.DataSource;
      if (dsBefore == null)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Grid Before");
        return false;
      }

      if (dsAfter != null)
      {
        foreach (DataRow row in dsBefore.Tables[0].Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            if (DBConvert.ParseInt(row["CheckItem"].ToString()) == 1)
            {
              DataRow[] foundRow = dsAfter.Tables[0].Select("SOPid = " + DBConvert.ParseLong(row["SaleOrderPid"].ToString())
                                                    + " AND [Item Code] = '" + row["Item Code"].ToString() + "'"
                                                    + " AND Revision = " + DBConvert.ParseInt(row["Revision"].ToString())
                                                    + " AND ItemGroupPid = " + DBConvert.ParseInt(row["Item Group"].ToString()));
              if (foundRow.Length > 0)
              {
                message = "Have Existed Record " + row["PONo"].ToString() + " Item Code :"
                        + row["Item Code"].ToString() + " Revision:" + row["Revision"].ToString() + " Item Kind: " + row["Item Group"].ToString();
                return false;
              }
            }
          }
        }
      }

      foreach (DataRow row in dsBefore.Tables[0].Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          if (DBConvert.ParseInt(row["CheckItem"].ToString()) == 1)
          {
            if (row["Qty"].ToString().Trim().Length == 0)
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Qty");
              return false;
            }

            if (row["Must Ship Qty"].ToString().Trim().Length == 0)
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Must Ship Qty");
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check Search Data (Customer,ShipDate,Container Name)
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidSearchInfo(out string message)
    {
      message = string.Empty;

      if (this.txtConName.Text.Trim().Length == 0)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Container Name");
        return false;
      }

      if (this.dt_ShipDate.Value == DateTime.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Ship Date");
        return false;
      }

      if (this.cmbCus.SelectedIndex == 0 || this.cmbCus.SelectedIndex == -1)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Customer");
        return false;
      }

      //check save luoi item - package
      //for (int i = 0; i < ultGridAllocatePackage.Rows.Count; i++)
      //{
      //  if (ultGridAllocatePackage.Rows[i].ChildBands[0].Rows.Count == 0)
      //  {
      //    message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0126"), "Package", i.ToString() + " Itemcode = " + ultGridAllocatePackage.Rows[i].Cells["ItemCode"].Value.ToString());
      //    return false;
      //  }
      //  int checkQty = 0;
      //  for (int j = 0; j < ultGridAllocatePackage.Rows[i].ChildBands[0].Rows.Count; j++)
      //  {
      //    checkQty += DBConvert.ParseInt(ultGridAllocatePackage.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString());
      //  }
      //  if (checkQty != DBConvert.ParseInt(ultGridAllocatePackage.Rows[i].Cells["Qty"].Value.ToString()))
      //  {
      //    ultGridAllocatePackage.Rows[i].Appearance.BackColor = Color.Red;
      //    message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Qty of Item " + ultGridAllocatePackage.Rows[i].Cells["ItemCode"].Value.ToString());
      //    return false;
      //  }
      //  else
      //  {
      //    ultGridAllocatePackage.Rows[i].Appearance.BackColor = Color.White;
      //  }
      //}

      return true;
    }

    /// <summary>
    /// Main Save Data (TblPLNShipmentRequest, TblPLNShipmentRequestDetail)
    /// </summary>
    /// <returns></returns>
    private bool SaveData(out string message)
    {
      message = string.Empty;
      bool result = true;

      long cst = this.SaveShipmentRequestInfo();
      if (cst == 0)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0005"));
        this.cmbCus.Enabled = false;
        return false;
      }
      else if (cst == 1)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0006"), "Cont.Name");
        this.cmbCus.Enabled = false;
        return false;
      }

      result = SaveShipmentRequestDetailInfo();
      if (result == false)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0005"));
        this.cmbCus.Enabled = false;
        return false;
      }

      result = SaveShipmentRequestItemPackage();
      if (result == false)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0005"));
        this.cmbCus.Enabled = false;
        return false;
      }
      return result;
    }

    /// <summary>
    /// Save TblPLNShipmentRequest
    /// </summary>
    /// <returns></returns>
    private long SaveShipmentRequestInfo()
    {
      DBParameter[] inputParam = new DBParameter[6];
      DBParameter[] outParam = new DBParameter[2];
      outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      outParam[1] = new DBParameter("@ShipmentRequestPid", DbType.Int64, long.MinValue);

      //insert
      if (this.rRequestPid == long.MinValue)
      {
        inputParam[1] = new DBParameter("@ContNo", DbType.String, this.txtConName.Text);
        if (this.chkLock.Checked)
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);
          this.status = 1;
        }
        else
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, 0);
        }
        inputParam[3] = new DBParameter("@ShipDate", DbType.DateTime, this.dt_ShipDate.Value);
        inputParam[4] = new DBParameter("@CustomerPid", DbType.Int64, DBConvert.ParseLong(this.cmbCus.SelectedValue.ToString()));
        inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }
      //update
      else
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.rRequestPid);
        inputParam[1] = new DBParameter("@ContNo", DbType.String, this.txtConName.Text);
        inputParam[3] = new DBParameter("@ShipDate", DbType.DateTime, this.dt_ShipDate.Value);
        inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        if (this.chkLock.Checked)
        {
          int sttus = status + 1;
          inputParam[2] = new DBParameter("@Status", DbType.Int32, sttus);
          this.status = sttus;
        }
        else
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, status);
        }

        if (this.chkWH.Checked)
        {
          int sttus = status + 1;
          inputParam[2] = new DBParameter("@Status", DbType.Int32, sttus);
          this.status = sttus;
        }
        else
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, status);
        }
      }
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(SP_PLNCONTAINERLIST_EDIT, inputParam, outParam);

      if (this.rRequestPid == long.MinValue)
      {
        this.rRequestPid = DBConvert.ParseLong(outParam[1].Value.ToString());
      }

      long result = DBConvert.ParseLong(outParam[0].Value.ToString());

      return result;
    }

    /// <summary>
    /// Save TblPLNShipmentRequestDetail
    /// </summary>
    /// <returns></returns>
    private bool SaveShipmentRequestDetailInfo()
    {
      DataSet ds = (DataSet)this.ultAfter.DataSource;

      if (ds != null && ds.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow row in ds.Tables[0].Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            DBParameter[] inputParam = new DBParameter[16];
            DBParameter[] outParam = new DBParameter[1];
            outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

            flagSendMainEXI = 1;

            //insert
            if (DBConvert.ParseLong(row["SRDPid"].ToString()) == long.MinValue)
            {
              inputParam[1] = new DBParameter("@ShipmentRequestPid", DbType.Int64, this.rRequestPid);
              inputParam[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseLong(row["Qty"].ToString()));
              inputParam[4] = new DBParameter("@MustShipQty", DbType.Int32, DBConvert.ParseLong(row["Must Ship Qty"].ToString()));
              inputParam[5] = new DBParameter("@WHQty", DbType.Int32, DBConvert.ParseInt(row["WH Qty"].ToString()));
              inputParam[6] = new DBParameter("@ItemGroup", DbType.Int32, DBConvert.ParseInt(row["ItemGroupPid"].ToString()));
              inputParam[7] = new DBParameter("@Remark", DbType.String, 500, row["Remark"].ToString());
              inputParam[8] = new DBParameter("@IsReturn", DbType.Int32, DBConvert.ParseInt(row["Check Box"].ToString()));
              try
              {
                inputParam[9] = new DBParameter("@SOPid", DbType.Int64, DBConvert.ParseLong(row["SOPid"].ToString()));
              }
              catch
              {
                inputParam[9] = new DBParameter("@SOPid", DbType.Int64, DBConvert.ParseLong(row["SaleOrderPid"].ToString()));
              }
              inputParam[10] = new DBParameter("@ItemCode", DbType.AnsiString, 16, row["Item Code"].ToString());
              inputParam[11] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseLong(row["Revision"].ToString()));
              if (row["Risk"] != null && row["Risk"].ToString().Length > 0)
              {
                inputParam[13] = new DBParameter("@Risk", DbType.Double, DBConvert.ParseDouble(row["Risk"].ToString()));
              }
              inputParam[14] = new DBParameter("@CSRemark", DbType.String, 1000, row["CSRemark"].ToString());
              inputParam[15] = new DBParameter("@ProRemark", DbType.String, 1000, row["ProRemark"].ToString());
            }
            //update
            else
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["SRDPid"].ToString()));
              inputParam[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseLong(row["Qty"].ToString()));
              inputParam[4] = new DBParameter("@MustShipQty", DbType.Int32, DBConvert.ParseLong(row["Must Ship Qty"].ToString()));
              inputParam[7] = new DBParameter("@Remark", DbType.String, 500, row["Remark"].ToString());
              if (row["WH Qty"].ToString().Length > 0)
              {
                inputParam[5] = new DBParameter("@WHQty", DbType.Int32, row["WH Qty"].ToString());
              }
              inputParam[8] = new DBParameter("@IsReturn", DbType.Int32, DBConvert.ParseInt(row["Check Box"].ToString()));
              if (row["Risk"] != null && row["Risk"].ToString().Length > 0)
              {
                inputParam[13] = new DBParameter("@Risk", DbType.Double, DBConvert.ParseDouble(row["Risk"].ToString()));
              }
              inputParam[14] = new DBParameter("@CSRemark", DbType.String, 1000, row["CSRemark"].ToString());
              inputParam[15] = new DBParameter("@ProRemark", DbType.String, 1000, row["ProRemark"].ToString());
            }
            Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(SP_PLNCONTAINERLISTDETAIL_EDIT, inputParam, outParam);
            long result = DBConvert.ParseLong(outParam[0].Value.ToString());
            if (result == long.MinValue)
            {
              return false;
            }
          }
        }

        //2.Delete
        foreach (long detailPid in this.listDeletedPid)
        {
          flagSendMainEXI = 1;
          DBParameter[] inputParamDelete = new DBParameter[1];
          inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
          DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(SP_PLNSHIPMENTREQUESTDETAIL_DELETE, inputParamDelete, outputParamDelete);
          long outputValue = DBConvert.ParseLong(outputParamDelete[0].Value.ToString());
          if (outputValue == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    private bool SaveShipmentRequestItemPackage()
    {
      DataSet dsPackage = (DataSet)ultGridAllocatePackage.DataSource;
      if (dsPackage == null)
      {
        return true;
      }
      DataTable dt = dsPackage.Tables[1];

      if (this.ListdeletedItemPackage != null)
      {
        foreach (long pid in this.ListdeletedItemPackage)
        {
          DBParameter[] inputParamDelete = new DBParameter[2];
          inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, pid);
          inputParamDelete[1] = new DBParameter("@DeleteFlag", DbType.Int32, 1);

          DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spPLNContainerListItemPackage_Edit", inputParamDelete, OutputParamDelete);
          long outputValue = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
          if (outputValue == 0)
          {
            return false;
          }
        }
      }

      if (dt != null && dt.Rows.Count > 0)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i].RowState != DataRowState.Deleted)
          {
            //insert
            if (DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()) == long.MinValue)
            {
              DBParameter[] param = new DBParameter[5];
              param[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, dt.Rows[i]["ItemCode"].ToString());
              param[1] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Rev"].ToString()));
              param[2] = new DBParameter("@PackagePid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["PackagePid"].ToString()));
              param[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString()));
              param[4] = new DBParameter("@ContainerListPid", DbType.Int64, this.rRequestPid);

              DBParameter[] outParam = new DBParameter[1];
              outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              DataBaseAccess.ExecuteStoreProcedure("spPLNContainerListItemPackage_Edit", param, outParam);
              long outResult = DBConvert.ParseLong(outParam[0].Value.ToString());
              if (outResult <= 0)
              {
                return false;
              }
            }
            if (DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()) > 0 && dt.Rows[i].RowState == DataRowState.Modified)
            {
              DBParameter[] param = new DBParameter[3];
              param[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()));
              param[1] = new DBParameter("@PackagePid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["PackagePid"].ToString()));
              param[2] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString()));

              DBParameter[] outParam = new DBParameter[1];
              outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              DataBaseAccess.ExecuteStoreProcedure("spPLNContainerListItemPackage_Edit", param, outParam);
              long outResult = DBConvert.ParseLong(outParam[0].Value.ToString());
              if (outResult <= 0)
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    private void SaveItemGroup()
    {
      if (ultAfter.Rows.Count > 0 && ultcmbItemkind.Value != null && ultcmbItemkind.Value.ToString().Length > 0)
      {
        bool flagCheckUp = false;
        for (int i = 0; i < ultAfter.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultAfter.Rows[i].Cells["Check Box"].Value.ToString()) == 1)
          {
            flagCheckUp = true;
          }
        }

        if (flagCheckUp == false)
        {
          for (int i = 0; i < ultAfter.Rows.Count; i++)
          {
            DBParameter[] input = new DBParameter[2];
            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int64, 0);
            input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultAfter.Rows[i].Cells["SRDPid"].Value.ToString()));
            input[1] = new DBParameter("@ItemGroup", DbType.Int32, DBConvert.ParseInt(ultcmbItemkind.Value.ToString().Trim()));
            DataBaseAccess.ExecuteStoreProcedure("spPLNContainerListDetail_UpdateItemGroup", input, output);
          }
        }
        else
        {
          for (int i = 0; i < ultAfter.Rows.Count; i++)
          {
            if (DBConvert.ParseInt(ultAfter.Rows[i].Cells["Check Box"].Value.ToString()) == 1)
            {
              DBParameter[] input = new DBParameter[2];
              DBParameter[] output = new DBParameter[1];
              output[0] = new DBParameter("@Result", DbType.Int64, 0);
              input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultAfter.Rows[i].Cells["SRDPid"].Value.ToString()));
              input[1] = new DBParameter("@ItemGroup", DbType.Int32, DBConvert.ParseInt(ultcmbItemkind.Value.ToString().Trim()));
              DataBaseAccess.ExecuteStoreProcedure("spPLNContainerListDetail_UpdateItemGroup", input, output);
            }
          }
        }
      }
      DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerPriorityRefeshData_Insert");
    }

    private void SendEmail()
    {
      if (this.status == 1 && this.chkLock.Checked)
      {
        Email email = new Email();
        email.Key = email.KEY_PLN_007;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), "Container List " + this.txtConName.Text, "confirmed temporary", " ", userName);
          string body = string.Format(arrList[2].ToString(), "Container List " + this.txtConName.Text, "confirmed temporary", " ", userName);
          email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }
      }
      else if (this.status == 2 && this.chkWH.Checked)
      {
        Email email = new Email();
        email.Key = email.KEY_PLN_007;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), "Container List " + this.txtConName.Text, "confirmed", " ", userName);
          string body = string.Format(arrList[2].ToString(), "Container List " + this.txtConName.Text, "confirmed", " ", userName);
          email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }
      }
      else if (this.status == 3 && this.chkLock.Checked)
      {
        Email email = new Email();
        email.Key = email.KEY_PLN_007;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), "Container List " + this.txtConName.Text, "confirmed", " ", userName);
          string body = string.Format(arrList[2].ToString(), "Container List " + this.txtConName.Text, "confirmed", " ", userName);
          email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }
      }

      //notification for EXI if PLN change loadlist
      try
      {
        string commantText = string.Format(@"SELECT	*
                                              FROM	  TblEXIShipment SHI
                                                INNER JOIN	TblEXIShipmentDetails SHID ON SHID.ShipmentPid = SHI.Pid
                                                INNER JOIN	TblPLNSHPContainerDetails COND ON COND.ContainerPid = SHID.ContainerPid
                                                INNER JOIN	TblPLNContainerList CONL ON CONL.Pid = COND.LoadingListPid
                                              WHERE	  CONL.ContainerNo = '{0}' AND ISNULL(SHI.Confirm, 0) = 1", this.txtConName.Text);
        DataTable dtShip = DataBaseAccess.SearchCommandTextDataTable(commantText);

        if (this.flagSendMainEXI == 1 && (dtShip != null && dtShip.Rows.Count > 0))
        {
          Email email = new Email();
          email.Key = email.KEY_EXI_001;
          ArrayList arrList = email.GetDataMain(email.Key);
          if (arrList.Count == 3)
          {
            string subject = string.Format(arrList[1].ToString(), this.txtConName.Text);
            string body = string.Format(arrList[2].ToString(), this.txtConName.Text);
            email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
          }
          this.flagSendMainEXI = 0;
        }
      }
      catch
      {
        MessageBox.Show("Send Email to Exim department fail!");
      }
    }
    #endregion Process

    #region Event
    /// <summary>
    /// Close Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Clear condition search control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.txtFrom.Text = string.Empty;
      this.txtTo.Text = string.Empty;
      this.txtImportExcel.Text = string.Empty;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    private void Search()
    {
      string message = string.Empty;

      bool success = this.CheckValidSearchInfo(out message);
      if (!success)
      {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      this.cmbCus.Enabled = false;

      DBParameter[] param = new DBParameter[6];
      param[5] = new DBParameter("@ShipmentRequest", DbType.Int64, this.rRequestPid);
      param[0] = new DBParameter("@Customer", DbType.Int64, DBConvert.ParseLong(this.cmbCus.SelectedValue.ToString()));
      param[1] = new DBParameter("@Status", DbType.Int32, 3);
      if (this.txtFrom.Text != string.Empty)
      {
        param[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.txtFrom.Text);
      }
      if (this.txtTo.Text != string.Empty)
      {
        try
        {
          param[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(this.txtTo.Text));
        }
        catch
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Revision");
        }
      }

      if (this.txtImportExcel.Text.Trim().Length > 0)
      {
        try
        {
          DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Data (1)$B2:B1000]").Tables[0];

          if (dtSource == null)
          {
            return;
          }

          string text = string.Empty;
          if (dtSource.Rows.Count > 0)
          {
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
              DataRow row = dtSource.Rows[i];
              text += row["ItemCode"].ToString() + ",";
            }
            text = text.Substring(0, text.Length - 1);

            param[4] = new DBParameter("@LstItemCode", DbType.String, text);
          }
        }
        catch
        {
        }
      }

      DaiCo.Shared.DataSetSource.Planning.dsLoadListDataSO dsSource = new DaiCo.Shared.DataSetSource.Planning.dsLoadListDataSO();
      DataSet ds1 = DataBaseAccess.SearchStoreProcedure(this.SP_PLNLISTBOXTYPECONT_SELECT, param);

      // Make ItemGroup Data
      this.MakeItemGroupData();
      // Load DropDown ItemGroup
      this.LoadDropdownItemGroup(this.udrpItemGroup);

      dsSource.Tables["dtMaster"].Merge(ds1.Tables[0]);
      ultBefore.DataSource = dsSource;
    }

    /// <summary>
    /// Search Item Code
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Init Layout ultBefore
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBefore_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultBefore);
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands["dtMaster"].Columns["Item Group"].ValueList = this.udrpItemGroup;
      e.Layout.Bands[0].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["StockBalance"].Hidden = true;
      e.Layout.Bands[0].Columns["Item Code"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Balance"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ShipQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["RemainQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ShipQty"].Header.Caption = "Pending Container";
      e.Layout.Bands[0].Columns["RemainQty"].Header.Caption = "Non-ShipQty";
      e.Layout.Bands[0].Columns["CheckItem"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CheckItem"].Header.Caption = "Selected";

      e.Layout.Bands[0].Columns["StockBalance"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["StockBalance"].Header.Caption = "Stock Balance";
      e.Layout.Bands[0].Columns["Item Group"].Header.Caption = "Item Kind";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.Yes;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["RemainQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["StockBalance"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Must Ship Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Item Group"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Check Error Before
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBefore_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      if (columnName == "Item Group" || columnName == "Remark")
      {
        return;
      }
      string text = e.Cell.Text;
      if (text.Trim().Length > 0)
      {
        double value = DBConvert.ParseDouble(e.Cell.Text);

        if (value < 0)
        {
          MessageBox.Show(Shared.Utility.FunctionUtility.GetMessage("ERR0024"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          e.Cancel = true;
          return;
        }
      }
    }

    /// <summary>
    /// Check Status Before
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private void ultBefore_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string qty, mShipQty, remainQty;
      string columnName = e.Cell.Column.ToString();

      switch (columnName)
      {
        case "Qty":
          qty = e.Cell.Row.Cells["Qty"].Value.ToString();
          remainQty = e.Cell.Row.Cells["RemainQty"].Value.ToString();
          mShipQty = e.Cell.Row.Cells["Must Ship Qty"].Value.ToString();
          if ((DBConvert.ParseInt(qty) < DBConvert.ParseInt(mShipQty)) && !(DBConvert.ParseInt(qty) == Int32.MinValue && DBConvert.ParseInt(mShipQty) == 0))
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Must Ship Qty", "Qty");
            e.Cell.Value = 0;
            break;
          }
          if (DBConvert.ParseInt(qty) > DBConvert.ParseInt(remainQty))
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Qty", "Non-ShipQty");
            e.Cell.Value = 0;
            break;
          }

          break;
        case "Must Ship Qty":
          qty = e.Cell.Row.Cells["Qty"].Value.ToString();
          mShipQty = e.Cell.Row.Cells["Must Ship Qty"].Value.ToString();
          if (DBConvert.ParseInt(qty) < DBConvert.ParseInt(mShipQty))
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Must Ship Qty", "Qty");
            break;
          }
          break;
        case "CheckItem":
          int chk = DBConvert.ParseInt(e.Cell.Value.ToString());
          if (chk == 1)
          {
            //Item Group
            string itemGroup = e.Cell.Row.Cells["Item Group"].Value.ToString();
            if (itemGroup.Length == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Item Group");
              e.Cell.Value = 0;
              break;
            }

            //Qty 
            qty = e.Cell.Row.Cells["Qty"].Value.ToString();
            if (qty.Length == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Qty");
              e.Cell.Value = 0;
              break;
            }

            //Must Ship Qty
            mShipQty = e.Cell.Row.Cells["Must Ship Qty"].Value.ToString();
            if (mShipQty.Length == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Must Ship Qty");
              e.Cell.Value = 0;
              break;
            }

            if ((DBConvert.ParseInt(qty) < DBConvert.ParseInt(mShipQty)) && !(DBConvert.ParseInt(qty) == Int32.MinValue && DBConvert.ParseInt(mShipQty) == 0))
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Must Ship Qty", "Qty");
              e.Cell.Value = 0;
              break;
            }
            try
            {

              e.Cell.Row.ParentRow.Cells["Item Group"].Activation = Activation.NoEdit;
              e.Cell.Row.Cells["Must Ship Qty"].Activation = Activation.AllowEdit;
              e.Cell.Row.Cells["Qty"].Activation = Activation.AllowEdit;
              e.Cell.Row.ParentRow.Cells["Remark"].Activation = Activation.AllowEdit;
              e.Cell.Row.ParentRow.Cells["Item Code"].Activation = Activation.NoEdit;
              e.Cell.Row.ParentRow.Cells["Revision"].Activation = Activation.NoEdit;
            }
            catch
            {
            }
          }
          else
          {
            try
            {
              e.Cell.Row.Cells["Must Ship Qty"].Value = 0;
              e.Cell.Row.Cells["Qty"].Value = DBNull.Value;
              e.Cell.Row.Cells["Remark"].Value = DBNull.Value;
              e.Cell.Row.Cells["Item Group"].Value = DBNull.Value;
              e.Cell.Row.Cells["Qty"].Activation = Activation.AllowEdit;
              e.Cell.Row.ParentRow.Cells["Item Group"].Activation = Activation.AllowEdit;
              e.Cell.Row.Cells["Must Ship Qty"].Activation = Activation.AllowEdit;
              e.Cell.Row.ParentRow.Cells["Remark"].Activation = Activation.AllowEdit;
              e.Cell.Row.ParentRow.Cells["Item Code"].Activation = Activation.AllowEdit;
              e.Cell.Row.ParentRow.Cells["Revision"].Activation = Activation.AllowEdit;
              e.Cell.Row.Cells["Qty"].Value = DBNull.Value;
              e.Cell.Row.Cells["Remark"].Value = DBNull.Value;
              e.Cell.Row.Cells["Item Group"].Value = DBNull.Value;
            }
            catch { }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Check Error After
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAfter_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      if (columnName == "Remark" || columnName == "CSRemark" || columnName == "ProRemark" || columnName == "Check Box")
      {
        return;
      }

      string text = e.Cell.Text;
      if (text.Trim().Length > 0)
      {
        double value = DBConvert.ParseDouble(e.Cell.Text);

        if (value < 0)
        {
          MessageBox.Show(Shared.Utility.FunctionUtility.GetMessage("ERR0024"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          e.Cancel = true;
          return;
        }
      }

      switch (columnName)
      {
        case "Must Ship Qty":
          long qty = DBConvert.ParseLong(e.Cell.Row.Cells["Qty"].Value.ToString());
          if (e.Cell.Text.Trim().Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Must Ship Qty");
            e.Cancel = true;
            break;
          }

          if (DBConvert.ParseLong(e.Cell.Text) > qty)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Must Ship Qty", "Qty");
            e.Cancel = true;
          }
          break;
        case "Risk":
          if (e.Cell.Text.Trim().Length > 0)
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["Qty"].Value.ToString()) < DBConvert.ParseLong(e.Cell.Text))
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Risk", "Qty");
              e.Cancel = true;
            }
            break;
          }
          break;
        case "Qty":
          long msQty = DBConvert.ParseLong(e.Cell.Row.Cells["Must Ship Qty"].Value.ToString());
          long RiskQty = DBConvert.ParseLong(e.Cell.Row.Cells["Risk"].Value.ToString());
          int remaiQty = DBConvert.ParseInt(e.Cell.Row.Cells["RemainQty"].Value.ToString());
          if (e.Cell.Text.Trim().Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Qty");
            e.Cancel = true;
            break;
          }

          if (DBConvert.ParseLong(e.Cell.Text) < msQty)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Must Ship Qty", "Qty");
            e.Cancel = true;
          }
          if (DBConvert.ParseLong(e.Cell.Text) < RiskQty)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Risk", "Qty");
            e.Cancel = true;
          }
          if (DBConvert.ParseInt(e.Cell.Text) > remaiQty)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Qty", "Non-Ship Qty");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Check Status After
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAfter_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      DataTable dt;
      string commandText = string.Empty;
      switch (columnName)
      {
        case "Qty":
          try
          {
            commandText = "SELECT TotalCBM FROM TblBOMPackage WHERE SetDefault = 1 AND ItemCode = '" + e.Cell.Row.Cells["Item Code"].Value.ToString() + "' AND ";
            commandText += " Revision = " + DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString());

            dt = DataBaseAccess.SearchCommandText(commandText).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
              e.Cell.Row.Cells["PLNCBM"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(dt.Rows[0][0].ToString());
            }
            else
            {
              e.Cell.Row.Cells["PLNCBM"].Value = 0;
            }

            e.Cell.Row.Cells["WH Qty"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString());
          }
          catch
          {
            e.Cell.Row.Cells["PLNCBM"].Value = DBNull.Value;
          }

          //Ha Anh add them qua luoi package
          DataSet dsPackage;
          if ((DataSet)ultGridAllocatePackage.DataSource == null)
          {
            dsPackage = new dsPLNContainerListItemCodePackage();
          }
          else
          {
            dsPackage = (DataSet)ultGridAllocatePackage.DataSource;
          }

          DataRow[] datarow = dsPackage.Tables[0].Select("ItemCode ='" + e.Cell.Row.Cells["Item Code"].Value.ToString() + "'");
          DataRow[] rowafter = ((DataSet)ultAfter.DataSource).Tables[0].Select("[Item Code] ='" + e.Cell.Row.Cells["Item Code"].Value.ToString() + "'");
          int qtysum = 0;
          foreach (DataRow r in rowafter)
          {
            qtysum += DBConvert.ParseInt(r["Qty"].ToString());
          }

          if (datarow.Length == 0)
          {
            DataRow rowParent = dsPackage.Tables[0].NewRow();
            rowParent["ItemCode"] = e.Cell.Row.Cells["Item Code"].Value.ToString();
            rowParent["Rev"] = DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString());
            rowParent["Qty"] = qtysum;
            dsPackage.Tables[0].Rows.Add(rowParent);
          }
          else
          {
            datarow[0]["Qty"] = qtysum;
          }

          ultGridAllocatePackage.DataSource = dsPackage;
          //end

          break;
        case "WH Qty":
          //WH Qty 
          string whqty = e.Cell.Row.Cells["WH Qty"].Value.ToString();
          if (whqty.Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "WH Qty");
            e.Cell.Value = e.Cell.Row.Cells["Qty"].Value.ToString();
            break;
          }

          if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) < DBConvert.ParseInt(whqty))
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "WH Qty", "Qty");
            e.Cell.Value = e.Cell.Row.Cells["Qty"].Value.ToString();
            break;
          }

          commandText = "SELECT TotalCBM FROM TblBOMPackage WHERE SetDefault = 1 AND ItemCode = '" + e.Cell.Row.Cells["Item Code"].Value.ToString() + "' AND ";
          commandText += " Revision = " + DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString());

          dt = DataBaseAccess.SearchCommandText(commandText).Tables[0];
          if (dt != null && dt.Rows.Count > 0)
          {
            e.Cell.Row.Cells["IQCBM"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["WH Qty"].Value.ToString()) * DBConvert.ParseDouble(dt.Rows[0][0].ToString());
          }
          else
          {
            e.Cell.Row.Cells["IQCBM"].Value = 0;
          }
          break;
        case "Check Box":
          int chk = DBConvert.ParseInt(e.Cell.Value.ToString());
          if (chk == 1)
          {
            //Qty 
            string qty = e.Cell.Row.Cells["Qty"].Value.ToString();
            if (qty.Length == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Qty");
              e.Cell.Value = 0;
              break;
            }

            //Must Ship Qty
            string mShipQty = e.Cell.Row.Cells["Must Ship Qty"].Value.ToString();
            if (mShipQty.Length == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Must Ship Qty");
              e.Cell.Value = 0;
              break;
            }

            if (DBConvert.ParseInt(qty) < DBConvert.ParseInt(mShipQty))
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Must Ship Qty", "Qty");
              e.Cell.Value = 0;
              break;
            }

            if (DBConvert.ParseInt(qty) < DBConvert.ParseInt(e.Cell.Row.Cells["WH Qty"].Value.ToString()))
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "WH Qty", "Qty");
              e.Cell.Value = 0;
              break;
            }

            e.Cell.Row.Cells["Qty"].Activation = Activation.AllowEdit;
            e.Cell.Row.Cells["WH Qty"].Activation = Activation.NoEdit;
            e.Cell.Row.Cells["Item Group"].Activation = Activation.NoEdit;
            e.Cell.Row.Cells["Must Ship Qty"].Activation = Activation.AllowEdit;
            e.Cell.Row.Cells["Remark"].Activation = Activation.AllowEdit;
            e.Cell.Row.Cells["Item Code"].Activation = Activation.NoEdit;
            e.Cell.Row.Cells["Revision"].Activation = Activation.NoEdit;
          }
          else
          {
            e.Cell.Row.Cells["Qty"].Activation = Activation.AllowEdit;
            e.Cell.Row.Cells["WH Qty"].Activation = Activation.NoEdit;
            e.Cell.Row.Cells["Item Group"].Activation = Activation.NoEdit;
            e.Cell.Row.Cells["Must Ship Qty"].Activation = Activation.AllowEdit;
            e.Cell.Row.Cells["Remark"].Activation = Activation.AllowEdit;
            e.Cell.Row.Cells["Item Code"].Activation = Activation.NoEdit;
            e.Cell.Row.Cells["Revision"].Activation = Activation.NoEdit;
          }

          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Add Data From Prepare -> List 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      string commandText = string.Empty;

      bool success = this.CheckValidBeforeInfo(out message);
      if (!success)
      {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      DataTable dtAfter = new DataTable();
      dtAfter.Columns.Add("SRDPid", typeof(System.Int64));
      dtAfter.Columns.Add("PONo", typeof(System.String));
      dtAfter.Columns.Add("SOPid", typeof(System.Int64));
      dtAfter.Columns.Add("Item Group", typeof(System.String));
      dtAfter.Columns.Add("ItemGroupPid", typeof(System.Int32));
      dtAfter.Columns.Add("Item Code", typeof(System.String));
      dtAfter.Columns.Add("Revision", typeof(System.Int32));
      dtAfter.Columns.Add("RemainQty", typeof(System.Int32));
      dtAfter.Columns.Add("Qty", typeof(System.Int32));
      dtAfter.Columns.Add("PLNCBM", typeof(System.Double));
      dtAfter.Columns.Add("Must Ship Qty", typeof(System.Int32));
      dtAfter.Columns.Add("WH Qty", typeof(System.Int32));
      dtAfter.Columns.Add("IQCBM", typeof(System.Double));
      dtAfter.Columns.Add("Remark", typeof(System.String));
      dtAfter.Columns.Add("Check Box", typeof(System.Int32));

      DataSet dsBefore = (DataSet)this.ultBefore.DataSource;
      foreach (DataRow row in dsBefore.Tables[0].Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          if (DBConvert.ParseInt(row["CheckItem"].ToString()) == 1)
          {
            DataRow after = dtAfter.NewRow();
            after["SRDPid"] = long.MinValue;
            after["Item Code"] = row["Item Code"].ToString();
            after["Revision"] = row["Revision"].ToString();
            after["SOPid"] = row["SaleOrderPid"].ToString();
            after["PONo"] = row["PONo"].ToString();
            int itemGroup = DBConvert.ParseInt(row["Item Group"].ToString());
            commandText = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 4001 AND DeleteFlag = 0 AND Code = " + DBConvert.ParseInt(row["Item Group"].ToString());
            DataTable dt = DataBaseAccess.SearchCommandText(commandText).Tables[0];
            if (dt.Rows.Count > 0)
            {
              after["Item Group"] = dt.Rows[0][0].ToString();
            }
            after["RemainQty"] = row["RemainQty"].ToString();
            after["Qty"] = row["Qty"].ToString();
            after["WH Qty"] = row["Qty"].ToString();
            after["PLNCBM"] = 0;
            after["IQCBM"] = 0;
            after["Must Ship Qty"] = row["Must Ship Qty"].ToString();
            after["Remark"] = row["Remark"].ToString();
            after["Check Box"] = 1;
            after["ItemGroupPid"] = row["Item Group"].ToString();
            dtAfter.Rows.Add(after);

            //Ha Anh add them khi add them item vao luoi after
            DataSet dsPackage;
            if ((DataSet)ultGridAllocatePackage.DataSource == null)
            {
              dsPackage = new dsPLNContainerListItemCodePackage();
            }
            else
            {
              dsPackage = (DataSet)ultGridAllocatePackage.DataSource;
            }

            DataRow[] datarow = dsPackage.Tables[0].Select("ItemCode ='" + after["Item Code"].ToString() + "'");
            DataRow[] rowafter = dtAfter.Select("[Item Code] ='" + after["Item Code"].ToString() + "'");
            int qty = 0;
            foreach (DataRow r in rowafter)
            {
              qty += DBConvert.ParseInt(r["Qty"].ToString());
            }

            if (datarow.Length == 0)
            {
              DataRow rowParent = dsPackage.Tables[0].NewRow();
              rowParent["ItemCode"] = after["Item Code"].ToString();
              rowParent["Rev"] = DBConvert.ParseInt(after["Revision"].ToString());
              rowParent["Qty"] = qty;
              dsPackage.Tables[0].Rows.Add(rowParent);
            }
            else
            {
              datarow[0]["Qty"] = qty;
            }

            ultGridAllocatePackage.DataSource = dsPackage;
          }
        }
      }

      DataSet dsAfter = (DataSet)this.ultAfter.DataSource;
      if (dsAfter != null && dsAfter.Tables[0].Rows.Count >= 0)
      {
        foreach (DataRow after in dtAfter.Rows)
        {
          DataRow rafter = dsAfter.Tables[0].NewRow();
          rafter["SRDPid"] = DBConvert.ParseLong(after["SRDPid"].ToString());
          try
          {
            rafter["SaleOrderPid"] = DBConvert.ParseLong(after["SOPid"].ToString());
          }
          catch
          {
            rafter["SOPid"] = DBConvert.ParseLong(after["SOPid"].ToString());
          }
          rafter["Item Code"] = after["Item Code"].ToString();
          rafter["PONo"] = after["PONo"].ToString();
          rafter["IQCBM"] = after["IQCBM"].ToString();
          rafter["Revision"] = DBConvert.ParseInt(after["Revision"].ToString());
          rafter["Item Group"] = after["Item Group"].ToString();
          rafter["RemainQty"] = DBConvert.ParseInt(after["RemainQty"].ToString());
          rafter["Qty"] = DBConvert.ParseInt(after["Qty"].ToString());
          rafter["Must Ship Qty"] = DBConvert.ParseInt(after["Must Ship Qty"].ToString());
          rafter["WH Qty"] = DBConvert.ParseInt(after["WH Qty"].ToString()); ;
          rafter["PLNCBM"] = DBConvert.ParseDouble(after["PLNCBM"].ToString());
          rafter["Remark"] = after["Remark"].ToString();
          rafter["Check Box"] = 1;
          rafter["ItemGroupPid"] = DBConvert.ParseInt(after["ItemGroupPid"].ToString());
          dsAfter.Tables[0].Rows.Add(rafter);
        }
      }

      //Get DataSet utlAfter
      DataSet dsData = GetDataSetAfter();

      if (dsAfter != null && dsAfter.Tables[0].Rows.Count >= 0)
      {
        dsData.Tables["taAfter"].Merge(dsAfter.Tables[0]);
      }
      else
      {
        dsData.Tables["taAfter"].Merge(dtAfter);
      }
      ultAfter.DataSource = dsData;
      ultBefore.DataSource = null;

      int count = this.ultAfter.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultAfter.Rows[i];
        if (row.Cells["Check Box"].Value.ToString() == "1")
        {
          row.Cells["Qty"].Activation = Activation.AllowEdit;
          row.Cells["WH Qty"].Activation = Activation.NoEdit;
          row.Cells["Item Group"].Activation = Activation.NoEdit;
          row.Cells["Must Ship Qty"].Activation = Activation.NoEdit;
          row.Cells["CSRemark"].Activation = Activation.NoEdit;
          if (this.IsMustShip)
          {
            row.Cells["Must Ship Qty"].Activation = Activation.AllowEdit;
            row.Cells["CSRemark"].Activation = Activation.AllowEdit;

          }
          row.Cells["Risk"].Activation = Activation.NoEdit;
          row.Cells["ProRemark"].Activation = Activation.NoEdit;
          if (this.IsRisk)
          {
            row.Cells["Risk"].Activation = Activation.AllowEdit;
            row.Cells["ProRemark"].Activation = Activation.AllowEdit;
          }
          row.Cells["Remark"].Activation = Activation.AllowEdit;
          row.Cells["Item Code"].Activation = Activation.NoEdit;
          row.Cells["Revision"].Activation = Activation.NoEdit;
        }
        else
        {
          row.Cells["Qty"].Activation = Activation.AllowEdit;
          row.Cells["WH Qty"].Activation = Activation.NoEdit;
          row.Cells["Item Group"].Activation = Activation.NoEdit;
          row.Cells["Must Ship Qty"].Activation = Activation.NoEdit;
          row.Cells["CSRemark"].Activation = Activation.NoEdit;
          if (this.IsMustShip)
          {
            row.Cells["Must Ship Qty"].Activation = Activation.AllowEdit;
            row.Cells["CSRemark"].Activation = Activation.AllowEdit;
          }
          row.Cells["Risk"].Activation = Activation.NoEdit;
          row.Cells["ProRemark"].Activation = Activation.NoEdit;
          if (this.IsRisk)
          {
            row.Cells["Risk"].Activation = Activation.AllowEdit;
            row.Cells["ProRemark"].Activation = Activation.AllowEdit;
          }
          row.Cells["Remark"].Activation = Activation.AllowEdit;
          row.Cells["Item Code"].Activation = Activation.NoEdit;
          row.Cells["Revision"].Activation = Activation.NoEdit;

        }
      }
    }

    /// <summary>
    /// Init Layout ultBefore
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAfter_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultAfter);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands["taAfter"].Columns["PONo"].Header.Fixed = true;
      e.Layout.Bands["taAfter"].Columns["Sale Code"].Header.Fixed = true;
      e.Layout.Bands["taAfter"].Columns["Item Group"].Header.Fixed = true;
      e.Layout.Bands["taAfter"].Columns["Item Code"].Header.Fixed = true;

      e.Layout.Bands["taAfter"].Columns["Check Box"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["taAfter"].Columns["ItemGroupPid"].Hidden = true;
      e.Layout.Bands["taAfter"].Columns["SRDPid"].Hidden = true;
      try
      {
        e.Layout.Bands["taAfter"].Columns["SaleOrderPid"].Hidden = true;
      }
      catch
      {
        e.Layout.Bands["taAfter"].Columns["SOPid"].Hidden = true;
      }
      e.Layout.Bands["taAfter"].Override.AllowAddNew = AllowAddNew.Yes;

      e.Layout.Bands["taAfter"].Columns["IQCBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taAfter"].Columns["Sale Code"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands["taAfter"].Columns["PLNCBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taAfter"].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taAfter"].Columns["Allocated"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taAfter"].Columns["Balance"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taAfter"].Columns["BalanceQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taAfter"].Columns["ShipQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taAfter"].Columns["RemainQty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands["taAfter"].Columns["Item Group"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["taAfter"].Columns["BalanceQty"].Header.Caption = "Balance Qty";
      e.Layout.Bands["taAfter"].Columns["ShipQty"].Header.Caption = "Pending Container";
      e.Layout.Bands["taAfter"].Columns["RemainQty"].Header.Caption = "Non-Ship Qty";
      e.Layout.Bands["taAfter"].Columns["PLNCBM"].Header.Caption = "PLN CBM";
      e.Layout.Bands["taAfter"].Columns["WH Qty"].Header.Caption = "IQ Qty";
      e.Layout.Bands["taAfter"].Columns["CSRemark"].Header.Caption = "CS Remark";
      e.Layout.Bands["taAfter"].Columns["ProRemark"].Header.Caption = "Prodution Remark";
      e.Layout.Bands["taAfter"].Columns["Item Group"].Header.Caption = "Item Kind";

      if (this.btnWH.Visible)
      {
        e.Layout.Bands["taAfter"].Override.AllowDelete = DefaultableBoolean.False;
      }
      else if (this.btnSave.Visible == true && this.status != 3)
      {
        e.Layout.Bands["taAfter"].Override.AllowDelete = DefaultableBoolean.True;
      }
      else
      {
        e.Layout.Bands["taAfter"].Override.AllowDelete = DefaultableBoolean.False;
      }

      e.Layout.Bands["taAfter"].Columns["Check Box"].Header.Caption = "Selected";

      e.Layout.Bands["taAfter"].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["Must Ship Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["WH Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["PLNCBM"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["IQCBM"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["Risk"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["Allocated"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["Balance"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["BalanceQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["ShipQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["RemainQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["QtyBox"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Columns["Balance"].Header.Caption = "FGW Qty";
      e.Layout.Bands["taAfter"].Columns["Qty"].Header.Caption = "PLN Qty";
      e.Layout.Bands["taAfter"].Columns["Risk"].Header.Caption = "Risk Qty";

      e.Layout.Bands["taAfter"].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["PLNCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["WH Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["IQCBM"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["taAfter"].Summaries[0].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands["taAfter"].Summaries[1].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands["taAfter"].Summaries[2].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands["taAfter"].Summaries[3].DisplayFormat = "{0:###,##0.00}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Save Data (tblPLNShipmentRequest,tblPLNShipmentRequestDetail)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckValidSearchInfo(out message);
      if (!success)
      {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      DataSet ds = (DataSet)this.ultAfter.DataSource;
      if (ds != null && ds.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
          if (dr.RowState == DataRowState.Modified || dr.RowState == DataRowState.Added)
          {
            if (dr["Qty"].ToString().Trim().Length == 0)
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Qty");
              MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
              return;
            }

            if (dr["Must Ship Qty"].ToString().Trim().Length == 0)
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Must Ship Qty");
              MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
              return;
            }
            //Check Non-ShipQty co du de Pending khong
            DBParameter[] inputcheck = new DBParameter[5];
            DBParameter[] outputcheck = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            inputcheck[0] = new DBParameter("@SaleOrderPid", DbType.Int64, DBConvert.ParseLong(dr["SOPid"].ToString()));
            inputcheck[1] = new DBParameter("@ItemCode", DbType.AnsiString, dr["Item Code"].ToString());
            inputcheck[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dr["Revision"].ToString()));
            inputcheck[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dr["Qty"].ToString()));
            inputcheck[4] = new DBParameter("@ShipmentRequest", DbType.Int64, this.rRequestPid);
            Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNLoadingList_CheckIsValid", inputcheck, outputcheck);
            long pidoutcheck = DBConvert.ParseLong(outputcheck[0].Value.ToString().Trim());
            if (pidoutcheck == -1)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0027", "Non-Ship Qty");
              return;
            }
          }
        }
      }

      if (this.chkLock.Checked)
      {
        if (ds == null || ds.Tables[0].Rows.Count == 0)
        {
          message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Grid Before");
          MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          this.cmbCus.Enabled = false;
          return;
        }

        if (ds.Tables[0].Rows.Count > 0)
        {
          bool flag = false;
          foreach (DataRow dr in ds.Tables[0].Rows)
          {
            if (dr.RowState != DataRowState.Deleted)
            {
              flag = true;
            }
          }
          if (flag == false)
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Grid Before");
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.cmbCus.Enabled = false;
            return;
          }
        }
      }

      if (this.status == 2 && this.chkLock.Checked == true)
      {
        bool chk = this.CheckSaveBeforeInfo(out message);
        if (!chk)
        {
          MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
      }

      if (this.chkLock.Checked)
      {
        DialogResult confirm;
        if (this.status == 0)
        {
          confirm = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0019");
          if (confirm == DialogResult.No)
          {
            return;
          }
        }
        else if (this.status == 1)
        {
          confirm = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0020");
          if (confirm == DialogResult.No)
          {
            return;
          }
        }
        else if (this.status == 2)
        {
          string commandText = string.Empty;
          commandText = "SELECT * FROM TblPLNSHPContainerDetails WHERE LoadingListPid = " + this.rRequestPid;
          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null && dtCheck.Rows.Count == 0)
          {
            MessageBox.Show(string.Format(Shared.Utility.FunctionUtility.GetMessage("ERRO117")), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }

          confirm = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0021");
          if (confirm == DialogResult.No)
          {
            return;
          }
        }
      }

      success = this.SaveData(out message);
      if (success)
      {
        this.SaveItemGroup();
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerPriorityRefeshData_Insert");
      this.SendEmail();

      if (this.chkLock.Checked)
      {
        this.chkLock.Checked = false;
      }
      // Load Data Control (After,Control Input)
      this.LoadData();

      //Load Control Status Depend Shipment Request Status
      this.LoadControlStatus();

    }

    /// <summary>
    /// Before After Deleted (Store Pid)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAfter_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long Pid = DBConvert.ParseLong(row.Cells["SRDPid"].Value.ToString());

        if (Pid != long.MinValue)
        {
          if (DBConvert.ParseInt(row.Cells["Allocated"].Value.ToString()) > 0)
          {
            MessageBox.Show("Can not delete because have box in Container List, please remove list boxes first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
          listDeletingPid.Add(Pid);
          flagDelete = true;
        }
      }
    }

    /// <summary>
    /// After After Deleted (Store Pid)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultAfter_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (flagDelete == false)
      {
        return;
      }
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    /// <summary>
    /// Return ShipmentRequest To WH 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReturnWH_Click(object sender, EventArgs e)
    {
      DialogResult confirm = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0018");
      if (confirm == DialogResult.No)
      {
        return;
      }

      DBParameter[] inputParam = new DBParameter[6];
      DBParameter[] outParam = new DBParameter[2];

      outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      outParam[1] = new DBParameter("@ShipmentRequestPid", DbType.Int64, long.MinValue);

      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.rRequestPid);
      inputParam[1] = new DBParameter("@ContNo", DbType.String, this.txtConName.Text);
      inputParam[3] = new DBParameter("@ShipDate", DbType.DateTime, this.dt_ShipDate.Value);
      inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);

      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(SP_PLNSHIPMENTREQUEST_EDIT, inputParam, outParam);
      int result = DBConvert.ParseInt(outParam[0].Value.ToString());
      if (result == 2)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0017", "WareHouse");
      }

      this.status = 1;

      //Send Mail
      if (this.chkLock.Checked)
      {
        Email email = new Email();
        email.Key = email.KEY_PLN_007;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), "Container List " + this.txtConName.Text, "returned to WH", "PLN", userName);
          string body = string.Format(arrList[2].ToString(), "Container List " + this.txtConName.Text, "returned to WH", "PLN", userName);
          email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }
      }

      if (this.chkLock.Checked)
      {
        this.chkLock.Checked = false;
      }
      // Load Data Control (After,Control Input)
      this.LoadData();

      //Load Control Status Depend Shipment Request Status
      this.LoadControlStatus();

    }

    /// <summary>
    /// Button Warehouse Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnWH_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      DataSet ds = (DataSet)this.ultAfter.DataSource;
      if (ds != null && ds.Tables[0].Rows.Count > 0)
      {
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
          if (dr["WH Qty"].ToString().Trim().Length == 0)
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "WH Qty");
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
        }
      }

      if (this.chkWH.Checked)
      {
        DialogResult confirm;
        if (this.status == 0)
        {
          confirm = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0019");
          if (confirm == DialogResult.No)
          {
            return;
          }
        }
        else if (this.status == 1)
        {
          confirm = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0020");
          if (confirm == DialogResult.No)
          {
            return;
          }
        }
        else if (this.status == 2)
        {
          confirm = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0021");
          if (confirm == DialogResult.No)
          {
            return;
          }
        }
      }

      bool success = this.SaveData(out message);
      if (success)
      {

        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Send Mail
      this.SendEmail();

      if (this.chkWH.Checked)
      {
        this.chkWH.Checked = false;
      }
      // Load Data Control (After,Control Input)
      this.LoadData();

      //Load Control Status Depend Shipment Request Status
      this.LoadControlStatus();
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void ultBefore_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
      {
        return;
      }
      int rowIndex = (e.KeyCode == Keys.Down) ? ultBefore.ActiveCell.Row.Index + 1 : ultBefore.ActiveCell.Row.Index - 1;
      int cellIndex = ultBefore.ActiveCell.Column.Index;
      try
      {
        ultBefore.Rows[rowIndex].Cells[cellIndex].Activate();
        ultBefore.PerformAction(UltraGridAction.EnterEditMode, false, false);
      }
      catch { }
    }

    private void ultAfter_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
      {
        return;
      }
      try
      {
        int rowIndex = (e.KeyCode == Keys.Down) ? ultAfter.ActiveCell.Row.Index + 1 : ultAfter.ActiveCell.Row.Index - 1;
        int cellIndex = ultAfter.ActiveCell.Column.Index;
        ultAfter.Rows[rowIndex].Cells[cellIndex].Activate();
        ultAfter.PerformAction(UltraGridAction.EnterEditMode, false, false);
      }
      catch { }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (ultAfter.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ultAfter.Rows.Band.Columns["Item Group"].Hidden = true;
        ultAfter.Rows.Band.Columns["Balance"].Hidden = true;
        ultAfter.Rows.Band.Columns["Allocated"].Hidden = true;
        ultAfter.Rows.Band.Columns["BalanceQty"].Hidden = true;
        ultAfter.Rows.Band.Columns["ShipQty"].Hidden = true;
        ultAfter.Rows.Band.Columns["RemainQty"].Hidden = true;
        ultAfter.Rows.Band.Columns["Must Ship Qty"].Hidden = true;
        ultAfter.Rows.Band.Columns["Risk"].Hidden = true;
        ultAfter.Rows.Band.Columns["WH Qty"].Hidden = true;
        ultAfter.Rows.Band.Columns["IQCBM"].Hidden = true;
        ultAfter.Rows.Band.Columns["Remark"].Hidden = true;
        ultAfter.Rows.Band.Columns["CSRemark"].Hidden = true;
        ultAfter.Rows.Band.Columns["ProRemark"].Hidden = true;
        ultAfter.Rows.Band.Columns["Check Box"].Hidden = true;

        ControlUtility.ExportToExcelWithDefaultPath(ultAfter, out xlBook, "Shipment Request", 6);

        ultAfter.Rows.Band.Columns["Item Group"].Hidden = false;
        ultAfter.Rows.Band.Columns["Balance"].Hidden = false;
        ultAfter.Rows.Band.Columns["Allocated"].Hidden = false;
        ultAfter.Rows.Band.Columns["BalanceQty"].Hidden = false;
        ultAfter.Rows.Band.Columns["ShipQty"].Hidden = false;
        ultAfter.Rows.Band.Columns["RemainQty"].Hidden = false;
        ultAfter.Rows.Band.Columns["Must Ship Qty"].Hidden = false;
        ultAfter.Rows.Band.Columns["Risk"].Hidden = false;
        ultAfter.Rows.Band.Columns["WH Qty"].Hidden = false;
        ultAfter.Rows.Band.Columns["IQCBM"].Hidden = false;
        ultAfter.Rows.Band.Columns["Remark"].Hidden = false;
        ultAfter.Rows.Band.Columns["CSRemark"].Hidden = false;
        ultAfter.Rows.Band.Columns["ProRemark"].Hidden = false;
        ultAfter.Rows.Band.Columns["Check Box"].Hidden = false;

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "Shipment Request";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }

    static public void releaseObject(object obj)
    {
      try
      {
        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        obj = null;
      }
      catch (Exception ex)
      {
        obj = null;
        throw new Exception("Exception Occured while releasing object " + ex.ToString());
      }
      finally
      {
        GC.Collect();
      }
    }

    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkHide.Checked)
      {
        this.panel1.Visible = false;
      }
      else
      {
        this.panel1.Visible = true;
      }
    }

    private void ultAfter_DoubleClick(object sender, EventArgs e)
    {
      if (this.btnSave.Visible)
      {
        bool selected = false;
        try
        {
          selected = ultAfter.Selected.Rows[0].Selected;
        }
        catch
        {
          selected = false;
        }
        if (!selected)
        {
          return;
        }
        UltraGridRow row = (ultAfter.Selected.Rows[0].ParentRow == null) ? ultAfter.Selected.Rows[0] : ultAfter.Selected.Rows[0].ParentRow;
        long pid = DBConvert.ParseLong(row.Cells["SRDPid"].Value.ToString());
        if (pid == long.MinValue)
        {
          return;
        }

        //viewPLN_06_019 view = new viewPLN_06_019();
        //view.containerListDetailPid = pid;
        //Shared.Utility.WindowUtinity.ShowView(view, "ALLOCATE BOX FOR CONTAINER", false, Shared.Utility.ViewState.ModalWindow);

        this.LoadData();
      }
    }

    private void btnCompileMustShipRisk_Click(object sender, EventArgs e)
    {
      btnSave_Click(sender, e);
    }

    private void btnProcess_Click(object sender, EventArgs e)
    {
      if (this.rRequestPid == long.MinValue)
      {
        return;
      }

      viewPLN_06_021 view = new viewPLN_06_021();
      view.containerListPid = this.rRequestPid;
      Shared.Utility.WindowUtinity.ShowView(view, "CHECKING CONTAINERLIST BETWEEN PLANNING & WAREHOUSE", false, Shared.Utility.ViewState.ModalWindow);

      string commandText = string.Empty;
      commandText += " SELECT [Status]";
      commandText += " FROM TblPLNContainerList";
      commandText += " WHERE Pid =" + this.rRequestPid;

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 4)
        {
          this.status = 4;
        }
      }

      this.LoadData();

      this.LoadControlStatus();
    }

    /// <summary>
    /// Init grid package
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridAllocatePackage_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 75;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 75;
      e.Layout.Bands[0].Columns["Rev"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["Rev"].MinWidth = 40;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 40;
      e.Layout.Bands[0].Columns["WHQtyItem"].MaxWidth = 65;
      e.Layout.Bands[0].Columns["WHQtyItem"].MinWidth = 65;
      e.Layout.Bands[0].Columns["WHQtyBox"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["WHQtyBox"].MinWidth = 55;
      e.Layout.Bands[0].Columns["StrPackage"].MaxWidth = 115;
      e.Layout.Bands[0].Columns["StrPackage"].MinWidth = 115;
      e.Layout.Bands[0].Columns["Package"].Hidden = true;
      e.Layout.Bands[0].Columns["FlagCont"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckBox"].Hidden = true;
      e.Layout.Bands[0].Columns["WHQtyBox"].Header.Caption = "WHBox";
      e.Layout.Bands[0].Columns["WHQtyItem"].Header.Caption = "WHItem";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";

      if (SharedObject.UserInfo.Department == "PLA")
      {
        e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.True;
        e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.True;
      }
      else
      {
        e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
        e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      }
      if (e.Layout.Bands[1].Columns.Exists("ContainerListPid"))
      {
        e.Layout.Bands[1].Columns["Pid"].Hidden = true;
        e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
        e.Layout.Bands[1].Columns["ContainerListPid"].Hidden = true;
        e.Layout.Bands[1].Columns["QtyBox"].Hidden = true;
        e.Layout.Bands[1].Columns["QtyItem"].Hidden = true;
        e.Layout.Bands[1].Columns["CheckBox"].Hidden = true;
        e.Layout.Bands[1].Columns["Rev"].Hidden = true;
        e.Layout.Bands[1].Columns["Qty"].MaxWidth = 50;
        e.Layout.Bands[1].Columns["Qty"].MinWidth = 50;

        e.Layout.Bands[1].Columns["PackagePid"].Header.Caption = "Package";

        e.Layout.Bands[1].Columns["PackagePid"].MaxWidth = 150;
        e.Layout.Bands[1].Columns["PackagePid"].MinWidth = 150;

        e.Layout.Bands[1].Columns["Qty"].MaxWidth = 50;
        e.Layout.Bands[1].Columns["Qty"].MinWidth = 50;
      }
    }

    /// <summary>
    /// init dropdown package
    /// </summary>
    private UltraDropDown DDPackage(string itemcode, int revision, UltraDropDown ultDDPackage)
    {
      if (ultDDPackage == null)
      {
        ultDDPackage = new UltraDropDown();
        this.Controls.Add(ultDDPackage);
      }
      string commandText = string.Format(@"  SELECT	Pid PackagePid, ItemCode + '_' + dbo.FSYSPadLeft(CONVERT(varchar, (Revision)), 0, 2) + '/' + 
		                                                  dbo.FSYSPadLeft(CONVERT(varchar, (QuantityItem)), 0, 2) + '_' + dbo.FSYSPadLeft(CONVERT(varchar, (QuantityBox)), 0, 2) Package,
                                                      ItemCode, Revision, SetDefault
                                              FROM    TblBOMPackage  
                                              WHERE ItemCode = '{0}' AND Revision = {1} ", itemcode, revision);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        ultDDPackage.DataSource = dt;
        ultDDPackage.ValueMember = "PackagePid";
        ultDDPackage.DisplayMember = "Package";
        ultDDPackage.DisplayLayout.Bands[0].Columns["PackagePid"].Hidden = true;
        ultDDPackage.DisplayLayout.Bands[0].Columns["ItemCode"].Hidden = true;
        ultDDPackage.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
        ultDDPackage.DisplayLayout.Bands[0].Columns["Package"].Width = 150;
        ultDDPackage.DisplayLayout.Bands[0].Columns["SetDefault"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      return ultDDPackage;
    }

    /// <summary>
    /// grid package before cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridAllocatePackage_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      string message = string.Empty;
      switch (colName)
      {
        case "qty":
          {
            if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Text.ToString()) <= 0)
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Qty");
              MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
              e.Cancel = true;
            }
            //int qty = 0;
            //for (int i = 0; i < e.Cell.Row.ParentRow.ChildBands[0].Rows.Count; i++)
            //{
            //  qty += DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Qty"].Text.ToString());
            //  if (qty > DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["Qty"].Text.ToString()))
            //  {
            //    message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0026"), "Qty");
            //    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.Cancel = true;
            //  }
            //}
            break;
          }
        case "packagepid":
          {
            //check invalid
            UltraDropDown UltraDDPackage_Init = (UltraDropDown)e.Cell.Row.Cells["PackagePid"].ValueList;
            e.Cell.Row.Cells["PackagePid"].ValueList = this.DDPackage(e.Cell.Row.Cells["ItemCode"].Text, DBConvert.ParseInt(e.Cell.Row.Cells["Rev"].Text), UltraDDPackage_Init);
            bool check = this.CheckMemberDownDrop(e.Cell.Row.Cells["PackagePid"].Text.ToString(), "Package", (DataTable)UltraDDPackage_Init.DataSource);
            if (check == false)
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Package");
              MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
              e.Cancel = true;
            }
            //check duplicate
            int count = 0;
            for (int i = 0; i < e.Cell.Row.ParentRow.ChildBands[0].Rows.Count; i++)
            {
              if (e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["PackagePid"].Text == e.Cell.Row.Cells["PackagePid"].Text)
              {
                count++;
              }
              if (count == 2)
              {
                message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0006"), "Package");
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
              }
            }
            break;
          }
      }
    }

    /// <summary>
    /// check giá trị inputvalue với downdrop list
    /// </summary>
    private bool CheckMemberDownDrop(string inputvalue, string colName, DataTable dt)
    {
      bool check = false;
      if (dt != null)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i][colName].ToString() == inputvalue)
          {
            check = true;
            break;
          }
        }
      }
      return check;
    }

    /// <summary>
    /// grid package after cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridAllocatePackage_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      switch (colName)
      {
        case "packagepid":
          {
            break;
          }
      }
    }

    /// <summary>
    /// grid package before row update 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridAllocatePackage_BeforeRowUpdate(object sender, CancelableRowEventArgs e)
    {
      if (e.Row.Cells["Qty"].Text.Length == 0 || e.Row.Cells["PackagePid"].Text.Length == 0)
      {
        e.Cancel = true;
      }
    }

    /// <summary>
    /// grid package before cell active
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridAllocatePackage_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      switch (colName)
      {
        case "packagepid":
          {
            string itemcode = e.Cell.Row.ParentRow.Cells["ItemCode"].Value.ToString();
            int revision = DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["Rev"].Value.ToString());
            for (int j = 0; j < e.Cell.Row.ParentRow.ChildBands[0].Rows.Count; j++)
            {
              UltraDropDown UltraDDPackage_Init = (UltraDropDown)e.Cell.Row.ParentRow.ChildBands[0].Rows[j].Cells["PackagePid"].ValueList;
              e.Cell.Row.ParentRow.ChildBands[0].Rows[j].Cells["PackagePid"].ValueList = this.DDPackage(itemcode, revision, UltraDDPackage_Init);
            }
            break;
          }
      }
    }

    /// <summary>
    /// Grid package after row delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridAllocatePackage_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.ListdeletingItemPackage)
      {
        this.ListdeletedItemPackage.Add(pid);
      }
    }

    /// <summary>
    /// grid package before row delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridAllocatePackage_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.ListdeletingItemPackage = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.ListdeletingItemPackage.Add(pid);
        }
      }
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "PLA_06_014_001";
      string sheetName = "Data";
      string outFileName = "PLA_06_014_001";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnGetTemplatecontainer_Click(object sender, EventArgs e)
    {
      string templateName = "TemplateImportLoadingList";
      string sheetName = "Data";
      string outFileName = "TemplateImportLoadingList";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnFilePatch_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtImport.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      if (this.txtImport.Text.Trim().Length == 0)
      {
        return;
      }

      if (this.rRequestPid == long.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Please save loading list before using this function");
      }
      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImport.Text.Trim(), "SELECT * FROM [Data (1)$B5:D200]").Tables[0];
        string message = string.Empty;
        string commandText = string.Empty;
        foreach (DataRow row in dtSource.Rows)
        {
          if (row["PONo"].ToString().Length == 0
            || row["ItemCode"].ToString().Length == 0
            || row["Qty"].ToString().Length == 0)
          {
            continue;
          }

          DataRow[] foundRow = dtSource.Select("PONo ='" + row["PONo"].ToString()
                + "' AND ItemCode ='" + row["ItemCode"].ToString() + "'");
          if (foundRow.Length > 1)
          {
            message = "PONO :" + row["PONo"].ToString() + "  ItemCode =" + row["ItemCode"].ToString() + " duplicate in excel file";
            WindowUtinity.ShowMessageErrorFromText(message);
            continue;
          }

          long soPid = 0;
          string itemCode = string.Empty;
          int revision = 0;
          int qty = 0;
          int balance = 0;

          commandText = string.Empty;
          commandText += " SELECT SO.Pid SOPid, SOD.ItemCode, SOD.Revision, ISNULL(AA.Balance, 0) Balance ";
          commandText += " FROM TblPLNSaleOrder SO ";
          commandText += "    INNER JOIN TblPLNSaleOrderDetail SOD ON SO.Pid = SOD.SaleOrderPid ";
          commandText += "    LEFT JOIN  ";
          commandText += "    (  ";
          commandText += "        SELECT SaleOrderPid SOPid, ItemCode, Revision, SUM(Balance) Balance ";
          commandText += "        FROM VPLNMasterPlan ";
          commandText += "        GROUP BY SaleOrderPid, ItemCode, Revision ";
          commandText += "    ) AA ON SO.Pid = AA.SOPid ";
          commandText += "    		AND SOD.ItemCode = AA.ItemCode ";
          commandText += " WHERE SO.CustomerPONo = '" + row["PONo"].ToString() + "'";
          commandText += "    AND SOD.ItemCode = '" + row["ItemCode"].ToString() + "'";
          commandText += "    AND SO.CustomerPid =" + DBConvert.ParseLong(this.cmbCus.SelectedValue.ToString());

          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null && dtCheck.Rows.Count > 0)
          {
            soPid = DBConvert.ParseLong(dtCheck.Rows[0]["SOPid"].ToString());
            itemCode = dtCheck.Rows[0]["ItemCode"].ToString();
            revision = DBConvert.ParseInt(dtCheck.Rows[0]["Revision"].ToString());
            qty = DBConvert.ParseInt(row["Qty"].ToString());
            balance = DBConvert.ParseInt(dtCheck.Rows[0]["Balance"].ToString());

            if (qty > balance)
            {
              message = "PONO :" + row["PONo"].ToString() + "  ItemCode =" + row["ItemCode"].ToString() + " > balance In ERP";
              WindowUtinity.ShowMessageErrorFromText(message);
              continue;
            }

            DBParameter[] input = new DBParameter[5];
            input[0] = new DBParameter("@ContainerListPid", DbType.Int64, this.rRequestPid);
            input[1] = new DBParameter("@SOPid", DbType.Int64, soPid);
            input[2] = new DBParameter("@ItemCode", DbType.String, itemCode);
            input[3] = new DBParameter("@Revision", DbType.Int32, revision);
            input[4] = new DBParameter("@Qty", DbType.Int32, qty);

            Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNContainerListDetailImport_Edit", input);
          }
          else
          {
            message = "PONO :" + row["PONo"].ToString() + "  ItemCode =" + row["ItemCode"].ToString() + " have not existed in ERP";
            WindowUtinity.ShowMessageErrorFromText(message);
            continue;
          }
        }

        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadData();
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }

    private void lblCheck_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultAfter.Rows.Count; i++)
      {
        ultAfter.Rows[i].Cells["Check Box"].Value = 1;
      }
    }

    private void lblClear_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultAfter.Rows.Count; i++)
      {
        ultAfter.Rows[i].Cells["Check Box"].Value = 0;
      }
    }
    #endregion Event
  }
}
