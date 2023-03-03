/*
 * Created By   : Nguyen Van Tron
 * Created Date : 28/06/2010
 * Description  : Add item for Sale Order
 */

using DaiCo.Application;
using DaiCo.Shared;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_05_010 : MainUserControl
  {
    public long customerPid = long.MinValue;
    public long customerDirect = long.MinValue;
    public long saleorderPid = long.MinValue;
    public string itemcodeSwapCancel = string.Empty;
    public int revisionSwapCancel = int.MinValue;
    public long Pid = long.MinValue;
    public DataTable dtExistingSource = new DataTable();
    public DataTable dtNewSource = new DataTable();

    public viewCSD_05_010()
    {
      InitializeComponent();
    }


    private void SaveSaleOrderCancel()
    {
      bool bItemAdded = false;
      DataTable dtSource = (DataTable)ultraGridSaleOrderDetail.DataSource;
      if (dtSource != null)
      {
        this.dtNewSource = null;
        this.dtNewSource = dtSource.Clone();
        DataRow[] arrRow = dtSource.Select("Cancel > 0");

        for (int i = 0; i < arrRow.Length; i++)
        {
          int cancelQty = DBConvert.ParseInt(arrRow[i]["Cancel"].ToString());
          int BalQty = DBConvert.ParseInt(arrRow[i]["BalQty"].ToString());
          if ((cancelQty > 0) && (cancelQty <= BalQty))
          {
            this.CopyRowEnquiryInfo(this.dtNewSource, arrRow[i]);
            bItemAdded = true;
          }
          else if (arrRow[i]["Cancel"].ToString().Trim().Length > 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("WRN0029", "Cancel", "Qty", "Balance", "SaleNo: " + arrRow[i]["SaleNo"].ToString() + " ItemCode: " + arrRow[i]["ItemCode"].ToString());
            return;
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

    private void SaveSaleOrderSwapCancel()
    {
      bool bItemAdded = false;
      DataTable dtSource = (DataTable)ultraGridSaleOrderDetail.DataSource;
      if (dtSource != null)
      {
        this.dtNewSource = null;
        this.dtNewSource = dtSource.Clone();
        DataRow[] arrRow = dtSource.Select("Cancel > 0");

        for (int i = 0; i < arrRow.Length; i++)
        {
          int cancelQty = DBConvert.ParseInt(arrRow[i]["Cancel"].ToString());
          int remainQty = DBConvert.ParseInt(arrRow[i]["Remain"].ToString());
          if ((cancelQty > 0) && (cancelQty <= remainQty))
          {
            this.CopyRowEnquiryInfoCancelSwap(this.dtNewSource, arrRow[i]);
            bItemAdded = true;
          }
          else if (arrRow[i]["Cancel"].ToString().Trim().Length > 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("WRN0029", "Cancel", "Qty", "Remain", "SaleNo: " + arrRow[i]["SaleNo"].ToString() + " ItemCode: " + arrRow[i]["ItemCode"].ToString());
            return;
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

    private void viewCSD_05_010_Load(object sender, EventArgs e)
    {
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      string saleOrderFrom = txtSaleOrderFrom.Text.Trim();
      string saleOrderTo = txtSaleOrderTo.Text.Trim();
      string itemCode = txtItemCode.Text.Trim();
      string saleCode = txtSaleCode.Text.Trim();
      DBParameter[] inputParam = new DBParameter[9];

      if (saleOrderFrom.Length > 0)
      {
        inputParam[0] = new DBParameter("@SaleOrderFrom", DbType.AnsiString, 16, saleOrderFrom);
      }
      if (saleOrderTo.Length > 0)
      {
        inputParam[1] = new DBParameter("@SaleOrderTo", DbType.AnsiString, 16, saleOrderTo);
      }
      if (itemCode.Length > 0)
      {
        inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, "%" + itemCode + "%");
      }
      if (this.customerPid != long.MinValue && saleorderPid == long.MinValue)
      {
        inputParam[3] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);
      }
      if (this.customerDirect != long.MinValue && saleorderPid == long.MinValue)
      {
        inputParam[4] = new DBParameter("@CustomerDirect", DbType.Int64, this.customerDirect);
      }
      if (saleCode.Length > 0)
      {
        inputParam[5] = new DBParameter("@SaleCode", DbType.AnsiString, 16, "%" + saleCode + "%");
      }
      if (saleorderPid != long.MinValue)
      {
        inputParam[6] = new DBParameter("@Saleorderpid", DbType.Int64, saleorderPid);
        inputParam[7] = new DBParameter("@ItemCodeSwapCancel", DbType.AnsiString, 16, itemcodeSwapCancel);
        inputParam[8] = new DBParameter("@RevisionSwapCancel", DbType.Int32, revisionSwapCancel);
      }
      DataTable dtSourceDB = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spPLNListItemOfSaleOrderCancel", 300, inputParam);
      DataTable dtSource = dtSourceDB.Clone();

      // Check Item
      foreach (DataRow newRow in dtSourceDB.Rows)
      {
        if (!this.CheckExistingItem(newRow))
        {
          this.CopyRowEnquiryInfo(dtSource, newRow);
        }
      }

      ultraGridSaleOrderDetail.DataSource = dtSource;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (saleorderPid == long.MinValue)
      {
        this.SaveSaleOrderCancel();
      }
      else
      {
        this.SaveSaleOrderSwapCancel();
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridEnquiryDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultraGridSaleOrderDetail);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[0].Columns["Remain"].Header.Caption = "Unreleased";
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      for (int i = 0; i < ultraGridSaleOrderDetail.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        string columnName = ultraGridSaleOrderDetail.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if (string.Compare(columnName, "Cancel", true) == 0)
        {
          ultraGridSaleOrderDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
        {
          ultraGridSaleOrderDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
        if ((string.Compare(columnName, "BalQty", true) == 0) || (string.Compare(columnName, "OriginalQty", true) == 0) || (string.Compare(columnName, "Cancelled", true) == 0) || (string.Compare(columnName, "Remain", true) == 0))
        {
          ultraGridSaleOrderDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.BackColor = Color.Wheat;
          ultraGridSaleOrderDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.ForeColor = Color.MediumBlue;
          ultraGridSaleOrderDetail.DisplayLayout.Bands[0].Columns["Cancel"].CellAppearance.BackColor = Color.SkyBlue;
          ultraGridSaleOrderDetail.DisplayLayout.Bands[0].Columns["Cancel"].CellAppearance.ForeColor = Color.OrangeRed;
        }

      }
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Cancel"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
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
          string existingEnquiry = existingRow["SaleOrderPid"].ToString();
          string newEnquiry = newRow["SaleOrderPid"].ToString();
          string existingItem = existingRow["ItemCode"].ToString();
          string newItem = newRow["ItemCode"].ToString();
          int existingRevision = DBConvert.ParseInt(existingRow["Revision"].ToString());
          int newRevision = DBConvert.ParseInt(newRow["Revision"].ToString());
          if (dtExistingSource.TableName == "SaleOrderChild")
          {
            if ((string.Compare(existingEnquiry, newEnquiry, true) == 0) && (string.Compare(existingItem, newItem, true) == 0) && (existingRevision == newRevision))
            {
              newRow["Remain"] = DBConvert.ParseInt(newRow["Remain"].ToString()) - DBConvert.ParseInt(existingRow["Cancel"].ToString());
              //return true;
            }
          }
          else
          {
            if ((string.Compare(existingEnquiry, newEnquiry, true) == 0) && (string.Compare(existingItem, newItem, true) == 0) && (existingRevision == newRevision))
            {
              return true;
            }
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Copy item info row of Enquiry
    /// </summary>
    /// 

    private void CopyRowEnquiryInfo(DataTable dtDestination, DataRow sourceRow)
    {
      DataRow newRow = dtDestination.NewRow();
      newRow["SaleOrderPid"] = sourceRow["SaleOrderPid"];
      newRow["SaleNo"] = sourceRow["SaleNo"];
      newRow["ItemCode"] = sourceRow["ItemCode"];
      newRow["ItemName"] = sourceRow["ItemName"];
      newRow["Revision"] = sourceRow["Revision"];
      newRow["OriginalQty"] = sourceRow["OriginalQty"];
      newRow["BalQty"] = sourceRow["BalQty"];
      //newRow["Qty"] = sourceRow["Qty"];
      newRow["Cancelled"] = sourceRow["Cancelled"];
      newRow["OpenWO"] = sourceRow["OpenWO"];
      newRow["Remain"] = sourceRow["Remain"];
      newRow["Cancel"] = sourceRow["Cancel"];
      newRow["SaleCode"] = sourceRow["SaleCode"];
      newRow["CUSName"] = sourceRow["CUSName"];
      newRow["CBM"] = sourceRow["CBM"];
      newRow["TotalCBM"] = sourceRow["TotalCBM"];
      dtDestination.Rows.Add(newRow);
    }

    private void CopyRowEnquiryInfoCancelSwap(DataTable dtDestination, DataRow sourceRow)
    {
      DataRow newRow = dtDestination.NewRow();
      newRow["SaleOrderPid"] = sourceRow["SaleOrderPid"];
      newRow["SaleNo"] = sourceRow["SaleNo"];
      newRow["ItemCode"] = sourceRow["ItemCode"];
      newRow["Revision"] = sourceRow["Revision"];
      newRow["Remain"] = sourceRow["Remain"];
      newRow["Cancel"] = sourceRow["Cancel"];
      dtDestination.Rows.Add(newRow);
    }

    private void chkDefaultQty_CheckedChanged(object sender, EventArgs e)
    {
      if (chkDefaultQty.Checked)
      {
        for (int i = 0; i < ultraGridSaleOrderDetail.Rows.Count; i++)
        {
          if (ultraGridSaleOrderDetail.Rows[i].Cells["Cancel"].Value.ToString().Trim().Length == 0)
          {
            ultraGridSaleOrderDetail.Rows[i].Cells["Cancel"].Value = ultraGridSaleOrderDetail.Rows[i].Cells["Remain"].Value;
          }
        }
      }
      else
      {
        for (int i = 0; i < ultraGridSaleOrderDetail.Rows.Count; i++)
        {
          ultraGridSaleOrderDetail.Rows[i].Cells["Cancel"].Value = DBNull.Value;
        }
      }
    }

    private void ultraGridSaleOrderDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      int cancelQty = DBConvert.ParseInt(e.Cell.Text.Trim());
      if (string.Compare(colName, "Cancel", true) == 0)
      {
        if (cancelQty <= 0 && e.Cell.Text.Trim().Length > 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Cancel Qty" });
          e.Cancel = true;
        }
      }
    }
  }
}