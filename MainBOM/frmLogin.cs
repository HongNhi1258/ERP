using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using FormSerialisation;
using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MainBOM
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string strCmdUser = "Select Top 1 EmployeePid From TblBOMUser Where UserName = @User AND PasswordI = @Pass";
            DBParameter[] arrParam = new DBParameter[2];
            arrParam[0] = new DBParameter("@User", DbType.AnsiString, 50, txtUser.Text.Trim());
            arrParam[1] = new DBParameter("@Pass", DbType.AnsiString, 200, DaiCo.Shared.Utility.FunctionUtility.EncodePassword(txtPass.Text.Trim()));
            DataTable dtAccount = DataBaseAccess.SearchCommandTextDataTable(strCmdUser, arrParam);

            if (dtAccount.Rows.Count > 0)
            {
                int userID = DBConvert.ParseInt(dtAccount.Rows[0]["EmployeePid"].ToString());
                string commandText = string.Empty;
                commandText += " SELECT EmpName, Department ";
                commandText += " FROM ";
                commandText += " ( ";
                commandText += " 	SELECT Pid, EmpName, Department FROM VHRMEmployee WHERE Resigned = 0";
                commandText += " 	UNION ALL ";
                commandText += " 	SELECT 999999 + Pid, EnglishName, 'SUP' FROM TblPURSupplierInfo ";
                commandText += " ) NV ";
                commandText += " WHERE NV.Pid = " + userID;
                DataTable dtUser = DataBaseAccess.SearchCommandTextDataTable(commandText);

                if (dtUser == null || dtUser.Rows.Count == 0)
                {
                    MessageBox.Show("Login failed!");
                    return;
                }
                DaiCo.Shared.Utility.SharedObject.UserInfo.UserPid = userID;
                DaiCo.Shared.Utility.SharedObject.UserInfo.EmpName = dtUser.Rows[0]["EmpName"].ToString();
                DaiCo.Shared.Utility.SharedObject.UserInfo.UserName = txtUser.Text.Trim();
                DaiCo.Shared.Utility.SharedObject.UserInfo.Department = dtUser.Rows[0]["Department"].ToString();
                DaiCo.Shared.Utility.SharedObject.UserInfo.LoginDate = DBConvert.ParseString(DateTime.Today, DaiCo.Shared.Utility.ConstantClass.FORMAT_DATETIME);
                DaiCo.Shared.Utility.SharedObject.UserInfo.MachineName = System.Environment.MachineName;
                string IP = "";
                try
                {
                    DBParameter[] arrParam1 = new DBParameter[1];
                    IP = DataBaseAccess.ExecuteScalarCommandText("SELECT VALUE FROM TblBOMCodeMaster WHERE [GROUP] = 16017 AND CODE = 1", arrParam1).ToString();
                }
                catch
                {
                    IP = "10.0.8.161";
                }
                //frmMain frm = new frmMain();
                //frm.UserName = SharedObject.UserInfo.EmpName.Split(' ')[SharedObject.UserInfo.EmpName.Split(' ').Length - 1] + "-" + SharedObject.UserInfo.Department + "-" + SharedObject.UserInfo.UserPid.ToString();
                //frm.IP = IP;
                //frm.Show();
                //frm.WindowState = FormWindowState.Minimized;

                this.Close();
            }
            else
            {
                DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "User name or Password");
            }

        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void frmLogin_Load(object sender, EventArgs e)
        {
            //MessageBox.Show(this.DecryptNew("OdTSVCq6w+j/E02OngDZ0k7unXQnKUD8", true));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLogin_FormClosing);
            try
            {
                FormSerialisor.Deserialise(this, ConstantClass.PATHCOOKIE + @"\serialiseLogin.xml");
                if (txtUser.Text.Trim().Length > 0)
                {
                    txtPass.Focus();
                }
                else
                {
                    txtUser.Focus();
                }
            }
            catch
            { }
        }
        public string DecryptNew(string cipherString, bool useHashing)
        {
            byte[] buffer;
            byte[] inputBuffer = Convert.FromBase64String(cipherString);
            string s = "DFHLASDLFDFSDHFSDFSLFDHFHLHKKJSJDSKLAFJKLASDJFKLSDFJKLASDJFKLDSK233434LAS998998899FDSAKFJS";
            if (useHashing)
            {
                MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
                buffer = provider.ComputeHash(Encoding.UTF8.GetBytes(s));
                provider.Clear();
            }
            else
            {
                buffer = Encoding.UTF8.GetBytes(s);
            }
            TripleDESCryptoServiceProvider provider2 = new TripleDESCryptoServiceProvider
            {
                Key = buffer,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] bytes = provider2.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            provider2.Clear();
            return Encoding.UTF8.GetString(bytes);
        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //txtPass.Text = "";
                FormSerialisor.Serialise(this, ConstantClass.PATHCOOKIE + @"\serialiseLogin.xml");
            }
            catch
            { }
        }
    }
}