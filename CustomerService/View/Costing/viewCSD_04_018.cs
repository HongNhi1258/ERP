using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using DaiCo.Shared.Utility;
using System.Collections;
using VBReport;
using System.Diagnostics;

namespace DaiCo.Technical
{
  public partial class viewCSD_04_018 : MainUserControl
  {
    #region Field
    private DataTable dtItem;
    #endregion Field

    #region Init
    public viewCSD_04_018()
    {
      InitializeComponent();
    }

    private void viewCSD_04_018_Load(object sender, EventArgs e)
    {
      this.dtItem = DataBaseAccess.SearchCommandTextDataTable("SELECT ItemCode, SaleCode, Name ItemName FROM TblBOMItemBasic");
    }
    #endregion Init

    #region Function
    private void SaveData()
    {
      // Check Invalid
      DataTable dtCheck = (DataTable) ultData.DataSource;
      if (dtCheck.Select(string.Format("StatusValue = 0")).Length > 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Some rows");
        return;
      }

      // Update data
      bool success = true;
      DataTable dtSource = (DataTable)ultData.DataSource;
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (row.RowState != DataRowState.Deleted)
          {
            string itemCode = row["ItemCode"].ToString();
            int moq = DBConvert.ParseInt(row["MOQ"].ToString());
            int updateBy = Shared.Utility.SharedObject.UserInfo.UserPid;

            DBParameter[] input = new DBParameter[3];
            input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
            input[1] = new DBParameter("@MOQ", DbType.Int32, moq);
            input[2] = new DBParameter("@UpdateBy", DbType.Int32, updateBy);            
            DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            DataBaseAccess.ExecuteStoreProcedure("spCSDItemInfo_UpdateMOQ", input, output);
            if (DBConvert.ParseLong(output[0].Value.ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0005");
              success = false;                            
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.SaveSuccess = true;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
          this.SaveSuccess = false;
        }
      }
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;            
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      e.Layout.Bands[0].Columns["StatusText"].Header.Caption = "Status";
      e.Layout.Bands[0].Columns["StatusValue"].Hidden = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if ((ultData.Rows[i].Cells["StatusValue"].Value.ToString()) == "0")
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
      }
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["MOQ"].CellActivation = Activation.AllowEdit;
    }

    private void btnOpen_Click(object sender, EventArgs e)
    {
      txtFilePath.Text = string.Empty;
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.InitialDirectory = folder;
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      string importFileName = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      txtFilePath.Text = importFileName;
      btnImport.Enabled = (importFileName.Length > 0);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      // Get info from Excel file
      DataSet dsInput = Shared.Utility.FunctionUtility.GetExcelToDataSet(txtFilePath.Text.Trim(), "SELECT * FROM [MOQ (1)$B5:C2005]");      
      if (dsInput != null && dsInput.Tables.Count > 0 && this.dtItem != null && dtItem.Rows.Count > 0)
      {
        DataTable dtMOQ = new DataTable();
        dtMOQ.Columns.Add("ItemCode", typeof(System.String));
        dtMOQ.Columns.Add("SaleCode", typeof(System.String));
        dtMOQ.Columns.Add("ItemName", typeof(System.String));
        dtMOQ.Columns.Add("MOQ", typeof(System.Int32));
        dtMOQ.Columns.Add("StatusText", typeof(System.String));
        dtMOQ.Columns.Add("StatusValue", typeof(System.Int32));

        foreach (DataRow row in dsInput.Tables[0].Rows)
        {
          if (row["ItemCode"].ToString().Length == 0)
          {
            break;
          }
          DataRow rowMOQ = dtMOQ.NewRow();
          rowMOQ["ItemCode"] = row["ItemCode"];
          rowMOQ["MOQ"] = row["MOQ"];
          DataRow[] rowSelected = this.dtItem.Select(string.Format("ItemCode = '{0}'", row["ItemCode"]));
          if (rowSelected.Length > 0)
          {
            rowMOQ["SaleCode"] = rowSelected[0]["SaleCode"];
            rowMOQ["ItemName"] = rowSelected[0]["ItemName"];
            rowMOQ["StatusValue"] = 1;
            rowMOQ["StatusText"] = "Ready";
          }
          else
          {
            rowMOQ["StatusValue"] = 0;
            rowMOQ["StatusText"] = "Invalid";            
          }
          if (DBConvert.ParseInt(row["MOQ"].ToString()) <= 0)
          {
            rowMOQ["StatusValue"] = 0;
            rowMOQ["StatusText"] = "Invalid"; 
          }
          dtMOQ.Rows.Add(rowMOQ);
        }        
        ultData.DataSource = dtMOQ;
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0024");        
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0105");
      }
    }

    private void btnRemove_Click(object sender, EventArgs e)
    {      
      DataTable dtSource = (DataTable)ultData.DataSource;
      DataRow[] arrRow = dtSource.Select("StatusValue = 0");
      for (int i = 0; i < arrRow.Length; i++ )
      {
        dtSource.Rows.Remove(arrRow[i]);
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "ItemSalePrice";
      string sheetName = "MOQ";
      string outFileName = "MOQItem";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (string.Compare(e.Cell.Column.ToString(), "MOQ", true) == 0)
      {
        if (DBConvert.ParseInt(e.Cell.Value.ToString()) <= 0)
        {
          e.Cell.Row.Cells["StatusValue"].Value = 0;
          e.Cell.Row.Cells["StatusText"].Value = "Invalid";
          e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
        }
        else if (this.dtItem.Select(string.Format("ItemCode = '{0}'", e.Cell.Row.Cells["ItemCode"].Value)).Length > 0)
        {
          e.Cell.Row.Cells["StatusValue"].Value = 1;
          e.Cell.Row.Cells["StatusText"].Value = "Ready";
          e.Cell.Row.CellAppearance.BackColor = Color.White;
        }
      }
    }
    #endregion Event
  }
}
