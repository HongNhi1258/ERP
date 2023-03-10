/*
  Author      : 
  Date        : 22/4/2014
  Description : CST Request Online From Loading List - 2
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_97_002 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private int checkdefault = 0;
    //public string itemCode = string.Empty;
    //public string container = string.Empty;
    #endregion Field

    #region Init
    public viewPLN_97_002()
    {
      InitializeComponent();
    }

    private void viewPLN_97_001_Load(object sender, EventArgs e)
    {
      this.ultDTAssRequestDateFrom.Value = DBNull.Value;
      this.ultDTAssRequestDateTo.Value = DBNull.Value;
      this.ultDTShipdateFrom.Value = DBNull.Value;
      this.ultDTShipdateTo.Value = DBNull.Value;

      this.LoadContainer();
      this.LoadCarcass();
      this.LoadWorkOrder();
    }

    private void LoadWorkOrder()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Confirm = 1 and Status = 0 ORDER BY Pid DESC";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt == null)
      {
        return;
      }
      ultCBWorkOrder.DataSource = dt;
      ultCBWorkOrder.DisplayLayout.AutoFitColumns = true;
      ultCBWorkOrder.DisplayMember = "Pid";
      ultCBWorkOrder.ValueMember = "Pid";
      ultCBWorkOrder.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    ///// <summary>
    ///// Load ItemCode
    ///// </summary>
    private void LoadContainer()
    {
      string commandText = string.Empty;
      commandText += " SELECT CON.Pid ContainerPid, CON.ContainerNo ";
      commandText += " FROM TblPLNSHPContainer CON ";
      commandText += " WHERE Confirm <> 3 ";

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ucContainer.DataSource = dtSource;
      ucContainer.ColumnWidths = "0;200";
      ucContainer.DataBind();
      ucContainer.ValueMember = "ContainerPid";
      ucContainer.DisplayMember = "ContainerNo";
      ucContainer.AutoSearchBy = "ContainerNo";
    }

    ///// <summary>
    ///// Load Customer
    ///// </summary>
    private void LoadCarcass()
    {
      string commandText = "SELECT	distinct CarcassCode, CarcassCode Carcass FROM	TblBOMCarcass";

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ucCarcass.DataSource = dt;
      ucCarcass.ColumnWidths = "0;200";
      ucCarcass.DataBind();
      ucCarcass.ValueMember = "CarcassCode";
      ucCarcass.DisplayMember = "Carcass";
      ucCarcass.AutoSearchBy = "Carcass";
    }

    #endregion Init

    #region Function
    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      DBParameter[] input = new DBParameter[7];

      if (ultDTShipdateFrom.Value != null)
      {
        input[0] = new DBParameter("@ShipDateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultDTShipdateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
      }

      if (ultDTShipdateTo.Value != null)
      {
        input[1] = new DBParameter("@ShipDateTo", DbType.DateTime, DBConvert.ParseDateTime(ultDTShipdateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME).AddDays(1));
      }

      if (ultDTAssRequestDateFrom.Value != null)
      {
        input[2] = new DBParameter("@ASSRequestDateFrom", DbType.DateTime, DBConvert.ParseDateTime(ultDTAssRequestDateFrom.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME));
      }

      if (ultDTAssRequestDateTo.Value != null)
      {
        input[3] = new DBParameter("@ASSRequestDateTo", DbType.DateTime, DBConvert.ParseDateTime(ultDTAssRequestDateTo.Value.ToString(), USER_COMPUTER_FORMAT_DATETIME).AddDays(1));
      }

      if (txtContainer.Text.Length > 0)
      {
        input[4] = new DBParameter("@ContainerPid", DbType.AnsiString, 4000, txtContainer.Text.Trim());
      }

      if (txtCarcass.Text.Length > 0)
      {
        input[5] = new DBParameter("@CarcassCode", DbType.AnsiString, 4000, txtCarcass.Text.Trim());
      }

      if (ultCBWorkOrder.Value != null)
      {
        input[6] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultCBWorkOrder.Value.ToString()));
      }

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNMasterPlanConnectWithCSTRequestOnlineFromLoadingList_Select", 5000, input);

      //DataTable dtColor = new DataTable();
      //dtColor.Columns.Add("Color", typeof(System.Int32));

      if (dsSource != null)
      {
        this.ultData.DataSource = dsSource.Tables[0];
      }
      else
      {
        ultData.DataSource = DBNull.Value;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow row = this.ultData.Rows[i];

        int checkDeadline = DBConvert.ParseInt(ultData.Rows[i].Cells["FlagDeadline"].Value.ToString());
        int checkASS = DBConvert.ParseInt(ultData.Rows[i].Cells["FlagASS"].Value.ToString());
        int checkWO = 0;
        int remain = DBConvert.ParseInt(ultData.Rows[i].Cells["Remain"].Value.ToString());
        int remainHangtag = DBConvert.ParseInt(ultData.Rows[i].Cells["RemainRequest"].Value.ToString());

        if (ultData.Rows[i].Cells["WO"].Value.ToString().Length == 0)
        {
          checkWO = 1;
        }
        int checkCarcass = 0;
        if (ultData.Rows[i].Cells["CarcassCode"].Value.ToString().Length == 0)
        {
          checkCarcass = 1;
        }
        int checkRequestIssued = 0;
        if (ultData.Rows[i].Cells["FlagDesign"].Value.ToString().Length == 0)
        {
          checkRequestIssued = 2;
        }

        // WO
        if (checkWO == 1)
        {
          ultData.Rows[i].Cells["WO"].Appearance.BackColor = Color.Orange;
        }

        // CarcassCode
        if (checkCarcass == 1)
        {
          ultData.Rows[i].Cells["CarcassCode"].Appearance.BackColor = Color.Orange;
        }

        // Deadline
        if (checkDeadline == 1)
        {
          ultData.Rows[i].Cells["Deadline"].Appearance.BackColor = Color.Orange;
        }

        // ASS
        if (checkASS == 1)
        {
          ultData.Rows[i].Cells["ASSRequestDate"].Appearance.BackColor = Color.Orange;
        }

        // Request Issued
        if (checkRequestIssued == 1)
        {
          ultData.Rows[i].Cells["RequestCode"].Appearance.BackColor = Color.Blue;
        }

        if (remain > 0 && remain <= remainHangtag)
        {
          ultData.Rows[i].Cells["QtyRequest"].Appearance.BackColor = Color.LightBlue;
          ultData.Rows[i].Cells["DeliveryDateRequest"].Appearance.BackColor = Color.LightBlue;
        }
      }
    }

    ///// <summary>
    ///// Export Excel
    ///// </summary>
    private void ExportExcel()
    {
      if (ultData.Rows.Count > 0)
      {
        Excel.Workbook xlBook;

        Utility.ExportToExcelWithDefaultPath(ultData, out xlBook, "CONNECT CST REQUEST ONLINE", 10);

        string filePath = xlBook.FullName;
        Excel.Worksheet xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);

        xlBook.Close(true, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
        Process.Start(filePath);
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
      e.Layout.UseFixedHeaders = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["FlagDeadline"].Hidden = true;
      e.Layout.Bands[0].Columns["FlagASS"].Hidden = true;
      e.Layout.Bands[0].Columns["FlagDesign"].Hidden = true;
      e.Layout.Bands[0].Columns["MinCreateDate"].Hidden = true;
      e.Layout.Bands[0].Columns["MaxCreateDate"].Hidden = true;
      e.Layout.Bands[0].Columns["DeliveryDateRequest"].Format = "dd-MMM-yyyy";
      e.Layout.Bands[0].Columns["Deadline"].Header.Caption = "PAK\nDeadline";
      e.Layout.Bands[0].Columns["RequestCode"].Header.Caption = "CST\nRequest Online";
      e.Layout.Bands[0].Columns["QtyRequestCreateDate"].Header.Caption = "Requet Qty\nAnd\nDelivery Required Date";
      e.Layout.Bands[0].Columns["RemainRequest"].Header.Caption = "Remain\nHangtag";
      e.Layout.Bands[0].Columns["QtyRequest"].Header.Caption = "Request\nQty";
      e.Layout.Bands[0].Columns["DeliveryDateRequest"].Header.Caption = "Delivery\nRequired Date";

      // Set header height
      e.Layout.Bands[0].ColHeaderLines = 3;

      //e.Layout.Override.AllowUpdate = DefaultableBoolean.False;
      e.Layout.Override.AllowDelete = DefaultableBoolean.False;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (e.Layout.Bands[0].Columns[i].DataType == typeof(DateTime))
        {
          e.Layout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;
        if ((string.Compare(columnName, "Request\nQty", true) == 0) || (string.Compare(columnName, "Delivery\nRequired Date", true) == 0))
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
          e.Layout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightBlue;
        }
        else
        {
          e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
      }

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Search 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
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

    private void ucContainer_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtContainer.Text = this.ucContainer.SelectedValue;
    }

    private void chkContainer_CheckedChanged(object sender, EventArgs e)
    {
      ucContainer.Visible = chkContainer.Checked;
    }

    private void chkCarcass_CheckedChanged(object sender, EventArgs e)
    {
      ucCarcass.Visible = chkCarcass.Checked;
    }

    private void ucCarcass_ValueChanged(object source, ValueChangedEventArgs args)
    {
      this.txtCarcass.Text = this.ucCarcass.SelectedValue;
    }

    private void chkShowImage_CheckedChanged(object sender, EventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void ultData_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowItemImage(ultData, groupItemImage, pictureItem, chkShowImage.Checked);
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      this.ExportExcel();
    }

    private void txtContainer_Leave(object sender, EventArgs e)
    {
      //this.ultContainer.Text = this.txtContainer.Text;
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.LoadContainer();
      this.LoadCarcass();
      this.LoadWorkOrder();

      this.txtContainer.Text = string.Empty;
      this.txtCarcass.Text = string.Empty;
      this.ultCBWorkOrder.Text = string.Empty;
      this.ultDTAssRequestDateFrom.Value = DBNull.Value;
      this.ultDTAssRequestDateTo.Value = DBNull.Value;
      this.ultDTShipdateFrom.Value = DBNull.Value;
      this.ultDTShipdateTo.Value = DBNull.Value;
    }

    //private void chkSetDefault_CheckedChanged(object sender, EventArgs e)
    //{
    //  if (chkSetDefault.Checked)
    //  {
    //    checkdefault = 1;
    //    for (int i = 0; i < ultData.Rows.Count; i++ )
    //    {
    //      ultData.Rows[i].Cells["QtyRequest"].Value = DBConvert.ParseInt(ultData.Rows[i].Cells["Remain"].Value.ToString());
    //      ultData.Rows[i].Cells["DeliveryDateRequest"].Value = DBConvert.ParseDateTime(ultData.Rows[i].Cells["ASSRequestDate"].Value.ToString(), USER_COMPUTER_FORMAT_DATETIME);
    //    }
    //  }
    //  else 
    //  {
    //    for (int i = 0; i < ultData.Rows.Count; i++)
    //    {
    //      ultData.Rows[i].Cells["QtyRequest"].Value = DBNull.Value;
    //      ultData.Rows[i].Cells["DeliveryDateRequest"].Value = DBNull.Value;
    //    }
    //  }
    //  checkdefault = 0;
    //}

    private void btnRequestCarcass_Click(object sender, EventArgs e)
    {
      if (WindowUtinity.ShowMessageConfirm("MSG0062").ToString() == "No")
      {
        return;
      }

      DataTable dtOldRequest = (DataTable)ultData.DataSource;
      DataTable dtNewRequest = dtOldRequest.Clone();

      for (int i = 0; i < dtOldRequest.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(dtOldRequest.Rows[i]["QtyRequest"].ToString()) > 0)
        {
          if (dtOldRequest.Rows[i]["DeliveryDateRequest"].ToString().Length == 0 ||
              dtOldRequest.Rows[i]["WO"].ToString().Length == 0 || dtOldRequest.Rows[i]["CarcassCode"].ToString().Length == 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "Data");
            return;
          }


          DataRow newrow = dtNewRequest.NewRow();
          newrow["WO"] = DBConvert.ParseLong(dtOldRequest.Rows[i]["WO"].ToString());
          newrow["CarcassCode"] = dtOldRequest.Rows[i]["CarcassCode"].ToString();
          newrow["QtyRequest"] = DBConvert.ParseInt(dtOldRequest.Rows[i]["QtyRequest"].ToString());
          newrow["DeliveryDateRequest"] = DBConvert.ParseDateTime(dtOldRequest.Rows[i]["DeliveryDateRequest"].ToString(), USER_COMPUTER_FORMAT_DATETIME);

          dtNewRequest.Rows.InsertAt(newrow, 0);
        }
      }

      if (dtNewRequest.Rows.Count == 0)
      {
        WindowUtinity.ShowMessageWarningFromText("Please input QtyRequest order to create Request Carcass!");
        return;
      }

      //Save phieu new request master
      for (int i = 0; i < dtNewRequest.Rows.Count; i++)
      {
        DataRow row = dtNewRequest.Rows[i];

        DBParameter[] inputParam = new DBParameter[6];
        inputParam[0] = new DBParameter("@Prefix", DbType.AnsiString, 3, "CRN");
        inputParam[1] = new DBParameter("@Urgent", DbType.Int32, 1);
        inputParam[2] = new DBParameter("@DeliveryDate", DbType.DateTime, DateTime.Now);
        inputParam[3] = new DBParameter("@Status", DbType.Int32, 1);
        inputParam[4] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
        inputParam[5] = new DBParameter("@TeamRequest", DbType.Int64, 32);

        DBParameter[] ouputParam = new DBParameter[1];
        ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spWIPCSTRequestCarcass_Insert", inputParam, ouputParam);
        if (DBConvert.ParseLong(ouputParam[0].Value.ToString()) <= 0)
        {
          return;
        }

        DBParameter[] inputParam1 = new DBParameter[5];
        inputParam1[0] = new DBParameter("@RequestPid", DbType.Int64, DBConvert.ParseLong(ouputParam[0].Value.ToString()));
        inputParam1[1] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(row["WO"].ToString()));
        inputParam1[2] = new DBParameter("@Carcass", DbType.AnsiString, 16, row["CarcassCode"].ToString());
        inputParam1[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(row["QtyRequest"].ToString()));
        inputParam1[4] = new DBParameter("@DeliveryDate", DbType.DateTime, DBConvert.ParseDateTime(row["DeliveryDateRequest"].ToString(), USER_COMPUTER_FORMAT_DATETIME));

        DBParameter[] outPutParam1 = new DBParameter[1];
        outPutParam1[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spWIPRequisitionNoteDetailCST_Insert", inputParam1, outPutParam1);
        if (DBConvert.ParseLong(outPutParam1[0].Value.ToString()) <= 0)
        {
          return;
        }
      }

      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.Search();
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      switch (colName)
      {
        // Code
        case "qtyrequest":
          int totalQty = 0;
          DataTable dtGrid = (DataTable)ultData.DataSource;
          for (int i = 0; i < dtGrid.Rows.Count; i++)
          {
            if (dtGrid.Rows[i]["WO"].ToString() == e.Cell.Row.Cells["WO"].Text && dtGrid.Rows[i]["CarcassCode"].ToString() == e.Cell.Row.Cells["CarcassCode"].Text)
            {
              totalQty += DBConvert.ParseInt(dtGrid.Rows[i]["QtyRequest"].ToString());
            }
          }

          if (e.Cell.Row.Cells["QtyRequest"].Text.Length > 0
            && checkdefault == 0
            && (DBConvert.ParseInt(e.Cell.Row.Cells["QtyRequest"].Text) < 0
                  || DBConvert.ParseInt(e.Cell.Row.Cells["QtyRequest"].Text) > DBConvert.ParseInt(e.Cell.Row.Cells["Remain"].Text)
                  || totalQty > DBConvert.ParseInt(e.Cell.Row.Cells["RemainRequest"].Text)
                )
          )
          {
            WindowUtinity.ShowMessageError("ERR0001", "Qty Request");
            e.Cancel = true;
            return;
          }
          break;
      }
    }
    #endregion Event
  }
}
