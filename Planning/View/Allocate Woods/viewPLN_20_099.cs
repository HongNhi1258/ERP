/*
  Author      : 
  Date        : 14/03/2013
  Description : Insert , Update Supplement No Woods
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace DaiCo.Planning
{
  public partial class viewPLN_20_099 : MainUserControl
  {
    #region Field    
    private bool nonConfirm = true;
    public string supplementNo = string.Empty;
    public long suppPid = long.MinValue;

    private DataTable dtGroup = new DataTable();
    private DataTable dtReason = new DataTable();
    private DataTable dtWo = new DataTable();

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
    public viewPLN_20_099()
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
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";

      // Group
      this.MakeGroupData();

      // Reason
      this.MakeReasonData();

      // WO
      this.MakeWOData();

      // Group
      this.LoadDropdownGroup(this.udrpMaterialCode);

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
    /// Group
    /// </summary>
    private void MakeGroupData()
    {
      string commandText = string.Empty;
      commandText += " SELECT GRP.[Group] + '-' + CAT.Category Code, CAT.Name GroupName ";
      commandText += " FROM TblGNRMaterialGroup GRP ";
      commandText += " 	INNER JOIN TblGNRMaterialCategory CAT ON GRP.[Group] = CAT.[Group] ";
      commandText += " WHERE GRP.Warehouse = 3 AND CAT.IsControl = 1";
      commandText += " ORDER BY CAT.Category ";

      this.dtGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
    }

    private void MakeDepartment()
    {
      string cm = "SELECT Code, Code +' - '+ Name AS Name FROM VHRDDepartmentInfo WHERE DelFlag = 0";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultcbDept, dt, "Code", "Name", false, "Code");
    }

    /// <summary>
    /// Reason Data
    /// </summary>
    private void MakeReasonData()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 1007 AND DeleteFlag = 0 Order By Sort";
      this.dtReason = DataBaseAccess.SearchCommandTextDataTable(commandText);
    }

    /// <summary>
    /// Group
    /// </summary>
    /// <param name="udrpMaterials"></param>
    private void LoadDropdownGroup(UltraDropDown udrpMaterials)
    {
      udrpMaterials.DataSource = this.dtGroup;
      udrpMaterials.ValueMember = "Code";
      udrpMaterials.DisplayMember = "Code";
      udrpMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpMaterials.DisplayLayout.Bands[0].Columns["Code"].Width = 100;
      udrpMaterials.DisplayLayout.Bands[0].Columns["GroupName"].Width = 200;
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

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SupplementNo", DbType.AnsiString, 16, this.supplementNo) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNWoodsSupplementInfomationBySupplementNo_Select", inputParam);
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
        System.Data.DataTable dtSupNo = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNWoodsGetNewSupplementCode() NewSupplementNo");
        if ((dtSupNo != null) && (dtSupNo.Rows.Count > 0))
        {
          txtSuppNo.Text = dtSupNo.Rows[0]["NewSupplementNo"].ToString();
        }

        this.txtCreateBy.Text = Shared.Utility.SharedObject.UserInfo.EmpName;
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
      //this.btnSave.Enabled = this.nonConfirm;
      if (chkLock.Checked)
      {
        ultcbDept.Enabled = false;
      }

    }
    #endregion Init Data

    #region ProcessData

    /// <summary>
    /// Get Main Structure Datatable Row
    /// </summary>
    /// <returns></returns>
    private DataTable UltraTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("CarcassCode", typeof(System.String));

      return dt;
    }

    /// <summary>
    /// LoadDataPackageComponent
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataComponent(DataSet dsSource)
    {
      ultData.DataSource = dsSource.Tables[1];
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultData.Rows[i];
        long wo = DBConvert.ParseLong(rowGrid.Cells["WO"].Value.ToString());

        if (wo != long.MinValue)
        {
          UltraDropDown ultCarcass = (UltraDropDown)rowGrid.Cells["CarcassCode"].ValueList;
          string select = string.Empty;
          select = "WoInfoPID =" + wo;

          DataRow[] foundRow = dsSource.Tables[2].Select(select);
          DataTable dtContainer = this.UltraTable();
          for (int j = 0; j < foundRow.Length; j++)
          {
            DataRow rowAdd = dtContainer.NewRow();
            rowAdd["CarcassCode"] = foundRow[j]["CarcassCode"].ToString();

            dtContainer.Rows.Add(rowAdd);
          }

          if (ultCarcass == null)
          {
            ultCarcass = new UltraDropDown();
            this.Controls.Add(ultCarcass);
          }

          DataView dtView = dtContainer.DefaultView;
          dtView.Sort = "CarcassCode";
          dtContainer = dtView.ToTable();

          ultCarcass.DataSource = dtContainer;
          ultCarcass.ValueMember = "CarcassCode";
          ultCarcass.DisplayMember = "CarcassCode";
          ultCarcass.DisplayLayout.Bands[0].ColHeadersVisible = true;

          ultCarcass.Visible = false;

          rowGrid.Cells["CarcassCode"].ValueList = ultCarcass;
        }
      }

      int count = ultData.Rows.Count;

      if (!this.canUpdate)
      {
        for (int j = 0; j < ultData.DisplayLayout.Bands[0].Columns.Count; j++)
        {
          ultData.DisplayLayout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
        }
        ultData.DisplayLayout.Bands[0].Columns["SuppQty"].CellActivation = Activation.AllowEdit;
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

      // Save Master Supplement
      this.SaveSupplement();

      if (this.supplementNo.Length == 0)
      {
        return false;
      }

      result = this.SaveSupplementDetail();

      return result;
    }

    /// <summary>
    /// Save Supplement Detail Information
    /// </summary>
    /// <returns></returns>
    private bool SaveSupplementDetail()
    {
      string storeName = string.Empty;
      bool result = true;

      int countComponent = ultData.Rows.Count;

      for (int i = 0; i < countComponent; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        storeName = string.Empty;

        long componentPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

        //insert 
        if (componentPid == long.MinValue)
        {
          storeName = "spPLNWoodsSupplementDetail_Insert";
          DBParameter[] inputParamInsert = new DBParameter[8];
          inputParamInsert[0] = new DBParameter("@SuppPid", DbType.Int64, this.suppPid);
          inputParamInsert[1] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(row.Cells["WO"].Value.ToString()));
          inputParamInsert[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, row.Cells["CarcassCode"].Value.ToString());
          inputParamInsert[3] = new DBParameter("@Group", DbType.String, row.Cells["Code"].Value.ToString().Substring(0, 3));
          inputParamInsert[4] = new DBParameter("@Category", DbType.String, row.Cells["Code"].Value.ToString().Substring(4, 2));
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
        //update
        else
        {
          storeName = "spPLNWoodsSupplementDetail_Update";
          DBParameter[] inputParamUpdate = new DBParameter[9];

          inputParamUpdate[0] = new DBParameter("@Pid", DbType.Int64, componentPid);
          inputParamUpdate[1] = new DBParameter("@SuppPid", DbType.Int64, this.suppPid);
          inputParamUpdate[2] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(row.Cells["WO"].Value.ToString()));
          inputParamUpdate[3] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, row.Cells["CarcassCode"].Value.ToString());
          inputParamUpdate[4] = new DBParameter("@Group", DbType.String, row.Cells["Code"].Value.ToString().Substring(0, 3));
          inputParamUpdate[5] = new DBParameter("@Category", DbType.String, row.Cells["Code"].Value.ToString().Substring(4, 2));
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
        }
      }
      return result;
    }

    /// <summary>
    /// Save Supplement Master
    /// </summary>
    /// <returns></returns>
    private void SaveSupplement()
    {
      string result = string.Empty;

      string storeName = string.Empty;

      if (this.supplementNo.Length > 0)
      {
        storeName = "spPLNWoodsSupplement_Update";
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
        storeName = "spPLNWoodsSupplement_Insert";
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
        if (ultcbDept.SelectedRow != null)
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

    private bool CheckInvalid()
    {
      System.Data.DataTable dtSource = (DataTable)ultData.DataSource;
      //Department
      if (ultcbDept.Text.Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Department");
        return false;
      }
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState != DataRowState.Deleted)
        {
          // Group Code
          string text = row["Code"].ToString().Trim();
          string commandText = string.Empty;
          commandText += " SELECT COUNT(*) ";
          commandText += " FROM TblGNRMaterialCategory ";
          commandText += " WHERE [Group] + '-' + Category ='" + text + "'";

          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck == null || dtCheck.Rows.Count == 0
              || DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Group");
            return false;
          }

          // WO
          if (row["WO"].ToString().Trim().Length == 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", "WO");
            return false;
          }

          // CarcassCode
          if (row["CarcassCode"].ToString().Trim().Length == 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", "Carcass Code");
            return false;
          }
          // Check WO Close

          long wo = DBConvert.ParseLong(row["WO"].ToString());
          string cacass = row["CarcassCode"].ToString().Trim();
          commandText = string.Format(@"SELECT WO, CarcassCode
                                               FROM VPLNListOfItemCloseWO
                                               WHERE WO = {0} AND CarcassCode = '{1}' 
                                                              AND (ISNULL(CloseCOM1, 0) = 1 OR ISNULL(CloseFactory, 0) = 1 )", wo, cacass);
          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null && dtCheck.Rows.Count > 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", "WO is Close");
            return false;
          }
          // Reason
          if (row["Reason"].ToString().Trim().Length == 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", "Reason");
            return false;
          }

          // Qty
          double qty = DBConvert.ParseDouble(row["SuppQty"].ToString());
          if (qty <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", "Supplement Qty");
            return false;
          }
        }
      }

      DataTable dt = (DataTable)ultData.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dtrow = dt.Rows[i];
        if (dtrow.RowState == DataRowState.Modified)
        {
          double issue = DBConvert.ParseDouble(dtrow["Issued"].ToString());
          double suppqty = DBConvert.ParseDouble(dtrow["SuppQty"].ToString());
          double suppQtyOld = DBConvert.ParseDouble(dtrow["SuppQtyOld"].ToString());
          if (suppqty > suppQtyOld || suppqty < issue)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Supplement Qty");
            return false;
          }
        }
      }

      return true;
    }

    /// <summary>
    /// Load Move To Customer
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="ultRevision"></param>
    /// <returns></returns>
    private UltraDropDown LoadCarcass(long wo, UltraDropDown ultCarcass)
    {
      if (ultCarcass == null)
      {
        ultCarcass = new UltraDropDown();
        this.Controls.Add(ultCarcass);
      }

      string commandText = string.Empty;
      commandText += " SELECT DISTINCT IIF.CarcassCode ";
      commandText += " FROM TblPLNWOInfoDetailGeneral WO ";
      commandText += " 	INNER JOIN TblBOMItemInfo IIF ON WO.ItemCode = IIF.ItemCode ";
      commandText += " 									AND WO.Revision = IIF.Revision	 ";
      commandText += " WHERE WO.WoInfoPID = " + wo;

      DataTable dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCarcass.DataSource = dt;
      ultCarcass.ValueMember = "CarcassCode";
      ultCarcass.DisplayMember = "CarcassCode";
      ultCarcass.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCarcass.Visible = false;
      return ultCarcass;
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
    #endregion Button Click

    #region ultComponent Handle Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Required"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Issued"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["StockQty"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Code"].ValueList = udrpMaterialCode;
      e.Layout.Bands[0].Columns["Reason"].ValueList = udrpReason;
      e.Layout.Bands[0].Columns["WO"].ValueList = udrpWo;

      e.Layout.Bands[0].Columns["Required"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Issued"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["StockQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SuppQty"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["StockQty"].Header.Caption = "Stock Qty";
      e.Layout.Bands[0].Columns["SuppQty"].Header.Caption = "Supp Qty";

      e.Layout.Bands[0].Columns["Code"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["CarcassCode"].CellAppearance.BackColor = Color.LightBlue;
      //e.Layout.Bands[0].Columns["SuppQty"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Reason"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      string commandText = string.Empty;
      switch (columnName.ToLower())
      {
        case "code":
          commandText += " SELECT COUNT(*) ";
          commandText += " FROM TblGNRMaterialGroup GRP ";
          commandText += " 	INNER JOIN TblGNRMaterialCategory CAT ON GRP.[Group] = CAT.[Group] ";
          commandText += " WHERE GRP.Warehouse = 3 AND CAT.IsControl = 1";
          commandText += "  AND GRP.[Group] + '-' + CAT.Category = '" + text + "'";

          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null && dtCheck.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", columnName);
              e.Cancel = true;
            }
          }
          else
          {
            WindowUtinity.ShowMessageError("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        case "wo":
          commandText += " SELECT Pid  ";
          commandText += " FROM TblPLNWorkOrder  ";
          commandText += " WHERE Status != 1  ";
          commandText += "  AND Confirm = 1 ";
          commandText += "  AND Pid = " + DBConvert.ParseLong(text);

          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null)
          {
            if (dtCheck.Rows.Count == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", columnName);
              e.Cancel = true;
            }
          }

          break;
        case "carcasscode":
          if (!isResetItemCode)
          {
            commandText += " SELECT COUNT(*) ";
            commandText += " FROM TblPLNWOInfoDetailGeneral WO ";
            commandText += "    INNER JOIN TblBOMItemInfo IIF ON WO.ItemCode = IIF.ItemCode ";
            commandText += "                                  AND WO.Revision = IIF.Revision ";
            commandText += " WHERE WO.WoInfoPID = " + DBConvert.ParseInt(e.Cell.Row.Cells["WO"].Value.ToString());
            commandText += "    AND IIF.CarcassCode = '" + text + "'";
            dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck != null)
            {
              if (dtCheck.Rows.Count > 0)
              {
                if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
                {
                  WindowUtinity.ShowMessageError("ERR0001", columnName);
                  e.Cancel = true;
                }
              }
              else
              {
                WindowUtinity.ShowMessageError("ERR0001", columnName);
                e.Cancel = true;
              }
            }
          }
          break;
        case "reason":
          commandText += " SELECT COUNT(*) ";
          commandText += " FROM TblBOMCodeMaster ";
          commandText += " WHERE [Group] = 1007 ";
          commandText += "  AND Value = '" + text + "'";

          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", columnName);
              e.Cancel = true;
            }
          }
          else
          {
            WindowUtinity.ShowMessageError("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// After Cell Update ==> Get Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      string commandText = string.Empty;
      long wo = 0;
      switch (columnName)
      {
        case "code":
          try
          {
            e.Cell.Row.Cells["Name"].Value = udrpMaterialCode.SelectedRow.Cells["GroupName"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["Name"].Value = DBNull.Value;
          }

          e.Cell.Row.Cells["Issued"].Value = 0;

          // Get Stock Qty
          commandText += " SELECT ROUND(Qty, 3) StockQty";
          commandText += " FROM VWHDWoodsStockBalanceAsCategory  ";
          commandText += " WHERE [Group] + '-' + Category ='" + e.Cell.Row.Cells["Code"].Value.ToString() + "'";

          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
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
          break;
        case "wo":
          wo = DBConvert.ParseLong(e.Cell.Row.Cells["WO"].Value.ToString().Trim());
          UltraDropDown ultCarcass = (UltraDropDown)e.Cell.Row.Cells["CarcassCode"].ValueList;
          if (wo != long.MinValue)
          {
            e.Cell.Row.Cells["CarcassCode"].ValueList = this.LoadCarcass(wo, ultCarcass);
          }

          this.isResetItemCode = true;
          e.Cell.Row.Cells["CarcassCode"].Value = DBNull.Value;
          this.isResetItemCode = false;
          break;
        case "carcasscode":
          e.Cell.Row.Cells["Required"].Value = 0;

          break;
        default:
          break;
      }
    }
    #endregion ultComponent Handle Event
  }
}
