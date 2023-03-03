/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: view_ExtraControl.cs
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System.Collections;
using System.Diagnostics;
using VBReport;
using System.IO;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_08_002 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    private bool isDuplicateProcess = false;
    #endregion field

    #region function

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

    private void LoadData()
    {
      DBParameter[] input = new DBParameter[1];
      if (txtSaleCode.Text.Length > 0)
      {
        input[0] = new DBParameter("@SaleCode", DbType.String, txtSaleCode.Text.Trim());
      }
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spCSDECatPriceCustomer_Select", input);
      if (dt != null)
      {
        ultraGridInformation.DataSource = dt;
        lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.Count);
      }
    }

    private void LoadGirdColumn()
    {
      DataTable dtColumn = new DataTable();
      dtColumn.Columns.Add("UKRetailPrice", typeof(System.Int32));
      dtColumn.Columns["UKRetailPrice"].DefaultValue = 0;
      dtColumn.Columns.Add("UKTradePrice", typeof(System.Int32));
      dtColumn.Columns["UKTradePrice"].DefaultValue = 0;
      dtColumn.Columns.Add("CNDeadlerPrice", typeof(System.Int32));
      dtColumn.Columns["CNDeadlerPrice"].DefaultValue = 0;
      dtColumn.Columns.Add("CNRetailPrice", typeof(System.Int32));
      dtColumn.Columns["CNRetailPrice"].DefaultValue = 0;
      dtColumn.Columns.Add("OCCPrice", typeof(System.Int32));
      dtColumn.Columns["OCCPrice"].DefaultValue = 0;
      DataRow row = dtColumn.NewRow();
      dtColumn.Rows.Add(row);
      ultGridColumn.DataSource = dtColumn;
      ultGridColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
    }
    private void CheckSaleCodeDuplicate()
    {
      this.isDuplicateProcess = false;
      for (int k = 0; k < ultraGridInformation.Rows.Count; k++)
      {
        UltraGridRow rowcurent = ultraGridInformation.Rows[k];
        rowcurent.CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow rowcurentA = ultraGridInformation.Rows[i];
        string code = rowcurentA.Cells["SaleCode"].Value.ToString();
        for (int x = i + 1; x < ultraGridInformation.Rows.Count; x++)
        {
          UltraGridRow rowcurentB = ultraGridInformation.Rows[x];
          string codeB = rowcurentB.Cells["SaleCode"].Value.ToString();
          if (string.Compare(code, codeB) == 0)
          {
            rowcurentA.CellAppearance.BackColor = Color.Yellow;
            rowcurentB.CellAppearance.BackColor = Color.Yellow;
            this.isDuplicateProcess = true;
          }
        }
      }
    }
    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      if (this.isDuplicateProcess)
      {
        errorMessage = "Data is Duplicate";
        return false;
      }
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["isModifier"].Value.ToString()) == 1)
        {
          ultraGridInformation.Rows[i].Appearance.BackColor = Color.LightGray;
          if (ultraGridInformation.Rows[i].Cells["SaleCode"].Value.ToString().Length == 0)
          {
            ultraGridInformation.Rows[i].Appearance.BackColor = Color.Yellow;
            errorMessage = "Sale Code";
            return false;
          }
        }
      }
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("spCSDECatPriceCustomer_Delete", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update 
        DataTable dtSource = new DataTable();
        dtSource.Columns.Add("Pid", typeof(System.Int64));
        dtSource.Columns.Add("SaleCode", typeof(System.String));
        dtSource.Columns.Add("1", typeof(System.Double));
        dtSource.Columns.Add("2", typeof(System.Double));
        dtSource.Columns.Add("3", typeof(System.Double));
        dtSource.Columns.Add("4", typeof(System.Double));
        dtSource.Columns.Add("5", typeof(System.Double));
        int isAdjust1 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["UKRetailPrice"].Value.ToString());
        int isAdjust2 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["UKTradePrice"].Value.ToString());
        int isAdjust3 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["CNDeadlerPrice"].Value.ToString());
        int isAdjust4 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["CNRetailPrice"].Value.ToString());
        int isAdjust5 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["OCCPrice"].Value.ToString());
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["isModifier"].Value.ToString()) == 1)
          {
            long pid = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["Pid"].Value.ToString());
            string saleCode = ultraGridInformation.Rows[i].Cells["SaleCode"].Value.ToString().Trim();
            double ukRetailPrice = DBConvert.ParseDouble(ultraGridInformation.Rows[i].Cells["UKRetailPrice"].Value.ToString());
            double ukTradePrice = DBConvert.ParseDouble(ultraGridInformation.Rows[i].Cells["UKTradePrice"].Value.ToString());
            double cnDeadlerPrice = DBConvert.ParseDouble(ultraGridInformation.Rows[i].Cells["CNDeadlerPrice"].Value.ToString());
            double cnRetailPrice = DBConvert.ParseDouble(ultraGridInformation.Rows[i].Cells["CNRetailPrice"].Value.ToString());
            double occPrice = DBConvert.ParseDouble(ultraGridInformation.Rows[i].Cells["OCCPrice"].Value.ToString());

            ukRetailPrice = (isAdjust1 == 1) ? ukRetailPrice : -1;
            ukTradePrice = (isAdjust2 == 1) ? ukTradePrice : -1;
            cnDeadlerPrice = (isAdjust3 == 1) ? cnDeadlerPrice : -1;
            cnRetailPrice = (isAdjust4 == 1) ? cnRetailPrice : -1;
            occPrice = (isAdjust5 == 1) ? occPrice : -1;

            DataRow rowItemSource = dtSource.NewRow();
            if (pid != long.MinValue)
            {
              rowItemSource["Pid"] = pid;
            }
            rowItemSource["SaleCode"] = saleCode;
            if (ukRetailPrice != double.MinValue)
            {
              rowItemSource["1"] = ukRetailPrice;
            }
            if (ukTradePrice != double.MinValue)
            {
              rowItemSource["2"] = ukTradePrice;
            }
            if (cnDeadlerPrice != double.MinValue)
            {
              rowItemSource["3"] = cnDeadlerPrice;
            }
            if (cnRetailPrice != double.MinValue)
            {
              rowItemSource["4"] = cnRetailPrice;
            }
            if (occPrice != double.MinValue)
            {
              rowItemSource["5"] = occPrice;
            }
            dtSource.Rows.Add(rowItemSource);
          }
        }
        if (dtSource.Rows.Count > 0)
        {
          SqlDBParameter[] input = new SqlDBParameter[2]; ;
          input[0] = new SqlDBParameter("@DataSource", SqlDbType.Structured, dtSource);
          input[1] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
          SqlDBParameter[] output = new SqlDBParameter[1];
          output[0] = new SqlDBParameter("@Result", SqlDbType.Int, 0);
          SqlDataBaseAccess.ExecuteStoreProcedure("spCSDECatPriceCustomer_Edit", input, output);
          if (output[0].Value.ToString() == "0")
          {
            success = false;
          }
        }

        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    private void FillDataForGrid(DataTable dtItemList)
    {
      DataTable dtSource = new DataTable();
      dtSource.Columns.Add("Pid", typeof(System.Int64));
      dtSource.Columns.Add("SaleCode", typeof(System.String));
      dtSource.Columns.Add("1", typeof(System.Double));
      dtSource.Columns.Add("2", typeof(System.Double));
      dtSource.Columns.Add("3", typeof(System.Double));
      dtSource.Columns.Add("4", typeof(System.Double));
      dtSource.Columns.Add("5", typeof(System.Double));
      int isAdjust1 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["UKRetailPrice"].Value.ToString());
      int isAdjust2 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["UKTradePrice"].Value.ToString());
      int isAdjust3 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["CNDeadlerPrice"].Value.ToString());
      int isAdjust4 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["CNRetailPrice"].Value.ToString());
      int isAdjust5 = DBConvert.ParseInt(ultGridColumn.Rows[0].Cells["OCCPrice"].Value.ToString());
      foreach (DataRow row in dtItemList.Rows)
      {
        string saleCode = row[0].ToString().Trim();
        double ukRetailPrice = DBConvert.ParseDouble(row[1].ToString().Trim());
        double ukTradePrice = DBConvert.ParseDouble(row[2].ToString().Trim());
        double cnDeadlerPrice = DBConvert.ParseDouble(row[3].ToString().Trim());
        double cnRetailPrice = DBConvert.ParseDouble(row[4].ToString().Trim());
        double occPrice = DBConvert.ParseDouble(row[5].ToString().Trim());

        ukRetailPrice = (isAdjust1 == 1) ? ukRetailPrice : -1;
        ukTradePrice = (isAdjust2 == 1) ? ukTradePrice : -1;
        cnDeadlerPrice = (isAdjust3 == 1) ? cnDeadlerPrice : -1;
        cnRetailPrice = (isAdjust4 == 1) ? cnRetailPrice : -1;
        occPrice = (isAdjust5 == 1) ? occPrice : -1;

        if (saleCode.Length > 0)
        {
          DataRow rowItemSource = dtSource.NewRow();
          rowItemSource["SaleCode"] = saleCode;
          if (ukRetailPrice != double.MinValue)
          {
            rowItemSource["1"] = ukRetailPrice;
          }
          if (ukTradePrice != double.MinValue)
          {
            rowItemSource["2"] = ukTradePrice;
          }
          if (cnDeadlerPrice != double.MinValue)
          {
            rowItemSource["3"] = cnDeadlerPrice;
          }
          if (cnRetailPrice != double.MinValue)
          {
            rowItemSource["4"] = cnRetailPrice;
          }
          if (occPrice != double.MinValue)
          {
            rowItemSource["5"] = occPrice;
          }
          dtSource.Rows.Add(rowItemSource);
        }
      }
      SqlDBParameter[] input = new SqlDBParameter[1];
      input[0] = new SqlDBParameter("@DataSource", SqlDbType.Structured, dtSource);
      DataTable dt = SqlDataBaseAccess.SearchStoreProcedureDataTable("spCSDECatPriceCustomer_Import", input);
      if (dt != null)
      {
        ultraGridInformation.DataSource = dt;
        lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.Count);
      }
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["isModifier"].Value.ToString()) == 1)
        {
          ultraGridInformation.Rows[i].Appearance.BackColor = Color.LightGray;
        }
      }
      this.CheckSaleCodeDuplicate();
    }

    #endregion function

    #region event
    public viewCSD_08_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_08_002_Load(object sender, EventArgs e)
    {
      //Init Data
      this.LoadGirdColumn();
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {        
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }
     
      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnTop;
      
      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["isModifier"].Hidden = true;
      
      // Set caption column
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["UKRetailPrice"].Header.Caption = "UK Retail Price";
      e.Layout.Bands[0].Columns["UKTradePrice"].Header.Caption = "UK Trade Price";
      e.Layout.Bands[0].Columns["CNDeadlerPrice"].Header.Caption = "CN Deadler Price";
      e.Layout.Bands[0].Columns["CNRetailPrice"].Header.Caption = "CN Retail Price";
      e.Layout.Bands[0].Columns["OCCPrice"].Header.Caption = "OCC Price";
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (!this.CheckPriceSave())
      {
        WindowUtinity.ShowMessageError("ERR0115", "Price to adjust");
        return;
      }
      this.SaveData();
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }
    private void ultraGridInformation_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.CheckSaleCodeDuplicate();
    }
    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      string value = e.NewValue.ToString();
      switch (colName)
      {
        case "UKRetailPrice":
          {
            if (value.Length > 0 && DBConvert.ParseDouble(value) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", colName);
              e.Cancel = true;
            }   
          }
          break;
        case "UKTradePrice":
          {
            if (value.Length > 0 && DBConvert.ParseDouble(value) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", colName);
              e.Cancel = true;
            }
          }
          break;
        case "CNDeadlerPrice":
          {
            if (value.Length > 0 && DBConvert.ParseDouble(value) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", colName);
              e.Cancel = true;
            }
          }
          break;
        case "CNRetailPrice":
          {
            if (value.Length > 0 && DBConvert.ParseDouble(value) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", colName);
              e.Cancel = true;
            }
          }
          break;
        case "OCCPrice":
          {
            if (value.Length > 0 && DBConvert.ParseDouble(value) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", colName);
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      switch (colName)
      {
        case "UKRetailPrice":
          {
            e.Cell.Row.Cells["isModifier"].Value = 1;
          }
          break;
        case "UKTradePrice":
          {
            e.Cell.Row.Cells["isModifier"].Value = 1;
          }
          break;
        case "CNDeadlerPrice":
          {
            e.Cell.Row.Cells["isModifier"].Value = 1;
          }
          break;
        case "CNRetailPrice":
          {
            e.Cell.Row.Cells["isModifier"].Value = 1;
          }
          break;
        case "OCCPrice":
          {
            e.Cell.Row.Cells["isModifier"].Value = 1;
          }
          break;
        case "SaleCode":
          {
            e.Cell.Row.Cells["isModifier"].Value = 1;
            this.CheckSaleCodeDuplicate();
          }
          break;
        default:
          break;
      }
      this.SetNeedToSave();
    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty; 
    }
    private bool CheckPriceSave()
    {
      int rs = 0;
      for (int i = 0; i < ultGridColumn.Rows[0].Band.Columns.Count; i++)
      {
        if (DBConvert.ParseInt(ultGridColumn.Rows[0].Cells[i].Value.ToString()) == 1)
        {
          rs += 1;
        }
      }
      if (rs > 0)
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      if (!this.CheckPriceSave())
      {
        WindowUtinity.ShowMessageError("ERR0115", "Price to adjust");
        return;
      }
      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [Sheet1 (1)$E2:E3]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [Sheet1 (1)$A4:F{0}]", itemCount + 4));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.FillDataForGrid(dsItemList.Tables[0]);
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "Template_view_08_002";
      string sheetName = "Sheet1";
      string outFileName = "Customer Price Template";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        ControlUtility.GetDataForClipboard(ultraGridInformation);
      }
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultraGridInformation.Selected.Rows.Count > 0 || ultraGridInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultraGridInformation, new Point(e.X, e.Y));
        }
      }
    }
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion event

    private void ultGridColumn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
    }

    private void ultGridColumn_CellChange(object sender, CellEventArgs e)
    {

    }
  }
}
