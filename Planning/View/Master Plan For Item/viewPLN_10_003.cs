/* Author: TRAN HUNG
 * Date  : 17/07/2012
 * */
using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Planning;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_10_003 : MainUserControl
  {
    #region Fied
    private IList listDeleteDetailPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private DataTable dtAddWork = new DataTable();
    public string StrWorkPID = "";
    public long wopid;
    private int loopFlag = 0;
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Fied

    #region Load
    public viewPLN_10_003()
    {
      InitializeComponent();
    }
    private void viewPLN_10_003_Load(object sender, EventArgs e)
    {
      this.ultdtShipdateFrom.Value = null;
      this.ultdtShipdateTo.Value = null;
      this.LoadType();
      this.LoadDropdownWorkOrder();
      Shared.Utility.ControlUtility.LoadCustomer(cmbCustomer);
      // Hien Wo Dang mo hien tai nho nhat
      string commandTextMin = "SELECT MAX(Pid ) FROM TblPLNWorkOrder WHERE Pid < 80000";
      this.lblWoMinOfMin.Text = DBConvert.ParseLong(DataBaseAccess.ExecuteScalarCommandText(commandTextMin)).ToString();

      string commandTextMax = "SELECT MAX(Pid ) FROM TblPLNWorkOrder WHERE Pid > 90000";
      lblWoMinOfMax.Text = DBConvert.ParseLong(DataBaseAccess.ExecuteScalarCommandText(commandTextMax)).ToString();
      StrWorkPID += txtWorkOrder.Text.Trim();
    }
    private void LoadType()
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Value FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", Shared.Utility.ConstantClass.GROUP_SALEORDERTYPE);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBType.DataSource = dtSource;
      ultCBType.DisplayLayout.AutoFitColumns = true;
      ultCBType.DisplayMember = "Value";
      ultCBType.ValueMember = "Code";
      ultCBType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }
    private void btnAddWork_Click(object sender, EventArgs e)
    {
      DataSet ds = (DataSet)ultGridSearch.DataSource;
      if (ds != null)
      {
        DataTable dt = ds.Tables[2];
        DataTable dtinsert = dt.Clone();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(dt.Rows[i]["Selected"].ToString()) > 0
            && DBConvert.ParseLong(dt.Rows[i]["SaleOrderDetailPid"].ToString()) > 0
             && DBConvert.ParseInt(dt.Rows[i]["Status"].ToString()) == 3
            && DBConvert.ParseInt(dt.Rows[i]["UnRelease"].ToString()) > 0)
          {
            DataRow dr = dtinsert.NewRow();
            for (int c = 0; c < dt.Columns.Count; c++)
            {
              dr[c] = dt.Rows[i][c];
            }
            dr["OpenWO"] = dr["OpenWO"];
            dtinsert.Rows.Add(dr);
          }
        }

        if (dtAddWork.Rows.Count == 0)
        {
          dtAddWork = dtinsert.Clone();
        }
        for (int i = 0; i < dtinsert.Rows.Count; i++)
        {
          DataRow row = dtinsert.Rows[i];
          dtAddWork.Rows.Add(row.ItemArray);
        }
        ultGridAddWord.DataSource = dtAddWork;
        this.AutoFill();
        for (int b = 0; b < ultGridAddWord.Rows.Count; b++)
        {

          string Sale = ultGridAddWord.Rows[b].Cells["SaleOrderPid"].Value.ToString();
          if (Sale != string.Empty)
          {
            ultGridAddWord.Rows[b].CellAppearance.BackColor = Color.SkyBlue;
          }
          else
            ultGridAddWord.Rows[b].CellAppearance.BackColor = Color.Pink;

          ultGridAddWord.Rows[b].Cells["Wo"].Appearance.BackColor = Color.SteelBlue;
          ultGridAddWord.Rows[b].Cells["OpenWO"].Appearance.BackColor = Color.SteelBlue;
          ultGridAddWord.Rows[b].Cells["ShipDate"].Appearance.BackColor = Color.SteelBlue;

        }
        for (int i = 0; i < ultGridAddWord.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          string columnName = ultGridAddWord.DisplayLayout.Bands[0].Columns[i].Header.Caption;
          if (string.Compare(columnName, "OpenWO", true) == 0 || string.Compare(columnName, "Wo", true) == 0 || string.Compare(columnName, "IsSubCon", true) == 0 || string.Compare(columnName, "Ship Date(Release Wo)", true) == 0 || string.Compare(columnName, "ShipDate", true) == 0)
          {
            ultGridAddWord.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
          }
          else
            ultGridAddWord.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;

        }
        btnSearch_Click(sender, e);
      }
    }
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }
    private void Search()
    {
      string storeName = "spPLNItemInformationForMasterPlan";

      DBParameter[] param = new DBParameter[13];
      if (ultdtShipdateFrom.Value != null)
      {
        param[0] = new DBParameter("@ScheduleFrom", DbType.DateTime, DBConvert.ParseDateTime(ultdtShipdateFrom.Value.ToString(), formatConvert));
      }

      if (ultdtShipdateTo.Value != null)
      {
        param[1] = new DBParameter("@ScheduleTo", DbType.DateTime, DBConvert.ParseDateTime(ultdtShipdateTo.Value.ToString(), formatConvert));
      }

      if (txtUnreleaseFrom.Text.Trim().Length > 0 && DBConvert.ParseInt(txtUnreleaseFrom.Text.Trim()) != int.MinValue)
      {
        param[2] = new DBParameter("@UnreleaseFrom", DbType.Int32, DBConvert.ParseInt(txtUnreleaseFrom.Text.Trim()));
      }

      if (txtUnreleaseTo.Text.Trim().Length > 0 && DBConvert.ParseInt(txtUnreleaseTo.Text.Trim()) != int.MinValue)
      {
        param[3] = new DBParameter("@UnreleaseTo", DbType.Int32, DBConvert.ParseInt(txtUnreleaseTo.Text.Trim()));
      }
      if (chkSameCarcass.Checked)
      {
        param[4] = new DBParameter("@SameCarcass", DbType.Int32, 1);
      }
      if (chkSubcon.Checked)
      {
        param[5] = new DBParameter("@IsSupCon", DbType.Int32, 1);
      }
      if (chkAll.Checked)
      {
        param[6] = new DBParameter("@IsAll", DbType.Int32, 1);
      }
      if (ChkRelationShip.Checked)
      {
        param[7] = new DBParameter("@IsRelative", DbType.Int32, 1);
      }
      if (txtItemCode.Text.Trim().Length > 0)
      {
        param[8] = new DBParameter("@ItemCode", DbType.AnsiString, 16, '%' + txtItemCode.Text.Trim() + '%');
      }
      if (txtSaleCode.Text.Trim().Length > 0)
      {
        param[9] = new DBParameter("@SaleCode", DbType.AnsiString, 30, '%' + txtSaleCode.Text.Trim() + '%');
      }
      if (cmbCustomer.Text.Trim().Length > 0)
      {
        param[10] = new DBParameter("@CustomerPid", DbType.Int64, cmbCustomer.SelectedValue);
      }
      if (txtBalanceFrom.Text.Trim().Length > 0 && DBConvert.ParseInt(txtBalanceFrom.Text.Trim()) != int.MinValue)
      {
        param[11] = new DBParameter("@BalanceFrom", DbType.Int32, DBConvert.ParseInt(txtBalanceFrom.Text.Trim()));
      }
      if (txtBalanceTo.Text.Trim().Length > 0 && DBConvert.ParseInt(txtBalanceTo.Text.Trim()) != int.MinValue)
      {
        param[12] = new DBParameter("@BalanceTo", DbType.Int32, DBConvert.ParseInt(txtBalanceTo.Text.Trim()));
      }
      DataSet dsSearch = DataBaseAccess.SearchStoreProcedure(storeName, 600, param);

      dsPLNMasterPlanItem ds = new dsPLNMasterPlanItem();
      ds.Tables["Item"].Merge(dsSearch.Tables[0]);
      ds.Tables["SaleOrder"].Merge(dsSearch.Tables[1]);
      ds.Tables["WorkItem"].Merge(dsSearch.Tables[2]);

      ultGridSearch.DataSource = ds;
      for (int i = 0; i < ultGridSearch.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultGridSearch.Rows[i].Cells["IsColor"].Value.ToString()) == 0)
        {
          ultGridSearch.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        if (DBConvert.ParseInt(ultGridSearch.Rows[i].Cells["IsColor"].Value.ToString()) == 1)
        {
          ultGridSearch.Rows[i].CellAppearance.BackColor = Color.SkyBlue;
        }
        if (DBConvert.ParseInt(ultGridSearch.Rows[i].Cells["IsColor"].Value.ToString()) == 2)
        {
          ultGridSearch.Rows[i].CellAppearance.BackColor = Color.Pink;
        }
        for (int j = 0; j < ultGridSearch.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          if (DBConvert.ParseInt(ultGridSearch.Rows[i].ChildBands[0].Rows[j].Cells["Status"].Value.ToString()) != 3)
          {
            ultGridSearch.Rows[i].ChildBands[0].Rows[j].CellAppearance.BackColor = Color.Pink;
          }
        }
      }
    }
    private DataTable GetDataTable(DataTable dtSource)
    {
      DataTable dt = new DataTable();
      SqlCommand cm = new SqlCommand("spPLNWoInfoDetail");
      cm.Connection = new SqlConnection(DBConvert.DecodeConnectiontring(ConfigurationSettings.AppSettings["connectionString"]));
      cm.CommandType = CommandType.StoredProcedure;

      // Data Table 
      SqlParameter para = cm.CreateParameter();
      para.ParameterName = "@GetData";
      para.SqlDbType = SqlDbType.Structured;
      para.Value = dtSource;

      cm.Parameters.Add(para);

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

      return dt = result.Tables[0];
    }
    private void LoadDropdownWorkOrder()
    {
      string commandText = string.Format(@"SELECT DISTINCT WID.WoInfoPID FROM TblPLNWOInfoDetailGeneral WID LEFT JOIN TblPLNWorkOrderConfirmedDetails WCD ON WID.WoInfoPID = WCD.WorkOrderPid WHERE WCD.WorkOrderPid IS NULL ORDER BY WID.WoInfoPID DESC ");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      udrpWorkOrder.DataSource = dt;
      DataRow dr = dt.NewRow();
      dt.Rows.InsertAt(dr, 0);
      udrpWorkOrder.DisplayMember = "WoInfoPID";
      udrpWorkOrder.ValueMember = "WoInfoPID";
      udrpWorkOrder.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }
    private bool Save()
    {
      bool result = false;
      DataTable dtsource = (DataTable)ultGridAddWord.DataSource;
      if (dtsource != null)
      {
        DataTable dt = new DataTable();
        dt.Columns.Add("SaleOrderDetailPid", typeof(System.String));
        dt.Columns.Add("ItemCode", typeof(System.String));
        dt.Columns.Add("Revision", typeof(System.String));
        dt.Columns.Add("OpenWo", typeof(System.String));
        dt.Columns.Add("UnRelease", typeof(System.String));
        foreach (DataRow drMain in dtsource.Rows)
        {
          DataRow row = dt.NewRow();
          row["SaleOrderDetailPid"] = drMain["SaleOrderDetailPid"].ToString();
          row["ItemCode"] = drMain["ItemCode"].ToString();
          row["Revision"] = drMain["Revision"].ToString();
          row["OpenWo"] = drMain["OpenWo"].ToString();
          row["UnRelease"] = drMain["UnRelease"].ToString();
          dt.Rows.Add(row);
        }
        DataTable dtGet = GetDataTable(dt);


        if (dtGet.Rows.Count == 0)
        {
          result = true;
          for (int i = 0; i < ultGridAddWord.Rows.Count; i++)
          {
            string storeName = string.Empty;
            DBParameter[] inputParam = new DBParameter[11];
            DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            UltraGridRow row = ultGridAddWord.Rows[i];
            storeName = "spPLNWOInfoDetail_Insert";
            long WoInfoPID = DBConvert.ParseLong(row.Cells["Wo"].Value.ToString());
            if (WoInfoPID != 0 && StrWorkPID.ToString().IndexOf(WoInfoPID.ToString()) >= 0)
            {
              if (DBConvert.ParseInt(row.Cells["OpenWO"].Value.ToString()) > 0)
              {
                if (row.Cells["OldWo"].Value.ToString() == "")
                {
                  inputParam[1] = new DBParameter("@WoInfoPID", DbType.Int64, WoInfoPID);
                  string itemCode = row.Cells["ItemCode"].Value.ToString().Trim().Replace("'", "''");
                  inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
                  int value = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
                  inputParam[3] = new DBParameter("@Revision", DbType.Int32, value);
                  value = DBConvert.ParseInt(row.Cells["OpenWO"].Value.ToString());
                  inputParam[4] = new DBParameter("@Qty", DbType.Int32, value);
                  long saleOrderDetailPid = DBConvert.ParseLong(row.Cells["SaleOrderDetailPid"].Value.ToString());
                  inputParam[6] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleOrderDetailPid);
                  inputParam[8] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                  if (DBConvert.ParseInt(row.Cells["IsSubCon"].Value.ToString()) == 1)
                  {
                    inputParam[7] = new DBParameter("@IsSubCon", DbType.Int32, 1);
                  }
                  else
                  {
                    inputParam[7] = new DBParameter("@IsSubCon", DbType.Int32, 0);
                  }
                  inputParam[9] = new DBParameter("@Note", DbType.AnsiString, 256, "");
                  DateTime DeadLineDate = new DateTime();
                  if (row.Cells["ShipDate"].ToString().Trim().Length > 0)
                  {
                    DeadLineDate = DBConvert.ParseDateTime(row.Cells["ShipDate"].Value.ToString(), formatConvert);
                    inputParam[10] = new DBParameter("@DeadLineDate", DbType.DateTime, DeadLineDate);
                  }

                  Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
                  long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
                  if (outValue == -1)
                  {
                    Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0002", "Work Order: " + WoInfoPID + " And Item Code: " + itemCode);
                    result = false;
                  }
                  else if (outValue == 0)
                  {
                    Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
                    result = false;
                  }
                  if (result == true)
                  {
                    row.Cells["PIDDetailInsert"].Value = outValue;
                  }
                }
                else
                {
                  string storeName_update = string.Empty;
                  DBParameter[] inputParam_update = new DBParameter[12];
                  DBParameter[] outputParam_update = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
                  UltraGridRow row_update = ultGridAddWord.Rows[i];
                  storeName = "spPLNWOInfoDetail_Update";
                  inputParam_update[1] = new DBParameter("@WoInfoPID", DbType.Int64, DBConvert.ParseInt(row_update.Cells["Wo"].Value.ToString()));
                  string itemCode = row_update.Cells["ItemCode"].Value.ToString().Trim().Replace("'", "''");
                  inputParam_update[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
                  int value = DBConvert.ParseInt(row_update.Cells["Revision"].Value.ToString());
                  inputParam_update[3] = new DBParameter("@Revision", DbType.Int32, value);
                  value = DBConvert.ParseInt(row_update.Cells["OpenWO"].Value.ToString());
                  inputParam_update[4] = new DBParameter("@Qty", DbType.Int32, value);
                  long saleOrderDetailPid = DBConvert.ParseLong(row_update.Cells["SaleOrderDetailPid"].Value.ToString());
                  inputParam_update[6] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleOrderDetailPid);
                  inputParam_update[8] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                  if (DBConvert.ParseInt(row_update.Cells["IsSubCon"].Value.ToString()) == 1)
                  {
                    inputParam_update[7] = new DBParameter("@IsSubCon", DbType.Int32, 1);
                  }
                  else
                  {
                    inputParam_update[7] = new DBParameter("@IsSubCon", DbType.Int32, 0);
                  }
                  DateTime DeadLineDate = new DateTime();
                  if (row.Cells["ShipDate"].ToString().Trim().Length > 0)
                  {
                    DeadLineDate = DBConvert.ParseDateTime(row.Cells["ShipDate"].Value.ToString(), formatConvert);
                    inputParam_update[11] = new DBParameter("@DeadLineDate", DbType.DateTime, DeadLineDate);
                  }

                  inputParam_update[9] = new DBParameter("@Note", DbType.AnsiString, 256, row.Cells["ItemCode"].Value.ToString());
                  inputParam_update[10] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseInt(row_update.Cells["PIDDetailInsert"].Value.ToString()));
                  Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam_update, outputParam_update);
                  long outValue = DBConvert.ParseLong(outputParam_update[0].Value.ToString());
                  if (outValue == -1)
                  {
                    Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0002", "Work Order: " + WoInfoPID + " And Item Code: " + itemCode);
                    result = false;
                  }
                  else if (outValue == 0)
                  {
                    Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
                    result = false;
                  }

                }
                if (result == true)
                {
                  ultGridAddWord.Rows[i].Cells["OldWo"].Value = ultGridAddWord.Rows[i].Cells["Wo"].Value.ToString();
                  ultGridAddWord.Rows[i].CellAppearance.BackColor = Color.White;
                }
              }
              else
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Open Wo");
                ultGridAddWord.Rows[i].CellAppearance.BackColor = Color.Yellow;
                result = false;
              }
            }
            else
            {
              Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0102", "This Work Order Detail");
              ultGridAddWord.Rows[i].CellAppearance.BackColor = Color.Yellow;
              result = false;
            }


          }
        }
        else
        {
          DataTable dtMain = (DataTable)ultGridAddWord.DataSource;
          // Loop for datatable get from SQL
          for (int x = 0; x < dtGet.Rows.Count; x++)
          {
            ultGridAddWord.Rows[x].CellAppearance.BackColor = Color.White;
            DataRow row = dtMain.NewRow();
            DataRow[] foundRows = dtMain.Select("SaleOrderDetailPid = '" + dtGet.Rows[x]["SaleOrderDetailPid"].ToString() + "'");

            if (foundRows.Length > 0)
            {
              for (int y = 0; y < ultGridAddWord.Rows.Count; y++)
              {
                if (ultGridAddWord.Rows[y].Cells["SaleOrderDetailPid"].Value.ToString() == dtGet.Rows[x]["SaleOrderDetailPid"].ToString())
                  ultGridAddWord.Rows[y].CellAppearance.BackColor = Color.Yellow;
              }

            }

          }
          Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
          result = false;
        }
      }
      return result;

    }
    private long SaveWorkOrder()
    {
      PLNWorkOrder workOrder = new PLNWorkOrder();

      workOrder.CreateBy = Shared.Utility.SharedObject.UserInfo.UserPid;
      workOrder.CreateDate = DateTime.Now;
      workOrder.Pid = DBConvert.ParseLong(txtWorkOrder.Text.ToString());

      workOrder.Type = DBConvert.ParseInt(ultCBType.Value.ToString());
      workOrder.Status = 0;
      workOrder.Confirm = 0;
      workOrder.Remark = txtRemark.Text.Trim();

      Shared.DataBaseUtility.DataBaseAccess.InsertObject(workOrder);
      if (workOrder.Pid > 0)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      StrWorkPID += "," + workOrder.Pid.ToString();
      return workOrder.Pid;
    }
    private bool SaveDetalWoOld()
    {
      bool result = false;
      if (ultGridSearch.Rows.Count > 0)
      {

        DataSet dtset = (DataSet)ultGridSearch.DataSource;
        DataTable dtsource = dtset.Tables[2];
        for (int i = 0; i < dtsource.Rows.Count; i++)
        {
          long WoInfoPID = DBConvert.ParseLong(dtsource.Rows[i]["Workorder"].ToString());
          if (WoInfoPID != long.MinValue)
          {
            result = true;
            if (DBConvert.ParseInt(dtsource.Rows[i]["OpenWO"].ToString()) > 0)
            {
              if (dtsource.Rows[i]["OldWo"].ToString() == "")
              {
                string storeName = string.Empty;
                DBParameter[] inputParam = new DBParameter[12];
                DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

                storeName = "spPLNWOInfoDetail_Insert";
                inputParam[1] = new DBParameter("@WoInfoPID", DbType.Int64, DBConvert.ParseLong(dtsource.Rows[i]["Workorder"].ToString()));
                string itemCode = dtsource.Rows[i]["ItemCode"].ToString().Trim().Replace("'", "''");
                inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
                int value = DBConvert.ParseInt(dtsource.Rows[i]["Revision"].ToString());
                inputParam[3] = new DBParameter("@Revision", DbType.Int32, value);
                value = DBConvert.ParseInt(dtsource.Rows[i]["OpenWO"].ToString());
                inputParam[4] = new DBParameter("@Qty", DbType.Int32, value);
                long saleOrderDetailPid = DBConvert.ParseLong(dtsource.Rows[i]["SaleOrderDetailPid"].ToString());
                inputParam[5] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleOrderDetailPid);
                inputParam[6] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                if (DBConvert.ParseInt(dtsource.Rows[i]["IsSubCon"].ToString()) == 1)
                {
                  inputParam[7] = new DBParameter("@IsSubCon", DbType.Int32, 1);
                }
                else
                {
                  inputParam[7] = new DBParameter("@IsSubCon", DbType.Int32, 0);
                }
                inputParam[8] = new DBParameter("@Note", DbType.AnsiString, 256, "");
                DateTime DeadLineDate = new DateTime();
                if (dtsource.Rows[i]["ShipDate"].ToString().Trim().Length > 0)
                {
                  DeadLineDate = DBConvert.ParseDateTime(dtsource.Rows[i]["ShipDate"].ToString(), formatConvert);
                  inputParam[9] = new DBParameter("@DeadLineDate", DbType.DateTime, DeadLineDate);
                }

                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
                long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
                if (outValue == -1)
                {
                  Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0002", "Work Order: " + WoInfoPID + " And Item Code: " + itemCode);
                  result = false;

                }
                else if (outValue == 0)
                {
                  Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
                  result = false;

                }
                if (result == true)
                {
                  dtsource.Rows[i]["PIDDetailInsert"] = outValue;
                }
              }
              else
              {
                string storeName_update = string.Empty;
                DBParameter[] inputParam_update = new DBParameter[12];
                DBParameter[] outputParam_update = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
                storeName_update = "spPLNWOInfoDetail_Update";
                inputParam_update[1] = new DBParameter("@WoInfoPID", DbType.Int64, DBConvert.ParseLong(dtsource.Rows[i]["Workorder"].ToString()));
                string itemCode = dtsource.Rows[i]["ItemCode"].ToString().Trim().Replace("'", "''");
                inputParam_update[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, itemCode);
                int value = DBConvert.ParseInt(dtsource.Rows[i]["Revision"].ToString());
                inputParam_update[3] = new DBParameter("@Revision", DbType.Int32, value);
                value = DBConvert.ParseInt(dtsource.Rows[i]["OpenWO"].ToString());
                inputParam_update[4] = new DBParameter("@Qty", DbType.Int32, value);
                long saleOrderDetailPid = DBConvert.ParseLong(dtsource.Rows[i]["SaleOrderDetailPid"].ToString());
                inputParam_update[6] = new DBParameter("@SaleOrderDetailPid", DbType.Int64, saleOrderDetailPid);
                inputParam_update[8] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
                if (DBConvert.ParseInt(dtsource.Rows[i]["IsSubCon"].ToString()) == 1)
                {
                  inputParam_update[7] = new DBParameter("@IsSubCon", DbType.Int32, 1);
                }
                else
                {
                  inputParam_update[7] = new DBParameter("@IsSubCon", DbType.Int32, 0);
                }
                DateTime DeadLineDate = new DateTime();
                if (dtsource.Rows[i]["ShipDate"].ToString().Trim().Length > 0)
                {
                  DeadLineDate = DBConvert.ParseDateTime(dtsource.Rows[i]["ShipDate"].ToString(), formatConvert);
                  inputParam_update[11] = new DBParameter("@DeadLineDate", DbType.DateTime, DeadLineDate);
                }

                inputParam_update[9] = new DBParameter("@Note", DbType.AnsiString, 256, dtsource.Rows[i]["ItemCode"].ToString());
                inputParam_update[10] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseInt(dtsource.Rows[i]["PIDDetailInsert"].ToString()));
                Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName_update, inputParam_update, outputParam_update);
                long outValue = DBConvert.ParseLong(outputParam_update[0].Value.ToString());
                if (outValue == -1)
                {
                  Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0002", "Work Order: " + WoInfoPID + " And Item Code: " + itemCode);
                  result = false;

                }
                else if ((outValue == 0) || outValue == int.MinValue)
                {
                  Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
                  result = false;

                }

              }
              if (result == true)
              {
                dtsource.Rows[i]["OldWo"] = DBNull.Value;
                long wo = DBConvert.ParseLong(dtsource.Rows[i]["Workorder"].ToString());
                if (wo != long.MinValue)
                {
                  dtsource.Rows[i]["OldWo"] = wo;
                }
              }

            }
            else
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Open Wo");
              result = false;

            }
          }
        }

      }
      return result;
    }
    private void AutoFill()
    {
      if (ultGridAddWord.Rows.Count > 0)
      {
        for (int i = 0; i < ultGridAddWord.Rows.Count; i++)
        {
          if ((ultGridAddWord.Rows[i].Cells["Wo"].Value.ToString() == "") && (this.txtWorkOrder.Text.ToString() != ""))
            ultGridAddWord.Rows[i].Cells["Wo"].Value = DBConvert.ParseLong(this.txtWorkOrder.Text.ToString());
        }
      }
    }
    #endregion Load

    #region Event
    private void ultGridSearch_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.AutoFitColumns = true;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultGridSearch);
      e.Layout.Bands[1].Override.RowAppearance.BackColor = Color.White;
      e.Layout.Bands[1].Override.RowAppearance.ForeColor = Color.Black;
      e.Layout.Bands[2].Override.RowAppearance.BackColor = Color.SandyBrown;
      e.Layout.Bands[2].Override.RowAppearance.ForeColor = Color.Black;
      e.Layout.Bands[1].Columns["Workorder"].CellAppearance.BackColor = Color.SteelBlue;
      e.Layout.Bands[1].Columns["OpenWO"].CellAppearance.BackColor = Color.SteelBlue;
      e.Layout.Bands[1].Columns["ShipDate"].CellAppearance.BackColor = Color.SteelBlue;
      e.Layout.Bands[1].Columns["Workorder"].ValueList = udrpWorkOrder;
      e.Layout.Bands[1].Columns["Workorder"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["Workorder"].MinWidth = 100;
      e.Layout.Bands[1].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[2].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[2].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Wo"].Hidden = true;
      e.Layout.Bands[1].Columns["SO Remain"].Hidden = true;
      e.Layout.Bands[1].Columns["Qty"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["StandardCBM"].Header.Caption = "CBM";
      e.Layout.Bands[1].Columns["ItemCBM"].Header.Caption = "Unit CBM";
      e.Layout.Bands[1].Columns["ShipDate"].Header.Caption = "Ship Date(Release Wo)";
      e.Layout.Bands[1].Columns["CancelQty"].Header.Caption = "Cancel Qty";
      e.Layout.Bands[1].Columns["PIDDetailInsert"].Hidden = true;
      e.Layout.Bands[1].Columns["OldWo"].Hidden = true;
      e.Layout.Bands[1].Columns["TotalCBM"].Hidden = true;
      e.Layout.Bands[1].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["Checked"].Hidden = true;
      e.Layout.Bands[1].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Hidden = true;
      e.Layout.Bands[0].Columns["IsColor"].Hidden = true;
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["StandardCBM"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalUnrelease"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.000}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,###}";
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,###}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[2].Appearance.TextHAlign = HAlign.Right;
      for (int i = 0; i < ultGridSearch.DisplayLayout.Bands[1].Columns.Count; i++)
      {
        string columnName = ultGridSearch.DisplayLayout.Bands[1].Columns[i].Header.Caption;
        if (string.Compare(columnName, "OpenWO", true) == 0 || string.Compare(columnName, "Workorder", true) == 0 || string.Compare(columnName, "IsSubCon", true) == 0 || string.Compare(columnName, "Selected", true) == 0 || string.Compare(columnName, "ShipDate", true) == 0 || string.Compare(columnName, "Ship Date(Release Wo)", true) == 0)
        {
          ultGridSearch.DisplayLayout.Bands[1].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
          ultGridSearch.DisplayLayout.Bands[1].Columns[i].CellActivation = Activation.ActivateOnly;
      }
    }
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultGridSearch.Rows.ExpandAll(true);
      }
      else
      {
        ultGridSearch.Rows.CollapseAll(true);
      }
    }
    private void ultGridSearch_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      //open SO form
      try
      {
        UltraGridRow row = ultGridSearch.Selected.Rows[0];
        viewPLN_01_004 uc = new viewPLN_01_004();
        long pid = long.MinValue;
        try
        {
          pid = DBConvert.ParseLong(row.Cells["SaleOrderPid"].Value.ToString());
        }
        catch
        {
        }
        if (pid > 0)
        {
          uc.saleOrderPid = pid;
          Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE SALE ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
        }
      }
      catch
      { }

      //open WO form
      try
      {
        UltraGridRow row = ultGridSearch.Selected.Rows[0];
        long pid = DBConvert.ParseLong(row.Cells["WO"].Value.ToString());
        if (pid > 0)
        {
          viewPLN_02_002 uc = new viewPLN_02_002();
          uc.woPid = pid;
          Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE WORK ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
        }
        else
        {
          return;
        }
      }
      catch
      {

      }
    }
    private void btnWipStatus_Click(object sender, EventArgs e)
    {
      viewPLN_10_002 view = new viewPLN_10_002();
      Shared.Utility.WindowUtinity.ShowView(view, "WORK IN PROCESS STATUS", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Normal);
    }
    private void ultGridAddWord_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultGridAddWord);
      e.Layout.Bands[0].Columns["ShipDate"].Header.Caption = "Ship Date(Release Wo)";
      e.Layout.Bands[0].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleOrderDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["Workorder"].Hidden = true;
      //e.Layout.Bands[0].Columns.Add("OldWo");
      //e.Layout.Bands[0].Columns.Add("PIDDetailInsert");
      e.Layout.Bands[0].Columns["PIDDetailInsert"].Hidden = true;
      e.Layout.Bands[0].Columns["OldWo"].Hidden = true;
      e.Layout.Bands[0].Columns["Qty"].Hidden = true;
      e.Layout.Bands[0].Columns["SO Remain"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCBM"].Header.Caption = "Total CBM";
      e.Layout.Bands[0].Columns["Checked"].Hidden = true;
      e.Layout.Bands[0].Columns["IsSubCon"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["ItemCBM"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["OpenWO"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.000}";
      e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[2].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OpenWO"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["ItemCBM"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["SO Remain"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["UnRelease"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Shipped Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Wo"].CellAppearance.TextHAlign = HAlign.Right;


    }
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool check = true;
      check = this.Save();
      if (check == true)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      btnSearch_Click(sender, e);
    }
    private void ultGridAddWord_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      if (colName == "openwo")
      {
        if (DBConvert.ParseInt(e.Cell.Row.Cells["OpenWO"].Text.ToString()) <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0115", "Open WO");
          e.Cancel = true;
        }
        if (DBConvert.ParseInt(e.Cell.Row.Cells["Unrelease"].Text.ToString()) < DBConvert.ParseInt(e.Cell.Row.Cells["OpenWO"].Text.ToString()))
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "OpenWO", "Unrelease");
          e.Cancel = true;
        }
      }

    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();

    }
    private void btnSaveWo_Click(object sender, EventArgs e)
    {
      if (txtWorkOrder.Text.Trim().Length == 0 || DBConvert.ParseLong(txtWorkOrder.Text.Trim()) == long.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Work Order");
        return;
      }
      else
      {
        string commandText = "SELECT COUNT(*) FROM TblPLNWorkOrder WHERE Pid = " + DBConvert.ParseLong(txtWorkOrder.Text.Trim());
        int count = (int)(DataBaseAccess.ExecuteScalarCommandText(commandText));
        if (count > 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0006", "Work Order");
          return;
        }
      }
      if (ultCBType.Value == null)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Type");
        return;
      }
      wopid = this.SaveWorkOrder();
    }
    private void txtWorkOrder_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && (Convert.ToInt32(e.KeyChar) != 44))
      {
        e.Handled = true;
      }
    }
    private void ultGridSearch_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {

      if (DBConvert.ParseInt(e.Cell.Row.Cells["Unrelease"].Text.Trim()) == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0110", "Unrelease");
        e.Cancel = true;
      }
      else
      {
        if (e.Cell.Column.ToString().ToLower() == "workorder")
        {
          if ((e.Cell.Row.Cells["Workorder"].Text.Trim().Length > 0) && DBConvert.ParseInt(e.Cell.Row.Cells["Workorder"].Text.Trim()) <= 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0115", "Work Order");
            e.Cancel = true;
          }
          else if (DBConvert.ParseInt(e.Cell.Row.Cells["Workorder"].Text.Trim()) > 0)
          {
            string commandText = string.Format(@"SELECT DISTINCT WID.WoInfoPID FROM TblPLNWOInfoDetailGeneral WID LEFT JOIN TblPLNWorkOrderConfirmedDetails WCD ON WID.WoInfoPID = WCD.WorkOrderPid WHERE WCD.WorkOrderPid IS NULL ORDER BY WID.WoInfoPID DESC ");
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            string wo = e.Cell.Row.Cells["Workorder"].Text.Trim();
            if (dt.Select("WoInfoPID=' " + wo + " ' ").Length == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0011", "Work Order");
              e.Cancel = true;
            }
          }
        }
        if (e.Cell.Column.ToString().ToLower() == "openwo")
        {
          if (DBConvert.ParseInt(e.Cell.Row.Cells["OpenWO"].Text.ToString()) <= 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0115", "Open WO");
            e.Cancel = true;
          }
          if (DBConvert.ParseInt(e.Cell.Row.Cells["Unrelease"].Text.Trim()) < DBConvert.ParseInt(e.Cell.Row.Cells["OpenWO"].Text.Trim()))
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "OpenWO", "Unrelease");
            e.Cancel = true;
          }
        }
      }
    }
    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkHide.Checked)
      {
        this.groupBox3.Visible = false;
      }
      else
      {
        this.groupBox3.Visible = true;
      }
    }
    private void btnSaveDetailWoOld_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool check = true;
      check = this.SaveDetalWoOld();
      if (check == true)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
        this.btnSearch_Click(sender, e);
      }

    }
    private void btnFinish_Click(object sender, EventArgs e)
    {
      DataTable dt = new DataTable();
      dt = ((DataTable)ultGridAddWord.DataSource).Clone();
      ultGridAddWord.DataSource = dt;
      dtAddWork = ((DataTable)ultGridAddWord.DataSource).Clone();
    }
    public void BOMShowItemImage(UltraGrid ultGridData, GroupBox groupItemImage, PictureBox pictureItem, bool showImage)
    {
      try
      {
        if (showImage)
        {
          UltraGridRow row = ultGridData.Selected.Rows[0];
          string itemCode = row.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          if (itemCode.Length > 0 && revision != int.MinValue)
          {
            groupItemImage.Text = string.Format("Item:{0}/{1}", itemCode, revision);
            pictureItem.ImageLocation = FunctionUtility.BOMGetItemImage(itemCode, revision);
            Point xy = new Point();
            int yMax = ultGridData.Location.Y + ultGridData.Height;
            xy.X = ultGridData.Location.X + row.Cells["ItemCode"].Width + row.Cells["Revision"].Width;
            xy.Y = ultGridData.Location.Y + (row.Cells["ItemCode"].Height * (row.Index + 2));
            if (xy.Y + groupItemImage.Height > yMax)
            {
              xy.Y = yMax - groupItemImage.Height;
            }
            groupItemImage.Location = xy;
            groupItemImage.Visible = true;
          }
          else
          {
            groupItemImage.Text = string.Empty;
          }
        }
        else
        {
          groupItemImage.Visible = false;
        }
      }
      catch
      {
        groupItemImage.Text = string.Empty;
      }
    }
    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      BOMShowItemImage(ultGridSearch, groupItemImage, pictureItem, chkShowImage.Checked);
    }
    private void ultGridSearch_MouseClick(object sender, MouseEventArgs e)
    {
      BOMShowItemImage(ultGridSearch, groupItemImage, pictureItem, chkShowImage.Checked);
    }
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ultdtShipdateFrom.Value = null;
      this.ultdtShipdateTo.Value = null;
      this.txtUnreleaseFrom.Text = string.Empty;
      this.txtUnreleaseTo.Text = string.Empty;
      this.chkAll.Checked = false;
      this.chkSameCarcass.Checked = false;
      this.chkSubcon.Checked = false;
      this.ChkRelationShip.Checked = false;
      this.txtItemCode.Text = String.Empty;
      this.txtSaleCode.Text = String.Empty;
      this.cmbCustomer.Text = String.Empty;
    }
    private void txtUnreleaseFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    private void txtUnreleaseTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    private void ultdtShipdateFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    private void ultdtShipdateTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    private void ultGridSearch_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    private void txtSaleCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    private void cmbCustomer_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    private void ultGridAddWord_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (loopFlag == 1)
      {
        return;
      }
      int index = e.Cell.Row.Index;
      string colName = e.Cell.Column.ToString().ToLower();
      if (colName == "shipdate" && chkAllShipdate.Checked)
      {
        loopFlag = 1;
        for (int i = 0; i < ultGridAddWord.Rows.Count; i++)
        {
          ultGridAddWord.Rows[i].Cells["ShipDate"].Value = ultGridAddWord.Rows[index].Cells["ShipDate"].Value;
        }
        loopFlag = 0;
      }
    }
    private void chkAllSubCon_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultGridAddWord.Rows.Count; i++)
      {
        if (chkAllSubCon.Checked)
        {
          ultGridAddWord.Rows[i].Cells["IsSubCon"].Value = 1;
        }
        else
        {
          ultGridAddWord.Rows[i].Cells["IsSubCon"].Value = 0;
        }
      }
    }

    private void txtWorkOrder_TextChanged(object sender, EventArgs e)
    {
      StrWorkPID += "," + txtWorkOrder.Text.Trim();
    }

    private void btnGoWo_Click(object sender, EventArgs e)
    {
      if (DBConvert.ParseLong(txtWorkOrder.Text) > 0)
      {
        long pid = DBConvert.ParseLong(txtWorkOrder.Text);
        viewPLN_02_002 uc = new viewPLN_02_002();
        uc.woPid = pid;
        Shared.Utility.WindowUtinity.ShowView(uc, "UPDATE WORK ORDER INFO", false, Shared.Utility.ViewState.MainWindow);
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0013", "Work Order");
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultGridAddWord, "WorkOrder");
    }

    private void btnExportSearch_Click(object sender, EventArgs e)
    {
      ControlUtility.ExportToExcelWithDefaultPath(ultGridSearch, "WorkOrder");
    }
    #endregion Event


  }
}
