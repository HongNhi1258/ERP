/*
  Author      : Duong Minh
  Date        : 09/05/2012
  Description : Return From Customer
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
  public partial class viewFGH_04_006 : MainUserControl
  {
    #region Field
    public long inStorePid = long.MinValue;
    private string rcCode = string.Empty;
    private int status = 0;
    #endregion Field

    #region Init
    public viewFGH_04_006()
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

      // Customer
      this.LoadCustomer();

      this.LoadData();
    }
    #endregion Init

    #region Function
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

    // Load Customer
    private void LoadCustomer()
    {
      string commandText = string.Empty;
      commandText += " SELECT Pid, CustomerCode + ' - ' + Name Name";
      commandText += " FROM TblCSDCustomerInfo ";
      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCustomer.DataSource = dtSource;
      ultCustomer.DisplayMember = "Name";
      ultCustomer.ValueMember = "Pid";
      ultCustomer.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultCustomer.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultCustomer.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load Data
    /// </summary>
    private void LoadData()
    {
      // Insert
      if (this.inStorePid == long.MinValue)
      {
        string commandText = string.Empty;
        commandText += "SELECT dbo.FWHFGetNewInStoreCodeReturnFromCustomer()";
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          this.rcCode = dt.Rows[0][0].ToString();
          this.txtReceivingCode.Text = rcCode;
        }
      }
      // Update
      else
      {
        string strCommandText = "SELECT InStoreCode, Note, Posting, Customer FROM TblWHFInStore WHERE PID = " + this.inStorePid;
        DataTable dt = DataBaseAccess.SearchCommandTextDataTable(strCommandText);
        if (dt != null && dt.Rows.Count > 0)
        {
          this.rcCode = dt.Rows[0][0].ToString();
          this.txtNote.Text = dt.Rows[0]["Note"].ToString();
          this.txtReceivingCode.Text = this.rcCode;
          this.status = DBConvert.ParseInt(dt.Rows[0]["Posting"].ToString());
          this.ultCustomer.Value = DBConvert.ParseInt(dt.Rows[0]["Customer"].ToString());
        }
        else
        {
          return;
        }

        string commandText = "  SELECT		  ROW_NUMBER() OVER( ORDER BY BOX.PID ) AS [No] , ";
        commandText += "                            ISD.PID, BOX.SeriBoxNo, BOT.BoxTypeCode, DIM.[Length], DIM.Width, DIM.Height, ";
        commandText += "                            LOC.Location, LOC.PID LocationPid, 0 [Check]";
        commandText += "        FROM			  TblWHFInStoreDetail		ISD ";
        commandText += "        INNER JOIN	TblWHFInStore			    IST		ON IST.PID = ISD.InStorePID ";
        commandText += "        INNER JOIN	TblWHFBox				      BOX		ON BOX.PID = ISD.BoxPID ";
        commandText += "        INNER JOIN	TblBOMBoxType			    BOT		ON BOT.Pid = BOX.BoxTypePID ";
        commandText += "        LEFT JOIN		TblWHFDimension			  DIM		ON DIM.Pid = BOX.DimensionPID ";
        commandText += "        LEFT JOIN		TblWHFLocation			  LOC		ON	ISD.Location = LOC.PID ";
        commandText += "        WHERE			  IST.PID = " + this.inStorePid;
        commandText += "        ORDER BY [No]			  ";

        dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
        this.ultInStoreDetail.DataSource = dt;
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
        for (int i = 0; i < ultInStoreDetail.Rows.Count; i++)
        {
          UltraGridRow row = ultInStoreDetail.Rows[i];
          row.Activation = Activation.ActivateOnly;
        }
        this.btnGetTemplate.Enabled = false;
        this.btnAddDetail.Enabled = false;
        this.btnAddDetailFromFile.Enabled = false;
        this.btnDeleteItem.Enabled = false;
        this.btnDelete.Enabled = false;
      }
      else
      {
        this.txtNote.Enabled = true;
        this.chkConfirm.Enabled = true;
        this.btnSave.Enabled = true;
        if (this.inStorePid != long.MinValue)
        {
          this.btnGetTemplate.Enabled = true;
          this.btnAddDetail.Enabled = true;
          this.btnAddDetailFromFile.Enabled = true;
          this.btnDeleteItem.Enabled = true;
          this.btnDelete.Enabled = true;
          this.chkConfirm.Enabled = true;
        }
        else
        {
          this.chkConfirm.Enabled = false;
          this.btnGetTemplate.Enabled = false;
          this.btnAddDetail.Enabled = false;
          this.btnAddDetailFromFile.Enabled = false;
          this.btnDeleteItem.Enabled = false;
          this.btnDelete.Enabled = false;
        }
      }
    }
    #endregion Function

    #region Event 
    private void btnSave_Click(object sender, EventArgs e)
    {

      if (this.ultCustomer.Value == null || DBConvert.ParseLong(this.ultCustomer.Value.ToString()) == long.MinValue)
      {
        Shared.Utility.WindowUtinity.ShowMessageErrorFromText("Fill Customer Pls !!");
        return;
      }
      //insert
      if (this.inStorePid == long.MinValue)
      {
        DBParameter[] arrInputParam = new DBParameter[4];
        arrInputParam[0] = new DBParameter("@UserWh", DbType.Int32, Shared.Utility.SharedObject.UserInfo.UserPid);
        arrInputParam[1] = new DBParameter("@Posting", DbType.Int32, 0);
        arrInputParam[2] = new DBParameter("@Remark", DbType.AnsiString, 255, this.txtNote.Text);
        arrInputParam[3] = new DBParameter("@Customer", DbType.String, this.ultCustomer.Value.ToString());
        DBParameter[] arrOutputParam = new DBParameter[2];
        arrOutputParam[0] = new DBParameter("@Pid", DbType.Int32, Int32.MinValue);
        arrOutputParam[1] = new DBParameter("@InStoreCode", DbType.String, string.Empty);

        DataBaseAccess.ExecuteStoreProcedure("spWHFInStoreReturnFromCustomer_Insert", arrInputParam, arrOutputParam);

        this.inStorePid = DBConvert.ParseLong(arrOutputParam[0].Value.ToString());
        this.rcCode = arrOutputParam[1].Value.ToString();
      }
      //update
      else
      {
        DBParameter[] arrInputParam = new DBParameter[5];
        arrInputParam[0] = new DBParameter("@UserWh", DbType.Int32, SharedObject.UserInfo.UserPid);
        int j = 0;
        if (this.chkConfirm.Checked == true)
        {
          j = 1;
        }
        arrInputParam[1] = new DBParameter("@Posting", DbType.Int32, j);
        arrInputParam[2] = new DBParameter("@Note", DbType.AnsiString, 255, this.txtNote.Text);
        arrInputParam[3] = new DBParameter("@Pid", DbType.Int64, this.inStorePid);
        arrInputParam[4] = new DBParameter("@Customer", DbType.String, this.ultCustomer.Value.ToString());
        DataBaseAccess.ExecuteStoreProcedure("spWHFInStoreReturnToCustomer_Update", arrInputParam);

        string strUrl = string.Empty;

        if (this.chkConfirm.Checked)
        {
          string commandText = "SELECT BOX.SeriBoxNo, BOX.PID, ISD.Location FROM TblWHFInStoreDetail ISD INNER JOIN TblWHFBox BOX ON ISD.BoxPID = BOX.Pid WHERE ISD.InStorePID = " + this.inStorePid;
          DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

          foreach (DataRow dr in dtSource.Rows)
          {
            DBParameter[] arrInput = new DBParameter[2];
            arrInput[0] = new DBParameter("@SeriBoxNo", DbType.String, dr["SeriBoxNo"].ToString());
            arrInput[1] = new DBParameter("@Status", DbType.Int32, 2);
            DataBaseAccess.ExecuteStoreProcedure("spWHFBox_UpdateStatus", arrInput);

            DBParameter[] arrInputBox = new DBParameter[2];
            arrInputBox[0] = new DBParameter("@BoxPID", DbType.Int64, DBConvert.ParseLong(dr["PID"].ToString()));
            arrInputBox[1] = new DBParameter("@Location", DbType.Int64, DBConvert.ParseLong(dr["Location"].ToString()));
            DataBaseAccess.ExecuteStoreProcedure("spWHFBoxInStore_Insert", arrInputBox);
          }
        }
      }
      WindowUtinity.ShowMessageSuccess("MSG0004");
      this.LoadData();
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "FGH_04_003_01";
      string sheetName = "ADJIN";
      string outFileName = "FGH_04_003_01";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }

    private void btnAddDetail_Click(object sender, EventArgs e)
    {
      viewFGH_04_004 uc = new viewFGH_04_004();
      uc.receiptPid = this.inStorePid;
      WindowUtinity.ShowView(uc, "ADD DETAIL - RETURN FROM CUSTOMER", false, ViewState.ModalWindow);

      this.LoadData();
    }

    private void btnAddDetailFromFile_Click(object sender, EventArgs e)
    {
      viewFGH_04_005 uc = new viewFGH_04_005();
      uc.receiptPid = this.inStorePid;
      uc.flagReturn = 1;
      WindowUtinity.ShowView(uc, "ADD DETAIL FROM FILE - RETURN FROM CUSTOMER", false, ViewState.ModalWindow);

      this.LoadData();
    }

    private void btnDeleteItem_Click(object sender, EventArgs e)
    {
      int flag = 0;
      for (int i = 0; i < ultInStoreDetail.Rows.Count; i++)
      {
        UltraGridRow row = ultInStoreDetail.Rows[i];
        if (DBConvert.ParseInt(row.Cells["Check"].Value.ToString()) == 1)
        {
          long lPid = DBConvert.ParseLong(row.Cells["PID"].Value.ToString());
          if (lPid != long.MinValue)
          {
            flag = 1;
            DBParameter[] arrInputParam = new DBParameter[1];
            arrInputParam[0] = new DBParameter("@Pid", DbType.Int64, lPid);
            DataBaseAccess.ExecuteStoreProcedure("spWHFInStoreDetail_Delete", arrInputParam);
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
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultInStoreDetail);

      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      if (this.status == 0)
      {
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;
      }
      else
      {
        e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.False;
      }

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
      }

      e.Layout.Bands[0].Columns["PID"].Hidden = true;
      e.Layout.Bands[0].Columns["LocationPid"].Hidden = true;

      e.Layout.Bands[0].Columns["SeriBoxNo"].Header.Caption = "Box Id";
      e.Layout.Bands[0].Columns["BoxTypeCode"].Header.Caption = "Box Code";

      e.Layout.Bands[0].Columns["Check"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Length"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Width"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Height"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["No"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    
      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (this.inStorePid != long.MinValue)
      {
        if (this.chkConfirm.Checked == false)
        {
          DBParameter[] arrInputParam = new DBParameter[1];
          arrInputParam[0] = new DBParameter("@Pid", DbType.Int64, this.inStorePid);

          DataBaseAccess.ExecuteStoreProcedure("spWHFInStore_Delete", arrInputParam);
        
          DBParameter[] arrInputParamDetail = new DBParameter[1];
          arrInputParamDetail[0] = new DBParameter("@InStorePID", DbType.Int64, this.inStorePid);
          DataBaseAccess.ExecuteStoreProcedure("spWHFInStoreDetailByInStorePid_Delete", arrInputParamDetail);

          WindowUtinity.ShowMessageSuccess("MSG0002");
          this.CloseTab();
        }
      }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
      ViewFGH_99_001 view = new ViewFGH_99_001();
      view.inStoreCode = this.rcCode;
      view.ncategory = 7;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }

    private void btnPrintDetail_Click(object sender, EventArgs e)
    {
      ViewFGH_99_001 view = new ViewFGH_99_001();
      view.inStoreCode = this.rcCode;
      view.ncategory = 8;
      Shared.Utility.WindowUtinity.ShowView(view, "Report", false, Shared.Utility.ViewState.ModalWindow);
    }
    #endregion Event
  }
}
