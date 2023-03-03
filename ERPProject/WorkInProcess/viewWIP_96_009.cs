/*
  Author      : Ha Anh
  Date        : 17/06/2014
  Description : Save master detail for adjustment Team
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWIP_96_009 : MainUserControl
  {
    #region Field

    public string strBarcodePid = string.Empty;
    public long pid = long.MinValue;
    private int status = int.MinValue;
    private IList listDeletedPid = new ArrayList();

    private string pathExport = string.Empty;
    private string pathTemplate = string.Empty;
    #endregion Field

    #region Init

    public viewWIP_96_009()
    {
      InitializeComponent();
    }

    private void viewWIP_96_009_Load(object sender, EventArgs e)
    {
      this.LoadInit();
      this.LoadData();
    }

    #endregion Init

    #region Function

    private void LoadInit()
    {
      // Load Data Control...
      this.LoadCombo();
    }

    ///// <summary>
    ///// Load UltraCombo
    ///// </summary>
    private void LoadCombo()
    {
      string commandText = string.Empty;
      commandText += "SELECT Code, Value FROM TblBOMCodeMaster WHERE [Group] = 3006";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource != null)
      {
        ultCBReason.DataSource = dtSource;
        ultCBReason.DisplayMember = "Value";
        ultCBReason.ValueMember = "Code";
        // Format Grid
        ultCBReason.DisplayLayout.Bands[0].ColHeadersVisible = false;
        ultCBReason.DisplayLayout.AutoFitColumns = true;
        ultCBReason.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      }
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      string commandText = string.Format(@" SELECT	PAR.Pid PartPid, FUR.FurnitureCode, PAR.Partcode, PAR.WorkAreaPid, AREA.StandByEN FromTeam,
		                                            FUR.WorkOrder, FUR.CarcassCode, FUR.ItemCode, FUR.Revision,
		                                            CASE WHEN (AREA.Pid = 42 AND BC.FurnitureId IS NULL) THEN 0 
                                                     WHEN AREA.Pid IS NULL OR ISNULL(AREA.IsTransfer, 0) = 0 THEN 1
                                                     ELSE 0 END Error
                                            FROM	Split('{0}', ',') A
	                                            INNER JOIN	TblWIPFurniturePartcode PAR ON PAR.Pid = A.Item
	                                            INNER JOIN	TblWIPFurnitureCode FUR ON FUR.Pid = PAR.FurniturePid
                                              LEFT JOIN	TblQCDFurniture QC ON QC.FurnitureCode = FUR.FurnitureCode
	                                            LEFT JOIN	TblWIPWorkArea AREA ON AREA.Pid = PAR.WorkAreaPid
                                              LEFT JOIN (
		                                                      SELECT	DISTINCT FurnitureId
		                                                      FROM	TblWIPBoxIdCombineFurnitureId BC 
			                                                      INNER JOIN	TblWHFBox B ON B.PID = BC.BoxId AND B.[Status] = 2 
	                                                      )BC ON BC.FurnitureId = QC.Pid", strBarcodePid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dt != null)
      {
        ultData.DataSource = dt;

        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 1)
          {
            ultData.Rows[i].Appearance.BackColor = Color.Yellow;
          }
        }
      }

    }

    private bool CheckVaild(out string message)
    {
      message = string.Empty;
      if (ultCBReason.Value == null)
      {
        message = "Please input Reason Adjust";
        return false;
      }

      if (ultData.Rows.Count == 0)
      {
        message = "Have no data in grid";
        return false;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 1)
        {
          message = "Have error data in grid!";
          return false;
        }

        DBParameter[] inputParam = new DBParameter[4];
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["PartPid"].Value.ToString()));
        inputParam[1] = new DBParameter("@WO", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].Cells["WorkOrder"].Value.ToString()));
        inputParam[2] = new DBParameter("@ItemCode", DbType.String, ultData.Rows[i].Cells["ItemCode"].Value.ToString());
        inputParam[3] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["Revision"].Value.ToString()));

        DBParameter[] ouputParam = new DBParameter[1];
        ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

        DataBaseAccess.ExecuteStoreProcedure("spWIPPrintRoutingAdjustmentOut_CheckValid", inputParam, ouputParam);
        if (DBConvert.ParseLong(ouputParam[0].Value.ToString()) == 0)
        {
          message = "Total Furniture > Total Remain";
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
          return false;
        }

      }
      return true;
    }

    private bool SaveData()
    {
      // Save master info
      bool success = this.SaveInfo();
      if (success)
      {
        // Save detail
        success = this.SaveDetail();
      }
      else
      {
        success = false;
      }
      return success;
    }

    private bool SaveInfo()
    {
      DBParameter[] inputParam = new DBParameter[2];

      inputParam[0] = new DBParameter("@Reason", DbType.Int32, DBConvert.ParseInt(ultCBReason.Value.ToString()));
      inputParam[1] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

      DBParameter[] ouputParam = new DBParameter[1];
      ouputParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);

      DataBaseAccess.ExecuteStoreProcedure("spWIPBarcodeAdjustmentMaster_Insert", inputParam, ouputParam);
      // Gan Lai Pid
      this.pid = DBConvert.ParseLong(ouputParam[0].Value.ToString());
      if (this.pid == long.MinValue)
      {
        return false;
      }
      return true;
    }

    private bool SaveDetail()
    {
      // Save Detail
      DataTable dtMain = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtMain.Rows.Count; i++)
      {
        DataRow row = dtMain.Rows[i];

        DBParameter[] inputParam = new DBParameter[3];

        inputParam[0] = new DBParameter("@BarcodePid", DbType.Int64, DBConvert.ParseLong(row["PartPid"].ToString()));
        inputParam[1] = new DBParameter("@AdjustPid", DbType.Int64, this.pid);
        inputParam[2] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

        DBParameter[] outPutParam = new DBParameter[1];
        outPutParam[0] = new DBParameter("@Result", DbType.Int64, long.MinValue);
        DataBaseAccess.ExecuteStoreProcedure("spWIPBarcodeAdjustmentDetail_Insert", inputParam, outPutParam);
        if (DBConvert.ParseLong(outPutParam[0].Value.ToString()) <= 0)
        {
          return false;
        }
      }
      // End
      return true;
    }
    #endregion Function

    #region Event

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;

      // Hide column
      e.Layout.Bands[0].Columns["PartPid"].Hidden = true;
      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].Columns["WorkAreaPid"].Hidden = true;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString().ToLower();
      //switch (columnName)
      //{
      //  case "location":
      //    if (DBConvert.ParseLong(e.Cell.Row.Cells["Location"].Value.ToStr,ing()) <= 0)
      //    {
      //      WindowUtinity.ShowMessageError("ERR0001", "Location");
      //      return;
      //    }
      //    break;
      //  default:
      //    break;
      //}
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString();
      //string text = e.Cell.Text.Trim();
      //switch (columnName.ToLower())
      //{
      //  case "qty":
      //    if (text.Length > 0)
      //    {
      //      if (DBConvert.ParseDouble(text) <= 0)
      //      {
      //        WindowUtinity.ShowMessageError("ERR0001", "Qty");
      //        e.Cancel = true;
      //      }
      //    }
      //    break;
      //  default:
      //    break;
      //}
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {

    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;

      // Check Valid
      bool success = this.CheckVaild(out message);
      if (!success)
      {
        if (message == "Total Furniture > Total Remain")
        {
          if (MessageBox.Show("Total Furniture > Total Remain. Are you continute?", "Warmning", MessageBoxButtons.YesNo) == DialogResult.No)
          {
            return;
          }
        }
        else
        {
          WindowUtinity.ShowMessageError("ERR0001", message);
          return;
        }
      }

      // Save Data
      success = this.SaveData();
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }

      //Load Data
      btnSave.Enabled = false;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    #endregion Event
  }
}
