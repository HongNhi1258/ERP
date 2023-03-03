/*
  Author      : 
  Date        : 07/02/2011
  Description : List Supplement  
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_098 : MainUserControl
  {
    #region Init Form
    private string STT_NON = "";
    private string STT_NON_CONFIRM = "NON Confirmed";
    private string STT_WAITING_FOR_ISSUE = "Waiting For Issue";
    private string STT_ISSUED = "Issued";

    public viewPLN_07_098()
    {
      InitializeComponent();
      this.drpDateFrom.FormatString = ConstantClass.FORMAT_DATETIME;
      this.drpDateTo.FormatString = ConstantClass.FORMAT_DATETIME;
    }

    /// <summary>
    /// frmBOMListPackage_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_07_098_Load(object sender, EventArgs e)
    {
      this.Text = this.Text.ToString() + " | " + SharedObject.UserInfo.UserName + " | " + SharedObject.UserInfo.LoginDate;
      // Load Init
      this.LoadInit();
      //this.Search();
    }

    /// <summary>
    /// Load Init 
    /// </summary>
    private void LoadInit()
    {
      this.cmbStatus.Items.Add(STT_NON);
      this.cmbStatus.Items.Add(STT_NON_CONFIRM);
      this.cmbStatus.Items.Add(STT_WAITING_FOR_ISSUE);
      this.cmbStatus.Items.Add(STT_ISSUED);

      string commandText = "SELECT ID_NhanVien , CAST(ID_NhanVien AS VARCHAR) + ' - ' + HoNV + ' ' + TenNV Name FROM VHRNhanVien";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadMultiCombobox(multiCBCreateBy, dtSource, "ID_NhanVien", "Name");
      multiCBCreateBy.ColumnWidths = "0, 400";
    }
    #endregion Init Form

    #region Process
    private void Search()
    {
      DateTime dateFrom = (drpDateFrom.Value != null ? (DateTime)drpDateFrom.Value : DateTime.MinValue);
      DateTime dateTo = (drpDateTo.Value != null ? (DateTime)drpDateTo.Value : DateTime.MinValue);
      if (dateTo != DateTime.MinValue)
      {
        dateTo = dateTo.AddDays(1);
      }

      DBParameter[] inputParam = new DBParameter[7];
      string text = txtSuppNo.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[0] = new DBParameter("@SupplementNo", DbType.AnsiString, 16, text);
      }

      if (this.cmbStatus.Text == this.STT_NON_CONFIRM)
      {
        inputParam[1] = new DBParameter("@Status", DbType.Int32, 0);
      }
      else if (this.cmbStatus.Text == this.STT_WAITING_FOR_ISSUE)
      {
        inputParam[1] = new DBParameter("@Status", DbType.Int32, 1);
      }
      else if (this.cmbStatus.Text == this.STT_ISSUED)
      {
        inputParam[1] = new DBParameter("@Status", DbType.Int32, 2);
      }

      text = txtWO.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[2] = new DBParameter("@WO", DbType.AnsiString, 100, text);
      }

      text = txtMaterial.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, text);
      }

      if (this.multiCBCreateBy.SelectedValue != null && this.multiCBCreateBy.SelectedValue.ToString().Length > 0)
      {
        inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(this.multiCBCreateBy.SelectedValue.ToString()));
      }

      //Date From 
      if (dateFrom != DateTime.MinValue)
      {
        inputParam[5] = new DBParameter("@CreateDateFrom", DbType.DateTime, dateFrom);
      }

      //Date To
      if (dateTo != DateTime.MinValue)
      {
        inputParam[6] = new DBParameter("@CreateDateTo", DbType.DateTime, dateTo);
      }
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNListSupplement", inputParam);
      DataSet dsList = CreateDataSet.ListSupplement();
      //dsList.Tables.Add(ds.Tables[0].Clone());
      dsList.Tables["TblWO"].Merge(ds.Tables[0]);
      //dsList.Tables.Add(ds.Tables[1].Clone());
      dsList.Tables["TblWODetail"].Merge(ds.Tables[1]);

      ultData.DataSource = dsList;
    }
    #endregion Process

    #region Event
    /// <summary>
    /// btnSearch_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// btnNew_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_07_099 view = new viewPLN_07_099();
      Shared.Utility.WindowUtinity.ShowView(view, "Supplement Information", false, Shared.Utility.ViewState.ModalWindow);
      this.Search();
    }

    /// <summary>
    /// Close tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// InitializeLayout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["SupplementNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["SupplementPid"].Hidden = true;
      e.Layout.Bands[1].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["MaterialNameEn"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["WoPid"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Issued"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Supplement"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Reason"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["WoPid"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Issued"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Supplement"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["SupplementNo"].Header.Caption = "Supp. No";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[1].Columns["MaterialNameEn"].Header.Caption = "Material Name";
      e.Layout.Bands[1].Columns["WoPid"].Header.Caption = "WO";
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0 && ultData.Selected.Rows[0].Band.ParentBand == null)
      {
        long suppPid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["Pid"].Value.ToString());
        string suppNo = ultData.Selected.Rows[0].Cells["SupplementNo"].Value.ToString();
        viewPLN_07_099 view = new viewPLN_07_099();
        view.suppPid = suppPid;
        view.supplementNo = suppNo;
        WindowUtinity.ShowView(view, "Supplement Information", true, ViewState.Window);
      }
    }
    #endregion Event
  }
}
