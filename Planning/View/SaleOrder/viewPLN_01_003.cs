/*
  Author      : Lậm Quang Hà
  Date        : 01/06/2010
  Decription  : Tìm kiếm thông tin Sale Order  
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;
namespace DaiCo.Planning
{
  public partial class viewPLN_01_003 : MainUserControl
  {
    private bool isEditlayOut = false;
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public viewPLN_01_003()
    {
      InitializeComponent();
      dt_POFrom.Value = DateTime.MinValue;
      dt_POTo.Value = DateTime.MinValue;
      ultReceivedDateFrom.Value = DBNull.Value;
      ultReceivedDateTo.Value = DBNull.Value;
    }

    #region Load Data

    private void viewPLN_01_003_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void LoadData()
    {
      // Customer
      Shared.Utility.ControlUtility.LoadCustomer(cmbCustomer);
      // Type
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);

      //Status
      this.LoadStatus();
      // Create By
      Shared.Utility.ControlUtility.LoadEmployee(cmbCreateBy, "CSD");
      //btnPrint.Enabled = false;
    }

    private void LoadStatus()
    {
      DataTable dtStatus = new DataTable();
      dtStatus.Columns.Add("Value", typeof(System.Int16));
      dtStatus.Columns.Add("Text", typeof(System.String));
      DataRow row1 = dtStatus.NewRow();
      row1["Value"] = 0;
      row1["Text"] = "New";
      dtStatus.Rows.Add(row1);
      DataRow row2 = dtStatus.NewRow();
      row2["Value"] = 2;
      row2["Text"] = "CS Confirm";
      dtStatus.Rows.Add(row2);
      DataRow row3 = dtStatus.NewRow();
      row3["Value"] = 3;
      row3["Text"] = "PLN Confirm";
      dtStatus.Rows.Add(row3);
      ControlUtility.LoadCombobox(cmbStatus, dtStatus, "Value", "Text");
    }

    #endregion Load Data

    #region Search

    private void Search()
    {
      DBParameter[] param = new DBParameter[15];
      // No
      string text = txtNoFrom.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@NoFrom", DbType.AnsiString, 20, text);
      }
      text = txtNoTo.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@NoTo", DbType.AnsiString, 20, text);
      }
      text = txtNoSet.Text.Trim();
      string[] listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[2] = new DBParameter("@NoSet", DbType.AnsiString, 1000, text);
      }
      // Customer's PO No
      text = txtCusNoFrom.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@CusOrderNoFrom", DbType.AnsiString, 20, text);
      }
      text = txtCusNoTo.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@CusOrderNoTo", DbType.AnsiString, 20, text);
      }
      text = txtCusNoSet.Text.Trim();
      listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[5] = new DBParameter("@CusOrderNoSet", DbType.AnsiString, 1000, text);
      }
      //Order Date

      if (dt_POFrom.Value != DateTime.MinValue)
      {
        DateTime orderDate = dt_POFrom.Value;
        param[6] = new DBParameter("@OrderDateFrom", DbType.DateTime, new DateTime(orderDate.Year, orderDate.Month, orderDate.Day));
      }


      if (dt_POTo.Value != DateTime.MinValue)
      {
        DateTime orderDate = dt_POTo.Value;
        orderDate = orderDate.AddDays(1);
        param[7] = new DBParameter("@OrderDateTo", DbType.DateTime, new DateTime(orderDate.Year, orderDate.Month, orderDate.Day));
      }
      // Customer
      long customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      if (customerPid != long.MinValue)
      {
        param[8] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }
      // Type
      int value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      if (value != int.MinValue)
      {
        param[9] = new DBParameter("@Type", DbType.Int32, value);
      }
      // Status
      value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbStatus));
      if (value >= 0)
      {
        param[10] = new DBParameter("@Status", DbType.Int32, value);
      }
      // ItemCode
      text = txtItemCode.Text.Trim();
      if (text.Length > 0)
      {
        param[11] = new DBParameter("@ItemCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
      }
      // Create By
      value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCreateBy));
      if (value != int.MinValue)
      {
        param[12] = new DBParameter("@CreateBy", DbType.Int32, value);
      }

      if (ultReceivedDateFrom.Value != null)
      {
        DateTime receivedFrom =
            DBConvert.ParseDateTime(ultReceivedDateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        param[13] = new DBParameter("@ReceivePLADateFrom", DbType.DateTime, receivedFrom);
      }

      if (ultReceivedDateTo.Value != null)
      {
        DateTime receivedTo =
            DBConvert.ParseDateTime(ultReceivedDateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        receivedTo = receivedTo.AddDays(1);
        param[14] = new DBParameter("@ReceivePLADateTo", DbType.DateTime, receivedTo);
      }

      string storeName = "spPLNListSaleOrder";
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
      DataSet dsData = Shared.Utility.CreateDataSet.SaleOrder_Enquiry();
      dsData.Tables["dtParent"].Merge(dsSource.Tables[0]);
      dsData.Tables["dtChild"].Merge(dsSource.Tables[1]);
      ultData.DataSource = dsData;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string status = ultData.Rows[i].Cells["ST"].Value.ToString().Trim();
        if (status == "2")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Cyan;
        }
        else if (status == "3")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (status == "4")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
      }
      if (!this.isEditlayOut)
      {
        this.Editlayout();
        this.isEditlayOut = true;
      }
    }

    private void ExportExcel()
    {
      string strTemplateName = "RPT_PLN_01_003";
      string strSheetName = "Sheet1";
      string strOutFileName = "Sale Order Information";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      oXlsReport.Cell("**Date").Value = DBConvert.ParseDateTime(DateTime.Today.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);

      //Search
      DBParameter[] param = new DBParameter[15];
      // No
      string text = txtNoFrom.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@NoFrom", DbType.AnsiString, 20, text);
      }
      text = txtNoTo.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@NoTo", DbType.AnsiString, 20, text);
      }
      text = txtNoSet.Text.Trim();
      string[] listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[2] = new DBParameter("@NoSet", DbType.AnsiString, 1000, text);
      }
      // Customer's PO No
      text = txtCusNoFrom.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@CusOrderNoFrom", DbType.AnsiString, 20, text);
      }
      text = txtCusNoTo.Text.Trim().Replace("'", "");
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@CusOrderNoTo", DbType.AnsiString, 20, text);
      }
      text = txtCusNoSet.Text.Trim();
      listNo = text.Split(',');
      text = string.Empty;
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          text += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }
      if (text.Length > 0)
      {
        text = string.Format("{0}", text.Remove(0, 1));
        param[5] = new DBParameter("@CusOrderNoSet", DbType.AnsiString, 1000, text);
      }
      //Order Date

      if (dt_POFrom.Value != DateTime.MinValue)
      {
        DateTime orderDate = dt_POFrom.Value;
        param[6] = new DBParameter("@OrderDateFrom", DbType.DateTime, new DateTime(orderDate.Year, orderDate.Month, orderDate.Day));
      }


      if (dt_POTo.Value != DateTime.MinValue)
      {
        DateTime orderDate = dt_POTo.Value;
        orderDate = orderDate.AddDays(1);
        param[7] = new DBParameter("@OrderDateTo", DbType.DateTime, new DateTime(orderDate.Year, orderDate.Month, orderDate.Day));
      }
      // Customer
      long customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      if (customerPid != long.MinValue)
      {
        param[8] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      }
      // Type
      int value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      if (value != int.MinValue)
      {
        param[9] = new DBParameter("@Type", DbType.Int32, value);
      }
      // Status
      try
      {
        value = cmbStatus.SelectedIndex;
        value = (value == -1 || value == 0) ? -1 : value + 1;
      }
      catch
      {
        value = int.MinValue;
      }
      if (value >= 0)
      {
        param[10] = new DBParameter("@Status", DbType.Int32, value);
      }
      // ItemCode
      text = txtItemCode.Text.Trim();
      if (text.Length > 0)
      {
        param[11] = new DBParameter("@ItemCode", DbType.AnsiString, 18, "%" + text.Replace("'", "''") + "%");
      }
      // Create By
      value = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCreateBy));
      if (value != int.MinValue)
      {
        param[12] = new DBParameter("@CreateBy", DbType.Int32, value);
      }

      if (ultReceivedDateFrom.Value != null)
      {
        DateTime receivedFrom =
            DBConvert.ParseDateTime(ultReceivedDateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        param[13] = new DBParameter("@ReceivePLADateFrom", DbType.DateTime, receivedFrom);
      }

      if (ultReceivedDateTo.Value != null)
      {
        DateTime receivedTo =
            DBConvert.ParseDateTime(ultReceivedDateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        receivedTo = receivedTo.AddDays(1);
        param[14] = new DBParameter("@ReceivePLADateTo", DbType.DateTime, receivedTo);
      }

      DataTable dtData = DataBaseAccess.SearchStoreProcedureDataTable("spPLNRPTListSaleOrder", 300, param);
      // End Search

      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B8:V8").Copy();
            oXlsReport.RowInsert(7 + i);
            oXlsReport.Cell("B8:V8", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"].ToString();
          oXlsReport.Cell("**SaleCode", 0, i).Value = dtRow["SaleCode"].ToString();
          oXlsReport.Cell("**Revision", 0, i).Value = DBConvert.ParseInt(dtRow["Revision"].ToString());
          oXlsReport.Cell("**SO", 0, i).Value = dtRow["SaleNo"].ToString();
          oXlsReport.Cell("**PONo", 0, i).Value = dtRow["CustomerPONo"].ToString();
          oXlsReport.Cell("**Type", 0, i).Value = dtRow["Type"].ToString();
          if (dtRow["PODate"].ToString().Length > 0)
          {
            oXlsReport.Cell("**PODate", 0, i).Value = dtRow["PODate"];
          }
          else
          {
            oXlsReport.Cell("**PODate", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**CustomerName", 0, i).Value = dtRow["CustomerName"].ToString();
          oXlsReport.Cell("**DirectCustomer", 0, i).Value = dtRow["DirectCustomer"].ToString();
          if (DBConvert.ParseInt(dtRow["Qty"].ToString()) != int.MinValue)
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseInt(dtRow["Qty"].ToString());
          }
          else
          {
            oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
          }
          if (DBConvert.ParseDouble(dtRow["UnitCBM"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**UnitCBM", 0, i).Value = DBConvert.ParseDouble(dtRow["UnitCBM"].ToString());
          }
          else
          {
            oXlsReport.Cell("**UnitCBM", 0, i).Value = double.MinValue;
          }
          if (DBConvert.ParseDouble(dtRow["CBMS"].ToString()) != double.MinValue)
          {
            oXlsReport.Cell("**CBMS", 0, i).Value = DBConvert.ParseDouble(dtRow["CBMS"].ToString());
          }
          else
          {
            oXlsReport.Cell("**CBMS", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**SpecialIntruction", 0, i).Value = dtRow["SpecialInstruction"].ToString();
          oXlsReport.Cell("**Remark", 0, i).Value = dtRow["Remark"].ToString();
          oXlsReport.Cell("**RemarkDetail", 0, i).Value = dtRow["RemarkDetail"].ToString();
          if (dtRow["ConfirmedShipdate"].ToString().Length > 0)
          {
            oXlsReport.Cell("**ConfirmedShipdate", 0, i).Value = dtRow["ConfirmedShipdate"];
          }
          else
          {
            oXlsReport.Cell("**ConfirmedShipdate", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**PackingRequirement", 0, i).Value = dtRow["PackingRequirement"].ToString();
          oXlsReport.Cell("**DeliveryRequirement", 0, i).Value = dtRow["DeliveryRequirement"].ToString();
          if (dtRow["DateToPLA"].ToString().Length > 0)
          {
            oXlsReport.Cell("**DateToPLA", 0, i).Value = dtRow["DateToPLA"];
          }
          else
          {
            oXlsReport.Cell("**DateToPLA", 0, i).Value = DBNull.Value;
          }
          oXlsReport.Cell("**ENQ", 0, i).Value = dtRow["EnquiryNo"].ToString();
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    #endregion Search

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      viewPLN_01_004 uc = new viewPLN_01_004();
      long pid = long.MinValue;
      try
      {
        pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      }
      catch
      {
      }
      uc.saleOrderPid = pid;
      Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE SALE ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
      this.Search();
    }

    private void Editlayout()
    {
      ultData.DisplayLayout.Bands["dtParent_dtChild"].Columns["ParentPid"].Hidden = true;
      ultData.DisplayLayout.Bands["dtParent"].Columns["Pid"].Hidden = true;
      ultData.DisplayLayout.Bands["dtParent"].Columns["No"].Header.Caption = "Sale No";
      ultData.DisplayLayout.Bands["dtParent"].Columns["CusNo"].Header.Caption = "Customer Po No";
      ultData.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
      ultData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    private void ultData_Click(object sender, EventArgs e)
    {
      string status = string.Empty;
      try
      {
        status = ultData.Selected.Rows[0].Cells["Status"].Value.ToString().Trim();
      }
      catch
      {
      }
      //if (status == "PLN Confirmed")
      //{
      //  btnPrint.Enabled = true;
      //}
      //else
      //{
      //  btnPrint.Enabled = false;
      //}
    }
    private void btnClear_Click(object sender, EventArgs e)
    {
      txtNoFrom.Text = string.Empty;
      txtNoTo.Text = string.Empty;
      txtNoSet.Text = string.Empty;
      txtCusNoFrom.Text = string.Empty;
      txtCusNoTo.Text = string.Empty;
      txtCusNoSet.Text = string.Empty;
      dt_POFrom.Value = DateTime.MinValue;
      dt_POTo.Value = DateTime.MinValue;
      cmbCustomer.SelectedIndex = 0;
      cmbType.SelectedIndex = 0;
      cmbStatus.SelectedIndex = 0;
      txtItemCode.Text = string.Empty;
      cmbCreateBy.SelectedIndex = 0;
      ultReceivedDateFrom.Value = DBNull.Value;
      ultReceivedDateTo.Value = DBNull.Value;
      txtNoFrom.Focus();
    }

    /// <summary>
    /// Layout UltData
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["ST"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].Header.Caption = "Sale No";
      e.Layout.Bands[0].Columns["CusNo"].Header.Caption = "Customer Po No";
      e.Layout.Bands[0].Columns["OrderDate"].Header.Caption = "Order Date";
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[1].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["CBM"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["TotalCBM"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
    }

    /// <summary>
    /// export excel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnExport_Click(object sender, EventArgs e)
    {
      btnExport.Enabled = false;
      this.ExportExcel();
      btnExport.Enabled = true;
    }

    static public bool exportDataToExcel(string tieude, DataTable dt)
    {
      bool result = false;
      //khoi tao cac doi tuong Com Excel de lam viec
      Excel.ApplicationClass xlApp;
      Excel.Worksheet xlSheet;
      Excel.Workbook xlBook;
      //doi tuong Trống để thêm  vào xlApp sau đó lưu lại sau
      object missValue = System.Reflection.Missing.Value;
      //khoi tao doi tuong Com Excel moi
      xlApp = new Excel.ApplicationClass();
      xlBook = xlApp.Workbooks.Add(missValue);
      //su dung Sheet dau tien de thao tac
      xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);
      //không cho hiện ứng dụng Excel lên để tránh gây đơ máy
      xlApp.Visible = false;
      int socot = dt.Columns.Count;
      int sohang = dt.Rows.Count;
      int i, j;

      SaveFileDialog f = new SaveFileDialog();
      f.Filter = "Excel file (*.xls)|*.xls";
      if (f.ShowDialog() == DialogResult.OK)
      {
        string strName = f.FileName;
      GoBack:
        ;
        try
        {
          File.Open(f.FileName, FileMode.OpenOrCreate).Close();
        }
        catch
        {
          MessageBox.Show("Already Opened:" + f.FileName + "\nPlease find and close it", "Can not save!");
          goto GoBack;
        }
        try
        {
          File.Delete(strName);
        }
        catch { }
        //set thuoc tinh cho tieu de
        xlSheet.get_Range("A1", Convert.ToChar(socot + 65) + "1").Merge(false);
        Excel.Range caption = xlSheet.get_Range("A1", Convert.ToChar(socot + 65) + "1");
        caption.Select();
        caption.FormulaR1C1 = tieude;
        //căn lề cho tiêu đề
        caption.HorizontalAlignment = Excel.Constants.xlCenter;
        caption.Font.Bold = true;
        caption.VerticalAlignment = Excel.Constants.xlCenter;
        caption.Font.Size = 15;
        //màu nền cho tiêu đề
        caption.Interior.ColorIndex = 20;
        caption.RowHeight = 30;
        //set thuoc tinh cho cac header
        Excel.Range header = xlSheet.get_Range("A2", Convert.ToChar(socot + 65) + "2");
        header.Select();

        header.HorizontalAlignment = Excel.Constants.xlCenter;
        header.Font.Bold = true;
        header.Font.Size = 10;
        //điền tiêu đề cho các cột trong file excel
        for (i = 0; i < socot; i++)
          xlSheet.Cells[2, i + 2] = dt.Columns[i].ColumnName;
        //dien cot stt
        xlSheet.Cells[2, 1] = "STT";

        //dien du lieu vao sheet
        for (i = 0; i < sohang; i++)
          for (j = 0; j < socot; j++)
          {
            if (j == 1)
            {
              xlSheet.Cells[i + 3, j] = i + 1;
            }
            xlSheet.Cells[i + 3, j + 2] = dt.Rows[i][j];
          }
        //autofit độ rộng cho các cột
        //for (i = 0; i < sohang; i++)
        //  ((Excel.Range)xlSheet.Cells[1, i + 1]).EntireColumn.AutoFit();
        xlSheet.Columns.AutoFit();
        //save file
        xlBook.SaveAs(strName, Excel.XlFileFormat.xlWorkbookNormal, missValue, missValue, missValue, missValue, Excel.XlSaveAsAccessMode.xlExclusive, missValue, missValue, missValue, missValue, missValue);
        xlBook.Close(true, missValue, missValue);
        xlApp.Quit();

        // release cac doi tuong COM
        releaseObject(xlSheet);
        releaseObject(xlBook);
        releaseObject(xlApp);
        result = true;
      }
      return result;
    }

    static public void releaseObject(object obj)
    {
      try
      {
        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        obj = null;
      }
      catch (Exception ex)
      {
        obj = null;
        throw new Exception("Exception Occured while releasing object " + ex.ToString());
      }
      finally
      {
        GC.Collect();
      }
    }

    private void txtNoFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtNoTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtNoSet_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtCusNoSet_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtCusNoTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtCusNoFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dt_POFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dt_POTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmbCustomer_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmbType_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmbStatus_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmbCreateBy_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    #endregion Event

  }
}
