/*
  Author      : 
  Date        : 29/06/2012
  Description : Mapping PRPO(Woods)
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

namespace DaiCo.Purchasing
{
  public partial class viewPUR_07_004 : MainUserControl
  {
    #region Field
    // Pid Receiving
    public long receivingPid = long.MinValue;
    // Status
    private int status = 0;
    // Flag Update
    private bool canUpdate = false;
    private long SupplierPid = long.MinValue;
    // Data Table Receiving Detail
    private DataTable dtDataRecDetail = new DataTable();
    private DataTable dtTemp = new DataTable();
    // Store
    string sp_PURGetPORemainOfReceivingSpecial = "spPURGetPORemainOfReceivingSpecialWoods";
    string sp_PURReceivingInfoSpecialByReveivingPid_Select = "spPURReceivingInfoSpecialWoods_Select";
    string sp_PURReceivingSpecialMapingPOMaterials_Edit = "spPURReceivingSpecialMapingPOWoods_Edit";
    string sp_PURReceivingSpecialStatusPRPOMaterials_Update = "spPURReceivingSpecialStatusPRPOWoods_Update";
    string sp_PURReceivingSummaryDetailWarehouse_Update = "spPURReceivingSummaryDetailWarehouse_Update";
    string sp_PURReceivingSummaryDetailWarehouse_Insert = "spPURReceivingSummaryDetailWarehouse_Insert";
    #endregion Field

    #region Init
    public viewPUR_07_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load Screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_07_004_Load(object sender, EventArgs e)
    {
      // Load UltraCombo Supplier
      this.LoadComboSupplier();

      // Load Data
      this.LoadData();

      // Load Drop Down PO Remain
      this.LoadDropdownPORemain(udrpPORemain);
    }

    /// <summary>
    /// Load UltraCombo Supplier
    /// </summary>
    private void LoadComboSupplier()
    {
      string commandText = "SELECT SupplierCode , EnglishName AS Name FROM TblPURSupplierInfo WHERE Confirm = 2 AND DeleteFlg = 0 ORDER BY EnglishName";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultSupplier.DataSource = dtSource;
      ultSupplier.DisplayMember = "Name";
      ultSupplier.ValueMember = "SupplierCode";
      ultSupplier.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultSupplier.DisplayLayout.Bands[0].Columns["Name"].Width = 450;
      ultSupplier.DisplayLayout.Bands[0].Columns["SupplierCode"].Hidden = true;
    }

    /// <summary>
    /// Load Drop Down PO Remain
    /// </summary>
    /// <param name="udrpMaterials"></param>
    private void LoadDropdownPORemain(UltraDropDown udrpPORemain)
    {
      string commandText = string.Format(@"SELECT Pid
                                          FROM TblPURSupplierInfo
                                          WHERE SupplierCode = '{0}'", ultSupplier.Value.ToString());
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dt != null && dt.Rows.Count > 0)
      {
        this.SupplierPid = DBConvert.ParseLong(dt.Rows[0]["Pid"].ToString());
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@SupplierPid", DbType.Int64, this.SupplierPid);
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(sp_PURGetPORemainOfReceivingSpecial, input);
        dtTemp = dtSource;
        if (dtSource != null)
        {
          udrpPORemain.DataSource = dtSource;
          udrpPORemain.ValueMember = "Value";
          udrpPORemain.DisplayMember = "PONo";
          udrpPORemain.DisplayLayout.Bands[0].ColHeadersVisible = false;
          udrpPORemain.DisplayLayout.Bands[0].Columns["PRNo"].Width = 100;
          udrpPORemain.DisplayLayout.Bands[0].Columns["PONo"].Width = 100;
          udrpPORemain.DisplayLayout.Bands[0].Columns["PRNo"].Width = 100;
          udrpPORemain.DisplayLayout.Bands[0].Columns["QtyRemain"].Width = 100;
          udrpPORemain.DisplayLayout.Bands[0].Columns["PODetailPid"].Hidden = true;
          udrpPORemain.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
        }
      }
    }

    /// <summary>
    /// LoadData
    /// </summary>
    private void LoadData()
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@ReceivingPid", DbType.Int64, this.receivingPid) };
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(sp_PURReceivingInfoSpecialByReveivingPid_Select, inputParam);
      DataTable dtReceivingInfo = dsSource.Tables[0];
      if (dtReceivingInfo.Rows.Count > 0)
      {
        DataRow row = dtReceivingInfo.Rows[0];
        this.txtReceivingNote.Text = row["ReceivingCode"].ToString();
        this.txtTitle.Text = row["Title"].ToString();
        this.ultSupplier.Value = row["ImportSource"].ToString();
        this.txtRemark.Text = row["Remark"].ToString();
        this.txtCreateBy.Text = row["CreateBy"].ToString();
        this.txtDate.Text = row["CreateDate"].ToString();
        this.status = DBConvert.ParseInt(row["Status"].ToString());
        if (this.status == 2)
        {
          this.chkComfirm.Checked = true;
        }
      }
      else
      {
        return;
      }
      this.SetStatusControl();
      // Load Data Detail Receiving
      this.LoadDataReceivingDetail(dsSource);
    }

    /// <summary>
    /// SetStatusControl
    /// </summary>
    private void SetStatusControl()
    {
      this.canUpdate = (btnSave.Visible && this.status == 1);
      this.chkComfirm.Enabled = this.canUpdate;
      this.btnSave.Enabled = this.canUpdate;
    }

    /// <summary>
    /// Load Data Detail Receiving
    /// </summary>
    /// <param name="dsSource"></param>
    private void LoadDataReceivingDetail(DataSet dsSource)
    {
      this.ultReceipt.DataSource = dsSource.Tables[1];
      for (int i = 0; i < ultReceipt.Rows.Count; i++)
      {
        UltraGridRow row = ultReceipt.Rows[i];
        row.Activation = Activation.ActivateOnly;
      }
      this.dtDataRecDetail = (DataTable)this.ultReceipt.DataSource;
      DataTable dtPurData = this.CreateDataTablePurData();
      // Transfer Data
      dtPurData.Merge(dsSource.Tables[2]);
      this.ultPurData.DataSource = dtPurData;
      for (int i = 0; i < ultPurData.Rows.Count; i++)
      {
        UltraGridRow rowGrid = ultPurData.Rows[i];
        rowGrid.Cells["MaterialCode"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["NameEN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["NameVN"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["TotalQty"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["PR"].Activation = Activation.ActivateOnly;
        rowGrid.Cells["AlternativeCode"].Activation = Activation.ActivateOnly;
        if (this.canUpdate)
        {
          rowGrid.Cells["PO"].Activation = Activation.AllowEdit;
          rowGrid.Cells["Qty"].Activation = Activation.AllowEdit;
        }
        else
        {
          rowGrid.Cells["PO"].Activation = Activation.ActivateOnly;
          rowGrid.Cells["Qty"].Activation = Activation.ActivateOnly;
        }
        if (rowGrid.Cells["AlternativeCode"].Value.ToString().Length > 0)
        {
          if (string.Compare(rowGrid.Cells["MaterialCode"].Value.ToString(),
                            rowGrid.Cells["AlternativeCode"].Value.ToString()) != 0)
          {
            rowGrid.CellAppearance.BackColor = Color.Yellow;
          }
          else
          {
            rowGrid.CellAppearance.BackColor = Color.White;
          }
        }
        else
        {
          rowGrid.CellAppearance.BackColor = Color.White;
        }
      }
    }
    #endregion Init

    #region Event
    /// <summary>
    /// After Cell Update (Get Infor From Drop Down)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPurData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().ToLower();
      switch (columnName)
      {
        case "po":
          try
          {
            e.Cell.Row.Cells["PR"].Value = udrpPORemain.SelectedRow.Cells["PRNo"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["PR"].Value = DBNull.Value;
          }
          try
          {
            e.Cell.Row.Cells["AlternativeCode"].Value = udrpPORemain.SelectedRow.Cells["MaterialCode"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["AlternativeCode"].Value = DBNull.Value;
          }
          try
          {
            e.Cell.Row.Cells["Qty"].Value = udrpPORemain.SelectedRow.Cells["QtyRemain"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["Qty"].Value = DBNull.Value;
          }
          try
          {
            e.Cell.Row.Cells["PODetailPid"].Value = udrpPORemain.SelectedRow.Cells["PODetailPid"].Value;
          }
          catch
          {
            e.Cell.Row.Cells["PODetailPid"].Value = DBNull.Value;
          }

          int count = 0;
          if (e.Cell.Row.Cells["AlternativeCode"].Value.ToString().Length > 0)
          {
            if (string.Compare(e.Cell.Row.Cells["AlternativeCode"].Value.ToString(),
                                      e.Cell.Row.Cells["MaterialCode"].Value.ToString()) != 0)
            {
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
            }
            else
            {
              e.Cell.Row.CellAppearance.BackColor = Color.White;
            }
          }
          else
          {
            e.Cell.Row.CellAppearance.BackColor = Color.White;
          }
          break;
        case "materialcode":
          DataRow[] foundRow = dtDataRecDetail.Select("MaterialCode = '" + e.Cell.Row.Cells["MaterialCode"].Value.ToString() + "'");
          if (foundRow.Length > 0)
          {
            e.Cell.Row.Cells["NameEN"].Value = foundRow[0]["NameEN"].ToString();
            e.Cell.Row.Cells["NameVN"].Value = foundRow[0]["NameVN"].ToString();
            e.Cell.Row.Cells["TotalQty"].Value = DBConvert.ParseDouble(foundRow[0]["TotalQty"].ToString());
            e.Cell.Row.Cells["PO"].Value = DBNull.Value;
            e.Cell.Row.Cells["PR"].Value = DBNull.Value;
            e.Cell.Row.Cells["AlternativeCode"].Value = DBNull.Value;

            count = 0;
            string materialCode = e.Cell.Row.Cells["MaterialCode"].Value.ToString();
            for (int i = 0; i < ultPurData.Rows.Count; i++)
            {
              UltraGridRow row = ultPurData.Rows[i];
              if (string.Compare(materialCode, row.Cells["MaterialCode"].Value.ToString()) == 0)
              {
                count++;
              }
            }

            if (count == 1)
            {
              e.Cell.Row.CellAppearance.BackColor = Color.White;
            }
            else
            {
              e.Cell.Row.CellAppearance.BackColor = Color.Yellow;
            }
          }
          else
          {
            e.Cell.Row.Cells["NameEN"].Value = DBNull.Value;
            e.Cell.Row.Cells["NameVN"].Value = DBNull.Value;
            e.Cell.Row.Cells["TotalQty"].Value = DBNull.Value;
            e.Cell.Row.Cells["PO"].Value = DBNull.Value;
            e.Cell.Row.Cells["PR"].Value = DBNull.Value;
            e.Cell.Row.Cells["AlternativeCode"].Value = DBNull.Value;
            e.Cell.Row.CellAppearance.BackColor = Color.Pink;
          }
          break;
        case "qty":
          if (e.Cell.Row.Cells["Qty"].Value.ToString().Length > 0)
          {
            if (DBConvert.ParseDouble(e.Cell.Row.Cells["Qty"].Value.ToString()) <= 0)
            {
              WindowUtinity.ShowMessageError("ERR0001", "Qty");
              break;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Save (Insert/Update) Receiving Note
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      DataTable dtMain = (DataTable)this.ultReceipt.DataSource;
      if (dtMain == null || dtMain.Rows.Count == 0)
      {
        return;
      }
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // Load Data
      this.LoadData();
    }

    /// <summary>
    /// Format Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultReceipt_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";

      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;

      e.Layout.Bands[0].Columns["TotalQty"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["TotalQty"].MinWidth = 80;

      e.Layout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.0000}";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Format Grid PUR Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultPurData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["PODetailPid"].Hidden = true;

      e.Layout.Bands[0].Columns["PO"].ValueList = udrpPORemain;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";

      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;

      e.Layout.Bands[0].Columns["PO"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["PO"].MinWidth = 90;

      e.Layout.Bands[0].Columns["PR"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["PR"].MinWidth = 90;

      e.Layout.Bands[0].Columns["AlternativeCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["AlternativeCode"].MinWidth = 90;

      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 90;

      e.Layout.Bands[0].Columns["TotalQty"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["TotalQty"].MinWidth = 80;

      e.Layout.Bands[0].Columns["TotalQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVN"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PR"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["AlternativeCode"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["AlternativeCode"].Header.Caption = "Alternative Code";

      //Sum Qty And Total CBM
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["TotalQty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["Qty"], SummaryPosition.UseSummaryPositionColumn).Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0.0000}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.0000}";

      e.Layout.Bands[0].Override.AllowAddNew = (this.canUpdate) ? AllowAddNew.TemplateOnBottom : AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = (this.canUpdate) ? DefaultableBoolean.True : DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
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
    #endregion Event

    #region Process
    /// <summary>
    /// Create DataTable Before Save
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTablePurData()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("NameEN", typeof(System.String));
      dt.Columns.Add("NameVN", typeof(System.String));
      dt.Columns.Add("TotalQty", typeof(System.Double));
      dt.Columns.Add("PO", typeof(System.String));
      dt.Columns.Add("PR", typeof(System.String));
      dt.Columns.Add("AlternativeCode", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      return dt;
    }

    /// <summary>
    /// Check valid before save
    /// </summary>
    /// <returns></returns>
    private bool CheckValid(out string message)
    {
      message = string.Empty;
      int count = 0;
      int coutFlagConfirm = 0;
      string race = "|";
      for (int i = 0; i < ultPurData.Rows.Count; i++)
      {
        count++;
        UltraGridRow row = ultPurData.Rows[i];
        string materialCode = string.Empty;

        if (row.CellAppearance.BackColor == Color.Pink)
        {
          message = "Data Input";
          return false;
        }
        if (chkComfirm.Checked)
        {
          string po = row.Cells["PO"].Value.ToString();
          if (po.Length == 0)
          {
            message = "Data Input";
            return false;
          }
          double qty = DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString());
          if (qty == double.MinValue || qty <= 0)
          {
            message = "Data Input";
            return false;
          }
          materialCode = row.Cells["MaterialCode"].Value.ToString();
          int indexOf = race.IndexOf(materialCode);
          race += materialCode + "|";
          if (dtDataRecDetail.Select("MaterialCode = '" + materialCode + "'").Length != 0 && indexOf == -1)
          {
            coutFlagConfirm++;
          }
        }
        materialCode = row.Cells["MaterialCode"].Value.ToString();
        DataRow[] foundRow = dtDataRecDetail.Select("MaterialCode = '" + materialCode + "'");
        if (foundRow.Length == 0)
        {
          message = "Data Input";
          return false;
        }
        else
        {
          continue;
        }
      }
      if (count == 0)
      {
        message = "Data Input";
        return false;
      }
      if (this.chkComfirm.Checked == true && coutFlagConfirm != dtDataRecDetail.Rows.Count)
      {
        message = "Data Input";
        return false;
      }

      // Check Total Remain
      double totalRemain = 0;
      double qty2 = 0;
      DataTable dt = (DataTable)ultPurData.DataSource;
      for (int k = 0; k < dtTemp.Rows.Count; k++)
      {
        DataRow row = dtTemp.Rows[k];
        totalRemain = DBConvert.ParseDouble(row["QtyRemain"].ToString());
        for (int p = 0; p < dt.Rows.Count; p++)
        {
          if (dt.Rows[p].RowState != DataRowState.Deleted)
          {
            // Check Qty 
            if (dt.Rows[p]["Qty"].ToString().Length > 0)
            {
              if (DBConvert.ParseDouble(dt.Rows[p]["Qty"].ToString()) <= 0)
              {
                message = "Qty";
                return false;
              }
            }
            DataRow row1 = dt.Rows[p];
            if (DBConvert.ParseDouble(row["PODetailPid"].ToString()) == DBConvert.ParseDouble(row1["PODetailPid"].ToString()))
            {
              qty2 = qty2 + DBConvert.ParseDouble(row1["Qty"].ToString());
            }
          }
        }
        // Check Qty Remain
        if (qty2 > totalRemain)
        {
          message = "Sum QtyRemain < " + totalRemain;
          return false;
        }
        else
        {
          qty2 = 0;
        }
      }
      return true;
    }

    /// <summary>
    /// Main Save Data
    /// </summary>
    /// <returns></returns>
    private bool SaveData()
    {
      bool result = true;
      //Master Information Receiving Veneer
      result = this.SaveReceivingVeneer();
      if (!result)
      {
        return false;
      }
      if (this.chkComfirm.Checked)
      {
        // Update Status && PR && PO
        result = this.SaveUpdatePRPO();
        if (!result)
        {
          return false;
        }
      }
      return result;
    }

    /// <summary>
    /// Update PR PO
    /// </summary>
    private bool SaveUpdatePRPO()
    {
      DBParameter[] inputParam = new DBParameter[2];
      // ReceivingPid
      inputParam[0] = new DBParameter("@IDPhieuNhap", DbType.AnsiString, 48, txtReceivingNote.Text);
      inputParam[1] = new DBParameter("@Status", DbType.Int32, 2);
      DBParameter[] outputParam = new DBParameter[1];
      outputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
      DataBaseAccess.ExecuteStoreProcedure(sp_PURReceivingSpecialMapingPOMaterials_Edit, inputParam, outputParam);
      long result = DBConvert.ParseLong(outputParam[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      else
      {
        // Update Status PR And PO
        DataBaseAccess.ExecuteStoreProcedure(sp_PURReceivingSpecialStatusPRPOMaterials_Update, inputParam, outputParam);
        result = DBConvert.ParseLong(outputParam[0].Value.ToString());
        if (result <= 0)
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Save Master Information Receiving Veneer
    /// </summary>
    /// <returns></returns>
    private bool SaveReceivingVeneer()
    {
      for (int i = 0; i < ultPurData.Rows.Count; i++)
      {
        UltraGridRow row = ultPurData.Rows[i];
        long pid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
        if (pid != long.MinValue)
        {
          DBParameter[] inputParamUpdate = new DBParameter[5];
          // Pid
          inputParamUpdate[0] = new DBParameter("@PID", DbType.Int64, pid);
          // PO
          if (DBConvert.ParseLong(row.Cells["PODetailPid"].Value.ToString()) != long.MinValue)
          {
            inputParamUpdate[1] = new DBParameter("@PODetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["PODetailPid"].Value.ToString()));
          }
          // AlternativeCode
          if (row.Cells["AlternativeCode"].Value.ToString().Length > 0)
          {
            inputParamUpdate[2] = new DBParameter("@AlternativeCode", DbType.String, row.Cells["AlternativeCode"].Value.ToString());
          }
          // Qty
          if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) != double.MinValue)
          {
            inputParamUpdate[3] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()));
          }
          // UpdateBy
          inputParamUpdate[4] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

          DBParameter[] outputParamUpdate = new DBParameter[1];
          outputParamUpdate[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(sp_PURReceivingSummaryDetailWarehouse_Update, inputParamUpdate, outputParamUpdate);

          long resultPid = DBConvert.ParseLong(outputParamUpdate[0].Value.ToString());
          if (resultPid == long.MinValue)
          {
            return false;
          }
        }
        else
        {
          // Insert
          DBParameter[] inputParamInsert = new DBParameter[8];
          inputParamInsert[0] = new DBParameter("@ReceivingNote", DbType.String, this.txtReceivingNote.Text);
          inputParamInsert[1] = new DBParameter("@MaterialCode", DbType.String, row.Cells["MaterialCode"].Value.ToString());
          inputParamInsert[2] = new DBParameter("@ReadQtyReceived", DbType.Double, DBConvert.ParseDouble(row.Cells["TotalQty"].Value.ToString()));
          // PO
          if (DBConvert.ParseLong(row.Cells["PODetailPid"].Value.ToString()) != long.MinValue)
          {
            inputParamInsert[3] = new DBParameter("@PODetailPid", DbType.Int64, DBConvert.ParseLong(row.Cells["PODetailPid"].Value.ToString()));
          }
          // AlternativeCode
          if (row.Cells["AlternativeCode"].Value.ToString().Length > 0)
          {
            inputParamInsert[4] = new DBParameter("@AlternativeCode", DbType.String, row.Cells["AlternativeCode"].Value.ToString());
          }
          // Qty
          if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) != double.MinValue)
          {
            inputParamInsert[5] = new DBParameter("@Qty", DbType.Double, DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()));
          }
          // UpdateBy
          inputParamInsert[6] = new DBParameter("@UpdateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
          inputParamInsert[7] = new DBParameter("@Type", DbType.Int32, 2);
          DBParameter[] outputParamInsert = new DBParameter[1];
          outputParamInsert[0] = new DBParameter("@ResultPid", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure(sp_PURReceivingSummaryDetailWarehouse_Insert, inputParamInsert, outputParamInsert);
          long resultPid = DBConvert.ParseLong(outputParamInsert[0].Value.ToString());
          if (resultPid == long.MinValue)
          {
            return false;
          }
        }
      }
      return true;
    }
    #endregion Process
  }
}
