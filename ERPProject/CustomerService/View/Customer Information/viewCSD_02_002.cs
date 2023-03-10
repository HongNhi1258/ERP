/*
  Author        : Lâm Quang Hà
  Create date   : 15/10/2010
  Decription    : Edit customer infomation
  Checked by    : Võ Hoa Lư
  Checked date  : 25/10/2010
  Updater       : Ha Anh 24-09-2011 Sửa lại cấu trúc customer Consignee  
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
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System.Collections;
using DaiCo.Shared.UserControls;
using DaiCo.ERPProject.CustomerService.DataSetSource;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_02_002 : MainUserControl
  {
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    public long customerPid = long.MinValue;
    private int currentTabIndex = 0;
    private bool isLoaddingData = true;
    private bool isChangeOtherInfo = false;
    private IList listDeletingOtherContactPersonPid = new ArrayList();
    private IList listDeletedOtherContactPersonPid = new ArrayList();
    private int maxEnquiryExpireDays = int.MinValue;
    
    #region Init Form
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_02_002()
    {
      InitializeComponent();      
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    private void viewCSD_02_002_Load(object sender, EventArgs e)
    {
      this.isLoaddingData = true;

      // Load max enquiry expire days      
      string commandText = string.Format("Select Value From TblBOMCodeMaster Where [Group] = {0}", ConstantClass.GROUP_ENQUIRY_MAX_EXPIRE_DAYS);
      DataTable dtExpireDays = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtExpireDays != null && dtExpireDays.Rows.Count > 0)
      {
        this.maxEnquiryExpireDays = DBConvert.ParseInt(dtExpireDays.Rows[0]["Value"].ToString());
      }

      this.LoadDropDownList();
      this.currentTabIndex = 0;
      this.LoadData();
      this.isLoaddingData = false;
    }
    #endregion Init Form

    #region Load Data

    private void LoadCBRegion()
    {
      string cmText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 99004");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmText);
      Utility.LoadUltraCombo(ultCBRegion, dt, "Code", "Value", false, "Code");
    }
    /// <summary>
    /// Load kind of customer
    /// </summary>
    private void LoadCustomerKind() {
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = {0} AND Code IN (4, 5, 6)", ConstantClass.GROUP_CUSTOMER_KIND);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadCombobox(cmbKind, dt, "Code", "Value");
    }

    /// <summary>
    /// Load dropdownlist ResponsiblePerson
    /// </summary>
      private void LoadResponsiblePerson()
      {
          string commandText = "SELECT Pid, (CAST(Pid as varchar) + ' - ' + EmpName)EmpName FROM VCSDResponsibleEmployee ORDER BY Pid";
          DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
          Utility.LoadCombobox(cmbResponsiblePerson, dt, "Pid", "EmpName");
      }

    /// <summary>
    /// Load dropdownlist Distribute
    /// </summary>
    private void LoadDistribute()
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer 
                                           FROM TblCSDCustomerInfo 
                                           WHERE ParentPid IS NULL AND Kind = 4 AND DeletedFlg = 0 AND Pid <> {0}", this.customerPid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadCombobox(cmbDistribute, dt, "Pid", "Customer");      
    }

    /// <summary>
    /// Load Forwarder Name
    /// </summary>
    private void LoadForwarder()
    {
      string commandText = "SELECT Pid, ForwarderCode + ' - ' + Name Forwarder FROM TblCSDForwarder WHERE ISNULL(DeleteFlag, 0) = 0 ORDER BY ForwarderCode DESC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadCombobox(cmbForwarder, dt, "Pid", "Forwarder");      
    }

    /// <summary>
    /// Load Sale throught
    /// </summary>
    private void LoadSaleThrought() {
      string commandText = "SELECT Pid, Name FROM VCSDSaleThrought";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadCombobox(cmbSaleThrought, dt, "Pid", "Name");
    }
    /// <summary>
    /// Load Res Sale
    /// </summary>
    private void LoadResSale()
    {
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 32 ");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadCombobox(cmbResSale, dt, "Code", "Value");
    }
    /// <summary>
    /// Load all dropdownlist in screen
    /// </summary>
    private void LoadDropDownList()
    {
      //1.Load dropdownlist Kind
      this.LoadCustomerKind();

      //2.Load dropdownlist ResponsiblePerson
      this.LoadResponsiblePerson();

      //3.Load Save Throught
      this.LoadSaleThrought();

      //4.Load dropdownlist Distribute
      this.LoadDistribute();      

      //5.Load Country
      Utility.LoadNation(cmbCountry);

      //6.Load Forwarder
      this.LoadForwarder();

      this.LoadCBGroupCustomer();

      //7.Load PriceBase
      Utility.LoadComboboxCodeMst(cmbPriceBase, ConstantClass.GROUP_PRICE_BASE);

      //8.Load Currency Sign
      Utility.LoadComboboxCodeMst(cmbCurrencySign, ConstantClass.GROUP_CURRENCY_SIGN);

      //9.Load Payment Term
      Utility.LoadComboboxCodeMst(cmbPaymentTerm, ConstantClass.GROUP_PAYMENT_TERM);

      //10.Load CB Consignee
      this.LoadCBConsignee();

      // 11. Load ultra cb country
      Utility.LoadUltCBNation(ultCBConsCountry);

      // 12.Bill Of Lading
      Utility.LoadUltraComboCodeMst(ultCBConsBill, ConstantClass.GROUP_BILL_OF_LADING);

      // 13.Certification Of Original
      Utility.LoadUltraComboCodeMst(ultCBConsCertification, ConstantClass.GROUP_CERTIFICATE);

      // 14.Packing List
      Utility.LoadUltraComboCodeMst(ultCBConsPackingList, ConstantClass.GROUP_PACKING);

      // 15.Invoice
      Utility.LoadUltraComboCodeMst(ultCBConsInvoice, ConstantClass.GROUP_INVOICE);
      
      //16.Load Region
      this.LoadCBRegion();

      //17.Load Price Option
      this.LoadCBPriceOption();

      //18. Load Res Sale
      this.LoadResSale();

    }

    /// <summary>
    /// Display or Visible controls lblDistribute and cmbDistibute1
    /// IF choose Dealer THEN Display
    /// ELSE Visible
    /// </summary>
    private void LoadStatusDistribute() {
      bool isDealer = radDealer.Checked;
      lblDistribute.Visible = isDealer;
      lblcmdDistribute.Visible = isDealer;
      cmbDistribute.Visible = isDealer;
    }

    /// <summary>
    /// Load information Other Contract Person
    /// </summary>
    private void LoadContactPerson()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CustomerPid", DbType.Int64, this.customerPid) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDContactPerson_Select", inputParam);
      dtSource.Columns["Name"].AllowDBNull = false;
      ultOtherContactPerson.DataSource = dtSource;
      ultOtherContactPerson.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultOtherContactPerson.DisplayLayout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
    }

    /// <summary>
    /// Load CB Consignee
    /// </summary>
    private void LoadCBConsignee()
    {
      string commandText = "SELECT Pid, ConsigneeCode, Name FROM TblCSDConsignee";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBConsignee.DataSource = dtSource;
      ultCBConsignee.DisplayMember = "ConsigneeCode";
      ultCBConsignee.ValueMember = "Pid";
      ultCBConsignee.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBConsignee.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBConsignee.DisplayLayout.Bands[0].Columns["ConsigneeCode"].MaxWidth = 100;
      ultCBConsignee.DisplayLayout.Bands[0].Columns["ConsigneeCode"].MinWidth = 100;
    }

    private void LoadCBGroupCustomer()
    {
      string cm = "SELECT Code,Value FROM TblBOMCodeMaster WHERE [Group] = 16033";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
     Utility.LoadUltraCombo(ultCBCusGroup,dt, "Code", "Value", false, "Code");
    }

    private void LoadCBPriceOption()
    {
      string cm = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 2016";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      Utility.LoadUltraCombo(ultCBPriceOption, dt, "Code", "Value", false, "Code");
    }

    private void LoadConsigneeInfo(long pid)
    {
      string commandText = string.Format("SELECT * FROM TblCSDConsignee WHERE Pid = {0}", pid);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        ultCBConsignee.Value = pid;
        txtConsName.Text = dtSource.Rows[0]["Name"].ToString();
        ultCBConsCountry.Value = dtSource.Rows[0]["Country"];
        txtConsState.Text = dtSource.Rows[0]["Region"].ToString();
        txtConsZipCode.Text = dtSource.Rows[0]["PostalCode"].ToString();
        txtConsStreet.Text = dtSource.Rows[0]["StreetAdress"].ToString();
        txtConsCity.Text = dtSource.Rows[0]["City"].ToString();
        txtConsTelephone.Text = dtSource.Rows[0]["Tel"].ToString();
        txtConsPOBox.Text = dtSource.Rows[0]["POBox"].ToString();
        txtConsEmail.Text = dtSource.Rows[0]["Email"].ToString();
        txtConsFax.Text = dtSource.Rows[0]["Fax"].ToString();
        ultCBConsBill.Value = dtSource.Rows[0]["BillOfLading"];
        txtConsContact.Text = dtSource.Rows[0]["Contact"].ToString();
        ultCBConsCertification.Value = dtSource.Rows[0]["Certificate"];
        int fumigation = DBConvert.ParseInt(dtSource.Rows[0]["Fumigation"].ToString());
        if (fumigation == 1)
        {
          rdConsYes.Checked = true;
        }
        else
        {
          rdConsNo.Checked = true;
        }
        ultCBConsPackingList.Value = dtSource.Rows[0]["PackingList"];
        ultCBConsInvoice.Value = dtSource.Rows[0]["Invoice"];
        txtConsDoor.Text = dtSource.Rows[0]["DoorDelivery"].ToString();
        txtConsPost.Text = dtSource.Rows[0]["PortOfDischarge"].ToString();
      }
    }

    /// <summary>
    /// Load Grid PricingHistory
    /// </summary>
    private void LoadPricingHistory(CSDCustomerInfo customer)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@CustomerPid", DbType.Int64, this.customerPid) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDCustomerPricing_History_Select", inputParam);
      ultPricingHistory.DataSource = dtSource;
      ultPricingHistory.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
      ultPricingHistory.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
      ultPricingHistory.DisplayLayout.Bands[0].Columns["Discount"].Header.Caption = "Discount (%)";
      ultPricingHistory.DisplayLayout.Bands[0].Columns["Discount"].Format = "#,###.##";
      ultPricingHistory.DisplayLayout.Bands[0].Columns["Discount"].CellAppearance.TextHAlign = HAlign.Right;
      ultPricingHistory.DisplayLayout.Bands[0].Columns["Markup"].Format = "#,###.##";
      ultPricingHistory.DisplayLayout.Bands[0].Columns["Markup"].CellAppearance.TextHAlign = HAlign.Right;
      ultPricingHistory.DisplayLayout.Bands[0].Columns["UpdateDate"].CellAppearance.TextHAlign = HAlign.Center;
      ultPricingHistory.DisplayLayout.Bands[0].Columns["UpdateDate"].Header.Caption = "Update Date";
    }

    /// <summary>
    /// Reload Information Forwarder
    /// </summary>
    private void Forwarder_SelectedChange()
    {
      long forwarderPid = DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbForwarder));
      CSDForwarder forwarder = new CSDForwarder();
      forwarder.Pid = forwarderPid;
      forwarder = (CSDForwarder)DataBaseAccess.LoadObject(forwarder, new string[] { "Pid" });
      if (forwarder == null)
      {
        txtForwarderContact.Text = string.Empty;
        txtForwarderCountry.Text = string.Empty;
        txtForwarderPostalCode.Text = string.Empty;
        txtForwarderRegion.Text = string.Empty;
        txtForwarderCity.Text = string.Empty;
        txtForwarderStreetAddress.Text = string.Empty;
        txtForwarderPOBox.Text = string.Empty;
        return;
      }
      txtForwarderContact.Text = forwarder.ContactPerson;
      string commandText = string.Format("SELECT NationEN FROM TblCSDNation WHERE pid = '{0}'", forwarder.Country);
      object country = DataBaseAccess.ExecuteScalarCommandText(commandText);
      txtForwarderCountry.Text = (country != null) ? country.ToString() : string.Empty;
      txtForwarderPostalCode.Text = forwarder.PostalCode;
      txtForwarderRegion.Text = forwarder.Region;
      txtForwarderCity.Text = forwarder.City;
      txtForwarderStreetAddress.Text = forwarder.StreetAdress;
      txtForwarderPOBox.Text = forwarder.POBox;
    }

    /// <summary>
    /// If Kind is OEM Then Checked radDistribute and disable radDistribute, radDealer
    /// Else enable radDistribute, radDealer
    /// </summary>
    private void Kind_SelectedChange() {
      string kind = Utility.GetSelectedValueCombobox(cmbKind);
      if (kind.Equals("5")) // OEM
      {
        radDistribute.Checked = true;
        radDistribute.Enabled = false;
        radDealer.Enabled = false;
      }
      else // JC OR JC & OEM
      {
        radDistribute.Enabled = true;
        radDealer.Enabled = true;
      }
    }

    /// <summary>
    /// Load General Information
    /// </summary>
    private void LoadGeneralInfo(CSDCustomerInfo customer) {
      // 1. Code
      txtCustomerCode.Text = customer.CustomerCode;
      // 2. Kind
      int kind = customer.Kind;
      try
      {
        cmbKind.SelectedValue = customer.Kind;
      }
      catch {
        cmbKind.SelectedIndex = 0;
      }
      this.Kind_SelectedChange();
      // 3. Name
      txtName.Text = customer.Name.Trim();
      // 4. ResponsiblePerson
      try
      {
        cmbResponsiblePerson.SelectedValue = customer.ResponsiblePerson;
      }
      catch {
        cmbResponsiblePerson.SelectedIndex = 0;
      }
      // 5. Sale Throught
      try
      {
        cmbSaleThrought.SelectedValue = customer.SaleThroughtPid;
      }
      catch
      {
        cmbSaleThrought.SelectedIndex = 0;
      }
      // 6. Distribute
      if (kind == 4 || kind == 6) { 
        bool isDistribute = (customer.ParentPid == long.MinValue);
        radDistribute.Checked = isDistribute;
        radDealer.Checked = !isDistribute;
        if (!isDistribute) {
          try
          {
            cmbDistribute.SelectedValue = customer.ParentPid;
          }
          catch {
            cmbDistribute.SelectedIndex = 0;
          }
        }
      }
      // 7. Enquiry Expire Days
      if (customer.EnquiryExpireDays != int.MinValue)
      {
        txtEnquiryExpireDays.Text = customer.EnquiryExpireDays.ToString();
      }
      else
      {
        txtEnquiryExpireDays.Text = string.Empty;
      }

      lbEnquiryExpireDays.Text = string.Format("(From 1 to {0} days)", this.maxEnquiryExpireDays);
      string cmd = string.Format("SELECT ResponsibleSale, TradeTerm, Status, OpenDate, CloseDate,GroupCustomerPid FROM TblCSDCustomerInfo WHERE Pid = {0}", customer.Pid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cmd);
      if (dt != null && dt.Rows.Count > 0)
      {
        cmbResSale.SelectedValue = DBConvert.ParseInt(dt.Rows[0]["ResponsibleSale"].ToString());
        txtTradeTerm.Text = dt.Rows[0]["TradeTerm"].ToString();
        ultDTOpenDate.Value = dt.Rows[0]["OpenDate"];
        ultDTCloseDate.Value = dt.Rows[0]["CloseDate"];
        if (dt.Rows[0]["GroupCustomerPid"].ToString().Length > 0)
        {
          ultCBCusGroup.Value = DBConvert.ParseInt(dt.Rows[0]["GroupCustomerPid"].ToString());
        }
        int status = DBConvert.ParseInt(dt.Rows[0]["Status"].ToString());
        if (status == 1)
        {
          rdActive.Checked = true;
        }
        else if (status == 0)
        {
          rdInactive.Checked = true;
        }
      }
    }
    
    /// <summary>
    /// Load Tab Contact Information
    /// </summary>
    private void LoadTabContact(CSDCustomerInfo customer)
    {
      // 1. Country
      try
      {
        cmbCountry.SelectedValue = customer.Country;
      }
      catch {
        cmbCountry.SelectedIndex = 0;
      }
      // 2. Zip / Postal Code
      txtPostalCode.Text = customer.PostalCode;
      // 3. State/Province/Region
      if (customer.Region != int.MinValue)
      {
        ultCBRegion.Value = customer.Region;
      }
      // 4. City
      txtCity.Text = customer.City;
      // 5. Street Address
      txtStreetAddress.Text = customer.StreetAddress;
      // 6. POBOX
      txtPOBox.Text = customer.PoBox;
      // 7. Contact Person
      txtContactPerson.Text = customer.ContactPerson.Trim();
      // 8. Telephone
      txtTelephone.Text = customer.CustomerTelephone.Trim();
      // 9. Fax
      txtFax.Text = customer.CustomerFax.Trim();
      // 10. Email
      txtEmail.Text = customer.CustomerEmail.Trim();
      // 11. Remark
      txtContactRemark.Text = customer.ContactRemark.Trim();
      // 12. Other Contact Person
      this.LoadContactPerson();
    }

    /// <summary>
    /// Load Tab Shipping Information
    /// </summary>
    private void LoadTabShipping(CSDCustomerInfo customer)
    {
      // 1. Forwarder Infomation
      try
      {
        cmbForwarder.SelectedValue = customer.ForwarderPid;        
      }
      catch {
        cmbForwarder.SelectedIndex = 0;
      }
      this.Forwarder_SelectedChange();
      
      // 2. Shiping Remark
      txtShippingRemark.Text = customer.ShippingRemark;
      // 3. Consignee Detail
      this.LoadConsignee();
      //this.LoadDealerConsignee();
    }

    /// <summary>
    /// Load Tab Pricing Information
    /// </summary>
    private void LoadTabPricing(CSDCustomerInfo customer)
    {
      // 1. Pricing Information
      // 1.1 Price Base
      try
      {
        cmbPriceBase.SelectedValue = customer.PriceBase;
      }
      catch {
        cmbPriceBase.SelectedIndex = 0;
      }
      // 1.2 Markup
      txtMarkup.Text = DBConvert.ParseString(customer.Markup);
      // 1.3 Discount
      txtDiscount.Text = DBConvert.ParseString(customer.Discount);
      // 1.4 Currency Sign
      try
      {
        cmbCurrencySign.SelectedValue = customer.CurrencySign;
      }
      catch
      {
        cmbCurrencySign.SelectedIndex = 0;
      }
      // Price Option
      if (customer.PriceOption > 0)
      {
        ultCBPriceOption.Value = customer.PriceOption;
      }

      // 1.5 Pricing History
      this.LoadPricingHistory(customer);

      // 2. Payment Information
      // 2.1 Bank Account
      txtBankAccount.Text = customer.BankAccount.Trim();
      // 2.2 Switf
      txtSwitf.Text = customer.Switf.Trim();
      // 2.3 Bank Name
      txtBankName.Text = customer.BankName.Trim();
      // 2.4 Payment Term
      try
      {
        cmbPaymentTerm.SelectedValue = customer.PaymentTerm;
      }
      catch
      {
        cmbPaymentTerm.SelectedIndex = 0;
      }
      // 2.5 Bank Address
      txtBankAddress.Text = customer.BankAddress.Trim();
      // 2.6 Bank Attn
      txtBankAttn.Text = customer.BankAttn.Trim();
      // 2.7 Remark
      txtPricingRemark.Text = customer.PricingRemark.Trim();
      // 2.8 Price Type
      if (customer.PriceType == 1)
      {
        rdFixPrice.Checked = true;
      }
      else
      {
        rdNoFixPrice.Checked = true;
      }
      txtDeposit.Text = DBConvert.ParseString(customer.Deposit);
    }

    /// <summary>
    /// Load information on screen.
    /// </summary>
    private void LoadData()
    {
      //btnAddConsignee.Enabled = (this.customerPid != long.MinValue);
      this.isLoaddingData = true;
      this.listDeletedPid = new ArrayList();
      this.LoadConsignee();
      if (this.customerPid == long.MinValue)
      {
        this.LoadStatusDistribute();
        this.LoadContactPerson();
        txtEnquiryExpireDays.Text = "14";
        this.NeedToSave = false;
        this.isChangeOtherInfo = false;
        this.isLoaddingData = false;
        return;
      }
      CSDCustomerInfo customer = new CSDCustomerInfo();
      customer.Pid = this.customerPid;
      customer = (CSDCustomerInfo)DataBaseAccess.LoadObject(customer, new string[] { "Pid" });
      if (customer == null)
      {
        WindowUtinity.ShowMessageError("ERR0007");
        this.CloseTab();
        return;
      }
      this.LoadGeneralInfo(customer);
      switch (this.currentTabIndex)
      {
        case 0:
          this.LoadTabContact(customer);
          break;
        case 1:
          this.LoadTabShipping(customer);
          break;
        case 2:
          this.LoadTabPricing(customer);
          break;
      }
      this.NeedToSave = false;
      this.isChangeOtherInfo = false;
      this.isLoaddingData = false;
      this.listDeletedOtherContactPersonPid = new ArrayList();      
    }

    private void LoadConsignee()
    {
      if (this.customerPid != long.MinValue)
      {
        string cmd = string.Format("SELECT Consignee FROM TblCSDCustomerConsignee WHERE Customer = {0}", this.customerPid);
        object obj = DataBaseAccess.ExecuteScalarCommandText(cmd);
        if (obj != null && DBConvert.ParseLong(obj.ToString()) != long.MinValue)
        {
          long consPid = DBConvert.ParseLong(obj.ToString());
          this.LoadConsigneeInfo(consPid);
        }
      }
    }
    #endregion Load Data

    #region Check & Save Data
    /// <summary>
    /// Check logic
    /// </summary>
    private bool CheckInvalid() {
      string value = txtCustomerCode.Text.Trim();
      if (value.Length == 0) {
        WindowUtinity.ShowMessageError("ERR0001", new string[] { "Code" });
        txtCustomerCode.Focus();
        return false;
      }

      if (cmbSaleThrought.Text.ToString().Length <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0115", "Sale Throught");
        return false;
      }

      if (ultCBRegion.SelectedRow == null)
      {
        WindowUtinity.ShowMessageError("ERR0115", "Region");
        return false;
      }
      int selectedIndex = cmbKind.SelectedIndex;
      if (selectedIndex <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", new string[] { "Kind" });
        cmbKind.Focus();
        return false;
      }
      else if(this.customerPid > 0)
      {        
        int kind = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbKind));
        if (kind == 4)
        {
          string commandCheck = string.Format("SELECT COUNT(*) FROM TblBOMItemBasic WHERE CustomerPid = {0}", this.customerPid);
          int count = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(commandCheck));
          if (count > 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Can't change kind of this customer because having some items is belonged to him.");
            cmbKind.Focus();
            return false;
          }
        }
      }
      value = txtName.Text.Trim();
      if (value.Length == 0)
      {
        txtName.Focus();
        WindowUtinity.ShowMessageError("ERR0001", new string[] { "Name" });
        return false;
      }
      if (radDealer.Checked) {
        selectedIndex = cmbDistribute.SelectedIndex;
        if (selectedIndex <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", new string[] { "Distribute" });
          cmbDistribute.Focus();
          return false;
        }
      }
      int enquiryExpireDays = DBConvert.ParseInt(txtEnquiryExpireDays.Text);
      if (enquiryExpireDays <= 0 || enquiryExpireDays > this.maxEnquiryExpireDays)
      {
        txtEnquiryExpireDays.Focus();
        WindowUtinity.ShowMessageError("ERR0001", new string[] { "Enquiry Expire Days" });
        return false;
      }
      if (this.ultCBCusGroup.Text.ToString().Length > 0)
      {
        if (this.ultCBCusGroup.Value != null)
        {
          string cm = string.Format("SELECT COUNT(*) FROM TblBOMCodeMaster WHERE [Group] = 16033 AND CODE ={0}", this.ultCBCusGroup.Value.ToString());
          int check = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText(cm));
          if (check <= 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Wrong Customer Group");
            return false;
          }
        }
        else
        {
          WindowUtinity.ShowMessageErrorFromText("Wrong Customer Group");
          return false;
        }
      }    
      return true;
    }

    /// <summary>
    /// Set General Information
    /// </summary>
    /// <returns></returns>
    private CSDCustomerInfo SetGeneralInfo(){
      CSDCustomerInfo customer = new CSDCustomerInfo();
      if (this.customerPid != long.MinValue) {
        customer.Pid = this.customerPid;
        customer = (CSDCustomerInfo)DataBaseAccess.LoadObject(customer, new string[] { "Pid" });
        if (customer == null)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          this.CloseTab();
          return null;
        }
      }
      // 1. Code
      customer.CustomerCode = txtCustomerCode.Text.Trim();
      // 2. Kind
      customer.Kind = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbKind));
      // 3. Name
      customer.Name = txtName.Text.Trim();
      // 4. Responsible Person
      customer.ResponsiblePerson =  DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbResponsiblePerson));
      // 5. Sale Throught
      customer.SaleThroughtPid = DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbSaleThrought));
      // 6. Distribute
      if (radDistribute.Checked)
      {
        customer.ParentPid = long.MinValue;
      }
      else {
        customer.ParentPid = DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbDistribute));
      }
      // 7. Enquiry Expire Days
      customer.EnquiryExpireDays = DBConvert.ParseInt(txtEnquiryExpireDays.Text);
      
      return customer;
    }

    /// <summary>
    /// Insert or Update record tblCSDCustomerInfo form object customer
    /// </summary>
    /// <param name="customer">object CSDCustomerInfo which we want to update</param>
    /// <returns></returns>
    private bool SaveCustomerInfo(CSDCustomerInfo customer) {
      bool result;
      if (customer.Pid != long.MinValue)
      {
        customer.UpdateBy = SharedObject.UserInfo.UserPid;
        customer.UpdateDate = DateTime.Now;
        result = DataBaseAccess.UpdateObject(customer, new string[] { "Pid" });
      }
      else
      {
        customer.CreateBy = SharedObject.UserInfo.UserPid;
        customer.CreateDate = DateTime.Now;
        long pid = DataBaseAccess.InsertObject(customer);
        customer.Pid = pid;
        result = (pid != long.MinValue);
      }
      if (!result)
      {
        return false;
      }
      this.customerPid = customer.Pid;
      //btnAddConsignee.Enabled = (this.customerPid != long.MinValue);
      return true;
    }

    private long SaveCustomerInfo()
    {
      // Get input data
      string cusCode = txtCustomerCode.Text.Trim();
      int kind = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbKind));
      string name = txtName.Text.Trim();
      int resPerson = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbResponsiblePerson));
      long throught = DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbSaleThrought));
      int enExpire = DBConvert.ParseInt(txtEnquiryExpireDays.Text);
      int status = rdActive.Checked ? 1 : 0;
      long parentPid = DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbDistribute));
      int resSale = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbResSale));
      string tradeTerm = txtTradeTerm.Text.Trim();

      DateTime openDate = new DateTime();
      if (ultDTOpenDate.Value != null)
      {
        openDate = (DateTime)ultDTOpenDate.Value;
      }
      DateTime closeDate = new DateTime();
      if (ultDTCloseDate.Value != null)
      {
        closeDate = (DateTime)ultDTCloseDate.Value;
      }

      DBParameter[] input = new DBParameter[16];
      if (this.customerPid != long.MinValue)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, this.customerPid);
      }
      input[1] = new DBParameter("@CustomerCode", DbType.AnsiString, 8, cusCode);
      input[2] = new DBParameter("@Kind", DbType.Int32, kind);
      input[3] = new DBParameter("@Name", DbType.String, 128, name);
      if (resPerson != int.MinValue)
      {
        input[4] = new DBParameter("@ResPerson", DbType.Int32, resPerson);
      }
      if (throught != long.MinValue)
      {
        input[5] = new DBParameter("@SaleThroughtPID", DbType.Int64, throught);
      }
      if (status != int.MinValue)
      {
        input[6] = new DBParameter("@Status", DbType.Int32, status);
      }
      if (radDealer.Checked)
      {
        input[7] = new DBParameter("@ParentPid", DbType.Int64, parentPid);
      }
      if (enExpire != int.MinValue)
      {
        input[8] = new DBParameter("@ENExpireDays", DbType.Int32, enExpire);
      }
      if (resSale != int.MinValue)
      {
        input[9] = new DBParameter("@ResSale", DbType.Int32, 128, resSale);
      }
      if (tradeTerm.Length > 0)
      {
        input[10] = new DBParameter("@TradeTerm", DbType.String, 128, tradeTerm);
      }
      if (openDate != DateTime.MinValue)
      {
        input[11] = new DBParameter("@OpenDate", DbType.DateTime, openDate);
      }
      if (closeDate != DateTime.MinValue)
      {
        input[12] = new DBParameter("@CloseDate", DbType.DateTime, closeDate);
      }

      if (ultCBCusGroup.Value != null && DBConvert.ParseInt(ultCBCusGroup.Value.ToString()) != int.MinValue)
      {
        input[13] = new DBParameter("@GroupCustomer", DbType.Int32, DBConvert.ParseInt(ultCBCusGroup.Value.ToString()));
      }

      input[14] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spCSDCustomerInfo_Edit", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      return result;
    }

    /// <summary>
    /// Update TblCSDContactPerson from ultOtherContactPerson
    /// </summary>
    /// <returns></returns>
    private bool SaveOtherContactPerson() {
      if (this.customerPid == long.MinValue) {
        return false;
      }
      bool result = true;
      long outputValue = 0;
      // 1. Delete
      foreach (long deletePid in this.listDeletedOtherContactPersonPid) {
        DBParameter[] inputParamsDelete = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, deletePid)};
        DBParameter[] outputParamsDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0)};
        DataBaseAccess.ExecuteStoreProcedure("spCSDContactPerson_Delete", inputParamsDelete, outputParamsDelete);
        outputValue = DBConvert.ParseLong(outputParamsDelete[0].Value.ToString());
        if (outputValue <= 0) {
          result = false;
        }
      }
      // 2. Insert/Update
      for (int i = 0; i < ultOtherContactPerson.Rows.Count; i++) {
        string storeName = string.Empty;
        UltraGridRow row = ultOtherContactPerson.Rows[i];
        DBParameter[] outputParams = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DBParameter[] inputParams = new DBParameter[9];
        long pid = DBConvert.ParseLong( row.Cells["Pid"].Value.ToString());
        // 2.1 Pid, CreateBy, UpdateBy
        if (pid != long.MinValue)
        {
          inputParams[0] = new DBParameter("@Pid", DbType.Int64, pid);
          inputParams[8] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          storeName = "spCSDContactPerson_Update";
        }
        else {
          inputParams[8] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          storeName = "spCSDContactPerson_Insert";
        }
        // 2.2 CustomerPid
        inputParams[1] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);
        // 2.3 Name
        string text = row.Cells["Name"].Value.ToString().Trim();
        inputParams[2] = new DBParameter("@Name", DbType.AnsiString, 128, text);
        // 2.4 Position
        text = row.Cells["Position"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParams[3] = new DBParameter("@Position", DbType.AnsiString, 128, text);          
        }
        // 2.5 Tel1
        text = row.Cells["Tel1"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParams[4] = new DBParameter("@Tel1", DbType.AnsiString, 32, text);
        }
        // 2.6 Tel2
        text = row.Cells["Tel2"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParams[5] = new DBParameter("@Tel2", DbType.AnsiString, 32, text);
        }
        // 2.7 Email1
        text = row.Cells["Email1"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParams[6] = new DBParameter("@Email1", DbType.AnsiString, 128, text);
        }
        // 2.8 Email2
        text = row.Cells["Email2"].Value.ToString().Trim();
        if (text.Length > 0)
        {
          inputParams[7] = new DBParameter("@Email2", DbType.AnsiString, 128, text);
        }
        DataBaseAccess.ExecuteStoreProcedure(storeName, inputParams, outputParams);
        outputValue = DBConvert.ParseLong(outputParams[0].Value.ToString());
        if (outputValue <= 0)
        {
          result = false;
        }
      }
      return result;
    }

    /// <summary>
    /// Save Contact Information
    /// </summary>
    /// <returns>true : save success; false : save unsuccess</returns>
    private bool SaveTabContact(CSDCustomerInfo cus)
    {
      // 1. Country
      cus.Country = DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbCountry));
      // 2. Zip / Postal Code
      cus.PostalCode = txtPostalCode.Text.Trim();
      // 3. State/Province/Region
      cus.Region = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ultCBRegion));
      // 4. City
      cus.City = txtCity.Text.Trim();
      // 5. Street Address
      cus.StreetAddress = txtStreetAddress.Text.Trim();
      // 6. POBox
      cus.PoBox = txtPOBox.Text.Trim();
      // 7. Contact Person
      cus.ContactPerson = txtContactPerson.Text.Trim();
      // 8. Telephone
      cus.CustomerTelephone = txtTelephone.Text.Trim();
      // 9. Fax
      cus.CustomerFax = txtFax.Text.Trim();
      // 10. Email
      cus.CustomerEmail = txtEmail.Text.Trim();
      // 11. Contact Remark
      cus.ContactRemark = txtContactRemark.Text.Trim();
      bool saveCustomer; 
      bool saveOtherContactPerson;
      saveCustomer = this.SaveCustomerInfo(cus);
      saveOtherContactPerson = this.SaveOtherContactPerson();
      return (saveCustomer && saveOtherContactPerson);
    }

    /// <summary>
    /// Save Shipping Information
    /// </summary>
    /// <returns>true : save success; false : save unsuccess</returns>
    private bool SaveTabShipping(CSDCustomerInfo cus)
    {
      // 1. Forwarder
      cus.ForwarderPid = DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbForwarder));
      // 2. Shipping remark
      cus.ShippingRemark = txtShippingRemark.Text.Trim();
      bool saveCustomer = this.SaveCustomerInfo(cus);
      return saveCustomer;
    }

    /// <summary>
    /// If change Pricing Info then Save PricingHistory
    /// </summary>
    /// <param name="customer"></param>
    private void SavePricingHistory(CSDCustomerInfo customer) {
      string commandText = string.Format(@"SELECT TOP 1 PriceBase, CurrencySign, Markup, Discount, PriceType, PriceOption
                                           FROM TblCSDCustomerPricing_History 
                                           WHERE CustomerPid = '{0}'
                                           ORDER BY Pid DESC", customer.Pid);
      DataTable dtHistory = DataBaseAccess.SearchCommandTextDataTable(commandText);
      bool isInsertHistory = false;
      if (dtHistory == null || dtHistory.Rows.Count == 0)
      {
        if (customer.PriceBase != int.MinValue || customer.CurrencySign != int.MinValue || customer.Discount != double.MinValue
          || customer.Markup != double.MinValue || customer.PriceType != int.MinValue || customer.PriceOption != int.MinValue)
        {
          isInsertHistory = true;
        }
      }
      else {
        int priceBase = DBConvert.ParseInt(dtHistory.Rows[0]["PriceBase"].ToString());
        int currencySign = DBConvert.ParseInt(dtHistory.Rows[0]["CurrencySign"].ToString());
        double markup = DBConvert.ParseDouble(dtHistory.Rows[0]["Markup"].ToString());
        double discount = DBConvert.ParseDouble(dtHistory.Rows[0]["Discount"].ToString());
        int priceType = DBConvert.ParseInt(dtHistory.Rows[0]["PriceType"].ToString());
        int priceOption = DBConvert.ParseInt(dtHistory.Rows[0]["PriceOption"].ToString());
        if (customer.PriceBase != priceBase || customer.CurrencySign != currencySign
          || customer.Discount != discount || customer.Markup != markup || customer.PriceType != priceType || customer.PriceOption != priceOption) 
        {
          isInsertHistory = true;
        }
      }
      if (isInsertHistory) {
        DBParameter[] input = new DBParameter[8];
        input[0] = new DBParameter("@CustomerPid", DbType.Int64, customer.Pid);
        if (customer.Markup != double.MinValue)
        {
          input[1] = new DBParameter("@Markup", DbType.Double, customer.Markup);
        }
        if (customer.Discount != double.MinValue)
        {
          input[2] = new DBParameter("@Discount", DbType.Double, customer.Discount);
        }
        if (customer.PriceBase != int.MinValue)
        {
          input[3] = new DBParameter("@PriceBase", DbType.Int32, customer.PriceBase);
        }
        if (customer.CurrencySign != int.MinValue)
        {
          input[4] = new DBParameter("@CurrencySign", DbType.Int32, customer.CurrencySign);
        }
        input[5] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        input[6] = new DBParameter("@PriceType", DbType.Int32, customer.PriceType);
        if (customer.PriceOption != int.MinValue)
        {
          input[7] = new DBParameter("@PriceOption", DbType.Int32, customer.PriceOption);
        }
        DataBaseAccess.ExecuteStoreProcedure("spCSDCustomerPricing_History_Insert", input);
      }
    }
    /// <summary>
    /// Save Pricing Information, save Pricing History
    /// </summary>
    /// <returns>true : save success; false : save unsuccess</returns>
    private bool SaveTabPricing(CSDCustomerInfo customer)
    {
      // 1. Price Base
      customer.PriceBase = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbPriceBase));
      // 2. Markup
      customer.Markup = DBConvert.ParseDouble(txtMarkup.Text);
      // 3. Discount
      customer.Discount = DBConvert.ParseDouble(txtDiscount.Text);
      // 4. Currency Sign
      customer.CurrencySign = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbCurrencySign));
      // 5. Bank Account
      customer.BankAccount = txtBankAccount.Text.Trim();
      // 6. Switf
      customer.Switf = txtSwitf.Text.Trim();
      // 7. Bank Name
      customer.BankName = txtBankName.Text.Trim();
      // 8. Payment Term
      customer.PaymentTerm = DBConvert.ParseInt(Utility.GetSelectedValueCombobox(cmbPaymentTerm));
      // 9. Bank Address
      customer.BankAddress = txtBankAddress.Text.Trim();
      // 10. Bank Attn
      customer.BankAttn = txtBankAttn.Text.Trim();
      // 11. Pricing Remark
      customer.PricingRemark = txtPricingRemark.Text;
      // 12. Price Type
      customer.PriceType = rdFixPrice.Checked ? 1 : 0;
      // 13. Deposit
      customer.Deposit = DBConvert.ParseDouble(txtDeposit.Text);
      //14. Price Option
      if (ultCBPriceOption.Value != null)
      {
          customer.PriceOption = DBConvert.ParseInt(ultCBPriceOption.Value.ToString());
      }
      else
      {
          customer.PriceOption = int.MinValue;
      }

      bool result = this.SaveCustomerInfo(customer);
      if (result) {
        this.SavePricingHistory(customer);
      }
      return result;
    }

    /// <summary>
    /// Save data on screen in database
    /// </summary>
    /// <returns>true : save success; false : save unsuccess</returns>
    private bool SaveData() 
    {
      bool result = false;
      CSDCustomerInfo cus = new CSDCustomerInfo();
      long pidCus = this.SaveCustomerInfo();
      if (pidCus == long.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return false;
      }
      else
      {
        cus.Pid = pidCus;
        cus = (CSDCustomerInfo)DataBaseAccess.LoadObject(cus, new string[] { "Pid" });
        switch (this.currentTabIndex)
        {
          case 0:
            result = this.SaveTabContact(cus);
            break;
          case 1:
            result = this.SaveTabShipping(cus);
            result = this.saveConsigneeOfCustomer();
            break;
          case 2:
            result = this.SaveTabPricing(cus);
            break;
          default:
            break;
        }
        if (result)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0005");
        }
      }
      return result;
    }

    /// <summary>
    /// save customer of consignee
    /// </summary>
    private bool saveConsigneeOfCustomer()
    {
      //Insert relation
      DBParameter[] inputParamCons = new DBParameter[3];
      string storenameCons = string.Empty;
      long consigneePid = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultCBConsignee));
      string commandText = string.Format("SELECT Pid FROM TblCSDCustomerConsignee WHERE Customer = {0}", this.customerPid);
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      long rel = (obj != null && DBConvert.ParseLong(obj.ToString()) != long.MinValue) ? DBConvert.ParseLong(obj.ToString()) : long.MinValue;
      if (rel == long.MinValue)
      {
        storenameCons = "spCSDCustomerConsignee_Insert";
        inputParamCons[0] = new DBParameter("@Select", DbType.Int32, 1);
      }
      else
      {
        storenameCons = "spCSDCustomerConsignee_Update";
        inputParamCons[0] = new DBParameter("@Pid", DbType.Int64, rel);
      }
      if (consigneePid != long.MinValue)
      {
        inputParamCons[1] = new DBParameter("@Customer", DbType.Int64, this.customerPid);
        inputParamCons[2] = new DBParameter("@Consignee", DbType.Int64, consigneePid);

        DBParameter[] outParam = new DBParameter[1];
        outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure(storenameCons, inputParamCons, outParam);
        if (DBConvert.ParseLong(outParam[0].Value.ToString()) == 0 && DBConvert.ParseLong(outParam[0].Value.ToString()) == long.MinValue)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary> 
    /// Save data before close
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      if (this.CheckInvalid())
      {
        if (this.SaveData())
        {
          this.SaveSuccess = true;
        }
      }
      else
      {
        this.SaveSuccess = false;
      }
    }
    #endregion Check & Save Data

    #region Events
    /// <summary>
    /// Check duplicate CustomerCode
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtCustomerCode_Leave(object sender, EventArgs e)
    {
      
      DBParameter[] input = new DBParameter[]{ new DBParameter("@CustomerCode", DbType.AnsiString, 8, txtCustomerCode.Text.Trim())};
      string commandText = "SELECT Count(*) FROM TblCSDCustomerInfo WHERE CustomerCode = @CustomerCode";
      if(this.customerPid != long.MinValue){
        commandText = string.Format("{0} AND Pid <> {1}", commandText, this.customerPid);
      }
      object result = DataBaseAccess.ExecuteScalarCommandText(commandText, input);
      int count = (result != null) ? DBConvert.ParseInt(result.ToString()) : 0;
      if(count > 0){
        WindowUtinity.ShowMessageError("ERR0006", new string[] { "Code" });
        txtCustomerCode.Focus();        
      }
    }

    /// <summary>
    /// If change Forwarder then reload Contact, Address, POD.
    /// </summary>
    private void cmbForwarder_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.isLoaddingData)
      {
        return;
      }
      this.isChangeOtherInfo = (btnSave.Visible);
      this.NeedToSave = (btnSave.Visible);
      this.Forwarder_SelectedChange();
    }

    /// <summary>
    /// IF OEM is chosen THEN radDistribute, radDealer will be disabled and set radDistribute.Checked = true
    /// ELSE radDistribute and radDealer will be enabled
    /// </summary>
    private void cmbKind_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.isLoaddingData)
      {
        return;
      }
      this.NeedToSave = (btnSave.Visible);
      this.Kind_SelectedChange();
    }
    
    /// <summary>
    /// If there are some change value (this.isChangeOtherInfo = true) Then confirm save data Else nothing
    /// Confirm save data : chose "Yes" -> save, "No" -> don't save
    /// </summary>
    private void tabOtherInformation_Deselecting(object sender, TabControlCancelEventArgs e)
    {
      if (this.isChangeOtherInfo)
      {
        DialogResult dlg = MessageBox.Show(FunctionUtility.GetMessage("MSG0008"), "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (dlg == DialogResult.Yes)
        {
          if (!this.CheckInvalid())
          {
            e.Cancel = true;
            return;
          }
          bool success = this.SaveData();
          if (!success)
          {
            WindowUtinity.ShowMessageError("ERR0005");
            this.LoadData();
            e.Cancel = true;
            return;
          }
        }
        else if (dlg == DialogResult.Cancel)
        {
          e.Cancel = true;
        }
      }      
    }

    /// <summary>
    /// Load data on selected tab
    /// </summary>
    private void tabOtherInformation_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.currentTabIndex = tabOtherInformation.SelectedIndex;
      this.LoadData();
    }

    /// <summary>
    /// Display or disable comtrol Distribute and set this.isChangeGeneralInfo = (btnSave.Visible);
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void rad_CheckedChanged(object sender, EventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
      this.LoadStatusDistribute();
    }

    /// <summary>
    /// Set this.NeedToSave = (btnSave.Visible); this.isChangeOtherInfo = (btnSave.Visible);
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultOtherInfo_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
      this.isChangeOtherInfo = (btnSave.Visible);
    }

    /// <summary>
    /// 1. Get list pids of TblCSDContactPerson which are deleted.
    /// 2. Set field NeedToSave = (btnSave.Visible).
    /// 3. Set field this.isChangeOtherInfo = (btnSave.Visible);
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultOtherContactPerson_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.NeedToSave = (btnSave.Visible);
      this.isChangeOtherInfo = (btnSave.Visible);
      foreach (long pid in this.listDeletingOtherContactPersonPid)
      {
        this.listDeletedOtherContactPersonPid.Add(pid);
      }
    }

    /// <summary>
    /// Get list pids of TblCSDContactPerson which are deleting.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultOtherContactPerson_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingOtherContactPersonPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long Pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (Pid != long.MinValue)
        {
          this.listDeletingOtherContactPersonPid.Add(Pid);
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Save data on screen in database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (!this.CheckInvalid())
      {
        return;
      }
      this.SaveData();
      this.LoadData();
    }

    /// <summary>
    /// Other object in group General Information change value : set this.isChangeGeneralInfo = (btnSave.Visible) 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GeneralInfo_ChangedValue(object sender, EventArgs e)
    {
      if (this.isLoaddingData)
      {
        return;
      }
      this.NeedToSave = (btnSave.Visible && btnSave.Enabled);
    }

    /// <summary>
    /// Other object in tabOtherInformation change value : set this.isChangeOtherInfo = (btnSave.Visible) 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OtherInfo_ChangedValue(object sender, EventArgs e)
    {
      if (this.isLoaddingData)
      {
        return;
      }
      this.isChangeOtherInfo = (btnSave.Visible);
      this.NeedToSave = (btnSave.Visible);
    }

    /// <summary>
    /// Open 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNewForwarder_Click(object sender, EventArgs e)
    {
      long forwarderPid = long.MinValue;
      if (cmbForwarder.SelectedIndex > 0)
      {
        forwarderPid = DBConvert.ParseLong(cmbForwarder.SelectedValue.ToString());
      }
      viewCSD_01_002 view = new viewCSD_01_002();
      WindowUtinity.ShowView(view, "New Forwarder", false, ViewState.ModalWindow);

      //6.Load Forwarder
      this.LoadForwarder();
      if (forwarderPid != long.MinValue)
      {
        cmbForwarder.SelectedValue = forwarderPid;
      }
      else
      {
        cmbForwarder.SelectedIndex = 1;
      }
      this.Forwarder_SelectedChange();
    }

    private void btnEditForwarder_Click(object sender, EventArgs e)
    {
      long pid = DBConvert.ParseLong(cmbForwarder.SelectedValue.ToString());
      viewCSD_01_002 view = new viewCSD_01_002();
      view.forwardPid = pid;
      WindowUtinity.ShowView(view, "FORWARDER INFORMATION", false, ViewState.ModalWindow);
      this.Forwarder_SelectedChange();
    }

    private void ultCBConsignee_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBConsignee.SelectedRow != null)
      {
        long pid = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultCBConsignee));
        this.LoadConsigneeInfo(pid);
      }
    }

    private void btnNewConsignee_Click(object sender, EventArgs e)
    {
      long consigneePid = long.MinValue;
      if (ultCBConsignee.SelectedRow != null)
      {
        consigneePid = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultCBConsignee));
      }

      viewCSD_02_003 view = new viewCSD_02_003();
      WindowUtinity.ShowView(view, "NEW CONSIGNEE", true, ViewState.ModalWindow);
      this.LoadCBConsignee();
      if (consigneePid != long.MinValue)
      {
        ultCBConsignee.Value = consigneePid;
      }
      else
      {
        int index = ultCBConsignee.Rows.Count - 1;
        ultCBConsignee.SelectedRow = ultCBConsignee.Rows[index];
        consigneePid = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultCBConsignee));
      }
      this.LoadConsigneeInfo(consigneePid);
    }

    private void btnEditConsignee_Click(object sender, EventArgs e)
    {
      if (ultCBConsignee.SelectedRow != null)
      {
        long pid = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ultCBConsignee));
        viewCSD_02_003 view = new viewCSD_02_003();
        view.consigneePid = pid;
        WindowUtinity.ShowView(view, "EDIT CONSIGNEE", true, ViewState.ModalWindow);
        this.LoadCBConsignee();
        this.LoadConsigneeInfo(pid);
      }
    }
    #endregion Events

   
  }
}
