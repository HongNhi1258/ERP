/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: view_SearchSave.cs
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
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System.Collections;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_08_001 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    private bool isDuplicateBaseItem = false;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      btnSave.Enabled = false;
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      btnSearch.Enabled = false;
      int paramNumber = 1;
      string storeName = "spCSDECatBaseItemCode_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      inputParam[0] = new DBParameter("@BaseItemCode", DbType.String, txtBaseItemCode.Text.Trim());

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ultraGridInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
      btnSave.Enabled = true;
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
    private void CheckBaseItemDuplicate()
    {
      isDuplicateBaseItem = false;
      for (int k = 0; k < ultraGridInformation.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultraGridInformation.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow rowcurentA = ultraGridInformation.Rows[i];
        string baseitemA = rowcurentA.Cells["BaseItemCode"].Value.ToString().Trim().ToLower();
        for (int j = i + 1; j < ultraGridInformation.Rows.Count; j++)
        {
          UltraGridRow rowcurentB = ultraGridInformation.Rows[j];
          string baseitemB = rowcurentB.Cells["BaseItemCode"].Value.ToString().Trim().ToLower();
          if (string.Compare(baseitemA, baseitemB) == 0)
          {
            rowcurentA.CellAppearance.BackColor = Color.Yellow;
            rowcurentB.CellAppearance.BackColor = Color.Yellow;
            isDuplicateBaseItem = true;
          }
        }
      }
    }
    private UltraDropDown loadItemFollowBase(long pidBase, UltraDropDown ultDDItem)
    {
      if (ultDDItem == null)
      {
        ultDDItem = new UltraDropDown();
        this.Controls.Add(ultDDItem);
      }
      string cmText = string.Format(@"SELECT ItemCode
                                      FROM TblCSDItemInfo
                                      WHERE BaseItemPid = {0}", pidBase);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      if (dt != null)
      {
        ControlUtility.LoadUltraDropDown(ultDDItem, dt, "ItemCode", "ItemCode");
      }
      return ultDDItem;
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      if (this.isDuplicateBaseItem == true)
      {
        errorMessage = "Duplicate Base Item Code";
        return false;
      }
      DataTable dtDetail = (DataTable)ultraGridInformation.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          string baseItem = row["BaseItemCode"].ToString().Trim();
          if (baseItem.Length == 0)
          {
            errorMessage = "Base Item Code";
            return false;
          }
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
          DataBaseAccess.ExecuteStoreProcedure("spCSDECatBaseItemCode_Delete", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ultraGridInformation.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[6];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@BaseItemCode", DbType.String, row["BaseItemCode"].ToString().Trim());
            if (row["ProductStory"].ToString().Trim().Length > 0)
            {
              inputParam[2] = new DBParameter("@ProductStory", DbType.String, row["ProductStory"].ToString().Trim());
            }
            if (row["LongDescription"].ToString().Trim().Length > 0)
            {
              inputParam[3] = new DBParameter("@LondDescription", DbType.String, row["LongDescription"].ToString().Trim());
            }
            if (row["ItemCodeDefault"].ToString().Trim().Length > 0)
            {
              inputParam[4] = new DBParameter("@ItemDefault", DbType.String, row["ItemCodeDefault"].ToString().Trim());
            }
            inputParam[5] = new DBParameter("@InputBy", DbType.Int32, SharedObject.UserInfo.UserPid);
            DataBaseAccess.ExecuteStoreProcedure("spCSDECatBaseItemCode_Edit", inputParam, outputParam);
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
        this.SearchData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region event
    public viewCSD_08_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_08_001_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);
      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }


    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
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
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnTop;
      
      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns["BaseItemCode"].Header.Caption = "Base Item Code";
      e.Layout.Bands[0].Columns["ProductStory"].Header.Caption = "Product Story";
      e.Layout.Bands[0].Columns["LongDescription"].Header.Caption = "Long Description";
      
      // Set Width
      e.Layout.Bands[0].Columns["BaseItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["BaseItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCodeDefault"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCodeDefault"].MinWidth = 100;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
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

    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "ItemCodeDefault":
          {
            if (value.Trim().Length > 0)
            {
              string cm = string.Format(@"SELECT COUNT(*)
                                      FROM TblBOMItemBasic
                                      WHERE ItemCode = '{0}'", value);
              int rs = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(cm).ToString());
              if (rs == 0)
              {
                WindowUtinity.ShowMessageError("ERR0029", "Item Code");
                e.Cancel = true;
              }
            }
          }
          break;
        default:
          break;
      }
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      switch (colName)
      {
        case "BaseItemCode":
          {
            this.CheckBaseItemDuplicate();
          }
          break;
        default:
          break;
      }
      this.SetNeedToSave();
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        ControlUtility.GetDataForClipboard(ultraGridInformation);
      }
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultraGridInformation.Selected.Rows.Count > 0 || ultraGridInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultraGridInformation, new Point(e.X, e.Y));
        }
      }
    }
    private void ultraGridInformation_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.CheckBaseItemDuplicate();
    }
    private void ultraGridInformation_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      switch (columnName)
      {
        case "ItemCodeDefault":
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()) != long.MinValue)
            {
              UltraDropDown ultItemDefault = (UltraDropDown)e.Cell.Row.Cells["ItemCodeDefault"].ValueList;
              e.Cell.Row.Cells["ItemCodeDefault"].ValueList = this.loadItemFollowBase(DBConvert.ParseLong(e.Cell.Row.Cells["Pid"].Value.ToString()), ultItemDefault);
            }
          }
          break;
        default:
          break;
      }
    }
    #endregion event


  }
}
