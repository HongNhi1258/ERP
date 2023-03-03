/*
  Author      : Huynh Thi Bang  
  Date        : 29/01/2019
  Description : Edit Stock Scan Routing

 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWIP_01_005 : MainUserControl
  {
    #region field
    //private int isStart = int.MinValue;
    //private bool selectedChange = true;
    private bool chkhSelect = true;
    #endregion field

    #region function

    public viewWIP_01_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWIP_01_005_Load(object sender, EventArgs e)
    {
      this.LoadComboWorkOrder();
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

    }
    /// <summary>
    /// Load Combo WorkOrder
    /// </summary>
    private void LoadComboWorkOrder()
    {
      string commandText = "SELECT WorkOrderPid FROM TblPLNWorkOrderConfirmedDetails ORDER BY WorkOrderPid DESC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbWo, dt, "WorkOrderPid", "WorkOrderPid", false);
    }
    /// <summary>
    /// Load Combo CarcassCode
    /// </summary>
    private void LoadComboCarcassCode()
    {
      if (ucbWo.Value != null)
      {
        string commandText = string.Format(@"SELECT DISTINCT CONFIRM.CarcassCode, (CONFIRM.CarcassCode + ' | ' + CAR.DescriptionVN) CarcassName
                                            FROM TblPLNWorkOrderConfirmedDetails CONFIRM
                                              INNER JOIN TblBOMCarcass CAR ON CONFIRM.CarcassCode = CAR.CarcassCode
                                             WHERE CONFIRM.WorkOrderPid = {0}", DBConvert.ParseLong(ucbWo.Value));
        DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbCarcass, dtCarcass, "CarcassCode", "CarcassName", false, "CarcassCode");
      }
    }
    /// <summary>
    /// Load Combo Component
    /// </summary>
    private void LoadComboComponentCode()
    {
      if (ucbWo.Value != null && ucbCarcass.Value != null)
      {
        string commandText = string.Format(@"SELECT ComponentCode, (ComponentCode + ' | ' + DescriptionVN) DisplayText
                                            FROM TblPLNWOCarcassInfomation
                                            WHERE  Wo = {0} AND CarcassCode = '{1}'
                                            ORDER BY[No], ComponentCode", DBConvert.ParseLong(ucbWo.Value), ucbCarcass.Value);
        DataTable dtComponent = DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbComponent, dtComponent, "ComponentCode", "DisplayText", false, "ComponentCode");
      }
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      // Kiem tra bat buot nhap WO

      if (ucbWo.Text.Length <= 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "WO");
        return;
      }

      // Kiem tra bat buot nhap Carcass

      if (ucbCarcass.Text.Length <= 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Carcass");
        return;
      }
      int paramNumber = 3;
      string storeName = "spWIPTransactionOfComponentScanner_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      long wo = DBConvert.ParseLong(ucbWo.Value.ToString());
      string carcass = ucbCarcass.Value.ToString();
      inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
      inputParam[1] = new DBParameter("@CarcassCode", DbType.String, carcass);
      if (ucbComponent.Text.Length > 0)
      {
        inputParam[2] = new DBParameter("@ComponentCode", DbType.String, ucbComponent.Value.ToString());
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridInformation.DataSource = dtSource;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Priority"].Value.ToString()) == 1)
        {
          ultraGridInformation.Rows[i].Cells["Selected"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
          ultraGridInformation.Rows[i].Cells["Selected"].Appearance.BackColor = Color.White;
        }
        else
        {
          ultraGridInformation.Rows[i].Cells["Selected"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
          ultraGridInformation.Rows[i].Cells["Selected"].Appearance.BackColor = Color.LightGray;
        }
      }

    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      ucbWo.Value = DBNull.Value;
      ucbCarcass.Value = DBNull.Value;
      ucbComponent.Value = DBNull.Value;
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }

    #endregion function

    #region event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }
    private void btnDeleteAll_Click(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ultraGridInformation.DataSource;
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          long wo = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["Wo"].Value.ToString());
          string carcass = ultraGridInformation.Rows[i].Cells["CarcassCode"].Value.ToString();
          string component = ultraGridInformation.Rows[i].Cells["ComponentCode"].Value.ToString();
          string confirmMessage = string.Format("Are you sure to delete transaction & stock of WO: {0}, Carcass: {1}, Component: {2} ?", wo, carcass, component);
          if (WindowUtinity.ShowMessageConfirmFromText(confirmMessage) == DialogResult.Yes)
          {
            int paramNumber = 4;
            string storeName = "spWIPTransactionOfComponentScanner_DeleteAll";
            DBParameter[] inputParam = new DBParameter[paramNumber];
            inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
            inputParam[1] = new DBParameter("@CarcassCode", DbType.String, carcass);
            if (component.Length > 0)
            {
              inputParam[2] = new DBParameter("@ComponentCode", DbType.String, ultraGridInformation.Rows[i].Cells["ComponentCode"].Value.ToString());
            }
            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, output);
            long reresult = long.Parse(output[0].Value.ToString());
            if (reresult > 0)
            {
              WindowUtinity.ShowMessageSuccess("MSG0002");
              this.SearchData();
            }
            else
            {
              WindowUtinity.ShowMessageError("ERR0004");
              return;
            }
          }
          else
          {
            return;
          }
        }
      }
    }
    private void btnDelete_Click(object sender, EventArgs e)
    {
      DataTable dtDetail = (DataTable)ultraGridInformation.DataSource;
      if (dtDetail != null && dtDetail.Rows.Count > 0)
      {
        foreach (DataRow row in dtDetail.Rows)
        {
          if (DBConvert.ParseInt(row["Selected"].ToString()) == 1)
          {
            int paramNumber = 6;
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            long wo = DBConvert.ParseLong(row["Wo"].ToString());
            string carcass = row["CarcassCode"].ToString();
            string component = row["ComponentCode"].ToString();
            long fromTeam = DBConvert.ParseLong(row["FromTeamPid"].ToString());
            long toTeam = DBConvert.ParseLong(row["ToTeamPid"].ToString());
            string confirmMessage = string.Format("Are you sure to delete transaction of WO: {0}, Carcass: {1}, Component: {2} ?", wo, carcass, component);
            if (WindowUtinity.ShowMessageConfirmFromText(confirmMessage) == DialogResult.Yes)
            {
              string storeName = "spWIPTransactionOfComponentScanner_Delete";
              DBParameter[] inputParam = new DBParameter[paramNumber];
              inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
              inputParam[1] = new DBParameter("@CarcassCode", DbType.String, carcass);
              inputParam[2] = new DBParameter("@ComponentCode", DbType.String, component);
              inputParam[3] = new DBParameter("@FromTeam", DbType.Int64, fromTeam);
              inputParam[4] = new DBParameter("@ToTeam", DbType.Int64, toTeam);
              inputParam[5] = new DBParameter("@Pid", DbType.Int64, pid);

              DBParameter[] output = new DBParameter[1];
              output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, output);
              long reresult = long.Parse(output[0].Value.ToString());
              if (reresult > 0)
              {
                WindowUtinity.ShowMessageSuccess("MSG0002");
                this.SearchData();
              }
              else
              {
                WindowUtinity.ShowMessageError("ERR0004");
                return;
              }
            }
            else
            {
              return;
            }
          }
        }
      }
    }
    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void ucbWo_ValueChanged(object sender, EventArgs e)
    {
      this.LoadComboCarcassCode();
    }

    private void ucbCarcass_ValueChanged(object sender, EventArgs e)
    {
      this.LoadComboComponentCode();
    }
    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AutoResizeColumnWidthOptions = Infragistics.Win.UltraWinGrid.AutoResizeColumnWidthOptions.All;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CarcassName"].Header.Caption = "Carcass Name";
      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Component Code";
      e.Layout.Bands[0].Columns["ComponentName"].Header.Caption = "Component Name";
      e.Layout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
      e.Layout.Bands[0].Columns["StockQty"].Header.Caption = "Stock Qty";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["InputHour"].Header.Caption = "Input Hour";

      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Priority"].Hidden = true;
      e.Layout.Bands[0].Columns["ToTeamPid"].Hidden = true;
      e.Layout.Bands[0].Columns["FromTeamPid"].Hidden = true;

      e.Layout.Bands[0].Columns["Wo"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Wo"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["TotalQty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["TotalQty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["StockQty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["StockQty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["ComponentCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["ComponentCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["CarcassName"].MinWidth = 300;
      e.Layout.Bands[0].Columns["CarcassName"].MaxWidth = 300;
      e.Layout.Bands[0].Columns["CreateDate"].MinWidth = 100;
      e.Layout.Bands[0].Columns["CreateDate"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["InputHour"].MinWidth = 80;
      e.Layout.Bands[0].Columns["InputHour"].MaxWidth = 80;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Selected"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      }

      // Set color
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (e.Layout.Bands[0].Columns[i].CellActivation == Infragistics.Win.UltraWinGrid.Activation.AllowEdit)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
      }
    }

    #endregion event

    private void ultraGridInformation_AfterCellActivate(object sender, EventArgs e)
    {
      //if (DBConvert.ParseLong(ultraCbToTeam.Value) != long.MinValue)
      //{
      //  string commandText = string.Format(@" SELECT AREA.IsStart
      //                                        FROM	  TblWIPWorkArea AREA 
      //                                        WHERE	AREA.Pid = {0}
      //                                        ORDER	BY AREA.Pid ", DBConvert.ParseLong(ultraCbToTeam.Value));
      //  DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      //  if (dt != null && dt.Rows.Count > 0)
      //  {
      //    this.isStart = DBConvert.ParseInt(dt.Rows[0]["IsStart"].ToString());
      //  }
      //}
      //if (this.selectedChange && string.Compare(ultraGridInformation.ActiveCell.Column.ToString(), "Selected", true) == 0)
      //{
      //  this.selectedChange = false;
      //  chkSelectAll.Checked = false;
      //  this.selectedChange = true;
      //}
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

  }
}
