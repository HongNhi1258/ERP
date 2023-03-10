using DaiCo.Application;
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
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_01_010 : MainUserControl
  {
    #region fields
    public string carcassCode = string.Empty;
    private bool confirmed = false;
    private IList listCompDeletedPid = new ArrayList();
    private IList listMaterialDeletedPid = new ArrayList();
    private bool isUpdateComponent = true;
    private IList listDirectLabourDeletedPid = new ArrayList();
    private IList listRelationDeletedPid = new ArrayList();
    private bool isCanUpdate;
    #endregion fields

    #region function
    /// <summary>
    /// Get new carcass code
    /// </summary>
    /// <returns></returns>
    private string GetNewCarcassCode()
    {
      string first = string.Empty;
      string last = string.Empty;
      if (ultraCBPrefixCode.SelectedRow != null)
      {
        first = ultraCBPrefixCode.SelectedRow.Cells["PrefixCode"].Value.ToString();
      }
      if (ultraCBSuffixCode.SelectedRow != null)
      {
        last = ultraCBSuffixCode.SelectedRow.Cells["MidCode"].Value.ToString();
      }
      DBParameter[] inputParam = new DBParameter[2];
      if (first.Length > 0)
      {
        inputParam[0] = new DBParameter("@Prefix", DbType.AnsiString, 16, first);
      }
      else
      {
        inputParam[0] = new DBParameter("@Prefix", DbType.AnsiString, 16, DBNull.Value);
      }
      if (last.Length > 0)
      {
        inputParam[1] = new DBParameter("@Middle", DbType.AnsiString, 16, last);
      }
      else
      {
        inputParam[1] = new DBParameter("@Middle", DbType.AnsiString, 16, DBNull.Value);
      }
      string commandText = "Select dbo.FBOMGetNewCarcassCode(@Prefix, @Middle)";
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText, inputParam);
      string code = string.Empty;
      if (obj != DBNull.Value)
      {
        code = (string)obj;
      }
      return code;
    }

    private void SetStatusControl()
    {
      chkConfirm.Checked = this.confirmed;
      btnSave.Enabled = !this.confirmed;
      btnCopy.Enabled = !this.confirmed;
      btnDeleteAll.Enabled = !this.confirmed;
      btnFixQty.Enabled = !this.confirmed;

      chkContract.Enabled = !this.confirmed;
      rdFastCosting.Enabled = !this.confirmed;
      rdBOM.Enabled = !this.confirmed;
      txtDescriptionEN.ReadOnly = this.confirmed;
      txtDescriptionVN.ReadOnly = this.confirmed;
      chkConfirm.Enabled = !this.confirmed;
      lbConfirm.Visible = !this.confirmed;

      this.SetStatusReferenceCarcass();
    }

    private void SetStatusReferenceCarcass()
    {
      // Carcass Reference
      if ((ultraGridComponentList.Rows.Count == 0) && (treeViewComponentStruct.Nodes.Count == 0) && (this.carcassCode.Length > 0) && (btnSave.Enabled) && (btnSave.Visible))
      {
        chkReference.Visible = true;
      }
      else
      {
        chkReference.Visible = false;
      }
      groupReference.Visible = (chkReference.Visible && chkReference.Checked);
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

    private void SetDefaultDimension(UltraGridRow row)
    {
      string materialCode = row.Cells["MaterialCode"].Value.ToString().Trim();
      string commandText = string.Format("Select ISNULL(IsFormula, 0) IsFormula From VBOMMaterials Where MaterialCode = '{0}'", materialCode);
      int isFormula = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandText).ToString());
      if (isFormula == 1)
      {
        string code = materialCode.Substring(0, 3);
        UltraGridRow parentRow = row.ParentRow;
        double finishLength = DBConvert.ParseDouble(parentRow.Cells["FIN_Length"].Value.ToString());
        double finishWidth = DBConvert.ParseDouble(parentRow.Cells["FIN_Width"].Value.ToString());
        double finishThickness = DBConvert.ParseDouble(parentRow.Cells["FIN_Thickness"].Value.ToString());
        double addRawLength = 0, addRawWidth = 0, addRawThickness = 0;

        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode) };
        DataTable dtRawDimension = DataBaseAccess.SearchStoreProcedureDataTable("spBOMGetAddRawDimension", inputParam);
        if (dtRawDimension != null && dtRawDimension.Rows.Count > 0)
        {
          addRawLength = DBConvert.ParseDouble(dtRawDimension.Rows[0]["RawLength"]);
          addRawWidth = DBConvert.ParseDouble(dtRawDimension.Rows[0]["RawWidth"]);
          addRawThickness = DBConvert.ParseDouble(dtRawDimension.Rows[0]["RawThickness"]);
        }

        if (finishLength != double.MinValue)
        {
          row.Cells["RAW_Length"].Value = finishLength + addRawLength;
        }
        else
        {
          row.Cells["RAW_Length"].Value = DBNull.Value;
        }
        if (finishWidth != double.MinValue)
        {
          row.Cells["RAW_Width"].Value = finishWidth + addRawWidth;
        }
        else
        {
          row.Cells["RAW_Width"].Value = DBNull.Value;
        }
        if (finishThickness != double.MinValue)
        {
          row.Cells["RAW_Thickness"].Value = finishThickness + addRawThickness;
        }
        else
        {
          row.Cells["RAW_Thickness"].Value = DBNull.Value;
        }
      }
    }

    private void SetStatusTreeNodeMenu(TreeNode node)
    {
      if (node == null)
      {
        contextMenuStripCompStruct.Items["propertiesToolStripMenuItem"].Visible = false;
        contextMenuStripCompStruct.Items["addRootComponentToolStripMenuItem"].Visible = true;
        contextMenuStripCompStruct.Items["deleteRootComponentToolStripMenuItem"].Visible = false;
      }
      else
      {
        long pidRootComp = DBConvert.ParseLong(node.Name);
        string commandText = string.Format("Select IsNull(IsMainComp, 0) IsMainComp From TblBOMCarcassComponent Where Pid = {0}", pidRootComp);
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          if (dt.Rows[0][0].ToString() == "1")
          {
            contextMenuStripCompStruct.Items["propertiesToolStripMenuItem"].Visible = true;
            contextMenuStripCompStruct.Items["addRootComponentToolStripMenuItem"].Visible = true;
            contextMenuStripCompStruct.Items["deleteRootComponentToolStripMenuItem"].Visible = true;
          }
          else
          {
            contextMenuStripCompStruct.Items["propertiesToolStripMenuItem"].Visible = true;
            contextMenuStripCompStruct.Items["addRootComponentToolStripMenuItem"].Visible = true;
            contextMenuStripCompStruct.Items["deleteRootComponentToolStripMenuItem"].Visible = false;
          }
        }
      }
    }

    private void ShowImageComp()
    {
      if (chkShowImageComp.Checked)
      {
        if (ultraGridComponentList.Selected.Rows.Count > 0 && ultraGridComponentList.Selected.Rows[0].ParentRow == null)
        {
          string componentCode = ultraGridComponentList.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
          string imagePath = FunctionUtility.BOMGetCarcassComponentImage(this.carcassCode, componentCode);
          FunctionUtility.ShowImagePopup(imagePath);
        }
      }
    }

    private void DataChanged()
    {
      this.NeedToSave = (this.isCanUpdate ? true : false);
    }
    #region Load Data
    /// <summary>
    /// Load suffix code of carcass
    /// </summary>
    /// <param name="prefixCode"></param>
    private void LoadUltraCBSuffixCode(string prefixCode)
    {
      string commandText = string.Format("SELECT DISTINCT SUBSTRING(CarcassCode, LEN('{0}') + 2, 6) MidCode FROM TblBOMCarcass WHERE CarcassCode LIKE '{0}%' ORDER BY MidCode DESC", prefixCode);
      DataTable dtMiddleCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCBSuffixCode, dtMiddleCode, "MidCode", "MidCode");
    }

    private void LoadUltraDDCarcassRelation()
    {
      string commandText = string.Format("Select CarcassCode, Description From TblBOMCarcass Where (DeleteFlag = 0 Or DeleteFlag Is Null)");
      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDCarcassCode.DataSource = dtCarcass;
      ultraDDCarcassCode.ValueMember = "CarcassCode";
      ultraDDCarcassCode.DisplayMember = "CarcassCode";
      ultraDDCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadUltraCBCarcassReference()
    {
      string commandText = string.Format("Select CarcassCode, Description, (CarcassCode + '  |  ' + Description) DisplayText From TblBOMCarcass Where (DeleteFlag = 0 Or DeleteFlag Is Null)");
      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBFromCarcass.DataSource = dtCarcass;
      ultraCBFromCarcass.ValueMember = "CarcassCode";
      ultraCBFromCarcass.DisplayMember = "DisplayText";
      ultraCBFromCarcass.DisplayLayout.Bands[0].Columns["CarcassCode"].MinWidth = 120;
      ultraCBFromCarcass.DisplayLayout.Bands[0].Columns["CarcassCode"].MaxWidth = 120;
      ultraCBFromCarcass.DisplayLayout.Bands[0].Columns["DisplayText"].Hidden = true;
      ultraCBFromCarcass.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadCBCarcassDirect()
    {
      string commandText = string.Format("Select CarcassCode, Description, (CarcassCode + '  |  ' + Description) DisplayText From TblBOMCarcass Where (DeleteFlag = 0 Or DeleteFlag Is Null)");
      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBCarcassDirectCopy.DataSource = dtCarcass;
      ultCBCarcassDirectCopy.ValueMember = "CarcassCode";
      ultCBCarcassDirectCopy.DisplayMember = "DisplayText";
      ultCBCarcassDirectCopy.DisplayLayout.Bands[0].Columns["CarcassCode"].MinWidth = 120;
      ultCBCarcassDirectCopy.DisplayLayout.Bands[0].Columns["CarcassCode"].MaxWidth = 120;
      ultCBCarcassDirectCopy.DisplayLayout.Bands[0].Columns["DisplayText"].Hidden = true;
      ultCBCarcassDirectCopy.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadDataCarcassInfo()
    {
      string commandText = string.Format("Select CarcassCode, OldCode, [Description], DescriptionVN, TecNote, ContractOut, TechConfirm Confirm, FastCostingFlag FROM TblBOMCarcass	WHERE CarcassCode = '{0}'", this.carcassCode);
      DataTable dtCarcassInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCarcassInfo != null && dtCarcassInfo.Rows.Count > 0)
      {
        DataRow row = dtCarcassInfo.Rows[0];
        txtCarcassCode.Text = this.carcassCode;
        picCarcass.ImageLocation = FunctionUtility.BOMGetCarcassImage(this.carcassCode);
        txtOldCode.Text = row["OldCode"].ToString();
        txtDescriptionEN.Text = row["Description"].ToString();
        txtDescriptionVN.Text = row["DescriptionVN"].ToString();
        //Tien Add
        txtTecNote.Text = row["TecNote"].ToString();
        //End
        chkContract.Checked = (DBConvert.ParseInt(row["ContractOut"].ToString()) == 1);
        chkSelectAllContractOut.Visible = chkContract.Checked;
        if (row["FastCostingFlag"].ToString().Equals("1"))
        {
          rdFastCosting.Checked = true;
        }
        else if (row["FastCostingFlag"].ToString().Equals("0"))
        {
          rdBOM.Checked = true;
        }
        ultraCBPrefixCode.ReadOnly = true;
        ultraCBSuffixCode.ReadOnly = true;
        btnGetCode.Enabled = false;
        this.confirmed = (DBConvert.ParseInt(row["Confirm"].ToString()) == 1);
      }
    }

    private void LoadCarcassRelationship()
    {
      string commandText = string.Format("Select Pid, CarcassRelative, Description From TblBOMCarcassRelative Where CarcassCode = '{0}'", this.carcassCode);
      DataTable dtRelation = DataBaseAccess.SearchCommandTextDataTable(commandText);
      dtRelation.PrimaryKey = new DataColumn[] { dtRelation.Columns["CarcassRelative"] };
      ultraGridRelationship.DataSource = dtRelation;
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

    private void LoadDataComponentList()
    {
      dsBOMCarcassCompMultiLevel dsComponent = new dsBOMCarcassCompMultiLevel();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spBOMListCarcassComponentInfo", inputParam);
      if (dsSource != null)
      {
        dsComponent.Tables["ComponentInfo"].Merge(dsSource.Tables[0]);
        dsComponent.Tables["ComponentMaterial"].Merge(dsSource.Tables[1]);
      }
      ultraGridComponentList.DataSource = dsComponent;
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
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
    }

    private void LoadDropdownMaterialName(UltraDropDown udrpMaterials)
    {
      string commandText = @"SELECT MaterialCode, MaterialName, MaterialNameVn, IDFactoryUnit, FactoryUnit 
                            FROM VBOMMaterialsForCarcassComponent WHERE Used = 1 ORDER BY MaterialName";
      DataTable dtSourceMaterials = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpMaterials.DataSource = dtSourceMaterials;
      udrpMaterials.ValueMember = "MaterialName";
      udrpMaterials.DisplayMember = "MaterialName";
      udrpMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpMaterials.DisplayLayout.Bands[0].Columns["IDFactoryUnit"].Hidden = true;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
    }

    private void LoadDropdownDirectLabour()
    {
      //Load data Dropdown Labour            
      string commandText = "Select Code, NameEN From VBOMSectionForCarcass ORDER BY Code";
      DataTable dtLabour = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDLabour.DataSource = dtLabour;
      ultraDDLabour.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDLabour.DisplayLayout.Bands[0].Columns[0].Width = 150;
      ultraDDLabour.DisplayLayout.Bands[0].Columns[1].Width = 400;
    }

    private void LoadDataCarcassDirectLabour()
    {
      string commandText = "SELECT LB.Pid, LB.SectionCode, SEC.NameEN, LB.Qty, LB.ContractOutQty, LB.[Description], LB.Remark ";
      commandText += "FROM TblBOMCarcassDirectLabour LB LEFT JOIN VBOMSection SEC ON LB.SectionCode = SEC.Code ";
      commandText += string.Format("WHERE LB.CarcassCode = '{0}' ORDER BY LB.SectionCode", this.carcassCode);
      DataTable dtDirectLabour = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraGridDirectLabour.DataSource = dtDirectLabour;
    }

    private void LoadUltraGridParentComp()
    {
      long componentPid = long.MinValue;
      if (chkShowParent.Checked)
      {
        ultraGridParentComp.Visible = true;
        if (ultraGridComponentList.Selected.Rows.Count > 0)
        {
          componentPid = DBConvert.ParseLong(ultraGridComponentList.Selected.Rows[0].Cells["Pid"].Value.ToString());
          string compCode = ultraGridComponentList.Selected.Rows[0].Cells["ComponentCode"].Value.ToString();
          ultraGridParentComp.Text = string.Format("Parent Components List of Component '{0}'", compCode);
        }
        else
        {
          ultraGridParentComp.Text = string.Empty;
        }
      }
      else
      {
        ultraGridParentComp.Visible = false;
      }
      string commandText = string.Format(
                              @"SELECT PARENT.ComponentCode, PARENT.DescriptionVN, STRUCT.Qty
                              FROM TblBOMCarcassComponentStruct STRUCT INNER JOIN TblBOMCarcassComponent PARENT ON STRUCT.MainCompPid = PARENT.Pid
                              WHERE SubCompPid = '{0}'", componentPid);
      ultraGridParentComp.DataSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
    }

    private void LoadTabData()
    {
      // Load Tab Data Component
      string tabPageName = tabControlComponent.SelectedTab.Name;
      switch (tabPageName)
      {
        case "tabPageComponentList":
          this.LoadDataComponentList();
          break;
        case "tabPageComponentStruct":
          Utility.LoadDataCarcassComponentStruct(treeViewComponentStruct, this.carcassCode);
          break;
        case "tabPageDirectLabour":
          this.LoadDataCarcassDirectLabour();
          break;
        default:
          break;
      }
    }

    private void LoadData()
    {
      this.listCompDeletedPid = new ArrayList();
      this.listDirectLabourDeletedPid = new ArrayList();
      this.listRelationDeletedPid = new ArrayList();
      // Load Carcass Information
      this.LoadDataCarcassInfo();
      // Load Carcass Relationship
      this.LoadCarcassRelationship();
      this.LoadTabData();
      this.SetStatusControl();
      this.isCanUpdate = (!this.confirmed && btnSave.Visible);
      chkReferenceFromFastCosting.Enabled = this.isCanUpdate;
      this.NeedToSave = false;
    }
    #endregion Load Data

    #region Check and Save Data
    private bool CheckInvalid()
    {
      string carcass = txtCarcassCode.Text.Trim();
      if (carcass.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "The Carcass Code");
        return false;
      }
      else
      {
        // If carcass is checked "Contract Out" then it must have "contract out data"
        if (chkContract.Checked)
        {
          string checkContractOutData = string.Format(@"SELECT COUNT(*) FROM TblBOMCarcassContractOutInfo A
					                                                INNER JOIN VBOMMaterials B ON A.ContractOut = B.MaterialCode
						                                                AND B.StandardCost > 0 AND A.[Default] = 1 AND A.CarcassCode = '{0}'", carcass);
          if (DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(checkContractOutData)) <= 0)
          {
            WindowUtinity.ShowMessageErrorFromText("You can't select contract out as default for this carcass because carcass Subcon don't have price.");
            return false;
          }
        }
      }
      string description = txtDescriptionEN.Text.Trim();
      if (description.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "The EN Description");
        return false;
      }
      description = txtDescriptionVN.Text.Trim();
      if (description.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "The VN Description");
        return false;
      }

      string tabPageName = tabControlComponent.SelectedTab.Name;
      switch (tabPageName)
      {
        case "tabPageComponentList":
          // Kiểm tra trường hợp component được chỉ định root component có là con của ai không
          for (int i = 0; i < ultraGridComponentList.Rows.Count; i++)
          {
            long pid = DBConvert.ParseLong(ultraGridComponentList.Rows[i].Cells["Pid"].Value.ToString());
            int isMainComp = DBConvert.ParseInt(ultraGridComponentList.Rows[i].Cells["IsMainComp"].Value.ToString());
            string compCode = ultraGridComponentList.Rows[i].Cells["ComponentCode"].Value.ToString();
            int qty = DBConvert.ParseInt(ultraGridComponentList.Rows[i].Cells["Qty"].Value.ToString());
            if (isMainComp == 1)
            {
              if (pid != long.MinValue)
              {
                string commandText = string.Format("SELECT COUNT(Pid) FROM TblBOMCarcassComponentStruct WHERE SubCompPid = {0}", pid);
                object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
                if (obj != null && (int)obj > 0)
                {
                  WindowUtinity.ShowMessageError("ERR0100", compCode);
                  return false;
                }
              }
              if (qty <= 0)
              {
                WindowUtinity.ShowMessageError("MSG0005", string.Format("Qty at row {0}", i + 1));
                return false;
              }
            }

            // Kiểm tra Unit
            for (int k = 0; k < ultraGridComponentList.Rows[i].ChildBands[0].Rows.Count; k++)
            {
              UltraGridRow childRow = ultraGridComponentList.Rows[i].ChildBands[0].Rows[k];
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
          }
          break;
        case "tabPageDirectLabour":
          DataTable dtSource = (DataTable)ultraGridDirectLabour.DataSource;
          for (int i = 0; i < ultraGridDirectLabour.Rows.Count; i++)
          {
            if (ultraGridDirectLabour.Rows[i].Cells["SectionCode"].Value.ToString().Length == 0)
            {
              WindowUtinity.ShowMessageError("MSG0005", string.Format("Section Code at row {0}", i + 1));
              return false;
            }
            DataRow[] count = dtSource.Select(string.Format("SectionCode = '{0}'", ultraGridDirectLabour.Rows[i].Cells["SectionCode"].Value.ToString().Trim()));
            if (count.Length > 1)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0006", "Section Code");
              return false;
            }

            double qty = DBConvert.ParseDouble(ultraGridDirectLabour.Rows[i].Cells["Qty"].Value.ToString());
            if (qty <= 0)
            {
              WindowUtinity.ShowMessageError("MSG0005", string.Format("Qty at row {0}", i + 1));
              return false;
            }
            if (ultraGridDirectLabour.Rows[i].Cells["ContractOutQty"].Value.ToString().Trim().Length > 0)
            {
              qty = DBConvert.ParseDouble(ultraGridDirectLabour.Rows[i].Cells["ContractOutQty"].Value.ToString());
              if (qty <= 0)
              {
                WindowUtinity.ShowMessageError("MSG0005", string.Format("Contract Out Qty at row {0}", i + 1));
                return false;
              }
            }
          }
          break;
        default:
          break;
      }
      return true;
    }

    private DBParameter[] SetCarcassComponentParam(DBParameter[] param, UltraGridRow row)
    {
      param[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);
      //string text = row.Cells["ComponentCode"].Value.ToString().Trim().Replace("'", "''");
      string text = row.Cells["ComponentCode"].Value.ToString().Trim();
      if (text.Length > 0)
      {
        param[2] = new DBParameter("@ComponentCode", DbType.AnsiString, 32, text);
      }
      //text = row.Cells["DescriptionVN"].Value.ToString().Trim().Replace("'", "''");
      text = row.Cells["DescriptionVN"].Value.ToString().Trim();
      param[3] = new DBParameter("@DescriptionVN", DbType.String, 256, text);
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

      int value1 = DBConvert.ParseInt(row.Cells["CriticalComponent"].Value.ToString());
      if (value1 != int.MinValue)
      {
        param[19] = new DBParameter("@Critical", DbType.Int32, value1);
      }
      string remark = row.Cells["Remark"].Value.ToString().Trim();
      if (remark.Length > 0)
      {
        param[20] = new DBParameter("@Remark", DbType.String, 512, remark);
      }
      string oldCode = row.Cells["OldCode"].Value.ToString().Trim();
      if (oldCode.Length > 0)
      {
        param[21] = new DBParameter("@OldCode", DbType.AnsiString, 64, oldCode);
      }
      return param;
    }

    private DBParameter[] SetComponentDetailParam(DBParameter[] param, UltraGridRow row)
    {
      //string text = row.Cells["MaterialCode"].Value.ToString().Replace("'", "''");
      string text = row.Cells["MaterialCode"].Value.ToString();
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
      //text = row.Cells["Alternative"].Value.ToString().Replace("'", "''");
      text = row.Cells["Alternative"].Value.ToString();
      if (text.Length > 0)
      {
        param[7] = new DBParameter("@Alternative", DbType.AnsiString, 16, text);
      }

      return param;
    }

    private bool SaveCarcassInfo()
    {
      DBParameter[] inputParam = new DBParameter[9];
      string storeName = string.Empty;
      if (this.carcassCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);
        inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spBOMCarcass_Update";
      }
      else
      {
        string prefix = string.Empty;
        string middle = string.Empty;
        if (ultraCBPrefixCode.SelectedRow != null)
        {
          prefix = ultraCBPrefixCode.SelectedRow.Cells["PrefixCode"].Value.ToString();
        }
        else
        {
          return false;
        }
        if (ultraCBSuffixCode.SelectedRow != null)
        {
          middle = ultraCBSuffixCode.SelectedRow.Cells["MidCode"].Value.ToString();
        }

        inputParam[0] = new DBParameter("@Prefix", DbType.AnsiString, 16, prefix);
        if (middle.Length > 0)
        {
          inputParam[7] = new DBParameter("@Middle", DbType.AnsiString, 16, middle);
        }
        inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        storeName = "spBOMCarcass_Insert";
      }
      string description = txtDescriptionEN.Text.Trim();
      if (description.Length > 0)
      {
        //inputParam[1] = new DBParameter("@Description", DbType.AnsiString, 128, description.Replace("'", "''"));
        inputParam[1] = new DBParameter("@Description", DbType.AnsiString, 128, description);
      }
      //Tien Add
      string tecnote = txtTecNote.Text.Trim();
      if (tecnote.Length > 0)
      {
        inputParam[8] = new DBParameter("@TecNote", DbType.AnsiString, 512, tecnote);
      }
      //End
      description = txtDescriptionVN.Text.Trim();
      if (description.Length > 0)
      {
        //inputParam[2] = new DBParameter("@DescriptionVN", DbType.String, 128, description.Replace("'", "''"));
        inputParam[2] = new DBParameter("@DescriptionVN", DbType.String, 128, description);
      }
      inputParam[3] = new DBParameter("@DeleteFlag", DbType.Int32, 0);
      int value = (chkContract.Checked) ? 1 : 0;
      inputParam[4] = new DBParameter("@ContractOut", DbType.Int32, value);
      int fastCosting;
      //Nếu đã confirm (đã tạo cấu trúc đầy đủ cho carcass) thì không còn tính theo fast costing nữa
      if (chkConfirm.Checked)
      {
        fastCosting = 0;
      }
      else
      {
        fastCosting = (rdFastCosting.Checked ? 1 : 0);
      }
      inputParam[5] = new DBParameter("@FastCostingFlag", DbType.Int32, fastCosting);
      //File : Không save field file mà là TECH tự chép vào thu mục theo 1 qui ước nhất định
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 16, string.Empty) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      this.carcassCode = outputParam[0].Value.ToString();
      if (this.carcassCode.Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return false;
      }
      return true;
    }

    private bool SaveCarcassRelation()
    {
      bool success = true;
      //Delete relation
      foreach (long deletePid in listRelationDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, deletePid) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassRelative_Delete", inputDelete, outputDelete);
        if (outputDelete[0].Value.ToString().Equals("0"))
        {
          success = false;
        }
      }
      //Insert, update relation
      DataTable dtCarcassRelation = (DataTable)ultraGridRelationship.DataSource;
      foreach (DataRow row in dtCarcassRelation.Rows)
      {
        if ((row.RowState == DataRowState.Added) || (row.RowState == DataRowState.Modified))
        {
          DBParameter[] inputParam = new DBParameter[4];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          string storeName = string.Empty;
          if (row.RowState == DataRowState.Modified)
          {
            storeName = "spBOMCarcassRelative_Update";
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, row["Pid"]);
          }
          else
          {
            storeName = "spBOMCarcassRelative_Insert";
          }
          inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);
          inputParam[2] = new DBParameter("@CarcassRelative", DbType.AnsiString, 16, row["CarcassRelative"]);
          if (row["Description"].ToString().Trim().Length > 0)
          {
            inputParam[3] = new DBParameter("@Description", DbType.String, 256, row["Description"].ToString().Trim());
          }
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          if (outputParam[0].Value.Equals(0))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private bool SaveComponentList()
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
      // Delete Component
      foreach (long pid in this.listCompDeletedPid)
      {
        DBParameter[] inputParams = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponent_Delete", inputParams, outputParams);
        if (DBConvert.ParseInt(outputParams[0].Value.ToString()) == 0)
        {
          result = false;
        }
      }
      // Insert/Update Component
      int countComponent = ultraGridComponentList.Rows.Count;
      for (int i = 0; i < countComponent; i++)
      {
        UltraGridRow row = ultraGridComponentList.Rows[i];
        // 1. Save CarcassComponentInfo
        string storeName = string.Empty;
        DBParameter[] inputParam = new DBParameter[22];
        long componentPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (componentPid == long.MinValue)
        {
          storeName = "spBOMCarcassComponent_Insert";
          inputParam[17] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        else
        {
          int rowState = DBConvert.ParseInt(row.Cells["RowState"].Value.ToString());
          if (rowState == 1)
          {
            storeName = "spBOMCarcassComponent_Update";
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, componentPid);
            inputParam[17] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
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

    private bool SaveDirectLabour()
    {
      bool result = true;
      // Delete
      foreach (long pid in this.listDirectLabourDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassDirectLabour_Delete", inputDelete, outputDelete);
        if (DBConvert.ParseInt(outputDelete[0].Value.ToString()) != 1)
        {
          result = false;
        }
      }
      //Insert / Update
      DataTable dtDirectLabour = (DataTable)ultraGridDirectLabour.DataSource;
      foreach (DataRow row in dtDirectLabour.Rows)
      {
        if ((row.RowState == DataRowState.Added) || (row.RowState == DataRowState.Modified))
        {
          string storeName = string.Empty;
          DBParameter[] inputParam = new DBParameter[8];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          if (row.RowState == DataRowState.Added)
          {
            storeName = "spBOMCarcassDirectLabour_Insert";
            inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          else
          {
            storeName = "spBOMCarcassDirectLabour_Update";
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, row["Pid"]);
            inputParam[7] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);
          inputParam[2] = new DBParameter("@SectionCode", DbType.AnsiString, 50, row["SectionCode"].ToString().Trim());
          inputParam[3] = new DBParameter("@Qty", DbType.Double, row["Qty"]);
          double contractOutQty = DBConvert.ParseDouble(row["ContractOutQty"].ToString());
          if (contractOutQty != double.MinValue)
          {
            inputParam[4] = new DBParameter("@ContractOutQty", DbType.Double, row["ContractOutQty"]);
          }
          if (row["Description"].ToString().Trim().Length > 0)
          {
            inputParam[5] = new DBParameter("@Description", DbType.String, 512, row["Description"]);
          }
          if (row["Remark"].ToString().Trim().Length > 0)
          {
            inputParam[6] = new DBParameter("@Remark", DbType.String, 512, row["Remark"]);
          }
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          if (outputParam[0].Value.ToString().Equals("0"))
          {
            result = false;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Confirm to carcass, if have some component not use, the carcass will be not confirm
    /// </summary>
    /// <returns></returns>
    private string ConfirmToCarcass()
    {
      if (chkConfirm.Checked)
      {
        if (chkContract.Checked)
        {
          Shared.Utility.FunctionUtility.CopyImageForSubcon(this.carcassCode);
        }
        //end copy hinh

        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode);
        inputParam[1] = new DBParameter("@Confirm", DbType.Int32, 1);
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.AnsiString, 256, string.Empty) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcass_Confirm", 300, inputParam, outputParam);

        return outputParam[0].Value.ToString();
      }
      return string.Empty;
    }

    private bool SaveData()
    {
      bool success = true;
      if (this.CheckInvalid())
      {
        if (this.SaveCarcassInfo())
        {
          //Save carcass relationship
          success = this.SaveCarcassRelation();

          string tabPageName = tabControlComponent.SelectedTab.Name;
          switch (tabPageName)
          {
            case "tabPageComponentList":
              if (!this.SaveComponentList())
              {
                success = false;
              }
              break;
            case "tabPageDirectLabour":
              if (!this.SaveDirectLabour())
              {
                success = false;
              }
              break;
            default:
              break;
          }

          string componentNotUsed = this.ConfirmToCarcass();
          if (componentNotUsed.Length > 0)
          {
            success = false;
            this.SaveSuccess = false;
            WindowUtinity.ShowMessageError("ERR0074", componentNotUsed);
          }
          else
          {
            if (success)
            {
              WindowUtinity.ShowMessageSuccess("MSG0004");
              this.SaveSuccess = true;
            }
            else
            {
              WindowUtinity.ShowMessageError("WRN0004");
              this.SaveSuccess = false;
            }
          }
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
          this.SaveSuccess = false;
          success = false;
        }
      }
      else
      {
        success = false;
        this.SaveSuccess = false;
      }
      return success;
    }
    #endregion Check and Save Data
    #endregion function

    #region event
    public viewBOM_01_010()
    {
      InitializeComponent();
    }

    private void viewBOM_01_010_Load(object sender, EventArgs e)
    {
      Utility.LoadComboboxPrefix(ultraCBPrefixCode, Shared.Utility.ConstantClass.PREFIX_CARCASS);
      try
      {
        ultraCBPrefixCode.Value = "CAR";
      }
      catch { }
      this.LoadUltraDDCarcassRelation();
      this.LoadUltraCBCarcassReference();
      this.LoadCBCarcassDirect();
      //Specify
      this.LoadDropdownCodeMst(UltraDDSpecify, ConstantClass.GROUP_COMPONENTSPECIFY);
      //Status
      this.LoadDropdownCodeMst(UltraDDStatus, ConstantClass.GROUP_COMPONENTSTATUS);
      // MaterialsCode
      this.LoadDropdownMaterial(udrpMaterialsCode);
      // MaterialsName
      this.LoadDropdownMaterialName(udrpMaterialsName);
      // Alternative
      this.LoadDropdownMaterial(udrpAlternative);
      // Load Direct Labour
      this.LoadDropdownDirectLabour();
      this.LoadData();
    }

    private void ultraGridRelationship_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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
          this.listRelationDeletedPid.Add(pid);
        }
      }
    }

    private void ultraGridRelationship_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (colName.Equals("CarcassRelative"))
      {
        string carcassRelative = e.Cell.Text.Trim();
        if (carcassRelative.Length > 0)
        {
          string commandText = string.Format("Select CarcassCode From TblBOMCarcass Where (DeleteFlag = 0 Or DeleteFlag Is Null) And CarcassCode = '{0}'", carcassRelative);
          DataTable dtCheckCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheckCarcass == null || dtCheckCarcass.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Carcass Relation");
            e.Cancel = true;
          }
        }
      }
      this.DataChanged();
    }

    private void btnGetCode_Click(object sender, EventArgs e)
    {
      txtCarcassCode.Text = this.GetNewCarcassCode();
    }

    private void ultraCBPrefixCode_ValueChanged(object sender, EventArgs e)
    {
      if (ultraCBPrefixCode.SelectedRow != null)
      {
        string prefixCode = ultraCBPrefixCode.SelectedRow.Cells["PrefixCode"].Value.ToString();
        this.LoadUltraCBSuffixCode(prefixCode);
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void tabControlComponent_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.LoadTabData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridComponentList_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          else
          {
            this.listCompDeletedPid.Add(pid);
          }
        }
      }
      this.DataChanged();
    }

    private void ultraGridComponentList_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
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

    private void ultraGridComponentList_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      int unitCode = int.MinValue;
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
          if (this.isUpdateComponent)
          {
            this.isUpdateComponent = false;
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
            try
            {
              unitCode = DBConvert.ParseInt(udrpMaterialsCode.SelectedRow.Cells["IDFactoryUnit"].Value.ToString());
            }
            catch { }
            this.SetStatusRow(e.Cell.Row, unitCode);
            this.isUpdateComponent = true;
          }
          break;
        case "materialname":
          if (this.isUpdateComponent)
          {
            this.isUpdateComponent = false;
            try
            {
              e.Cell.Row.Cells["MaterialCode"].Value = udrpMaterialsName.SelectedRow.Cells["MaterialCode"].Value;
            }
            catch
            {
              e.Cell.Row.Cells["MaterialCode"].Value = DBNull.Value;
            }

            try
            {
              e.Cell.Row.Cells["FactoryUnit"].Value = udrpMaterialsName.SelectedRow.Cells["FactoryUnit"].Value;
            }
            catch
            {
              e.Cell.Row.Cells["FactoryUnit"].Value = DBNull.Value;
            }
            try
            {
              e.Cell.Row.Cells["IDFactoryUnit"].Value = udrpMaterialsName.SelectedRow.Cells["IDFactoryUnit"].Value;
            }
            catch
            {
              e.Cell.Row.Cells["IDFactoryUnit"].Value = DBNull.Value;
            }
            this.SetDefaultDimension(e.Cell.Row);
            try
            {
              unitCode = DBConvert.ParseInt(udrpMaterialsName.SelectedRow.Cells["IDFactoryUnit"].Value.ToString());
            }
            catch { }
            this.SetStatusRow(e.Cell.Row, unitCode);
            this.isUpdateComponent = true;
          }
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
      this.DataChanged();
    }

    private void ultraGridComponentList_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultraGridComponentList.Selected.Rows.Count > 0)
      {
        UltraGridRow row = ultraGridComponentList.Selected.Rows[0];
        if (row.ParentRow == null)
        {
          long componentPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (componentPid != long.MinValue)
          {
            if (rdProperties.Checked)
            {
              viewBOM_01_012 view = new viewBOM_01_012();
              view.componentPid = componentPid;
              view.carcassCode = this.carcassCode;
              WindowUtinity.ShowView(view, "Component Properties", true, ViewState.Window);
            }
            else
            {
              viewBOM_01_005 view = new viewBOM_01_005();
              view.componentPid = componentPid;
              view.carConfirm = this.confirmed;// Ha Anh them thuoc tinh carcass confirm de show cho user thay
              WindowUtinity.ShowView(view, "Routing Ticket", true, ViewState.Window, FormWindowState.Maximized);
            }
          }
        }
      }
    }

    #region InitializeLayout
    private void ultraCBPrefixCode_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
    }

    private void ultraCBSuffixCode_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
    }

    private void ultraGridRelationship_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["CarcassRelative"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CarcassRelative"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["CarcassRelative"].MinWidth = 200;
      e.Layout.Bands[0].Columns["CarcassRelative"].ValueList = ultraDDCarcassCode;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Override.AllowDelete = (this.confirmed ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
      e.Layout.Bands[0].Override.AllowAddNew = (this.confirmed ? Infragistics.Win.UltraWinGrid.AllowAddNew.No : Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom);
      e.Layout.Bands[0].Override.AllowUpdate = (this.confirmed ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
    }

    private void ultraGridComponentList_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Override.HeaderClickAction = HeaderClickAction.SortMulti;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RowState"].Hidden = true;
      e.Layout.Bands[0].Columns["Primary"].Hidden = true;

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FIN_Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FIN_Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["FIN_Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Comp Code";
      e.Layout.Bands[0].Columns["FingerJoin"].Header.Caption = "Finger Join";
      e.Layout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "VN Description";
      e.Layout.Bands[0].Columns["IsMainComp"].Header.Caption = "Root";
      e.Layout.Bands[0].Columns["IsCompStore"].Header.Caption = "Store";
      e.Layout.Bands[0].Columns["FIN_Length"].Header.Caption = "Length";
      e.Layout.Bands[0].Columns["FIN_Width"].Header.Caption = "Width";
      e.Layout.Bands[0].Columns["FIN_Thickness"].Header.Caption = "Thick";
      e.Layout.Bands[0].Columns["Lamination"].Header.Caption = "La";
      e.Layout.Bands[0].Columns["FingerJoin"].Header.Caption = "FJ";
      e.Layout.Bands[0].Columns["CriticalComponent"].Header.Caption = "Critical";

      e.Layout.Bands[0].Columns["No"].MinWidth = 35;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 35;
      e.Layout.Bands[0].Columns["ComponentCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ComponentCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["DescriptionVN"].MinWidth = 200;
      e.Layout.Bands[0].Columns["DescriptionVN"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["FIN_Length"].MinWidth = 55;
      e.Layout.Bands[0].Columns["FIN_Length"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["FIN_Width"].MinWidth = 55;
      e.Layout.Bands[0].Columns["FIN_Width"].MaxWidth = 55;
      e.Layout.Bands[0].Columns["FIN_Thickness"].MinWidth = 55;
      e.Layout.Bands[0].Columns["FIN_Thickness"].MaxWidth = 55;
      //e.Layout.Bands[0].Columns["Lamination"].MinWidth = 40;
      //e.Layout.Bands[0].Columns["Lamination"].MaxWidth = 40;
      //e.Layout.Bands[0].Columns["FingerJoin"].MinWidth = 40;
      //e.Layout.Bands[0].Columns["FingerJoin"].MaxWidth = 40;
      //e.Layout.Bands[0].Columns["Specify"].MinWidth = 50;
      //e.Layout.Bands[0].Columns["Specify"].MaxWidth = 50;
      //e.Layout.Bands[0].Columns["Status"].MinWidth = 50;
      //e.Layout.Bands[0].Columns["Status"].MaxWidth = 50;
      //e.Layout.Bands[0].Columns["Primary"].MinWidth = 50;
      //e.Layout.Bands[0].Columns["Primary"].MaxWidth = 50;
      //e.Layout.Bands[0].Columns["Waste"].MinWidth = 50;
      //e.Layout.Bands[0].Columns["Waste"].MaxWidth = 50;
      //e.Layout.Bands[0].Columns["IsMainComp"].MinWidth = 65;
      //e.Layout.Bands[0].Columns["IsMainComp"].MaxWidth = 65;
      //e.Layout.Bands[0].Columns["ContractOut"].MinWidth = 75;
      //e.Layout.Bands[0].Columns["ContractOut"].MaxWidth = 75;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 35;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 35;
      //e.Layout.Bands[0].Columns["IsCompStore"].MinWidth = 50;
      //e.Layout.Bands[0].Columns["IsCompStore"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Lamination"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["FingerJoin"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Primary"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsMainComp"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsCompStore"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CriticalComponent"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Specify"].ValueList = UltraDDSpecify;
      e.Layout.Bands[0].Columns["Status"].ValueList = UltraDDStatus;

      e.Layout.Bands[0].Override.AllowDelete = (this.confirmed ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
      e.Layout.Bands[0].Override.AllowAddNew = (this.confirmed ? Infragistics.Win.UltraWinGrid.AllowAddNew.No : Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom);
      e.Layout.Bands[0].Override.AllowUpdate = (this.confirmed ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);

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
      e.Layout.Bands[1].Columns["MaterialName"].ValueList = udrpMaterialsName;
      e.Layout.Bands[1].Columns["Alternative"].ValueList = udrpAlternative;

      //e.Layout.Bands[1].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["FactoryUnit"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Override.AllowAddNew = (this.confirmed ? Infragistics.Win.UltraWinGrid.AllowAddNew.No : Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom);
      e.Layout.Bands[1].Override.AllowDelete = (this.confirmed ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
      e.Layout.Bands[1].Override.AllowUpdate = (this.confirmed ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
      e.Layout.Bands[1].Columns["MaterialName"].Width = 250;

      for (int k = 0; k < ultraGridComponentList.Rows.Count; k++)
      {
        UltraGridRow row = ultraGridComponentList.Rows[k];
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
        int isMainComp = DBConvert.ParseInt(ultraGridComponentList.Rows[k].Cells["IsMainComp"].Value.ToString());
        if (isMainComp == 1)
        {
          ultraGridComponentList.Rows[k].Cells["Qty"].Activation = Activation.AllowEdit;
          ultraGridComponentList.Rows[k].CellAppearance.BackColor = Color.LightGray;
        }
        else
        {
          ultraGridComponentList.Rows[k].Cells["Qty"].Activation = Activation.ActivateOnly;
        }
      }
    }

    private void ultraGridDirectLabour_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      //Gan Dropdown vao grid        
      e.Layout.Bands[0].Columns["SectionCode"].ValueList = ultraDDLabour;

      //Edit lai Grid        
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["SectionCode"].Header.Caption = "Section Code";
      e.Layout.Bands[0].Columns["SectionCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["NameEN"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Qty (man*hour)";
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ContractOutQty"].Header.Caption = "Contract Out Qty";
      e.Layout.Bands[0].Columns["ContractOutQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ContractOutQty"].MaxWidth = 100;

      e.Layout.Bands[0].Override.AllowDelete = (this.confirmed ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
      e.Layout.Bands[0].Override.AllowAddNew = (this.confirmed ? Infragistics.Win.UltraWinGrid.AllowAddNew.No : Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom);
      e.Layout.Bands[0].Override.AllowUpdate = (this.confirmed ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
    }

    private void UltraDDStatus_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Value"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Value"].MaxWidth = 70;
    }
    #endregion InitializeLayout

    #region ToolStripMenuItem event
    /// <summary>
    /// Open root component window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void addRootComponentToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (this.carcassCode.Length > 0)
      {
        viewBOM_01_011 view = new viewBOM_01_011();
        view.carcassCode = this.carcassCode;
        Shared.Utility.WindowUtinity.ShowView(view, "Carcass Information", true, ViewState.ModalWindow);
        Utility.LoadDataCarcassComponentStruct(treeViewComponentStruct, this.carcassCode);
      }
    }

    private void deleteRootComponentToolStripMenuItem_Click(object sender, EventArgs e)
    {
      TreeNode node = treeViewComponentStruct.SelectedNode;
      if (node != null)
      {
        long pidRootComp = DBConvert.ParseLong(node.Name);
        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@PidRootComp", DbType.Int64, pidRootComp);
        inputParam[1] = new DBParameter("@IsMainComp", DbType.Int32, 0);
        inputParam[2] = new DBParameter("@Qty", DbType.Int32, 0);
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponent_UpdateRootComp", inputParam, outputParam);
        if (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
        else
        {
          Utility.LoadDataCarcassComponentStruct(treeViewComponentStruct, this.carcassCode);
        }
      }
    }

    private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (treeViewComponentStruct.SelectedNode != null)
      {
        TreeNode node = treeViewComponentStruct.SelectedNode;
        viewBOM_01_012 view = new viewBOM_01_012();
        view.componentPid = DBConvert.ParseLong(node.Name);
        view.carcassCode = this.carcassCode;
        WindowUtinity.ShowView(view, "Component Properties", true, ViewState.Window);
      }
    }
    #endregion ToolStripMenuItem event

    #region component struct tab (tree node)
    private void treeViewComponentStruct_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        TreeNode node = treeViewComponentStruct.GetNodeAt(treeViewComponentStruct.PointToClient(Cursor.Position));
        treeViewComponentStruct.SelectedNode = node;
        this.SetStatusTreeNodeMenu(node);
      }
    }

    private void treeViewComponentStruct_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyValue == 93)
      {
        TreeNode node = treeViewComponentStruct.SelectedNode;
        this.SetStatusTreeNodeMenu(node);
      }
    }
    #endregion component struct tab (tree node)

    private void ultraGridDirectLabour_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      int index = e.Cell.Row.Index;
      if (columnName == "SectionCode")
      {
        if (ultraDDLabour.SelectedRow != null)
        {
          ultraGridDirectLabour.Rows[index].Cells["NameEN"].Value = ultraDDLabour.SelectedRow.Cells["NameEN"].Value.ToString();
        }
        else
        {
          ultraGridDirectLabour.Rows[index].Cells["NameEN"].Value = DBNull.Value;
        }
      }
      this.DataChanged();
    }

    private void ultraGridDirectLabour_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (colName.Equals("SectionCode"))
      {
        string code = e.Cell.Text.ToString().Trim();
        if (code.Length > 0)
        {
          string commandText = string.Format("Select Code From VBOMSection Where Code = '{0}'", code);
          DataTable dtLabour = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtLabour == null || dtLabour.Rows.Count == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", colName);
            e.Cancel = true;
          }
        }
      }
    }

    private void ultraGridDirectLabour_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listDirectLabourDeletedPid.Add(pid);
        }
      }
      this.DataChanged();
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.NeedToSave = (this.isCanUpdate ? true : false);
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    private void tabControlComponent_Deselecting(object sender, TabControlCancelEventArgs e)
    {
      if (this.NeedToSave && WindowUtinity.ShowMessageConfirm("MSG0008") == DialogResult.Yes && !this.SaveData())
      {
        e.Cancel = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// Fix qty of components per carcass
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFixQty_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode) };
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spBOMFixQtyCompPerCarcass", inputParam, outputParam);
      if (outputParam[0].Value.ToString() == "1")
      {
        WindowUtinity.ShowMessageSuccessFromText("Fixed qty of components per carcass success!");
      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText("Fixed qty of components per carcass error!");
      }
      this.LoadData();
    }

    private void chkShowImageComp_CheckedChanged(object sender, EventArgs e)
    {
      this.ShowImageComp();
    }

    private void ultraGridComponentList_MouseClick(object sender, MouseEventArgs e)
    {
      this.ShowImageComp();
    }

    private void ultraGridComponentList_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
    {
      this.LoadUltraGridParentComp();
    }

    private void chkShowParent_CheckedChanged(object sender, EventArgs e)
    {
      this.LoadUltraGridParentComp();
    }

    private void ultraGridParentComp_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Parent Comp";
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "Description";
      e.Layout.Bands[0].Columns["ComponentCode"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ComponentCode"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 50;
    }

    private void btnDeleteAll_Click(object sender, EventArgs e)
    {
      if (WindowUtinity.ShowMessageConfirmFromText("Are you sure to delete all data of this carcass?") == DialogResult.Yes)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CarcassCode", DbType.AnsiString, 16, this.carcassCode) };
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcass_DeleteAll", inputParam, outputParam);
        if (outputParam[0].Value.ToString().Equals("1"))
        {
          WindowUtinity.ShowMessageSuccess("MSG0002");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0004");
        }
      }
      this.LoadData();
    }

    private void chkReference_CheckedChanged(object sender, EventArgs e)
    {
      groupReference.Visible = chkReference.Checked;
    }
    /// <summary>
    /// Copy components, Struct, Process, Materials of carcass
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCopy_Click(object sender, EventArgs e)
    {
      if (ultraCBFromCarcass.SelectedRow != null)
      {
        string fromCarcass = ultraCBFromCarcass.Value.ToString();
        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@FromCarcassCode", DbType.AnsiString, 16, fromCarcass);
        inputParam[1] = new DBParameter("@ToCarcassCode", DbType.AnsiString, 16, this.carcassCode);
        inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMReferenceCarcass", inputParam, outputParam);
        if (outputParam[0].Value.ToString().Equals("1"))
        {
          WindowUtinity.ShowMessageSuccess("MSG0046");
        }
        else
        {
          WindowUtinity.ShowMessageErrorFromText("Copy carcass error.");
        }
        this.LoadTabData();
      }
      else
      {
        WindowUtinity.ShowMessageError("MSG0011", "Carcass you want to copy");
      }
      this.SetStatusControl();
    }

    private void chkReferenceFromFastCosting_CheckedChanged(object sender, EventArgs e)
    {
      if (chkReferenceFromFastCosting.Checked)
      {
        if (WindowUtinity.ShowMessageConfirmFromText("Are you sure to make direct labour reference from fast costing?") == DialogResult.Yes)
        {
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@FromCarcassCode", DbType.AnsiString, 16, this.carcassCode);
          inputParam[1] = new DBParameter("@ToCarcassCode", DbType.AnsiString, 16, this.carcassCode);
          inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMReferenceDirectLabourCarcassFromFastCosting", inputParam, outputParam);
          if ((int)outputParam[0].Value == 1)
          {
            WindowUtinity.ShowMessageSuccessFromText("Reference success!");
          }
          else
          {
            WindowUtinity.ShowMessageErrorFromText("Reference Error!");
          }
          this.LoadTabData();
        }
      }
    }

    private void chkContract_CheckedChanged(object sender, EventArgs e)
    {
      this.NeedToSave = (this.isCanUpdate ? true : false);
      chkSelectAllContractOut.Visible = chkContract.Checked;
    }

    private void chkSelectAllContractOut_CheckedChanged(object sender, EventArgs e)
    {
      int contractOut = chkSelectAllContractOut.Checked ? 1 : 0;
      for (int i = 0; i < ultraGridComponentList.Rows.Count; i++)
      {
        ultraGridComponentList.Rows[i].Cells["ContractOut"].Value = contractOut;
      }
    }

    private void btnCopyLabor_Click(object sender, EventArgs e)
    {
      if (ultCBCarcassDirectCopy.SelectedRow != null)
      {
        string carcassCode = ultCBCarcassDirectCopy.Value.ToString().Trim();
        string commandText = string.Format(@"SELECT LB.Pid, LB.SectionCode, SEC.NameEN, LB.Qty, LB.ContractOutQty, LB.[Description], LB.Remark
                                             FROM TblBOMCarcassDirectLabour LB LEFT JOIN VBOMSection SEC ON LB.SectionCode = SEC.Code
                                             WHERE LB.CarcassCode = '{0}' ORDER BY LB.SectionCode", carcassCode);
        DataTable dtDirectLabour = DataBaseAccess.SearchCommandTextDataTable(commandText);
        DataTable dtSource = (DataTable)ultraGridDirectLabour.DataSource;
        if (dtSource == null)
        {
          if (dtDirectLabour.Columns.Contains("Pid"))
          {
            dtDirectLabour.Columns.Remove("Pid");
            dtDirectLabour.Columns.Add("Pid", typeof(System.Int64));
          }
          dtSource = dtDirectLabour.Copy();
        }
        else
        {
          foreach (DataRow row in dtSource.Rows)
          {
            if (row.RowState != DataRowState.Deleted)
            {
              long pidDelete = DBConvert.ParseLong(row["Pid"].ToString());
              if (pidDelete != long.MinValue)
              {
                this.listDirectLabourDeletedPid.Add(pidDelete);
              }
            }
          }
          dtSource.Rows.Clear();
          foreach (DataRow directRow in dtDirectLabour.Rows)
          {
            directRow["Pid"] = DBNull.Value;
            DataRow newRow = dtSource.NewRow();
            newRow.ItemArray = directRow.ItemArray;
            dtSource.Rows.Add(newRow);
          }
        }
        ultraGridDirectLabour.DataSource = dtSource;
      }
    }

    private void ultraGridComponentList_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      if (e.Cell.Row.ParentRow == null)
      {
        string value = e.Cell.Row.Cells["No"].Value.ToString();
        if (value.Length == 0)
        {
          DataTable dtComp = ((DataSet)ultraGridComponentList.DataSource).Tables["ComponentInfo"];
          int maxNo = 1;
          if (dtComp.Rows.Count > 0)
          {
            maxNo = DBConvert.ParseInt(dtComp.Compute("Max(No)", "")) + 1;
          }
          e.Cell.Row.Cells["No"].Value = maxNo;
        }
      }
    }
    #endregion event
  }
}
