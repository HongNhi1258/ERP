/*
  Author        : 
  Create date   : 02/03/2013
  Decription    : Close WO
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;
namespace DaiCo.Planning
{
  public partial class viewPLN_20_007 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init Data

    public viewPLN_20_007()
    {
      InitializeComponent();
    }

    /// <summary>
    ///  Load From
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_20_007_Load(object sender, EventArgs e)
    {
      this.LoadWorkOrder();
      this.LoadComboStatus();
    }

    /// <summary>
    /// Load WO
    /// </summary>
    private void LoadWorkOrder()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder Order By Pid DESC";
      DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtWO == null)
      {
        return;
      }
      ultCBWO.DataSource = dtWO;
      ultCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBWO.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load Carcass
    /// </summary>
    /// <param name="wo"></param>
    private void LoadCarcass(long wo)
    {
      string commandText = string.Format(@"SELECT Distinct CarcassCode
                           FROM VPLNWorkOrderCarcassList  info
                           WHERE info.WoPid = {0}", wo);
      DataTable dt = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }
      ultCBCarcassCode.DataSource = dt;
      ultCBCarcassCode.ValueMember = "CarcassCode";
      ultCBCarcassCode.DisplayMember = "CarcassCode";
      ultCBCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBCarcassCode.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load UltraCombo Status
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'Not Yet Close' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Close' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBStatus.DataSource = dtSource;
      ultCBStatus.DisplayMember = "Name";
      ultCBStatus.ValueMember = "ID";
      ultCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBStatus.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
      ultCBStatus.DisplayLayout.AutoFitColumns = true;
    }

    #endregion Init Data

    #region Function 

    /// <summary>
    /// Enter Search
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.LoadData();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }


    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] input = new DBParameter[3];
      if (ultCBWO.Value != null && ultCBWO.Text.Length > 0)
      {
        input[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultCBWO.Value.ToString()));
      }
      if (ultCBCarcassCode.Value != null && ultCBCarcassCode.Text.Length > 0)
      {
        input[1] = new DBParameter("@Carcass", DbType.String, ultCBCarcassCode.Value.ToString());
      }
      if (ultCBStatus.Value != null && DBConvert.ParseInt(ultCBStatus.Value.ToString()) != int.MinValue)
      {
        input[2] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
      }
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNWoodsGroupStatus", input);
      if (dt == null)
      {
        return;
      }
      ultData.DataSource = dt;

      // Set Status Control
      this.SetStatusControl();
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (string.Compare(row.Cells["Status"].Value.ToString(), "Close", true) == 0)
        {
          row.Cells["Close"].Activation = Activation.ActivateOnly;
        }
      }
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseLong(row.Cells["WOClose"].Value.ToString()) == long.MinValue
            && row.Cells["CarcassClose"].Value.ToString().Length == 0
            && DBConvert.ParseInt(row.Cells["Close"].Value.ToString()) == 1)
        {
          // Input
          DBParameter[] input = new DBParameter[5];
          input[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row.Cells["WO"].Value.ToString()));
          input[1] = new DBParameter("@Carcass", DbType.String, row.Cells["Carcass"].Value.ToString());
          input[2] = new DBParameter("@MaterialGroup", DbType.AnsiString, 3, row.Cells["MaterialGroup"].Value.ToString());
          input[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          input[4] = new DBParameter("@Type", DbType.Int32, 1);

          // OutPut
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          // Execute Store
          DataBaseAccess.ExecuteStoreProcedure("spPLNWoodsCloseWO_Insert", input, output);

          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    #endregion Function

    #region Event

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      //e.Layout.Bands[0].Columns["WO"].Hidden = true;
      e.Layout.Bands[0].Columns["WOClose"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassClose"].Hidden = true;
      e.Layout.Bands[0].Columns["MaterialGroup"].Header.Caption = "Group";
      e.Layout.Bands[0].Columns["WO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Carcass"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialGroup"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Description"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Close"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Change WO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBWO.Value != null && ultCBWO.Text.Length > 0)
      {
        long wo = DBConvert.ParseLong(ultCBWO.Value.ToString());
        ultCBCarcassCode.Value = DBNull.Value;
        this.LoadCarcass(wo);
      }
      else
      {
        ultCBCarcassCode.Value = DBNull.Value;
        this.LoadCarcass(-1);
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
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Select All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      int check = (chkSelectAll.Checked ? 1 : 0);
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseLong(ultData.Rows[i].Cells["WOClose"].Value.ToString()) == long.MinValue
            && ultData.Rows[i].Cells["CarcassClose"].Value.ToString().Length == 0)
        {
          ultData.Rows[i].Cells["Close"].Value = check;
        }
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

    #endregion Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }
  }
}
