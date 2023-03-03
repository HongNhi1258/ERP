/*
 * Author       : 
 * CreateDate   : 11/01/2013
 * Description  : Update JCDL
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using VBReport;
using System.Diagnostics;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_01_013 : MainUserControl
  {
    #region Init
    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    public viewCSD_01_013()
    {
      InitializeComponent();
    }

    private void viewPUR_01_012_Load(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";
    }
    #endregion Init

    #region Load Data
    private void Search()
    {
      string commandText = string.Empty;
      commandText += " SELECT  ROW_NUMBER() OVER (ORDER BY JCDL.JCItem) [No], ";
      commandText += " 	  NAT.NationEN, JCDL.JCItem, JCDL.JCOH, JCDL.JCTRN, JCDL.JCVORD, JCDL.JCNYOR, ";
      commandText += "    JCDL.JCHIS6, JCDL.JCHIST, JCDL.JCHI24, JCDL.JCLINV, JCDL.OriginalJCTRN  ";
      commandText += " FROM TblPLNMasterPlanJCDL JCDL ";
      commandText += " 	  LEFT JOIN TblCSDNation NAT ON NAT.Pid = JCDL.NationPid";
      commandText += " WHERE JCDL.JCItem LIKE '%" + this.txtSaleCode.Text + "%'";
      commandText += " ORDER BY JCDL.JCItem  ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      this.ultData.DataSource = dtSource;
    }

    private void ExportExcel()
    {
      ControlUtility.ExportToExcelWithDefaultPath(this.ultData, "JCDL");
      //string strTemplateName = "RPT_CSD_01_013_002";
      //string strSheetName = "Sheet1";
      //string strOutFileName = "JCDL Report";
      //string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      //DataTable dtData = (DataTable)ultData.DataSource;

      //if (dtData != null && dtData.Rows.Count > 0)
      //{
      //  for (int i = 0; i < dtData.Rows.Count; i++)
      //  {
      //    DataRow dtRow = dtData.Rows[i];
      //    if (i > 0)
      //    {
      //      oXlsReport.Cell("B8:L8").Copy();
      //      oXlsReport.RowInsert(7 + i);
      //      oXlsReport.Cell("B8:L8", 0, i).Paste();
      //    }
      //    oXlsReport.Cell("**No", 0, i).Value = DBConvert.ParseInt(dtRow["No"].ToString());
      //    oXlsReport.Cell("**JCItem", 0, i).Value = dtRow["JCItem"].ToString();
      //    oXlsReport.Cell("**Nation", 0, i).Value = dtRow["NationEN"].ToString();
      //    if (DBConvert.ParseInt(dtRow["JCOH"].ToString()) != int.MinValue)
      //    {
      //      oXlsReport.Cell("**JCOH", 0, i).Value = DBConvert.ParseInt(dtRow["JCOH"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**JCOH", 0, i).Value = "";
      //    }
      //    if (DBConvert.ParseInt(dtRow["JCTRN"].ToString()) != int.MinValue)
      //    {
      //      oXlsReport.Cell("**JCTRN", 0, i).Value = DBConvert.ParseInt(dtRow["JCTRN"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**JCTRN", 0, i).Value = "";
      //    }
      //    if (DBConvert.ParseInt(dtRow["JCVORD"].ToString()) != int.MinValue)
      //    {
      //      oXlsReport.Cell("**JCVORD", 0, i).Value = DBConvert.ParseInt(dtRow["JCVORD"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**JCVORD", 0, i).Value = "";
      //    }
      //    if (DBConvert.ParseInt(dtRow["JCNYOR"].ToString()) != int.MinValue)
      //    {
      //      oXlsReport.Cell("**JCNYOR", 0, i).Value = DBConvert.ParseInt(dtRow["JCNYOR"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**JCNYOR", 0, i).Value = "";
      //    }
      //    if (DBConvert.ParseInt(dtRow["JCHIS6"].ToString()) != int.MinValue)
      //    {
      //      oXlsReport.Cell("**JCHIS6", 0, i).Value = DBConvert.ParseInt(dtRow["JCHIS6"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**JCHIS6", 0, i).Value = "";
      //    }
      //    if (DBConvert.ParseInt(dtRow["JCHIST"].ToString()) != int.MinValue)
      //    {
      //      oXlsReport.Cell("**JCHIST", 0, i).Value = DBConvert.ParseInt(dtRow["JCHIST"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**JCHIST", 0, i).Value = "";
      //    }
      //    if (DBConvert.ParseInt(dtRow["JCHI24"].ToString()) != int.MinValue)
      //    {
      //      oXlsReport.Cell("**JCHI24", 0, i).Value = DBConvert.ParseInt(dtRow["JCHI24"].ToString());
      //    }
      //    else
      //    {
      //      oXlsReport.Cell("**JCHI24", 0, i).Value = "";
      //    }
      //    oXlsReport.Cell("**JCLINV", 0, i).Value = dtRow["JCLINV"].ToString();
      //  }
      //}
      //oXlsReport.Out.File(strOutFileName);
      //Process.Start(strOutFileName);
    }
    #endregion

    #region Save Data
    #endregion

    #region Others Events

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
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

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      btnExportExcel.Enabled = false;
      this.ExportExcel();
      btnExportExcel.Enabled = true;
    }
    #endregion

    #region UltraGrid Events

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JCOH"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JCTRN"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JCVORD"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JCNYOR"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JCHIS6"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JCHIST"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["JCHI24"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OriginalJCTRN"].CellAppearance.TextHAlign = HAlign.Right;
      
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Alpha.Transparent;
      e.Layout.Override.CellAppearance.BackColorAlpha = Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Alpha.UseAlphaLevel;
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = this.pathExport;
      dialog.Title = "Select a Excel file";
      txtImport.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtImport.Text.Trim().Length > 0);
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "RPT_CSD_01_013_001";
      string sheetName = "jcdl";
      string outFileName = "RPT_CSD_01_013_001";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check nation USA(1: USA, 0: Not USA)
      int flagNationUSA = 0;

      if (this.txtImport.Text.Trim().Length == 0)
      {
        return;
      }

      try
      {
        DataTable dtSource = new DataTable();
        try
        {
          dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImport.Text.Trim(), "SELECT * FROM [jcdl (1)$A1:K5000]").Tables[0]; 
        }
        catch
        {
          dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImport.Text.Trim(), "SELECT * FROM [jcdl$A1:K5000]").Tables[0];
        }
        
        if (dtSource == null)
        {
          return;
        }

        // (Check Data Import)
        string message = string.Empty;
        bool success = this.CheckDataImport(out message, dtSource);
        if(!success)
        {
          WindowUtinity.ShowMessageError("ERR0001", message);
          return;
        }
        // (End Check)

        foreach (DataRow row in dtSource.Rows)
        {
          if (row["JCITEM"].ToString().Trim().Length == 0)
          {
            continue;
          }

          if (string.Compare(row["Nation"].ToString(), "USA", true) == 0)
          {
            flagNationUSA = 1;
          }

          DBParameter[] param = new DBParameter[12];
          param[0] = new DBParameter("@JCItem", DbType.String, row["JCITEM"].ToString());
          if (DBConvert.ParseInt(row["JCOH"].ToString()) != int.MinValue)
          {
            param[1] = new DBParameter("@JCOH", DbType.Int32, DBConvert.ParseInt(row["JCOH"].ToString()));
          }
          else
          {
            param[1] = new DBParameter("@JCOH", DbType.Int32, 0);
          }

          if (DBConvert.ParseInt(row["JCTRN"].ToString()) != int.MinValue)
          {
            param[2] = new DBParameter("@JCTRN", DbType.Int32, DBConvert.ParseInt(row["JCTRN"].ToString()));
          }
          else
          {
            param[2] = new DBParameter("@JCTRN", DbType.Int32, 0);
          }

          if (DBConvert.ParseInt(row["JCVORD"].ToString()) != int.MinValue)
          {
            param[3] = new DBParameter("@JCVORD", DbType.Int32, DBConvert.ParseInt(row["JCVORD"].ToString()));
          }
          else
          {
            param[3] = new DBParameter("@JCVORD", DbType.Int32, 0);
          }

          if (DBConvert.ParseInt(row["JCNYOR"].ToString()) != int.MinValue)
          {
            param[4] = new DBParameter("@JCNYOR", DbType.Int32, DBConvert.ParseInt(row["JCNYOR"].ToString()));
          }
          else
          {
            param[4] = new DBParameter("@JCNYOR", DbType.Int32, 0);
          }

          if (DBConvert.ParseInt(row["JCHIS6"].ToString()) != int.MinValue)
          {
            param[5] = new DBParameter("@JCHIS6", DbType.Int32, DBConvert.ParseInt(row["JCHIS6"].ToString()));
          }
          else
          {
            param[5] = new DBParameter("@JCHIS6", DbType.Int32, 0);
          }

          if (DBConvert.ParseInt(row["JCHIST"].ToString()) != int.MinValue)
          {
            param[6] = new DBParameter("@JCHIST", DbType.Int32, DBConvert.ParseInt(row["JCHIST"].ToString()));
          }
          else
          {
            param[6] = new DBParameter("@JCHIST", DbType.Int32, 0);
          }

          if (DBConvert.ParseInt(row["JCHI24"].ToString()) != int.MinValue)
          {
            param[7] = new DBParameter("@JCHI24", DbType.Int32, DBConvert.ParseInt(row["JCHI24"].ToString()));
          }
          else
          {
            param[7] = new DBParameter("@JCHI24", DbType.Int32, 0);
          }

          param[8] = new DBParameter("@JCLINV", DbType.String, row["JCLINV"].ToString());

          param[9] = new DBParameter("@Nation", DbType.String, row["Nation"].ToString());

          param[10] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          param[11] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

          DBParameter[] outputparam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanJCDL_Edit", 3000, param, outputparam);
          if (DBConvert.ParseInt(outputparam[0].Value.ToString()) == -1)
          {
            continue;
          }
        }
        DBParameter[] paramClear = new DBParameter[1];
        paramClear[0] = new DBParameter("@FlagNationUSA", DbType.Int32, flagNationUSA);
        DataBaseAccess.ExecuteStoreProcedure("spCSDClearJCDL_Edit", 600, paramClear);

        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");
        return;
      }
      this.Search();
    }

    /// <summary>
    /// Check Import Data
    /// </summary>
    /// <param name="message"></param>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private bool CheckDataImport(out string message, DataTable dtSource)
    {
      message = string.Empty;
      string commandText = string.Empty;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        if (row["JCItem"].ToString().Length > 0)
        {
          // (1): Check JCOH
          if (row["JCOH"].ToString().Length > 0 && DBConvert.ParseInt(row["JCOH"].ToString()) == int.MinValue)
          {
            message = "JCOH: " + row["JCOH"].ToString() + "";
            return false;
          }
          // (2): Check JCTRN
          if (row["JCTRN"].ToString().Length > 0 && DBConvert.ParseInt(row["JCTRN"].ToString()) == int.MinValue)
          {
            message = "JCTRN: " + row["JCTRN"].ToString() + "";
            return false;
          }
          // (3): Check JCVORD
          if (row["JCVORD"].ToString().Length > 0 && DBConvert.ParseInt(row["JCVORD"].ToString()) == int.MinValue)
          {
            message = "JCVORD: " + row["JCVORD"].ToString() + "";
            return false;
          }
          // (4): Check JCNYOR
          if (row["JCNYOR"].ToString().Length > 0 && DBConvert.ParseInt(row["JCNYOR"].ToString()) == int.MinValue)
          {
            message = "JCNYOR: " + row["JCNYOR"].ToString() + "";
            return false;
          }
          // (5): Check JCHIS6
          if (row["JCHIS6"].ToString().Length > 0 && DBConvert.ParseInt(row["JCHIS6"].ToString()) == int.MinValue)
          {
            message = "JCHIS6: " + row["JCHIS6"].ToString() + "";
            return false;
          }
          // (6): Check JCHIST
          if (row["JCHIST"].ToString().Length > 0 && DBConvert.ParseInt(row["JCHIST"].ToString()) == int.MinValue)
          {
            message = "JCHIST: " + row["JCHIST"].ToString() + "";
            return false;
          }
          // (7): Check JCHI24
          if (row["JCHI24"].ToString().Length > 0 && DBConvert.ParseInt(row["JCHI24"].ToString()) == int.MinValue)
          {
            message = "JCHI24: " + row["JCHI24"].ToString() + "";
            return false;
          }
          // (8): Check JCLINV
          // (9): Check Nation
          commandText = " SELECT NationEN FROM TblCSDNation WHERE NationEN = '" + row["Nation"].ToString() + "'";
          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck.Rows.Count == 0)
          {
            message = "Nation: " + row["Nation"].ToString() + "";
            return false;
          }
        }
      }
      return true;
    }
    #endregion
  }
}
