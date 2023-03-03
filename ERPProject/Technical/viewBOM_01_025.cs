/*
  Author      : 
  Description : Reload Carcass
  Date        : 20-09-2011
*/
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
  public partial class viewBOM_01_025 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewBOM_01_025()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewPLN_02_006
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_006_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(tableLayoutSearch);

      this.LoadData();
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      this.LoadComboWorkOrder();
      this.LoadComboCarcassCode();
      this.LoadComboItemCode();
      this.LoadComboRevision();
      this.LoadDropdownResion();
    }

    /// <summary>
    /// Load Combo WorkOrder
    /// </summary>
    private void LoadComboWorkOrder()
    {
      string commandText = "SELECT Pid AS WorkOrder FROM TblPLNWorkOrder WHERE Confirm = 1 AND Status = 0 ORDER BY Pid DESC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dt.NewRow();
      dt.Rows.InsertAt(row, 0);
      ultcbWorkOrder.DataSource = dt;
      ultcbWorkOrder.DisplayMember = "WorkOrder";
      ultcbWorkOrder.ValueMember = "WorkOrder";
      ultcbWorkOrder.DisplayLayout.Bands[0].Columns["WorkOrder"].Width = 250;
    }

    /// <summary>
    /// Load Combo ItemCode
    /// </summary>
    private void LoadComboItemCode()
    {
      string commandText = string.Format("SELECT DISTINCT ItemCode FROM TblPLNWorkOrderConfirmedDetails");
      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dtItem.NewRow();
      dtItem.Rows.InsertAt(row, 0);
      ultcbItemCode.DataSource = dtItem;
      ultcbItemCode.DisplayMember = "ItemCode";
      ultcbItemCode.ValueMember = "ItemCode";
      ultcbItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Width = 250;
      ultcbItemCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Combo CarcassCode
    /// </summary>
    private void LoadComboCarcassCode()
    {
      string commandText = string.Format("SELECT CarcassCode, OldCode, ItemCode, Revision FROM VPLNWorkOrderCarcassList");
      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dtCarcass.NewRow();
      dtCarcass.Rows.InsertAt(row, 0);
      ultcbCarcassCode.DataSource = dtCarcass;
      ultcbCarcassCode.DisplayMember = "CarcassCode";
      ultcbCarcassCode.ValueMember = "CarcassCode";
      ultcbCarcassCode.DisplayLayout.Bands[0].Columns["CarcassCode"].Width = 150;
    }

    /// <summary>
    /// Load Combo Revision
    /// </summary>
    private void LoadComboRevision()
    {
      string commandText = string.Format("SELECT DISTINCT Revision FROM TblPLNWorkOrderConfirmedDetails");
      DataTable dtRevision = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dtRevision.NewRow();
      dtRevision.Rows.InsertAt(row, 0);
      ultcbRevisionCode.DataSource = dtRevision;
      ultcbRevisionCode.DisplayMember = "Revision";
      ultcbRevisionCode.ValueMember = "Revision";
      ultcbRevisionCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbCarcassCode.DisplayLayout.Bands[0].Columns["Revision"].Width = 150;
    }

    private void LoadDropdownResion()
    {
      string commandText = string.Format(@"SELECT Code , Description +' / '+ Value  Name  FROM TblBOMCodeMaster WHERE [Group] = 16004 AND Code >= 100");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultdrResion.DataSource = dt;
      DataRow dr = dt.NewRow();
      dt.Rows.InsertAt(dr, 0);
      ultdrResion.DisplayMember = "Name";
      ultdrResion.ValueMember = "Code";
      ultdrResion.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultdrResion.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultdrResion.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;
      long workOrder = long.MinValue;
      string carcassCode = string.Empty;
      string itemCode = string.Empty;
      string saleCode = string.Empty;
      string oldCode = string.Empty;
      int revision = int.MinValue;

      if (ultcbWorkOrder.Value != null && ultcbWorkOrder.SelectedRow.Index > 0)
      {
        workOrder = DBConvert.ParseLong(ultcbWorkOrder.Value.ToString());
      }
      if (ultcbCarcassCode.Value != null && ultcbCarcassCode.SelectedRow.Index > 0)
      {
        carcassCode = ultcbCarcassCode.Value.ToString();
      }
      if (ultcbItemCode.Value != null && ultcbItemCode.SelectedRow.Index > 0)
      {
        itemCode = ultcbItemCode.Value.ToString();
      }
      if (ultcbRevisionCode.Value != null && ultcbRevisionCode.SelectedRow.Index > 0)
      {
        revision = DBConvert.ParseInt(this.ultcbRevisionCode.Value.ToString());
      }
      if (txtOldCode.Text.Length > 0)
      {
        oldCode = txtOldCode.Text;
      }
      if (txtSaleCode.Text.Length > 0)
      {
        saleCode = txtSaleCode.Text;
      }

      DBParameter[] inputParam = new DBParameter[6];
      if (workOrder != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Wo", DbType.Int64, workOrder);
      }
      if (carcassCode != string.Empty)
      {
        inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
      }
      if (itemCode != string.Empty)
      {
        inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      }
      if (revision != int.MinValue)
      {
        inputParam[3] = new DBParameter("@Revision", DbType.Int32, revision);
      }
      if (saleCode != string.Empty)
      {
        inputParam[4] = new DBParameter("@SaleCode", DbType.AnsiString, 16, saleCode);
      }
      if (oldCode != string.Empty)
      {
        inputParam[5] = new DBParameter("@OldCode", DbType.AnsiString, 16, oldCode);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNListCarcassNeedToReload", inputParam);
      dsPLNListCarcassNeedToReLoad dslist = new dsPLNListCarcassNeedToReLoad();
      if (ds != null)
      {
        dslist.Tables["CarcassReLoad"].Merge(ds.Tables[0]);
        dslist.Tables["ItemReLoad"].Merge(ds.Tables[1]);
        //dslist.Tables["History"].Merge(ds.Tables[2]);
      }
      ultData.DataSource = dslist;
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Save
    /// </summary>
    private bool SaveData()
    {
      long workOrderPid = long.MinValue;
      string carcassCode = string.Empty;

      DataSet dsMain = (DataSet)this.ultData.DataSource;
      DataTable dtMain = dsMain.Tables["CarcassReLoad"];
      string Remark = "";
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        int resultReLoad = string.Compare(dtMain.Rows[i]["ReLoad"].ToString(), "1", true);
        int resultStatus = string.Compare(dtMain.Rows[i]["Status"].ToString(), "1", true);
        if (resultReLoad == 0 && resultStatus == 0)
        {
          if (DBConvert.ParseLong(dtMain.Rows[i]["ReloadResionPid"].ToString()) > 0)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.White;
            workOrderPid = DBConvert.ParseLong(dtMain.Rows[i]["WorkOrderPid"].ToString());
            carcassCode = dtMain.Rows[i]["CarcassCode"].ToString();
            Remark = dtMain.Rows[i]["Remark"].ToString();
            DBParameter[] inputParam = new DBParameter[6];
            inputParam[0] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcassCode);
            inputParam[1] = new DBParameter("@WorkOrderPid", DbType.Int64, workOrderPid);
            inputParam[2] = new DBParameter("@Remark", DbType.String, 4000, Remark);
            inputParam[3] = new DBParameter("@EID", DbType.Int32, SharedObject.UserInfo.UserPid);
            inputParam[4] = new DBParameter("@ReloadResionPid", DbType.Int64, DBConvert.ParseLong(dtMain.Rows[i]["ReloadResionPid"].ToString()));
            int totalerror = DBConvert.ParseInt(dtMain.Rows[i]["TotalError"].ToString());
            if (totalerror >= 0)
            {
              inputParam[5] = new DBParameter("@TotalError", DbType.Int32, totalerror);
            }
            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);

            DataBaseAccess.ExecuteStoreProcedure("spWIPReLoadCarcassForTechnical", inputParam, outputParam);
            int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
            if (result == 0)
            {
              return false;
            }
          }
          else
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
          }
        }
      }
      return true;
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

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Click Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["PLNReLoad"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["WorkOrderPid"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["ReloadResionPid"].ValueList = ultdrResion;
      e.Layout.Bands[0].Columns["ReloadResionPid"].MaxWidth = 300;
      e.Layout.Bands[0].Columns["ReloadResionPid"].MinWidth = 300;
      e.Layout.Bands[0].Columns["ReloadResionPid"].Header.Caption = "Reload Reason";
      e.Layout.Bands[0].Columns["BOM Confirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ReLoad"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Production Locked"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["WIP Run"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;


      e.Layout.Bands[2].Columns["WorkOrderPid"].Hidden = true;
      e.Layout.Bands[2].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[2].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[2].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[2].Columns["ItemCode"].Header.Caption = "Item Code";

      e.Layout.Bands[0].Columns["WorkOrderPid"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["VN Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["EN Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["BOM Confirm"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Production Locked"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WIP Run"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["ReLoad"].CellActivation = Activation.AllowEdit;

      e.Layout.Bands[2].Override.RowAppearance.BackColor = Color.GreenYellow;
      e.Layout.Bands[2].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// ExpandAll
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// After Update Cell
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string strColName = e.Cell.Column.ToString().ToLower();
      if (strColName == "reload")
      {
        int result = string.Compare(e.Cell.Row.Cells["ReLoad"].Value.ToString(), "1", true);
        if (result == 0)
        {
          e.Cell.Row.Cells["Status"].Value = 1;
        }
        else
        {
          e.Cell.Row.Cells["Status"].Value = 0;
        }
      }
    }

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0045");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
    }
    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string strColName = e.Cell.Column.ToString().ToLower();
      if (strColName == "reload")
      {
        if (e.Cell.Row.Cells["BOM Confirm"].Value.ToString() == "1" && e.Cell.Row.Cells["Production Locked"].Value.ToString() != "1" && e.Cell.Row.Cells["WIP Run"].Value.ToString() != "1")
        {
          //
        }
        else
        {
          e.Cancel = true;
        }
      }
    }

    #endregion Event
  }
}
