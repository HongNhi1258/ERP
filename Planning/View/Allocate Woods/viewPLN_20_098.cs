/*
  Author      : 
  Date        : 14/03/2013
  Description : List Supplement For Woods
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_20_098 : MainUserControl
  {
    #region Init Form
    private string STT_NON = "";
    private string STT_NON_CONFIRM = "NON Confirmed";
    private string STT_WAITING_FOR_ISSUE = "Waiting For Issue";
    private string STT_ISSUED = "Issued";

    public viewPLN_20_098()
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
      // Load Status
      this.LoadStatus();

      // Load Create By
      this.LoadCreateBy();
    }

    /// <summary>
    /// Load Status
    /// </summary>
    private void LoadStatus()
    {
      DataTable dtStatus = new DataTable();
      dtStatus.Columns.Add("Status", typeof(System.String));

      DataRow row = dtStatus.NewRow();
      row["Status"] = this.STT_NON;
      dtStatus.Rows.Add(row);

      row = dtStatus.NewRow();
      row["Status"] = this.STT_NON_CONFIRM;
      dtStatus.Rows.Add(row);

      row = dtStatus.NewRow();
      row["Status"] = this.STT_WAITING_FOR_ISSUE;
      dtStatus.Rows.Add(row);

      row = dtStatus.NewRow();
      row["Status"] = this.STT_ISSUED;
      dtStatus.Rows.Add(row);

      ultStatus.DataSource = dtStatus;
      ultStatus.ValueMember = "Status";
      ultStatus.DisplayMember = "Status";
      ultStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultStatus.DisplayLayout.Bands[0].Columns["Status"].Width = 200;
    }

    /// <summary>
    /// Load Create By
    /// </summary>
    private void LoadCreateBy()
    {
      string commnadText = string.Empty;
      commnadText += " SELECT ID_NhanVien , CAST(ID_NhanVien AS VARCHAR) + ' - ' + HoNV + ' ' + TenNV Name ";
      commnadText += " FROM VHRNhanVien ";

      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commnadText);

      ultCreateBy.DataSource = dtCheck;
      ultCreateBy.ValueMember = "ID_NhanVien";
      ultCreateBy.DisplayMember = "Name";
      ultCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCreateBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
      ultCreateBy.DisplayLayout.Bands[0].Columns["Name"].Width = 400;
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

      DBParameter[] inputParam = new DBParameter[8];

      string text = txtSuppNo.Text.Trim();
      // Supplement No
      if (text.Length > 0)
      {
        inputParam[0] = new DBParameter("@SupplementNo", DbType.AnsiString, 16, text);
      }

      // Status
      if (this.ultStatus.Value != null)
      {
        if (string.Compare(this.ultStatus.Value.ToString(), this.STT_NON_CONFIRM, true) == 0)
        {
          inputParam[1] = new DBParameter("@Status", DbType.Int32, 0);
        }
        else if (string.Compare(this.ultStatus.Value.ToString(), this.STT_WAITING_FOR_ISSUE, true) == 0)
        {
          inputParam[1] = new DBParameter("@Status", DbType.Int32, 1);
        }
        else if (string.Compare(this.ultStatus.Value.ToString(), this.STT_ISSUED, true) == 0)
        {
          inputParam[1] = new DBParameter("@Status", DbType.Int32, 2);
        }
      }

      // WO
      text = txtWO.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[2] = new DBParameter("@WO", DbType.AnsiString, 100, text);
      }

      // Group
      text = txtGroup.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[3] = new DBParameter("@Group", DbType.AnsiString, 16, text);
      }

      if (this.ultCreateBy.Value != null
          && DBConvert.ParseInt(this.ultCreateBy.Value.ToString()) != int.MinValue)
      {
        inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(this.ultCreateBy.Value.ToString()));
      }

      // Date From 
      if (dateFrom != DateTime.MinValue)
      {
        inputParam[5] = new DBParameter("@CreateDateFrom", DbType.DateTime, dateFrom);
      }

      // Date To
      if (dateTo != DateTime.MinValue)
      {
        inputParam[6] = new DBParameter("@CreateDateTo", DbType.DateTime, dateTo);
      }

      // Item Code
      text = txtItemCode.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[7] = new DBParameter("@ItemCode", DbType.String, text);
      }

      // Carcass Code
      text = txtItemCode.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[8] = new DBParameter("@CarcassCode", DbType.String, text);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNWoodsListSupplement", inputParam);
      DataSet dsList = this.ListWoodsSupplement();
      // Master
      dsList.Tables["TblMaster"].Merge(ds.Tables[0]);
      // Detail
      dsList.Tables["TblDetail"].Merge(ds.Tables[1]);

      ultData.DataSource = dsList;
    }
    #endregion Process

    #region Function
    /// <summary>
    /// Dataset For List Of Supplement
    /// </summary>
    /// <returns></returns>
    private DataSet ListWoodsSupplement()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblMaster");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("SupplementNo", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Select", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblDetail");
      taChild.Columns.Add("SupplementPid", typeof(System.Int64));
      taChild.Columns.Add("Code", typeof(System.String));
      taChild.Columns.Add("Name", typeof(System.String));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Issued", typeof(System.Double));
      taChild.Columns.Add("Supplement", typeof(System.Double));
      taChild.Columns.Add("Reason", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblMaster_TblDetail", taParent.Columns["Pid"], taChild.Columns["SupplementPid"], false));
      return ds;
    }

    private void ExportData()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Deadline", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "Deadline";
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
    /// btnSearch_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;

      // Search
      this.Search();

      this.btnSearch.Enabled = true;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// btnNew_Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_20_099 view = new viewPLN_20_099();
      Shared.Utility.WindowUtinity.ShowView(view, "SUPPLEMENT INFORMATION", false, Shared.Utility.ViewState.MainWindow);
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
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["SupplementPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Code"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["WO"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Issued"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Supplement"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Reason"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["WO"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Issued"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Supplement"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["SupplementNo"].Header.Caption = "Supp. No";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
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
        viewPLN_20_099 view = new viewPLN_20_099();
        view.suppPid = suppPid;
        view.supplementNo = suppNo;
        WindowUtinity.ShowView(view, "SUPPLEMENT INFORMATION", true, ViewState.MainWindow);
      }
    }

    private void btnFinish_Click(object sender, EventArgs e)
    {
      int count = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          count = count + 1;
          if (count > 1)
          {
            WindowUtinity.ShowMessageError("ERR0114", "row");
            return;
          }
        }
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          if (String.Compare(row.Cells["Status"].Value.ToString(), "Issued", true) != 0)
          {
            DBParameter[] input = new DBParameter[2];
            input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            input[1] = new DBParameter("@Status", DbType.Int32, 2);

            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spPLNSupplementStatus_Update", input, output);

            if (DBConvert.ParseLong(output[0].Value.ToString()) == 1)
            {
              WindowUtinity.ShowMessageSuccess("MSG0051", row.Cells["SupplementNo"].Value.ToString());
            }
            else if (DBConvert.ParseLong(output[0].Value.ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERRO122", row.Cells["SupplementNo"].Value.ToString());
            }
            else if (DBConvert.ParseLong(output[0].Value.ToString()) == -1)
            {
              WindowUtinity.ShowMessageError("ERRO121", row.Cells["SupplementNo"].Value.ToString(), "is waiting for issuing");
            }
          }
          else
          {
            WindowUtinity.ShowMessageError("ERRO121", row.Cells["SupplementNo"].Value.ToString(), "was finished");
          }
        }
      }

      this.Search();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportData();
    }
    #endregion Event
  }
}
