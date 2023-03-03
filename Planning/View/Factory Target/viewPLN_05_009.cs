/*
 * Authour : TRAN HUNG
 * Date : 15/08/2012
 * Description : Sale Order Cancel 
 */
using DaiCo.Application;
using DaiCo.CustomerService;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.CustomerService;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Planning.View;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VBReport;


namespace DaiCo.Planning
{
  public partial class viewPLN_05_009 : MainUserControl
  {
    #region Field
    public long poCancelPid = long.MinValue;
    DataSet dtSource = new DataSet();
    private IList listDeletedPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedswapPid = new ArrayList();
    private IList listDeletingswapPid = new ArrayList();
    private bool canUpdate = false;
    public long customerPid;
    private long totalpid = -1;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region LoadData

    public viewPLN_05_009()
    {
      InitializeComponent();
      dtp_date.Value = DateTime.MinValue;
    }

    /// <summary>
    /// viewCSD_05_009_Load
    /// </summary>
    private void viewPLN_05_009_Load(object sender, EventArgs e)
    {
      this.LoadDropdownList();
      this.LoadData();
    }

    /// <summary>
    /// EnableCustomer
    /// </summary>
    private void EnableCustomer()
    {
      if (ultDetail.Rows.Count > 0)
      {
        chkDirect.Enabled = false;
        cmbCustomer.Enabled = false;
        cmbDirectCustomer.Enabled = false;
      }
      else
      {
        chkDirect.Enabled = true;
        cmbCustomer.Enabled = true;
        cmbDirectCustomer.Enabled = true;
        if (btnSave.Enabled)
        {
          cmbCustomer.Enabled = false;
        }
      }
    }

    /// <summary>
    /// LoadDropdownList
    /// </summary>
    private void LoadDropdownList()
    {
      // Customer
      Shared.Utility.ControlUtility.LoadCustomer(cmbCustomer);
    }

    /// <summary>
    /// LoadDirect Customer
    /// </summary>
    private void LoadDirectCustomer()
    {
      if (this.cmbCustomer.SelectedIndex > 0)
      {
        this.cmbDirectCustomer.DataSource = null;
        string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE ParentPid = " + this.cmbCustomer.SelectedValue);
        DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
        Shared.Utility.ControlUtility.LoadCombobox(cmbDirectCustomer, dtSource, "Pid", "Customer");
      }
    }

    /// <summary>
    /// Load Data 
    /// </summary>
    private void LoadData()
    {
      btnPrint.Enabled = false;
      btnPrintPDF.Enabled = false;
      chkConfirm.Enabled = false;
      dsCSDSaleorderCancel dsSaleordercancel = new dsCSDSaleorderCancel();
      if (this.poCancelPid != long.MinValue)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SaleOrderCancelPid", DbType.Int64, this.poCancelPid) };
        this.dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCSDSaleOrderCancelDetailAutoFill_Select", 600, inputParam);
        dsSaleordercancel.Tables["SaleOrderParent"].Merge(dtSource.Tables[1]);
        dsSaleordercancel.Tables["SaleOrderChild"].Merge(dtSource.Tables[2]);
        dsSaleordercancel.Tables["WoSwap"].Merge(dtSource.Tables[3]);
        //Load Master
        this.txtPoCancelNo.Text = dtSource.Tables[0].Rows[0]["PoCancelNo"].ToString();
        try
        {
          this.cmbCustomer.SelectedValue = DBConvert.ParseLong(dtSource.Tables[0].Rows[0]["CustomerPid"].ToString());
        }
        catch { }

        if (DBConvert.ParseLong(dtSource.Tables[0].Rows[0]["DirectPid"].ToString()) != long.MinValue)
        {
          this.chkDirect.Checked = true;
          this.cmbDirectCustomer.SelectedValue = DBConvert.ParseLong(dtSource.Tables[0].Rows[0]["DirectPid"].ToString());
        }

        this.txtCustomerPoCancelNo.Text = dtSource.Tables[0].Rows[0]["CustomerPoCancelNo"].ToString();
        this.dtp_date.Value = DBConvert.ParseDateTime(dtSource.Tables[0].Rows[0]["CancelDate"].ToString(), formatConvert);
        this.chkConfirm.Checked = (DBConvert.ParseInt(dtSource.Tables[0].Rows[0]["Status"].ToString()) >= 1 ? true : false);
        this.chkPLNcomfirm.Checked = (DBConvert.ParseInt(dtSource.Tables[0].Rows[0]["Status"].ToString()) >= 3 ? true : false);
        this.txtRemark.Text = dtSource.Tables[0].Rows[0]["Remark"].ToString();
        this.txtRef.Text = dtSource.Tables[0].Rows[0]["RefNo"].ToString();
        this.txtContract.Text = dtSource.Tables[0].Rows[0]["Contract"].ToString();
        //this.btnSave.Enabled = (DBConvert.ParseInt(dtSource.Tables[0].Rows[0]["Status"].ToString()) < 1);
        //this.btnPrint.Enabled = (DBConvert.ParseInt(dtSource.Tables[0].Rows[0]["Status"].ToString()) < 0);
        this.btnSaveConfirm.Enabled = (DBConvert.ParseInt(dtSource.Tables[0].Rows[0]["Status"].ToString()) == 2);
        this.chkPLNcomfirm.Enabled = (DBConvert.ParseInt(dtSource.Tables[0].Rows[0]["Status"].ToString()) == 2);
        this.btnPrint.Enabled = true;
        btnPrintPDF.Enabled = true;
        chkConfirm.Enabled = true;
      }
      else
      {
        if (btnSave.Enabled)
        {
          this.cmbCustomer.SelectedValue = 1;
        }
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Prefix", DbType.AnsiString, 16, "SOC") };
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@ResultNo", DbType.AnsiString, 32, string.Empty) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNGetNewPOCancelNo", inputParam, outputParam);
        txtPoCancelNo.Text = outputParam[0].Value.ToString();
        this.chkDirect.Visible = false;
        this.cmbDirectCustomer.Visible = false;
        this.label10.Visible = false;
      }
      this.canUpdate = (btnSave.Enabled && btnSave.Visible);
      if (!this.canUpdate)
      {
        this.cmbCustomer.Enabled = false;
        this.txtCustomerPoCancelNo.ReadOnly = true;
        this.txtCustomerPoCancelNo.Enabled = false;
        this.dtp_date.Enabled = false;
        this.chkConfirm.Enabled = false;
        this.chkDirect.Enabled = false;
        this.txtRemark.ReadOnly = true;
        this.txtRemark.Enabled = false;
        this.txtRef.Enabled = false;
        this.txtContract.Enabled = false;
        btnAddItem.Enabled = false;
      }
      ultDetail.DataSource = dsSaleordercancel;
      ultDetail.Rows.ExpandAll(true);

      // Container Effect
      if (dtSource.Tables[4].Rows.Count > 0 && DBConvert.ParseInt(dtSource.Tables[0].Rows[0]["Status"].ToString()) != 3)
      {
        ultContainerEffect.DataSource = dtSource.Tables[4];
        ultContainerEffect.Visible = true;
      }
      else
      {
        ultContainerEffect.Visible = false;
      }
      // End
      this.EnableCustomer();


      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        if (!this.canUpdate)
        {
          ultDetail.Rows[i].Activation = Activation.ActivateOnly;
        }
        if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["ShipContainer"].Value.ToString()) == 1)
        {
          ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
      }

      this.NeedToSave = false;
    }

    /// <summary>
    /// Get RandomString
    /// </summary>
    /// <returns></returns>
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

    #endregion LoadData,

    #region Check & Save
    /// <summary>
    /// check error saleOrderCancel
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckIsValid(out string message)
    {
      message = string.Empty;
      bool result = true;
      // Customer's PO Cancel No :
      //string text = txtCustomerPoCancelNo.Text.Trim();
      //if (text.Length == 0)
      //{
      //  message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Customer's SO Cancel No");
      //  result = false;
      //}

      // Customer :
      this.customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      if (this.customerPid == long.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Customer");
        result = false;
      }

      //if (chkDirect.Checked == true)
      //{
      //  if (this.cmbDirectCustomer.SelectedIndex == 0)
      //  {
      //    message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Direct Customer");
      //    result = false;
      //  }
      //}

      // Cancel Date :
      DateTime orderDate = dtp_date.Value;
      if (orderDate == DateTime.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "SO Cancel Date");
        result = false;
      }
      // Check value SO Cancel Detail
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        int totalcanceldetail = 0;
        int remain = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Remain"].Value.ToString());
        int cancel = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Cancel"].Value.ToString());
        int openwo = DBConvert.ParseInt(ultDetail.Rows[i].Cells["OpenWO"].Value.ToString());
        if (ultDetail.Rows[i].ChildBands[0].Rows.Count > 0)
        {
          for (int j = 0; j < ultDetail.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            totalcanceldetail += DBConvert.ParseInt(ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["Cancel"].Value.ToString());
          }
        }
        if (totalcanceldetail > cancel)
        {
          message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERRO124"), "Total Qty Swap ", " Cancel Qty");
          ultDetail.Rows[i].CellAppearance.BackColor = Color.PaleGreen;
          result = false;
        }
        else
        {
          if (totalcanceldetail > openwo) // Neu SO Swap co Qty lon hon Qty mo WO cua SO Cancel thi bao loi
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERRO124"), "Total Qty Swap ", " Open WO");
            ultDetail.Rows[i].CellAppearance.BackColor = Color.PaleGreen;
            result = false;
          }
        }
        if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["InternalPO"].Value.ToString()) == 0) // Neu Internal PO khong check thi kiem tra
        {
          if (totalcanceldetail + remain < cancel) //  Neu SO Swap co Qty + Remain SO Cancel khong bang Qty Cancel thi bao loi
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERRO124"), "Cancel ", " Remain");
            ultDetail.Rows[i].CellAppearance.BackColor = Color.PaleGreen;
            result = false;
          }
        }
        else // Neu Internal PO duoc check thi kiem tra
        {
          if (totalcanceldetail >= openwo) // Neu SO Swap co Qty lon hon Qty mo WO cua SO Cancel thi bao loi
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERRO124"), "Cancel ", " Remain");
            ultDetail.Rows[i].CellAppearance.BackColor = Color.PaleGreen;
            result = false;
          }
        }
      }
      return result;
    }

    /// <summary>
    /// save data
    /// </summary>
    /// <param name="row"></param>
    /// <param name="parentPid"></param>
    /// <returns></returns>
    private bool SaveData(out long pid)
    {
      pid = long.MinValue;
      DBParameter[] inputParam = new DBParameter[10];
      bool result = true;
      string storeName = string.Empty;

      // 1 . TblPLNSaleOrderCancel
      if (this.poCancelPid != long.MinValue) // Update vao TblPLNSaleOrderCancel
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.poCancelPid);
        inputParam[9] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = "spPLNSaleOrderCancelAutoFill_Update";
      }
      else // Insert vao TblPLNSaleOrderCancel
      {
        inputParam[9] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        storeName = "spPLNSaleOrderCancelAutoFill_Insert";
      }
      string text = txtCustomerPoCancelNo.Text.Trim();
      inputParam[1] = new DBParameter("@CustomerPoCancelNo", DbType.AnsiString, 64, text);

      this.customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));
      inputParam[2] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);

      DateTime cancelDate = dtp_date.Value;
      if (cancelDate != DateTime.MinValue)
      {
        inputParam[3] = new DBParameter("@CancelDate", DbType.DateTime, new DateTime(cancelDate.Year, cancelDate.Month, cancelDate.Day));
      }
      int index = (chkConfirm.Checked ? 1 : 0);
      if (index == 1 && ultDetail.Rows.Count == 0)
      {
        pid = long.MinValue;
        return false;
      }
      else
      {
        inputParam[4] = new DBParameter("@Status", DbType.Int32, index);
      }

      text = txtRemark.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[5] = new DBParameter("@Remark", DbType.AnsiString, 4000, text);
      }

      if (this.chkDirect.Checked == true)
      {
        if (DBConvert.ParseInt(this.cmbDirectCustomer.SelectedValue.ToString()) != int.MinValue)
        {
          inputParam[6] = new DBParameter("@DirectPid", DbType.Int32, this.cmbDirectCustomer.SelectedValue);
        }
      }

      text = txtRef.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[7] = new DBParameter("@Ref", DbType.String, 1024, text);
      }

      text = txtContract.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[8] = new DBParameter("@Contract", DbType.String, 1024, text);
      }

      DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      pid = DBConvert.ParseLong(outputParam[0].Value.ToString().Trim());
      if (pid <= 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
        return false;
      }
      else
      {
        this.poCancelPid = pid;
        // 2.2 Insert/Update Cancel Detail
        // 2.2.1 Insert/Update Cancel Detail(Cap cha)
        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          ultDetail.Rows[i].CellAppearance.BackColor = Color.Wheat;
          UltraGridRow row = ultDetail.Rows[i];
          DBParameter[] paramdetail = new DBParameter[11];
          string storeNameDetail = string.Empty;
          long piddetail = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          storeNameDetail = "spPLNSaleOrderCancelDetailAutoFill_Edit";
          if (piddetail > 0)
          {
            paramdetail[0] = new DBParameter("@Pid", DbType.Int64, piddetail);
            paramdetail[8] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          }
          else
          {
            paramdetail[7] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          }
          paramdetail[1] = new DBParameter("@PoCancelPid", DbType.Int64, poCancelPid);

          long saleorderPid = DBConvert.ParseLong(row.Cells["SaleOrderPid"].Value.ToString());
          paramdetail[2] = new DBParameter("@SaleOrderPid", DbType.Int64, saleorderPid);

          string textdetail = row.Cells["ItemCode"].Value.ToString();
          paramdetail[3] = new DBParameter("@ItemCode", DbType.AnsiString, 16, textdetail);

          int value = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          paramdetail[4] = new DBParameter("@Revision", DbType.Int32, value);

          value = DBConvert.ParseInt(row.Cells["Cancel"].Value.ToString());
          paramdetail[5] = new DBParameter("@Qty", DbType.Int32, value);

          textdetail = row.Cells["Remark"].Value.ToString().Trim();
          if (textdetail.Length > 0)
          {
            paramdetail[6] = new DBParameter("@Remark", DbType.AnsiString, 4000, textdetail);
          }
          if (chkConfirm.Checked)
          {
            paramdetail[10] = new DBParameter("@Confirm", DbType.Int32, 1);
          }
          DBParameter[] outputparamdetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeNameDetail, paramdetail, outputparamdetail);
          long outvalue = DBConvert.ParseLong(outputparamdetail[0].Value.ToString().Trim());
          if (outvalue == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
            ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
            result = false;
          }
          else
          {
            // 2.2.1 Insert/Update Cancel Detail(Cap con - Swap)

            if (ultDetail.Rows[i].ChildBands[0].Rows.Count > 0)// Co Detail de Swap
            {
              for (int j = 0; j < ultDetail.Rows[i].ChildBands[0].Rows.Count; j++)
              {
                ultDetail.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.LightBlue;
                UltraGridRow rowdetailswap = ultDetail.Rows[i].ChildBands[0].Rows[j];
                DBParameter[] paramdetailswap = new DBParameter[11];
                string storeNamedetailswap = string.Empty;
                long piddetailswap = DBConvert.ParseLong(rowdetailswap.Cells["Pid"].Value.ToString());
                storeNamedetailswap = "spPLNSaleOrderCancelDetailAutoFill_Edit";
                if (piddetailswap > 0)
                {
                  paramdetailswap[0] = new DBParameter("@Pid", DbType.Int64, piddetailswap);
                  paramdetailswap[8] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                }
                else
                {
                  paramdetailswap[7] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                }
                paramdetailswap[1] = new DBParameter("@PoCancelPid", DbType.Int64, poCancelPid);

                long saleorderPidswap = DBConvert.ParseLong(rowdetailswap.Cells["SaleOrderPid"].Value.ToString());
                paramdetailswap[2] = new DBParameter("@SaleOrderPid", DbType.Int64, saleorderPidswap);

                string textdetailswap = rowdetailswap.Cells["ItemCode"].Value.ToString();
                paramdetailswap[3] = new DBParameter("@ItemCode", DbType.AnsiString, 16, textdetailswap);

                int valueswap = DBConvert.ParseInt(rowdetailswap.Cells["Revision"].Value.ToString());
                paramdetailswap[4] = new DBParameter("@Revision", DbType.Int32, valueswap);

                valueswap = DBConvert.ParseInt(rowdetailswap.Cells["Cancel"].Value.ToString());
                paramdetailswap[5] = new DBParameter("@Qty", DbType.Int32, valueswap);
                paramdetailswap[9] = new DBParameter("@ParentPid", DbType.Int64, outvalue);
                DBParameter[] outputParamdetailswap = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeNamedetailswap, paramdetailswap, outputParamdetailswap);
                long outvaluedetail = DBConvert.ParseLong(outputParamdetailswap[0].Value.ToString().Trim());
                if (outvaluedetail == 0)
                {
                  Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
                  ultDetail.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Yellow;
                  result = false;
                }
              }
              if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["InternalPO"].Value.ToString()) == 1) // Neu InternalPO duoc check
              {
                int internalpoQty = 0;
                //int remainQty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Cancel"].Value.ToString()) - DBConvert.ParseInt(ultDetail.Rows[i].Cells["Remain"].Value.ToString());
                int openqty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["OpenWo"].Value.ToString());
                int cancel = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Cancel"].Value.ToString());
                int remain = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Remain"].Value.ToString());
                for (int a = 0; a < ultDetail.Rows[i].ChildBands[0].Rows.Count; a++)
                {
                  internalpoQty += DBConvert.ParseInt(ultDetail.Rows[i].ChildBands[0].Rows[a].Cells["Cancel"].Value.ToString());
                }
                if (internalpoQty < cancel)
                {
                  if (internalpoQty < openqty)
                  {
                    DBParameter[] paramAuto = new DBParameter[3];
                    paramAuto[0] = new DBParameter("@SaleOrderCancelDetailPid", DbType.Int64, outvalue);
                    paramAuto[1] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                    if (cancel > openqty)
                    {
                      if (WindowUtinity.ShowMessageConfirm("MSG0058").ToString() == "No")
                      {
                        if (cancel > remain)
                        {
                          if ((cancel - remain) > internalpoQty)
                          {
                            paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - remain - internalpoQty);
                          }
                          else
                          {
                            if (cancel - remain > openqty - internalpoQty)
                            {
                              paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openqty - internalpoQty);
                            }
                            else
                            {
                              paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - remain);
                            }
                          }
                        }
                        else
                        {
                          paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openqty - internalpoQty);
                        }
                      }
                      else
                      {
                        paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openqty - internalpoQty);
                      }
                    }
                    else // Cancel < Openqty
                    {
                      if (WindowUtinity.ShowMessageConfirm("MSG0057").ToString() == "No")
                      {
                        if (cancel > remain)
                        {
                          if ((cancel - remain) > internalpoQty)
                          {
                            paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - remain - internalpoQty);
                          }
                          else
                          {
                            paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - remain);
                          }
                        }
                        else
                        {
                          paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - internalpoQty);
                        }
                      }
                      else
                      {
                        paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - internalpoQty);
                      }
                    }
                    DBParameter[] outparamAuto = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
                    Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spCSDSaleOrderInternal_Insert", paramAuto, outparamAuto);
                    long outAuto = DBConvert.ParseLong(outparamAuto[0].Value.ToString().Trim());
                    if (outAuto == 0)
                    {
                      Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
                      ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
                      result = false;
                    }
                  }
                }
                else
                {
                  Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0135", "Total Cancel Swap ", " Cancel");
                  result = false;
                }
              }
            }
            else // Khong co Detail de Swap
            {
              int cancelqty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Cancel"].Value.ToString());
              int openwoqty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["OpenWo"].Value.ToString());
              int remainqty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Remain"].Value.ToString());
              if (openwoqty > 0 && DBConvert.ParseInt(ultDetail.Rows[i].Cells["InternalPO"].Value.ToString()) == 1)
              {
                DBParameter[] paramAuto = new DBParameter[3];
                paramAuto[0] = new DBParameter("@SaleOrderCancelDetailPid", DbType.Int64, outvalue);
                paramAuto[1] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                if (cancelqty < openwoqty)
                {
                  if (WindowUtinity.ShowMessageConfirm("MSG0057").ToString() == "No")
                  {
                    if (cancelqty > remainqty)
                    {
                      paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancelqty - remainqty);
                    }
                    else
                    {
                      paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancelqty);
                    }
                  }
                  else
                  {
                    paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancelqty);
                  }
                }
                else
                {
                  if (WindowUtinity.ShowMessageConfirm("MSG0058").ToString() == "No")
                  {
                    if (cancelqty > remainqty)
                    {
                      if ((cancelqty - remainqty) > openwoqty)
                      {
                        paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openwoqty);
                      }
                      else
                      {
                        paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancelqty - remainqty);
                      }
                    }
                    else
                    {
                      paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openwoqty);
                    }
                  }
                  else
                  {
                    paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openwoqty);
                  }
                }
                DBParameter[] outparamAuto = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spCSDSaleOrderInternal_Insert", paramAuto, outparamAuto);
                long outAuto = DBConvert.ParseLong(outparamAuto[0].Value.ToString().Trim());
                if (outAuto == 0)
                {
                  Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
                  ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
                  result = false;
                }
              }
            }
          }
        }
        //2.1 Delete Cancel Detail (cap cha)
        foreach (long detailPid in this.listDeletedPid)
        {
          DBParameter[] inputParamDelete = new DBParameter[2];
          inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
          inputParamDelete[1] = new DBParameter("@Flag", DbType.Int32, 0);
          DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderCancelDetailAutoFill_Delete", inputParamDelete, OutputParamDelete);
          long outputValue = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
          if (outputValue == 0)
          {
            result = false;
          }
        }
        foreach (long detailswapPid in this.listDeletedswapPid)
        {
          DBParameter[] inputParamDeleteswap = new DBParameter[2];
          inputParamDeleteswap[0] = new DBParameter("@Pid", DbType.Int64, detailswapPid);
          inputParamDeleteswap[1] = new DBParameter("@Flag", DbType.Int32, 1);
          DBParameter[] OutputParamDeleteswap = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderCancelDetailAutoFill_Delete", inputParamDeleteswap, OutputParamDeleteswap);
          long outputValueswap = DBConvert.ParseLong(OutputParamDeleteswap[0].Value.ToString());
          if (outputValueswap == 0)
          {
            result = false;
          }
        }

      }
      return result;
    }
    /// <summary>
    /// Swap WO
    /// </summary>
    /// <param name="row"></param>
    /// <param name="parentPid"></param>
    /// <returns></returns>

    private bool SaveSwapWO()
    {
      bool Result = true;
      if (ultDetail.Rows.Count > 0)
      {
        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultDetail.Rows[i];
          if (ultDetail.Rows[i].ChildBands[0].Rows.Count > 0)
          {
            for (int j = 0; j < ultDetail.Rows[i].ChildBands[0].Rows.Count; j++)
            {
              UltraGridRow rowdetailswap = ultDetail.Rows[i].ChildBands[0].Rows[j];
              DBParameter[] param = new DBParameter[4];
              param[0] = new DBParameter("@PoCancelDetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
              param[1] = new DBParameter("@SaleOrderCancelDetailPid", DbType.Int64, DBConvert.ParseLong(rowdetailswap.Cells["Pid"].Value.ToString()));
              param[2] = new DBParameter("@QtySwap", DbType.Int32, DBConvert.ParseInt(rowdetailswap.Cells["Cancel"].Value.ToString()));
              param[3] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              DBParameter[] outparam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
              Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderCancelDetailSwapWO", param, outparam);
              long outswap = DBConvert.ParseLong(outparam[0].Value.ToString().Trim());
              if (outswap == 0)
              {
                Result = false;
              }
            }
          }
        }
      }
      return Result;
    }

    #endregion Check & Save

    #region Event

    /// <summary>
    /// 
    /// chkDirect change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkDirect_CheckedChanged(object sender, EventArgs e)
    {
      if (chkDirect.Checked == true)
      {
        this.cmbDirectCustomer.Visible = true;
        this.cmbDirectCustomer.Enabled = true;
        this.label10.Visible = true;
        if (DBConvert.ParseInt(this.cmbCustomer.SelectedValue.ToString()) != int.MinValue)
        {
          this.LoadDirectCustomer();
        }
      }
      else
      {
        this.cmbDirectCustomer.Visible = false;
        this.cmbDirectCustomer.Enabled = false;
        this.label10.Visible = false;
      }
    }

    /// <summary>
    /// cmbCustomer_SelectedIndexChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.customerPid = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbCustomer));

      string commandText = string.Format("SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE ParentPid = {0}", this.customerPid);
      DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource.Rows.Count > 0)
      {
        this.chkDirect.Visible = true;
        this.chkDirect.Checked = false;
      }
      else
      {
        this.chkDirect.Visible = false;
        this.cmbDirectCustomer.Visible = false;
        this.label10.Visible = false;
        this.cmbDirectCustomer.DataSource = null;
      }
    }

    /// <summary>
    /// 
    /// 
    /// cmbDirectCustomer_SelectedIndexChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbDirectCustomer_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// object change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Object_Changed(object sender, EventArgs e)
    {
      this.NeedToSave = true;
    }

    /// <summary>
    /// Save data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (ultDetail.Rows.Count == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0007");
        return;
      }
      string message = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }
      long pid = long.MinValue;
      success = this.SaveData(out pid);
      if (success)
      {
        this.poCancelPid = pid;
        if (chkConfirm.Checked)
        {
          this.SaveSwapWO();
          DBParameter[] inputParam = new DBParameter[3];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.poCancelPid);
          inputParam[1] = new DBParameter("@Status", DbType.Int32, 3);
          inputParam[2] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spSaleOrderCancel_Confirm", inputParam);
          for (int i = 0; i < ultDetail.Rows.Count; i++)
          {
            if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["ShipContainer"].Value.ToString()) == 1)
            {
              Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0031", ultDetail.Rows[i].Cells["SaleNo"].Value.ToString(), ultDetail.Rows[i].Cells["ContainerNo"].Value.ToString(), ultDetail.Rows[i].Cells["ShipDate"].Value.ToString(), ultDetail.Rows[i].Cells["TotalCBMContainer"].Value.ToString());
            }
          }
        }
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        Shared.Utility.FunctionUtility.UnlockPLNSaleOrder(this.poCancelPid);
        if (chkConfirm.Checked == true && ultDetail.Rows.Count == 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0001");
          chkConfirm.Checked = false;
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        }
      }

      this.btnSave.Enabled = !chkConfirm.Checked;
      this.btnPrint.Visible = true;
      this.btnPrintPDF.Visible = true;
      this.LoadData();

    }

    /// <summary>
    /// close uc
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// design grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.AutoFitColumns = true;
      ultDetail.Rows.ExpandAll(true);
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PoCancelPid"].Hidden = true;
      e.Layout.Bands[0].Columns["PoCancelPid"].Hidden = true;
      e.Layout.Bands[2].Columns["SaleOrderCancelDetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ShipContainer"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerNo"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalCBMContainer"].Hidden = true;
      e.Layout.Bands[0].Columns["AddDetail"].Hidden = (!this.btnSave.Visible) ? true : false;
      e.Layout.Bands[0].Columns["InternalPO"].Hidden = (!this.btnSave.Visible) ? true : false;
      //e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      //e.Layout.Bands[1].Override.AllowDelete = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = (!this.btnSave.Visible) ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowDelete = (!this.btnSave.Visible) ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[2].Override.AllowDelete = (!this.btnSave.Visible) ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Remark"].CellMultiLine = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["SaleNo"].Header.Caption = "Sale No";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Item Name";
      e.Layout.Bands[0].Columns["OriginalQty"].Header.Caption = "Order Qty";
      e.Layout.Bands[0].Columns["BalQty"].Header.Caption = "Bal Qty";
      e.Layout.Bands[0].Columns["InternalPO"].Header.Caption = "Internal PO";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total Cancel CBM";
      e.Layout.Bands[0].Columns["Remain"].Header.Caption = "Unreleased";
      e.Layout.Bands[1].Columns["Remain"].Header.Caption = "Unreleased";
      e.Layout.Bands[0].Columns["ShipDate"].Header.Caption = "Loading Date";
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["InternalPO"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AddDetail"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
      e.Layout.Bands[2].Columns["WipRun"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["AddDetail"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
      e.Layout.Bands[0].Columns["AddDetail"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["AddDetail"].MinWidth = 70;
      e.Layout.Bands[0].Columns["InternalPO"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["InternalPO"].MinWidth = 70;
      e.Layout.Bands[0].Columns["SaleNo"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["SaleNo"].MinWidth = 100;
      e.Layout.Bands[0].Columns["AddDetail"].Header.Caption = "";
      for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        string columnName = ultDetail.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if (string.Compare(columnName, "Internal PO", true) == 0 || string.Compare(columnName, string.Empty, true) == 0 || string.Compare(columnName, "Cancel", true) == 0)
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
        if (string.Compare(columnName, "Cancel", true) == 0 || string.Compare(columnName, "Remain", true) == 0 || string.Compare(columnName, "Bal Qty", true) == 0)
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.BackColor = Color.BurlyWood;
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.ForeColor = Color.Blue;
        }
        ultDetail.DisplayLayout.Bands[0].Columns[i].CellAppearance.BackColor = Color.Wheat;
      }
      for (int j = 0; j < ultDetail.DisplayLayout.Bands[1].Columns.Count; j++)
      {
        ultDetail.DisplayLayout.Bands[1].Columns[j].CellActivation = Activation.ActivateOnly;
        ultDetail.DisplayLayout.Bands[1].Columns[j].CellAppearance.BackColor = Color.LightBlue;
      }
      for (int k = 0; k < ultDetail.DisplayLayout.Bands[2].Columns.Count; k++)
      {

        string columnName2 = ultDetail.DisplayLayout.Bands[2].Columns[k].Header.Caption;
        if (string.Compare(columnName2, "swapwo", true) == 0)
        {
          ultDetail.DisplayLayout.Bands[2].Columns[k].CellAppearance.ForeColor = Color.Blue;
        }
        if (string.Compare(columnName2, "swapsaleorder", true) == 0)
        {
          ultDetail.DisplayLayout.Bands[2].Columns[k].CellAppearance.ForeColor = Color.Green;
        }
        ultDetail.DisplayLayout.Bands[2].Columns[k].CellActivation = Activation.ActivateOnly;
        ultDetail.DisplayLayout.Bands[2].Columns[k].CellAppearance.BackColor = Color.Pink;
      }
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalCBM"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Cancel"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,###.##}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OriginalQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["BalQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Cancel"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Cancelled"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OpenWO"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Remain"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[1].Columns["Cancel"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[2].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[2].Columns["WoInfoPID"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[2].Columns["ItemCode"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[2].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
    }

    /// <summary>
    /// ultDetail_AfterCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.NeedToSave = true;
    }

    /// <summary>
    /// ultDetail_AfterRowsDeleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      this.NeedToSave = true;
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
      foreach (long pidswap in this.listDeletingswapPid)
      {
        this.listDeletedswapPid.Add(pidswap);
      }
      this.EnableCustomer();
    }

    /// <summary>
    /// ultDetail_BeforeCellUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      int remainQty = DBConvert.ParseInt(e.Cell.Row.Cells["Remain"].Value.ToString());
      int balanceQty = DBConvert.ParseInt(e.Cell.Row.Cells["BalQty"].Value.ToString());
      int cancelQty = DBConvert.ParseInt(e.Cell.Text.Trim());
      if (string.Compare(colName, "Cancel", true) == 0)
      {
        if (cancelQty <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Cancel Qty" });
          e.Cancel = true;
        }
        else if (cancelQty > balanceQty)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERRO124", "Cancel Qty", "Balance");
          e.Cancel = true;
        }
      }
    }

    /// <summary>
    /// ultDetail_BeforeRowsDeleted
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      this.listDeletingswapPid = new ArrayList();
      if (ultDetail.Selected.Rows[0].ParentRow == null)
      {
        foreach (UltraGridRow row in e.Rows)
        {
          long detailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (detailPid > 0)
          {
            listDeletingPid.Add(detailPid);
          }
        }
      }
      else
      {
        foreach (UltraGridRow row in e.Rows)
        {
          long detailswapPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          if (detailswapPid > 0)
          {
            listDeletingswapPid.Add(detailswapPid);
          }
        }
      }
    }

    /// <summary>
    /// Add Item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        directCus = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbDirectCustomer));
        if (directCus == long.MinValue)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Direct Customer");
          return;
        }
      }
      #endregion Check
      DataSet ds = (DataSet)ultDetail.DataSource;
      viewPLN_05_010 uc = new viewPLN_05_010();
      uc.customerPid = customerPid;
      uc.customerDirect = directCus;
      uc.dtExistingSource = ds.Tables[0];
      DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "ADD ITEM TO SALE ORDER CANCEL", false, DaiCo.Shared.Utility.ViewState.ModalWindow);

      foreach (DataRow row in uc.dtNewSource.Rows)
      {
        DataRow newRow = ds.Tables[0].NewRow();
        newRow["Pid"] = totalpid--;
        newRow["SaleOrderPid"] = row["SaleOrderPid"];
        newRow["SaleNo"] = row["SaleNo"];
        newRow["ItemCode"] = row["ItemCode"];
        newRow["ItemName"] = row["ItemName"];
        newRow["Revision"] = row["Revision"];
        //newRow["ScheduleDelivery"] = row["ScheduleDelivery"];
        newRow["OriginalQty"] = row["OriginalQty"];
        newRow["BalQty"] = row["BalQty"];
        //newRow["Qty"] = row["Qty"];
        newRow["Cancelled"] = row["Cancelled"];
        newRow["OpenWO"] = row["OpenWO"];
        newRow["Remain"] = row["Remain"];
        newRow["Cancel"] = row["Cancel"];
        newRow["SaleCode"] = row["SaleCode"];
        newRow["CBM"] = row["CBM"];
        newRow["TotalCBM"] = row["TotalCBM"];
        ds.Tables[0].Rows.Add(newRow);
      }
      dsCSDSaleorderCancel dt = new dsCSDSaleorderCancel();
      dt.Tables["SaleOrderParent"].Merge(ds.Tables[0]);
      dt.Tables["SaleOrderChild"].Merge(ds.Tables[1]);
      ultDetail.DataSource = dt;


      this.EnableCustomer();
    }

    /// <summary>
    /// ShowImage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      DaiCo.Shared.Utility.ControlUtility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    /// <summary>
    /// Mouse Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_MouseClick(object sender, MouseEventArgs e)
    {
      DaiCo.Shared.Utility.ControlUtility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    /// <summary>
    /// Export Exel
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrint_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SaleOrderCancelPid", DbType.Int64, this.poCancelPid) };
      DataSet dtset = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCSDSaleOrderCancel", inputParam);
      string strTemplateName = "CSDSaleOrderCancelTemplate";
      string strSheetName = "SaleOrderCancel";
      string strOutFileName = "SaleOrderCancel";
      string strStartupPath = System.Windows.Forms.Application.StartupPath;
      string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
      string strPathTemplate = strStartupPath + @"\ExcelTemplate\CustomerService";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);
      oXlsReport.Cell("**CusName").Value = dtset.Tables[0].Rows[0]["Name"].ToString();
      oXlsReport.Cell("**CancelNo").Value = dtset.Tables[0].Rows[0]["PoCancelNo"].ToString();
      oXlsReport.Cell("**Ref").Value = dtset.Tables[0].Rows[0]["RefNo"].ToString();
      oXlsReport.Cell("**Canceldate").Value = dtset.Tables[0].Rows[0]["CancelDate"].ToString();
      oXlsReport.Cell("**Contract").Value = dtset.Tables[0].Rows[0]["Contract"].ToString();
      oXlsReport.Cell("**Remark").Value = dtset.Tables[0].Rows[0]["Remark"].ToString();
      if (chkDirect.Checked)
      {
        oXlsReport.Cell("**DriectCus").Value = dtset.Tables[0].Rows[0]["DirectName"].ToString();
      }


      for (int i = 0; i < dtset.Tables[1].Rows.Count; i++)
      {
        DataRow dtRow = dtset.Tables[1].Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("A13:J13").Copy();
          oXlsReport.RowInsert(12 + i);
          oXlsReport.Cell("A13:J13", 0, i).Paste();
        }
        for (int c = 0; c < dtset.Tables[1].Columns.Count; c++)
        {
          oXlsReport.Cell("**1", 0, i).Value = i + 1;
          oXlsReport.Cell("**2", 0, i).Value = dtRow["CustomerPONo"];
          oXlsReport.Cell("**3", 0, i).Value = dtRow["SaleNo"];
          oXlsReport.Cell("**4", 0, i).Value = dtRow["SaleCode"];
          oXlsReport.Cell("**5", 0, i).Value = dtRow["ItemCode"];
          oXlsReport.Cell("**6", 0, i).Value = dtRow["ItemName"];
          oXlsReport.Cell("**7", 0, i).Value = dtRow["Unit"];
          oXlsReport.Cell("**8", 0, i).Value = dtRow["Qty"];
          oXlsReport.Cell("**9", 0, i).Value = dtRow["CBM"];
          oXlsReport.Cell("**10", 0, i).Value = dtRow["Remark"];
        }
      }
      int cnt = dtset.Tables[1].Rows.Count + 12;
      oXlsReport.Cell("**SumQty").Value = "=SUM(H13:H" + cnt.ToString() + ")";
      oXlsReport.Cell("**SumCBMS").Value = "=SUM(I13:I" + cnt.ToString() + ")";
      oXlsReport.Out.File(strOutFileName);
      Process.Start(strOutFileName);
    }

    /// <summary>
    /// ultDetail_InitializeRow
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeRow(object sender, InitializeRowEventArgs e)
    {
      // We only need to do this the first time the row is initialized.
      try
      {
        if (e.ReInitialize == false)
        {
          e.Row.Cells["AddDetail"].Value = "Add Detail";
          e.Row.Cells["AddDetail"].ButtonAppearance.ForeColor = Color.Blue;
        }
      }
      catch { }
    }

    /// <summary>
    /// ultDetail_ClickCellButton
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_ClickCellButton(object sender, CellEventArgs e)
    {
      UltraGridRow Row = e.Cell.Row;
      long saleorderPid = DBConvert.ParseLong(Row.Cells["SaleOrderPid"].Value.ToString());
      long Pid = DBConvert.ParseLong(Row.Cells["Pid"].Value.ToString());
      string itemcodeSwapCancel = Row.Cells["ItemCode"].Value.ToString();
      int revisionSwapCancel = DBConvert.ParseInt(Row.Cells["Revision"].Value.ToString());
      int openWO = DBConvert.ParseInt(Row.Cells["OpenWO"].Value.ToString());
      if (openWO > 0 && btnSave.Visible == true)
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
          directCus = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbDirectCustomer));
          if (directCus == long.MinValue)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Direct Customer");
            return;
          }
        }
        #endregion Check
        DataSet ds = (DataSet)ultDetail.DataSource;
        viewPLN_05_010 uc = new viewPLN_05_010();
        uc.customerPid = customerPid;
        uc.customerDirect = directCus;
        uc.dtExistingSource = ds.Tables[1];
        uc.Pid = Pid;
        uc.saleorderPid = saleorderPid;
        uc.itemcodeSwapCancel = itemcodeSwapCancel;
        uc.revisionSwapCancel = revisionSwapCancel;
        DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "ADD ITEM TO SALE ORDER CANCEL", false, DaiCo.Shared.Utility.ViewState.ModalWindow);

        foreach (DataRow row in uc.dtNewSource.Rows)
        {
          DataRow newRow = ds.Tables[1].NewRow();
          newRow["ParentPid"] = Pid;
          newRow["SaleOrderPid"] = row["SaleOrderPid"];
          newRow["SaleNo"] = row["SaleNo"];
          newRow["ItemCode"] = row["ItemCode"];
          newRow["Revision"] = row["Revision"];
          newRow["Remain"] = row["Remain"];
          newRow["Cancel"] = row["Cancel"];
          ds.Tables[1].Rows.Add(newRow);

        }
        dsCSDSaleorderCancel dt = new dsCSDSaleorderCancel();
        dt.Tables["SaleOrderParent"].Merge(ds.Tables[0]);
        dt.Tables["SaleOrderChild"].Merge(ds.Tables[1]);
        ultDetail.DataSource = dt;


        this.EnableCustomer();
      }
      else if (openWO <= 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0110", "Open WO");
      }
    }
    /// <summary>
    /// Print to PDF
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPrintPDF_Click(object sender, EventArgs e)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SaleOrderCancelPid", DbType.Int64, this.poCancelPid) };
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCSDSaleOrderCancel", inputParam);
      dsCUSSaleorderCancel dsCUSSaleorderCancel = new dsCUSSaleorderCancel();
      dsCUSSaleorderCancel.Tables["SaleOrderCancel"].Merge(dsSource.Tables[0]);
      dsCUSSaleorderCancel.Tables["SaleOrderCancelDetail"].Merge(dsSource.Tables[1]);
      DaiCo.Shared.View_Report report = null;
      DaiCo.Shared.ReportTemplate.CustomerService.cptCSDSaleOrderCancel cpt = new DaiCo.Shared.ReportTemplate.CustomerService.cptCSDSaleOrderCancel();
      cpt.SetDataSource(dsCUSSaleorderCancel);
      report = new DaiCo.Shared.View_Report(cpt);
      report.IsShowGroupTree = false;
      string random = GetRandomString();
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string filePath = string.Format(@"{0}\Reports\CustomerService{1}_{2}.pdf", startupPath, poCancelPid, random);
      cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);
      // @"+ startupPath + \Reports\CustomerService" + saleOrderPid + "_" + random + ".pdf");
      //report.ShowReport(DaiCo.Shared.Utility.ViewState.ModalWindow);
      Process.Start(filePath);
    }

    /// <summary>
    /// PLN Confirm
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveConfirm_Click(object sender, EventArgs e)
    {
      if (chkPLNcomfirm.Checked == false)
      {
        MessageBox.Show(Shared.Utility.FunctionUtility.GetMessage("ERR0025"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@Pid", DbType.Int64, this.poCancelPid);
      inputParam[1] = new DBParameter("@Status", DbType.Int32, 3);
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spSaleOrderCancel_Confirm", inputParam);

      // Update Container Effect
      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@POCancelPid", DbType.Int64, this.poCancelPid);

      DBParameter[] output = new DBParameter[1];
      output[0] = new DBParameter("@Result", DbType.Int64, 0);

      DataBaseAccess.ExecuteStoreProcedure("spPLNUQtyContainerListWhenCancelPO_Update", input, output);
      long result = DBConvert.ParseLong(output[0].Value.ToString());
      if (result <= 0)
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      // End
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["ShipContainer"].Value.ToString()) == 1)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0031", ultDetail.Rows[i].Cells["SaleNo"].Value.ToString(), ultDetail.Rows[i].Cells["ContainerNo"].Value.ToString(), ultDetail.Rows[i].Cells["ShipDate"].Value.ToString(), ultDetail.Rows[i].Cells["TotalCBMContainer"].Value.ToString());
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.LoadData();
    }

    /// <summary>
    /// PLN Swap WO 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        UltraGridRow row = ultDetail.Selected.Rows[0];
        UltraGridRow rowparent = ultDetail.Selected.Rows[0].ParentRow;
        UltraGridRow rowparentparent = ultDetail.Selected.Rows[0].ParentRow.ParentRow;
        if (row.HasParent() && !row.HasChild() && this.btnSaveConfirm.Enabled == true && !chkSwapWO.Checked)
        {
          viewPLN_02_015 view = new viewPLN_02_015();
          view.WoDetail = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          view.WorkOrderPID = DBConvert.ParseLong(row.Cells["WoInfoPID"].Value.ToString());
          view.Priority = DBConvert.ParseInt(row.Cells["Priority"].Value.ToString());
          view.saleordercanceldetailPid = DBConvert.ParseLong(rowparent.Cells["Pid"].Value.ToString());
          view.parentPid = DBConvert.ParseLong(rowparent.Cells["ParentPid"].Value.ToString());
          view.pocancelPid = DBConvert.ParseLong(rowparent.Cells["PoCancelPid"].Value.ToString());
          Shared.Utility.WindowUtinity.ShowView(view, "SWAP WORK ORDER", true, Shared.Utility.ViewState.ModalWindow);
          this.LoadData();

        }
        if (row.HasParent() && !row.HasChild() && this.btnSaveConfirm.Enabled == true && chkSwapWO.Checked)
        {
          viewPLN_02_016 view = new viewPLN_02_016();
          view.wo = DBConvert.ParseLong(row.Cells["WoInfoPID"].Value.ToString());
          view.pocanceldetalPid = DBConvert.ParseLong(rowparentparent.Cells["Pid"].Value.ToString());
          view.wodetailgeneralPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          view.saleordercanceldetalPid = DBConvert.ParseLong(row.Cells["SaleOrderCancelDetailPid"].Value.ToString());
          view.saleorderdetailswap = DBConvert.ParseLong(row.Cells["SaleOrderDetailPid"].Value.ToString());
          Shared.Utility.WindowUtinity.ShowView(view, "SWAP WORK ORDER LINK SALE ORDER", true, Shared.Utility.ViewState.ModalWindow);
          this.LoadData();

        }
      }
      catch { }

    }

    private void ultContainerEffect_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion Event
  }
}
