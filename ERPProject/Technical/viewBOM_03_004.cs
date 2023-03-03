using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_03_004 : MainUserControl
  {
    public string finishingCode;
    private DataTable dataSource;
    private IList listDeletedPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private IList listDirectlabourDeletedPid = new ArrayList();
    private IList listDirectlabourDeletingPid = new ArrayList();
    private bool loadingData = false;
    private bool canUpdate = false;
    public viewBOM_03_004()
    {
      InitializeComponent();
    }

    private void viewBOM_03_004_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + Shared.Utility.SharedObject.UserInfo.UserName + " | " + Shared.Utility.SharedObject.UserInfo.LoginDate;
      this.LoadDropdownMaterial();
      this.LoadDropdownSectionCode();
      this.LoadUltraCBFinishingType();
      this.LoadCBSheenLevel();
      this.LoadCustomer();
      this.LoadLastCarcassProcess();

      if (this.finishingCode.Length == 0)
      {
        Utility.LoadDropdownFinishsingCode(multicmbFinCode, "1 = 1");
        ultraCBFinishing.Visible = false;
      }
      else
      {
        Utility.LoadDropdownFinishsingCode(multicmbFinCode, string.Format("FinCode <> '{0}'", this.finishingCode));
        txtFinishingCode.Visible = false;
        this.LoadUltraCBFinishing();
      }
      multicmbFinCode.ColumnWidths = "100, 400, 0";
      this.LoadData();
      if (string.Compare(this.ViewParam, "BOM_03_003_02", true) == 0)
      {
        chkLock.Visible = false;
      }
    }

    #region Init Method

    private void LoadUltraCBFinishingType()
    {
      string commandText = @"SELECT Code, Value
                              FROM TblBOMCodeMaster 
                              WHERE [Group] = 2917";
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbFinType, dataSource, "Code", "Value", false, "Code");
    }

    /// <summary>
    /// Load DropDownList Section Code
    /// </summary>
    private void LoadDropdownSectionCode()
    {
      string commandText = "SELECT Code, NameEN FROM VBOMSectionForFinishingInfo ORDER BY Code";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDropDownSectionCode.DataSource = dtSource;
      ultraDropDownSectionCode.ValueMember = "Code";
      ultraDropDownSectionCode.DisplayMember = "Code";
      ultraDropDownSectionCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDropDownSectionCode.DisplayLayout.Bands[0].Columns["Code"].Width = 100;
      ultraDropDownSectionCode.DisplayLayout.Bands[0].Columns["NameEN"].Width = 250;
    }

    /// <summary>
    /// Load DropDownList Material Code
    /// </summary>
    private void LoadDropdownMaterial()
    {
      string commandText = string.Format(@" SELECT MaterialCode, MaterialName, MaterialNameVn, Unit FROM VBOMMaterialsForFinishingInfo ORDER BY MaterialCode ");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpMaterialsCode.DataSource = dtSource;
      udrpMaterialsCode.ValueMember = "MaterialCode";
      udrpMaterialsCode.DisplayMember = "MaterialCode";
      udrpMaterialsCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpMaterialsCode.DisplayLayout.Bands[0].Columns["Unit"].Hidden = true;
      udrpMaterialsCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Width = 100;
      udrpMaterialsCode.DisplayLayout.Bands[0].Columns["MaterialName"].Width = 300;
      udrpMaterialsCode.DisplayLayout.Bands[0].Columns["MaterialNameVn"].Width = 300;
    }

    /// <summary>
    /// Load Customer
    /// </summary>
    private void LoadCustomer()
    {
      string commandText = string.Format(@" SELECT Pid [Value], CustomerCode + ' - ' + [Name] Display
                            FROM TblCSDCustomerInfo
                            ORDER BY CustomerCode ");
      DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCustomer, dtCustomer, "Value", "Display", false, "Value");
    }

    /// <summary>
    /// Load last carcass process
    /// </summary>
    private void LoadLastCarcassProcess()
    {
      string commandText = string.Format(@" SELECT Code [Value],  [Value] Display
                                        FROM TblBOMCodeMaster 
                                        WHERE [Group] = 240221 ");
      DataTable dtLastCarcassProcess = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbLastCarcassProcess, dtLastCarcassProcess, "Value", "Display", false, "Value");
    }


    private void LoadData()
    {
      this.loadingData = true;
      bool confirm = false;
      if (this.finishingCode.Length > 0)
      {
        string cmd = string.Format(@"SELECT FinCode, [Name], NameVN, [Description], Confirm, DeleteFlag, 
	                                        CreateBy, CreateDate, UpdateBy, UpdateDate, Waste, ISNULL(BrassStyle, 0) BrassStyle, SheenLevel, 
	                                        SuffixCode, CustomerPid, FinishingType, LastCarcassProcessID
                                            FROM TblBOMFinishingInfo A
                                            WHERE FinCode = '{0}'", this.finishingCode);
        DataTable dtFinCode = DataBaseAccess.SearchCommandTextDataTable(cmd);

        txtFinishingCode.ReadOnly = true;
        txtFinishingCode.Text = this.finishingCode;
        ultraCBFinishing.Value = this.finishingCode;
        //BOMFinishingInfo obj = new BOMFinishingInfo();
        //obj.FinCode = this.finishingCode;
        //obj = (BOMFinishingInfo)DataBaseAccess.LoadObject(obj, new string[] { "FinCode" });

        if (dtFinCode == null)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0007");
          this.CloseTab();
          return;
        }
        txtNameEN.Text = dtFinCode.Rows[0]["Name"].ToString();
        txtNameVN.Text = dtFinCode.Rows[0]["NameVN"].ToString();
        ultcbSheen.Value = dtFinCode.Rows[0]["SheenLevel"].ToString();
        txtSuffix.Text = dtFinCode.Rows[0]["SuffixCode"].ToString();
        txtWaste.Text = DBConvert.ParseString(dtFinCode.Rows[0]["Waste"].ToString());
        chkBrassStyle.Checked = (DBConvert.ParseInt(dtFinCode.Rows[0]["BrassStyle"].ToString()) == 1);
        confirm = (DBConvert.ParseInt(dtFinCode.Rows[0]["Confirm"].ToString()) == 1);
        if (DBConvert.ParseInt(dtFinCode.Rows[0]["LastCarcassProcessID"].ToString()) > 0)
        {
          ucbLastCarcassProcess.Value = DBConvert.ParseInt(dtFinCode.Rows[0]["LastCarcassProcessID"].ToString());
        }
        if (DBConvert.ParseInt(dtFinCode.Rows[0]["CustomerPid"].ToString()) > 0)
        {
          ucbCustomer.Value = DBConvert.ParseInt(dtFinCode.Rows[0]["CustomerPid"].ToString());
        }
        if (DBConvert.ParseInt(dtFinCode.Rows[0]["FinishingType"].ToString()) > 0)
        {
          ucbFinType.Value = DBConvert.ParseInt(dtFinCode.Rows[0]["FinishingType"].ToString());
        }
        chkLock.Checked = confirm;
        if (confirm)
        {
          chkLock.Visible = false;
          btnSave.Enabled = false;
          lbConfirm.Visible = false;
          btnPrintFinishingInfo.Enabled = true;
        }
        else
        {
          chkLock.Visible = true;
          btnSave.Enabled = true;
          lbConfirm.Visible = true;
          btnPrintFinishingInfo.Enabled = false;
        }
        //if (confirm == 1)
        //{
        //    btnPrintFinishingInfo.Enabled = true;
        //}
        //else
        //{
        //    btnPrintFinishingInfo.Enabled = false;
        //}
      }
      this.canUpdate = ((!confirm) && (btnSave.Visible));
      if (!this.canUpdate)
      {
        txtNameEN.ReadOnly = true;
        txtNameVN.ReadOnly = true;
        txtWaste.ReadOnly = true;
        chkBrassStyle.Enabled = false;
        multicmbFinCode.Enabled = false;
        ucbLastCarcassProcess.ReadOnly = true;
        ucbCustomer.ReadOnly = true;
        txtSuffix.ReadOnly = true;
      }
      else
      {
        txtNameEN.ReadOnly = false;
        txtNameVN.ReadOnly = false;
        txtWaste.ReadOnly = false;
        chkBrassStyle.Enabled = true;
        multicmbFinCode.Enabled = true;
        ucbLastCarcassProcess.ReadOnly = false;
        ucbCustomer.ReadOnly = false;
        txtSuffix.ReadOnly = false;
      }
      this.LoadGrid();
      this.LoadDirectlabour();
      this.loadingData = false;
      this.NeedToSave = false;
    }

    private void LoadGrid()
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@FINCode", DbType.AnsiString, 16, this.finishingCode.Trim()) };
      this.dataSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListFinishingDetailByFinCode", inputParam);
      ultFinishingDetail.DataSource = this.dataSource;
    }

    private void LoadCBSheenLevel()
    {
      DataTable dtSourceReport = new DataTable();
      dtSourceReport.Columns.Add("value", typeof(System.String));
      dtSourceReport.Columns.Add("text", typeof(System.String));

      DataRow row2 = dtSourceReport.NewRow();
      row2["value"] = "10-20";
      row2["text"] = "10-20";
      dtSourceReport.Rows.Add(row2);

      DataRow row3 = dtSourceReport.NewRow();
      row3["Value"] = "20-30";
      row3["Text"] = "20-30";
      dtSourceReport.Rows.Add(row3);

      DataRow row4 = dtSourceReport.NewRow();
      row4["value"] = "30-40";
      row4["text"] = "30-40";
      dtSourceReport.Rows.Add(row4);

      DataRow row5 = dtSourceReport.NewRow();
      row5["Value"] = "40-50";
      row5["Text"] = "40-50";
      dtSourceReport.Rows.Add(row5);

      DataRow row6 = dtSourceReport.NewRow();
      row6["Value"] = "50-60";
      row6["Text"] = "50-60";
      dtSourceReport.Rows.Add(row6);

      DataRow row7 = dtSourceReport.NewRow();
      row7["Value"] = "60-70";
      row7["Text"] = "60-70";
      dtSourceReport.Rows.Add(row7);

      DataRow row8 = dtSourceReport.NewRow();
      row8["Value"] = "70-80";
      row8["Text"] = "70-80";
      dtSourceReport.Rows.Add(row8);

      DataRow row9 = dtSourceReport.NewRow();
      row9["Value"] = "80-90";
      row9["Text"] = "80-90";
      dtSourceReport.Rows.Add(row9);

      DataRow row10 = dtSourceReport.NewRow();
      row10["Value"] = "90-100";
      row10["Text"] = "90-100";
      dtSourceReport.Rows.Add(row10);

      Utility.LoadUltraCombo(ultcbSheen, dtSourceReport, "value", "text", "value");
      ultcbSheen.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Grid Direct Labour
    /// </summary>
    private void LoadDirectlabour()
    {
      this.listDirectlabourDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@FinishingCode", DbType.AnsiString, 16, this.finishingCode) };
      dataSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMFinishingDirectLabour_Select", inputParam);
      this.dataSource.PrimaryKey = new DataColumn[] { this.dataSource.Columns["SectionCode"] };
      ultDirectlabour.DataSource = dataSource;
    }

    /// <summary>
    /// Load ultra combo Finishing
    /// </summary>
    /// <param name="cmbFinishingCode"></param>
    /// <param name="condition"></param>
    private void LoadUltraCBFinishing()
    {
      string commandText = "SELECT FinCode, Name FROM TblBOMFinishingInfo";
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBFinishing.DataSource = dataSource;
      ultraCBFinishing.ValueMember = "FinCode";
      ultraCBFinishing.DisplayMember = "FinCode";
    }
    #endregion Init Method

    #region Process Method

    private bool CheckIsValid(out string message)
    {
      message = string.Empty;
      // Finishing Code
      string text = txtFinishingCode.Text.Trim();
      if (text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The Finishing code");
        return false;
      }
      if (string.Compare(text, this.finishingCode.Trim(), true) != 0)
      {
        BOMFinishingInfo finishing = new BOMFinishingInfo();
        finishing.FinCode = text;
        finishing = (BOMFinishingInfo)DataBaseAccess.LoadObject(finishing, new string[] { "FinCode" });
        if (finishing != null)
        {
          message = string.Format(FunctionUtility.GetMessage("MSG0006"), "The Finishing code");
          return false;
        }
      }
      // Finishing Name
      text = txtNameEN.Text.Trim();
      if (text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The English Name");
        return false;
      }
      text = txtNameVN.Text.Trim();
      if (text.Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The Vietnamese Name");
        return false;
      }
      // FinishingDetail
      int count = ultFinishingDetail.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        text = ultFinishingDetail.Rows[i].Cells["MaterialCode"].Value.ToString().Trim();
        if (text.Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The MaterialCode");
          return false;
        }
        double qty = DBConvert.ParseDouble(ultFinishingDetail.Rows[i].Cells["Qty"].Value.ToString().Trim());
        if (qty < 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Finishing Qty");
          return false;
        }
      }

      // Direct Labour
      int countDirect = ultDirectlabour.Rows.Count;
      for (int i = 0; i < countDirect; i++)
      {
        double qty = DBConvert.ParseDouble(ultDirectlabour.Rows[i].Cells["Qty"].Value.ToString().Trim());
        if (qty <= 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Direct Labour Qty");
          return false;
        }
      }
      return true;
    }

    private bool SaveData(out string code)
    {
      DBParameter[] inputParam = new DBParameter[14];
      bool result = true;
      string storeName = string.Empty;
      code = string.Empty;
      string text = string.Empty;
      // 1 . FinishingInfo
      if (this.finishingCode.Length > 0) // Update
      {
        inputParam[0] = new DBParameter("@NewFinCode", DbType.AnsiString, 16, txtFinishingCode.Text.Trim());
        inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        inputParam[7] = new DBParameter("@OldFinCode", DbType.AnsiString, 16, this.finishingCode.Trim());
        storeName = "spBOMFinishingInfo_Update";
      }
      else // Insert
      {
        inputParam[0] = new DBParameter("@FinCode", DbType.AnsiString, 16, txtFinishingCode.Text.Trim());
        inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = "spBOMFinishingInfo_Insert";
      }
      string name = txtNameEN.Text.Trim();
      inputParam[1] = new DBParameter("@Name", DbType.AnsiString, 128, name);
      name = txtNameVN.Text.Trim();
      if (name.Length > 0)
      {
        inputParam[2] = new DBParameter("@NameVN", DbType.String, 128, name);
      }
      int value = (chkLock.Checked) ? 1 : 0;
      inputParam[3] = new DBParameter("@Confirm", DbType.Int32, value);
      inputParam[4] = new DBParameter("@DeleteFlag", DbType.Int32, 0);
      double waste = DBConvert.ParseDouble(txtWaste.Text);
      if (waste != double.MinValue)
      {
        inputParam[5] = new DBParameter("@Waste", DbType.Double, DBConvert.ParseDouble(txtWaste.Text));
      }
      value = (chkBrassStyle.Checked) ? 1 : 0;
      inputParam[8] = new DBParameter("@BrassStyle", DbType.Int32, value);

      if (ultcbSheen.Value != null)
      {
        inputParam[9] = new DBParameter("@SheenLevel", DbType.String, ultcbSheen.Value.ToString());
      }

      if (ucbFinType.Value != null)
      {
        inputParam[10] = new DBParameter("@FinishingType", DbType.Int32, DBConvert.ParseInt(ucbFinType.Value.ToString()));
      }

      if (txtSuffix.Text.Trim().Length > 0)
      {
        inputParam[11] = new DBParameter("@Suffix", DbType.String, txtSuffix.Text.Trim().ToString());
      }

      if (ucbLastCarcassProcess.Value != null)
      {
        inputParam[12] = new DBParameter("@LastCarcassProcess", DbType.Int32, DBConvert.ParseInt(ucbLastCarcassProcess.Value.ToString()));
      }

      if (ucbCustomer.Value != null)
      {
        inputParam[13] = new DBParameter("@Customer", DbType.Int32, DBConvert.ParseInt(ucbCustomer.Value.ToString()));
      }

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      code = outputParam[0].Value.ToString().Trim();
      if (code.Length == 0)
      {
        return false;
      }
      else
      {
        // Update follow new finishing code
        if ((this.finishingCode.Length > 0) && (string.Compare(txtFinishingCode.Text.Trim(), this.finishingCode, true) != 0))
        {
          string commandTextUpdateMaterial = string.Format(@"Update TblBOMFinishingDetail 
                                                            Set FinCode = '{0}', UpdateBy = {1}, UpdateDate = '{2}' 
                                                            Where FinCode = '{3}'", code, Shared.Utility.SharedObject.UserInfo.UserPid, DateTime.Now, this.finishingCode);
          if (!DataBaseAccess.ExecuteCommandText(commandTextUpdateMaterial))
          {
            return false;
          }
          string commandTextUpdateDirectLabour = string.Format(@"Update TblBOMFinishingDirectLabour 
                                                            Set FinishingCode = '{0}', UpdateBy = {1}, UpdateDate = '{2}' 
                                                            Where FinishingCode = '{3}'", code, Shared.Utility.SharedObject.UserInfo.UserPid, DateTime.Now, this.finishingCode);
          if (!DataBaseAccess.ExecuteCommandText(commandTextUpdateDirectLabour))
          {
            return false;
          }
        }
      }

      // 2. FinishingDetail
      // 2.1 Insert/Update
      long outputValue = 0;
      this.dataSource = (DataTable)ultFinishingDetail.DataSource;
      foreach (DataRow drRow in this.dataSource.Rows)
      {
        storeName = string.Empty;
        if (drRow.RowState == DataRowState.Added)
        {
          storeName = "spBOMFinishingDetail_Insert";
        }
        else if (drRow.RowState == DataRowState.Modified)
        {
          storeName = "spBOMFinishingDetail_Update";
        }
        if (storeName.Length > 0)
        {
          DBParameter[] inputParamDetail = this.GetParamater(drRow, code);
          DBParameter[] outputParamDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamDetail, outputParamDetail);
          outputValue = DBConvert.ParseLong(outputParamDetail[0].Value.ToString());
          if (outputValue == 0)
          {
            result = false;
          }
        }
      }

      //2.2 Delete
      foreach (long pid in this.listDeletedPid)
      {
        DBParameter[] inputParamDelete = new DBParameter[1];
        inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, pid);
        DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMFinishingDetail_Delete", inputParamDelete, OutputParamDelete);
        outputValue = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
        if (outputValue == 0)
        {
          result = false;
        }
      }
      //3.TblBOMFinishingDirectLabour
      //3.1 Delete
      foreach (long deletePid in this.listDirectlabourDeletedPid)
      {
        DBParameter[] inputParamDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, deletePid) };
        DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMFinishingDirectLabour_Delete", inputParamDelete, OutputParamDelete);
        outputValue = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
        if (outputValue <= 0)
        {
          result = false;
        }
      }
      //3.2.Insert, update
      this.dataSource = (DataTable)ultDirectlabour.DataSource;
      foreach (DataRow drRow in dataSource.Rows)
      {
        DBParameter[] param = new DBParameter[7];
        storeName = string.Empty;
        if (drRow.RowState == DataRowState.Added)
        {
          storeName = "spBOMFinishingDirectLabour_Insert";
          param[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        else if (drRow.RowState == DataRowState.Modified)
        {
          storeName = "spBOMFinishingDirectLabour_Update";
          long detailPid = DBConvert.ParseLong(drRow["Pid"].ToString());
          param[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
          param[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        param[1] = new DBParameter("@FinishingCode", DbType.AnsiString, 16, code);
        if (storeName.Length > 0)
        {
          text = drRow["SectionCode"].ToString().Trim();
          if (text.Length > 0)
          {
            param[2] = new DBParameter("@SectionCode", DbType.AnsiString, 50, text);
          }
          double qty = DBConvert.ParseDouble(drRow["Qty"].ToString());
          if (qty != double.MinValue)
          {
            param[3] = new DBParameter("@Qty", DbType.Double, qty);
          }
          text = drRow["Description"].ToString().Trim();
          if (text.Length > 0)
          {
            param[4] = new DBParameter("@Description", DbType.String, 512, text);
          }
          text = drRow["Remark"].ToString().Trim();
          if (text.Length > 0)
          {
            param[5] = new DBParameter("@Remark", DbType.String, 512, text);
          }
          DBParameter[] outputParamDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, param, outputParamDetail);
          outputValue = DBConvert.ParseLong(outputParamDetail[0].Value.ToString());
          if (outputValue <= 0)
          {
            result = false;
          }
        }
      }
      return result;
    }

    private DBParameter[] GetParamater(DataRow drRow, string finCode)
    {
      DBParameter[] param = new DBParameter[8];
      long pid = DBConvert.ParseLong(drRow["Pid"].ToString());
      if (pid != long.MinValue)
      {
        param[0] = new DBParameter("@Pid", DbType.Int32, DBConvert.ParseInt(drRow["PID"].ToString()));
        param[7] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }
      else
      {
        param[7] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }
      param[1] = new DBParameter("@FinCode", DbType.AnsiString, 16, finCode);

      string text = drRow["MaterialCode"].ToString().Trim();
      param[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, text);
      double qty = DBConvert.ParseDouble(drRow["Qty"].ToString());
      if (qty != double.MinValue)
      {
        param[3] = new DBParameter("@Qty", DbType.Double, qty);
      }
      text = drRow["Remark"].ToString().Trim();
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@Remark", DbType.String, 128, text);
      }
      param[5] = new DBParameter("@InCompany", DbType.Int32, 1);
      param[6] = new DBParameter("@ContractOut", DbType.Int32, 0);
      return param;
    }

    #endregion Process Method

    #region Button CLick

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      string code = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarningFromText(message);
        return;
      }
      success = this.SaveData(out code);
      if (success)
      {
        this.finishingCode = code;
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadData();
      }
      else
      {
        FunctionUtility.UnlockBOMFinishingInfo(this.finishingCode);
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      string message = string.Empty;
      string code = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarningFromText(message);
        this.SaveSuccess = false;
        return;
      }
      success = this.SaveData(out code);
      if (success)
      {
        this.finishingCode = code;
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadData();
        this.SaveSuccess = true;
      }
      else
      {
        FunctionUtility.UnlockBOMFinishingInfo(this.finishingCode);
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        this.SaveSuccess = false;
      }
    }

    private void Object_TextChanged(object sender, EventArgs e)
    {
      if (this.loadingData)
      {
        return;
      }
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
      bool success = true;
      string code = string.Empty;
      if (this.NeedToSave || this.finishingCode.Length == 0)
      {
        string message = string.Empty;
        success = this.CheckIsValid(out message);
        if (!success)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarningFromText(message);
          return;
        }
        success = this.SaveData(out code);
      }
      DBParameter[] inputParams = new DBParameter[3];
      inputParams[0] = new DBParameter("@FinCode", DbType.AnsiString, 16, Utility.GetSelectedValueMultiCombobox(multicmbFinCode));
      inputParams[1] = new DBParameter("@NewFinCode", DbType.AnsiString, 16, txtFinishingCode.Text.Trim());
      inputParams[2] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);
      DataBaseAccess.ExecuteStoreProcedure("spBOMCopyFromReferenceFinishing", inputParams);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0022");
        this.finishingCode = txtFinishingCode.Text.Trim();
      }
      else
      {
        FunctionUtility.UnlockBOMFinishingInfo(this.finishingCode);
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadGrid();
      btnCopy.Enabled = false;
    }

    private void multicmbFinCode_SelectedIndexChanged(object sender, EventArgs e)
    {
      string selectedValue = Utility.GetSelectedValueMultiCombobox(multicmbFinCode);
      btnCopy.Enabled = (selectedValue.Length > 0);
    }
    #endregion Button CLick

    #region UtraGridView Handle Event

    private void ultFinishingDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      string message = string.Empty;
      string columnName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "materialcode":
          try
          {
            ultFinishingDetail.Rows[index].Cells["MaterialName"].Value = udrpMaterialsCode.SelectedRow.Cells["MaterialName"].Value;
          }
          catch
          {
            ultFinishingDetail.Rows[index].Cells["MaterialName"].Value = DBNull.Value;
          }
          try
          {
            ultFinishingDetail.Rows[index].Cells["Unit"].Value = udrpMaterialsCode.SelectedRow.Cells["Unit"].Value;
          }
          catch
          {
            ultFinishingDetail.Rows[index].Cells["Unit"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    private void ultFinishingDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "materialcode":
          string code = e.Cell.Text.Trim();
          bool isCode = FunctionUtility.CheckBOMMaterialCode(code, 6);
          if (!isCode)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        case "qty":
          double dQty = DBConvert.ParseDouble(e.Cell.Text);
          if (dQty <= 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "The Finishing Qty");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultFinishingDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    private void ultFinishingDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    private void btnPrintFinishingInfo_Click(object sender, EventArgs e)
    {
      View_BOM_0018_ItemMaterialReport view = new View_BOM_0018_ItemMaterialReport();
      view.code = txtFinishingCode.Text;
      view.ncategory = 12;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void ultDirectlabour_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      string columnName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "sectioncode":
          try
          {
            ultDirectlabour.Rows[index].Cells["NameEN"].Value = ultraDropDownSectionCode.SelectedRow.Cells["NameEN"].Value;
          }
          catch
          {
            ultDirectlabour.Rows[index].Cells["NameEN"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    private void ultDirectlabour_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      foreach (long pid in this.listDirectlabourDeletingPid)
      {
        this.listDirectlabourDeletedPid.Add(pid);
      }
    }

    private void ultDirectlabour_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (e.Cell.Text.ToString().Trim().Length > 0)
      {
        string strColName = e.Cell.Column.ToString();
        if (string.Compare(strColName, "SectionCode", true) == 0)
        {
          string sectionCodeCheck = e.Cell.Text.ToString().Trim();
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@SectionCodeCheck", DbType.String, sectionCodeCheck);
          string commandText = "Select COUNT(*) From VBOMSection Where Code = @SectionCodeCheck ";
          object obj = DataBaseAccess.ExecuteScalarCommandText(commandText, inputParam);
          int count = (int)obj;
          if (count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "SectionCode");
            e.Cancel = true;
          }
        }
        switch (strColName)
        {
          case "Qty":
            double qty = DBConvert.ParseDouble(e.Cell.Text);
            if (qty <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Direct Labour Qty");
              e.Cancel = true;
            }
            break;
          default:
            break;
        }
      }
    }

    private void ultDirectlabour_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDirectlabourDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long Pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (Pid != long.MinValue)
        {
          this.listDirectlabourDeletingPid.Add(Pid);
        }
      }
    }

    private void ultraCBFinishing_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["FinCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["FinCode"].MaxWidth = 100;
    }

    private void ultraCBFinishing_ValueChanged(object sender, EventArgs e)
    {
      if (ultraCBFinishing.SelectedRow != null)
      {
        string finishingCode = ultraCBFinishing.Value.ToString();
        this.finishingCode = finishingCode;
        this.LoadData();
      }
    }

    private void ultDirectlabour_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.InitLayout_UltraGrid(ultFinishingDetail);
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["SectionCode"].ValueList = ultraDropDownSectionCode;
      e.Layout.Bands[0].Columns["SectionCode"].Header.Caption = "Section Code";
      e.Layout.Bands[0].Columns["SectionCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["SectionCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 80;
      e.Layout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      if (!this.canUpdate)
      {
        int count = ultDirectlabour.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          ultDirectlabour.Rows[i].Activation = Activation.ActivateOnly;
        }
      }
    }

    private void ultFinishingDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.InitLayout_UltraGrid(ultFinishingDetail);
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = udrpMaterialsCode;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 150;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 80;
      e.Layout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      if (!this.canUpdate)
      {
        int count = ultFinishingDetail.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          ultFinishingDetail.Rows[i].Activation = Activation.ActivateOnly;
        }
      }
    }

    private void btnFinProcess_Click(object sender, EventArgs e)
    {
      this.NeedToSave = true;
      DataTable dtSource = (DataTable)ultFinishingDetail.DataSource;

      DataTable dtProcess = new DataTable();
      if (finishingCode.Trim().Length > 0)
      {
        string cm = string.Format(@"SELECT COL.MaterialCode, MA.MaterialName, MA.MaterialNameVn, MA.Unit, ROUND(SUM((CASE WHEN ISNULL(RA.Qty, 0) = 0 THEN 0 ELSE COL.Qty END) / RA.Qty * PRO.ConsumptionPerM2), 4) Qty
                                            FROM TblBOMFinishingProcess PRO
	                                            INNER JOIN TblBOMColorDetail COL ON PRO.ColorPid = COL.ColorPid
	                                            LEFT JOIN
												(
													SELECT PRO.Step, PRO.FinCode, SUM(ISNULL(Qty, 0)) Qty
													FROM TblBOMFinishingProcess PRO
														LEFT JOIN TblBOMColorDetail DT ON DT.ColorPid = PRO.ColorPid
													GROUP BY PRO.Step, PRO.FinCode
												) RA ON PRO.Step = RA.Step AND PRO.FinCode = RA.FinCode
	                                            LEFT JOIN VBOMMaterialsForFinishingInfo MA ON MA.MaterialCode = COL.MaterialCode
                                            WHERE PRO.FinCode = '{0}'
                                            GROUP BY COL.MaterialCode, MA.MaterialName, MA.MaterialNameVn, MA.Unit", this.finishingCode);
        dtProcess = DataBaseAccess.SearchCommandTextDataTable(cm);

      }
      if (dtProcess != null && dtProcess.Rows.Count > 0)
      {
        dtSource.Clear();
        foreach (DataRow drRow in dtProcess.Rows)
        {
          DataRow row = dtSource.NewRow();
          row["MaterialCode"] = drRow["MaterialCode"];
          row["MaterialName"] = drRow["MaterialName"];
          row["Unit"] = drRow["Unit"];
          row["Qty"] = drRow["Qty"];
          dtSource.Rows.Add(row);
        }
      }
    }
    #endregion UtraGridView Handle Event

  }
}