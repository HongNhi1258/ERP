/*
  Author      : HangNguyen
  Date        : 08-03-2011
  Description : List Supplier 
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
  public partial class viewPUR_02_001 : MainUserControl
  {
    #region variable
    #endregion variable

    #region Init Data
    public viewPUR_02_001()
    {
      InitializeComponent();
    }

    private void viewPUR_02_001_Load(object sender, EventArgs e)
    {
      this.LoadComboTradeType();
      this.LoadComboStatus();

    }

    private void ultListSupplier_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      e.Layout.Bands[0].Columns["SupplierCode"].Header.Caption = "Supplier Code";
      e.Layout.Bands[0].Columns["EnglishName"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["VietnameseName"].Header.Caption = "Vietnamese Name";
      e.Layout.Bands[0].Columns["TradeName"].Header.Caption = "Trade Name";
      e.Layout.Bands[0].Columns["TradeType"].Header.Caption = "Trade Type";
      e.Layout.Bands[0].Columns["PersonInChange"].Header.Caption = "Person In Charge";
      e.Layout.Bands[0].Columns["Confirm"].Header.Caption = "Status";
      e.Layout.Bands[0].Columns["IntroducePerson"].Hidden = true;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["SupplierID"].Hidden = true;
      e.Layout.Bands[1].Columns["ContactName"].Header.Caption = "Contact Name";
      e.Layout.Bands[1].Columns["ContactPostion"].Header.Caption = "Contact Postion";
      e.Layout.Bands[1].Columns["ContactMobile"].Header.Caption = "Contact Mobile";
      e.Layout.Bands[1].Columns["ContactEmail"].Header.Caption = "Contact Email";
      e.Layout.Bands[1].Columns["Sex"].Header.Caption = "Contact Sex";
    }

    #endregion Init Data

    #region Load Data

    /// <summary>
    /// Load UltraCombo Status 
    /// </summary>
    private void LoadComboStatus()
    {
      string commandText = string.Empty;
      commandText += " SELECT NULL ID, 'All' Name";
      commandText += " UNION ";
      commandText += " SELECT 0 ID, 'New' Name";
      commandText += " UNION";
      commandText += " SELECT 1 ID, 'Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 2 ID, 'Purchas Manager Confirmed' Name";
      commandText += " UNION";
      commandText += " SELECT 3 ID, 'Not Actived' Name";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBStatus.DataSource = dtSource;
      ultCBStatus.DisplayMember = "Name";
      ultCBStatus.ValueMember = "ID";
      ultCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBStatus.DisplayLayout.Bands[0].Columns["Name"].Width = 150;
      ultCBStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;
    }
    /// <summary>
    /// Load UltraCombo Trade Type
    /// </summary>
    private void LoadComboTradeType()
    {
      string commandText = string.Format(@"SELECT Code, Value + ISNULL(' - ' + Description, '') Name FROM TblBOMCodeMaster WHERE [Group] = {0} And DeleteFlag = 0 ORDER BY Sort", ConstantClass.GROUP_PUR_TRADETYPE);
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultCBTradeType.DataSource = dtSource;
      ultCBTradeType.DisplayMember = "Name";
      ultCBTradeType.ValueMember = "Code";
      ultCBTradeType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCBTradeType.DisplayLayout.Bands[0].Columns["Name"].Width = 450;
      ultCBTradeType.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadGridListSupplier()
    {
      DBParameter[] inputParam = new DBParameter[7];
      string text = txtSupplierCode.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[0] = new DBParameter("@SupplierCode", DbType.AnsiString, "%" + text.Replace("'", "''") + "%");
      }
      text = txtEnglishName.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[1] = new DBParameter("@EnglishName", DbType.AnsiString, "%" + text.Replace("'", "''") + "%");
      }
      text = txtVietnameseName.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[2] = new DBParameter("@VietnameseName", DbType.String, "%" + text.Replace("'", "''") + "%");
      }
      text = txtTradeName.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[3] = new DBParameter("@TradeName", DbType.AnsiString, "%" + text.Replace("'", "''") + "%");
      }

      if (ultCBTradeType.Value != null)
      {
        inputParam[4] = new DBParameter("@TradeType", DbType.Int32, DBConvert.ParseInt(ultCBTradeType.Value.ToString()));
      }
      if (ultCBStatus.Value != null)
      {
        if (DBConvert.ParseInt(ultCBStatus.Value.ToString()) != int.MinValue)
        {
          inputParam[5] = new DBParameter("@Status", DbType.Int32, DBConvert.ParseInt(ultCBStatus.Value.ToString()));
        }
      }
      text = txtTaxNo.Text.Trim();
      if (text.Length > 0)
      {
        inputParam[6] = new DBParameter("@TaxNo", DbType.AnsiString, "%" + text.Replace("'", "''") + "%");
      }

      string storeName = "spPURListSupplier";
      DataSet dataSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      DataSet dataSet = Shared.Utility.CreateDataSet.Supplier_Contact();
      try
      {
        dataSet.Tables["dtParent"].Merge(dataSource.Tables[0]);
      }
      catch { }
      try
      {
        dataSet.Tables["dtChild"].Merge(dataSource.Tables[1]);
      }
      catch { }
      ultListSupplier.DataSource = dataSet;

      for (int i = 0; i < ultListSupplier.Rows.Count; i++)
      {
        string status = ultListSupplier.Rows[i].Cells["Confirm"].Value.ToString();
        if (string.Compare(status, "Confirmed") == 0)
        {
          ultListSupplier.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        else if (string.Compare(status, "Purchase Manager Confirmed") == 0)
        {
          ultListSupplier.Rows[i].CellAppearance.BackColor = Color.CornflowerBlue;
        }
        else if (string.Compare(status, "Not Actived") == 0)
        {
          ultListSupplier.Rows[i].CellAppearance.BackColor = Color.Lime;
        }
      }
    }

    #endregion Load Data

    #region Event

    /// <summary>
    /// New Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPUR_02_002 view = new viewPUR_02_002();
      WindowUtinity.ShowView(view, "SUPPLIER INFORMATION", false, Shared.Utility.ViewState.MainWindow);
      this.LoadGridListSupplier();
    }

    /// <summary>
    /// Search 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.LoadGridListSupplier();
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultListSupplier_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
      {
        return;
      }
      try
      {
        viewPUR_02_002 view = new viewPUR_02_002();
        UltraGridRow row = ultListSupplier.Selected.Rows[0];
        if (row != null)
        {
          row = (row.ParentRow != null) ? row.ParentRow : row;
          view.supplierPid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          Shared.Utility.WindowUtinity.ShowView(view, "SUPPLIER INFORMATION", false, Shared.Utility.ViewState.MainWindow);
        }
      }
      catch { }
      this.LoadGridListSupplier();
    }

    /// <summary>
    /// Not Actived Supplier
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNotActived_Click(object sender, EventArgs e)
    {
      string storeName = "spPURSupplierInfoStatusNotActived_Update";
      for (int i = 0; i < ultListSupplier.Rows.Count; i++)
      {
        UltraGridRow row = ultListSupplier.Rows[i];
        if (String.Compare(row.Cells["Selected"].Text, "1") == 0)
        {
          DBParameter[] inputParam = new DBParameter[1];
          inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
          DBParameter[] outputParam = new DBParameter[1];
          outputParam[0] = new DBParameter("@Result", DbType.Int32, int.MinValue);

          DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
          int result = DBConvert.ParseInt(outputParam[0].Value.ToString());
          if (result <= 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Not Active Errors!");
            return;
          }
        }
      }
      WindowUtinity.ShowMessageSuccessFromText("Not Active Successful!");
      // Load Data
      this.LoadGridListSupplier();
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.LoadGridListSupplier();
        btnSearch.Enabled = true;
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
