/*
  Author      : 
  Date        : 18/01/2013
  Description : Register Department
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;

namespace DaiCo.General
{
  public partial class viewGNR_99_003 : MainUserControl
  {
    #region Field
    private IList listDeleting = new ArrayList();
    private IList listDeleted = new ArrayList();
    #endregion Field

    #region Init

    /// <summary>
    /// Itit Form
    /// </summary>
    public viewGNR_99_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_001_Load(object sender, EventArgs e)
    {
      // Load Kind
      this.LoadComboKind();

      // Load Grid
      this.LoadGrid();
    }

    /// <summary>
    /// Load UltraCombo Kind = Department
    /// </summary>
    private void LoadComboKind()
    {
      string commandText = string.Empty;
      commandText += " SELECT Code, Value";
      commandText += " FROM TblBOMCodeMaster ";
      commandText += " WHERE [Group] = 99001 ";
      commandText += " 	AND DeleteFlag = 0";
      commandText += " ORDER BY Sort ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultKind.DataSource = dtSource;
      ultKind.DisplayMember = "Value";
      ultKind.ValueMember = "Code";
      ultKind.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultKind.DisplayLayout.Bands[0].Columns["Value"].Width = 150;
      ultKind.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load UltraGrid 
    /// </summary>
    private void LoadGrid()
    {
      this.listDeleting = new ArrayList();
      this.listDeleted = new ArrayList();

      string commandText = string.Empty;
      commandText += " SELECT Pid, [Kind], [Function], [TimeDoIt], [Description], [KPI], 0 Selected ";
      commandText += " FROM TblGNRITLogDaily ";
      commandText += " WHERE [CreateBy] = " + SharedObject.UserInfo.UserPid;
      commandText += "  	AND CONVERT(date, CreateDate) = CONVERT(date, GETDATE()) ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultData.DataSource = dtSource;
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultData.Rows[i];
        int kind = DBConvert.ParseInt(rowGrid.Cells["Kind"].Value.ToString());
        if (kind >= 0)
        {
          UltraDropDown ultFunction = (UltraDropDown)rowGrid.Cells["Function"].ValueList;
          rowGrid.Cells["Function"].ValueList = this.LoadFunction(kind, ultFunction);
        }
      }
    }

    /// <summary>
    /// Check Input
    /// </summary>
    /// <param name="warehouse"></param>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private bool ValidationInput(DataTable dtSource)
    {
      bool result = true;
      string commandText = string.Empty;
      foreach (DataRow row in dtSource.Rows)
      {
        if (row["Kind"].ToString().Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Module");
          result = false;
          return result;
        }

        if (row["Function"].ToString().Length == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Function");
          result = false;
          return result;
        }

        if (row["TimeDoIt"].ToString().Length == 0 
            || DBConvert.ParseDouble(row["TimeDoIt"].ToString()) == double.MinValue)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Time Do It");
          result = false;
          return result;
        }

        // check Kind
        commandText += " SELECT COUNT(*) ";
        commandText += " FROM TblBOMCodeMaster ";
        commandText += " WHERE [Group] = 99001 ";
        commandText += "  	AND DeleteFlag = 0 ";
        commandText += "  	AND Code = " + DBConvert.ParseInt(row["Kind"].ToString());

        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Department");
          result = false;
          return result;
        }

        // Check function 
        commandText = string.Empty;
        commandText += " SELECT COUNT(*) ";
        commandText += " FROM TblBOMCodeMaster ";
        commandText += " WHERE [Group] = 99002 ";
        commandText += "  	AND DeleteFlag = 0 ";
        commandText += "  	AND Kind = " + DBConvert.ParseInt(row["Kind"].ToString());
        commandText += "  	AND Code = " + DBConvert.ParseInt(row["Function"].ToString());

        dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Function");
          result = false;
          return result;
        }
      }
      return result;
    }

    /// <summary>
    /// Load Move To Customer
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="ultRevision"></param>
    /// <returns></returns>
    private UltraDropDown LoadFunction(int kind, UltraDropDown ultFunction)
    {
      if (ultFunction == null)
      {
        ultFunction = new UltraDropDown();
        this.Controls.Add(ultFunction);
      }

      string commandText = string.Empty;
      commandText += " SELECT Code, Value";
      commandText += " FROM TblBOMCodeMaster ";
      commandText += " WHERE [Group] = 99002 ";
      commandText += "  	AND DeleteFlag = 0 ";
      commandText += "  	AND Kind = " + kind;
 
      DataTable dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultFunction.DataSource = dt;
      ultFunction.ValueMember = "Code";
      ultFunction.DisplayMember = "Value";
      ultFunction.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultFunction.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultFunction.DisplayLayout.Bands[0].Columns["Value"].Header.Caption = "Function";

      return ultFunction;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Bands[0].Columns["KPI"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["KPI"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].DefaultCellValue = 0;
      e.Layout.Bands[0].Columns["KPI"].DefaultCellValue = 0;

      e.Layout.Bands[0].Columns["Kind"].ValueList = this.ultKind;

      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["KPI"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["TimeDoIt"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Kind"].Header.Caption = "Module";
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// btnSave Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      bool checkSave = ValidationInput(dtSource);

      if (checkSave)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          string storeName = string.Empty;
          DBParameter[] input = new DBParameter[7];
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            string value = row["Kind"].ToString().Trim();
            string remark = row["Description"].ToString().Trim();

            if (DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
            {
              input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
            }
            input[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            input[2] = new DBParameter("@Kind", DbType.Int32, DBConvert.ParseInt(row["Kind"].ToString()));
            input[3] = new DBParameter("@Function", DbType.Int32, DBConvert.ParseInt(row["Function"].ToString()));
            input[4] = new DBParameter("@TimeDoIt", DbType.Double, DBConvert.ParseDouble(row["TimeDoIt"].ToString()));
            input[5] = new DBParameter("@Description", DbType.String, row["Description"].ToString());
            if (DBConvert.ParseInt(row["KPI"].ToString()) == 1)
            {
              input[6] = new DBParameter("@KPI", DbType.String, 1);
            }
            else
            {
              input[6] = new DBParameter("@KPI", DbType.String, 0);
            }

            storeName = "spGNRITLogDaily_Edit";

            DataBaseAccess.ExecuteStoreProcedure(storeName, input, output);
            if (DBConvert.ParseLong(output[0].Value.ToString()) <= 0)
            {
              continue;
            }
          }
        }
        WindowUtinity.ShowMessageSuccess("MSG0004");

        // Load Grid
        this.LoadGrid();
      }   
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    /// <summary>
    /// Delete Group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        int countCheck = 0;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
          if (selected == 1)
          {
            countCheck++;
          }
        }

        if (countCheck == 0)
        {
          return;
        }

        DialogResult result = WindowUtinity.ShowMessageConfirm("MSG0015");
        if (result == DialogResult.Yes)
        {
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
            if (selected == 1)
            {
              long pid = DBConvert.ParseInt(ultData.Rows[i].Cells["Pid"].Value.ToString());
              DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, 
                        DBConvert.ParseLong(ultData.Rows[i].Cells["Pid"].Value.ToString())) };
              DataBaseAccess.ExecuteStoreProcedure("spGNRITLogDaily_Delete", inputParam);
            }
          }
          WindowUtinity.ShowMessageSuccess("MSG0002");          
          // Load Grid
          this.LoadGrid();
        }
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      switch (columnName)
      {
        case "kind":
          int kind = DBConvert.ParseInt(e.Cell.Row.Cells["Kind"].Value.ToString().Trim());
          UltraDropDown ultFunction = (UltraDropDown)e.Cell.Row.Cells["Function"].ValueList;
          e.Cell.Row.Cells["Function"].ValueList = this.LoadFunction(kind, ultFunction);

          break;
      }
    }
    #endregion Event   
  }
}
