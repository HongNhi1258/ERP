/*
  Author      : Nguyen Thanh Binh
  Date        : 26/04/2022
  Description : Choose Loading List
  Standard Form: viewACC_14_003.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_13_009 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewACC_14_003).Assembly);
    public long documentPid = long.MinValue;
    public int currencyPid = int.MinValue;
    public int objectType = int.MinValue;
    public int objectPid = int.MinValue;
    public int actionCode = int.MinValue;
    public DataTable dtAddedDocList;
    public DataRow[] rowSelectedDoc;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spACCARInvoiceOutDetailLoadingList_InitDoc");
      Utility.LoadUltraCombo(ucbCurrency, dsInit.Tables[0], "Pid", "Code", false, new string[] { "Pid", "ExchangeRate" });
      ucbCurrency.Value = this.currencyPid;
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "KeyValue", "Object", false, new string[] { "ObjectPid", "Object", "ObjectType", "KeyValue" });
      ucbObject.Value = string.Format("{0}-{1}", this.objectType, this.objectPid);

      // Set Language
      this.SetLanguage();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 8;
      string storeName = "spACCARInvoiceOutDetailLoadingList_SearchDoc";

      SqlDBParameter[] inputParam = new SqlDBParameter[paramNumber];
      inputParam[0] = new SqlDBParameter("@AddedDocList", SqlDbType.NVarChar, DBConvert.ParseXMLString(this.dtAddedDocList));
      inputParam[1] = new SqlDBParameter("@CurrencyPid", SqlDbType.Int, this.currencyPid);
      inputParam[2] = new SqlDBParameter("@ObjectPid", SqlDbType.Int, this.objectPid);
      inputParam[3] = new SqlDBParameter("@ObjectType", SqlDbType.Int, this.objectType);
      if (txtDocCode.Text.Trim().Length > 0)
      {
        inputParam[4] = new SqlDBParameter("@DocCode", SqlDbType.VarChar, txtDocCode.Text.Trim());
      }
      if (udeFromDate.Value != null)
      {
        inputParam[5] = new SqlDBParameter("@CreateDateFrom", SqlDbType.Date, udeFromDate.Value);
      }
      if (udeToDate.Value != null)
      {
        inputParam[6] = new SqlDBParameter("@CreateDateTo", SqlDbType.Date, udeToDate.Value);
      }
      inputParam[7] = new SqlDBParameter("@DocumentPid", SqlDbType.BigInt, this.documentPid);
      DataTable dtSource = SqlDataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Đếm: {0}", ugdInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;    
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtDocCode.Text = string.Empty;
      udeFromDate.Value = null;
      udeToDate.Value = null;
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

    /// <summary>
    /// Set Auto Add 4 blank before text of button
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetBlankForTextOfButton(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count > 0)
        {
          this.SetBlankForTextOfButton(ctr);
        }
        else if (ctr.GetType().Name == "Button")
        {
          ctr.Text = string.Format("{0}{1}", "    ", ctr.Text);
        }
      }
    }

    private void SetLanguage()
    {
      //btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);     
      //btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      //btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }
    #endregion function

    #region event
    public viewACC_13_009()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_13_009_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(uegSearch);

      //Init Data
      this.InitData();
      this.SearchData();
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

    private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdInformation);
      e.Layout.Bands[0].ColHeaderLines = 2;
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        ugdInformation.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }
      // Hide column      
      e.Layout.Bands[0].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["LoadingPid"].Hidden = true;

      e.Layout.Bands[0].Columns["Selected"].Header.Caption = string.Empty;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;      

      // Set caption column
      e.Layout.Bands[0].Columns["LoadingNo"].Header.Caption = "Số Container";
      e.Layout.Bands[0].Columns["SaleOrder"].Header.Caption = "Mã đơn hàng";     
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Mã sản phẩm";
      e.Layout.Bands[0].Columns["ItemName"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";
      e.Layout.Bands[0].Columns["Revision"].Header.Caption = "Phiên bản";
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["UnitPrice"].Header.Caption = "Đơn giá";
      e.Layout.Bands[0].Columns["TotalPrice"].Header.Caption = "Thành tiền";

      // Set width column
      e.Layout.Bands[0].Columns["LoadingNo"].Width = 80;
      e.Layout.Bands[0].Columns["SaleOrder"].Width = 80;
      e.Layout.Bands[0].Columns["ItemCode"].Width = 80;
      e.Layout.Bands[0].Columns["Revision"].Width = 70;
      e.Layout.Bands[0].Columns["Unit"].Width = 70;
      e.Layout.Bands[0].Columns["Qty"].Width = 70;
      e.Layout.Bands[0].Columns["UnitPrice"].Width = 70;
      e.Layout.Bands[0].Columns["TotalPrice"].Width = 70;
      e.Layout.Bands[0].Columns["Selected"].Width = 40;

      // Read Only
      e.Layout.Bands[0].Columns["SaleOrder"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UnitPrice"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["TotalPrice"].CellActivation = Activation.ActivateOnly;

      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        ugdInformation.Rows[i].Appearance.BackColor = (i % 2 > 0 ? Color.White : Color.LightCyan);
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Data");
    }

    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        //Utility.GetDataForClipboard(ugdInformation);
      }
    }

    private void ugdInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ugdInformation.Selected.Rows.Count > 0 || ugdInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ugdInformation, new Point(e.X, e.Y));
        }
      }
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
      DataTable dtSource = (DataTable)ugdInformation.DataSource;
      this.rowSelectedDoc = dtSource.Select("Selected = 1");
      this.CloseTab();
    }

    private void ugdInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString();
      //if(columnName == "Amount")
      //{
      //  double oldAmount = DBConvert.ParseDouble(e.Cell.Row.Cells["NotYetProposal"].Value);
      //  double newAmount = DBConvert.ParseDouble(e.NewValue);
      //  if(newAmount > oldAmount || newAmount <= 0)
      //  {
      //    e.Cancel = true;
      //    DaiCo.Shared.Utility.WindowUtinity.ShowMessageErrorFromText(string.Format("Số tiền đề nghị phải lớn hơn 0 và nhỏ hơn hoặc bằng {0}", oldAmount));
      //  }  
      //}  
    }
    #endregion event
  }
}
