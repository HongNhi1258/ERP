/*
  Author      : 
  Date        : 20/04/2011
  Description : List PR
  Truong      : Update print
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using DaiCo.ERPProject.Purchasing;
using DaiCo.ERPProject.Purchasing.DataSetSource;
using DaiCo.ERPProject.Purchasing.Reports;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_03_003 : MainUserControl
  {
    #region Field
    bool flagviewPrice = false;
    #endregion Field

    #region Init Data
    /// <summary>
    /// List PR
    /// </summary>
    public viewPUR_03_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_03_003_Load(object sender, EventArgs e)
    {
      drpDateFrom.Value = DBNull.Value;
      drpDateTo.Value = DBNull.Value;

      this.LoadUltraEmployee(ultCreateBy);
      this.LoadUltraEmployee(ultRequester);
      this.LoadUltraComboProjectCode();
      this.LoadUltraComboHeaderDepartment();
      this.LoadUltraComboGroupInCharge();
      this.LoadUltraComboStatusPR();
      this.LoadUltraComboCodeMst(ultUrgentLevel, 7007);
      this.LoadUltraComboDepartment();
      this.txtPrNo.Text = "PROL-";

      if (btnPerPrice.Visible)
      {
        this.flagviewPrice = true;
        btnPerPrice.Visible = false;
      }
      else
      {
        this.flagviewPrice = false;
      }
    }

    private void LoadUltraComboProjectCode()
    {
      string commandText = string.Format(@" SELECT  Pid Code, ProjectCode Name 
                                            FROM    TblPURPRProjectCode 
                                            WHERE   ISNULL(Finished, 0) = 0 AND ISNULL([Status], 0) = 1");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultProjectCode.DataSource = dtSource;
      ultProjectCode.DisplayMember = "Name";
      ultProjectCode.ValueMember = "Code";
      ultProjectCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultProjectCode.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultProjectCode.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }
    #endregion Init Data

    #region LoadData

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadUltraComboDepartment()
    {
      string commandText = "SELECT Department, Department + ' - ' + DeparmentName DepartmentName FROM VHRDDepartment";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);

      ultDepartment.DataSource = dtSource;
      ultDepartment.DisplayMember = "DepartmentName";
      ultDepartment.ValueMember = "Department";
      ultDepartment.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDepartment.DisplayLayout.Bands[0].Columns["Department"].Hidden = true;
      ultDepartment.DisplayLayout.Bands[0].Columns["DepartmentName"].Width = 300;
    }

    /// <summary>
    /// Load UltraCombo Employee
    /// </summary>
    private void LoadUltraEmployee(UltraCombo ultCombo)
    {
      //string commandText = string.Format("SELECT Pid, CAST(Pid as varchar) + ' - ' + EmpName Description FROM VHRMEmployee ORDER BY Pid");
      string commandText = string.Format("SELECT ID_NhanVien Pid, TenNV + ' ' + HoNV + ' - ' + CAST(ID_NhanVien AS VARCHAR) Description FROM VHRNhanVien WHERE Resigned = 0 ORDER BY Description");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);

      ultCombo.DataSource = dtSource;
      ultCombo.ValueMember = "Pid";
      ultCombo.DisplayMember = "Description";
      ultCombo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCombo.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCombo.DisplayLayout.Bands[0].Columns["Description"].Width = 300;
    }

    /// <summary>
    /// Load UltraCombo Group In Charge
    /// </summary>
    private void LoadUltraComboGroupInCharge()
    {
      string commandText = "SELECT Pid, GroupName FROM TblPURStaffGroup WHERE DeleteFlg = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);

      ultPurchaserGroup.DataSource = dtSource;
      ultPurchaserGroup.DisplayMember = "GroupName";
      ultPurchaserGroup.ValueMember = "Pid";
      ultPurchaserGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultPurchaserGroup.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultPurchaserGroup.DisplayLayout.Bands[0].Columns["GroupName"].Width = 300;

      // Load Group In Charge(User Login)
      string commmadText = string.Format(@" SELECT SG.Pid
                                                FROM TblPURStaffGroup SG
	                                                LEFT JOIN TblPURStaffGroupDetail SGD ON SG.Pid = SGD.[Group]
                                                WHERE SG.DeleteFlg = 0 AND (SG.LeaderGroup = {0} OR SGD.Employee = {1})", SharedObject.UserInfo.UserPid, SharedObject.UserInfo.UserPid);
      DataTable dtGroupInCharge = DataBaseAccess.SearchCommandTextDataTable(commmadText);
      if (dtGroupInCharge != null && dtGroupInCharge.Rows.Count > 0)
      {
        ultPurchaserGroup.Value = DBConvert.ParseInt(dtGroupInCharge.Rows[0]["Pid"].ToString());
      }
    }

    private void LoadUltraComboStatusPR()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Waiting For Approval' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'Ready To Make PO' Name";
      commandText += " UNION";
      commandText += " SELECT 4 ID, 'Ready To Make PO A Part' Name";
      commandText += " UNION";
      commandText += " SELECT 5 ID, 'Ready To Make PO (ALL)' Name";
      commandText += " UNION";
      commandText += " SELECT 6 ID, 'Waiting For Receiving' Name";
      commandText += " UNION";
      commandText += " SELECT 7 ID, 'Received A Part' Name";
      commandText += " UNION";
      commandText += " SELECT 8 ID, 'Finished' Name";
      commandText += " UNION";
      commandText += " SELECT 9 ID, 'Cancel' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultStatus.DataSource = dtSource;
      ultStatus.DisplayMember = "Name";
      ultStatus.ValueMember = "ID";
      ultStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultStatus.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;

    }
    /// <summary>
    /// Load Header Department
    /// </summary>
    private void LoadUltraComboHeaderDepartment()
    {
      string commandText = "SELECT DISTINCT Manager, CAST(Dept.Manager as varchar) + ' - ' + Emp.EmpName Description FROM VHRDDepartmentInfo Dept INNER JOIN VHRMEmployee Emp ON (Dept.Manager = Emp.Pid)";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);

      ultApprovedBy.DataSource = dtSource;
      ultApprovedBy.DisplayMember = "Description";
      ultApprovedBy.ValueMember = "Manager";
      ultApprovedBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultApprovedBy.DisplayLayout.Bands[0].Columns["Manager"].Hidden = true;
      ultApprovedBy.DisplayLayout.Bands[0].Columns["Description"].Width = 300;
    }

    /// <summary>
    /// Load UltraCombo Code Master
    /// </summary>
    /// <param name="ultCombo"></param>
    /// <param name="group"></param>
    private void LoadUltraComboCodeMst(UltraCombo ultCombo, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCombo.DataSource = dtSource;
      ultCombo.ValueMember = "Code";
      ultCombo.DisplayMember = "Value";
      ultCombo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCombo.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCombo.DisplayLayout.Bands[0].Columns["Value"].Width = 300;
    }

    /// <summary>
    /// Search depend condition 
    /// </summary>
    private void Search()
    {
      try
      {
        int createBy = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultCreateBy));
        int approvedBy = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultApprovedBy));
        string dept = Utility.GetSelectedValueUltraCombobox(ultDepartment);
        int requester = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultRequester));
        string prNo = txtPrNo.Text.Trim();

        int projectPid = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultProjectCode));
        string dateFrom = (drpDateFrom.Value == null) ? string.Empty : drpDateFrom.Value.ToString().Trim();
        DateTime prDateFrom = DateTime.MinValue;
        if (dateFrom.Length > 0)
        {
          prDateFrom = (DateTime)drpDateFrom.Value;
        }
        string dateTo = (drpDateTo.Value == null) ? string.Empty : drpDateTo.Value.ToString().Trim();
        DateTime prDateTo = DateTime.MinValue;
        if (dateTo.Length > 0)
        {
          prDateTo = (DateTime)drpDateTo.Value;
        }
        string material = txtItemCodeName.Text.Trim();
        long groupInCharge = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultPurchaserGroup));
        int prStatus = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultStatus));
        //int prDetailStatus = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultStatusDetail));
        int urgent = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultUrgentLevel));
        double itemValueFrom = DBConvert.ParseDouble(txtItemValueFrom.Text.Trim());
        double itemValueTo = DBConvert.ParseDouble(txtItemValueTo.Text.Trim());
        double prValueFrom = DBConvert.ParseDouble(txtPrValueFrom.Text.Trim());
        double prValueTo = DBConvert.ParseDouble(txtPrValueTo.Text.Trim());

        DBParameter[] input = new DBParameter[16];
        if (createBy != int.MinValue)
        {
          input[0] = new DBParameter("@CreateBy", DbType.Int32, createBy);
        }
        else
        {
          string text = ultCreateBy.Text.Trim();
          if (text.Length > 0)
          {
            input[0] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(text));
          }
        }

        if (approvedBy != int.MinValue)
        {
          input[1] = new DBParameter("@ApprovedBy", DbType.Int32, approvedBy);
        }
        else
        {
          string text = ultApprovedBy.Text.Trim();
          if (text.Length > 0)
          {
            input[1] = new DBParameter("@ApprovedBy", DbType.Int32, DBConvert.ParseInt(text));
          }
        }

        if (dept.Length > 0)
        {
          input[2] = new DBParameter("@Department", DbType.AnsiString, 48, dept);
        }

        if (requester != int.MinValue)
        {
          input[3] = new DBParameter("@RequestBy", DbType.Int32, requester);
        }
        else
        {
          string text = ultRequester.Text.Trim();
          if (text.Length > 0)
          {
            input[3] = new DBParameter("@RequestBy", DbType.Int32, DBConvert.ParseInt(text));
          }
        }

        if (prNo.Length > 0)
        {
          input[4] = new DBParameter("@PRNo", DbType.AnsiString, 4000, prNo);
        }

        if (projectPid != int.MinValue)
        {
          input[5] = new DBParameter("@ProjectPid", DbType.Int32, projectPid);
        }
        else
        {
          string text = ultProjectCode.Text.Trim();
          if (text.Length > 0)
          {
            input[5] = new DBParameter("@ProjectPid", DbType.Int32, DBConvert.ParseInt(text));
          }
        }

        if (prDateFrom != DateTime.MinValue)
        {
          input[6] = new DBParameter("@PRDateFrom", DbType.DateTime, prDateFrom);
        }

        if (prDateTo != DateTime.MinValue)
        {
          prDateTo = (prDateTo != DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
          input[7] = new DBParameter("@PRDateTo", DbType.DateTime, prDateTo);
        }

        if (material.Length > 0)
        {
          input[8] = new DBParameter("@Material", DbType.String, 256, material);
        }

        if (groupInCharge != long.MinValue)
        {
          input[9] = new DBParameter("@GroupInCharge", DbType.Int64, groupInCharge);
        }
        else
        {
          string text = ultPurchaserGroup.Text.Trim();
          if (text.Length > 0)
          {
            input[9] = new DBParameter("@GroupInCharge", DbType.Int64, DBConvert.ParseLong(text));
          }
        }

        if (prStatus != int.MinValue)
        {
          input[10] = new DBParameter("@StatusPR", DbType.Int32, prStatus);
        }
        else
        {
          string text = ultStatus.Text.Trim();
          if (text.Length > 0)
          {
            input[10] = new DBParameter("@StatusPR", DbType.Int32, DBConvert.ParseInt(text));
          }
        }

        if (urgent != int.MinValue)
        {
          input[11] = new DBParameter("@Urgent", DbType.Int32, urgent);
        }
        else
        {
          string text = ultUrgentLevel.Text.Trim();
          if (text.Length > 0)
          {
            input[11] = new DBParameter("@Urgent", DbType.Int32, DBConvert.ParseInt(text));
          }
        }

        if (itemValueFrom != double.MinValue)
        {
          input[12] = new DBParameter("@ItemValueFrom", DbType.Double, itemValueFrom);
        }

        if (itemValueTo != double.MinValue)
        {
          input[13] = new DBParameter("@ItemValueTo", DbType.Double, itemValueTo);
        }

        if (prValueFrom != double.MinValue)
        {
          input[14] = new DBParameter("@PRValueFrom", DbType.Double, prValueFrom);
        }

        if (prValueTo != double.MinValue)
        {
          input[15] = new DBParameter("@PRValueTo", DbType.Double, prValueTo);
        }

        DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURListPR_Select", 300, input);
        DataSet dsList = this.ListPR();
        dsList.Tables["TblPR"].Merge(ds.Tables[0]);
        dsList.Tables["TblPRDetail"].Merge(ds.Tables[1]);
        dsList.Tables["TblPRReceiving"].Merge(ds.Tables[2]);
        ultData.DataSource = dsList;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          int type = DBConvert.ParseInt(ultData.Rows[i].Cells["PRStatus"].Value.ToString());

          if (type == 5 || type == 6)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
          }
          else if (type == 7 || type == 8)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Pink;
          }
          else if (type == 9 || type == 10)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
          }
          else if (type == 11)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.LightSkyBlue;
          }
          else if (type == 12)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Gray;
          }

          for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow childRow = ultData.Rows[i].ChildBands[0].Rows[j];
            if (DBConvert.ParseInt(childRow.Cells["PRDTStatus"].Value.ToString()) >= 10)
            {
              childRow.Cells["Note"].Value = "";
            }

            if (DBConvert.ParseInt(childRow.Cells["Flag"].Value.ToString()) == 1)
            {
              childRow.Cells["Note"].Appearance.ForeColor = Color.Red;
            }
            else if (DBConvert.ParseInt(childRow.Cells["Flag"].Value.ToString()) == 2)
            {
              childRow.Cells["Note"].Appearance.ForeColor = Color.Orange;
            }
            else if (DBConvert.ParseInt(childRow.Cells["Flag"].Value.ToString()) == 3)
            {
              childRow.Cells["Note"].Appearance.ForeColor = Color.Blue;
            }
          }
          ultData.Rows[i].Expanded = true;
        }
        chkExpandAll.Checked = false;
      }
      catch
      {
        this.Search();
      }
    }

    private DataSet ListPR()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblPR");
      taParent.Columns.Add("Selected", typeof(System.Int32));
      taParent.Columns.Add("PRNo", typeof(System.String));
      taParent.Columns.Add("DeparmentName", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("TotalAmountValue", typeof(System.Double));
      taParent.Columns.Add("TypeOfRequest", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("PurposeOfRequisition", typeof(System.String));
      taParent.Columns.Add("HeadDepartmentApproved", typeof(System.String));
      taParent.Columns.Add("RequestBy", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblPRDetail");
      taChild.Columns.Add("PRDetailPid", typeof(System.Int64));
      taChild.Columns.Add("Selected", typeof(System.Int32));
      taChild.Columns.Add("Flag", typeof(System.Int32));
      taChild.Columns.Add("Note", typeof(System.String));
      taChild.Columns.Add("PONo", typeof(System.String));
      taChild.Columns.Add("PRNo", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("NameEN", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("Status", typeof(System.String));
      taChild.Columns.Add("Quantity", typeof(System.Double));
      taChild.Columns.Add("QtyCancel", typeof(System.Double));
      taChild.Columns.Add("Price", typeof(System.Double));
      taChild.Columns.Add("Currency", typeof(System.String));
      taChild.Columns.Add("Urgent", typeof(System.String));
      taChild.Columns.Add("RequestDate", typeof(System.String));
      taChild.Columns.Add("ExpectedBrand", typeof(System.String));
      taChild.Columns.Add("VAT", typeof(System.String));
      taChild.Columns.Add("Imported", typeof(System.String));
      taChild.Columns.Add("ProjectCode", typeof(System.String));
      taChild.Columns.Add("GroupInCharge", typeof(System.Int64));
      taChild.Columns.Add("GroupName", typeof(System.String));
      taChild.Columns.Add("ConfirmBy", typeof(System.String));
      taChild.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taChild);

      // Receiving
      DataTable taRec = new DataTable("TblPRReceiving");
      taRec.Columns.Add("PRDetailPid", typeof(System.Int64));
      taRec.Columns.Add("ReceivingNote", typeof(System.String));
      taRec.Columns.Add("Qty", typeof(System.Double));
      taRec.Columns.Add("CreateDate", typeof(System.String));
      ds.Tables.Add(taRec);

      ds.Relations.Add(new DataRelation("TblPR_TblPRDetail", taParent.Columns["PRNo"], taChild.Columns["PRNo"], false));
      ds.Relations.Add(new DataRelation("TblPRDetail_TblPRReceiving", taChild.Columns["PRDetailPid"], taRec.Columns["PRDetailPid"], false));
      return ds;
    }

    /// <summary>
    /// Format Numeric
    /// </summary>
    /// <param name="number"></param>
    /// <param name="phanLe"></param>
    /// <returns></returns>
    private string NumericFormat(double number, int phanLe)
    {
      if (number == double.MinValue)
      {
        return string.Empty;
      }
      if (phanLe < 0)
      {
        return number.ToString();
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      double t = Math.Truncate(number);
      formatInfo.NumberDecimalDigits = phanLe;
      return number.ToString("N", formatInfo);
    }

    private void PrintReport(string prNo)
    {
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@PRNo", DbType.AnsiString, 16, prNo);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPurchaseRequisition_Select", input);
      if (ds != null)
      {
        dsPURPurchaseRequisition dsSource = new dsPURPurchaseRequisition();
        dsSource.Tables["dtPurchaseRequisitionInfo"].Merge(ds.Tables[0]);
        dsSource.Tables["dtPurchaseRequisitionDetail"].Merge(ds.Tables[1]);
        dsSource.Tables["dtPRDetailSubReport"].Merge(ds.Tables[2]);


        double totalAmount = 0;
        double totalAmountUSD = 0;
        double currentExchangeRate = 1;
        foreach (DataRow row in dsSource.Tables["dtPurchaseRequisitionDetail"].Rows)
        {
          totalAmount += DBConvert.ParseDouble(row["Amount"].ToString());
        }
        string commadText = "SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Code = 'USD'";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commadText);
        if (dt != null)
        {
          currentExchangeRate = DBConvert.ParseDouble(dt.Rows[0]["CurrentExchangeRate"].ToString());
          totalAmountUSD = totalAmount / currentExchangeRate;
        }
        DaiCo.Shared.View_Report report = null;
        cptPurchaseRequisition cpt = new cptPurchaseRequisition();
        if (ds.Tables[3] != null && ds.Tables[3].Rows.Count > 0)
        {
          dsSource.Tables["dtAdditionPrice"].Merge(ds.Tables[3]);
          for (int i = 0; i < dsSource.Tables["dtAdditionPrice"].Rows.Count; i++)
          {
            totalAmount = totalAmount + DBConvert.ParseDouble(dsSource.Tables["dtAdditionPrice"].Rows[i]["Amount"].ToString());
          }
          totalAmountUSD = totalAmount / currentExchangeRate;
          cpt.Section4.SectionFormat.EnableSuppress = false;
        }
        else
        {
          cpt.Section4.SectionFormat.EnableSuppress = true;
        }
        cpt.SetDataSource(dsSource);
        cpt.SetParameterValue("paramTotalAmount", totalAmount);
        cpt.SetParameterValue("paramTotalAmountUSD", totalAmountUSD);
        report = new DaiCo.Shared.View_Report(cpt);
        report.IsShowGroupTree = false;
        report.ShowReport(Shared.Utility.ViewState.MainWindow);
      }
    }

    /// <summary>
    /// PrintPR
    /// </summary>
    private void PrintPR()
    {
      string prNo = string.Empty;
      string prDetailPid = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          prNo = row.Cells["PRNo"].Value.ToString();
          for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow rowChild = row.ChildBands[0].Rows[j];
            if (DBConvert.ParseInt(rowChild.Cells["Selected"].Value.ToString()) == 1)
            {
              prDetailPid += rowChild.Cells["PRDetailPid"].Value.ToString() + ";";
            }
          }
        }
      }
      viewPUR_03_007 view = new viewPUR_03_007();
      view.prNO = prNo;
      view.prDetailPid = prDetailPid;
      Shared.Utility.WindowUtinity.ShowView(view, "Print PR", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
    }
    #endregion LoadData

    #region Event
    /// <summary>
    /// Grid Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Both;

      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Date"].Hidden = true;
      e.Layout.Bands[0].Columns["PRStatus"].Hidden = true;
      e.Layout.Bands[0].Columns["PRNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PRNo"].Header.Caption = "PR No";
      e.Layout.Bands[0].Columns["DeparmentName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["DeparmentName"].Header.Caption = "Department";
      e.Layout.Bands[0].Columns["HeadDepartmentApproved"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["HeadDepartmentApproved"].Header.Caption = "Head Dept Approved";
      e.Layout.Bands[0].Columns["RequestBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["RequestBy"].Header.Caption = "Request By";
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TypeOfRequest"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TypeOfRequest"].Header.Caption = "Type Of Request";
      e.Layout.Bands[0].Columns["PurposeOfRequisition"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PurposeOfRequisition"].Header.Caption = "Purpose Of PR";

      e.Layout.Bands[0].Columns["TotalAmountValue"].Format = "###,###.## VND";
      e.Layout.Bands[0].Columns["TotalAmountValue"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalAmountValue"].Header.Caption = "Total Amount Value";
      e.Layout.Bands[0].Columns["TotalAmountValue"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateBy"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["CreateBy"].MinWidth = 150;

      e.Layout.Bands[1].Columns["PRNo"].Hidden = true;
      e.Layout.Bands[1].Columns["Flag"].Hidden = true;
      e.Layout.Bands[1].Columns["PRDTStatus"].Hidden = true;
      e.Layout.Bands[1].Columns["PRDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["PONo"].Header.Caption = "PO No";
      e.Layout.Bands[1].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      //e.Layout.Bands[1].Columns["PRDetailPid"].Hidden = true;

      e.Layout.Bands[1].Columns["Note"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[1].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["NameEN"].Header.Caption = "Material Name";
      e.Layout.Bands[1].Columns["Status"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Quantity"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Quantity"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["QtyCancel"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["QtyCancel"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Price"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Price"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Price"].Format = "###,###.##";
      e.Layout.Bands[1].Columns["Currency"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Urgent"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["RequestDate"].Header.Caption = "Request Date";
      e.Layout.Bands[1].Columns["ExpectedBrand"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ExpectedBrand"].Header.Caption = "Expected Brand";
      e.Layout.Bands[1].Columns["VAT"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Imported"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Imported"].Header.Caption = "Local/Import";
      e.Layout.Bands[1].Columns["ProjectCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ProjectCode"].Header.Caption = "Project Code";
      e.Layout.Bands[1].Columns["GroupInCharge"].Hidden = true; ;
      e.Layout.Bands[1].Columns["GroupName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[1].Columns["ConfirmBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["ConfirmBy"].Header.Caption = "Confirm By";
      e.Layout.Bands[1].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Status"].CellAppearance.ForeColor = Color.Red;
      e.Layout.Bands[1].Columns["Status"].CellAppearance.FontData.Bold = DefaultableBoolean.True;

      e.Layout.Bands[1].Columns["Note"].CellAppearance.FontData.Bold = DefaultableBoolean.True;

      e.Layout.Bands[2].Columns["PRDetailPid"].Hidden = true;

      if (this.flagviewPrice)
      {
        e.Layout.Bands[1].Columns["Price"].Hidden = false;
        e.Layout.Bands[0].Columns["TotalAmountValue"].Hidden = false;
      }
      else
      {
        e.Layout.Bands[1].Columns["Price"].Hidden = true;
        e.Layout.Bands[0].Columns["TotalAmountValue"].Hidden = true;
      }

      e.Layout.Bands[2].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[2].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Search List PR Depend Condition
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Double Click ==> open Update PR Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultData.Selected != null && ultData.Selected.Rows.Count > 0)
      {
        viewPUR_03_001 view = new viewPUR_03_001();
        view.prNo = ultData.Selected.Rows[0].Cells["PRNo"].Value.ToString();
        Shared.Utility.WindowUtinity.ShowView(view, "UPDATE PR", false, DaiCo.Shared.Utility.ViewState.MainWindow);
        this.Search();
      }
    }

    /// <summary>
    /// New PR
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPUR_03_001 view = new viewPUR_03_001();
      Shared.Utility.WindowUtinity.ShowView(view, "NEW PR", false, DaiCo.Shared.Utility.ViewState.MainWindow);
      this.Search();
    }

    /// <summary>
    /// Close Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Format Price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtItemValueFrom_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtItemValueFrom.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtItemValueFrom.Text = numberRead;
    }

    /// <summary>
    /// Format Price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtItemValueTo_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtItemValueTo.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtItemValueTo.Text = numberRead;
    }

    /// <summary>
    /// Format Price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtPrValueFrom_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtPrValueFrom.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtPrValueFrom.Text = numberRead;
    }

    /// <summary>
    /// Format Price
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtPrValueTo_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtPrValueTo.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtPrValueTo.Text = numberRead;
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      //try
      //{
      //  int index = this.ultData.Selected.Rows[0].ParentRow.Index;
      //  this.ShowDetailInformation(ultData, groupDetailInfo, ultDetail, chkDetail.Checked, index);
      //}
      //catch
      //{
      //  groupDetailInfo.Visible = false;
      //}
    }

    private void ShowDetailInformation(UltraGrid ultData, GroupBox groupDetail, UltraGrid ultDetail, bool showDetail, int index)
    {
      try
      {
        if (showDetail)
        {
          UltraGridRow row = ultData.Selected.Rows[0];

          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@Pid", DbType.Int32, row.Cells["PRDetailPid"].Value.ToString());

          DataTable dtCheck = DataBaseAccess.SearchStoreProcedureDataTable("spPURListPRGetDetailInformation_Select", input);
          this.ultDetail.DataSource = dtCheck;

          groupDetail.Width = 530;
          Point xy = new Point();
          int yMax = ultData.Location.Y + ultData.Height;
          xy.X = ultData.Location.X + 160;
          xy.Y = ultData.Location.Y + (10 * (row.Index + 2) * index);
          if (xy.Y + groupDetail.Height > yMax)
          {
            xy.Y = yMax - groupDetail.Height;
          }
          groupDetail.Location = xy;
          groupDetail.Visible = true;
        }
        else
        {
          groupDetail.Visible = false;
        }
      }
      catch
      {
        groupDetail.Visible = false;
      }
    }

    private void chkDetail_CheckedChanged(object sender, EventArgs e)
    {
      try
      {
        int index = this.ultData.Selected.Rows[0].ParentRow.Index;
        this.ShowDetailInformation(ultData, groupDetailInfo, ultDetail, chkDetail.Checked, index);
      }
      catch
      {
        groupDetailInfo.Visible = false;
      }
    }

    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Scrollbars = Scrollbars.Horizontal;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["DeliveryDate"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["DeliveryDate"].CellAppearance.ForeColor = Color.Red;

      for (int i = 1; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      if (this.flagviewPrice)
      {
        e.Layout.Bands[1].Columns["Price"].Hidden = false;
      }
      else
      {
        e.Layout.Bands[1].Columns["Price"].Hidden = true;
      }

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      int count = 0;
      // Check Valid
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          count = count + 1;
          if (count > 1)
          {
            WindowUtinity.ShowMessageError("ERR0115", "One PR");
            return;
          }
        }
      }// Printf
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          string prNo = row.Cells["PRNo"].Value.ToString();
          this.PrintReport(prNo);
        }
      }
    }

    /// <summary>
    /// Cancel PR
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCancelPR_Click(object sender, EventArgs e)
    {
      ArrayList arrListPRDTPid = new ArrayList();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = ultData.Rows[i].ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
          {
            // Check PR Online
            string commandText = string.Empty;
            commandText += " SELECT PROL.PROnlineNo";
            commandText += " FROM TblPURPRDetail PRDT";
            commandText += " 	  INNER JOIN TblPURPROnlineInformation PROL ON PRDT.PRNo = PROL.PROnlineNo";
            commandText += " WHERE PRDT.Pid = " + DBConvert.ParseLong(row.Cells["PRDetailPid"].Value.ToString());
            DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck != null && dtCheck.Rows.Count == 0)
            {
              arrListPRDTPid.Add(row.Cells["PRDetailPid"].Value.ToString());
            }
          }
        }
      }
      viewPUR_03_006 view = new viewPUR_03_006();
      view.arrList = arrListPRDTPid;
      WindowUtinity.ShowView(view, "CANCEL PR", false, ViewState.ModalWindow, FormWindowState.Normal);

      this.Search();
    }

    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      if (e.Cell.Row.ParentRow == null)
      {
        string columnName = e.Cell.Column.ToString();
        if (string.Compare(columnName, "Selected", true) == 0)
        {
          UltraGridRow row = e.Cell.Row;
          for (int i = 0; i < row.ChildBands[0].Rows.Count; i++)
          {
            row.ChildBands[0].Rows[i].Cells["Selected"].Value = e.Cell.Text;
          }
        }
      }
      else
      {
        string columnName = e.Cell.Column.ToString();
        if (string.Compare(columnName, "Selected", true) == 0)
        {
          UltraGridRow rowParent = e.Cell.Row.ParentRow;
          rowParent.Cells["Selected"].Value = 0;
          for (int i = 0; i < rowParent.ChildBands[0].Rows.Count; i++)
          {
            UltraGridRow rowChild = rowParent.ChildBands[0].Rows[i];
            if (rowChild.Cells["Selected"].Text == "1")
            {
              rowParent.Cells["Selected"].Value = 1;
            }
          }
        }
      }
    }

    /// <summary>
    /// Print Selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrintSelect_Click(object sender, EventArgs e)
    {
      dsPURPurchaseRequisition dsSource = new dsPURPurchaseRequisition();
      string PRNo = string.Empty;
      int rowInfo = int.MinValue;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = ultData.Rows[i].ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
          {
            PRNo = ultData.Rows[i].Cells["PRNo"].Value.ToString();
            rowInfo = i;
            goto AA;
          }
        }
      }
    AA:
      // Info
      string commandText = string.Format(@"SELECT PR.PRNo, PR.Department, PR.PurposeOfRequisition, CODE.Value TypeOfRequest
                                            FROM TblPURPRInformation PR
                                            LEFT JOIN TblBOMCodeMaster CODE ON CODE.Code =  PR.TypeOfRequest
                                            WHERE PRNo = '{0}' AND CODE.[Group] = 7005", PRNo);
      DataTable dtInfo = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtInfo != null)
      {
        dsSource.Tables["dtPurchaseRequisitionInfo"].Merge(dtInfo);
      }

      // Detail
      for (int j = 0; j < ultData.Rows[rowInfo].ChildBands[0].Rows.Count; j++)
      {
        UltraGridRow row = ultData.Rows[rowInfo].ChildBands[0].Rows[j];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@PRNo", DbType.AnsiString, 16, PRNo);
          input[1] = new DBParameter("@PRDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["PRDetailPid"].Value.ToString()));
          DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPurchaseRequisitionPrintSelect_Select", input);
          if (ds != null)
          {
            dsSource.Tables["dtPurchaseRequisitionDetail"].Merge(ds.Tables[0]);
            dsSource.Tables["dtPRDetailSubReport"].Merge(ds.Tables[1]);
          }
        }
      }

      string commandText1 = string.Format(@"SELECT PRPO PRNo, Name, Qty, Price * ExchangeRate AS Price, (Qty * Price * ExchangeRate) Amount
                                              FROM TblPURAdditionPrice
                                              WHERE PRPO = '{0}'", PRNo);
      DataTable dt1 = DataBaseAccess.SearchCommandTextDataTable(commandText1);
      if (dt1 != null && dt1.Rows.Count > 0)
      {
        dsSource.Tables["dtAdditionPrice"].Merge(dt1);
      }

      // Addition Price
      double totalAmount = 0;
      double totalAmountUSD = 0;
      double currentExchangeRate = 1;
      foreach (DataRow row in dsSource.Tables["dtPurchaseRequisitionDetail"].Rows)
      {
        totalAmount += DBConvert.ParseDouble(row["Amount"].ToString());
      }
      string commadText = "SELECT CurrentExchangeRate FROM TblPURCurrencyInfo WHERE Code = 'USD'";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commadText);
      if (dt != null)
      {
        currentExchangeRate = DBConvert.ParseDouble(dt.Rows[0]["CurrentExchangeRate"].ToString());
        totalAmountUSD = totalAmount / currentExchangeRate;
      }
      DaiCo.Shared.View_Report report = null;
      cptPurchaseRequisition cpt = new cptPurchaseRequisition();
      if (dsSource.Tables["dtAdditionPrice"].Rows.Count > 0)
      {
        for (int i = 0; i < dsSource.Tables["dtAdditionPrice"].Rows.Count; i++)
        {
          totalAmount = totalAmount + DBConvert.ParseDouble(dsSource.Tables["dtAdditionPrice"].Rows[i]["Amount"].ToString());
        }
        totalAmountUSD = totalAmount / currentExchangeRate;
        cpt.Section4.SectionFormat.EnableSuppress = false;
      }
      else
      {
        cpt.Section4.SectionFormat.EnableSuppress = true;
      }
      cpt.SetDataSource(dsSource);
      cpt.SetParameterValue("paramTotalAmount", totalAmount);
      cpt.SetParameterValue("paramTotalAmountUSD", totalAmountUSD);

      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      report.ShowReport(Shared.Utility.ViewState.MainWindow);
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

    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// PrintPR
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrintPR_Click(object sender, EventArgs e)
    {
      this.PrintPR();
    }
    #endregion Event
  }
}
