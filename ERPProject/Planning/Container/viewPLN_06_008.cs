/*
 * Author :Nguyen Phuoc Vinh
 * CreateDate : 22/11/2010
 * Description :Insert, Update Container Information
 */
using DaiCo.Application;
using DaiCo.ERPProject.Planning.Reports;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.DataSetSource.Planning;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_06_008 : DaiCo.Shared.MainUserControl
  {
    public long ContainerPid;

    private IList listDeletedPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private int status = 0;
    public viewPLN_06_008()
    {
      InitializeComponent();
      this.ContainerPid = long.MinValue;
    }

    /// <summary>
    /// Load form, inid data inform
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_06_008_Load(object sender, EventArgs e)
    {
      this.loadTypeOfContainer();
      ultOriginalShipDate.Value = DBNull.Value;

      // Load Distributor
      this.LoadDistributor();

      string commandText = string.Empty;
      commandText += " SELECT CON.Confirm, COUNT(*) ";
      commandText += " FROM TblPLNSHPContainer CON ";
      commandText += " 	INNER JOIN TblWHFOutStore OS ON CON.Pid = OS.ContainerPid ";
      commandText += " WHERE CON.Pid =  " + this.ContainerPid;
      commandText += " GROUP BY CON.Confirm";
      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheck != null && dtCheck.Rows.Count == 1)
      {
        if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0
              && DBConvert.ParseInt(dtCheck.Rows[0][1].ToString()) == 0)
        {
          this.status = 0;
        }
        else if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0
              && DBConvert.ParseInt(dtCheck.Rows[0][1].ToString()) != 0)
        {
          this.status = 1;
        }
        else if (DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 3)
        {
          this.status = 2;
        }
      }

      DBParameter[] param = new DBParameter[1];
      param[0] = new DBParameter("@ContainerPid", DbType.Int64, this.ContainerPid);

      DataSet dsContainerInfo = DataBaseAccess.SearchStoreProcedure("spPLNGetContainerInformation", param);

      drpLoadingList.DataSource = dsContainerInfo.Tables[0];
      drpLoadingList.ValueMember = "Pid";
      drpLoadingList.DisplayMember = "ContainerNo";
      drpLoadingList.DisplayLayout.Bands[0].Columns["Name"].Width = 300;
      drpLoadingList.DisplayLayout.Bands[0].Columns["ContainerNo"].Width = 300;
      drpLoadingList.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;

      DataTable containerInfo = dsContainerInfo.Tables[1];
      if (containerInfo.Rows.Count > 0)
      {
        this.ContainerPid = DBConvert.ParseLong(containerInfo.Rows[0]["Pid"].ToString());
        this.txtContainerID.Text = DBConvert.ParseString(containerInfo.Rows[0]["ContainerNo"]);
        this.txtDescription.Text = DBConvert.ParseString(containerInfo.Rows[0]["Desccription"]);
        this.txtContainerNumber.Text = containerInfo.Rows[0]["ContainerNumber"].ToString();
        this.txtVehicle.Text = containerInfo.Rows[0]["Vehicle"].ToString();
        this.ultdtShipDate.Value = containerInfo.Rows[0]["ShipDate"];
        this.txtHardwarePri.Text = containerInfo.Rows[0]["HardwarePriority"].ToString();
        this.ultOriginalShipDate.Value = containerInfo.Rows[0]["OriginalShipDate"].ToString();
        if (DBConvert.ParseInt(containerInfo.Rows[0]["Type"].ToString()) != int.MinValue)
        {
          this.ultCBContainerType.Value = DBConvert.ParseInt(containerInfo.Rows[0]["Type"].ToString());
        }

        if (DBConvert.ParseInt(containerInfo.Rows[0]["Distributor"].ToString()) != int.MinValue)
        {
          this.ultCBDistributor.Value = DBConvert.ParseInt(containerInfo.Rows[0]["Distributor"].ToString());
        }

        if (DBConvert.ParseInt(containerInfo.Rows[0]["ShipmentExFactory"].ToString()) == 1)
        {
          this.chkShipment.Checked = true;
        }
        else
        {
          this.chkShipment.Checked = false;
        }

        DataTable dtSource = dsContainerInfo.Tables[2];
        ultContainerDetails.DataSource = dtSource;
      }

      if (this.status == 1 || this.status == 2)
      {
        if (this.status == 2)
        {
          this.txtContainerNumber.ReadOnly = true;
          this.txtVehicle.ReadOnly = true;
          this.btnSave.Enabled = false;
          this.txtDescription.ReadOnly = true;
          this.ultCBContainerType.ReadOnly = true;
          this.ultCBDistributor.ReadOnly = true;
        }
        this.txtContainerID.ReadOnly = true;
        this.ultdtShipDate.ReadOnly = true;
      }

      if (SharedObject.UserInfo.Department == "WHD")
      {
        ultContainerDetails.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        ultContainerDetails.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      }
    }

    private void loadTypeOfContainer()
    {
      string commandText = "SELECT Code, Value, [Description] FROM TblBOMCodeMaster WHERE [Group] = 11001";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBContainerType.DataSource = dt;
      ultCBContainerType.DisplayMember = "Description";
      ultCBContainerType.ValueMember = "Code";
      ultCBContainerType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCBContainerType.DisplayLayout.Bands[0].Columns["Description"].Width = 300;
      ultCBContainerType.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Distributor
    /// </summary>
    private void LoadDistributor()
    {
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 99003 ORDER BY Sort";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBDistributor.DataSource = dt;
      ultCBDistributor.DisplayMember = "Value";
      ultCBDistributor.ValueMember = "Code";
      ultCBDistributor.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCBDistributor.DisplayLayout.Bands[0].Columns["Value"].Width = 300;
      ultCBDistributor.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Init datagird
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultContainerDetails_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.SetPropertiesUltraGrid(ultContainerDetails);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LoadingListPid"].ValueList = this.drpLoadingList;
      e.Layout.Bands[0].Columns["LoadingListPid"].Header.Caption = "Container List";

    }

    void drpLoadingList_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["ContainerNo"].Header.Caption = "Loading Shipment Request";
      e.Layout.Bands[0].Columns["ContainerNo"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["ContainerNo"].MinWidth = 200;
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Customer Name";
      e.Layout.Bands[0].Columns["ShipDate"].Header.Caption = "Ship Date";
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Override function save before close
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      btnSave_Click(null, null);
    }

    /// <summary>
    /// When click button save, do save master and detail container
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (ultdtShipDate.Value != null)
      {
        string ccommand = "SELECT Holiday FROM VHRDWorkingDay WHERE Holiday = -1 AND CONVERT(varchar, WDate, 103) = '" + DBConvert.ParseString(DBConvert.ParseDateTime(ultdtShipDate.Value), ConstantClass.FORMAT_DATETIME) + "' ";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(ccommand);
        if (dt.Rows.Count > 0)
        {
          if (WindowUtinity.ShowMessageConfirm("WRN0034", "Ship Date") == DialogResult.No)
          {
            return;
          }
        }
      }
      if (txtHardwarePri.Text.Trim().Length > 0 && DBConvert.ParseInt(txtHardwarePri.Text.Trim()) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Hardware Priority");
        return;
      }
      bool result = true;
      DataTable dtContainerDetail = (DataTable)ultContainerDetails.DataSource;
      if (txtContainerID.Text.Trim().Length > 0)
      {
        foreach (long detailPid in this.listDeletedPid)
        {
          DBParameter[] inputParamDelete = new DBParameter[1];
          inputParamDelete[0] = new DBParameter("@Pid", DbType.Int64, detailPid);
          DBParameter[] OutputParamDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateSHPContainerDetails_Deleted", inputParamDelete, OutputParamDelete);
          long outputParamDelete = DBConvert.ParseLong(OutputParamDelete[0].Value.ToString());
          if (outputParamDelete == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
            return;
          }
        }

        this.ContainerPid = SaveData();

        if (dtContainerDetail != null && this.ContainerPid > 0 && dtContainerDetail.Rows.Count > 0)
        {
          DBParameter[] param = new DBParameter[2];
          foreach (DataRow row in dtContainerDetail.Rows)
          {
            if (row.RowState == DataRowState.Added)
            {
              param[0] = new DBParameter("@LoadingListPid", DbType.Int64, row[1]);
              param[1] = new DBParameter("@ContainerPid", DbType.Int64, this.ContainerPid);
              result = DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateSHPContainerDetails", param);
            }
            else if (row.RowState == DataRowState.Modified)
            {
              param[0] = new DBParameter("@Pid", DbType.Int64, row[0]);
              param[1] = new DBParameter("@LoadingListPid", DbType.Int64, row[1]);
              result = DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateSHPContainerDetails", param);
            }
          }
          if (result)
          {
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
            this.NeedToSave = false;
          }
          else
            Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
        }
        else
        {
          if (ContainerPid != long.MinValue)
          {
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
            this.NeedToSave = false;
          }
        }
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageWarning("WRN0001");
      }

      DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanContainerPriorityRefeshData_Insert");
      this.viewPLN_06_008_Load(null, null);
    }

    /// <summary>
    /// Save Container information
    /// </summary>
    /// <returns></returns>
    private long SaveData()
    {
      //check Valid
      DBParameter[] param = new DBParameter[13];
      if (this.ContainerPid > 0)
      {
        param[0] = new DBParameter("@Pid", DbType.Int64, this.ContainerPid);
      }
      string containerNo = txtContainerID.Text.Trim();
      if (containerNo.Length > 0)
      {
        string commandText = "SELECT COUNT(*) FROM TblPLNSHPContainer WHERE ContainerNo = '" + containerNo + "' AND Pid <> " + this.ContainerPid;
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandText);
        if (obj != null && (int)obj > 0)
        {
          WindowUtinity.ShowMessageError("ERR0006", "Container No");
          return long.MinValue;
        }
      }

      if (ultCBContainerType.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Container Type");
        return long.MinValue;
      }

      if (ultCBDistributor.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Distributor");
        return long.MinValue;
      }
      param[1] = new DBParameter("@ContainerNo", DbType.AnsiString, 50, containerNo);
      param[2] = new DBParameter("@Desccription", DbType.AnsiString, 512, txtDescription.Text.Trim());
      param[3] = new DBParameter("@EmployeePid", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
      param[4] = new DBParameter("@PLNConfirmed", DbType.Int32, 0);
      param[5] = new DBParameter("@ContainerType", DbType.Int32, DBConvert.ParseInt(ultCBContainerType.Value.ToString()));

      if (ultdtShipDate.Value != null)
      {
        param[6] = new DBParameter("@ShipDate", DbType.DateTime, (DateTime)ultdtShipDate.Value);
      }
      param[7] = new DBParameter("@Vehicle", DbType.AnsiString, 50, txtVehicle.Text.Trim());
      param[8] = new DBParameter("@ContainerNumber", DbType.AnsiString, 50, txtContainerNumber.Text.Trim());
      param[9] = new DBParameter("@Distributor", DbType.Int32, DBConvert.ParseInt(ultCBDistributor.Value.ToString()));
      if (DBConvert.ParseInt(txtHardwarePri.Text.Trim()) != int.MinValue)
      {
        param[10] = new DBParameter("@HardwarePriority", DbType.Int32, DBConvert.ParseInt(txtHardwarePri.Text.Trim()));
      }
      if (ultOriginalShipDate.Value != null)
      {
        param[11] = new DBParameter("@OriginalShipDate", DbType.DateTime, (DateTime)ultOriginalShipDate.Value);
      }

      if (chkShipment.Checked)
      {
        param[12] = new DBParameter("@ShipmentExFactory", DbType.Int32, 1);
      }
      else
      {
        param[12] = new DBParameter("@ShipmentExFactory", DbType.Int32, 0);
      }
      DBParameter[] paramOUT = new DBParameter[1];
      paramOUT[0] = new DBParameter("@Result", DbType.Int64, 0);

      DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateSHPContainer", param, paramOUT);

      return DBConvert.ParseLong(paramOUT[0].Value.ToString());
    }

    private void ultContainerDetails_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeletedPid.Add(pid);
      }
    }

    private void ultContainerDetails_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
    {
      if (SharedObject.UserInfo.Department == "WHD")
      {
        return;
      }
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    private void ultContainerDetails_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      int count = 0;
      if (colName == "containerno")
      {
        string containerNo = e.Cell.Row.Cells["ContainerNo"].Text;
        if (containerNo.Length == 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Shipment Request");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }
        for (int i = 0; i < drpLoadingList.Rows.Count; i++)
        {
          if (drpLoadingList.Rows[i].Cells["ContainerNo"].Text == e.Cell.Row.Cells["ContainerNo"].Text)
          {
            count = 1;
            break;
          }
        }
        if (count == 0)
        {
          string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Shipment Request");
          WindowUtinity.ShowMessageErrorFromText(message);
          e.Cancel = true;
          return;
        }

        count = 0;
        for (int i = 0; i < ultContainerDetails.Rows.Count; i++)
        {
          if (e.Cell.Row.Cells["ContainerNo"].Text == ultContainerDetails.Rows[i].Cells["ContainerNo"].Text)
          {
            count++;
            if (count == 2)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Shipment Request");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
              break;
            }
          }
        }
      }
    }

    private void ultContainerDetails_BeforeRowUpdate(object sender, CancelableRowEventArgs e)
    {
      int count = 0;
      string containerList = e.Row.Cells["LoadingListPid"].Text;
      if (containerList.Length == 0)
      {
        string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Shipment Request");
        WindowUtinity.ShowMessageErrorFromText(message);
        e.Cancel = true;
        return;
      }
      for (int i = 0; i < drpLoadingList.Rows.Count; i++)
      {
        if (DBConvert.ParseLong(drpLoadingList.Rows[i].Cells["Pid"].Text) == DBConvert.ParseLong(e.Row.Cells["LoadingListPid"].Value.ToString()))
        {
          count = 1;
          break;
        }
      }
      if (count == 0)
      {
        string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Shipment Request");
        WindowUtinity.ShowMessageErrorFromText(message);
        e.Cancel = true;
        return;
      }

      count = 0;
      for (int i = 0; i < ultContainerDetails.Rows.Count; i++)
      {
        if (e.Row.Cells["LoadingListPid"].Text == ultContainerDetails.Rows[i].Cells["LoadingListPid"].Text)
        {
          count++;
          if (count == 2)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Shipment Request");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
            break;
          }
        }
      }
    }

    private void btnDeliveryContainer_Click(object sender, EventArgs e)
    {
      if (this.ContainerPid != long.MinValue)
      {
        string storeName = "spPLNDeliveryItemOfContainer";
        DBParameter[] param = new DBParameter[1];
        param[0] = new DBParameter("@ContainerPid", DbType.Int64, this.ContainerPid);

        DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable(storeName, param);
        if (dt != null && dt.Rows.Count > 0)
        {
          dsPLNDeliveryContainer dsSource = new dsPLNDeliveryContainer();
          dsSource.Tables[0].Merge(dt);

          int totalQty = 0;
          int totalBoxQty = 0;
          double totalCBM = 0;
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            totalQty += DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString());
            totalBoxQty += DBConvert.ParseInt(dt.Rows[i]["BoxQty"].ToString());
            totalCBM += DBConvert.ParseDouble(dt.Rows[i]["TotalCBM"].ToString());
          }

          DaiCo.Shared.View_Report report = null;
          cptDeliveryNoteContainer cpt = new cptDeliveryNoteContainer();
          cpt.SetDataSource(dsSource);

          string RefNo = dt.Rows[0]["ContainerPid"].ToString();
          string vehicle = dt.Rows[0]["Vehicle"].ToString();
          string containerNumber = dt.Rows[0]["ContainerNumber"].ToString();

          cpt.SetParameterValue("RefNo", RefNo);
          cpt.SetParameterValue("Vehicle", vehicle);
          cpt.SetParameterValue("Container", containerNumber);
          cpt.SetParameterValue("TotalQty", totalQty);
          cpt.SetParameterValue("TotalBoxQty", totalBoxQty);
          cpt.SetParameterValue("TotalCBM", totalCBM);

          report = new DaiCo.Shared.View_Report(cpt);
          report.IsShowGroupTree = false;
          report.ShowReport(Shared.Utility.ViewState.Window, FormWindowState.Maximized);
        }
      }
    }

    private void btnChecking_Click(object sender, EventArgs e)
    {
      viewPLN_06_094 view = new viewPLN_06_094();
      view.containerPid = this.ContainerPid;
      Shared.Utility.WindowUtinity.ShowView(view, "CHECKING CONTAINER BETWEEN PLANNING & WAREHOUSE", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void btnWHCBM_Click(object sender, EventArgs e)
    {
      string commandText = string.Empty;
      commandText += " SELECT COUNT(*) ";
      commandText += " FROM TblPLNSHPContainer CON ";
      commandText += " 	INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid ";
      commandText += " 	INNER JOIN TblPLNContainerList CL ON COD.LoadingListPid = CL.Pid ";
      commandText += " WHERE CL.[Status] < 1 ";
      commandText += " 	 AND CON.Pid = " + this.ContainerPid;

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count != 0)
      {
        if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) > 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Planning chua confirmed het cac Loading List");
          return;
        }
      }
      else
      {
        return;
      }

      viewPLN_06_022 view = new viewPLN_06_022();
      view.containerPid = this.ContainerPid;
      Shared.Utility.WindowUtinity.ShowView(view, "WH CONFIRM CBM", false, Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
    }

    private void ultdtShipDate_ValueChanged(object sender, EventArgs e)
    {

    }
  }
}

