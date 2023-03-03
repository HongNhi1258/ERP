/*
  Author      : Vo Van Duy Qui
  Date        : 24-03-2011
  Description : Staff Group
*/
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_05_001 : DaiCo.Shared.MainUserControl
  {
    #region Field
    private string SP_PUR_LISTSTAFFGROUP = "spPURListStaffGroup";
    private string SP_PUR_STAFFGROUPUPDATE = "spPURStaffGroupDeleteFlg_Update";
    #endregion Field

    #region Init

    /// <summary>
    /// Init Form
    /// </summary>
    public viewPUR_05_001()
    {
      InitializeComponent();
    }

    #endregion Init

    #region Event

    private void Search()
    {
      string staffGroup = txtGroupName.Text.Trim();
      DBParameter[] input = new DBParameter[] { new DBParameter("@GroupName", DbType.String, 130, "%" + staffGroup + "%") };

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(SP_PUR_LISTSTAFFGROUP, input);
      ultStaffGroupDetail.DataSource = dtSource;
    }
    /// <summary>
    /// btnNew Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPUR_05_002 view = new viewPUR_05_002();
      Shared.Utility.WindowUtinity.ShowView(view, "New Staff Group", false, Shared.Utility.ViewState.ModalWindow);
      this.Search();
    }

    /// <summary>
    /// btnSearch Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }
    /// <summary>
    /// Ultra Grid Mouse Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultStaffGroupDetail_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultStaffGroupDetail.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultStaffGroupDetail.Selected.Rows[0];
      long groupPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      long leaderPid = DBConvert.ParseLong(row.Cells["LeaderPid"].Value.ToString());

      viewPUR_05_002 view = new viewPUR_05_002();
      view.groupPid = groupPid;
      view.leaderPid = leaderPid;
      Shared.Utility.WindowUtinity.ShowView(view, "New Staff Group", false, Shared.Utility.ViewState.ModalWindow);
      this.Search();
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultStaffGroupDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LeaderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[0].Columns["LeaderGroup"].Header.Caption = "Leader Group";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      for (int i = 0; i < ultStaffGroupDetail.DisplayLayout.Bands[0].Columns.Count - 1; i++)
      {
        ultStaffGroupDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
    }

    /// <summary>
    /// Override Function SaveAndClose
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      btnDelete_Click(null, null);
    }

    /// <summary>
    /// btnDelete Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ultStaffGroupDetail.DataSource;
      DataRow[] rowsSelected = dtSource.Select("Selected = 1");
      if (rowsSelected.Length > 0)
      {
        foreach (DataRow row in rowsSelected)
        {
          long groupPid = DBConvert.ParseLong(row["Pid"].ToString());
          string commandTextCheck = string.Format("SELECT COUNT(*) FROM TblGNRMaterialInformation WHERE GroupIncharge = {0}", groupPid);
          object obj = DataBaseAccess.ExecuteScalarCommandText(commandTextCheck);
          if (obj != null && (int)obj > 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0054", "Group Staff");
            return;
          }

          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@GroupPid", DbType.Int64, groupPid);
          input[1] = new DBParameter("@DeleteFlg", DbType.Int32, 1);

          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.ExecuteStoreProcedure(SP_PUR_STAFFGROUPUPDATE, input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
            return;
          }
        }
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0002");
        this.NeedToSave = false;
        this.btnSearch_Click(sender, e);
      }
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// ultStaffGroupDetail After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultStaffGroupDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim().ToLower();
      if (colName.CompareTo("selected") == 0)
      {
        this.NeedToSave = true;
      }
    }
    #endregion Event
  }
}
