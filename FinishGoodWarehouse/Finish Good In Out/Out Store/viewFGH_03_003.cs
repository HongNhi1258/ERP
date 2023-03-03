/*
  Author      : Duong Minh
  Date        : 09/03/2012
  Description : Issuing Code For Container
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using DaiCo.Application;
using DaiCo.Objects;
using System.Collections;
using Infragistics.Win.UltraWinGrid;
using DaiCo.FinishGoodWarehouse;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using DaiCo.Shared;
using Infragistics.Win;
using System.Diagnostics;
using VBReport;
using System.IO;
using System.Data.SqlClient;
using DaiCo.Shared.DataSetSource.FinishGoodWarehouse;

namespace DaiCo.FinishGoodWarehouse
{
  public partial class viewFGH_03_003 : MainUserControl
  {
    #region Field
    public long outStorePid = long.MinValue;
    private int status = int.MinValue;
    private string outStoreCode = string.Empty;
    private int flagRefresh = int.MinValue;
    #endregion Field

    #region Init
    public viewFGH_03_003()
    {
      InitializeComponent();
    }

    private void viewFGH_03_001_Load(object sender, EventArgs e)
    {
      // Check Summary MonthLy Duong Minh 10/10/2011 START
      bool result = this.CheckSummary();
      if (result == false)
      {
        this.CloseTab();
        return;
      }
      // Check Summary MonthLy Duong Minh 10/10/2011 END

      this.LoadContainer();
      this.LoadData();
    }

    /// <summary>
    /// Check Summary PreMonth && PreYear
    /// </summary>
    /// <returns></returns>
    private bool CheckSummary()
    {
      DateTime firstDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
      int result = DateTime.Compare(firstDate, DateTime.Today);

      if (result <= 0)
      {
        int preMonth = 0;
        int preYear = 0;
        if (DateTime.Today.Month == 1)
        {
          preMonth = 12;
          preYear = DateTime.Today.Year - 1;
        }
        else
        {
          preMonth = DateTime.Today.Month - 1;
          preYear = DateTime.Today.Year;
        }

        string commandText = "SELECT COUNT(*) FROM TblWHFBalance WHERE Month = " + preMonth + " AND Year = " + preYear;
        DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if ((dtCheck == null) || (dtCheck != null && DBConvert.ParseInt(dtCheck.Rows[0][0].ToString()) == 0))
        {
          WindowUtinity.ShowMessageError("ERR0303", preMonth.ToString(), preYear.ToString());
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Load ContainerList
    /// </summary>
    private void LoadContainer()
    {
      string commandText = " SELECT Pid, ContainerNo + ' - ' + CONVERT(VARCHAR, ShipDate, 103) AS Name ";
      commandText += "       FROM TblPLNSHPContainer  ";
      commandText += "       WHERE (Confirm != 2)  ";
      commandText += "       ORDER BY Pid DESC  ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultcbContainer.DataSource = dtSource;
      ultcbContainer.DisplayMember = "Name";
      ultcbContainer.ValueMember = "Pid";
      ultcbContainer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultcbContainer.DisplayLayout.Bands[0].Columns["Name"].Width = 350;
      ultcbContainer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      if (this.outStorePid == long.MinValue)
      {
        DataTable dtOutStore = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHFGetNewOutStoreCodeContainer() NewOutStoreNo");
        if ((dtOutStore != null) && (dtOutStore.Rows.Count > 0))
        {
          this.txtIssuingCode.Text = dtOutStore.Rows[0]["NewOutStoreNo"].ToString();
        }
      }
      else
      {
        string commandText = string.Empty;
        commandText += " SELECT OS.ContainerPid";
        commandText += " FROM TblWHFOutStore OS";
	      commandText += " WHERE OS.PID = " + this.outStorePid;
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);

        if (dt != null && dt.Rows.Count > 0)
        {
          this.ultcbContainer.Value = DBConvert.ParseLong(dt.Rows[0][0].ToString());
        }
	
        DBParameter[] inputParam = new DBParameter[2];
        inputParam[0] = new DBParameter("@Container", DbType.Int64, DBConvert.ParseLong(this.ultcbContainer.Value.ToString()));
        inputParam[1] = new DBParameter("@OutStorePid", DbType.Int64, this.outStorePid);

        DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHFLoadDataForIssuingContainer_Select", inputParam);
        DataTable dtOutStoreInfo = dsSource.Tables[0];
        if (dtOutStoreInfo.Rows.Count > 0)
        {
          DataRow row = dtOutStoreInfo.Rows[0];
          this.txtIssuingCode.Text = row["OutStoreCode"].ToString();
          this.status = DBConvert.ParseInt(row["Posting"].ToString());
          this.txtNote.Text = row["Note"].ToString();
          this.ultcbContainer.Value = DBConvert.ParseLong(row["ContainerPid"].ToString());
          if (this.status == 1)
          {
            this.chkConfirm.Checked = true;
          }
          this.ultcbContainer.ReadOnly = true;
          this.outStorePid = DBConvert.ParseLong(row["PID"].ToString());
          this.outStoreCode = row["OutStoreCode"].ToString();
        }

        this.ultOutStore.DataSource = dsSource.Tables[1];
        this.ultOutStoreDetail.DataSource = dsSource.Tables[2];

        //Set Control
        this.SetStatusControl();
      }
    }
    #endregion Init

    #region Function
    private void SetStatusControl()
    {
      if (this.chkConfirm.Checked)
      {
        this.txtNote.ReadOnly = true;
        this.btnLoad.Enabled = false;
        this.chkConfirm.Enabled = false;
        this.btnSave.Enabled = false;
      }
      else
      {
        this.txtNote.ReadOnly = false;
        this.chkConfirm.Enabled = true;
        this.btnSave.Enabled = true;
        if (ultOutStoreDetail.Rows.Count == 0)
        {
          this.btnLoad.Enabled = true;
        }
        else
        {
          this.btnLoad.Enabled = false;
        }
      }
    }

    private bool CheckInvalid()
    {
      if (this.ultcbContainer.Value == null || DBConvert.ParseLong(this.ultcbContainer.Value.ToString()) == long.MinValue)
      {
        WindowUtinity.ShowMessageError("MSG0005", new string[] { "Container" });
        return false;
      }

      return true;
    }

    private bool CheckInvalidSave()
    {
      if (this.ultOutStoreDetail.Rows.Count != 0)
      {
        for (int i = 0; i < this.ultOutStore.Rows.Count; i++)
        {
          UltraGridRow row = this.ultOutStore.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Flag"].Value.ToString()) == 0)
          {
            WindowUtinity.ShowMessageError("MSG0005", new string[] { "Data Detail" });
            return false;
          }
        }
      }

      return true;
    }

    #endregion Function

    #region Event 
    /// <summary>
    /// Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLoad_Click(object sender, EventArgs e)
    {
      if (!this.CheckInvalid())
      {
        return;
      }
      // Get path from Folder
      string path = @"\PhanmemDENSOBHT8000";
      path = Path.GetFullPath(path);

      string curFile = path + @"\THONGTIN.txt";

      if (!File.Exists(curFile))
      {
        string message = string.Format(DaiCo.Shared.Utility.FunctionUtility.GetMessage("ERR0011"), "THONGTIN.txt");
        WindowUtinity.ShowMessageErrorFromText(message);
        return;
      }
      string[] a = File.ReadAllLines(curFile);

      if (a.Length == 0)
      {
        return;
      }
      List<string> list = new List<string>(a);
      for (int i = 0; i < list.Count; i++)
      {
        for (int j = i + 1; j < list.Count; j++)
        {
          if (list[i] == list[j])
          {
            list.RemoveAt(j);
            j--;
          }
        }
      }
      a = list.ToArray();
      int m = 0;
      for (m = 0; m < a.Length; m++)
      {
        DBParameter[] inputBarCode = new DBParameter[2];
        inputBarCode[0] = new DBParameter("@SeriBox", DbType.AnsiString, 16, a[m].ToString());
        inputBarCode[1] = new DBParameter("@UserPid", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        DataBaseAccess.ExecuteStoreProcedure("spWHFSeriBoxNoForCalculate_Insert", inputBarCode);
      }

      DBParameter[] input = new DBParameter[2];
      input[0] = new DBParameter("@Container", DbType.Int64, DBConvert.ParseInt(this.ultcbContainer.Value.ToString()));
      input[1] = new DBParameter("@UserPid", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spWHFGetDataForIssuingContainer_Select", input);

      this.ultOutStore.DataSource = dsSource.Tables[0];
      this.ultOutStoreDetail.DataSource = dsSource.Tables[1];

      this.btnLoad.Enabled = false;
      this.ultcbContainer.Enabled = false;

      for (int i = 0; i < ultOutStore.Rows.Count; i++)
      {
        UltraGridRow row = ultOutStore.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Flag"].Value.ToString()) == 0 || DBConvert.ParseInt(row.Cells["Qty"].Value.ToString()) == 0)
        {
          row.CellAppearance.BackColor = Color.LightBlue;
        }
        else
        {
          row.CellAppearance.BackColor = Color.White;
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (this.outStorePid == long.MinValue && this.ultOutStoreDetail.Rows.Count == 0)
      {
        return;
      }

      if (!this.CheckInvalidSave())
      {
        return;
      }
      string commandText = string.Empty;
      //insert
      if (this.outStorePid == long.MinValue)
      {
        int j = 0;

        if (this.chkConfirm.Checked)
        {
          j = 1;
        }
        else
        {
          j = 0;
        }

        DataTable dtOutStore = DataBaseAccess.SearchCommandTextDataTable("SELECT dbo.FWHFGetNewOutStoreCodeContainer() NewOutStoreNo");
        if ((dtOutStore != null) && (dtOutStore.Rows.Count > 0))
        {
          outStoreCode = dtOutStore.Rows[0]["NewOutStoreNo"].ToString();
        }

        DBParameter[] arrInputParam = new DBParameter[5];

        arrInputParam[0] = new DBParameter("@OutStoreCode", DbType.String, outStoreCode);
        arrInputParam[1] = new DBParameter("@UserWH", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        arrInputParam[2] = new DBParameter("@Posting", DbType.Int32, j.ToString());
        arrInputParam[3] = new DBParameter("@Note", DbType.String, txtNote.Text.ToString());
        arrInputParam[4] = new DBParameter("@ContainerPid", DbType.Int64, DBConvert.ParseLong(this.ultcbContainer.Value.ToString()));

        DBParameter[] arrOutputParam = new DBParameter[1];
        arrOutputParam[0] = new DBParameter("@Result", DbType.Int32, 0);

        DataBaseAccess.ExecuteStoreProcedure("spWHFOutStore_Insert", arrInputParam, arrOutputParam);
        long iResult = DBConvert.ParseLong(arrOutputParam[0].Value.ToString());
        this.outStorePid = iResult;
        //Detail
        for (int i = 0; i < ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultOutStoreDetail.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Delete"].Value.ToString()) == 0 )
          {
            //TblWHFOutStoreDetail
            DBParameter[] arrInputParamInStore = new DBParameter[3];

            arrInputParamInStore[0] = new DBParameter("@OutStoreID", DbType.Int64, iResult);
            arrInputParamInStore[1] = new DBParameter("@BoxPID", DbType.Int32, DBConvert.ParseInt(row.Cells["PID"].Value.ToString()));
            arrInputParamInStore[2] = new DBParameter("@Location", DbType.Int32, DBConvert.ParseInt(row.Cells["LocationPid"].Value.ToString()));

            DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetail_Insert", arrInputParamInStore);

            //TBLWHFINBOX
            DBParameter[] arrInputParamBox = new DBParameter[2];
            arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, row.Cells["BoxId"].Value.ToString());
            arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 3);

            DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);

            //TBLWHFINBOXINSTORE
            DBParameter[] arrInputParamBoxInStore = new DBParameter[1];
            arrInputParamBoxInStore[0] = new DBParameter("@BoxPID", DbType.Int64, DBConvert.ParseInt(row.Cells["PID"].Value.ToString()));

            DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Delete", arrInputParamBoxInStore);
          }
        }
      }
      //update
      else
      {
        int j = 0;
        if (this.chkConfirm.Checked)
        {
          j = 1;
        }
        else
        {
          j = 0;
        }

        //update TblWHFOutStore
        DBParameter[] arrInputParam = new DBParameter[5];
        arrInputParam[0] = new DBParameter("@OutStoreCode", DbType.String, this.txtIssuingCode.Text);
        arrInputParam[1] = new DBParameter("@Note", DbType.AnsiString, 255, this.txtNote.Text);
        arrInputParam[2] = new DBParameter("@UserWh", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        arrInputParam[3] = new DBParameter("@Posting", DbType.Int32, j);
        arrInputParam[4] = new DBParameter("@ContainerPid", DbType.Int64, DBConvert.ParseLong(this.ultcbContainer.Value.ToString()));
        DataBaseAccess.ExecuteStoreProcedure("spWHFOutStore_Update", arrInputParam);

        for (int i = 0; i < this.ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultOutStoreDetail.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Delete"].Value.ToString()) == 1)
          {
            commandText = string.Empty;
            commandText += " SELECT COUNT(*)";
            commandText += " FROM TblWHFOutStore OS";
            commandText += " INNER JOIN TblWHFOutStoreDetail OSD ON OS.PID = OSD.OutStoreID";
            commandText += " WHERE OSD.BoxPID = " + DBConvert.ParseLong(row.Cells["PID"].Value.ToString());

            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) > 0)
            {
              //TBLWHFINBOX
              DBParameter[] arrInputParamBox = new DBParameter[2];
              arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, row.Cells["BoxId"].Value.ToString());
              arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 2);

              DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);

              commandText = "SELECT Location FROM TblWHFOutStoreDetail WHERE OutStoreID = " + this.outStorePid + " AND BoxPID = " 
                          + DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
              dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (dt.Rows.Count > 0)
              {
                //TBLWHFINBOXINSTORE
                DBParameter[] arrParamBoxStore = new DBParameter[2];
                arrParamBoxStore[0] = new DBParameter("@BoxPID", DbType.Int64, DBConvert.ParseLong(row.Cells["PID"].Value.ToString()));
                arrParamBoxStore[1] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(dt.Rows[0]["Location"].ToString()));
                DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Insert", arrParamBoxStore);
              }

              //TblWHFOutStoreDetail
              DBParameter[] arrParamBoxDetail = new DBParameter[2];
              arrParamBoxDetail[0] = new DBParameter("@OutStoreID", DbType.Int64, this.outStorePid);
              arrParamBoxDetail[1] = new DBParameter("@BoxPid", DbType.Int64, DBConvert.ParseLong(row.Cells["PID"].Value.ToString()));
              DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetail_Delete", arrParamBoxDetail);
            }
          }
          else
          {
            if (DBConvert.ParseLong(row.Cells["OutStoreDetailPid"].Value.ToString()) == long.MinValue)
            {
              commandText = string.Empty;
              commandText += " SELECT COUNT(*)";
              commandText += " FROM TblWHFOutStore OS";
              commandText += " INNER JOIN TblWHFOutStoreDetail OSD ON OS.PID = OSD.OutStoreID";
              commandText += " WHERE OSD.BoxPID = " + DBConvert.ParseLong(row.Cells["PID"].Value.ToString());

              DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
              if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 0)
              {
                //TblWHFOutStoreDetail
                DBParameter[] arrInputParamInStore = new DBParameter[3];

                arrInputParamInStore[0] = new DBParameter("@OutStoreID", DbType.Int64, this.outStorePid);
                arrInputParamInStore[1] = new DBParameter("@BoxPID", DbType.Int32, DBConvert.ParseInt(row.Cells["PID"].Value.ToString()));
                arrInputParamInStore[2] = new DBParameter("@Location", DbType.Int32, DBConvert.ParseInt(row.Cells["LocationPid"].Value.ToString()));

                DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetail_Insert", arrInputParamInStore);

                //TBLWHFINBOX
                DBParameter[] arrInputParamBox = new DBParameter[2];
                arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, row.Cells["BoxId"].Value.ToString());
                arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 3);
                DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);

                //TBLWHFINBOXINSTORE
                DBParameter[] arrInputParamBoxInStore = new DBParameter[1];
                arrInputParamBoxInStore[0] = new DBParameter("@BoxPID", DbType.Int32, DBConvert.ParseInt(row.Cells["PID"].Value.ToString()));
                DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Delete", arrInputParamBoxInStore);
              }
            }
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");

      if (this.chkConfirm.Checked)
      {
        DBParameter[] Param = new DBParameter[2];
        Param[0] = new DBParameter("@Container", DbType.Int64, DBConvert.ParseLong(this.ultcbContainer.Value.ToString()));
        Param[1] = new DBParameter("@OutStorePid", DbType.Int64, this.outStorePid);
        DataBaseAccess.ExecuteStoreProcedure("spWHFUpdateItemAllocated_Edit", Param);
      }
      this.LoadData();
    }

    private void ultOutStore_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      if (flagRefresh != 0)
      {
        e.Layout.Reset();
        e.Layout.AutoFitColumns = true;
        e.Layout.ScrollStyle = ScrollStyle.Immediate;
        e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

        e.Layout.Bands[0].Columns["Flag"].Hidden = true;

        e.Layout.Bands[0].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["QtyAllocated"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["CanGetOnContainer"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["FreeQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["RealQtyOnBarCode"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["IssuedForContainer"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

        e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
        e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
        e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
        e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
        for (int i = 0; i < ultOutStore.Rows.Count; i++)
        {
          UltraGridRow row = ultOutStore.Rows[i];
          row.Activation = Activation.ActivateOnly;
        }
      }
    }

    private void ultOutStoreDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      if (this.flagRefresh != 0)
      {
        e.Layout.Reset();
        e.Layout.AutoFitColumns = true;
        e.Layout.ScrollStyle = ScrollStyle.Immediate;
        e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

        e.Layout.Bands[0].Columns["OutStoreDetailPid"].Hidden = true;
        e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
        e.Layout.Bands[0].Columns["PID"].Hidden = true;

        e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        e.Layout.Bands[0].Columns["LocationPid"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

        e.Layout.Bands[0].Columns["Delete"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

        e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
        e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

        e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
        e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
        e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
        e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
        for (int i = 0; i < ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultOutStoreDetail.Rows[i];
          row.Cells["No"].Activation = Activation.ActivateOnly;
          row.Cells["ItemGroup"].Activation = Activation.ActivateOnly;
          row.Cells["BoxId"].Activation = Activation.ActivateOnly;
          row.Cells["BoxCode"].Activation = Activation.ActivateOnly;
          row.Cells["BoxName"].Activation = Activation.ActivateOnly;
          row.Cells["WO"].Activation = Activation.ActivateOnly;
          row.Cells["Length"].Activation = Activation.ActivateOnly;
          row.Cells["Width"].Activation = Activation.ActivateOnly;
          row.Cells["Height"].Activation = Activation.ActivateOnly;
          row.Cells["Location"].Activation = Activation.ActivateOnly;
          if (this.status == 1)
          {
            row.Cells["Delete"].Activation = Activation.ActivateOnly;
          }
        }
      }
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      if (this.outStorePid == long.MinValue)
      {
        DataTable dt = new DataTable();
        this.flagRefresh = 0;
        this.ultOutStore.DataSource = dt;
        this.ultOutStoreDetail.DataSource = dt;
        this.btnLoad.Enabled = true;
        this.ultcbContainer.ReadOnly = false;
        this.ultcbContainer.Enabled = true;
        this.flagRefresh = 1;
      }
    }

    private void chkCheckAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.ultOutStoreDetail.Rows.Count != 0)
      {
        for (int i = 0; i < ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultOutStoreDetail.Rows[i];
          if (chkCheckAll.Checked)
          {
            row.Cells["Delete"].Value = 1;
          }
          else
          {
            row.Cells["Delete"].Value = 0;
          }
        }
      }
    }
    #endregion Event
  }
}
