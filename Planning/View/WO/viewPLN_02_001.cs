using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using FormSerialisation;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_001 : MainUserControl
  {
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public viewPLN_02_001()
    {

      InitializeComponent();
      dt_CreateFrom.Value = DateTime.MinValue;
      dt_CreateTo.Value = DateTime.MinValue;

      //this.btnUnlock.Visible = false;
    }

    private void LoadDropDownSaleorder()
    {
      string commandText = string.Format(@"SELECT DISTINCT SO.Pid , SO.SaleNo FROM TblPLNSaleOrder SO ORDER BY SO.SaleNo DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      cmdSaleorder.DataSource = dt;
      cmdSaleorder.DisplayMember = "SaleNo";
      cmdSaleorder.ValueMember = "Pid";
      cmdSaleorder.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      cmdSaleorder.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    public void Search()
    {
      DBParameter[] param = new DBParameter[8];

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

      int index = cmdStatus.SelectedIndex - 1;
      if (index >= 0)
      {
        param[4] = new DBParameter("@Status", DbType.Int32, index);
      }

      index = cmdConfirm.SelectedIndex - 1;
      if (index >= 0)
      {
        param[5] = new DBParameter("@Confirm", DbType.Int32, index);
      }

      string text = txtItemCode.Text.Trim().Replace("'", "''");
      if (text.Length > 0)
      {
        param[6] = new DBParameter("@ItemCode", DbType.AnsiString, 18, "%" + text + "%");
      }
      if (cmdSaleorder.Value != null)
      {
        long saleorderpid = DBConvert.ParseLong(cmdSaleorder.Value.ToString());
        param[7] = new DBParameter("@SaleOrderPid", DbType.Int64, saleorderpid);
      }
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNListWorkOrder", 600, param);
      dsSource.Relations.Add(new DataRelation("dtParent_dtChild", dsSource.Tables[0].Columns["Wo"], dsSource.Tables[1].Columns["Wo"], false));
      ultData.DataSource = dsSource;
    }

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_02_025 uc = new viewPLN_02_025();
      Shared.Utility.WindowUtinity.ShowView(uc, "INSERT WORK ORDER INFO", true, Shared.Utility.ViewState.MainWindow);
    }

    private void btnUnlock_Click(object sender, EventArgs e)
    {

      int count = ultData.Rows.Count;
      bool selected = false;
      int count_unlock = 0;
      for (int i = 0; i < count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString()) == 1)
        {
          count_unlock++;
          if (count_unlock >= 2)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0103"));
            WindowUtinity.ShowMessageErrorFromText(message);
            return;
          }
        }
      }

      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        bool unlock = (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1);
        if (unlock)
        {
          selected = true;
          long pid = DBConvert.ParseLong(ultData.Rows[i].Cells["Wo"].Value.ToString());
          Shared.Utility.FunctionUtility.UnlockPLNWorkOrder(pid);
        }
      }
      if (!selected)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0014");
        //MessageBox.Show("Please Selected Some Item!");
      }
      //else
      //{
      //  Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0023");
      //  //MessageBox.Show("Unlocked successful!");
      //}
      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      FormSerialisor.Serialise(this, ConstantClass.PATHCOOKIE + @"\viewPLN_02_001.xml");
      this.ConfirmToCloseTab();
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands["dtParent"].Columns["Wo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent"].Columns["Wo"].Header.Caption = "WO";
      e.Layout.Bands["dtParent"].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent"].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent"].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent"].Columns["Confirm"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent"].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent"].Columns["WoClosed"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent"].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands["dtParent"].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands["dtParent"].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["dtParent"].Columns["WoClosed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["dtParent"].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands["dtParent"].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands["dtParent"].Columns["Wo"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands["dtParent"].Columns["WoClosed"].Hidden = true;
      e.Layout.Bands["dtParent"].Columns["Confirm"].Hidden = true;

      e.Layout.Bands["dtParent_dtChild"].Columns["Wo"].Hidden = true;
      e.Layout.Bands["dtParent_dtChild"].Columns["Flag"].Hidden = true;
      e.Layout.Bands["dtParent_dtChild"].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands["dtParent_dtChild"].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands["dtParent_dtChild"].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands["dtParent_dtChild"].Override.CellClickAction = CellClickAction.RowSelect;
      e.Layout.Bands["dtParent_dtChild"].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands["dtParent_dtChild"].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands["dtParent_dtChild"].Columns["IsSubCon"].Header.Caption = "SubCon";
      e.Layout.Bands["dtParent_dtChild"].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
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
      int closedWo = DBConvert.ParseInt(row.Cells["WoClosed"].Value.ToString());
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
        uc.isCloseWo = closedWo;
        Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE WORK ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
      }
      else
      {
        viewPLN_02_025 uc = new viewPLN_02_025();
        uc.pid = pid;
        uc.isCloseWo = closedWo;
        Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE WORK ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtWorkOrderForm.Text = string.Empty;
      txtWorkOrderTo.Text = string.Empty;
      txtItemCode.Text = string.Empty;
      dt_CreateFrom.Value = DateTime.MinValue;
      dt_CreateTo.Value = DateTime.MinValue;
      cmdSaleorder.Text = string.Empty;
      cmdStatus.SelectedIndex = -1;
      cmdConfirm.SelectedIndex = -1;
    }

    private void viewPLN_02_001_Load(object sender, EventArgs e)
    {
      string strf = this.GetType().ToString();
      LoadDropDownSaleorder();
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

    private void cmdStatus_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmdConfirm_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmdSaleorder_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void btnCapture_Click(object sender, EventArgs e)
    {
      string strTypeObject = this.GetType().FullName + "," + this.GetType().Namespace.Split('.')[1];
      string strTitle = SharedObject.tabContent.TabPages[SharedObject.tabContent.SelectedIndex].Text;
      string strFileName = this.Name + ".xml";
      long TaskPid = FunctionUtility.databaseFilePut(FormSerialisor.Serialise(this), strTypeObject, strTitle, strFileName);
      viewMAI_01_002 o = new viewMAI_01_002();
      o.pid = TaskPid;
      DaiCo.Shared.Utility.WindowUtinity.ShowView(o, "TASK TRANSFER", true, ViewState.MainWindow);
    }
    #endregion Event
  }
}
