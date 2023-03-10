/*
  Author      : 
  Date        : 06/02/2011
  Description : Insert , Update Supplement No 
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_099 : MainUserControl
  {
    #region Field    
    private bool nonConfirm = true;
    public string supplementNo = string.Empty;
    public long suppPid = long.MinValue;
    private System.Data.DataTable dtSourceMaterials = new System.Data.DataTable();
    private System.Data.DataTable dtReason = new System.Data.DataTable();
    private System.Data.DataTable dtWo = new System.Data.DataTable();
    private bool canUpdate = false;
    private int status = int.MinValue;
    private bool isResetItemCode = false;
    private string pathTemplate = string.Empty;
    private string pathExport = string.Empty;
    #endregion Field

    #region Init Data
    /// <summary>
    /// frmBOMPackageInfo
    /// </summary>
    public viewPLN_07_099()
    {
      InitializeComponent();
    }

    /// <summary>
    /// frmBOMPackageInfo_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_07_099_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + SharedObject.UserInfo.UserName + " | " + SharedObject.UserInfo.LoginDate;
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";

      //Materials
      this.MakeMaterialsData();

      //Reason
      this.MakeReasonData();

      //WO
      this.MakeWOData();

      // MaterialsCode
      this.LoadDropdownMaterial(this.udrpMaterialCode);

      // Reason
      this.LoadDropdownReason(this.udrpReason);

      // WO
      this.LoadDropdownWo(this.udrpWo);

      //Department
      this.MakeDepartment();

      this.LoadData();
    }

    /// <summary>
    /// MakeWO
    /// </summary>
    private void MakeWOData()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Confirm = 1 AND Status != 1 ORDER BY Pid DESC";
      this.dtWo = DataBaseAccess.SearchCommandTextDataTable(commandText);
    }

    /// <summary>
    /// MakeMaterialsData
    /// </summary>
    private void MakeMaterialsData()
    {
      string commandText = "SELECT MaterialCode, MaterialNameEn, Unit, ControlType FROM VBOMMaterials WHERE IsControl = 1 ORDER BY MaterialCode";
      this.dtSourceMaterials = DataBaseAccess.SearchCommandTextDataTable(commandText);
    }

    /// <summary>
    /// Reason Data
    /// </summary>
    private void MakeReasonData()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 1007 AND DeleteFlag = 0 Order By Sort";
      this.dtReason = DataBaseAccess.SearchCommandTextDataTable(commandText);
    }

    private void MakeDepartment()
    {
      string cm = "SELECT Code, Code +' - '+ Name AS Name FROM VHRDDepartmentInfo WHERE DelFlag = 0";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultcbDept, dt, "Code", "Name", false, "Code");
    }

    /// <summary>
    /// LoadDropdownMaterial
    /// </summary>
    /// <param name="udrpMaterials"></param>
    private void LoadDropdownMaterial(UltraDropDown udrpMaterials)
    {
      udrpMaterials.DataSource = this.dtSourceMaterials;
      udrpMaterials.ValueMember = "MaterialCode";
      udrpMaterials.DisplayMember = "MaterialCode";
      udrpMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpMaterials.DisplayLayout.Bands[0].Columns["Unit"].Width = 100;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialNameEN"].Width = 200;
      udrpMaterials.DisplayLayout.Bands[0].Columns["ControlType"].Hidden = true;
    }

    /// <summary>
    /// LoadDropdownReason
    /// </summary>
    /// <param name="udrpMaterials"></param>
    private void LoadDropdownReason(UltraDropDown udrpReason)
    {
      udrpReason.DataSource = this.dtReason;
      udrpReason.ValueMember = "Code";
      udrpReason.DisplayMember = "Value";
      udrpReason.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpReason.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }




    /// <summary>
    /// LoadDropdownWo
    /// </summary>
    /// <param name="udrpMaterials"></param>
    private void LoadDropdownWo(UltraDropDown udrpWo)
    {
      udrpWo.DataSource = this.dtWo;
      udrpWo.ValueMember = "Pid";
      udrpWo.DisplayMember = "Pid";
      udrpWo.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private UltraDropDown LoadUltraDDItem(long wo, UltraDropDown ultraDDItem)
    {
      if (ultraDDItem == null)
      {
        ultraDDItem = new UltraDropDown();
        this.Controls.Add(ultraDDItem);
      }
      string commandText = string.Format("SELECT DISTINCT ItemCode, Revision FROM TblPLNWorkOrderConfirmedDetails WHERE WorkOrderPid = {0}", wo);
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDItem.DataSource = dtSource;
      ultraDDItem.ValueMember = "ItemCode";
      ultraDDItem.DisplayMember = "ItemCode";
      ultraDDItem.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDItem.Visible = false;
      return ultraDDItem;
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SupplementNo", DbType.AnsiString, 16, this.supplementNo) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNSupplementInfomationBySupplementNo", inputParam);
      System.Data.DataTable dtSupInfo = dsSource.Tables[0];

      if (dtSupInfo.Rows.Count > 0)
      {
        DataRow row = dtSupInfo.Rows[0];
        txtSuppNo.Text = this.supplementNo;
        txtCreateBy.Text = row["CreateBy"].ToString();
        txtCreateDate.Text = row["CreateDate"].ToString();
        txtDescription.Text = row["Description"].ToString();
        ultcbDept.Value = row["Department"].ToString();
        this.nonConfirm = (DBConvert.ParseInt(row["Status"].ToString()) == 0);
        status = DBConvert.ParseInt(row["Status"].ToString());

        if (DBConvert.ParseInt(row["Status"].ToString()) != 0)
        {
          this.chkLock.Checked = true;
        }
      }
      else
      {
        System.Data.DataTable dtSupNo = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNGetNewSupplementCode() NewSupplementNo");
        if ((dtSupNo != null) && (dtSupNo.Rows.Count > 0))
        {
          txtSuppNo.Text = dtSupNo.Rows[0]["NewSupplementNo"].ToString();
        }
        string commandText = "SELECT CAST(ID_NhanVien AS VARCHAR) + ' - ' + HoNV + ' ' + TenNV Name FROM VHRNhanVien WHERE ID_NhanVien = " + SharedObject.UserInfo.UserPid;
        System.Data.DataTable dtTmp = DataBaseAccess.SearchCommandTextDataTable(commandText);

        if (dtTmp.Rows.Count > 0)
        {
          this.txtCreateBy.Text = dtTmp.Rows[0][0].ToString();
        }

        this.txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
      }

      this.SetStatusControl();
      this.LoadDataComponent(dsSource);
    }

    /// <summary>
    /// SetStatusControl
    /// </summary>
    private void SetStatusControl()
    {
      this.canUpdate = (btnSave.Visible && this.nonConfirm);
      txtDescription.ReadOnly = !this.canUpdate;
      chkLock.Enabled = this.nonConfirm;
      //btnSave.Enabled = this.nonConfirm;
      if (this.nonConfirm == false)
      {
        this.btnPrint.Enabled = true;
      }
      else
      {
        this.btnPrint.Enabled = false;

      }
      if (chkLock.Checked)
      {
        ultcbDept.Enabled = false;
      }
    }
    #endregion Init Data

    #region ProcessData
    /// <summary>
    /// SaveBeforeClosing
    /// </summary>
    /// <returns></returns>
    private DialogResult SaveBeforeClosing()
    {
      DialogResult result = FunctionUtility.SaveBeforeClosing();
      if (result == DialogResult.Yes)
      {
        if (this.CheckInvalid())
        {
          bool success = this.SaveData();
          if (success)
          {
            WindowUtinity.ShowMessageSuccess("MSG0004");
            this.SaveSuccess = true;
          }
          else
          {
            this.SaveSuccess = false;
            this.LoadData();
          }
        }
      }
      return result;
    }

    /// <summary>
    /// LoadDataPackageComponent
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataComponent(DataSet dsSource)
    {
      //if (this.supplementNo != string.Empty && status == 0)
      //{
      //  foreach (DataRow drData in dsData.Tables["dtSupp"].Rows)
      //  {
      //    DataRow[] foundRow = dsData.Tables["dtSupp"].Select("MaterialCode = '" + drData["MaterialCode"] + "'");
      //    double qty = 0;
      //    for (int i = 0; i < foundRow.Length; i++)
      //    {
      //      qty += DBConvert.ParseDouble(foundRow[i]["SuppQty"].ToString());
      //    }

      //    drData["StockQty"] = DBConvert.ParseDouble(drData["StockQty"].ToString()) + qty; 
      //  }
      //}

      ultData.DataSource = dsSource.Tables[1];

      int count = ultData.Rows.Count;

      if (!this.canUpdate)
      {
        for (int j = 0; j < ultData.DisplayLayout.Bands[0].Columns.Count; j++)
        {
          ultData.DisplayLayout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
        }
        ultData.DisplayLayout.Bands[0].Columns["SuppQty"].CellActivation = Activation.AllowEdit;
      }
      else
      {
        for (int i = 0; i < count; i++)
        {
          UltraGridRow row = ultData.Rows[i];

          if (DBConvert.ParseInt(row.Cells["ControlType"].Value.ToString()) == 0)
          {
            row.Cells["ItemCode"].Activation = Activation.ActivateOnly;
          }
          else
          {
            row.Cells["ItemCode"].Activation = Activation.AllowEdit;
          }
        }
      }
    }

    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;

      int countComponent = ultData.Rows.Count;

      //SaveSupplementForWorkOrder
      this.SaveSupplementForWorkOrderInfo();

      if (this.supplementNo.Length == 0)
      {
        return false;
      }

      result = this.SaveSupplementForWorkOrderDetail();

      return result;
    }

    /// <summary>
    /// Save TblPLNSupplementForWorkOrderDetail
    /// </summary>
    /// <returns></returns>
    private bool SaveSupplementForWorkOrderDetail()
    {
      string storeName = string.Empty;
      bool result = true;

      int countComponent = ultData.Rows.Count;

      //save data
      for (int i = 0; i < countComponent; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        // 1. Save TblPLNSupplementForWorkOrderDetail
        storeName = string.Empty;

        long componentPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

        //insert 
        if (componentPid == long.MinValue)
        {
          storeName = "spPLNSupplementForWorkOrderDetail_Insert";
          DBParameter[] inputParamInsert = new DBParameter[8];
          inputParamInsert[0] = new DBParameter("@SuppPid", DbType.Int64, this.suppPid);
          inputParamInsert[1] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(row.Cells["WoPid"].Value.ToString()));
          if (row.Cells["ItemCode"].Value.ToString().Length > 0)
          {
            inputParamInsert[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, row.Cells["ItemCode"].Value.ToString());
          }

          if (row.Cells["Revision"].Value.ToString().Length > 0)
          {
            inputParamInsert[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
          }

          inputParamInsert[4] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, row.Cells["MaterialCode"].Value.ToString());
          inputParamInsert[5] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(row.Cells["Reason"].Value.ToString()));
          inputParamInsert[6] = new DBParameter("@Supplement", DbType.Double, DBConvert.ParseDouble(row.Cells["SuppQty"].Value.ToString()));
          inputParamInsert[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          DBParameter[] outputParamInsert = new DBParameter[1];
          outputParamInsert[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);
          componentPid = DBConvert.ParseLong(outputParamInsert[0].Value.ToString());
          if (componentPid == 0)
          {
            result = false;
            continue;
          }

          if (componentPid == -1)
          {
            result = false;
            string message = string.Format(FunctionUtility.GetMessage("WRN0003"), "Row " + DBConvert.ParseString(i + 1) + " Suppp Qty", "Stock Qty");
            WindowUtinity.ShowMessageErrorFromText(message);
            continue;
          }
        }
        //update BoxType
        else
        {
          storeName = "spPLNSupplementForWorkOrderDetail_Update";
          DBParameter[] inputParamUpdate = new DBParameter[9];

          inputParamUpdate[0] = new DBParameter("@Pid", DbType.Int64, componentPid);
          inputParamUpdate[1] = new DBParameter("@SuppPid", DbType.Int64, this.suppPid);
          inputParamUpdate[2] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(row.Cells["WoPid"].Value.ToString()));
          if (row.Cells["ItemCode"].Value.ToString().Length > 0)
          {
            inputParamUpdate[3] = new DBParameter("@ItemCode", DbType.AnsiString, 16, row.Cells["ItemCode"].Value.ToString());
          }

          if (row.Cells["Revision"].Value.ToString().Length > 0)
          {
            inputParamUpdate[4] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()));
          }

          inputParamUpdate[5] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, row.Cells["MaterialCode"].Value.ToString());
          inputParamUpdate[6] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(row.Cells["Reason"].Value.ToString()));
          inputParamUpdate[7] = new DBParameter("@Supplement", DbType.Double, DBConvert.ParseDouble(row.Cells["SuppQty"].Value.ToString()));
          inputParamUpdate[8] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          DBParameter[] outputParamUpdate = new DBParameter[1];
          outputParamUpdate[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamUpdate, outputParamUpdate);

          componentPid = DBConvert.ParseLong(outputParamUpdate[0].Value.ToString());
          if (componentPid == 0)
          {
            result = false;
            continue;
          }
          else if (componentPid == -1)
          {
            result = false;
            string message = string.Format(FunctionUtility.GetMessage("WRN0003"), "Row " + DBConvert.ParseString(i + 1) + " Supp Qty", "Old Sup Qty");
            WindowUtinity.ShowMessageErrorFromText(message);
            continue;
          }
          else if (componentPid == -2)
          {
            result = false;
            string message = string.Format(FunctionUtility.GetMessage("WRN0003"), "Row " + DBConvert.ParseString(i + 1) + " Suppp Qty", "Remain");
            WindowUtinity.ShowMessageErrorFromText(message);
            continue;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// TblPLNSupplementForWorkOrder
    /// </summary>
    /// <returns></returns>
    private void SaveSupplementForWorkOrderInfo()
    {
      string result = string.Empty;


      string storeName = string.Empty;

      if (this.supplementNo.Length > 0)
      {
        storeName = "spPLNSupplement_Update";
        DBParameter[] inputParamUpdate = new DBParameter[5];
        inputParamUpdate[0] = new DBParameter("@SupplementNo", DbType.AnsiString, 16, this.supplementNo);
        inputParamUpdate[1] = new DBParameter("@Description", DbType.AnsiString, 255, this.txtDescription.Text);
        if (this.chkLock.Checked)
        {
          inputParamUpdate[2] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamUpdate[2] = new DBParameter("@Status", DbType.Int32, 0);
        }
        inputParamUpdate[3] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (ultcbDept.Value.ToString().Length > 0)
        {
          inputParamUpdate[4] = new DBParameter("@Dept", DbType.AnsiString, 8, ultcbDept.Value);
        }
        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamUpdate, null);
      }
      else
      {
        storeName = "spPLNSupplement_Insert";
        DBParameter[] inputParamInsert = new DBParameter[4];
        inputParamInsert[0] = new DBParameter("@Description", DbType.AnsiString, 255, this.txtDescription.Text);
        if (this.chkLock.Checked)
        {
          inputParamInsert[1] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else
        {
          inputParamInsert[1] = new DBParameter("@Status", DbType.Int32, 0);
        }

        inputParamInsert[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (ultcbDept.Value.ToString().Length > 0)
        {
          inputParamInsert[3] = new DBParameter("@Dept", DbType.AnsiString, 8, ultcbDept.Value.ToString());
        }
        DBParameter[] outputParamInsert = new DBParameter[2];
        outputParamInsert[0] = new DBParameter("@ResultSuppNo", DbType.AnsiString, 16, string.Empty);
        outputParamInsert[1] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamInsert, outputParamInsert);
        this.supplementNo = outputParamInsert[0].Value.ToString();
        this.suppPid = DBConvert.ParseLong(outputParamInsert[1].Value.ToString());
      }
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <returns></returns>
    private bool CheckInvalid()
    {
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();

      //Department
      if (ultcbDept.Text.Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Department");
        return false;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        //Material Code
        if (row.Cells["MaterialCode"].Value.ToString().Trim().Length == 0)
        {
          row.Cells["MaterialCode"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageError("ERR0001", "Material Code");
          return false;
        }
        else
        {
          commandText = "SELECT MaterialCode FROM VBOMMaterials WHERE IsControl = 1 AND MaterialCode = '" + row.Cells["MaterialCode"].Value.ToString().Trim() + "'";
          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);

          if (dtCheck == null || dtCheck.Rows.Count == 0)
          {
            row.Cells["MaterialCode"].Appearance.BackColor = Color.Yellow;
            WindowUtinity.ShowMessageError("ERR0001", "Material Code");
            return false;
          }
        }

        // WO
        if (row.Cells["WoPid"].Value.ToString().Trim().Length == 0)
        {
          row.Cells["WoPid"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageError("ERR0001", "WO");
          return false;
        }
        else if (this.status == 0)
        {
          commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Confirm = 1 AND Status != 1 AND Pid =" + DBConvert.ParseLong(row.Cells["WoPid"].Value.ToString()) + "";
          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck == null || dtCheck.Rows.Count == 0)
          {
            row.Cells["WoPid"].Appearance.BackColor = Color.Yellow;
            WindowUtinity.ShowMessageError("ERR0001", "WO");
            return false;
          }
        }

        // ItemCode
        if (DBConvert.ParseInt(row.Cells["ControlType"].Value.ToString()) == 1)
        {
          if (row.Cells["ItemCode"].Value.ToString().Trim().Length == 0)
          {
            row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
            WindowUtinity.ShowMessageError("ERR0001", "Item Code");
            return false;
          }
          else if (this.status == 0)
          {
            commandText = "SELECT DISTINCT ItemCode, Revision";
            commandText += " FROM TblPLNWorkOrderConfirmedDetails ";
            commandText += " WHERE WorkOrderPid = " + DBConvert.ParseLong(row.Cells["WoPid"].Value.ToString());
            commandText += " AND ItemCode = '" + row.Cells["ItemCode"].Value.ToString() + "'";
            dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck == null || dtCheck.Rows.Count == 0)
            {
              row.Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
              WindowUtinity.ShowMessageError("ERR0001", "ItemCode");
              return false;
            }
          }
        }

        // Revision
        if (DBConvert.ParseInt(row.Cells["ControlType"].Value.ToString()) == 1)
        {
          if (DBConvert.ParseInt(row.Cells["Revision"].Value.ToString()) == int.MinValue)
          {
            row.Cells["Revision"].Appearance.BackColor = Color.Yellow;
            WindowUtinity.ShowMessageError("ERR0001", "Revision");
            return false;
          }
          else if (this.status == 0)
          {
            commandText = "SELECT DISTINCT ItemCode, Revision";
            commandText += " FROM TblPLNWorkOrderConfirmedDetails ";
            commandText += " WHERE WorkOrderPid = " + DBConvert.ParseLong(row.Cells["WoPid"].Value.ToString());
            commandText += " AND ItemCode = '" + row.Cells["ItemCode"].Value.ToString() + "'";
            commandText += " AND Revision = " + DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
            dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck == null || dtCheck.Rows.Count == 0)
            {
              row.Cells["Revision"].Appearance.BackColor = Color.Yellow;
              WindowUtinity.ShowMessageError("ERR0001", "Revision");
              return false;
            }
          }
        }

        // Reason
        if (row.Cells["Reason"].Value.ToString().Trim().Length == 0 || DBConvert.ParseInt(row.Cells["Reason"].Value.ToString()) == int.MinValue)
        {
          row.Cells["Reason"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageError("ERR0001", "Reason");
          return false;
        }

        // Qty
        double qty = DBConvert.ParseDouble(row.Cells["SuppQty"].Value.ToString());
        if (qty < 0)
        {
          row.Cells["SuppQty"].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageError("ERR0001", "Supplement Qty");
          return false;
        }
        //Check Close WO
        long wo = DBConvert.ParseLong(row.Cells["WoPid"].Value.ToString());
        string item = row.Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
        commandText = string.Format(@"SELECT WO, ItemCode, Revision
                                      FROM VPLNListOfItemCloseWO
                                      WHERE WO = {0} AND ItemCode = '{1}' AND Revision = {2}
                                                     AND (ISNULL(CloseCAR, 0) = 1 OR ISNULL(CloseFactory, 0) = 1 )", wo, item, revision);
        dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtCheck != null && dtCheck.Rows.Count > 0)
        {
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
          WindowUtinity.ShowMessageError("ERR0001", "WO is Close");
          return false;
        }
      }

      return true;
    }
    #endregion ProcessData

    #region Button Click
    /// <summary>
    /// btnSave_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        bool success = this.SaveData();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        this.LoadData();
      }
    }

    /// <summary>
    /// btnClose_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// btnOpenDialog_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOpenDialog_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtFileName.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtFileName.Text.Trim().Length > 0);
    }

    /// <summary>
    /// Get Data From Excel File
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="nSheet"></param>
    /// <param name="areaData"></param>
    /// <returns></returns>
    private System.Data.DataTable GetDataFromExcel(string fileName, string sheetName, string areaData)
    {
      try
      {
        object missing = System.Reflection.Missing.Value;
        Excel.ApplicationClass xl = new Excel.ApplicationClass();
        Excel.Workbook xlBook;
        Excel.Sheets xlSheets;
        Excel.Worksheet xlSheet;

        xlBook = (Excel.Workbook)xl.Workbooks.Open(fileName, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
        xlSheets = xlBook.Worksheets;
        xlSheet = (Excel.Worksheet)xlSheets.get_Item(sheetName);
        xlBook.Close(null, null, null);

        OleDbConnection connection = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + fileName + ";Extended Properties=Excel 8.0;");
        string commandText = string.Format(@"SELECT * FROM [{0}${1}]", sheetName, areaData);
        OleDbDataAdapter adp = new OleDbDataAdapter(commandText, connection);
        System.Data.DataTable dtXLS = new System.Data.DataTable();
        adp.Fill(dtXLS);
        connection.Close();
        if (dtXLS == null)
        {
          return null;
        }
        return dtXLS;
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0069");
        return null;
      }
    }

    /// <summary>
    /// btnImport Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      System.Data.DataTable dtSource = this.GetDataFromExcel(txtFileName.Text.Trim(), "Sheet1 (1)", "B3:G500");
      if (dtSource == null)
      {
        return;
      }
      System.Data.DataTable dtSourceGrid = (DataTable)ultData.DataSource;
      string materialCode = string.Empty;
      string message = string.Empty;

      foreach (DataRow drRow in dtSource.Rows)
      {
        materialCode = drRow[0].ToString().Trim();
        if (materialCode.Length > 0)
        {
          try
          {
            DataRow row = dtSourceGrid.NewRow();

            // MaterialCode
            row["MaterialCode"] = materialCode;

            string commandTextMaterial = string.Format("SELECT MaterialNameEn, Unit, ControlType FROM VBOMMaterials WHERE MaterialCode = '{0}'", materialCode);
            System.Data.DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandTextMaterial);
            int controlType = 0;
            if (dt.Rows.Count > 0)
            {
              row["MaterialNameEn"] = dt.Rows[0]["MaterialNameEn"].ToString().Trim();
              row["Unit"] = dt.Rows[0]["Unit"].ToString().Trim();
              controlType = DBConvert.ParseInt(dt.Rows[0]["ControlType"].ToString());
              if (dt.Rows[0]["ControlType"].ToString().Trim().Length > 0)
              {
                row["ControlType"] = dt.Rows[0]["ControlType"].ToString();
              }
              row["Issued"] = 0;

              string commandText = string.Format(@"SELECT ISNULL(Qty,0) StockQty FROM VWHDMaterialStockBalance 
                                                  WHERE [Recovery] = 0 AND MaterialCode = '{0}'", materialCode);
              System.Data.DataTable dtFreeQty = DataBaseAccess.SearchCommandTextDataTable(commandText);

              if (dtFreeQty.Rows.Count > 0)
              {
                if (DBConvert.ParseDouble(dtFreeQty.Rows[0][0].ToString()) != double.MinValue)
                {
                  row["StockQty"] = DBConvert.ParseDouble(dtFreeQty.Rows[0][0].ToString());
                }
                else
                {
                  row["StockQty"] = 0;
                }
              }
              else
              {
                row["StockQty"] = 0;
              }
            }

            //Supp Qty
            double suppQty = DBConvert.ParseDouble(drRow[1].ToString());
            if (suppQty != double.MinValue)
            {
              row["SuppQty"] = suppQty;
            }

            // Reason
            string code = drRow[2].ToString();
            if (code.Length > 0)
            {
              string commandText = string.Format("SELECT Code FROM TblBOMCodeMaster WHERE [Group] = 1007 AND Value LIKE '{0}'", code);
              object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
              if (obj != null)
              {
                int reason = (int)obj;
                row["Reason"] = reason;
              }
            }

            // WO
            if (DBConvert.ParseLong(drRow["WO"].ToString()) != int.MinValue)
            {
              row["WoPid"] = DBConvert.ParseLong(drRow["WO"].ToString());
            }

            // ItemCode
            if (drRow["ItemCode"].ToString().Trim().Length > 0)
            {
              row["ItemCode"] = drRow["ItemCode"].ToString();
            }

            // Revision
            if (DBConvert.ParseInt(drRow["Revision"].ToString()) != int.MinValue)
            {
              row["Revision"] = DBConvert.ParseInt(drRow["Revision"].ToString());
            }

            dtSourceGrid.Rows.Add(row);
          }
          catch { }
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0024");
      btnImport.Enabled = false;
    }

    /// <summary>
    /// Print 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      ViewPLN_99_001 view = new ViewPLN_99_001();
      view.suppNo = this.supplementNo;
      view.ncategory = 1;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    /// <summary>
    /// btnGetTemplate Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "SupplementInformationTemplate";
      string sheetName = "Sheet1";
      string outFileName = "Supplement Information Template";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    /// <summary>
    /// txtFileName Text Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtFileName_TextChanged(object sender, EventArgs e)
    {
      if (txtFileName.Text.Trim().Length > 0)
      {
        btnImport.Enabled = true;
      }
    }
    #endregion Button Click

    #region ultComponent Handle Event
    /// <summary>
    /// Diect Labour After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDirectLabour_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "materialcode":
          try
          {
            e.Cell.Row.Cells["ControlType"].Value = udrpMaterialCode.SelectedRow.Cells["ControlType"].Value;
            if (DBConvert.ParseInt(e.Cell.Row.Cells["ControlType"].Value.ToString()) == 0)
            {
              e.Cell.Row.Cells["ItemCode"].Activation = Activation.ActivateOnly;
              this.isResetItemCode = true;
              e.Cell.Row.Cells["ItemCode"].Value = DBNull.Value;
              this.isResetItemCode = false;
            }
            else
            {
              e.Cell.Row.Cells["ItemCode"].Activation = Activation.AllowEdit;
            }
          }
          catch
          {
            e.Cell.Row.Cells["ControlType"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["MaterialNameEn"].Value = udrpMaterialCode.SelectedRow.Cells["MaterialNameEN"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["MaterialNameEn"].Value = DBNull.Value;
          }

          try
          {
            e.Cell.Row.Cells["Unit"].Value = udrpMaterialCode.SelectedRow.Cells["Unit"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["Unit"].Value = DBNull.Value;
          }

          e.Cell.Row.Cells["Issued"].Value = 0;

          string commandText = string.Empty;
          commandText += " SELECT ISNULL(Qty,0) StockQty";
          commandText += " FROM VWHDMaterialStockBalance  ";
          commandText += " WHERE [Recovery] = 0 AND MaterialCode ='" + e.Cell.Row.Cells["MaterialCode"].Value.ToString() + "'";
          System.Data.DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

          if (dt.Rows.Count > 0)
          {
            if (DBConvert.ParseDouble(dt.Rows[0][0].ToString()) != double.MinValue)
            {
              e.Cell.Row.Cells["StockQty"].Value = DBConvert.ParseDouble(dt.Rows[0][0].ToString());
            }
            else
            {
              e.Cell.Row.Cells["StockQty"].Value = 0;
            }
          }
          else
          {
            e.Cell.Row.Cells["StockQty"].Value = 0;
          }

          if (DBConvert.ParseInt(e.Cell.Row.Cells["ControlType"].Value.ToString()) == 0)
          {
            e.Cell.Row.Cells["ItemCode"].Activation = Activation.ActivateOnly;
          }
          else
          {
            e.Cell.Row.Cells["ItemCode"].Activation = Activation.AllowEdit;
          }
          break;
        case "wopid":
          //if (DBConvert.ParseInt(e.Cell.Row.Cells["ControlType"].Value.ToString()) == 1)
          //{
          //commandText = string.Empty;
          //commandText += " SELECT DISTINCT ItemCode, Revision";
          //commandText += " FROM TblPLNWorkOrderConfirmedDetails ";
          //commandText += " WHERE WorkOrderPid =" + DBConvert.ParseInt(e.Cell.Row.Cells["WoPid"].Value.ToString());
          //dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

          //udrpItemCode.DataSource = dt;
          //udrpItemCode.ValueMember = "ItemCode";
          //udrpItemCode.DisplayMember = "ItemCode";
          //udrpItemCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
          //e.Cell.Row.Cells["ItemCode"].ValueList = this.udrpItemCode;

          this.isResetItemCode = true;
          e.Cell.Row.Cells["ItemCode"].Value = DBNull.Value;
          this.isResetItemCode = false;
          //}
          break;
        case "itemcode":
          try
          {
            UltraDropDown ultraDDItem = (UltraDropDown)e.Cell.ValueList;
            e.Cell.Row.Cells["Revision"].Value = ultraDDItem.SelectedRow.Cells["Revision"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["Revision"].Value = DBNull.Value;
          }
          break;
        case "reason":
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Direct Labour Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDirectLabour_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      string commandText = string.Empty;
      switch (columnName.ToLower())
      {
        case "materialcode":
          commandText = "SELECT MaterialCode FROM VBOMMaterials WHERE IsControl = 1 AND MaterialCode ='" + text + "'";
          System.Data.DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
            {
              WindowUtinity.ShowMessageWarning("ERR0001", columnName);
              e.Cancel = true;
            }
          }
          else
          {
            WindowUtinity.ShowMessageWarning("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        case "wopid":
          commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Status != 1 AND Confirm = 1 AND Pid = " + DBConvert.ParseLong(text);
          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null)
          {
            if (dtCheck.Rows.Count == 0)
            {
              WindowUtinity.ShowMessageWarning("ERR0001", columnName);
              e.Cancel = true;
            }
          }

          break;
        case "itemcode":
          if (!isResetItemCode)
          {
            commandText = "SELECT ItemCode FROM TblPLNWOInfoDetailGeneral WHERE WoInfoPID = " + DBConvert.ParseInt(e.Cell.Row.Cells["WoPid"].Value.ToString()) + " AND ItemCode = '" + text + "'";
            dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck != null)
            {
              if (dtCheck.Rows.Count > 0)
              {
                if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
                {
                  WindowUtinity.ShowMessageWarning("ERR0001", columnName);
                  e.Cancel = true;
                }
              }
              else
              {
                WindowUtinity.ShowMessageWarning("ERR0001", columnName);
                e.Cancel = true;
              }
            }
          }

          break;
        case "reason":
          commandText = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 1007 AND Value = '" + text + "'";
          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
            {
              WindowUtinity.ShowMessageWarning("ERR0001", columnName);
              e.Cancel = true;
            }
          }
          else
          {
            WindowUtinity.ShowMessageWarning("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Direct Labour Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDirectLabour_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["ControlType"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialNameEn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Required"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Issued"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["StockQty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = udrpMaterialCode;
      e.Layout.Bands[0].Columns["Reason"].ValueList = udrpReason;
      e.Layout.Bands[0].Columns["WoPid"].ValueList = udrpWo;

      e.Layout.Bands[0].Columns["WoPid"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Issued"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["StockQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SuppQty"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialNameEn"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["WoPid"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["StockQty"].Header.Caption = "Stock Qty";
      e.Layout.Bands[0].Columns["SuppQty"].Header.Caption = "Supp Qty";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Delete Direct Labour
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDirectLabour_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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

          DataBaseAccess.ExecuteStoreProcedure("spPLNSupplementForWorkOrderdetail_Delete", inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) == 0)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadData();
            return;
          }
          else if (DBConvert.ParseInt(outputParams[0].Value.ToString()) == -1)
          {
            string message = string.Format(FunctionUtility.GetMessage("WRN0003"), "Issue Qty", "0");
            WindowUtinity.ShowMessageErrorFromText(message);
            this.LoadData();
            return;
          }
        }
      }
    }

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      if (string.Compare(columnName, "ItemCode", true) == 0)
      {
        long wo = DBConvert.ParseLong(e.Cell.Row.Cells["WoPid"].Value.ToString());
        UltraDropDown ultraDDItem = (UltraDropDown)e.Cell.ValueList;
        e.Cell.ValueList = this.LoadUltraDDItem(wo, ultraDDItem);
      }
    }
    #endregion ultComponent Handle Event
  }
}
