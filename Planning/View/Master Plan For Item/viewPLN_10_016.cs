/* Authour : 
 * Date : 28/9/2013
 * Description : Adjust Qty Shipped
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
  public partial class viewPLN_10_016 : MainUserControl
  {
    #region Field
    public bool value;
    public DataTable dtParent = new DataTable();
    public DataTable dtChild = new DataTable();
    #endregion Field

    #region Load
    public viewPLN_10_016()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Search
    /// </summary>
    private void Search()
    {
      this.btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[3];
      if (txtItemCode.Text.Length > 0)
      {
        input[0] = new DBParameter("@ItemCode", DbType.String, txtItemCode.Text.Trim());
      }
      if (txtSaleNo.Text.Length > 0)
      {
        input[1] = new DBParameter("@SaleNo", DbType.String, txtSaleNo.Text.Trim());
      }
      if (txtPONo.Text.Length > 0)
      {
        input[2] = new DBParameter("@PONo", DbType.String, txtPONo.Text.Trim());
      }
      DataSet dtSearch = DataBaseAccess.SearchStoreProcedure("spPLNWOLinkSOShippedQty_Select", input);
      if (dtSearch != null)
      {
        DataSet dsSource = this.CreateDataSet();
        dsSource.Tables["dtParent"].Merge(dtSearch.Tables[0]);
        dsSource.Tables["dtChild"].Merge(dtSearch.Tables[1]);
        ultSaleorderdetail.DataSource = dsSource;
      }
      this.btnSearch.Enabled = true;
    }

    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("dtParent");
      taParent.Columns.Add("SODetailPid", typeof(System.Int64));
      taParent.Columns.Add("SaleNo", typeof(System.String));
      taParent.Columns.Add("PONo", typeof(System.String));
      taParent.Columns.Add("SOPid", typeof(System.Int64));
      taParent.Columns.Add("ItemCode", typeof(System.String));
      taParent.Columns.Add("Revision", typeof(System.Int32));
      taParent.Columns.Add("ScheduleDelivery", typeof(System.String));
      taParent.Columns.Add("Qty", typeof(System.Int32));
      taParent.Columns.Add("QtyShipped", typeof(System.Int32));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("dtChild");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("SODetailPid", typeof(System.Int64));
      taChild.Columns.Add("SOPid", typeof(System.Int64));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.Int32));
      taChild.Columns.Add("WO", typeof(System.Int64));
      taChild.Columns.Add("Qty", typeof(System.Int32));
      taChild.Columns.Add("QtyShipped", typeof(System.Int32));
      taChild.Columns.Add("Errors", typeof(System.Int32));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("dtParent_dtChild", taParent.Columns["SODetailPid"], taChild.Columns["SODetailPid"], false));
      return ds;
    }

    private bool CheckValid(out string message)
    {
      message = string.Empty;
      for (int i = 0; i < ultSaleorderdetail.Rows.Count; i++)
      {
        UltraGridRow row = ultSaleorderdetail.Rows[i];
        for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = row.ChildBands[0].Rows[j];
          if (DBConvert.ParseInt(rowChild.Cells["Errors"].Value.ToString()) != 0)
          {
            message = "QtyShipped";
            return false;
          }
        }
      }
      return true;
    }

    private bool SaveData()
    {
      DataSet dsMain = (DataSet)ultSaleorderdetail.DataSource;
      DataTable dtMain = dsMain.Tables["dtChild"];
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];
        if (row.RowState == DataRowState.Modified)
        {
          // input
          DBParameter[] input = new DBParameter[2];
          input[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(row["Pid"].ToString()));
          input[1] = new DBParameter("@QtyShipped", DbType.Int32, DBConvert.ParseInt(row["QtyShipped"].ToString()));
          // output
          DBParameter[] output = new DBParameter[1];
          output[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          // excecute
          DataBaseAccess.ExecuteStoreProcedure("spPLNWOLinkSOQtyShipped_Update", input, output);
          long result = DBConvert.ParseLong(output[0].Value.ToString());
          if (result <= 0)
          {
            return false;
          }
        }
      }
      return true;
    }

    private void CheckQtyShipped(long soPid, string itemCode, int revision)
    {
      int qtyParent = 0;
      int qtyChild = 0;
      string select = string.Format(@"SOPid = {0} AND ItemCode = '{1}' AND Revision = {2}", soPid, itemCode, revision);
      DataRow[] foundRowParent = dtParent.Select(select);
      DataRow[] foundRowChild = dtChild.Select(select);

      if (foundRowParent.Length > 0)
      {
        for (int i = 0; i < foundRowParent.Length; i++)
        {
          qtyParent = qtyParent + DBConvert.ParseInt(foundRowParent[i]["QtyShipped"].ToString());
        }
      }

      if (foundRowChild.Length > 0)
      {
        for (int i = 0; i < foundRowChild.Length; i++)
        {
          qtyChild = qtyChild + DBConvert.ParseInt(foundRowChild[i]["QtyShipped"].ToString());
        }
      }

      for (int i = 0; i < ultSaleorderdetail.Rows.Count; i++)
      {
        UltraGridRow row = ultSaleorderdetail.Rows[i];
        for (int j = 0; j < row.ChildBands[0].Rows.Count; j++)
        {
          UltraGridRow rowChild = row.ChildBands[0].Rows[j];
          if (DBConvert.ParseLong(rowChild.Cells["SOPid"].Value.ToString()) == soPid &&
            rowChild.Cells["ItemCode"].Value.ToString() == itemCode &&
            DBConvert.ParseInt(rowChild.Cells["Revision"].Value.ToString()) == revision)
          {
            if (qtyChild != qtyParent)
            {
              rowChild.Cells["Errors"].Value = 1;
              rowChild.Appearance.BackColor = Color.Yellow;
            }
            else
            {
              rowChild.Cells["Errors"].Value = 0;
              rowChild.Appearance.BackColor = Color.White;
            }
          }
        }
      }
    }
    #endregion Load

    #region Event
    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_10_016_Load(object sender, EventArgs e)
    {

    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// Search
    /// </summary>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Init Layout
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSaleorderdetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.UseFixedHeaders = true;
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Set Align
      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Allow update, delete, add new
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      e.Layout.Bands[1].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[1].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[1].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      // Hide column
      e.Layout.Bands[0].Columns["SODetailPid"].Hidden = true;
      e.Layout.Bands[0].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SODetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["SOPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Hidden = true;
      e.Layout.Bands[1].Columns["Revision"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Errors"].Hidden = true;

      // Set color
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.Pink;
      e.Layout.Bands[0].Columns["QtyShipped"].CellAppearance.BackColor = Color.LightGreen;

      e.Layout.Bands[1].Columns["Qty"].CellAppearance.BackColor = Color.Pink;
      e.Layout.Bands[1].Columns["QtyShipped"].CellAppearance.BackColor = Color.Aqua;

      // Read only
      e.Layout.Bands[0].Columns["SaleNo"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["PONo"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["ItemCode"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Revision"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["Qty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["QtyShipped"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["WO"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["Qty"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    /// <summary>
    /// Save Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckValid(out message);
      if (!success)
      {
        WindowUtinity.ShowMessageError("ERR0001", message);
        return;
      }
      // Save Dataw
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0037", "Data");
      }
      // Search
      this.Search();
      this.chkExpandAll.Checked = false;
      this.chkExpandAll.Checked = true;
    }

    /// <summary>
    /// Clear Data
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClear_Click(object sender, EventArgs e)
    {
      this.txtSaleNo.Text = string.Empty;
      this.txtItemCode.Text = string.Empty;
      this.txtPONo.Text = string.Empty;
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

    /// <summary>
    /// Before Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSaleorderdetail_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      string text = e.Cell.Text.Trim();
      switch (columnName)
      {
        case "QtyShipped":
          if (DBConvert.ParseInt(text) < 0)
          {
            WindowUtinity.ShowMessageError("ERR0001", "ShippedWO >= 0");
            e.Cancel = true;
          }
          else
          {
            if (DBConvert.ParseInt(text) > DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()))
            {
              WindowUtinity.ShowMessageError("ERR0001", "QtyShipped <= " + DBConvert.ParseInt(e.Cell.Row.Cells["Qty"].Value.ToString()) + "");
              e.Cancel = true;
            }
          }
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// After Cell Update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultSaleorderdetail_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow rowCur = e.Cell.Row;
      switch (columnName)
      {
        case "QtyShipped":
          DataSet dsMain = (DataSet)ultSaleorderdetail.DataSource;
          dtParent = dsMain.Tables["dtParent"];
          dtChild = dsMain.Tables["dtChild"];
          long soPid = DBConvert.ParseLong(rowCur.Cells["SOPid"].Value.ToString());
          string itemCode = rowCur.Cells["ItemCode"].Value.ToString();
          int revision = DBConvert.ParseInt(rowCur.Cells["Revision"].Value.ToString());
          this.CheckQtyShipped(soPid, itemCode, revision);
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Expand All
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkExpandAll_CheckedChanged(object sender, EventArgs e)
    {
      if (chkExpandAll.Checked)
      {
        ultSaleorderdetail.Rows.ExpandAll(true);
      }
      else
      {
        ultSaleorderdetail.Rows.CollapseAll(true);
      }
    }
  }
  #endregion Event
}
