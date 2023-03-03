/*
  Author      : 
  Date        : 15/10/2011
  Description : Request IT Info
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.IO;
using System.Collections;
using System.Diagnostics;
using DaiCo.Shared.DataSetSource.General;
using DaiCo.Shared.ReportTemplate.General;

namespace DaiCo.General
{
  public partial class viewGNR_01_002_001 : MainUserControl
  {
    #region Field
    // requestITPid
    public long requestITPid = long.MinValue;
    // Status
    private int status = 0;
    // Flag Update
    private bool canUpdate = false;

    //Check Department
    private int checkdepartment = 0;

    private string sourseFile = string.Empty;
    private string destFile = string.Empty;

    private IList pidDetailCoding = new ArrayList();
    private IList pidScreenTime = new ArrayList();
    private long taskPid = -1;
    #endregion Field

    #region Init
    public viewGNR_01_002_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewGNR_01_002
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_01_002_001_Load(object sender, EventArgs e)
    {
      this.checkdepartment = this.CheckDepartmentIT();
      this.LoadComboDepartment();
      this.LoadComboProgramModule();
      this.LoadComboUrgentLevel();
      this.LoadComboType();
      this.LoadComboITType();
      this.LoadDropDownITStaff();
      this.LoadComboDepartmentUserLogin();
      this.LoadCBITRequest();
      this.LoadUltDDPerson();
      this.LoadScreenTime();

      //Load Data
      this.LoadData();
    }

    private void LoadScreenTime()
     {
      string commandText = "SELECT Code, Value, CAST([Description] AS float) [Time], ISNULL(Kind, 0) Kind FROM TblBOMCodeMaster WHERE [Group] = 9014";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDScreenTime.DataSource = dtSource;
      ultDDScreenTime.DisplayMember = "Value";
      ultDDScreenTime.ValueMember = "Code";
      ultDDScreenTime.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultDDScreenTime.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDDScreenTime.DisplayLayout.Bands[0].Columns["Value"].Width = 250;
      ultDDScreenTime.DisplayLayout.Bands[0].Columns["Time"].Width = 50;
    }

    private void LoadUltDDPerson()
    {
      string commandText = "Select EM.Pid, EmpName ";
      commandText += " From VHRMEmployee EM ";
      commandText += "    INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "                      AND EM.Department = CM.Value ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraDropDown(ultDDPerson, dtSource, "Pid", "EmpName", "Pid");
      ultDDPerson.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDPerson.DisplayLayout.Bands[0].Columns["EmpName"].Width = 250;
    }

    private void LoadCBITRequest()
    {
      string commandText = "SELECT Pid, RequestCode FROM TblGNRRequestIT ORDER BY Pid DESC";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBProject.DataSource = dtSource;
      ultraCBProject.DisplayMember = "RequestCode";
      ultraCBProject.ValueMember = "Pid";
      ultraCBProject.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultraCBProject.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private double GetTotalTime(DataTable dt)
    {
      double total = 0;
      foreach (DataRow row in dt.Rows)
      {
        double time = DBConvert.ParseDouble(row["Time"].ToString());
        if (time != double.MinValue)
        {
          total += time;
        }
      }
      return total;
    }

    /// <summary>
    /// Check Department
    /// </summary>
    /// <returns></returns>
    private int CheckDepartmentIT()
    {
      int userPid = SharedObject.UserInfo.UserPid;
      string commandText = string.Empty;
      commandText += " SELECT COUNT(*) " ;
      commandText += " FROM VHRNhanVien NV " ;
      commandText += "    INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "               AND NV.Department = CM.Value ";
      commandText += "               AND ID_NhanVien = " + userPid;
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 1)
        {
          return 1;
        }
      }
      return 0;
    }

    /// <summary>
    /// Set Default Department
    /// </summary>
    private void LoadComboDepartmentUserLogin()
    {
      string commandText = "SELECT Department FROM VHRNhanVien WHERE ID_NhanVien = " + SharedObject.UserInfo.UserPid;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        this.ultcbDepartment.Value = dtSource.Rows[0][0].ToString();
      }
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultcbDepartment.DataSource = dtSource;
      ultcbDepartment.DisplayMember = "Name";
      ultcbDepartment.ValueMember = "Department";
      ultcbDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultcbDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo ProgramModule
    /// </summary>
    private void LoadComboProgramModule()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_PROGRAMMODULE;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        this.ultcbProgramModule.Value = dtSource.Rows[0][0].ToString();
      }
      ultcbProgramModule.DataSource = dtSource;
      ultcbProgramModule.DisplayMember = "Value";
      ultcbProgramModule.ValueMember = "Code";
      ultcbProgramModule.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbProgramModule.DisplayLayout.Bands[0].Columns["Value"].Width = 350;
      ultcbProgramModule.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo UrgentLevel
    /// </summary>
    private void LoadComboUrgentLevel()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_URGENTLEVEL;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        this.ultcbUrgentLevel.Value = dtSource.Rows[3][0].ToString();
      }
      ultcbUrgentLevel.DataSource = dtSource;
      ultcbUrgentLevel.DisplayMember = "Value";
      ultcbUrgentLevel.ValueMember = "Code";
      ultcbUrgentLevel.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbUrgentLevel.DisplayLayout.Bands[0].Columns["Value"].Width = 350;
      ultcbUrgentLevel.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Type
    /// </summary>
    private void LoadComboType()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_TYPE;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        this.ultcbType.Value = dtSource.Rows[0][0].ToString();
      }
      ultcbType.DataSource = dtSource;
      ultcbType.DisplayMember = "Value";
      ultcbType.ValueMember = "Code";
      ultcbType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbType.DisplayLayout.Bands[0].Columns["Value"].Width = 350;
      ultcbType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo ITType
    /// </summary>
    private void LoadComboITType()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_ITTYPE;
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultcbITType.DataSource = dtSource;
      ultcbITType.DisplayMember = "Value";
      ultcbITType.ValueMember = "Code";
      ultcbITType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbITType.DisplayLayout.Bands[0].Columns["Value"].Width = 250;
      ultcbITType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load DropDown IT Staff
    /// </summary>
    private void LoadDropDownITStaff()
    {
      string commandText = string.Empty;
      commandText += "SELECT NV.Pid, CONVERT(VARCHAR, NV.Pid) + ' - ' + NV.EmpName ITUser ";
      commandText += "FROM VHRMEmployee NV ";
      commandText += "  INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "                      AND NV.Department = CM.Value ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDITStaff.DataSource = dtSource;
      ultDDITStaff.DisplayMember = "ITUser";
      ultDDITStaff.ValueMember = "Pid";
      ultDDITStaff.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDITStaff.DisplayLayout.Bands[0].Columns["ITUser"].Width = 200;
      ultDDITStaff.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      this.pidDetailCoding = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.requestITPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spGNRRequestITInformationByRequestITPid_Select", inputParam);
      DataTable dtRequestITInfo = dsSource.Tables[0];

      if (dtRequestITInfo.Rows.Count > 0)
      {
        DataRow row = dtRequestITInfo.Rows[0];

        this.txtRequestCode.Text = row["RequestCode"].ToString();
        this.ultcbProgramModule.Value = DBConvert.ParseInt(row["ProgramModule"].ToString());
        this.ultcbDepartment.Value = row["DepartMent"].ToString();
        this.txtTitle.Text = row["Title"].ToString();
        this.ultRequestDate.Value = (DateTime)row["RequestDate"];
        this.ultExpectDate.Value = (DateTime)row["ExpectDate"];
        this.ultcbUrgentLevel.Value = DBConvert.ParseInt(row["UrgenLevel"].ToString());
        this.ultcbType.Value = DBConvert.ParseInt(row["Type"].ToString());
        this.txtCreateBy.Text = row["CreateBy"].ToString();
        this.txtCreateDate.Text = DBConvert.ParseString((DateTime)row["CreateDate"], Shared.Utility.ConstantClass.FORMAT_DATETIME);
        this.txtDescription.Text = row["Desciption"].ToString();
        this.status = DBConvert.ParseInt(row["Status"].ToString());
        this.ultcbITType.Value = row["ITType"].ToString();
        this.txtDescriptionIT.Text = row["ITDescription"].ToString();
        this.txtDescriptionITConfirm.Text = row["DescriptionITConfirm"].ToString();
        this.txtRelativeDepartment.Text = row["RelativeDepartment"].ToString();

        if (this.status == 0)
        {
          this.lbStatusConfirm.Text = "New";
        }
        else if (this.status == 1)
        {
          this.lbStatusConfirm.Text = "Department Confirmed";
        }
        else if (this.status == 2)
        {
          this.lbStatusConfirm.Text = "IT Confirmed";
        }
        else if (this.status == 3)
        {
          this.lbStatusConfirm.Text = "Finished";
        }
        // Khac Nhan Vien IT
        if (this.checkdepartment == 0)
        {
          if (this.status >= 1)
          {
            this.chkConfirm.Checked = true;
          }
          else
          {
            this.chkConfirm.Checked = false;
          }
        }
        // La Nhan Vien IT
        else
        {
          if (this.status == 2)
          {
            this.chkConfirm.Checked = true;
          }
          else
          {
            this.chkConfirm.Checked = false;
          }
        }
        // Load total time for request
        this.LoadTotalTimeForRequest(this.requestITPid);
      }
      else
      {
        tabControl1.TabPages.Remove(tabPlan);
        DataTable dtRequestIT = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FGNRGetNewRequestNo('RES') NewRequestITNo");
        if ((dtRequestIT != null) && (dtRequestIT.Rows.Count > 0))
        {
          this.txtRequestCode.Text = dtRequestIT.Rows[0]["NewRequestITNo"].ToString();
          this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          this.lbStatusConfirm.Text = "New";
        }
      }
      this.ultData.DataSource = dsSource.Tables[1];

      // Department Upload File
      this.ultUploadFile.DataSource = dsSource.Tables[2];
      if (this.ultUploadFile.Rows.Count > 0)
      {
        this.chkUpload.Checked = true;
        this.ultUploadFile.Visible = true;
      }
      else
      {
        this.ultUploadFile.Visible = false;
      }
      for (int i = 0; i < ultUploadFile.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultUploadFile.Rows[i];
        if (File.Exists(rowGrid.Cells["File"].Value.ToString()))
        {
          rowGrid.Cells["Type"].Appearance.Image = Image.FromFile(rowGrid.Cells["File"].Value.ToString());
        }
      }

      //File IT Upload
      this.ultITUploadFile.DataSource = dsSource.Tables[3];

      for (int i = 0; i < ultITUploadFile.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultITUploadFile.Rows[i];
        if (File.Exists(rowGrid.Cells["File"].Value.ToString()))
        {
          rowGrid.Cells["Type"].Appearance.Image = Image.FromFile(rowGrid.Cells["File"].Value.ToString());
        }    
      }
      //Set Control
      this.SetStatusControl();

    }

    /// <summary>
    /// Load Total Time For Request
    /// </summary>
    /// <param name="pid"></param>
    private void LoadTotalTimeForRequest(long pid)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@RequestPid", DbType.Int64, pid);

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRITRequestGetTotalTimeRequest_Select", input);
      DataSet dsSource = this.CreateDataSet();
      dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
      dsSource.Tables["dtChild"].Merge(ds.Tables[1]);

      ultraGridInformation.DataSource = dsSource;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultraGridInformation.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(rowChild.Cells["Kind"].Value.ToString()) == 1)
          {
            rowChild.Cells["TimeDetail"].Activation = Activation.ActivateOnly;
          }
        }    
      }
      txtTotal.Text = this.GetTotalTime(dsSource.Tables["dtParent"]).ToString();
    }

    /// <summary>
    /// Create DataSet
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("TaskNo", typeof(System.String));
      taParent.Columns.Add("TaskName", typeof(System.String));
      taParent.Columns.Add("Time", typeof(System.Double));
      taParent.Columns.Add("PersonIncharge", typeof(System.Int32));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("TaskDetailPid", typeof(System.Int64));
      taChild.Columns.Add("Screen", typeof(System.Int32));
      taChild.Columns.Add("TimeDetail", typeof(System.Double));
      taChild.Columns.Add("Remark", typeof(System.String));
      taChild.Columns.Add("Kind", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["TaskDetailPid"], false));
      return ds;
    }

    /// <summary>
    /// SetStatus Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.checkdepartment == 0)
      {
        this.canUpdate = (btnSave.Visible && this.status >= 1);

        this.ultcbProgramModule.ReadOnly = canUpdate;
        this.ultcbDepartment.ReadOnly = true;
        this.ultcbProgramModule.ReadOnly = canUpdate;
        this.txtTitle.ReadOnly = canUpdate;
        this.ultRequestDate.ReadOnly = canUpdate;
        this.ultExpectDate.ReadOnly = canUpdate;
        this.ultcbUrgentLevel.ReadOnly = canUpdate;
        this.ultcbType.ReadOnly = canUpdate;
        this.txtDescription.ReadOnly = canUpdate;
        this.txtRelativeDepartment.ReadOnly = canUpdate;
        //26102011
        ultUploadFile.DisplayLayout.Bands[0].Columns["FileName"].CellActivation = Activation.ActivateOnly;
        ultUploadFile.DisplayLayout.Bands[0].Columns["Type"].CellActivation = Activation.ActivateOnly;
        if (this.canUpdate == true)
        {
          this.btnUpload.Enabled = false;
          this.btnBrowse.Enabled = false;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        }
        //

        this.ultcbITType.ReadOnly = true;
        this.txtDescriptionIT.ReadOnly = true;
        this.txtDescriptionITConfirm.ReadOnly = true;
        ultData.DisplayLayout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        ultData.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        ultData.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

        //Truong Update 17122011
        this.txtLocationITUpload.ReadOnly = true;
        this.btnITBrown.Enabled = false;
        this.btnITUpload.Enabled = false;
        ultITUploadFile.DisplayLayout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
        ultITUploadFile.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        ultITUploadFile.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        //

        if (this.canUpdate == true || this.status == 2)
        {
          this.chkConfirm.Enabled = false;
          this.btnDelete.Enabled = false;
        }
        else
        {
          this.chkConfirm.Enabled = true;
          this.btnSave.Enabled = true;
        }
        this.btnFinish.Enabled = false;
      }
      else
      {
        this.canUpdate = (btnSave.Visible && this.status >= 2);
        if (canUpdate == true)
        {
          ultData.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
          ultData.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        }
        this.ultcbProgramModule.ReadOnly = canUpdate;
        this.ultcbDepartment.ReadOnly = canUpdate;
        this.ultcbProgramModule.ReadOnly = canUpdate;
        this.txtTitle.ReadOnly = canUpdate;
        this.ultRequestDate.ReadOnly = canUpdate;
        this.ultExpectDate.ReadOnly = canUpdate;
        this.ultcbUrgentLevel.ReadOnly = canUpdate;
        this.ultcbType.ReadOnly = canUpdate;
        this.txtDescription.ReadOnly = canUpdate;
        this.txtRelativeDepartment.ReadOnly = canUpdate;
        this.ultcbITType.ReadOnly = canUpdate;
        this.txtDescriptionIT.ReadOnly = canUpdate;
        this.txtDescriptionITConfirm.ReadOnly = true;
        this.btnFinish.Enabled = false;
        ultUploadFile.DisplayLayout.Bands[0].Columns["FileName"].CellActivation = Activation.ActivateOnly;
        ultUploadFile.DisplayLayout.Bands[0].Columns["Type"].CellActivation = Activation.ActivateOnly;
        ultITUploadFile.DisplayLayout.Bands[0].Columns["FileName"].CellActivation = Activation.ActivateOnly;
        ultITUploadFile.DisplayLayout.Bands[0].Columns["Type"].CellActivation = Activation.ActivateOnly;
        if (this.status == 1)
        {
          this.ultcbProgramModule.ReadOnly = true;
          this.ultcbDepartment.ReadOnly = true;
          this.ultcbProgramModule.ReadOnly = true;
          this.txtTitle.ReadOnly = true;
          this.ultRequestDate.ReadOnly = true;
          this.ultExpectDate.ReadOnly = true;
          this.ultcbUrgentLevel.ReadOnly = true;
          this.ultcbType.ReadOnly = true;
          this.txtDescription.ReadOnly = true;
          this.txtRelativeDepartment.ReadOnly = true;
          this.txtDescriptionITConfirm.ReadOnly = true;
          this.btnFinish.Enabled = false;

          //26102011
          this.btnDelete.Enabled = false;
          this.btnUpload.Enabled = false;
          this.btnBrowse.Enabled = false;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
          //
        }
        else if (this.status == 2)
        {
          ultData.DisplayLayout.Bands[0].Columns[6].CellActivation = Activation.AllowEdit;
          ultData.DisplayLayout.Bands[0].Columns[2].CellActivation = Activation.ActivateOnly;
          ultData.DisplayLayout.Bands[0].Columns[3].CellActivation = Activation.AllowEdit;
          ultData.DisplayLayout.Bands[0].Columns[4].CellActivation = Activation.ActivateOnly;
          ultData.DisplayLayout.Bands[0].Columns[5].CellActivation = Activation.ActivateOnly;
          ultData.DisplayLayout.Bands[0].Columns[7].CellActivation = Activation.AllowEdit;
          ultData.DisplayLayout.Bands[0].Columns[8].CellActivation = Activation.AllowEdit;
          ultData.DisplayLayout.Bands[0].Columns[9].CellActivation = Activation.ActivateOnly;
          ultData.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
          ultData.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

          this.txtDescriptionIT.ReadOnly = true;
          this.txtDescriptionITConfirm.ReadOnly = false;
          this.ultcbITType.ReadOnly = true;
          this.btnFinish.Enabled = true;
          this.btnDelete.Enabled = false;
          this.chkConfirm.Enabled = false;

          //26102011
          this.btnUpload.Enabled = false;
          this.btnBrowse.Enabled = false;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        }
        else if (this.status == 3)
        {
          ultData.DisplayLayout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
          this.chkConfirm.Enabled = false;
          this.btnSave.Enabled = false;
          this.btnDelete.Enabled = false;
          this.btnFinish.Enabled = false;

          //26102011
          this.btnUpload.Enabled = false;
          this.btnBrowse.Enabled = false;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
          ultUploadFile.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

          //Truong Update 17/12/2011
          this.btnITBrown.Enabled = false;
          this.btnITUpload.Enabled = false;
          ultITUploadFile.DisplayLayout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
          ultITUploadFile.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
          ultITUploadFile.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
          //
        }
      }
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Check valid IT Request
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidITRequest(out string message)
    {
      message = string.Empty;

      bool success = this.CheckValidDepartment(out message);
      if (!success)
      {
        return false;
      }

      // ITType
      if (this.ultcbITType.Value != null && this.ultcbITType.Text.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_ITTYPE
                          + " AND Code = '" + this.ultcbITType.Value.ToString() + "'";
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "ITType";
            return false;
          }
        }
        else
        {
          message = "ITType";
          return false;
        }
      }
      else
      {
        message = "ITType";
        return false;
      }

      if (ultData.Rows.Count == 0)
      {
        message = "IT Staff";
        return false;
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        //Check IT User
        if (ultData.Rows[i].Cells["ITUser"].Value.ToString().Length == 0)
        {
          message = "IT User";
          return false;
        }

        //Check Promise Date
        if (ultData.Rows[i].Cells["PromiseDate"].Value.ToString().Length == 0)
        {
          message = "Promise Date";
          return false;
        }
        else
        {
          DateTime promiseDate = (DateTime)this.ultData.Rows[i].Cells["PromiseDate"].Value;
          DateTime requestDate = (DateTime)this.ultRequestDate.Value;
          int result = DateTime.Compare(promiseDate, requestDate);
          if (result < 0)
          {
            message = "Promise date >= Request date";
            return false;
          }
        }

        ////Check Start Date
        //if (ultData.Rows[i].Cells["StartDate"].Value.ToString().Length == 0)
        //{
        //  message = "Start Date";
        //  return false;
        //}
        //else
        //{
        //  DateTime startDate = (DateTime)this.ultData.Rows[i].Cells["StartDate"].Value;
        //  DateTime requestDate = (DateTime)this.ultRequestDate.Value;
        //  DateTime promiseDate = (DateTime)this.ultData.Rows[i].Cells["PromiseDate"].Value;
        //  int result = DateTime.Compare(startDate, requestDate);
        //  if (result < 0)
        //  {
        //    message = "Start Date >= Request date";
        //    return false;
        //  }
        //  int result1 = DateTime.Compare(promiseDate, startDate);
        //  if (result1 < 0)
        //  {
        //    message = "Start date <= Promise Date";
        //    return false;
        //  }
        //}

        //Check Finish Date
        if (ultData.Rows[i].Cells["FinishDate"].Value.ToString().Length > 0)
        {
          DateTime startDate = (DateTime)this.ultData.Rows[i].Cells["StartDate"].Value;
          DateTime finishDate = (DateTime)this.ultData.Rows[i].Cells["FinishDate"].Value;
          int result = DateTime.Compare(finishDate, startDate);
          if (result < 0)
          {
            message = "Finish Date < Start date";
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// CheckValid Department
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidDepartment(out string message)
    {
      message = string.Empty;

      // Department
      if (this.ultcbDepartment.Value != null && this.ultcbDepartment.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM VHRDDepartment WHERE Department = '" + this.ultcbDepartment.Value.ToString() + "'";
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Department";
            return false;
          }
        }
        else
        {
          message = "Department";
          return false;
        }
      }
      else
      {
        message = "Department";
        return false;
      }

      // Program Module
      if (this.ultcbProgramModule.Value != null && this.ultcbProgramModule.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_PROGRAMMODULE
                      + " AND Code ='" + this.ultcbProgramModule.Value.ToString() + "'";
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Program Module";
            return false;
          }
        }
        else
        {
          message = "Program Module";
          return false;
        }
      }
      else
      {
        message = "Program Module";
        return false;
      }

      // Urgent Level
      if (this.ultcbUrgentLevel.Value != null && this.ultcbUrgentLevel.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_URGENTLEVEL
                      + " AND Code = '" + this.ultcbUrgentLevel.Value.ToString() + "'";
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Urgent Level";
            return false;
          }
        }
        else
        {
          message = "Urgent Level";
          return false;
        }
      }
      else
      {
        message = "Urgent Level";
        return false;
      }

      // Type
      if (this.ultcbType.Value != null && this.ultcbType.Value.ToString().Length > 0)
      {
        string commandText = string.Empty;
        commandText = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_TYPE
                    + " AND Code = '" + this.ultcbType.Value.ToString() + "'";
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Type";
            return false;
          }
        }
        else
        {
          message = "Type";
          return false;
        }
      }
      else
      {
        message = "Type";
        return false;
      }

      // Title
      if (this.txtTitle.Text.ToString().Length == 0)
      {
        message = "Title";
        return false;
      }

      // Request Date
      if (this.ultRequestDate.Value == null)
      {
        message = "Request Date";
        return false;
      }

      DateTime requestDate = (DateTime)this.ultRequestDate.Value;
      int result = DateTime.Compare(DateTime.Today, requestDate);
      if (result < 0 && this.status == 0)
      {
        message = "Request Date <= Today";
        return false;
      }

      // Expect Date
      if (this.ultExpectDate.Value == null)
      {
        message = "Expect Date";
        return false;
      }
      DateTime expectDate = (DateTime)this.ultExpectDate.Value;
      if (DateTime.Compare(requestDate, expectDate) > 0)
      {
        message = "Expect Date >= Request Date";
        return false;
      }
      return true;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      bool result = true;
      if (this.checkdepartment == 1)
      {
        result = this.CheckValidITRequest(out message);
        if (!result)
        {
          return false;
        }
      }
      else
      {
        result = this.CheckValidDepartment(out message);
        if (!result)
        {
          return false;
        }
      }
      result = this.CheckTaskDetail(out message);
      if (!result)
      {
        return false;
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
      if (this.checkdepartment == 0)
      { //Save RequestIT
        result = this.SaveRequestIT();
      }
      else
      {
        //Save RequestIT
        result = this.SaveRequestIT();
        if (!result)
        {
          return false;
        }

        // Save RequestIT Detail
        result = this.SaveRequestITDetail();
        if (!result)
        {
          return false;
        }

        // Upload File
        result = SaveRequestITUploadFileTemplateConfirmed();
        if(!result)
        {
          return false;
        }
      }

      // IT Upload
      result = this.SaveRequestITUpload();

      // Save Task Detail
      result = this.SaveTaskDetail();

      return result;
    }

    /// <summary>
    /// Check Task Detail
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckTaskDetail(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        // Parent
        UltraGridRow rowParent = ultraGridInformation.Rows[i];
        string taskNo = rowParent.Cells["TaskNo"].Value.ToString().Trim();
        if (taskNo.Length == 0)
        {
          message = "Task No";
          this.SaveSuccess = false;
          return false;
        }
        double time = DBConvert.ParseDouble(rowParent.Cells["Time"].Value.ToString());
        if (time == double.MinValue)
        {
          message = "Time";
          this.SaveSuccess = false;
          return false;
        }
        int person = DBConvert.ParseInt(rowParent.Cells["PersonIncharge"].Value.ToString());
        if (person == int.MinValue)
        {
          message = "Person Incharge";
          this.SaveSuccess = false;
          return false;
        }

        for (int j = 0; j < rowParent.ChildBands[0].Rows.Count; j++)
        {
          // Detail
          UltraGridRow rowChild = rowParent.ChildBands[0].Rows[j];
          int screen = DBConvert.ParseInt(rowChild.Cells["Screen"].Value.ToString());
          if(screen == int.MinValue)
          {
            message = "Screen";
            this.SaveSuccess = false;
            return false;
          }
          double timeDetail = DBConvert.ParseDouble(rowChild.Cells["TimeDetail"].Value.ToString());
          if(timeDetail <= 0)
          {
            message = "Time Detail";
            this.SaveSuccess = false;
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Screen Time
    /// </summary>
    /// <param name="taskDetailPid"></param>
    /// <param name="rowParent"></param>
    /// <returns></returns>
    private bool SaveScreenTime(long taskDetailPid, UltraGridRow rowParent)
    {
      // Delete
      if (this.pidScreenTime.Count > 0)
      {
        foreach (long deletePid in this.pidScreenTime)
        {
          DBParameter[] input = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, deletePid) };
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spGNRITRequestScreenTime_Delete", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }

      // Save
      for (int i = 0; i < rowParent.ChildBands[0].Rows.Count; i++)
      {
        UltraGridRow rowChild = rowParent.ChildBands[0].Rows[i];

        // Get input data
        long pid = DBConvert.ParseLong(rowChild.Cells["Pid"].Value.ToString());
        int screen = DBConvert.ParseInt(rowChild.Cells["Screen"].Value.ToString());
        double timeDetail = DBConvert.ParseDouble(rowChild.Cells["TimeDetail"].Value.ToString());
        string remark = rowChild.Cells["remark"].Value.ToString().Trim();

        DBParameter[] input = new DBParameter[5];
        if (pid != long.MinValue)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, pid);
        }
        input[1] = new DBParameter("@TaskDetailPid", DbType.Int64, taskDetailPid);
        input[2] = new DBParameter("@Screen", DbType.Int32, screen);
        input[3] = new DBParameter("@Time", DbType.Double, timeDetail);
        if (remark.Length > 0)
        {
          input[4] = new DBParameter("@Remark", DbType.String, remark);
        }
        // Output
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spGNRITRequestScreenTime_Edit", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Task Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveTaskDetail()
    {
      bool success = true;
      // Delete
      if (this.pidDetailCoding.Count > 0)
      {
        foreach (long deletePid in this.pidDetailCoding)
        {
          DBParameter[] input = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, deletePid) };
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spGNRITRequestsTime_Delete", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            success = false;
          }
        }
      }

      // Save
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultraGridInformation.Rows[i];

        // Get input data
        long detailPid = DBConvert.ParseLong(rowParent.Cells["Pid"].Value.ToString());
        string taskNo = rowParent.Cells["TaskNo"].Value.ToString().Trim();
        string taskName = rowParent.Cells["TaskName"].Value.ToString().Trim();
        double time = DBConvert.ParseDouble(rowParent.Cells["Time"].Value.ToString());
        int person = DBConvert.ParseInt(rowParent.Cells["PersonIncharge"].Value.ToString());
        string remark = rowParent.Cells["Remark"].Value.ToString().Trim();

        DBParameter[] input = new DBParameter[8];
        if (detailPid > 0)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
        }
        input[1] = new DBParameter("@RequestPid", DbType.Int64, this.requestITPid);
        input[2] = new DBParameter("@TaskNo", DbType.AnsiString, 50, taskNo);
        if (taskName.Length > 0)
        {
          input[3] = new DBParameter("@TaskName", DbType.AnsiString, 200, taskName);
        }
        input[4] = new DBParameter("@Time", DbType.Double, time);
        input[5] = new DBParameter("@PersonIncharge", DbType.Int32, person);
        if (remark.Length > 0)
        {
          input[6] = new DBParameter("@Remark", DbType.String, 200, remark);
        }
        input[7] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        // Output
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spGNRITRequestsTime_Edit", input, output);
        // Task Pid
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result <= 0)
        {
          success = false;
        }
        else
        {
          // Save Screen Time
          success = this.SaveScreenTime(result, rowParent);
          if(success)
          {
            // Update Total Time Of Task
            success = this.UpdateTotalTime(result);
          }
        }
      }
      success = UpdateTaskOnERP();
      return success;
    }

    /// <summary>
    /// Update Total Time Of Task
    /// </summary>
    /// <param name="taskPid"></param>
    /// <returns></returns>
    private bool UpdateTotalTime(long taskPid)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@TaskPid", DbType.Int64, taskPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, 0);

      DataBaseAccess.ExecuteStoreProcedure("spGNRITRequestsTaskTotalTime_Update", input, output);

      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if(result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Update Task On Project
    /// </summary>
    /// <param name="taskPid"></param>
    /// <returns></returns>
    private bool UpdateTaskOnERP()
    {
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@RequestOnlinePid", DbType.Int64, this.requestITPid);
      input[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, 0);

      DataBaseAccess.ExecuteStoreProcedure("spGNRITInsertOnProject_InsertNew", input, output);

      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save RequestIT
    /// </summary>
    /// <returns></returns>
    private bool SaveRequestIT()
    {
      string storeName = string.Empty;
      storeName = "spGNRRequestIT_Edit";
      DBParameter[] inputParam = new DBParameter[17];

      //Pid
      if (this.requestITPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.requestITPid);
      }

      inputParam[1] = new DBParameter("@RequestCode", DbType.String, "RES");

      // Status
      if (this.chkConfirm.Checked)
      {
        if (checkdepartment == 0)
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          if (status == 2)
          {
            if (btnSave.Focused) 
            {
              inputParam[2] = new DBParameter("@Status", DbType.Int32, 2);
            }
            else if (btnFinish.Focused)
            {
              inputParam[2] = new DBParameter("@Status", DbType.Int32, 3);
            }
          }
          else
          {
            inputParam[2] = new DBParameter("@Status", DbType.Int32, 2);
          }
        }
      }
      else
      {
        if (this.status == 3)
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, 3);
        }
        else
        {
          if (checkdepartment == 0)
          {
            inputParam[2] = new DBParameter("@Status", DbType.Int32, 0);
          }
          else
          {
            if (this.status == 1)
            {
              inputParam[2] = new DBParameter("@Status", DbType.Int32, 1);
            }
            else
            {
              inputParam[2] = new DBParameter("@Status", DbType.Int32, 0);
            }
          }
        }
      }

      // DepartMent
      inputParam[3] = new DBParameter("@DepartMent", DbType.String, this.ultcbDepartment.Value.ToString());

      // Title
      if (this.txtTitle.Text.Length > 0)
      {
        inputParam[4] = new DBParameter("@Title", DbType.String, this.txtTitle.Text);
      }

      // Description
      if (this.txtDescription.Text.Length > 0)
      {
        inputParam[5] = new DBParameter("@Description", DbType.String, this.txtDescription.Text);
      }

      //Request Date
      DateTime requestDate = DateTime.MinValue;
      if (requestDate != null)
      {
        requestDate = (DateTime)ultRequestDate.Value;
      }
      inputParam[6] = new DBParameter("@RequestDate", DbType.DateTime, requestDate);

      //Expect Date
      DateTime expectDate = DateTime.MinValue;
      if (expectDate != null)
      {
        expectDate = (DateTime)ultExpectDate.Value;
      }
      inputParam[7] = new DBParameter("@ExpectDate", DbType.DateTime, expectDate);

      // Urgent Level
      inputParam[8] = new DBParameter("@UrgenLevel", DbType.Int32, DBConvert.ParseInt(this.ultcbUrgentLevel.Value.ToString()));

      // Program Module
      inputParam[9] = new DBParameter("@ProgramModule", DbType.Int32, DBConvert.ParseInt(this.ultcbProgramModule.Value.ToString()));

      // Type
      inputParam[10] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(this.ultcbType.Value.ToString()));

      // Description
      if (this.txtDescriptionIT.Text.Length > 0)
      {
        inputParam[11] = new DBParameter("@ITDescription", DbType.String, this.txtDescriptionIT.Text);
      }
      // ITType
      if (this.ultcbITType.Value != null && DBConvert.ParseInt(this.ultcbITType.Value.ToString()) != int.MinValue)
      {
        inputParam[12] = new DBParameter("@ITType", DbType.Int32, DBConvert.ParseInt(this.ultcbITType.Value.ToString()));
      }

      // CreateBy
      inputParam[13] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      // UpdateBy
      inputParam[14] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      //Description IT Confirm
      if (this.txtDescriptionITConfirm.Text.Length > 0)
      {
        inputParam[15] = new DBParameter("@DescriptionITConfirm", DbType.String, this.txtDescriptionITConfirm.Text);
      }
      if (this.txtRelativeDepartment.Text.Length > 0)
      {
        inputParam[16] = new DBParameter("@RelativeDepartment", DbType.String, this.txtRelativeDepartment.Text);
      }

      DBParameter[] outputParam = new DBParameter[2];
      outputParam[0] = new DBParameter("@RequestPid", DbType.Int64, long.MinValue);
      outputParam[1] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[1].Value.ToString());
      requestITPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save RequestITDetail
    /// </summary>
    /// <returns></returns>
    private bool SaveRequestITDetail()
    {
      string storeName = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["RowStatus"].Value.ToString()) == 1)
        {
          storeName = "spGNRRequestITDetail_Edit";
          UltraGridRow row = ultData.Rows[i];
          int ITUser = DBConvert.ParseInt(row.Cells["ITUser"].Value.ToString());
          double timeDoIT = DBConvert.ParseDouble(row.Cells["TimeDoIT"].Value.ToString());
          DateTime promiseDate = (DateTime)row.Cells["PromiseDate"].Value;
          DateTime startDate = DateTime.MinValue;
          if (row.Cells["StartDate"].Value.ToString().Length > 0)
          {
            startDate = (DateTime)row.Cells["StartDate"].Value;
          }
          DateTime finishDate = DateTime.MinValue;
          if (row.Cells["FinishDate"].Value.ToString().Length > 0)
          {
            finishDate = (DateTime)row.Cells["FinishDate"].Value;
          }

          int totalScreen = int.MinValue;
          int totalNumCode = int.MinValue;

          if (row.Cells["TotalScreen"].Value.ToString().Length > 0)
          {
            totalScreen = DBConvert.ParseInt(row.Cells["TotalScreen"].Value.ToString());
          }

          if (row.Cells["TotalNumCode"].Value.ToString().Length > 0)
          {
            totalNumCode = DBConvert.ParseInt(row.Cells["TotalNumCode"].Value.ToString());
          }

          DBParameter[] inputParam = new DBParameter[12];

          //Pid
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) >= 0)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          }

          //RequestITPid
          inputParam[1] = new DBParameter("@RequestITPid", DbType.Int64, this.requestITPid);

          //ITUser
          inputParam[2] = new DBParameter("@ITUser", DbType.Int32, ITUser);

          //TimeDoIT
          if (timeDoIT != Double.MinValue)
          {
            inputParam[3] = new DBParameter("@TimeDoIT", DbType.Double, timeDoIT);
          }

          //PromiseDate
          inputParam[4] = new DBParameter("@PromiseDate", DbType.DateTime, promiseDate);

          if (startDate != DateTime.MinValue)
          {
            //StartDate
            inputParam[5] = new DBParameter("@StartDate", DbType.DateTime, startDate);
          }

          //FinishDate
          if (finishDate != DateTime.MinValue)
          {
            inputParam[6] = new DBParameter("@FinishDate", DbType.DateTime, finishDate);
          }

          //TotalScreen
          if (totalScreen != int.MinValue)
          {
            inputParam[7] = new DBParameter("@TotalScreen", DbType.Int32, totalScreen);
          }

          //TotalNumCode
          if (totalNumCode != int.MinValue)
          {
            inputParam[8] = new DBParameter("@TotalNumCode", DbType.Int32, totalNumCode);
          }

          //CreateBy
          inputParam[9] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          //UpdateBy
          inputParam[10] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          //Remark
          inputParam[11] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Send Mail
    /// </summary>
    private void SendEmail()
    {
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      int i = 0;
      string symbol = " </br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp ";
      // Send Email
      if (this.status == 1 && this.chkConfirm.Checked)
      {
        Email email = new Email();
        email.Key = email.KEY_GNR_001;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string toFromSql = string.Empty;
          toFromSql = string.Format(arrList[0].ToString(), SharedObject.UserInfo.UserPid.ToString(), this.ultcbDepartment.Value.ToString());
          toFromSql = email.GetEmailToFromSql(toFromSql);

          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), this.txtRequestCode.Text, userName, this.txtTitle.Text);

          //HTML
          string desHTML = string.Empty;
          desHTML = DaiCo.Shared.Utility.FunctionUtility.BoDau(this.txtDescription.Text);

          string[] str = desHTML.Split('\n');
          string str2 = string.Empty;
          desHTML = string.Empty;

          for (i = 0; i < str.Length; i++)
          {
            str[i] = str[i].Trim();
            desHTML += str[i] + symbol;
          }
          desHTML = desHTML.Substring(0, desHTML.Length - symbol.Length);
          //HTML

          string body = string.Format(arrList[2].ToString(), desHTML);
          email.InsertEmail(email.Key, toFromSql, subject, body);
        }
      }
      else if (this.status == 2 && this.chkConfirm.Checked)
      {
        Email email = new Email();
        email.Key = email.KEY_GNR_002;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string idUser = string.Empty;
          for (i = 0; i < this.ultData.Rows.Count; i++)
          {
            idUser += this.ultData.Rows[i].Cells["ITUser"].Value.ToString() + ",";
          }
          idUser = idUser.Substring(0, idUser.Length - 1);
          commandText = "SELECT CreateBy FROM TblGNRRequestIT WHERE RequestCode = '" + this.txtRequestCode.Text + "'";
          DataTable dtMain = DataBaseAccess.SearchCommandTextDataTable(commandText);
          int createBy = int.MinValue;
          if (dtMain != null && dtMain.Rows.Count > 0)
          {
            createBy = DBConvert.ParseInt(dtMain.Rows[0][0].ToString());
          }

          string toFromSql = string.Empty;
          toFromSql = string.Format(arrList[0].ToString(), createBy, this.ultcbDepartment.Value.ToString(), idUser);
          toFromSql = email.GetEmailToFromSql(toFromSql);

          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), this.txtRequestCode.Text, userName, this.txtTitle.Text);

          //HTML
          string desHTML = string.Empty;
          desHTML = DaiCo.Shared.Utility.FunctionUtility.BoDau(this.txtDescription.Text);

          string[] str = desHTML.Split('\n');
          string str2 = string.Empty;
          desHTML = string.Empty;

          for (i = 0; i < str.Length; i++)
          {
            str[i] = str[i].Trim();
            desHTML += str[i] + symbol;
          }
          desHTML = desHTML.Substring(0, desHTML.Length - symbol.Length);

          string desITHTML = string.Empty;
          desITHTML = DaiCo.Shared.Utility.FunctionUtility.BoDau(this.txtDescriptionIT.Text);

          str = desITHTML.Split('\n');
          str2 = string.Empty;
          desITHTML = string.Empty;

          for (i = 0; i < str.Length; i++)
          {
            str[i] = str[i].Trim();
            desITHTML += str[i] + symbol;
          }
          desITHTML = desITHTML.Substring(0, desITHTML.Length - symbol.Length);
          //HTML

          string body = string.Format(arrList[2].ToString(), desHTML, desITHTML);
          email.InsertEmail(email.Key, toFromSql, subject, body);
        }
      }
      else if (this.status == 3)
      {
        Email email = new Email();
        email.Key = email.KEY_GNR_003;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string idUser = string.Empty;
          for (i = 0; i < this.ultData.Rows.Count; i++)
          {
            idUser += this.ultData.Rows[i].Cells["ITUser"].Value.ToString() + ",";
          }
          idUser = idUser.Substring(0, idUser.Length - 1);

          commandText = "SELECT CreateBy FROM TblGNRRequestIT WHERE RequestCode = '" + this.txtRequestCode.Text + "'";
          DataTable dtMain = DataBaseAccess.SearchCommandTextDataTable(commandText);
          int createBy = int.MinValue;
          if (dtMain != null && dtMain.Rows.Count > 0)
          {
            createBy = DBConvert.ParseInt(dtMain.Rows[0][0].ToString());
          }
          string toFromSql = string.Empty;
          toFromSql = string.Format(arrList[0].ToString(), createBy, this.ultcbDepartment.Value.ToString(), idUser);
          toFromSql = email.GetEmailToFromSql(toFromSql);

          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), this.txtRequestCode.Text, userName, this.txtTitle.Text);

          //HTML
          string desHTML = string.Empty;
          desHTML = DaiCo.Shared.Utility.FunctionUtility.BoDau(this.txtDescription.Text);

          string[] str = desHTML.Split('\n');
          string str2 = string.Empty;
          desHTML = string.Empty;

          for (i = 0; i < str.Length; i++)
          {
            str[i] = str[i].Trim();
            desHTML += str[i] + symbol;
          }
          desHTML = desHTML.Substring(0, desHTML.Length - symbol.Length);

          string desITHTML = string.Empty;
          desITHTML = DaiCo.Shared.Utility.FunctionUtility.BoDau(this.txtDescriptionIT.Text);

          str = desITHTML.Split('\n');
          str2 = string.Empty;
          desITHTML = string.Empty;

          for (i = 0; i < str.Length; i++)
          {
            str[i] = str[i].Trim();
            desITHTML += str[i] + symbol;
          }
          desITHTML = desITHTML.Substring(0, desITHTML.Length - symbol.Length);

          string desITConfirmHTML = string.Empty;
          desITConfirmHTML = DaiCo.Shared.Utility.FunctionUtility.BoDau(this.txtDescriptionITConfirm.Text);

          str = desITConfirmHTML.Split('\n');
          str2 = string.Empty;
          desITConfirmHTML = string.Empty;

          for (i = 0; i < str.Length; i++)
          {
            str[i] = str[i].Trim();
            desITConfirmHTML += str[i] + symbol;
          }
          desITConfirmHTML = desITConfirmHTML.Substring(0, desITConfirmHTML.Length - symbol.Length);
          //HTML

          string body = string.Format(arrList[2].ToString(), desHTML, desITHTML, desITConfirmHTML);
          email.InsertEmail(email.Key, toFromSql, subject, body);
        }
      }
    }

    /// <summary>
    /// Save RequestITUpload
    /// </summary>
    /// <returns></returns>
    private bool SaveRequestITUpload()
    {
      //Copy File 
      //System.IO.File.Copy(sourseFile, destFile, true);

      string storeName = string.Empty;
      DataTable dtMain = (DataTable)this.ultUploadFile.DataSource;
      foreach (DataRow row in dtMain.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (row.RowState == DataRowState.Added)
          {
            //Copy File
            System.IO.File.Copy(row["LocationFileLocal"].ToString(), row["LocationFile"].ToString(), true);
          }
          storeName = "spGNRRequestITUpload_Edit";
          DBParameter[] inputParam = new DBParameter[7];

          //Pid
          if (DBConvert.ParseLong(row["Pid"].ToString()) >= 0)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
          //RequestITPid
          if (this.requestITPid != long.MinValue)
          {
            inputParam[1] = new DBParameter("@RequestITPid", DbType.Int64, this.requestITPid);
          }

          inputParam[2] = new DBParameter("@FileName", DbType.String, 512, row["FileName"].ToString());

          inputParam[3] = new DBParameter("@LocationFile", DbType.String, 512, row["LocationFile"].ToString());

          inputParam[4] = new DBParameter("@Remark", DbType.String, 4000, row["Remark"].ToString());

          inputParam[5] = new DBParameter("@File", DbType.String, row["File"].ToString());

          inputParam[6] = new DBParameter("@TypeUpload", DbType.Int32, 0);

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save RequestITUpload
    /// </summary>
    /// <returns></returns>
    private bool SaveRequestITUploadFileTemplateConfirmed()
    {
      //Copy File 
      //System.IO.File.Copy(sourseFile, destFile, true);

      string storeName = string.Empty;
      DataTable dtMain = (DataTable)this.ultITUploadFile.DataSource;
      foreach (DataRow row in dtMain.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (row.RowState == DataRowState.Added)
          {
            //Copy File
            System.IO.File.Copy(row["LocationFileLocal"].ToString(), row["LocationFile"].ToString(), true);
          }
          storeName = "spGNRRequestITUpload_Edit";
          DBParameter[] inputParam = new DBParameter[7];

          //Pid
          if (DBConvert.ParseLong(row["Pid"].ToString()) >= 0)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
          //RequestITPid
          if (this.requestITPid != long.MinValue)
          {
            inputParam[1] = new DBParameter("@RequestITPid", DbType.Int64, this.requestITPid);
          }

          inputParam[2] = new DBParameter("@FileName", DbType.String, 512, row["FileName"].ToString());

          inputParam[3] = new DBParameter("@LocationFile", DbType.String, 512, row["LocationFile"].ToString());

          inputParam[4] = new DBParameter("@Remark", DbType.String, 4000, row["Remark"].ToString());

          inputParam[5] = new DBParameter("@File", DbType.String, 256, row["File"].ToString());

          inputParam[6] = new DBParameter("@TypeUpload", DbType.Int32, 1);

          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// GetRandomString
    /// </summary>
    /// <returns></returns>
    private string GetRandomString()
    {
      StringBuilder sBuilder = new StringBuilder();
      Random random = new Random();
      char ch;
      for (int i = 0; i < 10; i++)
      {
        ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
        sBuilder.Append(ch);
      }
      return sBuilder.ToString() + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + '_' + DateTime.Now.Ticks.ToString();
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      if (this.checkdepartment == 1 && this.status >= 0)
      {
        DataTable dtMain = (DataTable)this.ultData.DataSource;
        if (dtMain == null || dtMain.Rows.Count == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "IT Staff");
          return;
        }
      }
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      // Load Data
      this.LoadData();

      // Send Email
      this.SendEmail();
    }
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RequestITPid"].Hidden = true;
      e.Layout.Bands[0].Columns["RowStatus"].Hidden = true;

      e.Layout.Bands[0].Columns["ITUser"].Header.Caption = "IT Staff";
      e.Layout.Bands[0].Columns["TimeDoIt"].Header.Caption = "Time Do It(h)";
      e.Layout.Bands[0].Columns["PromiseDate"].Header.Caption = "Promise Date";
      e.Layout.Bands[0].Columns["StartDate"].Header.Caption = "Start Date";
      e.Layout.Bands[0].Columns["FinishDate"].Header.Caption = "Finish Date";
      e.Layout.Bands[0].Columns["TotalScreen"].Header.Caption = "Total Screen";
      e.Layout.Bands[0].Columns["TotalNumCode"].Header.Caption = "Total Num Code";

      e.Layout.Bands[0].Columns["ITUser"].ValueList = ultDDITStaff;

      e.Layout.Bands[0].Columns["PromiseDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["StartDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["FinishDate"].Format = ConstantClass.FORMAT_DATETIME;

      e.Layout.Bands[0].Columns["PromiseDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["StartDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["FinishDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      e.Layout.Bands[0].Columns["TimeDoIt"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalScreen"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalNumCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["ITUser"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["ITUser"].MinWidth = 150;
      e.Layout.Bands[0].Columns["TimeDoIt"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["TimeDoIt"].MinWidth = 100;
      e.Layout.Bands[0].Columns["PromiseDate"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["PromiseDate"].MinWidth = 100;
      e.Layout.Bands[0].Columns["StartDate"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["StartDate"].MinWidth = 100;
      e.Layout.Bands[0].Columns["FinishDate"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["FinishDate"].MinWidth = 100;
      e.Layout.Bands[0].Columns["TotalScreen"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["TotalScreen"].MinWidth = 100;
      e.Layout.Bands[0].Columns["TotalNumCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["TotalNumCode"].MinWidth = 100;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;

    }
    /// <summary>
    /// Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string strColName = e.Cell.Column.ToString();

      if (string.Compare(strColName, "ITUser") == 0)
      {
        string commandText = string.Format(@"SELECT count(*) 
                                    FROM VHRMEmployee  
                                    WHERE CONVERT(VARCHAR, Pid) + ' - ' + EmpName = N'{0}'", e.Cell.Row.Cells["ITUser"].Text.ToString());

        if (DBConvert.ParseInt(DataBaseAccess.SearchCommandTextDataTable(commandText).Rows[0][0].ToString()) == 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Employee");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
        }
      }
      else if (string.Compare(strColName, "TimeDoIt") == 0)
      {
        if (e.Cell.Row.Cells["TimeDoIt"].Text.Length > 0)
        {
          if (DBConvert.ParseDouble(e.Cell.Row.Cells["TimeDoIt"].Text) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Time Do It");
            e.Cancel = true;
          }
        }
      }
      else if (string.Compare(strColName, "TotalScreen") == 0)
      {
        if (e.Cell.Row.Cells["TotalScreen"].Text.Length > 0)
        {
          if (DBConvert.ParseInt(e.Cell.Row.Cells["TotalScreen"].Text) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Total Screen");
            e.Cancel = true;
          }
        }
      }
      else if (string.Compare(strColName, "TotalNumCode") == 0)
      {
        if (e.Cell.Row.Cells["TotalNumCode"].Text.Length > 0)
        {
          if (DBConvert.ParseInt(e.Cell.Row.Cells["TotalNumCode"].Text) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Total Num Code");
            e.Cancel = true;
          }
        }
      }
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      e.Cell.Row.Cells["RowStatus"].Value = 1;
      try
      {
        e.Cell.Row.ParentRow.Cells["RowStatus"].Value = 1;
      }
      catch
      {
      }
    }

    /// <summary>
    /// Before Row Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.requestITPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
          {
            DBParameter[] inputParams = new DBParameter[1];
            inputParams[0] = new DBParameter("@RequestITPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            string storeName = string.Empty;
            storeName = "spGNRRequestITDetail_Delete";
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

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Delete Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.requestITPid == long.MinValue)
      {
        return;
      }
      if (WindowUtinity.ShowMessageConfirm("MSG0007", "Request IT").ToString() == "No")
      {
        return;
      }
      if (requestITPid != long.MinValue)
      {
        string storeName = "spGNRRequestIT_Delete";
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@Pid", DbType.Int64, requestITPid);

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
          WindowUtinity.ShowMessageError("ERR0093", "Request IT");
          return;
        }
      }
      this.btnDelete.Visible = false;
      this.CloseTab();
    }

    /// <summary>
    /// Finish Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFinish_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["FinishDate"].Value.ToString().Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Finish Date");
          return;
        }
        else
        {
          DateTime finishDate = (DateTime)this.ultData.Rows[i].Cells["FinishDate"].Value;
          DateTime startDate = (DateTime)this.ultData.Rows[i].Cells["StartDate"].Value;
          int result = DateTime.Compare(finishDate, startDate);
          if (result < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Finish date >= Start date");
            return;
          }
        }
      }
      // Save Data
      bool success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // Load Data
      this.LoadData();
      // Send Mail
      //this.SendEmail();
    }

    private void ultUploadFile_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultUploadFile);
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RequestITPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFile"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFileLocal"].Hidden = true;
      e.Layout.Bands[0].Columns["File"].Hidden = true;

      e.Layout.Bands[0].Columns["FileName"].Header.Caption = "File Name";

      e.Layout.Bands[0].Columns["Type"].MaxWidth = 25;
      e.Layout.Bands[0].Columns["Type"].MinWidth = 25;
      e.Layout.Bands[0].Columns["FileName"].MaxWidth = 250;
      e.Layout.Bands[0].Columns["FileName"].MinWidth = 250;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    /// <summary>
    /// Browse
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBrowse_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnUpload.Enabled = (txtLocation.Text.Trim().Length > 0);
    }

    /// <summary>
    /// Upload File
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnUpload_Click(object sender, EventArgs e)
    {
      if (this.txtLocation.Text.Trim().Length > 0)
      {
        string file = txtLocation.Text;
        FileInfo f = new FileInfo(file);
        long fLength = f.Length;
        if (fLength < 5120000)
        {
          string extension = System.IO.Path.GetExtension(file).ToLower();
          string typeFile = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE Value = '" + extension + "' AND [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_TYPEFILEUPLOAD;
          DataTable dtTypeFile = DataBaseAccess.SearchCommandTextDataTable(typeFile);
          if (dtTypeFile != null && dtTypeFile.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dtTypeFile.Rows[0][0].ToString()) > 0)
            {
              string fileName1 = System.IO.Path.GetFileName(file).ToString();
              string fileName = System.IO.Path.GetFileNameWithoutExtension(file).ToString()
                                      + DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd"))
                                      + DBConvert.ParseString(DateTime.Now.Ticks)
                                      + System.IO.Path.GetExtension(file);

              string sourcePath = System.IO.Path.GetDirectoryName(file);
              string commandText = string.Empty;
              commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 1);
              DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              string targetPath = string.Empty;
              if (dt != null && dt.Rows.Count > 0)
              {
                targetPath = dt.Rows[0][0].ToString();
              }

              sourseFile = System.IO.Path.Combine(sourcePath, fileName1);
              destFile = System.IO.Path.Combine(targetPath, fileName);
              if (!System.IO.Directory.Exists(targetPath))
              {
                System.IO.Directory.CreateDirectory(targetPath);
              }
              DataTable dtSource = (DataTable)ultUploadFile.DataSource;
              int i = dtSource.Rows.Count;
              foreach (DataRow row1 in dtSource.Rows)
              {
                if (row1.RowState == DataRowState.Deleted)
                {
                  i = i - 1;
                }
              }
              DataRow row = dtSource.NewRow();
              row["FileName"] = fileName1;
              row["LocationFile"] = destFile;
              row["LocationFileLocal"] = sourseFile;
              dtSource.Rows.Add(row);
              if (String.Compare(extension, ".docx") == 0 || String.Compare(extension, ".doc") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "word.bmp");
                row["File"] = targetPath + "word.bmp";
              }
              else if (string.Compare(extension, ".xls") == 0 || string.Compare(extension, ".xlsx") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "xls.bmp");
                row["File"] = targetPath + "xls.bmp";
              }
              else if (string.Compare(extension, ".pdf") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "pdf.bmp");
                row["File"] = targetPath + "pdf.bmp";
              }
              else if (string.Compare(extension, ".txt") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "notepad.bmp");
                row["File"] = targetPath + "notepad.bmp";
              }
              else if (string.Compare(extension, ".gif") == 0
                        || string.Compare(extension, ".jpg") == 0
                        || string.Compare(extension, ".bmp") == 0
                        || string.Compare(extension, ".png") == 0)
              {
                this.ultUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "image.bmp");
                row["File"] = targetPath + "image.bmp";
              }
              this.btnUpload.Enabled = false;
              this.chkUpload.Checked = true;
            }
            else
            {
              WindowUtinity.ShowMessageError("ERR0001", "Type File Not UPload");
            }
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", "File Upload < 5Mb");
        }
      }
    }
    /// <summary>
    /// Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

      if (DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) == int.MinValue)
      {
        prc.StartInfo = new ProcessStartInfo(row.Cells["LocationFileLocal"].Value.ToString());
      }
      else
      {
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string folder = string.Format(@"{0}\Temporary", startupPath);
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }
        string locationFile = row.Cells["LocationFile"].Value.ToString();
        if (File.Exists(locationFile))
        {
          string newLocationFile = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(row.Cells["LocationFile"].Value.ToString()));
          if (File.Exists(newLocationFile))
          {
            try
            {
              File.Delete(newLocationFile);
            }
            catch
            {
              WindowUtinity.ShowMessageWarningFromText("File Is Opening!");
              return;
            }
          }
          File.Copy(locationFile, newLocationFile);
          prc.StartInfo = new ProcessStartInfo(newLocationFile);
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

    /// <summary>
    /// before row deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultUploadFile_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.requestITPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
          {
            DBParameter[] inputParams = new DBParameter[1];
            inputParams[0] = new DBParameter("@RequestITPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            string storeName = string.Empty;
            storeName = "spGNRRequestITUpload_Delete";
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

    /// <summary>
    /// Check Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkUpload_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkUpload.Checked)
      {
        this.ultUploadFile.Visible = true;
      }
      else
      {
        this.ultUploadFile.Visible = false;
      }
    }

    /// <summary>
    /// IT Brown Upload
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnITBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      txtLocationITUpload.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnITUpload.Enabled = (txtLocationITUpload.Text.Trim().Length > 0);
    }

    /// <summary>
    /// IT Upload File Report Confirmed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnITUpload_Click(object sender, EventArgs e)
    {
      if (this.txtLocationITUpload.Text.Trim().Length > 0)
      {
        string file = txtLocationITUpload.Text;
        FileInfo f = new FileInfo(file);
        long fLength = f.Length;
        if (fLength < 5120000)
        {
          string extension = System.IO.Path.GetExtension(file).ToLower();
          string typeFile = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE Value = '" + extension + "' AND [Group] = " + Shared.Utility.ConstantClass.GROUP_GNR_TYPEFILEUPLOAD;
          DataTable dtTypeFile = DataBaseAccess.SearchCommandTextDataTable(typeFile);
          if (dtTypeFile != null && dtTypeFile.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dtTypeFile.Rows[0][0].ToString()) > 0)
            {
              string fileName1 = System.IO.Path.GetFileName(file).ToString();
              string fileName = System.IO.Path.GetFileNameWithoutExtension(file).ToString()
                                      + DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd"))
                                      + DBConvert.ParseString(DateTime.Now.Ticks)
                                      + System.IO.Path.GetExtension(file);

              string sourcePath = System.IO.Path.GetDirectoryName(file);
              string commandText = string.Empty;
              commandText = String.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code = {1}", Shared.Utility.ConstantClass.GROUP_GNR_PATHFILEUPLOAD, 2);
              DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              string targetPath = string.Empty;
              if (dt != null && dt.Rows.Count > 0)
              {
                targetPath = dt.Rows[0][0].ToString();
              }

              sourseFile = System.IO.Path.Combine(sourcePath, fileName1);
              destFile = System.IO.Path.Combine(targetPath, fileName);
              if (!System.IO.Directory.Exists(targetPath))
              {
                System.IO.Directory.CreateDirectory(targetPath);
              }
              DataTable dtSource = (DataTable)ultITUploadFile.DataSource;
              int i = dtSource.Rows.Count;
              foreach (DataRow row1 in dtSource.Rows)
              {
                if (row1.RowState == DataRowState.Deleted)
                {
                  i = i - 1;
                }
              }
              DataRow row = dtSource.NewRow();
              row["FileName"] = fileName1;
              row["LocationFile"] = destFile;
              row["LocationFileLocal"] = sourseFile;
              dtSource.Rows.Add(row);
              if (String.Compare(extension, ".docx") == 0 || String.Compare(extension, ".doc") == 0)
              {
                this.ultITUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "word.bmp");
                row["File"] = targetPath + "word.bmp";
              }
              else if (string.Compare(extension, ".xls") == 0 || string.Compare(extension, ".xlsx") == 0)
              {
                this.ultITUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "xls.bmp");
                row["File"] = targetPath + "xls.bmp";
              }
              else if (string.Compare(extension, ".pdf") == 0)
              {
                this.ultITUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "pdf.bmp");
                row["File"] = targetPath + "pdf.bmp";
              }
              else if (string.Compare(extension, ".txt") == 0)
              {
                this.ultITUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "notepad.bmp");
                row["File"] = targetPath + "notepad.bmp";
              }
              else if (string.Compare(extension, ".gif") == 0
                        || string.Compare(extension, ".jpg") == 0
                        || string.Compare(extension, ".bmp") == 0
                        || string.Compare(extension, ".png") == 0)
              {
                this.ultITUploadFile.Rows[i].Cells["Type"].Appearance.Image = Image.FromFile(targetPath + "image.bmp");
                row["File"] = targetPath + "image.bmp";
              }
              this.btnITUpload.Enabled = false;
            }
            else
            {
              WindowUtinity.ShowMessageError("ERR0001", "Type File Not UPload");
            }
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", "File Upload < 5Mb");
        }
      }
    }

    private void ultITUploadFile_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultITUploadFile.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultITUploadFile.Selected.Rows[0];
      Process prc = new Process();

      if (DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) == int.MinValue)
      {
        prc.StartInfo = new ProcessStartInfo(row.Cells["LocationFileLocal"].Value.ToString());
      }
      else
      {
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string folder = string.Format(@"{0}\Temporary", startupPath);
        if (!Directory.Exists(folder))
        {
          Directory.CreateDirectory(folder);
        }
        string locationFile = row.Cells["LocationFile"].Value.ToString();
        if (File.Exists(locationFile))
        {
          string newLocationFile = string.Format(@"{0}\{1}", folder, System.IO.Path.GetFileName(row.Cells["LocationFile"].Value.ToString()));
          if (File.Exists(newLocationFile))
          {
            try
            {
              File.Delete(newLocationFile);
            }
            catch
            {
              WindowUtinity.ShowMessageWarningFromText("File Is Opening!");
              return;
            }
          }
          File.Copy(locationFile, newLocationFile);
          prc.StartInfo = new ProcessStartInfo(newLocationFile);
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

    private void ultITUploadFile_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultITUploadFile);
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RequestITPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFile"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationFileLocal"].Hidden = true;
      e.Layout.Bands[0].Columns["File"].Hidden = true;

      e.Layout.Bands[0].Columns["FileName"].Header.Caption = "File Name";

      e.Layout.Bands[0].Columns["Type"].MaxWidth = 25;
      e.Layout.Bands[0].Columns["Type"].MinWidth = 25;
      e.Layout.Bands[0].Columns["FileName"].MaxWidth = 250;
      e.Layout.Bands[0].Columns["FileName"].MinWidth = 250;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultITUploadFile_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      if (this.requestITPid != long.MinValue)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
          {
            DBParameter[] inputParams = new DBParameter[1];
            inputParams[0] = new DBParameter("@RequestITPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            string storeName = string.Empty;
            storeName = "spGNRRequestITUpload_Delete";
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

    private void btnPrint_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.requestITPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spGNRRequestITInformationByRequestITPid_Select", inputParam);
      dsGNRITRequestDetail ds = new dsGNRITRequestDetail();
      ds.Tables["RequestIT"].Merge(dsSource.Tables[0]);
      ds.Tables["RequestDetail"].Merge(dsSource.Tables[4]);
      if (ds.Tables["RequestDetail"].Rows.Count == 0)
      {
        DataRow row = ds.Tables["RequestDetail"].NewRow();
        row["RequestPid"] = this.requestITPid;
        ds.Tables["RequestDetail"].Rows.Add(row);
      }
      DaiCo.Shared.View_Report report = null;
      cptGNRITRequest cpt = new cptGNRITRequest();
      cpt.SetDataSource(ds);
      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      string random = GetRandomString();
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string filePath = string.Format(@"{0}\\Reports\RequestIT{1}_{2}.pdf", startupPath, requestITPid, random);
      cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);
      Process.Start(filePath);
    }

    private void ultraGridInformation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PersonIncharge"].Header.Caption = "Person Incharge";
      e.Layout.Bands[0].Columns["PersonIncharge"].ValueList = ultDDPerson;
      e.Layout.Bands[0].Columns["TaskNo"].Header.Caption = "Task No";
      e.Layout.Bands[0].Columns["TaskName"].Header.Caption = "Task Name";
      e.Layout.Bands[0].Columns["Time"].Header.Caption = "Time(h)";
      e.Layout.Bands[0].Columns["Time"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Time"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Kind"].Hidden = true;
      e.Layout.Bands[1].Columns["TaskDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Screen"].Header.Caption = "Component";
      e.Layout.Bands[1].Columns["TimeDetail"].Header.Caption = "Component Time";
      e.Layout.Bands[1].Columns["TimeDetail"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Screen"].ValueList = ultDDScreenTime;

      e.Layout.Bands[0].Columns["TaskNo"].CellAppearance.BackColor = Color.LightSkyBlue;
      e.Layout.Bands[0].Columns["PersonIncharge"].CellAppearance.BackColor = Color.LightSkyBlue;
      e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.LightSkyBlue;
      e.Layout.Bands[0].Columns["TaskName"].CellAppearance.BackColor = Color.LightSkyBlue;

      e.Layout.Bands[1].Columns["Screen"].CellAppearance.BackColor = Color.LightSkyBlue;
      e.Layout.Bands[1].Columns["TimeDetail"].CellAppearance.BackColor = Color.LightSkyBlue;
      e.Layout.Bands[1].Columns["Remark"].CellAppearance.BackColor = Color.LightSkyBlue;

      if (this.status == 3)
      {
        e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
        e.Layout.Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
      }
      else
      {
        e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      }
    }

    private void ultraCBProject_ValueChanged(object sender, EventArgs e)
    {
      long pid = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultraCBProject));
      this.LoadTotalTimeForRequest(pid);
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      foreach (UltraGridRow row in e.Rows)
      {
        // Task Parent
        if (row.ParentRow == null)
        {
          this.NeedToSave = true;
          long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (pid != long.MinValue)
          {
            this.pidDetailCoding.Add(pid);
          }
        }
        else
        {
          // Task Detail
          this.NeedToSave = true;
          long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (pid != long.MinValue)
          {
            this.pidScreenTime.Add(pid);
          }
        }
      }
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "TaskNo":
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) == long.MinValue)
          {
            taskPid = taskPid - 1;
            row.Cells["Pid"].Value = taskPid;
          }
          break;
        case "Screen":
          if (row.Cells["Screen"].Text.Length > 0 && DBConvert.ParseInt(row.Cells["Screen"].Value.ToString()) != int.MinValue)
          {
            row.Cells["TimeDetail"].Value = DBConvert.ParseDouble(ultDDScreenTime.SelectedRow.Cells["Time"].Value.ToString());
            if (DBConvert.ParseInt(ultDDScreenTime.SelectedRow.Cells["Kind"].Value.ToString()) == 1)
            {
              row.Cells["TimeDetail"].Activation = Activation.ActivateOnly;
            }
            else
            {
              row.Cells["TimeDetail"].Activation = Activation.AllowEdit;
            }
          }
          else
          {
            row.Cells["TimeDetail"].Value = DBNull.Value;
          }
          break;
        case "TimeDetail":
          UltraGridRow rowParent = row.ParentRow;
          double totalTime = 0;
          for (int i = 0; i < rowParent.ChildBands[0].Rows.Count; i++)
          {
            if(DBConvert.ParseDouble(rowParent.ChildBands[0].Rows[i].Cells["TimeDetail"].Value.ToString()) > 0)
            {
              totalTime = totalTime + DBConvert.ParseDouble(rowParent.ChildBands[0].Rows[i].Cells["TimeDetail"].Value.ToString());
            }
          }
          rowParent.Cells["Time"].Value = totalTime;
          break;
        default:
          break;
      }
    }

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName)
      {
        case "Screen":
          if(text.Trim().Length > 0 && ultDDScreenTime.SelectedRow == null)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Screen");
            e.Cancel = true;
          }
          break;
        case "TimeDetail":
          if(text.Trim().Length > 0 && DBConvert.ParseDouble(text) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Time Detail");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }
    #endregion Event
  }
}
