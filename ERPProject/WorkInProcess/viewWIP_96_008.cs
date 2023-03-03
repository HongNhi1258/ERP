/*
  Author      : 
  Date        : 
  Description : 
  Standard Form: view_SearchInfo.cs
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWIP_96_008 : MainUserControl
  {
    #region field
    public int partType = int.MinValue;
    public DataTable dtNewSource = new DataTable();
    #endregion field

    #region function
    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      string cm = string.Format(@"SELECT Value
                                  FROM TblBOMCodeMaster
                                  WHERE [Group] = 1992
	                                  AND Code = {0}", partType);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(cm);
      if (dt != null)
      {
        txtPartType.Text = dt.Rows[0]["Value"].ToString();
      }
      string cmText = string.Format(@"SELECT DISTINCT MS.Pid, MS.ItemCode, MS.Revision, MS.ItemCode + ' - ' + IT.Name ItemName
                                      FROM TblPLNProcessCarcass_RoutingDefaultMaster MS
	                                      INNER JOIN TblPLNProcessCarcass_RoutingDefaultMasterDetail MSDT ON MS.Pid = MSDT.RoutingDefaultMasterPid
																	                                      AND MS.IsDefault = 1
																	                                      AND PartType = {0}
	                                      INNER JOIN TblPLNProcessCarcass_RoutingDefaultMasterDescription MSDS ON MSDT.Pid = MSDS.RoutingDefaultMasterDetailPid
	                                      INNER JOIN TblBOMItemBasic IT ON MS.ItemCode = IT.ItemCode AND MS.Revision = IT.RevisionActive", partType);
      DataTable dtItem = DataBaseAccess.SearchCommandTextDataTable(cmText);
      Utility.LoadUltraCombo(ultraCBSearch, dtItem, "Pid", "ItemName", false, "Pid");
    }
    private DataSet CreateDataSet()
    {
      DataSet ds = new DataSet();

      ////Table Master
      DataTable dtMaster = new DataTable();
      dtMaster.Columns.Add("PidMaster", typeof(System.Int64));
      dtMaster.Columns.Add("ItemCode", typeof(System.String));
      dtMaster.Columns.Add("Revision", typeof(System.Int32));
      dtMaster.Columns.Add("PartGroup", typeof(System.String));
      ds.Tables.Add(dtMaster);

      //Table Detail

      DataTable dtDetail = new DataTable();
      dtDetail.Columns.Add("PidDetail", typeof(System.Int64));
      dtDetail.Columns.Add("PidMaster", typeof(System.Int64));
      dtDetail.Columns.Add("PartCode", typeof(System.String));
      dtDetail.Columns.Add("PartName", typeof(System.String));
      dtDetail.Columns.Add("PartType", typeof(System.String));
      dtDetail.Columns.Add("PartPercent", typeof(System.Double));
      dtDetail.Columns.Add("LocationDefault", typeof(System.String));
      dtDetail.Columns.Add("Select", typeof(System.Int32));
      ds.Tables.Add(dtDetail);

      //Table Description
      DataTable dtDescr = new DataTable();
      dtDescr.Columns.Add("PidDescription", typeof(System.Int64));
      dtDescr.Columns.Add("PidDetail", typeof(System.Int64));
      dtDescr.Columns.Add("Priority", typeof(System.Int32));
      dtDescr.Columns.Add("ProcessCode", typeof(System.String));
      dtDescr.Columns.Add("ProcessNameEN", typeof(System.String));
      dtDescr.Columns.Add("Capacity", typeof(System.Double));
      dtDescr.Columns.Add("SetupTime", typeof(System.Double));
      dtDescr.Columns.Add("ProcessTime", typeof(System.Double));
      dtDescr.Columns.Add("LeadTime1", typeof(System.Double));
      dtDescr.Columns.Add("LeadTime2", typeof(System.Double));
      dtDescr.Columns.Add("LeadTime3", typeof(System.Double));
      dtDescr.Columns.Add("LeadTime4", typeof(System.Double));
      dtDescr.Columns.Add("Notation", typeof(System.String));
      dtDescr.Columns.Add("NonCalculate", typeof(System.String));
      ds.Tables.Add(dtDescr);

      ds.Relations.Add(new DataRelation("dsMaster_dsDetail", new DataColumn[] { ds.Tables[0].Columns["PidMaster"] }, new DataColumn[] { ds.Tables[1].Columns["PidMaster"] }, false));
      ds.Relations.Add(new DataRelation("dsDetail_dsDescrip", new DataColumn[] { ds.Tables[1].Columns["PidDetail"] }, new DataColumn[] { ds.Tables[2].Columns["PidDetail"] }, false));

      return ds;
    }
    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      int paramNumber = 2;
      string storeName = "spWIPItemProcessByPartType";

      DBParameter[] inputParam = new DBParameter[paramNumber];
      if (ultraCBSearch.SelectedRow != null && DBConvert.ParseLong(ultraCBSearch.Value.ToString()) != long.MinValue)
      {
        inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(ultraCBSearch.Value.ToString()));
      }
      inputParam[1] = new DBParameter("@PartType", DbType.Int32, partType);

      DataSet dsSource = DataBaseAccess.SearchStoreProcedure(storeName, inputParam);
      DataSet dsData = this.CreateDataSet();
      dsData.Tables[0].Merge(dsSource.Tables[0]);
      dsData.Tables[1].Merge(dsSource.Tables[1]);
      dsData.Tables[2].Merge(dsSource.Tables[2]);

      ultraGridInformation.DataSource = dsData;
      lbCount.Text = string.Format("Count: {0}", ultraGridInformation.Rows.FilteredInRowCount);
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {

    }

    /// <summary>
    /// Set Auto Search Data When User Press Enter
    /// </summary>
    /// <param name="groupControl"></param>
    private void SetAutoSearchWhenPressEnter(Control groupControl)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in groupControl.Controls)
      {
        if (ctr.Controls.Count == 0)
        {
          ctr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
        }
        else
        {
          this.SetAutoSearchWhenPressEnter(ctr);
        }
      }
    }
    #endregion function

    #region event
    public viewWIP_96_008()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWIP_96_008_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      this.SetAutoSearchWhenPressEnter(groupBoxSearch);

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
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[0].Columns[i].Format = "#,##0.##";
        }
      }

      // Allow update, delete, add new
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[2].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

      // Hide column
      e.Layout.Bands[0].Columns["PidMaster"].Hidden = true;

      e.Layout.Bands[1].Columns["PidDetail"].Hidden = true;
      e.Layout.Bands[1].Columns["PidMaster"].Hidden = true;

      e.Layout.Bands[2].Columns["PidDescription"].Hidden = true;
      e.Layout.Bands[2].Columns["PidDetail"].Hidden = true;

      // Set caption column
      e.Layout.Bands[1].Columns["PartType"].Header.Caption = "Part Type";
      e.Layout.Bands[1].Columns["PartPercent"].Header.Caption = "Part Percent";
      e.Layout.Bands[1].Columns["LocationDefault"].Header.Caption = "Location\nDefault";
      e.Layout.Bands[1].Columns["PartName"].Header.Caption = "Part Name";
      e.Layout.Bands[1].Columns["PartCode"].Header.Caption = "Part Code";

      e.Layout.Bands[2].Columns["LeadTime1"].Header.Caption = "Lead Time\n<= 2pcs";
      e.Layout.Bands[2].Columns["LeadTime2"].Header.Caption = "Lead Time\n<= 6pcs";
      e.Layout.Bands[2].Columns["LeadTime3"].Header.Caption = "Lead Time\n<= 12pcs";
      e.Layout.Bands[2].Columns["LeadTime4"].Header.Caption = "Lead Time\n> 12pcs";
      e.Layout.Bands[2].Columns["ProcessCode"].Header.Caption = "Process\nCode";
      e.Layout.Bands[2].Columns["ProcessNameEN"].Header.Caption = "Process\nName";
      e.Layout.Bands[2].Columns["ProcessTime"].Header.Caption = "Process\nTime";
      e.Layout.Bands[2].Columns["SetupTime"].Header.Caption = "Setup\nTime";
      e.Layout.Bands[2].Columns["NonCalculate"].Header.Caption = "Non\nCalculate";


      // Set Width
      //e.Layout.Bands[0].Columns[""].MaxWidth = 100;
      //e.Layout.Bands[0].Columns[""].MinWidth = 100;

      // Set Column Style
      e.Layout.Bands[1].Columns["Select"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      // Set color
      //ultraGridInformation.Rows[0].Appearance.BackColor = Color.Yellow;
      //ultraGridInformation.Rows[0].Cells[""].Appearance.BackColor = Color.Yellow;
      //e.Layout.Bands[0].Columns[0].CellAppearance.BackColor = Color.Yellow;

      // Set header height
      e.Layout.Bands[1].ColHeaderLines = 2;
      e.Layout.Bands[2].ColHeaderLines = 2;

      //Read only

      // Format date (dd/MM/yy)
      //e.Layout.Bands[0].Columns[0].Format = ConstantClass.FORMAT_DATETIME;
      //e.Layout.Bands[0].Columns[0].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
    }


    public override void PopupMenu_MouseClick(object sender, MouseEventArgs e)
    {
      if (popupMenu.Items[0].Selected)
      {
        Utility.GetDataForClipboard(ultraGridInformation);
      }
    }

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (ultraGridInformation.Selected.Rows.Count > 0 || ultraGridInformation.Selected.Columns.Count > 0)
        {
          popupMenu.Show(ultraGridInformation, new Point(e.X, e.Y));
        }
      }
    }
    private void btnCopy_Click(object sender, EventArgs e)
    {
      DataTable ds = ((DataSet)ultraGridInformation.DataSource).Tables[1];
      int row = 0;
      long pidDetail = long.MinValue;
      if (ds != null)
      {
        for (int i = 0; i < ds.Rows.Count; i++)
        {
          if (DBConvert.ParseInt(ds.Rows[i]["Select"].ToString()) == 1)
          {
            row += 1;
            pidDetail = DBConvert.ParseLong(ds.Rows[i]["PidDetail"].ToString());
          }
        }
        if (row > 1)
        {
          WindowUtinity.ShowMessageError("ERR0114", "Part");
          return;
        }
        else if (row == 0)
        {
          WindowUtinity.ShowMessageError("ERR0115", "Part");
          return;
        }
        else
        {
          DBParameter[] input = new DBParameter[1];
          input[0] = new DBParameter("@DetailPid", DbType.Int64, pidDetail);
          dtNewSource = DataBaseAccess.SearchStoreProcedureDataTable("spWIPCopyProcessFromPartItem", input);
          this.CloseTab();
        }
      }
    }
    #endregion event

  }
}
