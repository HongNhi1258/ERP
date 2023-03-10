/*
  Author      : 
  Date        : 12/12/2012
  Description : Master Plan For Container(SO not yet put on container)
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
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_98_009 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    DataTable dtDetail = new DataTable();
    #endregion Field

    #region Init
    public viewPLN_98_009()
    {
      InitializeComponent();
      this.ultSOReceiveFrom.Value = DBNull.Value;
      this.ultSOReceiveTo.Value = DBNull.Value;
    }

    private void viewWHD_05_002_Load(object sender, EventArgs e)
    {
      this.LoadCustomer();
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
    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] param = new DBParameter[7];

      // ItemCode
      string text = string.Empty;
      text = this.txtItemCodeAll.Text;
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

      // SaleCode
      text = string.Empty;
      text = this.txtSaleCode.Text;
      if (text.Length > 0)
      {
        param[2] = new DBParameter("@SaleCode", DbType.String, text);
      }

      // Carcass Code
      text = this.txtCarcassCode.Text;
      if (text.Length > 0)
      {
        param[3] = new DBParameter("@CarcassCode", DbType.String, text);
      }

      // PONo
      text = this.txtPONo.Text;
      if (text.Length > 0)
      {
        param[4] = new DBParameter("@PONo", DbType.String, text);
      }

      // SO Received Date From
      if (this.ultSOReceiveFrom.Value != null)
      {
        param[5] = new DBParameter("@SOReceiveDateFrom", DbType.DateTime, DBConvert.ParseDateTime(this.ultSOReceiveFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
      }

      // SO Received Date To
      if (this.ultSOReceiveTo.Value != null)
      {
        param[6] = new DBParameter("@SOReceiveDateTo", DbType.DateTime, DBConvert.ParseDateTime(this.ultSOReceiveTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME).AddDays(1));
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNMasterPlanSONotYetPutOnContainer_Select", 300, param);
      dtDetail = dsSource.Tables[1];

      if (dsSource != null)
      {
        this.ultData.DataSource = dsSource.Tables[0];

        for (int i = 0; i < this.ultData.Rows.Count; i++)
        {
          UltraGridRow rowGrid = this.ultData.Rows[i];
          int customerPid = DBConvert.ParseInt(rowGrid.Cells["CustomerPid"].Value.ToString());

          string select = string.Empty;
          select = "CustomerPid =" + customerPid;
          UltraDropDown ultContainer = (UltraDropDown)rowGrid.Cells["ContainerNo"].ValueList;

          DataRow[] foundRow = dsSource.Tables[1].Select(select);
          DataTable dtContainer = this.UltraTable();
          for (int j = 0; j < foundRow.Length; j++)
          {
            DataRow rowAdd = dtContainer.NewRow();
            rowAdd["ContainerNo"] = foundRow[j]["ContainerNo"].ToString();
            rowAdd["LoadingDate"] = foundRow[j]["LoadingDate"].ToString();
            rowAdd["ContainerListPid"] = DBConvert.ParseLong(foundRow[j]["ContainerListPid"].ToString());
            rowAdd["LoadingQty"] = DBConvert.ParseInt(foundRow[j]["LoadingQty"].ToString());
            rowAdd["LoadingCBM"] = DBConvert.ParseDouble(foundRow[j]["LoadingCBM"].ToString());
            rowAdd["AddQty"] = DBConvert.ParseInt(rowGrid.Cells["Remain"].Value.ToString());
            rowAdd["CustomerPid"] = DBConvert.ParseInt(foundRow[j]["CustomerPid"].ToString());

            dtContainer.Rows.Add(rowAdd);
          }

          if (ultContainer == null)
          {
            ultContainer = new UltraDropDown();
            this.Controls.Add(ultContainer);
          }

          ultContainer.DataSource = dtContainer;
          ultContainer.ValueMember = "ContainerListPid";
          ultContainer.DisplayMember = "ContainerNo";
          ultContainer.DisplayLayout.Bands[0].Columns["ContainerListPid"].Hidden = true;
          ultContainer.DisplayLayout.Bands[0].Columns["CustomerPid"].Hidden = true;
          ultContainer.Visible = false;

          rowGrid.Cells["ContainerNo"].ValueList = ultContainer;

          // To Mau No
          int moreThanCustomer = DBConvert.ParseInt(ultData.Rows[i].Cells["NoteCustomer"].Value.ToString());
          if (moreThanCustomer == 1)
          {
            ultData.Rows[i].Cells["No"].Appearance.FontData.Bold = DefaultableBoolean.True;
            ultData.Rows[i].Cells["No"].Appearance.ForeColor = Color.Red;
          }

          ultData.Rows[i].Cells["DateToPLN"].Appearance.FontData.Bold = DefaultableBoolean.True;
          ultData.Rows[i].Cells["DateToPLN"].Appearance.ForeColor = Color.Blue;
        }
      }
      else
      {
        ultData.DataSource = DBNull.Value;
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
      dt.Columns.Add("ContainerNo", typeof(System.String));
      dt.Columns.Add("LoadingDate", typeof(System.String));
      dt.Columns.Add("ContainerListPid", typeof(System.Int64));
      dt.Columns.Add("LoadingQty", typeof(System.Int32));
      dt.Columns.Add("LoadingCBM", typeof(System.Double));
      dt.Columns.Add("AddQty", typeof(System.Int32));
      dt.Columns.Add("CustomerPid", typeof(System.Int32));

      return dt;
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

        if (string.Compare(column.ColumnName, "CustCode", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

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

        if (string.Compare(column.ColumnName, "DateToPLN", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "Remain", true) == 0)
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

        if (string.Compare(column.ColumnName, "Item/Box", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "ContainerNo", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "LoadingCBM", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "AddQty", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "AddCBM", true) == 0)
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

        if (string.Compare(column.ColumnName, "MCHDeadline", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "SUBDeadline", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }


        if (string.Compare(column.ColumnName, "FOUDeadline", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "USStock", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "AVEUS6M", true) == 0)
        {
          dtNew.Columns[column.ColumnName].DefaultValue = 1;
        }

        if (string.Compare(column.ColumnName, "AVEUS12M", true) == 0)
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
        Excel.Workbook xlBook;

        ControlUtility.ExportToExcelWithDefaultPath(ultData, out xlBook, "SO NOT YET PUT ON CONTAINER", 6);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlSheet.Cells[1, 1] = "VFR Co., LTD";
        Excel.Range r = xlSheet.get_Range("A1", "A1");

        xlSheet.Cells[2, 1] = "Binh Chuan Ward, Thuan An Town, Binh Duong Province, Vietnam.";

        xlSheet.Cells[3, 1] = "SO NOT YET PUT ON CONTAINER";
        r = xlSheet.get_Range("A3", "A3");
        r.Font.Bold = true;
        r.Font.Size = 14;
        r.EntireRow.RowHeight = 20;

        xlSheet.Cells[4, 1] = "Date: ";
        r.Font.Bold = true;
        xlSheet.Cells[4, 2] = DBConvert.ParseString(DateTime.Now, ConstantClass.FORMAT_DATETIME);

        //xlBook.Application.DisplayAlerts = false;

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
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
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 3;
      e.Layout.Bands[0].Columns["PendingContainer"].Header.Caption = "PendingShip";
      e.Layout.Bands[0].Columns["QtyCanPutOnContainer"].Header.Caption = "Qty\nCan Put\n On Container";
      e.Layout.Bands[0].Columns["QtyOnContainer"].Header.Caption = "Qty\nOn Container";
      e.Layout.Bands[0].Columns["ConfirmedShipDate"].Header.Caption = "Estimated\nShipDate";

      e.Layout.Bands[0].Columns["No"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["SaleCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["OldCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Fixed = true;
      e.Layout.Bands[0].Columns["CustomerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;

      e.Layout.Bands[0].Columns["Revision"].Hidden = true;
      e.Layout.Bands[0].Columns["UCBM"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleNo"].Hidden = true;
      e.Layout.Bands[0].Columns["SpecialRemark"].Hidden = true;
      e.Layout.Bands[0].Columns["PackingNote"].Hidden = true;
      e.Layout.Bands[0].Columns["UrgentNote"].Hidden = true;
      e.Layout.Bands[0].Columns["Remark & Delivery Requirement"].Hidden = true;
      e.Layout.Bands[0].Columns["OrderQty"].Hidden = true;
      e.Layout.Bands[0].Columns["ShippedQty"].Hidden = true;
      e.Layout.Bands[0].Columns["CancelledQty"].Hidden = true;
      e.Layout.Bands[0].Columns["PendingContainer"].Hidden = true;
      e.Layout.Bands[0].Columns["Balance"].Hidden = true;
      e.Layout.Bands[0].Columns["House/Sub"].Hidden = true;
      e.Layout.Bands[0].Columns["LoadingDate"].Hidden = true;
      e.Layout.Bands[0].Columns["LoadingQty"].Hidden = true;
      e.Layout.Bands[0].Columns["NoteCustomer"].Hidden = true;
      e.Layout.Bands[0].Columns["COntainerNoName"].Hidden = true;
      e.Layout.Bands[0].Columns["PLASORecDate"].Hidden = true;
      e.Layout.Bands[0].Columns["Direct"].Hidden = true;

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["UCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LoadingQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LoadingCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["OrderQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ShippedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["CancelledQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["PendingContainer"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Balance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalBalance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalWIP"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalUnrelease"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AVEUS6M"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AVEUS12M"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AddQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AddCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyCanPutOnContainer"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyOnContainer"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        if (e.Layout.Bands[0].Columns[i].Header.Caption == "ContainerNo" || e.Layout.Bands[0].Columns[i].Header.Caption == "AddQty")
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
        }
        else
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }

      e.Layout.Bands[0].Columns["ContainerNo"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["AddQty"].CellAppearance.BackColor = Color.Aqua;

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
      // Check Qty
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseLong(ultData.Rows[i].Cells["ContainerNo"].Value.ToString()) != long.MinValue)
        {
          long remainPendingQty = DBConvert.ParseLong(ultData.Rows[i].Cells["QtyCanPutOnContainer"].Value.ToString()) -
                  DBConvert.ParseLong(ultData.Rows[i].Cells["QtyOnContainer"].Value.ToString());
          long remain = DBConvert.ParseLong(ultData.Rows[i].Cells["Remain"].Value.ToString());
          long qtyPut = 0;
          if (remain > remainPendingQty)
          {
            qtyPut = remainPendingQty;
          }
          else
          {
            qtyPut = remain;
          }

          if (DBConvert.ParseInt(ultData.Rows[i].Cells["AddQty"].Value.ToString()) == int.MinValue || (DBConvert.ParseInt(ultData.Rows[i].Cells["AddQty"].Value.ToString()) > qtyPut))
          {
            WindowUtinity.ShowMessageError("ERR0001", "0 < AddQty <= Remain");
            return;
          }
        }
      }

      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        // End
        if (DBConvert.ParseLong(ultData.Rows[i].Cells["ContainerNo"].Value.ToString()) != long.MinValue && DBConvert.ParseInt(ultData.Rows[i].Cells["AddQty"].Value.ToString()) != int.MinValue)
        {
          UltraGridRow rowGrid = this.ultData.Rows[i];
          long containerListPid = DBConvert.ParseLong(rowGrid.Cells["ContainerNo"].Value.ToString());
          long SoPid = DBConvert.ParseLong(rowGrid.Cells["SoPid"].Value.ToString());
          string itemCode = rowGrid.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(rowGrid.Cells["Revision"].Value.ToString());
          int qty = DBConvert.ParseInt(rowGrid.Cells["AddQty"].Value.ToString());

          DBParameter[] inputParam = new DBParameter[5];
          inputParam[0] = new DBParameter("@ContainerListPid", DbType.Int64, containerListPid);
          inputParam[1] = new DBParameter("@SOPid", DbType.Int64, SoPid);
          inputParam[2] = new DBParameter("@ItemCode", DbType.String, itemCode);
          inputParam[3] = new DBParameter("@Revision", DbType.Int32, revision);
          inputParam[4] = new DBParameter("@Qty", DbType.Int32, qty);

          DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanSONotYetPutOnContainerList_Insert", inputParam);
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");

      this.Search();
    }

    //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    //{
    //  if (keyData == Keys.Enter)
    //  {
    //    this.Search();
    //    return true;
    //  }
    //  return base.ProcessCmdKey(ref msg, keyData);
    //}

    //private void btnRefreshData_Click(object sender, EventArgs e)
    //{
    //  System.Threading.Thread Thead2 = new System.Threading.Thread(new System.Threading.ThreadStart(Progessbar));
    //  Thead2.Start();
    //  DBParameter[] inputParam = new DBParameter[2];
    //  DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerSummarizeTotalData_Insert", 300, inputParam);
    //  WindowUtinity.ShowMessageSuccess("MSG0059");
    //  this.Search();
    //}

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

      long remainPendingQty = DBConvert.ParseLong(e.Cell.Row.Cells["QtyCanPutOnContainer"].Value.ToString()) -
                  DBConvert.ParseLong(e.Cell.Row.Cells["QtyOnContainer"].Value.ToString());
      long remain = DBConvert.ParseLong(e.Cell.Row.Cells["Remain"].Value.ToString());

      switch (columnName)
      {
        case "containerno":
          long containerListPid = DBConvert.ParseLong(e.Cell.Row.Cells["ContainerNo"].Value.ToString());
          if (containerListPid != long.MinValue)
          {
            string select = string.Empty;
            select = "ContainerListPid =" + containerListPid;
            DataRow[] foundRow = dtDetail.Select(select);
            e.Cell.Row.Cells["ContainerNoName"].Value = foundRow[0]["ContainerNo"].ToString();
            e.Cell.Row.Cells["LoadingDate"].Value = foundRow[0]["LoadingDate"].ToString();
            e.Cell.Row.Cells["LoadingQty"].Value = DBConvert.ParseLong(foundRow[0]["LoadingQty"].ToString());
            e.Cell.Row.Cells["LoadingCBM"].Value = DBConvert.ParseDouble(foundRow[0]["LoadingCBM"].ToString());
            if (remainPendingQty > remain)
            {
              e.Cell.Row.Cells["AddQty"].Value = remain;
            }
            else
            {
              e.Cell.Row.Cells["AddQty"].Value = remainPendingQty;
            }
          }
          else
          {
            e.Cell.Row.Cells["LoadingDate"].Value = "";
            e.Cell.Row.Cells["LoadingQty"].Value = "";
            e.Cell.Row.Cells["LoadingCBM"].Value = "";
            e.Cell.Row.Cells["AddQty"].Value = "";
          }
          break;
        case "addqty":
          if (DBConvert.ParseLong(e.Cell.Row.Cells["AddQty"].Value.ToString()) != long.MinValue)
          {
            long qtyPut = 0;
            if (remain > remainPendingQty)
            {
              qtyPut = remainPendingQty;
            }
            else
            {
              qtyPut = remain;
            }

            if (DBConvert.ParseLong(e.Cell.Row.Cells["AddQty"].Value.ToString()) > qtyPut)
            {
              WindowUtinity.ShowMessageError("ERR0001", "AddQty < Remain");
              e.Cell.Row.Cells["AddQty"].Value = 0;
              e.Cell.Row.Cells["AddCBM"].Value = 0;
              return;
            }
            e.Cell.Row.Cells["AddCBM"].Value = DBConvert.ParseLong(e.Cell.Row.Cells["AddQty"].Value.ToString()) * DBConvert.ParseDouble(e.Cell.Row.Cells["UCBM"].Value.ToString());
          }
          else
          {
            e.Cell.Row.Cells["AddCBM"].Value = "";
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
        long soPid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["SoPid"].Value.ToString().Trim());
        long customerPid = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["CustomerPid"].Value.ToString().Trim());
        long qty = DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["Remain"].Value.ToString().Trim());
        viewPLN_98_011 uc = new viewPLN_98_011();
        if (itemCode.Length > 0)
        {
          uc.itemCode = itemCode;
          uc.revision = revision;
          uc.soPid = soPid;
          uc.customerPid = customerPid;
          uc.qty = qty;
          Shared.Utility.WindowUtinity.ShowView(uc, "ADD CONTANER", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Normal);
        }
        if (uc.flag == true)
        {
          this.Search();
        }
      }
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      ControlUtility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      ControlUtility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
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
      e.Layout.Bands[0].Columns["PendingContainer"].Header.Caption = "PendingShip";

      e.Layout.Bands[0].Columns["SaleCode"].Hidden = true;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["OldCode"].Hidden = true;
      e.Layout.Bands[0].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["CustomerPid"].Hidden = true;
      e.Layout.Bands[0].Columns["NoteCustomer"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerNoName"].Hidden = true;
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

    private void btnAddContainer_Click(object sender, EventArgs e)
    {
      try
      {
        viewPLN_98_010 view = new viewPLN_98_010();
        Shared.Utility.WindowUtinity.ShowView(view, "ADD CONTAINER", true, DaiCo.Shared.Utility.ViewState.ModalWindow);
        if (view.dtDetail != null && view.dtDetail.Rows.Count == 1)
        {
          DataRow row = dtDetail.NewRow();
          row["ContainerNo"] = view.dtDetail.Rows[0]["ContainerNo"].ToString();
          row["LoadingDate"] = view.dtDetail.Rows[0]["LoadingDate"].ToString();
          row["ContainerListPid"] = view.dtDetail.Rows[0]["ContainerListPid"].ToString();
          row["LoadingQty"] = DBConvert.ParseInt(view.dtDetail.Rows[0]["LoadingQty"].ToString());
          row["LoadingCBM"] = DBConvert.ParseDouble(view.dtDetail.Rows[0]["LoadingCBM"].ToString());
          row["CustomerPid"] = DBConvert.ParseInt(view.dtDetail.Rows[0]["CustomerPid"].ToString());
          dtDetail.Rows.Add(row);

          for (int i = 0; i < this.ultData.Rows.Count; i++)
          {
            UltraGridRow rowGrid = this.ultData.Rows[i];
            int customerPid = DBConvert.ParseInt(rowGrid.Cells["CustomerPid"].Value.ToString());

            string select = string.Empty;
            select = "CustomerPid =" + customerPid;
            UltraDropDown ultContainer = (UltraDropDown)rowGrid.Cells["ContainerNo"].ValueList;

            DataRow[] foundRow = dtDetail.Select(select);
            DataTable dtContainer = this.UltraTable();
            for (int j = 0; j < foundRow.Length; j++)
            {
              DataRow rowAdd = dtContainer.NewRow();
              rowAdd["ContainerNo"] = foundRow[j]["ContainerNo"].ToString();
              rowAdd["LoadingDate"] = foundRow[j]["LoadingDate"].ToString();
              rowAdd["ContainerListPid"] = DBConvert.ParseLong(foundRow[j]["ContainerListPid"].ToString());
              rowAdd["LoadingQty"] = DBConvert.ParseInt(foundRow[j]["LoadingQty"].ToString());
              rowAdd["LoadingCBM"] = DBConvert.ParseDouble(foundRow[j]["LoadingCBM"].ToString());
              rowAdd["CustomerPid"] = DBConvert.ParseInt(foundRow[j]["CustomerPid"].ToString());

              dtContainer.Rows.Add(rowAdd);
            }

            if (ultContainer == null)
            {
              if (dtContainer.Rows.Count > 0)
              {
                //DataRow row = dtColor.NewRow();
                //row["Color"] = i;
                //dtColor.Rows.Add(row);
              }
              ultContainer = new UltraDropDown();
              this.Controls.Add(ultContainer);
            }

            ultContainer.DataSource = dtContainer;
            ultContainer.ValueMember = "ContainerListPid";
            ultContainer.DisplayMember = "ContainerNo";
            ultContainer.DisplayLayout.Bands[0].Columns["ContainerListPid"].Hidden = true;
            ultContainer.DisplayLayout.Bands[0].Columns["CustomerPid"].Hidden = true;
            ultContainer.Visible = false;

            rowGrid.Cells["ContainerNo"].ValueList = ultContainer;
          }
        }
      }
      catch
      {
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtCarcassCode.Text = string.Empty;
      txtCustomer.Text = string.Empty;
      txtItemCodeAll.Text = string.Empty;
      txtSaleCode.Text = string.Empty;
      txtPONo.Text = string.Empty;
      ultSOReceiveFrom.Value = DBNull.Value;
      ultSOReceiveTo.Value = DBNull.Value;
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {

    }
    #endregion Event
  }
}
