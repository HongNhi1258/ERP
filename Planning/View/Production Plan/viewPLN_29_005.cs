/*
  Author      : Nguyen Huynh Quoc Tuan
  Date        : 3/6/2015
  Description : Stuff Deadline
  Standard Code: view_MasterDetail.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_29_005 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewPLN_29_005()
    {
      InitializeComponent();
    }

    #endregion Init

    #region Function

    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Part Furniture Code", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "Part Furniture Code";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }

    //Tạo bảng tạm cho Option 1
    private DataTable dtDeadlineResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("FurnitureCode", typeof(System.String));
      return dt;
    }

    //Import Option 1
    private void ImportDataOption1()
    {
      if (this.txtLocation.Text.Trim().Length == 0)
      {
        return;
      }

      DataTable dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), "SELECT * FROM [Sheet1 (1)$A1:A100]").Tables[0];
      if (dtSource == null)
      {
        return;
      }
      // dt New 
      DataTable dtInput = this.dtDeadlineResult();
      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtInput.NewRow();
        if (row["FurnitureCode"].ToString().Length > 0)
        {
          // Furniture Old
          if (row["FurnitureCode"].ToString().Trim().Length > 0)
          {
            rowadd["FurnitureCode"] = row["FurnitureCode"];
          }

          //Add row datatable
          dtInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@Data", SqlDbType.Structured, dtInput);
      DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNProductionPlanPartFurniture", sqlinput);
      ultData.DataSource = dtResultDeadline;

      //for (int i = 0; i < ultData.Rows.Count; i++)
      //{
      //  if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) != 0)
      //  {
      //    ultData.Rows[i].Appearance.BackColor = Color.Yellow;
      //  }
      //}
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnGet_Click(object sender, EventArgs e)
    {
      string templateName = "viewPLN_29_005";
      string sheetName = "Sheet1";
      string outFileName = "Import FurnitureCode";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      this.ImportDataOption1();
    }

    private void btBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtLocation.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }

    #endregion Event
  }
}
