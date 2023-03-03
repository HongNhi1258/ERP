using DaiCo.Shared.Utility;
using System.Windows.Forms;

namespace DaiCo.Shared
{
  public class MainTreeNode : TreeNode
  {
    private ViewState viewState;
    private FormWindowState windowState;
    private string fullPathDomain;
    private long uiPid;
    private string uiParam;

    public MainTreeNode(string text, string name, int imageIndex, int selectedImageIndex, params MainTreeNode[] listNode) : base()
    {
      base.Text = text;
      base.Name = name;
      this.fullPathDomain = string.Empty;
      base.SelectedImageIndex = selectedImageIndex;
      base.ImageIndex = imageIndex;
      this.viewState = ViewState.MainWindow;
      this.windowState = FormWindowState.Normal;
      this.uiPid = long.MinValue;
      foreach (MainTreeNode n in listNode)
      {
        base.Nodes.Add(n);
      }
    }
    public MainTreeNode(string text, string name, int imageIndex, int selectedImageIndex, ViewState viewState, params MainTreeNode[] listNode)
      : base()
    {
      base.Text = text;
      base.Name = name;
      this.fullPathDomain = string.Empty;
      base.SelectedImageIndex = selectedImageIndex;
      base.ImageIndex = imageIndex;
      this.viewState = viewState;
      this.windowState = FormWindowState.Normal;
      this.uiPid = long.MinValue;
      foreach (MainTreeNode n in listNode)
      {
        base.Nodes.Add(n);
      }
    }
    public MainTreeNode(string text, string name, int imageIndex, int selectedImageIndex, ViewState viewState, FormWindowState windowState)
      : base()
    {
      base.Text = text;
      base.Name = name;
      this.fullPathDomain = string.Empty;
      base.SelectedImageIndex = selectedImageIndex;
      base.ImageIndex = imageIndex;
      this.viewState = viewState;
      this.windowState = windowState;
      this.uiPid = long.MinValue;
    }

    public MainTreeNode(string text, string name, string fullPathDomain, int imageIndex, int selectedImageIndex)
      : base()
    {
      base.Text = text;
      base.Name = name;
      this.fullPathDomain = fullPathDomain;
      base.SelectedImageIndex = selectedImageIndex;
      base.ImageIndex = imageIndex;
      this.viewState = ViewState.MainWindow;
      this.windowState = FormWindowState.Normal;
      this.uiPid = long.MinValue;
    }

    public ViewState ViewState
    {
      get { return this.viewState; }
      set { this.viewState = value; }
    }
    public string FullPathDomain
    {
      get { return this.fullPathDomain; }
      set { this.fullPathDomain = value; }
    }

    public FormWindowState WindowState
    {
      get { return this.windowState; }
      set { this.windowState = value; }
    }
    public long UIPid
    {
      get { return this.uiPid; }
      set { this.uiPid = value; }
    }
    public string UIParam
    {
      get { return this.uiParam; }
      set { this.uiParam = value; }
    }
  }
}
