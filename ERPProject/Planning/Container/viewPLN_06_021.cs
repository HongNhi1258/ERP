/*
  Author  : 
  Date    : 01/02/2011
  Description : Process ContainerList Between Warehouse & Planning
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPLN_06_021 : MainUserControl
  {
    #region Field
    public long containerListPid = long.MinValue;
    #endregion Field

    #region Init
    public viewPLN_06_021()
    {
      InitializeComponent();
    }

    /// <summary>
    /// User Control Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPLN_06_021_Load(object sender, EventArgs e)
    {
      this.LoadControl();
      this.Search();
    }
    #endregion Init

    #region LoadData
    private void LoadControl()
    {
      string commandText = string.Empty;
      commandText += " SELECT [Status]";
      commandText += " FROM TblPLNContainerList";
      commandText += " WHERE Pid =" + this.containerListPid;

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 4)
        {
          this.btnFinish.Enabled = false;
        }
        else
        {
          this.btnFinish.Enabled = true;
        }
      }
    }

    /// <summary>
    /// Search Box Information
    /// </summary>
    private void Search()
    {
      string commandText = string.Empty;
      commandText += " SELECT CL.ContainerNo";
      commandText += " FROM TblPLNContainerList CL";
      commandText += " WHERE CL.Pid = " + this.containerListPid;

      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      this.txtContainerList.Text = dtCheck.Rows[0][0].ToString();

      DBParameter[] input = new DBParameter[1];

      input[0] = new DBParameter("@ContainerListPid", DbType.Int64, containerListPid);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNProcessContainerList", input);
      ultDetail.DataSource = dtSource;
      for (int i = 0; i < ultDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Flag"].Value.ToString()) != 0)
        {
          row.CellAppearance.BackColor = Color.LightBlue;
        }

        if (DBConvert.ParseInt(row.Cells["AllocatedQty"].Value.ToString()) != DBConvert.ParseInt(row.Cells["QtyPlanning"].Value.ToString()))
        {
          row.CellAppearance.BackColor = Color.LightPink;
        }
      }
    }
    #endregion LoadData

    #region Process
    /// <summary>
    /// Check Search Data (Customer,ShipDate,Container Name)
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CheckValidFinishInfo(out string message)
    {
      message = string.Empty;

      DBParameter[] input = new DBParameter[1];

      input[0] = new DBParameter("@ContainerListPid", DbType.Int64, containerListPid);

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNProcessContainerList", input);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (DBConvert.ParseInt(row["Remain"].ToString()) != 0)
          {
            message = Shared.Utility.FunctionUtility.GetMessage("ERRO118");
            return false;
          }
        }
      }
      else
      {
        message = Shared.Utility.FunctionUtility.GetMessage("ERRO118");
        return false;
      }

      dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNAllocateBoxRemainContainerList", input);
      if (dtSource != null && dtSource.Rows.Count > 0)
      {
        foreach (DataRow row in dtSource.Rows)
        {
          if (DBConvert.ParseInt(row["QtyRemain"].ToString()) != 0)
          {
            message = Shared.Utility.FunctionUtility.GetMessage("ERRO119");
            return false;
          }
        }
      }
      else
      {
        message = Shared.Utility.FunctionUtility.GetMessage("ERRO119");
        return false;
      }

      return true;
    }
    #endregion Process

    #region Event
    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Utility.SetPropertiesUltraGrid(ultDetail);
      e.Layout.AutoFitColumns = true;

      e.Layout.Bands[0].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["BoxTypeCode"].Header.Caption = "Box Code";
      e.Layout.Bands[0].Columns["BoxTypeName"].Header.Caption = "Box Name";

      e.Layout.Bands[0].Columns["Flag"].Hidden = true;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["BoxTypeCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["BoxTypeName"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyShipped"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["QtyPlanning"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Remain"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["AllocatedQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.NoEdit;
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnFinish_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      bool success = this.CheckValidFinishInfo(out message);
      if (!success)
      {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      DBParameter[] input = new DBParameter[1];
      input[0] = new DBParameter("@ContainerListPid", DbType.Int64, containerListPid);

      DataBaseAccess.ExecuteStoreProcedure("spPLNUpdateShippedQty", input);

      Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
      this.btnFinish.Enabled = false;
    }
    #endregion Event
  }
}
