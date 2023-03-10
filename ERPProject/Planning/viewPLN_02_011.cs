/*
   Author  : Vo Van Duy Qui
   Email   : qui_it@daico-furniture.com
   Date    : 12-08-2010
   Company : Dai Co   
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
//using DaiCo.ERPProject;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_02_011 : MainUserControl
  {
    #region Field
    public viewPLN_02_002 parentUC = null;
    bool chkIsDefaultQty = false;
    public long WoDetail;
    public long WorkOrderPID;
    public int Priority;
    public int isSubCon;
    #endregion Field

    #region Init

    public viewPLN_02_011()
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
      if (ds != null)
      {
        ds.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { ds.Tables[0].Columns["ItemCode"],
                                                                                 ds.Tables[0].Columns["Revision"], ds.Tables[0].Columns["SaleOrderDetailPid"]},
                                                    new DataColumn[] { ds.Tables[1].Columns["ItemCode"],
                                                                                ds.Tables[1].Columns["Revision"], ds.Tables[1].Columns["OldSODPid"]}, false));
        ultData.DataSource = ds;
        ultData.Rows.ExpandAll(true);

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGray;
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
            //ultData.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.YellowGreen;
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
            if (DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Wo"].Value.ToString()) == WorkOrderPID)
            {
              ultData.Rows[i].Cells["IsSubCon"].Value = isSubCon;
            }
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
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
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
            if (qty < 0 || qty > remain)
            {
              chkIsFalse = true;
            }
            //int totalOpenQty = 0;
            //int totalOpenWo = 0;
            //DataSet dtSource = (DataSet)ultData.DataSource;

            //for (int i = 0; i < ultData.Rows.Count; i++)
            //{
            //  if (ultData.Rows[i].Cells["OpenQty"].Text.ToString().Length > 0)
            //  {
            //    totalOpenQty += DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Text.ToString());
            //  }
            //  if (ultData.Rows[i].Cells["WoQtyPid"].Text.ToString().Length > 0)
            //  {
            //    totalOpenWo += DBConvert.ParseInt(ultData.Rows[i].Cells["WoQtyPid"].Text.ToString());
            //  }
            //  for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
            //  {
            //    if (ultData.Rows[i].Cells["OpenQty"].Text.ToString().Length > 0)
            //    {
            //      totalOpenQty += DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Text.ToString());
            //    }
            //    if (ultData.Rows[i].Cells["WoQtyPid"].Text.ToString().Length > 0)
            //    {
            //      totalOpenWo += DBConvert.ParseInt(ultData.Rows[i].Cells["WoQtyPid"].Text.ToString());
            //    }
            //  }
            //}
            //for (int i = 0; i < dtSource.Tables[1].Rows.Count; i++)
            //{
            //  if (dtSource.Tables[1].Rows[i]["OpenQty"].ToString().Length > 0)
            //  {
            //    totalOpenQty += DBConvert.ParseInt(dtSource.Tables[1].Rows[i]["OpenQty"].ToString());
            //  }
            //}
            //if (totalOpenQty > totalOpenWo)
            //{
            //  if (!e.Cancel)
            //  {
            //    chkIsFalse = true;
            //  }
            //}
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

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Header.Caption = "Sale No";
      e.Layout.Bands[0].Columns["SaleNo"].Width = 150;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Schedule Delivery";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["CancelQty"].Header.Caption = "Cancel Qty";
      e.Layout.Bands[0].Columns["OpenWo"].Header.Caption = "Open Wo";
      e.Layout.Bands[0].Columns["OpenQty"].Header.Caption = "Open Qty";
      e.Layout.Bands[1].Columns["OpenQty"].Header.Caption = "Open Qty";
      e.Layout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands[0].Columns["PresentShipDate"].Header.Caption = "Present Ship Date";
      e.Layout.Bands[1].Columns["ShipDate"].Header.Caption = "Ship Date (Release WO) ";
      e.Layout.Bands[0].Columns["OpenQtyOld"].Hidden = true;
      e.Layout.Bands[0].Columns["IsSubCon"].Hidden = true;
      e.Layout.Bands[1].Columns["IsSubCon"].Hidden = true;

      e.Layout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsSubCon"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[1].Columns["ShipDate"].Format = ConstantClass.FORMAT_DATETIME;

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
      bool load = true;
      string Out = "";
      int totalOpenQty = 0;
      int totalOpenQtyOld = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        totalOpenQty += DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Text.ToString());
        totalOpenQtyOld += DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQtyOld"].Text.ToString());
      }
      if (totalOpenQty != totalOpenQtyOld)
      {
        WindowUtinity.ShowMessageError("ERR0001", new string[] { "OpenQty" });
        return;
      }
      else
      {
        for (int y = 0; y < ultData.Rows.Count; y++)
        {
          if (DBConvert.ParseInt(ultData.Rows[y].Cells["OpenQty"].Value.ToString()) != DBConvert.ParseInt(ultData.Rows[y].Cells["OpenQtyOld"].Value.ToString()))
          {
            if (DBConvert.ParseInt(ultData.Rows[y].Cells["OpenQty"].Value.ToString()) == 0)
            {
              load = false;
            }
            string storeName = string.Empty;
            DBParameter[] inputParam = new DBParameter[9];
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            storeName = "spPLNWOInfoDetail_execute";
            inputParam[0] = new DBParameter("@WoInfoPID", DbType.Int64, WorkOrderPID);
            string itemCode = ultData.Rows[y].Cells["ItemCode"].Value.ToString().Trim().Replace("'", "''");
            inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
            int Revision = DBConvert.ParseInt(ultData.Rows[y].Cells["Revision"].Value.ToString());
            inputParam[2] = new DBParameter("@Revision", DbType.Int32, Revision);
            int Qty = DBConvert.ParseInt(ultData.Rows[y].Cells["OpenQty"].Value.ToString());
            inputParam[3] = new DBParameter("@Qty", DbType.Int32, Qty);
            int QtyOld = DBConvert.ParseInt(ultData.Rows[y].Cells["OpenQtyOld"].Value.ToString());
            inputParam[4] = new DBParameter("@QtyOld", DbType.Int32, QtyOld);
            long saleOrderDetailPid = DBConvert.ParseLong(ultData.Rows[y].Cells["SaleOrderDetailPid"].Value.ToString());
            inputParam[5] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleOrderDetailPid);
            inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            if (DBConvert.ParseInt(ultData.Rows[y].Cells["IsSubCon"].Value.ToString()) == 1)
            {
              inputParam[7] = new DBParameter("@IsSubCon", DbType.Int32, 1);
            }
            else
            {
              inputParam[7] = new DBParameter("@IsSubCon", DbType.Int32, 0);
            }
            //int Priority = DBConvert.ParseInt(ultData.Rows[y].Cells["Priority"].Value.ToString());
            if (this.Priority >= 0)
            {
              inputParam[8] = new DBParameter("@Priority", DbType.Int32, Priority);
            }
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
            Out += "," + outValue.ToString();
          }
        }

        DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerPriorityRefeshData_Insert");
        if (Out.IndexOf("-1") > 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", new string[] { "OpenQty" });
        }
        else
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          if (load)
          {
            btnSearch_Click(sender, e);
          }
        }
      }
    }
    private void btnSwapWo_Click(object sender, EventArgs e)
    {
      DataSet dset = (DataSet)ultData.DataSource;
      DataTable dt = dset.Tables[1];
      DataRow[] dr = dt.Select("OpenQty > 0");
      if (dr.Length != 2)
      {
        WindowUtinity.ShowMessageError("ERR0125", "Two WorkOrder");
      }
      else
      {
        if (DBConvert.ParseInt(dr[0]["OpenQty"].ToString()) != DBConvert.ParseInt(dr[1]["OpenQty"].ToString()))
        {
          WindowUtinity.ShowMessageError("ERR0131", "Qty Swap WorkOrder " + dr[0]["Wo"].ToString(), " Qty Swap WorkOrder " + dr[1]["Wo"].ToString());
          return;
        }
        //if (DBConvert.ParseInt(dr[0]["IsSubCon"].ToString()) != DBConvert.ParseInt(dr[1]["IsSubCon"].ToString()))
        //{
        //  WindowUtinity.ShowMessageError("ERR0305");
        //  return;
        //}
        if (DBConvert.ParseInt(dr[0]["Wo"].ToString()) == DBConvert.ParseInt(dr[1]["Wo"].ToString()))
        {
          WindowUtinity.ShowMessageError("ERR0306");
          return;
        }
        else
        {
          DBParameter[] inputswapwo = new DBParameter[13];
          DBParameter[] outputswapwo = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          inputswapwo[0] = new DBParameter("@WoFrom", DbType.Int64, DBConvert.ParseInt(dr[0]["Wo"].ToString()));
          inputswapwo[1] = new DBParameter("@SaleOrderDetailPidFrom", DbType.Int64, DBConvert.ParseLong(dr[0]["OldSODPid"].ToString()));
          inputswapwo[2] = new DBParameter("@ItemCodeFrom", DbType.AnsiString, 16, dr[0]["ItemCode"].ToString());
          inputswapwo[3] = new DBParameter("@RevisionFrom", DbType.Int32, DBConvert.ParseInt(dr[0]["Revision"].ToString()));
          inputswapwo[4] = new DBParameter("@QtyFrom", DbType.Int32, DBConvert.ParseInt(dr[0]["OpenQty"].ToString()));

          inputswapwo[5] = new DBParameter("@WoTo", DbType.Int64, DBConvert.ParseInt(dr[1]["Wo"].ToString()));
          inputswapwo[6] = new DBParameter("@SaleOrderDetailPidTo", DbType.Int64, DBConvert.ParseLong(dr[1]["OldSODPid"].ToString()));
          inputswapwo[7] = new DBParameter("@ItemCodeTo", DbType.AnsiString, 16, dr[1]["ItemCode"].ToString());
          inputswapwo[8] = new DBParameter("@RevisionTo", DbType.Int32, DBConvert.ParseInt(dr[1]["Revision"].ToString()));
          inputswapwo[9] = new DBParameter("@QtyTo", DbType.Int32, DBConvert.ParseInt(dr[1]["OpenQty"].ToString()));
          inputswapwo[10] = new DBParameter("@IsSubCon", DbType.Int32, DBConvert.ParseInt(dr[1]["IsSubCon"].ToString()));
          inputswapwo[11] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          //int Priority = DBConvert.ParseInt(dr[1]["IsSubCon"].ToString());
          if (this.Priority >= 0)
          {
            inputswapwo[12] = new DBParameter("@Priority", DbType.Int32, this.Priority);
          }
          DataBaseAccess.ExecuteStoreProcedure("spPLNSwapWorkOrder_execute", inputswapwo, outputswapwo);
          long outvalueswapwo = DBConvert.ParseLong(outputswapwo[0].Value.ToString());
          if (outvalueswapwo == -1)
          {
            WindowUtinity.ShowMessageError("ERR0005");
          }
          else
          {
            WindowUtinity.ShowMessageSuccess("MSG0004");
            btnSearch_Click(sender, e);
          }
        }
      }
    }

    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.ExpandAll(false);
      }
    }
    #endregion Event
  }
}
