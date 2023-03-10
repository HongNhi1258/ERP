/*
 * Author       : TRAN HUNG
 * CreateDate   : 21/12/2012
 * Description  : ENQUIRY INFORMATION
 * Update (21/07/2015) Add Col Return
 */
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_10_010 : MainUserControl
  {
    #region Field
    public long enquiryPid = long.MinValue;
    DataTable dtSource = new DataTable();
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    bool isLock = false;
    private long customerPid = long.MinValue;
    private long directPid = long.MinValue;
    private int rowGrid = int.MinValue;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    DataTable dtMasterPlanData = new DataTable();
    private int status = int.MinValue;
    #endregion Feild

    #region LoadData

    private void LoadDropdownComponentCode()
    {
      string commandText = string.Format(@"SELECT INF.ItemCode + '|' + CAST(INF.Revision AS VARCHAR) Value, 
	                                                INF.ItemCode, INF.Revision, BS.Name, BS.NameVN
                                           FROM TblBOMItemInfo INF INNER JOIN TblBOMItemBasic BS ON (INF.ItemCode = BS.ItemCode)
                                           ORDER BY INF.ItemCode, INF.Revision");
      udrpItemCode.DataSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpItemCode.ValueMember = "Value";
      udrpItemCode.DisplayMember = "ItemCode";
      udrpItemCode.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      udrpItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }

    private void LoadData()
    {
      PLNEnquiry enquiry = new PLNEnquiry();
      if (this.enquiryPid != long.MinValue)
      {
        enquiry.Pid = this.enquiryPid;
        enquiry = (PLNEnquiry)DataBaseAccess.LoadObject(enquiry, new string[] { "Pid" });
        if (enquiry == null)
        {
          WindowUtinity.ShowMessageError("ERR0007");
          WindowUtinity.CloseView(this);
          return;
        }
        txtEnquiryNo.Text = enquiry.EnquiryNo;
        txtCustomerENNo.Text = enquiry.CustomerEnquiryNo;
        CSDCustomerInfo cusLoad = new CSDCustomerInfo();
        cusLoad.Pid = enquiry.CustomerPid;

        // Minhd 15/06/2011 Email 
        this.customerPid = cusLoad.Pid;
        // Minhd 15/06/2011 Email 

        CSDCustomerInfo cus = (CSDCustomerInfo)DataBaseAccess.LoadObject(cusLoad, new string[] { "Pid" });
        if (cus != null)
        {
          txtCustomer.Text = cus.Name;
        }
        cusLoad.Pid = enquiry.DirectPid;

        // Minhd 15/06/2011 Email 
        this.directPid = cusLoad.Pid;
        // Minhd 15/06/2011 Email 

        CSDCustomerInfo cusDi = (CSDCustomerInfo)DataBaseAccess.LoadObject(cusLoad, new string[] { "Pid" });
        if (cusDi != null)
        {
          txtDirect.Text = cusDi.Name;
        }
        txtENDate.Text = enquiry.OrderDate.ToString(ConstantClass.FORMAT_DATETIME);
        this.status = enquiry.Status;
        chkLock.Checked = (enquiry.Status == 2 || enquiry.Status == 3);
        txtType.Text = FunctionUtility.GetCodeMasterTitle(ConstantClass.GROUP_SALEORDERTYPE, enquiry.Type);
        txtRemark.Text = enquiry.Remark.Trim();
        txtDeliveryRequirement.Text = enquiry.DeliveryRequirement;
        txtPackingRequirement.Text = enquiry.PackingRequirement;
        txtDocumentRequirement.Text = enquiry.DocumentRequirement;
        txtShipmentTerms.Text = enquiry.ShipmentTerms;
        txtPaymentTerms.Text = enquiry.PaymentTerms;
        txtEnvironmentStatus.Text = enquiry.EnvironmentStatus;
        if (chkLock.Checked == true)
        {
          chkLock.Enabled = false;
          //btnSave.Enabled = false;
          btnReturnToCS.Enabled = false;
          btnCopy.Enabled = false;
        }
        txtRemark.ReadOnly = true;
        chkCancel.Checked = (enquiry.CancelFlag == 1);
        chkKeep.Checked = (enquiry.Keep == 1);
      }


      try
      {
        int isLoadMasterPlan = 0;
        if (this.dtMasterPlanData == null || this.dtMasterPlanData.Columns.Count == 0)
        {
          isLoadMasterPlan = 1;
        }
        if (enquiry.Status >= 2)
        {
          isLoadMasterPlan = 0;
        }
        this.LoadGrid(new DataSet(), isLoadMasterPlan);
      }
      catch
      { }
    }

    private void LoadGrid(DataSet dsSource, int LoadMasterPlan)
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[2];
      inputParam[0] = new DBParameter("@EnPid", DbType.Int64, this.enquiryPid);

      LoadMasterPlan = 0;
      if (LoadMasterPlan != 0)
      {
        inputParam[1] = new DBParameter("@IsLoadMasterPlanData", DbType.Int32, LoadMasterPlan);
      }
      DataSet dsData = Shared.Utility.CreateDataSet.EnquiryConfirmAnswer();
      if (dsSource.Tables.Count == 0)
      {
        dsSource = DataBaseAccess.SearchStoreProcedure("spPLNListEnquiryDetailByEnPidAnswer", 500, inputParam);
        if ((this.dtMasterPlanData == null || this.dtMasterPlanData.Columns.Count == 0))
        {
          try
          {
            dsData.Tables["dtChildTwo"].Merge(dsSource.Tables[3]);
          }
          catch
          {
            dsData.Tables["dtChildTwo"].Merge(this.dtMasterPlanData);
          }
        }
        else
        {
          dsData.Tables["dtChildTwo"].Merge(this.dtMasterPlanData);
        }
      }
      //else
      //{
      //  dsData.Tables["dtChildTwo"].Merge(dsSource.Tables[3]);
      //}
      dsData.Tables["dtParent"].Merge(dsSource.Tables[0]);
      dsData.Tables["dtChild"].Merge(dsSource.Tables[1]);
      dsData.Tables["dtChildDiffEn"].Merge(dsSource.Tables[2]);
      ultDetail.DataSource = dsData;

      int numberExpire = 0;
      string commandText = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 1003 AND Code = 2";
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      numberExpire = (obj != null) ? DBConvert.ParseInt(obj.ToString()) : 0;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        int childqty = 0;
        for (int x = 0; x < ultDetail.Rows[i].ChildBands[0].Rows.Count; x++)
        {
          if (ultDetail.Rows[i].ChildBands[0].Rows.Count > 0)
          {
            childqty += DBConvert.ParseInt(ultDetail.Rows[i].ChildBands[0].Rows[x].Cells["Qty"].Value.ToString());
          }
        }
        int remain = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()) - childqty;
        if (remain > 0)
        {
          DataRow newRow = dsSource.Tables[1].NewRow();
          newRow["EnquiryDetailPid"] = row.Cells["Pid"].Value;
          newRow["Qty"] = remain;
          newRow["Expire"] = 0;
          newRow["Keep"] = 0;
          newRow["NonPlan"] = 0;
          dsSource.Tables[1].Rows.Add(newRow);
        }
        DateTime requestDate = DateTime.MinValue;
        ultDetail.Rows[i].Cells["Select"].Value = 0;
        if (row.Cells["RequestDate"].Value.ToString().Trim().Length > 0)
        {
          requestDate = (DateTime)row.Cells["RequestDate"].Value;
        }
        if (requestDate != DateTime.MinValue)
        {
          for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
          {
            UltraGridRow childRow = row.ChildBands[0].Rows[j];
            DateTime scheduleDate = DateTime.MinValue;
            if (childRow.Cells["ScheduleDate"].Value.ToString().Trim().Length > 0)
            {
              scheduleDate = (DateTime)childRow.Cells["ScheduleDate"].Value;
            }
            if (scheduleDate.AddDays(numberExpire) > requestDate)
            {
              ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
              break;
            }
          }
        }
        for (int a = 0; a < ultDetail.Rows[i].ChildBands[1].Rows.Count; a++)
        {
          ultDetail.Rows[i].ChildBands[1].Rows[a].CellAppearance.BackColor = Color.Wheat;
          ultDetail.Rows[i].ChildBands[1].Rows[a].Activation = Activation.ActivateOnly;
        }

        for (int a = 0; a < ultDetail.Rows[i].ChildBands[2].Rows.Count; a++)
        {
          ultDetail.Rows[i].ChildBands[2].Rows[a].CellAppearance.BackColor = Color.GreenYellow;
          ultDetail.Rows[i].ChildBands[2].Rows[a].Activation = Activation.ActivateOnly;
        }
      }
      DataSet data = Shared.Utility.CreateDataSet.EnquiryConfirmAnswer();
      data.Tables["dtParent"].Merge(dsSource.Tables[0]);
      data.Tables["dtChild"].Merge(dsSource.Tables[1]);
      data.Tables["dtChildDiffEn"].Merge(dsSource.Tables[2]);
      try
      {
        if (dsSource.Tables.Count == 3 || dsSource.Tables[3] == null || dsSource.Tables[3].Rows.Count == 0)
        {
          data.Tables["dtChildTwo"].Merge(this.dtMasterPlanData);
        }
        else
        {
          data.Tables["dtChildTwo"].Merge(dsSource.Tables[3]);
        }
      }
      catch
      { }
      ultDetail.DataSource = data;
      PLNEnquiry enquiry = new PLNEnquiry();
      enquiry.Pid = this.enquiryPid;
      enquiry = (PLNEnquiry)DataBaseAccess.LoadObject(enquiry, new string[] { "Pid" });
      if (enquiry == null)
      {
        return;
      }
      if ((enquiry.Status == 2) || enquiry.Status == 3)
      {
        isLock = true;
        for (int i = 0; i < ultDetail.Rows.Count; i++)
        {
          for (int j = 0; j < ultDetail.Rows[i].ChildBands[0].Rows.Count; j++)
          {
            ultDetail.Rows[i].ChildBands[0].Rows[j].Activation = this.chkLock.Checked ? Activation.ActivateOnly : Activation.AllowEdit;
            ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Activation = Activation.ActivateOnly;
            ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["ScheduleDate"].Activation = Activation.ActivateOnly;
            ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["NonPlan"].Activation = Activation.ActivateOnly;
            if (DBConvert.ParseInt(ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["NonPlan"].Value.ToString()) == 1)
            {
              ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["Expire"].Activation = Activation.ActivateOnly;
              ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["Keep"].Activation = Activation.ActivateOnly;
            }
          }
          for (int a = 0; a < ultDetail.Rows[i].ChildBands[0].Rows.Count; a++)
          {
            if (ultDetail.Rows[i].ChildBands[0].Rows[a].Cells["Duplicated"].Value.ToString() == "0")
            {
              ultDetail.Rows[i].ChildBands[0].Rows[a].CellAppearance.BackColor = Color.White;
              ultDetail.Rows[i].CellAppearance.BackColor = Color.White;
            }
            else
            {
              ultDetail.Rows[i].ChildBands[0].Rows[a].Appearance.BackColor = Color.Yellow;
              ultDetail.Rows[i].Appearance.BackColor = Color.Yellow;
            }
          }
        }
        //ultDetail.DisplayLayout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      }
      DataTable dtColumn = new DataTable();
      if (dsSource != null)
      {
        dtColumn.Columns.Add("ColName");
        if (dsSource.Tables[0].Rows.Count > 0)
        {
          for (int i = 0; i < dsSource.Tables[0].Columns.Count; i++)
          {
            try
            {
              if (ultDetail.Rows.Band.Columns[dsSource.Tables[0].Columns[i].ColumnName].Hidden != true)
              {
                DataRow dr = dtColumn.NewRow();
                dr["ColName"] = dsSource.Tables[0].Columns[i].ColumnName;
                dtColumn.Rows.Add(dr);
              }
            }
            catch { }
          }
        }
        if (dsSource.Tables[1].Rows.Count > 0)
        {
          for (int i = 0; i < dsSource.Tables[1].Columns.Count; i++)
          {
            try
            {
              if (ultDetail.Rows.Band.Columns[dsSource.Tables[0].Columns[i].ColumnName].Hidden != true)
              {
                DataRow dr = dtColumn.NewRow();
                dr["ColName"] = dsSource.Tables[1].Columns[i].ColumnName;
                dtColumn.Rows.Add(dr);
              }
            }
            catch { }
          }
        }
      }
      if (rowGrid > 0)
      {
        ultDetail.Rows[rowGrid].Selected = true;
        ultDetail.ActiveRowScrollRegion.ScrollRowIntoView(ultDetail.Rows[rowGrid]);
      }

      this.LoadColumnName(dsData.Tables["dtChildTwo"]);
    }

    public viewPLN_10_010()
    {
      InitializeComponent();
    }

    private void viewPLN_01_002_Load(object sender, EventArgs e)
    {
      this.LoadDropdownComponentCode();
      this.LoadData();
    }

    #endregion LoadData

    #region Methods
    private bool CheckIsValid()
    {
      int count = ultDetail.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        ultDetail.Rows[i].CellAppearance.BackColor = Color.White;
        UltraGridRow row = ultDetail.Rows[i];
        int countChild = row.ChildBands[0].Rows.Count;
        if (chkLock.Checked)
        {
          string itemCodeCheck = string.Empty;
          long totalQty = DBConvert.ParseLong(row.Cells["Qty"].Value.ToString());
          long checkQty = 0;
          for (int k = 0; k < countChild; k++)
          {
            UltraGridRow rowChild = row.ChildBands[0].Rows[k];
            long q = DBConvert.ParseInt(rowChild.Cells["Qty"].Value.ToString());
            if (q <= 0)
            {
              WindowUtinity.ShowMessageWarning("ERR0001", "Qty");
              return false;
            }
            checkQty += q;
          }
          // Kiem tra tong cac Qty xem co bang totalQty khong?
          if (checkQty != totalQty)
          {
            itemCodeCheck = DBConvert.ParseString(row.Cells["ItemCode"].Value);
            if (checkQty > totalQty)
            {
              WindowUtinity.ShowMessageError("ERR0026", string.Format("Total Qty at item code = {0}", itemCodeCheck));
            }
            else
            {
              WindowUtinity.ShowMessageError("ERR0027", string.Format("Total Qty at item code = {0}", itemCodeCheck));
            }
            ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
            return false;
          }
        }
        // Kiem tra ScheduleDate
        if (this.chkLock.Checked)
        {
          for (int j = 0; j < countChild; j++)
          {
            ultDetail.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.White;
            UltraGridRow rowChild = row.ChildBands[0].Rows[j];
            object obj = rowChild.Cells["ScheduleDate"].Value;
            DateTime scheduleDate = (obj == DBNull.Value ? DateTime.MinValue : (DateTime)obj);
            int dem = 0;
            for (int h = 0; h < countChild; h++)
            {
              UltraGridRow rowChildDup = row.ChildBands[0].Rows[h];
              object objDup = rowChildDup.Cells["ScheduleDate"].Value;
              DateTime scheduleDateDup = (objDup == DBNull.Value ? DateTime.MinValue : (DateTime)objDup);
              if (scheduleDate == scheduleDateDup)
              {
                dem++;
              }
            }
            if (dem >= 2)
            {
              WindowUtinity.ShowMessageWarning("ERR0013", "ScheduleDate");
              ultDetail.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Yellow;
              return false;
            }
            int nonePlan = DBConvert.ParseInt(rowChild.Cells["NonPlan"].Value.ToString());
            nonePlan = (nonePlan == int.MinValue ? 0 : nonePlan);
            if (nonePlan == 0 && scheduleDate == DateTime.MinValue)
            {
              WindowUtinity.ShowMessageWarning("MSG0005", "ScheduleDate");
              ultDetail.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Yellow;
              return false;
            }
          }
        }
      }
      return true;
    }

    private bool PLNKeepDay()
    {
      return true;
    }

    /// <summary>
    ///  Save Data
    /// </summary>
    private void Save()
    {
      if (this.CheckIsValid())
      {
        bool success = true;
        int count = ultDetail.Rows.Count;
        for (int i = 0; i < count; i++)
        {
          UltraGridRow row = ultDetail.Rows[i];

          long totalQty = DBConvert.ParseLong(row.Cells["Qty"].Value.ToString());

          int countChild = row.ChildBands[0].Rows.Count;
          if (countChild > 0)
          {
            for (int j = 0; j < countChild; j++)
            {
              UltraGridRow rowChild = row.ChildBands[0].Rows[j];
              PLNEnquiryConfirmDetail enConfirmDT = new PLNEnquiryConfirmDetail();

              enConfirmDT.EnquiryDetailPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

              long pidConfirm = DBConvert.ParseLong(rowChild.Cells["Pid"].Value.ToString());
              if (pidConfirm == long.MinValue)
              {
                enConfirmDT.CreateDate = DateTime.Today;
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
                  return;
                }
                enConfirmDT.UpdateDate = DateTime.Now;
                enConfirmDT.UpdateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
              }

              enConfirmDT.Qty = DBConvert.ParseInt(rowChild.Cells["Qty"].Value.ToString());

              object obj = rowChild.Cells["ScheduleDate"].Value;
              if (obj != DBNull.Value)
              {
                enConfirmDT.ScheduleDate = (DateTime)obj;
              }
              else
              {
                enConfirmDT.ScheduleDate = DateTime.MinValue;
              }

              int nonePlan = DBConvert.ParseInt(rowChild.Cells["NonPlan"].Value.ToString());
              nonePlan = (nonePlan == int.MinValue ? 0 : nonePlan);
              enConfirmDT.NonPlan = nonePlan;
              if (nonePlan == 0)
              {
                int expire = DBConvert.ParseInt(rowChild.Cells["Expire"].Value.ToString());
                enConfirmDT.Expire = (expire == int.MinValue ? 0 : expire);

                int keep = DBConvert.ParseInt(rowChild.Cells["Keep"].Value.ToString());
                enConfirmDT.Keep = (keep == int.MinValue ? 0 : keep);
              }
              else
              {
                enConfirmDT.Expire = 0;
                enConfirmDT.Keep = 0;
                enConfirmDT.ScheduleDate = DateTime.MinValue;
              }
              enConfirmDT.Remark = rowChild.Cells["Remark"].Value.ToString().Trim();

              if (enConfirmDT.Pid == long.MinValue)
              {
                long result = DataBaseAccess.InsertObject(enConfirmDT);
                //result = MainBOMLibary.InsertObject(enConfirmDT);
                if (result == long.MinValue)
                {
                  success = false;
                }
              }
              else
              {
                bool result = DataBaseAccess.UpdateObject(enConfirmDT, new string[] { "Pid" });
                if (!result)
                {
                  success = false;
                }
              }
            }
          }

          // Minh doc khong hieu source save dang lam gi
          // nen mo 1 duong cap nhat save moi ngay 25/03/2013
          if (this.chkLock.Checked)
          {
            DBParameter[] inputParam = new DBParameter[2];
            inputParam[0] = new DBParameter("@EnquiryNo", DbType.String, this.txtEnquiryNo.Text);
            inputParam[1] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

            DataBaseAccess.ExecuteStoreProcedure("spPLNEnquiryPlnConfirm_Update", inputParam);

          }
        }

        foreach (long pidDelete in this.listDeletedPid)
        {
          PLNEnquiryConfirmDetail chx = new PLNEnquiryConfirmDetail();
          chx.Pid = pidDelete;
          success = DataBaseAccess.DeleteObject(chx, new string[] { "Pid" });
        }

        if (chkLock.Checked)
        {
          string updateCommand = string.Format("Update TblPLNEnquiry Set Status = {0} Where Pid = {1}", 2, enquiryPid);
          DataBaseAccess.ExecuteCommandText(updateCommand);
        }

        int cancelFlag = (chkCancel.Checked) ? 1 : 0;
        int keepEN = (chkKeep.Checked) ? 1 : 0;
        string updateCommandText = string.Format("Update TblPLNEnquiry Set CancelFlag = {0}, Keep = {1} Where Pid = {2}", cancelFlag, keepEN, enquiryPid);
        DataBaseAccess.ExecuteCommandText(updateCommandText);

        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");

          // Email
          if (chkLock.Checked)
          {
            Email email = new Email();
            if (directPid == long.MinValue)
            {
              email.Key = email.KEY_PLN_001;
            }
            else
            {
              email.Key = email.KEY_PLN_002;
            }

            ArrayList arrList = email.GetDataMain(email.Key);
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
              // Truong Add
              try
              {
                toFromSql = email.GetEmailToFromSql(toFromSql);
              }
              catch
              {
                this.LoadData();
                return;
              }
              // End

              string userName = DaiCo.Shared.Utility.FunctionUtility.BoDau(email.GetNameUserLogin(Shared.Utility.SharedObject.UserInfo.UserPid));
              string subject = string.Format(arrList[1].ToString(), this.txtEnquiryNo.Text, userName);
              string body = string.Format(arrList[2].ToString(), this.txtEnquiryNo.Text, userName);
              email.InsertEmail(email.Key, toFromSql, subject, body);
            }
          }
        }
        else
        {
          WindowUtinity.ShowMessageWarning("WRN0004");
        }

        this.LoadData();
      }
    }

    /// <summary>
    /// Load Column Name
    /// </summary>
    private void LoadColumnName(DataTable dtSource)
    {
      DataTable dtNew = new DataTable();
      DataTable dtColumn = dtSource;
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        dtNew.Columns[column.ColumnName].DefaultValue = 0;

        if (string.Compare(column.ColumnName, "PONo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "PODate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "ConfirmedShipDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "Balance", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "TotalBalance", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "TotalWIP", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "TotalUnrelease", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "TotalUnReleaseSameCarcass", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "WO", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "StatusWIP", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "Item/Box", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "ContainerNo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LoadingDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "WIPStatusForContainer", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      ultShowColumn.DataSource = dtNew;
      ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
    }


    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultDetail.DisplayLayout.Bands[3].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }
    #endregion Methods

    #region Event

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      bool success = false;
      success = this.PLNKeepDay();
      if (success && this.status == 1)
      {
        this.Save();
      }
      else
      {
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      // Load Data
      this.LoadData();
    }

    private void ultDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = this.chkLock.Checked ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[2].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[3].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = this.chkLock.Checked ? AllowAddNew.No : AllowAddNew.TemplateOnBottom;

      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[1].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Select"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Select"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Revision"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[0].Columns["SpecialInstruction"].Header.Caption = "Special Instruction";
      e.Layout.Bands[0].Columns["CarcassSUB"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["RequestDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["RequestDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      //Add Col Return
      e.Layout.Bands[0].Columns["Return"].Header.Caption = "Customer Return\n NG Item Fixing";
      e.Layout.Bands[0].Columns["Return"].Hidden = true;
      e.Layout.Bands[0].Columns["Return"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Request\nShip Date";
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].Hidden = true;
      e.Layout.Bands[0].Columns["Amount"].Hidden = true;
      e.Layout.Bands[0].Columns["SecondPrice"].Hidden = true;
      e.Layout.Bands[0].Columns["SecondAmount"].Hidden = true;
      e.Layout.Bands[0].Columns["RequiredShipDate"].Hidden = true;
      e.Layout.Bands[0].Columns["Unit"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].Hidden = true;
      e.Layout.Bands[1].Columns["Duplicated"].Hidden = true;
      e.Layout.Bands[1].Columns["EnquiryDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["ScheduleDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["ScheduleDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["ScheduleDate"].Header.Caption = "Confirmed\nShip Date";
      e.Layout.Bands[1].Columns["Expire"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Keep"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["NonPlan"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Return"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["NonPlan"].Header.Caption = "Non Plan";
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Keep"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["KeepDays"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Expire"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[2].Columns["EnquiryPid"].Hidden = true;
      e.Layout.Bands[2].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[2].Columns["Revision"].Hidden = true;
      e.Layout.Bands[2].Columns["ScheduleDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[2].Columns["ScheduleDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[2].Columns["ScheduleDate"].Header.Caption = "Confirm Ship Date";
      e.Layout.Bands[2].Columns["CustomerCode"].Header.Caption = "Customer";
      e.Layout.Bands[2].Columns["Expire"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[2].Columns["Keep"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[2].Columns["NonPlan"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[2].Columns["NonPlan"].Header.Caption = "Non Plan";
      e.Layout.Bands[2].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[3].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[3].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[3].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[3].Columns["OldCode"].Header.Caption = "Old Code";
      e.Layout.Bands[3].Columns["CustCode"].Header.Caption = "Customer Code";
      e.Layout.Bands[3].Columns["SpecialRemark"].Header.Caption = "Special Remark";
      e.Layout.Bands[3].Columns["PackingNote"].Header.Caption = "Packing Note";
      e.Layout.Bands[3].Columns["ConfirmedShipDate"].Header.Caption = "Confirmed ShipDate";
      e.Layout.Bands[3].Columns["UrgentNote"].Header.Caption = "Urgent Note";
      e.Layout.Bands[3].Columns["OrderQty"].Header.Caption = "Order Qty";
      e.Layout.Bands[3].Columns["ShippedQty"].Header.Caption = "Shipped Qty";
      e.Layout.Bands[3].Columns["CancelledQty"].Header.Caption = "Cancelled Qty";
      e.Layout.Bands[3].Columns["TotalBalance"].Header.Caption = "Total Balance";
      e.Layout.Bands[3].Columns["TotalWIP"].Header.Caption = "Total WIP";
      e.Layout.Bands[3].Columns["TotalUnrelease"].Header.Caption = "Total Unrelease";
      e.Layout.Bands[3].Columns["TotalUnReleaseSameCarcass"].Header.Caption = "Total UnRelease Same Carcass";
      e.Layout.Bands[3].Columns["StatusWIP"].Header.Caption = "Status WIP";
      e.Layout.Bands[3].Columns["LoadingDate"].Header.Caption = "Loading Date";
      e.Layout.Bands[3].Columns["LoadingQty"].Header.Caption = "Loading Qty";
      e.Layout.Bands[3].Columns["LoadingCBM"].Header.Caption = "Loading CBM";
      e.Layout.Bands[3].Columns["PackingQty"].Header.Caption = "Packing Qty";
      e.Layout.Bands[3].Columns["PackingCBM"].Header.Caption = "Packing CBM";
      e.Layout.Bands[3].Columns["WIPStatusForContainer"].Header.Caption = "WIP Status For Container";
      e.Layout.Bands[3].Columns["RepairQty"].Header.Caption = "Repair Qty";
      e.Layout.Bands[3].Columns["StatusRepair"].Header.Caption = "Status Repair";
      e.Layout.Bands[3].Columns["FGWReceivedDate"].Header.Caption = "FGW Received Date";
      e.Layout.Bands[3].Columns["MCHDeadline"].Header.Caption = "MCH Deadline";
      e.Layout.Bands[3].Columns["SUBDeadline"].Header.Caption = "SUB Deadline";
      e.Layout.Bands[3].Columns["FOUDeadline"].Header.Caption = "FOU Deadline";
      e.Layout.Bands[3].Columns["USStock"].Header.Caption = "US Stock";
      e.Layout.Bands[3].Columns["OrderQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["ShippedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["CancelledQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["TotalBalance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["TotalWIP"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["TotalUnReleaseSameCarcass"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["LoadingQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["UCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["TotalUnrelease"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["LoadingCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["PackingQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["RepairQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["USStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["AVEUS6M"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["AVEUS12M"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[3].Columns["PackingCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[3].Columns["Revision"].Hidden = true;
      e.Layout.Bands[3].Columns["UCBM"].Hidden = true;
      e.Layout.Bands[3].Columns["CustCode"].Hidden = true;
      e.Layout.Bands[3].Columns["SaleNo"].Hidden = true;
      e.Layout.Bands[3].Columns["SpecialRemark"].Hidden = true;
      e.Layout.Bands[3].Columns["PackingNote"].Hidden = true;
      e.Layout.Bands[3].Columns["UrgentNote"].Hidden = true;
      e.Layout.Bands[3].Columns["OrderQty"].Hidden = true;
      e.Layout.Bands[3].Columns["ShippedQty"].Hidden = true;
      e.Layout.Bands[3].Columns["CancelledQty"].Hidden = true;
      e.Layout.Bands[3].Columns["House/Sub"].Hidden = true;
      e.Layout.Bands[3].Columns["LoadingQty"].Hidden = true;
      e.Layout.Bands[3].Columns["PackingQty"].Hidden = true;
      e.Layout.Bands[3].Columns["PackingCBM"].Hidden = true;
      e.Layout.Bands[3].Columns["CarcassSUB"].Hidden = true;
      e.Layout.Bands[3].Columns["LoadingCBM"].Hidden = true;
      e.Layout.Bands[3].Columns["RepairQty"].Hidden = true;
      e.Layout.Bands[3].Columns["StatusRepair"].Hidden = true;
      e.Layout.Bands[3].Columns["FGWReceivedDate"].Hidden = true;
      e.Layout.Bands[3].Columns["MCHDeadline"].Hidden = true;
      e.Layout.Bands[3].Columns["SUBDeadline"].Hidden = true;
      e.Layout.Bands[3].Columns["FOUDeadline"].Hidden = true;
      e.Layout.Bands[3].Columns["USStock"].Hidden = true;
      e.Layout.Bands[3].Columns["AVEUS6M"].Hidden = true;
      e.Layout.Bands[3].Columns["AVEUS12M"].Hidden = true;

      e.Layout.Bands[3].Columns["No"].Header.Fixed = true;
      e.Layout.Bands[3].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[3].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[3].Columns["OldCode"].Header.Fixed = true;
      e.Layout.Bands[3].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Select"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Revision"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Name"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Qty"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CBM"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["TotalCBM"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["RequestDate"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SpecialInstruction"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Remark"].Header.Fixed = true;
      e.Layout.Bands[1].Columns["Qty"].Header.Fixed = true;
      e.Layout.Bands[1].Columns["ScheduleDate"].Header.Fixed = true;
      e.Layout.Bands[1].Columns["Expire"].Header.Fixed = true;
      e.Layout.Bands[1].Columns["Keep"].Header.Fixed = true;
      e.Layout.Bands[1].Columns["KeepDays"].Header.Fixed = true;
      e.Layout.Bands[1].Columns["NonPlan"].Header.Fixed = true;
      e.Layout.Bands[1].Columns["Remark"].Header.Fixed = true;
      e.Layout.Bands[3].Columns["CarcassSUB"].Hidden = true;

      e.Layout.Bands[2].Columns["EnquiryNo"].Header.Fixed = true;
      e.Layout.Bands[2].Columns["CustomerCode"].Header.Fixed = true;
      e.Layout.Bands[2].Columns["Qty"].Header.Fixed = true;
      e.Layout.Bands[2].Columns["ScheduleDate"].Header.Fixed = true;
      e.Layout.Bands[2].Columns["Expire"].Header.Fixed = true;
      e.Layout.Bands[2].Columns["Keep"].Header.Fixed = true;
      e.Layout.Bands[2].Columns["KeepDays"].Header.Fixed = true;
      e.Layout.Bands[2].Columns["NonPlan"].Header.Fixed = true;
      e.Layout.Bands[2].Columns["Remark"].Header.Fixed = true;

      for (int i = 0; i < ultDetail.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        string columnName = ultDetail.DisplayLayout.Bands[0].Columns[i].Header.Caption;
        if (string.Compare(columnName, "select", true) == 0)
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
        {
          ultDetail.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }
    }

    private void ultDetail_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      UltraGridRow row = e.Cell.Row;

      int index = e.Cell.Row.Index;
      switch (columnName)
      {
        case "expire":
          if (DBConvert.ParseInt(row.Cells["Expire"].Value.ToString()) == 1)
          {
            row.Cells["Keep"].Value = 0;
            row.Cells["Keep"].Activation = Activation.NoEdit;
            row.Cells["nonplan"].Activation = Activation.NoEdit;
          }
          else
          {
            row.Cells["Keep"].Activation = Activation.AllowEdit;
            if (!isLock)
              row.Cells["nonplan"].Activation = Activation.AllowEdit;
          }
          break;

        case "keep":
          if (DBConvert.ParseInt(row.Cells["keep"].Value.ToString()) == 1)
          {
            row.Cells["Expire"].Value = 0;
            row.Cells["Expire"].Activation = Activation.NoEdit;
            row.Cells["nonplan"].Value = 0;
            row.Cells["nonplan"].Activation = Activation.NoEdit;
          }
          else
          {
            row.Cells["Expire"].Activation = Activation.AllowEdit;
            if (!isLock)
              row.Cells["nonplan"].Activation = Activation.AllowEdit;
          }
          break;
        case "nonplan":
          if (DBConvert.ParseInt(row.Cells["nonplan"].Value.ToString()) == 1)
          {
            row.Cells["ScheduleDate"].Value = DBNull.Value;
            row.Cells["ScheduleDate"].Activation = Activation.NoEdit;
            row.Cells["Expire"].Value = 0;
            row.Cells["Keep"].Value = 0;
            row.Cells["Expire"].Activation = Activation.NoEdit;
            row.Cells["Keep"].Activation = Activation.NoEdit;
          }
          else
          {
            row.Cells["ScheduleDate"].Activation = Activation.AllowEdit;
            row.Cells["Expire"].Activation = Activation.AllowEdit;
            row.Cells["Keep"].Activation = Activation.AllowEdit;
          }
          break;

        default:
          break;
      }
    }

    private void ultDetail_AfterRowInsert(object sender, Infragistics.Win.UltraWinGrid.RowEventArgs e)
    {
      e.Row.Cells["Expire"].Value = 0;
      e.Row.Cells["Keep"].Value = 0;
      e.Row.Cells["NonPlan"].Value = 0;
    }

    private void ultDetail_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        listDeletedPid.Add(pid);
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

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      ControlUtility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultDetail_MouseClick(object sender, MouseEventArgs e)
    {
      ControlUtility.BOMShowItemImage(ultDetail, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultDetail_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //try
      //{
      //  UltraGridRow row = (ultDetail.Selected.Rows[0].ParentRow == null) ? ultDetail.Selected.Rows[0] : ultDetail.Selected.Rows[0].ParentRow;
      //  object pid = row.Cells["Pid"].Value;
      //  int qty = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
      //  if (pid != DBNull.Value)
      //  {
      //    viewPLN_01_007 ms = new viewPLN_01_007();
      //    ms.enquiryPid = (long)pid;
      //    if (qty != int.MinValue)
      //    {
      //      ms.qty = qty;
      //    }
      //    WindowUtinity.ShowView(ms, "Master Planning", false, ViewState.ModalWindow);
      //    if (ultDetail.Selected.Rows[0].ParentRow == null)
      //    {
      //      rowGrid = ultDetail.Selected.Rows[0].Index;
      //    }
      //    else
      //    {
      //      rowGrid = ultDetail.Selected.Rows[0].ParentRow.Index;
      //    }
      //    this.LoadGrid();
      //  }
      //}
      //catch { }
    }

    private void chkKeep_CheckedChanged(object sender, EventArgs e)
    {
      if (chkKeep.Checked)
      {
        chkCancel.Checked = false;
      }
    }

    private void chkCancel_CheckedChanged(object sender, EventArgs e)
    {
      if (chkCancel.Checked)
      {
        chkKeep.Checked = false;
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultDetail, "Enquiry Detail");
      //DataSet ds = (DataSet)ultDetail.DataSource;
      //DataTable dtMain = new DataTable();
      //for (int c = 1; c < ds.Tables[0].Columns.Count; c++)
      //{
      //  if (ds.Tables[0].Columns[c].ToString() != "CarcassSUB")
      //  {
      //    dtMain.Columns.Add(ds.Tables[0].Columns[c].ColumnName, ds.Tables[0].Columns[c].DataType);
      //  }
      //}
      //for (int c = 2; c < ds.Tables[1].Columns.Count; c++)
      //{
      //  dtMain.Columns.Add(ds.Tables[1].Columns[c].ColumnName + "_Detail", ds.Tables[1].Columns[c].DataType);
      //}

      //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
      //{
      //  DataRow dr = dtMain.NewRow();
      //  for (int c = 1; c < ds.Tables[0].Columns.Count; c++)
      //  {
      //    if (ds.Tables[0].Columns[c].ToString() != "CarcassSUB")
      //    {
      //      dr[ds.Tables[0].Columns[c].ColumnName] = ds.Tables[0].Rows[i][ds.Tables[0].Columns[c].ColumnName];
      //    }
      //  }
      //  dtMain.Rows.Add(dr);
      //  for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
      //  {
      //    if (ds.Tables[1].Rows[j]["EnquiryDetailPid"].ToString() == ds.Tables[0].Rows[i]["Pid"].ToString())
      //    {
      //      DataRow drChild = dtMain.NewRow();
      //      for (int c = 2; c < ds.Tables[1].Columns.Count; c++)
      //      {
      //        drChild[ds.Tables[1].Columns[c].ColumnName + "_Detail"] = ds.Tables[1].Rows[j][ds.Tables[1].Columns[c].ColumnName];
      //      }
      //      dtMain.Rows.Add(drChild);
      //      //break;
      //    }
      //  }
      //}
      //if (dtMain.Rows.Count > 0)
      //{
      //  exportDataToExcel("Enquiry Information", dtMain);
      //}
    }

    static public bool exportDataToExcel(string tieude, DataTable dt)
    {
      bool result = false;
      //khoi tao cac doi tuong Com Excel de lam viec
      Excel.ApplicationClass xlApp;
      Excel.Worksheet xlSheet;
      Excel.Workbook xlBook;
      //doi tuong Trống để thêm  vào xlApp sau đó lưu lại sau
      object missValue = System.Reflection.Missing.Value;
      //khoi tao doi tuong Com Excel moi
      xlApp = new Excel.ApplicationClass();
      xlBook = xlApp.Workbooks.Add(missValue);
      //su dung Sheet dau tien de thao tac
      xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);
      //không cho hiện ứng dụng Excel lên để tránh gây đơ máy
      xlApp.Visible = false;
      int socot = dt.Columns.Count - 1;
      int sohang = dt.Rows.Count;
      int i, j;

      SaveFileDialog f = new SaveFileDialog();
      f.Filter = "Excel file (*.xls)|*.xls";

      if (f.ShowDialog() == DialogResult.OK)
      {
      GoBack:
        ;
        try
        {
          File.Open(f.FileName, FileMode.OpenOrCreate).Close();
        }
        catch
        {
          MessageBox.Show("Already Opened:" + f.FileName + "\nPlease find and close it", "Can not save!");
          goto GoBack;
        }

        //set thuoc tinh cho tieu de
        xlSheet.get_Range("A1", Convert.ToChar(socot + 65) + "1").Merge(false);
        Excel.Range caption = xlSheet.get_Range("A1", Convert.ToChar(socot + 65) + "1");
        caption.Select();
        caption.FormulaR1C1 = tieude;
        //căn lề cho tiêu đề
        caption.HorizontalAlignment = Excel.Constants.xlCenter;
        caption.Font.Bold = true;
        caption.VerticalAlignment = Excel.Constants.xlCenter;
        caption.Font.Size = 15;
        //màu nền cho tiêu đề
        caption.Interior.ColorIndex = 20;
        caption.RowHeight = 30;
        //set thuoc tinh cho cac header
        Excel.Range header = xlSheet.get_Range("A2", Convert.ToChar(socot + 65) + "2");
        header.Select();

        header.HorizontalAlignment = Excel.Constants.xlCenter;
        header.Font.Bold = true;
        header.Font.Size = 10;
        //điền tiêu đề cho các cột trong file excel
        for (i = 0; i < socot; i++)
          xlSheet.Cells[2, i + 2] = dt.Columns[i].ColumnName;
        //dien cot stt
        xlSheet.Cells[2, 1] = "STT";

        //dien du lieu vao sheet
        for (i = 0; i < sohang; i++)
          for (j = 0; j < socot; j++)
          {
            if (j == 1)
            {
              xlSheet.Cells[i + 3, j] = i + 1;
            }
            xlSheet.Cells[i + 3, j + 2] = dt.Rows[i][j];
          }
        //autofit độ rộng cho các cột
        //for (i = 0; i < sohang; i++)
        //  ((Excel.Range)xlSheet.Cells[1, i + 1]).EntireColumn.AutoFit();
        xlSheet.Columns.AutoFit();
        //save file
        xlBook.SaveAs(f.FileName, Excel.XlFileFormat.xlWorkbookNormal, missValue, missValue, missValue, missValue, Excel.XlSaveAsAccessMode.xlExclusive, missValue, missValue, missValue, missValue, missValue);
        xlBook.Close(true, missValue, missValue);
        xlApp.Quit();

        // release cac doi tuong COM
        releaseObject(xlSheet);
        releaseObject(xlBook);
        releaseObject(xlApp);
        result = true;
      }
      return result;
    }

    static public void releaseObject(object obj)
    {
      try
      {
        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        obj = null;
      }
      catch (Exception ex)
      {
        obj = null;
        throw new Exception("Exception Occured while releasing object " + ex.ToString());
      }
      finally
      {
        GC.Collect();
      }
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
      if (ultConfirmShipDate.Value == null && !chkNonPlan.Checked)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Confirm Ship Date");
        return;
      }
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["Select"].Value.ToString()) == 1)
        {
          string commandText = "SELECT Pid  FROM TblPlnEnquiryConfirmDetail WHERE EnquiryDetailPid = " + DBConvert.ParseLong(ultDetail.Rows[i].Cells["Pid"].Value.ToString());

          DBParameter[] input = new DBParameter[6];
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          input[1] = new DBParameter("@EnquiryDetailPid", DbType.Int64, DBConvert.ParseLong(ultDetail.Rows[i].Cells["Pid"].Value.ToString()));
          if (ultConfirmShipDate.Value != null)
          {
            input[2] = new DBParameter("@ScheduleDate", DbType.DateTime, DBConvert.ParseDateTime(ultConfirmShipDate.Value.ToString(), formatConvert));
          }
          input[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultDetail.Rows[i].Cells["Qty"].Value.ToString()));
          input[4] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          if (chkNonPlan.Checked)
          {
            input[5] = new DBParameter("@Nonplan", DbType.Int32, 1);
          }
          else
          {
            input[5] = new DBParameter("@Nonplan", DbType.Int32, 0);
          }
          if (DataBaseAccess.ExecuteScalarCommandText(commandText) != null)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DataBaseAccess.ExecuteScalarCommandText(commandText));
          }
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNEnquiryConfirm_Edit", input, output);
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      // Tâm xóa cho chương trình chạy nhanh, Minh confirm Copy ko can load grid lai,
      this.LoadData();
    }

    private void chkAll_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        if (chkAll.Checked)
        {
          ultDetail.Rows[i].Cells["Select"].Value = 1;
        }
        else
        {
          ultDetail.Rows[i].Cells["Select"].Value = 0;
        }
      }
    }

    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultDetail.Rows.ExpandAll(true);
      }
      else
      {
        ultDetail.Rows.CollapseAll(true);
      }
    }

    private void ultShowColumn_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          ultDetail.DisplayLayout.Bands[3].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          ultDetail.DisplayLayout.Bands[3].Columns[columnName].Hidden = true;
        }
      }
      else
      {
        for (int i = 1; i < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          row.Cells[i].Value = e.Cell.Text;
        }
        this.ChkAll_CheckedChange();
      }
    }

    private void ultShowColumn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      DataTable dtColumn = (DataTable)ultShowColumn.DataSource;
      int count = dtColumn.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      e.Layout.Bands[0].Columns["No"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleCode"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["OldCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassSUB"].Hidden = true;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
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
      btnImport.Enabled = false;

      System.Data.DataTable dt = new DataTable();
      try
      {
        dt = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSetVersion2(txtFileName.Text.Trim(), "SELECT * FROM [Data$A5:F65536]").Tables[0];
      }
      catch
      {
        btnImport.Enabled = true;
        return;
      }
      if (dt != null)
      {
        DataTable dtMain = this.CreateDataTable();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          DataRow row = dtMain.NewRow();
          if (dt.Rows[i]["ItemCode"].ToString().Length > 0)
          {
            row["ItemCode"] = dt.Rows[i]["ItemCode"].ToString();
            row["Revision"] = DBConvert.ParseInt(dt.Rows[i]["Revision"].ToString());
            if (dt.Rows[i]["ConfirmShipDate"].ToString().Length > 0)
            {
              row["ConfirmShipDate"] = DBConvert.ParseDateTime(dt.Rows[i]["ConfirmShipDate"].ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (DBConvert.ParseInt(dt.Rows[i]["NonPlan"].ToString()) == 1)
            {
              row["NonPlan"] = 1;
            }
            else
            {
              row["NonPlan"] = 0;
            }
            if (DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString()) != int.MinValue)
            {
              row["Qty"] = DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString());
            }
            if (dt.Rows[i]["Remark"].ToString().Length > 0)
            {
              row["Remark"] = dt.Rows[i]["Remark"].ToString();
            }
            dtMain.Rows.Add(row);
          }
        }

        // Get DataTable Import
        DataSet result = this.GetDataTableImport(dtMain);
        if (result == null)
        {
          WindowUtinity.ShowMessageError("ERR0105");
          btnImport.Enabled = true;
          return;
        }
        this.LoadGrid(result, 0);
        btnImport.Enabled = true;
      }
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      string templateName = string.Format(@"{0}\ExcelTemplate\Planning\EnquiryConfirmShipDate.xls", startupPath);
      if (File.Exists(templateName))
      {
        string newFileName = string.Format(@"{0}\EnquiryConfirmShipDate.xls", folder);
        if (File.Exists(newFileName))
        {
          newFileName = string.Format(@"{0}\EnquiryConfirmShipDate{1}.xls", folder, DateTime.Now.Ticks);
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

    /// <summary>
    /// Create Data Table
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("ConfirmShipDate", typeof(System.DateTime));
      dt.Columns.Add("NonPlan", typeof(System.Int32));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Get DataTable When Import Data
    /// </summary>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private DataSet GetDataTableImport(DataTable dtSource)
    {
      DataTable dt = new DataTable();
      SqlCommand cm = new SqlCommand("spWIPGetDataImportENConfirmShipDate_Select");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;
      cm.CommandTimeout = 500;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@ImportData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      SqlParameter paraEnPid = cm.CreateParameter();
      paraEnPid.ParameterName = "@EnPid";
      paraEnPid.SqlDbType = SqlDbType.BigInt;
      paraEnPid.Value = this.enquiryPid;

      cm.Parameters.Add(para);
      cm.Parameters.Add(paraEnPid);

      SqlDataAdapter adp = new SqlDataAdapter();
      adp.SelectCommand = cm;
      DataSet result = new DataSet();
      try
      {
        if (cm.Connection.State != ConnectionState.Open)
        {
          cm.Connection.Open();
        }
        adp.Fill(result);
      }
      catch (Exception ex)
      {
        result = null;
        return null;
      }
      return result;
    }

    private void btnReturnToCS_Click(object sender, EventArgs e)
    {
      bool result = true;
      string commandText = string.Format("UPDATE TblPLNEnquiry SET Status = 0, UpdateDate = GETDATE(), UpdateBy = {0} WHERE Pid = {1}", SharedObject.UserInfo.UserPid, this.enquiryPid);
      result = DataBaseAccess.ExecuteCommandText(commandText);
      if (result)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.CloseTab();
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
    }

    private void btnAllocate_Click(object sender, EventArgs e)
    {
      viewPLN_10_050 view = new viewPLN_10_050();
      view.enquiryPid = this.enquiryPid;
      Shared.Utility.WindowUtinity.ShowView(view, "Allocate Furniture for Enquiry", false, Shared.Utility.ViewState.MainWindow);
    }
    #endregion Event
  }
}
