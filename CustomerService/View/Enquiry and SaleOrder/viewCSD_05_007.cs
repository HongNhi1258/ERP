/*
 * Created By   : Nguyen Van Tron
 * Created Date : 28/06/2010
 * Description  : Add item for Sale Order
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_05_007 : MainUserControl
  {
    public long customerPid = long.MinValue;
    public long customerDirect = long.MinValue;
    public int type = int.MinValue;
    public DataTable dtExistingSource = new DataTable();
    public DataTable dtNewSource = new DataTable();
    public string deliveryrequirement = string.Empty;
    public string packingrequirement = string.Empty;
    public string documentrequirement = string.Empty;
    public string shipmentTerms = string.Empty;
    public string paymentTerms = string.Empty;
    public int environmentStatus = int.MinValue;
    public viewCSD_05_007()
    {
      InitializeComponent();
      dt_OrderDateFrom.Value = DateTime.MinValue;
      dt_OrderDateTo.Value = DateTime.MinValue;
    }

    private void viewCSD_05_007_Load(object sender, EventArgs e)
    {
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      chkSelectedAll.Checked = false;
      string enquiryFrom = txtEnquiryFrom.Text.Trim();
      string enquiryTo = txtEnquiryTo.Text.Trim();
      DateTime orderDateFrom = dt_OrderDateFrom.Value.Date;
      DateTime orderDateTo = dt_OrderDateTo.Value.Date;

      DBParameter[] inputParam = new DBParameter[7];
      if (enquiryFrom.Length > 0)
      {
        inputParam[0] = new DBParameter("@EnquiryFrom", DbType.AnsiString, 16, enquiryFrom);
      }
      if (enquiryTo.Length > 0)
      {
        inputParam[1] = new DBParameter("@EnquiryTo", DbType.AnsiString, 16, enquiryTo);
      }
      if (orderDateFrom != DateTime.MinValue)
      {
        inputParam[2] = new DBParameter("@OrderDateFrom", DbType.DateTime, orderDateFrom);
      }
      if (orderDateTo != DateTime.MinValue)
      {
        inputParam[3] = new DBParameter("@OrderDateTo", DbType.DateTime, orderDateTo.AddDays(1));
      }
      if (this.customerPid != long.MinValue)
      {
        inputParam[4] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);
      }
      if (this.customerDirect != long.MinValue)
      {
        inputParam[5] = new DBParameter("@CustomerDirect", DbType.Int64, this.customerDirect);
      }
      if (this.type != int.MinValue)
      {
        inputParam[6] = new DBParameter("@Type", DbType.Int32, this.type);
      }
      DataTable dtSourceDB = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spPLNListEnquiryConfirmDetail", inputParam);
      DataTable dtSource = dtSourceDB.Clone();
      int index = 1;
      //Check Item
      foreach (DataRow newRow in dtSourceDB.Rows)
      {
        if (!this.CheckExistingItem(newRow))
        {
          this.CopyRowEnquiryInfo(dtSource, newRow, index);
          index++;
          this.deliveryrequirement = newRow["DeliveryRequirement"].ToString();
          this.packingrequirement = newRow["PackingRequirement"].ToString();
          this.documentrequirement = newRow["DocumentRequirement"].ToString();
          this.shipmentTerms = newRow["ShipmentTerms"].ToString();
          this.paymentTerms = newRow["PaymentTerms"].ToString();
          this.environmentStatus = DBConvert.ParseInt(newRow["EnvironmentStatus"].ToString());
        }
      }
      ultraGridEnquiryDetail.DataSource = dtSource;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool bItemAdded = false;
      DataTable dtSource = (DataTable)ultraGridEnquiryDetail.DataSource;
      if (dtSource != null)
      {
        this.dtNewSource = dtSource.Clone();
        int index = 1;
        foreach (DataRow row in dtSource.Rows)
        {
          if (DBConvert.ParseInt(row["Select"].ToString()) == 1)
          {
            this.CopyRowEnquiryInfo(this.dtNewSource, row, index);
            bItemAdded = true;
            index++;
          }
        }
      }
      if (bItemAdded)
      {
        this.ConfirmToCloseTab();
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0007");
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridEnquiryDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultraGridEnquiryDetail);
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["NonPlan"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      //Cuong Add
      e.Layout.Bands[0].Columns["Return"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      //-------------
      e.Layout.Bands[0].Columns["EnquiryConfirmDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["DeliveryRequirement"].Hidden = true;
      e.Layout.Bands[0].Columns["PackingRequirement"].Hidden = true;
      e.Layout.Bands[0].Columns["DocumentRequirement"].Hidden = true;
      e.Layout.Bands[0].Columns["QtyOld"].Hidden = true;
      e.Layout.Bands[0].Columns["Odd Box"].Hidden = true;
      e.Layout.Bands[0].Columns["PackageQty"].Hidden = true;
      e.Layout.Bands[0].Columns["IsMod"].Hidden = true;
      e.Layout.Bands[0].Columns["SpecialInstruction"].Hidden = true;
      e.Layout.Bands[0].Columns["RemarkForAccount"].Hidden = true;
      e.Layout.Bands[0].Columns["Return"].Hidden = true;
      e.Layout.Bands[0].Columns["Price"].Hidden = true;
      e.Layout.Bands[0].Columns["SecondPrice"].Hidden = true;
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Request\nShip Date";
      e.Layout.Bands[0].Columns["RequestDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["RequestDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Confirmed\nShip Date";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (e.Layout.Bands[0].Columns[i].ToString() != "ScheduleDelivery")
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      e.Layout.Bands[0].Columns["Select"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands[0].Columns["EnquiryNo"].Header.Caption = "Enquiry No";
      e.Layout.Bands[0].Columns["CustomerEnquiryNo"].Header.Caption = "Cus' EN No";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[0].Columns["NonPlan"].Header.Caption = "Non\nPlan";
      for (int i = 0; i < ultraGridEnquiryDetail.Rows.Count; i++)
      {
        if (ultraGridEnquiryDetail.Rows[i].Cells["NonPlan"].Value.ToString() == "1" && ultraGridEnquiryDetail.Rows[i].Cells["ScheduleDelivery"].Value.ToString().Length == 0)
        {
          ultraGridEnquiryDetail.Rows[i].Cells["ScheduleDelivery"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
        }
        else
        {
          ultraGridEnquiryDetail.Rows[i].Cells["ScheduleDelivery"].Activation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
        }
      }
    }

    /// <summary>
    /// Check item is existing on current sale order (Enquiry, ItemCode, Revision, ScheduleDelivery)
    /// </summary>
    private bool CheckExistingItem(DataRow newRow)
    {
      foreach (DataRow existingRow in dtExistingSource.Rows)
      {
        if (existingRow.RowState != DataRowState.Deleted)
        {
          string existingEnquiry = existingRow["EnquiryNo"].ToString();
          string newEnquiry = newRow["EnquiryNo"].ToString();
          string existingItem = existingRow["ItemCode"].ToString();
          string newItem = newRow["ItemCode"].ToString();
          int existingRevision = DBConvert.ParseInt(existingRow["Revision"].ToString());
          int newRevision = DBConvert.ParseInt(newRow["Revision"].ToString());
          DateTime existingScheduleDate = DateTime.MinValue;
          try
          {
            existingScheduleDate = (DateTime)existingRow["ScheduleDelivery"];
          }
          catch { }
          DateTime newScheduleDate = DateTime.MinValue;
          try
          {
            newScheduleDate = (DateTime)newRow["ScheduleDelivery"];
          }
          catch { }
          if ((string.Compare(existingEnquiry, newEnquiry, true) == 0) && (string.Compare(existingItem, newItem, true) == 0) && (existingRevision == newRevision) && (existingScheduleDate == newScheduleDate))
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Copy item info row of Enquiry
    /// </summary>
    private void CopyRowEnquiryInfo(DataTable dtDestination, DataRow sourceRow, int index)
    {
      DataRow newRow = dtDestination.NewRow();
      newRow["No"] = index;
      newRow["EnquiryNo"] = sourceRow["EnquiryNo"];
      newRow["ItemCode"] = sourceRow["ItemCode"];
      newRow["Name"] = sourceRow["Name"];
      newRow["Revision"] = sourceRow["Revision"];
      newRow["Qty"] = sourceRow["Qty"];
      newRow["QtyOld"] = sourceRow["QtyOld"];
      newRow["ScheduleDelivery"] = sourceRow["ScheduleDelivery"];
      newRow["RequestDate"] = sourceRow["RequestDate"];
      newRow["Remark"] = sourceRow["Remark"];
      newRow["CustomerEnquiryNo"] = sourceRow["CustomerEnquiryNo"];
      newRow["SpecialInstruction"] = sourceRow["SpecialInstruction"];
      newRow["EnquiryConfirmDetailPid"] = sourceRow["EnquiryConfirmDetailPid"];
      newRow["Select"] = sourceRow["Select"];
      newRow["SaleCode"] = sourceRow["SaleCode"];
      newRow["CBM"] = sourceRow["CBM"];
      newRow["Unit"] = sourceRow["Unit"];
      newRow["TotalCBM"] = sourceRow["TotalCBM"];
      newRow["Price"] = sourceRow["Price"];
      newRow["SecondPrice"] = sourceRow["SecondPrice"];
      newRow["RemarkForAccount"] = sourceRow["RemarkForAccount"];
      newRow["NonPlan"] = sourceRow["NonPlan"];
      //Cuong Add
      newRow["Return"] = sourceRow["Return"];
      //-----------

      //newRow["DeliveryRequirement"] = sourceRow["DeliveryRequirement"];
      //newRow["PackingRequirement"] = sourceRow["PackingRequirement"];
      //newRow["DocumentRequirement"] = sourceRow["DocumentRequirement"];

      dtDestination.Rows.Add(newRow);
    }

    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = chkSelectedAll.Checked ? 1 : 0;
      for (int i = 0; i < ultraGridEnquiryDetail.Rows.Count; i++)
      {
        ultraGridEnquiryDetail.Rows[i].Cells["Select"].Value = selected;
      }
    }

    private void ultraGridEnquiryDetail_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      int QtyOld = DBConvert.ParseInt(e.Cell.Row.Cells["QtyOld"].Value.ToString());

      if (string.Compare(colName, "Qty", true) == 0)
      {
        int Qty = DBConvert.ParseInt(e.Cell.Text.Trim());
        if (Qty <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Qty" });
          e.Cancel = true;
        }
        else if (Qty > QtyOld)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERRO124", "Qty", "Enquiry Qty");
          e.Cancel = true;
        }
      }
    }

   
  }
}