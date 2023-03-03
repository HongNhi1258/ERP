/*
  Author        : Võ Hoa Lư
  Create date   : 16/06/2011
  Decription    : Control Material
 */
using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Data;
using System.Drawing;

namespace DaiCo.Planning
{
  public partial class viewPLN_07_010 : MainUserControl
  {
    private bool loadCheck = true;
    #region Init Data
    /// <summary>
    /// Init usercontrol
    /// </summary>
    public viewPLN_07_010()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Load form, this event run when form is started
    /// </summary>
    private void viewPLN_07_010_Load(object sender, EventArgs e)
    {
      ControlUtility.LoadUltraComboMaterialGroup(ucmbGroup, true);
      this.LoadControlType();
      this.LoadDDDept();
    }
    #endregion Init Data

    #region Process Data
    /// <summary>
    /// Find and show data result
    /// </summary>
    private void LoadData()
    {
      DBParameter[] input = new DBParameter[2];
      string value = (ucmbGroup.SelectedRow != null) ? ucmbGroup.SelectedRow.Cells["Group"].Value.ToString().Trim() : string.Empty;
      if (value.Length > 0)
      {
        input[0] = new DBParameter("@Group", DbType.AnsiString, 3, value);
      }
      value = txtMaterialCode.Text.Trim();
      if (value.Length > 0)
      {
        input[1] = new DBParameter("@MaterialCode", DbType.AnsiString, 16, string.Format("%{0}%", value));
      }

      DataTable dataSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMaterialControlInfomation_Select_PMISDB", input);
      ultData.DataSource = dataSource;

      int count = ultData.DisplayLayout.Bands[0].Columns.Count;
      for (int i = 0; i < count; i++)
      {
        string name = ultData.DisplayLayout.Bands[0].Columns[i].ToString();
        if (string.Compare(name, "Control", true) != 0 && string.Compare(name, "ControlType", true) != 0 && string.Compare(name, "DepartmentCode", true) != 0)
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.ActivateOnly;
        }
        else
        {
          ultData.DisplayLayout.Bands[0].Columns[i].CellActivation = Activation.AllowEdit;
          ultData.DisplayLayout.Bands[0].Columns[i].CellAppearance.BackColor = Color.LightCyan;
        }
      }

      count = ultData.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        bool isLocked = row.Cells["Lock"].Value.ToString().Equals("1");
        if (isLocked)
        {
          row.Cells["Control"].Activation = Activation.ActivateOnly;
          row.Cells["ControlType"].Activation = Activation.ActivateOnly;
          row.Cells["DepartmentCode"].Activation = Activation.ActivateOnly;
          row.Appearance.BackColor = Color.LightGray;
        }
        else
        {
          int control = DBConvert.ParseInt(row.Cells["Control"].Value.ToString());
          row.Cells["ControlType"].Activation = (control == 1) ? Activation.AllowEdit : Activation.ActivateOnly;
          row.Cells["DepartmentCode"].Activation = (control == 1) ? Activation.AllowEdit : Activation.ActivateOnly;
        }
      }
    }
    /// <summary>
    /// Bind control type
    /// </summary>
    private void LoadControlType()
    {
      DataTable dataSource = new DataTable();
      dataSource.Columns.Add("Value", typeof(System.Int32));
      dataSource.Columns.Add("Text", typeof(System.String));
      DataRow row = dataSource.NewRow();
      dataSource.Rows.Add(row);
      row = dataSource.NewRow();
      row["Value"] = 0;
      row["Text"] = "WO";
      dataSource.Rows.Add(row);
      row = dataSource.NewRow();
      row["Value"] = 1;
      row["Text"] = "WO & Item";
      dataSource.Rows.Add(row);
      udrpControlType.DataSource = dataSource;

      udrpControlType.ValueMember = "Value";
      udrpControlType.DisplayMember = "Text";
      udrpControlType.DisplayLayout.Bands[0].ColHeadersVisible = false;
      udrpControlType.DisplayLayout.Bands[0].Columns["Value"].Hidden = true;
    }

    private void LoadDDDept()
    {
      string commandtext = @"SELECT DISTINCT [Description] Department
	                              FROM TblBOMCodeMaster
	                              WHERE [Group] = 16084";
      DataTable dt = DataBaseAccess.SearchCommandTextDataTable(commandtext);
      ControlUtility.LoadUltraDropDown(ultddDept, dt, "Department", "Department");
      ultddDept.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    private bool CheckValidData()
    {
      int count = ultData.Rows.Count;
      for (int i = 0; i < count; i++)
      {
        UltraGridRow row = ultData.Rows[i];
        int control = DBConvert.ParseInt(row.Cells["Control"].Value.ToString());
        if (control == 1)
        {
          int controlType = DBConvert.ParseInt(row.Cells["ControlType"].Value.ToString());
          if (controlType != 0 && controlType != 1)
          {
            Shared.Utility.WindowUtinity.ShowMessageError("MSG0011", "Control Type");
            row.Selected = true;
            return false;
          }
          else
          {
            int lockMaterial = DBConvert.ParseInt(row.Cells["Lock"].Value.ToString());
            if (lockMaterial == 0)
            {
              string materialCode = row.Cells["MaterialCode"].Value.ToString();
              DBParameter[] inputParam = new DBParameter[] { new DBParameter("@MaterialCode", DbType.AnsiString, 16, string.Format("%{0}%", materialCode)) };
              DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spPLNMaterialControlInfomation_Select_PMISDB", inputParam);
              if (dtSource != null && dtSource.Rows.Count > 0 && dtSource.Rows[0]["Lock"].ToString() == "1")
              {
                Shared.Utility.WindowUtinity.ShowMessageError("ERR0001", string.Format("Material at row {0}", i + 1));
                row.Selected = true;
                return false;
              }
            }
          }
        }
      }
      return true;
    }
    #endregion Process Data

    #region Events
    /// <summary>
    /// Find and show result data from condition group, category, material code
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSearch_Click(object sender, EventArgs e)
    {
      this.LoadData();
    }

    /// <summary>
    /// Hiden/show, format and resize column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
    {
      Shared.Utility.ControlUtility.SetPropertiesUltraGrid(ultData);
      e.Layout.AutoFitColumns = true;
      e.Layout.Bands[0].Columns["Lock"].Hidden = true;

      e.Layout.Bands[0].Columns["MaterialCode"].Header.Caption = "Material Code";
      e.Layout.Bands[0].Columns["MaterialCode"].MinWidth = 110;
      e.Layout.Bands[0].Columns["MaterialCode"].MaxWidth = 110;

      e.Layout.Bands[0].Columns["EnglishName"].Header.Caption = "English Name";
      e.Layout.Bands[0].Columns["VietNameseName"].Header.Caption = "VietNamese Name";

      e.Layout.Bands[0].Columns["Control"].Header.Caption = "Control";
      e.Layout.Bands[0].Columns["Control"].MinWidth = 60;
      e.Layout.Bands[0].Columns["Control"].MaxWidth = 60;
      e.Layout.Bands[0].Columns["Control"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;

      e.Layout.Bands[0].Columns["ControlType"].Header.Caption = "Control Type";
      e.Layout.Bands[0].Columns["ControlType"].MinWidth = 100;
      e.Layout.Bands[0].Columns["ControlType"].MaxWidth = 100;
      e.Layout.Bands[0].Columns["ControlType"].ValueList = udrpControlType;

      e.Layout.Bands[0].Columns["DepartmentCode"].Header.Caption = "Department Code";
      e.Layout.Bands[0].Columns["DepartmentCode"].MinWidth = 150;
      e.Layout.Bands[0].Columns["DepartmentCode"].MaxWidth = 150;
      e.Layout.Bands[0].Columns["DepartmentCode"].ValueList = ultddDept;

      e.Layout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
    }

    /// <summary>
    /// Change control --> change activation control type 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_CellChange(object sender, CellEventArgs e)
    {
      string columnName = e.Cell.Column.ToString().Trim();
      string value = e.Cell.Text.ToString();
      if (string.Compare(columnName, "Control", true) == 0)
      {
        if (DBConvert.ParseInt(value) == 1)
        {
          e.Cell.Row.Cells["ControlType"].Activation = Activation.AllowEdit;
          e.Cell.Row.Cells["DepartmentCode"].Activation = Activation.AllowEdit;
        }
        else
        {
          e.Cell.Row.Cells["ControlType"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["ControlType"].Value = DBNull.Value;
          e.Cell.Row.Cells["DepartmentCode"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["DepartmentCode"].Value = DBNull.Value;
          this.loadCheck = false;
          chkAll.Checked = false;
          this.loadCheck = true;
        }
      }
      if (string.Compare(columnName, "ControlType", true) == 0)
      {
        if (value.Length > 0)
        {
          e.Cell.Row.Cells["DepartmentCode"].Activation = Activation.AllowEdit;
        }
        else
        {
          e.Cell.Row.Cells["DepartmentCode"].Activation = Activation.ActivateOnly;
          e.Cell.Row.Cells["DepartmentCode"].Value = DBNull.Value;
        }
      }
    }

    /// <summary>
    /// Save data on screen in to data base
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (!this.CheckValidData())
      {
        return;
      }
      DataTable dataSource = (DataTable)ultData.DataSource;
      string commandText = string.Format(@"SELECT ID_SanPham MaterialCode, IsControl Control, ControlType, DepartmentCode FROM VWHMaterialInfomation_PMISDB");
      bool success = DataBaseAccess.UpdateDataTable(dataSource, commandText);
      if (success)
      {
        WindowUtinity.ShowMessageSuccess("MSG0004");
      }
      else
      {
        WindowUtinity.ShowMessageError("ERR0005");
      }
      this.LoadData();
    }

    /// <summary>
    /// Close screen
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.CloseTab();
    }

    /// <summary>
    /// Selected all Material
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkAll_CheckedChanged(object sender, EventArgs e)
    {
      if (this.loadCheck)
      {
        int check = (chkAll.Checked ? 1 : 0);
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          if (!ultData.Rows[i].Cells["Lock"].Value.ToString().Equals("1"))
          {
            ultData.Rows[i].Cells["Control"].Value = check;
            if (check == 0)
            {
              ultData.Rows[i].Cells["ControlType"].Activation = Activation.ActivateOnly;
              ultData.Rows[i].Cells["ControlType"].Value = DBNull.Value;
              ultData.Rows[i].Cells["DepartmentCode"].Activation = Activation.ActivateOnly;
              ultData.Rows[i].Cells["DepartmentCode"].Value = DBNull.Value;
            }
            else
            {
              ultData.Rows[i].Cells["ControlType"].Activation = Activation.AllowEdit;
              ultData.Rows[i].Cells["DepartmentCode"].Activation = Activation.AllowEdit;
            }
          }
        }
      }
    }
    #endregion Events
  }
}
