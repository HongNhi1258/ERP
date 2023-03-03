/*
  Author      : 
  Description : Woods Requisition Info
  Date        : 24/05/2013
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
  public partial class viewGNR_03_006 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    public int type = int.MinValue;
    public string department = string.Empty;

    private bool flagSearch = false;
    private bool flagSum = false;
    #endregion Field

    #region Init

    public viewGNR_03_006()
    {
      InitializeComponent();
    }

    private void viewGNR_03_006_Load(object sender, EventArgs e)
    {
      if (this.type != 1)
      {
        this.txtWOFrom.Enabled = false;
        this.txtWOTo.Enabled = false;
        this.ultSupplement.Enabled = false;
      }
      // Load Group
      this.LoadGroupCategory();

      // Load Supplement
      this.LoadSupplement();

      // Load Carcass
      this.LoadCarcass();
    }

    #endregion Init

    #region Function

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
    /// Supplement
    /// </summary>
    private void LoadSupplement()
    {
      string commandText = string.Format(@" SELECT DISTINCT SUP.Pid, SUP.SupplementNo, SUP.Description, 
                                              CONVERT(VARCHAR, SUP.CreateDate, 103) CreateDate
                                            FROM TblPLNWoodsSupplement SUP
	                                              INNER JOIN TblPLNWoodsSupplementDetail DT ON (SUP.Pid = DT.SupplementPid)
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
    /// Group - Category
    /// </summary>
    private void LoadGroupCategory()
    {
      string commandText = string.Empty;
      commandText += " SELECT CAT.[Group] + '-' + CAT.Category Code, ";
      commandText += "      CAT.[Group] + '-' + CAT.Category + ' ' + CAT.Name Name";
      commandText += " FROM TblGNRMaterialCategory CAT ";
      commandText += "    INNER JOIN TblGNRMaterialGroup GRP ON CAT.[Group] = GRP.[Group] ";
      commandText += " 								AND GRP.Warehouse = 3 ";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultGroup.DataSource = dtSource;
      ultGroup.ValueMember = "Code";
      ultGroup.DisplayMember = "Name";
      ultGroup.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultGroup.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Carcass
    /// </summary>
    private void LoadCarcass()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ALLO.CarcassCode, CAR.[Description] ";
      commandText += " FROM TblPLNWoodsAllocateWorkOrderSummary ALLO ";
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
    /// Enter Search
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
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
      // Check Valid
      if (ultGroup.Value == null || ultGroup.Value.ToString().Length == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Group-Category");
        return;
      }

      DBParameter[] inputParam = new DBParameter[7];
      if (txtWOFrom.Text.Length > 0)
      {
        inputParam[0] = new DBParameter("@WOFrom", DbType.Int32, DBConvert.ParseInt(txtWOFrom.Text));
      }
      if (txtWOTo.Text.Length > 0)
      {
        inputParam[1] = new DBParameter("@WOTo", DbType.Int32, DBConvert.ParseInt(txtWOTo.Text));
      }

      inputParam[2] = new DBParameter("@CodeName", DbType.String, ultGroup.Value.ToString());
      if (txtCarcassCode.Text.Length > 0)
      {
        inputParam[3] = new DBParameter("@CarcassCode", DbType.String, txtCarcassCode.Text);
      }
      if(ultSupplement.Value != null && DBConvert.ParseLong(ultSupplement.Value.ToString()) > 0)
      {
        inputParam[4] = new DBParameter("@Supplement", DbType.Int64, DBConvert.ParseLong(ultSupplement.Value.ToString()));
      }
      inputParam[5] = new DBParameter("@Department", DbType.String, this.department);
      inputParam[6] = new DBParameter("@TypeSearch", DbType.Int32, this.type);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spGNRGetRequestOnlineWoods_Select", inputParam);
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
            row.Cells["MainCode"].Appearance.BackColor = Color.Pink;
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
    /// Load Carcass Base On WO
    /// </summary>
    private void LoadCarcassBaseOnWO()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ALLO.CarcassCode, CAR.[Description] ";
      commandText += " FROM TblPLNWoodsAllocateWorkOrderSummary ALLO ";
      commandText += " 	INNER JOIN TblBOMCarcass CAR ON ALLO.CarcassCode = CAR.CarcassCode ";
      commandText += " WHERE 1 = 1 ";
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
    #endregion Function

    #region Event

    private void ucCarcassCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtCarcassCode.Text = ucCarcassCode.SelectedValue;
    }

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

    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      UltraGridRow rowSelect = ultData.Selected.Rows[0];

      string groupCategory = rowSelect.Cells["GetCode"].Value.ToString();
      double qtySuggestIssue = DBConvert.ParseDouble(rowSelect.Cells["QtySuggest"].Value.ToString());

      // Create Format Data
      DataTable dt = this.DataIssue();

      for (int i = 0; i < ultSearch.Rows.Count; i++)
      {
        UltraGridRow row = this.ultSearch.Rows[i];
    
        if (Math.Round(DBConvert.ParseDouble(row.Cells["Require"].Value.ToString()), 3) > 0)
        {
          DataRow rowAdd = dt.NewRow();
          rowAdd["Kind"] = DBConvert.ParseInt(row.Cells["Kind"].Value.ToString());
          rowAdd["WO"] = DBConvert.ParseInt(row.Cells["WO"].Value.ToString());
          rowAdd["CarcassCode"] = row.Cells["CarcassCode"].Value.ToString();
          if (DBConvert.ParseInt(row.Cells["Kind"].Value.ToString()) == 3)
          {
            rowAdd["SupplementPid"] = DBConvert.ParseLong(row.Cells["SupplementPid"].Value.ToString());
          }
          rowAdd["MainCode"] = row.Cells["MainCode"].Value.ToString();
          rowAdd["AltCode"] = row.Cells["AltCode"].Value.ToString();
          rowAdd["GetCode"] = row.Cells["GetCode"].Value.ToString();
          rowAdd["Cofficient"] = DBConvert.ParseDouble(row.Cells["Cofficient"].Value.ToString());
          rowAdd["Require"] = DBConvert.ParseDouble(row.Cells["Require"].Value.ToString());
          rowAdd["RealQty"] = 0;
          dt.Rows.Add(rowAdd);
        }
      }

      viewGNR_03_007 uc = new viewGNR_03_007();
      uc.pid = this.pid;
      uc.group = groupCategory;
      uc.dtSuggest = dt;
      uc.qtySugguestIssue = qtySuggestIssue;
      uc.dept = this.department;
      WindowUtinity.ShowView(uc, "REQUEST ONLINE DETAIL (MATERIAL CODE)", false, ViewState.ModalWindow, FormWindowState.Maximized);

      this.ultData.Rows[0].Cells["QtyIssue"].Value = uc.qtyAfterSave;
      this.ultData.Rows[0].Cells["QtyIssue"].Appearance.BackColor = Color.Yellow;

      // Format Search
      this.ultSearch.DataSource = null;
      this.ultGroup.Value = null;
      this.txtWOFrom.Text = string.Empty;
      this.txtWOTo.Text = string.Empty;
      this.ultSupplement.Value = null;
      this.txtCarcassCode.Text = string.Empty;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;

      // Search
      this.Search();

      this.btnSearch.Enabled = true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
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

    private void chkShowGroup_CheckedChanged(object sender, EventArgs e)
    {
      ucCarcassCode.Visible = chkShowGroup.Checked;
    }

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
    #endregion Event
  }
}
