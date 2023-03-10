/*
 * Authour : TRAN HUNG
 * Date : 29/12/2012
 * Description : Request Online
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using VBReport;
namespace DaiCo.Planning
{
  public partial class viewPLN_10_012 : MainUserControl
  {
    #region Init
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private IList listDeleteDetailPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    #endregion Intit

    #region Load
    private string department(int eid)
    {
      string commandText = string.Format(@"SELECT Department FROM VHRMEmployee  WHERE Pid = {0}", eid);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      return dt.Rows[0]["Department"].ToString();
    }
    private void LoadComboboxTeam()
    {
      string commandText = string.Empty;
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@EID", DbType.Int32, SharedObject.UserInfo.UserPid);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNLoadTeamByEmployee", inputParam);

      //commandText = string.Format(@"SELECT DISTINCT Team Code, Team+' / '+ VTeamName Name  FROM VHRMCompanyStructure WHERE  Department = '{0}'", department(SharedObject.UserInfo.UserPid));
      //DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCbmTeamCode.DataSource = dt;
      ultCbmTeamCode.DisplayMember = "Name";
      ultCbmTeamCode.ValueMember = "Code";
      ultCbmTeamCode.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultCbmTeamCode.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCbmTeamCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }
    private void LoadDropDownTeam()
    {
      //string commandText = string.Empty;
      //commandText = string.Format(@"SELECT DISTINCT Team Code, Team+' / '+ VTeamName Name  FROM VHRMCompanyStructure WHERE  Department = '{0}'", department(SharedObject.UserInfo.UserPid));
      //DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      DBParameter[] inputParam = new DBParameter[1];
      inputParam[0] = new DBParameter("@EID", DbType.Int32, SharedObject.UserInfo.UserPid);
      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNLoadTeamByEmployee", inputParam);

      ultDropdownTeam.DataSource = dt;
      ultDropdownTeam.DisplayMember = "Name";
      ultDropdownTeam.ValueMember = "Code";
      ultDropdownTeam.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultDropdownTeam.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultDropdownTeam.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }
    private void LoadComboboxWo()
    {
      string commandText = string.Format(@"SELECT DISTINCT WorkOrderPid WorkOrder 
                                           FROM TblPLNWorkOrderConfirmedDetails  ORDER BY WorkOrderPid DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCbmWo.DataSource = dt;
      ultCbmWo.DisplayMember = "WorkOrder";
      ultCbmWo.ValueMember = "WorkOrder";
      ultCbmWo.DisplayLayout.Bands[0].Columns["WorkOrder"].Width = 100;
      ultCbmWo.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }
    private void LoadComboboxProcess()
    {
      string commandText = string.Empty;

      commandText = string.Format(@"SELECT DISTINCT WorkAreaPid, WorkAreaNameVn  FROM VWIPWoItemProcessScan ORDER BY WorkAreaPid, WorkAreaNameVn");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        ultcFillProcess.DataSource = dt;
        ultcFillProcess.DisplayMember = "WorkAreaNameVn";
        ultcFillProcess.ValueMember = "WorkAreaPid";
        ultcFillProcess.DisplayLayout.Bands[0].ColHeadersVisible = true;
        ultcFillProcess.DisplayLayout.Bands[0].Columns["WorkAreaPid"].Hidden = true;
      }
    }
    private void LoadDropDownWo()
    {
      string commandText = string.Format(@"SELECT DISTINCT WorkOrderPid WorkOrder 
                                           FROM TblPLNWorkOrderConfirmedDetails  ORDER BY WorkOrderPid DESC");
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultdropdownWo.DataSource = dt;
      ultdropdownWo.DisplayMember = "WorkOrder";
      ultdropdownWo.ValueMember = "WorkOrder";
      ultdropdownWo.DisplayLayout.Bands[0].Columns["WorkOrder"].Width = 100;
      ultdropdownWo.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }
    //private void LoadComboboxItemCode()
    //{
    //  string commandText = string.Empty;
    //  if (ultCbmWo.Value == null)
    //  {
    //    commandText = string.Format(@"SELECT ItemCode FROM TblPLNWorkOrderConfirmedDetails");
    //  }
    //  else
    //  {
    //    commandText = string.Format(@"SELECT ItemCode FROM TblPLNWorkOrderConfirmedDetails WHERE WorkOrderPid = {0}", ultCbmWo.Value);
    //  }

    //  DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
    //  ultcbmItemCode.DataSource = dt;
    //  ultcbmItemCode.DisplayMember = "ItemCode";
    //  ultcbmItemCode.ValueMember = "ItemCode";
    //  ultcbmItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
    //}
    private void LoadComboboxCarcass()
    {
      string commandText = string.Empty;
      if (department(SharedObject.UserInfo.UserPid) == "COM1")
      {
        if (ultCbmWo.Value == null)
        {
          commandText = string.Format(@"SELECT DISTINCT CAR.CarcassCode  
                                      FROM TblPLNWOCarcassInfomation  CAR
                                      INNER JOIN TblPLNWorkOrderConfirmedDetails WOCDT ON CAR.CarcassCode = WOCDT.CarcassCode AND CAR.Wo = WOCDT.WorkOrderPid 
                                      INNER JOIN TblPLNWOInfoDetailGeneral WO ON WOCDT.ItemCode = WO.ItemCode
                                      ORDER BY CAR.CarcassCode ASC ");
        }
        else
        {
          commandText = string.Format(@"SELECT DISTINCT CAR.CarcassCode  
                                      FROM TblPLNWOCarcassInfomation  CAR
                                      INNER JOIN TblPLNWorkOrderConfirmedDetails WOCDT ON CAR.CarcassCode = WOCDT.CarcassCode AND CAR.Wo = WOCDT.WorkOrderPid 
                                      INNER JOIN TblPLNWOInfoDetailGeneral WO ON WOCDT.ItemCode = WO.ItemCode
                                      WHERE WOCDT.WorkOrderPid = {0}
                                      ORDER BY CAR.CarcassCode ASC", ultCbmWo.Value);
        }
      }
      else
      {
        if (ultCbmWo.Value == null)
        {
          commandText = string.Format(@"SELECT ItemCode FROM TblPLNWorkOrderConfirmedDetails");
        }
        else
        {
          commandText = string.Format(@"SELECT ItemCode FROM TblPLNWorkOrderConfirmedDetails WHERE WorkOrderPid = {0}", ultCbmWo.Value);
        }
      }

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCbmCarcassCode.DataSource = dt;
      if (department(SharedObject.UserInfo.UserPid) == "COM1")
      {
        ultCbmCarcassCode.DisplayMember = "CarcassCode";
        ultCbmCarcassCode.ValueMember = "CarcassCode";
      }
      else
      {
        ultCbmCarcassCode.DisplayMember = "ItemCode";
        ultCbmCarcassCode.ValueMember = "ItemCode";
      }
      ultCbmCarcassCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }
    private void LoadComboboxComponent()
    {
      string commandText = string.Empty;
      if (ultCbmCarcassCode.Value == null)
      {
        commandText = string.Format(@"SELECT ComponentCode  FROM TblPLNWOCarcassInfomation");
      }
      else
      {
        commandText = string.Format(@"SELECT ComponentCode  FROM TblPLNWOCarcassInfomation WHERE CarcassCode = '{0}'", ultCbmCarcassCode.Value.ToString());
      }

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCbmComponent.DataSource = dt;
      ultCbmComponent.DisplayMember = "ComponentCode";
      ultCbmComponent.ValueMember = "ComponentCode";
      ultCbmComponent.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }
    private UltraDropDown DropdownItemCode(long Wo, UltraDropDown ultDropdownItemCode)
    {
      if (ultDropdownItemCode == null)
      {
        ultDropdownItemCode = new UltraDropDown();
        this.Controls.Add(ultDropdownItemCode);
      }
      string commandText = string.Empty;
      if (Wo == long.MinValue)
      {
        commandText = string.Format(@"SELECT DISTINCT ItemCode FROM TblPLNWorkOrderConfirmedDetails ");
      }
      else
      {
        commandText = string.Format(@"SELECT ItemCode FROM TblPLNWorkOrderConfirmedDetails WHERE WorkOrderPid = {0}", Wo);
      }

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        ultDropdownItemCode.DataSource = dt;
        ultDropdownItemCode.DisplayMember = "ItemCode";
        ultDropdownItemCode.ValueMember = "ItemCode";
        ultDropdownItemCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
      }
      return ultDropdownItemCode;
    }

    private UltraDropDown DropdownRevision(long Wo, string strItemCode, UltraDropDown ultDropdownRevision)
    {
      if (ultDropdownRevision == null)
      {
        ultDropdownRevision = new UltraDropDown();
        this.Controls.Add(ultDropdownRevision);
      }
      string commandText = string.Empty;
      if (Wo == long.MinValue)
      {
        commandText = string.Format(@"SELECT DISTINCT Revision FROM TblPLNWorkOrderConfirmedDetails ORDER BY Revision");
      }
      else
      {
        commandText = string.Format(@"SELECT DISTINCT Revision FROM TblPLNWorkOrderConfirmedDetails WHERE WorkOrderPid = {0} AND ItemCode = '{1}'  ORDER BY Revision", Wo, strItemCode);
      }

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        ultDropdownRevision.DataSource = dt;
        ultDropdownRevision.DisplayMember = "Revision";
        ultDropdownRevision.ValueMember = "Revision";
        ultDropdownRevision.DisplayLayout.Bands[0].ColHeadersVisible = true;
      }
      //try
      //{
      //  ultDropdownRevision.SelectedRow.Cells["Revision"].Value = 1;
      //}
      //catch
      //{ }
      return ultDropdownRevision;
    }

    private UltraDropDown DropdownProcessCar(long Wo, string strItemCode, int revision, UltraDropDown ultDropdownProcess)
    {
      if (ultDropdownProcess == null)
      {
        ultDropdownProcess = new UltraDropDown();
        this.Controls.Add(ultDropdownProcess);
      }
      //this.Controls.Add(ultDropdownProcess);

      string commandText = string.Empty;

      commandText = string.Format(@"SELECT DISTINCT WorkAreaPid, WorkAreaNameVn  FROM VWIPWoItemProcessScan WHERE WorkOrder = {0} AND ItemCode = '{1}' AND Revision = {2}  ORDER BY WorkAreaPid, WorkAreaNameVn", Wo, strItemCode, revision);
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      //DataRow dr = dt.NewRow();
      //dr["WorkAreaPid"] = 0;
      //dr["WorkAreaNameVn"] = "";
      //dt.Rows.InsertAt(dr, 0);

      if (dt != null)
      {
        ultDropdownProcess.DataSource = dt;
        ultDropdownProcess.DisplayMember = "WorkAreaNameVn";
        ultDropdownProcess.ValueMember = "WorkAreaPid";
        ultDropdownProcess.DisplayLayout.Bands[0].ColHeadersVisible = true;
        ultDropdownProcess.DisplayLayout.Bands[0].Columns["WorkAreaPid"].Hidden = true;
      }
      return ultDropdownProcess;
    }
    private UltraDropDown DropdownCarcass(long Wo, UltraDropDown ultDropdownCarcass)
    {
      if (ultDropdownCarcass == null)
      {
        ultDropdownCarcass = new UltraDropDown();
        this.Controls.Add(ultDropdownCarcass);
      }
      string commandText = string.Empty;
      //if (ultdropdownWo.SelectedRow.Cells["Workorder"].Value == null)
      if (Wo == long.MinValue)
      {
        commandText = string.Format(@"SELECT DISTINCT CAR.CarcassCode  
                                      FROM TblPLNWOCarcassInfomation  CAR
                                      INNER JOIN TblPLNWorkOrderConfirmedDetails WOCDT ON CAR.CarcassCode = WOCDT.CarcassCode AND CAR.Wo = WOCDT.WorkOrderPid 
                                      INNER JOIN TblPLNWOInfoDetailGeneral WO ON WOCDT.ItemCode = WO.ItemCode
                                      ORDER BY CAR.CarcassCode ASC ");
      }
      else
      {
        commandText = string.Format(@"SELECT DISTINCT CAR.CarcassCode  
                                      FROM TblPLNWOCarcassInfomation  CAR
                                      INNER JOIN TblPLNWorkOrderConfirmedDetails WOCDT ON CAR.CarcassCode = WOCDT.CarcassCode AND CAR.Wo = WOCDT.WorkOrderPid 
                                      INNER JOIN TblPLNWOInfoDetailGeneral WO ON WOCDT.ItemCode = WO.ItemCode
                                      WHERE WOCDT.WorkOrderPid = {0}
                                      ORDER BY CAR.CarcassCode ASC", Wo);
      }

      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        ultDropdownCarcass.DataSource = dt;
        ultDropdownCarcass.DisplayMember = "CarcassCode";
        ultDropdownCarcass.ValueMember = "CarcassCode";
        ultDropdownCarcass.DisplayLayout.Bands[0].ColHeadersVisible = true;
      }
      return ultDropdownCarcass;
    }
    private UltraDropDown DropdownComponent(long wo, string Carcass, UltraDropDown ultDropdownComponent)
    {
      if (ultDropdownComponent == null)
      {
        ultDropdownComponent = new UltraDropDown();
        this.Controls.Add(ultDropdownComponent);
      }
      string commandText = string.Empty;
      //if (ultdropdownWo.SelectedRow.Cells["Workorder"].Value == null)
      if (wo != long.MinValue)
      {
        if (Carcass == string.Empty)
        {
          commandText = string.Format(@"SELECT ComponentCode  FROM TblPLNWOCarcassInfomation WHERE Wo = {0}", wo);
        }
        else
        {
          commandText = string.Format(@"SELECT ComponentCode  FROM TblPLNWOCarcassInfomation WHERE Wo ={0} AND CarcassCode = '{1}'", wo, Carcass);
        }
      }
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        ultDropdownComponent.DataSource = dt;
        ultDropdownComponent.DisplayMember = "ComponentCode";
        ultDropdownComponent.ValueMember = "ComponentCode";
        ultDropdownComponent.DisplayLayout.Bands[0].ColHeadersVisible = true;
      }
      return ultDropdownComponent;

    }
    private bool CheckIsValid(out string message)
    {
      message = string.Empty;
      bool result = true;
      DataTable dt = (DataTable)ultData.DataSource;
      if (DBConvert.ParseDouble(lblManhour.Text) < DBConvert.ParseDouble(lblTotal.Text))
      {
        message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Manhour");
        result = false;
      }

      for (int i = 0; i < dt.Rows.Count; i++)
      {
        if (dt.Rows[i].RowState == DataRowState.Modified || dt.Rows[i].RowState == DataRowState.Added)
        {
          if (dt.Rows[i]["Workorder"].ToString() == string.Empty || DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString()) <= 0 || DBConvert.ParseDouble(dt.Rows[i]["Manhour"].ToString()) <= 0)
          {
            message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0009"));
            //ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
            result = false;
          }
        }
      }
      return result;
    }

    #endregion Load

    #region Event

    public viewPLN_10_012()
    {
      InitializeComponent();
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void viewWCB_01_005_Load(object sender, EventArgs e)
    {
      //if (department(SharedObject.UserInfo.UserPid) == "COM1")
      //{
      //  this.label7.Visible = false;
      //  this.ultcbmItemCode.Visible = false;
      //}
      LoadComboboxTeam();
      LoadDropDownTeam();
      LoadComboboxWo();
      LoadComboboxProcess();
      LoadDropDownWo();
      //LoadComboboxItemCode();
      LoadComboboxCarcass();
      LoadComboboxComponent();
      this.ultdtFromDate.Value = DateTime.Today.AddDays(-7);
      this.ultdtToDate.Value = DateTime.Today;
      if (department(SharedObject.UserInfo.UserPid) == "COM2")
      {
        label2.Text = "Product Code";
      }
      if (department(SharedObject.UserInfo.UserPid) == "COM1")
      {
        label2.Text = "Carcass Code";
      }
      if (department(SharedObject.UserInfo.UserPid) == "CAR")
      {
        label2.Text = "Item Code";
      }
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.Bands[0].Summaries.Clear();
      e.Layout.Override.AllowRowFiltering = DefaultableBoolean.True;
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["Team"].ValueList = ultDropdownTeam;
      e.Layout.Bands[0].Columns["Team"].Hidden = true;
      e.Layout.Bands[0].Columns["Component"].Hidden = true;
      e.Layout.Bands[0].Columns["Date"].Hidden = true;
      DataTable dtNew = (DataTable)ultData.DataSource;
      for (int i = 0; i < dtNew.Columns.Count; i++)
      {
        if (dtNew.Columns[i].DataType == typeof(Int64) || dtNew.Columns[i].DataType == typeof(Int32) || dtNew.Columns[i].DataType == typeof(float) || dtNew.Columns[i].DataType == typeof(Double))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dtNew.Columns[i].DataType == typeof(DateTime))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }
      e.Layout.Bands[0].Columns["Team"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["Team"].MinWidth = 200;
      e.Layout.Bands[0].Columns["WorkOrder"].ValueList = ultdropdownWo;
      e.Layout.Bands[0].Columns["WorkOrder"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["WorkOrder"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Carcass"].ValueList = ultDropdownCarcass;
      e.Layout.Bands[0].Columns["Revision"].ValueList = ultDropdownRevision;
      e.Layout.Bands[0].Columns["ProcessPid"].ValueList = ultDropdownProcessCar;
      e.Layout.Bands[0].Columns["Carcass"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Carcass"].MinWidth = 100;
      e.Layout.Bands[0].Columns["Component"].ValueList = ultDropdownComponent;
      e.Layout.Bands[0].Columns["Component"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["Component"].MinWidth = 100;
      e.Layout.Bands[0].Columns["WorkOrder"].Header.Caption = "Work Order";
      e.Layout.Bands[0].Columns["WorkOrder"].Header.Caption = "Work Order";
      e.Layout.Bands[0].Columns["Date"].Width = 70;
      e.Layout.Bands[0].Columns["Qty"].Width = 70;
      e.Layout.Bands[0].Columns["Manhour"].Width = 70;
      e.Layout.Bands[0].Columns["Carcass"].Header.Caption = "Item Code";
      e.Layout.Bands[0].Columns["ProcessPid"].Header.Caption = "WCP";
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[0].Columns["WCP"].Hidden = true;
      e.Layout.Bands[0].Columns["Minutes"].Hidden = true;
      e.Layout.Bands[0].Columns["Processing"].Hidden = true;
      e.Layout.Bands[0].Columns["Minutes"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Processing"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[0].Columns["Manhour"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      int index = e.Cell.Row.Index;
      string columnName = e.Cell.Column.ToString().ToLower();
      string value = e.Cell.Value.ToString();
      switch (columnName)
      {
        case "workorder":
          try
          {
            if (DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString()) != long.MinValue)
            {
              if (department(SharedObject.UserInfo.UserPid) == "CAR")
              {
                this.DropdownCarcass(DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString()), ultDropdownCarcass);
              }
              else
              {
                this.DropdownItemCode(DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString()), ultDropdownItemCode);
              }
            }
          }
          catch
          {
          }
          break;
        case "carcass":
          try
          {
            if (e.Cell.Row.Cells["Carcass"].Value.ToString() != string.Empty)
            {
              this.DropdownComponent(DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString()), e.Cell.Row.Cells["Carcass"].Value.ToString(), ultDropdownComponent);
              this.DropdownRevision(DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString()), e.Cell.Row.Cells["Carcass"].Value.ToString(), ultDropdownRevision);
              try
              {
                e.Cell.Row.Cells["Revision"].Value = ultDropdownRevision.Rows[0].Cells["Revision"].Value;
              }
              catch
              { }
            }
          }
          catch
          {
          }
          break;
        //case "revision":
        //  try
        //  {
        //    if (DBConvert.ParseInt(e.Cell.Row.Cells["revision"].Value.ToString()) >= 0)
        //    {
        //      this.DropdownProcessCar(DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString()), e.Cell.Row.Cells["Carcass"].Value.ToString(), DBConvert.ParseInt(e.Cell.Row.Cells["revision"].Value.ToString()), ultDropdownProcessCar);
        //    }
        //  }
        //  catch
        //  {
        //  }
        //  break;
        default:
          break;
      }
      if (columnName == "manhour")
      {
        double totalMinutes = 0;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (DBConvert.ParseDouble(ultData.Rows[i].Cells["Manhour"].Value.ToString()) > 0)
          {
            totalMinutes += DBConvert.ParseDouble(ultData.Rows[i].Cells["Manhour"].Value.ToString());
          }
        }
        lblTotal.Text = totalMinutes.ToString();
      }
      try
      {
        if (DBConvert.ParseLong(ultcFillProcess.Value.ToString()) > 0 && columnName != "processpid")
        {
          DataTable dt = (DataTable)((UltraDropDown)e.Cell.Row.Cells["ProcessPid"].ValueList).DataSource;
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            if (DBConvert.ParseLong(dt.Rows[i]["WorkAreaPid"].ToString()) == DBConvert.ParseLong(ultcFillProcess.Value.ToString()))
            {
              e.Cell.Row.Cells["ProcessPid"].Value = ultcFillProcess.Value;
              return;
            }
          }
          e.Cell.Row.Cells["ProcessPid"].Value = DBNull.Value;
        }
      }
      catch
      {

      }
    }
    UltraGridCell cellDropDown = null;
    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      switch (colName)
      {
        case "carcass":
          {
            if (department(SharedObject.UserInfo.UserPid) == "COM1")
            {
              long wo = DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString());
              UltraDropDown ultDropdownCarcass = (UltraDropDown)e.Cell.Row.Cells["Carcass"].ValueList;
              e.Cell.Row.Cells["Carcass"].ValueList = this.DropdownCarcass(wo, ultDropdownCarcass);
            }
            else
            {
              long wo = DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString());
              UltraDropDown ultDropdownItemCode = (UltraDropDown)e.Cell.Row.Cells["Carcass"].ValueList;
              e.Cell.Row.Cells["Carcass"].ValueList = this.DropdownItemCode(wo, ultDropdownItemCode);
            }
            break;
          }
        case "component":
          {
            long wo = DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString());
            string carcass = e.Cell.Row.Cells["Carcass"].Value.ToString();
            UltraDropDown ultDropdownComponent = (UltraDropDown)e.Cell.Row.Cells["Component"].ValueList;
            e.Cell.Row.Cells["Component"].ValueList = this.DropdownComponent(wo, carcass, ultDropdownComponent);
            break;
          }
        case "revision":
          {
            long wo = DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString());
            string carcass = e.Cell.Row.Cells["Carcass"].Value.ToString();
            UltraDropDown ultDropdownRevision = (UltraDropDown)e.Cell.Row.Cells["Revision"].ValueList;
            //e.Cell.Row.Cells["Revision"].ValueList = this.DropdownRevision(wo, carcass, DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString()), ultDropdownRevision);
            e.Cell.Row.Cells["Revision"].ValueList = this.DropdownRevision(wo, carcass, ultDropdownRevision);
            break;
          }
        case "processpid":
          {
            long wo = DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString());
            string carcass = e.Cell.Row.Cells["Carcass"].Value.ToString();
            UltraDropDown ultDropdownProcessPid = (UltraDropDown)e.Cell.Row.Cells["ProcessPid"].ValueList;
            e.Cell.Row.Cells["ProcessPid"].ValueList = this.DropdownProcessCar(wo, carcass, DBConvert.ParseInt(e.Cell.Row.Cells["Revision"].Value.ToString()), ultDropdownProcessPid);
            try
            {
              if (DBConvert.ParseLong(ultcFillProcess.Value.ToString()) > 0)
              {
                DataTable dt = (DataTable)((UltraDropDown)e.Cell.Row.Cells["ProcessPid"].ValueList).DataSource;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                  if (DBConvert.ParseLong(dt.Rows[i]["WorkAreaPid"].ToString()) == DBConvert.ParseLong(ultcFillProcess.Value.ToString()))
                  {
                    e.Cell.Row.Cells["ProcessPid"].Value = ultcFillProcess.Value;
                    goto final;
                  }
                }
                e.Cell.Row.Cells["ProcessPid"].Value = DBNull.Value;
              final:;
              }
            }
            catch
            {

            }
            break;
          }

      }
      if (colName == "workorder" || colName == "carcass" || colName == "revision" || colName == "processpid")
      {
        cellDropDown = e.Cell.Row.Cells[colName];
      }
      else
      {
        cellDropDown = null;
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      this.ultCbmTeamCode.Text = string.Empty;
      this.ultCbmCarcassCode.Text = string.Empty;
      this.ultCbmComponent.Text = string.Empty;
      this.ultCbmWo.Text = string.Empty;
      this.ultdtFromDate.Value = null;
      this.ultdtToDate.Value = null;
    }
    int Kind = 0;
    string strTeamCode = "";
    DateTime dtdWDate = DateTime.MinValue;
    private void btnSearch_Click(object sender, EventArgs e)
    {
      //Check Team and WDate
      try
      {
        if (ultCbmTeamCode.Value.ToString().Trim().Length == 0 || DBConvert.ParseDateTime(ultdtFromDate.Value.ToString(), formatConvert) <= DateTime.MinValue)
        {
          Shared.Utility.WindowUtinity.ShowMessageWarningFromText("Team and Working Date must be selected");
          return;
        }
      }
      catch
      {
        Shared.Utility.WindowUtinity.ShowMessageWarningFromText("Team and Working Date must be selected");
        return;
      }
      DBParameter[] inputManhour = new DBParameter[2];
      inputManhour[0] = new DBParameter("@Team", DbType.AnsiString, 10, ultCbmTeamCode.Value.ToString());
      inputManhour[1] = new DBParameter("@WDate", DbType.DateTime, DBConvert.ParseDateTime(ultdtFromDate.Value.ToString(), formatConvert));

      DataTable dtManhour = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedureDataTable("spPLNManhourAllocationForCar_Select", inputManhour);
      strTeamCode = ultCbmTeamCode.Value.ToString();
      dtdWDate = DBConvert.ParseDateTime(ultdtFromDate.Value.ToString(), formatConvert);
      if (chkOT.Checked)
      {
        this.Kind = 1;
        lbllManhour.Text = "Overtime";
      }
      else
      {
        this.Kind = 0;
        lbllManhour.Text = "Manhour";
      }
      if (dtManhour == null || dtManhour.Rows.Count == 0)
      {
        return;
      }
      else
      {
        lblTeam.Text = dtManhour.Rows[0]["TeamName"].ToString();
        lblWDate.Text = dtManhour.Rows[0]["WDate"].ToString();
        if (Kind == 0)
        {
          lblManhour.Text = dtManhour.Rows[0]["Manhour"].ToString();
        }
        else
        {
          lblManhour.Text = dtManhour.Rows[0]["Overtime"].ToString();
        }
      }

      DBParameter[] input = new DBParameter[9];
      if (ultCbmTeamCode.Value != null)
      {
        input[0] = new DBParameter("@TeamCode", DbType.AnsiString, 32, ultCbmTeamCode.Value.ToString());
      }
      if (ultCbmWo.Value != null)
      {
        input[1] = new DBParameter("@Wo", DbType.Int64, DBConvert.ParseLong(ultCbmWo.Value.ToString()));
      }
      if (ultCbmCarcassCode.Text != string.Empty)
      {
        input[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 32, ultCbmCarcassCode.Text.Trim());
      }
      if (ultCbmComponent.Text != string.Empty)
      {
        input[3] = new DBParameter("@Component", DbType.AnsiString, 32, ultCbmComponent.Text.Trim());
      }
      if (ultdtFromDate.Value != null)
      {
        input[4] = new DBParameter("@FromDate", DbType.DateTime, DBConvert.ParseDateTime(ultdtFromDate.Value.ToString(), formatConvert));
      }
      if (ultdtToDate.Value != null)
      {
        input[5] = new DBParameter("@ToDate", DbType.DateTime, DBConvert.ParseDateTime(ultdtToDate.Value.ToString(), formatConvert));
      }
      if (department(SharedObject.UserInfo.UserPid) != "IT1")
      {
        input[6] = new DBParameter("@Department", DbType.AnsiString, 32, department(SharedObject.UserInfo.UserPid));
      }
      if (chkOT.Checked)
      {
        input[7] = new DBParameter("@Overtime", DbType.Int32, 1);
      }
      input[8] = new DBParameter("@EmployeePid", DbType.Int32, SharedObject.UserInfo.UserPid);

      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNManhourAllocation_Select", input);
      ultData.DataSource = dsSource.Tables[0];
      double TotalManhour = 0;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        //ultData.Rows[i].Activation = Activation.ActivateOnly;
        //ultData.Rows[i].Cells["ProcessPid"].ValueList = this.DropdownProcessCar(DBConvert.ParseLong(ultData.Rows[i].Cells["WorkOrder"].Value.ToString()), ultData.Rows[i].Cells["Carcass"].Value.ToString(), DBConvert.ParseInt(ultData.Rows[i].Cells["Revision"].Value.ToString()), ultDropdownProcessCar);
        if (DBConvert.ParseDouble(ultData.Rows[i].Cells["Manhour"].Value.ToString()) > 0)
        {
          TotalManhour += DBConvert.ParseDouble(ultData.Rows[i].Cells["Manhour"].Value.ToString());
        }
      }
      lblTotal.Text = TotalManhour.ToString();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }
      string Out = string.Empty;
      DataTable dt = (DataTable)ultData.DataSource;

      if (dt.Rows.Count > 0)
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          string storeName = string.Empty;
          DBParameter[] inputParam = new DBParameter[17];
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          if (dt.Rows[i].RowState == DataRowState.Modified)
          {
            storeName = "spPLNManhourAllocation_Update";
            inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()));
            inputParam[13] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          if (dt.Rows[i].RowState == DataRowState.Added)
          {
            storeName = "spPLNManhourAllocation_Insert";
            inputParam[12] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
          }
          if (storeName.Length > 0)
          {
            if (strTeamCode != string.Empty)
            {
              inputParam[1] = new DBParameter("@TeamCode", DbType.AnsiString, 32, strTeamCode);
            }
            if (dtdWDate != DateTime.MinValue)
            {
              inputParam[2] = new DBParameter("@WDate", DbType.DateTime, dtdWDate);
            }
            if (DBConvert.ParseLong(dt.Rows[i]["Workorder"].ToString()) != long.MinValue)
            {
              inputParam[3] = new DBParameter("@Wo", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Workorder"].ToString()));
            }
            if (dt.Rows[i]["Carcass"].ToString() != string.Empty)
            {
              inputParam[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 32, dt.Rows[i]["Carcass"].ToString().Trim());
            }
            if (dt.Rows[i]["Component"].ToString() != string.Empty)
            {
              inputParam[5] = new DBParameter("@Component", DbType.AnsiString, 32, dt.Rows[i]["Component"].ToString().Trim());
            }
            if (dt.Rows[i]["WCP"].ToString() != string.Empty)
            {
              inputParam[6] = new DBParameter("@WCP", DbType.String, 500, dt.Rows[i]["WCP"].ToString().Trim());
            }
            inputParam[7] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString()));

            if (DBConvert.ParseDouble(dt.Rows[i]["Minutes"].ToString()) != Double.MinValue)
            {
              inputParam[8] = new DBParameter("@Minutes", DbType.Double, DBConvert.ParseDouble(dt.Rows[i]["Minutes"].ToString()));
            }
            if (DBConvert.ParseDouble(dt.Rows[i]["Processing"].ToString()) != Double.MinValue)
            {
              inputParam[9] = new DBParameter("@Processing", DbType.Double, DBConvert.ParseDouble(dt.Rows[i]["Processing"].ToString()));
            }
            if (DBConvert.ParseDouble(dt.Rows[i]["Manhour"].ToString()) != Double.MinValue)
            {
              inputParam[10] = new DBParameter("@Manhour", DbType.Double, DBConvert.ParseDouble(dt.Rows[i]["Manhour"].ToString()));
            }
            if (dt.Rows[i]["Remark"].ToString() != string.Empty)
            {
              inputParam[11] = new DBParameter("@Remark", DbType.String, 500, dt.Rows[i]["Remark"].ToString());
            }
            if (DBConvert.ParseInt(dt.Rows[i]["Revision"].ToString()) >= 0)
            {
              inputParam[14] = new DBParameter("@Revision", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Revision"].ToString()));
            }
            if (DBConvert.ParseLong(dt.Rows[i]["ProcessPid"].ToString()) > 0)
            {
              inputParam[15] = new DBParameter("@ProcessPid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["ProcessPid"].ToString()));
            }
            if (this.Kind > 0)
            {
              inputParam[16] = new DBParameter("@Overtime", DbType.Int32, 1);
            }
            Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
            long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
            Out += "," + outValue.ToString();
            if (outValue == -1 || outValue == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
              ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
            }
          }
        }
      }
      if (this.listDeleteDetailPid.Count > 0)
      {
        foreach (long pid in this.listDeleteDetailPid)
        {
          string storeNamedelete = "spPLNManhourAllocation_Delete";
          DBParameter[] inputParamdelete = new DBParameter[1];
          inputParamdelete[0] = new DBParameter("@Pid", DbType.Int64, pid);
          Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeNamedelete, inputParamdelete);
        }
      }
      if (Out.IndexOf("-1") == -1)
      {
        Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0004");
        for (int a = 0; a < ultData.Rows.Count; a++)
        {
          ultData.Rows[a].CellAppearance.BackColor = Color.White;
        }
        this.btnSearch_Click(sender, e);
      }
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      this.listDeletingPid = new ArrayList();
      foreach (UltraGridRow row in e.Rows)
      {
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletingPid.Add(pid);
        }
      }
    }

    private void ultData_AfterRowsDeleted(object sender, EventArgs e)
    {
      foreach (long pid in this.listDeletingPid)
      {
        this.listDeleteDetailPid.Add(pid);
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      if (string.Compare(colName, "Minutes", true) == 0)
      {
        double values = DBConvert.ParseDouble(e.Cell.Text.Trim());
        if (values < 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0110", new string[] { "Minutes" });
          e.Cancel = true;
        }
      }
      if (string.Compare(colName, "Processing", true) == 0)
      {
        double values = DBConvert.ParseDouble(e.Cell.Text.Trim());
        if (values <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0110", new string[] { "Processing" });
          e.Cancel = true;
        }
      }
      if (string.Compare(colName, "Manhour", true) == 0)
      {
        double values = DBConvert.ParseDouble(e.Cell.Text.Trim());
        if (values <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0110", new string[] { "Manhour" });
          e.Cancel = true;
        }
      }
      if (string.Compare(colName, "Qty", true) == 0)
      {
        int values = DBConvert.ParseInt(e.Cell.Text.Trim());
        if (values <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0110", new string[] { "Qty" });
          e.Cancel = true;
        }
      }
      if (string.Compare(colName, "Workorder", true) == 0)
      {
        int values = DBConvert.ParseInt(e.Cell.Text.Trim());
        if (values < 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Workorder" });
          e.Cancel = true;
        }
      }
    }

    private void ultCbmWo_ValueChanged(object sender, EventArgs e)
    {
      LoadComboboxCarcass();
      //LoadComboboxItemCode();
    }

    private void ultCbmCarcassCode_ValueChanged(object sender, EventArgs e)
    {
      if (ultCbmCarcassCode.Value != null)
      {
        LoadComboboxComponent();
      }
    }

    private void ultCbmTeamCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultCbmWo_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultCbmCarcassCode_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultCbmComponent_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultdtFromDate_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void ultdtToDate_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
        this.btnSearch_Click(sender, e);
    }

    private void btnBrown_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtFilePatch.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      if (department(SharedObject.UserInfo.UserPid) == "COM1")
      {
        string templateName = "TemplateImportManHour_COM1";
        string sheetName = "ImportManHour(COM1)";
        string outFileName = "TemplateImportManHour_COM1";
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string pathOutputFile = string.Format(@"{0}\Report", startupPath);
        string pathTemplate = startupPath + @"\ExcelTemplate";
        XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
        oXlsReport.Out.File(outFileName);
        Process.Start(outFileName);
      }
      if (department(SharedObject.UserInfo.UserPid) == "COM2")
      {
        string templateName = "TemplateImportManHour_COM2";
        string sheetName = "ImportManHour(COM2)";
        string outFileName = "TemplateImportManHour_COM2";
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string pathOutputFile = string.Format(@"{0}\Report", startupPath);
        string pathTemplate = startupPath + @"\ExcelTemplate";
        XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
        oXlsReport.Out.File(outFileName);
        Process.Start(outFileName);
      }
      if (department(SharedObject.UserInfo.UserPid) == "CAR")
      {
        string templateName = "TemplateImportManHour_CAR";
        string sheetName = "ImportManHour(CAR)";
        string outFileName = "TemplateImportManHour_CAR";
        string startupPath = System.Windows.Forms.Application.StartupPath;
        string pathOutputFile = string.Format(@"{0}\Report", startupPath);
        string pathTemplate = startupPath + @"\ExcelTemplate";
        XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);
        oXlsReport.Out.File(outFileName);
        Process.Start(outFileName);
      }
    }

    private void btnShowData_Click(object sender, EventArgs e)
    {
      if (txtFilePatch.Text.Length > 0)
      {
        viewPLN_10_014 uc = new viewPLN_10_014();
        string commandText = string.Format(@"SELECT Pid ,EmpName, Department FROM VHRMEmployee  WHERE Pid = {0}", Shared.Utility.SharedObject.UserInfo.UserPid);
        DataTable dtUser = DataBaseAccess.SearchCommandTextDataTable(commandText);
        uc.label1.Text = dtUser.Rows[0]["EmpName"].ToString() + " - " + dtUser.Rows[0]["Department"].ToString();
        DataTable dtSource = new DataTable();
        DataTable data = new DataTable();
        data.Columns.Add("Team", typeof(string));
        data.Columns.Add("Date", typeof(DateTime));
        data.Columns.Add("WorkOrder", typeof(Int64));
        data.Columns.Add("CarcassCode", typeof(string));
        data.Columns.Add("Component", typeof(string));
        data.Columns.Add("Qty", typeof(Int32));
        data.Columns.Add("WCP", typeof(string));
        data.Columns.Add("Minutes", typeof(Double));
        data.Columns.Add("Processing", typeof(Double));
        data.Columns.Add("Manhour", typeof(Double));
        data.Columns.Add("Remark", typeof(string));
        if (department(SharedObject.UserInfo.UserPid) == "COM1")
        {
          if (this.txtFilePatch.Text.Trim().Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0046");
            return;
          }
          try
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSetVersion2(txtFilePatch.Text.Trim(), "SELECT * FROM [ImportManHour(COM1) (1)$B7:L500]").Tables[0];
          }
          catch { }
          if (dtSource == null)
          {
            return;
          }
          foreach (DataRow row in dtSource.Rows)
          {
            if (row["Team"].ToString().Length == 0
              || row["WorkOrder"].ToString().Length == 0
              || row["Date"].ToString().Length == 0
              || row["Qty"].ToString().Length == 0
              || row["Manhour"].ToString().Length == 0)
            {
              continue;
            }
            DataRow newRow = data.NewRow();
            newRow["Team"] = row["Team"];
            newRow["Date"] = DBConvert.ParseDateTime(row["Date"].ToString(), formatConvert);
            newRow["WorkOrder"] = row["WorkOrder"];
            newRow["CarcassCode"] = row["CarcassCode"];
            newRow["Component"] = row["Component"];
            newRow["Qty"] = row["Qty"];
            newRow["WCP"] = row["WCP"];
            newRow["Minutes"] = row["Minutes"];
            newRow["Processing"] = row["Processing"];
            newRow["Manhour"] = row["Manhour"];
            newRow["Remark"] = row["Remark"];
            data.Rows.Add(newRow);
          }
          uc.dt = data;
        }
        if (department(SharedObject.UserInfo.UserPid) == "COM2")
        {
          if (this.txtFilePatch.Text.Trim().Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0046");
            return;
          }
          try
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSetVersion2(txtFilePatch.Text.Trim(), "SELECT * FROM [ImportManHour(COM2) (1)$B7:I500]").Tables[0];
          }
          catch { }

          if (dtSource == null)
          {
            return;
          }
          foreach (DataRow row in dtSource.Rows)
          {
            if (row["Team"].ToString().Length == 0
              || row["WorkOrder"].ToString().Length == 0
              || row["Date"].ToString().Length == 0
              || row["Qty"].ToString().Length == 0
              || row["Manhour"].ToString().Length == 0)
            {
              continue;
            }
            DataRow newRow = data.NewRow();
            newRow["Team"] = row["Team"];
            newRow["Date"] = DBConvert.ParseDateTime(row["Date"].ToString(), formatConvert);
            newRow["WorkOrder"] = row["WorkOrder"];
            newRow["CarcassCode"] = row["ProductCode"];
            newRow["Component"] = row["Component"];
            newRow["Qty"] = row["Qty"];
            newRow["Manhour"] = row["Manhour"];
            newRow["Remark"] = row["Remark"];
            data.Rows.Add(newRow);
          }
          uc.dt = data;
        }
        if (department(SharedObject.UserInfo.UserPid) == "CAR")
        {
          if (this.txtFilePatch.Text.Trim().Length == 0)
          {
            Shared.Utility.WindowUtinity.ShowMessageWarning("ERR0046");
            return;
          }
          try
          {
            dtSource = DaiCo.Shared.Utility.FunctionUtility.GetExcelToDataSetVersion2(txtFilePatch.Text.Trim(), "SELECT * FROM [ImportManHour(CAR) (1)$B7:I500]").Tables[0];
          }
          catch { }
          if (dtSource == null)
          {
            return;
          }
          foreach (DataRow row in dtSource.Rows)
          {
            if (row["Team"].ToString().Length == 0
              || row["WorkOrder"].ToString().Length == 0
              || row["Date"].ToString().Length == 0
              || row["Qty"].ToString().Length == 0
              || row["Manhour"].ToString().Length == 0)
            {
              continue;
            }
            DataRow newRow = data.NewRow();
            newRow["Team"] = row["Team"];
            newRow["Date"] = DBConvert.ParseDateTime(row["Date"].ToString(), formatConvert);
            newRow["WorkOrder"] = row["WorkOrder"];
            newRow["CarcassCode"] = row["Carcass Code"];
            newRow["Component"] = row["Component"];
            newRow["Qty"] = row["Qty"];
            newRow["Manhour"] = row["Manhour"];
            newRow["Remark"] = row["Remark"];
            data.Rows.Add(newRow);
          }
          uc.dt = data;


        }
        if (uc.dt.Rows.Count > 0)
        {
          DaiCo.Shared.Utility.WindowUtinity.ShowView(uc, "IMPORT DATA MANHOUR TO EXCEL", false, DaiCo.Shared.Utility.ViewState.ModalWindow);
          btnSearch_Click(sender, e);
        }
      }
      else
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0046");
      }
    }

    #endregion Event

    private void ultData_AfterCellActivate(object sender, EventArgs e)
    {
      if (cellDropDown != null)
      {
        cellDropDown.DroppedDown = true;
      }
    }

  }
}
