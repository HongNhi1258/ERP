/*
  Author      : Đỗ Minh Tâm
  Date        : 16/11/2010
  Decription  : Customer Report
  Checked by  :
  Checked date:
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.UserControls;
using System.IO;
using System.Threading;
using Microsoft.VisualBasic;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_006 : MainUserControl
  {
    public viewCSD_04_006()
    {
      InitializeComponent();
    }

    private void cboReport_SelectedIndexChanged(object sender, EventArgs e)
    {
      inVisiblePanel();
      if (cboReport.SelectedValue.ToString() == "0")
      {
        pDimensionInfo.Visible = true;
      }
      else if (cboReport.SelectedValue.ToString() == "1")
      {
        pWeeklyStock.Visible = true;
      }
    }
    private void inVisiblePanel()
    {
      pDimensionInfo.Visible = false;
      pWeeklyStock.Visible = false;
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      if (cboReport.SelectedValue.ToString() == "0") 
      {
        ShowDimentionInfo();
      }
    }
    private string SelectTextFile(string initialDirectory)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter =
         "mdb files (*.xls)|*.xls|All files (*.*)|*.*";
      dialog.InitialDirectory = initialDirectory;
      dialog.Title = "Select a Excel file";
      return (dialog.ShowDialog() == DialogResult.OK)
         ? dialog.FileName : null;

    }
    private void InputData(int iInputType)
    {
      string strSheetName = "";
      

      string strPathFile = @SelectTextFile("My Computer");
      //string x = Interaction.InputBox("Input Sheet Name", "Input Sheet Name", "SheetName", 10, 10);
      string x = "";
      if (iInputType == 1)
      {
        x = "FGW";
      }
      else
      {
        x = "intransit";
      }
      DataSet ds = Shared.Utility.FunctionUtility.GetExcelToDataSet(strPathFile, "Select * from [" + x + "$A0:B1000]");
      if (ds.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0007", "Don't have got Data, please check Sheet Name or Data file.");
        return;
      }
      if (ds.Tables[0].Rows.Count > 0)
      {
        DataBaseAccess.ExecuteCommandText("update TblCSDWeeklyInputFromUSA set Qty = 0 where isnull(InputType,0) =  " + iInputType.ToString(), null);
        bool chkCheck = true;
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
          int iQty = 0;
          try
          {
            iQty = DBConvert.ParseInt(ds.Tables[0].Rows[i][1].ToString());
          }
          catch
          {

          }

          if (iQty > 0)
          {
            DBParameter[] inputParam = new DBParameter[6];
            inputParam[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ds.Tables[0].Rows[i][0].ToString());
            inputParam[1] = new DBParameter("@Qty", DbType.Int64, iQty);
            inputParam[2] = new DBParameter("@InputType", DbType.Int64, iInputType);
            if (!DataBaseAccess.ExecuteStoreProcedure("spCSDWeeklyInputFromUSA", inputParam))
            {
              chkCheck = false;
            }
          }
        }
        if (!chkCheck)
        {
          WindowUtinity.ShowMessageError("ERR0032", "Please check data again.");
        }
        else
        {
          WindowUtinity.ShowMessageSuccess("MSG0003", "");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0007", "Don't have got Data, please check Sheet Name or Data file.");
      }
    }
    private void ShowDimentionInfo()
    {
      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@ItemKind", DbType.AnsiString,9, cboItemKind.SelectedValue.ToString() + "%");
      if (txtCategory.Text != "")
      {
        inputParam[1] = new DBParameter("@Category", DbType.AnsiString, 130, "%" + txtCategory.Text + "%");
      }
      if (txtCollection.Text != "")
      {
        inputParam[2] = new DBParameter("@Collection", DbType.AnsiString, 66, "%" + txtCollection.Text + "%");
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDReportItemDimension", inputParam);
      dtSource.Columns.Add("Image", System.Type.GetType("System.Byte[]"));
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        dtSource.Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray_Always(@dtSource.Rows[i]["img"].ToString());
      }
      
      CustomerService.DataSetSource.dsCSDDimensionInfo ds = new DaiCo.CustomerService.DataSetSource.dsCSDDimensionInfo();
      ds.Tables.Add(dtSource.Clone());
      ds.Tables["dtDimensionInfo"].Merge(dtSource);
      DaiCo.CustomerService.ReportTemplate.cptCSDDimensionInfo cptDim = new DaiCo.CustomerService.ReportTemplate.cptCSDDimensionInfo();
      cptDim.SetDataSource(ds);
      Shared.View_Report frm = new View_Report(cptDim);
      frm.ShowReport(ViewState.Window,FormWindowState.Maximized);
    }
    private void ShowWeeklyStock()
    {

    }
    private void viewCSD_04_006_Load(object sender, EventArgs e)
    {
      DataTable dtReport = new DataTable();
      dtReport.Columns.Add("Tex");
      dtReport.Columns.Add("Val",typeof(Int32));

      DataRow dr = dtReport.NewRow();
      dr["Tex"] = "";
      dr["Val"] = int.MinValue;
      dtReport.Rows.Add(dr);

      dr = dtReport.NewRow();
      dr["Tex"] = "Dimension Info";
      dr["Val"] = 0;
      dtReport.Rows.Add(dr);

      dr = dtReport.NewRow();
      dr["Tex"] = "Weekly Stock";
      dr["Val"] = 1;
      dtReport.Rows.Add(dr);

      cboReport.DataSource = dtReport;
      cboReport.DisplayMember = "Tex";
      cboReport.ValueMember = "Val";

      string strSql = "SELECT [PrefixCode], [Description] FROM TblBOMPrefix WHERE [Group] = 1 ORDER BY [No]";
      DataTable dtItemKind = DataBaseAccess.SearchCommandTextDataTable(strSql, null);
      cboItemKind.DataSource = dtItemKind;
      cboItemKind.DisplayMember = "Description";
      cboItemKind.ValueMember = "PrefixCode";
    }

    private void btnPrintWeekly_Click(object sender, EventArgs e)
    {
      try
      {
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDReportWeeklyStockForCustomer", null);
        dtSource.Columns.Add("Image", System.Type.GetType("System.Byte[]"));
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          dtSource.Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray_Always(@dtSource.Rows[i]["img"].ToString());
          string strImage = dtSource.Rows[i]["Image"].ToString();
        }

        DaiCo.CustomerService.DataSetSource.dsCSDWeeklyStock dsSource = new DaiCo.CustomerService.DataSetSource.dsCSDWeeklyStock();
        dsSource.Tables.Add(dtSource.Clone());
        dsSource.Tables["dtWeeklyStock"].Merge(dtSource);

        DaiCo.CustomerService.ReportTemplate.cptCSDWeeklyStock cptWeeklyStock = new DaiCo.CustomerService.ReportTemplate.cptCSDWeeklyStock();
        cptWeeklyStock.SetDataSource(dsSource);
        Shared.View_Report frm = new View_Report(cptWeeklyStock);
        frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0007", "Don't have got Data, please check Sheet Name or Data file.");
      }
    }

    private void btnDimensionClose_Click(object sender, EventArgs e)
    {
      pDimensionInfo.Visible = false;
      cboReport.SelectedIndex = 0;
    }

    private void tbnCloseWeekly_Click(object sender, EventArgs e)
    {
      pWeeklyStock.Visible = false;
      cboReport.SelectedIndex = 0;
    }

    private void btnFG_Click(object sender, EventArgs e)
    {
      InputData(1);
    }

    private void btnTransit_Click(object sender, EventArgs e)
    {
      InputData(2);
    }
    
  }
}
