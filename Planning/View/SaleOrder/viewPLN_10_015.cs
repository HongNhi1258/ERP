/*
  Author      : Do Tam
  Date        : 21/01/2013
  Description : Change Note Confirm ShipDate List
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace DaiCo.Planning
{
  public partial class viewPLN_10_015 : MainUserControl
  {
    #region Init
    public viewPLN_10_015()
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
      Shared.Utility.ControlUtility.LoadCustomer(cmbCustomer);
      // Type
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
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
      DBParameter[] param = new DBParameter[15];
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
      // Customer
      long customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      if (customerPid != long.MinValue)
      {
        param[8] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }
      // Type
      int value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      if (value != int.MinValue)
      {
        param[9] = new DBParameter("@Type", DbType.Int32, value);
      }
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
      string storeName = "spPLNSaleOrderChangeNoteShipDate_Select";
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      DataSet dsData = new DaiCo.Shared.DataSetSource.Planning.dsPLNSaleOrderChangeNoteShipDate();
      dsData.Tables["Transaction"].Merge(dsSource.Tables[0]);
      dsData.Tables["TransactionDetail"].Merge(dsSource.Tables[1]);
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
      e.Layout.Bands[0].Columns["SST"].Hidden = true;
      e.Layout.Bands[0].Columns["STT"].Header.Caption = "Status";

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Reason"].Hidden = true;
      e.Layout.Bands[0].Columns["ST"].Hidden = true;
      e.Layout.Bands[0].Columns["CreateDateOr"].Hidden = true;

      e.Layout.Bands[1].Columns["ChangeShipDateDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[1].Columns["FlagAccept"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["FlagAccept"].Header.Caption = "Accepted";
      e.Layout.Bands[1].Columns["OriginalShipDate"].Header.Caption = "Original\nShipDate";
      e.Layout.Bands[1].Columns["NewShipDate"].Header.Caption = "New\nShipDate";

      e.Layout.Bands[1].Columns["FlagAccept"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[1].Columns["NewShipDate"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[1].Columns["OriginalShipDate"].CellAppearance.BackColor = Color.LightBlue;

      // Set header height
      e.Layout.Bands[1].ColHeaderLines = 2;

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
        viewPLN_10_013 uc = new viewPLN_10_013();
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
          Shared.Utility.WindowUtinity.ShowView(uc, "CONFIRMED SHIP DATE ADJUSTMENT", false, Shared.Utility.ViewState.MainWindow);
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
    private void button1_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {

        Excel.Workbook xlBook;
        Shared.Utility.ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "List SO Change Ship Date Note Report", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "List SO Change Ship Date Note Report";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }
    #endregion Event




  }

}
