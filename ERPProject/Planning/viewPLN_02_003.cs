/*
   Author  : Vo Van Duy Qui
   Email   : qui_it@daico-furniture.com
   Date    : 12-08-2010
   Company : Dai Co   
*/

using DaiCo.Application;
using DaiCo.Objects;
using DaiCo.Shared;
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


namespace DaiCo.ERPProject
{
  public partial class viewPLN_02_003 : MainUserControl
  {
    #region Field
    public viewPLN_02_025 parentUC = null;
    public long currentWorkOrder = long.MinValue;
    bool chkIsDefaultQty = false;
    #endregion Field

    #region Init

    public viewPLN_02_003()
    {
      InitializeComponent();
      dt_ScheduleDeliveryFrom.Value = DateTime.MinValue;
      dt_ScheduleDeliveryTo.Value = DateTime.MinValue;
      btnClose.Click += new EventHandler(btnClose_Click);
      btnSearch.Click += new EventHandler(btnSearch_Click);
      //btnNew.Click += new EventHandler(btnNew_Click);
      chkShowImage.CheckedChanged += new EventHandler(chkShowImage_CheckedChanged);
      chkDefaultQty.CheckedChanged += new EventHandler(chkDefaultQty_CheckedChanged);
      ultData.MouseClick += new MouseEventHandler(ultData_MouseClick);
      ultData.BeforeCellUpdate += new BeforeCellUpdateEventHandler(ultData_BeforeCellUpdate);
      Utility.LoadCustomer(cbCustomer);
    }

    private bool CheckValid()
    {
      bool result = true;
      for (int j = 0; j < ultData.Rows.Count; j++)
      {
        ultData.Rows[j].Appearance.BackColor = Color.White;
      }
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Remain"].Value.ToString()) < DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Value.ToString()))
        {
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
          result = false;
        }
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 2)
        {
          ultData.Rows[i].Appearance.BackColor = Color.Yellow;
          result = false;
        }
      }
      return result;
    }

    private DataTable CreateDataTable()
    {
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("SODetailPid", typeof(System.Int64));
      taParent.Columns.Add("UpdateQty", typeof(System.Int32));
      return taParent;
    }

    /// <summary>
    /// Search WorkOrder Detail Info
    /// </summary>
    private void Search()
    {
      PLNDataForAddWorkOrderDetail data = new PLNDataForAddWorkOrderDetail();
      if (this.strCondition.Length > 0)
      {
        data.Condition = this.strCondition;
        data.CurrentWorkOrder = this.currentWorkOrder;
      }
      else
      {
        data.ScheduleDeliveryFrom = dt_ScheduleDeliveryFrom.Value;
        DateTime scheduleDelivery = dt_ScheduleDeliveryTo.Value;
        if (scheduleDelivery != DateTime.MinValue)
        {
          scheduleDelivery = (scheduleDelivery != DateTime.MaxValue) ? scheduleDelivery.AddDays(1) : scheduleDelivery;
        }
        data.ScheduleDeliveryTo = scheduleDelivery;
        data.PoNoFrom = txtPoFrom.Text.Trim();
        data.PoNoTo = txtPoTo.Text.Trim();
        data.ItemCode = "%" + txtItemCode.Text.Trim() + "%";
        //add
        data.Revision = DBConvert.ParseInt(txtRevision.Text.ToString());
        data.OldCode = txtOldCode.Text.ToString();
        if (cbCustomer.Text.ToString().Trim().Length == 0)
        {
        }
        else if (cbCustomer.SelectedIndex < 0)
        {
          data.CustomerPid = 0;
        }
        else if (cbCustomer.SelectedIndex > 0)
        {
          data.CustomerPid = DBConvert.ParseLong(cbCustomer.SelectedValue.ToString());
        }
        data.CusPoNo = txtCusPoNo.Text.ToString();
        data.CurrentWorkOrder = this.currentWorkOrder;
      }
      this.strCondition = "";
      DataTable dtSource = DaiCo.Shared.DataBaseUtility.DataBaseAccess.SearchStoreObjectDataTable(data);
      DataColumn dtColumn = new DataColumn("OpenQty", typeof(Int32));

      dtSource.Columns.Add(dtColumn);
      ultData.DataSource = dtSource;
      int count = dtSource.Columns.Count;
      for (int i = 0; i < count - 1; i++)
      {
        ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }
      //total CBM, OpenQty
      double cbm = 0;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["CBM Remain"].Value.ToString().Length > 0)
        {
          cbm += DBConvert.ParseDouble(ultData.Rows[i].Cells["CBM Remain"].Value.ToString());
        }
      }
      //cbm
      if (cbm > 0)
      {
        lbTotalCBM.Text = cbm.ToString();
      }
      else
      {
        lbTotalCBM.Text = string.Empty;
      }
      ultData.DisplayLayout.Bands[0].Columns["SaleOrderDetailPid"].Hidden = true;
    }

    #endregion Init

    #region Event

    /// <summary>
    /// Add Work Order Detail To View_PLN_1002_WorkOrderInfo
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      if (this.CheckValid())
      {
        if (parentUC != null)
        {
          DataTable dt = (DataTable)ultData.DataSource;
          if (dt != null && dt.Rows.Count > 0)
          {
            DataTable dtAcess = this.CreateDataTable();
            DataRow[] dr = new DataRow[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
              DataRow drAcess = dtAcess.NewRow();
              if (DBConvert.ParseInt(dt.Rows[i]["OpenQty"].ToString()) > 0 && DBConvert.ParseInt(dt.Rows[i]["FlagAccess"].ToString()) == 1)
              {

                drAcess["SODetailPid"] = dt.Rows[i]["SaleOrderDetailPid"];
                drAcess["UpdateQty"] = DBConvert.ParseInt(dt.Rows[i]["OpenQty"].ToString());
                dtAcess.Rows.Add(drAcess);
              }
              dr[i] = dt.Rows[i];
            }
            SqlDBParameter[] inputParam = new SqlDBParameter[1];
            inputParam[0] = new SqlDBParameter("@TblDataSource", SqlDbType.Structured, dtAcess);
            SqlDBParameter[] outputParam = new SqlDBParameter[1];
            outputParam[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);
            DaiCo.Shared.DataBaseUtility.SqlDataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderDetail_UpdateAcessQty", inputParam, outputParam);
            if (outputParam != null && DBConvert.ParseInt(outputParam[0].Value.ToString()) == 0)
            {
              DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Data");
              return;
            }
            parentUC.dtRowDetail = dr;
          }
          this.ConfirmToCloseTab();
        }
      }
      else
      {
        DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", "Data");
      }
    }

    /// <summary>
    /// Search WorkOrder Detail Info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Close User Control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    /// <summary>
    /// Select Show Image Or Not Show
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    /// <summary>
    /// Set Default Open Qty
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkDefaultQty_CheckedChanged(object sender, EventArgs e)
    {
      chkIsDefaultQty = true;
      if (chkDefaultQty.Checked)
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].Cells["OpenQty"].Value = ultData.Rows[i].Cells["Remain"].Value;
        }
      }
      else
      {
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          ultData.Rows[i].Cells["OpenQty"].Value = DBNull.Value;
        }
      }
      int totalOpenQty = 0;
      DataTable dtSource = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["OpenQty"].Value.ToString().Length > 0)
        {
          totalOpenQty += DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Value.ToString());
        }
      }
      if (totalOpenQty > 0)
      {
        lbTotalOpenQty.Text = totalOpenQty.ToString();
      }
      else
      {
        lbTotalOpenQty.Text = string.Empty;
      }
      chkIsDefaultQty = false;
    }

    /// <summary>
    /// Check Valid Of OpenQty's Value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      if (chkIsDefaultQty)
      {
        return;
      }
      string columnName = e.Cell.Column.ToString();
      switch (columnName.ToLower())
      {
        case "openqty":
          if (e.Cell.Text.Length > 0)
          {
            int qty = DBConvert.ParseInt(e.Cell.Text);
            int remain = DBConvert.ParseInt(e.Cell.Row.Cells["Remain"].Value.ToString());
            if (qty <= 0 || qty > remain)
            {
              DaiCo.Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "OpenQty" });
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// After Cell update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      if (chkIsDefaultQty)
      {
        return;
      }
      string columnName = e.Cell.Column.ToString();
      switch (columnName.ToLower())
      {
        case "openqty":
          int totalOpenQty = 0;
          DataTable dtSource = (DataTable)ultData.DataSource;
          for (int i = 0; i < dtSource.Rows.Count; i++)
          {
            if (ultData.Rows[i].Cells["OpenQty"].Value.ToString().Length > 0)
            {
              totalOpenQty += DBConvert.ParseInt(ultData.Rows[i].Cells["OpenQty"].Value.ToString());
            }
          }
          if (totalOpenQty > 0)
          {
            lbTotalOpenQty.Text = totalOpenQty.ToString();
          }
          else
          {
            lbTotalOpenQty.Text = string.Empty;
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Show Image Of Item Selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    /// <summary>
    /// Init Ultra Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["CarcassCode"].Hidden = true;
      e.Layout.Bands[0].Columns["SaleOrderPid"].Hidden = true;
      e.Layout.Bands[0].Columns["FlagAccess"].Hidden = true;
      //e.Layout.Bands[0].Columns["Error"].Hidden = true;
      //e.Layout.Bands[0].Columns["UpdateQty"].Hidden = true;
      e.Layout.Bands[0].Columns["OpenQty_1"].Hidden = true;

      e.Layout.Bands[0].Columns["SaleNo"].Header.Caption = "Sale No";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].Header.Caption = "Confirmed Ship Date";
      e.Layout.Bands[0].Columns["ScheduleDelivery"].CellAppearance.TextHAlign = HAlign.Center;
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["CancelQty"].Header.Caption = "Cancel Qty";
      e.Layout.Bands[0].Columns["OpenWo"].Header.Caption = "Open Wo";
      e.Layout.Bands[0].Columns["OpenQty"].Header.Caption = "Open Qty";
      e.Layout.Bands[0].Columns["ContractOut"].Header.Caption = "Contract Out";
      e.Layout.Bands[0].Columns["OldCode"].Hidden = true;
      e.Layout.Bands[0].Columns["Error"].Hidden = true;

      e.Layout.Bands[0].Columns["ContractOut"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["CancelQty"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OpenWo"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Columns["OpenQty"].CellAppearance.TextHAlign = HAlign.Right;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;

      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["OpenQty"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries.Add(SummaryType.Sum, e.Layout.Bands[0].Columns["CBM Remain"], SummaryPosition.UseSummaryPositionColumn);
      e.Layout.Bands[0].Summaries[0].DisplayFormat = "{0:###,##0}";
      e.Layout.Bands[0].Summaries[1].DisplayFormat = "{0:###,##0.000}";

      e.Layout.Bands[0].Summaries[0].Appearance.TextHAlign = HAlign.Right;
      e.Layout.Bands[0].Summaries[1].Appearance.TextHAlign = HAlign.Right;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOpenDialog_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string folder = string.Format(@"{0}\Report", startupPath);
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      //dialog.InitialDirectory = this.pathExport;
      dialog.InitialDirectory = folder;
      dialog.Title = "Select a Excel file";
      txtFileName.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
      btnImport.Enabled = (txtFileName.Text.Trim().Length > 0);
    }

    private void FillDataForGrid(DataTable dt)
    {
      if (dt != null)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i]["SaleOrderNo"].ToString().Length > 0)
          {
            this.strCondition = this.strCondition + "," + dt.Rows[i][0].ToString() + "()" + dt.Rows[i][1].ToString() + "()" + dt.Rows[i][2].ToString();
          }
          else if (dt.Rows[i]["ItemCode"].ToString().Length > 0)
          {
            this.strCondition = this.strCondition + "," + dt.Rows[i][1].ToString() + "()" + dt.Rows[i][2].ToString() + "|" + dt.Rows[i][4].ToString();
          }

        }
        if (strCondition.Length > 0)
        {
          this.strCondition = this.strCondition.Substring(1, this.strCondition.Length - 1);
        }
        else
        {
          this.strCondition = ",,";
        }
        this.Search();
        for (int j = 0; j < ultData.Rows.Count; j++)
        {
          DataRow[] dr = dt.Select(string.Format(@"SaleOrderNo = '{0}' and ItemCode = '{1}' and Revision = {2}", ultData.Rows[j].Cells["SaleNo"].Value.ToString(), ultData.Rows[j].Cells["ItemCode"].Value.ToString(), DBConvert.ParseInt(ultData.Rows[j].Cells["Revision"].Value.ToString())));
          int qtyOpen = 0;
          for (int k = 0; k < dr.Length; k++)
          {
            if (DBConvert.ParseInt(dr[k]["Qty"].ToString()) != int.MinValue)
            {
              qtyOpen += DBConvert.ParseInt(dr[k]["Qty"].ToString());
            }
          }

          if (DBConvert.ParseInt(ultData.Rows[j].Cells["FlagAccess"].Value) == 1)
          {
            qtyOpen = DBConvert.ParseInt(ultData.Rows[j].Cells["OpenQty_1"].Value.ToString());
          }
          //qtyOpen = DBConvert.ParseInt(ultData.Rows[j].Cells["OpenQty_1"].Value.ToString());
          ultData.Rows[j].Cells["OpenQty"].Value = qtyOpen;
          if (DBConvert.ParseInt(ultData.Rows[j].Cells["Remain"].Value) < qtyOpen)
          {
            ultData.Rows[j].Appearance.BackColor = Color.Yellow;
          }
          //if (DBConvert.ParseInt(ultData.Rows[j].Cells["Error"].Value) == 2)
          //{
          //  ultData.Rows[j].Appearance.BackColor = Color.Pink;
          //}
        }
        this.strCondition = "";
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "viewPlan_02_003_Temp_2";
      string sheetName = "List";
      string outFileName = "viewPlan_02_003_Temp_2";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\Planning\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    string strCondition = "";
    private void btnImport_Click(object sender, EventArgs e)
    {
      // Check invalid file
      if (!File.Exists(txtFileName.Text.Trim()))
      {
        WindowUtinity.ShowMessageErrorFromText("File not found. Please check the file path.");
        return;
      }

      // Get Items Count
      DataSet dsCount = FunctionUtility.GetExcelToDataSet(txtFileName.Text.Trim(), "SELECT * FROM [List (1)$C3:C4]");
      int itemCount = 0;
      if (dsCount == null || dsCount.Tables.Count == 0 || dsCount.Tables[0].Rows.Count == 0)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Items Count");
        return;
      }
      else
      {
        itemCount = DBConvert.ParseInt(dsCount.Tables[0].Rows[0][0]);
        if (itemCount <= 0)
        {
          WindowUtinity.ShowMessageError("ERR0001", "Items Count");
          return;
        }
      }
      // Get data for items list
      DataSet dsItemList = FunctionUtility.GetExcelToDataSet(txtFileName.Text.Trim(), string.Format("SELECT * FROM [List (1)$B5:F{0}]", 5 + itemCount));
      if (dsItemList == null || dsItemList.Tables.Count == 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Can't get data. Please check template file.");
        return;
      }
      this.FillDataForGrid(dsItemList.Tables[0]);
    }

    /// <summary>
    /// Key Up && Down==> For input data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode != Keys.Down && e.KeyCode != Keys.Up)
      {
        return;
      }

      int rowIndex = (e.KeyCode == Keys.Down) ? ultData.ActiveCell.Row.Index + 1 : ultData.ActiveCell.Row.Index - 1;
      int cellIndex = ultData.ActiveCell.Column.Index;

      try
      {
        ultData.Rows[rowIndex].Cells[cellIndex].Activate();
        ultData.PerformAction(UltraGridAction.EnterEditMode, false, false);
      }
      catch { }
    }


    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      //UltraGridRow row = ultData.Selected.Rows[0];
      //viewPLN_10_007 view = new viewPLN_10_007();
      //view.Itemcode = row.Cells["ItemCode"].Value.ToString();
      //view.Revision = DBConvert.ParseInt(row.Cells["Revision"].Value.ToString());
      //view.Saleorderpid = DBConvert.ParseLong(row.Cells["SaleOrderPid"].Value.ToString());
      //view.value = true;
      //DaiCo.Shared.Utility.WindowUtinity.ShowView(view, "Update WO Link Sale Order", false, DaiCo.Shared.Utility.ViewState.ModalWindow, FormWindowState.Maximized);
      //this.btnSearch_Click(sender, e);

    }

    private void txtPoFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dt_ScheduleDeliveryTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtPoTo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtRevision_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtCusPoNo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtOldCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void cbCustomer_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void dt_ScheduleDeliveryFrom_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }
    #endregion Event


  }
}
