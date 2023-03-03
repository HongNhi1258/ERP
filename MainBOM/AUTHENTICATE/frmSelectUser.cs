using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;
using System.Windows.Forms;

namespace MainBOM.AUTHENTICATE
{
  public partial class frmSelectUser : Form
  {
    public long toUserPid;
    public frmSelectUser()
    {
      InitializeComponent();
    }

    private void frmSelectUser_Load(object sender, EventArgs e)
    {
      LoadDepartment();
    }

    /// <summary>
    /// Load Data To ComboBox Department
    /// </summary>
    private void LoadDepartment()
    {
      string commandText = "SELECT Department, DeparmentName FROM VHRDDepartment WHERE IsNew = 1 ORDER BY Department";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dtSource.NewRow();
      dtSource.Rows.InsertAt(row, 0);
      cmbDepartment.DataSource = dtSource;
    }

    private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cmbDepartment.SelectedIndex > 0)
      {
        string dept = cmbDepartment.SelectedValue.ToString().Trim();
        string commandText = string.Format(@" SELECT emp.Pid Pid, CAST(emp.Pid as varchar) + ' - ' + emp.EmpName AS EmpName 
                                              FROM VHRMEmployee emp
	                                              INNER JOIN TblBOMUser us ON (us.EmployeePid = emp.Pid)
                                              WHERE emp.Department = '{0}'
                                              ORDER BY emp.Pid", dept);
        DataTable dtSource = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
        DataRow row = dtSource.NewRow();
        dtSource.Rows.InsertAt(row, 0);
        cmbEmployee.DataSource = dtSource;
      }
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      if (cmbDepartment.SelectedIndex > 0 && cmbEmployee.SelectedIndex > 0)
      {
        try
        {
          long fromUserPid = DBConvert.ParseLong(cmbEmployee.SelectedValue.ToString());
          DBParameter[] param = new DBParameter[2];
          param[0] = new DBParameter("@fromUserPid", DbType.Int64, fromUserPid);
          param[1] = new DBParameter("@toUserPid", DbType.Int64, this.toUserPid);

          DataBaseAccess.ExecuteStoreProcedure("spAuthenticateSetSameRight", param);
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        catch
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        }
      }
    }
  }
}