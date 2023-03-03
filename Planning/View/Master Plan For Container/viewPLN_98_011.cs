using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;

namespace DaiCo.Planning
{
  public partial class viewPLN_98_011 : MainUserControl
  {
    #region Field

    public string itemCode = string.Empty;
    public int revision = int.MinValue;
    public long soPid = long.MinValue;
    public long customerPid = long.MinValue;
    public long qty = long.MinValue; // Lay Tu Remain Qty
    DataTable dtDetail = new DataTable();
    public bool flag = false;
    #endregion Field

    #region Init
    public viewPLN_98_011()
    {
      InitializeComponent();
    }

    private void viewPLN_98_011_Load(object sender, EventArgs e)
    {
      this.LoadData();
      this.LoadContainer(customerPid);
    }

    #endregion Init

    #region Function

    private void LoadData()
    {
      //ItemCode & revision
      labItemCode.Text = itemCode;
      labRev.Text = revision.ToString();
      if (qty != long.MinValue)
      {
        txtAddQty.Text = qty.ToString();
      }
      else
      {
        txtAddQty.Text = "";
      }

      // Load Luoi
      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@ItemCode", DbType.String, itemCode);
      input[1] = new DBParameter("@Revision", DbType.Int32, revision);
      DataTable dtMain = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMasterPlanLoadItemForAddContainer", input);
      if (dtMain != null && dtMain.Rows.Count > 0)
      {
        ultData.DataSource = dtMain;
      }
    }

    private void LoadContainer(long customerPid)
    {
      string commandText = string.Empty;
      commandText = string.Format(@"	SELECT CON.ContainerNo, CONVERT(VARCHAR, CON.ShipDate, 103) LoadingDate, CL.Pid ContainerListPid,
		                                     SUM(ISNULL(CLD.Qty, 0)) LoadingQty, ROUND(SUM(ISNULL(CLD.Qty, 0) * ISNULL(IIF.CBM, 0)), 3) LoadingCBM,
		                                     CL.CustomerPid
	                                    FROM TblPLNSHPContainer CON
		                                    INNER JOIN TblPLNSHPContainerDetails COD ON CON.Pid = COD.ContainerPid
		                                    INNER JOIN TblPLNContainerList CL ON COD.LoadingListPid = CL.Pid
		                                    LEFT JOIN TblPLNContainerListDetail CLD ON CL.Pid = CLD.ContainerListPid
		                                    LEFT JOIN TblBOMItemInfo IIF ON CLD.ItemCode = IIF.ItemCode
									                                    AND CLD.Revision = IIF.Revision
	                                    WHERE CON.Confirm <> 3 AND CL.CustomerPid = {0}
	                                    GROUP BY CON.ContainerNo, CON.ShipDate, CL.Pid, CL.CustomerPid
	                                    ORDER BY CON.ShipDate DESC", customerPid);

      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultContainer.DataSource = dtItem;
      ultContainer.DataBind();
      ultContainer.ValueMember = "ContainerListPid";
      ultContainer.DisplayMember = "ContainerNo";
      ultContainer.DisplayLayout.Bands[0].Columns["ContainerListPid"].Hidden = true;
      ultContainer.DisplayLayout.Bands[0].Columns["CustomerPid"].Hidden = true;

      dtDetail = dtItem;
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
    #endregion Function

    #region Event

    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check Qty
      if (DBConvert.ParseInt(txtAddQty.Text) == int.MinValue || DBConvert.ParseInt(txtAddQty.Text) > qty)
      {
        WindowUtinity.ShowMessageError("ERR0001", "0 < AddQty <= " + qty);
        return;
      }

      //Check Container
      if (ultContainer.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Container");
        return;
      }
      // End

      if (DBConvert.ParseLong(ultContainer.Value.ToString()) != long.MinValue &&
          DBConvert.ParseInt(txtAddQty.Text) != int.MinValue)
      {
        long containerListPid = DBConvert.ParseLong(ultContainer.Value.ToString());
        int qtyAdd = DBConvert.ParseInt(txtAddQty.Text);

        DBParameter[] inputParam = new DBParameter[5];
        inputParam[0] = new DBParameter("@ContainerListPid", DbType.Int64, containerListPid);
        inputParam[1] = new DBParameter("@SOPid", DbType.Int64, soPid);
        inputParam[2] = new DBParameter("@ItemCode", DbType.String, itemCode);
        inputParam[3] = new DBParameter("@Revision", DbType.Int32, revision);
        inputParam[4] = new DBParameter("@Qty", DbType.Int32, qtyAdd);

        DataBaseAccess.ExecuteStoreProcedure("spPLNMasterPlanSONotYetPutOnContainerList_Insert", inputParam);
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.flag = true;
      this.CloseTab();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["LoadingQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["LoadingCBM"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyRepair"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["USStock"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AVEUS6M"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AVEUS12M"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }

    private void btnAddContainer_Click(object sender, EventArgs e)
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

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.flag = false;
      this.CloseTab();
    }
    #endregion Event
  }
}
