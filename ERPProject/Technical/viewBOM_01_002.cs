using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Technical;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_01_002 : MainUserControl
  {
    #region Param
    public string itemCode;
    public int revision;
    private string carcassCode = string.Empty;
    private int carcassContractOut = int.MinValue;
    private int with;
    private int depth;
    private int height;
    private int depart;
    private int kd;
    private bool isDuplicate = false;
    private string unit;
    private string deleteMoreSize;
    private bool revisionChange;
    private DataTable otherFinishingSource;
    private DataTable dtItemSaleRelative;
    private IList listDeletingOtherFinishingCode = new ArrayList();
    private IList listDeletedOtherFinishingCode = new ArrayList();
    private IList listDeletedProductionRelative = new ArrayList();
    private bool loadingData = false;
    private int confirmed = 0;
    private string refItemCode;
    #endregion Param

    public viewBOM_01_002()
    {
      InitializeComponent();
    }

    private void viewBOM_01_002_Load(object sender, EventArgs e)
    {
      if (btn1st.Visible)
      {
        depart = 1;
      }
      if (btn2sd.Visible)
      {
        depart = 2;
      }
      pnlAccessRight.Visible = false;

      this.LoadMasterData();
      this.InitData();
    }

    #region function
    private void LoadItemKind()
    {
      string commandText = string.Format("Select Code, Value From TblBOMCodeMaster Where [Group] = {0} And DeleteFlag = 0 Order By Sort", ConstantClass.GROUP_ITEMKIND);
      DataTable dtItemKind = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtItemKind != null)
      {
        DataRow row = dtItemKind.NewRow();
        row["Code"] = 0;
        row["Value"] = "";
        dtItemKind.Rows.InsertAt(row, 0);
        Utility.LoadUltraCombo(ultraCBItemKind, dtItemKind, "Code", "Value", "Code");
        ultraCBItemKind.DisplayLayout.Bands[0].ColHeadersVisible = false;
      }

    }
    private void LoadUIStatus()
    {
      string cm = "";
      if (this.depart == 2)
      {
        cm = @"SELECT 0 Value, 'Request' Label
                      UNION
                      SELECT 1 Value, 'Yes' Label
                      UNION
                      SELECT 2 Value, 'No' Label";
      }

      if (this.depart == 1)
      {
        cm = @"SELECT 0 Value, 'Request' Label";
      }
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      Utility.LoadUltraCombo(ultCBUI, dt, "Value", "Label", false, "Value");
    }


    private void checkChangeDataBeforeClose()
    {
      if (this.loadingData)
      {
        return;
      }
      if (btnUpdate.Enabled && btnUpdate.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.checkChangeDataBeforeClose();
    }

    private bool CheckData()
    {
      //Reference Item
      if (chkReferenceLevel2.Checked && ultraCBReferenceItem.SelectedRow == null)
      {
        MessageBox.Show(string.Format(FunctionUtility.GetMessage("ERR0115"), "Item to reference level 2"));
        return false;
      }
      //With
      try
      {
        with = int.Parse(txtMMWidth.Text.Trim());
      }
      catch
      {
        MessageBox.Show(string.Format(FunctionUtility.GetMessage("ERR0001"), "Width"));
        return false;
      }
      //Depth
      try
      {
        depth = int.Parse(txtMMDepth.Text.Trim());
      }
      catch
      {
        MessageBox.Show(string.Format(FunctionUtility.GetMessage("ERR0001"), "Depth"));
        return false;
      }
      //Height
      try
      {
        height = int.Parse(txtMMHeight.Text.Trim());
      }
      catch
      {
        MessageBox.Show(string.Format(FunctionUtility.GetMessage("ERR0001"), "Height"));
        return false;
      }
      //COM
      if (txtCOM.Text.Trim().Length > 0)
      {
        double com = DBConvert.ParseDouble(txtCOM.Text);
        if (com < 0)
        {
          MessageBox.Show(string.Format(FunctionUtility.GetMessage("ERR0001"), "COM"));
          return false;
        }
      }
      //KD

      //More Size
      DataTable dtMoreSize = (DataTable)this.ultraGridMoreSize.DataSource;
      foreach (DataRow drMoreSize in dtMoreSize.Rows)
      {
        if (drMoreSize.RowState != DataRowState.Deleted)
        {
          if (drMoreSize["DimensionKind"].ToString().Trim().Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Type");
            return false;
          }
          if (drMoreSize["mm"].ToString().Trim().Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "mm");
            return false;
          }
        }
      }

      //Check all components of item have to be confirmed before confirm item      
      if (chkLock.Checked & chkLock.Enabled)
      {
        // Quoc request: user can't confirm item without carcass
        string commandCheckCarcass = string.Format("SELECT COUNT(*) FROM TblBOMItemInfo WHERE ItemCode = '{0}' AND Revision = {1} AND LEN(CarcassCode) > 0", this.itemCode, this.revision);
        int countCarcass = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandCheckCarcass));
        if (countCarcass == 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Error! Can't confirm item without carcass. Please add carcass.");
          return false;
        }

        // All components of item have to be confirmed before item's comfirmed
        object obj = DataBaseAccess.ExecuteScalarCommandText(string.Format("SELECT dbo.FBOMCheckConfirmCompOfItem('{0}', {1})", this.itemCode, this.revision));
        if (obj != null && obj.ToString().Length > 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0341", obj.ToString());
          return false;
        }
      }
      //Customer
      if (ultraCBCustomer.SelectedRow == null)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Customer");
        ultraCBCustomer.Focus();
        return false;
      }

      return true;
    }
    /// <summary>
    /// Check Double
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckFinishCodeDuplicate()
    {
      isDuplicate = false;
      for (int k = 0; k < ultOtherFinishCode.Rows.Count; k++)
      {
        ultOtherFinishCode.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultOtherFinishCode.Rows.Count; i++)
      {
        string finishcode = ultOtherFinishCode.Rows[i].Cells["OtherFinishingCode"].Value.ToString();
        for (int j = i + 1; j < ultOtherFinishCode.Rows.Count; j++)
        {
          string finishcodeCompare = ultOtherFinishCode.Rows[j].Cells["OtherFinishingCode"].Value.ToString();
          if (string.Compare(finishcode, finishcodeCompare, true) == 0)
          {
            ultOtherFinishCode.Rows[i].CellAppearance.BackColor = Color.Yellow;
            ultOtherFinishCode.Rows[j].CellAppearance.BackColor = Color.Yellow;
            this.isDuplicate = true;
          }
        }
      }
    }
    /// <summary>
    /// Check Finishing
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    private bool CheckFinishingCode(string code)
    {
      string strCommandText = @"SELECT Count(FinCode) FROM  TblBOMFinishingInfo WHERE FinCode = @Code AND DeleteFlag = 0"; // Qui, 30/10/2010 drop condition 'Confirm <> 0'
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@Code", DbType.AnsiString, 64, code) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) >= 1);
    }

    private bool CheckItemCode(string code)
    {
      string strCommandText = @"SELECT Count(ItemCode) FROM  TblBOMItemBasic WHERE ItemCode = @Code";
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@Code", DbType.AnsiString, 64, code) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) >= 1);
    }

    private void LoadMasterData()
    {
      this.loadingData = true;
      this.revisionChange = false;
      //Item Kind
      this.LoadItemKind();

      //Finish Code
      Utility.LoadUltraComboboxFinishingStyle(multiCBMainFin, false);

      //Other Finishing Dropdown
      string finishingCondition = "DeleteFlag = 0"; // Qui, 30/10/2010 drop condition 'Confirm <> 0'
      Utility.LoadUltraDropdownFinishsingCode(udrpOtherFinishing, finishingCondition);

      //Main Material
      Utility.LoadUltraComboCodeMst(cmbMainMaterial, ConstantClass.GROUP_MATERIALSTYPE);

      //Other Material
      Utility.LoadOtherMaterials(ultraCBOtherMaterials);

      //Unit
      DataTable dtUnit = DataBaseAccess.SearchCommandTextDataTable("Select UnitPid, Name From TblBOMUnit");
      Utility.LoadUltraCombo(cmbUnit, dtUnit, "UnitPid", "Name");
      cmbUnit.DisplayLayout.Bands[0].ColHeadersVisible = false;

      //List Revision Item
      this.LoadRevision();

      //Load dropdown type in more dimension
      Utility.LoadUltraDropdownCodeMst(ultraDDType, ConstantClass.GROUP_MOREDIMENSION);

      //Load ultraComboCustomer (distribute)
      Utility.LoadUltraCBJC_OEM_Customer(ultraCBCustomer);

      //Load Exhibition
      Utility.LoadUltraCBExhibition(ultraCBExhibition);

      //Set Control UI
      this.LoadUIStatus();
      this.LoadUI();


      //Load Item Reference Data
      string commandText = "SELECT INFO.ItemCode, INFO.Revision, BS.Name, (INFO.ItemCode + ' | ' + CAST(INFO.Revision as varchar) + ' | ' + BS.Name) DisplayText FROM TblBOMItemInfo INFO INNER JOIN TblBOMItemBasic BS ON INFO.ItemCode = BS.ItemCode AND INFO.Revision = BS.RevisionActive";
      DataTable dtReference = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCBReferenceItem, dtReference, "ItemCode", "DisplayText", "DisplayText");

      // Load ultra dropdown Production Item
      DataTable dtProduction = DataBaseAccess.SearchCommandTextDataTable("SELECT ItemCode, SaleCode, Name FROM TblBOMItemBasic ORDER BY ItemCode");
      Utility.LoadUltraDropDown(ultraDDProductionItem, dtProduction, "ItemCode", "ItemCode");
      Utility.LoadUltraDropDown(ultraDDProductionSaleCode, dtProduction, "SaleCode", "SaleCode");
      this.LoadLastCarcassProcess();
      this.loadingData = false;
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

    private void LoadUI()
    {
      //string cm = "spPLNGetAIInfor";
      //DBParameter[] inputParam = new DBParameter[1];
      //inputParam[0] = new DBParameter("@Itemcode", DbType.String, this.itemCode);
      //DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable(cm, inputParam);
      //int UI = DBConvert.ParseInt(dt.Rows[0]["AI"].ToString());
      //switch (depart)
      //{
      //  case 1:
      //    if (UI != int.MinValue)
      //    {
      //      ultCBUI.Value = "Request";
      //    }
      //    this.gbUIDetail.Visible = true;
      //    if (UI != int.MinValue)
      //    {

      //      if (UI == 0)
      //      {
      //        this.ultCBUI.Enabled = false;
      //        this.btnOpen.Visible = false;
      //      }
      //      if (UI == 1)
      //      {
      //        this.ultCBUI.Enabled = false;
      //        this.btnOpen.Visible = true;
      //      }
      //      else if (UI == 2)
      //      {
      //        this.ultCBUI.Enabled = false;
      //        this.btnOpen.Enabled = false;
      //      }
      //    }
      //    else
      //    {
      //      this.btnOpen.Visible = false;
      //    }
      //    break;
      //  case 2:

      //    this.gbUIDetail.Visible = true;
      //    this.pnRemark.Visible = true;
      //    this.txtRemark.Text = dt.Rows[0]["RemarkAI"].ToString();
      //    if (UI != int.MinValue)
      //    {
      //      if (UI == 0)
      //      {
      //        ultCBUI.Value = "Request";
      //        this.ultCBUI.Enabled = true;
      //        this.btnOpen.Visible = false;
      //        pnRemark.Visible = true;
      //      }
      //      if (UI == 1)
      //      {
      //        ultCBUI.Value = "Yes";
      //        this.btnOpen.Visible = true;
      //        this.btnOpen.Enabled = true;
      //        pnRemark.Visible = true;
      //      }
      //      else if (UI == 2)
      //      {
      //        ultCBUI.Value = "No";
      //        this.btnOpen.Visible = true;
      //        this.btnOpen.Enabled = false;
      //        pnRemark.Visible = false;
      //      }
      //    }
      //    else
      //    {
      //      this.btnOpen.Visible = false;
      //    }
      //    break;
      //  default:
      //    this.ultCBUI.Enabled = false;
      //    break;
      //}
    }

    private void LoadRevision()
    {
      //List Revision Item
      string strCmdRevision = string.Format("Select Revision From TblBOMItemInfo Where ItemCode = '{0}'", itemCode);
      cmbRevision.DataSource = DataBaseAccess.SearchCommandTextDataTable(strCmdRevision);
      cmbRevision.DisplayMember = "Revision";
      cmbRevision.ValueMember = "Revision";
      try
      {
        this.revisionChange = true;
        cmbRevision.SelectedValue = revision;
      }
      catch { }
    }
    private bool CheckUI()
    {
      if (txtRemark.Text == null || txtRemark.Text.Length == 0)
      {
        MessageBox.Show(string.Format(FunctionUtility.GetMessage("ERR0001"), "Remark"));
        return false;
      }
      return true;
    }
    private void InitData()
    {
      this.loadingData = true;
      with = int.MinValue;
      depth = int.MinValue;
      height = int.MinValue;
      kd = int.MinValue;
      unit = string.Empty;
      deleteMoreSize = string.Empty;

      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      //Master
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spBOMItemMaster", inputParam);

      if (dtData.Rows.Count > 0)
      {
        //Lock
        this.confirmed = DBConvert.ParseInt(dtData.Rows[0]["Confirm"].ToString());
        this.SetStatusControl();
        this.carcassCode = dtData.Rows[0]["CarcassCode"].ToString();
        this.carcassContractOut = DBConvert.ParseInt(dtData.Rows[0]["CarcassContractOut"].ToString());

        // Update no level 2 (23/12/2010)
        int noLevel2 = DBConvert.ParseInt(dtData.Rows[0]["IsNoLevel2"].ToString());
        chkNoLevel2.Checked = (noLevel2 == 1 ? true : false);
        ultraCBItemKind.Value = DBConvert.ParseInt(dtData.Rows[0]["ItemKind"].ToString());

        txtSaleCode.Text = dtData.Rows[0]["SaleCode"].ToString();
        txtShortName.Text = dtData.Rows[0]["ShortName"].ToString();
        txtItemCode.Text = itemCode;
        txtOldCode.Text = dtData.Rows[0]["OldCode"].ToString();
        txtItemName.Text = dtData.Rows[0]["Name"].ToString();
        //Tien Add
        txtTECNote.Text = dtData.Rows[0]["TecNote"].ToString();
        //
        picItemImage.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
        ultraCBCustomer.Value = dtData.Rows[0]["CustomerPid"];
        ultraCBExhibition.Value = dtData.Rows[0]["Exhibition"];

        //Revision Records
        DataTable dtRevisionRecord = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListRevisionRecords", inputParam);

        dgvRevisionRecord.DataSource = dtRevisionRecord;
        dgvRevisionRecord.DisplayLayout.Bands[0].Columns["Linked"].Header.Caption = "File Link";
        dgvRevisionRecord.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
        dgvRevisionRecord.DisplayLayout.Bands[0].Columns["ChangeKind"].Hidden = true;

        txtDescription.Text = dtData.Rows[0]["Description"].ToString();
        txtCategory.Text = dtData.Rows[0]["Category"].ToString();
        txtCollection.Text = dtData.Rows[0]["Collection"].ToString();

        txtMMWidth.Text = dtData.Rows[0]["MMWidth"].ToString();
        txtMMDepth.Text = dtData.Rows[0]["MMDepth"].ToString();
        txtMMHeight.Text = dtData.Rows[0]["MMHigh"].ToString();
        txtCBM.Text = dtData.Rows[0]["CBM"].ToString();
        txtCOM.Text = dtData.Rows[0]["COM"].ToString();
        try
        {
          cmbUnit.Value = dtData.Rows[0]["Unit"];
        }
        catch { }

        //KD        
        Utility.LoadUltraComboCodeMst(cmbKD, 3);
        try
        {
          cmbKD.Value = dtData.Rows[0]["KD"];
        }
        catch { }

        //More size

        DataTable dtMoreDIM = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListMoreDimention", inputParam);
        ultraGridMoreSize.DataSource = dtMoreDIM;

        try
        {
          multiCBMainFin.Value = dtData.Rows[0]["MainFinish"];
        }
        catch { }

        if (DBConvert.ParseInt(dtData.Rows[0]["CarcassProcessID"]) > 0)
        {
          ucbLastCarcassProcess.Value = DBConvert.ParseInt(dtData.Rows[0]["CarcassProcessID"]);
        }
        else
        {
          ucbLastCarcassProcess.Value = null;
        }


        // OtherFinishing
        this.otherFinishingSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListOtherFinishingByItemCode", inputParam);
        ultOtherFinishCode.DataSource = this.otherFinishingSource;

        try
        {
          cmbMainMaterial.Value = dtData.Rows[0]["MainMaterial"];

        }
        catch { }
        Utility.SetCheckedValueUltraCombobox(ultraCBOtherMaterials, dtData.Rows[0]["OtherMaterial"].ToString());

        //Sale Relationship
        DBParameter[] input = new DBParameter[] { new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode) };
        this.dtItemSaleRelative = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListItemSaleRelativeByItemCode", input);
        ultraGridSaleRelative.DataSource = this.dtItemSaleRelative;

        //Production Relationship
        DataTable dtProductionRelative = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spBOMProductionRelative_Select", input);
        dtProductionRelative.PrimaryKey = new DataColumn[] { dtProductionRelative.Columns["Relative"] };
        ultraGridProductionRelative.DataSource = dtProductionRelative;
      }
      chkReferenceLevel2.Checked = false;
      ultraCBReferenceItem.Value = null;
      this.refItemCode = string.Empty;
      this.loadingData = false;
      this.NeedToSave = false;
    }

    private void ReferenceItem(string refItemCode, int refRevision)
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, refItemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, refRevision);
      //Master
      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spBOMItemMaster", inputParam);

      if (dtData.Rows.Count > 0)
      {
        ultraCBCustomer.Value = dtData.Rows[0]["CustomerPid"];
        ultraCBExhibition.Value = dtData.Rows[0]["Exhibition"];
        txtMMWidth.Text = dtData.Rows[0]["MMWidth"].ToString();
        txtMMDepth.Text = dtData.Rows[0]["MMDepth"].ToString();
        txtMMHeight.Text = dtData.Rows[0]["MMHigh"].ToString();
        txtCOM.Text = dtData.Rows[0]["COM"].ToString();
        try
        {
          cmbUnit.Value = dtData.Rows[0]["Unit"];
        }
        catch { }

        //KD                
        try
        {
          cmbKD.Value = dtData.Rows[0]["KD"];
        }
        catch { }

        //More size
        DataTable dtMoreDIM = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListMoreDimention", inputParam);
        foreach (DataRow rMoreDIM in dtMoreDIM.Rows)
        {
          rMoreDIM["Pid"] = int.MinValue;
        }
        ultraGridMoreSize.DataSource = dtMoreDIM;

        try
        {
          multiCBMainFin.Value = dtData.Rows[0]["MainFinish"];
        }
        catch { }

        // OtherFinishing
        this.otherFinishingSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListOtherFinishingByItemCode", inputParam);
        foreach (DataRow rOtherFinishing in this.otherFinishingSource.Rows)
        {
          rOtherFinishing["Pid"] = long.MinValue;
        }
        ultOtherFinishCode.DataSource = this.otherFinishingSource;

        try
        {
          cmbMainMaterial.Value = dtData.Rows[0]["MainMaterial"];
        }
        catch { }
        Utility.SetCheckedValueUltraCombobox(ultraCBOtherMaterials, dtData.Rows[0]["OtherMaterial"].ToString());
      }
    }

    private void SetStatusControl()
    {
      btnNewRevision.Enabled = false;
      if (this.confirmed == 1)
      {
        chkLock.Checked = true;
        chkLock.Enabled = false;
        if (this.revision != 0)
        {
          btnNewRevision.Enabled = true;
        }
        btnRecords.Enabled = false;
        //btnUpdate.Enabled = false;
        btnPrint.Enabled = true;

        txtMMWidth.ReadOnly = true;
        txtMMDepth.ReadOnly = true;
        txtMMHeight.ReadOnly = true;
        cmbKD.ReadOnly = true;
        cmbUnit.ReadOnly = true;
        ultraGridMoreSize.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
        ultraGridMoreSize.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
        ultraGridMoreSize.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
        multiCBMainFin.ReadOnly = true;
        ucbLastCarcassProcess.ReadOnly = true;
        ultOtherFinishCode.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
        ultOtherFinishCode.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
        ultOtherFinishCode.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
        cmbMainMaterial.ReadOnly = true;
        ultraCBOtherMaterials.ReadOnly = true;
        lbNotConfirmed.Visible = false;
        ultraCBCustomer.ReadOnly = true;
        ultraCBExhibition.ReadOnly = true;
        btnLoadDataReference.Enabled = false;
        ultraGridProductionRelative.DisplayLayout.Override.AllowAddNew = AllowAddNew.No;
        ultraGridProductionRelative.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
        ultraGridProductionRelative.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
      }
      else
      {
        chkLock.Checked = false;
        chkLock.Enabled = true;
        //btnNewRevision.Enabled = false;
        btnRecords.Enabled = true;
        //btnUpdate.Enabled = true;
        btnPrint.Enabled = false;

        txtMMWidth.ReadOnly = false;
        txtMMDepth.ReadOnly = false;
        txtMMHeight.ReadOnly = false;
        cmbKD.ReadOnly = false;
        cmbUnit.ReadOnly = false;
        ultraGridMoreSize.DisplayLayout.Override.AllowAddNew = AllowAddNew.Yes;
        ultraGridMoreSize.DisplayLayout.Override.AllowDelete = DefaultableBoolean.True;
        ultraGridMoreSize.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
        multiCBMainFin.ReadOnly = false;
        ucbLastCarcassProcess.ReadOnly = false;
        ultOtherFinishCode.DisplayLayout.Override.AllowAddNew = AllowAddNew.Yes;
        ultOtherFinishCode.DisplayLayout.Override.AllowDelete = DefaultableBoolean.True;
        ultOtherFinishCode.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
        cmbMainMaterial.ReadOnly = false;
        ultraCBOtherMaterials.ReadOnly = false;
        lbNotConfirmed.Visible = true;
        ultraGridProductionRelative.DisplayLayout.Override.AllowAddNew = AllowAddNew.Yes;
        ultraGridProductionRelative.DisplayLayout.Override.AllowDelete = DefaultableBoolean.True;
        ultraGridProductionRelative.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.True;
      }
    }

    private DBParameter[] GetBOMOtherFinishingDeleteParams(DataRow dtRow)
    {
      DBParameter[] inputParam = new DBParameter[5];
      long pid = DBConvert.ParseLong(dtRow["Pid"].ToString());
      if (pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
      }
      inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[2] = new DBParameter("@Revision", DbType.Int32, this.revision);
      string otherFinishingCode = dtRow["OtherFinishingCode"].ToString();
      inputParam[3] = new DBParameter("@OtherFinishingCode", DbType.AnsiString, 16, otherFinishingCode);
      string description = dtRow["Description"].ToString();
      inputParam[4] = new DBParameter("@Description", DbType.String, 256, description);
      return inputParam;
    }

    private DBParameter[] GetBOMItemRelativeParams(DataRow dtRow)
    {
      DBParameter[] inputParam = new DBParameter[5];
      long pid = DBConvert.ParseLong(dtRow["Pid"].ToString());
      if (pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
      }
      inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[2] = new DBParameter("@Revision", DbType.Int32, this.revision);
      string itemRelative = dtRow["ItemRelative"].ToString();
      inputParam[3] = new DBParameter("@ItemRelative", DbType.AnsiString, 16, itemRelative);
      string description = dtRow["Description"].ToString();
      inputParam[4] = new DBParameter("@Description", DbType.String, 256, description);
      return inputParam;
    }

    private bool SaveProductionRelative()
    {
      bool success = true;
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, -1) };
      //1. Delete
      foreach (long pid in this.listDeletedProductionRelative)
      {
        DBParameter[] input = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spBOMProductionRelative_Delete", input, output);
        if ((output == null) || (output[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      //2. Insert/Update
      DataTable dtDetail = (DataTable)ultraGridProductionRelative.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[5];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          string itemRelative = row["Relative"].ToString();
          string description = row["Description"].ToString().Trim();
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
          inputParam[2] = new DBParameter("@ItemRelative", DbType.AnsiString, 16, itemRelative);
          if (description.Length > 0)
          {
            inputParam[3] = new DBParameter("@Description", DbType.String, 256, description);
          }
          inputParam[4] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spBOMProductionRelative_Edit", inputParam, output);
          if ((output == null) || (output[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private void SaveData()
    {
      if (this.CheckData())
      {
        DBParameter[] inputParam = new DBParameter[18];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
        inputParam[2] = new DBParameter("@Width", DbType.Int32, with);
        inputParam[3] = new DBParameter("@Depth", DbType.Int32, depth);
        inputParam[4] = new DBParameter("@High", DbType.Int32, height);
        try
        {
          kd = DBConvert.ParseInt(cmbKD.Value.ToString());
          if (kd != int.MinValue)
          {
            inputParam[5] = new DBParameter("@KD", DbType.Int32, kd);
          }
        }
        catch { }
        //Unit
        try
        {
          unit = cmbUnit.Value.ToString();
          if (unit.Length > 0)
          {
            inputParam[6] = new DBParameter("@Unit", DbType.AnsiString, 8, unit);
          }
        }
        catch { }
        try
        {
          inputParam[7] = new DBParameter("@MainFinish", DbType.AnsiString, 16, multiCBMainFin.Value.ToString());
        }
        catch { }
        try
        {
          inputParam[8] = new DBParameter("@MainMaterial", DbType.Int32, int.Parse(cmbMainMaterial.Value.ToString()));
        }
        catch { }
        inputParam[9] = new DBParameter("@OtherMaterial", DbType.AnsiString, 128, Utility.GetCheckedValueUltraCombobox(ultraCBOtherMaterials));
        inputParam[10] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        inputParam[11] = new DBParameter("@Confirm", DbType.Int32, (chkLock.Checked ? 1 : 0));
        if (ultraCBCustomer.SelectedRow != null)
        {
          long customerPid = DBConvert.ParseLong(ultraCBCustomer.Value);
          inputParam[12] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
        }
        if (ultraCBExhibition.SelectedRow != null)
        {
          int exhibition = DBConvert.ParseInt(ultraCBExhibition.Value.ToString());
          inputParam[13] = new DBParameter("@Exhibition", DbType.Int32, exhibition);
        }
        if (txtCOM.Text.Trim().Length > 0)
        {
          inputParam[14] = new DBParameter("@COM", DbType.Double, txtCOM.Text.Trim());
        }
        inputParam[15] = new DBParameter("@TecNote", DbType.String, txtTECNote.Text.Trim());
        if (ucbLastCarcassProcess.Value != null)
        {
          int carcassProcessID = DBConvert.ParseInt(ucbLastCarcassProcess.Value.ToString());
          inputParam[16] = new DBParameter("@CarcassProcessID", DbType.Int32, carcassProcessID);
        }
        // Other Finishing
        string otherfinishings = string.Empty;
        for (int i = 0; i < ultOtherFinishCode.Rows.Count; i++)
        {
          string finishName = ultOtherFinishCode.Rows[i].Cells["Name"].Value.ToString();
          if (otherfinishings.Length > 0)
          {
            otherfinishings = string.Format("{0}, ", otherfinishings);
          }
          otherfinishings = string.Format("{0}{1}", otherfinishings, finishName);
        }
        if (otherfinishings.Length > 0)
        {
          inputParam[17] = new DBParameter("@OtherFinish", DbType.AnsiString, 128, otherfinishings);
        }
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMItemInfo_Update", 36000, inputParam, outputParam);

        #region Delete, Insert, Update More Size
        if (outputParam[0].Value.ToString() == "1")
        {
          //Delete More size
          if (this.refItemCode.Length > 0) //Reference Item
          {
            DBParameter[] inputDelete = new DBParameter[2];
            DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            inputDelete[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
            inputDelete[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
            DataBaseAccess.ExecuteStoreProcedure("spBOMMoreDimension_DeleteByItemCode", inputDelete, outputDelete);
            if (outputDelete[0].Value.ToString() == "0")
            {
              MessageBox.Show(FunctionUtility.GetMessage("ERR0002"));
              this.InitData();
              return;
            }
          }
          else //Not Reference
          {
            string[] arrDelete = deleteMoreSize.Split('|');
            DBParameter[] inputDelete = new DBParameter[1];
            DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            for (int i = 0; i < arrDelete.Length; i++)
            {
              inputDelete[0] = new DBParameter("@Pid", DbType.Int32, DBConvert.ParseInt(arrDelete[i].ToString()));
              DataBaseAccess.ExecuteStoreProcedure("spBOMMoreDimension_Delete", inputDelete, outputDelete);
              if (outputDelete[0].Value.ToString() == "0")
              {
                MessageBox.Show(FunctionUtility.GetMessage("ERR0002"));
                this.InitData();
                return;
              }
            }
          }

          DataTable dtMoreSize = (DataTable)ultraGridMoreSize.DataSource;
          DBParameter[] outputParamSize = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          //Insert, Update More Size
          for (int i = 0; i < dtMoreSize.Rows.Count; i++)
          {
            if (this.refItemCode.Length > 0 || (dtMoreSize.Rows[i].RowState == DataRowState.Modified) || (dtMoreSize.Rows[i].RowState == DataRowState.Added))
            {
              string storeName = string.Empty;
              int pid = DBConvert.ParseInt(dtMoreSize.Rows[i]["Pid"].ToString());
              int dimensionKind = DBConvert.ParseInt(dtMoreSize.Rows[i]["DimensionKind"].ToString());
              DBParameter[] inputParamSize = new DBParameter[5];
              if (this.refItemCode.Length > 0 || dtMoreSize.Rows[i].RowState == DataRowState.Added)
              {
                storeName = "spBOMMoreDimension_Insert";
              }
              else if (dtMoreSize.Rows[i].RowState == DataRowState.Modified)
              {
                storeName = "spBOMMoreDimension_Update";
                inputParamSize[0] = new DBParameter("@Pid", DbType.Int32, pid);
              }
              inputParamSize[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
              inputParamSize[2] = new DBParameter("@Revision", DbType.Int32, revision);
              inputParamSize[3] = new DBParameter("@DimensionKind", DbType.Int32, dimensionKind);
              try
              {
                inputParamSize[4] = new DBParameter("@Values", DbType.Int32, int.Parse(dtMoreSize.Rows[i]["mm"].ToString()));
              }
              catch { }

              DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamSize, outputParamSize);
              if (outputParamSize[0].Value.ToString() == "0")
              {
                MessageBox.Show(FunctionUtility.GetMessage("ERR0002"));
                this.InitData();
                return;
              }
            }
          }
        }
        else
        {
          MessageBox.Show(FunctionUtility.GetMessage("ERR0002"));
          this.InitData();
          this.SaveSuccess = false;
          return;
        }
        #endregion Delete, Insert, Update More Size

        // 1. Save Other Finishing
        // 1.1 Delete
        if (this.refItemCode.Length > 0) //Reference Item
        {
          DBParameter[] inputDelete = new DBParameter[2];
          DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          inputDelete[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
          inputDelete[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
          DataBaseAccess.ExecuteStoreProcedure("spBOMOtherFinishing_DeleteByItemCode", inputDelete, outputDelete);
          if (outputDelete[0].Value.ToString() == "0")
          {
            MessageBox.Show(FunctionUtility.GetMessage("ERR0002"));
            this.InitData();
            return;
          }
        }
        else //Not Reference
        {
          foreach (long pid in this.listDeletedOtherFinishingCode)
          {
            DBParameter[] inputParamParamOtherFinishingDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
            DataBaseAccess.ExecuteStoreProcedure("spBOMOtherFinishing_Delete", inputParamParamOtherFinishingDelete);
          }
        }
        // 1.2 Insert/Update
        foreach (DataRow dtRow in this.otherFinishingSource.Rows)
        {
          string storeName = string.Empty;
          if (dtRow.RowState != DataRowState.Deleted)
          {
            if (this.refItemCode.Length > 0 || dtRow.RowState == DataRowState.Added)
            {
              storeName = "spBOMOtherFinishing_Insert";
            }
            else if (dtRow.RowState == DataRowState.Modified)
            {
              // Update
              storeName = "spBOMOtherFinishing_Update";
            }
          }
          if (storeName.Length > 0)
          {
            DBParameter[] inputParamOtherFinishing = this.GetBOMOtherFinishingDeleteParams(dtRow);
            DBParameter[] outputParamOtherFinishing = new DBParameter[] { new DBParameter("@Result", DbType.Int64, -1) };
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamOtherFinishing, outputParamOtherFinishing);
            long outputResult = DBConvert.ParseLong(outputParamOtherFinishing[0].Value.ToString());
            if (outputResult == 0)
            {
              MessageBox.Show(FunctionUtility.GetMessage("ERR0002"));
              return;
            }
          }
        }

        //if (chkSynchronize.Checked)
        //{
        Utility.SynchronizeFinishing(itemCode, revision);
        //}

        //Update 07/01/2013
        if (chkReferenceLevel2.Checked && ultraCBReferenceItem.SelectedRow != null)
        {
          string referenceItem = ultraCBReferenceItem.SelectedRow.Cells["ItemCode"].Value.ToString();
          int referenceRevision = DBConvert.ParseInt(ultraCBReferenceItem.SelectedRow.Cells["Revision"].Value.ToString());
          DBParameter[] inputReference = new DBParameter[5];
          DBParameter[] outputReference = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          inputReference[0] = new DBParameter("@FromItemCode", DbType.AnsiString, 16, referenceItem);
          inputReference[1] = new DBParameter("@FromRevision", DbType.Int32, referenceRevision);
          inputReference[2] = new DBParameter("@ToItemCode", DbType.AnsiString, 16, this.itemCode);
          inputReference[3] = new DBParameter("@ToRevision", DbType.Int32, this.revision);
          inputReference[4] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

          DataBaseAccess.ExecuteStoreProcedure("spBOMCopyLevel2OfItem", inputReference, outputReference);
          if (outputReference[0].Value.ToString() == "0")
          {
            MessageBox.Show(FunctionUtility.GetMessage("ERR0002"));
            this.InitData();
            return;
          }
        }

        //2. Save production relationship
        if (!this.SaveProductionRelative())
        {
          MessageBox.Show(FunctionUtility.GetMessage("ERR0002"));
          this.InitData();
          return;
        }

        DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.InitData();
        this.SaveSuccess = true;
        this.chkSynchronize.Checked = false;
      }
      else
      {
        this.SaveSuccess = false;
      }
    }

    private bool CheckRelationCode(string code)
    {
      string strCommandText = string.Format(@"SELECT Count(*) FROM  TblBOMItemBasic Where ItemCode <> '{0}' AND ItemCode = @Code", this.itemCode);
      DBParameter[] arrInput = new DBParameter[] { new DBParameter("@Code", DbType.AnsiString, 16, code) };
      return ((int)DataBaseAccess.ExecuteScalarCommandText(strCommandText, arrInput) >= 1);
    }

    #endregion function

    #region Event
    private void btnGoLevel2_Click(object sender, EventArgs e)
    {
      viewBOM_02_001 objItemComp = new viewBOM_02_001();
      objItemComp.itemCode = txtItemCode.Text;
      try
      {
        objItemComp.revision = DBConvert.ParseInt(cmbRevision.SelectedValue.ToString());
      }
      catch { }
      Shared.Utility.WindowUtinity.ShowView(objItemComp, "COMPONENT GROUP 2ND LEVEL", false, Shared.Utility.ViewState.Window);
    }

    private void btnNewFinishingCode_Click(object sender, EventArgs e)
    {
      viewBOM_03_004 view = new viewBOM_03_004();
      view.finishingCode = string.Empty;
      Shared.Utility.WindowUtinity.ShowView(view, "FINISHING INFORMATION", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void lblCarcass_Click(object sender, EventArgs e)
    {
      viewBOM_01_003 objITM = new viewBOM_01_003();
      objITM.iIndex = 9;
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      objITM.tabControl1.SelectTab(objITM.tabCarcass);
      Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP LEVEL 2ND", false, ViewState.Window);
      this.InitData();
    }

    private void lblHardware_Click(object sender, EventArgs e)
    {
      viewBOM_01_003 objITM = new viewBOM_01_003();
      objITM.iIndex = 1;
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      objITM.tabControl1.SelectTab(objITM.tabHardware);
      Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP LEVEL 2ND", false, ViewState.Window);
      this.InitData();
    }

    private void lblGlass_Click(object sender, EventArgs e)
    {
      viewBOM_01_003 objITM = new viewBOM_01_003();
      objITM.iIndex = 2;
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      objITM.tabControl1.SelectTab(objITM.tabGlass);
      Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP 2ND LEVEL", false, ViewState.Window);
      this.InitData();
    }

    private void lblSupport_Click(object sender, EventArgs e)
    {
      viewBOM_01_003 objITM = new viewBOM_01_003();
      objITM.iIndex = 3;
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      objITM.tabControl1.SelectTab(objITM.tabSuport);
      Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP 2ND LEVEL", false, ViewState.Window);
      this.InitData();
    }

    private void lblAccessory_Click(object sender, EventArgs e)
    {
      viewBOM_01_003 objITM = new viewBOM_01_003();
      objITM.iIndex = 4;
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      objITM.tabControl1.SelectTab(objITM.tabAccessory);
      Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP 2ND LEVEL", false, ViewState.Window);
      this.InitData();
    }

    private void lblUpholstery_Click(object sender, EventArgs e)
    {
      viewBOM_01_003 objITM = new viewBOM_01_003();
      objITM.iIndex = 5;
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      objITM.tabControl1.SelectTab(objITM.tabUpholstery);
      Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP 2ND LEVEL", false, ViewState.Window);
      this.InitData();
    }

    private void lblFinishing_Click(object sender, EventArgs e)
    {
      viewBOM_01_003 objITM = new viewBOM_01_003();
      objITM.iIndex = 6;
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      objITM.tabControl1.SelectTab(objITM.tabFinishing);
      Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP 2ND LEVEL", false, ViewState.Window);
      this.InitData();
    }

    private void lblPacking_Click(object sender, EventArgs e)
    {
      viewBOM_01_003 objITM = new viewBOM_01_003();
      objITM.iIndex = 7;
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      objITM.tabControl1.SelectTab(objITM.tabPacking);
      Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP 2ND LEVEL", false, ViewState.Window);
      this.InitData();
    }

    private void lblDirect_Click(object sender, EventArgs e)
    {
      viewBOM_01_003 objITM = new viewBOM_01_003();
      objITM.iIndex = 8;
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      objITM.tabControl1.SelectTab(objITM.tabDirectLabour);
      Shared.Utility.WindowUtinity.ShowView(objITM, "COMPONENT GROUP 2ND LEVEL", false, ViewState.Window);
      this.InitData();
    }

    private void lblQA_Click(object sender, EventArgs e)
    {
      viewBOM_07_001 objITM = new viewBOM_07_001();
      objITM.currItemCode = itemCode;
      objITM.currRevision = revision;
      WindowUtinity.ShowView(objITM, "QA Master", true, ViewState.MainWindow);
      this.InitData();
    }

    private void btnRecords_Click(object sender, EventArgs e)
    {
      viewBOM_01_006 objRecords = new viewBOM_01_006();
      objRecords.itemCode = itemCode;
      objRecords.revision = revision;

      Shared.Utility.WindowUtinity.ShowView(objRecords, "Revision Records Information", false, Shared.Utility.ViewState.ModalWindow);

      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, revision);
      DataTable dtRevisionRecord = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListRevisionRecords", inputParam);

      dgvRevisionRecord.DataSource = dtRevisionRecord;
      dgvRevisionRecord.DisplayLayout.Bands[0].Columns["Linked"].Header.Caption = "File Link";
      dgvRevisionRecord.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      dgvRevisionRecord.DisplayLayout.Bands[0].Columns["ChangeKind"].Hidden = true;
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void txtMMWidth_TextChanged(object sender, EventArgs e)
    {
      try
      {
        this.checkChangeDataBeforeClose();
        txtInchWidth.Text = FunctionUtility.ConverMilimetToInch(int.Parse(txtMMWidth.Text.Trim()));
      }
      catch
      {
        txtInchWidth.Text = "";
      }
    }

    private void txtMMDepth_TextChanged(object sender, EventArgs e)
    {
      try
      {
        this.checkChangeDataBeforeClose();
        txtInchDepth.Text = FunctionUtility.ConverMilimetToInch(int.Parse(txtMMDepth.Text.Trim()));
      }
      catch
      {
        txtInchDepth.Text = "";
      }
    }

    private void txtMMHeight_TextChanged(object sender, EventArgs e)
    {
      try
      {
        this.checkChangeDataBeforeClose();
        txtInchHeight.Text = FunctionUtility.ConverMilimetToInch(int.Parse(txtMMHeight.Text.Trim()));
      }
      catch
      {
        txtInchHeight.Text = "";
      }
    }

    private void btnNewRevision_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Do you want to create new revision?", "Create new revision", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
      {
        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
        inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
        inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMItemInfo_CreateNewRevision", inputParam, outputParam);
        if (outputParam[0].Value.ToString() == "0")
        {
          MessageBox.Show(FunctionUtility.GetMessage("ERR0003"));
        }
        else
        {
          // Auto Copy Image Item 12/10/2011  Start
          string commandText = string.Empty;
          commandText = "SELECT path FROM TBLBOMImagePath WHERE Pid = 1";
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

          string path = dt.Rows[0][0].ToString();
          string file = path + "\\" + this.itemCode + "-" + this.revision.ToString().PadLeft(2, '0') + ".jpg";
          string newFile = path + "\\" + this.itemCode + "-" + outputParam[0].Value.ToString().PadLeft(2, '0') + ".jpg";
          if (File.Exists(file))
          {
            if (!File.Exists(newFile))
            {
              File.Copy(file, newFile);
            }
          }
          // Auto Copy Image Item 12/10/2011  End

          //Copy image carcass and item for subcon
          if (this.carcassContractOut == 1)
          {
            Shared.Utility.FunctionUtility.CopyImageForSubcon(this.carcassCode);
          }
          //

          MessageBox.Show("Copy to new revision successfully");
          revision = DBConvert.ParseInt(outputParam[0].Value.ToString());
          this.revisionChange = false;
          this.LoadRevision();
        }
      }
    }

    private void cmbRevision_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.revisionChange)
      {
        try
        {
          revision = DBConvert.ParseInt(cmbRevision.SelectedValue.ToString());
          this.NeedToSave = false;
        }
        catch { }
        this.InitData();
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    private void toolStripMenuItemSave_Click(object sender, EventArgs e)
    {
      if (btnUpdate.Visible && btnUpdate.Enabled)
      {
        this.SaveData();
      }
    }

    private void dgvRevisionRecord_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Bands[0].Columns["Open"].Header.Caption = "View File";
      e.Layout.Bands[0].Columns["Open"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["Open"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellAppearance.AlphaLevel = 192;

      e.Layout.Override.HeaderAppearance.AlphaLevel = 192;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;

      foreach (Infragistics.Win.UltraWinGrid.UltraGridBand oBand in this.dgvRevisionRecord.DisplayLayout.Bands)
      {
        foreach (Infragistics.Win.UltraWinGrid.UltraGridColumn oColumn in oBand.Columns)
        {

          if (oColumn.DataType.ToString() == "System.Double")
          {
            oColumn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          }
          if (oColumn.DataType.ToString() == "System.DateTime")
          {
            oColumn.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
            oColumn.Format = "dd/MM/yyyy";
          }
        }
      }
    }

    private void dgvRevisionRecord_ClickCellButton(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString();
      switch (columnName)
      {
        case "Open":
          Process prc = new Process();
          prc.StartInfo = new ProcessStartInfo(dgvRevisionRecord.Rows[index].Cells["Linked"].Value.ToString());
          try
          {
            prc.Start();
          }
          catch
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
          }
          break;
      }
    }

    private void ultOtherFinishCode_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      string code = e.Cell.Text.Trim();
      switch (columnName)
      {
        case "otherfinishingcode":
          if (!this.CheckFinishingCode(code))
          {
            MessageBox.Show(string.Format(FunctionUtility.GetMessage("ERR0001"), columnName));
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultOtherFinishCode_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.checkChangeDataBeforeClose();
      string message = string.Empty;
      string columnName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "otherfinishingcode":
          try
          {
            ultOtherFinishCode.Rows[index].Cells["Name"].Value = udrpOtherFinishing.SelectedRow.Cells["FinName"].Value;
          }
          catch
          {
            ultOtherFinishCode.Rows[index].Cells["Name"].Value = DBNull.Value;
          }
          this.CheckFinishCodeDuplicate();
          break;
        default:
          break;
      }
    }

    private void ultOtherFinishCode_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingOtherFinishingCode = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingOtherFinishingCode.Add(pid);
        }
      }
    }

    private void ultOtherFinishCode_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.checkChangeDataBeforeClose();
      this.CheckFinishCodeDuplicate();
      foreach (long pid in this.listDeletingOtherFinishingCode)
      {
        this.listDeletedOtherFinishingCode.Add(pid);
      }
    }

    private void ultOtherFinishCode_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SheenLevel"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].Width = 200;
      e.Layout.Bands[0].Columns["OtherFinishingCode"].ValueList = udrpOtherFinishing;
      e.Layout.Bands[0].Columns["OtherFinishingCode"].Width = 100;
      e.Layout.Bands[0].Columns["OtherFinishingCode"].Header.Caption = "Other Finishing Code";
      e.Layout.Override.AllowAddNew = ((this.confirmed != 1 && btnUpdate.Enabled && btnUpdate.Visible) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No);
    }

    private void ultraGridSaleRelative_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RelativeItem"].Header.Caption = "Relative Item";
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      viewBOM_ItemReport view = new viewBOM_ItemReport();

      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, this.itemCode);
      inputParam[1] = new DBParameter("@Revision", DbType.Int32, this.revision);
      view.ncategory = 0;
      view.itemCode = this.itemCode;
      view.revision = this.revision;
      view.dtRevisionRecord = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListRevisionRecords", inputParam);
      view.dtSaleRelation = this.dtItemSaleRelative;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.Window, FormWindowState.Maximized);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridMoreSize_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        if (deleteMoreSize != string.Empty)
        {
          deleteMoreSize += "|";
        }
        deleteMoreSize += row.Cells["Pid"].Value.ToString();
      }
      this.checkChangeDataBeforeClose();
    }

    private void ultraGridMoreSize_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      if (colName.Equals("dimensionkind"))
      {
        try
        {
          e.Cell.Row.Cells["Description"].Value = ultraDDType.SelectedRow.Cells["Description"].Value;
        }
        catch { }
      }
      if (colName.Equals("mm"))
      {
        int mm = DBConvert.ParseInt(e.Cell.Value.ToString());
        string inch = string.Empty;
        if (mm != int.MinValue)
        {
          inch = FunctionUtility.ConverMilimetToInch(mm);
          e.Cell.Row.Cells["Inch"].Value = inch;
        }
      }
      this.checkChangeDataBeforeClose();
    }

    private void ultraGridMoreSize_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["DimensionKind"].Header.Caption = "Type";
      e.Layout.Bands[0].Columns["DimensionKind"].MinWidth = 80;
      e.Layout.Bands[0].Columns["DimensionKind"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["mm"].MinWidth = 60;
      e.Layout.Bands[0].Columns["mm"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["mm"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["mm"].Format = "#,###";
      e.Layout.Bands[0].Columns["DimensionKind"].ValueList = ultraDDType;
      e.Layout.Bands[0].Columns["Description"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Inch"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Inch"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Inch"].MaxWidth = 80;
      e.Layout.Bands[0].Override.AllowAddNew = ((this.confirmed != 1 && btnUpdate.Enabled && btnUpdate.Visible) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No);
    }

    private void ultraGridMoreSize_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (colName.Equals("DimensionKind"))
      {
        string value = e.Cell.Text.Trim();
        if (value.Length > 0)
        {
          DataTable dtMoreSize = (DataTable)ultraDDType.DataSource;
          DataRow[] foundRows = dtMoreSize.Select(string.Format("[Value] = '{0}'", value));
          if (foundRows.Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Type");
            e.Cancel = true;
          }
        }
      }
    }

    private void ultraCBExhibition_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void chkSynchronize_CheckedChanged(object sender, EventArgs e)
    {
      if (chkSynchronize.Checked)
      {
        if (WindowUtinity.ShowMessageConfirm("ERR0106").ToString() == "No")
        {
          chkSynchronize.Checked = false;
          return;
        }
      }
    }

    private void btnLoadDataReference_Click(object sender, EventArgs e)
    {
      if (ultraCBReferenceItem.SelectedRow != null)
      {
        this.refItemCode = ultraCBReferenceItem.SelectedRow.Cells["ItemCode"].Value.ToString();
        int refRevision = DBConvert.ParseInt(ultraCBReferenceItem.SelectedRow.Cells["Revision"].Value.ToString());
        this.ReferenceItem(this.refItemCode, refRevision);
      }
    }

    private void ultraCBReferenceItem_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 60;
    }

    private void ultraGridProductionRelative_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "salecode":
          if (ultraDDProductionSaleCode.SelectedRow != null)
          {
            e.Cell.Row.Cells["Relative"].Value = ultraDDProductionSaleCode.SelectedRow.Cells["ItemCode"].Value;
            e.Cell.Row.Cells["Name"].Value = ultraDDProductionSaleCode.SelectedRow.Cells["Name"].Value;
          }
          break;
        case "relative":
          if (ultraDDProductionItem.SelectedRow != null)
          {
            e.Cell.Row.Cells["SaleCode"].Value = ultraDDProductionItem.SelectedRow.Cells["SaleCode"].Value;
            e.Cell.Row.Cells["Name"].Value = ultraDDProductionItem.SelectedRow.Cells["Name"].Value;
          }
          break;
        default:
          break;
      }
      this.checkChangeDataBeforeClose();
    }

    private void ultraGridProductionRelative_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "relative":
          string code;
          if (e.Cell.Text.Trim().Length == 0)
          {
            code = ultraDDProductionSaleCode.SelectedRow.Cells["ItemCode"].Value.ToString();
          }
          else
          {
            code = e.Cell.Text.Trim();
          }
          if (!this.CheckRelationCode(code))
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Relative Item");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultraGridProductionRelative_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listDeletedProductionRelative.Add(pid);
        }
      }
      this.checkChangeDataBeforeClose();
    }

    private void ultraGridProductionRelative_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemRelative"].Hidden = true;
      e.Layout.Bands[0].Columns["Relative"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      //e.Layout.Bands[0].Columns["SaleCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Relative"].ValueList = ultraDDProductionItem;
      e.Layout.Bands[0].Columns["Relative"].MinWidth = 85;
      e.Layout.Bands[0].Columns["Relative"].MaxWidth = 85;
      e.Layout.Bands[0].Columns["SaleCode"].ValueList = ultraDDProductionSaleCode;
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 80;
    }
    private void btnOpen_Click(object sender, EventArgs e)
    {
      Process prc = new Process();

      string cm = string.Format(@"SELECT Value FROM TblBOMCodeMaster WHERE [GROUP] = 16070 AND Code= '1'");
      string Value = DataBaseAccess.ExecuteScalarCommandText(cm).ToString();
      Value = Value + this.itemCode + ".pdf";
      prc.StartInfo = new ProcessStartInfo(Value);
      try
      {
        prc.Start();
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
      }
    }
    private void ultCBUI_ValueChanged(object sender, EventArgs e)
    {
      string cm = "spPLNGetAIInfor";
      DBParameter[] inputParam_select = new DBParameter[1];
      inputParam_select[0] = new DBParameter("@Itemcode", DbType.String, this.itemCode);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable(cm, inputParam_select);
      int AICode = DBConvert.ParseInt(dt.Rows[0]["AI"].ToString());
      int code = int.MinValue;
      int saveflag = 0;
      if (ultCBUI.Value != null && ultCBUI.Value.ToString().Length > 0)
      {
        code = DBConvert.ParseInt(ultCBUI.Value.ToString().ToLower());
      }
      string messageUI = "";
      switch (code)
      {
        case 0:
          if (this.depart == 1)
          {
            messageUI = "Request AI To Technical";
            DialogResult dlgr = MessageBox.Show(messageUI, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgr == DialogResult.Yes)
            {
              string command = string.Format(@"exec spBOMItemBasic_RequestAIMail {0},'{1}',{2}", code, this.itemCode, SharedObject.UserInfo.UserPid);
              DataBaseAccess.ExecuteCommandText(command);
            }
            else
            {
              return;
            }
          }
          break;
        case 1:
          if (this.depart == 2)
          {
            if (AICode != 1)
            {
              if (code != int.MinValue)
              {
                messageUI = "Accept Request From Technical";
                DialogResult dlUI = MessageBox.Show(messageUI, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlUI == DialogResult.Yes)
                {
                  string command = string.Format(@"exec spBOMItemBasic_RequestAIMail {0},'{1}',{2}", code, this.itemCode, SharedObject.UserInfo.UserPid);
                  DataBaseAccess.ExecuteCommandText(command);
                }
                else
                {
                  return;
                }
              }
            }
            else
            {
              saveflag = 1;
            }
          }
          break;
        case 2:
          if (this.depart == 2)
          {
            if (AICode != 2 && this.CheckUI() && AICode != int.MinValue)
            {
              if (code != int.MinValue)
              {
                messageUI = "Deny Request From Technical";
                DialogResult dlUI = MessageBox.Show(messageUI, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlUI == DialogResult.Yes)
                {
                  string command = string.Format(@"exec spBOMItemBasic_RequestAIMail {0},'{1}',{2},'{3}'", code, this.itemCode, SharedObject.UserInfo.UserPid, txtRemark.Text);
                  DataBaseAccess.ExecuteCommandText(command);
                }
                else
                {
                  return;
                }

              }
            }
            else
            {
              saveflag = 1;
            }
          }
          break;
      }
      if ((code == 0 && this.depart == 1) || (code == 1 && saveflag == 0) || (code == 2 && saveflag == 0))
      {
        DBParameter[] inputParam = new DBParameter[4];
        inputParam[0] = new DBParameter("@ItemCode", DbType.String, this.itemCode);
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int64, SharedObject.UserInfo.UserPid);
        inputParam[2] = new DBParameter("@Status", DbType.Int32, code);
        if (code == 2 && txtRemark.Text.Length > 0)
        {
          inputParam[3] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
        }
        DBParameter[] outputParam = new DBParameter[1];
        outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spBOMItemBacic_SaveAI", inputParam, outputParam);
        if (outputParam[0].Value == null || DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "UI");
        }
        this.LoadMasterData();
        this.InitData();
      }

    }

    private void multiCBMainFin_ValueChanged(object sender, EventArgs e)
    {
      this.checkChangeDataBeforeClose();
      string cmd = string.Format(@"SELECT  LastCarcassProcessID
                    FROM TblBOMFinishingInfo 
                    WHERE FinCode = '{0}'", multiCBMainFin.Value.ToString());
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(cmd);
      if (DBConvert.ParseInt(dataSource.Rows[0][0].ToString()) > 0)
      {
        ucbLastCarcassProcess.Value = DBConvert.ParseInt(dataSource.Rows[0][0].ToString());
      }
    }

    #endregion Event


  }
}

