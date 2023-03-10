/*
  Author      : 
  Date        : 02/11/2012
  Description : Container Priority
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
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
  public partial class viewPLN_98_006 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPLN_98_006()
    {
      InitializeComponent();
    }

    private void viewWHD_05_002_Load(object sender, EventArgs e)
    {
      // Show Data
      this.Search();
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
    /// Search
    /// </summary>
    private void Search()
    {
      DataTable dtData = DataBaseAccess.SearchCommandTextDataTable("spPLNMasterPlanContainerOverviewSelectData_Select");

      this.ultData.DataSource = dtData;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["ContainerNo"].Value.ToString().Trim().Length == 0)
        {
          ultData.Rows[i].Appearance.FontData.Bold = DefaultableBoolean.True;
          ultData.Rows[i].Appearance.ForeColor = Color.Red;
        }
        else
        {
          ultData.Rows[i].Cells["MonthYear"].Appearance.FontData.Bold = DefaultableBoolean.True;
          ultData.Rows[i].Cells["MonthYear"].Appearance.ForeColor = Color.RoyalBlue;
          ultData.Rows[i].Cells["Remark"].Appearance.BackColor = Color.LightBlue;
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

        ultData.Rows.Band.Columns["Remark"].Hidden = true;
        Utility.ExportToExcelWithDefaultPath(ultData, out xlBook, "CONTAINER STATUS OVERVIEW", 6);
        ultData.Rows.Band.Columns["Remark"].Hidden = true;

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[3, 1] = "CONTAINER STATUS OVERVIEW";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlBook.Application.DisplayAlerts = false;

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
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["MonthYear"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ContainerNo"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["LoadingDate"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["LoadingQty"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["LoadingCBM"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Cust.Group"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["LoadingDate"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerPid"].Hidden = true;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Columns["LoadingDateStr"].Header.Caption = "LoadingDate";

      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["Remark"].MinWidth = 300;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
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

    //private void btnRefreshData_Click(object sender, EventArgs e)
    //{
    //  System.Threading.Thread Thead2 = new System.Threading.Thread(new System.Threading.ThreadStart(Progessbar));
    //  Thead2.Start();
    //  DBParameter[] inputParam = new DBParameter[2];
    //  DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerSummarizeTotalData_Insert", 300, inputParam);
    //  WindowUtinity.ShowMessageSuccess("MSG0059");
    //  this.Search();
    //}

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0 && ultData.Selected.Rows.Count > 0)
      {
        long containerPid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["ContainerPid"].Value.ToString().Trim());
        if (containerPid != long.MinValue)
        {
          viewPLN_98_007 uc = new viewPLN_98_007();
          uc.containerPid = containerPid;
          Shared.Utility.WindowUtinity.ShowView(uc, "CONTAINER STATUS OVERVIEW MOVE UP", true, DaiCo.Shared.Utility.ViewState.MainWindow);
        }
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      DataTable dtData = (DataTable)this.ultData.DataSource;
      foreach (DataRow row in dtData.Rows)
      {
        if (row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[2];
          inputParam[0] = new DBParameter("@ContainerPid", DbType.Int64, DBConvert.ParseLong(row["ContainerPid"].ToString()));
          inputParam[1] = new DBParameter("@Remark", DbType.String, row["Remark"].ToString());

          DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerOverviewUpdate_Select", inputParam);
        }
      }

      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.Search();
    }

    private void btnLoadData_Click(object sender, EventArgs e)
    {
      // Show Data
      this.Search();
      WindowUtinity.ShowMessageSuccessFromText("Load Data Successfully");
    }
    #endregion Event
  }
}
