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
  public partial class viewBOM_04_003 : MainUserControl
  {
    private bool loadingData = false;
    private long woNo = long.MinValue;

    public viewBOM_04_003()
    {
      InitializeComponent();
    }

    #region function
    private void Search()
    {
      chkEditQty.Enabled = true;
      chkEditQty.Checked = false;
      chkSelectedAll.Checked = false;
      this.woNo = 0;
      groupBoxCarcassList.Text = string.Format("Carcass list of WO {0}", this.woNo);
      DBParameter[] inputParam = new DBParameter[1];
      if (txtCarcass.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@Carcass", DbType.String, 128, "%" + txtCarcass.Text.Trim() + "%");
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMReviewCarcassInfoList", inputParam);
      if (dtSource != null)
      {
        ultraGridCarcassInfo.DataSource = dtSource;
      }
    }

    private void LoadData()
    {

    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
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
            DBParameter[] param = new DBParameter[1];
            string carcassCode = ultraGridCarcassInfo.Rows[i].Cells["CarcassCode"].Value.ToString();
            param[0] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
            DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMViewRoutingTicketForTest", 600, param);
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
            DBParameter[] param = new DBParameter[1];
            string carcassCode = ultraGridCarcassInfo.Rows[i].Cells["CarcassCode"].Value.ToString();
            param[0] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
            DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMViewRoutingTicketVeneerForTest", param);
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
      DBParameter[] inputParam = new DBParameter[1];
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
        inputParam[0] = new DBParameter("@CarcassCodeList", DbType.AnsiString, 1024, carcassList);
      }
      DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMReviewCarcassCuttingList", inputParam);
      if (dsData != null)
      {
        //string fileLogoPath = string.Format("{0}{1}", System.Windows.Forms.Application.StartupPath, Shared.Utility.ConstantClass.PATH_LOGO);
        //byte[] bImageLogo = FunctionUtility.ImagePathToByteArray(fileLogoPath);

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

    private void viewBOM_04_003_Load(object sender, EventArgs e)
    {
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
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 80;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultraGridCarcassInfo_DoubleClick(object sender, EventArgs e)
    {
      //try
      //{
      //  UltraGridRow row = ultraGridCarcassInfo.Selected.Rows[0];
      //  string carcassCode = row.Cells["CarcassCode"].Value.ToString();
      //  int qty = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
      //  viewBOM_04_004 view = new viewBOM_04_004();
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
      int checkAll = (chkSelectedAll.Checked ? 1 : 0);
      for (int i = 0; i < ultraGridCarcassInfo.Rows.Count; i++)
      {
        ultraGridCarcassInfo.Rows[i].Cells["Selected"].Value = checkAll;
      }
    }

    private void btnPrintRouting_Click(object sender, EventArgs e)
    {
      btnPrintRouting.Enabled = false;
      DaiCo.Shared.View_Report report = null;
      ReportClass cpt = null;
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
      btnPrintRouting.Enabled = true;
    }

    private void btnPrintCuttingList_Click(object sender, EventArgs e)
    {
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
        report.ShowReport(Shared.Utility.ViewState.ModalWindow, true, FormWindowState.Maximized);
      }
    }

    private void ultraGridCarcassInfo_CellChange(object sender, CellEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare("Selected", column, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          chkSelectedAll.Checked = false;
        }
      }
    }

    private void btnRoutingVeneer_Click(object sender, EventArgs e)
    {
      btnRoutingVeneer.Enabled = false;
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
      btnRoutingVeneer.Enabled = true;
    }
  }
}
