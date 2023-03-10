/*
  Author      : 
  Date        : 25/03/2014
  Description : WO Carcass Connect Foudry
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_98_020 : MainUserControl
  {
    #region fields
    #endregion fields

    #region function
    private void LoadData_ucUltraListItem()
    {
      txtItemCode.Text = string.Empty;
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ITEM.ItemCode, ITEM.Name ItemName  ";
      commandText += " FROM   ";
      commandText += "	(";
      commandText += "		SELECT DISTINCT ItemCode";
      commandText += "		FROM VFOUWoConfirmedDetail";
      commandText += "	) WO ";
      commandText += "	INNER JOIN TblBOMItemBasic ITEM ON WO.ItemCode = ITEM.ItemCode  ";
      commandText += " ORDER BY ITEM.ItemCode ";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucUltraListItem.DataSource = dtItem;
      ucUltraListItem.ColumnWidths = "100; 200";
      ucUltraListItem.DataBind();
      ucUltraListItem.ValueMember = "ItemCode";
    }

    private void LoadData_ucUltraListMaterial()
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@ItemCodes", DbType.AnsiString, 4000, txtItemCode.Text);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNGetListFOUComponent", inputParam);
      dtSource.Columns.Remove("Description");
      ucUltraListComponent.DataSource = dtSource;
      ucUltraListComponent.ColumnWidths = "100; 200";
      ucUltraListComponent.DataBind();
      ucUltraListComponent.ValueMember = "CompCode";
      txtComponent.Text = string.Empty;
    }

    private void Search()
    {
      btnSearch.Enabled = false;

      string materialCode = txtComponent.Text.Trim();

      DBParameter[] inputParam = new DBParameter[4];
      string storeName = "spPLNCarcassWOFoundry_Select";

      if (txtItemCode.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@ItemCodes", DbType.AnsiString, 4000, txtItemCode.Text.Trim());
      }

      if (txtComponent.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@Component", DbType.AnsiString, 4000, txtComponent.Text.Trim());
      }

      DateTime shipdate;
      // ShipDate From
      if (this.ultShipDateFrom.Value != null)
      {
        shipdate = DBConvert.ParseDateTime(this.ultShipDateFrom.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        if (shipdate != null && shipdate != DateTime.MinValue)
        {
          inputParam[2] = new DBParameter("@ShipDateFrom", DbType.DateTime, shipdate);
        }
      }

      // ShipDate To
      if (this.ultShipDateTo.Value != null)
      {
        shipdate = DBConvert.ParseDateTime(this.ultShipDateTo.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        if (shipdate != null && shipdate != DateTime.MinValue)
        {
          shipdate = (shipdate != DateTime.MaxValue) ? shipdate.AddDays(1) : shipdate;
          inputParam[3] = new DBParameter("@ShipDateTo", DbType.DateTime, shipdate);
        }
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, 3000, inputParam);
      ultraGridWOMaterialDetail.DataSource = dtSource;
      for (int i = 0; i < this.ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        UltraGridRow row = this.ultraGridWOMaterialDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Color"].Value.ToString()) == 1)
        {
          row.Cells["ProductionRequiredDate"].Appearance.ForeColor = Color.Red;
          row.Cells["ProductionRequiredDate"].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
        }
      }
      btnSearch.Enabled = true;
    }

    #endregion function

    #region event
    public viewPLN_98_020()
    {
      InitializeComponent();
    }

    private void viewPLN_98_020_Load(object sender, EventArgs e)
    {
      this.LoadData_ucUltraListItem();
      this.LoadData_ucUltraListMaterial();
    }

    private void chkShowItemListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListItem.Visible = chkShowItemListBox.Checked;
    }

    private void chkShowMaterialCodeListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListComponent.Visible = chkShowMaterialCodeListBox.Checked;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultraGridWOMaterialDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Priority"].Hidden = true;
      e.Layout.Bands[0].Columns["Color"].Hidden = true;

      e.Layout.Bands[0].Columns["Need"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["Need"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["ProductionRequiredDate"].Header.Caption = "Production\nRequiredDate";

      e.Layout.Bands[0].ColHeaderLines = 2;

      DataTable dtNew = (DataTable)ultraGridWOMaterialDetail.DataSource;
      for (int i = 0; i < dtNew.Columns.Count; i++)
      {
        if (dtNew.Columns[i].DataType == typeof(Int32) || dtNew.Columns[i].DataType == typeof(float) || dtNew.Columns[i].DataType == typeof(Double))
        {
          ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dtNew.Columns[i].DataType == typeof(DateTime))
        {
          ultraGridWOMaterialDetail.DisplayLayout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ucUltraListItem_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtItemCode.Text = ucUltraListItem.SelectedValue;
      this.LoadData_ucUltraListMaterial();
    }

    private void ucUltraListMaterial_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtComponent.Text = ucUltraListComponent.SelectedValue;
    }

    private void ultraCBWO_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (ultraGridWOMaterialDetail.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultraGridWOMaterialDetail, out xlBook, "CARCASS WO-FOUDRY", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Dai Co Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "5/3 - 5/5 Group 62, Area 5, Tan Thoi Nhat Ward, Distr 12, Ho Chi Minh, Viet Nam.";

        xlSheet.Cells[3, 1] = "CARCASS WO-FOUDRY";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.txtItemCode.Text = string.Empty;
      this.txtComponent.Text = string.Empty;
    }
    #endregion event
  }
}
