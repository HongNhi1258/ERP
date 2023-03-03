using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_10_054 : MainUserControl
  {
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public viewPLN_10_054()
    {

      InitializeComponent();
      dt_CreateFrom.Value = DateTime.MinValue;
      dt_CreateTo.Value = DateTime.MinValue;

      //this.btnUnlock.Visible = false;
    }

    public void Search()
    {
      DBParameter[] param = new DBParameter[4];

      long wo = DBConvert.ParseLong(txtWorkOrderForm.Text);
      if (wo != long.MinValue)
      {
        param[0] = new DBParameter("@WoFrom", DbType.Int64, wo);
      }

      wo = DBConvert.ParseLong(txtWorkOrderTo.Text);
      if (wo != long.MinValue)
      {
        param[1] = new DBParameter("@WoTo", DbType.Int64, wo);
      }

      DateTime createDate = dt_CreateFrom.Value;
      if (createDate != DateTime.MinValue)
      {
        DateTime createDateFrom = createDate;
        param[2] = new DBParameter("@CreateDateFrom", DbType.DateTime, new DateTime(createDateFrom.Year, createDateFrom.Month, createDateFrom.Day));
      }

      createDate = dt_CreateTo.Value;
      if (dt_CreateTo.Value != DateTime.MinValue)
      {
        DateTime createDateTo = createDate;
        createDateTo = (createDateTo != DateTime.MaxValue) ? createDateTo.AddDays(1) : createDateTo;
        param[3] = new DBParameter("@CreateDateTo", DbType.DateTime, new DateTime(createDateTo.Year, createDateTo.Month, createDateTo.Day));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNListWorkOrderForUpdateLotNo", 600, param);
      ultData.DataSource = dtSource;
    }

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }


    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.SetPropertiesUltraGrid(ultData);

      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Columns["Wo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Wo"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Confirm"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["ScanDate"].Header.Caption = "Scan Date";
      e.Layout.Bands[0].Columns["Wo"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Confirm"].Hidden = true;

      e.Layout.Override.RowAppearance.BackColorAlpha = Alpha.Transparent;

      e.Layout.Override.CellAppearance.BackColorAlpha = Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Alpha.UseAlphaLevel;
      //e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
    }

    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      long pid = DBConvert.ParseLong(row.Cells["Wo"].Value.ToString());
      //int closedWo = DBConvert.ParseInt(row.Cells["WoClosed"].Value.ToString());
      string commandTextNeedToSwap = string.Format(@"SELECT CASE WHEN WO.CreateDate > CM.ConditionDate THEN 1 ELSE 0 END
                                      FROM
                                      (	Select CAST(Value as date) ConditionDate from TblBOMCodeMaster WHERE [Group] = 16003
                                      ) CM
                                      INNER JOIN
                                      (	SELECT CAST(CreateDate as Date) CreateDate FROM TblPLNWorkOrder	WHERE Pid = {0}
                                      ) WO ON 1 = 1", pid);
      int iNew = (int)DataBaseAccess.ExecuteScalarCommandText(commandTextNeedToSwap);
      if (iNew == 0)
      {
        viewPLN_02_002 uc = new viewPLN_02_002();
        uc.woPid = pid;
        // uc.isCloseWo = closedWo;
        Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE WORK ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
      }
      else
      {
        viewPLN_02_025 uc = new viewPLN_02_025();
        uc.pid = pid;
        // uc.isCloseWo = closedWo;
        Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE WORK ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtWorkOrderForm.Text = string.Empty;
      txtWorkOrderTo.Text = string.Empty;
      dt_CreateFrom.Value = DateTime.MinValue;
      dt_CreateTo.Value = DateTime.MinValue;
    }

    private void viewPLN_10_054_Load(object sender, EventArgs e)
    {
      string strf = this.GetType().ToString();
    }

    private void txtWorkOrderForm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtWorkOrderTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dt_CreateFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dt_CreateTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion Event
  }
}
