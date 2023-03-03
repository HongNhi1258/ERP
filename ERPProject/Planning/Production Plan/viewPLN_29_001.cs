/*
  Author      : 
  Date        : 23/Apr/2015
  Description : Production Planing
  Standard From : viewGNR_90_006
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using PresentationControls;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_29_001 : MainUserControl
  {
    #region Field
    // WorkStation
    private string department = string.Empty;

    // Lock
    bool isLockCOM1 = false;
    bool isLockASSY = false;
    bool isLockSUBCON = false;
    bool isLockCAR = false;
    bool isLockASSHW = false;
    bool isLockMaterial = false;
    bool isLockFinalFittingHW = false;

    // Confirm
    bool isConfirmCOM1 = false;
    bool isConfirmASSY = false;
    bool isConfirmSUBCON = false;
    bool isConfirmCAR = false;
    bool isConfirmASSHW = false;
    bool isConfirmMaterial = false;
    bool isConfirmFinalFittingHW = false;

    // Data Table
    public DataRow[] dtRowDetail;

    //Check Box
    private bool checkCAR = true;
    private bool checkCOM1 = true;
    private bool checkASSY = true;
    private bool checkSUBCON = true;
    private bool checkTran = true;


    private bool isContainerInfo = false;

    #endregion Field

    #region Init
    public viewPLN_29_001()
    {
      InitializeComponent();
    }

    private void viewPLN_29_001_Load(object sender, EventArgs e)
    {
      ultDTEstimateShipdateFrom.Value = DBNull.Value;
      ultDTEstimateShipdateTo.Value = DBNull.Value;
      ultDTLoadingDateFrom.Value = DBNull.Value;
      ultDTLoadingDateTo.Value = DBNull.Value;
      ultDTPackingDeadlineFrom.Value = DBNull.Value;
      ultDTPackingDeadlineTo.Value = DBNull.Value;

      //--- User Login
      // PLN
      if (btnPerPLA.Visible == true)
      {
        this.department = "PLN";
        chkTran.Visible = true;
        btnPerPLA.Visible = false;
      }
      // COM1
      if (btnPerCOM1.Visible == true)
      {
        this.department = "COM1";
        btnPerCOM1.Visible = false;
      }
      // ASSY
      if (btnPerASSY.Visible == true)
      {
        this.department = "ASSY";
        btnPerASSY.Visible = false;
      }
      // SUBCON
      if (btnPerSUBCON.Visible == true)
      {
        this.department = "SUBCON";
        btnPerSUBCON.Visible = false;
      }
      // CAR
      if (btnPerCAR.Visible == true)
      {
        this.department = "CAR";
        btnPerCAR.Visible = false;
      }
      // ASSHW
      if (btnPerASSHW.Visible == true)
      {
        this.department = "ASSHW";
        btnPerASSHW.Visible = false;
      }
      // FFHW
      if (btnPerFFHW.Visible == true)
      {
        this.department = "FFHW";
        btnPerFFHW.Visible = false;
      }
      // MAT
      if (btnPerMAT.Visible == true)
      {
        this.department = "MAT";
        btnPerMAT.Visible = false;
      }


      //--- Lock
      // COM1
      if (btnPerLockCOM1.Visible == true)
      {
        isLockCOM1 = true;
        chkCOM1.Visible = true;
        btnPerLockCOM1.Visible = false;
      }

      // ASSY
      if (btnPerLockASSY.Visible == true)
      {
        isLockASSY = true;
        chkASSY.Visible = true;
        btnPerLockASSY.Visible = false;
      }

      // SUBCON
      if (btnPerLockSUBCON.Visible == true)
      {
        isLockSUBCON = true;
        chkSUBCON.Visible = true;
        btnPerLockSUBCON.Visible = false;
      }

      // Carcass
      if (btnPerLockCAR.Visible == true)
      {
        isLockCAR = true;
        chkCAR.Visible = true;
        btnPerLockCAR.Visible = false;
      }

      // ASS HW
      if (btnPerLockASSHW.Visible == true)
      {
        isLockASSHW = true;
        btnPerLockASSHW.Visible = false;
      }

      // MAT
      if (btnPerLockMaterial.Visible == true)
      {
        isLockMaterial = true;
        btnPerLockMaterial.Visible = false;
      }

      // FFHW
      if (btnPerLockFinalFittingHW.Visible == true)
      {
        isLockFinalFittingHW = true;
        btnPerLockFinalFittingHW.Visible = false;
      }

      //--- Confirm
      // COM1
      if (btnPerConfirmCOM1.Visible == true)
      {
        isConfirmCOM1 = true;
        chkCOM1Confirm.Visible = true;
        btnPerConfirmCOM1.Visible = false;
      }

      // ASSY
      if (btnPerConfirmASSY.Visible == true)
      {
        isConfirmASSY = true;
        chkASSYConfirm.Visible = true;
        btnPerConfirmASSY.Visible = false;
      }

      // SUBCON
      if (btnPerConfirmSUBCON.Visible == true)
      {
        isConfirmSUBCON = true;
        chkSUBCONConfirm.Visible = true;
        btnPerConfirmSUBCON.Visible = false;
      }

      // CAR
      if (btnPerConfirmCAR.Visible == true)
      {
        isConfirmCAR = true;
        chkCARConfirm.Visible = true;
        btnPerConfirmCAR.Visible = false;
      }

      // ASSHW
      if (btnPerConfirmASSHW.Visible == true)
      {
        isConfirmASSHW = true;
        btnPerConfirmASSHW.Visible = false;
      }

      // Material
      if (btnPerConfirmMaterial.Visible == true)
      {
        isConfirmMaterial = true;
        btnPerConfirmMaterial.Visible = false;
      }

      // FinalFittingHW
      if (btnPerConfirmFinalFittingHW.Visible == true)
      {
        isConfirmFinalFittingHW = true;
        btnPerConfirmFinalFittingHW.Visible = false;
      }

      // See Container Information
      if (btnPerContainerInfo.Visible == true)
      {
        isContainerInfo = true;
        btnPerContainerInfo.Visible = false;
      }

      //--- Set Name WorkStation
      this.SetNameWorkStation();
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Template Import Search Deadline
    /// </summary>
    /// <returns></returns>
    private System.Data.DataTable ImportSearchData()
    {
      System.Data.DataTable dt = this.CreateDataTable();
      if (this.dtRowDetail != null && dtRowDetail.Length > 0)
      {
        for (int i = 0; i < this.dtRowDetail.Length; i++)
        {
          DataRow row = dt.NewRow();
          row["WO"] = DBConvert.ParseLong(dtRowDetail[i]["WO"]);
          row["CarcassCode"] = dtRowDetail[i]["CarcassCode"];
          row["ItemCode"] = dtRowDetail[i]["ItemCode"];
          row["Revision"] = DBConvert.ParseInt(dtRowDetail[i]["Revision"]);
          if (dtRowDetail[i]["PackingDeadline"].ToString().Trim().Length > 0)
          {
            row["Deadline"] = DBConvert.ParseDateTime(dtRowDetail[i]["PackingDeadline"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }
          row["Qty"] = DBConvert.ParseInt(dtRowDetail[i]["Qty"]);
          dt.Rows.Add(row);
        }
      }
      return dt;
    }

    /// <summary>
    /// Search Deadline
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      SqlDBParameter[] input = new SqlDBParameter[11];

      // Estimate From
      if (ultDTEstimateShipdateFrom.Value != null)
      {
        input[0] = new SqlDBParameter("@EstimateShipDateFrom", SqlDbType.DateTime, DBConvert.ParseDateTime(ultDTEstimateShipdateFrom.Value.ToString(), ConstantClass.FORMAT_DATETIME));
      }

      // Estimate To
      if (ultDTEstimateShipdateTo.Value != null)
      {
        input[1] = new SqlDBParameter("@EstimateShipDateTo", SqlDbType.DateTime, DBConvert.ParseDateTime(ultDTEstimateShipdateTo.Value.ToString(), ConstantClass.FORMAT_DATETIME));
      }

      // Loading Date From
      if (ultDTLoadingDateFrom.Value != null)
      {
        input[2] = new SqlDBParameter("@LoadingDateFrom", SqlDbType.DateTime, DBConvert.ParseDateTime(ultDTLoadingDateFrom.Value.ToString(), ConstantClass.FORMAT_DATETIME));
      }

      // LoadingDate To
      if (ultDTLoadingDateTo.Value != null)
      {
        input[3] = new SqlDBParameter("@LoadingDateTo", SqlDbType.DateTime, DBConvert.ParseDateTime(ultDTLoadingDateTo.Value.ToString(), ConstantClass.FORMAT_DATETIME));
      }

      // Packing Deadline From
      if (ultDTPackingDeadlineFrom.Value != null)
      {
        input[4] = new SqlDBParameter("@PackingDeadlineFrom", SqlDbType.DateTime, DBConvert.ParseDateTime(ultDTPackingDeadlineFrom.Value.ToString(), ConstantClass.FORMAT_DATETIME));
      }

      // Packing Deadline To
      if (ultDTPackingDeadlineTo.Value != null)
      {
        input[5] = new SqlDBParameter("@PackingDeadlineTo", SqlDbType.DateTime, DBConvert.ParseDateTime(ultDTPackingDeadlineTo.Value.ToString(), ConstantClass.FORMAT_DATETIME));
      }

      // WO
      if (DBConvert.ParseLong(txtWorkOrder.Text) != long.MinValue)
      {
        input[6] = new SqlDBParameter("@WO", SqlDbType.BigInt, DBConvert.ParseLong(txtWorkOrder.Text));
      }

      // CarcassCode
      if (txtCarcassCode.Text.Trim().Length > 0)
      {
        input[7] = new SqlDBParameter("@CarcassCode", SqlDbType.Text, txtCarcassCode.Text.Trim());
      }

      // ItemCode
      if (txtItemCode.Text.Trim().Length > 0)
      {
        input[8] = new SqlDBParameter("@ItemCode", SqlDbType.Text, txtItemCode.Text.Trim());
      }

      System.Data.DataTable dtImport = this.ImportSearchData();
      input[9] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtImport);
      this.dtRowDetail = null;

      // IsCombine
      if (chkIsCombine.Checked)
      {
        input[10] = new SqlDBParameter("@IsCombine", SqlDbType.Int, 1);
      }
      else
      {
        input[10] = new SqlDBParameter("@IsCombine", SqlDbType.Int, 0);
      }

      // ----------
      // Result
      System.Data.DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNProductionPlanOverviewFinal_Select", 6000, input);
      if (dtResultDeadline != null)
      {
        ultData.DataSource = dtResultDeadline;

        // Set Status Column
        this.SetStatusControl();

        // Load Column Hide/ Unhide
        if (ultShowColumn.Rows.Count == 0)
        {
          this.LoadColumnName();
        }
        else
        {
          this.SetStatusColumn();
        }
      }
      else
      {
        ultData.DataSource = DBNull.Value;
      }

      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Set Name WorkStation
    /// </summary>
    private void SetNameWorkStation()
    {
      labWorkStation.Text = this.department;
      if (this.department == "PLN")
      {
        btnImportDeadline.Text = "PLN Suggest PKDL";
      }
      else
      {
        btnImportDeadline.Text = this.department + " Input DL";
      }
    }

    /// <summary>
    /// Set Status Column On Grid
    /// </summary>
    private void SetStatusControl()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["AlertSugCurPKDL"].Value.ToString()) == 1)
        {
          ultData.Rows[i].CellAppearance.ForeColor = Color.Red;
          ultData.Rows[i].CellAppearance.FontData.Bold = DefaultableBoolean.True;
        }

        //Lock CAR
        if (DBConvert.ParseInt(row.Cells["CARBeforeLocked"].Value.ToString()) >= 1
          //|| row.Cells["PackingDeadline"].Value.ToString().Length == 0
          || row.Cells["CARInputDeadline"].Value.ToString().Length == 0)
        {
          ultData.Rows[i].Cells["CARLocked"].Activation = Activation.ActivateOnly;
        }

        //Confirm CAR
        if (DBConvert.ParseInt(row.Cells["CARBeforeConfirmed"].Value.ToString()) == 1
          //|| row.Cells["PackingDeadline"].Value.ToString().Length == 0
          //|| row.Cells["CARInputDeadline"].Value.ToString().Length == 0
          || DBConvert.ParseInt(row.Cells["CARLocked"].Value.ToString()) == 0 || this.department == row.Cells["CARDepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["CARConfirmed"].Activation = Activation.ActivateOnly;
        }

        //Lock COM1
        if (DBConvert.ParseInt(row.Cells["COM1BeforeLocked"].Value.ToString()) >= 1
          //|| row.Cells["COM1Deadline"].Value.ToString().Length == 0
          || row.Cells["COM1InputDeadline"].Value.ToString().Length == 0)
        {
          ultData.Rows[i].Cells["COM1Locked"].Activation = Activation.ActivateOnly;
        }

        //Confirm COM1
        if (DBConvert.ParseInt(row.Cells["COM1BeforeConfirmed"].Value.ToString()) == 1
          //|| row.Cells["COM1Deadline"].Value.ToString().Length == 0
          //|| row.Cells["COM1InputDeadline"].Value.ToString().Length == 0
          || DBConvert.ParseInt(row.Cells["COM1Locked"].Value.ToString()) == 0 || this.department == row.Cells["COM1DepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["COM1Confirmed"].Activation = Activation.ActivateOnly;
        }

        //Lock SUBCON
        if (DBConvert.ParseInt(row.Cells["SUBCONBeforeLocked"].Value.ToString()) >= 1
          //|| row.Cells["SUBCONDeadline"].Value.ToString().Length == 0
          || row.Cells["SUBCONInputDeadline"].Value.ToString().Length == 0)
        {
          ultData.Rows[i].Cells["SUBCONLocked"].Activation = Activation.ActivateOnly;
        }

        //Confirm SUBCON
        if (DBConvert.ParseInt(row.Cells["SUBCONBeforeConfirmed"].Value.ToString()) == 1
          //|| row.Cells["SUBCONDeadline"].Value.ToString().Length == 0
          //|| row.Cells["SUBCONInputDeadline"].Value.ToString().Length == 0
          || DBConvert.ParseInt(row.Cells["SUBCONLocked"].Value.ToString()) == 0 || this.department == row.Cells["SUBCONDepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["SUBCONConfirmed"].Activation = Activation.ActivateOnly;
        }

        //Lock ASSY
        if (DBConvert.ParseInt(row.Cells["ASSYBeforeLocked"].Value.ToString()) >= 1
          //|| row.Cells["ASSYDeadline"].Value.ToString().Length == 0
          || row.Cells["ASSYInputDeadline"].Value.ToString().Length == 0)
        {
          ultData.Rows[i].Cells["ASSYLocked"].Activation = Activation.ActivateOnly;
        }

        //Confirm ASSY
        if (DBConvert.ParseInt(row.Cells["ASSYBeforeConfirmed"].Value.ToString()) == 1
          //|| row.Cells["ASSYDeadline"].Value.ToString().Length == 0
          //|| row.Cells["ASSYInputDeadline"].Value.ToString().Length == 0
          || DBConvert.ParseInt(row.Cells["ASSYLocked"].Value.ToString()) == 0 || this.department == row.Cells["ASSYDepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["ASSYConfirmed"].Activation = Activation.ActivateOnly;
        }

        //Lock ASSHW
        if (DBConvert.ParseInt(row.Cells["ASSHWBeforeLocked"].Value.ToString()) >= 1
          //|| row.Cells["ASSHWDeadline"].Value.ToString().Length == 0
          || row.Cells["ASSHWInputDeadline"].Value.ToString().Length == 0)
        {
          ultData.Rows[i].Cells["ASSHWLocked"].Activation = Activation.ActivateOnly;
        }

        //Confirm ASSHW
        if (DBConvert.ParseInt(row.Cells["ASSHWBeforeConfirmed"].Value.ToString()) == 1
          //|| row.Cells["ASSHWDeadline"].Value.ToString().Length == 0
          //|| row.Cells["ASSHWInputDeadline"].Value.ToString().Length == 0
          || DBConvert.ParseInt(row.Cells["ASSHWLocked"].Value.ToString()) == 0 || this.department == row.Cells["ASSHWDepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["ASSHWConfirmed"].Activation = Activation.ActivateOnly;
        }

        //Lock FFHW
        if (DBConvert.ParseInt(row.Cells["FFHWBeforeLocked"].Value.ToString()) >= 1
          //|| row.Cells["FFHWDeadline"].Value.ToString().Length == 0
          || row.Cells["FFHWInputDeadline"].Value.ToString().Length == 0)
        {
          ultData.Rows[i].Cells["FFHWLocked"].Activation = Activation.ActivateOnly;
        }

        //Confirm FFHW
        if (DBConvert.ParseInt(row.Cells["FFHWBeforeConfirmed"].Value.ToString()) == 1
          //|| row.Cells["FFHWDeadline"].Value.ToString().Length == 0
          //|| row.Cells["FFHWInputDeadline"].Value.ToString().Length == 0
          || DBConvert.ParseInt(row.Cells["FFHWLocked"].Value.ToString()) == 0 || this.department == row.Cells["FFHWDepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["FFHWConfirmed"].Activation = Activation.ActivateOnly;
        }

        //Lock MAT
        if (DBConvert.ParseInt(row.Cells["MATBeforeLocked"].Value.ToString()) >= 1
          //|| row.Cells["MATDeadline"].Value.ToString().Length == 0
          || row.Cells["MATInputDeadline"].Value.ToString().Length == 0)
        {
          ultData.Rows[i].Cells["MATLocked"].Activation = Activation.ActivateOnly;
        }

        //Confirm MAT
        if (DBConvert.ParseInt(row.Cells["MATBeforeConfirmed"].Value.ToString()) == 1
          //|| row.Cells["MATDeadline"].Value.ToString().Length == 0
          //|| row.Cells["MATInputDeadline"].Value.ToString().Length == 0
          || DBConvert.ParseInt(row.Cells["MATLocked"].Value.ToString()) == 0 || this.department == row.Cells["MATDepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["MATConfirmed"].Activation = Activation.ActivateOnly;
        }

        //Make Transaction
        //if (DBConvert.ParseInt(row.Cells["BeforeMakeTransaction"].Value.ToString()) == 1)
        //{
        //  ultData.Rows[i].Cells["MakeTransaction"].Activation = Activation.ActivateOnly;
        //}
      }
    }

    ///<summary>
    ///Get data for clipboard for coppy excel
    ///</summary>
    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        Utility.GetDataForClipboard(ultData);
      }
    }

    /// <summary>
    /// Load Column Name
    /// </summary>
    private void LoadColumnName()
    {
      System.Data.DataTable dtNew = new System.Data.DataTable();
      System.Data.DataTable dtColumn = (System.Data.DataTable)ultData.DataSource;
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        if (ultData.DisplayLayout.Bands[0].Columns[column.ColumnName].Hidden)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 0;
        }
        else
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      ultShowColumn.DataSource = dtNew;
      ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
    }

    /// <summary>
    /// Set Status Column When Search
    /// </summary>
    private void SetStatusColumn()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// Import Search Deadline
    /// </summary>
    /// <returns></returns>
    private System.Data.DataTable CreateDataTable()
    {
      System.Data.DataTable dt = new System.Data.DataTable();
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("Deadline", typeof(System.DateTime));
      dt.Columns.Add("Reason", typeof(System.Int32));
      return dt;
    }

    /// <summary>
    /// Import Deadline
    /// </summary>
    private void ImportDeadline()
    {
      this.dtRowDetail = null;
      viewPLN_29_002 uc = new viewPLN_29_002();
      uc.parentUC = this;
      uc.strWorkStation = this.department;

      WindowUtinity.ShowView(uc, "IMPORT DATA FOR CHANGE DEADLINE", false, ViewState.ModalWindow, FormWindowState.Maximized);
      if (uc.flagSearch == true)
      {
        this.SearchData();
      }
    }

    /// <summary>
    /// Check Data
    /// </summary>
    /// <returns></returns>
    private bool CheckData()
    {
      bool result = true;
      return result;
    }

    private System.Data.DataTable CreateDatatableConfirmedDeadline()
    {
      System.Data.DataTable dt = new System.Data.DataTable();
      dt.Columns.Add("WorkAreaPid", typeof(System.Int64));
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("StringHangtag", typeof(System.String));

      //Comp1
      dt.Columns.Add("COM1BeforeLocked", typeof(System.Int32));
      dt.Columns.Add("COM1Locked", typeof(System.Int32));
      dt.Columns.Add("COM1Confirmed", typeof(System.Int32));

      //ASSY
      dt.Columns.Add("ASSYBeforeLocked", typeof(System.Int32));
      dt.Columns.Add("ASSYLocked", typeof(System.Int32));
      dt.Columns.Add("ASSYConfirmed", typeof(System.Int32));

      //Subcon
      dt.Columns.Add("SUBCONBeforeLocked", typeof(System.Int32));
      dt.Columns.Add("SUBCONLocked", typeof(System.Int32));
      dt.Columns.Add("SUBCONConfirmed", typeof(System.Int32));

      //CAR
      dt.Columns.Add("CARBeforeLocked", typeof(System.Int32));
      dt.Columns.Add("CARLocked", typeof(System.Int32));
      dt.Columns.Add("CARConfirmed", typeof(System.Int32));

      //ASSHW
      dt.Columns.Add("ASSHWBeforeLocked", typeof(System.Int32));
      dt.Columns.Add("ASSHWLocked", typeof(System.Int32));
      dt.Columns.Add("ASSHWConfirmed", typeof(System.Int32));

      //FFHW
      dt.Columns.Add("FFHWBeforeLocked", typeof(System.Int32));
      dt.Columns.Add("FFHWLocked", typeof(System.Int32));
      dt.Columns.Add("FFHWConfirmed", typeof(System.Int32));

      //MAT
      dt.Columns.Add("MATBeforeLocked", typeof(System.Int32));
      dt.Columns.Add("MATLocked", typeof(System.Int32));
      dt.Columns.Add("MATConfirmed", typeof(System.Int32));

      return dt;
    }


    /// <summary>
    /// Save Confirm Deadline
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      System.Data.DataTable dtSource = (System.Data.DataTable)ultData.DataSource;
      System.Data.DataTable dtInput = this.CreateDatatableConfirmedDeadline();

      //Input
      SqlDBParameter[] sqlinput = new SqlDBParameter[3];
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        //DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtInput.NewRow();
        if (ultData.Rows[i].IsFilteredOut == false && DBConvert.ParseInt(ultData.Rows[i].Cells["IsModify"].Value.ToString()) == 1)
        {
          //WorkAreaPid
          if (DBConvert.ParseLong(ultData.Rows[i].Cells["WorkAreaPid"].Value.ToString()) != long.MinValue)
          {
            rowadd["WorkAreaPid"] = DBConvert.ParseLong(ultData.Rows[i].Cells["WorkAreaPid"].Value.ToString());
          }
          //WO
          if (DBConvert.ParseLong(ultData.Rows[i].Cells["WO"].Value.ToString()) != long.MinValue)
          {
            rowadd["WO"] = DBConvert.ParseLong(ultData.Rows[i].Cells["WO"].Value.ToString());
          }

          // CarcassCode
          if (ultData.Rows[i].Cells["CarcassCode"].Value.ToString().Trim().Length > 0)
          {
            rowadd["CarcassCode"] = ultData.Rows[i].Cells["CarcassCode"].Value.ToString();
          }

          // ItemCode
          if (ultData.Rows[i].Cells["ItemCode"].Value.ToString().Trim().Length > 0)
          {
            rowadd["ItemCode"] = ultData.Rows[i].Cells["ItemCode"].Value.ToString();
          }

          // Revision
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["Revision"].Value.ToString()) != int.MinValue)
          {
            rowadd["Revision"] = DBConvert.ParseInt(ultData.Rows[i].Cells["Revision"].Value.ToString());
          }

          //StringHangtag
          if (ultData.Rows[i].Cells["StringHangtag"].Value.ToString().Trim().Length > 0)
          {
            rowadd["StringHangtag"] = ultData.Rows[i].Cells["StringHangtag"].Value.ToString();
          }

          //COM1BeforeLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["COM1BeforeLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["COM1BeforeLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["COM1BeforeLocked"].Value.ToString());
          }

          //COM1Locked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["COM1Locked"].Value.ToString()) != int.MinValue)
          {
            rowadd["COM1Locked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["COM1Locked"].Value.ToString());
          }

          //COM1Confirmed
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["COM1Confirmed"].Value.ToString()) != int.MinValue)
          {
            rowadd["COM1Confirmed"] = DBConvert.ParseInt(ultData.Rows[i].Cells["COM1Confirmed"].Value.ToString());
          }

          //ASSYBeforeLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["ASSYBeforeLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["ASSYBeforeLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["ASSYBeforeLocked"].Value.ToString());
          }

          //ASSYLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["ASSYLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["ASSYLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["ASSYLocked"].Value.ToString());
          }

          //ASSYConfirmed
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["ASSYConfirmed"].Value.ToString()) != int.MinValue)
          {
            rowadd["ASSYConfirmed"] = DBConvert.ParseInt(ultData.Rows[i].Cells["ASSYConfirmed"].Value.ToString());
          }

          //SUBCONBeforeLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONBeforeLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["SUBCONBeforeLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONBeforeLocked"].Value.ToString());
          }

          //SUBCONLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["SUBCONLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONLocked"].Value.ToString());
          }

          //SUBCONConfirmed
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONConfirmed"].Value.ToString()) != int.MinValue)
          {
            rowadd["SUBCONConfirmed"] = DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONConfirmed"].Value.ToString());
          }

          //SUBCONLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["SUBCONLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONLocked"].Value.ToString());
          }

          //SUBCONConfirmed
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONConfirmed"].Value.ToString()) != int.MinValue)
          {
            rowadd["SUBCONConfirmed"] = DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONConfirmed"].Value.ToString());
          }

          //CARBeforeLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["CARBeforeLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["CARBeforeLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["CARBeforeLocked"].Value.ToString());
          }

          //CARLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["CARLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["CARLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["CARLocked"].Value.ToString());
          }

          //CARConfirmed
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["CARConfirmed"].Value.ToString()) != int.MinValue)
          {
            rowadd["CARConfirmed"] = DBConvert.ParseInt(ultData.Rows[i].Cells["CARConfirmed"].Value.ToString());
          }

          //ASSHWBeforeLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["ASSHWBeforeLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["ASSHWBeforeLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["ASSHWBeforeLocked"].Value.ToString());
          }

          //ASSHWLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["ASSHWLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["ASSHWLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["ASSHWLocked"].Value.ToString());
          }

          //ASSHWConfirmed
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["ASSHWConfirmed"].Value.ToString()) != int.MinValue)
          {
            rowadd["ASSHWConfirmed"] = DBConvert.ParseInt(ultData.Rows[i].Cells["ASSHWConfirmed"].Value.ToString());
          }

          //FFHWBeforeLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["FFHWBeforeLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["FFHWBeforeLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["FFHWBeforeLocked"].Value.ToString());
          }

          //FFHWLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["FFHWLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["FFHWLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["FFHWLocked"].Value.ToString());
          }

          //FFHWConfirmed
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["FFHWConfirmed"].Value.ToString()) != int.MinValue)
          {
            rowadd["FFHWConfirmed"] = DBConvert.ParseInt(ultData.Rows[i].Cells["FFHWConfirmed"].Value.ToString());
          }

          //MATBeforeLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["MATBeforeLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["MATBeforeLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["MATBeforeLocked"].Value.ToString());
          }

          //FFHWLocked
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["MATLocked"].Value.ToString()) != int.MinValue)
          {
            rowadd["MATLocked"] = DBConvert.ParseInt(ultData.Rows[i].Cells["MATLocked"].Value.ToString());
          }

          //FFHWConfirmed
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["MATConfirmed"].Value.ToString()) != int.MinValue)
          {
            rowadd["MATConfirmed"] = DBConvert.ParseInt(ultData.Rows[i].Cells["MATConfirmed"].Value.ToString());
          }

          //Add row datatable
          dtInput.Rows.Add(rowadd);
        }
      }

      sqlinput[0] = new SqlDBParameter("@CreateBy", SqlDbType.BigInt, SharedObject.UserInfo.UserPid);
      sqlinput[1] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtInput);
      sqlinput[2] = new SqlDBParameter("@Department", SqlDbType.Text, this.department);

      // Output ------
      SqlDBParameter[] sqloutput = new SqlDBParameter[1];
      sqloutput[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);

      // Result ------
      SqlDataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanConfirmedDeadline", 6000, sqlinput, sqloutput);
      long result = DBConvert.ParseLong(sqloutput[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
        Utility.ExportToExcelWithDefaultPath(ultData, "Deadline Information");

        //Excel.Workbook xlBook;

        //Utility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Deadline Information", 6);

        //string filePath = xlBook.FullName;
        //Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        //xlSheet.Cells[1, 1] = "VFR Co., LTD";
        //Excel.Range r = xlSheet.get_Range("A1", "A1");

        //xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        //xlSheet.Cells[3, 1] = "OVER VIEW DEADLINE";
        //r = xlSheet.get_Range("A3", "A3");
        //r.Font.Bold = true;
        //r.Font.Size = 14;
        //r.EntireRow.RowHeight = 20;

        //xlSheet.Cells[4, 1] = "Date: ";
        //r.Font.Bold = true;
        //xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        ////xlBook.Application.DisplayAlerts = false;

        //xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        //Process.Start(filePath);
      }
    }
    #endregion Function

    #region Event
    private void ultShowColumn_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
        }
      }
      else
      {
        for (int i = 1; i < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          row.Cells[i].Value = e.Cell.Text;
        }
        this.ChkAll_CheckedChange();
      }
    }

    private void ultShowColmn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      System.Data.DataTable dtColumn = (System.Data.DataTable)ultShowColumn.DataSource;
      int count = dtColumn.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }

      if (!this.isContainerInfo)
      {
        e.Layout.Bands[0].Columns["Container"].Hidden = true;
        e.Layout.Bands[0].Columns["LoadingDate"].Hidden = true;
        e.Layout.Bands[0].Columns["EstimateShipdate"].Hidden = true;
        e.Layout.Bands[0].Columns["OriginalEstimateShipdate"].Hidden = true;
      }

      //Information WO, Carcass, ItemCoce, Revision, PackingDeadline, WorkArea, Container, Qty
      e.Layout.Bands[0].Columns["CarcassWorkOrder"].Hidden = true;
      e.Layout.Bands[0].Columns["WO"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["PackingDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["WorkArea"].Hidden = true;
      e.Layout.Bands[0].Columns["Qty"].Hidden = true;
      e.Layout.Bands[0].Columns["AlertSugCurPKDL"].Hidden = true;
      //e.Layout.Bands[0].Columns["USStock"].Hidden = true;

      e.Layout.Bands[0].Columns["IsModify"].Hidden = true;
      e.Layout.Bands[0].Columns["WorkAreaPid"].Hidden = true;
      e.Layout.Bands[0].Columns["BeforeMakeTransaction"].Hidden = true;
      e.Layout.Bands[0].Columns["TransactionPid"].Hidden = true;

      //BeforeLocked
      e.Layout.Bands[0].Columns["COM1BeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSYBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["SUBCONBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["CARBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSHWBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["FFHWBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["MATBeforeLocked"].Hidden = true;

      //BeforeConfirmed
      e.Layout.Bands[0].Columns["COM1BeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSYBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["SUBCONBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["CARBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSHWBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["FFHWBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["MATBeforeConfirmed"].Hidden = true;

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 3;

      e.Layout.Bands[0].Columns["StringHangtag"].Header.Caption = "KeyInput";
      e.Layout.Bands[0].Columns["PLNSuggestDeadline"].Header.Caption = "PLN\n Target\n PAK Deadline";
      e.Layout.Bands[0].Columns["SuggestPackingDeadline"].Header.Caption = "PAK Deadline\n Suggested\n Base on Draft DL";
      e.Layout.Bands[0].Columns["SuggestPackingDeadlineFromCurDL"].Header.Caption = "PAK Deadline\n Suggested\n Base on Current DL";
      e.Layout.Bands[0].Columns["COM1Deadline"].Header.Caption = "Current\n COM1\n Deadline";
      e.Layout.Bands[0].Columns["ASSYDeadline"].Header.Caption = "Current\n ASSY + SAN\n Deadline";
      e.Layout.Bands[0].Columns["SUBCONDeadline"].Header.Caption = "Current\n SUBCON\n Deadline";

      // Department Input Confirmed Deadline
      e.Layout.Bands[0].Columns["COM1DepartmentInput"].Header.Caption = "COM1\n Confirmed \n Deadline By";
      e.Layout.Bands[0].Columns["ASSYDepartmentInput"].Header.Caption = "ASSY + SAN\n Confirmed \n Deadline By";
      e.Layout.Bands[0].Columns["SUBCONDepartmentInput"].Header.Caption = "SUBCON\n Confirmed \n Deadline By";
      e.Layout.Bands[0].Columns["CARDepartmentInput"].Header.Caption = "CAR\n Confirmed \n Deadline By";
      e.Layout.Bands[0].Columns["ASSHWDepartmentInput"].Header.Caption = "ASSHW\n Confirmed \n Deadline By";
      e.Layout.Bands[0].Columns["FFHWDepartmentInput"].Header.Caption = "FFHW\n Confirmed \n Deadline By";
      e.Layout.Bands[0].Columns["MATDepartmentInput"].Header.Caption = "MAT\n Confirmed \n Deadline By";

      // Time Input
      e.Layout.Bands[0].Columns["COM1TimeInput"].Header.Caption = "COM1\n Time Input";
      e.Layout.Bands[0].Columns["ASSYTimeInput"].Header.Caption = "ASSY + SAN\n Time Input";
      e.Layout.Bands[0].Columns["SUBCONTimeInput"].Header.Caption = "SUBCON\n Time Input";
      e.Layout.Bands[0].Columns["CARTimeInput"].Header.Caption = "CAR\n Time Input";
      e.Layout.Bands[0].Columns["ASSHWTimeInput"].Header.Caption = "ASSHW\n Time Input";
      e.Layout.Bands[0].Columns["FFHWTimeInput"].Header.Caption = "FFHW\n Time Input";
      e.Layout.Bands[0].Columns["MATTimeInput"].Header.Caption = "MAT\n Time Input";

      //Input deadline
      e.Layout.Bands[0].Columns["COM1InputDeadline"].Header.Caption = "COM1\n Confirmed \n Deadline";
      e.Layout.Bands[0].Columns["ASSYInputDeadline"].Header.Caption = "ASSY + SAN\n Confirmed\n Deadline";
      e.Layout.Bands[0].Columns["SUBCONInputDeadline"].Header.Caption = "SUBCON\n Confirmed\n Deadline";
      e.Layout.Bands[0].Columns["CARInputDeadline"].Header.Caption = "CAR\n Confirmed\n Deadline";

      //Suggest Deadline tung khu vuc
      e.Layout.Bands[0].Columns["COM1SuggestDeadline"].Header.Caption = "Target\n COM1\n Deadline";
      e.Layout.Bands[0].Columns["ASSYSuggestDeadline"].Header.Caption = "Target\n ASSY + SAN\n Deadline";
      e.Layout.Bands[0].Columns["SUBCONSuggestDeadline"].Header.Caption = "Target\n SUBCON\n Deadline";
      e.Layout.Bands[0].Columns["CARSuggestDeadline"].Header.Caption = "Target\n CAR\n Deadline";
      e.Layout.Bands[0].Columns["ASSHWSuggestDeadline"].Header.Caption = "Target\n ASSHW\n Deadline";
      e.Layout.Bands[0].Columns["MATSuggestDeadline"].Header.Caption = "Target\n MAT\n Deadline";
      e.Layout.Bands[0].Columns["FFHWSuggestDeadline"].Header.Caption = "Target\n FFHW\n Deadline";

      // Remark
      e.Layout.Bands[0].Columns["COM1Remark"].Header.Caption = "COM1\n Remark";
      e.Layout.Bands[0].Columns["ASSYRemark"].Header.Caption = "ASSY + SAN\n Remark";
      e.Layout.Bands[0].Columns["SUBCONRemark"].Header.Caption = "SUBCON\n Remark";
      e.Layout.Bands[0].Columns["CARRemark"].Header.Caption = "CAR\n Remark";
      e.Layout.Bands[0].Columns["ASSHWRemark"].Header.Caption = "ASSHW\n Remark";
      e.Layout.Bands[0].Columns["FFHWRemark"].Header.Caption = "FFHW\n Remark";
      e.Layout.Bands[0].Columns["MATRemark"].Header.Caption = "MAT\n Remark";

      //Alert
      e.Layout.Bands[0].Columns["COM1Alert"].Header.Caption = "COM1\n Alert";
      e.Layout.Bands[0].Columns["ASSYAlert"].Header.Caption = "ASSY + SAN\n Alert";
      e.Layout.Bands[0].Columns["SUBCONAlert"].Header.Caption = "SUBCON\n Alert";
      e.Layout.Bands[0].Columns["CARAlert"].Header.Caption = "CAR\n Alert";

      //Locked tung khu vuc
      e.Layout.Bands[0].Columns["COM1Locked"].Header.Caption = "COM1\n Locked";
      e.Layout.Bands[0].Columns["ASSYLocked"].Header.Caption = "ASSY + SAN\n Locked";
      e.Layout.Bands[0].Columns["SUBCONLocked"].Header.Caption = "SUBCON\n Locked";
      e.Layout.Bands[0].Columns["CARLocked"].Header.Caption = "CAR\n Locked";

      //Confirm tung khu vuc
      e.Layout.Bands[0].Columns["COM1Confirmed"].Header.Caption = "COM1\n Confirmed";
      e.Layout.Bands[0].Columns["ASSYConfirmed"].Header.Caption = "ASSY + SAN\n Confirmed";
      e.Layout.Bands[0].Columns["SUBCONConfirmed"].Header.Caption = "SUBCON\n Confirmed";
      e.Layout.Bands[0].Columns["CARConfirmed"].Header.Caption = "CAR\n Confirmed";

      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.UseFixedHeaders = true;

      e.Layout.Bands[0].ColHeaderLines = 3;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[0].Columns["MakeTransaction"].CellActivation = Activation.AllowEdit;
      }

      // Color
      e.Layout.Bands[0].Columns["CARSuggestDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["CARInputDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["CARReason"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["CARAlert"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["CARLocked"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["CARConfirmed"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["CARDepartmentInput"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["CARTimeInput"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["CARRemark"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);

      //COM1
      e.Layout.Bands[0].Columns["COM1Deadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1SuggestDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1InputDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1Reason"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1Alert"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1Locked"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1Confirmed"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1DepartmentInput"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1TimeInput"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["COM1Remark"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);

      //SUBCON
      e.Layout.Bands[0].Columns["SUBCONDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBCONSuggestDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBCONInputDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBCONReason"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBCONAlert"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBCONLocked"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBCONConfirmed"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBCONDepartmentInput"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBCONTimeInput"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["SUBCONRemark"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);

      //ASSY
      e.Layout.Bands[0].Columns["ASSYDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYSuggestDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYInputDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYReason"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYAlert"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYLocked"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYConfirmed"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYDepartmentInput"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYTimeInput"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["ASSYRemark"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);

      //ASSHW
      e.Layout.Bands[0].Columns["ASSHWStatus"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWSuggestDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWInputDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWReason"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWAlert"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWLocked"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWConfirmed"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWDepartmentInput"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWTimeInput"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["ASSHWRemark"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);

      //MAT
      e.Layout.Bands[0].Columns["MATStatus"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATSuggestDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATInputDeadline"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATReason"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATAlert"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATLocked"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATConfirmed"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATDepartmentInput"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATTimeInput"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);
      e.Layout.Bands[0].Columns["MATRemark"].CellAppearance.BackColor = Color.FromArgb(141, 180, 227);

      //FFHW
      e.Layout.Bands[0].Columns["FFHWStatus"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWSuggestDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWInputDeadline"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWReason"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWAlert"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWLocked"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWConfirmed"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWDepartmentInput"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWTimeInput"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      e.Layout.Bands[0].Columns["FFHWRemark"].CellAppearance.BackColor = Color.FromArgb(250, 192, 144);
      // Color

      e.Layout.Bands[0].Columns["CarcassWorkOrder"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["WO"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Revision"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["PackingDeadline"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Qty"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["WorkArea"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Container"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["PackingDeadline"].Format = "dd-MMM-yyyy";
      e.Layout.Bands[0].Columns["SuggestPackingDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["SuggestPackingDeadlineFromCurDL"].Hidden = true;
      e.Layout.Bands[0].Columns["PLNSuggestDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["SO"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerGroup"].Hidden = true;
      e.Layout.Bands[0].Columns["UCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["WorkAreaPid"].Hidden = true;
      e.Layout.Bands[0].Columns["FurnitureCode"].Hidden = true;
      e.Layout.Bands[0].Columns["AlertSugCurPKDL"].Hidden = true;
      e.Layout.Bands[0].Columns["BeforeMakeTransaction"].Hidden = true;
      e.Layout.Bands[0].Columns["TransactionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["USStock"].Hidden = true;
      e.Layout.Bands[0].Columns["Quarantine"].Hidden = true;

      if (!this.isContainerInfo)
      {
        e.Layout.Bands[0].Columns["Container"].Hidden = true;
        e.Layout.Bands[0].Columns["LoadingDate"].Hidden = true;
        e.Layout.Bands[0].Columns["EstimateShipdate"].Hidden = true;
        e.Layout.Bands[0].Columns["OriginalEstimateShipdate"].Hidden = true;
        e.Layout.Bands[0].Columns["PLNSuggestDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["SuggestPackingDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["SuggestPackingDeadlineFromCurDL"].Hidden = true;
        e.Layout.Bands[0].Columns["SO"].Hidden = true;
        e.Layout.Bands[0].Columns["CustomerGroup"].Hidden = true;
        e.Layout.Bands[0].Columns["UCBM"].Hidden = true;
        e.Layout.Bands[0].Columns["CarcassCBM"].Hidden = true;
        e.Layout.Bands[0].Columns["UVP"].Hidden = true;
        e.Layout.Bands[0].Columns["TotalVP"].Hidden = true;
        e.Layout.Bands[0].Columns["Quarantine"].Hidden = true;
      }

      //COM1
      if (this.isLockCOM1 == false && this.isConfirmCOM1 == false)
      {
        e.Layout.Bands[0].Columns["COM1Deadline"].Hidden = true;
        e.Layout.Bands[0].Columns["COM1InputDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["COM1SuggestDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["COM1Alert"].Hidden = true;
        e.Layout.Bands[0].Columns["COM1Locked"].Hidden = true;
        e.Layout.Bands[0].Columns["COM1Confirmed"].Hidden = true;
        e.Layout.Bands[0].Columns["COM1DepartmentInput"].Hidden = true;
        e.Layout.Bands[0].Columns["COM1TimeInput"].Hidden = true;
        e.Layout.Bands[0].Columns["COM1Reason"].Hidden = true;
        e.Layout.Bands[0].Columns["COM1Remark"].Hidden = true;
      }
      else if (this.isLockCOM1 == true && this.isConfirmCOM1 == false)
      {
        e.Layout.Bands[0].Columns["COM1Locked"].CellActivation = Activation.AllowEdit;
      }
      else if (this.isLockCOM1 == false && this.isConfirmCOM1 == true)
      {
        e.Layout.Bands[0].Columns["COM1Confirmed"].CellActivation = Activation.AllowEdit;
      }
      else
      {
        e.Layout.Bands[0].Columns["COM1Locked"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["COM1Confirmed"].CellActivation = Activation.AllowEdit;
      }

      //ASSY
      if (this.isLockASSY == false && this.isConfirmASSY == false)
      {
        e.Layout.Bands[0].Columns["ASSYDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSYInputDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSYSuggestDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSYAlert"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSYLocked"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSYConfirmed"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSYDepartmentInput"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSYTimeInput"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSYReason"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSYRemark"].Hidden = true;
      }
      else if (this.isLockASSY == true && this.isConfirmASSY == false)
      {
        e.Layout.Bands[0].Columns["ASSYLocked"].CellActivation = Activation.AllowEdit;
      }
      else if (this.isLockASSY == false && this.isConfirmASSY == true)
      {
        e.Layout.Bands[0].Columns["ASSYConfirmed"].CellActivation = Activation.AllowEdit;
      }
      else
      {
        e.Layout.Bands[0].Columns["ASSYLocked"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["ASSYConfirmed"].CellActivation = Activation.AllowEdit;
      }

      //SUBCON
      if (this.isLockSUBCON == false && this.isConfirmSUBCON == false)
      {
        e.Layout.Bands[0].Columns["SUBCONDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["SUBCONInputDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["SUBCONSuggestDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["SUBCONAlert"].Hidden = true;
        e.Layout.Bands[0].Columns["SUBCONLocked"].Hidden = true;
        e.Layout.Bands[0].Columns["SUBCONConfirmed"].Hidden = true;
        e.Layout.Bands[0].Columns["SUBCONDepartmentInput"].Hidden = true;
        e.Layout.Bands[0].Columns["SUBCONTimeInput"].Hidden = true;
        e.Layout.Bands[0].Columns["SUBCONReason"].Hidden = true;
        e.Layout.Bands[0].Columns["SUBCONRemark"].Hidden = true;
      }
      else if (this.isLockSUBCON == true && this.isConfirmSUBCON == false)
      {
        e.Layout.Bands[0].Columns["SUBCONLocked"].CellActivation = Activation.AllowEdit;
      }
      else if (this.isLockSUBCON == false && this.isConfirmSUBCON == true)
      {
        e.Layout.Bands[0].Columns["SUBCONConfirmed"].CellActivation = Activation.AllowEdit;
      }
      else
      {
        e.Layout.Bands[0].Columns["SUBCONLocked"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["SUBCONConfirmed"].CellActivation = Activation.AllowEdit;
      }

      //CAR
      if (this.isLockCAR == false && this.isConfirmCAR == false)
      {
        e.Layout.Bands[0].Columns["CARInputDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["CARSuggestDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["CARAlert"].Hidden = true;
        e.Layout.Bands[0].Columns["CARLocked"].Hidden = true;
        e.Layout.Bands[0].Columns["CARConfirmed"].Hidden = true;
        e.Layout.Bands[0].Columns["CARDepartmentInput"].Hidden = true;
        e.Layout.Bands[0].Columns["CARTimeInput"].Hidden = true;
        e.Layout.Bands[0].Columns["CARReason"].Hidden = true;
        e.Layout.Bands[0].Columns["CARRemark"].Hidden = true;
      }
      else if (this.isLockCAR == true && this.isConfirmCAR == false)
      {
        e.Layout.Bands[0].Columns["CARLocked"].CellActivation = Activation.AllowEdit;
      }
      else if (this.isLockCAR == false && this.isConfirmCAR == true)
      {
        e.Layout.Bands[0].Columns["CARConfirmed"].CellActivation = Activation.AllowEdit;
      }
      else
      {
        e.Layout.Bands[0].Columns["CARLocked"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["CARConfirmed"].CellActivation = Activation.AllowEdit;
      }

      //ASSHW
      if (this.isLockASSHW == false && this.isConfirmASSHW == false)
      {
        e.Layout.Bands[0].Columns["ASSHWStatus"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWInputDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWSuggestDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWAlert"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWLocked"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWConfirmed"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWDepartmentInput"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWTimeInput"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWReason"].Hidden = true;
        e.Layout.Bands[0].Columns["ASSHWRemark"].Hidden = true;
      }
      else if (this.isLockASSHW == true && this.isConfirmASSHW == false)
      {
        e.Layout.Bands[0].Columns["ASSHWLocked"].CellActivation = Activation.AllowEdit;
      }
      else if (this.isLockASSHW == false && this.isConfirmASSHW == true)
      {
        e.Layout.Bands[0].Columns["ASSHWConfirmed"].CellActivation = Activation.AllowEdit;
      }
      else
      {
        e.Layout.Bands[0].Columns["ASSHWLocked"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["ASSHWConfirmed"].CellActivation = Activation.AllowEdit;
      }

      //FFHW
      if (this.isLockFinalFittingHW == false && this.isConfirmFinalFittingHW == false)
      {
        e.Layout.Bands[0].Columns["FFHWStatus"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWInputDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWSuggestDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWAlert"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWLocked"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWConfirmed"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWDepartmentInput"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWTimeInput"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWReason"].Hidden = true;
        e.Layout.Bands[0].Columns["FFHWRemark"].Hidden = true;
      }
      else if (this.isLockFinalFittingHW == true && this.isConfirmFinalFittingHW == false)
      {
        e.Layout.Bands[0].Columns["FFHWLocked"].CellActivation = Activation.AllowEdit;
      }
      else if (this.isLockFinalFittingHW == false && this.isConfirmFinalFittingHW == true)
      {
        e.Layout.Bands[0].Columns["FFHWConfirmed"].CellActivation = Activation.AllowEdit;
      }
      else
      {
        e.Layout.Bands[0].Columns["FFHWLocked"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["FFHWConfirmed"].CellActivation = Activation.AllowEdit;
      }

      //MAT
      if (this.isLockMaterial == false && this.isConfirmMaterial == false)
      {
        e.Layout.Bands[0].Columns["MATStatus"].Hidden = true;
        e.Layout.Bands[0].Columns["MATDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["MATInputDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["MATSuggestDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["MATAlert"].Hidden = true;
        e.Layout.Bands[0].Columns["MATLocked"].Hidden = true;
        e.Layout.Bands[0].Columns["MATConfirmed"].Hidden = true;
        e.Layout.Bands[0].Columns["MATDepartmentInput"].Hidden = true;
        e.Layout.Bands[0].Columns["MATTimeInput"].Hidden = true;
        e.Layout.Bands[0].Columns["MATReason"].Hidden = true;
        e.Layout.Bands[0].Columns["MATRemark"].Hidden = true;
      }
      else if (this.isLockMaterial == true && this.isConfirmMaterial == false)
      {
        e.Layout.Bands[0].Columns["MATLocked"].CellActivation = Activation.AllowEdit;
      }
      else if (this.isLockMaterial == false && this.isConfirmMaterial == true)
      {
        e.Layout.Bands[0].Columns["MATConfirmed"].CellActivation = Activation.AllowEdit;
      }
      else
      {
        e.Layout.Bands[0].Columns["MATLocked"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["MATConfirmed"].CellActivation = Activation.AllowEdit;
      }

      e.Layout.Bands[0].Columns["COM1BeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSYBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["SUBCONBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["CARBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSHWBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["FFHWBeforeLocked"].Hidden = true;
      e.Layout.Bands[0].Columns["MATBeforeLocked"].Hidden = true;

      e.Layout.Bands[0].Columns["COM1BeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSYBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["SUBCONBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["CARBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSHWBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["FFHWBeforeConfirmed"].Hidden = true;
      e.Layout.Bands[0].Columns["MATBeforeConfirmed"].Hidden = true;

      e.Layout.Bands[0].Columns["IsModify"].Hidden = true;

      e.Layout.Bands[0].Columns["CarcassWorkOrder"].Header.Caption = "Carcass\n WorkOrder";
      e.Layout.Bands[0].Columns["StringHangtag"].Header.Caption = "KeyInput";
      e.Layout.Bands[0].Columns["Revision"].Header.Caption = "Carcass q'ty";
      e.Layout.Bands[0].Columns["Revision"].Header.Caption = "Rev.";
      e.Layout.Bands[0].Columns["PackingDeadline"].Header.Caption = "Current\n Packing\n Deadline";
      e.Layout.Bands[0].Columns["PLNSuggestDeadline"].Header.Caption = "PLN\n Target\n PAK Deadline";
      e.Layout.Bands[0].Columns["SuggestPackingDeadline"].Header.Caption = "PAK Deadline\n Suggested\n Base on Draft DL";
      e.Layout.Bands[0].Columns["SuggestPackingDeadlineFromCurDL"].Header.Caption = "PAK Deadline\n Suggested\n Base on Current DL";
      e.Layout.Bands[0].Columns["COM1Deadline"].Header.Caption = "Current\n COM1\n Deadline";
      e.Layout.Bands[0].Columns["ASSYDeadline"].Header.Caption = "Current\n ASSY + SAN\n Deadline";
      e.Layout.Bands[0].Columns["SUBCONDeadline"].Header.Caption = "Current\n SUBCON\n Deadline";
      e.Layout.Bands[0].Columns["ASSHWDeadline"].Header.Caption = "Current\n ASSHW\n Deadline";
      e.Layout.Bands[0].Columns["FFHWDeadline"].Header.Caption = "Current\n FFHW\n Deadline";
      e.Layout.Bands[0].Columns["MATDeadline"].Header.Caption = "Current\n MAT\n Deadline";

      e.Layout.Bands[0].Columns["COM1Remark"].Header.Caption = "COM1\n Remark";
      e.Layout.Bands[0].Columns["ASSYRemark"].Header.Caption = "ASSY + SAN\n Remark";
      e.Layout.Bands[0].Columns["SUBCONRemark"].Header.Caption = "SUBCON\n Remark";
      e.Layout.Bands[0].Columns["CARRemark"].Header.Caption = "CAR\n Remark";
      e.Layout.Bands[0].Columns["ASSHWRemark"].Header.Caption = "ASSHW\n Remark";
      e.Layout.Bands[0].Columns["FFHWRemark"].Header.Caption = "FFHW\n Remark";
      e.Layout.Bands[0].Columns["MATRemark"].Header.Caption = "MAT\n Remark";

      e.Layout.Bands[0].Columns["COM1InputDeadline"].Header.Caption = "COM1\n Confirmed \n Deadline";
      e.Layout.Bands[0].Columns["ASSYInputDeadline"].Header.Caption = "ASSY + SAN\n Confirmed\n Deadline";
      e.Layout.Bands[0].Columns["SUBCONInputDeadline"].Header.Caption = "SUBCON\n Confirmed\n Deadline";
      e.Layout.Bands[0].Columns["CARInputDeadline"].Header.Caption = "CAR\n Confirmed\n Deadline";
      e.Layout.Bands[0].Columns["ASSHWInputDeadline"].Header.Caption = "ASSHW\n Confirmed\n Deadline";
      e.Layout.Bands[0].Columns["FFHWInputDeadline"].Header.Caption = "FFHW\n Confirmed\n Deadline";
      e.Layout.Bands[0].Columns["MATInputDeadline"].Header.Caption = "MAT\n Confirmed\n Deadline";

      e.Layout.Bands[0].Columns["COM1DepartmentInput"].Header.Caption = "COM1\n Confirmed\n Deadline By";
      e.Layout.Bands[0].Columns["ASSYDepartmentInput"].Header.Caption = "ASSY + SAN\n Confirmed\n Deadline By";
      e.Layout.Bands[0].Columns["SUBCONDepartmentInput"].Header.Caption = "SUBCON\n Confirmed\n Deadline By";
      e.Layout.Bands[0].Columns["CARDepartmentInput"].Header.Caption = "CAR\n Confirmed\n Deadline By";
      e.Layout.Bands[0].Columns["ASSHWDepartmentInput"].Header.Caption = "ASSHW\n Confirmed\n Deadline By";
      e.Layout.Bands[0].Columns["FFHWDepartmentInput"].Header.Caption = "FFHW\n Confirmed\n Deadline By";
      e.Layout.Bands[0].Columns["MATDepartmentInput"].Header.Caption = "MAT\n Confirmed\n Deadline By";

      e.Layout.Bands[0].Columns["COM1TimeInput"].Header.Caption = "COM1\n Time Input";
      e.Layout.Bands[0].Columns["ASSYTimeInput"].Header.Caption = "ASSY + SAN\n Time Input";
      e.Layout.Bands[0].Columns["SUBCONTimeInput"].Header.Caption = "SUBCON\n Time Input";
      e.Layout.Bands[0].Columns["CARTimeInput"].Header.Caption = "CAR\n Time Input";
      e.Layout.Bands[0].Columns["ASSHWTimeInput"].Header.Caption = "ASSHW\n Time Input";
      e.Layout.Bands[0].Columns["FFHWTimeInput"].Header.Caption = "FFHW\n Time Input";
      e.Layout.Bands[0].Columns["MATTimeInput"].Header.Caption = "MAT\n Time Input";

      e.Layout.Bands[0].Columns["COM1SuggestDeadline"].Header.Caption = "Target\n COM1\n Deadline";
      e.Layout.Bands[0].Columns["ASSYSuggestDeadline"].Header.Caption = "Target\n ASSY + SAN\n Deadline";
      e.Layout.Bands[0].Columns["SUBCONSuggestDeadline"].Header.Caption = "Target\n SUBCON\n Deadline";
      e.Layout.Bands[0].Columns["CARSuggestDeadline"].Header.Caption = "Target\n CAR\n Deadline";
      e.Layout.Bands[0].Columns["ASSHWSuggestDeadline"].Header.Caption = "Target\n ASSHW\n Deadline";
      e.Layout.Bands[0].Columns["MATSuggestDeadline"].Header.Caption = "Target\n MAT\n Deadline";
      e.Layout.Bands[0].Columns["FFHWSuggestDeadline"].Header.Caption = "Target\n FFHW\n Deadline";

      e.Layout.Bands[0].Columns["COM1Alert"].Header.Caption = "COM1\n Alert";
      e.Layout.Bands[0].Columns["ASSYAlert"].Header.Caption = "ASSY + SAN\n Alert";
      e.Layout.Bands[0].Columns["SUBCONAlert"].Header.Caption = "SUBCON\n Alert";
      e.Layout.Bands[0].Columns["CARAlert"].Header.Caption = "CAR\n Alert";
      e.Layout.Bands[0].Columns["ASSHWAlert"].Header.Caption = "ASSHW\n Alert";
      e.Layout.Bands[0].Columns["FFHWAlert"].Header.Caption = "FFHW\n Alert";
      e.Layout.Bands[0].Columns["MATAlert"].Header.Caption = "MAT\n Alert";

      e.Layout.Bands[0].Columns["COM1Locked"].Header.Caption = "COM1\n Locked";
      e.Layout.Bands[0].Columns["ASSYLocked"].Header.Caption = "ASSY + SAN\n Locked";
      e.Layout.Bands[0].Columns["SUBCONLocked"].Header.Caption = "SUBCON\n Locked";
      e.Layout.Bands[0].Columns["CARLocked"].Header.Caption = "CAR\n Locked";
      e.Layout.Bands[0].Columns["ASSHWLocked"].Header.Caption = "ASSHW\n Locked";
      e.Layout.Bands[0].Columns["FFHWLocked"].Header.Caption = "FFHW\n Locked";
      e.Layout.Bands[0].Columns["MATLocked"].Header.Caption = "MAT\n Locked";

      e.Layout.Bands[0].Columns["COM1Confirmed"].Header.Caption = "COM1\n Confirmed";
      e.Layout.Bands[0].Columns["ASSYConfirmed"].Header.Caption = "ASSY + SAN\n Confirmed";
      e.Layout.Bands[0].Columns["SUBCONConfirmed"].Header.Caption = "SUBCON\n Confirmed";
      e.Layout.Bands[0].Columns["CARConfirmed"].Header.Caption = "CAR\n Confirmed";
      e.Layout.Bands[0].Columns["ASSHWConfirmed"].Header.Caption = "ASSHW\n Confirmed";
      e.Layout.Bands[0].Columns["FFHWConfirmed"].Header.Caption = "FFHW\n Confirmed";
      e.Layout.Bands[0].Columns["MATConfirmed"].Header.Caption = "MAT\n Confirmed";

      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;

      // Set Column Style
      e.Layout.Bands[0].Columns["COM1Locked"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["COM1Confirmed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["ASSYLocked"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ASSYConfirmed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["SUBCONLocked"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["SUBCONConfirmed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["CARLocked"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CARConfirmed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["ASSHWLocked"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ASSHWConfirmed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["FFHWLocked"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["FFHWConfirmed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["MATLocked"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MATConfirmed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["MakeTransaction"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["CarcassWorkOrder"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["WorkArea"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["CarcassCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["SaleCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Container"].CellAppearance.BackColor = Color.Yellow;

      e.Layout.Bands[0].Columns["PackingDeadline"].CellAppearance.BackColor = Color.GreenYellow;
      e.Layout.Bands[0].Columns["LoadingDate"].CellAppearance.BackColor = Color.GreenYellow;
      e.Layout.Bands[0].Columns["OriginalEstimateShipdate"].CellAppearance.BackColor = Color.GreenYellow;
      e.Layout.Bands[0].Columns["EstimateShipdate"].CellAppearance.BackColor = Color.GreenYellow;
      e.Layout.Bands[0].Columns["OriginalEstimateShipdate"].CellAppearance.BackColor = Color.GreenYellow;
      e.Layout.Bands[0].Columns["PLNSuggestDeadline"].CellAppearance.BackColor = Color.GreenYellow;
      e.Layout.Bands[0].Columns["SuggestPackingDeadline"].CellAppearance.BackColor = Color.GreenYellow;
      e.Layout.Bands[0].Columns["SuggestPackingDeadlineFromCurDL"].CellAppearance.BackColor = Color.GreenYellow;

      e.Layout.Bands[0].Columns["WorkArea"].Width = 80;
      e.Layout.Bands[0].Columns["WO"].Width = 50;
      e.Layout.Bands[0].Columns["CarcassCode"].Width = 100;
      e.Layout.Bands[0].Columns["ItemCode"].Width = 100;
      e.Layout.Bands[0].Columns["Revision"].Width = 60;
      e.Layout.Bands[0].Columns["PackingDeadline"].Width = 100;
      e.Layout.Bands[0].Columns["Qty"].Width = 50;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      e.Cell.Row.Cells["IsModify"].Value = 1;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
      if (ultData.Rows.Count > 0)
      {
        lblCount.Text = "Count: " + ultData.Rows.FilteredInRowCount;
      }
    }

    private void btnImportDeadline_Click(object sender, EventArgs e)
    {
      this.ImportDeadline();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckData();
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Confirmed Deadline
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      //Search Deadline
      this.SearchData();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void chkShowItem_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultData, grpItemPicture, picItem, chkShowItem.Checked);
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, grpItemPicture, picItem, chkShowItem.Checked);
      if (e.Button == MouseButtons.Right)
      {
        if (ultData.Selected.Rows.Count > 0 || ultData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultData, new System.Drawing.Point(e.X, e.Y));
        }
      }
    }

    private void chkCOM1_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkCOM1)
      {
        int checkAllCOM1 = (chkCOM1.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].IsFilteredOut == false
            && DBConvert.ParseInt(ultData.Rows[i].Cells["COM1BeforeLocked"].Value.ToString()) != 1
            && ultData.Rows[i].Cells["COM1Deadline"].Value.ToString().Trim().Length > 0
            && ultData.Rows[i].Cells["COM1InputDeadline"].Value.ToString().Trim().Length > 0)
          {
            ultData.Rows[i].Cells["COM1Locked"].Value = checkAllCOM1;
          }
        }
      }
    }

    private void chkCAR_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkCAR)
      {
        int checkAllCAR = (chkCAR.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].IsFilteredOut == false
            && DBConvert.ParseInt(ultData.Rows[i].Cells["CARBeforeLocked"].Value.ToString()) != 1
            //&& ultData.Rows[i].Cells["PackingDeadline"].Value.ToString().Trim().Length > 0
            && ultData.Rows[i].Cells["CARInputDeadline"].Value.ToString().Trim().Length > 0)
          {
            ultData.Rows[i].Cells["CARLocked"].Value = checkAllCAR;
          }
        }
      }
    }

    private void chkASSY_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkASSY)
      {
        int checkAllASSY = (chkASSY.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].IsFilteredOut == false
            && DBConvert.ParseInt(ultData.Rows[i].Cells["ASSYBeforeLocked"].Value.ToString()) != 1
            && ultData.Rows[i].Cells["ASSYDeadline"].Value.ToString().Trim().Length > 0
            && ultData.Rows[i].Cells["ASSYInputDeadline"].Value.ToString().Trim().Length > 0)
          {
            ultData.Rows[i].Cells["ASSYLocked"].Value = checkAllASSY;
          }
        }
      }
    }

    private void chkSUBCON_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkSUBCON)
      {
        int checkAllSUBCON = (chkSUBCON.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].IsFilteredOut == false
            && DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONBeforeLocked"].Value.ToString()) != 1
            && ultData.Rows[i].Cells["SUBCONDeadline"].Value.ToString().Trim().Length > 0
            && ultData.Rows[i].Cells["SUBCONInputDeadline"].Value.ToString().Trim().Length > 0)
          {
            ultData.Rows[i].Cells["SUBCONLocked"].Value = checkAllSUBCON;
          }
        }
      }
    }

    private void chkCARConfirm_CheckedChanged(object sender, EventArgs e)
    {
      int checkAllCAR = (chkCARConfirm.Checked ? 1 : 0);
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].IsFilteredOut == false
          && DBConvert.ParseInt(ultData.Rows[i].Cells["CARBeforeConfirmed"].Value.ToString()) != 1
          //&& ultData.Rows[i].Cells["PackingDeadline"].Value.ToString().Trim().Length > 0
          && DBConvert.ParseInt(ultData.Rows[i].Cells["CARBeforeLocked"].Value.ToString()) == 1
          //&& ultData.Rows[i].Cells["CARInputDeadline"].Value.ToString().Trim().Length > 0 
          && this.department != ultData.Rows[i].Cells["CARDepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["CARConfirmed"].Value = checkAllCAR;
        }
      }
    }

    private void chkCOM1Confirm_CheckedChanged(object sender, EventArgs e)
    {
      int checkAllCOM1 = (chkCOM1Confirm.Checked ? 1 : 0);
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].IsFilteredOut == false
          && DBConvert.ParseInt(ultData.Rows[i].Cells["COM1BeforeConfirmed"].Value.ToString()) != 1
          //&& ultData.Rows[i].Cells["COM1Deadline"].Value.ToString().Trim().Length > 0
          && DBConvert.ParseInt(ultData.Rows[i].Cells["COM1BeforeLocked"].Value.ToString()) == 1
          && ultData.Rows[i].Cells["COM1InputDeadline"].Value.ToString().Trim().Length > 0 && this.department != ultData.Rows[i].Cells["COM1DepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["COM1Confirmed"].Value = checkAllCOM1;
        }
      }
    }

    private void chkASSYConfirm_CheckedChanged(object sender, EventArgs e)
    {
      int checkAllASSY = (chkASSYConfirm.Checked ? 1 : 0);
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].IsFilteredOut == false
          && DBConvert.ParseInt(ultData.Rows[i].Cells["ASSYBeforeConfirmed"].Value.ToString()) != 1
          //&& ultData.Rows[i].Cells["ASSYDeadline"].Value.ToString().Trim().Length > 0
          && DBConvert.ParseInt(ultData.Rows[i].Cells["ASSYBeforeLocked"].Value.ToString()) == 1
          && ultData.Rows[i].Cells["ASSYInputDeadline"].Value.ToString().Trim().Length > 0 && this.department != ultData.Rows[i].Cells["ASSYDepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["ASSYConfirmed"].Value = checkAllASSY;
        }
      }
    }

    private void chkSUBCONConfirm_CheckedChanged(object sender, EventArgs e)
    {
      int checkAllSUBCON = (chkSUBCONConfirm.Checked ? 1 : 0);
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].IsFilteredOut == false
          && DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONBeforeConfirmed"].Value.ToString()) != 1
          //&& ultData.Rows[i].Cells["SUBCONDeadline"].Value.ToString().Trim().Length > 0
          && DBConvert.ParseInt(ultData.Rows[i].Cells["SUBCONBeforeLocked"].Value.ToString()) == 1
          && ultData.Rows[i].Cells["SUBCONInputDeadline"].Value.ToString().Trim().Length > 0 && this.department != ultData.Rows[i].Cells["SUBCONDepartmentInput"].Value.ToString())
        {
          ultData.Rows[i].Cells["SUBCONConfirmed"].Value = checkAllSUBCON;
        }
      }
    }

    private void chkTran_CheckedChanged(object sender, EventArgs e)
    {
      if (this.checkTran)
      {
        int checkAllTran = (chkTran.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].IsFilteredOut == false)
          {
            ultData.Rows[i].Cells["MakeTransaction"].Value = checkAllTran;
          }
        }
      }
    }

    private void btnMakeTrans_Click(object sender, EventArgs e)
    {
      System.Data.DataTable dtSource = (System.Data.DataTable)ultData.DataSource;
      System.Data.DataTable dtInput = this.CreateDatatableTransaction();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        DataRow rowadd = dtInput.NewRow();
        if (ultData.Rows[i].IsFilteredOut == false && DBConvert.ParseInt(ultData.Rows[i].Cells["BeforeMakeTransaction"].Value.ToString()) == 0
          && DBConvert.ParseInt(ultData.Rows[i].Cells["MakeTransaction"].Value.ToString()) == 1)
        {
          //StringHangtag
          if (ultData.Rows[i].Cells["StringHangtag"].Value.ToString().Trim().Length > 0)
          {
            rowadd["HangTag"] = ultData.Rows[i].Cells["StringHangtag"].Value.ToString();
          }
          dtInput.Rows.Add(rowadd);
        }
      }
      viewPLN_29_006 view = new viewPLN_29_006();
      view.dtTable = dtInput;
      Shared.Utility.WindowUtinity.ShowView(view, "New Transaction", false, Shared.Utility.ViewState.ModalWindow);
      //if (view.flagSearch == true)
      //{
      //  this.SearchData();
      //}
    }

    private System.Data.DataTable CreateDatatableTransaction()
    {
      System.Data.DataTable dt = new System.Data.DataTable();
      dt.Columns.Add("HangTag", typeof(System.String));
      return dt;
    }

    private void ultData_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
    {
      lblCount.Text = "Count: " + ultData.Rows.FilteredInRowCount;
    }

    private void ultData_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
    {
      string typeName = e.Type.Name;

      if ((string.Compare("ColumnHeader", typeName, true) == 0) && (ultData.Selected != null) && (ultData.Selected.Columns.Count > 0))
      {
        string colName = ultData.Selected.Columns[0].Caption;
        if (string.Compare(colName, "EstimateShipdate", true) == 0)
        {
          Popup description = new Popup(lblDescription);
          description.AutoSize = true;
          lblDescription.Text = "Min Estimate Shipdate";
          description.Show(new System.Drawing.Point(MousePosition.X, MousePosition.Y - 30));
        }
      }
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      btnRefresh.Enabled = false;
      DataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanHardWareStatus_Capture", 600, null, null);
      DataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanMaterialStatus_Capture", 600, null, null);
      btnRefresh.Enabled = true;
      this.SearchData();
    }

    private void btnAddRemove_Click(object sender, EventArgs e)
    {
      System.Data.DataTable dtSource = (System.Data.DataTable)ultData.DataSource;
      System.Data.DataTable dtInput = this.CreateDatatableTransaction();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        DataRow rowadd = dtInput.NewRow();
        if (ultData.Rows[i].IsFilteredOut == false && DBConvert.ParseInt(ultData.Rows[i].Cells["MakeTransaction"].Value.ToString()) == 1)
        {
          //StringHangtag
          if (ultData.Rows[i].Cells["StringHangtag"].Value.ToString().Trim().Length > 0)
          {
            rowadd["HangTag"] = ultData.Rows[i].Cells["StringHangtag"].Value.ToString();
          }
          dtInput.Rows.Add(rowadd);
        }
      }
      viewPLN_29_008 view = new viewPLN_29_008();
      view.dtTable = dtInput;
      Shared.Utility.WindowUtinity.ShowView(view, "UPDATE CARCASS WORK", false, Shared.Utility.ViewState.ModalWindow);
      //if (view.flagSearch == true)
      //{
      //  this.SearchData();
      //}
    }

    private void btnRefreshDeadline_Click(object sender, EventArgs e)
    {
      btnRefreshDeadline.Enabled = false;
      DataBaseAccess.ExecuteStoreProcedure("spPLNProductionPlanRefreshDeadline", 1000, null, null);
      btnRefreshDeadline.Enabled = true;
      this.SearchData();
    }
    #endregion Event
  }
}
