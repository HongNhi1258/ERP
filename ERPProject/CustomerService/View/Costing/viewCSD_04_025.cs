/*
  Author      : Nguyen Thanh Binh
  Date        : 03/12/2018
  Description : Search Transaction Item Price List
  Standard Form: view_SearchInfo.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace DaiCo.ERPProject
{
  public partial class viewCSD_04_025 : MainUserControl
  {
    #region field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    bool isApproved = false;
    #endregion field

    #region function

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 7;
      string storeName = string.Empty;
      storeName = "spCSDTransactionItemPrice_Select";
      DBParameter[] input = new DBParameter[paramNumber];

      if (txtTransactionCode.Text.Trim().Length > 0)
      {
        input[0] = new DBParameter("@TransactionCode", DbType.AnsiString, 50, txtTransactionCode.Text.Trim());
      }

      if (txtItemCode.Text.Trim().Length > 0)
      {
        input[1] = new DBParameter("@ItemCode", DbType.AnsiString, 20, txtItemCode.Text.Trim());
      }

      if (txtSaleCode.Text.Trim().Length > 0)
      {
        input[2] = new DBParameter("@SaleCode", DbType.AnsiString, 512, txtSaleCode.Text.Trim());
      }

      if (ucbCreateBy.Value != null)
      {
        input[3] = new DBParameter("@CreateBy", DbType.Int32, 50, DBConvert.ParseInt(ucbCreateBy.Value.ToString()));
      }

      if (udtCreateFrom.Value != null)
      {
        input[4] = new DBParameter("@CreateDateFrom", DbType.DateTime, DBConvert.ParseDateTime(udtCreateFrom.Value));
      }

      if (udtCreateTo.Value != null)
      {
        input[5] = new DBParameter("@CreateDateTo", DbType.DateTime, DBConvert.ParseDateTime(udtCreateTo.Value));
      }

      if (ucbStatus.Value != null)
      {
        input[6] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ucbStatus.Value));
      }


      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, 3000, input);

      dsSource.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { dsSource.Tables[0].Columns["Pid"] },
                                                  new DataColumn[] { dsSource.Tables[1].Columns["TransactionPid"] }, false));
      ugdtData.DataSource = dsSource;

      for (int i = 0; i < ugdtData.Rows.Count; i++)
      {

        int status = DBConvert.ParseInt(ugdtData.Rows[i].Cells["Status"].Value.ToString());
        if (status == 0)
        {
          ugdtData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (status == 1)
        {
          ugdtData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (status == 2)
        {
          ugdtData.Rows[i].CellAppearance.BackColor = Color.LightSkyBlue;
        }

        for (int j = 0; j < ugdtData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          ugdtData.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Plum;
        }
        ugdtData.Rows[i].Expanded = false;
      }

      lbCount.Text = string.Format("Count: {0}", ugdtData.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      ucbCreateBy.Value = null;
      txtTransactionCode.Text = string.Empty;
      txtItemCode.Text = string.Empty;
      udtCreateFrom.Value = null;
      udtCreateTo.Value = null;
      ucbStatus.Value = null;
      txtSaleCode.Text = string.Empty;
    }

    private void UpdateDeleteTransaction(string functionName, int status)
    {
      DataTable dtSource = (DataTable)ugdtData.DataSource;
      DataRow[] rows = dtSource.Select("Select = 1");
      if (rows.Length > 0)
      {
        string errorMessage = string.Empty;

        bool success = true;
        DBParameter[] inputParam = new DBParameter[1];
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

        foreach (DataRow row in rows)
        {
          //string code = row["CompCode"].ToString();

          //inputParam[0] = new DBParameter("@CompCode", DbType.AnsiString, 16, code);
          //DataBaseAccess.ExecuteStoreProcedure("spFOUCosting_UpdateConfirm", inputParam, outputParam);
          //long result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          //if (result != 1)
          //{
          //  success = false;
          //}
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SaveSuccess = true;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
          this.SaveSuccess = false;
        }
        this.SearchData();
      }
      else
      {
        WindowUtinity.ShowMessageErrorFromText("Please select the component which you want to unlock");
      }
    }
    #endregion function

    #region event
    public viewCSD_04_025()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_04_025_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.LoadStatus();
      foreach (Control ctr in groupBoxSearch.Controls)
      {
        ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      }
      this.LoadEmployeeCreateBy(ucbCreateBy);
      udtCreateFrom.Value = null;
      udtCreateTo.Value = null;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void LoadStatus()
    {
      string commandText = "SELECT 0 'Code', 'New' 'Display' ";
      commandText += "UNION ";
      commandText += "SELECT 1 , 'Waiting For Approval'";
      commandText += "UNION ";
      commandText += "SELECT 2 , 'Approved'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucbStatus.DataSource = dt;
      Utility.LoadUltraCombo(ucbStatus, dt, "Code", "Display", "Code");
    }

    private void LoadEmployeeCreateBy(UltraCombo UltraCombo)
    {
      string commandText = string.Format("SELECT Pid, CAST(Pid as varchar) + ' - ' + EmpName Description FROM VHRMEmployee WHERE Resigned = 0 AND Pid > 0 ORDER BY Pid");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCreateBy, dtSource, "Pid", "Description", false, "Pid");
      ucbCreateBy.DisplayLayout.Bands[0].Columns["Description"].Width = 300;
    }
    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      ugdtData.SyncWithCurrencyManager = false;
      ugdtData.StyleSetName = "Excel2013";


      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["Kind"].Hidden = true;
      e.Layout.Bands[1].Columns["TransactionPid"].Hidden = true;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      for (int i = 1; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["TransactionCode"].Header.Caption = "Transaction Code";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["ApprovedDate"].Header.Caption = "Approved Date";
      e.Layout.Bands[0].Columns["ApproveBy"].Header.Caption = "Approve By";
      e.Layout.Bands[0].Columns["Select"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Select"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["CreateDate"].MinWidth = 120;
      e.Layout.Bands[0].Columns["CreateDate"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["ApprovedDate"].MinWidth = 120;
      e.Layout.Bands[0].Columns["ApprovedDate"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["CreateBy"].MinWidth = 200;
      e.Layout.Bands[0].Columns["CreateBy"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["ApproveBy"].MinWidth = 200;
      e.Layout.Bands[0].Columns["ApproveBy"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["TransactionCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ApprovedDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ApproveBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[1].Columns["OldPrice"].Header.Caption = "Old Price";
      e.Layout.Bands[1].Columns["BOMPrice"].Header.Caption = "BOM Price";

      //e.Layout.Bands[1].Columns["Approved"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
    }


    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_04_024 view = new viewCSD_04_024();
      Shared.Utility.WindowUtinity.ShowView(view, "New Trans", false, Shared.Utility.ViewState.MainWindow);
    }

    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ugdtData.Selected.Rows.Count > 0)
      {
        viewCSD_04_024 view = new viewCSD_04_024();
        UltraGridRow rowSelected = null;
        if (ugdtData.Selected.Rows[0].HasParent())
        {
          rowSelected = (ugdtData.Selected.Rows[0].ParentRow.HasParent()) ? ugdtData.Selected.Rows[0].ParentRow.ParentRow : ugdtData.Selected.Rows[0].ParentRow;
        }
        else
        {
          rowSelected = ugdtData.Selected.Rows[0];
        }

        long pid = DBConvert.ParseLong(rowSelected.Cells["Pid"].Value.ToString().Trim());
        view.viewTransactionPid = pid;
        Shared.Utility.WindowUtinity.ShowView(view, "Update Trans", false, Shared.Utility.ViewState.MainWindow);
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      if (ugdtData.Rows.Count > 0)
      {
        Utility.ExportToExcelWithDefaultPath(ugdtData, "List Change Item Price");
      }
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      bool flag = true;
      int count = 0;
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < ugdtData.Rows.Count; i++)
      {
        UltraGridRow row = ugdtData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          count = count + 1;
          long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDItemChangePrice_Delete", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            flag = false;
          }
        }
      }
      if (count > 0 && flag == true)
      {
        MessageBox.Show("Delete success!");
      }
      else
      {
        if (count <= 0)
        {
          MessageBox.Show("Please select row need delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
      }
      this.SearchData();
    }

    private void ugdtData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "Select":
          {
            if (DBConvert.ParseInt(e.Cell.Row.Cells["Status"].Value) > 1)
            {
              e.Cancel = true;
            }
          }
          break;
      }
    }
    private void btnUpdate_Click(object sender, EventArgs e)
    {
      viewCSD_04_024 view = new viewCSD_04_024();
      Shared.Utility.WindowUtinity.ShowView(view, "New Trans", false, Shared.Utility.ViewState.MainWindow);
    }
    #endregion event
  }
}
