/*
   Author  : TRAN HUNG
   Email   : hung_it@daico-furniture.com
   Date    : 12-09-2012
   Company : Dai Co   
*/

using DaiCo.Application;
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
  public partial class viewPLN_02_015 : MainUserControl
  {
    #region Field
    bool chkIsDefaultQty = false;
    public long WoDetail;
    public int Priority;
    public long WorkOrderPID;
    public long saleordercanceldetailPid;
    public long parentPid;
    public long pocancelPid;
    #endregion Field

    #region Init

    public viewPLN_02_015()
    {
      InitializeComponent();
      btnClose.Click += new EventHandler(btnClose_Click);
      chkShowImage.CheckedChanged += new EventHandler(chkShowImage_CheckedChanged);
      ultData.MouseClick += new MouseEventHandler(ultData_MouseClick);
      ultData.BeforeCellUpdate += new BeforeCellUpdateEventHandler(ultData_BeforeCellUpdate);

    }

    /// <summary>
    /// Search WorkOrder Detail Info
    /// </summary>
    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@WorkOrderDetailPid", DbType.Int64, this.WoDetail);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNDataForChangeWorkOrderDetail", inputParam);
      ds.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { ds.Tables[0].Columns["ItemCode"],
                                                                                 ds.Tables[0].Columns["Revision"], ds.Tables[0].Columns["SaleOrderDetailPid"]},
                                                  new DataColumn[] { ds.Tables[1].Columns["ItemCode"],
                                                                                ds.Tables[1].Columns["Revision"], ds.Tables[1].Columns["OldSODPid"]}, false));
      ultData.DataSource = ds;
      ultData.Rows.ExpandAll(true);
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].CellAppearance.BackColor = Color.Aqua;
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Value.ToString()) > 0)
        {
          ultData.Rows[i].Cells["OpenQty"].Appearance.BackColor = Color.Yellow;
        }
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Remain"].Value.ToString()) > 0)
        {
          ultData.Rows[i].Cells["Remain"].Appearance.BackColor = Color.Green;
          ultData.Rows[i].Cells["Remain"].Appearance.ForeColor = Color.Yellow;
        }
        else
        {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["WoQtyPid"].Value.ToString()) <= 0)
          {
            ultData.Rows[i].Cells["OpenQty"].Activation = Activation.ActivateOnly;
          }
        }
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          ultData.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.YellowGreen;
          if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["OpenQty"].Value.ToString()) > 0)
          {
            ultData.Rows[i].ChildBands[0].Rows[j].Cells["OpenQty"].Appearance.BackColor = Color.Yellow;
          }
          if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Remain"].Value.ToString()) > 0)
          {
            ultData.Rows[i].ChildBands[0].Rows[j].Cells["Remain"].Appearance.BackColor = Color.Green;
            ultData.Rows[i].ChildBands[0].Rows[j].Cells["Remain"].Appearance.ForeColor = Color.Yellow;
          }
          else
          {
            ultData.Rows[i].ChildBands[0].Rows[j].Cells["OpenQty"].Activation = Activation.ActivateOnly;
          }
        }
      }
    }

    #endregion Init

    #region Event

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_011_Load(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Add Work Order Detail To View_PLN_1002_WorkOrderInfo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //private void btnNew_Click(object sender, EventArgs e)
    //{
    //  if (parentUC != null)
    //  {
    //    parentUC.dtNewDetail = (DataTable)ultData.DataSource;
    //    this.ConfirmToCloseTab();
    //  }
    //}

    /// <summary>
    /// Search WorkOrder Detail Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
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
      DaiCo.Shared.Utility.ControlUtility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    /// <summary>
    /// Check Valid Of OpenQty's Value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (chkIsDefaultQty)
      {
        return;
      }
      string columnName = e.Cell.Column.ToString();
      switch (columnName.ToLower())
      {
        case "openqty":
          if (e.Cell.Text.Length > 0)
          {
            bool chkIsFalse = false;
            int qty = DBConvert.ParseInt(e.Cell.Text);
            int remain = DBConvert.ParseInt(e.Cell.Row.Cells["Remain"].Value.ToString());
            //int WoQty;
            //if (e.Cell.Row.Cells.Exists("WoQtyPid"))
            //{
            //  WoQty = DBConvert.ParseInt(e.Cell.Row.Cells["WoQtyPid"].Value.ToString());
            //}
            //else 
            //{
            //  WoQty = 0;
            //}
            //if (WoQty > 0)
            //{
            //if (qty < 0 || qty > WoQty)
            //{
            //  DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "OpenQty" });
            //  e.Cancel = true;
            //}
            //}
            //else
            //{
            if (qty < 0 || qty > remain)
            {
              chkIsFalse = true;
            }
            //}

            int totalOpenQty = 0;
            int totalOpenWo = 0;
            DataSet dtSource = (DataSet)ultData.DataSource;
            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              if (ultData.Rows[i].Cells["OpenQty"].Text.ToString().Length > 0)
              {
                totalOpenQty += DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Text.ToString());
              }
              if (ultData.Rows[i].Cells["WoQtyPid"].Text.ToString().Length > 0)
              {
                totalOpenWo += DBConvert.ParseInt(ultData.Rows[i].Cells["WoQtyPid"].Text.ToString());
              }
              for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
              {
                if (ultData.Rows[i].Cells["OpenQty"].Text.ToString().Length > 0)
                {
                  totalOpenQty += DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Text.ToString());
                }
                if (ultData.Rows[i].Cells["WoQtyPid"].Text.ToString().Length > 0)
                {
                  totalOpenWo += DBConvert.ParseInt(ultData.Rows[i].Cells["WoQtyPid"].Text.ToString());
                }
              }
            }
            for (int i = 0; i < dtSource.Tables[1].Rows.Count; i++)
            {
              if (dtSource.Tables[1].Rows[i]["OpenQty"].ToString().Length > 0)
              {
                totalOpenQty += DBConvert.ParseInt(dtSource.Tables[1].Rows[i]["OpenQty"].ToString());
              }
            }
            if (totalOpenQty > totalOpenWo)
            {
              if (!e.Cancel)
              {
                chkIsFalse = true;
              }
            }
            if (chkIsFalse)
            {
              DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "OpenQty" });
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// After Cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (chkIsDefaultQty)
      {
        return;
      }
      string columnName = e.Cell.Column.ToString();
      switch (columnName.ToLower())
      {
        case "openqty":
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Show Image Of Item Selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      DaiCo.Shared.Utility.ControlUtility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    /// <summary>
    /// Init Ultra Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      DaiCo.Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (e.Layout.Bands[0].Columns[i].Key.IndexOf("Pid") >= 0)
        {
          e.Layout.Bands[0].Columns[i].Hidden = true;
        }
        if (e.Layout.Bands[0].Columns[i].Key.IndexOf("OpenQty") < 0)
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }
      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        if (e.Layout.Bands[1].Columns[i].Key.IndexOf("Pid") >= 0)
        {
          e.Layout.Bands[1].Columns[i].Hidden = true;
        }
        if (e.Layout.Bands[1].Columns[i].Key.IndexOf("OpenQty") < 0)
        {
          e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;

        }
      }

      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Header.Caption = "Sale No";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Schedule Delivery";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["CancelQty"].Header.Caption = "Cancel Qty";
      e.Layout.Bands[0].Columns["OpenWo"].Header.Caption = "Open Wo";
      e.Layout.Bands[0].Columns["Remain"].Header.Caption = "Balance";

      e.Layout.Bands[0].Columns["OpenQty"].Header.Caption = "Open Qty";
      e.Layout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands[0].Columns["PresentShipDate"].Header.Caption = "Present Ship Date";
      e.Layout.Bands[1].Columns["ShipDate"].Header.Caption = "Ship Date (Release WO) ";
      e.Layout.Bands[0].Columns["OpenQtyOld"].Hidden = true;
      e.Layout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsSubCon"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["CancelQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OpenWo"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SaleNo"].Width = 150;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OpenQty"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["OpenQty"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["CBM Remain"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.000}";

      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = HAlign.Right;
    }

    /// <summary>
    /// Key Up && Down==> For input data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
      {
        return;
      }

      int rowIndex = (e.KeyCode == Keys.Down) ? ultData.ActiveCell.Row.Index + 1 : ultData.ActiveCell.Row.Index - 1;
      int cellIndex = ultData.ActiveCell.Column.Index;

      try
      {
        ultData.Rows[rowIndex].Cells[cellIndex].Activate();
        ultData.PerformAction(UltraGridAction.EnterEditMode, false, false);
      }
      catch { }
    }
    private void btnSave_Click(object sender, EventArgs e)
    {
      string output = "";
      string outputCheck = "";
      int totalOpenQty = 0;
      int totalOpenQtyOld = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int qtyCheck = DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Value.ToString());
        int qtyOldCheck = DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQtyOld"].Value.ToString());
        totalOpenQty += qtyOldCheck;
        totalOpenQtyOld += qtyOldCheck;
        if (qtyCheck != qtyOldCheck)
        {
          DBParameter[] inputParamCheck = new DBParameter[9];
          DBParameter[] outputParamCheck = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          string itemCodeCheck = ultData.Rows[i].Cells["ItemCode"].Value.ToString().Trim().Replace("'", "''");
          inputParamCheck[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCodeCheck);
          int revisionCheck = DBConvert.ParseInt(ultData.Rows[i].Cells["Revision"].Value.ToString());
          inputParamCheck[1] = new DBParameter("@Revision", DbType.Int32, revisionCheck);
          inputParamCheck[2] = new DBParameter("@Qty", DbType.Int32, qtyCheck);
          long saleOrderDetailPidCheck = DBConvert.ParseLong(ultData.Rows[i].Cells["SaleOrderDetailPid"].Value.ToString());
          inputParamCheck[3] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleOrderDetailPidCheck);
          DataBaseAccess.ExecuteStoreProcedure("spPLNWOInfoDetailGeneral_CheckIsValid", inputParamCheck, outputParamCheck);
          long outvalueCheck = DBConvert.ParseLong(outputParamCheck[0].Value.ToString());
          outputCheck += "," + outvalueCheck.ToString();
        }
      }
      if ((totalOpenQty < totalOpenQtyOld) || outputCheck.IndexOf("-1") > 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", new string[] { "OpenQty" });
        return;
      }
      else
      {
        for (int y = 0; y < ultData.Rows.Count; y++)
        {
          int qty = DBConvert.ParseInt(ultData.Rows[y].Cells["OpenQty"].Value.ToString());
          int qtyOld = DBConvert.ParseInt(ultData.Rows[y].Cells["OpenQtyOld"].Value.ToString());
          if (qty != qtyOld)
          {
            string storeName = string.Empty;
            DBParameter[] inputParam = new DBParameter[12];
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            storeName = "spPLNWOInfoDetailGeneral_SwapSO";
            inputParam[0] = new DBParameter("@WoInfoPID", DbType.Int64, WorkOrderPID);
            string itemCode = ultData.Rows[y].Cells["ItemCode"].Value.ToString().Trim().Replace("'", "''");
            inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
            int Revision = DBConvert.ParseInt(ultData.Rows[y].Cells["Revision"].Value.ToString());
            inputParam[2] = new DBParameter("@Revision", DbType.Int32, Revision);
            inputParam[3] = new DBParameter("@Qty", DbType.Int32, qty);
            inputParam[4] = new DBParameter("@QtyOld", DbType.Int32, qtyOld);
            long saleOrderDetailPid = DBConvert.ParseLong(ultData.Rows[y].Cells["SaleOrderDetailPid"].Value.ToString());
            inputParam[5] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleOrderDetailPid);
            inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            inputParam[8] = new DBParameter("@SaleOrderCancelDetailPid", DbType.Int64, saleordercanceldetailPid);
            inputParam[9] = new DBParameter("@ParentPid", DbType.Int64, parentPid);
            inputParam[10] = new DBParameter("@PoCancelPid", DbType.Int64, pocancelPid);
            if (this.Priority >= 0)
            {
              inputParam[11] = new DBParameter("@Priority", DbType.Int32, Priority);
            }
            if (DBConvert.ParseInt(ultData.Rows[y].Cells["IsSubCon"].Value.ToString()) == 1)
            {
              inputParam[7] = new DBParameter("@IsSubCon", DbType.Int32, 1);
            }
            else
            {
              inputParam[7] = new DBParameter("@IsSubCon", DbType.Int32, 0);
            }
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
            output += "," + outValue.ToString();
          }
        }
        if (output.IndexOf("-1") > 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", new string[] { "OpenQty" });
        }
        else
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
      }

    }
    #endregion Event





  }
}
