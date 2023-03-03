/*
 * Authour : TRAN HUNG
 * Date : 15/08/2012
 * Description : Sale Order Cancel 
 */
using DaiCo.Application;
using DaiCo.Shared;
//using DaiCo.ERPProject.DataSetSource;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.CustomerService;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_05_009 : MainUserControl
  {
    #region Field
    public long poCancelPid = long.MinValue;
    DataSet dtSource = new DataSet();
    private IList listDeletedPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedswapPid = new ArrayList();
    private IList listDeletingswapPid = new ArrayList();
    private bool canUpdate = false;
    private long customerPid = long.MinValue;
    private long totalpid = -1;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region LoadData

    public viewCSD_05_009()
    {
      InitializeComponent();
      udtDate.Value = DateTime.MinValue;
    }

    /// <summary>
    /// viewCSD_05_009_Load
    /// </summary>
    private void viewCSD_05_009_Load(object sender, EventArgs e)
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
        ucbCustomer.Enabled = false;
        ucbDirectCustomer.Enabled = false;
      }
      else
      {
        chkDirect.Enabled = true;
        ucbCustomer.Enabled = true;
        ucbDirectCustomer.Enabled = true;
      }
    }

    /// <summary>
    /// LoadDropdownList
    /// </summary>
    private void LoadDropdownList()
    {
      // Customer
      Utility.LoadCustomer(ucbCustomer);
    }

    /// <summary>
    /// LoadDirect Customer
    /// </summary>
    private void LoadDirectCustomer()
    {
      if (this.ucbCustomer.SelectedRow.Index > 0)
      {
        this.ucbDirectCustomer.DataSource = null;
        string commandText = string.Format(@"SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo WHERE ParentPid = " + ucbCustomer.Value);
        DataTable dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
        Utility.LoadUltraCombo(ucbDirectCustomer, dtSource, "Pid", "Customer", false, "Pid");
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
      DaiCo.Shared.DataSetSource.CustomerService.dsCSDSaleorderCancel dsSaleordercancel = new DaiCo.Shared.DataSetSource.CustomerService.dsCSDSaleorderCancel();
      if (this.poCancelPid != long.MinValue)
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SaleOrderCancelPid", DbType.Int64, this.poCancelPid) };
        this.dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCSDSaleOrderCancelDetailAutoFill_Select", 300, inputParam);
        dsSaleordercancel.Tables["SaleOrderParent"].Merge(dtSource.Tables[1]);
        dsSaleordercancel.Tables["SaleOrderChild"].Merge(dtSource.Tables[2]);
        dsSaleordercancel.Tables["WoSwap"].Merge(dtSource.Tables[3]);
        //Load Master
        this.txtPoCancelNo.Text = dtSource.Tables[0].Rows[0]["PoCancelNo"].ToString();
        try
        {
          this.ucbCustomer.Value = DBConvert.ParseLong(dtSource.Tables[0].Rows[0]["CustomerPid"].ToString());
        }
        catch { }

        if (DBConvert.ParseLong(dtSource.Tables[0].Rows[0]["DirectPid"].ToString()) != long.MinValue)
        {
          this.chkDirect.Checked = true;
          this.ucbDirectCustomer.Value = DBConvert.ParseLong(dtSource.Tables[0].Rows[0]["DirectPid"].ToString());
        }

        this.txtCustomerPoCancelNo.Text = dtSource.Tables[0].Rows[0]["CustomerPoCancelNo"].ToString();
        this.udtDate.Value = DBConvert.ParseDateTime(dtSource.Tables[0].Rows[0]["CancelDate"].ToString(), formatConvert);
        this.chkConfirm.Checked = (DBConvert.ParseInt(dtSource.Tables[0].Rows[0]["Status"].ToString()) >= 1 ? true : false);
        this.txtRemark.Text = dtSource.Tables[0].Rows[0]["Remark"].ToString();
        this.txtRef.Text = dtSource.Tables[0].Rows[0]["RefNo"].ToString();
        this.txtContract.Text = dtSource.Tables[0].Rows[0]["Contract"].ToString();
        this.btnSave.Enabled = (DBConvert.ParseInt(dtSource.Tables[0].Rows[0]["Status"].ToString()) < 1);
        this.btnPrint.Enabled = true;
        this.btnPrintPDF.Enabled = true;
        chkConfirm.Enabled = true;
      }
      else
      {
        DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Prefix", DbType.AnsiString, 16, "POC") };
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@ResultNo", DbType.AnsiString, 32, string.Empty) };
        Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNGetNewPOCancelNo", inputParam, outputParam);
        txtPoCancelNo.Text = outputParam[0].Value.ToString();
        this.chkDirect.Visible = false;
        this.ucbDirectCustomer.Visible = false;
        this.label10.Visible = false;
      }
      this.canUpdate = (btnSave.Enabled && btnSave.Visible);
      if (!this.canUpdate)
      {
        this.ucbCustomer.Enabled = false;
        this.txtCustomerPoCancelNo.ReadOnly = true;
        this.txtCustomerPoCancelNo.Enabled = false;
        this.udtDate.Enabled = false;
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
      this.EnableCustomer();
      if (!this.canUpdate)
      {
        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          ultDetail.Rows[i].Activation = Activation.ActivateOnly;
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
      string text = txtCustomerPoCancelNo.Text.Trim();
      if (text.Length == 0)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Customer's SO Cancel No");
        result = false;
      }

      // Customer :
      this.customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (this.customerPid == long.MinValue)
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Customer");
        result = false;
      }

      if (chkDirect.Checked == true)
      {
        if (this.ucbDirectCustomer.Value == null)
        {
          message = string.Format(Shared.Utility.FunctionUtility.GetMessage("MSG0005"), "Direct Customer");
          result = false;
        }
      }

      // Cancel Date :
      DateTime orderDate =  DBConvert.ParseDateTime(udtDate.Value);
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
        string itemcode = ultDetail.Rows[i].Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Revision"].Value.ToString());
        long saleorderpid = DBConvert.ParseLong(ultDetail.Rows[i].Cells["SaleOrderPid"].Value.ToString());
        DBParameter[] inputParam = new DBParameter[3];
        inputParam[0] = new DBParameter("@SaleOrderPid", DbType.Int64, saleorderpid);
        inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 32, itemcode);
        inputParam[2] = new DBParameter("@Revision", DbType.Int32, revision);
        int totalUnrelease = DBConvert.ParseInt(DataBaseAccess.SearchStoreProcedureDataTable("spPLNCheckingTotalUnrealseForPOCancellation", inputParam).Rows[0][0].ToString());
        for (int t = 0; t < i; t++)
        {
          if (ultDetail.Rows[t].ChildBands[0].Rows.Count > 0)
          {
            for (int g = 0; g < ultDetail.Rows[t].ChildBands[0].Rows.Count; g++)
            {
              if (ultDetail.Rows[t].ChildBands[0].Rows[g].Cells["ItemCode"].Value.ToString() == itemcode && DBConvert.ParseInt(ultDetail.Rows[t].ChildBands[0].Rows[g].Cells["Revision"].Value.ToString()) == revision)
              {
                totalUnrelease -= DBConvert.ParseInt(ultDetail.Rows[t].ChildBands[0].Rows[g].Cells["Cancel"].Value.ToString());
              }
            }
          }
        }
        if (ultDetail.Rows[i].ChildBands[0].Rows.Count > 0)
        {
          for (int j = 0; j < ultDetail.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            totalcanceldetail += DBConvert.ParseInt(ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["Cancel"].Value.ToString());
          }
        }
        int minCancel = Math.Min((cancel - remain), totalUnrelease);
        if (totalcanceldetail < minCancel)
        {
          message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0027"), "Total Qty Swap ");
          ultDetail.Rows[i].CellAppearance.BackColor = Color.PaleGreen;
          result = false;
        }
        else if (totalcanceldetail == minCancel)
        {
          if (cancel - minCancel > 0)
          {
            ultDetail.Rows[i].Cells["InternalPO"].Value = 1;
          }
        }
        else if (totalcanceldetail > cancel)
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

        //if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["InternalPO"].Value.ToString()) == 0) // Neu Internal PO khong check thi kiem tra
        //{
        //  if (totalcanceldetail + remain < cancel) //  Neu SO Swap co Qty + Remain SO Cancel khong bang Qty Cancel thi bao loi
        //  {
        //    message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERRO124"), "Cancel ", " Remain");
        //    ultDetail.Rows[i].CellAppearance.BackColor = Color.PaleGreen;
        //    result = false;
        //  }
        //}
        //else // Neu Internal PO duoc check thi kiem tra
        //{
        //  if (totalcanceldetail >= openwo) // Neu SO Swap co Qty lon hon Qty mo WO cua SO Cancel thi bao loi
        //  {
        //    message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERRO124"), "Total Qty Swap ", " OpenWO");
        //    ultDetail.Rows[i].CellAppearance.BackColor = Color.PaleGreen;
        //    result = false;
        //  }
        //}
      }
      return result;
    }
    private bool CheckIsValidConfirm()
    {
      bool result = true;
      DBParameter[] input = new DBParameter[1] { new DBParameter("@PidCancelation", DbType.Int64, this.poCancelPid) };
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spCSDCheckConfirmCancelation", input);
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        DataRow[] dr = dt.Select(string.Format("Pid = {0} AND Error <> 0", DBConvert.ParseLong(ultDetail.Rows[i].Cells["Pid"].Value.ToString())));
        if (dr.Length > 0)
        {
          result = false;
          switch (dr[0]["Error"].ToString())
          {
            case "1":
              ultDetail.Rows[i].CellAppearance.BackColor = Color.GreenYellow;
              break;
            case "2":
              ultDetail.Rows[i].CellAppearance.BackColor = Color.Lime;
              break;
            case "3":
              ultDetail.Rows[i].CellAppearance.BackColor = Color.Thistle;
              break;
            default:
              break;
          }
        }
      }
      return result;
    }

    private bool CheckRemainInter()
    {
      bool success = true;
      string cm = "spPLNSaleOrderCancelRemain";
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Type", DbType.Int32, 1);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable(cm, inputParam); ;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow gr = ultDetail.Rows[i];
        for (int j = 0; j < gr.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow cgr = gr.ChildBands[0].Rows[j];
          DataRow[] row = dt.Select(string.Format("Pid = {0} and  REMAIN = 0", cgr.Cells["Pid"].Value.ToString()));
          if (row.Length > 0)
          {
            cgr.CellAppearance.BackColor = Color.GreenYellow;
            success = false;
          }
        }
      }
      return success;
    }
    private bool CheckRemainOther()
    {
      bool success = true;
      string cm = "spPLNSaleOrderCancelRemain";
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@Type", DbType.Int32, 2);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable(cm, inputParam);
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow gr = ultDetail.Rows[i];
        for (int j = 0; j < gr.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow cgr = gr.ChildBands[0].Rows[j];
          DataRow[] row = dt.Select(string.Format("Pid = {0} and  REMAIN < {1}", cgr.Cells["Pid"].Value.ToString(), cgr.Cells["Remain"].Value.ToString()));
          if (row.Length > 0)
          {
            cgr.CellAppearance.BackColor = Color.GreenYellow;
            success = false;
          }
        }
      }
      return success;
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
      DBParameter[] inputParam = new DBParameter[11];
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

      this.customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      inputParam[2] = new DBParameter("@CustomerPid", DbType.Int64, this.customerPid);

      DateTime cancelDate = DBConvert.ParseDateTime(udtDate.Value);
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
      if (chkConfirm.Checked)
      {
        inputParam[10] = new DBParameter("@CSConfirmDate", DbType.DateTime, DateTime.Now);
      }
      text = txtRemark.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[5] = new DBParameter("@Remark", DbType.AnsiString, 4000, text);
      }

      if (this.chkDirect.Checked == true)
      {
        if (DBConvert.ParseInt(this.ucbDirectCustomer.Value.ToString()) != int.MinValue)
        {
          inputParam[6] = new DBParameter("@DirectPid", DbType.Int32, DBConvert.ParseInt(this.ucbDirectCustomer.Value.ToString()));
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
          DBParameter[] paramdetail = new DBParameter[12];
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
          textdetail = row.Cells["RemarkForAccount"].Value.ToString().Trim();
          if (textdetail.Length > 0)
          {
            paramdetail[11] = new DBParameter("@RemarkForAccount", DbType.AnsiString, 4000, textdetail);
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
              //if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["InternalPO"].Value.ToString()) == 1) // Neu InternalPO duoc check
              //{
              //  int internalpoQty = 0;
              //  //int remainQty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Cancel"].Value.ToString()) - DBConvert.ParseInt(ultDetail.Rows[i].Cells["Remain"].Value.ToString());
              //  int openqty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["OpenWo"].Value.ToString());
              //  int cancel = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Cancel"].Value.ToString());
              //  int remain = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Remain"].Value.ToString());
              //  for (int a = 0; a < ultDetail.Rows[i].ChildBands[0].Rows.Count; a++)
              //  {
              //    internalpoQty += DBConvert.ParseInt(ultDetail.Rows[i].ChildBands[0].Rows[a].Cells["Cancel"].Value.ToString());
              //  }
              //  if (internalpoQty < cancel)
              //  {
              //    if (internalpoQty < openqty)
              //    {
              //      DBParameter[] paramAuto = new DBParameter[3];
              //      paramAuto[0] = new DBParameter("@SaleOrderCancelDetailPid", DbType.Int64, outvalue);
              //      paramAuto[1] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
              //      if (cancel > openqty)
              //      {
              //        if (WindowUtinity.ShowMessageConfirm("MSG0058").ToString() == "No")
              //        {
              //          if (cancel > remain)
              //          {
              //            if ((cancel - remain) > internalpoQty)
              //            {
              //              paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - remain - internalpoQty);
              //            }
              //            else
              //            {
              //              if (cancel - remain > openqty - internalpoQty)
              //              {
              //                paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openqty - internalpoQty);
              //              }
              //              else
              //              {
              //                paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - remain);
              //              }
              //            }
              //          }
              //          else
              //          {
              //            paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openqty - internalpoQty);
              //          }
              //        }
              //        else
              //        {
              //          paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openqty - internalpoQty);
              //        }
              //      }
              //      else // Cancel < Openqty
              //      {
              //        if (WindowUtinity.ShowMessageConfirm("MSG0057").ToString() == "No")
              //        {
              //          if (cancel > remain)
              //          {
              //            if ((cancel - remain) > internalpoQty)
              //            {
              //              paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - remain - internalpoQty);
              //            }
              //            else
              //            {
              //              paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - remain);
              //            }
              //          }
              //          else
              //          {
              //            paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel - internalpoQty);
              //          }
              //        }
              //        else
              //        {
              //          paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancel  - internalpoQty);
              //        }
              //      }
              //      DBParameter[] outparamAuto = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
              //      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spCSDSaleOrderInternal_Insert", paramAuto, outparamAuto);
              //      long outAuto = DBConvert.ParseLong(outparamAuto[0].Value.ToString().Trim());
              //      if (outAuto == 0)
              //      {
              //        Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
              //        ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
              //        result = false;
              //      }
              //    }
              //  }
              //  else
              //  {
              //    Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0135", "toatal Cancel Swap ", " Cancel");
              //    result = false;
              //  }
              //}
            }
            //else // Khong co Detail de Swap
            //{
            //  int cancelqty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Cancel"].Value.ToString());
            //  int openwoqty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["OpenWo"].Value.ToString());
            //  int remainqty = DBConvert.ParseInt(ultDetail.Rows[i].Cells["Remain"].Value.ToString());
            //  if (openwoqty > 0 && DBConvert.ParseInt(ultDetail.Rows[i].Cells["InternalPO"].Value.ToString()) == 1)
            //  {
            //    DBParameter[] paramAuto = new DBParameter[3];
            //    paramAuto[0] = new DBParameter("@SaleOrderCancelDetailPid", DbType.Int64, outvalue);
            //    paramAuto[1] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            //    if (cancelqty < openwoqty)
            //    {
            //      if (WindowUtinity.ShowMessageConfirm("MSG0057").ToString() == "No")
            //      {
            //        if (cancelqty > remainqty)
            //        {
            //          paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancelqty - remainqty);
            //        }
            //        else
            //        {
            //          paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancelqty);
            //        }
            //      }
            //      else
            //      {
            //        paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancelqty);
            //      }
            //    }
            //    else
            //    {
            //      if (WindowUtinity.ShowMessageConfirm("MSG0058").ToString() == "No")
            //      {
            //        if (cancelqty > remainqty)
            //        {
            //          if ((cancelqty - remainqty) > openwoqty)
            //          {
            //            paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openwoqty);
            //          }
            //          else
            //          {
            //            paramAuto[2] = new DBParameter("@Qty", DbType.Int32, cancelqty - remainqty);
            //          }
            //        }
            //        else
            //        {
            //          paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openwoqty);
            //        }
            //      }
            //      else
            //      {
            //        paramAuto[2] = new DBParameter("@Qty", DbType.Int32, openwoqty);
            //      }
            //    }
            //    DBParameter[] outparamAuto = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            //    Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spCSDSaleOrderInternal_Insert", paramAuto, outparamAuto);
            //    long outAuto = DBConvert.ParseLong(outparamAuto[0].Value.ToString().Trim());
            //    if (outAuto == 0)
            //    {
            //      Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
            //      ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
            //      result = false;
            //    }
            //  }
            //}
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
    private bool CreateInternalPO()
    {
      bool Result = true;

      DBParameter[] param = new DBParameter[4];
      param[0] = new DBParameter("@SaleOrderCancelPid", DbType.Int64, this.poCancelPid);
      param[1] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      DBParameter[] outparam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spCSDSaleOrderInternalForAll_Insert", param, outparam);
      long outswap = DBConvert.ParseLong(outparam[0].Value.ToString().Trim());
      if (outswap == 0)
      {
        Result = false;
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
        this.ucbDirectCustomer.Visible = true;
        this.ucbDirectCustomer.Enabled = true;
        this.label10.Visible = true;
        if (DBConvert.ParseInt(this.ucbCustomer.Value.ToString()) != int.MinValue)
        {
          this.LoadDirectCustomer();
        }
      }
      else
      {
        this.ucbDirectCustomer.Visible = false;
        this.ucbDirectCustomer.Enabled = false;
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
      this.customerPid = DBConvert.ParseLong(ucbCustomer.Value);

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
        this.ucbDirectCustomer.Visible = false;
        this.label10.Visible = false;
        this.ucbDirectCustomer.DataSource = null;
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
      long pid = long.MinValue;
      bool success = false;
      string message = string.Empty;
      bool checkConfirm = false;

      checkConfirm = chkConfirm.Checked;
      chkConfirm.Checked = false;
      success = this.SaveData(out pid);
      this.LoadData();
      if (success)
      {
        if (checkConfirm)
        {
          if (this.CheckIsValidConfirm())
          {
            chkConfirm.Checked = checkConfirm;
          }
          else
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
            return;
          }
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
          return;
        }
        success = this.CheckIsValid(out message);
        if (!success)
        {
          MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          return;
        }
      }
      else
      {
        return;
      }

      success = this.SaveData(out pid);
      if (success)
      {
        this.poCancelPid = pid;
        if (chkConfirm.Checked)
        {
          if (this.CheckRemainInter())
          {
            if (this.CheckRemainOther())
            {
              //this.CreateInternalPO();
              DaiCo.Shared.DataSetSource.CustomerService.dsCSDSaleorderCancel dsSaleordercancel = new DaiCo.Shared.DataSetSource.CustomerService.dsCSDSaleorderCancel();
              if (this.poCancelPid != long.MinValue)
              {
                DBParameter[] inputParam = new DBParameter[] { new DBParameter("@SaleOrderCancelPid", DbType.Int64, this.poCancelPid) };
                this.dtSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spCSDSaleOrderCancelDetailAutoFill_Select", 300, inputParam);
                dsSaleordercancel.Tables["SaleOrderParent"].Merge(dtSource.Tables[1]);
                dsSaleordercancel.Tables["SaleOrderChild"].Merge(dtSource.Tables[2]);
                dsSaleordercancel.Tables["WoSwap"].Merge(dtSource.Tables[3]);
              }
              ultDetail.DataSource = dsSaleordercancel;
              this.SaveSwapWO();
              Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
              this.LoadData();
            }
            else
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Remain");
              chkConfirm.Checked = false;

            }
          }
          else
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Remain");
            chkConfirm.Checked = false;

          }
        }
        else
        {
          this.LoadData();
        }
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
        this.LoadData();
      }



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
      Utility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.AutoFitColumns = true;
      ultDetail.Rows.ExpandAll(true);
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["BalQty"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ParentPid"].Hidden = true;
      e.Layout.Bands[1].Columns["PoCancelPid"].Hidden = true;
      e.Layout.Bands[0].Columns["PoCancelPid"].Hidden = true;
      e.Layout.Bands[2].Columns["SaleOrderCancelDetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ShipContainer"].Hidden = true;
      e.Layout.Bands[0].Columns["ShipDate"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerNo"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalCBMContainer"].Hidden = true;
      e.Layout.Bands[0].Columns["AddDetail"].Hidden = (!this.btnSave.Enabled) ? true : false;
      e.Layout.Bands[0].Columns["InternalPO"].Hidden = true;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowDelete = (this.canUpdate) ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Remark"].CellMultiLine = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["SaleNo"].Header.Caption = "Sale No";
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Item Name";
      e.Layout.Bands[0].Columns["OriginalQty"].Header.Caption = "Original Qty";
      //e.Layout.Bands[0].Columns["BalQty"].Header.Caption = "Bal Qty";
      e.Layout.Bands[0].Columns["OriginalBalance"].Header.Caption = "Bal Qty";
      e.Layout.Bands[0].Columns["InternalPO"].Header.Caption = "Internal PO";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total Cancel CBM";
      e.Layout.Bands[0].Columns["Remain"].Header.Caption = "Unreleased";
      e.Layout.Bands[1].Columns["Remain"].Header.Caption = "Unreleased";
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
      e.Layout.Bands[0].Columns["OriginalBalance"].CellAppearance.TextHAlign = HAlign.Right;
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
      e.Layout.Bands[2].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Center;
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

      if (string.Compare(colName, "Cancel", true) == 0)
      {
        int cancelQty = DBConvert.ParseInt(e.Cell.Text.Trim());
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
      //if (string.Compare(colName, "InternalPO", true) == 0)
      //{
      //  int cancelQty = DBConvert.ParseInt(e.Cell.Row.Cells["Cancel"].Value.ToString());
      //  if (remainQty >= cancelQty)
      //  {
      //    e.Cancel = true;
      //  }
      //}
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
      long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
      if (customerPid == long.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Customer");
        return;
      }

      //Direct Customer :
      long directCus = long.MinValue;
      if (chkDirect.Checked)
      {
        directCus = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ucbDirectCustomer));
        if (directCus == long.MinValue)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Direct Customer");
          return;
        }
      }
      #endregion Check
      DataSet ds = (DataSet)ultDetail.DataSource;
      viewCSD_05_010 uc = new viewCSD_05_010();
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
      DaiCo.Shared.DataSetSource.CustomerService.dsCSDSaleorderCancel dt = new DaiCo.Shared.DataSetSource.CustomerService.dsCSDSaleorderCancel();
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
      Utility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    /// <summary>
    /// Mouse Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
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
      if (openWO > 0 && btnSave.Enabled == true)
      {
        #region Check
        // Customer :
        long customerPid = DBConvert.ParseLong(ucbCustomer.Value);
        if (customerPid == long.MinValue)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Customer");
          return;
        }

        //Direct Customer :
        long directCus = long.MinValue;
        if (chkDirect.Checked)
        {
          directCus = DBConvert.ParseLong(Utility.GetSelectedValueUltraCombobox(ucbDirectCustomer));
          if (directCus == long.MinValue)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Direct Customer");
            return;
          }
        }
        #endregion Check
        DataSet ds = (DataSet)ultDetail.DataSource;
        viewCSD_05_010 uc = new viewCSD_05_010();
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
        DaiCo.Shared.DataSetSource.CustomerService.dsCSDSaleorderCancel dt = new DaiCo.Shared.DataSetSource.CustomerService.dsCSDSaleorderCancel();
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
      dsSource.Tables[1].Columns.Add("Picture", typeof(System.Byte[]));
      for (int i = 0; i < dsSource.Tables[1].Rows.Count; i++)
      {
        try
        {
          string imgPath = FunctionUtility.BOMGetItemImage(dsSource.Tables[1].Rows[i]["ItemCode"].ToString(), DBConvert.ParseInt(dsSource.Tables[1].Rows[i]["Revision"].ToString()));
          dsSource.Tables[1].Rows[i]["Picture"] = FunctionUtility.ImageToByteArrayWithFormat(imgPath, 380, 3.24, "JPG");
        }
        catch { }
      }
      dsCUSSaleorderCancel dsCUSSaleorderCancel = new dsCUSSaleorderCancel();
      dsCUSSaleorderCancel.Tables["SaleOrderCancel"].Merge(dsSource.Tables[0]);
      dsCUSSaleorderCancel.Tables["SaleOrderCancelDetail"].Merge(dsSource.Tables[1]);
      //DaiCo.Shared.View_Report report = null;
      Share.ReportTemplate.cptCSDSaleOrderCancel cpt = new Share.ReportTemplate.cptCSDSaleOrderCancel();
      cpt.SetDataSource(dsCUSSaleorderCancel);
      Utility.ViewCrystalReport(cpt);
      ////Utility.ViewCrystalReport(cpt);
      //report = new DaiCo.Shared.View_Report(cpt);
      //report.IsShowGroupTree = false;
      //string random = GetRandomString();
      //string startupPath = System.Windows.Forms.Application.StartupPath;
      //string filePath = string.Format(@"{0}\Reports\CustomerService{1}_{2}.pdf", startupPath, poCancelPid, random);
      //cpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);
      //// @"+ startupPath + \Reports\CustomerService" + saleOrderPid + "_" + random + ".pdf");
      ////report.ShowReport(DaiCo.Shared.Utility.ViewState.ModalWindow);
      //Process.Start(filePath);
    }
    #endregion Event

  }
}
