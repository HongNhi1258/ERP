using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Technical.DataSetSource;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_01_005 : MainUserControl
  {
    public long componentPid = long.MinValue;
    public bool carConfirm = false;
    private bool loaddingData = false;
    private bool canUpdate = false;
    private bool isUpdateComponent = true;
    private IList listDeleted = new ArrayList();
    private IList listDeleting = new ArrayList();
    private int deletedRowIndex = int.MinValue;
    private bool checkValue = true;
    private string carcassCode = string.Empty;
    private string componentCode = string.Empty;
    private IList listMaterialDeletedPid = new ArrayList();
    private UltraCombo ucbProcessList = new UltraCombo();

    public viewBOM_01_005()
    {
      InitializeComponent();
    }

    private void viewBOM_01_005_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + SharedObject.UserInfo.UserName + " | " + SharedObject.UserInfo.LoginDate;
      this.loaddingData = true;
      Utility.LoadDropDownBOMGroupProcess(ultraCBGroupProcess);
      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(string.Format("Select CarcassCode, [Description] From TblBOMCarcass Where DeleteFlag = 0"));
      Utility.LoadUltraCombo(ultraCBCarcass, dtCarcass, "CarcassCode", "CarcassCode");
      ultraCBCarcass.DisplayLayout.Bands[0].Columns["CarcassCode"].MinWidth = 120;
      ultraCBCarcass.DisplayLayout.Bands[0].Columns["CarcassCode"].MaxWidth = 120;

      this.LoadUltraCBProcessInfo();
      Utility.LoadDropdownProfile(udrpProfile);
      //Specify
      this.LoadDropdownCodeMst(UltraDDSpecify, ConstantClass.GROUP_COMPONENTSPECIFY);
      //Status
      this.LoadDropdownCodeMst(UltraDDStatus, ConstantClass.GROUP_COMPONENTSTATUS);
      // MaterialsCode
      this.LoadDropdownMaterial(udrpMaterialsCode);
      // Alternative
      this.LoadDropdownMaterial(udrpAlternative);

      this.LoadData();
      tableLayoutProcessReference.Visible = false;
      this.loaddingData = false;
      // Load default carcass reference
      try
      {
        ultraCBCarcass.Value = this.carcassCode;
      }
      catch { }
      chkProcessReference.Checked = true;
    }

    #region LoadData
    private void LoadUltraCBProcessInfo()
    {
      string commandText = "Select Pid, (ProcessCode + ' | ' + VNDescription) Process, STUFF((SELECT ', ' + NameEn FROM TblBOMMachine WHERE ('|' + PROCESS.MachineGroup + '|') LIKE ('%|' + CAST(Pid as varchar ) + '|%') FOR XML PATH('')),1,2,'') MachineGroup From TblBOMProcessInfo PROCESS";
      DataTable dtProcessInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbProcessList, dtProcessInfo, "Pid", "Process", "Pid");
      ucbProcessList.DisplayLayout.Bands[0].Columns["Process"].Width = 300;
      ucbProcessList.DisplayLayout.Bands[0].Columns["MachineGroup"].Header.Caption = "Machine Group";
      ucbProcessList.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
    }
    private void SetDefaultDimension(UltraGridRow row)
    {
      string code = row.Cells["MaterialCode"].Value.ToString().Trim().Substring(0, 3);
      UltraGridRow parentRow = row.ParentRow;
      double finishLength = DBConvert.ParseDouble(parentRow.Cells["FIN_Length"].Value.ToString());
      double finishWidth = DBConvert.ParseDouble(parentRow.Cells["FIN_Width"].Value.ToString());
      double finishThickness = DBConvert.ParseDouble(parentRow.Cells["FIN_Thickness"].Value.ToString());
      switch (code)
      {
        case "010":
        case "011":
        case "030":
        case "031":
          if (finishLength != double.MinValue)
          {
            row.Cells["RAW_Length"].Value = finishLength + 20;
          }
          else
          {
            row.Cells["RAW_Length"].Value = DBNull.Value;
          }
          if (finishWidth != double.MinValue)
          {
            row.Cells["RAW_Width"].Value = finishWidth + 20;
          }
          else
          {
            row.Cells["RAW_Width"].Value = DBNull.Value;
          }
          if (finishThickness != double.MinValue)
          {
            row.Cells["RAW_Thickness"].Value = finishThickness + 20;
          }
          else
          {
            row.Cells["RAW_Thickness"].Value = DBNull.Value;
          }
          break;
        case "012":
          if (finishLength != double.MinValue)
          {
            row.Cells["RAW_Length"].Value = finishLength + 40;
          }
          else
          {
            row.Cells["RAW_Length"].Value = DBNull.Value;
          }
          if (finishWidth != double.MinValue)
          {
            row.Cells["RAW_Width"].Value = finishWidth + 8;
          }
          else
          {
            row.Cells["RAW_Width"].Value = DBNull.Value;
          }
          if (finishThickness != double.MinValue)
          {
            row.Cells["RAW_Thickness"].Value = finishThickness + 8;
          }
          else
          {
            row.Cells["RAW_Thickness"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    private void LoadDropdownCodeMst(UltraDropDown udrpDropDown, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value, Description FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpDropDown.DataSource = dtSource;
      udrpDropDown.ValueMember = "Code";
      udrpDropDown.DisplayMember = "Value";
      udrpDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void LoadDropdownMaterial(UltraDropDown udrpMaterials)
    {
      string commandText = @"SELECT MaterialCode, MaterialName, MaterialNameVn, IDFactoryUnit, FactoryUnit 
                            FROM VBOMMaterialsForCarcassComponent WHERE Used = 1 ORDER BY MaterialCode";
      DataTable dtSourceMaterials = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpMaterials.DataSource = dtSourceMaterials;
      udrpMaterials.ValueMember = "MaterialCode";
      udrpMaterials.DisplayMember = "MaterialCode";
      udrpMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpMaterials.DisplayLayout.Bands[0].Columns["IDFactoryUnit"].Hidden = true;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialCode"].MinWidth = 110;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialCode"].MaxWidth = 110;
    }

    private void SetStatusRow(UltraGridRow row, int unitCode)
    {
      if (unitCode == 4)
      { // m
        row.Cells["RAW_Length"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Width"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Thickness"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Width"].Value = DBNull.Value;
        row.Cells["RAW_Thickness"].Value = DBNull.Value;
      }
      else if (unitCode == 6)
      { // sqm
        row.Cells["RAW_Length"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Width"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Thickness"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Thickness"].Value = DBNull.Value;
      }
      else if (unitCode == 7)
      { // cbm
        row.Cells["RAW_Length"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Width"].Activation = Activation.AllowEdit;
        row.Cells["RAW_Thickness"].Activation = Activation.AllowEdit;
      }
      else
      {
        row.Cells["RAW_Length"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Width"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Thickness"].Activation = Activation.ActivateOnly;
        row.Cells["RAW_Length"].Value = DBNull.Value;
        row.Cells["RAW_Width"].Value = DBNull.Value;
        row.Cells["RAW_Thickness"].Value = DBNull.Value;
      }
    }

    private void LoadData()
    {
      this.NeedToSave = false;
      this.listDeleted = new ArrayList();
      listMaterialDeletedPid = new ArrayList();
      BOMCarcassComponent obj = new BOMCarcassComponent();
      obj.Pid = this.componentPid;
      obj = (BOMCarcassComponent)DataBaseAccess.LoadObject(obj, new string[] { "Pid" });
      if (obj == null)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0007");
        return;
      }

      // Update 14/11/2011
      this.carcassCode = obj.CarcassCode;
      this.componentCode = obj.ComponentCode;
      dsBOMCarcassCompMultiLevel dsComponent = new dsBOMCarcassCompMultiLevel();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);
      inputParam[1] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, this.componentCode);
      DataSet dsCompSource = DataBaseAccess.SearchStoreProcedure("spBOMListCarcassComponentInfo", inputParam);
      if (dsCompSource != null)
      {
        dsComponent.Tables["ComponentInfo"].Merge(dsCompSource.Tables[0]);
        dsComponent.Tables["ComponentMaterial"].Merge(dsCompSource.Tables[1]);
      }
      ultraGridCarcassComp.DataSource = dsComponent;
      // End Update 14/11/2011

      picImage.ImageLocation = FunctionUtility.BOMGetCarcassComponentImage(obj.CarcassCode, obj.ComponentCode);
      //txtDimension.Text = string.Format("{0} X {1} X {2} ", DBConvert.ParseString(obj.Length), DBConvert.ParseString(obj.Width), DBConvert.ParseString(obj.Thickness));
      groupCarcassComp.Text = string.Format("Carcass Code: {0}, Comp Code: {1}", obj.CarcassCode, obj.ComponentCode);
      this.canUpdate = (btnSave.Visible && btnSave.Enabled);
      btnAdd.Visible = this.canUpdate;
      if (this.carConfirm)
      {
        lbConfirm.Visible = false;
      }
      else
      {
        lbConfirm.Visible = true;
      }
      DBParameter[] inputParamProcess = new DBParameter[] { new DBParameter("@ComponentPid", DbType.Int64, this.componentPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListCarcassComponentProcessByComponentPid", inputParamProcess);
      dsSource.Tables[0].TableName = "tblProcessInfo";
      dsSource.Tables[1].TableName = "tblProfile";
      dsSource.Relations.Add(new DataRelation("tblProcessInfo_tblProfile", dsSource.Tables["tblProcessInfo"].Columns["Pid"], dsSource.Tables["tblProfile"].Columns["ProcessPid"], false));
      ultGroupProcessDetail.DataSource = dsSource;
      if (!this.canUpdate)
      {
        ultGroupProcessDetail.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
        ultGroupProcessDetail.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
        ultGroupProcessDetail.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
      }
    }

    private void LoadCarcassComponent()
    {
      if (ultraCBCarcass.SelectedRow != null)
      {
        ultraCBComponent.Text = string.Empty;
        btnCopy.Enabled = false;
        string carcassCode = ultraCBCarcass.Value.ToString();
        DataTable dtComponent = DataBaseAccess.SearchCommandTextDataTable(string.Format("Select Pid, ComponentCode, DescriptionVN From TblBOMCarcassComponent Where CarcassCode = '{0}' ORDER BY No", carcassCode));
        Utility.LoadUltraCombo(ultraCBComponent, dtComponent, "Pid", "ComponentCode", "Pid");
        ultraCBComponent.DisplayLayout.Bands[0].Columns["ComponentCode"].MinWidth = 100;
        ultraCBComponent.DisplayLayout.Bands[0].Columns["ComponentCode"].MaxWidth = 100;
      }
      else
      {
        btnCopy.Enabled = false;
      }
    }

    #endregion LoadData

    #region ProcessData

    private bool SaveData()
    {
      bool result = this.SaveComponentInfo();
      if (result)
      {
        string storeName = string.Empty;
        long outputValue = long.MinValue;
        long pid = long.MinValue;
        // 1. Delete
        foreach (long deletePid in this.listDeleted)
        {
          DBParameter[] inputParamDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, deletePid) };
          DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponentProcess_Delete", inputParamDelete, outputParamDelete);
          outputValue = DBConvert.ParseInt(outputParamDelete[0].Value.ToString());
          if (outputValue != 1)
          {
            result = false;
          }
        }

        // 2. Insert, Update   
        DataSet dsSource = (DataSet)ultGroupProcessDetail.DataSource;
        foreach (DataRow row in dsSource.Tables["tblProcessInfo"].Rows)
        {
          DBParameter[] inputParam = new DBParameter[7];
          DBParameter[] outputParam = new DBParameter[1];
          storeName = string.Empty;
          if (row.RowState == DataRowState.Added)
          {
            storeName = "spBOMCarcassComponentProcess_Insert";
            inputParam[0] = new DBParameter("@ComponentPid", DbType.Int64, this.componentPid);
            inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          else if ((row.RowState == DataRowState.Modified) || ((row.RowState != DataRowState.Deleted) && (DBConvert.ParseInt(row["ChangeProfile"].ToString()) == 1)))
          {
            storeName = "spBOMCarcassComponentProcess_Update";
            pid = DBConvert.ParseLong(row["Pid"].ToString());
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          if (storeName.Length > 0)
          {
            int ordinal = DBConvert.ParseInt(row["Ordinal"].ToString());
            inputParam[1] = new DBParameter("@Ordinal", DbType.Int32, ordinal);

            string text = row["VNMoreDescription"].ToString().Trim();
            if (text.Length > 0)
            {
              inputParam[3] = new DBParameter("@DescriptionVN", DbType.String, 128, text);
            }

            long value = DBConvert.ParseLong(row["ProcessPid"].ToString().Trim());
            if (value != long.MinValue)
            {
              inputParam[4] = new DBParameter("@ProcessPid", DbType.Int64, value);
            }

            string profilePid = row["ProfilePid"].ToString().Trim();
            if (profilePid.Length > 0)
            {
              inputParam[5] = new DBParameter("@ProfilePid", DbType.AnsiString, 128, profilePid);
            }

            outputParam[0] = new DBParameter("@Result", DbType.Int64, 0);
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            outputValue = DBConvert.ParseInt(outputParam[0].Value.ToString());
            if (outputValue <= 0)
            {
              result = false;
            }
          }
        }
      }
      return result;
    }

    private string GetProfilePid(int rowIndex)
    {
      string profile = string.Empty;
      int index = ultGroupProcessDetail.Rows[rowIndex].ChildBands[0].Rows.Count;
      string profilePid = string.Empty;
      for (int i = 0; i < index; i++)
      {
        if (profilePid.Length > 0)
        {
          profilePid += "|";
        }
        profilePid += ultGroupProcessDetail.Rows[rowIndex].ChildBands[0].Rows[i].Cells["ProfilePid"].Value.ToString();
      }
      return profilePid;
    }

    private string GetProfileByProfilePid(string profilePid)
    {
      string profile = string.Empty;
      string commandText = string.Format("SELECT STUFF((SELECT ', ' + ProfileCode FROM TblBOMProfile WHERE ('|' + '{0}' + '|') LIKE ('%|' + CAST(Pid as varchar ) + '|%') FOR XML PATH('')),1,2,'')", profilePid);
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      if (obj != null)
      {
        profile = obj.ToString();
      }
      return profile;
    }

    private bool CheckInvalid()
    {
      //1. Check invalid component        
      int isMainComp = DBConvert.ParseInt(ultraGridCarcassComp.Rows[0].Cells["IsMainComp"].Value.ToString());
      string compCode = ultraGridCarcassComp.Rows[0].Cells["ComponentCode"].Value.ToString();
      int qty = DBConvert.ParseInt(ultraGridCarcassComp.Rows[0].Cells["Qty"].Value.ToString());
      if (isMainComp == 1)
      {
        if (this.componentPid != long.MinValue)
        {
          // Kiem tra xem component nay co la sub component cua ai ko
          string commandText = string.Format("SELECT COUNT(Pid) FROM TblBOMCarcassComponentStruct WHERE SubCompPid = {0}", this.componentPid);
          object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
          if (obj != null && (int)obj > 0)
          {
            WindowUtinity.ShowMessageError("ERR0100", compCode);
            return false;
          }
        }
        if (qty <= 0)
        {
          WindowUtinity.ShowMessageError("MSG0005", "Qty of component");
          return false;
        }
      }

      // Kiem tra Unit
      for (int k = 0; k < ultraGridCarcassComp.Rows[0].ChildBands[0].Rows.Count; k++)
      {
        UltraGridRow childRow = ultraGridCarcassComp.Rows[0].ChildBands[0].Rows[k];
        int unitCode = DBConvert.ParseInt(childRow.Cells["IDFactoryUnit"].Value.ToString());
        long componentPid = DBConvert.ParseLong(childRow.Cells["ComponentPid"].Value.ToString());

        if (unitCode == 4)
        { // m
          double length = DBConvert.ParseDouble(childRow.Cells["RAW_Length"].Value.ToString());
          if (length <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Length of {0}", compCode));
            return false;
          }
        }
        else if (unitCode == 6)
        { // sqm
          double length = DBConvert.ParseDouble(childRow.Cells["RAW_Length"].Value.ToString());
          if (length <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Length of {0}", compCode));
            return false;
          }
          double width = DBConvert.ParseDouble(childRow.Cells["RAW_Width"].Value.ToString());
          if (width <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Width of {0}", compCode));
            return false;
          }
        }
        else if (unitCode == 7)
        { // cbm
          double length = DBConvert.ParseDouble(childRow.Cells["RAW_Length"].Value.ToString());
          if (length <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Length of {0}", compCode));
            return false;
          }
          double width = DBConvert.ParseDouble(childRow.Cells["RAW_Width"].Value.ToString());
          if (width <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Width of {0}", compCode));
            return false;
          }
          double thickness = DBConvert.ParseDouble(childRow.Cells["RAW_Thickness"].Value.ToString());
          if (thickness <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Thickness of {0}", compCode));
            return false;
          }
        }
      }


      //2. Check invalid process
      for (int i = 0; i < ultGroupProcessDetail.Rows.Count; i++)
      {
        int step = DBConvert.ParseInt(ultGroupProcessDetail.Rows[i].Cells["Ordinal"].Value.ToString());
        if (step <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", string.Format("Step at row {0}", DBConvert.ParseString(i + 1)));
          return false;
        }
      }
      return true;
    }

    private DBParameter[] SetCarcassComponentParam(DBParameter[] param, UltraGridRow row)
    {
      param[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);
      string text = row.Cells["ComponentCode"].Value.ToString().Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, text);
      }
      text = row.Cells["DescriptionVN"].Value.ToString().Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@DescriptionVN", DbType.String, 256, text);
      }
      double waste = DBConvert.ParseDouble(row.Cells["Waste"].Value.ToString());
      if (waste != double.MinValue)
      {
        param[4] = new DBParameter("@Waste", DbType.Double, waste);
      }
      double dimesion = DBConvert.ParseDouble(row.Cells["FIN_Length"].Value.ToString());
      if (dimesion != double.MinValue)
      {
        param[5] = new DBParameter("@Length", DbType.Double, dimesion);
      }
      dimesion = DBConvert.ParseDouble(row.Cells["FIN_Width"].Value.ToString());
      if (dimesion != double.MinValue)
      {
        param[6] = new DBParameter("@Width", DbType.Double, dimesion);
      }
      dimesion = DBConvert.ParseDouble(row.Cells["FIN_Thickness"].Value.ToString());
      if (dimesion != double.MinValue)
      {
        param[7] = new DBParameter("@Thickness", DbType.Double, dimesion);
      }
      int value = DBConvert.ParseInt(row.Cells["Lamination"].Value.ToString());
      if (value != int.MinValue)
      {
        param[8] = new DBParameter("@Lamination", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["FingerJoin"].Value.ToString());
      if (value != int.MinValue)
      {
        param[9] = new DBParameter("@FingerJoin", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["Specify"].Value.ToString());
      if (value != int.MinValue)
      {
        param[10] = new DBParameter("@Specify", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["Status"].Value.ToString());
      if (value != int.MinValue)
      {
        param[11] = new DBParameter("@Status", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["ContractOut"].Value.ToString());
      if (value != int.MinValue)
      {
        param[12] = new DBParameter("@ContractOut", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["No"].Value.ToString());
      if (value != int.MinValue)
      {
        param[13] = new DBParameter("@No", DbType.Int32, value);
      }
      value = DBConvert.ParseInt(row.Cells["Primary"].Value.ToString());
      if (value != int.MinValue)
      {
        param[14] = new DBParameter("@Primary", DbType.Int32, value);
      }
      int isMainComp = DBConvert.ParseInt(row.Cells["IsMainComp"].Value.ToString());
      isMainComp = (isMainComp == 1 ? isMainComp : 0);
      param[15] = new DBParameter("@IsMainComp", DbType.Int32, isMainComp);
      if (isMainComp == 1)
      {
        value = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
        if (value != int.MinValue)
        {
          param[16] = new DBParameter("@Qty", DbType.Int32, value);
        }
      }
      int isCompStore = DBConvert.ParseInt(row.Cells["isCompStore"].Value.ToString());
      isCompStore = (isCompStore == 1 ? isCompStore : 0);
      param[18] = new DBParameter("@isCompStore", DbType.Int32, isCompStore);
      return param;
    }

    private DBParameter[] SetComponentDetailParam(DBParameter[] param, UltraGridRow row)
    {
      string text = row.Cells["MaterialCode"].Value.ToString().Replace("'", "''");
      param[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, text);
      double qty = DBConvert.ParseDouble(row.Cells["QtyCombine"].Value.ToString());
      if (qty != double.MinValue)
      {
        param[2] = new DBParameter("@Qty", DbType.Double, qty);
      }
      qty = DBConvert.ParseDouble(row.Cells["Waste"].Value.ToString());
      if (qty != double.MinValue)
      {
        param[3] = new DBParameter("@Waste", DbType.Double, qty);
      }
      double dimension = DBConvert.ParseDouble(row.Cells["RAW_Length"].Value.ToString());
      if (dimension != double.MinValue)
      {
        param[4] = new DBParameter("@Length", DbType.Double, dimension);
      }
      dimension = DBConvert.ParseDouble(row.Cells["RAW_Width"].Value.ToString());
      if (dimension != double.MinValue)
      {
        param[5] = new DBParameter("@Width", DbType.Double, dimension);
      }
      dimension = DBConvert.ParseDouble(row.Cells["RAW_Thickness"].Value.ToString());
      if (dimension != double.MinValue)
      {
        param[6] = new DBParameter("@Thickness", DbType.Double, dimension);
      }
      text = row.Cells["Alternative"].Value.ToString().Replace("'", "''");
      if (text.Length > 0)
      {
        param[7] = new DBParameter("@Alternative", DbType.AnsiString, 16, text);
      }
      return param;
    }

    private bool SaveComponentInfo()
    {
      bool result = true;
      // Delete Material
      foreach (long pid in this.listMaterialDeletedPid)
      {
        DBParameter[] inputParams = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponentDetail_Delete", inputParams, outputParams);
        if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
        {
          result = false;
        }
      }
      // Update Component
      int countComponent = ultraGridCarcassComp.Rows.Count;
      for (int i = 0; i < countComponent; i++)
      {
        UltraGridRow row = ultraGridCarcassComp.Rows[i];
        // 1. Save CarcassComponentInfo
        string storeName = string.Empty;
        DBParameter[] inputParam = new DBParameter[19];
        long componentPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        int rowState = DBConvert.ParseInt(row.Cells["RowState"].Value.ToString());
        if (rowState == 1)
        {
          storeName = "spBOMCarcassComponent_Update";
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, componentPid);
          inputParam[17] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        if (storeName.Length > 0)
        {
          inputParam = this.SetCarcassComponentParam(inputParam, row);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          componentPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (componentPid <= 0)
          {
            result = false;
            continue;
          }
        }
        // Save ComponentDetail (material)
        int countComponentDetail = row.ChildBands[0].Rows.Count;
        for (int j = 0; j < countComponentDetail; j++)
        {
          UltraGridRow rowDetail = row.ChildBands[0].Rows[j];
          storeName = string.Empty;
          DBParameter[] inputParamDetail = new DBParameter[9];
          long detailPid = DBConvert.ParseLong(rowDetail.Cells["Pid"].Value.ToString());
          if (detailPid == long.MinValue) // Insert
          {
            storeName = "spBOMCarcassComponentDetail_Insert";
            inputParamDetail[0] = new DBParameter("@ComponentPid", DbType.Int64, componentPid);
            inputParamDetail[8] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          else
          {
            int rowDetailState = DBConvert.ParseInt(rowDetail.Cells["RowState"].Value.ToString());
            if (rowDetailState == 1)
            {
              storeName = "spBOMCarcassComponentDetail_Update";
              inputParamDetail[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
              inputParamDetail[8] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            }
          }
          if (storeName.Length > 0)
          {
            inputParamDetail = this.SetComponentDetailParam(inputParamDetail, rowDetail);
            DBParameter[] outputParamDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamDetail, outputParamDetail);
            detailPid = DBConvert.ParseLong(outputParamDetail[0].Value.ToString());
            if (detailPid <= 0)
            {
              result = false;
            }
          }
        }
      }
      return result;
    }

    private long GetRandomPid()
    {
      Random random = new Random();
      int randomPid = int.MinValue;
      bool isDuplicate = true;
      while (isDuplicate)
      {
        randomPid = random.Next(-100, 0);
        int count;
        for (count = 0; count < ultGroupProcessDetail.Rows.Count; count++)
        {
          if ((long)randomPid == DBConvert.ParseLong(ultGroupProcessDetail.Rows[count].Cells["Pid"].Value.ToString()))
          {
            break;
          }
        }
        if (count == ultGroupProcessDetail.Rows.Count)
        {
          isDuplicate = false;
        }
      }
      return (long)randomPid;
    }
    #endregion ProcessData

    #region Handle Events

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        bool success;
        success = this.SaveData();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SaveSuccess = true;
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
          this.SaveSuccess = false;
        }
        this.LoadData();
      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      DataTable listProcessDetail = new DataTable();
      //long groupPid = DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbGroupProcess));
      long groupPid = DBConvert.ParseLong(ultraCBGroupProcess.Value.ToString());
      if (groupPid != long.MinValue)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@GroupPid", DbType.Int64, groupPid) };
        listProcessDetail = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListMasterProcessInfoByGroupPid", inputParam);
      }
      int count = ultGroupProcessDetail.Rows.Count;
      DataSet dsSource = (DataSet)ultGroupProcessDetail.DataSource;
      foreach (DataRow drRow in listProcessDetail.Rows)
      {
        DataRow row = dsSource.Tables["tblProcessInfo"].NewRow();
        row["Pid"] = this.GetRandomPid();
        row["Ordinal"] = ++count;
        row["ProcessPid"] = drRow["ProcessPid"];
        row["MachineGroup"] = drRow["MachineGroup"];
        dsSource.Tables["tblProcessInfo"].Rows.Add(row);
      }
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      if (ultraCBComponent.SelectedRow != null)
      {
        DataSet listAnotherProcessDetail = new DataSet();
        long pid = DBConvert.ParseLong(ultraCBComponent.Value.ToString());
        if (pid != long.MinValue)
        {
          DBParameter[] inputParamProcess = new DBParameter[] { new DBParameter("@ComponentPid", DbType.Int64, pid) };
          listAnotherProcessDetail = DataBaseAccess.SearchStoreProcedure("spBOMListCarcassComponentProcessByComponentPid", inputParamProcess);
        }

        int count = ultGroupProcessDetail.Rows.Count;
        DataSet dsSource = (DataSet)ultGroupProcessDetail.DataSource;
        foreach (DataRow drRow in listAnotherProcessDetail.Tables[0].Rows)
        {
          DataRow row = dsSource.Tables["tblProcessInfo"].NewRow();
          row["Pid"] = this.GetRandomPid();
          row["Ordinal"] = ++count;
          row["ProcessPid"] = drRow["ProcessPid"];
          row["VNMoreDescription"] = drRow["VNMoreDescription"];
          row["MachineGroup"] = drRow["MachineGroup"];
          row["ProfilePid"] = drRow["ProfilePid"];
          row["Profile"] = drRow["Profile"];
          dsSource.Tables["tblProcessInfo"].Rows.Add(row);
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.btnSave_Click(null, null);
    }

    private void ultGroupProcessDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["Ordinal"].Header.Caption = "Step";
      e.Layout.Bands[0].Columns["Ordinal"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Ordinal"].MaxWidth = 60;

      e.Layout.Bands[0].Columns["ProcessPid"].ValueList = ucbProcessList;
      e.Layout.Bands[0].Columns["ProcessPid"].Header.Caption = "Process";
      e.Layout.Bands[0].Columns["ProcessPid"].Width = 300;
      e.Layout.Bands[0].Columns["ProcessPid"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[0].Columns["ProcessPid"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["MachineGroup"].Header.Caption = "Machine Group";
      e.Layout.Bands[0].Columns["MachineGroup"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["VNMoreDescription"].Header.Caption = "More Description";
      e.Layout.Bands[0].Columns["ProfilePid"].Hidden = true;
      e.Layout.Bands[0].Columns["Profile"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ChangeProfile"].Hidden = true;

      e.Layout.Bands[1].Columns["ProfilePid"].Hidden = true;
      e.Layout.Bands[1].Columns["ProcessPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ProfileCode"].ValueList = udrpProfile;
      e.Layout.Bands[1].Columns["ProfileCode"].Width = 50;

      e.Layout.Bands[0].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowUpdate = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;
      e.Layout.ViewStyleBand = ViewStyleBand.Horizontal;
    }

    private void ultGroupProcessDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (this.checkValue)
      {
        if (e.Cell.Row.ParentRow == null)
        {
          long pid = DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString());
          if (pid == long.MinValue)
          {
            e.Cell.Row.Cells["Pid"].Value = this.GetRandomPid();
          }
        }
        if (this.canUpdate)
        {
          this.NeedToSave = true;
        }
        string columnName = e.Cell.Column.ToString().ToLower();
        switch (columnName)
        {
          case "processpid":
            long processPid = DBConvert.ParseLong(e.Cell.Value.ToString());
            if (processPid == long.MinValue)
            {
              e.Cell.Row.Cells["MachineGroup"].Value = string.Empty;
            }
            else
            {
              DataTable dtProcessInfo = (DataTable)ucbProcessList.DataSource;
              DataRow[] row = dtProcessInfo.Select(string.Format("Pid = {0}", processPid));
              if (row.Length > 0)
              {
                e.Cell.Row.Cells["MachineGroup"].Value = row[0]["MachineGroup"];
              }
            }
            break;
          case "profilecode":
            e.Cell.Row.Cells["ProfilePid"].Value = udrpProfile.SelectedRow.Cells["Pid"].Value;
            e.Cell.Row.Cells["Description"].Value = udrpProfile.SelectedRow.Cells["Description"].Value;
            int rowIndex = e.Cell.Row.ParentRow.Index;
            string profilePid = this.GetProfilePid(rowIndex);
            e.Cell.Row.ParentRow.Cells["ProfilePid"].Value = profilePid;
            e.Cell.Row.ParentRow.Cells["Profile"].Value = this.GetProfileByProfilePid(profilePid);
            e.Cell.Row.ParentRow.Cells["ChangeProfile"].Value = 1;
            break;
          default:
            break;
        }
      }
    }

    private void ultGroupProcessDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (this.checkValue)
      {
        string columnName = e.Cell.Column.ToString().ToLower();
        string text = e.NewValue.ToString();
        bool validData = false;
        switch (columnName)
        {
          case "ordinal":
            validData = (DBConvert.ParseInt(text) > 0);
            if (!validData)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Step");
              e.Cancel = true;
            }
            break;
          case "processpid":
            if (text.Length > 0)
            {
              DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ProcessPid", DbType.Int64, DBConvert.ParseLong(text)) };
              string commandText = "Select Count(ProcessCode) From TblBOMProcessInfo Where Pid = @ProcessPid";
              int count = (int)DataBaseAccess.ExecuteScalarCommandText(commandText, inputParam);
              if (count == 0)
              {
                WindowUtinity.ShowMessageError("ERR0001", "Process");
                e.Cancel = true;
              }
            }
            break;
          case "profilecode":
            validData = FunctionUtility.CheckProfile(text);
            if (!validData)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Profile");
              e.Cancel = true;
              break;
            }
            //Kiem tra trung code
            int profilePid = DBConvert.ParseInt(udrpProfile.SelectedRow.Cells["Pid"].Value.ToString());
            int rowIndex = e.Cell.Row.ParentRow.Index;
            int index = ultGroupProcessDetail.Rows[rowIndex].ChildBands[0].Rows.Count;
            for (int i = 0; i < index; i++)
            {
              if (profilePid == DBConvert.ParseInt(ultGroupProcessDetail.Rows[rowIndex].ChildBands[0].Rows[i].Cells["ProfilePid"].Text.Trim().ToString()))
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0006", "Profile");
                e.Cancel = true;
              }
            }
            break;
          default:
            break;
        }
      }
    }

    private void ultGroupProcessDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      foreach (long pid in this.listDeleting)
      {
        this.listDeleted.Add(pid);
      }
      if (deletedRowIndex != int.MinValue)
      {
        string profilePid = this.GetProfilePid(deletedRowIndex);
        ultGroupProcessDetail.Rows[deletedRowIndex].Cells["ProfilePid"].Value = profilePid;
        ultGroupProcessDetail.Rows[deletedRowIndex].Cells["Profile"].Value = this.GetProfileByProfilePid(profilePid);
        ultGroupProcessDetail.Rows[deletedRowIndex].Cells["ChangeProfile"].Value = 1;
      }
      deletedRowIndex = int.MinValue;
    }

    private void ultGroupProcessDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      UltraGridRow parent = e.Rows[0].ParentRow;
      if (parent == null)
      {
        this.listDeleting = new ArrayList();
        foreach (UltraGridRow row in e.Rows)
        {
          long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (pid > 0)
          {
            this.listDeleting.Add(pid);
          }
        }
      }
      else
      {
        this.deletedRowIndex = e.Rows[0].ParentRow.Index;
      }
    }

    #endregion Handle Events    

    private Byte[] ImagePathToByteArray(string imagePath)
    {
      try
      {
        FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
        BinaryReader br = new BinaryReader(fs);
        byte[] imgbyte = new byte[fs.Length + 1];
        imgbyte = br.ReadBytes(Convert.ToInt32((fs.Length)));
        br.Close();
        fs.Close();
        return imgbyte;
      }
      catch { }
      return null;
    }

    private void picImage_DoubleClick(object sender, EventArgs e)
    {
      FunctionUtility.ShowImagePopup(picImage.ImageLocation);
    }

    private void ultraCBGroupProcess_ValueChanged(object sender, EventArgs e)
    {
      btnAdd.Enabled = (ultraCBGroupProcess.SelectedRow != null);
    }

    private void ultraCBCarcass_ValueChanged(object sender, EventArgs e)
    {
      if (this.loaddingData)
      {
        return;
      }
      this.LoadCarcassComponent();
    }

    private void ultraCBComponent_ValueChanged(object sender, EventArgs e)
    {
      if (ultraCBCarcass.SelectedRow == null || ultraCBComponent.SelectedRow == null)
      {
        btnCopy.Enabled = false;
        return;
      }
      string carcassCode = ultraCBCarcass.Value.ToString();
      string anotherComponentCode = ultraCBComponent.Text;
      picAnotherImage.ImageLocation = FunctionUtility.BOMGetCarcassComponentImage(carcassCode, anotherComponentCode);
      btnCopy.Enabled = (anotherComponentCode.Length > 0);
    }

    private void ultraGridCarcassComp_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Override.HeaderClickAction = HeaderClickAction.SortMulti;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].Hidden = true;
      e.Layout.Bands[0].Columns["RowState"].Hidden = true;

      e.Layout.Bands[0].Columns["FIN_Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FIN_Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FIN_Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Comp Code";
      e.Layout.Bands[0].Columns["FingerJoin"].Header.Caption = "Finger Join";
      e.Layout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "VN Description";
      e.Layout.Bands[0].Columns["IsMainComp"].Header.Caption = "Root Comp";
      e.Layout.Bands[0].Columns["IsCompStore"].Header.Caption = "Is Store";
      e.Layout.Bands[0].Columns["FIN_Length"].Header.Caption = "Length";
      e.Layout.Bands[0].Columns["FIN_Width"].Header.Caption = "Width";
      e.Layout.Bands[0].Columns["FIN_Thickness"].Header.Caption = "Thickness";
      e.Layout.Bands[0].Columns["Lamination"].Header.Caption = "La";
      e.Layout.Bands[0].Columns["FingerJoin"].Header.Caption = "FJ";

      e.Layout.Bands[0].Columns["No"].MinWidth = 30;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 30;
      e.Layout.Bands[0].Columns["ComponentCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ComponentCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["FIN_Length"].MinWidth = 50;
      e.Layout.Bands[0].Columns["FIN_Length"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["FIN_Width"].MinWidth = 50;
      e.Layout.Bands[0].Columns["FIN_Width"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["FIN_Thickness"].MinWidth = 70;
      e.Layout.Bands[0].Columns["FIN_Thickness"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Lamination"].MinWidth = 40;
      e.Layout.Bands[0].Columns["Lamination"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["FingerJoin"].MinWidth = 40;
      e.Layout.Bands[0].Columns["FingerJoin"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["Specify"].MinWidth = 55;
      e.Layout.Bands[0].Columns["Specify"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Primary"].MinWidth = 55;
      e.Layout.Bands[0].Columns["Primary"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["Waste"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Waste"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["IsMainComp"].MinWidth = 80;
      e.Layout.Bands[0].Columns["IsMainComp"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ContractOut"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ContractOut"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["IsCompStore"].MinWidth = 80;
      e.Layout.Bands[0].Columns["IsCompStore"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["Lamination"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["FingerJoin"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Primary"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsMainComp"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsCompStore"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Specify"].ValueList = UltraDDSpecify;
      e.Layout.Bands[0].Columns["Status"].ValueList = UltraDDStatus;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = (this.carConfirm ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);

      e.Layout.Bands[1].Columns["QtyCombine"].Header.Caption = "Qty Combine";
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[1].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[1].Columns["FactoryUnit"].Header.Caption = "Factory Unit";

      e.Layout.Bands[1].Columns["RAW_Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["RAW_Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["RAW_Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["QtyCombine"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["RowState"].Hidden = true;
      e.Layout.Bands[1].Columns["ComponentPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ComponentCode"].Hidden = true;
      e.Layout.Bands[1].Columns["IDFactoryUnit"].Hidden = true;

      e.Layout.Bands[1].Columns["MaterialCode"].ValueList = udrpMaterialsCode;
      e.Layout.Bands[1].Columns["Alternative"].ValueList = udrpAlternative;

      e.Layout.Bands[1].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["FactoryUnit"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Override.AllowAddNew = (this.carConfirm ? Infragistics.Win.UltraWinGrid.AllowAddNew.No : Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom);
      e.Layout.Bands[1].Override.AllowDelete = (this.carConfirm ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
      e.Layout.Bands[1].Override.AllowUpdate = (this.carConfirm ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
      e.Layout.Bands[1].Columns["MaterialName"].Width = 250;

      for (int k = 0; k < ultraGridCarcassComp.Rows.Count; k++)
      {
        UltraGridRow row = ultraGridCarcassComp.Rows[k];
        for (int i = 0; i < row.ChildBands[0].Rows.Count; i++)
        {
          UltraGridRow childRow = row.ChildBands[0].Rows[i];
          int unitCode = int.MinValue;
          try
          {
            unitCode = DBConvert.ParseInt(childRow.Cells["IDFactoryUnit"].Value.ToString());
          }
          catch { }
          this.SetStatusRow(childRow, unitCode);
        }
        // Set qty status int root component
        int isMainComp = DBConvert.ParseInt(ultraGridCarcassComp.Rows[k].Cells["IsMainComp"].Value.ToString());
        if (isMainComp == 1)
        {
          ultraGridCarcassComp.Rows[k].Cells["Qty"].Activation = Activation.AllowEdit;
          ultraGridCarcassComp.Rows[k].CellAppearance.BackColor = Color.LightGray;
        }
        else
        {
          ultraGridCarcassComp.Rows[k].Cells["Qty"].Activation = Activation.ActivateOnly;
        }
      }
    }

    private void chkProcessReference_CheckedChanged(object sender, EventArgs e)
    {
      tableLayoutProcessReference.Visible = chkProcessReference.Checked;
    }

    private void ultraGridCarcassComp_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          if (row.ParentRow != null)
          {
            this.listMaterialDeletedPid.Add(pid);
          }
        }
      }
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
    }

    private void ultraGridCarcassComp_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName.ToLower())
      {
        case "materialcode":
        case "alternative":
          bool validMaterials = FunctionUtility.CheckBOMMaterialCode(text, 9);
          if (!validMaterials)
          {
            WindowUtinity.ShowMessageError("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultraGridCarcassComp_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      if ((string.Compare(columnName, "rowstate [hidden]", true) == 0) || (columnName.Equals("selected")))
      {
        return;
      }

      e.Cell.Row.Cells["RowState"].Value = 1;
      switch (columnName)
      {
        case "componentcode":
          if (this.isUpdateComponent)
          {
            this.isUpdateComponent = false;
            e.Cell.Value = e.Cell.Value.ToString().ToUpper();
            this.isUpdateComponent = true;
          }
          break;
        case "materialcode":
          try
          {
            e.Cell.Row.Cells["MaterialName"].Value = udrpMaterialsCode.SelectedRow.Cells["MaterialName"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["MaterialName"].Value = DBNull.Value;
          }
          try
          {
            e.Cell.Row.Cells["FactoryUnit"].Value = udrpMaterialsCode.SelectedRow.Cells["FactoryUnit"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["FactoryUnit"].Value = DBNull.Value;
          }
          try
          {
            e.Cell.Row.Cells["IDFactoryUnit"].Value = udrpMaterialsCode.SelectedRow.Cells["IDFactoryUnit"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["IDFactoryUnit"].Value = DBNull.Value;
          }
          this.SetDefaultDimension(e.Cell.Row);
          int unitCode = int.MinValue;
          try
          {
            unitCode = DBConvert.ParseInt(udrpMaterialsCode.SelectedRow.Cells["IDFactoryUnit"].Value.ToString());
          }
          catch { }
          this.SetStatusRow(e.Cell.Row, unitCode);
          break;
        case "fin_length":
        case "fin_width":
        case "fin_thickness":
          UltraGridRow row = e.Cell.Row;
          if (row.ParentRow == null)
          {
            for (int i = 0; i < row.ChildBands[0].Rows.Count; i++)
            {
              UltraGridRow childRow = row.ChildBands[0].Rows[i];
              this.SetDefaultDimension(childRow);
              unitCode = int.MinValue;
              try
              {
                unitCode = DBConvert.ParseInt(childRow.Cells["IDFactoryUnit"].Value.ToString());
              }
              catch { }
              this.SetStatusRow(childRow, unitCode);
            }
          }
          break;
        case "ismaincomp":
          int isMainComp = DBConvert.ParseInt(e.Cell.Value.ToString());
          if (isMainComp == 1)
          {
            e.Cell.Row.Cells["Qty"].Activation = Activation.AllowEdit;
          }
          else
          {
            e.Cell.Row.Cells["Qty"].Activation = Activation.ActivateOnly;
          }
          break;
        default:
          break;
      }
    }
  }
}