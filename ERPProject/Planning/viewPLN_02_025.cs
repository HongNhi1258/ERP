/*
  Author      : 
  Date        : 11/02/2014
  Description : Create New WO
  Standard Code: view_GNR_90_005
*/

using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_02_025 : MainUserControl
  {
    #region Field
    public long pid = long.MinValue;
    public int isCloseWo = 0;
    private int status = int.MinValue;
    private IList listDeletedPid = new ArrayList();

    public DataRow[] dtRowDetail;
    private DataTable dtSource;
    #endregion Field

    #region Init

    public viewPLN_02_025()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load viewPLN_02_025
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_025_Load(object sender, EventArgs e)
    {
      this.LoadInit();
      this.LoadData();
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      this.LoadComboType();
      this.LoadComboWOType();
      this.LoadSupplier();
      this.LoadProductionArea();
    }

    /// <summary>
    /// Load UltraCombo Type
    /// </summary>
    private void LoadComboType()
    {
      string commandText = string.Empty;
      commandText = "  SELECT Code, Value + ISNULL(' - ' + [Description], '') Value";
      commandText += " FROM TblBOMCodeMaster ";
      commandText += " WHERE [Group] = " + ConstantClass.GROUP_SALEORDERTYPE + " AND DeleteFlag = 0 ";
      commandText += " ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {        
        Utility.LoadUltraCombo(ultCBType, dtSource, "Code", "Value", false, "Code");       
      }
    }

    /// <summary>
    /// Load WO Type
    /// </summary>
    private void LoadComboWOType()
    {
      string commandText = string.Empty;
      commandText += " SELECT Code, Value, [Description] ";
      commandText += " FROM TblBOMCodeMaster ";
      commandText += " WHERE [Group] = 8050 ";
      commandText += " ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        Utility.LoadUltraCombo(ultWOType, dtSource, "Code", "Value", false, new string[] { "Code", "Description"});
      }
    }

    /// <summary>
    /// Load Supplier
    /// </summary>
    private void LoadSupplier()
    {
      string cmText = @" SELECT Pid Value, EnglishName + ' - ' + DefineCode Display
                         FROM TblPURSupplierInfo
                         WHERE DefineCode IS NOT NULL";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      Utility.LoadUltraCombo(ultraCBSupplier, dt, "Value", "Display", false, "Value");
    }

    /// <summary>
    /// Load Workshop
    /// </summary>
    private void LoadProductionArea()
    {
      string cmText = @"SELECT ProductionArea, ProductionDescription
                      FROM VWIPProductionArea";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      Utility.LoadUltraCombo(ucbProductionArea, dt, "ProductionArea", "ProductionDescription", false, "ProductionArea");
    }
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.pid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNWorkOrderInformation_Select", inputParam);
      if (dsSource != null)
      {
        DataTable dtInfo = dsSource.Tables[0];
        if (dtInfo.Rows.Count > 0)
        {
          DataRow row = dtInfo.Rows[0];
          this.status = DBConvert.ParseInt(row["Confirm"].ToString());
          txtWONO.Text = row["Pid"].ToString();
          txtCreateDate.Text = row["CreateBy"].ToString() + " " + row["CreateDate"].ToString();
          txtRemark.Text = row["Remark"].ToString();
          ultCBType.Value = DBConvert.ParseInt(row["Type"].ToString());
          ultWOType.Value = DBConvert.ParseInt(row["WOType"].ToString());
          ultraCBSupplier.Value = row["Supplier"].ToString();
          txtWoOld.Text = row["WoOld"].ToString();
          if (row["ProductionArea"].ToString().Length > 0)
          {
            ucbProductionArea.Value = row["ProductionArea"].ToString();
          }
        }
        else
        {
          txtCreateDate.Text = SharedObject.UserInfo.EmpName + " " + DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);
        }

        // Load Detail
        ultData.DataSource = dsSource.Tables[1];

        // Set Status Control
        this.SetStatusControl();

        // Set Total CBM, Qty
        this.LoadTotalOfLabel();
      }
    }

    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.status > 0)
      {
        txtWONO.ReadOnly = true;
        //txtWoOld.ReadOnly = true;
        chkConfirm.Checked = true;
        chkConfirm.Enabled = false;
        this.ultWOType.ReadOnly = true;
        //this.ultraCBSupplier.ReadOnly = true;
        this.btnAddDetail.Enabled = true;
      }
      else
      {
        this.btnAddDetail.Enabled = false;
      }

      if (this.pid != long.MinValue)
      {
        txtWONO.ReadOnly = true;
      }
      if (this.isCloseWo == 1)
      {
        btnAddDetail.Enabled = false;
      }
    }

    /// <summary>
    /// Check Vailid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      string commandText = string.Empty;
      DataTable dtCheck = new DataTable();

      // Check WO
      if (txtWONO.Text.Length == 0 ||
        DBConvert.ParseLong(txtWONO.Text) < 0)
      {
        message = "WO";
        return false;
      }
      else
      {
        if (this.pid == long.MinValue)
        {
          commandText = " SELECT Pid FROM TblPLNWorkOrder WHERE Pid = " + DBConvert.ParseLong(txtWONO.Text);
          dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null && dtCheck.Rows.Count > 0)
          {
            message = "WO Exist";
            return false;
          }
        }
      }
      // Check WO Old
      if (txtWoOld.Text.Length == 0)
      {
        message = "Old WO";
        return false;
      }
      // Check Type
      if (ultCBType.Value == null)
      {
        message = "Type";
        return false;
      }

      // Check WO Type
      if (ultWOType.Value == null || DBConvert.ParseInt(this.ultWOType.Value.ToString()) == int.MinValue)
      {
        message = "WO Type";
        return false;
      }
      //Check Supplier
      if (DBConvert.ParseInt(ultWOType.Value.ToString()) == 2 && (ultraCBSupplier.Value == null || ultraCBSupplier.Text.Trim().Length == 0))
      {
        message = "Supplier";
        return false;
      }

      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];

        if (DBConvert.ParseInt(row.Cells["Remain"].Value.ToString())
            < DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()))
        {
          message = "Data On Grid";
          return false;
        }
        string lotNo = row.Cells["LotNo"].Value.ToString();
        if (lotNo.Length < 0)
        {
          message = "LotNo";
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      // Save master info
      bool success = this.SaveInfo();
      if (success)
      {
        // Save detail
        success = this.SaveDetail();
      }
      else
      {
        success = false;
      }
      return success;
    }

    /// <summary>
    /// Save Master Information
    /// </summary>
    /// <returns></returns>
    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[11];

      if (this.pid >= 0)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.pid);
      }
      else
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(txtWONO.Text));
      }

      // Type
      inputParam[1] = new DBParameter("@Type", DbType.Int32, DBConvert.ParseInt(ultCBType.Value.ToString()));

      // Status
      if (chkConfirm.Checked)
      {
        inputParam[2] = new DBParameter("@Confirm", DbType.Int32, 1);
      }
      else
      {
        inputParam[2] = new DBParameter("@Confirm", DbType.Int32, 0);
      }

      inputParam[3] = new DBParameter("@Status", DbType.Int32, 0);

      // Remark
      inputParam[4] = new DBParameter("@Remark", DbType.String, txtRemark.Text);
      // CreateBy, UpdateBy
      inputParam[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[6] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[7] = new DBParameter("@WOType", DbType.Int32, DBConvert.ParseInt(this.ultWOType.Value.ToString()));
      if (ultWOType.Value != null && DBConvert.ParseInt(ultWOType.Value.ToString()) == 2)
      {
        inputParam[8] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(ultraCBSupplier.Value.ToString()));
      }
      inputParam[9] = new DBParameter("@WoOld", DbType.String, txtWoOld.Text.Trim());
      if (ucbProductionArea.SelectedRow != null)
      {
        inputParam[10] = new DBParameter("@ProductionArea", DbType.AnsiString, ucbProductionArea.Value);
      }

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrderInformation_Edit", inputParam, ouputParam);
      // Gan Lai Pid
      this.pid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if (this.pid < 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail()
    {
      // Delete Row In grid
      foreach (long pidDelete in this.listDeletedPid)
      {
        DBParameter[] inputDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pidDelete) };
        DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, long.MinValue) };

        DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrderDetailDelete_Edit", inputDelete, outputDelete);
        long resultDelete = DBConvert.ParseLong(outputDelete[0].Value.ToString());
        if (resultDelete <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0004");
          return false;
        }
      }
      // End

      // Save Detail
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified || row.RowState == DataRowState.Deleted || DBConvert.ParseInt(row["RowState"]) == 1)
        {
          DBParameter[] inputParam = new DBParameter[11];
          if (DBConvert.ParseLong(row["Pid"].ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          }
          inputParam[1] = new DBParameter("@WO", DbType.Int64, this.pid);
          inputParam[2] = new DBParameter("@ItemCode", DbType.String, row["ItemCode"].ToString());
          inputParam[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(row["Revision"].ToString()));
          inputParam[4] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row["Qty"].ToString()));
          inputParam[5] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, DBConvert.ParseLong(row["SaleOrderDetailPid"].ToString()));
          if (DBConvert.ParseInt(row["Priority"].ToString()) != int.MinValue)
          {
            inputParam[6] = new DBParameter("@Priority", DbType.Int32, DBConvert.ParseInt(row["Priority"].ToString()));
          }
          if (row["Note"].ToString().Trim().Length > 0)
          {
            inputParam[7] = new DBParameter("@Note", DbType.String, row["Note"].ToString());
          }
          inputParam[8] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          inputParam[9] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          inputParam[10] = new DBParameter("@LotNo", DbType.String, row["LotNo"].ToString());

          DBParameter[] outPutParam = new DBParameter[1];
          outPutParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spPLNWorkOrderDetail_Edit", inputParam, outPutParam);
          if (DBConvert.ParseLong(outPutParam[0].Value.ToString()) <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }
    /// <summary>
    /// Get LotNo
    /// </summary>
    /// <param name="oldWo"></param>
    /// <returns></returns>
    private DataTable GetWoOld(string woOld)
    {
      string commandText = string.Format(
        @"SELECT WO.MMBatchProductNo WoOld, LONO.MMBatchProductItemProductSerial LotNo, ITEM.MMProductionNormName OldCode
          FROM MMBatchProducts WO
             INNER JOIN MMBatchProductItems LONO ON WO.MMBatchProductID = LONO.FK_MMBatchProductID
             INNER JOIN 
             (
              SELECT MAX(ITEM.MMProductionNormID) MMProductionNormID, ITEM.MMProductionNormName, FK_ICProductID
              FROM MMProductionNorms ITEM
              WHERE AAStatus = 'Alive'
              GROUP BY ITEM.MMProductionNormName, FK_ICProductID
             )ITEM ON LONO.FK_MMProductionNormID = ITEM.MMProductionNormID
          WHERE WO.MMBatchProductNo ='{0}'", woOld);
      DataTable dtBYSWO = BYSDataBaseAccess.SearchCommandTextDataTable(commandText);

      return dtBYSWO;

      //BYSWorkOrder[] woList = Task.Run(async () => await RunAsync()).GetAwaiter().GetResult();
      //DataTable dtBYSWO = new DataTable();
      //dtBYSWO.Columns.AddRange(new DataColumn[] { new DataColumn("WoOld", typeof(string)), new DataColumn("OldCode", typeof(string)),
      //                          new DataColumn("LotNo", typeof(string)) });

      //foreach (var wo in woList)
      //{
      //  if (wo.batchProductNo == woOld)
      //  {
      //    DataRow row = dtBYSWO.NewRow();
      //    row["WoOld"] = wo.batchProductNo;
      //    row["OldCode"] = wo.batchProductItemProductNo;          
      //    row["LotNo"] = wo.batchProductItemProductSerial;
      //    dtBYSWO.Rows.Add(row);
      //  }
      //}

      //return dtBYSWO;
    }
    public async Task<BYSWorkOrder[]> RunAsync()
    {
      BYSWorkOrder[] woList = new BYSWorkOrder[0];
      try
      {
        string url = Utility.GetImagePathByPid(25);

        // Get the product        
        var result = string.Empty;
        HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
          result = await response.Content.ReadAsStringAsync();

          woList = (JsonConvert.DeserializeObject<BYSWOResponseResult>(result, new JsonSerializerSettings())).result;
        }
      }
      catch (Exception e)
      {
        WindowUtinity.ShowMessageErrorFromText(e.Message);
      }
      return woList;
    }

    /// <summary>
    /// load label total qty and cbm
    /// </summary>
    private void LoadTotalOfLabel()
    {
      DataTable dt = (DataTable)ultData.DataSource;
      double totalCbm = 0;
      int totalQty = 0;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState != DataRowState.Deleted)
        {
          if (dt.Rows[i]["CBM"].ToString().Length > 0)
          {
            totalCbm += DBConvert.ParseDouble(dt.Rows[i]["CBM"].ToString()) * DBConvert.ParseDouble(dt.Rows[i]["Qty"].ToString());
          }
          if (dt.Rows[i]["Qty"].ToString().Length > 0)
          {
            totalQty += DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString());
          }
        }
      }
      if (totalCbm > 0)
      {
        labTotalCBM.Text = totalCbm.ToString();
      }
      else
      {
        labTotalCBM.Text = string.Empty;
      }
      if (totalQty > 0)
      {
        labTotalQty.Text = totalQty.ToString();
      }
      else
      {
        labTotalQty.Text = string.Empty;
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
        Utility.ExportToExcelWithDefaultPath(ultData, out xlBook, "WO " + txtWONO.Text, 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "Lam Viet Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Thửa đất số 602,Tờ bản đồ số 39, KP. Khánh Lộc, P. Khánh Bình, Tân Uyên, Bình Dương, Viet Nam.";

        xlSheet.Cells[3, 1] = "WO " + txtWONO.Text;
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
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["WO"].Hidden = true;
      e.Layout.Bands[0].Columns["RowState"].Hidden = true;
      e.Layout.Bands[0].Columns["WOCarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Header.Caption = "Sale Order No";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["Revision"].Header.Caption = "Rev.";
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Remain"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Remain"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Shipped"].MinWidth = 65;
      e.Layout.Bands[0].Columns["Shipped"].MaxWidth = 65;
      e.Layout.Bands[0].Columns["CBM"].MinWidth = 50;
      e.Layout.Bands[0].Columns["CBM"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Priority"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Priority"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["PONo"].Header.Caption = "Customer \nPO No";
      e.Layout.Bands[0].Columns["WIPRun"].Header.Caption = "WIP \nRun";
      e.Layout.Bands[0].Columns["WIPRun"].MinWidth = 60;
      e.Layout.Bands[0].Columns["WIPRun"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["IsConfirm"].Header.Caption = "PLN \nConfirm";
      e.Layout.Bands[0].Columns["IsConfirm"].MinWidth = 60;
      e.Layout.Bands[0].Columns["IsConfirm"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Confirmed \nShip Date";
      e.Layout.Bands[0].Columns["IsSubCon"].Header.Caption = "Subcon";
      e.Layout.Bands[0].Columns["IsSubCon"].MinWidth = 60;
      e.Layout.Bands[0].Columns["IsSubCon"].MaxWidth = 60;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      e.Layout.Bands[0].Columns["WIPRun"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsConfirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Priority"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Note"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["LotNo"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["SaleNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remain"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Shipped"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WIPRun"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["IsConfirm"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["IsSubCon"].CellActivation = Activation.ActivateOnly;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (DBConvert.ParseInt(row.Cells["IsConfirm"].Value.ToString()) != 1)
        {
          if (pid != long.MinValue)
          {
            this.listDeletedPid.Add(pid);
          }
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      bool success = this.CheckVaild(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }

      //Load Data
      this.LoadData();
    }

    private void btnAddDetail_Click(object sender, EventArgs e)
    {
      this.dtRowDetail = null;
      viewPLN_02_003 uc = new viewPLN_02_003();
      uc.parentUC = this;
      uc.currentWorkOrder = this.pid;
      WindowUtinity.ShowView(uc, "Work Order Information", false, ViewState.ModalWindow, FormWindowState.Maximized);

      this.dtSource = (DataTable)ultData.DataSource;
      if (this.dtRowDetail == null)
      {
        return;
      }

      //Get lotno from BYS ERP
      string woOld = txtWoOld.Text.Trim();
      DataTable dtWoOld = new DataTable();
      if (woOld.Length > 0)
      {
        dtWoOld = this.GetWoOld(woOld);
        if (dtWoOld.Rows.Count == 0)
        {
          MessageBox.Show("WoOld not exists !");
        }
      }

      for (int i = 0; i < this.dtRowDetail.Length; i++)
      {
        if (dtRowDetail[i] != null)
        {
          int openQty = DBConvert.ParseInt(dtRowDetail[i]["OpenQty"].ToString());
          if (openQty > 0)
          {
            long saleOrderDetailPid = DBConvert.ParseLong(dtRowDetail[i]["SaleOrderDetailPid"].ToString());
            int remain = 0;
            string commandText = string.Format(@"SELECT Remain FROM VPLNMasterPlan WHERE SaleOrderDetailPid = {0}", saleOrderDetailPid);
            DataTable dtRemain = DataBaseAccess.SearchCommandTextDataTable(commandText);
            remain = DBConvert.ParseInt(dtRemain.Rows[0]["Remain"]);
            bool existDetailPid = false;
            for (int j = 0; j < ultData.Rows.Count; j++)
            {
              long value = DBConvert.ParseLong(ultData.Rows[j].Cells["SaleOrderDetailPid"].Value.ToString());
              if (value == saleOrderDetailPid)
              {
                int qty = DBConvert.ParseInt(ultData.Rows[j].Cells["Qty"].Value.ToString());
                //ultData.Rows[j].Cells["Remain"].Value = remain + qty;
                qty = (qty == int.MinValue) ? openQty : qty + openQty;
                ultData.Rows[j].Cells["Qty"].Value = qty;
                //ultData.Rows[j].Cells["Remain"].Value = remain + qty;
                ultData.Rows[j].Cells["RowState"].Value = 1;
                if (qty > DBConvert.ParseInt(ultData.Rows[j].Cells["Remain"].Value.ToString()))
                {
                  ultData.Rows[j].Appearance.BackColor = Color.Yellow;
                }
                existDetailPid = true;
                break;
              }
            }

            if (!existDetailPid)
            {
              DataRow row = dtSource.NewRow();
              string oldCode = dtRowDetail[i]["OldCode"].ToString();
              if (dtWoOld != null && dtWoOld.Rows.Count > 0)
              {
                DataRow[] rowLotNo = dtWoOld.Select(string.Format("WoOld = '{0}' AND OldCode ='{1}'", woOld, oldCode));
                if (rowLotNo.Length > 0)
                {
                  row["LotNo"] = rowLotNo[0]["LotNo"];
                }
              }
              row["OldCode"] = oldCode;
              row["ItemCode"] = dtRowDetail[i]["ItemCode"];
              row["Revision"] = dtRowDetail[i]["Revision"];
              row["CarcassCode"] = dtRowDetail[i]["CarcassCode"];
              row["SaleNo"] = dtRowDetail[i]["SaleNo"];
              row["PONo"] = dtRowDetail[i]["Customer's PO No"];
              row["SaleOrderDetailPid"] = dtRowDetail[i]["SaleOrderDetailPid"];
              row["ScheduleDelivery"] = dtRowDetail[i]["ScheduleDelivery"];
              row["Remain"] = dtRowDetail[i]["Remain"];
              //row["Remain"] = remain + DBConvert.ParseInt(dtRowDetail[i]["OpenQty"]);
              row["Qty"] = dtRowDetail[i]["OpenQty"];
              row["CBM"] = dtRowDetail[i]["CBM Remain"];
              row["IsConfirm"] = 0;
              row["WIPRun"] = 0;
              dtSource.Rows.Add(row);
            }
          }
        }
      }
      this.LoadTotalOfLabel();
    }

    private void ultData_Click(object sender, EventArgs e)
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
      UltraGridRow row = ultData.Selected.Rows[0];
      string itemCode = row.Cells["ItemCode"].Value.ToString();
      int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
      if (itemCode.Length > 0)
      {
        if (chkShowImage.Checked)
        {
          try
          {
            string local = FunctionUtility.BOMGetItemImage(itemCode, revision);
            FunctionUtility.ShowImagePopup(local);
          }
          catch
          {
          }
        }
      }
    }

    private void btnConfirmDetail_Click(object sender, EventArgs e)
    {
      viewPLN_02_026 view = new viewPLN_02_026();
      view.workOrder = this.pid;
      WindowUtinity.ShowView(view, "WO CONFIRM DETAIL", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Tranfers to Swap WO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
        viewPLN_02_011 view = new viewPLN_02_011();
        view.WoDetail = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        view.Priority = DBConvert.ParseInt(row.Cells["Priority"].Value.ToString());
        view.WorkOrderPID = DBConvert.ParseLong(this.txtWONO.Text.Trim());
        Shared.Utility.WindowUtinity.ShowView(view, "SWAP WORK ORDER", false, Shared.Utility.ViewState.ModalWindow);
        this.LoadData();
      }
      catch
      { }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultData, "Work Order Detail");
      //this.ExportExcel();
    }

    private void btnSubCon_Click(object sender, EventArgs e)
    {
      if (this.pid != long.MinValue)
      {
        viewPLN_02_031 uc = new viewPLN_02_031();
        uc.wo = this.pid;
        WindowUtinity.ShowView(uc, "WO CARCASS CONTRACT OUT RELATION", false, DaiCo.Shared.Utility.ViewState.MainWindow);
      }
    }
    private void ultWOType_ValueChanged(object sender, EventArgs e)
    {
      //Load Supplier
      if (ultWOType.Value != null && DBConvert.ParseInt(ultWOType.Value.ToString()) == 2)
      {
        ultraCBSupplier.ReadOnly = false;
      }
      else
      {
        ultraCBSupplier.ReadOnly = true;
        ultraCBSupplier.Value = null;
      }
    }
    #endregion Event

  }
}
