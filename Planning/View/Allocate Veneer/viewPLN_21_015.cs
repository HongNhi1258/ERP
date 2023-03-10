/*
  Author      : 
  Date        : 12/10/2013
  Description : Add Multi Allocate Special
  Standard From : viewGNR_90_003
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_015 : MainUserControl
  {
    #region Field
    public DataTable dtDetail = new DataTable();
    #endregion Field

    #region Init

    /// <summary>
    /// viewPLN_21_015
    /// </summary>
    public viewPLN_21_015()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load
    /// </summary>
    private void viewPLN_21_015_Load(object sender, EventArgs e)
    {
      // Init Data Control
      radAllocate.Checked = true;

      this.LoadLocation();
      this.LoadWO();
      this.LoadNewWO();
      this.LoadMainMaterialCode();
      this.dtDetail = this.CreateDataTable();
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadLocation()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_Position LocationPid, Name Location ";
      commandText += " FROM VWHDLocation ";

      DataTable dtLocation = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBLocation.DataSource = dtLocation;
      ultCBLocation.ValueMember = "LocationPid";
      ultCBLocation.DisplayMember = "Location";
      ultCBLocation.DisplayLayout.Bands[0].Columns["LocationPid"].Hidden = true;
      ultCBLocation.DataBind();
    }

    /// <summary>
    /// Load New WO
    /// </summary>
    private void LoadNewWO()
    {
      string commandText = string.Empty;
      commandText += " SELECT WOPid WO ";
      commandText += " FROM VPLNWorkOrderCarcassList ";

      DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBNewWO.DataSource = dtWO;
      ultCBNewWO.DataBind();
      ultCBNewWO.ValueMember = "WO";
    }

    /// <summary>
    /// Load New Carcass
    /// </summary>
    private void LoadNewCarcassCode(int wo)
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT CarcassCode ";
      commandText += " FROM VPLNWorkOrderCarcassList ";
      commandText += " WHERE WOPid = " + wo;

      DataTable dtCarcassCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBNewCarcassCode.DataSource = dtCarcassCode;
      ultCBNewCarcassCode.DataBind();
      ultCBNewCarcassCode.ValueMember = "CarcassCode";
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadWO()
    {
      string commandText = string.Empty;
      commandText = string.Format(@"
                                      SELECT DISTINCT WOC.WOPid WO
                                      FROM VPLNWorkOrderCarcassList WOC
                                      INNER JOIN 
                                      (
	                                      SELECT DISTINCT WorkOrderPid, CarcassCode
	                                      FROM TblPLNWorkOrderConfirmedDetails
	                                      WHERE ISNULL(CloseCOM1, 0) = 0
		                                      AND ISNULL(CloseAll, 0) = 0
                                      )CLO ON WOC.WOPid = CLO.WorkOrderPid
	                                      AND WOC.CarcassCode = CLO.CarcassCode
                                      ORDER BY WOC.WOPid
                                  ");

      DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBWO.DataSource = dtWO;
      ultCBWO.DataBind();
      ultCBWO.ValueMember = "WO";
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadCarcassCode(int wo)
    {
      string commandText = string.Empty;
      commandText = string.Format(@"SELECT DISTINCT WOC.CarcassCode
                                    FROM VPLNWorkOrderCarcassList WOC
                                    INNER JOIN 
                                    (
	                                    SELECT DISTINCT WorkOrderPid, CarcassCode
	                                    FROM TblPLNWorkOrderConfirmedDetails
	                                    WHERE ISNULL(CloseCOM1, 0) = 0
		                                    AND ISNULL(CloseAll, 0) = 0
                                    )CLO ON WOC.WOPid = CLO.WorkOrderPid
	                                    AND WOC.CarcassCode = CLO.CarcassCode
                                    WHERE WOC.WOPid = {0}
                                    ORDER BY WOC.CarcassCode
                                  ", wo);

      //commandText += " SELECT DISTINCT CarcassCode ";
      //commandText += " FROM VPLNWorkOrderCarcassList ";
      //commandText += " WHERE WOPid = " + wo;

      DataTable dtCarcassCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBCarcassCode.DataSource = dtCarcassCode;
      ultCBCarcassCode.DataBind();
      ultCBCarcassCode.ValueMember = "CarcassCode";
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadMainMaterialCode()
    {
      string commandText = string.Empty;
      commandText += " SELECT MaterialCode, MaterialNameEn, MaterialNameVn, Unit  ";
      commandText += " FROM VBOMMaterials  ";
      commandText += " WHERE Warehouse = 2 ";

      DataTable dtCarcassCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBMainMaterialCode.DataSource = dtCarcassCode;
      ultCBMainMaterialCode.DisplayMember = "MaterialCode";
      ultCBMainMaterialCode.DataBind();
      ultCBMainMaterialCode.ValueMember = "MaterialCode";
    }

    #endregion Init

    #region Function

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      this.btnSearch.Enabled = false;
      string storeName = string.Empty;
      storeName = "spWHDVeneerAddMultiAllocateSpecial_Select";

      DBParameter[] param = new DBParameter[9];
      if (txtMaterial.Text.Trim().Length > 0)
      {
        param[0] = new DBParameter("@Material", DbType.String, txtMaterial.Text);
      }
      if (ultCBLocation.Text.Trim().Length > 0 && ultCBLocation.Value != null)
      {
        param[1] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(ultCBLocation.Value.ToString()));
      }
      if (txtLengthFrom.Text.Trim().Length > 0 && DBConvert.ParseDouble(txtLengthFrom.Text) > 0)
      {
        param[2] = new DBParameter("@LengthFrom", DbType.Double, DBConvert.ParseDouble(txtLengthFrom.Text));
      }
      if (txtLengthTo.Text.Trim().Length > 0 && DBConvert.ParseDouble(txtLengthTo.Text) > 0)
      {
        param[3] = new DBParameter("@LengthTo", DbType.Double, DBConvert.ParseDouble(txtLengthTo.Text));
      }
      if (txtWidthFrom.Text.Trim().Length > 0 && DBConvert.ParseDouble(txtWidthFrom.Text) > 0)
      {
        param[4] = new DBParameter("@WidthFrom", DbType.Double, DBConvert.ParseDouble(txtWidthFrom.Text));
      }
      if (txtWidthTo.Text.Trim().Length > 0 && DBConvert.ParseDouble(txtWidthTo.Text) > 0)
      {
        param[5] = new DBParameter("@WidthTo", DbType.Double, DBConvert.ParseDouble(txtWidthTo.Text));
      }
      if (ultCBWO.Value != null)
      {
        param[6] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }
      if (ultCBCarcassCode.Value != null)
      {
        param[7] = new DBParameter("@CarcassCode", DbType.String, ultCBCarcassCode.Value.ToString());
      }
      param[8] = new DBParameter("@Type", DbType.Int32, (radAllocate.Checked) ? 1 : 2);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, param);
      if (dtSource != null)
      {
        ultData.DataSource = dtSource;
      }
      DataTable dtGrid = dtSource.Clone();
      if (dtDetail.Rows.Count > 0)
      {
        for (int i = 0; i < dtSource.Rows.Count; i++)
        {
          DataRow row = dtSource.Rows[i];
          DataRow[] foundRows = dtDetail.Select("LotNoId = '" + row["LotNoId"].ToString() + "'");
          if (foundRows.Length == 0)
          {
            DataRow rowGrid = dtGrid.NewRow();
            rowGrid["Selected"] = 0;
            if (DBConvert.ParseInt(row["NewWO"].ToString()) > 0)
            {
              rowGrid["NewWO"] = DBConvert.ParseInt(row["NewWO"].ToString());
            }
            rowGrid["NewCarcassCode"] = row["NewCarcassCode"].ToString();
            rowGrid["MainMaterialCode"] = row["MainMaterialCode"].ToString();
            rowGrid["LotNoId"] = row["LotNoId"].ToString();
            rowGrid["MaterialCode"] = row["MaterialCode"].ToString();
            rowGrid["MaterialNameVn"] = row["MaterialNameVn"].ToString();
            rowGrid["Balance"] = row["Balance"].ToString();
            if (DBConvert.ParseDouble(row["Allocated"].ToString()) > 0)
            {
              rowGrid["Allocated"] = DBConvert.ParseDouble(row["Allocated"].ToString());
            }
            rowGrid["Remain"] = DBConvert.ParseDouble(row["Remain"].ToString());
            rowGrid["Length"] = DBConvert.ParseDouble(row["Length"].ToString());
            rowGrid["Width"] = DBConvert.ParseDouble(row["Width"].ToString());
            rowGrid["Location"] = row["Location"].ToString();

            dtGrid.Rows.Add(rowGrid);
          }
        }
        ultData.DataSource = dtGrid;
      }
      btnSearch.Enabled = true;
    }

    /// <summary>
    ///  Check Valid
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          // Allocated
          if (row.Cells["WO"].Value.ToString().Length == 0)
          {
            // NewWO & NewCarcassCode
            if (row.Cells["NewWO"].Value.ToString().Length == 0)
            {
              message = "New WO";
              return false;
            }

            if (row.Cells["NewCarcassCode"].Value.ToString().Length == 0)
            {
              message = "New CarcassCode";
              return false;
            }
            // Remain
            if (DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()) < 0)
            {
              message = "Remain";
              return false;
            }
            else if (DBConvert.ParseDouble(row.Cells["Balance"].Value.ToString()) -
                      DBConvert.ParseDouble(row.Cells["AllocatedHide"].Value.ToString()) < DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString()))
            {
              message = "Remain";
              return false;
            }
          }
          else  // Re Allocated
          {
            // Allocated
            if (DBConvert.ParseDouble(row.Cells["Allocated"].Value.ToString()) < 0)
            {
              message = "Allocated";
              return false;
            }
            else if (DBConvert.ParseDouble(row.Cells["Allocated"].Value.ToString()) >
                      DBConvert.ParseDouble(row.Cells["AllocatedHide"].Value.ToString()))
            {
              message = "Allocated";
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Create Table Import
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int32));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("NewWO", typeof(System.Int32));
      dt.Columns.Add("NewCarcassCode", typeof(System.String));
      dt.Columns.Add("LotNoId", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("MainMaterialCode", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Add Data Table
    /// </summary>
    private void AddDataTable()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultData.Rows[i];
        if (DBConvert.ParseInt(rowGrid.Cells["Selected"].Value.ToString()) == 1)
        {
          if ((radAllocate.Checked && DBConvert.ParseDouble(rowGrid.Cells["Remain"].Value.ToString()) > 0) ||
            (radReAllocate.Checked && DBConvert.ParseDouble(rowGrid.Cells["Allocated"].Value.ToString()) > 0))
          {
            DataRow row = dtDetail.NewRow();
            if (DBConvert.ParseInt(rowGrid.Cells["WO"].Value.ToString()) > 0)
            {
              row["WO"] = DBConvert.ParseInt(rowGrid.Cells["WO"].Value.ToString());
            }
            if (rowGrid.Cells["CarcassCode"].Value.ToString().Length > 0)
            {
              row["CarcassCode"] = rowGrid.Cells["CarcassCode"].Value.ToString();
            }
            if (DBConvert.ParseInt(rowGrid.Cells["NewWO"].Value.ToString()) > 0)
            {
              row["NewWO"] = DBConvert.ParseInt(rowGrid.Cells["NewWO"].Value.ToString());
            }
            if (rowGrid.Cells["NewCarcassCode"].Value.ToString().Length > 0)
            {
              row["NewCarcassCode"] = rowGrid.Cells["NewCarcassCode"].Value.ToString();
            }
            row["LotNoId"] = rowGrid.Cells["LotNoId"].Value.ToString();
            if (radAllocate.Checked)
            {
              row["Qty"] = DBConvert.ParseDouble(rowGrid.Cells["Remain"].Value.ToString());
            }
            else
            {
              row["Qty"] = DBConvert.ParseDouble(rowGrid.Cells["Allocated"].Value.ToString());
            }
            if (rowGrid.Cells["MainMaterialCode"].Value.ToString().Length > 0)
            {
              row["MainMaterialCode"] = rowGrid.Cells["MainMaterialCode"].Value.ToString();
            }
            dtDetail.Rows.Add(row);
          }
        }
      }
      this.Search();
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
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Hide Column
      e.Layout.Bands[0].Columns["AllocatedHide"].Hidden = true;

      // Allow update, delete, add new
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      // Set Width
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;

      // Set Column Style
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Set color
      e.Layout.Bands[0].Columns["Selected"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Allocated"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.BackColor = Color.LightBlue;

      // Read only
      e.Layout.Bands[0].Columns["WO"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NewWO"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NewCarcassCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MainMaterialCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialNameVn"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Balance"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyM2"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Location"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Length"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Width"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["LotNoId"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

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
      this.Search();
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

    /// <summary>
    /// Save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      this.AddDataTable();
      this.CloseTab();
    }

    /// <summary>
    /// ProcessCmdKey
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Value Change WO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      ultCBCarcassCode.Value = DBNull.Value;
      if (ultCBWO.Text.Trim().Length > 0 && ultCBWO.Value != null)
      {
        int wo = DBConvert.ParseInt(ultCBWO.Value.ToString());
        this.LoadCarcassCode(wo);
      }
      else
      {
        this.LoadCarcassCode(-1);
      }
    }

    /// <summary>
    /// Copy WO, CarcassCode
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCopy_Click(object sender, EventArgs e)
    {
      // Check NewWO, NewCarcassCode
      if (ultCBNewWO.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "New WO");
      }
      if (ultCBNewCarcassCode.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "New CarcassCode");
      }
      // End Check WO, CarcassCode
      int newWo = int.MinValue;
      string newCarcassCode = string.Empty;
      string mainMaterialCode = string.Empty;

      newWo = DBConvert.ParseInt(ultCBNewWO.Value.ToString());
      newCarcassCode = ultCBNewCarcassCode.Value.ToString();
      if (ultCBMainMaterialCode.Value != null)
      {
        mainMaterialCode = ultCBMainMaterialCode.Value.ToString();
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          row.Cells["NewWO"].Value = newWo;
          row.Cells["NewCarcassCode"].Value = newCarcassCode;
          if (mainMaterialCode.Length > 0)
          {
            row.Cells["MainMaterialCode"].Value = mainMaterialCode;
          }
        }
      }
    }

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      txtMaterial.Text = string.Empty;
      ultCBLocation.Value = null;
      txtLengthFrom.Text = string.Empty;
      txtLengthTo.Text = string.Empty;
      txtWidthFrom.Text = string.Empty;
      txtWidthTo.Text = string.Empty;
    }

    /// <summary>
    /// Selected All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      int check = (chkSelectAll.Checked) ? 1 : 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        row.Cells["Selected"].Value = check;
      }
    }

    /// <summary>
    /// Save Continue
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveContinue_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      // Check Valid
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      this.AddDataTable();

      this.Search();
    }
    #endregion Event

    private void ultCBNewWO_ValueChanged(object sender, EventArgs e)
    {
      ultCBNewCarcassCode.Value = DBNull.Value;
      if (ultCBNewWO.Text.Trim().Length > 0 && ultCBNewWO.Value != null)
      {
        int wo = DBConvert.ParseInt(ultCBNewWO.Value.ToString());
        this.LoadNewCarcassCode(wo);
      }
      else
      {
        this.LoadNewCarcassCode(-1);
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "Remain":
          if (DBConvert.ParseDouble(row.Cells["Remain"].Text) <= 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Remain > 0");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          else if (DBConvert.ParseDouble(row.Cells["Remain"].Text) >
              (DBConvert.ParseDouble(row.Cells["Balance"].Value.ToString()) - DBConvert.ParseDouble(row.Cells["AllocatedHide"].Value.ToString())))
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Remain");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          break;
        case "Allocated":
          if (DBConvert.ParseDouble(row.Cells["Allocated"].Text) <= 0)
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Allocated > 0");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          else if (DBConvert.ParseDouble(row.Cells["Allocated"].Text) >
             DBConvert.ParseDouble(row.Cells["AllocatedHide"].Value.ToString()))
          {
            string message = string.Format(FunctionUtility.GetMessage("ERR0001"), "Allocated");
            WindowUtinity.ShowMessageErrorFromText(message);
            e.Cancel = true;
          }
          break;
        default:
          break;
      }
    }
  }
}
