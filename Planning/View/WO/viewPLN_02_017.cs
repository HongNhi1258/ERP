/*
  Author      : Do Tam
  Date        : 17/05/2013
  Description : Change Note Confirm ShipDate Info
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using FormSerialisation;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_017 : MainUserControl
  {
    #region Field
    public long transactionPid = long.MinValue;
    private IList listDeletingPid = new ArrayList();
    private IList listDeletedPid = new ArrayList();
    private bool _1st = true;
    private bool _2nd = true;
    private bool _3rd = true;
    private bool _4th = true;
    private bool _5th = true;
    private int departmentCreated;
    private int Status = 0;
    private int intDep = 0;
    private string tabActive;
    private bool isLoadForm;
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    #endregion Field

    #region Init
    public viewPLN_02_017()
    {
      InitializeComponent();
    }

    private void LoadData()
    {
      //grpShowCol.Height = 80;
      //pnlRight.Visible = true;
      if (!this.isLoadForm)
      {
        _1st = btn1st.Visible;
        _2nd = btn2sd.Visible;
        _3rd = btn3rd.Visible;
        _4th = btn4th.Visible;
        _5th = btn5th.Visible;
      }

      if (_1st)
      {
        intDep = 1;
      }
      else if (_2nd)
      {
        intDep = 2;
      }
      else if (_3rd)
      {
        intDep = 3;
      }
      else if (_4th)
      {
        intDep = 4;
      }
      else
      {
        intDep = 5;
      }
      pnlRight.Visible = false;
      //if (_1st)
      //{
      //    lblStatus.Text = "PLN Account";
      //}
      //else
      //{
      //    lblStatus.Text = "CSD Account";
      //}
      //pnlRight.Visible = false;

      if (this.transactionPid == long.MinValue)
      {
        txtTransaction.Text = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FCSDGetNewOutputCodeForChangeNoteDeadline('DEA')", null).Rows[0][0].ToString();
        txtCreateBy.Text = SharedObject.UserInfo.EmpName.ToString();
        txtCreateDate.Text = DateTime.Today.ToString("dd/MM/yyyy");
        Status = 0;
        departmentCreated = 0;
      }
      else
      {
        DBParameter[] param = new DBParameter[2];
        param[0] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
        param[1] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);
        string storeName = "spPLNWoChangeNoteDeadline_Select";
        DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure(storeName, param);
        DataSet dsData = new DataSet();
        if (dsSource.Tables.Count == 3)
        {
          dsData = new DaiCo.Shared.DataSetSource.Planning.dsPLNWoChangeNoteDeadlineDetail();
          dsData.Tables["TransactionDetail"].Merge(dsSource.Tables[1]);
          dsData.Tables["ContainerStatus"].Merge(dsSource.Tables[2]);
        }
        else
        {
          dsData.Tables.Add(new DataTable());
          dsData.Tables[0].Merge(dsSource.Tables[1]);
        }
        Status = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["ST"].ToString());
        txtTransaction.Text = dsSource.Tables[0].Rows[0]["TransactionCode"].ToString();
        txtCreateBy.Text = dsSource.Tables[0].Rows[0]["CreateBy"].ToString();
        txtCreateDate.Text = dsSource.Tables[0].Rows[0]["CreateDate"].ToString();
        departmentCreated = DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["DepartmentCreated"].ToString());
        if (DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["Pending"].ToString()) > 0)
        {
          chkPending.Checked = true;
        }
        else
        {
          chkPending.Checked = false;
        }
        if (DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["WorkAreaPid"].ToString()) == DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST)
        {
          ultData.DataSource = dsData;
          tabActive = "CST";
        }
        else if (DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["WorkAreaPid"].ToString()) == DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack)
        {
          ultDataCAR.DataSource = dsData;
          tabActive = "CAR";
        }
        else if (DBConvert.ParseInt(dsSource.Tables[0].Rows[0]["WorkAreaPid"].ToString()) == DaiCo.Shared.Utility.ConstantClass.SubCon)
        {
          ultDataSUB.DataSource = dsData;
          tabActive = "SUB";
        }
        else
        {
          ultDataFOU.DataSource = dsData;
          tabActive = "FOU";
        }
      }
      if (this.tabActive != null)
      {
        tabDepartment.SelectedTab = tabDepartment.TabPages[this.tabActive];
      }
      else if (_1st)
      {
        tabDepartment.SelectedTab = tabDepartment.TabPages["CST"];
        this.tabActive = "CST";
      }
      else if (_2nd)
      {
        tabDepartment.SelectedTab = tabDepartment.TabPages["CST"];
        this.tabActive = "CST";
      }
      else if (_3rd)
      {
        tabDepartment.SelectedTab = tabDepartment.TabPages["CAR"];
        this.tabActive = "CAR";
      }
      else if (_4th)
      {
        tabDepartment.SelectedTab = tabDepartment.TabPages["SUB"];
        this.tabActive = "SUB";
      }
      else
      {
        tabDepartment.SelectedTab = tabDepartment.TabPages["FOU"];
        this.tabActive = "FOU";

      }
      if (this.Status == 2)
      {
        this.ConfirmStatus(true);
      }
      else if (_1st)
      {
        if (this.departmentCreated == 1)
        {
          if (this.Status > 0)
          {
            this.ConfirmStatus(true);
          }
        }
        else
        {
          if (this.Status == 1)
          {
            this.ApprovedStatus(true);
          }
          else if (this.transactionPid > 0)
          {
            this.ConfirmStatus(true);
          }
        }
      }
      else if (_2nd)
      {
        if (this.departmentCreated == 2)
        {
          if (this.Status > 0)
          {
            this.ConfirmStatus(true);
          }
        }
        else
        {
          if (this.Status == 1)
          {
            this.ApprovedStatus(true);
          }
          else if (this.transactionPid > 0)
          {
            this.ConfirmStatus(true);
          }
        }
      }
      else if (_3rd)
      {
        if (this.departmentCreated == 3)
        {
          if (this.Status > 0)
          {
            this.ConfirmStatus(true);
          }
        }
        else
        {
          if (this.Status == 1)
          {
            this.ApprovedStatus(true);
          }
          else if (this.transactionPid > 0)
          {
            this.ConfirmStatus(true);
          }
        }

      }
      else if (_4th)
      {
        if (this.departmentCreated == 4)
        {
          if (this.Status > 0)
          {
            this.ConfirmStatus(true);
          }
        }
        else
        {
          if (this.Status == 1)
          {
            this.ApprovedStatus(true);
          }
          else if (this.transactionPid > 0)
          {
            this.ConfirmStatus(true);
          }
        }
      }
      else
      {
        if (this.departmentCreated == 5)
        {
          if (this.Status > 0)
          {
            this.ConfirmStatus(true);
          }
        }
        else
        {
          if (this.Status == 1)
          {
            this.ApprovedStatus(true);
          }
          else if (this.transactionPid > 0)
          {
            this.ConfirmStatus(true);
          }
        }
      }
      if (this.Status > 0)
      {
        grpSearch.Visible = false;
        grpCar.Visible = false;
        grpSub.Visible = false;
        grpFou.Visible = false;
        chkHideSearch.Visible = false;
      }
      else
      {


        if (this.transactionPid > 0 && this.departmentCreated != intDep)
        {
          chkHideSearch.Visible = false;
          grpSearch.Visible = false;
          grpCar.Visible = false;
          grpSub.Visible = false;
          grpFou.Visible = false;
        }
        else
        {
          chkHideSearch.Visible = true;
          grpSearch.Visible = true;
          grpCar.Visible = true;
          grpSub.Visible = true;
          grpFou.Visible = true;
        }

      }
    }
    #endregion Init

    #region Function
    public void Search()
    {
      this.isLoadForm = false;
      dt_DeadlineFrom.Value = DateTime.Today;
      dt_DeadlineTo.Value = DateTime.Today.AddDays(7);
      this.LoadData();
      if (!_1st)
      {
        btnRefresh.Visible = false;
      }
      //FormSerialisor.Deserialise(this, System.Windows.Forms.Application.StartupPath + @"\viewPLN_02_017.xml");
      this.isLoadForm = true;
    }
    /// <summary>
    /// Load Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_017_Load(object sender, EventArgs e)
    {
      this.isLoadForm = false;
      dt_DeadlineFrom.Value = DateTime.Today;
      dt_DeadlineTo.Value = DateTime.Today.AddDays(7);
      this.LoadData();
      if (!_1st)
      {
        btnRefresh.Visible = false;
      }
      //FormSerialisor.Deserialise(this, System.Windows.Forms.Application.StartupPath + @"\viewPLN_02_017.xml");
      this.isLoadForm = true;
    }
    private void ExportInformation(UltraGrid ultraGridIF)
    {
      DataSet ds = (DataSet)ultraGridIF.DataSource;
      DataTable dt = new DataTable();
      if (ds != null)
      {
        dt = ds.Tables[0].Clone();
        dt.Merge(ds.Tables[0]);
        DataSet dd = new DataSet();
        dd.Tables.Add(dt);
        UltraGrid UG = new UltraGrid();
        UG.DataSource = dd;
        this.Controls.Add(UG);
        ControlUtility.ExportToExcelWithDefaultPath(UG, "Information");
      }
    }
    private void ConfirmStatus(bool isConfirm)
    {
      chkHide.Checked = isConfirm;
      chkHide.Enabled = !isConfirm;
      chkHideCAR.Checked = isConfirm;
      chkHideCAR.Enabled = !isConfirm;
      chkHideSUB.Checked = isConfirm;
      chkHideSUB.Enabled = !isConfirm;
      chkHideFOU.Checked = isConfirm;
      chkHideFOU.Enabled = !isConfirm;
      chkConfirm.Checked = isConfirm;
      chkConfirm.Enabled = !isConfirm;
      btnSave.Visible = !isConfirm;
      chkHideSearch.Checked = isConfirm;
    }
    private void ApprovedStatus(bool isConfirm)
    {
      chkHide.Checked = isConfirm;
      chkHide.Enabled = !isConfirm;
      chkHideCAR.Checked = isConfirm;
      chkHideCAR.Enabled = !isConfirm;
      chkHideSUB.Checked = isConfirm;
      chkHideSUB.Enabled = !isConfirm;
      chkHideFOU.Checked = isConfirm;
      chkHideFOU.Enabled = !isConfirm;
    }
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        if (!chkHide.Checked)
        {
          this.SearchData(DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST);
          return true;
        }
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    /// <summary>
    /// Load Column Name
    /// </summary>
    private void LoadColumnName(int WorkAreaPid)
    {
      DataTable dtNew = new DataTable();
      DataTable dtColumn = new DataTable();
      if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST)
      {
        dtColumn = ((DataSet)ultInformation.DataSource).Tables[0];
      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack)
      {
        dtColumn = ((DataSet)ultInformationCAR.DataSource).Tables[0];
      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.SubCon)
      {
        dtColumn = ((DataSet)ultInformationSUB.DataSource).Tables[0];
      }
      else
      {
        dtColumn = ((DataSet)ultInformationFOU.DataSource).Tables[0];
      }
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        dtNew.Columns[column.ColumnName].DefaultValue = 0;

        if (string.Compare(column.ColumnName, "Revision", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
        if (string.Compare(column.ColumnName, "CarcassCode", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
        if (string.Compare(column.ColumnName, "Wo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
        if (string.Compare(column.ColumnName, "Qty", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
        if (string.Compare(column.ColumnName, "Deadline", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST)
      {
        ultShowColumn.DataSource = dtNew;
        ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack)
      {
        ultShowColumnCAR.DataSource = dtNew;
        ultShowColumnCAR.Rows[0].Appearance.BackColor = Color.LightBlue;
      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.SubCon)
      {
        ultShowColumnSUB.DataSource = dtNew;
        ultShowColumnSUB.Rows[0].Appearance.BackColor = Color.LightBlue;
      }
    }

    /// <summary>
    /// Set Status Column When Search
    /// </summary>
    private void SetStatusColumn(int WorkAreaPid)
    {
      if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST)
      {
        for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
        {
          UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
          if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
          {
            ultInformation.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
          }
        }
      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack)
      {
        for (int colIndex = 1; colIndex < ultShowColumnCAR.DisplayLayout.Bands[0].Columns.Count; colIndex++)
        {
          UltraGridCell cell = ultShowColumnCAR.Rows[0].Cells[colIndex];
          if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
          {
            ultInformationCAR.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
          }
        }
      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.SubCon)
      {
        for (int colIndex = 1; colIndex < ultShowColumnSUB.DisplayLayout.Bands[0].Columns.Count; colIndex++)
        {
          UltraGridCell cell = ultShowColumnSUB.Rows[0].Cells[colIndex];
          if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
          {
            ultInformationSUB.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
          }
        }
      }


    }

    /// <summary>
    /// Save Transaction Master
    /// </summary>
    /// <returns></returns>
    private bool SaveMaster()
    {
      DBParameter[] input = new DBParameter[10];
      if (this.transactionPid != long.MinValue)
      {
        input[0] = new DBParameter("@Pid", DbType.Int64, this.transactionPid);
      }
      input[1] = new DBParameter("@TransactionCode", DbType.AnsiString, 16, txtTransaction.Text);
      int confirm = 0;
      if (chkConfirm.Checked)
      {
        confirm = 1;
      }
      else
      {
        confirm = 0;
      }
      input[2] = new DBParameter("@Status", DbType.Int32, confirm);
      input[3] = new DBParameter("@CurrencyPid", DbType.Int32, SharedObject.UserInfo.UserPid);
      long WorkAreaPid = long.MinValue;
      int department = 0;
      if (_1st)
      {
        department = 1;
        if (this.tabActive == "CAR")
        {
          WorkAreaPid = DBConvert.ParseLong(DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack.ToString());
        }
        else if (this.tabActive == "FOU")
        {
          WorkAreaPid = DBConvert.ParseLong(DaiCo.Shared.Utility.ConstantClass.WorkArea_Other.ToString());
        }
        else if (this.tabActive == "SUB")
        {
          WorkAreaPid = DBConvert.ParseLong(DaiCo.Shared.Utility.ConstantClass.SubCon.ToString());
        }
        else
        {
          WorkAreaPid = DBConvert.ParseLong(DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST.ToString());

        }
      }
      else if (_2nd)
      {
        department = 2;
        WorkAreaPid = DBConvert.ParseLong(DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST.ToString());

      }
      else if (_3rd)
      {
        department = 3;
        WorkAreaPid = DBConvert.ParseLong(DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack.ToString());

      }
      else if (_4th)
      {
        department = 4;
        WorkAreaPid = DBConvert.ParseLong(DaiCo.Shared.Utility.ConstantClass.SubCon.ToString());

      }
      else
      {
        WorkAreaPid = DBConvert.ParseLong(DaiCo.Shared.Utility.ConstantClass.WorkArea_Other.ToString());
        department = 5;
      }
      input[4] = new DBParameter("@DepartmentCreated", DbType.Int32, department);
      input[5] = new DBParameter("@WorkAreaPid", DbType.Int64, WorkAreaPid);

      MainUserControl c = (MainUserControl)this;
      string strTypeObject = c.GetType().FullName + "," + c.GetType().Namespace.Split('.')[1];
      string strTitle = SharedObject.tabContent.TabPages[SharedObject.tabContent.SelectedIndex].Text;
      string strFileName = c.Name + ".xml";
      MemoryStream stream = new MemoryStream();
      stream = FormSerialisor.Serialise(c);
      byte[] file;
      file = stream.ToArray();
      stream.Close();

      input[6] = new DBParameter("@File", DbType.Binary, file.Length, file);
      input[7] = new DBParameter("@TypeObject", DbType.AnsiString, 500, strTypeObject);
      input[8] = new DBParameter("@Title", DbType.AnsiString, 300, strTitle);
      input[9] = new DBParameter("@FileName", DbType.String, 300, strFileName);

      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteDeadline_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      this.transactionPid = resultSave;
      if (resultSave == 0)
      {
        return false;
      }
      return true;
    }

    private bool ConfirmTransaction()
    {
      try
      {
        DBParameter[] input = new DBParameter[5];
        if (this.transactionPid != long.MinValue)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, this.transactionPid);
        }
        int department = 0;
        if (_1st)
        {
          department = 1;
        }
        else if (_2nd)
        {
          department = 2;
        }
        else if (_3rd)
        {
          department = 3;
        }
        else if (_4th)
        {
          department = 4;
        }
        else
        {
          department = 5;
        }
        input[2] = new DBParameter("@DepartmentCreated", DbType.Int32, department);
        input[3] = new DBParameter("@CurrencyPid", DbType.Int32, SharedObject.UserInfo.UserPid);

        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteDeadline_Confirm", 900, input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        //this.transactionPid = resultSave;
        if (resultSave == 0)
        {
          return false;
        }
        return true;
      }
      catch
      {
        return false;
      }
    }
    /// <summary>
    /// Save Transaction detail
    /// </summary>
    /// <returns></returns>
    private bool SaveDetail(UltraGrid dr)
    {
      bool flag = true;
      //Delete only one time
      string strDelete = "";
      foreach (long pidDelete in this.listDeletedPid)
      {
        if (pidDelete > 0)
        {
          strDelete += pidDelete.ToString() + ",";
        }
      }
      if (strDelete.Length > 0)
      {
        strDelete = "," + strDelete;
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@DeleteList", DbType.String, 4000, strDelete);
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteDeadlineDetail_Delete", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }
      }
      for (int i = 0; i < dr.Rows.Count; i++)
      {
        if (dr.Name != "ultDataFOU")
        {
          if ((DBConvert.ParseDateTime(dr.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(dr.Rows[i].Cells["Reason"].Value.ToString()) <= 0)
              || (DBConvert.ParseDateTime(dr.Rows[i].Cells["SuggestDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(dr.Rows[i].Cells["SuggestReason"].Value.ToString()) <= 0))
          {
            flag = false;
          }
        }
      }
      if (!flag)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Reason");
        return flag;
      }
      for (int i = 0; i < dr.Rows.Count; i++)
      {
        DBParameter[] input = new DBParameter[16];
        if (DBConvert.ParseLong(dr.Rows[i].Cells["ChangeDeadlineDetailPid"].Value.ToString()) != long.MinValue)
        {
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dr.Rows[i].Cells["ChangeDeadlineDetailPid"].Value.ToString()));
        }
        input[1] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
        if (dr.Rows[i].Cells["ItemCode"].Value.ToString().Trim().Length > 0)
        {
          input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, dr.Rows[i].Cells["ItemCode"].Value.ToString());
        }
        if (DBConvert.ParseInt(dr.Rows[i].Cells["Revision"].Value.ToString()) > 0)
        {
          input[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["Revision"].Value.ToString()));
        }
        input[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, dr.Rows[i].Cells["CarcassCode"].Value.ToString());

        if (DBConvert.ParseInt(dr.Rows[i].Cells["Reason"].Value.ToString()) > 0)
        {
          input[5] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["Reason"].Value.ToString()));
        }
        if (DBConvert.ParseDateTime(dr.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
        {
          input[6] = new DBParameter("@NewDeadline", DbType.DateTime, DBConvert.ParseDateTime(dr.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
        }
        if (DBConvert.ParseDateTime(dr.Rows[i].Cells["OldDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
        {
          input[7] = new DBParameter("@OldDeadline", DbType.DateTime, DBConvert.ParseDateTime(dr.Rows[i].Cells["OldDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
        }
        input[8] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["Qty"].Value.ToString()));
        input[9] = new DBParameter("@Approved", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["Approved"].Value.ToString()));
        if (dr.Rows[i].Cells["OtherRemark"].Value.ToString().Trim().Length > 0)
        {
          input[10] = new DBParameter("@OtherRemark", DbType.String, 500, dr.Rows[i].Cells["OtherRemark"].Value.ToString());
        }
        input[11] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(dr.Rows[i].Cells["Wo"].Value.ToString()));
        if (DBConvert.ParseDateTime(dr.Rows[i].Cells["SuggestDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
        {
          input[12] = new DBParameter("@SuggestDeadline", DbType.DateTime, DBConvert.ParseDateTime(dr.Rows[i].Cells["SuggestDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
          if (DBConvert.ParseInt(dr.Rows[i].Cells["SuggestReason"].Value.ToString()) > 0)
          {
            input[13] = new DBParameter("@SuggestReason", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["SuggestReason"].Value.ToString()));
          }
          if (dr.Rows[i].Cells["SuggestOtherReason"].Value.ToString().Trim().Length > 0)
          {
            input[14] = new DBParameter("@SuggestOtherReason", DbType.String, 500, dr.Rows[i].Cells["SuggestOtherReason"].Value.ToString());
          }

        }
        if (DBConvert.ParseInt(dr.Rows[i].Cells["Pending"].Value.ToString()) > 0)
        {
          input[15] = new DBParameter("@Pending", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["Pending"].Value.ToString()));
        }
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteDeadlineDetail_Edit", input, output);
        long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
        if (resultSave == 0)
        {
          flag = false;
        }

      }
      return flag;
    }

    /// <summary>
    /// Add Detail
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private bool AddDetail(DBParameter[] input)
    {
      bool flag = true;
      DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      DataBaseAccess.ExecuteStoreProcedure("spPLNWoChangeNoteDeadlineDetail_Edit", input, output);
      long resultSave = DBConvert.ParseLong(output[0].Value.ToString());
      if (resultSave == 0)
      {
        flag = false;
      }
      return flag;
    }

    /// <summary>
    /// Add Data
    /// </summary>
    private void AddData(UltraGrid dr, bool isChild)
    {
      for (int i = 0; i < dr.Rows.Count; i++)
      {
        if (isChild)
        {
          if (dr.Rows[i].Cells["Selected"].Value.ToString() == "1")
          {
            for (int j = 0; j < dr.Rows[i].ChildBands[0].Rows.Count; j++)
            {
              if (dr.Name != "ultInformationFOU")
              {
                DBParameter[] input = new DBParameter[15];
                input[1] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
                input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, dr.Rows[i].Cells["ItemCode"].Value.ToString());
                input[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["Revision"].Value.ToString()));
                input[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, dr.Rows[i].Cells["CarcassCode"].Value.ToString());
                if (DBConvert.ParseDateTime(dr.Rows[i].ChildBands[0].Rows[j].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                {
                  input[5] = new DBParameter("@NewDeadline", DbType.DateTime, DBConvert.ParseDateTime(dr.Rows[i].ChildBands[0].Rows[j].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
                }
                if (DBConvert.ParseDateTime(dr.Rows[i].Cells["Deadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                {
                  input[6] = new DBParameter("@OldDeadline", DbType.DateTime, DBConvert.ParseDateTime(dr.Rows[i].Cells["Deadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
                }
                input[7] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString()));
                input[8] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(dr.Rows[i].Cells["Wo"].Value.ToString()));
                this.AddDetail(input);
              }
              else
              {
                for (int g = 0; g < dr.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows.Count; g++)
                {
                  DataRow row = (dr.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[g].ListObject as DataRowView).Row;
                  if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                  {
                    DBParameter[] input = new DBParameter[15];
                    input[1] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
                    input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, dr.Rows[i].Cells["ItemCode"].Value.ToString());
                    input[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["Revision"].Value.ToString()));
                    input[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, dr.Rows[i].Cells["CompCode"].Value.ToString());
                    if (DBConvert.ParseDateTime(dr.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[g].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                    {
                      input[5] = new DBParameter("@NewDeadline", DbType.DateTime, DBConvert.ParseDateTime(dr.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[g].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
                    }
                    if (DBConvert.ParseDateTime(dr.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[g].Cells["OldDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
                    {
                      input[6] = new DBParameter("@OldDeadline", DbType.DateTime, DBConvert.ParseDateTime(dr.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[g].Cells["OldDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
                    }
                    input[7] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[g].Cells["Qty"].Value.ToString()));
                    input[8] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(dr.Rows[i].Cells["Wo"].Value.ToString()));
                    this.AddDetail(input);
                  }
                }
              }
            }
          }
        }
        else
        {
          if (DBConvert.ParseDateTime(dr.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != null)
          {
            if (dr.Rows[i].Cells["Selected"].Value.ToString() == "1")
            {
              DBParameter[] input = new DBParameter[15];
              input[1] = new DBParameter("@transactionPid", DbType.Int64, this.transactionPid);
              input[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, dr.Rows[i].Cells["ItemCode"].Value.ToString());
              input[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["Revision"].Value.ToString()));
              input[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, dr.Rows[i].Cells["CarcassCode"].Value.ToString());

              if (DBConvert.ParseDateTime(dr.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
              {
                input[5] = new DBParameter("@NewDeadline", DbType.DateTime, DBConvert.ParseDateTime(dr.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
              }
              if (DBConvert.ParseDateTime(dr.Rows[i].Cells["Deadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
              {
                input[6] = new DBParameter("@OldDeadline", DbType.DateTime, DBConvert.ParseDateTime(dr.Rows[i].Cells["Deadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
              }
              input[7] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dr.Rows[i].Cells["Qty"].Value.ToString()));
              input[8] = new DBParameter("@WoPid", DbType.Int64, DBConvert.ParseLong(dr.Rows[i].Cells["Wo"].Value.ToString()));

              this.AddDetail(input);
            }

          }
        }
      }
    }

    /// <summary>
    /// Search Condition to adding detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.SearchData(DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST);
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Check valid
    /// </summary>
    /// <returns></returns>
    //private int CheckValid()
    //{
    //    bool checkReason = true;
    //    if (_1st)
    //    {
    //        for (int i = 0; i < ultData.Rows.Count; i++)
    //        {
    //            if (DBConvert.ParseInt(ultData.Rows[i].Cells["Reason"].Value.ToString()) < 0)
    //            {
    //                ultData.Rows[i].Cells["Reason"].Appearance.BackColor = Color.Yellow;
    //                checkReason = false;
    //            }
    //            else
    //            {
    //                ultData.Rows[i].Cells["Reason"].Appearance.BackColor = Color.White;
    //            }
    //        }
    //    }
    //    if (!checkReason)
    //    {
    //        return -1;
    //    }
    //    return 0;
    //}

    #endregion Function

    #region Event
    /// <summary>
    /// Load Transaction Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = true;
      //e.Layout.Override.RowSizing = RowSizing.AutoFree;
      //e.Layout.Override.CellMultiLine = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Wo"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["OldDeadline"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["NewDeadline"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Revision"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Approved"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Pending"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Qty"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Reason"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["Approved"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Pending"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["NewDeadline"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["OldDeadline"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["SuggestDeadline"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["SuggestReason"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Reason"].CellAppearance.BackColor = Color.LightBlue;
      try
      {
        e.Layout.Bands[0].Columns["CST%"].Header.Fixed = true;
        e.Layout.Bands[1].Columns["Wo"].Hidden = true;
        e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
        e.Layout.Bands[1].Columns["Revision"].Hidden = true;
        e.Layout.Bands[1].Columns["HighLight"].Hidden = true;
        e.Layout.Bands[1].Columns["StatusWIP"].Width = 450;
        e.Layout.Bands[1].Columns["ContainerNo"].Width = 150;
        e.Layout.Bands[0].Columns["CarcassCode"].Width = 100;
      }
      catch
      { }
      try
      {
        this.FormatUDataGrid(ultData);
      }
      catch
      { }
      try
      {
        this.FormatUDataGrid(ultDataCAR);
      }
      catch
      { }
      try
      {
        this.FormatUDataGrid(ultDataSUB);
      }
      catch
      { }
      try
      {
        this.FormatUDataGrid(ultDataFOU);
      }
      catch
      { }

      //for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      //{
      //    e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      //}
      if (Status == 2)
      {
        e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      }
      //if (!_1st)
      //{
      if (Status == 1)
      {
        e.Layout.Bands[0].Columns["Approved"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["Pending"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["SuggestDeadline"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["SuggestReason"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["SuggestOtherReason"].CellActivation = Activation.AllowEdit;
      }
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      //}
      //else
      //{
      if (Status == 0)
      {
        e.Layout.Bands[0].Columns["NewDeadline"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["Reason"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["OtherRemark"].CellActivation = Activation.AllowEdit;
        e.Layout.Bands[0].Columns["SuggestDeadline"].Hidden = true;
        e.Layout.Bands[0].Columns["SuggestReason"].Hidden = true;
        e.Layout.Bands[0].Columns["SuggestOtherReason"].Hidden = true;
        if (this.departmentCreated == this.intDep)
        {
          e.Layout.Override.AllowDelete = DefaultableBoolean.True;
        }
      }
      //}
      ultdrResion.DataSource = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable("Select Code, Value + '-' + Description Value from TblBOMCodeMaster where [Group] = 16006 ORDER BY Kind, Code", null);
      ultdrResion.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultdrResion.DisplayLayout.Bands[0].Columns["Value"].Width = 200;
      ultdrResion.ValueMember = "Code";
      ultdrResion.DisplayMember = "Value";

      e.Layout.Bands[0].Columns["Reason"].ValueList = ultdrResion;
      e.Layout.Bands[0].Columns["SuggestReason"].ValueList = ultdrResion;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ChangeDeadlineDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Approved"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Pending"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      if (e.Layout.Bands[0].Columns["Approved"].CellActivation == Activation.AllowEdit)
      {
        ChkApproveAll.Visible = true;
        chkApprovedAllCAR.Visible = true;
        chkApprovedAllSUB.Visible = true;
        chkApprovedAllFOU.Visible = true;
      }
      else
      {
        ChkApproveAll.Visible = false;
        chkApprovedAllCAR.Visible = false;
        chkApprovedAllSUB.Visible = false;
        chkApprovedAllFOU.Visible = false;
      }
    }

    private void FormatUDataGrid(UltraGrid dr)
    {
      if (dr.Name == "ultInformationFOU")
      {
        dr.DisplayLayout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        dr.DisplayLayout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
        dr.DisplayLayout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }
      else if (dr.Name == "ultDataFOU")
      {
        dr.DisplayLayout.Bands[0].Columns["SuggestDeadline"].Hidden = true;
        //dr.DisplayLayout.Bands[0].Columns["SuggestReason"].Hidden = true;
        dr.DisplayLayout.Bands[0].Columns["SuggestOtherReason"].Hidden = true;
        dr.DisplayLayout.Bands[0].Columns["SaleCode"].Hidden = true;
        dr.DisplayLayout.Bands[0].Columns["OldCode"].Hidden = true;
        //dr.DisplayLayout.Bands[0].Columns["Reason"].Hidden = true;
        dr.DisplayLayout.Bands[0].Columns["OtherRemark"].Hidden = true;
        dr.DisplayLayout.Bands[0].Columns["CarcassCode"].Header.Caption = "CompCode";
      }

      DataTable dtNew = ((DataSet)dr.DataSource).Tables[0];
      for (int i = 0; i < dtNew.Columns.Count; i++)
      {
        if (dtNew.Columns[i].DataType == typeof(Int32) || dtNew.Columns[i].DataType == typeof(float) || dtNew.Columns[i].DataType == typeof(Double))
        {
          dr.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dtNew.Columns[i].DataType == typeof(DateTime))
        {
          dr.DisplayLayout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }
      try
      {
        if (dr.Name == "ultInformationFOU")
        {
          dr.DisplayLayout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
          dr.DisplayLayout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
          dr.DisplayLayout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;
          dr.DisplayLayout.Bands[1].Columns["OldDeadline"].Hidden = false;
          dr.DisplayLayout.Bands[1].Columns["OldDeadline"].CellAppearance.BackColor = Color.LightBlue;
          dr.DisplayLayout.Bands[1].Columns["NewDeadline"].Hidden = true;
          dr.DisplayLayout.Bands[2].Columns["NewDeadline"].CellAppearance.BackColor = Color.Yellow;
          dr.DisplayLayout.Bands[2].Columns["Qty"].CellAppearance.BackColor = Color.Yellow;
          dr.DisplayLayout.Bands[2].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
        }
      }
      catch (Exception ex)
      { string a = ex.Message; }
      for (int i = 0; i < dr.Rows.Count; i++)
      {
        for (int j = 0; j < dr.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          if (dr.Rows[i].ChildBands[0].Rows[j].Cells["HighLight"].Value.ToString() != "1")
          {
            dr.Rows[i].ChildBands[0].Rows[j].Appearance.BackColor = Color.White;
          }
          else
          {
            dr.Rows[i].ChildBands[0].Rows[j].Appearance.BackColor = Color.Yellow;
          }
        }
      }

    }

    /// <summary>
    /// Search Condition
    /// </summary>
    private void SearchData(int WorkAreaPid)
    {
      DBParameter[] param = new DBParameter[15];
      // ItemCode
      string text = string.Empty, SaleNo = string.Empty, CusPONo = string.Empty, ItemCode = string.Empty, SaleCode = string.Empty, OldCode = string.Empty, WoFrom = string.Empty, WoTo = string.Empty, CarcassCode = string.Empty;
      DateTime dtDeadlineFrom = DateTime.MinValue;
      DateTime dtDeadlineTo = DateTime.MinValue;

      if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST)
      {
        SaleNo = txtSaleNo.Text;
        CusPONo = txtCusPONo.Text;
        ItemCode = this.txtItemCode.Text;
        SaleCode = this.txtSaleCode.Text;
        OldCode = this.txtOldCode.Text;
        WoFrom = txtWOFrom.Text.Trim().Replace("'", "");
        WoTo = txtWoTo.Text.Trim().Replace("'", "");
        CarcassCode = this.txtCarcassCode.Text;
        if (dt_DeadlineFrom.Value != null)
        {
          dtDeadlineFrom = DBConvert.ParseDateTime(dt_DeadlineFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        }
        if (dt_DeadlineTo.Value != null)
        {
          dtDeadlineTo = DBConvert.ParseDateTime(dt_DeadlineTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        }

      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack)
      {
        SaleNo = txtSaleNoCAR.Text;
        CusPONo = txtCusPONoCAR.Text;
        ItemCode = this.txtItemCodeCAR.Text;
        SaleCode = this.txtSaleCodeCAR.Text;
        OldCode = this.txtOldCodeCAR.Text;
        WoFrom = txtWoFromCAR.Text.Trim().Replace("'", "");
        WoTo = txtWoToCAR.Text.Trim().Replace("'", "");
        CarcassCode = this.txtCarcassCodeCAR.Text;
        if (dt_DeadlineFromCAR.Value != null)
        {
          dtDeadlineFrom = DBConvert.ParseDateTime(dt_DeadlineFromCAR.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        }
        if (dt_DeadlineToCAR.Value != null)
        {
          dtDeadlineTo = DBConvert.ParseDateTime(dt_DeadlineToCAR.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        }
      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.SubCon)
      {
        SaleNo = txtSaleNoSUB.Text;
        CusPONo = txtCusPoNoSUB.Text;
        ItemCode = this.txtItemCodeSUM.Text;
        SaleCode = this.txtSaleCodeSUB.Text;
        OldCode = this.txtOldCodeSUB.Text;
        WoFrom = txtWoFromSUB.Text.Trim().Replace("'", "");
        WoTo = txtWoToSUB.Text.Trim().Replace("'", "");
        CarcassCode = this.txtCarcassCodeSUB.Text;
        if (dt_DeadlineFromSUB.Value != null)
        {
          dtDeadlineFrom = DBConvert.ParseDateTime(dt_DeadlineFromSUB.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        }
        if (dt_DeadlineToSUB.Value != null)
        {
          dtDeadlineTo = DBConvert.ParseDateTime(dt_DeadlineToSUB.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        }
      }
      else
      {
        SaleNo = txtSaleNoFOU.Text;
        CusPONo = txtCusPoNoFOU.Text;
        ItemCode = this.txtItemCodeFOU.Text;
        SaleCode = this.txtSaleCodeFOU.Text;
        OldCode = this.txtOldCodeFOU.Text;
        WoFrom = txtWoFromFOU.Text.Trim().Replace("'", "");
        WoTo = txtWoToFOU.Text.Trim().Replace("'", "");
        CarcassCode = this.txtCarcassCodeFOU.Text;
        if (dt_DeadlineFromFOU.Value != null)
        {
          dtDeadlineFrom = DBConvert.ParseDateTime(dt_DeadlineFromFOU.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        }
        if (dt_DeadlineToFOU.Value != null)
        {
          dtDeadlineTo = DBConvert.ParseDateTime(dt_DeadlineToFOU.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        }
      }
      if (SaleNo.Length > 0)
      {
        SaleNo = string.Format("{0}", SaleNo.Remove(0, 1));
        param[0] = new DBParameter("@NoSet", DbType.AnsiString, 1000, SaleNo);
      }
      if (CusPONo.Length > 0)
      {
        CusPONo = string.Format("{0}", CusPONo.Remove(0, 1));
        param[1] = new DBParameter("@CusOrderNoSet", DbType.AnsiString, 1000, CusPONo);
      }

      DateTime shipdate;
      if (dtDeadlineFrom != DateTime.MinValue)
      {
        //DateTime orderDate = DBConvert.ParseDateTime(dt_DeadlineFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        param[2] = new DBParameter("@DeadlineFrom", DbType.DateTime, dtDeadlineFrom);
      }


      if (dtDeadlineTo != DateTime.MinValue)
      {
        //DateTime orderDate = DBConvert.ParseDateTime(dt_DeadlineTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        dtDeadlineTo = dtDeadlineTo.AddDays(1);
        param[3] = new DBParameter("@DeadlineTo", DbType.DateTime, dtDeadlineTo);
      }

      if (ItemCode.Length > 0)
      {
        param[4] = new DBParameter("@ItemCode", DbType.String, ItemCode);
      }
      // SaleCode

      if (SaleCode.Length > 0)
      {
        param[5] = new DBParameter("@SaleCode", DbType.String, SaleCode);
      }
      // OldCode
      if (OldCode.Length > 0)
      {
        param[6] = new DBParameter("@OldCode", DbType.String, OldCode);
      }
      //Wo

      if (DBConvert.ParseLong(WoFrom) > 0)
      {
        param[7] = new DBParameter("@WoFromPid", DbType.Int64, DBConvert.ParseLong(WoFrom));
      }

      if (DBConvert.ParseLong(WoTo) > 0)
      {
        param[8] = new DBParameter("@WoToPid", DbType.Int64, DBConvert.ParseLong(WoTo));
      }
      // Carcass Code

      if (CarcassCode.Length > 0)
      {
        param[9] = new DBParameter("@CarcassCode", DbType.String, CarcassCode);
      }

      param[10] = new DBParameter("@WorkAreaPid", DbType.Int64, DBConvert.ParseLong(WorkAreaPid.ToString()));
      param[11] = new DBParameter("@UserPid", DbType.Int32, SharedObject.UserInfo.UserPid);
      string strStoreName = "";
      if (WorkAreaPid == 1000)
      {
        strStoreName = "spPLNWoChangeNoteDeadlineForCom2_Search";
      }
      else
      {
        strStoreName = "spPLNWoChangeNoteDeadline_Search";
      }
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(strStoreName, 900, param);
      if (WorkAreaPid == 1000)
      {
        dsSource.Relations.Add(new DataRelation("dtParent_dtChildDeadline", new DataColumn[] { dsSource.Tables[0].Columns["Wo"], dsSource.Tables[0].Columns["ItemCode"], dsSource.Tables[0].Columns["Revision"], dsSource.Tables[0].Columns["CompCode"] }, new DataColumn[] { dsSource.Tables[1].Columns["Wo"], dsSource.Tables[1].Columns["ItemCode"], dsSource.Tables[1].Columns["Revision"], dsSource.Tables[1].Columns["CompCode"] }, false));
      }
      else
      {
        dsSource.Relations.Add(new DataRelation("dtParent_dtChildDeadline", new DataColumn[] { dsSource.Tables[0].Columns["Wo"], dsSource.Tables[0].Columns["ItemCode"], dsSource.Tables[0].Columns["Revision"] }, new DataColumn[] { dsSource.Tables[1].Columns["Wo"], dsSource.Tables[1].Columns["ItemCode"], dsSource.Tables[1].Columns["Revision"] }, false));
      }
      if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST)
      {
        ultInformation.DataSource = dsSource;
        if (ultShowColumn == null || ultShowColumn.Rows.Count == 0)
        {
          this.LoadColumnName(DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST);
        }
        else
        {
          this.SetStatusColumn(DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST);
        }
      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack)
      {
        dsSource.Relations.RemoveAt(0);
        dsSource.Tables.RemoveAt(1);
        dsSource.Tables.Add(new DataTable());
        dsSource.Tables[1].Merge(dsSource.Tables[0]);
        for (int i = 0; i < dsSource.Tables[1].Rows.Count; i++)
        {
          dsSource.Tables[1].Rows[i]["NewDeadline"] = dsSource.Tables[1].Rows[i]["Deadline"];
        }
        dsSource.Relations.Add(new DataRelation("dtParent_dtChildDefault", new DataColumn[] { dsSource.Tables[0].Columns["Wo"], dsSource.Tables[0].Columns["ItemCode"], dsSource.Tables[0].Columns["Revision"], dsSource.Tables[0].Columns["Deadline"] }, new DataColumn[] { dsSource.Tables[1].Columns["Wo"], dsSource.Tables[1].Columns["ItemCode"], dsSource.Tables[1].Columns["Revision"], dsSource.Tables[1].Columns["Deadline"] }, false));

        ultInformationCAR.DataSource = dsSource;

        if (ultShowColumnCAR == null || ultShowColumnCAR.Rows.Count == 0)
        {
          this.LoadColumnName(DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack);
        }
        else
        {
          this.SetStatusColumn(DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack);
        }
      }
      else if (WorkAreaPid == DaiCo.Shared.Utility.ConstantClass.SubCon)
      {
        dsSource.Relations.RemoveAt(0);
        dsSource.Tables.RemoveAt(1);
        dsSource.Tables.Add(new DataTable());
        dsSource.Tables[1].Merge(dsSource.Tables[0]);
        for (int i = 0; i < dsSource.Tables[1].Rows.Count; i++)
        {
          dsSource.Tables[1].Rows[i]["NewDeadline"] = dsSource.Tables[1].Rows[i]["Deadline"];
        }
        dsSource.Relations.Add(new DataRelation("dtParent_dtChildDefault", new DataColumn[] { dsSource.Tables[0].Columns["Wo"], dsSource.Tables[0].Columns["ItemCode"], dsSource.Tables[0].Columns["Revision"], dsSource.Tables[0].Columns["Deadline"] }, new DataColumn[] { dsSource.Tables[1].Columns["Wo"], dsSource.Tables[1].Columns["ItemCode"], dsSource.Tables[1].Columns["Revision"], dsSource.Tables[1].Columns["Deadline"] }, false));

        ultInformationSUB.DataSource = dsSource;
        if (ultShowColumnSUB == null || ultShowColumnSUB.Rows.Count == 0)
        {
          this.LoadColumnName(DaiCo.Shared.Utility.ConstantClass.SubCon);
        }
        else
        {
          this.SetStatusColumn(DaiCo.Shared.Utility.ConstantClass.SubCon);
        }
      }
      else
      {
        //dsSource.Relations.RemoveAt(0);
        //dsSource.Tables.RemoveAt(1);
        dsSource.Tables.Add(new DataTable());
        dsSource.Tables[2].Merge(dsSource.Tables[1]);
        //for (int i = 0; i < dsSource.Tables[1].Rows.Count; i++)
        //{
        //  dsSource.Tables[1].Rows[i]["NewDeadline"] = dsSource.Tables[1].Rows[i]["Deadline"];
        //}
        dsSource.Relations.Add(new DataRelation("dtParent_dtChildDefaultNew", new DataColumn[] { dsSource.Tables[1].Columns["Wo"], dsSource.Tables[1].Columns["ItemCode"], dsSource.Tables[1].Columns["Revision"], dsSource.Tables[1].Columns["CompCode"], dsSource.Tables[1].Columns["OldDeadline"] }, new DataColumn[] { dsSource.Tables[2].Columns["Wo"], dsSource.Tables[2].Columns["ItemCode"], dsSource.Tables[2].Columns["Revision"], dsSource.Tables[2].Columns["CompCode"], dsSource.Tables[2].Columns["OldDeadline"] }, false));

        ultInformationFOU.DataSource = dsSource;
        //if (ultShowColumnFOU == null || ultShowColumnFOU.Rows.Count == 0)
        //{
        //    this.LoadColumnName(1000);
        //}
        //else
        //{
        //    this.SetStatusColumn(1000);
        //}
      }
    }

    /// <summary>
    /// Hide areas
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    //private void chkHide_CheckedChanged(object sender, EventArgs e)
    //{
    //    if (chkHide.Checked)
    //    {
    //        grpInformation.Height = 140;
    //        grpInformation.Visible = false;
    //        tblData.Visible = false;
    //        grpData.Visible = true;
    //        grpShowCol.Visible = false;
    //    }
    //    else if (chkHide.Checked == false)
    //    {
    //        grpData.Visible = false;
    //        grpInformation.Visible = true;
    //        tblData.Visible = true;
    //        grpInformation.Height = 325;
    //        grpShowCol.Visible = true;
    //    }
    //}

    /// <summary>
    /// Hiden/Show ultragrid columns 
    /// </summary>
    private void ChkAll_CheckedChange(UltraGrid dr)
    {
      for (int colIndex = 1; colIndex < dr.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = dr.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          try
          {
            ultInformation.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
          }
          catch
          { }
          try
          {
            ultInformationCAR.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
          }
          catch
          { }
          try
          {
            ultInformationSUB.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
          }
          catch
          { }
          try
          {
            ultInformationFOU.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
          }
          catch
          { }
        }
      }
    }

    /// <summary>
    /// Show columns
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultShowColumn_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          try
          {
            ultInformation.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
          }
          catch { }
          try
          {
            ultInformationCAR.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
          }
          catch { }
          try
          {
            ultInformationSUB.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
          }
          catch { }
          try
          {
            ultInformationFOU.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
          }
          catch { }

        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          try
          {
            ultInformation.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
          }
          catch { }
          try
          {
            ultInformationCAR.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
          }
          catch { }
          try
          {
            ultInformationSUB.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
          }
          catch { }
          try
          {
            ultInformationFOU.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
          }
          catch { }
        }
      }
      else
      {
        for (int i = 1; i < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          row.Cells[i].Value = e.Cell.Text;
        }
        try
        {
          this.ChkAll_CheckedChange(ultShowColumn);
        }
        catch
        { }
        try
        {
          this.ChkAll_CheckedChange(ultShowColumnCAR);
        }
        catch
        { }
        try
        {
          this.ChkAll_CheckedChange(ultShowColumnSUB);
        }
        catch
        { }
      }
    }

    /// <summary>
    /// Init 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultShowColumn_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      DataTable dtColumn = (DataTable)ultShowColumn.DataSource;
      if (dtColumn == null)
      {
        dtColumn = (DataTable)ultShowColumnCAR.DataSource;
      }
      if (dtColumn == null)
      {
        dtColumn = (DataTable)ultShowColumnSUB.DataSource;
      }

      int count = dtColumn.Columns.Count;
      for (int i = 0; i < count; i++)
      {
        e.Layout.Bands[0].Columns[i].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      }
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      if (this.tabActive.Trim() != "CST")
      {
        e.Layout.Bands[0].Columns["NewDeadline"].Hidden = true;

      }
    }

    /// <summary>
    /// Add Detail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      AddButton(ultInformation);
    }
    private bool checkvalid(UltraGrid ultData)
    {
      int Qty = 0;
      int SubQty = 0;
      bool isTrue = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString()) == 1)
        {
          try
          {
            Qty = DBConvert.ParseInt(ultData.Rows[i].Cells["Qty"].Value.ToString());
            SubQty = 0;
            for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
            {
              if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString()) > 0)
              {
                SubQty += DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString());
              }
            }
            if (Qty != SubQty)
            {
              ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
              isTrue = false;
            }
          }
          catch
          {
            for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
            {
              Qty = DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString());
              SubQty = 0;
              for (int g = 0; g < ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows.Count; g++)
              {
                if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[g].Cells["Qty"].Value.ToString()) > 0)
                {
                  SubQty += DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].ChildBands[0].Rows[g].Cells["Qty"].Value.ToString());
                }
              }
              if (Qty != SubQty)
              {
                ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
                isTrue = false;
              }
            }
          }
        }
      }
      if (!isTrue)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Quantity");
      }
      return isTrue;
    }
    private void AddButton(UltraGrid dr)
    {
      bool chkValid = true;
      bool chkIsSelected = false;
      for (int i = 0; i < dr.Rows.Count; i++)
      {
        if (String.Compare(dr.Rows[i].Cells["Selected"].Value.ToString(), "1", true) == 0)
        {
          chkIsSelected = true;
        }
        else
          continue;
        // Parent
        if (String.Compare(dr.Rows[i].Cells["Selected"].Value.ToString(), "1", true) == 0
            && DBConvert.ParseDateTime(dr.Rows[i].Cells["NewDeadline"].Value.ToString(),
                USER_COMPUTER_FORMAT_DATETIME) == DateTime.MinValue)
        {
          dr.Rows[i].CellAppearance.BackColor = Color.Yellow;
          if (chkValid)
            chkValid = false;
        }
        else
        {
          dr.Rows[i].CellAppearance.BackColor = Color.White;
        }

        if (!chkIsSelected)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0115", "Data");
        }
        else if (chkValid)
        {
          // Save Master
          this.SaveMaster();

          // Add Data
          this.AddData(dr, false);

          // LoadData
          this.LoadData();
          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0003");

          // Search 
          if (this.tabActive == "CST")
            this.SearchData(DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST);
          else if (this.tabActive == "CAR")
            this.SearchData(DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack);
          else if (this.tabActive == "SUB")
            this.SearchData(DaiCo.Shared.Utility.ConstantClass.SubCon);
          else
            this.SearchData(1000);
        }
        else
        {
          Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "DeadLine");
        }
      }
    }
    private void AddItemButton(UltraGrid dr)
    {
      // Save Master
      this.SaveMaster();

      // Add Data
      this.AddData(dr, true);

      // LoadData
      this.LoadData();
      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0003");

      // Search 
      if (this.tabActive == "CST")
        this.SearchData(DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST);
      else if (this.tabActive == "CAR")
        this.SearchData(DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack);
      else if (this.tabActive == "SUB")
        this.SearchData(DaiCo.Shared.Utility.ConstantClass.SubCon);
      else
        this.SearchData(1000);

    }

    /// <summary>
    /// Save click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      //if (this.CheckValid() == -1)
      //{
      //    Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Reason");
      //}
      //else
      //{
      bool flag = true;
      if (tabDepartment.SelectedTab.Name == "CST")
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if ((DBConvert.ParseDateTime(ultData.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(ultData.Rows[i].Cells["Reason"].Value.ToString()) <= 0)
              || (DBConvert.ParseDateTime(ultData.Rows[i].Cells["SuggestDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(ultData.Rows[i].Cells["SuggestReason"].Value.ToString()) <= 0))
          {
            flag = false;
            break;
          }

        }
      }
      else if (tabDepartment.SelectedTab.Name == "CAR")
      {
        for (int i = 0; i < ultDataCAR.Rows.Count; i++)
        {
          if ((DBConvert.ParseDateTime(ultDataCAR.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(ultDataCAR.Rows[i].Cells["Reason"].Value.ToString()) <= 0)
              || (DBConvert.ParseDateTime(ultDataCAR.Rows[i].Cells["SuggestDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(ultDataCAR.Rows[i].Cells["SuggestReason"].Value.ToString()) <= 0))
          {
            flag = false;
            break;
          }
        }
      }
      else if (tabDepartment.SelectedTab.Name == "SUB")
      {
        for (int i = 0; i < ultDataSUB.Rows.Count; i++)
        {
          if ((DBConvert.ParseDateTime(ultDataSUB.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(ultDataSUB.Rows[i].Cells["Reason"].Value.ToString()) <= 0)
              || (DBConvert.ParseDateTime(ultDataSUB.Rows[i].Cells["SuggestDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(ultDataSUB.Rows[i].Cells["SuggestReason"].Value.ToString()) <= 0))
          {
            flag = false;
            break;
          }
        }
      }
      else
      {
        if (chkConfirm.Checked && this.departmentCreated != this.intDep)
        {
          DataTable dtFOU = ((DataSet)ultDataFOU.DataSource).Tables[0];
          DataRow[] r = dtFOU.Select("Approved = 1");
          if (r.Length == 0 && (Shared.Utility.WindowUtinity.ShowMessageConfirmFromText("No component is approved. Area you sure to confirm this note?") == DialogResult.No))
          {
            return;
          }
        }
      }
      //else 
      //{
      //    for (int i = 0; i < ultDataFOU.Rows.Count; i++)
      //    {
      //        if ((DBConvert.ParseDateTime(ultDataFOU.Rows[i].Cells["NewDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(ultDataFOU.Rows[i].Cells["Reason"].Value.ToString()) <= 0)
      //            || (DBConvert.ParseDateTime(ultDataFOU.Rows[i].Cells["SuggestDeadline"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue && DBConvert.ParseInt(ultDataFOU.Rows[i].Cells["SuggestReason"].Value.ToString()) <= 0))
      //        {
      //            flag = false;
      //            break;
      //        }
      //    }
      //}
      if (!flag)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("MSG0005", "Reason");
        return;
      }
      if (this.SaveMaster())
      {
        if (tabDepartment.SelectedTab.Name == "CST")
        {
          if (this.SaveDetail(ultData))
          {
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
          }
          else
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          }
        }
        else if (tabDepartment.SelectedTab.Name == "CAR")
        {
          if (this.SaveDetail(ultDataCAR))
          {
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
          }
          else
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          }
        }
        else if (tabDepartment.SelectedTab.Name == "SUB")
        {
          if (this.SaveDetail(ultDataSUB))
          {
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
          }
          else
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          }
        }
        else
        {
          if (this.SaveDetail(ultDataFOU))
          {
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
          }
          else
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
          }
        }
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      }
      bool confirmTransaction = this.ConfirmTransaction();
      //this.listDeletingPid = new ArrayList();
      //this.listDeletedPid = new ArrayList();

      // Load Data
      this.LoadData();
      // Search 
      if (this.Status == 0 && this.departmentCreated == intDep)
      {
        if (this.tabActive == "CST")
          this.SearchData(DaiCo.Shared.Utility.ConstantClass.WorkArea_QCToCST);
        else if (this.tabActive == "CAR")
          this.SearchData(DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack);
        else if (this.tabActive == "SUB")
          this.SearchData(DaiCo.Shared.Utility.ConstantClass.SubCon);
        else
          this.SearchData(1000);
      }
      //}
    }
    /// <summary>
    /// Init Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInformationFOU_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Layout.AutoFitColumns = true;
      try
      {
        this.FormatUDataGrid(ultInformationFOU);
      }
      catch
      { }

    }

    /// <summary>
    /// Init Information
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultInformation_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["Wo"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Revision"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["Selected"].Header.Fixed = true;
      e.Layout.Bands[1].Columns["Wo"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;
      try
      {
        e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.LightBlue;
        e.Layout.Bands[0].Columns["Qty"].Header.Fixed = true;
        e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
        e.Layout.Bands[0].Columns["Deadline"].Header.Fixed = true;
        e.Layout.Bands[0].Columns["Deadline"].CellAppearance.BackColor = Color.LightBlue;
        e.Layout.Bands[0].Columns["NewDeadline"].Header.Fixed = true;
      }
      catch
      { }
      try
      {
        if (this.tabActive.Trim() != "CST")
        {
          e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
          e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.True;
          for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
          {
            if (!(e.Layout.Bands[1].Columns[i].Key == "NewDeadline" || e.Layout.Bands[1].Columns[i].Key == "Qty"))
            {
              e.Layout.Bands[1].Columns[i].Hidden = true;
            }
          }
          try
          {
            for (int i = 0; i < e.Layout.Bands[2].Columns.Count; i++)
            {
              if (!(e.Layout.Bands[2].Columns[i].Key == "NewDeadline" || e.Layout.Bands[2].Columns[i].Key == "Qty"))
              {
                e.Layout.Bands[2].Columns[i].Hidden = true;
              }
            }
          }
          catch
          {
          }
          e.Layout.Bands[1].Layout.AutoFitColumns = false;
          e.Layout.Bands[1].Columns["NewDeadline"].Width = 150;
          e.Layout.Bands[1].Columns["Qty"].Width = 150;
          e.Layout.Bands[1].Columns["NewDeadline"].Header.Caption = "New Deadline";
          e.Layout.Bands[1].Columns["NewDeadline"].CellAppearance.BackColor = Color.LightBlue;
          e.Layout.Bands[1].Columns["Qty"].CellAppearance.BackColor = Color.LightBlue;
          e.Layout.Bands[0].Columns["NewDeadline"].Hidden = true;
          e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
        }

      }
      catch (Exception ex)
      { string a = ex.Message; }
      //e.Layout.Bands[0].Columns["ContainerPid"].Hidden = true;

      //DataTable dsNew = (DataTable)ultInformation.DataSource;
      //for (int i = 0; i < dsNew.Columns.Count; i++)
      //{
      //    if (dsNew.Columns[i].DataType == typeof(Int32) || dsNew.Columns[i].DataType == typeof(float) || dsNew.Columns[i].DataType == typeof(Double))
      //    {
      //        ultInformation.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      //    }
      //    else if (dsNew.Columns[i].DataType == typeof(DateTime))
      //    {
      //        ultInformation.DisplayLayout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
      //    }
      //}
      try
      {
        this.FormatUDataGrid(ultInformation);
      }
      catch
      { }
      try
      {
        this.FormatUDataGrid(ultInformationCAR);
      }
      catch
      { }
      try
      {
        this.FormatUDataGrid(ultInformationSUB);
      }
      catch
      { }
      try
      {
        this.FormatUDataGrid(ultInformationFOU);
      }
      catch
      { }

      e.Layout.Bands[0].Columns["Deadline"].Format = "dd-MMM-yyyy";
      e.Layout.Bands[0].Columns["Deadline"].CellActivation = Activation.ActivateOnly;


      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Select All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultInformation.Rows.Count; i++)
      {
        if (ultInformation.Rows[i].Cells["Selected"].Value.ToString() != "1")
        {
          ultInformation.Rows[i].Cells["Selected"].Value = true;
        }
        else
        {
          ultInformation.Rows[i].Cells["Selected"].Value = false;

        }
      }
    }




    /// <summary>
    /// Hide change state
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkHide_CheckStateChanged(object sender, EventArgs e)
    {
      //if (chkHide.CheckState == CheckState.Indeterminate)
      //{
      //    grpInformation.Height = 140;
      //    grpInformation.Visible = true;
      //    tblData.Visible = true;
      //    grpInformation.Visible = true;
      //    grpShowCol.Visible = true;
      //}
    }

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      foreach (Control o in tableLayoutPanel4.Controls)
      {
        if (o.GetType() == typeof(TextBox))
        {
          o.Text = "";
        }
      }
      dt_DeadlineFrom.Value = null;
      dt_DeadlineTo.Value = null;
    }

    /// <summary>
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      //FormSerialisor.Serialise(this, System.Windows.Forms.Application.StartupPath + @"\viewPLN_02_017.xml");
      this.CloseTab();
    }

    /// <summary>
    /// Before Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long detailPid = DBConvert.ParseLong(row.Cells["ChangeDeadlineDetailPid"].Value.ToString());
        if (detailPid != long.MinValue)
        {
          listDeletingPid.Add(detailPid);
        }
      }
    }

    /// <summary>
    /// After Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        listDeletedPid.Add(pid);
      }
    }
    #endregion Event

    private void chkHide_CheckedChanged(object sender, EventArgs e)
    {
      if (chkHide.Checked)
      {
        tableLayoutPanel7.Visible = false;
      }
      else
      {
        tableLayoutPanel7.Visible = true;

      }
      btnSearch.Visible = !chkHide.Checked;
      btnClear.Visible = !chkHide.Checked;
    }

    private void tabDepartment_Deselecting(object sender, TabControlCancelEventArgs e)
    {
      if (this.transactionPid > 0)
      {
        if (this.isLoadForm)
        {
          if (string.Compare(e.TabPage.Name, this.tabActive, true) == 0)
          {
            e.Cancel = true;
          }
        }
      }
      else if (!_1st)
      {
        if (this.isLoadForm)
        {
          if (string.Compare(e.TabPage.Name, this.tabActive, true) == 0)
          {
            e.Cancel = true;
          }
        }
      }
    }

    private void btnClearCAR_Click(object sender, EventArgs e)
    {
      foreach (Control o in tableLayoutPanel9.Controls)
      {
        if (o.GetType() == typeof(TextBox))
        {
          o.Text = "";
        }
      }
      dt_DeadlineFromCAR.Value = null;
      dt_DeadlineToCAR.Value = null;
    }

    private void tblClearSUB_Click(object sender, EventArgs e)
    {
      foreach (Control o in tableLayoutPanel18.Controls)
      {
        if (o.GetType() == typeof(TextBox))
        {
          o.Text = "";
        }
      }
      dt_DeadlineFromSUB.Value = null;
      dt_DeadlineToSUB.Value = null;
    }

    private void btnClearFOU_Click(object sender, EventArgs e)
    {
      foreach (Control o in tableLayoutPanel23.Controls)
      {
        if (o.GetType() == typeof(TextBox))
        {
          o.Text = "";
        }
      }
      dt_DeadlineFromFOU.Value = null;
      dt_DeadlineToFOU.Value = null;
    }

    private void btnSearchCAR_Click(object sender, EventArgs e)
    {
      btnSearchCAR.Enabled = false;
      this.SearchData(DaiCo.Shared.Utility.ConstantClass.WorkArea_Pack);
      btnSearchCAR.Enabled = true;
    }

    private void btnSearchSUB_Click(object sender, EventArgs e)
    {
      btnSearchSUB.Enabled = false;
      this.SearchData(DaiCo.Shared.Utility.ConstantClass.SubCon);
      btnSearchSUB.Enabled = true;
    }

    private void btnSearchFOU_Click(object sender, EventArgs e)
    {
      btnSearchFOU.Enabled = false;
      this.SearchData(1000);
      btnSearchFOU.Enabled = true;
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
      if (chkHideCAR.Checked)
      {
        tableLayoutPanel10.Visible = false;
      }
      else
      {
        tableLayoutPanel10.Visible = true;
      }
    }

    private void checkBox5_CheckedChanged(object sender, EventArgs e)
    {
      if (chkHideSUB.Checked)
      {
        tableLayoutPanel19.Visible = false;
      }
      else
      {
        tableLayoutPanel19.Visible = true;

      }
    }

    private void checkBox7_CheckedChanged(object sender, EventArgs e)
    {
      if (chkHideFOU.Checked)
      {
        tableLayoutPanel24.Visible = false;
      }
      else
      {
        tableLayoutPanel24.Visible = true;

      }
    }

    private void chkSelectAllFOU_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultInformationFOU.Rows.Count; i++)
      {
        if (ultInformationFOU.Rows[i].Cells["Selected"].Value.ToString() != "1")
        {
          ultInformationFOU.Rows[i].Cells["Selected"].Value = true;
        }
        else
        {
          ultInformationFOU.Rows[i].Cells["Selected"].Value = false;

        }
      }
    }

    private void chkSelectAllCAR_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultInformationCAR.Rows.Count; i++)
      {
        if (ultInformationCAR.Rows[i].Cells["Selected"].Value.ToString() != "1")
        {
          ultInformationCAR.Rows[i].Cells["Selected"].Value = true;
        }
        else
        {
          ultInformationCAR.Rows[i].Cells["Selected"].Value = false;

        }
      }
    }

    private void chkSelectAllSUB_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultInformationSUB.Rows.Count; i++)
      {
        if (ultInformationSUB.Rows[i].Cells["Selected"].Value.ToString() != "1")
        {
          ultInformationSUB.Rows[i].Cells["Selected"].Value = true;
        }
        else
        {
          ultInformationSUB.Rows[i].Cells["Selected"].Value = false;

        }
      }
    }

    private void btnAddFOU_Click(object sender, EventArgs e)
    {
      if (this.checkvalid(ultInformationFOU))
      {
        this.AddItemButton(ultInformationFOU);
      }
    }

    private void btnAddSUB_Click(object sender, EventArgs e)
    {
      if (this.checkvalid(ultInformationSUB))
      {
        this.AddItemButton(ultInformationSUB);
      }
    }

    private void btnAddCAR_Click(object sender, EventArgs e)
    {
      if (this.checkvalid(ultInformationCAR))
      {
        this.AddItemButton(ultInformationCAR);
      }
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      System.Threading.Thread Thead2 = new System.Threading.Thread(new System.Threading.ThreadStart(Progessbar));
      Thead2.Start();
      DBParameter[] inputParam = new DBParameter[2];
      DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerSummarizeTotalData_Insert", 900, inputParam);
      WindowUtinity.ShowMessageSuccess("MSG0059");
    }
    private void Progessbar()
    {

      ProgressForm form = new ProgressForm();
      form.Text = "Please Waiting";
      form.Argument = false;
      form.DoWork += new ProgressForm.DoWorkEventHandler(form_DoWork);
      StartForm(form);
      form.FormBorderStyle = FormBorderStyle.None;
    }
    void form_DoWork(ProgressForm sender, DoWorkEventArgs e)
    {
      bool throwException = (bool)e.Argument;

      for (int i = 0; i < 100; i++)
      {
        System.Threading.Thread.Sleep(600);
        sender.SetProgress(i, "Step " + i.ToString() + " %");
        if (sender.CancellationPending)
        {
          e.Cancel = true;
          return;
        }
      }

    }
    void StartForm(ProgressForm form)
    {
      DialogResult result = form.ShowDialog();
      if (result == DialogResult.Cancel)
        MessageBox.Show("Operation has been cancelled");
      if (result == DialogResult.Abort)
        MessageBox.Show("Exception:" + Environment.NewLine + form.Result.Error.Message);
    }

    private void ultInformationCAR_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (String.Compare(e.Cell.Column.Key, "Qty", true) == 0 && e.Cell.Row.ParentRow != null)
      {
        int idx = e.Cell.Row.ParentRow.Index;
        int Qty = DBConvert.ParseInt(e.Cell.Row.ParentRow.Cells["Qty"].Value.ToString());
        int SubQty = 0;
        for (int i = 0; i < e.Cell.Row.ParentRow.ChildBands[0].Rows.Count; i++)
        {
          if (DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Qty"].Text.ToString()) > 0)
          {
            SubQty += DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Qty"].Text.ToString());
          }
        }
        if (SubQty > Qty)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Qty");
          e.Cancel = true;
        }
      }
    }

    private void ultInformationCAR_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if ((String.Compare(e.Cell.Column.Key, "Qty", true) == 0 || String.Compare(e.Cell.Column.Key, "NewDeadline", true) == 0) && e.Cell.Row.ParentRow != null)
      {
        try
        {
          e.Cell.Row.ParentRow.Cells["Selected"].Value = 1;
        }
        catch
        {
          e.Cell.Row.ParentRow.ParentRow.Cells["Selected"].Value = 1;

        }
      }
    }

    private void tabDepartment_Selected(object sender, TabControlEventArgs e)
    {
      this.tabActive = e.TabPage.Name;

    }

    private void chkHideSearch_CheckedChanged(object sender, EventArgs e)
    {
      grpSearch.Visible = !chkHideSearch.Checked;
      grpCar.Visible = !chkHideSearch.Checked;
      grpSub.Visible = !chkHideSearch.Checked;
      grpFou.Visible = !chkHideSearch.Checked;
    }

    private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
    {

    }

    private void tableLayoutPanel12_Paint(object sender, PaintEventArgs e)
    {

    }

    private void btnImportCST_Click(object sender, EventArgs e)
    {
      this.SaveMaster();
      if (this.transactionPid != long.MinValue)
      {
        viewPLN_02_027 uc = new viewPLN_02_027();
        uc.transactionPid = this.transactionPid;
        WindowUtinity.ShowView(uc, "IMPORT DATA FOR CHANGE DEADLINE", true, ViewState.ModalWindow, FormWindowState.Maximized);
      }
    }

    private void btnImportCAR_Click(object sender, EventArgs e)
    {
      this.SaveMaster();
      if (this.transactionPid != long.MinValue)
      {
        viewPLN_02_027 uc = new viewPLN_02_027();
        uc.transactionPid = this.transactionPid;
        WindowUtinity.ShowView(uc, "IMPORT DATA FOR CHANGE DEADLINE", true, ViewState.ModalWindow, FormWindowState.Maximized);
      }
    }

    private void btnImportSUB_Click(object sender, EventArgs e)
    {
      this.SaveMaster();
      if (this.transactionPid != long.MinValue)
      {
        viewPLN_02_027 uc = new viewPLN_02_027();
        uc.transactionPid = this.transactionPid;
        WindowUtinity.ShowView(uc, "IMPORT DATA FOR CHANGE DEADLINE", true, ViewState.ModalWindow, FormWindowState.Maximized);
      }
    }

    private void btnImportFOU_Click(object sender, EventArgs e)
    {
      viewPLN_02_032 uc = new viewPLN_02_032();
      uc.transactionPid = this.transactionPid;
      WindowUtinity.ShowView(uc, "IMPORT DATA FOR CHANGE DEADLINE COM2", true, ViewState.MainWindow, FormWindowState.Maximized);
    }

    private void ChkApproveAll_CheckedChanged(object sender, EventArgs e)
    {
      int selected = (ChkApproveAll.Checked ? 1 : 0);
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].Cells["Approved"].Value = selected;
      }
    }

    private void chkApprovedAllCAR_CheckedChanged(object sender, EventArgs e)
    {
      int selected = (chkApprovedAllCAR.Checked ? 1 : 0);
      for (int i = 0; i < ultDataCAR.Rows.Count; i++)
      {
        ultDataCAR.Rows[i].Cells["Approved"].Value = selected;
      }
    }

    private void chkApprovedAllSUB_CheckedChanged(object sender, EventArgs e)
    {
      int selected = (chkApprovedAllSUB.Checked ? 1 : 0);
      for (int i = 0; i < ultDataSUB.Rows.Count; i++)
      {
        ultDataSUB.Rows[i].Cells["Approved"].Value = selected;
      }
    }

    private void chkApprovedAllFOU_CheckedChanged(object sender, EventArgs e)
    {
      int selected = (chkApprovedAllFOU.Checked ? 1 : 0);
      DataTable dtFOU = ((DataSet)ultDataFOU.DataSource).Tables[0];
      for (int i = 0; i < dtFOU.Rows.Count; i++)
      {
        DataRow row = dtFOU.Rows[i];
        row["Approved"] = selected;
        if (selected == 1)
        {
          row["Pending"] = 0;
          row["SuggestDeadline"] = DBNull.Value;
        }
      }
    }

    private void ultDataFOU_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (WindowUtinity.ShowMessageConfirmFromText("If you delete this row, all components in the same Wo & Item will be deleted. Are you sure?") != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }

      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long detailPid = DBConvert.ParseLong(row.Cells["ChangeDeadlineDetailPid"].Value.ToString());
        if (detailPid != long.MinValue)
        {
          listDeletingPid.Add(detailPid);
        }

        long wo = DBConvert.ParseLong(row.Cells["Wo"].Value);
        string itemCode = row.Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(row.Cells["Revision"].Value);
        string compCode = row.Cells["CarcassCode"].Value.ToString();
        long changeDeadlineDetailPid = DBConvert.ParseLong(row.Cells["ChangeDeadlineDetailPid"].Value);

        DataTable dtFOU = ((DataSet)ultDataFOU.DataSource).Tables[0];
        DataRow[] arrRows = dtFOU.Select(string.Format(@"Wo = {0} And ChangeDeadlineDetailPid <> {1} And ItemCode = '{2}'
            And Revision = {3} And CarcassCode = '{4}'", wo, changeDeadlineDetailPid, itemCode, revision, compCode));
        if (arrRows.Length > 0)
        {
          foreach (DataRow r in arrRows)
          {
            detailPid = DBConvert.ParseLong(r["ChangeDeadlineDetailPid"]);
            if (detailPid != long.MinValue)
            {
              listDeletingPid.Add(detailPid);
            }
            dtFOU.Rows.Remove(r);
          }
        }
      }
    }

    private void ultDataFOU_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      if ((string.Compare(columnName, "Approved", true) == 0) || (string.Compare(columnName, "Pending", true) == 0))
      {
        string otherColumnName = "Approved";
        if (string.Compare(columnName, "Approved", true) == 0)
        {
          otherColumnName = "Pending";
        }
        int selected = DBConvert.ParseInt(e.Cell.Text);
        int otherSelected = (selected == 1 ? 0 : 1);
        UltraGridRow row = e.Cell.Row;
        if (selected == 1)
        {
          row.Cells[otherColumnName].Value = otherSelected;
          row.Cells["SuggestDeadline"].Value = DBNull.Value;
        }

        long wo = DBConvert.ParseLong(row.Cells["Wo"].Value);
        string itemCode = row.Cells["ItemCode"].Value.ToString();
        int revision = DBConvert.ParseInt(row.Cells["Revision"].Value);
        string compCode = row.Cells["CarcassCode"].Value.ToString();
        long changeDeadlineDetailPid = DBConvert.ParseLong(row.Cells["ChangeDeadlineDetailPid"].Value);

        DataTable dtFOU = ((DataSet)ultDataFOU.DataSource).Tables[0];
        DataRow[] arrRows = dtFOU.Select(string.Format(@"Wo = {0} And ChangeDeadlineDetailPid <> {1} And ItemCode = '{2}'
            And Revision = {3} And CarcassCode = '{4}'", wo, changeDeadlineDetailPid, itemCode, revision, compCode));
        if (arrRows.Length > 0)
        {
          foreach (DataRow r in arrRows)
          {
            r[columnName] = selected;
            if (selected == 1)
            {
              r[otherColumnName] = otherSelected;
              r["SuggestDeadline"] = DBNull.Value;
            }
          }
        }
      }
    }

    private void btnExportExcelCOM2Deadline_Click(object sender, EventArgs e)
    {
      if (tabDepartment.SelectedTab.Name == "CST")
      {
        ControlUtility.ExportToExcelWithDefaultPath(ultData, "Deadline");
      }
      else if (tabDepartment.SelectedTab.Name == "CAR")
      {
        ControlUtility.ExportToExcelWithDefaultPath(ultDataCAR, "Deadline");
      }
      else if (tabDepartment.SelectedTab.Name == "SUB")
      {
        ControlUtility.ExportToExcelWithDefaultPath(ultDataSUB, "Deadline");
      }
      else if (tabDepartment.SelectedTab.Name == "FOU")
      {
        ControlUtility.ExportToExcelWithDefaultPath(ultDataFOU, "Deadline");
      }
      else
      {
        ControlUtility.ExportToExcelWithDefaultPath(ultDataFOU, "Deadline");
      }
    }

    private void btnExportFOU_Click(object sender, EventArgs e)
    {
      this.ExportInformation(ultInformationFOU);
    }

    private void btnExportSUB_Click(object sender, EventArgs e)
    {
      this.ExportInformation(ultInformationSUB);
    }

    private void btnExportCAR_Click(object sender, EventArgs e)
    {
      this.ExportInformation(ultInformationCAR);
    }

    private void btnExportCST_Click(object sender, EventArgs e)
    {
      this.ExportInformation(ultInformation);
    }
  }
}
