/*
  Author      : 
  Date        : 17/8/2013
  Description : Template Search Info
  Standard Form: view_GNR_90_002
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_02_029 : MainUserControl
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadStatusCombo();
    }
    /// <summary>
    /// Get Status
    /// </summary>
    private void LoadStatusCombo()
    {
      string commandText = string.Format(@"SELECT Val, Tex FROM VPLNWoCarcassContractOutStatusLis Order By Ord");
      DataTable dtStatus = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Shared.Utility.ControlUtility.LoadUltraCombo(ultdStatus, dtStatus, "Val", "Tex", false);
      ultdStatus.DisplayLayout.Bands[0].Columns[0].Hidden = true;
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      string storeName = "spPLNWOCarcassContractOut_Select";

      DBParameter[] param = new DBParameter[5];
      if (txtWoFrom.Text.Trim().Length > 0)
      {
        param[0] = new DBParameter("@WoFrom", DbType.Int64, DBConvert.ParseLong(txtWoFrom.Text));
      }
      if (txtWoTo.Text.Trim().Length > 0)
      {
        param[1] = new DBParameter("@WoTo", DbType.Int64, DBConvert.ParseLong(txtWoTo.Text));
      }
      if (txtItem.Text.Trim().Length > 0)
      {
        param[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, txtItem.Text.Trim());
      }
      if (txtCarcass.Text.Trim().Length > 0)
      {
        param[3] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, txtCarcass.Text.Trim());
      }
      if (ultdStatus.Value != null && DBConvert.ParseInt(ultdStatus.Value.ToString()) >= 0)
      {
        param[4] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultdStatus.Value.ToString()));
      }
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, param);
      dsSource.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { dsSource.Tables[0].Columns["Pid"] },
                                                  new DataColumn[] { dsSource.Tables[1].Columns["Pid"] }, false));
      ultraGridInformation.DataSource = dsSource;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (ultraGridInformation.Rows[i].Cells["Confirm"].Value.ToString() == "1")
        {
          ultraGridInformation.Rows[i].Appearance.BackColor = Color.Yellow;
        }
        else
        {
          ultraGridInformation.Rows[i].Appearance.BackColor = Color.White;

        }
      }

      lbCount.Text = string.Format("Count: {0}", (dsSource != null ? dsSource.Tables[0].Rows.Count : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtWoFrom.Text = string.Empty;
      txtWoTo.Text = string.Empty;
      txtItem.Text = string.Empty;
      txtCarcass.Text = string.Empty;
      ultdStatus.Rows[0].Selected = true;
      this.txtWoFrom.Focus();
    }
    #endregion function

    #region event
    public viewPLN_02_029()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_02_029_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupBoxSearch.Controls)
      {
        ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      }
      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["Confirm"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      e.Layout.Bands[0].Columns["SubconName"].Header.Caption = "SubName";
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["SupplierPid"].Hidden = true;
      e.Layout.Bands[0].Columns["WoPid"].Header.Caption = "Wo";
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["WoPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SubSupplierPid"].Hidden = true;

      e.Layout.Bands[1].Columns["NetPrice"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[1].Columns["Price Of Material Supply"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[1].Columns["Other Price"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[1].Columns["GrossPrice"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[1].Columns["CarcassCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[1].Columns["Supplier Name"].CellAppearance.BackColor = Color.Yellow;

      e.Layout.Bands[1].Columns["NetPrice"].Format = "###,###";
      e.Layout.Bands[1].Columns["Price Of Material Supply"].Format = "###,###";
      e.Layout.Bands[1].Columns["Other Price"].Format = "###,###";
      e.Layout.Bands[1].Columns["GrossPrice"].Format = "###,###";
      e.Layout.Bands[0].Columns["CreateDate"].Format = "dd/MM/yyyy";
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultraGridInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultraGridInformation.Selected.Rows.Count > 0)
      {
        long carcassContractOutPid = DBConvert.ParseLong(ultraGridInformation.Selected.Rows[0].Cells["Pid"].Value.ToString());
        viewPLN_02_028 view = new viewPLN_02_028();
        view.pid = carcassContractOutPid;
        Shared.Utility.WindowUtinity.ShowView(view, "WO CARCASS CONTRACTOUT DETAIL", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
        this.SearchData();
      }
    }
    #endregion event
  }
}
