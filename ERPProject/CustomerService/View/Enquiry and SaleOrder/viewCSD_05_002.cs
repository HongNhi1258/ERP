/*
 *  Author    : Nguyen Chi Cuong
 *  Date      : 21/07/2015
 *  Description : Add Col Return 
*/
using DaiCo.Application;
using DaiCo.Application.Web.Mail;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.CustomerService;
using DaiCo.Shared.ReportTemplate.CustomerService;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_05_002 : MainUserControl
  {
    #region Field
    public long enquiryPid = long.MinValue;
    DataTable dtSource = new DataTable();
    private IList listDeletedPid = new ArrayList();
    private bool canUpdate = false;
    private bool isCancel = true;
    private bool isFinish = true;
    private string strType = string.Empty;
    public int status = int.MinValue;
    private string pathTemplate = string.Empty;
    private string pathExport = string.Empty;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init
    /// <summary>
    /// Init Form
    /// </summary>
    public viewCSD_05_002()
    {
      InitializeComponent();
      udtENDate.Value = DateTime.Now;
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_05_002_Load(object sender, EventArgs e)
    {
      if (this.enquiryPid != long.MinValue)
      {
        string commandText = string.Format("SELECT Status FROM TblPLNEnquiry WHERE Pid = {0}", this.enquiryPid);
        DataTable dtLoad = DataBaseAccess.SearchCommandTextDataTable(commandText);
        this.status = DBConvert.ParseInt(dtLoad.Rows[0][0].ToString());
      }
      if (enquiryPid == long.MinValue)
      {
        txtCustomerEnquiryNo.Text = "Customer Enquiry Only";
      }
      chkLoadCust.Checked = false;
      chkLoadCust.Visible = false;
      txtDCus.Visible = false;
      ucbCusGroup.Visible = false;
      lblDCus.Visible = false;
      chkLoaddata = true;
      this.LoadDropdownList();
      this.LoadData();
      chkLoaddata = false;
      this.NeedToSave = false;
    }

    /// <summary>
    /// Override function SaveAndClose
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      object sender = new object();
      EventArgs e = new EventArgs();
      btnSave_Click(sender, e);
    }
    #endregion Init

    #region Load Data
    bool chkLoaddata;
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      //btnPrint.Enabled = false;
      btnGetTemplate.Enabled = true;
      btnImport.Enabled = true;
      btnOpenDialog.Enabled = true;
      if (this.enquiryPid != long.MinValue)
      {
        PLNEnquiry enquiry = new PLNEnquiry();
        enquiry.Pid = this.enquiryPid;
        enquiry = (PLNEnquiry)Shared.DataBaseUtility.DataBaseAccess.LoadObject(enquiry, new string[] { "Pid" });
        if (enquiry == null)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0007");
          this.ConfirmToCloseTab();
          return;
        }
        txtEnquiryNo.Text = enquiry.EnquiryNo;
        try
        {
          ucbCustomer.Value = enquiry.CustomerPid;
        }
        catch { }
        try
        {
          ucbCusGroup.Value = enquiry.DirectPid;
        }
        catch { }
        txtCustomerEnquiryNo.Text = enquiry.CustomerEnquiryNo;
        udtENDate.Value = enquiry.OrderDate;
        try
        {
          ucbType.Value = enquiry.Type.ToString();
          strType = ucbType.Value.ToString();
        }
        catch { }
        chkCancel.Checked = (enquiry.CancelFlag == 0 ? false : true);
        chkKeep.Checked = (enquiry.Keep == 0 ? false : true);
        tableENExpireDays.Visible = chkKeep.Checked;
        if (enquiry.KeepDays > 0)
        {
          txtEnquiryExpireDays.Text = enquiry.KeepDays.ToString();
        }
        this.status = enquiry.Status;
        chkConfirm.Checked = ((enquiry.Status == 1) || (enquiry.Status == 3)) ? true : false;
        txtRemark.Text = enquiry.Remark.Trim();
        //Hung them REFNo PODate RequiredShipDate ContractNo,DeliveryRequirement,PackingRequirement,DocumentRequirement
        txtRef.Text = enquiry.REFNo.Trim();
        if (enquiry.PODate != DateTime.MinValue)
          udtPoDate.Value = enquiry.PODate;
        if (enquiry.RequiredShipDate != DateTime.MinValue)
          udtRequiredShipDate.Value = enquiry.RequiredShipDate;
        txtContract.Text = enquiry.ContractNo;
        txtDeliveryRequirement.Text = enquiry.DeliveryRequirement;
        txtPackingRequirement.Text = enquiry.PackingRequirement;
        txtDocumentRequirement.Text = enquiry.DocumentRequirement;
        txtShipmentTerms.Text = enquiry.ShipmentTerms;
        txtPaymentTerms.Text = enquiry.PaymentTerms;
        txtEnvironmentStatus.Text = enquiry.EnvironmentStatus;
        //btnSave.Enabled = (enquiry.Status == 3) ? false : true;
        btnReturn.Visible = (enquiry.Status == 2) ? true : false;

        //btnPrint.Enabled = (enquiry.Status == 1 || enquiry.Status == 2 || enquiry.Status == 3) ? true : false;
        chkConfirm.Enabled = (enquiry.Status == 1 || enquiry.Status == 3) ? false : true;
        btnSave.Text = "Save";
        btnFinish.Enabled = false;
        if (enquiry.Status > 0)
        {
          chkGetNewPrice.Enabled = false;
          ucbCustomer.ReadOnly = true;
          ucbCusGroup.ReadOnly = true;
          chkLoadCust.Enabled = false;
          btnGetTemplate.Enabled = false;
          btnImport.Enabled = false;
          btnOpenDialog.Enabled = false;
          //txtCustomerEnquiryNo.Enabled = false;
          udtENDate.ReadOnly = true;
          ucbType.ReadOnly = true;
          chkCancel.Visible = true;
          chkKeep.Visible = true;
          chkConfirm.Enabled = false;
          // Truong Add
          btnSentEmail.Enabled = true;
          lbErrorSaleCode.Visible = false;
          txtErrorSaleCode.Visible = false;
          // End
          if (enquiry.Status == 2)
          {
            chkConfirm.Checked = true;
            chkConfirm.Visible = false;
            //btnSave.Text = "Finish";
            btnFinish.Enabled = true;
            btnFinish.Visible = true;
          }
          if (enquiry.Status == 3)
          {
            //chkCancel.Enabled = false;
            //chkKeep.Enabled = false;

            txtRemark.Enabled = false;
          }
        }
        if (chkCancel.Checked)
        {
          txtRemark.Enabled = false;
          chkKeep.Enabled = false;
          chkCancel.Enabled = false;
          ultEnquiryDetail.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
          btnReturn.Enabled = false;
          btnSave.Enabled = false;
          chkConfirm.Enabled = false;
        }
      }
      else
      {
        txtEnquiryNo.Text = Shared.Utility.FunctionUtility.GetNewEnquiryNo("EN");
        chkCancel.Visible = false;
        chkKeep.Visible = false;
        tableENExpireDays.Visible = chkKeep.Visible;
        btnReturn.Visible = false;
      }
      this.canUpdate = ((chkConfirm.Checked == false) && (btnSave.Enabled));
      this.LoadGrid();
    }

    /// <summary>
    /// Check Ship Date
    /// </summary>
    /// <returns></returns>
    private bool CheckShipDate()
    {
      bool chkValid = true;
      int numberExpire = 0;
      string commandText = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 1003 AND Code = 2";
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      numberExpire = (obj != null) ? DBConvert.ParseInt(obj.ToString()) : 0;

      for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultEnquiryDetail.Rows[i];
        DateTime requestDate = DateTime.MinValue;
        if (row.Cells["RequestDate"].Value.ToString().Trim().Length > 0)
        {
          requestDate = DBConvert.ParseDateTime(row.Cells["RequestDate"].Value.ToString(), formatConvert);
        }
        if (requestDate != DateTime.MinValue)
        {
          for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow childRow = row.ChildBands[0].Rows[j];
            DateTime scheduleDate = DateTime.MinValue;
            if (childRow.Cells["ScheduleDate"].Value.ToString().Trim().Length > 0)
            {
              scheduleDate = DBConvert.ParseDateTime(childRow.Cells["ScheduleDate"].Value.ToString(), formatConvert);
            }
            int expire = DBConvert.ParseInt(childRow.Cells["Expire"].Value.ToString());
            if (expire == 0)
            {
              if (scheduleDate.AddDays(numberExpire) != requestDate)
              {
                row.CellAppearance.BackColor = Color.Yellow;
                //row.Cells["RequestDate"].Appearance.BackColor = Color.Yellow;
                childRow.Cells["ScheduleDate"].Appearance.BackColor = Color.Yellow;
                //chkValid = false;
              }
              else
              {
                childRow.Cells["ScheduleDate"].Appearance.BackColor = Color.White;
              }
            }
          }
        }
      }
      return chkValid;
    }

    private void LoadPrice()
    {
      DBParameter[] inputParam = new DBParameter[1];
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      inputParam[0] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      //inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultdpItemCode.SelectedRow.Cells["ItemCode"].Value.ToString());
      DataTable dtValue = DataBaseAccess.SearchStoreProcedureDataTable("spCSDGetItemPriceBaseOnCustomer", inputParam);
      if (dtValue != null && dtValue.Rows.Count > 0)
      {
        if (ultEnquiryDetail.Rows.Count > 0)
        {
          for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
          {
            //ultEnquiryDetail.Rows[i].Cells["Price"].Value = DBNull.Value;
            //ultEnquiryDetail.Rows[i].Cells["SecondPrice"].Value = DBNull.Value;
            for (int j = 0; j < dtValue.Rows.Count; j++)
            {
              if (ultEnquiryDetail.Rows[i].Cells["ItemCode"].Value.ToString() == dtValue.Rows[j]["ItemCode"].ToString())
              {
                if (btnImport.Enabled == false)
                {
                  ultEnquiryDetail.Rows[i].Cells["Price"].Value = DBNull.Value;
                  ultEnquiryDetail.Rows[i].Cells["SecondPrice"].Value = DBNull.Value;
                }
                try
                {
                  if (DBConvert.ParseDouble(ultEnquiryDetail.Rows[i].Cells["Price"].Value.ToString()) == double.MinValue)
                  {
                    ultEnquiryDetail.Rows[i].Cells["Price"].Value = dtValue.Rows[j]["SPIL_Price"];
                  }
                  if (DBConvert.ParseDouble(ultEnquiryDetail.Rows[i].Cells["SecondPrice"].Value.ToString()) == double.MinValue)
                  {
                    ultEnquiryDetail.Rows[i].Cells["SecondPrice"].Value = dtValue.Rows[j]["VFR_Price"];
                  }
                }
                catch
                {
                  ultEnquiryDetail.Rows[i].Cells["Price"].Value = DBNull.Value;
                  ultEnquiryDetail.Rows[i].Cells["SecondPrice"].Value = DBNull.Value;
                }
                break;
              }
            }
          }
        }
      }
      else
      {
        if (btnImport.Enabled == false)
        {
          for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
          {
            ultEnquiryDetail.Rows[i].Cells["Price"].Value = DBNull.Value;
            ultEnquiryDetail.Rows[i].Cells["SecondPrice"].Value = DBNull.Value;
          }
        }
      }
    }

    /// <summary>
    /// Load Grid Detail For Enquiry
    /// </summary>
    private void LoadGrid()
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@EnPid", DbType.Int64, this.enquiryPid) };
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCUSListEnquiryDetailByEnPidDemo", inputParam);

      DataSet dsData = Shared.Utility.CreateDataSet.EnquiryConfirm();
      dsData.Tables.Add(dsSource.Tables[0].Clone());
      dsData.Tables["dtParent"].Merge(dsSource.Tables[0]);
      dsData.Tables.Add(dsSource.Tables[1].Clone());
      dsData.Tables["dtChild"].Merge(dsSource.Tables[1]);
      dsData.Tables["dtParent"].Columns["Revision"].AllowDBNull = false;
      ultEnquiryDetail.DataSource = dsData;

      DataRow[] row = dsSource.Tables[1].Select("isGotoSO = 1");
      if (row.Length > 0)
      {
        chkCancel.Enabled = false;
      }

      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["Pid"].Hidden = true;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["EnquiryDetailPid"].Hidden = true;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["Expire"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ultEnquiryDetail.DisplayLayout.Bands[0].Columns["EnquirySale"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["Keep"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["NonPlan"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["ScheduleDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["ScheduleDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["ScheduleDate"].Header.Caption = "Confirmed\nShip Date";
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["ScheduleDate"].CellActivation = Activation.ActivateOnly;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["NonPlan"].CellActivation = Activation.ActivateOnly;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      ultEnquiryDetail.DisplayLayout.Bands[0].Columns["No"].CellActivation = Activation.ActivateOnly;
      ultEnquiryDetail.DisplayLayout.Bands[0].Columns["EnquirySale"].CellActivation = Activation.ActivateOnly;
      ultEnquiryDetail.DisplayLayout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      ultEnquiryDetail.DisplayLayout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      this.CheckShipDate();

      for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
      {
        try
        {
          if (DBConvert.ParseDouble(ultEnquiryDetail.Rows[i].Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(ultEnquiryDetail.Rows[i].Cells["Price"].Value.ToString()) > 0)
          {
            ultEnquiryDetail.Rows[i].Cells["Amount"].Value = DBConvert.ParseDouble(ultEnquiryDetail.Rows[i].Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(ultEnquiryDetail.Rows[i].Cells["Price"].Value.ToString());
          }
          else
          {
            ultEnquiryDetail.Rows[i].Cells["Amount"].Value = DBNull.Value;
          }
        }
        catch
        {
          ultEnquiryDetail.Rows[i].Cells["Amount"].Value = DBNull.Value;
        }
        if (ultEnquiryDetail.Rows[i].Cells["IsGreaterMOQ"].Value.ToString() == "1")
        {
          ultEnquiryDetail.Rows[i].Appearance.BackColor = Color.Yellow;
        }
        //if (ultEnquiryDetail.Rows[i].Cells["AllConfirm"].Value.ToString() == "0")
        //{
        //  ultEnquiryDetail.Rows[i].CellAppearance.BackColor = Color.Lime;
        //}

        for (int j = 0; j < ultEnquiryDetail.Rows[i].ChildBands[0].Rows.Count; j++)
        {

          if (ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["isGotoSO"].Value.ToString() == "1")
          {
            ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["Keep"].Activation = Activation.NoEdit;
            ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["Expire"].Activation = Activation.NoEdit;

          }
          else
          {
            ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["Keep"].Activation = Activation.AllowEdit;
            ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["Expire"].Activation = Activation.AllowEdit;
          }
          ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["KeepDays"].Activation = Activation.AllowEdit;
        }
      }

      //if (this.status == 2)
      //{
      //  for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
      //  {
      //    ultEnquiryDetail.Rows[i].Cells["SaleCode"].Activation = Activation.ActivateOnly;
      //    for (int j = 0; j < ultEnquiryDetail.Rows[i].ChildBands[0].Rows.Count; j++)
      //    {
      //      if (ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["isGotoSO"].Value.ToString() == "1")
      //      {
      //        ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["Keep"].Activation = Activation.NoEdit;
      //        ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["Expire"].Activation = Activation.NoEdit;

      //      }
      //      else
      //      {
      //        ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["Keep"].Activation = Activation.AllowEdit;
      //        ultEnquiryDetail.Rows[i].ChildBands[0].Rows[j].Cells["Expire"].Activation = Activation.AllowEdit;
      //      }
      //    }
      //  }
      //}
    }

    /// <summary>
    /// Load DropDown Sale Code
    /// </summary>
    /// <param name="ultra"></param>
    private void LoadDropDownSaleCode(UltraDropDown ultra)
    {
      string commandText = @"SELECT DISTINCT BS.SaleCode, BS.ItemCode, BS.Name , BS.RevisionActive
                                            , CASE WHEN CDS.SaleCode IS NULL THEN 1 ELSE CDS.IsConfirm END IsConfirm
                             FROM TblBOMItemBasic BS 
                                INNER JOIN TblBOMItemInfo INF ON (BS.ItemCode = INF.ItemCode)
                                LEFT JOIN VCSDSaleCodeCD CDS  ON (BS.SaleCode = CDS.SaleCode)
                             WHERE LEN(BS.SaleCode) > 0 ORDER BY BS.SaleCode DESC";
      ultra.DataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultra.ValueMember = "SaleCode";
      ultra.DisplayMember = "SaleCode";
      ultra.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultra.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultra.DisplayLayout.Bands[0].Columns["ItemCode"].Width = 150;
    }

    /// <summary>
    /// Load Dropdown Item Code
    /// </summary>
    /// <param name="udrpItemCode"></param>
    private void LoadDropdownItemCode(UltraDropDown udrpItemCode)
    {
      string commandText = string.Format(@"SELECT DISTINCT ItemCode, Name, NameVN
                                           FROM TblBOMItemBasic ORDER BY ItemCode DESC");
      udrpItemCode.DataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpItemCode.ValueMember = "ItemCode";
      udrpItemCode.DisplayMember = "ItemCode";
      udrpItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      udrpItemCode.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      udrpItemCode.DisplayLayout.Bands[0].Columns["NameVN"].Hidden = true;
      udrpItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Width = 150;
    }

    /// <summary>
    /// Load Dropdown Item Code Confirm
    /// </summary>
    /// <param name="udrpItemCode"></param>
    private void LoadDropdownItemCodeConfirm(UltraDropDown udrpItemCode)
    {
      string commandText = string.Format(@"SELECT DISTINCT INF.ItemCode
                                           FROM TblBOMItemInfo INF INNER JOIN TblBOMItemBasic BS ON (INF.ItemCode = BS.ItemCode)
                                           ORDER BY INF.ItemCode DESC");
      udrpItemCode.DataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpItemCode.ValueMember = "ItemCode";
      udrpItemCode.DisplayMember = "ItemCode";
      udrpItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      udrpItemCode.DisplayLayout.Bands[0].Columns["ItemCode"].Width = 150;
    }

    private void GetNewPrice()
    {
      // 1: Check Data
      // 1.1: Check Customer
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid <= 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Customer");
        return;
      }
      string stringItemQty = string.Empty;

      // 1.1: Check Item - Qty
      for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultEnquiryDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()) <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Qty");
          return;
        }
        stringItemQty += row.Cells["ItemCode"].Value.ToString() + "^" + row.Cells["Qty"].Value.ToString() + "~";

      }

      DBParameter[] input = new DBParameter[3];
      input[0] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      if (this.enquiryPid > 0)
      {
        input[1] = new DBParameter("@EnqiryPid", DbType.Int64, this.enquiryPid);
      }
      input[2] = new DBParameter("@StringItemQty", DbType.String, stringItemQty);
      DataTable dtItemPrice = DataBaseAccess.SearchStoreProcedureDataTable("spCSDGetPriceWhenCreateEnquiry", 600, input);
      if (dtItemPrice != null)
      {
        for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
        {
          UltraGridRow ultRow = ultEnquiryDetail.Rows[i];
          ultRow.Cells["FlagUpdate"].Value = 1;

          for (int j = 0; j < dtItemPrice.Rows.Count; j++)
          {
            DataRow row = dtItemPrice.Rows[j];
            if (ultRow.Cells["ItemCode"].Value.ToString() == row["ItemCode"].ToString() &&
                DBConvert.ParseInt(ultRow.Cells["Qty"].Value.ToString()) == DBConvert.ParseInt(row["Qty"].ToString()))
            {
              ultRow.Cells["SecondPrice"].Value = DBConvert.ParseDouble(row["Price"].ToString());
            }
          }
        }
      }

      //// 2: Get Item Price
      //for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
      //{
      //  UltraGridRow row = ultEnquiryDetail.Rows[i];
      //  DBParameter[] input = new DBParameter[3];
      //  input[0] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      //  input[1] = new DBParameter("@ItemCode", DbType.String, row.Cells["ItemCode"].Value.ToString());
      //  input[2] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()));
      //  DataTable dtItemPrice = DataBaseAccess.SearchStoreProcedureDataTable("spCSDGetPriceWhenCreateEnquiry", 600, input);
      //  if(dtItemPrice != null && dtItemPrice.Rows.Count > 0)
      //  {
      //    row.Cells["SecondPrice"].Value = DBConvert.ParseDouble(dtItemPrice.Rows[0]["Price"].ToString());
      //  }
      //}
      WindowUtinity.ShowMessageSuccess("MSG0001");
    }

    /// <summary>
    /// Load Revision
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="ultRevision"></param>
    /// <returns></returns>
    private UltraDropDown LoadRevision(string itemCode, UltraDropDown ultRevision)
    {
      if (ultRevision == null)
      {
        ultRevision = new UltraDropDown();
        this.Controls.Add(ultRevision);
      }
      string commandText = string.Format(@"SELECT DISTINCT Revision FROM TblBOMItemInfo WHERE ItemCode = '{0}' ORDER BY Revision ASC", itemCode);
      DataTable dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultRevision.DataSource = dt;
      ultRevision.ValueMember = "Revision";
      ultRevision.DisplayMember = "Revision";
      ultRevision.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultRevision.Visible = false;
      return ultRevision;
    }

    /// <summary>
    /// Load DropdownList
    /// </summary>
    private void LoadDropdownList()
    {
      chkLoaddata = true;
      this.LoadCustomer();
      chkLoaddata = false;

      Utility.LoadUltraComboCodeMst(ucbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      //LoadDropdownItemCode(ultdpItemCode);
      LoadDropDownSaleCode(ultdpItemCode);
      LoadDropdownItemCodeConfirm(ultItemCodeConfirm);
    }

    private void LoadCustomer()
    {
      string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE DeletedFlg = 0 AND ParentPid IS NULL ORDER BY CustomerCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ucbCustomer, dtSource, "Pid", "Customer", "Pid");
    }
    /// <summary>
    /// Checking Non CBM
    /// </summary>
    private bool CheckCBM()
    {
      bool chkresult = false;
      string strparam = "";
      if (ultEnquiryDetail.Rows.Count > 0)
      {
        for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
        {
          strparam += "&" + ultEnquiryDetail.Rows[i].Cells["ItemCode"].Value.ToString() + "-" + ultEnquiryDetail.Rows[i].Cells["Revision"].Value.ToString();
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
            for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
            {
              ultEnquiryDetail.Rows[i].CellAppearance.BackColor = Color.White;
              for (int j = 0; j < dtNonCBM.Rows.Count; j++)
              {
                if (ultEnquiryDetail.Rows[i].Cells["ItemCode"].Value.ToString() + "-" + ultEnquiryDetail.Rows[i].Cells["Revision"].Value.ToString() == dtNonCBM.Rows[j][0].ToString())
                {
                  ultEnquiryDetail.Rows[i].CellAppearance.BackColor = Color.Pink;
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

    #endregion Load Data

    #region Check & Save Data

    private bool CheckDiscontinue(string itemCode)
    {
      //return false;
      DataTable dt = new DataTable();
      string str = "";
      str = @"SELECT ItemCode  FROM TblCSDItemInfo WHERE DiscontinueFlag = 1            
              AND ItemCode ='" + itemCode + "'";

      dt = DataBaseAccess.SearchCommandTextDataTable(str, 900);
      if (dt != null && dt.Rows.Count > 0)
      {
        return true;
      }
      return false;
    }
    /// <summary>
    /// Check Input Data
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckIsValid(out string message)
    {
      message = string.Empty;
      // Type :
      int type = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucbType));
      if (type == int.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Type");
        return false;
      }

      if (chkLoadCust.Checked && ucbCusGroup.Value == null)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Direct Customer");
        return false;
      }
      // Customer :
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid == long.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Customer");
        return false;
      }
      // Enquiry Date :
      object obj = udtENDate.Value;
      if ((obj == null) || (obj != null && (DateTime)obj == DateTime.MinValue))
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "EN Date");
        return false;
      }

      // Kiem tra so ngay keep neu co keep
      if (tableENExpireDays.Visible && DBConvert.ParseInt(txtEnquiryExpireDays.Text) <= 0)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Keep Days");
        return false;
      }

      //Trọn update: Kiểm tra phần detail trước khi save
      DataSet ds = (DataSet)ultEnquiryDetail.DataSource;
      DataTable dt = ds.Tables["dtParent"];
      foreach (DataRow row in dt.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          string itemCode = row["ItemCode"].ToString().Trim();
          if (itemCode.Length == 0)
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "ItemCode");
            return false;
          }
          if (status <= 0 && this.CheckDiscontinue(DBConvert.ParseString(row["ItemCode"].ToString().Trim())))
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0065"), "Itemcode: " + row["ItemCode"].ToString());
            return false;
          }

          int revision = DBConvert.ParseInt(row["Revision"].ToString());
          if (revision < 0)
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Revision");
            return false;
          }

          int qty = DBConvert.ParseInt(row["Qty"].ToString());
          if (qty <= 0)
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Qty");
            return false;
          }

          double price = DBConvert.ParseDouble(row["Price"].ToString());
          if (price < 0)
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Price");
            return false;
          }
        }
      }

      // Kiem tra phan keepDays neu co keep
      DataTable dtKeepDays = ds.Tables["dtChild"];
      foreach (DataRow row in dtKeepDays.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          int keep = DBConvert.ParseInt(row["Keep"].ToString());
          int keepDays = DBConvert.ParseInt(row["KeepDays"].ToString());
          if ((keep == 1) && (keepDays <= 0))
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Keep Days");
            return false;
          }
        }
      }

      //End kiem tra detail
      if (!chkCancel.Checked && !chkKeep.Checked)
      {
        int keep = 0;
        if (this.enquiryPid != null)
        {
          string commandTextKeep = string.Format("SELECT Keep FROM TblPLNEnquiry WHERE Pid = {0}", this.enquiryPid);
          keep = (DataBaseAccess.ExecuteScalarCommandText(commandTextKeep) != null) ? (int)DataBaseAccess.ExecuteScalarCommandText(commandTextKeep) : 0;
        }
        if (keep == 0)
        {
          bool result = this.CheckShipDate();
          if (!result)
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Confirm Ship Date");
            return false;
          }
        }
      }
      //check all confirm

      string cm = string.Format("select Pid from TblPLNEnquiry where Status = 0 And Pid ='{0}'", this.enquiryPid);
      DataTable dtConfirm = DataBaseAccess.SearchCommandTextDataTable(cm);
      if (dtConfirm.Rows.Count > 0 && dtConfirm != null)
      {
        for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
        {
          UltraGridRow rowParent = ultEnquiryDetail.Rows[i];
          int AllConfirm = DBConvert.ParseInt(rowParent.Cells["AllConfirm"].Value.ToString());
          if (AllConfirm == 0 && chkConfirm.Checked)
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "AllComfirm");
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Save Detail
    /// </summary>
    /// <param name="row"></param>
    /// <param name="parentPid"></param>
    /// <returns></returns>
    private bool SaveDetail(DataRow row, long parentPid)
    {
      DBParameter[] inputParam = new DBParameter[15];
      string storeName = string.Empty;
      long pid = DBConvert.ParseLong(row["Pid"].ToString());
      string itemCode = row["ItemCode"].ToString().Trim();
      int revision = DBConvert.ParseInt(row["Revision"].ToString());
      int qty = DBConvert.ParseInt(row["Qty"].ToString());
      DateTime scheduleDelivery = DBConvert.ParseDateTime(row["RequestDate"].ToString(), formatConvert);
      string remark = row["Remark"].ToString().Trim();
      string RemarkForAccount = row["RemarkForAccount"].ToString().Trim();

      //Hung them RequiredShipDate	SpecialInstruction
      DateTime RequiredShipDate = DBConvert.ParseDateTime(row["RequiredShipDate"].ToString(), formatConvert);
      string SpecialInstruction = row["SpecialInstruction"].ToString();
      string specialDes = string.Empty;
      if (strType != "2")
      {
        specialDes = String.Empty;
      }
      else
      {
        specialDes = row["SpecialDescription"].ToString().Trim();
      }
      double dPrice = DBConvert.ParseDouble(row["Price"].ToString().Trim());
      double dSPrice = DBConvert.ParseDouble(row["SecondPrice"].ToString().Trim());

      //Add Col Return
      int dreturn = DBConvert.ParseInt(row["Return"].ToString().Trim());

      if (pid != long.MinValue)
      {
        inputParam[9] = new DBParameter("@UpdateByCSS", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        inputParam[10] = new DBParameter("@Pid", DbType.Int64, pid);
        storeName = "spCUSEnquiryDetail_Update";
      }
      else
      {
        inputParam[9] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = "spCUSEnquiryDetail_Insert";
      }
      inputParam[0] = new DBParameter("@EnquiryPid", DbType.Int64, parentPid);
      inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
      inputParam[2] = new DBParameter("@Revision", DbType.Int32, revision);
      inputParam[3] = new DBParameter("@Qty", DbType.Int32, qty);
      if (scheduleDelivery != DateTime.MinValue)
      {
        inputParam[4] = new DBParameter("@RequestDate", DbType.DateTime, scheduleDelivery);
      }
      if (RequiredShipDate != DateTime.MinValue)
      {
        inputParam[11] = new DBParameter("@RequiredShipDate", DbType.DateTime, RequiredShipDate);
      }

      inputParam[12] = new DBParameter("@SpecialInstruction", DbType.AnsiString, 4000, SpecialInstruction);
      inputParam[13] = new DBParameter("@RemarkForAccount", DbType.String, 4000, RemarkForAccount);
      inputParam[5] = new DBParameter("@Remark", DbType.AnsiString, 4000, remark);
      if (specialDes.Length > 0)
      {
        inputParam[6] = new DBParameter("@SpecialDescription", DbType.AnsiString, 4000, specialDes);
      }
      if (dPrice > 0)
      {
        inputParam[7] = new DBParameter("@Price", DbType.Double, dPrice);
      }
      if (dSPrice > 0)
      {
        inputParam[8] = new DBParameter("@SecondPrice", DbType.Double, dSPrice);
      }

      inputParam[14] = new DBParameter("@Return", DbType.Double, dreturn);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

      long result = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    private bool SaveData(out long pid, bool isbtnFinshed)
    {
      DataSet ds = (DataSet)ultEnquiryDetail.DataSource;
      this.dtSource = ds.Tables["dtParent"];

      string text = txtCustomerEnquiryNo.Text.Trim();
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      object obj = udtENDate.Value;
      DateTime enquiryDate = (obj != null) ? (DateTime)obj : DateTime.MinValue;
      int type = DBConvert.ParseInt(Utility.GetSelectedValueUltraCombobox(ucbType));
      int cancelFlag = (chkCancel.Checked) ? 1 : 0;
      if (cancelFlag == 1)
      {
        int count = 0;
        string commandTextCheckEN = "SELECT dbo.FCSDCheckEnquiryOpenSaleOrder(@EnquiryNo)";
        DBParameter[] param = new DBParameter[] { new DBParameter("@EnquiryNo", DbType.AnsiString, 16, txtEnquiryNo.Text.Trim()) };
        object objEN = DataBaseAccess.ExecuteScalarCommandText(commandTextCheckEN, param);

        count = (objEN != null) ? DBConvert.ParseInt(objEN.ToString()) : 0;
        if (count > 0)
        {
          this.isCancel = false;
          pid = long.MinValue;
          return false;
        }
      }
      int keep = (chkKeep.Checked) ? 1 : 0;
      string remark = txtRemark.Text.Trim();
      //Hung them REFNo PODate RequiredShipDate ContractNo,DeliveryRequirement,PackingRequirement,DocumentRequirement
      string REFNo = txtRef.Text.Trim();
      DateTime PODate = new DateTime();
      if (udtPoDate.Value != null)
      {
        PODate = DBConvert.ParseDateTime(udtPoDate.Value.ToString(), formatConvert);
      }
      DateTime requiredShipDate = new DateTime();
      if (udtRequiredShipDate.Value != null)
      {
        requiredShipDate = DBConvert.ParseDateTime(udtRequiredShipDate.Value.ToString(), formatConvert);
      }
      string ContractNo = txtContract.Text.Trim();
      string DeliveryRequirement = txtDeliveryRequirement.Text.Trim();
      string PackingRequirement = txtPackingRequirement.Text.Trim();
      string DocumentRequirement = txtDocumentRequirement.Text.Trim();
      string shipmentTerms = txtShipmentTerms.Text.Trim();
      string paymentTerms = txtPaymentTerms.Text.Trim();
      string environmentStatus = txtEnvironmentStatus.Text.Trim();
      long lDirectPid = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ucbCusGroup));

      if (((this.status == int.MinValue) || (this.status == 0)) && (chkConfirm.Checked))
      {
        this.status = 1;
      }
      else if (((this.status == int.MinValue) || (this.status == 0)) && (!chkConfirm.Checked))
      {
        this.status = 0;
      }
      else if (this.status == 2)
      {
        foreach (DataRow rowCheck in this.dtSource.Rows)
        {
          // Qty PLN
          long enDetailPid = DBConvert.ParseLong(rowCheck["Pid"].ToString());
          string commandText = string.Format("SELECT Qty FROM TblPLNEnquiryDetail WHERE Pid = {0}", enDetailPid);
          object objQty = DataBaseAccess.ExecuteScalarCommandText(commandText);
          int qtyPLN = (obj != null) ? DBConvert.ParseInt(objQty) : 0;
          // Qty Expire
          int qtyExpire = 0;
          DataRow[] rowExpire = ds.Tables["dtChild"].Select(string.Format("EnquiryDetailPid = {0} AND Expire = 1", enDetailPid));
          if (rowExpire.Length > 0)
          {
            foreach (DataRow r in rowExpire)
            {
              qtyExpire += DBConvert.ParseInt(r["Qty"].ToString());
            }
          }
          // Check Qty CSD With Qty PLN Not Expire
          int qty = DBConvert.ParseInt(rowCheck["Qty"].ToString());
          if (!chkCancel.Checked && !chkKeep.Checked)
          {
            int keepEN = 0;
            if (this.enquiryPid != null)
            {
              string commandTextKeep = string.Format("SELECT Keep FROM TblPLNEnquiry WHERE Pid = {0}", this.enquiryPid);
              keepEN = (DataBaseAccess.ExecuteScalarCommandText(commandTextKeep) != null) ? (int)DataBaseAccess.ExecuteScalarCommandText(commandTextKeep) : 0;
            }
            if (keepEN == 0)
            {
              if (qty != qtyPLN - qtyExpire)
              {
                if (chkConfirm.Checked)
                {
                  this.isFinish = false;
                  pid = this.enquiryPid;
                  return false;
                }
              }
              else
              {
                if (chkCancel.Checked || chkKeep.Checked)
                {
                  this.status = 2;
                  break;
                }
                if (chkConfirm.Checked && isbtnFinshed == true)
                {
                  this.status = 3;
                }
              }
            }
          }
        }
      }

      DBParameter[] inputParam = new DBParameter[23];
      bool result = true;
      string storeName = string.Empty;
      if (this.enquiryPid != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.enquiryPid);
        inputParam[10] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = "spCUSEnquiry_Update";
        pid = this.enquiryPid;
      }
      else
      {
        inputParam[10] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = "spCUSEnquiry_Insert";
      }

      inputParam[1] = new DBParameter("@CustomerEnquiryNo", DbType.AnsiString, 32, text);
      inputParam[2] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
      inputParam[3] = new DBParameter("@OrderDate", DbType.DateTime, new DateTime(enquiryDate.Year, enquiryDate.Month, enquiryDate.Day));
      inputParam[4] = new DBParameter("@Type", DbType.Int32, type);
      inputParam[5] = new DBParameter("@CancelFlag", DbType.Int32, cancelFlag);
      inputParam[6] = new DBParameter("@Keep", DbType.Int32, keep);
      if (keep == 1)
      {
        inputParam[11] = new DBParameter("@KeepDays", DbType.Int32, DBConvert.ParseInt(txtEnquiryExpireDays.Text));
      }
      inputParam[7] = new DBParameter("@Status", DbType.Int32, this.status);
      if (remark.Length > 0)
      {
        inputParam[8] = new DBParameter("@Remark", DbType.AnsiString, 4000, remark);
      }
      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      if (lDirectPid != long.MinValue && chkLoadCust.Checked)
      {
        inputParam[9] = new DBParameter("@DirectPid", DbType.Int64, lDirectPid);
      }
      if (status == 1 && ultEnquiryDetail.Rows.Count == 0)
      {
        pid = long.MinValue;
        return false;
      }
      //Hung them REFNo PODate RequiredShipDate ContractNo,DeliveryRequirement,PackingRequirement,DocumentRequirement
      inputParam[12] = new DBParameter("@REFNo", DbType.AnsiString, 32, REFNo);
      if (PODate != DateTime.MinValue)
      {
        inputParam[13] = new DBParameter("@PODate", DbType.DateTime, PODate);
      }
      if (requiredShipDate != DateTime.MinValue)
      {
        inputParam[14] = new DBParameter("@RequiredShipDate", DbType.DateTime, requiredShipDate);
      }
      if (ContractNo.Length > 0)
      {
        inputParam[15] = new DBParameter("@ContractNo", DbType.AnsiString, 32, ContractNo);
      }
      if (DeliveryRequirement.Length > 0)
      {
        inputParam[16] = new DBParameter("@DeliveryRequirement", DbType.String, 500, DeliveryRequirement);
      }
      if (PackingRequirement.Length > 0)
      {
        inputParam[17] = new DBParameter("@PackingRequirement", DbType.String, 500, PackingRequirement);
      }
      if (DocumentRequirement.Length > 0)
      {
        inputParam[18] = new DBParameter("@DocumentRequirement", DbType.String, 500, DocumentRequirement);
      }
      if (chkConfirm.Checked && this.chkConfirm.Enabled == true)
      {
        DateTime getdate = DBConvert.ParseDateTime(DateTime.Now.ToString(), formatConvert);
        inputParam[19] = new DBParameter("@CSConfirmDate", DbType.DateTime, getdate);
      }
      if (shipmentTerms.Length > 0)
      {
        inputParam[20] = new DBParameter("@ShipmentTerms", DbType.String, 500, shipmentTerms);
      }
      if (paymentTerms.Length > 0)
      {
        inputParam[21] = new DBParameter("@PaymentTerms", DbType.String, 500, paymentTerms);
      }
      if (environmentStatus.Length > 0)
      {
        inputParam[22] = new DBParameter("@EnvironmentStatus", DbType.String, 500, environmentStatus);
      }
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      pid = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
      if (pid <= 0)
      {
        return false;
      }

      // Send Email
      if (this.status == 1 && this.chkConfirm.Enabled == true)
      {
        Email email = new Email();
        email.Key = email.KEY_CSD_001;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), this.txtEnquiryNo.Text, userName);
          string body = string.Format(arrList[2].ToString(), this.txtEnquiryNo.Text, userName);
          email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }
      }

      //Save Detail
      foreach (DataRow dr in dtSource.Rows)
      {
        if (dr.RowState == DataRowState.Added || dr.RowState == DataRowState.Modified ||
            (dr.RowState != DataRowState.Deleted && DBConvert.ParseInt(dr["FlagUpdate"].ToString()) == 1))
        {
          bool success = this.SaveDetail(dr, pid);
          if (!success)
          {
            result = false;
          }
        }

        //if (dr.RowState == DataRowState.Added || dr.RowState == DataRowState.Modified || DBConvert.ParseInt(dr["FlagUpdate"].ToString()) == 1)
        //{
        //  bool success = this.SaveDetail(dr, pid);
        //  if (!success)
        //  {
        //    result = false;
        //  }
        //}
      }

      DataTable dtCheck = ds.Tables["dtChild"];
      foreach (DataRow dr in dtCheck.Rows)
      {
        if (dr.RowState == DataRowState.Added || dr.RowState == DataRowState.Modified)
        {
          CUSEnquiryConfirmDetail_Update objEnquiryConfirmDetail = new CUSEnquiryConfirmDetail_Update();
          objEnquiryConfirmDetail.Pid = DBConvert.ParseLong(dr["PID"].ToString());
          objEnquiryConfirmDetail.Keep = DBConvert.ParseInt(dr["Keep"].ToString());
          if (objEnquiryConfirmDetail.Keep == 1)
          {
            objEnquiryConfirmDetail.KeepDays = DBConvert.ParseInt(dr["KeepDays"].ToString());
          }
          objEnquiryConfirmDetail.Expire = DBConvert.ParseInt(dr["Expire"].ToString());
          object output1 = Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreObject(objEnquiryConfirmDetail);

          if (output1 == null)
          {
            result = false;
          }
        }
      }

      //Delete TblPLNEnquiryDetail
      foreach (long detailPid in this.listDeletedPid)
      {
        DBParameter[] inputParamDelete = new DBParameter[1];
        inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
        DBParameter[] outputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNEnquiryDetail_Delete", inputParamDelete, outputParamDelete);
        long outputValue = DBConvert.ParseLong(outputParamDelete[0].Value.ToString());
        if (outputValue == 0)
        {
          result = false;
        }
      }
      return result;
    }

    #endregion Check & Save Data

    #region Event

    /// <summary>
    /// btnSave Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.enquiryPid != long.MinValue)
      {
        string commandText = string.Format("SELECT Status FROM TblPLNEnquiry WHERE Pid = {0}", this.enquiryPid);
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        this.status = DBConvert.ParseInt(dt.Rows[0][0].ToString());
      }

      string message = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      long pid = long.MinValue;
      success = this.SaveData(out pid, false);
      if (success)
      {
        this.enquiryPid = pid;
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadData();
      }
      else
      {
        if (!this.isFinish)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0095");
          return;
        }
        if (!this.isCancel)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0094");
          return;
        }
        if (chkConfirm.Checked == true && ultEnquiryDetail.Rows.Count == 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0001");
          chkConfirm.Checked = false;
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        }
      }
      this.NeedToSave = false;
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// ultEnquiryDetail Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultEnquiryDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (e.Cell.Text.ToString().Trim().Length > 0)
      {
        string strColName = e.Cell.Column.ToString();
        if (string.Compare(strColName, "ItemCode", true) == 0)
        {
          //Kiem tra xem ItemCode nhap dung chua
          string commandText = string.Format("Select COUNT(ItemCode) From TblBOMItemInfo Where ItemCode = '{0}' ", e.Cell.Text.ToString().Trim());
          object obj = Shared.DataBaseUtility.DataBaseAccess.ExecuteScalarCommandText(commandText);
          int count = (int)obj;
          if (count == 0)
          {
            strColName = "ItemCode";
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", strColName);
            e.Cancel = true;
          }
        }
        // Kiem tra Sale Code 
        if (string.Compare(strColName, "SaleCode", true) == 0)
        {
          if (e.Cell.Text.Length > 0)
          {
            string commandText = string.Format("Select SaleCode From TblBOMItemBasic Where SaleCode = '{0}' ", e.Cell.Text.ToString().Trim());
            DataTable dtCheck = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);

            if ((dtCheck == null) || (dtCheck != null && dtCheck.Rows.Count == 0))
            {
              Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", "Sale Code");
              e.Cancel = true;
            }
          }
          else
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", "Sale Code");
            e.Cancel = true;
          }
        }
        // Kiem tra Price
        if (string.Compare(strColName, "Price", true) == 0)
        {
          if (e.Cell.Text.ToString().Trim().Length > 0)
          {
            if (DBConvert.ParseDouble(e.Cell.Text.ToString().Trim()) <= 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", "Price");
              e.Cancel = true;
            }
          }
        }
        // Kiem tra Second Price
        if (string.Compare(strColName, "SecondPrice", true) == 0)
        {
          if (e.Cell.Text.ToString().Trim().Length > 0)
          {
            if (DBConvert.ParseDouble(e.NewValue.ToString().Trim()) <= 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", "SecondPrice");
              e.Cancel = true;
            }
          }
        }
      }
      //else
      //{
      //  Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0001", "Sale Code");
      //  e.Cancel = true;
      //}
    }

    /// <summary>
    /// ultEnquiryDetail Before Rows Deleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultEnquiryDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        long detailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (detailPid != long.MinValue)
        {
          this.listDeletedPid.Add(detailPid);
        }
      }
    }

    /// <summary>
    /// ultEnquiryDetail After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultEnquiryDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (e.Cell.Row.ParentRow == null)
      {
        int index = e.Cell.Row.Index;
        string columnName = e.Cell.Column.ToString().ToLower();
        string value = e.Cell.Value.ToString();
        string commandText = string.Empty;
        DataTable dt;
        // Is Update
        e.Cell.Row.Cells["FlagUpdate"].Value = 1;

        switch (columnName)
        {
          case "salecode":
            // Set Value For Cell "ItemCode"
            try
            {
              ultEnquiryDetail.Rows[index].Cells["ItemCode"].Value = ultdpItemCode.SelectedRow.Cells["ItemCode"].Value;
              ultEnquiryDetail.Rows[index].Cells["AllConfirm"].Value = ultdpItemCode.SelectedRow.Cells["IsConfirm"].Value;
              ultEnquiryDetail.Rows[index].Cells["Return"].Value = 0;

              //this.CheckDiscontinue(DBConvert.ParseString(ultdpItemCode.SelectedRow.Cells["ItemCode"].Value));
            }
            catch
            {
              ultEnquiryDetail.Rows[index].Cells["ItemCode"].Value = DBNull.Value;
              ultEnquiryDetail.Rows[index].Cells["AllConfirm"].Value = DBNull.Value;
            }
            // Set Value For Cell "Name"
            try
            {
              ultEnquiryDetail.Rows[index].Cells["Name"].Value = ultdpItemCode.SelectedRow.Cells["Name"].Value;
            }
            catch
            {
              ultEnquiryDetail.Rows[index].Cells["Name"].Value = DBNull.Value;
            }
            // Load Data For UltraDropDown Revision
            string itemCode = e.Cell.Row.Cells["ItemCode"].Value.ToString().Trim();
            UltraDropDown ultRevision = (UltraDropDown)e.Cell.Row.Cells["Revision"].ValueList;
            e.Cell.Row.Cells["Revision"].ValueList = this.LoadRevision(itemCode, ultRevision);

            if (ultdpItemCode.SelectedRow != null)
            {
              if (DBConvert.ParseInt(ultdpItemCode.SelectedRow.Cells["RevisionActive"].Value.ToString()) >= 0)
              {
                e.Cell.Row.Cells["Revision"].Value = ultdpItemCode.SelectedRow.Cells["RevisionActive"].Value;
              }
              else if (DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString()) == int.MinValue)
              {
                e.Cell.Row.Cells["Revision"].Value = e.Cell.Row.Cells["Revision"].ValueList.GetValue(e.Cell.Row.Cells["Revision"].ValueList.ItemCount - 1);
              }
            }

            // Set Value For Cell "Total CBM"
            commandText = string.Empty;
            commandText += "  SELECT IB.SaleCode, ISNULL(II.CBM,0) CBM";
            commandText += "  FROM TblBOMItemBasic IB ";
            commandText += "    INNER JOIN  TblBOMItemInfo II ON (IB.ItemCode = II.ItemCode) ";
            commandText += "  WHERE IB.ItemCode = '" + itemCode + "' AND Revision = " + DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString());

            dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt.Rows.Count == 1)
            {
              ultEnquiryDetail.Rows[index].Cells["CBM"].Value = DBConvert.ParseDouble(dt.Rows[0]["CBM"].ToString());
              if (ultEnquiryDetail.Rows[index].Cells["Qty"].Value.ToString().Length > 0)
              {
                ultEnquiryDetail.Rows[index].Cells["TotalCBM"].Value = DBConvert.ParseInt(ultEnquiryDetail.Rows[index].Cells["Qty"].Value.ToString())
                                                                    * DBConvert.ParseDouble(ultEnquiryDetail.Rows[index].Cells["CBM"].Value.ToString());
              }
            }

            if (ultdpItemCode.SelectedRow != null)
            {
              if (ultdpItemCode.SelectedRow.Cells["ItemCode"].Value != null || ultdpItemCode.SelectedRow.Cells["ItemCode"].Value.ToString().Length > 0)
              {
                DBParameter[] inputParam = new DBParameter[2];
                long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
                inputParam[0] = new DBParameter("@CustomerPid", DbType.Int64, customerPid);
                inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultdpItemCode.SelectedRow.Cells["ItemCode"].Value.ToString());
                DataTable dtValue = DataBaseAccess.SearchStoreProcedureDataTable("spCSDGetItemPriceBaseOnCustomer", inputParam);
                if (dtValue != null && dtValue.Rows.Count > 0)
                {
                  if (DBConvert.ParseDouble(dtValue.Rows[0]["SPIL_Price"].ToString()) == double.MinValue)
                  {
                    ultEnquiryDetail.Rows[index].Cells["Price"].Value = DBNull.Value;
                  }
                  else
                  {
                    ultEnquiryDetail.Rows[index].Cells["Price"].Value = DBConvert.ParseDouble(dtValue.Rows[0]["SPIL_Price"].ToString());
                  }
                  //if (DBConvert.ParseDouble(dtValue.Rows[0]["VFR_Price"].ToString()) == double.MinValue)
                  //{
                  //  ultEnquiryDetail.Rows[index].Cells["SecondPrice"].Value = DBNull.Value;
                  //}
                  //else
                  //{
                  //  ultEnquiryDetail.Rows[index].Cells["SecondPrice"].Value = DBConvert.ParseDouble(dtValue.Rows[0]["VFR_Price"].ToString());
                  //}
                }
              }
            }
            break;
          case "revision":
            commandText = string.Empty;
            commandText += "  SELECT IB.SaleCode, ISNULL(II.CBM,0) CBM";
            commandText += "  FROM TblBOMItemBasic IB ";
            commandText += "    INNER JOIN  TblBOMItemInfo II ON (IB.ItemCode = II.ItemCode) ";
            commandText += "  WHERE IB.ItemCode = '" + e.Cell.Row.Cells["ItemCode"].Value.ToString() + "' AND Revision = " + DBConvert.ParseInt(value);

            dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt.Rows.Count == 1)
            {
              ultEnquiryDetail.Rows[index].Cells["CBM"].Value = DBConvert.ParseDouble(dt.Rows[0]["CBM"].ToString());
              if (ultEnquiryDetail.Rows[index].Cells["Qty"].Value.ToString().Length > 0)
              {
                ultEnquiryDetail.Rows[index].Cells["TotalCBM"].Value = DBConvert.ParseInt(ultEnquiryDetail.Rows[index].Cells["Qty"].Value.ToString())
                                                                    * DBConvert.ParseDouble(ultEnquiryDetail.Rows[index].Cells["CBM"].Value.ToString());
              }
            }
            break;
          case "qty":
            if (ultEnquiryDetail.Rows[index].Cells["CBM"].Value.ToString().Length > 0 && value.Trim().Length > 0)
            {
              ultEnquiryDetail.Rows[index].Cells["TotalCBM"].Value = DBConvert.ParseInt(value)
                                                                  * DBConvert.ParseDouble(ultEnquiryDetail.Rows[index].Cells["CBM"].Value.ToString());
            }
            else
            {
              ultEnquiryDetail.Rows[index].Cells["TotalCBM"].Value = DBNull.Value;
            }
            try
            {
              if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString()) > 0)
              {
                e.Cell.Row.Cells["Amount"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
              }
              else
              {
                e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
              }
            }
            catch
            {
              e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
            }
            try
            {
              if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value.ToString()) > 0)
              {
                e.Cell.Row.Cells["SecondAmount"].Value = (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value) > 0 ? DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value) : 0) * (DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value) > 0 ? DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value) : 0);
              }
              else
              {
                e.Cell.Row.Cells["SecondAmount"].Value = DBNull.Value;
              }
            }
            catch
            {
              e.Cell.Row.Cells["SecondAmount"].Value = DBNull.Value;
            }
            break;
          case "price":
            try
            {
              if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString()) > 0)
              {
                e.Cell.Row.Cells["Amount"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
              }
              else
              {
                e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
              }
            }
            catch
            {
              e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
            }
            try
            {
              if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value.ToString()) > 0)
              {
                e.Cell.Row.Cells["SecondAmount"].Value = (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value) > 0 ? DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value) : 0) * (DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value) > 0 ? DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value) : 0);
              }
              else
              {
                e.Cell.Row.Cells["SecondAmount"].Value = DBNull.Value;
              }
            }
            catch
            {
              e.Cell.Row.Cells["SecondAmount"].Value = DBNull.Value;
            }
            break;
          case "secondprice":
            try
            {
              if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString()) > 0)
              {
                e.Cell.Row.Cells["Amount"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["Price"].Value.ToString());
              }
              else
              {
                e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
              }
            }
            catch
            {
              e.Cell.Row.Cells["Amount"].Value = DBNull.Value;
            }
            try
            {
              if (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value.ToString()) > 0)
              {
                e.Cell.Row.Cells["SecondAmount"].Value = (DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value) > 0 ? DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value) : 0) * (DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value) > 0 ? DBConvert.ParseDouble(e.Cell.Row.Cells["SecondPrice"].Value) : 0);
              }
              else
              {
                e.Cell.Row.Cells["SecondAmount"].Value = DBNull.Value;
              }
            }
            catch
            {
              e.Cell.Row.Cells["SecondAmount"].Value = DBNull.Value;
            }
            break;
          default:
            break;
        }
      }
      if (!this.chkLoaddata)
      {
        this.NeedToSave = true;
      }
    }

    /// <summary>
    /// ultEnquiryDetail Before Cell Activate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultEnquiryDetail_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string itemCode;
      try
      {
        itemCode = e.Cell.Row.Cells["ItemCode"].Value.ToString();
      }
      catch
      {
        itemCode = "";
      }
      switch (columnName)
      {
        case "revision":
          //if (this.enquiryPid == long.MinValue)
          //{
          UltraDropDown ultRevision = (UltraDropDown)e.Cell.Row.Cells["Revision"].ValueList;
          e.Cell.Row.Cells["Revision"].ValueList = this.LoadRevision(itemCode, ultRevision);
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value) == int.MinValue)
          {
            e.Cell.Row.Cells["Revision"].Value = e.Cell.Row.Cells["Revision"].ValueList.GetValue(e.Cell.Row.Cells["Revision"].ValueList.ItemCount - 1);
          }
          //}
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultEnquiryDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultEnquiryDetail);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = false;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[1].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Return"].Header.Caption = "Customer Return\n NG Item Fixing";
      e.Layout.Bands[0].Columns["Return"].Hidden = true;
      e.Layout.Bands[0].Columns["MOQQty"].Header.Caption = "MOQ Qty";
      e.Layout.Bands[0].Columns["MOQQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["MOQQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SaleCode"].ValueList = ultdpItemCode;
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Remark"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Remark"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SecondAmount"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Amount"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Amount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SecondAmount"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "FOB Amount";
      e.Layout.Bands[0].Columns["SecondAmount"].Header.Caption = "Customer Amount";
      e.Layout.Bands[0].Columns["Return"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["CBM"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["CBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalCBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Item Name";
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RequestDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["RequestDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Request\nShip Date";
      e.Layout.Bands[0].Columns["RequestDate"].MinWidth = 70;
      e.Layout.Bands[0].Columns["RequestDate"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["RequiredShipDate"].Hidden = true;
      e.Layout.Bands[0].Columns["IsGreaterMOQ"].Hidden = true;
      e.Layout.Bands[0].Columns["Select"].Hidden = true;
      e.Layout.Bands[0].Columns["FlagUpdate"].Hidden = true;

      e.Layout.Bands[0].Columns["RequiredShipDate"].MinWidth = 70;
      e.Layout.Bands[0].Columns["RequiredShipDate"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["SpecialDescription"].Header.Caption = "Special Description";
      e.Layout.Bands[0].Columns["SpecialInstruction"].Header.Caption = "Special Instruction";
      e.Layout.Bands[0].Columns["EnquirySale"].Header.Caption = "Up To SO";
      e.Layout.Bands[0].Columns["Price"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Price"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Price"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["Price"].Header.Caption = "FOB Price";
      e.Layout.Bands[0].Columns["Amount"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["SecondPrice"].MinWidth = 60;
      e.Layout.Bands[0].Columns["SecondPrice"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["SecondPrice"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SecondPrice"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["SecondPrice"].Header.Caption = "Customer Price";
      e.Layout.Bands[0].Columns["SecondAmount"].Format = "#,##0.00";
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["UpdateByCSS"].Hidden = true;
      e.Layout.Bands[0].Columns["UpdateDateCSS"].Hidden = true;
      e.Layout.Bands[0].Columns["RemarkForAccount"].Hidden = true;
      if (strType != "2")
      {
        e.Layout.Bands[0].Columns["SpecialDescription"].Hidden = true;
      }
      e.Layout.Bands[0].Override.AllowAddNew = (this.canUpdate && (this.status == 0 || this.status == int.MinValue)) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate && (this.status == 0 || this.status == int.MinValue)) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;

      //e.Layout.Bands[0].Columns["RequestDate"].CellActivation = (this.canUpdate || this.status == 2) ? Activation.AllowEdit : Activation.ActivateOnly;
      //e.Layout.Bands[0].Columns["Qty"].CellActivation = (this.canUpdate || this.status == 2) ? Activation.AllowEdit : Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["NonPlan"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["isGotoSO"].Hidden = true;
      e.Layout.Bands[0].Columns["AllConfirm"].Hidden = true;
      if (this.status >= 1)
      {
        e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["SaleCode"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Price"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["SecondPrice"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Return"].CellActivation = Activation.ActivateOnly;
        if (this.status == 3)
        {
          e.Layout.Override.AllowAddNew = AllowAddNew.No;
          e.Layout.Override.AllowDelete = DefaultableBoolean.False;
        }
      }

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Amount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["SecondAmount"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,###}";
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands[0].Summaries[3].DisplayFormat = "{0:###,##0.00}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[2].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[3].Appearance.TextHAlign = HAlign.Right;
    }

    /// <summary>
    /// chkLoadCust Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkLoadCust_CheckedChanged(object sender, EventArgs e)
    {
      ucbCusGroup.Visible = chkLoadCust.Checked;
      lblDCus.Visible = chkLoadCust.Checked;
      //txtDCus.Visible = chkLoadCust.Checked;
    }

    /// <summary>
    /// cmbCustomer Selected Index Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ucbCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!this.chkLoaddata)
      {
        if (ucbCustomer.SelectedRow.Index > 0)
        {
          string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE ParentPid = {0}", DBConvert.ParseLong(ucbCustomer.Value));
          DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
          Utility.LoadUltraCombo(ucbCusGroup, dtSource, "Pid", "Customer", "Pid");
          chkLoadCust.Visible = false;
          ucbCusGroup.Visible = false;
          txtDCus.Visible = false;
          if (ucbCusGroup.Rows.Count > 1)
          {
            chkLoadCust.Visible = true;
            txtDCus.Visible = true;
          }
        }
      }
      if (!this.chkLoaddata)
      {
        this.NeedToSave = true;
      }
      this.LoadPrice();

    }

    /// <summary>
    /// ultEnquiryDetail Cell Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private void ultEnquiryDetail_CellChange(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      switch (columnName)
      {
        case "expire":
          e.Cell.Row.Cells["Keep"].Value = 0;
          break;

        case "keep":
          e.Cell.Row.Cells["Expire"].Value = 0;
          break;

        default:
          break;
      }

    }

    /// <summary>
    /// txtCustomerEnquiryNo Text Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtCustomerEnquiryNo_TextChanged(object sender, EventArgs e)
    {
      if (!this.chkLoaddata)
      {
        this.NeedToSave = true;
      }
      if (DBConvert.ParseInt(ucbCusGroup.Value) != int.MinValue)
      {
        chkLoadCust.Checked = true;
      }
      else
      {
        chkLoadCust.Checked = false;
      }
    }

    /// <summary>
    /// btnPrint Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultEnquiryDetail, "Enquiry Detail");
      //DBParameter[] inputParam = new DBParameter[] { new DBParameter("@EnPid", DbType.Int64, this.enquiryPid) };
      //DataSet dtset = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCUSListEnquiryDetailByEnPid_Report", inputParam);
      //DataTable dt = dtset.Tables[0];

      //if (dt.Rows.Count > 0 && dt != null)
      //{
      //  string strTemplateName = "CSDEnquiryDetailTemplate";
      //  string strSheetName = "Enquiry";
      //  string strOutFileName = "Enquiry";
      //  string strStartupPath = System.Windows.Forms.Application.StartupPath;
      //  string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //  string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      //  XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);
      //  oXlsReport.Cell("**EnquiryNo").Value = txtEnquiryNo.Text;
      //  oXlsReport.Cell("**CusNo").Value = txtCustomerEnquiryNo.Text;
      //  oXlsReport.Cell("**CusName").Value = cmbCustomer.Text;
      //  oXlsReport.Cell("**EnquiryType").Value = cmbType.Text;
      //  oXlsReport.Cell("**Ref").Value = txtRef.Text;
      //  oXlsReport.Cell("**Podate").Value = ultPoDate.Value;
      //  oXlsReport.Cell("**ReShipdate").Value = ultRequiredShipDate.Value;
      //  oXlsReport.Cell("**Contract").Value = txtContract.Text;
      //  oXlsReport.Cell("**DriectCus").Value = cmbCusGroup.Text;
      //  oXlsReport.Cell("**EnquiryDate").Value = dtdENDate.Value;
      //  oXlsReport.Cell("**Remark").Value = txtRemark.Text;
      //  oXlsReport.Cell("**Delivery").Value = txtDeliveryRequirement.Text;
      //  oXlsReport.Cell("**Packing").Value = txtPackingRequirement.Text;
      //  oXlsReport.Cell("**Document").Value = txtDocumentRequirement.Text;

      //  for (int i = 0; i < dt.Rows.Count; i++)
      //  {
      //    DataRow dtRow = dt.Rows[i];
      //    if (i > 0)
      //    {
      //      oXlsReport.Cell("A11:R11").Copy();
      //      oXlsReport.RowInsert(10 + i);
      //      oXlsReport.Cell("A11:R11", 0, i).Paste();
      //    }

      //    oXlsReport.Cell("**1", 0, i).Value = dtRow["No"];
      //    oXlsReport.Cell("**2", 0, i).Value = dtRow["SaleCode"];
      //    oXlsReport.Cell("**3", 0, i).Value = dtRow["ItemCode"];
      //    oXlsReport.Cell("**4", 0, i).Value = dtRow["Revision"];
      //    oXlsReport.Cell("**5", 0, i).Value = dtRow["Name"];
      //    oXlsReport.Cell("**6", 0, i).Value = dtRow["Qty"];
      //    oXlsReport.Cell("**7", 0, i).Value = dtRow["Unit"];
      //    oXlsReport.Cell("**8", 0, i).Value = dtRow["CBM"];
      //    oXlsReport.Cell("**9", 0, i).Value = dtRow["TotalCBM"];
      //    oXlsReport.Cell("**10", 0, i).Value = dtRow["Price"];
      //    oXlsReport.Cell("**11", 0, i).Value = dtRow["Amount"];
      //    oXlsReport.Cell("**12", 0, i).Value = dtRow["SecondPrice"];
      //    oXlsReport.Cell("**13", 0, i).Value = dtRow["SecondAmount"];
      //    oXlsReport.Cell("**14", 0, i).Value = dtRow["RequestDate"];
      //    oXlsReport.Cell("**15", 0, i).Value = dtRow["SpecialInstruction"];
      //    oXlsReport.Cell("**16", 0, i).Value = dtRow["Remark"];
      //    oXlsReport.Cell("**17", 0, i).Value = dtRow["ScheduleDate"];
      //    oXlsReport.Cell("**18", 0, i).Value = dtRow["PLNRemark"];
      //  }
      //  int cnt = dt.Rows.Count + 10;
      //  oXlsReport.Cell("**SumQty").Value = "=SUM(F11:F" + cnt.ToString() + ")";
      //  //oXlsReport.Cell("**SumCBM").Value = "=SUM(F10:F" + cnt.ToString() + ")";
      //  oXlsReport.Cell("**SumTotalCBM").Value = "=SUM(I11:I" + cnt.ToString() + ")";
      //  oXlsReport.Cell("**SumAmount").Value = "=SUM(K11:K" + cnt.ToString() + ")";
      //  oXlsReport.Cell("**SumSecondAmount").Value = "=SUM(M11:M" + cnt.ToString() + ")";
      //  //oXlsReport.Cell("**SumAmount").Value = "=SUM(L10:L" + cnt.ToString() + ")";
      //  //oXlsReport.Cell("**SumSAmount").Value = "=SUM(M10:M" + cnt.ToString() + ")";
      //  //oXlsReport.Cell("**SumPLNQty").Value = "=SUM(N10:N" + cnt.ToString() + ")";

      //  oXlsReport.Out.File(strOutFileName);
      //  Process.Start(strOutFileName);
      //}
    }


    /// <summary>
    /// cmbType Selected Index Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ucbType_ValueChanged(object sender, EventArgs e)
    {
      if (!this.chkLoaddata)
      {
        this.NeedToSave = true;
      }
      if (ucbType.Value != null)
      {
        if (ucbType.Value.ToString() != "2")
        {
          ultEnquiryDetail.Rows.Band.Columns["SpecialDescription"].Hidden = true;
        }
        else
        {
          ultEnquiryDetail.Rows.Band.Columns["SpecialDescription"].Hidden = false;
        }
      }

    }

    /// <summary>
    /// chkKeep Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkKeep_CheckedChanged(object sender, EventArgs e)
    {
      if (chkKeep.Checked)
      {
        chkCancel.Checked = false;
      }
      tableENExpireDays.Visible = chkKeep.Checked;
    }

    /// <summary>
    /// chkCancel Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkCancel_CheckedChanged(object sender, EventArgs e)
    {
      if (chkCancel.Checked)
      {
        chkKeep.Checked = false;
      }
    }

    /// <summary>
    /// btnReturn Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnReturn_Click(object sender, EventArgs e)
    {
      bool result = true;
      DataSet ds = (DataSet)ultEnquiryDetail.DataSource;
      foreach (DataRow rowReturn in ds.Tables["dtParent"].Rows)
      {
        if (rowReturn.RowState == DataRowState.Modified)
        {
          this.SaveDetail(rowReturn, this.enquiryPid);
          //long enDetailPid = DBConvert.ParseLong(rowReturn["Pid"].ToString());
          //int qtyReturn = DBConvert.ParseInt(rowReturn["Qty"].ToString());
          //string commnadTextUpdate = string.Format("UPDATE TblPLNEnquiryDetail SET Qty = {0} WHERE Pid = {1}", qtyReturn, enDetailPid);
          //result = DataBaseAccess.ExecuteCommandText(commnadTextUpdate);
        }
      }
      //if (result)
      //{
      string commandText = string.Format("UPDATE TblPLNEnquiry SET Status = 1, CSConfirmDate = GETDATE() WHERE Pid = {0}", this.enquiryPid);
      result = DataBaseAccess.ExecuteCommandText(commandText);
      //}
      if (result)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        // Add Email Minh-d 01/07/2011 START
        // Send To PLN
        Email email = new Email();
        email.Key = email.KEY_CSD_003;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), this.txtEnquiryNo.Text, userName);
          string body = string.Format(arrList[2].ToString(), this.txtEnquiryNo.Text, userName);
          email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }
        //Add Email Minh-d 01/07/2011 END

        this.status = 1;
        this.LoadData();
        this.NeedToSave = false;
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
    }

    /// <summary>
    /// chkExpand Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpand_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpand.Checked)
      {
        ultEnquiryDetail.Rows.ExpandAll(true);
      }
      else
      {
        ultEnquiryDetail.Rows.CollapseAll(true);
      }
    }

    private void btnOpenDialog_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      //dialog.InitialDirectory = this.pathExport;
      dialog.InitialDirectory = folder;
      dialog.Title = "Select a Excel file";
      txtFileName.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtFileName.Text.Trim().Length > 0);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid == long.MinValue)
      {
        string message1 = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Customer");
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText(message1);
        return;
      }

      System.Data.DataTable dt = new DataTable();
      try
      {
        dt = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSetVersion2(txtFileName.Text.Trim(), "SELECT * FROM [Data$A5:F65536]").Tables[0];
      }
      catch
      {
        return;
      }
      if (dtSource == null)
      {
        return;
      }
      string message = string.Empty;
      string strSaleCode = "('";
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i]["SaleCode"].ToString().Length > 0)
        {
          int check = 1;
          if (DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString()) == int.MinValue && dt.Rows[i]["Qty"].ToString() != string.Empty)
          {
            check = 0;
          }
          if (check == 1)
          {
            strSaleCode += dt.Rows[i]["SaleCode"].ToString() + "','";
          }
        }
      }
      if (strSaleCode.Length > 2)
      {
        strSaleCode = strSaleCode.Substring(0, strSaleCode.Length - 2);
        strSaleCode += ")";
      }
      else
      {
        strSaleCode += "')";
      }
      string commandtext = "SELECT DISTINCT BS.SaleCode, BS.ItemCode, BS.RevisionActive Revision, BS.Name, BS.CBM, convert(Bigint,null) as Pid, Convert(Float,null) as Price, Convert(Float,null) as SecondPrice, BS.Description SpecialDescription, ";
      commandtext += " Convert(Float,null) TotalCBM,Convert(int,null) Qty,convert(Datetime,null) RequestDate, '' Remark, convert(Float,null) as Amount, convert(Float,null) as SecondAmount, BS.UpdateBy UpdateByCSS, BS.UpdateDate UpdateDateCSS, CASE WHEN CDS.SaleCode IS NULL THEN 1 ELSE CDS.IsConfirm END AllConfirm, 0 [Return]";
      commandtext += " FROM TblBOMItemBasic BS INNER JOIN TblBOMItemInfo INF ON (BS.ItemCode = INF.ItemCode) LEFT JOIN VCSDSaleCodeCD CDS ON BS.SaleCode = CDS.SaleCode WHERE LOWER(BS.SaleCode) IN " + strSaleCode.ToLower();
      DataTable dt_search = DataBaseAccess.SearchCommandTextDataTable(commandtext);
      DataTable dtValue = dt_search.Clone();
      if (dt_search != null)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          for (int j = 0; j < dt_search.Rows.Count; j++)
          {
            if (dt.Rows[i]["SaleCode"].ToString() == dt_search.Rows[j]["SaleCode"].ToString())
            {
              DataRow dr = dtValue.NewRow();
              dr["SaleCode"] = dt_search.Rows[j]["SaleCode"].ToString();
              dr["ItemCode"] = dt_search.Rows[j]["ItemCode"].ToString();
              dr["Revision"] = DBConvert.ParseInt(dt_search.Rows[j]["Revision"].ToString());
              dr["AllConfirm"] = DBConvert.ParseInt(dt_search.Rows[j]["AllConfirm"].ToString());
              dr["Name"] = dt_search.Rows[j]["Name"].ToString();
              dr["CBM"] = DBConvert.ParseDouble(dt_search.Rows[j]["CBM"].ToString());

              if (DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString()) > 0)
              {
                dr["Qty"] = DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString());
              }
              if (dt.Rows[i]["RequestDate"].ToString().Length > 0)
              {
                //Măc dù ép user nhập theo 'dd/MM/yyyy' nhưng vẫn lây theo 4 kiều, (CS is super start!!!)
                string strDate = dt.Rows[i]["RequestDate"].ToString();
                try
                {
                  dr["RequestDate"] = DateTime.ParseExact(strDate, "dd/MM/yyyy", null);
                }
                catch
                {
                  try
                  {
                    dr["RequestDate"] = DateTime.ParseExact(strDate, "dd/MMM/yyyy", null);
                  }
                  catch
                  {
                    try
                    {
                      dr["RequestDate"] = DateTime.ParseExact(strDate, "dd/MM/yy", null);
                    }
                    catch
                    {
                      try
                      {
                        dr["RequestDate"] = DateTime.ParseExact(strDate, "dd/MMM/yy", null);
                      }
                      catch
                      {

                      }
                    }
                  }
                }
              }
              dr["UpdateByCSS"] = SharedObject.UserInfo.UserPid;
              dr["UpdateDateCSS"] = DateTime.Now;
              dr["Remark"] = dt.Rows[i]["Remark"].ToString();

              try
              {
                if (DBConvert.ParseDouble(dt.Rows[i]["Price"].ToString()) != double.MinValue)
                {
                  dr["Price"] = DBConvert.ParseDouble(dt.Rows[i]["Price"].ToString());
                }
                else
                {
                  dr["Price"] = DBNull.Value;
                }
              }
              catch
              {
                dr["Price"] = 0;
              }
              try
              {
                if (DBConvert.ParseDouble(dt.Rows[i]["SecondPrice"].ToString()) != double.MinValue)
                {
                  dr["SecondPrice"] = DBConvert.ParseDouble(dt.Rows[i]["SecondPrice"].ToString());
                }
                else
                {
                  dr["SecondPrice"] = DBNull.Value;
                }
              }
              catch
              {
                dr["SecondPrice"] = 0;
              }
              if (DBConvert.ParseInt(dr["Qty"].ToString()) > 0)
              {
                dr["TotalCBM"] = DBConvert.ParseDouble(dr["Qty"].ToString()) * DBConvert.ParseDouble(dt_search.Rows[j]["CBM"].ToString());
              }
              else
              {
                dr["TotalCBM"] = DBNull.Value;
              }
              try
              {
                if (DBConvert.ParseInt(dr["Qty"].ToString()) > 0 && DBConvert.ParseDouble(dr["Price"].ToString()) > double.MinValue)
                {
                  dr["Amount"] = DBConvert.ParseDouble(dr["Qty"].ToString()) * DBConvert.ParseDouble(dr["Price"].ToString());
                }
                else
                {
                  dr["Amount"] = DBNull.Value;
                }
              }
              catch
              {
                dr["Amount"] = DBNull.Value;
              }
              try
              {
                if (DBConvert.ParseInt(dr["Qty"].ToString()) > 0 && DBConvert.ParseDouble(dr["SecondPrice"].ToString()) > double.MinValue)
                {
                  dr["SecondAmount"] = (DBConvert.ParseInt(dr["Qty"].ToString()) > 0 ? DBConvert.ParseInt(dr["Qty"].ToString()) : 0) * (DBConvert.ParseDouble(dr["SecondPrice"].ToString()) > 0 ? DBConvert.ParseDouble(dr["SecondPrice"].ToString()) : 0);
                }
                else
                {
                  dr["SecondAmount"] = DBNull.Value;
                }
              }
              catch
              {
                dr["SecondAmount"] = DBNull.Value;
              }
              dr["Return"] = 0;
              dtValue.Rows.Add(dr);
              break;
            }
          }
        }
        DataSet ds = (DataSet)ultEnquiryDetail.DataSource;
        ds.Clear();
        ds.Tables["dtParent"].Merge(dtValue);
        ultEnquiryDetail.DataSource = ds;
        string strErrorSaleCode = string.Empty;
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          DataRow[] row = dt_search.Select("SaleCode = '" + dt.Rows[i]["SaleCode"].ToString() + "'");
          if (row.Length == 0 && dt.Rows[i]["SaleCode"].ToString().Length > 0)
          {
            strErrorSaleCode += dt.Rows[i]["SaleCode"].ToString() + " , ";
          }
        }
        if (strErrorSaleCode.Length > 0)
        {
          strErrorSaleCode = strErrorSaleCode.Substring(0, strErrorSaleCode.Length - 2);
          lbErrorSaleCode.Visible = true;
          txtErrorSaleCode.Visible = true;
          txtErrorSaleCode.Text = strErrorSaleCode;
          //Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0007");
        }
        else
        {
          lbErrorSaleCode.Visible = false;
          txtErrorSaleCode.Visible = false;
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0024");
        }
      }
      else
      {
        if (strSaleCode.Length > 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0007");
          return;
        }
      }
      this.LoadPrice();
      //bool chkresult = this.CheckRevision0();
      btnImport.Enabled = false;
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      string templateName = string.Format(@"{0}\ExcelTemplate\ItemSaleList.xls", startupPath);
      if (File.Exists(templateName))
      {
        string newFileName = string.Format(@"{0}\ItemSaleList.xls", folder);
        if (File.Exists(newFileName))
        {
          newFileName = string.Format(@"{0}\ItemSaleList{1}.xls", folder, DateTime.Now.Ticks);
        }
        File.Copy(templateName, newFileName);
        Process.Start(newFileName);
      }
      // Delete all file in folder Report
      foreach (string file in Directory.GetFiles(folder))
      {
        try
        {
          File.Delete(file);
        }
        catch
        {
        }
      }
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultEnquiryDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultEnquiryDetail_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultEnquiryDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void btnSentEmail_Click(object sender, EventArgs e)
    {
      if (WindowUtinity.ShowMessageConfirmFromText("Do You Want To Send Email To Customer").ToString() == "No")
      {
        return;
      }
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@EnquiryPid", DbType.Int64, this.enquiryPid);
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spCSDRPTOrderConfirmation", inputParam);
      ds.Tables[0].Columns.Add("ImageLogo", typeof(System.Byte[]));
      if (WindowUtinity.ShowMessageConfirmFromText("Do You Want To Sent Mail With VFR Logo?").ToString() == "No")
      {
        ds.Tables[0].Rows[0]["ImageLogo"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.VFRLogo_New);
      }
      else
      {
        ds.Tables[0].Rows[0]["ImageLogo"] = FunctionUtility.ImageToByteArray((Image)DaiCo.Shared.IconResource.VFRLogo_New);
      }
      dsCSDOrderConfirmation dsSource = new dsCSDOrderConfirmation();
      dsSource.Tables["dtEnQuiryInfo"].Merge(ds.Tables[0]);
      dsSource.Tables["dtEnQuiryDetail"].Merge(ds.Tables[1]);
      string customerPONO = dsSource.Tables["dtEnQuiryInfo"].Rows[0]["CustomerEnquiryNo"].ToString();
      string customerCode = dsSource.Tables["dtEnQuiryInfo"].Rows[0]["CustomerCode"].ToString();
      customerPONO = customerPONO.Replace("/", "-");
      int totalQty = 0;
      double totalCBMS = 0;
      double totalAmount = 0;
      for (int i = 0; i < dsSource.Tables["dtEnQuiryDetail"].Rows.Count; i++)
      {
        string imgPath = string.Empty;
        imgPath = FunctionUtility.BOMGetItemImage(dsSource.Tables["dtEnQuiryDetail"].Rows[i]["ItemCode"].ToString(), DBConvert.ParseInt(dsSource.Tables["dtEnQuiryDetail"].Rows[i]["Revision"].ToString()));
        dsSource.Tables["dtEnQuiryDetail"].Rows[i]["Picture"] = FunctionUtility.ImagePathToByteArray(imgPath);
      }
      foreach (DataRow row in dsSource.Tables["dtEnQuiryDetail"].Rows)
      {
        if (DBConvert.ParseInt(row["Qty"].ToString()) != int.MinValue)
        {
          totalQty = totalQty + DBConvert.ParseInt(row["Qty"].ToString());
        }

        if (DBConvert.ParseDouble(row["CBMS"].ToString()) != double.MinValue)
        {
          totalCBMS = totalCBMS + DBConvert.ParseDouble(row["CBMS"].ToString());
        }

        if (DBConvert.ParseDouble(row["Amount"].ToString()) != double.MinValue)
        {
          totalAmount = totalAmount + DBConvert.ParseDouble(row["Amount"].ToString());
        }
      }

      DaiCo.Shared.View_Report report = null;
      cptCSDOrderConfirmation_New cpt = new cptCSDOrderConfirmation_New();
      cpt.SetDataSource(dsSource);
      cpt.SetParameterValue("date", DBConvert.ParseString(DateTime.Now, "dd MMM, yy"));
      cpt.SetParameterValue("totalQty", totalQty);
      cpt.SetParameterValue("totalCBMS", totalCBMS);
      cpt.SetParameterValue("totalAmount", totalAmount);

      string time = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();

      // Check Exist Folder
      if (!Directory.Exists(@"\\fsdc01\public\\Reports"))
      {
        Directory.CreateDirectory(@"\\fsdc01\public\\Reports");
      }
      string path = string.Format(@"\\fsdc01\public\\Reports\\{0}_{1}_ORDER CONFIRMATION_{2}_encrypted.pdf", customerPONO, customerCode, DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd")) + time);
      string pathNew = string.Format(@"\\fsdc01\public\\Reports\\{0}_{1}_ORDER CONFIRMATION_{2}.pdf", customerPONO, customerCode, DBConvert.ParseString(DateTime.Now.ToString("yyyyMMdd")) + time);
      cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, path);
      using (Stream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
      using (Stream output = new FileStream(pathNew, FileMode.Create, FileAccess.Write, FileShare.None))
      {
        PdfReader reader = new PdfReader(input);
        PdfEncryptor.Encrypt(reader, output, iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, null, "it1daico", PdfWriter.ALLOW_PRINTING);
      }
      MailMessage mailMessage = new MailMessage();
      string body = string.Empty;
      body += "<p><i><font color='6699CC'>";
      body += "Dear customer,<br><br>";
      body += "This is an automatically generated Order Confirmation.<br>";
      body += "If you have any questions or disagree with any detail enclosed, ";
      body += "please contact our Customer Service Representative immediately.<br><br> ";
      body += "Thanks and kind regards,<br>";
      body += "Jonathan Charles Team ";
      body += "</font></i></p> ";

      string mailTo = string.Empty;
      string commandText = string.Format(@"SELECT email, HoNV + TenNV AS Name FROM VHRNhanVien WHERE ID_NhanVien = {0} AND  Resigned = 0", SharedObject.UserInfo.UserPid);

      //      string commandText = string.Format(@"
      //                          SELECT DISTINCT STUFF((SELECT '; ' + Email
      //                          FROM
      //                          (
      //                            SELECT {0} Customer, CASE WHEN Email1 IS NOT NULL THEN Email1 ELSE Email2 END Email, CT.Name 
      //                            FROM TblCSDContactInfo CT
      //                              LEFT JOIN TblCSDCustomerContactJob CS ON CT.Pid = CS.ContactPid  AND CS.JobPid = 5
      //                            WHERE CT.CustomerPid = {0} AND CS.ContactPid IS NOT NULL
      //                            UNION
      //                            SELECT {0} Customer, email, HoNV + TenNV AS Name FROM VHRNhanVien WHERE Resigned = 0 AND ID_NhanVien = {1}
      //                          ) B
      //                          WHERE A.Customer = B.Customer
      //                          FOR XML PATH ('')), 1, 2, '') Email
      //                          FROM
      //                          (
      //                            SELECT {0} Customer, CASE WHEN Email1 IS NOT NULL THEN Email1 ELSE Email2 END Email, CT.Name 
      //                            FROM TblCSDContactInfo CT
      //                              LEFT JOIN TblCSDCustomerContactJob CS ON CT.Pid = CS.ContactPid  AND CS.JobPid = 5
      //                            WHERE CT.CustomerPid = {0} AND CS.ContactPid IS NOT NULL
      //                            UNION
      //                            SELECT {0} Customer, email, HoNV + TenNV AS Name FROM VHRNhanVien WHERE Resigned = 0 AND ID_NhanVien = {1}
      //                          ) A", DBConvert.ParseLong(Utility.GetSelectedValueCombobox(cmbCustomer)), SharedObject.UserInfo.UserPid);

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        mailTo = dt.Rows[0]["email"].ToString();
      }
      mailMessage.ServerName = "10.0.0.5";
      mailMessage.Username = "dc@daico-furniture.com";
      mailMessage.Password = "dc123456";
      mailMessage.From = "dc@daico-furniture.com";
      mailMessage.To = mailTo;
      mailMessage.Subject = "ORDER CONFIRMATION";
      mailMessage.Body = body;
      //mailMessage.Bcc = "Chau@daico-furniture.com,minh_it@daico-furniture.com,truong_it@daico-furniture.com";

      IList attachments = new ArrayList();

      attachments.Add(pathNew);

      mailMessage.Attachfile = attachments;
      mailMessage.SendMail(true);

      //File.Delete(path);
      //File.Delete(pathNew);

      WindowUtinity.ShowMessageSuccess("MSG0052");
      this.CloseTab();
    }

    private void ultEnquiryDetail_AfterSortChange(object sender, BandEventArgs e)
    {
      for (int i = 0; i < ultEnquiryDetail.Rows.Count; i++)
      {
        ultEnquiryDetail.Rows[i].Cells["No"].Value = i + 1;
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
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@EnPid", DbType.Int64, this.enquiryPid) };
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCUSEnquiry", inputParam);
      dsCUSEnquiry CusEnquiry = new dsCUSEnquiry();
      CusEnquiry.Tables["EnquiryInfomation"].Merge(dsSource.Tables[0]);
      CusEnquiry.Tables["EnquiryDetail"].Merge(dsSource.Tables[1]);
      DaiCo.Shared.View_Report report = null;
      Share.ReportTemplate.cptCUSEnquiry cpt = new Share.ReportTemplate.cptCUSEnquiry();
      cpt.SetDataSource(CusEnquiry);
      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      string random = GetRandomString();
      ////string strStartupPath = System.Windows.Forms.Application.StartupPath;
      ////string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      //cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @"\Reports\CustomerService" + enquiryPid + "_" + random + ".pdf");
      ////report.ShowReport(DaiCo.Shared.Utility.ViewState.ModalWindow);
      //Process.Start(@"\Reports\CustomerService" + enquiryPid + "_" + random + ".pdf");
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string filePath = string.Format(@"{0}\Reports\CustomerService{1}_{2}.pdf", startupPath, enquiryPid, random);
      cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);
      // @"+ startupPath + \Reports\CustomerService" + saleOrderPid + "_" + random + ".pdf");
      //report.ShowReport(DaiCo.Shared.Utility.ViewState.ModalWindow);
      Process.Start(filePath);

    }
    private void ultEnquiryDetail_DoubleClick(object sender, EventArgs e)
    {
      if (ultEnquiryDetail.Selected.Rows.Count > 0 && ultEnquiryDetail.Selected.Rows[0].Band.ParentBand == null)
      {
        int AllConfirm = DBConvert.ParseInt(ultEnquiryDetail.Selected.Rows[0].Cells["AllConfirm"].Value.ToString());
        if (AllConfirm == 0)
        {
          string SaleCode = ultEnquiryDetail.Selected.Rows[0].Cells["SaleCode"].Value.ToString();
          viewCSD_03_015 view = new viewCSD_03_015();
          view.SaleCode = SaleCode;
          WindowUtinity.ShowView(view, "SaleCode Comfirn", true, ViewState.ModalWindow);
          this.LoadGrid();
        }
      }
    }

    /// <summary>
    /// Finish Enquiry before go to SO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFinish_Click(object sender, EventArgs e)
    {
      if (this.enquiryPid != long.MinValue)
      {
        string commandText = string.Format("SELECT Status FROM TblPLNEnquiry WHERE Pid = {0}", this.enquiryPid);
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        this.status = DBConvert.ParseInt(dt.Rows[0][0].ToString());
      }

      string message = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      long pid = long.MinValue;
      success = this.SaveData(out pid, true);
      if (success)
      {
        this.enquiryPid = pid;
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.LoadData();
      }
      else
      {
        if (!this.isFinish)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0095");
          return;
        }
        if (!this.isCancel)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0094");
          return;
        }
        if (chkConfirm.Checked == true && ultEnquiryDetail.Rows.Count == 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0001");
          chkConfirm.Checked = false;
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        }
      }
      this.NeedToSave = false;
    }

    private void chkGetNewPrice_CheckedChanged(object sender, EventArgs e)
    {
      if (chkGetNewPrice.Checked)
      {
        this.GetNewPrice();
        chkGetNewPrice.Checked = false;
      }
    }
    #endregion Event

    private void tableLayoutPanel11_Paint(object sender, PaintEventArgs e)
    {

    }
  }
}