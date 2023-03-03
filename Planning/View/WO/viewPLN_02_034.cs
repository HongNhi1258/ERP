/*
  Author      : Huynh Thi Bang
  Date        : 31/03/2016
  Description : Update Leadtime for Supplier on Separate Part Subcon
  Standard Form: viewPLN_02_034
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_034 : MainUserControl
  {
    #region field
    #endregion field
    #region Init
    public viewPLN_02_034()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_034_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);
      this.LoadSupplier();
    }
    #endregion Init

    #region function
    /// <summary>
    /// Load Supplier 
    /// </summary>
    private void LoadSupplier()
    {
      string cmText = @"SELECT Pid Value, DefineCode + ' - ' + EnglishName Display
                        FROM TblPURSupplierInfo
                        WHERE DefineCode IS NOT NULL";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      ultraDropSupplier.DataSource = dt;
      ultraDropSupplier.DisplayMember = "Display";
      ultraDropSupplier.ValueMember = "Value";
      ultraDropSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDropSupplier.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      string storeName = "spPLNListUpdateLeadtime_Select";
      string itemCode = txtItemCode.Text.Trim().ToString();
      string carcassCode = txtCarcassCode.Text.Trim().ToString();
      DBParameter[] inputParam = new DBParameter[2];
      if (itemCode.Length > 0)
      {
        inputParam[0] = new DBParameter("@ItemCode", DbType.String, itemCode);
      }
      if (carcassCode.Length > 0)
      {
        inputParam[1] = new DBParameter("@CarcassCode", DbType.String, carcassCode);
      }
      DataSet dtSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      dtSource.Relations.Add(new DataRelation("TblMaster_TblDetail", dtSource.Tables[0].Columns["PidMaster"], dtSource.Tables[1].Columns["MasterPid"], false));
      if (dtSource != null)
      {
        ultraGridInformation.DataSource = dtSource;
      }
      lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtItemCode.Text = string.Empty;
      txtCarcassCode.Text = string.Empty;
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
    #endregion function

    #region event

    /// <summary>
    /// Search Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }
    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }
    /// <summary>
    /// Check Valid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        int isUpdate = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["IsUpdate"].Value);
        if (isUpdate == 1)
          for (int j = 0; j < ultraGridInformation.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowChild = ultraGridInformation.Rows[i].ChildBands[0].Rows[j];
            long supplier1 = rowChild.Cells["Supplier1"].Value.ToString().Trim().Length;
            long supplier2 = rowChild.Cells["Supplier2"].Value.ToString().Trim().Length;
            long supplier3 = rowChild.Cells["Supplier3"].Value.ToString().Trim().Length;
            long supplier4 = rowChild.Cells["Supplier4"].Value.ToString().Trim().Length;
            if (
                (rowChild.Cells["Leadtime1"].Value.ToString().Trim().Length > 0 &&
                DBConvert.ParseDouble(rowChild.Cells["Leadtime1"].Value.ToString()) <= 0) ||
                (rowChild.Cells["Leadtime2"].Value.ToString().Trim().Length > 0 &&
                DBConvert.ParseDouble(rowChild.Cells["Leadtime2"].Value.ToString()) <= 0) ||
                (rowChild.Cells["Leadtime3"].Value.ToString().Trim().Length > 0 &&
                DBConvert.ParseDouble(rowChild.Cells["Leadtime3"].Value.ToString()) <= 0) ||
                (rowChild.Cells["Leadtime4"].Value.ToString().Trim().Length > 0 &&
                DBConvert.ParseDouble(rowChild.Cells["Leadtime4"].Value.ToString()) <= 0)
              )
            {
              ultraGridInformation.Rows[i].Appearance.BackColor = Color.Yellow;
              errorMessage = "Leadtime";
              return false;
            }
            if (
              (supplier1 == 0 &&
               DBConvert.ParseDouble(rowChild.Cells["Leadtime1"].Value.ToString()) >= 0) ||
              (supplier2 == 0 &&
               DBConvert.ParseDouble(rowChild.Cells["Leadtime2"].Value.ToString()) >= 0) ||
              (supplier3 == 0 &&
               DBConvert.ParseDouble(rowChild.Cells["Leadtime3"].Value.ToString()) >= 0) ||
              (supplier4 == 0 &&
               DBConvert.ParseDouble(rowChild.Cells["Leadtime4"].Value.ToString()) >= 0)
              )
            {
              ultraGridInformation.Rows[i].Appearance.BackColor = Color.Yellow;
              errorMessage = "Leadtime";
              return false;
            }
            if (
              (DBConvert.ParseInt(rowChild.Cells["Supplier1"].Value.ToString()) == DBConvert.ParseInt(rowChild.Cells["Supplier2"].Value.ToString()) && supplier1 > 0 && supplier2 > 0) ||
              (DBConvert.ParseInt(rowChild.Cells["Supplier1"].Value.ToString()) == DBConvert.ParseInt(rowChild.Cells["Supplier3"].Value.ToString()) && supplier1 > 0 && supplier3 > 0) ||
              (DBConvert.ParseInt(rowChild.Cells["Supplier1"].Value.ToString()) == DBConvert.ParseInt(rowChild.Cells["Supplier4"].Value.ToString()) && supplier1 > 0 && supplier4 > 0) ||
              (DBConvert.ParseInt(rowChild.Cells["Supplier2"].Value.ToString()) == DBConvert.ParseInt(rowChild.Cells["Supplier3"].Value.ToString()) && supplier2 > 0 && supplier3 > 0) ||
              (DBConvert.ParseInt(rowChild.Cells["Supplier2"].Value.ToString()) == DBConvert.ParseInt(rowChild.Cells["Supplier4"].Value.ToString()) && supplier2 > 0 && supplier4 > 0) ||
              (DBConvert.ParseInt(rowChild.Cells["Supplier3"].Value.ToString()) == DBConvert.ParseInt(rowChild.Cells["Supplier4"].Value.ToString()) && supplier3 > 0 && supplier4 > 0)
              )
            {
              ultraGridInformation.Rows[i].Appearance.BackColor = Color.Yellow;
              errorMessage = "Supplier";
              return false;
            }
          }
      }
      return true;
    }
    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveData()
    {
      string errorMessage;
      bool flag = true;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          int isUpdate = DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["IsUpdate"].Value);
          if (isUpdate == 1)
          {
            for (int j = 0; j < ultraGridInformation.Rows[i].ChildBands[0].Rows.Count; j++)
            {
              long detailPid = DBConvert.ParseLong(ultraGridInformation.Rows[i].ChildBands[0].Rows[j].Cells["PidDetail"].Value.ToString());
              long supplier1 = DBConvert.ParseLong(ultraGridInformation.Rows[i].ChildBands[0].Rows[j].Cells["Supplier1"].Value.ToString());
              long supplier2 = DBConvert.ParseLong(ultraGridInformation.Rows[i].ChildBands[0].Rows[j].Cells["Supplier2"].Value.ToString());
              long supplier3 = DBConvert.ParseLong(ultraGridInformation.Rows[i].ChildBands[0].Rows[j].Cells["Supplier3"].Value.ToString());
              long supplier4 = DBConvert.ParseLong(ultraGridInformation.Rows[i].ChildBands[0].Rows[j].Cells["Supplier4"].Value.ToString());
              double leadtime1 = DBConvert.ParseDouble(ultraGridInformation.Rows[i].ChildBands[0].Rows[j].Cells["Leadtime1"].Value.ToString());
              double leadtime2 = DBConvert.ParseDouble(ultraGridInformation.Rows[i].ChildBands[0].Rows[j].Cells["Leadtime2"].Value.ToString());
              double leadtime3 = DBConvert.ParseDouble(ultraGridInformation.Rows[i].ChildBands[0].Rows[j].Cells["Leadtime3"].Value.ToString());
              double leadtime4 = DBConvert.ParseDouble(ultraGridInformation.Rows[i].ChildBands[0].Rows[j].Cells["Leadtime4"].Value.ToString());

              DBParameter[] inputParam = new DBParameter[10];

              if (detailPid > 0)
              {
                inputParam[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
              }
              if (supplier1 > 0)
              {
                inputParam[1] = new DBParameter("@Sup1", DbType.Int64, supplier1);
              }
              if (supplier2 > 0)
              {
                inputParam[2] = new DBParameter("@Sup2", DbType.Int64, supplier2);
              }
              if (supplier3 > 0)
              {
                inputParam[3] = new DBParameter("@Sup3", DbType.Int64, supplier3);
              }
              if (supplier4 > 0)
              {
                inputParam[4] = new DBParameter("@Sup4", DbType.Int64, supplier4);
              }
              if (leadtime1 > 0)
              {
                inputParam[5] = new DBParameter("@Leadtime1", DbType.Int64, leadtime1);
              }
              if (leadtime2 > 0)
              {
                inputParam[6] = new DBParameter("@Leadtime2", DbType.Int64, leadtime2);
              }
              if (leadtime3 > 0)
              {
                inputParam[7] = new DBParameter("@Leadtime3", DbType.Int64, leadtime3);
              }
              if (leadtime4 > 0)
              {
                inputParam[8] = new DBParameter("@Leadtime4", DbType.Int64, leadtime4);
              }
              DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
              DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateLeadtimeForSupplier_Edit", inputParam, output);
              long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
              if (resultSave == 0)
              {
                flag = false;
              }
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SearchData();
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
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
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
    /// <summary>
    /// AfterCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (e.Cell.Row.ParentRow == null)
      {
        e.Cell.Row.Cells["IsUpdate"].Value = 1;
      }
      else
      {
        e.Cell.Row.ParentRow.Cells["IsUpdate"].Value = 1;
      }
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align & Status
      for (int j = 0; j < e.Layout.Bands[0].Columns.Count; j++)
      {
        Type colType = e.Layout.Bands[0].Columns[j].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[j].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[j].Format = "#,##0.##";
        }
        e.Layout.Bands[0].Columns[j].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      for (int j = 0; j < e.Layout.Bands[1].Columns.Count; j++)
      {
        Type colType = e.Layout.Bands[1].Columns[j].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[j].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[1].Columns[j].Format = "#,##0.##";
        }
        e.Layout.Bands[1].Columns["PartCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        e.Layout.Bands[1].Columns["PartName"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        e.Layout.Bands[1].Columns["LocationDefault"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["IsDefault"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsDefault"].Header.Caption = "Default";
      e.Layout.Bands[0].Columns["SStatus"].Header.Caption = "Confirm";

      e.Layout.Bands[0].Columns["PidMaster"].Hidden = true;
      e.Layout.Bands[0].Columns["IsUpdate"].Hidden = true;
      e.Layout.Bands[0].Columns["PartGroupPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PidDetail"].Hidden = true;
      e.Layout.Bands[1].Columns["MasterPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PartCodePid"].Hidden = true;

      e.Layout.Bands[1].Columns["Supplier1"].ValueList = ultraDropSupplier;
      e.Layout.Bands[1].Columns["Supplier2"].ValueList = ultraDropSupplier;
      e.Layout.Bands[1].Columns["Supplier3"].ValueList = ultraDropSupplier;
      e.Layout.Bands[1].Columns["Supplier4"].ValueList = ultraDropSupplier;

    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        ControlUtility.GetDataForClipboard(ultraGridInformation);
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion event

  }
}
