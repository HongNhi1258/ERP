/*
  Author  : Vo Van Duy Qui
  Email   : qui_it@daico-furniture.com
  Date    : 04-10-2010
  Company : Dai Co 
*/

using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;
using System.Windows.Forms;

namespace MainBOM.AUTHENTICATE
{
  public partial class frmAddUser : Form
  {
    #region Field
    public string department = string.Empty;
    public int userPid = int.MinValue;
    #endregion Field

    #region Init
    public frmAddUser()
    {
      InitializeComponent();
    }

    private void LoadComboDepartment()
    {
      string commandText = string.Empty;
      commandText += " SELECT Department, DEP.DeparmentName, DEP.Display ";
      commandText += " FROM ";
      commandText += " ( ";
      commandText += "   SELECT Department, DeparmentName, Department + ' | ' + DeparmentName Display  ";
      commandText += "   FROM VHRDDepartment   ";
      commandText += "	 UNION ALL   ";
      commandText += "	 SELECT 'SUP', 'Supplier', 'SUP | Supplier'   ";
      commandText += " ) DEP   ";
      commandText += " ORDER BY Department  ";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBDepartment.DataSource = dt;
      ultraCBDepartment.DisplayMember = "Display";
      ultraCBDepartment.ValueMember = "Department";
      ultraCBDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBDepartment.DisplayLayout.Bands[0].Columns["Display"].Width = 300;
      ultraCBDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
      ultraCBDepartment.DisplayLayout.Bands[0].Columns["DeparmentName"].Hidden = true;
    }

    /// <summary>
    /// Load Data To UltraCombo Employee
    /// </summary>
    private void LoadComboEmployee(string department)
    {
      string commandText = string.Empty;
      commandText += " SELECT EM.Pid, EM.EmpName, EM.Display ";
      commandText += " FROM ";
      commandText += " ( ";
      commandText += " 	 SELECT Pid, EmpName, Cast(Pid AS VARCHAR) + ' - ' + EmpName Display, Department FROM VHRMEmployee  ";
      commandText += "	 UNION    ";
      commandText += "	 SELECT SUP.Pid, SUP.SupplierCode, Cast(Pid AS VARCHAR) + ' - ' + SupplierCode Display, 'SUP'   ";
      commandText += "	 FROM   ";
      commandText += "   (   ";
      commandText += "    	SELECT 999999 + Pid Pid, SupplierCode  ";
      commandText += "   	  FROM TblPURSupplierInfo  ";
      commandText += "   ) SUP  ";
      commandText += " ) EM ";
      if (department.Length > 0)
      {
        commandText += " WHERE Department ='" + department + "'";
      }
      commandText += " ORDER BY EM.Pid ";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBEmployee.DataSource = dt;
      ultraCBEmployee.DisplayMember = "Display";
      ultraCBEmployee.ValueMember = "Pid";
      ultraCBEmployee.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBEmployee.DisplayLayout.Bands[0].Columns["Display"].Width = 300;
      ultraCBEmployee.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultraCBEmployee.DisplayLayout.Bands[0].Columns["EmpName"].Hidden = true;
    }

    /// <summary>
    /// Load data for Department, Employee, UserName
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAddUser_Load(object sender, EventArgs e)
    {
      //DaiCo.Shared.Utility.ControlUtility.LoadUltraComboDepartment(ultraCBDepartment);
      //DaiCo.Shared.Utility.ControlUtility.LoadUltraComboEmployee(ultraCBEmployee, string.Empty);
      // Load Combo Department
      this.LoadComboDepartment();
      this.LoadComboEmployee(string.Empty);

      // Set User And Department
      if (this.userPid > 0)
      {
        try
        {
          // Set department
          string commandText = string.Format("Select Department From VHRMEmployee Where Pid = {0}", this.userPid);
          DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtSource.Rows.Count > 0)
          {
            ultraCBDepartment.Value = dtSource.Rows[0]["Department"].ToString();
          }
          ultraCBDepartment.Enabled = false;
          // Set employee
          ultraCBEmployee.Value = this.userPid;
          ultraCBEmployee.Enabled = false;
        }
        catch { }
      }
      else if (this.department.Length > 0) // Set Department
      {
        try
        {
          ultraCBDepartment.Value = this.department;
        }
        catch { }
      }
    }
    #endregion Init

    #region LoadData

    #endregion LoadData

    #region Event
    /// <summary>
    /// Event Button Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool result = this.CheckValid();
      if (result)
      {
        this.SaveData();
      }
    }

    /// <summary>
    /// Event Button Cancel Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void ultraCBDepartment_ValueChanged(object sender, EventArgs e)
    {
      string dept = string.Empty;
      if (ultraCBDepartment.SelectedRow != null)
      {
        dept = ultraCBDepartment.SelectedRow.Cells["Department"].Value.ToString();
      }
      //DaiCo.Shared.Utility.ControlUtility.LoadUltraComboEmployee(ultraCBEmployee, dept);
      this.LoadComboEmployee(dept);
    }
    #endregion Event

    #region CheckValid And Save

    /// <summary>
    /// Check Input Data
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      if (ultraCBDepartment.SelectedRow == null)
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("MSG0011", "Department");
        return false;
      }

      if (ultraCBEmployee.SelectedRow == null)
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("MSG0011", "Employee");
        return false;
      }

      if (txtUser.Text.Length == 0)
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0005", "User Name");
        return false;
      }

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@UserName", DbType.AnsiString, 255, txtUser.Text.Trim()) };
      string commandText = "Select UserName From TblBOMUser Where UserName = @UserName";
      DataTable dtTest = DataBaseAccess.SearchCommandTextDataTable(commandText, inputParam);
      if (dtTest != null && dtTest.Rows.Count > 0)
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0006", "User Name");
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save User Info
    /// </summary>
    private void SaveData()
    {
      bool success = true;
      string userName = txtUser.Text.Trim();
      string pass = "123456";
      int employeePid = 0;
      BOMUser user = new BOMUser();
      if (this.userPid > 0) // Update
      {
        user.EmployeePid = this.userPid;
        user = (BOMUser)DataBaseAccess.LoadObject(user, new string[] { "EmployeePid" });
        user.UserName = userName;
        success = DataBaseAccess.UpdateObject(user, new string[] { "EmployeePid" });
      }
      else // Insert
      {
        employeePid = DBConvert.ParseInt(ultraCBEmployee.SelectedRow.Cells["Pid"].Value.ToString());
        user.EmployeePid = employeePid;
        user.UserName = userName;
        user.PasswordI = DaiCo.Shared.Utility.FunctionUtility.EncodePassword(pass);
        user.CreateDate = DateTime.Today;
        employeePid = (int)DataBaseAccess.InsertObject(user);
        success = (employeePid > 0 ? true : false);
      }
      if (!success)
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        return;
      }

      //insert user into group Public
      string storename = string.Empty;
      if (this.ultraCBDepartment.Value.ToString() != "SUP")
      {
        storename = "spGNRGroupPublic_Insert";
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@UserName", DbType.AnsiString, 255, userName) };
        DataBaseAccess.ExecuteStoreProcedure(storename, inputParam);
      }


      DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");

      this.Close();
    }
    #endregion CheckValid And Save

    private void ultraCBEmployee_ValueChanged(object sender, EventArgs e)
    {
      // Set Username
      if (this.userPid > 0)
      {
        string commandText = string.Format("Select UserName From TblBOMUser Where EmployeePid = {0}", this.userPid);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtSource.Rows.Count > 0)
        {
          txtUser.Text = dtSource.Rows[0]["UserName"].ToString();
        }
      }
      else if (ultraCBEmployee.SelectedRow != null)
      {
        string userName = string.Empty;
        long empPid = DBConvert.ParseLong(ultraCBEmployee.SelectedRow.Cells["Pid"].Value.ToString());
        string commandText = string.Format("SELECT UserName FROM TblBOMUser WHERE EmployeePid = {0}", empPid);
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (obj != null)
        {
          txtUser.Text = (string)obj;
          btnSave.Enabled = false;
        }
        else
        {
          txtUser.Text = string.Empty;
          btnSave.Enabled = true;
        }
      }
      else
      {
        txtUser.Text = string.Empty;
      }
    }
  }
}