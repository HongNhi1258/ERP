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
  public partial class viewHRD_01_002 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private IList listDeletedPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private bool isDuplicateProcess = false;
    private long pidtemp = -1;

    #endregion Field

    #region Init
    public viewHRD_01_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewHRD_01_002_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[4];
      if (ucbDivision.Value != null)
      {
        input[0] = new DBParameter("@Division", DbType.Int32, DBConvert.ParseInt(ucbDivision.Value.ToString()));
      }

      if (ucbDepartment.Value != null)
      {
        input[1] = new DBParameter("@Department", DbType.Int32, DBConvert.ParseInt(ucbDepartment.Value.ToString()));
      }
      if (ucbSection.Value != null)
      {
        input[2] = new DBParameter("@Section", DbType.Int32, DBConvert.ParseInt(ucbSection.Value.ToString()));
      }
      if (ucbTeam.Value != null)
      {
        input[3] = new DBParameter("@Team", DbType.Int32, DBConvert.ParseInt(ucbTeam.Value.ToString()));
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spHRDDBDepartmentSectionTeam_Search", input);
      ds.Relations.Add(new DataRelation("Parent_Child", ds.Tables[0].Columns["Pid"], ds.Tables[1].Columns["DepartmentPid"], false));
      ds.Relations.Add(new DataRelation("Child_Grand", ds.Tables[1].Columns["Pid"], ds.Tables[2].Columns["SectionPid"], false));
      if (ds != null)
      {
        ultData.DataSource = ds;
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
      this.LoadDevision();
      this.LoadManager();
      this.LoadSection();
      this.LoadTeam();
    }
    private void LoadDevision()
    {
      string commandText = string.Format(@"SELECT Pid, DivisionCode + ' - ' + ENDivisionName Division FROM TblHRDDBDivision WHERE IsDeleted = 0 ORDER BY DivisionCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbDivision, dtSource, "Pid", "Division", "Pid");
      ucbDivision.DisplayLayout.Bands[0].Columns["Division"].Width = 150;

    }
    private void LoadDepartment()
    {
      string commandText = string.Format(@"SELECT Pid, DepartmentCode + ' - ' + ENDepartmentName Department FROM TblHRDDBDepartment WHERE IsDeleted = 0 ORDER BY DepartmentCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbDepartment, dtSource, "Pid", "Department", "Pid");
      ucbDepartment.DisplayLayout.Bands[0].Columns["Department"].Width = 200;
    }
    private void LoadSection()
    {
      if (ucbDepartment.Value != null)
      {
        string commandText = string.Format(@"SELECT Pid, SectionCode + ' - ' + ENSectionName Section FROM TblHRDDBSection WHERE DepartmentPid = {0} AND IsDeleted = 0 ORDER BY SectionCode", ucbDepartment.Value);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbSection, dtSource, "Pid", "Section", "Pid");
      }
    }
    private void LoadTeam()
    {
      if (ucbSection.Value != null)
      {
        string commandText = string.Format(@"SELECT Pid, TeamCode + ' - ' + ENTeamName Team FROM TblHRDDBTeam WHERE SectionPid = {0} AND IsDeleted = 0 ORDER BY TeamCode", ucbSection.Value);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbTeam, dtSource, "Pid", "Team", "Pid");
      }
    }
    private void LoadManager()
    {
      string commandText = string.Format(@"SELECT EID, CAST(EID AS VARCHAR) + ' - ' + EmpName Name FROM TblHRDEmployee ORDER BY EID");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbManger, dtSource, "EID", "Name", "EID");
      ucbManger.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
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
    private void ucbDepartment_ValueChanged(object sender, EventArgs e)
    {
      this.LoadSection();
    }

    private void ucbSection_ValueChanged(object sender, EventArgs e)
    {
      this.LoadTeam();
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[2].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[2].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["Division"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[0].Columns["Division"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[0].Columns["Manager"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[0].Columns["Manager"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[1].Columns["Manager"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[1].Columns["Manager"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[2].Columns["Manager"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[2].Columns["Manager"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["IsUpdate"].Hidden = true;
      e.Layout.Bands[2].Columns["IsUpdate"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[2].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["DepartmentPid"].Hidden = true;
      e.Layout.Bands[2].Columns["SectionPid"].Hidden = true;

      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateBy"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["UpdateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["UpdateBy"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[2].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["UpdateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[2].Columns["UpdateBy"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["DepartmentCode"].Header.Caption = "Department Code";
      e.Layout.Bands[0].Columns["ENDepartmentName"].Header.Caption = "EN Department Name";
      e.Layout.Bands[0].Columns["VNDepartmentName"].Header.Caption = "VN Department Name";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["UpdateBy"].Header.Caption = "Update By";
      e.Layout.Bands[0].Columns["UpdateDate"].Header.Caption = "Update Date";

      e.Layout.Bands[1].Columns["SectionCode"].Header.Caption = "Section Code";
      e.Layout.Bands[1].Columns["ENSectionName"].Header.Caption = "EN Section Name";
      e.Layout.Bands[1].Columns["VNSectionName"].Header.Caption = "VN Section Name";
      e.Layout.Bands[1].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[1].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[1].Columns["UpdateBy"].Header.Caption = "Update By";
      e.Layout.Bands[1].Columns["UpdateDate"].Header.Caption = "Update Date";

      e.Layout.Bands[2].Columns["TeamCode"].Header.Caption = "Team Code";
      e.Layout.Bands[2].Columns["ENTeamName"].Header.Caption = "EN Team Name";
      e.Layout.Bands[2].Columns["VNTeamName"].Header.Caption = "VN Team Name";
      e.Layout.Bands[2].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[2].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[2].Columns["UpdateBy"].Header.Caption = "Update By";
      e.Layout.Bands[2].Columns["UpdateDate"].Header.Caption = "Update Date";

      e.Layout.Bands[0].Columns["Division"].ValueList = ucbDivision;
      e.Layout.Bands[0].Columns["Manager"].ValueList = ucbManger;
      e.Layout.Bands[1].Columns["Manager"].ValueList = ucbManger;
      e.Layout.Bands[2].Columns["Manager"].ValueList = ucbManger;

      e.Layout.Bands[0].Columns["Division"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Division"].MinWidth = 150;
      e.Layout.Bands[0].Columns["Manager"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Manager"].MinWidth = 150;
      e.Layout.Bands[0].Columns["DepartmentCode"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["DepartmentCode"].MinWidth = 150;
      e.Layout.Bands[0].Columns["CreateBy"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["CreateBy"].MinWidth = 150;
      e.Layout.Bands[0].Columns["UpdateBy"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["UpdateBy"].MinWidth = 150;
      e.Layout.Bands[0].Columns["CreateDate"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["CreateDate"].MinWidth = 150;
      e.Layout.Bands[0].Columns["UpdateDate"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["UpdateDate"].MinWidth = 150;

      e.Layout.Bands[1].Columns["Manager"].MaxWidth = 200;
      e.Layout.Bands[1].Columns["Manager"].MinWidth = 200;
      e.Layout.Bands[1].Columns["SectionCode"].MaxWidth = 150;
      e.Layout.Bands[1].Columns["SectionCode"].MinWidth = 150;

      e.Layout.Bands[2].Columns["Manager"].MaxWidth = 200;
      e.Layout.Bands[2].Columns["Manager"].MinWidth = 200;
      e.Layout.Bands[2].Columns["TeamCode"].MaxWidth = 150;
      e.Layout.Bands[2].Columns["TeamCode"].MinWidth = 150;

      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[2].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[2].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // CHECK DUPLICATE DATA
      if (this.isDuplicateProcess)
      {
        WindowUtinity.ShowMessageError("ERR0013", "SectionCode or TeamCode");
        return;
      }
      //Check valid
      bool success = CheckData(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        // Load Data
        this.Search();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

    }

    /// <summary>
    /// Check Valid Detail
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckData(out string message)
    {
      message = string.Empty;
      DataTable dt = (DataTable)((DataSet)ultData.DataSource).Tables[0];
      DataTable dt1 = (DataTable)((DataSet)ultData.DataSource).Tables[1];
      DataTable dt2 = (DataTable)((DataSet)ultData.DataSource).Tables[2];
      if (dt != null && dt.Rows.Count > 0)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          int division = DBConvert.ParseInt(row.Cells["Division"].Value.ToString());
          string department = row.Cells["DepartmentCode"].Value.ToString();
          string nameDepartment = row.Cells["ENDepartmentName"].Value.ToString();
          if (division < 0)
          {
            row.Appearance.BackColor = Color.Yellow;
            message = "Division is invalid ";
            return false;
          }
          if (department.Length <= 0)
          {
            row.Appearance.BackColor = Color.Yellow;
            message = "DepartmentCode is invalid ";
            return false;
          }
          if (nameDepartment.Length <= 0)
          {
            row.Appearance.BackColor = Color.Yellow;
            message = "Department Name is invalid ";
            return false;
          }
          if (dt1 != null && dt1.Rows.Count > 0)
          {
            for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
            {
              UltraGridRow row1 = ultData.Rows[i].ChildBands[0].Rows[j];
              string section = row1.Cells["SectionCode"].Value.ToString();
              string nameSection = row1.Cells["ENSectionName"].Value.ToString();
              if (section.Length < 0)
              {
                row1.Appearance.BackColor = Color.Yellow;
                message = "SectionCode is invalid ";
                return false;
              }
              if (nameSection.Length <= 0)
              {
                row1.Appearance.BackColor = Color.Yellow;
                message = "Section Name is invalid ";
                return false;
              }

              if (dt2 != null && dt2.Rows.Count > 0)
              {
                for (int k = 0; k < ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows.Count; k++)
                {
                  UltraGridRow row2 = ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[k];
                  string team = row2.Cells["TeamCode"].Value.ToString();
                  string nameTeam = row2.Cells["ENTeamName"].Value.ToString();
                  if (team.Length <= 0)
                  {
                    row2.Appearance.BackColor = Color.Yellow;
                    message = "TeamCode is invalid ";
                    return false;
                  }
                  if (nameTeam.Length <= 0)
                  {
                    row2.Appearance.BackColor = Color.Yellow;
                    message = "Team Name is invalid ";
                    return false;
                  }
                }
              }
            }
          }
        }
      }
      return true;
    }
    /// <summary>
    /// HÀM CHECK PROCESS DUPLICATE
    /// </summary>
    private void CheckProcessDuplicate()
    {
      isDuplicateProcess = false;
      //for (int k = 0; k < ultData.Rows.Count; k++)
      //{
      //  for (int n = 0; n < ultData.Rows[k].ChildBands[1].Rows.Count; n++)
      //  {
      //    UltraGridRow rowcurent = ultData.Rows[k].ChildBands[1].Rows[n];
      //    rowcurent.CellAppearance.BackColor = Color.Empty;
      //  }
      //}
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowcurentA = ultData.Rows[i].ChildBands[0].Rows[j];
          string secton1 = rowcurentA.Cells["SectionCode"].Value.ToString();
          for (int x = j + 1; x < ultData.Rows[i].ChildBands[0].Rows.Count; x++)
          {
            UltraGridRow rowcurentB = ultData.Rows[i].ChildBands[0].Rows[x];
            string section2 = rowcurentB.Cells["SectionCode"].Value.ToString();
            if (secton1 == section2)
            {
              rowcurentA.CellAppearance.BackColor = Color.Yellow;
              rowcurentB.CellAppearance.BackColor = Color.Yellow;
              isDuplicateProcess = true;
            }
          }
          for (int h = 0; h < ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows.Count; h++)
          {
            UltraGridRow rowcurent1 = ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[h];
            string team1 = rowcurent1.Cells["TeamCode"].Value.ToString();
            for (int a = h + 1; a < ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows.Count; a++)
            {
              UltraGridRow rowcurent2 = ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[a];
              string team2 = rowcurent2.Cells["TeamCode"].Value.ToString();
              if (team1 == team2)
              {
                rowcurent1.CellAppearance.BackColor = Color.Yellow;
                rowcurent2.CellAppearance.BackColor = Color.Yellow;
                isDuplicateProcess = true;
              }
            }
          }
        }

      }
    }
    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool flag = true;
      // 1. Delete      
      //string strDelete = "";
      //foreach (long pidDelete in this.listDeletedPid)
      //{
      //  if (pidDelete > 0)
      //  {
      //    strDelete += pidDelete.ToString() + ",";
      //  }
      //}
      //if (strDelete.Length > 0)
      //{
      //  strDelete = "," + strDelete;
      //  DBParameter[] input = new DBParameter[1];
      //  input[0] = new DBParameter("@DeleteList", DbType.String, 4000, strDelete);
      //  DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      //  DataBaseAccess.ExecuteStoreProcedure("spHRDDBDepartmentSectionTeam_Delete", input, output);
      //  long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      //  if (resultSave == 0)
      //  {
      //    flag = false;
      //  }
      //}
      //2. Insert/ Update
      DataTable dtLevel1 = ((DataSet)ultData.DataSource).Tables[0];
      foreach (DataRow rowLevel1 in dtLevel1.Rows)
      {
        if (rowLevel1.RowState != DataRowState.Deleted)
        {
          int departmentPid = DBConvert.ParseInt(rowLevel1["Pid"]);
          // Insert, Update level 1
          if (rowLevel1.RowState == DataRowState.Added || rowLevel1.RowState == DataRowState.Modified)
          {
            DBParameter[] input = new DBParameter[7];
            if (rowLevel1.RowState == DataRowState.Modified)
            {
              input[0] = new DBParameter("@Pid", DbType.Int32, departmentPid);
            }
            input[1] = new DBParameter("@DepartmentCode", DbType.String, rowLevel1["DepartmentCode"]);
            input[2] = new DBParameter("@ENDepartmentName", DbType.String, rowLevel1["ENDepartmentName"]);
            input[3] = new DBParameter("@VNDepartmentName", DbType.String, rowLevel1["VNDepartmentName"]);
            input[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            if (rowLevel1["Manager"].ToString().Length != 0)
            {
              input[5] = new DBParameter("@Manager", DbType.Int64, DBConvert.ParseLong(rowLevel1["Manager"]));
            }
            input[6] = new DBParameter("@Division", DbType.Int32, DBConvert.ParseInt(rowLevel1["Division"]));
            DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spHRDDBDepartmentInfo_Edit", input, output);
            int resultSave = DBConvert.ParseInt(output[0].Value.ToString());
            if (resultSave == 0)
            {
              flag = false;
              break;
            }
            departmentPid = resultSave;
          }
          DataTable dtLevel2 = ((DataSet)ultData.DataSource).Tables[1];
          foreach (DataRow rowLevel2 in dtLevel2.Select(string.Format("DepartmentPid = {0}", rowLevel1["Pid"])))
          {
            if (rowLevel2.RowState != DataRowState.Deleted)
            {
              // Insert, Update level 2
              int sectionPid = DBConvert.ParseInt(rowLevel2["Pid"]);
              if (rowLevel2.RowState == DataRowState.Added || rowLevel2.RowState == DataRowState.Modified)
              {
                DBParameter[] input1 = new DBParameter[7];
                if (rowLevel2.RowState == DataRowState.Modified)
                {
                  input1[0] = new DBParameter("@Pid", DbType.Int32, sectionPid);
                }
                input1[1] = new DBParameter("@SectionCode", DbType.String, rowLevel2["SectionCode"]);
                input1[2] = new DBParameter("@ENSectionName", DbType.String, rowLevel2["ENSectionName"]);
                input1[3] = new DBParameter("@VNSectionName", DbType.String, rowLevel2["VNSectionName"]);
                input1[4] = new DBParameter("@CreateBy", DbType.Int64, SharedObject.UserInfo.UserPid);
                if (rowLevel2["Manager"].ToString().Length != 0)
                {
                  input1[5] = new DBParameter("@ManagerID", DbType.Int64, DBConvert.ParseLong(rowLevel2["Manager"]));
                }
                input1[6] = new DBParameter("@DepartmentPid", DbType.Int32, departmentPid);
                DBParameter[] output1 = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
                DataBaseAccess.ExecuteStoreProcedure("spHRDDBSectionInfo_Edit", input1, output1);
                int resultSave2 = DBConvert.ParseInt(output1[0].Value.ToString());
                if (resultSave2 == 0)
                {
                  flag = false;
                  break;
                }
                sectionPid = resultSave2;
              }
              DataTable dtLevel3 = ((DataSet)ultData.DataSource).Tables[2];
              foreach (DataRow rowLevel3 in dtLevel3.Select(string.Format("SectionPid = {0}", rowLevel2["Pid"])))
              {
                // Insert, Update level 3
                int teamPid = DBConvert.ParseInt(rowLevel3["Pid"]);
                if (rowLevel3.RowState == DataRowState.Added || rowLevel3.RowState == DataRowState.Modified)
                {
                  DBParameter[] input = new DBParameter[7];
                  if (rowLevel3.RowState == DataRowState.Modified)
                  {
                    input[0] = new DBParameter("@Pid", DbType.Int32, teamPid);
                  }
                  input[1] = new DBParameter("@TeamCode", DbType.String, rowLevel3["TeamCode"]);
                  input[2] = new DBParameter("@ENTeamName", DbType.String, rowLevel3["ENTeamName"]);
                  input[3] = new DBParameter("@VNTeamName", DbType.String, rowLevel3["VNTeamName"]);
                  input[4] = new DBParameter("@CreateBy", DbType.Int64, SharedObject.UserInfo.UserPid);
                  if (rowLevel3["Manager"].ToString().Length != 0)
                  {
                    input[5] = new DBParameter("@ManagerID", DbType.Int64, DBConvert.ParseLong(rowLevel3["Manager"]));
                  }
                  input[6] = new DBParameter("@SectionPid", DbType.Int32, sectionPid);
                  DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
                  DataBaseAccess.ExecuteStoreProcedure("spHRDDBTeamInfo_Edit", input, output);
                  int resultSave3 = DBConvert.ParseInt(output[0].Value.ToString());
                  if (resultSave3 == 0)
                  {
                    flag = false;
                    break;
                  }
                }
              }
            }
          }
        }
      }
      return flag;
    }

    private void ultData_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long detailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (detailPid != long.MinValue)
        {
          listDeletingPid.Add(detailPid);
        }
      }

    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        listDeletedPid.Add(pid);
      }
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
      ucbDepartment.Text = string.Empty;
      ucbSection.Text = string.Empty;
      ucbDivision.Text = string.Empty;
      ucbTeam.Text = string.Empty;
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultData, "Data");
    }
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      //UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "DepartmentCode":
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) == long.MinValue)
            {
              e.Cell.Row.Cells["Pid"].Value = pidtemp;
              pidtemp += -1;
            }
          }
          break;
        case "SectionCode":
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) == long.MinValue)
            {
              e.Cell.Row.Cells["Pid"].Value = pidtemp;
              pidtemp += -2;
            }
          }
          break;
        default:
          break;
      }
      this.CheckProcessDuplicate();
      //if(e.Cell.Row.ParentRow != null)
      //{
      //  e.Cell.Row.Cells["IsUpdate"].Value = 1;
      //if (e.Cell.Row.ParentRow.ChildBands[0].Rows.Count > 0)
      //{
      //  for (int k = 0; k < e.Cell.Row.ParentRow.ChildBands[0].Rows.Count; k++)
      //  {
      //    e.Cell.Row.ParentRow.ChildBands[0].Rows[k].Cells["IsUpdate"].Value = 1;

      //    if (e.Cell.Row.ParentRow.ChildBands[0].Rows[k].ChildBands[0].Rows.Count > 0)
      //    {
      //      for (int j = 0; j < e.Cell.Row.ParentRow.ChildBands[0].Rows[k].ChildBands[0].Rows.Count; j++)
      //      {
      //        e.Cell.Row.ParentRow.ChildBands[0].Rows[k].ChildBands[0].Rows[j].Cells["IsUpdate"].Value = 1;
      //      }
      //    }
      //  }
      //}
      //}   
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion Event
  }
}
