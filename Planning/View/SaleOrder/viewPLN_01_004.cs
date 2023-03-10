/*
  Author      : Lậm Quang Hà
  Date        : 01/06/2010
  Decription  : Cập nhật thông tin Sale Order  
 */
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_01_004 : MainUserControl
  {
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public long saleOrderPid = long.MinValue;
    DataTable dtSource = new DataTable();
    private bool canUpdate = false;

    private long customerPid = long.MinValue;
    private long directPid = long.MinValue;

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      object sender = new object();
      EventArgs e = new EventArgs();
      btnSave_Click(sender, e);
    }
    public viewPLN_01_004()
    {
      InitializeComponent();
    }

    #region Load Data
    bool chkLoadData;
    private void viewPLN_01_004_Load(object sender, EventArgs e)
    {
      this.LoadDropdownList();
      this.LoadData();
      this.NeedToSave = false;

    }

    private void LoadDropdownList()
    {
      chkLoadData = true;
      // Customer
      Shared.Utility.ControlUtility.LoadCustomer(cmbCustomer);
      chkLoadData = false;

      // Type
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbType, Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);

      // EnvironmentalStatus
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbEnvironmentalStatus, Shared.Utility.ConstantClass.GROUP_EnvironmentalStatus);
    }

    private void LoadData()
    {
      PLNSaleOrder saleOrder = new PLNSaleOrder();
      saleOrder.Pid = this.saleOrderPid;
      saleOrder = (PLNSaleOrder)Shared.DataBaseUtility.DataBaseAccess.LoadObject(saleOrder, new string[] { "Pid" });
      // Minhd 15/06/2011 Email 
      this.customerPid = saleOrder.CustomerPid;
      this.directPid = saleOrder.DirectPid;
      // Minhd 15/06/2011 Email 
      if (saleOrder == null)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0007");
        this.ConfirmToCloseTab();
        return;
      }
      txtPoNo.Text = saleOrder.SaleNo;
      try
      {
        cmbCustomer.SelectedValue = saleOrder.CustomerPid;
      }
      catch
      {
      }
      try
      {
        cmbDCustomer.SelectedValue = saleOrder.DirectPid;
      }
      catch
      {
      }
      try
      {
        dtdPODate.Value = saleOrder.OrderDate;
      }
      catch
      {
      }

      txtCustomerPoNo.Text = saleOrder.CustomerPONo;
      txtDeliveryRequirement.Text = saleOrder.DeliveryRequirement;
      txtPackingRequirement.Text = saleOrder.PackingRequirement;
      txtDocumentRequirement.Text = saleOrder.DocumentRequirement;
      txtShipmentTerms.Text = saleOrder.ShipmentTerms;
      txtPaymentTerms.Text = saleOrder.PaymentTerms;
      cmbEnvironmentalStatus.SelectedValue = saleOrder.EnvironmentStatus;
      //txtOrderDate.Text = DBConvert.ParseString(saleOrder.OrderDate, MainBOMLibary.FORMAT_DATETIME);
      try
      {
        cmbType.SelectedValue = saleOrder.Type;
      }
      catch
      {
      }

      chkConfirm.Checked = ((saleOrder.Status == 2) || (saleOrder.Status == 0) || (saleOrder.Status == 1) ? false : true);
      if (chkConfirm.Checked)
      {
        btnSave.Enabled = false;
      }
      else
      {
        btnSave.Enabled = true;

      }
      chkConfirm.Enabled = (saleOrder.Status == 3 ? false : true);
      btnSave.Enabled = (saleOrder.Status != 3);
      btnPrint.Enabled = (saleOrder.Status == 3);
      txtRemark.Text = saleOrder.Remark.Trim();
      cmbCustomer.Enabled = false;
      cmbEnvironmentalStatus.Enabled = false;
      txtCustomerPoNo.ReadOnly = true;
      //txtOrderDate.ReadOnly = true;
      //dpkOrderDate.Enabled = false;
      cmbType.Enabled = false;
      txtRemark.ReadOnly = true;

      this.canUpdate = ((chkConfirm.Checked == false) && (btnSave.Enabled));
      this.LoadGrid();
    }

    private void LoadGrid()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, saleOrderPid) };
      this.dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spPLNListSaleOrderDetailByPid", inputParam);
      try
      {
        this.dtSource.PrimaryKey = new DataColumn[] { this.dtSource.Columns["ItemCode"], this.dtSource.Columns["Revision"], this.dtSource.Columns["ScheduleDelivery"] };
      }
      catch
      {
      }
      //this.dtSource.Columns["Qty"].AllowDBNull = false;
      ultDetail.DataSource = this.dtSource;
      string value = Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbType);
      if (value == "2")
      {
        ultDetail.DisplayLayout.Bands[0].Columns["SpecialDescription"].Hidden = false;
      }
      else
      {
        ultDetail.DisplayLayout.Bands[0].Columns["SpecialDescription"].Hidden = true;
      }

      if (!this.canUpdate)
      {
        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          ultDetail.Rows[i].Activation = Activation.ActivateOnly;
        }
      }
    }

    #endregion Load Data

    #region Save Data

    private bool SaveDetail(DataRow row, long parentPid)
    {

      DBParameter[] param = new DBParameter[11];
      string storeName = string.Empty;
      long pid = DBConvert.ParseLong(row["Pid"].ToString());
      if (pid != long.MinValue)
      {
        storeName = "spPLNSaleOrderDetail_Update";
        param[0] = new DBParameter("@Pid", DbType.Int64, pid);
      }
      param[1] = new DBParameter("@SaleOrderPid", DbType.Int64, saleOrderPid);

      string text = row["ItemCode"].ToString().Trim();
      param[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, text);

      int revision = DBConvert.ParseInt(row["Revision"].ToString());
      param[3] = new DBParameter("@Revision", DbType.Int32, revision);

      int qty = DBConvert.ParseInt(row["Qty"].ToString());
      param[4] = new DBParameter("@Qty", DbType.Int32, qty);

      DateTime scheduleDelivery = DateTime.MinValue;
      try
      {
        scheduleDelivery = (DateTime)row["ScheduleDelivery"];
      }
      catch { }
      if (scheduleDelivery != DateTime.MinValue)
      {
        param[5] = new DBParameter("@ScheduleDelivery", DbType.DateTime, scheduleDelivery);
      }

      DateTime realDelivery = DateTime.MinValue;
      try
      {
        realDelivery = (DateTime)row["RealDelivery"];
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

      text = row["Remark"].ToString().Trim();
      if (text.Length > 0)
      {
        param[8] = new DBParameter("@Remark", DbType.String, 4000, text);
      }

      text = row["SpecialDescription"].ToString().Trim();
      param[9] = new DBParameter("@SpecialDescription", DbType.String, 4000, text);

      param[10] = new DBParameter("@UpdateByPLN", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, param, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    private bool SaveData(out long pid)
    {
      pid = long.MinValue;
      bool result = true;
      this.dtSource = (DataTable)ultDetail.DataSource;
      foreach (DataRow dr in dtSource.Rows)
      {
        if (dr.RowState == DataRowState.Modified)
        {
          bool success = this.SaveDetail(dr, pid);
          if (!success)
          {
            result = false;
          }
        }
      }
      //Update status Sale Order
      if (chkConfirm.Checked && result)
      {
        //int status = 3;
        //string updateCommand = string.Format("Update TblPLNSaleOrder Set Status = {0} Where Pid = {1}", status, saleOrderPid);
        //result = Shared.DataBaseUtility.DataBaseAccess.ExecuteCommandText(updateCommand);
        DBParameter[] param = new DBParameter[2];
        param[0] = new DBParameter("@SaleorderPid", DbType.Int64, saleOrderPid);
        param[1] = new DBParameter("@PLNConfirmBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderConfirmByPLN", param);
      }
      return result;
    }

    #endregion Save Data

    #region Event

    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["DisplayItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVN"].Hidden = true;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Schedule Date";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Hidden = true;
      e.Layout.Bands[0].Columns["RealDelivery"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["RealDelivery"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["RealDelivery"].Hidden = true;
      e.Layout.Bands[0].Columns["SpecialDescription"].Header.Caption = "Special Description";
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Request Ship Date";
      e.Layout.Bands[0].Columns["RequestDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["RequestDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ScheduleDate"].Header.Caption = "Confirmed Ship Date";
      e.Layout.Bands[0].Columns["ScheduleDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["ScheduleDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["ScheduleDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SpecialInstruction"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SpecialInstruction"].Header.Caption = "Special Instruction";
      e.Layout.Bands[0].Columns["Price"].Hidden = true;
      e.Layout.Bands[0].Columns["UpdateByPLN"].Hidden = true;
      e.Layout.Bands[0].Columns["UpdateDatePLN"].Hidden = true;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      //Cuong Add 21/07/2015
      e.Layout.Bands[0].Columns["Return"].Header.Caption = "Customer Return\n NG Item Fixing";
      e.Layout.Bands[0].Columns["Return"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Return"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Return"].Hidden = true;
      e.Layout.Bands[0].Columns["UrgentNote"].Hidden = true;
      e.Layout.Bands[0].Columns["EnquiryRemark"].Header.Caption = "Enquiry Remark";
      e.Layout.Bands[0].Columns["EnquiryRemark"].CellActivation = Activation.ActivateOnly;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      long pid = long.MinValue;
      bool success = this.SaveData(out pid);
      if (success)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        // Add Email Minh-d 01/07/2011 START
        // Send To ACC
        Email email = new Email();
        email.Key = email.KEY_PLN_004;
        ArrayList arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), this.txtPoNo.Text, userName);
          string body = string.Format(arrList[2].ToString(), this.txtPoNo.Text, userName);
          email.InsertEmail(email.Key, arrList[0].ToString(), subject, body);
        }

        // Send To CS
        email = new Email();
        if (directPid == long.MinValue)
        {
          email.Key = email.KEY_PLN_005;
        }
        else
        {
          email.Key = email.KEY_PLN_006;
        }

        arrList = email.GetDataMain(email.Key);
        if (arrList.Count == 3)
        {
          string toFromSql = string.Empty;
          if (directPid == long.MinValue)
          {
            toFromSql = string.Format(arrList[0].ToString(), customerPid);
          }
          else
          {
            toFromSql = string.Format(arrList[0].ToString(), customerPid, directPid);
          }

          toFromSql = email.GetEmailToFromSql(toFromSql);

          string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
          string subject = string.Format(arrList[1].ToString(), this.txtPoNo.Text, userName);
          string body = string.Format(arrList[2].ToString(), this.txtPoNo.Text, userName);
          email.InsertEmail(email.Key, toFromSql, subject, body);
        }
        //Add Email Minh-d 01/07/2011 END
      }
      else
      {
        Shared.Utility.FunctionUtility.UnlockPLNSaleOrder(this.saleOrderPid);
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
      this.NeedToSave = false;

    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    #endregion Event

    private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (!chkLoadData)
      {
        if (cmbCustomer.SelectedIndex > 0)
        {
          try
          {
            Shared.Utility.ControlUtility.LoadDirectCustomer(cmbDCustomer, DBConvert.ParseLong(cmbCustomer.SelectedValue.ToString()));
          }
          catch
          { }
          chkCus.Checked = false;
          if (cmbDCustomer.Items.Count > 0)
          {
            chkCus.Checked = true;
          }
        }
      }
    }

    private void ultDetail_CellChange(object sender, CellEventArgs e)
    {
      this.NeedToSave = true;
    }

    private void chkConfirm_CheckedChanged(object sender, EventArgs e)
    {
      this.NeedToSave = true;
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      ControlUtility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultDetail_MouseClick(object sender, MouseEventArgs e)
    {
      ControlUtility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void txtRemark_TextChanged(object sender, EventArgs e)
    {

    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SaleorderPid", DbType.Int64, this.saleOrderPid) };
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCSDSaleOrderDetail", inputParam);
      DataTable dt = dsSource.Tables[1];
      string strTemplateName = "PLNSaleOrderDetailTemplate";
      string strSheetName = "SaleOrder";
      string strOutFileName = "SaleOrder";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\Planning";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

      oXlsReport.Cell("**CusName").Value = cmbCustomer.Text;
      oXlsReport.Cell("**CusNo").Value = txtCustomerPoNo.Text;
      oXlsReport.Cell("**Ref").Value = dsSource.Tables[0].Rows[0]["REFNo"].ToString();
      oXlsReport.Cell("**Podate").Value = dsSource.Tables[0].Rows[0]["PODate"].ToString();
      oXlsReport.Cell("**ReShipdate").Value = dsSource.Tables[0].Rows[0]["RequiredShipDate"].ToString();
      oXlsReport.Cell("**Contract").Value = dsSource.Tables[0].Rows[0]["ContractNo"].ToString();
      oXlsReport.Cell("**SaleorderNo").Value = dsSource.Tables[0].Rows[0]["SaleNo"].ToString();
      oXlsReport.Cell("**SaleorderDate").Value = dsSource.Tables[0].Rows[0]["OrderDate"].ToString();
      oXlsReport.Cell("**SaleorderType").Value = cmbType.Text;
      oXlsReport.Cell("**ConfShipdate").Value = "";
      oXlsReport.Cell("**Remark").Value = txtRemark.Text;
      oXlsReport.Cell("**Delivery").Value = txtDeliveryRequirement.Text;
      oXlsReport.Cell("**Packing").Value = txtPackingRequirement.Text;
      oXlsReport.Cell("**Document").Value = txtDocumentRequirement.Text;
      if (chkCus.Checked)
      {
        oXlsReport.Cell("**DriectCus").Value = cmbDCustomer.Text;
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
          oXlsReport.Cell("**18", 0, i).Value = dtRow["RequestDate"];
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
  }
}
