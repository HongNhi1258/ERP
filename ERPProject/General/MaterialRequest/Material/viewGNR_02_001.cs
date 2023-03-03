/*
  Author      : Xuan Truong
  Description : List Material Requisition 
  Date        : 04/01/2012
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.General;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;

namespace DaiCo.ERPProject
{
  public partial class viewGNR_02_001 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init

    public viewGNR_02_001()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load ViewGNR_02_001
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewGNR_02_001_Load(object sender, EventArgs e)
    {
      ultDateFrom.Value = DateTime.Today.AddDays(-7);
      ultDateTo.Value = DateTime.Today.AddDays(0);

      // Load Status
      this.LoadStatus();
      // Warehouse
      Utility.LoadUltraCBMaterialWHList(ucbWarehouse);
      // Load default WH
      if (ucbWarehouse.Rows.Count > 0)
      {
        ucbWarehouse.Rows[0].Selected = true;
      }
      // Load ItemCode
      this.LoadItemCode();
      // Load Material Code
      this.LoadMaterialCode();
      // Load Department
      this.LoadDepartment();
      // Load Department Login
      this.LoadComboDepartmentUserLogin();
      // Load WO
      this.LoadWO();
    }

    //    // Check Department
    //    private int CheckDepartment()
    //    {
    //      string commandText = string.Format(@"SELECT DE.Department
    //                                           FROM VHRNhanVien NV
    //                                           INNER JOIN VHRDDepartment DE ON NV.Department = DE.Department AND NV.ID_NhanVien = {0}", SharedObject.UserInfo.UserPid);
    //      string department = DataBaseAccess.SearchCommandTextDataTable(commandText).Rows[0][0].ToString();
    //      int check = string.Compare(department, "WHD", true);
    //      return check;
    //    }
    // Load Status
    private void LoadStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Finished' Name";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultStatus, dtSource, "ID", "Name", false, "ID");
    }
    // Load Material Code
    private void LoadMaterialCode()
    {
      string commandText = "SELECT MaterialCode, MaterialNameEn, MaterialCode +' | '+ MaterialNameEn AS Display FROM VBOMMaterials WHERE Warehouse = " + ConstantClass.MATERIALS_WAREHOUSE + "";
      DataTable dtMaterial = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultMaterialCode, dtMaterial, "MaterialCode", "Display", false, "Display");
      ultMaterialCode.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }
    // Load WO
    private void LoadWO()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultWO, dtSource, "Pid", "Pid", false);
    }
    // Load ItemCode
    private void LoadItemCode()
    {
      string commandText = "SELECT ItemCode, Name ItemName, (ItemCode + ' | ' + Name) DisplayText FROM TblBOMItemBasic";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultItemCode, dtSource, "ItemCode", "DisplayText", false, "DisplayText");
      ultItemCode.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }
    // Load Department
    private void LoadDepartment()
    {
      string commandText = "SELECT Department, Department + ' | ' + DeparmentName AS Name FROM VHRDDepartment";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultDepartment, dtSource, "Department", "Name", false, "Department");
      ultDepartment.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
    }

    /// <summary>
    /// Set Default Department
    /// </summary>
    private void LoadComboDepartmentUserLogin()
    {
      string commandText = string.Format(@" SELECT DEP.Department, DEP.Department + ' | ' + DEP.DeparmentName FROM VHRNhanVien NV
                                            INNER JOIN VHRDDepartment DEP ON NV.Department = DEP.Department AND NV.ID_NhanVien = {0}", SharedObject.UserInfo.UserPid);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        ultDepartment.Value = dtSource.Rows[0][0].ToString();
      }
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      string department = string.Empty;
      string itemCode = string.Empty;
      string materialCode = string.Empty;
      long wo = long.MinValue;
      int status = int.MinValue;
      string noSet = txtRequisitionNoteSet.Text.Trim();

      string[] listNo = noSet.Split(',');
      foreach (string no in listNo)
      {
        if (no.Trim().Length > 0)
        {
          noSet += string.Format(",'{0}'", no.Replace("'", "").Trim());
        }
      }

      if (ultDepartment.Value != null)
      {
        department = this.ultDepartment.Value.ToString();
      }
      if (ultItemCode.Value != null)
      {
        itemCode = this.ultItemCode.Value.ToString();
      }
      if (ultMaterialCode.Value != null)
      {
        materialCode = this.ultMaterialCode.Value.ToString();
      }
      if (ultWO.Value != null)
      {
        wo = DBConvert.ParseLong(this.ultWO.Value.ToString());
      }
      if (ultStatus.Value != null)
      {
        status = DBConvert.ParseInt(this.ultStatus.Value.ToString());
      }

      DateTime createDateFrom = DateTime.MinValue;
      if (ultDateFrom.Value != null)
      {
        createDateFrom = (DateTime)ultDateFrom.Value;
      }

      DateTime createDateTo = DateTime.MinValue;
      if (ultDateTo.Value != null)
      {
        createDateTo = (DateTime)ultDateTo.Value;
      }

      DBParameter[] intputParam = new DBParameter[12];
      if (noSet.Length > 0)
      {
        noSet = string.Format("{0}", noSet.Remove(0, 1));
        intputParam[0] = new DBParameter("@NoSet", DbType.AnsiString, 1024, noSet);
      }

      if (ultDateFrom.Value != null)
      {
        intputParam[1] = new DBParameter("@CreateDateFrom", DbType.DateTime, createDateFrom);
      }

      if (ultDateTo.Value != null)
      {
        createDateTo = createDateTo != (DateTime.MaxValue) ? createDateTo.AddDays(1) : createDateTo;
        intputParam[2] = new DBParameter("@CreateDateTo", DbType.DateTime, createDateTo);
      }

      if (department != string.Empty)
      {
        intputParam[3] = new DBParameter("@Department", DbType.AnsiString, 8, department);
      }

      if (itemCode != string.Empty)
      {
        intputParam[4] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      }

      if (materialCode != string.Empty)
      {
        intputParam[5] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
      }

      if (wo != long.MinValue)
      {
        intputParam[6] = new DBParameter("@WO", DbType.Int64, wo);
      }

      if (status != int.MinValue)
      {
        intputParam[7] = new DBParameter("@Status", DbType.Int32, status);
      }

      intputParam[8] = new DBParameter("@Prefix", DbType.String, "%" + "RNM" + "%");
      intputParam[9] = new DBParameter("@Prefix1", DbType.String, "%" + "RMA" + "%");

      intputParam[10] = new DBParameter("@DepartmentLogin", DbType.String, SharedObject.UserInfo.Department);
      if (ucbWarehouse.Value != null)
      {
        intputParam[11] = new DBParameter("@WarehousePid", DbType.Int32, ucbWarehouse.Value);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spGNRListMaterialRequisition_Select", 900, intputParam);
      dsGNRListMaterialRequisition dslist = new dsGNRListMaterialRequisition();
      if (ds != null)
      {
        dslist.Tables["MaterialRequisition"].Merge(ds.Tables[0]);
        dslist.Tables["MaterialRequisitionDetail"].Merge(ds.Tables[1]);
      }
      ultMaterialRequisition.DataSource = dslist;

      for (int i = 0; i < ultMaterialRequisition.Rows.Count; i++)
      {
        if (string.Compare(ultMaterialRequisition.Rows[i].Cells["Status"].Value.ToString(), "Confirmed", true) == 0)
        {
          ultMaterialRequisition.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
        else if (string.Compare(ultMaterialRequisition.Rows[i].Cells["Status"].Value.ToString(), "Finished", true) == 0)
        {
          ultMaterialRequisition.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
      }
    }
    //<summary>
    //SendMail
    //</summary>
    private void SendEmailWhenFinished()
    {
      for (int i = 0; i < ultMaterialRequisition.Rows.Count; i++)
      {
        UltraGridRow row = ultMaterialRequisition.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          Email email = new Email();
          email.Key = email.KEY_MRN_002;
          ArrayList arrList = email.GetDataMain(email.Key);
          if (arrList.Count == 3)
          {
            string toFromSql = string.Empty;
            toFromSql = string.Format(arrList[0].ToString(), DBConvert.ParseInt(row.Cells["CreateBy"].Value.ToString()), row.Cells["DepartmentRequest"].Value.ToString(), Shared.Utility.SharedObject.UserInfo.UserPid);
            toFromSql = email.GetEmailToFromSql(toFromSql);

            string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
            string subject = string.Format(arrList[1].ToString(), row.Cells["Code"].Value.ToString(), userName);

            string body = string.Format(arrList[2].ToString());
            email.InsertEmail(email.Key, toFromSql, subject, body);
            //email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
          }
        }
      }
    }

    //<summary>
    //SendMail
    //</summary>
    private void SendEmailWhenDeleted()
    {
      for (int i = 0; i < ultMaterialRequisition.Rows.Count; i++)
      {
        UltraGridRow row = ultMaterialRequisition.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1 && string.Compare(row.Cells["Status"].Value.ToString(), "Confirmed", true) == 0)
        {
          Email email = new Email();
          email.Key = email.KEY_MRN_003;
          ArrayList arrList = email.GetDataMain(email.Key);
          if (arrList.Count == 3)
          {
            string toFromSql = string.Empty;
            toFromSql = string.Format(arrList[0].ToString(), DBConvert.ParseInt(row.Cells["CreateBy"].Value.ToString()), row.Cells["DepartmentRequest"].Value.ToString(), Shared.Utility.SharedObject.UserInfo.UserPid);
            toFromSql = email.GetEmailToFromSql(toFromSql);

            string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
            string subject = string.Format(arrList[1].ToString(), row.Cells["Code"].Value.ToString(), userName);

            string body = string.Format(arrList[2].ToString());
            email.InsertEmail(email.Key, toFromSql, subject, body);
            //email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
          }
        }
      }
    }
    #endregion Function

    #region Event
    /// <summary>
    /// Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
      chkExpandAll.Checked = false;
    }
    /// <summary>
    /// Clear Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.txtRequisitionNoteSet.Text = string.Empty;
      ucbWarehouse.Value = null;
      this.ultDateFrom.Value = DBNull.Value;
      this.ultDateTo.Value = DBNull.Value;

      this.ultDepartment.Text = string.Empty;
      this.ultItemCode.Text = string.Empty;
      this.ultWO.Text = string.Empty;
      this.ultStatus.Text = string.Empty;
      this.ultMaterialCode.Text = string.Empty;
    }

    /// <summary>
    /// New Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      if (rdRequestNormal.Checked == true)
      {
        viewGNR_02_002 view = new viewGNR_02_002();
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "MATERIAL REQUISITION NOTE ONLINE", true, DaiCo.Shared.Utility.ViewState.MainWindow);
      }
      else if (rdRequestForPart.Checked == true)
      {
        viewGNR_02_004 view = new viewGNR_02_004();
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "MATERIAL REQUISITION NOTE ONLINE", true, DaiCo.Shared.Utility.ViewState.MainWindow);
      }
    }
    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultMaterialRequisition_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      Utility.SetPropertiesUltraGrid(ultMaterialRequisition);

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 4; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      e.Layout.Bands[0].Columns["CreateBy"].Hidden = true;     
      e.Layout.Bands[0].Columns["Code"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Code"].MinWidth = 150;
      e.Layout.Bands[0].Columns["DepartmentRequest"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["DepartmentRequest"].MinWidth = 200;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 100;
      e.Layout.Bands[0].Columns["DeliveryTime"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["DeliveryTime"].MinWidth = 150;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["MRNPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Wo"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["Wo"].MinWidth = 100;
      e.Layout.Bands[1].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[1].Columns["Revision"].MaxWidth = 50;
      e.Layout.Bands[1].Columns["Revision"].MinWidth = 50;
      e.Layout.Bands[1].Columns["Unit"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["Unit"].MinWidth = 100;
      e.Layout.Bands[1].Columns["Qty"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["Qty"].MinWidth = 100;

      // Set caption
      e.Layout.Bands[0].Columns["Code"].Header.Caption = "Mã phiếu";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Người tạo"; 
      e.Layout.Bands[0].Columns["DepartmentRequest"].Header.Caption = "Bộ phận y/c";
      e.Layout.Bands[0].Columns["DeliveryTime"].Header.Caption = "Thời gian nhận";
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Ghi chú";
      e.Layout.Bands[0].Columns["Status"].Header.Caption = "Trạng thái";
      e.Layout.Bands[0].Columns["Selected"].Header.Caption = "Chọn";

      e.Layout.Bands[1].Columns["WO"].Header.Caption = "LSX";
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Mã SP";
      e.Layout.Bands[1].Columns["Revision"].Header.Caption = "Phiên bản";
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Mã vật tư";
      e.Layout.Bands[1].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[1].Columns["Qty"].Header.Caption = "Số lượng";      
      e.Layout.Bands[1].Columns["MaterialNameVn"].Header.Caption = "Tên vật tư";

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
    private void ultMaterialRequisition_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultMaterialRequisition.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultMaterialRequisition.Selected.Rows[0].ParentRow == null) ? ultMaterialRequisition.Selected.Rows[0] : ultMaterialRequisition.Selected.Rows[0].ParentRow;

      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      // Begin
      int userPid = SharedObject.UserInfo.UserPid;
      string department = row.Cells["DepartmentRequest"].Value.ToString();

      string commandText = string.Format(@"SELECT DE.Department
                                           FROM VHRNhanVien NV
                                           INNER JOIN VHRDDepartment DE ON NV.Department = DE.Department AND NV.ID_NhanVien = {0}", userPid);
      string departmentRequest = DataBaseAccess.SearchCommandTextDataTable(commandText).Rows[0][0].ToString();
      int check = string.Compare(department, departmentRequest, true);
      string status = row.Cells["Status"].Value.ToString();

      if (string.Compare(status, "Finished", true) == 0 || string.Compare(status, "Confirmed", true) == 0)
      {
        if (rdRequestNormal.Checked == true)
        {
          viewGNR_02_002 uc = new viewGNR_02_002();
          uc.MaterialRequisitionPid = pid;
          WindowUtinity.ShowView(uc, "MATERIAL REQUISITION NOTE", true, ViewState.MainWindow);
        }
        else if (rdRequestForPart.Checked == true)
        {
          viewGNR_02_004 uc = new viewGNR_02_004();
          uc.MaterialRequisitionPid = pid;
          WindowUtinity.ShowView(uc, "MATERIAL REQUISITION NOTE", true, ViewState.MainWindow);
        }
      }
      else if (check == 0)
      {
        if (rdRequestNormal.Checked == true)
        {
          viewGNR_02_002 uc = new viewGNR_02_002();
          uc.MaterialRequisitionPid = pid;
          WindowUtinity.ShowView(uc, "MATERIAL REQUISITION NOTE", true, ViewState.MainWindow);
        }
        else if (rdRequestForPart.Checked == true)
        {
          viewGNR_02_004 uc = new viewGNR_02_004();
          uc.MaterialRequisitionPid = pid;
          WindowUtinity.ShowView(uc, "MATERIAL REQUISITION NOTE", true, ViewState.MainWindow);
        }
      }
      // End

      //viewGNR_02_003 uc = new viewGNR_02_003();
      //uc.MaterialRequisitionPid = pid;
      //WindowUtinity.ShowView(uc, "MATERIAL REQUISITION NOTE", true, ViewState.MainWindow);

      // Search Grid Again 
      //this.Search();
    }

    /// <summary>
    /// Expand All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultMaterialRequisition.Rows.ExpandAll(true);
      }
      else
      {
        ultMaterialRequisition.Rows.CollapseAll(true);
      }
    }
    /// <summary>
    /// Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }
    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      int count = 0;
      for (int i = 0; i < ultMaterialRequisition.Rows.Count; i++)
      {
        UltraGridRow row = ultMaterialRequisition.Rows[i];
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
      for (int i = 0; i < ultMaterialRequisition.Rows.Count; i++)
      {
        UltraGridRow row = ultMaterialRequisition.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          if (String.Compare(row.Cells["Status"].Value.ToString(), "Finished", true) != 0)
          {
            if (WindowUtinity.ShowMessageConfirm("MSG0007", row.Cells["Code"].Value.ToString()).ToString() == "No")
            {
              return;
            }

            DBParameter[] input = new DBParameter[2];
            input[0] = new DBParameter("@pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            input[1] = new DBParameter("@userPid", DbType.Int64, SharedObject.UserInfo.UserPid);
            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialRequisitionNote_Delete", input, output);
            if (DBConvert.ParseLong(output[0].Value.ToString()) == 1)
            {
              WindowUtinity.ShowMessageSuccess("MSG0002");
            }
            else if (DBConvert.ParseLong(output[0].Value.ToString()) == 0)
            {
              WindowUtinity.ShowMessageError("ERR0093", row.Cells["Code"].Value.ToString());
            }
            else if (DBConvert.ParseLong(output[0].Value.ToString()) == -1)
            {
              WindowUtinity.ShowMessageError("ERR0123", row.Cells["Code"].Value.ToString(), "is wating for issuing");
            }
            else if (DBConvert.ParseLong(output[0].Value.ToString()) == -2)
            {
              WindowUtinity.ShowMessageError("ERR0123", row.Cells["Code"].Value.ToString(), "was finished");
            }
            else if (DBConvert.ParseLong(output[0].Value.ToString()) == -3)
            {
              WindowUtinity.ShowMessageError("ERR0123", row.Cells["Code"].Value.ToString(), "was created by other user");
            }
          }
          else
          {
            WindowUtinity.ShowMessageError("ERR0123", row.Cells["Code"].Value.ToString(), "was finished");
          }
        }
      }
      this.SendEmailWhenDeleted();
      this.Search();
    }

    /// <summary>
    /// Finish
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFinish_Click(object sender, EventArgs e)
    {
      int count = 0;
      for (int i = 0; i < ultMaterialRequisition.Rows.Count; i++)
      {
        UltraGridRow row = ultMaterialRequisition.Rows[i];
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
      for (int i = 0; i < ultMaterialRequisition.Rows.Count; i++)
      {
        UltraGridRow row = ultMaterialRequisition.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@MRNPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));

          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialRequisitionNoteStatus_Update", input, output);
          if (DBConvert.ParseLong(output[0].Value.ToString()) == 1)
          {
            WindowUtinity.ShowMessageSuccess("MSG0051", row.Cells["Code"].Value.ToString());
          }
          else if (DBConvert.ParseLong(output[0].Value.ToString()) == 0)
          {
            WindowUtinity.ShowMessageError("ERRO122", row.Cells["Code"].Value.ToString());
          }
          else if (DBConvert.ParseLong(output[0].Value.ToString()) == -1)
          {
            WindowUtinity.ShowMessageError("ERRO121", row.Cells["Code"].Value.ToString(), "is wating for issuing");
          }
        }
      }
      this.SendEmailWhenFinished();
      this.Search();
    }

    #endregion Event
  }
}
