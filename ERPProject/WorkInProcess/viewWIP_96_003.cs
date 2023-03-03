/*************************************
 * Author: Nguyen Ngoc Tien          *
 * Create date: 13/10/2014           *
 * Description: Groups List          *
 * ***********************************/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewWIP_96_003 : MainUserControl
  {
    public viewWIP_96_003()
    {
      InitializeComponent();
    }

    #region Init

    public int RuleShow = 0;
    public int Index = 0;
    public DataTable dtNewSource = new DataTable();

    #endregion

    #region Function

    private void Search()
    {
      DBParameter[] param = new DBParameter[3];
      if (txtGroupCode.TextLength > 0)
      {
        param[1] = new DBParameter("@GroupCode", DbType.String, txtGroupCode.Text.Trim());
      }
      if (txtGroupName.TextLength > 0)
      {
        param[2] = new DBParameter("@GroupName", DbType.String, txtGroupName.Text.Trim());
      }
      DataSet dsSource = DataBaseAccess.SearchStoreProcedure("spPLNRoutingDefault_Select", param);
      dsSource.Relations.Add(new DataRelation("dsMaser_dsDetail", new DataColumn[] { dsSource.Tables[0].Columns["Pid"] }, new DataColumn[] { dsSource.Tables[1].Columns["RoutingDefaultPid"] }, false));
      ultData.DataSource = dsSource;
    }

    private void LoadDDTeam()
    {
      string commandText = "SELECT Pid, WorkAreaCode, WorkAreaName FROM VWIPCARSectionTeamInfo";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultDDTeam.DataSource = dtSource;
      ultDDTeam.DisplayMember = "WorkAreaCode";
      ultDDTeam.ValueMember = "Pid";
      ultDDTeam.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultDDTeam.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    #endregion

    #region Events

    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
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

      for (int i = 0; i < e.Layout.Bands[1].Columns.Count; i++)
      {
        Type colType = e.Layout.Bands[1].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
          e.Layout.Bands[1].Columns[i].Format = "#,##0.##";
        }
      }

      //Read Only
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        UltraGridRow ur = ultData.Rows[i];
        ur.Activation = Activation.ActivateOnly;
        for (int j = 0; j < ur.ChildBands[0].Rows.Count; j++)
        {
          ur.ChildBands[0].Rows[j].Activation = Activation.ActivateOnly;
        }
      }
      e.Layout.Bands[0].Override.RowAppearance.BackColor = Color.LightYellow;
      e.Layout.Bands[1].Override.RowAppearance.BackColor = Color.LightCyan;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["RoutingDefaultPid"].Hidden = true;
      e.Layout.Bands[1].Columns["ProcessCode"].Hidden = true;
      e.Layout.Bands[1].Columns["WorkAreaCode"].ValueList = ultDDTeam;

      //Header
      e.Layout.Bands[0].ColHeaderLines = 2;
      e.Layout.Bands[01].ColHeaderLines = 2;
      e.Layout.Bands[0].Columns["TotalSetupTime"].Header.Caption = "Total Setup\nTime";
      e.Layout.Bands[0].Columns["TotalProcessTIme"].Header.Caption = "Total Process\nTime";
      e.Layout.Bands[0].Columns["TotalLeadTime1"].Header.Caption = "Total Lead Time\n(<= 2pcs)";
      e.Layout.Bands[0].Columns["TotalLeadTime2"].Header.Caption = "Total Lead Time\n(2 < pcs <= 6)";
      e.Layout.Bands[0].Columns["TotalLeadTime3"].Header.Caption = "Total Lead Time\n(6 < pcs <= 12)";
      e.Layout.Bands[0].Columns["TotalLeadTime4"].Header.Caption = "Total Lead Time\n(> 12pcs)";
      e.Layout.Bands[0].Columns["GroupCode"].Header.Caption = "Group Code";
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group Name";
      e.Layout.Bands[0].Columns["GroupName"].MaxWidth = 250;
      e.Layout.Bands[0].Columns["GroupName"].MinWidth = 250;


      e.Layout.Bands[1].Columns["LeadTime1"].Header.Caption = "Lead Time\n(<= 2pcs)";
      e.Layout.Bands[1].Columns["LeadTime2"].Header.Caption = "Lead Time\n(2 < pcs <= 6)";
      e.Layout.Bands[1].Columns["LeadTime3"].Header.Caption = "Lead Time\n(6 < pcs <= 12)";
      e.Layout.Bands[1].Columns["LeadTime4"].Header.Caption = "Lead Time\n(> 12pcs)";
      e.Layout.Bands[1].Columns["ProcessNameEN"].Header.Caption = "Process Name";
      e.Layout.Bands[1].Columns["WorkAreaCode"].Header.Caption = "Team";

    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void button1_Click_1(object sender, EventArgs e)
    {
      viewWIP_96_002 view = new viewWIP_96_002();
      WindowUtinity.ShowView(view, "NEW ROUTING DEFAULT DETAIL", true, ViewState.MainWindow);
    }
    private void ultData_DoubleClick(object sender, EventArgs e)
    {
      bool selected = false;
      try
      {
        selected = ultData.Selected.Rows[0].Selected;
      }
      catch
      {
        selected = false;
      }
      if (!selected)
      {
        return;
      }
      UltraGridRow row = (!ultData.Selected.Rows[0].HasParent()) ? ultData.Selected.Rows[0] : ultData.Selected.Rows[0].ParentRow;
      long Pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());

      if (RuleShow == 1)
      {
        dtNewSource = dtNewSource.Clone();
        DBParameter[] input = new DBParameter[1];
        input[0] = new DBParameter("@RoutingDefaultPid", DbType.Int64, Pid);
        DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNLoadProcessCodeByGroupPid", input);
        dtNewSource = dtSource;
        this.CloseTab();
      }
      else
      {
        viewWIP_96_002 view = new viewWIP_96_002();
        view.RoutingDefaultPid = Pid;
        WindowUtinity.ShowView(view, "ROUTING DEFAULT DETAIL", true, ViewState.MainWindow);
      }
    }

    private void btnClose_Click_1(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void ViewWIP_96_003_Load(object sender, EventArgs e)
    {
      if (RuleShow == 1)
      {
        button1.Visible = false;
        this.Search();
      }

      LoadDDTeam();
    }

    private void txtGroupCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        Search();
      }
    }

    private void txtGroupName_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        Search();
      }
    }

    #endregion 
  }
}
