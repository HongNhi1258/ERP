/*
 * Author :Nguyen Phuoc Vinh
 * CreateDate : 22/11/2010
 * Description :Search list container
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_06_007 : MainUserControl
  {
    public viewPLN_06_007()
    {
      InitializeComponent();
    }

    private void viewPLN_06_007_Load(object sender, EventArgs e)
    {
      this.ultDatetimeShipTo.Value = DateTime.Today;
      this.ultDatetimeShipFrom.Value = DateTime.Today.AddMonths(-1);
      this.LoadShipType();
      this.LoadLoadingList();
      this.btnSearch_Click(null, null);
    }

    private void LoadShipType()
    {
      string commandText = "SELECT Code, Value, [Description] FROM TblBOMCodeMaster WHERE [Group] = 11001";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBShipType.DataSource = dt;
      ultCBShipType.DisplayMember = "Description";
      ultCBShipType.ValueMember = "Code";
      ultCBShipType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCBShipType.DisplayLayout.Bands[0].Columns["Description"].Width = 300;
    }

    private void LoadLoadingList()
    {
      string commandText = string.Empty;
      commandText = string.Format(@"  SELECT CL.Pid, CL.ContainerNo LoadingList, CON.ContainerNo, CONVERT(varchar, CON.ShipDate, 103) ShipDate
                                      FROM TblPLNSHPContainer CON
	                                      INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid
	                                      INNER JOIN TblPLNContainerList CL ON CL.Pid = COD.LoadingListPid
                                      ORDER BY CON.ShipDate DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBLoadingList.DataSource = dt;
      ultCBLoadingList.DisplayMember = "LoadingList";
      ultCBLoadingList.ValueMember = "Pid";
      ultCBLoadingList.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Init Datatgrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultContainerList_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultContainerList);

      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 40;
      e.Layout.Bands[0].Columns["No"].MinWidth = 40;

      e.Layout.Bands[0].Columns["No"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ContainerNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ContainerType"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CBM"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Vehicle"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ContainerNumber"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ShipDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["EmpName"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Confirm"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["HardwarePriority"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["HardwarePriority"].Header.Caption = "HW Priority";
      e.Layout.Bands[0].Columns["IsDelete"].Header.Caption = "Delete";
      e.Layout.Bands[0].Columns["ContainerNo"].Header.Caption = "Container ID";
      e.Layout.Bands[0].Columns["ContainerType"].Header.Caption = "Shipping Type";
      e.Layout.Bands[0].Columns["EmpName"].Header.Caption = "Employee Name";
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["CreateDate"].Format = Shared.Utility.ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateDate"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["IsDelete"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["IsDelete"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["IsDelete"].MinWidth = 80;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        //e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {

          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }

      }
    }

    /// <summary>
    /// DoubleClick to open container
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultContainerList_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultContainerList.Selected.Rows.Count > 0)
      {
        viewPLN_06_008 view = new viewPLN_06_008();
        long containerPid = DBConvert.ParseLong(ultContainerList.Selected.Rows[0].Cells["Pid"].Value.ToString());
        view.ContainerPid = containerPid;
        Shared.Utility.WindowUtinity.ShowView(view, "Container Information", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
        btnSearch_Click(null, null);
      }
    }

    /// <summary>
    /// New Container
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_06_008 view = new viewPLN_06_008();
      Shared.Utility.WindowUtinity.ShowView(view, "Container Information", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
      btnSearch_Click(null, null);
    }

    /// <summary>
    /// Search container base on condition search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      DBParameter[] param = new DBParameter[6];
      if (txtContainerId.Text.Trim().Length > 0)
      {
        param[1] = new DBParameter("@ConNo", DbType.AnsiString, 128, "%" + txtContainerId.Text.Trim() + "%");
      }
      if (ultCBShipType.Value != null)
      {
        param[2] = new DBParameter("@ShipType", DbType.Int32, DBConvert.ParseInt(ultCBShipType.Value.ToString()));
      }
      if (ultDatetimeShipFrom.Value != null)
      {
        param[3] = new DBParameter("@DateFrom", DbType.DateTime, (DateTime)ultDatetimeShipFrom.Value);
      }
      if (ultDatetimeShipTo.Value != null)
      {
        param[4] = new DBParameter("@DateTo", DbType.DateTime, (DateTime)ultDatetimeShipTo.Value);
      }

      if (ultCBLoadingList.Value != null && DBConvert.ParseLong(ultCBLoadingList.Value.ToString()) != long.MinValue)
      {
        param[5] = new DBParameter("@LoadingList", DbType.Int64, DBConvert.ParseLong(ultCBLoadingList.Value.ToString()));
      }

      DataTable dtContainerList = DataBaseAccess.SearchStoreProcedureDataTable("spPLNContainer_Search", param);
      ultContainerList.DataSource = dtContainerList;

      for (int i = 0; i < ultContainerList.Rows.Count; i++)
      {
        UltraGridRow row = ultContainerList.Rows[i];
        if (string.Compare("Plan ShipDate", row.Cells["Confirm"].Value.ToString(), true) == 0)
        {
          row.Cells["IsDelete"].Activation = Activation.AllowEdit;
        }
        else
        {
          row.Cells["IsDelete"].Activation = Activation.ActivateOnly;
        }
      }
    }

    /// <summary>
    /// Buton Close Click (Qui, 6/12/2010 15:00)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtContainerId.Text = string.Empty;
      ultCBShipType.Text = string.Empty;
      ultDatetimeShipFrom.Value = null;
      ultDatetimeShipTo.Value = null;
      ultCBLoadingList.Value = DBNull.Value;
    }

    private void txtContainerId_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultCBShipType_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultDatetimeShipFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultDatetimeShipTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void btnSwap_Click(object sender, EventArgs e)
    {
      viewPLN_06_009 view = new viewPLN_06_009();
      Shared.Utility.WindowUtinity.ShowView(view, "Swap SO After Container Was Deducted", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      for (int i = 0; i < ultContainerList.Rows.Count; i++)
      {
        UltraGridRow row = ultContainerList.Rows[i];
        if (DBConvert.ParseInt(row.Cells["IsDelete"].Value.ToString()) == 1 &&
          string.Compare("Plan ShipDate", row.Cells["Confirm"].Value.ToString(), true) == 0)
        {
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@ContainerPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          DBParameter[] output = new DBParameter[1];

          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure("spPLNContainer_Delete", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0093", row.Cells["ContainerNo"].Value.ToString());
            return;
          }
        }
      }

      WindowUtinity.ShowMessageSuccess("MSG0002");
      this.btnSearch_Click(sender, e);
    }
  }
}
