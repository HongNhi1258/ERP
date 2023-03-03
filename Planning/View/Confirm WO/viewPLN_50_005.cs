/*
  Author      : Nguyen Huynh Quoc Tuan
  Date        : 27/5/2016
  Description : 
  Standard Form: viewPLN_50_006.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_50_005 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    public long pid = long.MinValue;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
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

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMailInfo_Load", inputParam);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        DataRow row = dtSource.Rows[0];
        txtMailCC.Text = row["MailCC"].ToString();
        txtMailTo.Text = row["MailTo"].ToString();
        txtBody.Text = row["Body"].ToString();
        txtSubject.Text = row["Subject"].ToString();
        txtGroup.Text = row["Group"].ToString();
      }
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      if (this.pid < 0)
      {
        string cmGroup = string.Format(@"SELECT [Group] FROM TblPLNMailInfo WHERE [Group] = '{0}'", txtGroup.Text.Trim());
        DataTable dtGroup = DataBaseAccess.SearchCommandTextDataTable(cmGroup);
        if (dtGroup.Rows.Count > 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Group");
          return false;
        }
      }

      if (txtGroup.Text.Trim().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Group");
        return false;
      }

      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid())
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };
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

        DBParameter[] inputParam = new DBParameter[7];
        if (this.pid > 0)
        {
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
        }

        if (txtGroup.Text.Trim().Length > 0)
        {
          inputParam[1] = new DBParameter("@Group", DbType.String, txtGroup.Text.Trim());
        }

        if (txtMailTo.Text.Trim().Length > 0)
        {
          inputParam[2] = new DBParameter("@MailTo", DbType.String, txtMailTo.Text.Trim());
        }

        if (txtMailCC.Text.Trim().Length > 0)
        {
          inputParam[3] = new DBParameter("@MailCC", DbType.String, txtMailCC.Text.Trim());
        }

        if (txtSubject.Text.Trim().Length > 0)
        {
          inputParam[4] = new DBParameter("@Subject", DbType.String, txtSubject.Text.Trim());
        }

        if (txtBody.Text.Trim().Length > 0)
        {
          inputParam[5] = new DBParameter("@Body", DbType.String, txtBody.Text.Trim());
        }

        inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        DataBaseAccess.ExecuteStoreProcedure("spPLNMailInfo_Edit", inputParam, outputParam);
        if ((outputParam == null) || (DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0))
        {
          success = false;
        }

        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.pid = DBConvert.ParseLong(outputParam[0].Value.ToString());
          this.LoadData();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region event
    public viewPLN_50_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_50_005_Load(object sender, EventArgs e)
    {
      this.LoadData();
      //Init Data
      this.InitData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    #endregion event
  }
}
