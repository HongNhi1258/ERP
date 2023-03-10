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
  public partial class viewPLN_10_017 : MainUserControl
  {
    #region Init
    string formatConvert = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private IList listDeleteDetailPid = new ArrayList();
    private IList listDeletingPid = new ArrayList();
    private string TeamCode = "";
    private DateTime dWDate;

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
      if (department(SharedObject.UserInfo.UserPid) == "ITD")
      {
        commandText = string.Format(@"SELECT DISTINCT Team Code, Team+' / '+ VTeamName Name  FROM VHRMCompanyStructure ");
      }
      else
      {
        commandText = string.Format(@"SELECT DISTINCT Team Code, Team+' / '+ VTeamName Name  FROM VHRMCompanyStructure WHERE  Department = '{0}'", department(SharedObject.UserInfo.UserPid));
      }
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultCbmTeamCode.DataSource = dt;
      ultCbmTeamCode.DisplayMember = "Name";
      ultCbmTeamCode.ValueMember = "Code";
      ultCbmTeamCode.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
      ultCbmTeamCode.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultCbmTeamCode.DisplayLayout.Bands[0].ColHeadersVisible = true;
    }
    private void LoadDropDownTeam()
    {
      string commandText = string.Empty;
      if (department(SharedObject.UserInfo.UserPid) == "ITD")
      {
        commandText = string.Format(@"SELECT DISTINCT Team Code, Team+' / '+ VTeamName Name  FROM VHRMCompanyStructure ");
      }
      else
      {
        commandText = string.Format(@"SELECT DISTINCT Team Code, Team+' / '+ VTeamName Name  FROM VHRMCompanyStructure WHERE  Department = '{0}'", department(SharedObject.UserInfo.UserPid));
      }
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
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
    private void LoadDropDownWo()
    {
      string commandText = "";
      if (chkNonWIP.Checked)
      {
        commandText = string.Format(@"SELECT DISTINCT WorkOrderPid WorkOrder 
                                           FROM TblPLNWorkOrderConfirmedDetails  ORDER BY WorkOrderPid DESC");
      }
      else
      {
        commandText = string.Format(@"SELECT DISTINCT Wo WorkOrder FROM VWIPTransactionOfComponentForManhourAllocation WHERE WorkAreaCode = '{0}' AND (CreateDate IS NULL OR CONVERT(DATE,CreateDate) = '{1}') ORDER BY Wo DESC", this.TeamCode, this.dWDate.ToString("dd-MMM-yyyy"));
      }
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dt != null)
      {
        ultdropdownWo.DataSource = dt;
        ultdropdownWo.DisplayMember = "WorkOrder";
        ultdropdownWo.ValueMember = "WorkOrder";
        ultdropdownWo.DisplayLayout.Bands[0].Columns["WorkOrder"].Width = 100;
        ultdropdownWo.DisplayLayout.Bands[0].ColHeadersVisible = true;
      }
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
        commandText = string.Format(@"SELECT ItemCode FROM TblPLNWorkOrderConfirmedDetails ");
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
        if (chkNonWIP.Checked)
        {
          commandText = string.Format(@"SELECT DISTINCT CAR.CarcassCode  
                                      FROM TblPLNWOCarcassInfomation  CAR                                      
                                      ORDER BY CAR.CarcassCode ASC ");
        }
        else
        {
          commandText = string.Format(@"SELECT DISTINCT Carcass CarcassCode FROM VWIPTransactionOfComponentForManhourAllocation WHERE WorkAreaCode = '{0}' AND (CreateDate IS NULL OR CONVERT(DATE,CreateDate) = '{1}') ORDER BY Carcass", this.TeamCode, this.dWDate.ToString("dd-MMM-yyyy"));

        }
      }
      else
      {
        if (chkNonWIP.Checked)
        {
          commandText = string.Format(@"SELECT DISTINCT CAR.CarcassCode  
                                      FROM TblPLNWOCarcassInfomation  CAR                                      
                                      WHERE CAR.Wo = {0}
                                      ORDER BY CAR.CarcassCode ASC", Wo);
        }
        else
        {
          commandText = string.Format(@"SELECT DISTINCT Carcass CarcassCode FROM VWIPTransactionOfComponentForManhourAllocation WHERE Wo = {2} AND WorkAreaCode = '{0}' AND (CreateDate IS NULL OR CONVERT(DATE,CreateDate) = '{1}') ORDER BY Carcass", this.TeamCode, this.dWDate.ToString("dd-MMM-yyyy"), Wo.ToString());

        }
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
          if (chkNonWIP.Checked)
          {
            commandText = string.Format(@"SELECT ComponentCode  FROM TblPLNWOCarcassInfomation WHERE Wo = {0}", wo);
          }
          else
          {
            commandText = string.Format(@"SELECT DISTINCT Component ComponentCode FROM VWIPTransactionOfComponentForManhourAllocation WHERE Wo = {2} AND WorkAreaCode = '{0}' AND (CreateDate IS NULL OR CONVERT(DATE,CreateDate) = '{1}') ORDER BY Component", this.TeamCode, this.dWDate.ToString("dd-MMM-yyyy"), wo.ToString());
          }
        }
        else
        {
          if (chkNonWIP.Checked)
          {
            commandText = string.Format(@"SELECT ComponentCode  FROM TblPLNWOCarcassInfomation WHERE Wo ={0} AND CarcassCode = '{1}'", wo, Carcass);
          }
          else
          {
            commandText = string.Format(@"SELECT DISTINCT Component ComponentCode FROM VWIPTransactionOfComponentForManhourAllocation WHERE Wo = {2} AND Carcass = '{3}' AND WorkAreaCode = '{0}' AND (CreateDate IS NULL OR CONVERT(DATE,CreateDate) = '{1}') ORDER BY Component", this.TeamCode, this.dWDate.ToString("dd-MMM-yyyy"), wo.ToString(), Carcass);
          }
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
    private UltraDropDown DropdownProcess(long wo, string Carcass, string ComponentCode, UltraDropDown ultDropdownProcessPid)
    {
      if (ultDropdownProcessPid == null)
      {
        ultDropdownProcessPid = new UltraDropDown();
        this.Controls.Add(ultDropdownProcessPid);
      }
      string commandText = string.Empty;

      DBParameter[] inputParam = new DBParameter[3];
      inputParam[0] = new DBParameter("@Wo", DbType.Int64, wo);
      inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, Carcass);
      inputParam[2] = new DBParameter("@Component", DbType.AnsiString, 32, ComponentCode);

      DataTable dt = DataBaseAccess.SearchStoreProcedureDataTable("spPLNProcessByWoCarcass_Select", inputParam);
      if (dt != null)
      {
        ultDropdownProcessPid.DataSource = dt;
        ultDropdownProcessPid.DisplayMember = "ProcessCode";
        ultDropdownProcessPid.ValueMember = "ProcessPid";
        try
        {
          ultDropdownProcessPid.DisplayLayout.Bands[0].Columns["ProcessPid"].Hidden = true;
        }
        catch
        { }
        ultDropdownProcessPid.DisplayLayout.Bands[0].ColHeadersVisible = true;
      }
      return ultDropdownProcessPid;
    }
    private bool CheckIsValid(out string message)
    {
      message = string.Empty;
      bool result = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          DataRow dr = (ultData.Rows[i].ChildBands[0].Rows[j].ListObject as DataRowView).Row;
          if (dr.RowState == DataRowState.Modified || dr.RowState == DataRowState.Added)
          {
            if (dr["NonOutput"].ToString().Trim() == "0" && dr["Carcass"].ToString().Trim().Length == 0)
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Carcass");
              ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
              result = false;
              break;
            }
            if (dr["NonOutput"].ToString().Trim() == "0" && dr["Component"].ToString().Trim().Length == 0)
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Component");
              ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
              result = false;
              break;
            }
            double totalMinutes = 0, totalRMinutes = 0;
            if (ultData.Rows[i].Cells["Kind"].Value.ToString() == "1")
            {
              totalMinutes = DBConvert.ParseDouble(ultData.Rows[i].Cells["Overtime"].Value.ToString());
            }
            else
            {
              totalMinutes = DBConvert.ParseDouble(ultData.Rows[i].Cells["Manhour"].Value.ToString());
            }

            for (int t = 0; t < ultData.Rows[i].ChildBands[0].Rows.Count; t++)
            {
              if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[t].Cells["Processing"].Value.ToString()) > 0)
              {
                totalRMinutes += DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[t].Cells["Processing"].Value.ToString());
              }
              if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[t].Cells["Setup"].Value.ToString()) > 0)
              {
                totalRMinutes += DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[t].Cells["Setup"].Value.ToString());
              }
              if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[t].Cells["Minutes"].Value.ToString()) > 0)
              {
                totalRMinutes += DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[t].Cells["Minutes"].Value.ToString());
              }
            }
            if (totalMinutes < totalRMinutes)
            {
              message = string.Format(Shared.Utility.FunctionUtility.GetMessage("ERR0001"), "Qty");
              ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
              result = false;
              break;
            }
            break;
          }

        }
      }
      return result;
    }

    #endregion Load

    #region Event

    public viewPLN_10_017()
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
      this.ultdtWDate.Value = DateTime.Today.AddDays(-7);

      LoadComboboxTeam();
      LoadDropDownTeam();
      LoadComboboxWo();
      LoadDropDownWo();
      //LoadComboboxItemCode();
      LoadComboboxCarcass();
      LoadComboboxComponent();
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
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.False;
      //e.Layout.Bands[0].Columns["EID"].Hidden = true;
      e.Layout.Bands[0].Columns["Kind"].Hidden = true;
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Minutes"].Hidden = true;

      e.Layout.Bands[0].Columns["TotalMinutes"].CellAppearance.BackColor = Color.Yellow;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (i == 0)
        {
          if (ultData.Rows[i].Cells["Kind"].Value.ToString() == "1")
          {
            e.Layout.Bands[0].Columns["Manhour"].Hidden = true;
            e.Layout.Bands[0].Columns["Overtime"].Hidden = false;
            e.Layout.Bands[1].Columns["Minutes"].Header.Caption = "Overtime";

          }
          else
          {
            e.Layout.Bands[0].Columns["Overtime"].Hidden = true;
            e.Layout.Bands[0].Columns["Manhour"].Hidden = false;
            e.Layout.Bands[1].Columns["Minutes"].Header.Caption = "Manhour";
          }
        }
        ultData.Rows[i].ExpandAll();
      }
      DataTable dtNew = ((DataSet)ultData.DataSource).Tables[0];
      for (int i = 0; i < dtNew.Columns.Count; i++)
      {
        ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightSkyBlue;
        if (dtNew.Columns[i].DataType == typeof(Int32) || dtNew.Columns[i].DataType == typeof(float) || dtNew.Columns[i].DataType == typeof(Double))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dtNew.Columns[i].DataType == typeof(DateTime))
        {
          ultData.DisplayLayout.Bands[0].Columns[i].Format = "dd-MMM-yyyy";
        }
      }
      dtNew = ((DataSet)ultData.DataSource).Tables[1];
      for (int i = 0; i < dtNew.Columns.Count; i++)
      {
        if (dtNew.Columns[i].DataType == typeof(Int32) || dtNew.Columns[i].DataType == typeof(float) || dtNew.Columns[i].DataType == typeof(Double))
        {
          ultData.DisplayLayout.Bands[1].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
        else if (dtNew.Columns[i].DataType == typeof(DateTime))
        {
          ultData.DisplayLayout.Bands[1].Columns[i].Format = "dd-MMM-yyyy";
        }
      }
      e.Layout.Bands[0].Columns["Team"].ValueList = ultDropdownTeam;
      e.Layout.Bands[0].Columns["Team"].MaxWidth = 200;
      e.Layout.Bands[0].Columns["Team"].MinWidth = 200;
      e.Layout.Bands[0].Columns["Date"].Width = 70;
      e.Layout.Bands[1].Columns["WorkOrder"].ValueList = ultdropdownWo;
      e.Layout.Bands[1].Columns["WorkOrder"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["WorkOrder"].MinWidth = 100;
      e.Layout.Bands[1].Columns["Carcass"].ValueList = ultDropdownCarcass;
      e.Layout.Bands[1].Columns["Carcass"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["Carcass"].MinWidth = 100;
      e.Layout.Bands[1].Columns["Component"].ValueList = ultDropdownComponent;
      e.Layout.Bands[1].Columns["WCP"].ValueList = ultDropdownProcess;
      e.Layout.Bands[1].Columns["Component"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["Component"].MinWidth = 100;
      e.Layout.Bands[1].Columns["WorkOrder"].Header.Caption = "Work Order";
      e.Layout.Bands[1].Columns["WorkOrder"].Header.Caption = "Work Order";
      e.Layout.Bands[1].Columns["Qty"].Width = 70;
      e.Layout.Bands[1].Columns["Minutes"].Width = 70;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Kind"].Hidden = true;
      e.Layout.Bands[1].Columns["Estimated"].Hidden = true;
      e.Layout.Bands[1].Columns["ManhourAllocationDetailPid"].Hidden = true;
      e.Layout.Bands[1].Columns["NonOutput"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[1].Columns["NonOutput"].Header.Caption = "NON Carcass";
      e.Layout.Bands[1].Columns["Minutes"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Qty"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Processing"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["Minutes"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
      e.Layout.Bands[1].Columns["WorkOrder"].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
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
              this.DropdownCarcass(DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString()), ultDropdownCarcass);
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
            }
          }
          catch
          {
          }
          break;
        //case "component":
        //  try
        //  {
        //    if (e.Cell.Row.Cells["Component"].Value.ToString() != string.Empty)
        //    {
        //      this.DropdownProcess(DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString()), e.Cell.Row.Cells["Carcass"].Value.ToString(), e.Cell.Row.Cells["Component"].Value.ToString(), ultDropdownProcess);
        //    }
        //  }
        //  catch
        //  {
        //  }
        //  break;
        default:
          break;
      }
      if (columnName == "minutes" || columnName == "processing" || columnName == "setup")
      {
        int totalMinutes = 0;
        for (int i = 0; i < e.Cell.Row.ParentRow.ChildBands[0].Rows.Count; i++)
        {
          if (DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Minutes"].Value.ToString()) > 0)
          {
            totalMinutes += DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Minutes"].Value.ToString());
          }
          if (DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Processing"].Value.ToString()) > 0)
          {
            totalMinutes += DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Processing"].Value.ToString());
          }
          if (DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Setup"].Value.ToString()) > 0)
          {
            totalMinutes += DBConvert.ParseInt(e.Cell.Row.ParentRow.ChildBands[0].Rows[i].Cells["Setup"].Value.ToString());
          }
        }
        e.Cell.Row.ParentRow.Cells["TotalMinutes"].Value = totalMinutes;
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
            long wo = DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString());
            UltraDropDown ultDropdownCarcass = (UltraDropDown)e.Cell.Row.Cells["Carcass"].ValueList;
            e.Cell.Row.Cells["Carcass"].ValueList = this.DropdownCarcass(wo, ultDropdownCarcass);
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
        case "wcp":
          {
            long wo = DBConvert.ParseLong(e.Cell.Row.Cells["Workorder"].Value.ToString());
            string carcass = e.Cell.Row.Cells["Carcass"].Value.ToString();
            string component = e.Cell.Row.Cells["Component"].Value.ToString();
            UltraDropDown ultDropdownProcessPid = (UltraDropDown)e.Cell.Row.Cells["WCP"].ValueList;
            e.Cell.Row.Cells["WCP"].ValueList = this.DropdownProcess(wo, carcass, component, ultDropdownProcessPid);
            break;
          }
      }
      if (colName == "wcp")
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
      this.ultdtWDate.Value = null;
    }

    private void btnSearch_Click(object sender, EventArgs e)
    {
      try
      {
        if (ultCbmTeamCode.Value.ToString().Trim().Length == 0 || DBConvert.ParseDateTime(ultdtWDate.Value.ToString(), formatConvert) <= DateTime.MinValue)
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
      DBParameter[] input = new DBParameter[6];
      if (ultCbmTeamCode.Value != null)
      {
        input[0] = new DBParameter("@TeamCode", DbType.AnsiString, 32, ultCbmTeamCode.Value.ToString());
        this.TeamCode = ultCbmTeamCode.Value.ToString();
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
      if (ultdtWDate.Value != null)
      {
        input[4] = new DBParameter("@WDate", DbType.DateTime, DBConvert.ParseDateTime(ultdtWDate.Value.ToString(), formatConvert));
        this.dWDate = DBConvert.ParseDateTime(ultdtWDate.Value.ToString(), formatConvert);
      }
      if (chkOT.Checked)
      {
        input[5] = new DBParameter("@Kind", DbType.Int32, 1);
      }
      DataSet dsSource = Shared.DataBaseUtility.DataBaseAccess.SearchStoreProcedure("spPLNManhourAllocationForCom_Select", input);
      dsSource.Relations.Add(new DataRelation("TblParent_TblChild", new DataColumn[] { dsSource.Tables[0].Columns["Pid"] },
                                                    new DataColumn[] { dsSource.Tables[1].Columns["Pid"] }, false));
      this.LoadDropDownWo();
      ultData.DataSource = dsSource;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        ultData.Rows[i].CellAppearance.BackColor = Color.LightSkyBlue;
        if (!(chkOT.Checked) && DBConvert.ParseDouble(ultData.Rows[i].Cells["Manhour"].Value.ToString()) < DBConvert.ParseDouble(ultData.Rows[i].Cells["TotalMinutes"].Value.ToString()))
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
        if (chkOT.Checked && DBConvert.ParseDouble(ultData.Rows[i].Cells["Overtime"].Value.ToString()) < DBConvert.ParseDouble(ultData.Rows[i].Cells["TotalMinutes"].Value.ToString()))
        {
          ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
        }
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseLong(ultData.Rows[i].Cells["Pid"].Value.ToString()) <= 0)
        {
          ultData.Rows[i].Cells["Pid"].Value = -(i);
        }
      }

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
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        for (int j = 0; j < ultData.Rows[i].ChildBands[0].Rows.Count; j++)
        {
          DataRow dr = (ultData.Rows[i].ChildBands[0].Rows[j].ListObject as DataRowView).Row;
          if (dr.RowState == DataRowState.Modified || dr.RowState == DataRowState.Added)
          {
            long ParentPid = 0;
            if (DBConvert.ParseLong(ultData.Rows[i].Cells["Pid"].Value.ToString()) <= 0)
            {
              //insert parent
              DBParameter[] inputParam = new DBParameter[4];
              inputParam[0] = new DBParameter("@TeamCode", DbType.AnsiString, 32, ultData.Rows[i].Cells["Team"].Value.ToString().Trim());
              inputParam[1] = new DBParameter("@EID", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].Cells["EID"].Value.ToString()));
              inputParam[2] = new DBParameter("@WDate", DbType.DateTime, DBConvert.ParseDateTime(ultData.Rows[i].Cells["Date"].Value.ToString(), formatConvert));
              inputParam[3] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
              DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
              Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNManhourAllocationForCom_Edit", inputParam, outputParam);
              ParentPid = DBConvert.ParseLong(outputParam[0].Value.ToString());
            }
            else
            {
              ParentPid = DBConvert.ParseLong(ultData.Rows[i].Cells["Pid"].Value.ToString());
            }
            //insert child
            DBParameter[] inputParamChild = new DBParameter[14];
            if (DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["ManhourAllocationDetailPid"].Value.ToString()) > 0)
            {
              inputParamChild[0] = new DBParameter("@Pid", DbType.Int32, DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["ManhourAllocationDetailPid"].Value.ToString()));
            }
            inputParamChild[1] = new DBParameter("@WoPid", DbType.Int64, 32, DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["WorkOrder"].Value.ToString()));
            inputParamChild[2] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, ultData.Rows[i].ChildBands[0].Rows[j].Cells["Carcass"].Value.ToString());
            inputParamChild[3] = new DBParameter("@Component", DbType.AnsiString, 32, ultData.Rows[i].ChildBands[0].Rows[j].Cells["Component"].Value.ToString());
            if (DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["WCP"].Value.ToString()) > 0)
            {
              inputParamChild[4] = new DBParameter("@ProcessPid", DbType.Int64, DBConvert.ParseLong(ultData.Rows[i].ChildBands[0].Rows[j].Cells["WCP"].Value.ToString()));
            }
            if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString()) > 0)
            {
              inputParamChild[5] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Qty"].Value.ToString()));
            }
            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Minutes"].Value.ToString()) > 0)
            {
              inputParamChild[6] = new DBParameter("@Minutes", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Minutes"].Value.ToString()));
            }
            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Processing"].Value.ToString()) > 0)
            {
              inputParamChild[7] = new DBParameter("@Processing", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Processing"].Value.ToString()));
            }
            if (DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Setup"].Value.ToString()) > 0)
            {
              inputParamChild[8] = new DBParameter("@Setup", DbType.Double, DBConvert.ParseDouble(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Setup"].Value.ToString()));
            }
            if (DBConvert.ParseInt(ultData.Rows[i].Cells["Kind"].Value.ToString()) > 0)
            {
              inputParamChild[9] = new DBParameter("@Kind", DbType.Int32, 1);
            }
            else
            {
              inputParamChild[9] = new DBParameter("@Kind", DbType.Int32, 0);
            }
            if (ultData.Rows[i].ChildBands[0].Rows[j].Cells["Remark"].Value.ToString().Trim().Length > 0)
            {
              inputParamChild[10] = new DBParameter("@Remark", DbType.String, 128, ultData.Rows[i].ChildBands[0].Rows[j].Cells["Remark"].Value.ToString());
            }
            inputParamChild[11] = new DBParameter("@ManhourAllocationPid", DbType.Int64, ParentPid);
            if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Estimated"].Value.ToString()) > 0)
            {
              inputParamChild[12] = new DBParameter("@Estimated", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["Estimated"].Value.ToString()));
            }
            if (DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["NonOutPut"].Value.ToString()) > 0)
            {
              inputParamChild[13] = new DBParameter("@NonOutput", DbType.Int32, DBConvert.ParseInt(ultData.Rows[i].ChildBands[0].Rows[j].Cells["NonOutPut"].Value.ToString()));
            }
            DBParameter[] outputParamChild = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
            Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure("spPLNManhourAllocationForComDetail_Edit", inputParamChild, outputParamChild);
            long outValue = DBConvert.ParseLong(outputParamChild[0].Value.ToString());
            if (outValue == -1 || outValue == 0)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
              ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
            }
          }
        }
      }
      //DataTable dt = (DataTable)ultData.DataSource;

      //if (dt.Rows.Count > 0)
      //{
      //  for (int i = 0; i < dt.Rows.Count; i++)
      //  {
      //    string storeName = string.Empty;
      //    DBParameter[] inputParam = new DBParameter[14];
      //    DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
      //    if (dt.Rows[i].RowState == DataRowState.Modified)
      //    {
      //      storeName = "spPLNManhourAllocation_Update";
      //      inputParam[0] = new DBParameter("@Pid", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Pid"].ToString()));
      //      inputParam[13] = new DBParameter("@UpdateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      //    }
      //    if (dt.Rows[i].RowState == DataRowState.Added)
      //    {
      //      storeName = "spPLNManhourAllocation_Insert";
      //      inputParam[12] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);
      //    }
      //    if (storeName.Length > 0)
      //    {
      //      if (dt.Rows[i]["Team"].ToString() != string.Empty)
      //      {
      //        inputParam[1] = new DBParameter("@TeamCode", DbType.AnsiString, 32, dt.Rows[i]["Team"].ToString().Trim());
      //      }
      //      if (DBConvert.ParseDateTime(dt.Rows[i]["Date"].ToString(), formatConvert) != DateTime.MinValue)
      //      {
      //        inputParam[2] = new DBParameter("@WDate", DbType.DateTime, DBConvert.ParseDateTime(dt.Rows[i]["Date"].ToString(), formatConvert));
      //      }
      //      if (DBConvert.ParseLong(dt.Rows[i]["Workorder"].ToString()) != long.MinValue)
      //      {
      //        inputParam[3] = new DBParameter("@Wo", DbType.Int64, DBConvert.ParseLong(dt.Rows[i]["Workorder"].ToString()));
      //      }
      //      if (dt.Rows[i]["Carcass"].ToString() != string.Empty)
      //      {
      //        inputParam[4] = new DBParameter("@CarcassCode", DbType.AnsiString, 32, dt.Rows[i]["Carcass"].ToString().Trim());
      //      }
      //      if (dt.Rows[i]["Component"].ToString() != string.Empty)
      //      {
      //        inputParam[5] = new DBParameter("@Component", DbType.AnsiString, 32, dt.Rows[i]["Component"].ToString().Trim());
      //      }
      //      if (dt.Rows[i]["WCP"].ToString() != string.Empty)
      //      {
      //        inputParam[6] = new DBParameter("@WCP", DbType.String, 500, dt.Rows[i]["WCP"].ToString().Trim());
      //      }
      //      inputParam[7] = new DBParameter("@Qty", DbType.Int32, DBConvert.ParseInt(dt.Rows[i]["Qty"].ToString()));

      //      if (DBConvert.ParseDouble(dt.Rows[i]["Minutes"].ToString()) != Double.MinValue)
      //      {
      //        inputParam[8] = new DBParameter("@Minutes", DbType.Double, DBConvert.ParseDouble(dt.Rows[i]["Minutes"].ToString()));
      //      }
      //      if (DBConvert.ParseDouble(dt.Rows[i]["Processing"].ToString()) != Double.MinValue)
      //      {
      //        inputParam[9] = new DBParameter("@Processing", DbType.Double, DBConvert.ParseDouble(dt.Rows[i]["Processing"].ToString()));
      //      }
      //      if (DBConvert.ParseDouble(dt.Rows[i]["Manhour"].ToString()) != Double.MinValue)
      //      {
      //        inputParam[10] = new DBParameter("@Manhour", DbType.Double, DBConvert.ParseDouble(dt.Rows[i]["Manhour"].ToString()));
      //      }
      //      if (dt.Rows[i]["Remark"].ToString() != string.Empty)
      //      {
      //        inputParam[11] = new DBParameter("@Remark", DbType.String, 500, dt.Rows[i]["Remark"].ToString());
      //      }
      //      Shared.DataBaseUtility.DataBaseAccess.ExecuteStoreProcedure(storeName, inputParam, outputParam);
      //      long outValue = DBConvert.ParseLong(outputParam[0].Value.ToString());
      //      Out += "," + outValue.ToString();
      //      if (outValue == -1 || outValue == 0)
      //      {
      //        Shared.Utility.WindowUtinity.ShowMessageError("ERR0005");
      //        ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
      //      }
      //    }
      //  }
      //}
      if (this.listDeleteDetailPid.Count > 0)
      {
        foreach (long pid in this.listDeleteDetailPid)
        {
          string storeNamedelete = "spPLNManhourAllocationForComDetail_Delete";
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
        long pid = DBConvert.ParseLong(row.Cells["ManhourAllocationDetailPid"].Value.ToString());
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
      //if (string.Compare(colName, "Minutes", true) == 0)
      //{
      //  double values = DBConvert.ParseDouble(e.Cell.Text.Trim());
      //  if (values < 0)
      //  {
      //    Shared.Utility.WindowUtinity.ShowMessageError("ERR0110", new string[] { "Manhour" });
      //    e.Cancel = true;
      //  }
      //}
      if (string.Compare(colName, "Processing", true) == 0)
      {
        double values = DBConvert.ParseDouble(e.Cell.Text.Trim());
        if (values <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0110", new string[] { "Processing" });
          e.Cancel = true;
        }
      }
      if (string.Compare(colName, "Setup", true) == 0)
      {
        double values = DBConvert.ParseDouble(e.Cell.Text.Trim());
        if (values <= 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0110", new string[] { "Setup" });
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
      if (string.Compare(colName, "Workorder", true) == 0 && e.Cell.Row.Cells["NonOutPut"].Value.ToString() != "1")
      {
        int values = DBConvert.ParseInt(e.Cell.Text.Trim());
        if (values < 0)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Workorder" });
          e.Cancel = true;
        }
      }
      if (string.Compare(colName, "Carcass", true) == 0 && e.Cell.Row.Cells["NonOutPut"].Value.ToString() != "1")
      {
        UltraDropDown ultCarcass = (UltraDropDown)e.Cell.ValueList;
        DataTable dt = (DataTable)ultCarcass.DataSource;
        bool isvalid = false;
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i]["CarcassCode"].ToString().Trim() == e.Cell.Text.Trim())
          {
            isvalid = true;
            break;
          }
        }
        if (!isvalid)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Carcass" });
          e.Cancel = true;
        }
      }
      if (string.Compare(colName, "Component", true) == 0 && e.Cell.Row.Cells["NonOutPut"].Value.ToString() != "1")
      {
        UltraDropDown ultComponent = (UltraDropDown)e.Cell.ValueList;
        DataTable dt = (DataTable)ultComponent.DataSource;
        bool isvalid = false;
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          if (dt.Rows[i]["ComponentCode"].ToString().Trim() == e.Cell.Text.Trim())
          {
            isvalid = true;
            break;
          }
        }
        if (!isvalid)
        {
          Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", new string[] { "Component" });
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

    private void btnChecking_Click(object sender, EventArgs e)
    {
      string message = string.Empty;
      bool success = this.CheckIsValid(out message);
      if (!success)
      {
        MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }
    }

    private void btnExportExcel_Click(object sender, EventArgs e)
    {
      // Check Data
      if (ultCbmTeamCode.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "TeamCode");
        return;
      }

      if (ultdtWDate.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "WDate");
        return;
      }
      // End

      string team = ultCbmTeamCode.Value.ToString();
      DateTime wDate = DBConvert.ParseDateTime(ultdtWDate.Value.ToString(), ConstantClass.USER_COMPUTER_FORMAT_DATETIME);

      int kind = int.MinValue;
      if (chkOT.Checked) { kind = 1; } else { kind = 0; }
      int nonWIP = int.MinValue;
      if (chkNonWIP.Checked) { nonWIP = 1; } else { nonWIP = 0; }

      viewPLN_10_018 uc = new viewPLN_10_018();
      uc.team = team;
      uc.wDate = wDate;
      uc.kind = kind;
      uc.nonWIP = nonWIP;
      WindowUtinity.ShowView(uc, "IMPORT DATA ALLOCATE FOR COMPONENT", true, ViewState.ModalWindow, FormWindowState.Maximized);
    }
  }
}
