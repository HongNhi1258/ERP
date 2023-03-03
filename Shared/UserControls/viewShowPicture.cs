/*
  Author  : Nguyễn Văn Trọn  
  Date    : 11/04/2013
*/
using System;

namespace DaiCo.Shared
{
  public partial class viewShowPicture : MainUserControl
  {
    #region fields
    public string path = string.Empty;
    public string compCode = string.Empty;
    #endregion fields

    #region Innit
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewShowPicture()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewShowPicture_Load(object sender, EventArgs e)
    {
      picComponent.ImageLocation = this.path;
      groupBoxCompImg.Text = this.compCode;
    }

    #endregion Innit

    #region Events
    /// <summary>
    /// Close form when double click on image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void picComponent_DoubleClick(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Events
  }
}
