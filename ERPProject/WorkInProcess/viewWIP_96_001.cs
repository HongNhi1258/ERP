/*
  Author: Nguyễn Ngọc Tiên
  Description: Màn hình đăng ký process info
  Create Day: 10/10/2014
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWIP_96_001 : MainUserControl
  {
    public viewWIP_96_001()
    {
      InitializeComponent();
    }

    #region Init
    private long proPid = long.MinValue;
    private int parentIndex = int.MinValue;
    private bool isDuplicateProcess = false;
    private IList listDeletedPid = new ArrayList();
    private string flagWorkArea = string.Empty;
    private int k = -1;
    private int flagdelete = 0;
    #endregion

    #region Function

    private void LoadDDTeam()
    {
      string commandText = "SELECT Pid, WorkAreaCode, WorkAreaCode + ' - '+ WorkAreaName WorkAreaName FROM TblWIPWorkArea WHERE DevisionCode = 'WIPCAR'";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDTeam.DataSource = dtSource;
      ultDDTeam.DisplayMember = "WorkAreaCode";
      ultDDTeam.ValueMember = "Pid";
      ultDDTeam.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDTeam.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    private void LoadDDTeamCode()
    {
      string cm = string.Format(@"SELECT Pid, WorkAreaCode WorkAreaName,  WorkAreaCode +' - ' + WorkAreaName Name
                                  FROM TblWIPWorkArea
                                  WHERE DevisionCode = 'WIPCAR'
                                  ORDER BY Pid");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      string[] hiden = new string[2];
      hiden[0] = "Pid";
      hiden[1] = "WorkAreaName";
      Utility.LoadUltraDropDown(ultDDTeamCode, dt, "WorkAreaName", "Name", hiden);
      ultDDTeamCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      try
      {
        DBParameter[] param = new DBParameter[1];
        param[0] = new DBParameter("@ProcessCode", DbType.String, txtProcess.Text.Trim());
        DataSet dtSource = DataBaseAccess.SearchStoreProcedure("spPLNProcessCarcassInfo_Select", param);
        dtSource.Relations.Add(new DataRelation("Parent_Child", dtSource.Tables[0].Columns["Pid"], dtSource.Tables[1].Columns["ProcessPid"], false));
        ultData.DataSource = dtSource;
        btnSave.Enabled = true;
        this.NeedToSave = false;
      }
      catch
      {
      }
    }

    private void CheckProcessDuplicate()
    {
      this.isDuplicateProcess = false;
      for (int k = 0; k < ultData.Rows.Count; k++)
      {
        for (int n = 0; n < ultData.Rows[k].ChildBands[0].Rows.Count; n++)
        {
          UltraGridRow rowcurent = ultData.Rows[k].ChildBands[0].Rows[n];
          rowcurent.CellAppearance.BackColor = Color.Empty;
        }
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowcurentA = ultData.Rows[i].ChildBands[0].Rows[j];
          string code = rowcurentA.Cells["WorkArea"].Value.ToString();
          for (int x = j + 1; x < ultData.Rows[i].ChildBands[0].Rows.Count; x++)
          {
            UltraGridRow rowcurentB = ultData.Rows[i].ChildBands[0].Rows[x];
            string codeB = rowcurentB.Cells["WorkArea"].Value.ToString();
            if (string.Compare(code, codeB) == 0)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              this.isDuplicateProcess = true;
            }
          }
        }
      }
    }

    private bool CheckValid()
    {
      DataTable dtCheck = ((DataSet)ultData.DataSource).Tables[0];
      //Check Duplicate
      if (this.isDuplicateProcess)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Data is Duplicate");
        this.SaveSuccess = false;
        return false;
      }
      foreach (DataRow row in dtCheck.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          string processCode = row["ProcessCode"].ToString().Trim();
          if (processCode.Length == 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", "Process Code");
            this.SaveSuccess = false;
            return false;
          }
          string NameEN = row["ProcessNameEN"].ToString().Trim();
          if (NameEN.Length == 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", "Process Name EN");
            this.SaveSuccess = false;
            return false;
          }
          string NameVN = row["ProcessNameVN"].ToString().Trim();
          if (NameVN.Length == 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", "Process Name VN");
            this.SaveSuccess = false;
            return false;
          }
          double setupTime = DBConvert.ParseDouble(row["SetupTime"].ToString());
          if (setupTime <= 0 && setupTime != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Setup Time");
            this.SaveSuccess = false;
            return false;
          }
          double Capacity = DBConvert.ParseDouble(row["Capacity"].ToString());
          if (Capacity <= 0 && Capacity != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Capacity");
            this.SaveSuccess = false;
            return false;
          }
          double operationTime = DBConvert.ParseDouble(row["ProcessTime"].ToString());
          if (operationTime <= 0 && operationTime != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Process Time");
            this.SaveSuccess = false;
            return false;
          }
          double leadTime1 = DBConvert.ParseDouble(row["LeadTime1"].ToString());
          if (leadTime1 <= 0 && leadTime1 != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Lead Time1");
            this.SaveSuccess = false;
            return false;
          }
          double leadTime2 = DBConvert.ParseDouble(row["LeadTime2"].ToString());
          if (leadTime2 <= 0 && leadTime2 != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Lead Time2");
            this.SaveSuccess = false;
            return false;
          }
          double leadTime3 = DBConvert.ParseDouble(row["LeadTime3"].ToString());
          if (leadTime3 <= 0 && leadTime3 != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Lead Time3");
            this.SaveSuccess = false;
            return false;
          }
          double leadTime4 = DBConvert.ParseDouble(row["LeadTime4"].ToString());
          if (leadTime4 <= 0 && leadTime4 != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Lead Time4");
            this.SaveSuccess = false;
            return false;
          }
          string notation = row["Notation"].ToString();
          DataRow[] rows = dtCheck.Select(string.Format("Notation = '{0}'", notation));
          if (rows.Length > 1)
          {
            WindowUtinity.ShowMessageError("ERR0013", "Notation");
            this.SaveSuccess = false;
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Process Information
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool success = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["RowState"].Value.ToString()) == 1)
        {
          DBParameter[] input = new DBParameter[14];
          long ProcessCodePid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          string Code = row.Cells["ProcessCode"].Value.ToString().Trim();
          string NameEN = row.Cells["ProcessNameEN"].Value.ToString().Trim();
          string NameVN = row.Cells["ProcessNameVN"].Value.ToString().Trim();
          double Capacity = DBConvert.ParseDouble(row.Cells["Capacity"].Value.ToString());
          double Setup = DBConvert.ParseDouble(row.Cells["SetupTime"].Value.ToString());
          double Operation = DBConvert.ParseDouble(row.Cells["ProcessTime"].Value.ToString());
          double Lead1 = DBConvert.ParseDouble(row.Cells["LeadTime1"].Value.ToString());
          double Lead2 = DBConvert.ParseDouble(row.Cells["LeadTime2"].Value.ToString());
          double Lead3 = DBConvert.ParseDouble(row.Cells["LeadTime3"].Value.ToString());
          double Lead4 = DBConvert.ParseDouble(row.Cells["LeadTime4"].Value.ToString());
          string Remark = row.Cells["Remark"].Value.ToString().Trim();
          string Notation = row.Cells["Notation"].Value.ToString().Trim();
          long TeamCodeDefaultPid = DBConvert.ParseLong(row.Cells["TeamCodePid"].Value.ToString());
          if (ProcessCodePid > 0)
          {
            input[0] = new DBParameter("@ProcessCodePid", DbType.Int64, ProcessCodePid);
          }
          if (Code.Trim().Length > 0)
          {
            input[1] = new DBParameter("@ProcessCode", DbType.String, 16, Code);
          }
          if (NameEN.Trim().Length > 0)
          {
            input[2] = new DBParameter("@ProcessNameEN", DbType.AnsiString, 128, NameEN);
          }
          if (NameVN.Trim().Length > 0)
          {
            input[3] = new DBParameter("@ProcessNameVN", DbType.String, 128, NameVN);
          }
          if (Capacity != double.MinValue)
          {
            input[4] = new DBParameter("@Capacity", DbType.Double, Capacity);
          }
          if (Setup != double.MinValue)
          {
            input[5] = new DBParameter("@SetupTime", DbType.Double, Setup);
          }
          if (Operation != double.MinValue)
          {
            input[6] = new DBParameter("@ProcessTime", DbType.Double, Operation);
          }
          if (Lead1 != double.MinValue)
          {
            input[7] = new DBParameter("@LeadTime1", DbType.Double, Lead1);
          }
          if (Lead2 != double.MinValue)
          {
            input[8] = new DBParameter("@LeadTime2", DbType.Double, Lead2);
          }
          if (Lead3 != double.MinValue)
          {
            input[9] = new DBParameter("@LeadTime3", DbType.Double, Lead3);
          }
          if (Lead4 != double.MinValue)
          {
            input[10] = new DBParameter("@LeadTime4", DbType.Double, Lead4);
          }
          if (Remark.Length > 0)
          {
            input[11] = new DBParameter("@Remark", DbType.String, 128, Remark);
          }
          if (Notation.Length > 0)
          {
            input[12] = new DBParameter("@Notation", DbType.String, 128, Notation);
          }
          if (TeamCodeDefaultPid != double.MinValue)
          {
            input[13] = new DBParameter("@TeamCodeDefaultPid", DbType.Int64, TeamCodeDefaultPid);
          }

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spPLNProcessCarcassInfo_Edit", input, output);
          long processPid = DBConvert.ParseLong(output[0].Value.ToString());
          if (processPid <= 0)
          {
            success = false;
          }
          else
          {
            success = this.SaveChild(row, processPid);
          }
        }
      }
      return success;
    }

    /// <summary>
    /// Save Team Code
    /// </summary>
    /// <param name="row"></param>
    /// <param name="processPid"></param>
    /// <returns></returns>
    private bool SaveChild(UltraGridRow rowParent, long processPid)
    {
      for (int i = 0; i < rowParent.ChildBands[0].Rows.Count; i++)
      {
        UltraGridRow row = rowParent.ChildBands[0].Rows[i];
        if (DBConvert.ParseInt(row.Cells["RowState"].Value.ToString()) == 1)
        {
          DBParameter[] input = new DBParameter[3];
          if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          }
          input[1] = new DBParameter("@ProcessPid", DbType.Int64, processPid);
          if (DBConvert.ParseLong(row.Cells["TeamCodePid"].Value.ToString()) != long.MinValue)
          {
            input[2] = new DBParameter("@TeamCodePid", DbType.Int64, DBConvert.ParseLong(row.Cells["TeamCodePid"].Value.ToString()));
          }

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spWIProcessTeam_Edit", input, output);
          if (output == null || DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    #endregion

    #region Events

    private void ViewWIP_96_001_Load(object sender, EventArgs e)
    {
      btnSave.Enabled = false;
      this.LoadDDTeam();
      this.LoadDDTeamCode();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.True;

      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ViewStyleBand = ViewStyleBand.Horizontal;

      e.Layout.Bands[0].ColHeaderLines = 2;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RowState"].Hidden = true;
      e.Layout.Bands[0].Columns["TeamCodePid"].ValueList = ultDDTeam;
      e.Layout.Bands[0].Columns["TeamCode"].Header.Caption = "Other Team";

      e.Layout.Bands[1].Columns["ProcessPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["TeamCodePid"].Hidden = true;
      e.Layout.Bands[1].Columns["RowState"].Hidden = true;

      e.Layout.Bands[1].Columns["WorkArea"].ValueList = ultDDTeamCode;
      e.Layout.Bands[1].Columns["WorkArea"].Header.Caption = "Other Team";

      e.Layout.Bands[0].Columns["ProcessCode"].Header.Caption = "Process\nCode";
      e.Layout.Bands[0].Columns["ProcessCode"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["ProcessCode"].MinWidth = 70;

      e.Layout.Bands[0].Columns["ProcessNameEN"].Header.Caption = "English\nName";
      e.Layout.Bands[0].Columns["ProcessNameEN"].MaxWidth = 130;
      e.Layout.Bands[0].Columns["ProcessNameEN"].MinWidth = 130;

      e.Layout.Bands[0].Columns["ProcessNameVN"].Header.Caption = "Vietnamese\nName";

      e.Layout.Bands[0].Columns["SetupTime"].Header.Caption = "Setup\nTime";
      e.Layout.Bands[0].Columns["SetupTime"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["SetupTime"].MinWidth = 50;
      e.Layout.Bands[0].Columns["SetupTime"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["Capacity"].Header.Caption = "Capa.";
      e.Layout.Bands[0].Columns["Capacity"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Capacity"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Capacity"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["ProcessTime"].Header.Caption = "Process\nTime";
      e.Layout.Bands[0].Columns["ProcessTime"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["ProcessTime"].MinWidth = 50;
      e.Layout.Bands[0].Columns["ProcessTime"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["LeadTime1"].Header.Caption = "Lead Time\n<=2pcs";
      e.Layout.Bands[0].Columns["LeadTime1"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LeadTime1"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["LeadTime1"].MinWidth = 70;

      e.Layout.Bands[0].Columns["LeadTime2"].Header.Caption = "Lead Time\n<= 6pcs";
      e.Layout.Bands[0].Columns["LeadTime2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LeadTime2"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["LeadTime2"].MinWidth = 70;

      e.Layout.Bands[0].Columns["LeadTime3"].Header.Caption = "Lead Time\n<= 12pcs";
      e.Layout.Bands[0].Columns["LeadTime3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LeadTime3"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["LeadTime3"].MinWidth = 70;

      e.Layout.Bands[0].Columns["LeadTime4"].Header.Caption = "Lead Time\n> 12pcs";
      e.Layout.Bands[0].Columns["LeadTime4"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LeadTime4"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["LeadTime4"].MinWidth = 70;

      e.Layout.Bands[0].Columns["TeamCodePid"].Header.Caption = "Team Default";
      e.Layout.Bands[0].Columns["TeamCodePid"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["TeamCodePid"].MinWidth = 70;

      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Remark";
      e.Layout.Bands[0].Columns["Remark"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Remark"].MinWidth = 70;

      e.Layout.Bands[0].Columns["Notation"].Header.Caption = "Notation";
      e.Layout.Bands[0].Columns["Notation"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Notation"].MinWidth = 50;
    }

    private void ultData_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      string values = e.NewValue.ToString();
      switch (colName)
      {
        case "LeadTime1":
          if (DBConvert.ParseDouble(values) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "LeadTime1");
            e.Cancel = true;
          }
          break;
        case "LeadTime2":
          if (DBConvert.ParseDouble(values) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "LeadTime2");
            e.Cancel = true;
          }
          break;
        case "LeadTime3":
          if (DBConvert.ParseDouble(values) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "LeadTime3");
            e.Cancel = true;
          }
          break;
        case "LeadTime4":
          if (DBConvert.ParseDouble(values) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "LeadTime4");
            e.Cancel = true;
          }
          break;

        //case "Notation":
        //  string cmtext = string.Format("SELECT [dbo].[FPLNCheckProcessNotation]('{0}')", values.Trim());
        //  int resultno = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(cmtext).ToString());
        //  if (resultno != 0)
        //  {
        //    WindowUtinity.ShowMessageError("ERR0028", "Notation");
        //    e.Cancel = true;
        //  }
        //  break;
        default:
          break;
      }
    }

    private void btnSearch_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }

    private void ultData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      this.NeedToSave = false;
      e.Cell.Row.Cells["RowState"].Value = 1;

      switch (colName)
      {
        case "processcode":
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) < 0)
            {
              e.Cell.Row.Cells["Pid"].Value = k--;
            }
            break;
          }
        case "workarea":
          {
            if (ultDDTeamCode.SelectedRow != null)
            {
              e.Cell.Row.Cells["TeamCodePid"].Value = ultDDTeamCode.SelectedRow.Cells["Pid"].Value.ToString();
              e.Cell.Row.Cells["RowState"].Value = 1;
            }
            else
            {
              e.Cell.Row.Cells["TeamCodePid"].Value = DBNull.Value;
              e.Cell.Row.Cells["ProcessPid"].Value = DBNull.Value;
              e.Cell.Row.Cells["RowState"].Value = 0;
            }

            long pidParent = DBConvert.ParseLong(e.Cell.Row.ParentRow.Cells["Pid"].Value);
            string option = e.Cell.Row.ParentRow.Cells["TeamCode"].Value.ToString();
            option = string.Empty;
            RowsCollection rows = e.Cell.Row.ParentRow.ChildBands[0].Rows;
            for (int i = 0; i < rows.Count; i++)
            {
              if (option.Length == 0)
              {
                option = rows[i].Cells["WorkArea"].Value.ToString();
              }
              else
              {
                option += "," + rows[i].Cells["WorkArea"].Value.ToString();
              }
            }
            e.Cell.Row.ParentRow.Cells["TeamCode"].Value = option;
            this.CheckProcessDuplicate();
          }
          break;
        default:
          break;
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      //this.SaveData();

      if (this.CheckValid())
      {
        bool success = this.SaveData();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }

        // Search Data 
        this.Search();
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void txtProcess_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }
    #endregion

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (this.parentIndex >= 0)
      {
        string option = string.Empty;
        DataRow[] dt = (((DataSet)ultData.DataSource).Tables[1]).Select(string.Format("ProcessPid = {0}", this.proPid));
        foreach (DataRow row in dt)
        {
          if (option.Length == 0)
          {
            option = row["WorkArea"].ToString();
          }
          else if (option.Length > 0)
          {
            option += "," + row["WorkArea"].ToString();
          }
        }
        ultData.Rows[this.parentIndex].Cells["TeamCode"].Value = option;
      }
      this.CheckProcessDuplicate();
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;

      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {

        e.Cancel = true;
        return;
      }
      else
        this.flagdelete = 1;
      foreach (UltraGridRow row in e.Rows)
      {

        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
        this.SaveSuccess = false;
      }

      if (e.Rows[0].ParentRow != null)
      {
        this.parentIndex = e.Rows[0].ParentRow.Index;
        this.flagWorkArea = e.Rows[0].ParentRow.Cells["TeamCode"].Value.ToString();
        this.proPid = DBConvert.ParseLong(e.Rows[0].ParentRow.Cells["Pid"].Value.ToString());
      }
      else
      {
        this.parentIndex = int.MinValue;
      }
      // Check Duplicate
      this.CheckProcessDuplicate();
    }

    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        Utility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Process Info", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "Process Info";
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

    private void btnExport_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }
  }
}
