/*
  Author      : 
  Date        : 19/8/2013
  Description : Add Detail PR Online For SubCon
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

namespace DaiCo.Purchasing
{
  public partial class viewPUR_21_009 : MainUserControl
  {
    #region Field
    public DataTable dtDetail = new DataTable();
    #endregion Field

    #region Init

    /// <summary>
    /// viewPUR_21_009
    /// </summary>
    public viewPUR_21_009()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load
    /// </summary>
    private void viewPUR_21_009_Load(object sender, EventArgs e)
    {
      // Init Data Control
      this.LoadWorkOder();
      this.LoadUrgentLevel();
      this.LoadUrgentLevelCopy();
      this.LoadSupplier();
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
    #endregion Init

    #region Function
    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadWorkOder()
    {
      string commandText = string.Empty;
      commandText = " SELECT Pid WO FROM TblPLNWorkOrder ";
      DataTable dtWO = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCBWO.DataSource = dtWO;
      ultCBWO.ValueMember = "WO";
      ultCBWO.DisplayMember = "WO";
      ultCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBWO.DisplayLayout.Bands[0].Columns["WO"].Width = 250;
    }

    /// <summary>
    /// Load ItemCode
    /// </summary>
    private void LoadItemCode(int wo)
    {
      string commandText = string.Empty;
      commandText = " SELECT DISTINCT ItemCode FROM TblPLNWOInfoDetailGeneral";
      commandText += " WHERE WoInfoPID = " + wo;
      DataTable dtItemCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucItemCode.DataSource = dtItemCode;
      ucItemCode.ColumnWidths = "200";
      ucItemCode.DataBind();
      ucItemCode.ValueMember = "ItemCode";
      ucItemCode.DisplayMember = "ItemCode";
    }

    /// <summary>
    /// Load CarcassCode
    /// </summary>
    private void LoadCarcassCode(int wo)
    {
      string commandText = string.Empty;
      commandText = " SELECT DISTINCT ITE.CarcassCode ";
      commandText += " FROM TblPLNWOInfoDetailGeneral GNR";
      commandText += "    LEFT JOIN TblBOMItemInfo ITE ON ITE.ItemCode = GNR.ItemCode AND ITE.Revision = GNR.Revision";
      commandText += " WHERE GNR.WoInfoPID = " + wo;
      DataTable dtCarcassCode = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCarcassCode.DataSource = dtCarcassCode;
      ucCarcassCode.ColumnWidths = "200";
      ucCarcassCode.DataBind();
      ucCarcassCode.ValueMember = "CarcassCode";
      ucCarcassCode.DisplayMember = "CarcassCode";
    }

    /// <summary>
    /// Load Urgent
    /// </summary>
    private void LoadUrgentLevel()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7007 AND Code IN(1,3,5) AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultDDUrgent.DataSource = dtSource;
      ultDDUrgent.DisplayMember = "Name";
      ultDDUrgent.ValueMember = "Code";
      ultDDUrgent.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDUrgent.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultDDUrgent.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load Urgent Copy
    /// </summary>
    private void LoadUrgentLevelCopy()
    {
      string commandText = "SELECT [Code], [Value] Name FROM TblBOMCodeMaster WHERE [Group] = 7007 AND Code IN(1,3,5) AND DeleteFlag = 0 ORDER BY Sort";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBUrgentCopy.DataSource = dtSource;
      ultCBUrgentCopy.DisplayMember = "Name";
      ultCBUrgentCopy.ValueMember = "Code";
      ultCBUrgentCopy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBUrgentCopy.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
      ultCBUrgentCopy.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load Urgent
    /// </summary>
    private void LoadSupplier()
    {
      string commandText = string.Empty;
      commandText = " SELECT Pid ,SupplierCode Code ,EnglishName NameEN, VietnameseName NameVN, EnglishName + ' - ' + SupplierCode NameCode ";
      commandText += " FROM TblPURSupplierInfo ";
      commandText += "WHERE Confirm = 2 AND DeleteFlg = 0 AND LEN(EnglishName) > 2 ORDER BY EnglishName";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBSupplier.DataSource = dtSource;
      ultCBSupplier.DisplayMember = "NameCode";
      ultCBSupplier.ValueMember = "Pid";
      ultCBSupplier.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["Code"].Width = 100;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["NameEN"].Width = 350;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["NameVN"].Width = 350;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultCBSupplier.DisplayLayout.Bands[0].Columns["NameCode"].Hidden = true;
    }
    /// <summary>
    /// Create Data Table
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WO", typeof(System.Int32));
      dt.Columns.Add("CarcassCode", typeof(System.String));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("NameEN", typeof(System.String));
      dt.Columns.Add("NameVN", typeof(System.String));
      dt.Columns.Add("Unit", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      dt.Columns.Add("Urgent", typeof(System.Int32));
      dt.Columns.Add("RequestDate", typeof(System.DateTime));
      dt.Columns.Add("Remark", typeof(System.String));
      return dt;
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      // Check WO
      if (ultCBWO.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "WO");
        return;
      }
      // Check Supplier
      if (ultCBSupplier.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Supplier");
        return;
      }

      chkSelectAll.Checked = true;
      if (ultCBWO.Value != null)
      {
        DBParameter[] input = new DBParameter[4];
        input[0] = new DBParameter("@WO", DbType.Int32, DBConvert.ParseInt(ultCBWO.Value.ToString()));
        if (txtItemCode.Text.Length > 0)
        {
          input[1] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text);
        }
        if (txtCarcassCode.Text.Length > 0)
        {
          input[2] = new DBParameter("@CarcassCode", DbType.String, txtCarcassCode.Text);
        }
        input[3] = new DBParameter("@SupplierPid", DbType.Int64, DBConvert.ParseLong(ultCBSupplier.Value.ToString()));

        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPURGetInfoMakePROnlineForSubCon", input);
        if (dtSource != null)
        {
          ultData.DataSource = dtSource;
        }
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

      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Purpose Of Requisition";
      e.Layout.Bands[0].Columns["RequestDate"].Header.Caption = "(*)Delivery Date";
      e.Layout.Bands[0].Columns["Urgent"].Header.Caption = "(*)Urgent";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "(*)Qty";
      e.Layout.Bands[0].Columns["Urgent"].ValueList = ultDDUrgent;
      e.Layout.Bands[0].Columns["RequestDate"].Format = "dd-MMM-yyyy";
      //e.Layout.Bands[0].Columns["RequestDate"].Format = ConstantClass.USER_COMPUTER_FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["WO"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CarcassCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["MaterialCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["WOQty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PendingPR"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameEN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["NameVN"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Unit"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Qty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["PendingPR"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["PendingPR"].MinWidth = 80;
      e.Layout.Bands[0].Columns["WOQty"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["WOQty"].MinWidth = 70;
      e.Layout.Bands[0].Columns["WO"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["WO"].MinWidth = 70;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ItemCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 120;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 120;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Urgent"].MaxWidth = 80;
      e.Layout.Bands[0].Columns["Urgent"].MinWidth = 80;
      e.Layout.Bands[0].Columns["RequestDate"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["RequestDate"].MinWidth = 100;

      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Remark"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["Urgent"].CellAppearance.BackColor = Color.Aqua;
      e.Layout.Bands[0].Columns["RequestDate"].CellAppearance.BackColor = Color.Aqua;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    ///  Change Wo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultCBWO_ValueChanged(object sender, EventArgs e)
    {
      if (ultCBWO.Value != null)
      {
        this.LoadItemCode(DBConvert.ParseInt(ultCBWO.Value.ToString()));
        this.LoadCarcassCode(DBConvert.ParseInt(ultCBWO.Value.ToString()));
      }
      else
      {
        this.LoadItemCode(-1);
        this.LoadCarcassCode(-1);
      }
    }

    /// <summary>
    ///  Check Change Carcass
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkCarcassCode_CheckedChanged(object sender, EventArgs e)
    {
      ucCarcassCode.Visible = chkCarcassCode.Checked;
    }

    /// <summary>
    /// Check Change Carcass
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkItemCode_CheckedChanged(object sender, EventArgs e)
    {
      ucItemCode.Visible = chkItemCode.Checked;
    }

    /// <summary>
    /// Value Change Carcass
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucCarcassCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtCarcassCode.Text = ucCarcassCode.SelectedValue;
    }

    /// <summary>
    /// Value Change ItemCode
    /// </summary>
    /// <param name="source"></param>
    /// <param name="args"></param>
    private void ucItemCode_ValueChanged(object source, ValueChangedEventArgs args)
    {
      txtItemCode.Text = ucItemCode.SelectedValue;
    }

    /// <summary>
    /// Check Select All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        row.Cells["Selected"].Value = (chkSelectAll.Checked) ? 1 : 0;
      }
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAdd_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Add datatable
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      // Check Data
      for (int k = 0; k < ultData.Rows.Count; k++)
      {
        UltraGridRow row = ultData.Rows[k];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          if (row.Cells["MaterialCode"].Value.ToString().Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "MaterialCode");
            return;
          }
          else if (DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString()) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty");
            return;
          }
          else if (DBConvert.ParseInt(row.Cells["Urgent"].Value.ToString()) <= 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Urgent");
            return;
          }
          else if (row.Cells["RequestDate"].Value.ToString().Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Request Date");
            return;
          }
          else if (row.Cells["Remark"].Value.ToString().Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Purpose Of Requisition");
            return;
          }
        }
      }

      // End Check Data
      dtDetail = this.CreateDataTable();
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          DataRow rowDetail = dtDetail.NewRow();
          rowDetail["WO"] = DBConvert.ParseInt(row.Cells["WO"].Value.ToString());
          rowDetail["CarcassCode"] = row.Cells["CarcassCode"].Value.ToString();
          rowDetail["ItemCode"] = row.Cells["ItemCode"].Value.ToString();
          rowDetail["Revision"] = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
          rowDetail["MaterialCode"] = row.Cells["MaterialCode"].Value.ToString();
          rowDetail["NameEN"] = row.Cells["NameEN"].Value.ToString();
          rowDetail["NameVN"] = row.Cells["NameVN"].Value.ToString();
          rowDetail["Unit"] = row.Cells["Unit"].Value.ToString();
          rowDetail["Qty"] = DBConvert.ParseDouble(row.Cells["Qty"].Value.ToString());
          rowDetail["Urgent"] = DBConvert.ParseInt(row.Cells["Urgent"].Value.ToString());
          rowDetail["RequestDate"] = DBConvert.ParseDateTime(row.Cells["RequestDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          rowDetail["Remark"] = row.Cells["Remark"].Value.ToString();
          dtDetail.Rows.Add(rowDetail);
        }
      }
      this.CloseTab();
    }

    /// <summary>
    /// Copy
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCopy_Click(object sender, EventArgs e)
    {
      // Check Urgent And Request Date
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Selected"].Value.ToString()) == 1)
        {
          if (ultCBUrgentCopy.Text.Length > 0 && DBConvert.ParseInt(ultCBUrgentCopy.Value.ToString()) > 0)
          {
            row.Cells["Urgent"].Value = DBConvert.ParseInt(ultCBUrgentCopy.Value.ToString());
          }
          if (ultDDRequestDateCopy.Value.ToString().Length > 0 && ultDDRequestDateCopy.Value != null)
          {
            row.Cells["RequestDate"].Value = DBConvert.ParseDateTime(ultDDRequestDateCopy.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }
          if (txtPurposeOfRequisition.Text.Length > 0)
          {
            row.Cells["Remark"].Value = txtPurposeOfRequisition.Text;
          }
        }
      }
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
  }
}
