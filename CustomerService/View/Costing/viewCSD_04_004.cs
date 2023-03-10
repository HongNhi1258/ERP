/*
  Author      : Đỗ Minh Tâm
  Date        : 2/11/2010
  Decription  : Insert, Update List out item
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
using Microsoft.VisualBasic;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_004 : MainUserControl
  {
    public viewCSD_04_004()
    {
      InitializeComponent();
    }

    private void viewCSD_04_004_Load(object sender, EventArgs e)
    {
      Shared.Utility.ControlUtility.LoadEmployee(cboEmp, "CSD");
      button1_Click(sender, e);
    }

    private void btnPreview_Click(object sender, EventArgs e)
    {

    }

    private void button6_Click(object sender, EventArgs e)
    {

    }

    private void button1_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[5];
      if (txtOutCode.Text != "" || txtOutCode.Text.Length > 0)
      {
        inputParam[0] = new DBParameter("@OutputCode", DbType.AnsiString, txtOutCode.Text);
      }
      if (txtDes.Text != "" || txtDes.Text.Length > 0)
      {
        inputParam[1] = new DBParameter("@Description", DbType.String, txtDes.Text);
      }
      if (dtdFrom.Value != DateTime.MinValue)
      {
        inputParam[2] = new DBParameter("@CreateDateFrom", DbType.DateTime, dtdFrom.Value);
      }
      if (dtdTo.Value != DateTime.MinValue)
      {
        inputParam[3] = new DBParameter("@CreateDateTo", DbType.DateTime, dtdTo.Value.AddDays(1));
      }
      if (cboEmp.SelectedIndex > 0)
      {
        inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, cboEmp.SelectedValue.ToString());
      }
      DataTable dtsource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDOutputItem_Select", inputParam);
      ultListOutItem.DataSource = dtsource;
    }

    private void cboEmp_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
    {

    }

    private void ultListOutItem_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultListOutItem.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultListOutItem.Selected.Rows[0];
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      viewCSD_04_005 view = new viewCSD_04_005();
      view.outItemPid = pid;
      WindowUtinity.ShowView(view, "LIST OUT ITEM INFORMATION", false, ViewState.ModalWindow);
    }

    private void button3_Click(object sender, EventArgs e)
    {
      viewCSD_04_005 view = new viewCSD_04_005();
      WindowUtinity.ShowView(view, "LIST OUT ITEM INFORMATION", false, ViewState.ModalWindow);
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultListOutItem_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultListOutItem);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["OutputCode"].Header.Caption = "Output Code";
      e.Layout.Bands[0].Columns["BasePrice"].Header.Caption = "Base Price";
      e.Layout.Bands[0].Columns["BasePrice"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
    }
    private void InputData(int iInputType)
    {
      string strPathFile = @SelectTextFile("My Computer");
      //string x = Interaction.InputBox("Nhap Ten Sheet", "Input Sheet Name", "SheetName", 10, 10);
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
    private void btnFG_Click(object sender, EventArgs e)
    {
      InputData(1);
    }

    private void btnTransit_Click(object sender, EventArgs e)
    {
      InputData(2);
    }

    private void btnWeekly_Click(object sender, EventArgs e)
    {
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDReportWeeklyStockForCustomer", null);

      dtSource.Columns.Add("Image", System.Type.GetType("System.Byte[]"));
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        dtSource.Rows[i]["Image"] = Shared.Utility.FunctionUtility.ImagePathToByteArray_Always(@dtSource.Rows[i]["img"].ToString());
      }

      DaiCo.CustomerService.DataSetSource.dsCSDWeeklyStock dsSource = new DaiCo.CustomerService.DataSetSource.dsCSDWeeklyStock();
      dsSource.Tables.Add(dtSource.Clone());
      dsSource.Tables["dtWeeklyStock"].Merge(dtSource);

      DaiCo.CustomerService.ReportTemplate.cptCSDWeeklyStock cptWeeklyStock = new DaiCo.CustomerService.ReportTemplate.cptCSDWeeklyStock();
      cptWeeklyStock.SetDataSource(dsSource);
      View_Report frm = new View_Report(cptWeeklyStock);
      frm.ShowReport(ViewState.Window, FormWindowState.Maximized);
    }
  }
}
