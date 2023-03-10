/*
  Author      : 
  Date        : 25/03/2014
  Description : WO Carcass Connect Material
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

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_021 : MainUserControl
  {
    #region fields
    private int materialControlType = 1;
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion fields

    #region function
    private void LoadData_ucUltraListItem()
    {
      txtItemCode.Text = string.Empty;
      string commandText = "Select Distinct ITEM.ItemCode, ITEM.Name ItemName From TblPLNWorkOrderConfirmedDetails WO INNER JOIN TblBOMItemBasic ITEM On WO.ItemCode = ITEM.ItemCode Order By ITEM.ItemCode";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucUltraListItem.DataSource = dtItem;
      ucUltraListItem.ColumnWidths = "100; 200";
      ucUltraListItem.DataBind();
      ucUltraListItem.ValueMember = "ItemCode";
    }

    private void LoadData_ucUltraListMaterial()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, txtMaterialGroup.Text);
      inputParam[1] = new DBParameter("@ControlType", DbType.Int32, materialControlType);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNGetListControlMaterial", inputParam);
      dtSource.Columns.Remove("Description");
      ucUltraListMaterial.DataSource = dtSource;
      ucUltraListMaterial.ColumnWidths = "100; 200";
      ucUltraListMaterial.DataBind();
      ucUltraListMaterial.ValueMember = "MaterialCode";
      txtMaterialCode.Text = string.Empty;
    }

    private void Search()
    {
      btnSearch.Enabled = false;

      string materialCode = txtMaterialCode.Text.Trim();

      DBParameter[] inputParam = new DBParameter[5];
      string storeName = "spPLNCarcassWoMaterial_Select";

      if (txtItemCode.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@ItemCodes", DbType.AnsiString, 4000, txtItemCode.Text.Trim());
      }
      if (txtMaterialGroup.Text.Trim().Length > 0)
      {
        inputParam[1] = new DBParameter("@GroupMaterials", DbType.AnsiString, 4000, txtMaterialGroup.Text.Trim());
      }
      if (txtMaterialCode.Text.Trim().Length > 0)
      {
        inputParam[2] = new DBParameter("@Materials", DbType.AnsiString, 4000, materialCode);
      }

      DateTime shipdate;
      // ShipDate From
      if (this.ultDTShipDateFrom.Value != null)
      {
        shipdate = DBConvert.ParseDateTime(this.ultDTShipDateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        if (shipdate != null && shipdate != DateTime.MinValue)
        {
          inputParam[3] = new DBParameter("@ShipDateFrom", DbType.DateTime, shipdate);
        }
      }

      // ShipDate To
      if (this.ultDTShipDateTo.Value != null)
      {
        shipdate = DBConvert.ParseDateTime(this.ultDTShipDateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        if (shipdate != null && shipdate != DateTime.MinValue)
        {
          shipdate = (shipdate != DateTime.MaxValue) ? shipdate.AddDays(1) : shipdate;
          inputParam[4] = new DBParameter("@ShipDateTo", DbType.DateTime, shipdate);
        }
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, 3000, inputParam);
      ultraGridWOMaterialDetail.DataSource = dtSource;

      for (int i = 0; i < this.ultraGridWOMaterialDetail.Rows.Count; i++)
      {
        UltraGridRow row = this.ultraGridWOMaterialDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["PRLate"].Value.ToString()) == 1)
        {
          row.Cells["PRDeliveryDate"].Appearance.ForeColor = Color.Blue;
          row.Cells["PRDeliveryDate"].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
          row.Cells["ProductionRequiredDate"].Appearance.ForeColor = Color.Red;
          row.Cells["ProductionRequiredDate"].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
        }

        if (DBConvert.ParseInt(row.Cells["IssuedLate"].Value.ToString()) == 1)
        {
          row.Cells["ProductionRequiredDate"].Appearance.ForeColor = Color.Red;
          row.Cells["ProductionRequiredDate"].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
        }
        if (DBConvert.ParseInt(row.Cells["Remain"].Value.ToString()) != 0)
        {
          row.Cells["Remain"].Appearance.ForeColor = Color.Red;
          row.Cells["Remain"].Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
        }
      }
      btnSearch.Enabled = true;
    }

    #endregion function

    #region event
    public viewPLN_98_021()
    {
      InitializeComponent();
      ultDTShipDateFrom.Value = DBNull.Value;
      ultDTShipDateTo.Value = DBNull.Value;
    }

    private void viewPLN_98_021_Load(object sender, EventArgs e)
    {
      this.LoadData_ucUltraListItem();

      ControlUtility.LoaducUltraListMaterialGroup(ucUltraListMaterialGroup);
    }

    private void chkShowItemListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListItem.Visible = chkShowItemListBox.Checked;
    }

    private void chkShowMaterialListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListMaterialGroup.Visible = chkShowMaterialListBox.Checked;
    }

    private void chkShowMaterialCodeListBox_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListMaterial.Visible = chkShowMaterialCodeListBox.Checked;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (txtMaterialGroup.Text.Length == 0)
      {
        WindowUtinity.ShowMessageWarningFromText("Please input data for searching!");
        return;
      }
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
      e.Layout.Bands[0].Columns["NeedMinusIssued"].Hidden = true;
      e.Layout.Bands[0].Columns["PRLate"].Hidden = true;
      e.Layout.Bands[0].Columns["IssuedLate"].Hidden = true;

      e.Layout.Bands[0].Columns["Need"].CellAppearance.ForeColor = Color.Blue;
      e.Layout.Bands[0].Columns["Need"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["PRDeliveryDate"].Header.Caption = "PR Required\nDelivery Date";
      e.Layout.Bands[0].Columns["Order"].CellAppearance.ForeColor = Color.Red;
      e.Layout.Bands[0].Columns["Order"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["ProductionRequiredDate"].Header.Caption = "Production\nRequiredDate";
      e.Layout.Bands[0].Columns["Issued"].Header.Caption = "Remain\nIssued";

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
    }

    private void ucUltraListMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialGroup.Text = ucUltraListMaterialGroup.SelectedValue;
      this.LoadData_ucUltraListMaterial();
    }

    private void ucUltraListMaterial_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialCode.Text = ucUltraListMaterial.SelectedValue;
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (ultraGridWOMaterialDetail.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        Utility.ExportToExcelWithDefaultPath(ultraGridWOMaterialDetail, out xlBook, "CARCASS WORK ORDER MATERIAL", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);
        Excel.Range r = xlSheet.get_Range("A1", "A1");


        xlSheet.Cells[3, 1] = "CARCASS WORK ORDER MATERIAL";
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
      this.txtMaterialGroup.Text = string.Empty;
      this.txtMaterialCode.Text = string.Empty;
    }
    #endregion event
  }
}
