/*
 * Authour     : 
 * Date        : 30/06/2010
 * Description : Master Planning Information Enquiry
 */
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_01_007 : MainUserControl
  {
    #region Field
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    public long enquiryPid = long.MinValue;
    public int qty = int.MinValue;
    private bool IsConfirm;
    private DataTable dtSaleOrder;
    #endregion Field

    #region Init 
    /// <summary>
    /// Register Item on Screen  
    /// </summary>
    public viewPLN_01_007()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Event Load MasterPlanning Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UC_PLNMasterPlanInfo_Load(object sender, EventArgs e)
    {
      this.LoadData();
      this.LoadComboBoxData(string.Empty, string.Empty, int.MinValue, string.Empty);
      this.btnClose.Focus();
    }

    /// <summary>
    /// Get information into screen 
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@EnquiryDetailPid", DbType.Int64, enquiryPid);
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNMasterPlanInfo_Select", inputParam);

      DataTable dtEnquiryDetail = dsSource.Tables[0];

      if (dtEnquiryDetail.Rows.Count == 0)
      {
        return;
      }
      int status = DBConvert.ParseInt(dtEnquiryDetail.Rows[0]["IsConfirm"].ToString());
      IsConfirm = ((status == 2) || (status == 3));
      this.txtItemCode.Text = DBConvert.ParseString(dtEnquiryDetail.Rows[0]["ItemCode"]);
      this.txtRevision.Text = DBConvert.ParseString(dtEnquiryDetail.Rows[0]["Revision"]);
      this.txtSaleCode.Text = DBConvert.ParseString(dtEnquiryDetail.Rows[0]["SaleCode"]);

      int qty = DBConvert.ParseInt(dtEnquiryDetail.Rows[0]["Qty"]);
      double cbm = DBConvert.ParseDouble(dtEnquiryDetail.Rows[0]["CBM"]);

      this.txtQty.Text = DBConvert.ParseString(qty);
      this.txtCbm.Text = cbm > 0 && qty > 0 ? DBConvert.ParseString(qty * cbm) : string.Empty;
      this.txtUnsold.Text = DBConvert.ParseString(dtEnquiryDetail.Rows[0]["UnsoldQty"]);
      this.txtExcess.Text = DBConvert.ParseString(dtEnquiryDetail.Rows[0]["Excess"]);
      this.txtDate.Text = DBConvert.ParseString(dtEnquiryDetail.Rows[0]["RequestDate"]);

      txtStatus.Text = IsConfirm ? "Confirmed" : "None Confirmed";
      this.ultrEnquirySchedule.DataSource = dsSource.Tables[1];
      dtSaleOrder = dsSource.Tables[2];
      this.utrSaleOrderStatus.DataSource = dtSaleOrder;
      this.ultrItemStatus.DataSource = dsSource.Tables[3];
      this.ultrCarcassStatus.DataSource = dsSource.Tables[4];
      LoadGrid();
    }

    private void LoadGrid()
    {
      int count = ultrEnquirySchedule.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultrEnquirySchedule.Rows[i];

        if (DBConvert.ParseInt(row.Cells["Expire"].Value.ToString()) == 1)
        {
          row.Cells["Keep"].Activation = Activation.NoEdit;
          row.Cells["NonPlan"].Activation = Activation.NoEdit;
        }


        //if (DBConvert.ParseInt(row.Cells["Keep"].Value.ToString()) == 1)
        //{
        //  row.Cells["Expire"].Activation = Activation.NoEdit;
        //  row.Cells["NonPlan"].Activation = Activation.NoEdit;
        //}
        //if (DBConvert.ParseInt(row.Cells["NonPlan"].Value.ToString()) == 1)
        //{
        //  row.Cells["ScheduleDate"].Activation = Activation.NoEdit;
        //  row.Cells["Expire"].Activation = Activation.NoEdit;
        //  row.Cells["Keep"].Activation = Activation.NoEdit;
        //}
      }
    }

    #endregion Init

    private bool CheckIsValid()
    {
      long totalQty = DBConvert.ParseLong(txtQty.Text);
      if (ultrEnquirySchedule.DataSource != null)
      {
        DataTable dtCheck = (DataTable)ultrEnquirySchedule.DataSource;
        DataRow[] rowCheckQty = dtCheck.Select("Qty <= 0");
        if (rowCheckQty.Length > 0)
        {
          WindowUtinity.ShowMessageWarning("ERR0001", "Qty");
          return false;
        }

        long totalQtyCheck = (dtCheck.Compute("SUM(Qty)", "Qty > 0").ToString().Trim().Length > 0) ? (long)dtCheck.Compute("SUM(Qty)", "Qty > 0") : 0;
        if (totalQtyCheck > totalQty)
        {
          WindowUtinity.ShowMessageWarning("ERR0026", "Qty");
          return false;
        }

        if (totalQtyCheck < totalQty)
        {
          WindowUtinity.ShowMessageWarning("ERR0027", "Qty");
          return false;
        }
        DataRow[] rowCheckScheduleDate = dtCheck.Select(string.Format("NonPlan = 0 AND ScheduleDate < #{0}#", DBConvert.ParseString(DateTime.Now, "MM/dd/yyyy")));
        if (!IsConfirm && rowCheckScheduleDate.Length > 0)
        {
          WindowUtinity.ShowMessageWarning("ERR0001", "Confirm Ship Date");
          return false;
        }

        IList hasPass = new ArrayList();
        foreach (DataRow dr in dtCheck.Rows)
        {
          if (dr.RowState != DataRowState.Deleted)
          {
            int nonPlan = DBConvert.ParseInt(dr["NonPlan"].ToString());
            object schedule = dr["ScheduleDate"];
            DateTime date = (schedule.ToString().Trim().Length > 0) ? (DateTime)schedule : DateTime.MinValue;
            if (nonPlan == 0 && date == DateTime.MinValue)
            {
              WindowUtinity.ShowMessageWarning("ERR0001", "Confirm Ship Date");
              return false;
            }

            if (dr["ScheduleDate"] != DBNull.Value && !hasPass.Contains(dr["ScheduleDate"]))
            {
              hasPass.Add(dr["ScheduleDate"]);
            }
            else if (dr["ScheduleDate"] != DBNull.Value)
            {
              WindowUtinity.ShowMessageWarning("ERR0006", "Confirm Ship Date");
              return false;
            }
          }
        }
      }
      else
      {
        WindowUtinity.ShowMessageWarning("ERR0001", "Data");
        return false;
      }
      return true;
    }

    private bool Save()
    {
      bool success = true;
      int count = ultrEnquirySchedule.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultrEnquirySchedule.Rows[i];
        PLNEnquiryConfirmDetail enConfirmDT = new PLNEnquiryConfirmDetail();

        enConfirmDT.EnquiryDetailPid = this.enquiryPid;

        long pidConfirm = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pidConfirm == long.MinValue)
        {
          enConfirmDT.CreateDate = DateTime.Now;
          enConfirmDT.CreateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
        }
        else
        {
          PLNEnquiryConfirmDetail chx = new PLNEnquiryConfirmDetail();
          chx.Pid = pidConfirm;
          enConfirmDT = (PLNEnquiryConfirmDetail)DataBaseAccess.LoadObject(chx, new string[] { "Pid" });
          if (enConfirmDT == null)
          {
            WindowUtinity.ShowMessageError("ERR0005");
            return false;
          }
          enConfirmDT.UpdateDate = DateTime.Now;
          enConfirmDT.UpdateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
        }

        enConfirmDT.Qty = DBConvert.ParseInt(row.Cells["Qty"].Value);

        object obj = row.Cells["ScheduleDate"].Value;
        if (obj != DBNull.Value)
        {
          enConfirmDT.ScheduleDate = (DateTime)obj;
        }
        else
        {
          enConfirmDT.ScheduleDate = DateTime.MinValue;
        }

        int nonePlan = DBConvert.ParseInt(row.Cells["NonPlan"].Value);
        nonePlan = (nonePlan == int.MinValue ? 0 : nonePlan);
        enConfirmDT.NonPlan = nonePlan;
        int expire = DBConvert.ParseInt(row.Cells["Expire"].Value);
        enConfirmDT.Expire = (expire == int.MinValue ? 0 : expire);

        int keep = DBConvert.ParseInt(row.Cells["Keep"].Value);
        enConfirmDT.Keep = (keep == int.MinValue ? 0 : keep);


        enConfirmDT.Remark = DBConvert.ParseString(row.Cells["Remark"].Value);

        if (enConfirmDT.Pid == long.MinValue)
        {
          long result = DataBaseAccess.InsertObject(enConfirmDT);
          if (result == long.MinValue)
          {
            success = false;
          }
        }
        else
        {
          bool result = DataBaseAccess.UpdateObject(enConfirmDT, new string[] { "Pid" });
          bool bresult = true;
          if (enConfirmDT.NonPlan == 1 && enConfirmDT.ScheduleDate != DateTime.MinValue)
          {
            string sqlUpdateSO = String.Format("update TblPLNSaleOrderDetail set ScheduleDelivery = CONVERT(datetime,'{0}',103) where Pid = {1} and ScheduleDelivery is null", enConfirmDT.ScheduleDate.ToString("dd/MM/yyyy"), enConfirmDT.Pid.ToString());
            bresult = DataBaseAccess.ExecuteCommandText(sqlUpdateSO);
          }


          if (!result || !bresult)
          {
            success = false;
          }
        }
      }

      foreach (long pidDelete in this.listDeletedPid)
      {
        PLNEnquiryConfirmDetail chx = new PLNEnquiryConfirmDetail();
        chx.Pid = pidDelete;
        success = DataBaseAccess.DeleteObject(chx, new string[] { "Pid" });
      }
      return success;
    }

    #region  Button Event

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (CheckIsValid())
      {
        if (Save())
        {
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
          //WindowUtinity.CloseView(this);
          this.LoadData();
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          return;
        }
      }
    }

    /// <summary>
    /// Close Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      WindowUtinity.CloseView(this);
    }
    #endregion  Button Event 

    private void utrSaleOrderStatus_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["SaleNo"].Header.Caption = "Sale No";
      e.Layout.Bands[0].Columns["SaleNo"].MinWidth = 100;
      e.Layout.Bands[0].Columns["SaleNo"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CustomerName"].Header.Caption = "Customer Name";
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Schedule";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].MinWidth = 90;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].MaxWidth = 90;
    }
    private void ultrItemStatus_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
      e.Layout.Bands[0].Columns["WorkOrderPid"].Header.Caption = "Work Order";
      e.Layout.Bands[0].Columns["WorkOrderPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WorkOrderPid"].MinWidth = 60;
      e.Layout.Bands[0].Columns["WorkOrderPid"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["OpenWorkQty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["OpenWorkQty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["OpenWorkQty"].Header.Caption = "Open Work";
      e.Layout.Bands[0].Columns["OpenWorkQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ItemStatus"].Header.Caption = "Item Status";
    }

    private void ultrCarcassStatus_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultrCarcassStatus);
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
      e.Layout.Bands[0].Columns["WorkOrderPid"].Header.Caption = "Work Order";
      e.Layout.Bands[0].Columns["WorkOrderPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WorkOrderPid"].MinWidth = 60;
      e.Layout.Bands[0].Columns["WorkOrderPid"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["OpenWorkQty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["OpenWorkQty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["OpenWorkQty"].Header.Caption = "Open Work";
      e.Layout.Bands[0].Columns["OpenWorkQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["primaryCutQty"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["primaryCutQty"].MinWidth = 100;
      e.Layout.Bands[0].Columns["primaryCutQty"].Header.Caption = "Primary Cut Finish";
      e.Layout.Bands[0].Columns["primaryCutQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CarcassStatus"].Header.Caption = "Carcass Status";
    }

    private void ultrEnquirySchedule_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsConfirm"].Hidden = true;

      e.Layout.Bands[0].Columns["EnquiryDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ScheduleDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ScheduleDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ScheduleDate"].Header.Caption = "Confirm Ship Date";
      e.Layout.Bands[0].Columns["Expire"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Keep"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["NonPlan"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["NonPlan"].Header.Caption = "Non Plan";
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Keep"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Expire"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleDate"].Width = 30;
      e.Layout.Bands[0].Columns["Expire"].Width = 15;
      e.Layout.Bands[0].Columns["Keep"].Width = 15;
      e.Layout.Bands[0].Columns["NonPlan"].Width = 20;
      e.Layout.Bands[0].Columns["Qty"].Width = 15;
      if (IsConfirm)
      {
        e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
        //e.Layout.Bands[0].Columns["ScheduleDate"].CellActivation = Activation.NoEdit;
        e.Layout.Bands[0].Columns["NonPlan"].CellActivation = Activation.NoEdit;
        //e.Layout.Bands[0].Columns["Keep"].CellActivation = Activation.NoEdit;
        e.Layout.Bands[0].Columns["Expire"].CellActivation = Activation.NoEdit;
        e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.NoEdit;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      }
      else
      {
        e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["ScheduleDate"].CellActivation = Activation.AllowEdit;
        e.Layout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      }
      for (int i = 0; i < ultrEnquirySchedule.Rows.Count; i++)
      {
        UltraGridRow row = ultrEnquirySchedule.Rows[i];
        if (IsConfirm)
        {
          if (DBConvert.ParseInt(row.Cells["NonPlan"].Value.ToString()) == 1 && row.Cells["ScheduleDate"].Value.ToString().Length == 0)
          {
            row.Cells["ScheduleDate"].Activation = Activation.AllowEdit;
          }
          else
          {
            row.Cells["ScheduleDate"].Activation = Activation.NoEdit;
          }
        }
      }
    }
    private void ultrEnquirySchedule_AfterRowInsert(object sender, Infragistics.Win.UltraWinGrid.RowEventArgs e)
    {
      e.Row.Cells["Expire"].Value = 0;
      e.Row.Cells["Keep"].Value = 0;
      e.Row.Cells["NonPlan"].Value = 0;
    }

    private void ultrEnquirySchedule_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        listDeletedPid.Add(pid);
      }
    }

    private void ultrEnquirySchedule_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long detailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (detailPid != long.MinValue)
        {
          listDeletingPid.Add(detailPid);
        }
      }
    }

    private void LoadComboBoxData(string saleNo, string itemCode, int revision, string carcassCode)
    {
      DBParameter[] input = new DBParameter[4];
      if (saleNo.Length > 0)
      {
        input[0] = new DBParameter("@SaleNo", DbType.AnsiString, 16, saleNo);
      }
      if (itemCode.Length > 0)
      {
        input[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      }
      if (revision != int.MinValue)
      {
        input[2] = new DBParameter("@Revision", DbType.Int32, revision);
      }
      if (carcassCode.Length > 0)
      {
        input[3] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, saleNo);
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNGetSearchInfoForEnquiryDetail", input);
      if (dsSource != null)
      {
        // Sale No
        if (saleNo.Length == 0)
        {
          ultComboSaleNo.DataSource = dsSource.Tables[0];
          ultComboSaleNo.DisplayLayout.Bands[0].ColHeadersVisible = false;
        }
        // Item Code
        if (itemCode.Length == 0)
        {
          ultComboItemCode.DataSource = dsSource.Tables[1];
          ultComboItemCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
        }
        // Revision
        if (revision == int.MinValue)
        {
          ultComboRevision.DataSource = dsSource.Tables[2];
          ultComboRevision.DisplayLayout.Bands[0].ColHeadersVisible = false;
        }
        // Carcass Code
        if (carcassCode.Length == 0)
        {
          ultComboCarcassCode.DataSource = dsSource.Tables[3];
          ultComboCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
        }
      }
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      string saleNo = Utility.GetSelectedValueUltraCombobox(ultComboSaleNo);
      string itemCode = Utility.GetSelectedValueUltraCombobox(ultComboItemCode);
      if (ultComboRevision.SelectedRow != null && ultComboItemCode.SelectedRow == null)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0011", "Item Code");
        return;
      }
      int revision = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultComboRevision));
      string carcssCode = Utility.GetSelectedValueUltraCombobox(ultComboCarcassCode);

      DBParameter[] input = new DBParameter[5];
      input[0] = new DBParameter("@EnquiryDetailPid", DbType.Int64, this.enquiryPid);
      if (saleNo.Length > 0)
      {
        input[1] = new DBParameter("@SaleNo", DbType.AnsiString, 16, saleNo);
      }
      if (itemCode.Length > 0)
      {
        input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      }
      if (revision != int.MinValue)
      {
        input[3] = new DBParameter("@Revision", DbType.Int32, revision);
      }
      if (carcssCode.Length > 0)
      {
        input[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, carcssCode);
      }

      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNMasterPlanInfo_Select", input);
      this.utrSaleOrderStatus.DataSource = dsSource.Tables[2];
      this.ultrItemStatus.DataSource = dsSource.Tables[3];
      this.ultrCarcassStatus.DataSource = dsSource.Tables[4];
    }

    private void ultComboSaleNo_ValueChanged(object sender, EventArgs e)
    {
      if (ultComboSaleNo.SelectedRow != null)
      {
        string saleNo = Utility.GetSelectedValueUltraCombobox(ultComboSaleNo);
        string itemCode = Utility.GetSelectedValueUltraCombobox(ultComboItemCode);
        int revision = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultComboRevision));
        string carcassCode = Utility.GetSelectedValueUltraCombobox(ultComboCarcassCode);
        this.LoadComboBoxData(saleNo, itemCode, revision, carcassCode);
        ultComboItemCode.Value = string.Empty;
        ultComboRevision.Value = string.Empty;
        ultComboCarcassCode.Value = string.Empty;
      }
    }

    private void ultComboItemCode_ValueChanged(object sender, EventArgs e)
    {
      if (ultComboItemCode.SelectedRow != null)
      {
        string saleNo = Utility.GetSelectedValueUltraCombobox(ultComboSaleNo);
        string itemCode = Utility.GetSelectedValueUltraCombobox(ultComboItemCode);
        int revision = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultComboRevision));
        string carcassCode = Utility.GetSelectedValueUltraCombobox(ultComboCarcassCode);
        this.LoadComboBoxData(saleNo, itemCode, revision, carcassCode);
        ultComboRevision.Value = string.Empty;
        ultComboCarcassCode.Value = string.Empty;
      }
    }

    private void ultComboRevision_ValueChanged(object sender, EventArgs e)
    {
      if (ultComboRevision.SelectedRow != null)
      {
        string saleNo = Utility.GetSelectedValueUltraCombobox(ultComboSaleNo);
        string itemCode = Utility.GetSelectedValueUltraCombobox(ultComboItemCode);
        int revision = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultComboRevision));
        string carcassCode = Utility.GetSelectedValueUltraCombobox(ultComboCarcassCode);

        if (itemCode.Length == 0 && revision != int.MinValue)
        {
          WindowUtinity.ShowMessageError("MSG0011", "Item Code");
          ultComboItemCode.Focus();
          goto finish;
        }

        this.LoadComboBoxData(saleNo, itemCode, revision, carcassCode);
        ultComboCarcassCode.Value = string.Empty;
      finish:
        ultComboRevision.Value = int.MinValue; ;
      }
    }

    private void ultComboCarcassCode_ValueChanged(object sender, EventArgs e)
    {
      if (ultComboCarcassCode.SelectedRow != null)
      {
        string saleNo = Utility.GetSelectedValueUltraCombobox(ultComboSaleNo);
        string itemCode = Utility.GetSelectedValueUltraCombobox(ultComboItemCode);
        int revision = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultComboRevision));
        string carcassCode = Utility.GetSelectedValueUltraCombobox(ultComboCarcassCode);
        this.LoadComboBoxData(saleNo, itemCode, revision, carcassCode);
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.LoadComboBoxData(string.Empty, string.Empty, int.MinValue, string.Empty);
      ultComboSaleNo.Value = string.Empty;
      ultComboItemCode.Value = string.Empty;
      ultComboRevision.Value = string.Empty;
      ultComboCarcassCode.Value = string.Empty;
    }
    int iexpire, ikeep, inonplan;
    private void ultrEnquirySchedule_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;
      iexpire = DBConvert.ParseInt(row.Cells["Expire"].Value.ToString());
      ikeep = DBConvert.ParseInt(row.Cells["Keep"].Value.ToString());
      inonplan = DBConvert.ParseInt(row.Cells["NonPlan"].Value.ToString());
      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "expire":
          if (DBConvert.ParseInt(row.Cells["Expire"].Value) == 0 || DBConvert.ParseInt(row.Cells["Expire"].Value) == int.MinValue)
          {
            if (ikeep == 1)
            {
              row.Cells["Keep"].Value = 0;
            }
            if (inonplan == 1)
            {
              row.Cells["NonPlan"].Value = 0;
            }
          }
          break;

        case "keep":
          if (DBConvert.ParseInt(row.Cells["Keep"].Value) == 0 || DBConvert.ParseInt(row.Cells["Keep"].Value) == int.MinValue)
          {
            if (iexpire == 1)
            {
              row.Cells["Expire"].Value = 0;
            }
          }
          break;
        case "nonplan":
          if (DBConvert.ParseInt(row.Cells["NonPlan"].Value) == 0 || DBConvert.ParseInt(row.Cells["NonPlan"].Value) == int.MinValue)
          {
            if (iexpire == 1)
            {
              row.Cells["Expire"].Value = 0;
            }
          }
          break;
        case "scheduledate":
          if (!IsConfirm)
          {
            row.Cells["Qty"].Value = this.qty;
          }
          break;
        default:
          break;
      }
    }

    private void ultComboSaleNo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultComboCarcassCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultComboItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultComboRevision_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }


  }
}
