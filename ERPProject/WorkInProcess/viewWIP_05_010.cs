/*
  Author      : Nguyen Van Tron
  Date        : 20/04/2013
  Description : Pass By Not Finished Process Of Finished Component
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
  public partial class viewWIP_05_010 : MainUserControl
  {
    #region field
    private bool selectedChange = true;
    private bool chkhSelect = true;
    #endregion field

    #region function
    private void LoadStatus()
    {
      string cm = string.Format(@"SELECT 0 ID, N'Tất cả' Name
                                   UNION
                                  SELECT 1 ID, N'Chưa hoàn thành' Name
                                   UNION
                                  SELECT 2 ID, N'Đã hoàn thành' Name ");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(cm);
      if (dtSource == null)
      {
        return;
      }
      ultraCBStatus.DataSource = dtSource;
      ultraCBStatus.DisplayMember = "Name";
      ultraCBStatus.ValueMember = "ID";
      ultraCBStatus.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultraCBStatus.DisplayLayout.Bands[0].Columns["ID"].Hidden = true;

    }
    private void LoadDataWO()
    {
      string commandText = "SELECT Pid FROM TblPLNWorkOrder WHERE Confirm = 1 ORDER BY Pid DESC";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCBWO, dtSource, "Pid", "Pid");
      ultraCBWO.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadDataCarcass()
    {
      ultraCBCarcass.Value = null;
      string condition = string.Empty;
      if (ultraCBWO.Value != null)
      {
        condition = string.Format("WHERE CAR.Wo = {0}", ultraCBWO.Value);
      }
      string commandText = string.Format(@"SELECT DISTINCT CAR.CarcassCode, CAR.CarcassCode + ' | ' + INF.[Description] Display 
                                           FROM TblPLNWOCarcassInfomation CAR 
                                           INNER JOIN TblBOMCarcass INF ON (CAR.CarcassCode = INF.CarcassCode) {0} ORDER BY CAR.CarcassCode", condition);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      Utility.LoadUltraCombo(ultraCBCarcass, dtSource, "CarcassCode", "Display", "CarcassCode");
      ultraCBCarcass.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    public void LoadDataComponent()
    {
      ultraCBComponent.Value = null;
      if (ultraCBWO.Value != null && ultraCBCarcass.Value != null)
      {
        string commandText = "SELECT DISTINCT ComponentCode, (ComponentCode + ' | ' +  DescriptionVN) DisplayTex FROM TblPLNWOCarcassInfomation WHERE 1 = 1";
        commandText += string.Format(" AND Wo = {0}", ultraCBWO.Value);
        commandText += string.Format(" AND CarcassCode = '{0}'", ultraCBCarcass.Value);
        commandText += " ORDER BY ComponentCode";
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText, 600);
        Utility.LoadUltraCombo(ultraCBComponent, dtSource, "ComponentCode", "DisplayTex", "ComponentCode");
        ultraCBComponent.DisplayLayout.Bands[0].ColHeadersVisible = false;
      }
    }

    private void LoadDataProcess()
    {
      string commandText;
      if (ultraCBWO.Value != null && ultraCBCarcass.Value != null && ultraCBComponent.Value != null)
      {
        commandText = string.Format(@" SELECT PRC.Pid ProcessPid, (ProcessCode + ' | ' + ENDescription) Display 
                                      FROM TblBOMProcessInfo PRC
	                                      INNER JOIN TblPLNWOProcessInformation INF ON PRC.Pid = INF.ProcessPid
	                                      INNER JOIN TblPLNWOCarcassInfomation CAR ON INF.ComponentPid = CAR.Pid
                                      WHERE CAR.Wo= {0} AND CAR.CarcassCode = '{1}' AND CAR.ComponentCode = '{2}'
                                      ORDER BY ProcessCode", ultraCBWO.Value, ultraCBCarcass.Value, ultraCBComponent.Value);
      }
      else
      {
        commandText = string.Format(@" SELECT Pid ProcessPid, (ProcessCode + ' | ' + ENDescription) Display 
                                       FROM TblBOMProcessInfo 
                                       ORDER BY ProcessCode");
      }
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText, 600);
      Utility.LoadUltraCombo(ultraCBProcess, dtSource, "ProcessPid", "Display", "ProcessPid");
      ultraCBProcess.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private void LoadDataSection()
    {
      ultraCBProcess.Value = null;
      string commandText = "SELECT Code, Value FROM TblBOMCodeMaster WHERE([Group] = 3008) AND Code > 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText, 600);
      Utility.LoadUltraCombo(ultraCBSection, dtSource, "Code", "Value", false, "Code");
    }

    /// <summary>
    /// Set data for controls
    /// </summary>
    private void InitData()
    {
      this.LoadDataWO();
      this.LoadDataCarcass();
      this.LoadDataComponent();
      this.LoadDataProcess();
      this.LoadStatus();
      this.LoadDataSection();
    }

    /// <summary>
    /// Get information from database
    /// </summary>
    private void SearchData()
    {
      btnSearch.Enabled = false;
      this.selectedChange = false;
      chkSelectAll.Checked = false;
      this.selectedChange = true;

      DBParameter[] inputParam = new DBParameter[6];
      if (ultraCBWO.Value != null)
      {
        inputParam[0] = new DBParameter("@Wo", DbType.Int64, ultraCBWO.Value);
      }
      if (ultraCBCarcass.Value != null)
      {
        inputParam[1] = new DBParameter("@CarcassCode", DbType.AnsiString, 16, ultraCBCarcass.Value);
      }
      if (ultraCBComponent.Value != null)
      {
        inputParam[2] = new DBParameter("@CompCode", DbType.AnsiString, 32, ultraCBComponent.Value);
      }
      if (ultraCBProcess.Value != null)
      {
        inputParam[3] = new DBParameter("@ProcessPid", DbType.Int64, ultraCBProcess.Value);
      }
      if (ultraCBStatus.Value != null)
      {
        inputParam[4] = new DBParameter("@Status", DbType.Int32, ultraCBStatus.Value);
      }
      if (ultraCBSection.Value != null)
      {
        inputParam[5] = new DBParameter("@Section", DbType.Int32, ultraCBSection.Value);
      }
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spWIPWOCarcassCompProcessPassBy", 600, inputParam);
      ultraGridInformation.DataSource = dtSource;
      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["IsAllowPassBy"].Value.ToString()) == 1)
        {
          ultraGridInformation.Rows[i].Cells["PassBy"].Activation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
        }
        else
        {
          ultraGridInformation.Rows[i].Cells["PassBy"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
          ultraGridInformation.Rows[i].Cells["PassBy"].Appearance.BackColor = Color.LightGray;
        }
      }
      lbCount.Text = string.Format("Count: {0}", (dtSource != null ? dtSource.Rows.Count : 0));
      btnSearch.Enabled = true;
    }
    /// <summary>
    /// Clear all data of search fields
    /// </summary>
    private void ClearCondition()
    {
      ultraCBWO.Value = null;
      ultraCBCarcass.Value = null;
      ultraCBComponent.Value = null;
      ultraCBProcess.Value = null;
    }

    /// <summary>
    /// Pass By Not Finished Process Of Finished Component
    /// </summary>
    private void PassBy()
    {
      bool success = true;
      long processPid;
      int approvePassBy = Shared.Utility.SharedObject.UserInfo.UserPid;

      for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
      {
        if (DBConvert.ParseInt(ultraGridInformation.Rows[i].Cells["PassBy"].Value.ToString()) == 1)
        {
          processPid = DBConvert.ParseLong(ultraGridInformation.Rows[i].Cells["CompProcessPid"].Value.ToString());
          DBParameter[] inputParam = new DBParameter[2];
          inputParam[0] = new DBParameter("@ProcessPid", DbType.Int64, processPid);
          inputParam[1] = new DBParameter("@ApprovePassBy", DbType.Int32, approvePassBy);

          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DataBaseAccess.ExecuteStoreProcedure("spWIPPassByNotFinishedProcessOfFinishedComp", inputParam, outputParam);
          if (string.Compare(outputParam[0].Value.ToString(), "0") == 0)
          {
            success = false;
          }
        }
      }
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0001");
      }
      else
      {
        WindowUtinity.ShowMessageError("WRN0004");
      }
    }
    #endregion function

    #region event
    public viewWIP_05_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load data for search fields
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewWIP_05_010_Load(object sender, EventArgs e)
    {
      // Add KeyDown even for all controls in groupBoxSearch
      foreach (Control ctr in tableLayoutSearch.Controls)
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

    private void ultraGridInformation_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
    {
      string column = e.Cell.Column.ToString();
      if (string.Compare("PassBy", column, true) == 0)
      {
        int select = DBConvert.ParseInt(e.Cell.Text);
        if (select == 0)
        {
          this.chkhSelect = false;
          chkSelectAll.Checked = false;
          this.chkhSelect = true;
        }
      }
    }
    private void ultraGridInformation_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Override.AutoResizeColumnWidthOptions = Infragistics.Win.UltraWinGrid.AutoResizeColumnWidthOptions.All;
      e.Layout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
      e.Layout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
      e.Layout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
      e.Layout.Bands[0].Columns["PassBy"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["ProcessPid"].Hidden = true;
      e.Layout.Bands[0].Columns["CompProcessPid"].Hidden = true;
      e.Layout.Bands[0].Columns["IsAllowPassBy"].Hidden = true;

      e.Layout.Bands[0].Columns["CarcassCode"].Header.Caption = "Carcass Code";
      e.Layout.Bands[0].Columns["CarcassName"].Header.Caption = "Carcass Name";
      e.Layout.Bands[0].Columns["ComponentCode"].Header.Caption = "Component Code";
      e.Layout.Bands[0].Columns["ComponentName"].Header.Caption = "Component Name";
      e.Layout.Bands[0].Columns["ProcessCode"].Header.Caption = "Process Code";
      e.Layout.Bands[0].Columns["ProcessName"].Header.Caption = "Process Name";
      e.Layout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
      e.Layout.Bands[0].Columns["TotalQty"].Header.Caption = "Total Qty";
      e.Layout.Bands[0].Columns["WorkAreaDetail"].Header.Caption = "Stock";

      e.Layout.Bands[0].Columns["Section"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Section"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Wo"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Wo"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["PassBy"].MinWidth = 60;
      e.Layout.Bands[0].Columns["PassBy"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["CarcassCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["CarcassCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["TotalQty"].MinWidth = 60;
      e.Layout.Bands[0].Columns["TotalQty"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Ordinal"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Ordinal"].MaxWidth = 50;
      //e.Layout.Bands[0].Columns["CarcassName"].MinWidth = 300;
      //e.Layout.Bands[0].Columns["CarcassName"].MaxWidth = 300;
      e.Layout.Bands[0].Columns["ComponentCode"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ComponentCode"].MaxWidth = 100;
      //e.Layout.Bands[0].Columns["ProcessName"].MinWidth = 170;
      //e.Layout.Bands[0].Columns["ProcessName"].MaxWidth = 170;
      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
      for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
      {
        e.Layout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
      e.Layout.Bands[0].Columns["PassBy"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.AllowEdit;
    }

    private void ultraCBWO_ValueChanged(object sender, EventArgs e)
    {
      this.LoadDataCarcass();
    }

    private void ultraCBCarcass_ValueChanged(object sender, EventArgs e)
    {
      this.LoadDataComponent();
    }

    private void ultraCBComponent_ValueChanged(object sender, EventArgs e)
    {
      this.LoadDataProcess();
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.chkhSelect)
      {
        int checkAll = (chkSelectAll.Checked ? 1 : 0);
        for (int i = 0; i < ultraGridInformation.Rows.Count; i++)
        {
          if (ultraGridInformation.Rows[i].IsFilteredOut == false && ultraGridInformation.Rows[i].Cells["PassBy"].Activation != Infragistics.Win.UltraWinGrid.Activation.ActivateOnly)
          {
            ultraGridInformation.Rows[i].Cells["PassBy"].Value = checkAll;
          }
        }
      }
      //if (this.selectedChange)
      //{
      //  int selected = (chkSelectAll.Checked ? 1 : 0);
      //  DataTable dtSource = (DataTable)ultraGridInformation.DataSource;
      //  foreach (DataRow row in dtSource.Rows)
      //  {
      //    row["PassBy"] = selected;
      //  }
      //}
    }

    private void ultraGridInformation_AfterCellActivate(object sender, EventArgs e)
    {
      if (this.selectedChange && string.Compare(ultraGridInformation.ActiveCell.Column.ToString(), "Select", true) == 0)
      {
        this.selectedChange = false;
        chkSelectAll.Checked = false;
        this.selectedChange = true;
      }
    }

    private void btnPassBy_Click(object sender, EventArgs e)
    {
      this.PassBy();
      this.SearchData();
    }

    private void ultraChkShowCarcassImage_CheckedChanged(object sender, EventArgs e)
    {
      grpItemPicture.Visible = ultraChkShowCarcassImage.Checked;
      Utility.BOMShowCarcassImage(ultraGridInformation, grpItemPicture, picItem, ultraChkShowCarcassImage.Checked);
    }
    #endregion event

    private void ultraGridInformation_MouseClick(object sender, MouseEventArgs e)
    {
      Utility.BOMShowCarcassImage(ultraGridInformation, grpItemPicture, picItem, ultraChkShowCarcassImage.Checked);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (ultraGridInformation.Rows.Count > 0)
      {
        Utility.ExportToExcelWithDefaultPath(ultraGridInformation, "Data");
      }
    }
  }
}
