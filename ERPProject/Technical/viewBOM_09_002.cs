/* Creator: Huynh Thi Bang
 * Date: 28/08/2017
 * Description: WO Carcass Info
 */

//using DaiCo.ERPProject.DataSetSource;
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.ERPProject.Share.DataSetSource;
using DaiCo.ERPProject.Share.ReportTemplate;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_09_002 : MainUserControl
  {
    private bool loadingData = false;
    private IList listDeletedPid = new ArrayList();
    private long woNo = long.MinValue;
    private bool isSelectAllChange = true;
    private int flgTEC = 0;

    public viewBOM_09_002()
    {
      InitializeComponent();
    }

    #region function
    private void Search()
    {
      // Kiem tra bat buot nhap WO
      //if (ultCBWo.SelectedRow == null)
      //{
      //  if (ultCBWo.Text.Length > 0)
      //  {
      //    WindowUtinity.ShowMessageError("ERR0029", "WO");
      //    return;
      //  }
      //  else
      //  {
      //    WindowUtinity.ShowMessageError("MSG0005", "WO");
      //    return;
      //  }
      //}

      //chkSelectedAll.Checked = false;
      chkEditQty.Enabled = true;
      chkEditQty.Checked = false;

      long wo = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultCBWo));
      this.woNo = wo;
      DBParameter[] inputParam = new DBParameter[5];
      if (this.woNo != long.MinValue)
      {
        groupBoxCarcassList.Text = string.Format("Carcass list of WO {0}", wo);
        inputParam[0] = new DBParameter("@WO", DbType.Int64, wo);
      }
      if (txtCarcass.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@Carcass", DbType.String, 32, txtCarcass.Text.Trim());
      }
      if (txtOldCode.Text.Trim().Length > 0)
      {
        string oldCode = ("%" + txtOldCode.Text.Trim() + "%");
        inputParam[2] = new DBParameter("@OldCode", DbType.AnsiString, 34, oldCode);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNWOItemCarcassInfo_Search", inputParam);
      if (dtSource != null)
      {
        ugdCarcassInfo.DataSource = dtSource;
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

      for (int i = 0; i < ugdCarcassInfo.Rows.Count; i++)
      {
        try
        {
          int select = DBConvert.ParseInt(ugdCarcassInfo.Rows[i].Cells["Selected"].Value.ToString());
          if (select != 0)
          {
            if (dsResult == null)
            {
              dsResult = new dsBOMWORoutingTicket();
            }
            DBParameter[] param = new DBParameter[5];
            param[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ugdCarcassInfo.Rows[i].Cells["WorkOrder"].Value.ToString()));
            string carcassCode = ugdCarcassInfo.Rows[i].Cells["CarcassCode"].Value.ToString();
            param[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
            param[2] = new DBParameter("@BarcodeStatus", DbType.Int32, ConstantClass.BARCODE_CARCASS_COMP_NEW);
            if (this.flgTEC == 1)
            {
              param[3] = new DBParameter("@Flag", DbType.Int32, 1);
            }
            else
            {
              param[3] = new DBParameter("@Flag", DbType.Int32, 0);
            }
            int qty = DBConvert.ParseInt(ugdCarcassInfo.Rows[i].Cells["Qty"].Value.ToString());
            param[4] = new DBParameter("@Qty", DbType.Int32, qty);
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
    /// Get records were seleted to print cutting list report
    /// </summary>
    /// <returns></returns>
    private dsReportBOMCarcassCuttingList GetSelectedCuttingListData()
    {
      dsReportBOMCarcassCuttingList dsResult = new dsReportBOMCarcassCuttingList();
      DBParameter[] inputParam = new DBParameter[2];
      //if (this.woNo != long.MinValue)
      //{
      //  inputParam[0] = new DBParameter("@WO", DbType.Int64, this.woNo);
      //}
      //else
      //{
      for (int i = 0; i < ugdCarcassInfo.Rows.Count; i++)
      {
        int selected = DBConvert.ParseInt(ugdCarcassInfo.Rows[i].Cells["Selected"].Value.ToString());
        if (selected == 1)
        {
          long wo = DBConvert.ParseLong(ugdCarcassInfo.Rows[i].Cells["WorkOrder"].Value.ToString());
          inputParam[0] = new DBParameter("@WO", DbType.Int64, wo);
        }
      }
      //} 
      if (!chkSelectedAll.Checked)
      {
        string carcassList = string.Empty;
        for (int i = 0; i < ugdCarcassInfo.Rows.Count; i++)
        {
          int selected = DBConvert.ParseInt(ugdCarcassInfo.Rows[i].Cells["Selected"].Value.ToString());
          if (selected == 1)
          {
            string carcass = ugdCarcassInfo.Rows[i].Cells["CarcassCode"].Value.ToString();
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

    private void viewBOM_09_002_Load(object sender, EventArgs e)
    {
      ///* flag TEC */
      //if (this.btnTEC.Visible)
      //{
      //  this.flgTEC = 1;
      //}

      //this.btnTEC.Visible = false;
      ///* flag TEC */
      this.LoadData();
    }

    private void ugdCarcassInfo_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      //e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].Columns["WorkOrder"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["WorkOrder"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WorkOrder"].MinWidth = 60;
      e.Layout.Bands[0].Columns["WorkOrder"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 110;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 110;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["OldCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["OldCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[0].Columns["Description"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["OldCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation != Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

    }



    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.isSelectAllChange)
      {
        int checkAll = (chkSelectedAll.Checked ? 1 : 0);
        for (int i = 0; i < ugdCarcassInfo.Rows.Count; i++)
        {
          if (ugdCarcassInfo.Rows[i].IsFilteredOut == false)
          {
            ugdCarcassInfo.Rows[i].Cells["Selected"].Value = checkAll;
          }
        }
      }
    }

    private void chkEditQty_CheckedChanged(object sender, EventArgs e)
    {
      if (chkEditQty.Checked)
      {
        ugdCarcassInfo.DisplayLayout.Bands[0].Columns["Qty"].CellActivation = Activation.AllowEdit;
        chkEditQty.Enabled = false;
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

    private void ugdCarcassInfo_CellChange(object sender, CellEventArgs e)
    {

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

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdCarcassInfo, "WoCarcassInfo");
    }

    private void ugdCarcassInfo_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
    {
      for (int i = 0; i < ugdCarcassInfo.Rows.Count; i++)
      {
        UltraGridRow row = ugdCarcassInfo.Rows[i];
        if (row.IsFilteredOut == true)
        {
          row.Cells["Selected"].Value = 0;
        }
      }
      lbCount.Text = string.Format("Count: {0}", ugdCarcassInfo.Rows.FilteredInRowCount);
    }

    private void ugdCarcassInfo_DoubleClick(object sender, EventArgs e)
    {
      if (ugdCarcassInfo.Selected.Rows.Count > 0)
      {
        UltraGridRow row = ugdCarcassInfo.Selected.Rows[0];
        string carcassCode = row.Cells["CarcassCode"].Value.ToString();
        long wo = DBConvert.ParseLong(row.Cells["WorkOrder"].Value);
        int qty = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
        viewBOM_04_006 view = new viewBOM_04_006();
        view.wo = wo;
        view.carcass = carcassCode;
        view.carcassQty = qty;
        Shared.Utility.WindowUtinity.ShowView(view, carcassCode, true, ViewState.Window, FormWindowState.Maximized);
      }
    }

    private void ugdCarcassInfo_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {

    }

    private void btnPrintRoutingItem_Click(object sender, EventArgs e)
    {
      tableLayoutButton.Enabled = false;
      DaiCo.Shared.View_Report report = null;
      ReportClass cpt = null;

      // Report Routing Ticket
      dsBOMWORoutingTicketForItem dsRoutingTicket = this.GetSelectedRoutingDataForItem();
      if (dsRoutingTicket != null)
      {
        cpt = new cptBOMWORoutingTicketForItem();
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
    /// <summary>
    /// Get records were seleted to print routing report
    /// </summary>
    /// <returns></returns>
    private dsBOMWORoutingTicketForItem GetSelectedRoutingDataForItem()
    {
      dsBOMWORoutingTicketForItem dsResult = null;

      for (int i = 0; i < ugdCarcassInfo.Rows.Count; i++)
      {
        try
        {
          int select = DBConvert.ParseInt(ugdCarcassInfo.Rows[i].Cells["Selected"].Value.ToString());
          if (select != 0)
          {
            if (dsResult == null)
            {
              dsResult = new dsBOMWORoutingTicketForItem();
            }
            DBParameter[] param = new DBParameter[5];
            if (this.woNo != long.MinValue)
            {
              param[0] = new DBParameter("@WO", DbType.Int64, this.woNo);
            }
            else
            {
              param[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ugdCarcassInfo.Rows[i].Cells["WorkOrder"].Value.ToString()));
            }
            string itemCode = ugdCarcassInfo.Rows[i].Cells["ItemCode"].Value.ToString();
            int revision = DBConvert.ParseInt(ugdCarcassInfo.Rows[i].Cells["Revision"].Value.ToString());
            int qty = DBConvert.ParseInt(ugdCarcassInfo.Rows[i].Cells["Qty"].Value.ToString());
            param[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);

            if (this.flgTEC == 1)
            {
              param[2] = new DBParameter("@Flag", DbType.Int32, 1);
            }
            else
            {
              param[2] = new DBParameter("@Flag", DbType.Int32, 0);
            }
            param[3] = new DBParameter("@Revision", DbType.Int32, revision);
            param[4] = new DBParameter("@Qty", DbType.Int32, qty);
            DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMGetDataRoutingTicketWOForItem", 600, param);
            if (dsData != null)
            {
              string fileLogoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, Shared.Utility.ConstantClass.PATH_LOGO);
              byte[] bImageLogo = FunctionUtility.ImagePathToByteArray(fileLogoPath);

              foreach (DataRow rowParent in dsData.Tables[0].Rows)
              {
                DataRow rowResultParent = dsResult.Tables["dtRoutingTicketHeader"].NewRow();
                rowResultParent["Pid"] = rowParent["Pid"];
                rowResultParent["Wo"] = rowParent["Wo"];
                rowResultParent["ItemCode"] = rowParent["ItemCode"];
                rowResultParent["Revision"] = rowParent["Revision"];
                rowResultParent["ItemName"] = rowParent["ItemName"];
                rowResultParent["PartCode"] = rowParent["PartCode"];
                rowResultParent["PartName"] = rowParent["PartName"];
                rowResultParent["PartPerItemQty"] = rowParent["PartPerItemQty"];
                rowResultParent["ItemQty"] = rowParent["ItemQty"];
                rowResultParent["PartPerWOQty"] = rowParent["PartPerWOQty"];
                rowResultParent["Size"] = rowParent["Size"];
                rowResultParent["PrepareBy"] = rowParent["PrepareBy"];
                rowResultParent["PrepareDate"] = rowParent["PrepareDate"];
                rowResultParent["UpdateBy"] = rowParent["UpdateBy"];
                rowResultParent["UpdateDate"] = rowParent["UpdateDate"];
                rowResultParent["DateStart"] = rowParent["DateStart"];
                rowResultParent["DateRequired"] = rowParent["DateRequired"];
                rowResultParent["BarCode"] = rowParent["BarCode"];
                rowResultParent["Flag"] = rowParent["Flag"];
                rowResultParent["CustomerName"] = rowParent["CustomerName"];
                rowResultParent["Finishing"] = rowParent["Finishing"];
                rowResultParent["CriticalComponent"] = rowParent["CriticalComponent"];
                rowResultParent["Carving"] = rowParent["Carving"];
                rowResultParent["Lamination"] = rowParent["Lamination"];
                rowResultParent["FingerJoin"] = rowParent["FingerJoin"];
                rowResultParent["Store"] = rowParent["Store"];
                rowResultParent["EnvironmentStatus"] = rowParent["EnvironmentStatus"];

                //Image logo                  
                rowResultParent["LogoImage"] = bImageLogo;

                //Image Item
                string fileItemPath = FunctionUtility.RDDGetItemImage(rowParent["ItemCode"].ToString());
                //rowResultParent["ImageHeader"] = FunctionUtility.ImagePathToByteArray(fileCarcassPath);
                rowResultParent["ImageHeader"] = FunctionUtility.ImageToByteArrayWithFormat(fileItemPath, 380, 0.94, "JPG");

                //Image Item Detail
                string fileCompPath = FunctionUtility.RDDGetItemImage(rowParent["ItemCode"].ToString());
                rowResultParent["ImageFull"] = FunctionUtility.ImageToByteArrayWithFormat(fileCompPath, 380, 1.716, "JPG");

                //User
                rowResultParent["User"] = Shared.Utility.SharedObject.UserInfo.EmpName;

                dsResult.Tables["dtRoutingTicketHeader"].Rows.Add(rowResultParent);
              }
              foreach (DataRow rowChild in dsData.Tables[1].Rows)
              {
                DataRow rowResultChild = dsResult.Tables["dtWorkingStep"].NewRow();
                rowResultChild["WO"] = rowChild["WO"];
                rowResultChild["ItemCode"] = rowChild["ItemCode"];
                rowResultChild["Revision"] = rowChild["Revision"];
                rowResultChild["Step"] = rowChild["Step"];
                rowResultChild["ProcessCode"] = rowChild["ProcessCode"];
                rowResultChild["VNDescription"] = rowChild["VNDescription"];
                rowResultChild["ENDescription"] = rowChild["ENDescription"];
                rowResultChild["MachineGroup"] = rowChild["MachineGroup"];
                rowResultChild["ProcessTime"] = rowChild["ProcessTime"];
                rowResultChild["LeadTime"] = rowChild["LeadTime"];
                dsResult.Tables["dtWorkingStep"].Rows.Add(rowResultChild);
              }

            }
          }
        }
        catch { }
      }
      return dsResult;
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    //private void ugdCarcassInfo_AfterCellUpdate(object sender, CellEventArgs e)
    //{
    //  string colName = e.Cell.Column.ToString().ToLower();
    //  UltraGridRow row = e.Cell.Row;
    //  switch (colName)
    //  {
    //    case "selected":
    //      {
    //        if (e.Cell.Value.ToString() == "1")
    //        {
    //          int rowIndex = e.Cell.Row.Index;
    //          for (int i = 0; i < ugdCarcassInfo.Rows.Count; i++)
    //          {
    //            if (i != rowIndex)
    //            {
    //              ugdCarcassInfo.Rows[i].Cells["Selected"].Value = 0;
    //            }
    //          }
    //        }
    //      }
    //      break;
    //  }   
    //}
  }
}
