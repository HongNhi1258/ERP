/*
  Author      : 
  Date        : 06/11/2012
  Description : Master Plan For Container
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_004 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public string itemCode = string.Empty;
    public string container = string.Empty;
    private int flagMoveCont = 0;
    #endregion Field

    #region Init
    public viewPLN_98_004()
    {
      InitializeComponent();
      ultShipDateFrom.Value = DBNull.Value;
      ultShipDateTo.Value = DBNull.Value;
    }

    private void viewWHD_05_002_Load(object sender, EventArgs e)
    {
      this.txtContainer.Text = this.container;
      this.ultContainer.Text = this.txtContainer.Text;

      this.ultContainer_ValueChanged(null, null);

      // Load Status Control
      this.LoadStatusControl();

      // Load ItemCode
      this.LoadItemCode();

      // Load Customer
      this.LoadCustomer();

      // Load Customer
      this.LoadItemKind();

      // Load Note
      this.LoadNote();

      // Load Container
      this.LoadContainer();

      // Load MonthYear Loading Date
      this.LoadDropdownMonthYearLoadingDate();

      //Load Reason Moving
      this.LoadReasonMoving();

      //Load Reason Change Shipdate
      this.LoadReasonChangeShipdate();
    }

    private void LoadStatusControl()
    {
      if (this.btnPer1.Visible)
      {
        this.flagMoveCont = 1;
      }
      else
      {
        this.flagMoveCont = 0;
      }

      if (this.flagMoveCont == 0)
      {
        this.btnImport.Visible = false;
        this.btnBrownse.Visible = false;
        this.btnTemplate.Visible = false;
      }

      this.btnPer1.Visible = false;
    }

    private void LoadItemKind()
    {
      string commandText = "SELECT Code, [Value] FROM TblBOMCodeMaster WHERE [Group] = 4001 ORDER BY Sort";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dt != null)
      {
        ultDDItemGroup.DataSource = dt;
        ultDDItemGroup.DisplayMember = "Value";
        ultDDItemGroup.ValueMember = "Code";
        ultDDItemGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultDDItemGroup.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
        ultDDItemGroup.DisplayLayout.Bands[0].Columns["Value"].Width = 200;
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

    private DataTable CreateTableImport()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.String));
      taParent.Columns.Add("PONo", typeof(System.String));
      taParent.Columns.Add("Container", typeof(System.String));
      taParent.Columns.Add("MoveToContainer", typeof(System.String));
      taParent.Columns.Add("ReasonMove", typeof(System.String));
      taParent.Columns.Add("OldConfirmShipdate", typeof(System.String));
      taParent.Columns.Add("NewConfirmShipdate", typeof(System.String));
      taParent.Columns.Add("ReasonChangeShipdate", typeof(System.String));
      taParent.Columns.Add("Remarks", typeof(System.String));
      taParent.Columns.Add("ContainerRemark", typeof(System.String));
      return taParent;
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

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadItemCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT ItemCode ";
      commandText += " FROM TblBOMItemBasic ";
      commandText += " ORDER BY ItemCode ";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucItemCode.DataSource = dtItem;
      ucItemCode.ColumnWidths = "200";
      ucItemCode.DataBind();
      ucItemCode.ValueMember = "ItemCode";
    }

    /// <summary>
    /// Load Reason Moving
    /// </summary>
    private void LoadReasonMoving()
    {
      Utility.LoadUltraDropdownCodeMstDefault(ultDDReasonMoving, 16005);
    }

    /// <summary>
    /// Load Reason Change Shipdate
    /// </summary>
    private void LoadReasonChangeShipdate()
    {
      Utility.LoadUltraDropdownCodeMstDefault(ultDDReasonChange, 16074);
    }



    /// <summary>
    /// Load Customer
    /// </summary>
    private void LoadCustomer()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, CustomerCode + '-' + Name Name ";
      commandText += " FROM TblCSDCustomerInfo CSD ";
      commandText += " ORDER BY CustomerCode ";

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCustomer.DataSource = dtItem;
      ucCustomer.ColumnWidths = "0;200";
      ucCustomer.DataBind();
      ucCustomer.ValueMember = "Pid";
      ucCustomer.DisplayMember = "Name";
      ucCustomer.AutoSearchBy = "Name";
    }

    /// <summary>
    /// Load Note
    /// </summary>
    private void LoadNote()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 PID, 'Show all containers in 1 month' Name ";
      commandText += " UNION ";
      commandText += " SELECT 2 PID, 'Show all containers in 2 months' Name ";
      commandText += " UNION ";
      commandText += " SELECT 3 PID, 'Show all containers in 3 months' Name ";
      commandText += " UNION ";
      commandText += " SELECT 4 PID, 'Show all' Name ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultLoadingList.DataSource = dtSource;
      ultLoadingList.DisplayMember = "Name";
      ultLoadingList.ValueMember = "PID";
      ultLoadingList.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLoadingList.DisplayLayout.Bands[0].Columns["Name"].Width = 500;
      ultLoadingList.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;

      this.ultLoadingList.Value = 4;
    }

    /// <summary>
    /// Load Container
    /// </summary>
    private void LoadContainer()
    {
      string commandText = string.Empty;
      commandText += " SELECT CON.Pid ContainerPid, CON.ContainerNo ";
      commandText += " FROM TblPLNSHPContainer CON ";
      commandText += " WHERE Confirm <> 3 ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultContainer.DataSource = dtSource;
      ultContainer.DisplayMember = "ContainerNo";
      ultContainer.ValueMember = "ContainerPid";
      ultContainer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultContainer.DisplayLayout.Bands[0].Columns["ContainerNo"].Width = 300;
      ultContainer.DisplayLayout.Bands[0].Columns["ContainerPid"].Hidden = true;

      txtContainer.DataSource = dtSource;
      txtContainer.DisplayMember = "ContainerNo";
      txtContainer.ValueMember = "ContainerPid";
      txtContainer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      txtContainer.DisplayLayout.Bands[0].Columns["ContainerNo"].Width = 300;
      txtContainer.DisplayLayout.Bands[0].Columns["ContainerPid"].Hidden = true;
    }

    private void MerceDatatoGrid(DataTable dt)
    {

      DataTable dtSource = this.CreateTableImport();
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i][0].ToString().Length > 0)
        {
          DataRow dr = dtSource.NewRow();
          dr["ItemCode"] = dt.Rows[i][0].ToString();
          dr["Revision"] = dt.Rows[i][1].ToString();
          dr["Qty"] = dt.Rows[i][2].ToString();
          dr["PONo"] = dt.Rows[i][3].ToString();
          dr["Container"] = dt.Rows[i][4].ToString();
          dr["MoveToContainer"] = dt.Rows[i][5].ToString();
          dr["ReasonMove"] = dt.Rows[i][6].ToString();
          if (dt.Rows[i][7].ToString().Trim().Length > 0)
          {
            dr["OldConfirmShipdate"] = dt.Rows[i][7].ToString();
          }
          if (dt.Rows[i][8].ToString().Trim().Length > 0)
          {
            dr["NewConfirmShipdate"] = dt.Rows[i][8].ToString();
          }
          dr["ReasonChangeShipdate"] = dt.Rows[i][9].ToString();
          dr["Remarks"] = dt.Rows[i][10].ToString();
          dr["ContainerRemark"] = dt.Rows[i][11].ToString();
          dtSource.Rows.Add(dr);
        }
      }
      SqlDBParameter[] inputparam = new SqlDBParameter[1];
      inputparam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dtSource);
      DataTable dataSource = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanContainerMainData_Import", inputparam);
      viewPLN_98_024 view = new viewPLN_98_024();
      view.dtSource = dataSource;
      WindowUtinity.ShowView(view, "LIST OF MOVE CONTAINER", true, ViewState.MainWindow);

    }

    /// <summary>
    /// LoadDropdown Month Year LoadingDate
    /// </summary>
    /// <param name="udrpMaterials"></param>
    private void LoadDropdownMonthYearLoadingDate()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 PID, 'Show all containers in 1 month' Name ";
      commandText += " UNION ";
      commandText += " SELECT 2 PID, 'Show all containers in 2 months' Name ";
      commandText += " UNION ";
      commandText += " SELECT 3 PID, 'Show all containers in 3 months' Name ";
      commandText += " UNION ";
      commandText += " SELECT 4 PID, 'Show all' Name ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      udrpMonthYearLoadingDate.DataSource = dtSource;
      udrpMonthYearLoadingDate.ValueMember = "PID";
      udrpMonthYearLoadingDate.DisplayMember = "Name";
      udrpMonthYearLoadingDate.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpMonthYearLoadingDate.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;
      udrpMonthYearLoadingDate.DisplayLayout.Bands[0].Columns["Name"].Width = 500;
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[10];

      // ItemCode
      string text = string.Empty;
      text = this.txtItemCode.Text;
      if (text.Length > 0)
      {
        param[0] = new DBParameter("@ItemCode", DbType.String, text);
      }

      // Customer
      text = string.Empty;
      text = this.txtCustomer.Text;
      if (text.Length > 0)
      {
        param[1] = new DBParameter("@Customer", DbType.String, text);
      }

      // Container
      text = string.Empty;
      text = this.txtContainer.Text;
      if (text.Length > 0)
      {
        param[2] = new DBParameter("@Container", DbType.String, text);
      }

      // SaleCode
      text = string.Empty;
      text = this.txtSaleCode.Text;
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@SaleCode", DbType.String, text);
      }

      // Carcass Code
      text = this.txtCarcassCode.Text;
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@CarcassCode", DbType.String, text);
      }

      // Old Code
      text = this.txtOldCode.Text;
      if (text.Length > 0)
      {
        param[5] = new DBParameter("@OldCode", DbType.String, text);
      }
      DateTime shipdate;
      // ShipDate From
      if (this.ultShipDateFrom.Value != null)
      {
        shipdate = DBConvert.ParseDateTime(this.ultShipDateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        if (shipdate != null && shipdate != DateTime.MinValue)
        {
          param[6] = new DBParameter("@ShipDateFrom", DbType.DateTime, shipdate);
        }
      }

      // ShipDate To
      if (this.ultShipDateTo.Value != null)
      {
        shipdate = DBConvert.ParseDateTime(this.ultShipDateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
        if (shipdate != null && shipdate != DateTime.MinValue)
        {
          shipdate = (shipdate != DateTime.MaxValue) ? shipdate.AddDays(1) : shipdate;
          param[7] = new DBParameter("@ShipDateTo", DbType.DateTime, shipdate);
        }
      }

      //MonthYearLoadingDate
      int monthYearLoadingDate = DBConvert.ParseInt(this.ultLoadingList.Value.ToString());
      if (monthYearLoadingDate != int.MinValue)
      {
        param[8] = new DBParameter("@MonthYearLoadingDate", DbType.Int32, monthYearLoadingDate);
      }

      // WO
      text = this.txtWO.Text;
      if (text.Length > 0)
      {
        param[9] = new DBParameter("@WO", DbType.String, text);
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNMasterPlanContainerMainData_Select", 3000, param);

      DataTable dtColor = new DataTable();
      dtColor.Columns.Add("Color", typeof(System.Int32));

      if (dsSource != null)
      {
        this.ultData.DataSource = dsSource.Tables[0];

        for (int i = 0; i < this.ultData.Rows.Count; i++)
        {
          UltraGridRow rowGrid = this.ultData.Rows[i];
          int customerPid = DBConvert.ParseInt(rowGrid.Cells["CustomerPid"].Value.ToString());
          monthYearLoadingDate = DBConvert.ParseInt(rowGrid.Cells["MonthYearLoadingDate"].Value.ToString());
          DateTime finishDate = DBConvert.ParseDateTime(rowGrid.Cells["ShipDateCon"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
          string containerNo = rowGrid.Cells["ContainerNo"].Value.ToString();
          if (finishDate != DateTime.MinValue)
          {
            UltraDropDown ultMoveToCustomer = (UltraDropDown)rowGrid.Cells["MoveToContainer"].ValueList;
            string select = string.Empty;

            containerNo = containerNo.Replace("'", "''");
            if (monthYearLoadingDate == 1)
            {
              // 1
              DateTime addMonthOne = finishDate.AddMonths(2);
              DateTime misMonthOne = finishDate.AddMonths(-1);
              DateTime dateOneAdd = new DateTime(addMonthOne.Year, addMonthOne.Month, 1);
              DateTime dateOneMis = new DateTime(misMonthOne.Year, misMonthOne.Month, 1);

              select = "CustomerPid =" + customerPid;
              select += " AND ShipDate >= '" + dateOneMis + "'";
              select += " AND ShipDate < '" + dateOneAdd + "'";
              select += " AND ContainerNo <> '" + containerNo + "'";
            }
            else if (monthYearLoadingDate == 2)
            {
              // 2
              DateTime addMonthTwo = finishDate.AddMonths(3);
              DateTime misMonthTwo = finishDate.AddMonths(-2);
              DateTime dateTwoAdd = new DateTime(addMonthTwo.Year, addMonthTwo.Month, 1);
              DateTime dateTwoMis = new DateTime(misMonthTwo.Year, misMonthTwo.Month, 1);
              select = "CustomerPid =" + customerPid;
              select += " AND ShipDate >= '" + dateTwoMis + "'";
              select += " AND ShipDate < '" + dateTwoAdd + "'";
              select += " AND ContainerNo <> '" + containerNo + "'";
            }
            else if (monthYearLoadingDate == 3)
            {
              // 3
              DateTime addMonthThree = finishDate.AddMonths(4);
              DateTime misMonthThree = finishDate.AddMonths(-3);
              DateTime dateThreeAdd = new DateTime(addMonthThree.Year, addMonthThree.Month, 1);
              DateTime dateThreeMis = new DateTime(misMonthThree.Year, misMonthThree.Month, 1);
              select = "CustomerPid =" + customerPid;
              select += " AND ShipDate >= '" + dateThreeMis + "'";
              select += " AND ShipDate < '" + dateThreeAdd + "'";
              select += " AND ContainerNo <> '" + containerNo + "'";
            }
            else if (monthYearLoadingDate == 4)
            {
              select = "CustomerPid =" + customerPid
                    + " AND ContainerNo <> '" + containerNo + "'";
            }

            DataRow[] foundRow = dsSource.Tables[1].Select(select);
            DataTable dtContainer = this.UltraTable();
            for (int j = 0; j < foundRow.Length; j++)
            {
              DataRow rowAdd = dtContainer.NewRow();
              rowAdd["LoadingListPid"] = DBConvert.ParseLong(foundRow[j]["LoadingListPid"].ToString());
              rowAdd["CustomerCode"] = foundRow[j]["CustomerCode"].ToString();
              rowAdd["ContainerNo"] = foundRow[j]["ContainerNo"].ToString();
              rowAdd["ShipDateStr"] = foundRow[j]["ShipDateStr"].ToString();
              rowAdd["ShipDate"] = foundRow[j]["ShipDate"].ToString();
              rowAdd["CBM"] = DBConvert.ParseDouble(foundRow[j]["CBM"].ToString());

              dtContainer.Rows.Add(rowAdd);
            }

            if (ultMoveToCustomer == null)
            {
              if (dtContainer.Rows.Count > 0)
              {
                DataRow row = dtColor.NewRow();
                row["Color"] = i;
                dtColor.Rows.Add(row);
              }
              ultMoveToCustomer = new UltraDropDown();
              this.Controls.Add(ultMoveToCustomer);
            }

            DataView dtView = dtContainer.DefaultView;
            dtView.Sort = "ShipDate";
            dtContainer = dtView.ToTable();

            ultMoveToCustomer.DataSource = dtContainer;
            ultMoveToCustomer.ValueMember = "LoadingListPid";
            ultMoveToCustomer.DisplayMember = "ContainerNo";
            ultMoveToCustomer.DisplayLayout.Bands[0].Columns["LoadingListPid"].Hidden = true;
            ultMoveToCustomer.DisplayLayout.Bands[0].Columns["ShipDate"].Hidden = true;
            ultMoveToCustomer.DisplayLayout.Bands[0].Columns["ShipDateStr"].Header.Caption = "Ship Date";

            ultMoveToCustomer.Visible = false;

            rowGrid.Cells["MoveToContainer"].ValueList = ultMoveToCustomer;
          }
        }
      }
      else
      {
        ultData.DataSource = DBNull.Value;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int type = DBConvert.ParseInt(ultData.Rows[i].Cells["KindEarlyLate"].Value.ToString());

        if (type == 0)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.LightGreen;
        }
        else if (type == 1)
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }

        int moreThanCustomer = DBConvert.ParseInt(ultData.Rows[i].Cells["MoreThanCustomer"].Value.ToString());
        if (moreThanCustomer == 2)
        {
          ultData.Rows[i].Cells["No"].Appearance.FontData.Bold = DefaultableBoolean.True;
          ultData.Rows[i].Cells["No"].Appearance.ForeColor = Color.Red;
        }
        else if (moreThanCustomer == 3)
        {
          ultData.Rows[i].Cells["No"].Appearance.FontData.Bold = DefaultableBoolean.True;
          ultData.Rows[i].Cells["No"].Appearance.ForeColor = Color.Blue;
        }

        DataRow[] foundRowColor = dtColor.Select("Color =" + i);
        if (foundRowColor.Length > 0)
        {
          ultData.Rows[i].Cells["MoveToContainer"].Appearance.BackColor = Color.LightBlue;
          ultData.Rows[i].Cells["MoveQty"].Appearance.BackColor = Color.LightBlue;
        }
        else
        {
          ultData.Rows[i].Cells["MoveToContainer"].Appearance.BackColor = Color.White;
          ultData.Rows[i].Cells["MoveQty"].Appearance.BackColor = Color.White;
        }

        int affectToContDirect = DBConvert.ParseInt(ultData.Rows[i].Cells["AffectToContDirect"].Value.ToString());
        if (affectToContDirect == 1)
        {
          ultData.Rows[i].Cells["AffectToContDirect"].Appearance.ForeColor = Color.Red;
          ultData.Rows[i].Cells["AffectToContDirect"].Appearance.FontData.Bold = DefaultableBoolean.True;
        }

        int scheduleDeadlineLate = DBConvert.ParseInt(ultData.Rows[i].Cells["ScheduleDeadlineLate"].Value.ToString());
        if (scheduleDeadlineLate == 1)
        {
          ultData.Rows[i].Cells["ScheduleDeadlineLate"].Appearance.ForeColor = Color.Red;
          ultData.Rows[i].Cells["ScheduleDeadlineLate"].Appearance.FontData.Bold = DefaultableBoolean.True;
        }

        ultData.Rows[i].Cells["ContainerRemark"].Appearance.BackColor = Color.LightBlue;
        ultData.Rows[i].Cells["ItemKind"].Appearance.BackColor = Color.LightBlue;

        ultData.Rows[i].Cells["ReasonMove"].Appearance.BackColor = Color.LightBlue;
        ultData.Rows[i].Cells["OldConfirmShipdate"].Appearance.BackColor = Color.LightBlue;
        ultData.Rows[i].Cells["NewConfirmShipdate"].Appearance.BackColor = Color.LightBlue;
        ultData.Rows[i].Cells["ReasonChangeShipdate"].Appearance.BackColor = Color.LightBlue;
        ultData.Rows[i].Cells["Remarks"].Appearance.BackColor = Color.LightBlue;

        // To mau Lock
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["LockContainerListDetail"].Value) == 1)
        {
          ultData.Rows[i].Cells["LockContainerListDetail"].Appearance.BackColor = Color.Red;
          ultData.Rows[i].Cells["MoveToContainer"].Activation = Activation.ActivateOnly;
          ultData.Rows[i].Cells["ReasonMove"].Activation = Activation.ActivateOnly;
          ultData.Rows[i].Cells["OldConfirmShipdate"].Activation = Activation.ActivateOnly;
          ultData.Rows[i].Cells["NewConfirmShipdate"].Activation = Activation.ActivateOnly;
          ultData.Rows[i].Cells["ReasonChangeShipdate"].Activation = Activation.ActivateOnly;
          ultData.Rows[i].Cells["Remarks"].Activation = Activation.ActivateOnly;
        }
        // End
      }
      // Load Column Hide/ Unhide
      if (ultShowColumn.Rows.Count == 0)
      {
        this.LoadColumnName();
      }
      else
      {
        this.SetStatusColumn();
      }
    }

    /// <summary>
    /// Set Status Column When Search
    /// </summary>
    private void SetStatusColumn()
    {
      for (int colIndex = 1; colIndex < ultShowColumn.DisplayLayout.Bands[0].Columns.Count; colIndex++)
      {
        UltraGridCell cell = ultShowColumn.Rows[0].Cells[colIndex];
        if (cell.Activation != Activation.ActivateOnly && cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// Get Main Structure Datatable Row Container
    /// </summary>
    /// <returns></returns>
    private DataTable UltraTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("LoadingListPid", typeof(System.Int64));
      dt.Columns.Add("CustomerCode", typeof(System.String));
      dt.Columns.Add("ContainerNo", typeof(System.String));
      dt.Columns.Add("ShipDateStr", typeof(System.String));
      dt.Columns.Add("ShipDate", typeof(System.DateTime));
      dt.Columns.Add("CBM", typeof(System.Double));
      return dt;
    }

    /// <summary>
    /// Load Move To Customer
    /// </summary>
    /// <param name="itemCode"></param>
    /// <param name="ultRevision"></param>
    /// <returns></returns>
    private UltraDropDown LoadMoveToCustomer(int customerPid, DateTime finishDate,
            UltraDropDown ultMoveToCustomer, int monthYearLoadingDate, string containerNo)
    {
      if (ultMoveToCustomer == null)
      {
        ultMoveToCustomer = new UltraDropDown();
        this.Controls.Add(ultMoveToCustomer);
      }
      string commandText = string.Empty;
      commandText += " SELECT CL.Pid LoadingListPid, CUS.CustomerCode, CON.ContainerNo, ";
      commandText += "      CONVERT(VARCHAR, CON.ShipDate, 103) ShipDateStr, ROUND(SUM(CLD.Qty * ISNULL(IIF.CBM, 0)), 2) CBM ";
      commandText += " FROM TblPLNSHPContainer CON ";
      commandText += " 	  INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid ";
      commandText += " 	  INNER JOIN TblPLNContainerList CL ON COD.LoadingListPid = CL.Pid ";
      commandText += "		LEFT JOIN TblPLNContainerListDetail CLD ON CL.Pid = CLD.ContainerListPid ";
      commandText += "		LEFT JOIN TblBOMItemInfo IIF ON CLD.ItemCode = IIF.ItemCode ";
      commandText += " 										AND CLD.Revision = IIF.Revision";
      commandText += " 		LEFT JOIN TblCSDCustomerInfo CUS ON CL.CustomerPid = CUS.Pid";
      commandText += " WHERE CON.Confirm <> 3 ";
      commandText += "    AND CUS.Pid = " + customerPid;
      commandText += "    AND CON.ContainerNo != '" + containerNo + "'";

      if (monthYearLoadingDate == 1)
      {
        // 1
        DateTime addMonthOne = finishDate.AddMonths(2);
        DateTime misMonthOne = finishDate.AddMonths(-1);
        DateTime dateOneAdd = new DateTime(addMonthOne.Year, addMonthOne.Month, 1);
        DateTime dateOneMis = new DateTime(misMonthOne.Year, misMonthOne.Month, 1);
        commandText += " AND CON.ShipDate >= '" + dateOneMis + "'";
        commandText += " AND CON.ShipDate < '" + dateOneAdd + "'";
      }
      else if (monthYearLoadingDate == 2)
      {
        // 2
        DateTime addMonthTwo = finishDate.AddMonths(3);
        DateTime misMonthTwo = finishDate.AddMonths(-2);
        DateTime dateTwoAdd = new DateTime(addMonthTwo.Year, addMonthTwo.Month, 1);
        DateTime dateTwoMis = new DateTime(misMonthTwo.Year, misMonthTwo.Month, 1);
        commandText += " AND CON.ShipDate >= '" + dateTwoMis + "'";
        commandText += " AND CON.ShipDate < '" + dateTwoAdd + "'";
      }
      else if (monthYearLoadingDate == 3)
      {
        // 3
        DateTime addMonthThree = finishDate.AddMonths(4);
        DateTime misMonthThree = finishDate.AddMonths(-3);
        DateTime dateThreeAdd = new DateTime(addMonthThree.Year, addMonthThree.Month, 1);
        DateTime dateThreeMis = new DateTime(misMonthThree.Year, misMonthThree.Month, 1);
        commandText += " AND CON.ShipDate >= '" + dateThreeMis + "'";
        commandText += " AND CON.ShipDate < '" + dateThreeAdd + "'";
      }
      commandText += " GROUP BY CL.Pid, CUS.CustomerCode, CON.ContainerNo, CON.ShipDate ";
      commandText += " ORDER BY CON.ShipDate";

      DataTable dt = Shared.DataBaseUtility.DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultMoveToCustomer.DataSource = dt;
      ultMoveToCustomer.ValueMember = "LoadingListPid";
      ultMoveToCustomer.DisplayMember = "ContainerNo";
      ultMoveToCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultMoveToCustomer.DisplayLayout.Bands[0].Columns["LoadingListPid"].Hidden = true;
      ultMoveToCustomer.DisplayLayout.Bands[0].Columns["ShipDateStr"].Header.Caption = "Ship Date";
      ultMoveToCustomer.Visible = false;
      return ultMoveToCustomer;
    }


    /// <summary>
    /// Load Column Name
    /// </summary>
    private void LoadColumnName()
    {
      DataTable dtNew = new DataTable();
      DataTable dtColumn = (DataTable)ultData.DataSource;
      dtNew.Columns.Add("All", typeof(Int32));
      dtNew.Columns["All"].DefaultValue = 0;
      foreach (DataColumn column in dtColumn.Columns)
      {
        dtNew.Columns.Add(column.ColumnName, typeof(Int32));
        dtNew.Columns[column.ColumnName].DefaultValue = 0;

        if (string.Compare(column.ColumnName, "ContainerNo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LoadingQty", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "WO", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "WipQty", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "WipStatus", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "WipItem", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        //if (string.Compare(column.ColumnName, "MCHDeadline", true) == 0)
        //{
        //  dtNew.Columns[column.ColumnName].DefaultValue = 1;
        //}

        //if (string.Compare(column.ColumnName, "SUBDeadline", true) == 0)
        //{
        //  dtNew.Columns[column.ColumnName].DefaultValue = 1;
        //}

        //if (string.Compare(column.ColumnName, "FOUDeadline", true) == 0)
        //{
        //  dtNew.Columns[column.ColumnName].DefaultValue = 1;
        //}

        //if (string.Compare(column.ColumnName, "Priority", true) == 0)
        //{
        //  dtNew.Columns[column.ColumnName].DefaultValue = 1;
        //}

        //if (string.Compare(column.ColumnName, "FinishDateStr", true) == 0)
        //{
        //  dtNew.Columns[column.ColumnName].DefaultValue = 1;
        //}

        //if (string.Compare(column.ColumnName, "Point", true) == 0)
        //{
        //  dtNew.Columns[column.ColumnName].DefaultValue = 1;
        //}

        //if (string.Compare(column.ColumnName, "MonthYearLoadingDate", true) == 0)
        //{
        //  dtNew.Columns[column.ColumnName].DefaultValue = 1;
        //}

        //if (string.Compare(column.ColumnName, "MoveQty", true) == 0)
        //{
        //  dtNew.Columns[column.ColumnName].DefaultValue = 1;
        //}

        //if (string.Compare(column.ColumnName, "MoveToContainer", true) == 0)
        //{
        //  dtNew.Columns[column.ColumnName].DefaultValue = 1;
        //}
      }
      DataRow row = dtNew.NewRow();
      dtNew.Rows.Add(row);
      ultShowColumn.DataSource = dtNew;
      ultShowColumn.Rows[0].Appearance.BackColor = Color.LightBlue;
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
        Utility.ExportToExcelWithDefaultPath(ultData, "MASTER PLAN FOR CONTAINER");

        //Excel.Workbook xlBook;
        //UltraGrid AA = ultData;
        //AA.Rows.Band.Columns["MonthYearLoadingDate"].Hidden = true;
        //AA.Rows.Band.Columns["MoveQty"].Hidden = true;
        //Utility.ExportToExcelWithDefaultPath(AA, out xlBook, "Master Plan For Container", 7);

        //AA.Rows.Band.Columns["MonthYearLoadingDate"].Hidden = false;
        //AA.Rows.Band.Columns["MoveQty"].Hidden = false;
        //string filePath = xlBook.FullName;
        //Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        //xlSheet.Cells[1, 1] = "VFR Co., LTD";
        //Excel.Range r = xlSheet.get_Range("A1", "A1");

        //xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        //xlSheet.Cells[3, 1] = "MASTER PLAN FOR CONTAINER";
        //r = xlSheet.get_Range("A3", "A3");
        //r.Font.Bold = true;
        //r.Font.Size = 14;
        //r.EntireRow.RowHeight = 20;

        //xlSheet.Cells[4, 1] = "Date: ";
        //r.Font.Bold = true;
        //xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlSheet.Cells[5, 1] = "Note: ";
        //r.Font.Bold = true;

        //xlSheet.Cells[5, 2] = "On Time";

        //xlSheet.Cells[5, 3] = "Late";
        //r = xlSheet.get_Range("C5", "C5");
        //r.Interior.Color = (object)ColorTranslator.ToOle(Color.FromArgb(255, 255, 0));

        //xlSheet.Cells[5, 4] = "Early ";
        //r = xlSheet.get_Range("D5", "D5");
        //r.Interior.Color = (object)ColorTranslator.ToOle(Color.FromArgb(144, 238, 144));

        //xlBook.Application.DisplayAlerts = false;
        //xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        //Process.Start(filePath);
      }
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
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }
    private void ImportExcel()
    {
      // Check invalid file
      if (!File.Exists(txtImportExcelFile.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }
      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), "SELECT * FROM [List (1)$E3:E4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0].ToString());
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }

      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtImportExcelFile.Text.Trim(), string.Format("SELECT * FROM [List (1)$B5:M{0}]", itemCount + 5));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.MerceDatatoGrid(dsItemList.Tables[0]);
    }

    private bool CheckValidSaveHistory()
    {
      bool result = true;
      DataTable dtSource = (DataTable)this.ultData.DataSource;
      for (int j = 0; j < ultData.Rows.Count; j++)
      {
        ultData.Rows[j].Cells["Status"].Value = "";
      }

      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        if (DBConvert.ParseLong(row["MoveToContainer"].ToString()) != long.MinValue)
        {
          //Check ReasonMove
          if (row["ReasonMove"].ToString().Length == 0)
          {
            this.ultData.Rows[i].Cells["Status"].Value = "Reason Moving Is Empty";
            this.ultData.Rows[i].Appearance.BackColor = Color.Yellow;
            result = false;
            continue;
          }
          else
          {
            string cm = string.Format(@"select value from TblBOMCodeMaster where [group] = 16005 and code = {0} and DeleteFlag = 0 ", row["ReasonMove"].ToString());
            DataTable dtTest = DataBaseAccess.SearchCommandTextDataTable(cm);
            if (dtTest.Rows.Count == 0)
            {
              this.ultData.Rows[i].Cells["Status"].Value = "Reason Moving Is Valid";
              this.ultData.Rows[i].Appearance.BackColor = Color.Yellow;
              result = false;
              continue;
            }
          }
          //Check OldConfirmShipdate,NewConfirmShipDate,Reason Change Shipdate, QtyMoving.
          string storeName = "spPLNContainerChecking";
          DBParameter[] inputParam = new DBParameter[7];
          if (row["OldConfirmShipdate"].ToString().Length != 0)
          {
            if (DBConvert.ParseDateTime(row["OldConfirmShipdate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
            {
              inputParam[0] = new DBParameter("@OldDate", DbType.DateTime, DBConvert.ParseDateTime(row["OldConfirmShipdate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            }
            else
            {
              this.ultData.Rows[i].Cells["Status"].Value = "Old Confirm ShipDate is Valid";
              this.ultData.Rows[i].Appearance.BackColor = Color.Yellow;
              result = false;
              continue;
            }
          }
          if (row["NewConfirmShipdate"].ToString().Length != 0)
          {
            if (DBConvert.ParseDateTime(row["NewConfirmShipdate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
            {
              inputParam[1] = new DBParameter("@NewDate", DbType.DateTime, DBConvert.ParseDateTime(row["NewConfirmShipdate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
            }
            else
            {
              this.ultData.Rows[i].Cells["Status"].Value = "New Confirm ShipDate Is Valid";
              this.ultData.Rows[i].Appearance.BackColor = Color.Yellow;
              result = false;
              continue;
            }
          }
          if (DBConvert.ParseInt(row["ReasonChangeShipdate"].ToString()) > 0)
          {
            inputParam[2] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(row["ReasonChangeShipdate"].ToString()));
          }
          if (DBConvert.ParseInt(row["MoveQty"].ToString()) > 0)
          {
            inputParam[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row["MoveQty"].ToString()));
          }
          if (row["ItemCode"].ToString().Length > 0)
          {
            inputParam[4] = new DBParameter("@ItemCode", DbType.String, 32, row["ItemCode"].ToString());
          }
          if (DBConvert.ParseInt(row["Revision"].ToString()) > 0)
          {
            inputParam[5] = new DBParameter("@Revision", DbType.Int32, row["Revision"].ToString());
          }
          if (DBConvert.ParseLong(row["SalePid"].ToString()) > 0)
          {
            inputParam[6] = new DBParameter("@SaleOrderPid", DbType.Int64, row["SalePid"].ToString());
          }
          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);

          if (outputParam[0].Value == null || DBConvert.ParseLong(outputParam[0].Value) < 0)
          {
            if (DBConvert.ParseLong(outputParam[0].Value) == -1)
            {
              this.ultData.Rows[i].Cells["Status"].Value = "Old Shipdate invald Or Reason Change Shipdate is requirement";
              this.ultData.Rows[i].Cells["Status"].Appearance.BackColor = Color.Yellow;
              result = false;
              continue;
            }
            if (DBConvert.ParseLong(outputParam[0].Value) == -2)
            {
              this.ultData.Rows[i].Cells["Status"].Value = "Reason Change Shipdate is Valid";
              this.ultData.Rows[i].Appearance.BackColor = Color.Yellow;
              result = false;
              continue;
            }
            if (DBConvert.ParseLong(outputParam[0].Value) == -3)
            {
              this.ultData.Rows[i].Cells["Status"].Value = "Old Shipdate Is valid Or SaleOrder is pending";
              this.ultData.Rows[i].Cells["Status"].Appearance.BackColor = Color.Yellow;
              result = false;
              continue;
            }
            if (DBConvert.ParseLong(outputParam[0].Value) == -10)
            {
              this.ultData.Rows[i].Cells["Status"].Value = "Data Valid";
              this.ultData.Rows[i].Cells["Status"].Appearance.BackColor = Color.Yellow;
              result = false;
              continue;
            }
          }
        }
      }
      return result;
    }
    public DataTable ContainerMovingInformation()
    {
      DataTable taParent = new DataTable();
      taParent.Columns.Add("SaleOrderPid", typeof(System.Int64));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("LoadingFromPid", typeof(System.Int64));
      taParent.Columns.Add("LoadingToPid", typeof(System.Int64));
      taParent.Columns.Add("MovingQty", typeof(System.String));
      taParent.Columns.Add("MovingReason", typeof(System.Int32));
      taParent.Columns.Add("OldConfirmShipdate", typeof(System.DateTime));
      taParent.Columns.Add("NewConfirmShipdate", typeof(System.DateTime));
      taParent.Columns.Add("ReasonChangeShipDate", typeof(System.Int32));
      taParent.Columns.Add("Remarks", typeof(System.String));

      return taParent;
    }

    #endregion Function

    #region Event
    /// <summary>
    /// Init Layout 
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

      e.Layout.Bands[0].Columns["No"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["CustomerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["FinishDate"].Hidden = true;
      e.Layout.Bands[0].Columns["KindEarlyLate"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerListDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["MoreThanCustomer"].Hidden = true;
      e.Layout.Bands[0].Columns["ShipDateCon"].Hidden = true;

      e.Layout.Bands[0].Columns["SaleCode"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["OldCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["Unit"].Hidden = true;
      e.Layout.Bands[0].Columns["UCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerName"].Hidden = true;
      e.Layout.Bands[0].Columns["Direct"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Hidden = true;
      e.Layout.Bands[0].Columns["PONo"].Hidden = true;
      e.Layout.Bands[0].Columns["PODate"].Hidden = true;
      e.Layout.Bands[0].Columns["SpecialRemark"].Hidden = true;
      e.Layout.Bands[0].Columns["PackingNote"].Hidden = true;
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].Hidden = true;
      e.Layout.Bands[0].Columns["SubCon"].Hidden = true;
      e.Layout.Bands[0].Columns["RevisionConNo"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemGroup"].Hidden = true;
      e.Layout.Bands[0].Columns["RepairQty"].Hidden = true;
      e.Layout.Bands[0].Columns["RepairCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["FGWRec.Date"].Hidden = true;
      e.Layout.Bands[0].Columns["UrgentNote"].Hidden = true;
      e.Layout.Bands[0].Columns["Remark & Delivery Requirement"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalLoadingQtyMonth"].Hidden = true;
      e.Layout.Bands[0].Columns["Point"].Hidden = true;
      e.Layout.Bands[0].Columns["MoveQty"].Hidden = true;
      e.Layout.Bands[0].Columns["MonthYearLoadingDate"].Hidden = true;
      e.Layout.Bands[0].Columns["MoveToContainer"].Hidden = true;
      e.Layout.Bands[0].Columns["LoadingDate"].Hidden = true;
      e.Layout.Bands[0].Columns["LoadingCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["FGWQty"].Hidden = true;
      e.Layout.Bands[0].Columns["MCHDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["SUBDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["FOUDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["Priority"].Hidden = true;
      e.Layout.Bands[0].Columns["FinishDateStr"].Hidden = true;
      e.Layout.Bands[0].Columns["Item/Box"].Hidden = true;
      e.Layout.Bands[0].Columns["PackingQty"].Hidden = true;
      e.Layout.Bands[0].Columns["PackingCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["AffectToContDirect"].Hidden = true;
      e.Layout.Bands[0].Columns["ScheduleDeadlineLate"].Hidden = true;
      e.Layout.Bands[0].Columns["USStock"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerRemark"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemKind"].Hidden = true;
      e.Layout.Bands[0].Columns["ReturnGoods"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalBalance"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalWIP"].Hidden = true;
      e.Layout.Bands[0].Columns["TotalUnrelease"].Hidden = true;

      e.Layout.Bands[0].Columns["ReasonMove"].Hidden = true;
      e.Layout.Bands[0].Columns["OldConfirmShipdate"].Hidden = true;
      e.Layout.Bands[0].Columns["NewConfirmShipdate"].Hidden = true;
      e.Layout.Bands[0].Columns["ReasonChangeShipdate"].Hidden = true;
      e.Layout.Bands[0].Columns["Remarks"].Hidden = true;

      e.Layout.Bands[0].Columns["SalePid"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerListPid"].Hidden = true;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (e.Layout.Bands[0].Columns[i].DataType == typeof(DateTime))
        {
          e.Layout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["LockContainerListDetail"].Header.Caption = "Lock";
      e.Layout.Bands[0].Columns["AffectToContDirect"].Header.Caption = "Affect To\n Cont Direct";
      e.Layout.Bands[0].Columns["ScheduleDeadlineLate"].Header.Caption = "Schedule\n Deadline Late";
      e.Layout.Bands[0].Columns["ReturnGoods"].Header.Caption = "Return\nGoods";

      e.Layout.Bands[0].Columns["ReasonMove"].Header.Caption = "Reason Move\n Loading List";
      e.Layout.Bands[0].Columns["OldConfirmShipdate"].Header.Caption = "Old Estimated\n Shipdate";
      e.Layout.Bands[0].Columns["NewConfirmShipdate"].Header.Caption = "New Estimated\n Shipdate";
      e.Layout.Bands[0].Columns["ReasonChangeShipdate"].Header.Caption = "Reason Change\n Confirmed Shipdate";
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].Header.Caption = "Estimated\n Shipdate";

      e.Layout.Bands[0].Columns["ContainerRemark"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["ItemKind"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["ReasonMove"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["OldConfirmShipdate"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["NewConfirmShipdate"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["ReasonChangeShipdate"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["FinishDateStr"].Header.Caption = "FinishDate";
      e.Layout.Bands[0].Columns["Revision"].Header.Caption = "Rev.";
      e.Layout.Bands[0].Columns["No"].MinWidth = 20;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 20;
      e.Layout.Bands[0].Columns["SaleCode"].MinWidth = 70;
      e.Layout.Bands[0].Columns["SaleCode"].MaxWidth = 70;

      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 75;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 75;

      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 70;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 70;

      e.Layout.Bands[0].Columns["Revision"].MinWidth = 20;
      e.Layout.Bands[0].Columns["Revision"].MaxWidth = 20;

      e.Layout.Bands[0].Columns["OldCode"].MinWidth = 75;
      e.Layout.Bands[0].Columns["OldCode"].MaxWidth = 75;

      e.Layout.Bands[0].Columns["UCBM"].MinWidth = 30;
      e.Layout.Bands[0].Columns["UCBM"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["PODate"].MinWidth = 80;
      e.Layout.Bands[0].Columns["PODate"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["SaleNo"].MinWidth = 110;
      e.Layout.Bands[0].Columns["SaleNo"].MaxWidth = 110;

      e.Layout.Bands[0].Columns["ConfirmedShipDate"].MinWidth = 80;
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["RevisionConNo"].MinWidth = 30;
      e.Layout.Bands[0].Columns["RevisionConNo"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["LoadingDate"].MinWidth = 80;
      e.Layout.Bands[0].Columns["LoadingDate"].MaxWidth = 80;

      e.Layout.Bands[0].Columns["LoadingQty"].MinWidth = 30;
      e.Layout.Bands[0].Columns["LoadingQty"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["LoadingCBM"].MinWidth = 30;
      e.Layout.Bands[0].Columns["LoadingCBM"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["PackingQty"].MinWidth = 30;
      e.Layout.Bands[0].Columns["PackingQty"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["PackingCBM"].MinWidth = 30;
      e.Layout.Bands[0].Columns["PackingCBM"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["RepairQty"].MinWidth = 30;
      e.Layout.Bands[0].Columns["RepairQty"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["RepairCBM"].MinWidth = 30;
      e.Layout.Bands[0].Columns["RepairCBM"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["Priority"].MinWidth = 30;
      e.Layout.Bands[0].Columns["Priority"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["FinishDateStr"].MinWidth = 30;
      e.Layout.Bands[0].Columns["FinishDateStr"].MaxWidth = 30;

      e.Layout.Bands[0].Columns["MonthYearLoadingDate"].ValueList = this.udrpMonthYearLoadingDate;

      e.Layout.Bands[0].Columns["ReasonMove"].ValueList = this.ultDDReasonMoving;
      e.Layout.Bands[0].Columns["ReasonChangeShipdate"].ValueList = this.ultDDReasonChange;

      e.Layout.Bands[0].Columns["ItemKind"].ValueList = this.ultDDItemGroup;
      e.Layout.Bands[0].Columns["LockContainerListDetail"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Override.AllowUpdate = DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 14; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Status"].CellActivation = Activation.ActivateOnly;

      if (this.flagMoveCont == 0)
      {
        e.Layout.Bands[0].Columns["LockContainerListDetail"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["MoveToContainer"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["ReasonMove"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["OldConfirmShipdate"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["NewConfirmShipdate"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Remarks"].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Search 
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
    /// Close
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      // Update Remark Container
      DataTable dtSource = (DataTable)this.ultData.DataSource;
      foreach (DataRow row in dtSource.Rows)
      {
        if (row.RowState == DataRowState.Modified && row.RowState != DataRowState.Deleted)
        {
          DBParameter[] inputParamRemark = new DBParameter[4];
          inputParamRemark[0] = new DBParameter("@ContainerListDetailPid", DbType.Int64, DBConvert.ParseLong(row["ContainerListDetailPid"].ToString()));
          inputParamRemark[1] = new DBParameter("@ContainerRemark", DbType.String, 512, row["ContainerRemark"].ToString());
          inputParamRemark[2] = new DBParameter("@ItemKind", DbType.Int32, DBConvert.ParseInt(row["ItemKind"].ToString()));
          if (DBConvert.ParseInt(row["LockContainerListDetail"]) == 1)
          {
            inputParamRemark[3] = new DBParameter("@LockContainerListDetail", DbType.Int32, 1);
          }
          else
          {
            inputParamRemark[3] = new DBParameter("@LockContainerListDetail", DbType.Int32, 0);
          }
          DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerMainDataContainerRemark_Update", inputParamRemark);
        }
      }
      // Update History
      if (CheckValidSaveHistory())
      {
        DataTable dt = this.ContainerMovingInformation();
        foreach (DataRow row in dtSource.Rows)
        {
          if (DBConvert.ParseLong(row["MoveToContainer"].ToString()) > 0)
          {
            DataRow dr = dt.NewRow();

            dr["SaleOrderPid"] = DBConvert.ParseLong(row["SalePid"].ToString());
            dr["ItemCode"] = row["ItemCode"].ToString();
            dr["Revision"] = DBConvert.ParseInt(row["Revision"].ToString());
            dr["LoadingFromPid"] = DBConvert.ParseLong(row["ContainerListPid"].ToString());
            dr["LoadingToPid"] = DBConvert.ParseLong(row["MoveToContainer"].ToString());

            dr["MovingQty"] = DBConvert.ParseInt(row["MoveQty"].ToString());
            dr["MovingReason"] = DBConvert.ParseInt(row["ReasonMove"].ToString());
            if (row["OldConfirmShipdate"].ToString().Length > 0)
            {
              dr["OldConfirmShipdate"] = DBConvert.ParseDateTime(row["OldConfirmShipdate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["NewConfirmShipdate"].ToString().Length > 0)
            {
              dr["NewConfirmShipdate"] = DBConvert.ParseDateTime(row["NewConfirmShipdate"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
            }
            if (row["OldConfirmShipdate"].ToString().Length > 0 || row["NewConfirmShipdate"].ToString().Length > 0)
            {
              dr["ReasonChangeShipDate"] = DBConvert.ParseInt(row["ReasonChangeShipDate"].ToString());
            }
            dr["Remarks"] = row["Remarks"].ToString();
            dt.Rows.Add(dr);
          }
        }
        if (dt.Rows.Count > 0)
        {
          SqlDBParameter[] inputParam = new SqlDBParameter[2];
          inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dt);
          inputParam[1] = new SqlDBParameter("@CurrentPid", SqlDbType.BigInt, SharedObject.UserInfo.UserPid);
          SqlDBParameter[] outputParam = new SqlDBParameter[1];
          outputParam[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);
          SqlDataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanHistory_Insert", 900, inputParam, outputParam);
          if (outputParam[0].Value == null || DBConvert.ParseLong(outputParam[0].Value.ToString()) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Save History");
            return;
          }
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", "Data");
        return;
      }

      DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerPriorityRefeshData_Insert");
      WindowUtinity.ShowMessageSuccess("MSG0004");

      this.Search();
    }

    private void ucItemCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtItemCode.Text = this.ucItemCode.SelectedValue;
    }

    private void chkItemCode_CheckedChanged(object sender, EventArgs e)
    {
      ucItemCode.Visible = chkItemCode.Checked;
    }

    private void chkCustomer_CheckedChanged(object sender, EventArgs e)
    {
      ucCustomer.Visible = chkCustomer.Checked;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      string commandText = string.Empty;
      switch (columnName)
      {
        case "monthyearloadingdate":
          int customerPid = DBConvert.ParseInt(e.Cell.Row.Cells["CustomerPid"].Value.ToString().Trim());
          DateTime finishDate = DBConvert.ParseDateTime(e.Cell.Row.Cells["ShipDateCon"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
          int monthYearLoadingDate = DBConvert.ParseInt(e.Cell.Row.Cells["MonthYearLoadingDate"].Value.ToString().Trim());
          string containerNo = e.Cell.Row.Cells["ContainerNo"].Value.ToString().Trim();
          UltraDropDown ultMoveToCustomer = (UltraDropDown)e.Cell.Row.Cells["MoveToContainer"].ValueList;
          if (finishDate != DateTime.MinValue)
          {
            e.Cell.Row.Cells["MoveToContainer"].ValueList = this.LoadMoveToCustomer(customerPid, finishDate,
                                        ultMoveToCustomer, monthYearLoadingDate, containerNo);
          }
          break;
        case "reasonmove":
          if (e.Cell.Value.ToString().Length > 0)
          {
            string cm = string.Format(@"select value from TblBOMCodeMaster where [group] = 16005 and code = {0} and DeleteFlag = 0 ", value);
            DataTable dtTest = DataBaseAccess.SearchCommandTextDataTable(cm);
            if (dtTest.Rows.Count == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Reason Moving");
            }
          }
          break;
      }
    }

    private void ucCustomer_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtCustomer.Text = this.ucCustomer.SelectedValue;
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0 && ultData.Selected.Rows.Count > 0)
      {
        string itemCode = ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString().Trim();
        int revision = DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["Revision"].Value.ToString().Trim());
        if (itemCode.Length > 0)
        {
          viewPLN_98_005 uc = new viewPLN_98_005();
          uc.itemCode = itemCode;
          uc.revision = revision;
          Shared.Utility.WindowUtinity.ShowView(uc, "CONTAINER DETAIL", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Normal);
        }
      }
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void btnChangeLoadingDate_Click(object sender, EventArgs e)
    {
      viewPLN_98_008 uc = new viewPLN_98_008();
      Shared.Utility.WindowUtinity.ShowView(uc, "CHANGE CONTAINER SHIP DATE", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Normal);
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
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
      e.Layout.Bands[0].Columns["CustomerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["FinishDate"].Hidden = true;
      e.Layout.Bands[0].Columns["KindEarlyLate"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerListDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerListPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SalePid"].Hidden = true;
      e.Layout.Bands[0].Columns["MoreThanCustomer"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ShipDateCon"].Hidden = true;

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["LockContainerListDetail"].Header.Caption = "Lock";
      e.Layout.Bands[0].Columns["AffectToContDirect"].Header.Caption = "Affect To\n Cont Direct";
      e.Layout.Bands[0].Columns["ScheduleDeadlineLate"].Header.Caption = "Schedule\n Deadline Late";

      e.Layout.Bands[0].Columns["FinishDateStr"].Header.Caption = "FinishDate";
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    private void ultShowColumn_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      UltraGridRow row = e.Cell.Row;
      if (!columnName.Equals("ALL", StringComparison.OrdinalIgnoreCase))
      {
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = e.Cell.Text.Equals("0");
        }
        if (e.Cell.Activation != Activation.ActivateOnly && e.Cell.Column.Hidden == false && e.Cell.Text == string.Empty)
        {
          ultData.DisplayLayout.Bands[0].Columns[columnName].Hidden = true;
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

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      switch (columnName)
      {
        case "MoveQty":
          int qty = DBConvert.ParseInt(e.Cell.Text);
          int loadingQty = DBConvert.ParseInt(e.Cell.Row.Cells["LoadingQty"].Value.ToString());
          if (e.Cell.Text.Trim().Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Move Qty");
            e.Cancel = true;
            break;
          }

          if (loadingQty < qty)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0010", "Move Qty", "Qty");
            e.Cancel = true;
          }

          if (qty <= 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Move Qty");
            e.Cancel = true;
            break;
          }
          break;
        case "ReasonMove":

          if (e.Cell.Row.Cells["MoveToContainer"].Value.ToString().Length != 0 && e.Cell.Text.ToString().Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Reason Moving");
            e.Cancel = true;
          }
          break;
        default:
        case "ItemKind":
          {
            if (e.Cell.Row.Cells["ItemKind"].Text.Length == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "ItemKind");
              e.Cancel = true;
            }
          }

          break;

      }
    }

    private void txtContainer_Leave(object sender, EventArgs e)
    {
      this.ultContainer.Text = this.txtContainer.Text;
    }

    private void ultContainer_ValueChanged(object sender, EventArgs e)
    {
      if (this.ultContainer.Text.Length > 0)
      {

        string commandText = string.Empty;
        commandText += " SELECT SUM(CLD.Qty) Qty, ROUND(SUM(IIF.CBM * CLD.Qty), 3) CBM ";
        commandText += " FROM TblPLNSHPContainer CON ";
        commandText += " 	INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid ";
        commandText += " 	INNER JOIN TblPLNContainerListDetail CLD ON COD.LoadingListPid = CLD.ContainerListPid ";
        commandText += " 	INNER JOIN TblBOMItemInfo IIF ON CLD.ItemCode = IIF.ItemCode ";
        commandText += " 									AND CLD.Revision = IIF.Revision ";
        commandText += " WHERE CON.ContainerNo ='" + this.ultContainer.Text + "'";

        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dtSource != null && dtSource.Rows.Count > 0)
        {
          this.lblContainer.Text = dtSource.Rows[0]["Qty"].ToString() + " " + dtSource.Rows[0]["CBM"].ToString();
        }
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtContainer.Value = DBNull.Value;
      ultContainer.Value = DBNull.Value;
      txtSaleCode.Text = string.Empty;
      txtCarcassCode.Text = string.Empty;
      txtOldCode.Text = string.Empty;
      txtWO.Text = string.Empty;
      ultShipDateFrom.Value = DBNull.Value;
      ultShipDateTo.Value = DBNull.Value;
      ultLoadingList.Value = DBNull.Value;
      txtItemCode.Text = string.Empty;
      txtCustomer.Text = string.Empty;
      this.LoadItemCode();
    }

    private void btnWIPWorkArea_Click(object sender, EventArgs e)
    {
      viewPLN_98_019 uc = new viewPLN_98_019();
      Shared.Utility.WindowUtinity.ShowView(uc, "WIP WORKAREA ON CONTAINER", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Normal);
    }

    private void btnTemplate_Click(object sender, EventArgs e)
    {
      string templateName = string.Empty;
      templateName = "PLA_98_004_01";
      string sheetName = "List";
      string outFileName = "Template Import Container";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNReasonForMoveContainerLoading");

      for (int i = 0; i < dt.Rows.Count; i++)
      {
        DataRow dtRow = dt.Rows[i];
        if (i > 0)
        {
          oXlsReport.Cell("O7:S7").Copy();
          oXlsReport.RowInsert(10 + i);
          oXlsReport.Cell("O7:S7", 0, i).Paste();
        }
        //if (dtRow["CodeMoveContainer"].ToString().Length > 0)
        //{
        oXlsReport.Cell("**1", 0, i).Value = dtRow["CodeMoveContainer"];
        //}
        //if (dtRow["TextMoveContainer"].ToString().Length > 0)
        //{
        oXlsReport.Cell("**2", 0, i).Value = dtRow["TextMoveContainer"];
        //}
        //if (dtRow["CodeChangeShipDate"].ToString().Length > 0)
        //{
        oXlsReport.Cell("**3", 0, i).Value = dtRow["CodeChangeShipDate"];
        //}
        //if (dtRow["TextChangeShipDate"].ToString().Length > 0)
        //{
        oXlsReport.Cell("**4", 0, i).Value = dtRow["TextChangeShipDate"];
        //}
      }
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      btnImport.Enabled = false;
      this.ImportExcel();
      btnImport.Enabled = true;
    }
    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a excel file";
      txtImportExcelFile.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }
    #endregion Event

    private void ultData_AfterRowUpdate(object sender, RowEventArgs e)
    {
      if (e.Row.Appearance.BorderColor == Color.Yellow)
      {
        int type = DBConvert.ParseInt(e.Row.Cells["KindEarlyLate"].Value.ToString());

        if (type == 0)
        {
          e.Row.CellAppearance.BackColor = Color.LightGreen;
        }
        else if (type == 1)
        {
          e.Row.CellAppearance.BackColor = Color.Yellow;
        }

        int moreThanCustomer = DBConvert.ParseInt(e.Row.Cells["MoreThanCustomer"].Value.ToString());
        if (moreThanCustomer == 2)
        {
          e.Row.Cells["No"].Appearance.FontData.Bold = DefaultableBoolean.True;
          e.Row.Cells["No"].Appearance.ForeColor = Color.Red;
        }
        else if (moreThanCustomer == 3)
        {
          e.Row.Cells["No"].Appearance.FontData.Bold = DefaultableBoolean.True;
          e.Row.Cells["No"].Appearance.ForeColor = Color.Blue;
        }
        //DataTable dtColor = new DataTable();
        //dtColor.Columns.Add("Color", typeof(System.Int32));

        //DataTable dtContainer = this.UltraTable();
        //if (ultMoveToCustomer == null)
        //{
        //  if (dtContainer.Rows.Count > 0)
        //  {
        //    DataRow row = dtColor.NewRow();
        //    row["Color"] = i;
        //    dtColor.Rows.Add(row);
        //  }
        //  ultMoveToCustomer = new UltraDropDown();
        //  this.Controls.Add(ultMoveToCustomer);
        //}
        //DataRow[] foundRowColor = dtColor.Select("Color =" + i);
        //if (foundRowColor.Length > 0)
        //{
        //  e.Row.Cells["MoveToContainer"].Appearance.BackColor = Color.LightBlue;
        //  e.Row.Cells["MoveQty"].Appearance.BackColor = Color.LightBlue;
        //}
        //else
        //{
        //  e.Row.Cells["MoveToContainer"].Appearance.BackColor = Color.White;
        //  e.Row.Cells["MoveQty"].Appearance.BackColor = Color.White;
        //}

        int affectToContDirect = DBConvert.ParseInt(e.Row.Cells["AffectToContDirect"].Value.ToString());
        if (affectToContDirect == 1)
        {
          e.Row.Cells["AffectToContDirect"].Appearance.ForeColor = Color.Red;
          e.Row.Cells["AffectToContDirect"].Appearance.FontData.Bold = DefaultableBoolean.True;
        }

        int scheduleDeadlineLate = DBConvert.ParseInt(e.Row.Cells["ScheduleDeadlineLate"].Value.ToString());
        if (scheduleDeadlineLate == 1)
        {
          e.Row.Cells["ScheduleDeadlineLate"].Appearance.ForeColor = Color.Red;
          e.Row.Cells["ScheduleDeadlineLate"].Appearance.FontData.Bold = DefaultableBoolean.True;
        }

        e.Row.Cells["ContainerRemark"].Appearance.BackColor = Color.LightBlue;
        e.Row.Cells["ItemKind"].Appearance.BackColor = Color.LightBlue;

        e.Row.Cells["ReasonMove"].Appearance.BackColor = Color.LightBlue;
        e.Row.Cells["OldConfirmShipdate"].Appearance.BackColor = Color.LightBlue;
        e.Row.Cells["NewConfirmShipdate"].Appearance.BackColor = Color.LightBlue;
        e.Row.Cells["ReasonChangeShipdate"].Appearance.BackColor = Color.LightBlue;
        e.Row.Cells["Remarks"].Appearance.BackColor = Color.LightBlue;
        // To mau Lock
        if (DBConvert.ParseInt(e.Row.Cells["LockContainerListDetail"].Value) == 1)
        {
          e.Row.Cells["LockContainerListDetail"].Appearance.BackColor = Color.Red;
          e.Row.Cells["MoveToContainer"].Activation = Activation.ActivateOnly;
        }
      }
    }
  }
}
