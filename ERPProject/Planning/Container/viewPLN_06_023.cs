/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: viewPLN_06_023
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_06_023 : MainUserControl
  {
    #region Field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    private IList listDeletedPid = new ArrayList();
    private IList listDeletedBox = new ArrayList();
    public long shipmentPid = long.MinValue;
    private bool isPlaRight = false;
    private bool isProRight = false;
    private int boxPanelH = 0;
    private int boxItemH = 0;
    private bool isFirstLoadData = true;
    #endregion Field

    #region Init
    public viewPLN_06_023()
    {
      InitializeComponent();
    }

    private void viewPLN_06_023_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      this.SetAutoAskSaveWhenCloseForm(uegMaster);
      this.InitData();
      this.LoadData();

      ////appData.ReadXml(string.Format("{0}/data.xml", System.Windows.Forms.Application.StartupPath));
      //string cmd = string.Format(@"SELECT Code, English, Vietnamese
      //                            FROM tblCMSDefineLanguages");
      //DataTable dt = SqlDataBaseAccess.SearchCommandTextDataTable(cmd);
      //ugrdData.DataSource = dt;

      //pathEN = System.Windows.Forms.Application.StartupPath + "/resource.en-US.resources";
      //pathVI = System.Windows.Forms.Application.StartupPath + "/resource.vi-US.resources";

      //File.Delete(pathEN);
      //File.Delete(pathVI);
      //ResourceWriter ren = new ResourceWriter(System.Windows.Forms.Application.StartupPath + "/resource.en-US.resources");
      //ResourceWriter rvi = new ResourceWriter(System.Windows.Forms.Application.StartupPath + "/resource.vi-US.resources");
      //foreach (DataRow row in dt.Rows)
      //{
      //  ren.AddResource(row["Code"].ToString(), row["English"].ToString());
      //  rvi.AddResource(row["Code"].ToString(), row["Vietnamese"].ToString());
      //}
      //ren.Generate();
      //ren.Close();
      //rvi.Generate();
      //rvi.Close();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Set Auto Ask Save Data When User Close Form
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoAskSaveWhenCloseForm(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.TextChanged += new System.EventHandler(this.Object_Changed);
        }
        else
        {
          this.SetAutoAskSaveWhenCloseForm(ctr);
        }
      }
    }

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
      // Load Customer List
      string commandText = @"Select Pid, CustomerCode Code, Name, (CustomerCode + ' - ' + Name) Display 
                            From TblCSDCustomerInfo 
                            Where ParentPid Is Null And Kind NOT IN (1, 2)
                            Order By CustomerCode";
      DataTable dtCustomer = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCustomer, dtCustomer, "Pid", "Display", false, new string[] { "Pid", "Display" });

      // Load Container Type
      commandText = @"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 1";
      DataTable dtContainerType = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbContainerType, dtContainerType, "Code", "Value", false, "Code");

      // Load Item List in grid
      commandText = @"SELECT Pid Wo, WoOld FROM TblPLNWorkOrder WHERE WoOld IS NOT NULL ORDER BY WoOld DESC";
      DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataTable dtWOForBox = Utility.CloneTable(dtWO);
      Utility.LoadUltraCombo(ucbWO, dtWO, "Wo", "WoOld", false, "Wo");
      Utility.LoadUltraCombo(ucbWOForBox, dtWOForBox, "Wo", "WoOld", false, "Wo");

      // Load Item List in grid
      commandText = @"SELECT ItemCode, OldCode, Name, (OldCode + ' - ' + Name) DisplayText FROM TblBOMItemBasic WHERE LEN(OldCode) > 0 ORDER BY DisplayText";
      DataTable dtItemList = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbItemList, dtItemList, "ItemCode", "DisplayText", false, new string[] { "ItemCode", "DisplayText" });
      ucbItemList.DisplayLayout.Bands[0].Columns["OldCode"].Width = 110;

      // Set default Create Date
      udeCreateDate.Value = DateTime.Now.Date;

      // Format Date
      udeCreateDate.FormatString = ConstantClass.FORMAT_DATETIME;
      udeCreateDate.FormatProvider = new System.Globalization.CultureInfo("vi-VN", true);
      udePlanShipDate.FormatString = ConstantClass.FORMAT_DATETIME;
      udePlanShipDate.FormatProvider = new System.Globalization.CultureInfo("vi-VN", true);
      udeActualShipDate.FormatString = ConstantClass.FORMAT_DATETIME;
      udeActualShipDate.FormatProvider = new System.Globalization.CultureInfo("vi-VN", true);
      udeETD.FormatString = ConstantClass.FORMAT_DATETIME;
      udeETD.FormatProvider = new System.Globalization.CultureInfo("vi-VN", true);

      // Set Language
      this.SetLanguage();

      // Load Right of PLA & PRO
      this.LoadRightOfPlaAndPro();
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void LoadRightOfPlaAndPro()
    {
      if (btnPerPla.Visible)
      {
        this.isPlaRight = true;
      }
      if (btnPerPro.Visible)
      {
        this.isProRight = true;
      }
      btnPerPla.Visible = false;
      btnPerPro.Visible = false;
    }

    private void SetStatusControl()
    {
      bool isConfirm = uceConfirm.Checked;
      txtContainerNumber.ReadOnly = isConfirm;
      ucbCustomer.ReadOnly = isConfirm;
      ucbContainerType.ReadOnly = isConfirm;
      udePlanShipDate.ReadOnly = isConfirm;
      udeActualShipDate.ReadOnly = isConfirm;
      udeETD.ReadOnly = isConfirm;
      uteRemark.ReadOnly = isConfirm;
      txtBarcode.ReadOnly = isConfirm;
      uceConfirm.Enabled = !isConfirm;
      btnSave.Enabled = !isConfirm;
      if (isConfirm)
      {
        ugdBoxList.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        ugdBoxList.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
        ugdData.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
        ugdData.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
      }

      // Set status control base on right
      if (this.isPlaRight && !this.isProRight)
      {
        udeActualShipDate.ReadOnly = true;
        txtBarcode.ReadOnly = true;
        ugdBoxList.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
      }
      if (!this.isPlaRight && this.isProRight)
      {
        txtShipmentCode.ReadOnly = true;
        ucbCustomer.ReadOnly = true;
        ucbContainerType.ReadOnly = true;
        udePlanShipDate.ReadOnly = true;
        uteRemark.ReadOnly = true;
        uceConfirm.Enabled = false;
        ugdData.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
        ugdData.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
        txtBarcode.Focus();
      }
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
        DataRow rowMain = dtMain.Rows[0];
        txtShipmentCode.Text = rowMain["ShipmentCode"].ToString();
        txtContainerNumber.Text = rowMain["ContainerNumber"].ToString();
        ucbCustomer.Value = rowMain["CustomerPid"];
        udeCreateDate.Value = rowMain["CreateDate"];
        ucbContainerType.Value = rowMain["ContainerType"];
        udePlanShipDate.Value = rowMain["PlanShipDate"];
        udeActualShipDate.Value = rowMain["ActualShipDate"];
        udeETD.Value = rowMain["ETD"];
        uteRemark.Value = rowMain["Remark"];
        uceConfirm.Checked = (DBConvert.ParseInt(rowMain["Status"]) == 1 ? true : false);
        this.shipmentPid = DBConvert.ParseLong(rowMain["Pid"]);
      }
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ShipmentPid", DbType.Int64, this.shipmentPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNShipment_Select", inputParam);
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        this.LoadMainData(dsSource.Tables[0]);
        ugdBoxList.DataSource = dsSource.Tables[1];
        ugdData.DataSource = dsSource.Tables[2];
        this.ShowRowCount();
      }

      this.SetStatusControl();
      this.isFirstLoadData = false;
      this.NeedToSave = false;
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      //1. Check master
      if (txtShipmentCode.Text.Trim().Length == 0)
      {
        errorMessage = rm.GetString("ShipmentCode", ConstantClass.CULTURE);
        return false;
      }
      if (ucbCustomer.SelectedRow == null)
      {
        errorMessage = rm.GetString("Customer", ConstantClass.CULTURE);
        return false;
      }
      if (udeCreateDate.Value == null)
      {
        errorMessage = rm.GetString("CreateDate", ConstantClass.CULTURE);
        return false;
      }
      if (udePlanShipDate.Value == null)
      {
        errorMessage = rm.GetString("PlanShipDate", ConstantClass.CULTURE);
        return false;
      }

      //2. Check detail
      //2.1. Check WO for box
      for (int i = 0; i < ugdBoxList.Rows.Count; i++)
      {
        if (ugdBoxList.Rows[i].Cells["Wo"].Value == DBNull.Value)
        {
          errorMessage = string.Format("{0} tại dòng {1} {2}", rm.GetString("WorkOrder", ConstantClass.CULTURE), i + 1, rm.GetString("BoxList", ConstantClass.CULTURE));
          return false;
        }
        if (uceConfirm.Checked)
        {
          if (DBConvert.ParseInt(ugdBoxList.Rows[i].Cells["IsError"].Value) == 1)
          {
            errorMessage = string.Format("Không thể hoàn thành. Có lỗi tại dòng {0} {1}", i + 1, rm.GetString("BoxList", ConstantClass.CULTURE));
            return false;
          }
        }
      }
      //2.2. Check Invalid Item info
      int planQty = 0;
      int actualQty = 0;
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        if (ugdData.Rows[i].Cells["Wo"].Value == DBNull.Value)
        {
          errorMessage = string.Format("{0} tại dòng {1}", rm.GetString("WorkOrder", ConstantClass.CULTURE), i + 1);
          return false;
        }
        if (ugdData.Rows[i].Cells["ItemCode"].Value == DBNull.Value)
        {
          errorMessage = string.Format("{0} tại dòng {1}", rm.GetString("Item", ConstantClass.CULTURE), i + 1);
          return false;
        }
        planQty = DBConvert.ParseInt(ugdData.Rows[i].Cells["PlanQty"].Value);
        if (planQty <= 0)
        {
          errorMessage = string.Format("{0} tại dòng {1}", rm.GetString("PlanQty", ConstantClass.CULTURE), i + 1);
          return false;
        }
        actualQty = DBConvert.ParseInt(ugdData.Rows[i].Cells["ActualQty"].Value);
        if (actualQty > planQty)
        {
          errorMessage = string.Format("Lỗi tại dòng {0}! {1} không thể lớn hơn {2}", i + 1, rm.GetString("ActualQty", ConstantClass.CULTURE), rm.GetString("PlanQty", ConstantClass.CULTURE), i);
          return false;
        }
      }
      //2.3. Check duplicate wo, item
      DataTable dtItemList = (DataTable)ugdData.DataSource;
      bool isDuplicate = false;
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        DataRow[] rows = dtItemList.Select(string.Format("Wo = {0} AND ItemCode = '{1}'", ugdData.Rows[i].Cells["Wo"].Value, ugdData.Rows[i].Cells["ItemCode"].Value));
        if (rows.Length > 1)
        {
          ugdData.Rows[i].Appearance.BackColor = Color.Yellow;
          isDuplicate = true;
        }
      }
      if (isDuplicate)
      {
        errorMessage = string.Format("Lỗi! {0}, {1} bị lặp lại.", rm.GetString("WorkOrder", ConstantClass.CULTURE), rm.GetString("Item", ConstantClass.CULTURE));
        return false;
      }

      return true;
    }

    private bool SaveMain()
    {
      string containerNumber = txtContainerNumber.Text.Trim();
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      int containerType = DBConvert.ParseInt(ucbContainerType.Value);
      DateTime planShipDate = DateTime.MinValue;
      if (udePlanShipDate.Value != null)
      {
        planShipDate = DBConvert.ParseDateTime(udePlanShipDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      DateTime actualShipDate = DateTime.MinValue;
      if (udeActualShipDate.Value != null)
      {
        actualShipDate = DBConvert.ParseDateTime(udeActualShipDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      DateTime eTD = DateTime.MinValue;
      if (udeETD.Value != null)
      {
        eTD = DBConvert.ParseDateTime(udeETD.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
      }
      string remark = uteRemark.Text.Trim();
      int isConfirm = (uceConfirm.Checked ? 1 : 0);
      string storeName = "spPLNShipment_Edit";
      int paramNumber = 10;
      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (this.shipmentPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@ShipmentPid", DbType.Int64, this.shipmentPid);
      }
      inputParam[1] = new DBParameter("@ContainerNumber", DbType.String, 64, containerNumber);
      inputParam[2] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      if (containerType != int.MinValue)
      {
        inputParam[3] = new DBParameter("@ContainerType", DbType.Int32, containerType);
      }
      if (planShipDate != DateTime.MinValue)
      {
        inputParam[4] = new DBParameter("@PlanShipDate", DbType.DateTime, planShipDate);
      }
      if (actualShipDate != DateTime.MinValue)
      {
        inputParam[5] = new DBParameter("@ActualShipDate", DbType.DateTime, actualShipDate);
      }
      if (actualShipDate != DateTime.MinValue)
      {
        inputParam[5] = new DBParameter("@ActualShipDate", DbType.DateTime, actualShipDate);
      }
      if (eTD != DateTime.MinValue)
      {
        inputParam[6] = new DBParameter("@ETD", DbType.DateTime, eTD);
      }
      if (remark.Length > 0)
      {
        inputParam[7] = new DBParameter("@Remark", DbType.String, 512, remark);
      }
      inputParam[8] = new DBParameter("@Status", DbType.Int32, isConfirm);
      inputParam[9] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
      {
        this.shipmentPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
        return true;
      }
      return false;
    }

    private bool SaveDetail()
    {
      bool success = true;
      // 1. Delete      
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      for (int i = 0; i < listDeletedPid.Count; i++)
      {
        DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNShipmentDetail_Delete", deleteParam, outputParam);
        if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
        {
          success = false;
        }
      }
      // 2.1. Insert/Update BOX  
      DataTable dtBoxList = (DataTable)ugdBoxList.DataSource;
      foreach (DataRow row in dtBoxList.Rows)
      {
        if (row.RowState == DataRowState.Modified)
        {
          long pid = DBConvert.ParseLong(row["Pid"]);
          long wo = DBConvert.ParseLong(row["Wo"]);

          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          inputParam[1] = new DBParameter("@Wo", DbType.Int64, wo);
          inputParam[2] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spPLNShipmentBoxBarcode_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      // 2.2. Insert/Update Item List  
      DataTable dtDetail = (DataTable)ugdData.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          long wo = DBConvert.ParseLong(row["Wo"]);
          string pONo = row["PONo"].ToString().Trim();
          string itemCode = row["ItemCode"].ToString();
          double planQty = DBConvert.ParseDouble(row["PlanQty"]);
          double actualQty = DBConvert.ParseDouble(row["ActualQty"]);
          string remark = row["Remark"].ToString();

          DBParameter[] inputParam = new DBParameter[9];
          long pid = DBConvert.ParseLong(row["Pid"].ToString());
          if (row.RowState == DataRowState.Modified) // Update
          {
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
          }
          inputParam[1] = new DBParameter("@ShipmentPid", DbType.Int64, this.shipmentPid);
          if (wo != long.MinValue)
          {
            inputParam[2] = new DBParameter("@Wo", DbType.Int64, wo);
          }
          if (pONo.Length > 0)
          {
            inputParam[3] = new DBParameter("@PONo", DbType.AnsiString, 50, pONo);
          }
          inputParam[4] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
          inputParam[5] = new DBParameter("@PlanQty", DbType.Double, planQty);
          if (actualQty > 0)
          {
            inputParam[6] = new DBParameter("@ActualQty", DbType.Double, actualQty);
          }
          inputParam[7] = new DBParameter("@Remark", DbType.String, 256, remark);
          inputParam[8] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          DataBaseAccess.ExecuteStoreProcedure("spPLNShipmentDetail_Edit", inputParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
      }
      return success;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        if (this.SaveMain())
        {
          success = this.SaveDetail();
        }
        else
        {
          success = false;
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    /// <summary>
    /// Set Auto Add 4 blank before text of button
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetBlankForTextOfButton(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count > 0)
        {
          this.SetBlankForTextOfButton(ctr);
        }
        else if (ctr.GetType().Name == "Button")
        {
          ctr.Text = string.Format("{0}{1}", "    ", ctr.Text);
        }
      }
    }

    private void SetLanguage()
    {
      uegMaster.Text = rm.GetString("Master", ConstantClass.CULTURE);
      uegBoxList.Text = rm.GetString("BoxList", ConstantClass.CULTURE);
      uegItemList.Text = rm.GetString("ItemList", ConstantClass.CULTURE);
      lblShipmentCode.Text = rm.GetString("ShipmentCode", ConstantClass.CULTURE);
      lblContainerNumber.Text = rm.GetString("ContainerNumber", ConstantClass.CULTURE);
      lblCustomer.Text = rm.GetString("Customer", ConstantClass.CULTURE);
      lblCreateDate.Text = rm.GetString("CreateDate", ConstantClass.CULTURE);
      lblContainerType.Text = rm.GetString("ContainerType", ConstantClass.CULTURE);
      lblPlanShipDate.Text = rm.GetString("PlanShipDate", ConstantClass.CULTURE);
      lblActualShipDate.Text = rm.GetString("ActualShipDate", ConstantClass.CULTURE);
      lblRemark.Text = rm.GetString("Remark", ConstantClass.CULTURE);

      btnSave.Text = rm.GetString("Save", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);
      uceConfirm.Text = rm.GetString("Confirm", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }

    private void ShowRowCount()
    {
      uegBoxList.Text = string.Format("{0}. {1}: {2}", rm.GetString("BoxList", ConstantClass.CULTURE), rm.GetString("Count", ConstantClass.CULTURE), ugdBoxList.Rows.FilteredInRowCount);
      lblCount.Text = string.Format("{0}: {1}", rm.GetString("Count", ConstantClass.CULTURE), ugdData.Rows.FilteredInRowCount);
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      for (int i = 0; i < ugdData.Rows.Count; i++)
      {
        ugdData.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }

      if (this.isFirstLoadData)
      {
        e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
        e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
        e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
        e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
        e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

        // Allow update, delete, add new
        e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

        // Read only
        e.Layout.Bands[0].Columns["RemainQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["ActualQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

        for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
        {
          // Set color for edit & read only cell
          if (e.Layout.Bands[0].Columns[i].CellActivation == Activation.ActivateOnly)
          {
            e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightGray;
          }
          // Set Align column
          Type colType = e.Layout.Bands[0].Columns[i].DataType;
          if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
          {
            e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
          }
        }

        // Set Combobox for column
        e.Layout.Bands[0].Columns["ItemCode"].ValueList = ucbItemList;
        e.Layout.Bands[0].Columns["Wo"].ValueList = ucbWO;

        // Set auto complete combo in grid
        e.Layout.Bands[0].Columns["Wo"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
        e.Layout.Bands[0].Columns["Wo"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
        e.Layout.Bands[0].Columns["ItemCode"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
        e.Layout.Bands[0].Columns["ItemCode"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;

        // Set caption column      
        e.Layout.Bands[0].Columns["Wo"].Header.Caption = rm.GetString("WorkOrder", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["PONo"].Header.Caption = rm.GetString("PONo", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = rm.GetString("Item", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["PlanQty"].Header.Caption = rm.GetString("PlanQty", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["ActualQty"].Header.Caption = rm.GetString("ActualQty", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["RemainQty"].Header.Caption = rm.GetString("RemainQty", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["Remark"].Header.Caption = rm.GetString("Remark", ConstantClass.CULTURE);

        // Hide column
        e.Layout.Bands[0].Columns["Pid"].Hidden = true;

        // Set With     
        e.Layout.Bands[0].Columns["Wo"].Width = 30;
        e.Layout.Bands[0].Columns["PONo"].Width = 40;
        e.Layout.Bands[0].Columns["PlanQty"].Width = 30;
        e.Layout.Bands[0].Columns["ActualQty"].Width = 35;
        e.Layout.Bands[0].Columns["RemainQty"].Width = 30;
        e.Layout.Bands[0].Columns["Remark"].Width = 50;

        /*

        // Enable support for displaying errors through IDataErrorInfo interface. By default
        // the functionality is disabled.
        e.Layout.Override.SupportDataErrorInfo = SupportDataErrorInfo.RowsAndCells;

        // Set data error related appearances.
        e.Layout.Override.DataErrorCellAppearance.ForeColor = Color.Red;
        e.Layout.Override.DataErrorRowAppearance.BackColor = Color.LightYellow;
        e.Layout.Override.DataErrorRowSelectorAppearance.BackColor = Color.Green;

        // Make the row selectors bigger so they can accomodate the data error icon as 
        // well active row indicator.
        e.Layout.Override.RowSelectorWidth = 32;   

        // Set Align
        e.Layout.Bands[0].Columns[""].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns[""].CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;

        // Set Column Style
        e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

        // Add Column Selected
        if (!e.Layout.Bands[0].Columns.Exists("Selected"))
        {
          UltraGridColumn checkedCol = e.Layout.Bands[0].Columns.Add();
          checkedCol.Key = "Selected";
          checkedCol.Header.Caption = string.Empty;
          checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;        
          checkedCol.DataType = typeof(bool);
          checkedCol.Header.VisiblePosition = 0;
        } 

        // Set color
        ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
        ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
        e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;

        // Set header height
        e.Layout.Bands[0].ColHeaderLines = 2;

        // Read only
        e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridInformation.Rows[0].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        ultraGridInformation.Rows[0].Cells[""].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

        // Format date (dd/MM/yy)
        e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
        e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

        // Set language
        e.Layout.Bands[0].Columns["EID"].Header.Caption = rm.GetString("EID", ConstantClass.CULTURE);
        */
      }
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      //string value = e.NewValue.ToString();
      if ((colName == "Wo") && e.Cell.Text.Length > 0 && ucbWO.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText(rm.GetString("ERR_WorkOrder", ConstantClass.CULTURE));
        e.Cancel = true;
      }
      if ((colName == "ItemCode") && e.Cell.Text.Length > 0 && ucbItemList.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText(rm.GetString("ERR_Item", ConstantClass.CULTURE));
        e.Cancel = true;
      }
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
      this.ConfirmToCloseTab();
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

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdData, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdData);
      }
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdData.Selected.Rows.Count > 0 || ugdData.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdData, new Point(e.X, e.Y));
        }
      }
    }

    private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter && txtBarcode.Text.Trim().Length > 0 && this.shipmentPid > 0)
      {
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@ShipmentPid", DbType.Int64, this.shipmentPid);
        inputParam[1] = new DBParameter("@Barcode", DbType.AnsiString, 50, txtBarcode.Text.Trim());
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNShipmentBoxBarcode_Check", inputParam);
        if (dsSource != null && dsSource.Tables.Count > 1)
        {
          ugdBoxList.DataSource = dsSource.Tables[0];
          ugdData.DataSource = dsSource.Tables[1];
          this.ShowRowCount();
        }
        txtBarcode.Clear();
        txtBarcode.Focus();
      }
    }

    private void ugdBoxList_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      // Error Rows
      for (int i = 0; i < ugdBoxList.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ugdBoxList.Rows[i].Cells["IsError"].Value) == 1)
        {
          ugdBoxList.Rows[i].Appearance.BackColor = Color.Yellow;
        }
      }

      if (this.isFirstLoadData)
      {
        e.Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
        e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
        e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
        e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
        e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

        // Allow update, delete, add new
        e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
        e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

        // Set combobox to Wo cell
        e.Layout.Bands[0].Columns["Wo"].ValueList = ucbWOForBox;

        for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
        {
          // Set Align column
          Type colType = e.Layout.Bands[0].Columns[i].DataType;
          if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
          {
            e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
            e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
          }

          // Read only columns
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
        e.Layout.Bands[0].Columns["Wo"].CellActivation = Activation.AllowEdit;

        // Hide column
        e.Layout.Bands[0].Columns["Pid"].Hidden = true;
        e.Layout.Bands[0].Columns["IsError"].Hidden = true;

        // Set caption column      
        e.Layout.Bands[0].Columns["Barcode"].Header.Caption = rm.GetString("Barcode", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["BoxName"].Header.Caption = rm.GetString("BoxName", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["Item"].Header.Caption = rm.GetString("Item", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["BoxQty"].Header.Caption = rm.GetString("BoxQty", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["Wo"].Header.Caption = rm.GetString("WorkOrder", ConstantClass.CULTURE);
        e.Layout.Bands[0].Columns["ErrorMessage"].Header.Caption = rm.GetString("ErrorMessage", ConstantClass.CULTURE);

        // Set With             
        e.Layout.Bands[0].Columns["Barcode"].Width = 40;
        e.Layout.Bands[0].Columns["BoxQty"].Width = 30;
        e.Layout.Bands[0].Columns["BoxName"].Width = 50;
        e.Layout.Bands[0].Columns["Wo"].Width = 30;
        e.Layout.Bands[0].Columns["ErrorMessage"].Width = 50;

        // Set auto complete combo in grid
        e.Layout.Bands[0].Columns["Wo"].AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
        e.Layout.Bands[0].Columns["Wo"].AutoSuggestFilterMode = AutoSuggestFilterMode.Contains;
      }
    }

    private void ugdBoxList_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      int countRows = e.Rows.Length;
      string message = string.Format("Bạn muốn xóa {0} dòng?\n Chọn Yes nếu bạn chắc chắn muốn xóa hoặc No để thoát.", countRows);
      if (MessageBox.Show(message, "Xóa dữ liệu", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedBox.Add(pid);
        }
      }
    }

    private void ugdBoxList_AfterRowsDeleted(object sender, EventArgs e)
    {
      for (int i = 0; i < this.listDeletedBox.Count; i++)
      {
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, listDeletedBox[i]);
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNShipmentBoxBarcode_Delete", inputParam);
        if (dsSource != null && dsSource.Tables.Count > 1)
        {
          ugdBoxList.DataSource = dsSource.Tables[0];
          ugdData.DataSource = dsSource.Tables[1];
          this.ShowRowCount();
        }
      }
      this.listDeletedBox = new ArrayList();
    }


    private void uegBoxList_ExpandedStateChanging(object sender, CancelEventArgs e)
    {
      if (uegBoxList.Expanded)
      {
        this.boxPanelH = spcDetail.SplitterDistance;
        if (!uegItemList.Expanded)
        {
          this.boxPanelH = (spcDetail.SplitterDistance / 2);
          uegItemList.Expanded = true;
        }
        spcDetail.SplitterDistance = 25;

      }
      else
      {
        spcDetail.SplitterDistance = this.boxPanelH;
      }
    }

    private void uegItemList_ExpandedStateChanging(object sender, CancelEventArgs e)
    {
      if (uegItemList.Expanded)
      {
        this.boxItemH = spcDetail.SplitterDistance;
        if (!uegBoxList.Expanded)
        {
          this.boxItemH = (spcDetail.Panel2.Height / 2);
          uegBoxList.Expanded = true;
        }
        spcDetail.SplitterDistance = this.boxItemH + spcDetail.Panel2.Height;
      }
      else
      {
        spcDetail.SplitterDistance = this.boxItemH;
      }
    }

    private void udePlanShipDate_ValueChanged(object sender, EventArgs e)
    {
      if (udePlanShipDate.Value != null && this.shipmentPid == long.MinValue)
      {
        DateTime planShipDate = DBConvert.ParseDateTime(udePlanShipDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
        string shipmentCode = DataBaseAccess.ExecuteScalarCommandText(string.Format("SELECT dbo.FPLNGetShipmentCode({0}, {1})", planShipDate.Month, planShipDate.Year)).ToString();
        txtShipmentCode.Text = shipmentCode;
      }
    }

    private void ugdBoxList_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      //string value = e.NewValue.ToString();
      if ((colName == "Wo") && e.Cell.Text.Length > 0 && ucbWOForBox.SelectedRow == null)
      {
        WindowUtinity.ShowMessageErrorFromText("Lỗi! LSX không đúng.");
        e.Cancel = true;
      }
    }

    private void ugdBoxList_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }
    #endregion Event    
  }
}
