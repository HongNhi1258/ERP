/*
 * Created By   : Do Minh Tam
 * Created Date : 18/02/2014
 * Description  : Replace work order
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DaiCo.Application;
using DaiCo.Objects;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using Infragistics.Win;
using VBReport;
using DaiCo.Shared.DataBaseUtility;
using System.Diagnostics;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataSetSource.CustomerService;
using System.IO;

namespace DaiCo.CustomerService
{
  public partial class viewCSD_05_012 : MainUserControl
  {
    public viewCSD_05_012()
    {
      InitializeComponent();
      dt_OrderDate.Value = DateTime.MinValue;
    }

    # region Fied
    private bool load = false;
    private UltraGridCell cellDropDown = null;
    public string saleNo = string.Empty;
    public long saleOrderPid = long.MinValue;
    public long oldSaleOrderPid = long.MinValue;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool confirmSO = false;
    private int statusSO = 0;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    string strSaleOrderPid = ",";
    int checkSubCon = 0;
    #endregion Fied

    #region function

    /// <summary>
    /// Sale and close
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion function

    #region LoadData

    /// <summary>
    /// LoadDropdownItemCode
    /// </summary>    
    private void LoadDropdownItemCode(UltraDropDown udrpItemCode)
    {
      string commandText = string.Format(@"SELECT DISTINCT INF.ItemCode, BS.Name, BS.NameVN
                                           FROM TblBOMItemInfo INF INNER JOIN TblBOMItemBasic BS ON (INF.ItemCode = BS.ItemCode)");
      udrpItemCode.DataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpItemCode.ValueMember = "ItemCode";
      udrpItemCode.DisplayMember = "ItemCode";
      udrpItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      udrpItemCode.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      udrpItemCode.DisplayLayout.Bands[0].Columns["NameVN"].Hidden = true;
      udrpItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Width = 150;
    }

    /// <summary>
    /// LoadGrid
    /// </summary>    
    private void LoadGrid()
    {
      this.listDeletedPid = new ArrayList();
      string commandText = "spCSDReplaceSaleOrderDetail_Select";
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.oldSaleOrderPid);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable(commandText, inputParam);

      ultDetail.DataSource = dtSource;
      string strItemCode = "";
      int intRevision = 0;

      string value = Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType);
      if (value == "2")
      {
        ultDetail.DisplayLayout.Bands[0].Columns["SpecialDescription"].Hidden = false;
      }
      else
      {
        ultDetail.DisplayLayout.Bands[0].Columns["SpecialDescription"].Hidden = true;
      }

      //Customer, Type
      if (ultDetail.Rows.Count > 0)
      {
        this.EnableCustomer(false);
      }
      else
      {
        this.EnableCustomer(true);
      }
    }

    /// <summary>
    /// Enable or disable customer, type
    /// </summary>    
    private void EnableCustomer(bool status)
    {
      cmbCustomer.Enabled = status;
      chkDirect.Enabled = status;
      cmbDirectCus.Enabled = status;
      cmbType.Enabled = status;
    }

    /// <summary>
    /// LoadData
    /// </summary>    
    private void LoadData()
    {
      this.confirmSO = false;
      this.load = false;
        string commandText = "spCSDReplaceSaleOrder_Select";
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@SaleNo", DbType.AnsiString, 16, txtSOReplace.Text.Trim());
        DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable(commandText, inputParam);
      
        panSave.Visible = false;
        panSavecancel.Visible = false;
        if (dtSource == null || dtSource.Rows.Count == 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "SO Replace");
          return;
        }
        
        this.oldSaleOrderPid = DBConvert.ParseLong(dtSource.Rows[0]["SOPid"].ToString());
        if (this.oldSaleOrderPid > 0)
        {
          btnSaveAll.Visible = true;
        }
        else
        {
          btnSaveAll.Visible = false;
        }
        txtSaleNo.Text = dtSource.Rows[0]["SaleNo"].ToString();
        txtCustomerPoNo.Text = dtSource.Rows[0]["CustomerPONo"].ToString();
        try
        {
          cmbCustomer.SelectedValue = DBConvert.ParseLong(dtSource.Rows[0]["CustomerPid"].ToString()); 
          Shared.Utility.ControlUtility.LoadDirectCustomer(cmbDirectCus, DBConvert.ParseLong(dtSource.Rows[0]["CustomerPid"].ToString()));
        }
        catch { }
        if (DBConvert.ParseLong(dtSource.Rows[0]["DirectPid"].ToString()) > 0)
        {
          try
          {
            chkDirect.Enabled = true;
            chkDirect.Checked = true;
            cmbDirectCus.Visible = true;
            cmbDirectCus.SelectedValue = DBConvert.ParseLong(dtSource.Rows[0]["DirectPid"].ToString());
          }
          catch { }
        }
        dt_OrderDate.Value = DBConvert.ParseDateTime(dtSource.Rows[0]["OrderDate"].ToString(), formatConvert);
        try
        {
          cmbType.SelectedValue = DBConvert.ParseInt(dtSource.Rows[0]["Type"].ToString());
        }
        catch { }
        txtRemark.Text = dtSource.Rows[0]["Remark"].ToString();
        txtRef.Text = dtSource.Rows[0]["REFNo"].ToString().Trim();
        if (DBConvert.ParseDateTime(dtSource.Rows[0]["PODate"].ToString(), formatConvert) != DateTime.MinValue)
          ultPODate.Value = DBConvert.ParseDateTime(dtSource.Rows[0]["PODate"].ToString(), formatConvert);
        if (DBConvert.ParseDateTime(dtSource.Rows[0]["RequiredShipDate"].ToString(), formatConvert) != DateTime.MinValue)
          ultRequiredShipDate.Value = DBConvert.ParseDateTime(dtSource.Rows[0]["RequiredShipDate"].ToString(), formatConvert);
        txtcontract.Text = dtSource.Rows[0]["ContractNo"].ToString();
        txtDeliveryRequirement.Text = dtSource.Rows[0]["DeliveryRequirement"].ToString();
        txtPackingRequirement.Text = dtSource.Rows[0]["PackingRequirement"].ToString();
        txtDocumentRequirement.Text = dtSource.Rows[0]["DocumentRequirement"].ToString();
        // Truong Add(Fix)
        if (DBConvert.ParseInt(dtSource.Rows[0]["PriceType"].ToString()) == 1)
        {
          chkFix.Checked = true;
        }
        else
        {
          chkFix.Checked = false; 
        }
        // End
        if (DBConvert.ParseInt(dtSource.Rows[0]["Cancel"].ToString()) == 1)
        {
          chkCancel.Checked = true;
        }
        else
        {
          chkCancel.Checked = false;
        }
        if (DBConvert.ParseDateTime(dtSource.Rows[0]["ConfirmedShipDate"].ToString(), formatConvert) != DateTime.MinValue)
          ult_ConfirmedShipDate.Value = DBConvert.ParseDateTime(dtSource.Rows[0]["ConfirmedShipDate"].ToString(), formatConvert);
        this.confirmSO = (DBConvert.ParseInt(dtSource.Rows[0]["Status"].ToString()) >= 1);
        
        if (DBConvert.ParseInt(dtSource.Rows[0]["Status"].ToString()) == 1)
        {
          panSave.Visible = true;
          panSavecancel.Visible = false;
          this.chkCancel.Visible = true;
        }
        if (DBConvert.ParseInt(dtSource.Rows[0]["Status"].ToString()) == 0)
        {
          panSave.Visible = true;
          panSavecancel.Visible = false;
          this.chkCancel.Visible = true;
        }
        // Truong Add
        if (DBConvert.ParseInt(dtSource.Rows[0]["Status"].ToString()) > 1)
        {
          panSave.Visible = true;
        }
      txtSaleNo.Text = Shared.Utility.FunctionUtility.GetNewSaleOrderNo("SO");
      this.LoadGrid();
      this.load = true;
    }

    /// <summary>
    /// Load Revision By ItemCode
    /// </summary>
    /// <param name="udrpTemp"></param>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    private UltraDropDown LoadRevisionByItemCode(UltraDropDown udrpTemp, string itemCode)
    {
      if (udrpTemp == null)
      {
        udrpTemp = new UltraDropDown();
        this.Controls.Add(udrpTemp);
      }
      string commandText = string.Format("Select Revision From TblBOMRevision Where ItemCode = '{0}' Order by Revision DESC", itemCode);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpTemp.DataSource = dtSource;
      udrpTemp.ValueMember = "Revision";
      udrpTemp.DisplayMember = "Revision";
      udrpTemp.DisplayLayout.Bands[0].ColHeadersVisible = false;
      return udrpTemp;
    }
    #endregion LoadData

    #region CheckValid And Save

    private bool CheckIsValid(out string message)
    {
      message = string.Empty;

      // Customer's Po No :
      string text = txtCustomerPoNo.Text.Trim();
      if (text.Length == 0)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Customer's Po No");
        return false;
      }
      string strItemCode = "";
      int intRevision = int.MinValue;
      double price = double.MinValue;
      double secondPrice = double.MinValue;
      bool validPrice = true;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["Select"].Value.ToString()) == 1)
        {
          ultDetail.Rows[i].Cells["Price"].Appearance.BackColor = Color.White;
          ultDetail.Rows[i].Cells["SecondPrice"].Appearance.BackColor = Color.White;
          if (ultDetail.Rows[i].Cells["ItemCode"].Value.ToString() == strItemCode && DBConvert.ParseInt(ultDetail.Rows[i].Cells["Revision"].Value.ToString()) == intRevision)
          {
            if (price != DBConvert.ParseDouble(ultDetail.Rows[i].Cells["Price"].Value.ToString()))
            {
              ultDetail.Rows[i].Cells["Price"].Appearance.BackColor = Color.Yellow;
              validPrice = false;
            }
            else if (secondPrice != DBConvert.ParseDouble(ultDetail.Rows[i].Cells["SecondPrice"].Value.ToString()))
            {
              ultDetail.Rows[i].Cells["SecondPrice"].Appearance.BackColor = Color.Yellow;
              validPrice = false;
            }
          }
          else
          {
            price = DBConvert.ParseDouble(ultDetail.Rows[i].Cells["Price"].Value.ToString());
            secondPrice = DBConvert.ParseDouble(ultDetail.Rows[i].Cells["SecondPrice"].Value.ToString());
            strItemCode = ultDetail.Rows[i].Cells["ItemCode"].Value.ToString();
            intRevision = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Revision"].Value.ToString());
          }
        }
      }
      if (!validPrice)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Price");
        return false;
      }
      // Customer :
      long customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      if (customerPid == long.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Customer");
        return false;
      }

      //Direct Customer :
      if ((chkDirect.Checked) && (cmbDirectCus.SelectedIndex == 0))
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Direct Customer");
        return false;
      }

      // Type :
      int type = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      if (type == int.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Type");
        return false;
      }

      // Order Date :
      DateTime obj = DBConvert.ParseDateTime(dt_OrderDate.Value.ToString(), formatConvert);
      if (obj == DateTime.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Order Date");
        return false;
      }
      //Check enquiry, item, revision is added in database
      DataTable dtDetail = (DataTable)ultDetail.DataSource;
      if (dtDetail.Rows.Count <= 0)
      {
        message = Shared.Utility.FunctionUtility.GetMessage("WRN0007");
        return false;
      }

      string commandText = string.Empty;
      for (int i = 0; i < dtDetail.Rows.Count; i++)
      {
        if (dtDetail.Rows[i].RowState != DataRowState.Deleted)
        {
          commandText = string.Empty;
          commandText += " SELECT ISNULL(ItemKind, 0) TypeItem ";
          commandText += " FROM TblBOMItemBasic ";
          commandText += " WHERE ItemCode = '" + dtDetail.Rows[i]["ItemCode"].ToString() + "'";

          DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtCheck != null)
          {
            int typeItem = DBConvert.ParseInt(dtCheck.Rows[0][0].ToString());
            if (typeItem != 0 && !this.chkFix.Checked)
            {
              message = "Exist Type Of ItemCode is Custom Or Spare-Part Without Unfix Price !";
              return false;
            }
          }
        }
      }
      return true;
    }

    private bool SaveMain()
    {
      bool success = true;
      PLNSaleOrder saleOrder = new PLNSaleOrder();
      if (this.saleOrderPid != long.MinValue) //Update
      {
        saleOrder.Pid = this.saleOrderPid;
        saleOrder = (PLNSaleOrder)Shared.DataBaseUtility.DataBaseAccess.LoadObject(saleOrder, new string[] { "Pid" });
      }

      string text = txtCustomerPoNo.Text.Trim();
      saleOrder.CustomerPONo = text;

      long customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      saleOrder.CustomerPid = customerPid;

      if (chkDirect.Checked)
      {
        customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbDirectCus));
        saleOrder.DirectPid = customerPid;
      }
      else
      {
        saleOrder.DirectPid = long.MinValue;
      }

      DateTime orderDate = DBConvert.ParseDateTime(dt_OrderDate.Value.ToString(), formatConvert);
      if (orderDate != DateTime.MinValue)
      {
        saleOrder.OrderDate = orderDate;
      }

      int type = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      saleOrder.Type = type;

      saleOrder.DeleteFlag = 0;


      // Get status
      this.statusSO = saleOrder.Status;
      text = txtRemark.Text.Trim();
      saleOrder.Remark = text;
      // Hung them REFNo PODate RequiredShipDate ContractNo,ConfirmedShipDate,DeliveryRequirement,PackingRequirement,DocumentRequirement
      string REFNo = txtRef.Text.Trim();
      DateTime PODate = new DateTime();
      if (ultPODate.Value != null)
      {
        PODate = DBConvert.ParseDateTime(ultPODate.Value.ToString(), formatConvert);
      }
      DateTime RequiredShipDate = new DateTime();
      if (ultRequiredShipDate.Value != null)
      {
        RequiredShipDate = DBConvert.ParseDateTime(ultRequiredShipDate.Value.ToString(), formatConvert);
      }
      string ContractNo = txtcontract.Text.Trim();
      string DeliveryRequirement = txtDeliveryRequirement.Text.Trim();
      string PackingRequirement = txtPackingRequirement.Text.Trim();
      string DocumentRequirement = txtDocumentRequirement.Text.Trim();
      saleOrder.REFNo = REFNo;
      saleOrder.PODate = PODate;
      saleOrder.RequiredShipDate = RequiredShipDate;
      saleOrder.ContractNo = ContractNo;
      saleOrder.DeliveryRequirement = DeliveryRequirement;
      saleOrder.PackingRequirement = PackingRequirement;
      saleOrder.DocumentRequirement = DocumentRequirement;
      // Truong Add(Fix)
      if (chkFix.Checked)
      {
        saleOrder.PriceType = 1;
      }
      else
      {
        saleOrder.PriceType = 0;
      }
      // End
      if (this.saleOrderPid != long.MinValue) //Update
      {
        saleOrder.UpdateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
        saleOrder.UpdateDate = DateTime.Now;
        success = Shared.DataBaseUtility.DataBaseAccess.UpdateObject(saleOrder, new string[] { "Pid" });
      }
      else //Insert
      {
        saleOrder.SaleNo = Shared.Utility.FunctionUtility.GetNewSaleOrderNo("SO");
        saleOrder.CreateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
        saleOrder.CreateDate = DateTime.Now;
        saleOrder.Status = 0;
        this.saleOrderPid = Shared.DataBaseUtility.DataBaseAccess.InsertObject(saleOrder);
        if (this.saleOrderPid <= 0)
        {
          success = false;
        }
      }
      return success;
    }

    private bool SaveDetail()
    {
      string strListOfSwap = "";
      //Insert
      DataTable dtDetail = (DataTable)ultDetail.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (DBConvert.ParseInt(row["Select"].ToString()) == 1)
        {
          DBParameter[] param = new DBParameter[12];
          string storeName = string.Empty;
          storeName = "spPLNSaleOrderDetail_Insert";
          param[1] = new DBParameter("@SaleOrderPid", DbType.Int64, this.saleOrderPid);
          string text = row["ItemCode"].ToString().Trim();
          param[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, text);

          int revision = DBConvert.ParseInt(row["Revision"].ToString());
          param[3] = new DBParameter("@Revision", DbType.Int32, revision);

          int qty = DBConvert.ParseInt(row["Balance"].ToString());
          param[4] = new DBParameter("@Qty", DbType.Int32, qty);

          DateTime scheduleDelivery = DateTime.MinValue;
          try
          {
            if (row["ScheduleDate"].ToString().Trim().Length > 0)
            {
              scheduleDelivery = DBConvert.ParseDateTime(row["ScheduleDate"].ToString(), formatConvert);
            }
          }
          catch { }
          if (scheduleDelivery != DateTime.MinValue)
          {
            param[5] = new DBParameter("@ScheduleDelivery", DbType.DateTime, scheduleDelivery);
          }

          DateTime realDelivery = DateTime.MinValue;
          try
          {
            realDelivery = DBConvert.ParseDateTime(row["RealDelivery"].ToString(), formatConvert);
          }
          catch { }
          if (realDelivery != DateTime.MinValue)
          {
            param[6] = new DBParameter("@RealDelivery", DbType.DateTime, realDelivery);
          }

          double price = DBConvert.ParseDouble(row["Price"].ToString());
          if (price != double.MinValue)
          {
            param[7] = new DBParameter("@Price", DbType.Double, price);
          }

          double Secondprice = DBConvert.ParseDouble(row["SecondPrice"].ToString());
          if (Secondprice != double.MinValue)
          {
            param[8] = new DBParameter("@SecondPrice", DbType.Double, Secondprice);
          }

          text = row["Remark"].ToString().Trim();
          if (text.Length > 0)
          {
            param[9] = new DBParameter("@Remark", DbType.String, 4000, text);
          }

          text = row["SpecialDescription"].ToString().Trim();
          param[10] = new DBParameter("@SpecialDescription", DbType.String, 4000, text);

          param[11] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, param, outputParam);
          long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
          else
          {
            strListOfSwap += row["Pid"].ToString() + "," + result.ToString() + "@";
          }
        }
      }
      DBParameter[] paramReplace = new DBParameter[4];
      string storeNameReplace = "spPLNReplaceSaleOrderPrice";
      paramReplace[0] = new DBParameter("@SaleOrderPid", DbType.Int64, this.saleOrderPid);
      paramReplace[1] = new DBParameter("@OldSaleOrderPid", DbType.Int64, this.oldSaleOrderPid);
      paramReplace[2] = new DBParameter("@CreatedBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      paramReplace[3] = new DBParameter("@StringData", DbType.String, strListOfSwap);
      DBParameter[] outputParamReplace = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeNameReplace, paramReplace, outputParamReplace);
      long resultReplace = DBConvert.ParseLong(outputParamReplace[0].Value.ToString());
      if (resultReplace <= 0)
      {
        return false;
      }
      return true;
    }

    private void SaveData()
    {
      string message = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      success = this.SaveMain();
      if (!success)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        return;
      }
      else
      {
        success = this.SaveDetail();
        if (!success)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("WRN0004");
          return;
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      if (this.saleOrderPid > 0)
      {
        viewCSD_05_004 uc = new viewCSD_05_004();
        uc.saleOrderPid = this.saleOrderPid;
        Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE SALE ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
      }
      this.CloseTab();
      this.NeedToSave = false;
    }

    #endregion CheckValid And Save

    #region Events
    void viewCSD_05_012_Load(object sender, System.EventArgs e)
    {
      // Customer
      Shared.Utility.ControlUtility.LoadCustomer(cmbCustomer);
      cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
      // Type
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      this.load = true;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void ultDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = false;
      for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        Type colType = ultDetail.DisplayLayout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        string columnName = ultDetail.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if (string.Compare(columnName, "Select", true) == 0 || string.Compare(columnName, "IsHold", true) == 0
              || string.Compare(columnName, "Price", true) == 0 || string.Compare(columnName, "SecondPrice", true) == 0
              || string.Compare(columnName, "FOB Price", true) == 0 || string.Compare(columnName, "Customer Price", true) == 0
              || string.Compare(columnName, "Remark", true) == 0 || string.Compare(columnName, "Special Instruction", true) == 0
              || string.Compare(columnName, "RemarkForAccount", true) == 0 || string.Compare(columnName, "SpecialInstruction", true) == 0)
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.BackColor = Color.Aqua;
        }
        else
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.BackColor = Color.White;

        }
      }

      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["No"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Revision"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Select"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ScheduleDate"].Header.Caption = "ConfirmedShipDate";
      e.Layout.Bands[0].Columns["SpecialInstruction"].Header.Caption = "Special Instruction";
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Balance"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,###}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:$###,###.##}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["SecondAmount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[3].DisplayFormat = "{0:$###,###.##}";

      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Override.AllowDelete = (this.confirmSO ? DefaultableBoolean.False : DefaultableBoolean.True);
      e.Layout.Bands[0].Columns["No"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["No"].MinWidth = 40;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 40;
      e.Layout.Bands[0].Columns["Price"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Price"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Price"].Header.Caption = "FOB Price";
      e.Layout.Bands[0].Columns["SecondPrice"].Header.Caption = "Customer Price";
      e.Layout.Bands[0].Columns["SecondPrice"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["SecondPrice"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Amount"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Amount"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "FOB Amount";
      e.Layout.Bands[0].Columns["SecondAmount"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["SecondAmount"].MinWidth = 60;
      e.Layout.Bands[0].Columns["SecondAmount"].Header.Caption = "Customer Amount";
      e.Layout.Bands[0].Columns["ScheduleDate"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ScheduleDate"].MinWidth = 80;

    }

    private void ultDetail_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      this.NeedToSave = true;
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      DataTable dt;
      switch (columnName)
      {
        case "price":
          try
          {
            //if (checkSubCon == 1)
            //{
            //  return;
            //}
            e.Cell.Row.Cells["Amount"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Balance"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
            e.Cell.Row.Cells["Select"].Value = 1;
          }
          catch
          {
            e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
            e.Cell.Row.Cells["Select"].Value = 0;
          }
          break;
        case "secondprice":
          try
          {
            e.Cell.Row.Cells["SecondAmount"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Balance"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value.ToString());
            e.Cell.Row.Cells["Select"].Value = 1;
          }
          catch
          {
            e.Cell.Row.Cells["SecondAmount"].Value = DBNull.Value;
            e.Cell.Row.Cells["Select"].Value = 0;

          }
          break;
        case "qty":
          try
          {
            e.Cell.Row.Cells["Amount"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Balance"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
          }
          catch
          {
            e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
          }
          break;
        case "select":
          try
          {
            
            if (checkSubCon == 1)
            {
              return;
            }
            string strItemCode = e.Cell.Row.Cells["ItemCode"].Value.ToString();
            int intRevision = DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString());
            for (int i = 0; i < ultDetail.Rows.Count; i++)
            {
              if (ultDetail.Rows[i].Cells["ItemCode"].Value.ToString() == strItemCode && DBConvert.ParseInt(ultDetail.Rows[i].Cells["Revision"].Value.ToString()) == intRevision)
              {
                checkSubCon = 1;
                ultDetail.Rows[i].Cells["Select"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Select"].Value.ToString()); ;
              }
            }
            checkSubCon = 0;
          }
          catch
          {
          }
          break;
        
        default:
          break;
      }
    }

    private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.load == true)
      {
        this.NeedToSave = true;
        string value = Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType);
        if (value == "2")
        {
          ultDetail.DisplayLayout.Bands[0].Columns["SpecialDescription"].Hidden = false;
        }
        else
        {
          DataTable dtSource = (DataTable)ultDetail.DataSource;
          foreach (DataRow row in dtSource.Rows)
          {
            row["SpecialDescription"] = string.Empty;
          }
          ultDetail.DisplayLayout.Bands[0].Columns["SpecialDescription"].Hidden = true;
        }
      }
    }

    private void ultDetail_AfterCellActivate(object sender, EventArgs e)
    {
      if (cellDropDown != null)
      {
        cellDropDown.DroppedDown = true;
      }
    }

    private void ultDetail_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "itemcode":
          cellDropDown = e.Cell.Row.Cells["ItemCode"];
          break;
        case "revision":
          break;
        case "scheduledelivery":
          cellDropDown = e.Cell.Row.Cells["ScheduleDelivery"];
          break;
        default:
          cellDropDown = null;
          break;
      }
    }

    private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
      //Direct Customer
      if (this.load == true)
      {
        this.NeedToSave = true;
        long customerPid = long.MinValue;
        try
        {
          lbDirect.Visible = false;
          cmbDirectCus.Visible = false;
          chkDirect.Checked = false;

          customerPid = DBConvert.ParseLong(cmbCustomer.SelectedValue.ToString());
          string commandText = string.Format(@"SELECT Pid FROM TblCSDCustomerInfo Where ParentPid = {0}", customerPid);
          DataTable dtDirectCus = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
          if (dtDirectCus.Rows.Count > 0)
          {
            chkDirect.Enabled = true;
          }
          else
          {
            chkDirect.Enabled = false;
          }
        }
        catch
        {
          chkDirect.Enabled = false;
        }
        // Truong Add(Load Fix/ Unfix dua vao PriceType trong TblCSDCustomerInfo)
        string commandTextFix = string.Format(@"SELECT PriceType FROM TblCSDCustomerInfo WHERE Pid = {0}", customerPid);
        DataTable dtFix = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandTextFix);
        if (dtFix != null && dtFix.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtFix.Rows[0]["PriceType"].ToString()) == 1)
          {
            chkFix.Checked = true;
          }
          else
          {
            chkFix.Checked = false;
          }
        }
      }
    }

    private void chkDirect_CheckedChanged(object sender, EventArgs e)
    {
      if (chkDirect.Checked)
      {
        lbDirect.Visible = true;
        cmbDirectCus.Visible = true;
        try
        {
          long cusPid = DBConvert.ParseLong(cmbCustomer.SelectedValue.ToString());
          Shared.Utility.ControlUtility.LoadDirectCustomer(cmbDirectCus, cusPid);
        }
        catch { }
      }
      else
      {
        lbDirect.Visible = false;
        cmbDirectCus.Visible = false;
      }
    }

    private void btnSaveAll_Click(object sender, EventArgs e)
    {
      this.SaveData();

    }

    private void ultDetail_MouseClick(object sender, MouseEventArgs e)
    {
      DaiCo.Shared.Utility.ControlUtility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      DaiCo.Shared.Utility.ControlUtility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultDetail_AfterSortChange(object sender, BandEventArgs e)
    {
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        ultDetail.Rows[i].Cells["No"].Value = i + 1;
      }
    }

    private void txtRevision_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
      {
        e.Handled = true;
      }
    }

    private void btnAddReplace_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      if (this.load)
      {
        this.NeedToSave = true;
        long customerPid = long.MinValue;

        // Truong Add(Load Fix/ Unfix dua vao PriceType trong TblCSDCustomerInfo)
        if (cmbDirectCus.SelectedIndex > 0)
        {
          customerPid = DBConvert.ParseLong(cmbDirectCus.SelectedValue.ToString());
        }
        else
        {
          customerPid = DBConvert.ParseLong(cmbCustomer.SelectedValue.ToString());
        }
        string commandTextFix = string.Format(@"SELECT PriceType FROM TblCSDCustomerInfo WHERE Pid = {0}", customerPid);
        DataTable dtFix = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandTextFix);
        if (dtFix != null && dtFix.Rows.Count > 0)
        {
          if (DBConvert.ParseInt(dtFix.Rows[0]["PriceType"].ToString()) == 1)
          {
            chkFix.Checked = true;
          }
          else
          {
            chkFix.Checked = false;
          }
        }
        // End
      }
    }

    #endregion Events

  }
}
