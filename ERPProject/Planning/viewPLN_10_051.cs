/*
  Author      : Nguyen Huynh Quoc Tuan
  Date        : 11/5/2016
  Description : input furniture for allocate enquiry
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
//using Excel;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_10_051 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    public DataTable dtDraft = new DataTable();
    public DataTable dtConfirm = new DataTable();
    public DataTable dtKind = new DataTable();
    public string dept = string.Empty;
    public long transactionPid = long.MinValue;
    public bool flagSearch = false;
    private int flagError = int.MinValue;
    #endregion Field

    #region Init
    public viewPLN_10_051()
    {
      InitializeComponent();
    }

    private void viewPLN_10_051_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      foreach (Control ctr in tableLayoutMaster.Controls)
      {
        ctr.TextChanged += new System.EventHandler(this.Object_Changed);
      }
      this.InitData();
      this.LoadData();
    }
    #endregion Init

    #region Function
    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
    }

    private void SetStatusControl()
    {

    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      //DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, long.MinValue) };
      //DataSet dsSource = DataBaseAccess.SearchStoreProcedure("", inputParam);
      //if (dsSource != null && dsSource.Tables.Count > 1)
      //{
      //  this.LoadMainData(dsSource.Tables[0]);
      //  ultData.DataSource = dsSource.Tables[1];
      //}

      //this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      if (this.flagError == 1)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Data inport");
        return false;
      }
      return true;
    }

    private DataTable dtDeadlineResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("EnquiryNo", typeof(System.String));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("FurnitureCode", typeof(System.String));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }

    private void ImportData()
    {
      if (this.txtImport.Text.Trim().Length == 0)
      {
        return;
      }
      btnSave.Enabled = true;
      // Get Data Table From Excel
      DataTable dtSource = new DataTable();
      //dtSource = FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B5:F805]").Tables[0];
      dtSource = Utility.GetDataFromExcel(txtImport.Text.Trim(), "Sheet1 (1)", "B3:F1000");
      if (dtSource == null)
      {
        return;
      }

      // Input ------- 
      SqlDBParameter[] sqlinput = new SqlDBParameter[1];
      DataTable dtDeadlineInput = this.dtDeadlineResult();

      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtDeadlineInput.NewRow();
        if (row["EnquiryNo"].ToString().Trim().Length > 0)
        {
          // WO
          if (row["EnquiryNo"].ToString().Trim().Length > 0)
          {
            rowadd["EnquiryNo"] = row["EnquiryNo"].ToString();
          }

          // ItemCode
          if (row["ItemCode"].ToString().Length > 0)
          {
            rowadd["ItemCode"] = row["ItemCode"];
          }

          // Revision
          if (DBConvert.ParseInt(row["Revision"].ToString()) != int.MinValue)
          {
            rowadd["Revision"] = DBConvert.ParseInt(row["Revision"]);
          }

          // Furniture
          if (row["FurnitureCode"].ToString().Length > 0)
          {
            rowadd["FurnitureCode"] = row["FurnitureCode"];
          }

          // Furniture
          if (row["Remark"].ToString().Length > 0)
          {
            rowadd["Remark"] = row["Remark"];
          }

          //Add row datatable
          dtDeadlineInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtDeadlineInput);
      DataSet dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedure("spPLNAllocateFurnitureForEnquiry_Import", 1000, sqlinput);
      this.flagError = DBConvert.ParseInt(dtResultDeadline.Tables[0].Rows[0]["Error"].ToString());
      ultData.DataSource = dtResultDeadline.Tables[1];
      if (this.flagError == 1)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["ErrorCode"].Value.ToString()) != 0)
          {
            if (DBConvert.ParseInt(ultData.Rows[i].Cells["ErrorCode"].Value.ToString()) == 1)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.ForestGreen;
            }
            else if (DBConvert.ParseInt(ultData.Rows[i].Cells["ErrorCode"].Value.ToString()) == 2)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.CadetBlue;
            }
            else if (DBConvert.ParseInt(ultData.Rows[i].Cells["ErrorCode"].Value.ToString()) == 5)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.Goldenrod;
            }
            else if (DBConvert.ParseInt(ultData.Rows[i].Cells["ErrorCode"].Value.ToString()) == 6)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.Violet;
            }
            else if (DBConvert.ParseInt(ultData.Rows[i].Cells["ErrorCode"].Value.ToString()) == 7)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.Salmon;
            }
            else if (DBConvert.ParseInt(ultData.Rows[i].Cells["ErrorCode"].Value.ToString()) == 4)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.LimeGreen;
            }
            else if (DBConvert.ParseInt(ultData.Rows[i].Cells["ErrorCode"].Value.ToString()) == 3)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.LightSkyBlue;
            }
          }
        }
      }
    }

    private bool SaveMain()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        DBParameter[] input = new DBParameter[4];
        long endetailPid = DBConvert.ParseLong(ultData.Rows[i].Cells["EnDetailPid"].Value.ToString());
        long furPid = DBConvert.ParseLong(ultData.Rows[i].Cells["FurniturePid"].Value.ToString());
        string remark = ultData.Rows[i].Cells["Remark"].Value.ToString();
        input[0] = new DBParameter("@ENDTPid", DbType.Int64, endetailPid);
        input[1] = new DBParameter("@FURPid", DbType.Int64, furPid);
        input[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        if (remark.Trim().Length > 0)
        {
          input[3] = new DBParameter("@Remark", DbType.String, remark);
        }
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spPLNAllocateFurForEnquiry_Edit", input, output);
        long result = DBConvert.ParseLong(output[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    private void SaveData()
    {
      if (this.CheckValid())
      {
        bool success = this.SaveMain();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.flagSearch = true;
          btnSave.Enabled = false;
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].ColHeaderLines = 2;
      if (this.flagError == 1)
      {
        e.Layout.Bands[0].Columns["ErrorCode"].Hidden = true;
      }
      else
      {
        e.Layout.Bands[0].Columns["EnDetailPid"].Hidden = true;
        e.Layout.Bands[0].Columns["FurniturePid"].Hidden = true;
        e.Layout.Bands[0].Columns["EnquiryNo"].MinWidth = 100;
        e.Layout.Bands[0].Columns["EnquiryNo"].MaxWidth = 100;
      }
      //e.Layout.Bands[0].Columns["WorkOrder"].CellAppearance.BackColor = Color.Yellow;
      //e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.Yellow;
      //e.Layout.Bands[0].Columns["Revision"].CellAppearance.BackColor = Color.Yellow;
      //e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.Yellow;

      //e.Layout.Bands[0].Columns["CARDeadline"].Header.Caption = "CAR\nDeadline";
      //e.Layout.Bands[0].Columns["CARRemark"].Header.Caption = "CAR\nRemark";
      //e.Layout.Bands[0].Columns["COM1Deadline"].Header.Caption = "COM1\nDeadline";
      //e.Layout.Bands[0].Columns["COM1Remark"].Header.Caption = "COM1\nRemark";
      //e.Layout.Bands[0].Columns["ASSHWDeadline"].Header.Caption = "ASSHW\nDeadline";
      //e.Layout.Bands[0].Columns["FFHWDeadline"].Header.Caption = "FFHW\nDeadline";
      //e.Layout.Bands[0].Columns["MATDeadline"].Header.Caption = "MAT\nDeadline";
      //e.Layout.Bands[0].Columns["ASSHWRemark"].Header.Caption = "ASSHW\nRemark";
      //e.Layout.Bands[0].Columns["FFHWRemark"].Header.Caption = "FFHW\nRemark";
      //e.Layout.Bands[0].Columns["MATRemark"].Header.Caption = "MAT\nRemark";
      //e.Layout.Bands[0].Columns["SUBDeadline"].Header.Caption = "SUB\nDeadline";
      //e.Layout.Bands[0].Columns["SUBRemark"].Header.Caption = "SUB\nRemark";
      //e.Layout.Bands[0].Columns["ASSYDeadline"].Header.Caption = "ASSY\nDeadline";
      //e.Layout.Bands[0].Columns["ASSYRemark"].Header.Caption = "ASSY\nRemark";
      //e.Layout.Bands[0].Columns["SAMDeadline"].Header.Caption = "SAM\nDeadline";
      //e.Layout.Bands[0].Columns["SAMRemark"].Header.Caption = "SAM\nRemark";
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }
    #endregion Event

    private void btnBro_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtImport.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      if (txtImport.Text.Trim() == "")
      {
        WindowUtinity.ShowMessageError("ERR0046");
        return;
      }
      this.ImportData();
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "TemplateAllocateFurnitureforEnquiry";
      string sheetName = "Sheet1 (1)";
      string outFileName = "Template Allocate Furniture for Enquiry";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      string strInFileName = string.Format(@"{0}\{1}.xls", pathTemplate, templateName);
      //XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, templateName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
      //oXlsReport.Out.File(outFileName);
      //Process.Start(outFileName);
      System.IO.File.Copy(strInFileName, strOutFileName);
      Process.Start(strOutFileName);

      //string startupPath = System.Windows.Forms.Application.StartupPath;
      //string pathOutputFile = string.Format(@"{0}\Report\", startupPath);
      //string strOutFileName = string.Format(@"{0}\{1}_{2}_{3}.xls", pathOutputFile, fileName, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.Ticks);
    }
  }
}
