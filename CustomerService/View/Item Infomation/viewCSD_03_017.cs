/*
  Author      : Nguyen Chi Cuong
  Date        : 25/07/2015
  Description : Search Transaction SaleCode
  Standard Form: view_SearchInfo.cs
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using System.Diagnostics;


namespace DaiCo.CustomerService
{
  public partial class viewCSD_03_017 : MainUserControl
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
      int paramNumber = 9;
      string storeName = string.Empty;
      storeName ="spCSDTransactionSaleCode_Select";
      DBParameter[] input = new DBParameter[paramNumber];

      if (txtTransactionCode.Text.Trim().Length > 0)
      {
        input[0] = new DBParameter("@TransactionCode", DbType.AnsiString, 50, txtTransactionCode.Text.Trim());
      }

      if (ultCBCreateBy.Value != null)
      {
        input[1] = new DBParameter("@CreateBy", DbType.AnsiString, 50, ultCBCreateBy.Value.ToString());
      }

      if (ultCBApprovedBy.Value != null)
      {
        input[2] = new DBParameter("@ApprovedBy", DbType.AnsiString, 50, ultCBApprovedBy.Value.ToString());
      }

      if (ultDTCreateFrom.Value != null)
      {
        input[3] = new DBParameter("@CreateDateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultDTCreateFrom.Value));
      }

      if (ultDTCreateTo.Value != null)
      {
        input[4] = new DBParameter("@CreateDateTo", DbType.DateTime, DBConvert.ParseDateTime(ultDTCreateTo.Value));
      }

      if (ultraCBStatus.Value != null)
      {
        input[5] = new DBParameter("@Status", DbType.AnsiString, 10,ultraCBStatus.Value.ToString());
      }

      if (txtItemCode.Text.Trim().Length > 0)
      {
        input[6] = new DBParameter("@ItemCode", DbType.AnsiString ,20, txtItemCode.Text.Trim());
      }

      if (txtOldSaleCode.Text.Trim().Length > 0)
      {
        input[7] = new DBParameter("@OldSaleCode", DbType.AnsiString, 20, txtOldSaleCode.Text.Trim());
      }

      if (txtNewSaleCode.Text.Trim().Length > 0)
      {
        input[8] = new DBParameter("@NewSaleCode", DbType.AnsiString, 20, txtNewSaleCode.Text.Trim());
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, input);

      DataSet dsList = this.ListTransaction();
      dsList.Tables["TblInf"].Merge(dsSource.Tables[0]);
      dsList.Tables["TblTranDetail"].Merge(dsSource.Tables[1]);

      ultData.DataSource = dsList;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        
        string type = ultData.Rows[i].Cells["Status"].Value.ToString();
        if (type == "New")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (type == "Waiting For Approved")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (type == "Approved")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightSkyBlue;
        }
        
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          ultData.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Plum;
        }
        ultData.Rows[i].Expanded = false;
      }

      lbCount.Text = string.Format("Count: {0}", ultData.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
    }

    private DataSet ListTransaction()
    {
      DataSet ds = new DataSet();

      // Tran Information
      DataTable taParent = new DataTable("TblInf");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("TransactionCode", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("ApprovedBy", typeof(System.String));
      taParent.Columns.Add("ApprovedDate", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      ds.Tables.Add(taParent);

      // Tran Detail
      DataTable taChild = new DataTable("TblTranDetail");
      taChild.Columns.Add("TranDetailPid", typeof(System.Int64));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Name", typeof(System.String));
      taChild.Columns.Add("OldSaleCode", typeof(System.String));
      taChild.Columns.Add("NewSaleCode", typeof(System.String));
      taChild.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblInf", taParent.Columns["Pid"], taChild.Columns["TranDetailPid"]));
      return ds;
    }


    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      ultCBCreateBy.Value = null;
      txtTransactionCode.Text = string.Empty;
      ultCBApprovedBy.Value = null;
      txtItemCode.Text = string.Empty;
      txtOldSaleCode.Text = string.Empty;
      txtNewSaleCode.Text = string.Empty;
      ultDTCreateFrom.Value = null;
      ultDTCreateTo.Value = null;
      ultraCBStatus.Value = null;
    }
    #endregion function

    #region event
    public viewCSD_03_017()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_03_017_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      LoadStatus();
      foreach (Control ctr in groupBoxSearch.Controls)
      {
        ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      }
      this.LoadEmployeeCreateBy(ultCBCreateBy);
      this.LoadEmployeeApprovedBy(ultCBApprovedBy);

      if (btnperApproved.Visible == true)
      {
        isApproved = true; 
        btnperApproved.Visible = false;
      }
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
      string commandText    =  "SELECT 0 'Code', 'New' 'Display' ";
                commandText +=  "UNION " ;
                commandText +=  "SELECT 1 , 'Waiting For Approved'";
                commandText +=  "UNION ";
                commandText += "SELECT 2 , 'Approved'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultraCBStatus.DataSource = dt;
      ultraCBStatus.DisplayMember = "Display";
      ultraCBStatus.ValueMember = "Code";
      ultraCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBStatus.DisplayLayout.Bands[0].Columns["Display"].Width = 250;
      ultraCBStatus.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    private void LoadEmployeeCreateBy(UltraCombo UltraCombo)
    {
      string commandText = string.Format("SELECT Pid, CAST(Pid as varchar) + ' - ' + EmpName Description FROM VHRMEmployee WHERE Resigned = 0 AND Pid > 0 ORDER BY Pid");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);

      ultCBCreateBy.DataSource = dtSource;
      ultCBCreateBy.ValueMember = "Pid";
      ultCBCreateBy.DisplayMember = "Description";
      ultCBCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBCreateBy.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBCreateBy.DisplayLayout.Bands[0].Columns["Description"].Width = 300;
    }

    private void LoadEmployeeApprovedBy(UltraCombo UltraCombo)
    {
      string commandText = string.Format("SELECT Pid, CAST(Pid as varchar) + ' - ' + EmpName Description FROM VHRMEmployee WHERE Resigned = 0 AND Pid > 0 ORDER BY Pid");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);

      ultCBApprovedBy.DataSource = dtSource;
      ultCBApprovedBy.ValueMember = "Pid";
      ultCBApprovedBy.DisplayMember = "Description";
      ultCBApprovedBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBApprovedBy.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBApprovedBy.DisplayLayout.Bands[0].Columns["Description"].Width = 300;
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["TranDetailPid"].Hidden = true;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {        
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      for (int i = 1; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[1].Columns["Approved"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
    }
    #endregion event

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_03_018 view = new viewCSD_03_018();
      view.isperApproved = isApproved;
      Shared.Utility.WindowUtinity.ShowView(view, "New Trans", false, Shared.Utility.ViewState.MainWindow);
    }

    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0)
      {
        viewCSD_03_018 view = new viewCSD_03_018();
        UltraGridRow rowSelected = null;
        if (ultData.Selected.Rows[0].HasParent())
        {
          rowSelected = (ultData.Selected.Rows[0].ParentRow.HasParent()) ? ultData.Selected.Rows[0].ParentRow.ParentRow : ultData.Selected.Rows[0].ParentRow;
        }
        else
        {
          rowSelected = ultData.Selected.Rows[0];
        }

        long pid = DBConvert.ParseLong(rowSelected.Cells["Pid"].Value.ToString().Trim());
        view.viewTransactionPid = pid;
        view.isperApproved = isApproved;
        Shared.Utility.WindowUtinity.ShowView(view, "Update Trans", false, Shared.Utility.ViewState.MainWindow);
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Update SaleCode", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "Update SaleCode";
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

  }
}
