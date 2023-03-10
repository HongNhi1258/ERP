/*
  Author      : 
  Date        : 02/11/2012
  Description : List Of Item Need To Review
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_002 : MainUserControl
  {
    #region Field
    private BackgroundWorker bw = new BackgroundWorker();
    #endregion Field

    #region Init
    public viewPLN_98_002()
    {
      InitializeComponent();
    }

    private void viewWHD_05_002_Load(object sender, EventArgs e)
    {
      this.LoadWorkArea();
    }

    void StartForm(ProgressForm form)
    {
      DialogResult result = form.ShowDialog();
      if (result == DialogResult.Cancel)
        MessageBox.Show("Operation has been cancelled");
      if (result == DialogResult.Abort)
        MessageBox.Show("Exception:" + Environment.NewLine + form.Result.Error.Message);
    }

    void form_DoWork(ProgressForm sender, DoWorkEventArgs e)
    {
      bool throwException = (bool)e.Argument;

      for (int i = 0; i < 100; i++)
      {
        System.Threading.Thread.Sleep(600);
        sender.SetProgress(i, "Step " + i.ToString() + " %");
        if (sender.CancellationPending)
        {
          e.Cancel = true;
          return;
        }
      }

    }

    private void Progessbar()
    {

      ProgressForm form = new ProgressForm();
      form.Text = "Please Waiting";
      form.Argument = false;
      form.DoWork += new ProgressForm.DoWorkEventHandler(form_DoWork);
      StartForm(form);
      form.FormBorderStyle = FormBorderStyle.None;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load WorkArea
    /// </summary>
    private void LoadWorkArea()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, WorkAreaName + ' - ' + WorkAreaCode Name ";
      commandText += " FROM TblWIPWorkArea ";
      commandText += " ORDER BY WorkAreaName ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultWorkArea.DataSource = dtSource;
      ultWorkArea.DisplayMember = "Name";
      ultWorkArea.ValueMember = "Pid";
      ultWorkArea.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultWorkArea.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultWorkArea.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[12];

      // Work Area
      if (this.ultWorkArea.Value != null && DBConvert.ParseInt(this.ultWorkArea.Value.ToString()) != int.MinValue)
      {
        param[0] = new DBParameter("@WorkArea", DbType.Int32, DBConvert.ParseInt(this.ultWorkArea.Value.ToString()));
      }

      // SaleCode
      string text = string.Empty;
      text = this.txtSaleCode.Text;
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@SaleCode", DbType.String, text);
      }

      // Carcass Code
      text = this.txtCarcassCode.Text;
      if (text.Length > 0)
      {
        param[2] = new DBParameter("@CarcassCode", DbType.String, text);
      }

      // Item Code
      text = this.txtItemCode.Text;
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@ItemCode", DbType.String, text);
      }

      // Old Code
      text = this.txtOldCode.Text;
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@OldCode", DbType.String, text);
      }

      // Customer PO No
      text = this.txtCusPONo.Text;
      if (text.Length > 0)
      {
        param[5] = new DBParameter("@CustomerPONo", DbType.String, text);
      }

      // WO
      text = this.txtWO.Text;
      if (text.Length > 0)
      {
        param[7] = new DBParameter("@WO", DbType.String, text);
      }

      // Late From
      text = this.txtLateFrom.Text;
      if (text.Length > 0)
      {
        param[8] = new DBParameter("@LateFrom", DbType.Int32, DBConvert.ParseInt(text));
      }

      // Late To
      text = this.txtLateTo.Text;
      if (text.Length > 0)
      {
        param[9] = new DBParameter("@LateTo", DbType.Int32, DBConvert.ParseInt(text));
      }

      // FIN From
      text = this.txtFINFrom.Text;
      if (text.Length > 0)
      {
        param[10] = new DBParameter("@FINFrom", DbType.Int32, DBConvert.ParseInt(text));
      }

      // FIN To
      text = this.txtFINTo.Text;
      if (text.Length > 0)
      {
        param[11] = new DBParameter("@FINTo", DbType.Int32, DBConvert.ParseInt(text));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanContainerListOfItemNeedReview_Select", 5000, param);
      if (dtSource != null)
      {
        this.ultData.DataSource = dtSource;
      }
      else
      {
        ultData.DataSource = DBNull.Value;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int type = DBConvert.ParseInt(ultData.Rows[i].Cells["KindEarlyLate"].Value.ToString());
        if (type == 0)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (type == 1)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }

        if (ultData.Rows[i].Cells["Priority"].Value.ToString().Length > 0)
        {
          int priority = DBConvert.ParseInt(ultData.Rows[i].Cells["Priority"].Value.ToString());
          if (priority != int.MinValue)
          {
            ultData.Rows[i].Cells["No"].Appearance.FontData.Bold = DefaultableBoolean.True;
            ultData.Rows[i].Cells["No"].Appearance.ForeColor = Color.Red;
          }
        }
      }
      if (ultShowColumn.Rows.Count == 0)
      {
        this.LoadColumnName();
      }
      else
      {
        this.SetStatusColumn();
      }
    }

    /// <summary>
    ///  Set Status Column When Search
    /// </summary>
    private void SetStatusColumn()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// Load Column Name
    /// </summary>
    private void LoadColumnName()
    {
      DataTable dtNew = new DataTable();
      DataTable dtColumn = (DataTable)ultData.DataSource;
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        dtNew.Columns[column.ColumnName].DefaultValue = 0;

        if (string.Compare(column.ColumnName, "Qty", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "WO", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "StatusWIP", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "Priority", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "ContainerNo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LoadingDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "NewDeadline", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "FinshDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LateDay", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LeadTimeForFIN", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "FinishDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      ultShowColumn.DataSource = dtNew;
      ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;
        Utility.ExportToExcelWithDefaultPath(ultData, out xlBook, "ITEM LIST CHANGED DEADLINE", 7);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "ITEM LIST CHANGED DEADLINE";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        xlSheet.Cells[5, 1] = "Note: ";
        r.Font.Bold = true;

        xlSheet.Cells[5, 2] = "On Time";

        xlSheet.Cells[5, 3] = "Late";
        r = xlSheet.get_Range("C5", "C5");
        r.Interior.Color = (object)ColorTranslator.ToOle(Color.FromArgb(255, 255, 0));

        xlSheet.Cells[5, 4] = "Early ";
        r = xlSheet.get_Range("D5", "D5");
        r.Interior.Color = (object)ColorTranslator.ToOle(Color.FromArgb(144, 238, 144));

        xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Init Layout 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["No"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["WorkArea"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["OldCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["WorkAreaPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerListDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["KindEarlyLate"].Hidden = true;

      e.Layout.Bands[0].Columns["Customer"].Hidden = true;
      e.Layout.Bands[0].Columns["DirectCustomer"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerPONo"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Hidden = true;
      e.Layout.Bands[0].Columns["NewDeadline"].Header.Caption = "Deadline";

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LateDay"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LeadTimeForFIN"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Priority"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Search 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
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

    //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    //{
    //  if (keyData == Keys.Enter)
    //  {
    //    this.Search();
    //    return true;
    //  }
    //  return base.ProcessCmdKey(ref msg, keyData);
    //}

    //private void btnRefreshData_Click(object sender, EventArgs e)
    //{
    //  System.Threading.Thread Thead2 = new System.Threading.Thread(new System.Threading.ThreadStart(Progessbar));
    //  Thead2.Start();
    //  DBParameter[] inputParam = new DBParameter[2];
    //  DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerSummarizeTotalData_Insert", 300, inputParam);
    //  WindowUtinity.ShowMessageSuccess("MSG0059");
    //  this.Search();
    //}

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }

    private void ultShowColumn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      DataTable dtColumn = (DataTable)ultShowColumn.DataSource;
      int count = dtColumn.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      e.Layout.Bands[0].Columns["WorkAreaPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["KindEarlyLate"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerListDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].Hidden = true;
      e.Layout.Bands[0].Columns["WorkArea"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleCode"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["OldCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;

      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    private void ultShowColumn_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
        }
      }
      else
      {
        for (int i = 1; i < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          row.Cells[i].Value = e.Cell.Text;
        }
        this.ChkAll_CheckedChange();
      }
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0 && ultData.Selected.Rows.Count > 0)
      {
        string itemCode = ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString().Trim();
        string container = ultData.Selected.Rows[0].Cells["ContainerNo"].Value.ToString().Trim();
        if (itemCode.Length > 0)
        {
          viewPLN_98_004 view = new viewPLN_98_004();
          view.itemCode = itemCode;
          view.container = container;
          Shared.Utility.WindowUtinity.ShowView(view, "MASTER PLAN FOR CONTAINER", true, DaiCo.Shared.Utility.ViewState.MainWindow);
        }
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      ultWorkArea.Value = DBNull.Value;
      txtCarcassCode.Text = string.Empty;
      txtCusPONo.Text = string.Empty;
      txtFINFrom.Text = string.Empty;
      txtFINTo.Text = string.Empty;
      txtLateFrom.Text = string.Empty;
      txtLateTo.Text = string.Empty;
      txtSaleCode.Text = string.Empty;
      txtWO.Text = string.Empty;
      txtItemCode.Text = string.Empty;
      txtOldCode.Text = string.Empty;
    }
    #endregion Event
  }
}
