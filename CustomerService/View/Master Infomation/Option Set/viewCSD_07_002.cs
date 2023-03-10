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
  public partial class viewCSD_07_002 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private IList listDeletedPid = new ArrayList();

    #endregion Field

    #region Init
    public viewCSD_07_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_07_002_Load(object sender, EventArgs e)
    {
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

      DBParameter[] input = new DBParameter[4];
      if (txtOptionSetCode.Text.Length > 0)
      {
        input[0] = new DBParameter("@Code", DbType.AnsiString, 16, "%" + txtOptionSetCode.Text.Trim() + "%" );
      }

      if (txtOptionSetName.Text.Length > 0)
      {
        input[1] = new DBParameter("@Name", DbType.String, 128, "%" + txtOptionSetName.Text.Trim() + "%");
      }
      if (ultCBOptionSetGroup.Value != null)
      {
        input[3] = new DBParameter("@Group", DbType.Int64, DBConvert.ParseLong(ultCBOptionSetGroup.Value.ToString()));
      }

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spCSDOptionSet_Search", input);
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
      this.LoadOptionSetGroup();
    }

    private void LoadOptionSetGroup()
    {
      string commandText = "SELECT	Pid, Name FROM	TblCSDOptionSetKind";
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
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["OptionSetPid"].Hidden = true;
      e.Layout.Bands[0].Columns["GroupPid"].Hidden = true;

      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Group"].ValueList = ultDDOptionSetGroup;

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
      for (int i = 0; i < ultData.Rows.Count; i++ )
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
        for (int i = 0; i < listDeletedPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ultData.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[7];
            long pid = DBConvert.ParseLong(row["OptionSetPid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@Code", DbType.AnsiString, 16, row["Code"].ToString());
            inputParam[2] = new DBParameter("@Name", DbType.String, 256, row["Name"].ToString());
            inputParam[3] = new DBParameter("@Group", DbType.Int64, DBConvert.ParseLong(row["GroupPid"].ToString()));
            inputParam[4] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);

            DataBaseAccess.ExecuteStoreProcedure("spCSDOptionSet_Edit", inputParam, outputParam);
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
        long pid = DBConvert.ParseLong(row.Cells["OptionSetPid"].Value.ToString());
        if (pid != long.MinValue)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0093"), "Code");
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
      if (colName == "code")
      {
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

        string commandText = string.Format(@"SELECT *	FROM	TblCSDOptionSet WHERE Code = '{0}'", e.Cell.Row.Cells["Code"].Text);
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if(dt != null && dt.Rows.Count > 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Code");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }
      }

      if(colName == "group")
      {
        UltraDropDown group = (UltraDropDown)e.Cell.Row.Cells["Group"].ValueList;
        bool check = this.CheckMemberDownDrop(e.Cell.Row.Cells["Group"].Text.Trim(), "Name", (DataTable)ultDDOptionSetGroup.DataSource);
        if (check == false)
        {
          string message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Group");
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
        case "group":
          {
            e.Cell.Row.Cells["GroupPid"].Value = DBConvert.ParseLong(e.Cell.Row.Cells["Group"].Value.ToString());
          }
          break;
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

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "Data");
    }

    #endregion Event

    
  }
}
