/*************************************
 * Author: Nguyen Ngoc Tien          *
 * Create date: 13/10/2014           *
 * Description: Define group process *
 * ***********************************/
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

namespace DaiCo.ERPProject
{

  public partial class viewWIP_96_002 : MainUserControl
  {
    public viewWIP_96_002()
    {
      InitializeComponent();
    }
    #region Init

    public long RoutingDefaultPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private bool isDuplicateProcess = false;

    #endregion

    #region Function
    /// <summary>
    /// hàm load danh sách team
    /// </summary>
    private void LoadDDTeam()
    {
      string commandText = "SELECT Pid, WorkAreaCode, WorkAreaName FROM VWIPCARSectionTeamInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDTeam.DataSource = dtSource;
      ultDDTeam.DisplayMember = "WorkAreaCode";
      ultDDTeam.ValueMember = "Pid";
      ultDDTeam.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDTeam.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// hàm load danh sách process
    /// </summary>
    private void LoadDDProcess()
    {
      string commandText = string.Format(@"SELECT	A.Pid, A.ProcessCode, A.ProcessNameEN, 0 AS Priority,
		                                              A.SetupTime, A.ProcessTime, A.Capacity, A.LeadTime1,
		                                              A.LeadTime2, A.LeadTime3, A.LeadTime4,
		                                              B.Pid WorkAreaCode,A.Remark 
                                           FROM TBlPLNProcessCarcass_ProcessInfo A 
                                                INNER JOIN VWIPCARSectionTeamInfo B ON A.TeamCodePid = B.Pid
                                           ORDER BY A.Pid ASC");
      DataTable dtSourcePro = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDProcess.DataSource = dtSourcePro;
      ultDDProcess.DisplayMember = "ProcessCode";
      ultDDProcess.ValueMember = "Pid";
      ultDDProcess.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDProcess.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["Priority"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["SetupTime"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["ProcessTime"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["Capacity"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["LeadTime1"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["LeadTime2"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["LeadTime3"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["LeadTime4"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["WorkAreaCode"].Hidden = true;
      ultDDProcess.DisplayLayout.Bands[0].Columns["Remark"].Hidden = true;
    }

    /// <summary>
    /// Hàm load lưới không có thông tin
    /// </summary>
    private void LoadUltraProcess()
    {
      string commandText = string.Format(@"SELECT	A.Pid, A.ProcessCode, A.ProcessNameEN, 0 AS Priority,
		                                              A.SetupTime, A.ProcessTime, A.Capacity, A.LeadTime1,
		                                              A.LeadTime2, A.LeadTime3, A.LeadTime4,
		                                              B.Pid WorkAreaCode,A.Remark 
                                           FROM TBlPLNProcessCarcass_ProcessInfo A 
                                                INNER JOIN VWIPCARSectionTeamInfo B ON A.TeamCodePid = B.Pid
                                           WHERE 0 = 1");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultData.DataSource = dtSource;
    }

    /// <summary>
    /// Load lưới có thông tin
    /// </summary>
    private void LoadData()
    {
      DBParameter[] param = new DBParameter[1];
      param[0] = new DBParameter("@RoutingDefaultPid", DbType.Int64, this.RoutingDefaultPid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNRoutingDefault_Select", param);
      ultData.DataSource = dsSource.Tables[1];
      txtGroupCode.Text = dsSource.Tables[0].Rows[0]["GroupCode"].ToString();
      txtGroupName.Text = dsSource.Tables[0].Rows[0]["GroupName"].ToString();
      txtRemark.Text = dsSource.Tables[0].Rows[0]["Remark"].ToString();
    }

    //private void AutoPriority()
    //{
    //  if (ultData.Rows.Count > 0)
    //  {
    //    for (int i = 0; i < ultData.Rows.Count; i++)
    //    {
    //      ultData.Rows[i].Cells["Priority"].Value = i + 1;
    //    }
    //  }
    //}

    /// <summary>
    /// HÀM CHECK PROCESS DUPLICATE
    /// </summary>
    private void CheckProcessDuplicate()
    {
      isDuplicateProcess = false;
      for (int k = 0; k < ultData.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultData.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowcurentA = ultData.Rows[i];
        long processCode = DBConvert.ParseLong(rowcurentA.Cells["ProcessCode"].Value.ToString());
        int priority = DBConvert.ParseInt(rowcurentA.Cells["Priority"].Value.ToString());
        for (int j = i + 1; j < ultData.Rows.Count; j++)
        {
          UltraGridRow rowcurentB = ultData.Rows[j];
          long processCodeCom = DBConvert.ParseLong(rowcurentB.Cells["ProcessCode"].Value.ToString());
          int priorityCom = DBConvert.ParseInt(rowcurentB.Cells["Priority"].Value.ToString());
          if (processCode == processCodeCom || priority == priorityCom)
          {
            rowcurentA.CellAppearance.BackColor = Color.Yellow;
            rowcurentB.CellAppearance.BackColor = Color.Yellow;
            isDuplicateProcess = true;
          }
        }
      }
    }

    private bool CheckValid()
    {
      DataTable dtCheck = (DataTable)ultData.DataSource;
      foreach (DataRow row in dtCheck.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          string processCode = row["ProcessCode"].ToString().Trim();
          if (processCode.Length == 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", "Process Code");
            return false;
          }
          double setupTime = DBConvert.ParseDouble(row["SetupTime"].ToString());
          if (setupTime <= 0 && setupTime != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Setup Time");
            return false;
          }
          double ProcessTime = DBConvert.ParseDouble(row["ProcessTime"].ToString());
          if (ProcessTime <= 0 && ProcessTime != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Process Time");
            return false;
          }
          double Capacity = DBConvert.ParseDouble(row["Capacity"].ToString());
          if (Capacity <= 0 && Capacity != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Capacity");
            return false;
          }
          int priority = DBConvert.ParseInt(row["Priority"].ToString());
          if (priority <= 0 && priority != int.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Priority");
            return false;
          }
          double leadTime1 = DBConvert.ParseDouble(row["LeadTime1"].ToString());
          if (leadTime1 <= 0 && leadTime1 != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Lead Time1");
            return false;
          }
          double leadTime2 = DBConvert.ParseDouble(row["LeadTime2"].ToString());
          if (leadTime2 <= 0 && leadTime2 != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Lead Time2");
            return false;
          }
          double leadTime3 = DBConvert.ParseDouble(row["LeadTime3"].ToString());
          if (leadTime3 <= 0 && leadTime3 != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Lead Time3");
            return false;
          }
          double leadTime4 = DBConvert.ParseDouble(row["LeadTime4"].ToString());
          if (leadTime4 <= 0 && leadTime4 != double.MinValue)
          {
            WindowUtinity.ShowMessageError("ERR0110", "Lead Time4");
            return false;
          }
        }
      }
      return true;
    }
    /// <summary>
    /// Save Master
    /// </summary>
    private bool SaveParent()
    {
      bool success = true;
      // 1. Insert/Update     
      DBParameter[] inputParam = new DBParameter[4];
      string cm = string.Format("SELECT [dbo].[FPLNGetNewRoutingDefaultCode]('{0}')", "RTD");
      inputParam[1] = new DBParameter("@GroupCode", DbType.String, DataBaseAccess.ExecuteScalarCommandText(cm).ToString());
      if (this.RoutingDefaultPid > 0 && this.RoutingDefaultPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@RoutingDefaultPid", DbType.Int64, this.RoutingDefaultPid);
        inputParam[1] = new DBParameter("@GroupCode", DbType.String, txtGroupCode.Text.Trim());
      }
      if (txtGroupName.TextLength > 0)
      {
        inputParam[2] = new DBParameter("@GroupName", DbType.String, txtGroupName.Text.Trim());
      }
      if (txtRemark.TextLength > 0)
      {
        inputParam[3] = new DBParameter("@Remark", DbType.String, txtRemark.Text.Trim());
      }
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure("spPLNRoutingDefault_Edit", inputParam, outputParam);
      if ((outputParam == null) || DBConvert.ParseInt(outputParam[0].Value.ToString()) <= 0)
      {
        success = false;
      }
      else
      {
        this.RoutingDefaultPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
      }
      if (success)
      {
        success = this.SaveChild(this.RoutingDefaultPid);
      }
      return success;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    private bool SaveChild(long RDefaultPid)
    {
      bool success = true;
      //delete
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNDeleteProcessInRouting", deleteParam, outputParam);
        if (DBConvert.ParseLong(outputParam[0].Value) <= 0)
        {
          success = false;
        }
      }
      DBParameter[] output = new DBParameter[1];
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        long RoutingDefaultDetailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        long RoutingDefaultPid = RDefaultPid;
        long ProcessCodePid = DBConvert.ParseLong(row.Cells["ProcessCode"].Value.ToString());
        int Priority = DBConvert.ParseInt(row.Cells["Priority"].Value.ToString());
        double Capacity = DBConvert.ParseDouble(row.Cells["Capacity"].Value.ToString());
        double Setup = DBConvert.ParseDouble(row.Cells["SetupTime"].Value.ToString());
        double Operation = DBConvert.ParseDouble(row.Cells["ProcessTime"].Value.ToString());
        double Lead1 = DBConvert.ParseDouble(row.Cells["LeadTime1"].Value.ToString());
        double Lead2 = DBConvert.ParseDouble(row.Cells["LeadTime2"].Value.ToString());
        double Lead3 = DBConvert.ParseDouble(row.Cells["LeadTime3"].Value.ToString());
        double Lead4 = DBConvert.ParseDouble(row.Cells["LeadTime4"].Value.ToString());
        int TeamCodePid = DBConvert.ParseInt(row.Cells["WorkAreaCode"].Value.ToString());
        string Remark = row.Cells["Remark"].Value.ToString().Trim();
        DBParameter[] input = new DBParameter[13];
        if (RoutingDefaultDetailPid != long.MinValue)
        {
          input[0] = new DBParameter("@RoutingDefaultDetailPid", DbType.Int64, RoutingDefaultDetailPid);
        }
        if (RoutingDefaultPid != long.MinValue)
        {
          input[1] = new DBParameter("@RoutingDefaultPid", DbType.Int64, RoutingDefaultPid);
        }
        if (ProcessCodePid != long.MinValue)
        {
          input[2] = new DBParameter("@ProcessCodePid", DbType.Int64, ProcessCodePid);
        }
        if (Priority != int.MinValue)
        {
          input[3] = new DBParameter("@Priority", DbType.Int32, Priority);
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
        if (TeamCodePid != int.MinValue)
        {
          input[11] = new DBParameter("@TeamCodePid", DbType.Int32, TeamCodePid);
        }
        if (Remark.Length > 0)
        {
          input[12] = new DBParameter("@Remark", DbType.String, 128, Remark);
        }
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spPLNRoutingDefaultDetail_Edit", input, output);

        if (output == null || DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
        {
          success = false;
        }
      }
      return success;
    }

    private void SaveData()
    {
      if (this.CheckValid())
      {
        bool success = true;
        if (this.SaveParent())
        {
          success = true;
        }
        else
        {
          success = false;
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "");
        this.SaveSuccess = false;
      }
    }
    #endregion

    #region Events


    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      //Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
      e.Layout.Bands[0].Columns["ProcessCode"].CellAppearance.TextHAlign = HAlign.Left;
      e.Layout.Bands[0].Columns["WorkAreaCode"].CellAppearance.TextHAlign = HAlign.Left;
      //
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      if (this.RoutingDefaultPid != long.MinValue)
      {
        e.Layout.Bands[0].Columns["RoutingDefaultPid"].Hidden = true;
      }
      e.Layout.Bands[0].Columns["ProcessNameEN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ProcessNameEN"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["ProcessNameEN"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ProcessNameEN"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ProcessCode"].ValueList = ultDDProcess;
      e.Layout.Bands[0].Columns["ProcessCode"].Header.Caption = "Process Code";
      e.Layout.Bands[0].Columns["ProcessNameEN"].Header.Caption = "Process Name";
      e.Layout.Bands[0].Columns["Priority"].Header.Caption = "Priority";
      e.Layout.Bands[0].Columns["SetupTime"].Header.Caption = "Setup Time(Minutes)";
      e.Layout.Bands[0].Columns["ProcessTime"].Header.Caption = "Process Time(Minutes)";
      e.Layout.Bands[0].Columns["Capacity"].Header.Caption = "Capacity";
      e.Layout.Bands[0].Columns["Capacity"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Capacity"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LeadTime1"].Header.Caption = "Lead Time\n<= 2pcs";
      e.Layout.Bands[0].Columns["LeadTime2"].Header.Caption = "Lead Time\n<= 6pcs";
      e.Layout.Bands[0].Columns["LeadTime3"].Header.Caption = "Lead Time\n<= 12pcs";
      e.Layout.Bands[0].Columns["LeadTime4"].Header.Caption = "Lead Time\n> 12pcs";
      e.Layout.Bands[0].Columns["WorkAreaCode"].ValueList = ultDDTeam;
      e.Layout.Bands[0].Columns["WorkAreaCode"].Header.Caption = "Team Code";
      e.Layout.Bands[0].Columns["WorkAreaCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WorkAreaCode"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Remark";
    }

    private void ultData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      switch (colName)
      {
        case "ProcessCode":
          if (ultDDProcess.SelectedRow != null)
          {
            e.Cell.Row.Cells["ProcessNameEN"].Value = ultDDProcess.SelectedRow.Cells["ProcessNameEN"].Value;
            //e.Cell.Row.Cells["Priority"].Value = ultDDProcess.SelectedRow.Cells["Priority"].Value;
            e.Cell.Row.Cells["Priority"].Value = e.Cell.Row.Index + 1;
            e.Cell.Row.Cells["SetupTime"].Value = ultDDProcess.SelectedRow.Cells["SetupTime"].Value;
            e.Cell.Row.Cells["ProcessTime"].Value = ultDDProcess.SelectedRow.Cells["ProcessTime"].Value;
            e.Cell.Row.Cells["Capacity"].Value = ultDDProcess.SelectedRow.Cells["Capacity"].Value;
            e.Cell.Row.Cells["LeadTime1"].Value = ultDDProcess.SelectedRow.Cells["LeadTime1"].Value;
            e.Cell.Row.Cells["LeadTime2"].Value = ultDDProcess.SelectedRow.Cells["LeadTime2"].Value;
            e.Cell.Row.Cells["LeadTime3"].Value = ultDDProcess.SelectedRow.Cells["LeadTime3"].Value;
            e.Cell.Row.Cells["LeadTime4"].Value = ultDDProcess.SelectedRow.Cells["LeadTime4"].Value;
            e.Cell.Row.Cells["WorkAreaCode"].Value = ultDDProcess.SelectedRow.Cells["WorkAreaCode"].Value;
            e.Cell.Row.Cells["Remark"].Value = ultDDProcess.SelectedRow.Cells["Remark"].Value;
            btnSave.Enabled = true;
          }
          else
          {
            e.Cell.Row.Cells["ProcessNameEN"].Value = DBNull.Value;
            e.Cell.Row.Cells["Priority"].Value = DBNull.Value;
            e.Cell.Row.Cells["SetupTime"].Value = DBNull.Value;
            e.Cell.Row.Cells["ProcessTime"].Value = DBNull.Value;
            e.Cell.Row.Cells["Capacity"].Value = DBNull.Value;
            e.Cell.Row.Cells["LeadTime1"].Value = DBNull.Value;
            e.Cell.Row.Cells["LeadTime2"].Value = DBNull.Value;
            e.Cell.Row.Cells["LeadTime3"].Value = DBNull.Value;
            e.Cell.Row.Cells["LeadTime4"].Value = DBNull.Value;
            e.Cell.Row.Cells["WorkAreaCode"].Value = DBNull.Value;
            e.Cell.Row.Cells["Remark"].Value = DBNull.Value;
          }
          //Auto priority
          //UltraGridRow row = e.Cell.Row;
          //int priority = DBConvert.ParseInt(row.Cells["Priority"].Value.ToString());
          //if (priority != int.MinValue)
          //{
          //  priority = priority + 1;
          //  for (int i = 0; i < ultData.Rows.Count; i++)
          //  {
          //    UltraGridRow rownext = ultData.Rows[i];
          //    if (rownext != row && DBConvert.ParseInt(ultData.Rows[i].Cells["Priority"].Value.ToString()) == priority)
          //    {
          //      priority = priority + 1;
          //    }

          //  }
          //  row.Cells["Priority"].Value = priority;
          //}
          //
          //this.AutoPriority();
          //
          this.CheckProcessDuplicate();
          break;
        case "Priority":
          this.CheckProcessDuplicate();
          break;
        default:
          break;
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count <= 0)
      {
        WindowUtinity.ShowMessageWarning("WRN0001");
        return;
      }
      if (this.isDuplicateProcess)
      {
        WindowUtinity.ShowMessageError("ERR0013", "Process Code or Priority");
        return;
      }
      this.SaveData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_BeforeCellUpdate(object sender, Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "Priority":
          if (DBConvert.ParseDouble(value) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Priority");
            e.Cancel = true;
          }
          break;
        ////
        case "LeadTime1":
          if (DBConvert.ParseDouble(value) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "LeadTime1");
            e.Cancel = true;
          }
          break;
        case "LeadTime2":
          if (DBConvert.ParseDouble(value) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "LeadTime2");
            e.Cancel = true;
          }
          break;
        case "LeadTime3":
          if (DBConvert.ParseDouble(value) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "LeadTime3");
            e.Cancel = true;
          }
          break;
        case "LeadTime4":
          if (DBConvert.ParseDouble(value) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "LeadTime4");
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
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
    #endregion

    private void viewWIP_96_002_Load(object sender, EventArgs e)
    {
      if (RoutingDefaultPid != long.MinValue)
      {
        LoadData();
        LoadDDTeam();
        LoadDDProcess();
      }
      else
      {
        txtGroupCode.Text = DataBaseAccess.ExecuteScalarCommandText(string.Format("select [dbo].[FPLNGetNewRoutingDefaultCode]('{0}')", "RTD")).ToString();
        LoadDDTeam();
        LoadDDProcess();
        LoadUltraProcess();
        btnSave.Enabled = false;
      }
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      //this.AutoPriority();
      this.CheckProcessDuplicate();
    }
  }
}
