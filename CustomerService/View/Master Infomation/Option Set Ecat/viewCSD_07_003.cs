using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.IO;
using System.Collections;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_07_003 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private IList listDeletedPid = new ArrayList();
    private long pidtemp = -1;
    private bool isDuplicateProcess = false;
    private int parentIndex = int.MinValue;
    #endregion Field

    #region Init
    public viewCSD_07_003()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_07_003_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// List Option
    /// </summary>
    private DataSet ListOptionSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblMaster");
      taParent.Columns.Add("OptionSetPid", typeof(System.Int64));
      taParent.Columns.Add("Code", typeof(System.String));
      taParent.Columns.Add("Name", typeof(System.String));
      taParent.Columns.Add("Option", typeof(System.String));
      taParent.Columns.Add("OptionPid", typeof(System.String));
      taParent.Columns.Add("KindPid", typeof(System.Int64));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.DateTime));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblDetail");
      taChild.Columns.Add("OptionSetPid", typeof(System.Int64));
      taChild.Columns.Add("Code", typeof(System.String));
      taChild.Columns.Add("Name", typeof(System.String));
      taChild.Columns.Add("CreateBy", typeof(System.String));
      taChild.Columns.Add("CreateDate", typeof(System.DateTime));
      ds.Tables.Add(taChild);
      ds.Relations.Add(new DataRelation("TblMaster_TblDetail", taParent.Columns["OptionSetPid"], taChild.Columns["OptionSetPid"], false));
      return ds;
    }
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[4];
      if (txtOptionSetCode.Text.Length > 0)
      {
        input[0] = new DBParameter("@Code", DbType.AnsiString, 16, "%" + txtOptionSetCode.Text.Trim() + "%");
      }

      if (txtOptionSetName.Text.Length > 0)
      {
        input[1] = new DBParameter("@Name", DbType.String, 128, "%" + txtOptionSetName.Text.Trim() + "%");
      }
      if (ultCBOptionSetGroup.Value != null)
      {
        input[3] = new DBParameter("@Group", DbType.Int64, DBConvert.ParseLong(ultCBOptionSetGroup.Value.ToString()));
      }
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spCSDOptionSetForEcat_Search", input);
      ds.Relations.Add(new DataRelation("TblMaster_TblDetail", ds.Tables[0].Columns["OptionSetPid"], ds.Tables[1].Columns["OptionSetPid"], false));
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
      this.LoadOptionSetGroup();
    }

    private void LoadOptionSetGroup()
    {
      string command = "SELECT Pid, Code, Name  FROM	TblCSDOptionCodeForEcat";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(command);

      string commandText = "SELECT	Pid, Name FROM	TblCSDOptionSetKindEcat";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBOptionSetGroup.DataSource = dtSource;
      ultCBOptionSetGroup.DisplayMember = "Name";
      ultCBOptionSetGroup.ValueMember = "Pid";
      ultCBOptionSetGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBOptionSetGroup.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBOptionSetGroup.DisplayLayout.AutoFitColumns = true;

      ultDDOptionSetGroup.DataSource = dtSource;
      ultDDOptionSetGroup.DisplayMember = "Name";
      ultDDOptionSetGroup.ValueMember = "Pid";
      ultDDOptionSetGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDOptionSetGroup.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultDDOptionSetGroup.DisplayLayout.AutoFitColumns = true;

      ultraDDOptionSetEcat.DataSource = dt;
      ultraDDOptionSetEcat.DisplayMember = "Code";
      ultraDDOptionSetEcat.ValueMember = "Code";
      ultraDDOptionSetEcat.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDOptionSetEcat.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      //ultraDDOptionSetEcat.DisplayLayout.AutoFitColumns = true;
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
      this.ConfirmToCloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["OptionSetPid"].Hidden = true;
      e.Layout.Bands[0].Columns["GroupPid"].Hidden = true;
      e.Layout.Bands[0].Columns["OptionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsUpdate"].Hidden = true;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["OptionSetPid"].Hidden = true;

      e.Layout.Bands[0].Columns["Code"].Header.Caption = "Option Group Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Option Group Name";
      e.Layout.Bands[0].Columns["Option"].Header.Caption = "Option Set";

      e.Layout.Bands[1].Columns["CodeOption"].Header.Caption = "Option Code";
      e.Layout.Bands[1].Columns["Name"].Header.Caption = "Option Name";

      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Option"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Group"].ValueList = ultDDOptionSetGroup;
      e.Layout.Bands[1].Columns["CodeOption"].ValueList = ultraDDOptionSetEcat;
      //e.Layout.Bands[0].Columns["Option"].ValueList = ultraDDOptionSetEcat;

      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

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
        if (ultData.Rows[i].Cells["Code"].Value.ToString().Trim() == string.Empty || ultData.Rows[i].Cells["Group"].Value.ToString() == string.Empty)
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
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        foreach (long pidParent in this.listDeletedPid)
        {

          DBParameter[] inputParams = new DBParameter[1];
          inputParams[0] = new DBParameter("@Pid", DbType.Int64, pidParent);
          DataBaseAccess.ExecuteStoreProcedure("spCSDOptiongGroupForEcat_Delete", inputParams, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update                 
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          long pid = DBConvert.ParseLong(ultData.Rows[i].Cells["OptionSetPid"].Value);
          int isUpdate = DBConvert.ParseInt(ultData.Rows[i].Cells["IsUpdate"].Value);
          DBParameter[] inputParam = new DBParameter[7];
          if (isUpdate == 1)
          {
            string code = ultData.Rows[i].Cells["Code"].Value.ToString();
            string name = ultData.Rows[i].Cells["Name"].Value.ToString();
            string optionPid = ultData.Rows[i].Cells["OptionPid"].Value.ToString();
            long groupPid = DBConvert.ParseLong(ultData.Rows[i].Cells["GroupPid"].Value);

            if (pid > 0) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@Code", DbType.AnsiString, 16, code);
            inputParam[2] = new DBParameter("@Name", DbType.String, 256, name);
            inputParam[3] = new DBParameter("@Option", DbType.String, 128, optionPid);
            inputParam[4] = new DBParameter("@Group", DbType.Int64, groupPid);
            inputParam[5] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);

            DataBaseAccess.ExecuteStoreProcedure("spCSDOptionSetForEcat_Edit", inputParam, outputParam);
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
      // Check Invalid before deleted
//      foreach (UltraGridRow row in e.Rows)
//      {
//        if (row.ParentRow != null)
//        {
//          long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
//          if (pid != long.MinValue)
//          {
//            string cmtext = string.Format(@"SELECT OP.ItemCode
//                                           FROM  TblCSDECatItemOptionCode OP
//      	                                          INNER JOIN TblCSDOptionCodeForEcat OPCE ON '|' + OP.OptionCode + '|' LIKE '%|' + CAST(OPCE.Pid AS VARCHAR) + '|%'
//                                            WHERE OPCE.Pid = {0}", pid);
//            DataTable dtWOInfo = DataBaseAccess.SearchCommandTextDataTable(cmtext);
//            if (dtWOInfo != null && dtWOInfo.Rows.Count > 0)
//            {
//              string message = string.Format(FunctionUtility.GetMessage("ERR0093"), "Option Group");
//              WindowUtinity.ShowMessageErrorFromText(message);
//              e.Cancel = true;
//              return;
//            }
//          }
//        }
//      }
      //Delete parent
      foreach (UltraGridRow row in e.Rows)
      {
        if (row.ParentRow == null)
        {
          long pidParent = DBConvert.ParseLong(row.Cells["OptionSetPid"].Value.ToString());
          if (pidParent != long.MinValue)
          {
            this.listDeletedPid.Add(pidParent);
          }
        }
      }
      //foreach (UltraGridRow row in e.Rows)
      //{
      //  if (row.ParentRow != null)
      //  {
      //    long pid = DBConvert.ParseLong(row.Cells["OptionSetPid"].Value.ToString());
      //    if (pid > 0)
      //    {
      //      string message = string.Format(FunctionUtility.GetMessage("ERR0093"), "Option Group");
      //      WindowUtinity.ShowMessageErrorFromText(message);
      //      e.Cancel = true;
      //      return;
      //    }
      //  }
      //}

      // Set Options for Option Group
      if (e.Rows[0].ParentRow != null)
      {
        this.parentIndex = e.Rows[0].ParentRow.Index;
      }
      else
      {
        this.parentIndex = int.MinValue;
      }

      this.CheckProcessDuplicate();
      this.SetNeedToSave();
    }
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      int count = 0;
      switch (colName)
      {
        case "code":
          // Check duplicate option group
          for (int i = 0; i < ultData.Rows.Count; i++)
          {
            if (e.Cell.Row.Cells["Code"].Text == ultData.Rows[i].Cells["Code"].Text)
            {
              count++;
              if (count == 2)
              {
                string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Code");
                WindowUtinity.ShowMessageErrorFromText(message);
                e.Cancel = true;
                return;
              }
            }
          }
          string commandText = string.Format(@"SELECT *	FROM	TblCSDOptionSetForEcat WHERE Code = '{0}'", e.Cell.Row.Cells["Code"].Text);
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dt != null && dt.Rows.Count > 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Code");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
            return;
          }
          break;
        case "group":
          // Check Invalid Option Set
          UltraDropDown group = (UltraDropDown)e.Cell.Row.Cells["Group"].ValueList;
          bool check = this.CheckMemberDownDrop(e.Cell.Row.Cells["Group"].Text.Trim(), "Name", (DataTable)ultDDOptionSetGroup.DataSource);
          if (check == false)
          {
            string message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Group");
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.Cancel = true;
            return;
          }
          break;
        default:
          break;
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
    /// <summary>
    /// HÀM CHECK PROCESS DUPLICATE
    /// </summary>
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
          string code = rowcurentA.Cells["CodeOption"].Value.ToString();
          for (int x = j + 1; x < ultData.Rows[i].ChildBands[0].Rows.Count; x++)
          {
            UltraGridRow rowcurentB = ultData.Rows[i].ChildBands[0].Rows[x];
            string codeB = rowcurentB.Cells["CodeOption"].Value.ToString();
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

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      //e.Cell.Row.ParentRow.Cells["Option"].Value = "";
      switch (colName)
      {
        case "group":
          {
            e.Cell.Row.Cells["GroupPid"].Value = DBConvert.ParseLong(e.Cell.Row.Cells["Group"].Value.ToString());
          }
          break;
        case "codeoption":
          {
            long pidParent = DBConvert.ParseLong(e.Cell.Row.ParentRow.Cells["OptionSetPid"].Value);    
            // Set name for OptionSet Code & Set options for OptionSet Group
            e.Cell.Row.Cells["Name"].Value = ultraDDOptionSetEcat.SelectedRow.Cells["Name"].Value.ToString();
            e.Cell.Row.Cells["Pid"].Value = ultraDDOptionSetEcat.SelectedRow.Cells["Pid"].Value.ToString();
            e.Cell.Row.Cells["OptionSetPid"].Value = pidParent;
            string option = e.Cell.Row.ParentRow.Cells["Option"].Value.ToString();
            string optionPid = e.Cell.Row.ParentRow.Cells["OptionPid"].Value.ToString();
            option = string.Empty;
            optionPid = string.Empty;
            RowsCollection rows = e.Cell.Row.ParentRow.ChildBands[0].Rows;
            for (int i = 0; i < rows.Count; i++)
            {
              if (option.Length == 0)
              {
                option = rows[i].Cells["CodeOption"].Value.ToString();
                optionPid = rows[i].Cells["Pid"].Value.ToString();
              }
              else
              {
                option += "," + rows[i].Cells["CodeOption"].Value.ToString();
                optionPid += "|" + rows[i].Cells["Pid"].Value.ToString();
              }
            }
            e.Cell.Row.ParentRow.Cells["Option"].Value = option;
            e.Cell.Row.ParentRow.Cells["OptionPid"].Value = optionPid;            
            
            this.CheckProcessDuplicate();
          }
          break;
        case "code":
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["OptionSetPid"].Value.ToString()) == long.MinValue)
            {
              e.Cell.Row.Cells["OptionSetPid"].Value = pidtemp;
              pidtemp += -1;
            }
          }
          break;
        default:
          break;
      }
      if (e.Cell.Row.ParentRow == null)
      {
        e.Cell.Row.Cells["IsUpdate"].Value = 1;
      }
      else
      {
        e.Cell.Row.ParentRow.Cells["IsUpdate"].Value = 1;
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
      txtOptionSetCode.Text = string.Empty;
      txtOptionSetName.Text = string.Empty;
      ultCBOptionSetGroup.Text = string.Empty;
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      if (this.parentIndex >= 0)
      {
        string option = string.Empty;
        RowsCollection rows = ultData.Rows[this.parentIndex].ChildBands[0].Rows;
        for (int i = 0; i < rows.Count; i++)
        {
          if (option.Length == 0)
          {
            option = rows[i].Cells["CodeOption"].Value.ToString();
          }
          else
          {
            option += "," + rows[i].Cells["CodeOption"].Value.ToString();
          }
        }
        ultData.Rows[this.parentIndex].Cells["Option"].Value = option;

        string optionPid = string.Empty;
        RowsCollection rows1 = ultData.Rows[this.parentIndex].ChildBands[0].Rows;
        for (int i = 0; i < rows1.Count; i++)
        {
          if (optionPid.Length == 0)
          {
            optionPid = rows[i].Cells["Pid"].Value.ToString();
          }
          else
          {
            optionPid += "|" + rows[i].Cells["Pid"].Value.ToString();
          }
        }
        ultData.Rows[this.parentIndex].Cells["OptionPid"].Value = optionPid;
        ultData.Rows[this.parentIndex].Cells["IsUpdate"].Value = 1;        
      }
      this.CheckProcessDuplicate();
    }
    #endregion Event

    private void btnExport_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "Data");
    }
  }
}
