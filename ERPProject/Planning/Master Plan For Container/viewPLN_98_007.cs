/*
  Author      : 
  Date        : 07/11/2012
  Description : Move Up Overview Container
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_007 : MainUserControl
  {
    #region Field
    public long containerPid = 0;
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init
    public viewPLN_98_007()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_002_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void LoadData()
    {
      DBParameter[] param = new DBParameter[1];

      param[0] = new DBParameter("@ContainerPid", DbType.Int64, containerPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNMasterPlanContainerOverviewMoveUpData_Select", param);

      DataTable dtColor = new DataTable();
      dtColor.Columns.Add("Color", typeof(System.Int32));

      this.ultData.DataSource = dsSource.Tables[0];
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultData.Rows[i];
        int customerPid = DBConvert.ParseInt(rowGrid.Cells["CustomerPid"].Value.ToString());
        DateTime finishDate = DBConvert.ParseDateTime(rowGrid.Cells["FinishDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        string containerNo = rowGrid.Cells["ContainerNo"].Value.ToString();
        if (finishDate != DateTime.MinValue)
        {
          UltraDropDown ultMoveToCustomer = (UltraDropDown)rowGrid.Cells["MoveToCustomer"].ValueList;
          string select = string.Empty;
          select = "CustomerPid =" + customerPid
                  + " AND ShipDate >= '" + finishDate
                  + "' AND ContainerNo <> '" + containerNo + "'";

          DataRow[] foundRow = dsSource.Tables[1].Select(select);
          DataTable dtContainer = this.UltraTable();
          for (int j = 0; j < foundRow.Length; j++)
          {
            DataRow rowAdd = dtContainer.NewRow();
            rowAdd["LoadingListPid"] = DBConvert.ParseLong(foundRow[j]["LoadingListPid"].ToString());
            rowAdd["CustomerCode"] = foundRow[j]["CustomerCode"].ToString();
            rowAdd["ContainerNo"] = foundRow[j]["ContainerNo"].ToString();
            rowAdd["ShipDateStr"] = foundRow[j]["ShipDateStr"].ToString();
            rowAdd["ShipDate"] = foundRow[j]["ShipDate"].ToString();

            dtContainer.Rows.Add(rowAdd);
          }

          DataView dtView = dtContainer.DefaultView;
          dtView.Sort = "ShipDate";
          dtContainer = dtView.ToTable();
          if (ultMoveToCustomer == null)
          {
            if (dtContainer.Rows.Count > 0)
            {
              DataRow row = dtColor.NewRow();
              row["Color"] = i;
              dtColor.Rows.Add(row);
            }

            ultMoveToCustomer = new UltraDropDown();
            this.Controls.Add(ultMoveToCustomer);
          }

          ultMoveToCustomer.DataSource = dtContainer;
          ultMoveToCustomer.ValueMember = "LoadingListPid";
          ultMoveToCustomer.DisplayMember = "ContainerNo";
          ultMoveToCustomer.DisplayLayout.Bands[0].Columns["LoadingListPid"].Hidden = true;
          ultMoveToCustomer.DisplayLayout.Bands[0].Columns["ShipDate"].Hidden = true;
          ultMoveToCustomer.DisplayLayout.Bands[0].Columns["ShipDateStr"].Header.Caption = "Ship Date";

          ultMoveToCustomer.Visible = false;

          rowGrid.Cells["MoveToCustomer"].ValueList = ultMoveToCustomer;
        }
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Cells["No"].Appearance.FontData.Bold = DefaultableBoolean.True;
        ultData.Rows[i].Cells["No"].Appearance.ForeColor = Color.Red;
        DataRow[] foundRowColor = dtColor.Select("Color =" + i);
        if (foundRowColor.Length > 0)
        {
          ultData.Rows[i].Cells["MoveToCustomer"].Appearance.BackColor = Color.LightBlue;
        }
        else
        {
          ultData.Rows[i].Cells["MoveToCustomer"].Appearance.BackColor = Color.White;
        }
      }
      this.LoadColumnName();
    }

    /// <summary>
    /// Get Main Structure Datatable Row Container
    /// </summary>
    /// <returns></returns>
    private DataTable UltraTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("LoadingListPid", typeof(System.Int64));
      dt.Columns.Add("CustomerCode", typeof(System.String));
      dt.Columns.Add("ContainerNo", typeof(System.String));
      dt.Columns.Add("ShipDateStr", typeof(System.String));
      dt.Columns.Add("ShipDate", typeof(System.DateTime));

      return dt;
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["No"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["OldCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["ContainerListDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["FinishDate"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerName"].Hidden = true;
      e.Layout.Bands[0].Columns["DirectName"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerPONo"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["RevConNo"].Hidden = true;

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LoadingCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RevConNo"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["FinishDateStr"].Header.Caption = "FinishDate";

      e.Layout.Bands[0].Columns["No"].MinWidth = 20;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 20;

      e.Layout.Bands[0].Columns["Qty"].MinWidth = 30;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 30;

      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }


    /// <summary>
    /// Load Column Name
    /// </summary>
    private void LoadColumnName()
    {
      DataTable dtNew = new DataTable();
      DataTable dtColumn = (DataTable)ultData.DataSource;
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        dtNew.Columns[column.ColumnName].DefaultValue = 0;

        if (string.Compare(column.ColumnName, "Qty", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "UCBM", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LoadingCBM", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "WO", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "StatusWIP", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "ContainerNo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LoadingDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "FinishDateStr", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "MoveToCustomer", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      ultShowColumn.DataSource = dtNew;
      ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }
    #endregion Init

    #region Load Data
    #endregion Load Data

    #region Validation
    #endregion Validation

    #region Event
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = this.ultData.Rows[i];
        if (DBConvert.ParseInt(rowGrid.Cells["MoveToCustomer"].Value.ToString()) != int.MinValue)
        {
          long containerListDetailPid = DBConvert.ParseLong(rowGrid.Cells["ContainerListDetailPid"].Value.ToString());
          long containerListPid = DBConvert.ParseLong(rowGrid.Cells["MoveToCustomer"].Value.ToString());
          int qty = DBConvert.ParseInt(rowGrid.Cells["Qty"].Value.ToString());

          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@ContainerListDetailPid", DbType.Int64, containerListDetailPid);
          inputParam[1] = new DBParameter("@Qty", DbType.Int32, qty);
          inputParam[2] = new DBParameter("@ContainerListPid", DbType.Int64, containerListPid);

          DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerMainDataMoveToContainerList_Insert", inputParam);
        }
      }
      DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerPriorityRefeshData_Insert");

      WindowUtinity.ShowMessageSuccess("MSG0004");

      // Load Data
      this.LoadData();
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultShowColumns_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
        }
      }
      else
      {
        for (int i = 1; i < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          row.Cells[i].Value = e.Cell.Text;
        }
        this.ChkAll_CheckedChange();
      }
    }

    private void ultShowColumn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      DataTable dtColumn = (DataTable)ultShowColumn.DataSource;
      int count = dtColumn.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      e.Layout.Bands[0].Columns["No"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerListDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleCode"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["OldCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["FinishDate"].Hidden = true;
      e.Layout.Bands[0].Columns["FinishDateStr"].Header.Caption = "FinishDate";
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }
    #endregion Event
  }
}
