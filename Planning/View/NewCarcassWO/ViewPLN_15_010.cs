/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: ViewPLN_15_010
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Planning
{
  public partial class ViewPLN_15_010 : MainUserControl
  {
    #region field
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      //Load CB CreateBy
      this.LoadCbCreateBy();

      //Load CB Status
      this.LoadCbStatus();

      //Load CB Wo
      this.LoadCbWO();

      //Load DD Reason
      this.LoadDDReason();
    }

    private void LoadCbCreateBy()
    {
      string cm = @"SELECT Pid, CAST(Pid AS VARCHAR) +' - '+ EmpName [Text] FROM VHRMEmployee";
      DataTable DT = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraCombo(ultCBCreateBy, DT, "Pid", "Text", "Pid");
    }
    private void LoadCbStatus()
    {
      string cm = @"SELECT 0 Value,'New' [Text]
                    UNION
                    SELECT 1 Value,'Confirm' [Text]";
      DataTable DT = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraCombo(ultCBStatus, DT, "Value", "Text", "Value");
    }
    private void LoadCbWO()
    {
      string cm = @"SELECT distinct WoInfoPID FROM TblPLNWOInfoDetailGeneral ORDER BY WoInfoPID";
      DataTable DT = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraCombo(ultCBWo, DT, "WoInfoPID", "WoInfoPID");
    }
    private void LoadCbItemCode(int WO)
    {
      string cm = string.Format(@"SELECT ItemCode FROM TblPLNWorkOrderConfirmedDetails
WHERE WorkOrderPid = {0}", WO);
      DataTable DT = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraCombo(ultCBItemCode, DT, "ItemCode", "ItemCode");
    }
    private void LoadCbRevision(string Itemcode)
    {
      string cm = string.Format(@"SELECT Revision FROM TblBOMItemInfo WHERE ItemCode = '{0}'", Itemcode);
      DataTable DT = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraCombo(ultCBRevision, DT, "Revision", "Revision");
    }

    private void LoadDDReason()
    {
      string cm = "SELECT Code,Value  FROM TblBOMCodeMaster where [Group] =9223 and DeleteFlag = 0";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      DaiCo.Shared.Utility.ControlUtility.LoadUltraDropDown(ultDDReason, dt, "Code", "Value", "Code");
    }




    private DataSet Createdata()
    {
      DataSet ds = new DataSet();
      DataTable taParent = new DataTable("TblMaster");
      taParent.Columns.Add("Pid", typeof(System.Int64));
      taParent.Columns.Add("CarcassWONo", typeof(System.String));
      taParent.Columns.Add("FromDate", typeof(System.DateTime));
      taParent.Columns.Add("ToDate", typeof(System.DateTime));
      taParent.Columns.Add("EmpName", typeof(System.String));
      taParent.Columns.Add("CreateDate", typeof(System.String));
      taParent.Columns.Add("Remark", typeof(System.String));
      taParent.Columns.Add("Status", typeof(System.String));
      ds.Tables.Add(taParent);

      DataTable taChild = new DataTable("TblDetail");
      taChild.Columns.Add("Pid", typeof(System.Int64));
      taChild.Columns.Add("TransactionPid", typeof(System.Int64));
      taChild.Columns.Add("WO", typeof(System.String));
      taChild.Columns.Add("ItemCode", typeof(System.String));
      taChild.Columns.Add("Revision", typeof(System.String));
      taChild.Columns.Add("Qty", typeof(System.String));
      taChild.Columns.Add("PackingDeadline", typeof(System.DateTime));
      taChild.Columns.Add("Reason", typeof(System.String));
      taChild.Columns.Add("PLNRemark", typeof(System.String));
      taChild.Columns.Add("CARRemark", typeof(System.String));
      ds.Tables.Add(taChild);

      ds.Relations.Add(new DataRelation("TblMaster_TblDetail", taParent.Columns["Pid"], taChild.Columns["TransactionPid"], false));
      return ds;
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      string storeName = "spPLNCarcassWO_Select";

      DBParameter[] param = new DBParameter[10];
      param[0] = new DBParameter("@CarcassWONo", DbType.String, txtCarcassWO.Text);
      if (ultCreateFrom.Value != null && ultCreateFrom.Value.ToString().Length > 0)
      {
        param[1] = new DBParameter("@CreateFrom", DbType.DateTime, ultCreateFrom.Value.ToString());
      }
      if (ultCreateTo.Value != null && ultCreateTo.Value.ToString().Length > 0)
      {
        param[2] = new DBParameter("@CreateTo", DbType.DateTime, ultCreateTo.Value.ToString());
      }
      if (ultCBCreateBy.Value != null && ultCBCreateBy.Value.ToString().Length > 0)
      {
        param[3] = new DBParameter("@CreateBy", DbType.Int64, ultCBCreateBy.Value.ToString());
      }
      if (ultFromDate.Value != null && ultFromDate.Value.ToString().Length > 0)
      {
        param[4] = new DBParameter("@DateFrom", DbType.DateTime, ultFromDate.Value.ToString());
      }
      if (ultToDate.Value != null && ultToDate.Value.ToString().Length > 0)
      {
        param[5] = new DBParameter("@DateTo", DbType.DateTime, ultToDate.Value.ToString());
      }
      if (ultCBStatus.Value != null && ultCBStatus.Value.ToString().Length > 0)
      {
        param[6] = new DBParameter("@Status", DbType.Int32, ultCBStatus.Value.ToString());
      }
      if (ultCBWo.Value != null && ultCBWo.Value.ToString().Length > 0)
      {
        param[7] = new DBParameter("@Wo", DbType.Int64, ultCBWo.Value.ToString());
      }
      if (ultCBItemCode.Value != null && ultCBItemCode.Value.ToString().Length > 0)
      {
        param[8] = new DBParameter("@ItemCode", DbType.String, ultCBItemCode.Value.ToString());
      }
      if (ultCBRevision.Value != null && ultCBRevision.Value.ToString().Length > 0)
      {
        param[9] = new DBParameter("@Revision", DbType.Int32, ultCBRevision.Value.ToString());
      }
      DataSet ds = DataBaseAccess.SearchStoreProcedure(storeName, param);
      DataSet dsSource = this.Createdata();
      if (ds != null && ds.Tables.Count > 1)
      {
        dsSource.Tables["TblMaster"].Merge(ds.Tables[0]);
        dsSource.Tables["TblDetail"].Merge(ds.Tables[1]);
      }
      ultraGridInformation.DataSource = dsSource;
      lbCount.Text = string.Format("Count: {0}", (ultraGridInformation.Rows.FilteredInRowCount));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      txtCarcassWO.Text = string.Empty;
      ultFromDate.Value = DBNull.Value;
      ultToDate.Value = DBNull.Value;
      ultCreateFrom.Value = DBNull.Value;
      ultCreateTo.Value = DBNull.Value;
      ultCBCreateBy.Value = DBNull.Value;
      ultCBItemCode.Value = DBNull.Value;
      ultCBWo.Value = DBNull.Value;
      ultCBRevision.Value = DBNull.Value;
      this.txtCarcassWO.Focus();
    }
    #endregion function

    #region event
    public ViewPLN_15_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewPLN_15_010_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupBoxSearch.Controls)
      {
        ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
      }

      //Init Data
      this.InitData();
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.SearchData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ClearCondition();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Auto search when user press Enter
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.SearchData();
      }
    }

    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      // Set Align
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }
      for (int j = 0; j < e.Layout.Bands[1].Columns.Count; j++)
      {
        e.Layout.Bands[1].Columns[j].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[1].Columns["Reason"].ValueList = this.ultDDReason;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["StatusID"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["TransactionPid"].Hidden = true;
      e.Layout.Bands[1].Columns["Error"].Hidden = true;
      e.Layout.Bands[1].Columns["Status"].Hidden = true;

      //set Align
      e.Layout.Bands[1].Columns["WO"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Revision"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Reason"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;


      // Set caption column
      e.Layout.Bands[0].Columns["CarcassWONo"].Header.Caption = "CarcassWO No";
      e.Layout.Bands[1].Columns["ItemCode"].Header.Caption = "Item Code";
      e.Layout.Bands[1].Columns["PackingDeadline"].Header.Caption = "Packing Deadline";
      e.Layout.Bands[1].Columns["PLNRemark"].Header.Caption = "PLN Remark";
      e.Layout.Bands[1].Columns["CARRemark"].Header.Caption = "CAR Remark";

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }
    private void ultraGridInformation_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultraGridInformation.Selected.Rows.Count > 0)
      {
        long pid = long.MinValue;
        if (!ultraGridInformation.Selected.Rows[0].HasParent())
        {
          pid = DBConvert.ParseLong(ultraGridInformation.Selected.Rows[0].Cells["Pid"].Value.ToString());
        }
        else
        {
          pid = DBConvert.ParseLong(ultraGridInformation.Selected.Rows[0].ParentRow.Cells["Pid"].Value.ToString());
        }
        ViewPLN_15_009 view = new ViewPLN_15_009();
        view.viewPid = pid;
        WindowUtinity.ShowView(view, "CarcassWo Detail", true, ViewState.MainWindow, FormWindowState.Normal);
        this.SearchData();
      }
    }

    private void btnNewCCWo_Click(object sender, EventArgs e)
    {
      ViewPLN_15_009 view = new ViewPLN_15_009();
      WindowUtinity.ShowView(view, "CarcassWo Detail", true, ViewState.MainWindow, FormWindowState.Normal);
      this.SearchData();
    }

    private void ultCBWo_ValueChanged(object sender, EventArgs e)
    {
      if (this.ultCBWo.Value != null && this.ultCBWo.Value.ToString().Length > 0)
      {
        this.LoadCbItemCode(DBConvert.ParseInt(this.ultCBWo.Value.ToString()));
        this.ultCBItemCode.Value = string.Empty;
        this.ultCBRevision.Value = string.Empty;
      }
    }

    private void ultCBItemCode_ValueChanged(object sender, EventArgs e)
    {
      if (this.ultCBItemCode.Value != null && this.ultCBItemCode.Value.ToString().Length > 0)
      {
        this.LoadCbRevision(this.ultCBItemCode.Value.ToString());
        this.ultCBRevision.Value = string.Empty;
      }
    }



    #endregion event



  }
}
