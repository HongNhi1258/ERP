/*
  Author      : Ha Anh
  Description : List Consignee Customer muốn add thêm
  Date        : 23-09-2011
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using DaiCo.Shared.DataBaseUtility;
using System.Diagnostics;
using VBReport;
using System.IO;

namespace DaiCo.ERPProject
{
  public partial class viewCSD_02_005 : MainUserControl
  {
    #region Field
    
    public long customerPid = long.MinValue;
    private int rowFocus = int.MinValue;

    #endregion Field

    #region Init
    public viewCSD_02_005()
    {
      InitializeComponent();
    }

    /// <summary>
    /// load view
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewCSD_02_005_Load(object sender, EventArgs e)
    {
      this.LoadConsignee();
    }

    /// <summary>
    /// Load consignee
    /// </summary>
    private void LoadConsignee()
    {
      if (this.customerPid != long.MinValue)
      {
        DBParameter[] inputParam = new DBParameter[2];

        inputParam[0] = new DBParameter("@Customer", DbType.Int64, this.customerPid);
        inputParam[1] = new DBParameter("@Check", DbType.Int32, 1);
        
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spCSDConsignee_Select", inputParam);
        ultraGrid.DataSource = dtSource;
        ultraGrid.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
        ultraGrid.DisplayLayout.Override.AllowDelete = DefaultableBoolean.False;
        for (int i = 0; i < ultraGrid.DisplayLayout.Bands[0].Columns.Count; i++)
        {
          if (ultraGrid.DisplayLayout.Bands[0].Columns[i].Header.Caption != "Select")
          {
            ultraGrid.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
          }
        }
        for (int i = 0; i < ultraGrid.Rows.Count; i++ )
        {
          if (i % 2 == 1)
          {
            ultraGrid.Rows[i].Appearance.BackColor = Color.LightGreen;
          }
        }
        ultraGrid.DisplayLayout.Bands[0].Columns["ConsigneeCode"].Header.Caption = "Consignee Code";
      }
    }

    /// <summary>
    /// init ultraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultraGrid);
      e.Layout.AutoFitColumns = true;
      ultraGrid.DisplayLayout.Bands[0].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
    }

    #endregion Init

    #region Event
    /// <summary>
    /// Close click button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Save Click button
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      bool result = this.SaveConsigneeOfCustomer();
      if (result)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
        return;
      }
      this.LoadConsignee();
    }

    /// <summary>
    /// Save consignee of Customer
    /// </summary>
    /// <returns></returns>
    private bool SaveConsigneeOfCustomer()
    {
      DataTable dt = (DataTable)ultraGrid.DataSource;
      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState == DataRowState.Modified)
        {
          string storename = "spCSDCustomerConsignee_Insert";

          DBParameter[] inputParam = new DBParameter[4];
          if (DBConvert.ParseInt(dt.Rows[i]["Select"].ToString()) == 0)
          {
            inputParam[0] = new DBParameter("@Select", DbType.Int32, 0);
          }
          else
          {
            inputParam[0] = new DBParameter("@Select", DbType.Int32, 1);
          }

          inputParam[2] = new DBParameter("@Customer", DbType.Int64, this.customerPid);
          inputParam[3] = new DBParameter("@Consignee", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()));

          DBParameter[] outParam = new DBParameter[1];
          outParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
          DataBaseAccess.ExecuteStoreProcedure(storename, inputParam, outParam);

          if (DBConvert.ParseLong(outParam[0].Value.ToString()) == 0 && DBConvert.ParseLong(outParam[0].Value.ToString()) == long.MinValue)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// button search click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    /// <summary>
    /// search function
    /// </summary>
    private void Search()
    {
      string consignee = txtConsigneeCode.Text.Trim().ToLower();
      //scroll grid
      int countRow = 0;
      rowFocus++;
      string message = string.Empty;

      if (consignee.Length > 0)
      {
        for (int i = 0; i < ultraGrid.Rows.Count; i++)
        {
          ultraGrid.Rows[i].Selected = false;
        }
        for (int i = 0; i < ultraGrid.Rows.Count; i++)
        {
          if (ultraGrid.Rows[i].Cells["ConsigneeCode"].Value.ToString().ToLower().IndexOf(consignee) >= 0)
          {
            countRow++;
            if (countRow == rowFocus)
            {
              ultraGrid.Rows[i].Selected = true;
              ultraGrid.ActiveRowScrollRegion.ScrollRowIntoView(ultraGrid.Rows[i]);
            }
          }
        }
        if (rowFocus >= countRow)
        {
          rowFocus = 0;
        }
      }
    }

    /// <summary>
    /// consignee text change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void txtConsigneeCode_TextChanged(object sender, EventArgs e)
    {
      rowFocus = 0;
    }

    /// <summary>
    /// event double click row of grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultraGrid_DoubleClick(object sender, EventArgs e)
    {
      //bool selected = false;
      //try
      //{
      //  selected = ultraGrid.Selected.Rows[0].Selected;
      //}
      //catch
      //{
      //  selected = false;
      //}
      //if (!selected)
      //{
      //  return;
      //}
      //UltraGridRow row = ultraGrid.Selected.Rows[0];
      //long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
      //viewCSD_02_003 view = new viewCSD_02_003();
      //view.consigneePid = pid;
      ////view.customerPid = this.customerPid;
      //WindowUtinity.ShowView(view, "Consignee Information", false, ViewState.ModalWindow);
      //this.LoadConsignee();
    }
    #endregion Event

  }
}
