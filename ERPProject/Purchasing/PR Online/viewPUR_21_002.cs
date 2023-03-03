/*
  Author      : 
  Date        : 02/07/2013
  Description : PR Online
 
  Update by   : 
  Update date : 29-Dec-2014
  Description Update: Check Over Budged Over
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using FormSerialisation;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_21_002 : MainUserControl
  {
    #region Field
    public long PRPid = long.MinValue;
    private bool flagHeadDepartment = false;
    private bool flagPur = false;
    private bool flagHideWOCarcass = true;
    private int status = 0;
    private DataTable dtWO;
    private DataTable dtCarcass;
    private DataTable dtItemCode;
    private bool flagViewPrice = false;

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    #endregion Field

    #region Init
    public viewPUR_21_002()
    {
      InitializeComponent();
    }

    private void viewPUR_21_002_Load(object sender, EventArgs e)
    {
      // Check Head Department
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT Code FROM VHRDDepartmentInfoForApprove WHERE Manager = {0}", SharedObject.UserInfo.UserPid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if ((dt != null && dt.Rows.Count > 0))
      {
        this.flagHeadDepartment = true;
        this.btnPerPurApproved.Visible = false;
      }

      if (this.btnPerPurApproved.Visible == true)
      {
        this.flagPur = true;
        this.btnPerPurApproved.Visible = false;
      }

      if (this.btnPerViewPrice.Visible == true)
      {
        this.flagViewPrice = true;
        this.btnPerViewPrice.Visible = false;
      }

      this.LoadInit();
      this.LoadData();
    }

    private void LoadInit()
    {
      this.LoadWO();
      this.LoadMaterial();
      this.LoadDropDownProjectCode();
      this.LoadDropDownUrgentLevel();
      this.LoadTypeOfRequest();
      this.LoadDrawing();
      this.LoadSample();
    }

    private void LoadDropDownProjectCode()
    {
      string commandText = string.Format(@" SELECT Pid Code, ProjectCode Name 
                                            FROM TblPURPRProjectCode 
                                            WHERE ISNULL(Status, 0) = 1 AND Department = '{0}'

                                            UNION 
                                            SELECT	PJ.Pid, PJ.ProjectCode
                                            FROM	TblPURPROnlineDetailInformation PRD
	                                            INNER JOIN  TblPURPRProjectCode PJ ON PJ.Pid = PRD.ProjectPid
                                            WHERE	PRD.PROnlinePid = {1}
                                            ", SharedObject.UserInfo.Department, PRPid);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      Utility.LoadUltraCombo(ultDDProjectCode, dtSource, "Code", "Name", false, "Code");

      ultDDProjectCode.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
    }

    private void LoadDropDownUrgentLevel()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7007 AND Code IN(1,3,5) AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultDDUrgentLevel, dtSource, "Code", "Name", false, "Code");
      ultDDUrgentLevel.DataSource = dtSource;
      ultDDUrgentLevel.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
    }




    private void LoadWO()
    {
      string commandText = "SELECT Pid WO FROM TblPLNWorkOrder";
      dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtWO == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultddWO, dtWO, "WO", "WO", false);      
    }

    private void LoadCarcass(long wo)
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT CarcassCode FROM TblPLNWorkOrderConfirmedDetails";
      commandText += " WHERE WorkOrderPid = " + wo;
      dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCarcass == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultddCarcass, dtCarcass, "CarcassCode", "CarcassCode", false);
      ultddCarcass.DisplayLayout.Bands[0].Columns["CarcassCode"].Width = 150;
    }
    private UltraDropDown LoadDDRevision(UltraDropDown ultdd, string ItemCode)
    {
      if (ultdd == null)
      {
        ultdd = new UltraDropDown();
      }
      string commandText = string.Format("SELECT Revision FROM TblBOMItemInfo where ItemCode = '{0}'", ItemCode);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraDropDown(ultdd, dtSource, "Revision", "Revision");
      return ultdd;
    }

    private void LoadItem(long wo)
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ItemCode FROM TblPLNWOInfoDetailGeneral";
      commandText += " WHERE WoInfoPID = " + wo;
      dtItemCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtItemCode == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultddItem, dtItemCode, "ItemCode", "ItemCode", false);
      ultddItem.DisplayLayout.Bands[0].Columns["ItemCode"].Width = 150;
     
    }

    private void LoadMaterial()
    {
      string commandText = string.Empty;
      if (PRPid != long.MinValue)
      {
        commandText += "SELECT MaterialCode, MaterialNameVn, MaterialNameEn, Unit, LeadTime FROM VBOMMaterials";
      }
      else
      {
        commandText += @"SELECT MaterialCode, MaterialNameVn, MaterialNameEn, Unit, LeadTime
                          FROM VBOMMaterials
                          WHERE Used = 1";
      }
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultddMaterial, dtSource, "MaterialCode", "MaterialCode", true);
      ultddMaterial.DisplayLayout.Bands[0].Columns["MaterialCode"].Width = 100;
      ultddMaterial.DisplayLayout.Bands[0].Columns["MaterialNameVn"].Width = 200;
      ultddMaterial.DisplayLayout.Bands[0].Columns["MaterialNameEn"].Width = 200;
      ultddMaterial.DisplayLayout.Bands[0].Columns["Unit"].Width = 50;
      ultddMaterial.DisplayLayout.Bands[0].Columns["LeadTime"].Width = 50;
    }

    private void LoadTypeOfRequest()
    {
      string commandText = "SELECT Code, Value Name FROM TblBOMCodeMaster WHERE [Group] = 7005 AND DeleteFlag = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultcbTypeOfRequest, dtSource, "Code", "Name", false, "Code");   
      ultcbTypeOfRequest.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void LoadDrawing()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'No' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Yes' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultddDrawing, dtSource, "ID", "Name", false, "ID");
      ultddDrawing.DataSource = dtSource;
    }

    private void LoadSample()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 ID, 'No' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Yes' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      Utility.LoadUltraCombo(ultddSample, dtSource, "ID", "Name", false, "ID");
    }

    #endregion Init

    #region Function
    public void Search()
    {
      // Check Head Department
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT Code FROM VHRDDepartmentInfo WHERE Manager = {0}", SharedObject.UserInfo.UserPid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if ((dt != null && dt.Rows.Count > 0))
      {
        this.flagHeadDepartment = true;
        this.btnPerPurApproved.Visible = false;
      }

      if (this.btnPerPurApproved.Visible == true)
      {
        this.flagPur = true;
        this.btnPerPurApproved.Visible = false;
      }

      this.LoadInit();
      this.LoadData();
    }

    private void LoadData()
    {
      chkConfirm.Enabled = true;
      btnSave.Enabled = true;
      btnReturn.Enabled = true;
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PRPid", DbType.Int64, this.PRPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURPROnlineInfomation_Select", 300, inputParam);
      if (dsSource != null)
      {
        // 1: Load Master
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          txtPRNo.Text = row["PROnlineNo"].ToString();
          txtCreateDate.Text = row["CreateDate"].ToString();
          txtDepartment.Text = row["Department"].ToString();
          txtRequester.Text = row["Requester"].ToString();
          txtRemark.Text = row["Remark"].ToString();
          txtPurposeOfPR.Text = row["PurposeOfRequisition"].ToString();
          if (DBConvert.ParseInt(row["TypeOfRequest"].ToString()) > 0)
          {
            ultcbTypeOfRequest.Value = DBConvert.ParseInt(row["TypeOfRequest"].ToString());
          }
          labStatus.Text = row["StatusName"].ToString();
          this.status = DBConvert.ParseInt(row["Status"].ToString());
        }
        else
        {
          DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPURPROnlineGetNewPRNo('PROL') NewPRNo");
          if ((dtCode != null) && (dtCode.Rows.Count == 1))
          {
            txtPRNo.Text = dtCode.Rows[0]["NewPRNo"].ToString();
            txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
            txtRequester.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
            txtDepartment.Text = SharedObject.UserInfo.Department;
            labStatus.Text = "New";
          }
        }
        // 2: Load Detail
        //ultData.DataSource = dsSource.Tables[1];

        DataSet dsMain = this.CreateDataSet();
        dsMain.Tables["dtParent"].Merge(dsSource.Tables[1]);
        dsMain.Tables["dtChild"].Merge(dsSource.Tables[3]);
        ultData.DataSource = dsMain;

        // 3: Load File UpLoad
        ultUploadFile.DataSource = dsSource.Tables[2];
        // 4: Set Status Control
        this.SetStatusControl();
      }
    }

    private void SetStatusControl()
    {
      txtPurposeOfPR.ReadOnly = false;
      txtRemark.ReadOnly = false;
      ultcbTypeOfRequest.ReadOnly = false;
      chkConfirm.Enabled = true;
      chkConfirm.Checked = false;
      btnSave.Enabled = true;
      btnSave.Enabled = true;
      btnDelete.Enabled = true;
      btnImport.Enabled = true;
      btnBrown.Enabled = true;
      btnGettemplate.Enabled = true;

      // Requester
      if (this.flagHeadDepartment == false)
      {
        if (this.status == 0)
        {
          btnReturn.Enabled = false;
        }
        else if (this.status >= 1)
        {
          txtPurposeOfPR.ReadOnly = true;
          txtRemark.ReadOnly = true;
          ultcbTypeOfRequest.ReadOnly = true;
          chkConfirm.Enabled = false;
          chkConfirm.Checked = true;
          btnSave.Enabled = false;
          btnReturn.Enabled = false;
          btnDelete.Enabled = false;
          btnImport.Enabled = false;
          btnBrown.Enabled = false;
          btnGettemplate.Enabled = false;
        }
      }
      // HeadDepartment
      if (this.flagHeadDepartment == true)
      {
        if (this.status == 0)
        {
          if (this.PRPid != long.MinValue)
          {
            string commandText = string.Empty;
            commandText = string.Format(@"SELECT CreateBy FROM TblPURPROnlineInformation WHERE PID = {0}", this.PRPid);
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            int createBy = DBConvert.ParseInt(dt.Rows[0][0]);
            if (SharedObject.UserInfo.UserPid != createBy)
            {
              txtPurposeOfPR.ReadOnly = true;
              txtRemark.ReadOnly = true;
              ultcbTypeOfRequest.ReadOnly = true;
              chkConfirm.Enabled = false;
              chkConfirm.Checked = false;
              btnSave.Enabled = false;
              btnReturn.Enabled = false;
              btnDelete.Enabled = false;
              btnImport.Enabled = false;
              btnBrown.Enabled = false;
              btnGettemplate.Enabled = false;
            }
            else
            {
              btnReturn.Enabled = false;
            }
          }
          else
          {
            btnReturn.Enabled = false;
          }
        }
        else if (this.status == 1)
        {
          txtPurposeOfPR.ReadOnly = true;
          txtRemark.ReadOnly = true;
          ultcbTypeOfRequest.ReadOnly = true;
          chkConfirm.Enabled = false;
          chkConfirm.Checked = true;
          btnSave.Enabled = true;
          btnSave.Text = "Approve";
          btnReturn.Enabled = true;
          btnDelete.Enabled = false;
          btnImport.Enabled = false;
          btnBrown.Enabled = false;
          btnGettemplate.Enabled = false;
        }
        else if (this.status >= 2)
        {
          txtPurposeOfPR.ReadOnly = true;
          txtRemark.ReadOnly = true;
          ultcbTypeOfRequest.ReadOnly = true;
          chkConfirm.Enabled = false;
          chkConfirm.Checked = true;
          btnSave.Enabled = false;
          btnReturn.Enabled = false;
          btnDelete.Enabled = false;
          btnImport.Enabled = false;
          btnBrown.Enabled = false;
          btnGettemplate.Enabled = false;
        }
      }

      // Upload File
      if (this.status > 0 && ultUploadFile.Rows.Count == 0)
      {
        this.grbUploadFile.Visible = false;
      }
      else
      {
        this.grbUploadFile.Visible = true;
        this.chkUploadFileHide.Checked = true;
      }

      //for (int i = 0; i < ultData.Rows.Count; i++)
      //{
      //  UltraGridRow row = ultData.Rows[i];
      //  if (DBConvert.ParseInt(row.Cells["AlertRequestDate"].Value.ToString()) == 1)
      //  {
      //    row.Appearance.ForeColor = Color.Red;
      //    row.Appearance.FontData.Bold = DefaultableBoolean.True;
      //    row.Appearance.BackColor = Color.Yellow;
      //  }
      //}
    }

    /// <summary>
    /// Create DataSet
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("PID", typeof(System.Int64));      
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("ItemQty", typeof(System.Int32));
      taParent.Columns.Add("PackingDeadline", typeof(System.DateTime));
      taParent.Columns.Add("WORemark", typeof(System.String));
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("MaterialNameEn", typeof(System.String));
      taParent.Columns.Add("MaterialNameVn", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("LeadTime", typeof(System.Double));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("QtyRequest", typeof(System.Double));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("RequestDate", typeof(System.DateTime));
      taParent.Columns.Add("Urgent", typeof(System.Int32));
      taParent.Columns.Add("ProjectCode", typeof(System.Int32));
      taParent.Columns.Add("ExpectedBrand", typeof(System.String));
      taParent.Columns.Add("Drawing", typeof(System.Int32));
      taParent.Columns.Add("Sample", typeof(System.Int32));
      taParent.Columns.Add("PurRemark", typeof(System.String));
      taParent.Columns.Add("PurCancel", typeof(System.Int32));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("PriceVN", typeof(System.Double));
      taChild.Columns.Add("Column6", typeof(System.Double));
      taChild.Columns.Add("Column5", typeof(System.Double));
      taChild.Columns.Add("Column4", typeof(System.Double));
      taChild.Columns.Add("Column3", typeof(System.Double));
      taChild.Columns.Add("Column2", typeof(System.Double));
      taChild.Columns.Add("Column1", typeof(System.Double));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["MaterialCode"], taChild.Columns["MaterialCode"], false));
      return ds;
    }

    /// <summary>
    /// Alert Create PR + Leadtime > Request Date
    /// </summary>
    private bool AlertRequestDate()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["AlertRequestDate"].Value.ToString()) == 1)
        {
          if (WindowUtinity.ShowMessageConfirmFromText("Required Delivery Date < CreateDate PR + Leadtime Material") == DialogResult.Yes)
          {
            return true;
          }
          else
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check Data
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckData(out String message)
    {
      message = string.Empty;
      long projectPid = long.MinValue;

      // Check info
      if (ultcbTypeOfRequest.Value == null || ultcbTypeOfRequest.SelectedRow == null)
      {
        message = "Type Of Request";
        return false;
      }

      //// Check Budget
      //if (this.flagHeadDepartment == true || this.flagPur == true)
      //{
      //  DBParameter[] input = new DBParameter[1];
      //  input[0] = new DBParameter("@PRNo", DbType.String, txtPRNo.Text);
      //  DBParameter[] output = new DBParameter[2];
      //  output[0] = new DBParameter("@Result", DbType.Int64, 0);
      //  output[1] = new DBParameter("@BudgetCode", DbType.String, 128, string.Empty);
      //  DataBaseAccess.ExecuteStoreProcedure("spPURPRCheckingBudgetOver", input, output);
      //  long result = DBConvert.ParseLong(output[0].Value.ToString());
      //  string budgetCode = output[1].Value.ToString();
      //  if (result == 0)
      //  {
      //    message = "Budget Requested is Over";
      //    if (WindowUtinity.ShowMessageConfirmFromText("Budget Request is Over, Do You want to show detail ?") == DialogResult.Yes)
      //    {
      //      bool a = true;
      //      a = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = '" + budgetCode + "' WHERE [Group] = 16077 AND Value = 1", null);
      //      Process.Start(DataBaseAccess.ExecuteScalarCommandText("SELECT [Description] FROM TblBOMCodeMaster WHERE [Group] = 16077 AND Value = 2", null).ToString());
      //      Thread.Sleep(2000);
      //      a = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = NULL WHERE [Group] = 16077 AND Value = 1", null);
      //      return false; 
      //    }
      //    else
      //    {
      //      return false;
      //    }
      //  }
      //}

      // Check Detail
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        string commandText = string.Empty;

        if (i == 0)
        {
          projectPid = DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString());
        }
        if (i > 0 && projectPid != DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString()))
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Project Code");
          return false;
        }

        // Check WO
        if (row.Cells["WO"].Text.Trim().Length > 0)
        {
          commandText = " SELECT Pid WO FROM TblPLNWorkOrder ";
          commandText += " WHERE Pid = " + DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) + "";
          DataTable dtCheckWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheckWO == null || dtCheckWO.Rows.Count == 0)
          {
            message = "WO";
            return false;
          }
        }

        // Check CarcassCode
        if (row.Cells["WO"].Text.Trim().Length > 0 && row.Cells["CarcassCode"].Text.Trim().Length > 0)
        {
          commandText = " SELECT DISTINCT CarcassCode FROM TblPLNWorkOrderConfirmedDetails ";
          commandText += " WHERE WorkOrderPid = " + DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) + "";
          commandText += " AND CarcassCode = '" + row.Cells["CarcassCode"].Value.ToString() + "'";
          DataTable dtCheckCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheckCarcass == null || dtCheckCarcass.Rows.Count == 0)
          {
            message = "CarcassCode";
            return false;
          }
        }

        // Check ItemCode
        if (row.Cells["WO"].Text.Trim().Length > 0 && row.Cells["ItemCode"].Text.Trim().Length > 0)
        {
          commandText = " SELECT DISTINCT ItemCode FROM TblPLNWOInfoDetailGeneral ";
          commandText += " WHERE WoInfoPID = " + DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) + "";
          commandText += " AND ItemCode = '" + row.Cells["ItemCode"].Value.ToString() + "'";
          DataTable dtCheckItemCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheckItemCode == null || dtCheckItemCode.Rows.Count == 0)
          {
            message = "ItemCode";
            return false;
          }
        }
        //// Check Revision

        //if (row.Cells["ItemCode"].Text.Trim().Length > 0 && row.Cells["Revision"].Text.Trim().Length > 0)
        //{
        //  commandText = string.Format(@" SELECT DISTINCT ItemCode FROM TblBOMItemInfo WHERE ItemCode = '{0}' AND Revision = {1}", row.Cells["ItemCode"].Value.ToString(), DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        //  DataTable dtCheckItemCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
        //  if (dtCheckItemCode == null || dtCheckItemCode.Rows.Count == 0)
        //  {
        //    message = "Revision";
        //    return false;
        //  }
        //}
        //else
        //{
        //  if (row.Cells["ItemCode"].Text.Trim().Length > 0)
        //  {
        //    if (row.Cells["Revision"].Text.Trim().Length == 0)
        //    {
        //      message = "Revision";
        //      return false;
        //    }
        //  }
        //}

        // Check MaterialCode
        if (row.Cells["MaterialCode"].Text.ToString().Length == 0)
        {
          message = row.Cells["MaterialCode"].Column.Header.Caption;
          return false;
        }
        else
        {
          commandText = "SELECT MaterialCode FROM VBOMMaterials WHERE MaterialCode = '" + row.Cells["MaterialCode"].Value.ToString() + "'";
          DataTable dtMain = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtMain == null || dtMain.Rows.Count == 0)
          {
            message = row.Cells["MaterialCode"].Column.Header.Caption;
            return false;
          }
        }
        // Check Qty
        if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) <= 0)
        {
          message = row.Cells["Qty"].Column.Header.Caption;
          return false;
        }
        // Check RequestDate
        if (row.Cells["RequestDate"].Value.ToString().Length == 0)
        {
          message = row.Cells["RequestDate"].Column.Header.Caption;
          return false;
        }

        commandText = "SELECT CONVERT(date, GETDATE())";
        DataTable dtDate = DataBaseAccess.SearchCommandTextDataTable(commandText);

        DateTime requestDate = DBConvert.ParseDateTime(row.Cells["RequestDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        DateTime curenDate = DBConvert.ParseDateTime(dtDate.Rows[0][0].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        if (requestDate < curenDate)
        {
          message = "Required Delivery Date < Current Date";
          return false;
        }

        // Check Urgent
        if (row.Cells["Urgent"].Value.ToString().Length == 0)
        {
          message = row.Cells["Urgent"].Column.Header.Caption;
          return false;
        }
      }
      return true;
    }

    private bool SaveData()
    {
      bool success = this.SaveMaster();
      if (success)
      {
        success = this.SaveDetail();
        if (success)
        {
          success = this.SaveCaptureMaterialNeed();
          if (success)
          {
            success = this.SaveUpLoadFile();
            if(success)
            {
              string checkStatus = "SELECT [Status] FROM TblPURPROnlineInformation WHERE PID = " + this.PRPid;
              DataTable dt = DataBaseAccess.SearchCommandTextDataTable(checkStatus);
              if(DBConvert.ParseInt(dt.Rows[0]["Status"].ToString()) == 2)
              {
                success = this.SaveMappingPR();            
              }
              return success;
            } 
            else
            {
              return false;
            }  
            
          }
          else
          {
            return false;
          }
        }
        else
        {
          return false;
        }
      }
      else
      {
        return false;
      }
    }

    private bool SaveMaster()
    {
      DBParameter[] input = new DBParameter[11];
      if (this.PRPid != long.MinValue)
      {
        input[0] = new DBParameter("@PID", DbType.Int64, this.PRPid);
      }
      input[1] = new DBParameter("@RequestBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      input[2] = new DBParameter("@Department", DbType.String, SharedObject.UserInfo.Department);
      if (chkConfirm.Checked)
      {
        if (this.flagHeadDepartment == true || this.flagPur == true)
        {
          input[3] = new DBParameter("@Status", DbType.Int32, 2);
        }
        else
        {
          input[3] = new DBParameter("@Status", DbType.Int32, 1);
        }
      }
      else
      {
        if (this.flagHeadDepartment == true && this.status == 1)
        {
          input[3] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          input[3] = new DBParameter("@Status", DbType.Int32, 0);
        }
      }
      if (this.flagHeadDepartment == true)
      {
        input[4] = new DBParameter("@HeadDepartmentApproved", DbType.Int32, SharedObject.UserInfo.UserPid);
      }

      if (this.flagPur == true)
      {
        string commandText = string.Empty;
        commandText += " SELECT DEP.Manager ";
        commandText += " FROM VHRNhanVien NV ";
        commandText += " INNER JOIN VHRDDepartment DEP ON NV.Department = DEP.Department ";
        commandText += " WHERE ID_NhanVien = " + SharedObject.UserInfo.UserPid;

        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count == 1)
        {
          input[4] = new DBParameter("@HeadDepartmentApproved", DbType.Int32, DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()));
        }
      }

      if (ultcbTypeOfRequest.Value != null)
      {
        input[5] = new DBParameter("@TypeOfRequest", DbType.Int32, DBConvert.ParseInt(ultcbTypeOfRequest.Value.ToString()));
      }
      if (txtPurposeOfPR.Text.Trim().Length > 0)
      {
        input[6] = new DBParameter("@PurposeOfRequisition", DbType.String, txtPurposeOfPR.Text);
      }
      if (txtRemark.Text.Trim().Length > 0)
      {
        input[7] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
      }
      input[8] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      input[9] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      input[10] = new DBParameter("@PROnlineNo", DbType.String, "PROL");

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPURPROnlineInfomation_Edit", input, output);
      this.PRPid = DBConvert.ParseLong(output[0].Value.ToString());
      if (this.PRPid <= 0)
      {
        return false;
      }
      return true;
    }

    private bool SaveDetail()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        DBParameter[] input = new DBParameter[20];
        if (DBConvert.ParseLong(row.Cells["PID"].Value.ToString()) != long.MinValue)
        {
          input[0] = new DBParameter("@PID", DbType.Int64, DBConvert.ParseLong(row.Cells["PID"].Value.ToString()));
        }
        input[1] = new DBParameter("@PROnlinePid", DbType.Int64, this.PRPid);
        if (DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) != int.MinValue)
        {
          input[2] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
        }
        if (row.Cells["CarcassCode"].Value.ToString().Length > 0)
        {
          input[3] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
        }
        if (row.Cells["ItemCode"].Value.ToString().Length > 0)
        {
          input[4] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
        }
        input[5] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MaterialCode"].Value.ToString());
        input[6] = new DBParameter("@Status", DbType.Int32, 0);
        input[7] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()));
        input[8] = new DBParameter("@RequestDate", DbType.DateTime, DBConvert.ParseDateTime(row.Cells["RequestDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
        if (DBConvert.ParseInt(row.Cells["Urgent"].Value.ToString()) != int.MinValue)
        {
          input[9] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseInt(row.Cells["Urgent"].Value.ToString()));
        }

        if (DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString()) != long.MinValue)
        {
          input[10] = new DBParameter("@ProjectCode", DbType.Int64, DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString()));
        }

        if (row.Cells["Remark"].Value.ToString().Trim().Length > 0)
        {
          input[11] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
        }

        if (row.Cells["PurRemark"].Value.ToString().Trim().Length > 0)
        {
          input[12] = new DBParameter("@PurRemark", DbType.String, row.Cells["PurRemark"].Value.ToString());
        }

        if (row.Cells["ExpectedBrand"].Value.ToString().Trim().Length > 0)
        {
          input[13] = new DBParameter("@ExpectedBrand", DbType.String, row.Cells["ExpectedBrand"].Value.ToString());
        }

        if (DBConvert.ParseInt(row.Cells["Drawing"].Value.ToString()) != int.MinValue)
        {
          input[14] = new DBParameter("@Drawing", DbType.Int32, DBConvert.ParseInt(row.Cells["Drawing"].Value.ToString()));
        }

        if (DBConvert.ParseInt(row.Cells["Sample"].Value.ToString()) != int.MinValue)
        {
          input[15] = new DBParameter("@Sample", DbType.Int32, DBConvert.ParseInt(row.Cells["Sample"].Value.ToString()));
        }

        if (DBConvert.ParseInt(row.Cells["ItemQty"].Value.ToString()) != int.MinValue)
        {
          input[16] = new DBParameter("@ItemQty", DbType.Int32, DBConvert.ParseInt(row.Cells["ItemQty"].Value.ToString()));
        }
        if (DBConvert.ParseDateTime(row.Cells["PackingDeadline"].Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
        {
          input[17] = new DBParameter("@PackingDeadline", DbType.DateTime, row.Cells["PackingDeadline"].Value.ToString());
        }
        if (row.Cells["WORemark"].Value.ToString().Length > 0)
        {
          input[18] = new DBParameter("@WORemark", DbType.String, row.Cells["WORemark"].Value.ToString());
        }
        if (DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()) != int.MinValue)
        {
          input[19] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
        }
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPURPROnlineDetailInfomation_Edit", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    private bool SaveUpLoadFile()
    {
      for (int i = 0; i < ultUploadFile.Rows.Count; i++)
      {
        UltraGridRow row = ultUploadFile.Rows[i];
        string location = string.Empty;
        string fileName = string.Empty;
        // Get FileName
        fileName = row.Cells["FileName"].Value.ToString();
        fileName = System.IO.Path.GetFileNameWithoutExtension(fileName).ToString() + DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd"))
                            + DBConvert.ParseString(DateTime.Now.Ticks)
                            + System.IO.Path.GetExtension(fileName);

        // Get Location
        string commandText = string.Empty;
        commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 3);
        DataTable dtLocation = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtLocation != null && dtLocation.Rows.Count > 0)
        {
          location = System.IO.Path.Combine(dtLocation.Rows[0]["Value"].ToString(), fileName);
        }

        // Copy File
        System.IO.File.Copy(row.Cells["Location"].Value.ToString(), location, true);

        // Input
        DBParameter[] input = new DBParameter[5];
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
        }
        // PROnline Pid
        input[1] = new DBParameter("@PROnlinePid", DbType.Int64, this.PRPid);
        // FileName
        input[2] = new DBParameter("@FileName", DbType.String, row.Cells["FileName"].Value.ToString());
        // Location
        input[3] = new DBParameter("@Location", DbType.String, location);
        // Remark
        if (row.Cells["Remark"].Text.Trim().Length > 0)
        {
          input[4] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
        }
        // Output
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        // Excecute
        DataBaseAccess.ExecuteStoreProcedure("spPURPROnlineUploadFile_Edit", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    private bool SaveCaptureMaterialNeed()
    {
      if (this.flagHeadDepartment == false && chkConfirm.Checked)
      {
        DBParameter[] input = new DBParameter[2];
        input[0] = new DBParameter("@PROnlinePid", DbType.Int64, this.PRPid);
        input[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, 0);
        DataBaseAccess.ExecuteStoreProcedure("spPLNMaterialFollowUp_Insert", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Mapping PR
    /// </summary>
    /// <returns></returns>
    private bool SaveMappingPR()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PRPid", DbType.Int64, this.PRPid);
      input[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] ouput = new DBParameter[1];
      ouput[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPURPROnlineMappingPRInfomation_Insert", input, ouput);
      long result = DBConvert.ParseLong(ouput[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }


    private void SendEmail()
    {
      if (chkConfirm.Checked)
      {
        DBParameter[] input = new DBParameter[6];
        input[0] = new DBParameter("@PROnlinePid", DbType.Int64, this.PRPid);
        if (this.flagHeadDepartment == false)
        {
          input[1] = new DBParameter("@Type", DbType.Int32, 1);
        }
        else
        {
          input[1] = new DBParameter("@Type", DbType.Int32, 2);
        }
        MainUserControl c = new MainUserControl();
        if (this.flagHeadDepartment == true)
        {
          viewPUR_21_006 viewPURConfirm = new viewPUR_21_006();
          viewPURConfirm.PRPid = this.PRPid;
          c = viewPURConfirm;
        }
        else
        {
          c = (MainUserControl)this;
        }
        string strTypeObject = c.GetType().FullName + "," + c.GetType().Namespace.Split('.')[1];
        string strTitle = SharedObject.tabContent.TabPages[SharedObject.tabContent.SelectedIndex].Text;
        string strFileName = c.Name + ".xml";
        MemoryStream stream = new MemoryStream();
        stream = FormSerialisor.Serialise(c);
        byte[] file;
        file = stream.ToArray();
        stream.Close();

        input[2] = new DBParameter("@File", DbType.Binary, file.Length, file);
        input[3] = new DBParameter("@TypeObject", DbType.AnsiString, 500, strTypeObject);
        input[4] = new DBParameter("@Title", DbType.AnsiString, 300, strTitle);
        input[5] = new DBParameter("@FileName", DbType.String, 300, strFileName);

        DataBaseAccess.ExecuteStoreProcedure("spPURSendEmailWhenConfirmedPROnline", input);
      }
    }

    private void ImportExcel()
    {
      if (this.txtImportExcel.Text.Trim().Length == 0)
      {
        return;
      }
      try
      {
        // Get Data Table From Excel
        DataTable dtSource = new DataTable();   
        dtSource = FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B5:H1500]").Tables[0];       
        // Check Data Tabel From Excel
        if (dtSource == null)
        {
          return;
        }
        // Data Table From UltraGrird
        DataSet dsMain = (DataSet)ultData.DataSource;
        DataTable dtMain = dsMain.Tables[0];
        foreach (DataRow drSource in dtSource.Rows)
        {
          if (drSource["MaterialCode"].ToString().Trim().Length > 0)
          {
            DataRow row = dtMain.NewRow();
            // MaterialCode
            string commandText = "SELECT MaterialCode, MaterialNameEn, MaterialNameVn, Unit FROM VBOMMaterials WHERE MaterialCode = '" + drSource["MaterialCode"].ToString() + "'";
            DataTable dtMaterialCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtMaterialCode != null && dtMaterialCode.Rows.Count > 0)
            {
              row["MaterialCode"] = dtMaterialCode.Rows[0]["MaterialCode"].ToString();
              row["MaterialNameEn"] = dtMaterialCode.Rows[0]["MaterialNameEn"];
              row["MaterialNameVn"] = dtMaterialCode.Rows[0]["MaterialNameVn"].ToString();
              row["Unit"] = dtMaterialCode.Rows[0]["Unit"].ToString();
            }
            else
            {
              row["MaterialCode"] = drSource["MaterialCode"].ToString();
            }
            // Qty
            if (DBConvert.ParseDouble(drSource["Qty"].ToString()) != double.MinValue)
            {
              row["Qty"] = DBConvert.ParseDouble(drSource["Qty"].ToString());
            }
            else
            {
              row["Qty"] = DBNull.Value;
            }
            // RequestDate
            if (drSource["RequestDate"].ToString().Length > 0)
            {
              row["RequestDate"] = DBConvert.ParseDateTime(drSource["RequestDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            else
            {
              row["RequestDate"] = DBNull.Value;
            }
            // Urgent
            if (DBConvert.ParseInt(drSource["Urgent"].ToString()) != int.MinValue)
            {
              row["Urgent"] = DBConvert.ParseInt(drSource["Urgent"].ToString());
            }
            else
            {
              row["Urgent"] = DBNull.Value;
            }
            //// Drawing
            //if (DBConvert.ParseInt(drSource["Drawing"].ToString()) == 1)
            //{
            //  row["Drawing"] = 1;
            //}
            //else
            //{
            //  row["Drawing"] = 0;
            //}
            //// Sample
            //if (DBConvert.ParseInt(drSource["Sample"].ToString()) == 1)
            //{
            //  row["Sample"] = 1;
            //}
            //else
            //{
            //  row["Sample"] = 0;
            //}
            // Purpose Of Requisition
            if (drSource["PurposeOfRequisition"].ToString().Length > 0)
            {
              row["Remark"] = drSource["PurposeOfRequisition"].ToString();
            }
            else
            {
              row["Remark"] = DBNull.Value;
            }
            if (this.flagHideWOCarcass)
            {
              // WO
              if (DBConvert.ParseInt(drSource["WO"].ToString()) > 0)
              {
                row["WO"] = DBConvert.ParseInt(drSource["WO"].ToString());
                //// CarcassCode
                //if (drSource["CarcassCode"].ToString().Trim().Length > 0)
                //{
                //  row["CarcassCode"] = drSource["CarcassCode"].ToString();
                //}
                //else
                //{
                //  row["CarcassCode"] = ""; ;
                //}
                // ItemCode
                if (drSource["ItemCode"].ToString().Trim().Length > 0)
                {
                  row["ItemCode"] = drSource["ItemCode"].ToString();
                }
                else
                {
                  row["ItemCode"] = "";
                }
                ////Revision
                //if (DBConvert.ParseInt(drSource["Revision"].ToString()) != int.MinValue)
                //{
                //  row["Revision"] = DBConvert.ParseInt(drSource["Revision"].ToString());
                //}
                //else
                //{
                //  row["Revision"] = DBNull.Value;
                //}
                ////Add ItemQty
                //if (DBConvert.ParseInt(drSource["ItemQty"].ToString()) != int.MinValue)
                //{
                //  row["ItemQty"] = DBConvert.ParseInt(drSource["ItemQty"].ToString());
                //}
                //else
                //{
                //  row["ItemQty"] = DBNull.Value;
                //}
                ////PackingDeadline
                //if (DBConvert.ParseDateTime(drSource["PackingDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                //{
                //  row["PackingDeadline"] = DBConvert.ParseDateTime(drSource["PackingDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
                //}
                //else
                //{
                //  row["PackingDeadline"] = DBNull.Value;
                //}
                ////WoRemark
                //if (drSource["WoRemark"].ToString().Trim().Length > 0)
                //{
                //  row["WoRemark"] = drSource["WoRemark"].ToString();
                //}
                //else
                //{
                //  row["WoRemark"] = "";
                //}
              }
            }
            // Add Row
            dtMain.Rows.Add(row);
          }
        }
        // Gan DataSource
        ultData.DataSource = dsMain;
        if (this.flagHideWOCarcass)
        {
          // Check WO, Carcass, ItemCode
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            UltraGridRow row = ultData.Rows[i];
            string commandText = string.Empty;
            if (DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) > 0)
            {
              DataTable dtCheck = new DataTable();
              int wo = DBConvert.ParseInt(row.Cells["WO"].Value.ToString());
              string carcassCode = row.Cells["CarcassCode"].Value.ToString();
              string itemCode = row.Cells["ItemCode"].Value.ToString();
              int Revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
              // Check WO
              commandText = " SELECT Pid FROM TblPLNWorkOrder WHERE Pid " + wo;
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck != null && dtCheck.Rows.Count == 0)
              {
                row.Cells["WO"].Appearance.BackColor = Color.Yellow;
              }
              // Check Carcass
              commandText = " SELECT CarcassCode FROM TblPLNWorkOrderConfirmedDetails ";
              commandText += " WHERE WorkOrderPid = " + wo + " AND CarcassCode = '" + carcassCode + "'";
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck != null && dtCheck.Rows.Count == 0)
              {
                row.Cells["CarcassCode"].Appearance.BackColor = Color.Yellow;
              }
              // Check Item              
              commandText = " SELECT ItemCode FROM TblPLNWOInfoDetailGeneral ";
              commandText += " WHERE WoInfoPID = " + wo + " AND ItemCode = '" + itemCode + "'";
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck != null && dtCheck.Rows.Count == 0)
              {
                row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
              }
              //Check Revision
              commandText = string.Format(@"SELECT DISTINCT ItemCode FROM TblBOMItemInfo WHERE ItemCode = '{0}' AND Revision = {1}", itemCode, Revision);
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck != null && dtCheck.Rows.Count == 0)
              {
                row.Cells["Revision"].Appearance.BackColor = Color.Yellow;
              }
            }
          }
        }
        // Enable button Import
        btnImport.Enabled = false;
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }
    #endregion Function

    #region Event
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Budget
      if (this.flagHeadDepartment == true || this.flagPur == true)
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@PRNo", DbType.String, txtPRNo.Text);
        DBParameter[] output = new DBParameter[2];
        output[0] = new DBParameter("@Result", DbType.Int64, 0);
        output[1] = new DBParameter("@BudgetCode", DbType.String, 128, string.Empty);
        DataBaseAccess.ExecuteStoreProcedure("spPURPRCheckingBudgetOver", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        string budgetCode = output[1].Value.ToString();
        if (result == 0)
        {
          if (WindowUtinity.ShowMessageConfirmFromText("Total Budget Requested > Budget Amount, Do You want to show detail ?") == DialogResult.Yes)
          {
            bool a = true;
            a = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = '" + budgetCode + "' WHERE [Group] = 16077 AND Value = 1", null);
            Process.Start(DataBaseAccess.ExecuteScalarCommandText("SELECT [Description] FROM TblBOMCodeMaster WHERE [Group] = 16077 AND Value = 2", null).ToString());
            Thread.Sleep(2000);
            a = DataBaseAccess.ExecuteCommandText("UPDATE TblBOMCodeMaster SET Description = NULL WHERE [Group] = 16077 AND Value = 1", null);
            return;
          }
          else
          {
            return;
          }
        }
      }
      // End

      // 1: Check Valid
      bool success = this.CheckData(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Alert Request Date
      success = this.AlertRequestDate();
      if (!success)
      {
        return;
      }

      // 2: Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // 3: SendEmail
      this.SendEmail();
      // 4: Load Data
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      Utility.InitLayout_UltraGrid(ultData);
      e.Layout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;      

      e.Layout.Bands[0].Columns["AlertRequestDate"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemQty"].Hidden = true;
      e.Layout.Bands[0].Columns["PackingDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["WORemark"].Hidden = true;
      e.Layout.Bands[0].Columns["Drawing"].Hidden = true;
      e.Layout.Bands[0].Columns["Sample"].Hidden = true;
      e.Layout.Bands[0].Columns["PurRemark"].Hidden = true;
      e.Layout.Bands[0].Columns["PurCancel"].Hidden = true;
      e.Layout.Bands[0].Columns["Leadtime"].Hidden = true;
      e.Layout.Bands[0].Columns["MATControl"].Hidden = true;
      e.Layout.Bands[0].Columns["GroupIncharge"].Hidden = true;
      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["QtyRequest"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialNameEn"].Hidden = true;
      e.Layout.Bands[0].Columns["ProjectCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ExpectedBrand"].Hidden = true;

      e.Layout.Bands[0].ColHeaderLines = 3;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm\n(*)";
      e.Layout.Bands[0].Columns["MaterialNameEn"].Header.Caption = "Tên SP TA";
      e.Layout.Bands[0].Columns["MaterialNameVn"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Ngày yêu\ncầu (*)";
      e.Layout.Bands[0].Columns["QtyRequest"].Header.Caption = "Số lượng\nyêu cầu ban đầu";   
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Ghi chú";
      e.Layout.Bands[0].Columns["Urgent"].Header.Caption = "Khẩn cấp\n(*)";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng\n(*)";
      e.Layout.Bands[0].Columns["Status"].Header.Caption = "Trạng thái";
      //e.Layout.Bands[0].Columns["LastPOSupplier"].Header.Caption = "Nhà cung cấp\ngần nhất";
      //e.Layout.Bands[0].Columns["LastPrice"].Header.Caption = "Giá\ngần nhất";
      e.Layout.Bands[0].Columns["WO"].Header.Caption = "LSX";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Mã hàng";

      e.Layout.Bands[0].Columns["WO"].ValueList = ultddWO;
      e.Layout.Bands[0].Columns["CarcassCode"].ValueList = ultddCarcass;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultddItem;
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ultddMaterial;
      e.Layout.Bands[0].Columns["Urgent"].ValueList = ultDDUrgentLevel;
      e.Layout.Bands[0].Columns["ProjectCode"].ValueList = ultDDProjectCode;
      e.Layout.Bands[0].Columns["Revision"].ValueList = ultDDRevision;

      e.Layout.Bands[0].Columns["Drawing"].ValueList = ultddDrawing;
      e.Layout.Bands[0].Columns["Sample"].ValueList = ultddSample;
      e.Layout.Bands[0].Columns["MaterialNameEn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialNameVn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyRequest"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LeadTime"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PURRemark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PURCancel"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PurCancel"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["PurCancel"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["RequestDate"].Format = ConstantClass.FORMAT_DATETIME;      
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LeadTime"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyRequest"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["MaterialNameVn"].Width = 150;
      e.Layout.Bands[0].Columns["WO"].Width = 50;      
      e.Layout.Bands[0].Columns["CarcassCode"].Width = 100;      
      e.Layout.Bands[0].Columns["ItemCode"].Width = 80;      
      e.Layout.Bands[0].Columns["MaterialCode"].Width = 100;      
      e.Layout.Bands[0].Columns["Unit"].Width = 50;
      e.Layout.Bands[0].Columns["Qty"].Width = 70;      
      e.Layout.Bands[0].Columns["RequestDate"].Width = 80;      
      e.Layout.Bands[0].Columns["PURCancel"].Width = 60;
      e.Layout.Bands[0].Columns["Status"].Width = 80;
      e.Layout.Bands[0].Columns["Urgent"].Width = 70;
      e.Layout.Bands[0].Columns["ProjectCode"].Width = 120;
      e.Layout.Bands[0].Columns["Drawing"].Width = 60;
      e.Layout.Bands[0].Columns["Sample"].Width = 60;
      e.Layout.Bands[0].Columns["ItemQty"].Width = 100;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["CarcassCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Columns["Revision"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ItemQty"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["PackingDeadline"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["WORemark"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Bands[0].Columns["RequestDate"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Urgent"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ProjectCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ExpectedBrand"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Drawing"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Sample"].CellAppearance.BackColor = Color.Aqua;

      System.Globalization.DateTimeFormatInfo d = new System.Globalization.DateTimeFormatInfo();
      string columnName1 = d.MonthNames[DateTime.Now.AddMonths(-5).Month - 1];
      string columnName2 = d.MonthNames[DateTime.Now.AddMonths(-4).Month - 1];
      string columnName3 = d.MonthNames[DateTime.Now.AddMonths(-3).Month - 1];
      string columnName4 = d.MonthNames[DateTime.Now.AddMonths(-2).Month - 1];
      string columnName5 = d.MonthNames[DateTime.Now.AddMonths(-1).Month - 1];
      string monthCurrent = d.MonthNames[DateTime.Now.Month - 1];

      e.Layout.Bands[1].Columns["MaterialCode"].Hidden = true;
      e.Layout.Bands[1].Columns["PriceVN"].Header.Caption = "LastPrice(VND)";
      e.Layout.Bands[1].Columns["Column1"].Header.Caption = columnName1;
      e.Layout.Bands[1].Columns["Column2"].Header.Caption = columnName2;
      e.Layout.Bands[1].Columns["Column3"].Header.Caption = columnName3;
      e.Layout.Bands[1].Columns["Column4"].Header.Caption = columnName4;
      e.Layout.Bands[1].Columns["Column5"].Header.Caption = columnName5;
      e.Layout.Bands[1].Columns["Column6"].Header.Caption = monthCurrent;

      e.Layout.Bands[1].Columns["PriceVN"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Column1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Column2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Column3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Column4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Column5"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Column6"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["PriceVN"].Format = "###,###.##";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;

      if (this.flagHideWOCarcass)
      {
        e.Layout.Bands[0].Columns["WO"].Hidden = false;
        e.Layout.Bands[0].Columns["ItemCode"].Hidden = false;
      }
      else
      {
        e.Layout.Bands[0].Columns["WO"].Hidden = true;        
        e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      }

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      //Requester
      if (this.flagHeadDepartment == false)
      {
        if (this.status == 0)
        {
          e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
          e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
          e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
        }
      }
      else if (this.flagHeadDepartment == true)
      {
        if (this.status == 0)
        {
          if (this.PRPid == long.MinValue)
          {
            e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
            e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
            e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
          }
          else
          {
            string commandText = string.Empty;
            commandText = string.Format(@"SELECT CreateBy FROM TblPURPROnlineInformation WHERE PID = {0}", this.PRPid);
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            int createBy = DBConvert.ParseInt(dt.Rows[0][0]);
            if (SharedObject.UserInfo.UserPid == createBy)
            {
              e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
              e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
              e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
            }
          }
        }
      }

      if (this.flagViewPrice == true)
      {
        e.Layout.Bands[1].Columns["PriceVN"].Hidden = false;
      }
      else
      {
        e.Layout.Bands[1].Columns["PriceVN"].Hidden = true;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;

      DateTime createDate = DateTime.MinValue;
      string materialCode = string.Empty;
      DateTime requestDate = DateTime.MinValue;
      switch (columnName)
      {
        case "WO":
          row.Cells["CarcassCode"].Value = DBNull.Value;
          row.Cells["ItemCode"].Value = DBNull.Value;
          break;
        case "MaterialCode":
          row.Cells["MaterialNameEn"].Value = ultddMaterial.SelectedRow.Cells["MaterialNameEn"].Value.ToString();
          row.Cells["MaterialNameVn"].Value = ultddMaterial.SelectedRow.Cells["MaterialNameVn"].Value.ToString();
          row.Cells["Unit"].Value = ultddMaterial.SelectedRow.Cells["Unit"].Value.ToString();
          row.Cells["Leadtime"].Value = DBConvert.ParseDouble(ultddMaterial.SelectedRow.Cells["Leadtime"].Value.ToString());

          createDate = DBConvert.ParseDateTime(txtCreateDate.Text, ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          materialCode = row.Cells["MaterialCode"].Value.ToString();

          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@CreateDate", DbType.DateTime, createDate);
          input[1] = new DBParameter("@MaterialCode", DbType.String, materialCode);
          DataTable dtRequestDate = DataBaseAccess.SearchStoreProcedureDataTable("spPURPROnlineGetRequestDate", input);
          if (dtRequestDate != null && dtRequestDate.Rows.Count > 0 && DBConvert.ParseDateTime(dtRequestDate.Rows[0]["RequestDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            row.Cells["RequestDate"].Value = DBConvert.ParseDateTime(dtRequestDate.Rows[0]["RequestDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }
          else
          {
            row.Cells["RequestDate"].Value = System.DateTime.Now.AddDays(7);
          }
          break;
        case "RequestDate":
          if (row.Cells["MaterialCode"].Text.Length > 0 && row.Cells["RequestDate"].Text.Length > 0)
          {
            createDate = DBConvert.ParseDateTime(txtCreateDate.Text, ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            materialCode = row.Cells["MaterialCode"].Value.ToString();
            requestDate = DBConvert.ParseDateTime(row.Cells["RequestDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);

            DBParameter[] input1 = new DBParameter[3];
            input1[0] = new DBParameter("@CreateDate", DbType.DateTime, createDate);
            input1[1] = new DBParameter("@MaterialCode", DbType.String, materialCode);
            input1[2] = new DBParameter("@RequestDate", DbType.DateTime, requestDate);
            DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPURPROnlineRequestDateAlert", input1);
            if (dt != null && dt.Rows.Count > 0)
            {
              row.Appearance.ForeColor = Color.Red;
              row.Appearance.FontData.Bold = DefaultableBoolean.True;
              row.Appearance.BackColor = Color.Yellow;
              row.Cells["AlertRequestDate"].Value = 1;
            }
            else
            {
              row.Appearance.ForeColor = Color.Empty;
              row.Appearance.FontData.Bold = DefaultableBoolean.False;
              row.Appearance.BackColor = Color.Empty;
              row.Cells["AlertRequestDate"].Value = 0;
            }
          }
          else
          {
            row.Appearance.ForeColor = Color.Empty;
            row.Appearance.FontData.Bold = DefaultableBoolean.False;
            row.Appearance.BackColor = Color.Empty;
            row.Cells["AlertRequestDate"].Value = 0;
          }
          break;
        default:
          break;
      }
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.PRPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["PID"].Value.ToString()) > 0)
          {
            DBParameter[] inputParams = new DBParameter[1];
            inputParams[0] = new DBParameter("@PRDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["PID"].Value.ToString()));
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            string storeName = string.Empty;
            storeName = "spPURPROnlineDetail_Delete";

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
    }

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      if (row.ParentRow == null)
      {
        int wo = DBConvert.ParseInt(row.Cells["WO"].Value.ToString());
        string ItemCode = row.Cells["ItemCode"].Value.ToString();
        switch (columnName)
        {
          case "CarcassCode":
            this.LoadCarcass(wo);
            break;
          case "ItemCode":
            this.LoadItem(wo);
            break;
          case "Revision":
            UltraDropDown ultdd = this.ultDDRevision;
            row.Cells["Revision"].ValueList = this.LoadDDRevision(ultdd, ItemCode);
            break;
          default:
            break;
        }
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.PRPid == long.MinValue)
      {
        return;
      }
      if (WindowUtinity.ShowMessageConfirm("MSG0007", txtPRNo.Text).ToString() == "No")
      {
        return;
      }
      if (this.PRPid != long.MinValue)
      {
        string storeName = "spPURPROnlineInfomation_Delete";
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@PRPid", DbType.Int64, this.PRPid);

        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result == 1)
        {
          WindowUtinity.ShowMessageSuccess("MSG0002");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0093", txtPRNo.Text);
          return;
        }
      }
      this.btnDelete.Visible = false;
      this.CloseTab();
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      string commandText = string.Empty;
      DataTable dtCheck;
      DataRow[] foundRow;
      switch (columnName.ToLower())
      {
        case "qty":
          if (DBConvert.ParseDouble(text) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty");
            e.Cancel = true;
          }
          break;

        case "wo":
          foundRow = dtWO.Select("WO = " + DBConvert.ParseInt(text));
          if (foundRow.Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "WO");
            e.Cancel = true;
          }
          break;
        case "carcasscode":
          if (text.Length > 0)
          {
            foundRow = dtCarcass.Select("CarcassCode = '" + text + "'");
            if (foundRow.Length == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Carcass Code");
              e.Cancel = true;
            }
          }
          break;
        case "itemcode":
          if (text.Length > 0)
          {
            foundRow = dtItemCode.Select("ItemCode = '" + text + "'");
            if (foundRow.Length == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Item Code");
              e.Cancel = true;
            }
          }
          break;
        case "materialcode":
          commandText += " SELECT COUNT(*) SL ";
          commandText += " FROM TblGNRMaterialInformation ";
          commandText += " WHERE MaterialCode = '" + text + "'";

          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null)
          {
            if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Material Code");
              e.Cancel = true;
            }
          }
          break;
        case "urgent":
          commandText += " SELECT COUNT(*) SL ";
          commandText += " FROM TblBOMCodeMaster ";
          commandText += " WHERE [Group] = 7007 ";
          commandText += "    AND Value = '" + text + "'";

          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null)
          {
            if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Urgent");
              e.Cancel = true;
            }
          }
          break;
        case "projectcode":
          commandText = string.Format(@" SELECT  Pid Code, ProjectCode Name 
                                        FROM    TblPURPRProjectCode 
                                        WHERE   ISNULL(Finished, 0) = 0 AND ISNULL([Status], 0) = 1 AND ProjectCode = '{0}' AND Department = '{1}'", text, SharedObject.UserInfo.Department);

          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (e.Cell.Text.Trim().Length > 0 && dtCheck != null && dtCheck.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Project Code");
            e.Cancel = true;
          }
          break;
        case "requestdate":
          DateTime requestDate = DBConvert.ParseDateTime(text, ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          // Check RequestDate Is Holiday
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@Date", DbType.DateTime, requestDate);
          DataTable dtCheckHoliday = DataBaseAccess.SearchStoreProcedureDataTable("spGNRCheckDateHoliday", input);
          if (dtCheckHoliday != null && dtCheckHoliday.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Required Delivery Date Is Holiday ");
            e.Cancel = true;
          }
          // Check RequestDate Is Holiday
          break;
        default:
          break;
      }
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      this.ImportExcel();
    }

    private void btnGettemplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_PUR_21_002_001";      
      string sheetName = "Sheet1";
      string outFileName = "Template Import PR Online";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\Purchasing\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtImportExcel.Text.Trim().Length > 0);
    }

    private void btnReturn_Click(object sender, EventArgs e)
    {
      // 2: Save Data
      bool success = this.SaveData();
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
        return;
      }

      // input
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PRPid", DbType.Int64, this.PRPid);
      input[1] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      // output
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      //Execute
      DataBaseAccess.ExecuteStoreProcedure("spPURPROnlineReturnFromHeadDepartment_Update", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        // Error
        WindowUtinity.ShowMessageError("ERR0037", "Return");
      }
      else
      {
        // Success
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      this.LoadData();
    }

    private void ultUploadFile_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultUploadFile.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }

      UltraGridRow row = ultUploadFile.Selected.Rows[0];
      Process prc = new Process();

      if (DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) <= 0)
      {
        prc.StartInfo = new ProcessStartInfo(row.Cells["Location"].Value.ToString());
      }
      else
      {
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string folder = string.Format(@"{0}\Temporary", startupPath);
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }
        string location = row.Cells["Location"].Value.ToString();
        if (File.Exists(location))
        {
          string newLocation = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(row.Cells["Location"].Value.ToString()));
          if (File.Exists(newLocation))
          {
            try
            {
              File.Delete(newLocation);
            }
            catch
            {
              WindowUtinity.ShowMessageWarningFromText("File Is Opening!");
              return;
            }
          }
          File.Copy(location, newLocation);
          prc.StartInfo = new ProcessStartInfo(newLocation);
        }
      }
      try
      {
        prc.Start();
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
      }
    }

    private void btnBrownUploadFile_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnUploadFile.Enabled = (txtLocation.Text.Trim().Length > 0);
    }

    private void btnUploadFile_Click(object sender, EventArgs e)
    {
      // Get Datatable From UltraGrid
      DataTable dtMain = (DataTable)ultUploadFile.DataSource;
      DataRow row = dtMain.NewRow();

      // Upload
      if (txtLocation.Text.Trim().Length > 0)
      {
        string fileName = string.Empty;
        string location = string.Empty;
        string commandText = string.Empty;
        // FileName
        fileName = System.IO.Path.GetFileName(txtLocation.Text);

        row["FileName"] = fileName;
        row["Location"] = txtLocation.Text;

        // Add Row
        dtMain.Rows.Add(row);
        ultUploadFile.DataSource = dtMain;

        btnUploadFile.Enabled = false;
      }
    }

    private void ultUploadFile_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Location"].Hidden = true;





      e.Layout.Bands[0].Columns["FileName"].CellActivation = Activation.ActivateOnly;

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      if (this.status > 0)
      {
        e.Layout.Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultUploadFile_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.PRPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
          {
            // Delete File
            string location = row.Cells["Location"].Value.ToString();
            File.Delete(location);
            // End Delete File

            DBParameter[] inputParams = new DBParameter[1];
            inputParams[0] = new DBParameter("@UploadDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            string storeName = string.Empty;
            storeName = "spPURPROnlineUploadFile_Delete";
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
            if (DBConvert.ParseInt(outputParams[0].Value.ToString()) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0004");
              this.LoadData();
              return;
            }
          }
        }
      }
    }

    private void chkUploadFileHide_CheckedChanged(object sender, EventArgs e)
    {
      this.grbUploadFile.Visible = chkUploadFileHide.Checked;
    }

    /// <summary>
    /// Add For WO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddForWO_Click(object sender, EventArgs e)
    {
      
    }

    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }

    private void btnAdjust_Click(object sender, EventArgs e)
    {
      viewPUR_21_015 view = new viewPUR_21_015();
      view.PROnlinePid = this.PRPid;
      view.flagHeadDepartment = this.flagHeadDepartment;
      WindowUtinity.ShowView(view, "ADJUST DEMAND PR", true, DaiCo.Shared.Utility.ViewState.Window);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultData, "Data");
    }
    #endregion Event
  }
}
