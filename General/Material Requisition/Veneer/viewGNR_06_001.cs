/*
  Author      : 
  Date        : 12/06/2013
  Description : List Requisiton Veneer
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
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using System.IO;
using System.Diagnostics;

namespace DaiCo.General
{
  public partial class viewGNR_06_001 : MainUserControl
  {
    #region Field
    //private int wo = int.MinValue;
    #endregion Field

    #region Init
    public viewGNR_06_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_06_001_Load(object sender, EventArgs e)
    {
      drpDateFrom.Value = DateTime.Today.AddDays(-7);

      // Load Init
      this.LoadInit();
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {

      // Load UltraCombo Status
      this.LoadComboStatus();

      // Load UltraCombo Create By
      this.LoadComboCreateBy();

      // Load UltraCombo Department
      this.LoadComboDepartment();

      // Load WO
      this.LoadWO();

      //Load Carcass
      this.LoadCarcass(int.MinValue);
      //Load Confirmed Status
      this.LoadDDConfirmedStatus();

      //Load Reason
      this.LoadDDReason();

      this.ultCBCreateBy.Value = SharedObject.UserInfo.UserPid;
      this.ultStatus.Value = 0;

      // Search
      this.Search();
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Resigned = 0";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if(dtSource == null)
      {
        return;
      }
      ultCBCreateBy.DataSource = dtSource;
      ultCBCreateBy.DisplayMember = "Name";
      ultCBCreateBy.ValueMember = "ID_NhanVien";
      ultCBCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBCreateBy.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBCreateBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }
    private void LoadDDConfirmedStatus()
    {
      string commandText = @"SELECT Code,Value FROM  TblBOMCodeMaster WHERE    [Group] = 16037";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Shared.Utility.ControlUtility.LoadUltraDropDown(ultDDConfirmedStatus, dtSource, "Code", "Value", "Code");
    }
    private void LoadDDReason()
    {
      string commandText = @"SELECT Code,Value FROM  TblBOMCodeMaster WHERE    [Group] = 16038";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Shared.Utility.ControlUtility.LoadUltraDropDown(ultDDReason, dtSource, "Code", "Value", "Code");
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = string.Empty;
      commandText += " SELECT Department, Department + ' - ' + DeparmentName AS DeparmentName FROM VHRDDepartment";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBDepartment.DataSource = dtSource;
      ultCBDepartment.DisplayMember = "DeparmentName";
      ultCBDepartment.ValueMember = "Department";
      ultCBDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["DeparmentName"].Width = 150;
      ultCBDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;

      // Load Defaul Department User Login
      ultCBDepartment.Value = SharedObject.UserInfo.Department;
    }

    /// <summary>
    /// Load UltraCombo Status (0: New / 1: Confirm / 2: Finished)
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Finished' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if(dtSource == null)
      {
        return;
      }
      ultStatus.DataSource = dtSource;
      ultStatus.DisplayMember = "Name";
      ultStatus.ValueMember = "ID";
      ultStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultStatus.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }

    /// <summary>
    /// Load WO
    /// </summary>
    private void LoadWO()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Confirm = 1 and Status = 0 ORDER BY Pid DESC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }
      ultCBWO.DataSource = dt;
      ultCBWO.DisplayMember = "Pid";
      ultCBWO.ValueMember = "Pid";
      ultCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Carcass
    /// </summary>
    /// <param name="wo"></param>
    private void LoadCarcass(int wo)
    {
      string commandText = string.Empty;
      if (ultCBWO.SelectedRow != null)
      {
        commandText = string.Format(@"SELECT Distinct CarcassCode
                           FROM VPLNWorkOrderCarcassList  info
                           WHERE info.WoPid = {0}", wo);
      }
      else
      {
        commandText = string.Format(@"SELECT Distinct CarcassCode
                           FROM VPLNWorkOrderCarcassList ");
      }
      DataTable dt = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }
      ultCBCarcass.DataSource = dt;
      ultCBCarcass.ValueMember = "CarcassCode";
      ultCBCarcass.DisplayMember = "CarcassCode";
      ultCBCarcass.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[10];
      // Request From
      if(txtRequestFrom.Text.Length > 0)
      {
        input[0] = new DBParameter("@RESNoFrom", DbType.String, txtRequestFrom.Text.Trim());
      }
      // Request To
      if (txtRequestTo.Text.Length > 0)
      {
        input[1] = new DBParameter("@RESNoTo", DbType.String, txtRequestTo.Text.Trim());
      }
      // DateFrom
      if (drpDateFrom.Value != null)
      {
        DateTime prDateFrom = DateTime.MinValue;
        prDateFrom = (DateTime)drpDateFrom.Value;
        input[2] = new DBParameter("@DateFrom", DbType.DateTime, prDateFrom);
      }
      // DateTo
      if (drpDateTo.Value != null)
      {
        DateTime prDateTo = DateTime.MinValue;
        prDateTo = (DateTime)drpDateTo.Value;
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        input[3] = new DBParameter("@DateTo", DbType.DateTime, prDateTo);
      }
      // CreateBy
      if (ultCBCreateBy.Value != null)
      {
        input[4] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(ultCBCreateBy.Value.ToString()));
      }
      // Status
      if (ultStatus.Value != null &&  DBConvert.ParseInt(ultStatus.Value.ToString()) != int.MinValue)
      {
        input[5] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultStatus.Value.ToString()));
      }
      // WO
      if(ultCBWO.Value != null)
      {
        input[6] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }
      // Carcas
      if (ultCBCarcass.Value != null)
      {
        input[7] = new DBParameter("@Carcass", DbType.String, ultCBCarcass.Value.ToString());
      }
      // Material
      if (txtMaterial.Text.Trim().Length > 0)
      {
        input[8] = new DBParameter("@Material", DbType.String, txtMaterial.Text.Trim());
      }
      // Department Request
      if(ultCBDepartment.Value != null)
      {
        input[9] = new DBParameter("@Department", DbType.String, ultCBDepartment.Value.ToString());
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRVeneerListRequisitionNote_Select", input);
      if (ds != null)
      {
        DataSet dsSource = this.CreateDataSet();
        dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
        dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
        ultData.DataSource = dsSource;

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          string status = ultData.Rows[i].Cells["Status"].Value.ToString();
          UltraGridRow gr = ultData.Rows[i];
          if (string.Compare(gr.Cells["Status"].Value.ToString(), "Confirmed", true) == 0)
          {
            ultData.Rows[i].Appearance.BackColor = Color.Pink;
            for (int j = 0; j < gr.ChildBands[0].Rows.Count; j++)
            {
              gr.ChildBands[0].Rows[j].Cells["WO"].Activation = Activation.ActivateOnly;
              gr.ChildBands[0].Rows[j].Cells["Carcass"].Activation = Activation.ActivateOnly;
              gr.ChildBands[0].Rows[j].Cells["MaterialCode"].Activation = Activation.ActivateOnly;
              gr.ChildBands[0].Rows[j].Cells["MaterialNameVn"].Activation = Activation.ActivateOnly;
              gr.ChildBands[0].Rows[j].Cells["Unit"].Activation = Activation.ActivateOnly;
              gr.ChildBands[0].Rows[j].Cells["QtyRequest"].Activation = Activation.ActivateOnly;
              gr.ChildBands[0].Rows[j].Cells["QtyIssued"].Activation = Activation.ActivateOnly;

            }
          }
          else
          {
            for (int j = 0; j < gr.ChildBands[0].Rows.Count; j++)
            {
              gr.ChildBands[0].Rows[j].Activation = Activation.ActivateOnly;
            }
            if (string.Compare(gr.Cells["Status"].Value.ToString(), "Finished", true) == 0)
            {
              ultData.Rows[i].Appearance.BackColor = Color.LightGreen;
            }
          }
        }
      }
      // Enable button search
      btnSearch.Enabled = true;
    }

     private bool CheckValid(out String message)
    {
       message = string.Empty;
       DataTable dt = ((DataSet)ultData.DataSource).Tables[1];
       DataTable dtcheck;
       string conmand = string.Empty;
       for (int i = 0; i < dt.Rows.Count; i++)
       {
         DataRow dr = dt.Rows[i];
         if (dt.Rows[i].RowState == DataRowState.Modified)
         {
           if (DBConvert.ParseInt(dr["StatusConfirmed"].ToString()) != int.MinValue)
           {
             conmand = string.Format(@"SELECT Code FROM  TblBOMCodeMaster WHERE [Group] = 16037 AND code = {0}",DBConvert.ParseInt(dr["StatusConfirmed"].ToString()));
             dtcheck = DataBaseAccess.SearchCommandTextDataTable(conmand);
             if (dtcheck.Rows.Count == 0)
             {
               message = "StatusConfirmed";
               return false;               
             }
           }
           if (DBConvert.ParseInt(dr["Reason"].ToString()) != int.MinValue)
           {
             conmand = string.Format(@"SELECT Code FROM  TblBOMCodeMaster WHERE [Group] = 16038 AND code = {0}", DBConvert.ParseInt(dr["Reason"].ToString()));
             dtcheck = DataBaseAccess.SearchCommandTextDataTable(conmand);            
             if (dtcheck.Rows.Count == 0)
             {
               message = "Reason";
               return false;
             }
           }
         }
       }
       return true;
     }
    private bool SaveData()
    {
      bool resutl = true;
      DataTable dt = ((DataSet)ultData.DataSource).Tables[1];
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dr = dt.Rows[i];
        if (dr.RowState == DataRowState.Modified)
        {
          string storeName = "spGNRVeneerListRequisitionNote_Edit";
          DBParameter[] inputParam = new DBParameter[4];
          if (DBConvert.ParseLong(dr["PidDetail"].ToString()) != long.MinValue)
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dr["PidDetail"].ToString()));
            if (DBConvert.ParseInt(dr["StatusConfirmed"].ToString()) != int.MinValue)
            {
              inputParam[1] = new DBParameter("@StatusConfirm", DbType.Int32, DBConvert.ParseInt(dr["StatusConfirmed"].ToString()));
            }
            if (DBConvert.ParseInt(dr["Reason"].ToString()) != int.MinValue)
            {
              inputParam[2] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(dr["Reason"].ToString()));
            }
            if (dr["Remark"].ToString().Length > 0)
            {
              inputParam[3] = new DBParameter("@Remark", DbType.String, dr["Remark"].ToString());
            }
            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            if (outputParam == null || DBConvert.ParseLong(outputParam[0].Value.ToString()) <= 0)
            {
              resutl = false;
            }
          }
        }
      }
      return resutl;
    }

    /// <summary>
    /// Enter Search
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Create DataSet
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("RequestNo", typeof(System.String));
      taParent.Columns.Add("DepartmentRequest", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("PidDetail", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("Carcass", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("MaterialNameVn", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("QtyRequest", typeof(System.Double));
      taChild.Columns.Add("QtyIssued", typeof(System.Double));
      taChild.Columns.Add("StatusConfirmed", typeof(System.Int32));
      taChild.Columns.Add("Reason", typeof(System.Int32));
      taChild.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["Pid"], false));
      return ds;
    }

    private void ExportData()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "Deadline", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "Deadline";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
      }
    }

    #endregion Function

    #region Event

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      txtRequestFrom.Text = string.Empty;
      txtRequestTo.Text = string.Empty;
      ultStatus.Text = string.Empty;
      ultCBCreateBy.Text = string.Empty;
      ultCBWO.Text = string.Empty;
      ultCBCarcass.Text = string.Empty;
      ultCBDepartment.Text = string.Empty;
      txtMaterial.Text = string.Empty;
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["PidDetail"].Hidden = true;

      e.Layout.Bands[0].Columns["RequestNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["DepartmentRequest"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["Reason"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[1].Columns["StatusConfirmed"].CellAppearance.BackColor = Color.LightGray;
      e.Layout.Bands[1].Columns["Remark"].CellAppearance.BackColor = Color.LightGray;

      //e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      //e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      //e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[1].Columns["QtyRequest"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["QtyIssued"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["StatusConfirmed"].ValueList = this.ultDDConfirmedStatus;
      e.Layout.Bands[1].Columns["Reason"].ValueList = this.ultDDReason;
 
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      string departmentRequest = row.Cells["DepartmentRequest"].Value.ToString();
      string departmentLogin = SharedObject.UserInfo.Department;
      string status = row.Cells["Status"].Value.ToString();

      if(string.Compare(departmentRequest, departmentLogin, true) == 0)
      {
        viewGNR_06_002 uc = new viewGNR_06_002();
        uc.veneerRequisitionPid = pid;
        WindowUtinity.ShowView(uc, "UPDATE REQUISITION VENEER", false, ViewState.MainWindow);
      }
      else if(string.Compare(status, "Confirmed", true) == 0 ||
              string.Compare(status, "Finished", true) == 0)
      {
        viewGNR_06_002 uc = new viewGNR_06_002();
        uc.veneerRequisitionPid = pid;
        WindowUtinity.ShowView(uc, "UPDATE REQUISITION VENEER", false, ViewState.MainWindow);
      }
    }

    /// <summary>
    /// WO Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBWO.Value != null && ultCBWO.Text.ToString().Length > 0)
      {
        ultCBCarcass.Value = null;
        this.LoadCarcass(DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }
      else
      {
        ultCBCarcass.Value = null;
      }
    }


    /// <summary>
    /// New Requisition Special
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewGNR_06_002 view = new viewGNR_06_002();
      WindowUtinity.ShowView(view, "NEW REQUISITION VENEER ONLINE ", true, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    /// <summary>
    /// Finished
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFinished_Click(object sender, EventArgs e)
    {
      int count = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          count = count + 1;
          if (count > 1)
          {
            WindowUtinity.ShowMessageError("ERR0114", "row");
            return;
          }
        }
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@VRNPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spGNRVeneerRequisitionNoteStatus_Update", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageError("ERRO121", row.Cells["RequestNo"].Value.ToString(), "was not Finished");
          }
          else
          {
            WindowUtinity.ShowMessageSuccess("MSG0051", row.Cells["RequestNo"].Value.ToString());
          }
        }
      }
      this.Search();
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
    private void btnSave_Click(object sender, EventArgs e)
    {
      // 1: Check Valid
      string message = string.Empty;
      bool success = this.CheckValid(out message);
      if (success)
      {
        success = this.SaveData();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0037", "Data");
        }
        this.Search();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
    }
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {   
    }
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Value.ToString().Trim();
      string commandText = string.Empty;
      DataTable dtCheck;
      switch (columnName.ToLower())
      {
        case "statusconfirmed":
          if (text.Length > 0)
          {
            commandText = string.Format(@"SELECT Code FROM  TblBOMCodeMaster WHERE [Group] = 16037 AND code = {0}", DBConvert.ParseInt(text));
            dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck.Rows.Count == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Status Confirmed");
              e.Cell.Row.Cells["StatusConfirmed"].Value = DBNull.Value;
            }
          }
          break;
        case "reason":
          if (text.Length > 0)
          {
            commandText = string.Format(@"SELECT Code FROM  TblBOMCodeMaster WHERE [Group] = 16038 AND code = {0}", DBConvert.ParseInt(text));
            dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck.Rows.Count == 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "reason");
              e.Cell.Row.Cells["Reason"].Value = DBNull.Value;
            }
          }
          break;
        default:
          break;
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      this.ExportData();
    }

    #endregion Event



   

   

   
  }
}
