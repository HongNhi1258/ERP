/*
 * Authour : Nguyen Ngoc Tien
 * Date : 05/12/2014
 * Description : Print Routing Ticket
 */
using CrystalDecisions.CrystalReports.Engine;
using DaiCo.Application;
using DaiCo.ERPProject.Share.DataSetSource;
using DaiCo.ERPProject.Share.ReportTemplate;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWIP_96_006 : MainUserControl
  {
    public viewWIP_96_006()
    {
      InitializeComponent();
    }
    #region Init
    //Dinh dang thoi gian
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private bool isSelectAllChange = true;
    private bool isDeleteAllChange = true;

    #endregion

    #region Functions
    /// <summary>
    /// Key events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.Search();
      }
    }


    /// <summary>
    /// Load Work Order
    /// </summary>
    private void LoadComboboxWoSearch()
    {
      string commandText = string.Format(@"SELECT DISTINCT WorkOrderPid  
                                           FROM TblPLNWorkOrderConfirmedDetails  ORDER BY WorkOrderPid DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultcbmWoSearch, dt, "WorkOrderPid", "WorkOrderPid", false);
    }
    /// <summary>
    /// Load Item Code
    /// </summary>
    private void LoadComboboxItemCode()
    {
      string commandText = string.Empty;
      if (ultcbmWoSearch.Value != null)
      {
        commandText = string.Format(@"SELECT ItemCode FROM TblPLNWorkOrderConfirmedDetails WHERE WorkOrderPid ={0}", ultcbmWoSearch.Value.ToString());
      }
      else
      {
        commandText = string.Format(@"SELECT ItemCode FROM TblBOMItemBasic");
      }
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultcbmItemCode, dt, "ItemCode", "ItemCode", false);
    }
    /// <summary>
    /// Load Carcass Code
    /// </summary>
    private void LoadCarcassSearch()
    {
      string commandText = string.Empty;
      if (ultcbmWoSearch.Value != null)
      {
        commandText = string.Format(@"SELECT DISTINCT CarcassCode FROM TblPLNWorkOrderConfirmedDetails WHERE WorkOrderPid ={0}", ultcbmWoSearch.Value.ToString());
      }
      else
      {
        commandText = string.Format(@"SELECT DISTINCT CarcassCode FROM TblBOMItemBasic");
      }
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultCBCarcass, dt, "CarcassCode", "CarcassCode", false);
    }
    /// <summary>
    /// Search data
    /// </summary>
    private void Search()
    {
      DBParameter[] inputParam = new DBParameter[7];
      if (txtFurnitureCode.Text.Trim().Length > 0)
      {
        inputParam[0] = new DBParameter("@FurnitureCode", DbType.AnsiString, 128, txtFurnitureCode.Text.Trim());
      }
      if (ultcbmWoSearch.SelectedRow != null)
      {
        inputParam[1] = new DBParameter("@Wo", DbType.Int64, DBConvert.ParseLong(ultcbmWoSearch.Value.ToString()));
      }
      if (ultcbmItemCode.SelectedRow != null)
      {
        inputParam[2] = new DBParameter("@ItemCode", DbType.AnsiString, 16, ultcbmItemCode.Value.ToString());
      }
      if (ultCBCarcass.SelectedRow != null)
      {
        inputParam[3] = new DBParameter("@Carcass", DbType.AnsiString, 16, ultCBCarcass.Value.ToString());
      }
      if (ultdtFromDate.Value != null)
      {
        inputParam[4] = new DBParameter("@FromDate", DbType.DateTime, DBConvert.ParseDateTime(ultdtFromDate.Value.ToString(), formatConvert));
      }
      if (ultdtToDate.Value != null)
      {
        inputParam[5] = new DBParameter("@ToDate", DbType.DateTime, DBConvert.ParseDateTime(ultdtToDate.Value.ToString(), formatConvert));
      }
      if (chkOverview.Checked)
      {
        inputParam[6] = new DBParameter("@OverView", DbType.Int32, 1);
      }

      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spWIPFurnitureCode_Select", 600, inputParam);
      if (dsSource != null)
      {
        chkSelectAll.Checked = false;
        this.ultData.DataSource = dsSource.Tables[0];
      }
      else
      {
        ultData.DataSource = DBNull.Value;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (ultData.Rows[i].Cells["StandByEN"].Value.ToString() == "ADJ")
        {
          ultData.Rows[i].Cells["Delete"].Activation = Activation.AllowEdit;
        }
        else
        {
          ultData.Rows[i].Cells["Delete"].Activation = Activation.ActivateOnly;
          ultData.Rows[i].Cells["Delete"].Appearance.BackColor = Color.LightGray;

        }
      }

      lbCount.Text = string.Format("Count: {0}", ultData.Rows.FilteredInRowCount);
    }

    private dsWIPRoutingTicketByPart GetSelectRoutingTicket()
    {
      dsWIPRoutingTicketByPart dsResult = null;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int select = DBConvert.ParseInt(ultData.Rows[i].Cells["Select"].Value.ToString());
        if (select != 0)
        {
          if (dsResult == null)
          {
            dsResult = new dsWIPRoutingTicketByPart();
          }
          DBParameter[] input = new DBParameter[3];
          long wo = DBConvert.ParseLong(ultData.Rows[i].Cells["WorkOrder"].Value.ToString());
          string carcode = ultData.Rows[i].Cells["CarcassCode"].Value.ToString();
          long partcode = DBConvert.ParseLong(ultData.Rows[i].Cells["PartCodePid"].Value.ToString());
          input[0] = new DBParameter("@WO", DbType.Int64, wo);
          input[1] = new DBParameter("@CarcassCode", DbType.String, carcode);
          input[2] = new DBParameter("@PartPid", DbType.Int64, partcode);
          DataSet dsData = DataBaseAccess.SearchStoreProcedure("spWIPRountingTicketByPart_Ver1", input);
          if (dsData != null)
          {
            foreach (DataRow rowParent in dsData.Tables[0].Rows)
            {
              DataRow rowResultParent = dsResult.Tables["ItemInfo"].NewRow();
              rowResultParent["ItemCodeDisplay"] = rowParent["ItemCodeDisplay"].ToString();
              rowResultParent["SaleCode"] = rowParent["SaleCode"].ToString();
              rowResultParent["CarcassCode"] = rowParent["CarcassCode"].ToString();
              rowResultParent["Partcode"] = rowParent["Partcode"].ToString();
              rowResultParent["Category"] = rowParent["Category"].ToString();
              rowResultParent["CustomerCode"] = rowParent["CustomerCode"].ToString();
              rowResultParent["SpecialInfo"] = rowParent["SpecialInfo"].ToString();
              rowResultParent["WorkOrderDisplay"] = rowParent["WorkOrderDisplay"].ToString();
              rowResultParent["CWoQty"] = rowParent["CWoQty"].ToString();
              rowResultParent["Itemsize"] = rowParent["Itemsize"].ToString();
              rowResultParent["PartInfo"] = rowParent["PartInfo"].ToString();
              rowResultParent["MainFinish"] = rowParent["MainFinish"].ToString();
              rowResultParent["OtherFinish1"] = rowParent["OtherFinish1"].ToString();
              rowResultParent["OtherFinish2"] = rowParent["OtherFinish2"].ToString();
              rowResultParent["OtherFinish3"] = rowParent["OtherFinish3"].ToString();
              rowResultParent["OtherFinish4"] = rowParent["OtherFinish4"].ToString();
              rowResultParent["OtherFinish5"] = rowParent["OtherFinish5"].ToString();
              rowResultParent["OtherFinish6"] = rowParent["OtherFinish6"].ToString();
              rowResultParent["OtherFinish7"] = rowParent["OtherFinish7"].ToString();
              rowResultParent["OtherFinish8"] = rowParent["OtherFinish8"].ToString();
              rowResultParent["OtherFinish9"] = rowParent["OtherFinish9"].ToString();
              rowResultParent["OtherFinish10"] = rowParent["OtherFinish10"].ToString();
              rowResultParent["Hardware"] = rowParent["Hardware"].ToString();
              rowResultParent["ItemCode"] = rowParent["ItemCode"].ToString();
              rowResultParent["Revision"] = rowParent["Revision"].ToString();
              rowResultParent["WorkOrder"] = rowParent["WorkOrder"].ToString();
              rowResultParent["PartPid"] = rowParent["PartPid"].ToString();
              //Image Item
              string fileItemCodePath = FunctionUtility.BOMGetItemImage(rowParent["ItemCode"].ToString(), DBConvert.ParseInt(rowParent["Revision"].ToString()));
              rowResultParent["ItemImage"] = FunctionUtility.ImageToByteArrayWithFormat(fileItemCodePath, 380, 1.34, "JPG");
              rowResultParent["Collection"] = rowParent["Collection"].ToString();
              dsResult.Tables["ItemInfo"].Rows.Add(rowResultParent);
            }
            foreach (DataRow rowChild in dsData.Tables[1].Rows)
            {
              DataRow rowResultChild = dsResult.Tables["Process"].NewRow();
              rowResultChild["Ordinal"] = rowChild["Ordinal"];
              rowResultChild["CarcassProcessPid"] = rowChild["CarcassProcessPid"];
              rowResultChild["Description"] = rowChild["Description"];
              rowResultChild["TeamCode"] = rowChild["TeamCode"];
              rowResultChild["ProcessTime"] = rowChild["ProcessTime"];
              rowResultChild["Lead"] = rowChild["Lead"];
              rowResultChild["QC"] = rowChild["QC"];
              rowResultChild["ItemCode"] = rowChild["ItemCode"];
              rowResultChild["Wo"] = rowChild["WO"];
              rowResultChild["Revision"] = rowChild["Revision"];
              rowResultChild["PartPid"] = rowChild["PartPid"];
              dsResult.Tables["Process"].Rows.Add(rowResultChild);
            }
          }
        }
      }
      return dsResult;
    }
    #endregion

    #region Events
    /// <summary>
    /// Ham Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWIP_96_006_Load(object sender, EventArgs e)
    {
      LoadComboboxWoSearch();
      LoadComboboxItemCode();
    }
    /// <summary>
    /// Khi thay doi WO
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultcbmWoSearch_ValueChanged(object sender, EventArgs e)
    {
      LoadComboboxItemCode();
      LoadCarcassSearch();
      ultcbmItemCode.Text = string.Empty;
    }
    /// <summary>
    /// Khi bam nut Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      Search();
    }
    /// <summary>
    /// Khi click chuot tren luoi
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      try
      {
        if (ultData.Selected.Rows[0].Cells["ItemCode"].Value.ToString() == string.Empty)
        {
          pictureItem.ImageLocation = Shared.Utility.FunctionUtility.BOMGetCarcassImage(ultData.Selected.Rows[0].Cells["CarcassCode"].Value.ToString());
        }
        else
        {
          Utility.BOMShowItemImage(ultData, groupBox5, pictureItem, true);
        }

        txtRemarkItem.Text = ultData.Selected.Rows[0].Cells["Remark"].Value.ToString();
      }
      catch
      {
      }
    }
    /// <summary>
    /// Dinh dang luoi
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["FurnitureCode"].Header.Caption = "Furniture Code";
      e.Layout.Bands[0].Columns["WorkOrder"].Header.Caption = "Work Order";
      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["PartCodePid"].Hidden = true;
      e.Layout.Bands[0].Columns["Remark"].Hidden = true;
      e.Layout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      //e.Layout.Bands[0].Columns["Delete"].Hidden = true;
      e.Layout.Bands[0].Columns["Delete"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      if (!chkOverview.Checked)
      {
        e.Layout.Bands[0].Columns["CreatedBy"].Hidden = true;
      }
      else
      {
        e.Layout.Bands[0].Columns["CreatedBy"].Hidden = false;
      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;

        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Columns["Select"].CellActivation = Activation.AllowEdit;
      //e.Layout.Bands[0].Columns["Delete"].CellActivation = Activation.AllowEdit;
    }
    /// <summary>
    /// Select All Furniture
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.isSelectAllChange)
      {
        int checkAll = (chkSelectAll.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].IsFilteredOut == false)
          {
            ultData.Rows[i].Cells["Select"].Value = checkAll;
          }
        }
      }
    }

    /// <summary>
    /// Select All Furniture
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkDeleteAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.isDeleteAllChange)
      {
        int checkAll = (chkDeleteAll.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (ultData.Rows[i].IsFilteredOut == false && ultData.Rows[i].Cells["StandByEN"].Value.ToString() == "ADJ")
          {
            ultData.Rows[i].Cells["Delete"].Value = checkAll;
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_AfterRowFilterChanged(object sender, AfterRowFilterChangedEventArgs e)
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        if (row.IsFilteredOut == true)
        {
          row.Cells["Select"].Value = 0;
        }
      }
      lbCount.Text = string.Format("Count: {0}", ultData.Rows.FilteredInRowCount);
    }

    private void btnPrintHangTagOld_Click(object sender, EventArgs e)
    {
      DaiCo.Shared.View_Report rp = null;
      ReportClass cpt = null;
      dsWIPRoutingTicketByPart dsRoutingTicket = GetSelectRoutingTicket();
      if (dsRoutingTicket != null)
      {
        cpt = new cptRountingTicket();
        cpt.SetDataSource(dsRoutingTicket);
        rp = new DaiCo.Shared.View_Report(cpt);
        rp.IsShowGroupTree = true;
        rp.ShowReport(Shared.Utility.ViewState.Window, true, FormWindowState.Maximized);
      }
      else
      {
        WindowUtinity.ShowMessageWarning("WRN0024", "part");
      }
    }

    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare("Select", column, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          this.isSelectAllChange = false;
          chkSelectAll.Checked = false;
          this.isSelectAllChange = true;
        }
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      ultCBCarcass.Value = DBNull.Value;
      ultcbmWoSearch.Value = DBNull.Value;
      ultcbmItemCode.Value = DBNull.Value;
      ultdtFromDate.Value = DBNull.Value;
      ultdtToDate.Value = DBNull.Value;
      txtFurnitureCode.Text = string.Empty;
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }
    #endregion

    private void btnAdjustOut_Click(object sender, EventArgs e)
    {
      DataTable dt = (DataTable)ultData.DataSource;

      string strPartcodePid = string.Empty;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(dt.Rows[i]["Select"].ToString()) == 1)
        {
          if (strPartcodePid == string.Empty)
          {
            strPartcodePid = dt.Rows[i]["PartCodePid"].ToString();
          }
          else
          {
            strPartcodePid += "," + dt.Rows[i]["PartCodePid"].ToString();
          }
        }
      }

      viewWIP_96_009 view = new viewWIP_96_009();
      view.strBarcodePid = strPartcodePid;

      WindowUtinity.ShowView(view, "ADJUSTMENT BARCODE", true, ViewState.ModalWindow);
      this.btnSearch_Click(null, null);
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      DataTable dt = (DataTable)ultData.DataSource;
      string strPartcodePid = string.Empty;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(dt.Rows[i]["Delete"].ToString()) == 1)
        {
          if (strPartcodePid == string.Empty)
          {
            strPartcodePid = dt.Rows[i]["PartCodePid"].ToString();
          }
          else
          {
            strPartcodePid += "," + dt.Rows[i]["PartCodePid"].ToString();
          }
        }
      }

      DBParameter[] inputParam = new DBParameter[4];
      try
      {
        if (DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["PartCodePid"].Value.ToString()) != long.MinValue)
        {
          inputParam[0] = new DBParameter("@BarcodePid", DbType.Int64, DBConvert.ParseLong(ultData.Selected.Rows[0].Cells["PartCodePid"].Value.ToString()));
        }
      }
      catch { }
      inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      inputParam[2] = new DBParameter("@Remark", DbType.String, 512, txtRemarkItem.Text);
      inputParam[3] = new DBParameter("@StrBarcodePidDelete", DbType.AnsiString, 8000, strPartcodePid);

      DataBaseAccess.ExecuteStoreProcedure("spWIPFurniturePartCode_Update", inputParam);

      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.Search();
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        Utility.ExportToExcelWithDefaultPath(ultData, "Print Routing");
      }
    }
  }
}
