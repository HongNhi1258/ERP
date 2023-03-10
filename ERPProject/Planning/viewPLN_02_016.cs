/*
   Author  : TRAN HUNG
   Email   : hung_it@daico-furniture.com
   Date    : 11/09/2012
   Company : Dai Co   
*/

using DaiCo.Application;
using DaiCo.ERPProject;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Planning.View
{
  public partial class viewPLN_02_016 : MainUserControl
  {
    #region Field
    public long wo;
    public long saleorderdetailswap;
    public long pocanceldetalPid;
    public long saleordercanceldetalPid;
    public long wodetailgeneralPid;
    //public long pocancelPid;
    #endregion Field

    #region Init

    public viewPLN_02_016()
    {
      InitializeComponent();
      btnClose.Click += new EventHandler(btnClose_Click);
      chkShowImage.CheckedChanged += new EventHandler(chkShowImage_CheckedChanged);
      ultData.MouseClick += new MouseEventHandler(ultData_MouseClick);
      ultData.BeforeCellUpdate += new BeforeCellUpdateEventHandler(ultData_BeforeCellUpdate);

    }

    #endregion Init

    #region Event
    /// <summary>
    /// Load Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void LoadData()
    {
      DBParameter[] input = new DBParameter[3];
      input[0] = new DBParameter("@SaleOrderCancelDetailPid", DbType.Int64, pocanceldetalPid);
      //input[1] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleorderdetailpid);
      input[1] = new DBParameter("@Wo", DbType.Int64, wo);
      input[2] = new DBParameter("@WoDetailGeneralPid", DbType.Int64, wodetailgeneralPid);
      DataTable dtsource = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spPLNWOlinkSaleOrder_Select", input);
      ultData.DataSource = dtsource;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Value.ToString()) > 0)
          ultData.Rows[i].Cells["OpenQty"].Appearance.BackColor = Color.Yellow;
      }
      if (ultData.Rows.Count <= 1)
      {
        this.btnSave.Enabled = false;
      }

    }
    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// 
    private void viewPLN_02_011_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    /// <summary>
    /// Close User Control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Select Show Image Or Not Show
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    /// <summary>
    /// Check Valid Of OpenQty's Value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      int qty = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString());
      int openQty = DBConvert.ParseInt(e.Cell.Text.Trim());
      if (string.Compare(columnName, "OpenQty", true) == 0)
      {
        if (openQty < 0)
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Open  Qty" });
          e.Cancel = true;
        }
        else if (openQty > qty)
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERRO124", "Open Qty", "Qty");
          e.Cancel = true;
        }
      }
    }

    /// <summary>
    /// Show Image Of Item Selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    /// <summary>
    /// Init Ultra Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["SaleNo"].Header.Caption = "Sale No";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["OpenQty"].Header.Caption = "Open Qty";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Balance";
      e.Layout.Bands[0].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["QtyOld"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OpenQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Override.CellAppearance.BackColor = Color.LightSkyBlue;
      e.Layout.Bands[0].Override.CellAppearance.ForeColor = Color.OrangeRed;
      //e.Layout.Bands[0].Columns["OpenQty"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["OpenQty"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = HAlign.Right;
      for (int i = 0; i < ultData.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        string columnName = ultData.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if (string.Compare(columnName, "Open Qty", true) == 0)
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.ForeColor = Color.Blue;
        }
        else
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }
    }

    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string output = "";
      int totalQty = 0;
      int totalQtyOld = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int qtyCheck = DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Value.ToString());
        int qtyOldCheck = DBConvert.ParseInt(ultData.Rows[i].Cells["QtyOld"].Value.ToString());
        totalQty += qtyOldCheck;
        totalQtyOld += qtyOldCheck;
      }
      if (totalQty != totalQtyOld)
      {
        WindowUtinity.ShowMessageError("ERR0001", new string[] { "Qty" });
        return;
      }
      for (int y = 0; y < ultData.Rows.Count; y++)
      {
        int qty = DBConvert.ParseInt(ultData.Rows[y].Cells["OpenQty"].Value.ToString());
        int qtyOld = DBConvert.ParseInt(ultData.Rows[y].Cells["QtyOld"].Value.ToString());
        if (qty != qtyOld)
        {
          string storeName = string.Empty;
          DBParameter[] inputParam = new DBParameter[11];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          storeName = "spPLNWOInfoDetailGeneral_SwapWO";
          long woInfoPid = DBConvert.ParseLong(ultData.Rows[y].Cells["WoInfoPID"].Value.ToString());
          inputParam[0] = new DBParameter("@WoInfoPID", DbType.Int64, woInfoPid);
          inputParam[1] = new DBParameter("@WoSwap", DbType.Int64, wo);
          string itemCode = ultData.Rows[y].Cells["ItemCode"].Value.ToString().Trim().Replace("'", "''");
          inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          int Revision = DBConvert.ParseInt(ultData.Rows[y].Cells["Revision"].Value.ToString());
          inputParam[3] = new DBParameter("@Revision", DbType.Int32, Revision);
          inputParam[4] = new DBParameter("@Qty", DbType.Int32, qty);
          inputParam[5] = new DBParameter("@QtyOld", DbType.Int32, qtyOld);
          inputParam[6] = new DBParameter("@SaleOrderDetailSwap", DbType.Int64, saleorderdetailswap);
          long saleOrderDetailPid = DBConvert.ParseLong(ultData.Rows[y].Cells["SaleOrderDetailPid"].Value.ToString());
          inputParam[7] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleOrderDetailPid);
          inputParam[8] = new DBParameter("@SaleOrderCancelDetailPid", DbType.Int64, saleordercanceldetalPid);
          inputParam[9] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          int Priority = DBConvert.ParseInt(ultData.Rows[y].Cells["Priority"].Value.ToString());
          if (Priority >= 0)
          {
            inputParam[10] = new DBParameter("@Priority", DbType.Int32, Priority);

          }
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
          output += "," + outValue.ToString();
        }
      }
      if (output.IndexOf("-1") > 0)
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
    }


    #endregion Event

  }
}
