/*
  Author      : Nguyen Thanh Binh
  Date        : 27/03/2021
  Description : List of type account
  Standard Form: view_ExtraControl.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_09_003 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(view_ExtraControl).Assembly);
    private IList listDeletedDocumentPid = new ArrayList();
    private IList listDeletedEntryPid = new ArrayList();
    #endregion field

    #region function

    /// <summary>
    /// Load tab data
    /// </summary>
    private void LoadTabData()
    {
      // Load Tab Data Component
      string tabPageName = utabDocument.SelectedTab.TabPage.Name;
      switch (tabPageName)
      {
        case "utpcDocumentEntry":
          this.LoadDatadocumentEntry();
          break;
        case "utpcAccountEntry":
          this.LoadDataEntry();
          break;
        default:
          break;
      }
    }


    private void LoadDatadocumentEntry()
    {
      string cmd = string.Format(@"SELECT Pid, DocName
                FROM TblACCDocumentType
                ORDER BY DocName");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
      ugrdDocumentEntry.DataSource = dt;
      lbCountDocument.Text = string.Format(@"Đếm: {0}", ugrdDocumentEntry.Rows.FilteredInRowCount > 0 ? ugrdDocumentEntry.Rows.FilteredInRowCount : 0);
    }

    private void LoadDataEntry()
    {
      string cmd = string.Format(@"SELECT Pid, EntryName
                                  FROM TblACCEntryType
                                  ORDER BY EntryName");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
      ugrdEntry.DataSource = dt;
      lbCountEntry.Text = string.Format(@"Đếm: {0}", ugrdEntry.Rows.FilteredInRowCount > 0 ? ugrdEntry.Rows.FilteredInRowCount : 0);
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {

      // Set Language
      this.SetLanguage();
    }
    private void SetNeedToSave()
    {
      if (btnSaveDocument.Enabled && btnSaveDocument.Visible)
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
      this.LoadTabData();
    }

    /// <summary>
    /// Set Auto Add 4 blank before text of button
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetBlankForTextOfButton(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count > 0)
        {
          this.SetBlankForTextOfButton(ctr);
        }
        else if (ctr.GetType().Name == "Button")
        {
          ctr.Text = string.Format("{0}{1}", "    ", ctr.Text);
        }
      }
    }
    private void SetLanguage()
    {
      //btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);      

      this.SetBlankForTextOfButton(this);
    }

    private void SaveDataDocument()
    {
      if (this.CheckValidDocument())
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedDocumentPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedDocumentPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ugrdDocumentEntry.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[3];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@DocName", DbType.String, row["DocName"].ToString());
            inputParam[2] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spACCAccountDocument_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadTabData();
      }
      else
      {
        this.SaveSuccess = false;
      }
    }

    private bool CheckValidDocument()
    {
      DataTable dtDetail = (DataTable)ugrdDocumentEntry.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (row["DocName"].ToString().Trim().Length <= 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Diễn giải không được để trống!!!");
            return false;
          }
        }
      }
      return true;
    }


    /// <summary>
    /// save entry
    /// </summary>
    private void SaveDataEntry()
    {
      if (this.CheckValidEntry())
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedEntryPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedEntryPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ugrdEntry.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[3];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@EntryName", DbType.String, row["EntryName"].ToString());
            inputParam[2] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spACCAccountEntryType_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadTabData();
      }
      else
      {
        this.SaveSuccess = false;
      }
    }

    private bool CheckValidEntry()
    {
      DataTable dtDetail = (DataTable)ugrdEntry.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (row["EntryName"].ToString().Trim().Length <= 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Diễn giải không được để trống!!!");
            return false;
          }
        }
      }
      return true;
    }

    #endregion function

    #region event
    public viewACC_09_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_09_003_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ugrdDocumentEntry_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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
          this.listDeletedDocumentPid.Add(pid);
        }
      }
    }

    private void ugrdEntry_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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
          this.listDeletedEntryPid.Add(pid);
        }
      }
    }



    private void ugrdDocumentEntry_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.InitLayout_UltraGrid(ugrdDocumentEntry);
      ugrdDocumentEntry.DisplayLayout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.FixedAddRowOnTop;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["DocName"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["DocName"].Width = 250;
    }


    private void utabAccount_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
    {
      this.LoadTabData();
    }


    private void btnSaveDocument_Click(object sender, EventArgs e)
    {
      this.SaveDataDocument();
      this.LoadTabData();
    }


    private void btnCloseEntry_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSaveEntry_Click(object sender, EventArgs e)
    {
      this.SaveDataEntry();
      this.LoadTabData();
    }


    private void ugrdEntry_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.InitLayout_UltraGrid(ugrdEntry);
      ugrdEntry.DisplayLayout.AutoFitStyle = AutoFitStyle.None;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.FixedAddRowOnTop;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["EntryName"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["EntryName"].Width = 250;
    }

    private void ugrdDocumentEntry_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      long accountDocumentPid = DBConvert.ParseLong(ugrdDocumentEntry.Selected.Rows[0].Cells["Pid"].Value.ToString());
      viewACC_09_004 view = new viewACC_09_004();
      view.accountDocumentPid = accountDocumentPid;
      Shared.Utility.WindowUtinity.ShowView(view, "Cấu hình chứng từ", false, Shared.Utility.ViewState.ModalWindow);
    }
    #endregion event
  }
}
