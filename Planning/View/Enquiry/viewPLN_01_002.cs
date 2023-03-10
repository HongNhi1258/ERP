using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_01_002 : MainUserControl
  {
    public long enquiryPid = long.MinValue;
    DataTable dtSource = new DataTable();
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    bool isLock = false;
    private long customerPid = long.MinValue;
    private long directPid = long.MinValue;
    private int rowGrid = int.MinValue;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

    public viewPLN_01_002()
    {
      InitializeComponent();
    }

    private void viewPLN_01_002_Load(object sender, EventArgs e)
    {
      this.LoadDropdownComponentCode();
      this.LoadData();
    }

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
      if (this.enquiryPid != long.MinValue)
      {
        PLNEnquiry enquiry = new PLNEnquiry();
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
        chkLock.Checked = (enquiry.Status == 2 || enquiry.Status == 3);
        txtType.Text = FunctionUtility.GetCodeMasterTitle(ConstantClass.GROUP_SALEORDERTYPE, enquiry.Type);
        txtRemark.Text = enquiry.Remark.Trim();
        txtDeliveryRequirement.Text = enquiry.DeliveryRequirement;
        txtPackingRequirement.Text = enquiry.PackingRequirement;
        txtDocumentRequirement.Text = enquiry.DocumentRequirement;
        if (enquiry.CSConfirmDate != DateTime.MinValue)
        {
          ultCSConfirmDate.Text = enquiry.CSConfirmDate.ToString().Trim();
        }
        else
        {
          ultCSConfirmDate.Text = "";
        }
        if (chkLock.Checked == true)
        {
          chkLock.Enabled = false;
          btnSave.Enabled = false;
          btnCopy.Enabled = false;
        }
        txtRemark.ReadOnly = true;
        chkCancel.Checked = (enquiry.CancelFlag == 1);
        chkKeep.Checked = (enquiry.Keep == 1);
        uC_CustomerQuota1.CustomerPid = enquiry.CustomerPid;
        uC_CustomerQuota1.CustomerDirectId = enquiry.DirectPid;
        uC_CustomerQuota1.DateFrom = enquiry.OrderDate < DateTime.Now ? enquiry.OrderDate : DateTime.Now;
        uC_CustomerQuota1.ShowData();
      }

      this.LoadGrid();
    }

    private void LoadGrid()
    {
      this.listDeletedPid = new ArrayList();
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@EnPid", DbType.Int64, this.enquiryPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNListEnquiryDetailByEnPid", inputParam);
      DataSet dsData = Shared.Utility.CreateDataSet.EnquiryConfirm();
      dsData.Tables["dtParent"].Merge(dsSource.Tables[0]);
      dsData.Tables["dtChild"].Merge(dsSource.Tables[1]);
      ultDetail.DataSource = dsData;
      int numberExpire = 0;
      string commandText = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 1003 AND Code = 2";
      object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
      numberExpire = (obj != null) ? DBConvert.ParseInt(obj.ToString()) : 0;

      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        DateTime requestDate = DateTime.MinValue;
        ultDetail.Rows[i].Cells["Select"].Value = 0;
        if (row.Cells["RequestDate"].Value.ToString().Trim().Length > 0)
        {
          requestDate = (DateTime)row.Cells["RequestDate"].Value;
        }
        //DateTime requestDate = DBConvert.ParseDateTime(row.Cells["RequestDate"].Value.ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
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
            //DateTime scheduleDate = DBConvert.ParseDateTime(childRow.Cells["ScheduleDate"].Value.ToString(), Shared.Utility.ConstantClass.FORMAT_DATETIME);
            if (scheduleDate.AddDays(numberExpire) > requestDate)
            {
              ultDetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
              break;
            }
          }
        }
      }

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
            ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Activation = Activation.ActivateOnly;
            ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["ScheduleDate"].Activation = Activation.ActivateOnly;
            ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["NonPlan"].Activation = Activation.ActivateOnly;
            if (DBConvert.ParseInt(ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["NonPlan"].Value.ToString()) == 1)
            {
              ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["Expire"].Activation = Activation.ActivateOnly;
              ultDetail.Rows[i].ChildBands[0].Rows[j].Cells["Keep"].Activation = Activation.ActivateOnly;
            }
          }
        }
        ultDetail.DisplayLayout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
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
    }

    #endregion LoadData

    #region Methods
    private bool CheckIsValid()
    {
      int count = ultDetail.Rows.Count;
      for (int i = 0; i < count; i++)
      {
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
            return false;
          }
        }
        // Kiem tra ScheduleDate
        for (int j = 0; j < countChild; j++)
        {
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
            return false;
          }
          int nonePlan = DBConvert.ParseInt(rowChild.Cells["NonPlan"].Value.ToString());
          nonePlan = (nonePlan == int.MinValue ? 0 : nonePlan);
          if (nonePlan == 0 && scheduleDate == DateTime.MinValue)
          {
            WindowUtinity.ShowMessageWarning("MSG0005", "ScheduleDate");
            return false;
          }
        }
      }
      return true;
    }

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

    #endregion Methods

    #region Event

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.Save();
    }

    private void ultDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Name"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["RequestDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["RequestDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["RequestDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Remark"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "Request Date";
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Price"].Hidden = true;
      e.Layout.Bands[0].Columns["Amount"].Hidden = true;
      e.Layout.Bands[0].Columns["SecondPrice"].Hidden = true;
      e.Layout.Bands[0].Columns["SecondAmount"].Hidden = true;
      e.Layout.Bands[0].Columns["RequiredShipDate"].Hidden = true;
      //e.Layout.Bands[0].Columns["SpecialInstruction"].Hidden = true;
      e.Layout.Bands[0].Columns["Unit"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].Hidden = true;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["EnquiryDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ScheduleDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["ScheduleDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["ScheduleDate"].Header.Caption = "Confirm Ship Date";
      e.Layout.Bands[1].Columns["ScheduleDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Expire"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Keep"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["NonPlan"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["NonPlan"].Header.Caption = "Non Plan";
      e.Layout.Bands[0].Columns["Select"].CellActivation = Activation.AllowEdit;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
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
      //e.Layout.Override.CellClickAction = CellClickAction.RowSelect;
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
      try
      {
        UltraGridRow row = (ultDetail.Selected.Rows[0].ParentRow == null) ? ultDetail.Selected.Rows[0] : ultDetail.Selected.Rows[0].ParentRow;
        object pid = row.Cells["Pid"].Value;
        int qty = DBConvert.ParseInt(row.Cells["Qty"].Value.ToString());
        if (pid != DBNull.Value)
        {
          viewPLN_01_007 ms = new viewPLN_01_007();
          ms.enquiryPid = (long)pid;
          if (qty != int.MinValue)
          {
            ms.qty = qty;
          }
          WindowUtinity.ShowView(ms, "Master Planning", false, ViewState.ModalWindow);
          if (ultDetail.Selected.Rows[0].ParentRow == null)
          {
            rowGrid = ultDetail.Selected.Rows[0].Index;
          }
          else
          {
            rowGrid = ultDetail.Selected.Rows[0].ParentRow.Index;
          }
          this.LoadGrid();
        }
      }
      catch { }
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
      DataSet ds = (DataSet)ultDetail.DataSource;
      DataTable dtMain = new DataTable();
      for (int c = 1; c < ds.Tables[0].Columns.Count; c++)
      {
        dtMain.Columns.Add(ds.Tables[0].Columns[c].ColumnName, ds.Tables[0].Columns[c].DataType);
      }
      for (int c = 2; c < ds.Tables[1].Columns.Count; c++)
      {
        dtMain.Columns.Add(ds.Tables[1].Columns[c].ColumnName + "_Detail", ds.Tables[1].Columns[c].DataType);
      }

      for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
      {
        DataRow dr = dtMain.NewRow();
        for (int c = 1; c < ds.Tables[0].Columns.Count; c++)
        {
          dr[ds.Tables[0].Columns[c].ColumnName] = ds.Tables[0].Rows[i][ds.Tables[0].Columns[c].ColumnName];
        }
        dtMain.Rows.Add(dr);
        for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
        {
          if (ds.Tables[1].Rows[j]["EnquiryDetailPid"].ToString() == ds.Tables[0].Rows[i]["Pid"].ToString())
          {
            DataRow drChild = dtMain.NewRow();
            for (int c = 2; c < ds.Tables[1].Columns.Count; c++)
            {
              drChild[ds.Tables[1].Columns[c].ColumnName + "_Detail"] = ds.Tables[1].Rows[j][ds.Tables[1].Columns[c].ColumnName];
            }
            dtMain.Rows.Add(drChild);
            //break;
          }
        }
      }
      if (dtMain.Rows.Count > 0)
      {
        exportDataToExcel("Enquiry Information", dtMain);
      }
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
      int socot = dt.Columns.Count;
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
      DateTime confirmshipdate = new DateTime();
      if (ultConfirmShipDate.Value == null || DBConvert.ParseDateTime(ultConfirmShipDate.Value.ToString(), formatConvert) == DateTime.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Confirm Ship Date");
        return;
      }
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultDetail.Rows[i].Cells["Select"].Value.ToString()) == 1)
        {
          string commandText = "SELECT Pid  FROM TblPlnEnquiryConfirmDetail WHERE EnquiryDetailPid = " + DBConvert.ParseLong(ultDetail.Rows[i].Cells["Pid"].Value.ToString());
          confirmshipdate = DBConvert.ParseDateTime(ultConfirmShipDate.Value.ToString(), formatConvert);
          DBParameter[] input = new DBParameter[5];
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          input[1] = new DBParameter("@EnquiryDetailPid", DbType.Int64, DBConvert.ParseLong(ultDetail.Rows[i].Cells["Pid"].Value.ToString()));
          input[2] = new DBParameter("@ScheduleDate", DbType.DateTime, confirmshipdate);
          input[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultDetail.Rows[i].Cells["Qty"].Value.ToString()));
          input[4] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          if (DataBaseAccess.ExecuteScalarCommandText(commandText) != null)
          {
            input[0] = new DBParameter("@Pid", DbType.Int64, DataBaseAccess.ExecuteScalarCommandText(commandText));
          }
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNEnquiryConfirm_Edit", input, output);
        }
      }
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
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

    #endregion Event
  }
}
