
/*
  Author  : Vo Van Duy Qui
  Email   : qui_it@daico-furniture.com
  Date    : 04-10-2010
  Company : Dai Co 
*/

using DaiCo.Shared.DataBaseUtility;
using System;
using System.Windows.Forms;

namespace MainBOM
{
  public partial class frmChangePassword : Form
  {
    #region Init
    public frmChangePassword()
    {
      InitializeComponent();
    }
    #endregion Init

    #region Check Data

    /// <summary>
    /// Check Current Password
    /// </summary>
    /// <returns></returns>
    private bool CheckCurrentPassword()
    {
      string curPass = DaiCo.Shared.Utility.FunctionUtility.EncodePassword(txtCurrentPass.Text.Trim());
      string passUser = string.Empty;
      string commandText = string.Format("SELECT PasswordI FROM TblBOMUser WHERE EmployeePid = {0}", DaiCo.Shared.Utility.SharedObject.UserInfo.UserPid);
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      if (obj != null)
      {
        passUser = (string)obj;
      }

      int cmp = curPass.CompareTo(passUser);
      if (cmp == 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Check Input Data
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      bool result = this.CheckCurrentPassword();
      if (result)
      {
        string newPass = txtNewPass.Text.Trim();
        string confirmPass = txtConfirmPass.Text.Trim();

        if (newPass.Length == 0)
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0011", "New Password");
          return false;
        }

        if (confirmPass.Length == 0)
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageWarning("MSG0011", "Confirm Password");
          return false;
        }

        if (newPass.CompareTo(confirmPass) != 0)
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", "Confirm Password");
          return false;
        }
        return true;
      }
      else
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", "Password");
        return false;
      }
    }
    #endregion Check Data

    #region Event

    /// <summary>
    /// Event Button Change Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnChange_Click(object sender, EventArgs e)
    {
      bool result = this.CheckValid();
      if (result)
      {
        string newPass = DaiCo.Shared.Utility.FunctionUtility.EncodePassword(txtNewPass.Text.Trim());
        string commandText = string.Format("UPDATE TblBOMUser SET PasswordI = '{0}' WHERE EmployeePid = {1}", newPass, DaiCo.Shared.Utility.SharedObject.UserInfo.UserPid);
        bool changed = DataBaseAccess.ExecuteCommandText(commandText);
        if (changed)
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0014");
          this.Close();
        }
        else
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0044");
        }
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
    #endregion Event
  }
}