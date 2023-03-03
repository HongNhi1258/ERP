/*
  Author      : 
  Date        : 03/01/2013
  Description : Min-Max Woods(Module)
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

namespace DaiCo.Purchasing
{
  public partial class viewPUR_06_005 : MainUserControl
  {
    #region Init
    public viewPUR_06_005()
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
      // Load UltraCombo Supplier
      this.LoadComboSupplier();

      // Load UltraCombo Lead Time
      this.LoadComboLeadTimeType();

      // Load UltraCombo Warehouse
      this.LoadComboWarehouse();

      // Load UltraCombo Note
      this.LoadComboNoteType();

      // Load Purchaser
      this.LoadComboPurchaser();
    }

    /// <summary>
    /// Load UltraCombo Purchaser
    /// </summary>
    private void LoadComboPurchaser()
    {
      string commandText = "SELECT Pid, GroupName FROM TblPURStaffGroup WHERE DeleteFlg = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);

      ultPurchaser.DataSource = dtSource;
      ultPurchaser.DisplayMember = "GroupName";
      ultPurchaser.ValueMember = "Pid";
      ultPurchaser.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultPurchaser.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultPurchaser.DisplayLayout.Bands[0].Columns["GroupName"].Width = 300;

      // Load Group In Charge(User Login)
      string commmadText = string.Format(@" SELECT SG.Pid
                                                FROM TblPURStaffGroup SG
	                                                LEFT JOIN TblPURStaffGroupDetail SGD ON SG.Pid = SGD.[Group]
                                                WHERE SG.LeaderGroup = {0} OR SGD.Employee = {1}", SharedObject.UserInfo.UserPid, SharedObject.UserInfo.UserPid);
      DataTable dtGroupInCharge = DataBaseAccess.SearchCommandTextDataTable(commmadText);
      if (dtGroupInCharge != null && dtGroupInCharge.Rows.Count > 0)
      {
        ultPurchaser.Value = DBConvert.ParseInt(dtGroupInCharge.Rows[0]["Pid"].ToString());
      }
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = "SELECT Pid ID_NhaCC, EnglishName + '(' +  SupplierCode + ')' Name FROM TblPURSupplierInfo WHERE Confirm <> 3";
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

      this.ultLeadTime.Value = 2;
    }

    /// <summary>
    /// Load UltraCombo Note
    /// </summary>
    private void LoadComboNoteType()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'Show All' Name";
      commandText += " UNION ";
      commandText += " SELECT 1 ID, 'Only Show Material Have Data(Begin, In, Out, Ending)' Name";
      commandText += " UNION ";
      commandText += " SELECT 2 ID, 'Stock Materials' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultNote.DataSource = dtSource;
      ultNote.DisplayMember = "Name";
      ultNote.ValueMember = "ID";
      ultNote.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultNote.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultNote.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;

      this.ultNote.Value = DBNull.Value;
    }

    /// <summary>
    /// Load UltraCombo Warehouse
    /// </summary>
    private void LoadComboWarehouse()
    {
      string commandText = "SELECT Pid, Name FROM TblWHDWarehouse";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultWarehouse.DataSource = dtSource;
      ultWarehouse.DisplayMember = "Name";
      ultWarehouse.ValueMember = "Pid";
      ultWarehouse.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultWarehouse.DisplayLayout.Bands[0].Columns["Name"].Width = 450;
      ultWarehouse.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;

      this.ultWarehouse.Value = 3;
    }
    #endregion Init

    #region LoadData

    private DataTable CreateTableForMainSearch()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("Select", typeof(System.Int32));
      taParent.Columns.Add("ID_SanPham", typeof(System.String));
      taParent.Columns.Add("TenEnglish", typeof(System.String));
      taParent.Columns.Add("TenVietNam", typeof(System.String));
      taParent.Columns.Add("TenDonViEN", typeof(System.String));
      taParent.Columns.Add("GiaTien", typeof(System.Double));
      taParent.Columns.Add("Currency", typeof(System.String));
      taParent.Columns.Add("Supplier", typeof(System.String));
      taParent.Columns.Add("LeadTime", typeof(System.String));
      taParent.Columns.Add("Beginning", typeof(System.Double));
      taParent.Columns.Add("InStock", typeof(System.Double));
      taParent.Columns.Add("OutStock", typeof(System.Double));
      taParent.Columns.Add("Ending", typeof(System.Double));
      taParent.Columns.Add("ReOrder", typeof(System.String));
      taParent.Columns.Add("MinimumStock", typeof(System.String));
      taParent.Columns.Add("TrungBinhTon", typeof(System.Int32));
      taParent.Columns.Add("MinDefault", typeof(System.Double));
      taParent.Columns.Add("Average", typeof(System.Int32));
      taParent.Columns.Add("SoLuong", typeof(System.Double));
      taParent.Columns.Add("MinOfStockOption", typeof(System.Double));
      taParent.Columns.Add("Purchaser", typeof(System.String));
      taParent.Columns.Add("RefCode", typeof(System.String));

      for (int i = 11; i > 0; i--)
      {
        DateTime LastMonthDate = DateTime.Now.AddMonths(-i);

        string column1 = "REC " + LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString();
        string column2 = "ISS " + LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString();
        taParent.Columns.Add(column1, typeof(System.String));
        taParent.Columns.Add(column2, typeof(System.String));
      }
      return taParent;
    }
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      // Warehouse
      int warehouse = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultWarehouse));

      if (warehouse == int.MinValue)
      {
        string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Warehouse");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }

      DateTime LastMonthDate = DateTime.Now.AddMonths(-1);
      string commandText = string.Empty;
      string column1 = string.Empty;
      string column2 = string.Empty;
      DBParameter[] input = new DBParameter[10];
      input[0] = new DBParameter("@PreMonth", DbType.Int32, LastMonthDate.Month);
      input[1] = new DBParameter("@PreYear", DbType.Int32, LastMonthDate.Year);
      input[2] = new DBParameter("@Month", DbType.Int32, DateTime.Now.Month);
      input[3] = new DBParameter("@Year", DbType.Int32, DateTime.Now.Year);
      input[4] = new DBParameter("@Warehouse", DbType.Int32, DBConvert.ParseInt(ultWarehouse.Value.ToString()));
      if (txtMaterial.Text.Length > 0)
      {
        input[5] = new DBParameter("@MaterialCode", DbType.String, txtMaterial.Text);
      }

      if (ultSupplier.Value != null && ultSupplier.Value.ToString().Length > 0)
      {
        input[6] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(ultSupplier.Value.ToString()));
      }

      if (ultLeadTime.Value != null && ultLeadTime.Value.ToString().Length > 0)
      {
        input[7] = new DBParameter("@LeadTime", DbType.Int32, DBConvert.ParseInt(ultLeadTime.Value.ToString()));
      }

      if (ultNote.Value != null && ultNote.Value.ToString().Length > 0)
      {
        input[8] = new DBParameter("@Note", DbType.Int32, DBConvert.ParseInt(ultNote.Value.ToString()));
      }

      if (ultPurchaser.Value != null && ultPurchaser.Value.ToString().Length > 0)
      {
        input[9] = new DBParameter("@GroupPurchaser", DbType.Int32, DBConvert.ParseInt(ultPurchaser.Value.ToString()));
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPURReOrderWoods_Select", 3000, input);

      int dem = 22;

      for (int i = 11; i > 0; i--)
      {
        LastMonthDate = DateTime.Now.AddMonths(-i);

        column1 = "REC " + LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString();
        column2 = "ISS " + LastMonthDate.Month.ToString() + '/' + LastMonthDate.Year.ToString();

        dtSource.Columns[dem].ColumnName = column1;
        dem++;
        dtSource.Columns[dem].ColumnName = column2;
        dem++;
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
          int loop = 0;
          for (int k = 22; DBConvert.ParseInt(row["TrungBinhTon"].ToString()) > loop; k = k - 2)
          {
            if (DBConvert.ParseDouble(row[k + 21].ToString()) != double.MinValue)
            {
              double value = 0;
              if (DBConvert.ParseDouble(row[k + 21].ToString()) == double.MinValue)
              {
                value = 0;
              }
              else
              {
                value = DBConvert.ParseDouble(row[k + 21].ToString());
              }

              if (k == 22)
              {
                minOut = value;
              }

              //Set init Default for MinOut
              if (value < minOut)
              {
                minOut = value;
              }

              sumOut += value;
              if (maxOut < value)
              {
                maxOut = value;
              }

              if (minOut > value)
              {
                minOut = value;
              }
            }
            loop++;
          }

          // Formula Sum
          if (DBConvert.ParseInt(row["Average"].ToString()) == 1)
          {
            row["MonthlyConsumption"] = DBConvert.ParseDouble(sumOut / DBConvert.ParseInt(row["TrungBinhTon"].ToString()));
          }
          // Formula Max
          else if (DBConvert.ParseInt(row["Average"].ToString()) == 2)
          {
            row["MonthlyConsumption"] = maxOut;
          }
          // Formula Min
          else if (DBConvert.ParseInt(row["Average"].ToString()) == 3)
          {
            row["MonthlyConsumption"] = minOut;
          }
          // By Default Min Of Stock 
          else
          {
            row["MonthlyConsumption"] = 0;
          }

          if (DBConvert.ParseInt(row["MinOfStockOption"].ToString()) == 1)
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

          if (DBConvert.ParseDouble(row["ReOrderQty"].ToString()) > 0)
          {
            row["ReOrderStr"] = "Order!";
          }
          else
          {
            row["ReOrderStr"] = "";
          }
          row["ReOrderQty"] = Math.Round(DBConvert.ParseDouble(row["ReOrderQty"].ToString()), 3);
          row["MinOfStock"] = Math.Round(DBConvert.ParseDouble(row["MinOfStock"].ToString()), 3);
          row["MonthlyConsumption"] = Math.Round(DBConvert.ParseDouble(row["MonthlyConsumption"].ToString()), 3);
        }
      }
      this.ultDetail.DataSource = dtSource;
      for (int m = 0; m < ultDetail.Rows.Count; m++)
      {
        UltraGridRow row = ultDetail.Rows[m];
        if (DBConvert.ParseInt(row.Cells["MinOfStockOption"].Value.ToString()) == 1)
        {
          row.Cells["MinOfStock"].Appearance.BackColor = Color.LightBlue;
        }

        if (DBConvert.ParseDouble(row.Cells["Ending"].Value.ToString()) >= 0
              && DBConvert.ParseDouble(row.Cells["Ending"].Value.ToString()) <= 2 * DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString())
              && DBConvert.ParseDouble(row.Cells["Ending"].Value.ToString()) >= DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString()))
        {
          row.Cells["ReOrderStr"].Appearance.BackColor = Color.Orange;
          row.Cells["ReOrderQty"].Appearance.BackColor = Color.Orange;
        }
        else if (DBConvert.ParseDouble(row.Cells["Ending"].Value.ToString()) >= 0
              && DBConvert.ParseDouble(row.Cells["Ending"].Value.ToString()) <= DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString()))
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
          row.Cells["SoLuong"].Appearance.BackColor = Color.GreenYellow;
        }
      }
      this.LoadColumnName();
    }

    /// <summary>
    /// Truong
    /// </summary>
    private void LoadColumnName()
    {
      DataTable dtNew = new DataTable();
      DataTable dtColumn = (DataTable)ultDetail.DataSource;
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        dtNew.Columns[column.ColumnName].DefaultValue = 0;

        if (string.Compare(column.ColumnName, "Ending", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "MonthlyConsumption", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "SoLuong", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "ReOrderStr", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "ReOrderQty", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "MinOfStock", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      ultShowColumns.DataSource = dtNew;
    }

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange()
    {
      for (int colIndex = 1; colIndex < ultShowColumns.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumns.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultDetail.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
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
      this.Search();
      this.btnSearch.Enabled = true;
    }

    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Select"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ID_SanPham"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TenEnglish"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TenDonViEN"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["ReOrder"].Hidden = true;
      e.Layout.Bands[0].Columns["MinimumStock"].Hidden = true;
      e.Layout.Bands[0].Columns["TrungBinhTon"].Hidden = true;
      e.Layout.Bands[0].Columns["MinDefault"].Hidden = true;
      e.Layout.Bands[0].Columns["Average"].Hidden = true;
      e.Layout.Bands[0].Columns["TenVietNam"].Hidden = true;
      e.Layout.Bands[0].Columns["MinOfStockOption"].Hidden = true;

      e.Layout.Bands[0].Columns["ID_SanPham"].Header.Caption = "Mat.Code";
      e.Layout.Bands[0].Columns["TenEnglish"].Header.Caption = "Name EN";
      e.Layout.Bands[0].Columns["TenVietNam"].Header.Caption = "Name VN";
      e.Layout.Bands[0].Columns["TenDonViEN"].Header.Caption = "Unit";
      e.Layout.Bands[0].Columns["GiaTien"].Header.Caption = "Unit Price";
      e.Layout.Bands[0].Columns["GiaTien"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["SoLuong"].Header.Caption = "Stock PR";
      e.Layout.Bands[0].Columns["ReOrderStr"].Header.Caption = "Alert";
      e.Layout.Bands[0].Columns["ReOrderQty"].Header.Caption = "Re-Order";

      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        for (int j = 22; j < 44; j++)
        {
          row.Cells[j].Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      for (int i = 5; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].Hidden = true;
      }

      e.Layout.Bands[0].Columns["Ending"].Hidden = false;
      e.Layout.Bands[0].Columns["MonthlyConsumption"].Hidden = false;
      e.Layout.Bands[0].Columns["SoLuong"].Hidden = false;
      e.Layout.Bands[0].Columns["MinOfStock"].Hidden = false;
      e.Layout.Bands[0].Columns["ReOrderStr"].Hidden = false;
      e.Layout.Bands[0].Columns["ReOrderQty"].Hidden = false;

      for (int i = 1; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        if (i == 22 || i == 24 || i == 26 || i == 28 || i == 30 || i == 32 || i == 34
              || i == 36 || i == 38 || i == 40 || i == 42)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightBlue;
        }

        if (i == 23 || i == 25 || i == 27 || i == 29 || i == 31 || i == 33 || i == 35
              || i == 37 || i == 39 || i == 41 || i == 43)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGreen;
        }

        if (i > 0 && i < 5)
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.Yellow;
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

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void bnExport_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultDetail, "Data");
      //  if (this.ultDetail.Rows.Count == 0)
      //  {
      //    return;
      //  }

      //  string strTemplateName = "RPT_PUR_10_005_02";
      //  string strSheetName = "Sheet1";
      //  string strOutFileName = "Excel Material Template";
      //  string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //  string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //  string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      //  XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //  DataTable dtSource = (DataTable)ultDetail.DataSource;

      //  if (dtSource != null && dtSource.Rows.Count > 0)
      //  {
      //    oXlsReport.Cell("**Month1").Value = dtSource.Columns[22].ColumnName;
      //    oXlsReport.Cell("**Month2").Value = dtSource.Columns[23].ColumnName;
      //    oXlsReport.Cell("**Month3").Value = dtSource.Columns[24].ColumnName;
      //    oXlsReport.Cell("**Month4").Value = dtSource.Columns[25].ColumnName;
      //    oXlsReport.Cell("**Month5").Value = dtSource.Columns[26].ColumnName;
      //    oXlsReport.Cell("**Month6").Value = dtSource.Columns[27].ColumnName;
      //    oXlsReport.Cell("**Month7").Value = dtSource.Columns[28].ColumnName;
      //    oXlsReport.Cell("**Month8").Value = dtSource.Columns[29].ColumnName;
      //    oXlsReport.Cell("**Month9").Value = dtSource.Columns[30].ColumnName;
      //    oXlsReport.Cell("**Month10").Value = dtSource.Columns[31].ColumnName;
      //    oXlsReport.Cell("**Month11").Value = dtSource.Columns[32].ColumnName;
      //    oXlsReport.Cell("**Month12").Value = dtSource.Columns[33].ColumnName;
      //    oXlsReport.Cell("**Month13").Value = dtSource.Columns[34].ColumnName;
      //    oXlsReport.Cell("**Month14").Value = dtSource.Columns[35].ColumnName;
      //    oXlsReport.Cell("**Month15").Value = dtSource.Columns[36].ColumnName;
      //    oXlsReport.Cell("**Month16").Value = dtSource.Columns[37].ColumnName;
      //    oXlsReport.Cell("**Month17").Value = dtSource.Columns[38].ColumnName;
      //    oXlsReport.Cell("**Month18").Value = dtSource.Columns[39].ColumnName;
      //    oXlsReport.Cell("**Month19").Value = dtSource.Columns[40].ColumnName;
      //    oXlsReport.Cell("**Month20").Value = dtSource.Columns[41].ColumnName;
      //    oXlsReport.Cell("**Month21").Value = dtSource.Columns[42].ColumnName;
      //    oXlsReport.Cell("**Month22").Value = dtSource.Columns[43].ColumnName;

      //    for (int i = 0; i < ultDetail.Rows.Count; i++)
      //    {
      //      UltraGridRow row = ultDetail.Rows[i];

      //      if (i > 0)
      //      {
      //        oXlsReport.Cell("B3:AN3").Copy();
      //        oXlsReport.RowInsert(2 + i);
      //        oXlsReport.Cell("B3:AN3", 0, i).Paste();
      //      }
      //      oXlsReport.Cell("**No", 0, i).Value = i + 1;
      //      oXlsReport.Cell("**MatCode", 0, i).Value = row.Cells["ID_SanPham"].Value.ToString();
      //      oXlsReport.Cell("**NameEn", 0, i).Value = row.Cells["TenEnglish"].Value.ToString();
      //      oXlsReport.Cell("**Price", 0, i).Value = row.Cells["GiaTien"].Value.ToString();
      //      oXlsReport.Cell("**Supplier", 0, i).Value = row.Cells["Supplier"].Value.ToString();
      //      oXlsReport.Cell("**Unit", 0, i).Value = row.Cells["TenDonViEN"].Value.ToString();
      //      oXlsReport.Cell("**RefCode", 0, i).Value = row.Cells["RefCode"].Value.ToString();

      //      if (DBConvert.ParseDouble(row.Cells["LeadTime"].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**LeadTime", 0, i).Value = DBConvert.ParseDouble(row.Cells["LeadTime"].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**LeadTime", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells["Ending"].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**Ending", 0, i).Value = DBConvert.ParseDouble(row.Cells["Ending"].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**Ending", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells["SoLuong"].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**StockPR", 0, i).Value = DBConvert.ParseDouble(row.Cells["SoLuong"].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**StockPR", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells["MonthlyConsumption"].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**MonthlyConsumption", 0, i).Value = DBConvert.ParseDouble(row.Cells["MonthlyConsumption"].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**MonthlyConsumption", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**Min", 0, i).Value = DBConvert.ParseDouble(row.Cells["MinOfStock"].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**Min", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**ReOrderQty", 0, i).Value = DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**ReOrderQty", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells["Beginning"].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**Beginning", 0, i).Value = DBConvert.ParseDouble(row.Cells["Beginning"].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**Beginning", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells["InStock"].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**InStock", 0, i).Value = DBConvert.ParseDouble(row.Cells["InStock"].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**InStock", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells["OutStock"].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**OutStock", 0, i).Value = DBConvert.ParseDouble(row.Cells["OutStock"].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**OutStock", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[22].Value.ToString()) != double.MinValue)
      //      {

      //        oXlsReport.Cell("**1", 0, i).Value = DBConvert.ParseDouble(row.Cells[22].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**1", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[23].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**2", 0, i).Value = DBConvert.ParseDouble(row.Cells[23].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**2", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[24].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**3", 0, i).Value = DBConvert.ParseDouble(row.Cells[24].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**3", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[25].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**4", 0, i).Value = DBConvert.ParseDouble(row.Cells[25].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**4", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[26].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**5", 0, i).Value = DBConvert.ParseDouble(row.Cells[26].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**5", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[27].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**6", 0, i).Value = DBConvert.ParseDouble(row.Cells[27].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**6", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[28].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**7", 0, i).Value = DBConvert.ParseDouble(row.Cells[28].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**7", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[29].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**8", 0, i).Value = DBConvert.ParseDouble(row.Cells[29].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**8", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[30].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**9", 0, i).Value = DBConvert.ParseDouble(row.Cells[30].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**9", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[31].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**10", 0, i).Value = DBConvert.ParseDouble(row.Cells[31].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**10", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[32].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**11", 0, i).Value = DBConvert.ParseDouble(row.Cells[32].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**11", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[33].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**12", 0, i).Value = DBConvert.ParseDouble(row.Cells[33].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**12", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[34].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**13", 0, i).Value = DBConvert.ParseDouble(row.Cells[34].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**13", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[35].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**14", 0, i).Value = DBConvert.ParseDouble(row.Cells[35].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**14", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[36].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**15", 0, i).Value = DBConvert.ParseDouble(row.Cells[36].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**15", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[37].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**16", 0, i).Value = DBConvert.ParseDouble(row.Cells[37].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**16", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[38].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**17", 0, i).Value = DBConvert.ParseDouble(row.Cells[38].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**17", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[39].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**18", 0, i).Value = DBConvert.ParseDouble(row.Cells[39].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**18", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[40].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**19", 0, i).Value = DBConvert.ParseDouble(row.Cells[40].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**19", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[41].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**20", 0, i).Value = DBConvert.ParseDouble(row.Cells[41].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**20", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[42].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**21", 0, i).Value = DBConvert.ParseDouble(row.Cells[42].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**21", 0, i).Value = "";
      //      }

      //      if (DBConvert.ParseDouble(row.Cells[43].Value.ToString()) != double.MinValue)
      //      {
      //        oXlsReport.Cell("**22", 0, i).Value = DBConvert.ParseDouble(row.Cells[43].Value.ToString());
      //      }
      //      else
      //      {
      //        oXlsReport.Cell("**22", 0, i).Value = "";
      //      }
      //    }
      //  }
      //  else
      //  {
      //    return;
      //  }
      //  oXlsReport.Out.File(strOutFileName);
      //  Process.Start(strOutFileName);
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      int count = this.ultDetail.Rows.Count;
      int dem = 0;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = this.ultDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          dem++;
        }
      }

      if (dem != 0 && dem < count)
      {
        for (int i = 0; i < count; i++)
        {
          UltraGridRow row = this.ultDetail.Rows[i];
          row.Cells["Select"].Value = 1;
        }
      }
      else
      {
        if (dem == 0)
        {
          for (int i = 0; i < count; i++)
          {
            UltraGridRow row = this.ultDetail.Rows[i];
            row.Cells["Select"].Value = 1;
          }
        }
        else if (dem == count)
        {
          for (int i = 0; i < count; i++)
          {
            UltraGridRow row = this.ultDetail.Rows[i];
            row.Cells["Select"].Value = 0;
          }
        }
      }
    }

    private void btnMakePR_Click(object sender, EventArgs e)
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("NameEN", typeof(System.String));
      dt.Columns.Add("Unit", typeof(System.String));
      dt.Columns.Add("Quantity", typeof(System.Double));

      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          DataRow rowData = dt.NewRow();
          rowData["MaterialCode"] = row.Cells["ID_SanPham"].Value.ToString();
          rowData["NameEN"] = row.Cells["TenEnglish"].Value.ToString();
          rowData["Unit"] = row.Cells["TenDonViEN"].Value.ToString();
          rowData["Quantity"] = DBConvert.ParseDouble(row.Cells["ReOrderQty"].Value.ToString());
          dt.Rows.Add(rowData);
        }
      }

      viewPUR_06_002 view = new viewPUR_06_002();
      view.dtData = dt;
      Shared.Utility.WindowUtinity.ShowView(view, "MAKE PR", false, DaiCo.Shared.Utility.ViewState.MainWindow);

    }

    private void ultShowColumns_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      DataTable dtColumn = (DataTable)ultShowColumns.DataSource;
      int count = dtColumn.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        e.Layout.Bands[0].Columns[i].Width = 60;
      }

      e.Layout.Bands[0].Columns["Select"].Hidden = true;
      e.Layout.Bands[0].Columns["ID_SanPham"].Hidden = true;
      e.Layout.Bands[0].Columns["TenEnglish"].Hidden = true;
      e.Layout.Bands[0].Columns["TenVietNam"].Hidden = true;
      e.Layout.Bands[0].Columns["TenDonViEN"].Hidden = true;
      e.Layout.Bands[0].Columns["ReOrder"].Hidden = true;
      e.Layout.Bands[0].Columns["MinimumStock"].Hidden = true;
      e.Layout.Bands[0].Columns["TrungBinhTon"].Hidden = true;
      e.Layout.Bands[0].Columns["MinDefault"].Hidden = true;
      e.Layout.Bands[0].Columns["Average"].Hidden = true;
      e.Layout.Bands[0].Columns["MinOfStockOption"].Hidden = true;

      e.Layout.Bands[0].Columns["GiaTien"].Header.Caption = "Unit Price";
      e.Layout.Bands[0].Columns["GiaTien"].Format = "###,###.##";
      e.Layout.Bands[0].Columns["SoLuong"].Header.Caption = "Stock PR";
      e.Layout.Bands[0].Columns["ReOrderStr"].Header.Caption = "Alert";
      e.Layout.Bands[0].Columns["ReOrderQty"].Header.Caption = "Re-Order";

      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    private void ultShowColumns_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          ultDetail.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          ultDetail.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
        }
      }
      else
      {
        for (int i = 1; i < ultShowColumns.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          row.Cells[i].Value = e.Cell.Text;
        }
        this.ChkAll_CheckedChange();
      }
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }

    private void btnNewReorder_Click(object sender, EventArgs e)
    {
      viewPUR_06_005 view = new viewPUR_06_005();
      Shared.Utility.WindowUtinity.ShowView(view, "REORDER MATERIALS", true, DaiCo.Shared.Utility.ViewState.MainWindow);
    }
    #endregion Event
  }
}
