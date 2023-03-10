/*
 * Created By   : Nguyen Van Tron
 * Created Date : 28/06/2010
 * Description  : Create or Update Sale Order (Customer Service Department)
 * Update       : Tran Hung
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
  public partial class viewCSD_05_004 : MainUserControl
  {
    public viewCSD_05_004()
    {
      InitializeComponent();
      //ultOrderDate.Value = DateTime.MinValue;
    }
    void viewCSD_05_004_Load(object sender, System.EventArgs e)
    {
      // Customer
      Shared.Utility.ControlUtility.LoadCustomer(cmbCustomer);
      cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
      // Type
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      //EnvironmentalStatus
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbEnvironmentalStatus, Shared.Utility.ConstantClass.GROUP_EnvironmentalStatus);

      this.LoadData();
      this.load = true;
    }

    # region Fied
    private bool load = false;
    private UltraGridCell cellDropDown = null;
    public string saleNo = string.Empty;
    public long saleOrderPid = long.MinValue;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool confirmSO = false;
    private int statusSO = 0;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    string strSaleOrderPid = ",";
    private int CountCheck = 0;
    #endregion Fied

    #region function

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
      string commandText = "spCSDSaleOrderDetail_Select";
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.saleOrderPid);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable(commandText, 600, inputParam);

      ultDetail.DataSource = dtSource;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        try
        {
          if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["Qty"].Value.ToString()) > 0 && DBConvert.ParseDouble(ultDetail.Rows[i].Cells["Price"].Value.ToString()) > 0)
          {
            ultDetail.Rows[i].Cells["Amount"].Value = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(ultDetail.Rows[i].Cells["Price"].Value.ToString());
          }
          else
          {
            ultDetail.Rows[i].Cells["Amount"].Value = DBNull.Value;
          }
        }
        catch
        {
          ultDetail.Rows[i].Cells["Amount"].Value = DBNull.Value;
        }
      }

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
      if (this.saleOrderPid != long.MinValue)
      {
        string commandText = "spCSDSaleOrder_Select";
        DBParameter[] inputParam = new DBParameter[1];
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.saleOrderPid);
        DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable(commandText, inputParam);
      
        panSave.Visible = false;
        panSavecancel.Visible = false;
        if (dtSource == null)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0007");
          this.CloseTab();
          return;
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
        ultOrderDate.Value = DBConvert.ParseDateTime(dtSource.Rows[0]["OrderDate"].ToString(), formatConvert);
        try
        {
          cmbType.SelectedValue = DBConvert.ParseInt(dtSource.Rows[0]["Type"].ToString());
        }
        catch { }
        txtRemark.Text = dtSource.Rows[0]["Remark"].ToString();
        txtRef.Text = dtSource.Rows[0]["REFNo"].ToString().Trim();
        if (DBConvert.ParseDateTime(dtSource.Rows[0]["PODate"].ToString(), formatConvert) != DateTime.MinValue)
          ultPODate.Value = DBConvert.ParseDateTime(dtSource.Rows[0]["PODate"].ToString(), formatConvert);        
        txtcontract.Text = dtSource.Rows[0]["ContractNo"].ToString();
        txtDeliveryRequirement.Text = dtSource.Rows[0]["DeliveryRequirement"].ToString();
        txtPackingRequirement.Text = dtSource.Rows[0]["PackingRequirement"].ToString();
        txtDocumentRequirement.Text = dtSource.Rows[0]["DocumentRequirement"].ToString();
        txtShipmentTerms.Text = dtSource.Rows[0]["ShipmentTerms"].ToString();
        txtPaymentTerms.Text = dtSource.Rows[0]["PaymentTerms"].ToString();
        cmbEnvironmentalStatus.SelectedValue = DBConvert.ParseInt(dtSource.Rows[0]["EnvironmentStatus"].ToString());
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
        this.confirmSO = (DBConvert.ParseInt(dtSource.Rows[0]["Status"].ToString()) >= 1);
        chkConfirm.Checked = this.confirmSO;
        if (this.confirmSO)
        {
          cmbCustomer.Enabled = false;
          chkDirect.Enabled = false;
          cmbDirectCus.Enabled = false;
          btnAddItem.Enabled = false;
          for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
          {
            ultDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
          }
        }
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
          txtRef.ReadOnly = true;
          ultPODate.ReadOnly = true;
          txtcontract.ReadOnly = true;
          //chkFix.Enabled = false;
          txtCustomerPoNo.ReadOnly = true;
          txtPackingRequirement.ReadOnly = true;
          txtDeliveryRequirement.ReadOnly = true;
          txtDocumentRequirement.ReadOnly = true;
          txtShipmentTerms.ReadOnly = true;
          txtPaymentTerms.ReadOnly = true;
          cmbEnvironmentalStatus.Enabled = false;
          ultOrderDate.Enabled = false;
          chkConfirm.Enabled = false;
        }
        // Tâm thêm nếu chưa ship thì bật fixprice
        if (DBConvert.ParseInt(dtSource.Rows[0]["ShipQty"].ToString()) > 0)
        {
          chkFix.Enabled = false;
        }
        else
        {
          chkFix.Enabled = true;
        }
        // End
      }
      else
      {
        txtSaleNo.Text = Shared.Utility.FunctionUtility.GetNewSaleOrderNo("SO");
      }
      this.LoadGrid();
      this.load = true;
    }

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
      ////Check ShipTogether > 2
      //if (this.CountCheck >= 2 || this.CountCheck == 0)
      //{  
      //  return true;
      //}
      //else
      //{
      //  message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "ShipTogether > 2");
      //  return false;
      //}
      ////End
      // Customer's Po No :
      string text = txtCustomerPoNo.Text.Trim();
      if (text.Length == 0)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Customer's Po No");
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
      DateTime obj = DBConvert.ParseDateTime(ultOrderDate.Value.ToString(), formatConvert);
      if (obj == DateTime.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Order Date");
        return false;
      }
      // Confirm
      if (chkConfirm.Checked && ultDetail.Rows.Count == 0)
      {
        message = Shared.Utility.FunctionUtility.GetMessage("WRN0001");
        return false;
      }
      //Check enquiry, item, revision is added in database
      DataTable dtDetail = (DataTable)ultDetail.DataSource;
      if (dtDetail.Rows.Count <= 0)
      {
        message = Shared.Utility.FunctionUtility.GetMessage("WRN0007");
        return false;
      }

      //(Thinh Update) Check CBM for Sale Order
      //if (chkConfirm.Checked)
      //{
      //  bool chkresult = this.CheckCBM();
      //  if (chkresult)
      //  {
      //    message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Check CBM =0");
      //    return false;
      //  }
      //}

      string commandText = string.Empty;
      for (int i = 0; i < dtDetail.Rows.Count; i++)
      {
        if (chkConfirm.Checked == true)
        {
          //Tien add
          if (DBConvert.ParseInt(dtDetail.Rows[i]["Revision"].ToString()) > 0 && DBConvert.ParseDouble(dtDetail.Rows[i]["Price"].ToString()) <= 0 && DBConvert.ParseInt(dtDetail.Rows[i]["FOC"].ToString()) == 0)
          {
            message = "Please input FOB price or check in FOC";
            return false;
          }
        }
        if (dtDetail.Rows[i].RowState == DataRowState.Added)
        {
          long pidENConfirm = DBConvert.ParseLong(dtDetail.Rows[i]["EnquiryConfirmDetailPid"].ToString());
          PLNSaleOrderDetail objDetail = new PLNSaleOrderDetail();
          objDetail.EnquiryConfirmDetailPid = pidENConfirm;
          objDetail = (PLNSaleOrderDetail)Shared.DataBaseUtility.DataBaseAccess.LoadObject(objDetail, new string[] { "EnquiryConfirmDetailPid" });
          if (objDetail != null)
          {
            string enNo = dtDetail.Rows[i]["EnquiryNo"].ToString();
            string itemCode = dtDetail.Rows[i]["ItemCode"].ToString();
            string revision = dtDetail.Rows[i]["Revision"].ToString();
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0028"), string.Format("Enquiry: {0}, Item: {1}, Revision: {2}", enNo, itemCode, revision));
            return false;
          }
        }

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

      DateTime orderDate = DBConvert.ParseDateTime(ultOrderDate.Value.ToString(), formatConvert);
      if (orderDate != DateTime.MinValue)
      {
        saleOrder.OrderDate = orderDate;
      }

      int type = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      saleOrder.Type = type;

      saleOrder.DeleteFlag = 0;

      int confirm = (chkConfirm.Checked ? 1 : 0);
      if (saleOrder.Status <= 1)
      {
        saleOrder.Status = confirm;
      }

      if (confirm == 1 && saleOrder.Status < 2)
      {
        saleOrder.CSConfirmDate = DateTime.Now;
        saleOrder.CSConfirmBy = Shared.Utility.SharedObject.UserInfo.UserPid;
        saleOrder.ACCConfirmDate = DateTime.Now;
        saleOrder.ACCConfirmBy = Shared.Utility.SharedObject.UserInfo.UserPid;
        saleOrder.Status = 2;
      }
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
      string ContractNo = txtcontract.Text.Trim();
      string DeliveryRequirement = txtDeliveryRequirement.Text.Trim();
      string PackingRequirement = txtPackingRequirement.Text.Trim();
      string DocumentRequirement = txtDocumentRequirement.Text.Trim();
      saleOrder.REFNo = REFNo;
      saleOrder.PODate = PODate;
      saleOrder.ContractNo = ContractNo;
      saleOrder.DeliveryRequirement = DeliveryRequirement;
      saleOrder.PackingRequirement = PackingRequirement;
      saleOrder.DocumentRequirement = DocumentRequirement;
      saleOrder.ShipmentTerms = txtShipmentTerms.Text.Trim();
      saleOrder.PaymentTerms = txtPaymentTerms.Text.Trim();
      int environment = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbEnvironmentalStatus));
      saleOrder.EnvironmentStatus = environment;
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
        if (chkConfirm.Checked)
        {
          saleOrder.CSConfirmBy = Shared.Utility.SharedObject.UserInfo.UserPid;
          saleOrder.CSConfirmDate = DateTime.Now;
        }
        saleOrder.UpdateDate = DateTime.Now;
        success = Shared.DataBaseUtility.DataBaseAccess.UpdateObject(saleOrder, new string[] { "Pid" });
      }
      else //Insert
      {
        saleOrder.SaleNo = Shared.Utility.FunctionUtility.GetNewSaleOrderNo("SO");
        saleOrder.CreateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
        saleOrder.CreateDate = DateTime.Now;
        if (chkConfirm.Checked)
        {
          saleOrder.CSConfirmBy = Shared.Utility.SharedObject.UserInfo.UserPid;
          saleOrder.CSConfirmDate = DateTime.Now;
        }
        this.saleOrderPid = Shared.DataBaseUtility.DataBaseAccess.InsertObject(saleOrder);
        if (this.saleOrderPid <= 0)
        {
          success = false;
        }
      }
      return success;
    }

    private bool CheckCBM()
    {
      bool chkresult = false;
      string strparam = "";
      if (ultDetail.Rows.Count > 0)
      {
        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          strparam += "&" + ultDetail.Rows[i].Cells["ItemCode"].Value.ToString() + "-" + ultDetail.Rows[i].Cells["Revision"].Value.ToString();
        }
        // Check CBM
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@StringParam", DbType.String, strparam);
        DBParameter[] output = new DBParameter[1];
        output[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
        DataTable dtNonCBM = DataBaseAccess.SearchStoreProcedureDataTable("spPLNCheckNonCBMForEnquiry", input, output);
        int result = DBConvert.ParseInt(output[0].Value.ToString());
        if (result == 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Check CBM");
          return false;
        }
        try
        {
          if (dtNonCBM.Rows.Count > 0)
          {
            for (int i = 0; i < ultDetail.Rows.Count; i++)
            {
              ultDetail.Rows[i].CellAppearance.BackColor = Color.White;
              for (int j = 0; j < dtNonCBM.Rows.Count; j++)
              {
                if (ultDetail.Rows[i].Cells["ItemCode"].Value.ToString() + "-" + ultDetail.Rows[i].Cells["Revision"].Value.ToString() == dtNonCBM.Rows[j][0].ToString())
                {
                  ultDetail.Rows[i].CellAppearance.BackColor = Color.Pink;
                  chkresult = true;
                }
              }
            }
          }
        }
        catch { }
      }
      return chkresult;
    }

    private bool SaveDetail()
    {
      bool success = true;
      //Delete
      foreach (long pid in this.listDeletedPid)
      {
        PLNSaleOrderDetail saleOrderDT = new PLNSaleOrderDetail();
        saleOrderDT.Pid = pid;
        saleOrderDT = (PLNSaleOrderDetail)Shared.DataBaseUtility.DataBaseAccess.LoadObject(saleOrderDT, new string[] { "Pid" });
        long pidENConfirm = saleOrderDT.EnquiryConfirmDetailPid;
        bool isDeleted = Shared.DataBaseUtility.DataBaseAccess.DeleteObject(saleOrderDT, new string[] { "Pid" });
        if (!isDeleted)
        {
          success = false;
        }
        else //Delete success
        {
          PLNEnquiryConfirmDetail enConfirm = new PLNEnquiryConfirmDetail();
          enConfirm.Pid = pidENConfirm;
          enConfirm = (PLNEnquiryConfirmDetail)Shared.DataBaseUtility.DataBaseAccess.LoadObject(enConfirm, new string[] { "Pid" });
          enConfirm.LoadedFlg = 0;
          Shared.DataBaseUtility.DataBaseAccess.UpdateObject(enConfirm, new string[] { "Pid" });
        }
      }

      //Update
      DataTable dtDetail = (DataTable)ultDetail.DataSource;
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Modified)
        {
          PLNSaleOrderDetail saleOrderDT = new PLNSaleOrderDetail();
          saleOrderDT.Pid = DBConvert.ParseLong(row["Pid"].ToString());
          saleOrderDT = (PLNSaleOrderDetail)Shared.DataBaseUtility.DataBaseAccess.LoadObject(saleOrderDT, new string[] { "Pid" });
          saleOrderDT.Price = DBConvert.ParseDouble(row["Price"].ToString());
          saleOrderDT.SecondPrice = DBConvert.ParseDouble(row["SecondPrice"].ToString());
          saleOrderDT.Remark = row["Remark"].ToString();
          saleOrderDT.UrgentNote = row["UrgentNote"].ToString();
          saleOrderDT.RemarkForAccount = row["RemarkForAccount"].ToString();
          saleOrderDT.SpecialDescription = row["SpecialDescription"].ToString();
          saleOrderDT.SpecialInstruction = row["SpecialInstruction"].ToString();
          //Tien Add
          saleOrderDT.Foc = DBConvert.ParseInt(row["FOC"].ToString());
          //Tuan Add
          //saleOrderDT.ShipTogether = DBConvert.ParseInt(row["ShipTogether"].ToString());
          //Cuong Add
          saleOrderDT.IReturn = DBConvert.ParseInt(row["Return"].ToString());
          //Truong Add(confirm Price)
          if (DBConvert.ParseInt(row["ConfirmPrice"].ToString()) != 1)
          {
            if (DBConvert.ParseInt(row["Select"].ToString()) == 1)
            {
              saleOrderDT.ConfirmPrice = 1;
            }
            else
            {
              saleOrderDT.ConfirmPrice = 0;
            }
          }
          //End
          if (!Shared.DataBaseUtility.DataBaseAccess.UpdateObject(saleOrderDT, new string[] { "Pid" }))
          {
            success = false;
          }
        }
        else if (row.RowState == DataRowState.Added)
        {
          PLNSaleOrderDetail saleOrderDetail = new PLNSaleOrderDetail();
          saleOrderDetail.SaleOrderPid = this.saleOrderPid;
          saleOrderDetail.EnquiryConfirmDetailPid = DBConvert.ParseLong(row["EnquiryConfirmDetailPid"].ToString());
          saleOrderDetail.ItemCode = row["ItemCode"].ToString();
          saleOrderDetail.Revision = DBConvert.ParseInt(row["Revision"].ToString());
          saleOrderDetail.Qty = DBConvert.ParseInt(row["Qty"].ToString());
          DateTime scheduleDelivery = DateTime.MinValue;
          try
          {
            scheduleDelivery = DBConvert.ParseDateTime(row["ScheduleDate"].ToString(), formatConvert);
          }
          catch { }
          saleOrderDetail.ScheduleDelivery = scheduleDelivery;
          saleOrderDetail.Price = DBConvert.ParseDouble(row["Price"].ToString());
          saleOrderDetail.SecondPrice = DBConvert.ParseDouble(row["SecondPrice"].ToString());

          saleOrderDetail.Remark = row["Remark"].ToString();
          saleOrderDetail.UrgentNote = row["UrgentNote"].ToString();
          saleOrderDetail.SpecialDescription = row["SpecialDescription"].ToString();
          saleOrderDetail.SpecialInstruction = row["SpecialInstruction"].ToString();
          saleOrderDetail.CreateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
          saleOrderDetail.CreateDate = DateTime.Now;
          saleOrderDetail.Foc = DBConvert.ParseInt(row["FOC"].ToString());
          saleOrderDetail.IReturn = DBConvert.ParseInt(row["Return"].ToString());
          //Truong Add(confirm Price & Fix)
          if (DBConvert.ParseInt(row["ConfirmPrice"].ToString()) != 1)
          {
            if (DBConvert.ParseInt(row["Select"].ToString()) == 1)
            {
              saleOrderDetail.ConfirmPrice = 1;
            }
            else
            {
              saleOrderDetail.ConfirmPrice = 0;
            }
          }
          //End
          //Tuan Add (ShipTogether)
          //if (DBConvert.ParseInt(row["ShipTogether"].ToString()) == 1)
          //{
          //  saleOrderDetail.ShipTogether = 1;
          //}
          //else
          //{
          //  saleOrderDetail.ShipTogether = 0;
          //}
          //End
          long d = Shared.DataBaseUtility.DataBaseAccess.InsertObject(saleOrderDetail);
          if (d <= 0)
          {
            success = false;
          }
          else
          {
            PLNEnquiryConfirmDetail ENConfirm = new PLNEnquiryConfirmDetail();
            ENConfirm.Pid = DBConvert.ParseLong(row["EnquiryConfirmDetailPid"].ToString());
            ENConfirm = (PLNEnquiryConfirmDetail)Shared.DataBaseUtility.DataBaseAccess.LoadObject(ENConfirm, new string[] { "Pid" });
            ENConfirm.LoadedFlg = 1;
            if (!Shared.DataBaseUtility.DataBaseAccess.UpdateObject(ENConfirm, new string[] { "Pid" }))
            {
              Shared.DataBaseUtility.DataBaseAccess.DeleteObject(saleOrderDetail, new string[] { "Pid" });
              success = false;
            }
          }
        }
      }
      return success;
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
        //Unlock        
        if (chkConfirm.Checked)
        {
          string commandUpdate = string.Format("Update TblPLNSaleOrder Set Status = 0 Where Pid = {0}", this.saleOrderPid);
          Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandUpdate);
        }
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        return;
      }
      else
      {
        success = this.SaveDetail();
        if (!success)
        {
          //Unlock        
          if (chkConfirm.Checked)
          {
            string commandUpdate = string.Format("Update TblPLNSaleOrder Set Status = 0 Where Pid = {0}", this.saleOrderPid);
            Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandUpdate);
          }
          Shared.Utility.WindowUtinity.ShowMessageError("WRN0004");
          return;
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");



      // Email
      if (statusSO == 1)
      {
        Email email = new Email();
        email.Key = email.KEY_CSD_002;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), this.txtSaleNo.Text, userName);
          string body = string.Format(arrList[2].ToString(), this.txtSaleNo.Text, userName);
          email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }
      }
      this.SaveCancel();
      this.LoadData();
      this.NeedToSave = false;
    }

    private long SaveCancel()
    {
      DBParameter[] input = new DBParameter[3];
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      input[0] = new DBParameter("@Pid", DbType.Int64, this.saleOrderPid);
      if (chkCancel.Checked == true)
      {
        input[1] = new DBParameter("@Cancel", DbType.Int32, 1);
      }
      else
      {
        input[1] = new DBParameter("@Cancel", DbType.Int32, 0);
      }
      input[2] = new DBParameter("@CanceBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrder_UpdateCancel", input, output);
      return DBConvert.ParseLong(output[0].Value.ToString());
    }

    #endregion CheckValid And Save

    #region Events
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
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
        if (string.Compare(columnName, "Select", true) == 0
              || string.Compare(columnName, "Remark", true) == 0
              || string.Compare(columnName, "RemarkForAccount", true) == 0
              || string.Compare(columnName, "UrgentNote", true) == 0)
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


      e.Layout.Bands[0].Columns["FOC"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[0].Columns["FOC"].CellAppearance.BackColor = Color.Aqua;
      //Cuong Add
      //e.Layout.Bands[0].Columns["Return"].CellActivation = Activation.AllowEdit;
      //------------
      // Truong Add
      for (int j = 0; j < ultDetail.Rows.Count; j++)
      {
        UltraGridRow row = ultDetail.Rows[j];
        if (DBConvert.ParseInt(row.Cells["ConfirmPrice"].Value.ToString()) == 1 || this.confirmSO == true)
        {
          //row.Cells["Price"].Column.CellActivation = Activation.ActivateOnly;
          //row.Cells["Price"].Column.CellAppearance.BackColor = Color.White;
          //Tien Add
          row.Cells["FOC"].Column.CellActivation = Activation.ActivateOnly;
          row.Cells["FOC"].Column.CellAppearance.BackColor = Color.White;
        }
        else
        {
          //row.Cells["Price"].Column.CellActivation = Activation.AllowEdit;
          //row.Cells["Price"].Column.CellAppearance.BackColor = Color.Aqua;
          row.Cells["FOC"].Column.CellActivation = Activation.AllowEdit;
          row.Cells["FOC"].Column.CellAppearance.BackColor = Color.Aqua;

        }
    
        //Tâm thêm luồng nếu chưa ship thì được sửa giá CustomerPrice
        //Tien Edit lai kiem tra tren tung item
        e.Layout.Bands[0].Columns["SecondPrice"].CellActivation = Activation.AllowEdit;


        if (DBConvert.ParseInt(row.Cells["SQty"].Value.ToString()) > 0)
        {
          row.Cells["SecondPrice"].Activation = Activation.ActivateOnly;
          row.Cells["SecondPrice"].Appearance.BackColor = Color.White;
        }
        else
        {
          row.Cells["SecondPrice"].Activation = Activation.AllowEdit;
          row.Cells["SecondPrice"].Appearance.BackColor = Color.Aqua;
        }
      }
      //End

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Return"].Header.Caption = "Customer Return\n NG Item Fixing";
      e.Layout.Bands[0].Columns["No"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["EnquiryNo"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Po Number"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemKind"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Revision"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["EnquiryNo"].Header.Caption = "Enquiry No";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Item Name";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ScheduleDate"].Header.Caption = "Confirmed Ship Date";
      e.Layout.Bands[0].Columns["SpecialInstruction"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SpecialInstruction"].Header.Caption = "Special Instruction";
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Request Ship Date";
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Select"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemKind"].Hidden = true;
      e.Layout.Bands[0].Columns["TypeItem"].Hidden = true;
      e.Layout.Bands[0].Columns["Odd Box"].Hidden = true;
      e.Layout.Bands[0].Columns["RealDelivery"].Hidden = true;
      e.Layout.Bands[0].Columns["ShipQty"].Hidden = true;
      e.Layout.Bands[0].Columns["EnquiryConfirmDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsMod"].Hidden = true;
      e.Layout.Bands[0].Columns["PackageQty"].Hidden = true;
      e.Layout.Bands[0].Columns["ConfirmPrice"].Hidden = true;
      e.Layout.Bands[0].Columns["Return"].Hidden = true;
      e.Layout.Bands[0].Columns["UrgentNote"].Hidden = true;
      e.Layout.Bands[0].Columns["RemarkForAccount"].Hidden = true;
      e.Layout.Bands[0].Columns["RequestDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["RequestDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["RequestDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ScheduleDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ScheduleDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemKind"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["SQty"].Hidden = true;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,###}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["SecondAmount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[3].DisplayFormat = "{0:###,###.##}";

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

      //Tien Add
      e.Layout.Bands[0].Columns["FOC"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["FOC"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["FOC"].MinWidth = 50;

      // Cuong Add
      e.Layout.Bands[0].Columns["Return"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;


      //e.Layout.Bands[0].Columns["ShipTogether"].CellActivation = Activation.AllowEdit;
      //e.Layout.Bands[0].Columns["ShipTogether"].CellAppearance.BackColor = Color.Aqua;
      //e.Layout.Bands[0].Columns["ShipTogether"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      //e.Layout.Bands[0].Columns["ShipTogether"].MaxWidth = 50;
      //e.Layout.Bands[0].Columns["ShipTogether"].MinWidth = 50;

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
            e.Cell.Row.Cells["Amount"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
            if (DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString()) > 0)
            {
              e.Cell.Row.Cells["FOC"].Value = 0;
            }
          }
          catch
          {
            e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
          }
          break;
        case "qty":
          try
          {
            e.Cell.Row.Cells["Amount"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
          }
          catch
          {
            e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
          }
          break;
        case "select":
          try
          {
            if (DBConvert.ParseInt(e.Cell.Row.Cells["Select"].Value.ToString()) == 1)
            {
              strSaleOrderPid += e.Cell.Row.Cells["Pid"].Value.ToString() + ",";
            }
            else
            {
              strSaleOrderPid = strSaleOrderPid.Replace("" + e.Cell.Row.Cells["Pid"].Value.ToString() + "," + "", "");
            }
          }
          catch
          {

          }
          break;
        //case "shiptogether":
        //  if (DBConvert.ParseInt(e.Cell.Row.Cells["ShipTogether"].Value.ToString()) == 1)
        //  {
        //    this.CountCheck++;
        //  }
        //  else
        //  {
        //    this.CountCheck--;
        //  }
        //  break;
        default:
          break;
      }
    }

    private void ultDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.NeedToSave = true;
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
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

    private void ultDetail_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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

    private void cmbEnPid_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.load == true)
      {
        int pId = int.MinValue;

        string commandText = string.Format("Select ISNULL(ItemCodeConfirm, ItemCode) ItemCode, ISNULL(RevisionConfirm, Revision) Revision, ISNULL(ScheduleDeliveryConfirm, ScheduleDelivery) ScheduleDelivery, SUM(ISNULL(QtyConfirm, Qty)) Qty From TblPLNEnquiryDetail Where EnquiryPid = {0} And Expire = 0 Group By ISNULL(ItemCodeConfirm, ItemCode), ISNULL(RevisionConfirm, Revision), ISNULL(ScheduleDeliveryConfirm, ScheduleDelivery)Order By ISNULL(ItemCodeConfirm, ItemCode), ISNULL(RevisionConfirm, Revision), ISNULL(ScheduleDeliveryConfirm, ScheduleDelivery)", pId);
        DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);

        PLNEnquiry enquiry = new PLNEnquiry();
        enquiry.Pid = pId;
        enquiry = (PLNEnquiry)Shared.DataBaseUtility.DataBaseAccess.LoadObject(enquiry, new string[] { "Pid" });
        if (enquiry == null)
        {
          cmbType.Enabled = true;
          cmbType.SelectedIndex = -1;
          cmbCustomer.Enabled = true;
          cmbCustomer.SelectedIndex = -1;
          txtRemark.Enabled = true;
          txtRemark.Text = string.Empty;
        }
        else
        {
          try
          {
            cmbCustomer.SelectedValue = enquiry.CustomerPid;
          }
          catch
          {
          }
          cmbCustomer.Enabled = false;
          try
          {
            cmbType.SelectedValue = enquiry.Type;
          }
          catch
          { }
          cmbType.Enabled = false;
          txtRemark.Text = enquiry.Remark.Trim();
        }

        dtSource.Columns.Add("RealDelivery", typeof(DateTime));
        dtSource.Columns.Add("Price", typeof(double));
        dtSource.Columns.Add("Remark", typeof(string));
        dtSource.Columns.Add("SpecialDescription", typeof(string));
        dtSource.Columns.Add("Pid", typeof(Int64));
        ultDetail.DataSource = dtSource;
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          dtSource.Rows[i].SetAdded();
        }

        string value = Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType);
        if (value == "2")
        {
          ultDetail.DisplayLayout.Bands[0].Columns["SpecialDescription"].Hidden = false;
        }
        else
        {
          ultDetail.DisplayLayout.Bands[0].Columns["SpecialDescription"].Hidden = true;
        }
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

    private void btnAddItem_Click(object sender, EventArgs e)
    {
      #region Check
      // Customer :
      long customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      if (customerPid == long.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Customer");
        return;
      }

      //Direct Customer :
      long directCus = long.MinValue;
      if (chkDirect.Checked)
      {
        directCus = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbDirectCus));
        if (directCus == long.MinValue)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Direct Customer");
          return;
        }
      }

      // Type :
      int type = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType));
      if (type == int.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Type");
        return;
      }
      // Environment :
      int environment = DBConvert.ParseInt(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbEnvironmentalStatus));
      if (environment == int.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Environment");
        return;
      }
      #endregion Check

      DataTable dtSource = (DataTable)ultDetail.DataSource;
      viewCSD_05_007 view = new viewCSD_05_007();
      view.customerPid = customerPid;
      view.customerDirect = directCus;
      view.type = type;
      view.dtExistingSource = dtSource;
      DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "ADD ITEM TO SALE ORDER", false, DaiCo.Shared.Utility.ViewState.ModalWindow);

      foreach (DataRow row in view.dtNewSource.Rows)
      {
        DataRow newRow = dtSource.NewRow();
        newRow["EnquiryNo"] = row["EnquiryNo"];
        newRow["ItemCode"] = row["ItemCode"];
        newRow["Name"] = row["Name"];
        newRow["Revision"] = row["Revision"];
        newRow["Qty"] = row["Qty"];
        newRow["ScheduleDate"] = row["ScheduleDelivery"];
        newRow["RequestDate"] = row["RequestDate"];
        newRow["Return"] = row["Return"];
        newRow["Remark"] = row["Remark"];
        newRow["RemarkForAccount"] = row["RemarkForAccount"];
        newRow["PO Number"] = row["CustomerEnquiryNo"];
        newRow["SpecialInstruction"] = row["SpecialInstruction"];
        newRow["EnquiryConfirmDetailPid"] = row["EnquiryConfirmDetailPid"];
        newRow["SaleCode"] = row["SaleCode"];
        newRow["CBM"] = row["CBM"];
        newRow["Unit"] = row["Unit"];
        newRow["TotalCBM"] = row["TotalCBM"];
        newRow["Price"] = row["Price"];
        newRow["FOC"] = 0;
        newRow["SecondPrice"] = row["SecondPrice"];
        newRow["Amount"] = DBConvert.ParseInt(newRow["Qty"].ToString()) * DBConvert.ParseDouble(newRow["Price"].ToString());
        newRow["SecondAmount"] = DBConvert.ParseInt(newRow["Qty"].ToString()) * DBConvert.ParseDouble(newRow["SecondPrice"].ToString());

        dtSource.Rows.Add(newRow);
      }
      ultDetail.DataSource = dtSource;
      if (txtDeliveryRequirement.Text.Trim().Length == 0)
      {
        txtDeliveryRequirement.Text = view.deliveryrequirement.ToString();
      }
      if (txtPackingRequirement.Text.Trim().Length == 0)
      {
        txtPackingRequirement.Text = view.packingrequirement.ToString();
      }
      if (txtDocumentRequirement.Text.Trim().Length == 0)
      {
        txtDocumentRequirement.Text = view.documentrequirement.ToString();
      }
      if (txtShipmentTerms.Text.Trim().Length == 0)
      {
        txtShipmentTerms.Text = view.shipmentTerms;
      }
      if (txtPaymentTerms.Text.Trim().Length == 0)
      {
        txtPaymentTerms.Text = view.paymentTerms;
      }
      view.environmentStatus = environment;
      //Customet, type
      if (dtSource.Rows.Count > 0)
      {
        this.EnableCustomer(false);
      }
      else
      {
        this.EnableCustomer(true);
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
        // End
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

    private void btnPrint_Click(object sender, EventArgs e)
    {

      DataTable dt = (DataTable)ultDetail.DataSource;
      string strTemplateName = "CSDSaleOrderDetailTemplate";
      string strSheetName = "SaleOrder";
      string strOutFileName = "SaleOrder";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      oXlsReport.Cell("**CusName").Value = cmbCustomer.Text;
      oXlsReport.Cell("**CusNo").Value = txtCustomerPoNo.Text;
      oXlsReport.Cell("**Ref").Value = txtRef.Text;
      oXlsReport.Cell("**Podate").Value = ultPODate.Value;
      oXlsReport.Cell("**ReShipdate").Value = "";
      oXlsReport.Cell("**Contract").Value = txtcontract.Text;
      oXlsReport.Cell("**SaleorderNo").Value = txtSaleNo.Text;
      oXlsReport.Cell("**SaleorderDate").Value = ultOrderDate.Value;
      oXlsReport.Cell("**SaleorderType").Value = cmbType.Text;
      oXlsReport.Cell("**ConfShipdate").Value = "";
      oXlsReport.Cell("**Remark").Value = txtRemark.Text;
      oXlsReport.Cell("**Delivery").Value = txtDeliveryRequirement.Text;
      oXlsReport.Cell("**Packing").Value = txtPackingRequirement.Text;
      oXlsReport.Cell("**Document").Value = txtDocumentRequirement.Text;
      if (chkDirect.Checked)
      {
        oXlsReport.Cell("**DriectCus").Value = cmbDirectCus.Text;
      }
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dtRow = dt.Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("A11:Q11").Copy();
          oXlsReport.RowInsert(10 + i);
          oXlsReport.Cell("A11:Q11", 0, i).Paste();
        }
        for (int c = 0; c < dt.Columns.Count; c++)
        {
          oXlsReport.Cell("**1", 0, i).Value = i + 1;
          oXlsReport.Cell("**2", 0, i).Value = dtRow["EnquiryNo"];
          oXlsReport.Cell("**3", 0, i).Value = dtRow["SaleCode"];
          oXlsReport.Cell("**4", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**5", 0, i).Value = dtRow["Revision"];
          oXlsReport.Cell("**6", 0, i).Value = dtRow["Name"];
          oXlsReport.Cell("**7", 0, i).Value = dtRow["Qty"];
          oXlsReport.Cell("**8", 0, i).Value = dtRow["Unit"];
          oXlsReport.Cell("**9", 0, i).Value = dtRow["CBM"];
          oXlsReport.Cell("**10", 0, i).Value = dtRow["TotalCBM"];
          oXlsReport.Cell("**11", 0, i).Value = dtRow["Price"];
          oXlsReport.Cell("**12", 0, i).Value = dtRow["Amount"];
          oXlsReport.Cell("**13", 0, i).Value = dtRow["SecondPrice"];
          oXlsReport.Cell("**14", 0, i).Value = dtRow["SecondAmount"];
          oXlsReport.Cell("**15", 0, i).Value = dtRow["ScheduleDate"];
          oXlsReport.Cell("**16", 0, i).Value = dtRow["SpecialInstruction"];
          oXlsReport.Cell("**17", 0, i).Value = dtRow["Remark"];
        }
      }

      int cnt = dt.Rows.Count + 10;
      oXlsReport.Cell("**SumQty").Value = "=SUM(G11:G" + cnt.ToString() + ")";
      oXlsReport.Cell("**SumTotalCBM").Value = "=SUM(J11:J" + cnt.ToString() + ")";
      oXlsReport.Cell("**SumAmount").Value = "=SUM(L11:L" + cnt.ToString() + ")";
      oXlsReport.Cell("**SumSecondAmount").Value = "=SUM(N11:N" + cnt.ToString() + ")";
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    private void ultDetail_AfterSortChange(object sender, BandEventArgs e)
    {
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        ultDetail.Rows[i].Cells["No"].Value = i + 1;
      }
    }

    private string GetRandomString()
    {
      StringBuilder sBuilder = new StringBuilder();
      Random random = new Random();
      char ch;
      for (int i = 0; i < 10; i++)
      {
        ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
        sBuilder.Append(ch);
      }
      return sBuilder.ToString() + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + '_' + DateTime.Now.Ticks.ToString();
    }

    private void btnPrintPDF_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SaleorderPid", DbType.Int64, this.saleOrderPid) };
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCSDSaleOrderDetail", inputParam);
      dsSource.Tables[1].Columns.Add("Picture", typeof(System.Byte[]));
      dsSource.Tables[1].Columns.Add("ItemKind", typeof(System.Byte[]));
      for (int i = 0; i < dsSource.Tables[1].Rows.Count; i++)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemImage(dsSource.Tables[1].Rows[i]["ItemCode"].ToString(), DBConvert.ParseInt(dsSource.Tables[1].Rows[i]["Revision"].ToString()));
          dsSource.Tables[1].Rows[i]["Picture"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 3.24, "JPG");
          switch (DBConvert.ParseInt(dsSource.Tables[1].Rows[i]["StatusIcon"].ToString()))
          {
            case 0:
              dsSource.Tables[1].Rows[i]["ItemKind"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.UK_US);
              break;
            case 1:
              dsSource.Tables[1].Rows[i]["ItemKind"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.DisCont);
              break;
            case 2:
              dsSource.Tables[1].Rows[i]["ItemKind"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.SpecialOrder);
              break;
            case 3:
              dsSource.Tables[1].Rows[i]["ItemKind"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.USQS);
              break;
            case 4:
              dsSource.Tables[1].Rows[i]["ItemKind"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.UKQS);
              break;
            case 5:
              dsSource.Tables[1].Rows[i]["ItemKind"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.Standard);
              break;
          }
        }
        catch { }
      }
      dsCUSSaleorder dsCUSSaleorder = new dsCUSSaleorder();
      dsCUSSaleorder.Tables["SaleorderInfomation"].Merge(dsSource.Tables[0]);
      dsCUSSaleorder.Tables["SaleorderDetail"].Merge(dsSource.Tables[1]);

      //DaiCo.Shared.View_Report report = null;
      DaiCo.CustomerService.ReportTemplate.cptCUSSaleorder cpt = new DaiCo.CustomerService.ReportTemplate.cptCUSSaleorder();
      cpt.SetDataSource(dsCUSSaleorder);

      ControlUtility.ViewCrystalReport(cpt);

      //report = new DaiCo.Shared.View_Report(cpt);
      //report.IsShowGroupTree = false;
      //string random = GetRandomString();
      //string startupPath = System.Windows.Forms.Application.StartupPath;
      //string filePath = string.Format(@"{0}\Reports\CustomerService{1}_{2}.pdf", startupPath, saleOrderPid, random);
      //cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);



      //string filePath = string.Format(@"{0}\Reports\CustomerService{1}_{2}.xls", startupPath, saleOrderPid, random);
      //cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, filePath);

      // @"+ startupPath + \Reports\CustomerService" + saleOrderPid + "_" + random + ".pdf");
      //report.ShowReport(DaiCo.Shared.Utility.ViewState.ModalWindow);
      //Process.Start(filePath);
    }

    private void btnSaveCancel_Click(object sender, EventArgs e)
    {
      long SaveCancel = this.SaveCancel();
      if (SaveCancel == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0002");
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0001");
      }

    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        ultDetail.Rows[i].Cells["Select"].Value = chkSelectAll.Checked;
      }
    }

    private void txtRevision_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
      {
        e.Handled = true;
      }
    }

    private void btnUpdateRevision_Click(object sender, EventArgs e)
    {
      if (strSaleOrderPid.Length > 1)
      {
        DBParameter[] input = new DBParameter[2];
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        input[0] = new DBParameter("@Revision", DbType.Int16, DBConvert.ParseInt(txtRevision.Text.Trim()));
        input[1] = new DBParameter("@SaleOrderDetailPid", DbType.AnsiString, 1000, strSaleOrderPid);
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spCSDSalOrderUpdateRevision", input, output);
        int result = DBConvert.ParseInt(output[0].Value.ToString());
        if (result == 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0002");
        }
        if (result == -1)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0133");
        }
        if (result == -2)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0134");
        }
        if (result == 1)
        {
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0001");
        }
        strSaleOrderPid = ",";
        this.LoadData();
      }
    }

    private void ultDetail_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultDetail.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultDetail.Selected.Rows[0];
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      double FODPrice = DBConvert.ParseDouble(row.Cells["Price"].Value.ToString());
      int FOC = DBConvert.ParseInt(row.Cells["FOC"].Value.ToString());
      if (chkUpdateRevision.Checked && (FODPrice > 0 || FOC > 0))
      {
        viewCSD_05_013 uc = new viewCSD_05_013();
        uc.pid = pid;
        WindowUtinity.ShowView(uc, "Upgrade Revision", false, ViewState.ModalWindow);
        // Load Data
        this.LoadData();
        this.NeedToSave = false;
      }
    }

    #endregion Events
  }
}
