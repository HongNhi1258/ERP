/*
  Author  : Vo Van Duy Qui
  Email   : qui_it@daico-furniture.com
  Date    : 12-10-2010
  Company : Dai Co 
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Planning;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_06_015 : MainUserControl
  {
    #region Init
    public int flagWindow = 0;
    public viewPLN_06_015()
    {
      InitializeComponent();
      dtpShipDateFrom.Value = DateTime.MinValue;
      dtpShipDateTo.Value = DateTime.MinValue;
      btnClose.Click += new EventHandler(btnClose_Click);
      btnNew.Click += new EventHandler(btnNew_Click);
      btnSearch.Click += new EventHandler(btnSearch_Click);
      btnDelete.Click += new EventHandler(btnDelete_Click);
      btnClear.Click += new EventHandler(btnClear_Click);
    }

    /// <summary>
    /// User Control Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_06_015_Load(object sender, EventArgs e)
    {
      this.LoadComboBoxCustomer();
      string dep = Shared.Utility.SharedObject.UserInfo.Department;
    }
    #endregion Init

    #region LoadData

    /// <summary>
    /// Load ComboBox Customer
    /// </summary>
    private void LoadComboBoxCustomer()
    {
      string commandText = "SELECT Pid, CustomerCode + ' - ' + Name Customer FROM TblCSDCustomerInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);
      cmbCustomer.DataSource = dtSource;
      cmbCustomer.ValueMember = "Pid";
      cmbCustomer.DisplayMember = "Customer";
    }

    /// <summary>
    /// Search Container Information
    /// </summary>
    private void Search()
    {
      string contName = txtContName.Text.Trim();
      int status = int.MinValue;
      if (cmbStatus.SelectedIndex > 0)
      {
        status = cmbStatus.SelectedIndex - 1;
      }
      DateTime dateFrom = dtpShipDateFrom.Value;
      DateTime dateTo = dtpShipDateTo.Value;
      if (dateTo != DateTime.MaxValue && dateTo != DateTime.MinValue)
      {
        dateTo = dateTo.AddDays(1);
      }

      long cusPid = long.MinValue;
      if (cmbCustomer.SelectedIndex > 0)
      {
        cusPid = DBConvert.ParseLong(cmbCustomer.SelectedValue.ToString());
      }

      DBParameter[] input = new DBParameter[8];
      if (contName.Length > 0)
      {
        input[0] = new DBParameter("@ContainerNo", DbType.AnsiString, 16, contName);
      }
      if (status != int.MinValue)
      {
        input[1] = new DBParameter("@Status", DbType.Int32, status);
      }
      if (dateFrom != DateTime.MinValue)
      {
        input[2] = new DBParameter("@ShipDateFrom", DbType.DateTime, dateFrom);
      }
      if (dateTo != DateTime.MinValue)
      {
        input[3] = new DBParameter("@ShipDateTo", DbType.DateTime, dateTo);
      }
      if (cusPid != long.MinValue)
      {
        input[4] = new DBParameter("@Customer", DbType.Int64, cusPid);
      }
      string dep = Shared.Utility.SharedObject.UserInfo.Department;
      if (this.btnNew.Visible)
      {
        input[5] = new DBParameter("@Department", DbType.String, dep);
      }
      if (txtSaleOrderCustomerPONo.Text.Trim().Length > 0)
      {
        input[6] = new DBParameter("@SaleOrderCustomerPONo", DbType.String, txtSaleOrderCustomerPONo.Text);
      }
      if (txtItemCode.Text.Trim().Length > 0)
      {
        input[7] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text);
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNListShipmentRequest", input);
      if (dsSource != null)
      {

        dsPLNListShipmentRequest ds = new dsPLNListShipmentRequest();
        ds.Tables["TableInfo"].Merge(dsSource.Tables[0]);
        ds.Tables["TableDetail"].Merge(dsSource.Tables[1]);
        ultDetail.DataSource = ds;
      }
    }
    #endregion LoadData

    #region Event

    /// <summary>
    /// Event Button Delete Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      int k = 0;
      string containerList = string.Empty;
      long containerListPid = long.MinValue;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Delete"].Value.ToString()) == 1)
        {
          containerListPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          containerList = row.Cells["ContainerNo"].Value.ToString();
          k++;
          if (k > 1)
          {
            WindowUtinity.ShowMessageError("ERR0114", "Container No");
            return;
          }
        }
      }
      if (k == 1)
      {
        DataSet ds = (DataSet)this.ultDetail.DataSource;
        if (ds != null)
        {
          DataTable dtSource = ds.Tables[0];
          if (dtSource == null)
          {
            return;
          }

          if (dtSource.Rows.Count == 0)
          {
            return;
          }

          IList listDelete = new ArrayList();
          foreach (DataRow row in dtSource.Rows)
          {
            int select = DBConvert.ParseInt(row["Delete"].ToString());
            int status = DBConvert.ParseInt(row["Status"].ToString());

            if (select == 1)
            {
              if (status != 0)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Status");
                return;
              }

              long pid = DBConvert.ParseLong(row["Pid"].ToString());
              listDelete.Add(pid);
            }
          }

          DialogResult confirm;
          confirm = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0007", "Shipment Request");
          if (confirm == DialogResult.No)
          {
            return;
          }

          foreach (long deletePid in listDelete)
          {
            DBParameter[] input = new DBParameter[] { new DBParameter("@ShipmentRequestPid", DbType.Int64, deletePid) };
            DBParameter[] outputSR = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            DBParameter[] outputSRDetail = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            DataBaseAccess.ExecuteStoreProcedure("spPLNShipmentRequestDetail_DeleteByShipmentRequestPid", input, outputSRDetail);
            if (outputSRDetail[0].Value.ToString().Trim() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
              return;
            }

            DataBaseAccess.ExecuteStoreProcedure("spPLNShipmentRequest_Delete", input, outputSR);
            if (outputSR[0].Value.ToString().Trim() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
              return;
            }
          }

          Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0002");
          this.Search();
        }
      }
    }

    /// <summary>
    /// Event Button Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Event Button Clear Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void btnClear_Click(object sender, EventArgs e)
    {
      txtContName.Text = string.Empty;
      dtpShipDateFrom.Value = DateTime.MinValue;
      dtpShipDateTo.Value = DateTime.MinValue;
      cmbStatus.SelectedIndex = -1;
      cmbCustomer.SelectedIndex = 0;
    }

    /// <summary>
    /// Event Button New Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_06_014 view = new viewPLN_06_014();
      view.status = 0;
      DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "SHIPMENT REQUEST", false, DaiCo.Shared.Utility.ViewState.MainWindow);
    }

    /// <summary>
    /// Event Button Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// FOrmat UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["Status"].Hidden = true;
      e.Layout.Bands[0].Columns["StatusState"].Header.Caption = "Status";
      e.Layout.Bands[0].Columns["Delete"].Header.Caption = "Select";
      e.Layout.Bands[0].Columns["Delete"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["ShipDate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;

      e.Layout.Bands[0].Columns["No"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["No"].MinWidth = 50;
      e.Layout.Bands[0].Columns["ShipDate"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["ShipDate"].MinWidth = 80;
      e.Layout.Bands[0].Columns["Delete"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Delete"].MinWidth = 60;
      e.Layout.Bands[0].Columns["StatusState"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["StatusState"].MinWidth = 200;

      e.Layout.Bands[0].Columns["ContainerNo"].Header.Caption = "Load List";
      e.Layout.Bands[0].Columns["ShipDate"].Header.Caption = "Ship Date";

      if (!this.btnNew.Visible)
      {
        e.Layout.Bands[0].Columns["Delete"].Hidden = true;
      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 3; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      e.Layout.Bands[1].Columns["ContainerListPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Event UltraGrid Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultDetail.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultDetail.Selected.Rows[0].ParentRow == null) ? ultDetail.Selected.Rows[0] : ultDetail.Selected.Rows[0].ParentRow;
      //UltraGridRow row = ultDetail.Selected.Rows[0];
      viewPLN_06_014 view = new viewPLN_06_014();
      long requestPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      //int status = DBConvert.ParseInt(row.Cells["Status"].Value.ToString());
      string commandText = "SELECT [Status] FROM TblPLNContainerList WHERE Pid = " + requestPid;
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      int status = DBConvert.ParseInt(dt.Rows[0]["Status"].ToString());
      view.rRequestPid = requestPid;
      view.status = status;

      if (flagWindow == 0)
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "SHIPMENT REQUEST", false, DaiCo.Shared.Utility.ViewState.MainWindow);
      }
      else if (flagWindow == 1)
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "SHIPMENT REQUEST", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
      }

    }

    /// <summary>
    /// Unlock
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnUnlock_Click(object sender, EventArgs e)
    {
      //int k = 0;
      //string containerList = string.Empty;
      //long containerListPid = long.MinValue;
      //for (int i = 0; i < ultDetail.Rows.Count; i++)
      //{
      //  UltraGridRow row = ultDetail.Rows[i];
      //  if (DBConvert.ParseInt(row.Cells["Delete"].Value.ToString()) == 1)
      //  {
      //    containerListPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      //    containerList = row.Cells["ContainerNo"].Value.ToString();
      //    k++;
      //    if (k > 1)
      //    {
      //      WindowUtinity.ShowMessageError("ERR0114", "Container No");
      //      return;
      //    }
      //  }
      //}

      //if (k == 1)
      //{
      //  int status = int.MinValue;
      //  string commandText = "SELECT [Status] FROM TblPLNContainerList WHERE Pid = " + containerListPid;
      //  DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      //  if (dt != null && dt.Rows.Count > 0)
      //  {
      //    status = DBConvert.ParseInt(dt.Rows[0]["Status"].ToString());
      //    if (status == 2 || status == 3)
      //    {
      //      string storeName = "spPLNContainerListUnlockStatus_Update";
      //      DBParameter[] input = new DBParameter[1];
      //      input[0] = new DBParameter("@Pid", DbType.Int64, containerListPid);

      //      DBParameter[] output = new DBParameter[1];
      //      output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      //      DataBaseAccess.ExecuteStoreProcedure(storeName, input, output);
      //      long success = DBConvert.ParseLong(output[0].Value.ToString());
      //      if (success == 1)
      //      {
      //        WindowUtinity.ShowMessageSuccess("MSG0023");
      //        return;
      //      }
      //      else
      //      {
      //        WindowUtinity.ShowMessageError("ERR0034", containerList);
      //        return;
      //      }
      //    }
      //    else
      //    {
      //      WindowUtinity.ShowMessageError("ERR0113", containerList);
      //      return;
      //    }
      //  }
      //  this.Search();
      //}
    }

    /// <summary>
    /// Unlock
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtContName_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    /// <summary>
    /// Unlock
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cmbStatus_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dtpShipDateFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dtpShipDateTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cmbCustomer_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    #endregion Event
  }
}
