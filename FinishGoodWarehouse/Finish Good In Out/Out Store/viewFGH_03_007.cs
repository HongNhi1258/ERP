/*
 * Author       : Duong Minh 
 * CreateDate   : 12/05/2012
 * Description  : Add Detail (Adjustment Out-Finish Good Warehouse)
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
  public partial class viewFGH_03_007 : DaiCo.Shared.MainUserControl
  {
    #region Init
    public long issPid = long.MinValue;
    private long boxPid = long.MinValue;
    private long locationPid = long.MinValue;
    public viewFGH_03_007()
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
    }
    #endregion Init

    #region Process
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

      if (this.txtLocation.Text.Length == 0)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Location is incorrect");
        return;
      }

      if (this.boxPid == long.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Box Id is incorrect");
        return;
      }

      string commandText = string.Empty;
      commandText = " SELECT COUNT(*) FROM TblWHFOutStoreDetail WHERE BoxPID = " + this.boxPid + " AND InStorePID = " + this.issPid;
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
      commandText += " FROM TblWHFOutStore ISS";
      commandText += "	  INNER JOIN TblWHFOutStoreDetail ISD ON ISS.PID = ISD.OutStoreID";
      commandText += " WHERE ISS.[Posting] = 0";
      commandText += "	  AND ISD.BoxPID = " + this.boxPid;
      commandText += "	  AND ISD.OutStoreID != " + this.issPid;

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
      arrInputParam[0] = new DBParameter("@OutstorePID", DbType.Int64, this.issPid);
      arrInputParam[1] = new DBParameter("@BoxPID", DbType.Int64, this.boxPid);
      arrInputParam[2] = new DBParameter("@Location", DbType.Int64, this.locationPid);
      arrInputParam[3] = new DBParameter("@Note", DbType.AnsiString, 255, this.txtDescription.Text);
      DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetailAdjustment_Insert", arrInputParam);

      //TBLWHFINBOX
      DBParameter[] arrInputParamBox = new DBParameter[2];
      arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, this.txtBoxId.Text);
      arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 3);
      DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);

      //TBLWHFINBOXINSTORE
      DBParameter[] arrInputParamBoxInStore = new DBParameter[1];
      arrInputParamBoxInStore[0] = new DBParameter("@BoxPID", DbType.Int32, this.boxPid);

      DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Delete", arrInputParamBoxInStore);

      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.CloseTab();
    }

    private void txtBoxId_Leave(object sender, EventArgs e)
    {
      string commandText = "  SELECT BOX.Pid, BOX.SeriBoxNo, BOT.BoxTypeCode, DIM.Length, DIM.Width, DIM.Height, LOC.Location, LOC.Pid LocationPid";
      commandText += "        FROM TblWHFBoxInStore BIS  ";
      commandText += "        	INNER JOIN TblWHFBox BOX ON BIS.BoxPID = BOX.PID ";
      commandText += "        	INNER JOIN TblBOMBoxType BOT ON BOT.Pid = BOX.BoxTypePID ";
      commandText += "        	LEFT JOIN TblWHFDimension DIM ON DIM.Pid = BOX.DimensionPID ";
      commandText += "        	LEFT JOIN TblWHFLocation LOC ON BIS.Location = LOC.PID ";
      commandText += "        WHERE BOX.SeriBoxNo = '" + this.txtBoxId.Text.Trim() + "' AND BOX.[Status] = 2 ";
     
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      if (dtSource.Rows.Count > 0)
      {
        this.txtLocation.Text = dtSource.Rows[0]["Location"].ToString();
        this.txtBoxCode.Text = dtSource.Rows[0]["BoxTypeCode"].ToString();
        this.txtLength.Text = dtSource.Rows[0]["Length"].ToString();
        this.txtWidth.Text = dtSource.Rows[0]["Width"].ToString();
        this.txtHeight.Text = dtSource.Rows[0]["Height"].ToString();
        this.boxPid = DBConvert.ParseLong(dtSource.Rows[0]["Pid"].ToString());
        this.locationPid = DBConvert.ParseLong(dtSource.Rows[0]["LocationPid"].ToString());
      }
      else
      {
        this.txtLocation.Text = string.Empty;
        this.txtBoxCode.Text = string.Empty;
        this.txtLength.Text = string.Empty;
        this.txtWidth.Text = string.Empty;
        this.txtHeight.Text = string.Empty;
      }
    }
    #endregion Process
  }
}

