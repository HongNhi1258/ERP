/*
 * Author       : Duong Minh 
 * CreateDate   : 07/05/2012
 * Description  : Add Detail (Adjustment In-Finish Good Warehouse)
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DaiCo.Shared;
using System.Collections;
using DaiCo.Shared.UserControls;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Application;
using Infragistics.Win.UltraWinGrid;
using DaiCo.Shared.Utility;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_04_004 : DaiCo.Shared.MainUserControl
  {
    #region Init
    public long receiptPid = long.MinValue;
    private long boxPid = long.MinValue;
    public viewFGH_04_004()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewFGH_04_004_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Process
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      string commandText = string.Empty;
      commandText += " SELECT PID, Location FROM TblWHFLocation";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultLocation.DataSource = dtSource;
      ultLocation.DisplayMember = "Location";
      ultLocation.ValueMember = "PID";
      ultLocation.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultLocation.DisplayLayout.Bands[0].Columns["Location"].Width = 200;
      ultLocation.DisplayLayout.Bands[0].Columns["PID"].Hidden = true;

      ultLocation.Value = 20;
    }

    /// <summary>
    /// Close Tab
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Save Allocate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.txtBoxCode.Text.Length == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Box Code is incorrect");
        return;
      }

      if (this.boxPid == long.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Box Id is incorrect");
        return;
      }

      if (this.ultLocation.Value == null || DBConvert.ParseLong(this.ultLocation.Value.ToString()) == long.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Location is incorrect");
        return;
      }

      string commandText = string.Empty;
      commandText = " SELECT COUNT(*) FROM TblWHFInStoreDetail WHERE BoxPID = " + this.boxPid + " AND InStorePID = " + this.receiptPid;
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) != 0)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.CloseTab();
          return;
        }
      }

      commandText = " SELECT COUNT(*)";
      commandText += " FROM TblWHFInStore ISS";
      commandText += "	  INNER JOIN TblWHFInStoreDetail ISD ON ISS.PID = ISD.InStorePID";
      commandText += " WHERE ISS.[Posting] = 0";
      commandText += "	  AND ISD.BoxPID = " + this.boxPid;
      commandText += "	  AND ISD.InStorePID != " + this.receiptPid;

      dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null && dt.Rows.Count > 0)
      {
        if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) != 0)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.CloseTab();
          return;
        }
      }

      DBParameter[] arrInputParam = new DBParameter[4];
      arrInputParam[0] = new DBParameter("@InstorePID", DbType.Int64, this.receiptPid);
      arrInputParam[1] = new DBParameter("@BoxPID", DbType.Int64, this.boxPid);
      arrInputParam[2] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(this.ultLocation.Value.ToString()));
      arrInputParam[3] = new DBParameter("@Note", DbType.AnsiString, 255, this.txtDescription.Text);
      DataBaseAccess.ExecuteStoreProcedure("spWHFInStoreDetailAdjustment_Insert", arrInputParam);
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.CloseTab();
    }

    private void txtBoxId_Leave(object sender, EventArgs e)
    {
      string commandText = "  SELECT		  BOX.Pid, BOX.SeriBoxNo, BOT.BoxTypeCode, DIM.Length, DIM.Width, DIM.Height";
      commandText += "        FROM			  TblWHFBox				      BOX ";
      commandText += "        INNER JOIN	TblBOMBoxType			    BOT		ON BOT.Pid    = BOX.BoxTypePID ";
      commandText += "        LEFT JOIN		TblWHFDimension			  DIM		ON DIM.Pid    = BOX.DimensionPID ";
      commandText += "        WHERE			  BOX.SeriBoxNo = '" + this.txtBoxId.Text + "' AND BOX.Status != 2";

      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dtSource.Rows.Count > 0)
      {
        this.txtBoxCode.Text = dtSource.Rows[0]["BoxTypeCode"].ToString();
        this.txtLength.Text = dtSource.Rows[0]["Length"].ToString();
        this.txtWidth.Text = dtSource.Rows[0]["Width"].ToString();
        this.txtHeight.Text = dtSource.Rows[0]["Height"].ToString();
        this.boxPid = DBConvert.ParseLong(dtSource.Rows[0]["Pid"].ToString());
        this.ultLocation.Enabled = true;
      }
      else
      {
        this.ultLocation.Enabled = false;
        this.txtBoxCode.Text = string.Empty;
        this.txtLength.Text = string.Empty;
        this.txtWidth.Text = string.Empty;
        this.txtHeight.Text = string.Empty;
      }
    }
    #endregion Process
  }
}

