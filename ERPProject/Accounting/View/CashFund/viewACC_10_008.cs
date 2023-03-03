/*
  Author      : Nguyen Thanh Binh
  Date        : 22/04/2021
  Description : Select payment advance or direct
  Standard Form: view_SearchInfo.cs
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
using System.Resources;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewACC_10_008 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    private int docTypePid = ConstantClass.Payment_Voucher;
    public DataTable dtAdvance = null;
    public int objectPid = int.MinValue;
    public int currency = int.MinValue;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      string cmd = string.Empty;
      cmd = string.Format(@"SELECT ObjectPid, CAST(ObjectCode as nvarchar) + ' - ' + [ObjectName] [Object], [ObjectName], ObjectType
	                        FROM VACCObjectList");
      DataTable dtObject = DataBaseAccess.SearchCommandTextDataTable(cmd);
      Utility.LoadUltraCombo(ucbObject, dtObject, "ObjectPid", "Object", false, new string[] { "ObjectPid", "ObjectName", "ObjectType" });
      cmd = string.Format(@"SELECT Pid, Code
	                          FROM TblPURCurrencyInfo");
      DataTable dtCurrency = DataBaseAccess.SearchCommandTextDataTable(cmd);
      Utility.LoadUltraCombo(ucbCurrency, dtCurrency, "Pid", "Code", false, "Pid");
      if (this.objectPid > 0)
      {
        ucbObject.Value = this.objectPid;
      }
      if (this.currency > 0)
      {
        ucbCurrency.Value = this.currency;
      }
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
      string storeName = "spACCPaymentVoucherForDirect_Search";

      DBParameter[] inputParam = new DBParameter[paramNumber];

      if (ucbObject.SelectedRow != null)
      {
        inputParam[0] = new DBParameter("@ObjectPid", DbType.Int32, DBConvert.ParseInt(ucbObject.Value));
      }
      if (ucbCurrency.SelectedRow != null)
      {
        inputParam[1] = new DBParameter("@Currency", DbType.Int32, DBConvert.ParseInt(ucbCurrency.Value));
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;

      lbCount.Text = string.Format("Count: {0}", ugdInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
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
      btnSearch.Text = rm.GetString("Search", ConstantClass.CULTURE);
      btnExportExcel.Text = rm.GetString("Export", ConstantClass.CULTURE);
      btnClose.Text = rm.GetString("Close", ConstantClass.CULTURE);

      this.SetBlankForTextOfButton(this);
    }

    /// <summary>
    /// Create Data Table
    /// </summary>
    /// <returns></returns>
    private DataTable CreateDataTable()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Pid", typeof(System.Int64));
      dt.Columns.Add("CurrencyPid", typeof(System.Int32));
      dt.Columns.Add("DetailDesc", typeof(System.String));
      dt.Columns.Add("DebitPid", typeof(System.Int32));
      dt.Columns.Add("ProjectPid", typeof(System.Int32));
      dt.Columns.Add("SegmentPid", typeof(System.Int32));
      dt.Columns.Add("ObjectPid", typeof(System.Int32));
      dt.Columns.Add("Remain", typeof(System.Double));
      dt.Columns.Add("VATPercent", typeof(System.Double));
      dt.Columns.Add("VATNumber", typeof(System.String));
      dt.Columns.Add("VATDate", typeof(System.DateTime));
      dt.Columns.Add("VATInvoiceNo", typeof(System.String));
      dt.Columns.Add("VATFormNo", typeof(System.String));
      dt.Columns.Add("VATSymbol", typeof(System.String));
      dt.Columns.Add("InputVATDocType", typeof(System.Int32));
      dt.Columns.Add("UnCostConstructionPid", typeof(System.Int32));
      dt.Columns.Add("CostObjectPid", typeof(System.Int32));
      dt.Columns.Add("EInvoiceTypePid", typeof(System.Int32));
      dt.Columns.Add("CostCenterPid", typeof(System.Int32));
      return dt;
    }

    #endregion function

    #region event
    public viewACC_10_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewACC_10_008_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(uegSearch);

      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
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
      e.Layout.AutoFitColumns = false;
      e.Layout.UseFixedHeaders = true;
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;
      // Allow update, delete, add new

      // Read only
      for (int i = 0; i < ugdInformation.DisplayLayout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[0].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["Select"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["SegmentPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ObjectPid"].Hidden = true;
      e.Layout.Bands[0].Columns["ProjectPid"].Hidden = true;
      e.Layout.Bands[0].Columns["DebitPid"].Hidden = true;
      e.Layout.Bands[0].Columns["CreditPid"].Hidden = true;
      e.Layout.Bands[0].Columns["UnCostConstructionPid"].Hidden = true;
      e.Layout.Bands[0].Columns["CostObjectPid"].Hidden = true;
      e.Layout.Bands[0].Columns["EInvoiceTypePid"].Hidden = true;
      e.Layout.Bands[0].Columns["CostCenterPid"].Hidden = true;
      e.Layout.Bands[0].Columns["InputVATDocType"].Hidden = true;

      e.Layout.Bands[0].Columns["RequestCode"].Header.Caption = "Mã chứng từ";
      e.Layout.Bands[0].Columns["Currency"].Header.Caption = "Tiền Tệ";
      e.Layout.Bands[0].Columns["DetailDesc"].Header.Caption = "Diễn giải";
      e.Layout.Bands[0].Columns["Object"].Header.Caption = "Đối tượng";
      e.Layout.Bands[0].Columns["Amount"].Header.Caption = "Tiền tạm ứng";
      e.Layout.Bands[0].Columns["VATPercent"].Header.Caption = "% thuế";
      e.Layout.Bands[0].Columns["VATAmount"].Header.Caption = "Tiền thuế";
      e.Layout.Bands[0].Columns["TotalAmount"].Header.Caption = "Tổng tiền";
      e.Layout.Bands[0].Columns["Paid"].Header.Caption = "Tiền đã chi";
      e.Layout.Bands[0].Columns["Remain"].Header.Caption = "Tiền còn lại";
      e.Layout.Bands[0].Columns["TaxNumber"].Header.Caption = "MST";
      e.Layout.Bands[0].Columns["VATDate"].Header.Caption = "Ngày hóa đơn";
      e.Layout.Bands[0].Columns["VATInvoiceNo"].Header.Caption = "Số hóa đơn";
      e.Layout.Bands[0].Columns["VATFormNo"].Header.Caption = "Mẫu số";
      e.Layout.Bands[0].Columns["VATSymbol"].Header.Caption = "Ký hiệu";
      e.Layout.Bands[0].Columns["Select"].Header.Caption = "Chọn";
      // Set Column Style
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Danh sách phiếu chi");
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

    private void ugdInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {

    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
      //check data
      for (int k = 0; k < ugdInformation.Rows.Count; k++)
      {
        UltraGridRow row = ugdInformation.Rows[k];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          for (int j = 0 + k; j < ugdInformation.Rows.Count; j++)
          {
            UltraGridRow row2 = ugdInformation.Rows[k];
            if (DBConvert.ParseInt(row2.Cells["ObjectPid"].Value.ToString()) != DBConvert.ParseInt(row.Cells["ObjectPid"].Value.ToString()))
            {
              WindowUtinity.ShowMessageErrorFromText("Chỉ được chọn 1 đối tượng.");
              return;
            }
            if (DBConvert.ParseInt(row2.Cells["CurrencyPid"].Value.ToString()) != DBConvert.ParseInt(row.Cells["CurrencyPid"].Value.ToString()))
            {
              WindowUtinity.ShowMessageErrorFromText("Chỉ được chọn 1 loại tiền tệ.");
              return;
            }
          }
        }
      }

      dtAdvance = this.CreateDataTable();
      for (int i = 0; i < ugdInformation.Rows.Count; i++)
      {
        UltraGridRow row = ugdInformation.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Select"].Value.ToString()) == 1)
        {
          DataRow rowDetail = dtAdvance.NewRow();
          rowDetail["Pid"] = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
          rowDetail["CurrencyPid"] = DBConvert.ParseInt(row.Cells["CurrencyPid"].Value.ToString());
          rowDetail["DetailDesc"] = row.Cells["DetailDesc"].Value.ToString();
          rowDetail["DebitPid"] = DBConvert.ParseInt(row.Cells["DebitPid"].Value.ToString());
          if (DBConvert.ParseInt(row.Cells["ProjectPid"].Value.ToString()) > 0)
          {
            rowDetail["ProjectPid"] = DBConvert.ParseInt(row.Cells["ProjectPid"].Value.ToString());
          }
          if (DBConvert.ParseInt(row.Cells["SegmentPid"].Value.ToString()) > 0)
          {
            rowDetail["SegmentPid"] = DBConvert.ParseInt(row.Cells["SegmentPid"].Value.ToString());
          }
          rowDetail["ObjectPid"] = DBConvert.ParseInt(row.Cells["ObjectPid"].Value.ToString());
          rowDetail["Remain"] = DBConvert.ParseDouble(row.Cells["Remain"].Value.ToString());
          rowDetail["VATPercent"] = DBConvert.ParseDouble(row.Cells["VATPercent"].Value.ToString());
          rowDetail["VATNumber"] = row.Cells["TaxNumber"].Value.ToString();
          rowDetail["VATDate"] = DBConvert.ParseDateTime(row.Cells["VATDate"].Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          rowDetail["VATInvoiceNo"] = row.Cells["VATInvoiceNo"].Value.ToString();
          rowDetail["VATFormNo"] = row.Cells["VATFormNo"].Value.ToString();
          rowDetail["VATSymbol"] = row.Cells["VATSymbol"].Value.ToString();
          if (DBConvert.ParseInt(row.Cells["InputVATDocType"].Value.ToString()) > 0)
          {
            rowDetail["InputVATDocType"] = DBConvert.ParseInt(row.Cells["InputVATDocType"].Value.ToString());
          }
          if (DBConvert.ParseInt(row.Cells["UnCostConstructionPid"].Value.ToString()) > 0)
          {
            rowDetail["UnCostConstructionPid"] = DBConvert.ParseInt(row.Cells["UnCostConstructionPid"].Value.ToString());
          }
          if (DBConvert.ParseInt(row.Cells["CostObjectPid"].Value.ToString()) > 0)
          {
            rowDetail["CostObjectPid"] = DBConvert.ParseInt(row.Cells["CostObjectPid"].Value.ToString());
          }
          if (DBConvert.ParseInt(row.Cells["EInvoiceTypePid"].Value.ToString()) > 0)
          {
            rowDetail["EInvoiceTypePid"] = DBConvert.ParseInt(row.Cells["EInvoiceTypePid"].Value.ToString());
          }
          if (DBConvert.ParseInt(row.Cells["CostCenterPid"].Value.ToString()) > 0)
          {
            rowDetail["CostCenterPid"] = DBConvert.ParseInt(row.Cells["CostCenterPid"].Value.ToString());
          }
          dtAdvance.Rows.Add(rowDetail);
        }
      }
      this.CloseTab();
    }
    #endregion event
  }
}
