/*
  Author      : Võ Hoa Lư
  Date        : 16/12/2010
  Decription  : Insert, Update CSDConsignee  
  Updater     : 24-09-2011 Sửa lại cấu trúc customer Consignee 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Objects;
using DaiCo.Application;
namespace DaiCo.ERPProject
{
  public partial class viewCSD_02_003 : MainUserControl
  {
    #region Fields
    public long consigneePid = long.MinValue;
    #endregion Fields

    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewCSD_02_003()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_02_003_Load(object sender, EventArgs e)
    {
      this.LoadDropDownList();
      this.LoadData();
    }
    #endregion Init Data

    #region Load Data
    /// <summary>
    /// Load all dropdownlist in screen
    /// </summary>
    private void LoadDropDownList()
    {
      //1.Load Country
      ControlUtility.LoadNation(cmbCountry);
      //2.Bill Of Lading
      ControlUtility.LoadComboboxCodeMst(cmbBillOfLading, ConstantClass.GROUP_BILL_OF_LADING);
      //3.Certification Of Original
      ControlUtility.LoadComboboxCodeMst(cmbCertification, ConstantClass.GROUP_CERTIFICATE);
      //4.Packing List
      ControlUtility.LoadComboboxCodeMst(cmbParking, ConstantClass.GROUP_PACKING);
      //5.Invoice
      ControlUtility.LoadComboboxCodeMst(cmbInvoice, ConstantClass.GROUP_INVOICE);
    }

    /// <summary>
    /// Load information on screen.
    /// </summary>
    private void LoadData()
    {     
      if (this.consigneePid != long.MinValue)
      {
        CSDConsignee consignee = new CSDConsignee();
        consignee.Pid = this.consigneePid;
        consignee = (CSDConsignee)DataBaseAccess.LoadObject(consignee, new string[] { "Pid" });
        if (consignee == null)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          this.CloseTab();
          return;
        }
        txtConsigneeCode.Text = consignee.ConsigneeCode;
        txtConsigneeName.Text = consignee.Name;
        try
        {
          cmbCountry.SelectedValue = consignee.Country;
        }
        catch { }
        txtPostalCode.Text = consignee.PostalCode;
        txtRegion.Text = consignee.Region;
        txtCity.Text = consignee.City;
        txtStreetAddress.Text = consignee.StreetAdress;
        txtPOBox.Text = consignee.POBox;
        txtPhone.Text = consignee.Tel;
        txtFax.Text = consignee.Fax;
        txtEmail.Text = consignee.Email;
        txtContact.Text = consignee.Contact;
        try
        {
          cmbBillOfLading.SelectedValue = consignee.BillOfLading;
        }
        catch { }
        if (consignee.Fumigation == 1)
        {
          radFumigationYes.Checked = true;
        }
        else {
          radFumigationNo.Checked = true;
        }
        try
        {
          cmbCertification.SelectedValue = consignee.Certificate;
        }
        catch { }
        try
        {
          cmbParking.SelectedValue = consignee.PackingList;
        }
        catch { }
        try
        {
          cmbInvoice.SelectedValue = consignee.Invoice;
        }
        catch { }
        txtDoorDelivery.Text = consignee.DoorDelivery;
        txtPOD.Text = consignee.PortOfDischarge;
      }
      this.NeedToSave = false;
    }

    /// <summary>
    /// Clear screen, update this.consigneePid = long.MinValue, 
    /// </summary>
    private void Clear()
    {
      this.NeedToSave = false;
      this.consigneePid = long.MinValue;
      txtConsigneeCode.Text = string.Empty;
      txtConsigneeName.Text = string.Empty;
      cmbCountry.SelectedIndex = 0;
      txtPostalCode.Text = string.Empty;
      txtRegion.Text = string.Empty;
      txtCity.Text = string.Empty;
      txtStreetAddress.Text = string.Empty;
      txtPOBox.Text = string.Empty;
      txtPhone.Text = string.Empty;
      txtFax.Text = string.Empty;
      txtEmail.Text = string.Empty;
      txtContact.Text = string.Empty;
      cmbBillOfLading.SelectedIndex = 0;
      radFumigationNo.Checked = true;
      cmbCertification.SelectedIndex = 0;
      cmbParking.SelectedIndex = 0;
      cmbInvoice.SelectedIndex = 0;
      txtDoorDelivery.Text = string.Empty;
      txtPOD.Text = string.Empty;      
    }
    #endregion Load Data

    #region Check & Save Data
    private bool CheckSameCode() {
      string code = txtConsigneeCode.Text.Trim();
      if (code.Length == 0)
      {
        return true;
      }
      string commandText = "SELECT Pid FROM TblCSDConsignee WHERE ConsigneeCode = @Code";
      DBParameter[] input = new DBParameter[] { new DBParameter("@Code", DbType.AnsiString, 8, code) };
      object result = DataBaseAccess.ExecuteScalarCommandText(commandText, input);
      if (result == null)
      {
        return true;
      }
      long pid = DBConvert.ParseLong(result.ToString());
      if (pid != long.MinValue && pid != this.consigneePid)
      {
        WindowUtinity.ShowMessageError("WRN0020");
        return false;
      }
      return true;
    }
    /// <summary>
    /// Check logic
    /// </summary>
    private bool CheckInvalid()
    {
      string value = txtConsigneeCode.Text.Trim();
      if (value.Length == 0)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Consignee Code" });
        txtConsigneeCode.Focus();
        return false;
      }
      //if (!this.CheckSameCode()) {
      //  txtConsigneeCode.Focus();
      //  return false;
      //}
      value = txtConsigneeName.Text.Trim();
      if (value.Length == 0)
      {
        txtConsigneeName.Focus();
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Consignee Name" });
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save data on screen in database
    /// </summary>
    /// <returns>true : save success; false : save unsuccess</returns>
    private bool SaveData()
    {
      bool result = false;
      if (!this.CheckInvalid()) {
        return false;
      }
      CSDConsignee consignee = new CSDConsignee();
      if (this.consigneePid != long.MinValue)
      {
        consignee.Pid = this.consigneePid;
        consignee = (CSDConsignee)DataBaseAccess.LoadObject(consignee, new string[] { "Pid" });
        if (consignee == null)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          this.CloseTab();
          return false;
        }
      }
      consignee.ConsigneeCode = txtConsigneeCode.Text.Trim();
      consignee.Name = txtConsigneeName.Text.Trim();
      consignee.Country = DBConvert.ParseLong(ControlUtility.GetSelectedValueCombobox(cmbCountry));
      consignee.PostalCode = txtPostalCode.Text.Trim();
      consignee.Region = txtRegion.Text.Trim();
      consignee.City = txtCity.Text.Trim();
      consignee.StreetAdress = txtStreetAddress.Text.Trim();
      consignee.POBox = txtPOBox.Text.Trim();
      consignee.Tel = txtPhone.Text.Trim();
      consignee.Fax = txtFax.Text.Trim();
      consignee.Email = txtEmail.Text.Trim();
      consignee.Contact = txtContact.Text.Trim();
      consignee.BillOfLading = DBConvert.ParseInt(ControlUtility.GetSelectedValueCombobox(cmbBillOfLading));
      consignee.Fumigation = (radFumigationYes.Checked) ? 1 : 0;
      consignee.Certificate = DBConvert.ParseInt(ControlUtility.GetSelectedValueCombobox(cmbCertification));
      consignee.PackingList = DBConvert.ParseInt(ControlUtility.GetSelectedValueCombobox(cmbParking));
      consignee.Invoice = DBConvert.ParseInt(ControlUtility.GetSelectedValueCombobox(cmbInvoice));
      consignee.DoorDelivery = txtDoorDelivery.Text.Trim();
      consignee.PortOfDischarge = txtPOD.Text.Trim();

      if (consignee.Pid != long.MinValue)
      {
        consignee.UpdateBy = SharedObject.UserInfo.UserPid;
        consignee.UpdateDate = DateTime.Now;
        result = DataBaseAccess.UpdateObject(consignee, new string[] { "Pid" });
      }
      else
      {
        consignee.CreateBy = SharedObject.UserInfo.UserPid;
        consignee.CreateDate = DateTime.Now;
        long pid = DataBaseAccess.InsertObject(consignee);
        consignee.Pid = pid;
        result = (pid != long.MinValue);
      }
      if (!result)
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return false;
      }
      this.consigneePid = consignee.Pid;      
      WindowUtinity.ShowMessageSuccess("MSG0004");
      return true;
    }

    /// <summary> 
    /// Save data before close
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveSuccess = this.SaveData();      
    }
    #endregion Check & Save Data

    #region Events
    /// <summary>
    /// Description : 
    ///   1/ Insert new record or update one record TblCSDConsignee in database.
    ///   2/ Reset screen for save register new  TblCSDConsignee
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveAndContinue_Click(object sender, EventArgs e)
    {
      bool sucess = this.SaveData();
      if (sucess)
      {
        this.Clear();
      }
    }

    /// <summary>
    /// Save data on screen in database
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
      this.LoadData();
    }

    /// <summary>
    /// Confirm Save and Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Set flg NeedToSave = btnSave.Visible OR btnSaveAndContinue.Visible when another object change value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Obj_TextChanged(object sender, EventArgs e)
    {
      this.NeedToSave = (btnSave.Visible) || (btnSaveAndContinue.Visible);
    }

    private void ConsigneeCode_Leave(object sender, EventArgs e)
    {
      //this.CheckSameCode();      
    }

    private void ConsigneeName_Leave(object sender, EventArgs e)
    {
      //string name = txtConsigneeName.Text.Trim();
      //if (name.Length == 0)
      //{
      //  return;
      //}
      //string commandText = "SELECT Pid FROM TblCSDConsignee WHERE Name = @Name";
      //DBParameter[] input = new DBParameter[] { new DBParameter("@Name", DbType.AnsiString, 128, name) };
      //object result = DataBaseAccess.ExecuteScalarCommandText(commandText, input);
      //if (result == null)
      //{
      //  return;
      //}
      //long pid = DBConvert.ParseLong(result.ToString());
      //if (pid != long.MinValue && pid != this.consigneePid)
      //{
      //  WindowUtinity.ShowMessageWarning("WRN0019");
      //}
    }
    #endregion Events
  }
}
