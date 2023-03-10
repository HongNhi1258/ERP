/*
  Author      : 
  Date        : 30/12/2013
  Description : Follow Deadline
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_98_016 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    public string itemCode = string.Empty;
    public string container = string.Empty;
    #endregion Field

    #region Init
    public viewPLN_98_016()
    {
      InitializeComponent();
    }

    private void viewPLN_98_016_Load(object sender, EventArgs e)
    {
      this.txtContainer.Text = this.container;
      this.ultContainer.Text = this.txtContainer.Text;

      // Load ItemCode
      this.LoadItemCode();

      // Load Customer
      this.LoadCustomer();

      // Load Container
      this.LoadContainer();

      // Load Container
      this.LoadGroupMaterial();

      DateTime today = DateTime.Today;
      DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
      DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

      ultShipDateFrom.Value = startOfMonth;
      ultShipDateTo.Value = endOfMonth;
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

    /// <summary>
    /// Load Customer
    /// </summary>
    private void LoadGroupMaterial()
    {
      string commandText = string.Empty;
      commandText = " SELECT [Group], Name FROM TblGNRMaterialGroup";

      DataTable dtGroup = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBGroup.DataSource = dtGroup;
      ultCBGroup.DataSource = dtGroup;
      ultCBGroup.DisplayMember = "Group";
      ultCBGroup.ValueMember = "Group";
      ultCBGroup.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBGroup.Value = "020";
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

      // WO
      text = this.txtWO.Text;
      if (text.Length > 0)
      {
        param[8] = new DBParameter("@WO", DbType.String, text);
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNMasterPlanMainDeadline_Exec1_Test", 10000, param);

      DataTable dtColor = new DataTable();
      dtColor.Columns.Add("Color", typeof(System.Int32));

      if (dsSource != null)
      {
        this.ultData.DataSource = dsSource.Tables[0];
      }
      else
      {
        ultData.DataSource = DBNull.Value;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];

        int checkWO = DBConvert.ParseInt(ultData.Rows[i].Cells["CheckWO"].Value.ToString());
        // Packing
        int checkPakDeadline = DBConvert.ParseInt(ultData.Rows[i].Cells["CheckPakDeadline"].Value.ToString());
        int checkOrderByPak = DBConvert.ParseInt(ultData.Rows[i].Cells["CheckOrderByPak"].Value.ToString());
        // Subcon
        int checkSubDeadline = DBConvert.ParseInt(ultData.Rows[i].Cells["CheckSubDeadline"].Value.ToString());
        int checkSubProcessDeadline = DBConvert.ParseInt(ultData.Rows[i].Cells["CheckSubProcessDeadline"].Value.ToString());
        // ASS
        int checkASSDeadline = DBConvert.ParseInt(ultData.Rows[i].Cells["CheckASSDeadline"].Value.ToString());
        // CST
        int checkCSTDeadline = DBConvert.ParseInt(ultData.Rows[i].Cells["CheckCSTDeadline"].Value.ToString());
        // COM1
        int checkCOM1Deadline = DBConvert.ParseInt(ultData.Rows[i].Cells["CheckCOM1Deadline"].Value.ToString());
        int checkCOM1ProcessDeadline = DBConvert.ParseInt(ultData.Rows[i].Cells["CheckCOM1ProcessDeadline"].Value.ToString());

        DateTime com1Deadline = DBConvert.ParseDateTime(row.Cells["COM1Deadline"].Value.ToString(),
                     USER_COMPUTER_FORMAT_DATETIME);
        DateTime processCOM1Deadline = DBConvert.ParseDateTime(row.Cells["DTCOM1ProcessDeadline"].Value.ToString(),
                     USER_COMPUTER_FORMAT_DATETIME);
        //DateTime com2Deadline = DBConvert.ParseDateTime(row.Cells["COM2DeadlineStatus"].Value.ToString(),
        //             USER_COMPUTER_FORMAT_DATETIME);
        //DateTime processCOM2Deadline = DBConvert.ParseDateTime(row.Cells["LCOM2ProcessDeadline"].Value.ToString(),
        //             USER_COMPUTER_FORMAT_DATETIME);

        DateTime subConDeadline = DBConvert.ParseDateTime(row.Cells["SUBDeadline"].Value.ToString(),
                     USER_COMPUTER_FORMAT_DATETIME);
        DateTime processSubConDeadline = DBConvert.ParseDateTime(row.Cells["DTLSUBDeadline"].Value.ToString(),
                     USER_COMPUTER_FORMAT_DATETIME);

        DateTime pakDeadline = DBConvert.ParseDateTime(row.Cells["PAKDeadline"].Value.ToString(),
                     USER_COMPUTER_FORMAT_DATETIME);
        DateTime processPAKDeadline = DBConvert.ParseDateTime(row.Cells["DTLPAKDeadline"].Value.ToString(),
                     USER_COMPUTER_FORMAT_DATETIME);

        // WO
        if (checkWO == 1)
        {
          ultData.Rows[i].Cells["WO"].Appearance.BackColor = Color.Orange;
        }

        // Packing
        if (checkPakDeadline == 1)
        {
          ultData.Rows[i].Cells["PAKDeadlineStatus"].Appearance.BackColor = Color.Orange;
        }

        // Subcon
        if (checkSubDeadline == 1)
        {
          ultData.Rows[i].Cells["SUBDeadlineStatus"].Appearance.BackColor = Color.Orange;
        }

        if (checkSubProcessDeadline == 1)
        {
          ultData.Rows[i].Cells["LSUBDeadline"].Appearance.BackColor = Color.Orange;
          ultData.Rows[i].Cells["LSUBWeek"].Appearance.BackColor = Color.Orange;
        }

        // ASS
        if (checkASSDeadline == 1)
        {
          ultData.Rows[i].Cells["ASSDeadline"].Appearance.BackColor = Color.Orange;
          ultData.Rows[i].Cells["ASSWeek"].Appearance.BackColor = Color.Orange;
        }

        // CST
        if (checkCSTDeadline == 1)
        {
          ultData.Rows[i].Cells["CSTDeadline"].Appearance.BackColor = Color.Orange;
          ultData.Rows[i].Cells["CSTWeek"].Appearance.BackColor = Color.Orange;
        }

        // COM1
        if (checkCOM1Deadline == 1)
        {
          ultData.Rows[i].Cells["COM1DeadlineStatus"].Appearance.BackColor = Color.Orange;
        }

        if (checkCOM1ProcessDeadline == 1)
        {
          ultData.Rows[i].Cells["LCOM1ProcessDeadline"].Appearance.BackColor = Color.Orange;
          ultData.Rows[i].Cells["LCOM1Week"].Appearance.BackColor = Color.Orange;
        }

        // Alert
        int result = 0;

        // Packing
        result = DateTime.Compare(pakDeadline, processPAKDeadline);
        if (result > 0 && checkOrderByPak == 1)
        {
          ultData.Rows[i].Cells["LPAKDeadline"].Appearance.ForeColor = Color.Red;
          ultData.Rows[i].Cells["PAKDeadlineStatus"].Appearance.ForeColor = Color.Red;
        }

        // Subcon
        result = DateTime.Compare(subConDeadline, processSubConDeadline);
        if (result > 0 && checkSubDeadline == 0)
        {
          ultData.Rows[i].Cells["LSUBDeadline"].Appearance.ForeColor = Color.Red;
          ultData.Rows[i].Cells["SUBDeadlineStatus"].Appearance.ForeColor = Color.Red;
        }

        // COM1
        result = DateTime.Compare(com1Deadline, processCOM1Deadline);
        if (result > 0 && checkCOM1Deadline == 0)
        {
          ultData.Rows[i].Cells["LCOM1ProcessDeadline"].Appearance.ForeColor = Color.Red;
          ultData.Rows[i].Cells["COM1DeadlineStatus"].Appearance.ForeColor = Color.Red;
        }
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

        if (string.Compare(column.ColumnName, "ShipDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "CustomerPONO", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "Qty", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "CSTDeadline", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "CSTWeek", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "CSTCBM", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "SUBDeadlineStatus", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LSUBDeadline", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LSUBWeek", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "SUBCBM", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "SubMinuteDate", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "PAKDeadlineStatus", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LPAKDeadline", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LPAKWeek", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "PACCBM", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "PackingActual", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "CarcassWONo", true) == 0)
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
          ultData.DisplayLayout.Bands[0].Columns[cell.Column.ToString().Trim()].Hidden = !cell.Value.ToString().Equals("1");
        }
      }
    }

    /// <summary>
    /// Export Excel
    /// </summary>
    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {


        Utility.ExportToExcelWithDefaultPath(ultData, "Follow Up Deadline Container No");



      }
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

      e.Layout.Bands[0].Columns["StandByEN"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["QtyWIP"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["WO"].Header.Fixed = true;

      e.Layout.Bands[0].Columns["WorkAreaPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerListDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["COM1Deadline"].Hidden = true;
      e.Layout.Bands[0].Columns["SUBDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["SUBQty"].Hidden = true;
      e.Layout.Bands[0].Columns["PAKDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["PAKQty"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleCode"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckWO"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckPakDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckOrderByPak"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckSubDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckSubProcessDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckASSDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckCSTDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckCOM1Deadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckCOM1ProcessDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["DTLSUBDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["DTLPAKDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["DTCOM1ProcessDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["WorkAreaName"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Hidden = true;
      e.Layout.Bands[0].Columns["TCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Hidden = true;
      e.Layout.Bands[0].Columns["COM1DeadlineStatus"].Hidden = true;
      e.Layout.Bands[0].Columns["LCOM1Week"].Hidden = true;
      e.Layout.Bands[0].Columns["LCOM1ProcessDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CSTMinuteDate"].Hidden = true;
      e.Layout.Bands[0].Columns["COM1CBM"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSWeek"].Hidden = true;
      e.Layout.Bands[0].Columns["ASSCBM"].Hidden = true;

      e.Layout.Bands[0].Columns["StandByEN"].Header.Caption = "Work\nStation";
      e.Layout.Bands[0].Columns["ShipdateStr"].Header.Caption = "Loading\nDate";
      e.Layout.Bands[0].Columns["CustomerPONO"].Header.Caption = "Customer\nPONO";
      e.Layout.Bands[0].Columns["COM1DeadlineStatus"].Header.Caption = "COM1\nDeadline";
      e.Layout.Bands[0].Columns["QtyWip"].Header.Caption = "Qty\nWip";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Loading List\nQty";
      e.Layout.Bands[0].Columns["LCOM1ProcessDeadline"].Header.Caption = "COM1\nDeadline";
      e.Layout.Bands[0].Columns["LCOM1Week"].Header.Caption = "COM1\nWeek";
      e.Layout.Bands[0].Columns["COM1DeadlineStatus"].Header.Caption = "PLN\nCOM1\nDeadline";
      e.Layout.Bands[0].Columns["CSTMinuteDate"].Header.Caption = "PAK\nMinute\nCOM1";
      e.Layout.Bands[0].Columns["COM1CBM"].Header.Caption = "COM1\nCBM";
      //e.Layout.Bands[0].Columns["COM2DeadlineStatus"].Header.Caption = "PLN\nCOM2\nDeadline";
      //e.Layout.Bands[0].Columns["LCOM2ProcessDeadline"].Header.Caption = "COM2\nDeadline";
      e.Layout.Bands[0].Columns["CSTDeadline"].Header.Caption = "CST\nDeadline";
      e.Layout.Bands[0].Columns["CSTWeek"].Header.Caption = "CST\nWeek";
      e.Layout.Bands[0].Columns["CSTCBM"].Header.Caption = "CST\nCBM";
      e.Layout.Bands[0].Columns["ASSDeadline"].Header.Caption = "ASS\nDeadline";
      e.Layout.Bands[0].Columns["ASSWeek"].Header.Caption = "ASS\nWeek";
      e.Layout.Bands[0].Columns["ASSCBM"].Header.Caption = "ASS\nCBM";
      e.Layout.Bands[0].Columns["SUBDeadlineStatus"].Header.Caption = "PLN\nSUB\nDeadline";
      e.Layout.Bands[0].Columns["LSUBDeadline"].Header.Caption = "SUB\nDeadline";
      e.Layout.Bands[0].Columns["LSUBWeek"].Header.Caption = "SUB\nWeek";
      e.Layout.Bands[0].Columns["SUBCBM"].Header.Caption = "SUB\nCBM";
      e.Layout.Bands[0].Columns["SubMinuteDate"].Header.Caption = "PAK\nMinute\nSUB";
      e.Layout.Bands[0].Columns["PAKDeadlineStatus"].Header.Caption = "PLN\nPAK\nDeadline";
      e.Layout.Bands[0].Columns["LPAKDeadline"].Header.Caption = "PAK\nDeadline";
      e.Layout.Bands[0].Columns["LPAKWeek"].Header.Caption = "PAK\nWeek";
      e.Layout.Bands[0].Columns["PACCBM"].Header.Caption = "PAK\nCBM";
      e.Layout.Bands[0].Columns["PackingActual"].Header.Caption = "PAK\nActual";
      e.Layout.Bands[0].Columns["CarcassWONo"].Header.Caption = "Carcass\nWO";

      e.Layout.Bands[0].Columns["StandByEN"].CellAppearance.ForeColor = Color.SteelBlue;
      e.Layout.Bands[0].Columns["StandByEN"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["QtyWip"].CellAppearance.ForeColor = Color.SteelBlue;
      e.Layout.Bands[0].Columns["QtyWip"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.ForeColor = Color.Sienna;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["COM1DeadlineStatus"].CellAppearance.BackColor = Color.Tan;
      e.Layout.Bands[0].Columns["LCOM1Week"].CellAppearance.BackColor = Color.Tan;
      e.Layout.Bands[0].Columns["COM1CBM"].CellAppearance.BackColor = Color.Tan;
      e.Layout.Bands[0].Columns["LCOM1ProcessDeadline"].CellAppearance.BackColor = Color.Tan;
      e.Layout.Bands[0].Columns["CSTMinuteDate"].CellAppearance.BackColor = Color.Tan;
      e.Layout.Bands[0].Columns["CSTDeadline"].CellAppearance.BackColor = Color.BurlyWood;
      e.Layout.Bands[0].Columns["CSTWeek"].CellAppearance.BackColor = Color.BurlyWood;
      e.Layout.Bands[0].Columns["CSTCBM"].CellAppearance.BackColor = Color.BurlyWood;
      e.Layout.Bands[0].Columns["ASSDeadline"].CellAppearance.BackColor = Color.DarkSeaGreen;
      e.Layout.Bands[0].Columns["ASSWeek"].CellAppearance.BackColor = Color.DarkSeaGreen;
      e.Layout.Bands[0].Columns["ASSCBM"].CellAppearance.BackColor = Color.DarkSeaGreen;
      e.Layout.Bands[0].Columns["SUBDeadlineStatus"].CellAppearance.BackColor = Color.MediumAquamarine;
      e.Layout.Bands[0].Columns["LSUBDeadline"].CellAppearance.BackColor = Color.MediumAquamarine;
      e.Layout.Bands[0].Columns["LSUBWeek"].CellAppearance.BackColor = Color.MediumAquamarine;
      e.Layout.Bands[0].Columns["SubMinuteDate"].CellAppearance.BackColor = Color.MediumAquamarine;
      e.Layout.Bands[0].Columns["SUBCBM"].CellAppearance.BackColor = Color.MediumAquamarine;
      e.Layout.Bands[0].Columns["PAKDeadlineStatus"].CellAppearance.BackColor = Color.Plum;
      e.Layout.Bands[0].Columns["LPAKDeadline"].CellAppearance.BackColor = Color.Plum;
      e.Layout.Bands[0].Columns["LPAKWeek"].CellAppearance.BackColor = Color.Plum;
      e.Layout.Bands[0].Columns["PACCBM"].CellAppearance.BackColor = Color.Plum;
      e.Layout.Bands[0].Columns["CarcassWONo"].CellAppearance.FontData.Bold = DefaultableBoolean.True;

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 3;

      e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
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
      // Check Material Group
      if (ultCBGroup.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Material Group");
        return;
      }
      // End
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

    private void ucCustomer_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtCustomer.Text = this.ucCustomer.SelectedValue;
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
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
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 3;

      e.Layout.Bands[0].Columns["WorkAreaPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerListDetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["COM1Deadline"].Hidden = true;
      e.Layout.Bands[0].Columns["SUBDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["SUBQty"].Hidden = true;
      e.Layout.Bands[0].Columns["PAKDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["PAKQty"].Hidden = true;
      e.Layout.Bands[0].Columns["StandByEN"].Hidden = true;
      e.Layout.Bands[0].Columns["QtyWIP"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["WO"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckWO"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckPakDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckOrderByPak"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckSubDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckSubProcessDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckASSDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckCSTDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckCOM1Deadline"].Hidden = true;
      e.Layout.Bands[0].Columns["CheckCOM1ProcessDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["DTLSUBDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["DTLPAKDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["DTCOM1ProcessDeadline"].Hidden = true;

      e.Layout.Bands[0].Columns["StandByEN"].Header.Caption = "Work\nStation";
      e.Layout.Bands[0].Columns["ShipdateStr"].Header.Caption = "Loading\nDate";
      e.Layout.Bands[0].Columns["CustomerPONO"].Header.Caption = "Customer\nPONO";
      e.Layout.Bands[0].Columns["LCOM1ProcessDeadline"].Header.Caption = "COM1\nDeadline";
      e.Layout.Bands[0].Columns["QtyWip"].Header.Caption = "Qty\nWip";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Loading List\nQty";
      e.Layout.Bands[0].Columns["LCOM1ProcessDeadline"].Header.Caption = "COM1\nDeadline";
      e.Layout.Bands[0].Columns["COM1DeadlineStatus"].Header.Caption = "PLN\nCOM1\nDeadline";
      e.Layout.Bands[0].Columns["CSTMinuteDate"].Header.Caption = "PAK\nMinute\nCOM1";
      e.Layout.Bands[0].Columns["LCOM1Week"].Header.Caption = "COM1\nWeek";
      e.Layout.Bands[0].Columns["COM1CBM"].Header.Caption = "COM1\nCBM";
      e.Layout.Bands[0].Columns["CSTDeadline"].Header.Caption = "CST\nDeadline";
      e.Layout.Bands[0].Columns["CSTWeek"].Header.Caption = "CST\nWeek";
      e.Layout.Bands[0].Columns["CSTCBM"].Header.Caption = "CST\nCBM";
      e.Layout.Bands[0].Columns["ASSDeadline"].Header.Caption = "ASS\nDeadline";
      e.Layout.Bands[0].Columns["ASSWeek"].Header.Caption = "ASS\nWeek";
      e.Layout.Bands[0].Columns["ASSCBM"].Header.Caption = "ASS\nCBM";
      e.Layout.Bands[0].Columns["SUBDeadlineStatus"].Header.Caption = "PLN\nSUB\nDeadline";
      e.Layout.Bands[0].Columns["LSUBDeadline"].Header.Caption = "SUB\nDeadline";
      e.Layout.Bands[0].Columns["LSUBWeek"].Header.Caption = "SUB\nWeek";
      e.Layout.Bands[0].Columns["SUBMinuteDate"].Header.Caption = "PAK\nMinute\nSUB";
      e.Layout.Bands[0].Columns["SUBCBM"].Header.Caption = "SUB\nCBM";
      e.Layout.Bands[0].Columns["PAKDeadlineStatus"].Header.Caption = "PLN\nPAK\nDeadline";
      e.Layout.Bands[0].Columns["LPAKDeadline"].Header.Caption = "PAK\nDeadline";
      e.Layout.Bands[0].Columns["LPAKWeek"].Header.Caption = "PAK\nWeek";
      e.Layout.Bands[0].Columns["PACCBM"].Header.Caption = "PAK\nCBM";
      e.Layout.Bands[0].Columns["PackingActual"].Header.Caption = "PAK\nActual";
      e.Layout.Bands[0].Columns["CarcassWONo"].Header.Caption = "Carcass\nWO";

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

    private void txtContainer_Leave(object sender, EventArgs e)
    {
      this.ultContainer.Text = this.txtContainer.Text;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      // Load ItemCode
      this.LoadItemCode();

      // Load Customer
      this.LoadCustomer();

      // Load Container
      this.LoadContainer();

      this.txtItemCode.Text = string.Empty;
      this.txtCustomer.Text = string.Empty;
      this.txtContainer.Text = string.Empty;
      this.txtSaleCode.Text = string.Empty;
      this.txtCarcassCode.Text = string.Empty;
      this.ultContainer.Text = string.Empty;
      this.txtOldCode.Text = string.Empty;
      this.txtWO.Text = string.Empty;
      this.ultShipDateFrom.Value = DBNull.Value;
      this.ultShipDateTo.Value = DBNull.Value;
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      viewPLN_98_018 uc = new viewPLN_98_018();
      DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "FOLLOW DEADLINE(COMBINE ITEM)", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    private void btnMaterial_Click(object sender, EventArgs e)
    {
      viewPLN_98_021 uc = new viewPLN_98_021();
      DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "MATERIAL INFORMATION", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    private void btnFoundry_Click(object sender, EventArgs e)
    {
      viewPLN_98_020 uc = new viewPLN_98_020();
      DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "FOUNDRY SHIPPING INFORMATION", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    private void btnOutput_Click(object sender, EventArgs e)
    {
      viewPLN_98_017 uc = new viewPLN_98_017();
      DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "OUTPUT PAK & ASS", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    private void btnFoundryDL_Click(object sender, EventArgs e)
    {
      viewPLN_98_022 uc = new viewPLN_98_022();
      DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "FOUNDRY DEADLINE INFORMATION", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }
    #endregion Event
  }
}
