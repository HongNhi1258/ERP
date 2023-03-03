/*
 * Created By   : Nguyen Van Tron
 * Created Date : 28/06/2010
 * Description  : Add item for Sale Order
 */

using DaiCo.Application;
using DaiCo.Shared;
using System;
using System.Data;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_05_007 : MainUserControl
  {
    public long customerPid = long.MinValue;
    public long customerDirect = long.MinValue;
    public int type = int.MinValue;
    public DataTable dtExistingSource = new DataTable();    
    public DataTable dtAddedEnquiryList;
    public viewCSD_05_007()
    {
      InitializeComponent();
      udtOrderDateFrom.Value = null;
      udtOrderDateTo.Value = null;
    }

    private void viewCSD_05_007_Load(object sender, EventArgs e)
    {
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      chkSelectedAll.Checked = false;
      string enquiryFrom = txtEnquiryFrom.Text.Trim();
      string enquiryTo = txtEnquiryTo.Text.Trim();
      DateTime orderDateFrom = DBConvert.ParseDateTime(udtOrderDateFrom.Value);
      DateTime orderDateTo = DBConvert.ParseDateTime(udtOrderDateTo.Value);

      DBParameter[] inputParam = new DBParameter[8];
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
      inputParam[7] = new DBParameter("@AddedEnquiryList", DbType.String, 4000, DBConvert.ParseXMLString(this.dtAddedEnquiryList));
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spCSDSaleOrderDetail_SearchEnquiry", inputParam);
      ultraGridEnquiryDetail.DataSource = dtSource;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool bItemAdded = false;
      DataTable dtSource = (DataTable)ultraGridEnquiryDetail.DataSource;
      if (dtSource != null)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (DBConvert.ParseInt(row["Select"].ToString()) == 1)
          {
            DataRow newRow = dtExistingSource.NewRow();
            newRow["EnquiryNo"] = row["EnquiryNo"];
            newRow["ItemCode"] = row["ItemCode"];
            newRow["SaleCode"] = row["SaleCode"];
            newRow["Revision"] = row["Revision"];
            newRow["Name"] = row["Name"];
            newRow["Qty"] = row["Qty"];
            newRow["Unit"] = row["Unit"];
            newRow["Price"] = row["Price"];
            newRow["Amount"] = row["Amount"];
            newRow["ScheduleDelivery"] = row["ScheduleDelivery"];
            newRow["ConfirmedShipDate"] = row["ConfirmedShipDate"];
            newRow["EnquiryConfirmDetailPid"] = row["EnquiryConfirmDetailPid"];
            newRow["Remark"] = row["Remark"];
            dtExistingSource.Rows.Add(newRow);
            bItemAdded = true;
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
      Utility.SetPropertiesUltraGrid(ultraGridEnquiryDetail);
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
     
      e.Layout.Bands[0].Columns["EnquiryConfirmDetailPid"].Hidden = true;      
      e.Layout.Bands[0].Columns["ScheduleDelivery"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;      
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;      
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (e.Layout.Bands[0].Columns[i].ToString() != "ConfirmedShipDate")
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      e.Layout.Bands[0].Columns["Select"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Price"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;

      e.Layout.Bands[0].Columns["Select"].Header.Caption = "Chọn";
      e.Layout.Bands[0].Columns["EnquiryNo"].Header.Caption = "Mã Enquiry";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Mã SP";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Mã SP KH";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Tên SP";      
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Ngày yêu cầu\ngiao hàng";
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].Header.Caption = "Ngày xác nhận\ngiao hàng";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["Price"].Header.Caption = "Đơn giá";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Thành tiền";
    }

    private void chkSelectedAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = chkSelectedAll.Checked ? 1 : 0;
      for (int i = 0; i < ultraGridEnquiryDetail.Rows.Count; i++)
      {
        ultraGridEnquiryDetail.Rows[i].Cells["Select"].Value = selected;
      }
    }

    private void ultraGridEnquiryDetail_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "price":
        case "qty":
          double qty = DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value);
          double price = DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value);
          if (qty >= 0 && price >= 0)
          {
            e.Cell.Row.Cells["Amount"].Value = qty * price;
          }
          else
          {
            e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
          }
          break;
        default:
          break;
      }
    }
  }
}