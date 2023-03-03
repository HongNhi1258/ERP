/*
  Author  : Vo Van Duy Qui
  Email   : qui_it@daico-furniture.com
  Date    : 30-11-2010
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using DaiCo.CustomerService.DataSetSource;
using Infragistics.Win.UltraWinGrid;
namespace DaiCo.CustomerService
{
  public partial class viewCSD_04_001 : MainUserControl
  {
    #region Init
    public viewCSD_04_001()
    {
      InitializeComponent();      
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_04_001_Load(object sender, EventArgs e)
    {
      Shared.Utility.ControlUtility.LoadComboboxCodeMst(cmbPriceBaseOn, 2006);
    }

    #endregion Init

    #region LoadData
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      long priceBaseOn = long.MinValue;
      priceBaseOn = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueCombobox(cmbPriceBaseOn));
      string description = txtDescription.Text.Trim();

      DBParameter[] input = new DBParameter[2];
      if (priceBaseOn != long.MinValue)
      {
        input[0] = new DBParameter("@PriceBaseOn", DbType.Int64, priceBaseOn);
      }
      if (description.Length > 0)
      {
        input[1] = new DBParameter("@Description", DbType.String, 256, "%" + description + "%");
      }

      dsCSDListHistoryItemPrice dsListItemPrice = new dsCSDListHistoryItemPrice();
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spCSDListHistoryChangeItemPrice", input);
      if (dsSource != null && dsSource.Tables.Count > 1)
      {
        dsListItemPrice.Tables["TblCSDItemPrice"].Merge(dsSource.Tables[0]);
        dsListItemPrice.Tables["TblCSDItemPriceDetail"].Merge(dsSource.Tables[1]);
      }
      ultBasePrice.DataSource = dsListItemPrice;
    }
    #endregion LoadData

    #region Event
    /// <summary>
    /// UltraGrid Mouse Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBasePrice_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultBasePrice.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (ultBasePrice.Selected.Rows[0].ParentRow == null) ? ultBasePrice.Selected.Rows[0] : ultBasePrice.Selected.Rows[0].ParentRow;
      viewCSD_04_002 view = new viewCSD_04_002();
      view.itemPricePid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      view.des = row.Cells["Description"].Value.ToString().Trim();
      WindowUtinity.ShowView(view, "Item Price Information", false, ViewState.MainWindow);
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultBasePrice_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultBasePrice);
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 90;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["PriceBaseOn"].Header.Caption = "Price Base On";
      e.Layout.Bands[0].Columns["PriceBaseOn"].MinWidth = 130;
      e.Layout.Bands[0].Columns["PriceBaseOn"].MaxWidth = 130;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Created By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Created Date";
      e.Layout.Bands[0].Columns["CreateDate"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
      e.Layout.Bands[0].Columns["CreateDate"].MinWidth = 120;
      e.Layout.Bands[0].Columns["CreateDate"].MaxWidth = 120;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 2; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }

      for (int j = 0; j < ultBasePrice.Rows.Count; j++)
      {
        string status = ultBasePrice.Rows[j].Cells["Status"].Value.ToString().Trim();
        if (status == "Confirmed")
        {
          ultBasePrice.Rows[j].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }

      e.Layout.Bands[1].Columns["ItemPricePid"].Hidden = true;
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["ItemCode"].MinWidth = 140;
      e.Layout.Bands[1].Columns["ItemCode"].MaxWidth = 140;
      e.Layout.Bands[1].Columns["Name"].Header.Caption = "Item Name";
      e.Layout.Bands[1].Columns["SaleCode"].Header.Caption = "Sale Code";
      e.Layout.Bands[1].Columns["SaleCode"].MinWidth = 120;
      e.Layout.Bands[1].Columns["SaleCode"].MaxWidth = 120;
      e.Layout.Bands[1].Columns["Price"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Price"].MinWidth = 150;
      e.Layout.Bands[1].Columns["Price"].MaxWidth = 150;
      e.Layout.Bands[1].Columns["Price"].Format = "#,###.##";
      e.Layout.Bands[1].Override.CellClickAction = CellClickAction.RowSelect;
    }

    /// <summary>
    /// Button Close Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Button Search Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// Button New Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewCSD_04_002 view = new viewCSD_04_002();
      WindowUtinity.ShowView(view, "Item Price Information", false, ViewState.MainWindow);
    }

    /// <summary>
    /// Button Delete Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      DialogResult result = WindowUtinity.ShowMessageConfirm("MSG0015");
      if (result == DialogResult.Yes)
      {
        int countDelete = 0;
        for (int i = 0; i < ultBasePrice.Rows.Count; i++)
        {
          int select = DBConvert.ParseInt(ultBasePrice.Rows[i].Cells["Selected"].Value.ToString());
          if (select == 1)
          {
            countDelete++;
            long pid = DBConvert.ParseLong(ultBasePrice.Rows[i].Cells["Pid"].Value.ToString());
            DBParameter[] input = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, pid) };
            DBParameter[] outputDelete = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

            DataBaseAccess.ExecuteStoreProcedure("spCSDItemPrice_Delete", input, outputDelete);
            if (outputDelete[0].Value.ToString().Trim() == "0")
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
              return;
            }
          }
        }

        if (countDelete == 0)
        {
          WindowUtinity.ShowMessageWarning("WRN0012");
          return;
        }
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0002");
        this.Search();
      }
    }
    #endregion Event
  }
}
