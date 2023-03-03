/*
  Author      : Do Minh Tam
  Date        : 1/07/2012
  Description : Report for CSD get list of shipment
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Application;
using DaiCo.Shared.Utility;
using VBReport;
using System.Diagnostics;
using DaiCo.Shared.DataBaseUtility;
using System.IO;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_06_003 : MainUserControl
  {
    #region Field
    int maxRows = int.MinValue;
    string formatDate = "dd/MM/yy";
    private IList processList = new ArrayList();
    #endregion Field

    #region Init
    public viewCSD_06_003()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load viewCSD_06_003
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_06_003_Load(object sender, EventArgs e)
    {
      // Load Report Source
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Get Template Report
    /// </summary>
    private void GetTemplateReport()
    {
        string templateName = "CSDListForShipmentTemplate";
      string sheetName = "Sheet1";
      string outFileName = "List for shipment template";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate\CustomerService";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Get Template
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
        this.GetTemplateReport();
    }
    /// <summary>
    /// Brown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBrowse_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.InitialDirectory = folder;
      dialog.Title = "Select a Excel file";
      txtFilePath.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }
    /// <summary>
    /// Print
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      btnPrint.Enabled = false;
      string strTemplateName = "CSDListForShipmentReport";
      string strSheetName = "Sheet1";
      string strOutFileName = "CSDListForShipmentReport";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      DataSet dsTitleExcel = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), "SELECT * FROM [Sheet1 (1)$A1:E50000]");
      btnPrint.Enabled = true;
      DataSet dsResult = new DataSet();
      dsResult = this.GetDataTableImport(dsTitleExcel.Tables[0]);
      if (dsResult != null && dsResult.Tables[0].Rows.Count > 0)
      {
          for (int i = 0; i < dsResult.Tables[0].Rows.Count; i++)
          {
              DataRow dtRow = dsResult.Tables[0].Rows[i];
              if (i > 0)
              {
                  oXlsReport.Cell("A5:X5").Copy();
                  oXlsReport.RowInsert(4 + i);
                  oXlsReport.Cell("A5:X5", 0, i).Paste();
              }
              for (int y = 0; y < dsResult.Tables[0].Columns.Count; y++)
              {
                  oXlsReport.Cell("**" + String.Format("{0}", (y + 1)), 0, i).Value = dsResult.Tables[0].Rows[i][y].ToString();
              }
          }
      }
      oXlsReport.Cell("**title1").Value = ultcbReport.Text;
      oXlsReport.Cell("**title2").Value = txtTitle.Text;
      oXlsReport.Cell("**date").Value = ultDTDateFrom.Value;

      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }
    /// <summary>
    /// Get DataTable When Import Data
    /// </summary>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private DataSet GetDataTableImport(DataTable dtSource)
    {
        DataTable dt = new DataTable();
        SqlCommand cm = new SqlCommand("spCSDListForShipment_Report");
        cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
        cm.CommandType = CommandType.StoredProcedure;

        // Data Table 
        SqlParameter para = cm.CreateParameter();
        para.ParameterName = "@ImportData";
        para.SqlDbType = SqlDbType.Structured;
        para.Value = dtSource;


        cm.Parameters.Add(para);

        SqlDataAdapter adp = new SqlDataAdapter();
        adp.SelectCommand = cm;
        DataSet result = new DataSet();
        try
        {
            if (cm.Connection.State != ConnectionState.Open)
            {
                cm.Connection.Open();
            }
            adp.Fill(result);
        }
        catch (Exception ex)
        {
            result = null;
            return null;
        }


        return result;
    }
    /// <summary>
    /// Close
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

