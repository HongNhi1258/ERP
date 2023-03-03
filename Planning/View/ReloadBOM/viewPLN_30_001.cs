/*
  Author      : Huynh Thi Bang
  Date        : 05/01/2016
  Description : Reload BOM
  Standard Form: viewPLN_30_001
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

namespace DaiCo.Planning
{
  public partial class viewPLN_30_001 : MainUserControl
  {
    #region Field
    public long transactionPid = long.MinValue;
    private int status = int.MinValue;
    private bool isDuplicateProcess = false;
    #endregion Field

    #region Init
    public viewPLN_30_001()
    {
      InitializeComponent();
    }

    private void viewPLN_30_001_Load(object sender, EventArgs e)
    {
      this.LoadWorkOrder();
      this.LoadReason();
      this.LoadMainData();
      this.LoadDetailData(this.transactionPid);
    }

    /// <summary>
    /// Load WO
    /// </summary>
    private void LoadWorkOrder()
    {
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT DISTINCT WorkOrderPid WO
                                    FROM TblPLNWorkOrderConfirmedDetails
                                    WHERE ISNULL(CloseCAR, 0) = 0
	                                    AND ISNULL(CloseAll, 0) = 0
                                    ORDER BY WO");

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropWO.DataSource = dtSource;
      ultDropWO.DisplayMember = "WO";
      ultDropWO.ValueMember = "WO";
      ultDropWO.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropWO.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load CarcassCode
    /// </summary>
    /// <param name="wo"></param>
    private void LoadCarcassCode(int wo)
    {
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT DISTINCT CarcassCode
                                    FROM TblPLNWorkOrderConfirmedDetails
                                    WHERE WorkOrderPid = {0} 
	                                    AND ISNULL(CloseCAR, 0) = 0
	                                    AND ISNULL(CloseAll, 0) = 0", wo);

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropCarcassCode.DataSource = dtSource;
      ultDropCarcassCode.DisplayMember = "CarcassCode";
      ultDropCarcassCode.ValueMember = "CarcassCode";
      ultDropCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropCarcassCode.DisplayLayout.AutoFitColumns = true;
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    /// <param name="wo"></param>
    /// <param name="carcassCode"></param>
    private void LoadItemCode(int wo, string carcassCode)
    {
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT DISTINCT ItemCode, Revision
                                    FROM TblPLNWorkOrderConfirmedDetails
                                    WHERE WorkOrderPid = {0} AND CarcassCode = '{1}'
                                        AND ISNULL(CloseCAR, 0) = 0
                                        AND ISNULL(CloseAll, 0) = 0", wo, carcassCode);

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropItemCode.DataSource = dtSource;
      ultDropItemCode.DisplayMember = "ItemCode";
      ultDropItemCode.ValueMember = "ItemCode";
      ultDropItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultDropItemCode.DisplayLayout.AutoFitColumns = true;
      ultDropItemCode.DisplayLayout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }
    /// <summary>
    /// Load Reason
    /// </summary>
    private void LoadReason()
    {
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 24");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultDropReason.DataSource = dtSource;
      ultDropReason.DisplayMember = "Value";
      ultDropReason.ValueMember = "Code";
      ultDropReason.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDropReason.DisplayLayout.AutoFitColumns = true;
      ultDropReason.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;

    }
    /// <summary>
    /// Load Main Data
    /// </summary>
    private void LoadMainData()
    {
      DBParameter[] inputParam = new DBParameter[1] { new DBParameter("@TransactionPid", DbType.Int64, this.transactionPid) };
      DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spPLNReloadBOM_Select", inputParam);

      if (dtMain != null && dtMain.Rows.Count > 0)
      {
        txtTransaction.Text = dtMain.Rows[0]["TransactionCode"].ToString();
        txtCreateBy.Text = dtMain.Rows[0]["CreateBy"].ToString();
        txtCreateDate.Text = dtMain.Rows[0]["CreateDate"].ToString();
        txtRemark.Text = dtMain.Rows[0]["Remark"].ToString();
        this.status = DBConvert.ParseInt(dtMain.Rows[0]["Status"].ToString());
      }
      else
      {
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FPLNReloadBOM('REL') TransactionCode");
        if ((dt != null) && (dt.Rows.Count > 0))
        {
          txtTransaction.Text = dt.Rows[0]["TransactionCode"].ToString();
          txtCreateBy.Text = SharedObject.UserInfo.UserPid.ToString() + " - " + SharedObject.UserInfo.EmpName;
          txtCreateDate.Text = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME);
        }
      }

      // Set Status Control
      this.SetStatusControl();
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Set Status Control
    /// </summary>
    private void SetStatusControl()
    {
      if (this.status == 1)
      {
        btnGetData.Enabled = false;
        txtRemark.ReadOnly = true;
        btnSave.Enabled = false;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (btnGetData.Enabled == false)
        {
          for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowChild = row.ChildBands[0].Rows[j];
            row.ChildBands[0].Rows[j].Activation = Activation.ActivateOnly;
            row.Activation = Activation.ActivateOnly;
            rowChild.Cells["WO"].Column.Hidden = true;
            rowChild.Cells["CarcassCode"].Column.Hidden = true;
            rowChild.Cells["ItemCode"].Column.Hidden = true;
            rowChild.Cells["Revision"].Column.Hidden = true;
            rowChild.Cells["Status"].Column.Hidden = true;

            // Set Color
            if (String.Compare(rowChild.Cells["RemarkStatus"].Value.ToString(), "Insert New", true) == 0)
            {
              row.ChildBands[0].Rows[j].Appearance.BackColor = Color.Yellow;
            }
            else if (String.Compare(rowChild.Cells["RemarkStatus"].Value.ToString(), "Delete", true) == 0)
            {
              row.ChildBands[0].Rows[j].Appearance.BackColor = Color.Red;
            }
            else if (String.Compare(rowChild.Cells["RemarkStatus"].Value.ToString(), "Update QtyPerMaterial", true) == 0)
            {
              row.ChildBands[0].Rows[j].Appearance.BackColor = Color.Pink;
            }

            // set status column
            row.ChildBands[0].Band.Override.AllowDelete = DefaultableBoolean.False;
            row.ChildBands[0].Band.Override.AllowUpdate = DefaultableBoolean.False;

            row.Band.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            row.Band.Override.AllowDelete = DefaultableBoolean.False;
            row.Band.Override.AllowUpdate = DefaultableBoolean.False;

          }
        }
      }
    }
    /// <summary>
    /// Load Data Detail
    /// </summary>
    /// <param name="cancelWOPid"></param>
    private void LoadDetailData(long transactionPid)
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@TransactionPid", DbType.Int64, this.transactionPid);
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNReloadBOMDetail", inputParam);
      if (dsSource != null)
      {
        DataSet dsInfo = this.CreateDataSet();
        dsInfo.Tables["dtParent"].Merge(dsSource.Tables[0]);
        dsInfo.Tables["dtChildMaterial"].Merge(dsSource.Tables[1]);
        ultData.DataSource = dsInfo;
      }
      this.SetStatusControl();
    }

    /// <summary>
    /// Create Data Set Get Data
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("WO", typeof(System.Int64));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("Reason", typeof(System.Int32));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);

      // Material
      DataTable taChildMaterial = new DataTable("dtChildMaterial");
      taChildMaterial.Columns.Add("WO", typeof(System.Int64));
      taChildMaterial.Columns.Add("CarcassCode", typeof(System.String));
      taChildMaterial.Columns.Add("ItemCode", typeof(System.String));
      taChildMaterial.Columns.Add("Revision", typeof(System.Int32));
      taChildMaterial.Columns.Add("MaterialCode", typeof(System.String));
      taChildMaterial.Columns.Add("MaterialNameEn", typeof(System.String));
      taChildMaterial.Columns.Add("MaterialNameVn", typeof(System.String));
      taChildMaterial.Columns.Add("Unit", typeof(System.String));
      taChildMaterial.Columns.Add("AlternativeCode", typeof(System.String));
      taChildMaterial.Columns.Add("QtyPerMaterialOld", typeof(System.Double));
      taChildMaterial.Columns.Add("QtyPerMaterialNew", typeof(System.Double));
      taChildMaterial.Columns.Add("WOQtyOld", typeof(System.Int32));
      taChildMaterial.Columns.Add("WOQtyNew", typeof(System.Int32));
      taChildMaterial.Columns.Add("Status", typeof(System.Int32));
      ds.Tables.Add(taChildMaterial);

      ds.Relations.Add(new DataRelation("dtParent_dtChildMaterial", new DataColumn[] { taParent.Columns["WO"], taParent.Columns["ItemCode"], taParent.Columns["Revision"] }, new DataColumn[] { taChildMaterial.Columns["WO"], taChildMaterial.Columns["ItemCode"], taChildMaterial.Columns["Revision"] }, false));
      return ds;
    }
    /// <summary>
    /// Check Valid Get Data
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        //check wo
        if (row.Cells["WO"].Value.ToString().Length == 0)
        {
          message = "WO";
          return false;
        }
        //Carcasscode
        if (row.Cells["CarcassCode"].Value.ToString().Length == 0)
        {
          message = "CarcassCode";
          return false;
        }
        //ItemCode
        if (row.Cells["ItemCode"].Value.ToString().Length == 0)
        {
          message = "ItemCode";
          return false;
        }
        //Reason
        if (row.Cells["Reason"].Value.ToString().Length == 0)
        {
          message = "Reason";
          return false;
        }
      }
      return true;
    }
    /// <summary>
    /// Check Valid Save
    /// </summary>
    /// <returns></returns>
    private bool CheckValidSaveData(out string message)
    {
      message = string.Empty;
      if (btnGetData.Enabled)
      {
        WindowUtinity.ShowMessageError("MSG0005", "Please Get Data");
        return false;
      }

      return true;
    }
    /// <summary>
    /// HÀM CHECK PROCESS DUPLICATE
    /// </summary>
    private void CheckDuplicate()
    {
      isDuplicateProcess = false;
      for (int k = 0; k < ultData.Rows.Count; k++)
      {
        ultData.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string comcode = ultData.Rows[i].Cells["WO"].Value.ToString();
        string itencode = ultData.Rows[i].Cells["ItemCode"].Value.ToString();
        for (int j = i + 1; j < ultData.Rows.Count; j++)
        {
          string comcodeCompare = ultData.Rows[j].Cells["WO"].Value.ToString();
          string itemcodeCompare = ultData.Rows[j].Cells["ItemCode"].Value.ToString();
          if ((string.Compare(comcode, comcodeCompare, true) == 0) && (string.Compare(itencode, itemcodeCompare, true) == 0))
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
            ultData.Rows[j].CellAppearance.BackColor = Color.Yellow;
            this.isDuplicateProcess = true;
          }
        }
      }
    }
    /// <summary>
    /// Save Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster()
    {
      DBParameter[] input = new DBParameter[4];
      if (this.transactionPid != long.MinValue)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, this.transactionPid);
      }
      input[1] = new DBParameter("@TransactionCode", DbType.AnsiString, 16, txtTransaction.Text);
      input[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      if (txtRemark.Text.Trim().Length > 0)
      {
        input[3] = new DBParameter("@Remark", DbType.String, 500, txtRemark.Text.Trim());
      }
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNReloadBOM_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      this.transactionPid = resultSave;
      if (resultSave == 0)
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
      string stringData = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowParent = ultData.Rows[i];
        stringData += this.transactionPid.ToString() + '^'
                    + rowParent.Cells["WO"].Value.ToString() + '^'
                    + rowParent.Cells["CarcassCode"].Value.ToString() + '^'
                    + rowParent.Cells["ItemCode"].Value.ToString() + '^'
                    + rowParent.Cells["Revision"].Value.ToString() + '^'
                    + rowParent.Cells["Reason"].Value.ToString() + '^'
                    + rowParent.Cells["Remark"].Value.ToString() + '~';

      }

      DBParameter[] input = new DBParameter[1];
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      input[0] = new DBParameter("@StringData", DbType.String, stringData);
      DataBaseAccess.ExecuteStoreProcedure("spPLNReloadBOM_SaveData", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      if (resultSave == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Cast Material & Update Allocated(material, wood, veneer) When Reload
    /// </summary>
    /// <returns></returns>
    private bool CastMaterialWhenReload()
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@TransactionPid", DbType.Int64, this.transactionPid);
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      DataBaseAccess.ExecuteStoreProcedure("spPLNReloadBOM_Update", 1000, input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      if (resultSave <= 0)
      {
        return false;
      }
      return true;
    }

    private void ExportData()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Deadline", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "Deadline";
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

    /// <summary>
    /// Get info data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetData_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // 1: Check Valid
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      //check duplicate
      if (this.isDuplicateProcess == true)
      {
        WindowUtinity.ShowMessageError("ERR0013");
        return;
      }
      if (success)
      {
        string stringData = string.Empty;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow rowParent = ultData.Rows[i];
          stringData += rowParent.Cells["WO"].Value.ToString() + '^'
                      + rowParent.Cells["CarcassCode"].Value.ToString() + '^'
                      + rowParent.Cells["ItemCode"].Value.ToString() + '^'
                      + rowParent.Cells["Revision"].Value.ToString() + '^'
                      + rowParent.Cells["Reason"].Value.ToString() + '^'
                      + rowParent.Cells["Remark"].Value.ToString() + '~';

        }
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@StringData", DbType.String, stringData);
        DataSet dtSource = DataBaseAccess.SearchStoreProcedure("spPLNReloadBOMGetData", 600, input);
        if (dtSource != null)
        {
          dtSource.Relations.Add(new DataRelation("dtParent_dtChildMaterial", new DataColumn[] { dtSource.Tables[0].Columns["WO"], dtSource.Tables[0].Columns["ItemCode"], dtSource.Tables[0].Columns["Revision"] }, new DataColumn[] { dtSource.Tables[1].Columns["WO"], dtSource.Tables[1].Columns["ItemCode"], dtSource.Tables[1].Columns["Revision"] }, false));
          ultData.DataSource = dtSource;
        }
        btnGetData.Enabled = false;
      }
      this.SetStatusControl();
      return;
    }
    /// <summary>
    /// Save data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      btnSave.Enabled = false;
      string message = string.Empty;
      // 1: Check Valid
      bool success = this.CheckValidSaveData(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // 2: Save Data
      success = this.SaveMaster();
      if (success)
      {
        success = this.SaveDetail();
        if (success)
        {
          success = this.CastMaterialWhenReload();
          if (success)
          {
            WindowUtinity.ShowMessageSuccess("MSG0004");
            btnSave.Enabled = false;
          }
          else
          {
            WindowUtinity.ShowMessageError("ERR0037", "Data");
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0037", "Data");
        }
      }
      //Load Data
      this.LoadMainData();
      this.LoadDetailData(this.transactionPid);
    }

    /// <summary>
    /// Init LayOut
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.AutoFitColumns = true;
      ControlUtility.SetPropertiesUltraGrid(ultData);

      e.Layout.Bands[0].Override.CellAppearance.BackColor = Color.LightGray;

      e.Layout.Bands[0].Columns["WO"].ValueList = ultDropWO;
      e.Layout.Bands[0].Columns["CarcassCode"].ValueList = ultDropCarcassCode;
      e.Layout.Bands[0].Columns["ItemCode"].ValueList = ultDropItemCode;
      e.Layout.Bands[0].Columns["Reason"].ValueList = ultDropReason;

      e.Layout.Bands[0].Columns["WO"].MinWidth = 100;
      e.Layout.Bands[0].Columns["WO"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 200;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 200;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Reason"].MinWidth = 200;
      e.Layout.Bands[0].Columns["Reason"].MaxWidth = 200;
      e.Layout.Bands[1].Columns["RemarkStatus"].Header.Caption = "Status";
      e.Layout.Bands[1].Columns["WOQtyOld"].Hidden = true;
      e.Layout.Bands[1].Columns["WOQtyNew"].Hidden = true;

      // Set Align column
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      e.Layout.Bands[1].Columns["WOQtyOld"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["WOQtyNew"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyPerMaterialOld"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyPerMaterialNew"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    ///<summary>
    /// Before Cell Activate
    /// </summary>

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "CarcassCode":
          if (DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) != Int32.MinValue)
          {
            this.LoadCarcassCode(DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
          }
          else
          {
            this.LoadCarcassCode(-1);
          }
          break;
        case "ItemCode":
          if (DBConvert.ParseInt(row.Cells["WO"].Value.ToString()) != Int32.MinValue &&
             row.Cells["CarcassCode"].Value.ToString().Length > 0)
          {
            this.LoadItemCode(DBConvert.ParseInt(row.Cells["WO"].Value.ToString()), row.Cells["CarcassCode"].Value.ToString());
          }
          else
          {
            this.LoadItemCode(-1, "");
          }
          this.CheckDuplicate();
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "WO":
          row.Cells["CarcassCode"].Value = DBNull.Value;
          row.Cells["ItemCode"].Value = DBNull.Value;
          row.Cells["Revision"].Value = DBNull.Value;
          break;
        case "CarcassCode":
          row.Cells["ItemCode"].Value = DBNull.Value;
          row.Cells["Revision"].Value = DBNull.Value;
          break;
        case "ItemCode":
          string commandText = string.Empty;
          commandText = " SELECT DISTINCT ItemCode, Revision";
          commandText += " FROM TblPLNWorkOrderConfirmedDetails";
          commandText += " WHERE WorkOrderPid = " + DBConvert.ParseInt(row.Cells["WO"].Value.ToString());
          commandText += " AND CarcassCode  = '" + row.Cells["CarcassCode"].Value.ToString() + "'";
          commandText += " AND ItemCode  = '" + row.Cells["ItemCode"].Value.ToString() + "'";
          System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtSource != null && dtSource.Rows.Count > 0)
          {
            row.Cells["Revision"].Value = DBConvert.ParseInt(ultDropItemCode.SelectedRow.Cells["Revision"].Value.ToString());
          }
          else
          {
            row.Cells["Revision"].Value = DBNull.Value;
          }
          this.CheckDuplicate();
          break;
        default:
          break;
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportData();
    }

    /// <summary>
    /// Close Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    #endregion Event

  }
}
