/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: view_ExtraControl.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_01_008 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //Utility.LoadUltraCombo();
      //Utility.LoadUltraDropDown();
    }
    #endregion function

    #region event
    public viewPLN_01_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_01_008_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      // Get data for items list
      DataTable dtItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [Sheet1 (1)$A3:A{0}]", 500)).Tables[0];
      if (dtItemList == null || dtItemList.Rows.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      DataTable dtEN = new DataTable();
      dtEN.Columns.Add("EnquiryNo", typeof(System.String));
      foreach (DataRow row in dtItemList.Rows)
      {
        string enNo = row[0].ToString().Trim();
        if (enNo.Length > 0)
        {
          DataRow rowEN = dtEN.NewRow();
          rowEN["EnquiryNo"] = enNo;
          dtEN.Rows.Add(rowEN);
        }
      }
      SqlDBParameter[] input = new SqlDBParameter[1];
      input[0] = new SqlDBParameter("@DataSource", SqlDbType.Structured, dtEN);
      SqlDBParameter[] output = new SqlDBParameter[1];
      output[0] = new SqlDBParameter("@Result", SqlDbType.Int, 0);
      SqlDataBaseAccess.ExecuteStoreProcedure("spPLNReAllocateForEnquiry_Import", input, output);
      if (DBConvert.ParseInt(output[0].Value.ToString()) <= 0)
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
      else
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.ConfirmToCloseTab();
      }
    }


    private void btnBrowseItem_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      dialog.InitialDirectory = pathOutputFile;
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }


    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "viewPLN_01_003_Template";
      string sheetName = "Sheet1";
      string outFileName = "Enquiry Template";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\Planning\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }
    #endregion event
  }
}
