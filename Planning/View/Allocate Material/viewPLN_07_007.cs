/*
  Author      : Vo Van Duy Qui
  Date        : 24/02/2011 14:05
  Description : Close Work Order Materials
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_007 : MainUserControl
  {
    #region Field
    private bool loadWo = false;
    #endregion Field

    #region Init
    public viewPLN_07_007()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_07_007_Load(object sender, EventArgs e)
    {
      this.LoadComboStatus();
    }
    #endregion Init

    #region Function

    private void LoadComboStatus()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("Value", typeof(System.Int32));
      dt.Columns.Add("Text", typeof(System.String));

      DataRow row1 = dt.NewRow();
      row1["Value"] = 0;
      row1["Text"] = "All";
      dt.Rows.Add(row1);

      DataRow row2 = dt.NewRow();
      row2["Value"] = 1;
      row2["Text"] = "Closed";
      dt.Rows.Add(row2);

      //DataRow row3 = dt.NewRow();
      //row3["Value"] = 2;
      //row3["Text"] = "Not Close";
      //dt.Rows.Add(row3);

      ultCBStatus.DataSource = dt;
      ultCBStatus.ValueMember = "Value";
      ultCBStatus.DisplayMember = "Text";
      ultCBStatus.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
      ultCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load Data To UltraGrid With Work Order
    /// </summary>
    /// <param name="wo"></param>
    private void LoadGrid(long wo)
    {
      DBParameter[] inputParam = new DBParameter[] { new DBParameter("@WO", DbType.Int64, wo) };
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMaterialGroupStatus", inputParam);

      foreach (DataRow row in dtSource.Rows)
      {
        string statusRow = row["Status"].ToString().Trim();
        if (statusRow.CompareTo("Closed") == 0)
        {
          row["Closed"] = 1;
        }
      }
      ultData.DataSource = dtSource;
    }

    #endregion Function

    #region Event

    /// <summary>
    /// Ultra ComboBox Value Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraComboWO_ValueChanged(object sender, EventArgs e)
    {
      if (this.loadWo && ultraComboWO.SelectedRow != null)
      {
        long wo = DBConvert.ParseLong(ultraComboWO.SelectedRow.Cells[0].Value.ToString());
        this.LoadGrid(wo);
      }
      else if (this.loadWo && ultraComboWO.SelectedRow == null && ultraComboWO.Text.Length > 0)
      {
        WindowUtinity.ShowMessageErrorFromText("Work Order is closed or not exist");
      }
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Closed"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MaterialGroup"].Header.Caption = "Material Group";
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
    }

    /// <summary>
    /// UltraGird Mouse Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0)
      {
        int rowStatus = DBConvert.ParseInt(ultData.Selected.Rows[0].Cells["Closed"].Value.ToString());
        if (rowStatus == 1)
        {
          long wo = DBConvert.ParseLong(Shared.Utility.ControlUtility.GetSelectedValueUltraCombobox(ultraComboWO));
          viewPLN_07_008 uc = new viewPLN_07_008();
          uc.wo = wo;
          uc.materialGroup = ultData.Selected.Rows[0].Cells["MaterialGroup"].Value.ToString().Trim();
          Shared.Utility.WindowUtinity.ShowView(uc, "Re-Open Work Order Material", true, DaiCo.Shared.Utility.ViewState.ModalWindow);
          this.LoadGrid(wo);
        }
      }
    }

    /// <summary>
    /// UltraGrid Cell Change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string column = e.Cell.Column.ToString().Trim();
      long wo = long.MinValue;
      if (ultraComboWO.SelectedRow != null)
      {
        wo = DBConvert.ParseLong(ultraComboWO.SelectedRow.Cells[0].Value.ToString());
      }
      string materialGroup = e.Cell.Row.Cells["MaterialGroup"].Value.ToString().Trim();
      if (column.CompareTo("Closed") == 0)
      {
        int value = DBConvert.ParseInt(e.Cell.Row.Cells["Closed"].Value.ToString());
        DialogResult dgResult;
        if (value == 1) // chi cho Close WO, khong cho Open lai 
        {
          ///////
          return;
          //////
          dgResult = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0025", "Open");
          if (dgResult == DialogResult.Yes)
          {
            DBParameter[] inputParam = new DBParameter[2];
            inputParam[0] = new DBParameter("@WO", DbType.Int64, wo);
            inputParam[1] = new DBParameter("@MaterialGroup", DbType.AnsiString, 3, materialGroup);
            DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            DataBaseAccess.SearchStoreProcedure("spPLNCloseWOMaterial_Delete", inputParam, output);
            long result = DBConvert.ParseLong(output[0].Value.ToString());
            if (result != 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0026", "Open");
            }
          }
          else if (dgResult == DialogResult.No)
          {
            e.Cell.Row.Cells["Closed"].Value = 1;
            return;
          }
        }
        else
        {

          dgResult = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0025", "Close");
          if (dgResult == DialogResult.Yes)
          {
            DBParameter[] inputParam = new DBParameter[3];
            inputParam[0] = new DBParameter("@WO", DbType.Int64, wo);
            inputParam[1] = new DBParameter("@MaterialGroup", DbType.AnsiString, 3, materialGroup);
            inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
            DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

            DataBaseAccess.SearchStoreProcedure("spPLNCloseWOMaterial_Insert", inputParam, output);
            long result = DBConvert.ParseLong(output[0].Value.ToString());
            if (result != 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0026", "Closed");
            }
          }
          else if (dgResult == DialogResult.No)
          {
            e.Cell.Row.Cells["Closed"].Value = 0;
            return;
          }
        }
        this.LoadGrid(wo);
      }
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ultCBStatus_ValueChanged(object sender, EventArgs e)
    {
      int value = DBConvert.ParseInt(ControlUtility.GetSelectedValueUltraCombobox(ultCBStatus));
      ultraComboWO.Value = string.Empty;
      ultraComboWO.DataSource = null;
      ultData.DataSource = null;
      DBParameter[] input = new DBParameter[1];
      switch (value)
      {
        case 0:
          input[0] = new DBParameter("@Type", DbType.Int32, 0);
          break;
        case 1:
          input[0] = new DBParameter("@Type", DbType.Int32, 1);
          break;
        case 2:
          input[0] = new DBParameter("@Type", DbType.Int32, 2);
          break;
        default:
          break;
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNWorkOrderMaterialClosed_Select", input);
      ultraComboWO.DataSource = dtSource;
      ultraComboWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
      this.loadWo = true;
    }
    #endregion Event
  }
}
