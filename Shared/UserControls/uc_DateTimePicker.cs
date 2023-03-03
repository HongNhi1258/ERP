using System;
using System.Windows.Forms;

namespace DaiCo.Shared.UserControls
{
  public delegate void ValueChangedEventHandler(object sender, EventArgs e);
  public partial class uc_DateTimePicker : UserControl
  {
    public event ValueChangedEventHandler ValueChanged;
    public uc_DateTimePicker()
    {
      InitializeComponent();
      //this.ultraDateTimeEditor1.GotFocus += new EventHandler(ultraDateTimeEditor1_GotFocus);
    }

    //void ultraDateTimeEditor1_GotFocus(object sender, EventArgs e)
    //{
    //  this.ultraDateTimeEditor1.DropDown();
    //}
    public DateTime Value
    {
      get
      {
        object obj = this.ultraDateTimeEditor1.Value;
        if (obj != null)
          return (DateTime)obj;
        else
          return DateTime.MinValue;
      }
      set
      {
        if (value == DateTime.MinValue)
          this.ultraDateTimeEditor1.Value = null;
        else
        {
          try
          {
            this.ultraDateTimeEditor1.Value = value;
          }
          catch
          {
            this.ultraDateTimeEditor1.Value = null;
          }
        }
      }
    }

    private void ultraDateTimeEditor1_ValueChanged(object sender, EventArgs e)
    {
      if (ValueChanged != null)
        ValueChanged(sender, e);
    }
  }
}
