/*TRAN HUNG
 * Date: 21/12/2012
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
  public partial class viewPLN_10_011 : MainUserControl
  {
    public bool value;
    public bool flgInterNoPO = false;
    //public bool Flag = true;
    #region Load
    public viewPLN_10_011()
    {
      InitializeComponent();
    }

    private void Search()
    {
      btnSearch.Enabled = false;
      DBParameter[] input = new DBParameter[4];
      if (txtItemCode.Text.Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.AnsiString, 255, txtItemCode.Text.Trim());
      }
      if (txtSaleNo.Text.Length > 0)
      {
        input[1] = new DBParameter("@SaleNo", DbType.AnsiString, 255, txtSaleNo.Text.Trim());
      }
      if (txtPONo.Text.Length > 0)
      {
        input[2] = new DBParameter("@PONo", DbType.AnsiString, 255, txtPONo.Text.Trim());
      }
      if (this.flgInterNoPO == false)
      {
        input[3] = new DBParameter("@InterNoPO", DbType.Int32, 1);
      }
      DataTable dtSearch = DataBaseAccess.SearchStoreProcedureDataTable("spPLNSaleOrderQtyShipped_Select", 500, input);
      ultSaleorderdetail.DataSource = dtSearch;
      //Flag = true;
      btnSearch.Enabled = true;
    }
    #endregion Load

    #region Event
    private void viewPLN_10_007_Load(object sender, EventArgs e)
    {
      if (btnPerInterNoPO.Visible)
      {
        this.flgInterNoPO = true;
      }
      else
      {
        this.flgInterNoPO = false;
      }
      btnPerInterNoPO.Visible = false;
    }
    /// <summary>
    /// Search
    /// </summary>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultSaleorderdetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultSaleorderdetail);
      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["SaleNo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CustomerPONo"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["OpenWOQty"].CellActivation = Activation.ActivateOnly;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["OpenWOQty"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["QtyAdjShip"].CellAppearance.BackColor = Color.LightBlue;
      e.Layout.Bands[0].Columns["QtyAdj"].CellAppearance.BackColor = Color.LightBlue;

      e.Layout.Bands[0].Columns["Qty"].Header.Caption = "Shipped Qty";
      e.Layout.Bands[0].Columns["QtyAdjShip"].Header.Caption = "Down Unrelease WO";
      e.Layout.Bands[0].Columns["QtyAdj"].Header.Caption = "Cancelled Qty";
      e.Layout.Bands[0].Columns["OpenWOQty"].Header.Caption = "Opened WOQty";

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      DataTable dt = (DataTable)ultSaleorderdetail.DataSource;
      // Check Valid
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState == DataRowState.Modified)
        {
          DataRow row = dt.Rows[i];
          if (DBConvert.ParseInt(row["Qty"].ToString()) > DBConvert.ParseInt(row["OpenWOQty"].ToString()))
          {
            WindowUtinity.ShowMessageErrorFromText("Ship Qty Must <= Total Opened WO Qty");
            return;
          }

          // Check Qty Cancel
          DBParameter[] input = new DBParameter[4];
          input[0] = new DBParameter("@SaleOrderPid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["SOPid"].ToString()));
          input[1] = new DBParameter("@ItemCode", DbType.String, dt.Rows[i]["ItemCode"].ToString());
          input[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Revision"].ToString()));
          input[3] = new DBParameter("@CancelledQty", DbType.Int32, DBConvert.ParseInt(row["QtyAdj"].ToString()));

          DataTable dtCheckQtyCancel = DataBaseAccess.SearchStoreProcedureDataTable("spPLNCheckAdjustCancelledQty_Select", input);
          if (dtCheckQtyCancel.Rows.Count > 0)
          {
            WindowUtinity.ShowMessageErrorFromText("Cancelled Qty  <= " + dtCheckQtyCancel.Rows[0]["Remain"].ToString());
            return;
          }
        }
      }
      // Check Valid

      string Out = string.Empty;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState == DataRowState.Modified)
        {
          DBParameter[] inputParam = new DBParameter[7];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          inputParam[0] = new DBParameter("@SOPid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["SOPid"].ToString()));
          inputParam[1] = new DBParameter("@ItemCode", DbType.AnsiString, 255, dt.Rows[i]["ItemCode"].ToString());
          inputParam[2] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Revision"].ToString()));
          if (ultSaleorderdetail.Rows[i].Cells["Qty"].Value.ToString().Length > 0)
          {
            inputParam[3] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString()));
          }
          if (ultSaleorderdetail.Rows[i].Cells["QtyAdjShip"].Value.ToString().Length > 0)
          {
            inputParam[4] = new DBParameter("@QtyAdjShip", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["QtyAdjShip"].ToString()));
          }
          if (ultSaleorderdetail.Rows[i].Cells["QtyAdj"].Value.ToString().Length > 0)
          {
            inputParam[5] = new DBParameter("@QtyAdj", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["QtyAdj"].ToString()));
          }
          inputParam[6] = new DBParameter("@Remark", DbType.AnsiString, 255, "PLN Update Qty Shipped");
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNSaleOrderQtyShipped_Update", inputParam, outputParam);
          long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
          Out += "," + outValue.ToString();
          if (outValue == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0005");
            ultSaleorderdetail.Rows[i].CellAppearance.BackColor = Color.Yellow;
          }
          else
          {
            ultSaleorderdetail.Rows[i].CellAppearance.BackColor = Color.White;
          }
        }
      }
      if (Out.IndexOf("1") == 1)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        this.Search();
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.txtSaleNo.Text = string.Empty;
      this.txtItemCode.Text = string.Empty;
      this.txtPONo.Text = string.Empty;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultSaleorderdetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (string.Compare(colName, "Qty", true) == 0)
      {
        int Qty = DBConvert.ParseInt(e.Cell.Text.Trim());
        if (Qty < 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Qty" });
          e.Cancel = true;
        }
      }

      if (string.Compare(colName, "Qty", true) == 0)
      {
        int qty = DBConvert.ParseInt(e.Cell.Text.Trim());
        int totalWO = DBConvert.ParseInt(e.Cell.Row.Cells["OpenWOQty"].Value.ToString());
        if (qty > totalWO)
        {
          Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Ship Qty Must <= Total Opened WO Qty");
          e.Cancel = true;
        }
      }

      if (string.Compare(colName, "QtyAdjShip", true) == 0)
      {
        int QtyAdjShip = DBConvert.ParseInt(e.Cell.Text.Trim());
        if (QtyAdjShip < 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Qty Adj Ship" });
          e.Cancel = true;
        }
      }
      if (string.Compare(colName, "QtyAdj", true) == 0)
      {
        int QtyAdj = DBConvert.ParseInt(e.Cell.Text.Trim());
        if (QtyAdj < 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Qty Adj " });
          e.Cancel = true;
        }
      }
    }

    private void txtItemCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtSaleNo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void txtPONo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

  }
  #endregion Event
}
