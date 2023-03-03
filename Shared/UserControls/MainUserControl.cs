using DaiCo.Shared.Utility;
using System.Drawing;
using System.Windows.Forms;


namespace DaiCo.Shared
{
  public partial class MainUserControl : UserControl
  {
    private bool needToSave = false;
    private string tabName = string.Empty;
    private string fullPath = string.Empty;
    private bool saveSuccess = true;
    private string viewParam;
    public ContextMenuStrip popupMenu = new ContextMenuStrip();
    public bool NeedToSave
    {
      get { return this.needToSave; }
      set { this.needToSave = value; }
    }

    public bool SaveSuccess
    {
      get { return this.saveSuccess; }
      set { this.saveSuccess = value; }
    }

    public string TabName
    {
      get { return this.tabName; }
      set { this.tabName = value; }
    }
    public string FullPath
    {
      get { return this.fullPath; }
      set { this.fullPath = value; }
    }
    public string ViewParam
    {
      get { return this.viewParam; }
      set { this.viewParam = value; }
    }

    public MainUserControl()
    {
      InitializeComponent();
      this.viewParam = string.Empty;
      popupMenu.Items.Add("Copy");
      popupMenu.Items[0].MouseDown += new MouseEventHandler(this.PopupMenu_MouseClick);
    }

    public void CloseTab()
    {
      try
      {
        Control parentCon = this.Parent;
        if (parentCon.GetType() == typeof(Form))
        {
          Form frm = (Form)parentCon;
          if (frm != null)
            frm.Close();
        }
        else if (parentCon.GetType() == typeof(TabPage))
        {
          TabPage page = (TabPage)parentCon;
          if (page != null)
          {
            MainTabControl con = (MainTabControl)page.Parent;
            con.CloseTab(this.TabName);
            this.Parent.Dispose();
            MemoryManagement.FlushMemory();
          }
        }
      }
      catch
      {
      }
    }
    public bool ConfirmToCloseForm()
    {
      bool result = true;
      Control parentCon = this.Parent;
      try
      {
        if (parentCon.GetType() == typeof(Form))
        {
          Form frm = (Form)parentCon;
          if (frm != null)
          {
            if (NeedToSave)
            {
              string messageConfirm = FunctionUtility.GetMessage("MSG0008");
              DialogResult dlgr = MessageBox.Show(messageConfirm, "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
              if (dlgr == DialogResult.Yes)
              {
                SaveAndClose();
                if (saveSuccess)
                {
                  this.Parent.Dispose();
                }
                else
                {
                  result = false;
                }
              }
              else if (dlgr == DialogResult.No)
              {
                this.Parent.Dispose();
              }
              else if (dlgr == DialogResult.Cancel)
              {
                result = false;
              }
            }
            else
            {
              this.Parent.Dispose();
              result = true;
            }
          }
        }
      }
      catch
      {
      }
      return result;
    }

    public bool ConfirmToCloseTab()
    {
      bool result = true;
      Control parentCon = this.Parent;
      try
      {
        if (parentCon.GetType() == typeof(Form))
        {
          Form frm = (Form)parentCon;
          if (frm != null)
          {
            frm.Close();
            if (saveSuccess)
            {
              try
              {
                this.Parent.Dispose();
              }
              catch { }
            }
          }
        }
        else if (parentCon.GetType() == typeof(TabPage))
        {
          MainTabControl con = null;
          TabPage page = (TabPage)parentCon;
          if (page != null)
          {
            con = (MainTabControl)page.Parent;
          }
          if (con != null)
          {
            if (NeedToSave)
            {
              string messageConfirm = FunctionUtility.GetMessage("MSG0008");
              DialogResult dlgr = MessageBox.Show(messageConfirm, "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
              if (dlgr == DialogResult.Yes)
              {
                SaveAndClose();
                if (saveSuccess)
                {
                  con.CloseTab(this.tabName);
                  this.Parent.Dispose();
                }
              }
              else if (dlgr == DialogResult.No)
              {
                con.CloseTab(this.tabName);
                this.Parent.Dispose();
              }
              else if (dlgr == DialogResult.Cancel)
                result = false;
            }
            else
            {
              con.CloseTab(this.tabName);
              this.Parent.Dispose();
              MemoryManagement.FlushMemory();
            }
          }
        }
      }
      catch
      {
      }

      return result;
    }

    public void DrawLable()
    {
      Label lbScreenNo = new Label();
      lbScreenNo.Name = "lbScreenNo";
      lbScreenNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      string screenName = this.Name;
      screenName = screenName.StartsWith("view") ? screenName.Remove(0, 4) : screenName;
      screenName = screenName.Substring(0, 10);
      lbScreenNo.Text = "SN: " + screenName;
      lbScreenNo.Width = 100;
      lbScreenNo.Location = new Point(5, this.Height - 20);
      this.Controls.Add(lbScreenNo);
      this.Controls.SetChildIndex(lbScreenNo, 0);
    }

    public virtual void SaveAndClose()
    {
    }
    public virtual void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
    }
  }
}
