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
  public partial class viewHRD_01_001 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private IList listDeletedPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private bool isDuplicateProcess = false;
    private long pidtemp = -1;

    #endregion Field

    #region Init
    public viewHRD_01_001()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewHRD_01_001_Load(object sender, EventArgs e)
    {
      this.SetAutoSearchWhenPressEnter(gpbSearch);
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

      DBParameter[] input = new DBParameter[2];
      if (ucbBranch.Value != null)
      {
        input[0] = new DBParameter("@Branch", DbType.Int32, DBConvert.ParseInt(ucbBranch.Value.ToString()));
      }
      if (ucbDivision.Value != null)
      {
        input[1] = new DBParameter("@Division", DbType.Int32, DBConvert.ParseInt(ucbDivision.Value.ToString()));
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spHRDDBBranchDivision_Search", input);
      ds.Relations.Add(new DataRelation("TblMaster_TblDetail", ds.Tables[0].Columns["Pid"], ds.Tables[1].Columns["BranchPid"], false));
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
      this.LoadBranch();
      this.LoadDevision();
      this.LoadManager();
    }

    private void LoadBranch()
    {
      string commandText = string.Format(@"SELECT Pid, BranchCode + ' - ' + ENBranchName Branch FROM TblHRDDBBranch WHERE IsDeleted = 0 ORDER BY BranchCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbBranch, dtSource, "Pid", "Branch", "Pid");
    }
    private void LoadDevision()
    {
      if (ucbBranch.Value != null)
      {
        string commandText = string.Format(@"SELECT Pid, DivisionCode + ' - ' + ENDivisionName Division FROM TblHRDDBDivision WHERE BranchPid = {0} AND IsDeleted = 0 ORDER BY DivisionCode", ucbBranch.Value);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbDivision, dtSource, "Pid", "Division", "Pid");
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

      e.Layout.Bands[0].Columns["Manager"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[0].Columns["Manager"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      e.Layout.Bands[1].Columns["Manager"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
      e.Layout.Bands[1].Columns["Manager"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["IsUpdate"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["BranchPid"].Hidden = true;

      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateBy"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["UpdateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["UpdateBy"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["BranchCode"].Header.Caption = "Branch Code";
      e.Layout.Bands[0].Columns["ENBranchName"].Header.Caption = "EN Branch Name";
      e.Layout.Bands[0].Columns["VNBranchName"].Header.Caption = "VN Branch Name";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["UpdateBy"].Header.Caption = "Update By";
      e.Layout.Bands[0].Columns["UpdateDate"].Header.Caption = "Update Date";

      e.Layout.Bands[1].Columns["DivisionCode"].Header.Caption = "Division Code";
      e.Layout.Bands[1].Columns["ENDivisionName"].Header.Caption = "EN Division Name";
      e.Layout.Bands[1].Columns["VNDivisionName"].Header.Caption = "VN Division Name";
      e.Layout.Bands[1].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[1].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[1].Columns["UpdateBy"].Header.Caption = "Update By";
      e.Layout.Bands[1].Columns["UpdateDate"].Header.Caption = "Update Date";

      e.Layout.Bands[0].Columns["Manager"].ValueList = ucbManger;
      e.Layout.Bands[1].Columns["Manager"].ValueList = ucbManger;

      e.Layout.Bands[0].Columns["Manager"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Manager"].MinWidth = 150;
      e.Layout.Bands[0].Columns["BranchCode"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["BranchCode"].MinWidth = 200;
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
      e.Layout.Bands[1].Columns["DivisionCode"].MaxWidth = 200;
      e.Layout.Bands[1].Columns["DivisionCode"].MinWidth = 200;

      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message;
      // CHECK DUPLICATE DATA
      if (this.isDuplicateProcess)
      {
        WindowUtinity.ShowMessageError("ERR0013", "Division Code");
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
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool flag = true;
      // 1. Delete      
      string strDelete = "";
      foreach (long pidDelete in this.listDeletedPid)
      {
        if (pidDelete > 0)
        {
          strDelete += pidDelete.ToString() + ",";
        }
      }
      if (strDelete.Length > 0)
      {
        strDelete = "," + strDelete;
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@DeleteList", DbType.String, 4000, strDelete);
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spHRDDBBranchDivision_Delete", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }
      }
      //2. Insert/ Update
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        long pid = DBConvert.ParseLong(ultData.Rows[i].Cells["Pid"].Value.ToString());
        DBParameter[] input = new DBParameter[6];
        if (pid > 0)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, pid);
        }
        input[1] = new DBParameter("@BranchCode", DbType.String, ultData.Rows[i].Cells["BranchCode"].Value.ToString());
        input[2] = new DBParameter("@ENBranchName", DbType.String, ultData.Rows[i].Cells["ENBranchName"].Value.ToString());
        input[3] = new DBParameter("@VNBranchName", DbType.String, ultData.Rows[i].Cells["VNBranchName"].Value.ToString());
        input[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (ultData.Rows[i].Cells["Manager"].Value.ToString().Length != 0)
        {
          input[5] = new DBParameter("@Manager", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["Manager"].Value.ToString()));
        }
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spHRDDBBranchInfo_Edit", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }
        else
        {
          for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            int isUpdate = DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["IsUpdate"].Value);
            long detailPid = DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Pid"].Value.ToString());
            DBParameter[] input2 = new DBParameter[7];
            if (isUpdate == 1)
            {
              if (detailPid > 0)
              {
                input2[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
              }
              input2[1] = new DBParameter("@DivisionCode", DbType.String, ultData.Rows[i].ChildBands[0].Rows[j].Cells["DivisionCode"].Value.ToString());
              input2[2] = new DBParameter("@ENDivisionName", DbType.String, ultData.Rows[i].ChildBands[0].Rows[j].Cells["ENDivisionName"].Value.ToString());
              input2[3] = new DBParameter("@VNDivisionName", DbType.String, ultData.Rows[i].ChildBands[0].Rows[j].Cells["VNDivisionName"].Value.ToString());
              if (ultData.Rows[i].ChildBands[0].Rows[j].Cells["Manager"].Value.ToString().Length != 0)
              {
                input2[4] = new DBParameter("@ManagerID", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Manager"].Value.ToString()));
              }
              input2[5] = new DBParameter("@BranchPid", DbType.Int32, resultSave);
              input2[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              DBParameter[] output2 = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
              DataBaseAccess.ExecuteStoreProcedure("spHRDDBDivisionInfo_Edit", input2, output);
              long resultSave2 = DBConvert.ParseLong(output[0].Value.ToString());
              if (resultSave2 == 0)
              {
                flag = false;
              }
            }
          }
        }
      }
      return flag;
    }
    /// <summary>
    /// HÀM CHECK PROCESS DUPLICATE
    /// </summary>
    private void CheckProcessDuplicate()
    {
      this.isDuplicateProcess = false;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowcurentA = ultData.Rows[i].ChildBands[0].Rows[j];
          string code = rowcurentA.Cells["DivisionCode"].Value.ToString();
          for (int x = j + 1; x < ultData.Rows[i].ChildBands[0].Rows.Count; x++)
          {
            UltraGridRow rowcurentB = ultData.Rows[i].ChildBands[0].Rows[x];
            string codeB = rowcurentB.Cells["DivisionCode"].Value.ToString();
            if (string.Compare(code, codeB) == 0)
            {
              rowcurentA.Cells["DivisionCode"].Appearance.BackColor = Color.Yellow;
              rowcurentB.Cells["DivisionCode"].Appearance.BackColor = Color.Yellow;
              this.isDuplicateProcess = true;
            }
          }
        }
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
      if (dt != null && dt.Rows.Count > 0)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          string branch = row.Cells["BranchCode"].Value.ToString();
          string nameBranch = row.Cells["ENBranchName"].Value.ToString();

          if (branch.Length <= 0)
          {
            row.Appearance.BackColor = Color.Yellow;
            message = "BranchCode is invalid ";
            return false;
          }
          if (nameBranch.Length <= 0)
          {
            row.Appearance.BackColor = Color.Yellow;
            message = "Name Branch is invalid ";
            return false;
          }
          if (dt1 != null && dt1.Rows.Count > 0)
          {
            for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
            {
              UltraGridRow row1 = ultData.Rows[i].ChildBands[0].Rows[j];
              string division = row1.Cells["DivisionCode"].Value.ToString();
              string nameDivision = row1.Cells["ENDivisionName"].Value.ToString();
              if (division.Length <= 0)
              {
                row1.Appearance.BackColor = Color.Yellow;
                message = "DivisionCode is invalid ";
                return false;
              }
              if (nameDivision.Length <= 0)
              {
                row1.Appearance.BackColor = Color.Yellow;
                message = "Division Name is invalid ";
                return false;
              }
            }
          }
        }
      }
      return true;
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
      ucbBranch.Text = string.Empty;
      ucbDivision.Text = string.Empty;
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
        case "BranchCode":
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) == long.MinValue)
            {
              e.Cell.Row.Cells["Pid"].Value = pidtemp;
              pidtemp += -1;
            }
          }

          break;
        default:
          break;
      }
      this.CheckProcessDuplicate();
      if (e.Cell.Row.ParentRow != null)
      {
        if (e.Cell.Row.ParentRow.ChildBands[0].Rows.Count > 0)
        {
          for (int k = 0; k < e.Cell.Row.ParentRow.ChildBands[0].Rows.Count; k++)
          {
            e.Cell.Row.ParentRow.ChildBands[0].Rows[k].Cells["IsUpdate"].Value = 1;
          }
        }
      }
    }
    private void ucbBranch_ValueChanged(object sender, EventArgs e)
    {
      this.LoadDevision();
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion Event

  }
}
