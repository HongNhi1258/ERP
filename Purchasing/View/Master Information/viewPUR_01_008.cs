/*
 * Author       : Vo Van Duy Qui
 * CreateDate   : 29-03-2011
 * Description  : List Currency Exchange Rate
 */

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_01_008 : MainUserControl
  {
    #region Init

    /// <summary>
    /// Init Form
    /// </summary>
    public viewPUR_01_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_008_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region LoadData

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      string commandText = "SELECT Pid, Code, Name, CurrentExchangeRate, ActualCurrentExchangeRate FROM TblPURCurrencyInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultData.DataSource = dtSource;
    }
    #endregion LoadData

    #region Event

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["CurrentExchangeRate"].Header.Caption = "Standard Current Exchange Rate";
      e.Layout.Bands[0].Columns["CurrentExchangeRate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CurrentExchangeRate"].Format = "###,###.## VND";
      e.Layout.Bands[0].Columns["ActualCurrentExchangeRate"].Header.Caption = "Actual Current Exchange Rate";
      e.Layout.Bands[0].Columns["ActualCurrentExchangeRate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ActualCurrentExchangeRate"].Format = "###,###.## VND";
      e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
    }

    /// <summary>
    /// ultData Mouse Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultData.Selected != null && ultData.Selected.Rows.Count > 0)
      {
        viewPUR_01_009 view = new viewPUR_01_009();
        view.currencyPid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["Pid"].Value.ToString());
        if (btnPerApproveCurrencyStandard.Visible)
        {
          view.approvedStandard = true;
        }
        else
        {
          view.approvedStandard = false;
        }

        Shared.Utility.WindowUtinity.ShowView(view, "Currency Exchange Rate Information", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
        this.LoadData();
      }
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
