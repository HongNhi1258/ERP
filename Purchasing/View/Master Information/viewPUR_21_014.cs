/*
  Author      : Ha Anh
  Date        : 29/03/2014
  Description : Define Project Code
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
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_21_014 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private IList listDeletedPid = new ArrayList();

    #endregion Field

    #region Init
    public viewPUR_21_014()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_21_014_Load(object sender, EventArgs e)
    {
      this.chkActive.Checked = true;
      this.ultDTCreateFrom.Value = DBNull.Value;
      this.ultDTCreateTo.Value = DBNull.Value;
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[6];
      if (ultCBDepartment.Value != null)
      {
        input[0] = new DBParameter("@Department", DbType.AnsiString, 50, ultCBDepartment.Value.ToString());
      }

      if (txtProjectCode.Text.Length > 0)
      {
        input[1] = new DBParameter("@ProjectCode", DbType.AnsiString, 32, "%" + txtProjectCode.Text.Trim() + "%");
      }

      if (ultDTCreateFrom.Value != null)
      {
        input[2] = new DBParameter("@CreateDateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultDTCreateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
      }

      if (ultDTCreateTo.Value != null)
      {
        input[3] = new DBParameter("@CreateDateTo", DbType.DateTime, DBConvert.ParseDateTime(ultDTCreateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME).AddDays(1));
      }

      if (chkActive.Checked == true)
      {
        input[4] = new DBParameter("@Active", DbType.Int32, 1);
      }
      else
      {
        input[4] = new DBParameter("@Active", DbType.Int32, 0);
      }

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPURPRProjectCode_Search", 600, input);
      if (dt != null)
      {
        ultData.DataSource = dt;
      }
      // Enable button search
      btnSearch.Enabled = true;
      this.NeedToSave = false;
    }

    /// <summary>
    /// Load All Data For Search Information
    /// </summary>
    private void LoadData()
    {
      this.LoadDepartment();
    }

    private void LoadDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBDepartment.DataSource = dtSource;
      ultCBDepartment.DisplayMember = "Name";
      ultCBDepartment.ValueMember = "Department";
      ultCBDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;

      ultDDDepartment.DataSource = dtSource;
      ultDDDepartment.DisplayMember = "Name";
      ultDDDepartment.ValueMember = "Department";
      ultDDDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultDDDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
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
    #endregion Function

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnTop;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Finished"].Hidden = true;

      e.Layout.Bands[0].Columns["BudgetAmount"].Header.Caption = "Budget(USD)";
      e.Layout.Bands[0].Columns["BudgetAmount"].Format = "###,##0";
      e.Layout.Bands[0].Columns["Actual"].Format = "###,##0.00";

      e.Layout.Bands[0].Columns["Finished"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Actual"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Department"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["ProjectCode"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["BudgetAmount"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Description"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["Department"].ValueList = ultDDDepartment;

      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      e.Layout.Bands[0].Columns["UpdateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["UpdateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["ProjectCode"].Value.ToString().Trim() == string.Empty ||
            ultData.Rows[i].Cells["Department"].Value.ToString() == string.Empty ||
            ultData.Rows[i].Cells["BudgetAmount"].Value.ToString() == string.Empty
        )
        {
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
          ultData.Rows[i].Selected = true;
          ultData.ActiveRowScrollRegion.ScrollRowIntoView(ultData.Rows[i]);

          errorMessage = "Data Input Error";
          return false;
        }
      }
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;

        DataTable dtDetail = (DataTable)ultData.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[7];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());

            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@Deparment", DbType.AnsiString, 50, row["Department"].ToString());
            inputParam[2] = new DBParameter("@ProjectCode", DbType.AnsiString, 32, row["ProjectCode"].ToString());
            inputParam[3] = new DBParameter("@BudgetAmount", DbType.Double, DBConvert.ParseDouble(row["BudgetAmount"].ToString()));
            inputParam[4] = new DBParameter("@Description", DbType.AnsiString, 1024, row["Description"].ToString());
            inputParam[5] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            if (DBConvert.ParseInt(row["Finished"].ToString()) > 0)
            {
              inputParam[6] = new DBParameter("@Finished", DbType.Int32, DBConvert.ParseInt(row["Finished"].ToString()));
            }
            else
            {
              inputParam[6] = new DBParameter("@Finished", DbType.Int32, 0);
            }

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spPURPRProjectCode_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.Search();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    private void ultData_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells[""].Value.ToString());
        if (pid != long.MinValue)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0093"), "");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      int count = 0;
      if (colName == "projectcode")
      {
        if (e.Cell.Row.Cells["Department"].Text.Length == 0)
        {
          WindowUtinity.ShowMessageErrorFromText("Please input Department First!");
          e.Cancel = true;
          return;
        }

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (e.Cell.Row.Cells["ProjectCode"].Text.Trim() == ultData.Rows[i].Cells["ProjectCode"].Text.Trim() && e.Cell.Row.Cells["Department"].Text.Trim() == ultData.Rows[i].Cells["Department"].Text.Trim())
          {
            count++;
            if (count == 2)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Project Code");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
              return;
            }
          }
        }

        string commandText = string.Format(@"SELECT *	FROM	TblPURPRProjectCode WHERE ProjectCode = '{0}' AND Department = '{1}'", e.Cell.Row.Cells["ProjectCode"].Text, e.Cell.Row.Cells["Department"].Text);
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "ProjectCode");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }
      }

      if (colName == "department")
      {
        bool check = this.CheckMemberDownDrop(e.Cell.Row.Cells["Department"].Text.Trim(), "Name", (DataTable)ultDDDepartment.DataSource);
        if (check == false)
        {
          string message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Department");
          MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          e.Cancel = true;
          return;
        }

        if (e.Cell.Row.Cells["ProjectCode"].Text.Trim().Length > 0)
        {
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            if (e.Cell.Row.Cells["ProjectCode"].Text.Trim() == ultData.Rows[i].Cells["ProjectCode"].Text.Trim() && e.Cell.Row.Cells["Department"].Text.Trim() == ultData.Rows[i].Cells["Department"].Text.Trim())
            {
              count++;
              if (count == 2)
              {
                string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Project Code");
                WindowUtinity.ShowMessageErrorFromText(message);
                e.Cancel = true;
                return;
              }
            }
          }
        }
      }

      if (colName == "budgetamount")
      {
        if ((DBConvert.ParseDouble(e.Cell.Row.Cells["BudgetAmount"].Text) <= 0) ||
           (DBConvert.ParseDouble(e.Cell.Row.Cells["BudgetAmount"].Text) <= DBConvert.ParseDouble(e.Cell.Row.Cells["Actual"].Text)))
        {
          string message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Budget Amount");
          MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          e.Cancel = true;
          return;
        }
      }
    }

    private bool CheckMemberDownDrop(string inputvalue, string colName, DataTable dt)
    {
      bool check = false;
      if (dt != null)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i][colName].ToString() == inputvalue)
          {
            check = true;
            break;
          }
        }
      }
      return check;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      switch (colName)
      {
        //case "group":
        //  {
        //    e.Cell.Row.Cells["GroupPid"].Value = DBConvert.ParseLong(e.Cell.Row.Cells["Group"].Value.ToString());
        //  }
        //  break;
      }
      this.SetNeedToSave();
    }

    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary> 
    /// Save data before close
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      ultCBDepartment.Text = string.Empty;
      txtProjectCode.Text = string.Empty;
      ultDTCreateFrom.Value = null;
      ultDTCreateTo.Value = null;
      chkActive.Checked = true;
    }

    private void btnDeactive_Click(object sender, EventArgs e)
    {
      DataTable dtDetail = (DataTable)ultData.DataSource;
      if (dtDetail != null)
      {
        for (int i = 0; i < dtDetail.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["Select"].Value.ToString()) == 1)
          {
            DataRow row = dtDetail.Rows[i];

            DBParameter[] inputParam = new DBParameter[8];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());

            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@Deparment", DbType.AnsiString, 50, row["Department"].ToString());
            inputParam[2] = new DBParameter("@ProjectCode", DbType.AnsiString, 32, row["ProjectCode"].ToString());
            inputParam[3] = new DBParameter("@BudgetAmount", DbType.Double, DBConvert.ParseDouble(row["BudgetAmount"].ToString()));
            inputParam[4] = new DBParameter("@Description", DbType.AnsiString, 1024, row["Description"].ToString());
            inputParam[5] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            inputParam[6] = new DBParameter("@Status", DbType.Int32, 0); //Deactive = 0, Active , Delete = 2
            inputParam[7] = new DBParameter("@Finished", DbType.Int32, DBConvert.ParseInt(row["Finished"].ToString()));

            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spPURPRProjectCode_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              WindowUtinity.ShowMessageError("ERR0002");
              this.SaveSuccess = false;
              return;
            }
          }
        }

        WindowUtinity.ShowMessageSuccess("MSG0004");

        this.Search();
      }
    }

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      switch (colName)
      {
        case "department":
          {
            if (DBConvert.ParseDouble(e.Cell.Row.Cells["Actual"].Text) > 0)
            {
              e.Cancel = true;
            }
          }
          break;

        case "projectcode":
          {
            if (DBConvert.ParseDouble(e.Cell.Row.Cells["Actual"].Text) > 0)
            {
              e.Cancel = true;
            }
          }
          break;
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        ControlUtility.ExportToExcelWithDefaultPath(ultData, "PROJECT CODE");
        //Excel.Workbook xlBook;

        //ultData.Rows.Band.Columns["Select"].Hidden = true;
        //ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "PROJECT CODE", 6);
        //ultData.Rows.Band.Columns["Select"].Hidden = false;

        //string filePath = xlBook.FullName;
        //Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        //xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        //Excel.Range r = xlSheet.get_Range("A1", "A1");

        //xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        //xlSheet.Cells[3, 1] = "PROJECT CODE";
        //r = xlSheet.get_Range("A3", "A3");
        //r.Font.Bold = true;
        //r.Font.Size = 14;
        //r.EntireRow.RowHeight = 20;

        //xlSheet.Cells[4, 1] = "Date: ";
        //r.Font.Bold = true;
        //xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        ////xlBook.Application.DisplayAlerts = false;

        //xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        //Process.Start(filePath);
      }
    }
    #endregion Event
  }
}
