/*
  Author      : 
  Date        : 31/05/2013
  Description : List Request Special ID (Veneer)
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

namespace DaiCo.Planning
{
  public partial class viewPLN_21_002 : MainUserControl
  {
    #region Field
    #endregion Field

    #region Init
    public viewPLN_21_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_21_002_Load(object sender, EventArgs e)
    {
      this.LoadInit();
    }

    /// <summary>
    /// Load Init
    /// </summary>
    private void LoadInit()
    {
      drpDateFrom.Value = DateTime.Today.AddDays(-7);

      // Load UltraCombo Status
      this.LoadComboStatus();

      // Load UltraCombo Create By
      this.LoadComboCreateBy();
    }

    /// <summary>
    /// Load UltraCombo Create By
    /// </summary>
    private void LoadComboCreateBy()
    {
      string commandText = string.Empty;
      commandText += " SELECT ID_NhanVien, CONVERT(VARCHAR, ID_NhanVien) + ' - ' + HoNV + ' ' + TenNV Name";
      commandText += " FROM VHRNhanVien ";
      commandText += " WHERE Department = 'PLA' AND Resigned = 0";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultCBCreateBy.DataSource = dtSource;
      ultCBCreateBy.DisplayMember = "Name";
      ultCBCreateBy.ValueMember = "ID_NhanVien";
      ultCBCreateBy.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBCreateBy.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBCreateBy.DisplayLayout.Bands[0].Columns["ID_NhanVien"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Status (0: New / 1: Confirm)
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'PLN Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Finished' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultStatus.DataSource = dtSource;
      ultStatus.DisplayMember = "Name";
      ultStatus.ValueMember = "ID";
      ultStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultStatus.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[14];
      // Receiving Code
      if (txtRequestFrom.Text.Length > 0)
      {
        input[0] = new DBParameter("@RESNoFrom", DbType.String, txtRequestFrom.Text.Trim());
      }

      if (txtRequestTo.Text.Length > 0)
      {
        input[1] = new DBParameter("@RESNoTo", DbType.String, txtRequestTo.Text.Trim());
      }

      // DateFrom
      if (drpDateFrom.Value != null)
      {
        DateTime prDateFrom = DateTime.MinValue;
        prDateFrom = (DateTime)drpDateFrom.Value;
        input[2] = new DBParameter("@DateFrom", DbType.DateTime, prDateFrom);
      }

      // DateTo
      if (drpDateTo.Value != null)
      {
        DateTime prDateTo = DateTime.MinValue;
        prDateTo = (DateTime)drpDateTo.Value;
        prDateTo = prDateTo != (DateTime.MaxValue) ? prDateTo.AddDays(1) : prDateTo;
        input[3] = new DBParameter("@DateTo", DbType.DateTime, prDateTo);
      }

      // CreateBy
      if (ultCBCreateBy.Value != null)
      {
        input[4] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(ultCBCreateBy.Value.ToString()));
      }

      // Status
      if (ultStatus.Value != null && DBConvert.ParseInt(ultStatus.Value.ToString()) != int.MinValue)
      {
        input[5] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultStatus.Value.ToString()));
      }

      // WO
      if (txtWO.Text.Trim().Length > 0 && DBConvert.ParseInt(txtWO.Text) > 0)
      {
        input[6] = new DBParameter("@WO", DbType.String, DBConvert.ParseInt(txtWO.Text));
      }

      // Carcas
      if (txtCarcass.Text.Trim().Length > 0)
      {
        input[7] = new DBParameter("@Carcass", DbType.String, txtCarcass.Text.Trim());
      }

      // LotNoId
      if (txtLotNoId.Text.Trim().Length > 0)
      {
        input[8] = new DBParameter("@LotNoId", DbType.String, txtLotNoId.Text.Trim());
      }

      // Category
      if (txtMaterial.Text.Trim().Length > 0)
      {
        input[9] = new DBParameter("@Material", DbType.String, txtMaterial.Text.Trim());
      }

      // Length
      if (DBConvert.ParseDouble(txtLengthFrom.Text) != double.MinValue)
      {
        input[10] = new DBParameter("@Length", DbType.Double, DBConvert.ParseDouble(txtLengthFrom.Text));
      }

      // Width
      if (DBConvert.ParseDouble(txtWidthFrom.Text) != double.MinValue)
      {
        input[11] = new DBParameter("@Width", DbType.Double, DBConvert.ParseDouble(txtWidthFrom.Text));
      }

      // Length To
      if (DBConvert.ParseDouble(txtLengthTo.Text) != double.MinValue)
      {
        input[12] = new DBParameter("@LengthTo", DbType.Double, DBConvert.ParseDouble(txtLengthTo.Text));
      }

      // Width To
      if (DBConvert.ParseDouble(txtWidthTo.Text) != double.MinValue)
      {
        input[13] = new DBParameter("@WidthTo", DbType.Double, DBConvert.ParseDouble(txtWidthTo.Text));
      }

      DataSet ds = DataBaseAccess.SearchStoreProcedure("spPLNVeneerListRequestSpecialID_Select", input);
      if (ds != null)
      {
        DataSet dsSource = this.CreateDataSet();
        dsSource.Tables["dtParent"].Merge(ds.Tables[0]);
        dsSource.Tables["dtChild"].Merge(ds.Tables[1]);
        ultData.DataSource = dsSource;

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          string status = ultData.Rows[i].Cells["Status"].Value.ToString();
          if (string.Compare(ultData.Rows[i].Cells["Status"].Value.ToString(), "PLN Confirmed", true) == 0)
          {
            ultData.Rows[i].Appearance.BackColor = Color.Pink;
          }
          else if (string.Compare(ultData.Rows[i].Cells["Status"].Value.ToString(), "Finished", true) == 0)
          {
            ultData.Rows[i].Appearance.BackColor = Color.LightGreen;
          }
        }
      }
      // Enable button search
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Enter Search
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
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
    /// Create DataSet
    /// </summary>
    /// <returns></returns>
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("RequestNo", typeof(System.String));
      taParent.Columns.Add("CreateBy", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.Int32));
      taChild.Columns.Add("CarcassCode", typeof(System.String));
      taChild.Columns.Add("LotNoId", typeof(System.String));
      taChild.Columns.Add("MaterialCode", typeof(System.String));
      taChild.Columns.Add("MaterialNameVn", typeof(System.String));
      taChild.Columns.Add("Dimension", typeof(System.String));
      taChild.Columns.Add("WONew", typeof(System.Int32));
      taChild.Columns.Add("CarcassNew", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["Pid"], taChild.Columns["Pid"], false));
      return ds;
    }
    #endregion Function

    #region Event

    /// <summary>
    /// Clear
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      txtRequestFrom.Text = string.Empty;
      txtRequestTo.Text = string.Empty;
      ultStatus.Text = string.Empty;
      ultCBCreateBy.Text = string.Empty;
      txtWO.Text = string.Empty;
      txtCarcass.Text = string.Empty;
      txtLotNoId.Text = string.Empty;
      txtMaterial.Text = string.Empty;
      txtLengthFrom.Text = string.Empty;
      txtLengthTo.Text = string.Empty;
      txtWidthFrom.Text = string.Empty;
      txtWidthTo.Text = string.Empty;
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
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Dimension"].Header.Caption = "Dimension(LxW)";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[1].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowUpdate = DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
      UltraGridRow row = (ultData.Selected.Rows[0].ParentRow == null) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      string status = row.Cells["Status"].Value.ToString();

      if (string.Compare(this.ViewParam, "WHD", true) == 0)
      {
        if (string.Compare(status, "New", true) != 0)
        {
          viewPLN_21_001 uc = new viewPLN_21_001();
          uc.pid = pid;
          uc.flagWHD = true;
          WindowUtinity.ShowView(uc, "UPDATE REQUEST SPECIAL ID", false, ViewState.MainWindow);
        }
      }
      else
      {
        viewPLN_21_001 uc = new viewPLN_21_001();
        uc.pid = pid;
        uc.flagWHD = false;
        WindowUtinity.ShowView(uc, "UPDATE REQUEST SPECIAL ID", false, ViewState.MainWindow);
      }
      this.Search();
    }

    private void btnNewRequest_Click(object sender, EventArgs e)
    {
      viewPLN_21_001 view = new viewPLN_21_001();
      Shared.Utility.WindowUtinity.ShowView(view, "REQUEST SPECIAL ID INFORMATION", false, Shared.Utility.ViewState.MainWindow);
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
