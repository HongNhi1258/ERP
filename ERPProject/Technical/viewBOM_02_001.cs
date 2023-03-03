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
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_02_001 : MainUserControl
  {
    public string itemCode = string.Empty;
    public string support = string.Empty;
    public string pack = string.Empty;
    public string carcass = string.Empty;
    public int revision = int.MinValue;
    private bool loadingRevision = false;
    private IList listDeletedPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private string tempDelete;
    private string deletePid;
    private bool canUpdate = false;

    public viewBOM_02_001()
    {
      InitializeComponent();
    }

    private void viewBOM_02_001_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + Shared.Utility.SharedObject.UserInfo.UserName + " | " + Shared.Utility.SharedObject.UserInfo.LoginDate;
      this.LoadDropdownList();
      this.LoadData();
    }

    #region Load Data

    private void LoadDropdownComponent(UltraDropDown udrp)
    {
      string commandText = string.Format(@"SELECT COM.Code + '|' + ISNULL(CAST(COM.Revision AS VARCHAR), '') Value, 
                                                  COM.Code, COM.Revision, COM.Name, COM.NameVn, COM.Length, 
                                                  COM.Width, COM.Thickness, COM.ContractOut, COM.CompGroup, MST.Value [Group]
                                          FROM VBOMComponent COM LEFT JOIN TblBOMCodeMaster MST ON (COM.CompGroup = MST.Code AND MST.[Group] = 9)
                                          WHERE COM.CompGroup <> 3
                                          ORDER BY CompGroup, Code, Revision");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      udrp.DataSource = dtSource;
      udrp.ValueMember = "Value";
      udrp.DisplayMember = "Code";
      udrp.DisplayLayout.Bands[0].ColHeadersVisible = true;
      udrp.DisplayLayout.Bands[0].Columns["CompGroup"].Hidden = true;
      udrp.DisplayLayout.Bands[0].Columns["Length"].Hidden = true;
      udrp.DisplayLayout.Bands[0].Columns["Width"].Hidden = true;
      udrp.DisplayLayout.Bands[0].Columns["Thickness"].Hidden = true;
      udrp.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      udrp.DisplayLayout.Bands[0].Columns["ContractOut"].Hidden = true;
    }

    private void LoadDropdownList()
    {
      // 1. Component Group
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = {0} And Code not in (3, 8, 9) And DeleteFlag = 0 ORDER BY Sort", 9);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadCheckBoxComboBox(chkBGroup, dtSource, "Code", "Value");

      // 2. Item Code
      DataTable dtItemInfo = DataBaseAccess.SearchCommandTextDataTable("Select ItemCode, Name, (ItemCode + ' | ' + Name) Display From TblBOMItemBasic");
      if (dtItemInfo != null)
      {
        Utility.LoadUltraCombo(ultraCBItem, dtItemInfo, "ItemCode", "ItemCode", "Display");
        ultraCBItem.Value = this.itemCode;
      }

      // 3. Revision
      this.loadingRevision = true;
      this.LoadRevision(this.itemCode);
      if (this.revision != int.MinValue)
      {
        try
        {
          cmbRevision.SelectedValue = this.revision;
        }
        catch { }
      }

      this.loadingRevision = false;

      // 4. Component Code
      this.LoadDropdownComponent(ultDpComponent);

      // 5. Alternative Code
      this.LoadDropdownComponent(ultDpAlternative);

      //Load data Dropdown Labour            
      string command = "Select Code, NameEN From VBOMSectionForItem ORDER BY Code";
      DataTable dtLabour = DataBaseAccess.SearchCommandTextDataTable(command);
      ultraDDLabour.DataSource = dtLabour;
      ultraDDLabour.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDLabour.DisplayLayout.Bands[0].Columns[0].Width = 150;
      ultraDDLabour.DisplayLayout.Bands[0].Columns[1].Width = 200;

      //Carcass     
      command = "Select CarcassCode From TblBOMCarcass";
      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(command);
      Utility.LoadUltraCombo(ultraCBCarcass, dtCarcass, "CarcassCode", "CarcassCode");
      ultraCBCarcass.Value = this.carcass;

      //Load support code
      command = "Select Code SupCode, Name Description, (Code + ' | ' + Name) as SupDesc From VBOMComponent Where CompGroup = 3";
      DataTable dtSupport = DataBaseAccess.SearchCommandTextDataTable(command);
      Utility.LoadUltraCombo(ultraCBSupportMaterial, dtSupport, "SupCode", "SupDesc");
      //ultraCBSupportMaterial.ColumnWidths = "120, 320, 0";
      ultraCBSupportMaterial.Value = this.support;

      //Load Dropdown Work Area
      Utility.LoadUltraDropdownCodeMst(ultraDDWorkArea, ConstantClass.GROUP_FOUNDY_WORK_AREA);
    }

    private void LoadData()
    {
      if (ultraCBItem.SelectedRow != null)
      {
        this.itemCode = ultraCBItem.Value.ToString();
      }
      else
      {
        this.itemCode = string.Empty;
      }
      this.revision = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbRevision));
      //Load Package
      string command = string.Format("Select PackageCode, PackageName, (PackageCode + ' | ' + PackageName) DescPackage From TblBOMPackage Where ItemCode = '{0}' And Revision = {1}", this.itemCode, this.revision);
      DataTable dtPackage = DataBaseAccess.SearchCommandTextDataTable(command);
      Utility.LoadUltraCombo(ultraCBPackage, dtPackage, "PackageCode", "DescPackage");
      //ultraCBPackage.ColumnWidths = "120, 400, 0";
      ultraCBPackage.Value = this.pack;

      string componentGroup = Utility.GetValueCheckBoxComboBox(chkBGroup);
      pictureItem.ImageLocation = FunctionUtility.BOMGetItemImage(this.itemCode, this.revision);
      if (this.itemCode.Length == 0)
      {
        groupItemImg.Text = string.Empty;
      }
      else
      {
        groupItemImg.Text = string.Format("{0}, Revision: {1}", this.itemCode, this.revision);
      }

      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      if (componentGroup.Length > 0)
      {
        inputParam[2] = new DBParameter("@CompomentGroup", DbType.AnsiString, 1000, string.Format("|{0}|", componentGroup));
      }

      DataSet dsData = DataBaseAccess.SearchStoreProcedure("spBOMItemComponentInfomation", inputParam);
      DataTable dtMaster = dsData.Tables[0];
      DataTable dtDetail = dsData.Tables[1];
      if (dtMaster != null && dtMaster.Rows.Count > 0)
      {
        DataRow row = dtMaster.Rows[0];
        txtSaleCode.Text = row["SaleCode"].ToString();
        txtDescription.Text = row["Description"].ToString();
        try
        {
          ultraCBSupportMaterial.Value = row["SupportMaterial"].ToString();
        }
        catch
        {
          ultraCBSupportMaterial.Value = 0;
        }
        try
        {
          ultraCBCarcass.Value = row["CarcassCode"].ToString();
        }
        catch
        {
          ultraCBCarcass.Value = 0;
        }
        try
        {
          ultraCBPackage.Value = row["PackageCode"].ToString();
        }
        catch
        {
          ultraCBPackage.Value = 0;
        }
        txtItemSize.Text = row["ItemSize"].ToString();
        txtCBM.Text = row["CBM"].ToString();
        int confirm = DBConvert.ParseInt(row["Confirm"].ToString());
        if (confirm == 0)
        {
          lbConfirm.Visible = true;
        }
        else
        {
          lbConfirm.Visible = false;
        }
        int knockDown = DBConvert.ParseInt(row["KD"].ToString());
        if (knockDown == 0)
        {
          rbtNo.Checked = true;
        }
        else
        {
          rbtYes.Checked = true;
        }
        btnPrint.Enabled = (confirm == 1);
        btnSave.Enabled = (confirm == 0);

        //Detail        
        this.canUpdate = (btnSave.Visible && btnSave.Enabled);
        ultItemComp.DataSource = dtDetail;
        if (!this.canUpdate)
        {
          int count = ultItemComp.Rows.Count;
          for (int i = 0; i < count; i++)
          {
            ultItemComp.Rows[i].Activation = Activation.ActivateOnly;
          }
        }

        //Direct Labour
        this.deletePid = string.Empty;
        DBParameter[] inputDBParam = new DBParameter[2];
        inputDBParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputDBParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
        DataTable dtDirectLabour = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListTabDirectLabourInfo", inputDBParam);
        ultraGridLabourInfo.DataSource = dtDirectLabour;
        //Gan Dropdown vao grid        
        ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["SectionCode"].ValueList = ultraDDLabour;
        //Edit lai Grid        
        ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
        ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["NameEN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["NameEN"].CellAppearance.BackColor = Color.LightGray;
        ultraGridLabourInfo.DisplayLayout.Bands[0].Columns["Qty"].Header.Caption = "Qty (Man*hour)";
      }
      else
      {
        txtSaleCode.Text = string.Empty;
        txtDescription.Text = string.Empty;
        txtItemSize.Text = string.Empty;
        txtCBM.Text = string.Empty;
        ultraCBSupportMaterial.Value = DBNull.Value;
        ultraCBCarcass.Value = DBNull.Value;
        ultraCBPackage.Value = DBNull.Value;
        ultItemComp.DataSource = null;
        ultraGridLabourInfo.DataSource = null;
        btnSave.Enabled = false;
        btnPrint.Enabled = false;
      }
      this.NeedToSave = false;
    }

    private void LoadRevision(string itemCode)
    {
      string commandText = string.Format("Select distinct Revision from TblBOMItemInfo Where ItemCode = '{0}' order by Revision desc", itemCode);
      DataTable dtDataRevision = DataBaseAccess.SearchCommandTextDataTable(commandText);
      cmbRevision.DataSource = dtDataRevision;
      cmbRevision.DisplayMember = "Revision";
      cmbRevision.ValueMember = "Revision";
    }

    #endregion Load Data

    #region Save Data

    private DBParameter[] GetParamater(DataRow drRow)
    {
      DBParameter[] param = new DBParameter[13];

      // Pid
      long pid = DBConvert.ParseLong(drRow["Pid"].ToString());
      if (pid != long.MinValue)
      {
        param[0] = new DBParameter("@Pid", DbType.Int64, pid);
      }

      param[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);

      param[2] = new DBParameter("@Revision", DbType.Int32, this.revision);

      param[3] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, drRow["ComponentCode"].ToString().Trim());

      int compRevision = DBConvert.ParseInt(drRow["CompRevision"].ToString());
      if (compRevision != int.MinValue)
      {
        param[4] = new DBParameter("@CompRevision", DbType.Int32, compRevision);
      }


      double value = DBConvert.ParseDouble(drRow["Qty"].ToString());
      if (value != double.MinValue)
      {
        param[5] = new DBParameter("@Qty", DbType.Double, value);
      }

      value = DBConvert.ParseDouble(drRow["Waste"].ToString());
      if (value != double.MinValue)
      {
        param[6] = new DBParameter("@Waste", DbType.Double, value);
      }

      int compGroup = DBConvert.ParseInt(drRow["CompGroup"].ToString());
      if (compGroup != int.MinValue)
      {
        param[7] = new DBParameter("@CompGroup", DbType.Int32, compGroup);
      }

      string text = drRow["Alternative"].ToString().Trim();
      if (text.Length > 0)
      {
        param[8] = new DBParameter("@Alternative", DbType.AnsiString, 16, text);
      }

      compRevision = DBConvert.ParseInt(drRow["AlterRevision"].ToString());
      if (compRevision != int.MinValue)
      {
        param[9] = new DBParameter("@AlterRevision", DbType.Int32, compRevision);
      }

      if (drRow.RowState == DataRowState.Added)
      {
        param[10] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }
      else
      {
        param[10] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      }

      text = drRow["Remark"].ToString().Trim();
      if (text.Length > 0)
      {
        param[11] = new DBParameter("@Remark", DbType.String, 128, drRow["Remark"].ToString().Trim());
      }
      long workAreaPid = DBConvert.ParseLong(drRow["WorkAreaPid"]);
      if (workAreaPid > 0)
      {
        param[12] = new DBParameter("@WorkAreaPid", DbType.Int64, workAreaPid);
      }
      return param;
    }

    private bool SaveData()
    {
      // 1. Update Main
      string carcassCode = string.Empty;
      try
      {
        carcassCode = ultraCBCarcass.Value.ToString();
      }
      catch { }
      string supCode = string.Empty;
      try
      {
        supCode = ultraCBSupportMaterial.Value.ToString();
      }
      catch { }
      string packageCode = string.Empty;
      try
      {
        packageCode = ultraCBPackage.Value.ToString();
      }
      catch { }

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DBParameter[] inputParam = new DBParameter[5];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      inputParam[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
      inputParam[3] = new DBParameter("@SupCode", DbType.AnsiString, 16, supCode);
      inputParam[4] = new DBParameter("@PackageCode", DbType.AnsiString, 50, packageCode);
      DataBaseAccess.ExecuteStoreProcedure("spBOMItemInfo_UpdateMain", inputParam, outputParam);
      if (outputParam[0].Value.ToString() == "0")
      {
        return false;
      }

      // 2.1 Delete
      long outputValue = 0;
      string storeName = string.Empty;
      foreach (long pid in this.listDeletedPid)
      {
        DBParameter[] inputParamDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMItemComponent_Delete", inputParamDelete, outputParamDelete);
        outputValue = DBConvert.ParseLong(outputParamDelete[0].Value.ToString());
        if (outputValue == 0)
        {
          return false;
        }
      }
      // 2.2 Insert, Update
      DataTable dataSource = (DataTable)ultItemComp.DataSource;
      foreach (DataRow drRow in dataSource.Rows)
      {
        storeName = string.Empty;
        switch (drRow.RowState)
        {
          case DataRowState.Modified:
            storeName = "spBOMItemComponent_Update";
            break;
          case DataRowState.Added:
            storeName = "spBOMItemComponent_Insert";
            break;
        }
        if (storeName.Length > 0)
        {
          DBParameter[] inputParamDetail = this.GetParamater(drRow);
          DBParameter[] outputParamDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamDetail, outputParamDetail);
          outputValue = DBConvert.ParseLong(outputParamDetail[0].Value.ToString());
          if (outputValue == 0)
          {
            return false;
          }
        }
      }
      return this.SaveLabour();
    }

    private bool SaveLabour()
    {
      // 1. Delete
      if (deletePid != null && deletePid != string.Empty)
      {
        string[] arrDelete = deletePid.Split('|');
        deletePid = string.Empty;
        for (int i = 0; i < arrDelete.Length; i++)
        {
          DBParameter[] inputParamDelete = new DBParameter[1];
          inputParamDelete[0] = new DBParameter("@Pid", DbType.Int32, arrDelete[i]);
          DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMDirectLabour_Delete", inputParamDelete, outputParamDelete);
          if (outputParamDelete[0].Value.ToString() == "0")
          {
            return false;
          }
        }
      }
      // 2. Insert, update      
      DataTable dtSource = (DataTable)ultraGridLabourInfo.DataSource;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        if ((dtSource.Rows[i].RowState == DataRowState.Modified) || (dtSource.Rows[i].RowState == DataRowState.Added))
        {
          string storeName = string.Empty;
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DBParameter[] inputParam = new DBParameter[8];
          inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
          inputParam[2] = new DBParameter("@SectionCode", DbType.AnsiString, 50, dtSource.Rows[i]["SectionCode"]);
          inputParam[3] = new DBParameter("@Qty", DbType.Double, dtSource.Rows[i]["Qty"]);
          inputParam[4] = new DBParameter("@Description", DbType.String, 512, dtSource.Rows[i]["Description"]);
          inputParam[5] = new DBParameter("@Remark", DbType.String, 512, dtSource.Rows[i]["Remark"]);

          if (dtSource.Rows[i].RowState == DataRowState.Added) //Insert
          {
            inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            storeName = "spBOMDirectLabour_Insert";
          }
          else //Update
          {
            inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            inputParam[7] = new DBParameter("@PID", DbType.Int32, dtSource.Rows[i]["PID"]);
            storeName = "spBOMDirectLabour_Update";
          }
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          if (outputParam[0].Value.ToString() == "0")
          {
            return false;
          }
        }
      }
      return true;
    }

    #endregion Save Data

    #region Other Events
    private void ObjectChanged(object sender, EventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      bool success = this.SaveData();
      if (success)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.SaveSuccess = true;
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        this.SaveSuccess = false;
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = this.SaveData();
      if (success)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
    }

    private void cmbItemCode_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void cmbRevision_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!this.loadingRevision)
      {
        //this.LoadData();
      }
    }

    #endregion Other Events

    #region UltraGrid Events

    private void ultItemComp_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;

      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellAppearance.AlphaLevel = 192;

      e.Layout.Override.HeaderAppearance.AlphaLevel = 192;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ComponentCode"].Hidden = true;
      e.Layout.Bands[0].Columns["DisplayComponentCode"].Header.Caption = "Component Code";
      ultItemComp.DisplayLayout.Bands[0].Columns["DisplayComponentCode"].ValueList = ultDpComponent;
      ultItemComp.DisplayLayout.Bands[0].Columns["WorkAreaPid"].ValueList = ultraDDWorkArea;
      ultItemComp.DisplayLayout.Bands[0].Columns["WorkAreaPid"].Header.Caption = "Work Area";
      ultItemComp.DisplayLayout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Alternative"].Hidden = true;
      e.Layout.Bands[0].Columns["DisplayAlternative"].Header.Caption = "Alternative";
      e.Layout.Bands[0].Columns["DisplayAlternative"].ValueList = ultDpAlternative;
      e.Layout.Bands[0].Columns["CompGroup"].Hidden = true;
      e.Layout.Bands[0].Columns["CompRevision"].Header.Caption = "Comp Revision";
      e.Layout.Bands[0].Columns["CompRevision"].Width = 100;
      e.Layout.Bands[0].Columns["CompRevision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["AlterRevision"].Header.Caption = "Alter Revision";
      e.Layout.Bands[0].Columns["AlterRevision"].Width = 90;
      e.Layout.Bands[0].Columns["AlterRevision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["DisplayAlternative"].Hidden = true;
      e.Layout.Bands[0].Columns["AlterRevision"].Hidden = true;

      e.Layout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;

      e.Layout.Bands[0].Columns["ComponentName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ComponentName"].Header.Caption = "Component Name";
      e.Layout.Bands[0].Columns["ComponentName"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["ComponentName"].Width = 120;
      e.Layout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands[0].Columns["ContractOut"].Width = 90;
      e.Layout.Bands[0].Columns["Length"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Width"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Thickness"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Thickness"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["ContractOut"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ContractOut"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["CompRevision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CompRevision"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Group"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Group"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["AlterRevision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["AlterRevision"].CellAppearance.BackColor = Color.LightGray;
    }

    private void ultraGridLabourInfo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["SectionCode"].Header.Caption = "Section Code";
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
    }

    private void ultItemComp_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      string columnName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "displaycomponentcode":

          try
          {
            ultItemComp.Rows[index].Cells["ComponentName"].Value = ultDpComponent.SelectedRow.Cells["Name"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["ComponentName"].Value = DBNull.Value;
          }
          try
          {
            ultItemComp.Rows[index].Cells["ComponentCode"].Value = ultDpComponent.SelectedRow.Cells["Code"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["ComponentCode"].Value = DBNull.Value;
          }
          try
          {
            ultItemComp.Rows[index].Cells["Length"].Value = ultDpComponent.SelectedRow.Cells["Length"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["Length"].Value = DBNull.Value;
          }
          try
          {
            ultItemComp.Rows[index].Cells["Width"].Value = ultDpComponent.SelectedRow.Cells["Width"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["Width"].Value = DBNull.Value;
          }
          try
          {
            ultItemComp.Rows[index].Cells["Thickness"].Value = ultDpComponent.SelectedRow.Cells["Thickness"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["Thickness"].Value = DBNull.Value;
          }
          try
          {
            ultItemComp.Rows[index].Cells["CompGroup"].Value = ultDpComponent.SelectedRow.Cells["CompGroup"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["CompGroup"].Value = DBNull.Value;
          }
          try
          {
            ultItemComp.Rows[index].Cells["Group"].Value = ultDpComponent.SelectedRow.Cells["Group"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["Group"].Value = DBNull.Value;
          }
          try
          {
            ultItemComp.Rows[index].Cells["ContractOut"].Value = ultDpComponent.SelectedRow.Cells["ContractOut"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["ContractOut"].Value = DBNull.Value;
          }
          try
          {
            ultItemComp.Rows[index].Cells["CompRevision"].Value = ultDpComponent.SelectedRow.Cells["Revision"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["CompRevision"].Value = DBNull.Value;
          }
          break;
        case "displayalternative":
          try
          {
            ultItemComp.Rows[index].Cells["AlterRevision"].Value = ultDpAlternative.SelectedRow.Cells["Revision"].Value;
          }
          catch
          {
            ultItemComp.Rows[index].Cells["AlterRevision"].Value = DBNull.Value;
          }
          try
          {
            ultItemComp.Rows[index].Cells["Alternative"].Value = ultDpAlternative.SelectedRow.Cells["Code"].Value.ToString();
          }
          catch
          {
            ultItemComp.Rows[index].Cells["Alternative"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }

    private void ultItemComp_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "displaycomponentcode":
        case "displayalternative":
          string code = e.Cell.Text;
          columnName = (columnName.Equals("displaycomponentcode")) ? "ComponentCode" : "Alternative";
          if (!FunctionUtility.CheckBOMComponentCode(code))
          {
            MessageBox.Show(string.Format(FunctionUtility.GetMessage("ERR0001"), columnName));
            e.Cancel = true;
          }
          break;
        case "qty":
          double dQty = DBConvert.ParseDouble(e.Cell.Text);
          if (dQty < 0)
          {
            MessageBox.Show(string.Format("Please check data at column {0}", columnName));

            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultItemComp_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    private void ultItemComp_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    private void ultItemComp_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;

      try
      {
        int iIndex = DBConvert.ParseInt(ultItemComp.Selected.Rows[0].Cells["CompGroup"].Value.ToString());

        viewBOM_01_003 objITM = new viewBOM_01_003();
        objITM.iIndex = iIndex;
        objITM.currItemCode = this.itemCode;
        objITM.currRevision = this.revision;
        objITM.tabControl1.SelectTab(objITM.tabCarcass);

        switch (iIndex)
        {
          case 9:
            objITM.tabControl1.SelectTab(objITM.tabCarcass);
            break;
          case 1:
            objITM.tabControl1.SelectTab(objITM.tabHardware);
            break;
          case 2:
            objITM.tabControl1.SelectTab(objITM.tabGlass);
            break;
          case 3:
            objITM.tabControl1.SelectTab(objITM.tabSuport);
            break;
          case 4:
            objITM.tabControl1.SelectTab(objITM.tabAccessory);
            break;
          case 5:
            objITM.tabControl1.SelectTab(objITM.tabUpholstery);
            break;
          case 6:
            objITM.tabControl1.SelectTab(objITM.tabFinishing);
            break;
          case 7:
            objITM.tabControl1.SelectTab(objITM.tabPacking);
            break;
          case 8:
            objITM.tabControl1.SelectTab(objITM.tabDirectLabour);
            break;
          default:
            break;
        }
        Shared.Utility.WindowUtinity.ShowView(objITM, "", false, ViewState.ModalWindow);
      }
      catch
      {
      }
    }

    private void ultItemComp_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      try
      {
        string compCode = ultItemComp.Selected.Rows[0].Cells["ComponentCode"].Value.ToString().Trim();
        string compRevision = ultItemComp.Selected.Rows[0].Cells["CompRevision"].Value.ToString().Trim();
        pictureComp.ImageLocation = FunctionUtility.BOMGetItemComponentImage(compCode);
        groupCompImg.Text = compCode;
      }
      catch { }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void ultraGridLabourInfo_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (e.Cell.Text.ToString().Trim().Length > 0)
      {
        string strColName = e.Cell.Column.ToString();
        DataTable dtTest;
        if (string.Compare(strColName, "SectionCode", true) == 0)
        {
          //Kiem tra xem Section nhap dung chua
          string commandText = string.Format("Select Code From VBOMSection Where Code = '{0}'", e.Cell.Text.ToString().Trim());
          dtTest = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtTest.Rows.Count <= 0)
          {
            MessageBox.Show(string.Format(FunctionUtility.GetMessage("ERR0001"), strColName));
            e.Cancel = true;
          }
        }
      }
    }

    private void ultraGridLabourInfo_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      string columnName = e.Cell.Column.ToString();
      int index = e.Cell.Row.Index;
      if (columnName == "SectionCode")
      {
        try
        {
          ultraGridLabourInfo.Rows[index].Cells["NameEN"].Value = ultraDDLabour.SelectedRow.Cells["NameEN"].Value.ToString();
        }
        catch { }
      }
    }

    private void ultraGridLabourInfo_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.tempDelete = string.Empty;
      foreach (Infragistics.Win.UltraWinGrid.UltraGridRow ultRow in e.Rows)
      {
        if (tempDelete != string.Empty)
        {
          tempDelete += '|';
        }
        try
        {
          tempDelete = tempDelete + ultRow.Cells["PID"].Value.ToString();
        }
        catch { }
      }
    }

    private void ultraGridLabourInfo_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      if (deletePid != string.Empty && deletePid != null)
      {
        deletePid += "|";
      }
      deletePid += tempDelete;
    }
    #endregion UltraGrid Events

    private void btnPrint_Click(object sender, EventArgs e)
    {
      View_BOM_0018_ItemMaterialReport view = new View_BOM_0018_ItemMaterialReport();
      view.itemCode = itemCode;
      view.revision = revision;
      view.ncategory = 15;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void ultraCBItem_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
    }

    private void ultraCBItem_ValueChanged(object sender, EventArgs e)
    {
      string code = string.Empty;
      if (ultraCBItem.SelectedRow != null)
      {
        code = ultraCBItem.Value.ToString();
        txtItemName.Text = ultraCBItem.SelectedRow.Cells["Name"].Value.ToString();
      }
      else
      {
        txtItemName.Text = string.Empty;
      }
      this.loadingRevision = true;
      this.LoadRevision(code);
      this.loadingRevision = false;

    }
  }
}