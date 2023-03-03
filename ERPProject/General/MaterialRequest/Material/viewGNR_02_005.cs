/*
  Author      : Huynh Thi Bang  
  Date        : 03/08/2015
  Description : Request Material 
  Standard Form: viewGNR_02_005
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

namespace DaiCo.ERPProject
{
  public partial class viewGNR_02_005 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    private bool checkGrid = false;
    #endregion Field

    #region Init
    public viewGNR_02_005()
    {
      InitializeComponent();
    }

    private void viewGNR_02_005_Load(object sender, EventArgs e)
    {
      //NeedToSave = 
      // Add ask before closing form even if user change data
      this.SetAutoSearchWhenPressEnter(groupBoxMaster);
      //this.InitData();
      this.LoadData();
    }


    #endregion Init

    #region Function
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    /// 
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
    /// <summary>
    /// Search when Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new KeyEventHandler(ctr_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }

    void ctr_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
        //this.Check();
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
      //Utility.LoadUltraCombo();
      //Utility.LoadUltraDropDown();
    }
    private void LoadData()
    {
      this.LoadCbWo();
      this.LoadCbFur();
      this.LoadCbItem();
      this.LoadCbDepartment();
      this.LoadCbMaterialCode();
      this.listDeletedPid = new ArrayList();
      this.NeedToSave = false;
    }
    /// <summary>
    /// Load Wo
    /// </summary>
    private void LoadCbWo()
    {
      string cm = "SELECT distinct WorkOrderPid FROM TblPLNWorkOrderConfirmedDetails ORDER BY WorkOrderPid";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      Utility.LoadUltraCombo(ultCBWo, dt, "WorkOrderPid", "WorkOrderPid", false);

    }
    /// <summary>
    /// Load FurnitureCode
    /// </summary>
    private void LoadCbFur()
    {
      string cm = string.Empty;
      DataTable dt = new DataTable();
      cm = "SELECT Pid,FurnitureCode FROM TblWIPFurnitureCode";
      dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      Utility.LoadUltraCombo(ultcbFurCode, dt, "Pid", "FurnitureCode", false, "Pid");
    }
    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadCbItem()
    {

      string commandText = string.Empty;
      commandText = string.Format(@"SELECT ItemCode, Revision, ItemCode + ' - Revision : ' + CONVERT(varchar, Revision) Descption
                                           FROM dbo.TblPLNWorkOrderConfirmedDetails ORDER BY ItemCode DESC");

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultcbItCode.DataSource = dtSource;
      ultcbItCode.ValueMember = "ItemCode";
      ultcbItCode.DisplayMember = "Descption";
      ultcbItCode.DisplayLayout.Bands[0].Columns["Descption"].Width = 200;
      ultcbItCode.DisplayLayout.Bands[0].Columns["ItemCode"].Hidden = true;
      ultcbItCode.DisplayLayout.Bands[0].Columns["Revision"].Hidden = true;
      ultcbItCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    /// <summary>
    /// Load MaterialCode
    /// </summary>
    private void LoadCbMaterialCode()
    {
      string commandText = "SELECT MaterialCode, MaterialNameEn, MaterialCode +' - '+ MaterialNameEn AS Display FROM VBOMMaterials WHERE DepartmentCode IS NOT NULL";
      DataTable dtMaterial = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtMaterial != null && dtMaterial.Rows.Count > 0)
      {

        ultcbMaCode.DataSource = dtMaterial;
        ultcbMaCode.ValueMember = "MaterialCode";
        ultcbMaCode.DisplayMember = "Display";
        ultcbMaCode.DisplayLayout.Bands[0].Columns["Display"].Hidden = true;
        ultcbMaCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      }
    }
    /// <summary>
    /// Load Location
    /// </summary>
    private void LoadCbDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName AS Name FROM VHRDDepartment";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "Name";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
    }
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      //if (this.SaveData)
      //{ 

      //}
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

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["FurPid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsModified"].Hidden = true;
      e.Layout.Bands[0].Columns["Close"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["WorkOrder"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["WorkOrder"].MinWidth = 100;

      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;

      e.Layout.Bands[0].Columns["FurnitureCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["FurnitureCode"].MinWidth = 120;

      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 70;

      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 130;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 130;

      //e.Layout.Bands[0].Columns["MaterialNameVn"].MaxWidth = 250;
      //e.Layout.Bands[0].Columns["MaterialNameVn"].MinWidth = 250;

      e.Layout.Bands[0].Columns["Department"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Department"].MinWidth = 80;

      e.Layout.Bands[0].Columns["Remain"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Remain"].MinWidth = 100;

      e.Layout.Bands[0].Columns["Close"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Close"].MinWidth = 50;

      for (int j = 0; j < e.Layout.Bands[0].Columns.Count - 1; j++)
      {
        e.Layout.Bands[0].Columns[j].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Close"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["Close"].CellAppearance.BackColor = Color.LightGray;
      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        //if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
        //{
        // // e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
        //}
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          //e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName)
      {
        case "Close":
          {
            e.Cell.Row.Cells["IsModified"].Value = 1;
          }
          break;
        default:
          break;
      }
      this.NeedToSave = true;
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {

    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (SaveData())
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {

      }
      this.Search();
    }
    private bool SaveData()
    {
      DataTable dt = new DataTable();
      dt = (DataTable)ultData.DataSource;
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("PidCl", typeof(System.Int64));
      dtSource.Columns.Add("FurPid", typeof(System.Int64));
      dtSource.Columns.Add("MaterialCode", typeof(System.String));
      dtSource.Columns.Add("CreateBy", typeof(System.String));
      dtSource.Columns.Add("IsClose", typeof(System.Double));
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow row = dt.Rows[i];
        if (DBConvert.ParseInt(row["IsModified"].ToString()) == 1)
        {
          DataRow rowinsert = dtSource.NewRow();
          if (DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
          {
            rowinsert["PidCl"] = DBConvert.ParseLong(row["Pid"].ToString());
          }

          if (DBConvert.ParseLong(row["FurPid"].ToString()) != long.MinValue)
          {
            rowinsert["FurPid"] = DBConvert.ParseLong(row["FurPid"].ToString());
          }
          if (DBConvert.ParseString(row["MaterialCode"].ToString()) != null)
          {
            rowinsert["MaterialCode"] = DBConvert.ParseString(row["MaterialCode"].ToString());
          }
          rowinsert["CreateBy"] = SharedObject.UserInfo.UserPid;
          rowinsert["IsClose"] = DBConvert.ParseInt(row["Close"].ToString());
          dtSource.Rows.Add(rowinsert);
        }
      }
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        SqlDBParameter[] input = new SqlDBParameter[2];
        input[1] = new SqlDBParameter("@DataSource", SqlDbType.Structured, dtSource);
        SqlDBParameter[] output = new SqlDBParameter[1];
        output[0] = new SqlDBParameter("@Result", SqlDbType.Int, int.MinValue);
        SqlDataBaseAccess.ExecuteStoreProcedure("spGNRGetRequiredMaterials_Edit", input, output);
        long success = DBConvert.ParseLong(output[0].Value.ToString());
        if (success <= 0)
        {
          return false;
        }
      }
      return true;
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        Utility.GetDataForClipboard(ultData);
      }
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultData.Selected.Rows.Count > 0 || ultData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultData, new Point(e.X, e.Y));
        }
      }
    }
    #endregion Event

    private void btnClear_Click_1(object sender, EventArgs e)
    {
      chkRemain.Checked = false;
      ultcbFurCode.Value = DBNull.Value;
      ultcbItCode.Value = DBNull.Value;
      ultcbMaCode.Value = DBNull.Value;
      ultCBWo.Value = DBNull.Value;
      ultDepartment.Value = DBNull.Value;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }
    private void Search()
    {
      btnSearch.Enabled = false;
      DBParameter[] input = new DBParameter[7];
      // Wo
      if (ultCBWo.SelectedRow != null)
      {
        input[0] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultCBWo.Value.ToString()));
      }
      //FurnitureCode
      if (ultcbFurCode.SelectedRow != null)
      {
        input[1] = new DBParameter("@FurnitureCode", DbType.Int64, ultcbFurCode.SelectedRow.Cells["Pid"].Value.ToString().Trim());
      }
      //ItemCode
      if (ultcbItCode.SelectedRow != null)
      {
        input[2] = new DBParameter("@ItemCode", DbType.String, ultcbItCode.Value.ToString());
      }
      //MaterialCode
      if (ultcbMaCode.Value != null)
      {
        input[3] = new DBParameter("@MaterialCode", DbType.String, ultcbMaCode.Value.ToString());
      }
      //Location
      if (ultDepartment.SelectedRow != null)
      {
        input[4] = new DBParameter("@Department", DbType.String, ultDepartment.Value.ToString());
      }
      //Remain
      if (chkRemain.Checked)
      {
        input[5] = new DBParameter("@Remain", DbType.Int32, 1);
      }
      input[6] = new DBParameter("@RequestBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spGNRGetRequiredMaterials", input);
      ultData.DataSource = dt;
      lbCount.Text = string.Format("Count: {0}", ultData.Rows.Count.ToString());
      btnSearch.Enabled = true;
      checkGrid = false;
      chkAuto.Checked = false;
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultData, "Data");
    }

    private void chkAuto_CheckedChanged(object sender, EventArgs e)
    {

    }

    private void ultData_Click(object sender, EventArgs e)
    {
      //DataTable dt = (DataTable)ultData.DataSource;
      //for (int i = 0; i < dt.Rows.Count; i++)
      //{
      //  DataRow dr = dt.Rows[i];
      //  if (dr.RowState == DataRowState.Modified)
      //  {
      //    checkGrid = true;
      //    if (chkAuto.Checked == true)
      //    {
      //      chkAuto.Checked = false;
      //    }
      //  }
    }



    private void chkAuto_Click(object sender, EventArgs e)
    {
      int selected = (chkAuto.Checked ? 1 : 0);
      checkGrid = false;
      if (!checkGrid)
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].Cells["Close"].Value = selected;
        }
    }

    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      DataTable dt = (DataTable)ultData.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dr = dt.Rows[i];
        if (dr.RowState == DataRowState.Modified)
        {
          checkGrid = true;
          if (chkAuto.Checked == true)
          {
            chkAuto.Checked = false;
          }
        }
      }

    }
  }
}
