/*
  Author      : Nguyen Huynh Quoc Tuan
  Date        : 9/1/2015
  Description : List Reload BOM
  Standard Form: viewPLN_30_002.cs
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
  public partial class viewPLN_30_002 : MainUserControl
  {
    #region field
    public long transactionPid = long.MinValue;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadCbCreateBy();
      //ControlUtility.LoadUltraDropDown();
    }

    private void LoadCbCreateBy()
    {
      string cm = string.Format(@"SELECT DISTINCT ID_NhanVien,CAST(ID_NhanVien AS varchar(16))+' - '+ HoNV + ' '+TenNV AS HoTen 
                                  FROM VHRNhanVien NV 
                                  WHERE Resigned = 0 AND ID_NhanVien > 0
                                  ORDER BY ID_NhanVien");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      ControlUtility.LoadUltraCombo(ultcbCreateBy, dt, "ID_NhanVien", "HoTen", false, "ID_NhanVien");
    }


    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 7;
      string storeName = "spPLNReloadMatrialBOM";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (txtTransaction.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@TransactionCode", DbType.String, txtTransaction.Text.Trim());
      }

      if (ultcbCreateBy.Value != null && DBConvert.ParseLong(ultcbCreateBy.Value.ToString()) != long.MinValue)
      {
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int64, DBConvert.ParseLong(ultcbCreateBy.Value.ToString()));
      }

      if (ultdtCreateDateFrom.Value != null && DBConvert.ParseDateTime(ultdtCreateDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        inputParam[2] = new DBParameter("@CreateDateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultdtCreateDateFrom.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }
      if (ultdtCreateDateTo.Value != null && DBConvert.ParseDateTime(ultdtCreateDateTo.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME) != DateTime.MinValue)
      {
        inputParam[3] = new DBParameter("@CreateDateTo", DbType.DateTime, DBConvert.ParseDateTime(ultdtCreateDateTo.Value.ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME));
      }

      if (DBConvert.ParseLong(txtWO.Text.Trim()) > 0)
      {
        inputParam[4] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(txtWO.Text.Trim()));
      }

      if (txtCarcass.Text.Trim().Length > 0)
      {
        inputParam[5] = new DBParameter("@CarcassCode", DbType.String, txtCarcass.Text.Trim());
      }

      if (txtItemCode.Text.Trim().Length > 0)
      {
        inputParam[6] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text.Trim());
      }

      DataSet dtSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      dtSource.Relations.Add(new DataRelation("Parent_Child", dtSource.Tables[0].Columns["Pid"], dtSource.Tables[1].Columns["TranPid"], false));

      ultraGridInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtItemCode.Text = "";
      txtCarcass.Text = "";
      txtWO.Text = "";
      txtTransaction.Text = "";
      ultdtCreateDateFrom.Value = "";
      ultdtCreateDateTo.Value = "";
      ultcbCreateBy.Value = "";
      txtTransaction.Focus();
    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }
    #endregion function

    #region event
    public viewPLN_30_002()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_30_002_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

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
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      for (int j = 0; j < e.Layout.Bands[1].Columns.Count; j++)
      {
        Type colType = e.Layout.Bands[1].Columns[j].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[j].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[1].Columns[j].Format = "#,##0.##";
        }
        e.Layout.Bands[1].Columns[j].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["TranetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["TranPid"].Hidden = true;
      e.Layout.Bands[1].Columns["TranetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["TranSubPid"].Hidden = true;
    }

    /// <summary>
    /// Double click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private void ultraGridInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        UltraGridRow row = ultraGridInformation.Selected.Rows[0];
        viewPLN_30_001 uc = new viewPLN_30_001();
        long pid = long.MinValue;
        try
        {
          pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        }
        catch
        {
        }
        if (pid > 0)
        {
          uc.transactionPid = pid;
          Shared.Utility.WindowUtinity.ShowView(uc, "Reload BOM", false, Shared.Utility.ViewState.MainWindow);
        }
      }
      catch
      { }

    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        ControlUtility.GetDataForClipboard(ultraGridInformation);
      }
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultraGridInformation.Selected.Rows.Count > 0 || ultraGridInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultraGridInformation, new Point(e.X, e.Y));
        }
      }
    }

    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPLN_30_001 view = new viewPLN_30_001();
      Shared.Utility.WindowUtinity.ShowView(view, "Reload BOM", false, Shared.Utility.ViewState.MainWindow);
    }
    #endregion event

  }
}
