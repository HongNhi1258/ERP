using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Planning;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_012 : MainUserControl
  {
    private int flag = 0;

    public viewPLN_02_012()
    {
      InitializeComponent();
    }

    /// <summary>
    /// load view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_012_Load(object sender, EventArgs e)
    {
      this.LoadcbWorkOrder();
      this.LoadcbItemCode();
      this.LoadcbRevision();
    }

    /// <summary>
    /// load revision
    /// </summary>
    private void LoadcbRevision()
    {
      string commandText = string.Format("SELECT DISTINCT Revision FROM TblPLNWOInfoDetailGeneral ORDER BY Revision");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dt.NewRow();
      dt.Rows.InsertAt(row, 0);
      ultCBRevision.DataSource = dt;
      ultCBRevision.DisplayLayout.AutoFitColumns = true;
      ultCBRevision.DisplayMember = "Revision";
      ultCBRevision.ValueMember = "Revision";
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
      ultCBItemCode.DataSource = dt;
      ultCBItemCode.DisplayLayout.AutoFitColumns = true;
      ultCBItemCode.DisplayMember = "ItemCode";
      ultCBItemCode.ValueMember = "ItemCode";
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
      ultCBWorkOrder.DataSource = dt;
      ultCBWorkOrder.DisplayLayout.AutoFitColumns = true;
      ultCBWorkOrder.DisplayMember = "Pid";
      ultCBWorkOrder.ValueMember = "Pid";
    }

    /// <summary>
    /// search click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (ultCBWorkOrder.Value == null)
      {
        WindowUtinity.ShowMessageWarning("WRN0013", "Work Order");
        return;
      }
      this.search();
    }

    /// <summary>
    /// search khi click button search
    /// </summary>
    private void search()
    {
      string store = "spPLNAdjustQtyWorkOrder";
      DBParameter[] param = new DBParameter[4];

      if (ultCBWorkOrder.Value != null)
      {
        param[0] = new DBParameter("@WorkOrder", DbType.Int64, DBConvert.ParseLong(ultCBWorkOrder.Value.ToString()));
      }
      if (ultCBItemCode.Value != null)
      {
        param[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultCBItemCode.Value.ToString());
      }
      if (ultCBRevision.Text.Trim().Length > 0 && DBConvert.ParseInt(ultCBRevision.Text.Trim()) != int.MinValue)
      {
        param[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(ultCBRevision.Text.ToString()));
      }
      if (SharedObject.UserInfo.Department == "PLA")
      {
        param[3] = new DBParameter("@Department", DbType.Int32, 1);
      }
      else
      {
        param[3] = new DBParameter("@Department", DbType.Int32, 2);
      }

      DataSet dsSearch = DataBaseAccess.SearchStoreProcedure(store, param);
      dsPLNItemForContainerListFromWO dsItem = new dsPLNItemForContainerListFromWO();
      dsItem.Tables["WOItem"].Merge(dsSearch.Tables[0]);
      dsItem.Tables["SOItem"].Merge(dsSearch.Tables[1]);

      ultGrid.DataSource = dsItem;

      if (SharedObject.UserInfo.Department == "PLA")
      {
        for (int i = 0; i < ultGrid.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          string columnName = ultGrid.DisplayLayout.Bands[0].Columns[i].Header.Caption;
          if (string.Compare(columnName, "AdjustQty", true) == 0 || string.Compare(columnName, "Select", true) == 0 || string.Compare(columnName, "ReasonAdjust", true) == 0)
          {
            ultGrid.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
          }
          else
          {
            ultGrid.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
          }
        }

        for (int i = 0; i < ultGrid.DisplayLayout.Bands[1].Columns.Count; i++)
        {
          string columnName = ultGrid.DisplayLayout.Bands[1].Columns[i].Header.Caption;
          if (string.Compare(columnName, "AdjustQty", true) == 0 || string.Compare(columnName, "Select", true) == 0)
          {
            ultGrid.DisplayLayout.Bands[1].Columns[i].CellActivation = Activation.AllowEdit;
          }
          else
          {
            ultGrid.DisplayLayout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
          }
        }
      }
      if (SharedObject.UserInfo.Department == "COM1" || SharedObject.UserInfo.Department == "COM2" || SharedObject.UserInfo.Department == "CAR")
      {
        for (int i = 0; i < ultGrid.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          string columnName = ultGrid.DisplayLayout.Bands[0].Columns[i].Header.Caption;
          if (string.Compare(columnName, "Select", true) == 0 || string.Compare(columnName, "Adjust", true) == 0)
          {
            ultGrid.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
          }
          else
          {
            ultGrid.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
          }
        }

        for (int i = 0; i < ultGrid.DisplayLayout.Bands[1].Columns.Count; i++)
        {
          string columnName = ultGrid.DisplayLayout.Bands[1].Columns[i].Header.Caption;
          if (string.Compare(columnName, "Select", true) == 0 || string.Compare(columnName, "Adjust", true) == 0)
          {
            ultGrid.DisplayLayout.Bands[1].Columns[i].CellActivation = Activation.AllowEdit;
          }
          else
          {
            ultGrid.DisplayLayout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
          }
        }
        ultGrid.DisplayLayout.Bands[0].Columns["Select"].Header.Caption = "Adjust";
        ultGrid.DisplayLayout.Bands[1].Columns["Select"].Header.Caption = "Adjust";
      }
      for (int i = 0; i < ultGrid.Rows.Count; i++)
      {
        for (int j = 0; j < ultGrid.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          if (DBConvert.ParseInt(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["AdjustQty"].Value.ToString()) > 0)
          {
            ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["AdjustQty"].Appearance.BackColor = Color.Yellow;
          }
        }
      }
      chkExpandAll.Checked = false;
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
    /// expand all grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultGrid.Rows.ExpandAll(true);
      }
      else
      {
        ultGrid.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// save click - save confirm or delete item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool check = true;
      //if (SharedObject.UserInfo.Department == "PLA")
      //{
      //  check = this.save("spPLNWorkOrderAdjustQty_Request");
      //}
      //else if (SharedObject.UserInfo.Department == "COM1" || SharedObject.UserInfo.Department == "COM2" || SharedObject.UserInfo.Department == "CAR")
      //{
      //  check = this.save("spPLNWorkOrderAdjustQty_Response");
      //}
      check = this.save("spPLNWorkOrderAdjustQty_Update");
      if (check == true)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return;
      }
      this.search();
    }

    private bool CheckIsValid()
    {
      for (int i = 0; i < ultGrid.Rows.Count; i++)
      {
        for (int j = 0; j < ultGrid.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          if (DBConvert.ParseInt(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["Select"].Value.ToString()) == 1)
          {
            DBParameter[] param = new DBParameter[3];

            if (ultGrid.Rows[i].ChildBands[0].Rows[j].Cells.Exists("WorkOrderPid"))
            {
              param[0] = new DBParameter("@WorkOrder", DbType.Int64, DBConvert.ParseLong(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["WorkOrderPid"].Value.ToString()));
            }
            else
            {
              param[0] = new DBParameter("@WorkOrder", DbType.Int64, DBConvert.ParseLong(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["WoInfoPID"].Value.ToString()));
            }
            param[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["ItemCode"].Value.ToString());
            param[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["Revision"].Value.ToString()));

            DBParameter[] paramOut = new DBParameter[1];
            paramOut[0] = new DBParameter("@Result", DbType.Int64, 0);

            DataBaseAccess.ExecuteStoreProcedure("spPLNAdjustQtyWork_CheckCST", param, paramOut);
            if (DBConvert.ParseLong(paramOut[0].Value.ToString()) < 0)
            {
              WindowUtinity.ShowMessageError("ERRO124", "WOQty", "CSTQty (Not enought Carcass in CST)");
              return false;
            }
          }
        }
      }
      return true;
    }
    private bool save(string storeName)
    {
      if (this.CheckIsValid())
      {
        for (int i = 0; i < ultGrid.Rows.Count; i++)
        {
          for (int j = 0; j < ultGrid.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            if (DBConvert.ParseInt(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["Select"].Value.ToString()) == 1)
            {
              DBParameter[] param = new DBParameter[7];

              param[0] = new DBParameter("@WorkOrder", DbType.Int64, DBConvert.ParseLong(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["WoInfoPID"].Value.ToString()));
              param[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["ItemCode"].Value.ToString());
              param[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["Revision"].Value.ToString()));
              param[3] = new DBParameter("@WIDPid", DbType.Int64, DBConvert.ParseLong(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["WIDPid"].Value.ToString()));
              param[4] = new DBParameter("@AdjustQty", DbType.Int32, DBConvert.ParseInt(ultGrid.Rows[i].ChildBands[0].Rows[j].Cells["AdjustQty"].Value.ToString()));
              param[5] = new DBParameter("@AdjustBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              param[6] = new DBParameter("@Reason", DbType.AnsiString, 256, ultGrid.Rows[i].Cells["ReasonAdjust"].Value.ToString());

              DBParameter[] paramOut = new DBParameter[1];
              paramOut[0] = new DBParameter("@Result", DbType.Int64, 0);

              try
              {
                DataBaseAccess.ExecuteStoreProcedure(storeName, param, paramOut);
                if (DBConvert.ParseLong(paramOut[0].Value.ToString()) == 0)
                {
                  return false;
                }
              }
              catch { return false; }
            }
          }
        }
        return true;
      }
      else
      {
        return false;
      }
    }

    private void ultGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["ReasonAdjust"].CellMultiLine = DefaultableBoolean.True;
      e.Layout.Override.RowSizing = RowSizing.AutoFree;
      e.Layout.Bands[0].Columns["Wo"].Hidden = true;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["TotalQty"].Header.Caption = "WoQty";
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      if (e.Layout.Bands[1].Columns.Exists("NeedAdjust"))
      {
        e.Layout.Bands[1].Columns["NeedAdjust"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      e.Layout.Bands[1].Columns["ScheduleDelivery"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["ScheduleDelivery"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["Qty"].Header.Caption = "WoQty";
      e.Layout.Bands[1].Columns["WoInfoPID"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;
      e.Layout.Bands[1].Columns["WIDPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[1].Override.RowAppearance.BackColor = Color.DarkSeaGreen;
    }

    private void ultGrid_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (flag == 1)
      {
        return;
      }
      string colName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      if (colName == "adjustqty")
      {
        if (DBConvert.ParseInt(e.Cell.Row.Cells["AdjustQty"].Text.ToString()) == int.MinValue)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Qty");
          e.Cancel = true;
        }
        else if (DBConvert.ParseInt(e.Cell.Row.Cells["AdjustQty"].Text.ToString()) < 0)
        {
          WindowUtinity.ShowMessageError("ERR0036");
          e.Cancel = true;
        }
        //if (e.Cell.Row.Cells.Exists("SORemain"))
        //{
        //  if (DBConvert.ParseInt(e.Cell.Row.Cells["AdjustQty"].Text.ToString()) > DBConvert.ParseInt(e.Cell.Row.Cells["SORemain"].Text.ToString()))
        //  {
        //    WindowUtinity.ShowMessageError("ERR0010", "AdjustQty", "SORemain");
        //    e.Cancel = true;
        //  }
        //}
      }
      if (!row.HasChild() && row.Cells.Exists("NeedAdjust"))
      {
        if (DBConvert.ParseInt(row.Cells["NeedAdjust"].Value.ToString()) == 0)
        {
          e.Cancel = true;
        }
      }
    }

    private void ultGrid_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      if (row.HasChild())
      {
        flag = 1;
        if (colName == "adjustqty")
        {
          for (int i = 0; i < e.Cell.Row.ChildBands[0].Rows.Count; i++)
          {
            e.Cell.Row.ChildBands[0].Rows[i].Cells["AdjustQty"].Value = 0;
            e.Cell.Row.ChildBands[0].Rows[i].Cells["Select"].Value = 0;
          }

          int remain = DBConvert.ParseInt(e.Cell.Row.Cells["AdjustQty"].Value.ToString());
          for (int i = 0; i < e.Cell.Row.ChildBands[0].Rows.Count; i++)
          {
            int temp = DBConvert.ParseInt(e.Cell.Row.ChildBands[0].Rows[i].Cells["Qty"].Value.ToString());
            if (remain <= temp)
            {
              e.Cell.Row.ChildBands[0].Rows[i].Cells["AdjustQty"].Value = remain;
              e.Cell.Row.ChildBands[0].Rows[i].Cells["Select"].Value = 1;
              temp = 0;
              break;
            }
            else
            {
              e.Cell.Row.ChildBands[0].Rows[i].Cells["AdjustQty"].Value = DBConvert.ParseInt(e.Cell.Row.ChildBands[0].Rows[i].Cells["Qty"].Value.ToString());
              e.Cell.Row.ChildBands[0].Rows[i].Cells["Select"].Value = 1;
              remain -= DBConvert.ParseInt(e.Cell.Row.ChildBands[0].Rows[i].Cells["Qty"].Value.ToString());
            }
          }
          e.Cell.Row.Cells["Select"].Value = 1;
        }
        if (colName == "select")
        {
          if (e.Cell.Row.Cells["Select"].Value.ToString() == "1")
          {
            for (int i = 0; i < e.Cell.Row.ChildBands[0].Rows.Count; i++)
            {
              if (SharedObject.UserInfo.Department == "PLA")
              {
                if (DBConvert.ParseInt(e.Cell.Row.ChildBands[0].Rows[i].Cells["AdjustQty"].Value.ToString()) >= 0)
                {
                  e.Cell.Row.ChildBands[0].Rows[i].Cells["Select"].Value = 1;
                }
              }
              else
              {
                if (DBConvert.ParseInt(e.Cell.Row.ChildBands[0].Rows[i].Cells["NeedAdjust"].Value.ToString()) > 0)
                {
                  e.Cell.Row.ChildBands[0].Rows[i].Cells["Select"].Value = 1;
                }
              }
            }
          }
          else
          {
            for (int i = 0; i < e.Cell.Row.ChildBands[0].Rows.Count; i++)
            {
              e.Cell.Row.ChildBands[0].Rows[i].Cells["Select"].Value = 0;
            }
          }
        }
        flag = 0;
      }
    }

    private void ultGrid_CellChange(object sender, CellEventArgs e)
    {
      string strColName = e.Cell.Column.ToString().ToLower();
      if (e.Cell.Row.Cells.Exists("SORemain"))
      {
        if (strColName == "select")
        {
          if (e.Cell.Row.Cells.Exists("NeedAdjust"))
          {
            if (e.Cell.Row.Cells["NeedAdjust"].Value.ToString() == "0")
            {
              e.Cell.CancelUpdate();
            }
          }
        }
      }
      if (strColName == "adjustqty")
      {
        DBParameter[] param = new DBParameter[3];

        if (e.Cell.Row.Cells.Exists("WorkOrderPid"))
        {
          param[0] = new DBParameter("@WorkOrder", DbType.Int64, DBConvert.ParseLong(e.Cell.Row.Cells["WorkOrderPid"].Value.ToString()));
        }
        else
        {
          param[0] = new DBParameter("@WorkOrder", DbType.Int64, DBConvert.ParseLong(e.Cell.Row.Cells["WoInfoPID"].Value.ToString()));
        }
        param[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, e.Cell.Row.Cells["ItemCode"].Value.ToString());
        param[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString()));

        DBParameter[] paramOut = new DBParameter[1];
        paramOut[0] = new DBParameter("@Result", DbType.Int64, 0);

        DataBaseAccess.ExecuteStoreProcedure("spPLNAdjustQtyWork_CheckCST", param, paramOut);
        if (DBConvert.ParseLong(paramOut[0].Value.ToString()) < 0)
        {
          WindowUtinity.ShowMessageError("ERRO124", "WOQty", "CSTQty (Not enought Carcass in CST)");
          e.Cell.CancelUpdate();
        }
      }
    }

    private void ultCBWorkOrder_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultCBItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultCBRevision_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
  }
}
