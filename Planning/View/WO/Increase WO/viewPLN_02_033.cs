/*
  Author      : Huynh Thi Bang
  Date        : 04/09/2015
  Description : Import Increase Wo
  Standard Form: viewPLN_02_033
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;


namespace DaiCo.Planning
{
  public partial class viewPLN_02_033 : MainUserControl
  {
    #region Field
    public long transactionPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init
    public viewPLN_02_033()
    {
      InitializeComponent();
    }

    private void viewPLN_02_033_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function

    private void LoadData()
    {
      if (this.transactionPid == long.MinValue)
      {
        txtTransaction.Text = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNGetNewOutputCodeForChangeNoteWoQuantity('WCQ')", null).Rows[0][0].ToString();
        txtCreateBy.Text = SharedObject.UserInfo.EmpName.ToString();
        txtCreateDate.Text = DateTime.Today.ToString("dd/MM/yyyy");

      }
      else
      {
        DBParameter[] param = new DBParameter[1];
        param[0] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
        string storeName = "spPLNChangeWoQuantityList_Select";
        DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
        txtTransaction.Text = dsSource.Tables[0].Rows[0]["TransactionCode"].ToString();
        txtCreateBy.Text = dsSource.Tables[0].Rows[0]["CreateBy"].ToString();
        txtRemark.Text = dsSource.Tables[0].Rows[0]["Remark"].ToString();
        txtReason.Text = dsSource.Tables[0].Rows[0]["Reason"].ToString();
        txtCreateDate.Text = dsSource.Tables[0].Rows[0]["CreateDate"].ToString();
        //ultData.DataSource = dsSource.Tables[1];
      }
    }
    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "WO Inrease", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "WO Inrease";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "PLN_02_020";
      string sheetName = "WOIncrease";
      string outFileName = "WOIncrease";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      // Get Items Count

      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [WOIncrease (1)$H3:H4]");
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
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [WOIncrease (1)$B5:G{0}]", itemCount + 5));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      //input data
      DataSet dt = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [WOIncrease (1)$B5:G{0}]", itemCount + 5));
      DataTable dtSource = new DataTable();
      dtSource = dt.Tables[0];

      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      DataTable dtInput = this.dtWOIncreaseResult();
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtInput.NewRow();
        if (DBConvert.ParseString(dtSource.Rows[i][1].ToString()) != "" && DBConvert.ParseString(dtSource.Rows[i][2].ToString()) != "")
        {
          if (DBConvert.ParseString(row["WO"].ToString()) != "")
          {
            rowadd["WO"] = DBConvert.ParseLong(row["WO"]);
          }
          if (DBConvert.ParseString(row["ItemCode"].ToString()) != "")
          {
            rowadd["ItemCode"] = DBConvert.ParseString(row["ItemCode"]);
          }
          if (DBConvert.ParseString(row["Revision"].ToString()) != "")
          {
            rowadd["Revision"] = DBConvert.ParseInt(row["Revision"]);
          }
          if (DBConvert.ParseString(row["SaleNo"].ToString()) != "")
          {
            rowadd["SaleNo"] = DBConvert.ParseString(row["SaleNo"]);
          }
          if (DBConvert.ParseString(row["QtyIncrease"]).ToString() != "")
          {
            rowadd["QtyIncrease"] = DBConvert.ParseInt(row["QtyIncrease"]);
          }
          if (DBConvert.ParseString(row["Remark"].ToString()) != "")
          {
            rowadd["Remark"] = DBConvert.ParseString(row["Remark"]);
          }
          dtInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@Data", SqlDbType.Structured, dtInput);
      DataSet dtResult = SqlDataBaseAccess.SearchStoreProcedure("spPLNWoIncrease", sqlinput);
      dtResult.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { dtResult.Tables[0].Columns["Wo"],
                                                                                 dtResult.Tables[0].Columns["CarcassCode"]},
                                                 new DataColumn[] { dtResult.Tables[1].Columns["Wo"],
                                                                                dtResult.Tables[1].Columns["CarcassCode"]}, false));
      ultData.DataSource = dtResult;
      for (int j = 0; j < ultData.Rows.Count; j++)
      {
        if (ultData.Rows[j].Cells["StatusText"].Value.ToString().Trim().Length > 0)
        {
          ultData.Rows[j].Appearance.BackColor = Color.Yellow;
        }
      }

    }
    private DataTable dtWOIncreaseResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int16));
      dt.Columns.Add("SaleNo", typeof(System.String));
      dt.Columns.Add("QtyIncrease", typeof(System.Int16));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      //Check error
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["StatusText"].Value.ToString().Length > 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          return;
        }
      }
      // Add Data      
      if (this.SaveMaster())
      {
        this.AddData();
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }
      this.CloseTab();
    }
    /// <summary>
    /// Save Transaction Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster()
    {
      DBParameter[] input = new DBParameter[6];
      if (this.transactionPid != long.MinValue)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, this.transactionPid);
      }
      input[1] = new DBParameter("@TransactionCode", DbType.AnsiString, 16, txtTransaction.Text);
      input[2] = new DBParameter("@Status", DbType.Int32, 0);
      input[3] = new DBParameter("@CurrencyPid", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (txtRemark.Text.Trim().Length > 0)
      {
        input[4] = new DBParameter("@Remark", DbType.String, 500, txtRemark.Text.Trim());
      }
      if (txtReason.Text.Trim().Length > 0)
      {
        input[5] = new DBParameter("@Reason", DbType.String, 500, txtReason.Text.Trim());
      }

      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantity_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      this.transactionPid = resultSave;
      if (resultSave == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Transaction detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      bool flag = true;
      ////Delete only one time
      //string strDelete = "";

      //foreach (long pidDelete in this.listDeletedPid)
      //{
      //  if (pidDelete > 0)
      //  {
      //    strDelete += pidDelete.ToString() + ",";
      //  }
      //}
      //if (strDelete.Length > 0)
      //{
      //  strDelete = "," + strDelete;

      //  DBParameter[] input = new DBParameter[1];
      //  input[0] = new DBParameter("@DeleteList", DbType.String, 4000, strDelete);
      //  DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      //  DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantityDetail_Delete", input, output);
      //  long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      //  if (resultSave == 0)
      //  {
      //    flag = false;
      //  }
      //}
      //for (int i = 0; i < ultData.Rows.Count; i++)
      //{
      //  DBParameter[] input = new DBParameter[8];
      //  if (DBConvert.ParseLong(ultData.Rows[i].Cells["TransactionDetailPid"].Value.ToString()) != long.MinValue)
      //  {
      //    input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["TransactionDetailPid"].Value.ToString()));
      //  }
      //  input[1] = new DBParameter("@TransactionPid", DbType.Int64, this.transactionPid);
      //  input[2] = new DBParameter("@WorkOrderPid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["WorkOrder"].Value.ToString()));
      //  input[3] = new DBParameter("@CarcassCode", DbType.AnsiString, 32, ultData.Rows[i].Cells["CarcassCode"].Value.ToString());
      //  input[4] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["NewQty"].Value.ToString()));
      //  input[5] = new DBParameter("@OldQty	", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["OldQty"].Value.ToString()));
      //  if (ultData.Rows[i].Cells["Remark"].Value.ToString().Length > 0)
      //  {
      //    input[6] = new DBParameter("@Remark", DbType.AnsiString, 500, ultData.Rows[i].Cells["Remark"].Value.ToString());
      //  }
      //  input[7] = new DBParameter("@FlagAccept", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["FlagAccept"].Value.ToString()));
      //  DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      //  DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantityDetail_Edit", input, output);
      //  long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      //  if (resultSave == 0)
      //  {
      //    flag = false;
      //  }

      //}
      return flag;
    }
    /// <summary>
    /// Add Data
    /// </summary>
    /// <returns></returns>
    private void AddData()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        //int newQty = DBConvert.ParseInt(ultData.Rows[i].Cells["QtyIncrease"].Value) + DBConvert.ParseInt(ultData.Rows[i].Cells["Qty"].Value);
        DBParameter[] input = new DBParameter[7];
        input[0] = new DBParameter("@TransactionPid", DbType.Int64, this.transactionPid);
        input[1] = new DBParameter("@WorkOrderPid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["Wo"].Value.ToString()));
        input[2] = new DBParameter("@CarcassCode", DbType.String, ultData.Rows[i].Cells["CarcassCode"].Value.ToString());
        input[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["NewQty"].Value.ToString()));
        input[4] = new DBParameter("@OldQty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["OldQty"].Value.ToString()));
        if (ultData.Rows[i].Cells["Remark"].Value.ToString().Length > 0)
        {
          input[5] = new DBParameter("@Remark", DbType.String, ultData.Rows[i].Cells["Remark"].Value.ToString());
        }
        input[6] = new DBParameter("@FlagAccept", DbType.Int32, 0);
        this.AddDetail(input, i);
      }
    }
    /// <summary>
    /// Add Detail
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private bool AddDetail(DBParameter[] input, int index)
    {
      bool flag = true;
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantityDetail_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      if (resultSave == 0)
      {
        flag = false;
      }
      else
      {
        this.AddWoLinkSo(resultSave, index);
      }
      return flag;
    }
    /// <summary>
    /// Add WorkOrder Link SaleOrder
    /// </summary>
    private void AddWoLinkSo(long TransactionDetailPid, int RowIndex)
    {
      int i = RowIndex;
      if (ultData.Rows[i].Cells["StatusText"].Value.ToString() == "")
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          DBParameter[] input = new DBParameter[4];
          input[0] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["SaleOrderDetailPid"].Value.ToString()));
          input[1] = new DBParameter("@WorkOrderPid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["Wo"].Value.ToString()));
          input[2] = new DBParameter("@ReviseQty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["QtyIncrease"].Value.ToString()));
          input[3] = new DBParameter("@TransactionDetailPid", DbType.Int64, this.transactionPid);
          bool flag = true;
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteQuantityWoLinkSo_Edit", input, output);
          long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
          if (resultSave == 0)
          {
            flag = false;
          }
        }
      }
    }

    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }

      e.Layout.Bands[0].Columns["FlagAccept"].Hidden = true;
      //e.Layout.Bands[0].Columns["TransactionDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["StatusText"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["StatusText"].MinWidth = 200;
      e.Layout.Bands[0].Columns["FlagAccept"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["FlagAccept"].Header.Caption = "Accepted";

      //e.Layout.Bands[1].Columns["ScheduleDelivery"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Wo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;

      //e.Layout.Bands[1].Columns["SaleNo"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[1].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["StatusText"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;

    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion Event
  }
}
