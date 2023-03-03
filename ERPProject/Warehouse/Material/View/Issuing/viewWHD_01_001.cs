using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Windows.Forms;
namespace DaiCo.ERPProject
{
  public partial class viewWHD_01_001 : MainUserControl
  {
    #region Init Data
    public viewWHD_01_001()
    {
      InitializeComponent();
    }

    private void viewWHD_01_001_Load(object sender, EventArgs e)
    {
      this.LoadDropdownList();
      dtpIssueDateFrom.Value = DateTime.Today.AddDays(-7);
    }
    private void LoadDropdownList()
    {
      // 1. Create By
      Utility.LoadUltraEmployeeByDeparment(drpCreateBy, string.Empty, true);
      // 2.Status
      drpStatus.SelectedIndex = 0;
      // 3. Issue Type
      drpIssueType.SelectedIndex = 0;
      // 4. Warehouse
      this.LoadWarehouse();
      // 5. Recovery
      drpWriteOff.SelectedIndex = 0;
      // 6. Department
      Utility.LoadUltraDepartment(drpDepartment, true);
      this.LoadUltraRequestOnline();
    }

    private void LoadUltraRequestOnline()
    {
      string commandText = string.Empty;
      commandText += "  SELECT Pid, Code ";
      commandText += "  FROM TblGNRMaterialRequisitionNote";
      commandText += "  WHERE Code LIKE '%RNM%' ";
      commandText += "  ORDER BY Pid DESC ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultRequestOnline.DataSource = dtSource;
      ultRequestOnline.ValueMember = "Pid";
      ultRequestOnline.DisplayMember = "Code";
      ultRequestOnline.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultRequestOnline.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultRequestOnline.DisplayLayout.Bands[0].Columns["Code"].MinWidth = 150;
      ultRequestOnline.DisplayLayout.Bands[0].Columns["Code"].MaxWidth = 150;
    }

    /// <summary>
    /// Load Warehouse List
    /// </summary>
    private void LoadWarehouse()
    {
      Utility.LoadUltraCBMaterialWHListByUser(ucbWarehouse);
    }
    #endregion Init Data

    #region function
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      //if (ucbWarehouse.SelectedRow == null)
      //{
      //  message = "Warehouse";
      //  return false;
      //}
      return true;
    }

    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[17];
      long wo = DBConvert.ParseLong(txtWorkOrder.Text);
      if (wo != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
      }
      string department = string.Empty;
      try
      {
        department = drpDepartment.SelectedRow.Cells["Code"].Value.ToString();
      }
      catch { }
      if (department.Length > 0)
      {
        inputParam[1] = new DBParameter("@Department", DbType.AnsiString, 50, department);
      }
      string text = txtIssueNo.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[2] = new DBParameter("@IssueNo", DbType.AnsiString, 52, "%" + text + "%");
      }
      int index = drpStatus.SelectedIndex;
      if (index > 0)
      {
        inputParam[3] = new DBParameter("@ConfrimStatus", DbType.Int32, index - 1);
      }
      text = txtMaterialCode.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[4] = new DBParameter("@MaterialsCode", DbType.AnsiString, 52, "%" + text + "%");
      }
      index = drpIssueType.SelectedIndex;
      if (index > 0)
      {
        inputParam[5] = new DBParameter("@IssueType", DbType.Int32, index - 1);
      }
      if (ucbWarehouse.SelectedRow != null)
      {
        inputParam[6] = new DBParameter("@Warehouse", DbType.Int32, ucbWarehouse.Value);
      }
      index = drpWriteOff.SelectedIndex;
      if (index > 0)
      {
        inputParam[7] = new DBParameter("@WriteOff", DbType.Int32, index - 1);
      }
      DateTime issueDate = DateTime.Now;
      object datetime = dtpIssueDateFrom.Value;
      if (datetime != null)
      {
        issueDate = (DateTime)datetime;
        inputParam[8] = new DBParameter("@IssueDateFrom", DbType.DateTime, issueDate);
      }
      datetime = dtpIssueDateTo.Value;
      if (datetime != null)
      {
        issueDate = ((DateTime)datetime).AddDays(1);
        inputParam[9] = new DBParameter("@IssueDateTo", DbType.DateTime, issueDate);
      }
      double value = DBConvert.ParseDouble(txtLength.Text);
      if (value != double.MinValue)
      {
        inputParam[10] = new DBParameter("@Length", DbType.Double, value);
      }
      value = DBConvert.ParseDouble(txtWidth.Text);
      if (value != double.MinValue)
      {
        inputParam[11] = new DBParameter("@Width", DbType.Double, value);
      }
      value = DBConvert.ParseDouble(txtThickness.Text);
      if (value != double.MinValue)
      {
        inputParam[12] = new DBParameter("@Thickness", DbType.Double, value);
      }
      int createBy = int.MinValue;
      try
      {
        createBy = DBConvert.ParseInt(drpCreateBy.SelectedRow.Cells["Pid"].Value.ToString());
      }
      catch { }
      if (createBy != int.MinValue)
      {
        inputParam[13] = new DBParameter("@CreateBy", DbType.Int32, createBy);
      }
      createBy = int.MinValue;

      try
      {
        createBy = DBConvert.ParseInt(drpReceiver.SelectedRow.Cells["Pid"].Value.ToString());
      }
      catch { }
      if (createBy != int.MinValue)
      {
        inputParam[14] = new DBParameter("@Receiver", DbType.Int32, createBy);
      }

      long mrn = long.MinValue;
      try
      {
        mrn = DBConvert.ParseLong(ultRequestOnline.Value.ToString());
      }
      catch
      {
      }
      if (mrn != long.MinValue)
      {
        inputParam[15] = new DBParameter("@MRN", DbType.Int64, mrn);
      }
      inputParam[16] = new DBParameter("@EmpID", DbType.Int32, SharedObject.UserInfo.UserPid);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWHDListIssuingNote_Materials", 240, inputParam);
      ultData.DataSource = dtSource;
    }
    #endregion function

    #region Events
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      string message;
      bool success = this.CheckValid(out message);
      if (success)
      {
        this.Search();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
      }
      btnSearch.Enabled = true;
    }

    private void drpDepartment_TextChanged(object sender, EventArgs e)
    {
      string department = string.Empty;
      try
      {
        department = drpDepartment.Value.ToString();
      }
      catch { }
      Utility.LoadUltraEmployeeByDeparment(drpReceiver, department, true);
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
      e.Layout.Bands[0].Columns["Issue No"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["Issue No"].MinWidth = 120;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Issue Date"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Issue Date"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Issued By"].Width = 150;
      e.Layout.Bands[0].Columns["Receipt By"].Width = 200;
      e.Layout.Bands[0].Columns["Type"].Hidden = true;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
    }

    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        string issueNo = ultData.Selected.Rows[0].Cells["Issue No"].Value.ToString();
        string Prefix = issueNo.Substring(0, 5);
        int type = DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["Type"].Value.ToString());
        MainUserControl view = null;
        if (issueNo.Length > 0)
        {
          if (type == 0)
          {
            view = new viewWHD_02_007();
            ((viewWHD_02_007)view).issueNo = issueNo;
            WindowUtinity.ShowView(view, "TRANSFER TO WIP WAREHOUSE", true, ViewState.MainWindow);
          }
          if (type == 1)
          {
            if (Prefix == "03ITR")
            {
              view = new viewWHD_02_006();
              ((viewWHD_02_006)view).issueNo = issueNo;
              WindowUtinity.ShowView(view, "ISSUING TO NON STOCK", true, ViewState.MainWindow);
            }
            else
            {
              view = new viewWHD_02_005();
              ((viewWHD_02_005)view).issueNo = issueNo;
              WindowUtinity.ShowView(view, "ISSUING NOTE", true, ViewState.MainWindow);
            }
          }
          else if (type == 2 || type == 4)
          {
            view = new viewWHD_02_003();
            ((viewWHD_02_003)view).issueNo = issueNo;
            WindowUtinity.ShowView(view, "RETURN TO SUPPLIER", true, ViewState.MainWindow);
          }
          else if (type == 3)
          {
            view = new viewWHD_02_004();
            ((viewWHD_02_004)view).issueNo = issueNo;
            WindowUtinity.ShowView(view, "ADJSUTMENT OUT", true, ViewState.MainWindow);
          }
        }
      }
      catch { }
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    private void btnNew_Click(object sender, EventArgs e)
    {
      if (radIssueToProduction.Checked)
      {
        viewWHD_02_005 view = new viewWHD_02_005();
        //viewWHD_02_001 view = new viewWHD_02_001();
        view.issueNo = string.Empty;
        WindowUtinity.ShowView(view, "ISSUING NOTE", true, ViewState.MainWindow);
      }
      else if (radReturnToSupplier.Checked)
      {
        viewWHD_02_003 view = new viewWHD_02_003();
        view.issueNo = string.Empty;
        WindowUtinity.ShowView(view, "RETURN TO SUPPLIER", true, ViewState.MainWindow);
      }
      else if (radAdjustment.Checked)
      {
        viewWHD_02_004 view = new viewWHD_02_004();
        view.issueNo = string.Empty;
        WindowUtinity.ShowView(view, "ADJSUTMENT OUT", true, ViewState.MainWindow);
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtWorkOrder.Text = string.Empty;
      drpCreateBy.Value = null;
      txtIssueNo.Text = string.Empty;
      drpStatus.SelectedIndex = 0;
      txtMaterialCode.Text = string.Empty;
      drpIssueType.SelectedIndex = 0;
      drpWriteOff.SelectedIndex = 0;
      drpDepartment.Value = null;
      drpReceiver.Value = null;
      txtLength.Text = string.Empty;
      txtWidth.Text = string.Empty;
      txtThickness.Text = string.Empty;

      dtpIssueDateFrom.Value = DateTime.Today.AddDays(-7);
      dtpIssueDateTo.Value = DateTime.Today;
    }
    #endregion Events
  }
}
