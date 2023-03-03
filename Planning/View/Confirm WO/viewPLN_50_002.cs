/*
  Author      : Nguyen Huynh Quoc Tuan
  Date        : 11/5/2016
  Description : input deadline confirm Wo Online
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
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
  public partial class viewPLN_50_002 : MainUserControl
  {
    #region Field
    public long viewPid = long.MinValue;
    private IList listDeletedPid = new ArrayList();
    public DataTable dtDraft = new DataTable();
    public DataTable dtConfirm = new DataTable();
    public DataTable dtKind = new DataTable();
    public string dept = string.Empty;
    public long transactionPid = long.MinValue;
    public bool flagSearch = false;
    #endregion Field

    #region Init
    public viewPLN_50_002()
    {
      InitializeComponent();
    }

    private void viewPLN_50_002_Load(object sender, EventArgs e)
    {
      // Add ask before closing form even if user change data
      foreach (Control ctr in tableLayoutMaster.Controls)
      {
        ctr.TextChanged += new System.EventHandler(this.Object_Changed);
      }
      this.InitData();
      this.LoadData();
    }
    #endregion Init

    #region Function
    private void SetNeedToSave()
    {
      if (btnSave.Enabled && btnSave.Visible)
      {
        this.NeedToSave = true;
      }
      else
      {
        this.NeedToSave = false;
      }
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadCBKind();
      lblDept.Text = this.dept;
    }

    private void SetStatusControl()
    {

    }
    private void LoadCBKind()
    {
      ControlUtility.LoadUltraCombo(ultcbDeadline, this.dtKind, "value", "text", false, "value");
    }

    private void LoadMainData(DataTable dtMain)
    {
      if (dtMain.Rows.Count > 0)
      {
      }
    }

    private void LoadData()
    {
      this.listDeletedPid = new ArrayList();

      //DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, long.MinValue) };
      //DataSet dsSource = DataBaseAccess.SearchStoreProcedure("", inputParam);
      //if (dsSource != null && dsSource.Tables.Count > 1)
      //{
      //  this.LoadMainData(dsSource.Tables[0]);
      //  ultData.DataSource = dsSource.Tables[1];
      //}

      //this.SetStatusControl();
      this.NeedToSave = false;
    }

    private bool CheckValid()
    {
      if (ultCBType.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Type");
        return false;
      }

      if (ultcbDeadline.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Kind");
        return false;
      }

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) > 0)
        {
          WindowUtinity.ShowMessageError("ERR0050", "Error");
          return false;
        }
      }
      return true;
    }

    private DataTable dtDeadlineResult()
    {
      DataTable dt = new DataTable();
      dt.Columns.Add("WorkOrder", typeof(System.Int64));
      dt.Columns.Add("ItemCode", typeof(System.String));
      dt.Columns.Add("Revision", typeof(System.Int32));
      dt.Columns.Add("Qty", typeof(System.Int32));
      dt.Columns.Add("COM1Deadline", typeof(System.DateTime));
      dt.Columns.Add("COM1Remark", typeof(System.String));
      dt.Columns.Add("ASSYDeadline", typeof(System.DateTime));
      dt.Columns.Add("ASSYRemark", typeof(System.String));
      dt.Columns.Add("SUBDeadline", typeof(System.DateTime));
      dt.Columns.Add("SUBRemark", typeof(System.String));
      dt.Columns.Add("CARDeadline", typeof(System.DateTime));
      dt.Columns.Add("CARRemark", typeof(System.String));
      dt.Columns.Add("ASSHWDeadline", typeof(System.DateTime));
      dt.Columns.Add("ASSHWRemark", typeof(System.String));
      dt.Columns.Add("FFHWDeadline", typeof(System.DateTime));
      dt.Columns.Add("FFHWRemark", typeof(System.String));
      dt.Columns.Add("MATDeadline", typeof(System.DateTime));
      dt.Columns.Add("MATRemark", typeof(System.String));
      dt.Columns.Add("SAMDeadline", typeof(System.DateTime));
      dt.Columns.Add("SAMRemark", typeof(System.String));
      return dt;
    }

    private void ImportData()
    {
      if (this.txtImport.Text.Trim().Length == 0)
      {
        return;
      }

      if (ultCBType.Value == null)
      {
        WindowUtinity.ShowMessageError("ERR0001", "Type");
        return;
      }

      // Get Data Table From Excel
      DataTable dtSource = new DataTable();
      //dtSource = FunctionUtility.GetExcelToDataSet(txtLocation.Text.Trim(), "SELECT * FROM [Sheet1 (1)$B5:F805]").Tables[0];
      dtSource = FunctionUtility.GetDataFromExcel(txtImport.Text.Trim(), "Sheet1 (1)", "B3:U1000");
      if (dtSource == null)
      {
        return;
      }

      // Input ------- 
      SqlDBParameter[] sqlinput = new SqlDBParameter[4];
      DataTable dtDeadlineInput = this.dtDeadlineResult();

      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtDeadlineInput.NewRow();
        if (DBConvert.ParseLong(row["WorkOrder"].ToString()) > 0)
        {
          // WO
          if (DBConvert.ParseLong(row["WorkOrder"].ToString()) > 0)
          {
            rowadd["WorkOrder"] = DBConvert.ParseLong(row["WorkOrder"].ToString());
          }

          // ItemCode
          if (row["ItemCode"].ToString().Length > 0)
          {
            rowadd["ItemCode"] = row["ItemCode"];
          }

          // Revision
          if (DBConvert.ParseInt(row["Revision"].ToString()) != int.MinValue)
          {
            rowadd["Revision"] = DBConvert.ParseInt(row["Revision"]);
          }

          // Qty
          if (DBConvert.ParseInt(row["Qty"].ToString()) != int.MinValue)
          {
            rowadd["Qty"] = DBConvert.ParseInt(row["Qty"]);
          }

          // COM1 Deadline
          if (row["COM1_Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["COM1Deadline"] = DBConvert.ParseDateTime(row["COM1_Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // COM1 Remark
          if (row["COM1_Remark"].ToString().Length > 0)
          {
            rowadd["COM1Remark"] = row["COM1_Remark"];
          }

          // ASSY Deadline
          if (row["ASSY_Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["ASSYDeadline"] = DBConvert.ParseDateTime(row["ASSY_Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // ASSY Remark
          if (row["ASSY_Remark"].ToString().Length > 0)
          {
            rowadd["ASSYRemark"] = row["ASSY_Remark"];
          }

          // SUB Deadline
          if (row["SUB_Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["SUBDeadline"] = DBConvert.ParseDateTime(row["SUB_Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // SUB Remark
          if (row["SUB_Remark"].ToString().Length > 0)
          {
            rowadd["SUBRemark"] = row["SUB_Remark"];
          }

          // CAR Deadline
          if (row["CAR_Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["CARDeadline"] = DBConvert.ParseDateTime(row["CAR_Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // CAR Remark
          if (row["CAR_Remark"].ToString().Length > 0)
          {
            rowadd["CARRemark"] = row["CAR_Remark"];
          }

          // ASSHW Deadline
          if (row["ASSHW_Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["ASSHWDeadline"] = DBConvert.ParseDateTime(row["ASSHW_Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // ASSHW Remark
          if (row["ASSHW_Remark"].ToString().Length > 0)
          {
            rowadd["ASSHWRemark"] = row["ASSHW_Remark"];
          }

          // FFHW Deadline
          if (row["FFHW_Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["FFHWDeadline"] = DBConvert.ParseDateTime(row["FFHW_Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // FFHW Remark
          if (row["FFHW_Remark"].ToString().Length > 0)
          {
            rowadd["FFHWRemark"] = row["FFHW_Remark"];
          }

          // MAT Deadline
          if (row["MAT_Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["MATDeadline"] = DBConvert.ParseDateTime(row["MAT_Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // ASSHW Remark
          if (row["MAT_Remark"].ToString().Length > 0)
          {
            rowadd["MATRemark"] = row["MAT_Remark"];
          }

          // SAM Deadline
          if (row["SAM_Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["SAMDeadline"] = DBConvert.ParseDateTime(row["SAM_Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // SAM Remark
          if (row["SAM_Remark"].ToString().Length > 0)
          {
            rowadd["SAMRemark"] = row["SAM_Remark"];
          }


          //Add row datatable
          dtDeadlineInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtDeadlineInput);
      sqlinput[1] = new SqlDBParameter("@Type", SqlDbType.Int, DBConvert.ParseInt(ultCBType.Value.ToString()));
      sqlinput[2] = new SqlDBParameter("@TransactionPid", SqlDbType.BigInt, this.transactionPid);
      sqlinput[3] = new SqlDBParameter("@Kind", SqlDbType.Int, DBConvert.ParseInt(ultcbDeadline.Value.ToString()));
      DataTable dtResultDeadline = SqlDataBaseAccess.SearchStoreProcedureDataTable("spPLNImportConfirmedWODraftDeadline", 1000, sqlinput);
      ultData.DataSource = dtResultDeadline;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) != 0)
        {
          if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 1)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.ForestGreen;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 2)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.CadetBlue;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 3)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Goldenrod;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 4)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Violet;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 5)
          {
            ultData.Rows[i].Cells["COM1Deadline"].Appearance.BackColor = Color.LightBlue;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 6)
          {
            ultData.Rows[i].Cells["ASSYDeadline"].Appearance.BackColor = Color.LightBlue;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 7)
          {
            ultData.Rows[i].Cells["SUBDeadline"].Appearance.BackColor = Color.LightBlue;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 8)
          {
            ultData.Rows[i].Cells["CARDeadline"].Appearance.BackColor = Color.LightBlue;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 9)
          {
            ultData.Rows[i].Cells["ASSHWDeadline"].Appearance.BackColor = Color.LightBlue;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 10)
          {
            ultData.Rows[i].Cells["FFHWDeadline"].Appearance.BackColor = Color.LightBlue;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 11)
          {
            ultData.Rows[i].Cells["MATDeadline"].Appearance.BackColor = Color.LightBlue;
          }
          else if (DBConvert.ParseInt(ultData.Rows[i].Cells["Error"].Value.ToString()) == 12)
          {
            ultData.Rows[i].Cells["SAMDeadline"].Appearance.BackColor = Color.LightBlue;
          }
          else
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Empty;
          }
        }
      }
    }

    private bool SaveMain()
    {
      DataTable dtSource = (DataTable)ultData.DataSource;
      // Input ------- 
      SqlDBParameter[] sqlinput = new SqlDBParameter[5];
      DataTable dtDeadlineInput = this.dtDeadlineResult();

      for (int i = 0; i < dtSource.Rows.Count; i++)
      {
        DataRow row = dtSource.Rows[i];
        DataRow rowadd = dtDeadlineInput.NewRow();
        if (DBConvert.ParseLong(row["WorkOrder"].ToString()) > 0)
        {
          // WO
          if (DBConvert.ParseLong(row["WorkOrder"].ToString()) > 0)
          {
            rowadd["WorkOrder"] = DBConvert.ParseLong(row["WorkOrder"].ToString());
          }

          // ItemCode
          if (row["ItemCode"].ToString().Length > 0)
          {
            rowadd["ItemCode"] = row["ItemCode"];
          }

          // Revision
          if (DBConvert.ParseInt(row["Revision"].ToString()) != int.MinValue)
          {
            rowadd["Revision"] = DBConvert.ParseInt(row["Revision"]);
          }

          // Qty
          if (DBConvert.ParseInt(row["Qty"].ToString()) != int.MinValue)
          {
            rowadd["Qty"] = DBConvert.ParseInt(row["Qty"]);
          }

          // COM1 Deadline
          if (row["COM1Deadline"].ToString().Trim().Length > 0)
          {
            rowadd["COM1Deadline"] = DBConvert.ParseDateTime(row["COM1Deadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // COM1 Remark
          if (row["COM1Remark"].ToString().Length > 0)
          {
            rowadd["COM1Remark"] = row["COM1Remark"];
          }

          // ASSY Deadline
          if (row["ASSYDeadline"].ToString().Trim().Length > 0)
          {
            rowadd["ASSYDeadline"] = DBConvert.ParseDateTime(row["ASSYDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // ASSY Remark
          if (row["ASSYRemark"].ToString().Length > 0)
          {
            rowadd["ASSYRemark"] = row["ASSYRemark"];
          }

          // SUB Deadline
          if (row["SUBDeadline"].ToString().Trim().Length > 0)
          {
            rowadd["SUBDeadline"] = DBConvert.ParseDateTime(row["SUBDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // SUB Remark
          if (row["SUBRemark"].ToString().Length > 0)
          {
            rowadd["SUBRemark"] = row["SUBRemark"];
          }

          // CAR Deadline
          if (row["CARDeadline"].ToString().Trim().Length > 0)
          {
            rowadd["CARDeadline"] = DBConvert.ParseDateTime(row["CARDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // CAR Remark
          if (row["CARRemark"].ToString().Length > 0)
          {
            rowadd["CARRemark"] = row["CARRemark"];
          }

          // ASSHW Deadline
          if (row["ASSHWDeadline"].ToString().Trim().Length > 0)
          {
            rowadd["ASSHWDeadline"] = DBConvert.ParseDateTime(row["ASSHWDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // ASSHW Remark
          if (row["ASSHWRemark"].ToString().Length > 0)
          {
            rowadd["ASSHWRemark"] = row["ASSHWRemark"];
          }

          // FFHW Deadline
          if (row["FFHWDeadline"].ToString().Trim().Length > 0)
          {
            rowadd["FFHWDeadline"] = DBConvert.ParseDateTime(row["FFHWDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // FFHW Remark
          if (row["FFHWRemark"].ToString().Length > 0)
          {
            rowadd["FFHWRemark"] = row["FFHWRemark"];
          }

          // MAT Deadline
          if (row["MATDeadline"].ToString().Trim().Length > 0)
          {
            rowadd["MATDeadline"] = DBConvert.ParseDateTime(row["MATDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // MAT Remark
          if (row["MATRemark"].ToString().Length > 0)
          {
            rowadd["MATRemark"] = row["MATRemark"];
          }

          // SAM Deadline
          if (row["SAMDeadline"].ToString().Trim().Length > 0)
          {
            rowadd["SAMDeadline"] = DBConvert.ParseDateTime(row["SAMDeadline"].ToString(), Shared.Utility.ConstantClass.USER_COMPUTER_FORMAT_DATETIME);
          }

          // SAM Remark
          if (row["SAMRemark"].ToString().Length > 0)
          {
            rowadd["SAMRemark"] = row["SAMRemark"];
          }


          //Add row datatable
          dtDeadlineInput.Rows.Add(rowadd);
        }
      }
      sqlinput[0] = new SqlDBParameter("@ImportData", SqlDbType.Structured, dtDeadlineInput);
      if (ultcbDeadline.Value != null)
      {
        sqlinput[1] = new SqlDBParameter("@Kind", SqlDbType.Int, DBConvert.ParseInt(ultcbDeadline.Value.ToString()));
      }
      if (ultCBType.Value != null)
      {
        sqlinput[2] = new SqlDBParameter("@Type", SqlDbType.Int, DBConvert.ParseInt(ultCBType.Value.ToString()));
      }
      sqlinput[3] = new SqlDBParameter("@CreateBy", SqlDbType.Int, SharedObject.UserInfo.UserPid);
      sqlinput[4] = new SqlDBParameter("@TransactionPid", SqlDbType.BigInt, this.transactionPid);
      // Output ------
      SqlDBParameter[] sqloutput = new SqlDBParameter[1];
      sqloutput[0] = new SqlDBParameter("@Result", SqlDbType.BigInt, long.MinValue);
      SqlDataBaseAccess.ExecuteStoreProcedure("spPLNConfirmWOLInputDeadline_Edit", sqlinput, sqloutput);
      long result = DBConvert.ParseLong(sqloutput[0].Value.ToString());
      if (result <= 0)
      {
        return false;
      }
      return true;
    }

    private void SaveData()
    {
      if (this.CheckValid())
      {
        bool success = this.SaveMain();
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.flagSearch = true;
          //this.CloseTab();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
        this.LoadData();
      }
    }

    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }
    #endregion Function

    #region Event
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      //e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;

      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        // Set Align column
        Type colType = e.Layout.Bands[0].Columns[i].DataType;
        if (colType == typeof(System.Double) || colType == typeof(System.Int16) || colType == typeof(System.Int32) || colType == typeof(System.Int64))
        {
          e.Layout.Bands[0].Columns[i].CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
        }
      }

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.No;
      e.Layout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;

      e.Layout.Bands[0].Columns["Error"].Hidden = true;
      e.Layout.Bands[0].ColHeaderLines = 2;

      e.Layout.Bands[0].Columns["WorkOrder"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["ItemCode"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Revision"].CellAppearance.BackColor = Color.Yellow;
      e.Layout.Bands[0].Columns["Qty"].CellAppearance.BackColor = Color.Yellow;

      e.Layout.Bands[0].Columns["CARDeadline"].Header.Caption = "CAR\nDeadline";
      e.Layout.Bands[0].Columns["CARRemark"].Header.Caption = "CAR\nRemark";
      e.Layout.Bands[0].Columns["COM1Deadline"].Header.Caption = "COM1\nDeadline";
      e.Layout.Bands[0].Columns["COM1Remark"].Header.Caption = "COM1\nRemark";
      e.Layout.Bands[0].Columns["ASSHWDeadline"].Header.Caption = "ASSHW\nDeadline";
      e.Layout.Bands[0].Columns["FFHWDeadline"].Header.Caption = "FFHW\nDeadline";
      e.Layout.Bands[0].Columns["MATDeadline"].Header.Caption = "MAT\nDeadline";
      e.Layout.Bands[0].Columns["ASSHWRemark"].Header.Caption = "ASSHW\nRemark";
      e.Layout.Bands[0].Columns["FFHWRemark"].Header.Caption = "FFHW\nRemark";
      e.Layout.Bands[0].Columns["MATRemark"].Header.Caption = "MAT\nRemark";
      e.Layout.Bands[0].Columns["SUBDeadline"].Header.Caption = "SUB\nDeadline";
      e.Layout.Bands[0].Columns["SUBRemark"].Header.Caption = "SUB\nRemark";
      e.Layout.Bands[0].Columns["ASSYDeadline"].Header.Caption = "ASSY\nDeadline";
      e.Layout.Bands[0].Columns["ASSYRemark"].Header.Caption = "ASSY\nRemark";
      e.Layout.Bands[0].Columns["SAMDeadline"].Header.Caption = "SAM\nDeadline";
      e.Layout.Bands[0].Columns["SAMRemark"].Header.Caption = "SAM\nRemark";
    }

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      this.SetNeedToSave();
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string colName = e.Cell.Column.ToString();      
      //string value = e.NewValue.ToString();      
      //switch (colName)
      //{
      //  case "CompCode":
      //    WindowUtinity.ShowMessageError("ERR0029", "Comp Code");
      //    e.Cancel = true;          
      //    break;        
      //  default:
      //    break;
      //}
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private void ultData_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
    {
      e.DisplayPromptMsg = false;
      if (FunctionUtility.PromptDeleteMessage(e.Rows.Length) != DialogResult.Yes)
      {
        e.Cancel = true;
        return;
      }
      foreach (UltraGridRow row in e.Rows)
      {
        this.SetNeedToSave();
        long pid = DBConvert.ParseLong(row.Cells["Pid"].Value.ToString());
        if (pid != long.MinValue)
        {
          this.listDeletedPid.Add(pid);
        }
      }
    }

    private void Object_Changed(object sender, EventArgs e)
    {
      this.SetNeedToSave();
    }
    #endregion Event

    private void btnBro_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excels files (*.xls)|*.xls| All files (*.*)|*.*";
      dialog.Title = "Select a Excel file";
      txtImport.Text = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : string.Empty;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      this.ImportData();
    }

    private void ultcbDeadline_ValueChanged(object sender, EventArgs e)
    {
      if (ultcbDeadline.Value != null)
      {
        if (DBConvert.ParseInt(ultcbDeadline.Value.ToString()) == 0)
        {
          ControlUtility.LoadUltraCombo(ultCBType, this.dtDraft, "value", "text", false, "value");
        }
        else
        {
          ControlUtility.LoadUltraCombo(ultCBType, this.dtConfirm, "value", "text", false, "value");
        }
      }
    }

    private void btnGetTemplate_Click(object sender, EventArgs e)
    {
      string templateName = "ConfirmWorkOnlineTemplate";
      string sheetName = "Sheet1";
      string outFileName = "Template Import Deadline For WO Draft";
      string startupPath = System.Windows.Forms.Application.StartupPath;
      string pathOutputFile = string.Format(@"{0}\Report", startupPath);
      string pathTemplate = startupPath + @"\ExcelTemplate";
      XlsReport oXlsReport = FunctionUtility.InitializeXlsReport(templateName, sheetName, outFileName, pathTemplate, pathOutputFile, out outFileName);

      oXlsReport.Out.File(outFileName);
      Process.Start(outFileName);
    }
  }
}
