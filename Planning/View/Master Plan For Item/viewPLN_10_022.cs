/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewPLN_10_022.cs
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
  public partial class viewPLN_10_022 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    public DataTable dtSource;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
      this.SearchData();
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      this.NeedToSave = false;
      //btnSearch.Enabled = false;        
      ultraGridInformation.DataSource = this.dtSource;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow dr = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(dr.Cells["Error"].Value) == 0)
        {
          dr.CellAppearance.BackColor = Color.White;
        }
        if (DBConvert.ParseInt(dr.Cells["Error"].Value) == 1)
        {
          dr.CellAppearance.BackColor = Color.Lime;

        }
        if (DBConvert.ParseInt(dr.Cells["Error"].Value) == 2)
        {
          dr.CellAppearance.BackColor = Color.SkyBlue;
          dr.Cells["OldWO"].Appearance.ForeColor = Color.Red;
          dr.Cells["OldWO"].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
          dr.Cells["NewWO"].Appearance.ForeColor = Color.Red;
          dr.Cells["NewWO"].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
        }
        if (DBConvert.ParseInt(dr.Cells["Error"].Value) == 3)
        {
          dr.CellAppearance.BackColor = Color.Pink;
          dr.Cells["OldSO"].Appearance.ForeColor = Color.Red;
          dr.Cells["OldSO"].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
          dr.Cells["NewSO"].Appearance.ForeColor = Color.Red;
          dr.Cells["NewSO"].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
        }
        if (DBConvert.ParseInt(dr.Cells["Error"].Value) == 4)
        {
          dr.CellAppearance.BackColor = Color.Orange;

        }
      }
      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      //btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {

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
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow dr = ultraGridInformation.Rows[i];
        if (DBConvert.ParseInt(dr.Cells["Error"].Value) > 0)
        {
          errorMessage = "Error";
          return false;
        }
      }
      return true;
    }
    private DataTable CreateDataTableBeforeSave()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("SwapQty", typeof(System.Int32));
      dt.Columns.Add("OldWOPid", typeof(System.Int64));
      dt.Columns.Add("OldSOPid", typeof(System.Int64));
      dt.Columns.Add("NewWOPid", typeof(System.Int64));
      dt.Columns.Add("NewSOPid", typeof(System.Int64));
      return dt;
    }
    private bool Savedetail()
    {

      // 1. Insert
      DataTable workTable = this.CreateDataTableBeforeSave();
      string storeName = "spPLNListOfSwapWOSO_Insert";
      DataRow workRow;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        workRow = workTable.NewRow();
        if (DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["NewWO"].Value.ToString()) >= 0)
        {
          workRow["NewWOPid"] = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["NewWO"].Value.ToString());
        }
        else
        {
          workRow["NewWOPid"] = DBNull.Value;
        }
        if (DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["NewSOPid"].Value.ToString()) >= 0)
        {
          workRow["NewSOPid"] = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["NewSOPid"].Value.ToString());
        }
        else
        {
          workRow["NewSOPid"] = DBNull.Value;
        }
        workRow["OldSOPid"] = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["OldSOPid"].Value.ToString());
        workRow["OldWOPid"] = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["OldWO"].Value.ToString());
        workRow["SwapQty"] = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["SwapQty"].Value.ToString());
        workRow["ItemCode"] = ultraGridInformation.Rows[i].Cells["ItemCode"].Value.ToString();
        workRow["Revision"] = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["Revision"].Value.ToString());
        workTable.Rows.Add(workRow);
      }
      SqlDBParameter[] inputParam = new SqlDBParameter[2];
      inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, workTable);
      inputParam[1] = new SqlDBParameter("@CurrentUser", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      SqlDBParameter[] outputParam = new SqlDBParameter[1];
      outputParam[0] = new SqlDBParameter("@Result", SqlDbType.Int, int.MinValue);
      DataSet dt = SqlDataBaseAccess.SearchStoreProcedure(storeName, 900, inputParam, outputParam);
      if (DBConvert.ParseInt(outputParam[0].Value.ToString()) < 0)
      {
        return false;
      }
      return true;
    }
    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = this.Savedetail();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.CloseTab();
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

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region event
    public viewPLN_10_022()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_10_022_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      //this.SetAutoSearchWhenPressEnter(groupBoxSearch);

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

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["NewSOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["OldSOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Error"].Hidden = true;

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
    #endregion event
  }
}
