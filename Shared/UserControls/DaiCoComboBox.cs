using System.ComponentModel;
using System.Windows.Forms;

namespace DaiCo.Shared
{
  public partial class DaiCoComboBox : ComboBox
  {
    public DaiCoComboBox()
    {
      InitializeComponent();
    }

    public DaiCoComboBox(IContainer container)
    {
      container.Add(this);

      InitializeComponent();
    }
    //protected override void OnKeyPress(KeyPressEventArgs e)
    //{
    //  base.OnKeyPress(e);
    //  bool blnLimitToList = true;
    //  string strFindStr = string.Empty;

    //  //Neu phim Backspace duoc nhan
    //  if (e.KeyChar == (char)8)
    //  {
    //    if (this.SelectionStart <= 1)
    //    {
    //      this.Text = string.Empty;
    //      return;
    //    }

    //    if (this.SelectionLength == 0)
    //    {
    //      strFindStr = this.Text.Substring(0, this.Text.Length - 1);
    //    }
    //    else
    //    {
    //      strFindStr = this.Text.Substring(0, this.SelectionStart - 1);
    //    }
    //  }
    //  else
    //  {
    //    if (this.SelectionLength == 0)
    //    {
    //      strFindStr = this.Text + e.KeyChar;
    //    }
    //    else
    //    {
    //      strFindStr = this.Text.Substring(0, this.SelectionStart) + e.KeyChar;
    //    }
    //  }
    //  int intIdx = -1;
    //  intIdx = this.FindString(strFindStr);
    //  if (intIdx != -1)
    //  {
    //    this.SelectedText = string.Empty;
    //    this.SelectedIndex = intIdx;
    //    this.SelectionStart = strFindStr.Length;
    //    this.SelectionLength = this.Text.Length;
    //    e.Handled = true;
    //  }
    //  else
    //  {
    //    e.Handled = blnLimitToList;
    //  }
    //  //Neu phim nhan la Enter thi bo drop down di
    //  if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Tab)
    //  {
    //    this.DroppedDown = false;
    //  }
    //  else
    //  {
    //    this.DroppedDown = true;
    //  }
    //}
  }
}
