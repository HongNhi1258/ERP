/*
  Author      : Duong Minh
  Date        : 12/05/2012
  Description : Adjustment Out
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
  public partial class viewFGH_03_006 : MainUserControl
  {
    #region Field
    public long outStorePid = long.MinValue;
    private string issCode = string.Empty;
    private int status = 0;
    #endregion Field

    #region Init
    public viewFGH_03_006()
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

      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      // Insert
      if (this.outStorePid == long.MinValue)
      {
        string commandText = string.Empty;
        commandText += "SELECT dbo.FWHFGetNewOutStoreCodeAdjustment()";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          this.issCode = dt.Rows[0][0].ToString();
          this.txtIssuingCode.Text = issCode;
        }
      }
      // Update
      else
      {
        DBParameter[] arrInputParam = new DBParameter[1];
        arrInputParam[0] = new DBParameter("@IssPid", DbType.Int64, this.outStorePid);

        DataSet ds = DataBaseAccess.SearchStoreProcedure("spWHFOutStoreIssuingToProduction_Select", arrInputParam);
        if (ds != null)
        {
          if (ds.Tables[0].Rows.Count > 0)
          {
            this.issCode = ds.Tables[0].Rows[0]["OutStoreCode"].ToString();
            this.status = DBConvert.ParseInt(ds.Tables[0].Rows[0]["Posting"].ToString());
            this.txtNote.Text = ds.Tables[0].Rows[0]["Note"].ToString();
            this.txtIssuingCode.Text = this.issCode;
          }

          this.ultSummary.DataSource = ds.Tables[1];
          for (int i = 0; i < ultSummary.Rows.Count; i++)
          {
            UltraGridRow row = ultSummary.Rows[i];
            if (DBConvert.ParseInt(row.Cells["Flag"].Value.ToString()) == 1)
            {
              row.CellAppearance.BackColor = Color.Yellow;
            }
          }
          this.ultOutStoreDetail.DataSource = ds.Tables[2];
        }
      }

      //Set Control
      this.SetStatusControl();
    }

    private void SetStatusControl()
    {
      if (this.status == 1)
      {
        this.txtNote.Enabled = false;
        this.chkConfirm.Checked = true;
        this.chkConfirm.Enabled = false;
        this.btnSave.Enabled = false;
        for (int i = 0; i < ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultOutStoreDetail.Rows[i];
          row.Activation = Activation.ActivateOnly;
        }
        this.btnGetTemplate.Enabled = false;
        this.btnAddDetail.Enabled = false;
        this.btnAddDetailFromFile.Enabled = false;
        this.btnDeleteItem.Enabled = false;
      }
      else
      {
        this.txtNote.Enabled = true;
        this.btnSave.Enabled = true;
        if (this.outStorePid != long.MinValue)
        {
          this.btnGetTemplate.Enabled = true;
          this.btnAddDetail.Enabled = true;
          this.btnAddDetailFromFile.Enabled = true;
          this.btnDeleteItem.Enabled = true;
          this.chkConfirm.Enabled = true;

          for (int i = 0; i < ultOutStoreDetail.Rows.Count; i++)
          {
            UltraGridRow row = ultOutStoreDetail.Rows[i];
            row.Cells["No"].Activation = Activation.ActivateOnly;
            row.Cells["ItemGroup"].Activation = Activation.ActivateOnly;
            row.Cells["BoxCode"].Activation = Activation.ActivateOnly;
            row.Cells["BoxName"].Activation = Activation.ActivateOnly;
            row.Cells["BoxId"].Activation = Activation.ActivateOnly;
            row.Cells["Length"].Activation = Activation.ActivateOnly;
            row.Cells["Width"].Activation = Activation.ActivateOnly;
            row.Cells["Height"].Activation = Activation.ActivateOnly;
            row.Cells["Set"].Activation = Activation.ActivateOnly;
            row.Cells["WorkOrder"].Activation = Activation.ActivateOnly;
            row.Cells["Location"].Activation = Activation.ActivateOnly;
            row.Cells["Check"].Activation = Activation.AllowEdit;
          }
        }
        else
        {
          this.chkConfirm.Enabled = false;
          this.btnGetTemplate.Enabled = false;
          this.btnAddDetail.Enabled = false;
          this.btnAddDetailFromFile.Enabled = false;
          this.btnDeleteItem.Enabled = false;
        }
      }

      for (int i = 0; i < ultSummary.Rows.Count; i++)
      {
        UltraGridRow row = ultSummary.Rows[i];
        row.Activation = Activation.ActivateOnly;
      }
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
    #endregion Function

    #region Event 
    private void btnSave_Click(object sender, EventArgs e)
    {
      //insert
      if (this.outStorePid == long.MinValue)
      {
        DBParameter[] arrInputParam = new DBParameter[3];
        arrInputParam[0] = new DBParameter("@UserWh", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        arrInputParam[1] = new DBParameter("@Posting", DbType.Int32, 0);
        arrInputParam[2] = new DBParameter("@Remark", DbType.AnsiString, 255, this.txtNote.Text);

        DBParameter[] arrOutputParam = new DBParameter[2];
        arrOutputParam[0] = new DBParameter("@Pid", DbType.Int32, Int32.MinValue);
        arrOutputParam[1] = new DBParameter("@OutStoreCode", DbType.String, string.Empty);

        DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreAdjustment_Insert", arrInputParam, arrOutputParam);

        this.outStorePid = DBConvert.ParseLong(arrOutputParam[0].Value.ToString());
        this.issCode = arrOutputParam[1].Value.ToString();
      }
      //update
      else
      {
        DBParameter[] arrInputParam = new DBParameter[4];
        arrInputParam[0] = new DBParameter("@UserWh", DbType.Int32, SharedObject.UserInfo.UserPid);
        int j = 0;
        if (this.chkConfirm.Checked == true)
        {
          j = 1;
        }
        arrInputParam[1] = new DBParameter("@Posting", DbType.Int32, j);
        arrInputParam[2] = new DBParameter("@Note", DbType.AnsiString, 255, this.txtNote.Text);
        arrInputParam[3] = new DBParameter("@Pid", DbType.Int64, this.outStorePid);
        DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreAdj_Update", arrInputParam);

        for (int i = 0; i < this.ultOutStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = this.ultOutStoreDetail.Rows[i];
          if (DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 1)
          {
            string commandText = " SELECT COUNT(*) FROM TblWHFBoxInStore WHERE BoxPID = " + DBConvert.ParseInt(row.Cells["Pid"].Value.ToString());
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt != null && dt.Rows.Count > 0)
            {
              if (DBConvert.ParseInt(dt.Rows[0][0].ToString()) == 0)
              {
                continue;
              }
            }
            else
            {
              continue;
            }

            //TBLWHFINBOX
            DBParameter[] arrInputParamBox = new DBParameter[2];
            arrInputParamBox[0] = new DBParameter("@SeriBoxNo", DbType.String, row.Cells["BoxId"].Value.ToString());
            arrInputParamBox[1] = new DBParameter("@Status", DbType.Int32, 2);
            DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInputParamBox);

            //TBLWHFINBOXINSTORE
            DBParameter[] arrParamBoxStore = new DBParameter[2];
            arrParamBoxStore[0] = new DBParameter("@BoxPID", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            arrParamBoxStore[1] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(row.Cells["LocationPid"].Value.ToString()));
            DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Insert", arrParamBoxStore);

            //TblWHFOutStoreDetail
            DBParameter[] arrParamBoxDetail = new DBParameter[2];
            arrParamBoxDetail[0] = new DBParameter("@OutStoreID", DbType.Int64, this.outStorePid);
            arrParamBoxDetail[1] = new DBParameter("@BoxPid", DbType.Int64, DBConvert.ParseLong(row.Cells["Pid"].Value.ToString()));
            DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetail_Delete", arrParamBoxDetail);
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.LoadData();
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "FGH_03_006_01";
      string sheetName = "ADJOUT";
      string outFileName = "FGH_03_006_01";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnAddDetail_Click(object sender, EventArgs e)
    {
      viewFGH_03_007 uc = new viewFGH_03_007();
      uc.issPid = this.outStorePid;
      WindowUtinity.ShowView(uc, "ADD DETAIL - ADJUSTMENT OUT", false, ViewState.ModalWindow);

      this.LoadData();
    }

    private void btnAddDetailFromFile_Click(object sender, EventArgs e)
    {
      viewFGH_03_008 uc = new viewFGH_03_008();
      uc.issPid = this.outStorePid;
      WindowUtinity.ShowView(uc, "ADD DETAIL FROM FILE - ADJUSTMENT OUT", false, ViewState.ModalWindow);

      this.LoadData();
    }

    private void btnDeleteItem_Click(object sender, EventArgs e)
    {
      int flag = 0;
      for (int i = 0; i < ultOutStoreDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultOutStoreDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 1)
        {
          long lPid = DBConvert.ParseLong(row.Cells["OSDPidDetail"].Value.ToString());
          if (lPid != long.MinValue)
          {
            flag = 1;

            string commandText = "  SELECT BIS.BoxPID BoxStore, OSD.Location, OSD.BoxPID BoxDetail ";
            commandText += "        FROM  TblWHFOutStoreDetail OSD " ;
            commandText += "          LEFT JOIN TblWHFBoxInStore  BIS ON OSD.BoxPID = BIS.BoxPID ";
            commandText += "        WHERE OSD.PID = " + lPid;
            DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
            if (dt.Rows.Count > 0 && dt.Rows[0]["BoxStore"].ToString().Length == 0)
            {
              DBParameter[] arrInputParamBox = new DBParameter[2];
              arrInputParamBox[0] = new DBParameter("@BoxPID", DbType.Int64, DBConvert.ParseLong(dt.Rows[0]["BoxDetail"].ToString()));
              arrInputParamBox[1] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(dt.Rows[0]["Location"].ToString()));
              DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Insert", arrInputParamBox);

              DBParameter[] arrInput = new DBParameter[2];
              arrInput[0] = new DBParameter("@SeriBoxNo", DbType.String, row.Cells["BoxId"].Value.ToString());
              arrInput[1] = new DBParameter("@Status", DbType.Int32, 2);

              DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInput);
            }

            DBParameter[] arrInputParam = new DBParameter[1];
            arrInputParam[0] = new DBParameter("@Pid", DbType.Int64, lPid);
            DataBaseAccess.ExecuteStoreProcedure("spWHFOutStoreDetailByPid_Delete", arrInputParam);
          }
        }
      }
      if (flag == 1)
      {
        WindowUtinity.ShowMessageSuccess("MSG0002");
        this.LoadData();
      }
    }

    private void ultInStoreDetail_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;
      e.Layout.Bands[0].Columns["OSDPidDetail"].Hidden = true;

      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["WorkOrder"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Set"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;

      e.Layout.Bands[0].Columns["WorkOrder"].Header.Caption = "WO";
      e.Layout.Bands[0].Columns["BoxName"].Header.Caption = "Box Name";
      e.Layout.Bands[0].Columns["BoxCode"].Header.Caption = "Box Code";
      e.Layout.Bands[0].Columns["BoxId"].Header.Caption = "Box Id";

      e.Layout.Bands[0].Columns["Check"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["No"].MinWidth = 45;
      e.Layout.Bands[0].Columns["No"].MaxWidth = 45;

      e.Layout.Bands[0].Columns["Location"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Location"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Check"].MinWidth = 45;
      e.Layout.Bands[0].Columns["Check"].MaxWidth = 45;

      e.Layout.Bands[0].Columns["Length"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Length"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Width"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Width"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Height"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Height"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["WorkOrder"].MinWidth = 50;
      e.Layout.Bands[0].Columns["WorkOrder"].MaxWidth = 50;

      e.Layout.Bands[0].Columns["Set"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Set"].MaxWidth = 50;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      ViewFGH_99_001 view = new ViewFGH_99_001();
      view.outStorePid = this.outStorePid;
      view.ncategory = 11;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void btnPrintDetail_Click(object sender, EventArgs e)
    {
      ViewFGH_99_001 view = new ViewFGH_99_001();
      view.outStorePid = this.outStorePid;
      view.ncategory = 12;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void ultSummary_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;

      e.Layout.Bands[0].Columns["Flag"].Hidden = true;

      e.Layout.Bands[0].Columns["IssQty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["RemainStockBalance"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;


      e.Layout.Bands[0].Columns["IssQty"].Header.Caption = "Iss Qty";
      e.Layout.Bands[0].Columns["RemainStockBalance"].Header.Caption = "Remain StockBalance";

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    #endregion Event
  }
}
