/*
  Author      : 
  Description : Veneer Requisition (Allocate WO & Carcass)
  Date        : 12/06/2013
  Kind        : 1 (Veneer Allocation for WO & Carcass)
              : 2 (Veneer Allocation for Department)
              : 3 (Veneer Allocation for Supplement)
              : 4 (Veneer No Allocation)
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using System.Collections;
using DaiCo.Shared.UserControls;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;
using VBReport;
using System.Diagnostics;
using Infragistics.Win;

namespace DaiCo.General
{
  public partial class viewGNR_06_003 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    public int type = int.MinValue;
    public string department = string.Empty;
    private bool flagSearch = false;
    private bool flagSum = false;

    #endregion Field

    #region Init

    public viewGNR_06_003()
    {
      InitializeComponent();
    }

    private void viewGNR_06_003_Load(object sender, EventArgs e)
    {
      if (this.type != 1)
      {
        this.txtWOFrom.Enabled = false;
        this.txtWOTo.Enabled = false;
        this.ultSupplement.Enabled = false;
      }

      // Load Init
      this.LoadInit();   
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      // Load Supplement
      this.LoadSupplement();

      // Load Carcass
      this.LoadCarcass();

      // Load MaterialCode
      this.LoadMaterialCode();
    }

    /// <summary>
    /// Create Datatable
    /// </summary>
    /// <returns></returns>
    private DataTable DataIssue()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("Kind", typeof(System.Int32));
      taParent.Columns.Add("WO", typeof(System.Int32));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("SupplementPid", typeof(System.Int64));
      taParent.Columns.Add("MainCode", typeof(System.String));
      taParent.Columns.Add("AltCode", typeof(System.String));
      taParent.Columns.Add("GetCode", typeof(System.String));
      taParent.Columns.Add("Cofficient", typeof(System.Double));
      taParent.Columns.Add("Require", typeof(System.Double));
      taParent.Columns.Add("RealQty", typeof(System.Double));

      return taParent;
    }

    /// <summary>
    /// Load Supplement
    /// </summary>
    private void LoadSupplement()
    {
      string commandText = string.Format(@" SELECT DISTINCT SUP.Pid, SUP.SupplementNo, SUP.Description, 
                                              CONVERT(VARCHAR, SUP.CreateDate, 103) CreateDate
                                            FROM TblPLNVeneerSupplement SUP
	                                              INNER JOIN TblPLNVeneerSupplementDetail DT ON (SUP.Pid = DT.SupplementPid)
                                            WHERE (SUP.Status = 1) AND (DT.IsCloseWork = 0)
                                            ORDER BY Pid DESC");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText, 120);
      ultSupplement.DataSource = dtSource;
      ultSupplement.ValueMember = "Pid";
      ultSupplement.DisplayMember = "SupplementNo";
      ultSupplement.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultSupplement.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Carcass
    /// </summary>
    private void LoadCarcass()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ALLO.CarcassCode, CAR.[Description] ";
      commandText += " FROM TblPLNVeneerAllocateWorkOrderSummary ALLO ";
      commandText += " 	INNER JOIN TblBOMCarcass CAR ON ALLO.CarcassCode = CAR.CarcassCode ";
      commandText += " ORDER BY ALLO.CarcassCode ";

      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCarcassCode.DataSource = dtCarcass;
      ucCarcassCode.ColumnWidths = "200;400";
      ucCarcassCode.DataBind();
      ucCarcassCode.ValueMember = "CarcassCode";
      ucCarcassCode.DisplayMember = "CarcassCode";
    }

    /// <summary>
    /// Load Carcass Base On WO
    /// </summary>
    private void LoadCarcassBaseOnWO()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ALLO.CarcassCode, CAR.[Description] ";
      commandText += " FROM TblPLNVeneerAllocateWorkOrderSummary ALLO ";
      commandText += " 	INNER JOIN TblBOMCarcass CAR ON ALLO.CarcassCode = CAR.CarcassCode ";
      commandText += " WHERE 1 = 1 ";

      if(ultCBMaterialCode.Value != null && ultCBMaterialCode.Text.Length > 0)
      {
        commandText += " 	AND ALLO.MaterialCode = '" + ultCBMaterialCode.Value.ToString() + "'";
      }

      if (txtWOFrom.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOFrom.Text.Trim()) != int.MinValue)
      {
        commandText += " 	AND ALLO.WO >= " + DBConvert.ParseInt(txtWOFrom.Text.Trim());
      }

      if (txtWOTo.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWOTo.Text.Trim()) != int.MinValue)
      {
        commandText += " 	AND ALLO.WO <= " + DBConvert.ParseInt(txtWOTo.Text.Trim());
      }
      commandText += " ORDER BY ALLO.CarcassCode ";

      DataTable dtCarcass = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCarcassCode.DataSource = dtCarcass;
      ucCarcassCode.ColumnWidths = "200;400";
      ucCarcassCode.DataBind();
      ucCarcassCode.ValueMember = "CarcassCode";
      ucCarcassCode.DisplayMember = "CarcassCode";
    }

    /// <summary>
    /// Load MaterialCode
    /// </summary>
    private void LoadMaterialCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT MaterialCode, MaterialCode + ' - ' + MaterialNameVn AS Name ";
      commandText += " FROM VBOMMaterials ";
      commandText += " WHERE Warehouse = 2 ";
      commandText += " ORDER BY MaterialCode ";
      
      DataTable dtMaterialCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBMaterialCode.DataSource = dtMaterialCode;
      ultCBMaterialCode.ValueMember = "MaterialCode";
      ultCBMaterialCode.DisplayMember = "MaterialCode";
      ultCBMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Hidden = true;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Enter Search
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter && btnSave.Enabled == true)
      {
        this.btnSearch.Enabled = false;

        // Search
        this.Search();

        this.btnSearch.Enabled = true;
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      // Check Valid MaterialCode
      if (ultCBMaterialCode.Value == null || ultCBMaterialCode.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "MaterialCode");
        return;
      }
      // End

      DBParameter[] inputParam = new DBParameter[7];
      // WO From
      if (txtWOFrom.Text.Length > 0)
      {
        inputParam[0] = new DBParameter("@WOFrom", DbType.Int32, DBConvert.ParseInt(txtWOFrom.Text));
      }
      // WO To
      if (txtWOTo.Text.Length > 0)
      {
        inputParam[1] = new DBParameter("@WOTo", DbType.Int32, DBConvert.ParseInt(txtWOTo.Text));
      }
      // CarcassCode
      if (txtCarcassCode.Text.Length > 0)
      {
        inputParam[2] = new DBParameter("@CarcassCode", DbType.String, txtCarcassCode.Text);
      }
      // Supplement
      if(ultSupplement.Value != null && DBConvert.ParseLong(ultSupplement.Value.ToString()) > 0)
      {
        inputParam[3] = new DBParameter("@Supplement", DbType.Int64, DBConvert.ParseLong(ultSupplement.Value.ToString()));
      }
      // Department
      inputParam[4] = new DBParameter("@Department", DbType.String, this.department);
      // Type 
      inputParam[5] = new DBParameter("@TypeSearch", DbType.Int32, this.type);
      // MaterialCode
      if (ultCBMaterialCode.Value != null && ultCBMaterialCode.Text.Length > 0)
      {
        inputParam[6] = new DBParameter("@CodeName", DbType.String, ultCBMaterialCode.Value.ToString());
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spGNRVeneerGetRequestOnline_Select", inputParam);
      if(dsSource != null)
      {
        ultSearch.DataSource = dsSource.Tables[0];
        ultData.DataSource = dsSource.Tables[1];

        // To mau dua vao Kind
        for (int i = 0; i < ultSearch.Rows.Count; i++)
        {
          UltraGridRow row = ultSearch.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 1)
          {
            row.Cells["WO"].Appearance.BackColor = Color.LightSalmon;
            row.Cells["CarcassCode"].Appearance.BackColor = Color.LightSalmon;
            row.Cells["ItemCode"].Appearance.BackColor = Color.LightSalmon;
          }
          else if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 2)
          {
            row.Cells["WO"].Appearance.BackColor = Color.GreenYellow;
            row.Cells["CarcassCode"].Appearance.BackColor = Color.GreenYellow;
            row.Cells["ItemCode"].Appearance.BackColor = Color.GreenYellow;
          }
          else if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 3)
          {
            row.Cells["WO"].Appearance.BackColor = Color.Pink;
            row.Cells["CarcassCode"].Appearance.BackColor = Color.Pink;
            row.Cells["ItemCode"].Appearance.BackColor = Color.Pink;
          }
          else if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 4)
          {
            row.Cells["WO"].Appearance.BackColor = Color.Aquamarine;
            row.Cells["CarcassCode"].Appearance.BackColor = Color.Aquamarine;
            row.Cells["ItemCode"].Appearance.BackColor = Color.Aquamarine;
          }
        }

        // To mau khi CurrentRemain > Remain 
        for (int j = 0; j < ultData.Rows.Count; j++)
        {
          UltraGridRow row = ultData.Rows[j];
          if (DBConvert.ParseDouble(row.Cells["CurrentRemain"].Value.ToString()) >
              DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()))
          {
            row.Cells["Remain"].Appearance.BackColor = Color.Yellow;
            row.Cells["CurrentRemain"].Appearance.BackColor = Color.Yellow;
            row.Cells["QtySuggest"].Appearance.BackColor = Color.Yellow;
          }
          else
          {
            row.Cells["Remain"].Appearance.BackColor = Color.White;
            row.Cells["CurrentRemain"].Appearance.BackColor = Color.White;
            row.Cells["QtySuggest"].Appearance.BackColor = Color.White;
          }
        }
      }
      this.chkAuto.Checked = false;
      this.chkAuto.Checked = true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      DataTable dtMain = (DataTable)ultSearch.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if (DBConvert.ParseDouble(row["Require"].ToString()) > 0)
        {
          DBParameter[] input = new DBParameter[10];
          // Pid
          input[0] = new DBParameter("@VRNPid", DbType.Int64, this.pid);
          // Type
          input[1] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(row["Kind"].ToString()));
          // WO
          if (DBConvert.ParseInt(row["WO"].ToString()) != int.MinValue)
          {
            input[2] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row["WO"].ToString()));
          }
          // CarcassCode
          if (row["CarcassCode"].ToString().Length > 0)
          {
            input[3] = new DBParameter("@CarcassCode", DbType.String, row["CarcassCode"].ToString());
          }
          // Supplement
          if (DBConvert.ParseLong(row["SupplementPid"].ToString()) != long.MinValue)
          {
            input[4] = new DBParameter("@SupplementPid", DbType.Int64, DBConvert.ParseLong(row["SupplementPid"].ToString()));
          }
          //MainCode
          if (row["MainCode"].ToString().Length > 0)
          {
            input[5] = new DBParameter("@MainCode", DbType.String, row["MainCode"].ToString());
          }
          // Alternative Code
          if (row["AltCode"].ToString().Length > 0)
          {
            input[6] = new DBParameter("@AlternativeCode", DbType.String, row["AltCode"].ToString());
          }
          // QtyAllocate
          input[7] = new DBParameter("@QtyAllocate", DbType.Double, 
                  DBConvert.ParseDouble(row["Require"].ToString()) / DBConvert.ParseDouble(row["Cofficient"].ToString()));
          // GetCode
          input[8] = new DBParameter("@MaterialCode", DbType.String, row["GetCode"].ToString());
          // Require
          input[9] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row["Require"].ToString()));
      
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure("spGNRVeneerRequisitionNoteDetail_Insert", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check Data
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseDouble(row.Cells["CurrentRemain"].Value.ToString()) <= DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()))
        {
          if (DBConvert.ParseDouble(row.Cells["QtySuggest"].Value.ToString()) > DBConvert.ParseDouble(row.Cells["CurrentRemain"].Value.ToString()))
          {
            message = "QtySuggest <= CurrentRemain";
            return false;
          }
        }
        else
        {
          if (DBConvert.ParseDouble(row.Cells["QtySuggest"].Value.ToString()) > DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()))
          {
            message = "QtySuggest <= Remain";
            return false;
          }
        }
      }
      return true;
    }

    #endregion Function

    #region Event

    /// <summary>
    /// InitData
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Columns["GetCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Balance"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Required"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remain"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CurrentRemain"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyIssue"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Required"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CurrentRemain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyIssue"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtySuggest"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Init Data (Search)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearch_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultSearch);

      e.Layout.Bands[0].Columns["Kind"].Hidden = true;
      e.Layout.Bands[0].Columns["SupplementPid"].Hidden = true;

      e.Layout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Cofficient"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalRequired"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AllocatedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["IssuedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Required"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Require"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Auto"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Require"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["GetCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["GetName"].CellAppearance.BackColor = Color.Yellow;

      e.Layout.Bands[0].Columns["Balance"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["TotalRequired"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["AllocatedQty"].CellAppearance.BackColor = Color.Khaki;
      e.Layout.Bands[0].Columns["IssuedQty"].CellAppearance.BackColor = Color.Khaki;
      e.Layout.Bands[0].Columns["Required"].CellAppearance.BackColor = Color.Khaki;

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void txtWOFrom_Leave(object sender, EventArgs e)
    {
      // Load Carcass Base On WO
      this.LoadCarcassBaseOnWO();
    }

    private void txtWOTo_Leave(object sender, EventArgs e)
    {
      // Load Carcass Base On WO
      this.LoadCarcassBaseOnWO();
    }

    /// <summary>
    /// Check Show Group Carcass
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowGroup_CheckedChanged(object sender, EventArgs e)
    {
      ucCarcassCode.Visible = chkShowGroup.Checked;
    }

    /// <summary>
    /// Auto Check
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkAuto_CheckedChanged(object sender, EventArgs e)
    {
      int selected = 0;
      if (chkAuto.Checked)
      {
        selected = 1;
      }
      for (int i = 0; i < ultSearch.Rows.Count; i++)
      {
        ultSearch.Rows[i].Cells["Auto"].Value = selected;
      }
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearch_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();

      switch (columnName)
      {
        case "auto":
          if (this.flagSum == false)
          {
            if (DBConvert.ParseInt(e.Cell.Row.Cells["Auto"].Value.ToString()) == 1)
            {
              e.Cell.Row.Cells["Require"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
            }
            else
            {
              e.Cell.Row.Cells["Require"].Value = 0;
            }
          }
          break;
        case "require":
          if (this.flagSum == false)
          {
            this.flagSearch = true;
            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              double qtySuggest = 0;
              UltraGridRow rowSum = ultData.Rows[i];
              for (int j = 0; j < ultSearch.Rows.Count; j++)
              {
                UltraGridRow rowInfo = ultSearch.Rows[j];
                if (string.Compare(rowSum.Cells["GetCode"].Value.ToString(), rowInfo.Cells["GetCode"].Value.ToString(), true) == 0)
                {
                  if (DBConvert.ParseDouble(rowInfo.Cells["Require"].Value.ToString()) > 0 &&
                       DBConvert.ParseDouble(rowInfo.Cells["Require"].Value.ToString()) <= DBConvert.ParseDouble(rowInfo.Cells["Remain"].Value.ToString()))
                  {
                    qtySuggest = qtySuggest + DBConvert.ParseDouble(rowInfo.Cells["Require"].Value.ToString());
                  }
                }
              }
              rowSum.Cells["QtySuggest"].Value = Math.Round(qtySuggest, 3);
            }
            this.flagSearch = false;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearch_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;

      switch (columnName)
      {
        case "require":
          if (DBConvert.ParseDouble(row.Cells["Require"].Text) < 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Require > 0");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true; ;
          }
          else if (DBConvert.ParseDouble(row.Cells["Require"].Text) >
                    DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()))
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Require <= Remain");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;

      switch (columnName)
      {
        case "qtysuggest":
          if (this.flagSearch == false)
          {
            if (DBConvert.ParseDouble(row.Cells["QtySuggest"].Text) < 0)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "QtySuggest > 0");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
            }
            else if (DBConvert.ParseDouble(row.Cells["QtySuggest"].Text) >
                      DBConvert.ParseDouble(row.Cells["CurrentRemain"].Value.ToString()))
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "QtySuggest <= CurrentRemain");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "qtysuggest":
          if (this.flagSearch == false)
          {
            this.flagSum = true;
            double qtySuggest = DBConvert.ParseDouble(e.Cell.Row.Cells["QtySuggest"].Value.ToString());
            for (int i = 0; i < ultSearch.Rows.Count; i++)
            {
              UltraGridRow row = ultSearch.Rows[i];
              if (qtySuggest > DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()))
              {
                row.Cells["Require"].Value = DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString());
                row.Cells["Auto"].Value = 1;
                qtySuggest = qtySuggest - DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString());
              }
              else
              {
                if (qtySuggest > 0)
                {
                  row.Cells["Require"].Value = qtySuggest;
                  row.Cells["Auto"].Value = 1;
                  qtySuggest = 0;
                }
                else
                {
                  row.Cells["Require"].Value = 0;
                  row.Cells["Auto"].Value = 0;
                }
              }
            }
            this.flagSum = false;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Cell Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSearch_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "auto":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Auto"].Text) == 1)
          {
            e.Cell.Row.Cells["Require"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Remain"].Value.ToString());
          }
          else
          {
            e.Cell.Row.Cells["Require"].Value = 0;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Change CarcassCode
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucCarcassCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtCarcassCode.Text = ucCarcassCode.SelectedValue;
    }

    private void ultCBMaterialCode_ValueChanged(object sender, EventArgs e)
    {
      // Load Carcass Base On WO
      this.LoadCarcassBaseOnWO();
    }

    /// <summary>
    /// Double Click Choose IDVeneer
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      // Check Save
      if(btnSave.Enabled == true)
      {
        string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Please Save Data !");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      // End

      UltraGridRow rowSelect = ultData.Selected.Rows[0];
      if(DBConvert.ParseDouble(rowSelect.Cells["QtyIssue"].Value.ToString()) > 0)
      {
        return;
      }
      string materialCode = rowSelect.Cells["GetCode"].Value.ToString();
      string materialName = rowSelect.Cells["Name"].Value.ToString();
      double qtySuggestIssue = DBConvert.ParseDouble(rowSelect.Cells["QtySuggest"].Value.ToString());

      viewGNR_06_005 uc = new viewGNR_06_005();
      uc.pid = this.pid;
      uc.materialCode = materialCode;
      uc.materialName = materialName;
      uc.qtysuggest = qtySuggestIssue;

      WindowUtinity.ShowView(uc, "REQUEST ONLINE DETAIL (MATERIAL CODE)", false, ViewState.ModalWindow, FormWindowState.Maximized);

      this.ultData.Rows[0].Cells["QtyIssue"].Value = uc.qtyAfterSave;
      this.ultData.Rows[0].Cells["QtyIssue"].Appearance.BackColor = Color.Yellow;

      // Format Search
      this.ultSearch.DataSource = null;
      this.ultCBMaterialCode.Value = null;
      this.txtWOFrom.Text = string.Empty;
      this.txtWOTo.Text = string.Empty;
      this.ultSupplement.Value = null;
      this.txtCarcassCode.Text = string.Empty;
    }

    /// <summary>
    /// Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;
      // Search
      this.Search();
      this.btnSearch.Enabled = true;
    }

    /// <summary>
    /// Save Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      DataTable dtData = (DataTable)this.ultData.DataSource;
      if (dtData == null || dtData.Rows.Count == 0)
      {
        return;
      }

      string message = string.Empty;
      // 1: Check Valid
      bool success = this.CheckVaild(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }    
      btnSave.Enabled = false;
      btnSearch.Enabled = false;
      ultData.DisplayLayout.Bands[0].Columns["QtySuggest"].CellActivation = Activation.ActivateOnly;
    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event
  }
}
