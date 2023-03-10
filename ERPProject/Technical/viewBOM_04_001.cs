/* Creator: Nguyễn Văn Trọn
 * Date: 28/12/2010
 * Description: WO Carcass Info
 */

using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.ERPProject.Share.DataSetSource;
using DaiCo.ERPProject.Share.ReportTemplate;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_04_001 : MainUserControl
  {
    private bool loadingData = false;
    private long woNo = long.MinValue;
    private bool isSelectAllChange = true;
    private int flgTEC = 0;

    public viewBOM_04_001()
    {
      InitializeComponent();
    }

    #region function
    private void Search()
    {
      // Kiem tra bat buot nhap WO
      if (ultCBWo.SelectedRow == null)
      {
        if (ultCBWo.Text.Length > 0)
        {
          WindowUtinity.ShowMessageError("ERR0029", "WO");
          return;
        }
        else
        {
          WindowUtinity.ShowMessageError("MSG0005", "WO");
          return;
        }
      }

      chkEditQty.Enabled = true;
      chkEditQty.Checked = false;
      chkSelectedAll.Checked = false;
      long wo = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultCBWo));
      this.woNo = wo;
      groupBoxCarcassList.Text = string.Format("Carcass list of WO {0}", wo);
      DBParameter[] inputParam = new DBParameter[4];
      inputParam[0] = new DBParameter("@WO", DbType.Int64, wo);
      if (ultCBCarcassFrom.SelectedRow != null)
      {
        string fromCarcass = Utility.GetSelectedValueUltraCombobox(ultCBCarcassFrom);
        inputParam[1] = new DBParameter("@FromCarcass", DbType.AnsiString, 16, fromCarcass);
      }
      if (ultCBCarcassTo.SelectedRow != null)
      {
        string toCarcass = Utility.GetSelectedValueUltraCombobox(ultCBCarcassTo);
        inputParam[2] = new DBParameter("@ToCarcass", DbType.AnsiString, 16, toCarcass);
      }
      if (txtOldCode.Text.Trim().Length > 0)
      {
        string oldCode = ("%" + txtOldCode.Text.Trim() + "%");
        inputParam[3] = new DBParameter("@OldCode", DbType.AnsiString, 34, oldCode);
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNWOCarcassInfoList", inputParam);
      if (dtSource != null)
      {
        ultraGridCarcassInfo.DataSource = dtSource;
      }
      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
    }

    private void LoadData()
    {
      this.loadingData = true;
      // Load WO
      string commandText = "Select Distinct Pid From TblPLNWorkOrder Where Confirm = 1 Order By Pid Desc";
      DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtWO != null)
      {
        Utility.LoadUltraCombo(ultCBWo, dtWO, "Pid", "Pid", false);
      }
      this.loadingData = false;
    }

    /// <summary>
    /// Get records were seleted to print routing report
    /// </summary>
    /// <returns></returns>
    private dsBOMWORoutingTicket GetSelectedRoutingData()
    {
      dsBOMWORoutingTicket dsResult = null;

      for (int i = 0; i < ultraGridCarcassInfo.Rows.Count; i++)
      {
        try
        {
          int select = DBConvert.ParseInt(ultraGridCarcassInfo.Rows[i].Cells["Selected"].Value.ToString());
          if (select != 0)
          {
            if (dsResult == null)
            {
              dsResult = new dsBOMWORoutingTicket();
            }
            DBParameter[] param = new DBParameter[5];
            param[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultraGridCarcassInfo.Rows[i].Cells["Wo"].Value.ToString()));
            string carcassCode = ultraGridCarcassInfo.Rows[i].Cells["CarcassCode"].Value.ToString();
            param[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
            if (chkEditQty.Checked)
            {
              int qty = DBConvert.ParseInt(ultraGridCarcassInfo.Rows[i].Cells["Qty"].Value.ToString());
              param[2] = new DBParameter("@Qty", DbType.Int32, qty);
            }
            param[3] = new DBParameter("@BarcodeStatus", DbType.Int32, ConstantClass.BARCODE_CARCASS_COMP_NEW);
            if (this.flgTEC == 1)
            {
              param[4] = new DBParameter("@Flag", DbType.Int32, 1);
            }
            else
            {
              param[4] = new DBParameter("@Flag", DbType.Int32, 0);
            }
            DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMGetDataRoutingTicketWO", 600, param);
            if (dsData != null)
            {
              string fileLogoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, Shared.Utility.ConstantClass.PATH_LOGO);
              byte[] bImageLogo = FunctionUtility.ImagePathToByteArray(fileLogoPath);

              foreach (DataRow rowParent in dsData.Tables[0].Rows)
              {
                DataRow rowResultParent = dsResult.Tables["dtRoutingTicketHeader"].NewRow();
                rowResultParent["Pid"] = rowParent["Pid"];
                rowResultParent["Wo"] = rowParent["Wo"];
                rowResultParent["CarcassCode"] = rowParent["CarcassCode"];
                rowResultParent["CarcassName"] = rowParent["CarcassName"];
                rowResultParent["OldCode"] = rowParent["OldCode"];
                rowResultParent["SaleCode"] = rowParent["SaleCode"];
                rowResultParent["ComponentCode"] = rowParent["ComponentCode"];
                rowResultParent["ComponentName"] = rowParent["ComponentName"];
                rowResultParent["CompPerCarcassQty"] = rowParent["CompPerCarcassQty"];
                rowResultParent["CarcassQty"] = rowParent["CarcassQty"];
                rowResultParent["CompPerWOQty"] = rowParent["CompPerWOQty"];
                rowResultParent["Size"] = rowParent["Size"];
                rowResultParent["PrepareBy"] = rowParent["PrepareBy"];
                rowResultParent["PrepareDate"] = rowParent["PrepareDate"];
                rowResultParent["UpdateBy"] = rowParent["UpdateBy"];
                rowResultParent["UpdateDate"] = rowParent["UpdateDate"];
                rowResultParent["DateStart"] = rowParent["DateStart"];
                rowResultParent["DateRequired"] = rowParent["DateRequired"];
                rowResultParent["Specify"] = rowParent["Specify"];
                rowResultParent["Status"] = rowParent["Status"];
                rowResultParent["Lamination"] = rowParent["Lamination"];
                rowResultParent["FingerJoin"] = rowParent["FingerJoin"];
                rowResultParent["ConStructionRelationship"] = rowParent["ConStructionRelationship"];
                rowResultParent["ItemReferenceInWO"] = rowParent["ItemReferenceInWO"].ToString().Replace(",", System.Environment.NewLine);
                rowResultParent["ProfileList"] = rowParent["ProfileList"];
                rowResultParent["ToolList"] = rowParent["ToolList"];
                rowResultParent["BarCode"] = rowParent["BarCode"];
                rowResultParent["Store"] = rowParent["Store"];
                rowResultParent["CriticalComponent"] = rowParent["CriticalComponent"];
                rowResultParent["Carving"] = rowParent["Carving"];
                rowResultParent["Flag"] = rowParent["Flag"];
                rowResultParent["QCHistory"] = rowParent["QCHistory"];
                rowResultParent["CustomerName"] = rowParent["CustomerName"];
                rowResultParent["Remark"] = rowParent["Remark"];
                rowResultParent["EnvironmentStatus"] = rowParent["EnvironmentStatus"];

                // Component Reference
                DBParameter[] inputParam = new DBParameter[3];
                inputParam[0] = new DBParameter("@WO", DbType.Int64, rowParent["Wo"]);
                inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, rowParent["CarcassCode"]);
                inputParam[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, rowParent["ComponentCode"]);
                DBParameter[] outputParam = new DBParameter[] { new DBParameter("@MainCompList", DbType.AnsiString, 256, string.Empty) };
                DataBaseAccess.ExecuteStoreProcedure("spPLNGetAllLevelMainCompByComponentPid", inputParam, outputParam);
                rowResultParent["ComponentReference"] = outputParam[0].Value;

                //Image logo                  
                rowResultParent["LogoImage"] = bImageLogo;

                //Image carcass
                string fileCarcassPath = FunctionUtility.BOMGetCarcassImage(rowParent["CarcassCode"].ToString());
                //rowResultParent["ImageHeader"] = FunctionUtility.ImagePathToByteArray(fileCarcassPath);
                rowResultParent["ImageHeader"] = FunctionUtility.ImageToByteArrayWithFormat(fileCarcassPath, 380, 0.94, "JPG");

                //Image component
                string fileCompPath = FunctionUtility.BOMGetCarcassComponentImage(rowParent["CarcassCode"].ToString(), rowParent["PictureCode"].ToString());
                rowResultParent["ImageFull"] = FunctionUtility.ImagePathToByteArray(fileCompPath);

                //User
                rowResultParent["User"] = Shared.Utility.SharedObject.UserInfo.EmpName;

                dsResult.Tables["dtRoutingTicketHeader"].Rows.Add(rowResultParent);
              }
              foreach (DataRow rowChild in dsData.Tables[1].Rows)
              {
                DataRow rowResultChild = dsResult.Tables["dtWorkingStep"].NewRow();
                rowResultChild["Wo"] = rowChild["Wo"];
                rowResultChild["CarcassCode"] = rowChild["CarcassCode"];
                rowResultChild["ComponentCode"] = rowChild["ComponentCode"];
                rowResultChild["Step"] = rowChild["Step"];
                rowResultChild["ProcessCode"] = rowChild["ProcessCode"];
                rowResultChild["VNDescription"] = rowChild["VNDescription"];
                rowResultChild["ENDescription"] = rowChild["ENDescription"];
                rowResultChild["MachineGroup"] = rowChild["MachineGroup"];
                rowResultChild["Profile"] = rowChild["Profile"];
                rowResultChild["SetupTime"] = rowChild["SetupTime"];
                rowResultChild["ProcessTime"] = rowChild["ProcessTime"];
                rowResultChild["LeadTime"] = rowChild["LeadTime"];
                rowResultChild["Specification"] = rowChild["Specification"];
                dsResult.Tables["dtWorkingStep"].Rows.Add(rowResultChild);
              }
              foreach (DataRow rowChild in dsData.Tables[2].Rows)
              {
                DataRow rowResultChild = dsResult.Tables["dtMaterials"].NewRow();
                rowResultChild["CompPid"] = rowChild["CompPid"];
                rowResultChild["MaterialCode"] = rowChild["MaterialCode"];
                rowResultChild["MaterialName"] = rowChild["MaterialName"];
                rowResultChild["Qty"] = rowChild["Qty"];
                rowResultChild["Alternative"] = rowChild["Alternative"];
                dsResult.Tables["dtMaterials"].Rows.Add(rowResultChild);
              }
            }
          }
        }
        catch { }
      }
      return dsResult;
    }

    /// <summary>
    /// Get records were seleted to print routing veneer report
    /// </summary>
    /// <returns></returns>
    private dsBOMWORoutingTicketVeneer GetSelectedRoutingVeneerData()
    {
      dsBOMWORoutingTicketVeneer dsResult = null;

      for (int i = 0; i < ultraGridCarcassInfo.Rows.Count; i++)
      {
        try
        {
          int select = DBConvert.ParseInt(ultraGridCarcassInfo.Rows[i].Cells["Selected"].Value.ToString());
          if (select != 0)
          {
            if (dsResult == null)
            {
              dsResult = new dsBOMWORoutingTicketVeneer();
            }
            DBParameter[] param = new DBParameter[4];
            param[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultraGridCarcassInfo.Rows[i].Cells["Wo"].Value.ToString()));
            string carcassCode = ultraGridCarcassInfo.Rows[i].Cells["CarcassCode"].Value.ToString();
            param[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
            if (chkEditQty.Checked)
            {
              int qty = DBConvert.ParseInt(ultraGridCarcassInfo.Rows[i].Cells["Qty"].Value.ToString());
              param[2] = new DBParameter("@Qty", DbType.Int32, qty);
            }
            param[3] = new DBParameter("@BarcodeStatus", DbType.Int32, ConstantClass.BARCODE_CARCASS_COMP_NEW);
            DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMGetDataRoutingTicketVeneerWO", param);
            if (dsData != null)
            {
              string fileLogoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, Shared.Utility.ConstantClass.PATH_LOGO);
              byte[] bImageLogo = FunctionUtility.ImagePathToByteArray(fileLogoPath);

              foreach (DataRow rowParent in dsData.Tables[0].Rows)
              {
                DataRow rowResultParent = dsResult.Tables["dtRoutingTicketHeader"].NewRow();
                rowResultParent["Pid"] = rowParent["Pid"];
                rowResultParent["Wo"] = rowParent["Wo"];
                rowResultParent["CarcassCode"] = rowParent["CarcassCode"];
                rowResultParent["CarcassName"] = rowParent["CarcassName"];
                rowResultParent["ComponentCode"] = rowParent["ComponentCode"];
                rowResultParent["ComponentName"] = rowParent["ComponentName"];
                rowResultParent["CompPerCarcassQty"] = rowParent["CompPerCarcassQty"];
                rowResultParent["CarcassQty"] = rowParent["CarcassQty"];
                rowResultParent["CompPerWOQty"] = rowParent["CompPerWOQty"];
                rowResultParent["Size"] = rowParent["Size"];
                rowResultParent["PrepareBy"] = rowParent["PrepareBy"];
                rowResultParent["PrepareDate"] = rowParent["PrepareDate"];
                rowResultParent["UpdateBy"] = rowParent["UpdateBy"];
                rowResultParent["UpdateDate"] = rowParent["UpdateDate"];
                rowResultParent["DateStart"] = rowParent["DateStart"];
                rowResultParent["ConStructionRelationship"] = rowParent["ConStructionRelationship"];
                rowResultParent["ItemReferenceInWO"] = rowParent["ItemReferenceInWO"];
                rowResultParent["BarCode"] = rowParent["BarCode"];

                // Component Reference
                DBParameter[] inputParam = new DBParameter[3];
                inputParam[0] = new DBParameter("@WO", DbType.Int64, rowParent["Wo"]);
                inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, rowParent["CarcassCode"]);
                inputParam[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, rowParent["ComponentCode"]);
                DBParameter[] outputParam = new DBParameter[] { new DBParameter("@MainCompList", DbType.AnsiString, 256, string.Empty) };
                DataBaseAccess.ExecuteStoreProcedure("spPLNGetAllLevelMainCompByComponentPid", inputParam, outputParam);
                rowResultParent["ComponentReference"] = outputParam[0].Value;

                //Image logo                  
                rowResultParent["LogoImage"] = bImageLogo;

                //Image carcass
                string fileCarcassPath = FunctionUtility.BOMGetCarcassImage(rowParent["CarcassCode"].ToString());
                //rowResultParent["ImageHeader"] = FunctionUtility.ImagePathToByteArray(fileCarcassPath);
                rowResultParent["ImageHeader"] = FunctionUtility.ImageToByteArrayWithFormat(fileCarcassPath, 380, 0.865, "JPG");

                //Image component
                string fileCompPath = FunctionUtility.BOMGetCarcassComponentImage(rowParent["CarcassCode"].ToString(), rowParent["ComponentCode"].ToString());
                rowResultParent["ImageFull"] = FunctionUtility.ImagePathToByteArray(fileCompPath);

                //User
                rowResultParent["User"] = Shared.Utility.SharedObject.UserInfo.EmpName;

                dsResult.Tables["dtRoutingTicketHeader"].Rows.Add(rowResultParent);
              }
              foreach (DataRow rowChild in dsData.Tables[1].Rows)
              {
                DataRow rowResultChild = dsResult.Tables["dtMaterials"].NewRow();
                rowResultChild["CompPid"] = rowChild["CompPid"];
                rowResultChild["MaterialCode"] = rowChild["MaterialCode"];
                rowResultChild["MaterialName"] = rowChild["MaterialName"];
                rowResultChild["Qty"] = rowChild["Qty"];
                rowResultChild["Alternative"] = rowChild["Alternative"];
                dsResult.Tables["dtMaterials"].Rows.Add(rowResultChild);
              }
            }
          }
        }
        catch { }
      }
      return dsResult;
    }

    /// <summary>
    /// Get records were seleted to print cutting list report
    /// </summary>
    /// <returns></returns>
    private dsReportBOMCarcassCuttingList GetSelectedCuttingListData()
    {
      dsReportBOMCarcassCuttingList dsResult = new dsReportBOMCarcassCuttingList();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@WO", DbType.Int64, this.woNo);
      if (!chkSelectedAll.Checked)
      {
        string carcassList = string.Empty;
        for (int i = 0; i < ultraGridCarcassInfo.Rows.Count; i++)
        {
          int selected = DBConvert.ParseInt(ultraGridCarcassInfo.Rows[i].Cells["Selected"].Value.ToString());
          if (selected == 1)
          {
            string carcass = ultraGridCarcassInfo.Rows[i].Cells["CarcassCode"].Value.ToString();
            carcassList += string.Format("|{0}", carcass);
          }
        }
        if (carcassList.Length > 0)
        {
          carcassList += "|";
        }
        else
        {
          return dsResult;
        }
        inputParam[1] = new DBParameter("@CarcassCodeList", DbType.AnsiString, 1024, carcassList);
      }
      DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMCarcassCuttingList", inputParam);
      if (dsData != null)
      {
        dsResult.Tables["tblCarcassInfo"].Merge(dsData.Tables[0]);
        dsResult.Tables["tblMainCompInfo"].Merge(dsData.Tables[1]);
        dsResult.Tables["tblSubCompInfo"].Merge(dsData.Tables[2]);
        dsResult.Tables["tblSummarizeMaterialInfo"].Merge(dsData.Tables[3]);
        foreach (DataRow row in dsResult.Tables["tblCarcassInfo"].Rows)
        {
          row["Image"] = FunctionUtility.ImageToByteArrayWithFormat(FunctionUtility.BOMGetCarcassImage(row["CarcassCode"].ToString()), 380, 1.143, "JPG");
        }
      }
      return dsResult;
    }

    /// <summary>
    /// Load data employee for combobox by dept
    /// </summary>
    /// <param name="comboBox"></param>
    /// <param name="dept"></param>
    public static void LoadComboboxEmployeeInDepartment(ComboBox comboBox, string dept)
    {
      DataTable dt = null;
      if (dept != string.Empty)
      {
        string sql = string.Format("Select (ShortName + ' ' + OtherName) Name From FMISDB.dbo.V_SHREmpInfo_Out WHERE Department = '{0}' AND Resigned = 0 ORDER BY OtherName", dept);
        dt = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(sql);
        DataRow newRow = dt.NewRow();
        dt.Rows.InsertAt(newRow, 0);
      }
      comboBox.DataSource = dt;
      comboBox.ValueMember = "Name";
      comboBox.DisplayMember = "Name";
    }
    #endregion function

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void viewBOM_04_001_Load(object sender, EventArgs e)
    {
      /* flag TEC */
      if (this.btnTEC.Visible)
      {
        this.flgTEC = 1;
      }

      this.btnTEC.Visible = false;
      /* flag TEC */
      this.LoadData();
    }

    private void ultraGridCarcassInfo_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["WO"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WO"].MinWidth = 80;
      e.Layout.Bands[0].Columns["WO"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 110;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 110;
      e.Layout.Bands[0].Columns["Description"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["DescriptionVN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "VN Description";
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["DeadLine"].Header.Caption = "Dead Line";
      e.Layout.Bands[0].Columns["DeadLine"].MinWidth = 80;
      e.Layout.Bands[0].Columns["DeadLine"].MaxWidth = 80;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultraGridCarcassInfo_DoubleClick(object sender, EventArgs e)
    {
      if (ultraGridCarcassInfo.Selected.Rows.Count > 0)
      {
        UltraGridRow row = ultraGridCarcassInfo.Selected.Rows[0];
        string carcassCode = row.Cells["CarcassCode"].Value.ToString();
        int qty = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
        viewBOM_04_006 view = new viewBOM_04_006();
        view.wo = DBConvert.ParseLong(row.Cells["WO"].Value.ToString()); ;
        view.carcass = carcassCode;
        Shared.Utility.WindowUtinity.ShowView(view, carcassCode, true, ViewState.Window, FormWindowState.Maximized);
      }
      //try
      //{
      //  UltraGridRow row = ultraGridCarcassInfo.Selected.Rows[0];
      //  string carcassCode = row.Cells["CarcassCode"].Value.ToString();
      //  int qty = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
      //  viewBOM_04_002 view = new viewBOM_04_002();
      //  view.wo = woNo;
      //  view.carcassCode = carcassCode;
      //  view.qty = qty;
      //  Shared.Utility.WindowUtinity.ShowView(view, carcassCode, true, ViewState.Window, FormWindowState.Maximized);
      //}
      //catch { }
    }

    private void chkEditQty_CheckedChanged(object sender, EventArgs e)
    {
      if (chkEditQty.Checked)
      {
        ultraGridCarcassInfo.DisplayLayout.Bands[0].Columns["Qty"].CellActivation = Activation.AllowEdit;
        chkEditQty.Enabled = false;
      }
    }

    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.isSelectAllChange)
      {
        int checkAll = (chkSelectedAll.Checked ? 1 : 0);
        for (int i = 0; i < ultraGridCarcassInfo.Rows.Count; i++)
        {
          if (ultraGridCarcassInfo.Rows[i].IsFilteredOut == false)
          {
            ultraGridCarcassInfo.Rows[i].Cells["Selected"].Value = checkAll;
          }
        }
      }
    }

    private void btnPrintRouting_Click(object sender, EventArgs e)
    {
      tableLayoutButton.Enabled = false;
      DaiCo.Shared.View_Report report = null;
      ReportClass cpt = null;

      // Report Routing Ticket
      dsBOMWORoutingTicket dsRoutingTicket = this.GetSelectedRoutingData();
      if (dsRoutingTicket != null)
      {
        cpt = new cptBOMWORoutingTicketTec();
        cpt.SetDataSource(dsRoutingTicket);
        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = true;
        report.ShowReport(Shared.Utility.ViewState.Window, true, FormWindowState.Maximized);
      }
      else
      {
        WindowUtinity.ShowMessageWarning("WRN0024", "Carcass");
      }
      tableLayoutButton.Enabled = true;
    }

    private void btnPrintCuttingList_Click(object sender, EventArgs e)
    {
      tableLayoutButton.Enabled = false;
      dsReportBOMCarcassCuttingList dsCuttingList = this.GetSelectedCuttingListData();
      if (dsCuttingList != null)
      {
        cptBOMWOCarcassCuttingList cpt = new cptBOMWOCarcassCuttingList();
        cpt.SetDataSource(dsCuttingList);
        cpt.SetParameterValue("UserName", Shared.Utility.SharedObject.UserInfo.EmpName);
        string commandText = string.Format("Select ManagerName From VHRDDepartmentInfo Where Code = '{0}'", Shared.Utility.ConstantClass.TECHICAL_DEPT);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        string managerName = string.Empty;
        if (obj != null)
        {
          managerName = obj.ToString();
        }
        cpt.SetParameterValue("ApprovedBy", managerName);
        DaiCo.Shared.View_Report report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = true;
        report.ShowReport(Shared.Utility.ViewState.Window, true, FormWindowState.Maximized);
      }
      tableLayoutButton.Enabled = true;
    }

    private void ultraGridCarcassInfo_CellChange(object sender, CellEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare("Selected", column, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          this.isSelectAllChange = false;
          chkSelectedAll.Checked = false;
          this.isSelectAllChange = true;
        }
      }
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }

    private void btnRoutingVeneer_Click(object sender, EventArgs e)
    {
      tableLayoutButton.Enabled = false;
      // Report Routing Ticket Veneer      
      DaiCo.Shared.View_Report report = null;
      ReportClass cpt = null;

      dsBOMWORoutingTicketVeneer dsRoutingTicketVeneer = this.GetSelectedRoutingVeneerData();

      if (dsRoutingTicketVeneer != null && dsRoutingTicketVeneer.Tables.Count > 0 && dsRoutingTicketVeneer.Tables["dtRoutingTicketHeader"].Rows.Count > 0)
      {
        cpt = new cptBOMWORoutingTicketVeneer();
        cpt.SetDataSource(dsRoutingTicketVeneer);
        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = true;
        report.ShowReport(Shared.Utility.ViewState.Window, true, FormWindowState.Maximized);
      }
      tableLayoutButton.Enabled = true;
    }

    private void ultCBWo_ValueChanged(object sender, EventArgs e)
    {
      if (!this.loadingData)
      {
        if (ultCBWo.SelectedRow != null)
        {
          ultCBCarcassFrom.Value = string.Empty;
          ultCBCarcassTo.Value = string.Empty;
          long wo = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultCBWo));
          string commandCarcass = string.Format("Select Distinct WO.CarcassCode, CARCASS.[Description], (WO.CarcassCode + ' | ' + CARCASS.[Description]) Descript From TblPLNWorkOrderConfirmedDetails WO INNER JOIN TblBOMCarcass CARCASS ON WO.CarcassCode = CARCASS.CarcassCode Where WO.WorkOrderPid = {0}", wo);
          DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandCarcass);
          if (dtCarcass != null)
          {
            Utility.LoadUltraCombo(ultCBCarcassFrom, dtCarcass, "CarcassCode", "Descript");
            DataTable dtToCarcass = FunctionUtility.CloneTable(dtCarcass);
            Utility.LoadUltraCombo(ultCBCarcassTo, dtToCarcass, "CarcassCode", "Descript");
          }
        }
        else
        {
          ultCBCarcassFrom.DataSource = null;
          ultCBCarcassTo.DataSource = null;
        }
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultraGridCarcassInfo, "WoCarcassInfo");
    }

    private void ultraGridCarcassInfo_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
    {
      for (int i = 0; i < ultraGridCarcassInfo.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridCarcassInfo.Rows[i];
        if (row.IsFilteredOut == true)
        {
          row.Cells["Selected"].Value = 0;
        }
      }
      lbCount.Text = string.Format("Count: {0}", ultraGridCarcassInfo.Rows.FilteredInRowCount);
    }
  }
}
