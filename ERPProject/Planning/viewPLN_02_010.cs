/*
  Author      : Ha Anh
  Description : PLN Confirm Item and confirm work
  Date        : 28-06-2011
*/

using DaiCo.Application;
using DaiCo.ERPProject.Share.DataSetSource;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_02_010 : MainUserControl
  {

    public long workOrder = long.MinValue;
    private string insertPid = string.Empty;
    public viewPLN_02_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// load view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_010_Load(object sender, EventArgs e)
    {
      this.LoadcbWorkOrder();
      this.LoadcbItemCode();
      this.LoadcbRevision();
      this.LoadcbStatusWorkOrder();
      if (workOrder != long.MinValue)
      {
        cbWorkOrder.SelectedValue = workOrder;
      }
      this.search();
    }

    /// <summary>
    /// load combobox pln confirm, bom confirm ...
    /// </summary>
    private void LoadcbStatusWorkOrder()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Text", typeof(string));
      dt.Columns.Add("Value", typeof(Int32));

      DataTable dt1 = new DataTable();
      dt1.Columns.Add("Text", typeof(string));
      dt1.Columns.Add("Value", typeof(Int32));

      DataTable dt2 = new DataTable();
      dt2.Columns.Add("Text", typeof(string));
      dt2.Columns.Add("Value", typeof(Int32));

      DataTable dt3 = new DataTable();
      dt3.Columns.Add("Text", typeof(string));
      dt3.Columns.Add("Value", typeof(Int32));

      DataRow row = dt.NewRow();
      DataRow row1 = dt.NewRow();
      DataRow row2 = dt.NewRow();

      row1["Text"] = "Yes";
      row1["Value"] = 1;

      row2["Text"] = "No";
      row2["Value"] = 0;

      dt.Rows.InsertAt(row, 0);
      dt.Rows.Add(row1);
      dt.Rows.Add(row2);

      //--------------------------
      row = dt1.NewRow();
      row1 = dt1.NewRow();
      row2 = dt1.NewRow();

      row1["Text"] = "Yes";
      row1["Value"] = 1;

      row2["Text"] = "No";
      row2["Value"] = 0;

      dt1.Rows.InsertAt(row, 0);
      dt1.Rows.Add(row1);
      dt1.Rows.Add(row2);

      //----------------------
      row = dt2.NewRow();
      row1 = dt2.NewRow();
      row2 = dt2.NewRow();

      row1["Text"] = "Yes";
      row1["Value"] = 1;

      row2["Text"] = "No";
      row2["Value"] = 0;

      dt2.Rows.InsertAt(row, 0);
      dt2.Rows.Add(row1);
      dt2.Rows.Add(row2);

      //----------------------
      row = dt3.NewRow();
      row1 = dt3.NewRow();
      row2 = dt3.NewRow();

      row1["Text"] = "Yes";
      row1["Value"] = 1;

      row2["Text"] = "No";
      row2["Value"] = 0;

      dt3.Rows.InsertAt(row, 0);
      dt3.Rows.Add(row1);
      dt3.Rows.Add(row2);

      cbAllocated.DataSource = dt;
      cbAllocated.DisplayMember = "Text";
      cbAllocated.ValueMember = "Value";

      cbWipRun.DataSource = dt1;
      cbWipRun.DisplayMember = "Text";
      cbWipRun.ValueMember = "Value";

      cbPLNConfirm.DataSource = dt2;
      cbPLNConfirm.DisplayMember = "Text";
      cbPLNConfirm.ValueMember = "Value";

      cbBOMConfirm.DataSource = dt3;
      cbBOMConfirm.DisplayMember = "Text";
      cbBOMConfirm.ValueMember = "Value";
    }

    /// <summary>
    /// load revision
    /// </summary>  
    private void LoadcbRevision()
    {
      string commandText = string.Format("SELECT DISTINCT Revision FROM TblPLNWOInfoDetailGeneral ");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dt.NewRow();
      dt.Rows.InsertAt(row, 0);
      cbRevision.DataSource = dt;
      cbRevision.DisplayMember = "Revision";
      cbRevision.ValueMember = "Revision";
    }

    /// <summary>
    /// load itemcode
    /// </summary>
    private void LoadcbItemCode()
    {
      string commandText = string.Format("SELECT DISTINCT ItemCode FROM TblPLNWOInfoDetailGeneral ");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dt.NewRow();
      dt.Rows.InsertAt(row, 0);
      cbItemCode.DataSource = dt;
      cbItemCode.DisplayMember = "ItemCode";
      cbItemCode.ValueMember = "ItemCode";
    }

    /// <summary>
    /// load work order
    /// </summary>
    private void LoadcbWorkOrder()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Status = 0 ORDER BY Pid DESC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dt.NewRow();
      dt.Rows.InsertAt(row, 0);
      cbWorkOrder.DataSource = dt;
      cbWorkOrder.DisplayMember = "Pid";
      cbWorkOrder.ValueMember = "Pid";
    }

    /// <summary>
    /// search click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.search();
    }

    /// <summary>
    /// search khi click button search
    /// </summary>
    private void search()
    {
      if (cbWorkOrder.Text.Trim().ToString().Length == 0)
      {
        string message = string.Format(FunctionUtility.GetMessage("WRN0013"), "Work Order");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      chkConfirm.Visible = true;
      chkConfirm.Checked = false;
      if (cbItemCode.SelectedIndex >= 1 || cbRevision.SelectedIndex >= 1)
      {
        chkConfirm.Visible = false;

      }
      else
      {
        if (cbWorkOrder.SelectedIndex >= 1)
        {
          string strcnt = "Select COUNT(*) from TblPLNWorkOrder where Pid = " + cbWorkOrder.SelectedValue.ToString() + " and Confirm > 0";
          string strValue = DataBaseAccess.ExecuteScalarCommandText(strcnt).ToString();
          if (strValue != "0")
          {
            chkConfirm.Checked = true;
            chkConfirm.Enabled = false;
          }
        }
        else
        {
          chkConfirm.Checked = false;
          chkConfirm.Enabled = false;
        }
      }
      long workorder = workOrder;
      string itemcode = string.Empty;
      int revision = int.MinValue;
      int wiprun = int.MinValue;
      int allocate = int.MinValue;
      int plnconfirm = int.MinValue;
      int bomconfirm = int.MinValue;
      chkConfirm.Visible = false;

      if (cbWorkOrder.Text.ToString().Trim().Length == 0)
      {
        workorder = 0;
      }
      else if (cbWorkOrder.SelectedIndex > 0)
      {
        workorder = DBConvert.ParseLong(cbWorkOrder.SelectedValue.ToString());
      }
      else if (cbWorkOrder.SelectedIndex < 0)
      {
        workorder = 0;
      }
      if (cbItemCode.Text.ToString().Trim().Length == 0)
      {
      }
      else
      {
        itemcode = cbItemCode.Text.ToString();
      }

      if (cbRevision.Text.ToString().Trim().Length == 0)
      {
      }
      else if (cbRevision.SelectedIndex > 0)
      {
        revision = DBConvert.ParseInt(cbRevision.SelectedValue.ToString());
      }
      else if (cbRevision.SelectedIndex < 0)
      {
        revision = 0;
      }

      if (cbAllocated.Text.ToString().Trim().Length == 0)
      {

      }
      else if (cbAllocated.SelectedIndex > 0)
      {
        allocate = DBConvert.ParseInt(cbAllocated.SelectedValue.ToString());
      }
      else if (cbAllocated.SelectedIndex < 0)
      {
        allocate = 0;
      }

      if (cbWipRun.Text.ToString().Trim().Length == 0)
      {
      }
      else if (cbWipRun.SelectedIndex > 0)
      {
        wiprun = DBConvert.ParseInt(cbWipRun.SelectedValue.ToString());
      }
      else if (cbWipRun.SelectedIndex < 0)
      {
        wiprun = 0;
      }
      if (cbPLNConfirm.Text.ToString().Trim().Length == 0)
      {

      }
      else if (cbPLNConfirm.SelectedIndex > 0)
      {
        plnconfirm = DBConvert.ParseInt(cbPLNConfirm.SelectedValue.ToString());
      }
      else if (cbPLNConfirm.SelectedIndex < 0)
      {
        plnconfirm = 0;
      }
      if (cbBOMConfirm.Text.ToString().Trim().Length == 0)
      {

      }
      else if (cbBOMConfirm.SelectedIndex > 0)
      {
        bomconfirm = DBConvert.ParseInt(cbBOMConfirm.SelectedValue.ToString());
      }
      else if (cbBOMConfirm.SelectedIndex < 0)
      {
        bomconfirm = 0;
      }
      DBParameter[] inputParam = new DBParameter[8];
      if (workorder != long.MinValue)
      {
        inputParam[1] = new DBParameter("@WoInfoPID", DbType.Int64, workorder);
      }
      if (itemcode != string.Empty)
      {
        inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemcode);
      }
      if (revision != int.MinValue)
      {
        inputParam[3] = new DBParameter("@Revision", DbType.Int32, revision);
      }
      if (allocate != int.MinValue)
      {
        inputParam[4] = new DBParameter("@Allocated", DbType.Int32, allocate);
      }
      if (wiprun != int.MinValue)
      {
        inputParam[5] = new DBParameter("@WIPRun", DbType.Int32, wiprun);
      }
      if (plnconfirm != int.MinValue)
      {
        inputParam[6] = new DBParameter("@PLNConfirm", DbType.Int32, plnconfirm);
      }
      if (bomconfirm != int.MinValue)
      {
        inputParam[7] = new DBParameter("@BOMConfirm", DbType.Int32, bomconfirm);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNWorkOrderInfomationByItem", inputParam);
      dsPLNWoConfirmDetail ds_Detail = new dsPLNWoConfirmDetail();
      if (ds != null)
      {
        try
        {
          ds_Detail.Tables["WO_ListItem"].Merge(ds.Tables[0]);
          ds_Detail.Tables["WO_Detail"].Merge(ds.Tables[1]);
        }
        catch
        {
        }
      }
      ultraGrid.DataSource = ds_Detail;

      for (int i = 0; i < ultraGrid.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        string columnName = ultraGrid.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if ((string.Compare(columnName, "PLN Confirm", true) == 0) || (string.Compare(columnName, "IsDelete", true) == 0))
        {
          ultraGrid.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
        {
          ultraGrid.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }
      ultraGrid.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
      ultraGrid.DisplayLayout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
      ultraGrid.DisplayLayout.Bands[1].Override.RowAppearance.BackColor = Color.DarkSeaGreen;
    }

    /// <summary>
    /// close form click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    /// <summary>
    /// init ultra grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultraGrid);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].ColHeaderLines = 2;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["WoInfoPID"].Header.Caption = "Work Order";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ProcessItemCode"].Header.Caption = "Process\nItemCode";
      e.Layout.Bands[0].Columns["WoInfoPID"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Total Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["IsDelete"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CBM Existed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["BOM Confirm"].Hidden = true;
      e.Layout.Bands[0].Columns["BOM Confirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["PLN Confirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Item Confirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Carcass Confirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["WIP Run"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Allocated"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ProcessItemCode"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["RowState"].Hidden = true;
      e.Layout.Bands[0].Columns["Old PLN Confirm"].Hidden = true;
      e.Layout.Bands[0].Columns["SubConConfirm"].Hidden = true;
      e.Layout.Bands[0].Columns["IsSubCon"].Header.Caption = "Sub Con";
      e.Layout.Bands[0].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["WoInfoPID"].Hidden = true;
      e.Layout.Bands[1].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["RowState"].Hidden = true;
      e.Layout.Bands[1].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;
      e.Layout.Bands[1].Columns["Remain"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["CBM"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Priority"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["SaleNo"].Header.Caption = "Sale No";
      e.Layout.Bands[1].Columns["CustomerPONo"].Header.Caption = "Customer's Po No";
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["ScheduleDelivery"].Header.Caption = "Schedule Delivery";
      e.Layout.Bands[1].Columns["ScheduleDelivery"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["IsSubCon"].Header.Caption = "Sub Con";
      e.Layout.Bands[1].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
    }

    /// <summary>
    /// expand all grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultraGrid.Rows.ExpandAll(true);
      }
      else
      {
        ultraGrid.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// clear điều kiện search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      cbWorkOrder.Text = string.Empty;
      cbItemCode.Text = string.Empty;
      cbRevision.Text = string.Empty;
      cbPLNConfirm.Text = string.Empty;
      cbAllocated.Text = string.Empty;
      cbWipRun.Text = string.Empty;
      cbBOMConfirm.Text = string.Empty;
      workOrder = long.MinValue;
      this.search();
    }
    /// <summary>
    /// grid cell change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_CellChange(object sender, CellEventArgs e)
    {
      string strColName = e.Cell.Column.ToString().ToLower();
      if (strColName == "pln confirm")
      {
        if (e.Cell.Row.Cells["CBM Existed"].Value.ToString() == "0")
        {
          WindowUtinity.ShowMessageError("ERR0029", "CBM");
          e.Cell.CancelUpdate();
          e.Cell.Row.Cells["RowState"].Value = "0";
          return;
        }

        if (e.Cell.Row.Cells["ProcessItemCode"].Value.ToString() == "0")
        {
          WindowUtinity.ShowMessageError("ERR0029", "Process ItemCode");
          e.Cell.CancelUpdate();
          e.Cell.Row.Cells["RowState"].Value = "0";
          return;
        }

        if ((e.Cell.Row.Cells["SubConConfirm"].Value.ToString() == "0"
          && e.Cell.Row.Cells["BOM Confirm"].Value.ToString() == "0"
          && DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) == long.MinValue)
          || DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) != long.MinValue)
        {
          e.Cell.CancelUpdate();
        }
        else
        {
          if (e.Cell.Row.Cells["RowState"].Value.ToString() == "1")
          {
            e.Cell.Row.Cells["RowState"].Value = "0";
          }
          else
          {
            e.Cell.Row.Cells["RowState"].Value = "1";
          }
        }
      }
      else if (strColName == "isdelete")
      {
        if ((DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) == long.MinValue) || (e.Cell.Row.Cells["WIP Run"].Value.ToString() == "1" || e.Cell.Row.Cells["Allocated"].Value.ToString() == "1"))
        {
          e.Cell.CancelUpdate();
        }
      }
    }
    /// <summary>
    /// grid after cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_AfterCellUpdate(object sender, CellEventArgs e)
    {

    }
    /// <summary>
    /// save click - save confirm or delete item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.insertPid = string.Empty;
      string strcnt = "Select COUNT(*) from TblPLNWorkOrder where Pid = " + cbWorkOrder.SelectedValue.ToString() + " and Confirm > 0";
      string strValue = DataBaseAccess.ExecuteScalarCommandText(strcnt).ToString();
      bool result = true;
      if (strValue == "0" && (chkConfirm.Checked))
      {
        if (DBConvert.ParseLong(cbWorkOrder.SelectedValue.ToString()) != long.MinValue)
        {
          string strsql = "UPDATE TblPLNWorkOrder SET Confirm = 1 WHERE Pid = " + cbWorkOrder.SelectedValue.ToString();
          result = DataBaseAccess.ExecuteCommandText(strsql);
          result = this.CastDataWhenWorkConfirmed(DBConvert.ParseLong(cbWorkOrder.SelectedValue.ToString()));
        }
      }
      for (int i = 0; i < ultraGrid.Rows.Count; i++)
      {
        if (ultraGrid.Rows[i].Cells["IsDelete"].Value.ToString() == "1")
        {
          if (DBConvert.ParseLong(ultraGrid.Rows[i].Cells["Pid"].Value.ToString()) >= 0)
          {
            result = DeleteConfirmWork(DBConvert.ParseLong(ultraGrid.Rows[i].Cells["Pid"].Value.ToString()));
          }
        }
      }
      for (int i = 0; i < ultraGrid.Rows.Count; i++)
      {
        if (ultraGrid.Rows[i].Cells["RowState"].Value.ToString() == "1")
        {
          if (DBConvert.ParseLong(ultraGrid.Rows[i].Cells["PLN Confirm"].Value.ToString()) >= 0)
          {
            long Wo = DBConvert.ParseLong(ultraGrid.Rows[i].Cells["WoInfoPID"].Value.ToString());
            string ItemCode = ultraGrid.Rows[i].Cells["ItemCode"].Value.ToString();
            int Revision = DBConvert.ParseInt(ultraGrid.Rows[i].Cells["Revision"].Value.ToString());
            int IsSubCon = DBConvert.ParseInt(ultraGrid.Rows[i].Cells["SubConConfirm"].Value.ToString());
            if (Wo != long.MinValue && Revision != int.MinValue && ItemCode.Length > 0)
            {
              result = ConfirmWorkByItem(Wo, ItemCode, Revision, IsSubCon);
            }
          }
        }
      }
      if (insertPid.Length > 0)
      {
        result = UnlockCarcass(this.insertPid);
      }
      if (result)
      {
        WindowUtinity.ShowMessageSuccessFromText(FunctionUtility.GetMessage("MSG0004"));
      }
      else
      {
        WindowUtinity.ShowMessageError(FunctionUtility.GetMessage("ERR0005"));
      }
      this.UpdateConfirmWork(DBConvert.ParseLong(cbWorkOrder.SelectedValue.ToString()));
      btnSearch_Click(sender, e);
    }

    /// <summary>
    /// update work confirm
    /// </summary>
    /// <param name="workPid"></param>
    private void UpdateConfirmWork(long workPid)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@WorkPid", DbType.Int64, workPid) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrder_UpdateStatusConfirm", inputParam);
    }

    /// <summary>
    /// copy cau truc carcass
    /// </summary>
    /// <param name="Wo"></param>
    /// <returns></returns>
    private bool CastDataWhenWorkConfirmed(long Wo)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Wo", DbType.Int64, Wo) };
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNCastWorkDetailsWhenConfirm", 5000, inputParam, outputParam);
      int success = DBConvert.ParseInt(outputParam[0].Value.ToString());
      return (success == 1 ? true : false);
    }
    /// <summary>
    /// Unlock Carcass After Confirm Wo
    /// </summary>
    /// <param name="Wo"></param>
    /// <returns></returns>
    private bool UnlockCarcass(string stringPid)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@StringPid", DbType.AnsiString, 4000, stringPid) };
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNUnlockCarcassAfterConfirmWo", inputParam, outputParam);
      int success = DBConvert.ParseInt(outputParam[0].Value.ToString());
      return (success == 1 ? true : false);
    }
    /// <summary>
    /// delete work confirm
    /// </summary>
    /// <param name="ConfimPid"></param>
    /// <returns></returns>
    private bool DeleteConfirmWork(long ConfimPid)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ConfimPid", DbType.Int64, ConfimPid) };
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrderConfirmDetail_Delete", inputParam, outputParam);
      int success = DBConvert.ParseInt(outputParam[0].Value.ToString());
      return (success == 1 ? true : false);
    }

    /// <summary>
    /// conofirm item
    /// </summary>
    /// <param name="Wo"></param>
    /// <param name="ItemCode"></param>
    /// <param name="Revision"></param>
    /// <returns></returns>
    private bool ConfirmWorkByItem(long Wo, string ItemCode, int Revision, int IsSubCon)
    {
      DBParameter[] inputParam = new DBParameter[4];
      inputParam[0] = new DBParameter("@Wo", DbType.Int64, Wo);
      inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ItemCode);
      inputParam[2] = new DBParameter("@Revision", DbType.Int32, Revision);
      inputParam[3] = new DBParameter("@IsSubCon", DbType.Int32, IsSubCon);
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrderConfirmDetail", 90, inputParam, outputParam);
      long success = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (success > 0)
      {
        insertPid += success.ToString().Trim() + ",";
        return true;
      }
      else
      {
        return false;
      }
    }
    /// <summary>
    /// confirm click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkConfirm_Click(object sender, EventArgs e)
    {
    }
    /// <summary>
    /// confirm all checked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkPLNConfirmAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkPLNConfirmAll.Checked)
      {
        for (int i = 0; i < ultraGrid.Rows.Count; i++)
        {
          if ((DBConvert.ParseInt(ultraGrid.Rows[i].Cells["BOM Confirm"].Value.ToString()) == 1
            || DBConvert.ParseInt(ultraGrid.Rows[i].Cells["SubConConfirm"].Value.ToString()) == 1)
            && (DBConvert.ParseInt(ultraGrid.Rows[i].Cells["ProcessItemCode"].Value.ToString()) == 1)
            && DBConvert.ParseInt(ultraGrid.Rows[i].Cells["Old PLN Confirm"].Value.ToString()) == 0)
          {
            ultraGrid.Rows[i].Cells["PLN Confirm"].Value = 1;
            ultraGrid.Rows[i].Cells["RowState"].Value = 1;
          }
        }
      }
      else
      {
        for (int i = 0; i < ultraGrid.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultraGrid.Rows[i].Cells["Old PLN Confirm"].Value.ToString()) == 0)
          {
            ultraGrid.Rows[i].Cells["PLN Confirm"].Value = 0;
            ultraGrid.Rows[i].Cells["RowState"].Value = 0;
          }
        }
      }
    }

    private void cbWorkOrder_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cbItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cbRevision_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cbPLNConfirm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cbAllocated_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cbWipRun_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cbBOMConfirm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
  }
}
