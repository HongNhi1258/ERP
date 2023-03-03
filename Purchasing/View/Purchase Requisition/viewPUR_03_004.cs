/*
 * Author       :  
 * CreateDate   : 18/04/2011
 * Description  : List Approved Note For 1 PR
 */
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_03_004 : DaiCo.Shared.MainUserControl
  {
    #region Field
    // PR No
    public string prNo = string.Empty;
    #endregion Field

    #region Init
    public viewPUR_03_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_03_004_Load(object sender, EventArgs e)
    {
      this.lblPRNO.Text = this.prNo;

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT APP.Pid, VHR.HoNV + ' ' + VHR.TenNV ApproveBy, CONVERT(VARCHAR, APP.ApprovedDate, 103) ApproveDate, APP.Remark";
      commandText += " FROM TblPURPRApprove APP";
      commandText += " INNER JOIN TblPURPRApproveDetail APPDT ON APP.Pid = APPDT.PRApprovePid";
      commandText += " INNER JOIN TblPURPRDetail PRDT ON PRDT.Pid = APPDT.PRDetailPid";
      commandText += " LEFT JOIN VHRNhanVien VHR ON APP.ApprovedBy	= VHR.ID_NhanVien";
      commandText += " WHERE PRDT.PRNo = '" + this.prNo + "'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dt != null && dt.Rows.Count > 0)
      {
        this.ultData.DataSource = dt;
      }

    }
    #endregion Init

    #region Event
    /// <summary>
    /// New ==> Open Approve Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPUR_03_005 view = new viewPUR_03_005();
      view.prNo = this.prNo;
      WindowUtinity.ShowView(view, "Approved", true, ViewState.ModalWindow);

      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Close Tab Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Init Gird
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();

      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["ApproveBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ApproveBy"].Header.Caption = "Approve By";
      e.Layout.Bands[0].Columns["ApproveDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ApproveDate"].Header.Caption = "Approve Date";
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Double Click ==> Open Approve Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      //if (ultData.Selected != null && ultData.Selected.Rows.Count > 0)
      //{
      //  viewPUR_03_005 view = new viewPUR_03_005();
      //  view.approvePid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["Pid"].Value.ToString());
      //  view.prNo = this.prNo;
      //  view.remark = ultData.Selected.Rows[0].Cells["Remark"].Value.ToString();
      //  Shared.Utility.WindowUtinity.ShowView(view, "Approve", false, DaiCo.Shared.Utility.ViewState.ModalWindow);

      //  // Load Data Again
      //  this.LoadData();
      //}
    }
    #endregion Event
  }
}

