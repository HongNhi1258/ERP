/*
  Author  : Duong Minh
  Email   : minh_it@daico-furniture.com
  Date    : 09/02/2012
*/

using DaiCo.Application;
using DaiCo.ERPProject.Warehouse.DataSetSource;
using DaiCo.ERPProject.Warehouse.Material.DataSetSource;
using DaiCo.ERPProject.Warehouse.Material.Reports;
using DaiCo.ERPProject.Warehouse.Reports;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWHD_02_005 : MainUserControl
  {
    #region Field
    private int whPid = 0;
    public string issueNo = string.Empty;
    private string dep = string.Empty;

    // Flag Update
    private bool canUpdate = false;

    // IsProduction
    private int isProduction = int.MinValue;
    private bool isConfirm = false;
    private int docTypePid = 79;
    private int actionCode = 1; //From Request Online
    private long issuingPid = long.MinValue;
    private DataTable dtProductSerial = new DataTable();
    #endregion Field

    #region Init Data
    public viewWHD_02_005()
    {
      InitializeComponent();
    }

    private void viewWHD_02_005_Load(object sender, EventArgs e)
    {
      this.LoadFlagConfirm();
      this.LoadUltraRequestOnline();
      // Department
      this.LoadUcbWorkshop();
      this.LoadUcbProcess();
      Utility.LoadUltraCBMaterialWHListByUser(ucbWarehouse);
      if (issueNo == string.Empty)
      {
        this.grpMainData.Enabled = false;
      }
      else
      {
        this.grpMainData.Enabled = true;
        this.LoadData();
      }
    }

    private void LoadFlagConfirm()
    {
      string commandText = string.Empty;
      if (issueNo.Length > 0)
      {
        commandText += " SELECT COUNT(*) ";
        commandText += " FROM TblWHDMaterialOutStore ";
        commandText += " WHERE ID_PhieuXuat = '" + issueNo + "' AND Posting = 1 ";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 1)
          {
            isConfirm = true;
          }
          else
          {
            isConfirm = false;
          }
        }
      }
      else
      {
        isConfirm = false;
      }
    }

    private void LoadData()
    {
      Utility.LoadUltraEmployeeByDeparment(drpReceiver, this.dep, true);

      DBParameter[] param = new DBParameter[] { new DBParameter("@IssueNo", DbType.AnsiString, 50, this.issueNo) };
      DataSet dsData = DataBaseAccess.SearchStoreProcedure("spWHDIssuingNoteInfomationMRN_Materials", param);
      DataTable dtBaseData = dsData.Tables[0];
      if (dtBaseData.Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0007");
        this.CloseTab();
        return;
      }

      // 1. Load Base Data
      DataRow row = dtBaseData.Rows[0];
      this.issuingPid = DBConvert.ParseLong(row["Pid"]);
      txtIssueNo.Text = row["IssueNo"].ToString();
      txtTitle.Text = row["Title"].ToString();
      txtCreateBy.Text = row["CreateBy"].ToString();
      ucbWorkshop.Value = row["WorkshopPid"];
      ucbProcess.Value = row["ProcessPid"];
      txtIssueDate.Text = row["IssueDate"].ToString();
      drpReceiver.Value = row["Recever"];

      this.ultRequestOnline.Value = DBConvert.ParseLong(row["MRN"].ToString());
      this.whPid = DBConvert.ParseInt(row["WHPid"]);
      ucbWarehouse.Value = this.whPid;
      int confirm = DBConvert.ParseInt(row["Confirm"].ToString());
      this.canUpdate = ((confirm != 1) && (btnSave.Visible) && (btnSave.Enabled));
      chkConfirm.Checked = (confirm == 1);
      this.btnPrint.Enabled = false;
      this.btnPrintDetail.Enabled = false;
      if (confirm == 1)
      {
        this.MainData();
        chkConfirm.Enabled = false;
        btnSave.Enabled = false;
        btnPrint.Enabled = true;
        btnPrintDetail.Enabled = true;
        for (int i = 0; i < ultIssueDetail.Rows.Count; i++)
        {
          UltraGridRow rowDetail = ultIssueDetail.Rows[i];
          rowDetail.Activation = Activation.ActivateOnly;
        }
      }
      // 2. Load Grid
      ultIssueDetail.DataSource = dsData.Tables[1];
      this.dtProductSerial = dsData.Tables[2];
      this.LoadProductSerialSource(dsData.Tables[2]);
      Utility.LoadUltraCBLocationListByWHPid(ucbLocation, this.whPid);
      this.LoadTransationData();
      ultRequestOnline.ReadOnly = true;
      btnLoad.Enabled = false;
    }

    private void LoadUltraRequestOnline()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@EmpID", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[1] = new DBParameter("@IsConfirmIssuing", DbType.Boolean, this.isConfirm);
      DataTable dtRequisitionNote = DataBaseAccess.SearchStoreProcedureDataTable("spWHDRequisitionNoteListByUser", inputParam);
      Utility.LoadUltraCombo(ultRequestOnline, dtRequisitionNote, "Pid", "Code", false, new string[] { "Pid", "WarehousePid" });
    }

    private void LoadUcbWorkshop()
    {
      string commandText = "SELECT WorkshopPid, WorkshopName FROM VBOMWorkshop";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbWorkshop, dtSource, "WorkshopPid", "WorkshopName", false, "WorkshopPid");
    }

    private void LoadUcbProcess()
    {
      string commandText = "SELECT ProcessPid, ProcessName FROM VBOMMainProcess";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbProcess, dtSource, "ProcessPid", "ProcessName", false, "ProcessPid");
    }

    private void LoadProductSerialSource(DataTable dtSource)
    {
      Utility.LoadUltraCombo(ucbProductSerial, dtSource, "ProductSerialPid", "SerialNo", true, new string[] { "MaterialCode", "ProductSerialPid", "LocationPid" });
      ucbProductSerial.DisplayLayout.Bands[0].Columns["SerialNo"].Header.Caption = "Mã lô";
      ucbProductSerial.DisplayLayout.Bands[0].Columns["LocationName"].Header.Caption = "Vị trí";
      ucbProductSerial.DisplayLayout.Bands[0].Columns["StockQty"].Header.Caption = "SL tồn";
    }

    private UltraCombo LoadProductSerialSource(DataTable dtSource, UltraCombo ucb)
    {
      Utility.LoadUltraCombo(ucb, dtSource, "ProductSerialPid", "SerialNo", true, new string[] { "MaterialCode", "ProductSerialPid", "LocationPid" });
      ucb.DisplayLayout.Bands[0].Columns["SerialNo"].Header.Caption = "Mã lô";
      ucb.DisplayLayout.Bands[0].Columns["LocationName"].Header.Caption = "Vị trí";
      ucb.DisplayLayout.Bands[0].Columns["StockQty"].Header.Caption = "SL tồn";
      return ucb;
    }
    #endregion Init Data  

    #region Process    

    private void MainData()
    {
      txtTitle.ReadOnly = true;
      ucbWorkshop.Enabled = false;
      drpReceiver.Enabled = false;
    }

    /// <summary>
    /// Load transaction
    /// </summary>
    private void LoadTransationData()
    {
      grdPostTran.SetDataSource(this.docTypePid, this.issuingPid);
    }

    private bool CheckValidLoadRequest(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();
      // Master Information
      if (ultRequestOnline.Value != null && ultRequestOnline.SelectedRow != null)
      {
        commandText = "SELECT COUNT(*) FROM TblGNRMaterialRequisitionNote WHERE Pid = " + DBConvert.ParseLong(this.ultRequestOnline.Value.ToString());
        dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            message = "Request Online No";
            WindowUtinity.ShowMessageError("ERR0001", message);
            return false;
          }
        }
        else
        {
          message = "Request Online No";
          WindowUtinity.ShowMessageError("ERR0001", message);
          return false;
        }
      }
      else
      {
        message = "Request Online No";
        WindowUtinity.ShowMessageError("ERR0001", message);
        return false;
      }

      commandText = string.Empty;
      commandText += " SELECT COUNT(*)";
      commandText += " FROM TblWHDMaterialOutStore";
      commandText += " WHERE MRN = " + DBConvert.ParseLong(this.ultRequestOnline.Value.ToString());
      dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheck != null && dtCheck.Rows.Count > 0)
      {
        if (DBConvert.ParseLong(dtCheck.Rows[0][0].ToString()) > 0)
        {
          message = "Request Online No";
          WindowUtinity.ShowMessageError("ERR0028", message);
          return false;
        }
      }
      else
      {
        message = "Request Online No";
        WindowUtinity.ShowMessageError("ERR0028", message);
        return false;
      }

      return true;
    }

    private string GetIssueNo()
    {
      //string commandText = string.Format("SELECT dbo.FWHDGetNewIssueNo({0}, {1}, {2})", this.whPid, ConstantClass.ISSUE_TO_PRODUCTION, 0);
      //object result = DataBaseAccess.ExecuteScalarCommandText(commandText);
      //return (result != null) ? result.ToString() : string.Empty;

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@NewDocCode", DbType.String, 32, string.Empty) };
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@DocTypePid", DbType.Int32, this.docTypePid);
      inputParam[1] = new DBParameter("@DocDate", DbType.Date, DateTime.Now.Date);

      DataBaseAccess.SearchStoreProcedure("spACCGetNewDocCode", inputParam, outputParam);
      if (outputParam[0].Value.ToString().Length > 0)
      {
        return outputParam[0].Value.ToString();       
      }
      return string.Empty;
    }

    private bool CheckInvalid()
    {
      // Create new issuing note
      if (this.issueNo.Length == 0)
      {
        // Check WH Summary of preMonth
        if (!Utility.CheckWHPreMonthSummary(this.whPid))
        {
          return false;
        }
        // Check Request No has been issued
        string commandText = string.Empty;
        string message = string.Empty;
        commandText += " SELECT COUNT(*)";
        commandText += " FROM TblWHDMaterialOutStore";
        commandText += " WHERE MRN = " + DBConvert.ParseLong(this.ultRequestOnline.Value.ToString());
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          if (DBConvert.ParseLong(dtCheck.Rows[0][0].ToString()) > 0)
          {
            message = "Request Online No";
            WindowUtinity.ShowMessageError("ERR0028", message);
            return false;
          }
        }
        else
        {
          message = "Request Online No";
          WindowUtinity.ShowMessageError("ERR0028", message);
          return false;
        }
      }

      // Truong Add
      string commandTextCheck = string.Format(@"SELECT Pid FROM TblGNRMaterialRequisitionNote WHERE Pid = {0}", DBConvert.ParseLong(ultRequestOnline.Value.ToString()));
      DataTable dtCheckRequestOnline = DataBaseAccess.SearchCommandTextDataTable(commandTextCheck);
      if (dtCheckRequestOnline == null || dtCheckRequestOnline.Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0050", "Because " + ultRequestOnline.Text + " was deleted!");
        return false;
      }
      // End
      if (this.txtCreateBy.Text.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Create by" });
        txtCreateBy.Focus();
        return false;
      }
      string selectedValue = string.Empty;
      //selectedValue = (ucbWorkshop.Value != null) ? ucbWorkshop.Value.ToString() : string.Empty;
      //if (ucbWorkshop.SelectedRow == null || selectedValue.Length == 0)
      //{
      //  WindowUtinity.ShowMessageError("MSG0005", new string[] { "Department" });
      //  ucbWorkshop.Focus();
      //  return false;
      //}
      selectedValue = (drpReceiver.Value != null) ? drpReceiver.Value.ToString() : string.Empty;
      if (drpReceiver.SelectedRow == null || selectedValue.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Receiver" });
        drpReceiver.Focus();
        return false;
      }

      if (this.ultIssueDetail.Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Data Issuing Detail" });
        drpReceiver.Focus();
        return false;
      }

      // Check Stock Qty
      DataTable dtDataCheck = new DataTable();
      dtDataCheck.Columns.Add("IssuingDetailPid", typeof(System.Int64));
      dtDataCheck.Columns.Add("MaterialCode", typeof(System.String));
      dtDataCheck.Columns.Add("ProductSerialPid", typeof(System.Int64));
      dtDataCheck.Columns.Add("LocationPid", typeof(System.Int32));      
      dtDataCheck.Columns.Add("Issue", typeof(System.Double));

      DataTable dtDetail = (DataTable)this.ultIssueDetail.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          double totalRequire = DBConvert.ParseDouble(row["Qty"]);

          DataRow[] foundRow = dtDetail.Select("MaterialCode ='" + row["MaterialCode"].ToString() + "'");
          double totalIssue = 0;
          for (int m = 0; m < foundRow.Length; m++)
          {
            totalIssue += DBConvert.ParseDouble(foundRow[m]["Issue"].ToString());
          }

          if (totalIssue > totalRequire)
          {
            WindowUtinity.ShowMessageError("ERRO120", new string[] { row["MaterialCode"].ToString() });
            return false;
          }
          DataRow rowCheck = dtDataCheck.NewRow();
          if (DBConvert.ParseLong(row["Pid"]) > 0)
          {
            rowCheck["IssuingDetailPid"] = row["Pid"];
          }
          rowCheck["MaterialCode"] = row["MaterialCode"];
          rowCheck["ProductSerialPid"] = row["ProductSerialPid"];
          rowCheck["LocationPid"] = row["LocationPid"];
          rowCheck["Issue"] = row["Issue"];
          dtDataCheck.Rows.Add(rowCheck);
        }
      }
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@WarehousePid", SqlDbType.Int, this.whPid);
      inputParam[1] = new SqlDBParameter("@IsseMaterialList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtDataCheck));
      DataSet dsDataCheck = SqlDataBaseAccess.SearchStoreProcedure("spWHDMaterialOutStore_CheckValid", inputParam);
      if (dsDataCheck != null && dsDataCheck.Tables.Count > 0)
      {
         // Check stock qty
        if (dsDataCheck.Tables[0].Rows.Count > 0)
        {
          DataRow row = dsDataCheck.Tables[0].Rows[0];
          WindowUtinity.ShowMessageErrorFromText(string.Format("Số lượng xuất sản phẩm {0} không được vượt quá {1}", row["MaterialCode"], row["StockQty"]));
          return false;
        }
      }
      return true;
    }

    private void SaveData()
    {
      DataTable dtDetail = (DataTable)ultIssueDetail.DataSource;

      //Tien Adjust
      SqlDBParameter[] input = new SqlDBParameter[14];
      if (this.issuingPid > 0)
      {
        input[0] = new SqlDBParameter("@IssuingPid", SqlDbType.BigInt, this.issuingPid);
      }
      input[1] = new SqlDBParameter("@Tittle", SqlDbType.NVarChar, 4000, txtTitle.Text.Trim());
      input[2] = new SqlDBParameter("@WriteOff", SqlDbType.Int, 0);
      input[3] = new SqlDBParameter("@AdjsutBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      if(ucbWorkshop.SelectedRow != null)
      {
        input[4] = new SqlDBParameter("@WorkshopPid", SqlDbType.Int, ucbWorkshop.Value);
      }      
      input[5] = new SqlDBParameter("@Receiver", SqlDbType.Int, (drpReceiver.SelectedRow != null) ? DBConvert.ParseInt(drpReceiver.Value.ToString()) : int.MinValue);
      input[6] = new SqlDBParameter("@IsProduction", SqlDbType.Int, this.isProduction);
      input[7] = new SqlDBParameter("@Warehouse", SqlDbType.Int, this.whPid);
      input[8] = new SqlDBParameter("@Type", SqlDbType.Int, ConstantClass.ISSUE_TO_PRODUCTION);
      input[9] = new SqlDBParameter("@MRN", SqlDbType.Int, DBConvert.ParseLong(ultRequestOnline.Value.ToString()));
      input[10] = new SqlDBParameter("@Posting", SqlDbType.Int, (chkConfirm.Checked) ? 1 : 0);

      // Get Isse Material
      DataTable dtIssueMaterial = new DataTable();
      dtIssueMaterial.Columns.Add("IssuingDetailPid", typeof(System.Int64));
      dtIssueMaterial.Columns.Add("MaterialCode", typeof(System.String));
      dtIssueMaterial.Columns.Add("ProductSerialPid", typeof(System.Int64));
      dtIssueMaterial.Columns.Add("LocationPid", typeof(System.Int32));
      dtIssueMaterial.Columns.Add("Issue", typeof(System.Double));
            
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {          
          DataRow rowCheck = dtIssueMaterial.NewRow();
          if (DBConvert.ParseLong(row["Pid"]) > 0)
          {
            rowCheck["IssuingDetailPid"] = row["Pid"];
          }
          rowCheck["MaterialCode"] = row["MaterialCode"];
          rowCheck["ProductSerialPid"] = row["ProductSerialPid"];
          rowCheck["LocationPid"] = row["LocationPid"];
          rowCheck["Issue"] = row["Issue"];
          dtIssueMaterial.Rows.Add(rowCheck);
        }
      }
      input[11] = new SqlDBParameter("@IssueMaterialList", SqlDbType.NVarChar, DBConvert.ParseXMLString(dtIssueMaterial));      
      input[12] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, this.docTypePid);
      if(ucbProcess.SelectedRow != null)
      {
        input[13] = new SqlDBParameter("@ProcessPid", SqlDbType.Int, ucbProcess.Value);
      } 
      
      SqlDBParameter[] output = new SqlDBParameter[2];
      output[0] = new SqlDBParameter("@IssueNo", SqlDbType.VarChar, 32, string.Empty);
      output[1] = new SqlDBParameter("@Result", SqlDbType.BigInt, 0);
        
      SqlDataBaseAccess.ExecuteStoreProcedure("spWHDIssueMaterialsForRequestOnline", 300, input, output);
      long result = DBConvert.ParseLong(output[1].Value);
      if (result <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      else
      {
        this.issuingPid = result;
        this.issueNo = output[0].Value.ToString();
        if (chkConfirm.Checked)
        {
          bool isPosted = true;
          Utility.ACCPostTransaction(this.docTypePid, this.issuingPid, isPosted, SharedObject.UserInfo.UserPid);
        }
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }

    private UltraCombo LoadUcbProductSerial(UltraCombo ucbSerial, string materialCode)
    {
      if (ucbSerial == null)
      {
        ucbSerial = new UltraCombo();
      }
      ucbSerial.Width = 500;
      DataTable dtSource = this.dtProductSerial.Clone();
      DataRow[] rowSource = this.dtProductSerial.Select(string.Format("MaterialCode = '{0}'", materialCode));
      foreach (DataRow row in rowSource)
      {
        DataRow newRow = dtSource.NewRow();
        newRow["MaterialCode"] = row["MaterialCode"];
        newRow["ProductSerialPid"] = row["ProductSerialPid"];
        newRow["LocationPid"] = row["LocationPid"];
        newRow["SerialNo"] = row["SerialNo"];
        newRow["LocationName"] = row["LocationName"];
        newRow["StockQty"] = row["StockQty"];
        dtSource.Rows.Add(newRow);
      }
      this.LoadProductSerialSource(dtSource, ucbSerial);
      return ucbSerial;
    }
    #endregion Process

    #region Event
    private void btnLoad_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValidLoadRequest(out message);
      if (!success)
      {
        return;
      }
      this.whPid = DBConvert.ParseInt(ultRequestOnline.SelectedRow.Cells["WarehousePid"].Value);
      ucbWarehouse.Value = this.whPid;
      Utility.LoadUltraCBLocationListByWHPid(ucbLocation, this.whPid);
      string commandText = string.Empty;
      DataTable dtSource = new DataTable();

      this.grpMainData.Enabled = true;
      txtIssueNo.Text = this.GetIssueNo();
      txtIssueDate.Text = DBConvert.ParseString(DateTime.Today, "dd/MM/yyyy");

      commandText += " SELECT CAST(ID_NhanVien AS VARCHAR) + ' - ' + HoNV + ' ' + TenNV Name ";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE ID_NhanVien = " + SharedObject.UserInfo.UserPid;

      dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        this.txtCreateBy.Text = dtSource.Rows[0][0].ToString();
      }
      else
      {
        this.txtCreateBy.Text = string.Empty;
      }

      // Department
      Utility.LoadUltraDepartment(ucbWorkshop, true);

      commandText = string.Empty;
      commandText += "  SELECT MRN.DepartmentRequest, DEP.WorkshopPid, MRN.IsProduction, MRN.CreateBy";
      commandText += "  FROM TblGNRMaterialRequisitionNote MRN";
      commandText += "  	INNER JOIN VHRDDepartment DEP ON MRN.DepartmentRequest = DEP.Department";
      commandText += "  WHERE MRN.Pid =" + DBConvert.ParseLong(ultRequestOnline.Value);
      dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        ucbWorkshop.Value = dtSource.Rows[0]["WorkshopPid"];
        this.dep = dtSource.Rows[0]["DepartmentRequest"].ToString();
        this.isProduction = DBConvert.ParseInt(dtSource.Rows[0]["IsProduction"].ToString());
      }
      else
      {
        this.dep = string.Empty;
        this.ucbWorkshop.Value = string.Empty;
        this.isProduction = int.MinValue;
      }

      Utility.LoadUltraEmployeeByDeparment(drpReceiver, this.dep, true);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        this.drpReceiver.Value = DBConvert.ParseInt(dtSource.Rows[0]["CreateBy"].ToString());
      }

      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultRequestOnline.Value.ToString()));
      inputParam[1] = new DBParameter("@WarehousePid", DbType.Int32, this.whPid);

      DataSet dsData = DataBaseAccess.SearchStoreProcedure("spPLNGetMaterialRequisitionOnlineIssuingNote_Select", inputParam);

      ultIssueDetail.DataSource = dsData.Tables[0];
      this.dtProductSerial = dsData.Tables[1];
      this.LoadProductSerialSource(dsData.Tables[1]);      
      ultRequestOnline.ReadOnly = true;
      btnLoad.Enabled = false;
    }

    private void ultIssueDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Stock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Issue"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["LocationPid"].ValueList = ucbLocation;
      e.Layout.Bands[0].Columns["ProductSerialPid"].ValueList = ucbProductSerial;

      // Read Only
      e.Layout.Bands[0].Columns["Pid"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["No"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialNameVn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LocationPid"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Stock"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UnitCost"].CellActivation = Activation.ActivateOnly;
      if (isConfirm == true)
      {
        e.Layout.Bands[0].Columns["Issue"].CellActivation = Activation.ActivateOnly;
      }

      // Set Caption
      e.Layout.Bands[0].Columns["No"].Header.Caption = "STT";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã SP";
      e.Layout.Bands[0].Columns["MaterialNameVn"].Header.Caption = "Tên SP";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["ProductSerialPid"].Header.Caption = "Mã lô";
      e.Layout.Bands[0].Columns["LocationPid"].Header.Caption = "Vị trí";
      e.Layout.Bands[0].Columns["Issue"].Header.Caption = "SL xuất";
      e.Layout.Bands[0].Columns["Stock"].Header.Caption = "SL tồn";
      e.Layout.Bands[0].Columns["UnitCost"].Header.Caption = "Giá vốn";
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        this.SaveData();
        this.LoadData();
      }
    }

    private void ultIssueDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      bool success = true;
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spWHDIssueToProductionMaterialsDetailMRN_Delete_PMISDB", inputParam, outputParam);
        long outputValue = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
        if (outputValue == 0)
        {
          success = false;
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0004");
      }
      this.LoadData();
    }

    private void ultIssueDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void ultIssueDetail_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
      {
        return;
      }
      int rowIndex = (e.KeyCode == Keys.Down) ? ultIssueDetail.ActiveCell.Row.Index + 1 : ultIssueDetail.ActiveCell.Row.Index - 1;
      int cellIndex = ultIssueDetail.ActiveCell.Column.Index;
      try
      {
        ultIssueDetail.Rows[rowIndex].Cells[cellIndex].Activate();
        ultIssueDetail.PerformAction(UltraGridAction.EnterEditMode, false, false);
      }
      catch
      {
      }
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      if (this.issueNo.Length == 0)
      {
        return;
      }
      DBParameter[] input = new DBParameter[] { new DBParameter("@IssueNo", DbType.AnsiString, 50, txtIssueNo.Text.Trim()) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDIssuingNotePrintSummary_Materials", input);
      dsMaterialsIssuingSummary dsIssue = new dsMaterialsIssuingSummary();
      dsIssue.Tables["dtIssuingInfo"].Merge(dsSource.Tables[0]);
      dsIssue.Tables["dtIssuingDetail"].Merge(dsSource.Tables[1]);
      dsIssue.Tables["dtIssuingDetailSubReport"].Merge(dsSource.Tables[2]);

      double sumqty = 0;
      foreach (DataRow row in dsIssue.Tables["dtIssuingDetail"].Rows)
      {
        if (DBConvert.ParseDouble(row["Qty"].ToString()) != double.MinValue)
        {
          sumqty = sumqty + DBConvert.ParseDouble(row["Qty"].ToString());
        }
      }
      DaiCo.Shared.View_Report report = null;
      cptMaterialsIssueToProductionSummary cpt = new cptMaterialsIssueToProductionSummary();
      cpt.SetDataSource(dsIssue);
      cpt.SetParameterValue("Sumqty", sumqty);
      cpt.SetParameterValue("Code", ultRequestOnline.Text);
      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(Shared.Utility.ViewState.ModalWindow);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultIssueDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      if (string.Compare(columnName, "Issue", true) == 0)
      {
        string text = e.Cell.Text.Trim();
        if (text.Length == 0)
        {
          return;
        }
        double issue = DBConvert.ParseDouble(text);
        if (issue < 0)
        {
          WindowUtinity.ShowMessageError("ERR0009");
          e.Cancel = true;
          return;
        }
        double balance = DBConvert.ParseDouble(e.Cell.Row.Cells["Stock"].Value.ToString());
        if (issue > balance)
        {
          WindowUtinity.ShowMessageError("ERR0010", new string[] { "Issue", "Balance" });
          e.Cancel = true;
          return;
        }
      }
    }

    private void btnPrintDetail_Click(object sender, EventArgs e)
    {
      DBParameter[] input = new DBParameter[] { new DBParameter("@IssueNo", DbType.AnsiString, 50, txtIssueNo.Text.Trim()) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHDIssuingNotePrint_Materials_New", input);
      dsMaterialsIssuing dsIssue = new dsMaterialsIssuing();
      dsIssue.Tables["dtIssuingInfo"].Merge(dsSource.Tables[0]);
      dsIssue.Tables["dtIssuingDetail"].Merge(dsSource.Tables[1]);
      dsIssue.Tables["dtCompanyInfo"].Merge(dsSource.Tables[2]);

      //ReportClass cpt = null;
      DaiCo.Shared.View_Report report = null;

      cptMaterialsIssueToProduction cpt = new cptMaterialsIssueToProduction();
      var dateTime = DateTime.Now;
      string date = $"Ngày   {dateTime.ToString("dd")}   tháng   {dateTime.ToString("MM")}   năm   {dateTime.Year}";
      cpt.SetDataSource(dsIssue);
      cpt.SetParameterValue("Code", ultRequestOnline.Text);
      cpt.SetParameterValue("Date", date);
      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(Shared.Utility.ViewState.ModalWindow);
    }

    private void ultIssueDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();      
      switch (colName)
      {
        case "ProductSerialPid":
          UltraGridRow row = e.Cell.Row;
          UltraCombo ucb = (UltraCombo)e.Cell.ValueList;
          if(ucb != null && ucb.SelectedRow != null)
          {
            row.Cells["LocationPid"].Value = ucb.SelectedRow.Cells["LocationPid"].Value;
            row.Cells["Stock"].Value = ucb.SelectedRow.Cells["StockQty"].Value;
          }  
          else
          {
            row.Cells["LocationPid"].Value = DBNull.Value;
            row.Cells["Stock"].Value = DBNull.Value;
          }  
          break;
        default:
          break;
      }
    }
    private void ultIssueDetail_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "ProductSerialPid":
          string materialCode = row.Cells["MaterialCode"].Value.ToString();          
          UltraCombo ucbSerial = (UltraCombo)row.Cells["ProductSerialPid"].ValueList;
          row.Cells["ProductSerialPid"].ValueList = this.LoadUcbProductSerial(ucbSerial, materialCode);
          break;
        default:
          break;
      }
    }

    
    #endregion Event    
  }
}
