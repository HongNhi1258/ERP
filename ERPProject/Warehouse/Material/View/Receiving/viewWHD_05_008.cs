/*
  Author      : Nguyen Thanh Binh
  Date        : 16/04/2021
  Description : Load PO to make receiving note
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
  public partial class viewWHD_05_008 : MainUserControl
  {
    #region field
    ResourceManager rm = new ResourceManager(ConstantClass.LANGUAGE_RESOURCE, typeof(viewHRD_02_003).Assembly);
    public DataTable dtPODetail = null;
    private bool isDuplicateObject = false;
    private bool isDuplicateCurrency = false;
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      DataSet dsInit = SqlDataBaseAccess.SearchStoreProcedure("spWHDMaterialReceivingLoadPO_Init");
      Utility.LoadUltraCombo(ucbCreateBy, dsInit.Tables[0], "EmployeePid", "Employee", false, "EmployeePid");
      Utility.LoadUltraCombo(ucbObject, dsInit.Tables[1], "Pid", "Object", false, new string[] { "Pid", "ObjectType" });
      udtFromDate.Value = null;
      udtToDate.Value = null;

      // Set Language
      //this.SetLanguage();
    }

    private bool CheckValid()
    {      
      // Have to choose 1 row
      bool isRowSelected = false;
      foreach (UltraGridRow gRow in ugdInformation.Rows)
      {
        if ((bool)gRow.Cells["Selected"].Value == true)
        {
          isRowSelected = true;
        }
      }
      if (!isRowSelected)
      {        
        WindowUtinity.ShowMessageErrorFromText("Bạn chưa chọn hóa đơn.");
        return false;
      }

      // Check Supllier Duplicate
      this.CheckObjectDuplicate(ugdInformation);
      if (isDuplicateObject)
      {
        WindowUtinity.ShowMessageErrorFromText("Chỉ được chọn hóa đơn cùng đối tượng (NCC).");
        return false;
      }

      // Check Currency Duplicate
      this.CheckCurrency(ugdInformation);
      if (isDuplicateCurrency)
      {
        WindowUtinity.ShowMessageErrorFromText("Chỉ được chọn hóa đơn cùng tiền tệ.");
        return false;
      }

      return true;
    }
    /// <summary>
    /// check duplicate object
    /// </summary>
    /// <param name="ultG"></param>
    private void CheckObjectDuplicate(UltraGrid ultG)
    {
      isDuplicateObject = false;
      for (int k = 0; k < ultG.Rows.Count; k++)
      {
        ultG.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultG.Rows.Count; i++)
      {
        if ((bool)ultG.Rows[i].Cells["Selected"].Value == true)
        {
          int objectID = DBConvert.ParseInt(ultG.Rows[i].Cells["SupplierPid"].Value.ToString());
          for (int j = i + 1; j < ultG.Rows.Count; j++)
          {
            if ((bool)ultG.Rows[j].Cells["Selected"].Value == true)
            {
              int objectCompareID = DBConvert.ParseInt(ultG.Rows[j].Cells["SupplierPid"].Value.ToString());
              if (objectID != objectCompareID)
              {
                ultG.Rows[i].CellAppearance.BackColor = Color.Yellow;
                ultG.Rows[j].CellAppearance.BackColor = Color.Yellow;
                this.isDuplicateObject = true;
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// check duplicate object
    /// </summary>
    /// <param name="ultG"></param>
    private void CheckCurrency(UltraGrid ultG)
    {
      isDuplicateCurrency = false;
      for (int k = 0; k < ultG.Rows.Count; k++)
      {
        ultG.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultG.Rows.Count; i++)
      {
        if ((bool)ultG.Rows[i].Cells["Selected"].Value == true)
        {
          int currencyID = DBConvert.ParseInt(ultG.Rows[i].Cells["CurrencyPid"].Value.ToString());
          for (int j = i + 1; j < ultG.Rows.Count; j++)
          {
            if ((bool)ultG.Rows[j].Cells["Selected"].Value == true)
            {
              int currencyCompareID = DBConvert.ParseInt(ultG.Rows[j].Cells["CurrencyPid"].Value.ToString());
              if (currencyID != currencyCompareID)
              {
                ultG.Rows[i].CellAppearance.BackColor = Color.Yellow;
                ultG.Rows[j].CellAppearance.BackColor = Color.Yellow;
                this.isDuplicateCurrency = true;
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 5;
      string storeName = "spWHDMaterialInStore_LoadPO";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (txtPONo.Text.ToString().Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@PONo", DbType.String, txtPONo.Text.ToString());
      }
      if (ucbCreateBy.SelectedRow != null)
      {
        inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, DBConvert.ParseInt(ucbCreateBy.Value));
      }
      if (udtFromDate.Value != null)
      {
        inputParam[2] = new DBParameter("@FromDate", DbType.DateTime, DBConvert.ParseDateTime(udtFromDate.Value));
      }
      if (udtToDate.Value != null)
      {
        inputParam[3] = new DBParameter("@ToDate", DbType.DateTime, DBConvert.ParseDateTime(udtToDate.Value));
      }
      if (ucbObject.SelectedRow != null)
      {
        inputParam[4] = new DBParameter("@Object", DbType.Int32, DBConvert.ParseInt(ucbObject.Value));
      }


      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(storeName, inputParam);
      ugdInformation.DataSource = dtSource;
      lbCount.Text = string.Format(String.Format("Đếm: {0}", ugdInformation.Rows.FilteredInRowCount > 0 ? ugdInformation.Rows.FilteredInRowCount : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtPONo.Text = string.Empty;
      ucbCreateBy.Value = null;
      udtFromDate.Value = null;
      udtToDate.Value = null;
      ucbObject.Value = null;
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

    private DataTable CreatePODetail()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("PONo", typeof(System.String));
      dt.Columns.Add("MaterialCode", typeof(System.String));
      dt.Columns.Add("Qty", typeof(System.Double));
      return dt;
    }

    #endregion function

    #region event
    public viewWHD_05_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWHD_05_008_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(uegSearch);
      this.SetBlankForTextOfButton(this);
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

    private void ugdInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ugdInformation);      
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Override.RowSelectorWidth = 32;
      // Add Column Selected
      if (!e.Layout.Bands[0].Columns.Exists("Selected"))
      {
        UltraGridColumn checkedCol = e.Layout.Bands[0].Columns.Add();
        checkedCol.Key = "Selected";
        checkedCol.Header.Caption = "Selected";
        checkedCol.Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;
        checkedCol.DataType = typeof(bool);
        checkedCol.Header.VisiblePosition = 0;
      }

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 2;

      // Hide column
      e.Layout.Bands[0].Columns["CurrencyPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SupplierPid"].Hidden = true;

      e.Layout.Bands[0].Columns["PONo"].Header.Caption = "Đơn mua hàng";
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Mã sản phẩm";
      e.Layout.Bands[0].Columns["MaterialNameVn"].Header.Caption = "Tên sản phẩm";
      e.Layout.Bands[0].Columns["Unit"].Header.Caption = "Đơn vị";      
      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Số lượng";
      e.Layout.Bands[0].Columns["QtyReceipted"].Header.Caption = "Số lượng\nđã nhận";
      e.Layout.Bands[0].Columns["Remain"].Header.Caption = "Số lượng\ncòn lại";
      e.Layout.Bands[0].Columns["SupplierName"].Header.Caption = "Nhà cung cấp";
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ugdInformation, "Danh sách phiếu mua hàng");
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

    private void btnSelect_Click(object sender, EventArgs e)
    {
      bool invalid = this.CheckValid();
      if (invalid)
      {
        this.dtPODetail = this.CreatePODetail();
        for (int i = 0; i < ugdInformation.Rows.Count; i++)
        {
          UltraGridRow row = ugdInformation.Rows[i];
          DataRow rowadd = dtPODetail.NewRow();
          if ((bool)row.Cells["Selected"].Value == true)
          {
            rowadd["PONo"] = row.Cells["PONo"].Value.ToString();
            rowadd["MaterialCode"] = row.Cells["MaterialCode"].Value.ToString();
            rowadd["Qty"] = DBConvert.ParseDouble(row.Cells["Remain"].Value);
            dtPODetail.Rows.Add(rowadd);
          }
        }
        this.CloseTab();
      }      
    }
    #endregion event
  }
}
