/*
  Author      : 
  Date        : 22/08/2013
  Description : Manage The Process In Process
  Standard Form : viewGNR_90_008 
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
  public partial class viewGNR_50_003 : MainUserControl
  {
    #region Field
    private IList listDeleting = new ArrayList();
    private IList listDeleted = new ArrayList();
    private DataTable dtCoder;
    #endregion Field

    #region Init

    /// <summary>
    /// Itit Form
    /// </summary>
    public viewGNR_50_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_50_003_Load(object sender, EventArgs e)
    {
      // Load Combo Project
      this.LoadComboProject();

      // Load Combo Programmer
      this.LoadComboProgrammer();

      // Load Combo Leader
      this.LoadComboLeader();

      // Load Combo Coder
      this.LoadComboCoder();

      // Load Status
      this.LoadComboStatus();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// ProcessCmdKey
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.LoadGrid();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Load UltraCombo Status
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT 0 Pid, 'On Progress' Label ";
      commandText += " UNION ALL ";
      commandText += " SELECT 1 Pid, 'Finished' Label ";
      commandText += " UNION ALL ";
      commandText += " SELECT 2 Pid, 'Cancel' Label ";
      //commandText += " UNION ALL ";
      //commandText += " SELECT 3 Pid, 'On Progress' Label ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultStatus.DataSource = dtSource;
        ultStatus.DisplayMember = "Label";
        ultStatus.ValueMember = "Pid";
        // Format Grid
        ultStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultStatus.DisplayLayout.Bands[0].Columns["Label"].Width = 250;
        ultStatus.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;

        this.ultStatus.Value = 0;
      }
    }

    /// <summary>
    /// Load UltraCombo Project
    /// </summary>
    private void LoadComboProject()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid ProjectPid, Project ";
      commandText += " FROM TblGNRProject ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultProject.DataSource = dtSource;
        ultProject.DisplayMember = "Project";
        ultProject.ValueMember = "ProjectPid";
        // Format Grid
        ultProject.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultProject.DisplayLayout.Bands[0].Columns["Project"].Width = 250;
        ultProject.DisplayLayout.Bands[0].Columns["ProjectPid"].Hidden = true;
      }
    }

    /// <summary>
    /// Load UltraCombo Programmmer
    /// </summary>
    private void LoadComboProgrammer()
    {
      string commandText = string.Empty;
      commandText += " SELECT NV.ID_NhanVien, NV.TenNV + ' ' + NV.HoNV Name ";
      commandText += " FROM VHRNhanVien NV ";
      commandText += "  INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "                      AND NV.Department = CM.Value ";
      commandText += " WHERE Resigned = 0 ";
      commandText += " ORDER BY ID_NhanVien ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultProgrammer.DataSource = dtSource;
        ultProgrammer.DisplayMember = "Name";
        ultProgrammer.ValueMember = "ID_NhanVien";
        // Format Grid
        ultProgrammer.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultProgrammer.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
        ultProgrammer.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
      }
    }

    /// <summary>
    /// Load UltraCombo Leader
    /// </summary>
    private void LoadComboLeader()
    {
      string commandText = string.Empty;
      commandText += " SELECT NV.ID_NhanVien, NV.TenNV + ' ' + NV.HoNV Name ";
      commandText += " FROM VHRNhanVien NV ";
      commandText += "  INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "                      AND NV.Department = CM.Value ";
      commandText += " WHERE Resigned = 0 ";
      commandText += " ORDER BY ID_NhanVien ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultLeader.DataSource = dtSource;
        ultLeader.DisplayMember = "Name";
        ultLeader.ValueMember = "ID_NhanVien";
        // Format Grid
        ultLeader.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultLeader.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
        ultLeader.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;

        this.ultLeader.Value = SharedObject.UserInfo.UserPid;
      }
    }

    /// <summary>
    /// Load UltraCombo Coder
    /// </summary>
    private void LoadComboCoder()
    {
      string commandText = string.Empty;
      commandText += " SELECT NV.ID_NhanVien, NV.TenNV + ' ' + NV.HoNV Name , COF.Coefficient";
      commandText += " FROM VHRNhanVien NV";
      commandText += "  INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "                      AND NV.Department = CM.Value ";
      commandText += "  LEFT JOIN TblGNRProjectCoefficient COF ON COF.UserPid = NV.ID_NhanVien";
      commandText += " WHERE NV.Resigned = 0 ";
      commandText += " ORDER BY NV.ID_NhanVien ";

      dtCoder = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCoder != null)
      {
        udrpCoder.DataSource = dtCoder;
        udrpCoder.DisplayMember = "Name";
        udrpCoder.ValueMember = "ID_NhanVien";
        // Format Grid
        udrpCoder.DisplayLayout.Bands[0].ColHeadersVisible = false;
        udrpCoder.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
        udrpCoder.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
      }
    }

    /// <summary>
    /// Load UltraGrid 
    /// </summary>
    private void LoadGrid()
    {
      this.btnSearch.Enabled = false;
      DBParameter[] param = new DBParameter[4];

      // Project
      if (this.ultProject.Value != null &&
          DBConvert.ParseLong(this.ultProject.Value.ToString()) != long.MinValue)
      {
        param[0] = new DBParameter("@ProjectPid", DbType.Int64, DBConvert.ParseLong(this.ultProject.Value.ToString()));
      }

      // Programmer
      if (this.ultProgrammer.Value != null &&
          DBConvert.ParseInt(this.ultProgrammer.Value.ToString()) != int.MinValue)
      {
        param[1] = new DBParameter("@Programmer", DbType.Int32, DBConvert.ParseInt(this.ultProgrammer.Value.ToString()));
      }

      // Leader
      if (this.ultLeader.Value != null &&
          DBConvert.ParseInt(this.ultLeader.Value.ToString()) != int.MinValue)
      {
        param[2] = new DBParameter("@Leader", DbType.Int32, DBConvert.ParseInt(this.ultLeader.Value.ToString()));
      }

      // Status
      if (this.ultStatus.Value != null &&
          DBConvert.ParseInt(this.ultStatus.Value.ToString()) != int.MinValue)
      {
        param[3] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(this.ultStatus.Value.ToString()));
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRProjectScheduleProject_Select", param);

      // Create DataSet
      DataSet dsData = this.CreateDataSet();
      dsData.Tables["Project"].Merge(ds.Tables[0]);
      dsData.Tables["CoderResponsible"].Merge(ds.Tables[1]);
      dsData.Tables["Function"].Merge(ds.Tables[2]);
      dsData.Tables["Schedule"].Merge(ds.Tables[3]);
      // Load Data On Grid

      ultData.DataSource = dsData;

      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultData.Rows[i];
        // On Process
        if (DBConvert.ParseInt(rowGrid.Cells["StatusProject"].Value.ToString()) == 0)
        {
          rowGrid.CellAppearance.BackColor = Color.Cyan;
        }
        else if (DBConvert.ParseInt(rowGrid.Cells["StatusProject"].Value.ToString()) == 3)
        {
          rowGrid.CellAppearance.BackColor = Color.Cyan;
        }
        else if (DBConvert.ParseInt(rowGrid.Cells["StatusProject"].Value.ToString()) == 1)
        {
          rowGrid.CellAppearance.BackColor = Color.DodgerBlue;
        }
        else if (DBConvert.ParseInt(rowGrid.Cells["StatusProject"].Value.ToString()) == 2)
        {
          rowGrid.CellAppearance.BackColor = Color.Gray;
        }

        if (DBConvert.ParseInt(rowGrid.Cells["StatusProject"].Value.ToString()) == 0)
        {
          try
          {
            for (int j = 0; j < rowGrid.ChildBands[1].Rows.Count; j++)
            {
              UltraGridRow rowChild = rowGrid.ChildBands[1].Rows[j];
              if (DBConvert.ParseInt(rowChild.Cells["LastFinished"].Value.ToString()) != 1)
              {
                for (int k = 0; k < rowChild.ChildBands[0].Rows.Count; k++)
                {
                  UltraGridRow rowK = rowChild.ChildBands[0].Rows[k];

                  if (DBConvert.ParseInt(rowK.Cells["Finished"].Value.ToString()) == 1
                      || DBConvert.ParseInt(rowK.Cells["Cancelled"].Value.ToString()) == 1)
                  {
                    rowK.Cells["Coder"].Activation = Activation.ActivateOnly;
                    rowK.Cells["StartTime"].Activation = Activation.ActivateOnly;
                    rowK.Cells["EndTime"].Activation = Activation.ActivateOnly;
                    rowK.Cells["Time"].Activation = Activation.ActivateOnly;
                    rowK.Cells["Finished"].Activation = Activation.ActivateOnly;
                    rowK.Cells["Cancelled"].Activation = Activation.ActivateOnly;
                  }
                  else
                  {
                    rowK.Cells["Coder"].Activation = Activation.AllowEdit;
                    rowK.Cells["StartTime"].Activation = Activation.AllowEdit;
                    rowK.Cells["EndTime"].Activation = Activation.AllowEdit;
                    if (DBConvert.ParseInt(rowK.Cells["Kind"].Value.ToString()) == 1)
                    {
                      rowK.Cells["Time"].Activation = Activation.ActivateOnly;
                    }
                    rowK.Cells["Finished"].Activation = Activation.AllowEdit;
                    rowK.Cells["Cancelled"].Activation = Activation.AllowEdit;
                  }
                }
              }
            }
          }
          catch
          {
            continue;
          }
        }
      }

      this.btnSearch.Enabled = true;
    }

    /// <summary>
    /// Create DataSet
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      // Project
      DataTable taProject = new DataTable("Project");
      taProject.Columns.Add("ProjectPid", typeof(System.Int64));
      taProject.Columns.Add("Project", typeof(System.String));
      taProject.Columns.Add("Leader", typeof(System.String));
      taProject.Columns.Add("StartTime", typeof(System.DateTime));
      taProject.Columns.Add("EndTime", typeof(System.DateTime));
      taProject.Columns.Add("RealEndTime", typeof(System.DateTime));
      taProject.Columns.Add("ScheduleDay", typeof(System.Double));
      taProject.Columns.Add("CoderDay", typeof(System.Double));
      taProject.Columns.Add("Finished", typeof(System.Int32));
      taProject.Columns.Add("StatusProject", typeof(System.Int32));
      ds.Tables.Add(taProject);

      // Coder Responsible
      DataTable taCoder = new DataTable("CoderResponsible");
      taCoder.Columns.Add("ProjectPid", typeof(System.Int64));
      taCoder.Columns.Add("Coder", typeof(System.String));
      taCoder.Columns.Add("Coefficient", typeof(System.Double));
      ds.Tables.Add(taCoder);

      // Function
      DataTable taFunction = new DataTable("Function");
      taFunction.Columns.Add("No", typeof(System.Int64));
      taFunction.Columns.Add("ProjectPid", typeof(System.Int64));
      taFunction.Columns.Add("FunctionPid", typeof(System.Int64));
      taFunction.Columns.Add("Function", typeof(System.String));
      taFunction.Columns.Add("Time", typeof(System.Double));
      taFunction.Columns.Add("Finished", typeof(System.Int32));
      ds.Tables.Add(taFunction);

      // Schedule
      DataTable taSchedule = new DataTable("Schedule");
      taSchedule.Columns.Add("Pid", typeof(System.Int64));
      taSchedule.Columns.Add("ProjectPid", typeof(System.Int64));
      taSchedule.Columns.Add("FunctionPid", typeof(System.Int64));
      taSchedule.Columns.Add("Component", typeof(System.String));
      taSchedule.Columns.Add("Coder", typeof(System.Int32));
      taSchedule.Columns.Add("StartTime", typeof(System.DateTime));
      taSchedule.Columns.Add("EndTime", typeof(System.DateTime));
      taSchedule.Columns.Add("Time", typeof(System.Double));
      taSchedule.Columns.Add("StartTimeActual", typeof(System.DateTime));
      taSchedule.Columns.Add("EndTimeActual", typeof(System.DateTime));
      taSchedule.Columns.Add("TimeActual", typeof(System.Double));
      taSchedule.Columns.Add("PercentFinish",typeof(System.Double));
      taSchedule.Columns.Add("RealEndTime", typeof(System.DateTime));
      taSchedule.Columns.Add("Finished", typeof(System.Int32));
      taSchedule.Columns.Add("Cancelled", typeof(System.Int32));
      taSchedule.Columns["Finished"].DefaultValue = 0;
      taSchedule.Columns["Cancelled"].DefaultValue = 0;
      ds.Tables.Add(taSchedule);

      ds.Relations.Add(new DataRelation("Project_CoderResponsible", 
            taProject.Columns["ProjectPid"], taCoder.Columns["ProjectPid"], false));

      ds.Relations.Add(new DataRelation("Project_Function",
            taProject.Columns["ProjectPid"], taFunction.Columns["ProjectPid"], false));

      ds.Relations.Add(new DataRelation("Function_Schedule",
            new DataColumn[] { taFunction.Columns["ProjectPid"], taFunction.Columns["FunctionPid"] }, 
            new DataColumn[] { taSchedule.Columns["ProjectPid"], taSchedule.Columns["FunctionPid"] }));
      return ds;
    }

    /// <summary>
    /// Check Input
    /// </summary>
    /// <param name="warehouse"></param>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private bool CheckVaild(out string message)
    {
      message = string.Empty;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        for (int j = 0; j < rowParent.ChildBands[1].Rows.Count; j++)
        {
          UltraGridRow rowChild = rowParent.ChildBands[1].Rows[j];
          double totalTime = DBConvert.ParseDouble(rowChild.Cells["Time"].Value.ToString());
          double time = 0;
          for (int k = 0; k < rowChild.ChildBands[0].Rows.Count; k++)
          {
            UltraGridRow rowChildDetail = rowChild.ChildBands[0].Rows[k];
            if (DBConvert.ParseInt(rowChildDetail.Cells["IsUpdate"].Value.ToString()) == 1)
            {
              if (DBConvert.ParseDouble(rowChildDetail.Cells["Time"].Value.ToString()) <= 0)
              {
                message = "Time";
                return false;
              }

              if (DBConvert.ParseDouble(rowChildDetail.Cells["Time"].Value.ToString()) > 0)
              {
                time = time + DBConvert.ParseDouble(rowChildDetail.Cells["Time"].Value.ToString());
              }

              // Check user
              if (DBConvert.ParseInt(rowChildDetail.Cells["Coder"].Value.ToString()) <= 0)
              {
                message = "Coder";
                return false;
              }
              // Check Start Date
              if (rowChildDetail.Cells["StartTime"].Text.Length == 0 ||
                DBConvert.ParseDateTime(rowChildDetail.Cells["StartTime"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
              {
                message = "Start Time";
                return false;
              }
              // Check End Date
              if (rowChildDetail.Cells["EndTime"].Text.Length == 0 ||
                DBConvert.ParseDateTime(rowChildDetail.Cells["EndTime"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
              {
                message = "End Time";
                return false;
              }
            }
          }

          time = Math.Round(time, 2);
          // Check
          if (time > totalTime)
          {
            message = "Total Time > " + totalTime.ToString() + "";
            return false;
          }
        }
      }
      return true;
    }

    private bool SaveData()
    {
      // Save info
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultData.Rows[i];
        if (DBConvert.ParseInt(rowGrid.Cells["StatusProject"].Value.ToString()) == 0 || DBConvert.ParseInt(rowGrid.Cells["StatusProject"].Value.ToString()) == 3)
        {
          try
          {
            for (int j = 0; j < rowGrid.ChildBands[1].Rows.Count; j++)
            {
              UltraGridRow rowChild = rowGrid.ChildBands[1].Rows[j];
              if (DBConvert.ParseInt(rowChild.Cells["LastFinished"].Value.ToString()) != 1)
              {
                for (int k = 0; k < rowChild.ChildBands[0].Rows.Count; k++)
                {
                  UltraGridRow rowK = rowChild.ChildBands[0].Rows[k];
                 if (DBConvert.ParseInt(rowK.Cells["IsUpdate"].Value.ToString()) == 1)
                  {
                    DBParameter[] inputParam = new DBParameter[13];

                    if (DBConvert.ParseLong(rowK.Cells["Pid"].Value.ToString()) != long.MinValue)
                    {
                      inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(rowK.Cells["Pid"].Value.ToString()));
                    }

                    inputParam[1] = new DBParameter("@ProjectPid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["ProjectPid"].Value.ToString()));
                    if (DBConvert.ParseInt(rowK.Cells["Coder"].Value.ToString()) > 0)
                    {
                      inputParam[2] = new DBParameter("@UserPid", DbType.Int32, DBConvert.ParseInt(rowK.Cells["Coder"].Value.ToString()));
                    }
                    inputParam[3] = new DBParameter("@FunctionPid", DbType.Int64, DBConvert.ParseLong(rowChild.Cells["FunctionPid"].Value.ToString()));

                    if (DBConvert.ParseDateTime(rowK.Cells["StartTime"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME)
                          != DateTime.MinValue)
                    {
                      inputParam[4] = new DBParameter("@StartTime", DbType.DateTime,
                        DBConvert.ParseDateTime(rowK.Cells["StartTime"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
                    }
                    else
                    {
                      inputParam[4] = new DBParameter("@StartTime", DbType.DateTime, DBNull.Value);
                    }

                    if (DBConvert.ParseDateTime(rowK.Cells["EndTime"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME)
                          != DateTime.MinValue)
                    {
                      inputParam[5] = new DBParameter("@EndTime", DbType.DateTime,
                        DBConvert.ParseDateTime(rowK.Cells["EndTime"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
                    }
                    else
                    {
                      inputParam[5] = new DBParameter("@EndTime", DbType.DateTime, DBNull.Value);
                    }

                    inputParam[6] = new DBParameter("@Time", DbType.Double, DBConvert.ParseDouble(rowK.Cells["Time"].Value.ToString()));
                    inputParam[7] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseDouble(rowK.Cells["Finished"].Value.ToString()));
                    inputParam[8] = new DBParameter("@Cancelled", DbType.Int32, DBConvert.ParseDouble(rowK.Cells["Cancelled"].Value.ToString()));

                    if (DBConvert.ParseDateTime(rowK.Cells["StartTimeActual"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME)
                        != DateTime.MinValue)
                    {
                        inputParam[9] = new DBParameter("@StartTimeActual", DbType.DateTime,
                          DBConvert.ParseDateTime(rowK.Cells["StartTimeActual"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
                    }
                    else
                    {
                        inputParam[9] = new DBParameter("@StartTimeActual", DbType.DateTime, DBNull.Value);
                    }

                    if (DBConvert.ParseDateTime(rowK.Cells["EndTimeActual"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME)
                          != DateTime.MinValue)
                    {
                        inputParam[10] = new DBParameter("@EndTimeActual", DbType.DateTime,
                          DBConvert.ParseDateTime(rowK.Cells["EndTimeActual"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
                    }
                    else
                    {
                        inputParam[10] = new DBParameter("@EndTimeActual", DbType.DateTime, DBNull.Value);
                    }
                    if (DBConvert.ParseDouble(rowK.Cells["TimeActual"].Value.ToString()) != double.MinValue)
                    {
                        inputParam[11] = new DBParameter("@TimeActual", DbType.Double, DBConvert.ParseDouble(rowK.Cells["TimeActual"].Value.ToString()));
                    }
                    if (DBConvert.ParseDouble(rowK.Cells["PercentFinish"].Value.ToString()) != double.MinValue)
                    {
                        inputParam[12] = new DBParameter("@PercentFinish", DbType.Double, DBConvert.ParseDouble(rowK.Cells["PercentFinish"].Value.ToString()));
                    }
                    DBParameter[] outputParam = new DBParameter[1];
                    outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);

                    DataBaseAccess.ExecuteStoreProcedure("spGNRProjectScheduleCodingForDeveloper_Edit", inputParam, outputParam);
                    long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
                    if (result == 0)
                    {
                      return false;
                    }
                  }
                }
              }
            }
          }
          catch
          {
            continue;
          }
        }
      }

      return true;
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
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[3].ColHeaderLines = 2;
      if (this.radHorizontal.Checked)
      {
        e.Layout.ViewStyleBand = ViewStyleBand.Horizontal;
      }
      else if (this.radOutlook.Checked)
      {
        e.Layout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
      }
      else if (this.radVertical.Checked)
      {
        e.Layout.ViewStyleBand = ViewStyleBand.Vertical;
      }

      // Set Align
      // Band 0
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Band 1
      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Band 2
      for (int i = 0; i < e.Layout.Bands[2].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[2].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[2].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Band 3
      for (int i = 0; i < e.Layout.Bands[3].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[3].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[3].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Allow update, delete, add new
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[3].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;

      // Hide column
      e.Layout.Bands[0].Columns["ProjectPid"].Hidden = true;
      e.Layout.Bands[0].Columns["StatusProject"].Hidden = true;
      e.Layout.Bands[1].Columns["ProjectPid"].Hidden = true;
      e.Layout.Bands[2].Columns["ProjectPid"].Hidden = true;
      e.Layout.Bands[2].Columns["FunctionPid"].Hidden = true;
      e.Layout.Bands[3].Columns["ProjectPid"].Hidden = true;
      e.Layout.Bands[3].Columns["FunctionPid"].Hidden = true;
      e.Layout.Bands[3].Columns["Pid"].Hidden = true;
      e.Layout.Bands[3].Columns["CoefficientOld"].Hidden = true;
      e.Layout.Bands[3].Columns["Kind"].Hidden = true;
      e.Layout.Bands[3].Columns["IsUpdate"].Hidden = true;
      e.Layout.Bands[2].Columns["LastFinished"].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[2].Columns["Time"].Header.Caption = "Time(Day)";
      e.Layout.Bands[3].Columns["Time"].Header.Caption = "Estimated Time\n(Day)";
      
      // Set dropdownlist for column
      e.Layout.Bands[3].Columns["Coder"].ValueList = udrpCoder;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Finished"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[2].Columns["Finished"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[3].Columns["Finished"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[3].Columns["Cancelled"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Read only
      // Band 0
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      // Band 1
      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      // Band 2
      for (int i = 0; i < e.Layout.Bands[2].Columns.Count; i++)
      {
        e.Layout.Bands[2].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[3].Columns["RealEndTime"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[3].Columns["Component"].CellActivation = Activation.ActivateOnly;

      // Format date (dd/MM/yy)
      e.Layout.Bands[3].Columns["StartTime"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[3].Columns["StartTime"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[3].Columns["EndTime"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[3].Columns["EndTime"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[3].Columns["RealEndTime"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[3].Columns["RealEndTime"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[3].Columns["StartTimeActual"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[3].Columns["StartTimeActual"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[3].Columns["EndTimeActual"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[3].Columns["EndTimeActual"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["StartTime"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["StartTime"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["EndTime"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["EndTime"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["RealEndTime"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["RealEndTime"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      e.Layout.Bands[3].Columns["Coder"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[3].Columns["StartTime"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[3].Columns["EndTime"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[3].Columns["Time"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[3].Columns["StartTimeActual"].CellAppearance.BackColor = Color.LightGoldenrodYellow;
      e.Layout.Bands[3].Columns["EndTimeActual"].CellAppearance.BackColor = Color.LightGoldenrodYellow;
      e.Layout.Bands[3].Columns["TimeActual"].CellAppearance.BackColor = Color.LightGoldenrodYellow;
      e.Layout.Bands[3].Columns["Finished"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[3].Columns["Cancelled"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[3].Columns["PercentFinish"].CellAppearance.BackColor = Color.LightGoldenrodYellow;

      //Sum Qty And Total CBM
      e.Layout.Bands[3].Summaries.Add(SummaryType.Sum, e.Layout.Bands[3].Columns["Time"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Summaries[0].DisplayFormat = "{0:###,##0.00}";

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
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckVaild(out message);
      if (!success)
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

      // Load Grid
      this.LoadGrid();
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
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      // Load Grid
      this.LoadGrid();
    }

    /// <summary>
    /// Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName.ToLower())
      {
        case "coder":
          if (text.Length > 0)
          {
            DataRow[] foundRows = dtCoder.Select("Name ='" + text + "'");
            if (foundRows.Length == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Coder");
              e.Cancel = true;
            }
          }
          break;
        case "time":
          if (text.Length > 0)
          {
            if (DBConvert.ParseDouble(text) == long.MinValue)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Time(Day)");
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Delete Row On Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      // Confirmed Save
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      foreach (UltraGridRow row in e.Rows)
      {
        if (DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()) > 0)
        {
          DBParameter[] inputParams = new DBParameter[1];
          inputParams[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          DataBaseAccess.ExecuteStoreProcedure("spGNRProjectDeleteScheduleCoder_Delete", inputParams, outputParams);
          if (DBConvert.ParseInt(outputParams[0].Value.ToString()) != 1)
          {
            WindowUtinity.ShowMessageError("ERR0004");
            this.LoadGrid();
            return;
          }
        }
      }
    }

    /// <summary>
    /// Outlook Checked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void radOutlook_CheckedChanged(object sender, EventArgs e)
    {
      this.LoadGrid();
    }

    /// <summary>
    /// Horizontal
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void radHorizontal_CheckedChanged(object sender, EventArgs e)
    {
      this.LoadGrid();
    }

    /// <summary>
    /// Vertical
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void radVertical_CheckedChanged(object sender, EventArgs e)
    {
      this.LoadGrid();
    }

    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "Coder":
          row.Cells["IsUpdate"].Value = 1;

          if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 1)
          {
            if (DBConvert.ParseInt(row.Cells["Coder"].Value.ToString()) != int.MinValue)
            {
              double coefficient = DBConvert.ParseDouble(udrpCoder.SelectedRow.Cells["Coefficient"].Value.ToString());
              double coefficientOld = (DBConvert.ParseDouble(row.Cells["CoefficientOld"].Value.ToString()) == Double.MinValue) ? 0 : DBConvert.ParseDouble(row.Cells["CoefficientOld"].Value.ToString());
              row.Cells["Time"].Value = (DBConvert.ParseDouble(row.Cells["Time"].Value.ToString()) / coefficientOld) * coefficient;
              row.Cells["CoefficientOld"].Value = coefficient;
            }
            else
            {
              double coefficientOld = (DBConvert.ParseDouble(row.Cells["CoefficientOld"].Value.ToString()) == Double.MinValue) ? 0 : DBConvert.ParseDouble(row.Cells["CoefficientOld"].Value.ToString());
              row.Cells["Time"].Value = (DBConvert.ParseDouble(row.Cells["Time"].Value.ToString()) / coefficientOld);
              row.Cells["CoefficientOld"].Value = 1;
            }
          }
          break;
        case "Time":
          {
            row.Cells["IsUpdate"].Value = 1;
          }
          break;
        case "StartTime":
          {
            row.Cells["IsUpdate"].Value = 1;
          }
          break;
        case "EndTime":
          {
            row.Cells["IsUpdate"].Value = 1;
          }
          break;
        case "Finished":
          {
            row.Cells["IsUpdate"].Value = 1;
          }
          break;
        case "Cancelled":
          {
            row.Cells["IsUpdate"].Value = 1;
          }
          break;
      case "StartTimeActual":
          {
              row.Cells["IsUpdate"].Value = 1;
          }
          break;
      case "EndTimeActual":
          {
              row.Cells["IsUpdate"].Value = 1;
          }
          break;
      case "TimeActual":
          {
              row.Cells["IsUpdate"].Value = 1;
          }
          break;
      case "PercentFinish":
          {
              row.Cells["IsUpdate"].Value = 1;
          }
          break;
        default:
          break;
      }
    }
    #endregion Event   
  }
}
