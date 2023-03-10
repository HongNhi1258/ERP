/*
  Author      : Do Tam
  Date        : 17/05/2013
  Description : Change Note Confirm ShipDate List
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace DaiCo.Planning
{
  public partial class viewPLN_02_018 : MainUserControl
  {
    #region Init
    public viewPLN_02_018()
    {
      InitializeComponent();
    }
    #endregion Init

    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    #endregion Field

    #region Load Data

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      // Customer
      // Type
      // Create By
      Shared.Utility.ControlUtility.LoadEmployee(cmbCreateBy, "CSD");
    }

    #endregion Load Data

    #region Search

    /// <summary>
    /// Edit layout
    /// </summary>
    private void Editlayout()
    {
      ultData.DisplayLayout.Bands["dtParent_dtChild"].Columns["ParentPid"].Hidden = true;
      ultData.DisplayLayout.Bands["dtParent"].Columns["Pid"].Hidden = true;
      ultData.DisplayLayout.Bands["dtParent"].Columns["No"].Header.Caption = "Sale No";
      ultData.DisplayLayout.Bands["dtParent"].Columns["CusNo"].Header.Caption = "Customer Po No";
      ultData.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
      ultData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[23];
      // No
      string text = txtNoFrom.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@NoFrom", DbType.AnsiString, 20, text);
      }
      text = txtNoTo.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@NoTo", DbType.AnsiString, 20, text);
      }
      text = txtNoSet.Text.Trim();
      string[] listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[2] = new DBParameter("@NoSet", DbType.AnsiString, 1000, text);
      }
      // Customer's PO No
      text = txtCusNoFrom.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@CusOrderNoFrom", DbType.AnsiString, 20, text);
      }
      text = txtCusNoTo.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@CusOrderNoTo", DbType.AnsiString, 20, text);
      }
      text = txtCusNoSet.Text.Trim();
      listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[5] = new DBParameter("@CusOrderNoSet", DbType.AnsiString, 1000, text);
      }
      //Order Date

      if (dt_POFrom.Value != null)
      {
        DateTime orderDate = DBConvert.ParseDateTime(dt_POFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        param[6] = new DBParameter("@OrderDateFrom", DbType.DateTime, orderDate);
      }


      if (dt_POTo.Value != null)
      {
        DateTime orderDate = DBConvert.ParseDateTime(dt_POTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        orderDate = orderDate.AddDays(1);
        param[7] = new DBParameter("@OrderDateTo", DbType.DateTime, orderDate);
      }
      int value = int.MinValue;
      // Status
      try
      {
        value = cmbStatus.SelectedIndex;
        value = (value == -1 || value == 0) ? -1 : value - 1;
      }
      catch
      {
        value = int.MinValue;
      }
      if (value >= 0)
      {
        param[10] = new DBParameter("@Status", DbType.Int32, value);
      }
      // ItemCode
      text = txtItemCode.Text.Trim();
      if (text.Length > 0)
      {
        param[11] = new DBParameter("@ItemCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
      }
      // Create By
      value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCreateBy));
      if (value != int.MinValue)
      {
        param[12] = new DBParameter("@CreateBy", DbType.Int32, value);
      }
      // SaleCode
      text = txtSaleCode.Text.Trim();
      if (text.Length > 0)
      {
        param[13] = new DBParameter("@SaleCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
      }
      // OldCode
      text = txtOldCode.Text.Trim();
      if (text.Length > 0)
      {
        param[14] = new DBParameter("@OldCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
      }
      // WO
      text = txtWoFrom.Text.Trim().Replace("'", "");
      if (DBConvert.ParseLong(text) > 0)
      {
        param[15] = new DBParameter("@WoNoFrom", DbType.Int64, DBConvert.ParseLong(text));
      }
      text = txtWoTo.Text.Trim().Replace("'", "");
      if (DBConvert.ParseLong(text) > 0)
      {
        param[16] = new DBParameter("@WoNoTo", DbType.Int64, DBConvert.ParseLong(text));
      }
      text = txtWoNoSet.Text.Trim().Replace("'", "");
      if (DBConvert.ParseLong(text) > 0)
      {
        param[17] = new DBParameter("@WoNoSet", DbType.Int64, DBConvert.ParseLong(text));
      }
      text = txtCarcassCode.Text.Trim();
      if (text.Length > 0)
      {
        param[18] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, text);
      }
      param[19] = new DBParameter("@EmpDepartment", DbType.AnsiString, 16, SharedObject.UserInfo.Department);
      if (chkPending.Checked)
      {
        param[20] = new DBParameter("@Pending", DbType.Int32, 1);
      }
      param[21] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);

      string storeName = "spPLNWoChangeNoteDeadline_Select";
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, 900, param, null);
      DataSet dsData = new DaiCo.Shared.DataSetSource.Planning.dsPLNWoChangeNoteDeadlineNew();
      dsData.Tables["Transaction"].Merge(dsSource.Tables[0]);
      dsData.Tables["TransactionDetail"].Merge(dsSource.Tables[1]);
      dsData.Tables["ContainerStatus"].Merge(dsSource.Tables[2]);

      ultData.DataSource = dsData;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string status = ultData.Rows[i].Cells["ST"].Value.ToString().Trim();
        if (status == "1")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (status == "2")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
      }
    }

    #endregion Search

    #region Event

    /// <summary>
    /// Load Date
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_01_003_Load(object sender, EventArgs e)
    {
      dt_POFrom.Value = DateTime.Today.AddDays(-3);
      dt_POTo.Value = DateTime.Today.AddDays(3);
      this.LoadData();
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Enabled = false;
      // Search Data
      this.Search();
      this.Enabled = true;
    }

    /// <summary>
    /// Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = true;

      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      DataTable dtNew = ((DataSet)ultData.DataSource).Tables[1];
      for (int i = 0; i < dtNew.Columns.Count; i++)
      {
        if (dtNew.Columns[i].DataType == typeof(Int32) || dtNew.Columns[i].DataType == typeof(float) || dtNew.Columns[i].DataType == typeof(Double))
        {
          ultData.DisplayLayout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dtNew.Columns[i].DataType == typeof(DateTime))
        {
          ultData.DisplayLayout.Bands[1].Columns[i].Format = "dd-MMM-yyyy";
        }
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["STT"].Header.Caption = "Status";

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Reason"].Hidden = true;
      e.Layout.Bands[0].Columns["ST"].Hidden = true;
      e.Layout.Bands[0].Columns["SST"].Hidden = true;
      e.Layout.Bands[0].Columns["WorkAreaPid"].Hidden = true;
      e.Layout.Bands[0].Columns["DepartmentCreated"].Hidden = true;
      e.Layout.Bands[0].Columns["CreateDateOr"].Hidden = true;
      e.Layout.Bands[0].Columns["CreateDateOr"].Hidden = true;
      e.Layout.Bands[1].Columns["ChangeDeadlineDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Approved"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Pending"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Deleted"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Pending"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Approved"].Header.Caption = "Approved";
      e.Layout.Bands[1].Columns["Approved"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[1].Columns["NewDeadline"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[1].Columns["OldDeadline"].CellAppearance.BackColor = Color.LightBlue;

      // Set header height
      e.Layout.Bands[1].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Deleted"].CellActivation = Activation.AllowEdit;

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Double click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //open SO form
      try
      {
        UltraGridRow row = ultData.Selected.Rows[0];
        viewPLN_02_017 uc = new viewPLN_02_017();
        long pid = long.MinValue;
        try
        {
          pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        }
        catch
        {
        }
        if (pid > 0)
        {
          uc.transactionPid = pid;
          Shared.Utility.WindowUtinity.ShowView(uc, "CONFIRMED DEADLINE ADJUSTMENT", false, Shared.Utility.ViewState.MainWindow);
        }
      }
      catch
      { }
    }

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      foreach (Control o in tableLayoutPanel1.Controls)
      {
        if (o.GetType() != typeof(Label))
        {
          o.Text = "";

        }
      }
      txtSaleCode.Text = "";
      dt_POFrom.Value = null;
      dt_POTo.Value = null;
      chkPending.Checked = false;
    }
    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event

    private void button1_Click(object sender, EventArgs e)
    {
      //open SO form
      try
      {
        viewPLN_02_017 uc = new viewPLN_02_017();
        Shared.Utility.WindowUtinity.ShowView(uc, "DEADLINE ADJUSTMENT", false, Shared.Utility.ViewState.MainWindow);
      }
      catch
      { }
    }

    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      if (e.Cell.Column.Header.Caption == "Deleted")
      {
        string strValue = e.Cell.Text.ToString();
        if (strValue == "1")
        {
          if (e.Cell.Row.Cells["ST"].Value.ToString() != "0")
          {
            e.Cell.Value = 0;
            return;
          }
        }
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (WindowUtinity.ShowMessageConfirm("MSG0007", "Deadline Transaction").ToString() == "No")
      {
        return;
      }
      string strValue = ",";
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["Deleted"].Value.ToString() == "1")
        {
          strValue += ultData.Rows[i].Cells["Pid"].Value.ToString() + ",";
        }
      }
      if (strValue != ",")
      {
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@StringParamPid", DbType.String, 4000, strValue);

        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteDeadline_Delete", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          WindowUtinity.ShowMessageError("ERR0093", "Deadline Transaction");
          return;
        }
        WindowUtinity.ShowMessageSuccess("MSG0002");
        btnSearch_Click(sender, e);
      }
    }
  }
}
