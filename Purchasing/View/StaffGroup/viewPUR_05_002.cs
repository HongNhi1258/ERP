/*
 * Author       : Vo Van Duy Qui   
 * CreateDate   : 24-03-2011
 * Description  : Staff Group Information
 */
using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_05_002 : DaiCo.Shared.MainUserControl
  {
    #region Variable
    public long groupPid = long.MinValue;
    public long leaderPid = long.MinValue;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool loadData = false;

    private string SP_PUR_STAFFGROUPINFORMATION = "spPURStaffGroupInformation";
    private string SP_PUR_STAFFGROUP_EDIT = "spPURStaffGroup_Edit";
    private string SP_PUR_STAFFGROUPDETAIL_EDIT = "spPURStaffGroupDetail_Edit";
    private string SP_PUR_STAFFGROUPDETAIL_DELETE = "spPURStaffGroupDetail_Delete";
    #endregion Variable

    #region Init Data

    /// <summary>
    /// Init Form
    /// </summary>
    public viewPUR_05_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_05_002_Load(object sender, EventArgs e)
    {
      this.LoadComboBoxLeader();
      this.LoadDropDownEmp(this.leaderPid);

      this.LoadData();
      this.loadData = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] input = new DBParameter[] { new DBParameter("@GroupPid", DbType.Int64, this.groupPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(SP_PUR_STAFFGROUPINFORMATION, input);

      if (dsSource != null)
      {
        DataTable dtMain = dsSource.Tables[0];
        if (dtMain.Rows.Count > 0)
        {
          txtGroupName.Text = dtMain.Rows[0]["GroupName"].ToString().Trim();
          txtGroupName.Enabled = false;
          ultComboLeader.Value = dtMain.Rows[0]["LeaderGroup"];
        }

        DataTable dtDetail = dsSource.Tables[1];
        dtDetail.PrimaryKey = new DataColumn[] { dtDetail.Columns["Employee"] };
        ultData.DataSource = dtDetail;
      }
    }

    /// <summary>
    /// Load Ultra ComboBox Leader
    /// </summary>
    private void LoadComboBoxLeader()
    {
      string commandText = "SELECT Pid , CAST(Pid AS VARCHAR) + ' - ' + EmpName Name FROM VHRMEmployee WHERE IsNew = 1";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultComboLeader.DataSource = dtSource;
      ultComboLeader.ValueMember = "Pid";
      ultComboLeader.DisplayMember = "Name";

      ultComboLeader.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboLeader.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultComboLeader.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
    }

    /// <summary>
    /// Load Ultra DropDownList Employee
    /// </summary>
    /// <param name="leaderPid"></param>
    private void LoadDropDownEmp(long leaderPid)
    {
      string commandText = string.Format("SELECT Pid, CAST(Pid AS VARCHAR) + ' - ' + EmpName Name FROM VHRMEmployee WHERE IsNew = 1 AND Pid <> {0} ORDER BY Pid", leaderPid);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpAlternative.DataSource = dtSource;
      udrpAlternative.ValueMember = "Pid";
      udrpAlternative.DisplayMember = "Name";

      udrpAlternative.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      udrpAlternative.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpAlternative.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
    }

    /// <summary>
    /// Get Employee Name With Employee Pid
    /// </summary>
    /// <param name="empPid"></param>
    /// <returns></returns>
    private string GetEmployeeName(int empPid)
    {
      string name = string.Empty;
      string commandText = string.Format("SELECT EmpName FROM VHRMEmployee WHERE Pid = {0}", empPid);
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      if (obj != null)
      {
        name = (string)obj;
      }
      return name;
    }
    #endregion Init Data

    #region Save

    /// <summary>
    /// Check Valid Input Data
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      // Group Staff
      string name = txtGroupName.Text.Trim();
      if (name.Length == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("WRN0013", "Group Name");
        txtGroupName.Focus();
        this.NeedToSave = false;
        this.SaveSuccess = false;
        return false;
      }

      if (this.groupPid == long.MinValue)
      {
        string commandText = string.Format("SELECT COUNT(*) FROM TblPURStaffGroup WHERE DeleteFlg = 0 AND GroupName = '{0}'", name);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (obj != null && (int)obj > 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0028", "Group Name");
          this.NeedToSave = false;
          this.SaveSuccess = false;
          txtGroupName.Focus();
          return false;
        }
      }

      int lPid = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultComboLeader));
      if (lPid == int.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0011", "Leader Group");
        this.NeedToSave = false;
        this.SaveSuccess = false;
        ultComboLeader.Focus();
        return false;
      }

      DataTable dtSource = (DataTable)ultData.DataSource;
      foreach (DataRow row in dtSource.Rows)
      {
        int empPid = DBConvert.ParseInt(row["Employee"].ToString());
        if (lPid == empPid)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0075");
          this.NeedToSave = false;
          this.SaveSuccess = false;
          ultComboLeader.Focus();
          return false;
        }

        DataRow[] rowsDup = dtSource.Select(string.Format("Employee = {0}", empPid));
        if (rowsDup.Length > 1)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0013", "input data");
          this.SaveSuccess = false;
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Save Main
    /// </summary>
    /// <returns></returns>
    private long SaveMain()
    {
      string groupName = txtGroupName.Text.Trim();
      int lPid = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultComboLeader));

      DBParameter[] input = new DBParameter[4];
      if (this.groupPid != long.MinValue)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, this.groupPid);
      }
      input[1] = new DBParameter("@GroupName", DbType.String, 128, groupName);
      input[2] = new DBParameter("@LeaderGroup", DbType.Int32, lPid);
      input[3] = new DBParameter("@DeleteFlg", DbType.Int64, 0);

      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure(SP_PUR_STAFFGROUP_EDIT, input, output);
      long success = DBConvert.ParseLong(output[0].Value.ToString());
      return success;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <param name="grpPid"></param>
    /// <returns></returns>
    private bool SaveDetail(long grpPid)
    {
      foreach (long deletePid in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, deletePid) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

        DataBaseAccess.ExecuteStoreProcedure(SP_PUR_STAFFGROUPDETAIL_DELETE, inputDelete, outputDelete);
        if (outputDelete[0].Value.ToString().Trim() == "0")
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }

      DataTable dtSource = (DataTable)ultData.DataSource;
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          long detailPid = DBConvert.ParseLong(row["Pid"].ToString());
          int empPid = DBConvert.ParseInt(row["Employee"].ToString());
          DBParameter[] input = new DBParameter[3];
          if (detailPid != long.MinValue)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
          }
          input[1] = new DBParameter("@Group", DbType.Int64, grpPid);
          input[2] = new DBParameter("@Employee", DbType.Int32, empPid);

          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure(SP_PUR_STAFFGROUPDETAIL_EDIT, input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result == 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    #endregion Save

    #region Event

    /// <summary>
    /// Override Function SaveAndClose
    /// </summary>
    public override void SaveAndClose()
    {
      this.btnSave_Click(null, null);
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Employee"].ValueList = udrpAlternative;
      e.Layout.Bands[0].Columns["Employee"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// ultData Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Text.Split('-')[0].Trim();
      int empPid = DBConvert.ParseInt(value);
      if (colName.CompareTo("employee") == 0)
      {
        string commandText = string.Format("SELECT COUNT(*) FROM VHRMEmployee WHERE Pid = {0}", empPid);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if ((obj == null) || (obj != null && (int)obj == 0))
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0011", "Employee");
          e.Cancel = true;
        }
      }
    }

    /// <summary>
    /// ultData After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (this.loadData)
      {
        this.NeedToSave = true;
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
    /// ultComboLeader Value Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboLeader_ValueChanged(object sender, EventArgs e)
    {
      if (ultComboLeader.SelectedRow != null && this.loadData)
      {
        int pid = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultComboLeader));
        DataTable dtSource = (DataTable)ultData.DataSource;
        foreach (DataRow row in dtSource.Rows)
        {
          int empPid = DBConvert.ParseInt(row["Employee"].ToString());
          if (pid == empPid)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0075");
            ultComboLeader.ResetText();
            return;
          }
        }
        this.LoadDropDownEmp(pid);
        this.NeedToSave = true;
      }
    }

    /// <summary>
    /// btnSave Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.CheckValid())
      {
        long grpPid = this.SaveMain();
        this.groupPid = grpPid;
        if (grpPid != long.MinValue)
        {
          bool result = this.SaveDetail(grpPid);
          if (!result)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
            this.SaveSuccess = false;
            return;
          }
        }
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.NeedToSave = false;
        this.SaveSuccess = true;
        if (this.SaveSuccess)
        {
          this.CloseTab();
        }
      }
    }

    /// <summary>
    /// ultData Before Rows Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long detailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (detailPid != long.MinValue)
        {
          listDeletingPid.Add(detailPid);
        }
      }
    }

    /// <summary>
    /// ultData After Rows Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        listDeletedPid.Add(pid);
      }
    }
    #endregion Event
  }
}

