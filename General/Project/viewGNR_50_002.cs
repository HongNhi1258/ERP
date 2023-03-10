/*
  Author      : XUÂN TRỪƠNG
  Date        : 23/08/2013
  Description : Definition Project
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
  public partial class viewGNR_50_002 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    long keys = long.MinValue;
    #endregion Field

    #region Init

    /// <summary>
    /// Itit Form
    /// </summary>
    public viewGNR_50_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_50_002_Load(object sender, EventArgs e)
    {
      // Init
      this.Init();

      // Load User IT
      this.LoadComboUserID();
      // Load Status
      this.LoadComboStatus();

      // Load Request Onlinr
      this.LoadComboRequestOnline();

      // Load Combo Status Filter
      this.LoadComboFilterStatus();

      //Load Task
      this.LoadScreenTime();

      // Load Grid
      this.LoadGrid();
      //
      btnDelete.Visible = false;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Init
    /// </summary>
    private void Init()
    {
      DateTime today = DateTime.Today;
      DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
      DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
      ultDateFrom.Value = startOfMonth;
      ultDateTo.Value = endOfMonth;
    }

    /// <summary>
    /// Load UltraCombo Status
    /// </summary>
    private void LoadComboFilterStatus()
    {
      string commandText = string.Empty;
      commandText += "SELECT 0 Pid, 'Analyzing & Coding' label ";
      commandText += "UNION ALL ";
      commandText += "SELECT 1 Pid, 'Finished' label ";
      commandText += "UNION ALL ";
      commandText += "SELECT NULL Pid, 'All' label ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultStatus.DataSource = dtSource;
        ultStatus.DisplayMember = "label";
        ultStatus.ValueMember = "Pid";
        // Format Grid
        ultStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultStatus.DisplayLayout.Bands[0].Columns["label"].Width = 250;
        ultStatus.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      }

      this.ultStatus.Value = 0;
    }

    /// <summary>
    /// Load UltraCombo Status
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += "SELECT 0 Pid, 'Analyzing' label ";
      commandText += "UNION ALL ";
      commandText += "SELECT 1 Pid, 'Finished' label ";
      commandText += "UNION ALL ";
      commandText += "SELECT 2 Pid, 'Cancel' label ";
      commandText += "UNION ALL ";
      commandText += "SELECT 3 Pid, 'Coding' label ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultdrpStatus.DataSource = dtSource;
        ultdrpStatus.DisplayMember = "label";
        ultdrpStatus.ValueMember = "Pid";
        // Format Grid
        ultdrpStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultdrpStatus.DisplayLayout.Bands[0].Columns["label"].Width = 250;
        ultdrpStatus.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      }
    }

    private void LoadScreenTime()
    {
      string commandText = "SELECT Code, Value, CAST([Description] AS float) [Time], ISNULL(Kind, 0) Kind FROM TblBOMCodeMaster WHERE [Group] = 9014";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDScreenTime.DataSource = dtSource;
      ultDDScreenTime.DisplayMember = "Value";
      ultDDScreenTime.ValueMember = "Code";
      ultDDScreenTime.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultDDScreenTime.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDDScreenTime.DisplayLayout.Bands[0].Columns["Value"].Width = 250;
      ultDDScreenTime.DisplayLayout.Bands[0].Columns["Time"].Width = 50;
    }

    /// <summary>
    /// Load UltraCombo UserID
    /// </summary>
    private void LoadComboUserID()
    {
      string commandText = string.Empty;
      commandText += "SELECT EM.Pid UserID, EM.EmpName UserName ";
      commandText += "FROM VHRMEmployee EM ";
      commandText += "INNER JOIN TblBOMCodeMaster CM ON CM.[Group] = 9200 ";
      commandText += "AND EM.Department = CM.Value ";
      commandText += "AND EM.Resigned = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultdrpUserIT.DataSource = dtSource;
        ultdrpUserIT.DisplayMember = "UserName";
        ultdrpUserIT.ValueMember = "UserID";
        // Format Grid
        ultdrpUserIT.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultdrpUserIT.DisplayLayout.Bands[0].Columns["UserName"].Width = 250;
        ultdrpUserIT.DisplayLayout.Bands[0].Columns["UserID"].Hidden = true;
      }
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboRequestOnline()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, RequestCode, Title Project FROM TblGNRRequestIT ORDER BY RequestCode DESC";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultdrpRequestOnline.DataSource = dtSource;
        ultdrpRequestOnline.DisplayMember = "RequestCode";
        ultdrpRequestOnline.ValueMember = "Pid";
        // Format Grid
        ultdrpRequestOnline.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultdrpRequestOnline.DisplayLayout.Bands[0].Columns["RequestCode"].Width = 150;
        ultdrpRequestOnline.DisplayLayout.Bands[0].Columns["Project"].Width = 250;
        ultdrpRequestOnline.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      }
    }

    /// <summary>
    /// Load UltraGrid 
    /// </summary>
    private void LoadGrid()
    {
      //Load Data On Grid
      this.Search();
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
      DataSet dsSource = (DataSet)ultData.DataSource;
      for (int i = 0;i< dsSource.Tables[0].Rows.Count; i++)
      {
        DataRow rowparent = dsSource.Tables[0].Rows[i];
        if (rowparent.RowState == DataRowState.Modified)
        {
          if (DBConvert.ParseDateTime(rowparent["StartTimeP"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) >
                         DBConvert.ParseDateTime(rowparent["EndTimeP"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME))
          {
            message = "StartTime <= EndTime";
            return false;
          }
        }
      }
      for (int j = 0; j < dsSource.Tables[1].Rows.Count; j++)
      {
        DataRow rowcon = dsSource.Tables[1].Rows[j];
        if (rowcon.RowState == DataRowState.Added || rowcon.RowState == DataRowState.Modified)
        {
          if (rowcon["NameProject"].ToString().Length == 0)
          {
            message = "Name Function";
            return false;
          }
          if (rowcon["RemarkF"].ToString().Length == 0)
          {
            message = "Remark";
            return false;
          }
          if (rowcon["UserPidF"].ToString().Length == 0)
          {
            message = "Person incharge";
            return false;
          }
          if (rowcon["TimeF"].ToString().Length == 0)
          {
            message = "Time";
            return false;
          }
        }
      }
      for (int k = 0; k < dsSource.Tables[2].Rows.Count; k++)
      {
        DataRow rowChau = dsSource.Tables[2].Rows[k];
        if (rowChau.RowState == DataRowState.Added || rowChau.RowState == DataRowState.Modified)
        {
          if (DBConvert.ParseDateTime(rowChau["StartTimeS"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) >
                   DBConvert.ParseDateTime(rowChau["EndTimeS"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME))
          {
            message = "StartTime <= EndTime";
            return false;
          }

          if (DBConvert.ParseDateTime(rowChau["StartTimeActual"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) >
                DBConvert.ParseDateTime(rowChau["EndTimeActual"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME))
          {
            message = "StartTime <= EndTime";
            return false;
          }

          // Check Coder
          if (rowChau["UserPidS"].ToString().Length == 0)
          {
            message = "Coder";
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns
    /// 
    private void SaveDataToSQL()
    {
      bool resultTotal = true;
      DataSet dsSource = (DataSet)ultData.DataSource;
      for (int i = 0; i < dsSource.Tables[0].Rows.Count; i++)
      {
        DataRow rowparent = dsSource.Tables[0].Rows[i];
        for (int j = 0; j < dsSource.Tables[1].Rows.Count; j++)
        {
          DataRow rowchild = dsSource.Tables[1].Rows[j];
          long resultCon = long.MinValue;
          if (DBConvert.ParseLong(rowparent["Pid"]) == DBConvert.ParseLong(rowchild["ProjectPid"]))
          {
            if (DBConvert.ParseInt(rowchild["IsUpdateF"])==1)
            {
              resultCon = DBConvert.ParseLong(rowchild["FuncPid"]);
              DBParameter[] inputCon = new DBParameter[7];
              string cmstore = "spGNRProjectFunction_Edit";
              if (DBConvert.ParseLong(rowchild["FuncPid"]) > 0)
              {
                inputCon[0] = new DBParameter("@PidFunction", DbType.Int64, DBConvert.ParseLong(rowchild["FuncPid"]));
              }
              inputCon[1] = new DBParameter("@ProjectPid", DbType.Int64, DBConvert.ParseLong(rowparent["Pid"]));
              inputCon[2] = new DBParameter("@NameProject", DbType.String, rowchild["NameProject"].ToString());
              inputCon[3] = new DBParameter("@Remark", DbType.String, rowchild["RemarkF"].ToString());
              inputCon[4] = new DBParameter("@Time", DbType.Double, DBConvert.ParseDouble(rowchild["TimeF"]));
              inputCon[5] = new DBParameter("@UserPidF", DbType.Int64, DBConvert.ParseLong(rowchild["UserPidF"]));
              inputCon[6] = new DBParameter("@Finished", DbType.Int32, DBConvert.ParseInt(rowchild["FinishedF"]));
              DBParameter[] outputCon = new DBParameter[1];
              outputCon[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
              DataBaseAccess.ExecuteStoreProcedure(cmstore, inputCon, outputCon);
              resultCon = DBConvert.ParseLong(outputCon[0].Value.ToString());
              if (resultCon > 0)
              {
                resultTotal = true;

                for (int k = 0; k < dsSource.Tables[2].Rows.Count; k++)
                {
                  DataRow rowGChild = dsSource.Tables[2].Rows[k];
                  if (DBConvert.ParseLong(rowchild["FuncPid"]) == DBConvert.ParseLong(rowGChild["FunctionPid"]))
                  {
                     //DBConvert.ParseInt(rowGChild["IsUpdateS"])==1
                    if (rowGChild.RowState == DataRowState.Added || rowGChild.RowState == DataRowState.Modified)
                    {
                      string StoreNameChau = "spGNRProjectSchedule_Edit";
                      DBParameter[] inputChau = new DBParameter[14];
                      inputChau[0] = new DBParameter("@PidProject", DbType.Int64, DBConvert.ParseLong(rowparent["Pid"]));
                      inputChau[1] = new DBParameter("@PidFunction", DbType.Int64, resultCon);
                      if (DBConvert.ParseLong(rowGChild["TaskPid"]) != long.MinValue)
                      {
                        inputChau[2] = new DBParameter("@TaskPid", DbType.Int64, DBConvert.ParseLong(rowGChild["TaskPid"]));
                      }
                      if (DBConvert.ParseInt(rowGChild["ComponentPid"]) != int.MinValue)
                      {
                        inputChau[3] = new DBParameter("@CompoPid", DbType.Int32, DBConvert.ParseInt(rowGChild["ComponentPid"]));
                      }
                      inputChau[4] = new DBParameter("@Remark", DbType.String, rowGChild["RemarkS"].ToString());
                      inputChau[5] = new DBParameter("@Coder", DbType.Int32, DBConvert.ParseInt(rowGChild["UserPidS"]));
                      inputChau[6] = new DBParameter("@StartTimeTask", DbType.DateTime, DBConvert.ParseDateTime(rowGChild["StartTimeS"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
                      if (DBConvert.ParseDateTime(rowGChild["EndTimeS"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                      {
                        inputChau[7] = new DBParameter("@EndTimeTask", DbType.DateTime, DBConvert.ParseDateTime(rowGChild["EndTimeS"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
                      }

                      if (DBConvert.ParseDouble(rowGChild["TimeS"].ToString()) != double.MinValue)
                      {
                        inputChau[8] = new DBParameter("@TimeTask", DbType.Double, DBConvert.ParseDouble(rowGChild["TimeS"].ToString()));
                      }

                      if (DBConvert.ParseDateTime(rowGChild["StartTimeActual"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                      {
                        inputChau[9] = new DBParameter("@StartTimeActual", DbType.DateTime, DBConvert.ParseDateTime(rowGChild["StartTimeActual"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
                      }
                      if (DBConvert.ParseDateTime(rowGChild["EndTimeActual"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                      {
                        inputChau[10] = new DBParameter("@EndTimeActual", DbType.DateTime, DBConvert.ParseDateTime(rowGChild["EndTimeActual"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
                      }
                      if (DBConvert.ParseDouble(rowGChild["TimeActual"].ToString()) != double.MinValue)
                      {
                        inputChau[11] = new DBParameter("@TimeActual", DbType.Double, DBConvert.ParseDouble(rowGChild["TimeActual"].ToString()));
                      }
                      if (DBConvert.ParseDouble(rowGChild["PercentFinish"].ToString()) != double.MinValue)
                      {
                        inputChau[12] = new DBParameter("@PercentFinish", DbType.Double, DBConvert.ParseDouble(rowGChild["PercentFinish"].ToString()));
                      }

                      if (DBConvert.ParseInt(rowGChild["FinishedS"].ToString()) != int.MinValue)
                      {
                        inputChau[13] = new DBParameter("@FinishTask", DbType.Int32, DBConvert.ParseInt(rowGChild["FinishedS"].ToString()));
                      }

                      DBParameter[] outputChau = new DBParameter[1];
                      outputChau[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
                      DataBaseAccess.ExecuteStoreProcedure(StoreNameChau, inputChau, outputChau);
                      if (DBConvert.ParseLong(outputChau[0].Value.ToString()) != 0)
                      {
                        resultTotal = true;
                      }
                      else
                      {
                        string message = string.Format(FunctionUtility.GetMessage("ERR0028"), "Error");
                        WindowUtinity.ShowMessageErrorFromText(message);
                        return;
                      }
                    }
                  }
                }
              }
              else
              {
                string message = string.Format(FunctionUtility.GetMessage("ERR0028"), "Error");
                WindowUtinity.ShowMessageErrorFromText(message);
                return;
              }
            }
          }
        }
        if (dsSource.Tables[0].Rows[i].RowState == DataRowState.Modified)
        {
          string StoreName = "spGNRProject_Edit";
          DBParameter[] inputCha = new DBParameter[11];
          inputCha[0] = new DBParameter("@PidProject", DbType.Int64, DBConvert.ParseLong(rowparent["Pid"]));
          inputCha[1] = new DBParameter("@Leader", DbType.Int32, DBConvert.ParseInt(rowparent["Leader"].ToString()));

          if (DBConvert.ParseDateTime(rowparent["StartTimeP"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            inputCha[2] = new DBParameter("@StartTime", DbType.DateTime, DBConvert.ParseDateTime(rowparent["StartTimeP"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }
          if (DBConvert.ParseDateTime(rowparent["EndTimeP"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            inputCha[3] = new DBParameter("@EndTime", DbType.DateTime, DBConvert.ParseDateTime(rowparent["EndTimeP"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }
          if (DBConvert.ParseDateTime(rowparent["DueDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            inputCha[4] = new DBParameter("@DueDate", DbType.DateTime, DBConvert.ParseDateTime(rowparent["DueDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }
          if (DBConvert.ParseDateTime(rowparent["IssueDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            inputCha[5] = new DBParameter("@IssueDate", DbType.DateTime, DBConvert.ParseDateTime(rowparent["IssueDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }
          if (rowparent["RelativeDepartment"].ToString().Length > 0)
          {
            inputCha[6] = new DBParameter("@RelDepartment", DbType.String, rowparent["RelativeDepartment"].ToString());
          }
          if (rowparent["Remark"].ToString().Length > 0)
          {
            inputCha[7] = new DBParameter("@Remark", DbType.String, rowparent["Remark"].ToString());
          }

          inputCha[8] = new DBParameter("@Hold", DbType.Int32, DBConvert.ParseInt(rowparent["Status"].ToString()));

          if (DBConvert.ParseDateTime(rowparent["DevIssueDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
          {
            inputCha[9] = new DBParameter("@DevIssueDate", DbType.DateTime, DBConvert.ParseDateTime(rowparent["DevIssueDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
          }
          inputCha[10] = new DBParameter("@UpdateBy", DbType.Int64, SharedObject.UserInfo.UserPid);          
          DBParameter[] outputCha = new DBParameter[1];
          outputCha[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure(StoreName, inputCha, outputCha);
          if (DBConvert.ParseLong(outputCha[0].Value.ToString()) == 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0028"), "Error");
            WindowUtinity.ShowMessageErrorFromText(message);
            return;
          }
          else
          {
            resultTotal = true;
          }
        }
      }
      if (resultTotal == true)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.Search();
      }
    }
    /// <summary>
    /// CREATE DATASET
    /// </summary>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      // Project
      DataTable taProject = new DataTable("Project");
      taProject.Columns.Add("Pid", typeof(System.Int64));
      taProject.Columns.Add("RequestOnlinePid", typeof(System.Int64));
      taProject.Columns.Add("RequestCode", typeof(System.String));
      taProject.Columns.Add("Project", typeof(System.String));
      taProject.Columns.Add("Leader", typeof(System.Int32));
      taProject.Columns.Add("StartTimeP", typeof(System.DateTime));
      taProject.Columns.Add("EndTimeP", typeof(System.DateTime));
      taProject.Columns.Add("DueDate", typeof(System.DateTime));
      taProject.Columns.Add("IssueDate", typeof(System.DateTime));
      taProject.Columns.Add("DevIssueDate", typeof(System.DateTime));
      taProject.Columns.Add("RealEndTime", typeof(System.DateTime));
      taProject.Columns.Add("TimeP", typeof(System.Double));
      taProject.Columns.Add("Status", typeof(System.Int32));
      taProject.Columns.Add("Hold", typeof(System.Int32));
      taProject.Columns.Add("HoldReason", typeof(System.String));
      taProject.Columns.Add("RelativeDepartment", typeof(System.String));
      taProject.Columns.Add("UpdateDate", typeof(System.DateTime));
      taProject.Columns.Add("Remark", typeof(System.String));
      taProject.Columns.Add("Select", typeof(System.Int32));
      taProject.Columns.Add("IsUpdateP", typeof(System.Int32));     
      taProject.Columns["IsUpdateP"].DefaultValue = 0;
      taProject.Columns["TimeP"].DefaultValue = 0;
      ds.Tables.Add(taProject);

      // Function
      DataTable taFunction = new DataTable("Function");
      taFunction.Columns.Add("FuncPid", typeof(System.Int64));
      taFunction.Columns.Add("ProjectPid", typeof(System.Int64));
      taFunction.Columns.Add("NameProject", typeof(System.String));
      taFunction.Columns.Add("RemarkF", typeof(System.String));
      taFunction.Columns.Add("TimeF", typeof(System.Double));
      taFunction.Columns.Add("UserPidF", typeof(System.Int32));
      taFunction.Columns.Add("FinishedF", typeof(System.Int32));
      taFunction.Columns.Add("IsUpdateF", typeof(System.Int32));
      taFunction.Columns["IsUpdateF"].DefaultValue = 0;
      taFunction.Columns["TimeF"].DefaultValue = 0;
      ds.Tables.Add(taFunction);

      // Schedule
      DataTable taSchedule = new DataTable("Schedule");
      taSchedule.Columns.Add("TaskPid", typeof(System.Int64));
      taSchedule.Columns.Add("ProjectPid", typeof(System.Int64));
      taSchedule.Columns.Add("FunctionPid", typeof(System.Int64));
      taSchedule.Columns.Add("ComponentPid", typeof(System.Int32));
      taSchedule.Columns.Add("RemarkS", typeof(System.String));
      taSchedule.Columns.Add("UserPidS", typeof(System.Int32));
      taSchedule.Columns.Add("StartTimeS", typeof(System.DateTime));
      taSchedule.Columns.Add("EndTimeS", typeof(System.DateTime));
      taSchedule.Columns.Add("TimeS", typeof(System.Double));
      taSchedule.Columns.Add("StartTimeActual", typeof(System.DateTime));
      taSchedule.Columns.Add("EndTimeActual", typeof(System.DateTime));
      taSchedule.Columns.Add("TimeActual", typeof(System.Double));
      taSchedule.Columns.Add("PercentFinish", typeof(System.Double));
      taSchedule.Columns.Add("RealEndTime", typeof(System.DateTime));
      taSchedule.Columns.Add("FinishedS", typeof(System.Int32));
      taSchedule.Columns.Add("Cancelled", typeof(System.Int32));
      taSchedule.Columns.Add("IsUpdateS", typeof(System.Int32));
      taSchedule.Columns["IsUpdateS"].DefaultValue = 0;
      taSchedule.Columns["FinishedS"].DefaultValue = 0;
      taSchedule.Columns["Cancelled"].DefaultValue = 0;
      taSchedule.Columns["TimeS"].DefaultValue = 0;
      taSchedule.Columns["TimeActual"].DefaultValue = 0;
      ds.Tables.Add(taSchedule);

      ds.Relations.Add(new DataRelation("Project_Function",
            taProject.Columns["Pid"], taFunction.Columns["ProjectPid"], false));

      ds.Relations.Add(new DataRelation("Function_Schedule",
            new DataColumn[] { taFunction.Columns["ProjectPid"], taFunction.Columns["FuncPid"] },
            new DataColumn[] { taSchedule.Columns["ProjectPid"], taSchedule.Columns["FunctionPid"] }, false));      

      //ds.Relations.Add(new DataRelation("Schedule", taSchedule.Columns["ProjectPid"], taProject.Columns["Pid"], false));
            //new DataColumn[] { taFunction.Columns["ProjectPid"], taFunction.Columns["FuncPid"] },
            //new DataColumn[] { taSchedule.Columns["FunctionPid"], taFunction.Columns["FuncPid"] }));
      
      return ds;
    }
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[4];

      DateTime date;
      // DateFrom
      if (this.ultDateFrom.Value != null)
      {
        date = DBConvert.ParseDateTime(this.ultDateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        if (date != null && date != DateTime.MinValue)
        {
          param[0] = new DBParameter("@DateFrom", DbType.DateTime, date);
        }
      }

      // DateTo
      if (this.ultDateTo.Value != null)
      {
        date = DBConvert.ParseDateTime(this.ultDateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        if (date != null && date != DateTime.MinValue)
        {
          date = (date != DateTime.MaxValue) ? date.AddDays(1) : date;
          param[1] = new DBParameter("@DateTo", DbType.DateTime, date);
        }
      }

      // Status
      if (this.ultStatus.Value.ToString().Length > 0)
      {
        param[2] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(this.ultStatus.Value.ToString()));
      }
        
      //Request
      if (this.txtRequestOnl.Text.Length > 0)
      {
          param[3] = new DBParameter("@RequestOnl", DbType.String, this.txtRequestOnl.Text.Trim());
      }
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spGNRProject_SelectNew", 3000, param);
      //create dataset
      DataSet dsData = this.CreateDataSet();
      dsData.Tables["Project"].Merge(dsSource.Tables[0]);
      dsData.Tables["Function"].Merge(dsSource.Tables[1]);
      dsData.Tables["Schedule"].Merge(dsSource.Tables[2]);
      this.ultData.DataSource = dsData;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow gr = ultData.Rows[i];
        DateTime current = DateTime.Today.Date;
        DateTime Update = DBConvert.ParseDateTime(gr.Cells["UpdateDate"].Value.ToString(),Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        if (DBConvert.ParseDateTime(gr.Cells["DevIssueDate"].Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && Update != DateTime.MinValue)
        {
          TimeSpan value = current - Update;
          if (value.Days >= 0 && value.Days <= 2)
          {
            gr.CellAppearance.BackColor = Color.Yellow;
          }
        }
      }

        this.keys = long.MinValue;
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

      // Set Align

      e.Layout.Bands[0].Columns["TimeP"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["TimeF"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["TimeS"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["TimeActual"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[2].Columns["PercentFinish"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[2].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["RequestOnlinePid"].Hidden = true;
      e.Layout.Bands[0].Columns["Hold"].Hidden = true;
      e.Layout.Bands[0].Columns["HoldReason"].Hidden = true;
      e.Layout.Bands[0].Columns["Select"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Hidden = true;
      e.Layout.Bands[0].Columns["IsUpdateP"].Hidden = true;
      e.Layout.Bands[0].Columns["UpdateDate"].Hidden = true;

      e.Layout.Bands[1].Columns["FuncPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ProjectPid"].Hidden = true;
      e.Layout.Bands[1].Columns["IsUpdateF"].Hidden = true;

      e.Layout.Bands[2].Columns["TaskPid"].Hidden = true;
      e.Layout.Bands[2].Columns["ProjectPid"].Hidden = true;
      e.Layout.Bands[2].Columns["FunctionPid"].Hidden = true;
      e.Layout.Bands[2].Columns["Cancelled"].Hidden = true;
      e.Layout.Bands[2].Columns["IsUpdateS"].Hidden = true;
      //
      //Row Color
      e.Layout.Bands[0].Override.RowAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[1].Override.RowAppearance.BackColor = Color.LightCyan;
      e.Layout.Bands[2].Override.RowAppearance.BackColor = Color.LightGreen;

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[1].ColHeaderLines = 2;
      e.Layout.Bands[2].ColHeaderLines = 2;

      // Header
      e.Layout.Bands[0].Columns["RequestCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["RequestCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["StartTimeP"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["StartTimeP"].MinWidth = 70;
      e.Layout.Bands[0].Columns["EndTimeP"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["EndTimeP"].MinWidth = 70;
      e.Layout.Bands[0].Columns["IssueDate"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["IssueDate"].MinWidth = 70;
      e.Layout.Bands[0].Columns["RealEndTime"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["RealEndTime"].MinWidth = 70;
      e.Layout.Bands[0].Columns["DueDate"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["DueDate"].MinWidth = 70;
      e.Layout.Bands[0].Columns["TimeP"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["TimeP"].MinWidth = 70;
      e.Layout.Bands[0].Columns["RelativeDepartment"].Header.Caption = "Relative\n Department";
      e.Layout.Bands[0].Columns["StartTimeP"].Header.Caption = "Start Time";
      e.Layout.Bands[0].Columns["EndTimeP"].Header.Caption = "End Time";
      e.Layout.Bands[0].Columns["RealEndTime"].Header.Caption = "Real End\n Time";
      e.Layout.Bands[0].Columns["TimeP"].Header.Caption = "Time\n (Days)";
      e.Layout.Bands[0].Columns["DevIssueDate"].Header.Caption = "Developer\n Issue Date";

      e.Layout.Bands[1].Columns["NameProject"].Header.Caption = "Function Name";
      e.Layout.Bands[1].Columns["TimeF"].Header.Caption = "Time\n(Days)";
      e.Layout.Bands[1].Columns["UserPidF"].Header.Caption = "Person Incharge";
      e.Layout.Bands[1].Columns["FinishedF"].Header.Caption = "Finished";
      e.Layout.Bands[1].Columns["RemarkF"].Header.Caption = "Remark\nFunction";
      e.Layout.Bands[1].Columns["NameProject"].MaxWidth = 250;
      e.Layout.Bands[1].Columns["NameProject"].MinWidth = 250;
      e.Layout.Bands[1].Columns["TimeF"].MaxWidth = 50;
      e.Layout.Bands[1].Columns["TimeF"].MinWidth = 50;
      e.Layout.Bands[1].Columns["FinishedF"].MaxWidth = 70;
      e.Layout.Bands[1].Columns["FinishedF"].MinWidth = 70;
      e.Layout.Bands[1].Columns["UserPidF"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["UserPidF"].MinWidth = 100;

      e.Layout.Bands[2].Columns["ComponentPid"].Header.Caption = "Task Type";
      e.Layout.Bands[2].Columns["UserPidS"].Header.Caption = "Coder";
      e.Layout.Bands[2].Columns["StartTimeS"].Header.Caption = "Start Time";
      e.Layout.Bands[2].Columns["EndTimeS"].Header.Caption = "End Time";
      e.Layout.Bands[2].Columns["TimeS"].Header.Caption = "Time\n (Hour)";
      e.Layout.Bands[2].Columns["StartTimeActual"].Header.Caption = "Start Time\n Actual";
      e.Layout.Bands[2].Columns["EndTimeActual"].Header.Caption = "End Time\n Actual";
      e.Layout.Bands[2].Columns["TimeActual"].Header.Caption = "Time Actual\n (Hour)";
      e.Layout.Bands[2].Columns["UserPidS"].Header.Caption = "Coder";
      e.Layout.Bands[2].Columns["PercentFinish"].Header.Caption = "Percent\n Finished";
      e.Layout.Bands[2].Columns["RealEndTime"].Header.Caption = "Real End\n Time";
      e.Layout.Bands[2].Columns["FinishedS"].Header.Caption = "Finished";
      e.Layout.Bands[2].Columns["FinishedS"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["FinishedS"].MinWidth = 70;
      e.Layout.Bands[2].Columns["TimeS"].MaxWidth = 50;
      e.Layout.Bands[2].Columns["TimeS"].MinWidth = 50;
      e.Layout.Bands[2].Columns["TimeActual"].MaxWidth = 50;
      e.Layout.Bands[2].Columns["TimeActual"].MinWidth = 50;
      e.Layout.Bands[2].Columns["PercentFinish"].MaxWidth = 50;
      e.Layout.Bands[2].Columns["PercentFinish"].MinWidth = 50;
      e.Layout.Bands[2].Columns["StartTimeS"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["StartTimeS"].MinWidth = 70;
      e.Layout.Bands[2].Columns["StartTimeActual"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["StartTimeActual"].MinWidth = 70;
      e.Layout.Bands[2].Columns["EndTimeS"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["EndTimeS"].MinWidth = 70;
      e.Layout.Bands[2].Columns["RealEndTime"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["RealEndTime"].MinWidth = 70;
      e.Layout.Bands[2].Columns["EndTimeActual"].MaxWidth = 70;
      e.Layout.Bands[2].Columns["EndTimeActual"].MinWidth = 70;
      e.Layout.Bands[2].Columns["UserPidS"].MaxWidth = 100;
      e.Layout.Bands[2].Columns["UserPidS"].MinWidth = 100;
      e.Layout.Bands[2].Columns["ComponentPid"].MaxWidth = 100;
      e.Layout.Bands[2].Columns["ComponentPid"].MinWidth = 100;

      // Set dropdownlist for column
      e.Layout.Bands[0].Columns["Leader"].ValueList = ultdrpUserIT;
      e.Layout.Bands[1].Columns["UserPidF"].ValueList = ultdrpUserIT;
      e.Layout.Bands[2].Columns["UserPidS"].ValueList = ultdrpUserIT;
      e.Layout.Bands[0].Columns["Status"].ValueList = ultdrpStatus;
      e.Layout.Bands[2].Columns["ComponentPid"].ValueList = ultDDScreenTime;

      // Read only
      e.Layout.Bands[0].Columns["RealEndTime"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Project"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TimeP"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["RequestCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["TimeF"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["RealEndTime"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      // Set Column Style
      e.Layout.Bands[1].Columns["FinishedF"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["FinishedF"].DefaultCellValue = 0;
      e.Layout.Bands[2].Columns["FinishedS"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[2].Columns["FinishedS"].DefaultCellValue = 0;

      for (int i = 0; i < ultData.Rows.Count; i++)
      { 
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Status"].Value.ToString()) == 1
             || DBConvert.ParseInt(ultData.Rows[i].Cells["Status"].Value.ToString()) == 2)
        {
          ultData.Rows[i].Activation = Activation.ActivateOnly;
          
          for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            ultData.Rows[i].ChildBands[0].Rows[j].Activation = Activation.ActivateOnly;
            for (int k = 0; k < ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows.Count; k++)
            {
              ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[k].Activation = Activation.ActivateOnly;
            }
          }
        }
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["FinishedF"].Value.ToString()) == 1)
          {
            ultData.Rows[i].ChildBands[0].Rows[j].Activation = Activation.ActivateOnly;
          }
          for (int k = 0; k < ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows.Count; k++)
          {
            if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[k].Cells["FinishedS"].Value.ToString()) == 1)
            {
              ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[k].Activation = Activation.ActivateOnly;
            }
          }
        }

        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Status"].Value.ToString()) == 1)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightSeaGreen;
        }
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Status"].Value.ToString()) == 2)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGray;
        }
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Status"].Value.ToString()) == 3)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Orange;
        }
      }
      // Format date (dd/MM/yy)

      e.Layout.Bands[0].Columns["StartTimeP"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["StartTimeP"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["EndTimeP"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["EndTimeP"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["RealEndTime"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["RealEndTime"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["DueDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["DueDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["IssueDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["IssueDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["DevIssueDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["DevIssueDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      e.Layout.Bands[2].Columns["StartTimeS"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[2].Columns["StartTimeS"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[2].Columns["EndTimeS"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[2].Columns["EndTimes"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[2].Columns["StartTimeActual"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[2].Columns["StartTimeActual"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[2].Columns["EndTimeActual"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[2].Columns["EndTimeActual"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[2].Columns["RealEndTime"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[2].Columns["RealEndTime"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);


      e.Layout.Bands[0].Summaries.Add(Infragistics.Win.UltraWinGrid.SummaryType.Sum, e.Layout.Bands[0].Columns["TimeP"], Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[1].Summaries.Add(Infragistics.Win.UltraWinGrid.SummaryType.Sum, e.Layout.Bands[1].Columns["TimeF"], Infragistics.Win.UltraWinGrid.SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[1].Summaries[0].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands[1].Summaries[0].Appearance.TextHAlign = Infragistics.Win.HAlign.Right;

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
      SaveDataToSQL();
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
      // Delete
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if(DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1 &&
            DBConvert.ParseInt(row.Cells["Pid"].Value.ToString()) > 0)
        {
          // Input
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          // Output
          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          // Execute
          DataBaseAccess.ExecuteStoreProcedure("spGNRProject_Delete", inputParam, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            // Error
            WindowUtinity.ShowMessageError("ERR0004");
            return;
          }
          else 
          {
            // Success
            WindowUtinity.ShowMessageSuccess("MSG0002");
          }
        }
      }
      // Load Data
      this.LoadGrid();
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      string value = e.Cell.Value.ToString();
      switch (columnName)
      {
        case "requestonlinepid":
          if (value.Length > 0 && DBConvert.ParseInt(value) != int.MinValue)
          {
            // Load Project
            e.Cell.Row.Cells["Project"].Value = ultdrpRequest.SelectedRow.Cells["Project"].Value.ToString();
          }
          else
          {
            e.Cell.Row.Cells["Project"].Value = "";
          }
          break;
        case "nameproject":
          {
            if (DBConvert.ParseLong(row.Cells["FuncPid"].Value) < 0 && DBConvert.ParseLong(row.Cells["FuncPid"].Value) == long.MinValue)
            {

              row.Cells["FuncPid"].Value = this.keys + 1;
              this.keys = this.keys + 1;
            }
            row.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "remarkf":
          {
            row.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "timef":
          {
            row.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "userpidf":
          {
            row.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "finishedf":
          { 
            row.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "componentpid":
          {
            row.Cells["TimeS"].Value = ultDDScreenTime.SelectedRow.Cells["Time"].Value;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
            row.Cells["IsUpdateS"].Value = 1;
          }
          break;
        case "remarks":
          {
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
            row.Cells["IsUpdateS"].Value = 1;
          }
          break;
        case "userpids":
          {
            row.Cells["IsUpdateS"].Value = 1;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "starttimes":
          {
            row.Cells["IsUpdateS"].Value = 1;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "endtimes":
          {
            row.Cells["IsUpdateS"].Value = 1;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "times":
          {
            row.Cells["IsUpdateS"].Value = 1;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
            double sumtime = 0;
            for (int i = 0; i < row.ParentRow.ChildBands[0].Rows.Count; i++)
            {
              UltraGridRow r = row.ParentRow.ChildBands[0].Rows[i];
              sumtime += Math.Round((DBConvert.ParseDouble(r.Cells["TimeS"].Value.ToString()) / 7), 2);
            }
            row.ParentRow.Cells["TimeF"].Value = sumtime;
          }
          break;
        case "starttimeactual":
          {
            row.Cells["IsUpdateS"].Value = 1;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "endtimeactual":
          {
            row.Cells["IsUpdateS"].Value = 1;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "timeactual":
          {
            row.Cells["IsUpdateS"].Value = 1;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "percentfinish":
          {
            row.Cells["IsUpdateS"].Value = 1;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
          }
          break;
        case "finisheds":
          {
            row.Cells["IsUpdateS"].Value = 1;
            row.ParentRow.Cells["IsUpdateF"].Value = 1;
          }
          break;
        default:
          break;
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;
      this.Search();
      this.btnSearch.Enabled = true;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ultStatus.Value = DBNull.Value;
      this.ultDateFrom.Value = DBNull.Value;
      this.ultDateTo.Value = DBNull.Value;
      this.txtRequestOnl.Clear();
    }
    #endregion Event

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      string values = e.NewValue.ToString();
      switch (colName)
      {
        case "TimeF":
          if (DBConvert.ParseDouble(values) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Time");
            e.Cancel = true;
          }
          break;
        case "TimeS":
          if (DBConvert.ParseDouble(values) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Time");
            e.Cancel = true;
          }
          break;
        case "TimeActual":
          if (DBConvert.ParseDouble(values) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Time Actual");
            e.Cancel = true;
          }
          break;
        case "PercentFinish":
          if (DBConvert.ParseInt(values) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Percent Finished");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }
  }
}
