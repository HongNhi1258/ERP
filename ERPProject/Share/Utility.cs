using DaiCo.Application;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using PresentationControls;
using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public class Utility
  {

    #region Group code master

    public const int GROUP_CATEGORY = 1;
    public const int GROUP_COLLECTION = 2;
    public const int GROUP_KNOCKDOWN = 3;
    public const int GROUP_CHANGEKIND = 4;
    public const int GROUP_MOREDIMENSION = 5;
    public const int GROUP_COMPONENTSPECIFY = 6;
    public const int GROUP_COMPONENTSTATUS = 7;
    public const int GROUP_MATERIALSTYPE = 8;
    public const int GROUP_COMPONENT_ADJECTIVE = 9;
    public const int GROUP_ADJECTIVE = 10;
    public const int GROUP_GLASS = 11;
    public const int GROUP_GLASSTYPE = 12; //Mirror or Glass
    public const int GROUP_BEVELTYPE = 13;
    public const int GROUP_RDD_GROUP = 14;
    public const int GROUP_EXHIBITION = 16;
    public const int GROUP_ITEMKIND = 17;
    public const int GROUP_ITEM_PATH_FOLDER = 21;
    public const int GROUP_SALEORDERTYPE = 1001;
    public const int GROUP_CONTAINERSTYPE = 1002;
    public const int GROUP_ENQUIRY_LIMIT = 1003;
    public const int GROUP_AREA = 1004;
    public const int GROUP_WO_CHECKPOINT_DEALINE = 1005;
    public const int GROUP_ITEM = 1006;
    public const int GROUP_BILL_OF_LADING = 2001;
    public const int GROUP_CERTIFICATE = 2002;
    public const int GROUP_PACKING = 2003;
    public const int GROUP_INVOICE = 2004;
    public const int GROUP_PAYMENT_TERM = 2005;
    public const int GROUP_PRICE_BASE = 2006;
    public const int GROUP_PRICE_OPTION = 2016;
    public const int GROUP_CURRENCY_SIGN = 2007;
    public const int GROUP_CUSTOMER_KIND = 2008;
    public const int GROUP_PRICELIST_TIME = 2011;
    public const int GROUP_CATALOGUE = 2013;
    public const int GROUP_ENQUIRY_MAX_EXPIRE_DAYS = 2014;
    public const int GROUP_WIPADJUSTMENTTOWPOINT = 3001;
    public const int GROUP_WIPADJUSTMENTREASONIN = 3002;
    public const int GROUP_WIPADJUSTMENTREASONOUT = 3003;
    public const int GROUP_FINISHED_ITEM = 4001;
    public const int GROUP_EXICUSTOME_TYPE = 5001;
    public const int GROUP_DELIVERY_TERM = 5002;
    public const int GROUP_EXI_CURRENCY = 5003;
    public const int GROUP_ROLE = 10001;

    // Add Group General  15/10/2011 START
    public const int GROUP_GNR_PROGRAMMODULE = 9001;
    public const int GROUP_GNR_URGENTLEVEL = 9002;
    public const int GROUP_GNR_TYPE = 9003;
    public const int GROUP_GNR_ITTYPE = 9004;
    public const int GROUP_GNR_PATHFILEUPLOAD = 9005;
    public const int GROUP_GNR_TYPEFILEUPLOAD = 9006;
    public const int GROUP_GNR_TYPEFILEITUPLOAD = 9007;
    // Add Group General  15/10/2011 END

    public const int GROUP_FOUNDY_COMPONENT_KIND = 13001;
    public const int GROUP_FOUNDY_ISCASTING = 13002;
    public const int GROUP_FOUNDY_MISC_REASON = 13003;
    public const int GROUP_COMP_NO_COUNT = 13006;
    public const int GROUP_FOUNDY_WORK_AREA = 13007;
    public const int GROUP_FOUNDRY_COMPONENT_STATUS = 13008;

    #endregion Group code master
    #region Init layout Grid
    public static void InitLayout_UltraGrid(UltraGrid ultGrid)
    {
      ultGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      ultGrid.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
      ultGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      ultGrid.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      ultGrid.DisplayLayout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;
      ultGrid.DisplayLayout.Override.RowSelectorWidth = 32;      
      ultGrid.DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;      
      ultGrid.DisplayLayout.Override.FixedHeaderIndicator = FixedHeaderIndicator.None;
      ultGrid.DisplayLayout.Override.FixedCellSeparatorColor = Color.Transparent;
      ultGrid.DisplayLayout.Override.RowSizing = RowSizing.AutoFree;
      ultGrid.DisplayLayout.PerformAutoResizeColumns(false, PerformAutoSizeType.AllRowsInBand, AutoResizeColumnWidthOptions.All);
      ultGrid.StyleSetName = "Excel2013";
      ultGrid.SyncWithCurrencyManager = false;

      // Set Align
      for (int i = 0; i < ultGrid.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        Type colType = ultGrid.DisplayLayout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ultGrid.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          ultGrid.DisplayLayout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        if (colType == typeof(System.DateTime))
        {
          ultGrid.DisplayLayout.Bands[0].Columns[i].Format = ConstantClass.FORMAT_DATETIME;
        }
      }

    }

    /// <summary>
    /// Format UltraNumericEditor
    /// </summary>
    /// <param name="groupControl"></param>
    public static void Format_UltraNumericEditor(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.GetType().Name == "UltraNumericEditor")
        {
          ((UltraNumericEditor)ctr).MinValue = 0;
          ((UltraNumericEditor)ctr).Nullable = true;
          ((UltraNumericEditor)ctr).NumericType = NumericType.Double;
          if (ctr.Name.Contains("Percent"))
          {
            ((UltraNumericEditor)ctr).FormatString = "##0.##";
            ((UltraNumericEditor)ctr).MaxValue = 100;
            ((UltraNumericEditor)ctr).MaskInput = "{LOC}nnn.nn";
          }
          else if (ctr.Name.Contains("ExchangeRate"))
          {
            ((UltraNumericEditor)ctr).FormatString = "##,##0.##";
            ((UltraNumericEditor)ctr).MaxValue = 1000000;
            ((UltraNumericEditor)ctr).MaskInput = "{LOC}nnn,nnn.nn";
          }
          else
          {
            ((UltraNumericEditor)ctr).FormatString = "###,###,###,##0.##";
            ((UltraNumericEditor)ctr).MaxValue = "999999999999";
            ((UltraNumericEditor)ctr).MaskInput = "{LOC}nnn,nnn,nnn,nnn.nn";
          }
        }
        else
        {
          Format_UltraNumericEditor(ctr);
        }
      }
    }

    public static DataRow[] GetSelectedRow(UltraGrid ugdData)
    {
      DataTable dtSource = (DataTable)ugdData.DataSource;
      foreach (DataRow row in dtSource.Rows)
      {
        if (DBConvert.ParseInt(row["Selected"]) == 1 && row.RowState == DataRowState.Unchanged)
        {
          row.SetModified();
          row["Selected"] = 1;
        }
      }
      return dtSource.Select("Selected = 1");
    }

    /// <summary>
    /// Hide colums base on material group & category. Must have column name MaterialCode on grid
    /// </summary>
    /// <param name="ultGrid"></param>
    public static void HideColumnsOnGrid(UltraGrid ultGrid)
    {
      try
      {
        DataTable dtSource = (DataTable)ultGrid.DataSource;
        if (dtSource != null && dtSource.Rows.Count > 0)
        {
          string materialCode = dtSource.Rows[0]["MaterialCode"].ToString();
          DBParameter[] inputParam = new DBParameter[] { new DBParameter("@MaterialCode", DbType.String, 16, materialCode) };
          DataTable dtHiddenCols = DataBaseAccess.SearchStoreProcedureDataTable("spGNRGetHiddenColsByMaterial", inputParam);
          if (dtHiddenCols != null && dtHiddenCols.Rows.Count > 0)
          {
            string hiddenCols = dtHiddenCols.Rows[0]["HiddenColumnsValue"].ToString().ToLower();
            for (int i = 0; i < ultGrid.DisplayLayout.Bands[0].Columns.Count; i++)
            {
              string colName = string.Format("|{0}|", ultGrid.DisplayLayout.Bands[0].Columns[i].ToString().ToLower());
              if(hiddenCols.Contains(colName))
              {
                ultGrid.DisplayLayout.Bands[0].Columns[i].Hidden = true;
              }                
            }
          }          
        }
      }
      catch { }
    }


    #endregion  Init layout Grid
    #region LoadUltraCombo Data
    public static void LoadUltraDropdownCodeMst(UltraDropDown udrpDropDown, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value, Description FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpDropDown.DataSource = dtSource;
      udrpDropDown.ValueMember = "Code";
      udrpDropDown.DisplayMember = "Value";
      udrpDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Value"].Width = 100;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Description"].Width = 250;
    }
    public static void LoadUltraDropdownCodeMstDefault(UltraDropDown udrpDropDown, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpDropDown.DataSource = dtSource;
      udrpDropDown.ValueMember = "Code";
      udrpDropDown.DisplayMember = "Value";
      udrpDropDown.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      udrpDropDown.DisplayLayout.Bands[0].Columns["Value"].Width = 100;
    }

    public static void LoadComboboxMasterName(ComboBox cmbMaster)
    {
      string commandText = string.Format(@"SELECT [Group], NameEn FROM TblBOMMasterName ORDER BY [Group]");
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbMaster, dtSource, "Group", "NameEn");
    }
    public static void LoadUltraCombo(UltraCombo ucbData, DataTable dtSource, string columnValue, string columnText, params string[] columnHidden)
    {
      ucbData.DataSource = dtSource;
      if (dtSource != null)
      {
        ucbData.ValueMember = columnValue;
        ucbData.DisplayMember = columnText;
        for (int i = 0; i < columnHidden.Length; i++)
        {
          ucbData.DisplayLayout.Bands[0].Columns[columnHidden[i]].Hidden = true;
        }
      }
      ucbData.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      ucbData.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
    }

    public static void LoadUltraCombo(UltraCombo ucbData, DataTable dtSource, string columnValue, string columnText, bool colHeadersVisible, params string[] columnHidden)
    {
      ucbData.DataSource = dtSource;
      if (dtSource != null)
      {
        ucbData.ValueMember = columnValue;
        ucbData.DisplayMember = columnText;
        for (int i = 0; i < columnHidden.Length; i++)
        {
          ucbData.DisplayLayout.Bands[0].Columns[columnHidden[i]].Hidden = true;
        }
      }
      ucbData.DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      ucbData.DisplayLayout.Bands[0].ColHeadersVisible = colHeadersVisible;
      ucbData.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      ucbData.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
      ucbData.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
    }

    public static void LoadUltraDropDown(UltraDropDown ultraDD, DataTable dtSource, string columnValue, string columnText, params string[] columnHidden)
    {
      ultraDD.DataSource = dtSource;
      if (dtSource != null)
      {
        ultraDD.ValueMember = columnValue;
        ultraDD.DisplayMember = columnText;
        for (int i = 0; i < columnHidden.Length; i++)
        {
          ultraDD.DisplayLayout.Bands[0].Columns[columnHidden[i]].Hidden = true;
        }
      }
    }

    public static void LoadUltraDropDown(UltraDropDown ultraDD, DataTable dtSource, string columnValue, string columnText, bool colHeadersVisible, params string[] columnHidden)
    {
      ultraDD.DataSource = dtSource;
      if (dtSource != null)
      {
        ultraDD.ValueMember = columnValue;
        ultraDD.DisplayMember = columnText;
        for (int i = 0; i < columnHidden.Length; i++)
        {
          ultraDD.DisplayLayout.Bands[0].Columns[columnHidden[i]].Hidden = true;
        }
      }
      ultraDD.DisplayLayout.Bands[0].ColHeadersVisible = colHeadersVisible;
    }

    public static void LoadUltraCBMasterData(UltraCombo ucb, int group)
    {
      string commandText = string.Format("SELECT Code, Name FROM TblHRDDBMasterDetail WHERE GroupPid = {0} AND IsDeleted = 0 ORDER BY SortBy", group);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucb, dtSource, "Code", "Name", false, "Code");
    }
    #endregion LoadUltraCombo Data

    /// <summary>
    /// Workbook return is not closed. Please close it before viewing for user.
    /// Example: xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
    /// </summary>
    /// <param name="ultraGridSource"></param>
    /// <param name="pathOutputFile"></param>
    /// <param name="startRow"></param>
    /// <returns></returns>
    public static void ExportToExcel(UltraGrid ultraGridSource, out Excel.Workbook xlBook, string pathOutputFile, params int[] startRow)
    {
      Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter excelExport = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
      excelExport.Export(ultraGridSource, pathOutputFile);

      object missing = System.Reflection.Missing.Value;
      Excel.Application excelApp = new Excel.Application();
      xlBook = excelApp.Workbooks.Open(pathOutputFile, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
      Excel.Sheets xlSheets = xlBook.Worksheets;
      Excel.Worksheet xlSheet = (Excel.Worksheet)xlSheets.get_Item(1);

      Excel.Range rng = (Excel.Range)xlSheet.Cells[1, 1];
      Excel.Range row = rng.EntireRow;
      // Add new row
      if (startRow.Length > 0)
      {
        for (int i = 1; i < startRow[0]; i++)
        {
          row.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);
        }
      }
      //xlBook.Close(true, missing, missing);      
    }
    /// <summary>
    /// Default Path: StartupPath\Report\
    /// </summary>
    /// <param name="ultraGridSource"></param>
    /// <param name="pathOutputFile"></param>
    public static void ExportToExcelWithDefaultPath(UltraGrid ultraGridSource, string fileName)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report\", startupPath);
      if (!Directory.Exists(pathOutputFile))
      {
        Directory.CreateDirectory(pathOutputFile);
      }
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, fileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter excelExport = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
      excelExport.Export(ultraGridSource, strOutFileName);
      Process.Start(strOutFileName);
    }

    internal static void ExportToExcelWithDefaultPath(object ultraGridInformation, string v)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Default Path: StartupPath\Report\
    /// </summary>
    /// <param name="ultraGridSource"></param>
    /// <param name="pathOutputFile"></param>
    public static void ExportToExcelWithDefaultPath(UltraGrid ultraGridSource, string fileName, int startRow)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report\", startupPath);
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, fileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      ExportToExcel(ultraGridSource, strOutFileName, startRow);
    }

    private static void ExportToExcel(UltraGrid ultraGridSource, string strOutFileName, int startRow)
    {
      throw new NotImplementedException();
    }

    /// <summary>    
    /// Workbook return is not closed. Please close it before viewing for user.
    /// Example: xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
    /// </summary>
    /// <param name="ultraGridSource"></param>
    /// <param name="fileName"></param>
    /// <param name="startRow"></param>
    /// <returns></returns>
    public static void ExportToExcelWithDefaultPath(UltraGrid ultraGridSource, out Excel.Workbook xlBook, string fileName, params int[] startRow)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report\", startupPath);
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, fileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      ExportToExcel(ultraGridSource, out xlBook, strOutFileName, startRow);
    }
    public static void LoadDropDownBOMGroupProcess(UltraCombo cmbGroupProcess)
    {
      string commandText = string.Format(@"SELECT Pid, GroupProcessNameVN, GroupProcessName, (GroupProcessNameVN + ' | ' + GroupProcessName) DisplayText FROM TblBOMGroupProcess WHERE DeleteFlag = 0 Order By GroupProcessNameVN");
      DataTable dtGropProcess = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(cmbGroupProcess, dtGropProcess, "Pid", "DisplayText", false, new string[] { "Pid", "DisplayText" });
    }
    public static void LoadDropdownProfile(UltraDropDown udrpDropDown)
    {
      string commandText = "SELECT Pid, ProfileCode, Description, DescriptionVN FROM TblBOMProfile";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraDropDown(udrpDropDown, dtSource, "Pid", "ProfileCode", false, "Pid");
      udrpDropDown.DisplayLayout.Bands[0].Columns["Description"].Width = 200;
      udrpDropDown.DisplayLayout.Bands[0].Columns["DescriptionVN"].Width = 200;
    }
    public static void LoadComboboxPrefix(UltraCombo UltraCBPrefix, int group)
    {
      DataTable dtPrefix = DataBaseAccess.SearchCommandTextDataTable(string.Format(@"Select PrefixCode From TblBOMPrefix Where [Group] = {0} ORDER BY No", group));
      LoadUltraCombo(UltraCBPrefix, dtPrefix, "PrefixCode", "PrefixCode");
    }
    public static void SynchronizeFinishing(string itemCode, int revision)
    {
      //synchronize finishing level 1 and level 2
      // Delete and Copy new finishing and new other finishing 
      string storename = "spBOMCopyOtherFinishing";
      DBParameter[] inputParamFinCopy = new DBParameter[4];
      inputParamFinCopy[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParamFinCopy[1] = new DBParameter("@Revision", DbType.Int32, revision);
      inputParamFinCopy[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParamFinCopy[3] = new DBParameter("@CompGroup", DbType.Int32, Shared.Utility.ConstantClass.COMP_FINISHING);

      DBParameter[] outputParamFinCopy = new DBParameter[] { new DBParameter("@Result", DbType.Int32, int.MinValue) };
      DataBaseAccess.ExecuteStoreProcedure(storename, inputParamFinCopy, outputParamFinCopy);
    }
    public static void LoadUltraDropdownFormular(UltraDropDown udrpDropDown)
    {
      string commandText = string.Format(@"SELECT Pid, PaintCode, PaintName, FaceQty, ShortEdgeQty, LongEdgeQty FROM TblBOMPaintFormula");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraDropDown(udrpDropDown, dtSource, "Pid", "PaintCode", false, "Pid");
      udrpDropDown.DisplayLayout.Bands[0].Columns["PaintCode"].Width = 50;
      udrpDropDown.DisplayLayout.Bands[0].Columns["PaintName"].Width = 100;
    }

    public static void LoadUltraComboFormula(UltraCombo ucbFormula)
    {
      string commandText = string.Format(@"SELECT Pid, PaintCode, PaintName, FaceQty, ShortEdgeQty, LongEdgeQty FROM TblBOMPaintFormula");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ucbFormula, dtSource, "Pid", "PaintCode", false, "Pid");
      ucbFormula.DisplayLayout.Bands[0].Columns["PaintCode"].Width = 50;
      ucbFormula.DisplayLayout.Bands[0].Columns["PaintName"].Width = 100;
    }

    public static string GetImagePathByPid(int pid)
    {
      string commandText = string.Format("SELECT Path FROM TBLBOMImagePath WHERE Pid = '{0}'", pid);
      return DataBaseAccess.ExecuteScalarCommandText(commandText).ToString();
    }

    public static string BOMGetCarcassImage(string carcassCode)
    {
      string fileName = string.Format(@"{0}\{1}.wmf", GetImagePathByPid(3), carcassCode);
      if (File.Exists(fileName))
      {
        return fileName;
      }
      return string.Format(@"{0}\{1}.jpg", GetImagePathByPid(3), carcassCode);
    }

    /// <summary>
    /// Show Image of Item on the grid data. Notice: the columns name of grid must be "ItemCode" and "Revision"
    /// </summary>
    /// <param name="ultGridData"></param>
    /// <param name="pictureCarcass"></param>
    /// <param name="showImage"></param>
    /// <returns></returns>
    public static void BOMShowCarcassImage(UltraGrid ultGridData, GroupBox groupCarcassImage, PictureBox pictureItem, bool showImage)
    {
      try
      {
        if (showImage)
        {
          UltraGridRow row = ultGridData.Selected.Rows[0];
          string carcassCode = row.Cells["CarcassCode"].Value.ToString();
          if (carcassCode.Length > 0)
          {
            groupCarcassImage.Text = string.Format("Carcass: {0}", carcassCode);
            pictureItem.ImageLocation = BOMGetCarcassImage(carcassCode);
            Point xy = new Point();
            int yMax = ultGridData.Location.Y + ultGridData.Height;
            xy.X = ultGridData.Location.X + row.Cells["CarcassCode"].Width;
            xy.Y = ultGridData.Location.Y + (row.Cells["CarcassCode"].Height * (row.Index + 2));
            if (xy.Y + groupCarcassImage.Height > yMax)
            {
              xy.Y = yMax - groupCarcassImage.Height;
            }
            groupCarcassImage.Location = xy;
            groupCarcassImage.Visible = true;
          }
          else
          {
            groupCarcassImage.Text = string.Empty;
          }
        }
        else
        {
          groupCarcassImage.Visible = false;
        }
      }
      catch
      {
        groupCarcassImage.Text = string.Empty;
      }
    }

    #region Load data carcass
    public static void LoadDataCarcassComponentStruct(TreeView treeViewComponentStruct, string carcassCode)
    {
      treeViewComponentStruct.Nodes.Clear();
      string commandTextRootComp = string.Format(@"Select Pid, ComponentCode, DescriptionVN, Qty From TblBOMCarcassComponent Where CarcassCode = '{0}' And IsMainComp = 1", carcassCode);
      DataTable dtRootComp = DataBaseAccess.SearchCommandTextDataTable(commandTextRootComp);
      string commandTextSubComp = string.Format(@"SELECT STRUCT.MainCompPid, STRUCT.SubCompPid, SUBCOMP.ComponentCode SubCompCode, 
                                                  SUBCOMP.DescriptionVN SubCompDescription, STRUCT.Qty
                                               FROM TblBOMCarcassComponentStruct STRUCT
  	                                              INNER JOIN TblBOMCarcassComponent SUBCOMP ON SUBCOMP.Pid = STRUCT.SubCompPid AND SUBCOMP.CarcassCode = '{0}' ORDER BY STRUCT.No ASC", carcassCode);
      DataTable dtSubComp = DataBaseAccess.SearchCommandTextDataTable(commandTextSubComp);
      foreach (DataRow rootRow in dtRootComp.Rows)
      {
        TreeNode node = new TreeNode();
        node.Name = rootRow["Pid"].ToString();
        node.Text = string.Format("{0} - {1}, qty: {2}", rootRow["ComponentCode"], rootRow["DescriptionVN"], rootRow["Qty"]);
        treeViewComponentStruct.Nodes.Add(node);
        LoadDataCarcassSubComponent(node, dtSubComp);
      }
    }

    public static void LoadDataCarcassSubComponent(TreeNode mainNode, DataTable dtSubComp)
    {
      long pidMainComp = DBConvert.ParseLong(mainNode.Name.ToString());
      DataRow[] subRows = dtSubComp.Select(string.Format("MainCompPid = {0}", pidMainComp));
      for (int i = 0; i < subRows.Length; i++)
      {
        TreeNode subNode = new TreeNode();
        subNode.Name = subRows[i]["SubCompPid"].ToString();
        subNode.Text = string.Format("{0} - {1}, qty: {2}", subRows[i]["SubCompCode"], subRows[i]["SubCompDescription"], subRows[i]["Qty"]);
        mainNode.Nodes.Add(subNode);
        LoadDataCarcassSubComponent(subNode, dtSubComp);
      }
    }
    #endregion Load data carcass

    public static void LoadUltCBNation(UltraCombo ultCBNation)
    {
      string commandText = "SELECT Pid, NationEN FROM TblCSDNation";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCBNation, dt, "Pid", "NationEN");
    }

    /// <summary>
    /// Load Combobox nation
    /// </summary>
    public static void LoadNation(ComboBox cmbNation)
    {
      string commandText = "SELECT Pid, NationEN FROM TblCSDNation";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ControlUtility.LoadCombobox(cmbNation, dt, "Pid", "NationEN");
    }
    /// <summary>
    /// Load Data To UltraCombo Department
    /// </summary>
    public static void LoadUltraComboDepartment(UltraCombo ultraCBDepartment)
    {
      string commandText = "SELECT Department, DeparmentName, Department + ' | ' + DeparmentName Display FROM VHRDDepartment ORDER BY Department";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBDepartment, dtSource, "Department", "Display", "Display");
    }

    /// <summary>
    /// Load Data To UltraCombo Employee
    /// </summary>
    public static void LoadUltraComboEmployee(UltraCombo ultraCBEmployee, string department)
    {
      string commandText = string.Empty;
      if (department.Length > 0)
      {
        commandText = string.Format("Select Pid, EmpName, Cast(Pid as varchar) + ' | ' + EmpName Display From VHRMEmployee Where Department = '{0}'", department);
        ultraCBEmployee.Text = string.Empty;
      }
      else
      {
        commandText = "Select Pid, EmpName, Cast(Pid as varchar) + ' - ' + EmpName Display From VHRMEmployee";
      }
      if (commandText.Length > 0)
      {
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        LoadUltraCombo(ultraCBEmployee, dtSource, "Pid", "Display", "Display");
      }
    }

    /// <summary>
    /// Load data ultra combobox category from Customer service
    /// </summary>
    /// <param name="cmbCategory"></param>
    public static void LoadUltraCBCategory(UltraCombo ultCBCategory)
    {
      string commandText = @"SELECT CH.Pid Code, PR.USCateCode + ' - ' + PR.Category + ' | ' + CH.Category Value
                            FROM
                            (
                                SELECT Pid, USCateCode, Category
                                FROM TblCSDCategory
                                WHERE ParentPid IS NULL
                            )PR
                            LEFT JOIN 
                            (
	                            SELECT Pid, Category, ParentPid
	                            FROM TblCSDCategory
	                            WHERE ParentPid IS NOT NULL
                            )CH ON PR.Pid = CH.ParentPid
                            WHERE CH.Pid IS NOT NULL
                            ORDER BY PR.USCateCode";
      DataTable dtCategory = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultCBCategory, dtCategory, "Code", "Value", "Code");
    }
    /// <summary>
    /// Load Customer
    /// </summary>
    public static void LoadCustomer(UltraCombo ucbCustomer)
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE DeletedFlg = 0 AND ParentPid IS NULL ORDER BY CustomerCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCustomer, dtSource, "Pid", "Customer", "Pid");
    }
    /// <summary>
    /// Load data ultra combo collection from Customer service
    /// </summary>
    /// <param name="cmbCategory"></param>
    public static void LoadUltraCBCollection(UltraCombo ultCBCollection)
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Value", Utility.GROUP_COLLECTION);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultCBCollection, dtSource, "Code", "Value", false, "Code");
    }

    public static void LoadUltraComboCodeMst(UltraCombo ultCombo, int group, params int[] kind)
    {
      string commandText = string.Format(@"SELECT Code, Value, Description
                                           FROM TblBOMCodeMaster 
                                           WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      if (kind.Length > 0)
      {
        commandText = string.Format(commandText + " AND (Kind = {0})", kind[0]);
      }
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultCombo, dtSource, "Code", "Value", false, "Code");
      ultCombo.DisplayLayout.Bands[0].Columns["Value"].Width = 100;
      ultCombo.DisplayLayout.Bands[0].Columns["Description"].Width = 250;
    }

    public static void LoadComboboxCodeMst(ComboBox cmbCodeMst, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbCodeMst, dtSource, "Code", "Value");
    }

    public static void LoadComboboxUnit(ComboBox cmbUnit)
    {
      string commandText = string.Format("SELECT UnitPid FROM TblBOMUnit ORDER BY UnitPid");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbUnit, dtSource, "UnitPid", "UnitPid");
    }

    public static void LoadCombobox(ComboBox cmb, DataTable dtSoure, string columnValue, string columnText)
    {
      if (dtSoure != null)
      {
        DataTable dt = dtSoure.Clone();
        dt.Merge(dtSoure);
        DataRow row = dt.NewRow();
        dt.Rows.InsertAt(row, 0);
        cmb.DataSource = dt;
        cmb.DisplayMember = columnText;
        cmb.ValueMember = columnValue;
      }
    }

    public static void LoadUltraCBCarcass(UltraCombo ultCBCarcass)
    {
      string commandText = "Select CarcassCode, Description, (CarcassCode + ' | ' + Description) DescCarcass From TblBOMCarcass Where DeleteFlag = 0 Order By CarcassCode Desc";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultCBCarcass, dtSource, "CarcassCode", "DescCarcass", false, "DescCarcass");
    }

    public static void LoadUltraComboboxFinishingStyle(UltraCombo ultCmbFinishingStyle, bool filterBrassType)
    {
      string commandText = string.Format(@"SELECT FinCode Code, Name, NameVN, FinCode + ' - ' + ISNULL(Name, '')  DisplayMember 
                                           FROM TblBOMFinishingInfo
                                           WHERE DeleteFlag = 0");
      if (filterBrassType)
      {
        commandText = string.Format("{0}  AND BrassStyle = 1", commandText);
      }
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultCmbFinishingStyle, dtSource, "Code", "DisplayMember", false, "DisplayMember");
      ultCmbFinishingStyle.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 100;
      ultCmbFinishingStyle.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 100;
    }

    public static void LoadUltraDropdownFinishsingCode(UltraDropDown udrpFinishingCode, string condition)
    {
      string commandText = string.Format(@"SELECT FinCode, Name as FinName FROM TblBOMFinishingInfo WHERE {0}", condition);
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpFinishingCode.DataSource = dataSource;
      udrpFinishingCode.DisplayLayout.Bands[0].Columns["FinCode"].Width = 100;
      udrpFinishingCode.DisplayLayout.Bands[0].Columns["FinName"].Width = 250;
    }

    public static void LoadUltraComboFinishsingCode(UltraCombo ultraCBFinishingCode, string condition)
    {
      string commandText = string.Format(@"SELECT FinCode, Name as FinName, (FinCode + ' | ' + Name) DisplayMember FROM TblBOMFinishingInfo WHERE {0}", condition);
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBFinishingCode, dataSource, "FinCode", "DisplayMember", false, "DisplayMember");
      ultraCBFinishingCode.Size = new System.Drawing.Size(300, 22);
      ultraCBFinishingCode.DisplayLayout.Bands[0].Columns["FinCode"].Width = 80;
      ultraCBFinishingCode.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    public static void LoadOtherMaterials(UltraCombo ultraCBOthermaterials)
    {
      string commandText = string.Format(@"SELECT Code, (Value + CASE WHEN LEN(ISNULL(Description, '')) > 0 THEN (' - ' + Description) ELSE '' END) Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Value", Shared.Utility.ConstantClass.GROUP_MATERIALSTYPE);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBOthermaterials, dtSource, "Code", "Value", "Code");
      ultraCBOthermaterials.DisplayLayout.Bands[0].Columns["Value"].Header.Caption = "Material";

      //Add an additional unbound column to WinCombo.
      //This will be used for the Selection of each Item
      UltraGridColumn checkedCol = ultraCBOthermaterials.DisplayLayout.Bands[0].Columns.Add();
      checkedCol.Key = "Selected";
      checkedCol.Header.Caption = string.Empty;

      //This allows end users to select / unselect ALL items
      checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;
      checkedCol.DataType = typeof(bool);

      //Move the checkbox column to the first position.
      checkedCol.Header.VisiblePosition = 0;

      ultraCBOthermaterials.CheckedListSettings.CheckStateMember = "Selected";
      ultraCBOthermaterials.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems;
      // Set up the control to use a custom list delimiter
      ultraCBOthermaterials.CheckedListSettings.ListSeparator = ", ";
      // Set ItemCheckArea to Item, so that clicking directly on an item also checks the item
      ultraCBOthermaterials.CheckedListSettings.ItemCheckArea = Infragistics.Win.ItemCheckArea.Item;
    }
    /// <summary>
    /// Get value of checked data from ultraCombo
    /// </summary>
    /// <param name="ultraCBOthermaterials"></param>
    public static string GetCheckedValueUltraCombobox(UltraCombo cmb)
    {
      string checkedData = string.Empty;
      foreach (UltraGridRow r in cmb.CheckedRows)
      {
        if (checkedData.Length > 0)
        {
          checkedData += "|";
        }
        checkedData += r.Cells[cmb.ValueMember].Value.ToString();
      }
      return checkedData;
    }

    /// <summary>
    /// Show Image of Item on the grid data. Notice: the columns name of grid must be "ItemCode" and "Revision"
    /// </summary>
    /// <param name="ultGridData"></param>
    /// <param name="pictureItem"></param>
    /// <param name="showImage"></param>
    /// <returns></returns>
    public static void BOMShowItemImage(UltraGrid ultGridData, GroupBox groupItemImage, PictureBox pictureItem, bool showImage)
    {
      try
      {
        if (showImage)
        {
          UltraGridRow row = ultGridData.Selected.Rows[0];
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          if (itemCode.Length > 0 && revision != int.MinValue)
          {
            groupItemImage.Text = string.Format("Item: {0}, Revision: {1}", itemCode, revision);
            pictureItem.ImageLocation = Utility.BOMGetItemImage(itemCode, revision);
            Point xy = new Point();
            int yMax = ultGridData.Location.Y + ultGridData.Height;
            xy.X = ultGridData.Location.X + row.Cells["ItemCode"].Width + row.Cells["Revision"].Width;
            xy.Y = ultGridData.Location.Y + (row.Cells["ItemCode"].Height * (row.Index + 2));
            if (xy.Y + groupItemImage.Height > yMax)
            {
              xy.Y = yMax - groupItemImage.Height;
            }
            groupItemImage.Location = xy;
            groupItemImage.Visible = true;
          }
          else
          {
            groupItemImage.Text = string.Empty;
          }
        }
        else
        {
          groupItemImage.Visible = false;
        }
      }
      catch
      {
        groupItemImage.Text = string.Empty;
      }
    }

    public static void LoadCheckBoxComboBox(CheckBoxComboBox chkCB, DataTable dtSoure, string value, string text)
    {
      chkCB.Items.Clear();
      chkCB.CheckBoxItems.Clear();
      chkCB.Clear();
      int i = 1;
      foreach (DataRow row in dtSoure.Rows)
      {
        chkCB.Items.Add(row[value]);
        chkCB.CheckBoxItems[i].Text = row[text].ToString();
        i++;
      }
    }

    public static void SetPropertiesUltraGrid(UltraGrid ultraGridData)
    {
      ultraGridData.DisplayLayout.ScrollStyle = ScrollStyle.Immediate;
      ultraGridData.DisplayLayout.Override.HeaderClickAction = HeaderClickAction.SortMulti;
    }

    public static string BOMGetItemImage(string itemCode, int revision)
    {
      string imagePath;
      if (revision == 0)
      {
        imagePath = string.Format(@"{0}\{1}.jpg", GetImagePathByPid(4), itemCode);
      }
      else
      {
        imagePath = string.Format(@"{0}\{1}-{2}.jpg", GetImagePathByPid(1), itemCode, revision.ToString().PadLeft(2, '0'));
      }
      return imagePath;
    }


    /// <summary>
    /// Get Data From Excel File
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="nSheet"></param>
    /// <param name="areaData"></param>
    /// <returns></returns>
    public static DataTable GetDataFromExcel(string fileName, string sheetName, string areaData)
    {
      try
      {
        OleDbConnection connection = new OleDbConnection("provider=Microsoft.Jet.OLEDB.4.0;data source=" + fileName + ";Extended Properties=Excel 8.0;");
        string commandText = string.Format(@"SELECT * FROM [{0}${1}]", sheetName, areaData);
        OleDbDataAdapter adp = new OleDbDataAdapter(commandText, connection);
        System.Data.DataTable dtXLS = new System.Data.DataTable();
        adp.Fill(dtXLS);
        connection.Close();
        if (dtXLS == null)
        {
          return null;
        }
        return dtXLS;
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0069");
        return null;
      }
    }

    public static string GetSelectedValueCombobox(ComboBox cmb)
    {
      string value = string.Empty;
      try
      {
        value = cmb.SelectedValue.ToString();
      }
      catch { }
      return value;
    }

    public static void LoadCustomer(ComboBox cmb)
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE DeletedFlg = 0 AND ParentPid IS NULL ORDER BY CustomerCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmb, dtSource, "Pid", "Customer");
    }

    public static string GetSelectedValueUltraCombobox(UltraCombo cmb)
    {
      string value = string.Empty;
      try
      {
        if (cmb.SelectedRow != null)
        {
          value = cmb.Value.ToString();
        }
      }
      catch { }
      return value;
    }

    public static void BOMShowGroupboxOnGrid(UltraGrid ultGridData, string colName, int rowIndex, GroupBox groupDetail, bool show)
    {
      try
      {
        if (show)
        {
          UltraGridRow row = ultGridData.Rows[rowIndex];
          Point xy = new Point();
          int xMax = ultGridData.Location.X + ultGridData.Width;
          int yMax = ultGridData.Location.Y + ultGridData.Height;
          int colLocationX = 0;
          for (int i = 0; i <= row.Cells[colName].Column.Index; i++)
          {
            colLocationX += row.Cells[i].Column.Width;
          }
          xy.X = ultGridData.Location.X + colLocationX;
          xy.Y = ultGridData.Location.Y + (row.Cells[colName].Height * (row.Index + 2));
          if (xy.Y + groupDetail.Height > yMax)
          {
            xy.Y = yMax - groupDetail.Height;
          }
          if (xy.X + groupDetail.Width > xMax)
          {
            xy.X = xMax - groupDetail.Width;
          }
          groupDetail.Location = xy;
          groupDetail.Visible = true;
        }
        else
        {
          groupDetail.Visible = false;
        }
      }
      catch
      {
        groupDetail.Text = string.Empty;
      }
    }

    public static void GetDataForClipboard(UltraGrid ultraGridData)
    {
      string newLine = System.Environment.NewLine;
      string tab = "\t";
      string clipboard_string = "";
      if (ultraGridData.Selected.Rows.Count > 0)
      {
        // Get Caption
        for (int i = 0; i < ultraGridData.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          if (!ultraGridData.DisplayLayout.Bands[0].Columns[i].Hidden)
          {
            string header = ultraGridData.DisplayLayout.Bands[0].Columns[i].Header.Caption.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            clipboard_string = string.Format("{0}{1}{2}", clipboard_string, header, tab);
          }
        }

        // Get Data
        for (int nRow = 0; nRow < ultraGridData.Selected.Rows.Count; nRow++)
        {
          clipboard_string = string.Format("{0}{1}", clipboard_string, newLine);
          Infragistics.Win.UltraWinGrid.UltraGridRow row = ultraGridData.Selected.Rows[nRow];
          for (int i = 0; i < row.Cells.Count; i++)
          {
            if (!row.Cells[i].Column.Hidden)
            {
              clipboard_string = string.Format("{0}{1}{2}", clipboard_string, row.Cells[i].Text, tab);
            }
          }
        }
      }
      else if (ultraGridData.Selected.Columns.Count > 0)
      {
        // Get Caption
        for (int i = 0; i < ultraGridData.Selected.Columns.Count; i++)
        {
          string header = ultraGridData.Selected.Columns[i].Caption.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
          clipboard_string = string.Format("{0}{1}{2}", clipboard_string, header, tab);
        }

        // Get Data
        for (int nRow = 0; nRow < ultraGridData.Rows.Count; nRow++)
        {
          Infragistics.Win.UltraWinGrid.UltraGridRow row = ultraGridData.Rows[nRow];
          if (row.IsFilteredOut == false)
          {
            clipboard_string = string.Format("{0}{1}", clipboard_string, newLine);

            for (int i = 0; i < ultraGridData.Selected.Columns.Count; i++)
            {
              int colIndex = ultraGridData.Selected.Columns[i].Column.Index;
              clipboard_string = string.Format("{0}{1}{2}", clipboard_string, row.Cells[colIndex].Text, tab);
            }
          }
        }
      }
      Clipboard.SetText(clipboard_string);
    }
    public static void LoadEmployee(ComboBox cmb, string deparment)
    {
      string whereClause = (deparment.Length > 0) ? string.Format(@"Where Department = '{0}'", deparment) : string.Empty;
      string commandText = string.Format(@" SELECT Pid, dbo.FSYSPadLeft(CAST(Pid as varchar), '0', 4) + ' - ' + EmpName EmpName FROM VHRMEmployee {0} ORDER BY Pid", whereClause);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmb, dtSource, "Pid", "EmpName");
    }

    public static void LoadDirectCustomer(ComboBox cmb, long parentCustomerPid)
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE ParentPid = {0}", parentCustomerPid);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmb, dtSource, "Pid", "Customer");
    }

    public static void ExportToOpenOfficeCalc(UltraGrid ultraGridSource, string fileName)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report\", startupPath);
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, fileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter excelExport = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
      excelExport.Export(ultraGridSource, strOutFileName);
      Process.Start(strOutFileName);
    }

    public static void SetCheckedValueUltraCombobox(UltraCombo ultraCB, string checkedValue)
    {
      checkedValue = string.Format("|{0}|", checkedValue);
      string valueMember = ultraCB.ValueMember;
      string checkedColumn = ultraCB.CheckedListSettings.CheckStateMember;
      for (int i = 0; i < ultraCB.Rows.Count; i++)
      {
        ultraCB.Rows[i].Cells[checkedColumn].Value = false;
        if (checkedValue.IndexOf(string.Format("|{0}|", ultraCB.Rows[i].Cells[valueMember].Value.ToString())) >= 0)
        {
          ultraCB.Rows[i].Cells[checkedColumn].Value = true;
        }
      }
    }

    public static void LoadUltraCBExhibition(UltraCombo ultraCBExhibition)
    {
      string commandText = string.Format("Select Code, Value From TblBOMCodeMaster Where [Group] = {0} Order By Sort", ConstantClass.GROUP_EXHIBITION);
      DataTable dtExhibition = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBExhibition, dtExhibition, "Code", "Value", false);
    }

    public static void LoadItemImageFolder(UltraCombo ultraCB)
    {
      string commandText = string.Format(@"SELECT Value, Description
                                           FROM TblBOMCodeMaster
                                           WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", ConstantClass.GROUP_ITEM_PATH_FOLDER);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCB, dtSource, "Value", "Description", false, "Value");
    }

    public static void LoadMultiCombobox(MultiColumnComboBox multiCB, DataTable dtSoure, string columnValue, string columnText)
    {
      if (dtSoure != null)
      {
        DataTable dt = dtSoure.Clone();
        dt.Merge(dtSoure);
        DataRow row = dt.NewRow();
        dt.Rows.InsertAt(row, 0);
        multiCB.DataSource = dt;
        multiCB.DisplayMember = columnText;
        multiCB.ValueMember = columnValue;
        multiCB.ColumnWidths = "100, 200";
      }
    }

    /// <summary>
    /// Load ultraComboCustomer for item (distribute & Pid > 3)
    /// </summary>
    public static void LoadUltraCBCustomer(UltraCombo ultraCBCustomer)
    {
      string commandText = @"Select Pid, CustomerCode Code, Name, (CustomerCode + ' - ' + Name) Display 
                            From TblCSDCustomerInfo 
                            Where ParentPid Is Null And Pid > 3
                            Order By CustomerCode";
      DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBCustomer, dtCustomer, "Pid", "Display", false, new string[] { "Pid", "Display" });
    }

    /// <summary>
    /// Load JC, OEM Customer List
    /// </summary>
    /// <param name="ultraCBCustomer"></param>
    public static void LoadUltraCBJC_OEM_Customer(UltraCombo ultraCBCustomer)
    {
      string commandText = @"SELECT	Pid, CustomerCode, CustomerCode + ' - ' + Name Display



                             FROM	TblCSDCustomerInfo
                             WHERE	ParentPid IS NULL AND (Pid = 27 OR Kind = 5) ORDER BY CustomerCode";
      DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBCustomer, dtCustomer, "Pid", "Display", false, new string[] { "Pid", "CustomerCode" });
    }

    public static void LoadDropdownFinishsingCode(MultiColumnComboBox cmbFinishingCode, string condition)
    {
      string commandText = string.Format(@"SELECT FinCode, Name as FinName, (FinCode + ' | ' + Name) as FinCodeName  
                                           FROM TblBOMFinishingInfo 
                                           WHERE {0}", condition);
      DataTable dataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dataSource.NewRow();
      dataSource.Rows.InsertAt(row, 0);
      cmbFinishingCode.DataSource = dataSource;
      cmbFinishingCode.ValueMember = "FinCode";
      cmbFinishingCode.DisplayMember = "FinCodeName";
    }

    public static string GetSelectedValueMultiCombobox(MultiColumnComboBox multiCB)
    {
      string value = string.Empty;
      try
      {
        value = multiCB.SelectedValue.ToString();
      }
      catch { }
      return value;
    }

    public static string GetValueCheckBoxComboBox(CheckBoxComboBox chkCB)
    {
      string checkedValue = string.Empty;
      for (int i = 0; i < chkCB.Items.Count; i++)
      {
        if (chkCB.CheckBoxItems[i].Checked)
        {
          if (checkedValue != string.Empty)
          {
            checkedValue += "|";
          }
          checkedValue += chkCB.Items[i].ToString();
        }
      }
      return checkedValue;
    }

    public static void LoadItemComboBox(MultiColumnComboBox multiCBItem)
    {
      string commandText = "SELECT ItemCode, Name as ItemName, (ItemCode + ' | ' + Name) as ItemCodeName FROM TblBOMItemBasic";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow row = dtSource.NewRow();
      dtSource.Rows.InsertAt(row, 0);
      multiCBItem.DataSource = dtSource;
      multiCBItem.ValueMember = "ItemCode";
      multiCBItem.DisplayMember = "ItemCodeName";
      multiCBItem.ColumnWidths = "100, 200, 0";
    }

    public static void LoadItemComboBox(UltraCombo ultraCBItem)
    {
      string commandText = "SELECT ItemCode, Name as ItemName, (ItemCode + ' | ' + Name) as ItemCodeName FROM TblBOMItemBasic";
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBItem, dtSource, "ItemCode", "ItemCodeName", "ItemCodeName");
    }

    public static void LoadRevisionByItemCode(ComboBox cmbRevision, string itemCode)
    {
      string commandText = string.Format("Select Revision From TblBOMRevision Where ItemCode = '{0}'", itemCode);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadCombobox(cmbRevision, dtSource, "Revision", "Revision");
    }

    public static void LoadRevisionByItemCode(UltraCombo ultraCBRevision, string itemCode)
    {
      string commandText = string.Format("Select Revision From TblBOMRevision Where ItemCode = '{0}'", itemCode);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      LoadUltraCombo(ultraCBRevision, dtSource, "Revision", "Revision");
    }

    public static DialogResult PromptDeleteMessage(int countRows)
    {
      string message = (countRows > 1) ? "rows" : "row";
      message = string.Format("You have selected {0} {1} for deletion.\n Choose Yes to delete the {1} or No to exit.", countRows, message);
      return MessageBox.Show(message, "Delete Rows", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    }

    public static void ViewCrystalReport(CrystalDecisions.CrystalReports.Engine.ReportClass cpt)
    {
      SaveFileDialog f = new SaveFileDialog();
      f.Filter = "Report files (*.rpt)|*.rpt";
      if (f.ShowDialog() == DialogResult.OK)
      {
        string strName = f.FileName;
      GoBack:;
        try
        {
          File.Open(f.FileName, FileMode.OpenOrCreate).Close();
        }
        catch
        {
          MessageBox.Show("Already Opened:" + f.FileName + "\nPlease find and close it", "Can not save!");
          goto GoBack;
        }
        cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.CrystalReport, strName);
        System.Diagnostics.Process.Start(@strName);
      }
    }
    /// <summary>
    /// Convert a number in Milimet to Inch
    /// </summary>
    /// <param name="value">Number in milimet</param>
    /// <returns>Number in Inch</returns>
    public static string ConverMilimetToInch(double value)
    {
      if (value == int.MinValue)
      {
        return string.Empty;
      }
      double result = value * 0.0393700787;
      int integerPart = (int)result;
      double decimalPart = result - integerPart;
      if (decimalPart < 0.125)
      {
        return DBConvert.ParseString(integerPart) + "''";
      }
      else if (decimalPart < 0.375)
      {
        return DBConvert.ParseString(integerPart) + " " + "1/4''";
      }
      else if (decimalPart < 0.635)
      {
        return DBConvert.ParseString(integerPart) + " " + "1/2''";
      }
      else if (decimalPart < 0.875)
      {
        return DBConvert.ParseString(integerPart) + " " + "3/4''";
      }
      return DBConvert.ParseString(integerPart + 1) + "''";
    }

    public static DataTable CloneTable(DataTable dtSource)
    {
      DataTable dtCopy = new DataTable();
      dtCopy = dtSource.Clone();
      dtCopy.Merge(dtSource);
      return dtCopy;
    }

    /// <summary>
    /// Account Post Transaction
    /// </summary>
    /// <param name="DocTypePid">Loại chức năng</param>
    /// <param name="DocumentPid">id của phiếu</param>
    /// <param name="IsPosted"></param>
    /// <param name="CreateBy"></param>    
    /// <returns></returns>
    public static bool ACCPostTransaction(int docTypePid, long documentPid, bool isPosted, int createBy)
    {
      bool success = false;
      SqlDBParameter[] inputParam = new SqlDBParameter[4];
      inputParam[0] = new SqlDBParameter("@DocTypePid", SqlDbType.Int, docTypePid);
      inputParam[1] = new SqlDBParameter("@DocumentPid", SqlDbType.BigInt, documentPid);
      inputParam[2] = new SqlDBParameter("@IsPosted", SqlDbType.Bit, isPosted);
      inputParam[3] = new SqlDBParameter("@CreateBy", SqlDbType.Int, createBy);
      SqlDBParameter[] outputParam = new SqlDBParameter[] { new SqlDBParameter("@Result", SqlDbType.Int, 0) };

      SqlDataBaseAccess.ExecuteStoreProcedure("spACCPostTransaction", inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseInt(outputParam[0].Value.ToString()) > 0)
      {
        success = true;
      }
      return success;
    }

    public static void LoadUltraDepartment(UltraCombo drpDepartment, bool isAddNewRow)
    {
      string newRow = isAddNewRow ? "SELECT NULL Code, NULL Name UNION" : string.Empty;
      string commandText = string.Format(@"{0} SELECT Code, Name 
                                          FROM VHRDDepartmentInfo DEP
	                                            INNER JOIN VHRMEmployee EMP ON (DEP.Code = EMP.Department)
                                          GROUP BY Code, Name 
                                          ORDER BY Name", newRow);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      drpDepartment.DataSource = dtSource;
      drpDepartment.ValueMember = "Code";
      drpDepartment.DisplayMember = "Name";
      drpDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      drpDepartment.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 50;
      drpDepartment.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 50;
      drpDepartment.DisplayLayout.Bands[0].Columns["Name"].MinWidth = 200;
      drpDepartment.DisplayLayout.Bands[0].Columns["Name"].MaxWidth = 200;
    }
    public static void LoadUltraEmployeeByDeparment(UltraCombo drpEmployee, string department, bool isAddNewRow)
    {
      string whereClause = (department.Length > 0) ? string.Format(@"Where Department = '{0}'", department) : string.Empty;
      string newRow = isAddNewRow ? "SELECT NULL Pid, NULL EmpName UNION" : string.Empty;
      string commandText = string.Format(@" {0} SELECT Pid, EmpName FROM VHRMEmployee {1} ORDER BY Pid", newRow, whereClause);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      drpEmployee.DataSource = dtSource;
      drpEmployee.ValueMember = "Pid";
      drpEmployee.DisplayMember = "EmpName";
      drpEmployee.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    public static void LoadUltraMaterialCode(UltraCombo drpMaterialCode, int warehouse)
    {
      string commandText = string.Format(@"SELECT MaterialCode Code, MaterialNameEn EnglishName, IsControl, ControlType,
                                                MaterialCode + ' - ' + MaterialNameEn DisplayName
                                           FROM VBOMMaterials
                                           WHERE Warehouse = {0}
                                           ORDER BY MaterialCode", warehouse);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      drpMaterialCode.DataSource = dtSource;
      drpMaterialCode.ValueMember = "Code";
      drpMaterialCode.DisplayMember = "DisplayName";
      drpMaterialCode.DisplayLayout.Bands[0].Columns["IsControl"].Hidden = true;
      drpMaterialCode.DisplayLayout.Bands[0].Columns["ControlType"].Hidden = true;
      drpMaterialCode.DisplayLayout.Bands[0].Columns["DisplayName"].Hidden = true;
      drpMaterialCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    public static void LoadUltraSupplier(UltraCombo drpSupplier, bool isAddNewRow)
    {
      string newRow = isAddNewRow ? "SELECT NULL Code, NULL Name UNION" : string.Empty;
      string commandText = string.Format(@"{0} SELECT SupplierCode Code, EnglishName Name
                                          FROM VPURSupplierInfo_PMISDB
                                          WHERE EnglishName IS NOT NULL AND REPLACE(EnglishName, ' ', '') <> ''
                                          ORDER BY Name", newRow);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      drpSupplier.DataSource = dtSource;
      drpSupplier.ValueMember = "Code";
      drpSupplier.DisplayMember = "Name";
      drpSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      drpSupplier.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 80;
      drpSupplier.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 80;
    }

    public static void LoadUltraCBSupplier(UltraCombo ucb)
    {
      string commandText = "SELECT Pid, SupplierCode, EnglishName SupplierName, (SupplierCode + ' | ' + EnglishName) DisplayText FROM TblPURSupplierInfo WHERE DeleteFlg = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucb, dtSource, "Pid", "DisplayText", false, new string[] { "Pid", "DisplayText" });
      ucb.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    public static void LoadUltraCBMaterialWHListByUser(UltraCombo ucb)
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@EmpID", DbType.Int32, SharedObject.UserInfo.UserPid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHDMaterialWHListByUser", inputParam);
      Utility.LoadUltraCombo(ucb, dtSource, "Pid", "DisplayText", false, new string[] { "Pid", "DisplayText" });
      ucb.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    public static void LoadUltraCBMaterialWHList(UltraCombo ucb)
    {
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable("SELECT Pid, Code, Name, DisplayText FROM VWHDWarehouseList");
      Utility.LoadUltraCombo(ucb, dtSource, "Pid", "DisplayText", false, new string[] { "Pid", "DisplayText" });
      ucb.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    public static void LoadUltraCBMaterialListByWHPid(UltraCombo ucb, int whPid)
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@WHPid", DbType.Int32, whPid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHDMaterialListByWHPid", inputParam);
      Utility.LoadUltraCombo(ucb, dtSource, "MaterialCode", "DisplayText", false, "DisplayText");
      ucb.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    public static void LoadUltraCBLocationListByWHPid(UltraCombo ucb, int whPid)
    {
      string commandText = string.Format(@"SELECT Pid, Name + ' - ' + ISNULL(Remark, '') AS Location 
                                          FROM TblWHDPosition WHERE WarehousePid = {0} ORDER BY Location", whPid);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucb, dtSource, "Pid", "Location", false, "Pid");
    }

    public static void LoadUltraCBAccountList(UltraCombo ucb)
    {
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(
        @"SELECT DISTINCT MAIN.Pid, MAIN.AccountCode, MAIN.AccountName, (CASE WHEN CHILD.Pid IS NULL THEN 1 ELSE 0 END) IsLeaf
          FROM TblACCAccount MAIN
            LEFT JOIN TblACCAccount CHILD ON MAIN.Pid = CHILD.ParentPid
          WHERE MAIN.IsActive = 1
          ORDER BY MAIN.AccountCode");
      Utility.LoadUltraCombo(ucb, dtSource, "Pid", "AccountCode", false, new string[] { "Pid", "IsLeaf" });
      ucb.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    /// <summary>
    /// Check Warehouse Summary PreMonth && PreYear
    /// </summary>
    /// <returns></returns>
    public static bool CheckWHPreMonthSummary(int warehousehPid)
    {
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@WarehousePid", DbType.Int32, warehousehPid);
      DBParameter[] outputParam = new DBParameter[3];
      outputParam[0] = new DBParameter("@IsClosed", DbType.Int32, 0);
      outputParam[1] = new DBParameter("@PreMonth", DbType.Int32, 0);
      outputParam[2] = new DBParameter("@PreYear", DbType.Int32, 0);

      DataBaseAccess.ExecuteStoreProcedure("spWHDCheckPreMonthClosing", inputParam, outputParam);
      if (DBConvert.ParseInt(outputParam[0].Value) == 0) //Pre month is not yet closed
      {
        WindowUtinity.ShowMessageError("ERR0303", outputParam[1].Value.ToString(), outputParam[2].Value.ToString());
        return false;
      }
      return true;
    }
  }
}

