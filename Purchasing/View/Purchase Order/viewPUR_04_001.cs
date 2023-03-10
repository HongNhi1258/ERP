/*
  Author      : Vo Van Duy Qui
  Date        : 29/03/2011
  Description : List PO
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_04_001 : MainUserControl
  {
    #region Field
    bool flagviewPrice = false;
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init Data  
    /// <summary>
    /// Init Form
    /// </summary>
    public viewPUR_04_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_04_001_Load(object sender, EventArgs e)
    {
      this.LoadUltraEmployee(ultComboCreateBy);
      this.LoadUltraStatusPO();
      this.LoadUltraComboGroupInCharge();
      this.LoadUltraComboSupplier();
      drpDateFrom.Value = DBNull.Value;
      drpDateTo.Value = DBNull.Value;

      if (btnPerPrice.Visible)
      {
        this.flagviewPrice = true;
        btnPerPrice.Visible = false;
      }
      else
      {
        this.flagviewPrice = false;
      }
    }
    #endregion Init Data

    #region LoadData & Search

    private void LoadUltraStatusPO()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Waiting For Receiving' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'Waiting A Part For Receiving' Name";
      commandText += " UNION";
      commandText += " SELECT 4 ID, 'Finished' Name";
      commandText += " UNION";
      commandText += " SELECT 5 ID, 'Cancel' Name";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultComboStatus.DataSource = dtSource;
      ultComboStatus.DisplayMember = "Name";
      ultComboStatus.ValueMember = "ID";
      ultComboStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboStatus.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      ultComboStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }
    /// <summary>
    /// Load UltraCombo Employee
    /// </summary>
    private void LoadUltraEmployee(UltraCombo ultCombo)
    {
      string commandText = string.Format("SELECT Pid, CAST(Pid as varchar) + ' - ' + EmpName Description FROM VHRMEmployee WHERE Department = 'PUR' ORDER BY Pid");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);

      ultCombo.DataSource = dtSource;
      ultCombo.ValueMember = "Pid";
      ultCombo.DisplayMember = "Description";
      ultCombo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCombo.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCombo.DisplayLayout.Bands[0].Columns["Description"].Width = 300;
    }

    /// <summary>
    /// Load UltraCombo Group In Charge
    /// </summary>
    private void LoadUltraComboGroupInCharge()
    {
      string commandText = "SELECT Pid, GroupName FROM TblPURStaffGroup WHERE DeleteFlg = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);
      ultComboGroupInCharge.DataSource = dtSource;
      ultComboGroupInCharge.DisplayMember = "GroupName";
      ultComboGroupInCharge.ValueMember = "Pid";
      ultComboGroupInCharge.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboGroupInCharge.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultComboGroupInCharge.DisplayLayout.Bands[0].Columns["GroupName"].Width = 300;

      // Load Group In Charge(User Login)
      string commmadText = string.Format(@" SELECT SG.Pid
                                                FROM TblPURStaffGroup SG
	                                                LEFT JOIN TblPURStaffGroupDetail SGD ON SG.Pid = SGD.[Group]
                                                WHERE SG.DeleteFlg = 0 AND (SG.LeaderGroup = {0} OR SGD.Employee = {1})", SharedObject.UserInfo.UserPid, SharedObject.UserInfo.UserPid);
      DataTable dtGroupInCharge = DataBaseAccess.SearchCommandTextDataTable(commmadText);
      if (dtGroupInCharge != null && dtGroupInCharge.Rows.Count > 0)
      {
        ultComboGroupInCharge.Value = DBConvert.ParseInt(dtGroupInCharge.Rows[0]["Pid"].ToString());
      }
    }

    /// <summary>
    /// Load UltraCombo Code Master
    /// </summary>
    /// <param name="ultCombo"></param>
    /// <param name="group"></param>
    private void LoadUltraComboCodeMst(UltraCombo ultCombo, int group)
    {
      string commandText = string.Format(@"SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", group);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCombo.DataSource = dtSource;
      ultCombo.ValueMember = "Code";
      ultCombo.DisplayMember = "Value";
      ultCombo.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCombo.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCombo.DisplayLayout.Bands[0].Columns["Value"].Width = 300;
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadUltraComboSupplier()
    {
      string commandText = "SELECT Pid, EnglishName + ' - ' + SupplierCode Description FROM TblPURSupplierInfo ";
      commandText += "      WHERE Confirm = 2 AND DeleteFlg = 0 AND LEN(EnglishName) > 2 ORDER BY EnglishName";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);
      ultComboSupplier.DataSource = dtSource;
      ultComboSupplier.DisplayMember = "Description";
      ultComboSupplier.ValueMember = "Pid";
      ultComboSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultComboSupplier.DisplayLayout.Bands[0].Columns["Description"].Width = 400;
    }

    /// <summary>
    /// Search PO Information
    /// </summary>
    private void Search()
    {
      int createBy = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultComboCreateBy));
      int poStatus = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultComboStatus));
      string poNo = txtPONo.Text.Trim();
      string prNo = txtPRNo.Text.Trim();
      string dateFrom = (drpDateFrom.Value == null) ? string.Empty : drpDateFrom.Value.ToString().Trim();
      DateTime poDateFrom = DateTime.MinValue;
      if (dateFrom.Length > 0)
      {
        poDateFrom = (DateTime)drpDateFrom.Value;
      }

      string dateTo = (drpDateTo.Value == null) ? string.Empty : drpDateTo.Value.ToString().Trim();
      DateTime poDateTo = DateTime.MinValue;
      if (dateTo.Length > 0)
      {
        poDateTo = (DateTime)drpDateTo.Value;
      }

      string deliveryDateFrom = (drpDeliveryDateFrom.Value == null) ? string.Empty : drpDeliveryDateFrom.Value.ToString().Trim();
      DateTime poDeliveryDateFrom = DateTime.MinValue;
      if (deliveryDateFrom.Length > 0)
      {
        poDeliveryDateFrom = (DateTime)drpDeliveryDateFrom.Value;
      }

      string deliveryDateTo = (drpDeliveryDateTo.Value == null) ? string.Empty : drpDeliveryDateTo.Value.ToString().Trim();
      DateTime poDeliveryDateTo = DateTime.MinValue;
      if (deliveryDateTo.Length > 0)
      {
        poDeliveryDateTo = (DateTime)drpDeliveryDateTo.Value;
      }
      string material = txtItemCodeName.Text.Trim();
      long groupInCharge = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultComboGroupInCharge));
      long supplier = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultComboSupplier));
      double totalAmountFrom = DBConvert.ParseDouble(txtAmountFrom.Text.Trim());
      double totalAmountTo = DBConvert.ParseDouble(txtAmountTo.Text.Trim());
      string receivingNote = txtReceivingNote.Text;
      DBParameter[] input = new DBParameter[14];
      if (createBy != int.MinValue)
      {
        input[0] = new DBParameter("@CreateBy", DbType.Int32, createBy);
      }
      else
      {
        string text = ultComboCreateBy.Text.Trim();
        if (text.Length > 0)
        {
          input[0] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(text));
        }
      }

      if (poStatus != int.MinValue)
      {
        input[1] = new DBParameter("@StatusPO", DbType.Int32, poStatus);
      }
      else
      {
        string text = ultComboStatus.Text.Trim();
        if (text.Length > 0)
        {
          input[1] = new DBParameter("@StatusPO", DbType.Int32, DBConvert.ParseInt(text));
        }
      }

      if (prNo.Length > 0)
      {
        input[2] = new DBParameter("@PRNo", DbType.AnsiString, 16, prNo);
      }

      if (poNo.Length > 0)
      {
        input[3] = new DBParameter("@PONo", DbType.AnsiString, 16, poNo);
      }

      if (poDateFrom != DateTime.MinValue)
      {
        input[4] = new DBParameter("@PODateFrom", DbType.DateTime, poDateFrom);
      }

      if (poDateTo != DateTime.MinValue)
      {
        poDateTo = (poDateTo != DateTime.MaxValue) ? poDateTo.AddDays(1) : poDateTo;
        input[5] = new DBParameter("@PODateTo", DbType.DateTime, poDateTo);
      }

      if (poDeliveryDateFrom != DateTime.MinValue)
      {
        input[6] = new DBParameter("@DeliveryDateFrom", DbType.DateTime, poDeliveryDateFrom);
      }

      if (poDeliveryDateTo != DateTime.MinValue)
      {
        poDeliveryDateTo = (poDeliveryDateTo != DateTime.MaxValue) ? poDeliveryDateTo.AddDays(1) : poDeliveryDateTo;
        input[7] = new DBParameter("@DeliveryDateTo", DbType.DateTime, poDeliveryDateTo);
      }

      if (material.Length > 0)
      {
        input[8] = new DBParameter("@Material", DbType.String, 256, material);
      }

      if (groupInCharge != long.MinValue)
      {
        input[9] = new DBParameter("@GroupInCharge", DbType.Int64, groupInCharge);
      }
      else
      {
        string text = ultComboGroupInCharge.Text.Trim();
        if (text.Length > 0)
        {
          input[9] = new DBParameter("@GroupInCharge", DbType.Int64, DBConvert.ParseLong(text));
        }
      }

      if (supplier != long.MinValue)
      {
        input[10] = new DBParameter("@Supplier", DbType.Int64, supplier);
      }
      else
      {
        string text = ultComboSupplier.Text.Trim();
        if (text.Length > 0)
        {
          input[10] = new DBParameter("@Supplier", DbType.Int64, DBConvert.ParseLong(text));
        }
      }

      if (totalAmountFrom != double.MinValue)
      {
        input[11] = new DBParameter("@TotalAmountFrom", DbType.Double, totalAmountFrom);
      }

      if (totalAmountTo != double.MinValue)
      {
        input[12] = new DBParameter("@TotalAmountTo", DbType.Double, totalAmountTo);
      }

      if (receivingNote.Length > 0)
      {
        input[13] = new DBParameter("@ReceivingNote", DbType.String, receivingNote);
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURListPO_Select", 300, input);
      DataSet dsList = this.ListPONo();
      dsList.Tables["TblPO"].Merge(ds.Tables[0]);
      dsList.Tables["TblPODetail"].Merge(ds.Tables[1]);
      dsList.Tables["TblPODetailSchedule"].Merge(ds.Tables[2]);
      dsList.Tables["TblPODetailScheduleRec"].Merge(ds.Tables[3]);
      ultData.DataSource = dsList;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int type = DBConvert.ParseInt(ultData.Rows[i].Cells["POStatus"].Value.ToString());
        if (type == 1 || type == 2)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (type == 3 || type == 4)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (type == 5)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightSkyBlue;
        }
        else if (type == 6)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Gray;
        }

        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          ultData.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Plum;
        }
        ultData.Rows[i].Expanded = true;
      }
    }

    private DataSet ListPONo()
    {
      DataSet ds = new DataSet();

      // PO Information
      DataTable taParent = new DataTable("TblPO");
      taParent.Columns.Add("Selected", typeof(System.Int32));
      taParent.Columns.Add("FinishDate", typeof(System.String));
      taParent.Columns.Add("PONo", typeof(System.String));
      taParent.Columns.Add("SupplierName", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      taParent.Columns.Add("TotalMoney", typeof(System.Double));
      taParent.Columns.Add("ApprovedBy", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      ds.Tables.Add(taParent);

      // PO Detail
      DataTable taChild = new DataTable("TblPODetail");
      taChild.Columns.Add("Selected", typeof(System.Int32));
      taChild.Columns.Add("Note", typeof(System.String));
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("PONo", typeof(System.String));
      taChild.Columns.Add("PRNo", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("MaterialName", typeof(System.String));
      taChild.Columns.Add("REFCode", typeof(System.String));
      taChild.Columns.Add("Unit", typeof(System.String));
      taChild.Columns.Add("Status", typeof(System.String));
      taChild.Columns.Add("Quantity", typeof(System.Double));
      taChild.Columns.Add("QtyCancel", typeof(System.Double));
      taChild.Columns.Add("Price", typeof(System.Double));
      taChild.Columns.Add("Currency", typeof(System.String));
      taChild.Columns.Add("VAT", typeof(System.String));
      taChild.Columns.Add("OVER", typeof(System.String));
      taChild.Columns.Add("Remark", typeof(System.String));
      ds.Tables.Add(taChild);

      //PO Detail Schedule
      DataTable taSchedule = new DataTable("TblPODetailSchedule");
      taSchedule.Columns.Add("Pid", typeof(System.Int64));
      taSchedule.Columns.Add("PODetailPid", typeof(System.Int64));
      taSchedule.Columns.Add("Quantity", typeof(System.Double));
      taSchedule.Columns.Add("ReceiptedQty", typeof(System.Double));
      taSchedule.Columns.Add("ExpectDate", typeof(System.String));
      ds.Tables.Add(taSchedule);

      // PO Schedule
      DataTable taRec = new DataTable("TblPODetailScheduleRec");
      taRec.Columns.Add("Pid", typeof(System.Int64));
      taRec.Columns.Add("ReceivingNote", typeof(System.String));
      taRec.Columns.Add("Qty", typeof(System.Double));
      taRec.Columns.Add("CreateDate", typeof(System.String));
      ds.Tables.Add(taRec);

      ds.Relations.Add(new DataRelation("TblPO_TblPODetail", taParent.Columns["PONo"], taChild.Columns["PONo"]));
      ds.Relations.Add(new DataRelation("TblPODetail_TblPODetailSchedule", taChild.Columns["Pid"], taSchedule.Columns["PODetailPid"]));
      ds.Relations.Add(new DataRelation("TblPODetailSchedule_TblPODetailScheduleRec", taSchedule.Columns["Pid"], taRec.Columns["Pid"]));
      return ds;
    }

    /// <summary>
    ///  Cancel PO
    /// </summary>
    /// <returns></returns>
    private bool CancelPO()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowInfo = ultData.Rows[i];
        for (int j = 0; j < rowInfo.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowDetail = rowInfo.ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(rowDetail.Cells["Selected"].Value.ToString()) == 1)
          {
            DBParameter[] inputParam = new DBParameter[2];
            inputParam[0] = new DBParameter("@PONo", DbType.String, rowInfo.Cells["PONo"].Value.ToString());
            inputParam[1] = new DBParameter("@PODetailPid", DbType.Int64, DBConvert.ParseLong(rowDetail.Cells["Pid"].Value.ToString()));
            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);
            DataBaseAccess.ExecuteStoreProcedure("spPURStatusPRPOWhenCancelPO_Update", inputParam, outputParam);
            int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
            if (result == 0)
            {
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Print Report
    /// </summary>
    /// <param name="poNo"></param>
    //private void PrintReport(string poNo)
    //{
    //  DBParameter[] input = new DBParameter[1];
    //  input[0] = new DBParameter("@PONo", DbType.AnsiString, 16, poNo);
    //  DataSet ds = DataBaseAccess.SearchStoreProcedure("spPURRPTPOInfomation_Select", input);
    //  if (ds != null)
    //  {
    //    dsPURPOInfomation dsSource = new dsPURPOInfomation();
    //    dsSource.Tables["dtPOInfo"].Merge(ds.Tables[0]);
    //    dsSource.Tables["dtPODetail"].Merge(ds.Tables[1]);
    //    if (chkWithAddition.Checked)
    //    {
    //      dsSource.Tables["dtPOAdditionPrice"].Merge(ds.Tables[2]);
    //    }
    //    dsSource.Tables["dtListPR"].Merge(ds.Tables[3]);

    //    DaiCo.Shared.View_Report report = null;
    //    cptPURPOInformation cpt = new cptPURPOInformation();
    //    double totalAmount = 0;
    //    double totalAdditionPrice = 0;
    //    double totalVAT = 0;
    //    double total = 0;
    //    string orderRemark = string.Empty;
    //    // Total Amount
    //    for (int i = 0; i < dsSource.Tables["dtPODetail"].Rows.Count; i++)
    //    {
    //      DataRow row = dsSource.Tables["dtPODetail"].Rows[i];
    //      totalAmount = totalAmount + DBConvert.ParseDouble(row["Amount"].ToString());
    //      if (row["VAT"].ToString().Length > 0)
    //      {
    //        totalVAT = totalVAT + ((DBConvert.ParseDouble(row["Amount"].ToString()) * DBConvert.ParseDouble(row["VAT"].ToString())) / 100);
    //      }
    //    }
    //    // Total Addition Price
    //    for (int j = 0; j < dsSource.Tables["dtPOAdditionPrice"].Rows.Count; j++)
    //    {
    //      DataRow row = dsSource.Tables["dtPOAdditionPrice"].Rows[j];
    //      totalAdditionPrice = totalAdditionPrice + DBConvert.ParseDouble(row["Amount"].ToString());
    //    }
    //    // Total
    //    total = totalAmount + totalAdditionPrice + totalVAT;
    //    // Number To English
    //    string numberToEnglish = Shared.Utility.NumberToEnglish.ChangeNumericToWords(total) + "(" + dsSource.Tables["dtPOInfo"].Rows[0]["Currency"].ToString() + ")";
    //    // Order Remark
    //    if (dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString().Length > 0)
    //    {
    //      orderRemark = dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString();
    //      //DateTime date = DateTime.MinValue;
    //      //DateTime dateHtml = DateTime.MinValue;
    //      //date = DBConvert.ParseDateTime(dsSource.Tables["dtPOInfo"].Rows[0]["CreateDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
    //      //string commandText1 = "SELECT Value FROM TblBOMCodeMaster WHERE [Group] = 9009";
    //      //DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText1);
    //      //if (dt != null)
    //      //{
    //      //  dateHtml = DBConvert.ParseDateTime(dt.Rows[0]["Value"].ToString(), USER_COMPUTER_FORMAT_DATETIME);
    //      //}
    //      //if (date >= dateHtml)
    //      //{
    //      //  orderRemark = dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString();
    //      //}
    //      //else
    //      //{
    //      //  orderRemark = this.StripHTML(dsSource.Tables["dtPOInfo"].Rows[0]["OrderRemark"].ToString());
    //      //}
    //      cpt.ReportFooterSection3.SectionFormat.EnableSuppress = false;
    //    }
    //    else
    //    {
    //      cpt.ReportFooterSection3.SectionFormat.EnableSuppress = true;
    //    }

    //    // Remark Detail
    //    if (dsSource.Tables["dtPOInfo"].Rows[0]["RemarkDetailEN"].ToString().Length > 0 ||
    //        dsSource.Tables["dtPOInfo"].Rows[0]["RemarkDetailVN"].ToString().Length > 0)
    //    {
    //      cpt.ReportFooterSection6.SectionFormat.EnableSuppress = false;
    //    }
    //    else
    //    {
    //      cpt.ReportFooterSection6.SectionFormat.EnableSuppress = true;
    //    }

    //    string companyName = string.Empty;
    //    string email = string.Empty;
    //    string website = string.Empty;
    //    string telephone = string.Empty;
    //    string taxCode = string.Empty;
    //    string accountNo = string.Empty;
    //    string fax = string.Empty;
    //    string address = string.Empty;
    //    string purchaseManager = string.Empty;
    //    string PrintDate = string.Empty;

    //    string commandText = "SELECT Code, ISNULL([Description], '') [Description] FROM TblBOMCodeMaster WHERE [Group] = 9008";
    //    DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
    //    if (dtSource != null)
    //    {
    //      address = dtSource.Rows[0]["Description"].ToString();
    //      telephone = dtSource.Rows[1]["Description"].ToString();
    //      fax = dtSource.Rows[2]["Description"].ToString();
    //      email = dtSource.Rows[3]["Description"].ToString();
    //      website = dtSource.Rows[4]["Description"].ToString();
    //      taxCode = dtSource.Rows[5]["Description"].ToString();
    //      accountNo = dtSource.Rows[6]["Description"].ToString();
    //      companyName = dtSource.Rows[7]["Description"].ToString();
    //    }
    //    // Purchas Manager
    //    string commandTex = "SELECT ManagerName FROM VHRDDepartmentInfo WHERE CODE = 'PUR'";
    //    DataTable dtPurchaseManagerName = DataBaseAccess.SearchCommandTextDataTable(commandTex);
    //    if (dtPurchaseManagerName != null)
    //    {
    //      purchaseManager = dtPurchaseManagerName.Rows[0]["ManagerName"].ToString();
    //    }
    //    // PrintDate
    //    PrintDate = DBConvert.ParseString(DateTime.Now, Shared.Utility.ConstantClass.FORMAT_DATETIME) + " By: " + SharedObject.UserInfo.EmpName;
    //    // Hide, Unhide Header, Detail(When Check RefCode)
    //    if (chkRefCode.Checked)
    //    {
    //      cpt.PageHeaderSection1.SectionFormat.EnableSuppress = true;
    //      cpt.DetailSection1.SectionFormat.EnableSuppress = true;
    //      cpt.PageHeaderSection2.SectionFormat.EnableSuppress = false;
    //      cpt.DetailSection2.SectionFormat.EnableSuppress = false;
    //      if (chkNameVn.Checked)
    //      {
    //        cpt.DetailSection3.SectionFormat.EnableSuppress = true;
    //        cpt.DetailSection4.SectionFormat.EnableSuppress = false;
    //      }
    //      else
    //      {
    //        cpt.DetailSection3.SectionFormat.EnableSuppress = true;
    //        cpt.DetailSection4.SectionFormat.EnableSuppress = true;
    //      }
    //    }
    //    else
    //    {
    //      cpt.PageHeaderSection1.SectionFormat.EnableSuppress = false;
    //      cpt.DetailSection1.SectionFormat.EnableSuppress = false;
    //      cpt.PageHeaderSection2.SectionFormat.EnableSuppress = true;
    //      cpt.DetailSection2.SectionFormat.EnableSuppress = true;
    //      if (chkNameVn.Checked)
    //      {
    //        cpt.DetailSection3.SectionFormat.EnableSuppress = false;
    //        cpt.DetailSection4.SectionFormat.EnableSuppress = true;
    //      }
    //      else
    //      {
    //        cpt.DetailSection3.SectionFormat.EnableSuppress = true;
    //        cpt.DetailSection4.SectionFormat.EnableSuppress = true;
    //      }
    //    }
    //    // Hide, unhide AdditionPrice
    //    if (dsSource.Tables["dtPOAdditionPrice"].Rows.Count > 0)
    //    {
    //      cpt.ReportFooterSection1.SectionFormat.EnableSuppress = false;
    //    }
    //    else
    //    {
    //      cpt.ReportFooterSection1.SectionFormat.EnableSuppress = true;
    //    }

    //    cpt.SetDataSource(dsSource);

    //    cpt.SetParameterValue("address", address);
    //    cpt.SetParameterValue("telephone", telephone);
    //    cpt.SetParameterValue("email", email);
    //    cpt.SetParameterValue("website", website);
    //    cpt.SetParameterValue("taxCode", taxCode);
    //    cpt.SetParameterValue("accountNo", accountNo);
    //    cpt.SetParameterValue("fax", fax);
    //    cpt.SetParameterValue("companyName", companyName);
    //    // PurchaseManager Name
    //    cpt.SetParameterValue("purchaseManager", purchaseManager);
    //    // Total
    //    cpt.SetParameterValue("totalAmount", totalAmount);
    //    cpt.SetParameterValue("totalVAT", totalVAT);
    //    cpt.SetParameterValue("total", total);
    //    // Order Remark
    //    cpt.SetParameterValue("orderRemark", orderRemark);
    //    // Number To English
    //    cpt.SetParameterValue("numberToEnglish", numberToEnglish);
    //    //PrintDate
    //    cpt.SetParameterValue("PrintDate", PrintDate);
    //    if (chkRefCode.Checked)
    //    {
    //      cpt.SetParameterValue("checkRefCode", 1);
    //    }
    //    else
    //    {
    //      cpt.SetParameterValue("checkRefCode", 0);
    //    }
    //    //ControlUtility.ViewCrystalReport(cpt);

    //    report = new DaiCo.Shared.View_Report(cpt);
    //    report.IsShowGroupTree = false;
    //    report.ShowReport(Shared.Utility.ViewState.MainWindow);
    //  }
    //}

    private string StripHTML(string source)
    {
      try
      {
        string result;

        // Remove HTML Development formatting
        // Replace line breaks with space
        // because browsers inserts space
        result = source.Replace("\r", " ");
        // Replace line breaks with space
        // because browsers inserts space
        result = result.Replace("\n", " ");
        // Remove step-formatting
        result = result.Replace("\t", string.Empty);
        // Remove repeating spaces because browsers ignore them
        result = System.Text.RegularExpressions.Regex.Replace(result,
                                                              @"( )+", " ");

        // Remove the header (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*head([^>])*>", "<head>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<( )*(/)( )*head( )*>)", "</head>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(<head>).*(</head>)", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // remove all scripts (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*script([^>])*>", "<script>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<( )*(/)( )*script( )*>)", "</script>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //result = System.Text.RegularExpressions.Regex.Replace(result,
        //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
        //         string.Empty,
        //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<script>).*(</script>)", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // remove all styles (prepare first by clearing attributes)
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*style([^>])*>", "<style>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"(<( )*(/)( )*style( )*>)", "</style>",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(<style>).*(</style>)", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert tabs in spaces of <td> tags
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*td([^>])*>", "\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert line breaks in places of <BR> and <LI> tags
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*br( )*>", "\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*li( )*>", "\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // insert line paragraphs (double line breaks) in place
        // if <P>, <DIV> and <TR> tags
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*div([^>])*>", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*tr([^>])*>", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<( )*p([^>])*>", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Remove remaining tags like <a>, links, images,
        // comments etc - anything that's enclosed inside < >
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"<[^>]*>", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // replace special characters:
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @" ", " ",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&bull;", " * ",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&lsaquo;", "<",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&rsaquo;", ">",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&trade;", "(tm)",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&frasl;", "/",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&lt;", "<",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&gt;", ">",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&copy;", "(c)",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&reg;", "(r)",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove all others. More can be added, see
        // http://hotwired.lycos.com/webmonkey/reference/special_characters/
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 @"&(.{2,6});", string.Empty,
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // for testing
        //System.Text.RegularExpressions.Regex.Replace(result,
        //       this.txtRegex.Text,string.Empty,
        //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // make line breaking consistent
        result = result.Replace("\n", "\r");

        // Remove extra line breaks and tabs:
        // replace over 2 breaks with 2 and over 4 tabs with 4.
        // Prepare first to remove any whitespaces in between
        // the escaped characters and remove redundant tabs in between line breaks
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)( )+(\r)", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\t)( )+(\t)", "\t\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\t)( )+(\r)", "\t\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)( )+(\t)", "\r\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove redundant tabs
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)(\t)+(\r)", "\r\r",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Remove multiple tabs following a line break with just one tab
        result = System.Text.RegularExpressions.Regex.Replace(result,
                 "(\r)(\t)+", "\r\t",
                 System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        // Initial replacement target string for line breaks
        string breaks = "\r\r\r";
        // Initial replacement target string for tabs
        string tabs = "\t\t\t\t\t";
        for (int index = 0; index < result.Length; index++)
        {
          result = result.Replace(breaks, "\r\r");
          result = result.Replace(tabs, "\t\t\t\t");
          breaks = breaks + "\r";
          tabs = tabs + "\t";
        }

        // That's it.
        return result;
      }
      catch
      {
        MessageBox.Show("Error");
        return source;
      }
    }
    #endregion LoadData & Search

    #region Event

    /// <summary>
    /// chkHide Checked Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (this.groupBoxItem.Visible)
      {
        this.groupBoxItem.Visible = false;
      }
      else
      {
        this.groupBoxItem.Visible = true;
      }
    }

    /// <summary>
    /// btnSearch Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.True;
      for (int i = 1; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        e.Layout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["SupplierName"].Header.Caption = "Supplier Name";
      e.Layout.Bands[0].Columns["PONo"].Header.Caption = "PO No";
      e.Layout.Bands[0].Columns["TotalMoney"].Header.Caption = "Total Money";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["ApprovedBy"].Header.Caption = "Approved By";
      e.Layout.Bands[0].Columns["POStatus"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalMoney"].Format = "###,###.## VND";
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["FinishDate"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["FinishDate"].CellAppearance.ForeColor = Color.Red;

      e.Layout.Bands[1].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Status"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[1].Columns["Status"].CellAppearance.ForeColor = Color.Red;

      e.Layout.Bands[1].Columns["Note"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[1].Columns["Note"].CellAppearance.ForeColor = Color.Red;

      e.Layout.Bands[1].Columns["PONo"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;

      e.Layout.Bands[1].Columns["PRNo"].Header.Caption = "PR No";
      e.Layout.Bands[1].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[1].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[1].Columns["Price"].Format = "###,###.##";

      e.Layout.Bands[3].Columns["Pid"].Hidden = true;
      e.Layout.Bands[2].Columns["Pid"].Hidden = true;
      e.Layout.Bands[2].Columns["PODetailPid"].Hidden = true;
      e.Layout.Bands[2].Columns["ReceiptedQty"].Header.Caption = "Receipted Qty";

      if (this.flagviewPrice)
      {
        e.Layout.Bands[1].Columns["Price"].Hidden = false;
        e.Layout.Bands[0].Columns["TotalMoney"].Hidden = false;
      }
      else
      {
        e.Layout.Bands[1].Columns["Price"].Hidden = true;
        e.Layout.Bands[0].Columns["TotalMoney"].Hidden = true;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// btnNewPO Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNewPO_Click(object sender, EventArgs e)
    {
      viewPUR_04_003 view = new viewPUR_04_003();
      Shared.Utility.WindowUtinity.ShowView(view, "NEW PO", false, Shared.Utility.ViewState.MainWindow);
    }

    /// <summary>
    /// ultData Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0)
      {
        viewPUR_04_003 view = new viewPUR_04_003();
        UltraGridRow rowSelected = null;
        if (ultData.Selected.Rows[0].HasParent())
        {
          rowSelected = (ultData.Selected.Rows[0].ParentRow.HasParent()) ? ultData.Selected.Rows[0].ParentRow.ParentRow : ultData.Selected.Rows[0].ParentRow;
        }
        else
        {
          rowSelected = ultData.Selected.Rows[0];
        }
        string poNo = rowSelected.Cells["PONo"].Value.ToString().Trim();
        view.poNo = poNo;
        Shared.Utility.WindowUtinity.ShowView(view, "UPDATE PO", false, Shared.Utility.ViewState.MainWindow);
      }
    }

    private void txtAmountFrom_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtAmountFrom.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtAmountFrom.Text = numberRead;
    }

    private void txtAmountTo_Leave(object sender, EventArgs e)
    {
      double number = DBConvert.ParseDouble(txtAmountTo.Text);
      string numberRead = this.NumericFormat(number, 2);
      txtAmountTo.Text = numberRead;
    }

    /// <summary>
    /// Format Numeric
    /// </summary>
    /// <param name="number"></param>
    /// <param name="phanLe"></param>
    /// <returns></returns>
    private string NumericFormat(double number, int phanLe)
    {
      if (number == double.MinValue)
      {
        return string.Empty;
      }
      if (phanLe < 0)
      {
        return number.ToString();
      }
      System.Globalization.NumberFormatInfo formatInfo = new System.Globalization.NumberFormatInfo();
      double t = Math.Truncate(number);
      formatInfo.NumberDecimalDigits = phanLe;
      return number.ToString("N", formatInfo);
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      ArrayList arrListPODTPid = new ArrayList();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = ultData.Rows[i].ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
          {
            arrListPODTPid.Add(row.Cells["Pid"].Value.ToString());
          }
        }
      }
      viewPUR_04_002 view = new viewPUR_04_002();
      view.arrList = arrListPODTPid;
      view.flag = 0;
      WindowUtinity.ShowView(view, "CANCEL PO", false, ViewState.ModalWindow, FormWindowState.Normal);

      this.Search();
    }

    private void btnEndReceiving_Click(object sender, EventArgs e)
    {
      ArrayList arrListPODTPid = new ArrayList();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = ultData.Rows[i].ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
          {
            arrListPODTPid.Add(row.Cells["Pid"].Value.ToString());
          }
        }
      }
      viewPUR_04_002 view = new viewPUR_04_002();
      view.arrList = arrListPODTPid;
      view.flag = 1;
      WindowUtinity.ShowView(view, "END RECEIVING", false, ViewState.ModalWindow, FormWindowState.Normal);

      this.Search();
    }

    /// <summary>
    /// Cell Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      if (e.Cell.Row.ParentRow == null)
      {
        string columnName = e.Cell.Column.ToString();
        if (string.Compare(columnName, "Selected", true) == 0)
        {
          UltraGridRow row = e.Cell.Row;
          for (int i = 0; i < row.ChildBands[0].Rows.Count; i++)
          {
            row.ChildBands[0].Rows[i].Cells["Selected"].Value = e.Cell.Text;
          }
        }
      }
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      int count = 0;
      string poNo = string.Empty;
      // Check Valid
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          count = count + 1;
          if (count > 1)
          {
            WindowUtinity.ShowMessageError("ERR0115", "One PO");
            return;
          }
        }
      }
      // Printf
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          poNo = row.Cells["poNo"].Value.ToString();
          //this.PrintReport(poNo);
        }
      }

      viewPUR_04_007 view = new viewPUR_04_007();
      view.poNO = poNo;
      if (chkWithAddition.Checked)
      {
        view.chkWithAddition = true;
      }
      if (chkRefCode.Checked)
      {
        view.chkRefCode = true;
      }
      else
      {
        view.chkRefCode = false;
      }
      if (chkNameVn.Checked)
      {
        view.chkNameVn = true;
      }
      else
      {
        view.chkNameVn = false;
      }
      DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "Print PO", false, DaiCo.Shared.Utility.ViewState.ModalWindow);

    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }

    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultData.Rows.ExpandAll(true);
      }
      else
      {
        ultData.Rows.CollapseAll(true);
      }
    }

    /// <summary>
    /// Close Form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultData_InitializeRow(object sender, InitializeRowEventArgs e)
    {
    }

    private void btnFinish_Click(object sender, EventArgs e)
    {
      ArrayList arrListPODTPid = new ArrayList();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = ultData.Rows[i].ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
          {
            arrListPODTPid.Add(row.Cells["Pid"].Value.ToString());
          }
        }
      }
      viewPUR_04_005 view = new viewPUR_04_005();
      view.arrList = arrListPODTPid;
      WindowUtinity.ShowView(view, "FINISH", false, ViewState.ModalWindow, FormWindowState.Normal);

      this.Search();
    }

    private void btnClosePO_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow row = ultData.Rows[i].ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
          {
            DBParameter[] inputParam = new DBParameter[3];
            inputParam[0] = new DBParameter("@PONo", DbType.String, row.Cells["PONo"].Value.ToString());
            inputParam[1] = new DBParameter("@PODetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            inputParam[2] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);

            DBParameter[] outputParam = new DBParameter[1];
            outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);

            DataBaseAccess.ExecuteStoreProcedure("spPURStatusPRPOWhenCloseWO_Update", inputParam, outputParam);

            int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
            if (result <= 0)
            {
              WindowUtinity.ShowMessageError("WRN0004");
              return;
            }
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.Search();
    }
    #endregion Event  
  }
}
