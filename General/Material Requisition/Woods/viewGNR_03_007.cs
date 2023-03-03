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
  public partial class viewGNR_03_007 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    public string group = string.Empty;
    public DataTable dtSuggest = new DataTable();
    public double qtySugguestIssue = long.MinValue;
    private double qtyIssue = 0;
    private bool flag = false;
    public string dept = string.Empty;
    public double qtyAfterSave = 0;
    #endregion Field

    #region Init

    public viewGNR_03_007()
    {
      InitializeComponent();
    }

    private void viewGNR_03_007_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void LoadData()
    {
      this.lblQtySuggest.Text = "Suggest: " + qtySugguestIssue.ToString();

      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Group", DbType.String, this.group);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spGNRWoodsLoadMaterial_Select", inputParam);

      if (dsSource != null)
      {
        if (dsSource.Tables[0].Rows.Count == 1)
        {
          this.txtGroupCategory.Text = dsSource.Tables[0].Rows[0]["Name"].ToString();
        }

        DataSet ds = this.DsLoadGrid();
        ds.Tables["dtParent"].Merge(dsSource.Tables[1]);
        ds.Tables["dtChild"].Merge(dsSource.Tables[2]);

        this.ultData.DataSource = ds;
      } 
    }

    private DataSet DsLoadGrid()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("NameEN", typeof(System.String));
      taParent.Columns.Add("NameVN", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("FactoryUnit", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("Required", typeof(System.Double));
      taParent.Columns.Add("Auto", typeof(System.Int32));
      taParent.Columns.Add("Require2", typeof(System.Double));
      ds.Tables.Add(taParent);
      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("Package", typeof(System.String));
      taChild.Columns.Add("Location", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.Double));
      taChild.Columns.Add("AutoPak", typeof(System.Int32));
      taChild.Columns.Add("Require3", typeof(System.Double));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["MaterialCode"], taChild.Columns["MaterialCode"], false));
      return ds;
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
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("Require", typeof(System.Double));

      return taParent;
    }

    /// <summary>
    /// Save
    /// </summary>
    private bool SavaData()
    {
      // Create Format Data
      DataTable dt = this.DataIssue();

      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowData = this.ultData.Rows[i];
        string materialCode = rowData.Cells["MaterialCode"].Value.ToString();
        double require = DBConvert.ParseDouble(rowData.Cells["Require2"].Value.ToString());
        if (require > 0)
        {
          for (int j = 0; j < this.dtSuggest.Rows.Count; j++)
          {
            DataRow row = this.dtSuggest.Rows[j];
            double requireChild = DBConvert.ParseDouble(row["Require"].ToString());
            double realQty = DBConvert.ParseDouble(row["RealQty"].ToString());
            if (requireChild - realQty > 0)
            {
              DataRow rowNew = dt.NewRow();
              rowNew["Kind"] = DBConvert.ParseInt(row["Kind"].ToString());
              if (DBConvert.ParseInt(row["WO"].ToString()) != int.MinValue)
              {
                rowNew["WO"] = DBConvert.ParseInt(row["WO"].ToString());
              }
              rowNew["CarcassCode"] = row["CarcassCode"].ToString();
              if (DBConvert.ParseInt(row["Kind"].ToString()) == 3)
              {
                rowNew["SupplementPid"] = DBConvert.ParseLong(row["SupplementPid"].ToString());
              }
              rowNew["MainCode"] = row["MainCode"].ToString();
              rowNew["AltCode"] = row["AltCode"].ToString();
              rowNew["GetCode"] = row["GetCode"].ToString();
              rowNew["Cofficient"] = DBConvert.ParseDouble(row["Cofficient"].ToString());
              rowNew["MaterialCode"] = materialCode;

              if (require > requireChild - realQty)
              {
                rowNew["Require"] = requireChild - realQty;
                require = require - (requireChild - realQty);
                row["RealQty"] = DBConvert.ParseDouble(row["RealQty"].ToString())
                          + (requireChild - realQty);
                dt.Rows.Add(rowNew);
              }
              else
              {
                rowNew["Require"] = require;
                row["RealQty"] = DBConvert.ParseDouble(row["RealQty"].ToString()) + require;
                dt.Rows.Add(rowNew);
                break;
              }
            }
          }
        }
      }

      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow row = dt.Rows[i];
        DBParameter[] inputParam = new DBParameter[13];
        inputParam[0] = new DBParameter("@MRNPid", DbType.Int64, this.pid);
        inputParam[1] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(row["Kind"].ToString()));
        if (DBConvert.ParseLong(row["WO"].ToString()) != long.MinValue)
        {
          inputParam[2] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row["WO"].ToString()));
        }
        inputParam[3] = new DBParameter("@CarcassCode", DbType.String, row["CarcassCode"].ToString());
        if (DBConvert.ParseLong(row["SupplementPid"].ToString()) > 0)
        {
          inputParam[4] = new DBParameter("@SupplementDetailPid", DbType.Int64, DBConvert.ParseLong(row["SupplementPid"].ToString()));
        }
        if (row["MainCode"].ToString().Length > 0)
        {
          inputParam[5] = new DBParameter("@MainGroup", DbType.String, row["MainCode"].ToString().Substring(0, 3));
          inputParam[6] = new DBParameter("@MainCategory", DbType.String, row["MainCode"].ToString().Substring(4, 2));
        }
        if (row["AltCode"].ToString().Length > 0)
        {
          inputParam[7] = new DBParameter("@AltGroup", DbType.String, row["AltCode"].ToString().Substring(0, 3));
          inputParam[8] = new DBParameter("@AltCategory", DbType.String, row["AltCode"].ToString().Substring(4, 2));
        }
        inputParam[9] = new DBParameter("@QtyAllocated", DbType.Double,
            DBConvert.ParseDouble(row["Require"].ToString()) / DBConvert.ParseDouble(row["Cofficient"].ToString()));
        inputParam[10] = new DBParameter("@MaterialCode", DbType.String, row["MaterialCode"].ToString());
        inputParam[11] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row["Require"].ToString()));
        inputParam[12] = new DBParameter("@DepartmentRequest", DbType.String, this.dept);

        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spGNRWoodsRequisitionNoteDetail_Edit", inputParam, outputParam);
        if (DBConvert.ParseInt(outputParam[0].Value.ToString()) <= 0)
        {
          return false;
        }

        qtyAfterSave += DBConvert.ParseDouble(row["Require"].ToString());
      }
      return true;
    }

    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid()
    {
      string message = string.Empty;
      //for (int i = 0; i < ultData.Rows.Count; i++)
      //{
      //  UltraGridRow row = ultData.Rows[i];
      //  // Check Require
      //  if (DBConvert.ParseDouble(row.Cells["Require2"].Value.ToString()) > 0)
      //  {
      //    if (DBConvert.ParseDouble(row.Cells["Require2"].Value.ToString())
      //        > DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString())
      //          + DBConvert.ParseDouble(row.Cells["Required"].Value.ToString()))
      //    {
      //      message = "Require Must Less Or Equal Qty - Required";
      //      WindowUtinity.ShowMessageErrorFromText(message);
      //      return false;
      //    }
      //  }
      //}

      // Check total
      if (this.qtySugguestIssue < this.qtyIssue)
      {
        message = "Require Must Less Or Equal Sugguest Qty";
        WindowUtinity.ShowMessageErrorFromText(message);
        return false;
      }
      return true;
    }

    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Columns["Auto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Required"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Require2"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Require2"].Header.Caption = "Require";
      e.Layout.Bands[0].Columns["MaterialCode"].CellAppearance.BackColor = Color.YellowGreen;
      e.Layout.Bands[0].Columns["Auto"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Require2"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Auto"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Auto"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Require2"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Require2"].MinWidth = 150;

      e.Layout.Bands[1].Columns["AutoPak"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["MaterialCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Require3"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Require3"].Header.Caption = "Require";
      e.Layout.Bands[1].Columns["AutoPak"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[1].Columns["Require3"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[1].Columns["AutoPak"].MaxWidth = 50;
      e.Layout.Bands[1].Columns["AutoPak"].MinWidth = 50;
      e.Layout.Bands[1].Columns["Require3"].MaxWidth = 150;
      e.Layout.Bands[1].Columns["Require3"].MinWidth = 150;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;

      switch (columnName)
      {
        case "require3":
          UltraGridRow rowParent = row.ParentRow;
          double totalRequire3 = 0;
          for (int i = 0; i < rowParent.ChildBands[0].Rows.Count; i++)
          {
            if (DBConvert.ParseDouble(rowParent.ChildBands[0].Rows[i].Cells["Require3"].Value.ToString()) > 0)
            {
              totalRequire3 = totalRequire3 + DBConvert.ParseDouble(rowParent.ChildBands[0].Rows[i].Cells["Require3"].Value.ToString());
            }
          }
          rowParent.Cells["Require2"].Value = totalRequire3;
          break;
        case "require2":
          double saiso = 0.001;
          //if (DBConvert.ParseDouble(row.Cells["Require2"].Value.ToString()) >
          //            DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) - DBConvert.ParseDouble(row.Cells["Required"].Value.ToString()) + saiso)
          //{
          //  string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Require <= Qty - Required");
          //  WindowUtinity.ShowMessageErrorFromText(message);
          //  row.Cells["Require2"].Appearance.BackColor = Color.Yellow;
          //}
          //else
          //{
          //  row.Cells["Require2"].Appearance.BackColor = Color.LightBlue;
          //}

          // TotalQty
          DataSet ds = (DataSet)ultData.DataSource;
          double totalQty = 0;
          for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
          {
            if (DBConvert.ParseDouble(ds.Tables[0].Rows[i]["Require2"].ToString()) > 0)
            {
              totalQty = totalQty + DBConvert.ParseDouble(ds.Tables[0].Rows[i]["Require2"].ToString());
            }
          }
          // Gan Lable
          lblQtyIssue.Text = "Issue: " + totalQty;
          qtyIssue = totalQty;
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
        case "require2":
          if (this.flag == false)
          {
            if (DBConvert.ParseDouble(row.Cells["Require2"].Text) < 0)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Require > 0");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
            }
          }
          break;
        case "require3":
          if (DBConvert.ParseDouble(row.Cells["Require3"].Text) < 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Require > 0");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          else if (DBConvert.ParseDouble(row.Cells["Require3"].Text) >
                    DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()))
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Require <= Qty");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }

    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "auto":
          this.flag = true;
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Auto"].Text) == 1)
          {
            e.Cell.Row.Cells["Require2"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString()) -
                                                  DBConvert.ParseDouble(e.Cell.Row.Cells["Required"].Value.ToString());
          }
          else
          {
            e.Cell.Row.Cells["Require2"].Value = 0;
          }
          this.flag = false;
          break;
        case "autopak":
          if (DBConvert.ParseInt(e.Cell.Row.Cells["AutoPak"].Text) == 1)
          {
            e.Cell.Row.Cells["Require3"].Value = DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString());
          }
          else
          {
            e.Cell.Row.Cells["Require3"].Value = 0;
          }
          break;
        default:
          break;
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid();
      if (!success)
      {
        return;
      }

      success = this.SavaData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }

      this.CloseTab();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion Event
  }
}
