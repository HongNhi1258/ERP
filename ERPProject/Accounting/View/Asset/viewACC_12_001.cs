/*
  Author      : Nguyen Thanh Binh
  Date        : 03/06/2021
  Description : Asset
  Standard Form: view_ExtraControl.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_12_001 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(view_ExtraControl).Assembly);
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCAsset_Init");
      Utility.LoadUltraCombo(ucbUsedEmployee, dsInit.Tables[0], "EmployeePid", "EmployeeName", false, "EmployeePid");
      Utility.LoadUltraCombo(ucbDepartment, dsInit.Tables[1], "DepartmentPid", "DeparmentName", false, "DepartmentPid");
      Utility.LoadUltraCombo(ucbUnit, dsInit.Tables[2], "UnitPid", "Unit", false, "UnitPid");
      Utility.LoadUltraCombo(ucbAccount, dsInit.Tables[3], "AccountPid", "AccountCode", true, "AccountPid");
      ucbAccount.DisplayLayout.Bands[0].Columns["AccountCode"].Header.Caption = "Mã tài khoản";
      ucbAccount.DisplayLayout.Bands[0].Columns["AccountName"].Header.Caption = "Tên tài khoản";
      Utility.LoadUltraCombo(ucbACCost, dsInit.Tables[3], "AccountPid", "AccountCode", true, "AccountPid");
      ucbACCost.DisplayLayout.Bands[0].Columns["AccountCode"].Header.Caption = "Mã tài khoản";
      ucbACCost.DisplayLayout.Bands[0].Columns["AccountName"].Header.Caption = "Tên tài khoản";
      Utility.LoadUltraCombo(ucbACDepreciation, dsInit.Tables[3], "AccountPid", "AccountCode", true, "AccountPid");
      ucbACDepreciation.DisplayLayout.Bands[0].Columns["AccountCode"].Header.Caption = "Mã tài khoản";
      ucbACDepreciation.DisplayLayout.Bands[0].Columns["AccountName"].Header.Caption = "Tên tài khoản";
      Utility.LoadUltraCombo(ucbMaterialCode, dsInit.Tables[4], "MaterialCode", "MaterialName", true, "UnitPid");
      ucbMaterialCode.DisplayLayout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã SP";
      ucbMaterialCode.DisplayLayout.Bands[0].Columns["MaterialName"].Header.Caption = "Mô tả";
      Utility.LoadUltraCombo(ucbCostCenter, dsInit.Tables[5], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbSegment, dsInit.Tables[6], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbUnCostConstruction, dsInit.Tables[7], "Value", "Display", false, "Value");
      Utility.LoadUltraCombo(ucbStatus, dsInit.Tables[8], "StatusCode", "StatusName", false, "StatusCode");
      Utility.LoadUltraCombo(ucbDepreciationMethod, dsInit.Tables[9], "MethodCode", "MethodName", false, "MethodCode");
      Utility.LoadUltraCombo(ucbAssetType, dsInit.Tables[10], "Pid", "GroupName", false, new string[] { "Pid", "AccountPid", "ACDepreciationPid" });

      // Set Language
      this.SetLanguage();
    }

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

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        txtAssetCode.Text = dtMain.Rows[0]["AssetCode"].ToString();
        txtAssetName.Text = dtMain.Rows[0]["AssetName"].ToString();
        txtAssetDesc.Text = dtMain.Rows[0]["AssetDesc"].ToString();
        if (DBConvert.ParseInt(dtMain.Rows[0]["AssetType"].ToString()) > 0)
        {
          ucbAssetType.Value = DBConvert.ParseInt(dtMain.Rows[0]["AssetType"].ToString());
        }
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["PurchasedDate"]) != DateTime.MinValue)
        {
          udtPurchasedDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["PurchasedDate"]);
        }
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["UsedDate"]) != DateTime.MinValue)
        {
          udtUsedDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["UsedDate"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["UsedEmployeePid"]) > 0)
        {
          ucbUsedEmployee.Value = DBConvert.ParseInt(dtMain.Rows[0]["UsedEmployeePid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["DepartmentPid"]) > 0)
        {
          ucbDepartment.Value = DBConvert.ParseInt(dtMain.Rows[0]["DepartmentPid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["UnitPid"]) > 0)
        {
          ucbUnit.Value = DBConvert.ParseInt(dtMain.Rows[0]["UnitPid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["AccountPid"]) > 0)
        {
          ucbAccount.Value = DBConvert.ParseInt(dtMain.Rows[0]["AccountPid"]);
        }
        else
        {
          ucbAccount.Value = null;
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["ACCostPid"]) > 0)
        {
          ucbACCost.Value = DBConvert.ParseInt(dtMain.Rows[0]["ACCostPid"]);
        }
        else
        {
          ucbACCost.Value = null;
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["ACDepreciationPid"]) > 0)
        {
          ucbACDepreciation.Value = DBConvert.ParseInt(dtMain.Rows[0]["ACDepreciationPid"]);
        }
        else
        {
          ucbACDepreciation.Value = null;
        }
        if (dtMain.Rows[0]["MaterialCode"].ToString().Length > 0)
        {
          ucbMaterialCode.Value = dtMain.Rows[0]["MaterialCode"].ToString();
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["CostCenterPid"]) > 0)
        {
          ucbCostCenter.Value = DBConvert.ParseInt(dtMain.Rows[0]["CostCenterPid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["SegmentPid"]) > 0)
        {
          ucbSegment.Value = DBConvert.ParseInt(dtMain.Rows[0]["SegmentPid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["UnCostConstructionPid"]) > 0)
        {
          ucbUnCostConstruction.Value = DBConvert.ParseInt(dtMain.Rows[0]["UnCostConstructionPid"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["Status"]) > 0)
        {
          ucbStatus.Value = DBConvert.ParseInt(dtMain.Rows[0]["Status"]);
          if (DBConvert.ParseInt(dtMain.Rows[0]["Status"]) != 1)
          {
            ucbStatus.ReadOnly = true;
          }
        }
        if (DBConvert.ParseDouble(dtMain.Rows[0]["OriginalAmount"]) > 0)
        {
          uneOriginalAmount.Value = DBConvert.ParseDouble(dtMain.Rows[0]["OriginalAmount"]);
        }
        if (DBConvert.ParseDouble(dtMain.Rows[0]["DepreciationAmount"]) > 0)
        {
          uneDepreciationAmount.Value = DBConvert.ParseDouble(dtMain.Rows[0]["DepreciationAmount"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["DepreciationMonth"]) > 0)
        {
          uneDepreciationMonth.Value = DBConvert.ParseInt(dtMain.Rows[0]["DepreciationMonth"]);
        }
        if (DBConvert.ParseInt(dtMain.Rows[0]["DepreciationMethod"].ToString()) > 0)
        {
          ucbDepreciationMethod.Value = DBConvert.ParseInt(dtMain.Rows[0]["DepreciationMethod"].ToString());
        }
        if (DBConvert.ParseDateTime(dtMain.Rows[0]["DepreciationDate"]) != DateTime.MinValue)
        {
          udtDepreciationDate.Value = DBConvert.ParseDateTime(dtMain.Rows[0]["DepreciationDate"]);
        }
        if (DBConvert.ParseDouble(dtMain.Rows[0]["DepreciationPercent"].ToString()) > 0)
        {
          uneDepreciationPercent.Value = DBConvert.ParseDouble(dtMain.Rows[0]["DepreciationPercent"]);
        }
        if (DBConvert.ParseDouble(dtMain.Rows[0]["InitDepreciatedAmount"]) > 0)
        {
          uneInitDepreciatedAmount.Value = DBConvert.ParseDouble(dtMain.Rows[0]["InitDepreciatedAmount"]);
        }
        if (DBConvert.ParseDouble(dtMain.Rows[0]["RemainedAmount"]) > 0)
        {
          uneRemainedAmount.Value = DBConvert.ParseDouble(dtMain.Rows[0]["RemainedAmount"]);
        }
        if (DBConvert.ParseDouble(dtMain.Rows[0]["AccumulatedDepAmount"]) > 0)
        {
          uneAccumulatedDepAmount.Value = DBConvert.ParseDouble(dtMain.Rows[0]["AccumulatedDepAmount"]);
        }
        uneInitQty.Value = DBConvert.ParseDouble(dtMain.Rows[0]["InitQty"]);
      }
      else
      {
        string cmd = string.Format(@"SELECT dbo.FACCAssetNo() AssetCode");
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
        if (dt.Rows.Count > 0)
        {
          txtAssetCode.Text = dt.Rows[0][0].ToString();
        }
      }
    }

    private void LoadData()
    {
      //spACCAsset_Load
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, this.viewPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spACCAsset_Load", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 0)
      {
        this.LoadMainData(dsSource.Tables[0]);
      }

      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      if (txtAssetName.Text.Length == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Tên tài sản không được bỏ trống.");
        return false;
      }

      if (DBConvert.ParseDateTime(udtUsedDate.Value) == DateTime.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Ngày sử dụng không được để trống.");
        return false;
      }

      if (DBConvert.ParseInt(ucbUnit.Value) == int.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Đơn vị tính không được bỏ trống.");
        return false;
      }

      if (DBConvert.ParseInt(ucbAccount.Value) == int.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Tài khoản tài sản không được bỏ trống.");
        return false;
      }

      if (DBConvert.ParseInt(ucbACCost.Value) == int.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Tài khoản chi phí không được bỏ trống.");
        return false;
      }

      if (DBConvert.ParseInt(ucbACDepreciation.Value) == int.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Tài khoản khấu hao không được bỏ trống.");
        return false;
      }

      if (DBConvert.ParseInt(ucbStatus.Value) == int.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Trạng thái không được bỏ trống.");
        return false;
      }

      if (DBConvert.ParseDouble(uneOriginalAmount.Value) == double.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Nguyên giá không được bỏ trống.");
        return false;
      }

      if (DBConvert.ParseInt(ucbDepreciationMethod.Value) == int.MinValue)
      {
        WindowUtinity.ShowMessageErrorFromText("Phương pháp khấu hao không được bỏ trống.");
        return false;
      }

      if (DBConvert.ParseDouble(uneDepreciationAmount.Value) > DBConvert.ParseDouble(uneOriginalAmount.Value))
      {
        WindowUtinity.ShowMessageErrorFromText("Giá trị khấu hao không được lớn hơn nguyên giá.");
        return false;
      }
      return true;
    }

    private bool SaveMain()
    {
      SqlDBParameter[] inputParam = new SqlDBParameter[28];
      if (this.viewPid != long.MinValue)
      {
        inputParam[0] = new SqlDBParameter("@Pid", SqlDbType.BigInt, this.viewPid);
      }

      if (txtAssetName.Text.Trim().Length > 0)
      {
        inputParam[1] = new SqlDBParameter("@AssetName", SqlDbType.NVarChar, txtAssetName.Text.Trim().ToString());
      }

      if (txtAssetDesc.Text.Trim().Length > 0)
      {
        inputParam[2] = new SqlDBParameter("@AssetDesc", SqlDbType.NVarChar, txtAssetDesc.Text.Trim().ToString());
      }

      if (DBConvert.ParseInt(ucbAssetType.Value) != int.MinValue)
      {
        inputParam[3] = new SqlDBParameter("@AssetType", SqlDbType.Int, DBConvert.ParseInt(ucbAssetType.Value));
      }

      inputParam[4] = new SqlDBParameter("@PurchasedDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtPurchasedDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));

      inputParam[5] = new SqlDBParameter("@UsedDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtUsedDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));

      if (DBConvert.ParseInt(ucbUsedEmployee.Value) != int.MinValue)
      {
        inputParam[6] = new SqlDBParameter("@UsedEmployeePid", SqlDbType.Int, DBConvert.ParseInt(ucbUsedEmployee.Value));
      }

      if (DBConvert.ParseInt(ucbDepartment.Value) != int.MinValue)
      {
        inputParam[7] = new SqlDBParameter("@DepartmentPid", SqlDbType.Int, DBConvert.ParseInt(ucbDepartment.Value));
      }

      if (DBConvert.ParseInt(ucbUnit.Value) != int.MinValue)
      {
        inputParam[8] = new SqlDBParameter("@UnitPid", SqlDbType.Int, DBConvert.ParseInt(ucbUnit.Value));
      }

      if (DBConvert.ParseInt(ucbAccount.Value) != int.MinValue)
      {
        inputParam[9] = new SqlDBParameter("@AccountPid", SqlDbType.Int, DBConvert.ParseInt(ucbAccount.Value));
      }

      if (DBConvert.ParseInt(ucbACCost.Value) != int.MinValue)
      {
        inputParam[10] = new SqlDBParameter("@ACCostPid", SqlDbType.Int, DBConvert.ParseInt(ucbACCost.Value));
      }

      if (DBConvert.ParseInt(ucbACDepreciation.Value) != int.MinValue)
      {
        inputParam[11] = new SqlDBParameter("@ACDepreciationPid", SqlDbType.Int, DBConvert.ParseInt(ucbACDepreciation.Value));
      }

      if (ucbMaterialCode.Value != null)
      {
        inputParam[12] = new SqlDBParameter("@MaterialCode", SqlDbType.VarChar, ucbMaterialCode.Value);
      }

      if (DBConvert.ParseInt(ucbCostCenter.Value) != int.MinValue)
      {
        inputParam[13] = new SqlDBParameter("@CostCenterPid", SqlDbType.Int, DBConvert.ParseInt(ucbCostCenter.Value));
      }

      if (DBConvert.ParseInt(ucbSegment.Value) != int.MinValue)
      {
        inputParam[14] = new SqlDBParameter("@SegmentPid", SqlDbType.Int, DBConvert.ParseInt(ucbSegment.Value));
      }

      if (DBConvert.ParseInt(ucbUnCostConstruction.Value) != int.MinValue)
      {
        inputParam[15] = new SqlDBParameter("@UnCostConstructionPid", SqlDbType.Int, DBConvert.ParseInt(ucbUnCostConstruction.Value));
      }

      if (DBConvert.ParseInt(ucbStatus.Value) != int.MinValue)
      {
        inputParam[16] = new SqlDBParameter("@Status", SqlDbType.Int, DBConvert.ParseInt(ucbStatus.Value));
      }

      if (DBConvert.ParseDouble(uneOriginalAmount.Value) >= 0)
      {
        inputParam[17] = new SqlDBParameter("@OriginalAmount", SqlDbType.Float, DBConvert.ParseDouble(uneOriginalAmount.Value));
      }

      if (DBConvert.ParseInt(ucbDepreciationMethod.Value) != int.MinValue)
      {
        inputParam[18] = new SqlDBParameter("@DepreciationMethod", SqlDbType.Int, DBConvert.ParseInt(ucbDepreciationMethod.Value));
      }

      if (DBConvert.ParseInt(uneDepreciationMonth.Value) >= 0)
      {
        inputParam[19] = new SqlDBParameter("@DepreciationMonth", SqlDbType.Int, DBConvert.ParseInt(uneDepreciationMonth.Value));
      }

      inputParam[20] = new SqlDBParameter("@DepreciationDate", SqlDbType.DateTime, DBConvert.ParseDateTime(udtDepreciationDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME));

      if (DBConvert.ParseDouble(uneDepreciationPercent.Value) >= 0)
      {
        inputParam[21] = new SqlDBParameter("@DepreciationPercent", SqlDbType.Float, DBConvert.ParseDouble(uneDepreciationPercent.Value));
      }


      inputParam[22] = new SqlDBParameter("@InitQty", SqlDbType.Int, DBConvert.ParseInt(uneInitQty.Value));


      if (DBConvert.ParseDouble(uneDepreciationAmount.Value) >= 0)
      {
        inputParam[23] = new SqlDBParameter("@DepreciationAmount", SqlDbType.Float, DBConvert.ParseDouble(uneDepreciationAmount.Value));
      }

      if (DBConvert.ParseDouble(uneInitDepreciatedAmount.Value) >= 0)
      {
        inputParam[24] = new SqlDBParameter("@InitDepreciatedAmount", SqlDbType.Float, DBConvert.ParseDouble(uneInitDepreciatedAmount.Value));
      }

      if (DBConvert.ParseDouble(uneRemainedAmount.Value) >= 0)
      {
        inputParam[25] = new SqlDBParameter("@RemainedAmount", SqlDbType.Float, DBConvert.ParseDouble(uneRemainedAmount.Value));
      }

      if (DBConvert.ParseDouble(uneAccumulatedDepAmount.Value) >= 0)
      {
        inputParam[26] = new SqlDBParameter("@AccumulatedDepAmount", SqlDbType.Float, DBConvert.ParseDouble(uneAccumulatedDepAmount.Value));
      }

      inputParam[27] = new SqlDBParameter("@EditBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);


      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.BigInt, 0) };

      SqlDataBaseAccess.ExecuteStoreProcedure("spACCAsset_Save", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.viewPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }
      return false;
    }

    private void SaveData()
    {
      if (this.CheckValid())
      {
        bool success = true;
        success = this.SaveMain();
        // 2. Insert/Update      
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
        this.SaveSuccess = false;
      }
    }

    private void FillDataForGrid(DataTable dtItemList)
    {
      //DataTable dtItemCheck = (DataTable)ultraDDItemList.DataSource;
      //DataTable dtItemSource = (DataTable)ultData.DataSource;
      //foreach (DataRow row in dtItemList.Rows)
      //{
      //  string itemCode = row[0].ToString().Trim();
      //  if (itemCode.Length > 0)
      //  {
      //    DataRow rowItemSource = dtItemSource.NewRow();
      //    rowItemSource["ItemCode"] = itemCode;
      //    DataRow[] rows = dtItemCheck.Select(string.Format("ItemCode = '{0}'", itemCode));
      //    if (rows.Length == 0)
      //    {
      //      rowItemSource["StatusText"] = "Invalid";
      //      rowItemSource["StatusValue"] = 0;
      //    }
      //    else
      //    {
      //      rowItemSource["ItemName"] = rows[0]["ItemName"];
      //      rowItemSource["ActiveRevision"] = rows[0]["ActiveRevision"];
      //      rowItemSource["StatusText"] = rows[0]["StatusText"];
      //      rowItemSource["StatusValue"] = rows[0]["StatusValue"];
      //      rowItemSource["RevisionPid"] = rows[0]["RevisionPid"];
      //    }
      //    dtItemSource.Rows.Add(rowItemSource);
      //  }
      //}
    }

    /// <summary>
    /// Set Auto Add 4 blank before text of button
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetBlankForTextOfButton(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count > 0)
        {
          this.SetBlankForTextOfButton(ctr);
        }
        else if (ctr.GetType().Name == "Button")
        {
          ctr.Text = string.Format("{0}{1}", "    ", ctr.Text);
        }
      }
    }
    private void SetLanguage()
    {
      //btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);      

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewACC_12_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_12_001_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
      this.LoadData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }


    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ugdInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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

    private void ugdInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void ugdInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtAssetCode.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtAssetCode.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtAssetCode.Text.Trim(), "SELECT * FROM [Items (1)$E3:E4]");
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
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtAssetCode.Text.Trim(), string.Format("SELECT * FROM [Items (1)$B5:B{0}]", itemCount));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.FillDataForGrid(dsItemList.Tables[0]);
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      //string templateName = "ItemListTemplate";
      //string sheetName = "Items";
      //string outFileName = "Items Template";
      //string startupPath = System.Windows.Forms.Application.StartupPath;
      //string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      //string pathTemplate = startupPath + @"\ExcelTemplate\Technical";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      //oXlsReport.Out.File(outFileName);
      //Process.Start(outFileName);
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      //Utility.ExportToExcelWithDefaultPath(ugdInformation, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdInformation);
      }
    }

    private void ucbAssetType_ValueChanged(object sender, EventArgs e)
    {
      if (ucbAssetType.SelectedRow != null)
      {
        ucbAccount.Value = DBConvert.ParseInt(ucbAssetType.SelectedRow.Cells["AccountPid"].Value);
        ucbACDepreciation.Value = DBConvert.ParseInt(ucbAssetType.SelectedRow.Cells["ACDepreciationPid"].Value);
        ucbACCost.Value = DBConvert.ParseInt(ucbAssetType.SelectedRow.Cells["ACCostPid"].Value);
      }
    }

    private void ucbMaterialCode_ValueChanged(object sender, EventArgs e)
    {
      if (ucbMaterialCode.SelectedRow != null)
      {
        ucbUnit.Value = DBConvert.ParseInt(ucbMaterialCode.SelectedRow.Cells["UnitPid"].Value);
      }
    }
    #endregion event
  }
}
