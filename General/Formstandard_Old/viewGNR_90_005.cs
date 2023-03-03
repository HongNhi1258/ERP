/*
  Author      : 
  Date        : 19/8/2013
  Description : Save Master Detail Basic
  Standard Code: view_GNR_90_005
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;
using VBReport;

namespace DaiCo.General
{
  public partial class view_GNR_90_005 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    private int status = int.MinValue;
    private IList listDeletedPid = new ArrayList();

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    #endregion Field

    #region Init

    public view_GNR_90_005()
    {
      InitializeComponent();
    }

    private void viewGNR_90_005_Load(object sender, EventArgs e)
    {
      this.LoadInit();
      this.LoadData();
      this.SetStatusControl();
    }

    #endregion Init

    #region Function

    private void LoadInit()
    {
      // Load Data Control...
      //this.LoadCombo();
    }

    ///// <summary>
    ///// Load UltraCombo
    ///// </summary>
    //private void LoadCombo()
    //{
    //  string commandText = string.Empty;
    //  commandText += " ....";
    //  DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
    //  if (dtSource != null)
    //  {
    //    ultControl.DataSource = dtSource;
    //    ultControl.DisplayMember = "...";
    //    ultControl.ValueMember = "...";
    //    // Format Grid
    //    ultControl.DisplayLayout.Bands[0].ColHeadersVisible = false;
    //    ultControl.DisplayLayout.Bands[0].Columns["..."].Width = 250;
    //    ultControl.DisplayLayout.Bands[0].Columns["..."].Hidden = true;
    //  }
    //}

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("", inputParam);
      if (dsSource != null)
      {
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          this.status = DBConvert.ParseInt(row["Status"].ToString());
          //Code
          //Code
          if (this.status > 0)
          {
            chkConfirm.Checked = true;
          }
        }
        else
        {
          //DataTable dtCode = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHDGetNewReceivingNo('01ADI') NewReceivingNo");
          //if ((dtCode != null) && (dtCode.Rows.Count == 1))
          //{
          //  this.txtReceivingNote.Text = dtCode.Rows[0]["NewReceivingNo"].ToString();
          //  this.txtDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
          //  this.txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          //}
        }

        // Load Detail
        ultData.DataSource = dsSource.Tables[1];
        // Set Status Control
        this.SetStatusControl();
      }
    }

    private void SetStatusControl()
    {
      
    }

    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      //if (ultControl.Value == null)
      //{
      //  message = "Control...";
      //  return false;
      //}
      return true;
    }

    private bool SaveData()
    {
      // Save master info
      bool success = this.SaveInfo();
      if (success)
      {
        // Save detail
        success = this.SaveDetail();
      }
      else
      {
        success = false;
      }
      return success;
    }

    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[1];
      if (this.pid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }

      //Code
      //Code

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("", inputParam, ouputParam);
      // Gan Lai Pid
      this.pid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if(this.pid == long.MinValue)
      {
        return false;
      }
      return true;
    }

    private bool SaveDetail()
    {
      // Delete Row In grid
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      // Save Detail
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if(row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[1];
          if(DBConvert.ParseLong(row["DetailPid"].ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@DetailPid", DbType.Int64, DBConvert.ParseLong(row["DetailPid"].ToString()));
          }

          // Code
          // Code

          DBParameter[] outPutParam = new DBParameter[1];
          outPutParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("", inputParam, outPutParam);
          if(DBConvert.ParseLong(outPutParam[0].Value.ToString()) == long.MinValue)
          {
            return false;
          }
        }
      }
      // End
      return true;
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      /*
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      */

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString().ToLower();
      //switch (columnName)
      //{
      //  case "location":
      //    if (DBConvert.ParseLong(e.Cell.Row.Cells["Location"].Value.ToString()) <= 0)
      //    {
      //      WindowUtinity.ShowMessageError("ERR0001", "Location");
      //      return;
      //    }
      //    break;
      //  default:
      //    break;
      //}
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString();
      //string text = e.Cell.Text.Trim();
      //switch (columnName.ToLower())
      //{
      //  case "qty":
      //    if (text.Length > 0)
      //    {
      //      if (DBConvert.ParseDouble(text) <= 0)
      //      {
      //        WindowUtinity.ShowMessageError("ERR0001", "Qty");
      //        e.Cancel = true;
      //      }
      //    }
      //    break;
      //  default:
      //    break;
      //}
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      bool success = this.CheckVaild(out message);
      if(!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      //Load Data
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Brown The Link
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtImportExcel.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtImportExcel.Text.Trim().Length > 0);
    }

    /// <summary>
    /// Get Template
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      //string templateName = "RPT_VEN_01_002_01";
      //string sheetName = "Data";
      //string outFileName = "RPT_VEN_01_002_01";
      //string startupPath = System.Windows.Forms.Application.StartupPath;
      //string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      //string pathTemplate = startupPath + @"\ExcelTemplate";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      //oXlsReport.Out.File(outFileName);
      //Process.Start(outFileName);
    }

    /// <summary>
    /// Import Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnImport_Click(object sender, EventArgs e)
    {
      if (this.txtImportExcel.Text.Trim().Length == 0)
      {
        return;
      }

      try
      {
        DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImportExcel.Text.Trim(), "SELECT * FROM [Data (1)$B5:J1007]").Tables[0];

        if (dtSource == null)
        {
          return;
        }

        //// Data

        btnImport.Enabled = false;
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
    }
    #endregion Event
  }
}
