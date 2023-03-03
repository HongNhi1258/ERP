/*
  Author      : Ha Anh
  Description : add user into group
  Date        : 05-10-2011
*/

using DaiCo.Application;
using DaiCo.ERPProject;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MainBOM.AUTHENTICATE
{
  public partial class frmAddUserIntoGroup : Form
  {
    #region Variable
    private int currentTab = 0;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool isChangeOtherInfo = false;
    public long group = long.MinValue;
    private int flag = 0;
    private int status = 0;
    #endregion Variable

    #region Load Data
    /// <summary>
    /// init form
    /// </summary>
    public frmAddUserIntoGroup()
    {
      InitializeComponent();
    }

    /// <summary>
    /// load form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAddUserIntoGroup_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    /// <summary>
    /// load data
    /// </summary>
    private void LoadData()
    {
      switch (this.currentTab)
      {
        case 0:
          this.loadTabGroupAddUser();
          break;
        case 1:
          this.LoadTabUserAddGroup();
          break;
      }
    }

    /// <summary>
    /// load tab user add group
    /// </summary>
    private void LoadTabUserAddGroup()
    {
      Utility.LoadUltraComboDepartment(ultraCBDepartment);
      Utility.LoadUltraComboEmployee(ultraCBEmployee, string.Empty);
    }

    /// <summary>
    /// load tab group add user
    /// </summary>
    private void loadTabGroupAddUser()
    {
      this.LoadGroup();
      if (group != long.MinValue)
      {
        ultcbGroup.Value = group;
      }
      this.LoadDDUser();
      this.LoadDDEmpCode();
      this.searchUserFollowGroup();
    }

    /// <summary>
    /// load empcode
    /// </summary>
    private void LoadDDEmpCode()
    {
      string commandText = " SELECT U.Pid, U.EmployeePid, U.UserName, EMP.EmpName " +
                           " FROM TblBOMUser U INNER JOIN VHRMEmployee EMP ON EMP.Pid = U.EmployeePid ORDER BY U.EmployeePid";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDEmpCode.DataSource = dt;
      ultDDEmpCode.DisplayMember = "EmployeePid";
      ultDDEmpCode.ValueMember = "EmployeePid";
      ultDDEmpCode.DisplayLayout.Bands[0].Columns["UserName"].Width = 100;
      ultDDEmpCode.DisplayLayout.Bands[0].Columns["EmpName"].Width = 300;
      ultDDEmpCode.DisplayLayout.Bands[0].Columns["EmployeePid"].Width = 50;
      ultDDEmpCode.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultDDEmpCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// load down drop user
    /// </summary>
    private void LoadDDUser()
    {
      string commandText = " SELECT U.Pid, U.EmployeePid, U.UserName, EMP.EmpName " +
                           " FROM TblBOMUser U INNER JOIN VHRMEmployee EMP ON EMP.Pid = U.EmployeePid ORDER BY U.EmployeePid";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDUser.DataSource = dt;
      ultDDUser.DisplayMember = "UserName";
      ultDDUser.ValueMember = "UserName";
      ultDDUser.DisplayLayout.Bands[0].Columns["UserName"].Width = 100;
      ultDDUser.DisplayLayout.Bands[0].Columns["EmpName"].Width = 300;
      ultDDUser.DisplayLayout.Bands[0].Columns["EmployeePid"].Width = 50;
      ultDDUser.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultDDUser.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// load group
    /// </summary>
    private void LoadGroup()
    {
      string commandText = "SELECT Pid, NameEN, Description FROM TblGNRAccessGroup ORDER BY NameEN";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultcbGroup.DataSource = dt;
      ultcbGroup.DisplayLayout.AutoFitColumns = true;
      ultcbGroup.DisplayMember = "NameEN";
      ultcbGroup.ValueMember = "Pid";
      ultcbGroup.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultcbGroup.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }
    #endregion Load Data

    #region Event
    /// <summary>
    /// button search click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.searchUserFollowGroup();
      this.isChangeOtherInfo = false;
    }

    /// <summary>
    /// search user follow group
    /// </summary>
    private void searchUserFollowGroup()
    {
      long group = long.MinValue;
      if (ultcbGroup.Text.Trim().Length > 0 && ultcbGroup.Value != null)
      {
        group = DBConvert.ParseLong(ultcbGroup.Value.ToString());
      }

      string commandText = string.Empty;
      if (group > 0)
      {
        commandText = " SELECT U.Pid, U.EmployeePid, U.UserName, EMP.EmpName, EMP.Department, U.Pid OldPid " +
                        " FROM TblBOMUser U " +
                        " INNER JOIN VHRMEmployee EMP ON U.EmployeePid = EMP.Pid " +
                        " INNER JOIN TblGNRAccessGroupUser GU ON U.Pid = GU.UserPid AND GU.GroupPid =" + group + " ORDER BY U.EmployeePid";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        ultGridUser.DataSource = dt;
      }
    }

    /// <summary>
    /// button save click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool result = this.save();
      if (result)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      isChangeOtherInfo = false;
      this.searchUserFollowGroup();
    }

    /// <summary>
    /// function save of tab group add user
    /// </summary>
    /// <returns></returns>
    private bool save()
    {
      //remove relation
      if (this.listDeletedPid != null)
      {
        foreach (long pid in this.listDeletedPid)
        {
          string storename = "spGNRGroupUser_edit";
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@Select", DbType.Int32, 0);
          inputParam[1] = new DBParameter("@Group", DbType.Int64, DBConvert.ParseLong(ultcbGroup.Value.ToString()));
          inputParam[2] = new DBParameter("@User", DbType.Int64, pid);

          DBParameter[] outParam = new DBParameter[1];
          outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outParam);
          if (DBConvert.ParseLong(outParam[0].Value.ToString()) == 0 && DBConvert.ParseLong(outParam[0].Value.ToString()) == long.MinValue)
          {
            return false;
          }
        }
      }

      //insert relation
      DataTable dt = (DataTable)ultGridUser.DataSource;
      if (dt != null)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i].RowState == DataRowState.Modified || dt.Rows[i].RowState == DataRowState.Added)
          {
            long user = DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString());
            if (user != long.MinValue)
            {
              string storename = "spGNRGroupUser_edit";
              DBParameter[] inputParam = new DBParameter[3];
              inputParam[0] = new DBParameter("@Select", DbType.Int32, 1);
              inputParam[1] = new DBParameter("@Group", DbType.Int64, DBConvert.ParseLong(ultcbGroup.Value.ToString()));
              inputParam[2] = new DBParameter("@User", DbType.Int64, user);

              DBParameter[] outParam = new DBParameter[1];
              outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outParam);
              if (DBConvert.ParseLong(outParam[0].Value.ToString()) == 0 && DBConvert.ParseLong(outParam[0].Value.ToString()) == long.MinValue)
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// button close click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// close tab user add group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCloseTab2_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// save tab user add group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveTab2_Click(object sender, EventArgs e)
    {
      bool result = this.saveTabAddGroup();
      if (result)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      isChangeOtherInfo = false;
      this.searchGroupFollowUsername();
    }

    /// <summary>
    /// save tab user add group
    /// </summary>
    /// <returns></returns>
    private bool saveTabAddGroup()
    {
      DataTable dt = (DataTable)ultraGridGroup.DataSource;
      if (dt != null)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i].RowState == DataRowState.Modified)
          {
            string storename = "spGNRUserAddGroup";
            DBParameter[] inputParam = new DBParameter[4];
            if (DBConvert.ParseInt(dt.Rows[i]["Select"].ToString()) == 0)
            {
              inputParam[0] = new DBParameter("@Select", DbType.Int32, 0);
            }
            else
            {
              inputParam[0] = new DBParameter("@Select", DbType.Int32, 1);
            }

            if (txtUserName.Text.Trim().Length == 0)
            {
              string username = string.Empty;
              string query = "SELECT U.UserName FROM VHRMEmployee EMP " +
                              " INNER JOIN TblBOMUser U ON U.EmployeePid = EMP.Pid" +
                              " WHERE CONVERT(varchar,U.EmployeePid) + ' - ' + EmpName = N'" + ultraCBEmployee.Text.Trim() + "'";
              username = DataBaseAccess.ExecuteScalarCommandText(query).ToString();
              inputParam[2] = new DBParameter("@User", DbType.AnsiString, 255, username);
            }
            else
            {
              inputParam[2] = new DBParameter("@User", DbType.AnsiString, 255, txtUserName.Text);
            }
            inputParam[3] = new DBParameter("@GroupPid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()));

            DBParameter[] outParam = new DBParameter[1];
            outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outParam);

            if (DBConvert.ParseLong(outParam[0].Value.ToString()) == 0 && DBConvert.ParseLong(outParam[0].Value.ToString()) == long.MinValue)
            {
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// search user of tab user add group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearchUser_Click(object sender, EventArgs e)
    {
      if (ultraCBEmployee.Text.Trim().Length == 0 && txtUserName.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("WRN0013", "Employee");
        return;
      }
      this.searchGroupFollowUsername();
      this.isChangeOtherInfo = false;
    }

    /// <summary>
    /// function search
    /// </summary>
    private void searchGroupFollowUsername()
    {
      string deptCode = string.Empty;
      int userPid = int.MinValue;
      string userName = string.Empty;
      DBParameter[] inputParam = new DBParameter[3];
      if (ultraCBDepartment.SelectedRow != null)
      {
        deptCode = ultraCBDepartment.SelectedRow.Cells["Department"].Value.ToString();
        inputParam[0] = new DBParameter("@DeptCode", DbType.AnsiString, 50, deptCode);
      }
      if (ultraCBEmployee.SelectedRow != null)
      {
        userPid = DBConvert.ParseInt(ultraCBEmployee.SelectedRow.Cells["Pid"].Value.ToString());
        inputParam[1] = new DBParameter("@EmpPid", DbType.Int32, userPid);
      }
      if (txtUserName.Text.Trim().Length > 0)
      {
        userName = txtUserName.Text.Trim();
        inputParam[2] = new DBParameter("@UserName", DbType.AnsiString, 255, userName);
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spGNRListGroupFollowUser", inputParam);
      ultraGridGroup.DataSource = dtSource;
    }

    /// <summary>
    /// tab selected index change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.currentTab = tabControl.SelectedIndex;
      this.LoadData();
    }

    /// <summary>
    /// tab deselected - save before change tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tabControl_Deselecting(object sender, TabControlCancelEventArgs e)
    {
      if (this.isChangeOtherInfo)
      {
        DialogResult dlg = MessageBox.Show(FunctionUtility.GetMessage("MSG0008"), "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (dlg == DialogResult.Yes)
        {
          //save tab1
          if (tabControl.TabIndex == 0)
          {
            bool result = this.save();
            if (result)
            {
              WindowUtinity.ShowMessageSuccess("MSG0004");
            }
            else
            {
              WindowUtinity.ShowMessageError("ERR0005");
            }
            this.searchUserFollowGroup();
          }
          //save tab2
          if (tabControl.TabIndex == 1)
          {
            bool result = this.saveTabAddGroup();
            if (result)
            {
              WindowUtinity.ShowMessageSuccess("MSG0004");
            }
            else
            {
              WindowUtinity.ShowMessageError("ERR0005");
            }
            this.searchGroupFollowUsername();
          }
        }
        else if (dlg == DialogResult.Cancel)
        {
          e.Cancel = true;
        }
      }
      this.isChangeOtherInfo = false;
    }

    /// <summary>
    /// grid user init layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridUser_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["OldPid"].Hidden = true;
      e.Layout.Bands[0].Columns["UserName"].Header.Caption = "Username";
      e.Layout.Bands[0].Columns["UserName"].ValueList = ultDDUser;
      e.Layout.Bands[0].Columns["EmployeePid"].ValueList = ultDDEmpCode;
      e.Layout.Bands[0].Columns["EmployeePid"].Header.Caption = "Employee Code";
      e.Layout.Bands[0].Columns["EmployeePid"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["EmpName"].Header.Caption = "Employee Name";
      e.Layout.Bands[0].Columns["EmpName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Department"].CellActivation = Activation.ActivateOnly;
    }

    /// <summary>
    /// grid user before cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridUser_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (status == 0)
      {
        string colName = e.Cell.Column.ToString().ToLower();
        int count = 0;
        if (colName == "username")
        {
          string username = e.Cell.Row.Cells["UserName"].Text;
          if (username.Length == 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Username");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
            return;
          }
          for (int i = 0; i < ultDDUser.Rows.Count; i++)
          {
            if (ultDDUser.Rows[i].Cells["UserName"].Text == e.Cell.Row.Cells["UserName"].Text)
            {
              count = 1;
              break;
            }
          }
          if (count == 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Username");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
            return;
          }

          count = 0;
          for (int i = 0; i < ultGridUser.Rows.Count; i++)
          {
            if (e.Cell.Row.Cells["UserName"].Text == ultGridUser.Rows[i].Cells["UserName"].Text)
            {
              count++;
              if (count == 2)
              {
                string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Username");
                WindowUtinity.ShowMessageErrorFromText(message);
                e.Cancel = true;
                break;
              }
            }
          }
        }

        if (colName == "employeepid")
        {
          string code = e.Cell.Row.Cells["EmployeePid"].Text;
          if (code.Length == 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Employee Code");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
            return;
          }

          for (int i = 0; i < ultDDEmpCode.Rows.Count; i++)
          {
            if (ultDDEmpCode.Rows[i].Cells["EmployeePid"].Text == e.Cell.Row.Cells["EmployeePid"].Text)
            {
              count = 1;
              break;
            }
          }
          if (count == 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Employee Code");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
            return;
          }

          count = 0;
          for (int i = 0; i < ultGridUser.Rows.Count; i++)
          {
            if (e.Cell.Row.Cells["EmployeePid"].Text == ultGridUser.Rows[i].Cells["EmployeePid"].Text)
            {
              count++;
              if (count == 2)
              {
                string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Employee Code");
                WindowUtinity.ShowMessageErrorFromText(message);
                e.Cancel = true;
                break;
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// grid user after cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridUser_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      if (flag == 1)
      {
        return;
      }
      if (colName == "username")
      {
        flag = 1;
        status = 1;
        string username = e.Cell.Row.Cells["UserName"].Value.ToString();
        string commandText = " SELECT U.Pid, U.UserName, U.EmployeePid, EMP.EmpName, EMP.Department " +
                              " FROM TblBOMUser U INNER JOIN VHRMEmployee EMP ON EMP.Pid = U.EmployeePid WHERE U.UserName ='" + username + "'";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        e.Cell.Row.Cells["Pid"].Value = dt.Rows[0]["Pid"].ToString();
        e.Cell.Row.Cells["EmpName"].Value = dt.Rows[0]["EmpName"].ToString();
        e.Cell.Row.Cells["EmployeePid"].Value = dt.Rows[0]["EmployeePid"];
        e.Cell.Row.Cells["Department"].Value = dt.Rows[0]["Department"].ToString();
        e.Cell.Row.Appearance.BackColor = Color.White;
        if (DBConvert.ParseLong(e.Cell.Row.Cells["OldPid"].Value.ToString()) != long.MinValue && e.Cell.Row.Cells["Pid"].Value != e.Cell.Row.Cells["OldPid"].Value && !listDeletedPid.Contains(e.Cell.Row.Cells["OldPid"].Value))
        {
          listDeletedPid.Add(e.Cell.Row.Cells["OldPid"].Value);
        }
        this.isChangeOtherInfo = (btnSave.Visible);
        status = 0;
      }
      if (colName == "employeepid")
      {
        flag = 1;
        status = 1;
        int code = DBConvert.ParseInt(e.Cell.Row.Cells["EmployeePid"].Value.ToString());
        string commandText = " SELECT U.Pid, U.UserName, U.EmployeePid, EMP.EmpName, EMP.Department " +
                              " FROM TblBOMUser U INNER JOIN VHRMEmployee EMP ON EMP.Pid = U.EmployeePid WHERE U.EmployeePid =" + code;
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        e.Cell.Row.Cells["Pid"].Value = dt.Rows[0]["Pid"].ToString();
        e.Cell.Row.Cells["EmpName"].Value = dt.Rows[0]["EmpName"].ToString();
        e.Cell.Row.Cells["UserName"].Value = dt.Rows[0]["UserName"].ToString();
        e.Cell.Row.Cells["Department"].Value = dt.Rows[0]["Department"].ToString();
        e.Cell.Row.Appearance.BackColor = Color.White;
        if (DBConvert.ParseLong(e.Cell.Row.Cells["OldPid"].Value.ToString()) != long.MinValue && e.Cell.Row.Cells["Pid"].Value != e.Cell.Row.Cells["OldPid"].Value && !listDeletedPid.Contains(e.Cell.Row.Cells["OldPid"].Value))
        {
          listDeletedPid.Add(e.Cell.Row.Cells["OldPid"].Value);
        }
        this.isChangeOtherInfo = (btnSave.Visible);
        status = 0;
      }
      flag = 0;
    }

    /// <summary>
    /// after row deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridUser_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
      this.isChangeOtherInfo = (btnSave.Visible);
    }

    /// <summary>
    /// before row deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultGridUser_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    /// <summary>
    /// init combobox department
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultcbDepartment_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Department"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Department"].MaxWidth = 70;
    }

    /// <summary>
    /// init combobox employee
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultcbEmployee_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].ColHeadersVisible = false;
      e.Layout.Bands[0].Columns["Pid"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Pid"].MaxWidth = 70;
    }

    /// <summary>
    /// init grid group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridGroup_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Description"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
    }

    /// <summary>
    /// Grid group after cell update - bat flag save tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridGroup_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.isChangeOtherInfo = true;
    }

    /// <summary>
    /// value change of ultcombo department
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraCBDepartment_ValueChanged(object sender, EventArgs e)
    {
      string dept = string.Empty;
      if (ultraCBDepartment.SelectedRow != null)
      {
        dept = ultraCBDepartment.SelectedRow.Cells["Department"].Value.ToString();
      }
      Utility.LoadUltraComboEmployee(ultraCBEmployee, dept);
    }
    #endregion Event
  }
}