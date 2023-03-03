/*
  Author      : Nguyen Van Tron
  Date        : 25/12/2013
  Description : Set Need Days of Material for Item
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_09_001 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      // Load data for WO
      string commandText = "SELECT DISTINCT WoInfoPID FROM TblPLNWOInfoDetailGeneral ORDER BY WoInfoPID DESC";
      DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadUltraCombo(ultraCBWo, dtWO, "WoInfoPID", "WoInfoPID", false);
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 4;
      string storeName = "spPLNItemMaterialCategory_Select";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (ultraCBWo.Value != null)
      {
        inputParam[0] = new DBParameter("@Wo", DbType.Int64, ultraCBWo.Value);
      }
      if (txtItemCode.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, txtItemCode.Text.Trim());
      }
      if (txtRevision.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(txtRevision.Text));
      }
      if (chkActiveRevision.Checked)
      {
        inputParam[3] = new DBParameter("@IsActiveRevision", DbType.Int32, 1);
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, 300, inputParam);
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        dsSource.Relations.Add("ItemRelation", dsSource.Tables[0].Columns["KeyRelation"], dsSource.Tables[1].Columns["KeyRelation"], false);
        ultraGridInformation.DataSource = dsSource;
        lbCount.Text = string.Format("Count: {0}", dsSource.Tables[0].Rows.Count);
      }
      else
      {
        ultraGridInformation.DataSource = null;
        lbCount.Text = "Count: 0";
      }
      this.NeedToSave = false;
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      ultraCBWo.Value = null;
      txtItemCode.Text = string.Empty;
      txtRevision.Text = string.Empty;
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

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //if (ultCBWo.Text.Length == 0)
      //{
      //  errorMessage = "Work Order";      
      //  return false;
      //}
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
        DataSet dsSource = (DataSet)ultraGridInformation.DataSource;
        if (dsSource != null && dsSource.Tables.Count > 1)
        {
          DataTable dtDetail = dsSource.Tables[1];
          foreach (DataRow row in dtDetail.Rows)
          {
            if (row.RowState == DataRowState.Modified)
            {
              DBParameter[] inputParam = new DBParameter[6];
              long pid = DBConvert.ParseLong(row["Pid"].ToString());
              if (pid > 0) // Update
              {
                inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
              }
              else
              {
                string itemCode = row["ItemCode"].ToString();
                int revision = DBConvert.ParseInt(row["Revision"]);
                string category = row["MaterialCode"].ToString();
                inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
                inputParam[2] = new DBParameter("@Revision", DbType.Int32, revision);
                inputParam[3] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, category);
              }
              double needDays = DBConvert.ParseDouble(row["NeedDay"]);
              if (needDays > 0)
              {
                inputParam[4] = new DBParameter("@NeedDay", DbType.Double, needDays);
              }
              inputParam[5] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              DataBaseAccess.ExecuteStoreProcedure("spPLNItemMaterialCategory_Edit", inputParam, outputParam);
              if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
              {
                success = false;
              }
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
    #endregion function

    #region event
    public viewPLN_09_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_09_001_Load(object sender, EventArgs e)
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

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
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

      //Sort
      e.Layout.Bands[0].Columns["ItemCode"].SortIndicator = SortIndicator.Ascending;
      e.Layout.Bands[1].Columns["MaterialCode"].SortIndicator = SortIndicator.Ascending;

      // Set Align
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["NeedDay"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      // Set color
      e.Layout.Bands[1].Columns["NeedDay"].CellAppearance.BackColor = Color.LightGray;

      // Hide column
      e.Layout.Bands[0].Columns["KeyRelation"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["KeyRelation"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;

      // Set caption column
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Item Name";
      e.Layout.Bands[1].Columns["NeedDay"].Header.Caption = "Need Days";
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";

      // Set Width
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 100;
      e.Layout.Bands[1].Columns["MaterialCode"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["MaterialCode"].MinWidth = 100;
      e.Layout.Bands[1].Columns["NeedDay"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["NeedDay"].MinWidth = 100;

      // Read only
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Name"].CellActivation = Activation.ActivateOnly;
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
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void chkActiveRevision_CheckedChanged(object sender, EventArgs e)
    {
      if (chkActiveRevision.Checked)
      {
        txtRevision.Text = string.Empty;
        txtRevision.ReadOnly = true;
      }
      else
      {
        txtRevision.ReadOnly = false;
      }
    }
    #endregion event    
  }
}
