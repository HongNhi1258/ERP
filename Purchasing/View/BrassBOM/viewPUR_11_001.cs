/*
  Author  : 
  Date    : 25/04/2012
  Description : Min-Max Material Brass BOM
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
using VBReport;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_11_001 : MainUserControl
  {
    #region Init
    public viewPUR_11_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// User Control Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_06_006_Load(object sender, EventArgs e)
    {
      // Load UltraCombo MaterialCode
      this.LoadComboMaterialCode();

      // Load UltraCombo Supplier
      this.LoadComboSupplier();

      // Load UltraCombo Lead Time
      this.LoadComboLeadTimeType();
    }

    /// <summary>
    /// Load UltraCombo Material Code
    /// </summary>
    private void LoadComboMaterialCode()
    {
      string commandText = string.Empty;
      commandText += "  SELECT MAT.MaterialCode ID_SanPham, SP.MaterialCode + ' - ' + SP.NameEN Name";
      commandText += "  FROM VPURMaterialBrassBOM MAT";
      commandText += "  	INNER JOIN TblGNRMaterialInformation SP ON MAT.MaterialCode = SP.MaterialCode";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultMaterialCode.DataSource = dtSource;
      ultMaterialCode.DisplayMember = "Name";
      ultMaterialCode.ValueMember = "ID_SanPham";
      ultMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultMaterialCode.DisplayLayout.Bands[0].Columns["ID_SanPham"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = "SELECT SupplierCode ID_NhaCC, EnglishName AS Name FROM TblPURSupplierInfo";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultSupplier.DataSource = dtSource;
      ultSupplier.DisplayMember = "Name";
      ultSupplier.ValueMember = "ID_NhaCC";
      ultSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSupplier.DisplayLayout.Bands[0].Columns["Name"].Width = 450;
      ultSupplier.DisplayLayout.Bands[0].Columns["ID_NhaCC"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Lead Time
    /// </summary>
    private void LoadComboLeadTimeType()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 1 ID, 'Non-Stock' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'ReOrder' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultLeadTime.DataSource = dtSource;
      ultLeadTime.DisplayMember = "Name";
      ultLeadTime.ValueMember = "ID";
      ultLeadTime.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLeadTime.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultLeadTime.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }
    #endregion Init

    #region LoadData
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {

      DateTime LastMonthDate = DateTime.Now.AddMonths(-1);
      string commandText = string.Empty;
      string column1 = string.Empty;
      DBParameter[] input = new DBParameter[5];
      input[0] = new DBParameter("@Month", DbType.Int32, DateTime.Now.Month);
      input[1] = new DBParameter("@Year", DbType.Int32, DateTime.Now.Year);

      if (ultMaterialCode.Value != null && ultMaterialCode.Value.ToString().Length > 0)
      {
        input[2] = new DBParameter("@MaterialCode", DbType.String, ultMaterialCode.Value.ToString());
      }

      if (ultSupplier.Value != null && ultSupplier.Value.ToString().Length > 0)
      {
        input[3] = new DBParameter("@SupplierCode", DbType.String, ultSupplier.Value.ToString());
      }

      if (ultLeadTime.Value != null && ultLeadTime.Value.ToString().Length > 0)
      {
        input[4] = new DBParameter("@LeadTime", DbType.Int32, DBConvert.ParseInt(ultLeadTime.Value.ToString()));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPURReOrderMaterialBrassBOM_Select", 900, input);

      for (int i = 11; i > 0; i--)
      {
        LastMonthDate = DateTime.Now.AddMonths(-i);
        commandText = string.Empty;
        commandText += " SELECT MaterialCode, SUM([Weight]) QtyOut";
        commandText += " FROM VPURMaterialBrassBOMIssuedMonthlyClosing";
        commandText += " WHERE [Month] = " + LastMonthDate.Month + " AND [Year] = " + LastMonthDate.Year;
        commandText += " GROUP BY MaterialCode";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

        column1 = LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString();

        dtSource.Columns.Add(column1, typeof(System.Double));
        foreach (DataRow row in dtSource.Rows)
        {
          string materialCode = row["ID_SanPham"].ToString();
          DataRow[] foundRows = dt.Select("MaterialCode = '" + materialCode + "'");
          if (foundRows.Length == 0)
          {
            continue;
          }

          row[column1] = DBConvert.ParseDouble(foundRows[0]["QtyOut"].ToString());
        }
      }
      dtSource.Columns.Add("MonthlyConsumption", typeof(System.Double));
      dtSource.Columns.Add("MinOfStock", typeof(System.Double));
      dtSource.Columns.Add("ReOrderQty", typeof(System.Double));
      dtSource.Columns.Add("ReOrderStr", typeof(System.String));
      for (int j = 0; j < dtSource.Rows.Count; j++)
      {
        DataRow row = dtSource.Rows[j];
        if (DBConvert.ParseInt(row["TrungBinhTon"].ToString()) != int.MinValue)
        {
          double sumOut = 0;
          double maxOut = 0;
          double minOut = 0;
          for (int k = 11; k > 11 - DBConvert.ParseInt(row["TrungBinhTon"].ToString()); k--)
          {
            if (DBConvert.ParseDouble(row[k + 17].ToString()) != double.MinValue)
            {
              //Set init Default for MinOut
              if (minOut == 0)
              {
                minOut = DBConvert.ParseDouble(row[k + 17].ToString());
              }

              sumOut += DBConvert.ParseDouble(row[k + 17].ToString());
              if (maxOut < DBConvert.ParseDouble(row[k + 17].ToString()))
              {
                maxOut = DBConvert.ParseDouble(row[k + 17].ToString());
              }

              if (minOut > DBConvert.ParseDouble(row[k + 17].ToString()))
              {
                minOut = DBConvert.ParseDouble(row[k + 17].ToString());
              }
            }
          }

          // Formula Sum
          if (DBConvert.ParseInt(row["Average"].ToString()) == 0)
          {
            row["MonthlyConsumption"] = DBConvert.ParseDouble(sumOut / DBConvert.ParseInt(row["TrungBinhTon"].ToString()));
          }
          // Formula Max
          else if (DBConvert.ParseInt(row["Average"].ToString()) == 1)
          {
            row["MonthlyConsumption"] = maxOut;
          }
          // Formula Min
          else if (DBConvert.ParseInt(row["Average"].ToString()) == 2)
          {
            row["MonthlyConsumption"] = minOut;
          }
          // By Default Min Of Stock 
          else
          {
            row["MonthlyConsumption"] = 0;
          }

          if (DBConvert.ParseInt(row["Average"].ToString()) == -1)
          {
            row["MinOfStock"] = DBConvert.ParseDouble(row["MinDefault"].ToString());
          }
          else
          {
            row["MinOfStock"] = DBConvert.ParseDouble(row["MonthlyConsumption"].ToString())
                                    * DBConvert.ParseDouble(row["LeadTime"].ToString())
                                    * DBConvert.ParseDouble(row["MinimumStock"].ToString());
          }

          row["ReOrderQty"] = ((DBConvert.ParseDouble(row["MinOfStock"].ToString()) * 2)
                        - DBConvert.ParseDouble(row["Ending"].ToString()))
                        * DBConvert.ParseDouble(row["ReOrder"].ToString());
          if (DBConvert.ParseDouble(row["ReOrderQty"].ToString()) < 0)
          {
            row["ReOrderQty"] = 0;
          }

          if (DBConvert.ParseDouble(row["ReOrderQty"].ToString()) > 0 &&
                        DBConvert.ParseDouble(row["ReOrderQty"].ToString()) < 2 * DBConvert.ParseDouble(row["MinOfStock"].ToString()))
          {
            row["ReOrderStr"] = "Order!";
          }
          else
          {
            row["ReOrderStr"] = "";
          }
          row["ReOrderQty"] = Math.Round(DBConvert.ParseDouble(row["ReOrderQty"].ToString()), 2);
          row["MinOfStock"] = Math.Round(DBConvert.ParseDouble(row["MinOfStock"].ToString()), 2);
          row["MonthlyConsumption"] = Math.Round(DBConvert.ParseDouble(row["MonthlyConsumption"].ToString()), 2);
        }
      }
      this.ultDetail.DataSource = dtSource;
      for (int m = 0; m < ultDetail.Rows.Count; m++)
      {
        UltraGridRow row = ultDetail.Rows[m];
        row.Activation = Activation.ActivateOnly;
        if (DBConvert.ParseInt(row.Cells["Average"].Value.ToString()) == -1)
        {
          row.Cells["MinOfStock"].Appearance.BackColor = Color.LightBlue;
        }

        if (DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString()) > 0
              && DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString()) < 2 * DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString())
              && DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString()) > DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString()))
        {
          row.Cells["ReOrderStr"].Appearance.BackColor = Color.Orange;
          row.Cells["ReOrderQty"].Appearance.BackColor = Color.Orange;
        }
        else if (DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString()) > 0
              && DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString()) <= DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString()))
        {
          row.Cells["ReOrderStr"].Appearance.BackColor = Color.Red;
          row.Cells["ReOrderQty"].Appearance.BackColor = Color.Red;
        }
        else
        {
          row.Cells["ReOrderStr"].Appearance.BackColor = Color.White;
          row.Cells["ReOrderQty"].Appearance.BackColor = Color.White;
        }

        if (row.Cells["SoLuong"].Value.ToString().Length > 0)
        {
          row.Cells["SoLuong"].Appearance.BackColor = Color.LightBlue;
        }
      }
    }
    #endregion LoadData

    #region Event
    /// <summary>
    /// Search Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.btnSearch.Enabled = false;
      // Search Information
      this.Search();
      this.btnSearch.Enabled = true;
    }

    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Columns["ID_SanPham"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TenEnglish"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TenDonViEN"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["GiaTien"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Supplier"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["LeadTime"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Ending"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["MonthlyConsumption"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["MinOfStock"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ReOrderQty"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ReOrderStr"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SoLuong"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["PurchaseUnit"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["ReOrder"].Hidden = true;
      e.Layout.Bands[0].Columns["MinimumStock"].Hidden = true;
      e.Layout.Bands[0].Columns["TrungBinhTon"].Hidden = true;
      e.Layout.Bands[0].Columns["MinDefault"].Hidden = true;
      e.Layout.Bands[0].Columns["Average"].Hidden = true;
      e.Layout.Bands[0].Columns["TenVietNam"].Hidden = true;

      e.Layout.Bands[0].Columns["ID_SanPham"].Header.Caption = "Mat.Code";
      e.Layout.Bands[0].Columns["TenEnglish"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["TenVietNam"].Header.Caption = "Name VN";
      e.Layout.Bands[0].Columns["TenDonViEN"].Header.Caption = "Unit";
      e.Layout.Bands[0].Columns["GiaTien"].Header.Caption = "Unit Price($)";
      e.Layout.Bands[0].Columns["SoLuong"].Header.Caption = "Stock PR";
      e.Layout.Bands[0].Columns["ReOrderStr"].Header.Caption = "Alert";
      e.Layout.Bands[0].Columns["ReOrderQty"].Header.Caption = "Re-Order";
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        for (int j = 18; j < 29; j++)
        {
          row.Cells[j].Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Columns["Ending"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MonthlyConsumption"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ReOrderQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["SoLuong"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["MinOfStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["GiaTien"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ReOrder"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["Beginning"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["InStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["OutStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LeadTime"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["ID_SanPham"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ID_SanPham"].MinValue = 80;

      e.Layout.Bands[0].Columns["TenDonViEN"].MaxWidth = 30;
      e.Layout.Bands[0].Columns["TenDonViEN"].MinValue = 30;

      e.Layout.Bands[0].Columns["ReOrderStr"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["ReOrderStr"].MinValue = 40;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void bnExport_Click(object sender, EventArgs e)
    {
      if (this.ultDetail.Rows.Count == 0)
      {
        return;
      }

      string strTemplateName = "RPT_PUR_10_005_01";
      string strSheetName = "Sheet1";
      string strOutFileName = "Excel Veneer Template";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataTable dtSource = (DataTable)ultDetail.DataSource;

      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        oXlsReport.Cell("**Month1").Value = dtSource.Columns[18].ColumnName;
        oXlsReport.Cell("**Month2").Value = dtSource.Columns[19].ColumnName;
        oXlsReport.Cell("**Month3").Value = dtSource.Columns[20].ColumnName;
        oXlsReport.Cell("**Month4").Value = dtSource.Columns[21].ColumnName;
        oXlsReport.Cell("**Month5").Value = dtSource.Columns[22].ColumnName;
        oXlsReport.Cell("**Month6").Value = dtSource.Columns[23].ColumnName;
        oXlsReport.Cell("**Month7").Value = dtSource.Columns[24].ColumnName;
        oXlsReport.Cell("**Month8").Value = dtSource.Columns[25].ColumnName;
        oXlsReport.Cell("**Month9").Value = dtSource.Columns[26].ColumnName;
        oXlsReport.Cell("**Month10").Value = dtSource.Columns[27].ColumnName;
        oXlsReport.Cell("**Month11").Value = dtSource.Columns[28].ColumnName;

        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultDetail.Rows[i];

          if (i > 0)
          {
            oXlsReport.Cell("B3:AB3").Copy();
            oXlsReport.RowInsert(2 + i);
            oXlsReport.Cell("B3:AB3", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**MatCode", 0, i).Value = row.Cells["ID_SanPham"].Value.ToString();
          oXlsReport.Cell("**NameEn", 0, i).Value = row.Cells["TenEnglish"].Value.ToString();
          oXlsReport.Cell("**Price", 0, i).Value = row.Cells["GiaTien"].Value.ToString();
          oXlsReport.Cell("**Supplier", 0, i).Value = row.Cells["Supplier"].Value.ToString();
          oXlsReport.Cell("**Unit", 0, i).Value = row.Cells["TenDonViEN"].Value.ToString();

          if (DBConvert.ParseDouble(row.Cells["LeadTime"].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**LeadTime", 0, i).Value = DBConvert.ParseDouble(row.Cells["LeadTime"].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**LeadTime", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells["Ending"].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Ending", 0, i).Value = DBConvert.ParseDouble(row.Cells["Ending"].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**Ending", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells["SoLuong"].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = DBConvert.ParseDouble(row.Cells["SoLuong"].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**StockPR", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells["MonthlyConsumption"].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**MonthlyConsumption", 0, i).Value = DBConvert.ParseDouble(row.Cells["MonthlyConsumption"].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**MonthlyConsumption", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Min", 0, i).Value = DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**Min", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**ReOrderQty", 0, i).Value = DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**ReOrderQty", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells["Beginning"].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**Beginning", 0, i).Value = DBConvert.ParseDouble(row.Cells["Beginning"].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**Beginning", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells["InStock"].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**InStock", 0, i).Value = DBConvert.ParseDouble(row.Cells["InStock"].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**InStock", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells["OutStock"].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**OutStock", 0, i).Value = DBConvert.ParseDouble(row.Cells["OutStock"].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**OutStock", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[18].Value.ToString()) != double.MinValue)
          {

            oXlsReport.Cell("**1", 0, i).Value = DBConvert.ParseDouble(row.Cells[18].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**1", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[19].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**2", 0, i).Value = DBConvert.ParseDouble(row.Cells[19].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**2", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[20].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**3", 0, i).Value = DBConvert.ParseDouble(row.Cells[20].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**3", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[21].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**4", 0, i).Value = DBConvert.ParseDouble(row.Cells[21].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**4", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[22].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**5", 0, i).Value = DBConvert.ParseDouble(row.Cells[22].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**5", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[23].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**6", 0, i).Value = DBConvert.ParseDouble(row.Cells[23].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**6", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[24].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**7", 0, i).Value = DBConvert.ParseDouble(row.Cells[24].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**7", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[25].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**8", 0, i).Value = DBConvert.ParseDouble(row.Cells[25].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**8", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[26].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**9", 0, i).Value = DBConvert.ParseDouble(row.Cells[26].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**9", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[27].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**10", 0, i).Value = DBConvert.ParseDouble(row.Cells[27].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**10", 0, i).Value = "";
          }

          if (DBConvert.ParseDouble(row.Cells[28].Value.ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**11", 0, i).Value = DBConvert.ParseDouble(row.Cells[28].Value.ToString());
          }
          else
          {
            oXlsReport.Cell("**11", 0, i).Value = "";
          }
        }
      }
      else
      {
        return;
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }
    #endregion Event
  }
}
