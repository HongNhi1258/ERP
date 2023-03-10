/*
 * Created By   : Nguyen Van Tron
 * Created Date : 14/12/2011
 * Description  : Allocate Materials follow WO or WO, Item
 * */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_014 : MainUserControl
  {
    #region fields
    private bool isLoadingData = false;
    public int materialControlType = 1; //Theo Item, Material
    #endregion fields

    #region function
    private void LoadData_ucUltraListItem()
    {
      txtItemCode.Text = string.Empty;
      string commandText;
      if (ultraCBWO.SelectedRow != null)
      {
        // Load ListView Material Group
        long wo = DBConvert.ParseLong(ultraCBWO.SelectedRow.Cells[0].Value.ToString());
        commandText = string.Format("Select Distinct ITEM.ItemCode, ITEM.Name ItemName From TblPLNWorkOrderConfirmedDetails WO INNER JOIN TblBOMItemBasic ITEM On WO.ItemCode = ITEM.ItemCode And WO.WorkOrderPid = {0} Order By ITEM.ItemCode", wo);
      }
      else
      {
        commandText = "Select Distinct ITEM.ItemCode, ITEM.Name ItemName From TblPLNWorkOrderConfirmedDetails WO INNER JOIN TblBOMItemBasic ITEM On WO.ItemCode = ITEM.ItemCode Order By ITEM.ItemCode";
      }
      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucUltraListItem.DataSource = dtItem;
      ucUltraListItem.ColumnWidths = "100; 200";
      ucUltraListItem.DataBind();
      ucUltraListItem.ValueMember = "ItemCode";
    }

    private void LoadData_ucUltraListMaterial()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, txtMaterialGroup.Text);
      inputParam[1] = new DBParameter("@ControlType", DbType.Int32, materialControlType); // = 1: Theo Wo, Item
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNGetListControlMaterial", inputParam);
      dtSource.Columns.Remove("Description");
      ucUltraListMaterial.DataSource = dtSource;
      ucUltraListMaterial.ColumnWidths = "100; 200";
      ucUltraListMaterial.DataBind();
      ucUltraListMaterial.ValueMember = "MaterialCode";
      txtMaterialCode.Text = string.Empty;
    }

    private string GetSelectedListView(ListView lst)
    {
      string result = string.Empty;
      foreach (ListViewItem item in lst.Items)
      {
        if (result.Length > 0)
        {
          result += "; ";
        }
        result += item.Text;
      }
      return result;
    }

    private void Search()
    {
      // Check Invalid
      long wo = long.MinValue;
      if (ultraCBWO.SelectedRow != null)
      {
        wo = DBConvert.ParseLong(ultraCBWO.SelectedRow.Cells[0].Value.ToString());
      }
      string materialCode = txtMaterialCode.Text.Trim();
      //if (wo == long.MinValue && materialCode.Length == 0)
      //{
      //  WindowUtinity.ShowMessageError("WRN0013", "Work Order Or Marerial Code");
      //  return;
      //}

      DBParameter[] inputParam = new DBParameter[4];
      string storeName = string.Empty;
      if (wo != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
      }
      if (this.materialControlType == 1) //Theo WO, Item
      {
        storeName = "spPLNAllocateAdvanceForWorkOrderAndItemWereDeleted";
        if (txtItemCode.Text.Trim().Length > 0)
        {
          inputParam[1] = new DBParameter("@ItemCodes", DbType.AnsiString, 4000, txtItemCode.Text.Trim());
        }
      }
      else if (this.materialControlType == 0)
      {
        storeName = "spPLNAllocateAdvanceForWorkOrderWereDeleted";
      }
      if (txtMaterialGroup.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, txtMaterialGroup.Text.Trim());
      }
      if (txtMaterialCode.Text.Trim().Length > 0)
      {
        inputParam[3] = new DBParameter("@Materials", DbType.AnsiString, 4000, materialCode);
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridWOMaterialDetail.DataSource = dtSource;
      chkSelectedAll.Checked = false;
    }

    private bool CheckInvalid()
    {
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        if (ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value.ToString().Trim().Length > 0)
        {
          double allocateQty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value.ToString());
          if (allocateQty == double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0001", string.Format("Allocate Qty at row {0}", i + 1));
            ultraGridWOMaterialDetail.Rows[i].Selected = true;
            return false;
          }
          else
          {
            double remainQty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Remain"].Value.ToString());
            if (allocateQty - remainQty > 1)
            {
              WindowUtinity.ShowMessageError("ERR0010", "Allocate Qty - Remain Qty", "1");
              return false;
            }
          }
        }
      }
      return true;
    }

    private bool SaveData()
    {
      bool success = true;
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        double qty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value.ToString());
        if (qty != double.MinValue)
        {
          string materialCode = ultraGridWOMaterialDetail.Rows[i].Cells["MaterialCode"].Value.ToString();
          long wo = DBConvert.ParseLong(ultraGridWOMaterialDetail.Rows[i].Cells["Wo"].Value.ToString());
          DBParameter[] inputParam = new DBParameter[6];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          inputParam[0] = new DBParameter("@WoPid", DbType.Int64, wo);
          if (materialControlType == 1) // Theo WO, Item
          {
            string itemCode = ultraGridWOMaterialDetail.Rows[i].Cells["ItemCode"].Value.ToString();
            int revision = DBConvert.ParseInt(ultraGridWOMaterialDetail.Rows[i].Cells["Revision"].Value.ToString());
            inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
            inputParam[2] = new DBParameter("@Revision", DbType.Int32, revision);
          }
          inputParam[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
          inputParam[4] = new DBParameter("@Qty", DbType.Double, qty);
          inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spPLNAllocateAdvanceForWorkOrder_Insert", inputParam, outputParam);
          int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            success = false;
          }
        }
      }
      return success;
    }

    /// <summary>
    /// Đối với 1 số đơn vị như sheet, pia, pc,... thì khi register phải làm tròn số nguyên lớn hơn số nguyên ở cột "remain". 
    /// (Ex: 0.225 --> 1; 1.013 --> 2)
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    private double RoundUp(double value, string unit)
    {
      if ((Shared.Utility.ConstantClass.ROUND_UNIT.IndexOf(string.Format("|{0}|", unit.ToLower())) >= 0) && (value > 0))
      {
        return DBConvert.ParseDouble(DBConvert.ParseString(value + 1).Split('.')[0]);
      }
      return value;
    }
    #endregion function

    #region event
    public viewPLN_07_014()
    {
      InitializeComponent();
    }

    private void viewPLN_07_014_Load(object sender, EventArgs e)
    {
      #region demo
      //DataTable dtSource = new DataTable();
      //dtSource.Columns.Add("WO");
      //dtSource.Columns.Add("Item Code");
      //dtSource.Columns.Add("Rev.");
      //dtSource.Columns.Add("Item Qty");
      //dtSource.Columns.Add("Material Code");
      //dtSource.Columns.Add("Material Name");
      //dtSource.Columns.Add("Material/Item");
      //dtSource.Columns.Add("Total Qty");
      //dtSource.Columns.Add("Allocated");
      //dtSource.Columns.Add("Supplement");
      //dtSource.Columns.Add("Issued");
      //dtSource.Columns.Add("Non-Issue");
      //dtSource.Columns.Add("Remain");
      //dtSource.Columns.Add("Non-Allocate");
      //dtSource.Columns.Add("Stock");
      //dtSource.Columns.Add("Auto");
      //dtSource.Columns.Add("Allocate");
      //dtSource.Columns.Add("Unit");
      //DataRow row1 = dtSource.NewRow();
      //row1["WO"] = "254";
      //row1["Item Code"] = "001006-AA";
      //row1["Rev."] = 1;
      //row1["Item Qty"] = 7;
      //row1["Material Code"] = "010-01-00017";
      //row1["Material Name"] = "MDF CARB 3mmx1220x2440";
      //row1["Material/Item"] = 0.768075;
      //row1["Total Qty"] = 5.376525;
      //row1["Allocated"] = 2;
      //row1["Supplement"] = 0;
      //row1["Issued"] = 0;
      //row1["Non-Issue"] = 2;
      //row1["Remain"] = 3.376525;
      //row1["Non-Allocate"] = 8;
      //row1["Stock"] = 10;
      //row1["Auto"] = 0;
      //row1["Allocate"] = string.Empty;
      //row1["Unit"] = "Sheet";
      //dtSource.Rows.Add(row1);

      //DataRow row2 = dtSource.NewRow();
      //row2["WO"] = "254";
      //row2["Item Code"] = "001006-AA";
      //row2["Rev."] = 1;
      //row2["Item Qty"] = 7;
      //row2["Material Code"] = "010-01-00018";
      //row2["Material Name"] = "MDF CARB 6mmx1220x2440";
      //row2["Material/Item"] = 0.127993;
      //row2["Total Qty"] = 0.895948;
      //row2["Allocated"] = 0;
      //row2["Supplement"] = 0;
      //row2["Issued"] = 0;
      //row2["Non-Issue"] = 0;
      //row2["Remain"] = 0.895948;
      //row2["Non-Allocate"] = 3;
      //row2["Stock"] = 3;
      //row2["Auto"] = 0;
      //row2["Allocate"] = string.Empty;
      //row2["Unit"] = "Sheet";
      //dtSource.Rows.Add(row2);

      //ultraGridWOMaterialDetail.DataSource = dtSource;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["WO"].MinWidth = 40;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["WO"].MaxWidth = 40;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Item Code"].MinWidth = 70;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Item Code"].MaxWidth = 70;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Item Qty"].MinWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Item Qty"].MaxWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Rev."].MinWidth = 40;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Rev."].MaxWidth = 40;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Supplement"].MinWidth = 70;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Supplement"].MaxWidth = 70;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Issued"].MinWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Material/Item"].MaxWidth = 72;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Material/Item"].MinWidth = 72;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Total Qty"].MaxWidth = 60;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Total Qty"].MinWidth = 60;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Issued"].MaxWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Remain"].MinWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Remain"].MaxWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Non-Issue"].MinWidth = 60;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Non-Issue"].MaxWidth = 60;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Non-Allocate"].MinWidth = 72;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Non-Allocate"].MaxWidth = 72;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Auto"].MinWidth = 40;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Auto"].MaxWidth = 40;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Unit"].MinWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Unit"].MaxWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Allocate"].MinWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Allocate"].MaxWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Allocated"].MinWidth = 55;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Allocated"].MaxWidth = 55;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Stock"].MinWidth = 50;
      //ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Stock"].MaxWidth = 50;
      #endregion demo

      this.isLoadingData = true;
      // Load data for WO
      string commandText = "Select Pid From TblPLNWorkOrder Order By Pid DESC";
      DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBWO.DataSource = dtWO;
      ultraCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      // Load Data For Items
      this.LoadData_ucUltraListItem();
      // Load Data For Material Group
      ControlUtility.LoaducUltraListMaterialGroup(ucUltraListMaterialGroup);

      if (this.materialControlType == 0) //Theo WO
      {
        lbItemCode.Visible = false;
        chkShowItemListBox.Visible = false;
        txtItemCode.Visible = false;
      }
      else if (this.materialControlType == 1) //Theo WO, Item
      {
        lbItemCode.Visible = true;
        chkShowItemListBox.Visible = true;
        txtItemCode.Visible = true;
      }
      this.isLoadingData = false;
    }

    private void chkShowItemListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListItem.Visible = chkShowItemListBox.Checked;
    }

    private void chkShowMaterialListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListMaterialGroup.Visible = chkShowMaterialListBox.Checked;
    }

    private void chkShowMaterialCodeListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListMaterial.Visible = chkShowMaterialCodeListBox.Checked;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultraGridWOMaterialDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["WO"].MinWidth = 40;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["WO"].MaxWidth = 40;
      if (this.materialControlType == 1)
      {
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["ItemCode"].MinWidth = 70;
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["ItemCode"].MaxWidth = 70;
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Revision"].Header.Caption = "Rev.";
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Revision"].MinWidth = 40;
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Revision"].MaxWidth = 40;
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["ItemQty"].Header.Caption = "Item Qty";
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["ItemQty"].MinWidth = 50;
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["ItemQty"].MaxWidth = 50;
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["MatQtyPerItem"].Header.Caption = "Material/Item";
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["MatQtyPerItem"].MaxWidth = 72;
        ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["MatQtyPerItem"].MinWidth = 72;
      }
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["TotalMaterial"].Header.Caption = "Total Qty";
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["TotalMaterial"].MaxWidth = 60;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["TotalMaterial"].MinWidth = 60;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Allocated"].MinWidth = 55;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Allocated"].MaxWidth = 55;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Supplement"].MinWidth = 70;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Supplement"].MaxWidth = 70;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Issued"].MinWidth = 50;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Issued"].MaxWidth = 50;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Remain"].MinWidth = 55;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Remain"].MaxWidth = 55;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["NonIssue"].Header.Caption = "Non-Issue";
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["NonIssue"].MinWidth = 60;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["NonIssue"].MaxWidth = 60;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["NonAllocate"].Header.Caption = "Non-Allocate";
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["NonAllocate"].MinWidth = 72;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["NonAllocate"].MaxWidth = 72;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Stock"].MinWidth = 50;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Stock"].MaxWidth = 50;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Auto"].MinWidth = 40;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Auto"].MaxWidth = 40;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Unit"].MinWidth = 50;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Unit"].MaxWidth = 50;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Allocate"].MinWidth = 55;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Allocate"].MaxWidth = 55;


      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Auto"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Allocate"].CellActivation = Activation.AllowEdit;
    }

    private void ultraCBWO_ValueChanged(object sender, EventArgs e)
    {
      if (!this.isLoadingData)
      {
        this.LoadData_ucUltraListItem();
      }
    }

    private void ultraGridWOMaterialDetail_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        if (ultraGridWOMaterialDetail.Selected.Rows.Count > 0)
        {
          viewPLN_07_005 view = new viewPLN_07_005();
          long wo = DBConvert.ParseLong(ultraGridWOMaterialDetail.Selected.Rows[0].Cells["Wo"].Value.ToString());
          string material = ultraGridWOMaterialDetail.Selected.Rows[0].Cells["MaterialCode"].Value.ToString();
          if (this.materialControlType == 1)
          {
            string itemCode = ultraGridWOMaterialDetail.Selected.Rows[0].Cells["ItemCode"].Value.ToString();
            int revision = DBConvert.ParseInt(ultraGridWOMaterialDetail.Selected.Rows[0].Cells["Revision"].Value.ToString());
            view.itemCode = itemCode;
            view.revision = revision;
          }
          view.wo = wo;
          view.materialCode = material;
          view.materialControlType = this.materialControlType;
          WindowUtinity.ShowView(view, "Allocation Information", true, ViewState.Window);
        }
      }
      catch
      {
        WindowUtinity.ShowMessageError("MSG0011", "the row you want to allocate or register");
      }
    }

    private void ultraGridWOMaterialDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (string.Compare("Auto", colName, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Value.ToString());
        if (select == 1)
        {
          double qty = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
          if (qty != double.MinValue)
          {
            e.Cell.Row.Cells["Allocate"].Value = qty;
          }
        }
        else if (select == 0)
        {
          e.Cell.Row.Cells["Allocate"].Value = DBNull.Value;
        }
      }
    }

    private void ultraGridWOMaterialDetail_CellChange(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (string.Compare("Auto", colName, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Text.ToString());
        if (select == 1)
        {
          double qty = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
          if (qty != double.MinValue)
          {
            e.Cell.Row.Cells["Allocate"].Value = qty;
          }
        }
        else if (select == 0)
        {
          e.Cell.Row.Cells["Allocate"].Value = DBNull.Value;
        }
      }
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
        this.Search();
      }
    }

    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = (chkSelectedAll.Checked ? 1 : 0);
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        ultraGridWOMaterialDetail.Rows[i].Cells["Auto"].Value = selected;
      }
    }

    static public void releaseObject(object obj)
    {
      try
      {
        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        obj = null;
      }
      catch (Exception ex)
      {
        obj = null;
        throw new Exception("Exception Occured while releasing object " + ex.ToString());
      }
      finally
      {
        GC.Collect();
      }
    }

    private void ucUltraListItem_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtItemCode.Text = ucUltraListItem.SelectedValue;
    }

    private void ucUltraListMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialGroup.Text = ucUltraListMaterialGroup.SelectedValue;
      this.LoadData_ucUltraListMaterial();
    }

    private void ucUltraListMaterial_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialCode.Text = ucUltraListMaterial.SelectedValue;
    }

    private void btnExportToExcel_Click(object sender, EventArgs e)
    {
      btnExportToExcel.Enabled = false;

      string strTemplateName = "PlanningReport";
      string strSheetName = string.Empty;
      if (this.materialControlType == 0)
      {
        strSheetName = "AllocationWO";
      }
      else
      {
        strSheetName = "AllocationWOAndItem";
      }
      string strOutFileName = "Material Allocation Information";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Search :
      DataTable dtData = (DataTable)ultraGridWOMaterialDetail.DataSource;
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B6:Q6").Copy();
            oXlsReport.RowInsert(5 + i);
            oXlsReport.Cell("B6:Q6", 0, i).Paste();
          }
          oXlsReport.Cell("**Wo", 0, i).Value = dtRow["Wo"];
          if (this.materialControlType == 1)
          {
            oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"];
            oXlsReport.Cell("**Revision", 0, i).Value = dtRow["Revision"];
            oXlsReport.Cell("**ItemQty", 0, i).Value = dtRow["ItemQty"];
            oXlsReport.Cell("**MaterialPerItem", 0, i).Value = dtRow["MatQtyPerItem"];
          }
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"];
          oXlsReport.Cell("**MaterialName", 0, i).Value = dtRow["MaterialName"];
          oXlsReport.Cell("**TotalQty", 0, i).Value = dtRow["TotalMaterial"];
          oXlsReport.Cell("**Allocated", 0, i).Value = dtRow["Allocated"];
          oXlsReport.Cell("**Supplement", 0, i).Value = dtRow["Supplement"];
          oXlsReport.Cell("**Issued", 0, i).Value = dtRow["Issued"];
          oXlsReport.Cell("**NonIssue", 0, i).Value = dtRow["NonIssue"];
          oXlsReport.Cell("**Remain", 0, i).Value = dtRow["Remain"];
          oXlsReport.Cell("**NonAllocate", 0, i).Value = dtRow["NonAllocate"];
          oXlsReport.Cell("**Stock", 0, i).Value = dtRow["Stock"];
          oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"];
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);

      btnExportToExcel.Enabled = true;
    }
    #endregion event
  }
}
