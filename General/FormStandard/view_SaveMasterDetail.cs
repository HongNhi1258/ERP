/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: view_SaveMasterDetail
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;

namespace DaiCo.General
{
  public partial class view_SaveMasterDetail : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init
    public view_SaveMasterDetail()
    {
      InitializeComponent();
    }

    private void view_SaveMasterDetail_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(gpbMaster);      
      this.InitData();
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoAskSaveWhenCloseForm(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.TextChanged += new System.EventHandler(this.Object_Changed);
        }
        else
        {
          this.SetAutoAskSaveWhenCloseForm(ctr);
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

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //ControlUtility.LoadUltraCombo();
      //ControlUtility.LoadUltraDropDown();
    }

    private void SetStatusControl()
    {

    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
      }
    }
    
    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      //DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, long.MinValue) };
      //DataSet dsSource = DataBaseAccess.SearchStoreProcedure("", inputParam);
      //if (dsSource != null && dsSource.Tables.Count > 1)
      //{
      //  this.LoadMainData(dsSource.Tables[0]);
      //  ultData.DataSource = dsSource.Tables[1];
      //}

      //this.SetStatusControl();
      this.NeedToSave = false;
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

    private bool SaveMain()
    {
      //string storeName = "";
      //int paramNumber = 1;
      //DBParameter[] inputParam = new DBParameter[paramNumber];      
      //inputParam[0] = new DBParameter("", DbType.String, 16, "");
      //inputParam[5] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      //DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      
      //DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      //if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      //{        
      //  this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
      //  return true;
      //}
      return false;
    }

    private bool SaveDetail()
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
      DataTable dtDetail = (DataTable)ugdData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[7];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          inputParam[6] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private void SaveData()
    {
      //string errorMessage;
      //if (this.CheckValid(out errorMessage))
      //{
      //  bool success = true;
      //  if (this.SaveMain())
      //  {
      //    success = this.SaveDetail();      
      //  }
      //  else
      //  {
      //    success = false;
      //  }
      //  if (success)
      //  {
      //    WindowUtinity.ShowMessageSuccess("MSG0004");
      //  }
      //  else
      //  {
      //    WindowUtinity.ShowMessageError("WRN0004");
      //  }
      //  this.LoadData();
      //}
      //else
      //{
      //  WindowUtinity.ShowMessageError("ERR0001", errorMessage);
      //  this.SaveSuccess = false;        
      //}
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        }
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }

      /*
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      
      // Hide column
      e.Layout.Bands[0].Columns[""].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns[""].Header.Caption = "\n";
      
      // Set dropdownlist for column
      e.Layout.Bands[0].Columns[""].ValueList = ultraDropdownList;
      
      // Set Align
      e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
      
      // Set Width
      e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      e.Layout.Bands[0].Columns[""].MinWidth = 100;
      
      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      
      // Set color
      ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;
      
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      
      // Read only
      e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      
      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      */
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();      
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
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

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();      
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
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();      
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ugdData, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        ControlUtility.GetDataForClipboard(ugdData);
      }
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdData.Selected.Rows.Count > 0 || ugdData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdData, new Point(e.X, e.Y));
        }
      }
    }
    #endregion Event
  }
}
