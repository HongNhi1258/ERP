/*
  Author      : 
  Date        : 06/04/02011
  Description : Insert PR, Update PR
  Update      : Truong Them Pham Insert Detail PR Tren Luoi
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Purchasing;
using Purchasing.DataSetSource;
using Purchasing.Reports;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_03_001 : MainUserControl
  {
    #region Field
    // Format Date User
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    // PR No
    public string prNo = string.Empty;
    private bool isPurchaseManager = false;
    private bool isPROnline = false;
    private long groupStaff = long.MinValue;
    // New
    private int STT_PRDETAIL_NEW = 0;

    // Group Purchase Approved
    private int STT_PRDETAIL_HEADDEPAPP = 3;

    // Return Group Purchase Approved
    private int STT_PRDETAIL_RTNGROUPPUR = 4;

    // Return Purchase Manager Approved
    private int STT_PRDETAIL_RTNPURMAN = 6;

    // Status Control
    private bool nonConfirm = true;
    private bool canUpdate = false;
    private string pathTemplate = string.Empty;
    private string pathExport = string.Empty;
    private long curID = 0;
    private Double exchangeRate = 0;

    private string strPathOutputFile;
    private string strPathTemplate;

    // Status
    int status = int.MinValue;

    double totalPrice = double.MinValue;
    // Check ExchangeRate In After Cell Update
    bool flag = true;
    #endregion Field

    #region Init Data
    /// <summary>
    /// 
    /// </summary>
    public viewPUR_03_001()
    {
      InitializeComponent();
      drpRequestDate.FormatString = ConstantClass.FORMAT_DATETIME;
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_03_001_Load(object sender, EventArgs e)
    {
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      this.strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      this.strPathTemplate = strStartupPath + @"\Template"; ;
      // Check PROnline
      string commandText = "SELECT PROnlineNo FROM TblPURPROnlineInformation WHERE PROnlineNo = '" + this.prNo + "'";
      DataTable dtCheckPROnline = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheckPROnline != null && dtCheckPROnline.Rows.Count == 1)
      {
        this.isPROnline = true;
      }
      // Load Department
      this.LoadComboDepartment();

      // Load DepartmentReceived
      this.LoadComboDepartmentReceived();

      // Load Type Of Request
      this.LoadComboTypeRequest();

      // Check User
      this.CheckUser();

      // Load Currency 
      this.LoadComboCurrency();
      this.LoadComboExchangeRate();
      this.LoadDropDownMaterial();
      this.LoadDropDownUrgentLevel();
      this.LoadDropDownLocalImport();
      this.LoadDropDownCurrency();
      //this.LoadDropDownProjectCode();
      this.LoadDropDownSupplier();
      this.LoadDropDownReasonLate();

      // Load Data
      this.LoadData();
    }

    private void LoadDropDownMaterial()
    {
      string commandText = "SELECT MaterialCode, MaterialCode + ' - ' + NameEN Name FROM TblGNRMaterialInformation WHERE [Status] >= 1 AND [Status] != 3";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDMaterials.DataSource = dtSource;
      ultDDMaterials.DisplayMember = "MaterialCode";
      ultDDMaterials.ValueMember = "MaterialCode";
      ultDDMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDMaterials.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      //ultDDMaterials.DisplayLayout.Bands[0].Columns["Name"].Hidden = true;
    }

    private void LoadDropDownLocalImport()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7006 AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDLocalImport.DataSource = dtSource;
      ultDDLocalImport.DisplayMember = "Name";
      ultDDLocalImport.ValueMember = "Code";
      ultDDLocalImport.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDLocalImport.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDDLocalImport.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void LoadDropDownUrgentLevel()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7007 AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDUrgentLevel.DataSource = dtSource;
      ultDDUrgentLevel.DisplayMember = "Name";
      ultDDUrgentLevel.ValueMember = "Code";
      ultDDUrgentLevel.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDUrgentLevel.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDDUrgentLevel.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void LoadDropDownSupplier()
    {
      string commandText = string.Format(@"SELECT Pid, EnglishName FROM TblPURSupplierInfo ORDER BY EnglishName");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDSupplier.DataSource = dtSource;
      ultDDSupplier.DisplayMember = "EnglishName";
      ultDDSupplier.ValueMember = "Pid";
      ultDDSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDSupplier.DisplayLayout.Bands[0].Columns["EnglishName"].Width = 250;
      ultDDSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    private void LoadDropDownReasonLate()
    {
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 9226");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDReasonLate.DataSource = dtSource;
      ultDDReasonLate.DisplayMember = "Value";
      ultDDReasonLate.ValueMember = "Code";
      ultDDReasonLate.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDReasonLate.DisplayLayout.Bands[0].Columns["Value"].Width = 250;
      ultDDReasonLate.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void LoadDropDownCurrency()
    {
      string commandText = "SELECT Pid, CurrentExchangeRate, [Code] + ' - ' + CAST(CurrentExchangeRate AS VARCHAR) Name FROM TblPURCurrencyInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDCurrency.DataSource = dtSource;
      ultDDCurrency.DisplayMember = "Name";
      ultDDCurrency.ValueMember = "Pid";
      ultDDCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDCurrency.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDDCurrency.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultDDCurrency.DisplayLayout.Bands[0].Columns["CurrentExchangeRate"].Hidden = true;
    }

    private void LoadComboCurrency()
    {
      string commandText = "SELECT Pid, CurrentExchangeRate,[Code] Name FROM TblPURCurrencyInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      udrpCurrency.DataSource = dtSource;
      udrpCurrency.DisplayMember = "Name";
      udrpCurrency.ValueMember = "Pid";
      udrpCurrency.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpCurrency.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      udrpCurrency.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      udrpCurrency.DisplayLayout.Bands[0].Columns["CurrentExchangeRate"].Hidden = true;
    }

    private void LoadComboExchangeRate()
    {
      string commandText = "SELECT Pid, CurrentExchangeRate,[Code] Name FROM TblPURCurrencyInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDExchangeRate.DataSource = dtSource;
      ultDDExchangeRate.DisplayMember = "Name";
      ultDDExchangeRate.ValueMember = "Pid";
      ultDDExchangeRate.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDExchangeRate.DisplayLayout.Bands[0].Columns["Name"].Width = 100;
      ultDDExchangeRate.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultDDExchangeRate.DisplayLayout.Bands[0].Columns["CurrentExchangeRate"].Hidden = true;
    }

    private void LoadDropDownProjectCode()
    {
      string commandText = string.Format(@" SELECT Pid Code, ProjectCode Name 
                                            FROM TblPURPRProjectCode 
                                            WHERE ISNULL(Status, 0) = 1 AND Department = '{0}'

                                            UNION 
                                            SELECT	PJ.Pid, PJ.ProjectCode
                                            FROM	TblPURPRDetail PRD
	                                            INNER JOIN TblPURPRProjectCode PJ ON PJ.Pid = PRD.ProjectPid
                                            WHERE	PRNo = '{1}'
                                            ", ultDepartment.Value.ToString(), prNo);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDProjectCode.DataSource = dtSource;
      ultDDProjectCode.DisplayMember = "Name";
      ultDDProjectCode.ValueMember = "Code";
      ultDDProjectCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDProjectCode.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDDProjectCode.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Check User
    /// </summary>
    private void CheckUser()
    {
      string commandText = "SELECT Manager FROM VHRDDepartmentInfo WHERE CODE = 'PUR'";
      System.Data.DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dt != null && dt.Rows.Count > 0)
      {
        if (SharedObject.UserInfo.UserPid == DBConvert.ParseInt(dt.Rows[0]["Manager"].ToString()))
        {
          this.isPurchaseManager = true;
        }
      }

      if (this.isPurchaseManager == false)
      {
        commandText = "SELECT Pid FROM TblPURStaffGroup WHERE LeaderGroup = " + SharedObject.UserInfo.UserPid;
        dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

        if (dt != null && dt.Rows.Count > 0)
        {
          groupStaff = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
        }
        else
        {
          commandText = "SELECT [Group] FROM TblPURStaffGroupDetail WHERE Employee = " + SharedObject.UserInfo.UserPid;
          dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt != null && dt.Rows.Count > 0)
          {
            groupStaff = DBConvert.ParseLong(dt.Rows[0]["Group"].ToString());
          }
        }
      }
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment ORDER BY Department";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "Name";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartmentReceived()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment ORDER BY Department";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBDepartmentRec.DataSource = dtSource;
      ultCBDepartmentRec.DisplayMember = "Name";
      ultCBDepartmentRec.ValueMember = "Department";
      ultCBDepartmentRec.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBDepartmentRec.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultCBDepartmentRec.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Type Of Request
    /// </summary>
    private void LoadComboTypeRequest()
    {
      string commandText = "SELECT Code, Value Name FROM TblBOMCodeMaster WHERE [Group] = 7005 AND DeleteFlag = 0";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultTypeRequest.DataSource = dtSource;
      ultTypeRequest.DisplayMember = "Name";
      ultTypeRequest.ValueMember = "Code";
      ultTypeRequest.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultTypeRequest.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultTypeRequest.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo PR NO
    /// </summary>
    private void LoadComboPR()
    {
      string commandText = string.Empty;
      commandText += "  SELECT PRNo, PRNo + ' ' + CONVERT(VARCHAR, CreateDate, 103) Name ";
      commandText += "  FROM TblPURPRInformation PR ";
      commandText += "  WHERE [Status] >= 7 ";
      commandText += "  ORDER BY PRNo DESC ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultPRNo.DataSource = dtSource;
      ultPRNo.DisplayMember = "Name";
      ultPRNo.ValueMember = "PRNo";
      ultPRNo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultPRNo.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultPRNo.DisplayLayout.Bands[0].Columns["PRNo"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Requester
    /// </summary>
    /// <param name="group"></param>
    private void LoadComboRequester(string department)
    {
      string commandText = string.Format("SELECT ID_NhanVien, TenNV + ' ' + HoNV + ' - ' + CAST(ID_NhanVien AS VARCHAR) Name FROM VHRNhanVien ORDER BY Name");
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);
      ultRequester.DataSource = dtSource;
      ultRequester.DisplayMember = "Name";
      ultRequester.ValueMember = "ID_NhanVien";
      ultRequester.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultRequester.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultRequester.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      this.flag = false;

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PRNo", DbType.AnsiString, 16, this.prNo) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPURPRInformationByPRNo", 300, inputParam);
      System.Data.DataTable dtPR = dsSource.Tables[0];
      //int status = int.MinValue;

      if (dtPR.Rows.Count > 0)
      {
        DataRow row = dtPR.Rows[0];

        this.txtPRNo.Text = this.prNo;
        this.ultCreateDate.Value = DBConvert.ParseDateTime(row["CreateDate"].ToString(), ConstantClass.FORMAT_DATETIME);
        this.ultRequester.Value = DBConvert.ParseInt(row["RequestBy"].ToString());
        this.ultDepartment.Value = row["Department"].ToString();
        this.txtPurposePR.Text = row["PurposeOfRequisition"].ToString();
        this.ultTypeRequest.Value = DBConvert.ParseInt(row["TypeOfRequest"].ToString());
        this.txtRemark.Text = row["Remark"].ToString();
        this.ultCBDepartmentRec.Value = row["DepartmentReceived"].ToString();
        this.nonConfirm = (DBConvert.ParseInt(row["Status"].ToString()) == 0);
        if (DBConvert.ParseInt(row["Status"].ToString()) != 0)
        {
          this.chkLock.Checked = true;
        }
        this.status = DBConvert.ParseInt(row["Status"].ToString());
        string commandText = "SELECT Value FROM TblBOMCodeMaster WHERE [GROUP] = 7010 AND Code = " + status;
        System.Data.DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          this.lblStatus.Text = dt.Rows[0][0].ToString();
        }
        this.btnAddDetail.Enabled = true;
        this.chkLock.Enabled = true;
        this.chkAdddition.Enabled = true;
        this.txtPRNo.ReadOnly = true;
        // total Price
        this.totalPrice = DBConvert.ParseDouble(row["ScheduleTotalMoney"].ToString());
      }
      else
      {
        this.ultCreateDate.Value = DBNull.Value;
        this.lblStatus.Text = "New";
        this.txtPRNo.Text = "PR-";
      }

      if (this.prNo.Length != 0)
      {
        this.LoadComboPR();

        this.LoadDataPRDetail(dsSource);

        // Set status control
        if (status > 10)
        {
          this.chkLock.Enabled = false;
          btnSave.Enabled = false;
          ultCBDepartmentRec.Enabled = false;
        }

        this.SetStatusControl();
      }
      // Truong Add
      this.flag = true;
      this.LoadDataPRDetail(dsSource);
      // End
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      this.canUpdate = (btnSave.Visible && this.nonConfirm);

      this.ultDepartment.Enabled = false;
      this.ultRequester.Enabled = false;
      this.ultCreateDate.Enabled = false;
      this.ultTypeRequest.Enabled = this.canUpdate;
      this.txtRemark.Enabled = this.canUpdate;
      this.txtPurposePR.Enabled = this.canUpdate;
      this.btnCopy.Enabled = this.canUpdate;
      this.drpRequestDate.Enabled = this.canUpdate;
      this.btnPrint.Enabled = true;
      this.ultPRNo.Enabled = this.canUpdate;
      if (this.status < 11)
      {
        btnGetTemplate.Enabled = true;
        btnPatch.Enabled = true;
      }
      else
      {
        btnGetTemplate.Enabled = false;
        btnPatch.Enabled = false;
      }

      if (isPROnline == true)
      {
        this.btnAddDetail.Enabled = false;
        this.btnCopy.Enabled = false;
      }
    }

    /// <summary>
    /// Load Data PR Detail
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataPRDetail(DataSet dsSource)
    {
      this.canUpdate = (btnSave.Visible && this.nonConfirm);

      this.ultData.DataSource = dsSource.Tables[1];
      this.ultAddition.DataSource = dsSource.Tables[2];
      if (dsSource.Tables[2].Rows.Count > 0)
      {
        this.grpAddition.Visible = true;
      }
      else
      {
        this.grpAddition.Visible = false;
      }

      for (int i = 0; i < ultAddition.Rows.Count; i++)
      {
        UltraGridRow row = ultAddition.Rows[i];

        if (this.canUpdate)
        {
          row.Activation = Activation.AllowEdit;
        }
        else
        {
          row.Activation = Activation.ActivateOnly;
        }
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];

        // Status Delete Row
        if (this.isPurchaseManager == false)
        {
          if (DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_NEW
            || DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_RTNGROUPPUR)
          {
            if (this.isPROnline == false)
            {
              row.Band.Override.AllowDelete = DefaultableBoolean.True;
            }
          }
          else
          {
            //row.Band.Override.AllowDelete = DefaultableBoolean.False;
            // Truong Add
            if (DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == 10
            || DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == 11)
            {
              row.Band.Override.AllowDelete = DefaultableBoolean.False;
            }
            else
            {
              if (this.isPROnline == false)
              {
                row.Band.Override.AllowDelete = DefaultableBoolean.True;
              }
            }
          }
        }
        else
        {
          if (DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_NEW
            || DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_HEADDEPAPP
            || DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_RTNGROUPPUR
            || DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_RTNPURMAN)
          {
            if (this.isPROnline == false)
            {
              row.Band.Override.AllowDelete = DefaultableBoolean.True;
            }
          }
          else
          {
            row.Band.Override.AllowDelete = DefaultableBoolean.False;
          }
        }

        // Select 
        if (this.isPurchaseManager == true)
        {
          if (DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_NEW
            || DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_HEADDEPAPP
            || DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_RTNGROUPPUR
            || DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_RTNPURMAN)
          {
            row.Cells["Selected"].Activation = Activation.AllowEdit;
            row.Cells["ChkDelete"].Value = 1;
            continue;
          }
          else
          {
            row.Cells["Selected"].Activation = Activation.ActivateOnly;
            continue;
          }
        }

        if (this.groupStaff != long.MinValue)
        {
          if (DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_HEADDEPAPP)
          {
            row.Cells["Selected"].Activation = Activation.ActivateOnly;
            continue;
          }

          if (DBConvert.ParseLong(row.Cells["GroupInCharge"].Value.ToString()) != this.groupStaff)
          {
            row.Cells["Selected"].Activation = Activation.ActivateOnly;
            continue;
          }
          else
          {
            if (DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_NEW
              || DBConvert.ParseInt(row.Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_RTNGROUPPUR)
            {
              row.Cells["Selected"].Activation = Activation.AllowEdit;
              row.Cells["ChkDelete"].Value = 1;
              continue;
            }
            else
            {
              row.Cells["Selected"].Activation = Activation.ActivateOnly;
              continue;
            }
          }
        }
        else
        {
          row.Cells["Selected"].Activation = Activation.ActivateOnly;
        }
      }
    }
    #endregion Init Data

    #region Event

    /// <summary>
    /// Delete data grid
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
      string message = string.Empty;

      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          DBParameter[] inputParams = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          string storeName = string.Empty;
          storeName = "spPURPRDetail_Delete";

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) == 0)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadData();
            return;
          }
        }
      }

      // Update Status TblPURPRInformation
      string no = this.UpdateStatusPRInformation();
      if (no.Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0004");
        this.LoadData();
        return;
      }
    }

    /// <summary>
    /// Department Value Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDepartment_ValueChanged(object sender, EventArgs e)
    {
      if (ultDepartment.SelectedRow != null)
      {
        string department = ultDepartment.SelectedRow.Cells["Department"].Value.ToString().Trim();
        this.LoadComboRequester(department);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].Cells["ProjectCode"].Value = DBNull.Value;
        }
        this.LoadDropDownProjectCode();
      }
    }

    /// <summary>
    /// Department Leave
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDepartment_Leave(object sender, EventArgs e)
    {
      //string value = string.Empty;
      //try
      //{
      //  value = ultDepartment.Value.ToString().Trim();
      //}
      //catch { }
      //if (value.Length == 0)
      //{
      //  string text = ultDepartment.Text.Trim();

      //  string commandText = string.Empty;
      //  commandText = "SELECT COUNT(*) FROM VHRDDepartment WHERE Department = '" + text +"'";
      //  object objDep = DataBaseAccess.ExecuteScalarCommandText(commandText);
      //  if ((objDep == null) || (objDep != null && (int)objDep == 0))
      //  {
      //    WindowUtinity.ShowMessageError("ERR0011", "Department");
      //    ultDepartment.Focus();
      //    return;
      //  }
      //}
    }

    /// <summary>
    /// Open Insert Material Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddDetail_Click(object sender, EventArgs e)
    {
      viewPUR_03_002 view = new viewPUR_03_002();
      view.prNo = this.prNo;
      WindowUtinity.ShowView(view, "Insert PR Detail", true, ViewState.ModalWindow);
      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Save PR
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success;
      string message = string.Empty;
      // Insert PR
      if (this.prNo.Length == 0)
      {
        success = this.CheckValidPRInformationInfo(out message);
        if (!success)
        {
          WindowUtinity.ShowMessageErrorFromText(message);
          return;
        }

        success = this.CheckValidPRDetailInfo(out message);
        if (!success)
        {
          WindowUtinity.ShowMessageErrorFromText(message);
          return;
        }

        success = this.SaveAddNew();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }

        this.LoadData();
      }
      // Update PR
      else
      {
        success = this.CheckValidPRInformationInfo(out message);
        if (!success)
        {
          WindowUtinity.ShowMessageErrorFromText(message);
          return;
        }

        success = this.CheckValidPRDetailInfo(out message);
        if (!success)
        {
          WindowUtinity.ShowMessageErrorFromText(message);
          return;
        }

        success = this.CheckValidPRDetailAdditionInfo(out message);
        if (!success)
        {
          WindowUtinity.ShowMessageErrorFromText(message);
          return;
        }

        success = this.SaveUpdatePR();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
        // Load Data
        this.LoadData();
      }
    }

    /// <summary>
    /// Init Layout grid data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = ultDDMaterials;
      e.Layout.Bands[0].Columns["Currency"].ValueList = ultDDCurrency;
      e.Layout.Bands[0].Columns["Urgent"].ValueList = ultDDUrgentLevel;
      e.Layout.Bands[0].Columns["Imported"].ValueList = ultDDLocalImport;
      e.Layout.Bands[0].Columns["ProjectCode"].ValueList = ultDDProjectCode;
      e.Layout.Bands[0].Columns["SupplierPid"].ValueList = ultDDSupplier;
      e.Layout.Bands[0].Columns["ReasonLate"].ValueList = ultDDReasonLate;

      e.Layout.Bands[0].Columns["No"].MaxWidth = 20;
      e.Layout.Bands[0].Columns["No"].MinWidth = 20;
      e.Layout.Bands[0].Columns["VAT"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["VAT"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 60;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PRNo"].Hidden = true;
      e.Layout.Bands[0].Columns["GroupInCharge"].Hidden = true;
      e.Layout.Bands[0].Columns["ChkDelete"].Hidden = true;
      e.Layout.Bands[0].Columns["PRDTStatus"].Hidden = true;
      e.Layout.Bands[0].Columns["PJOutStanding"].Hidden = true;
      e.Layout.Bands[0].Columns["PriceExchangeRate"].Hidden = true;
      e.Layout.Bands[0].Columns["FlagUnit"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Drawing"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Sample"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["QtyRequest"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyRequest"].Header.Caption = "Qty From Requester";
      e.Layout.Bands[0].Columns["Quantity"].Header.Caption = "Purchase Qty(*)";
      e.Layout.Bands[0].Columns["SupplierPid"].Header.Caption = "Supplier (*)";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "MaterialCode(*)";
      e.Layout.Bands[0].Columns["Price"].Header.Caption = "Price(*)";
      e.Layout.Bands[0].Columns["Currency"].Header.Caption = "Currency(*)";
      e.Layout.Bands[0].Columns["Urgent"].Header.Caption = "Urgent(*)";
      e.Layout.Bands[0].Columns["Imported"].Header.Caption = "Imported(*)";
      e.Layout.Bands[0].Columns["LaborPrice"].Header.Caption = "Labor Price(VND)";

      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalAmount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Total Price";
      e.Layout.Bands[0].Columns["TotalAmount"].Format = "###,###.##";

      e.Layout.Bands[0].Columns["TotalAmountVND"].Header.Caption = "Total Price VND";
      e.Layout.Bands[0].Columns["TotalAmountVND"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["TotalAmountUSD"].Header.Caption = "Total Amount";
      e.Layout.Bands[0].Columns["TotalAmountUSD"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["UnitPriceUSD"].Header.Caption = "Unit Price";
      e.Layout.Bands[0].Columns["UnitPriceUSD"].Format = "###,###.##";

      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Required Delivery Date(*)";
      e.Layout.Bands[0].Columns["RequestDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ConfirmedDateDelivery"].Header.Caption = "Confirmed Required Delivery Date(*)";
      e.Layout.Bands[0].Columns["ConfirmedDateDelivery"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ExpectedBrand"].Header.Caption = "Expected Brand";
      e.Layout.Bands[0].Columns["ProjectCode"].Header.Caption = "Project Code";
      e.Layout.Bands[0].Columns["GroupName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[0].Columns["ConfirmBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ConfirmBy"].Header.Caption = "Confirmed By";
      e.Layout.Bands[0].Columns["Price"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Purpose Of Requisition(*)";
      e.Layout.Bands[0].Columns["LastPrice"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LastPrice"].Header.Caption = "Last Price";
      e.Layout.Bands[0].Columns["LastPrice"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["ExchangeRate"].Header.Caption = "ExChangeRate =>";
      e.Layout.Bands[0].Columns["LaborPrice"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["TotalAmountUSD"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalAmountVND"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UnitPriceUSD"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Quantity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyRequest"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LastPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["VAT"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalAmount"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalAmountUSD"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalAmountVND"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UnitPriceUSD"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LaborPrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Quantity"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["VAT"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["RequestDate"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ConfirmedDateDelivery"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ReasonLate"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["ProjectCode"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Imported"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Urgent"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Currency"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["SupplierPid"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["LaborPrice"].CellAppearance.BackColor = Color.Aqua;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseLong(row.Cells["PRDTStatus"].Value.ToString()) < 10)
        {
          row.Activation = Activation.AllowEdit;
        }
        else
        {
          row.Cells["MaterialCode"].Activation = Activation.ActivateOnly;
          row.Cells["Quantity"].Activation = Activation.ActivateOnly;
          row.Cells["Price"].Activation = Activation.ActivateOnly;
          row.Cells["Currency"].Activation = Activation.ActivateOnly;
          row.Cells["VAT"].Activation = Activation.ActivateOnly;
          row.Cells["ProjectCode"].Activation = Activation.ActivateOnly;
          row.Cells["RequestDate"].Activation = Activation.ActivateOnly;
          row.Cells["ConfirmedDateDelivery"].Activation = Activation.ActivateOnly;
          row.Cells["ReasonLate"].Activation = Activation.ActivateOnly;
          row.Cells["Remark"].Activation = Activation.ActivateOnly;
          row.Cells["ExpectedBrand"].Activation = Activation.ActivateOnly;
          row.Cells["LaborPrice"].Activation = Activation.ActivateOnly;
          row.Cells["ExchangeRate"].Activation = Activation.AllowEdit;
        }

        // ExchangeRate
        if (i == 0)
        {
          e.Layout.Bands[0].Columns["ExchangeRate"].ValueList = ultDDExchangeRate;
          row.Cells["ExchangeRate"].Value = 2;
        }
        if (i > 0)
        {
          row.Cells["ExchangeRate"].Activation = Activation.ActivateOnly;
        }
      }

      if (this.status >= 11)
      {
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      }

      if (this.isPROnline == true)
      {
        //e.Layout.Bands[0].Columns["Quantity"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Urgent"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["ProjectCode"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseLong(row.Cells["PRDTStatus"].Value.ToString()) < 3)
        {
          row.Cells["Quantity"].Activation = Activation.AllowEdit;
        }
        else
        {
          row.Cells["Quantity"].Activation = Activation.ActivateOnly;
        }

        if (DBConvert.ParseInt(row.Cells["FlagUnit"].Value.ToString()) == 1)
        {
          row.Cells["MaterialCode"].Appearance.BackColor = Color.Salmon;
          row.Cells["NameEN"].Appearance.BackColor = Color.Salmon;
        }
      }

      //Sum Total Price
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmountVND"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalAmountUSD"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.00}";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Close Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Double Click / Open Popup Update PR Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //if (ultData.Selected != null && ultData.Selected.Rows.Count > 0)
      //{
      //  if (isPurchaseManager == false)
      //  {
      //    if (DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_NEW
      //        || DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_RTNGROUPPUR)
      //    {
      //      viewPUR_03_002 view = new viewPUR_03_002();
      //      view.prDetailPid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["Pid"].Value.ToString());
      //      Shared.Utility.WindowUtinity.ShowView(view, "Update PR Detail", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
      //      this.LoadData();
      //    }
      //  }
      //  else
      //  {
      //    if (DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_NEW
      //      || DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_HEADDEPAPP
      //      || DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_RTNGROUPPUR
      //      || DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["PRDTStatus"].Value.ToString()) == STT_PRDETAIL_RTNPURMAN)
      //    {
      //      viewPUR_03_002 view = new viewPUR_03_002();
      //      view.prDetailPid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["Pid"].Value.ToString());
      //      Shared.Utility.WindowUtinity.ShowView(view, "Update PR Detail", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
      //      this.LoadData();
      //    }
      //  }
      //}
    }

    /// <summary>
    /// Return To Group Purchase Aprroved
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReturn_Click(object sender, EventArgs e)
    {
      string commandText = string.Empty;
      System.Data.DataTable dt = new System.Data.DataTable();
      Boolean flag = false;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          flag = true;
          string storeName = string.Empty;
          storeName = "spPURPRDetailStatus_Update";
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@Status", DbType.Int32, 4);
          inputParam[1] = new DBParameter("@ConfirmBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          inputParam[2] = new DBParameter("@PrDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));

          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@ResultPid", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
        }
      }
      if (flag == false)
      {
        return;
      }
      WindowUtinity.ShowMessageSuccess("MSG0033");

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Open List Approved
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnApproved_Click(object sender, EventArgs e)
    {
      if (this.prNo.Length > 0 && ultData.Rows.Count > 0)
      {
        viewPUR_03_005 view = new viewPUR_03_005();
        view.prNo = this.prNo;
        view.totalPrice = this.totalPrice;

        WindowUtinity.ShowView(view, "Approved", true, ViewState.ModalWindow);
        // Load Data
        this.LoadData();
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "materialcode":
          if (e.Cell.Row.Cells["MaterialCode"].Value != null)
          {
            string materialCode = e.Cell.Row.Cells["MaterialCode"].Value.ToString();
            DBParameter[] input = new DBParameter[2];
            input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, materialCode);
            input[1] = new DBParameter("@PRNo", DbType.AnsiString, this.prNo);
            DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPURGetInfoMaterialPRDetail", input);
            if (dt != null && dt.Rows.Count > 0)
            {
              e.Cell.Row.Cells["NameEN"].Value = dt.Rows[0]["NameEN"].ToString();
              e.Cell.Row.Cells["Unit"].Value = dt.Rows[0]["Unit"].ToString();
              e.Cell.Row.Cells["GroupIncharge"].Value = dt.Rows[0]["GroupIncharge"].ToString();
              e.Cell.Row.Cells["GroupName"].Value = dt.Rows[0]["GroupName"].ToString();
              if (DBConvert.ParseDouble(dt.Rows[0]["LastPrice"].ToString()) != double.MinValue)
              {
                e.Cell.Row.Cells["LastPrice"].Value = DBConvert.ParseDouble(dt.Rows[0]["LastPrice"].ToString());
              }
              else
              {
                e.Cell.Row.Cells["LastPrice"].Value = DBNull.Value;
              }
              e.Cell.Row.Cells["Selected"].Value = 0;
            }
            else
            {
              e.Cell.Row.Cells["NameEN"].Value = "";
              e.Cell.Row.Cells["Unit"].Value = "";
              e.Cell.Row.Cells["GroupName"].Value = "";
              e.Cell.Row.Cells["LastPrice"].Value = DBNull.Value;
              e.Cell.Row.Cells["GroupIncharge"].Value = DBNull.Value;
            }
          }
          break;
        case "exchangerate":
          {
            int index = e.Cell.Row.Index;
            if (index == 0)
            {
              if (this.flag == true)
              {
                if (e.Cell.Row.Cells["ExchangeRate"].Value != null)
                {

                  for (int i = 0; i < ultData.Rows.Count; i++)
                  {
                    UltraGridRow row = ultData.Rows[i];
                    row.Cells["TotalAmountUSD"].Value = (DBConvert.ParseDouble(row.Cells["TotalAmountVND"].Value.ToString()) / DBConvert.ParseDouble(ultDDExchangeRate.SelectedRow.Cells["CurrentExchangeRate"].Value.ToString()));
                    row.Cells["UnitPriceUSD"].Value = (DBConvert.ParseDouble(row.Cells["PriceExchangeRate"].Value.ToString()) / DBConvert.ParseDouble(ultDDExchangeRate.SelectedRow.Cells["CurrentExchangeRate"].Value.ToString()));
                  }
                }
              }
            }
            break;
          }
        default:
          break;
      }
    }

    /// <summary>
    /// Key Up
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_KeyUp(object sender, KeyEventArgs e)
    {
      try
      {
        if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
        {
          return;
        }
        int columnIndex = (e.KeyCode == Keys.Left) ? ultData.ActiveCell.Column.Index - 1 : ultData.ActiveCell.Column.Index + 1;
        int cellIndex = ultData.ActiveCell.Row.Index;
        try
        {
          ultData.Rows[cellIndex].Cells[columnIndex].Activate();
          ultData.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }
        catch
        {
        }
      }
      catch
      {
      }
    }

    private void btnPrintApproved_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@PRNo", DbType.AnsiString, 16, this.prNo);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPurchaseRequisitionApproved_Select", input);
      if (ds != null)
      {
        dsPURPurchaseRequisition dsSource = new dsPURPurchaseRequisition();
        dsSource.Tables["dtPurchaseRequisitionInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtPurchaseRequisitionDetail"].Merge(ds.Tables[1]);
        dsSource.Tables["dtPRDetailSubReport"].Merge(ds.Tables[2]);

        double totalAmount = 0;
        double totalAmountUSD = 0;
        double currentExchangeRate = 1;
        foreach (DataRow row in dsSource.Tables["dtPurchaseRequisitionDetail"].Rows)
        {
          totalAmount += DBConvert.ParseDouble(row["Amount"].ToString());
        }
        string commadText = "SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Code = 'USD'";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commadText);
        if (dt != null)
        {
          currentExchangeRate = DBConvert.ParseDouble(dt.Rows[0]["CurrentExchangeRate"].ToString());
          totalAmountUSD = totalAmount / currentExchangeRate;
        }
        DaiCo.Shared.View_Report report = null;
        cptPurchaseRequisition cpt = new cptPurchaseRequisition();
        if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
        {
          dsSource.Tables["dtAdditionPrice"].Merge(ds.Tables[3]);
          for (int i = 0; i < dsSource.Tables["dtAdditionPrice"].Rows.Count; i++)
          {
            totalAmount = totalAmount + DBConvert.ParseDouble(dsSource.Tables["dtAdditionPrice"].Rows[i]["Amount"].ToString());
          }
          totalAmountUSD = totalAmount / currentExchangeRate;
          cpt.Section4.SectionFormat.EnableSuppress = false;
        }
        else
        {
          cpt.Section4.SectionFormat.EnableSuppress = true;
        }
        cpt.SetDataSource(dsSource);
        cpt.SetParameterValue("paramTotalAmount", totalAmount);
        cpt.SetParameterValue("paramTotalAmountUSD", totalAmountUSD);

        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = false;
        report.ShowReport(Shared.Utility.ViewState.MainWindow);
      }
    }

    private void ultAddition_KeyUp(object sender, KeyEventArgs e)
    {
      try
      {
        if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
        {
          return;
        }
        int columnIndex = (e.KeyCode == Keys.Left) ? ultAddition.ActiveCell.Column.Index - 1 : ultAddition.ActiveCell.Column.Index + 1;
        int cellIndex = ultAddition.ActiveCell.Row.Index;
        try
        {
          ultAddition.Rows[cellIndex].Cells[columnIndex].Activate();
          ultAddition.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }
        catch
        {
        }
      }
      catch
      {
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName)
      {
        case "Quantity":
          if (text.Trim().Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Quantity");
            e.Cancel = true;
          }
          else if (DBConvert.ParseDouble(text) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Quantity > 0");
            e.Cancel = true;
          }
          else if (DBConvert.ParseDouble(text) < DBConvert.ParseDouble(e.Cell.Row.Cells["QtyRequest"].Value.ToString()))
          {
            WindowUtinity.ShowMessageError("ERR0001", "Quantity >= QtyRequest");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }
    #endregion Event

    #region Process

    /// <summary>
    /// Add New PR Information
    /// </summary>
    /// <returns></returns>
    private bool SaveUpdatePR()
    {
      bool result = true;

      // Pr Information
      this.prNo = this.SaveUpdatePrInformation();
      if (prNo.Trim().Length == 0)
      {
        return false;
      }

      result = this.SaveInsertNewPRDetail();
      if (result == false)
      {
        return false;
      }

      //Pr Detail Information
      result = this.SaveUpdatePrDetailInformation();
      if (result == false)
      {
        return false;
      }

      //Update TotalMoney
      result = this.UpdateTotalMoney(prNo);
      if (result == false)
      {
        return false;
      }

      //Pr Addition Price
      result = this.SaveUpdatePrAddition();
      if (result == false)
      {
        return false;
      }
      return result;
    }

    /// <summary>
    /// Update PR Schedule Total Money
    /// </summary>
    private bool UpdateTotalMoney(string pr)
    {
      // Input
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@PRNo", DbType.AnsiString, 16, pr);
      // Output
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, 0);
      // Exec
      DataBaseAccess.ExecuteStoreProcedure("spPURUpdatePRTotalAmountFromPRDetail", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Add New PR Information
    /// </summary>
    /// <returns></returns>
    private bool SaveAddNew()
    {
      this.prNo = this.SaveInsertNewPrInformation();
      if (prNo.Trim().Length == 0)
      {
        return false;
      }
      else
      {
        bool result = this.SaveInsertNewPRDetail();
        if (!result)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Truong Add
    /// </summary>
    /// <returns></returns>
    private bool SaveInsertNewPRDetail()
    {
      int result = int.MinValue;
      string storeName = string.Empty;
      // Insert
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        // Lay Thong Tin ExchangeRate
        double currentExchangeRate = float.MinValue;
        string commandText = string.Format(@"SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Pid = {0}", DBConvert.ParseLong(row.Cells["Currency"].Value.ToString()));
        DataTable dtExchangeRate = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtExchangeRate != null && dtExchangeRate.Rows.Count > 0)
        {
          currentExchangeRate = DBConvert.ParseDouble(dtExchangeRate.Rows[0]["CurrentExchangeRate"].ToString());
        }
        // Lay Thong Tin ExchangeRate
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) == long.MinValue)
        {
          storeName = "spPURPRDetail_Insert";
          DBParameter[] inputParamInsert = new DBParameter[18];

          inputParamInsert[0] = new DBParameter("@PrNo", DbType.AnsiString, 16, this.prNo);
          inputParamInsert[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, row.Cells["MaterialCode"].Value.ToString());
          inputParamInsert[2] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Quantity"].Value.ToString()));
          if (DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()) != double.MinValue)
          {
            inputParamInsert[3] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()));
          }
          inputParamInsert[4] = new DBParameter("@CurrencyPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Currency"].Value.ToString()));
          inputParamInsert[5] = new DBParameter("@ExchangeRate", DbType.Double, currentExchangeRate);
          inputParamInsert[6] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseLong(row.Cells["Urgent"].Value.ToString()));
          inputParamInsert[7] = new DBParameter("@RequestDate", DbType.DateTime, (DateTime)row.Cells["RequestDate"].Value);

          if (row.Cells["ExpectedBrand"].Value.ToString().Length > 0)
          {
            inputParamInsert[8] = new DBParameter("@ExpectedBrand", DbType.AnsiString, 512, row.Cells["ExpectedBrand"].Value.ToString());
          }

          if (DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString()) != double.MinValue)
          {
            inputParamInsert[9] = new DBParameter("@VAT", DbType.Double, DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString()));
          }

          inputParamInsert[10] = new DBParameter("@Imported", DbType.Int32, DBConvert.ParseInt(row.Cells["Imported"].Value.ToString()));

          if (DBConvert.ParseInt(DBConvert.ParseInt(row.Cells["ProjectCode"].Value.ToString())) != int.MinValue)
          {
            inputParamInsert[11] = new DBParameter("@ProjectPid", DbType.Int64, DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString()));
          }

          inputParamInsert[12] = new DBParameter("@GroupPid", DbType.Int32, DBConvert.ParseInt(row.Cells["GroupInCharge"].Value.ToString()));

          // Purpose Of PR Detail
          if (row.Cells["Remark"].Value.ToString().Length > 0)
          {
            inputParamInsert[13] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
          }

          // Supplier
          if (DBConvert.ParseLong(DBConvert.ParseLong(row.Cells["SupplierPid"].Value.ToString())) != long.MinValue)
          {
            inputParamInsert[14] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(row.Cells["SupplierPid"].Value.ToString()));
          }

          // ConfirmedDateDelivery
          if (row.Cells["ConfirmedDateDelivery"].Value.ToString().Trim().Length > 0)
          {
            inputParamInsert[15] = new DBParameter("@ConfirmedDateDelivery", DbType.DateTime, DBConvert.ParseDateTime(row.Cells["ConfirmedDateDelivery"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }

          // Reason Late
          if (DBConvert.ParseInt(DBConvert.ParseInt(row.Cells["ReasonLate"].Value.ToString())) != int.MinValue)
          {
            inputParamInsert[16] = new DBParameter("@ReasonLate", DbType.Int64, DBConvert.ParseInt(row.Cells["ReasonLate"].Value.ToString()));
          }

          // LaborPrice
          if (row.Cells["LaborPrice"].Text.Trim().Length > 0 && DBConvert.ParseDouble(row.Cells["LaborPrice"].Value.ToString()) > 0)
          {
            inputParamInsert[17] = new DBParameter("@LaborPrice", DbType.Double, DBConvert.ParseDouble(row.Cells["LaborPrice"].Value.ToString()));
          }

          DBParameter[] outputParamInsert = new DBParameter[] { new DBParameter("@Result", DbType.Int32, int.MinValue) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);
          result = DBConvert.ParseInt(outputParamInsert[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
        // Update
        else
        {
          storeName = "spPURPRDetail_Update";
          DBParameter[] inputParamUpdate = new DBParameter[18];

          inputParamUpdate[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, row.Cells["MaterialCode"].Value.ToString());
          inputParamUpdate[1] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Quantity"].Value.ToString()));
          if (DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()) != double.MinValue)
          {
            inputParamUpdate[2] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()));
          }
          inputParamUpdate[3] = new DBParameter("@CurrencyPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Currency"].Value.ToString()));
          inputParamUpdate[4] = new DBParameter("@ExchangeRate", DbType.Double, currentExchangeRate);
          inputParamUpdate[5] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseLong(row.Cells["Urgent"].Value.ToString()));
          inputParamUpdate[6] = new DBParameter("@RequestDate", DbType.DateTime, (DateTime)row.Cells["RequestDate"].Value);

          if (row.Cells["ExpectedBrand"].Value.ToString().Length > 0)
          {
            inputParamUpdate[7] = new DBParameter("@ExpectedBrand", DbType.AnsiString, 512, row.Cells["ExpectedBrand"].Value.ToString());
          }

          if (DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString()) != double.MinValue)
          {
            inputParamUpdate[8] = new DBParameter("@VAT", DbType.Double, DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString()));
          }

          inputParamUpdate[9] = new DBParameter("@Imported", DbType.Int32, DBConvert.ParseInt(row.Cells["Imported"].Value.ToString()));

          if (DBConvert.ParseInt(row.Cells["ProjectCode"].Value.ToString()) != int.MinValue)
          {
            inputParamUpdate[10] = new DBParameter("@ProjectPid", DbType.Int64, DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString()));
          }

          inputParamUpdate[11] = new DBParameter("@GroupPid", DbType.Int32, DBConvert.ParseInt(row.Cells["GroupInCharge"].Value.ToString()));

          if (row.Cells["Remark"].Value.ToString().Length > 0)
          {
            inputParamUpdate[12] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
          }

          inputParamUpdate[13] = new DBParameter("@PRDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["pid"].Value.ToString()));

          if (DBConvert.ParseLong(DBConvert.ParseLong(row.Cells["SupplierPid"].Value.ToString())) != long.MinValue)
          {
            inputParamUpdate[14] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(row.Cells["SupplierPid"].Value.ToString()));
          }

          // ConfirmedDateDelivery
          if (row.Cells["ConfirmedDateDelivery"].Value.ToString().Trim().Length > 0)
          {
            inputParamUpdate[15] = new DBParameter("@ConfirmedDateDelivery", DbType.DateTime, DBConvert.ParseDateTime(row.Cells["ConfirmedDateDelivery"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }

          // Reason Late
          if (DBConvert.ParseInt(DBConvert.ParseInt(row.Cells["ReasonLate"].Value.ToString())) != int.MinValue)
          {
            inputParamUpdate[16] = new DBParameter("@ReasonLate", DbType.Int64, DBConvert.ParseInt(row.Cells["ReasonLate"].Value.ToString()));
          }

          // LaborPrice
          if (row.Cells["LaborPrice"].Text.Trim().Length > 0 && DBConvert.ParseDouble(row.Cells["LaborPrice"].Value.ToString()) > 0)
          {
            inputParamUpdate[17] = new DBParameter("@LaborPrice", DbType.Double, DBConvert.ParseDouble(row.Cells["LaborPrice"].Value.ToString()));
          }

          DBParameter[] outputParamUpdate = new DBParameter[] { new DBParameter("@Result", DbType.Int32, int.MinValue) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamUpdate, outputParamUpdate);
          result = DBConvert.ParseInt(outputParamUpdate[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    /// <summary>
    /// Save Update Pr Detail
    /// </summary>
    /// <returns></returns>
    private Boolean SaveUpdatePrAddition()
    {
      string result = string.Empty;

      string commandText = string.Empty;
      DataTable dt = new DataTable();

      for (int i = 0; i < ultAddition.Rows.Count; i++)
      {
        UltraGridRow row = ultAddition.Rows[i];
        string storeName = "spPURPURAdditionPrice_Edit";
        DBParameter[] inputParam = new DBParameter[8];
        // Update
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) != long.MinValue)
        {
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          inputParam[2] = new DBParameter("@Name", DbType.String, row.Cells["Name"].Value.ToString());
          inputParam[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()));
          inputParam[4] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()));
          if (row.Cells["Remark"].ToString().Length > 0)
          {
            inputParam[5] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
          }
          inputParam[6] = new DBParameter("@CurrencyPid", DbType.Int32, DBConvert.ParseInt(row.Cells["CurrencyPid"].Value.ToString()));
          inputParam[7] = new DBParameter("@ExchangeRate", DbType.Double, DBConvert.ParseDouble(row.Cells["ExchangeRate"].Value.ToString()));
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          if (DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
          {
            return false;
          }
        }
        // Insert
        else
        {
          inputParam[1] = new DBParameter("@PRPONo", DbType.String, this.prNo);
          inputParam[2] = new DBParameter("@Name", DbType.String, row.Cells["Name"].Value.ToString());
          inputParam[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()));
          inputParam[4] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()));
          if (row.Cells["Remark"].ToString().Length > 0)
          {
            inputParam[5] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());
          }
          inputParam[6] = new DBParameter("@CurrencyPid", DbType.Int32, DBConvert.ParseInt(row.Cells["CurrencyPid"].Value.ToString()));
          inputParam[7] = new DBParameter("@ExchangeRate", DbType.Double, DBConvert.ParseDouble(row.Cells["ExchangeRate"].Value.ToString()));
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          if (DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Update Pr Detail
    /// </summary>
    /// <returns></returns>
    private Boolean SaveUpdatePrDetailInformation()
    {
      string result = string.Empty;

      string commandText = string.Empty;
      System.Data.DataTable dt = new System.Data.DataTable();
      string listPRDetail = string.Empty;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseDouble(row.Cells["Quantity"].Value.ToString()) != DBConvert.ParseDouble(row.Cells["QtyRequest"].Value.ToString()))
        {
          listPRDetail += row.Cells["Pid"].Value.ToString() + "; ";
        }

        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          string storeName = string.Empty;
          storeName = "spPURPRDetailStatus_Update";
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@Status", DbType.Int32, 3);
          inputParam[1] = new DBParameter("@ConfirmBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          inputParam[2] = new DBParameter("@PrDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@ResultPid", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          result = outputParam[0].Value.ToString();
          if (DBConvert.ParseLong(result) == 0)
          {
            return false;
          }
        }
      }

      int length = listPRDetail.Length;
      if (length > 0)
      {
        listPRDetail = listPRDetail.Substring(0, length - 2);
        DBParameter[] iParam = new DBParameter[1];
        iParam[0] = new DBParameter("@ListPRDetail", DbType.String, listPRDetail);

        DBParameter[] oParam = new DBParameter[] { new DBParameter("@ResultPid", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPURCastQtyWhenReviseQtyOnPROnline_Update", iParam, oParam);
        if (DBConvert.ParseLong(oParam[0].Value.ToString()) == 0)
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Update Status PR Information ==> Pending
    /// </summary>
    /// <returns></returns>
    private string UpdateStatusPRInformation()
    {
      string result = string.Empty;
      DBParameter[] inputParam = new DBParameter[4];

      string storeName = string.Empty;
      storeName = "spPURPRInformationStatus_Update";

      inputParam[0] = new DBParameter("@PRNo", DbType.AnsiString, 16, this.prNo);
      inputParam[1] = new DBParameter("@StatusPRDetail", DbType.Int32, 8);
      inputParam[2] = new DBParameter("@StatusPR1", DbType.Int32, 7);
      inputParam[3] = new DBParameter("@StatusPR2", DbType.Int32, 6);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, String.Empty) };
      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      result = outputParam[0].Value.ToString();

      string commandText = "SELECT CM.[Value] FROM TblPURPRInformation PR LEFT JOIN TblBOMCodeMaster CM ON PR.[Status] = CM.Code WHERE [Group] = 7010 AND PR.PRNo = '" + this.prNo + "'";
      System.Data.DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        this.lblStatus.Text = dt.Rows[0][0].ToString();
      }

      return result;
    }

    /// <summary>
    /// Save Update Pr Information
    /// </summary>
    /// <returns></returns>
    private string SaveUpdatePrInformation()
    {
      string result = string.Empty;

      DBParameter[] inputParam = new DBParameter[11];
      string commandText = string.Empty;
      System.Data.DataTable dt = new System.Data.DataTable();

      string storeName = string.Empty;
      storeName = "spPURPRInformation_Update";

      inputParam[0] = new DBParameter("@RequestBy", DbType.Int32, DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultRequester)));
      inputParam[1] = new DBParameter("@Department", DbType.AnsiString, 48, ControlUtility.GetSelectedValueUltraCombobox(ultDepartment));

      commandText = "SELECT [Status] FROM TblPURPRInformation WHERE PRNo = '" + this.prNo + "'";
      dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      int status = 0;
      if (dt != null && dt.Rows.Count > 0)
      {
        status = DBConvert.ParseInt(dt.Rows[0][0].ToString());
      }
      if (this.chkLock.Checked)
      {
        if (status > 5)
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, status);
        }
        else
        {
          inputParam[2] = new DBParameter("@Status", DbType.Int32, 5);
        }
      }
      else
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, 0);
      }

      commandText += " SELECT Manager";
      commandText += " FROM VHRDDepartmentInfo";
      commandText += " WHERE Code ='" + ControlUtility.GetSelectedValueUltraCombobox(ultDepartment) + "'";
      dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dt != null && dt.Rows.Count > 0)
      {
        inputParam[3] = new DBParameter("@HeadDepartmentApproved", DbType.Int32, DBConvert.ParseInt(dt.Rows[0][0].ToString()));
      }

      inputParam[4] = new DBParameter("@TypeOfRequest", DbType.Int32, ControlUtility.GetSelectedValueUltraCombobox(ultTypeRequest));
      inputParam[5] = new DBParameter("@PurposeOfRequisition", DbType.AnsiString, 200, this.txtPurposePR.Text);
      inputParam[6] = new DBParameter("@Remark", DbType.AnsiString, 512, this.txtRemark.Text);
      inputParam[7] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[8] = new DBParameter("@PRNo", DbType.AnsiString, 16, this.prNo);
      inputParam[9] = new DBParameter("@CreateDate", DbType.DateTime, DBConvert.ParseDateTime(ultCreateDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      if (ultCBDepartmentRec.Value != null)
      {
        inputParam[10] = new DBParameter("@DepartmentRec", DbType.String, ultCBDepartmentRec.Value.ToString());
      }

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      result = outputParam[0].Value.ToString();
      return result;
    }

    /// <summary>
    /// Save Insert New Pr Information
    /// </summary>
    /// <returns></returns>
    private string SaveInsertNewPrInformation()
    {
      string result = string.Empty;

      DBParameter[] inputParam = new DBParameter[11];
      string commandText = string.Empty;
      System.Data.DataTable dt = new System.Data.DataTable();

      string storeName = string.Empty;
      storeName = "spPURPRInformation_Insert";

      inputParam[0] = new DBParameter("@RequestBy", DbType.Int32, DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultRequester)));
      inputParam[1] = new DBParameter("@Department", DbType.AnsiString, 48, ControlUtility.GetSelectedValueUltraCombobox(ultDepartment));
      inputParam[2] = new DBParameter("@Status", DbType.Int32, 0);

      commandText += " SELECT Manager";
      commandText += " FROM VHRDDepartmentInfo";
      commandText += " WHERE Code ='" + ControlUtility.GetSelectedValueUltraCombobox(ultDepartment) + "'";
      dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dt != null && dt.Rows.Count > 0)
      {
        inputParam[3] = new DBParameter("@HeadDepartmentApproved", DbType.Int32, DBConvert.ParseInt(dt.Rows[0][0].ToString()));
      }

      inputParam[4] = new DBParameter("@TypeOfRequest", DbType.Int32, ControlUtility.GetSelectedValueUltraCombobox(ultTypeRequest));
      inputParam[5] = new DBParameter("@PurposeOfRequisition", DbType.AnsiString, 200, this.txtPurposePR.Text);
      inputParam[6] = new DBParameter("@Remark", DbType.AnsiString, 512, this.txtRemark.Text);
      inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[8] = new DBParameter("@PRNo", DbType.AnsiString, 16, txtPRNo.Text);
      inputParam[9] = new DBParameter("@CreateDate", DbType.DateTime, DBConvert.ParseDateTime(ultCreateDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      if (ultCBDepartmentRec.Value != null)
      {
        inputParam[10] = new DBParameter("@DepartmentRec", DbType.String, ultCBDepartmentRec.Value.ToString());
      }

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      result = outputParam[0].Value.ToString();
      return result;
    }

    /// <summary>
    /// Check Valid PR Detail Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPRDetailAdditionInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      for (int i = 0; i < ultAddition.Rows.Count; i++)
      {
        UltraGridRow row = ultAddition.Rows[i];
        if (row.Cells["Name"].Value.ToString().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/Name");
          return false;
        }

        if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/Qty");
          return false;
        }

        if (DBConvert.ParseDouble(row.Cells["Price"].Value.ToString()) == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/Price");
          return false;
        }

        if (DBConvert.ParseInt(row.Cells["CurrencyPid"].Value.ToString()) == int.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/Currency");
          return false;
        }

        if (DBConvert.ParseDouble(row.Cells["ExchangeRate"].Value.ToString()) == double.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Addition/ExchangeRate");
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Check Valid PR Detail Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPRDetailInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      System.Data.DataTable dtCheck = new System.Data.DataTable();

      int count = ultData.Rows.Count;
      if (count == 0 && this.chkLock.Checked)
      {
        message = FunctionUtility.GetMessage("ERR0087");
        return false;
      }

      Boolean flag = false;
      long projectPid = long.MinValue;
      Double totalAmount = 0;
      Double PJOutStanding = 0;

      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        //Ha Anh add check project code
        if (i == 0)
        {
          projectPid = DBConvert.ParseLong(ultData.Rows[i].Cells["ProjectCode"].Value.ToString());
        }
        if (i > 0 && projectPid != DBConvert.ParseLong(ultData.Rows[i].Cells["ProjectCode"].Value.ToString()))
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Project Code");
          return false;
        }

        commandText = string.Format(@"SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Pid = {0}", DBConvert.ParseLong(row.Cells["Currency"].Value.ToString()));
        Double dtExchangeRate = DBConvert.ParseDouble(DataBaseAccess.ExecuteScalarCommandText(commandText));

        commandText = string.Format(@"SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Pid = 2"); //USD
        Double usExchangeRate = DBConvert.ParseDouble(DataBaseAccess.ExecuteScalarCommandText(commandText));


        totalAmount += (DBConvert.ParseDouble(ultData.Rows[i].Cells["Price"].Value.ToString()))
                        * DBConvert.ParseInt(ultData.Rows[i].Cells["Quantity"].Value.ToString()) * dtExchangeRate / usExchangeRate;


        PJOutStanding = DBConvert.ParseDouble(ultData.Rows[i].Cells["PJOutStanding"].Value.ToString());
        //end

        // Truong Add
        commandText = string.Format(@"SELECT * FROM TblGNRMaterialInformation WHERE MaterialCode = '{0}'", row.Cells["MaterialCode"].Value.ToString());
        DataTable dtMaterial = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtMaterial == null || dtMaterial.Rows.Count == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Material Code");
          return false;
        }
        else
        {
          // Quantity
          if (row.Cells["Quantity"].Value.ToString().Trim().Length == 0)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Quantity");
            return false;
          }

          double qty = DBConvert.ParseDouble(row.Cells["Quantity"].Value.ToString().Trim());
          if (qty == double.MinValue)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Quantity");
            return false;
          }

          if (qty < DBConvert.ParseDouble(row.Cells["QtyRequest"].Value.ToString().Trim()))
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Quantity >= QtyRequest");
            return false;
          }

          if (DBConvert.ParseLong(row.Cells["Imported"].Value.ToString()) == long.MinValue)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Local/Import");
            return false;
          }

          if (DBConvert.ParseLong(row.Cells["SupplierPid"].Value.ToString()) == long.MinValue &&
            row.Cells["Status"].Value.ToString() != "Cancel" &&
            row.Cells["Status"].Value.ToString() != "Finished")
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Supplier");
            return false;
          }

          if (DBConvert.ParseLong(row.Cells["Urgent"].Value.ToString()) == long.MinValue)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Urgent Level");
            return false;
          }

          // Request Date
          if (row.Cells["RequestDate"].Value.ToString().Trim().Length == 0)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Request Date");
            return false;
          }

          DateTime date = DBConvert.ParseDateTime(row.Cells["RequestDate"].Value.ToString(), formatConvert);
          if (date == DateTime.MinValue)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Request Date");
            return false;
          }

          // Confirmed Delivery Date
          if (row.Cells["ConfirmedDateDelivery"].Value.ToString().Trim().Length == 0 &&
            row.Cells["Status"].Value.ToString() != "Cancel" &&
            row.Cells["Status"].Value.ToString() != "Finished")
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Confirmed Required Delivery Date");
            return false;
          }

          // VAT
          if (row.Cells["VAT"].Value.ToString().Trim().Length > 0)
          {
            double vat = DBConvert.ParseDouble(row.Cells["VAT"].Value.ToString().Trim());
            if (vat == double.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The VAT");
              return false;
            }
          }

          // Project Code
          string projCode = row.Cells["ProjectCode"].Text.ToString().Trim();
          if (projCode.Length != 0)
          {
            if (DBConvert.ParseLong(row.Cells["ProjectCode"].Value.ToString()) == long.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Project Code");
              return false;
            }
          }

          // LaborPrice
          if (row.Cells["LaborPrice"].Text.Trim().Length > 0 && DBConvert.ParseDouble(row.Cells["LaborPrice"].Value.ToString()) <= 0)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Labor Price");
            return false;
          }
        }
        // End
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          flag = true;
          if (row.Cells["Price"].Value.ToString().Length == 0)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Price");
            return false;
          }

        }
      }

      //DataTable dt = (DataTable)ultData.DataSource;
      //if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["ProjectCode"].ToString().Length > 0)
      //{
      //  if (totalAmount > PJOutStanding)
      //  {
      //    message = string.Format("Project Code is over Budget outstanding");
      //    return false;
      //  }
      //}

      if (flag == true && this.chkLock.Checked == false)
      {
        message = FunctionUtility.GetMessage("ERR0084");
        return false;
      }

      return true;
    }

    /// <summary>
    /// Check Valid PR Information Info
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidPRInformationInfo(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      System.Data.DataTable dtCheck = new System.Data.DataTable();

      // Check PRNo
      string text = string.Empty;
      text = txtPRNo.Text;
      if (text.Length == 0 || text.Trim().Length > 16)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "PRNo");
        return false;
      }
      else
      {
        if (this.prNo.Length == 0)
        {
          commandText = "SELECT PRNo FROM TblPURPRInformation WHERE PRNo = '" + this.txtPRNo.Text.Trim() + "'";
        }
        else
        {
          commandText = " SELECT PRNo FROM TblPURPRInformation WHERE PRNo = '" + this.txtPRNo.Text.Trim() + "' AND PRNo != '" + this.prNo + "'";
        }
        dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck.Rows.Count > 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "PRNo is existed on system");
          return false;
        }
      }

      // Department
      string department = ControlUtility.GetSelectedValueUltraCombobox(ultDepartment);

      if (department.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Department");
        return false;
      }

      // Requester
      int requester = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultRequester));

      if (requester == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Requester");
        return false;
      }

      commandText = "SELECT COUNT(*) FROM VHRNhanVien WHERE ID_NhanVien = " + requester;
      dtCheck = DataBaseAccess.SearchCommandText(commandText).Tables[0];
      if (dtCheck != null && DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Requester");
        return false;
      }

      // Type Of Request
      int typeOfRequest = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultTypeRequest));

      if (typeOfRequest == int.MinValue)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Type Of Request");
        return false;
      }

      // Create Date
      if (ultCreateDate.Value == null)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "CreateDate Of PR");
        return false;
      }

      // Check Department Received
      if (ultCBDepartmentRec.Value == null || ultCBDepartmentRec.Value.ToString().Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Department Received");
        return false;
      }
      return true;
    }

    /// <summary>
    /// PrintPR
    /// </summary>
    private void PrintPR()
    {
      viewPUR_03_007 view = new viewPUR_03_007();
      view.prNO = this.prNo;
      Shared.Utility.WindowUtinity.ShowView(view, "Print PR", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
    }

    #endregion Process

    #region Import Excel
    /// <summary>
    /// Open Dialog order to choose excel file
    /// </summary>
    private void btnOpenDialog_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtImportExcel.Text.Trim().Length > 0);
    }

    /// <summary>
    /// Click Import excel, save date and load grid
    /// </summary>
    private void btnImport_Click(object sender, EventArgs e)
    {
      string materialCode = string.Empty;
      string message = string.Empty;
      System.Data.DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B14:L200]").Tables[0];
      if (dtSource == null || dtSource.Rows.Count == 0)
      {
        return;
      }

      bool check = this.ValidationInputFromExcel(dtSource, out message);
      if (!check)
      {
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      foreach (DataRow row in dtSource.Rows)
      {
        if (row["Material Code"].ToString().Trim().Length > 0)
        {
          DBParameter[] inputParam = new DBParameter[16];
          string storename = "spPURPRDetail_Insert";
          string sql = "SELECT * FROM TblPURCurrencyInfo WHERE Code = '" + row["Currency"].ToString() + "'";
          DataSet ds = DataBaseAccess.SearchCommandText(sql);
          if (ds != null && ds.Tables[0].Rows.Count > 0)
          {
            curID = DBConvert.ParseLong(ds.Tables[0].Rows[0]["Pid"].ToString());
            exchangeRate = DBConvert.ParseDouble(ds.Tables[0].Rows[0]["CurrentExchangeRate"].ToString());
          }
          string PRNo = this.txtPRNo.Text.ToString();
          inputParam[1] = new DBParameter("@PRNo", DbType.AnsiString, 16, PRNo);

          string material_Code = row["Material Code"].ToString().Trim();
          inputParam[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, material_Code);

          double quantity = DBConvert.ParseDouble(row["Quantity"].ToString().Trim());
          inputParam[3] = new DBParameter("@Qty", DbType.Double, quantity);

          inputParam[4] = new DBParameter("@CurrencyPid", DbType.Int64, curID);
          sql = "SELECT Code FROM TblBOMCodeMaster WHERE [Group] = 7006 AND Value ='" + row["Local/Import"].ToString() + "'";

          ds = DataBaseAccess.SearchCommandText(sql);
          if (ds != null && ds.Tables[0].Rows.Count > 0)
          {
            inputParam[5] = new DBParameter("@Imported", DbType.Int32, ds.Tables[0].Rows[0]["Code"].ToString());
          }

          sql = "SELECT Code FROM TblBOMCodeMaster WHERE [Group] = 7007 AND Value ='" + row["Urgent Level"].ToString() + "'";
          ds = DataBaseAccess.SearchCommandText(sql);
          if (ds != null && ds.Tables[0].Rows.Count > 0)
          {
            inputParam[6] = new DBParameter("@Urgent", DbType.Int32, ds.Tables[0].Rows[0]["Code"].ToString());
          }

          DateTime requestDate = DBConvert.ParseDateTime(row["Request Date"].ToString(), DaiCo.Shared.Utility.ConstantClass.FORMAT_DATETIME);
          inputParam[7] = new DBParameter("@RequestDate", DbType.DateTime, requestDate);

          double price = DBConvert.ParseDouble(row["Price"].ToString().Trim());
          if (price != double.MinValue)
          {
            inputParam[8] = new DBParameter("@Price", DbType.Double, price);
          }

          inputParam[9] = new DBParameter("@ExchangeRate", DbType.Double, exchangeRate);
          string brand = row["Expected Brand"].ToString().Trim();
          inputParam[10] = new DBParameter("@ExpectedBrand", DbType.AnsiString, 512, brand);

          double vat = DBConvert.ParseDouble(row["VAT"].ToString().Trim());
          if (vat != double.MinValue)
          {
            inputParam[11] = new DBParameter("@VAT", DbType.Double, vat);
          }

          string remark = row["Remark"].ToString().Trim();
          inputParam[12] = new DBParameter("@Remark", DbType.AnsiString, 512, remark);
          sql = string.Format(@"SELECT  Pid Code, ProjectCode Name 
                                FROM    TblPURPRProjectCode 
                                WHERE   ISNULL(Finished, 0) = 0 AND ISNULL([Status], 0) = 1 AND ProjectCode = '{0}'", row["Project Code"].ToString());
          ds = DataBaseAccess.SearchCommandText(sql);
          if (ds != null && ds.Tables[0].Rows.Count > 0)
          {
            inputParam[13] = new DBParameter("@ProjectPid", DbType.Int32, ds.Tables[0].Rows[0]["Code"].ToString());
          }

          sql = "SELECT GroupIncharge FROM TblGNRMaterialInformation WHERE MaterialCode = '" + row["Material Code"].ToString().Trim() + "'";
          ds = DataBaseAccess.SearchCommandText(sql);

          if (ds != null && ds.Tables[0].Rows.Count > 0)
          {
            inputParam[14] = new DBParameter("@GroupPid", DbType.Int32, ds.Tables[0].Rows[0]["GroupIncharge"].ToString());
          }

          DBParameter[] outputParamInsert = new DBParameter[] { new DBParameter("@Result", DbType.Int32, int.MinValue) };
          DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outputParamInsert);
          int result = DBConvert.ParseInt(outputParamInsert[0].Value.ToString());
          if (result == 0 || result == int.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0005");
            return;
          }

          // Update Schedule Money
          double amount = quantity * exchangeRate * price;
          this.UpdateTotalMoney(this.prNo, amount);
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      btnImport.Enabled = false;
      this.LoadData();
    }

    /// <summary>
    /// Update PR Schedule Total Money
    /// </summary>
    private bool UpdateTotalMoney(string pr, double amount)
    {
      // Input
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@PRNo", DbType.AnsiString, 16, pr);
      input[1] = new DBParameter("@Amount", DbType.Double, amount);
      // Output
      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, 0);
      // Exec
      DataBaseAccess.ExecuteStoreProcedure("spPURUpdatePRTotalAmount", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// validate input form in excel file
    /// </summary>
    private bool ValidationInputFromExcel(System.Data.DataTable dttable, out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      System.Data.DataTable dtCheck = new System.Data.DataTable();

      foreach (DataRow row in dttable.Rows)
      {
        if (row["Material Code"].ToString().Length > 0)
        {
          string materialCode = row["Material Code"].ToString().Trim();
          commandText = "SELECT * FROM TblGNRMaterialInformation WHERE MaterialCode = '" + materialCode + "'";
          DataSet ds = DataBaseAccess.SearchCommandText(commandText);
          if (ds.Tables[0].Rows.Count == 0)
          {
            message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Material Code");
            return false;
          }
          else
          {
            // Currency
            string currency = row["Currency"].ToString().Trim();
            if (currency.Length == 0)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Currency");
              return false;
            }
            else
            {
              commandText = "SELECT COUNT(*) FROM TblPURCurrencyInfo WHERE Code = '" + currency + "'";
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck != null && dtCheck.Rows.Count > 0)
              {
                if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
                {
                  message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Currency");
                  return false;
                }
              }
            }

            // Quantity
            if (row["Quantity"].ToString().Trim().Length == 0)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Quantity");
              return false;
            }

            double qty = DBConvert.ParseDouble(row["Quantity"].ToString().Trim());
            if (qty == double.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Quantity");
              return false;
            }

            // Local/Import
            string local = row["Local/Import"].ToString().Trim();
            if (local.Length == 0)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Local/Import");
              return false;
            }
            else
            {
              commandText = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE [Group] = 7006 AND [Value] = '" + local + "'";
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck != null && dtCheck.Rows.Count > 0)
              {
                if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
                {
                  message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Local/Import");
                  return false;
                }
              }
            }

            // Urgent Level
            string urgentLevel = row["Urgent Level"].ToString().Trim();
            if (urgentLevel.Length == 0)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Urgent Level");
              return false;
            }
            else
            {
              commandText = "SELECT COUNT(*) FROM TblBOMCodeMaster WHERE [Group] = 7007 AND [Value] = '" + urgentLevel + "'";
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck != null && dtCheck.Rows.Count > 0)
              {
                if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
                {
                  message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Urgent Level");
                  return false;
                }
              }
            }

            // Request Date
            if (row["Request Date"].ToString().Trim().Length == 0)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Request Date");
              return false;
            }

            DateTime date = DBConvert.ParseDateTime(row["Request Date"].ToString(), DaiCo.Shared.Utility.ConstantClass.FORMAT_DATETIME);
            if (date == DateTime.MinValue)
            {
              message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Request Date");
              return false;
            }

            // VAT
            if (row["VAT"].ToString().Trim().Length > 0)
            {
              double vat = DBConvert.ParseDouble(row["VAT"].ToString().Trim());
              if (vat == double.MinValue)
              {
                message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The VAT");
                return false;
              }
            }

            // Price
            if (row["Price"].ToString().Trim().Length > 0)
            {
              double price = DBConvert.ParseDouble(row["Price"].ToString().Trim());
              if (price == double.MinValue)
              {
                message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Price");
                return false;
              }
            }

            // Project Code
            string projCode = row["Project Code"].ToString().Trim();
            if (projCode.Length != 0)
            {
              commandText = string.Format(@"  SELECT  Pid Code, ProjectCode Name 
                                              FROM    TblPURPRProjectCode 
                                              WHERE   ISNULL(Finished, 0) = 0 AND ISNULL([Status], 0) = 1 AND ProjectCode = '{0}'", projCode);
              dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dtCheck != null && dtCheck.Rows.Count > 0)
              {
                if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
                {
                  message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Project Code");
                  return false;
                }
              }
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check File Is Exist. If Exist Then Delete And Create File Else Create File
    /// Create by: Qui_VVD (18/10/2010 16:10)
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
    /// get Template 
    /// </summary>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      //Init report
      string strTemplateName = "Tmp_AddDetailListMaterialPRByExcel";
      string strSheetName = "Sheet1";
      string strOutFileName = "Template Add Detail List Materials PR By Excel";
      XlsReport oXlsReport = this.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, out strOutFileName);

      //Export
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void chkSelect_CheckedChanged(object sender, EventArgs e)
    {
      bool flag = false;
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];

        if (row.Cells["Selected"].Activation == Activation.AllowEdit)
        {
          if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 0)
          {
            row.Cells["Selected"].Value = 1;
            if (row.Cells["MaterialCode"].Value.ToString().Contains("060-05-"))
            {
              flag = true;
            }
          }
          else
          {
            row.Cells["Selected"].Value = 0;
          }
        }
      }

      if (flag == true)
      {
        string message = "Exists Material"
                                   + " need to check Quantity again before confirm PR";
        WindowUtinity.ShowMessageWarningFromText(message);
      }
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      DataTable dtCheck = new DataTable();
      string prNoCopy = ControlUtility.GetSelectedValueUltraCombobox(ultPRNo);

      if (drpRequestDate.Value == null)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Request Date Copy");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      if (prNoCopy.Length == 0 && drpRequestDate.Value != null)
      {
        // Copy Request Date Add Detail
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          row.Cells["RequestDate"].Value = DBConvert.ParseDateTime(drpRequestDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        }
        WindowUtinity.ShowMessageSuccess("MSG0022");
        return;

        // Copy PR
        //message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The PR No(Copy)");
        //WindowUtinity.ShowMessageErrorFromText(message);
        //return;
      }
      else if (prNoCopy.Length > 0 && DBConvert.ParseDateTime(drpRequestDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        if (prNo.Length == 0)
        {
          return;
        }
        // Request Date
        DateTime requestDate = DBConvert.ParseDateTime(this.drpRequestDate.Value);
        if (requestDate == DateTime.MinValue)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Request Date Copy");
          WindowUtinity.ShowMessageErrorFromText(message);
          return;
        }

        string commandText = string.Empty;
        commandText += "  SELECT PRDT.MaterialCode, PRDT.Quantity, PRDT.Price, CurrencyPid, ";
        commandText += "  	ExchangeRate, Urgent, ISNULL(ExpectedBrand, '') ExpectedBrand, VAT,  ";
        commandText += "  	Imported, ProjectPid, GroupInCharge, Remark  ";
        commandText += "  FROM TblPURPRDetail PRDT  ";
        commandText += "  WHERE PRNo = '" + prNoCopy + "'";

        DataTable dtData = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtData != null && dtData.Rows.Count > 0)
        {
          bool flag = false;
          foreach (DataRow row in dtData.Rows)
          {
            string storeName = "spPURPRDetail_Insert";
            DBParameter[] inputParamInsert = new DBParameter[14];

            inputParamInsert[0] = new DBParameter("@PrNo", DbType.AnsiString, 16, this.prNo);
            inputParamInsert[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, row["MaterialCode"].ToString());
            inputParamInsert[2] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row["Quantity"].ToString()));
            if (DBConvert.ParseDouble(row["Price"].ToString()) != double.MinValue)
            {
              inputParamInsert[3] = new DBParameter("@Price", DbType.Double, DBConvert.ParseDouble(row["Price"].ToString()));
            }
            inputParamInsert[4] = new DBParameter("@CurrencyPid", DbType.Int64, DBConvert.ParseLong(row["CurrencyPid"].ToString()));
            inputParamInsert[5] = new DBParameter("@ExchangeRate", DbType.Double, DBConvert.ParseLong(row["ExchangeRate"].ToString()));
            inputParamInsert[6] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseLong(row["Urgent"].ToString()));
            DateTime date = new DateTime(this.drpRequestDate.DateTime.Year, this.drpRequestDate.DateTime.Month, this.drpRequestDate.DateTime.Day);
            inputParamInsert[7] = new DBParameter("@RequestDate", DbType.DateTime, date);

            if (row["ExpectedBrand"].ToString().Length > 0)
            {
              inputParamInsert[8] = new DBParameter("@ExpectedBrand", DbType.AnsiString, 512, row["ExpectedBrand"].ToString());
            }

            if (DBConvert.ParseDouble(row["VAT"].ToString()) != double.MinValue)
            {
              inputParamInsert[9] = new DBParameter("@VAT", DbType.Double, DBConvert.ParseDouble(row["VAT"].ToString()));
            }

            inputParamInsert[10] = new DBParameter("@Imported", DbType.Int32, DBConvert.ParseDouble(row["Imported"].ToString()));

            if (DBConvert.ParseInt(row["ProjectPid"].ToString()) != int.MinValue)
            {
              inputParamInsert[11] = new DBParameter("@ProjectPid", DbType.Int32, DBConvert.ParseInt(row["ProjectPid"].ToString()));
            }

            inputParamInsert[12] = new DBParameter("@GroupPid", DbType.Int32, DBConvert.ParseInt(row["GroupInCharge"].ToString()));
            inputParamInsert[13] = new DBParameter("@Remark", DbType.AnsiString, 512, row["Remark"].ToString());

            DBParameter[] outputParamInsert = new DBParameter[] { new DBParameter("@Result", DbType.Int32, int.MinValue) };

            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);
            int result = DBConvert.ParseInt(outputParamInsert[0].Value.ToString());
            if (result == int.MinValue)
            {
              flag = true;
              break;
            }
          }

          if (!flag)
          {
            WindowUtinity.ShowMessageSuccess("MSG0022");
          }
          else
          {
            WindowUtinity.ShowMessageError("ERR0005");
          }
        }
        // Load Data
        this.LoadData();
      }
    }

    private void ultAddition_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Horizontal;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Price"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["ExchangeRate"].Format = "###,###.##";

      e.Layout.Bands[0].Columns["ExchangeRate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CurrencyPid"].Header.Caption = "Currency";

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ExchangeRate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      if (this.canUpdate)
      {
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      }
      else
      {
        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }

      e.Layout.Bands[0].Columns["Name"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["CurrencyPid"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.Aqua;

      //Sum Total Price
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Price"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";

      e.Layout.Bands[0].Columns["CurrencyPid"].ValueList = udrpCurrency;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultAddition_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "currencypid":
          try
          {
            e.Cell.Row.Cells["ExchangeRate"].Value = udrpCurrency.SelectedRow.Cells["CurrentExchangeRate"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["ExchangeRate"].Value = DBNull.Value;
          }
          break;
      }
    }

    private void ultAddition_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          string storeName = string.Empty;
          storeName = "spPURAdditionPrice_Delete";

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) == 0)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadData();
            return;
          }
        }
      }
    }

    private void chkAdddition_CheckedChanged(object sender, EventArgs e)
    {
      if (this.grpAddition.Visible)
      {
        this.grpAddition.Visible = false;
      }
      else
      {
        this.grpAddition.Visible = true;
      }
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      this.PrintPR();
    }

    private void ultDDMaterials_TextChanged(object sender, EventArgs e)
    {
      if (ultDDMaterials.SelectedRow != null)
      {
        string material = ultDDMaterials.SelectedRow.Cells["MaterialCode"].Value.ToString();
        string commandText = "SELECT GroupIncharge FROM TblGNRMaterialInformation WHERE MaterialCode = '" + material + "'";
        DataTable dt = DataBaseAccess.SearchCommandText(commandText).Tables[0];
        if (dt.Rows.Count > 0)
        {
          this.ultDDProjectCode.Text = dt.Rows[0]["GroupIncharge"].ToString();
        }
        else
        {
          this.ultDDProjectCode.Text = string.Empty;
        }
      }
      else
      {
        this.ultDDProjectCode.Text = string.Empty;
      }
    }

    private void btnNewPR_Click(object sender, EventArgs e)
    {
      viewPUR_03_001 view = new viewPUR_03_001();
      Shared.Utility.WindowUtinity.ShowView(view, "NEW PR", true, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (string.Compare("Selected", colName, true) == 0)
      {
        if (DBConvert.ParseInt(e.Cell.Row.Cells["Selected"].Text.ToString()) == 1)
        {
          if (DBConvert.ParseInt(e.Cell.Row.Cells["FlagUnit"].Value.ToString()) == 1)
          {
            string message = "Unit Of Material " + e.Cell.Row.Cells["MaterialCode"].Value.ToString()
                            + " need to check Quantity again before confirm PR";
            WindowUtinity.ShowMessageWarningFromText(message);
          }
        }
      }
    }
    #endregion Import Excel
  }
}
