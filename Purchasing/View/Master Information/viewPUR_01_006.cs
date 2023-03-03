/*
  Author      : Vo Van Duy Qui
  Date        : 25-03-2011
  Description : List Material Information
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using System;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_01_006 : MainUserControl
  {
    #region Field
    private bool loadData = false;

    private string SP_GNR_MATERIALINFO_SELECT = "spGNRMaterialInformation_Select";
    private string SP_GNR_MATERIALINFO_CONFIRM = "spGNRMaterialInformation_Confirm";
    private string SP_GNR_MATERIALINFO_DELETE = "spGNRMaterialInformation_Delete";
    #endregion Field

    #region Init

    /// <summary>
    /// Init Form
    /// </summary>
    public viewPUR_01_006()
    {
      InitializeComponent();
    }

    /// <summary>
    /// From Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_006_Load(object sender, EventArgs e)
    {
      this.LoadComboMaterialGroup();
      this.LoadComboGroupInCharge();
      this.LoadComboDepartment();

      this.loadData = true;
    }
    #endregion Init

    #region Load Data

    private bool CheckGroupInCharge(int userPid, string materialCode)
    {
      string commmadText = string.Format(@" SELECT MAT.MaterialCode
                                            FROM TblGNRMaterialInformation MAT
	                                            INNER JOIN
	                                            (
		                                            SELECT SG.Pid GroupInCharge
		                                            FROM TblPURStaffGroup SG
			                                            LEFT JOIN TblPURStaffGroupDetail SGD ON SG.Pid = SGD.[Group]
		                                            WHERE SG.LeaderGroup = {0} OR SGD.Employee = {1}
	                                            )GR ON GR.GroupInCharge = MAT.GroupIncharge
                                            WHERE MAT.MaterialCode = '{2}'", userPid, userPid, materialCode);
      DataTable dtGroupInCharge = DataBaseAccess.SearchCommandTextDataTable(commmadText);
      if (dtGroupInCharge != null && dtGroupInCharge.Rows.Count > 0)
      {
        return true;
      }
      return false;
    }
    /// <summary>
    /// Check Purchase Manager
    /// </summary>
    /// <param name="userPid"></param>
    /// <returns></returns>
    private bool CheckPurchaseManager(int userPid)
    {
      string commandText = string.Format("SELECT Manager FROM VHRDDepartmentInfo WHERE Code = 'PUR' AND Manager = {0}", userPid);
      DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(commandText);
      if (dtCheck != null && dtCheck.Rows.Count > 0)
      {
        return true;
      }
      return false;
    }

    private bool CheckUserLogin(out string message)
    {
      message = string.Empty;
      bool success = this.CheckPurchaseManager(SharedObject.UserInfo.UserPid);
      if (!success)
      {
        if (this.loadData)
        {
          DataTable dtSource = (DataTable)ultData.DataSource;
          DataRow[] rowsDisactive = dtSource.Select("Selected = 1");
          if (rowsDisactive.Length > 0)
          {
            foreach (DataRow row in rowsDisactive)
            {
              success = this.CheckGroupInCharge(SharedObject.UserInfo.UserPid, row["MaterialCode"].ToString());
              if (success == false)
              {
                message = "Discontinue";
                return false;
              }
            }
          }
        }
      }
      else
      {
        return true;
      }
      return true;
    }
    /// <summary>
    /// Search Material Information
    /// </summary>
    private void Search()
    {
      string materialCode = txtMaterialCode.Text.Trim();
      string materialName = txtMaterialName.Text.Trim();
      string materialGroup = ControlUtility.GetSelectedValueUltraCombobox(ultComboMaterialGroup);
      if (materialGroup.Length == 0)
      {
        materialGroup = ultComboMaterialGroup.Text.Trim();
      }
      string materialCategory = ControlUtility.GetSelectedValueUltraCombobox(ultComboMaterialCategory);
      if (materialCategory.Length == 0)
      {
        materialCategory = ultComboMaterialCategory.Text.Trim();
      }
      long groupInCharge = DBConvert.ParseLong(ControlUtility.GetSelectedValueUltraCombobox(ultComboGroupInCharge));
      if (groupInCharge == long.MinValue && ultComboGroupInCharge.Text.Trim().Length > 0)
      {
        string commandTezt = string.Format("SELECT Pid FROM TblPURStaffGroup WHERE DeleteFlg = 0 AND GroupName = {0}", ultComboGroupInCharge.Text.Trim());
        object obj = DataBaseAccess.ExecuteScalarCommandText(commandTezt);
        if (obj == null)
        {
          groupInCharge = -1;
        }
      }
      string deptInCharge = ControlUtility.GetSelectedValueUltraCombobox(ultComboDeptInCharge);
      if (deptInCharge.Length == 0)
      {
        deptInCharge = ultComboDeptInCharge.Text.Trim();
      }
      int status = cmbStatus.SelectedIndex - 1;

      DBParameter[] input = new DBParameter[9];
      if (materialCode.Length > 0)
      {
        input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
      }
      if (materialName.Length > 0)
      {
        input[1] = new DBParameter("@MaterialName", DbType.String, 256, materialName);
      }
      if (materialGroup.Length > 0)
      {
        input[2] = new DBParameter("@MaterialGroup", DbType.AnsiString, 10, materialGroup);
      }

      if (materialCategory.Length > 0)
      {
        input[3] = new DBParameter("@MaterialCategory", DbType.AnsiString, 10, materialCategory);
      }
      if (groupInCharge != long.MinValue)
      {
        input[4] = new DBParameter("@GroupInCharge", DbType.Int64, groupInCharge);
      }
      if (deptInCharge.Length > 0)
      {
        input[5] = new DBParameter("@DeptInCharge", DbType.AnsiString, 50, deptInCharge);
      }
      if (status >= 0)
      {
        input[6] = new DBParameter("@Status", DbType.Int32, status);
      }

      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable(SP_GNR_MATERIALINFO_SELECT, input);
      ultData.DataSource = dtSource;
    }

    /// <summary>
    /// Load UltraCombo Material Group
    /// </summary>
    private void LoadComboMaterialGroup()
    {
      string commandText = "SELECT [Group], [Group] + ' - ' + Name AS Name FROM TblGNRMaterialGroup";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      ultComboMaterialGroup.DataSource = dtSource;
      ultComboMaterialGroup.DisplayMember = "Name";
      ultComboMaterialGroup.ValueMember = "Group";
      ultComboMaterialGroup.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboMaterialGroup.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
    }

    /// <summary>
    /// Load UltraCombo Material Category
    /// </summary>
    /// <param name="group"></param>
    private void LoadComboMaterialCategory(string group)
    {
      string commandText = string.Format("SELECT Category, Category + ' - ' + Name AS Name FROM TblGNRMaterialCategory WHERE [Group] = '{0}'", group);
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);
      ultComboMaterialCategory.DataSource = dtSource;
      ultComboMaterialCategory.DisplayMember = "Name";
      ultComboMaterialCategory.ValueMember = "Category";
      ultComboMaterialCategory.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboMaterialCategory.DisplayLayout.Bands[0].Columns["Name"].Width = 250;
    }

    /// <summary>
    /// Load UltraCombo Group Incharge
    /// </summary>
    private void LoadComboGroupInCharge()
    {
      string commandText = "SELECT Pid, GroupName FROM TblPURStaffGroup WHERE DeleteFlg = 0";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);
      ultComboGroupInCharge.DataSource = dtSource;
      ultComboGroupInCharge.ValueMember = "Pid";
      ultComboGroupInCharge.DisplayMember = "GroupName";
      ultComboGroupInCharge.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboGroupInCharge.DisplayLayout.Bands[0].Columns["GroupName"].Width = 300;
      ultComboGroupInCharge.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
    }

    /// <summary>
    /// Load UltraCombo Department
    /// </summary>
    private void LoadComboDepartment()
    {
      string commandText = string.Format("SELECT Department, Department + ' - ' + DeparmentName AS DeparmentName FROM VHRDDepartment WHERE IsNew = 1 ORDER BY Department");
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);

      DataRow newRow = dtSource.NewRow();
      dtSource.Rows.InsertAt(newRow, 0);
      ultComboDeptInCharge.DataSource = dtSource;
      ultComboDeptInCharge.ValueMember = "Department";
      ultComboDeptInCharge.DisplayMember = "DeparmentName";
      ultComboDeptInCharge.DisplayLayout.Bands[0].ColHeadersVisible = false;
      ultComboDeptInCharge.DisplayLayout.Bands[0].Columns["DeparmentName"].MinWidth = 200;
      ultComboDeptInCharge.DisplayLayout.Bands[0].Columns["DeparmentName"].MaxWidth = 200;
    }

    #endregion Load Data

    #region Event

    /// <summary>
    /// ultData Mouse Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultData.Selected.Rows.Count > 0 && btnNew.Visible == true)
      {
        viewPUR_01_003 uc = new viewPUR_01_003();
        uc.materialCode = ultData.Selected.Rows[0].Cells["MaterialCode"].Value.ToString().Trim();
        Shared.Utility.WindowUtinity.ShowView(uc, "MATERIAL INFOMATION", false, DaiCo.Shared.Utility.ViewState.MainWindow);
        this.Search();
      }
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        btnSearch.Enabled = false;
        this.Search();
        btnSearch.Enabled = true;
      }
    }

    /// <summary>
    /// btnSearch Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      btnSearch.Enabled = false;
      this.Search();
      btnSearch.Enabled = true;
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["Min-Max"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialName"].Header.Caption = "Material Name";
      e.Layout.Bands[0].Columns["GroupName"].Header.Caption = "Group In Charge";
      e.Layout.Bands[0].Columns["DeparmentName"].Header.Caption = "Deparment In Charge";
      e.Layout.Bands[0].Columns["OldMaterialCode"].Header.Caption = "Old Material Code";
      e.Layout.Bands[0].Columns["MStatus"].Hidden = true;
      e.Layout.Bands[0].Columns["Group"].Hidden = true;
      e.Layout.Bands[0].Columns["Category"].Hidden = true;

      e.Layout.Bands[0].Columns["Group"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Group"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MinWidth = 50;
      e.Layout.Bands[0].Columns["Unit"].MaxWidth = 50;
      e.Layout.Bands[0].Columns["Status"].MinWidth = 170;
      e.Layout.Bands[0].Columns["Status"].MaxWidth = 170;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 65;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 65;
      e.Layout.Bands[0].Columns["Min-Max"].MinWidth = 65;
      e.Layout.Bands[0].Columns["Min-Max"].MaxWidth = 65;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 90;
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 90;
      e.Layout.Bands[0].Columns["Category"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Category"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["GroupName"].MinWidth = 160;
      e.Layout.Bands[0].Columns["GroupName"].MaxWidth = 160;
      e.Layout.Bands[0].Columns["DeparmentName"].MinWidth = 160;
      e.Layout.Bands[0].Columns["DeparmentName"].MaxWidth = 160;

      for (int i = 0; i < ultData.DisplayLayout.Bands[0].Columns.Count - 1; i++)
      {
        ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
      }
    }

    /// <summary>
    /// btnClose Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// ultComboMaterialGroup Value Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboMaterialGroup_ValueChanged(object sender, EventArgs e)
    {
      if (ultComboMaterialGroup.SelectedRow != null && this.loadData)
      {
        string group = ultComboMaterialGroup.SelectedRow.Cells["Group"].Value.ToString().Trim();
        this.LoadComboMaterialCategory(group);
      }
    }

    /// <summary>
    /// btnNew Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNew_Click(object sender, EventArgs e)
    {
      viewPUR_01_003 view = new viewPUR_01_003();
      Shared.Utility.WindowUtinity.ShowView(view, "MATERIAL INFORMATION", false, ViewState.MainWindow);
      //this.Search();
    }

    /// <summary>
    /// btnConfirm Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnConfirm_Click(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        DataTable dtSource = (DataTable)ultData.DataSource;
        if (dtSource != null && dtSource.Rows.Count > 0)
        {
          DataRow[] rowsConfirm = dtSource.Select("Selected = 1");
          if (rowsConfirm.Length > 0)
          {
            foreach (DataRow rowcheck in rowsConfirm)
            {
              int status = DBConvert.ParseInt(rowcheck["MStatus"].ToString().Trim());
              if (status >= 1)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0079", "confirm");
                return;
              }
            }

            DialogResult dlgr = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0030", "Confirm");
            if (dlgr == DialogResult.Yes)
            {
              foreach (DataRow row in rowsConfirm)
              {
                string materialCode = row["MaterialCode"].ToString().Trim();
                int status = DBConvert.ParseInt(row["MStatus"].ToString().Trim());

                DBParameter[] input = new DBParameter[2];
                input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
                input[1] = new DBParameter("@Status", DbType.Int32, 1);
                DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

                DataBaseAccess.ExecuteStoreProcedure(SP_GNR_MATERIALINFO_CONFIRM, input, output);
                int result = DBConvert.ParseInt(output[0].Value.ToString());
                if (result == 0)
                {
                  Shared.Utility.WindowUtinity.ShowMessageError("ERR0034");
                  return;
                }
              }
              Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0012");
            }
            else if (dlgr == DialogResult.No)
            {
              return;
            }
          }
          // Search Data
          this.Search();
        }
      }
    }

    /// <summary>
    /// btnDelete Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      //Check User
      string message = string.Empty;
      bool success = this.CheckUserLogin(out message);
      if (success == false)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0204", message);
        return;
      }

      if (this.loadData)
      {
        DataTable dtSource = (DataTable)ultData.DataSource;
        DataRow[] rowsDelete = dtSource.Select("Selected = 1");
        if (rowsDelete.Length > 0)
        {
          foreach (DataRow rowcheck in rowsDelete)
          {
            int status = DBConvert.ParseInt(rowcheck["MStatus"].ToString().Trim());
            if (status >= 2)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0079", "delete");
              return;
            }
          }

          DialogResult dlgr = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0030", "delete");
          if (dlgr == DialogResult.Yes)
          {
            foreach (DataRow row in rowsDelete)
            {
              string materialCode = row["MaterialCode"].ToString().Trim();
              int status = DBConvert.ParseInt(row["MStatus"].ToString().Trim());

              DBParameter[] input = new DBParameter[] { new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode) };
              DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

              DataBaseAccess.ExecuteStoreProcedure(SP_GNR_MATERIALINFO_DELETE, input, output);
              int result = DBConvert.ParseInt(output[0].Value.ToString());
              if (result == 0)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0004");
                return;
              }
            }
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0002");
          }
          else if (dlgr == DialogResult.No)
          {
            return;
          }
        }
        this.Search();
      }
    }

    /// <summary>
    /// btnDisactive Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDisactive_Click(object sender, EventArgs e)
    {
      //Check User
      string message = string.Empty;
      bool success = this.CheckUserLogin(out message);
      if (success == false)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0204", message);
        return;
      }

      if (this.loadData)
      {
        DataTable dtSource = (DataTable)ultData.DataSource;
        DataRow[] rowsDisactive = dtSource.Select("Selected = 1");
        if (rowsDisactive.Length > 0)
        {
          //foreach (DataRow rowcheck in rowsDisactive)
          //{
          //  int status = DBConvert.ParseInt(rowcheck["MStatus"].ToString().Trim());
          //  if (status != 2)
          //  {
          //    Shared.Utility.WindowUtinity.ShowMessageError("ERR0079", "disactive");
          //    return;
          //  }
          //}
          DialogResult dlgr = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0030", "Disactive");
          if (dlgr == DialogResult.Yes)
          {
            foreach (DataRow row in rowsDisactive)
            {
              string materialCode = row["MaterialCode"].ToString().Trim();

              DBParameter[] input = new DBParameter[2];
              input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
              input[1] = new DBParameter("@Status", DbType.Int32, 3);
              DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

              DataBaseAccess.ExecuteStoreProcedure(SP_GNR_MATERIALINFO_CONFIRM, input, output);
              int result = DBConvert.ParseInt(output[0].Value.ToString());
              if (result == 0)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0034");
                return;
              }
            }
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0012");
          }
          else if (dlgr == DialogResult.No)
          {
            return;
          }
        }
        this.Search();
      }
    }

    private void btnDeleteMinMax_Click(object sender, EventArgs e)
    {
      if (this.loadData)
      {
        DataTable dtSource = (DataTable)ultData.DataSource;
        if (dtSource != null && dtSource.Rows.Count > 0)
        {
          DataRow[] rowsConfirm = dtSource.Select("Selected = 1");
          if (rowsConfirm.Length > 0)
          {
            DialogResult dlgr = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0030", "Delete Min-Max");
            if (dlgr == DialogResult.Yes)
            {
              foreach (DataRow row in rowsConfirm)
              {
                string materialCode = row["MaterialCode"].ToString().Trim();

                DBParameter[] input = new DBParameter[1];
                input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
                DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

                DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialInformation_DeleteMinMax", input, output);
                int result = DBConvert.ParseInt(output[0].Value.ToString());
                if (result == 0)
                {
                  Shared.Utility.WindowUtinity.ShowMessageError("ERR0034");
                  return;
                }
              }
              Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0012");
            }
            else if (dlgr == DialogResult.No)
            {
              return;
            }
          }
          // Search Data
          this.Search();
        }
      }
    }

    private void btnActive_Click(object sender, EventArgs e)
    {
      //Check User
      string message = string.Empty;
      bool success = this.CheckUserLogin(out message);
      if (success == false)
      {
        Shared.Utility.WindowUtinity.ShowMessageError("ERR0204", message);
        return;
      }

      if (this.loadData)
      {
        DataTable dtSource = (DataTable)ultData.DataSource;
        DataRow[] rowsDisactive = dtSource.Select("Selected = 1");
        if (rowsDisactive.Length > 0)
        {
          foreach (DataRow rowcheck in rowsDisactive)
          {
            int status = DBConvert.ParseInt(rowcheck["MStatus"].ToString().Trim());
            if (status != 3)
            {
              Shared.Utility.WindowUtinity.ShowMessageError("ERR0079", "Active");
              return;
            }
          }
          DialogResult dlgr = Shared.Utility.WindowUtinity.ShowMessageConfirm("MSG0030", "Active");
          if (dlgr == DialogResult.Yes)
          {
            foreach (DataRow row in rowsDisactive)
            {
              string materialCode = row["MaterialCode"].ToString().Trim();

              DBParameter[] input = new DBParameter[2];
              input[0] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, materialCode);
              input[1] = new DBParameter("@Status", DbType.Int32, 2);
              DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };

              DataBaseAccess.ExecuteStoreProcedure(SP_GNR_MATERIALINFO_CONFIRM, input, output);
              int result = DBConvert.ParseInt(output[0].Value.ToString());
              if (result == 0)
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0034");
                return;
              }
            }
            Shared.Utility.WindowUtinity.ShowMessageSuccess("MSG0012");
          }
          else if (dlgr == DialogResult.No)
          {
            return;
          }
        }
        this.Search();
      }
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        ControlUtility.ExportToExcelWithDefaultPath(ultData, "MaterialList");
      }
    }

    #endregion Event


  }
}
