using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_01_012 : MainUserControl
  {
    #region fields
    public long componentPid = long.MinValue;
    public string carcassCode = string.Empty;
    private int mainComp = 0;
    private int confirmed = 0;
    private IList listSubCompDeletedPid;
    private IList listMaterialDeletedPid;
    private bool isCanUpdate;
    #endregion fields

    #region function
    #region Load Data
    private void LoadDropdownMaterial(UltraDropDown udrpMaterials)
    {
      string commandText = "SELECT MaterialCode, MaterialName, MaterialNameVn, IDFactoryUnit, FactoryUnit FROM VBOMMaterialsForCarcassComponent ORDER BY MaterialCode";
      DataTable dtSourceMaterials = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpMaterials.DataSource = dtSourceMaterials;
      udrpMaterials.ValueMember = "MaterialCode";
      udrpMaterials.DisplayMember = "MaterialCode";
      udrpMaterials.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpMaterials.DisplayLayout.Bands[0].Columns["IDFactoryUnit"].Hidden = true;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialCode"].MinWidth = 110;
      udrpMaterials.DisplayLayout.Bands[0].Columns["MaterialCode"].MaxWidth = 110;
    }

    private void LoadData()
    {
      this.listSubCompDeletedPid = new ArrayList();
      this.listMaterialDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@PidComponent", DbType.Int64, this.componentPid) };
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spBOMCarcassComponentStructInfo", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 2)
      {
        DataTable dtCompInfo = dsSource.Tables[0];
        if (dtCompInfo.Rows.Count > 0)
        {
          // Load component info
          DataRow row = dtCompInfo.Rows[0];
          txtComponentDescription.Text = row["ComponentDescription"].ToString();
          txtCarcassCode.Text = this.carcassCode;
          txtCarcassDescription.Text = row["CarcassDescription"].ToString();
          int qty = DBConvert.ParseInt(row["Qty"].ToString());
          this.mainComp = DBConvert.ParseInt(row["IsMainComp"].ToString());

          this.confirmed = DBConvert.ParseInt(row["Confirm"].ToString());

          // Load sub component list
          DataTable dtSubCompList = dsSource.Tables[1];
          dtSubCompList.PrimaryKey = new DataColumn[] { dtSubCompList.Columns["SubCompPid"] };
          ultraGridSubCompList.DataSource = dtSubCompList;

          // Load material of component
          DataTable dtMaterial = dsSource.Tables[2];
          ultraGridMaterialInfo.DataSource = dtMaterial;

          this.SetStatusControl();
          this.isCanUpdate = (!(this.confirmed == 1) && btnSave.Visible);
        }
      }
      this.NeedToSave = false;
    }

    private void LoadUltraDDSubComponent()
    {
      string commandText = string.Format(@"Select Pid, ComponentCode, DescriptionVN, Length, Width, Thickness 
                                      From TblBOMCarcassComponent 
                                      Where IsNull(IsMainComp, 0) <> 1 And CarcassCode = '{0}' And Pid <> {1}", this.carcassCode, this.componentPid);
      DataTable dtSubComp = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraDDSubComponent.DataSource = dtSubComp;
      ultraDDSubComponent.DisplayMember = "ComponentCode";
      ultraDDSubComponent.ValueMember = "Pid";
    }

    private void SetDefaultDimension(UltraGridRow row)
    {
      string code = row.Cells["MaterialCode"].Value.ToString().Trim().Substring(0, 3);
      string commandText = string.Format("Select Length, Width, Thickness From TblBOMCarcassComponent Where Pid = {0}", this.componentPid);
      DataTable dtDimention = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtDimention != null && dtDimention.Rows.Count > 0)
      {
        double finishLength = DBConvert.ParseDouble(dtDimention.Rows[0]["Length"].ToString());
        double finishWidth = DBConvert.ParseDouble(dtDimention.Rows[0]["Width"].ToString());
        double finishThickness = DBConvert.ParseDouble(dtDimention.Rows[0]["Thickness"].ToString());
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
    #endregion Load Data

    #region Check And Save Data
    private DBParameter[] SetComponentDetailParam(DBParameter[] param, DataRow row)
    {
      string text = row["MaterialCode"].ToString().Replace("'", "''");
      param[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, text);
      double qty = DBConvert.ParseDouble(row["QtyCombine"].ToString());
      if (qty != double.MinValue)
      {
        param[2] = new DBParameter("@Qty", DbType.Double, qty);
      }
      qty = DBConvert.ParseDouble(row["Waste"].ToString());
      if (qty != double.MinValue)
      {
        param[3] = new DBParameter("@Waste", DbType.Double, qty);
      }
      double dimension = DBConvert.ParseDouble(row["RAW_Length"].ToString());
      if (dimension != double.MinValue)
      {
        param[4] = new DBParameter("@Length", DbType.Double, dimension);
      }
      dimension = DBConvert.ParseDouble(row["RAW_Width"].ToString());
      if (dimension != double.MinValue)
      {
        param[5] = new DBParameter("@Width", DbType.Double, dimension);
      }
      dimension = DBConvert.ParseDouble(row["RAW_Thickness"].ToString());
      if (dimension != double.MinValue)
      {
        param[6] = new DBParameter("@Thickness", DbType.Double, dimension);
      }
      text = row["Alternative"].ToString().Replace("'", "''");
      if (text.Length > 0)
      {
        param[7] = new DBParameter("@Alternative", DbType.AnsiString, 16, text);
      }
      return param;
    }

    private bool CheckInvalid()
    {
      for (int i = 0; i < ultraGridSubCompList.Rows.Count; i++)
      {
        long subCompPid = DBConvert.ParseLong(ultraGridSubCompList.Rows[i].Cells["SubCompPid"].Value.ToString());
        int qty = DBConvert.ParseInt(ultraGridSubCompList.Rows[i].Cells["Qty"].Value.ToString());
        // Check Qty
        if (qty <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", string.Format("Qty of sub comp at row {0}", i + 1));
          return false;
        }

        // Check vong lap vo tan
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@PidMainComp", DbType.Int64, this.componentPid);
        inputParam[1] = new DBParameter("@PidSubComp", DbType.Int64, subCompPid);
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCheckStructComp", inputParam, outputParam);
        if (outputParam[0].Value.ToString() == "0")
        {
          string componentCode = ultraGridSubCompList.Rows[i].Cells["SubCompPid"].Text;
          WindowUtinity.ShowMessageError("ERR0001", componentCode);
          return false;
        }

      }

      for (int k = 0; k < ultraGridMaterialInfo.Rows.Count; k++)
      {
        UltraGridRow childRow = ultraGridMaterialInfo.Rows[k];

        string materialCode = childRow.Cells["MaterialCode"].Value.ToString();
        double qty = DBConvert.ParseDouble(childRow.Cells["QtyCombine"].Value.ToString());
        // Check Qty
        if (qty <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", string.Format("Qty of material {0} at row {1}", materialCode, k + 1));
          return false;
        }

        // Kiểm tra Unit
        int unitCode = DBConvert.ParseInt(childRow.Cells["IDFactoryUnit"].Value.ToString());

        if (unitCode == 4)
        { // m
          double length = DBConvert.ParseDouble(childRow.Cells["RAW_Length"].Value.ToString());
          if (length <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Length of material {0}", materialCode));
            return false;
          }
        }
        else if (unitCode == 6)
        { // sqm
          double length = DBConvert.ParseDouble(childRow.Cells["RAW_Length"].Value.ToString());
          if (length <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Length of material {0}", materialCode));
            return false;
          }
          double width = DBConvert.ParseDouble(childRow.Cells["RAW_Width"].Value.ToString());
          if (width <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Width of material {0}", materialCode));
            return false;
          }
        }
        else if (unitCode == 7)
        { // cbm
          double length = DBConvert.ParseDouble(childRow.Cells["RAW_Length"].Value.ToString());
          if (length <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Length of material {0}", materialCode));
            return false;
          }
          double width = DBConvert.ParseDouble(childRow.Cells["RAW_Width"].Value.ToString());
          if (width <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Width of material {0}", materialCode));
            return false;
          }
          double thickness = DBConvert.ParseDouble(childRow.Cells["RAW_Thickness"].Value.ToString());
          if (thickness <= 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", string.Format("Thickness of material {0}", materialCode));
            return false;
          }
        }
      }
      return true;
    }

    private bool SaveData()
    {
      bool success = true;
      //1. Save data sub component of this
      //1.1. Delete sub component      
      foreach (long pid in this.listSubCompDeletedPid)
      {
        DBParameter[] inputParams = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponentStruct_Delete", inputParams, outputParams);
        if (DBConvert.ParseInt(outputParams[0].Value.ToString()) == 0)
        {
          success = false;
        }
      }
      //1.2. Insert/Update sub component
      DataTable dtSubCompList = (DataTable)ultraGridSubCompList.DataSource;
      for (int i = 0; i < dtSubCompList.Rows.Count; i++)
      {
        DataRow subRow = dtSubCompList.Rows[i];
        if (subRow.RowState != DataRowState.Deleted)
        {
          long pid = DBConvert.ParseLong(subRow["Pid"].ToString());
          int no = DBConvert.ParseInt(subRow["No"].ToString());
          long subCompPid = DBConvert.ParseLong(subRow["SubCompPid"].ToString());
          int qty = DBConvert.ParseInt(subRow["Qty"].ToString());

          DBParameter[] inputParam = new DBParameter[5];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          string storeName = string.Empty;
          if (subRow.RowState == DataRowState.Added)
          {
            storeName = "spBOMCarcassComponentStruct_Insert";
          }
          else if (subRow.RowState == DataRowState.Modified)
          {
            storeName = "spBOMCarcassComponentStruct_Update";
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          if (storeName.Length > 0)
          {
            if (no != int.MinValue)
            {
              inputParam[1] = new DBParameter("@No", DbType.Int32, no);
            }
            inputParam[2] = new DBParameter("@MainCompPid", DbType.Int64, this.componentPid);
            inputParam[3] = new DBParameter("@SubCompPid", DbType.Int64, subCompPid);
            inputParam[4] = new DBParameter("@Qty", DbType.Int32, qty);

            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if (DBConvert.ParseLong(outputParam[0].Value.ToString()) == 0)
            {
              success = false;
            }
          }
        }
      }
      //2. Save Material    

      //2.1. Delete Material
      foreach (long pid in this.listMaterialDeletedPid)
      {
        DBParameter[] inputParams = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
        DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spBOMCarcassComponentDetail_Delete", inputParams, outputParams);
        if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
        {
          success = false;
        }
      }
      //2.2. Insert/Update Material
      DataTable dtMaterial = (DataTable)ultraGridMaterialInfo.DataSource;
      for (int j = 0; j < dtMaterial.Rows.Count; j++)
      {
        DataRow rowMaterial = dtMaterial.Rows[j];
        string storeName = string.Empty;
        DBParameter[] inputParamDetail = new DBParameter[9];
        if (rowMaterial.RowState == DataRowState.Added)
        {
          storeName = "spBOMCarcassComponentDetail_Insert";
          inputParamDetail[0] = new DBParameter("@ComponentPid", DbType.Int64, this.componentPid);
          inputParamDetail[8] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        else if (rowMaterial.RowState == DataRowState.Modified)
        {
          storeName = "spBOMCarcassComponentDetail_Update";
          long materialPid = DBConvert.ParseLong(rowMaterial["Pid"].ToString());
          inputParamDetail[0] = new DBParameter("@Pid", DbType.Int64, materialPid);
          inputParamDetail[8] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        }
        if (storeName.Length > 0)
        {
          inputParamDetail = this.SetComponentDetailParam(inputParamDetail, rowMaterial);
          DBParameter[] outputParamDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParamDetail, outputParamDetail);
          long materialPid = DBConvert.ParseLong(outputParamDetail[0].Value.ToString());
          if (materialPid <= 0)
          {
            success = false;
          }
        }
      }
      return success;
    }
    #endregion Check And Save Data

    private void SetStatusControl()
    {
      btnSave.Enabled = !(this.confirmed == 1);
    }

    private void DataChanged()
    {
      this.NeedToSave = (this.isCanUpdate ? true : false);
    }
    #endregion function

    #region event
    public viewBOM_01_012()
    {
      InitializeComponent();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void viewBOM_01_012_Load(object sender, EventArgs e)
    {
      // MaterialsCode
      this.LoadDropdownMaterial(udrpMaterialsCode);
      // Alternative
      this.LoadDropdownMaterial(udrpAlternative);
      // Load UltraCombobox Component
      string commandText = string.Format("Select Pid, ComponentCode, DescriptionVN From TblBOMCarcassComponent Where CarcassCode = '{0}'", this.carcassCode);
      DataTable dtCompList = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCBComponent, dtCompList, "Pid", "ComponentCode", "Pid");

      ultraCBComponent.Value = this.componentPid;
      //this.LoadData();
      //this.LoadUltraDDSubComponent();
    }

    private void ultraDDSubComponent_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Length"].Hidden = true;
      e.Layout.Bands[0].Columns["Width"].Hidden = true;
      e.Layout.Bands[0].Columns["Thickness"].Hidden = true;
    }

    private void ultraGridSubCompList_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["SubCompPid"].ValueList = ultraDDSubComponent;
      e.Layout.Bands[0].Columns["SubCompPid"].Header.Caption = "Sub Comp";
      e.Layout.Bands[0].Columns["DescriptionVN"].Header.Caption = "Description";

      e.Layout.Bands[0].Columns["DescriptionVN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["FIN_Length"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["FIN_Width"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["FIN_Thickness"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Override.AllowAddNew = (this.confirmed == 1 ? Infragistics.Win.UltraWinGrid.AllowAddNew.No : Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom);
      e.Layout.Override.AllowDelete = (this.confirmed == 1 ? DefaultableBoolean.False : DefaultableBoolean.True);
      e.Layout.Override.AllowUpdate = (this.confirmed == 1 ? DefaultableBoolean.False : DefaultableBoolean.True);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalid())
      {
        if (this.SaveData())
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
    }

    private void ultraGridSubCompList_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string col = e.Cell.Column.ToString();
      if (string.Compare("SubCompPid", col, true) == 0)
      {
        string compCode = e.Cell.Text;
        if (compCode.Length > 0)
        {
          string commandText = string.Format("Select Count(Pid) From TblBOMCarcassComponent Where CarcassCode = '{0}' And ComponentCode = '{1}'", this.carcassCode, compCode);
          object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
          if ((obj == null) || ((int)obj == 0))
          {
            WindowUtinity.ShowMessageError("ERR0001", "Sub Comp Code");
            e.Cancel = true;
          }
        }
      }
    }

    private void ultraGridSubCompList_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string col = e.Cell.Column.ToString();
      if (string.Compare("SubCompPid", col, true) == 0)
      {
        if (ultraDDSubComponent.SelectedRow != null)
        {
          e.Cell.Row.Cells["DescriptionVN"].Value = ultraDDSubComponent.SelectedRow.Cells["DescriptionVN"].Value;
          e.Cell.Row.Cells["FIN_Length"].Value = ultraDDSubComponent.SelectedRow.Cells["Length"].Value;
          e.Cell.Row.Cells["FIN_Width"].Value = ultraDDSubComponent.SelectedRow.Cells["Width"].Value;
          e.Cell.Row.Cells["FIN_Thickness"].Value = ultraDDSubComponent.SelectedRow.Cells["Thickness"].Value;
        }
        else
        {
          e.Cell.Row.Cells["DescriptionVN"].Value = DBNull.Value;
          e.Cell.Row.Cells["FIN_Length"].Value = DBNull.Value;
          e.Cell.Row.Cells["FIN_Width"].Value = DBNull.Value;
          e.Cell.Row.Cells["FIN_Thickness"].Value = DBNull.Value;
        }
      }
      this.DataChanged();
    }

    private void ultraGridSubCompList_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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
        if (pid > 0)
        {
          listSubCompDeletedPid.Add(pid);
        }
      }
      this.DataChanged();
    }

    private void ultraGridMaterialInfo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["QtyCombine"].Header.Caption = "Qty Combine";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["FactoryUnit"].Header.Caption = "Factory Unit";

      e.Layout.Bands[0].Columns["RAW_Length"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["RAW_Width"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["RAW_Thickness"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Waste"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["QtyCombine"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["IDFactoryUnit"].Hidden = true;

      e.Layout.Bands[0].Columns["MaterialCode"].ValueList = udrpMaterialsCode;
      e.Layout.Bands[0].Columns["Alternative"].ValueList = udrpAlternative;

      e.Layout.Bands[0].Columns["MaterialName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["FactoryUnit"].CellActivation = Activation.ActivateOnly;

      e.Layout.Override.AllowAddNew = (this.confirmed == 1 ? Infragistics.Win.UltraWinGrid.AllowAddNew.No : Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom);
      e.Layout.Override.AllowDelete = (this.confirmed == 1 ? DefaultableBoolean.False : DefaultableBoolean.True);
      e.Layout.Override.AllowUpdate = (this.confirmed == 1 ? DefaultableBoolean.False : DefaultableBoolean.True);

      for (int i = 0; i < ultraGridMaterialInfo.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridMaterialInfo.Rows[i];
        int unitCode = int.MinValue;
        unitCode = DBConvert.ParseInt(row.Cells["IDFactoryUnit"].Value.ToString());
        this.SetStatusRow(row, unitCode);
      }
    }

    private void ultraGridMaterialInfo_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      if (string.Compare(columnName, "materialcode", true) == 0)
      {
        UltraGridRow row = udrpMaterialsCode.SelectedRow;
        if (row != null)
        {
          e.Cell.Row.Cells["MaterialName"].Value = row.Cells["MaterialName"].Value;
          e.Cell.Row.Cells["FactoryUnit"].Value = row.Cells["FactoryUnit"].Value;
          e.Cell.Row.Cells["IDFactoryUnit"].Value = row.Cells["IDFactoryUnit"].Value;

          this.SetDefaultDimension(e.Cell.Row);
          int unitCode = DBConvert.ParseInt(row.Cells["IDFactoryUnit"].Value.ToString());
          this.SetStatusRow(e.Cell.Row, unitCode);
        }
        else
        {
          e.Cell.Row.Cells["MaterialName"].Value = DBNull.Value;
          e.Cell.Row.Cells["FactoryUnit"].Value = DBNull.Value;
          e.Cell.Row.Cells["IDFactoryUnit"].Value = DBNull.Value;
        }
      }
      this.DataChanged();
    }

    private void ultraGridMaterialInfo_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
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

    private void ultraGridMaterialInfo_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
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
          this.listMaterialDeletedPid.Add(pid);
        }
      }
      this.DataChanged();
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      if (this.CheckInvalid())
      {
        if (this.SaveData())
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SaveSuccess = true;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
          this.SaveSuccess = false;
        }
        this.LoadData();
      }
      else
      {
        this.SaveSuccess = false;
      }
    }

    private void ultraCBComponent_ValueChanged(object sender, EventArgs e)
    {
      if (ultraCBComponent.SelectedRow != null)
      {
        this.componentPid = DBConvert.ParseLong(ultraCBComponent.SelectedRow.Cells["Pid"].Value.ToString());
        this.LoadUltraDDSubComponent();
        this.LoadData();
      }
      else
      {
        // Load component info        
        txtComponentDescription.Text = string.Empty;

        // Load sub component list        
        ultraGridSubCompList.DataSource = null;

        // Load material of component        
        ultraGridMaterialInfo.DataSource = null;

        btnSave.Enabled = false;
        this.isCanUpdate = false;
        this.NeedToSave = false;
      }
    }

    private void ultraCBComponent_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].ColHeadersVisible = false;
    }

    private void txtComponentDescription_TextChanged(object sender, EventArgs e)
    {
      //if (this.NeedToSave && WindowUtinity.ShowMessageConfirm("MSG0008") == DialogResult.Yes)
      //{
      //  if (this.CheckInvalid())
      //  {
      //    if (this.SaveData())
      //    {
      //      WindowUtinity.ShowMessageSuccess("MSG0004");
      //    }
      //    else
      //    {
      //      WindowUtinity.ShowMessageError("WRN0004");
      //      try
      //      {
      //        ultraCBComponent.Value = this.componentPid;
      //        return;
      //      }
      //      catch { }
      //    }
      //  }
      //  else
      //  {
      //    try
      //    {
      //      ultraCBComponent.Value = this.componentPid;
      //      return;
      //    }
      //    catch { }
      //  }
      //}
    }
    #endregion event
  }
}
