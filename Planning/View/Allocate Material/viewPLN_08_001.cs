using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Diagnostics;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_08_001 : MainUserControl
  {
    public viewPLN_08_001()
    {
      InitializeComponent();
    }

    #region Function
    private void LoadData()
    {
      ControlUtility.LoaducUltraListMaterialGroup(ucUltraListMaterialGroup);
    }

    private void SearchData()
    {
      DBParameter[] inputParam = new DBParameter[3];
      string wos = txtWO.Text.Trim();
      string itemCodes = txtItemCode.Text.Trim();
      string materialGroups = ucUltraListMaterialGroup.SelectedValue;
      if (wos.Length > 0)
      {
        inputParam[0] = new DBParameter("@WOs", DbType.AnsiString, 512, wos);
      }
      if (itemCodes.Length > 0)
      {
        inputParam[1] = new DBParameter("@ItemCodes", DbType.AnsiString, 1024, itemCodes);
      }
      if (materialGroups.Length > 0)
      {
        inputParam[2] = new DBParameter("@MaterialGroups", DbType.AnsiString, 512, materialGroups);
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNDimentionMaterialInformation", inputParam);
      ultraGridInformation.DataSource = dtSource;
      lbTotalRecords.Text = string.Format("Total Records: {0}", dtSource.Rows.Count);
    }
    #endregion Function

    #region Event
    private void chkShowMaterialGroup_CheckedChanged(object sender, EventArgs e)
    {
      ucUltraListMaterialGroup.Visible = chkShowMaterialGroup.Checked;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.SearchData();
      btnSearch.Enabled = true;
    }

    private void viewPLN_08_001_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void ucUltraListMaterialGroup_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtMaterialGroup.Text = ucUltraListMaterialGroup.SelectedValue;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnExportToExcel_Click(object sender, EventArgs e)
    {
      btnExportToExcel.Enabled = false;

      string strTemplateName = "PlanningReport";
      string strSheetName = "DimentionMaterial";
      string strOutFileName = "Dimention Material Report";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      // Search :
      DataTable dtData = (DataTable)ultraGridInformation.DataSource;
      if (dtData != null && dtData.Rows.Count > 0)
      {
        for (int i = 0; i < dtData.Rows.Count; i++)
        {
          DataRow dtRow = dtData.Rows[i];
          if (i > 0)
          {
            oXlsReport.Cell("B6:N6").Copy();
            oXlsReport.RowInsert(5 + i);
            oXlsReport.Cell("B6:N6", 0, i).Paste();
          }
          oXlsReport.Cell("**No", 0, i).Value = i + 1;
          oXlsReport.Cell("**ItemCode", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**Revision", 0, i).Value = dtRow["Revision"];
          oXlsReport.Cell("**ENItemName", 0, i).Value = dtRow["ENItemName"];
          oXlsReport.Cell("**VNItemName", 0, i).Value = dtRow["VNItemName"];
          oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"];
          oXlsReport.Cell("**ENMaterialName", 0, i).Value = dtRow["ENMaterialName"];
          oXlsReport.Cell("**VNMaterialName", 0, i).Value = dtRow["VNMaterialName"];
          oXlsReport.Cell("**Qty", 0, i).Value = dtRow["Qty"];
          oXlsReport.Cell("**FactoryUnit", 0, i).Value = dtRow["FactoryUnit"];
          oXlsReport.Cell("**Length", 0, i).Value = dtRow["Length"];
          oXlsReport.Cell("**Width", 0, i).Value = dtRow["Width"];
          oXlsReport.Cell("**Thickness", 0, i).Value = dtRow["Thickness"];
        }
      }
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);

      btnExportToExcel.Enabled = true;
    }
    #endregion Event
  }
}
