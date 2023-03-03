using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DaiCo.Shared
{
  public delegate bool PreRemoveTab(string key);
  public partial class MainTabControl : TabControl
  {
    public MainTabControl() : base()
    {
      PreRemoveTabPage = null;
      this.DrawMode = TabDrawMode.OwnerDrawFixed;
    }

    public PreRemoveTab PreRemoveTabPage;

    protected override void OnMouseLeave(EventArgs e)
    {
      Graphics g = CreateGraphics();
      g.SmoothingMode = SmoothingMode.AntiAlias;
      RectangleF tabTextArea = RectangleF.Empty;
      for (int nIndex = 0; nIndex < this.TabCount; nIndex++)
      {
        if (nIndex != this.SelectedIndex)
        {
          tabTextArea = (RectangleF)this.GetTabRect(nIndex);
          GraphicsPath _Path = new GraphicsPath();
          _Path.AddRectangle(tabTextArea);
          using (LinearGradientBrush _Brush = new LinearGradientBrush(tabTextArea, SystemColors.Control, SystemColors.ControlLight, LinearGradientMode.Vertical))
          {
            ColorBlend _ColorBlend = new ColorBlend(3);

            _ColorBlend.Colors = new Color[]{  SystemColors.ActiveBorder,
                                                        SystemColors.ActiveBorder,SystemColors.ActiveBorder,
                                                        SystemColors.ActiveBorder};

            _ColorBlend.Positions = new float[] { 0f, .4f, 0.5f, 1f };
            _Brush.InterpolationColors = _ColorBlend;
            g.FillRectangle(_Brush, tabTextArea.X + tabTextArea.Width - 22, 4, tabTextArea.Height - 2, tabTextArea.Height - 5);
            g.DrawRectangle(Pens.White, tabTextArea.X + tabTextArea.Width - 20, 6, tabTextArea.Height - 8, tabTextArea.Height - 9);
            using (Pen pen = new Pen(Color.White, 2))
            {
              g.DrawLine(pen, tabTextArea.X + tabTextArea.Width - 19, 6, tabTextArea.X + tabTextArea.Width - 10, 14);
              g.DrawLine(pen, tabTextArea.X + tabTextArea.Width - 19, 14, tabTextArea.X + tabTextArea.Width - 10, 6);
            }
          }
          _Path.Dispose();

        }
        else
        {

          tabTextArea = (RectangleF)this.GetTabRect(nIndex);
          GraphicsPath _Path = new GraphicsPath();
          _Path.AddRectangle(tabTextArea);
          using (LinearGradientBrush _Brush = new LinearGradientBrush(tabTextArea, SystemColors.Control, SystemColors.ControlLight, LinearGradientMode.Vertical))
          {
            ColorBlend _ColorBlend = new ColorBlend(3);
            _ColorBlend.Positions = new float[] { 0f, .4f, 0.5f, 1f };

            _ColorBlend.Colors = new Color[]{Color.FromArgb(255,231,164,152),
                                                      Color.FromArgb(255,231,164,152),Color.FromArgb(255,197,98,79),
                                                      Color.FromArgb(255,197,98,79)};
            _Brush.InterpolationColors = _ColorBlend;
            g.FillRectangle(_Brush, tabTextArea.X + tabTextArea.Width - 22, 4, tabTextArea.Height - 3, tabTextArea.Height - 5);
            g.DrawRectangle(Pens.White, tabTextArea.X + tabTextArea.Width - 20, 6, tabTextArea.Height - 8, tabTextArea.Height - 9);
            using (Pen pen = new Pen(Color.White, 2))
            {
              g.DrawLine(pen, tabTextArea.X + tabTextArea.Width - 19, 6, tabTextArea.X + tabTextArea.Width - 10, 14);
              g.DrawLine(pen, tabTextArea.X + tabTextArea.Width - 19, 14, tabTextArea.X + tabTextArea.Width - 10, 6);
            }
          }
          _Path.Dispose();
        }

      }

      g.Dispose();


    }

    protected override void OnMouseMove(MouseEventArgs e)
    {

      if (!DesignMode)
      {
        Graphics g = CreateGraphics();
        g.SmoothingMode = SmoothingMode.AntiAlias;
        for (int nIndex = 0; nIndex < this.TabCount; nIndex++)
        {
          RectangleF tabTextArea = (RectangleF)this.GetTabRect(nIndex);
          tabTextArea = new RectangleF(tabTextArea.X + tabTextArea.Width - 22, 4, tabTextArea.Height - 3, tabTextArea.Height - 5);

          Point pt = new Point(e.X, e.Y);
          if (tabTextArea.Contains(pt))
          {
            using (
                LinearGradientBrush _Brush =
                    new LinearGradientBrush(tabTextArea, SystemColors.Control, SystemColors.ControlLight,
                                            LinearGradientMode.Vertical))
            {
              ColorBlend _ColorBlend = new ColorBlend(3);
              _ColorBlend.Colors = new Color[]
                              {
                                  Color.FromArgb(255, 252, 193, 183),
                                  Color.FromArgb(255, 252, 193, 183), Color.FromArgb(255, 210, 35, 2),
                                  Color.FromArgb(255, 210, 35, 2)
                              };
              _ColorBlend.Positions = new float[] { 0f, .4f, 0.5f, 1f };
              _Brush.InterpolationColors = _ColorBlend;

              g.FillRectangle(_Brush, tabTextArea);
              g.DrawRectangle(Pens.White, tabTextArea.X + 2, 6, tabTextArea.Height - 3,
                              tabTextArea.Height - 4);
              using (Pen pen = new Pen(Color.White, 2))
              {
                g.DrawLine(pen, tabTextArea.X + 3, 6, tabTextArea.X + 12, 14);
                g.DrawLine(pen, tabTextArea.X + 3, 14, tabTextArea.X + 12, 6);
              }
            }
          }
          else
          {
            if (nIndex != SelectedIndex)
            {
              using (
                  LinearGradientBrush _Brush =
                      new LinearGradientBrush(tabTextArea, SystemColors.Control, SystemColors.ControlLight,
                                              LinearGradientMode.Vertical))
              {
                ColorBlend _ColorBlend = new ColorBlend(3);
                _ColorBlend.Colors = new Color[]
                                  {
                                      SystemColors.ActiveBorder,
                                      SystemColors.ActiveBorder, SystemColors.ActiveBorder,
                                      SystemColors.ActiveBorder
                                  };
                _ColorBlend.Positions = new float[] { 0f, .4f, 0.5f, 1f };
                _Brush.InterpolationColors = _ColorBlend;

                g.FillRectangle(_Brush, tabTextArea);
                g.DrawRectangle(Pens.White, tabTextArea.X + 2, 6, tabTextArea.Height - 3,
                                tabTextArea.Height - 4);
                using (Pen pen = new Pen(Color.White, 2))
                {
                  g.DrawLine(pen, tabTextArea.X + 3, 6, tabTextArea.X + 12, 14);
                  g.DrawLine(pen, tabTextArea.X + 3, 14, tabTextArea.X + 12, 6);
                }
              }
            }
          }
        }
        g.Dispose();
      }
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {

      if (e.Bounds != RectangleF.Empty)
      {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        RectangleF tabTextArea = RectangleF.Empty;

        for (int nIndex = 0; nIndex < this.TabCount; nIndex++)
        {

          tabTextArea = (RectangleF)this.GetTabRect(nIndex);
          GraphicsPath _Path = new GraphicsPath();
          _Path.AddRectangle(tabTextArea);

          if (nIndex != this.SelectedIndex)
          {
            using (LinearGradientBrush _Brush = new LinearGradientBrush(tabTextArea, SystemColors.Control, SystemColors.ControlLight, LinearGradientMode.Vertical))
            {
              ColorBlend _ColorBlend = new ColorBlend(3);
              _ColorBlend.Colors = new Color[]{SystemColors.ControlLightLight,
                                                      Color.FromArgb(255,SystemColors.ControlLight),SystemColors.ControlDark,
                                                      SystemColors.ControlLightLight};

              _ColorBlend.Positions = new float[] { 0f, .4f, 0.5f, 1f };
              _Brush.InterpolationColors = _ColorBlend;

              e.Graphics.FillPath(_Brush, _Path);
              using (Pen pen = new Pen(SystemColors.ActiveBorder))
              {
                e.Graphics.DrawPath(pen, _Path);
              }


              _ColorBlend.Colors = new Color[]{  SystemColors.ActiveBorder,
                                                        SystemColors.ActiveBorder,SystemColors.ActiveBorder,
                                                        SystemColors.ActiveBorder};

              _ColorBlend.Positions = new float[] { 0f, .4f, 0.5f, 1f };
              _Brush.InterpolationColors = _ColorBlend;
              e.Graphics.FillRectangle(_Brush, tabTextArea.X + tabTextArea.Width - 22, 4, tabTextArea.Height - 3, tabTextArea.Height - 5);
              e.Graphics.DrawRectangle(Pens.White, tabTextArea.X + tabTextArea.Width - 20, 6, tabTextArea.Height - 8, tabTextArea.Height - 9);
              using (Pen pen = new Pen(Color.White, 2))
              {
                e.Graphics.DrawLine(pen, tabTextArea.X + tabTextArea.Width - 19, 6, tabTextArea.X + tabTextArea.Width - 10, 14);
                e.Graphics.DrawLine(pen, tabTextArea.X + tabTextArea.Width - 19, 14, tabTextArea.X + tabTextArea.Width - 10, 6);
              }
            }
            _Path.Dispose();
          }
          else
          {
            using (LinearGradientBrush _Brush = new LinearGradientBrush(tabTextArea, SystemColors.Control, SystemColors.ControlLight, LinearGradientMode.Vertical))
            {
              ColorBlend _ColorBlend = new ColorBlend(3);
              _ColorBlend.Colors = new Color[]{SystemColors.ControlLightLight,
                                                      Color.FromArgb(255,SystemColors.Control),SystemColors.ControlLight,
                                                      SystemColors.Control};
              _ColorBlend.Positions = new float[] { 0f, .4f, 0.5f, 1f };
              _Brush.InterpolationColors = _ColorBlend;
              e.Graphics.FillPath(_Brush, _Path);
              using (Pen pen = new Pen(SystemColors.ActiveBorder))
              {
                e.Graphics.DrawPath(pen, _Path);
              }
              //Drawing Close Button
              _ColorBlend.Colors = new Color[]{Color.FromArgb(255,231,164,152),
                                                      Color.FromArgb(255,231,164,152),Color.FromArgb(255,197,98,79),
                                                      Color.FromArgb(255,197,98,79)};
              _Brush.InterpolationColors = _ColorBlend;
              e.Graphics.FillRectangle(_Brush, tabTextArea.X + tabTextArea.Width - 22, 4, tabTextArea.Height - 3, tabTextArea.Height - 5);
              e.Graphics.DrawRectangle(Pens.White, tabTextArea.X + tabTextArea.Width - 20, 6, tabTextArea.Height - 8, tabTextArea.Height - 9);
              using (Pen pen = new Pen(Color.White, 2))
              {
                e.Graphics.DrawLine(pen, tabTextArea.X + tabTextArea.Width - 19, 6, tabTextArea.X + tabTextArea.Width - 10, 14);
                e.Graphics.DrawLine(pen, tabTextArea.X + tabTextArea.Width - 19, 14, tabTextArea.X + tabTextArea.Width - 10, 6);
              }
            }
            _Path.Dispose();
          }
          string str = this.TabPages[nIndex].Text;
          StringFormat stringFormat = new StringFormat();
          stringFormat.Alignment = StringAlignment.Near;
          RectangleF rT = new RectangleF(tabTextArea.X, tabTextArea.Y, tabTextArea.Width, tabTextArea.Height);
          rT.Offset(0, 3);
          e.Graphics.DrawString(str, this.Font, new SolidBrush(this.TabPages[nIndex].ForeColor), rT, stringFormat);
        }
      }

    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
      Point p = e.Location;
      if (e.Button == MouseButtons.Left)
      {
        RectangleF tabTextArea = (RectangleF)this.GetTabRect(SelectedIndex);
        tabTextArea = new RectangleF(tabTextArea.X + tabTextArea.Width - 22, 4, tabTextArea.Height - 3, tabTextArea.Height - 5);
        Point pt = new Point(e.X, e.Y);

        if (tabTextArea.Contains(pt))
        {
          TabPage pageSelect = this.TabPages[SelectedIndex];
          MainUserControl con = null;
          foreach (UserControl c in pageSelect.Controls)
          {
            con = (MainUserControl)c;
            if (c != null)
            {
              break;
            }
          }
          if (con != null)
            con.ConfirmToCloseTab();
        }
      }
      else if (e.Button == MouseButtons.Right)
      {
        for (int i = 0; i < TabCount; i++)
        {
          Rectangle r = GetTabRect(i);
          if (r.Contains(p))
          {
            SelectedIndex = i;
            break;
          }
        }
        ContextMenu contMenu = new ContextMenu();
        MenuItem mnuClose = new MenuItem("Close");
        mnuClose.Click += new EventHandler(mnuClose_Click);
        MenuItem mnuCloseAll = new MenuItem("Close All But This");
        mnuCloseAll.Click += new EventHandler(mnuCloseAll_Click);
        contMenu.MenuItems.Add(mnuClose);
        contMenu.MenuItems.Add(mnuCloseAll);
        contMenu.Show(this, p);
      }
    }

    void mnuCloseAll_Click(object sender, EventArgs e)
    {
      TabPage selectTab = SelectedTab;
      foreach (TabPage page in TabPages)
      {
        if (page.Name != selectTab.Name)
        {
          this.SelectedTab = TabPages[page.Name];
          MainUserControl con = GetMainControl(page);
          if (con != null)
            con.ConfirmToCloseTab();
        }
      }
    }

    void mnuClose_Click(object sender, EventArgs e)
    {
      MainUserControl con = GetMainControl(SelectedTab);
      if (con != null)
        con.ConfirmToCloseTab();
    }

    private MainUserControl GetMainControl(TabPage page)
    {
      MainUserControl con = null;
      foreach (UserControl c in page.Controls)
      {
        con = (MainUserControl)c;
        if (c != null)
        {
          break;
        }
      }
      return con;
    }

    public void CloseTab(string key)
    {
      if (PreRemoveTabPage != null)
      {
        bool closeIt = PreRemoveTabPage(key);
        if (!closeIt)
          return;
      }

      //Close by index of
      int index = TabPages.IndexOf(this.SelectedTab);
      TabPages.RemoveAt(index);

      if (index > 0)
        this.SelectedTab = TabPages[index - 1];
      if (TabPages.Count == 0)
        this.Visible = false;
    }
  }
}
