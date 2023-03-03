/*
 * Author       : Huu Phuoc
 * CreateDate   : 22/03/2016
 * Description  : Update JCSD
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
  public partial class viewCSD_01_016 : MainUserControl
  {
    #region Init
    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    public viewCSD_01_016()
    {
      InitializeComponent();
    }

    private void viewCSD_01_016_Load(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      this.pathTemplate = startupPath + @"\ExcelTemplate";
      this.pathExport = startupPath + @"\Report";
    }
    #endregion Init

    #region Load Data
    private void Search()
    {
      int customerPid = 0;

      if (rdbtJCRU.Checked)
      {
        customerPid = 10;
      }
      if (rdbtUK.Checked)
      {
        customerPid = 11;
      }
      if (rdbtCn.Checked)
      {
        customerPid = 133;
      }
      if (rdbtInt.Checked)
      {
        customerPid = 146;
      }
      if (rdbtAll.Checked)
      {
        customerPid = 0;
      }
      if (rdbtME.Checked)
      {
        customerPid = 19;
      }
      DBParameter[] param = new DBParameter[2];
      param[0] = new DBParameter("@SaleCode", DbType.String, txtSaleCode.Text.ToString());
      param[1] = new DBParameter("@CustomerPid", DbType.Int16, customerPid);
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDSearchInformationOfSaleCode", param);
      
      this.ultData.DataSource = dtSource;
      if (ultData.Rows !=null)
      {
        lblCount.Text = "Count: " + ultData.Rows.Count;
      }
      
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
      ControlUtility.ExportToExcelWithDefaultPath(ultData, "Data");
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
      e.Layout.Bands[0].Columns["No"].Hidden  = true ;
      e.Layout.Bands[0].Columns["WeeksInt"].Hidden = true;      
      //header label
      e.Layout.Bands[0].Columns["CustomerCode"].Header.Caption = "Customer Code";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Customer Name";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Q.ty";
      e.Layout.Bands[0].Columns["Weeks"].Header.Caption = "Weeks";
      e.Layout.Bands[0].Columns["WeeksInt"].Header.Caption = "Week Int";

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        // Set color for edit & read only cell
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;      
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }

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
      string templateName = "RPT_CSD_01_016_001";
      string sheetName = "Stock";
      string outFileName = "RPT_CSD_01_016_001";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      btnImport.Enabled = false;
      int customerPid = 0;
      if (this.txtImport.Text.Trim().Length == 0)
      {
        return;
      }
      if (!rdbtUK.Checked && !rdbtJCRU.Checked && !rdbtCn.Checked && !rdbtInt.Checked && !rdbtME.Checked)
      {
        MessageBox.Show("Please Choose a option for Customer!");
        return;
      }     
      try
      {
        int maxRows = int.MinValue;
        DataTable dtMaxRows = new DataTable();
        try
        {
           dtMaxRows = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImport.Text.Trim(), string.Format(@"SELECT * FROM [Stock$I2:I3]")).Tables[0];
        }
        catch
        {
           dtMaxRows = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImport.Text.Trim(), string.Format(@"SELECT * FROM [Stock (1)$I2:I3]")).Tables[0];
        }

        if (dtMaxRows != null && dtMaxRows.Rows.Count > 0)
        {
          maxRows = DBConvert.ParseInt(dtMaxRows.Rows[0][0].ToString());
        }

        if (maxRows > 0)
        {
          DataTable dtSource = new DataTable();
          try
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImport.Text.Trim(), string.Format(@"SELECT * FROM [Stock$A3:F{0}]", maxRows + 3)).Tables[0];
          }
          catch
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSet(txtImport.Text.Trim(), string.Format(@"SELECT * FROM [Stock (1)$A3:F{0}]", maxRows + 3)).Tables[0];
          }

          if (dtSource == null)
          {
            return;
          }

          if (rdbtJCRU.Checked)
          {
            customerPid = 10;
          }
          else if(rdbtUK.Checked)
          {
            customerPid = 11;
          }
          else if (rdbtCn.Checked)
          {
            customerPid = 133;
          }
          else if (rdbtInt.Checked)
          {
            customerPid = 146;
          }
          else if (rdbtME.Checked)
          {
            customerPid = 19;
          }

          foreach (DataRow row in dtSource.Rows)
          {
            DBParameter[] param = new DBParameter[7];
            param[0] = new DBParameter("@SaleCode", DbType.String, row["SaleCode"].ToString().Trim());

            param[1] = new DBParameter("@CustomerPid", DbType.Int32, customerPid);

            if (DBConvert.ParseInt(row["1WEEK"].ToString()) != int.MinValue)
            {
              param[2] = new DBParameter("@W1", DbType.Int32, DBConvert.ParseInt(row["1WEEK"].ToString()));
            }
            else
            {
              param[2] = new DBParameter("@W1", DbType.Int32, 0);
            }

            if (DBConvert.ParseInt(row["2-6WEEKS"].ToString()) != int.MinValue)
            {
              param[3] = new DBParameter("@W2", DbType.Int32, DBConvert.ParseInt(row["2-6WEEKS"].ToString()));
            }
            else
            {
              param[3] = new DBParameter("@W2", DbType.Int32, 0);
            }

            if (DBConvert.ParseInt(row["6-8WEEKS"].ToString()) != int.MinValue)
            {
              param[4] = new DBParameter("@W3", DbType.Int32, DBConvert.ParseInt(row["6-8WEEKS"].ToString()));
            }
            else
            {
              param[4] = new DBParameter("@W3", DbType.Int32, 0);
            }

            if (DBConvert.ParseInt(row["8-14WEEKS"].ToString()) != int.MinValue)
            {
              param[5] = new DBParameter("@W4", DbType.Int32, DBConvert.ParseInt(row["8-14WEEKS"].ToString()));
            }
            else
            {
              param[5] = new DBParameter("@W4", DbType.Int32, 0);
            }

            if (DBConvert.ParseInt(row["14-22WEEKS"].ToString()) != int.MinValue)
            {
              param[6] = new DBParameter("@W5", DbType.Int32, DBConvert.ParseInt(row["14-22WEEKS"].ToString()));
            }
            else
            {
              param[6] = new DBParameter("@W5", DbType.Int32, 0);
            }

            DBParameter[] outputparam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spCSDGetDataFromExcelTemplete", param, outputparam);
            if (DBConvert.ParseInt(outputparam[0].Value.ToString()) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0037", "Data");
              return;
            }
          }
          DBParameter[] param1 = new DBParameter[1];
          param1[0] = new DBParameter("@CustomerPid", DbType.Int32, customerPid);
          DataBaseAccess.ExecuteStoreProcedure("spCSDClearCSDDATA_Edit", param1);
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
      }
      catch
      {
        WindowUtinity.ShowMessageError("ERR0105");;   
        return;
      }   
      this.Search();
      btnImport.Enabled = true;
    }
    #endregion
  }
}
