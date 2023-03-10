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
  public partial class viewPLN_07_012 : MainUserControl
  {
    #region fields
    private bool isLoadingData = false;
    private int materialControlType = 1; //Theo Item, Material
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
      btnSearch.Enabled = false;
      long wo = long.MinValue;
      if (ultraCBWO.SelectedRow != null)
      {
        wo = DBConvert.ParseLong(ultraCBWO.SelectedRow.Cells[0].Value.ToString());
      }
      string materialCode = txtMaterialCode.Text.Trim();

      DBParameter[] inputParam = new DBParameter[4];
      string storeName = string.Empty;
      if (wo != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
      }
      if (this.materialControlType == 1) //Theo WO, Item
      {
        storeName = "spPLNAllocateAdvanceForWorkOrderAndItemInfomation";
        if (txtItemCode.Text.Trim().Length > 0)
        {
          inputParam[1] = new DBParameter("@ItemCodes", DbType.AnsiString, 4000, txtItemCode.Text.Trim());
        }
      }
      else if (this.materialControlType == 0)
      {
        storeName = "spPLNAllocateAdvanceForWorkOrderInformation";
      }
      if (txtMaterialGroup.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, txtMaterialGroup.Text.Trim());
      }
      if (txtMaterialCode.Text.Trim().Length > 0)
      {
        inputParam[3] = new DBParameter("@Materials", DbType.AnsiString, 4000, materialCode);
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, 600, inputParam);
      ultraGridWOMaterialDetail.DataSource = dtSource;
      chkSelectedAll.Checked = false;
      btnSearch.Enabled = true;
    }

    private bool CheckInvalid()
    {
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        if (ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value.ToString().Trim().Length > 0)
        {
          double allocateQty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value.ToString());
          if (allocateQty == double.MinValue || allocateQty == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", string.Format("Allocate Qty at row {0}", i + 1));
            ultraGridWOMaterialDetail.Rows[i].Selected = true;
            return false;
          }
          else
          {
            double remainQty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Remain"].Value.ToString());
            if (allocateQty > 0)
            {
              if (allocateQty - remainQty > 1)
              {
                WindowUtinity.ShowMessageError("ERR0010", "Allocate Qty - Remain Qty", "1");
                return false;
              }
            }
            // Truong hop re-allocate (Qty re-allocate < Required + Issued)
            if (allocateQty < 0)
            {
              double requiredQty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Required"].Value.ToString());
              double issuedQty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Issued"].Value.ToString());
              double allocatedQty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Allocated"].Value.ToString());
              double supplementQty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Supplement"].Value.ToString());
              requiredQty = (requiredQty == double.MinValue ? 0 : requiredQty);
              issuedQty = (issuedQty == double.MinValue ? 0 : issuedQty);
              allocatedQty = (allocatedQty == double.MinValue ? 0 : allocatedQty);
              supplementQty = (supplementQty == double.MinValue ? 0 : supplementQty);
              double canReallocateQty = allocatedQty + supplementQty - requiredQty - issuedQty;

              if (allocateQty + canReallocateQty < 0)
              {
                WindowUtinity.ShowMessageError("ERR0010", "Re-Allocate + Supplement Qty", "Required + Issued");
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    private bool CheckInvalidReAllocate()
    {
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        if (ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value.ToString().Trim().Length > 0)
        {
          double allocateQty = -1 * DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value.ToString());
          if (allocateQty == double.MinValue || allocateQty == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", string.Format("Allocate Qty at row {0}", i + 1));
            ultraGridWOMaterialDetail.Rows[i].Selected = true;
            return false;
          }
          else
          {
            double remainQty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Remain"].Value.ToString());
            if (allocateQty > 0)
            {
              if (allocateQty - remainQty > 1)
              {
                WindowUtinity.ShowMessageError("ERR0010", "Allocate Qty - Remain Qty", "1");
                return false;
              }
            }
            // Truong hop re-allocate (Qty re-allocate < Required + Issued)
            if (allocateQty < 0)
            {
              double requiredQty = Math.Round(DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Required"].Value.ToString()), 4);
              double issuedQty = Math.Round(DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Issued"].Value.ToString()), 4);
              double allocatedQty = Math.Round(DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Allocated"].Value.ToString()), 4);
              double supplementQty = Math.Round(DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Supplement"].Value.ToString()), 4);
              requiredQty = (requiredQty == double.MinValue ? 0 : requiredQty);
              issuedQty = (issuedQty == double.MinValue ? 0 : issuedQty);
              allocatedQty = (allocatedQty == double.MinValue ? 0 : allocatedQty);
              supplementQty = (supplementQty == double.MinValue ? 0 : supplementQty);
              double canReallocateQty = allocatedQty + supplementQty - requiredQty - issuedQty;

              if (allocateQty + canReallocateQty < 0)
              {
                WindowUtinity.ShowMessageError("ERR0010", "Re-Allocate + Supplement Qty", "Required + Issued");
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    private bool SaveDataReAllocate()
    {
      bool success = true;
      for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        double qty = DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value.ToString());
        if (qty != double.MinValue)
        {
          qty = (-1) * qty;
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
    public viewPLN_07_012()
    {
      InitializeComponent();
    }

    private void viewPLN_07_012_Load(object sender, EventArgs e)
    {
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

      if (this.ViewParam == "viewPLN_07_012_01") //Theo WO
      {
        this.materialControlType = 0;
        lbItemCode.Visible = false;
        chkShowItemListBox.Visible = false;
        txtItemCode.Visible = false;
      }
      else if (this.ViewParam == "viewPLN_07_012_02") //Theo WO, Item
      {
        this.materialControlType = 1;
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
      this.Search();
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
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Required"].MinWidth = 55;
      ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns["Required"].MaxWidth = 55;
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

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

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
          if (qty > 0)
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
          double qty = 0;
          qty = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
          if (qty > 0)
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

    private void linkDeletedMaterials_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      viewPLN_07_014 view = new viewPLN_07_014();
      view.materialControlType = this.materialControlType;
      WindowUtinity.ShowView(view, "Deleted Materials Allocation Information", false, ViewState.Window, FormWindowState.Maximized);
    }

    private void ultraCBWO_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void btnReallocate_Click(object sender, EventArgs e)
    {
      if (this.CheckInvalidReAllocate())
      {
        if (this.SaveDataReAllocate())
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
    private void chkAutoReAllocate_CheckedChanged(object sender, EventArgs e)
    {
      if (chkAutoReAllocate.Checked)
      {
        for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
        {
          if (Math.Round(DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["NonIssue"].Value), 4) > 0.0001)
          {
            ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value = Math.Round(DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["NonIssue"].Value), 4) - 0.0001;
          }
        }
      }
      else
      {
        for (int i = 0; i < ultraGridWOMaterialDetail.Rows.Count; i++)
        {
          if (DBConvert.ParseDouble(ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value) > 0)
          {
            ultraGridWOMaterialDetail.Rows[i].Cells["Allocate"].Value = DBNull.Value;
          }
        }
      }
    }
    #endregion event
  }
}
