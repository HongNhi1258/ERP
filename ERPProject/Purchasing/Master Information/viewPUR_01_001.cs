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
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DaiCo.ERPProject
{
  public partial class viewPUR_01_001 : MainUserControl
  {
    #region Field
    private IList listDeleting = new ArrayList();
    private IList listDeleted = new ArrayList();
    private bool isDuplicateGroup = false;
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
      LoadCBMaterialGroup();
      LoadGrid();
      ultData.BeforeCellActivate += new Infragistics.Win.UltraWinGrid.CancelableCellEventHandler(this.ultData_BeforeCellActivate);
    }
    #endregion Init

    #region Function

    private void LoadCBMaterialGroup()
    {
      string cmd = string.Format(@"SELECT Pid Value, DisplayText Display
              FROM VWHDWarehouseList
              ORDER BY DisplayText");
      DataTable dtMaterialGroup = DataBaseAccess.SearchCommandTextDataTable(cmd);

      this.ucbeMaterialGroup.DataSource = dtMaterialGroup;
      this.ucbeMaterialGroup.DisplayMember = "Display";
      ucbeMaterialGroup.ValueMember = "Value";
      ucbeMaterialGroup.CheckedListSettings.CheckBoxStyle = Infragistics.Win.CheckStyle.CheckBox;
      ucbeMaterialGroup.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems;
      ucbeMaterialGroup.CheckedListSettings.ItemCheckArea = Infragistics.Win.ItemCheckArea.Item;
      ucbeMaterialGroup.CheckedListSettings.ListSeparator = ",";
    }


    /// <summary>
    /// Load UltraGrid 
    /// </summary>
    private void LoadGrid()
    {
      this.listDeleting = new ArrayList();
      this.listDeleted = new ArrayList();
      DataTable dtSource = DataBaseAccess.SearchStoreProcedureDataTable("spGNRMaterialGroup_Load");
      ultData.DataSource = dtSource;
      lbCount.Text = string.Format(String.Format("Đếm: {0}", ultData.Rows.FilteredInRowCount > 0 ? ultData.Rows.FilteredInRowCount : 0));
    }

    /// <summary>
    /// HÀM CHECK PROCESS DUPLICATE
    /// </summary>
    private bool CheckGroupDuplicate()
    {
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string comcode = ultData.Rows[i].Cells["Group"].Value.ToString();
        for (int j = i + 1; j < ultData.Rows.Count; j++)
        {
          string comcodeCompare = ultData.Rows[j].Cells["Group"].Value.ToString();
          if (string.Compare(comcode, comcodeCompare, true) == 0)
          {
            ultData.Rows[i].CellAppearance.BackColor = Color.Yellow;
            ultData.Rows[j].CellAppearance.BackColor = Color.Yellow;
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Check Input
    /// </summary>
    /// <returns></returns>
    private bool CheckValid()
    {
      if (this.CheckGroupDuplicate())
      {
        WindowUtinity.ShowMessageErrorFromText("Duplicate Data");
        return false;
      }

      return true;
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
    /// Format UltraGrid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ultData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
    {
      e.Layout.Reset();
      Utility.InitLayout_UltraGrid(ultData);
      e.Layout.Override.HeaderAppearance.FontData.Bold = DefaultableBoolean.True;
      e.Layout.Bands[0].Columns["Group"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Group"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MaxWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].MinWidth = 70;
      e.Layout.Bands[0].Columns["Selected"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
      e.Layout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.TemplateOnBottom;
      e.Layout.Bands[0].Columns["WHPids"].Hidden = true;

      ucbeMaterialGroup.CheckedListSettings.CheckBoxStyle = Infragistics.Win.CheckStyle.CheckBox;
      ucbeMaterialGroup.CheckedListSettings.EditorValueSource = Infragistics.Win.EditorWithComboValueSource.CheckedItems;
      ucbeMaterialGroup.CheckedListSettings.ItemCheckArea = Infragistics.Win.ItemCheckArea.Item;
      ucbeMaterialGroup.CheckedListSettings.ListSeparator = ",";
      e.Layout.Bands[0].Columns["Warehouses"].EditorComponent = ucbeMaterialGroup;
      for (int i = 0; i < ultData.Rows.Count; i++)
      {
        string value = ultData.Rows[i].Cells["Group"].Value.ToString();
        if (value.Length > 0)
        {
          ultData.Rows[i].Cells["Group"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
          ultData.Rows[i].Cells["Name"].Activation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
        }
      }
      e.Layout.Bands[0].Columns["Group"].Header.Caption = "Nhóm hàng";
      e.Layout.Bands[0].Columns["Name"].Header.Caption = "Tên nhóm hàng";
      e.Layout.Bands[0].Columns["Remark"].Header.Caption = "Ghi chú";
      e.Layout.Bands[0].Columns["Warehouses"].Header.Caption = "Kho";
      e.Layout.Bands[0].Columns["Warehouses"].Header.Caption = "Chọn";
    }

    /// <summary>
    /// btnSave Click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {
      if(this.CheckValid())
      {
        bool success = true;
        DataTable dtSource = (DataTable)ultData.DataSource;
        foreach (DataRow row in dtSource.Rows)
        {
          string storeName = string.Empty;
          DBParameter[] input = new DBParameter[4];
          DBParameter[] output = new DBParameter[] { new DBParameter("@Result", DbType.Int32, 0) };
          DBParameter[] outputParam = new DBParameter[] { new DBParameter("@Result", DbType.Int64, 0) };
          if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
          {
            string group = row["Group"].ToString().Trim();
            string name = row["Name"].ToString().Trim();
            string remark = row["Remark"].ToString().Trim();
            string whPids = row["WHPids"].ToString();

            input[0] = new DBParameter("@Group", DbType.AnsiString, 3, group);
            input[1] = new DBParameter("@Name", DbType.String, 128, name);
            input[2] = new DBParameter("@WHPids", DbType.AnsiString, 64, whPids);
            input[3] = new DBParameter("@Remark", DbType.String, 512, remark);

            DataBaseAccess.ExecuteStoreProcedure("spGNRMaterialGroup_Edit", input, outputParam);

            if (outputParam != null && DBConvert.ParseLong(outputParam[0].Value.ToString()) > 0)
            {
              success = true;
            }
            else
            { 
              success = false; 
            }
          }          
        }
        if (success)
        {
          WindowUtinity.ShowMessageSuccess("MSG0004");
          this.LoadGrid();
        }
        else
        {
          WindowUtinity.ShowMessageError("WRN0004");
        }
      }
      else
      {
        this.SaveSuccess = false;
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
          viewPUR_01_002 uc = new viewPUR_01_002();
          uc.materialGroup = ultData.Selected.Rows[0].Cells["Group"].Value.ToString().Trim();
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
  

    private void ultData_BeforeCellActivate(object sender, CancelableCellEventArgs e)
    {
      this.LoadCBMaterialGroup();
    }
  

    private void ultData_AfterCellUpdate(object sender, CellEventArgs e)
    {
      string colName = e.Cell.Column.ToString();
      switch (colName)
      {
        case "Warehouses":
          {
            CheckedValueListItemsCollection chekedItems = ucbeMaterialGroup.CheckedItems;
            DataTable dt = new DataTable();
            dt.Columns.Add("WHPids", typeof(String));
            foreach (ValueListItem item in chekedItems)
            {

              DataRow row_ = dt.NewRow();
              row_[0] = item.DataValue;
              dt.Rows.Add(row_);
            }
            string warehouses = string.Empty;
            for (int l = 0; l < dt.Rows.Count; l++)
            {
              if (l == 0)
              {
                warehouses = dt.Rows[l]["WHPids"].ToString();
              }
              else
              {
                warehouses = warehouses + "," + dt.Rows[l]["WHPids"].ToString();
              }

            }
            e.Cell.Row.Cells["WHPids"].Value = warehouses;
            break;
          }
        default:
          break;
      }
    }
    #endregion Event   
  }
}
