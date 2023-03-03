using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_08_005 : MainUserControl
  {
    public long finishingProceesPid;
    public string finishingCode;    
    private IList listDeletedPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private IList listDirectlabourDeletedPid = new ArrayList();
    private IList listDirectlabourDeletingPid = new ArrayList();
    private bool loadingData = false;
    private bool canUpdate = false;       
    public viewBOM_08_005()
    {
      InitializeComponent();
    }

    private void viewBOM_08_005_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + Shared.Utility.SharedObject.UserInfo.UserName + " | " + Shared.Utility.SharedObject.UserInfo.LoginDate;      
      this.LoadUltraCBFinishingType();
      this.LoadUltraDDChemical();      
      this.LoadUltraCBFinishingCode();
      this.LoadLastCarcassProcess();
      this.LoadCBCustomer();
      this.LoadCBSupplier();
      this.LoadData();
    }

    #region Init Method

    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@FinishingProcessPid", DbType.Int64, this.finishingProceesPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMFinishingProcess_LoadData", inputParam);
      if (dsSource.Tables[0].Rows.Count > 0)
      {
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          ucbFinCode.Value = row["FinCode"].ToString();
          txtProcessCode.Text = row["ProcessCode"].ToString();
          long supplierPid = DBConvert.ParseLong(row["SupplierPid"].ToString());
          long customerPid = DBConvert.ParseLong(row["CustomerPid"].ToString());
          if (supplierPid > 0)
          {
            ucbSupplier.Value = supplierPid;
          }  
          else
          {
            ucbSupplier.Value = null;
          }  
          if(customerPid > 0)
          {
            ucbCustomer.Value = customerPid;
          }  
          else
          {
            ucbCustomer.Value = null;
          }  

          txtDescription.Text = row["Description"].ToString();
          unePrice.Value = DBConvert.ParseDouble(row["Price"].ToString());
          if (DBConvert.ParseInt(row["ProcessConfirm"].ToString()) == 0)
          {
            chkLock.Checked = false;            
          }
          else
          {
            chkLock.Checked = true;            
          }
          uceIsDefault.Checked = (bool)row["IsDefault"];
        }
      }
      else
      {
        DataTable dtPR = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FBOMFinishingProcessCode() NewProcess");
        if ((dtPR != null) && (dtPR.Rows.Count > 0))
        {
          txtProcessCode.Text = dtPR.Rows[0]["NewProcess"].ToString();
        }
        uceIsDefault.Checked = true;
      }

      if (dsSource.Tables.Count >= 3)
      {
        DataSet ds = new DataSet();
        ds.Tables.Add(Utility.CloneTable(dsSource.Tables[1]));
        ds.Tables.Add(Utility.CloneTable(dsSource.Tables[2]));
        ds.Relations.Add(new DataRelation("ChemicalMaterial", new DataColumn[1] { ds.Tables[0].Columns["ChemicalPid"] }, new DataColumn[1] { ds.Tables[1].Columns["ChemicalPid"] }, false));
        ultFinishingDetail.DataSource = ds;
      }
     
      this.SetStatusControl();
    }

    private void SetStatusControl()
    {
      if (chkLock.Checked)
      {
        ucbFinCode.ReadOnly = true;
        chkLock.Enabled = false;
        btnSave.Enabled = false;
        for (int i = 0; i < ultFinishingDetail.Rows.Count; i++)
        {
          ultFinishingDetail.Rows[i].Activation = Activation.ActivateOnly;
        }       
      }
      else
      {
        ucbFinCode.ReadOnly = false;
        chkLock.Enabled = true;
        btnSave.Enabled = true;
        for (int i = 0; i < ultFinishingDetail.Rows.Count; i++)
        {
          ultFinishingDetail.Rows[i].Activation = Activation.AllowEdit;
        }
      }

    }

    /// <summary>
    /// Load customer
    /// </summary>
    private void LoadCBCustomer()
    {
      string commandText = @"SELECT Pid, CustomerCode, [Name], (CustomerCode + ' | ' + [Name]) Customer
                            FROM TblCSDCustomerInfo A
                            WHERE DeletedFlg = 0
                            ORDER BY CustomerCode";
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCustomer, dataSource, "Pid", "Customer", false, new string[] { "Pid", "Customer" });
      ucbCustomer.DisplayLayout.Bands[0].Columns["CustomerCode"].Width = 70;
    }


    /// <summary>
    /// Load Supplier
    /// </summary>
    private void LoadCBSupplier()
    {
      string commandText = @"SELECT Pid, SupplierCode, VietnameseName, (SupplierCode + ' | ' + VietnameseName) Supplier
                            FROM TblPURSupplierInfo
                            WHERE DeleteFlg = 0
                            ORDER BY SupplierCode";
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbSupplier, dataSource, "Pid", "Supplier", false, new string[] { "Pid", "Supplier" });
      ucbSupplier.DisplayLayout.Bands[0].Columns["SupplierCode"].Width = 70;
    }

    private void LoadUltraDDChemical()
    {
      string commandText = "SELECT Pid, ColorCode, Name, (ColorCode + ' | ' + Name) DisplayText FROM TblBOMColor";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(udrpChemical, dtSource, "Pid", "DisplayText", false, new string[] { "Pid", "DisplayText" });
      udrpChemical.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    /// <summary>
    /// Load ultra combo Finishing
    /// </summary>
    /// <param name="cmbFinishingCode"></param>
    /// <param name="condition"></param>
    private void LoadUltraCBFinishingType()
    {
      string commandText = @"SELECT Code, Value
                              FROM TblBOMCodeMaster 
                              WHERE [Group] = 2917";
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbFinishingType, dataSource, "Code", "Value", "Code");
      ucbFinishingType.DisplayLayout.Bands[0].ColHeadersVisible = false;
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
      Utility.LoadUltraCombo(ucbLastProcessCode, dtLastCarcassProcess, "Value", "Display", false, "Value");
    }


    /// <summary>
    /// Load ultra combo Finishing code
    /// </summary>
    /// <param name="cmbFinishingCode"></param>
    /// <param name="condition"></param>
    private void LoadUltraCBFinishingCode()
    {
      // Finishing List
      string commandText = @"SELECT FinCode FinishingCode, NameVN, Waste, SheenLevel, 
	                            LastCarcassProcessID, FinishingType, (FinCode + ' | ' + NameVN) DisplayText
                            FROM TblBOMFinishingInfo 
                            ORDER BY FinCode";
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbFinCode, dataSource, "FinishingCode", "DisplayText", false, new string[] { "Waste", "SheenLevel", "LastCarcassProcessID", "FinishingType", "DisplayText" });

      // Process list of finishing for copying
      commandText = @"SELECT MAST.Pid, MAST.ProcessCode, FIN.FinCode, FIN.NameVN FinName, SUP.VietnameseName SupplerName,
	                      (MAST.ProcessCode + ' | ' + FIN.FinCode + ' | ' + FIN.NameVN + ' | ' + SUP.VietnameseName) DisplayText
                      FROM TblBOMFinishingProcessMaster MAST
                        INNER JOIN TblBOMFinishingInfo FIN ON MAST.FinishingCode = FIN.FinCode
                        LEFT JOIN TblPURSupplierInfo SUP ON MAST.SupplierPid = SUP.Pid
                      ORDER BY ProcessCode";
      dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultcbFinCopy, dataSource, "Pid", "DisplayText", true, new string[] { "Pid", "DisplayText" });
      ultcbFinCopy.DisplayLayout.Bands[0].Columns["ProcessCode"].Header.Caption = "Mã qui trình";
      ultcbFinCopy.DisplayLayout.Bands[0].Columns["FinCode"].Header.Caption = "Mã màu";
      ultcbFinCopy.DisplayLayout.Bands[0].Columns["FinName"].Header.Caption = "Tên màu";
      ultcbFinCopy.DisplayLayout.Bands[0].Columns["SupplerName"].Header.Caption = "Nhà cung cấp";
      ultcbFinCopy.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }
    #endregion Init Method

    #region Process Method

    private bool CheckIsValid(out string message)
    {
      message = string.Empty;
      // Finishing Code
      if (ucbFinCode.SelectedRow == null)
      {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), lblFinishing.Text);
        return false;
      }

      // Check Detail
      foreach(UltraGridRow row in ultFinishingDetail.Rows)
      {
        if(row.Cells["Step"].Value.ToString().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("MSG0005"), row.Cells["Step"].Column.Header.Caption);
          return false;
        }
        if (row.Cells["ProcessDescription"].Value.ToString().Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("MSG0005"), row.Cells["ProcessDescription"].Column.Header.Caption);
          return false;
        }
      }  

      return true;
    }

    private bool SaveData(out string code)
    {
      code = string.Empty;
      //INSERT FINISHING INFO
      SqlDBParameter[] inputParamInfo = new SqlDBParameter[9];
      string storeNameInfo = string.Empty;
      string codeInfo = string.Empty;
      storeNameInfo = "spBOMFinishingProcessMaster_Save";
      if (this.finishingProceesPid != long.MinValue)
      {
        inputParamInfo[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.finishingProceesPid);
      }
      inputParamInfo[1] = new SqlDBParameter("@Status", SqlDbType.Int, chkLock.Checked ? 1 : 0);
      inputParamInfo[2] = new SqlDBParameter("@FinCode", SqlDbType.VarChar, ucbFinCode.Value.ToString());
      inputParamInfo[3] = new SqlDBParameter("@Price", SqlDbType.Float, unePrice.Value);
      if (txtDescription.Text.Trim().ToString().Length > 0)
      {
        inputParamInfo[4] = new SqlDBParameter("@Description", SqlDbType.NVarChar, txtDescription.Text.Trim().ToString());
      }
      if (ucbCustomer.SelectedRow != null)
      {
        inputParamInfo[5] = new SqlDBParameter("@Customer", SqlDbType.BigInt, DBConvert.ParseLong(ucbCustomer.Value));
      }
      if (ucbSupplier.SelectedRow != null)
      {
        inputParamInfo[6] = new SqlDBParameter("@Supplier", SqlDbType.BigInt, DBConvert.ParseLong(ucbSupplier.Value));
      }
      inputParamInfo[7] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      inputParamInfo[8] = new SqlDBParameter("@IsDefault", SqlDbType.Bit, uceIsDefault.Checked);
      SqlDBParameter[] outputParamInfo = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue) };
      SqlDataBaseAccess.ExecuteStoreProcedure(storeNameInfo, inputParamInfo, outputParamInfo);
      this.finishingProceesPid = DBConvert.ParseLong(outputParamInfo[0].Value.ToString());
      if (this.finishingProceesPid <= 0)
      {
        return false;
      }
      //END
      this.finishingCode = ucbFinCode.Value.ToString().Trim();
      bool result = true;
      string storeName = string.Empty;
      string text = string.Empty;

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };
      // 2. FinishingDetail
      // 2.1 Insert/Update
      long outputValue = 0;
      DataTable dataSource = (DataTable)((DataSet)ultFinishingDetail.DataSource).Tables[0];
      foreach (DataRow drRow in dataSource.Rows)
      {
        storeName = string.Empty;
        long pid = DBConvert.ParseLong(drRow["Pid"].ToString());
        if (pid < 0)
        {
          storeName = "spBOMFinishingProcess_Insert";
        }
        else
        {
          storeName = "spBOMFinishingProcess_Update";
        }
        if (storeName.Length > 0)
        {
          DBParameter[] inputParamDetail = this.GetParamater(drRow, codeInfo);
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
        DataBaseAccess.ExecuteStoreProcedure("spBOMFinishingProcess_Delete", inputParamDelete, OutputParamDelete);
        outputValue = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
        if (outputValue == 0)
        {
          result = false;
        }
      }
      return result;
    }

    private DBParameter[] GetParamater(DataRow drRow, string finCode)
    {
      DBParameter[] param = new DBParameter[13];
      long pid = DBConvert.ParseLong(drRow["Pid"].ToString());
      if (pid > 0)
      {
        param[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseInt(drRow["PID"].ToString()));
      }

      param[1] = new DBParameter("@FinCode", DbType.String, finCode);

      string text = drRow["ProcessDescription"].ToString().Trim();
      param[2] = new DBParameter("@Description", DbType.String, text);
      double holdTime = DBConvert.ParseDouble(drRow["HoldTime"].ToString());
      int a = DBConvert.ParseInt(drRow["A"]);
      int b = DBConvert.ParseInt(drRow["B"]);
      int c = DBConvert.ParseInt(drRow["C"]);

      if (holdTime != double.MinValue)
      {
        param[3] = new DBParameter("@HoldTime", DbType.Double, holdTime);
      }
      text = drRow["Remark"].ToString().Trim();
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@Remark", DbType.String, text);
      }
      if (DBConvert.ParseInt(drRow["Step"].ToString()) > 0)
      {
        param[5] = new DBParameter("@Step", DbType.Int32, DBConvert.ParseInt(drRow["Step"].ToString()));
      }
      if (DBConvert.ParseLong(drRow["ChemicalPid"].ToString()) > 0)
      {
        param[6] = new DBParameter("@ColorPid", DbType.Int64, DBConvert.ParseLong(drRow["ChemicalPid"].ToString()));
      }
      param[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      double comsumption = DBConvert.ParseDouble(drRow["ConsumptionPerM2"]);
      if (comsumption != double.MinValue)
      {
        param[8] = new DBParameter("@ConsumptionPerM2", DbType.Double, comsumption);
      }
      if (a > 0)
      {
        param[9] = new DBParameter("@A", DbType.Int32, a);
      }
      if (b > 0)
      {
        param[10] = new DBParameter("@B", DbType.Int32, b);
      }
      if (c > 0)
      {
        param[11] = new DBParameter("@C", DbType.Int32, c);
      }
      param[12] = new DBParameter("@FinishingProcessPid", DbType.Int64, this.finishingProceesPid);
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
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      success = this.SaveData(out code);
      if (success)
      {
        //this.finishingCode = txtFin.Text.Trim();
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
    #endregion Button CLick

    #region UtraGridView Handle Event

    private void ultFinishingDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }      
    }

    private void ultFinishingDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
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

    private void ultFinishingDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {      
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["ChemicalPid"].ValueList = udrpChemical;
      e.Layout.Bands[0].Columns["ChemicalPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      e.Layout.Bands[0].Columns["ChemicalPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ChemicalPid"].Hidden = true;           

      for (int j = 0; j < e.Layout.Bands[1].Columns.Count; j++)
      {
        Type colType = e.Layout.Bands[1].Columns[j].DataType;
        e.Layout.Bands[1].Columns[j].CellActivation = Activation.ActivateOnly;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[j].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[1].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Step"].Header.Caption = "Bước";
      e.Layout.Bands[0].Columns["ProcessDescription"].Header.Caption = "Qui trình";
      e.Layout.Bands[0].Columns["ChemicalPid"].Header.Caption = "Hợp chất";      
      e.Layout.Bands[0].Columns["HoldTime"].Header.Caption = "Thời gian chờ\nkhô (phút)";
      e.Layout.Bands[0].Columns["ConsumptionPerM2"].Header.Caption = "Số lượng\n(đơn vị/m2)";
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Mã hóa chất";
      e.Layout.Bands[1].Columns["MaterialName"].Header.Caption = "Tên hóa chất";
      e.Layout.Bands[1].Columns["ConsumptionPerM2"].Header.Caption = "Số lượng\n(đơn vị/m2)";
      e.Layout.Bands[1].Columns["Qty"].Header.Caption = "Tỷ lệ pha";
      e.Layout.Bands[1].Columns["Unit"].Header.Caption = "Đơn vị";

      e.Layout.Bands[0].Columns["A"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["B"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["C"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["A"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["B"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["C"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["A"].MinWidth = 40;
      e.Layout.Bands[0].Columns["A"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["B"].MinWidth = 40;
      e.Layout.Bands[0].Columns["B"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["C"].MinWidth = 40;
      e.Layout.Bands[0].Columns["C"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["Step"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Step"].MaxWidth = 50;

      if (!this.canUpdate)
      {
        int count = ultFinishingDetail.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          ultFinishingDetail.Rows[i].Activation = Activation.ActivateOnly;
        }
      }
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
      if (ultcbFinCopy.SelectedRow != null)
      {
        long finProcessPid = DBConvert.ParseLong(ultcbFinCopy.Value);
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@FinishingProcessPid", DbType.Int64, this.finishingProceesPid) };
        DataTable dtCopy = DataBaseAccess.SearchStoreProcedureDataTable("spBOMFinishingProcess_LoadData", inputParam);
        if (dtCopy != null && dtCopy.Rows.Count > 0)
        {
          // Delete old process
          foreach (UltraGridRow row in ultFinishingDetail.Rows)
          {
            long pid = DBConvert.ParseLong(row.Cells["Pid"].Value);
            if (pid != long.MinValue)
            {
              this.listDeletedPid.Add(pid);
            }
          }
          // Insert copied process
          DataSet dsSource = (DataSet)ultFinishingDetail.DataSource;
          dsSource.Tables[0].Clear();
          dsSource.Tables[0].Merge(dtCopy);          
        }
        else
        {
          WindowUtinity.ShowMessageWarningFromText("Qui trình sơn đang chọn chưa được định nghĩa các bước!");
        }  
      }
    }


    private void ucbFinCode_ValueChanged(object sender, EventArgs e)
    {
      if (ucbFinCode.SelectedRow != null)
      {        
        if (DBConvert.ParseInt(ucbFinCode.SelectedRow.Cells["LastCarcassProcessID"].Value.ToString()) > 0)
        {
          ucbLastProcessCode.Value = DBConvert.ParseInt(ucbFinCode.SelectedRow.Cells["LastCarcassProcessID"].Value.ToString());
        }        
        if (DBConvert.ParseInt(ucbFinCode.SelectedRow.Cells["FinishingType"].Value.ToString()) > 0)
        {
          ucbFinishingType.Value = DBConvert.ParseInt(ucbFinCode.SelectedRow.Cells["FinishingType"].Value.ToString());
        }        
      }
    }

    private void ultFinishingDetail_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      if (e.Cell.Row.ParentRow == null)
      {
        string value = e.Cell.Row.Cells["Step"].Value.ToString();
        if (value.Length == 0)
        {
          DataTable dtComp = ((DataSet)ultFinishingDetail.DataSource).Tables[0];
          int maxNo = 1;
          if (dtComp.Rows.Count > 0)
          {
            maxNo = DBConvert.ParseInt(dtComp.Compute("Max(Step)", "")) + 1;
          }
          e.Cell.Row.Cells["Step"].Value = maxNo;
        }
      }
    }
    #endregion UtraGridView Handle Event
  }
}