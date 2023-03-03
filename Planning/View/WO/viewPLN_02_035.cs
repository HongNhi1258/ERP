/*
  Author      : Huynh Thi Bang
  Date        : 18/04/2016
  Description : Close Wo
  Standard Form: viewPLN_02_035.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_035 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    private bool chkhcom1 = true;
    private bool chkcar = true;
    private bool chkfac = true;
    int typeClose = int.MinValue;
    #endregion field

    #region Init
    public viewPLN_02_035()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_035_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);
      this.LoadStatus();
      this.LoadPrecentFinished();
    }
    #endregion Init

    #region function

    //    private void LoadWO()
    //    {
    //      string cm = string.Format(@"SELECT DISTINCT WorkOrderPid Wo
    //                                FROM TblPLNWorkOrderConfirmedDetails
    //                                ORDER BY Wo ");
    //      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
    //      ControlUtility.LoadUltraCombo(ultCBWo, dt, "Wo", "Wo");
    //    }
    /// <summary>
    /// Load CB Status
    /// </summary>
    private void LoadStatus()
    {
      string cm = string.Format(@"SELECT 0 ID, 'All' Name
                                   UNION
                                  SELECT 1 ID, 'Closed COM1' Name
                                   UNION
                                  SELECT 2 ID, 'Not Yet Closed COM1' Name
                                   UNION
                                  SELECT 3 ID, 'Closed CAR' Name
                                   UNION
                                  SELECT 4 ID, 'Not Yet Closed CAR' Name
                                   UNION
                                  SELECT 5 ID, 'Closed Factory' Name
                                   UNION
                                  SELECT 6 ID, 'Not Yet Closed Factory' Name ");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(cm);
      if (dtSource == null)
      {
        return;
      }
      ultCBStatus.DataSource = dtSource;
      ultCBStatus.DisplayMember = "Name";
      ultCBStatus.ValueMember = "ID";
      ultCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBStatus.DisplayLayout.AutoFitColumns = true;
    }

    private void LoadPrecentFinished()
    {
      string cmm = string.Format(@"SELECT 0 ID, 'COM1 100%' Name
                                   UNION
                                  SELECT 1 ID, 'Out Of FGW' Name");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(cmm);
      if (dtSource == null)
      {
        return;
      }
      ultCBPercentFinished.DataSource = dtSource;
      ultCBPercentFinished.DisplayMember = "Name";
      ultCBPercentFinished.ValueMember = "ID";
      ultCBPercentFinished.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBPercentFinished.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBPercentFinished.DisplayLayout.AutoFitColumns = true;
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      string storeName = "spPLNCloseListWo_Select";
      DBParameter[] inputParam = new DBParameter[4];

      long woFrom = DBConvert.ParseLong(txtWoFrom.Text.Trim());
      if (woFrom != long.MinValue)
      {
        inputParam[0] = new DBParameter("@WoFrom", DbType.Int64, woFrom);
      }

      long woTo = DBConvert.ParseLong(txtWoTo.Text.Trim());
      if (woTo != long.MinValue)
      {
        inputParam[1] = new DBParameter("@WoTo", DbType.Int64, woTo);
      }
      if (ultCBStatus.Text.Length > 0 && ultCBStatus.Value != null)
      {
        inputParam[2] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
      }
      if (ultCBPercentFinished.Text.Length > 0 && ultCBPercentFinished.Value != null)
      {
        inputParam[3] = new DBParameter("@PercentFinished", DbType.Int32, DBConvert.ParseInt(ultCBPercentFinished.Value.ToString()));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridInformation.DataSource = dtSource;
      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;

      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseCOM1"].Value.ToString()) == 1)
        {
          ultraGridInformation.Rows[i].Cells["CloseCOM1"].Activation = Activation.ActivateOnly;
          ultraGridInformation.Rows[i].Cells["CloseCOM1"].Appearance.BackColor = Color.Moccasin;
        }
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseCAR"].Value.ToString()) == 1)
        {
          ultraGridInformation.Rows[i].Cells["CloseCAR"].Activation = Activation.ActivateOnly;
          ultraGridInformation.Rows[i].Cells["CloseCAR"].Appearance.BackColor = Color.PowderBlue;
        }
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseFactory"].Value.ToString()) == 1)
        {
          ultraGridInformation.Rows[i].Cells["CloseFactory"].Activation = Activation.ActivateOnly;
          ultraGridInformation.Rows[i].Cells["CloseFactory"].Appearance.BackColor = Color.Thistle;
        }
      }
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtWoFrom.Text = string.Empty;
      txtWoTo.Text = string.Empty;
      ultCBPercentFinished.Value = DBNull.Value;
      ultCBStatus.Value = DBNull.Value;
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
    private bool CheckValid()
    {
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["IsModified"].Value.ToString()) == 1)
        {
          ultraGridInformation.Rows[i].Cells["WorkOrder"].Appearance.BackColor = Color.Empty;
          ultraGridInformation.Rows[i].Cells["ItemCode"].Appearance.BackColor = Color.Empty;
          ultraGridInformation.Rows[i].Cells["Revision"].Appearance.BackColor = Color.Empty;
          long wo = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["WorkOrder"].Value.ToString());
          string item = ultraGridInformation.Rows[i].Cells["ItemCode"].Value.ToString();
          int rev = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Revision"].Value.ToString());
          int closeCom1 = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseCOM1"].Value.ToString());
          int closeCar = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseCAR"].Value.ToString());
          int closeAll = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseFactory"].Value.ToString());
          if (closeCom1 == 1)
          {
            this.typeClose = 1;
          }
          if (closeCar == 1)
          {
            this.typeClose = 2;
          }
          if (closeAll == 1)
          {
            this.typeClose = 3;
          }
          DBParameter[] input = new DBParameter[4];
          input[0] = new DBParameter("@WO", DbType.Int64, wo);
          input[1] = new DBParameter("@ItemCode", DbType.String, item);
          input[2] = new DBParameter("@Revision", DbType.Int32, rev);
          input[3] = new DBParameter("@TypeClose", DbType.Int32, this.typeClose);
          DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNCheckHangTagOnWIPForCloseWO", input);
          if (dt != null && dt.Rows.Count > 0)
          {
            if (DBConvert.ParseInt(dt.Rows[0]["Err"].ToString()) == 1)
            {
              MessageBox.Show(dt.Rows[0]["ErrMsg"].ToString());
              ultraGridInformation.Rows[i].Cells["WorkOrder"].Appearance.BackColor = Color.Yellow;
              ultraGridInformation.Rows[i].Cells["ItemCode"].Appearance.BackColor = Color.Yellow;
              ultraGridInformation.Rows[i].Cells["Revision"].Appearance.BackColor = Color.Yellow;
              return false;
            }
          }
        }
      }
      return true;
    }
    private void SaveData()
    {
      bool success = true;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["IsModified"].Value.ToString()) == 1)
        {
          long pid = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["Pid"].Value);
          DBParameter[] inputParam = new DBParameter[7];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseCOM1"].Value.ToString()) == 1)
          {
            inputParam[1] = new DBParameter("@CloseCOM1", DbType.Int16, DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseCOM1"].Value.ToString()));
          }
          if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseCAR"].Value.ToString()) == 1)
          {
            inputParam[2] = new DBParameter("@CloseCAR", DbType.Int16, DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseCAR"].Value.ToString()));
          }
          if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseFactory"].Value.ToString()) == 1)
          {
            inputParam[3] = new DBParameter("@CloseFactory", DbType.Int16, DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["CloseFactory"].Value.ToString()));
          }
          inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spPLNListCloseWo_Edit", inputParam, outputParam);
          if (outputParam == null || DBConvert.ParseInt(outputParam[0].Value.ToString()) <= 0)
          {
            this.SaveSuccess = false;
            success = false;
          }
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      this.SearchData();
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
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
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      // Set Column Style
      e.Layout.Bands[0].Columns["CloseCOM1"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CloseCOM1"].CellActivation = Activation.AllowEdit;
      //e.Layout.Bands[0].Columns["CloseCOM1"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["CloseCOM1"].Header.Caption = "Close COM1";
      e.Layout.Bands[0].Columns["CloseCOM1By"].Header.Caption = "Close COM1 By";
      e.Layout.Bands[0].Columns["CloseCOM1Date"].Header.Caption = "Close COM1 Date";

      e.Layout.Bands[0].Columns["CloseCAR"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CloseCAR"].CellActivation = Activation.AllowEdit;
      //e.Layout.Bands[0].Columns["CloseCAR"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["CloseCAR"].Header.Caption = "Close CAR";
      e.Layout.Bands[0].Columns["CloseCARBy"].Header.Caption = "Close CAR By";
      e.Layout.Bands[0].Columns["CloseCARDate"].Header.Caption = "Close CAR Date";

      e.Layout.Bands[0].Columns["CloseFactory"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CloseFactory"].CellActivation = Activation.AllowEdit;
      //e.Layout.Bands[0].Columns["CloseFactory"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["CloseFactory"].Header.Caption = "Close Factory";
      e.Layout.Bands[0].Columns["CloseFactoryBy"].Header.Caption = "Close Factory By";
      e.Layout.Bands[0].Columns["CloseFactoryDate"].Header.Caption = "Close Factory Date";


      // Hide column
      e.Layout.Bands[0].Columns["IsModified"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
    }
    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckValid())
      {
        this.SaveData();
      }
    }
    /// <summary>
    /// Export Excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      switch (colName)
      {
        case "CloseCOM1":
          {
            if (DBConvert.ParseInt(e.Cell.Value.ToString()) == 1)
            {
              e.Cell.Row.Cells["IsModified"].Value = 1;
            }
            else
            {
              e.Cell.Row.Cells["IsModified"].Value = 0;
            }
          }
          break;
        case "CloseCAR":
          {
            if (DBConvert.ParseInt(e.Cell.Value.ToString()) == 1)
            {
              e.Cell.Row.Cells["IsModified"].Value = 1;
            }
            else
            {
              e.Cell.Row.Cells["IsModified"].Value = 0;
            }
          }
          break;
        case "CloseFactory":
          {
            if (DBConvert.ParseInt(e.Cell.Value.ToString()) == 1)
            {
              e.Cell.Row.Cells["IsModified"].Value = 1;
            }
            else
            {
              e.Cell.Row.Cells["IsModified"].Value = 0;
            }
          }
          break;
        default:
          break;
      }
    }
    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    /// <summary>
    /// Check All COM1
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkCOM1_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkhcom1)
      {
        int checkAll = (chkCOM1.Checked ? 1 : 0);
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (ultraGridInformation.Rows[i].IsFilteredOut == false && ultraGridInformation.Rows[i].Cells["CloseCOM1"].Activation != Activation.ActivateOnly)
          {
            ultraGridInformation.Rows[i].Cells["CloseCOM1"].Value = checkAll;
          }
        }
      }
    }
    /// <summary>
    /// Check All CAR
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkCAR_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkcar)
      {
        int checkAll = (chkCAR.Checked ? 1 : 0);
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (ultraGridInformation.Rows[i].IsFilteredOut == false && ultraGridInformation.Rows[i].Cells["CloseCAR"].Activation != Activation.ActivateOnly)
          {
            ultraGridInformation.Rows[i].Cells["CloseCAR"].Value = checkAll;
          }
        }
      }
    }
    /// <summary>
    /// Check All FAC
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkFAC_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkfac)
      {
        int checkAll = (chkFAC.Checked ? 1 : 0);
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (ultraGridInformation.Rows[i].IsFilteredOut == false && ultraGridInformation.Rows[i].Cells["CloseFactory"].Activation != Activation.ActivateOnly)
          {
            ultraGridInformation.Rows[i].Cells["CloseFactory"].Value = checkAll;
          }
        }
      }
    }
    /// <summary>
    /// Cell Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInformation_CellChange(object sender, CellEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare("CloseCOM1", column, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          this.chkhcom1 = false;
          chkCOM1.Checked = false;
          this.chkhcom1 = true;
        }
      }
      if (string.Compare("CloseCAR", column, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          this.chkcar = false;
          chkCAR.Checked = false;
          this.chkcar = true;
        }
      }
      if (string.Compare("CloseFactory", column, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          this.chkfac = false;
          chkFAC.Checked = false;
          this.chkfac = true;
        }
      }
    }
    #endregion event

  }
}
