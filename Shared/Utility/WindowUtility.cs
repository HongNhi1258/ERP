using System.Windows.Forms;

namespace DaiCo.Shared.Utility
{
  public static class WindowUtinity
  {
    public static void CloseView(UserControl con1)
    {
      Control con = con1.Parent;
      if (con.GetType() == typeof(Form))
      {
        Form frm = (Form)con;
        frm.Close();
      }
      else if (con.GetType() == typeof(TabPage))
      {
        TabPage page = (TabPage)con;
        TabControl tabContent = (TabControl)page.Parent;
        if (tabContent != null && page != null)
        {
          tabContent.TabPages.Remove(page);
          if (tabContent.TabPages.Count == 0)
            tabContent.Visible = false;
        }
      }
    }

    private static int CheckUIPermit(string uiCode)
    {
      if (SharedObject.UserInfo.UserPid == ConstantClass.UserAddmin)
      {
        return 1;
      }
      DaiCo.Application.DBParameter[] inputParam = new DaiCo.Application.DBParameter[2];
      inputParam[0] = new DaiCo.Application.DBParameter("@UICode", System.Data.DbType.String, 128, uiCode);
      inputParam[1] = new DaiCo.Application.DBParameter("@UserPid", System.Data.DbType.Int64, SharedObject.UserInfo.UserPid);
      string commandText = "Select dbo.FGNRCheckUIPermit(@UICode, @UserPid)";
      return (int)DaiCo.Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandText, inputParam);
    }

    public static void ShowView(MainUserControl uc, string header, bool isNewForm, ViewState state)
    {
      //MemoryManagement.FlushMemory();
      if (uc != null && CheckUIPermit(uc.Name) > 0)
      {
        uc.DrawLable();
        ControlUtility.UserAccessRight(SharedObject.UserInfo.UserPid, uc);
        if (state == ViewState.MainWindow)
        {
          bool isExist = false;
          TabControl tabContent = DaiCo.Shared.Utility.SharedObject.tabContent;
          if (isNewForm)
          {
            TabPage tab = new TabPage(header + "      ");
            tab.AutoScroll = true;
            tab.Name = uc.Name;
            uc.TabName = uc.Name;
            uc.Dock = DockStyle.Fill;
            tab.Controls.Add(uc);
            tabContent.TabPages.Add(tab);
            tabContent.SelectedTab = tab;
          }
          else
          {
            foreach (TabPage page in tabContent.TabPages)
            {
              if (page.Name == uc.Name)
              {
                MainUserControl oldUC = (MainUserControl)page.Controls[0];
                page.Controls.Clear();
                oldUC.Dispose();
                uc.Dock = DockStyle.Fill;
                uc.TabName = uc.Name;
                page.Controls.Add(uc);
                page.Text = header + "      ";
                tabContent.SelectedTab = page;
                isExist = true;
                break;
              }
            }
            if (!isExist)
            {
              TabPage tab = new TabPage(header + "      ");
              tab.AutoScroll = true;
              tab.Name = uc.Name;
              uc.TabName = uc.Name;
              uc.Dock = DockStyle.Fill;
              tab.Controls.Add(uc);
              tabContent.TabPages.Add(tab);
              tabContent.SelectedTab = tab;
            }
          }
        }
        else
        {
          Form frm = new Form();
          frm.FormClosing += new FormClosingEventHandler(frm_FormClosing);
          string frmName = uc.Name + uc.ViewParam;
          //foreach (Form OpenForm in System.Windows.Forms.Application.OpenForms)
          //{
          //  if (OpenForm.Name == frmName)
          //  {
          //    frm = OpenForm;
          //    break;
          //  }
          //}

          frm.WindowState = FormWindowState.Normal;
          frm.Name = frmName;
          frm.StartPosition = FormStartPosition.Manual;
          //frm.FormBorderStyle = FormBorderStyle.FixedDialog;
          //frm.MinimizeBox = false;
          //frm.MaximizeBox = false;
          frm.Width = uc.Width + 10;
          frm.Height = uc.Height + 35;
          frm.Text = header;
          frm.StartPosition = FormStartPosition.CenterScreen;
          uc.Dock = DockStyle.Fill;
          frm.Controls.Clear();
          frm.Controls.Add(uc);
          if (state == ViewState.ModalWindow)
            frm.ShowDialog();
          else
            frm.Show();
        }
      }
    }

    public static void ShowView(MainUserControl uc, string header, bool isNewForm, ViewState state, System.Windows.Forms.FormWindowState windowState)
    {
      //MemoryManagement.FlushMemory();
      if (uc == null)
      {
        WindowUtinity.ShowMessageWarningFromText("This view is underconstruct!");
        return;
      }
      if (CheckUIPermit(uc.Name) > 0)
      {
        uc.DrawLable();
        ControlUtility.UserAccessRight(SharedObject.UserInfo.UserPid, uc);
        if (state == ViewState.MainWindow)
        {
          bool isExist = false;
          TabControl tabContent = DaiCo.Shared.Utility.SharedObject.tabContent;
          if (isNewForm)
          {
            TabPage tab = new TabPage(header + "      ");
            tab.AutoScroll = true;
            tab.Name = uc.Name + uc.ViewParam;
            uc.TabName = tab.Name;
            uc.Dock = DockStyle.Fill;
            tab.Controls.Add(uc);
            tabContent.TabPages.Add(tab);
            tabContent.SelectedTab = tab;
          }
          else
          {
            foreach (TabPage page in tabContent.TabPages)
            {
              if (page.Name == uc.Name + uc.ViewParam)
              {
                MainUserControl oldUC = (MainUserControl)page.Controls[0];
                page.Controls.Clear();
                oldUC.Dispose();
                uc.Dock = DockStyle.Fill;
                uc.TabName = page.Name;
                page.Controls.Add(uc);
                tabContent.SelectedTab = page;
                isExist = true;
                break;
              }
            }
            if (!isExist)
            {
              TabPage tab = new TabPage(header + "      ");
              tab.AutoScroll = true;
              tab.Name = uc.Name + uc.ViewParam;
              uc.TabName = tab.Name;
              uc.Dock = DockStyle.Fill;
              tab.Controls.Add(uc);
              tabContent.TabPages.Add(tab);
              tabContent.SelectedTab = tab;
            }
          }
        }
        else
        {
          Form frm = new Form();
          frm.FormClosing += new FormClosingEventHandler(frm_FormClosing);
          string frmName = uc.Name + uc.ViewParam;
          if (!isNewForm)
          {
            foreach (Form OpenForm in System.Windows.Forms.Application.OpenForms)
            {
              if (OpenForm.Name == frmName)
              {
                //frm = OpenForm;
                OpenForm.Close();
                break;
              }
            }
          }
          frm.WindowState = windowState;
          frm.Name = frmName;
          frm.StartPosition = FormStartPosition.CenterScreen;
          //frm.FormBorderStyle = FormBorderStyle.FixedDialog;
          //frm.MinimizeBox = false;
          //frm.MaximizeBox = false;
          frm.Width = uc.Width + 10;
          frm.Height = uc.Height + 35;
          frm.Text = header;
          uc.Dock = DockStyle.Fill;

          frm.Controls.Clear();

          MemoryManagement.FlushMemory();

          frm.Controls.Add(uc);
          if (state == ViewState.ModalWindow)
            frm.ShowDialog();
          else
            frm.Show();
        }
      }
    }

    static void frm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Form frm = (Form)sender;
      MainUserControl muc = null;
      foreach (Control uc in frm.Controls)
      {
        try
        {
          muc = (MainUserControl)uc;
        }
        catch { }
        if (muc != null)
        {
          break;
        }
      }
      try
      {
        e.Cancel = !muc.ConfirmToCloseForm();
      }
      catch { }
    }

    public static void ShowMessageError(string messageNo, params string[] param)
    {
      string message = FunctionUtility.GetMessage(messageNo);
      if (param.Length > 0)
        message = string.Format(message, param);
      MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void ShowMessageErrorFromText(string message)
    {
      MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void ShowMessageSuccess(string messageNo, params string[] param)
    {
      string message = FunctionUtility.GetMessage(messageNo);
      if (param.Length > 0)
        message = string.Format(message, param);
      MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    public static void ShowMessageSuccessFromText(string message)
    {
      MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static DialogResult ShowMessageConfirm(string messageNo, params string[] param)
    {
      string message = FunctionUtility.GetMessage(messageNo);
      if (param.Length > 0)
        message = string.Format(message, param);
      return MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    }

    public static DialogResult ShowMessageConfirmFromText(string message)
    {
      return MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    }

    public static void ShowMessageWarning(string messageNo, params string[] param)
    {
      string message = FunctionUtility.GetMessage(messageNo);
      if (param.Length > 0)
        message = string.Format(message, param);
      MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public static void ShowMessageWarningFromText(string message)
    {
      MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public static void ShowMessageInfomation(string messageNo, params string[] param)
    {
      string message = FunctionUtility.GetMessage(messageNo);
      if (param.Length > 0)
        message = string.Format(message, param);
      MessageBox.Show(message, "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
  }
}
