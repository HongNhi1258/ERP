using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_10_002 : MainUserControl
  {
    #region Field
    string USER_COMPUTER_FORMAT_DATETIME = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
    private IList listDeletedPid = new ArrayList();
    #endregion Field

    #region Init
    public viewBOM_10_002()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Load Data Init
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewBOM_10_002_Load(object sender, EventArgs e)
    {
      this.LoadData();
    }
    #endregion Init

    #region Function
    /// <summary>
    /// Search Information
    /// </summary>
    private void Search()
    {
      btnSearch.Enabled = false;

      DBParameter[] input = new DBParameter[2];
      if (txtMachineName.Text.Length > 0)
      {
        input[0] = new DBParameter("@MachineName", DbType.String, 125, "%" + txtMachineName.Text.Trim() + "%");
      }
      if (ultraCBMachine.Value != null)
      {
        input[1] = new DBParameter("@Machine", DbType.Int64, DBConvert.ParseLong(ultraCBMachine.Value.ToString()));
      }
      DataSet ds = DataBaseAccess.SearchStoreProcedure("spBOMMachineInformation_Search", input);
      ds.Relations.Add(new DataRelation("TblMaster_TblDetail", ds.Tables[0].Columns["Pid"], ds.Tables[1].Columns["DetailPid"], false));
      if (ds != null)
      {
        ultData.DataSource = ds;
      }
      // Enable button search
      btnSearch.Enabled = true;
      this.NeedToSave = false;
    }

    /// <summary>
    /// Load All Data For Search Information
    /// </summary>

    private void LoadData()
    {
      string command = "SELECT EID, CAST(EID AS VARCHAR) +' - ' + EmpName Name FROM TblHRDEmployee";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(command);

      string commandText = string.Format(@"SELECT MC.Pid, MC.MachineCode + ' - ' + MC.MachineNameVN Display
                                          FROM TblBOMMachineInformation MC");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultraCBMachine.DataSource = dtSource;
      ultraCBMachine.DisplayMember = "Display";
      ultraCBMachine.ValueMember = "Pid";
      ultraCBMachine.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBMachine.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      //ultraCBMachine.DisplayLayout.AutoFitColumns = true;

      ultraDDNhanvien.DataSource = dt;
      ultraDDNhanvien.ValueMember = "EID";
      ultraDDNhanvien.DisplayMember = "Name";
      ultraDDNhanvien.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraDDNhanvien.DisplayLayout.Bands[0].Columns["EID"].Hidden = true;
      ultraDDNhanvien.DisplayLayout.Bands[0].Columns["Name"].Width = 200;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == Keys.Enter)
      {
        this.Search();
        return true;
      }
      return base.ProcessCmdKey(ref msg, keyData);
    }
    #endregion Function

    #region Event

    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.Search();
    }

    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      e.Layout.AutoFitColumns = true;
      e.Layout.ScrollBounds = ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = ScrollStyle.Immediate;

      e.Layout.Bands[0].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[1].Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Override.AllowDelete = DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowUpdate = DefaultableBoolean.True;

      e.Layout.Bands[0].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["Pid"].Hidden = true;
      e.Layout.Bands[1].Columns["DetailPid"].Hidden = true;

      e.Layout.Bands[0].Columns["MachineCode"].Header.Caption = "Machine Code";
      e.Layout.Bands[0].Columns["MachineNameEN"].Header.Caption = "Machine Name EN";
      e.Layout.Bands[0].Columns["MachineNameVN"].Header.Caption = "Machine Name VN";

      e.Layout.Bands[1].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[1].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[1].Columns["UpdateBy"].Header.Caption = "Update By";
      e.Layout.Bands[1].Columns["UpdateDate"].Header.Caption = "Update Date";
      e.Layout.Bands[1].Columns["EID"].Header.Caption = "Employee Name";
      e.Layout.Bands[1].Columns["FromDate"].Header.Caption = "From Date";
      e.Layout.Bands[1].Columns["ToDate"].Header.Caption = "To Date";
      e.Layout.Bands[1].Columns["FromHour"].Header.Caption = "From Hour";
      e.Layout.Bands[1].Columns["ToHour"].Header.Caption = "To Hour";

      e.Layout.Bands[0].Columns["MachineCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MachineCode"].MinWidth = 100;
      //e.Layout.Bands[0].Columns["Factory"].MaxWidth = 100;
      //e.Layout.Bands[0].Columns["Factory"].MinWidth = 100;

      e.Layout.Bands[1].Columns["FromDate"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["FromDate"].MinWidth = 100;

      e.Layout.Bands[1].Columns["ToDate"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["ToDate"].MinWidth = 100;

      e.Layout.Bands[1].Columns["FromHour"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["FromHour"].MinWidth = 100;

      e.Layout.Bands[1].Columns["ToHour"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["ToHour"].MinWidth = 100;

      e.Layout.Bands[1].Columns["CreateDate"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["CreateDate"].MinWidth = 100;

      e.Layout.Bands[1].Columns["UpdateDate"].MaxWidth = 100;
      e.Layout.Bands[1].Columns["UpdateDate"].MinWidth = 100;

      e.Layout.Bands[1].Columns["CreateDate"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["CreateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["UpdateBy"].CellActivation = Activation.ActivateOnly;
      e.Layout.Bands[1].Columns["UpdateDate"].CellActivation = Activation.ActivateOnly;

      e.Layout.Bands[1].Columns["EID"].ValueList = ultraDDNhanvien;

      e.Layout.Bands[1].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[1].Columns["UpdateBy"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[1].Columns["UpdateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);

      e.Layout.Override.RowAppearance.BackColorAlpha = Infragistics.Win.Alpha.Transparent;
      e.Layout.Override.RowAlternateAppearance = e.Layout.Override.RowAlternateAppearance;
      e.Layout.Override.CellAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
      e.Layout.Override.HeaderAppearance.BackColorAlpha = Infragistics.Win.Alpha.UseAlphaLevel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      DataTable dtDetail = (DataTable)((DataSet)ultData.DataSource).Tables[1];
      foreach (DataRow row in dtDetail.Rows)
      {
        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          if (DBConvert.ParseInt(row["EID"].ToString().Trim()) == int.MinValue)
          {
            errorMessage = "Data Input Error";
            return false;
          }
        }
      }
      return true;
    }

    private void SaveData()
    {
      string errorMessage;
      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        foreach (long pidChild in this.listDeletedPid)
        {

          DBParameter[] inputParams = new DBParameter[1];
          inputParams[0] = new DBParameter("@Pid", DbType.Int64, pidChild);
          DataBaseAccess.ExecuteStoreProcedure("spBOMMachineInformationDetail_Delete", inputParams, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)((DataSet)ultData.DataSource).Tables[1];
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[8];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            string fromHour = row["FromHour"].ToString();
            string toHour = row["ToHour"].ToString();
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@DetailPid", DbType.Int32, DBConvert.ParseLong(row["DetailPid"].ToString()));
            inputParam[2] = new DBParameter("@EID", DbType.Int32, DBConvert.ParseLong(row["EID"].ToString()));
            string fromDate = DBConvert.ParseString(DBConvert.ParseDateTime(row["FromDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
            if (fromDate.Length > 0)
            {
              inputParam[3] = new DBParameter("@FromDate", DbType.DateTime, DBConvert.ParseDateTime(row["FromDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME));
            }
            string toDate = DBConvert.ParseString(DBConvert.ParseDateTime(row["ToDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME), ConstantClass.FORMAT_DATETIME);
            if (toDate.Length > 0)
            {
              inputParam[4] = new DBParameter("@ToDate", DbType.DateTime, DBConvert.ParseDateTime(row["ToDate"].ToString(), USER_COMPUTER_FORMAT_DATETIME));
            }
            if (fromHour.Length > 0)
            {
              inputParam[5] = new DBParameter("@FromHour", DbType.String, 5, row["FromHour"].ToString().Substring(0, 5));
            }
            if (toHour.Length > 0)
            {
              inputParam[6] = new DBParameter("@ToHour", DbType.String, 5, row["ToHour"].ToString().Substring(0, 5));
            }
            inputParam[7] = new DBParameter("@CreateBy", DbType.Int32, SharedObject.UserInfo.UserPid);

            DataBaseAccess.ExecuteStoreProcedure("spBOMMachineInformationDetail_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.Search();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0001", errorMessage);
        this.SaveSuccess = false;
      }
    }

    private void ultData_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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
      //DataTable dtDetail = (DataTable)((DataSet)ultData.DataSource).Tables[1];
      //foreach (DataRow row in dtDetail.Rows)
      //{
      //    long pid = DBConvert.ParseLong(row["Pid"].ToString());
      //    if (pid != long.MinValue)
      //    {
      //      this.listDeletedPid.Add(pid);
      //    }    
      //}
    }
    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      //string commandText = string.Format(@"SELECT *	FROM	TblCSDOptionSetForEcat WHERE Code = '{0}'", e.Cell.Row.Cells["Code"].Text);
      //DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandText);
      //if (dt != null && dt.Rows.Count > 0)
      //{
      //  string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "Code");
      //  WindowUtinity.ShowMessageErrorFromText(message);
      //  e.Cancel = true;
      //  return;
      //}
    }

    //private bool CheckMemberDownDrop(string inputvalue, string colName, DataTable dt)
    //{
    //  bool check = false;
    //  if (dt != null)
    //  {
    //    for (int i = 0; i < dt.Rows.Count; i++)
    //    {
    //      if (dt.Rows[i][colName].ToString() == inputvalue)
    //      {
    //        check = true;
    //        break;
    //      }
    //    }
    //  }
    //  return check;
    //}

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
    /// Save data before close
    /// </summary>
    public override void SaveAndClose()
    {
      base.SaveAndClose();
      this.SaveData();
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      txtMachineName.Text = string.Empty;
      ultraCBMachine.Text = string.Empty;
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultData, "Data");
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    #endregion Event

  }
}
