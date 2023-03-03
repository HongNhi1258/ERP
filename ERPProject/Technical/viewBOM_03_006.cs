using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using DaiCo.Shared;
using VBReport;
using System.Diagnostics;
using System.IO;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_03_006 : MainUserControl
  {
    public string supCode;
    private IList listPidDeleted = new ArrayList();
    private IList listPidDeleting = new ArrayList();
    private DataTable dataSource;
    private bool loadingData = false;
    private bool isLoadingComboboxSupport = false;
    private bool canUpdate = false;
    
    public viewBOM_03_006()
    {
      InitializeComponent();
    }

    #region InitData

    private void viewBOM_03_006_Load(object sender, EventArgs e)
    {
      this.LoadComboboxSupport();
      // MaterialsCode
      this.LoadDropdownMaterial(udrpMaterialsCode);
      // Alternative
      this.LoadDropdownMaterial(udpAlternative);
      this.LoadData();
      if (string.Compare(this.ViewParam, "BOM_03_005_02", true) == 0)
      {
        chkLock.Visible = false;
      }
    }

    private void LoadComboboxSupport()
    {
      this.isLoadingComboboxSupport = true;
      DataTable dtSupport = DataBaseAccess.SearchCommandTextDataTable(string.Format("Select SupCode From TblBOMSupportInfo Where DeleteFlag = 0 And SupCode <> '{0}'", this.supCode));
      ControlUtility.LoadCombobox(cmbSupport, dtSupport, "SupCode", "SupCode");
      this.isLoadingComboboxSupport = false;
    }
    
    private void LoadData()
    {
      this.loadingData = true;
      bool confirm = false;
      if (this.supCode.Length > 0)
      {
        txtSupCode.Text = this.supCode.Substring(4, this.supCode.Length - 4);
        txtSupCode.ReadOnly = true;        
        BOMSupportInfo obj = new BOMSupportInfo();
        obj.SupCode = this.supCode;
        obj = (BOMSupportInfo)DataBaseAccess.LoadObject(obj, new string[] { "SupCode" });

        if (obj == null)
        {
          return;
        }
        txtNameEN.Text = obj.Description;
        txtNameVN.Text = obj.DescriptionVN;
        txtRemark.Text = obj.Remark;
        confirm = (obj.Confirm == 1);
        chkLock.Checked = confirm;
        if (confirm)
        {
          chkLock.Enabled = false;
          btnSave.Enabled = false;
          lbConfirm.Visible = false;
        }        
      }      
      this.canUpdate = ((!confirm) && (btnSave.Visible) && (btnSave.Enabled));
      if (!this.canUpdate) {
        txtNameEN.ReadOnly = true;
        txtNameVN.ReadOnly = true;
        txtRemark.ReadOnly = true;
        gbReference.Visible = false;
        groupImportFromExcel.Visible = false;
        btnPrint.Enabled = true; ;
      }
      this.LoadGrid();
      this.loadingData = false;
      this.NeedToSave = false;
    }
    
    private void LoadDropdownMaterial(UltraDropDown udrpMaterials)
    {
      string commandText = string.Format(@" SELECT MaterialCode, MaterialName, MaterialNameVn, Unit 
                                            FROM VBOMMaterialsForSupportInfo 
                                            ORDER BY MaterialCode ");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpMaterials.DataSource = dtSource;
      udrpMaterials.ValueMember = "MaterialCode";
      udrpMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      //udrpMaterials.DisplayLayout.Bands[0].Columns["Unit"].Hidden = true;
      //udrpMaterials.DisplayLayout.Bands[0].Columns["UnitCode"].Hidden = true;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialName"].Width = 350;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialNameVn"].Width = 350;
    }

    private void LoadGrid()
    {
      this.listPidDeleted = new ArrayList();     

      // Data
      //this.listMaterialCodeDeleted = string.Empty;
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SupCode", DbType.AnsiString, 16, this.supCode) };
      this.dataSource = DataBaseAccess.SearchStoreProcedureDataTable("spBOMListSupportDetailBySupCode", inputParam);
      ultSupportDetail.DataSource = this.dataSource;      
    }

    #endregion InitData

    #region Process

    private bool CheckValid(out string message)
    {
      message = string.Empty;
      // 1. SupCode
      if (txtSupCode.Text.Replace(" ", "").Replace("'", "").Length == 0)
      {
        message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Support Code");
        return false;
      }
      // 2. TblBOMSupportInfo
      string text = txtNameEN.Text.Trim();
      if (text.Length == 0) {
        message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The English Name");
        return false;
      }
      // 3. TblBOMSupportDetail
      int count = ultSupportDetail.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultSupportDetail.Rows[i];
        text = row.Cells["MaterialCode"].Value.ToString().Trim();
        if (text.Length == 0)
        {
          message = string.Format(FunctionUtility.GetMessage("MSG0005"), "The MaterialCode");
          return false;
        }
        double qty = DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString());
        if (qty < 0)
        {
          message = string.Format(FunctionUtility.GetMessage("ERR0001"), "The Qty");
          return false;
        }
      }
      return true;
    }

    private string SaveSupportInfo()
    {
      // 1. TblBOMSupportInfo
      DBParameter[] inputParam = new DBParameter[7];
      string code = string.Format("{0}-{1}", txtPrefix.Text, txtSupCode.Text.Replace(" ", "").Replace("'", ""));
      string storeName = string.Empty;


      if (this.supCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@SupCode", DbType.AnsiString, 16, this.supCode);
        inputParam[5] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spBOMSupportInfo_Update";

      }
      else
      {
        inputParam[0] = new DBParameter("@SupCode", DbType.AnsiString, 16, code);
        inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spBOMSupportInfo_Insert";

      }
      string Name = txtNameEN.Text.Trim();
      inputParam[1] = new DBParameter("@Description", DbType.AnsiString, 128, Name.Replace("'", "''"));
      Name = txtNameVN.Text.Trim();
      if (Name.Length > 0)
      { 
        inputParam[2] = new DBParameter("@DescriptionVN", DbType.String, 128, Name.Replace("'", "''"));
      }
      int value = (chkLock.Checked) ? 1 : 0;
      inputParam[3] = new DBParameter("@Confirm", DbType.Int32, value);
      inputParam[4] = new DBParameter("@DeleteFlag", DbType.Int32, 0);
      string remark = txtRemark.Text.Trim();
      inputParam[6] = new DBParameter("@Remark", DbType.String, 256, remark);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam[0].Value.ToString() == "1")
      {
        return code;
      }
      return string.Empty;
    }

    private bool SaveSupportDetail(string code)
    {
      // 1.Save TblBOMSupportDetail
      // 1.1 Insert/Update
      int outputResult = 0;
      bool result = true;
      string storeName = string.Empty;
      foreach (DataRow dtRow in this.dataSource.Rows)
      {
        storeName = string.Empty;
        if (dtRow.RowState == DataRowState.Added)
        {
          storeName = "spBOMSupportDetail_Insert";
        }
        else if (dtRow.RowState == DataRowState.Modified)
        {
          // Update
          storeName = "spBOMSupportDetail_Update";
        }
        if (storeName.Length > 0)
        {
          DBParameter[] inputParamDetail = this.GetParamater(dtRow, code);
          DBParameter[] outputParamDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, -1) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamDetail, outputParamDetail);
          outputResult = DBConvert.ParseInt(outputParamDetail[0].Value.ToString());
          if (outputResult == 0)
          {
            result = false;
            break;
          }
        }
      }

      // 2.Delete TblBOMSupportDetail
      foreach (long detailPid in this.listPidDeleted)
      {
        DBParameter[] inputParamDelete = new DBParameter[1];
        inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
        DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMSupportDetail_Delete", inputParamDelete, OutputParamDelete);
        long outputValue = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
        if (outputValue == 0)
        {
          result = false;
        }
      }
      return result;
    }

    private bool SaveData()
    {
      bool result = true;
      this.supCode = this.SaveSupportInfo();
      if (this.supCode.Trim().Length == 0)
      {
        return false;
      }
      result = this.SaveSupportDetail(this.supCode);
      return result;
    }

    private DBParameter[] GetParamater(DataRow drRow, string supCode)
    {
      long pid = DBConvert.ParseLong(drRow["Pid"].ToString());
      double qty = DBConvert.ParseDouble(drRow["Qty"].ToString());
      double waste = DBConvert.ParseDouble(drRow["Waste"].ToString());
      int width = DBConvert.ParseInt(drRow["Width"].ToString());
      int depth = DBConvert.ParseInt(drRow["Depth"].ToString());
      int height = DBConvert.ParseInt(drRow["Height"].ToString());
      

      DBParameter[] param = new DBParameter[11];
      if (pid != long.MinValue)
      {
        param[0] = new DBParameter("@Pid", DbType.Int64, pid);
        param[10] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }
      else
      {
        param[10] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      }
      param[1] = new DBParameter("@SupCode", DbType.AnsiString, 16, supCode);
      param[2] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, drRow["MaterialCode"].ToString().Trim());
      if (qty != double.MinValue)
      {
        param[3] = new DBParameter("@Qty", DbType.Double, qty);
      }
      if (waste != double.MinValue)
      {
        param[4] = new DBParameter("@Waste", DbType.Double, waste);
      }
      if (width != int.MinValue)
      {
        param[5] = new DBParameter("@Width", DbType.Int32, width);
      }
      if (depth != int.MinValue)
      {
        param[6] = new DBParameter("@Depth", DbType.Int32, depth);
      }
      if (height != int.MinValue)
      {
        param[7] = new DBParameter("@Height", DbType.Int32, height);
      }
      string text = drRow["Remark"].ToString().Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[8] = new DBParameter("@Remark", DbType.AnsiString, 128, text);
      }
      text = drRow["Alternative"].ToString().Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[9] = new DBParameter("@Alternative", DbType.AnsiString, 16, text);
      }      
      return param;
    }

    private void FillDataForGrid(DataTable dtExcelMaterial)
    {
      DataTable dtMaterialSource = (DataTable)udrpMaterialsCode.DataSource;
      DataTable dtGridSource = (DataTable)ultSupportDetail.DataSource;
      foreach (DataRow excelRow in dtExcelMaterial.Rows)
      {
        string materialCode = excelRow[0].ToString().Trim();
        double qty = DBConvert.ParseDouble(excelRow[1]);
        if (materialCode.Length > 0)
        {          
          DataRow[] arrRow = dtMaterialSource.Select(string.Format("MaterialCode = '{0}'", materialCode));
          if (arrRow.Length > 0)
          {
            string materialName = arrRow[0]["MaterialName"].ToString();
            string unit = arrRow[0]["Unit"].ToString();
            int unitCode = DBConvert.ParseInt(arrRow[0]["UnitCode"]);
            DataRow rowGrid = dtGridSource.NewRow();
            rowGrid["MaterialCode"] = materialCode;
            rowGrid["MaterialName"] = materialName;
            rowGrid["Unit"] = unit;
            rowGrid["UnitCode"] = unitCode;
            if (qty > 0)
            {
              rowGrid["Qty"] = qty;
            }
            dtGridSource.Rows.Add(rowGrid);
          }
        }
      }
      ultSupportDetail.DataSource = FunctionUtility.CloneTable(dtGridSource);
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
    }

    #endregion Process

    #region Event

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      string code = string.Empty;
      bool success = this.CheckValid(out message);
      if (!success) {
        Shared.Utility.WindowUtinity.ShowMessageWarningFromText(message);
        this.SaveSuccess = false;
        return;
      }
      success = this.SaveData();
      if (success)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.SaveSuccess = true;
      }
      else
      {
        FunctionUtility.UnlockBOMSupportInfo(this.supCode);
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        this.SaveSuccess = false;
      }
      this.LoadData();
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

    private void ultSupportDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string message = string.Empty;
      string columnName = e.Cell.Column.ToString();
      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "MaterialCode":
          if (udrpMaterialsCode.SelectedRow == null)
          {
            ultSupportDetail.Rows[index].Cells["MaterialName"].Value = DBNull.Value;
            ultSupportDetail.Rows[index].Cells["Unit"].Value = DBNull.Value;            
            ultSupportDetail.Rows[index].Cells["Depth"].Activation = Activation.ActivateOnly;
            ultSupportDetail.Rows[index].Cells["Width"].Activation = Activation.ActivateOnly;
            ultSupportDetail.Rows[index].Cells["Height"].Activation = Activation.ActivateOnly;
          }
          else
          {
            ultSupportDetail.Rows[index].Cells["MaterialName"].Value = udrpMaterialsCode.SelectedRow.Cells["MaterialName"].Value;
            ultSupportDetail.Rows[index].Cells["Unit"].Value = udrpMaterialsCode.SelectedRow.Cells["Unit"].Value;
            //int unitCode = DBConvert.ParseInt(udrpMaterialsCode.SelectedRow.Cells["UnitCode"].Value.ToString());
            //switch (unitCode)
            //{
            //  case 4: //m
            //    ultSupportDetail.Rows[index].Cells["Depth"].Activation = Activation.AllowEdit;
            //    ultSupportDetail.Rows[index].Cells["Width"].Activation = Activation.ActivateOnly;
            //    ultSupportDetail.Rows[index].Cells["Height"].Activation = Activation.ActivateOnly;
            //    break;
            //  case 6: //sqm
            //    ultSupportDetail.Rows[index].Cells["Depth"].Activation = Activation.AllowEdit;
            //    ultSupportDetail.Rows[index].Cells["Width"].Activation = Activation.AllowEdit;
            //    ultSupportDetail.Rows[index].Cells["Height"].Activation = Activation.ActivateOnly;
            //    break;
            //  case 7: //cbm
            //    ultSupportDetail.Rows[index].Cells["Depth"].Activation = Activation.AllowEdit;
            //    ultSupportDetail.Rows[index].Cells["Width"].Activation = Activation.AllowEdit;
            //    ultSupportDetail.Rows[index].Cells["Height"].Activation = Activation.AllowEdit;
            //    break;
            //  default:
            //    ultSupportDetail.Rows[index].Cells["Depth"].Activation = Activation.ActivateOnly;
            //    ultSupportDetail.Rows[index].Cells["Width"].Activation = Activation.ActivateOnly;
            //    ultSupportDetail.Rows[index].Cells["Height"].Activation = Activation.ActivateOnly;
            //    break;
            //}
          }
          break;

        default:
          break;
      }
    }

    private void ultSupportDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "materialcode":
        case "alternative":
          string code = e.Cell.Text;
          bool validCode = FunctionUtility.CheckBOMMaterialCode(code, 3);
          if (!validCode)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", columnName);
            e.Cancel = true;
          }
          break;
        case "qty":
          double dQty = DBConvert.ParseDouble(e.Cell.Text);
          if (dQty == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0045", columnName);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultSupportDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listPidDeleting = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue) {
          this.listPidDeleting.Add(pid);
        }        
      }      
    }

    private void ultSupportDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
      foreach(long pid in this.listPidDeleting){
        this.listPidDeleted.Add(pid);
      }
    }

    private void Object_TextChanged(object sender, EventArgs e)
    {
      if (this.loadingData) {
        return;
      }
      if (this.canUpdate)
      {
        this.NeedToSave = true;
      }
    }

    private void btnCopyFromSupport_Click(object sender, EventArgs e)
    {
      bool success = true;
      if (this.NeedToSave || this.supCode.Length == 0)
      {
        string message = string.Empty;
        success = this.CheckValid(out message);
        if (!success)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarningFromText(message);
          return;
        }
        string code = string.Empty;
        success = this.SaveData();
      }
      DBParameter[] inputParams = new DBParameter[3];
      inputParams[0] = new DBParameter("@SupCode", DbType.AnsiString, 16, ControlUtility.GetSelectedValueCombobox(cmbSupport));
      inputParams[1] = new DBParameter("@NewSupCode", DbType.AnsiString, 16, this.supCode);
      inputParams[2] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);
      DataBaseAccess.ExecuteStoreProcedure("spBOMCopyCuttingListFromReferenceSupport", inputParams);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0022");
      }
      else
      {
        FunctionUtility.UnlockBOMSupportInfo(this.supCode);
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
      btnCopyFromSupport.Enabled = false;
    }

    private void cmbSupport_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.isLoadingComboboxSupport)
      {
        return;
      }
      string selectedValue = ControlUtility.GetSelectedValueCombobox(cmbSupport);
      btnCopyFromSupport.Enabled = (selectedValue.Length > 0);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      View_BOM_0018_ItemMaterialReport view = new View_BOM_0018_ItemMaterialReport();
      view.code = supCode;
      view.ncategory = 13;
      Shared.Utility.WindowUtinity.ShowView(view, "Item Material Report", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void txtSupCode_Leave(object sender, EventArgs e)
    {
      if (!txtSupCode.ReadOnly)
      {
        string code = string.Format("{0}-{1}", txtPrefix.Text, txtSupCode.Text.Trim());
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SupCode", DbType.AnsiString, 16, code) };
        string commandText = "Select Count(SupCode) From TblBOMSupportInfo Where SupCode = @SupCode";
        int count = (int)DataBaseAccess.ExecuteScalarCommandText(commandText, inputParam);
        if (count > 0)
        {
          WindowUtinity.ShowMessageError("ERR0006", "Support Code");
          txtSupCode.Focus();
        }
      }
    }

    private void ultSupportDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Depth"].Hidden = true;
      e.Layout.Bands[0].Columns["Height"].Hidden = true;
      e.Layout.Bands[0].Columns["Width"].Hidden = true;
      e.Layout.Bands[0].Columns["Depth"].Header.Caption = "Length";
      e.Layout.Bands[0].Columns["Height"].Header.Caption = "Thickness";
      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = udrpMaterialsCode;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["UnitCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Width"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Depth"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Depth"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Depth"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Height"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Height"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Waste"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Waste"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 40;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["Alternative"].ValueList = udpAlternative;
      e.Layout.Bands[0].Columns["Alternative"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Alternative"].MaxWidth = 100;
      e.Layout.Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Override.AllowDelete = (!this.canUpdate) ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowUpdate = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      int count = ultSupportDetail.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        int unitCode = DBConvert.ParseInt(ultSupportDetail.Rows[i].Cells["UnitCode"].Value.ToString());
        switch (unitCode)
        {
          case 4: //m
            ultSupportDetail.Rows[i].Cells["Depth"].Activation = Activation.AllowEdit;
            ultSupportDetail.Rows[i].Cells["Width"].Activation = Activation.ActivateOnly;
            ultSupportDetail.Rows[i].Cells["Height"].Activation = Activation.ActivateOnly;
            break;
          case 6: //sqm
            ultSupportDetail.Rows[i].Cells["Depth"].Activation = Activation.AllowEdit;
            ultSupportDetail.Rows[i].Cells["Width"].Activation = Activation.AllowEdit;
            ultSupportDetail.Rows[i].Cells["Height"].Activation = Activation.ActivateOnly;
            break;
          case 7: //cbm
            ultSupportDetail.Rows[i].Cells["Depth"].Activation = Activation.AllowEdit;
            ultSupportDetail.Rows[i].Cells["Width"].Activation = Activation.AllowEdit;
            ultSupportDetail.Rows[i].Cells["Height"].Activation = Activation.AllowEdit;
            break;
          default:
            ultSupportDetail.Rows[i].Cells["Depth"].Activation = Activation.ActivateOnly;
            ultSupportDetail.Rows[i].Cells["Width"].Activation = Activation.ActivateOnly;
            ultSupportDetail.Rows[i].Cells["Height"].Activation = Activation.ActivateOnly;
            break;
        }
      }
      lbCount.Text = string.Format("Count: {0}", ultSupportDetail.Rows.FilteredInRowCount);
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "ItemListTemplate";
      string sheetName = "Materials";
      string outFileName = "Material For Support Template";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate\Technical";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [Materials (1)$E3:E4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [Materials (1)$B5:C{0}]", itemCount));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageError("Can't get data. Please check template file.");
        return;
      }
      this.FillDataForGrid(dsItemList.Tables[0]);
    }
    #endregion Event    
  }
}