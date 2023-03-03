/*
  Author        : 
  Create date   : 27/05/2013
  Decription    : Stock Balance With Image
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VBReport;

namespace DaiCo.Planning
{
  public partial class viewPLN_21_004 : MainUserControl
  {
    #region Field
    private int typeSearch;
    #endregion Field

    #region Init

    public viewPLN_21_004()
    {
      InitializeComponent();
    }

    private void viewPLN_21_004_Load(object sender, EventArgs e)
    {
      this.LoadWO();
      this.LoadType();
      this.LoadLocation();
    }

    #endregion Init

    #region Function

    private void LoadType()
    {
      string commandText = string.Empty;
      commandText += " SELECT 1 Pid, 'All' Label ";
      commandText += " UNION ALL ";
      commandText += " SELECT 2 Pid, 'ID Veneer Locked' Label ";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultType.DataSource = dt;
      ultType.DisplayMember = "Label";
      ultType.ValueMember = "Pid";
      ultType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultType.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultType.DisplayLayout.AutoFitColumns = true;

      this.ultType.Value = 1;
    }

    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[9];
      if (txtMaterial.Text.Length > 0)
      {
        input[0] = new DBParameter("@Material", DbType.String, txtMaterial.Text);
      }
      if (ultCBWO.Value != null && ultCBWO.Text.Length > 0)
      {
        input[1] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }
      if (ultCBCarcass.Value != null && ultCBCarcass.Text.Length > 0)
      {
        input[2] = new DBParameter("@Carcass", DbType.String, ultCBCarcass.Value.ToString());
      }
      if (ultCBLocation.Value != null && ultCBLocation.Text.Length > 0)
      {
        input[3] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(ultCBLocation.Value.ToString()));
      }
      if (DBConvert.ParseDouble(txtLengthFrom.Text) != double.MinValue)
      {
        input[4] = new DBParameter("@LengthFrom", DbType.Double, DBConvert.ParseDouble(txtLengthFrom.Text));
      }
      if (DBConvert.ParseDouble(txtLengthTo.Text) != double.MinValue)
      {
        input[5] = new DBParameter("@LengthTo", DbType.Double, DBConvert.ParseDouble(txtLengthTo.Text));
      }
      if (DBConvert.ParseDouble(txtWidthFrom.Text) != double.MinValue)
      {
        input[6] = new DBParameter("@WidthFrom", DbType.Double, DBConvert.ParseDouble(txtWidthFrom.Text));
      }
      if (DBConvert.ParseDouble(txtWidthTo.Text) != double.MinValue)
      {
        input[7] = new DBParameter("@WidthTo", DbType.Double, DBConvert.ParseDouble(txtWidthTo.Text));
      }
      if (txtLotNoID.Text.Length > 0)
      {
        input[8] = new DBParameter("@LotNoID", DbType.String, txtLotNoID.Text);
      }

      if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 1)
      {
        this.typeSearch = 1;
        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNVeneerStockBalanceWithImage", input);
        if (dsSource != null)
        {
          DataSet ds = this.CreateDataSet();
          ds.Tables["dtParent"].Merge(dsSource.Tables[0]);
          ds.Tables["dtChild"].Merge(dsSource.Tables[1]);
          ultData.DataSource = ds;
          this.ultWOCopy.Enabled = true;
          this.ultCarcassCodeCopy.Enabled = true;
          this.btnCopy.Enabled = true;
          this.chkSelectAll.Visible = false;
        }
      }
      else if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 2)
      {
        this.typeSearch = 2;
        DataTable dtData = DataBaseAccess.SearchCommandTextDataTable("spPLNVeneerStockBalanceLockQty", input);
        if (dtData != null)
        {
          this.ultData.DataSource = dtData;
          this.ultWOCopy.Enabled = false;
          this.ultCarcassCodeCopy.Enabled = false;
          this.btnCopy.Enabled = false;
          this.chkSelectAll.Visible = true;
        }
      }

      // To mau LotNoID co Image
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        string lotNoId = row.Cells["LotNoId"].Value.ToString();
        string locationImage = @FunctionUtility.GetImagePathByPid(11) + string.Format(@"\{0}", lotNoId);
        if (Directory.Exists(locationImage))
        {
          row.Appearance.BackColor = Color.Yellow;
        }
      }

      btnSearch.Enabled = true;
    }

    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("MaterialCode", typeof(System.String));
      taParent.Columns.Add("MaterialNameVn", typeof(System.String));
      taParent.Columns.Add("LotNoId", typeof(System.String));
      taParent.Columns.Add("Unit", typeof(System.String));
      taParent.Columns.Add("Length", typeof(System.Double));
      taParent.Columns.Add("Width", typeof(System.Double));
      taParent.Columns.Add("Location", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Double));
      taParent.Columns.Add("QtyM2", typeof(System.Double));
      taParent.Columns.Add("Allocated", typeof(System.Int32));
      taParent.Columns.Add("AllocatedM2", typeof(System.Double));
      taParent.Columns.Add("Locked", typeof(System.Int32));
      taParent.Columns.Add("LockedM2", typeof(System.Double));
      taParent.Columns.Add("RemainQty", typeof(System.Double));
      taParent.Columns.Add("RemainQtyM2", typeof(System.Double));
      taParent.Columns.Add("LockQty", typeof(System.Double));
      taParent.Columns.Add("LockQtyM2", typeof(System.Double));
      taParent.Columns.Add("WO", typeof(System.String));
      taParent.Columns.Add("CarcassCode", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("Flag", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("LotNoId", typeof(System.String));
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("AllocateQty", typeof(System.Int32));
      taChild.Columns.Add("Type", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("taParent_taChild", taParent.Columns["LotNoId"], taChild.Columns["LotNoId"], false));
      return ds;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    private void LoadWO()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder ORDER BY Pid DESC";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBWO.DataSource = dtSource;
      ultCBWO.DisplayMember = "Pid";
      ultCBWO.ValueMember = "Pid";
      ultCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBWO.DisplayLayout.Bands[0].Columns["Pid"].Width = 200;
      ultCBWO.DisplayLayout.AutoFitColumns = true;

      ultWOCopy.DataSource = dtSource;
      ultWOCopy.DisplayMember = "Pid";
      ultWOCopy.ValueMember = "Pid";
      ultWOCopy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultWOCopy.DisplayLayout.Bands[0].Columns["Pid"].Width = 200;
      ultWOCopy.DisplayLayout.AutoFitColumns = true;

      udrpWO.DataSource = dtSource;
      udrpWO.ValueMember = "Pid";
      udrpWO.DisplayMember = "Pid";
      udrpWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadCarcass(long wo)
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT CarcassCode ";
      commandText += " FROM dbo.TblPLNWorkOrderConfirmedDetails ";
      if (wo != long.MinValue)
      {
        commandText += " WHERE WorkOrderPid = " + wo;
      }
      commandText += " ORDER BY CarcassCode ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultCBCarcass.DataSource = dtSource;
        ultCBCarcass.ValueMember = "CarcassCode";
        ultCBCarcass.DisplayMember = "CarcassCode";
        ultCBCarcass.DisplayLayout.Bands[0].Columns["CarcassCode"].Width = 200;
        ultCBCarcass.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultCBCarcass.DisplayLayout.AutoFitColumns = true;
      }
    }

    private void LoadCarcassCopy(long wo)
    {
      string commandText = string.Empty;
      commandText += " SELECT DISTINCT CarcassCode ";
      commandText += " FROM dbo.TblPLNWorkOrderConfirmedDetails ";
      if (wo != long.MinValue)
      {
        commandText += " WHERE WorkOrderPid = " + wo;
      }
      commandText += " ORDER BY CarcassCode ";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultCarcassCodeCopy.DataSource = dtSource;
        ultCarcassCodeCopy.ValueMember = "CarcassCode";
        ultCarcassCodeCopy.DisplayMember = "CarcassCode";
        ultCarcassCodeCopy.DisplayLayout.Bands[0].Columns["CarcassCode"].Width = 200;
        ultCarcassCodeCopy.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultCarcassCodeCopy.DisplayLayout.AutoFitColumns = true;
      }
    }

    private void LoadLocation()
    {
      string commandText = "SELECT ID_Position, Name FROM VWHDLocation";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBLocation.DataSource = dtSource;
      ultCBLocation.DisplayMember = "Name";
      ultCBLocation.ValueMember = "ID_Position";
      ultCBLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBLocation.DisplayLayout.Bands[0].Columns["ID_Position"].Hidden = true;
      ultCBLocation.DisplayLayout.AutoFitColumns = true;
    }

    static bool IsValidImage(string filePath)
    {
      return File.Exists(filePath);
    }

    private bool SaveData()
    {
      if (typeSearch == 1)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          if (DBConvert.ParseInt(row.Cells["LockQty"].Value.ToString()) != int.MinValue
              && row.Cells["WO"].Value.ToString().Length > 0
              && row.Cells["CarcassCode"].Value.ToString().Length > 0)
          {
            DBParameter[] input = new DBParameter[6];
            input[0] = new DBParameter("@PID", DbType.Int64, DBConvert.ParseLong(row.Cells["PID"].Value.ToString()));
            input[1] = new DBParameter("@LotNoId", DbType.String, row.Cells["LotNoId"].Value.ToString());
            input[2] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
            input[3] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
            input[4] = new DBParameter("@QtyLock", DbType.Int32, DBConvert.ParseInt(row.Cells["LockQty"].Value.ToString()));
            input[5] = new DBParameter("@Remark", DbType.String, row.Cells["Remark"].Value.ToString());

            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

            DataBaseAccess.ExecuteStoreProcedure("spPLNVeneerStockBalanceRemark_Update", input, output);
            long result = DBConvert.ParseLong(output[0].Value.ToString());
            if (result <= 0)
            {
              return false;
            }
          }
        }
      }
      else if (typeSearch == 2)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          UltraGridRow row = ultData.Rows[i];
          if (DBConvert.ParseInt(row.Cells["UnLock"].Value.ToString()) == 1)
          {
            DBParameter[] input = new DBParameter[4];
            input[0] = new DBParameter("@LotNoId", DbType.String, row.Cells["LotNoId"].Value.ToString());
            input[1] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(row.Cells["WO"].Value.ToString()));
            input[2] = new DBParameter("@CarcassCode", DbType.String, row.Cells["CarcassCode"].Value.ToString());
            input[3] = new DBParameter("@QtyUnLock", DbType.Int32, DBConvert.ParseInt(row.Cells["QtyLock"].Value.ToString()));

            DBParameter[] output = new DBParameter[1];
            output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

            DataBaseAccess.ExecuteStoreProcedure("spPLNVeneerLockID_Update", input, output);
            long result = DBConvert.ParseLong(output[0].Value.ToString());
            if (result <= 0)
            {
              return false;
            }
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Check valid before Copy
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      if (this.ultWOCopy.Value == null || DBConvert.ParseInt(this.ultWOCopy.Value.ToString()) == int.MinValue)
      {
        message = "WO";
        return false;
      }

      if (this.ultCarcassCodeCopy.Value == null || this.ultCarcassCodeCopy.Value.ToString().Length == 0)
      {
        message = "Carcass";
        return false;
      }

      return true;
    }

    /// <summary>
    /// Check valid before Save
    /// </summary>
    /// <returns></returns>
    private bool CheckValidSave(out string message)
    {
      message = "";
      if (typeSearch == 1)
      {
        string commandText = string.Empty;
        DataTable dtCheck;
        for (int i = 0; i < this.ultData.Rows.Count; i++)
        {
          UltraGridRow row = this.ultData.Rows[i];
          if ((row.Cells["WO"].Value.ToString().Length == 0
              && row.Cells["CarcassCode"].Value.ToString().Length > 0)
            || (row.Cells["WO"].Value.ToString().Length > 0
              && row.Cells["CarcassCode"].Value.ToString().Length == 0))
          {
            message = "Data WO Or CarcassCode";
            return false;
          }

          if (row.Cells["WO"].Value.ToString().Length > 0
              && row.Cells["CarcassCode"].Value.ToString().Length > 0)
          {
            commandText = string.Empty;
            commandText += " SELECT STO.Qty - ISNULL(QTYUS.AllocatedQty, 0) Qty ";
            commandText += " FROM TblWHDStockBalance_Veneer STO ";
            commandText += " 	LEFT JOIN ";
            commandText += " 	( ";
            commandText += " 		SELECT AA.LotNoId, SUM(AA.AllocatedQty) AllocatedQty ";
            commandText += " 		FROM ";
            commandText += " 		( ";
            commandText += " 			SELECT LotNoId, SUM(AllocateQty - ISNULL(IssuedQty, 0)) AllocatedQty ";
            commandText += " 			FROM tblPLNVeneerLotNoIdAllocation ";
            commandText += " 			WHERE LotNoId = '" + row.Cells["LotNoId"].Value.ToString() + "'";
            commandText += " 		  GROUP BY LotNoId ";
            commandText += " 		  UNION ALL ";
            commandText += " 			SELECT LotNoId, SUM(QtyLock) QtyLock ";
            commandText += " 		  FROM TblPLNVeneerLotNoIdLocked ";
            commandText += " 		  WHERE LotNoId = '" + row.Cells["LotNoId"].Value.ToString() + "'";
            commandText += " 		  GROUP BY LotNoId ";
            commandText += " 		) AA ";
            commandText += " 		GROUP BY AA.LotNoId ";
            commandText += " 	) QTYUS ON STO.LotNoId = QTYUS.LotNoId";
            commandText += " 	WHERE STO.LotNoId = '" + row.Cells["LotNoId"].Value.ToString() + "'";

            dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dtCheck.Rows.Count > 0)
            {
              if (DBConvert.ParseInt(row.Cells["LockQty"].Value.ToString()) >
                  DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()))
              {
                message = "Lock Qty";
                return false;
              }
            }
            else
            {
              message = "Lock Qty";
              return false;
            }
          }
        }
      }

      return true;
    }

    #endregion Function

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      if (this.ultType.Value == null
          || DBConvert.ParseInt(this.ultType.Value.ToString()) == int.MinValue)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Type");
        return;
      }

      this.Search();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBWO.Value != null && ultCBWO.Text.Length > 0)
      {
        this.LoadCarcass(DBConvert.ParseLong(ultCBWO.Value.ToString()));
      }
      else
      {
        this.LoadCarcass(long.MinValue);
      }
    }

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      ControlUtility.SetPropertiesUltraGrid(ultData);

      if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 1)
      {
        e.Layout.Bands[0].Columns["PID"].Hidden = true;
        e.Layout.Bands[0].Columns["Flag"].Hidden = true;
        e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Remark(QC)";
        e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Sheet";
        e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["QtyM2"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["RemainQty"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["RemainQtyM2"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["LockQty"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["LockQtyM2"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[1].Columns["AllocateQty"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["Allocated"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["AllocatedM2"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["Locked"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["LockedM2"].CellAppearance.TextHAlign = HAlign.Right;

        e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["MaterialNameVn"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["LotNoId"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["QtyM2"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Length"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Width"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Location"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["RemainQty"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["RemainQtyM2"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["LockQtyM2"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Allocated"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["AllocatedM2"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Locked"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["LockedM2"].CellActivation = Activation.ActivateOnly;

        e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.LightGreen;
        e.Layout.Bands[0].Columns["Allocated"].CellAppearance.BackColor = Color.LightGreen;
        e.Layout.Bands[0].Columns["Locked"].CellAppearance.BackColor = Color.LightGreen;
        e.Layout.Bands[0].Columns["RemainQty"].CellAppearance.BackColor = Color.LightGreen;
        e.Layout.Bands[0].Columns["LockQty"].CellAppearance.BackColor = Color.LightSteelBlue;
        e.Layout.Bands[0].Columns["WO"].CellAppearance.BackColor = Color.LightSteelBlue;
        e.Layout.Bands[0].Columns["CarcassCode"].CellAppearance.BackColor = Color.LightSteelBlue;

        e.Layout.Bands[0].Columns["WO"].ValueList = this.udrpWO;

        //Sum Qty
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["QtyM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Allocated"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["AllocatedM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Locked"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["LockedM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RemainQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["RemainQtyM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["LockQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["LockQtyM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
        e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.00}";
        e.Layout.Bands[0].Summaries[2].DisplayFormat = "{0:###,##0}";
        e.Layout.Bands[0].Summaries[3].DisplayFormat = "{0:###,##0.00}";
        e.Layout.Bands[0].Summaries[4].DisplayFormat = "{0:###,##0}";
        e.Layout.Bands[0].Summaries[5].DisplayFormat = "{0:###,##0.00}";
        e.Layout.Bands[0].Summaries[6].DisplayFormat = "{0:###,##0}";
        e.Layout.Bands[0].Summaries[7].DisplayFormat = "{0:###,##0.00}";
        e.Layout.Bands[0].Summaries[8].DisplayFormat = "{0:###,##0}";
        e.Layout.Bands[0].Summaries[9].DisplayFormat = "{0:###,##0.00}";

        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
        e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      }
      else if (DBConvert.ParseInt(this.ultType.Value.ToString()) == 2)
      {
        e.Layout.Bands[0].Columns["QtyLock"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["QtyM2"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = HAlign.Right;

        e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["MaterialNameVn"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Unit"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["LotNoId"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["QtyLock"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["QtyM2"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Length"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Width"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["Location"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["WO"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Activation.ActivateOnly;
        e.Layout.Bands[0].Columns["UnLock"].CellAppearance.BackColor = Color.LightSteelBlue;

        //Sum Qty
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["QtyLock"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["QtyM2"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
        e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
        e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.00}";

        e.Layout.Bands[0].Columns["UnLock"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = ultData.Selected.Rows[0];
      string lotNoId = row.Cells["LotNoId"].Value.ToString();
      try
      {
        // Location Image(ID = 11)
        string locationImage = @FunctionUtility.GetImagePathByPid(11) + string.Format(@"\{0}\{1}_1.jpg", lotNoId, lotNoId);
        Process p = new Process();
        p.StartInfo.FileName = "rundll32.exe";
        if (IsValidImage(locationImage))
        {
          p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen " + locationImage;
        }
        else
        {
          p.StartInfo.Arguments = @"C:\WINDOWS\System32\shimgvw.dll,ImageView_Fullscreen ";
        }
        p.Start();
      }
      catch
      {
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckValidSave(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }

      this.Search();
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (string.Compare(colName, "Remark", true) == 0
          || string.Compare(colName, "LockQty", true) == 0
          || string.Compare(colName, "WO", true) == 0
          || string.Compare(colName, "CarcassCode", true) == 0)
      {
        e.Cell.Row.Cells["Flag"].Value = 1;
      }

      if (string.Compare(colName, "LockQty", true) == 0)
      {
        e.Cell.Row.Cells["LockQtyM2"].Value = DBConvert.ParseInt(e.Cell.Row.Cells["LockQty"].Value.ToString())
                  * DBConvert.ParseDouble(e.Cell.Row.Cells["Length"].Value.ToString())
                  * DBConvert.ParseDouble(e.Cell.Row.Cells["Width"].Value.ToString()) / 1000000;
      }

      if (string.Compare(colName, "WO", true) == 0)
      {
        if (DBConvert.ParseLong(e.Cell.Row.Cells["WO"].Value.ToString()) != long.MinValue)
        {
          e.Cell.Row.Cells["CarcassCode"].Value = DBNull.Value;
          UltraDropDown udrpCarcassCode = (UltraDropDown)e.Cell.Row.Cells["CarcassCode"].ValueList;
          string commandText = string.Empty;
          commandText += " SELECT DISTINCT CarcassCode ";
          commandText += " FROM dbo.TblPLNWorkOrderConfirmedDetails ";
          commandText += " WHERE WorkOrderPid = " + DBConvert.ParseInt(e.Cell.Row.Cells["WO"].Value.ToString());
          commandText += " ORDER BY CarcassCode ";
          DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

          if (udrpCarcassCode == null)
          {
            udrpCarcassCode = new UltraDropDown();
            this.Controls.Add(udrpCarcassCode);
          }

          udrpCarcassCode.DataSource = dtSource;
          udrpCarcassCode.ValueMember = "CarcassCode";
          udrpCarcassCode.DisplayMember = "CarcassCode";
          udrpCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = false;
          udrpCarcassCode.Visible = false;

          e.Cell.Row.Cells["CarcassCode"].ValueList = udrpCarcassCode;
        }
        else
        {
          e.Cell.Row.Cells["CarcassCode"].Value = DBNull.Value;
        }
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      if (typeSearch == 1)
      {
        string strTemplateName = "RPT_PLN_21_004";
        string strSheetName = "Sheet1";
        string strOutFileName = "Stock Balance (Veneer)";
        string strStartupPath = System.Windows.Forms.Application.StartupPath;
        string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
        string strPathTemplate = strStartupPath + @"\ExcelTemplate";
        XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

        DataSet dsData = (DataSet)ultData.DataSource;
        DataTable dtData = dsData.Tables[0];

        if (dtData != null && dtData.Rows.Count > 0)
        {
          for (int i = 0; i < dtData.Rows.Count; i++)
          {
            DataRow dtRow = dtData.Rows[i];
            if (i > 0)
            {
              oXlsReport.Cell("B8:P8").Copy();
              oXlsReport.RowInsert(7 + i);
              oXlsReport.Cell("B8:P8", 0, i).Paste();
            }
            oXlsReport.Cell("**No", 0, i).Value = i + 1;
            oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
            oXlsReport.Cell("**MaterialName", 0, i).Value = dtRow["MaterialNameVn"].ToString();
            oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
            oXlsReport.Cell("**LotNoId", 0, i).Value = dtRow["LotNoId"].ToString();
            oXlsReport.Cell("**Length", 0, i).Value = DBConvert.ParseDouble(dtRow["Length"].ToString());
            oXlsReport.Cell("**Width", 0, i).Value = DBConvert.ParseDouble(dtRow["Width"].ToString());
            oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();
            if (DBConvert.ParseInt(dtRow["Qty"].ToString()) != int.MinValue)
            {
              oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseInt(dtRow["Qty"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
            }

            if (DBConvert.ParseDouble(dtRow["QtyM2"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**Qtym2", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Qtym2", 0, i).Value = DBNull.Value;
            }

            if (DBConvert.ParseInt(dtRow["RemainQty"].ToString()) != int.MinValue)
            {
              oXlsReport.Cell("**RemainQty", 0, i).Value = DBConvert.ParseInt(dtRow["RemainQty"].ToString());
            }
            else
            {
              oXlsReport.Cell("**RemainQty", 0, i).Value = DBNull.Value;
            }

            if (DBConvert.ParseDouble(dtRow["RemainQtyM2"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**RemainQtyM2", 0, i).Value = DBConvert.ParseDouble(dtRow["RemainQtyM2"].ToString());
            }
            else
            {
              oXlsReport.Cell("**RemainQtyM2", 0, i).Value = DBNull.Value;
            }

            if (DBConvert.ParseInt(dtRow["LockQty"].ToString()) != int.MinValue)
            {
              oXlsReport.Cell("**LockQty", 0, i).Value = DBConvert.ParseInt(dtRow["LockQty"].ToString());
            }
            else
            {
              oXlsReport.Cell("**LockQty", 0, i).Value = DBNull.Value;
            }

            if (DBConvert.ParseDouble(dtRow["LockQtyM2"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**LockQtyM2", 0, i).Value = DBConvert.ParseDouble(dtRow["LockQtyM2"].ToString());
            }
            else
            {
              oXlsReport.Cell("**LockQtyM2", 0, i).Value = DBNull.Value;
            }
            oXlsReport.Cell("**Remark", 0, i).Value = dtRow["Remark"].ToString();
          }
        }

        oXlsReport.Out.File(strOutFileName);
        Process.Start(strOutFileName);
      }
      else if (typeSearch == 2)
      {
        string strTemplateName = "RPT_PLN_21_004_01";
        string strSheetName = "Sheet1";
        string strOutFileName = "Lock Qty (Veneer)";
        string strStartupPath = System.Windows.Forms.Application.StartupPath;
        string strPathOutputFile = string.Format(@"{0}\Report", strStartupPath);
        string strPathTemplate = strStartupPath + @"\ExcelTemplate";
        XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(strTemplateName, strSheetName, strOutFileName, strPathTemplate, strPathOutputFile, out strOutFileName);

        DataTable dtData = (DataTable)ultData.DataSource;

        if (dtData != null && dtData.Rows.Count > 0)
        {
          for (int i = 0; i < dtData.Rows.Count; i++)
          {
            DataRow dtRow = dtData.Rows[i];
            if (i > 0)
            {
              oXlsReport.Cell("B8:M8").Copy();
              oXlsReport.RowInsert(7 + i);
              oXlsReport.Cell("B8:M8", 0, i).Paste();
            }
            oXlsReport.Cell("**No", 0, i).Value = i + 1;
            oXlsReport.Cell("**MaterialCode", 0, i).Value = dtRow["MaterialCode"].ToString();
            oXlsReport.Cell("**MaterialName", 0, i).Value = dtRow["MaterialNameVn"].ToString();
            oXlsReport.Cell("**Unit", 0, i).Value = dtRow["Unit"].ToString();
            oXlsReport.Cell("**WO", 0, i).Value = DBConvert.ParseInt(dtRow["WO"].ToString());
            oXlsReport.Cell("**CarcassCode", 0, i).Value = dtRow["CarcassCode"].ToString();
            oXlsReport.Cell("**LotNoId", 0, i).Value = dtRow["LotNoId"].ToString();
            oXlsReport.Cell("**Length", 0, i).Value = DBConvert.ParseDouble(dtRow["Length"].ToString());
            oXlsReport.Cell("**Width", 0, i).Value = DBConvert.ParseDouble(dtRow["Width"].ToString());
            oXlsReport.Cell("**Location", 0, i).Value = dtRow["Location"].ToString();
            if (DBConvert.ParseInt(dtRow["QtyLock"].ToString()) != int.MinValue)
            {
              oXlsReport.Cell("**Qty", 0, i).Value = DBConvert.ParseInt(dtRow["QtyLock"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Qty", 0, i).Value = DBNull.Value;
            }

            if (DBConvert.ParseDouble(dtRow["QtyM2"].ToString()) != double.MinValue)
            {
              oXlsReport.Cell("**Qtym2", 0, i).Value = DBConvert.ParseDouble(dtRow["QtyM2"].ToString());
            }
            else
            {
              oXlsReport.Cell("**Qtym2", 0, i).Value = DBNull.Value;
            }
          }
        }

        oXlsReport.Out.File(strOutFileName);
        Process.Start(strOutFileName);
      }
    }

    private void ultWOCopy_ValueChanged(object sender, EventArgs e)
    {
      if (ultWOCopy.Value != null && ultWOCopy.Text.Length > 0)
      {
        this.LoadCarcassCopy(DBConvert.ParseLong(ultWOCopy.Value.ToString()));
      }
      else
      {
        this.LoadCarcassCopy(long.MinValue);
      }
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }

      // Copy WO-Carcass
      for (int i = 0; i < this.ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];
        row.Cells["WO"].Value = this.ultWOCopy.Value.ToString();
        row.Cells["CarcassCode"].Value = this.ultCarcassCodeCopy.Value.ToString();
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (typeSearch == 1)
      {
        string columnName = e.Cell.Column.ToString();
        string text = e.Cell.Text.Trim();
        string commandText = string.Empty;
        DataTable dt;
        switch (columnName.ToLower())
        {
          case "lockqty":
            if (DBConvert.ParseInt(text) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", columnName);
              e.Cancel = true;
              break;
            }

            if (DBConvert.ParseInt(text) > DBConvert.ParseInt(e.Cell.Row.Cells["RemainQty"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0010", columnName, "RemainQty");
              e.Cancel = true;
              break;
            }
            break;
          case "wo":
            if (text.Length > 0)
            {
              if (DBConvert.ParseInt(text) <= 0)
              {
                WindowUtinity.ShowMessageError("ERR0001", columnName);
                e.Cancel = true;
                break;
              }

              commandText = string.Empty;
              commandText += "SELECT Pid FROM TblPLNWorkOrder WHERE Pid =" + DBConvert.ParseInt(text);
              dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dt == null || dt.Rows.Count == 0)
              {
                WindowUtinity.ShowMessageError("ERR0001", columnName);
                e.Cancel = true;
                break;
              }
            }
            break;
          case "carcasscode":
            if (text.Length > 0)
            {
              commandText = string.Empty;
              commandText += " SELECT DISTINCT CarcassCode ";
              commandText += " FROM dbo.TblPLNWorkOrderConfirmedDetails ";
              commandText += " WHERE WorkOrderPid = " + DBConvert.ParseInt(e.Cell.Row.Cells["WO"].Value.ToString());
              commandText += "    AND CarcassCode ='" + text + "'";

              dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dt == null || dt.Rows.Count == 0)
              {
                WindowUtinity.ShowMessageError("ERR0001", columnName);
                e.Cancel = true;
                break;
              }
            }
            break;
          default:
            break;
        }
      }
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkSelectAll.Checked)
      {
        for (int i = 0; i < this.ultData.Rows.Count; i++)
        {
          UltraGridRow row = this.ultData.Rows[i];
          row.Cells["UnLock"].Value = 1;
        }
      }
      else
      {
        for (int i = 0; i < this.ultData.Rows.Count; i++)
        {
          UltraGridRow row = this.ultData.Rows[i];
          row.Cells["UnLock"].Value = 0;
        }
      }

    }
    #endregion Event
  }
}
