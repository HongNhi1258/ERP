/*
  Author      :  Huynh Thi Bang
  Date        :  12/10/2018
  Description :  Define information machine
*/
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewBOM_10_001 : MainUserControl
  {
    #region field
    private IList listDeletedPid = new ArrayList();
    private bool isDuplicateProcess = false;
    private int factory = int.MinValue;
    #endregion field

    #region function
    public viewBOM_10_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewBOM_10_001_Load(object sender, EventArgs e)
    {
      //Init Data
      this.InitData();

      this.LoadFactory();
      this.LoadSection();
      this.LoadTeam();
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      string commandText = string.Format(@" SELECT MC.Pid, MC.MachineCode, MC.MachineNameVN, MC.MachineNameEN, MC.Factory, MC.Section, MC.Team,
                                                 EM.EmpName CreateBy, MC.CreateDate, NV.EmpName UpdateBy, MC.UpdateDate
                                            FROM TblBOMMachineInformation MC
	                                            INNER JOIN TblHRDEmployee EM ON MC.CreateBy = EM.EID
	                                            LEFT JOIN TblHRDEmployee NV ON MC.UpdateBy = NV.EID
	                                            INNER JOIN TblWIPWorkArea ARE ON MC.Team = ARE.WorkAreaCode
                                          ORDER BY MC.MachineCode");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultraGridInformation.DataSource = dtSource;
    }
    /// <summary>
    /// Load Factory
    /// </summary>
    private void LoadFactory()
    {
      string cmdText = string.Empty;
      cmdText = string.Format(@"SELECT Code, [Description] Factory
                                FROM TblBOMCodeMaster 
                                WHERE [Group] = 37 ");

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(cmdText);
      if (dtSource == null)
      {
        return;
      }
      ultraDDFactory.DataSource = dtSource;
      ultraDDFactory.DisplayMember = "Factory";
      ultraDDFactory.ValueMember = "Code";
      ultraDDFactory.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultraDDFactory.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultraDDFactory.DisplayLayout.Bands[0].Columns["Factory"].Width = 200;
    }
    /// <summary>
    /// Load Section
    /// </summary>
    /// <param name="factory"></param>
    private void LoadSection()
    {
      string commandText = string.Empty;
      commandText = string.Format(@" SELECT CM1.Code, CM1.[Description] +' - '+ CM2.[Description] Section
                                    FROM
                                    (
	                                    SELECT CM1.Code, CM1.[Description], CM1.MoreDescription
	                                    FROM TblBOMCodeMaster CM1
	                                    WHERE CM1.[Group] = 38
                                    )CM1
                                    LEFT JOIN
                                    (
	                                    SELECT CM2.Value, CM2.[Description]
	                                    FROM TblBOMCodeMaster CM2
	                                    WHERE CM2.[Group] = 37
                                    )CM2 ON CM1.MoreDescription = CM2.Value");

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultraDDSection.DataSource = dtSource;
      ultraDDSection.DisplayMember = "Section";
      ultraDDSection.ValueMember = "Code";
      ultraDDSection.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultraDDSection.DisplayLayout.Bands[0].Columns["Code"].Hidden = true;
      ultraDDSection.DisplayLayout.Bands[0].Columns["Section"].Width = 300;
    }
    /// <summary>
    /// Load Team
    /// </summary>
    /// <param name="factory"></param>
    private void LoadTeam()
    {
      string commandText = string.Empty;
      commandText = string.Format(@" SELECT WorkAreaCode, WorkAreaName Team 
                                    FROM TblWIPWorkArea 
                                    WHERE DevisionCode = 'COM'");

      System.Data.DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtSource == null)
      {
        return;
      }
      ultraDDTeam.DataSource = dtSource;
      ultraDDTeam.DisplayMember = "Team";
      ultraDDTeam.ValueMember = "WorkAreaCode";
      ultraDDTeam.DisplayLayout.Bands[0].ColHeadersVisible = true;
      ultraDDTeam.DisplayLayout.Bands[0].Columns["WorkAreaCode"].Hidden = true;
      ultraDDTeam.DisplayLayout.Bands[0].Columns["Team"].Width = 200;
    }
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

    private bool CheckValid(out string errorMessage)
    {
      errorMessage = string.Empty;
      DataTable dt = (DataTable)ultraGridInformation.DataSource;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        UltraGridRow row = ultraGridInformation.Rows[i];
        string machine = row.Cells["MachineCode"].Value.ToString();
        string machineNameEN = row.Cells["MachineNameEN"].Value.ToString();
        string machineNameVN = row.Cells["MachineNameVN"].Value.ToString();
        string factory = row.Cells["Factory"].Value.ToString();
        string section = row.Cells["Section"].Value.ToString();
        string team = row.Cells["Team"].Value.ToString();
        if (machine.Length == 0)
        {
          errorMessage = "MachineCode";
          return false;
        }
        if (machineNameEN.Length == 0)
        {
          errorMessage = "MachineNameEN";
          return false;
        }
        if (machineNameVN.Length == 0)
        {
          errorMessage = "MachineNameVN";
          return false;
        }
        if (factory.Length == 0)
        {
          errorMessage = "Factory";
          return false;
        }
        if (section.Length == 0)
        {
          errorMessage = "Section";
          return false;
        }
        if (team.Length == 0)
        {
          errorMessage = "Team";
          return false;
        }

      }
      return true;
    }
    /// <summary>
    /// Check Double
    /// </summary>
    private void CheckDuplicate()
    {
      isDuplicateProcess = false;
      for (int k = 0; k < ultraGridInformation.Rows.Count; k++)
      {
        ultraGridInformation.Rows[k].CellAppearance.BackColor = Color.Empty;
      }
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        string component = ultraGridInformation.Rows[i].Cells["MachineCode"].Value.ToString();

        for (int j = i + 1; j < ultraGridInformation.Rows.Count; j++)
        {
          string componentCompare = ultraGridInformation.Rows[j].Cells["MachineCode"].Value.ToString();
          if (component == componentCompare)
          {
            ultraGridInformation.Rows[i].CellAppearance.BackColor = Color.Yellow;
            ultraGridInformation.Rows[j].CellAppearance.BackColor = Color.Yellow;
            this.isDuplicateProcess = true;
          }
        }
      }
    }
    private void SaveData()
    {
      string errorMessage;

      if (this.isDuplicateProcess == true)
      {
        WindowUtinity.ShowMessageError("ERR0013");
        return;
      }

      if (this.CheckValid(out errorMessage))
      {
        bool success = true;
        // 1. Delete      
        DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
        for (int i = 0; i < listDeletedPid.Count; i++)
        {
          DBParameter[] deleteParam = new DBParameter[] { new DBParameter("@Pid", DbType.Int64, listDeletedPid[i]) };
          DataBaseAccess.ExecuteStoreProcedure("spBOMMachineInformation_Delete", deleteParam, outputParam);
          if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
          {
            success = false;
          }
        }
        // 2. Insert/Update      
        DataTable dtDetail = (DataTable)ultraGridInformation.DataSource;
        foreach (DataRow row in dtDetail.Rows)
        {
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            DBParameter[] inputParam = new DBParameter[8];
            long pid = DBConvert.ParseLong(row["Pid"].ToString());
            if (row.RowState == DataRowState.Modified) // Update
            {
              inputParam[0] = new DBParameter("@Pid", DbType.Int64, pid);
            }
            inputParam[1] = new DBParameter("@MachineCode", DbType.AnsiString, 16, row["MachineCode"].ToString());
            inputParam[2] = new DBParameter("@MachineNameEN", DbType.String, 128, row["MachineNameEN"].ToString());
            inputParam[3] = new DBParameter("@MachineNameVN", DbType.String, 128, row["MachineNameVN"].ToString());
            inputParam[4] = new DBParameter("@Factory", DbType.Int32, DBConvert.ParseInt(row["Factory"].ToString()));
            inputParam[5] = new DBParameter("@Section", DbType.Int32, DBConvert.ParseInt(row["Section"].ToString()));
            inputParam[6] = new DBParameter("@Team", DbType.String, 125, row["Team"].ToString());
            inputParam[7] = new DBParameter("@EditBy", DbType.Int32, SharedObject.UserInfo.UserPid);

            DataBaseAccess.ExecuteStoreProcedure("spBOMMachineInformation_Edit", inputParam, outputParam);
            if ((outputParam == null) || (outputParam[0].Value.ToString() == "0"))
            {
              success = false;
            }
          }
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.InitData();
          this.NeedToSave = false;
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
    #endregion function

    #region event

    private void btnSave_Click(object sender, EventArgs e)
    {
      this.SaveData();
    }
    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.AutoFitColumns = true;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;


      // Allow update, delete, add new
      e.Layout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.True;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      // Hide column
      e.Layout.Bands[0].Columns["Pid"].Hidden = true;

      // Read only
      e.Layout.Bands[0].Columns["CreateBy"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["CreateDate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateBy"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      e.Layout.Bands[0].Columns["UpdateDate"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;

      e.Layout.Bands[0].Columns["Factory"].ValueList = ultraDDFactory;
      e.Layout.Bands[0].Columns["Section"].ValueList = ultraDDSection;
      e.Layout.Bands[0].Columns["Team"].ValueList = ultraDDTeam;

      e.Layout.Bands[0].Columns["MachineCode"].Header.Caption = "Machine Code";
      e.Layout.Bands[0].Columns["MachineNameEN"].Header.Caption = "Machine Name EN";
      e.Layout.Bands[0].Columns["MachineNameVN"].Header.Caption = "Machine Name VN";
      e.Layout.Bands[0].Columns["CreateBy"].Header.Caption = "Create By";
      e.Layout.Bands[0].Columns["CreateDate"].Header.Caption = "Create Date";
      e.Layout.Bands[0].Columns["UpdateBy"].Header.Caption = "Update By";
      e.Layout.Bands[0].Columns["UpdateDate"].Header.Caption = "Update Date";

      e.Layout.Bands[0].Columns["MachineCode"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["MachineCode"].MinWidth = 100;

      e.Layout.Bands[0].Columns["CreateDate"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["CreateDate"].MinWidth = 100;

      e.Layout.Bands[0].Columns["UpdateDate"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["UpdateDate"].MinWidth = 100;

      e.Layout.Bands[0].Columns["Team"].MaxWidth = 130;
      e.Layout.Bands[0].Columns["Team"].MinWidth = 130;

      e.Layout.Bands[0].Columns["Factory"].MaxWidth = 180;
      e.Layout.Bands[0].Columns["Factory"].MinWidth = 180;

      //e.Layout.Bands[0].Columns["Factory"].MaxWidth = 60;
      //e.Layout.Bands[0].Columns["Factory"].MinWidth = 60;

      // Format date (dd/MM/yy)
      e.Layout.Bands[0].Columns["CreateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["CreateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
      e.Layout.Bands[0].Columns["UpdateDate"].Format = ConstantClass.FORMAT_DATETIME;
      e.Layout.Bands[0].Columns["UpdateDate"].FormatInfo = new System.Globalization.CultureInfo("vi-VN", true);
    }

    private void ultraGridInformation_BeforeRowsDeleted(object sender, Infragistics.Win.UltraWinGrid.BeforeRowsDeletedEventArgs e)
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
    private void ultraGridInformation_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      //string columnName = e.Cell.Column.ToString();
      //UltraGridRow row = e.Cell.Row;
      //switch (columnName)
      //{
      //  case "Section":
      //    if (DBConvert.ParseInt(row.Cells["Factory"].Value.ToString()) != Int32.MinValue)
      //    {
      //      this.LoadSection(DBConvert.ParseInt(row.Cells["Factory"].Value.ToString()));
      //    }
      //    else
      //    {
      //      this.LoadSection(-1);
      //    }
      //    break;
      //  default:
      //    break;
      //}
    }
    private void ultraGridInformation_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().ToLower();
      int count = 0;
      if (colName == "machinecode")
      {
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (e.Cell.Row.Cells["MachineCode"].Text == ultraGridInformation.Rows[i].Cells["MachineCode"].Text)
          {
            count++;
            if (count == 2)
            {
              string message = string.Format(FunctionUtility.GetMessage("ERR0006"), "MachineCode");
              WindowUtinity.ShowMessageErrorFromText(message);
              e.Cancel = true;
              break;
            }
          }
        }
      }
    }

    private void ultraGridInformation_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString();
      UltraGridRow row = e.Cell.Row;
      switch (columnName)
      {
        case "Factory":
          row.Cells["Section"].Value = DBNull.Value;
          break;
        default:
          break;
      }
      this.SetNeedToSave();
    }
    private void btnExport_Click(object sender, EventArgs e)
    {
      Utility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
    }
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.ConfirmToCloseTab();
    }

    #endregion event
  }
}
