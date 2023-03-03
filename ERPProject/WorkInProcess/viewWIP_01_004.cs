/*
  Author      : Huynh Thi Bang  
  Date        : 14/09/2017
  Description : Tranfer Barcode

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
  public partial class viewWIP_01_004 : MainUserControl
  {
    #region field
    private int isStart = int.MinValue;
    private bool selectedChange = true;
    private bool chkhSelect = true;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadWIPProcess();
      this.LoadFromTeam();
      this.LoadToTeam();
    }
    private void LoadWIPProcess()
    {
      string cm = string.Format(@"SELECT DISTINCT P.Pid, (P.ProcessCode + ' | ' + P.VNDescription) ProcessCode
                                  FROM	 TblBOMProcessInfo P
                                  ORDER BY ProcessCode");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      Utility.LoadUltraCombo(ultCBProcess, dt, "Pid", "ProcessCode", "Pid");
      ultCBProcess.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    public viewWIP_01_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWIP_01_004_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

      //Init Data
      this.InitData();
    }
    /// <summary>
    /// Load From Team
    /// </summary>
    private void LoadFromTeam()
    {
      string str = "";
      str = @"SELECT Pid TeamPid,  WorkAreaNameVN TeamName
	            FROM TblWIPWorkArea
	            WHERE DevisionCode = 'COM' AND ISNULL(IsDeleted, 0) = 0
	            ORDER BY OrderBy";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(str, 90);
      Utility.LoadUltraCombo(ultraCbFromTeam, dt, "TeamPid", "TeamName", "TeamPid");

    }
    /// <summary>
    /// Load To Team
    /// </summary>
    private void LoadToTeam()
    {
      string str = "";
      str = @"SELECT Pid TeamPid,  WorkAreaNameVN TeamName
	            FROM TblWIPWorkArea
	            WHERE DevisionCode = 'COM' AND ISNULL(IsDeleted, 0) = 0
	            ORDER BY OrderBy";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(str, 90);
      Utility.LoadUltraCombo(ultraCbToTeam, dt, "TeamPid", "TeamName", "TeamPid");

    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {

      int paramNumber = 3;
      string storeName = "spWIPWOComponentStatus_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      long wo = DBConvert.ParseLong(txtWo.Text.Trim().ToString());
      string carcass = txtCarcassCode.Text.Trim().ToString();
      string component = txtComponentCode.Text.Trim().ToString();
      if (wo > 0)
      {
        inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
      }
      if (carcass.Length > 0)
      {
        inputParam[1] = new DBParameter("@CarcassCode", DbType.String, carcass);
      }
      if (component.Length > 0)
      {
        inputParam[2] = new DBParameter("@ComponentCode", DbType.String, component);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridInformation.DataSource = dtSource;
      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));

    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtWo.Text = null;
      txtCarcassCode.Text = null;
      txtComponentCode.Text = null;
      ultraCbFromTeam.Value = null;
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

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      if (ultraCbToTeam.Text.Length == 0)
      {
        errorMessage = "To Team";
        return false;
      }
      if (ultCBProcess.Text.Length == 0)
      {
        errorMessage = "Process";
        return false;
      }
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        int select = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Selected"].Value);
        if (select == 1)
        {
          int transferQty = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["TransferQty"].Value);
          if (transferQty <= 0)
          {
            errorMessage = "Transfer Qty";
            return false;
          }
        }
      }
      return true;
    }

    private bool TransferLocationInfo(long fromTeam, long toTeam, int transferQty, string barcode, long processPid, int isLeaf, long compPid)
    {

      string store = "spWIPTransactionInfoDetailScanner_Insert";
      DBParameter[] inputParam1 = new DBParameter[14];
      //inputParam1[0] = new DBParameter("@TransactionPid", DbType.Int64, this.transactionPid);
      if (fromTeam < 0)
      {
        if (isLeaf == 0)
        {
          string commandText = string.Format(@"SELECT TOP 1 STO.WorkAreaPid
                                              FROM TblPLNWOCarcassCompStruct STRU
	                                              INNER JOIN TblPLNWOCarcassInfomation CAR ON STRU.MainCompPid = {0} AND CAR.Pid = STRU.SubCompPid
	                                              INNER JOIN TblWIPWorkAreaStore STO ON CAR.Wo = STO.WorkOrderPid AND CAR.CarcassCode = STO.CarcassCode
		                                              AND CAR.ComponentCode = STO.ComponentCode AND STO.Qty > 0", compPid);
          DataTable dtFromTeam = DataBaseAccess.SearchCommandTextDataTable(commandText);
          fromTeam = (dtFromTeam.Rows.Count > 0 ? DBConvert.ParseLong(dtFromTeam.Rows[0][0]) : long.MinValue);
        }
        else
        {
          fromTeam = 45; //Start (Ra Phoi)
        }
        inputParam1[1] = new DBParameter("@FromTeam", DbType.Int64, fromTeam);
      }
      else
      {
        inputParam1[1] = new DBParameter("@FromTeam", DbType.Int64, fromTeam);
      }
      inputParam1[2] = new DBParameter("@ToTeam", DbType.Int64, toTeam);
      inputParam1[3] = new DBParameter("@Qty", DbType.Int32, transferQty);
      inputParam1[4] = new DBParameter("@BarCode", DbType.AnsiString, 32, barcode);
      inputParam1[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam1[6] = new DBParameter("@QC", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam1[7] = new DBParameter("@FlagReturn", DbType.Int32, 0);
      inputParam1[8] = new DBParameter("@Process", DbType.Int64, processPid);
      DBParameter[] outputParam1 = new DBParameter[1];
      outputParam1[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure(store, inputParam1, outputParam1);
      long result = long.Parse(outputParam1[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      else if (result == 2)
      {
        MessageBox.Show("Carcass đang được Reload!");
        return false;
      }

      return true;
    }

    private bool SaveData()
    {
      bool success = true;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        long fromTeam = long.MinValue;
        int stockQty = int.MinValue;
        if (ultraCbFromTeam.Value != null)
        {
          fromTeam = DBConvert.ParseLong(ultraCbFromTeam.Value);
          long wo = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Wo"].Value);
          string carcass = ultraGridInformation.Rows[i].Cells["CarcassCode"].Value.ToString();
          string component = ultraGridInformation.Rows[i].Cells["ComponentCode"].Value.ToString();
          string commandText = string.Format(@"SELECT Qty
                                             FROM TblWIPWorkAreaStore
                                             WHERE WorkOrderPid = {0} and CarcassCode = '{1}' and ComponentCode = '{2}' AND WorkAreaPid = {3} ", wo, carcass, component, DBConvert.ParseLong(ultraCbFromTeam.Value));
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt != null && dt.Rows.Count > 0)
          {
            stockQty = DBConvert.ParseInt(dt.Rows[0]["Qty"].ToString());
          }
        }
        else
        {
          fromTeam = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["FromTeam"].Value);
          stockQty = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["StockQty"].Value);
        }

        int transferQty = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["TransferQty"].Value);
        stockQty = (stockQty <= 0 ? 0 : stockQty);
        //int isStart = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["IsStart"].Value);
        long toTeam = DBConvert.ParseLong(ultraCbToTeam.Value);
        string barcode = ultraGridInformation.Rows[i].Cells["Barcode"].Value.ToString();
        long processPid = DBConvert.ParseLong(ultCBProcess.Value);
        int select = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Selected"].Value);
        int isLeaf = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["IsLeaf"].Value);
        long compPid = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CompPid"].Value);

        if (select == 1)
        {
          if (transferQty > stockQty && this.isStart == 1)
          {
            if (MessageBox.Show("Số lượng không đủ, bạn muốn ra phôi thêm không?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
              break;
            }
            else
            {
              string store = "spWIPTransactionComponentStart_Scanner";   //ra phoi them so luong du de to ra phoi transfer di
              DBParameter[] input = new DBParameter[6];
              input[0] = new DBParameter("@FromTeam", DbType.Int64, 45);
              input[1] = new DBParameter("@ToTeam", DbType.Int64, fromTeam);
              input[2] = new DBParameter("@Qty", DbType.Int32, transferQty - stockQty);
              input[3] = new DBParameter("@BarCode", DbType.AnsiString, 32, barcode);
              input[4] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              input[5] = new DBParameter("@QC", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              DBParameter[] outputParam1 = new DBParameter[1];
              outputParam1[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              DataBaseAccess.ExecuteStoreProcedure(store, input, outputParam1);
              long result = long.Parse(outputParam1[0].Value.ToString());
              if (result <= 0)
              {
                MessageBox.Show("Số lượng ra phôi vượt quá cho phép!");
                break;
              }
            }
          }

          success = this.TransferLocationInfo(fromTeam, toTeam, transferQty, barcode, processPid, isLeaf, compPid);
          if (!success)
          {
            success = false;
          }
        }
      }
      return success;
    }
    private void btnSave_Click(object sender, EventArgs e)
    {
      string errorMessage = string.Empty;
      if (this.CheckValid(out errorMessage))
      {
        if (this.SaveData())
        {
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SearchData();
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
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

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
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

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AutoResizeColumnWidthOptions = Infragistics.Win.UltraWinGrid.AutoResizeColumnWidthOptions.All;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CarcassName"].Header.Caption = "Carcass Name";
      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Component Code";
      e.Layout.Bands[0].Columns["ComponentName"].Header.Caption = "Component Name";
      e.Layout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
      e.Layout.Bands[0].Columns["StockQty"].Header.Caption = "Stock Qty";
      e.Layout.Bands[0].Columns["StockTeam"].Header.Caption = "Stock Team";

      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["FromTeam"].Hidden = true;
      e.Layout.Bands[0].Columns["IsStart"].Hidden = true;
      e.Layout.Bands[0].Columns["IsLeaf"].Hidden = true;
      e.Layout.Bands[0].Columns["CompPid"].Hidden = true;
      e.Layout.Bands[0].Columns["DefaultTransferQty"].Hidden = true;

      e.Layout.Bands[0].Columns["Wo"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Wo"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["TotalQty"].MinWidth = 60;
      e.Layout.Bands[0].Columns["TotalQty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["ComponentCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["ComponentCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["CarcassName"].MinWidth = 300;
      e.Layout.Bands[0].Columns["CarcassName"].MaxWidth = 300;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Selected"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      e.Layout.Bands[0].Columns["TransferQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
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

    private void ultraCbToTeam_ValueChanged(object sender, EventArgs e)
    {
      if (DBConvert.ParseLong(ultraCbFromTeam.Value) != long.MinValue)
      {
        string commandText = string.Format(@" SELECT AREA.IsStart
                                              FROM	  TblWIPWorkArea AREA 
                                              WHERE	AREA.Pid = {0}
                                              ORDER	BY AREA.Pid ", DBConvert.ParseLong(ultraCbFromTeam.Value));
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          this.isStart = DBConvert.ParseInt(dt.Rows[0]["IsStart"].ToString());
        }
      }
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkhSelect)
      {
        int checkAll = (chkSelectAll.Checked ? 1 : 0);
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (ultraGridInformation.Rows[i].IsFilteredOut == false && ultraGridInformation.Rows[i].Cells["Selected"].Activation != Infragistics.Win.UltraWinGrid.Activation.ActivateOnly)
          {
            ultraGridInformation.Rows[i].Cells["Selected"].Value = checkAll;
            if (checkAll == 0)
            {
              ultraGridInformation.Rows[i].Cells["TransferQty"].Value = DBNull.Value;
            }
            else
            {
              if (ultraGridInformation.Rows[i].Cells["TransferQty"].Value.ToString().Trim().Length == 0)
              {
                ultraGridInformation.Rows[i].Cells["TransferQty"].Value = ultraGridInformation.Rows[i].Cells["DefaultTransferQty"].Value;
              }
            }
          }
        }
      }
    }

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

    private void ultraGridInformation_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare("Selected", column, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          this.chkhSelect = false;
          chkSelectAll.Checked = false;
          this.chkhSelect = true;
          e.Cell.Row.Cells["TransferQty"].Value = DBNull.Value;
        }
        else
        {
          int transferQty = DBConvert.ParseInt(e.Cell.Row.Cells["TransferQty"].Value);
          if (transferQty <= 0)
          {
            int stockQty = DBConvert.ParseInt(e.Cell.Row.Cells["StockQty"].Value);
            int totalQty = DBConvert.ParseInt(e.Cell.Row.Cells["TotalQty"].Value);
            transferQty = (stockQty <= 0 ? totalQty : stockQty);
            e.Cell.Row.Cells["TransferQty"].Value = transferQty;
          }
        }
      }
    }
  }
}
