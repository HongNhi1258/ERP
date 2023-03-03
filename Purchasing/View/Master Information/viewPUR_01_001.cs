/*
  Author      : Vo Van Duy Qui
  Date        : 10/03/2011
  Description : Edit Material Group
  Update By   :  (04/05/2011)
*/

using DaiCo.Application;
using DaiCo.Shared;
using DaiCo.Shared.DataBaseUtility;
using DaiCo.Shared.Utility;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;

namespace DaiCo.Purchasing
{
  public partial class viewPUR_01_001 : MainUserControl
  {
    #region Field
    private IList listDeleting = new ArrayList();
    private IList listDeleted = new ArrayList();
    #endregion Field

    #region Init

    /// <summary>
    /// Itit Form
    /// </summary>
    public viewPUR_01_001()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Form Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void viewPUR_01_001_Load(object sender, EventArgs e)
    {
      // Load ComboBox Warehouse
      this.LoadComboBox();
    }
    #endregion Init

    #region Function

    /// <summary>
    /// Load ComboBox Warehouse
    /// </summary>
    private void LoadComboBox()
    {
      string commandText = "SELECT Pid, Name FROM TblWHDWarehouse";
      DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
      ultComboWareHouse.DataSource = dtSource;
      ultComboWareHouse.DisplayMember = "Name";
      ultComboWareHouse.ValueMember = "Pid";
      ultComboWareHouse.DisplayLayout.Bands[0].Columns["Pid"].Hidden = true;
      ultComboWareHouse.DisplayLayout.Bands[0].ColHeadersVisible = false;
    }

    /// <summary>
    /// Load UltraGrid 
    /// </summary>
    private void LoadGrid()
    {
      if (ultComboWareHouse.SelectedRow.Index >= 0)
      {
        this.listDeleting = new ArrayList();
        this.listDeleted = new ArrayList();
        long pidWareHouse = DBConvert.ParseLong(ultComboWareHouse.SelectedRow.Cells["Pid"].Value.ToString());
        string commandText = string.Format("SELECT [Group], Name, Remark, 0 AS Selected FROM TblGNRMaterialGroup WHERE Warehouse = {0}", pidWareHouse);
        DataTable dtSource = DataBaseAccess.SearchCommandTextDataTable(commandText);
        dtSource.Columns["Selected"].DefaultValue = 0; // Add HangNguyen
        ultData.DataSource = dtSource;
      }
      else
      {
        ultData.DataSource = null;
      }
    }

    /// <summary>
    /// Check Input
    /// </summary>
    /// <param name="warehouse"></param>
    /// <param name="dtSource"></param>
    /// <returns></returns>
    private bool ValidationInput(long warehouse, DataTable dtSource)
    {
      bool result = true;
      foreach (DataRow row in dtSource.Rows)
      {
        string storeName = string.Empty;
        DBParameter[] input = new DBParameter[4];
        DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
        {
          string group = string.Empty;
          long check = long.MinValue;
          group = row["Group"].ToString().Trim();
          if (group.Length == 0)
          {
            WindowUtinity.ShowMessageError("WRN0013", "Group");
            result = false;
            return result;
          }
          else if (DBConvert.ParseLong(group) == long.MinValue || group.Length < 3 || DBConvert.ParseLong(group) < 1)
          {
            WindowUtinity.ShowMessageError("ERR0073", group);
            result = false;
            return result;
          }

          input[0] = new DBParameter("@Warehouse", DbType.Int64, warehouse);
          input[1] = new DBParameter("@Group", DbType.AnsiString, 3, group);
          int count = int.MinValue;
          count = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText("SELECT dbo.FPURCheckGroupInMaterialGroup(@Warehouse,@Group)", input));
          if (row.RowState == DataRowState.Added)
          {
            if (count == 1)
            {
              WindowUtinity.ShowMessageError("ERR0028", group);
              result = false;
              return result;
            }
          }
          else
          {
            if (count > 1)
            {
              WindowUtinity.ShowMessageError("ERR0028", group);
              result = false;
              return result;
            }
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Check Delete
    /// </summary>
    /// <returns></returns>
    private bool ValidateDelete()
    {
      bool result = true;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
        if (selected == 1)
        {
          string group = ultData.Rows[i].Cells["Group"].Value.ToString();
          DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Group", DbType.AnsiString, 3, group) };
          int count = int.MinValue;
          count = DBConvert.ParseInt(DataBaseAccess.ExecuteScalarCommandText("SELECT dbo.FPURCheckGroupInMaterialCategory(@Group)", inputParam));
          if (count == 1)
          {
            WindowUtinity.ShowMessageError("ERR0054", group);
            result = false;
            break;
          }
        }
      }
      return result;
    }
    #endregion Function

    #region Event
    /// <summary>
    /// ultComboWareHouse Value Changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultComboWareHouse_ValueChanged(object sender, EventArgs e)
    {
      // Load Grid
      this.LoadGrid();
    }

    /// <summary>
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;

      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string value = ultData.Rows[i].Cells["Group"].Value.ToString();
        if (value.Length > 0)
        {
          ultData.Rows[i].Cells["Group"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
          ultData.Rows[i].Cells["Name"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
    }

    /// <summary>
    /// btnSave Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if (ultComboWareHouse.SelectedRow != null)
      {
        long warehouse = DBConvert.ParseLong(ultComboWareHouse.SelectedRow.Cells["Pid"].Value.ToString());
        DataTable dtSource = (DataTable)ultData.DataSource;
        bool checkSave = ValidationInput(warehouse, dtSource);
        if (checkSave)
        {
          foreach (DataRow row in dtSource.Rows)
          {
            string storeName = string.Empty;
            DBParameter[] input = new DBParameter[4];
            DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };

            if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
            {
              string group = row["Group"].ToString().Trim();
              string name = row["Name"].ToString().Trim();
              string remark = row["Remark"].ToString().Trim();

              input[0] = new DBParameter("@Group", DbType.AnsiString, 3, group);
              input[1] = new DBParameter("@Name", DbType.String, 128, name);
              input[2] = new DBParameter("@Warehouse", DbType.Int64, warehouse);
              input[3] = new DBParameter("@Remark", DbType.String, 512, remark);

              if (row.RowState == DataRowState.Added)
              {
                storeName = "spGNRMaterialGroup_Insert";
              }
              else if (row.RowState == DataRowState.Modified)
              {
                storeName = "spGNRMaterialGroup_Update";
              }

              if (storeName.Length > 0)
              {
                DataBaseAccess.ExecuteStoreProcedure(storeName, input);
              }
            }
          }
          WindowUtinity.ShowMessageSuccess("MSG0004");

          // Load Grid
          this.LoadGrid();
        }
      }
    }

    /// <summary>
    /// ultData Mouse Double Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      if (ultData.Rows.Count > 0 && ultData.Selected.Rows.Count > 0)
      {
        string group = ultData.Selected.Rows[0].Cells["Group"].Value.ToString().Trim();
        if (group.Length > 0)
        {
          long warehouse = DBConvert.ParseLong(ultComboWareHouse.SelectedRow.Cells["Pid"].Value.ToString());
          viewPUR_01_002 uc = new viewPUR_01_002();
          uc.materialGroup = ultData.Selected.Rows[0].Cells["Group"].Value.ToString().Trim();
          uc.wareHouse = warehouse;
          Shared.Utility.WindowUtinity.ShowView(uc, "MATERIAL CATEGORY LIST", false, DaiCo.Shared.Utility.ViewState.MainWindow);
        }
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
    /// Set Max Length
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
    {
      e.Band.Columns["Group"].FieldLen = 3;
    }

    /// <summary>
    /// Delete Group
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelete_Click(object sender, EventArgs e)
    {
      if (ultData.Rows.Count > 0)
      {
        int countCheck = 0;
        for (int i = 0; i < ultData.Rows.Count; i++)
        {
          int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
          if (selected == 1)
          {
            countCheck++;
          }
        }

        if (countCheck == 0)
        {
          return;
        }

        DialogResult result = WindowUtinity.ShowMessageConfirm("MSG0015");
        if (result == DialogResult.Yes)
        {
          bool check = ValidateDelete();
          if (check)
          {
            for (int i = 0; i < ultData.Rows.Count; i++)
            {
              int selected = DBConvert.ParseInt(ultData.Rows[i].Cells["Selected"].Value.ToString());
              if (selected == 1)
              {
                string group = ultData.Rows[i].Cells["Group"].Value.ToString();
                DBParameter[] inputParam = new DBParameter[] { new DBParameter("@Group", DbType.AnsiString, 3, group) };
                DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialGroupCategory_Delete", inputParam);
              }
            }
            WindowUtinity.ShowMessageSuccess("MSG0002");
          }
          // Load Grid
          this.LoadGrid();
        }
      }
    }

    private void ultData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
    {
      string colName = e.Cell.Column.ToString().Trim();
      string values = e.NewValue.ToString();
      switch (colName)
      {
        case "Group":
          {
            if (DBConvert.ParseLong(values) != long.MinValue || DBConvert.ParseInt(values) > 1 || values.Length == 3)
            {
              string cm = string.Format(@"SELECT [Group]
                          FROM TblGNRMaterialGroup
                          WHERE [Group] = '{0}'", values);
              DataTable dtCheck = DataBaseAccess.SearchCommandTextDataTable(cm);
              if (dtCheck != null && dtCheck.Rows.Count > 0)
              {
                WindowUtinity.ShowMessageError("ERR0028", "Group " + values);
                e.Cancel = true;
              }
            }
          }
          break;
        default:
          break;
      }
    }
    #endregion Event   
  }
}
